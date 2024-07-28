// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.RemoteAgentPoolProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Client;
using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class RemoteAgentPoolProvider : IServerPoolProvider, ICloneable
  {
    protected TaskAgentCloud m_agentCloudSettings;
    protected TaskAgentPoolData m_agentPool;
    private static readonly RegistryQuery s_requestSendTimeoutQuery = (RegistryQuery) "/Service/DistributedTask/PoolProviderRequestSendTimeout";

    internal RemoteAgentPoolProvider(TaskAgentCloud agentCloud, TaskAgentPoolData pool)
    {
      this.m_agentCloudSettings = agentCloud;
      this.m_agentPool = pool;
    }

    private RemoteAgentPoolProvider(RemoteAgentPoolProvider toClone)
    {
      this.InitialLeaseTimeout = toClone.InitialLeaseTimeout;
      this.ProvisioningLeaseTimeout = toClone.ProvisioningLeaseTimeout;
      this.LeaseRenewalTimeout = toClone.LeaseRenewalTimeout;
      this.RequestSendTimeout = toClone.RequestSendTimeout;
      this.m_agentCloudSettings = toClone.m_agentCloudSettings.Clone();
      this.m_agentPool = this.m_agentPool.Clone();
    }

    public TimeSpan InitialLeaseTimeout
    {
      get => TimeSpan.FromMinutes(15.0);
      set
      {
      }
    }

    public TimeSpan ProvisioningLeaseTimeout
    {
      get => TimeSpan.FromMinutes(45.0);
      set
      {
      }
    }

    public TimeSpan LeaseRenewalTimeout
    {
      get => TimeSpan.FromMinutes(10.0);
      set
      {
      }
    }

    public TimeSpan RequestSendTimeout
    {
      get => TimeSpan.FromSeconds(30.0);
      set
      {
      }
    }

    public bool SingleJobPerRequest
    {
      get => true;
      set
      {
      }
    }

    public int? AgentCloudId => new int?(this.m_agentCloudSettings.AgentCloudId);

    public virtual Task BillForResourcesAsync(
      IVssRequestContext requestContext,
      TaskAgentJobRequest jobRequest)
    {
      return Task.CompletedTask;
    }

    public async Task DeprovisionAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentReference agent)
    {
      IInternalAgentCloudService agentCloudService = requestContext.GetService<IInternalAgentCloudService>();
      TaskAgentCloudRequest agentCloudRequest = await agentCloudService.GetAgentCloudRequestForAgentAsync(requestContext, poolId, agent.Id);
      if (agentCloudRequest == null)
      {
        agentCloudService = (IInternalAgentCloudService) null;
        agentCloudRequest = (TaskAgentCloudRequest) null;
      }
      else
      {
        AgentIdentifier identifier = new AgentIdentifier()
        {
          AgentId = agentCloudRequest.RequestId,
          AccountId = requestContext.ServiceHost.InstanceId,
          AgentCloudId = this.m_agentCloudSettings.Id,
          AgentPool = this.m_agentPool.Pool.Name,
          AgentData = agentCloudRequest.AgentData
        };
        try
        {
          using (RemoteProviderHttpClient client = await this.m_agentCloudSettings.GetPoolProviderClient(requestContext, this.GetPoolProviderConfigurationForRequest(requestContext, agentCloudRequest)))
          {
            using (requestContext.CreateAsyncTimeOutScope(this.GetRequestSendTimeout(requestContext)))
            {
              try
              {
                await client.ReleaseAgentAsync(identifier, requestContext.CancellationToken);
              }
              catch (InvalidResponseCodeException ex)
              {
                throw new TaskAgentCloudCommunicationException(TaskResources.AgentCloudResponseStatusCodeInvalid((object) this.m_agentCloudSettings.Name, (object) ex.StatusCode));
              }
            }
          }
        }
        finally
        {
          requestContext.TraceAlways(10015163, TraceLevel.Info, "DistributedTask", "PoolProvider", "Sent Release Agent request to {0}", (object) this.m_agentCloudSettings.ReleaseAgentEndpoint);
          agentCloudRequest.ReleaseRequestTime = new DateTime?(DateTime.UtcNow);
          TaskAgentCloudRequest agentCloudRequest1 = await agentCloudService.UpdateAgentCloudRequestAsync(requestContext, agentCloudRequest);
        }
        identifier = (AgentIdentifier) null;
        agentCloudService = (IInternalAgentCloudService) null;
        agentCloudRequest = (TaskAgentCloudRequest) null;
      }
    }

    public async Task<AccountParallelismResponse> GetAccountParallelismAsync(
      IVssRequestContext requestContext)
    {
      AccountParallelismResponse parallelismAsync = (AccountParallelismResponse) null;
      if (this.m_agentCloudSettings.GetAccountParallelismEndpoint != null)
      {
        AccountParallelismRequest parallelismRequest = new AccountParallelismRequest()
        {
          AccountId = requestContext.ServiceHost.InstanceId,
          AgentCloudId = this.m_agentCloudSettings.Id
        };
        using (RemoteProviderHttpClient client = await this.m_agentCloudSettings.GetPoolProviderClient(requestContext, this.GetPoolProviderConfigurationForRequest(requestContext)))
        {
          try
          {
            parallelismAsync = await client.GetAccountParallelismAsync(parallelismRequest, requestContext.CancellationToken);
          }
          catch (InvalidResponseCodeException ex)
          {
            throw new TaskAgentCloudCommunicationException(TaskResources.AgentCloudResponseStatusCodeInvalid((object) this.m_agentCloudSettings.Name, (object) ex.StatusCode));
          }
          finally
          {
            requestContext.TraceAlways(10015163, TraceLevel.Info, "DistributedTask", "PoolProvider", "Sent Get Account Parallelism request to {0}", (object) this.m_agentCloudSettings.GetAccountParallelismEndpoint);
          }
        }
        parallelismRequest = (AccountParallelismRequest) null;
      }
      else
        requestContext.TraceAlways(10015163, TraceLevel.Info, "DistributedTask", "PoolProvider", "Cannot update account parallelism for agentcloud {0}, since GetAccountParallelismEndpoint is null", (object) this.m_agentCloudSettings.Name);
      return parallelismAsync;
    }

    public virtual async Task<IList<AgentDefinition>> GetAgentDefinitionsAsync(
      IVssRequestContext requestContext)
    {
      IList<AgentDefinition> agentDefinitionList = (IList<AgentDefinition>) null;
      IList<AgentDefinition> definitionsAsync;
      using (RemoteProviderHttpClient client = await this.m_agentCloudSettings.GetPoolProviderClient(requestContext, this.GetPoolProviderConfigurationForRequest(requestContext)))
      {
        using (requestContext.CreateAsyncTimeOutScope(this.GetRequestSendTimeout(requestContext)))
        {
          try
          {
            agentDefinitionList = await client.GetAgentDefinitionsAsync(requestContext.CancellationToken);
          }
          catch (InvalidResponseCodeException ex)
          {
            throw new TaskAgentCloudCommunicationException(TaskResources.AgentCloudResponseStatusCodeInvalid((object) this.m_agentCloudSettings.Name, (object) ex.StatusCode));
          }
          finally
          {
            requestContext.TraceAlways(10015163, TraceLevel.Info, "DistributedTask", "PoolProvider", "Sent Get Agent Definition request to {0}", (object) this.m_agentCloudSettings.GetAgentDefinitionEndpoint);
          }
          definitionsAsync = agentDefinitionList;
        }
      }
      return definitionsAsync;
    }

    public async Task<string> GetAgentRequestStatusAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest agentCloudRequest)
    {
      AgentRequestStatus agentRequestStatus = (AgentRequestStatus) null;
      AgentIdentifier identifier = new AgentIdentifier()
      {
        AgentId = agentCloudRequest.RequestId,
        AccountId = requestContext.ServiceHost.InstanceId,
        AgentPool = agentCloudRequest.Pool.Name,
        AgentData = agentCloudRequest.AgentData
      };
      using (RemoteProviderHttpClient client = await this.m_agentCloudSettings.GetPoolProviderClient(requestContext, this.GetPoolProviderConfigurationForRequest(requestContext, agentCloudRequest)))
      {
        using (requestContext.CreateAsyncTimeOutScope(this.GetRequestSendTimeout(requestContext)))
        {
          try
          {
            agentRequestStatus = await client.GetAgentRequestStatusAsync(identifier, requestContext.CancellationToken);
          }
          catch (InvalidResponseCodeException ex)
          {
            throw new TaskAgentCloudCommunicationException(TaskResources.AgentCloudResponseStatusCodeInvalid((object) this.m_agentCloudSettings.Name, (object) ex.StatusCode));
          }
          finally
          {
            requestContext.TraceAlways(10015163, TraceLevel.Info, "DistributedTask", "PoolProvider", "Sent Get Agent Request Status request to {0}", (object) this.m_agentCloudSettings.GetAgentRequestStatusEndpoint);
          }
        }
      }
      string statusMessage = agentRequestStatus?.StatusMessage;
      identifier = (AgentIdentifier) null;
      return statusMessage;
    }

    public async Task ProvisionAgentAsync(
      IVssRequestContext requestContext,
      TaskAgentJobRequest jobRequest)
    {
      TaskAgentCloudRequest agentCloudRequest = this.GetAgentCloudRequest(requestContext, jobRequest);
      IInternalAgentCloudService agentCloudService = requestContext.GetService<IInternalAgentCloudService>();
      agentCloudRequest = await agentCloudService.CreateAgentCloudRequestAsync(requestContext, agentCloudRequest);
      AgentRequest agentRequest = await this.GetAgentRequestAsync(requestContext, agentCloudRequest, jobRequest);
      PoolProviderConfiguration poolProviderConfiguration = await this.GetPoolProviderConfigurationForProvisioning(requestContext, agentCloudRequest);
      AgentRequestResponse agentRequestResponse;
      using (RemoteProviderHttpClient client = await this.m_agentCloudSettings.GetPoolProviderClient(requestContext, poolProviderConfiguration))
      {
        using (requestContext.CreateAsyncTimeOutScope(this.GetRequestSendTimeout(requestContext)))
        {
          try
          {
            agentRequestResponse = await client.ProvisionAgentAsync(agentRequest, requestContext.CancellationToken);
            agentRequestResponse.AddProviderConfigurationToAgentRequestResponse(poolProviderConfiguration);
          }
          catch (InvalidResponseCodeException ex)
          {
            throw new TaskAgentCloudCommunicationException(TaskResources.AgentCloudResponseStatusCodeInvalid((object) this.m_agentCloudSettings.Name, (object) ex.StatusCode));
          }
          finally
          {
            requestContext.TraceAlways(10015163, TraceLevel.Info, "DistributedTask", "PoolProvider", "Sent Acquire Agent request to {0}", (object) this.m_agentCloudSettings.AcquireAgentEndpoint);
          }
        }
      }
      agentCloudRequest.ProvisionRequestTime = new DateTime?(DateTime.UtcNow);
      agentCloudRequest.AgentData = agentRequestResponse.AgentData;
      TaskAgentCloudRequest agentCloudRequest1 = await agentCloudService.UpdateAgentCloudRequestAsync(requestContext, agentCloudRequest);
      agentCloudRequest = (TaskAgentCloudRequest) null;
      agentCloudService = (IInternalAgentCloudService) null;
      agentRequest = (AgentRequest) null;
      poolProviderConfiguration = (PoolProviderConfiguration) null;
    }

    private TimeSpan GetRequestSendTimeout(IVssRequestContext requestContext)
    {
      int seconds = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in RemoteAgentPoolProvider.s_requestSendTimeoutQuery, false, this.RequestSendTimeout.Seconds);
      if (seconds <= 0)
        seconds = this.RequestSendTimeout.Seconds;
      return TimeSpan.FromSeconds((double) seconds);
    }

    public virtual TaskAgentCloudRequest GetAgentCloudRequest(
      IVssRequestContext requestContext,
      TaskAgentJobRequest jobRequest)
    {
      return new TaskAgentCloudRequest()
      {
        AgentCloudId = this.m_agentCloudSettings.AgentCloudId,
        RequestId = Guid.NewGuid(),
        Pool = new TaskAgentPoolReference()
        {
          Id = jobRequest.PoolId
        },
        Agent = new TaskAgentReference()
        {
          Id = jobRequest.ReservedAgent.Id
        },
        AgentSpecification = jobRequest.AgentSpecification
      };
    }

    protected virtual PoolProviderConfiguration GetPoolProviderConfigurationForRequest(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest cloudRequest = null)
    {
      return (PoolProviderConfiguration) null;
    }

    protected virtual Task<PoolProviderConfiguration> GetPoolProviderConfigurationForProvisioning(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest cloudRequest = null)
    {
      return Task.FromResult<PoolProviderConfiguration>((PoolProviderConfiguration) null);
    }

    protected virtual Task<AgentRequest> GetAgentRequestAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest agentCloudRequest,
      TaskAgentJobRequest jobRequest)
    {
      return Task.FromResult<AgentRequest>(PoolProviderAgentRequestHelper.GetAgentRequest(requestContext, this.m_agentCloudSettings, agentCloudRequest, (TaskAgentPoolReference) this.m_agentPool.Pool, jobRequest.ReservedAgent, jobRequest));
    }

    public object Clone() => (object) new RemoteAgentPoolProvider(this);
  }
}
