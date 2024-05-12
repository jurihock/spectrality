using System.Linq;
using Spectrality.Misc;

namespace Spectrality.ViewModels;

public sealed class NoteSelectorViewModel : ViewModelBase
{
  public int MinOctave { get; private init; }
  public int MaxOctave { get; private init; }

  private int octave;
  public int Octave
  {
    get => octave;
    set
    {
      SetAndNotifyIfChanged(ref octave, value).AlsoNotify(
        nameof(NoteString), nameof(FrequencyString));
    }
  }

  private string note = string.Empty;
  public string Note
  {
    get => note;
    set
    {
      SetAndNotifyIfChanged(ref note, value ?? note).AlsoNotify(
        nameof(NoteString), nameof(FrequencyString));
    }
  }

  private double a4;
  public double A4
  {
    get => a4;
    set
    {
      SetAndNotifyIfChanged(ref a4, value).AlsoNotify(
        nameof(NoteString), nameof(FrequencyString));
    }
  }

  public string NoteString => ToString();
  public string FrequencyString => $"{ToFrequency():F1} Hz";

  public NoteSelectorViewModel(string note = "C", int octave = 0, double a4 = 440)
  {
    MinOctave = Scale.Octaves.Min();
    MaxOctave = Scale.Octaves.Max();

    Note = note;
    Octave = octave;
    A4 = a4;
  }

  public override string ToString() => $"{Note}{Octave}";
  public double ToFrequency() => new Scale(A4).GetFrequency(NoteString);
}
