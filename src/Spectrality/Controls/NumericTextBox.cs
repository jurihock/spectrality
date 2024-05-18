using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace Spectrality.Controls;

public sealed class NumericTextBox : TextBox
{
  protected override Type StyleKeyOverride => typeof(TextBox);

  public NumericTextBox()
  {
    AddHandler(
      PastingFromClipboardEvent,
      OnPastingFromClipboard,
      RoutingStrategies.Bubble);
  }

  protected override void OnGotFocus(GotFocusEventArgs args)
  {
    Dispatcher.UIThread.Post(SelectAll);

    base.OnGotFocus(args);
  }

  protected override void OnLostFocus(RoutedEventArgs args)
  {
    // TODO update text binding somehow
    // https://github.com/AvaloniaUI/Avalonia/issues/14965

    base.OnLostFocus(args);
  }

  protected override void OnTextInput(TextInputEventArgs args)
  {
    args.Text = string.Concat(args.Text?.Where(char.IsDigit) ?? []);

    base.OnTextInput(args);
  }

  private async void OnPastingFromClipboard(object? sender, RoutedEventArgs args)
  {
    args.Handled = true;

    string? text = null;

    try { text = await GetClipboardTextAsync(); }
    catch (TimeoutException) {}

    text = string.Concat(text?.Where(char.IsDigit) ?? []);

    if (!string.IsNullOrEmpty(text))
    {
      SelectedText = text;
    }
  }

  private async Task<string?> GetClipboardTextAsync()
  {
    var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;

    if (clipboard != null)
    {
      try { return await clipboard.GetTextAsync(); }
      catch (TimeoutException) {}
    }

    return null;
  }
}
