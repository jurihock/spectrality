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
  const double samplerate,
  const double bandwidth_min,
  const double bandwidth_max,
  const double resolution,
  const double quality);

SPECTRALITY_LIBRARY_FUNCTION
void spectrality_qdft_free(
  qdft_t* qdft);

SPECTRALITY_LIBRARY_FUNCTION
int spectrality_qdft_size(
  const qdft_t* qdft);

SPECTRALITY_LIBRARY_FUNCTION
void spectrality_qdft_frequencies(
  const qdft_t* qdft,
  double* const frequencies);

SPECTRALITY_LIBRARY_FUNCTION
void spectrality_qdft_analyze_decibel(
  const qdft_t* qdft,
  const float* samples,
  float* const decibels,
  const int count);
