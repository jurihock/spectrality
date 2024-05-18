using System.ComponentModel;

namespace Spectrality.Serialization;

public partial class PropertyBag : INotifyPropertyChanging, INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyAdded;
  public event PropertyChangingEventHandler? PropertyChanging;
  public event PropertyChangedEventHandler? PropertyChanged;
  public event PropertyChangedEventHandler? PropertyRemoved;

  protected virtual void OnPropertyAdded(string propertyName) =>
    PropertyAdded?.Invoke(this, new PropertyChangedEventArgs(propertyName));

  protected virtual void OnPropertyChanging(string propertyName) =>
    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

  protected virtual void OnPropertyChanged(string propertyName) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

  protected virtual void OnPropertyRemoved(string propertyName) =>
    PropertyRemoved?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
