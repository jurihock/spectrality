using OxyPlot;
using Spectrality.Models;

namespace Spectrality.Plot;

public interface ISpectrogramImage
{
  OxyImage GetImage(Spectrogram spectrogram);
}
