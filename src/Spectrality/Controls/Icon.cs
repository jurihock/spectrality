using FluentIcons.Avalonia;

namespace Spectrality.Controls;

public sealed class Icon : SymbolIcon
{
  public Icon()
  {
    IsFilled = true;
    UseSegoeMetrics = true;

    Height = FontSize;
    Width = FontSize;
  }
}
