using System.Linq;
using Avalonia;
using Spectrality.Misc;

namespace Spectrality.Controls;

public sealed partial class NoteSelectorControl : TemplatedControlBase
{
  private readonly string[] AllProperties;

  public static readonly DirectProperty<NoteSelectorControl, int> MinOctaveProperty =
    AvaloniaProperty.RegisterDirect<NoteSelectorControl, int>(
      nameof(MinOctave), _ => _.MinOctave);

  public static readonly DirectProperty<NoteSelectorControl, int> MaxOctaveProperty =
    AvaloniaProperty.RegisterDirect<NoteSelectorControl, int>(
      nameof(MaxOctave), _ => _.MaxOctave);

  public static readonly DirectProperty<NoteSelectorControl, int> OctaveProperty =
    AvaloniaProperty.RegisterDirect<NoteSelectorControl, int>(
      nameof(Octave), _ => _.Octave, (_, value) => _.Octave = value);

  public static readonly DirectProperty<NoteSelectorControl, string> NoteProperty =
    AvaloniaProperty.RegisterDirect<NoteSelectorControl, string>(
      nameof(Note), _ => _.Note, (_, value) => _.Note = value);

  public static readonly DirectProperty<NoteSelectorControl, double> A4Property =
    AvaloniaProperty.RegisterDirect<NoteSelectorControl, double>(
      nameof(A4), _ => _.A4, (_, value) => _.A4 = value);

  public static readonly DirectProperty<NoteSelectorControl, string> NoteStringProperty =
    AvaloniaProperty.RegisterDirect<NoteSelectorControl, string>(
      nameof(NoteString), _ => _.NoteString);

  public static readonly DirectProperty<NoteSelectorControl, string> FrequencyStringProperty =
    AvaloniaProperty.RegisterDirect<NoteSelectorControl, string>(
      nameof(FrequencyString), _ => _.FrequencyString);

  public int MinOctave { get; private init; } = Scale.Octaves.Min();
  public int MaxOctave { get; private init; } = Scale.Octaves.Max();

  public int Octave
  {
    get => octave;
    set => SetAndNotifyIfChanged(ref octave, value).AlsoNotify(AllProperties);
  }

  public string Note
  {
    get => note;
    set => SetAndNotifyIfChanged(ref note, value ?? note).AlsoNotify(AllProperties);
  }

  public double A4
  {
    get => a4;
    set => SetAndNotifyIfChanged(ref a4, value).AlsoNotify(AllProperties);
  }

  public string NoteString => $"{Note}{Octave}";
  public string FrequencyString => $"{new Scale(A4).GetFrequency($"{Note}{Octave}"):F1} Hz";

  private int octave = 0;
  private string note = "C";
  private double a4 = 440;

  public NoteSelectorControl()
  {
    AllProperties = DirectProperties.Keys.ToArray();
  }
}
