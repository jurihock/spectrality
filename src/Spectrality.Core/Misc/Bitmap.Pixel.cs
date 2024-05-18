using System.Runtime.InteropServices;

namespace Spectrality.Misc;

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
