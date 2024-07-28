// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.PersistenceTransmitter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.VisualStudio.LocalLogger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal class PersistenceTransmitter : IDisposable
  {
    private readonly IProcessLock locker;
    private ConcurrentBag<Sender> senders = new ConcurrentBag<Sender>();
    private StorageBase storage;
    private CancellationTokenSource sendingCancellationTokenSource;
    private int disposeCount;
    private AutoResetEvent eventToKeepMutexThreadAlive;

    internal PersistenceTransmitter(StorageBase storage, int sendersCount, bool createSenders = true)
      : this(storage, sendersCount, (IProcessLockFactory) new WindowsProcessLockFactory(), createSenders)
    {
    }

    internal PersistenceTransmitter(
      StorageBase storage,
      int sendersCount,
      IProcessLockFactory processLockFactory,
      bool createSenders = true)
    {
      PersistenceTransmitter persistenceTransmitter = this;
      this.storage = storage;
      this.sendingCancellationTokenSource = new CancellationTokenSource();
      this.eventToKeepMutexThreadAlive = new AutoResetEvent(false);
      try
      {
        string folderFullName = this.storage.StorageFolder?.FullName ?? string.Empty;
        this.locker = processLockFactory.CreateLocker(folderFullName, "_675531BB6E734D2F846AB8511A8963FD_");
      }
      catch (Exception ex)
      {
        this.locker = (IProcessLock) null;
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PersistenceTransmitter: Failed to construct the mutex: {0}", new object[1]
        {
          (object) ex
        });
        CoreEventSource.Log.LogVerbose(message);
      }
      if (!createSenders)
        return;
      Task.Factory.StartNew((Action) (() => persistenceTransmitter.AcquireMutex((Action) (() => persistenceTransmitter.CreateSenders(sendersCount)))), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).ContinueWith((Action<Task>) (task =>
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PersistenceTransmitter: Unhandled exception in CreateSenders: {0}", new object[1]
        {
          (object) task.Exception
        });
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", str);
        CoreEventSource.Log.LogVerbose(str);
      }), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
    }

    internal string StorageUniqueFolder => this.storage.FolderName;

    internal TimeSpan SendingInterval { get; set; }

    public async Task Flush(CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      Sender result;
      if (!this.senders.TryPeek(out result) || result == null)
        return;
      await result.FlushAll(token).ConfigureAwait(false);
    }

    public void Dispose()
    {
      if (Interlocked.Increment(ref this.disposeCount) != 1)
        return;
      this.sendingCancellationTokenSource.Cancel();
      this.sendingCancellationTokenSource.Dispose();
      this.locker?.Dispose();
      this.eventToKeepMutexThreadAlive.Dispose();
      this.StopSenders();
    }

    private void AcquireMutex(Action action)
    {
      if (this.locker == null)
        return;
      while (!this.sendingCancellationTokenSource.IsCancellationRequested)
      {
        try
        {
          LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", "PersistenceTransmitter.AcquireMutex try to acquire mutex");
          this.locker.Acquire(action, this.sendingCancellationTokenSource.Token);
          if (this.sendingCancellationTokenSource.IsCancellationRequested)
            return;
          LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", "PersistenceTransmitter.AcquireMutex mutex acquired successfully and action executed");
          this.eventToKeepMutexThreadAlive.WaitOne();
          LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", "PersistenceTransmitter.AcquireMutex exit from the thread naturally");
          return;
        }
        catch (ObjectDisposedException ex)
        {
          LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", "PersistenceTransmitter.AcquireMutex exit from the thread by object disposed exception");
          return;
        }
        catch (ProcessLockException ex)
        {
          if (!ex.IsRetryable)
          {
            LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", "PersistenceTransmitter.AcquireMutex exit from the thread by non-retriebale ProcessLockException");
            return;
          }
        }
      }
      LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", "PersistenceTransmitter.AcquireMutex exit from the thread because of the cancellation token source");
    }

    private void CreateSenders(int sendersCount)
    {
      LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", "PersistenceTransmitter.CreateSenders start creating senders");
      for (int index = 0; index < sendersCount; ++index)
        this.senders.Add(new Sender(this.storage, this));
      LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", "PersistenceTransmitter.CreateSenders finished creating senders");
    }

    private void StopSenders()
    {
      if (this.senders == null)
        return;
      List<Task> taskList = new List<Task>();
      foreach (Sender sender in this.senders)
        taskList.Add(sender.StopAsync());
      Task.WaitAll(taskList.ToArray());
    }
  }
}
