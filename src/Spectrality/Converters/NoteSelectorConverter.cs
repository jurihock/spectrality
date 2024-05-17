using System;
using System.Globalization;
using Avalonia.Data;

namespace Spectrality.Converters;

public sealed class NoteSelectorConverter : ValueConverterBase
{
  protected override object? Convert(object value, Type srcType, Type dstType, object? parameter, CultureInfo culture)
  {
    if (srcType != typeof(string))
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(srcType)),
        BindingErrorType.Error);
    }

    if (dstType != typeof(bool))
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(dstType)),
        BindingErrorType.Error);
    }

    if (parameter is not string)
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(parameter)),
        BindingErrorType.Error);
    }

    return string.Equals(
      value.ToString(),
      parameter.ToString(),
      StringComparison.OrdinalIgnoreCase);
  }

  protected override object? ConvertBack(object value, Type srcType, Type dstType, object? parameter, CultureInfo culture)
  {
    if (srcType != typeof(bool))
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(srcType)),
        BindingErrorType.Error);
    }

    if (dstType != typeof(string))
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(dstType)),
        BindingErrorType.Error);
    }

    if (parameter is not string)
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(parameter)),
        BindingErrorType.Error);
    }

    return (bool)value
      ? parameter.ToString()
      : null;
  }
}
