#pragma once

#include <spectroscope/FFmpeg.h>

#include <filesystem>
#include <span>

class Encoder
{

public :

  Encoder(const std::filesystem::path& filename, const size_t frameheight, const size_t framewidth, const int framerate = 25);

  void open();
  void close();

  void encode(const std::span<uint8_t> video);

private:

  const std::filesystem::path filename;
  const size_t frameheight;
  const size_t framewidth;
  const int framerate;

  uint64_t framenumber;

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
