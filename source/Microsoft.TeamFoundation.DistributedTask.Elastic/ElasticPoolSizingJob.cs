// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticPoolSizingJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

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
  public class ElasticPoolSizingJob : ElasticPoolSizingJobBase
  {
    protected override OrchestrationType OrchestrationType { get; set; }

    public static void EnableJob(IVssRequestContext requestContext)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(DistributedTaskJobIds.ElasticPoolSizingJobId, "Elastic Pool Sizing Job", "Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticPoolSizingJob", (XmlNode) null, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.Normal);
      foundationJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
      {
        ScheduledTime = DateTime.UtcNow,
        TimeZoneId = TimeZoneInfo.Utc.Id,
        Interval = 3600
      });
      IVssRequestContext requestContext1 = requestContext;
      service.UpdateJobDefinitions(requestContext1, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      });
      ElasticPoolSizingJob.QueueJob(requestContext, 0);
    }

    private static void QueueJob(IVssRequestContext requestContext, int delayMinutes) => requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      DistributedTaskJobIds.ElasticPoolSizingJobId
    }, 60 * delayMinutes);

    public static void DisableJob(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationJobService>().DeleteJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      DistributedTaskJobIds.ElasticPoolSizingJobId
    });

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      VssJobResult vssJobResult = await base.RunAsync(requestContext, jobDefinition, queueTime);
      ElasticPoolSizingJob.QueueJob(requestContext, requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/SizingJobPeriodMinutes", 5));
      return vssJobResult;
    }

    protected override async Task<List<ElasticPoolSizingJobBase.SizingAction>> AdjustSize(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      PoolState state,
      StringBuilder sb)
    {
      ElasticPoolSizingJob elasticPoolSizingJob = this;
      bool canChangeScaleSet = state.IsScalesetStable;
      int num1 = state.IsReimageOngoing ? 1 : 0;
      List<ElasticPoolSizingJobBase.SizingAction> actions = new List<ElasticPoolSizingJobBase.SizingAction>();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/PrioritizeReimageFactor";
      ref RegistryQuery local1 = ref registryQuery;
      int num2 = service.GetValue<int>(requestContext1, in local1, 50);
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/PrioritizeReimagePostponeDivisorPath";
      ref RegistryQuery local2 = ref registryQuery;
      int num3 = service.GetValue<int>(requestContext2, in local2, 5);
      int num4 = state.DesiredSize - state.ComputeSize;
      bool prioritizeReimage = requestContext.IsFeatureEnabled("DistributedTask.PrioritizeElasticPoolOperations") && (double) state.ReimageList.Count * (double) num2 / 100.0 > (double) num4 && state.ReimageList.Count > 0 && !state.WasSizeUpSuccessful;
      if (requestContext.IsFeatureEnabled("DistributedTask.DontPrioritizeReimageWhenDeleteCountIsHigh") && state.DeleteList.Count >= num4 + state.ReimageList.Count)
        prioritizeReimage = false;
      if (elasticPool.SizingAttempts % num3 == 0)
        prioritizeReimage = false;
      if (num1 != 0)
        prioritizeReimage = false;
      if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDeleteStuckVM") && state.StuckList.Count > 0)
        prioritizeReimage = false;
      if (state.RetryList.Count > 0)
        prioritizeReimage = false;
      sb.AppendLine(string.Format("ReimageFactor:{0}, IncreaseCount:{1}, PrioritizeReimage:{2}", (object) num2, (object) num4, (object) prioritizeReimage));
      if (canChangeScaleSet && state.DesiredSize > state.ComputeSize && !prioritizeReimage && state.RetryList.Count == 0)
      {
        if (await elasticPoolSizingJob.TryIncreaseCapacity(requestContext, sb, elasticPool, state))
        {
          actions.Add(ElasticPoolSizingJobBase.SizingAction.SizeUp);
          canChangeScaleSet = false;
        }
      }
      if (canChangeScaleSet && state.DesiredSize < state.ComputeSize)
      {
        int count1 = state.ComputeSize - state.DesiredSize;
        int count2 = state.LongIdleList.Count;
        state.DeleteList.AddRange(state.LongIdleList.Take<ElasticNode>(count1));
        state.DeleteList.AddRange(state.IdleList.Take<ElasticNode>(count1 - count2));
      }
      if (state.DeleteList.Count > 0 && !prioritizeReimage)
      {
        bool force = false;
        if ((state.StuckList.Count > 0 || state.RetryList.Count > 0) && requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolForceDelete"))
          force = true;
        if (await elasticPoolSizingJob.TryDeleteNodesAsync(requestContext, elasticPool, state.DeleteList, state.ComputeSize, canChangeScaleSet, sb, force))
        {
          actions.Add(ElasticPoolSizingJobBase.SizingAction.SizeDown);
          canChangeScaleSet = false;
        }
      }
      if (((!canChangeScaleSet ? 0 : (state.ReimageList.Count > 0 ? 1 : 0)) | (prioritizeReimage ? 1 : 0)) != 0)
      {
        if (await elasticPoolSizingJob.TryReimageVMs(requestContext, sb, elasticPool, state.ReimageList))
          actions.Add(ElasticPoolSizingJobBase.SizingAction.Reimage);
        else if (await elasticPoolSizingJob.TryDeleteNodesAsync(requestContext, elasticPool, state.ReimageList, state.ComputeSize, canChangeScaleSet, sb, requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolForceDelete")))
        {
          actions.Add(ElasticPoolSizingJobBase.SizingAction.SizeDown);
          canChangeScaleSet = false;
        }
      }
      List<ElasticPoolSizingJobBase.SizingAction> sizingActionList = actions;
      actions = (List<ElasticPoolSizingJobBase.SizingAction>) null;
      return sizingActionList;
    }

    protected async Task<bool> TryReimageVMs(
      IVssRequestContext requestContext,
      StringBuilder sb,
      ElasticPool elasticPool,
      List<ElasticNode> nodes)
    {
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      IAzureVirtualMachineScaleSetResourceServiceInternal ssrs = requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
      HashSet<string> ids = new HashSet<string>();
      await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.Reimage, ElasticResources.ReimagingMachines((object) nodes.Count));
      foreach (ElasticNode node in nodes)
        ids.Add(node.ComputeId);
      try
      {
        await ssrs.UpgradeMachinesAsync(requestContext, elasticPool, ids);
      }
      catch (CloudException ex)
      {
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.Reimage, ex.Message);
        requestContext.TraceException(0, "DistributedTask", nameof (ElasticPoolSizingJob), (Exception) ex);
        sb.AppendLine("UpgradeMachines failed. " + ex.Message);
      }
      int num;
      try
      {
        sb.AppendLine(string.Format("Reimaging VMs:{0}", (object) ids.Count));
        await ssrs.ReimageAllMachinesAsync(requestContext, elasticPool, ids);
        StringBuilder sb1 = new StringBuilder();
        sb1.AppendLine(string.Format("ElasticPool Id:{0} Reimage Nodes", (object) elasticPool.PoolId));
        foreach (ElasticNode node in nodes)
          node.TransitionState(ElasticNodeState.ReimagingCompute, sb1);
        requestContext.TraceAlways(10015202, TraceLevel.Info, "DistributedTask", "ElasticPools", sb1.ToString());
        IReadOnlyList<ElasticNode> elasticNodeList = await requestContext.GetService<IElasticNodeService>().UpdateElasticNodesAsync(requestContext, elasticPool.PoolId, (IList<ElasticNode>) nodes);
        return true;
      }
      catch (CloudException ex)
      {
        num = 1;
      }
      if (num == 1)
      {
        ex = ex;
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.Reimage, ElasticResources.ReimagingMachinesError((object) ex.Message));
        requestContext.TraceException(0, "DistributedTask", nameof (ElasticPoolSizingJob), (Exception) ex);
        sb.AppendLine("ReimageMachine failed. " + ex.Message);
        ex = (CloudException) null;
      }
      return false;
    }

    protected override void DisableAgents(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      List<ElasticNode> nodes,
      StringBuilder sb)
    {
      base.DisableAgents(requestContext, elasticPool, nodes, sb);
    }

    protected override async Task UpdateElasticPool(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      PoolState state,
      List<ElasticPoolSizingJobBase.SizingAction> actions,
      StringBuilder sb)
    {
      if (!state.IsScalesetStable)
        return;
      await base.UpdateElasticPool(requestContext, elasticPool, state, actions, sb);
    }

    protected override async Task<bool> TryIncreaseCapacity(
      IVssRequestContext requestContext,
      StringBuilder sb,
      ElasticPool elasticPool,
      PoolState state)
    {
      return await base.TryIncreaseCapacity(requestContext, sb, elasticPool, state);
    }

    protected override async Task<bool> TryDeleteNodesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      List<ElasticNode> nodeCandidates,
      int currentSize,
      bool canChangeScaleSet,
      StringBuilder sb,
      bool force)
    {
      return await base.TryDeleteNodesAsync(requestContext, elasticPool, nodeCandidates, currentSize, canChangeScaleSet, sb, force);
    }

    protected override async Task ExtractUnhealthyNodeDetailsAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      ElasticNode node,
      StringBuilder sb)
    {
      await base.ExtractUnhealthyNodeDetailsAsync(requestContext, elasticPool, node, sb);
    }
  }
}
