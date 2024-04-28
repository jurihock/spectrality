using OxyPlot;

namespace Spectrality.Models;

public class CartesianCoordinateTransformationModel : ICoordinateTransformationModel<DataPoint>
{
  public ICoordinateTransformationModel<double> TransformX { get; private set; }
  public ICoordinateTransformationModel<double> TransformY { get; private set; }

  public CartesianCoordinateTransformationModel(ICoordinateTransformationModel<double> transformX,
                                                ICoordinateTransformationModel<double> transformY)
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
