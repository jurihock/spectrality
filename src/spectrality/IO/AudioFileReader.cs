using System;
using System.IO;
using System.Text;

namespace Spectrality.IO;

public class AudioFileReader
{
  private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

  public string Path { get; private init; }

  public AudioFileReader(string path)
  {
    Path = path;
  }

  public (float[] samples, double samplerate) Read(int channel = 0, double skip = 0, double take = 0)
  {
    Logger.Info($"Reading audio file \"{Path}\".");

    var path = Encoding.UTF8.GetBytes(Path);

    var touch = Library.Audio.Touch(
      path,
      path.Length,
      out var samplerate,
      out var channels,
      out var frames);

    if (!touch)
    {
      throw new FileLoadException(
        $"Unable to read audio file \"{Path}\".",
        Path);
    }

    skip = Math.Truncate(skip * samplerate);
    skip = Math.Clamp(skip, 0, frames);

    take = Math.Truncate(take * samplerate);
    take = Math.Clamp(take > 0 ? take : frames, 0, frames - skip);

    frames = (int)(skip + take);

    var samples = new float[frames * channels];

    var framesread = frames;
    var read = Library.Audio.Read(
      path,
      path.Length,
      samples,
      ref framesread);

    if (!read)
    {
      throw new FileLoadException(
        $"Unable to read audio file \"{Path}\".",
        Path);
    }

    if (framesread != frames)
    {
      Logger.Warn($"Requested {frames} PCM frames to read, " +
                  $"got {framesread} frames instead!");
    }

    if (channels > 1)
    {
      channel = (channel < 0) ? channels + channel : channel;
      channel = Math.Clamp(channel, 0, channels - 1);

      for (int i = 0, j = channel; i < frames; i++, j+=channels)
      {
        samples[i] = samples[j];
      }

      Array.Resize(ref samples, frames);
    }

    if (skip > 0)
    {
      for (int i = 0, j = (int)skip; i < (int)take; i++, j++)
      {
        samples[i] = samples[j];
      }

      Array.Resize(ref samples, (int)take);
    }

    var duration = TimeSpan.FromSeconds(samples.Length / samplerate);
    Logger.Info($"Read {samples.Length} samples " +
                $"of {duration.TotalSeconds:F3}s duration " +
                $"at {samplerate}Hz.");

    return (samples, samplerate);
  }
}
