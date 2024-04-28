using OxyPlot;
using OxyPlot.Axes;
using Spectrality.DSP;
using Spectrality.IO;
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

    var analyzer = new SpectrumAnalyzer(samplerate, 10e-3);
    var spectrogram = analyzer.GetSpectrogram(samples);

    PlotModel = new PlotModel()
    {
      Background = OxyColors.Black,
      TextColor = OxyColors.White,
      PlotAreaBorderColor = OxyColors.White
    };

    PlotModel.Axes.Add(new LogarithmicAxis()
    {
      Position = AxisPosition.Left,
      TicklineColor = OxyColors.White
    });

    PlotModel.Axes.Add(new LinearAxis()
    {
      Position = AxisPosition.Bottom,
      TicklineColor = OxyColors.White
    });

    PlotModel.Series.Add(new SpectrogramSeries(spectrogram));
  }
}
