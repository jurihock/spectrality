using System;
using System.Diagnostics;

namespace Spectrality.IO;

public class AudioFileReader
{
  private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

  public string Path { get; private init; }

  public AudioFileReader(string path)
  {
    Path = path;
  }

  public (float[], double) Read(int channel = 0)
  {
    Logger.Info($"Reading audio file \"{Path}\".");

    var reader = new NAudio.Wave.AudioFileReader(Path);

    var format = reader.WaveFormat;
    var samplerate = (double)format.SampleRate;
    var channels = format.Channels;
    var length = (int)(reader.Length / (format.BitsPerSample / 8));

    var samples = new float[length];
    var result = reader.Read(samples, 0, length);

    Debug.Assert(result == length);

    if (channels > 1)
    {
      channel = (channel < 0) ? channels + channel : channel;
      channel = Math.Clamp(channel, 0, channels - 1);

      length /= channels;

      for (int i = 0, j = channel; i < length; i++, j+=channels)
      {
        samples[i] = samples[j];
      }

      Array.Resize(ref samples, length);
    }

    Debug.Assert(samples.Length == length);

    var duration = TimeSpan.FromSeconds(length / samplerate);
    Logger.Info($"Read {length} samples of {duration.TotalSeconds:F3}s duration at {samplerate}Hz.");

    return (samples, samplerate);
  }
}
