using Spectrality.Misc;

namespace Spectrality.Models;

public readonly struct Spectrogram
{
  public enum Tag { None, Analyzed, Rendered }

  public IDatagram Data { get; init; }
  public Metagram Meta { get; init; }
  public Bitmap Bitmap { get; init; }
  public Tag[] Tags { get; init; }
}
