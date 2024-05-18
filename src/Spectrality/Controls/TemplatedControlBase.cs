using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Avalonia.Controls.Primitives;

namespace Spectrality.Controls;

public abstract partial class TemplatedControlBase : TemplatedControl
{
  protected TemplatedControlBase()
  {
    DirectProperties = GetDirectProperties();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected static ref T Get<T>(ref T value) => ref value;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected PropertyChange SetAndNotify<T>(
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
}
