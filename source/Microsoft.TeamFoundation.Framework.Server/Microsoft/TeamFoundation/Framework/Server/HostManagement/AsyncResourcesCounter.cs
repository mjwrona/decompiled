// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostManagement.AsyncResourcesCounter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Performance;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.HostManagement
{
  internal class AsyncResourcesCounter : IDisposable
  {
    private int m_mainThread = -1;
    private AsyncLocal<int> m_threadsTracker;
    private Dictionary<int, AsyncResourcesCounter.ThreadCounters> m_threadsCounters = new Dictionary<int, AsyncResourcesCounter.ThreadCounters>();
    private readonly object m_counterLock = new object();
    private long m_cpuCycles;
    private long m_allocatedBytes;

    internal AsyncResourcesCounter() => this.AttachThreadCounter();

    internal (long, long) CaptureCollectedCounters()
    {
      this.CaptureThreadStats(true);
      this.m_threadsTracker = (AsyncLocal<int>) null;
      return (this.m_cpuCycles, this.m_allocatedBytes);
    }

    public void Dispose()
    {
      this.m_threadsTracker = (AsyncLocal<int>) null;
      this.m_isDisposed = true;
    }

    private void AttachThreadCounter()
    {
      this.CaptureThreadStats();
      this.m_threadsTracker = new AsyncLocal<int>(new Action<AsyncLocalValueChangedArgs<int>>(this.ValueChangedHandler));
      this.m_threadsTracker.Value = this.m_mainThread = PerformanceNativeMethods.GetCurrentThreadId();
    }

    private void ValueChangedHandler(AsyncLocalValueChangedArgs<int> changedArgs)
    {
      if (this.m_threadsTracker == null || this.m_isDisposed || !changedArgs.ThreadContextChanged)
        return;
      this.CaptureThreadStats(changedArgs.PreviousValue == this.m_mainThread && changedArgs.CurrentValue == 0);
    }

    private void CaptureThreadStats(bool isLeavingThread = false)
    {
      try
      {
        int currentThreadId = PerformanceNativeMethods.GetCurrentThreadId();
        long cpuTime = PerformanceNativeMethods.GetCPUTime();
        long forCurrentThread = GC.GetAllocatedBytesForCurrentThread();
        if (isLeavingThread)
        {
          lock (this.m_counterLock)
          {
            Dictionary<int, AsyncResourcesCounter.ThreadCounters> threadsCounters = this.m_threadsCounters;
            AsyncResourcesCounter.ThreadCounters threadCounters;
            // ISSUE: explicit non-virtual call
            if ((threadsCounters != null ? (__nonvirtual (threadsCounters.TryGetValue(currentThreadId, out threadCounters)) ? 1 : 0) : 0) == 0)
              return;
            this.m_cpuCycles += cpuTime - threadCounters.BeforeCpuTime;
            this.m_allocatedBytes += forCurrentThread - threadCounters.BeforeBytes;
            this.m_threadsCounters.Remove(currentThreadId);
          }
        }
        else
        {
          lock (this.m_counterLock)
            this.m_threadsCounters[currentThreadId] = new AsyncResourcesCounter.ThreadCounters()
            {
              BeforeBytes = forCurrentThread,
              BeforeCpuTime = cpuTime
            };
        }
      }
      catch (Exception ex)
      {
      }
    }

    private bool m_isDisposed { get; set; }

    private struct ThreadCounters
    {
      public long BeforeCpuTime { get; set; }

      public long BeforeBytes { get; set; }

      public ThreadCounters()
      {
        this.BeforeCpuTime = 0L;
        this.BeforeBytes = 0L;
      }
    }
  }
}
