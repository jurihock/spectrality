#pragma once

#include <spectroscope/FFmpeg.h>

#include <filesystem>
#include <span>

class Encoder
{

public :

  Encoder(const std::filesystem::path& filename, const size_t frameheight, const size_t framewidth, const int framerate, const int samplerate);

  void open();
  void close();

  void encode(const std::span<uint8_t> video);

private:

  const std::filesystem::path filename;
  const size_t frameheight;
  const size_t framewidth;
  const int framerate;
  const int samplerate;
  const int modulo;

  int64_t framenumber;
  int64_t samplenumber;

  struct
  {
    SwsContextPointer sws;
    AVFormatContextPointer format;
    AVCodecContextPointer codec;
  }
  context;

  struct
  {
    AVStream* video;
  }
  stream;

  struct
  {
    AVFramePointer bgr;
    AVFramePointer yuv;
  }
  frame;

  AVPacketPointer packet;

};
