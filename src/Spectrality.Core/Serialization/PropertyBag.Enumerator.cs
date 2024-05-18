using System.Collections;
using System.Collections.Generic;

namespace Spectrality.Serialization;

public partial class PropertyBag : IEnumerable<PropertyBag.Property>
{
  public sealed class Enumerator(PropertyBag bag) : IEnumerator<Property>
  {
    private readonly IEnumerator<KeyValuePair<string, Property>> PropertiesEnumerator
      = bag.Properties.GetEnumerator();

    public Property Current => PropertiesEnumerator.Current.Value;
    object IEnumerator.Current => PropertiesEnumerator.Current.Value;

    public void Dispose() => PropertiesEnumerator.Dispose();
    public bool MoveNext() => PropertiesEnumerator.MoveNext();
    public void Reset() => PropertiesEnumerator.Reset();
  }

  public IEnumerator<Property> GetEnumerator() => new Enumerator(this);
  IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
}
