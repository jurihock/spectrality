namespace Spectrality.Models;

public interface IDatagram
{
  int Width { get; }
  int Height { get; }

  float[] X { get; }
  float[] Y { get; }

  float this[int x, int y] { get; set; }
}
