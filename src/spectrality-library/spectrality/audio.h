#pragma once

#include <spectrality/spectrality.h>

using samplerate_t = double;
using channels_t   = int;
using frames_t     = int;

SPECTRALITY_LIBRARY_FUNCTION
bool spectrality_audio_touch(
  const char* pathchars,
  const int pathsize,
  samplerate_t* const samplerate,
  channels_t* const channels,
  frames_t* const frames);

SPECTRALITY_LIBRARY_FUNCTION
bool spectrality_audio_read(
  const char* pathchars,
  const int pathsize,
  float* const samples,
  frames_t* const frames);
