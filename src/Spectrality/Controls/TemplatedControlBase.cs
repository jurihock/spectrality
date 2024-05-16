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
  private IReadOnlyDictionary<string, (object Property, Action Raise)> DirectProperties { get; init; }

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
        PropertyOwner.DirectProperties[propertyName].Raise();
      }
    }
  }

  protected TemplatedControlBase()
  {
    MethodInfo GetRaisePropertyChangedMethod()
    {
      var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

      var tempType = Type.MakeGenericMethodParameter(0);
      var baseType = typeof(DirectPropertyBase<>).MakeGenericType(tempType);

      var method = typeof(AvaloniaObject).GetMethod(
        nameof(RaisePropertyChanged), 1, bindingFlags,
        null, [baseType, tempType, tempType], null);

      ArgumentNullException.ThrowIfNull(method);

      return method;
    }

    IReadOnlyDictionary<string, (object Property, Action Raise)> GetDirectProperties()
    {
      var properties = new Dictionary<string, (object value, Action raise)>();

      var bindingFlags = BindingFlags.Public | BindingFlags.Static;
      var bindingFilter = (FieldInfo field) => field.Name.EndsWith("Property");

      var propertyInfos = GetType().GetFields(bindingFlags).Where(bindingFilter);
      var propertyRaiser = GetRaisePropertyChangedMethod();

      foreach (var propertyInfo in propertyInfos)
      {
        var property = propertyInfo.GetValue(null);

        ArgumentNullException.ThrowIfNull(property);

        ArgumentOutOfRangeException.ThrowIfNotEqual(
          typeof(DirectPropertyBase<>)
            .MakeGenericType(property.GetType().GenericTypeArguments.Last())
            .IsAssignableFrom(property.GetType()),
          true);

        var name = propertyInfo.Name[.. ^ "Property".Length];

        var raiser = propertyRaiser.MakeGenericMethod(
          property.GetType().GenericTypeArguments.Last());

        var raise = () => { raiser.Invoke(this, [property, default, default]); };

        properties.Add(name, (property, raise));
      }

      return properties;
    }

    DirectProperties = GetDirectProperties();
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
    var property = DirectProperties[propertyName].Property as DirectPropertyBase<T>;

    ArgumentNullException.ThrowIfNull(property);

    return property;
  }
}
