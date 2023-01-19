#include <spectroscope/Spectroscope.h>

#include <spectroscope/Encoder.h>

#include <vector>

int main()
{
  std::vector<uint8_t> video(288 * 352 * 3);

  Encoder encoder("/tmp/foo.mp4", 288, 352);

  encoder.open();

  for (auto i = 0; i < 100; ++i)
  {
    encoder.encode(video);
  }

  encoder.close();

  return 0;
}
