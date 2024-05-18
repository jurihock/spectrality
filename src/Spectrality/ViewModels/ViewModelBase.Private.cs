using Spectrality.Serialization;

namespace Spectrality.ViewModels;

public abstract partial class ViewModelBase
{
  private PropertyBag SerializableProperties { get; init; }
}
