using System.Runtime.CompilerServices;
using OxyPlot;
using Spectrality.Misc;

namespace Spectrality.Extensions;

public static class BitmapExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static OxyImage ToOxyImage(this Bitmap bitmap)
  {
    return new OxyImage(bitmap.Bytes);
  }
}
