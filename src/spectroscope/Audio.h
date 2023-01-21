#pragma once

#include <spectroscope/FFmpeg.h>

#include <algorithm>
#include <filesystem>
#include <vector>

class Audio
{

public:

  Audio(const std::filesystem::path& filename);

  void read(std::vector<double>& samples, int& samplerate, const int channel = -1);

private:

  const std::filesystem::path filename;

};
