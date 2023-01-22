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

  SDFT<double, double> sdft(height);

  int samplerate;
  std::vector<double> samples;
  std::vector<std::complex<double>> dft(sdft.size());

  Audio audio("face.wav");
  audio.read(samples, samplerate);
  samples.resize(samples.size() * 2);

  Renderer renderer(height, width, 100, samplerate);

  Encoder encoder("/tmp/face.mp4", height, width, 25, samplerate);
  encoder.open();

  ptrdiff_t progress = -1;

  for (ptrdiff_t i = 0; i < samples.size(); ++i)
  {
    if (cancel)
    {
      break;
    }

    sdft.sdft(samples[i], dft.data());
    encoder.encode(renderer.render(dft));

    ptrdiff_t p = 10 * (i + 1) / samples.size();

    if (p > progress)
    {
      std::cout << (p * 10) << "%" << std::endl;
    }

    progress = p;
  }

  encoder.close();

  return 0;
}
