using OxyPlot;

namespace Spectrality.Plot;

public sealed class SyncZoomManipulator : MouseManipulator
{
  public bool Fine { get; init; }
  public double Step { get; init; }

  public SyncZoomManipulator(IPlotView view) : base(view)
  {
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

    var current = InverseTransform(
      args.Position.X,
      args.Position.Y);

    var scale = Step;

    if (Fine)
    {
      scale *= 3;
    }

    if (scale > 0)
    {
      scale = 1 + scale;
    }
    else
    {
      scale = 1.0 / (1 - scale);
    }

    XAxis?.ZoomAt(scale, current.X);
    YAxis?.ZoomAt(scale, current.Y);

    PlotView.InvalidatePlot(false);
    args.Handled = true;
  }
}
