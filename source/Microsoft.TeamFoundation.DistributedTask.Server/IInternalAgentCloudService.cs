// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IInternalAgentCloudService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (AgentCloudService))]
  internal interface IInternalAgentCloudService : IAgentCloudService, IVssFrameworkService
  {
    bool DeliverEvent(
      IVssRequestContext requestContext,
      RunAgentEvent agentEvent,
      bool createOrchestration = false);

    Task<bool> DeliverEventAsync(
      IVssRequestContext requestContext,
      RunAgentEvent agentEvent,
      bool createOrchestration = false);

    OrchestrationInstance EnsureRunAgentOrchestrationExists(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid agentCloudRequestId,
      int poolId,
      int agentId,
      out bool createdInstance);

    Task<IList<TaskAgentCloudRequest>> GetActiveAgentCloudRequestsAsync(
      IVssRequestContext requestContext);

    TaskAgentCloud GetAgentCloudInternal(
      IVssRequestContext requestContext,
      int agentCloudId,
      bool includeSharedSecret = false,
      bool includeType = false);

    Task<TaskAgentCloud> GetAgentCloudInternalAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      bool includeSharedSecret = false,
      bool updateCache = true);

    IServerPoolProvider GetPrivatePoolProvider();

    IServerPoolProvider GetPoolProviderForAgentCloud(
      IVssRequestContext requestContext,
      TaskAgentCloud agentCloud,
      TaskAgentPoolData agentPool);

    IServerPoolProvider GetPoolProviderForPool(
      IVssRequestContext requestContext,
      TaskAgentPoolData agentPool);

    Task<IServerPoolProvider> GetPoolProviderForPoolAsync(
      IVssRequestContext requestContext,
      TaskAgentPoolData agentPool);

    Task<IServerPoolProvider> GetPoolProviderForPoolAsync(
      IVssRequestContext requestContext,
      TaskAgentPoolData agentPool,
      bool includeSharedSecret,
      bool updateCache = true);

    void SetServiceIdentityPermissions(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity);

    bool HasAgentCloudListenPermission(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid requestId);

    Task<TaskAgentCloud> UpdateAgentCloudAsync(
      IVssRequestContext requestContext,
      TaskAgentCloud toUpdate);

    Task<TaskAgentCloudRequest> UpdateAgentCloudRequestAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest toUpdate);
  }
}
