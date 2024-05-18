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
  protected T? Get<T>(
    T? fallback = default,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    return SerializableProperties.Get(propertyName, fallback);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected PropertyChange SetAndNotify<T>(
    ref T oldValue,
    T newValue,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
    {
      return new PropertyChange(this, propertyName, false);
    }

    this.RaisePropertyChanging(propertyName);
    oldValue = newValue;

    this.RaisePropertyChanged(propertyName);
    return new PropertyChange(this, propertyName, true);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected PropertyChange SetAndNotify<T>(
    T newValue,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    var oldValue = SerializableProperties.Get<T>(propertyName);

    if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
    {
      return new PropertyChange(this, propertyName, false);
    }

    this.RaisePropertyChanging(propertyName);
    SerializableProperties.Set(propertyName, newValue);

    this.RaisePropertyChanged(propertyName);
    return new PropertyChange(this, propertyName, true);
  }
}
