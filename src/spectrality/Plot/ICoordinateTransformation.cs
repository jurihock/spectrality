namespace Spectrality.Plot;

public interface ICoordinateTransformation<T>
{
  T Forward(T value);
  T Backward(T value);
}
