namespace Spectrality.Models;

public interface ICoordinateTransformationModel<T>
{
  T Forward(T value);
  T Backward(T value);
}
