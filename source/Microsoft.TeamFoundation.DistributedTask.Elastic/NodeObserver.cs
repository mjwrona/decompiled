// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.NodeObserver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public class NodeObserver
  {
    public Dictionary<string, ElasticNode> AllNodes { get; private set; }

    public bool IsValid { get; private set; }

    public int ComputeSize { get; private set; }

    public int QueuedRequestCount { get; private set; }

    public bool IsSizeUpJustDone { get; private set; }

    public static async Task<NodeObserver> GetNodesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      NodeObserver nodes = new NodeObserver();
      await nodes.PopulateAsync(requestContext, elasticPool, sb);
      NodeObserver nodesAsync = nodes;
      nodes = (NodeObserver) null;
      return nodesAsync;
    }

    private NodeObserver()
    {
      this.QueuedRequestCount = 0;
      this.AllNodes = new Dictionary<string, ElasticNode>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private async Task PopulateAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      VirtualMachineScaleSet vmss = (VirtualMachineScaleSet) null;
      int num = 0;
      object obj;
      try
      {
        vmss = await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().GetScaleSetAsync(requestContext, elasticPool);
        if (!vmss.Tags.ContainsKey("__AzureDevOpsElasticPool"))
        {
          sb.AppendLine(string.Format("The Scaleset with AzureId {0} is not tagged as in use by any pool. Tagging it for ElasticPool Id: {1}.", (object) elasticPool.AzureId, (object) elasticPool.PoolId));
          vmss = await requestContext.GetService<IElasticPoolService>().TagElasticPoolAsync(requestContext, elasticPool, scaleSet: vmss);
        }
        this.ComputeSize = vmss.Sku.Capacity;
      }
      catch (Exception ex)
      {
        obj = (object) ex;
        num = 1;
      }
      if (num == 1)
      {
        Exception exception = (Exception) obj;
        sb.AppendLine("Get Scaleset failed. " + exception.Message);
        this.IsValid = false;
        await requestContext.GetService<IElasticPoolLogService>().AddElasticPoolLogAsync(requestContext, elasticPool.PoolId, Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel.Error, OperationType.SizingJob, exception.Message);
        vmss = (VirtualMachineScaleSet) null;
      }
      else
      {
        obj = (object) null;
        try
        {
          await this.ReadNodesAsync(requestContext, elasticPool, sb);
          await this.InspectScaleSetAsync(requestContext, elasticPool, vmss, sb);
          this.InspectAgentPool(requestContext, elasticPool, sb);
          await this.TransitionStates(requestContext, elasticPool, sb);
          await this.SaveNodesAsync(requestContext, elasticPool, sb);
          this.IsValid = true;
          vmss = (VirtualMachineScaleSet) null;
        }
        catch (Exception ex)
        {
          this.IsValid = false;
          requestContext.TraceException(0, "DistributedTask", "ElasticPoolSizingJob", ex);
          sb.AppendLine(ex.ToString());
          vmss = (VirtualMachineScaleSet) null;
        }
      }
    }

    private async Task ReadNodesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      IReadOnlyList<ElasticNode> elasticNodesAsync = await requestContext.GetService<IElasticNodeService>().GetElasticNodesAsync(requestContext, elasticPool.PoolId);
      elasticPool.SavedNodeCount = 0;
      foreach (ElasticNode elasticNode in (IEnumerable<ElasticNode>) elasticNodesAsync)
      {
        this.AllNodes.Add(elasticNode.Name, elasticNode);
        if (elasticNode.State == ElasticNodeState.Saved)
          ++elasticPool.SavedNodeCount;
      }
    }

    private async Task InspectScaleSetAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      VirtualMachineScaleSet vmss,
      StringBuilder sb)
    {
      int creating = 0;
      int reimaging = 0;
      int healthy = 0;
      int deleting = 0;
      int failed = 0;
      int stopped = 0;
      int stuck = 0;
      int unhealthy = 0;
      IVssRegistryService registry = requestContext.GetService<IVssRegistryService>();
      IAzureVirtualMachineScaleSetResourceServiceInternal ssrs = requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
      IAzureVirtualMachineResourceService service = requestContext.GetService<IAzureVirtualMachineResourceService>();
      IReadOnlyList<VirtualMachineScaleSetVM> vmList;
      if (elasticPool.OrchestrationType == OrchestrationType.Flexible)
        vmList = await service.ListVMInstancesAsync(requestContext, elasticPool);
      else
        vmList = await ssrs.ListVMInstancesAsync(requestContext, elasticPool);
      if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolRetryDelete"))
        this.ComputeSize = Math.Max(this.ComputeSize, vmList.Count);
      bool flag1 = requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolFlexibleOrchestrationRefreshVMs");
      int num = registry.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/VMExpirationInHours", 46);
      foreach (VirtualMachineScaleSetVM machineScaleSetVm in (IEnumerable<VirtualMachineScaleSetVM>) vmList)
      {
        string key = machineScaleSetVm.Properties?.OSProfile?.ComputerName ?? "Unknown VM" + machineScaleSetVm.InstanceId;
        ElasticNode elasticNode = (ElasticNode) null;
        if (!this.AllNodes.TryGetValue(key, out elasticNode))
        {
          elasticNode = new ElasticNode()
          {
            PoolId = elasticPool.PoolId,
            Name = key,
            ComputeId = machineScaleSetVm.InstanceId ?? machineScaleSetVm.Name,
            State = ElasticNodeState.New
          };
          this.AllNodes.Add(key, elasticNode);
        }
        elasticNode.ComputeExists = true;
        ElasticComputeState elasticComputeState = ElasticComputeState.None;
        if (flag1 && machineScaleSetVm != null)
        {
          string s;
          bool? nullable = machineScaleSetVm.Tags?.TryGetValue("__AzureDevOpsElasticPoolTimeStamp", out s);
          bool flag2 = true;
          DateTime result;
          if (nullable.GetValueOrDefault() == flag2 & nullable.HasValue && DateTime.TryParse(s, out result))
            elasticNode.NeedUpgrade = new bool?(result < DateTime.UtcNow.AddHours((double) -num));
        }
        if (machineScaleSetVm.Properties.IsStopped())
        {
          elasticComputeState = ElasticComputeState.Stopped;
          sb.AppendLine(string.Format("Error: VM {0}, InstanceId {1} is not running. PowerState: {2}", (object) key, (object) machineScaleSetVm.InstanceId, (object) machineScaleSetVm.Properties.GetPowerState()));
          ++stopped;
        }
        else if (elasticNode.State == ElasticNodeState.AssignedPendingDelete)
        {
          elasticComputeState = elasticNode.ComputeState;
          ++stuck;
        }
        else if (machineScaleSetVm.Properties.ProvisioningState == ProvisioningState.Creating || machineScaleSetVm.Properties.ProvisioningState == ProvisioningState.Updating || machineScaleSetVm.Properties.ProvisioningState == ProvisioningState.OSProvisioningInProgress || machineScaleSetVm.Properties.ProvisioningState == ProvisioningState.OSProvisioningComplete)
        {
          if (elasticNode.State == ElasticNodeState.ReimagingCompute || elasticNode.State == ElasticNodeState.PendingDelete)
          {
            elasticComputeState = ElasticComputeState.Reimaging;
            ++reimaging;
          }
          else
          {
            elasticComputeState = ElasticComputeState.Creating;
            ++creating;
          }
        }
        else if (machineScaleSetVm.Properties.ProvisioningState == ProvisioningState.Succeeded)
        {
          elasticComputeState = ElasticComputeState.Healthy;
          ++healthy;
        }
        else if (machineScaleSetVm.Properties.ProvisioningState == ProvisioningState.Deleting)
        {
          elasticComputeState = ElasticComputeState.Deleting;
          ++deleting;
        }
        else if (machineScaleSetVm.Properties.ProvisioningState == ProvisioningState.Failed)
        {
          elasticComputeState = ElasticComputeState.Failed;
          ++failed;
        }
        else
          sb.AppendLine(string.Format("ERROR: Unknown ProvisioningState:{0} for VM:{1}, InstanceId{2}", (object) machineScaleSetVm.Properties.ProvisioningState, (object) key, (object) machineScaleSetVm.InstanceId));
        if (vmss.OrchestrationType() == OrchestrationType.Flexible && !machineScaleSetVm.Equals((object) vmss))
        {
          elasticNode.State = ElasticNodeState.PendingDelete;
          elasticNode.ComputeChangeLog = string.Format("{0}->{1}", (object) elasticNode.ComputeState, (object) ElasticNodeState.PendingDelete);
          elasticNode.ComputeState = elasticComputeState;
        }
        else if (elasticComputeState != elasticNode.ComputeState)
        {
          elasticNode.ComputeChangeLog = string.Format("{0}->{1}", (object) elasticNode.ComputeState, (object) elasticComputeState);
          elasticNode.ComputeState = elasticComputeState;
        }
        else
          elasticNode.ComputeChangeLog = string.Format("{0}", (object) elasticNode.ComputeState);
      }
      if (elasticPool.OrchestrationType == OrchestrationType.Flexible && requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDeleteUnhealthyVms"))
      {
        IReadOnlyList<VirtualMachineScaleSetVM> ssvmList = await ssrs.ListVMsAsync(requestContext, elasticPool);
        IEnumerable<VirtualMachineScaleSetVM> machineScaleSetVms1 = vmList.Where<VirtualMachineScaleSetVM>((Func<VirtualMachineScaleSetVM, bool>) (x => !ssvmList.Any<VirtualMachineScaleSetVM>((Func<VirtualMachineScaleSetVM, bool>) (y => x.Name == y.Name))));
        IEnumerable<VirtualMachineScaleSetVM> machineScaleSetVms2 = ssvmList.Where<VirtualMachineScaleSetVM>((Func<VirtualMachineScaleSetVM, bool>) (x => !vmList.Any<VirtualMachineScaleSetVM>((Func<VirtualMachineScaleSetVM, bool>) (y => x.Name == y.Name))));
        if (machineScaleSetVms1.Any<VirtualMachineScaleSetVM>())
          unhealthy += this.UpdateComputeStateForUnhealthyVms(machineScaleSetVms1, ElasticComputeState.UnhealthyVm, elasticPool);
        if (machineScaleSetVms2.Any<VirtualMachineScaleSetVM>())
          unhealthy += this.UpdateComputeStateForUnhealthyVms(machineScaleSetVms2, ElasticComputeState.UnhealthyVmssVm, elasticPool);
      }
      string str = string.Format("Scaleset Capacity:{0}, VMCount:{1}, Healthy:{2}, Creating:{3}, Reimaging:{4}, Deleting:{5}, Failed:{6}, Stopped:{7}", (object) vmss.Sku.Capacity, (object) vmList.Count, (object) healthy, (object) creating, (object) reimaging, (object) deleting, (object) failed, (object) stopped);
      if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDeleteStuckVM"))
        str += string.Format(", Stuck:{0}", (object) stuck);
      if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDeleteUnhealthyVms"))
        str += string.Format(", Unhealthy: {0}", (object) unhealthy);
      sb.AppendLine(str);
      registry = (IVssRegistryService) null;
      ssrs = (IAzureVirtualMachineScaleSetResourceServiceInternal) null;
    }

    private int UpdateComputeStateForUnhealthyVms(
      IEnumerable<VirtualMachineScaleSetVM> vms,
      ElasticComputeState unhealthyState,
      ElasticPool elasticPool)
    {
      int stateForUnhealthyVms = 0;
      foreach (VirtualMachineScaleSetVM vm in vms)
      {
        string key = vm.Properties?.OSProfile?.ComputerName ?? vm.Name ?? "Unknown VM" + vm.InstanceId;
        ElasticNode elasticNode;
        if (this.AllNodes.TryGetValue(key, out elasticNode))
        {
          elasticNode.ComputeState = unhealthyState;
          elasticNode.ComputeChangeLog = string.Format("{0}->{1}", (object) elasticNode.ComputeState, (object) unhealthyState);
          elasticNode.ComputeExists = true;
        }
        else
        {
          elasticNode = new ElasticNode()
          {
            PoolId = elasticPool.PoolId,
            Name = key,
            ComputeId = vm.InstanceId ?? vm.Name,
            State = ElasticNodeState.New,
            ComputeExists = true,
            ComputeState = unhealthyState,
            ComputeChangeLog = string.Format("{0}", (object) unhealthyState)
          };
          this.AllNodes.Add(key, elasticNode);
        }
        ++stateForUnhealthyVms;
      }
      return stateForUnhealthyVms;
    }

    private void InspectAgentPool(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
      IList<TaskAgent> agents = service.GetAgents(requestContext, elasticPool.PoolId, includeAssignedRequest: true, includeLastCompletedRequest: true);
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      foreach (TaskAgent taskAgent in (IEnumerable<TaskAgent>) agents)
      {
        ElasticNode elasticNode1 = (ElasticNode) null;
        if (!this.AllNodes.TryGetValue(taskAgent.Name, out elasticNode1))
        {
          sb.AppendLine(string.Format("Found unexpected agent not in our scaleset. {0}, {1}", (object) taskAgent.Name, (object) taskAgent.Id));
          elasticNode1 = new ElasticNode()
          {
            PoolId = elasticPool.PoolId,
            Name = taskAgent.Name,
            AgentId = new int?(taskAgent.Id),
            State = ElasticNodeState.New
          };
          this.AllNodes.Add(elasticNode1.Name, elasticNode1);
        }
        elasticNode1.AgentExists = true;
        int? agentId = elasticNode1.AgentId;
        if (!agentId.HasValue)
        {
          elasticNode1.AgentId = new int?(taskAgent.Id);
        }
        else
        {
          agentId = elasticNode1.AgentId;
          int id = taskAgent.Id;
          if (!(agentId.GetValueOrDefault() == id & agentId.HasValue))
          {
            sb.AppendLine(string.Format("ERROR: AgentId does not match! {0}, expected: {1}, actual: {2}", (object) elasticNode1.Name, (object) elasticNode1.AgentId, (object) taskAgent.Id));
            elasticNode1.AgentId = new int?(taskAgent.Id);
          }
        }
        if (elasticNode1.ComputeState == ElasticComputeState.Healthy)
        {
          long? requestId = elasticNode1.RequestId;
          if (taskAgent.AssignedRequest != null)
            elasticNode1.RequestId = new long?(taskAgent.AssignedRequest.RequestId);
          else if (taskAgent.LastCompletedRequest != null)
          {
            elasticNode1.RequestId = new long?(taskAgent.LastCompletedRequest.RequestId);
            elasticNode1.LastAgentTaskResult = taskAgent.LastCompletedRequest.Result.GetValueOrDefault();
          }
          ElasticNode elasticNode2 = elasticNode1;
          long? nullable1;
          long? nullable2;
          if (taskAgent == null)
          {
            nullable1 = new long?();
            nullable2 = nullable1;
          }
          else
          {
            TaskAgentJobRequest completedRequest = taskAgent.LastCompletedRequest;
            if (completedRequest == null)
            {
              nullable1 = new long?();
              nullable2 = nullable1;
            }
            else
              nullable2 = new long?(completedRequest.RequestId);
          }
          elasticNode2.LastAgentRequestId = nullable2;
          ElasticNode elasticNode3 = elasticNode1;
          nullable1 = elasticNode1.RequestId;
          int num4;
          if (nullable1.HasValue)
          {
            nullable1 = elasticNode1.RequestId;
            long? nullable3 = requestId;
            num4 = !(nullable1.GetValueOrDefault() == nullable3.GetValueOrDefault() & nullable1.HasValue == nullable3.HasValue) ? 1 : 0;
          }
          else
            num4 = 0;
          elasticNode3.HasNewRequest = num4 != 0;
        }
        ElasticAgentState elasticAgentState = ElasticAgentState.None;
        bool? enabled = taskAgent.Enabled;
        bool flag = true;
        if (enabled.GetValueOrDefault() == flag & enabled.HasValue)
        {
          elasticAgentState |= ElasticAgentState.Enabled;
          ++num1;
        }
        if (taskAgent.Status == TaskAgentStatus.Online)
        {
          elasticAgentState |= ElasticAgentState.Online;
          ++num2;
        }
        if (taskAgent.AssignedRequest != null)
        {
          elasticAgentState |= ElasticAgentState.Assigned;
          ++num3;
        }
        if (elasticAgentState != elasticNode1.AgentState)
        {
          elasticNode1.AgentChangeLog = string.Format("{0} -> {1}", (object) elasticNode1.AgentState, (object) elasticAgentState);
          elasticNode1.AgentState = elasticAgentState;
        }
        else
          elasticNode1.AgentChangeLog = string.Format("{0}", (object) elasticNode1.AgentState);
        if (elasticNode1.HasNewRequest)
          elasticNode1.AgentChangeLog += string.Format(" RequestId: {0}", (object) elasticNode1.RequestId);
      }
      IList<TaskAgentPoolStatus> agentPoolStatusByIds = service.GetAgentPoolStatusByIds(requestContext, (IList<int>) new List<int>()
      {
        elasticPool.PoolId
      });
      if (agentPoolStatusByIds.Count > 0)
        this.QueuedRequestCount = agentPoolStatusByIds[0].QueuedRequestCount;
      sb.AppendLine(string.Format("Agent Pool: Count:{0}, Enabled:{1}, Online:{2}, Assigned:{3}, QueuedRequestCount:{4}", (object) agents.Count, (object) num1, (object) num2, (object) num3, (object) this.QueuedRequestCount));
    }

    private async Task TransitionStates(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder summarySb)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int maxStartTime = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/StartTimeMinutes", 10);
      int maxLostTime = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/LostTimeMinutes", 20);
      int maxOfflineTime = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/OfflineTimeMinutes", 20);
      int maxDeletingTime = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/DeletingTimeMinutes", 30);
      int maxCreatingTime = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/CreatingTimeMinutes", 45);
      int maxStartTimeForUnhealthyVm = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/StartTimeForUnhealthyVm", 10);
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(string.Format("ElasticPool Id:{0} Observed Node Transitions", (object) elasticPool.PoolId));
      int[] counts = new int[Enum.GetValues(typeof (ElasticNodeState)).Length];
      this.IsSizeUpJustDone = true;
      bool foundStartingAgentNode = false;
      foreach (ElasticNode node in this.AllNodes.Values)
      {
        if (!node.ComputeExists)
        {
          node.ComputeState = ElasticComputeState.None;
          if (!node.AgentExists)
          {
            node.TransitionState(ElasticNodeState.Deleted, sb);
            continue;
          }
        }
        do
        {
          if (node.State == ElasticNodeState.StartingAgent)
            foundStartingAgentNode = true;
          node.HasStateChanged = false;
          if (!requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDeleteUnhealthyVms"))
          {
            switch (node.State)
            {
              case ElasticNodeState.UnhealthyVm:
                node.TransitionState(ElasticNodeState.CreatingCompute);
                break;
              case ElasticNodeState.UnhealthyVmPendingDelete:
                node.TransitionState(ElasticNodeState.PendingDelete);
                break;
            }
          }
          int totalMinutes = (int) (DateTime.UtcNow - node.StateChangedOn).TotalMinutes;
          if (requestContext.IsFeatureEnabled("DistributedTask.MoveNodeToDeletingCompute") && node.ComputeState == ElasticComputeState.Deleting && node.State != ElasticNodeState.DeletingCompute && node.State != ElasticNodeState.Saved && node.State != ElasticNodeState.Deleted && node.State != ElasticNodeState.RetryDelete)
          {
            sb.AppendLine("Node " + node.Name + " is in the process of being deleted. Transitioning to DeletingCompute state.");
            node.TransitionState(ElasticNodeState.DeletingCompute, sb);
          }
          switch (node.State)
          {
            case ElasticNodeState.New:
              node.IsNew = true;
              if (node.ComputeState == ElasticComputeState.None)
              {
                node.TransitionState(ElasticNodeState.Lost, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.ComputeState == ElasticComputeState.UnhealthyVm || node.ComputeState == ElasticComputeState.UnhealthyVmssVm)
              {
                node.TransitionState(ElasticNodeState.UnhealthyVm, sb);
                goto case ElasticNodeState.Saved;
              }
              else
              {
                node.TransitionState(ElasticNodeState.CreatingCompute, sb);
                goto case ElasticNodeState.Saved;
              }
            case ElasticNodeState.CreatingCompute:
            case ElasticNodeState.ReimagingCompute:
              ElasticNodeState state1 = ElasticNodeState.FailedVMPendingDelete;
              if (node.State == ElasticNodeState.ReimagingCompute)
                state1 = ElasticNodeState.PendingDelete;
              ElasticNodeState state2 = ElasticNodeState.StartingAgent;
              if (node.State == ElasticNodeState.ReimagingCompute)
                state2 = ElasticNodeState.RestartingAgent;
              if (node.ComputeState == ElasticComputeState.Creating || node.ComputeState == ElasticComputeState.Reimaging)
              {
                if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolCreateTimeout") && totalMinutes > maxCreatingTime)
                {
                  sb.AppendLine("Node " + node.Name + " is stuck in creating. Transitioning to delete.");
                  node.TransitionState(state1, sb);
                  goto case ElasticNodeState.Saved;
                }
                else
                  goto case ElasticNodeState.Saved;
              }
              else if (node.ComputeState == ElasticComputeState.Failed || node.ComputeState == ElasticComputeState.Stopped)
              {
                node.TransitionState(state1, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.ComputeState == ElasticComputeState.Deleting)
              {
                node.TransitionState(ElasticNodeState.DeletingCompute, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.ComputeState == ElasticComputeState.Healthy)
              {
                node.TransitionState(state2, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.ComputeState == ElasticComputeState.None)
              {
                node.TransitionState(ElasticNodeState.Lost, sb);
                goto case ElasticNodeState.Saved;
              }
              else
              {
                sb.AppendLine(string.Format("ERROR: Unexpected ComputeState: {0}", (object) node.ComputeState));
                goto case ElasticNodeState.Saved;
              }
            case ElasticNodeState.StartingAgent:
            case ElasticNodeState.RestartingAgent:
              ElasticNodeState state3 = ElasticNodeState.FailedToStartPendingDelete;
              if (node.State == ElasticNodeState.RestartingAgent)
                state3 = ElasticNodeState.FailedToRestartPendingDelete;
              if (node.AgentState.IsAssigned())
              {
                node.TransitionState(ElasticNodeState.Assigned, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.AgentState.IsOnline())
              {
                node.TransitionState(ElasticNodeState.Idle, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (elasticPool.RecycleAfterEachUse && node.HasNewRequest)
              {
                node.TransitionState(ElasticNodeState.PendingReimage, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.ComputeState == ElasticComputeState.None)
              {
                node.TransitionState(ElasticNodeState.Lost, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (totalMinutes >= maxStartTime)
              {
                await node.TransitionOrSaveAsync(requestContext, elasticPool, state3, sb, true, ElasticResources.AgentFailedToStart((object) node.Name));
                goto case ElasticNodeState.Saved;
              }
              else if (node.State == ElasticNodeState.StartingAgent)
              {
                this.IsSizeUpJustDone = false;
                goto case ElasticNodeState.Saved;
              }
              else
                goto case ElasticNodeState.Saved;
            case ElasticNodeState.Idle:
              if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolIdleNodeWithoutAgent") && !node.AgentExists && totalMinutes >= maxStartTime)
              {
                await node.TransitionOrSaveAsync(requestContext, elasticPool, ElasticNodeState.PendingDelete, sb, true, ElasticResources.AgentWentOffline((object) node.Name));
                node.AgentState = ElasticAgentState.None;
                goto case ElasticNodeState.Saved;
              }
              else if (node.AgentState.IsAssigned())
              {
                node.TransitionState(ElasticNodeState.Assigned, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.AgentState.IsDisabled())
              {
                node.TransitionState(ElasticNodeState.PendingDelete, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.AgentState.IsOffline())
              {
                bool flag = requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolReimageVmForOfflineAgent") && node.LastAgentRequestId.HasValue;
                await node.TransitionOrSaveAsync(requestContext, elasticPool, ElasticNodeState.Offline, sb, !elasticPool.RecycleAfterEachUse || !(node.HasNewRequest | flag), ElasticResources.AgentWentOffline((object) node.Name));
                goto case ElasticNodeState.Saved;
              }
              else if (node.ComputeState == ElasticComputeState.None)
              {
                node.TransitionState(ElasticNodeState.Lost, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (!node.WasIdleResetted && !elasticPool.RecycleAfterEachUse && node.AgentState.IsOnline() && node.HasNewRequest)
              {
                node.TransitionState(ElasticNodeState.Idle, sb, forceIdle: true);
                node.WasIdleResetted = true;
                goto case ElasticNodeState.Saved;
              }
              else
                goto case ElasticNodeState.Saved;
            case ElasticNodeState.Assigned:
              if (!node.AgentState.IsAssigned() || !node.AgentState.IsOnline())
              {
                if (elasticPool.RecycleAfterEachUse)
                {
                  await node.TransitionOrSaveAsync(requestContext, elasticPool, ElasticNodeState.PendingReimage, sb, node.LastAgentTaskResult == TaskResult.Abandoned, ElasticResources.AgentWentOffline((object) node.Name));
                  goto case ElasticNodeState.Saved;
                }
                else if (node.AgentState.IsDisabled())
                {
                  node.TransitionState(ElasticNodeState.PendingDelete, sb);
                  goto case ElasticNodeState.Saved;
                }
                else if (node.AgentState.IsOffline())
                {
                  await node.TransitionOrSaveAsync(requestContext, elasticPool, ElasticNodeState.Offline, sb, true, ElasticResources.AgentWentOffline((object) node.Name));
                  goto case ElasticNodeState.Saved;
                }
                else
                {
                  node.TransitionState(ElasticNodeState.Idle, sb);
                  goto case ElasticNodeState.Saved;
                }
              }
              else
                goto case ElasticNodeState.Saved;
            case ElasticNodeState.Offline:
              if (node.AgentState.IsOnline())
              {
                node.TransitionState(ElasticNodeState.Idle, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.ComputeState == ElasticComputeState.None)
              {
                node.TransitionState(ElasticNodeState.Lost, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (elasticPool.RecycleAfterEachUse)
              {
                node.TransitionState(ElasticNodeState.PendingReimage, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.AgentState.IsDisabled())
              {
                node.TransitionState(ElasticNodeState.PendingDelete, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (totalMinutes >= maxOfflineTime)
              {
                node.TransitionState(ElasticNodeState.PendingDelete, sb);
                goto case ElasticNodeState.Saved;
              }
              else
                goto case ElasticNodeState.Saved;
            case ElasticNodeState.PendingReimage:
              if (node.AgentState.IsAssigned() && node.AgentState.IsOnline())
              {
                node.TransitionState(ElasticNodeState.Assigned, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.ComputeState == ElasticComputeState.None)
              {
                node.TransitionState(ElasticNodeState.PendingDelete, sb);
                goto case ElasticNodeState.Saved;
              }
              else
                goto case ElasticNodeState.Saved;
            case ElasticNodeState.PendingDelete:
            case ElasticNodeState.FailedVMPendingDelete:
              if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDeleteStuckVM") && node.ComputeState != ElasticComputeState.Healthy && node.ComputeState != ElasticComputeState.Failed && node.AgentState.IsAssigned())
              {
                if (node.ComputeState == ElasticComputeState.Creating)
                  this.IsSizeUpJustDone = false;
                node.TransitionState(ElasticNodeState.AssignedPendingDelete, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.AgentState.IsAssigned())
              {
                node.TransitionState(ElasticNodeState.Assigned, sb);
                goto case ElasticNodeState.Saved;
              }
              else
                goto case ElasticNodeState.Saved;
            case ElasticNodeState.Saved:
              continue;
            case ElasticNodeState.DeletingCompute:
              if (node.ComputeState == ElasticComputeState.None)
              {
                node.TransitionState(ElasticNodeState.Lost, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (totalMinutes >= maxDeletingTime)
              {
                if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolRetryDelete"))
                {
                  node.TransitionState(ElasticNodeState.RetryDelete, sb);
                  goto case ElasticNodeState.Saved;
                }
                else
                {
                  node.TransitionState(ElasticNodeState.PendingDelete, sb);
                  goto case ElasticNodeState.Saved;
                }
              }
              else
                goto case ElasticNodeState.Saved;
            case ElasticNodeState.Lost:
              if (node.AgentState != ElasticAgentState.None && node.ComputeState != ElasticComputeState.None)
              {
                node.TransitionState(ElasticNodeState.StartingAgent, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (totalMinutes >= maxLostTime)
              {
                node.TransitionState(ElasticNodeState.PendingDelete, sb);
                goto case ElasticNodeState.Saved;
              }
              else
                goto case ElasticNodeState.Saved;
            case ElasticNodeState.FailedToStartPendingDelete:
            case ElasticNodeState.FailedToRestartPendingDelete:
              if (node.AgentState.IsAssigned())
              {
                await node.TransitionStateAndLogAsync(requestContext, elasticPool, ElasticNodeState.Assigned, sb, ElasticResources.UnhealthyAgentCameOnline((object) node.Name));
                goto case ElasticNodeState.Saved;
              }
              else if (node.AgentState.IsOnline())
              {
                await node.TransitionStateAndLogAsync(requestContext, elasticPool, ElasticNodeState.Idle, sb, ElasticResources.UnhealthyAgentCameOnline((object) node.Name));
                goto case ElasticNodeState.Saved;
              }
              else if (elasticPool.RecycleAfterEachUse && node.HasNewRequest)
              {
                await node.TransitionStateAndLogAsync(requestContext, elasticPool, ElasticNodeState.PendingReimage, sb, ElasticResources.UnhealthyAgentCameOnline((object) node.Name));
                goto case ElasticNodeState.Saved;
              }
              else
                goto case ElasticNodeState.Saved;
            case ElasticNodeState.AssignedPendingDelete:
              if (!node.AgentState.IsAssigned())
              {
                if (node.ComputeState == ElasticComputeState.Creating)
                {
                  node.TransitionState(ElasticNodeState.FailedVMPendingDelete, sb);
                  goto case ElasticNodeState.Saved;
                }
                else
                {
                  node.TransitionState(ElasticNodeState.PendingDelete, sb);
                  goto case ElasticNodeState.Saved;
                }
              }
              else
                goto case ElasticNodeState.Saved;
            case ElasticNodeState.UnhealthyVm:
              if (node.ComputeState == ElasticComputeState.None)
              {
                node.TransitionState(ElasticNodeState.Lost, sb);
                goto case ElasticNodeState.Saved;
              }
              else if (node.ComputeState == ElasticComputeState.UnhealthyVm || node.ComputeState == ElasticComputeState.UnhealthyVmssVm)
              {
                if (totalMinutes > maxStartTimeForUnhealthyVm)
                {
                  node.TransitionState(ElasticNodeState.UnhealthyVmPendingDelete, sb);
                  goto case ElasticNodeState.Saved;
                }
                else
                  goto case ElasticNodeState.Saved;
              }
              else
              {
                node.TransitionState(ElasticNodeState.CreatingCompute, sb);
                goto case ElasticNodeState.Saved;
              }
            default:
              sb.AppendLine(string.Format("ERROR: Unexpected node state:{0}", (object) node.State));
              goto case ElasticNodeState.Saved;
          }
        }
        while (node.HasStateChanged);
        ++counts[(int) node.State];
      }
      this.IsSizeUpJustDone = foundStartingAgentNode && this.IsSizeUpJustDone;
      requestContext.TraceAlways(10015202, TraceLevel.Info, "DistributedTask", "ElasticPools", sb.ToString());
      summarySb.AppendLine(string.Format("Total Nodes: {0},", (object) this.AllNodes.Count) + string.Format(" None:{0},", (object) counts[0]) + string.Format(" New:{0},", (object) counts[1]) + string.Format(" CreatingCompute:{0},", (object) counts[2]) + string.Format(" ReimagingCompute:{0},", (object) counts[13]) + string.Format(" AssignedPendingDelete:{0}", (object) counts[18]) + string.Format(" StartingAgent:{0},", (object) counts[3]) + string.Format(" RestartingAgent:{0},", (object) counts[14]) + string.Format(" Idle:{0},", (object) counts[4]) + string.Format(" Assigned:{0},", (object) counts[5]) + string.Format(" Offline:{0},", (object) counts[6]) + string.Format(" PendingReimage:{0},", (object) counts[7]) + string.Format(" PendingDelete:{0},", (object) (counts[8] + counts[16] + counts[15] + counts[17] + counts[19])) + string.Format(" Saved:{0},", (object) counts[9]) + string.Format(" DeletingCompute:{0},", (object) counts[10]) + string.Format(" Deleted:{0},", (object) counts[11]) + string.Format(" Lost:{0}", (object) counts[12]) + string.Format(" Unhealthy:{0}", (object) (counts[20] + counts[21])));
      sb = (StringBuilder) null;
      counts = (int[]) null;
    }

    private async Task SaveNodesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      IElasticNodeService ens = requestContext.GetService<IElasticNodeService>();
      List<ElasticNode> nodes = new List<ElasticNode>();
      List<ElasticNode> deletedList = new List<ElasticNode>();
      List<ElasticNode> updatedList = new List<ElasticNode>();
      foreach (ElasticNode elasticNode in this.AllNodes.Values)
      {
        if (elasticNode.IsNew)
          nodes.Add(elasticNode);
        else if (elasticNode.State == ElasticNodeState.Deleted)
          deletedList.Add(elasticNode);
        else
          updatedList.Add(elasticNode);
      }
      if (nodes.Count > 0)
      {
        IReadOnlyList<ElasticNode> elasticNodeList1 = await ens.AddElasticNodesAsync(requestContext, elasticPool.PoolId, (IList<ElasticNode>) nodes);
      }
      if (deletedList.Count > 0)
      {
        await ens.DeleteElasticNodesAsync(requestContext, elasticPool.PoolId, (IList<ElasticNode>) deletedList);
        foreach (ElasticNode elasticNode in deletedList)
          this.AllNodes.Remove(elasticNode.Name);
      }
      if (updatedList.Count <= 0)
      {
        ens = (IElasticNodeService) null;
        deletedList = (List<ElasticNode>) null;
        updatedList = (List<ElasticNode>) null;
      }
      else
      {
        IReadOnlyList<ElasticNode> elasticNodeList2 = await ens.UpdateElasticNodesAsync(requestContext, elasticPool.PoolId, (IList<ElasticNode>) updatedList);
        ens = (IElasticNodeService) null;
        deletedList = (List<ElasticNode>) null;
        updatedList = (List<ElasticNode>) null;
      }
    }
  }
}
