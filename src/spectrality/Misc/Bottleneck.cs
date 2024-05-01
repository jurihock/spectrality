using System;
using System.Diagnostics;
using System.Threading;

namespace Spectrality.Misc;

public sealed class Bottleneck
{
  private readonly object SyncRoot = new();
  private long TryPassTimestamp = Stopwatch.GetTimestamp();

  public void Pass(Action action)
  {
    Monitor.Enter(SyncRoot);

    try
    {
      action();
    }
    finally
    {
      Monitor.Exit(SyncRoot);
    }
  }

  public bool TryPass(Action action, TimeSpan? debounce = null)
  {
    if (!Monitor.TryEnter(SyncRoot))
    {
      return false;
    }

    if (debounce.HasValue)
    {
      if (Stopwatch.GetElapsedTime(TryPassTimestamp) < debounce)
      {
        Monitor.Exit(SyncRoot);
        return false;
      }
    }

    try
    {
      action();
    }
    finally
    {
      TryPassTimestamp = Stopwatch.GetTimestamp();
      Monitor.Exit(SyncRoot);
    }

    return true;
  }
}
