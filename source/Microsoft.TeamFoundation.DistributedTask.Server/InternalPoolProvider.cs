// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.InternalPoolProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.AzComm.SharedContracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Enums;
using Microsoft.VisualStudio.Services.AzComm.WebApi.HttpClients;
using Microsoft.VisualStudio.Services.MachineManagement.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class InternalPoolProvider : RemoteAgentPoolProvider
  {
    public InternalPoolProvider(TaskAgentCloud agentCloud, TaskAgentPoolData pool)
      : base(agentCloud, pool)
    {
    }

    public override async Task BillForResourcesAsync(
      IVssRequestContext requestContext,
      TaskAgentJobRequest jobRequest)
    {
      string parallelismTag = jobRequest.GetParallelismTag();
      bool flag = this.HasPremiumAgents(requestContext);
      if (jobRequest.Result.Value == TaskResult.Abandoned || !jobRequest.ReceiveTime.HasValue || !jobRequest.FinishTime.HasValue || string.Equals(parallelismTag, "Public") || flag)
        return;
      TaskOrchestrationPlan planAsync = await jobRequest.GetPlanAsync(requestContext);
      DateTime? nullable = jobRequest.FinishTime;
      DateTime dateTime1 = nullable.Value;
      nullable = jobRequest.ReceiveTime;
      DateTime dateTime2 = nullable.Value;
      int minutes = (int) Math.Ceiling((dateTime1 - dateTime2).TotalMinutes);
      if (planAsync == null || minutes <= 0)
        return;
      this.ReportUsage(requestContext, planAsync, minutes);
    }

    private void ReportUsage(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      int minutes)
    {
      requestContext.To(TeamFoundationHostType.Deployment).GetClient<MeterUsage2HttpClient>().ReportUsageAsync(new MeterUsageReportRequest()
      {
        MeterId = AzCommMeterIds.BuildMinutesMeterId,
        Quantity = (double) minutes,
        Kind = UsageReportOperation.Increment,
        EventId = plan.ArtifactUri.ToString()
      }, requestContext.ServiceHost.InstanceId).SyncResult();
    }

    private bool HasPremiumAgents(IVssRequestContext requestContext)
    {
      MeterUsage2GetResponse usage2GetResponse = requestContext.To(TeamFoundationHostType.Deployment).GetClient<MeterUsage2HttpClient>().GetMeterUsageAsync(requestContext.ServiceHost.InstanceId, AzCommMeterIds.MSHostedCICDMeterId).SyncResult<MeterUsage2GetResponse>();
      return usage2GetResponse != null && usage2GetResponse.MaxQuantity > 0.0;
    }

    public override TaskAgentCloudRequest GetAgentCloudRequest(
      IVssRequestContext requestContext,
      TaskAgentJobRequest jobRequest)
    {
      bool flag1 = requestContext.RunSynchronously<bool>((Func<Task<bool>>) (() => jobRequest.TryUpdateAgentSpecificationForPoolAsync(requestContext, (TaskAgentPoolReference) this.m_agentPool.Pool)));
      bool flag2 = requestContext.IsFeatureEnabled("DistributedTask.EnforceInternalAgentSpecifcation");
      requestContext.TraceAlways(10015269, TraceLevel.Info, nameof (InternalPoolProvider), nameof (GetAgentCloudRequest), Array.Empty<string>(), string.Format("CodePath: {0}. PoolName: {1}. AgentSpecificationUpdated: {2}. EnforceInternalAgentSpecifcation: {3}.", (object) nameof (GetAgentCloudRequest), (object) this.m_agentPool.Pool.Name, (object) flag1, (object) flag2));
      if (flag2 && !flag1)
        throw new MachineImageLabelDoesNotExistException(TaskResources.ImageLabelNotFound((object) this.m_agentPool.Pool.Name));
      TaskAgentCloudRequest agentCloudRequest = base.GetAgentCloudRequest(requestContext, jobRequest);
      if (agentCloudRequest.AgentSpecification != null && !agentCloudRequest.AgentSpecification.Properties().Any<JProperty>((Func<JProperty, bool>) (x => string.Equals(x.Name, "PerformanceMetrics", StringComparison.OrdinalIgnoreCase))))
        agentCloudRequest.AgentSpecification.Add((object) new JProperty("PerformanceMetrics", (object) jobRequest.GetPerformanceMetrics()));
      return agentCloudRequest;
    }

    protected override async Task<AgentRequest> GetAgentRequestAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest agentCloudRequest,
      TaskAgentJobRequest jobRequest)
    {
      AgentRequest agentRequest = await base.GetAgentRequestAsync(requestContext, agentCloudRequest, jobRequest);
      HostedTaskAgentExtension taskAgentExtension = new HostedTaskAgentExtension();
      agentRequest.ProjectType = jobRequest.GetParallelismTag();
      IVssRequestContext requestContext1 = requestContext;
      int poolId = jobRequest.PoolId;
      string projectType = agentRequest.ProjectType;
      Guid scopeId = jobRequest.ScopeId;
      Guid planId = jobRequest.PlanId;
      HostedPoolSettings hostedPoolSettings = await taskAgentExtension.CheckHostedBillingResourcesAsync(requestContext1, poolId, projectType, false, scopeId, planId);
      agentRequest.Timeout = new int?(hostedPoolSettings.RequestTimeout);
      agentRequest.MaxParallelism = new int?(hostedPoolSettings.MaxParallelism);
      agentRequest.IsScheduled = jobRequest.GetIsScheduledTag();
      agentRequest.AddTierToAgentSpecification(hostedPoolSettings.Tier);
      agentRequest.AddRegionToAgentSpecification(requestContext);
      agentRequest.AddRequestTags(requestContext);
      AgentRequest agentRequestAsync = agentRequest;
      agentRequest = (AgentRequest) null;
      return agentRequestAsync;
    }

    public override async Task<IList<AgentDefinition>> GetAgentDefinitionsAsync(
      IVssRequestContext requestContext)
    {
      List<AgentDefinition> agentDefinitions = new List<AgentDefinition>();
      await AddAgentDefinitions(requestContext, agentDefinitions);
      await AddAgentDefinitions(requestContext.To(TeamFoundationHostType.Deployment), agentDefinitions);
      IList<AgentDefinition> definitionsAsync = (IList<AgentDefinition>) agentDefinitions;
      agentDefinitions = (List<AgentDefinition>) null;
      return definitionsAsync;

      static async Task AddAgentDefinitions(
        IVssRequestContext rc,
        List<AgentDefinition> agentDefinitions)
      {
        List<InternalCloudAgentDefinition> definitionsAsync = await rc.GetService<IInternalCloudAgentDefinitionService>().GetInternalCloudAgentDefinitionsAsync(rc);
        if (definitionsAsync == null)
          return;
        agentDefinitions.AddRange((IEnumerable<AgentDefinition>) definitionsAsync.Where<InternalCloudAgentDefinition>((Func<InternalCloudAgentDefinition, bool>) (agent => agent.IsVisible)));
      }
    }

    protected override async Task<PoolProviderConfiguration> GetPoolProviderConfigurationForProvisioning(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest cloudRequest = null)
    {
      PoolProviderConfiguration providerConfiguration = await PoolProviderConfigurationHelpers.GetDeploymentPoolProviderConfiguration(requestContext);
      requestContext.TraceInfo(10015239, nameof (InternalPoolProvider), string.Format("Pool provider configuration for provisioning RequestId: {0} is: {1}", (object) cloudRequest?.RequestId, (object) providerConfiguration?.ToString()));
      return providerConfiguration;
    }

    protected override PoolProviderConfiguration GetPoolProviderConfigurationForRequest(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest cloudRequest = null)
    {
      PoolProviderConfiguration providerConfiguration = cloudRequest != null ? cloudRequest.AgentData.GetPoolProviderConfiguration(requestContext) : (PoolProviderConfiguration) null;
      requestContext.TraceInfo(10015239, nameof (InternalPoolProvider), string.Format("Saved pool provider configuration for provisioning RequestId: {0} is: {1}", (object) cloudRequest?.RequestId, (object) providerConfiguration?.ToString()));
      return providerConfiguration;
    }
  }
}
