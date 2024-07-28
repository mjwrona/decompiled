// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.IAgentPoolManager
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public interface IAgentPoolManager
  {
    Task CancelJob(
      int poolId,
      long requestId,
      TaskAgentReference agent,
      Guid scopeId,
      Guid planId,
      Guid jobId,
      TimeSpan timeout);

    Task DeleteRequest(
      int poolId,
      long requestId,
      Guid scopeId,
      Guid planId,
      Guid jobId,
      TaskResult result,
      TaskAgentReference agentRef = null,
      bool agentShuttingDown = false);

    Task<AgentRequestData> GetRequest(int poolId, long requestId);

    Task<TaskResult?> GetJobResult(
      int poolId,
      long requestId,
      Guid scopeId,
      Guid planId,
      Guid jobId);

    Task<AgentRequestData> QueueRequest(
      int queueId,
      Guid scopeId,
      Guid planId,
      JobParameters parameters,
      JObject agentSpecification = null);

    Task<AgentRequestData> QueueRequestToPool(
      int poolId,
      Guid scopeId,
      Guid planId,
      JobParameters parameters,
      JObject agentSpecification = null);

    Task StartJob(
      int poolId,
      long requestId,
      TaskAgentReference agent,
      Guid scopeId,
      Guid planId,
      JobParameters parameters);

    Task SendMetadataUpdate(int poolId, long requestId, JobMetadataMessage jobMetadataMessage);
  }
}
