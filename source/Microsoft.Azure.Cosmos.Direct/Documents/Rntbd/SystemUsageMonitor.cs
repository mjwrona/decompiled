// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.SystemUsageMonitor
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class SystemUsageMonitor : IDisposable
  {
    private readonly SystemUtilizationReaderBase systemUtilizationReader = SystemUtilizationReaderBase.SingletonInstance;
    private readonly IDictionary<string, SystemUsageRecorder> recorders = (IDictionary<string, SystemUsageRecorder>) new Dictionary<string, SystemUsageRecorder>();
    private readonly Stopwatch watch = new Stopwatch();
    private int pollDelayInMilliSeconds;
    private CancellationTokenSource cancellation;

    private Task periodicTask { set; get; }

    private bool disposed { set; get; }

    internal int PollDelayInMs => this.pollDelayInMilliSeconds;

    public bool IsRunning() => this.periodicTask.Status == TaskStatus.Running;

    internal bool TryGetBackgroundTaskException(out AggregateException aggregateException)
    {
      aggregateException = this.periodicTask?.Exception;
      return aggregateException != null;
    }

    public static SystemUsageMonitor CreateAndStart(
      IReadOnlyList<SystemUsageRecorder> usageRecorders)
    {
      SystemUsageMonitor andStart = new SystemUsageMonitor(usageRecorders);
      andStart.Start();
      return andStart;
    }

    private SystemUsageMonitor(IReadOnlyList<SystemUsageRecorder> recorders)
    {
      if (recorders.Count == 0)
        throw new ArgumentException("No Recorders are configured so nothing to process");
      int timeInterval2 = 0;
      foreach (SystemUsageRecorder recorder in (IEnumerable<SystemUsageRecorder>) recorders)
      {
        this.recorders.Add(recorder.identifier, recorder);
        timeInterval2 = this.GCD((int) recorder.refreshInterval.TotalMilliseconds, timeInterval2);
      }
      this.pollDelayInMilliSeconds = timeInterval2;
    }

    private int GCD(int timeInterval1, int timeInterval2) => timeInterval2 != 0 ? this.GCD(timeInterval2, timeInterval1 % timeInterval2) : timeInterval1;

    private void Start()
    {
      this.ThrowIfDisposed();
      if (this.periodicTask != null)
        throw new InvalidOperationException("SystemUsageMonitor already started");
      this.cancellation = new CancellationTokenSource();
      this.periodicTask = Task.Run((Action) (() => this.RefreshLoopAsync(this.cancellation.Token)), this.cancellation.Token);
      this.periodicTask.ContinueWith((Action<Task>) (t => DefaultTrace.TraceError("The CPU and Memory usage monitoring refresh task failed. Exception: {0}", (object) t.Exception)), TaskContinuationOptions.OnlyOnFaulted);
      this.periodicTask.ContinueWith((Action<Task>) (t => DefaultTrace.TraceWarning("The CPU and Memory usage monitoring refresh task stopped. Status: {0}", (object) t.Status)), TaskContinuationOptions.NotOnFaulted);
      DefaultTrace.TraceInformation("SystemUsageMonitor started");
    }

    public void Stop()
    {
      this.ThrowIfDisposed();
      if (this.periodicTask == null)
        throw new InvalidOperationException("SystemUsageMonitor not running");
      CancellationTokenSource cancellation = this.cancellation;
      Task periodicTask = this.periodicTask;
      this.watch.Stop();
      this.cancellation = (CancellationTokenSource) null;
      this.periodicTask = (Task) null;
      cancellation.Cancel();
      try
      {
        periodicTask.Wait();
      }
      catch (AggregateException ex)
      {
      }
      cancellation.Dispose();
      DefaultTrace.TraceInformation("SystemUsageMonitor stopped");
    }

    public SystemUsageRecorder GetRecorder(string recorderKey)
    {
      this.ThrowIfDisposed();
      if (this.periodicTask == null)
      {
        DefaultTrace.TraceError("SystemUsageMonitor is not started");
        throw new InvalidOperationException("SystemUsageMonitor was not started");
      }
      SystemUsageRecorder recorder;
      if (!this.recorders.TryGetValue(recorderKey, out recorder))
        throw new ArgumentException("Recorder Identifier not present i.e. " + recorderKey);
      return recorder;
    }

    public void Dispose()
    {
      this.ThrowIfDisposed();
      if (this.periodicTask != null)
        this.Stop();
      this.disposed = true;
    }

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(nameof (SystemUsageMonitor));
    }

    private void RefreshLoopAsync(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        if (!this.watch.IsRunning)
          this.watch.Start();
        SystemUsageLoad? nullable = new SystemUsageLoad?();
        foreach (SystemUsageRecorder systemUsageRecorder in (IEnumerable<SystemUsageRecorder>) this.recorders.Values)
        {
          if (systemUsageRecorder.IsEligibleForRecording(this.watch))
          {
            if (!nullable.HasValue)
              nullable = new SystemUsageLoad?(new SystemUsageLoad(DateTime.UtcNow, ThreadInformation.Get(), new float?(this.systemUtilizationReader.GetSystemWideCpuUsage()), this.systemUtilizationReader.GetSystemWideMemoryAvailabilty(), new int?(Connection.NumberOfOpenTcpConnections)));
            systemUsageRecorder.RecordUsage(nullable.Value, this.watch);
          }
        }
        Thread.Sleep(this.pollDelayInMilliSeconds);
      }
    }
  }
}
