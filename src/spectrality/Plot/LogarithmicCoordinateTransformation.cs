using System;
using System.Linq;

namespace Spectrality.Plot;

public class LogarithmicCoordinateTransformation : ICoordinateTransformation<double>
{
  public double Slope { get; private set; }
  public double Intercept { get; private set; }

  public LogarithmicCoordinateTransformation(double slope, double intercept)
  {
    Slope = slope;
    Intercept = intercept;
  }

  public LogarithmicCoordinateTransformation(double[] values)
  {
    var min = Math.Log(values.Min());
    var max = Math.Log(values.Max());
    var num = values.Length;

    Slope = (max - min) / (num - 1);
    Intercept = min;
  }

  public LogarithmicCoordinateTransformation(float[] values)
  {
    var min = Math.Log(values.Min());
    var max = Math.Log(values.Max());
    var num = values.Length;

    Slope = (max - min) / (num - 1);
    Intercept = min;
  }

  public double Forward(double value)
  {
    return Math.Exp(value * Slope + Intercept);
  }

  public double Backward(double value)
  {
    return (Math.Log(value) - Intercept) / Slope;
  }
}
