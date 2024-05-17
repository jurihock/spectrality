using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;

namespace Spectrality.Converters;

public sealed class NumericTextConverter : ValueConverterBase
{
  private static readonly Dictionary<Type, Func<object?, object?>> SupportedNumericTypes = new()
  {
    { typeof(double), x => double.TryParse(x?.ToString(), out double y) ? y : null }
  };

  protected override object? Convert(object value, Type srcType, Type dstType, object? parameter, CultureInfo culture)
  {
    if (!SupportedNumericTypes.ContainsKey(srcType))
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

    return value.ToString();
  }

  protected override object? ConvertBack(object value, Type srcType, Type dstType, object? parameter, CultureInfo culture)
  {
    if (srcType != typeof(string))
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(srcType)),
        BindingErrorType.Error);
    }

    if (!SupportedNumericTypes.TryGetValue(dstType, out Func<object?, object?>? convert))
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(dstType)),
        BindingErrorType.Error);
    }

    return convert(value);
  }
}
