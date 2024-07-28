// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.InMemoryTransmitter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal class InMemoryTransmitter : IDisposable
  {
    private readonly TelemetryBuffer buffer;
    private object sendingLockObj = new object();
    private AutoResetEvent startRunnerEvent;
    private bool enabled = true;
    private int disposeCount;
    private TimeSpan sendingInterval = TimeSpan.FromSeconds(30.0);
    private Uri endpointAddress = new Uri("https://dc.services.visualstudio.com/v2/track");

    internal InMemoryTransmitter(TelemetryBuffer buffer)
    {
      this.buffer = buffer;
      this.buffer.OnFull = new Action(this.OnBufferFull);
      Task.Factory.StartNew(new Action(this.Runner), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).ContinueWith((Action<Task>) (task =>
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "InMemoryTransmitter: Unhandled exception in Runner: {0}", new object[1]
        {
          (object) task.Exception
        });
        CoreEventSource.Log.LogVerbose(message);
      }), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
    }

    internal Uri EndpointAddress
    {
      get => this.endpointAddress;
      set => Property.Set<Uri>(ref this.endpointAddress, value);
    }

    internal TimeSpan SendingInterval
    {
      get => this.sendingInterval;
      set => this.sendingInterval = value;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    internal void Flush() => this.DequeueAndSend();

    private void Runner()
    {
      using (this.startRunnerEvent = new AutoResetEvent(false))
      {
        while (this.enabled)
        {
          this.DequeueAndSend();
          this.startRunnerEvent.WaitOne(this.sendingInterval);
        }
      }
    }

    private void OnBufferFull() => this.startRunnerEvent.Set();

    private void DequeueAndSend()
    {
      lock (this.sendingLockObj)
      {
        IEnumerable<ITelemetry> telemetryItems = this.buffer.Dequeue();
        try
        {
          this.Send(telemetryItems).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
          CoreEventSource.Log.LogVerbose("DequeueAndSend: Failed Sending: Exception: " + ex.ToString());
        }
      }
    }

    private async Task Send(IEnumerable<ITelemetry> telemetryItems)
    {
      if (telemetryItems == null || !telemetryItems.Any<ITelemetry>())
        CoreEventSource.Log.LogVerbose("No Telemetry Items passed to Enqueue");
      else
        await new Transmission(this.endpointAddress, JsonSerializer.Serialize(telemetryItems), "application/x-json-stream; charset=utf-8", JsonSerializer.CompressionType).SendAsync().ConfigureAwait(false);
    }

    private void Dispose(bool disposing)
    {
      if (Interlocked.Increment(ref this.disposeCount) != 1)
        return;
      this.enabled = false;
      if (this.startRunnerEvent == null)
        return;
      this.startRunnerEvent.Set();
    }
  }
}
