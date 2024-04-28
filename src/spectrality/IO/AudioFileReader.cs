using System;

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

    var length = reader.Length / (format.BitsPerSample / 8);
    var samples = new float[length];

    reader.Read(samples, 0, samples.Length);

    var samplerate = (double)format.SampleRate;
    var duration = TimeSpan.FromSeconds(samples.Length / samplerate);

    Logger.Info($"Read {samples.Length} samples of {duration.TotalSeconds:F3}s duration at {samplerate}Hz.");

    return (samples, samplerate);
  }
}
