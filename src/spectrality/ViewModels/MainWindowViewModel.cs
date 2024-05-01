using System;
using System.Threading;
using System.Threading.Tasks;
using Spectrality.DSP;
using Spectrality.IO;
using Spectrality.Misc;
using Spectrality.Plot;

namespace Spectrality.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
  public SpectrogramPlotModel PlotModel { get; set; }

  public MainWindowViewModel()
  {
    PlotModel = new SpectrogramPlotModel();

    new Task(() =>
    {
      Thread.Sleep(1000);
      Test();
    })
    .Start();
  }

  private void Test()
  {
    var file = @"/Users/juho/Documents/Projects/spectrality/x.wav";

    var reader = new AudioFileReader(file);
    var (samples, samplerate) = reader.Read();

    var bottleneck = new Bottleneck();
    var cancellation = new CancellationTokenSource();

    var progress = new Progress<double>(percent =>
    {
      if (percent < 100)
      {
        bottleneck.TryPass(PlotModel.Update,
          TimeSpan.FromSeconds(1));
      }
      else
      {
        bottleneck.Pass(PlotModel.Update);
      }
    });

    var analyzer = new SpectrumAnalyzer(samplerate, 10e-3)
    {
      ProgressCallback = progress,
      CancellationToken = cancellation.Token
    };

    var (spectrogram, task) = analyzer.GetSpectrogramTask(samples);

    PlotModel.Spectrogram = spectrogram;

    task.Start();
  }
}
