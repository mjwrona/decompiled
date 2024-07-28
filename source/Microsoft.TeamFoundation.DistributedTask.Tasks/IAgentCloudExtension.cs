// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.IAgentCloudExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  public interface IAgentCloudExtension
  {
    Task AddRequestMessageAsync(
      int agentCloudId,
      Guid requestId,
      string message,
      AgentRequestMessageVerbosity verbosity);

    Task<TaskAgentCloud> GetAgentCloudAsync(int agentCloudId);

    Task<TaskAgentCloudRequest> GetAgentCloudRequestAsync(int agentCloudId, Guid requestId);

    Task<TaskAgentCloudRequest> SetAgentCloudRequestProvisionedAsync(
      int agentCloudId,
      Guid requestId,
      DateTime? provisionedTime);

    Task<TaskAgentCloudRequest> SetAgentCloudRequestProvisionSentAsync(
      int agentCloudId,
      Guid requestId,
      DateTime sentTime,
      JObject agentData);

    Task<TaskAgentCloudRequest> SetAgentCloudRequestAgentConnectedAsync(
      int agentCloudId,
      Guid requestId,
      DateTime connectionTime);

    Task<TaskAgentCloudRequest> SetAgentCloudRequestDeprovisionedAsync(
      int agentCloudId,
      Guid requestId,
      DateTime sentTime);
  }
}
