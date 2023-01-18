#include <spectroscope/Spectroscope.h>

#include <spectroscope/Encoder.h>

int main()
{
  Encoder encoder("/tmp/foo.mp4", 288, 352);

  std::vector<uint8_t> video(288 * 352 * 3);

  for (auto i = 0; i < 100; ++i)
  {
    encoder(video);
  }

  encoder();

  return 0;
}
