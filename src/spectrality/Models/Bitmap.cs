using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Spectrality.Models;

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

public sealed partial class Bitmap
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct Pixel
  {
    public byte B;
    public byte G;
    public byte R;
  }
}

public sealed partial class Bitmap
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public readonly struct Header
  {
    public const int BitmapFileHeaderSize = 14;
    public const int BitmapInfoHeaderSize = 40;
    public const int BitmapHeaderSize = BitmapFileHeaderSize + BitmapInfoHeaderSize;

    public const int BitmapBitsPerPixel = 24;
    public const int BitmapBytesPerPixel = 3;

    public ushort FileType { get; init; }
    public uint FileSize { get; init; }
    public uint Reserved { get; init; }
    public uint DataOffset { get; init; }
    public uint InfoSize { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public ushort Planes { get; init; }
    public ushort BitsPerPixel { get; init; }
    public uint Compression { get; init; }
    public uint ImageSize { get; init; }
    public int PixelsPerMeterX { get; init; }
    public int PixelsPerMeterY { get; init; }
    public uint ColorsUsed { get; init; }
    public uint ColorsImportant { get; init; }

    public int BitsPerLine => (Width * BitsPerPixel + 31) / 32;
    public int BytesPerLine => BitsPerLine * 4;

    public Header()
    {
    }

    public Header(int width, int height)
    {
      Width = width;
      Height = height;

      BitsPerPixel = BitmapBitsPerPixel;
      Planes = 1;

      FileType = 0x4D42;
      FileSize = BitmapHeaderSize + (uint)(height * BytesPerLine);
      ImageSize = (uint)(height * BytesPerLine);
      InfoSize = BitmapInfoHeaderSize;
      DataOffset = BitmapHeaderSize;
    }
  }
}
