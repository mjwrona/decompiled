// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeploymentPoolExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Server.Hubs;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class DeploymentPoolExtension : ITaskAgentPoolExtension
  {
    public void AgentAdded(IVssRequestContext requestContext, int poolId, TaskAgent agent)
    {
      IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
      ITaskAgentPoolHubDispatcher poolHubDispatcher = poolRequestContext.GetService<ITaskAgentPoolHubDispatcher>();
      DeploymentPoolExtension.SwallowException(poolRequestContext, (Action) (() => poolHubDispatcher.NotifyAgentAdded(poolRequestContext, poolId, agent)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentPoolExtension.NotifyAllCollections<TaskAgent>(requestContext, poolId, agent, DeploymentPoolExtension.\u003C\u003EO.\u003C0\u003E__NotifyDeploymentMachineAdded ?? (DeploymentPoolExtension.\u003C\u003EO.\u003C0\u003E__NotifyDeploymentMachineAdded = new Action<IVssRequestContext, int, TaskAgent>(DeploymentPoolExtension.NotifyDeploymentMachineAdded)));
    }

    public void AgentConnected(IVssRequestContext requestContext, int poolId, int agentId)
    {
      IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
      ITaskAgentPoolHubDispatcher poolHubDispatcher = poolRequestContext.GetService<ITaskAgentPoolHubDispatcher>();
      DeploymentPoolExtension.SwallowException(poolRequestContext, (Action) (() => poolHubDispatcher.NotifyAgentConnected(poolRequestContext, poolId, agentId)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentPoolExtension.NotifyAllCollections<int>(requestContext, poolId, agentId, DeploymentPoolExtension.\u003C\u003EO.\u003C1\u003E__NotifyDeploymentMachineConnected ?? (DeploymentPoolExtension.\u003C\u003EO.\u003C1\u003E__NotifyDeploymentMachineConnected = new Action<IVssRequestContext, int, int>(DeploymentPoolExtension.NotifyDeploymentMachineConnected)));
    }

    public void AgentDeleted(IVssRequestContext requestContext, int poolId, int agentId)
    {
      IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
      ITaskAgentPoolHubDispatcher poolHubDispatcher = poolRequestContext.GetService<ITaskAgentPoolHubDispatcher>();
      DeploymentPoolExtension.SwallowException(poolRequestContext, (Action) (() => poolHubDispatcher.NotifyAgentDeleted(poolRequestContext, poolId, agentId)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentPoolExtension.NotifyAllCollections<int>(requestContext, poolId, agentId, DeploymentPoolExtension.\u003C\u003EO.\u003C2\u003E__NotifyDeploymentMachineDeleted ?? (DeploymentPoolExtension.\u003C\u003EO.\u003C2\u003E__NotifyDeploymentMachineDeleted = new Action<IVssRequestContext, int, int>(DeploymentPoolExtension.NotifyDeploymentMachineDeleted)));
    }

    public void AgentDisconnected(IVssRequestContext requestContext, int poolId, int agentId)
    {
      IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
      ITaskAgentPoolHubDispatcher poolHubDispatcher = poolRequestContext.GetService<ITaskAgentPoolHubDispatcher>();
      DeploymentPoolExtension.SwallowException(poolRequestContext, (Action) (() => poolHubDispatcher.NotifyAgentDisconnected(poolRequestContext, poolId, agentId)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentPoolExtension.NotifyAllCollections<int>(requestContext, poolId, agentId, DeploymentPoolExtension.\u003C\u003EO.\u003C3\u003E__NotifyDeploymentMachineDisconnected ?? (DeploymentPoolExtension.\u003C\u003EO.\u003C3\u003E__NotifyDeploymentMachineDisconnected = new Action<IVssRequestContext, int, int>(DeploymentPoolExtension.NotifyDeploymentMachineDisconnected)));
    }

    public void AgentRequestAssigned(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
      ITaskAgentPoolHubDispatcher poolHubDispatcher = poolRequestContext.GetService<ITaskAgentPoolHubDispatcher>();
      DeploymentPoolExtension.SwallowException(poolRequestContext, (Action) (() => poolHubDispatcher.NotifyAgentRequestAssigned(poolRequestContext, poolId, request)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentPoolExtension.NotifyRequestEvent(requestContext, poolId, request, DeploymentPoolExtension.\u003C\u003EO.\u003C4\u003E__NotifyDeploymentMachineRequestAssigned ?? (DeploymentPoolExtension.\u003C\u003EO.\u003C4\u003E__NotifyDeploymentMachineRequestAssigned = new Action<IVssRequestContext, int, TaskAgentJobRequest>(DeploymentPoolExtension.NotifyDeploymentMachineRequestAssigned)));
    }

    public void AgentRequestCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
      ITaskAgentPoolHubDispatcher poolHubDispatcher = poolRequestContext.GetService<ITaskAgentPoolHubDispatcher>();
      DeploymentPoolExtension.SwallowException(poolRequestContext, (Action) (() => poolHubDispatcher.NotifyAgentRequestCompleted(poolRequestContext, poolId, request)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentPoolExtension.NotifyRequestEvent(requestContext, poolId, request, DeploymentPoolExtension.\u003C\u003EO.\u003C5\u003E__NotifyDeploymentMachineRequestCompleted ?? (DeploymentPoolExtension.\u003C\u003EO.\u003C5\u003E__NotifyDeploymentMachineRequestCompleted = new Action<IVssRequestContext, int, TaskAgentJobRequest>(DeploymentPoolExtension.NotifyDeploymentMachineRequestCompleted)));
    }

    public void AgentRequestQueued(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
      ITaskAgentPoolHubDispatcher poolHubDispatcher = poolRequestContext.GetService<ITaskAgentPoolHubDispatcher>();
      DeploymentPoolExtension.SwallowException(poolRequestContext, (Action) (() => poolHubDispatcher.NotifyAgentRequestQueued(poolRequestContext, poolId, request)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentPoolExtension.NotifyRequestEvent(requestContext, poolId, request, DeploymentPoolExtension.\u003C\u003EO.\u003C6\u003E__NotifyDeploymentMachineRequestQueued ?? (DeploymentPoolExtension.\u003C\u003EO.\u003C6\u003E__NotifyDeploymentMachineRequestQueued = new Action<IVssRequestContext, int, TaskAgentJobRequest>(DeploymentPoolExtension.NotifyDeploymentMachineRequestQueued)));
    }

    public void AgentRequestStarted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
      ITaskAgentPoolHubDispatcher poolHubDispatcher = poolRequestContext.GetService<ITaskAgentPoolHubDispatcher>();
      DeploymentPoolExtension.SwallowException(poolRequestContext, (Action) (() => poolHubDispatcher.NotifyAgentRequestStarted(poolRequestContext, poolId, request)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentPoolExtension.NotifyRequestEvent(requestContext, poolId, request, DeploymentPoolExtension.\u003C\u003EO.\u003C7\u003E__NotifyDeploymentMachineRequestStarted ?? (DeploymentPoolExtension.\u003C\u003EO.\u003C7\u003E__NotifyDeploymentMachineRequestStarted = new Action<IVssRequestContext, int, TaskAgentJobRequest>(DeploymentPoolExtension.NotifyDeploymentMachineRequestStarted)));
    }

    public void AgentUpdated(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent,
      TaskAgent agentBeforeUpdate = null)
    {
      IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
      ITaskAgentPoolHubDispatcher poolHubDispatcher = poolRequestContext.GetService<ITaskAgentPoolHubDispatcher>();
      DeploymentPoolExtension.SwallowException(poolRequestContext, (Action) (() => poolHubDispatcher.NotifyAgentUpdated(poolRequestContext, poolId, agent)));
      bool? enabled = agent.Enabled;
      bool flag = false;
      if (!(enabled.GetValueOrDefault() == flag & enabled.HasValue) || agentBeforeUpdate == null || !agentBeforeUpdate.Enabled.HasValue || !agentBeforeUpdate.Enabled.Value)
        return;
      List<TaskAgentJobRequest> source = new List<TaskAgentJobRequest>();
      using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        source.AddRange((IEnumerable<TaskAgentJobRequest>) component.GetAgentRequestsForAgent(poolId, agent.Id, 0));
      if (!source.Any<TaskAgentJobRequest>((Func<TaskAgentJobRequest, bool>) (req => req.MatchedAgents.All<TaskAgentReference>((Func<TaskAgentReference, bool>) (a => !a.Enabled.HasValue || !a.Enabled.Value)))))
        return;
      requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        TaskConstants.DeploymentPoolMonitorJobJob
      }, (int) TimeSpan.FromMinutes(30.0).TotalSeconds);
    }

    public void PoolMaintenanceCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob)
    {
    }

    public void PoolMaintenanceDetailUpdated(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob)
    {
    }

    public void PoolMaintenanceQueued(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob)
    {
    }

    public void PoolMaintenanceStarted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob)
    {
    }

    public void ResourceUsageUpdated(IVssRequestContext requestContext, ResourceUsage usage)
    {
    }

    public void CheckIfPoolCanBeDeleted(IVssRequestContext requestContext, int poolId)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        TeamFoundationHostManagementService service = vssRequestContext.GetService<TeamFoundationHostManagementService>();
        foreach (TeamFoundationServiceHostProperties serviceHostProperties in service.QueryServiceHostProperties(vssRequestContext, requestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.IncludeChildren).Children.Where<TeamFoundationServiceHostProperties>((Func<TeamFoundationServiceHostProperties, bool>) (x => x.HostType == TeamFoundationHostType.ProjectCollection)))
        {
          using (IVssRequestContext requestContext1 = service.BeginRequest(vssRequestContext, serviceHostProperties.Id, RequestContextType.SystemContext, true, true))
            DeploymentPoolExtension.ValidateIfPoolIsInUse(requestContext1, poolId);
        }
      }
      else
        DeploymentPoolExtension.ValidateIfPoolIsInUse(requestContext, poolId);
    }

    public IList<TaskAgent> GetFilteredAgents(
      IList<TaskAgent> demandMatchingAgents,
      List<TaskAgentReference> candidateAgents)
    {
      HashSet<int> candidateAgentIds = new HashSet<int>(candidateAgents.Select<TaskAgentReference, int>((Func<TaskAgentReference, int>) (x => x.Id)));
      return (IList<TaskAgent>) demandMatchingAgents.Where<TaskAgent>((Func<TaskAgent, bool>) (x =>
      {
        if (x.Status == TaskAgentStatus.Online)
        {
          bool? enabled = x.Enabled;
          if (enabled.HasValue)
          {
            enabled = x.Enabled;
            bool flag = true;
            if (enabled.GetValueOrDefault() == flag & enabled.HasValue)
              return candidateAgentIds.Contains(x.Id);
          }
        }
        return false;
      })).ToList<TaskAgent>();
    }

    public bool DefaultAutoProvision => false;

    private static void NotifyAllCollections<T>(
      IVssRequestContext requestContext,
      int poolId,
      T agentData,
      Action<IVssRequestContext, int, T> notifyEvent)
    {
      try
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          requestContext.GetService<IDeploymentGroupHubDispatcher>();
          IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
          TeamFoundationHostManagementService service = vssRequestContext1.GetService<TeamFoundationHostManagementService>();
          foreach (TeamFoundationServiceHostProperties serviceHostProperties in service.QueryServiceHostProperties(vssRequestContext1, requestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.IncludeChildren).Children.Where<TeamFoundationServiceHostProperties>((Func<TeamFoundationServiceHostProperties, bool>) (x => x.HostType == TeamFoundationHostType.ProjectCollection)))
          {
            using (IVssRequestContext vssRequestContext2 = service.BeginRequest(vssRequestContext1, serviceHostProperties.Id, RequestContextType.SystemContext, true, true))
              notifyEvent(vssRequestContext2, poolId, agentData);
          }
        }
        else
          notifyEvent(requestContext, poolId, agentData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015125, "Notification", ex);
      }
    }

    private static void NotifyRequestEvent(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request,
      Action<IVssRequestContext, int, TaskAgentJobRequest> notifyEvent)
    {
      try
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
          using (IVssRequestContext vssRequestContext2 = vssRequestContext1.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext1, request.HostId, RequestContextType.SystemContext))
            notifyEvent(vssRequestContext2, poolId, request);
        }
        else
          notifyEvent(requestContext, poolId, request);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015125, "Notification", ex);
      }
    }

    private static void NotifyDeploymentMachineRequestAssigned(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      IList<DeploymentGroup> deploymentGroupsByPoolId = DeploymentPoolExtension.GetDeploymentGroupsByPoolId(requestContext, poolId);
      DeploymentGroup deploymentGroup = deploymentGroupsByPoolId != null ? deploymentGroupsByPoolId.Where<DeploymentGroup>((Func<DeploymentGroup, bool>) (dg => dg.Project.Id == request.ScopeId)).FirstOrDefault<DeploymentGroup>() : (DeploymentGroup) null;
      if (deploymentGroup == null)
        return;
      requestContext.GetService<IDeploymentGroupHubDispatcher>().NotifyAgentRequestAssigned(requestContext, request.ScopeId, deploymentGroup.Id, request);
    }

    private static void NotifyDeploymentMachineRequestStarted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      IList<DeploymentGroup> deploymentGroupsByPoolId = DeploymentPoolExtension.GetDeploymentGroupsByPoolId(requestContext, poolId);
      DeploymentGroup deploymentGroup = deploymentGroupsByPoolId != null ? deploymentGroupsByPoolId.Where<DeploymentGroup>((Func<DeploymentGroup, bool>) (dg => dg.Project.Id == request.ScopeId)).FirstOrDefault<DeploymentGroup>() : (DeploymentGroup) null;
      if (deploymentGroup == null)
        return;
      requestContext.GetService<IDeploymentGroupHubDispatcher>().NotifyAgentRequestStarted(requestContext, request.ScopeId, deploymentGroup.Id, request);
    }

    private static void NotifyDeploymentMachineRequestQueued(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      IList<DeploymentGroup> deploymentGroupsByPoolId = DeploymentPoolExtension.GetDeploymentGroupsByPoolId(requestContext, poolId);
      DeploymentGroup deploymentGroup = deploymentGroupsByPoolId != null ? deploymentGroupsByPoolId.Where<DeploymentGroup>((Func<DeploymentGroup, bool>) (dg => dg.Project.Id == request.ScopeId)).FirstOrDefault<DeploymentGroup>() : (DeploymentGroup) null;
      if (deploymentGroup == null)
        return;
      requestContext.GetService<IDeploymentGroupHubDispatcher>().NotifyAgentRequestQueued(requestContext, request.ScopeId, deploymentGroup.Id, request);
    }

    private static void NotifyDeploymentMachineRequestCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      IList<DeploymentGroup> deploymentGroupsByPoolId = DeploymentPoolExtension.GetDeploymentGroupsByPoolId(requestContext, poolId);
      DeploymentGroup deploymentGroup = deploymentGroupsByPoolId != null ? deploymentGroupsByPoolId.Where<DeploymentGroup>((Func<DeploymentGroup, bool>) (dg => dg.Project.Id == request.ScopeId)).FirstOrDefault<DeploymentGroup>() : (DeploymentGroup) null;
      if (deploymentGroup == null)
        return;
      requestContext.GetService<IDeploymentGroupHubDispatcher>().NotifyAgentRequestCompleted(requestContext, request.ScopeId, deploymentGroup.Id, request);
    }

    private static void NotifyDeploymentMachineAdded(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent)
    {
      DeploymentMachine machine = new DeploymentMachine();
      machine.Id = agent.Id;
      machine.Agent = agent;
      IList<DeploymentGroup> deploymentGroupsByPoolId = DeploymentPoolExtension.GetDeploymentGroupsByPoolId(requestContext, poolId);
      IDeploymentGroupHubDispatcher service = requestContext.GetService<IDeploymentGroupHubDispatcher>();
      foreach (DeploymentGroup deploymentGroup in (IEnumerable<DeploymentGroup>) deploymentGroupsByPoolId)
        service.NotifyDeploymentMachineAdded(requestContext, deploymentGroup.Project.Id, deploymentGroup.Id, machine);
    }

    private static void NotifyDeploymentMachineDeleted(
      IVssRequestContext requestContext,
      int poolId,
      int agentId)
    {
      IList<DeploymentGroup> deploymentGroupsByPoolId = DeploymentPoolExtension.GetDeploymentGroupsByPoolId(requestContext, poolId);
      IDeploymentGroupHubDispatcher service = requestContext.GetService<IDeploymentGroupHubDispatcher>();
      foreach (DeploymentGroup deploymentGroup in (IEnumerable<DeploymentGroup>) deploymentGroupsByPoolId)
        service.NotifyDeploymentMachineDeleted(requestContext, deploymentGroup.Project.Id, deploymentGroup.Id, agentId);
    }

    private static void NotifyDeploymentMachineConnected(
      IVssRequestContext requestContext,
      int poolId,
      int agentId)
    {
      IList<DeploymentGroup> deploymentGroupsByPoolId = DeploymentPoolExtension.GetDeploymentGroupsByPoolId(requestContext, poolId);
      IDeploymentGroupHubDispatcher service = requestContext.GetService<IDeploymentGroupHubDispatcher>();
      foreach (DeploymentGroup deploymentGroup in (IEnumerable<DeploymentGroup>) deploymentGroupsByPoolId)
        service.NotifyDeploymentMachineConnected(requestContext, deploymentGroup.Project.Id, deploymentGroup.Id, agentId);
    }

    private static void NotifyDeploymentMachineDisconnected(
      IVssRequestContext requestContext,
      int poolId,
      int agentId)
    {
      IList<DeploymentGroup> deploymentGroupsByPoolId = DeploymentPoolExtension.GetDeploymentGroupsByPoolId(requestContext, poolId);
      IDeploymentGroupHubDispatcher service = requestContext.GetService<IDeploymentGroupHubDispatcher>();
      foreach (DeploymentGroup deploymentGroup in (IEnumerable<DeploymentGroup>) deploymentGroupsByPoolId)
        service.NotifyDeploymentMachineDisconnected(requestContext, deploymentGroup.Project.Id, deploymentGroup.Id, agentId);
    }

    private static IList<DeploymentGroup> GetDeploymentGroupsByPoolId(
      IVssRequestContext requestContext,
      int poolId)
    {
      using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        return component.GetAgentQueuesByPoolIds((IEnumerable<int>) new int[1]
        {
          poolId
        }, TaskAgentQueueType.Deployment).MachineGroups;
    }

    private static void ValidateIfPoolIsInUse(IVssRequestContext requestContext, int poolId)
    {
      IList<Guid> first = (IList<Guid>) new List<Guid>();
      using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        first = (IList<Guid>) component.GetAgentQueuesByPoolIds((IEnumerable<int>) new int[1]
        {
          poolId
        }, TaskAgentQueueType.Deployment).MachineGroups.Select<DeploymentGroup, Guid>((Func<DeploymentGroup, Guid>) (dg => dg.Project.Id)).ToList<Guid>();
      IEnumerable<ProjectInfo> projects = requestContext.GetService<IProjectService>().GetProjects(requestContext);
      IEnumerable<Guid> second = projects.Where<ProjectInfo>((Func<ProjectInfo, bool>) (p => !p.State.Equals((object) ProjectState.Deleted))).Select<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (x => x.Id));
      List<Guid> validReferencedProjectIds = first.Intersect<Guid>(second).ToList<Guid>();
      int count = validReferencedProjectIds.Count;
      if (count > 0)
      {
        string poolName = DeploymentPoolExtension.GetPoolName(requestContext, poolId);
        string name = projects.Where<ProjectInfo>((Func<ProjectInfo, bool>) (p => p.Id == validReferencedProjectIds[0])).First<ProjectInfo>().Name;
        throw new DeploymentPoolInUseException(count == 1 ? TaskResources.DeploymentPoolInUseSingleCountMessage((object) poolName, (object) name) : TaskResources.DeploymentPoolInUseMultipleCountMessage((object) poolName, (object) name, (object) (count - 1)));
      }
    }

    private static string GetPoolName(IVssRequestContext requestContext, int poolId)
    {
      using (TaskResourceComponent component = requestContext.ToPoolRequestContext().CreateComponent<TaskResourceComponent>())
      {
        TaskAgentPoolData agentPool = component.GetAgentPool(poolId);
        return agentPool != null && agentPool.Pool != null ? agentPool.Pool.Name : string.Empty;
      }
    }

    private static void SwallowException(IVssRequestContext requestContext, Action action)
    {
      try
      {
        action();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015014, "Notification", ex);
      }
    }
  }
}
