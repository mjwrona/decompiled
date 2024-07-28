// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticPoolConfigurationJobBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models;
using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public class ElasticPoolConfigurationJobBase : VssAsyncJobExtension
  {
    protected virtual OrchestrationType OrchestrationType { get; set; }

    protected virtual async Task<TaskAgentPool> EnsureAgentPoolExists(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      TaskAgentPool agentPool = requestContext.GetService<IDistributedTaskResourceService>().GetAgentPool(requestContext, elasticPool.PoolId);
      if (agentPool == null)
      {
        sb.AppendLine(string.Format("AgentPool {0} {1} does not exist!  Deleting ElasticPool.", (object) elasticPool.PoolId, (object) elasticPool.AzureId));
        await requestContext.GetService<IElasticPoolService>().DeleteElasticPoolAsync(requestContext, elasticPool, false);
      }
      TaskAgentPool taskAgentPool = agentPool;
      agentPool = (TaskAgentPool) null;
      return taskAgentPool;
    }

    protected virtual async Task UpdateVmExtension(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      TaskAgentPool agentPool,
      StringBuilder sb)
    {
      try
      {
        await requestContext.GetService<IElasticPoolService>().UpdateExtensionAsync(requestContext, elasticPool, agentPool.Name);
        sb.AppendLine("Updated VM Extension");
      }
      catch (CloudException ex)
      {
        sb.AppendLine("Failed to update VM Extension. " + ex.Message);
      }
    }

    protected virtual async Task EnsureScaleSetAutoscaleConfiguration(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      IAzureMonitorResourceService monitorClient = requestContext.GetService<IAzureMonitorResourceService>();
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = await requestContext.GetService<IServiceEndpointService2>().GetServiceEndpointAsync(requestContext, elasticPool.ServiceEndpointScope, elasticPool.ServiceEndpointId);
      if (serviceEndpoint == null)
      {
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.ConfigurationJob, ElasticResources.ServiceEndpointDoesNotExist());
        throw new ServiceEndpointDoesNotExistException(elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope);
      }
      if (!serviceEndpoint.Data.ContainsKey("subscriptionId") || string.IsNullOrEmpty(serviceEndpoint.Data["subscriptionId"]))
      {
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.ConfigurationJob, ElasticResources.InvalidServiceEndpoint());
        throw new InvalidServiceEndpointException("Service endpoint " + serviceEndpoint.Name + " does not contain an Azure Subscription Id");
      }
      List<AutoscaleSettingResource> list = (await monitorClient.ListAutoscaleSettingsInSubscriptionAsync(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope)).ToList<AutoscaleSettingResource>();
      if (list.Count == 0)
        sb.AppendLine(string.Format("No AutoscaleSettings found for ServiceEndpoint {0} Scope: {1}", (object) elasticPool.ServiceEndpointId, (object) elasticPool.ServiceEndpointScope));
      foreach (AutoscaleSettingResource setting in list)
      {
        if (string.Equals(setting.Properties.TargetResourceUri, elasticPool.AzureId, StringComparison.OrdinalIgnoreCase))
        {
          sb.AppendLine(string.Format("Inspecting setting:{0}, TargetResourceUri:{1}, Enabled:{2}", (object) setting.Name, (object) setting.Properties.TargetResourceUri, (object) setting.Properties.Enabled));
          if (setting.Properties.Enabled)
          {
            try
            {
              sb.AppendLine("Removing autoscale rule " + setting.Name);
              await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Warning, OperationType.ConfigurationJob, ElasticResources.UpdatingVirtualMachineScaleSet() + " " + ElasticResources.RemovingAutoscaleRule((object) setting.Name));
              await monitorClient.DeleteAutoscaleSettingAsync(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope, setting);
            }
            catch (CloudException ex)
            {
              await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.ConfigurationJob, ex.Message);
              sb.AppendLine("Failed to delete autoscale rule. " + ex.Message);
            }
          }
        }
      }
      monitorClient = (IAzureMonitorResourceService) null;
      els = (IElasticPoolLogService) null;
      serviceEndpoint = (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint) null;
    }

    protected virtual async Task<VirtualMachineScaleSet> EnsureScaleSetIsOnline(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      IElasticPoolService eps = requestContext.GetService<IElasticPoolService>();
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      IAzureVirtualMachineScaleSetResourceServiceInternal ssrs = requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
      VirtualMachineScaleSet scaleSet = (VirtualMachineScaleSet) null;
      bool updatePool = false;
      try
      {
        scaleSet = await ssrs.GetScaleSetAsync(requestContext, elasticPool);
        if (scaleSet.OrchestrationType() == OrchestrationType.Uniform)
        {
          IReadOnlyList<VirtualMachineScaleSetVM> machineScaleSetVmList = await ssrs.ListVMInstancesAsync(requestContext, elasticPool);
        }
      }
      catch (Exception ex)
      {
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.ConfigurationJob, ElasticResources.GetScalesetFailure((object) ex.Message));
        sb.AppendLine("Get Scaleset failed. " + ex.Message);
      }
      DateTime? nullable1;
      if (scaleSet == null && elasticPool.State != ElasticPoolState.Offline)
      {
        nullable1 = elasticPool.OfflineSince;
        if (!nullable1.HasValue)
        {
          elasticPool.OfflineSince = new DateTime?(DateTime.UtcNow);
          updatePool = true;
        }
        else
        {
          DateTime utcNow = DateTime.UtcNow;
          nullable1 = elasticPool.OfflineSince;
          TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(utcNow - nullable1.GetValueOrDefault()) : new TimeSpan?();
          TimeSpan timeSpan = TimeSpan.FromHours(8.0);
          if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() > timeSpan ? 1 : 0) : 0) != 0)
          {
            elasticPool.State = ElasticPoolState.Offline;
            updatePool = true;
          }
        }
      }
      if (scaleSet != null)
      {
        if (elasticPool.State != ElasticPoolState.Offline && elasticPool.State != ElasticPoolState.New)
        {
          nullable1 = elasticPool.OfflineSince;
          if (!nullable1.HasValue)
            goto label_17;
        }
        elasticPool.State = ElasticPoolState.Online;
        ElasticPool elasticPool1 = elasticPool;
        nullable1 = new DateTime?();
        DateTime? nullable3 = nullable1;
        elasticPool1.OfflineSince = nullable3;
        updatePool = true;
      }
