using Avalonia.Controls;

namespace Spectrality.Extensions;

public static class TextBoxExtensions
{
  public static void SelectNone(this TextBox textbox)
  {
    textbox.SelectionStart = textbox.Text?.Length ?? 0;
    textbox.SelectionEnd = textbox.SelectionStart;
  }
}
