// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticPoolSizingJobBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models;
using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public class ElasticPoolSizingJobBase : VssAsyncJobExtension
  {
    private const string NotActiveVMInstance = "is not an active Virtual Machine Scale Set VM instanceId";
    private const string InstanceId = "instanceId ";
    private const string InstanceIdErrorMessageRegEx = "instanceId \\d+";
    private const string CannotBeFound = "cannot be found";
    private const string VMInstance = "VM Instance ";
    private const string VMInstanceErrorMessageRegEx = "VM Instance \\d+";

    protected virtual OrchestrationType OrchestrationType { get; set; }

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      StringBuilder sb = new StringBuilder();
      TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
      string jobMessage = "Success";
      requestContext.GetService<IVssRegistryService>();
      foreach (ElasticPool elasticPool in (IEnumerable<ElasticPool>) await requestContext.GetService<IElasticPoolService>().GetElasticPoolsByTypeAsync(requestContext, this.OrchestrationType))
      {
        if (elasticPool.State == ElasticPoolState.Online || elasticPool.State == ElasticPoolState.Unhealthy)
        {
          try
          {
            sb.AppendLine(string.Format("ElasticPool Id:{0}, DesiredIdle:{1} MaxCapacity:{2} RecycleAfterEachUse:{3}, OSType:{4}, AzureId:{5}", (object) elasticPool.PoolId, (object) elasticPool.DesiredIdle, (object) elasticPool.MaxCapacity, (object) elasticPool.RecycleAfterEachUse, (object) elasticPool.OsType, (object) elasticPool.AzureId));
            PoolState state = await PoolState.GetStateAsync(requestContext, elasticPool, sb);
            if (state.IsValid)
            {
              List<ElasticPoolSizingJobBase.SizingAction> actions = await this.AdjustSize(requestContext, elasticPool, state, sb);
              this.DisableAgents(requestContext, elasticPool, state.SavedList, sb);
              await this.UpdateElasticPool(requestContext, elasticPool, state, actions, sb);
            }
            else
              sb.AppendLine("Skipping sizing job for offline pool.");
            state = (PoolState) null;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "DistributedTask", "ElasticPoolSizingJob", ex);
            sb.AppendLine(ex.ToString());
            result = TeamFoundationJobExecutionResult.Failed;
            jobMessage = sb.ToString();
          }
          requestContext.TraceAlways(10015196, TraceLevel.Info, "DistributedTask", "ElasticPools", sb.ToString());
          sb.Clear();
        }
      }
      VssJobResult vssJobResult = new VssJobResult(result, jobMessage);
      sb = (StringBuilder) null;
      jobMessage = (string) null;
      return vssJobResult;
    }

    protected virtual Task<List<ElasticPoolSizingJobBase.SizingAction>> AdjustSize(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      PoolState state,
      StringBuilder sb)
    {
      throw new NotImplementedException();
    }

    protected virtual void DisableAgents(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      List<ElasticNode> nodes,
      StringBuilder sb)
    {
      foreach (ElasticNode node in nodes)
      {
        int? agentId1 = node.AgentId;
        if (agentId1.HasValue)
        {
          if (node.AgentState.IsEnabled())
          {
            try
            {
              IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
              IVssRequestContext requestContext1 = requestContext;
              int poolId = elasticPool.PoolId;
              agentId1 = node.AgentId;
              int agentId2 = agentId1.Value;
              TaskAgent agent = service.GetAgent(requestContext1, poolId, agentId2);
              agent.Enabled = new bool?(false);
              service.UpdateAgent(requestContext, elasticPool.PoolId, agent);
              sb.AppendLine(string.Format("Disabled agent {0} on node {1}", (object) node.AgentId, (object) node.Name));
            }
            catch
            {
              sb.AppendLine(string.Format("Error: Failed to disable agent {0} on saved node {1}", (object) node.AgentId, (object) node.Name));
            }
          }
        }
      }
    }

    protected virtual async Task UpdateElasticPool(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      PoolState state,
      List<ElasticPoolSizingJobBase.SizingAction> actions,
      StringBuilder sb)
    {
      bool flag = false;
      NullableElasticPool elasticPool1 = new NullableElasticPool(elasticPool.PoolId);
      if (actions.Contains(ElasticPoolSizingJobBase.SizingAction.SizeUp))
      {
        elasticPool1.SizingAttempts = new int?(state.WasSizeUpSuccessful ? 1 : elasticPool.SizingAttempts + 1);
        flag = true;
        sb.AppendLine(string.Format("Setting SizingAttempts to {0}", (object) elasticPool1.SizingAttempts));
      }
      else if (state.WasSizeUpSuccessful && elasticPool.SizingAttempts > 0)
      {
        elasticPool1.SizingAttempts = new int?(0);
        flag = true;
        sb.AppendLine("Setting SizingAttempts to 0");
      }
      if (state.DesiredSize != elasticPool.DesiredSize)
      {
        elasticPool1.DesiredSize = new int?(state.DesiredSize);
        flag = true;
        sb.AppendLine(string.Format("Setting DesiredSize to {0}", (object) state.DesiredSize));
      }
      if (!flag)
        return;
      ElasticPool elasticPool2 = await requestContext.GetService<IElasticPoolService>().UpdateElasticPoolAsync(requestContext, elasticPool1);
    }

    protected virtual async Task<bool> TryIncreaseCapacity(
      IVssRequestContext requestContext,
      StringBuilder sb,
      ElasticPool elasticPool,
      PoolState state)
    {
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      int num;
      try
      {
        IAzureVirtualMachineScaleSetResourceServiceInternal service = requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
        sb.AppendLine(string.Format("Increasing capacity from {0} to {1}. Retry count: {2}", (object) state.ComputeSize, (object) state.DesiredSize, (object) state.RetryList.Count));
        IVssRequestContext requestContext1 = requestContext;
        ElasticPool elasticPool1 = elasticPool;
        int desiredSize = state.DesiredSize;
        await service.SetCapacityAsync(requestContext1, elasticPool1, desiredSize);
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.IncreaseCapacity, ElasticResources.IncreasingCapacity((object) state.ComputeSize, (object) state.DesiredSize));
        this.PublishResizeEvent(requestContext, elasticPool, state.ComputeSize, state.DesiredSize);
        return true;
      }
      catch (CloudException ex)
      {
        num = 1;
      }
      if (num == 1)
      {
        CloudException e = ex;
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.IncreaseCapacity, ElasticResources.IncreasingCapacityError((object) e.Message));
        sb.AppendLine("Failed to increase capacity. " + e.Message);
        e = (CloudException) null;
      }
      return false;
    }

    protected virtual async Task<bool> TryDeleteNodesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      List<ElasticNode> nodeCandidates,
      int currentSize,
      bool canChangeScaleSet,
      StringBuilder sb,
      bool force)
    {
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      int deletedIdleCount = 0;
      int deletedUnhealthyCount = 0;
      List<ElasticNode> deleted = new List<ElasticNode>();
      List<ElasticNode> source = this.DeleteAgents(requestContext, elasticPool, nodeCandidates, sb);
      List<ElasticNode> computeExistsList = source;
      List<ElasticNode> noComputeList = new List<ElasticNode>();
      if (requestContext.IsFeatureEnabled("DistributedTask.SkipActionsForNotExistingCompute"))
      {
        noComputeList = source.Where<ElasticNode>((Func<ElasticNode, bool>) (node => !node.ComputeExists)).ToList<ElasticNode>();
        computeExistsList = source.Where<ElasticNode>((Func<ElasticNode, bool>) (node => node.ComputeExists)).ToList<ElasticNode>();
      }
      if (canChangeScaleSet && computeExistsList.Count > 0)
      {
        foreach (ElasticNode node in computeExistsList)
          await this.ExtractUnhealthyNodeDetailsAsync(requestContext, elasticPool, node, sb);
        deleted = await this.DeleteVmssVmAsync(requestContext, elasticPool, computeExistsList, sb, force);
        if (deleted.Count > 0)
        {
          StringBuilder sb1 = new StringBuilder();
          sb1.AppendLine(string.Format("ElasticPool Id:{0} Delete Nodes", (object) elasticPool.PoolId));
          foreach (ElasticNode node in deleted)
          {
            if (node.State == ElasticNodeState.Idle || node.State == ElasticNodeState.PendingReimage)
              ++deletedIdleCount;
            else
              ++deletedUnhealthyCount;
            node.TransitionState(ElasticNodeState.DeletingCompute, sb1);
          }
          requestContext.TraceAlways(10015202, TraceLevel.Info, "DistributedTask", "ElasticPools", sb1.ToString());
          IReadOnlyList<ElasticNode> elasticNodeList = await requestContext.GetService<IElasticNodeService>().UpdateElasticNodesAsync(requestContext, elasticPool.PoolId, (IList<ElasticNode>) deleted);
        }
        if (deletedIdleCount > 0)
        {
          this.PublishResizeEvent(requestContext, elasticPool, currentSize, currentSize - deletedIdleCount);
          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Info, OperationType.DeleteVMs, ElasticResources.DecreasingCapacity((object) currentSize, (object) (currentSize - deletedIdleCount)));
        }
        if (deletedUnhealthyCount > 0)
          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Warning, OperationType.DeleteVMs, ElasticResources.DeletingUnhealthyMachines((object) deletedUnhealthyCount));
      }
      if (canChangeScaleSet && noComputeList.Any<ElasticNode>())
      {
        StringBuilder sb2 = new StringBuilder();
        sb2.AppendLine(string.Format("ElasticPool Id:{0} Delete Nodes with no compute.", (object) elasticPool.PoolId));
        foreach (ElasticNode node in noComputeList)
          node.TransitionState(ElasticNodeState.DeletingCompute, sb2);
        requestContext.TraceAlways(10015202, TraceLevel.Info, "DistributedTask", "ElasticPools", sb2.ToString());
        IReadOnlyList<ElasticNode> elasticNodeList = await requestContext.GetService<IElasticNodeService>().UpdateElasticNodesAsync(requestContext, elasticPool.PoolId, (IList<ElasticNode>) noComputeList);
        requestContext.TraceAlways(10015196, TraceLevel.Info, "DistributedTask", "ElasticPools", string.Format("Deleteing {0} nodes with no compute.", (object) noComputeList.Count));
      }
      bool flag = deleted.Count > 0 || noComputeList.Count > 0;
      els = (IElasticPoolLogService) null;
      deleted = (List<ElasticNode>) null;
      computeExistsList = (List<ElasticNode>) null;
      noComputeList = (List<ElasticNode>) null;
      return flag;
    }

    protected virtual async Task ExtractUnhealthyNodeDetailsAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      ElasticNode node,
      StringBuilder sb)
    {
      if (node.ComputeState == ElasticComputeState.Stopped)
      {
        await requestContext.GetService<IElasticPoolLogService>().AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.SizingJob, ElasticResources.VirtualMachineStopped((object) node.Name));
      }
      else
      {
        if (node.ComputeId == null || (node.ComputeState != ElasticComputeState.Healthy || node.AgentId.HasValue) && node.State != ElasticNodeState.FailedToRestartPendingDelete)
          return;
        IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
        try
        {
          StringBuilder details = new StringBuilder();
          details.AppendLine("Tracing extension details for unhealthy Node " + node.Name + ", ComputeId " + node.ComputeId);
          IAzureVirtualMachineScaleSetResourceServiceInternal service = requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
          VirtualMachineScaleSetVMInstanceView instanceViewAsync;
          if (elasticPool.OrchestrationType == OrchestrationType.Flexible)
            instanceViewAsync = await requestContext.GetService<IAzureVirtualMachineResourceService>().GetInstanceViewAsync(requestContext, elasticPool, node.ComputeId);
          else
            instanceViewAsync = await service.GetInstanceViewAsync(requestContext, elasticPool, node.ComputeId);
          if (instanceViewAsync.Extensions != null)
          {
            VirtualMachineExtensionInstanceView[] extensionInstanceViewArray = instanceViewAsync.Extensions;
            for (int index1 = 0; index1 < extensionInstanceViewArray.Length; ++index1)
            {
              VirtualMachineExtensionInstanceView extension = extensionInstanceViewArray[index1];
              if (!string.IsNullOrEmpty(extension.Type))
              {
                bool isPipelinesExtension = extension.Type.Contains("TeamServicesAgent");
                bool isCustomScriptExtension = extension.Type.Contains("CustomScript");
                if (isPipelinesExtension | isCustomScriptExtension)
                {
                  details.AppendLine("Extension type " + extension.Type);
                  if (extension.Statuses != null)
                  {
                    InstanceViewStatus[] instanceViewStatusArray = extension.Statuses;
                    for (int index2 = 0; index2 < instanceViewStatusArray.Length; ++index2)
                    {
                      InstanceViewStatus status = instanceViewStatusArray[index2];
                      if (string.Equals(status.Level, "Error", StringComparison.OrdinalIgnoreCase))
                      {
                        if (isPipelinesExtension)
                          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.SizingJob, ElasticResources.AzurePipelinesExtensionMessage((object) status.Message));
                        else if (isCustomScriptExtension)
                          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.SizingJob, ElasticResources.CustomScriptExtensionMessage((object) status.Message));
                      }
                      string str = status.Message;
                      if (isCustomScriptExtension && str != null)
                        str = str.Substring(0, Math.Min(100, str.Length));
                      details.AppendLine(string.Format("Status: {0}, {1}, {2}, {3}, {4}", (object) status.Time, (object) status.Code, (object) status.Level, (object) status.DisplayStatus, (object) str));
                      status = (InstanceViewStatus) null;
                    }
                    instanceViewStatusArray = (InstanceViewStatus[]) null;
                  }
                  if (extension.Substatuses != null)
                  {
                    foreach (InstanceViewStatus substatuse in extension.Substatuses)
                      details.AppendLine("SubStatus: " + substatuse.Code + ", " + substatuse.Level + ", " + substatuse.DisplayStatus + ", " + substatuse.Message);
                  }
                }
              }
              extension = (VirtualMachineExtensionInstanceView) null;
            }
            extensionInstanceViewArray = (VirtualMachineExtensionInstanceView[]) null;
          }
          requestContext.TraceAlways(10015198, TraceLevel.Info, "DistributedTask", "ElasticPools", details.ToString());
          details = (StringBuilder) null;
        }
        catch (Exception ex)
        {
          await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.SizingJob, ElasticResources.VirtualMachineExtensionMessageError((object) ex.Message));
          requestContext.TraceAlways(10015198, TraceLevel.Info, "DistributedTask", "ElasticPools", string.Format("Failed to extract extension details for {0}. {1}", (object) node.Name, (object) ex));
        }
        els = (IElasticPoolLogService) null;
      }
    }

    protected void PublishResizeEvent(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      int previousSize,
      int newSize)
    {
      TaskAgentPool agentPool = requestContext.GetService<IDistributedTaskResourceService>().GetAgentPool(requestContext, elasticPool.PoolId);
      requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyElasticPoolResizedEvent(requestContext, agentPool, elasticPool, previousSize, newSize);
    }

    protected List<ElasticNode> DeleteAgents(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      List<ElasticNode> nodeCandidates,
      StringBuilder sb)
    {
      List<ElasticNode> elasticNodeList = new List<ElasticNode>();
      IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
      foreach (ElasticNode nodeCandidate in nodeCandidates)
      {
        if (!nodeCandidate.AgentId.HasValue)
        {
          elasticNodeList.Add(nodeCandidate);
        }
        else
        {
          sb.AppendLine(string.Format("{0} deleting agent {1}. ComputeExists: {2}", (object) nodeCandidate.Name, (object) nodeCandidate.AgentId, (object) nodeCandidate.ComputeExists));
          try
          {
            service.DeleteAgents(requestContext, elasticPool.PoolId, (IEnumerable<int>) new int[1]
            {
              nodeCandidate.AgentId.Value
            });
            elasticNodeList.Add(nodeCandidate);
          }
          catch (TaskAgentJobStillRunningException ex)
          {
            sb.AppendLine("Failed to delete VM from DT. " + ex.Message + ".");
          }
        }
      }
      return elasticNodeList;
    }

    protected async Task<List<ElasticNode>> DeleteVmssVmAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      List<ElasticNode> nodeCandidates,
      StringBuilder sb,
      bool force)
    {
      IElasticPoolLogService els = requestContext.GetService<IElasticPoolLogService>();
      IAzureVirtualMachineScaleSetResourceServiceInternal ssrs = requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
      List<ElasticNode> deleted = new List<ElasticNode>();
      bool tooManyRequests = false;
      try
      {
        sb.AppendLine(string.Format("Deleting VMs:{0}", (object) nodeCandidates.Count));
        await ssrs.DeleteMachinesAsync(requestContext, elasticPool, nodeCandidates.Where<ElasticNode>((Func<ElasticNode, bool>) (n => n.ComputeId != null)).Select<ElasticNode, string>((Func<ElasticNode, string>) (n => n.ComputeId)), force);
        deleted.AddRange((IEnumerable<ElasticNode>) nodeCandidates);
      }
      catch (CloudException ex)
      {
        HttpResponseMessageWrapper response = ex.Response;
        tooManyRequests = response != null && response.StatusCode == (HttpStatusCode) 429;
        await els.AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.DeleteVMs, ElasticResources.DecreasingCapacityError((object) ex.Message));
        sb.AppendLine("Failed to delete VMs " + ex.Message);
      }
      if (deleted.Count == 0 && nodeCandidates.Count > 1 && !tooManyRequests)
      {
        foreach (ElasticNode node in nodeCandidates)
        {
          try
          {
            if (node.ComputeId != null)
            {
              sb.AppendLine(node.Name + " deleting compute " + node.ComputeId);
              await ssrs.DeleteMachinesAsync(requestContext, elasticPool, (IEnumerable<string>) new string[1]
              {
                node.ComputeId
              }, (force ? 1 : 0) != 0);
            }
            deleted.Add(node);
          }
          catch (CloudException ex)
          {
          }
        }
      }
      List<ElasticNode> elasticNodeList = deleted;
      els = (IElasticPoolLogService) null;
      ssrs = (IAzureVirtualMachineScaleSetResourceServiceInternal) null;
      deleted = (List<ElasticNode>) null;
      return elasticNodeList;
    }

    protected async Task<List<ElasticNode>> DeleteVmAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      List<ElasticNode> nodeCandidates,
      StringBuilder sb,
      bool force)
    {
      List<ElasticNode> deleted = new List<ElasticNode>();
      if (elasticPool.OrchestrationType != OrchestrationType.Flexible || !nodeCandidates.Any<ElasticNode>())
        return deleted;
      IAzureVirtualMachineResourceService vmsrs = requestContext.GetService<IAzureVirtualMachineResourceService>();
      bool tooManyRequests = false;
      foreach (ElasticNode nodeCandidate in nodeCandidates)
      {
        ElasticNode node = nodeCandidate;
        if (!tooManyRequests)
        {
          try
          {
            if (node.ComputeId != null)
            {
              sb.AppendLine(node.Name + " deleting compute " + node.ComputeId);
              await vmsrs.DeleteMachineAsync(requestContext, elasticPool, node.ComputeId, force);
            }
            deleted.Add(node);
          }
          catch (CloudException ex)
          {
            HttpResponseMessageWrapper response = ex.Response;
            tooManyRequests = response != null && response.StatusCode == (HttpStatusCode) 429;
            sb.AppendLine("Failed to delete VMs " + ex.Message);
          }
          node = (ElasticNode) null;
        }
        else
          break;
      }
      return deleted;
    }

    public enum SizingAction
    {
      None,
      SizeUp,
      SizeDown,
      Reimage,
    }
  }
}
