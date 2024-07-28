// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTargetExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class AgentPoolQueueTargetExtensions
  {
    public static Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseTarget ToPipelinePhaseTarget(
      this AgentPoolQueueTarget target,
      IVssRequestContext requestContext,
      IDictionary<string, VariableValue> environment,
      IReadOnlyList<Demand> demands,
      int executionTimeout,
      int cancelTimeout,
      bool isGatedTrigger,
      string workspaceCleanOption)
    {
      if (target == null)
        return (Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseTarget) null;
      ParallelExecutionOptions executionOptions1 = (ParallelExecutionOptions) null;
      IVariableMultiplierExecutionOptions multiplierOptions = target.GetMultiplierOptions();
      MultipleAgentExecutionOptions executionOptions2 = target.ExecutionOptions as MultipleAgentExecutionOptions;
      if (multiplierOptions != null)
        executionOptions1 = new ParallelExecutionOptions()
        {
          MaxConcurrency = (ExpressionValue<int>) multiplierOptions.MaxConcurrency,
          Matrix = (ExpressionValue<IDictionary<string, IDictionary<string, string>>>) (IDictionary<string, IDictionary<string, string>>) multiplierOptions.GetMatrix(environment, isGatedTrigger)
        };
      else if (executionOptions2 != null)
        executionOptions1 = new ParallelExecutionOptions()
        {
          MaxConcurrency = (ExpressionValue<int>) executionOptions2.MaxConcurrency
        };
      AgentQueueReference agentQueueReference = new AgentQueueReference();
      if (target.Queue != null)
        agentQueueReference.Id = target.Queue.Id;
      AgentQueueTarget agentQueueTarget = new AgentQueueTarget();
      agentQueueTarget.CancelTimeoutInMinutes = (ExpressionValue<int>) cancelTimeout;
      agentQueueTarget.Queue = agentQueueReference;
      agentQueueTarget.TimeoutInMinutes = (ExpressionValue<int>) executionTimeout;
      agentQueueTarget.Execution = executionOptions1;
      AgentQueueTarget pipelinePhaseTarget = agentQueueTarget;
      if (target.AgentSpecification != null)
        pipelinePhaseTarget.AgentSpecification = target.AgentSpecification.ToJObject();
      pipelinePhaseTarget.Demands.AddRange<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand, ISet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>>(target.Demands.ToDistributedTaskDemands());
      if (demands != null)
        pipelinePhaseTarget.Demands.AddRange<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand, ISet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>>(demands.ToDistributedTaskDemands());
      if (!string.IsNullOrEmpty(workspaceCleanOption))
        pipelinePhaseTarget.Workspace = new Microsoft.TeamFoundation.DistributedTask.Pipelines.WorkspaceOptions()
        {
          Clean = workspaceCleanOption
        };
      return (Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseTarget) pipelinePhaseTarget;
    }
  }
}
