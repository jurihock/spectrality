using Avalonia;
using Avalonia.ReactiveUI;
using Spectrality.Extensions;
using System;
using System.Globalization;

namespace Spectrality;

static class Program
{
  [STAThread]
  public static void Main(string[] args)
  {
    CultureInfo.DefaultThreadCurrentCulture =
    CultureInfo.DefaultThreadCurrentUICulture =
    CultureInfo.InvariantCulture;

    BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
  }

  public static AppBuilder BuildAvaloniaApp() => AppBuilder
    .Configure<App>()
    .LogToConsole()
    .UsePlatformDetect()
    .UseReactiveUI()
    .WithInterFont();
}
