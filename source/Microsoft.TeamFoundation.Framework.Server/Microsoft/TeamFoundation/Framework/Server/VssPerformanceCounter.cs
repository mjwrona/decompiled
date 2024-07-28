// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssPerformanceCounter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct VssPerformanceCounter
  {
    private readonly CounterData _counterData;

    internal VssPerformanceCounter(CounterData counterData) => this._counterData = counterData;

    public long Decrement() => this.IncrementBy(-1L);

    public long Increment() => this.IncrementBy(1L);

    public long IncrementTicks(Stopwatch stopwatch) => this.IncrementBy(stopwatch.ElapsedTicks);

    public long IncrementTicks(TimeSpan timeSpan) => this.IncrementBy((long) (timeSpan.TotalSeconds * (double) Stopwatch.Frequency));

    public long IncrementMilliseconds(long milliseconds) => this.IncrementBy(milliseconds * Stopwatch.Frequency / 1000L);

    public long IncrementMicroseconds(long microseconds) => this.IncrementBy(microseconds * Stopwatch.Frequency / 1000000L);

    public long IncrementBy(long value)
    {
      if (this._counterData == null)
        return 0;
      if (this._counterData.Value + value < 0L)
        this._counterData.Value = 0L;
      else
        this._counterData.IncrementBy(value);
      return this._counterData.Value;
    }

    public long Value
    {
      get
      {
        CounterData counterData = this._counterData;
        return counterData == null ? 0L : counterData.Value;
      }
    }

    public void SetValue(long value)
    {
      if (this._counterData == null)
        return;
      if (value < 0L)
        this._counterData.Value = 0L;
      else
        this._counterData.Value = value;
    }
  }
}
