// https://ffmpeg.org/doxygen/trunk/encode__video_8c_source.html
// https://github.com/FFmpeg/FFmpeg/blob/master/doc/examples/encode_video.c

// https://stackoverflow.com/a/4281147

#include <spectroscope/Encoder.h>

Encoder::Encoder(const std::filesystem::path& filename, const size_t frameheight, const size_t framewidth, const int framerate) :
  filename(filename),
  frameheight(frameheight),
  framewidth(framewidth),
  framerate(framerate),
  framenumber(0)
{
  int error;

  const AVCodec* codec = avcodec_find_encoder_by_name("libvpx-vp9");
  AVAssert("Codec \"libvpx-vp9\" not found!", codec);

  context.sws = SwsContextPointer(sws_getContext(
    framewidth, frameheight, AV_PIX_FMT_BGR24,
    framewidth, frameheight, AV_PIX_FMT_YUV420P,
    SWS_FAST_BILINEAR, nullptr, nullptr, nullptr));
  AVAssert("Could not initialize sws context!", context.sws.get());

  AVFormatContext* format = nullptr;
  error = avformat_alloc_output_context2(&format, nullptr, nullptr, filename.c_str());
  AVAssert("Could not initialize format context!", error);
  context.format = AVFormatContextPointer(format);

  context.codec = AVCodecContextPointer(avcodec_alloc_context3(codec));
  AVAssert("Could not initialize codec context!", context.codec.get());
  context.codec->height = frameheight;
  context.codec->width = framewidth;
  context.codec->time_base = { 1, framerate };
  context.codec->framerate = { framerate, 1 };
  context.codec->pix_fmt = AV_PIX_FMT_YUV420P;
  av_opt_set(context.codec->priv_data, "preset", "ultrafast", 0);
  av_opt_set(context.codec->priv_data, "crf", "32", 0);
  error = avcodec_open2(context.codec.get(), codec, nullptr);
  AVAssert("Could not open codec!", error);

  stream.video = avformat_new_stream(context.format.get(), codec);
  AVAssert("Could not initialize video stream!", stream.video);
  avcodec_parameters_from_context(stream.video->codecpar, context.codec.get());
  av_dump_format(context.format.get(), 0, filename.c_str(), 1);

  frame.bgr = AVFramePointer(av_frame_alloc());
  AVAssert("Could not allocate AVFrame!", frame.bgr.get());
  frame.bgr->format = AV_PIX_FMT_BGR24;
  frame.bgr->height = frameheight;
  frame.bgr->width = framewidth;
  error = av_frame_get_buffer(frame.bgr.get(), 1);
  AVAssert("Could not allocate AVFrame!", error);

  frame.yuv = AVFramePointer(av_frame_alloc());
  AVAssert("Could not allocate AVFrame!", frame.yuv.get());
  frame.yuv->format = AV_PIX_FMT_YUV420P;
  frame.yuv->height = frameheight;
  frame.yuv->width = framewidth;
  error = av_frame_get_buffer(frame.yuv.get(), 1);
  AVAssert("Could not allocate AVFrame!", error);

  packet = AVPacketPointer(av_packet_alloc());
  AVAssert("Could not allocate AVPacket!", packet.get());
}

void Encoder::open()
{
  int error;

  error = avio_open(&context.format->pb, filename.c_str(), AVIO_FLAG_WRITE);
  AVAssert(error);

  error = avformat_write_header(context.format.get(), nullptr);
  AVAssert(error);
}

void Encoder::close()
{
  int error;

  error = av_write_trailer(context.format.get());
  AVAssert(error);

  error = avio_closep(&context.format->pb);
  AVAssert(error);
}

void Encoder::encode(const std::span<uint8_t> video)
{
  int error;

  std::copy(video.data(), video.data() + video.size(), frame.bgr->data[0]);

  error = sws_scale(
    context.sws.get(),
    frame.bgr->data, frame.bgr->linesize,
    0, frameheight,
    frame.yuv->data, frame.yuv->linesize);
  AVAssert(error);

  frame.yuv->pts = framenumber++;

  error = avcodec_send_frame(context.codec.get(), frame.yuv.get());
  AVAssert(error);

  while (error >= 0)
  {
    error = avcodec_receive_packet(context.codec.get(), packet.get());

    if (error == AVERROR(EAGAIN) || error == AVERROR_EOF)
    {
      return;
    }
    else
    {
      AVAssert(error);
    }

    av_packet_rescale_ts(packet.get(), { 1, framerate }, stream.video->time_base);

    packet.get()->stream_index = stream.video->index;

    error = av_interleaved_write_frame(context.format.get(), packet.get());
    AVAssert(error);

    av_packet_unref(packet.get());
  }
}
