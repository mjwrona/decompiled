// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.IAgentPoolExtension7
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public interface IAgentPoolExtension7
  {
    Task CancelJob(
      int poolId,
      long requestId,
      TaskAgentReference agent,
      Guid scopeId,
      Guid planId,
      Guid jobId,
      TimeSpan timeout);

    Task DeleteAgentRequest(
      int poolId,
      long requestId,
      Guid scopeId,
      Guid planId,
      Guid jobId,
      TaskResult result);

    Task<TaskAgentJobRequest> GetAgentRequest(int poolId, long requestId);

    Task<TaskResult?> GetJobResult(
      int poolId,
      long requestId,
      Guid scopeId,
      Guid planId,
      Guid jobId);

    Task<TaskAgentJobRequest> QueueAgentRequest(
      int poolId,
      IList<int> agentIds,
      IList<Demand> demands,
      Guid scopeId,
      Guid planId,
      Guid jobId);

    Task<IList<TaskAgent>> GetAgents(int poolId, IList<Demand> demands);

    Task<IList<TaskAgent>> GetAgentsForDeploymentGroup(
      Guid projectId,
      int deploymentGroupId,
      IList<string> tagFilters);

    Task<IList<int>> GetAgentIds(int poolId, IList<Demand> demands);

    Task<IList<TaskAgentReference>> GetTaskAgentReferences(
      Guid projectId,
      int deploymentGroupId,
      IList<string> tagFilters);

    Task<IList<DeploymentMachine>> GetDeploymentMachines(
      Guid projectId,
      int machineGroupId,
      IList<string> tagFilters);

    Task<IList<TaskAgentJobRequest>> GetAgentRequestsForAgents(
      int poolId,
      IList<int> agentIds,
      int completedRequests);

    Task StartJob(
      int poolId,
      long requestId,
      TaskAgentReference agent,
      Guid scopeId,
      Guid planId,
      Guid jobId,
      TaskOrchestrationJobAttempt jobAttempt);
  }
}
