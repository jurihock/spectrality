#include <spectrality/qdft.h>

qdft_t* spectrality_qdft_alloc(
  const double samplerate,
  const double bandwidth_min,
  const double bandwidth_max,
  const double resolution,
  const double quality)
{
  qdft_t* qdft = new qdft_t{};

  qdft->qdft = new qdft::QDFT<float, double>(
    samplerate,
    std::make_pair(bandwidth_min, bandwidth_max),
    resolution,
    quality);

  qdft->dft = new std::complex<double>[
    qdft->qdft->size()];

  return qdft;
}

void spectrality_qdft_free(
  qdft_t* qdft)
{
  if (qdft == nullptr)
  {
    return;
  }

  if (qdft->qdft != nullptr)
  {
    delete qdft->qdft;
    qdft->qdft = nullptr;
  }

  if (qdft->dft != nullptr)
  {
    delete[] qdft->dft;
    qdft->dft = nullptr;
  }

  delete qdft;
  qdft = nullptr;
}

int spectrality_qdft_size(
  const qdft_t* qdft)
{
  if (qdft == nullptr)
  {
    throw std::runtime_error("Invalid QDFT instance pointer!");
  }

  return static_cast<int>(qdft->qdft->size());
}

void spectrality_qdft_frequencies(
  const qdft_t* qdft,
  double* const frequencies)
{
  if (qdft == nullptr)
  {
    throw std::runtime_error("Invalid QDFT instance pointer!");
  }

  std::copy(
    qdft->qdft->frequencies().begin(),
    qdft->qdft->frequencies().end(),
    frequencies);
}

void spectrality_qdft_analyze_decibel(
  const qdft_t* qdft,
  const float* samples,
  float* const decibels,
  const int count)
{
  const auto decibel = [](const std::complex<double>& x) -> float
  {
    const double epsilon = std::numeric_limits<double>::epsilon();
    const double modulus = std::abs(x);
    const double y = 20 * std::log10(modulus + epsilon);
    return static_cast<float>(y);
  };

  if (qdft == nullptr)
  {
    throw std::runtime_error("Invalid QDFT instance pointer!");
  }

  if (count < 1)
  {
    return;
  }

  qdft->qdft->qdft(samples[0], qdft->dft);

  for (auto i = size_t(0); i < qdft->qdft->size(); ++i)
  {
    decibels[i] = decibel(qdft->dft[i]);
  }

  for (auto i = 1; i < count; ++i)
  {
    qdft->qdft->qdft(samples[i], qdft->dft);
  }
}
