using System.Collections.Generic;

namespace Spectrality.Extensions;

public static class LoggingExtensions
{
  public static Avalonia.AppBuilder LogToConsole(this Avalonia.AppBuilder builder)
  {
    var config = new NLog.Config.LoggingConfiguration();
    var target = new NLog.Targets.ColoredConsoleTarget();
    var sink = new AvaloniaNLogSink();

    target.Layout = string.Join(
      " | ",
      "${level:uppercase=true:padding=-5}",
      "${time}",
      "${logger}",
      "${message}" +
      "${onexception:${newline}\t${exception:format=tostring}}");

    target.WordHighlightingRules.Add(
      new NLog.Targets.ConsoleWordHighlightingRule()
      {
        Text = "INFO",
        IgnoreCase = false,
        WholeWords = true,
        ForegroundColor = NLog.Targets.ConsoleOutputColor.Green
      });

    config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, target);

    NLog.LogManager.Configuration = config;
    Avalonia.Logging.Logger.Sink = sink;

    return builder;
  }

  private sealed class AvaloniaNLogSink : Avalonia.Logging.ILogSink
  {
    private static readonly IReadOnlyDictionary<Avalonia.Logging.LogEventLevel, NLog.LogLevel> LogLevelMap =
      new Dictionary<Avalonia.Logging.LogEventLevel, NLog.LogLevel>
      {
        { Avalonia.Logging.LogEventLevel.Verbose, NLog.LogLevel.Trace },
        { Avalonia.Logging.LogEventLevel.Debug, NLog.LogLevel.Debug },
        { Avalonia.Logging.LogEventLevel.Information, NLog.LogLevel.Info },
        { Avalonia.Logging.LogEventLevel.Warning, NLog.LogLevel.Warn },
        { Avalonia.Logging.LogEventLevel.Error, NLog.LogLevel.Error },
        { Avalonia.Logging.LogEventLevel.Fatal, NLog.LogLevel.Fatal }
      };

    private static NLog.Logger GetLogger(string area, object? source) =>
      NLog.LogManager.GetLogger(source?.ToString() ?? $"{nameof(Avalonia)}.{area}");

    public bool IsEnabled(Avalonia.Logging.LogEventLevel level, string area) =>
      level >= Avalonia.Logging.LogEventLevel.Warning;

    public void Log(Avalonia.Logging.LogEventLevel level, string area, object? source, string message) =>
      GetLogger(area, source).Log(LogLevelMap[level], message);

    public void Log(Avalonia.Logging.LogEventLevel level, string area, object? source, string message, params object?[] args) =>
      GetLogger(area, source).Log(LogLevelMap[level], message, args);
  }
}