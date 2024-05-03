#include <spectrality/audio.h>

#include <dr_wav.h>

void spectrality_audio_touch_wav(
  const std::filesystem::path& path,
  double* samplerate,
  int* channels,
  int* frames)
{
  drwav wav;

  if (drwav_init_file(&wav, path.c_str(), nullptr) != DRWAV_TRUE)
  {
    throw std::runtime_error(
      "Unable to open \"" + path.string() + "\"!");
  }

  *samplerate = static_cast<double>(wav.sampleRate);
  *channels = static_cast<int>(wav.channels);
  *frames = static_cast<int>(wav.totalPCMFrameCount);

  drwav_uninit(&wav);
}

void spectrality_audio_touch(
  const char* pathchars,
  int pathsize,
  double* samplerate,
  int* channels,
  int* frames)
{
  const std::filesystem::path path(pathchars, pathchars + pathsize);

  std::string extension = path.extension();
  std::transform(extension.begin(), extension.end(), extension.begin(),
    [](auto c){ return std::tolower(c); });

  if (extension == ".wav")
  {
    spectrality_audio_touch_wav(path, samplerate, channels, frames);
  }
  else
  {
    throw std::runtime_error(
      "Unsupported audio file type " + extension + "! " +
      "Can only handle .wav files...");
  }
}

void spectrality_audio_read_wav(
  const std::filesystem::path& path,
  float* samples)
{
  drwav wav;

  if (drwav_init_file(&wav, path.c_str(), nullptr) != DRWAV_TRUE)
  {
    throw std::runtime_error(
      "Unable to open \"" + path.string() + "\"!");
  }

  const auto frames = wav.totalPCMFrameCount;

  if (drwav_read_pcm_frames_f32(&wav, frames, samples) != frames)
  {
    drwav_uninit(&wav);

    throw std::runtime_error(
      "Unable to read \"" + path.string() + "\"!");
  }

  drwav_uninit(&wav);
}

void spectrality_audio_read(
  const char* pathchars,
  int pathsize,
  float* samples)
{
  const std::filesystem::path path(pathchars, pathchars + pathsize);

  std::string extension = path.extension();
  std::transform(extension.begin(), extension.end(), extension.begin(),
    [](auto c){ return std::tolower(c); });

  if (extension == ".wav")
  {
    spectrality_audio_read_wav(path, samples);
  }
  else
  {
    throw std::runtime_error(
      "Unsupported audio file type " + extension + "! " +
      "Can only handle .wav files...");
  }
}
