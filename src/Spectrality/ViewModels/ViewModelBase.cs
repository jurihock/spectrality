using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace Spectrality.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
  protected readonly struct PropertyChange(
    ViewModelBase propertyOwner,
    string propertyName,
    bool isPropertyChanged)
  {
    public readonly ViewModelBase PropertyOwner = propertyOwner;
    public readonly string PropertyName = propertyName;
    public readonly bool IsPropertyChanged = isPropertyChanged;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PropertyChange AlsoInvoke(Action action)
    {
      if (IsPropertyChanged)
      {
        action();
      }

      return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PropertyChange AlsoInvoke(Action<string> action)
    {
      if (IsPropertyChanged)
      {
        action(PropertyName);
      }

      return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PropertyChange AlsoNotify(params string[] propertyNames)
    {
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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected PropertyChange SetAndNotify<T>(
    ref T oldValue,
    T newValue,
    [CallerMemberName] string? propertyName = null)
  {
    ArgumentNullException.ThrowIfNull(propertyName);

    this.RaisePropertyChanging(propertyName);
    oldValue = newValue;

    this.RaisePropertyChanged(propertyName);
    return new PropertyChange(this, propertyName, true);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected PropertyChange SetAndNotifyIfChanged<T>(
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
}
