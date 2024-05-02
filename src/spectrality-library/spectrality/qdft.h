#pragma once

#include <spectrality/spectrality.h>

#include <qdft/qdft.h>

struct qdft_t
{
  qdft::QDFT<float, double>* qdft;
  std::complex<double>* dft;
};

SPECTRALITY_LIBRARY_FUNCTION
qdft_t* spectrality_qdft_alloc(
  double samplerate,
  double bandwidth_min,
  double bandwidth_max,
  double resolution,
  double quality);

SPECTRALITY_LIBRARY_FUNCTION
void spectrality_qdft_free(
  qdft_t* qdft);

SPECTRALITY_LIBRARY_FUNCTION
int spectrality_qdft_size(
  qdft_t* qdft);

SPECTRALITY_LIBRARY_FUNCTION
void spectrality_qdft_frequencies(
  qdft_t* qdft,
  double* frequencies);

SPECTRALITY_LIBRARY_FUNCTION
void spectrality_qdft_analyze_decibel(
  qdft_t* qdft,
  float* samples,
  float* decibels,
  int offset,
  int count);
