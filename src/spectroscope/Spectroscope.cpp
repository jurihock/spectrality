#include <spectroscope/Spectroscope.h>

#include <spectroscope/Audio.h>
#include <spectroscope/Encoder.h>
#include <spectroscope/Renderer.h>

#include <qdft/qdft.h>
#include <sdft/sdft.h>

bool cancel = false;

void onsignal(int)
{
  cancel = true;
}

int main()
{
  std::signal(SIGINT, onsignal);

  const size_t height = 768;
  const size_t width = 1024;

  Renderer renderer(height, width);
  SDFT<double, double> sdft(height);

  int samplerate;
  std::vector<double> samples;
  std::vector<std::complex<double>> dft(sdft.size());

  Audio audio("face.wav");
  audio.read(samples, samplerate);

  Encoder encoder("/tmp/face.mp4", height, width, samplerate);
  encoder.open();

  const size_t start = 3 * samplerate;
  const size_t stop = 4 * samplerate;

  int progress = 0;

  for (size_t i = 0; i < samples.size(); ++i)
  {
    if (cancel)
    {
      break;
    }

    if (i < start)
    {
      continue;
    }

    if (i > stop)
    {
      break;
    }

    int p = int(100.0 * (i - start) / (stop - start));

    if (p > progress)
    {
      std::cout << p << "%" << std::endl;
    }

    progress = p;

    sdft.sdft(samples[i], dft.data());

    encoder.encode(renderer.render(dft));
  }

  encoder.close();

  return 0;
}
