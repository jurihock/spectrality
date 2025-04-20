using System;
using FluentIcons.Avalonia;

namespace Spectrality.Controls;

public sealed class Icon : SymbolIcon
{
  protected override Type StyleKeyOverride => typeof(SymbolIcon);

  public Icon()
  {
    // TODO deprecated props
    // IsFilled = true;
    // UseSegoeMetrics = true;

    Height = FontSize;
    Width = FontSize;
  }
}
