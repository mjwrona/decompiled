// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.PersistenceChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal sealed class PersistenceChannel : ITelemetryChannel, IDisposable, ISupportConfiguration
  {
    internal readonly TelemetryBuffer TelemetryBuffer;
    internal PersistenceTransmitter Transmitter;
    private readonly FlushManager flushManager;
    private bool developerMode;
    private int disposeCount;
    private int telemetryBufferSize;
    private StorageBase storage;

    public PersistenceChannel(
      StorageBase storage,
      IProcessLockFactory processLockFactory,
      string apiKey,
      int sendersCount = 1)
    {
      this.TelemetryBuffer = new TelemetryBuffer();
      this.storage = storage;
      this.Transmitter = new PersistenceTransmitter(this.storage, sendersCount, processLockFactory);
      this.flushManager = new FlushManager(this.storage, this.TelemetryBuffer, apiKey);
      this.EndpointAddress = "https://dc.services.visualstudio.com/v2/track";
    }

    public string StorageUniqueFolder => this.Transmitter.StorageUniqueFolder;

    public bool DeveloperMode
    {
      get => this.developerMode;
      set
      {
        if (value == this.developerMode)
          return;
        if (value)
        {
          this.telemetryBufferSize = this.TelemetryBuffer.Capacity;
          this.TelemetryBuffer.Capacity = 1;
        }
        else
          this.TelemetryBuffer.Capacity = this.telemetryBufferSize;
        this.developerMode = value;
      }
    }

    public TimeSpan SendingInterval
    {
      get => this.Transmitter.SendingInterval;
      set => this.Transmitter.SendingInterval = value;
    }

    public TimeSpan FlushInterval
    {
      get => this.flushManager.FlushDelay;
      set => this.flushManager.FlushDelay = value;
    }

    public string EndpointAddress
    {
      get => this.flushManager.EndpointAddress.ToString();
      set => this.flushManager.EndpointAddress = new Uri(value ?? "https://dc.services.visualstudio.com/v2/track");
    }

    public int MaxTelemetryBufferCapacity
    {
      get => this.TelemetryBuffer.Capacity;
      set => this.TelemetryBuffer.Capacity = value;
    }

    public ulong MaxTransmissionStorageCapacity
    {
      get => this.storage.CapacityInBytes;
      set => this.storage.CapacityInBytes = value;
    }

    public uint MaxTransmissionStorageFilesCapacity
    {
      get => this.storage.MaxFiles;
      set => this.storage.MaxFiles = value;
    }

    [Obsolete("This value is now obsolete and will be removed in next release. Currently it does nothing.")]
    public double StopUploadAfterIntervalInSeconds { get; set; }

    [Obsolete("This value is now obsolete and will be removed in next release, use FlushInterval instead.")]
    public double DataUploadIntervalInSeconds
    {
      get => (double) this.flushManager.FlushDelay.Seconds;
      set => this.flushManager.FlushDelay = TimeSpan.FromSeconds(value);
    }

    [Obsolete("This value is now obsolete and will be removed in next release. Currently it does nothing.")]
    public int MaxTransmissionBufferCapacity { get; set; }

    [Obsolete("This value is now obsolete and will be removed in next release, use the sendersCount parameter in the constructor instead.")]
    public int MaxTransmissionSenderCapacity { get; set; }

    public void Dispose()
    {
      if (Interlocked.Increment(ref this.disposeCount) != 1)
        return;
      if (this.Transmitter != null)
        this.Transmitter.Dispose();
      if (this.flushManager == null)
        return;
      this.flushManager.Dispose();
    }

    public void Send(ITelemetry item) => this.TelemetryBuffer.Enqueue(item);

    public void Flush() => this.flushManager.Flush();

    public async Task FlushAndTransmitAsync(CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      this.Flush();
      token.ThrowIfCancellationRequested();
      await this.Transmitter.Flush(token).ConfigureAwait(false);
    }

    public void Initialize(TelemetryConfiguration configuration)
    {
    }
  }
}
