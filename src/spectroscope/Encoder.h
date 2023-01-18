#pragma once

#include <memory>
#include <span>
#include <string>
#include <vector>

extern "C"
{
  #include <libavcodec/avcodec.h>
  #include <libavformat/avformat.h>
  #include <libavutil/opt.h>
  #include <libswresample/swresample.h>
  #include <libswscale/swscale.h>
}

class EncoderError : public std::runtime_error
{

public:

  EncoderError(const std::string& what) :
    std::runtime_error(what)
  {
  }

  EncoderError(const std::string& what, const int errorcode) :
    std::runtime_error(what + "(" + std::string(av_err2str(errorcode)) + ")")
  {
  }

  EncoderError(const int errorcode) :
    std::runtime_error(std::string(av_err2str(errorcode)))
  {
  }

};

class Encoder
{

public :

  Encoder(const std::string& filename, const size_t frameheight, const size_t framewidth, const int framerate = 25);
  ~Encoder();

  void operator()(std::span<uint8_t> video);
  void operator()();

private:

  const std::string filename;
  const size_t frameheight;
  const size_t framewidth;
  const int framerate;

  uint64_t framenumber;

  struct
  {
    SwsContext* sws;
    AVFormatContext* format;
    AVCodecContext* codec;
  }
  context;

  struct
  {
    AVStream* video;
  }
  stream;

  struct
  {
    AVFrame* bgr;
    AVFrame* yuv;
  }
  frame;

  AVPacket* packet;

};
