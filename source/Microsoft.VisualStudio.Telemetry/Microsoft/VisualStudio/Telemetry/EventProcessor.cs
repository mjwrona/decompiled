// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.EventProcessor
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber;
using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class EventProcessor : TelemetryDisposableObject, IEventProcessor, IDisposable
  {
    private readonly IEventProcessorContext eventProcessorContext;
    private readonly TelemetrySession mainSession;
    private readonly List<IEventProcessorAction> customActionList = new List<IEventProcessorAction>();
    private readonly object updateManifestLock = new object();
    private readonly Lazy<ISensitiveDataScrubber> sensitiveDataScrubber;
    private const string RedactionString = "REDACTED";
    private TelemetryManifest currentManifest;
    private bool diagnosticNeedsToBePosted = true;

    internal TelemetryManifest CurrentManifest
    {
      get => this.currentManifest;
      set
      {
        value.RequiresArgumentNotNull<TelemetryManifest>(nameof (value));
        this.UpdateManifest(value);
      }
    }

    internal IEventProcessorContext EventProcessorContext => this.eventProcessorContext;

    public EventProcessor(TelemetrySession session, IEventProcessorContext eventProcessorContext)
    {
      session.RequiresArgumentNotNull<TelemetrySession>(nameof (session));
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      this.mainSession = session;
      this.eventProcessorContext = eventProcessorContext;
      this.sensitiveDataScrubber = new Lazy<ISensitiveDataScrubber>((Func<ISensitiveDataScrubber>) (() => (ISensitiveDataScrubber) new Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber()));
    }

    public void ProcessEvent(TelemetryEvent telemetryEvent)
    {
      this.RequiresNotDisposed();
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      IEnumerable<IEventProcessorAction> eventProcessorActions = this.CurrentManifest != null ? this.GetMergedCustomAndManifestActionsInOrder(this.CurrentManifest.GetActionsForEvent(telemetryEvent)) : throw new NullReferenceException("CurrentManifest");
      bool flag = true;
      this.eventProcessorContext.InitForNewEvent(telemetryEvent);
      foreach (IEventProcessorAction eventProcessorAction in eventProcessorActions)
      {
        flag = !eventProcessorAction.Execute(this.eventProcessorContext);
        if (flag)
          break;
      }
      EventProcessor.ScrubPropertiesAndReservedProperties(telemetryEvent, this.sensitiveDataScrubber.Value);
      this.eventProcessorContext.Router.RouteEvent(telemetryEvent, this.mainSession.SessionId, flag || this.eventProcessorContext.IsEventDropped);
    }

    private static void ScrubPropertiesAndReservedProperties(
      TelemetryEvent telemetryEvent,
      ISensitiveDataScrubber scrubber)
    {
      EventProcessor.ScrubProperties(telemetryEvent?.Properties, scrubber, telemetryEvent is FaultEvent);
      EventProcessor.ScrubProperties((IDictionary<string, object>) telemetryEvent?.ReservedProperties, scrubber, telemetryEvent is FaultEvent);
    }

    private static void ScrubProperties(
      IDictionary<string, object> properties,
      ISensitiveDataScrubber scrubber,
      bool scrubAllPersonalData)
    {
      List<string> stringList = (List<string>) null;
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
      {
        if (property.Value is string && !property.Key.StartsWith("Context.Default", StringComparison.OrdinalIgnoreCase))
        {
          string propertyValue = property.Value?.ToString();
          if (propertyValue != null && scrubber.ContainsSensitiveData(propertyValue, scrubAllPersonalData))
          {
            if (stringList == null)
              stringList = new List<string>();
            stringList.Add(property.Key);
          }
        }
      }
      if (stringList == null)
        return;
      foreach (string key in stringList)
        properties[key] = (object) "REDACTED";
    }

    public void AddChannel(ISessionChannel channel)
    {
      if (this.IsDisposed)
        return;
      channel.RequiresArgumentNotNull<ISessionChannel>(nameof (channel));
      this.eventProcessorContext.Router.AddChannel(channel);
    }

    public void AddCustomAction(IEventProcessorAction eventProcessorAction)
    {
      eventProcessorAction.RequiresArgumentNotNull<IEventProcessorAction>(nameof (eventProcessorAction));
      this.customActionList.Add(eventProcessorAction);
    }

    public void PostDiagnosticInformationIfNeeded() => this.UpdateManifest((TelemetryManifest) null);

    public async Task DisposeAndTransmitAsync(CancellationToken token)
    {
      base.DisposeManagedResources();
      await this.eventProcessorContext.DisposeAndTransmitAsync(token).ConfigureAwait(false);
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      this.eventProcessorContext.Dispose();
    }

    private void UpdateManifest(TelemetryManifest manifestToBeUpdated)
    {
      lock (this.updateManifestLock)
      {
        if (this.diagnosticNeedsToBePosted)
          this.PostDiagnosticInformation(manifestToBeUpdated);
        if (manifestToBeUpdated != null)
        {
          this.currentManifest = manifestToBeUpdated;
          this.diagnosticNeedsToBePosted = true;
        }
        else
          this.diagnosticNeedsToBePosted = false;
      }
    }

    private void PostDiagnosticInformation(TelemetryManifest newManifest)
    {
      foreach (IEventProcessorAction customAction in this.customActionList)
      {
        if (customAction is IEventProcessorActionDiagnostics actionDiagnostics)
          actionDiagnostics.PostDiagnosticInformation(this.mainSession, newManifest);
      }
    }

    private IEnumerable<IEventProcessorAction> GetMergedCustomAndManifestActionsInOrder(
      IEnumerable<ITelemetryManifestAction> manifestActions)
    {
      return (IEnumerable<IEventProcessorAction>) ((IEnumerable<IEventProcessorAction>) manifestActions).Concat<IEventProcessorAction>((IEnumerable<IEventProcessorAction>) this.customActionList).OrderBy<IEventProcessorAction, int>((Func<IEventProcessorAction, int>) (action => action.Priority));
    }
  }
}
