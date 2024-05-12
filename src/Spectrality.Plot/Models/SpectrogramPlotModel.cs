using OxyPlot;
using OxyPlot.Axes;
using Spectrality.Models;
using Spectrality.Plot.Series;

namespace Spectrality.Plot.Models;

public sealed class SpectrogramPlotModel : PlotModel
{
  private static readonly OxyColor DefaultBackgroundColor = OxyColors.Black;
  private static readonly OxyColor DefaultForegroundColor = OxyColors.White;

  public LinearAxis AxisX { get; init; }
  public LogarithmicAxis AxisY { get; init; }

  public SpectrogramSeries SpectrogramSeries { get; init; }

  public Spectrogram? Spectrogram
  {
    get => SpectrogramSeries.Spectrogram;
    set => SpectrogramSeries.Spectrogram = value;
  }

  public SpectrogramPlotModel()
  {
    Background = DefaultBackgroundColor;
    TextColor = DefaultForegroundColor;
    PlotAreaBorderColor = DefaultForegroundColor;

    AxisX = new LinearAxis()
    {
      Position = AxisPosition.Bottom,
      TicklineColor = DefaultForegroundColor,
      IsPanEnabled = true,
      IsZoomEnabled = true
    };
    Axes.Add(AxisX);

    AxisY = new LogarithmicAxis()
    {
      Position = AxisPosition.Left,
      TicklineColor = DefaultForegroundColor,
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
