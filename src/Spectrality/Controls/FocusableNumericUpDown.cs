using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Spectrality.Extensions;

namespace Spectrality.Controls;

public sealed class FocusableNumericUpDown : NumericUpDown
{
  protected override Type StyleKeyOverride => typeof(NumericUpDown);

  private TextBox? TextBox => typeof(NumericUpDown)
    .GetProperty(nameof(TextBox), BindingFlags.NonPublic | BindingFlags.Instance)?
    .GetValue(this) as TextBox;

  private void SelectAll() => TextBox?.SelectAll();
  private void SelectNone() => TextBox?.SelectNone();

  protected override void OnGotFocus(GotFocusEventArgs args)
  {
    Dispatcher.UIThread.Post(SelectAll);

    base.OnGotFocus(args);
  }

  protected override void OnLostFocus(RoutedEventArgs args)
  {
    Dispatcher.UIThread.Post(SelectNone);

    base.OnLostFocus(args);
  }
}
