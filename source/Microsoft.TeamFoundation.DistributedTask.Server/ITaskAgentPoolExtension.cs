// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.ITaskAgentPoolExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public interface ITaskAgentPoolExtension
  {
    void AgentAdded(IVssRequestContext requestContext, int poolId, TaskAgent agent);

    void AgentConnected(IVssRequestContext requestContext, int poolId, int agentId);

    void AgentDeleted(IVssRequestContext requestContext, int poolId, int agentId);

    void AgentDisconnected(IVssRequestContext requestContext, int poolId, int agentId);

    void AgentRequestAssigned(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request);

    void AgentRequestCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request);

    void AgentRequestQueued(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request);

    void AgentRequestStarted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request);

    void AgentUpdated(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent,
      TaskAgent agentBeforeUpdate = null);

    void PoolMaintenanceCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob);

    void PoolMaintenanceDetailUpdated(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob);

    void PoolMaintenanceQueued(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob);

    void PoolMaintenanceStarted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob);

    void ResourceUsageUpdated(IVssRequestContext requestContext, ResourceUsage usage);

    void CheckIfPoolCanBeDeleted(IVssRequestContext requestContext, int poolId);

    IList<TaskAgent> GetFilteredAgents(
      IList<TaskAgent> demandMatchingAgents,
      List<TaskAgentReference> candidateAgents);

    bool DefaultAutoProvision { get; }
  }
}
