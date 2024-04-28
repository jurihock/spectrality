using System;
using System.Linq;
using OxyPlot;
using Spectrality.Models;

namespace Spectrality.Plot;

public class ChromesthesiaSpectrogramImage : ISpectrogramImage
{
  public (double, double) Limit { get; private set; }
  public double Gamma { get; private set; }
  public double Brightness { get; private set; }
  public double Saturation { get; private set; }

  public ChromesthesiaSpectrogramImage((double, double) limit,
                                       double gamma = 1,
                                       double brightness = 1,
                                       double saturation = 1)
  {
    Limit = limit;
    Gamma = gamma;
    Brightness = brightness;
    Saturation = saturation;
  }

  public OxyImage GetImage(Spectrogram spectrogram)
  {
    var freqs = spectrogram.Frequencies;
    var data = spectrogram.Magnitudes;

    var m = data.GetLength(0);
    var n = data.GetLength(1);

    var min = Limit.Item1;
    var max = Limit.Item2;

    var slope = 1.0 / (max - min);
    var intercept = min / (max - min);

    var scale = new Scale();

    var colors = Enumerable
        .Range(0, n)
        .Select(_ => 1.0 - Math.Clamp(scale.GetOctaveRatio(freqs[_]) / scale.Octaves.Length, 0.0, 1.0))
        .ToArray();

    var gamma = Math.Clamp(Gamma, 0.1, 10.0);
    var brightness = Math.Clamp(Brightness, 0.1, 10.0);
    var saturation = (Saturation >= 0) ? Math.Clamp(Saturation, 0.0, 1.0) : 1.0;
    var highlighting = (Saturation < 0) ? Math.Clamp(Saturation, -1.0, 0.0) : 0.0;

    var pixels = new OxyColor[m, n];

    for (var i = 0; i < m; i++)
    {
      for (var j = 0; j < n; j++)
      {
        var h = colors[j];
        var s = saturation;
        var v = (double)data[i, j];

        v = Math.Clamp(v * slope - intercept, 0.0, 1.0);
        v = Math.Pow(v, gamma);
        v = Math.Clamp(v * brightness, 0.0, 1.0);

        s += v * highlighting;

        pixels[i, j] = OxyColor.FromHsv(h, s, v);
      }
    }

    return OxyImage.Create(pixels, ImageFormat.Bmp);
  }
}
