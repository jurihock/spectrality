using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Spectrality.Converters;

public abstract class ValueConverterBase : IValueConverter
{
  public object? Convert(object? value, Type type, object? parameter, CultureInfo culture)
  {
    ArgumentNullException.ThrowIfNull(value);

    static Type getType(Type t) => Nullable.GetUnderlyingType(t) ?? t;
    var srcType = getType(value.GetType());
    var dstType = getType(type);

    return Convert(value, srcType, dstType, parameter, culture);
  }

  public object? ConvertBack(object? value, Type type, object? parameter, CultureInfo culture)
  {
    ArgumentNullException.ThrowIfNull(value);

    static Type getType(Type t) => Nullable.GetUnderlyingType(t) ?? t;
    var srcType = getType(value.GetType());
    var dstType = getType(type);

    return ConvertBack(value, srcType, dstType, parameter, culture);
  }

  protected abstract object? Convert(object value, Type srcType, Type dstType, object? parameter, CultureInfo culture);
  protected abstract object? ConvertBack(object value, Type srcType, Type dstType, object? parameter, CultureInfo culture);
}
