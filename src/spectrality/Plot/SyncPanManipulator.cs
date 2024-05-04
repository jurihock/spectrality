using System;
using OxyPlot;

namespace Spectrality.Plot;

public sealed class SyncPanEventArgs(double minX, double maxX, double minY, double maxY) : EventArgs
{
  public double ActualMinimumX { get; private init; } = minX;
  public double ActualMaximumX { get; private init; } = maxX;

  public double ActualMinimumY { get; private init; } = minY;
  public double ActualMaximumY { get; private init; } = maxY;
}

public sealed class SyncPanManipulator : MouseManipulator
{
  private ScreenPoint PreviousPosition;
  private bool IsPanEnabled;

  public event EventHandler<SyncPanEventArgs>? PanChanged;

  public SyncPanManipulator(IPlotView view) : base(view)
  {
  }

  public override void Started(OxyMouseEventArgs args)
  {
    base.Started(args);

    PreviousPosition = args.Position;

    IsPanEnabled =
      (XAxis?.IsPanEnabled ?? false) ||
      (YAxis?.IsPanEnabled ?? false);

    if (!IsPanEnabled)
    {
      return;
    }

    View.SetCursorType(CursorType.Pan);
    args.Handled = true;
  }

  public override void Completed(OxyMouseEventArgs args)
  {
    base.Completed(args);

    if (!IsPanEnabled)
    {
      return;
    }

    View.SetCursorType(CursorType.Default);
    args.Handled = true;
  }

  public override void Delta(OxyMouseEventArgs args)
  {
    base.Delta(args);

    if (!IsPanEnabled)
    {
      return;
    }

    XAxis?.Pan(PreviousPosition, args.Position);
    YAxis?.Pan(PreviousPosition, args.Position);

    PlotView.InvalidatePlot(false);
    PreviousPosition = args.Position;
    args.Handled = true;

    PanChanged?.Invoke(this, new SyncPanEventArgs(
      XAxis?.ActualMinimum ?? double.NaN,
      XAxis?.ActualMaximum ?? double.NaN,
      YAxis?.ActualMinimum ?? double.NaN,
      YAxis?.ActualMaximum ?? double.NaN));
  }
}
