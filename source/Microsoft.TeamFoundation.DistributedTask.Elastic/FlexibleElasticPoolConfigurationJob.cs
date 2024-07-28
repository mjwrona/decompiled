// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.FlexibleElasticPoolConfigurationJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.Server.Constants;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public class FlexibleElasticPoolConfigurationJob : ElasticPoolConfigurationJobBase
  {
    protected override OrchestrationType OrchestrationType { get; set; } = OrchestrationType.Flexible;

    public static void EnableJob(IVssRequestContext requestContext, int delayInMinutes)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(DistributedTaskJobIds.FlexibleElasticPoolConfigurationJobId, "Flexible Elastic Pool Configuration Job", "Microsoft.TeamFoundation.DistributedTask.Elastic.FlexibleElasticPoolConfigurationJob", (XmlNode) null, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.Normal);
      foundationJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
      {
        ScheduledTime = DateTime.UtcNow,
        TimeZoneId = TimeZoneInfo.Utc.Id,
        Interval = 86400
      });
      IVssRequestContext requestContext1 = requestContext;
      service.UpdateJobDefinitions(requestContext1, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      });
      FlexibleElasticPoolConfigurationJob.QueueJob(requestContext, delayInMinutes);
    }

    public static void QueueJob(IVssRequestContext requestContext, int delayMinutes) => requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      DistributedTaskJobIds.FlexibleElasticPoolConfigurationJobId
    }, 60 * delayMinutes);

    public static void DisableJob(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationJobService>().DeleteJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      DistributedTaskJobIds.FlexibleElasticPoolConfigurationJobId
    });

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      FlexibleElasticPoolConfigurationJob configurationJob = this;
      IVssRegistryService registry = requestContext.GetService<IVssRegistryService>();
      IElasticPoolService service = requestContext.GetService<IElasticPoolService>();
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      IVssRegistryService registryService1 = registry;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/DaysOfLogsToKeep";
      ref RegistryQuery local1 = ref registryQuery;
      int daysOfLogsToKeep = registryService1.GetValue<int>(requestContext1, in local1, 30);
      IVssRegistryService registryService2 = registry;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/DaysOffProsessFlexiblePools";
      ref RegistryQuery local2 = ref registryQuery;
      int daysOffProsessFlexiblePools = registryService2.GetValue<int>(requestContext2, in local2, 7);
      TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
      string jobMessage = "Success";
      StringBuilder sb = new StringBuilder();
      int onlineCount = 0;
      IVssRequestContext requestContext3 = requestContext;
      IReadOnlyList<ElasticPool> poolsByTypeAsync = await service.GetElasticPoolsByTypeAsync(requestContext3, OrchestrationType.Flexible);
      if (!requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolFlexibleOrchestration") && poolsByTypeAsync.Count > 0)
      {
        try
        {
          await configurationJob.EnsureFlexiblePools(requestContext, (IEnumerable<ElasticPool>) poolsByTypeAsync, daysOffProsessFlexiblePools, sb);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, "DistributedTask", "ElasticPoolConfigurationJob", ex);
        }
        return new VssJobResult(TeamFoundationJobExecutionResult.Blocked, "DistributedTask.ElasticPoolFlexibleOrchestration feature is disabled.");
      }
      if (poolsByTypeAsync.Count == 0)
      {
        FlexibleElasticPoolSizingJob.DisableJob(requestContext);
        FlexibleElasticPoolConfigurationJob.DisableJob(requestContext);
        return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, "Disabled job because no flexible elastic pools were found.");
      }
      foreach (ElasticPool elasticPool in (IEnumerable<ElasticPool>) poolsByTypeAsync)
      {
        try
        {
          sb.AppendLine(string.Format("Inspecting AgentPool Pool Id:{0}, AzureId:{1}", (object) elasticPool.PoolId, (object) elasticPool.AzureId));
          TaskAgentPool agentPool = await configurationJob.EnsureAgentPoolExists(requestContext, elasticPool, sb);
          if (agentPool != null)
          {
            sb.AppendLine(string.Format("Inspecting Elastic Pool Id:{0}, AzureId:{1}, State:{2}, OfflineSince:{3}", (object) elasticPool.PoolId, (object) elasticPool.AzureId, (object) elasticPool.State, (object) elasticPool.OfflineSince));
            VirtualMachineScaleSet scaleSet = await configurationJob.EnsureScaleSetIsOnline(requestContext, elasticPool, sb);
            if (scaleSet != null)
            {
              ++onlineCount;
              await configurationJob.EnsureScaleSetConfiguration(requestContext, elasticPool, scaleSet, sb);
              await configurationJob.EnsureScaleSetAutoscaleConfiguration(requestContext, elasticPool, sb);
              await configurationJob.UpdateVmExtension(requestContext, elasticPool, agentPool, sb);
            }
            else
              requestContext.TraceAlways(10015197, TraceLevel.Error, "DistributedTask", "ElasticPools", "ScaleSet is offline or does not exist. ScaleSet identifier: " + elasticPool.AzureId);
          }
          else
            requestContext.TraceAlways(10015197, TraceLevel.Error, "DistributedTask", "ElasticPools", "Agent pool does not exist. Pool identifier: " + elasticPool.AzureId);
          await els.DeleteElasticPoolLogsByTimeAsync(requestContext, elasticPool.PoolId, DateTime.UtcNow.AddDays((double) -daysOfLogsToKeep));
          agentPool = (TaskAgentPool) null;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, "DistributedTask", "ElasticPoolConfigurationJob", ex);
          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.ConfigurationJob, ex.Message);
          sb.AppendLine(ex.ToString());
          result = TeamFoundationJobExecutionResult.PartiallySucceeded;
          jobMessage += string.Format("\n{0}", (object) sb);
        }
        requestContext.TraceAlways(10015197, TraceLevel.Info, "DistributedTask", "ElasticPools", sb.ToString());
        sb.Clear();
      }
      if (onlineCount == 0)
        FlexibleElasticPoolSizingJob.DisableJob(requestContext);
      else
        FlexibleElasticPoolSizingJob.EnableJob(requestContext);
      FlexibleElasticPoolConfigurationJob.QueueJob(requestContext, registry.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/ConfigurationJobPeriodMinutes", 120));
      return new VssJobResult(result, jobMessage);
    }

    protected override async Task EnsureScaleSetConfiguration(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      VirtualMachineScaleSet scaleSet,
      StringBuilder sb)
    {
      IElasticPoolService eps = requestContext.GetService<IElasticPoolService>();
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      OperatingSystemType operatingSystemType = scaleSet.OSType();
      OrchestrationType orchestrationType = scaleSet.OrchestrationType();
      VirtualMachineScaleSetProperties properties = scaleSet.Properties;
      sb.AppendLine(string.Format("Inspecting configuration. SKU:{0}, OsType:{1}, OrchestrationType:{2}, Overprovision:{3}, EnableAutomaticOSUpgrade:{4}, Mode:{5}, SinglePlacementGroup:{6}", (object) scaleSet.Sku?.Name, (object) operatingSystemType, (object) orchestrationType, (object) properties.Overprovision, (object) (bool?) properties.UpgradePolicy?.AutomaticOSUpgradePolicy?.EnableAutomaticOSUpgrade, (object) (VirtualMachineScaleSetUpgradeMode?) properties.UpgradePolicy?.Mode, (object) properties.SinglePlacementGroup));
      NullableElasticPool copy = new NullableElasticPool();
      bool flag = false;
      if (operatingSystemType != elasticPool.OsType || orchestrationType != elasticPool.OrchestrationType)
      {
        sb.AppendLine(string.Format("ElasticPool {0} {1} switched OS from ${2} to ${3}", (object) elasticPool.PoolId, (object) elasticPool.AzureId, (object) elasticPool.OsType, (object) operatingSystemType));
        elasticPool.OsType = operatingSystemType;
        elasticPool.OrchestrationType = orchestrationType;
        copy.PoolId = elasticPool.PoolId;
        copy.OsType = new OperatingSystemType?(elasticPool.OsType);
        copy.OrchestrationType = new OrchestrationType?(elasticPool.OrchestrationType);
        flag = true;
      }
      if (scaleSet.HasEmptyImage())
      {
        copy.PoolId = elasticPool.PoolId;
        copy.State = new ElasticPoolState?(ElasticPoolState.Unhealthy);
        flag = true;
      }
      if (flag)
      {
        ElasticPool elasticPool1 = await eps.UpdateElasticPoolAsync(requestContext, copy);
        ElasticPoolState? state = copy.State;
        ElasticPoolState elasticPoolState = ElasticPoolState.Unhealthy;
        if (state.GetValueOrDefault() == elasticPoolState & state.HasValue)
        {
          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.ConfigurationJob, ElasticResources.ServiceEndpointDoesNotExist());
          throw new ElasticPoolNotConfiguredException("Elastic pool " + scaleSet.Id + " is not configured");
        }
      }
      await eps.UpdateElasticPoolTimeStampTagAsync(requestContext, elasticPool, scaleSet);
      eps = (IElasticPoolService) null;
      els = (IElasticPoolLogService) null;
      copy = (NullableElasticPool) null;
    }

    protected override async Task<VirtualMachineScaleSet> EnsureScaleSetIsOnline(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      return await base.EnsureScaleSetIsOnline(requestContext, elasticPool, sb);
    }

    protected override async Task<TaskAgentPool> EnsureAgentPoolExists(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      return await base.EnsureAgentPoolExists(requestContext, elasticPool, sb);
    }

    protected override async Task UpdateVmExtension(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      TaskAgentPool agentPool,
      StringBuilder sb)
    {
      await base.UpdateVmExtension(requestContext, elasticPool, agentPool, sb);
    }

    protected override async Task EnsureScaleSetAutoscaleConfiguration(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      await base.EnsureScaleSetAutoscaleConfiguration(requestContext, elasticPool, sb);
    }

    private async Task EnsureFlexiblePools(
      IVssRequestContext requestContext,
      IEnumerable<ElasticPool> flexiblePools,
      int daysOffProsessFlexiblePools,
      StringBuilder sb)
    {
      IElasticPoolService eps = requestContext.GetService<IElasticPoolService>();
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      foreach (ElasticPool pool in flexiblePools)
      {
        DateTime? nullable1 = pool.OfflineSince;
        ref DateTime? local = ref nullable1;
        DateTime dateTime;
        DateTime? nullable2;
        if (!local.HasValue)
        {
          nullable2 = new DateTime?();
        }
        else
        {
          dateTime = local.GetValueOrDefault();
          nullable2 = new DateTime?(dateTime.AddDays((double) daysOffProsessFlexiblePools));
        }
        DateTime? nullable3 = nullable2;
        if (nullable3.HasValue)
        {
          nullable1 = nullable3;
          dateTime = DateTime.Now;
          if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() > dateTime ? 1 : 0) : 0) == 0)
            goto label_10;
        }
        NullableElasticPool nullableElasticPool = new NullableElasticPool();
        nullableElasticPool.PoolId = pool.PoolId;
        nullableElasticPool.State = new ElasticPoolState?(ElasticPoolState.Unhealthy);
        nullable1 = pool.OfflineSince;
        nullableElasticPool.OfflineSince = !nullable1.HasValue ? new DateTime?(DateTime.Now) : pool.OfflineSince;
        NullableElasticPool elasticPool1 = nullableElasticPool;
        sb.AppendLine(string.Format("Flexible pool is not supported:{0}, OfflineSince:{1}", (object) pool.PoolId, (object) elasticPool1.OfflineSince));
        ElasticPool elasticPool2 = await eps.UpdateElasticPoolAsync(requestContext, elasticPool1);
        await els.AddElasticPoolLogAsync(requestContext, pool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.ConfigurationJob, "VMSS with flexible orchestration mode is not supported");
label_10:;
      }
      eps = (IElasticPoolService) null;
      els = (IElasticPoolLogService) null;
    }
  }
}
