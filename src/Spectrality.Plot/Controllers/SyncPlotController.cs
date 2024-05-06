using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using Spectrality.Plot.Manipulators;
using Spectrality.Plot.Series;

namespace Spectrality.Plot.Controllers;

public sealed class SyncPlotController : PlotController
{
  public IReadOnlyList<PlotModel> SyncPlotModels { get; private init; }

  public SyncPlotController(params PlotModel[] syncPlotModels)
  {
    SyncPlotModels = syncPlotModels.ToList();

    UnbindAll();
    BindSync();
  }

  public void ResetAllAxes()
  {
    foreach (var syncPlotModel in SyncPlotModels)
    {
      syncPlotModel.ResetAllAxes();
    }

    OnResetAllAxes();
  }

  private void BindSync()
  {
    this.BindMouseDown(OxyMouseButton.Right, OxyModifierKeys.None, 2,
      new DelegatePlotCommand<OxyMouseEventArgs>(
        (IPlotView view, IController controller, OxyMouseEventArgs args) =>
        {
          ResetAllAxes();
          args.Handled = true;
        }));

    this.BindMouseDown(OxyMouseButton.Right,
      new DelegatePlotCommand<OxyMouseDownEventArgs>(
        (IPlotView view, IController controller, OxyMouseDownEventArgs args) =>
        {
          var manipulator = new SyncPanZoomManipulator(view);
          manipulator.PanZoomChanged += OnMasterPanZoomChanged;
          controller.AddMouseManipulator(view, manipulator, args);
        }));

    this.BindMouseDown(OxyMouseButton.Left,
      new DelegatePlotCommand<OxyMouseDownEventArgs>(
        (IPlotView view, IController controller, OxyMouseDownEventArgs args) =>
        {
          var manipulator = new SyncTrackerManipulator(view);
          manipulator.TrackerChanged += OnMasterTrackerChanged;
          controller.AddMouseManipulator(view, manipulator, args);
        }));
  }

  private void OnResetAllAxes()
  {
    var filter = (OxyPlot.Series.Series series) =>
      (series is ISyncSeries) &&
      (series is XYAxisSeries);

    var cast = (OxyPlot.Series.Series? series) =>
      (XYAxisSeries)(series ?? throw new InvalidOperationException());

    var slaves = SyncPlotModels
      .Select(model => (model, series: model.Series.FirstOrDefault(filter)))
      .Where(slave => slave.series != null)
      .Select(slave => (slave.model, series: cast(slave.series)))
      .ToList();

    var minX = slaves.Min(_ => _.series.XAxis.ActualMinimum);
    var maxX = slaves.Max(_ => _.series.XAxis.ActualMaximum);

    var minY = slaves.Min(_ => _.series.YAxis.ActualMinimum);
    var maxY = slaves.Max(_ => _.series.YAxis.ActualMaximum);

    foreach (var (slavePlotModel, slavePlotSeries) in slaves)
    {
      var invalidate = false;

      if (slavePlotSeries.XAxis.IsZoomEnabled)
      {
        slavePlotSeries.XAxis.Zoom(
          minX,
          maxX);

        invalidate = true;
      }

      if (slavePlotSeries.YAxis.IsZoomEnabled)
      {
        slavePlotSeries.YAxis.Zoom(
          minY,
          maxY);

        invalidate = true;
      }

      if (invalidate)
      {
        Task.Factory.StartNew(
          _ => (_ as PlotModel)?.InvalidatePlot(false),
          slavePlotModel);
      }
    }
  }

  private void OnMasterPanZoomChanged(object? sender, SyncPanZoomEventArgs args)
  {
    var masterPlotManipulator = sender as PlotManipulator<OxyMouseEventArgs>;
    var masterPlotModel = masterPlotManipulator?.PlotView?.ActualModel;

    if (masterPlotModel == null)
    {
      return;
    }

    var filter = (OxyPlot.Series.Series series) =>
      (series is ISyncSeries) &&
      (series is XYAxisSeries);

    var cast = (OxyPlot.Series.Series? series) =>
      (XYAxisSeries)(series ?? throw new InvalidOperationException());

    var slaves = SyncPlotModels
      .Where(model => model != masterPlotModel)
      .Select(model => (model, series: model.Series.FirstOrDefault(filter)))
      .Where(slave => slave.series != null)
      .Select(slave => (slave.model, series: cast(slave.series)));

    foreach (var (slavePlotModel, slavePlotSeries) in slaves)
    {
      var invalidate = false;

      if (slavePlotSeries.XAxis.IsZoomEnabled)
      {
        slavePlotSeries.XAxis.Zoom(
          args.ActualMinimumX,
          args.ActualMaximumX);

        invalidate = true;
      }

      if (slavePlotSeries.YAxis.IsZoomEnabled)
      {
        slavePlotSeries.YAxis.Zoom(
          args.ActualMinimumY,
          args.ActualMaximumY);

        invalidate = true;
      }

      if (invalidate)
      {
        Task.Factory.StartNew(
          _ => (_ as PlotModel)?.InvalidatePlot(false),
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

    var filter = (OxyPlot.Series.Series series) =>
      (series is ISyncSeries) &&
      (series is XYAxisSeries);

    var cast = (OxyPlot.Series.Series? series) =>
      (XYAxisSeries)(series ?? throw new InvalidOperationException());

    var slaves = SyncPlotModels
      .Where(model => model != masterPlotModel)
      .Select(model => (model, series: model.Series.FirstOrDefault(filter)))
      .Where(slave => slave.series != null)
      .Select(slave => (slave.model, series: cast(slave.series)));

    foreach (var (slavePlotModel, slavePlotSeries) in slaves)
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
