using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Spectrality.ViewModels;
using Spectrality.Views;

namespace Spectrality;

public sealed partial class App : Application
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      desktop.MainWindow = new MainWindowView
      {
        DataContext = new MainWindowViewModel(),
      };
    }

    base.OnFrameworkInitializationCompleted();
  }
}
