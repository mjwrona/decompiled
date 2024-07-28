// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Hubs.IDeploymentGroupHubDispatcher
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Hubs
{
  [DefaultServiceImplementation(typeof (DeploymentGroupHubDispatcher))]
  internal interface IDeploymentGroupHubDispatcher : IVssFrameworkService
  {
    void NotifyDeploymentMachineAdded(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      DeploymentMachine machine);

    void NotifyDeploymentMachineDeleted(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int machineId);

    void NotifyDeploymentMachinesUpdated(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<DeploymentMachine> deploymentMachines);

    void NotifyDeploymentMachineConnected(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int machineId);

    void NotifyDeploymentMachineDisconnected(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int machineId);

    void NotifyAgentRequestQueued(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      TaskAgentJobRequest request);

    void NotifyAgentRequestAssigned(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      TaskAgentJobRequest request);

    void NotifyAgentRequestStarted(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      TaskAgentJobRequest request);

    void NotifyAgentRequestCompleted(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      TaskAgentJobRequest request);

    Task Subscribe(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      string connectionId);

    Task Unsubscribe(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      string connectionId);

    Task Disconnect(IVssRequestContext requestContext, string connectionId);
  }
}
