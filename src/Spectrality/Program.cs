using Avalonia;
using Avalonia.ReactiveUI;
using Spectrality.Extensions;
using System;

namespace Spectrality;

static class Program
{
  // Initialization code.
  // Don't use any Avalonia, third-party APIs or any
  // SynchronizationContext-reliant code before
  // OnFrameworkInitializationCompleted is called.
  // Things aren't initialized yet and stuff might break.
  [STAThread]
  public static void Main(string[] args) => BuildAvaloniaApp()
    .StartWithClassicDesktopLifetime(args);

  // Avalonia configuration.
  // Don't remove, also used by visual designer.
  public static AppBuilder BuildAvaloniaApp() => AppBuilder
    .Configure<App>()
    .UsePlatformDetect()
    .UseReactiveUI()
    .WithInterFont()
    .LogToConsole();
}
