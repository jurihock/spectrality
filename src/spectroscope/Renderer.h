#pragma once

#include <spectroscope/Colormap.h>

#include <complex>
#include <span>
#include <vector>

class Renderer
{

public:

  Renderer(const size_t height, const size_t width);

  std::span<uint8_t> render(const std::span<std::complex<double>> dft);

private:

  const size_t height;
  const size_t width;

  const Colormap colormap;

  std::vector<uint8_t> buffer;

};
