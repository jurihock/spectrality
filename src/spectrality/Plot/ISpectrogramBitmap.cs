using Spectrality.Models;

namespace Spectrality.Plot;

public interface ISpectrogramBitmap
{
  Bitmap GetBitmap(Spectrogram spectrogram);
}
