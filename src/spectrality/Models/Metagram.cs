namespace Spectrality.Models;

public readonly struct Metagram
{
  public enum AxisType
  {
    Linear,
    Logarithmic
  }

  public readonly struct AxisMeta
  {
    public string Name { get; init; }
    public string Unit { get; init; }
    public AxisType Type { get; init; }
  }

  public AxisMeta X { get; init; }
  public AxisMeta Y { get; init; }
  public AxisMeta Z { get; init; }
}
