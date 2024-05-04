using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Spectrality.Models;

public sealed class MemoryMappedDatagram : IDatagram
{
  private readonly int dx, dy;

  public string Path { get; private init; }
  public MemoryMappedFile File { get; private init; }
  public MemoryMappedViewAccessor View { get; private init; }

  public int Width => X.Length;
  public int Height => Y.Length;

  public float[] X { get; init; }
  public float[] Y { get; init; }

  public float this[int x, int y]
  {
    get => View.ReadSingle(x * dx + y * dy);
    set => View.Write(x * dx + y * dy, value);
  }

  public MemoryMappedDatagram(int width, int height, string path)
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

    dx = Height * sizeof(float);
    dy = sizeof(float);

    // dx = sizeof(float);
    // dy = Width * sizeof(float);
  }

  public MemoryMappedDatagram(float[] x, float[] y, string path) :
    this(x.Length, y.Length, path)
  {
    Array.Copy(x, X, Width);
    Array.Copy(y, Y, Height);
  }
}
