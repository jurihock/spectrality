using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace Spectrality.ViewModels;

public abstract partial class ViewModelBase : ReactiveObject
{
  protected ViewModelBase()
  {
    SerializableProperties = new();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected static ref T Get<T>(ref T value) => ref value;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T Get<T>(
    T fallback,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    return SerializableProperties.Get(propertyName, fallback);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected PropertyChange<T> SetAndNotify<T>(
    ref T oldValue,
    T newValue,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
    {
      return new PropertyChange<T>(this, propertyName, false, oldValue, newValue);
    }

    this.RaisePropertyChanging(propertyName);
    oldValue = newValue;

    this.RaisePropertyChanged(propertyName);
    return new PropertyChange<T>(this, propertyName, true, oldValue, newValue);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected PropertyChange<T> SetAndNotify<T>(
    T newValue,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    if (!SerializableProperties.TryGet<T>(propertyName, out var oldValue))
    {
      oldValue = newValue;
    }
    else if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
    {
      return new PropertyChange<T>(this, propertyName, false, oldValue, newValue);
    }

    this.RaisePropertyChanging(propertyName);
    SerializableProperties.Set(propertyName, newValue);

    this.RaisePropertyChanged(propertyName);
    return new PropertyChange<T>(this, propertyName, true, oldValue, newValue);
  }
}
