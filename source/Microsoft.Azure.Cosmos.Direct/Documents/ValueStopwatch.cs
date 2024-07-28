// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ValueStopwatch
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Diagnostics;

namespace Microsoft.Azure.Documents
{
  internal struct ValueStopwatch
  {
    private static readonly double ToTimeSpanTicks = 10000000.0 / (double) Stopwatch.Frequency;
    private static readonly double ToMilliseconds = 1000.0 / (double) Stopwatch.Frequency;
    public static readonly long Frequency = Stopwatch.Frequency;
    public static readonly bool IsHighResolution = Stopwatch.IsHighResolution;
    private long state;

    public readonly bool IsRunning => this.state > 0L;

    public readonly long ElapsedTicks
    {
      get
      {
        long state = this.state;
        if (state == 0L)
          return 0;
        return state < 0L ? Math.Abs(state) : Stopwatch.GetTimestamp() - state;
      }
    }

    public readonly long ElapsedMilliseconds => (long) ((double) this.ElapsedTicks * ValueStopwatch.ToMilliseconds);

    public readonly TimeSpan Elapsed => new TimeSpan((long) ((double) this.ElapsedTicks * ValueStopwatch.ToTimeSpanTicks));

    public void Reset() => this.state = 0L;

    public void Restart()
    {
      this.Reset();
      this.Start();
    }

    public void Start()
    {
      long state = this.state;
      if (state > 0L)
        return;
      this.state = Stopwatch.GetTimestamp() + state;
    }

    public void Stop()
    {
      long state = this.state;
      if (state <= 0L)
        return;
      this.state = -Math.Max(Stopwatch.GetTimestamp() - state, 0L);
    }

    public static long GetTimestamp() => Stopwatch.GetTimestamp();

    public static ValueStopwatch StartNew()
    {
      ValueStopwatch valueStopwatch = new ValueStopwatch();
      valueStopwatch.Start();
      return valueStopwatch;
    }
  }
}
