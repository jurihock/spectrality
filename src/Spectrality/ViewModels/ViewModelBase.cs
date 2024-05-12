using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace Spectrality.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool SetAndNotify<T>(
    ref T oldValue,
    T newValue,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    this.RaisePropertyChanging(propertyName);
    oldValue = newValue;

    this.RaisePropertyChanged(propertyName);
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool SetAndNotifyIfChanged<T>(
    ref T oldValue,
    T newValue,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
    {
      return false;
    }

    this.RaisePropertyChanging(propertyName);
    oldValue = newValue;

    this.RaisePropertyChanged(propertyName);
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void Notify(
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    this.RaisePropertyChanged(propertyName);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void Notify(
    params string[] propertyNames)
  {
    ArgumentNullException.ThrowIfNull(propertyNames);

    foreach (var propertyName in propertyNames)
    {
      this.RaisePropertyChanged(propertyName);
    }
  }
}
