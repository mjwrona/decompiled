// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.ClientSideThrottlingAction
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class ClientSideThrottlingAction : 
    IEventProcessorAction,
    IEventProcessorActionDiagnostics
  {
    private const long BaseThreshold = 100;
    private const string UnknownValue = "Unknown";
    private readonly HashSet<string> passthroughEvents = new HashSet<string>();
    private readonly HashSet<string> droppedEvents = new HashSet<string>();
    private readonly HashSet<string> noisyAllowedListEvents = new HashSet<string>();
    private long counter;
    private long allowedListCounter;
    private double resetCounter = 10.0;
    private long threshold;
    private DateTimeOffset bucketStartTime;

    public int Priority => 1000;

    public ClientSideThrottlingAction(
      IEnumerable<string> passthroughEvents = null,
      double resetCounterOverride = 0.0,
      long thresholdOverride = 0)
    {
      if (passthroughEvents != null)
        this.passthroughEvents.UnionWith(passthroughEvents);
      if (resetCounterOverride > 0.0)
        this.resetCounter = resetCounterOverride;
      if (thresholdOverride > 0L)
        this.threshold = thresholdOverride;
      else
        this.threshold = (long) (100.0 * this.resetCounter);
    }

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
      DateTimeOffset postTimestamp = telemetryEvent.PostTimestamp;
      if (this.bucketStartTime == new DateTimeOffset())
        this.bucketStartTime = postTimestamp;
      if (eventProcessorContext.ThrottlingAction == ThrottlingAction.DoNotThrottle || eventProcessorContext.ThrottlingAction != ThrottlingAction.Throttle && this.passthroughEvents.Contains(telemetryEvent.Name))
      {
        if (this.counter < this.threshold)
          ++this.counter;
        else if (this.allowedListCounter >= this.threshold)
          this.noisyAllowedListEvents.Add(telemetryEvent.Name);
        ++this.allowedListCounter;
        return true;
      }
      if ((postTimestamp - this.bucketStartTime).TotalSeconds > this.resetCounter)
      {
        TelemetrySession telemetrySession = eventProcessorContext.HostTelemetrySession;
        this.Reset(telemetrySession, telemetrySession.EventProcessor.CurrentManifest, postTimestamp);
      }
      if (this.counter++ < this.threshold)
        return true;
      this.droppedEvents.Add(telemetryEvent.Name);
      return false;
    }

    public void PostDiagnosticInformation(
      TelemetrySession mainSession,
      TelemetryManifest newManifest)
    {
      this.Reset(mainSession, newManifest, new DateTimeOffset());
    }

    public void AddPassthroughEventName(string eventName)
    {
      eventName.RequiresArgumentNotNullAndNotEmpty(nameof (eventName));
      this.passthroughEvents.Add(eventName);
    }

    private void Reset(
      TelemetrySession mainSession,
      TelemetryManifest newManifest,
      DateTimeOffset timeToReset)
    {
      if (mainSession != null && (this.counter > this.threshold || this.allowedListCounter > this.threshold))
      {
        string str = "Unknown";
        TelemetryManifest currentManifest = mainSession.EventProcessor.CurrentManifest;
        if (currentManifest != null)
          str = currentManifest.Version;
        Dictionary<string, object> source = new Dictionary<string, object>();
        source["VS.TelemetryApi.DynamicTelemetry.Manifest.Version"] = (object) str;
        source["VS.TelemetryApi.DynamicTelemetry.HostName"] = (object) mainSession.HostName;
        source["VS.TelemetryApi.ClientSideThrottling.Threshold"] = (object) this.threshold;
        source["VS.TelemetryApi.ClientSideThrottling.TimerReset"] = (object) this.resetCounter;
        source["VS.TelemetryApi.ClientSideThrottling.BucketStart"] = (object) this.bucketStartTime.UtcDateTime.ToString("MM/dd/yy H:mm:ss.fffffff", (IFormatProvider) CultureInfo.InvariantCulture);
        if (this.counter > this.threshold)
        {
          long num = this.counter - this.threshold;
          TelemetryEvent telemetryEvent = new TelemetryEvent("VS/TelemetryApi/ClientSideThrottling");
          telemetryEvent.Properties.AddRange<string, object>((IDictionary<string, object>) source);
          telemetryEvent.Properties["VS.TelemetryApi.ClientSideThrottling.TotalDropped"] = (object) num;
          telemetryEvent.Properties["VS.TelemetryApi.ClientSideThrottling.Events"] = (object) this.droppedEvents.Join(",");
          mainSession.PostEvent(telemetryEvent);
        }
        if (this.allowedListCounter > this.threshold)
        {
          long num = this.allowedListCounter - this.threshold;
          TelemetryEvent telemetryEvent = new TelemetryEvent("VS/TelemetryApi/ClientSideThrottling/NoisyAllowedList");
          telemetryEvent.Properties.AddRange<string, object>((IDictionary<string, object>) source);
          telemetryEvent.Properties["VS.TelemetryApi.ClientSideThrottling.TotalNoise"] = (object) num;
          telemetryEvent.Properties["VS.TelemetryApi.ClientSideThrottling.Events"] = (object) this.noisyAllowedListEvents.Join(",");
          mainSession.PostEvent(telemetryEvent);
        }
      }
      this.counter = 0L;
      this.allowedListCounter = 0L;
      this.bucketStartTime = timeToReset;
      if (newManifest != null)
      {
        if (newManifest.ThrottlingThreshold > 0L)
          this.threshold = newManifest.ThrottlingThreshold;
        if (newManifest.ThrottlingTimerReset > 0.0)
          this.resetCounter = newManifest.ThrottlingTimerReset;
      }
      this.droppedEvents.Clear();
      this.noisyAllowedListEvents.Clear();
    }
  }
}
