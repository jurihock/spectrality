using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    // TODO: https://github.com/oxyplot/oxyplot/blob/master/Source/OxyPlot/PlotController/PlotController.cs

    this.BindMouseDown(OxyMouseButton.Right, new DelegatePlotCommand<OxyMouseDownEventArgs>(
      (IPlotView view, IController controller, OxyMouseDownEventArgs args) =>
      {
        var manipulator = new SyncPanManipulator(view);

        manipulator.PanChanged += OnMasterPanChanged;

        controller.AddMouseManipulator(view, manipulator, args);
      }));

    this.BindMouseDown(OxyMouseButton.Left, new DelegatePlotCommand<OxyMouseDownEventArgs>(
      (IPlotView view, IController controller, OxyMouseDownEventArgs args) =>
      {
        var manipulator = new SyncTrackerManipulator(view);

        manipulator.TrackerChanged += OnMasterTrackerChanged;

        controller.AddMouseManipulator(view, manipulator, args);
      }));

    this.BindMouseWheel(new DelegatePlotCommand<OxyMouseWheelEventArgs>(
      (IPlotView view, IController controller, OxyMouseWheelEventArgs args) =>
      {
        var fine = true;
        var factor = fine ? 0.1 : 1.0;

        var manipulator = new SyncZoomManipulator(view)
        {
          Fine = fine,
          Step = args.Delta * 1e-3 * factor
        };

        manipulator.Started(args);
      }));
  }

  private void OnMasterPanChanged(object? sender, SyncPanEventArgs args)
  {
    var masterPlotManipulator = sender as PlotManipulator<OxyMouseEventArgs>;
    var masterPlotModel = masterPlotManipulator?.PlotView?.ActualModel;

    if (masterPlotModel == null)
    {
      return;
    }

    var panableSlaves = SyncPlotModels
      .Where(model => model != masterPlotModel)
      .Select(model => (model, series: model.Series.FirstOrDefault(series => series is IPanableSeries)))
      .Where(slave => slave.series is XYAxisSeries)
      .Select(slave => (slave.model, (XYAxisSeries)(slave.series ?? throw new InvalidOperationException())));

    foreach (var (slavePlotModel, slavePlotSeries) in panableSlaves)
    {
      var invalidate = false;

      if (slavePlotSeries.XAxis.IsPanEnabled)
      {
        slavePlotSeries.XAxis.Zoom(
          args.ActualMinimumX,
          args.ActualMaximumX);

        invalidate = true;
      }

      if (slavePlotSeries.YAxis.IsPanEnabled)
      {
        slavePlotSeries.YAxis.Zoom(
          args.ActualMinimumY,
          args.ActualMaximumY);

        invalidate = true;
      }

      if (invalidate)
      {
        Task.Factory.StartNew(
          model => (model as PlotModel)?.InvalidatePlot(false),
          slavePlotModel);
      }
    }
  }

  private void OnMasterTrackerChanged(object? sender, SyncTrackerEventArgs args)
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
