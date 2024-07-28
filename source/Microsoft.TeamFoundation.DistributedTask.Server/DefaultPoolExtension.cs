// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DefaultPoolExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Hubs;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class DefaultPoolExtension : ITaskAgentPoolExtension
  {
    public void AgentAdded(IVssRequestContext requestContext, int poolId, TaskAgent agent)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (AgentAdded)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyAgentAdded(requestContext, poolId, agent)));
        IPoolHubDispatcher queueHubDispatcher = requestContext.GetService<IPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => queueHubDispatcher.NotifyAgentAdded(requestContext, poolId, agent.Id)));
      }
    }

    public void AgentConnected(IVssRequestContext requestContext, int poolId, int agentId)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (AgentConnected)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyAgentConnected(requestContext, poolId, agentId)));
        IPoolHubDispatcher queueHubDispatcher = requestContext.GetService<IPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => queueHubDispatcher.NotifyAgentConnected(requestContext, poolId, agentId)));
      }
    }

    public void AgentDeleted(IVssRequestContext requestContext, int poolId, int agentId)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (AgentDeleted)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyAgentDeleted(requestContext, poolId, agentId)));
        IPoolHubDispatcher queueHubDispatcher = requestContext.GetService<IPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => queueHubDispatcher.NotifyAgentDeleted(requestContext, poolId, agentId)));
      }
    }

    public void AgentDisconnected(IVssRequestContext requestContext, int poolId, int agentId)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (AgentDisconnected)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyAgentDisconnected(requestContext, poolId, agentId)));
        IPoolHubDispatcher queueHubDispatcher = requestContext.GetService<IPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => queueHubDispatcher.NotifyAgentDisconnected(requestContext, poolId, agentId)));
      }
    }

    public void AgentRequestAssigned(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (AgentRequestAssigned)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        IPoolsHubDispatcher projectPoolHubDispatcher = requestContext.GetService<IPoolsHubDispatcher>();
        IPoolHubDispatcher queueHubDispatcher = requestContext.GetService<IPoolHubDispatcher>();
        IAgentHubDispatcher agentHubDispatcher = requestContext.GetService<IAgentHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyAgentRequestAssigned(requestContext, poolId, request)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => projectPoolHubDispatcher.NotifyAgentRequestUpdated(requestContext, request)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => queueHubDispatcher.NotifyAgentRequestAssigned(requestContext, poolId, request.RequestId)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => agentHubDispatcher.NotifyAgentRequestUpdated(requestContext, poolId, request)));
      }
    }

    public void AgentRequestCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (AgentRequestCompleted)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        IPoolsHubDispatcher projectPoolHubDispatcher = requestContext.GetService<IPoolsHubDispatcher>();
        IPoolHubDispatcher queueHubDispatcher = requestContext.GetService<IPoolHubDispatcher>();
        IAgentHubDispatcher agentHubDispatcher = requestContext.GetService<IAgentHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyAgentRequestCompleted(requestContext, poolId, request)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => projectPoolHubDispatcher.NotifyAgentRequestUpdated(requestContext, request)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => queueHubDispatcher.NotifyAgentRequestCompleted(requestContext, poolId, request.RequestId)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => agentHubDispatcher.NotifyAgentRequestUpdated(requestContext, poolId, request)));
      }
    }

    public void AgentRequestQueued(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (AgentRequestQueued)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        IPoolsHubDispatcher projectPoolHubDispatcher = requestContext.GetService<IPoolsHubDispatcher>();
        IPoolHubDispatcher queueHubDispatcher = requestContext.GetService<IPoolHubDispatcher>();
        IAgentHubDispatcher agentHubDispatcher = requestContext.GetService<IAgentHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyAgentRequestQueued(requestContext, poolId, request)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => projectPoolHubDispatcher.NotifyAgentRequestUpdated(requestContext, request)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => queueHubDispatcher.NotifyAgentRequestQueued(requestContext, poolId, request.RequestId)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => agentHubDispatcher.NotifyAgentRequestUpdated(requestContext, poolId, request)));
      }
    }

    public void AgentRequestStarted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (AgentRequestStarted)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        IPoolsHubDispatcher projectPoolHubDispatcher = requestContext.GetService<IPoolsHubDispatcher>();
        IPoolHubDispatcher queueHubDispatcher = requestContext.GetService<IPoolHubDispatcher>();
        IAgentHubDispatcher agentHubDispatcher = requestContext.GetService<IAgentHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyAgentRequestStarted(requestContext, poolId, request)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => projectPoolHubDispatcher.NotifyAgentRequestUpdated(requestContext, request)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => queueHubDispatcher.NotifyAgentRequestStarted(requestContext, poolId, request.RequestId)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => agentHubDispatcher.NotifyAgentRequestUpdated(requestContext, poolId, request)));
      }
    }

    public void AgentUpdated(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent,
      TaskAgent agentBeforeUpdate = null)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (AgentUpdated)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        IPoolHubDispatcher queueHubDispatcher = requestContext.GetService<IPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyAgentUpdated(requestContext, poolId, agent)));
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => queueHubDispatcher.NotifyAgentUpdated(requestContext, poolId, agent.Id)));
      }
    }

    public void PoolMaintenanceCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (PoolMaintenanceCompleted)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyPoolMaintenanceCompleted(requestContext, poolId, maintenanceJob)));
      }
    }

    public void PoolMaintenanceDetailUpdated(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (PoolMaintenanceDetailUpdated)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyPoolMaintenanceDetailUpdated(requestContext, poolId, maintenanceJob)));
      }
    }

    public void PoolMaintenanceQueued(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (PoolMaintenanceQueued)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyPoolMaintenanceQueued(requestContext, poolId, maintenanceJob)));
      }
    }

    public void PoolMaintenanceStarted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (PoolMaintenanceStarted)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyPoolMaintenanceStarted(requestContext, poolId, maintenanceJob)));
      }
    }

    public void ResourceUsageUpdated(IVssRequestContext requestContext, ResourceUsage usage)
    {
      using (new MethodScope(requestContext, nameof (DefaultPoolExtension), nameof (ResourceUsageUpdated)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        ITaskAgentPoolHubDispatcher poolHubDispatcher = requestContext.GetService<ITaskAgentPoolHubDispatcher>();
        DefaultPoolExtension.SwallowException(requestContext, (Action) (() => poolHubDispatcher.NotifyResourceUsageUpdated(requestContext, usage)));
      }
    }

    public void CheckIfPoolCanBeDeleted(IVssRequestContext requestContext, int poolId)
    {
    }

    public IList<TaskAgent> GetFilteredAgents(
      IList<TaskAgent> demandMatchingAgents,
      List<TaskAgentReference> candidateAgents)
    {
      HashSet<int> candidateAgentIds = new HashSet<int>(candidateAgents.Select<TaskAgentReference, int>((Func<TaskAgentReference, int>) (x => x.Id)));
      return (IList<TaskAgent>) demandMatchingAgents.Where<TaskAgent>((Func<TaskAgent, bool>) (x => candidateAgentIds.Contains(x.Id))).ToList<TaskAgent>();
    }

    public bool DefaultAutoProvision => true;

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
