using OxyPlot;

namespace Spectrality.Plot.Transforms;

public sealed class CartesianCoordinateTransformation : ICoordinateTransformation<DataPoint>
{
  public ICoordinateTransformation<double> TransformX { get; private set; }
  public ICoordinateTransformation<double> TransformY { get; private set; }

  public CartesianCoordinateTransformation(ICoordinateTransformation<double> transformX,
                                           ICoordinateTransformation<double> transformY)
  {
    TransformX = transformX;
    TransformY = transformY;
  }

  public DataPoint Forward(DataPoint value)
  {
    return new DataPoint(
      TransformX.Forward(value.X),
      TransformY.Forward(value.Y));
  }

  public DataPoint Backward(DataPoint value)
  {
    return new DataPoint(
      TransformX.Backward(value.X),
      TransformY.Backward(value.Y));
  }
}
