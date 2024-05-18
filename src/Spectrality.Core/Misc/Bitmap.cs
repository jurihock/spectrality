using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Spectrality.Misc;

public sealed partial class Bitmap
{
  private const int HeaderSize = Header.BitmapHeaderSize;
  private const int BytesPerPixel = Header.BitmapBytesPerPixel;

  private int BytesPerLine;

  public int Width { get; private set; }
  public int Height { get; private set; }
  public byte[] Bytes { get; private set; } = [];

  public ref Pixel this[int x, int y]
  {
    get
    {
      var bytes = Bytes.AsSpan(
        y * BytesPerLine +
        x * BytesPerPixel +
        HeaderSize,
        BytesPerPixel);

      return ref MemoryMarshal.AsRef<Pixel>(bytes);
    }
  }

  static Bitmap()
  {
    Debug.Assert(Marshal.SizeOf<Header>() == HeaderSize);
    Debug.Assert(Marshal.SizeOf<Pixel>() == BytesPerPixel);
  }

  public Bitmap()
  {
  }

  public Bitmap(int width, int height)
  {
    var header = new Header(width, height);
    var bytes = new byte[header.FileSize];

    MemoryMarshal.Write(bytes, header);

    BytesPerLine = header.BytesPerLine;
    Width = header.Width;
    Height = header.Height;
    Bytes = bytes;
  }

  public void Read(string path)
  {
    var bytes = File.ReadAllBytes(path);
    var header = MemoryMarshal.Read<Header>(bytes);

    BytesPerLine = header.BytesPerLine;
    Width = header.Width;
    Height = header.Height;
    Bytes = bytes;
  }

  public void Write(string path)
  {
    File.WriteAllBytes(path, Bytes);
  }
}
