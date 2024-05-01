using OxyPlot;
using OxyPlot.Axes;
using Spectrality.Models;

namespace Spectrality.Plot;

public class SpectrogramPlotModel : PlotModel
{
  private LinearAxis AxisX { get; init; }
  private LogarithmicAxis AxisY { get; init; }

  private SpectrogramSeries SpectrogramSeries { get; init; }

  public Spectrogram? Spectrogram
  {
    get => SpectrogramSeries.Spectrogram;
    set => SpectrogramSeries.Spectrogram = value;
  }

  public SpectrogramPlotModel()
  {
    Background = OxyColors.Black;
    TextColor = OxyColors.White;
    PlotAreaBorderColor = OxyColors.White;

    AxisX = new LinearAxis()
    {
      Position = AxisPosition.Bottom,
      TicklineColor = OxyColors.White,
      IsPanEnabled = true,
      IsZoomEnabled = true
    };
    Axes.Add(AxisX);

    AxisY = new LogarithmicAxis()
    {
      Position = AxisPosition.Left,
      TicklineColor = OxyColors.White,
      IsPanEnabled = false,
      IsZoomEnabled = false
    };
    Axes.Add(AxisY);

    SpectrogramSeries = new SpectrogramSeries();
    Series.Add(SpectrogramSeries);
  }

  public void Update()
  {
    SpectrogramSeries.Update();
    InvalidatePlot(true);
  }
}
