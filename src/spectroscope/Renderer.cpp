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
  chromesthesia(-120, 0),
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

  std::copy(buffer.begin() + frameheight * 3, buffer.end(), buffer.begin());

  for (size_t i = 0; i < std::min(dft.size(), frameheight); ++i)
  {
    size_t j = ((framewidth - 1) * frameheight + i) * 3;

    double value = std::abs(dft[dft.size() - i - 1]);

    value = 20 * std::log10(value);

    // colormap.tobgr(value, buffer[j + 0], buffer[j + 1], buffer[j + 2]);
    chromesthesia.tobgr(double(i) / dft.size(), value, buffer[j + 0], buffer[j + 1], buffer[j + 2]);
  }

  return buffer;
}
