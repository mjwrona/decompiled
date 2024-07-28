// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.InMemoryChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  public class InMemoryChannel : ITelemetryChannel, IDisposable
  {
    private readonly TelemetryBuffer buffer;
    private readonly InMemoryTransmitter transmitter;
    private bool developerMode;
    private int bufferSize;

    public InMemoryChannel()
    {
      this.buffer = new TelemetryBuffer();
      this.transmitter = new InMemoryTransmitter(this.buffer);
    }

    internal InMemoryChannel(TelemetryBuffer telemetryBuffer, InMemoryTransmitter transmitter)
    {
      this.buffer = telemetryBuffer;
      this.transmitter = transmitter;
    }

    public bool DeveloperMode
    {
      get => this.developerMode;
      set
      {
        if (value == this.developerMode)
          return;
        if (value)
        {
          this.bufferSize = this.buffer.Capacity;
          this.buffer.Capacity = 1;
        }
        else
          this.buffer.Capacity = this.bufferSize;
        this.developerMode = value;
      }
    }

    public TimeSpan SendingInterval
    {
      get => this.transmitter.SendingInterval;
      set => this.transmitter.SendingInterval = value;
    }

    public string EndpointAddress
    {
      get => this.transmitter.EndpointAddress.ToString();
      set => this.transmitter.EndpointAddress = new Uri(value);
    }

    [Obsolete("This value is now obsolete and will be removed in next release, use SendingInterval instead.")]
    public double DataUploadIntervalInSeconds
    {
      get => this.transmitter.SendingInterval.TotalSeconds;
      set => this.transmitter.SendingInterval = TimeSpan.FromSeconds(value);
    }

    public int MaxTelemetryBufferCapacity
    {
      get => this.buffer.Capacity;
      set => this.buffer.Capacity = value;
    }

    public void Send(ITelemetry item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      try
      {
        this.buffer.Enqueue(item);
        CoreEventSource.Log.LogVerbose("TelemetryBuffer.Enqueue succeeded");
      }
      catch (Exception ex)
      {
        CoreEventSource.Log.LogVerbose("TelemetryBuffer.Enqueue failed: ", ex.ToString());
      }
    }

    public void Flush() => this.transmitter.Flush();

    public async Task FlushAndTransmitAsync(CancellationToken token)
    {
      this.transmitter.Flush();
      int num = await Task.FromResult<bool>(true) ? 1 : 0;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.transmitter == null)
        return;
      this.transmitter.Dispose();
    }
  }
}
