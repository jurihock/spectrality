using System;
using System.Linq;

namespace Spectrality.Misc;

public sealed class Scale
{
    public readonly string[] Notes =
        [ "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" ];

    public readonly int[] Octaves =
        Enumerable.Range(-1, 12).ToArray();

    public readonly char[] ValidNoteChars =
        "#ABCDEFG".ToCharArray();

    public readonly char[] ValidOctaveChars =
        "-0123456789".ToCharArray();

    public double A4 { get; private set; }
    public double C0 { get; private set; }

    public Scale(double a4 = 440)
    {
        A4 = a4;
        C0 = a4 * Math.Pow(2.0, -(9.0 + 4.0 * 12.0) / 12.0);
    }

    public string GetNote(double frequency)
    {
      var semitone = GetSemitone(frequency);
      var octave = GetOctave(frequency);

      return Notes[semitone] + octave.ToString();
    }

    public int GetSemitone(string note)
    {
        var chars = string.Concat(note.ToUpper().Where(ValidNoteChars.Contains));

        ArgumentOutOfRangeException.ThrowIfNullOrEmpty(chars, nameof(note));

        var semitone = Array.IndexOf(Notes, chars);

        ArgumentOutOfRangeException.ThrowIfNegative(semitone, nameof(note));

        return semitone;
    }

    public int GetSemitone(double frequency)
    {
        return (int)Math.Round(12.0 * Math.Log2(frequency / C0)) % 12;
    }

    public int GetOctave(string note)
    {
        var chars = string.Concat(note.Where(ValidOctaveChars.Contains));

        if (chars.Length == 0)
        {
            return 0;
        }

        var octave = int.Parse(chars);

        return octave;
    }

    public int GetOctave(double frequency)
    {
        return (int)Math.Round(12.0 * Math.Log2(frequency / C0)) / 12;
    }

    public double GetOctaveRatio(double frequency)
    {
        return Math.Log2(frequency / C0);
    }

    public double GetFrequency(int semitone, int octave)
    {
        return Math.Pow(2.0, semitone / 12.0 + octave) * C0;
    }

    public double GetFrequency(string note)
    {
        var semitone = GetSemitone(note);
        var octave = GetOctave(note);

        return GetFrequency(semitone, octave);
    }
}
