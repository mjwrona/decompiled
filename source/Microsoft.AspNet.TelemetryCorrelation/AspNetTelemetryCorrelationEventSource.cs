// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.TelemetryCorrelation.AspNetTelemetryCorrelationEventSource
// Assembly: Microsoft.AspNet.TelemetryCorrelation, Version=1.0.8.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7ACB3991-3C84-47CC-A6F7-137F032D1A74
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.TelemetryCorrelation.dll

using System.Diagnostics.Tracing;

namespace Microsoft.AspNet.TelemetryCorrelation
{
  [EventSource(Name = "Microsoft-AspNet-Telemetry-Correlation", Guid = "ace2021e-e82c-5502-d81d-657f27612673")]
  internal sealed class AspNetTelemetryCorrelationEventSource : EventSource
  {
    public static readonly AspNetTelemetryCorrelationEventSource Log = new AspNetTelemetryCorrelationEventSource();

    [Event(1, Message = "Callback='{0}'", Level = EventLevel.Verbose)]
    public void TraceCallback(string callback) => this.WriteEvent(1, callback);

    [Event(2, Message = "Activity started, Id='{0}'", Level = EventLevel.Verbose)]
    public void ActivityStarted(string id) => this.WriteEvent(2, id);

    [Event(3, Message = "Activity stopped, Id='{0}', Name='{1}'", Level = EventLevel.Verbose)]
    public void ActivityStopped(string id, string eventName) => this.WriteEvent(3, id, eventName);

    [Event(4, Message = "Failed to parse header '{0}', value: '{1}'", Level = EventLevel.Informational)]
    public void HeaderParsingError(string headerName, string headerValue) => this.WriteEvent(4, headerName, headerValue);

    [Event(5, Message = "Failed to extract activity, reason '{0}'", Level = EventLevel.Error)]
    public void ActvityExtractionError(string reason) => this.WriteEvent(5, reason);

    [Event(6, Message = "Finished Activity is detected on the stack, Id: '{0}', Name: '{1}'", Level = EventLevel.Error)]
    public void FinishedActivityIsDetected(string id, string name) => this.WriteEvent(6, id, name);

    [Event(7, Message = "System.Diagnostics.Activity stack is too deep. This is a code authoring error, Activity will not be stopped.", Level = EventLevel.Error)]
    public void ActivityStackIsTooDeepError() => this.WriteEvent(7);

    [Event(8, Message = "Activity restored, Id='{0}'", Level = EventLevel.Informational)]
    public void ActivityRestored(string id) => this.WriteEvent(8, id);

    [Event(9, Message = "Failed to invoke OnExecuteRequestStep, Error='{0}'", Level = EventLevel.Error)]
    public void OnExecuteRequestStepInvokationError(string error) => this.WriteEvent(9, error);

    [Event(10, Message = "System.Diagnostics.Activity stack is too deep. Current Id: '{0}', Name: '{1}'", Level = EventLevel.Warning)]
    public void ActivityStackIsTooDeepDetails(string id, string name) => this.WriteEvent(10, id, name);
  }
}
