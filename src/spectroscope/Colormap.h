#pragma once

#include <algorithm>
#include <span>

class Colormap
{

public:

  Colormap(const double min, const double max) :
    min(min),
    max(max),
    slope(+1.0 / (max - min)),
    intercept(-min / (max - min))
  {
  }

  void tobgr(const double value, uint8_t& b, uint8_t& g, uint8_t& r) const
  {
    const uint8_t intensity = static_cast<uint8_t>(
      0xFF * std::clamp(value * slope + intercept, 0.0, 1.0));

    b = intensity;
    g = intensity;
    r = intensity;
  }

  void tobgr(const std::span<double> values, const std::span<uint8_t> bgr) const
  {
    for (size_t i = 0, j = 0; i < values.size(); ++i, j+=3)
    {
      tobgr(values[i], bgr[j + 0], bgr[j + 1], bgr[j + 2]);
    }
  }

private:

  const double min;
  const double max;
  const double slope;
  const double intercept;

};
