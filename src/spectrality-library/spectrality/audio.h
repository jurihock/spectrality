#pragma once

#include <spectrality/spectrality.h>

using samplerate_t = double;
using channels_t   = uint32_t;
using frames_t     = uint64_t;

SPECTRALITY_LIBRARY_FUNCTION
bool spectrality_audio_touch(
  const char* pathchars,
  int pathsize,
  samplerate_t* samplerate,
  channels_t* channels,
  frames_t* frames);

SPECTRALITY_LIBRARY_FUNCTION
bool spectrality_audio_read(
  const char* pathchars,
  int pathsize,
  float* samples,
  frames_t* frames);
