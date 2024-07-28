// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Tracing.OrchestrationTraceSource
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace Microsoft.VisualStudio.Services.Orchestration.Tracing
{
  internal sealed class OrchestrationTraceSource : TraceSource
  {
    public OrchestrationTraceSource(SourceLevels sourceLevels)
      : base("Microsoft-VisualStudio-Orchestration-Runtime", sourceLevels)
    {
    }

    internal void Trace(EventLevel level, string message)
    {
      switch (level)
      {
        case EventLevel.Critical:
          this.Critical(message);
          break;
        case EventLevel.Error:
          this.Error(message);
          break;
        case EventLevel.Warning:
          this.Warning(message);
          break;
        case EventLevel.Informational:
          this.Information(message);
          break;
        case EventLevel.Verbose:
          this.Verbose(message);
          break;
      }
    }

    internal void TraceSession(EventLevel level, string sessionId, string message)
    {
      switch (level)
      {
        case EventLevel.Critical:
          this.SessionCritical(sessionId, message);
          break;
        case EventLevel.Error:
          this.SessionError(sessionId, message);
          break;
        case EventLevel.Warning:
          this.SessionWarning(sessionId, message);
          break;
        case EventLevel.Informational:
          this.SessionInformation(sessionId, message);
          break;
        case EventLevel.Verbose:
          this.SessionVerbose(sessionId, message);
          break;
      }
    }

    internal void TraceInstance(
      EventLevel level,
      string instanceId,
      string executionId,
      string message)
    {
      switch (level)
      {
        case EventLevel.Critical:
          this.InstanceCritical(instanceId, executionId, message);
          break;
        case EventLevel.Error:
          this.InstanceError(instanceId, executionId, message);
          break;
        case EventLevel.Warning:
          this.InstanceWarning(instanceId, executionId, message);
          break;
        case EventLevel.Informational:
          this.InstanceInformation(instanceId, executionId, message);
          break;
        case EventLevel.Verbose:
          this.InstanceVerbose(instanceId, executionId, message);
          break;
      }
    }

    internal void Critical(string message) => this.TraceData(TraceEventType.Critical, 1, (object) message);

    internal void Error(string message) => this.TraceData(TraceEventType.Error, 2, (object) message);

    internal void Warning(string message) => this.TraceData(TraceEventType.Warning, 3, (object) message);

    internal void Information(string message) => this.TraceData(TraceEventType.Information, 4, (object) message);

    internal void Verbose(string message) => this.TraceData(TraceEventType.Verbose, 5, (object) message);

    internal void SessionCritical(string sessionId, string message) => this.TraceData(TraceEventType.Critical, 6, (object) sessionId, (object) message);

    internal void SessionError(string sessionId, string message) => this.TraceData(TraceEventType.Error, 7, (object) sessionId, (object) message);

    internal void SessionWarning(string sessionId, string message) => this.TraceData(TraceEventType.Warning, 8, (object) sessionId, (object) message);

    internal void SessionInformation(string sessionId, string message) => this.TraceData(TraceEventType.Information, 9, (object) sessionId, (object) message);

    internal void SessionVerbose(string sessionId, string message) => this.TraceData(TraceEventType.Verbose, 10, (object) sessionId, (object) message);

    internal void InstanceCritical(string instanceId, string executionId, string message) => this.TraceData(TraceEventType.Critical, 11, (object) instanceId, (object) executionId, (object) message);

    internal void InstanceError(string instanceId, string executionId, string message) => this.TraceData(TraceEventType.Error, 12, (object) instanceId, (object) executionId, (object) message);

    internal void InstanceWarning(string instanceId, string executionId, string message) => this.TraceData(TraceEventType.Warning, 13, (object) instanceId, (object) executionId, (object) message);

    internal void InstanceInformation(string instanceId, string executionId, string message) => this.TraceData(TraceEventType.Information, 14, (object) instanceId, (object) executionId, (object) message);

    internal void InstanceVerbose(string instanceId, string executionId, string message) => this.TraceData(TraceEventType.Verbose, 15, (object) instanceId, (object) executionId, (object) message);
  }
}
