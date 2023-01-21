#include <spectroscope/Renderer.h>

Renderer::Renderer(const size_t height, const size_t width) :
  height(height),
  width(width),
  colormap(-120, 0),
  buffer(height * width * 3)
{
}

std::span<uint8_t> Renderer::render(const std::span<std::complex<double>> dft)
{
  for (size_t y = 0; y < height; ++y)
  {
    for (size_t x = 1; x < width; ++x)
    {
      size_t i = (y * width + x) * 3;
      size_t j = i - 3;

      buffer[j + 0] = buffer[i + 0];
      buffer[j + 1] = buffer[i + 1];
      buffer[j + 2] = buffer[i + 2];
    }
  }

  for (size_t i = 0; i < std::min(dft.size(), height); ++i)
  {
    size_t j = ((i + 1) * width - 1) * 3;

    double value = std::abs(dft[dft.size() - i - 1]);

    value = 20 * std::log10(value);

    colormap.tobgr(value, buffer[j + 0], buffer[j + 1], buffer[j + 2]);
  }

  return buffer;
}
