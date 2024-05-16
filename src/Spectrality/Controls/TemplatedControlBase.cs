using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Spectrality.Controls;

public abstract class TemplatedControlBase : TemplatedControl
{
  protected readonly struct PropertyChange(
    TemplatedControlBase propertyOwner,
    string propertyName,
    bool isPropertyChanged)
  {
    public readonly TemplatedControlBase PropertyOwner = propertyOwner;
    public readonly string PropertyName = propertyName;
    public readonly bool IsPropertyChanged = isPropertyChanged;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AlsoNotify(params string[] propertyNames)
    {
      if (!IsPropertyChanged)
      {
        return;
      }

      foreach (var propertyName in propertyNames)
      {
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        var property = typeof(TemplatedControlBase)
          .GetMethod(nameof(GetDirectProperty), 0, bindingFlags, null, [typeof(string)], null)!
          .Invoke(PropertyOwner, [propertyName]);

        ArgumentNullException.ThrowIfNull(property);

        var tempType = Type.MakeGenericMethodParameter(0);
        var baseType = typeof(DirectPropertyBase<>).MakeGenericType(tempType);

        typeof(AvaloniaObject)
          .GetMethod(nameof(RaisePropertyChanged), 1, bindingFlags, null, [baseType, tempType, tempType], null)!
          .MakeGenericMethod(property.GetType().GenericTypeArguments.Last())
          .Invoke(PropertyOwner, [property, default, default]);
      }
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected PropertyChange SetAndNotify<T>(
    ref T oldValue,
    T newValue,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);
    VerifyAccess();

    var property = GetDirectProperty<T>(propertyName);

    oldValue = newValue;
    RaisePropertyChanged(property, oldValue, newValue);

    return new PropertyChange(this, propertyName, true);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected PropertyChange SetAndNotifyIfChanged<T>(
    ref T oldValue,
    T newValue,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);
    VerifyAccess();

    if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
    {
      return new PropertyChange(this, propertyName, false);
    }

    var property = GetDirectProperty<T>(propertyName);

    oldValue = newValue;
    RaisePropertyChanged(property, oldValue, newValue);

    return new PropertyChange(this, propertyName, true);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private DirectPropertyBase<T> GetDirectProperty<T>(string propertyName)
  {
    var property = GetDirectProperty(propertyName) as DirectPropertyBase<T>;

    ArgumentNullException.ThrowIfNull(property);

    return property;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private object GetDirectProperty(string propertyName)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    var bindingFlags = BindingFlags.Public | BindingFlags.Static;

    var fieldFilter = (FieldInfo field) => string.Equals(
      field.Name, $"{propertyName}Property", StringComparison.Ordinal);

    var propertyInfo = GetType().GetFields(bindingFlags).FirstOrDefault(fieldFilter);

    ArgumentNullException.ThrowIfNull(propertyInfo);

    var property = propertyInfo.GetValue(null);

    ArgumentNullException.ThrowIfNull(property);

    ArgumentOutOfRangeException.ThrowIfNotEqual(
      typeof(DirectPropertyBase<>)
        .MakeGenericType(property.GetType().GenericTypeArguments.Last())
        .IsAssignableFrom(property.GetType()),
      true);

    return property;
  }
}
