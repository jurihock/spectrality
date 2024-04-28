using System.Linq;

namespace Spectrality.Plot;

public class LinearCoordinateTransformation : ICoordinateTransformation<double>
{
  public double Slope { get; private set; }
  public double Intercept { get; private set; }

  public LinearCoordinateTransformation(double slope, double intercept)
  {
    Slope = slope;
    Intercept = intercept;
  }

  public LinearCoordinateTransformation(double[] values)
  {
    var min = values.Min();
    var max = values.Max();
    var num = values.Length;

    Slope = (max - min) / (num - 1);
    Intercept = min;
  }

  public double Forward(double value)
  {
    return value * Slope + Intercept;
  }

  public double Backward(double value)
  {
    return (value - Intercept) / Slope;
  }
}
