#pragma once

#include <memory>
#include <string>

extern "C"
{
  #include <libavcodec/avcodec.h>
  #include <libavformat/avformat.h>
  #include <libavutil/opt.h>
  #include <libswresample/swresample.h>
  #include <libswscale/swscale.h>
}

struct AVError : std::runtime_error
{
  AVError(const std::string& what) :
    std::runtime_error(what) {}

  AVError(const std::string& what, const int errorcode) :
    std::runtime_error(what + "(" + std::string(av_err2str(errorcode)) + ")") {}

  AVError(const int errorcode) :
    std::runtime_error(std::string(av_err2str(errorcode))) {}
};

template<typename T>
static void AVAssert(const std::string& what, const T errorcode_or_pointer)
{
  if constexpr (std::is_integral<T>())
  {
    const auto errorcode = errorcode_or_pointer;

    if (errorcode < 0)
    {
      throw AVError(what, errorcode);
    }
  }
  else if constexpr (std::is_pointer<T>())
  {
    const auto pointer = errorcode_or_pointer;

    if (pointer == nullptr)
    {
      throw AVError(what);
    }
  }
}

static void AVAssert(const int errorcode)
{
  if (errorcode < 0)
  {
    throw AVError(errorcode);
  }
}

template<typename T, typename V, V(*D)(T*)>
struct AVDeleter1
{
  inline void operator() (T* pointer) const
  {
    if (pointer) D(pointer);
  }
};

template<typename T, typename V, V(*D)(T**)>
struct AVDeleter2
{
  inline void operator() (T* pointer) const
  {
    if (pointer) D(&pointer);
  }
};

using AVCodecContextPointer = std::unique_ptr<
    AVCodecContext, AVDeleter2<AVCodecContext, void, avcodec_free_context>
>;

using AVFormatContextPointer = std::unique_ptr<
    AVFormatContext, AVDeleter1<AVFormatContext, void, avformat_free_context>
>;

using AVFramePointer = std::unique_ptr<
  AVFrame, AVDeleter2<AVFrame, void, av_frame_free>
>;

using AVPacketPointer = std::unique_ptr<
  AVPacket, AVDeleter2<AVPacket, void, av_packet_free>
>;

using SwsContextPointer = std::unique_ptr<
  SwsContext, AVDeleter1<SwsContext, void, sws_freeContext>
>;
