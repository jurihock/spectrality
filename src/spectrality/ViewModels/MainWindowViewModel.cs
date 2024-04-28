using System;
using System.Linq;
using NAudio.Wave;
using OxyPlot;
using OxyPlot.Axes;
using Spectrality.DSP;
using Spectrality.Models;
using Spectrality.Plot;

namespace Spectrality.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public PlotModel PlotModel { get; set; }

    public MainWindowViewModel()
    {
        var file = @"/Users/juho/Documents/Projects/spectrality/x.wav";
        var reader = new AudioFileReader(file);

        System.Console.WriteLine(reader.WaveFormat.SampleRate);
        System.Console.WriteLine(reader.WaveFormat.Channels);
        System.Console.WriteLine(reader.WaveFormat.BitsPerSample);
        System.Console.WriteLine(reader.Length);

        var n = reader.Length / (reader.WaveFormat.BitsPerSample / 8);
        var samples = new float[n];
        var m = reader.Read(samples, 0, samples.Length);
        System.Console.WriteLine(n);
        System.Console.WriteLine(m);
        System.Console.WriteLine();

        var analyzer = new SpectrumAnalyzer(reader.WaveFormat.SampleRate, 10e-3);

        var spectrogram = analyzer.GetSpectrogram(samples);

        PlotModel = new PlotModel()
        {
          PlotType = PlotType.XY,
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
