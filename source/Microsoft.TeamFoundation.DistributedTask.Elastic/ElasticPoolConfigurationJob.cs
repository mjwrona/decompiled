// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticPoolConfigurationJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models;
using Microsoft.TeamFoundation.DistributedTask.Server.Constants;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public class ElasticPoolConfigurationJob : ElasticPoolConfigurationJobBase
  {
    protected override OrchestrationType OrchestrationType { get; set; }

    public static void EnableJob(IVssRequestContext requestContext, int delayInMinutes)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(DistributedTaskJobIds.ElasticPoolConfigurationJobId, "Elastic Pool Configuration Job", "Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticPoolConfigurationJob", (XmlNode) null, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.Normal);
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
      ElasticPoolConfigurationJob.QueueJob(requestContext, delayInMinutes);
    }

    public static void QueueJob(IVssRequestContext requestContext, int delayMinutes) => requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      DistributedTaskJobIds.ElasticPoolConfigurationJobId
    }, 60 * delayMinutes);

    public static void DisableJob(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationJobService>().DeleteJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      DistributedTaskJobIds.ElasticPoolConfigurationJobId
    });

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      ElasticPoolConfigurationJob configurationJob = this;
      IElasticPoolService service = requestContext.GetService<IElasticPoolService>();
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      IVssRegistryService registry = requestContext.GetService<IVssRegistryService>();
      int daysOfLogsToKeep = registry.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/DaysOfLogsToKeep", 30);
      registry.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/DaysOffProsessFlexiblePools", 7);
      TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
      string jobMessage = "Success";
      StringBuilder sb = new StringBuilder();
      int onlineCount = 0;
      IVssRequestContext requestContext1 = requestContext;
      IReadOnlyList<ElasticPool> poolsByTypeAsync = await service.GetElasticPoolsByTypeAsync(requestContext1, OrchestrationType.Uniform);
      if (poolsByTypeAsync.Count<ElasticPool>() == 0)
      {
        ElasticPoolSizingJob.DisableJob(requestContext);
        ElasticPoolConfigurationJob.DisableJob(requestContext);
        return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, "Disabled job because no elastic pools were found.");
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
          }
          await els.DeleteElasticPoolLogsByTimeAsync(requestContext, elasticPool.PoolId, DateTime.UtcNow.AddDays((double) -daysOfLogsToKeep));
          agentPool = (TaskAgentPool) null;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, "DistributedTask", nameof (ElasticPoolConfigurationJob), ex);
          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.ConfigurationJob, ex.Message);
          sb.AppendLine(ex.ToString());
          result = TeamFoundationJobExecutionResult.Failed;
          jobMessage = sb.ToString();
        }
        requestContext.TraceAlways(10015197, TraceLevel.Info, "DistributedTask", "ElasticPools", sb.ToString());
        sb.Clear();
      }
      if (onlineCount == 0)
        ElasticPoolSizingJob.DisableJob(requestContext);
      else
        ElasticPoolSizingJob.EnableJob(requestContext);
      ElasticPoolConfigurationJob.QueueJob(requestContext, registry.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/ConfigurationJobPeriodMinutes", 120));
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
      bool replaceConfiguration = false;
      OperatingSystemType operatingSystemType = scaleSet.OSType();
      VirtualMachineScaleSetProperties properties = scaleSet.Properties;
      sb.AppendLine(string.Format("Inspecting configuration. SKU:{0}, OsType:{1}, Overprovision:{2}, EnableAutomaticOSUpgrade:{3}, Mode:{4}, SinglePlacementGroup:{5}", (object) scaleSet.Sku?.Name, (object) operatingSystemType, (object) properties.Overprovision, (object) (bool?) properties.UpgradePolicy?.AutomaticOSUpgradePolicy?.EnableAutomaticOSUpgrade, (object) (VirtualMachineScaleSetUpgradeMode?) properties.UpgradePolicy?.Mode, (object) properties.SinglePlacementGroup));
      if (operatingSystemType != elasticPool.OsType)
      {
        sb.AppendLine(string.Format("ElasticPool {0} {1} switched OS from ${2} to ${3}", (object) elasticPool.PoolId, (object) elasticPool.AzureId, (object) elasticPool.OsType, (object) operatingSystemType));
        elasticPool.OsType = operatingSystemType;
        ElasticPool elasticPool1 = await eps.UpdateElasticPoolAsync(requestContext, new NullableElasticPool()
        {
          PoolId = elasticPool.PoolId,
          OsType = new OperatingSystemType?(elasticPool.OsType)
        });
      }
      bool? nullable = scaleSet.Properties.Overprovision;
      bool? overprovision = ElasticPoolConfigurationJob.DesiredConfig.Properties.Overprovision;
      if (!(nullable.GetValueOrDefault() == overprovision.GetValueOrDefault() & nullable.HasValue == overprovision.HasValue))
      {
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.ConfigurationJob, ElasticResources.UpdatingVirtualMachineScaleSet() + " " + ElasticResources.DisablingOverprovisionSetting());
        replaceConfiguration = true;
      }
      if (scaleSet.Properties.UpgradePolicy?.AutomaticOSUpgradePolicy != null)
      {
        bool? automaticOsUpgrade = (bool?) scaleSet.Properties.UpgradePolicy?.AutomaticOSUpgradePolicy.EnableAutomaticOSUpgrade;
        nullable = ElasticPoolConfigurationJob.DesiredConfig.Properties.UpgradePolicy.AutomaticOSUpgradePolicy.EnableAutomaticOSUpgrade;
        if (!(automaticOsUpgrade.GetValueOrDefault() == nullable.GetValueOrDefault() & automaticOsUpgrade.HasValue == nullable.HasValue))
        {
          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.ConfigurationJob, ElasticResources.UpdatingVirtualMachineScaleSet() + " " + ElasticResources.DisablingAutomaticOSUpgradePolicy());
          replaceConfiguration = true;
        }
      }
      VirtualMachineScaleSetUpgradeMode? mode1 = (VirtualMachineScaleSetUpgradeMode?) scaleSet.Properties.UpgradePolicy?.Mode;
      VirtualMachineScaleSetUpgradeMode? mode2 = ElasticPoolConfigurationJob.DesiredConfig.Properties.UpgradePolicy.Mode;
      if (!(mode1.GetValueOrDefault() == mode2.GetValueOrDefault() & mode1.HasValue == mode2.HasValue))
      {
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.ConfigurationJob, ElasticResources.UpdatingVirtualMachineScaleSet() + " " + ElasticResources.DisablingVmssUpgradePolicy());
        replaceConfiguration = true;
      }
      if (elasticPool.MaxCapacity > 100)
      {
        nullable = scaleSet.Properties.SinglePlacementGroup;
        bool? singlePlacementGroup = ElasticPoolConfigurationJob.DesiredConfig.Properties.SinglePlacementGroup;
        if (!(nullable.GetValueOrDefault() == singlePlacementGroup.GetValueOrDefault() & nullable.HasValue == singlePlacementGroup.HasValue))
        {
          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.ConfigurationJob, ElasticResources.UpdatingVirtualMachineScaleSet() + " " + ElasticResources.DisablingSinglePlacementGroupSetting());
          replaceConfiguration = true;
        }
      }
      if (replaceConfiguration)
      {
        sb.AppendLine("Replacing config");
        try
        {
          VirtualMachineScaleSet virtualMachineScaleSet = await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().UpdateScaleSetAsync(requestContext, elasticPool, ElasticPoolConfigurationJob.DesiredConfig);
        }
        catch (CloudException ex)
        {
          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.ConfigurationJob, ElasticResources.UpdateScalesetFailure((object) ex.Message));
          sb.AppendLine("Failed to replace config. " + ex.Message);
        }
      }
      await eps.UpdateElasticPoolTimeStampTagAsync(requestContext, elasticPool, scaleSet);
      eps = (IElasticPoolService) null;
      els = (IElasticPoolLogService) null;
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

    private static VirtualMachineScaleSet DesiredConfig { get; } = new VirtualMachineScaleSet()
    {
      Properties = new VirtualMachineScaleSetProperties()
      {
        Overprovision = new bool?(false),
        SinglePlacementGroup = new bool?(false),
        UpgradePolicy = new VirtualMachineScaleSetUpgradePolicy()
        {
          AutomaticOSUpgradePolicy = new VirtualMachineAutomaticOSUpgradePolicy()
          {
            EnableAutomaticOSUpgrade = new bool?(false)
          },
          Mode = new VirtualMachineScaleSetUpgradeMode?(VirtualMachineScaleSetUpgradeMode.Manual)
        }
      }
    };
  }
}
