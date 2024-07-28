// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Timers.TimerWheelCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Concurrent;
using System.Threading;


#nullable enable
namespace Microsoft.Azure.Cosmos.Timers
{
  internal sealed class TimerWheelCore : TimerWheel, IDisposable
  {
    private readonly ConcurrentQueue<TimerWheelTimer>[] timers;
    private readonly int resolutionInTicks;
    private readonly int resolutionInMs;
    private readonly int buckets;
    private readonly Timer timer;
    private readonly object timerConcurrencyLock;
    private bool isDisposed;
    private bool isRunning;
    private int expirationIndex;

    private TimerWheelCore(double resolution, int buckets)
    {
      if (resolution <= 20.0)
        throw new ArgumentOutOfRangeException(nameof (resolution), "Value is too low, machine resolution less than 20 ms has unexpected results https://docs.microsoft.com/dotnet/api/system.threading.timer");
      if (buckets <= 0)
        throw new ArgumentOutOfRangeException(nameof (buckets));
      this.resolutionInMs = (int) resolution;
      this.resolutionInTicks = (int) TimeSpan.FromMilliseconds((double) this.resolutionInMs).Ticks;
      this.buckets = buckets;
      this.timers = new ConcurrentQueue<TimerWheelTimer>[buckets];
      for (int index = 0; index < buckets; ++index)
        this.timers[index] = new ConcurrentQueue<TimerWheelTimer>();
      this.timerConcurrencyLock = new object();
    }

    internal TimerWheelCore(TimeSpan resolution, int buckets)
      : this(resolution.TotalMilliseconds, buckets)
    {
      this.timer = new Timer(new TimerCallback(this.OnTimer), (object) null, this.resolutionInMs, this.resolutionInMs);
    }

    internal TimerWheelCore(TimeSpan resolution, int buckets, Timer timer)
      : this(resolution.TotalMilliseconds, buckets)
    {
      this.timer = timer;
    }

    public override void Dispose()
    {
      if (this.isDisposed)
        return;
      this.DisposeAllTimers();
      this.isDisposed = true;
    }

    public override TimerWheelTimer CreateTimer(TimeSpan timeout)
    {
      this.ThrowIfDisposed();
      int totalMilliseconds = (int) timeout.TotalMilliseconds;
      if (totalMilliseconds < this.resolutionInMs)
        throw new ArgumentOutOfRangeException("timeoutInMs", string.Format("TimerWheel configured with {0} resolution, cannot use a smaller timeout of {1}.", (object) this.resolutionInMs, (object) totalMilliseconds));
      if (totalMilliseconds % this.resolutionInMs != 0)
        throw new ArgumentOutOfRangeException("timeoutInMs", string.Format("TimerWheel configured with {0} resolution, cannot use a different resolution of {1}.", (object) this.resolutionInMs, (object) totalMilliseconds));
      if (totalMilliseconds > this.resolutionInMs * this.buckets)
        throw new ArgumentOutOfRangeException("timeoutInMs", string.Format("TimerWheel configured with {0} max, cannot use a larger timeout of {1}.", (object) (this.resolutionInMs * this.buckets), (object) totalMilliseconds));
      return (TimerWheelTimer) new TimerWheelTimerCore(TimeSpan.FromMilliseconds((double) totalMilliseconds), (TimerWheel) this);
    }

    public override void SubscribeForTimeouts(TimerWheelTimer timer)
    {
      this.ThrowIfDisposed();
      this.timers[this.GetIndexForTimeout((int) timer.Timeout.Ticks / this.resolutionInTicks)].Enqueue(timer);
    }

    public void OnTimer(object stateInfo)
    {
      lock (this.timerConcurrencyLock)
      {
        if (this.isRunning)
          return;
        this.isRunning = true;
      }
      try
      {
        ConcurrentQueue<TimerWheelTimer> timer = this.timers[this.expirationIndex];
        TimerWheelTimer result;
        while (timer.TryDequeue(out result))
          result.FireTimeout();
        if (++this.expirationIndex != this.buckets)
          return;
        this.expirationIndex = 0;
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("TimerWheel: OnTimer error : " + ex.Message + "\n, stack: " + ex.StackTrace);
      }
      finally
      {
        lock (this.timerConcurrencyLock)
          this.isRunning = false;
      }
    }

    private int GetIndexForTimeout(int bucket)
    {
      int num = bucket + this.expirationIndex;
      if (num > this.buckets)
        num -= this.buckets;
      return num - 1;
    }

    private void DisposeAllTimers()
    {
      foreach (ConcurrentQueue<TimerWheelTimer> timer in this.timers)
      {
        TimerWheelTimer result;
        while (timer.TryDequeue(out result))
          result.CancelTimer();
      }
      this.timer?.Dispose();
    }

    private void ThrowIfDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException("TimerWheel is disposed.");
    }
  }
}
