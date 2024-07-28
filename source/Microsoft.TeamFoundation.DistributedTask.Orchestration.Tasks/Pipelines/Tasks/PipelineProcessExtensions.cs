// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.PipelineProcessExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal static class PipelineProcessExtensions
  {
    public static PipelineExecutionState Compile(
      this PipelineProcess process,
      ResourceStore resourceStore = null,
      TaskStore taskStore = null)
    {
      PipelineExecutionState pipelineExecutionState = new PipelineExecutionState();
      foreach (Stage stage in (IEnumerable<Stage>) process.Stages)
        pipelineExecutionState.Stages.Add(stage.Compile(resourceStore, taskStore));
      return pipelineExecutionState;
    }

    public static StageExecutionState Compile(
      this Stage stage,
      ResourceStore resourceStore = null,
      TaskStore taskStore = null)
    {
      StageExecutionState stageState = new StageExecutionState(stage);
      foreach (PhaseNode phase1 in (IEnumerable<PhaseNode>) stage.Phases)
      {
        PhaseNode phaseNode = phase1;
        PhaseExecutionState phaseState = stageState.Phases.First<PhaseExecutionState>((Func<PhaseExecutionState, bool>) (x => string.Equals(x.Name, phaseNode.Name, StringComparison.OrdinalIgnoreCase)));
        PhaseExecutionContext executionContext = PipelineProcessExtensions.CreatePhaseExecutionContext(stageState, phaseState, resourceStore, taskStore);
        if (phaseNode is Phase phase2)
        {
          foreach (JobInstance job in (IEnumerable<JobInstance>) phase2.Expand(executionContext).Jobs)
            phaseState.Jobs.Add(new JobExecutionState(job));
        }
      }
      return stageState;
    }

    private static PhaseExecutionContext CreatePhaseExecutionContext(
      StageExecutionState stageState,
      PhaseExecutionState phaseState,
      ResourceStore resourceStore,
      TaskStore taskStore)
    {
      return new PhaseExecutionContext((StageInstance) stageState.Name, (PhaseInstance) phaseState.Name, (IDictionary<string, PhaseInstance>) null, (IDictionary<string, IDictionary<string, PhaseInstance>>) null, PipelineState.NotStarted, (ICounterStore) new CounterStore(), (IPackageStore) new PackageStore((IEnumerable<PackageMetadata>) null, (IPackageResolver) null), (IResourceStore) (resourceStore ?? new ResourceStore()), (ITaskStore) (taskStore ?? new TaskStore(Array.Empty<TaskDefinition>())), (IList<IStepProvider>) null, (IPipelineIdGenerator) null, (IPipelineTraceWriter) null, (EvaluationOptions) null, (ExecutionOptions) null);
    }
  }
}
