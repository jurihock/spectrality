using System;
using FluentIcons.Avalonia;

namespace Spectrality.Controls;

public sealed class Icon : SymbolIcon
{
  protected override Type StyleKeyOverride => typeof(SymbolIcon);

  public Icon()
  {
    IsFilled = true;
    UseSegoeMetrics = true;

    Height = FontSize;
    Width = FontSize;
  }
}
