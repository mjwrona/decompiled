// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.FlushManager
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal class FlushManager : IDisposable
  {
    private readonly TelemetryBuffer telemetryBuffer;
    private AutoResetEvent flushWaitHandle;
    private StorageBase storage;
    private int disposeCount;
    private bool flushLoopEnabled = true;

    internal FlushManager(
      StorageBase storage,
      TelemetryBuffer telemetryBuffer,
      string apiKey,
      bool supportAutoFlush = true)
      : this(storage, telemetryBuffer, supportAutoFlush)
    {
      this.ApiKey = !string.IsNullOrEmpty(apiKey) ? apiKey : throw new ArgumentNullException("ApiKey cannot be null or empty");
    }

    internal FlushManager(
      StorageBase storage,
      TelemetryBuffer telemetryBuffer,
      bool supportAutoFlush = true)
    {
      this.storage = storage;
      this.telemetryBuffer = telemetryBuffer;
      this.telemetryBuffer.OnFull = new Action(this.OnTelemetryBufferFull);
      this.FlushDelay = TimeSpan.FromSeconds(30.0);
      if (!supportAutoFlush)
        return;
      Task.Factory.StartNew(new Action(this.FlushLoop), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).ContinueWith((Action<Task>) (t => CoreEventSource.Log.LogVerbose("FlushManager: Failure in FlushLoop: Exception: " + t.Exception.ToString())), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
    }

    internal TimeSpan FlushDelay { get; set; }

    internal Uri EndpointAddress { get; set; }

    private string ApiKey { get; }

    public void Dispose()
    {
      this.flushLoopEnabled = false;
      this.FlushDelay = TimeSpan.FromSeconds(0.0);
      if (Interlocked.Increment(ref this.disposeCount) != 1 || this.flushWaitHandle == null)
        return;
      this.flushWaitHandle.Set();
    }

    internal void Flush()
    {
      IEnumerable<ITelemetry> telemetries = this.telemetryBuffer.Dequeue();
      if (telemetries == null || !telemetries.Any<ITelemetry>())
        return;
      byte[] content = JsonSerializer.Serialize(telemetries);
      this.storage.EnqueueAsync(!"https://events.data.microsoft.com/OneCollector/1.0".Equals(this.EndpointAddress.OriginalString, StringComparison.OrdinalIgnoreCase) ? new Transmission(this.EndpointAddress, content, "application/x-json-stream; charset=utf-8", JsonSerializer.CompressionType) : new Transmission(this.EndpointAddress, content, "application/x-json-stream; charset=utf-8", JsonSerializer.CompressionType, this.ApiKey)).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private void FlushLoop()
    {
      using (this.flushWaitHandle = new AutoResetEvent(false))
      {
        while (this.flushLoopEnabled)
        {
          this.Flush();
          this.flushWaitHandle.WaitOne(this.FlushDelay);
        }
      }
    }

    private void OnTelemetryBufferFull()
    {
      if (this.flushWaitHandle == null || !this.flushLoopEnabled)
        return;
      this.flushWaitHandle.Set();
    }
  }
}
