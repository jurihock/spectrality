using System;
using System.Runtime.CompilerServices;

namespace Spectrality.Controls;

public abstract partial class TemplatedControlBase
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
          PropertyOwner.DirectProperties[propertyName].Raise();
        }
      }

      return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PropertyChange AlsoNotifyOthers()
    {
      if (IsPropertyChanged)
      {
        foreach (var propertyName in PropertyOwner.DirectProperties.Keys)
        {
          if (propertyName == PropertyName)
          {
            continue;
          }

          PropertyOwner.DirectProperties[propertyName].Raise();
        }
      }

      return this;
    }
  }
}
