using System;
using System.Collections.Generic;
using System.Linq;

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
    private static readonly Dictionary<Avalonia.Logging.LogEventLevel, NLog.LogLevel> LogLevelMap = new()
    {
      { Avalonia.Logging.LogEventLevel.Verbose, NLog.LogLevel.Trace },
      { Avalonia.Logging.LogEventLevel.Debug, NLog.LogLevel.Debug },
      { Avalonia.Logging.LogEventLevel.Information, NLog.LogLevel.Info },
      { Avalonia.Logging.LogEventLevel.Warning, NLog.LogLevel.Warn },
      { Avalonia.Logging.LogEventLevel.Error, NLog.LogLevel.Error },
      { Avalonia.Logging.LogEventLevel.Fatal, NLog.LogLevel.Fatal }
    };

    private static bool OxyPlotTrackerWarning = false;

    private static bool IsOxyPlotTrackerWarning(Avalonia.Logging.LogEventLevel level, string source)
    {
      // Notify once and then suppress subsequent warnings:
      //   An error occurred binding A to "B" at "C":
      //   "Could not find a matching property accessor for 'C' on 'D'."

      if (level != Avalonia.Logging.LogEventLevel.Warning)
      {
        return false;
      }

      var sources = new[]
      {
        typeof(OxyPlot.Avalonia.TrackerControl).FullName,
        typeof(Avalonia.Controls.TextBlock).FullName
      };

      if (!sources.Contains(source))
      {
        return false;
      }

      try
      {
        return OxyPlotTrackerWarning;
      }
      finally
      {
        OxyPlotTrackerWarning = true;
      }
    }

    private static NLog.Logger? GetLogger(Avalonia.Logging.LogEventLevel level, string area, object? source)
    {
      var name = source?.ToString() ?? $"{nameof(Avalonia)}.{area}";

      if (IsOxyPlotTrackerWarning(level, name))
      {
        return null;
      }

      return NLog.LogManager.GetLogger(name);
    }

    public bool IsEnabled(Avalonia.Logging.LogEventLevel level, string area) =>
      level >= Avalonia.Logging.LogEventLevel.Warning;

    public void Log(Avalonia.Logging.LogEventLevel level, string area, object? source, string message) =>
      GetLogger(level, area, source)?.Log(LogLevelMap[level], message);

    public void Log(Avalonia.Logging.LogEventLevel level, string area, object? source, string message, params object?[] args) =>
      GetLogger(level, area, source)?.Log(LogLevelMap[level], message, args);
  }
}
