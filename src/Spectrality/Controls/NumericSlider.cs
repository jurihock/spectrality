using System;
using Avalonia.Controls;

namespace Spectrality.Controls;

public sealed class NumericSlider : Slider
{
  protected override Type StyleKeyOverride => typeof(Slider);

  public NumericSlider()
  {
    IsSnapToTickEnabled = true;
    TickFrequency = 1;
  }
}
