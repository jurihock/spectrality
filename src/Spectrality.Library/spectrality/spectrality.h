#pragma once

#if defined(_MSC_VER)

  #define SPECTRALITY_LIBRARY_FUNCTION \
    extern "C" __declspec(dllexport)

#else

  #define SPECTRALITY_LIBRARY_FUNCTION \
    extern "C"

#endif

#define SPECTRALITY_AUDIO_ROUND_NEXT_SECOND

#include <cmath>
#include <filesystem>
#include <string>
