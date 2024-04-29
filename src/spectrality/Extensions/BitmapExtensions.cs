using OxyPlot;
using Spectrality.Models;

public static class BitmapExtensions
{
  public static OxyImage ToOxyImage(this Bitmap bitmap)
  {
    return new OxyImage(bitmap.Bytes);
  }
}
