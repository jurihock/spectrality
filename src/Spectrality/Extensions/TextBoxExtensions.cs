using Avalonia.Controls;

namespace Spectrality.Extensions;

public static class TextBoxExtensions
{
  public static void SelectNone(this TextBox textbox)
  {
    textbox.CaretIndex = 0;
    textbox.SelectionStart = 0;
    textbox.SelectionEnd = 0;
  }
}
