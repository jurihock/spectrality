using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Spectrality.Serialization;

public partial class PropertyBag(IEqualityComparer<string>? comparer = null)
{
  protected ConcurrentDictionary<string, Property> Properties { get; private init; }
    = new(comparer ?? StringComparer.OrdinalIgnoreCase);

  public int Count => Properties.Count;
  public bool IsEmpty => Properties.IsEmpty;
  public ICollection<string> Keys => Properties.Keys;

  public bool Contains(string key) => Properties.ContainsKey(key);
  public bool Clear() => Properties.Keys.ToArray().All(Remove);

  public bool Remove(string key)
  {
    if (Properties.TryRemove(key, out Property _))
    {
      try
      {
        return true;
      }
      finally
      {
        OnPropertyRemoved(key);
      }
    }

    return false;
  }

  public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value)
  {
    if (Properties.TryGetValue(key, out Property property))
    {
      value = property.GetValue<T>();
      return true;
    }
    else
    {
      value = default;
      return false;
    }
  }

  public T Get<T>(string key, T fallback)
  {
    ArgumentNullException.ThrowIfNull(fallback, nameof(fallback));

    if (Properties.TryGetValue(key, out Property property))
    {
      return property.GetValue<T>();
    }

    return fallback;
  }

  public Property Set<T>(string key, T value)
  {
    ArgumentNullException.ThrowIfNull(value, nameof(value));

    var property = new Property(key, typeof(T), value);

    Action? notify = null;

    Properties.AddOrUpdate(key,
      (_) =>
      {
        notify = () => OnPropertyAdded(key);
        return property;
      },
      (_, _) =>
      {
        OnPropertyChanging(key);
        notify = () => OnPropertyChanged(key);
        return property;
      });

    if (notify is not null)
    {
      notify();
    }

    return property;
  }
}
