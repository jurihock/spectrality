namespace Spectrality.Models;

struct Spectrogram
{
    public double Samplerate;
    public double[] Timestamps;
    public double[] Frequencies;
    public float[,] Magnitudes;
}