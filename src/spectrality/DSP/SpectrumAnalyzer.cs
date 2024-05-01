using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Spectrality.Extensions;
using Spectrality.Misc;
using Spectrality.Models;

namespace Spectrality.DSP;

public readonly struct SpectrumAnalysisBag
{
  public Spectrogram Spectrogram { get; init; }
  public float[] Samples { get; init; }
  public int[][] Chunks { get; init; }
  public QDFT QDFT { get; init; }
  public IProgress<double>? Progress { get; init; }
}

public class SpectrumAnalyzer
{
  private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

  public double Samplerate { get; private init; }
  public double Timestep { get; private init; }

  public SpectrumAnalyzer(double samplerate, double timestep)
  {
    Samplerate = samplerate;
    Timestep = timestep;
  }

  public Spectrogram GetSpectrogram(float[] samples, IProgress<double>? progress = null)
  {
    return Analyze(Prepare(Samplerate, Timestep, samples, progress));
  }

  public (Spectrogram, Task) GetSpectrogramTask(float[] samples, IProgress<double>? progress = null)
  {
    var bag = Prepare(Samplerate, Timestep, samples, progress);
    var task = new Task(() => { Analyze(bag); });

    return (bag.Spectrogram, task);
  }

  private static SpectrumAnalysisBag Prepare(double samplerate, double timestep, float[] samples, IProgress<double>? progress)
  {
    var scale = new Scale();
    var bandwidth = (scale.GetFrequency("A1"), scale.GetFrequency("A8"));
    var resolution = 12 * 4;
    var quality = -1;
    var latency = 0;

    var qdft = new QDFT(samplerate, bandwidth, resolution, quality, latency);

    Logger.Info($"Preparing for spectral analysis (" +
                $"samplerate={samplerate}" + ", " +
                $"timestep={timestep:F3}" + ", " +
                $"hopsize={Math.Ceiling(timestep * samplerate)}" + ", " +
                $"dftsize={qdft.Size}" + ", " +
                $"bandwidth=({bandwidth.Item1:F3}, {bandwidth.Item2:F3})" + ", " +
                $"resolution={resolution}" + ", " +
                $"quality={quality}" + ", " +
                $"latency={latency}" + $").");

    var hopsize = (int)Math.Ceiling(timestep * samplerate);
    var dftsize = qdft.Size;

    var chunks = Enumerable.Range(0, samples.Length).Chunk(hopsize).ToArray();

    var bytes = chunks.Length * dftsize * (sizeof(float) + sizeof(int));
    Logger.Info($"Approx. memory footprint {bytes:N0} bytes.");

    var timepoints = chunks.Select(chunk => (float)(chunk.First() / samplerate)).ToArray();
    var frequencies = qdft.Frequencies.Select(freq => (float)freq).ToArray();
    var magnitudes = new float[chunks.Length, dftsize];

    magnitudes.Fill(float.MinValue);

    var spectrogram = new Spectrogram
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

    return new SpectrumAnalysisBag
    {
      Spectrogram = spectrogram,
      Samples = samples,
      Chunks = chunks,
      QDFT = qdft,
      Progress = progress
    };
  }

  private static Spectrogram Analyze(SpectrumAnalysisBag bag)
  {
    var spectrogram = bag.Spectrogram;
    var samples = bag.Samples;
    var chunks = bag.Chunks;
    var qdft = bag.QDFT;
    var progress = bag.Progress;

    var duration = TimeSpan.FromSeconds(samples.Length / qdft.Samplerate);
    Logger.Info($"Begin analyzing {samples.Length} samples of {duration.TotalSeconds:F3}s duration.");

    var watch = Stopwatch.GetTimestamp();
    var magnitudes = spectrogram.Data.Z;
    var dft = new Complex[qdft.Size];

    try
    {
      var j = 0;

      foreach (var chunk in chunks)
      {
        qdft.Analyze(samples[chunk[0]], dft);

        for (var i = 0; i < dft.Length; i++)
        {
          magnitudes[j, i] = (float)dft[i].Decibel();
        }

        progress?.Report(100.0 * j / chunks.Length);

        for (var i = 1; i < chunk.Length; i++)
        {
          qdft.Analyze(samples[chunk[i]], dft);
        }

        j++;
      }
    }
    finally
    {
      progress?.Report(100.0);
    }

    var elapsed = Stopwatch.GetElapsedTime(watch);
    Logger.Info($"Analysis completed in {elapsed.TotalSeconds:F3}s.");

    return spectrogram;
  }
}
