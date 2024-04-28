using OxyPlot;

namespace Spectrality.Models;

public interface ICoordinateTransformationModel
{
  DataPoint Forward(DataPoint point);
  DataPoint Backward(DataPoint point);
}
