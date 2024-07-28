// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IInternalDistributedTaskResourceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (DistributedTaskResourceService))]
  internal interface IInternalDistributedTaskResourceService : 
    IDistributedTaskResourceService,
    IVssFrameworkService
  {
    Task<int> CleanupUnassignableRequestsAsync(IVssRequestContext requestContext);

    int GetUnassignedRequestTimeoutBatchSize();

    Task ClearAgentSlotAsync(IVssRequestContext requestContext, int poolId, int agentId);

    Task NotifyAgentReadyAsync(IVssRequestContext requestContext, int poolId, long requestId);

    Microsoft.VisualStudio.Services.Identity.Identity ProvisionServiceIdentity(
      IVssRequestContext poolRequestContext,
      TaskAgentPoolData poolData,
      AgentPoolServiceAccountRoles role);

    void QueueRequestAssignmentJob(IVssRequestContext requestContext);

    TaskAgentPool UpdateAgentPool(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPool pool,
      bool removePoolMetadata = false,
      Stream poolMetadataStream = null,
      bool removeAgentCloudId = false);

    IAgentPoolSecurityProvider GetAgentPoolSecurity(IVssRequestContext requestContext, int poolId);
  }
}
