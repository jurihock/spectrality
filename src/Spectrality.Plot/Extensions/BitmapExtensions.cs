using OxyPlot;
using Spectrality.Misc;

namespace Spectrality.Plot.Extensions;

public static class BitmapExtensions
{
  public static OxyImage ToOxyImage(this Bitmap bitmap)
  {
    return new OxyImage(bitmap.Bytes);
  }
}
