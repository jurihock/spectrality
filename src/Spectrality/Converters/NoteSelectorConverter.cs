using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Spectrality.Converters;

public sealed class NoteSelectorConverter : IValueConverter
{
  public object? Convert(object? value, Type type, object? parameter, CultureInfo culture)
  {
    if (value is not string)
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(value)),
        BindingErrorType.Error);
    }

    if ((Nullable.GetUnderlyingType(type) ?? type) != typeof(bool))
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(type)),
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

  public object? ConvertBack(object? value, Type type, object? parameter, CultureInfo culture)
  {
    if (value is not bool)
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(value)),
        BindingErrorType.Error);
    }

    if ((Nullable.GetUnderlyingType(type) ?? type) != typeof(string))
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(type)),
        BindingErrorType.Error);
    }

    if (parameter is not string)
    {
      return new BindingNotification(
        new ArgumentOutOfRangeException(nameof(parameter)),
        BindingErrorType.Error);
    }

    return (bool)value ? parameter.ToString() : null;
  }
}
