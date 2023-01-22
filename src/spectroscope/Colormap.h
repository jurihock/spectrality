#pragma once

#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>

#include <algorithm>
#include <cmath>
#include <numeric>
#include <vector>

class Colormap
{

public:

  Colormap(const double min, const double max) :
    min(min),
    max(max),
    slope(+1.0 / (max - min)),
    intercept(-min / (max - min)),
    gamma(1.5)
  {
    std::vector<uint8_t> input(256);
    std::vector<uint8_t> output(256 * 3);

    std::iota(input.begin(), input.end(), 0);

    cv::Mat gray(1, 256, CV_8UC1, input.data());
    cv::Mat color(1, 256, CV_8UC3, output.data());

    cv::applyColorMap(gray, color, cv::COLORMAP_OCEAN);

    for (size_t i = 0; i < 256; ++i)
    {
      lut[i][0] = output[i * 3 + 0];
      lut[i][1] = output[i * 3 + 1];
      lut[i][2] = output[i * 3 + 2];
    }
  }

  void tobgr(const double value, uint8_t& b, uint8_t& g, uint8_t& r) const
  {
    double i = value;

    i = i * slope + intercept;
    i = std::clamp(i, 0.0, 1.0);
    i = std::pow(i, gamma);

    uint8_t j = static_cast<uint8_t>(0xFF * i);

    b = lut[j][0];
    g = lut[j][1];
    r = lut[j][2];
  }

private:

  const double min;
  const double max;
  const double slope;
  const double intercept;
  const double gamma;

  uint8_t lut[256][3];

};
