using System;
using Spectrality.Extensions;

namespace Spectrality.DSP;

public sealed class QDFT : IDisposable
{
  private nint? Qdft { get; set; }

  public double Samplerate { get; private init; }
  public (double min, double max) Bandwidth { get; private init; }
  public double Resolution { get; private init; }
  public double Quality { get; private init; }

  public int Size { get; private init; }
  public double[] Frequencies { get; private init; }

  public QDFT(double samplerate,
              (double min, double max) bandwidth,
              double resolution = 24,
              double quality = 0)
  {
    Qdft = Library.QDFT.Alloc(
      samplerate,
      bandwidth.min,
      bandwidth.max,
      resolution,
      quality);

    Samplerate = samplerate;
    Bandwidth = bandwidth;
    Resolution = resolution;
    Quality = quality;

    Size = Library.QDFT.Size(Qdft.Value);
    Frequencies = new double[Size];

    Library.QDFT.Frequencies(Qdft.Value, out Frequencies.Ref());
  }

  public void Dispose()
  {
    if (Qdft.HasValue)
    {
      Library.QDFT.Free(Qdft.Value);
      Qdft = null;
    }
  }

  public void AnalyzeDecibel(ReadOnlySpan<float> samples, Span<float> decibels)
  {
    ArgumentOutOfRangeException.ThrowIfNotEqual(decibels.Length, Size, nameof(decibels));

    var qdft = Qdft ?? throw new InvalidOperationException(
      "Invalid QDFT instance pointer!");

    Library.QDFT.AnalyzeDecibel(qdft, in samples.Ref(), out decibels.Ref(), samples.Length);
  }
}
