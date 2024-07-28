// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.CpuMonitor
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class CpuMonitor : IDisposable
  {
    internal const int DefaultRefreshIntervalInSeconds = 10;
    private const int HistoryLength = 6;
    private static TimeSpan refreshInterval = TimeSpan.FromSeconds(10.0);
    private bool disposed;
    private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    private CancellationTokenSource cancellation;
    private CpuLoadHistory currentReading;
    private Task periodicTask;

    internal static void OverrideRefreshInterval(TimeSpan newRefreshInterval) => CpuMonitor.refreshInterval = newRefreshInterval;

    public void Start()
    {
      this.ThrowIfDisposed();
      this.rwLock.EnterWriteLock();
      try
      {
        if (this.periodicTask != null)
          throw new InvalidOperationException("CpuMonitor already started");
        this.cancellation = new CancellationTokenSource();
        CancellationToken cancellationToken = this.cancellation.Token;
        this.periodicTask = Task.Factory.StartNew<Task>((Func<Task>) (() => this.RefreshLoopAsync(cancellationToken)), cancellationToken, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
        this.periodicTask.ContinueWith((Action<Task>) (t => DefaultTrace.TraceError("The CPU monitor refresh task failed. Exception: {0}", (object) t.Exception)), TaskContinuationOptions.OnlyOnFaulted);
        this.periodicTask.ContinueWith((Action<Task>) (t => DefaultTrace.TraceInformation("The CPU monitor refresh task stopped. Status: {0}", (object) t.Status)), TaskContinuationOptions.NotOnFaulted);
      }
      finally
      {
        this.rwLock.ExitWriteLock();
      }
      DefaultTrace.TraceInformation("CpuMonitor started");
    }

    public void Stop()
    {
      this.ThrowIfDisposed();
      CancellationTokenSource cancellationTokenSource = (CancellationTokenSource) null;
      Task task = (Task) null;
      this.rwLock.EnterWriteLock();
      try
      {
        if (this.periodicTask == null)
          throw new InvalidOperationException("CpuMonitor not started");
        cancellationTokenSource = this.cancellation;
        task = this.periodicTask;
        this.cancellation = (CancellationTokenSource) null;
        this.currentReading = (CpuLoadHistory) null;
        this.periodicTask = (Task) null;
      }
      finally
      {
        this.rwLock.ExitWriteLock();
      }
      cancellationTokenSource.Cancel();
      try
      {
        task.Wait();
      }
      catch (AggregateException ex)
      {
      }
      cancellationTokenSource.Dispose();
      DefaultTrace.TraceInformation("CpuMonitor stopped");
    }

    public CpuLoadHistory GetCpuLoad()
    {
      this.ThrowIfDisposed();
      this.rwLock.EnterReadLock();
      try
      {
        if (this.periodicTask == null)
          throw new InvalidOperationException("CpuMonitor was not started");
        return this.currentReading;
      }
      finally
      {
        this.rwLock.ExitReadLock();
      }
    }

    public void Dispose()
    {
      this.ThrowIfDisposed();
      this.rwLock.EnterReadLock();
      try
      {
        if (this.periodicTask != null)
          throw new InvalidOperationException("CpuMonitor must be stopped before Dispose");
      }
      finally
      {
        this.rwLock.ExitReadLock();
      }
      this.rwLock.Dispose();
      this.MarkDisposed();
    }

    private void MarkDisposed()
    {
      this.disposed = true;
      Interlocked.MemoryBarrier();
    }

    private void ThrowIfDisposed()
    {
      Interlocked.MemoryBarrier();
      if (this.disposed)
        throw new ObjectDisposedException(nameof (CpuMonitor));
    }

    private async Task RefreshLoopAsync(CancellationToken cancellationToken)
    {
      System.Diagnostics.Trace.CorrelationManager.ActivityId = Guid.NewGuid();
      SystemUtilizationReaderBase cpuReader = SystemUtilizationReaderBase.SingletonInstance;
      CpuLoad[] buffer = new CpuLoad[6];
      int clockHand = 0;
      TaskCompletionSource<object> cancellationCompletion = new TaskCompletionSource<object>();
      cancellationToken.Register((Action) (() => cancellationCompletion.SetCanceled()));
      Task[] refreshTasks = new Task[2]
      {
        null,
        (Task) cancellationCompletion.Task
      };
      DateTime nextExpiration = DateTime.UtcNow;
      while (!cancellationToken.IsCancellationRequested)
      {
        DateTime utcNow1 = DateTime.UtcNow;
        float systemWideCpuUsage = cpuReader.GetSystemWideCpuUsage();
        if (!float.IsNaN(systemWideCpuUsage))
        {
          List<CpuLoad> list = new List<CpuLoad>(buffer.Length);
          CpuLoadHistory cpuLoadHistory = new CpuLoadHistory(new ReadOnlyCollection<CpuLoad>((IList<CpuLoad>) list), CpuMonitor.refreshInterval);
          buffer[clockHand] = new CpuLoad(utcNow1, systemWideCpuUsage);
          clockHand = (clockHand + 1) % buffer.Length;
          for (int index1 = 0; index1 < buffer.Length; ++index1)
          {
            int index2 = (clockHand + index1) % buffer.Length;
            if (buffer[index2].Timestamp != DateTime.MinValue)
              list.Add(buffer[index2]);
          }
          this.rwLock.EnterWriteLock();
          try
          {
            if (cancellationToken.IsCancellationRequested)
            {
              cpuReader = (SystemUtilizationReaderBase) null;
              buffer = (CpuLoad[]) null;
              refreshTasks = (Task[]) null;
              return;
            }
            this.currentReading = cpuLoadHistory;
          }
          finally
          {
            this.rwLock.ExitWriteLock();
          }
        }
        DateTime utcNow2 = DateTime.UtcNow;
        while (nextExpiration <= utcNow2)
          nextExpiration += CpuMonitor.refreshInterval;
        TimeSpan delay = nextExpiration - DateTime.UtcNow;
        if (delay > TimeSpan.Zero)
        {
          refreshTasks[0] = Task.Delay(delay);
          if (await Task.WhenAny(refreshTasks) == refreshTasks[1])
          {
            cpuReader = (SystemUtilizationReaderBase) null;
            buffer = (CpuLoad[]) null;
            refreshTasks = (Task[]) null;
            return;
          }
        }
      }
      cpuReader = (SystemUtilizationReaderBase) null;
      buffer = (CpuLoad[]) null;
      refreshTasks = (Task[]) null;
    }
  }
}
