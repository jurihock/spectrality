using System;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace Spectrality.Controls;

public sealed class NoteSelectorButton : ToggleButton
{
  // https://stackoverflow.com/a/46388707
  // https://github.com/AvaloniaUI/Avalonia/issues/3812

  protected override Type StyleKeyOverride => typeof(ToggleButton);

  public NoteSelectorButton()
  {
    HorizontalAlignment = HorizontalAlignment.Stretch;
    VerticalAlignment = VerticalAlignment.Stretch;

    HorizontalContentAlignment = HorizontalAlignment.Center;
    VerticalContentAlignment = VerticalAlignment.Center;
  }

  protected override void Toggle()
  {
    if (IsChecked != true)
    {
      base.Toggle();
    }
  }
}
