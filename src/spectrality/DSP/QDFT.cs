using System;
using System.Linq;
using System.Numerics;
using Nito.Collections;

class QDFT
{
    public double Samplerate { get; private set; }
    public (double, double) Bandwidth { get; private set; }
    public double Resolution { get; private set; }
    public double Quality { get; private set; }
    public double Latency { get; private set; }
    public (double, double) Window { get; private set; }

    public int Length { get; private set; }

    public double[] Frequencies { get; private set; }
    public double[] Qualities { get; private set; }
    public double[] Latencies { get; private set; }
    public int[] Periods { get; private set; }
    public int[] Offsets { get; private set; }
    public double[] Weights { get; private set; }

    public Complex[] Fiddles { get; private set; }
    public Complex[] Twiddles { get; private set; }

    private Deque<double> Inputs { get; set; }
    private Complex[] Outputs { get; set; }

    public QDFT(double samplerate,
                (double, double) bandwidth,
                double resolution = 24,
                double quality = 0,
                double latency = 0)
    {
        Samplerate = samplerate;
        Bandwidth = bandwidth;
        Resolution = resolution;
        Quality = quality;
        Latency = latency;
        Window = (+0.5, -0.5);

        Length = (int)Math.Ceiling(Resolution * Math.Log2(Bandwidth.Item2 / Bandwidth.Item1));

        Frequencies = new double[Length];
        Qualities = new double[Length];
        Latencies = new double[Length];
        Periods = new int[Length];
        Offsets = new int[Length];
        Weights = new double[Length];

        Fiddles = new Complex[Length * 3];
        Twiddles = new Complex[Length * 3];

        Bootstrap();

        Inputs = new Deque<double>(new double[Periods.First() + 1]);
        Outputs = new Complex[Length * 3];
    }

    private void Bootstrap()
    {
        var alpha = Math.Pow(2.0, 1.0 / Resolution) - 1.0;
        var beta = (Quality < 0) ? (alpha * 24.7 / 0.108) : Quality;

        for (int i = 0; i < Length; i++)
        {
            var frequency = Bandwidth.Item1 * Math.Pow(2.0, i / Resolution);

            Frequencies[i] = frequency;

            var quality = frequency / (alpha * frequency + beta);

            Qualities[i] = quality;

            var period = Math.Ceiling(quality * Samplerate / frequency);

            Periods[i] = (int)period;

            var offset = Math.Ceiling((Periods.First() - period)
                * Math.Clamp(Latency * 0.5 + 0.5, 0.0, 1.0));

            Offsets[i] = (int)offset;

            var latency = (Periods.First() - offset) / Samplerate;

            Latencies[i] = latency;

            var weight = 1.0 / period;

            Weights[i] = weight;
        }

        foreach (int k in new[] { -1, 0, +1 })
        {
            for (int i = 0, j = 1; i < Length; ++i, j+=3)
            {
                var fiddle = Complex.FromPolarCoordinates(
                    1.0, -2.0 * Math.PI * (Qualities[i] + k));

                Fiddles[j + k] = fiddle;

                var twiddle = Complex.FromPolarCoordinates(
                    1.0, +2.0 * Math.PI * (Qualities[i] + k) / Periods[i]);

                Twiddles[j + k] = twiddle;
            }
        }
    }

    public void Reset()
    {
        for (int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i] = 0;
        }

        for (int i = 0; i < Outputs.Length; i++)
        {
            Outputs[i] = 0;
        }
    }

    public void Analyze(double sample, Span<Complex> dft)
    {
        if (dft.Length != Length)
        {
            throw new ArgumentException();
        }

        var periods = Periods;
        var offsets = Offsets;
        var weights = Weights;

        var fiddles = Fiddles;
        var twiddles = Twiddles;

        var inputs = Inputs;
        var outputs = Outputs;

        var a = Window.Item1;
        var b = Window.Item2 / 2;

        inputs.RemoveFromFront();
        inputs.AddToBack(sample);

        for (int i = 0, j = 1; i < Length; ++i, j+=3)
        {
          var period = periods[i];
          var offset = offsets[i];
          var weight = weights[i];

          var left = inputs[offset + period];
          var right = inputs[offset];

          var k1 = j - 1;
          var k2 = j;
          var k3 = j + 1;

          var delta1 = (fiddles[k1] * left - right) * weight;
          var delta2 = (fiddles[k2] * left - right) * weight;
          var delta3 = (fiddles[k3] * left - right) * weight;

          outputs[k1] = twiddles[k1] * (outputs[k1] + delta1);
          outputs[k2] = twiddles[k2] * (outputs[k2] + delta2);
          outputs[k3] = twiddles[k3] * (outputs[k3] + delta3);

          dft[i] = outputs[k2] * a + (outputs[k1] + outputs[k3]) * b;
        }
    }
}
