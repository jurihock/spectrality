using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Spectrality.ViewModels;
using Spectrality.Views;

namespace Spectrality;

public partial class App : Application
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    EnableConsoleLogging();

    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      desktop.MainWindow = new MainWindow
      {
        DataContext = new MainWindowViewModel(),
      };
    }

    base.OnFrameworkInitializationCompleted();
  }

  private static void EnableConsoleLogging()
  {
    var config = new NLog.Config.LoggingConfiguration();
    var target = new NLog.Targets.ConsoleTarget();

    config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, target);

    NLog.LogManager.Configuration = config;
  }
}
