﻿using System;
using NAudio.Wave;
using OxyPlot;
using OxyPlot.Axes;

namespace Spectrality.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #pragma warning disable CA1822 // Mark members as static
    public string Greeting => "Welcome to Avalonia!";
    #pragma warning restore CA1822 // Mark members as static

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

        var spectrogram = new SpectrumAnalyzer(reader.WaveFormat.SampleRate, 10e-3).GetSpectrogram(samples);

        PlotModel = new PlotModel()
        {
            Background = OxyColors.Black,
            TextColor = OxyColors.White,
            PlotAreaBorderColor = OxyColors.White
        };
        PlotModel.Series.Add(new SpectrogramSeries(spectrogram));
    }
}