using System;
using OxyPlot;
using OxyPlot.Axes;
using Spectrality.DSP;
using Spectrality.IO;
using Spectrality.Misc;
using Spectrality.Plot;

namespace Spectrality.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
  public PlotModel PlotModel { get; set; }

  public MainWindowViewModel()
  {
    var file = @"/Users/juho/Documents/Projects/spectrality/x.wav";

    var reader = new AudioFileReader(file);
    var (samples, samplerate) = reader.Read();

    var series = new SpectrogramSeries();
    var bottleneck = new Bottleneck();

    var progress = new Progress<double>(percent =>
    {
      if (percent < 10)
      {
        return;
      }
      else if (percent < 100)
      {
        bottleneck.TryPass(series.Update,
          TimeSpan.FromSeconds(1));
      }
      else
      {
        bottleneck.Pass(series.Update);
      }
    });

    var analyzer = new SpectrumAnalyzer(samplerate, 10e-3);
    var (spectrogram, task) = analyzer.GetSpectrogramTask(samples, progress);

    series.Spectrogram = spectrogram;

    PlotModel = new PlotModel()
    {
      Background = OxyColors.Black,
      TextColor = OxyColors.White,
      PlotAreaBorderColor = OxyColors.White
    };

    PlotModel.Axes.Add(new LinearAxis()
    {
      Position = AxisPosition.Bottom,
      TicklineColor = OxyColors.White
    });

    PlotModel.Axes.Add(new LogarithmicAxis()
    {
      Position = AxisPosition.Left,
      TicklineColor = OxyColors.White,
      IsPanEnabled = false,
      IsZoomEnabled = false
    });

    PlotModel.Series.Add(series);

    task.Start();
  }
}
