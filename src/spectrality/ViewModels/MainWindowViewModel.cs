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
  private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

  public SpectrogramPlotModel PlotModel1 { get; set; }
  public SpectrogramPlotModel PlotModel2 { get; set; }

  public SyncPlotController PlotController { get; set; }

  public MainWindowViewModel()
  {
    PlotModel1 = new SpectrogramPlotModel();
    PlotModel2 = new SpectrogramPlotModel();

    PlotController = new SyncPlotController(PlotModel1, PlotModel2);

    var file1 = @"/Users/juho/Documents/Projects/spectrality/test.wav";
    var file2 = @"/Users/juho/Documents/Projects/spectrality/test.wav";

    var channel1 = 0;
    var channel2 = 1;

    var skip = 0;
    var take = 0;

    var models   = new[] { PlotModel1, PlotModel2 };
    var files    = new[] { file1, file2 };
    var channels = new[] { channel1, channel2 };

    for (var i = 0; i < 2; i++)
    {
      var j = i;

      var task = new Task(() =>
      {
        Thread.Sleep(1000);

        try
        {
          Test(models[j], files[j], channels[j], skip, take);
        }
        catch (Exception exception)
        {
          var message = exception.Message;

          if (message.Length > 100)
          {
            message = string.Concat(
              message.AsSpan(0, 100),
              "...");
          }

          Logger.Error(message);
        }
      });

      task.Start();
    }
  }

  private static void Test(SpectrogramPlotModel model, string file, int channel, double skip, double take)
  {
    var reader = new AudioFileReader(file);

    var (samples, samplerate) = reader.Read(
      channel: channel,
      skip: skip,
      take: take);

    var bottleneck = new Bottleneck();
    var cancellation = new CancellationTokenSource();

    var progress = new Progress<double>(percent =>
    {
      if (percent < 100)
      {
        bottleneck.TryPass(model.Update,
          TimeSpan.FromSeconds(1));
      }
      else
      {
        bottleneck.Pass(model.Update);
      }
    });

    var analyzer = new SpectrumAnalyzer(samplerate)
    {
      ProgressCallback = progress,
      CancellationToken = cancellation.Token
    };

    var (spectrogram, task) = analyzer.GetSpectrogramTask(samples);

    model.Spectrogram = spectrogram;

    task.Start();
  }
}
