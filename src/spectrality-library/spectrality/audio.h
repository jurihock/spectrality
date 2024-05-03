#pragma once

#include <spectrality/spectrality.h>

SPECTRALITY_LIBRARY_FUNCTION
void spectrality_audio_touch(
  const char* pathchars,
  int pathsize,
  double* samplerate,
  int* channels,
  int* frames);

SPECTRALITY_LIBRARY_FUNCTION
void spectrality_audio_read(
  const char* pathchars,
  int pathsize,
  float* samples);
