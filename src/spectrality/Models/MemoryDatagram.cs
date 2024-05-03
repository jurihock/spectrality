namespace Spectrality.Models;

public sealed class MemoryDatagram : IDatagram
{
  public int Width => Z.GetLength(0);
  public int Height => Z.GetLength(1);

  public float[] X { get; init; }
  public float[] Y { get; init; }
  public float[,] Z { get; init; }

  public float this[int x, int y]
  {
    get => Z[x, y];
    set => Z[x, y] = value;
  }

  public MemoryDatagram(int width = 0, int height = 0)
  {
    X = new float[width];
    Y = new float[height];
    Z = new float[width, height];
  }
}
