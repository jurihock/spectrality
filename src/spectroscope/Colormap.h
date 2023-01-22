#pragma once

#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>

#include <algorithm>
#include <numeric>
#include <span>
#include <vector>

class Colormap
{

public:

  Colormap(const double min, const double max) :
    min(min),
    max(max),
    slope(+1.0 / (max - min)),
    intercept(-min / (max - min))
  {
    std::vector<uint8_t> input(256);
    std::vector<uint8_t> output(256 * 3);

    std::iota(input.begin(), input.end(), 0);

    cv::Mat gray(1, 256, CV_8UC1, input.data());
    cv::Mat color(1, 256, CV_8UC3, output.data());

    cv::applyColorMap(gray, color, cv::COLORMAP_MAGMA);

    for (size_t i = 0; i < 256; ++i)
    {
      lut[i][0] = output[i * 3 + 0];
      lut[i][1] = output[i * 3 + 1];
      lut[i][2] = output[i * 3 + 2];
    }
  }

  void tobgr(const double value, uint8_t& b, uint8_t& g, uint8_t& r) const
  {
    const uint8_t i = static_cast<uint8_t>(
      0xFF * std::clamp(value * slope + intercept, 0.0, 1.0));

    b = lut[i][0];
    g = lut[i][1];
    r = lut[i][2];
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

  uint8_t lut[256][3];

};
