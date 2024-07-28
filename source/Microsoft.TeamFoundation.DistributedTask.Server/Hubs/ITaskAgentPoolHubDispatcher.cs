// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Hubs.ITaskAgentPoolHubDispatcher
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Hubs
{
  [DefaultServiceImplementation(typeof (TaskAgentPoolHubDispatcher))]
  internal interface ITaskAgentPoolHubDispatcher : IVssFrameworkService
  {
    void NotifyAgentAdded(IVssRequestContext requestContext, int poolId, TaskAgent agent);

    void NotifyAgentConnected(IVssRequestContext requestContext, int poolId, int agentId);

    void NotifyAgentDeleted(IVssRequestContext requestContext, int poolId, int agentId);

    void NotifyAgentDisconnected(IVssRequestContext requestContext, int poolId, int agentId);

    void NotifyAgentRequestQueued(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request);

    void NotifyAgentRequestAssigned(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request);

    void NotifyAgentRequestStarted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request);

    void NotifyAgentRequestCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request);

    void NotifyAgentUpdated(IVssRequestContext requestContext, int poolId, TaskAgent agent);

    void NotifyPoolMaintenanceQueued(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob);

    void NotifyPoolMaintenanceStarted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob);

    void NotifyPoolMaintenanceCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob);

    void NotifyPoolMaintenanceDetailUpdated(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceJob maintenanceJob);

    void NotifyDeploymentMachinesUpdated(
      IVssRequestContext requestContext,
      int poolId,
      IList<DeploymentMachine> deploymentMachines);

    void NotifyResourceUsageUpdated(IVssRequestContext requestContext, ResourceUsage usage);

    Task Subscribe(IVssRequestContext requestContext, int poolId, string connectionId);

    Task Unsubscribe(IVssRequestContext requestContext, int poolId, string connectionId);
  }
}
