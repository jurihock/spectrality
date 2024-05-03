#include <spectrality/audio.h>

#include <dr_mp3.h>
#include <dr_wav.h>

bool spectrality_audio_touch_mp3(
  const std::filesystem::path& path,
  samplerate_t* samplerate,
  channels_t* channels,
  frames_t* frames)
{
  drmp3 mp3;

  if (drmp3_init_file(&mp3, path.c_str(), nullptr) != DRWAV_TRUE)
  {
    drmp3_uninit(&mp3);
    return false;
  }

  *samplerate = static_cast<samplerate_t>(mp3.sampleRate);
  *channels = static_cast<channels_t>(mp3.channels);
  *frames = static_cast<frames_t>(drmp3_get_pcm_frame_count(&mp3));

  drmp3_uninit(&mp3);
  return true;
}

bool spectrality_audio_touch_wav(
  const std::filesystem::path& path,
  samplerate_t* samplerate,
  channels_t* channels,
  frames_t* frames)
{
  drwav wav;

  if (drwav_init_file(&wav, path.c_str(), nullptr) != DRWAV_TRUE)
  {
    drwav_uninit(&wav);
    return false;
  }

  *samplerate = static_cast<samplerate_t>(wav.sampleRate);
  *channels = static_cast<channels_t>(wav.channels);
  *frames = static_cast<frames_t>(wav.totalPCMFrameCount);

  drwav_uninit(&wav);
  return true;
}

bool spectrality_audio_touch(
  const char* pathchars,
  int pathsize,
  samplerate_t* samplerate,
  channels_t* channels,
  frames_t* frames)
{
  const std::filesystem::path path(pathchars, pathchars + pathsize);

  std::string extension = path.extension();
  std::transform(extension.begin(), extension.end(), extension.begin(),
    [](auto c){ return std::tolower(c); });

  if (extension == ".mp3")
  {
    return spectrality_audio_touch_mp3(path, samplerate, channels, frames);
  }

  if (extension == ".wav")
  {
    return spectrality_audio_touch_wav(path, samplerate, channels, frames);
  }

  return false;
}

bool spectrality_audio_read_mp3(
  const std::filesystem::path& path,
  float* samples,
  frames_t* frames)
{
  drmp3 mp3;

  if (drmp3_init_file(&mp3, path.c_str(), nullptr) != DRWAV_TRUE)
  {
    drmp3_uninit(&mp3);
    return false;
  }

  *frames = drmp3_read_pcm_frames_f32(&mp3, *frames, samples);

  drmp3_uninit(&mp3);
  return true;
}

bool spectrality_audio_read_wav(
  const std::filesystem::path& path,
  float* samples,
  frames_t* frames)
{
  drwav wav;

  if (drwav_init_file(&wav, path.c_str(), nullptr) != DRWAV_TRUE)
  {
    drwav_uninit(&wav);
    return false;
  }

  *frames = drwav_read_pcm_frames_f32(&wav, *frames, samples);

  drwav_uninit(&wav);
  return true;
}

bool spectrality_audio_read(
  const char* pathchars,
  int pathsize,
  float* samples,
  frames_t* frames)
{
  const std::filesystem::path path(pathchars, pathchars + pathsize);

  std::string extension = path.extension();
  std::transform(extension.begin(), extension.end(), extension.begin(),
    [](auto c){ return std::tolower(c); });

  if (extension == ".mp3")
  {
    return spectrality_audio_read_mp3(path, samples, frames);
  }

  if (extension == ".wav")
  {
    return spectrality_audio_read_wav(path, samples, frames);
  }

  return false;
}
