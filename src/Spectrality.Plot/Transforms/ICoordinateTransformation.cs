namespace Spectrality.Plot.Transforms;

public interface ICoordinateTransformation<T>
{
  T Forward(T value);
  T Backward(T value);
}
