// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.EventProcessorContext
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class EventProcessorContext : 
    TelemetryDisposableObject,
    IEventProcessorContext,
    IDisposable
  {
    private readonly TelemetrySession hostTelemetrySession;
    private readonly IEventProcessorRouter eventProcessorRouter;
    private TelemetryEvent workerTelemetryEvent;
    private Dictionary<string, object> excludedProperties;

    public bool IsEventDropped { get; set; }

    public ThrottlingAction ThrottlingAction { get; set; }

    public TelemetrySession HostTelemetrySession => this.hostTelemetrySession;

    public IEventProcessorRouter Router => this.eventProcessorRouter;

    public TelemetryEvent TelemetryEvent => this.workerTelemetryEvent;

    public EventProcessorContext(
      TelemetrySession hostTelemetrySession,
      IEventProcessorRouter eventProcessorRouter)
    {
      hostTelemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (hostTelemetrySession));
      eventProcessorRouter.RequiresArgumentNotNull<IEventProcessorRouter>(nameof (eventProcessorRouter));
      this.hostTelemetrySession = hostTelemetrySession;
      this.eventProcessorRouter = eventProcessorRouter;
    }

    public void InitForNewEvent(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      this.workerTelemetryEvent = telemetryEvent;
      this.IsEventDropped = false;
      this.excludedProperties = (Dictionary<string, object>) null;
      this.ThrottlingAction = ThrottlingAction.Default;
      this.Router.Reset();
      this.Router.UpdateDefaultChannel(this.HostTelemetrySession.UseCollector);
    }

    public void ExcludePropertyFromEvent(string propertyName)
    {
      propertyName.RequiresArgumentNotNullAndNotEmpty(nameof (propertyName));
      if (this.TelemetryEvent == null || !this.TelemetryEvent.Properties.ContainsKey(propertyName))
        return;
      if (this.excludedProperties == null)
        this.excludedProperties = new Dictionary<string, object>();
      this.excludedProperties[propertyName] = this.TelemetryEvent.Properties[propertyName];
      this.TelemetryEvent.Properties.Remove(propertyName);
    }

    public void IncludePropertyToEvent(string propertyName)
    {
      propertyName.RequiresArgumentNotNullAndNotEmpty(nameof (propertyName));
      object obj;
      if (this.excludedProperties == null || this.TelemetryEvent == null || !this.excludedProperties.TryGetValue(propertyName, out obj))
        return;
      this.TelemetryEvent.Properties[propertyName] = obj;
    }

    public async Task DisposeAndTransmitAsync(CancellationToken token)
    {
      base.DisposeManagedResources();
      await this.Router.DisposeAndTransmitAsync(token).ConfigureAwait(false);
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      this.Router.Dispose();
    }
  }
}
