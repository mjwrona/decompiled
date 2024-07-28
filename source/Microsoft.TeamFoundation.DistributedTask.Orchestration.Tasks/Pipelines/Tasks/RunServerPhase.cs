// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunServerPhase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.VisualStudio.Services.Orchestration;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public sealed class RunServerPhase : RunPhase
  {
    protected override Task<JobExecutionState> RunJob(
      OrchestrationContext context,
      PhaseExecutionState phase,
      JobExecutionState job,
      string jobInstanceId,
      RunPhaseInput input)
    {
      RunServerJobInput input1 = new RunServerJobInput()
      {
        ActivityDispatcherShardsCount = input.ActivityDispatcherShardsCount,
        ShardKey = input.ShardKey,
        ScopeId = input.ScopeId,
        PlanId = input.PlanId,
        PlanVersion = input.PlanVersion,
        StageName = input.StageName,
        StageAttempt = input.StageAttempt,
        PhaseName = input.Phase.Name,
        PhaseAttempt = input.Phase.Attempt,
        Job = job
      };
      return input.PlanVersion < 2 ? context.CreateSubOrchestrationInstance<JobExecutionState>("RunPipelineServerJob", "1.0", jobInstanceId, (object) input1) : context.CreateSubOrchestrationInstance<JobExecutionState>("RunPipelineServerJob", "2.0", jobInstanceId, (object) input1);
    }
  }
}
