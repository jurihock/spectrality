using System;
using System.Linq;

namespace Spectrality.Models;

public class LogarithmicCoordinateTransformationModel : ICoordinateTransformationModel<double>
{
  public double Slope { get; private set; }
  public double Intercept { get; private set; }

  public LogarithmicCoordinateTransformationModel(double slope, double intercept)
  {
    Slope = slope;
    Intercept = intercept;
  }

  public LogarithmicCoordinateTransformationModel(double[] values)
  {
    var min = values.Min();
    var max = values.Max();
    var num = values.Length;

    Slope = Math.Log(max / min) / (num - 1);
    Intercept = min;
  }

  public double Forward(double value)
  {
    return Math.Exp(value * Slope) * Intercept;
  }

  public double Backward(double value)
  {
    return Math.Log(value / Intercept) / Slope;
  }
}
