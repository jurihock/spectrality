using System;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;

namespace Spectrality.Plot;

public sealed class SyncTrackerEventArgs(DataPoint? point) : EventArgs
{
  public DataPoint? WorldPoint { get; private init; } = point;
}

public sealed class SyncTrackerManipulator : MouseManipulator
{
  private readonly Series? TrackableSeries;
  private readonly bool IsTrackerEnabled;

  public event EventHandler<SyncTrackerEventArgs>? TrackerChanged;

  public SyncTrackerManipulator(IPlotView view) : base(view)
  {
    TrackableSeries = PlotView.ActualModel.Series.FirstOrDefault(series => series is ISyncSeries);
    IsTrackerEnabled = TrackableSeries != null;
  }

  public override void Started(OxyMouseEventArgs args)
  {
    base.Started(args);

    if (!IsTrackerEnabled)
    {
      return;
    }

    Delta(args);

    View.SetCursorType(CursorType.ZoomRectangle);
    args.Handled = true;
  }

  public override void Completed(OxyMouseEventArgs args)
  {
    base.Completed(args);

    if (!IsTrackerEnabled)
    {
      return;
    }

    HideTracker();

    View.SetCursorType(CursorType.Default);
    args.Handled = true;
  }

  public override void Delta(OxyMouseEventArgs args)
  {
    base.Delta(args);

    if (!IsTrackerEnabled)
    {
      return;
    }

    if (!PlotView.ActualModel.PlotArea.Contains(args.Position))
    {
      return;
    }

    var hit = GetNearestPoint(args.Position);

    if (hit == null)
    {
      return;
    }

    ShowTracker(hit);

    args.Handled = true;
  }

  private TrackerHitResult? GetNearestPoint(ScreenPoint point)
  {
    return TrackableSeries?.GetNearestPoint(point, interpolate: false);
  }

  private void ShowTracker(TrackerHitResult hit)
  {
    ArgumentNullException.ThrowIfNull(hit, nameof(hit));
    hit.PlotModel = PlotView.ActualModel;

    PlotView.ShowTracker(hit);
    TrackerChanged?.Invoke(this, new SyncTrackerEventArgs(hit.DataPoint));
  }

  private void HideTracker()
  {
    PlotView.HideTracker();
    TrackerChanged?.Invoke(this, new SyncTrackerEventArgs(null));
  }
}
