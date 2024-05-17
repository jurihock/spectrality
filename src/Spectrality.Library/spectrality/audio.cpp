#include <spectrality/audio.h>

#include <dr_mp3.h>
#include <dr_wav.h>

#if defined(SPECTRALITY_AUDIO_ROUND_NEXT_SECOND)
frames_t spectrality_audio_round_next_second(
  const frames_t frames,
  const samplerate_t samplerate)
{
  return static_cast<frames_t>(
    std::ceil(1.0 * frames / samplerate) *
    samplerate);
}

bool spectrality_audio_round_error(
  const frames_t frames0,
  const frames_t frames1,
  const samplerate_t samplerate)
{
  auto error =
    std::abs(1.0 * frames0 - 1.0 * frames1) /
    samplerate;

  return (error < 1.0) ? false : true;
}
#endif

bool spectrality_audio_touch_mp3(
  const std::filesystem::path& path,
  samplerate_t* const samplerate,
  channels_t* const channels,
  frames_t* const frames)
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

  #if defined(SPECTRALITY_AUDIO_ROUND_NEXT_SECOND)
  *frames = spectrality_audio_round_next_second(*frames, *samplerate);
  #endif

  drmp3_uninit(&mp3);
  return true;
}

bool spectrality_audio_touch_wav(
  const std::filesystem::path& path,
  samplerate_t* const samplerate,
  channels_t* const channels,
  frames_t* const frames)
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

  #if defined(SPECTRALITY_AUDIO_ROUND_NEXT_SECOND)
  *frames = spectrality_audio_round_next_second(*frames, *samplerate);
  #endif

  drwav_uninit(&wav);
  return true;
}

bool spectrality_audio_touch(
  const char* pathchars,
  const int pathsize,
  samplerate_t* const samplerate,
  channels_t* const channels,
  frames_t* const frames)
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
  float* const samples,
  frames_t* const frames)
{
  drmp3 mp3;

  if (drmp3_init_file(&mp3, path.c_str(), nullptr) != DRWAV_TRUE)
  {
    drmp3_uninit(&mp3);
    return false;
  }

  auto frames0 = static_cast<drmp3_uint64>(*frames);
  auto frames1 = drmp3_read_pcm_frames_f32(&mp3, frames0, samples);

  #if defined(SPECTRALITY_AUDIO_ROUND_NEXT_SECOND)
  auto error = spectrality_audio_round_error(
    static_cast<frames_t>(frames0),
    static_cast<frames_t>(frames1),
    static_cast<samplerate_t>(mp3.sampleRate));
  frames1 = error ? frames1 : frames0;
  #endif

  *frames = static_cast<frames_t>(frames1);

  drmp3_uninit(&mp3);
  return true;
}

bool spectrality_audio_read_wav(
  const std::filesystem::path& path,
  float* const samples,
  frames_t* const frames)
{
  drwav wav;

  if (drwav_init_file(&wav, path.c_str(), nullptr) != DRWAV_TRUE)
  {
    drwav_uninit(&wav);
    return false;
  }

  auto frames0 = static_cast<drmp3_uint64>(*frames);
  auto frames1 = drwav_read_pcm_frames_f32(&wav, frames0, samples);

  #if defined(SPECTRALITY_AUDIO_ROUND_NEXT_SECOND)
  auto error = spectrality_audio_round_error(
    static_cast<frames_t>(frames0),
    static_cast<frames_t>(frames1),
    static_cast<samplerate_t>(wav.sampleRate));
  frames1 = error ? frames1 : frames0;
  #endif

  *frames = static_cast<frames_t>(frames1);

  drwav_uninit(&wav);
  return true;
}

bool spectrality_audio_read(
  const char* pathchars,
  const int pathsize,
  float* const samples,
  frames_t* const frames)
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
