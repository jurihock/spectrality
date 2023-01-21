#include <spectroscope/Audio.h>

#include <dr_wav.h>

Audio::Audio(const std::filesystem::path& filename) :
  filename(filename)
{
}

void Audio::read(std::vector<double>& doubles, int& samplerate, const int channel)
{
  drwav wav;

  if (drwav_init_file(&wav, filename.c_str(), nullptr) != DRWAV_TRUE)
  {
    throw std::runtime_error(filename);
  }

  const size_t samples = wav.totalPCMFrameCount;
  const size_t channels = wav.channels;
  const size_t bytes = samples * channels * sizeof(float);

  if (bytes > DRWAV_SIZE_MAX)
  {
    drwav_uninit(&wav);

    throw std::runtime_error(filename);
  }

  std::vector<float> floats(samples * channels);

  if (drwav_read_pcm_frames_f32(&wav, samples, floats.data()) != samples)
  {
    drwav_uninit(&wav);

    throw std::runtime_error(filename);
  }

  drwav_uninit(&wav);

  if (channels > 1)
  {
    for (size_t i = 0; i < samples; ++i)
    {
      floats[i] = floats[i * channels];

      for (size_t j = 1; j < channels; ++j)
      {
        floats[i] += floats[i * channels + j];
      }

      floats[i] /= channels;
    }

    floats.resize(samples);
  }

  doubles.resize(floats.size());
  std::copy(floats.begin(), floats.end(), doubles.begin());
  samplerate = wav.sampleRate;
}

/*

TODO: read audio file via ffmpeg

#include <iostream>

void Audio::read(std::vector<double>& samples, int& samplerate, const int channel)
{
  int error;

  struct
  {
    AVFormatContextPointer format;
    AVCodecContextPointer codec;
    SwsContextPointer sws;
  }
  context;

  AVFormatContext* format = nullptr;
  error = avformat_open_input(&format, filename.c_str(), nullptr, nullptr);
  AVAssert("Could not initialize format context!", error);
  context.format = AVFormatContextPointer(format);

  error = avformat_find_stream_info(context.format.get(), nullptr);
  AVAssert("Could not find stream info!", error);

  const AVCodec* codec = nullptr;
  error = av_find_best_stream(context.format.get(), AVMEDIA_TYPE_AUDIO, -1, -1, &codec, 0);
  AVAssert("Could not find appropriate codec!", error);
  AVAssert("Could not find appropriate codec!", codec);
  const int stream = error;

  context.codec = AVCodecContextPointer(avcodec_alloc_context3(codec));
  AVAssert("Could not initialize codec context!", context.codec.get());

  error = avcodec_parameters_to_context(context.codec.get(), context.format->streams[stream]->codecpar);
  AVAssert("Could not set codec context parameters!", error);

  error = avcodec_open2(context.codec.get(), codec, nullptr);
  AVAssert("Could not open codec!", error);

  // if (!context.codec->channel_layout)
  // {
  //   context.codec->channel_layout = av_get_default_channel_layout(context.codec->channels);
  // }

  // samplerate = context.codec->sample_rate;

  // context.sws = SwsContextPointer(swr_alloc_set_opts(
  //     nullptr,
  //     context.codec->channel_layout,
  //     AV_SAMPLE_FMT_DBL,
  //     context.codec->sample_rate,
  //     context.codec->channel_layout,
  //     context.codec->sample_fmt,
  //     context.codec->sample_rate,
  //     0,
  //     nullptr));
  // AVAssert("Could not initialize sws context!", context.sws.get());

  AVFramePointer frame = AVFramePointer(av_frame_alloc());
  AVAssert("Could not allocate AVFrame!", frame.get());
  // error = av_frame_get_buffer(frame.get(), 1);
  // AVAssert("Could not allocate AVFrame!", error);

  AVPacketPointer packet = AVPacketPointer(av_packet_alloc());
  AVAssert("Could not allocate AVPacket!", packet.get());

  int i = -1;

  while(++i < 100)
  {
    error = av_read_frame(context.format.get(), packet.get());

    if (error == AVERROR(EAGAIN) || error == AVERROR_EOF)
    {
      std::cout << "WTF: EAGAIN AVERROR_EOF" << std::endl;

      break;
    }
    else
    {
      AVAssert(error);
    }

    if (packet->stream_index != stream)
    {
      std::cout << "WTF: packet->stream_index != stream" << std::endl;
    }

    // error = avcodec_send_packet(context.codec.get(), packet.get());
    // AVAssert(error);

    // while (error >= 0)
    // {
    //   error = avcodec_receive_frame(context.codec.get(), frame.get());
    //   AVAssert(error);

    //   std::cout << "avcodec_receive_frame" << std::endl;
    // }

    // av_packet_unref(packet.get());

    std::cout << i << " " << (packet->stream_index == stream) << std::endl;
  }
}

*/
