using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

class SpectrumAnalyzer
{
    public double Samplerate { get; private set; }
    public double Hopsize { get; private set; }

    private readonly QDFT QDFT;

    public SpectrumAnalyzer(double samplerate, double hopsize)
    {
        Samplerate = samplerate;
        Hopsize = hopsize;

        QDFT = new QDFT(samplerate, (100, 10000), 12 * 4);
    }

    public Spectrogram GetSpectrogram(Span<float> samples)
    {
        var watch = Stopwatch.GetTimestamp();

        var qdft = QDFT;

        var hop = (int)Math.Ceiling(1.0 * Hopsize * Samplerate);
        var hops = (int)Math.Ceiling(1.0 * samples.Length / hop);
        var bins = qdft.Length;

        var timestamps = Enumerable.Range(0, hops).Select(_ => _ * Hopsize).ToArray();
        var frequencies = qdft.Frequencies;
        var magnitudes = new float[hops, bins];
        var dft = new Complex[bins];

        qdft.Reset();

        for (var i = 0; i < samples.Length; i++)
        {
            qdft.Analyze(samples[i], dft);

            if (i % hop != 0)
                continue;
            
            var t = i / hop;

            for (var j = 0; j < bins; j++)
            {
                var magnitude = dft[j].Magnitude;

                magnitude = 20.0 * Math.Log10(
                    magnitude + double.Epsilon);

                magnitudes[t, j] = (float)magnitude;
            }
        }

        System.Console.WriteLine($"{Stopwatch.GetElapsedTime(watch).Milliseconds}ms");

        return new Spectrogram
        {
            Samplerate = Samplerate,
            Timestamps = timestamps,
            Frequencies = frequencies,
            Magnitudes = magnitudes
        };
    }
}
