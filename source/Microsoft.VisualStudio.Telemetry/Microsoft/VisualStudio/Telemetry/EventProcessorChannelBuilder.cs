// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.EventProcessorChannelBuilder
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class EventProcessorChannelBuilder
  {
    private readonly IPersistentPropertyBag persistentPropertyBag;
    private readonly ITelemetryScheduler telemetryScheduler;

    public EventProcessor EventProcessor { get; private set; }

    public EventProcessorChannel EventProcessorChannel { get; private set; }

    public EventProcessorContext EventProcessorContext { get; private set; }

    public EventProcessorRouter EventProcessorRouter { get; private set; }

    public EventProcessorChannelBuilder(
      IPersistentPropertyBag persistentPropertyBag,
      ITelemetryScheduler telemetryScheduler)
    {
      persistentPropertyBag.RequiresArgumentNotNull<IPersistentPropertyBag>(nameof (persistentPropertyBag));
      telemetryScheduler.RequiresArgumentNotNull<ITelemetryScheduler>(nameof (telemetryScheduler));
      this.persistentPropertyBag = persistentPropertyBag;
      this.telemetryScheduler = telemetryScheduler;
    }

    public void Build(TelemetrySession hostSession)
    {
      hostSession.RequiresArgumentNotNull<TelemetrySession>(nameof (hostSession));
      this.EventProcessorRouter = this.BuildRouter();
      this.EventProcessorContext = this.BuildContext(hostSession, (IEventProcessorRouter) this.EventProcessorRouter);
      this.EventProcessor = this.BuildProcessor(hostSession, (IEventProcessorContext) this.EventProcessorContext);
      this.EventProcessorChannel = this.BuildChannel((IEventProcessor) this.EventProcessor, hostSession);
    }

    private EventProcessorChannel BuildChannel(
      IEventProcessor eventProcessor,
      TelemetrySession telemetrySession)
    {
      return new EventProcessorChannel(eventProcessor, this.telemetryScheduler, telemetrySession);
    }

    private EventProcessor BuildProcessor(
      TelemetrySession hostSession,
      IEventProcessorContext context)
    {
      return new EventProcessor(hostSession, context);
    }

    private EventProcessorContext BuildContext(
      TelemetrySession hostSession,
      IEventProcessorRouter eventProcessorRouter)
    {
      return new EventProcessorContext(hostSession, eventProcessorRouter);
    }

    private EventProcessorRouter BuildRouter() => new EventProcessorRouter(this.persistentPropertyBag);
  }
}
