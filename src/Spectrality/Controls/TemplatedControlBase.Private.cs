using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;

namespace Spectrality.Controls;

public abstract partial class TemplatedControlBase
{
  private Dictionary<string, (object Property, Action Raise)> DirectProperties { get; init; }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private DirectPropertyBase<T> GetDirectProperty<T>(string propertyName)
  {
    var property = DirectProperties[propertyName].Property as DirectPropertyBase<T>;

    ArgumentNullException.ThrowIfNull(property);

    return property;
  }

  private MethodInfo GetRaisePropertyChangedMethod()
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

  private Dictionary<string, (object Property, Action Raise)> GetDirectProperties()
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

      var propertyType = property.GetType();
      var valueType = propertyType.GenericTypeArguments.Last();

      ArgumentOutOfRangeException.ThrowIfNotEqual(
        typeof(DirectPropertyBase<>)
          .MakeGenericType(valueType)
          .IsAssignableFrom(propertyType),
        true);

      var value = valueType.IsValueType
        ? Activator.CreateInstance(valueType)
        : null;

      var name = propertyInfo.Name[.. ^ "Property".Length];

      var raiser = propertyRaiser.MakeGenericMethod(
        valueType);

      var raise = () => { raiser.Invoke(this, [property, value, value]); };

      properties.Add(name, (property, raise));
    }

    return properties;
  }
}
