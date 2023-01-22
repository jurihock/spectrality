#pragma once

#include <color/color.hpp>

#include <algorithm>
#include <cmath>
#include <numeric>
#include <vector>

class Chromesthesia
{

public:

  Chromesthesia(const double min, const double max) :
    min(min),
    max(max),
    slope(+1.0 / (max - min)),
    intercept(-min / (max - min)),
    gamma(1.5)
  {
  }

  void tobgr(const double index, const double value, uint8_t& b, uint8_t& g, uint8_t& r) const
  {
    double i = value;

    i = i * slope + intercept;
    i = std::clamp(i, 0.0, 1.0);
    i = std::pow(i, gamma);

    const color::hsv<double> hsv({ 360 * index, 100, 100 * i });
    const color::bgr<uint8_t> bgr(hsv);

    b = color::get::blue(bgr);
    g = color::get::green(bgr);
    r = color::get::red(bgr);
  }

private:

  const double min;
  const double max;
  const double slope;
  const double intercept;
  const double gamma;

};
