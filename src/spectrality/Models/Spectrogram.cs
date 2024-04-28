namespace Spectrality.Models;

public struct Spectrogram
{
    public double Samplerate;
    public double[] Timepoints;
    public double[] Frequencies;
    public float[,] Magnitudes;
}
