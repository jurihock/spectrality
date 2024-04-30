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

  public (float[], double) Read(int channel = 0, double offset = 0, double limit = 0)
  {
    Logger.Info($"Reading audio file \"{Path}\".");

    var reader = new NAudio.Wave.AudioFileReader(Path);

    var format = reader.WaveFormat;
    var samplerate = (double)format.SampleRate;
    var channels = format.Channels;
    var bytes = format.BitsPerSample / 8;
    var samples = (int)(reader.Length / bytes / channels);

    if (offset > 0)
    {
      var newoffset = (int)(offset * samplerate);

      newoffset = Math.Min(newoffset, samples);

      samples -= newoffset;
      reader.Position = newoffset * channels * bytes;
    }

    if (limit > 0)
    {
      var newsamples = (int)(limit * samplerate);

      samples = Math.Min(newsamples, samples);
    }

    var buffer = new float[samples * channels];
    var result = reader.Read(buffer, 0, buffer.Length);

    Debug.Assert(result == buffer.Length);

    if (channels > 1)
    {
      channel = (channel < 0) ? channels + channel : channel;
      channel = Math.Clamp(channel, 0, channels - 1);

      for (int i = 0, j = channel; i < samples; i++, j+=channels)
      {
        buffer[i] = buffer[j];
      }

      Array.Resize(ref buffer, samples);
    }

    Debug.Assert(buffer.Length == samples);

    var duration = TimeSpan.FromSeconds(samples / samplerate);
    Logger.Info($"Read {samples} samples of {duration.TotalSeconds:F3}s duration at {samplerate}Hz.");

    return (buffer, samplerate);
  }
}
