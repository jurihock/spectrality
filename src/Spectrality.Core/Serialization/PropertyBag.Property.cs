using System;

namespace Spectrality.Serialization;

public partial class PropertyBag
{
  public readonly struct Property(string key, Type type, object? value)
  {
    public string Key { get; init; } = key;
    public Type Type { get; init; } = type;
    public object? Value { get; init; } = value;

    public T? GetValue<T>()
    {
      if (Type == typeof(T))
      {
        return (T?)Value;
      }

      throw new ArgumentOutOfRangeException(nameof(T));
    }

    public override string ToString() => $"{Key}: {Value}";
  }
}
