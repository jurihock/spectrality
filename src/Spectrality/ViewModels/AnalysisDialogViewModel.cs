namespace Spectrality.ViewModels;

public sealed class AnalysisDialogViewModel : ViewModelBase
{
  private bool isOpen = true;
  public bool IsOpen
  {
    get => isOpen;
    set => SetAndNotify(ref isOpen, value);
  }

  // private NoteSelectorViewModel bandwidthMin = new();
  // public NoteSelectorViewModel BandwidthMin
  // {
  //   get => bandwidthMin;
  //   set => SetAndNotify(ref bandwidthMin, value);
  // }

  // private NoteSelectorViewModel bandwidthMax = new();
  // public NoteSelectorViewModel BandwidthMax
  // {
  //   get => bandwidthMax;
  //   set => SetAndNotify(ref bandwidthMax, value);
  // }

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
