// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.PhaseTargetExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal static class PhaseTargetExtensions
  {
    public static PhaseTarget TrimForExecution(this PhaseTarget target)
    {
      if (target == null)
        return (PhaseTarget) null;
      PhaseTarget phaseTarget = target.Clone();
      phaseTarget.ContinueOnError = (ExpressionValue<bool>) null;
      phaseTarget.TimeoutInMinutes = (ExpressionValue<int>) null;
      if (target.Type == PhaseTargetType.Queue)
      {
        AgentQueueTarget agentQueueTarget = phaseTarget as AgentQueueTarget;
        agentQueueTarget.Execution = (ParallelExecutionOptions) null;
        agentQueueTarget.Demands?.Clear();
        agentQueueTarget.Workspace = (WorkspaceOptions) null;
        agentQueueTarget.Container = (ExpressionValue<string>) null;
        agentQueueTarget.SidecarContainers?.Clear();
      }
      return phaseTarget;
    }
  }
}
