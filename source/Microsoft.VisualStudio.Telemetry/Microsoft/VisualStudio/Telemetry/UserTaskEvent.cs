// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.UserTaskEvent
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.Telemetry
{
  public sealed class UserTaskEvent : OperationEvent
  {
    public UserTaskEvent(string eventName, TelemetryResult result, string resultSummary = null)
      : this(eventName, OperationStageType.Atomic, result, resultSummary)
    {
    }

    internal UserTaskEvent(
      string eventName,
      OperationStageType stageType,
      TelemetryResult result,
      string resultSummary = null)
      : base(eventName, DataModelEventType.UserTask, stageType, result, resultSummary)
    {
      this.Severity = TelemetrySeverity.High;
    }
  }
}
