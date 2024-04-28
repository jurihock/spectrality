namespace Spectrality.Models;

public readonly struct Datagram
{
  public float[]  X { get; init; }
  public float[]  Y { get; init; }
  public float[,] Z { get; init; }

  public readonly int Width  => Z.GetLength(0);
  public readonly int Height => Z.GetLength(1);

  public readonly float this[int x, int y] => Z[x, y];
}
