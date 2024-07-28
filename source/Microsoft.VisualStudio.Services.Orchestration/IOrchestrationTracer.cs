// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.IOrchestrationTracer
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public interface IOrchestrationTracer
  {
    void TraceStarted(string orchestrationId, string orchestrationName);

    void TracePhaseStarted(string orchestrationId, string orchestrationName, string action);

    void TraceCompleted(string orchestrationId, string orchestrationName, string action);

    void TraceCompletedWithError(
      string orchestrationId,
      string orchestrationName,
      string action,
      string errorCode,
      string errorMessage,
      bool errorIsExpected);

    void Trace(
      string orchestrationId,
      int eventId,
      TraceLevel level,
      string format,
      params object[] arguments);

    void Trace(
      string orchestrationId,
      int eventId,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] arguments);
  }
}
