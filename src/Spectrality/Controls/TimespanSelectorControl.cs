using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;

namespace Spectrality.Controls;

public sealed partial class TimespanSelectorControl : TemplatedControlBase
{
  public static readonly DirectProperty<TimespanSelectorControl, double?> BeginProperty =
    AvaloniaProperty.RegisterDirect<TimespanSelectorControl, double?>(
      nameof(Begin), _ => _.Begin, (_, value) => _.Begin = value);

  public static readonly DirectProperty<TimespanSelectorControl, double?> EndProperty =
    AvaloniaProperty.RegisterDirect<TimespanSelectorControl, double?>(
      nameof(End), _ => _.End, (_, value) => _.End = value);

  public static readonly DirectProperty<TimespanSelectorControl, double?> LengthProperty =
    AvaloniaProperty.RegisterDirect<TimespanSelectorControl, double?>(
      nameof(Length), _ => _.Length, (_, value) => _.Length = value);

  public static readonly DirectProperty<TimespanSelectorControl, long> TotalProperty =
    AvaloniaProperty.RegisterDirect<TimespanSelectorControl, long>(
      nameof(Total), _ => _.Total, (_, value) => _.Total = value);

  public double? Begin
  {
    get
    {
      ArgumentNullException.ThrowIfNull(begin);
      return Get(ref begin);
    }
    set
    {
      if (value is null) { return; }
      SetAndNotify(ref begin, value).AlsoInvoke(OnChange).AlsoNotifyAll();
    }
  }

  public double? End
  {
    get
    {
      ArgumentNullException.ThrowIfNull(end);
      return Get(ref end);
    }
    set
    {
      if (value is null) { return; }
      SetAndNotify(ref end, value).AlsoInvoke(OnChange).AlsoNotifyAll();
    }
  }

  public double? Length
  {
    get
    {
      ArgumentNullException.ThrowIfNull(length);
      return Get(ref length);
    }
    set
    {
      if (value is null) { return; }
      SetAndNotify(ref length, value).AlsoInvoke(OnChange).AlsoNotifyAll();
    }
  }

  public long Total
  {
    get => Get(ref total);
    set => SetAndNotify(ref total, value).AlsoInvoke(OnChange).AlsoNotifyAll();
  }

  private double? begin = 0, end = 0, length = 0;
  private long total = 0;

  private void OnChange(string changedPropertyName)
  {
    var slider = IsSliderChange();

    var begin = this.begin!.Value;
    var end = this.end!.Value;
    var length = this.length!.Value;

    switch (changedPropertyName)
    {
      case nameof(Begin):
      {
        if (begin <= end || !slider)
        {
          begin = Math.Clamp(begin, 0, end);
          length = end - begin;
        }
        else
        {
          begin = end = Math.Clamp(begin, 0, total);
          length = end - begin;
        }
        break;
      }
      case nameof(End):
      {
        if (end >= begin || !slider)
        {
          end = Math.Clamp(end, begin, total);
          length = end - begin;
        }
        else
        {
          begin = end = Math.Clamp(end, 0, total);
          length = end - begin;
        }
        break;
      }
      case nameof(Length):
      {
        if (end < total || !slider)
        {
          length = Math.Clamp(length, 0, total - begin);
          end = begin + length;
        }
        else
        {
          length = Math.Clamp(length, 0, total);
          begin = end - length;
        }
        break;
      }
      case nameof(Total):
      {
        begin = 0;
        end = total;
        length = total;
        break;
      }
    }

    this.begin = begin;
    this.end = end;
    this.length = length;
  }

  private static bool IsSliderChange()
  {
    var trace = new StackTrace();
    var frames = trace.GetFrames();

    foreach (var frame in frames)
    {
      var whois = frame.GetMethod()?.DeclaringType?.FullName;

      var slider = string.Equals(
        whois,
        typeof(Slider).FullName,
        StringComparison.Ordinal);

      var textbox = string.Equals(
        whois,
        typeof(TextBox).FullName,
        StringComparison.Ordinal);

      if (slider) { return true; }
      else if (textbox) { return false; }
    }

    return false;
  }
}
