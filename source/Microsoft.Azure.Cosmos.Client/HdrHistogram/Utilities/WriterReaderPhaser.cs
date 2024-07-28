// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Utilities.WriterReaderPhaser
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace HdrHistogram.Utilities
{
  internal class WriterReaderPhaser
  {
    private readonly object _readerLock = new object();
    private long _startEpoch;
    private long _evenEndEpoch;
    private long _oddEndEpoch = long.MinValue;

    private static long GetAndIncrement(ref long value) => Interlocked.Increment(ref value) - 1L;

    private static long GetAndSet(ref long value, long newValue) => Interlocked.Exchange(ref value, newValue);

    private static void LazySet(ref long value, long newValue) => Interlocked.Exchange(ref value, newValue);

    public long WriterCriticalSectionEnter() => WriterReaderPhaser.GetAndIncrement(ref this._startEpoch);

    public void WriterCriticalSectionExit(long criticalValueAtEnter)
    {
      if (criticalValueAtEnter < 0L)
        WriterReaderPhaser.GetAndIncrement(ref this._oddEndEpoch);
      else
        WriterReaderPhaser.GetAndIncrement(ref this._evenEndEpoch);
    }

    public void ReaderLock() => Monitor.Enter(this._readerLock);

    public void ReaderUnlock() => Monitor.Exit(this._readerLock);

    public void FlipPhase(TimeSpan yieldPeriod)
    {
      if (!Monitor.IsEntered(this._readerLock))
        throw new InvalidOperationException("FlipPhase can only be called while holding the reader lock");
      bool flag1 = this._startEpoch < 0L;
      long newValue;
      if (flag1)
      {
        newValue = 0L;
        WriterReaderPhaser.LazySet(ref this._evenEndEpoch, newValue);
      }
      else
      {
        newValue = long.MinValue;
        WriterReaderPhaser.LazySet(ref this._oddEndEpoch, newValue);
      }
      long andSet = WriterReaderPhaser.GetAndSet(ref this._startEpoch, newValue);
      bool flag2;
      do
      {
        flag2 = !flag1 ? this._evenEndEpoch == andSet : this._oddEndEpoch == andSet;
        if (!flag2)
        {
          if (yieldPeriod == TimeSpan.Zero)
            Task.Yield().GetAwaiter().GetResult();
          else
            Task.Delay(yieldPeriod).GetAwaiter().GetResult();
        }
      }
      while (!flag2);
    }

    public void FlipPhase() => this.FlipPhase(TimeSpan.Zero);
  }
}
