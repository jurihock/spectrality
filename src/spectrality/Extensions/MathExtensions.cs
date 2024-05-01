using System;
using System.Numerics;
using System.Runtime.CompilerServices;

public static class MathExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static double Decibel(this double value)
  {
    return 20.0 * Math.Log10(value + double.Epsilon);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static double Decibel(this Complex value)
  {
    return value.Magnitude.Decibel();
  }
}
