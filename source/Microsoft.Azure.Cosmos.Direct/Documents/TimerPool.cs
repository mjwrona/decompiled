// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.TimerPool
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Azure.Documents
{
  internal sealed class TimerPool : IDisposable
  {
    [ThreadStatic]
    private static Random PooledTimerBucketSelector;
    private readonly Timer timer;
    private readonly ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>>[] pooledTimersByTimeout;
    private readonly TimeSpan minSupportedTimeout;
    private readonly object timerConcurrencyLock;
    private bool isRunning;
    private bool isDisposed;

    public TimeSpan MinSupportedTimeout => this.minSupportedTimeout;

    public TimerPool(int minSupportedTimerDelayInSeconds, int maxBucketsForPools = -1)
    {
      this.timerConcurrencyLock = new object();
      this.minSupportedTimeout = TimeSpan.FromSeconds(minSupportedTimerDelayInSeconds > 0 ? (double) minSupportedTimerDelayInSeconds : 1.0);
      maxBucketsForPools = maxBucketsForPools > 0 ? maxBucketsForPools : Environment.ProcessorCount;
      this.pooledTimersByTimeout = new ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>>[maxBucketsForPools];
      for (int index = 0; index < maxBucketsForPools; ++index)
        this.pooledTimersByTimeout[index] = new ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>>();
      this.timer = new Timer(new TimerCallback(this.OnTimer), (object) null, TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds((double) minSupportedTimerDelayInSeconds));
      DefaultTrace.TraceInformation("TimerPool Created with minSupportedTimerDelayInSeconds = {0}", (object) minSupportedTimerDelayInSeconds);
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      this.DisposeAllPooledTimers();
      this.isDisposed = true;
    }

    private void ThrowIfDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (TimerPool));
    }

    private void DisposeAllPooledTimers()
    {
      DefaultTrace.TraceInformation("TimerPool Disposing");
      foreach (ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>> concurrentDictionary in this.pooledTimersByTimeout)
      {
        foreach (KeyValuePair<int, ConcurrentQueue<PooledTimer>> keyValuePair in concurrentDictionary)
        {
          ConcurrentQueue<PooledTimer> concurrentQueue = keyValuePair.Value;
          PooledTimer result;
          while (concurrentQueue.TryDequeue(out result))
            result.CancelTimer();
        }
      }
      this.timer.Dispose();
      DefaultTrace.TraceInformation("TimePool Disposed");
    }

    private void OnTimer(object stateInfo)
    {
      lock (this.timerConcurrencyLock)
      {
        if (this.isRunning)
          return;
        this.isRunning = true;
      }
      try
      {
        long ticks = DateTime.UtcNow.Ticks;
        foreach (ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>> concurrentDictionary in this.pooledTimersByTimeout)
        {
          foreach (KeyValuePair<int, ConcurrentQueue<PooledTimer>> keyValuePair in concurrentDictionary)
          {
            ConcurrentQueue<PooledTimer> concurrentQueue = keyValuePair.Value;
            int count = keyValuePair.Value.Count;
            long num = 0;
            for (int index = 0; index < count; ++index)
            {
              PooledTimer result1;
              if (concurrentQueue.TryPeek(out result1))
              {
                if (ticks >= result1.TimeoutTicks)
                {
                  if (result1.TimeoutTicks < num)
                    DefaultTrace.TraceCritical("LastTicks: {0}, PooledTimer.Ticks: {1}", (object) num, (object) result1.TimeoutTicks);
                  result1.FireTimeout();
                  num = result1.TimeoutTicks;
                  PooledTimer result2;
                  if (concurrentQueue.TryDequeue(out result2) && result2 != result1)
                  {
                    DefaultTrace.TraceCritical("Timer objects peeked and dequeued are not equal");
                    concurrentQueue.Enqueue(result2);
                  }
                }
                else
                  break;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceCritical("Hit exception ex: {0}\n, stack: {1}", (object) ex.Message, (object) ex.StackTrace);
      }
      finally
      {
        lock (this.timerConcurrencyLock)
          this.isRunning = false;
      }
    }

    internal ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>>[] PooledTimersByTimeout => this.pooledTimersByTimeout;

    public PooledTimer GetPooledTimer(int timeoutInSeconds)
    {
      this.ThrowIfDisposed();
      return new PooledTimer(timeoutInSeconds, this);
    }

    public PooledTimer GetPooledTimer(TimeSpan timeout)
    {
      this.ThrowIfDisposed();
      return new PooledTimer(timeout, this);
    }

    public long SubscribeForTimeouts(PooledTimer pooledTimer)
    {
      this.ThrowIfDisposed();
      TimeSpan timeSpan;
      if (pooledTimer.Timeout < this.minSupportedTimeout)
      {
        object[] objArray = new object[2];
        timeSpan = pooledTimer.Timeout;
        objArray[0] = (object) timeSpan.TotalSeconds;
        timeSpan = this.minSupportedTimeout;
        objArray[1] = (object) timeSpan.TotalSeconds;
        DefaultTrace.TraceWarning("Timer timeoutinSeconds {0} is less than minSupportedTimeoutInSeconds {1}, will use the minsupported value", objArray);
        pooledTimer.Timeout = this.minSupportedTimeout;
      }
      if (TimerPool.PooledTimerBucketSelector == null)
        TimerPool.PooledTimerBucketSelector = new Random();
      int index = TimerPool.PooledTimerBucketSelector.Next(this.pooledTimersByTimeout.Length);
      ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>> concurrentDictionary1 = this.pooledTimersByTimeout[index];
      timeSpan = pooledTimer.Timeout;
      int totalSeconds1 = (int) timeSpan.TotalSeconds;
      ConcurrentQueue<PooledTimer> orAdd;
      ref ConcurrentQueue<PooledTimer> local = ref orAdd;
      if (!concurrentDictionary1.TryGetValue(totalSeconds1, out local))
      {
        ConcurrentDictionary<int, ConcurrentQueue<PooledTimer>> concurrentDictionary2 = this.pooledTimersByTimeout[index];
        timeSpan = pooledTimer.Timeout;
        int totalSeconds2 = (int) timeSpan.TotalSeconds;
        orAdd = concurrentDictionary2.GetOrAdd(totalSeconds2, (Func<int, ConcurrentQueue<PooledTimer>>) (_ => new ConcurrentQueue<PooledTimer>()));
      }
      lock (orAdd)
      {
        orAdd.Enqueue(pooledTimer);
        return DateTime.UtcNow.Ticks;
      }
    }
  }
}
