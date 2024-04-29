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

  public (float[], double) Read(int channel = 0, double start = 0, double limit = 0)
  {
    Logger.Info($"Reading audio file \"{Path}\".");

    var reader = new NAudio.Wave.AudioFileReader(Path);

    var format = reader.WaveFormat;
    var samplerate = (double)format.SampleRate;
    var channels = format.Channels;
    var length = (int)(reader.Length / (format.BitsPerSample / 8));
    var read = (offset: 0, count: length);

    if (start > 0)
    {
      read.offset = (int)(start * samplerate);
      read.offset *= channels;
    }

    if (limit > 0)
    {
      read.count = (int)(limit * samplerate);
      read.count *= channels;
    }

    read.offset = Math.Min(read.offset, length);
    read.count = Math.Min(read.count, length - read.offset);
    length = read.count;

    var samples = new float[length];
    var result = reader.Read(samples, read.offset, read.count);

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
