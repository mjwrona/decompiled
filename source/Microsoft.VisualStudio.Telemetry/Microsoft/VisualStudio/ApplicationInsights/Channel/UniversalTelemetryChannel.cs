// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.UniversalTelemetryChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  public sealed class UniversalTelemetryChannel : ITelemetryChannel, IDisposable
  {
    private readonly ConcurrentDictionary<string, EventSourceWriter> eventSourceWriters;
    private bool disposed;

    public UniversalTelemetryChannel() => this.eventSourceWriters = new ConcurrentDictionary<string, EventSourceWriter>();

    public bool DeveloperMode { get; set; }

    public string EndpointAddress { get; set; }

    public static bool IsAvailable() => false;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public void Send(ITelemetry item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this.GetEventSourceWriter(item.Context.InstrumentationKey).WriteTelemetry(item);
    }

    public void Flush()
    {
    }

    public async Task FlushAndTransmitAsync(CancellationToken token)
    {
      int num = await Task.FromResult<bool>(true) ? 1 : 0;
    }

    internal EventSourceWriter GetEventSourceWriter(string instrumentationKey) => this.eventSourceWriters.GetOrAdd(instrumentationKey, (Func<string, EventSourceWriter>) (key => new EventSourceWriter(key, this.DeveloperMode)));

    private void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      if (disposing)
      {
        foreach (KeyValuePair<string, EventSourceWriter> eventSourceWriter in this.eventSourceWriters)
          eventSourceWriter.Value.Dispose();
      }
      this.disposed = true;
    }
  }
}
