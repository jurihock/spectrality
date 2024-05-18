using System;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace Spectrality.ViewModels;

public abstract partial class ViewModelBase
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
