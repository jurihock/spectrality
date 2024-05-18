using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace Spectrality.Controls;

public sealed class FocusableTextBox : TextBox
{
  protected override Type StyleKeyOverride => typeof(TextBox);

  protected override void OnGotFocus(GotFocusEventArgs args)
  {
    Dispatcher.UIThread.Post(SelectAll);

    base.OnGotFocus(args);
  }
}
