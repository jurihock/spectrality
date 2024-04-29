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

    var hop = (int)Math.Ceiling(1.0 * Timestep * Samplerate);
    var hops = (int)Math.Ceiling(1.0 * samples.Length / hop);
    var bins = qdft.Size;

    var timepoints = Enumerable.Range(0, hops).Select(_ => (float)(_ * Timestep)).ToArray();
    var frequencies = qdft.Frequencies.Select(_ => (float)_).ToArray();
    var magnitudes = new float[hops, bins];
    var dft = new Complex[bins];

    var bytes = hops * bins * (sizeof(float) + sizeof(int));
    Logger.Info($"Approx. memory footprint {bytes:N0} bytes.");

    qdft.Reset();

    for (var i = 0; i < samples.Length; i++)
    {
      qdft.Analyze(samples[i], dft);

      if (i % hop != 0)
        continue;

      var t = i / hop;

      for (var j = 0; j < bins; j++)
      {
        var magnitude = dft[j].Magnitude;

        magnitude = 20.0 * Math.Log10(
          magnitude + double.Epsilon);

        magnitudes[t, j] = (float)magnitude;
      }
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
      }
    };
  }
}
