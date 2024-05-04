using System;

namespace Spectrality.Models;

public sealed class MemoryDatagram : IDatagram
{
  public int Width => X.Length;
  public int Height => Y.Length;

  public float[] X { get; private init; }
  public float[] Y { get; private init; }
  public float[,] Z { get; private init; }

  public float this[int x, int y]
  {
    get => Z[x, y];
    set => Z[x, y] = value;
  }

  public MemoryDatagram(int width, int height)
  {
    X = new float[width];
    Y = new float[height];
    Z = new float[width, height];
  }

  public MemoryDatagram(float[] x, float[] y) :
    this(x.Length, y.Length)
  {
    Array.Copy(x, X, Width);
    Array.Copy(y, Y, Height);
  }
}
