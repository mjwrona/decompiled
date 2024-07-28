// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.Sender
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.VisualStudio.LocalLogger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal class Sender : IDisposable
  {
    private const int TriggerCountStaleHashCheck = 50;
    private static readonly TimeSpan StaleHashPeriod = TimeSpan.FromMinutes(5.0);
    protected readonly AutoResetEvent DelayHandler;
    private readonly TimeSpan sendingIntervalOnNoData = TimeSpan.FromSeconds(10.0);
    private readonly TimeSpan maxIntervalBetweenRetries = TimeSpan.FromHours(1.0);
    private readonly AutoResetEvent stoppedHandler;
    private readonly LinkedList<Tuple<DateTime, string>> listOfTransmissionHash = new LinkedList<Tuple<DateTime, string>>();
    private readonly Dictionary<string, LinkedListNode<Tuple<DateTime, string>>> setOfTransmissionHash = new Dictionary<string, LinkedListNode<Tuple<DateTime, string>>>();
    private readonly object hashLock = new object();
    private int checkStaleHashCount;
    private bool stopped;
    private TimeSpan drainingTimeout;
    private StorageBase storage;
    private int disposeCount;
    private PersistenceTransmitter transmitter;

    internal Sender(StorageBase storage, PersistenceTransmitter transmitter, bool startSending = true)
    {
      this.stopped = false;
      this.DelayHandler = new AutoResetEvent(false);
      this.stoppedHandler = new AutoResetEvent(false);
      this.drainingTimeout = TimeSpan.FromSeconds(100.0);
      this.transmitter = transmitter;
      this.storage = storage;
      if (!startSending)
        return;
      Task.Factory.StartNew(new Action(this.SendLoop), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).ContinueWith((Action<Task>) (t =>
      {
        CoreEventSource.Log.LogVerbose("Sender: Failure in SendLoop: Exception: " + t.Exception.ToString());
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", "Sender: Failure in SendLoop: Exception: " + t.Exception.Message);
      }), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
    }

    private TimeSpan SendingInterval => this.transmitter.SendingInterval;

    public void Dispose()
    {
      if (Interlocked.Increment(ref this.disposeCount) != 1)
        return;
      LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", "Sender.Dispose() start disposing sender");
      this.StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
      this.DelayHandler.Dispose();
      this.stoppedHandler.Dispose();
    }

    internal Task StopAsync()
    {
      this.stopped = true;
      this.DelayHandler.Set();
      return Task.Factory.StartNew((Action) (() =>
      {
        try
        {
          this.stoppedHandler.WaitOne(this.drainingTimeout);
        }
        catch (ObjectDisposedException ex)
        {
        }
      }), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
    }

    internal async Task FlushAll(CancellationToken token)
    {
      List<Task> taskList = new List<Task>();
      token.ThrowIfCancellationRequested();
      LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", "Sender.FlushAll() start flushing all");
      foreach (StorageTransmission storageTransmission in this.storage.PeekAll(token))
      {
        StorageTransmission transmission = storageTransmission;
        token.ThrowIfCancellationRequested();
        taskList.Add(Task.Run((Func<Task>) (async () =>
        {
          token.ThrowIfCancellationRequested();
          if (!(await this.SendAsync(transmission, token, new TimeSpan()).ConfigureAwait(false)).Item1)
            this.storage.Delete(transmission);
          transmission.Dispose();
        })));
      }
      await Task.WhenAll((IEnumerable<Task>) taskList).ConfigureAwait(false);
    }

    protected void SendLoop()
    {
      TimeSpan prevSendInterval = TimeSpan.Zero;
      TimeSpan intervalOnNoData = this.sendingIntervalOnNoData;
      try
      {
        while (!this.stopped)
        {
          using (StorageTransmission transmission = this.storage.Peek())
          {
            if (!this.stopped)
            {
              if (transmission != null)
              {
                if (LocalFileLoggerService.Default.Enabled)
                  LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceTransmitter.SendLoop about to send", new object[1]
                  {
                    (object) transmission
                  }));
                bool flag = this.Send(transmission, ref intervalOnNoData);
                if (LocalFileLoggerService.Default.Enabled)
                  LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceTransmitter.SendLoop shouldRetry == {1}", new object[2]
                  {
                    (object) transmission,
                    (object) flag
                  }));
                if (!flag)
                  this.storage.Delete(transmission);
              }
              else
                intervalOnNoData = this.sendingIntervalOnNoData;
            }
            else
              break;
          }
          Sender.LogInterval(prevSendInterval, intervalOnNoData);
          this.DelayHandler.WaitOne(intervalOnNoData);
          prevSendInterval = intervalOnNoData;
        }
        this.stoppedHandler.Set();
      }
      catch (ObjectDisposedException ex)
      {
      }
    }

    protected virtual bool Send(StorageTransmission transmission, ref TimeSpan nextSendInterval)
    {
      Tuple<bool, TimeSpan> result = this.SendAsync(transmission, new CancellationToken(), nextSendInterval).ConfigureAwait(false).GetAwaiter().GetResult();
      nextSendInterval = result.Item2;
      return result.Item1;
    }

    private async Task<Tuple<bool, TimeSpan>> SendAsync(
      StorageTransmission transmission,
      CancellationToken token,
      TimeSpan sendInterval)
    {
      bool isRetryable = false;
      try
      {
        if (transmission != null)
        {
          bool flag = true;
          lock (this.hashLock)
          {
            flag = !this.setOfTransmissionHash.ContainsKey(transmission.ContentHash);
            if (flag)
            {
              this.setOfTransmissionHash[transmission.ContentHash] = this.listOfTransmissionHash.AddLast(Tuple.Create<DateTime, string>(DateTime.UtcNow, transmission.ContentHash));
              ++this.checkStaleHashCount;
              if (this.checkStaleHashCount >= 50)
              {
                this.CleanupStaleTransmissionHash();
                this.checkStaleHashCount = 0;
              }
            }
          }
          if (flag)
            await transmission.SendAsync(token).ConfigureAwait(false);
          sendInterval = this.SendingInterval;
        }
      }
      catch (WebException ex)
      {
        int? statusCode = Sender.GetStatusCode(ex);
        sendInterval = this.CalculateNextInterval(statusCode, sendInterval, this.maxIntervalBetweenRetries);
        isRetryable = Sender.IsRetryable(statusCode, ex.Status);
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceTransmitter.SendAsync WebException ({1}), isRetryable == {2}", new object[3]
        {
          (object) transmission,
          (object) ex.Message,
          (object) isRetryable
        }));
      }
      catch (Exception ex)
      {
        sendInterval = this.CalculateNextInterval(new int?(), sendInterval, this.maxIntervalBetweenRetries);
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unknown exception during sending: {0}", new object[1]
        {
          (object) ex
        });
        CoreEventSource.Log.LogVerbose(message);
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceTransmitter.SendAsync Exception ({1})", new object[2]
        {
          (object) transmission,
          (object) ex.Message
        }));
        if (ex is OperationCanceledException)
          throw;
      }
      if (isRetryable)
      {
        lock (this.hashLock)
        {
          LinkedListNode<Tuple<DateTime, string>> node = (LinkedListNode<Tuple<DateTime, string>>) null;
          if (this.setOfTransmissionHash.TryGetValue(transmission.ContentHash, out node))
          {
            if (node != null)
            {
              this.listOfTransmissionHash.Remove(node);
              this.setOfTransmissionHash.Remove(transmission.ContentHash);
            }
          }
        }
      }
      return Tuple.Create<bool, TimeSpan>(isRetryable, sendInterval);
    }

    private void CleanupStaleTransmissionHash()
    {
      while (this.listOfTransmissionHash.Count > 0 && DateTime.UtcNow - this.listOfTransmissionHash.First.Value.Item1 > Sender.StaleHashPeriod)
      {
        this.setOfTransmissionHash.Remove(this.listOfTransmissionHash.First.Value.Item2);
        this.listOfTransmissionHash.RemoveFirst();
      }
    }

    private static void LogInterval(TimeSpan prevSendInterval, TimeSpan nextSendInterval)
    {
      if (Math.Abs(nextSendInterval.TotalSeconds - prevSendInterval.TotalSeconds) <= 60.0)
        return;
      CoreEventSource.Log.LogVerbose("next sending interval: " + nextSendInterval.ToString());
    }

    private static int? GetStatusCode(WebException e) => e.Response is HttpWebResponse response ? new int?((int) response.StatusCode) : new int?();

    private static bool IsRetryable(int? httpStatusCode, WebExceptionStatus webExceptionStatus)
    {
      switch (webExceptionStatus)
      {
        case WebExceptionStatus.NameResolutionFailure:
        case WebExceptionStatus.ConnectFailure:
        case WebExceptionStatus.Timeout:
        case WebExceptionStatus.ProxyNameResolutionFailure:
          return true;
        default:
          if (!httpStatusCode.HasValue)
            return false;
          switch (httpStatusCode.Value)
          {
            case 408:
            case 500:
            case 502:
            case 503:
            case 511:
              return true;
            default:
              return false;
          }
      }
    }

    private TimeSpan CalculateNextInterval(
      int? httpStatusCode,
      TimeSpan currentSendInterval,
      TimeSpan maxInterval)
    {
      if (httpStatusCode.HasValue && httpStatusCode.Value == 400)
        return this.SendingInterval;
      return currentSendInterval.TotalSeconds == 0.0 ? TimeSpan.FromSeconds(1.0) : TimeSpan.FromSeconds(Math.Min(currentSendInterval.TotalSeconds * 2.0, maxInterval.TotalSeconds));
    }
  }
}
