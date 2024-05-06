using Spectrality.Models;

namespace Spectrality.Plot.Renderers;

public interface ISpectrogramRenderer
{
  void RenderBitmap(Spectrogram spectrogram);
}
