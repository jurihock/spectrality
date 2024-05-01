using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Spectrality.Models;

namespace Spectrality.DSP;

public class SpectrumAnalyzer
{
  private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

  public double Samplerate { get; private init; }
  public double Timestep { get; private init; }

  private QDFT QDFT { get; init; }

  public SpectrumAnalyzer(double samplerate, double timestep)
  {
    Samplerate = samplerate;
    Timestep = timestep;

    var scale = new Scale();
    var bandwidth = (scale.GetFrequency("A1"), scale.GetFrequency("A8"));
    var resolution = 12 * 4;
    var quality = -1;
    var latency = 0;

    QDFT = new QDFT(samplerate, bandwidth, resolution, quality, latency);

    Logger.Info($"Initializing spectrum analyzer (" +
                $"samplerate={samplerate}" + ", " +
                $"timestep={timestep:F3}" + ", " +
                $"hopsize={Math.Ceiling(timestep * Samplerate)}" + ", " +
                $"dftsize={QDFT.Size}" + ", " +
                $"bandwidth=({bandwidth.Item1:F3}, {bandwidth.Item2:F3})" + ", " +
                $"resolution={resolution}" + ", " +
                $"quality={quality}" + ", " +
                $"latency={latency}" + $").");
  }

  public Spectrogram GetSpectrogram(Span<float> samples)
  {
    var duration = TimeSpan.FromSeconds(samples.Length / Samplerate);
    Logger.Info($"Begin analyzing {samples.Length} samples of {duration.TotalSeconds:F3}s duration.");

    var watch = Stopwatch.GetTimestamp();
    var qdft = QDFT;

    var hopsize = (int)Math.Ceiling(Timestep * Samplerate);
    var dftsize = qdft.Size;

    var chunks = Enumerable.Range(0, samples.Length).Chunk(hopsize).ToArray();
    var timepoints = chunks.Select(chunk => (float)(chunk.First() / Samplerate)).ToArray();
    var frequencies = qdft.Frequencies.Select(freq => (float)freq).ToArray();
    var magnitudes = new float[chunks.Length, dftsize];
    var dft = new Complex[dftsize];

    var bytes = chunks.Length * dftsize * (sizeof(float) + sizeof(int));
    Logger.Info($"Approx. memory footprint {bytes:N0} bytes.");

    qdft.Reset();

    var j = 0;

    foreach (var chunk in chunks)
    {
      qdft.Analyze(samples[chunk[0]], dft);

      for (var i = 0; i < dft.Length; i++)
      {
        magnitudes[j, i] = (float)dft[i].Decibel();
      }

      for (var i = 1; i < chunk.Length; i++)
      {
        qdft.Analyze(samples[chunk[i]], dft);
      }

      j++;
    }

    var elapsed = Stopwatch.GetElapsedTime(watch);
    Logger.Info($"Analysis completed in {elapsed.TotalSeconds:F3}s.");

    return new Spectrogram
    {
      Data = new Datagram
      {
        X = timepoints,
        Y = frequencies,
        Z = magnitudes
      },
      Meta = new Metagram
      {
        X = new Metagram.AxisMeta
        {
          Name = "Timepoint",
          Unit = "s",
          Type = Metagram.AxisType.Linear
        },
        Y = new Metagram.AxisMeta
        {
          Name = "Frequency",
          Unit = "Hz",
          Type = Metagram.AxisType.Logarithmic
        },
        Z = new Metagram.AxisMeta
        {
          Name = "Magnitude",
          Unit = "dB",
          Type = Metagram.AxisType.Logarithmic
        },
      },
      Bitmap = new Bitmap(magnitudes.GetLength(0), magnitudes.GetLength(1))
    };
  }
}
