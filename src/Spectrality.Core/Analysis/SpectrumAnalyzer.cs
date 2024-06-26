using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectrality.Misc;
using Spectrality.Models;

namespace Spectrality.Analysis;

public sealed class SpectrumAnalyzer
{
  private readonly struct PrepareBag
  {
    public ReadOnlyMemory<float> Samples { get; init; }
    public double Samplerate { get; init; }
    public double Timestep { get; init; }
    public IProgress<double>? Progress { get; init; }
    public CancellationToken? Cancellation { get; init; }

    public (string min, string max) Bandwidth { get; init; } = ("A1", "A9");
    public double Resolution { get; init; } = 12 * 4;
    public double Quality { get; init; } = -1;

    public PrepareBag() {}
  }

  private readonly struct AnalysisBag
  {
    public Spectrogram Spectrogram { get; init; }
    public ReadOnlyMemory<float> Samples { get; init; }
    public int[][] Chunks { get; init; }
    public QDFT QDFT { get; init; }
    public IProgress<double>? Progress { get; init; }
    public CancellationToken? Cancellation { get; init; }
  }

  private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

  public double Samplerate { get; private init; }
  public double Timestep { get; private init; }

  public IProgress<double>? ProgressCallback { get; init; }
  public CancellationToken? CancellationToken { get; init; }

  public SpectrumAnalyzer(double samplerate, double timestep = 10e-3)
  {
    Samplerate = samplerate;
    Timestep = timestep;
  }

  public Spectrogram GetSpectrogram(ReadOnlyMemory<float> samples)
  {
    var prebag = new PrepareBag
    {
      Samples = samples,
      Samplerate = Samplerate,
      Timestep = Timestep,
      Progress = ProgressCallback,
      Cancellation = CancellationToken
    };

    return Analyze(Prepare(prebag));
  }

  public (Spectrogram, Task) GetSpectrogramTask(ReadOnlyMemory<float> samples)
  {
    var prebag = new PrepareBag
    {
      Samples = samples,
      Samplerate = Samplerate,
      Timestep = Timestep,
      Progress = ProgressCallback,
      Cancellation = CancellationToken
    };

    var bag = Prepare(prebag);

    var spectrogram = bag.Spectrogram;
    var task = new Task(() => { Analyze(bag); });

    return (spectrogram, task);
  }

  private static AnalysisBag Prepare(PrepareBag bag)
  {
    var scale = new Scale();

    var samples = bag.Samples;
    var samplerate = bag.Samplerate;
    var timestep = bag.Timestep;
    var progress = bag.Progress;
    var cancellation = bag.Cancellation;

    var bandwidth = (min: scale.GetFrequency(bag.Bandwidth.min), max: scale.GetFrequency(bag.Bandwidth.max));
    var resolution = bag.Resolution;
    var quality = bag.Quality;

    var qdft = new QDFT(samplerate, bandwidth, resolution, quality);

    Logger.Info($"Preparing for spectral analysis (" +
                $"samplerate={samplerate}" + ", " +
                $"timestep={timestep:F3}" + ", " +
                $"hopsize={Math.Ceiling(timestep * samplerate)}" + ", " +
                $"dftsize={qdft.Size}" + ", " +
                $"bandwidth=({bandwidth.min:F3}, {bandwidth.max:F3})" + ", " +
                $"resolution={resolution}" + ", " +
                $"quality={quality}" + $").");

    var hopsize = (int)Math.Ceiling(timestep * samplerate);
    var dftsize = qdft.Size;

    var chunks = Enumerable.Range(0, samples.Length).Chunk(hopsize).ToArray();

    var footprint = chunks.LongLength * dftsize * (sizeof(float) + sizeof(int));
    Logger.Info($"Approximated memory footprint {footprint:N0} bytes.");
    footprint = GC.GetTotalMemory(true);

    var timepoints = chunks.Select(chunk => (float)(chunk.First() / samplerate)).ToArray();
    var frequencies = qdft.Frequencies.Select(freq => (float)freq).ToArray();

    var spectrogram = new Spectrogram
    {
      Data = new MemoryDatagram(timepoints, frequencies),
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
        }
      },
      Bitmap = new Bitmap(timepoints.Length, frequencies.Length),
      Tags = new Spectrogram.Tag[timepoints.Length]
    };

    footprint = GC.GetTotalMemory(false) - footprint;
    Logger.Info($"Measured memory footprint {footprint:N0} bytes.");

    return new AnalysisBag
    {
      Spectrogram = spectrogram,
      Samples = samples,
      Chunks = chunks,
      QDFT = qdft,
      Progress = progress,
      Cancellation = cancellation
    };
  }

  private static Spectrogram Analyze(AnalysisBag bag)
  {
    var entrypoint = Stopwatch.GetTimestamp();

    var spectrogram = bag.Spectrogram;
    var samples = bag.Samples;
    var chunks = bag.Chunks;
    var qdft = bag.QDFT;
    var progress = bag.Progress;
    var cancellation = bag.Cancellation;

    var duration = TimeSpan.FromSeconds(samples.Length / qdft.Samplerate);
    Logger.Info($"Analyzing {samples.Length} samples of {duration.TotalSeconds:F3}s duration.");

    var datagram = spectrogram.Data;
    var tags = spectrogram.Tags;

    Span<float> buffer = stackalloc float[qdft.Size];

    try
    {
      var j = 0;

      foreach (var chunk in chunks)
      {
        if (cancellation?.IsCancellationRequested ?? false)
        {
          Logger.Info($"Cancelling analysis at {100 * j / chunks.Length}%.");
          break;
        }

        qdft.AnalyzeDecibel(samples.Span.Slice(chunk.First(), chunk.Length), buffer);

        for (var i = 0; i < buffer.Length; i++)
        {
          datagram[j, i] = buffer[i];
        }

        tags[j] = Spectrogram.Tag.Analyzed;

        progress?.Report(100.0 * j / chunks.Length);

        j++;
      }
    }
    finally
    {
      progress?.Report(100.0);
    }

    var elapsed = Stopwatch.GetElapsedTime(entrypoint);
    Logger.Info($"Analysis completed in {elapsed.TotalSeconds:F3}s.");

    return spectrogram;
  }
}
