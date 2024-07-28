// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IDeploymentGroupService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (DeploymentGroupService))]
  public interface IDeploymentGroupService : IVssFrameworkService
  {
    DeploymentGroup AddDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      DeploymentGroup machineGroup);

    void DeleteDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId);

    DeploymentGroup GetDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None,
      bool includeMachines = true,
      bool includeTags = false);

    IList<DeploymentGroup> GetDeploymentGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string machineGroupName = null,
      DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None,
      bool includeMachines = false);

    IPagedList<DeploymentGroup> GetDeploymentGroupsPaged(
      IVssRequestContext requestContext,
      Guid projectId,
      string machineGroupName = null,
      DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None,
      bool includeMachines = false,
      string lastDeploymentGroupName = null,
      int maxDeploymentGroupCount = 10000);

    IPagedList<DeploymentGroupMetrics> GetDeploymentGroupsMetrics(
      IVssRequestContext requestContext,
      Guid projectId,
      string deploymentGroupName = null,
      string lastDeploymentGroupName = null,
      int maxDeploymentGroupCount = 50);

    IList<DeploymentGroup> GetDeploymentGroupsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> deploymentGroupIds,
      DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None,
      bool includeMachines = false);

    Task<IPagedList<DeploymentMachine>> GetDeploymentTargetsPagedAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<string> tagFilters = null,
      string machineName = null,
      bool partialNameMatch = false,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      TaskAgentStatusFilter agentStatusFilter = TaskAgentStatusFilter.All,
      TaskAgentJobResultFilter agentJobResultFilter = TaskAgentJobResultFilter.All,
      string continuationToken = null,
      int top = 1000,
      bool? enabled = null,
      IList<string> propertyFilters = null);

    Task<DeploymentMachine> GetDeploymentTargetAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int targetId,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false);

    IList<TaskAgentJobRequest> GetAgentRequestsForDeploymentTarget(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int targetId,
      int completedRequestCount = 50);

    IList<TaskAgentJobRequest> GetAgentRequestsForDeploymentTargets(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<int> targetIds,
      int completedRequestCount = 50,
      int? ownerId = null,
      DateTime? completedOn = null);

    DeploymentGroup UpdateDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      DeploymentGroup machineGroup);

    string GeneratePersonalAccessTokenWithDeploymentGroupScope(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId);

    string GeneratePersonalAccessTokenWithDeploymentPoolScope(
      IVssRequestContext requestContext,
      int poolId);

    Task SendRefreshMessageToDeploymentTargetsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId);

    DeploymentMachine AddDeploymentTarget(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      DeploymentMachine machine);

    void DeleteDeploymentTarget(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int machineId);

    void MigrateDeploymentGroups(
      IVssRequestContext requestContext,
      string sourcePoolName,
      string destinationPoolName);

    DeploymentMachine UpdateDeploymentTarget(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      DeploymentMachine machine,
      TaskAgentCapabilityType capabilityUpdate = TaskAgentCapabilityType.System);

    IList<DeploymentMachine> UpdateDeploymentTargets(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      IList<DeploymentMachine> machinesToUpdate);

    Task<IList<DeploymentMachine>> GetDeploymentMachinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      IList<string> tagFilters = null,
      string machineName = null,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false);

    IList<DeploymentMachine> UpdateDeploymentMachines(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      IList<DeploymentMachine> machinesToUpdate);
  }
}
