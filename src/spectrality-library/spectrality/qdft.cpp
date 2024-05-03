#include <spectrality/qdft.h>

qdft_t* spectrality_qdft_alloc(
  double samplerate,
  double bandwidth_min,
  double bandwidth_max,
  double resolution,
  double quality)
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
  qdft_t* qdft)
{
  if (qdft == nullptr)
  {
    throw std::runtime_error("Invalid QDFT instance pointer!");
  }

  return static_cast<int>(qdft->qdft->size());
}

void spectrality_qdft_frequencies(
  qdft_t* qdft,
  double* frequencies)
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
  qdft_t* qdft,
  float* samples,
  float* decibels,
  int offset,
  int count)
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

  qdft->qdft->qdft(samples[offset], qdft->dft);

  for (auto i = size_t(0); i < qdft->qdft->size(); ++i)
  {
    decibels[i] = decibel(qdft->dft[i]);
  }

  for (auto i = offset + 1; i < offset + count - 1; ++i)
  {
    qdft->qdft->qdft(samples[i], qdft->dft);
  }
}
