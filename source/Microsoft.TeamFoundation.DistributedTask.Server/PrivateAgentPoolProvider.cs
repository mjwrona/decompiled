// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PrivateAgentPoolProvider
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
  internal class PrivateAgentPoolProvider : IServerPoolProvider, ICloneable
  {
    internal PrivateAgentPoolProvider()
    {
    }

    private PrivateAgentPoolProvider(PrivateAgentPoolProvider toClone)
    {
      this.InitialLeaseTimeout = toClone.InitialLeaseTimeout;
      this.ProvisioningLeaseTimeout = toClone.ProvisioningLeaseTimeout;
      this.LeaseRenewalTimeout = toClone.LeaseRenewalTimeout;
      this.RequestSendTimeout = toClone.RequestSendTimeout;
    }

    public TimeSpan InitialLeaseTimeout { get; set; }

    public TimeSpan ProvisioningLeaseTimeout { get; set; }

    public TimeSpan LeaseRenewalTimeout { get; set; }

    public TimeSpan RequestSendTimeout { get; set; }

    public bool SingleJobPerRequest
    {
      get => false;
      set
      {
      }
    }

    public int? AgentCloudId => new int?();

    public object Clone() => (object) new PrivateAgentPoolProvider(this);

    public Task BillForResourcesAsync(
      IVssRequestContext requestContext,
      TaskAgentJobRequest jobRequest)
    {
      return Task.CompletedTask;
    }

    public Task DeprovisionAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentReference agent)
    {
      return Task.CompletedTask;
    }

    public Task<AccountParallelismResponse> GetAccountParallelismAsync(
      IVssRequestContext requestContext)
    {
      return Task.FromResult<AccountParallelismResponse>((AccountParallelismResponse) null);
    }

    public Task<IList<AgentDefinition>> GetAgentDefinitionsAsync(IVssRequestContext requestContext) => Task.FromResult<IList<AgentDefinition>>((IList<AgentDefinition>) new List<AgentDefinition>());

    public Task<string> GetAgentRequestStatusAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest agentCloudRequest)
    {
      return Task.FromResult<string>(TaskResources.WaitingOnAgentToStartRequest());
    }

    public Task ProvisionAgentAsync(
      IVssRequestContext requestContext,
      TaskAgentJobRequest jobRequest)
    {
      return Task.CompletedTask;
    }
  }
}
