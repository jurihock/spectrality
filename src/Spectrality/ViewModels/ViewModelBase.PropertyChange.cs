using System;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace Spectrality.ViewModels;

public abstract partial class ViewModelBase
{
  protected readonly struct PropertyChange<T>(
    ViewModelBase propertyOwner,
    string propertyName,
    bool isPropertyChanged,
    T oldPropertyValue,
    T newPropertyValue)
  {
    public readonly ViewModelBase PropertyOwner = propertyOwner;
    public readonly string PropertyName = propertyName;
    public readonly bool IsPropertyChanged = isPropertyChanged;
    public readonly T OldPropertyValue = oldPropertyValue;
    public readonly T NewPropertyValue = newPropertyValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PropertyChange<T> AlsoInvoke(Action action)
    {
      if (IsPropertyChanged)
      {
        action();
      }

      return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PropertyChange<T> AlsoInvoke(Action<string> action)
    {
      if (IsPropertyChanged)
      {
        action(PropertyName);
      }

      return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PropertyChange<T> AlsoNotify(params string[] propertyNames)
    {
      ArgumentOutOfRangeException.ThrowIfZero(propertyNames.Length);

      if (IsPropertyChanged)
      {
        foreach (var propertyName in propertyNames)
        {
          PropertyOwner.RaisePropertyChanged(propertyName);
        }
      }

      return this;
    }
  }
}
