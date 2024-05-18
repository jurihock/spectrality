using System.Runtime.InteropServices;

namespace Spectrality.Misc;

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
