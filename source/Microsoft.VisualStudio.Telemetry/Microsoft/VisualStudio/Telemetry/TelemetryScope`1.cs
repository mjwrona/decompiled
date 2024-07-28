// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryScope`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  public sealed class TelemetryScope<T> where T : OperationEvent
  {
    private const int ScopeIsEnded = 1;
    private const int ScopeIsNotEnded = 0;
    private int isEnded;

    private TelemetrySession TelemetrySession { get; set; }

    private DateTime StartTime { get; set; }

    public bool IsEnd => this.isEnded == 1;

    public T EndEvent { get; }

    public TelemetryEventCorrelation Correlation => this.EndEvent.Correlation;

    internal TelemetryScope(
      TelemetrySession telemetrySession,
      string eventName,
      TelemetryScope<T>.CreateNewEvent createNewEvent,
      TelemetryScopeSettings settings)
    {
      this.isEnded = 0;
      this.TelemetrySession = telemetrySession;
      Guid.NewGuid();
      this.StartTime = DateTime.UtcNow;
      T obj = createNewEvent(OperationStageType.Start);
      obj.Severity = settings.Severity;
      obj.Correlate(settings.Correlations);
      obj.IsOptOutFriendly = settings.IsOptOutFriendly;
      if (settings.StartEventProperties != null)
        obj.Properties.AddRange<string, object>(settings.StartEventProperties);
      if (settings.PostStartEvent)
        this.TelemetrySession.PostEvent((TelemetryEvent) obj);
      this.EndEvent = obj;
      this.EndEvent.SetPostStartEventProperty(settings.PostStartEvent);
      this.EndEvent.StageType = OperationStageType.End;
    }

    public void End(TelemetryResult result, string resultSummary = null)
    {
      if (Interlocked.CompareExchange(ref this.isEnded, 1, 0) == 1)
        throw new InvalidOperationException("The scoped user task is already ended.");
      this.EndEvent.SetResultProperties(result, resultSummary);
      this.EndEvent.SetTimeProperties(this.StartTime, DateTime.UtcNow, (DateTime.UtcNow - this.StartTime).TotalMilliseconds);
      this.TelemetrySession.PostEvent((TelemetryEvent) this.EndEvent);
    }

    internal delegate T CreateNewEvent(OperationStageType stageType) where T : OperationEvent;
  }
}
