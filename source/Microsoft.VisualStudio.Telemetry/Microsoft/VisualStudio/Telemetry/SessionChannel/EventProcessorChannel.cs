// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.EventProcessorChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal class EventProcessorChannel : 
    TelemetryDisposableObject,
    ISessionChannel,
    IDisposeAndTransmit
  {
    private const int SchedulerDelay = 1;
    private readonly ConcurrentQueue<TelemetryEvent> queue = new ConcurrentQueue<TelemetryEvent>();
    private readonly IEventProcessor eventProcessor;
    private readonly ITelemetryScheduler scheduler;
    private readonly TelemetrySession telemetrySession;
    private bool hasProcessedEvents;
    private Action initializedAction = (Action) (() => { });

    public string ChannelId => "eventProcessorChannel";

    public ChannelProperties Properties
    {
      get => ChannelProperties.None;
      set => throw new MemberAccessException("it is not allowed to change properties for this channel");
    }

    internal EventProcessorChannel(
      IEventProcessor theEventProcessor,
      ITelemetryScheduler theScheduler,
      TelemetrySession telemetrySession)
    {
      theEventProcessor.RequiresArgumentNotNull<IEventProcessor>(nameof (theEventProcessor));
      theScheduler.RequiresArgumentNotNull<ITelemetryScheduler>(nameof (theScheduler));
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      this.eventProcessor = theEventProcessor;
      this.scheduler = theScheduler;
      this.scheduler.InitializeTimed(TimeSpan.FromSeconds(1.0));
      this.telemetrySession = telemetrySession;
    }

    public void PostEvent(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      if (this.IsDisposed)
        return;
      this.queue.Enqueue(telemetryEvent);
      this.scheduler.ScheduleTimed(new Action(this.ProcessEvents));
    }

    public void PostEvent(
      TelemetryEvent telemetryEvent,
      IEnumerable<ITelemetryManifestRouteArgs> args)
    {
      throw new ApplicationException("event is routed to the EventProcessor channel");
    }

    public void Start(string sessionId)
    {
    }

    public bool IsStarted => true;

    public string TransportUsed => this.ChannelId;

    public void ProcessEvents()
    {
      try
      {
        if (this.IsDisposed)
          throw new ObjectDisposedException(this.GetType().Name, "it is not allowed to process events after channel is disposed");
        if (!this.scheduler.CanEnterTimedDelegate())
          return;
        TelemetryEvent result;
        while (this.queue.TryDequeue(out result))
          this.eventProcessor.ProcessEvent(result);
        if (this.hasProcessedEvents)
          return;
        this.hasProcessedEvents = true;
        this.initializedAction();
      }
      catch (Exception ex)
      {
        FaultEvent faultEvent = new FaultEvent("VS/Telemetry/InternalFault", string.Format("Exception in SessionChannel.EventProcessorChannel ProcessEvents Channel = {0}", (object) this.ChannelId), ex)
        {
          PostThisEventToTelemetry = false
        };
        using (Process currentProcess = Process.GetCurrentProcess())
        {
          faultEvent.AddProcessDump(currentProcess.Id);
          this.telemetrySession.PostEvent((TelemetryEvent) faultEvent);
        }
      }
      finally
      {
        this.scheduler.ExitTimedDelegate();
      }
    }

    public async Task DisposeAndTransmitAsync(CancellationToken token)
    {
      this.DisposeInit();
      await this.eventProcessor.DisposeAndTransmitAsync(token).ConfigureAwait(false);
    }

    internal Action InitializedAction
    {
      set
      {
        value.RequiresArgumentNotNull<Action>(nameof (value));
        this.initializedAction = value;
      }
    }

    protected override void DisposeManagedResources()
    {
      this.DisposeInit();
      this.eventProcessor.Dispose();
    }

    private void DisposeInit()
    {
      base.DisposeManagedResources();
      this.scheduler.CancelTimed(true);
      this.ProcessEvents();
    }

    public override string ToString() => string.Format("{0} QueueCnt = {1}", (object) this.ChannelId, (object) this.queue.Count);
  }
}
