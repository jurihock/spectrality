// https://ffmpeg.org/doxygen/trunk/encode__video_8c_source.html
// https://github.com/FFmpeg/FFmpeg/blob/master/doc/examples/encode_video.c

// https://stackoverflow.com/a/4281147

#include <spectroscope/Encoder.h>

Encoder::Encoder(const std::string& filename, const size_t frameheight, const size_t framewidth, const int framerate) :
  filename(filename),
  frameheight(frameheight),
  framewidth(framewidth),
  framerate(framerate),
  framenumber(0)
{
  int error;

  const AVCodec* codec = avcodec_find_encoder_by_name("libvpx-vp9");

  if (!codec)
  {
    throw std::runtime_error("Codec \"libvpx-vp9\" not found!");
  }

  context.sws = sws_getContext(
    framewidth, frameheight, AV_PIX_FMT_BGR24,
    framewidth, frameheight, AV_PIX_FMT_YUV420P,
    SWS_FAST_BILINEAR, nullptr, nullptr, nullptr);

  if (!context.sws)
  {
    throw std::runtime_error("Could not initialize sws context!");
  }

  error = avformat_alloc_output_context2(&context.format, nullptr, nullptr, filename.c_str());

  if (error < 0)
  {
    avformat_alloc_output_context2(&context.format, nullptr, "mpeg", filename.c_str());
  }

  if (error < 0)
  {
    throw std::runtime_error("Could not initialize format context!");
  }

  context.codec = avcodec_alloc_context3(codec);

  if (!context.codec)
  {
    throw std::runtime_error("Could not initialize codec context!");
  }

  context.codec->height = frameheight;
  context.codec->width = framewidth;
  context.codec->time_base = { 1, framerate };
  context.codec->framerate = { framerate, 1 };
  context.codec->pix_fmt = AV_PIX_FMT_YUV420P;

  av_opt_set(context.codec->priv_data, "preset", "ultrafast", 0);
  av_opt_set(context.codec->priv_data, "crf", "32", 0);

  error = avcodec_open2(context.codec, codec, NULL);

  if (error < 0)
  {
    throw std::runtime_error("Could not initialize codec: " + std::string(av_err2str(error)) + "!");
  }

  stream.video = avformat_new_stream(context.format, codec);

  if (!stream.video)
  {
    throw std::runtime_error("Could not initialize video stream!");
  }

  stream.video->codecpar->codec_type = AVMEDIA_TYPE_VIDEO;
  stream.video->codecpar->codec_id = codec->id;
  stream.video->codecpar->height = frameheight;
  stream.video->codecpar->width = framewidth;

  av_dump_format(context.format, 0, filename.c_str(), 1);
  avio_open(&context.format->pb, filename.c_str(), AVIO_FLAG_WRITE);

  error = avformat_write_header(context.format, nullptr);

  if (error < 0)
  {
    throw std::runtime_error("Could not initialize video stream: " + std::string(av_err2str(error)) + "!");
  }

  frame.bgr = av_frame_alloc();

  if (!frame.bgr)
  {
    throw std::runtime_error("Could not allocate AVFrame!");
  }

  frame.bgr->format = AV_PIX_FMT_BGR24;
  frame.bgr->height = frameheight;
  frame.bgr->width = framewidth;
  error = av_frame_get_buffer(frame.bgr, 1);

  if (error < 0)
  {
    throw EncoderError("Could not allocate AVFrame!", error);
  }

  frame.yuv = av_frame_alloc();

  if (!frame.yuv)
  {
    throw std::runtime_error("Could not allocate AVFrame!");
  }

  frame.yuv->format = AV_PIX_FMT_YUV420P;
  frame.yuv->height = frameheight;
  frame.yuv->width = framewidth;
  error = av_frame_get_buffer(frame.yuv, 1);

  if (error < 0)
  {
    throw EncoderError("Could not allocate AVFrame!", error);
  }

  packet = av_packet_alloc();

  if (!packet)
  {
    throw std::runtime_error("Could not allocate AVPacket!");
  }
}

Encoder::~Encoder()
{
  av_packet_free(&packet);
  av_frame_free(&frame.yuv);
  av_frame_free(&frame.bgr);
  avcodec_free_context(&context.codec);
  avformat_free_context(context.format);
}

void Encoder::operator()()
{
  av_write_trailer(context.format);
  avio_closep(&context.format->pb);
}

void Encoder::operator()(std::span<uint8_t> video)
{
  sws_scale(
    context.sws,
    frame.bgr->data, frame.bgr->linesize,
    0, frameheight,
    frame.yuv->data, frame.yuv->linesize);

  frame.yuv->pts = framenumber++;

  int error;

  error = avcodec_send_frame(context.codec, frame.yuv);

  if (error < 0)
  {
    throw EncoderError(error);
  }

  while (error >= 0)
  {
    error = avcodec_receive_packet(context.codec, packet);

    if (error == AVERROR(EAGAIN) || error == AVERROR_EOF)
    {
      return;
    }

    if (error < 0)
    {
      throw EncoderError(error);
    }

    av_packet_rescale_ts(packet, { 1, framerate }, stream.video->time_base);

    packet->stream_index = stream.video->index;

    av_interleaved_write_frame(context.format, packet);

    av_packet_unref(packet);
  }
}
