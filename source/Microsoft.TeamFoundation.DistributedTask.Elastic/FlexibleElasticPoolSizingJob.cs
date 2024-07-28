// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.FlexibleElasticPoolSizingJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models;
using Microsoft.TeamFoundation.DistributedTask.Server;
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
  internal class FlexibleElasticPoolSizingJob : ElasticPoolSizingJobBase
  {
    protected override OrchestrationType OrchestrationType { get; set; } = OrchestrationType.Flexible;

    public static void EnableJob(IVssRequestContext requestContext)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(DistributedTaskJobIds.FlexibleElasticPoolSizingJobId, "Flexible Elastic Pool Sizing Job", "Microsoft.TeamFoundation.DistributedTask.Elastic.FlexibleElasticPoolSizingJob", (XmlNode) null, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.Normal);
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
      FlexibleElasticPoolSizingJob.QueueJob(requestContext, 0);
    }

    public static void QueueJob(IVssRequestContext requestContext, int delayMinutes) => requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      DistributedTaskJobIds.FlexibleElasticPoolSizingJobId
    }, 60 * delayMinutes);

    public static void DisableJob(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationJobService>().DeleteJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      DistributedTaskJobIds.FlexibleElasticPoolSizingJobId
    });

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolFlexibleOrchestration"))
      {
        FlexibleElasticPoolSizingJob.DisableJob(requestContext);
        return new VssJobResult(TeamFoundationJobExecutionResult.Blocked, "DistributedTask.ElasticPoolFlexibleOrchestration feature is disabled.");
      }
      VssJobResult vssJobResult = await base.RunAsync(requestContext, jobDefinition, queueTime);
      FlexibleElasticPoolSizingJob.QueueJob(requestContext, requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/SizingJobPeriodMinutes", 5));
      return vssJobResult;
    }

    protected override async Task<List<ElasticPoolSizingJobBase.SizingAction>> AdjustSize(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      PoolState state,
      StringBuilder sb)
    {
      FlexibleElasticPoolSizingJob elasticPoolSizingJob = this;
      List<ElasticPoolSizingJobBase.SizingAction> actions = new List<ElasticPoolSizingJobBase.SizingAction>();
      if (state.DesiredSize > state.ComputeSize)
      {
        if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolFlexibleOrchestrationRefreshVMs") && state.ReimageList.Any<ElasticNode>((Func<ElasticNode, bool>) (node =>
        {
          bool? needUpgrade = node.NeedUpgrade;
          bool flag = true;
          return needUpgrade.GetValueOrDefault() == flag & needUpgrade.HasValue;
        })))
          requestContext.TraceAlways(10015196, TraceLevel.Info, "DistributedTask", "ElasticPools", "Skipping scale-out operation since there are some VM required recreation.");
        else if (await elasticPoolSizingJob.TryIncreaseCapacity(requestContext, sb, elasticPool, state))
          actions.Add(ElasticPoolSizingJobBase.SizingAction.SizeUp);
      }
      else if (state.DesiredSize < state.ComputeSize)
      {
        int count1 = state.ComputeSize - state.DesiredSize;
        int count2 = state.LongIdleList.Count;
        state.DeleteList.AddRange(state.LongIdleList.Take<ElasticNode>(count1));
        state.DeleteList.AddRange(state.IdleList.Take<ElasticNode>(count1 - count2));
      }
      if (state.UnhealthyVmList.Count > 0 && requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDeleteUnhealthyVms"))
      {
        if (!await elasticPoolSizingJob.TryDeleteUnhealthyNodesAsync(requestContext, elasticPool, state.UnhealthyVmList, sb, requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolForceDelete")))
          requestContext.TraceAlways(10015196, TraceLevel.Info, "DistributedTask", "ElasticPools", "Failed to delete unhealthy VMs.");
      }
      if (state.ReimageList.Count > 0)
      {
        if ((await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().GetScaleSetAsync(requestContext, elasticPool)).AllowReimage())
        {
          List<ElasticNode> elasticNodeList = await elasticPoolSizingJob.TryReimageVMs(requestContext, sb, elasticPool, state.ReimageList, state);
          if (elasticNodeList.Count<ElasticNode>() > 0 && elasticNodeList.Count<ElasticNode>() != state.ReimageList.Count)
            actions.Add(ElasticPoolSizingJobBase.SizingAction.Reimage);
          if (elasticNodeList.Any<ElasticNode>())
            state.DeleteList.AddRange((IEnumerable<ElasticNode>) elasticNodeList);
        }
        else
          state.DeleteList.AddRange((IEnumerable<ElasticNode>) state.ReimageList);
      }
      if (state.DeleteList.Count > 0)
      {
        bool force = false;
        if ((state.StuckList.Count > 0 || state.RetryList.Count > 0) && requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolForceDelete"))
          force = true;
        if (await elasticPoolSizingJob.TryDeleteNodesAsync(requestContext, elasticPool, state.DeleteList, state.ComputeSize, true, sb, force))
          actions.Add(ElasticPoolSizingJobBase.SizingAction.SizeDown);
      }
      List<ElasticPoolSizingJobBase.SizingAction> sizingActionList = actions;
      actions = (List<ElasticPoolSizingJobBase.SizingAction>) null;
      return sizingActionList;
    }

    protected async Task<List<ElasticNode>> TryReimageVMs(
      IVssRequestContext requestContext,
      StringBuilder sb,
      ElasticPool elasticPool,
      List<ElasticNode> nodes,
      PoolState state)
    {
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      IAzureVirtualMachineScaleSetResourceServiceInternal ssrs = requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
      await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.Reimage, ElasticResources.ReimagingMachines((object) nodes.Count));
      List<ElasticNode> nodesWithCompute = nodes;
      List<ElasticNode> collection = new List<ElasticNode>();
      if (requestContext.IsFeatureEnabled("DistributedTask.SkipActionsForNotExistingCompute"))
      {
        nodesWithCompute = nodes.Where<ElasticNode>((Func<ElasticNode, bool>) (node => node.ComputeExists)).ToList<ElasticNode>();
        collection = nodes.Where<ElasticNode>((Func<ElasticNode, bool>) (node => !node.ComputeExists)).ToList<ElasticNode>();
      }
      List<ElasticNode> failedToReimageNodes = new List<ElasticNode>((IEnumerable<ElasticNode>) collection);
      List<ElasticNode> reimagedNodes = new List<ElasticNode>();
      nodesWithCompute = await this.FilterOutAndDeleteExpiredVMsAsync(requestContext, elasticPool, sb, nodesWithCompute, state);
      int num;
      try
      {
        if (!nodesWithCompute.Any<ElasticNode>())
          return failedToReimageNodes;
        sb.AppendLine(string.Format("Reimaging VMs:{0}", (object) nodesWithCompute.Count));
        if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolFlexibleOrchestrationReimageBatch"))
        {
          await ssrs.ReimageMachinesAsync(requestContext, elasticPool, nodesWithCompute.Select<ElasticNode, string>((Func<ElasticNode, string>) (node => node.ComputeId)).ToHashSet<string>());
          sb.AppendLine(string.Format("Reimaged :{0}", (object) nodesWithCompute.Count));
          reimagedNodes.AddRange((IEnumerable<ElasticNode>) nodesWithCompute);
        }
        else
        {
          IAzureVirtualMachineResourceService vmsrs = requestContext.GetService<IAzureVirtualMachineResourceService>();
          foreach (ElasticNode node in nodesWithCompute)
          {
            try
            {
              await vmsrs.ReimageMachineAsync(requestContext, elasticPool, node.ComputeId);
              sb.AppendLine("Reimaged :" + node.ComputeId);
              reimagedNodes.Add(node);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "DistributedTask", "ElasticPoolSizingJob", ex);
              failedToReimageNodes.Add(node);
            }
          }
          vmsrs = (IAzureVirtualMachineResourceService) null;
        }
        if (reimagedNodes.Count > 0)
        {
          StringBuilder sb1 = new StringBuilder();
          sb1.AppendLine(string.Format("ElasticPool Id:{0} Reimage Nodes", (object) elasticPool.PoolId));
          foreach (ElasticNode node in reimagedNodes)
            node.TransitionState(ElasticNodeState.ReimagingCompute, sb1);
          requestContext.TraceAlways(10015202, TraceLevel.Info, "DistributedTask", "ElasticPools", sb1.ToString());
          IReadOnlyList<ElasticNode> elasticNodeList = await requestContext.GetService<IElasticNodeService>().UpdateElasticNodesAsync(requestContext, elasticPool.PoolId, (IList<ElasticNode>) reimagedNodes);
        }
        return failedToReimageNodes;
      }
      catch (CloudException ex)
      {
        num = 1;
      }
      if (num == 1)
      {
        CloudException e = ex;
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.Reimage, ElasticResources.ReimagingMachinesError((object) e.Message));
        requestContext.TraceException(0, "DistributedTask", "ElasticPoolSizingJob", (Exception) e);
        sb.AppendLine("ReimageMachine failed. " + e.Message);
        e = (CloudException) null;
      }
      return nodes.ToList<ElasticNode>();
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

    private async Task<List<ElasticNode>> FilterOutAndDeleteExpiredVMsAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb,
      List<ElasticNode> nodes,
      PoolState state)
    {
      FlexibleElasticPoolSizingJob elasticPoolSizingJob = this;
      if (!requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolFlexibleOrchestrationRefreshVMs") || !nodes.Any<ElasticNode>((Func<ElasticNode, bool>) (node =>
      {
        bool? needUpgrade = node.NeedUpgrade;
        bool flag = true;
        return needUpgrade.GetValueOrDefault() == flag & needUpgrade.HasValue;
      })))
        return nodes;
      try
      {
        IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
        requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
        List<ElasticNode> nodesToDelete = nodes.Where<ElasticNode>((Func<ElasticNode, bool>) (node =>
        {
          if (node == null)
            return false;
          bool? needUpgrade = node.NeedUpgrade;
          bool flag = true;
          return needUpgrade.GetValueOrDefault() == flag & needUpgrade.HasValue;
        })).ToList<ElasticNode>();
        int capacity = state.ComputeSize;
        if (capacity > 0)
        {
          elasticPoolSizingJob.DeleteAgents(requestContext, elasticPool, nodesToDelete, sb);
          if (await elasticPoolSizingJob.TryRecreateCompute(requestContext, nodesToDelete, elasticPool, state, sb))
          {
            await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.Reimage, ElasticResources.RefreshingExpiredMachines((object) nodesToDelete.Count));
            if (state.DesiredSize > capacity)
            {
              await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.IncreaseCapacity, ElasticResources.IncreasingCapacity((object) state.ComputeSize, (object) state.DesiredSize));
              elasticPoolSizingJob.PublishResizeEvent(requestContext, elasticPool, state.ComputeSize + nodesToDelete.Count<ElasticNode>(), state.DesiredSize);
            }
            return nodes.Where<ElasticNode>((Func<ElasticNode, bool>) (node => !nodesToDelete.Contains(node))).ToList<ElasticNode>();
          }
          requestContext.TraceAlways(10015196, TraceLevel.Warning, "DistributedTask", "ElasticPools", "Failed to recreate VMs.");
          return nodes;
        }
        els = (IElasticPoolLogService) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "DistributedTask", "ElasticPoolConfigurationJob", ex);
        sb.AppendLine(ex.ToString());
      }
      return nodes;
    }

    private async Task<bool> TryRecreateCompute(
      IVssRequestContext requestContext,
      List<ElasticNode> nodesToDelete,
      ElasticPool elasticPool,
      PoolState state,
      StringBuilder sb)
    {
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      int num;
      try
      {
        IAzureVirtualMachineScaleSetResourceServiceInternal ssrs = requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
        sb.AppendLine(string.Format("Recreating {0} expired machines.", (object) nodesToDelete.Count));
        await ssrs.DeleteMachinesAsync(requestContext, elasticPool, nodesToDelete.Select<ElasticNode, string>((Func<ElasticNode, string>) (node => node.ComputeId)), false);
        requestContext.TraceAlways(10015196, TraceLevel.Info, "DistributedTask", "ElasticPools", "Deleted " + string.Join(",", nodesToDelete.Select<ElasticNode, string>((Func<ElasticNode, string>) (node => node.ComputeId + "($" + node.Name + ")"))) + " machines.");
        foreach (ElasticNode node in nodesToDelete)
          node.TransitionState(ElasticNodeState.DeletingCompute, sb);
        IReadOnlyList<ElasticNode> elasticNodeList = await requestContext.GetService<IElasticNodeService>().UpdateElasticNodesAsync(requestContext, elasticPool.PoolId, (IList<ElasticNode>) nodesToDelete);
        int newCapacity = !requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolUpdateCapacityToComputeSizeAfterRecreation") || state.DesiredSize >= state.ComputeSize ? state.DesiredSize : state.ComputeSize;
        sb.AppendLine(string.Format("Increasing capacity from {0} to {1}", (object) state.ComputeSize, (object) newCapacity));
        await ssrs.SetCapacityAsync(requestContext, elasticPool, newCapacity);
        requestContext.TraceAlways(10015196, TraceLevel.Info, "DistributedTask", "ElasticPools", string.Format("Updating capacity after refresh. New capacity - {0}", (object) state.DesiredSize));
        return true;
      }
      catch (Exception ex)
      {
        num = 1;
      }
      if (num == 1)
      {
        Exception ex = ex;
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.Reimage, ex.Message);
        requestContext.TraceException("ElasticPools", ex);
        sb.AppendLine("Failed to recreate compute. " + ex.Message + ".");
        return false;
      }
      els = (IElasticPoolLogService) null;
      bool flag;
      return flag;
    }

    private async Task<bool> TryDeleteUnhealthyNodesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      List<ElasticNode> nodeCandidates,
      StringBuilder sb,
      bool force)
    {
      FlexibleElasticPoolSizingJob elasticPoolSizingJob = this;
      if (nodeCandidates.Count == 0 || !requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDeleteUnhealthyVms"))
        return true;
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      IElasticNodeService ens = requestContext.GetService<IElasticNodeService>();
      List<ElasticNode> source = elasticPoolSizingJob.DeleteAgents(requestContext, elasticPool, nodeCandidates, sb);
      List<ElasticNode> deletedVmssVm = new List<ElasticNode>();
      List<ElasticNode> deletedVm = new List<ElasticNode>();
      List<ElasticNode> list = source.Where<ElasticNode>((Func<ElasticNode, bool>) (node => node.ComputeState == ElasticComputeState.UnhealthyVmssVm)).ToList<ElasticNode>();
      List<ElasticNode> vms = source.Where<ElasticNode>((Func<ElasticNode, bool>) (node => node.ComputeState == ElasticComputeState.UnhealthyVm)).ToList<ElasticNode>();
      if (list.Any<ElasticNode>())
        deletedVmssVm = await elasticPoolSizingJob.DeleteVmssVmAsync(requestContext, elasticPool, list, sb, force);
      if (vms.Any<ElasticNode>())
        deletedVm = await elasticPoolSizingJob.DeleteVmAsync(requestContext, elasticPool, vms, sb, force);
      List<ElasticNode> deleted = new List<ElasticNode>();
      deleted.AddRange((IEnumerable<ElasticNode>) deletedVmssVm);
      deleted.AddRange((IEnumerable<ElasticNode>) deletedVm);
      if (deleted.Count > 0)
      {
        StringBuilder transitions = new StringBuilder();
        transitions.AppendLine(string.Format("ElasticPool Id:{0} Delete unhealthy Nodes", (object) elasticPool.PoolId));
        deleted.ForEach((Action<ElasticNode>) (node => node.TransitionState(ElasticNodeState.DeletingCompute, transitions)));
        requestContext.TraceAlways(10015202, TraceLevel.Info, "DistributedTask", "ElasticPools", transitions.ToString());
        IReadOnlyList<ElasticNode> elasticNodeList = await ens.UpdateElasticNodesAsync(requestContext, elasticPool.PoolId, (IList<ElasticNode>) deleted);
      }
      if (deletedVmssVm.Count > 0)
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Warning, OperationType.DeleteVMs, string.Format("Deleting {0} unhealthy machines. Please validate scale set configuration.", (object) deletedVmssVm.Count));
      if (deletedVm.Count > 0)
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Warning, OperationType.DeleteVMs, ElasticResources.DeletingUnhealthyMachines((object) deletedVmssVm.Count));
      return deleted.Count > 0;
    }
  }
}
