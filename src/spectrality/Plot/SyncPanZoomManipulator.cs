using System;
using OxyPlot;

namespace Spectrality.Plot;

public sealed class SyncPanZoomEventArgs(double minX, double maxX, double minY, double maxY) : EventArgs
{
  public double ActualMinimumX { get; private init; } = minX;
  public double ActualMaximumX { get; private init; } = maxX;

  public double ActualMinimumY { get; private init; } = minY;
  public double ActualMaximumY { get; private init; } = maxY;
}

public sealed class SyncPanZoomManipulator : MouseManipulator
{
  private ScreenPoint PreviousPosition;
  private bool IsPanEnabled;
  private bool IsZoomEnabled;

  public double ZoomThreshold { get; init; } = double.Epsilon;
  public double ZoomDirection { get; init; } = -1;
  public double ZoomGamma { get; init; } = 1.5;
  public double ZoomSensitivity { get; init; } = 1e-2;

  public event EventHandler<SyncPanZoomEventArgs>? PanZoomChanged;

  public SyncPanZoomManipulator(IPlotView view) : base(view)
  {
  }

  public override void Started(OxyMouseEventArgs args)
  {
    base.Started(args);

    PreviousPosition = args.Position;

    IsPanEnabled =
      (XAxis?.IsPanEnabled ?? false) ||
      (YAxis?.IsPanEnabled ?? false);

    IsZoomEnabled =
      (XAxis?.IsZoomEnabled ?? false) ||
      (YAxis?.IsZoomEnabled ?? false);

    if (!IsPanEnabled && !IsZoomEnabled)
    {
      return;
    }

    View.SetCursorType(CursorType.Pan);
    args.Handled = true;
  }

  public override void Completed(OxyMouseEventArgs args)
  {
    base.Completed(args);

    if (!IsPanEnabled && !IsZoomEnabled)
    {
      return;
    }

    View.SetCursorType(CursorType.Default);
    args.Handled = true;
  }

  public override void Delta(OxyMouseEventArgs args)
  {
    base.Delta(args);

    if (!IsPanEnabled && !IsZoomEnabled)
    {
      return;
    }

    var x = PreviousPosition;
    var y = args.Position;

    var vector = y - x;
    var length = double.Hypot(vector.X, vector.Y);

    var zoom = (length > ZoomThreshold) && (double.Abs(vector.Y) > double.Abs(vector.X));
    var invalidate = false;

    if (zoom && IsZoomEnabled)
    {
      var factor = double.Sign(vector.Y) * ZoomDirection *
                   double.Pow(length, ZoomGamma) * ZoomSensitivity;

      if (factor > 0)
      {
        factor = 1.0 + factor;
      }
      else
      {
        factor = 1.0 / (1.0 - factor);
      }

      var z = InverseTransform(y.X, y.Y);

      XAxis?.ZoomAt(factor, z.X);
      YAxis?.ZoomAt(factor, z.Y);

      invalidate = true;
    }
    else if (!zoom && IsPanEnabled)
    {
      XAxis?.Pan(x, y);
      YAxis?.Pan(x, y);

      invalidate = true;
    }

    if (!invalidate)
    {
      return;
    }

    PlotView.InvalidatePlot(false);
    PreviousPosition = y;
    args.Handled = true;

    PanZoomChanged?.Invoke(this, new SyncPanZoomEventArgs(
      XAxis?.ActualMinimum ?? double.NaN,
      XAxis?.ActualMaximum ?? double.NaN,
      YAxis?.ActualMinimum ?? double.NaN,
      YAxis?.ActualMaximum ?? double.NaN));
  }
}
