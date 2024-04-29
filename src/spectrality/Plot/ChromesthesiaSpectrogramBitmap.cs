using System;
using System.Linq;
using OxyPlot;
using Spectrality.Models;

namespace Spectrality.Plot;

public class ChromesthesiaSpectrogramBitmap : ISpectrogramBitmap
{
  public (double, double) Limit { get; private set; }
  public double Gamma { get; private set; }
  public double Brightness { get; private set; }
  public double Saturation { get; private set; }

  public ChromesthesiaSpectrogramBitmap((double, double) limit,
                                        double gamma = 1,
                                        double brightness = 1,
                                        double saturation = 1)
  {
    Limit = limit;
    Gamma = gamma;
    Brightness = brightness;
    Saturation = saturation;
  }

  public void RenderBitmap(Spectrogram spectrogram)
  {
    var data = spectrogram.Data;
    var freqs = data.Y;

    var width = data.Width;
    var height = data.Height;

    var min = Limit.Item1;
    var max = Limit.Item2;

    var slope = 1.0 / (max - min);
    var intercept = min / (max - min);

    var scale = new Scale();

    var colors = Enumerable
      .Range(0, height)
      .Select(_ => scale.GetOctaveRatio(freqs[_]) / scale.Octaves.Length)
      .Select(_ => 1.0 - Math.Clamp(_, 0.0, 1.0))
      .ToArray();

    var gamma = Math.Clamp(Gamma, 0.1, 10.0);
    var brightness = Math.Clamp(Brightness, 0.1, 10.0);
    var saturation = (Saturation >= 0) ? Math.Clamp(Saturation, 0.0, 1.0) : 1.0;
    var highlighting = (Saturation < 0) ? Math.Clamp(Saturation, -1.0, 0.0) : 0.0;

    var bitmap = spectrogram.Bitmap;

    for (var x = 0; x < width; x++)
    {
      for (var y = 0; y < height; y++)
      {
        var h = colors[y];
        var s = saturation;
        var v = (double)data[x, y];

        v = Math.Clamp(v * slope - intercept, 0.0, 1.0);
        v = Math.Pow(v, gamma);
        v = Math.Clamp(v * brightness, 0.0, 1.0);

        s += v * highlighting;

        var color = OxyColor.FromHsv(h, s, v);
        ref var pixel = ref bitmap[x, y];

        pixel.R = color.R;
        pixel.G = color.G;
        pixel.B = color.B;
      }
    }
  }
}
