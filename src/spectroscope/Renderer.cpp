#include <spectroscope/Renderer.h>

Renderer::Renderer(const size_t frameheight, const size_t framewidth, const int framerate, const int samplerate) :
  frameheight(frameheight),
  framewidth(framewidth),
  framerate(framerate),
  samplerate(samplerate),
  modulo(samplerate / framerate),
  framenumber(-1),
  samplenumber(-1),
  colormap(-120, 0),
  buffer(frameheight * framewidth * 3)
{
}

std::span<uint8_t> Renderer::render()
{
  return buffer;
}

std::span<uint8_t> Renderer::render(const std::span<std::complex<double>> dft)
{
  if (++samplenumber % modulo)
  {
    return buffer;
  }
  else
  {
    ++framenumber;
  }

  for (size_t y = 0; y < frameheight; ++y)
  {
    for (size_t x = 1; x < framewidth; ++x)
    {
      size_t i = (y * framewidth + x) * 3;
      size_t j = i - 3;

      buffer[j + 0] = buffer[i + 0];
      buffer[j + 1] = buffer[i + 1];
      buffer[j + 2] = buffer[i + 2];
    }
  }

  for (size_t i = 0; i < std::min(dft.size(), frameheight); ++i)
  {
    size_t j = ((i + 1) * framewidth - 1) * 3;

    double value = std::abs(dft[dft.size() - i - 1]);

    value = 20 * std::log10(value);

    colormap.tobgr(value, buffer[j + 0], buffer[j + 1], buffer[j + 2]);
  }

  return buffer;
}
