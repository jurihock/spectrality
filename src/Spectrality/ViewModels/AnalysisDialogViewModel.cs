namespace Spectrality.ViewModels;

public sealed class AnalysisDialogViewModel : ViewModelBase
{
  private bool open = true;
  public bool IsOpen
  {
    get => Get(ref open);
    set => SetAndNotifyIfChanged(ref open, value);
  }

  public bool IsExpanderOpen1
  {
    get => Get(true);
    set => SetAndNotifyIfChanged(value);
  }

  public bool IsExpanderOpen2
  {
    get => Get(false);
    set => SetAndNotifyIfChanged(value);
  }

  public bool IsExpanderOpen3
  {
    get => Get(false);
    set => SetAndNotifyIfChanged(value);
  }

  public AnalysisDialogViewModel()
  {
    Reset();
  }

  public void Open()  => IsOpen = true;
  public void Close() => IsOpen = false;

  public void Reset()
  {
    // BandwidthMin = new NoteSelectorViewModel("A", 1);
    // BandwidthMax = new NoteSelectorViewModel("A", 9);
  }
}
