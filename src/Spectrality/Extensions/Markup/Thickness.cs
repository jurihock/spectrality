using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Spectrality.Extensions.Markup;

public sealed class Thickness
{
  public const double Baseline = 10;

  public string Value { get; private init; }

  public Thickness(int factor, char? orientation = null)
  {
    var value = (factor switch
    {
      > 0 => Baseline * Math.Pow(2, factor - 1),
      < 0 => Baseline * Math.Pow(2, factor),
        _ => 0,
    }).ToString(CultureInfo.InvariantCulture);

    Value = char.ToUpperInvariant(orientation ?? default) switch
    {
      'L' => $"{value} 0 0 0",
      'T' => $"0 {value} 0 0",
      'R' => $"0 0 {value} 0",
      'B' => $"0 0 0 {value}",
      'X' => $"{value} 0 {value} 0",
      'Y' => $"0 {value} 0 {value}",
       _  => $"{value} {value} {value} {value}",
    };
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public string ProvideValue() => Value;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override string ToString() => Value;
}
