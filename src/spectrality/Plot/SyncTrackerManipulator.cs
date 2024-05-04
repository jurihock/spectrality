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

  public event EventHandler<SyncTrackerEventArgs>? TrackerChanged;

  public SyncTrackerManipulator(IPlotView view) : base(view)
  {
    TrackableSeries = PlotView.ActualModel.Series.FirstOrDefault(series => series is ITrackableSeries);
  }

  public override void Started(OxyMouseEventArgs args)
  {
    base.Started(args);
    args.Handled = true;

    Delta(args);
  }

  public override void Completed(OxyMouseEventArgs args)
  {
    base.Completed(args);
    args.Handled = true;

    HideTracker();
  }

  public override void Delta(OxyMouseEventArgs args)
  {
    base.Delta(args);
    args.Handled = true;

    if (TrackableSeries == null)
    {
      return;
    }

    if (!PlotView.ActualModel.PlotArea.Contains(args.Position))
    {
      return;
    }

    var hit = TrackableSeries.GetNearestPoint(args.Position, interpolate: false);

    if (hit == null)
    {
      return;
    }

    ShowTracker(hit);
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
