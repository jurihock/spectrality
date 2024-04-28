using System.Linq;
using OxyPlot;

namespace Spectrality.Models;

public class LinearCoordinateTransformationModel : ICoordinateTransformationModel
{
  public double SlopeX { get; private set; }
  public double SlopeY { get; private set; }

  public double InterceptX { get; private set; }
  public double InterceptY { get; private set; }

  public LinearCoordinateTransformationModel(double slopeX, double interceptX, double slopeY, double interceptY)
  {
    SlopeX = slopeX;
    SlopeY = slopeY;

    InterceptX = interceptX;
    InterceptY = interceptY;
  }

  public LinearCoordinateTransformationModel(double[] valuesX, double[] valuesY)
  {
    if (valuesX.Length > 1)
    {
      var num = valuesX.Length;
      var min = valuesX.Min();
      var max = valuesX.Max();

      SlopeX = (max - min) / (num - 1);
      InterceptX = min;
    }
    else
    {
      SlopeX = 1;
      InterceptX = 0;
    }

    if (valuesY.Length > 1)
    {
      var num = valuesY.Length;
      var min = valuesY.Min();
      var max = valuesY.Max();

      SlopeY = (max - min) / (num - 1);
      InterceptY = min;
    }
    else
    {
      SlopeY = 1;
      InterceptY = 0;
    }
  }

  public DataPoint Forward(DataPoint point)
  {
    return new DataPoint(
      point.X * SlopeX + InterceptX,
      point.Y * SlopeY + InterceptY);
  }

  public DataPoint Backward(DataPoint point)
  {
    return new DataPoint(
      (point.X - InterceptX) / SlopeX,
      (point.Y - InterceptY) / SlopeY);
  }
}
