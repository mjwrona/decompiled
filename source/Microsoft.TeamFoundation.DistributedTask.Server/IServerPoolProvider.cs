// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IServerPoolProvider
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
  internal interface IServerPoolProvider : ICloneable
  {
    TimeSpan InitialLeaseTimeout { get; set; }

    TimeSpan ProvisioningLeaseTimeout { get; set; }

    TimeSpan LeaseRenewalTimeout { get; set; }

    TimeSpan RequestSendTimeout { get; set; }

    bool SingleJobPerRequest { get; set; }

    int? AgentCloudId { get; }

    Task BillForResourcesAsync(IVssRequestContext requestContext, TaskAgentJobRequest jobRequest);

    Task DeprovisionAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentReference agent);

    Task<AccountParallelismResponse> GetAccountParallelismAsync(IVssRequestContext requestContext);

    Task<IList<AgentDefinition>> GetAgentDefinitionsAsync(IVssRequestContext requestContext);

    Task<string> GetAgentRequestStatusAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest agentCloudRequest);

    Task ProvisionAgentAsync(IVssRequestContext requestContext, TaskAgentJobRequest jobRequest);
  }
}
