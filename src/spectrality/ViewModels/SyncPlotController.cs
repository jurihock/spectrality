using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;

namespace Spectrality.Plot;

public sealed class SyncPlotController : PlotController
{
  public List<PlotModel> SyncPlotModels { get; private init; }

  public SyncPlotController(params PlotModel[] syncPlotModels)
  {
    SyncPlotModels = syncPlotModels.ToList();

    UnbindAll();

    this.BindMouseDown(OxyMouseButton.Right, PlotCommands.PanAt);

    this.BindMouseDown(OxyMouseButton.Left, new DelegatePlotCommand<OxyMouseDownEventArgs>(
      (IPlotView view, IController controller, OxyMouseDownEventArgs args) =>
      {
        var manipulator = new SyncTrackerManipulator(view);

        manipulator.TrackerChanged += OnTrackerChanged;

        controller.AddMouseManipulator(view, manipulator, args);
      }));
  }

  private void OnTrackerChanged(object? sender, SyncTrackerEventArgs args)
  {
    var masterPlotManipulator = sender as PlotManipulator<OxyMouseEventArgs>;
    var masterPlotModel = masterPlotManipulator?.PlotView?.ActualModel;

    if (masterPlotModel == null)
    {
      return;
    }

    var trackableSlaves = SyncPlotModels
      .Where(model => model != masterPlotModel)
      .Select(model => (model, series: model.Series.FirstOrDefault(series => series is ITrackableSeries)))
      .Where(slave => slave.series is XYAxisSeries)
      .Select(slave => (slave.model, (XYAxisSeries)(slave.series ?? throw new InvalidOperationException())));

    foreach (var (slavePlotModel, slavePlotSeries) in trackableSlaves)
    {
      var worldPoint = args.WorldPoint;

      if (worldPoint == null)
      {
        slavePlotModel.PlotView?.HideTracker();
        continue;
      }

      var screenPoint = slavePlotSeries.Transform(worldPoint.Value);

      var hit = slavePlotSeries.GetNearestPoint(screenPoint, interpolate: false);

      if (hit == null)
      {
        continue;
      }

      hit.PlotModel = slavePlotModel;

      slavePlotModel.PlotView?.ShowTracker(hit);
    }
  }
}
