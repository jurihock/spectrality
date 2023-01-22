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

  const size_t frameheight = 768;
  const size_t framewidth = 1024;

  SDFT<double, double> sdft(frameheight);

  int samplerate;
  std::vector<double> samples;
  std::vector<std::complex<double>> dft(sdft.size());

  Audio audio("face.wav");
  audio.read(samples, samplerate);
  samples.resize(samples.size() * 2);

  Renderer renderer(frameheight, framewidth, 50, samplerate);

  Encoder encoder("/tmp/face.mp4", frameheight, framewidth, 25, samplerate);
  encoder.open();

  ptrdiff_t progress = -1;

  for (ptrdiff_t i = 0; i < samples.size(); ++i)
  {
    if (cancel)
    {
      break;
    }

    ptrdiff_t p = 10 * (i + 1) / samples.size();

    if (p > progress)
    {
      std::cout << (p * 10) << "%" << std::endl;
    }

    progress = p;

    sdft.sdft(samples[i], dft.data());

    renderer.render(dft);

    if (i < samples.size() / 2)
    {
      continue;
    }

    encoder.encode(renderer.render());
  }

  encoder.close();

  return 0;
}
