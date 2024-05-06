using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Spectrality.Models;

public sealed class MemoryMappedDatagram : IDatagram
{
  private readonly int dx, dy;

  private MemoryMappedFile File { get; init; }
  private MemoryMappedViewAccessor View { get; init; }

  public string Path { get; private init; }

  public int Width => X.Length;
  public int Height => Y.Length;

  public float[] X { get; init; }
  public float[] Y { get; init; }

  public float this[int x, int y]
  {
    get => View.ReadSingle(x * dx + y * dy);
    set => View.Write(x * dx + y * dy, value);
  }

  public MemoryMappedDatagram(int width, int height, string path, bool transpose = false)
  {
    Path = path;

    File = MemoryMappedFile.CreateFromFile(
      path,
      FileMode.Create,
      null,
      width * height * sizeof(float),
      MemoryMappedFileAccess.ReadWrite);

    View = File.CreateViewAccessor();

    X = new float[width];
    Y = new float[height];

    dx = (transpose ? 1 : Height) * sizeof(float);
    dy = (transpose ? Width : 1) * sizeof(float);
  }

  public MemoryMappedDatagram(float[] x, float[] y, string path, bool transpose = false) :
    this(x.Length, y.Length, path, transpose)
  {
    Array.Copy(x, X, Width);
    Array.Copy(y, Y, Height);
  }
}
