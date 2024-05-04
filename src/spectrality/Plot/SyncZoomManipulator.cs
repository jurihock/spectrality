using System;
using OxyPlot;

namespace Spectrality.Plot;

public sealed class SyncZoomEventArgs(double minX, double maxX, double minY, double maxY) : EventArgs
{
  public double ActualMinimumX { get; private init; } = minX;
  public double ActualMaximumX { get; private init; } = maxX;

  public double ActualMinimumY { get; private init; } = minY;
  public double ActualMaximumY { get; private init; } = maxY;
}

public sealed class SyncZoomManipulator : MouseManipulator
{
  public double Factor { get; private init; }

  public event EventHandler<SyncZoomEventArgs>? ZoomChanged;

  public SyncZoomManipulator(IPlotView view, double delta) : base(view)
  {
    Factor = delta * 1e-3 * 3e-1;
  }

  public override void Started(OxyMouseEventArgs args)
  {
    base.Started(args);

    var isZoomEnabled =
      (XAxis?.IsZoomEnabled ?? false) ||
      (YAxis?.IsZoomEnabled ?? false);

    if (!isZoomEnabled)
    {
      return;
    }

    var factor = Factor;

    if (factor > 0)
    {
      factor = 1 + factor;
    }
    else
    {
      factor = 1.0 / (1 - factor);
    }

    var origin = InverseTransform(
      args.Position.X,
      args.Position.Y);

    XAxis?.ZoomAt(factor, origin.X);
    YAxis?.ZoomAt(factor, origin.Y);

    PlotView.InvalidatePlot(false);
    args.Handled = true;

    ZoomChanged?.Invoke(this, new SyncZoomEventArgs(
      XAxis?.ActualMinimum ?? double.NaN,
      XAxis?.ActualMaximum ?? double.NaN,
      YAxis?.ActualMinimum ?? double.NaN,
      YAxis?.ActualMaximum ?? double.NaN));
  }
}
