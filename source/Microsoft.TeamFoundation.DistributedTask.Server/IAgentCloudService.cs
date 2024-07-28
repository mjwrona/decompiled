// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IAgentCloudService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (AgentCloudService))]
  public interface IAgentCloudService : IVssFrameworkService
  {
    Task<TaskAgentCloud> AddAgentCloudAsync(
      IVssRequestContext requestContext,
      TaskAgentCloud agentCloud);

    Task AddAgentCloudRequestMessageAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid agentCloudRequestId,
      AgentRequestMessage requestMessage);

    Task<TaskAgentCloudRequest> CreateAgentCloudRequestAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest request);

    Task<TaskAgentCloud> DeleteAgentCloudAsync(IVssRequestContext requestContext, int agentCloudId);

    TaskAgentCloud GetAgentCloud(IVssRequestContext requestContext, int agentCloudId);

    Task<TaskAgentCloud> GetAgentCloudAsync(IVssRequestContext requestContext, int agentCloudId);

    Task<IList<TaskAgentCloud>> GetAgentCloudsAsync(IVssRequestContext requestContext);

    Task<TaskAgentCloudRequest> GetAgentCloudRequestAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid requestId);

    Task<IList<TaskAgentCloudRequest>> GetAgentCloudRequestsAsync(
      IVssRequestContext requestContext,
      int agentCloudId);

    Task<TaskAgentCloudRequest> GetAgentCloudRequestForAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      int agentId);

    Task<string> GetAgentCloudRequestStatusAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest request);

    Task<IList<AgentDefinition>> GetAgentDefinitionsAsync(
      IVssRequestContext requestContext,
      int agentCloudId);

    Task<TaskAgentCloud> UpdateAgentCloudAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      TaskAgentCloud toUpdate);

    Task UpdateAgentCloudRequestAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid agentCloudRequestId,
      AgentRequestProvisioningResult provisioningResult);

    Task<AgentRequestJob> GetTaskAgentCloudRequestJobAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid agentCloudRequestId);
  }
}
