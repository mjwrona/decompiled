// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.OrchestrationContextExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.VisualStudio.Services.Orchestration;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal static class OrchestrationContextExtensions
  {
    public static void TraceStartLinearOrchestration(
      this OrchestrationContext context,
      string instanceId = null)
    {
      if (context.IsReplaying)
        return;
      context.Tracer?.TraceStarted(instanceId ?? context.OrchestrationInstance.InstanceId, (string) null);
    }

    public static void TraceStartLinearPhase(
      this OrchestrationContext context,
      string owningTeamName,
      string action,
      string instanceId = null)
    {
      if (context.IsReplaying)
        return;
      context.Tracer?.TracePhaseStarted(instanceId ?? context.OrchestrationInstance.InstanceId, owningTeamName, action);
    }

    public static void TraceCompleteLinearOrchestration(
      this OrchestrationContext context,
      string owningTeamName,
      string action,
      string instanceId = null)
    {
      if (context.IsReplaying)
        return;
      context.Tracer?.TraceCompleted(instanceId ?? context.OrchestrationInstance.InstanceId, owningTeamName, action);
    }

    public static void TraceCompleteLinearOrchestrationWithError(
      this OrchestrationContext context,
      string owningTeamName,
      string action,
      string errorCode,
      string errorMessage,
      bool errorIsExpected)
    {
      if (context.IsReplaying)
        return;
      context.Tracer?.TraceCompletedWithError(context.OrchestrationInstance.InstanceId, owningTeamName, action, errorCode, errorMessage, errorIsExpected);
    }
  }
}
