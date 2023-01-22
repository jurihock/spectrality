#pragma once

#include <spectroscope/Colormap.h>
#include <spectroscope/Chromesthesia.h>

#include <complex>
#include <span>
#include <vector>

class Renderer
{

public:

  Renderer(const size_t frameheight, const size_t framewidth, const int framerate, const int samplerate);

  std::span<uint8_t> render();
  std::span<uint8_t> render(const std::span<std::complex<double>> dft);

private:

  const size_t frameheight;
  const size_t framewidth;
  const int framerate;
  const int samplerate;
  const int modulo;

  int64_t framenumber;
  int64_t samplenumber;

  const Colormap colormap;
  const Chromesthesia chromesthesia;

  std::vector<uint8_t> buffer;

};