label_17:
      if (updatePool)
      {
        NullableElasticPool elasticPool2 = new NullableElasticPool()
        {
          PoolId = elasticPool.PoolId,
          State = new ElasticPoolState?(elasticPool.State),
          OfflineSince = elasticPool.OfflineSince,
          OrchestrationType = new OrchestrationType?(scaleSet == null ? elasticPool.OrchestrationType : scaleSet.OrchestrationType())
        };
        sb.AppendLine(string.Format("Updating Pool State. State:{0}, OfflineSince:{1}", (object) elasticPool2.State, (object) elasticPool2.OfflineSince));
        ElasticPool elasticPool3 = await eps.UpdateElasticPoolAsync(requestContext, elasticPool2);
      }
      VirtualMachineScaleSet virtualMachineScaleSet = scaleSet;
      eps = (IElasticPoolService) null;
      els = (IElasticPoolLogService) null;
      ssrs = (IAzureVirtualMachineScaleSetResourceServiceInternal) null;
      scaleSet = (VirtualMachineScaleSet) null;
      return virtualMachineScaleSet;
    }

    public override Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      throw new NotImplementedException();
    }

    protected virtual Task EnsureScaleSetConfiguration(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      VirtualMachineScaleSet scaleSet,
      StringBuilder sb)
    {
      throw new NotImplementedException();
    }
  }
}
