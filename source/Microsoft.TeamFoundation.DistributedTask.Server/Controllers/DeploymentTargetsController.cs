// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentTargetsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "targets")]
  public class DeploymentTargetsController : DeploymentGroupApiController
  {
    [HttpGet]
    [ClientLocationId("2F0AA599-C121-4256-A5FD-BA370E0AE7B6")]
    [ClientResponseType(typeof (IPagedList<DeploymentMachine>), null, null)]
    [ClientExample("GET__distributedtask_DeploymentTargets.json", "List all deployment targets in a deployment group", null, null)]
    [ClientExample("GET__distributedtask_DeploymentTargets__Tags_.json", "Get deployment targets having given tags", null, null)]
    [ClientExample("GET__distributedtask_DeploymentTargets__Name_.json", "Get deployment targets by partial name match", null, null)]
    [ClientExample("GET__distributedtask_DeploymentTargets__LastRequest_.json", "Get deployment targets including their last job requests", null, null)]
    [ClientExample("GET__distributedtask_DeploymentTargets__AgentStatus_.json", "Get deployment targets filtered by agent status", null, null)]
    [ClientExample("GET__distributedtask_DeploymentTargets__Page_.json", "Get deployment targets in pages", null, null)]
    public async Task<HttpResponseMessage> GetDeploymentTargets(
      int deploymentGroupId,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string tags = null,
      [ClientQueryParameter] string name = null,
      [ClientQueryParameter] bool partialNameMatch = false,
      [FromUri(Name = "$expand")] DeploymentTargetExpands expands = DeploymentTargetExpands.None,
      [ClientQueryParameter] TaskAgentStatusFilter agentStatus = TaskAgentStatusFilter.All,
      [ClientQueryParameter] TaskAgentJobResultFilter agentJobResult = TaskAgentJobResultFilter.All,
      [ClientQueryParameter] string continuationToken = null,
      [FromUri(Name = "$top")] int top = 1000,
      [ClientQueryParameter] bool? enabled = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null)
    {
      DeploymentTargetsController targetsController = this;
      if (top > 1000)
        throw new ArgumentOutOfRangeException(TaskResources.RequestedMoreThanMaxSupport());
      bool flag1 = (expands & DeploymentTargetExpands.Capabilities) == DeploymentTargetExpands.Capabilities;
      bool flag2 = (expands & DeploymentTargetExpands.AssignedRequest) == DeploymentTargetExpands.AssignedRequest;
      bool flag3 = (expands & DeploymentTargetExpands.LastCompletedRequest) == DeploymentTargetExpands.LastCompletedRequest;
      IDeploymentGroupService deploymentGroupService = targetsController.DeploymentGroupService;
      IVssRequestContext tfsRequestContext = targetsController.TfsRequestContext;
      Guid projectId = targetsController.ProjectId;
      int deploymentGroupId1 = deploymentGroupId;
      string str = tags;
      List<string> tagFilters;
      if (str == null)
        tagFilters = (List<string>) null;
      else
        tagFilters = ((IEnumerable<string>) str.Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
      string machineName = name;
      int num1 = partialNameMatch ? 1 : 0;
      int num2 = flag1 ? 1 : 0;
      int num3 = flag2 ? 1 : 0;
      int num4 = flag3 ? 1 : 0;
      int num5 = (int) agentStatus;
      int num6 = (int) agentJobResult;
      string continuationToken1 = continuationToken;
      int top1 = top;
      bool? enabled1 = enabled;
      IList<string> propertyFilters1 = ArtifactPropertyKinds.AsPropertyFilters(propertyFilters);
      IPagedList<DeploymentMachine> targetsPagedAsync = await deploymentGroupService.GetDeploymentTargetsPagedAsync(tfsRequestContext, projectId, deploymentGroupId1, (IList<string>) tagFilters, machineName, num1 != 0, num2 != 0, num3 != 0, num4 != 0, (TaskAgentStatusFilter) num5, (TaskAgentJobResultFilter) num6, continuationToken1, top1, enabled1, propertyFilters1);
      HttpResponseMessage response = targetsController.Request.CreateResponse<IPagedList<DeploymentMachine>>(HttpStatusCode.OK, targetsPagedAsync);
      if (!string.IsNullOrWhiteSpace(targetsPagedAsync.ContinuationToken))
        DistributedTaskProjectApiController.SetContinuationToken(response, targetsPagedAsync.ContinuationToken);
      return response;
    }

    [HttpPost]
    [ClientInternalUseOnly(false)]
    public DeploymentMachine AddDeploymentTarget(int deploymentGroupId, DeploymentMachine machine) => this.DeploymentGroupService.AddDeploymentTarget(this.TfsRequestContext, this.ProjectId, deploymentGroupId, machine);

    [HttpPut]
    [ClientInternalUseOnly(false)]
    public DeploymentMachine ReplaceDeploymentTarget(
      int deploymentGroupId,
      int targetId,
      DeploymentMachine machine)
    {
      if (machine != null)
        machine.Id = targetId;
      return this.DeploymentGroupService.UpdateDeploymentTarget(this.TfsRequestContext, this.ProjectId, deploymentGroupId, machine);
    }

    [HttpPatch]
    [ClientExample("PATCH__distributedtask_DeploymentTargets.json", "Update tags of a deployment target", null, null)]
    public IList<DeploymentMachine> UpdateDeploymentTargets(
      int deploymentGroupId,
      IList<DeploymentTargetUpdateParameter> machines)
    {
      return this.DeploymentGroupService.UpdateDeploymentTargets(this.TfsRequestContext, this.ProjectId, deploymentGroupId, machines != null ? (IList<DeploymentMachine>) machines.Select<DeploymentTargetUpdateParameter, DeploymentMachine>((Func<DeploymentTargetUpdateParameter, DeploymentMachine>) (m => m.ToDeploymentTarget())).ToList<DeploymentMachine>() : (IList<DeploymentMachine>) null);
    }

    [HttpDelete]
    [ClientExample("DELETE_distributedtask_DeploymentTargets__Id_.json", "Delete a deployment target", null, null)]
    public void DeleteDeploymentTarget(int deploymentGroupId, int targetId) => this.DeploymentGroupService.DeleteDeploymentTarget(this.TfsRequestContext, this.ProjectId, deploymentGroupId, targetId);

    [HttpPatch]
    [ClientInternalUseOnly(false)]
    public DeploymentMachine UpdateDeploymentTarget(
      int deploymentGroupId,
      int targetId,
      DeploymentMachine machine)
    {
      if (machine != null)
        machine.Id = targetId;
      return this.DeploymentGroupService.UpdateDeploymentTarget(this.TfsRequestContext, this.ProjectId, deploymentGroupId, machine, TaskAgentCapabilityType.None);
    }

    [HttpGet]
    [ClientExample("GET__distributedtask_DeploymentTargets__Id_.json", "Get a deployment target by its ID", null, null)]
    public async Task<DeploymentMachine> GetDeploymentTarget(
      int deploymentGroupId,
      int targetId,
      [FromUri(Name = "$expand")] DeploymentTargetExpands expands = DeploymentTargetExpands.None)
    {
      DeploymentTargetsController targetsController = this;
      bool includeCapabilities = (expands & DeploymentTargetExpands.Capabilities) == DeploymentTargetExpands.Capabilities;
      bool includeAssignedRequest = (expands & DeploymentTargetExpands.AssignedRequest) == DeploymentTargetExpands.AssignedRequest;
      bool includeLastCompletedRequest = (expands & DeploymentTargetExpands.LastCompletedRequest) == DeploymentTargetExpands.LastCompletedRequest;
      return await targetsController.DeploymentGroupService.GetDeploymentTargetAsync(targetsController.TfsRequestContext, targetsController.ProjectId, deploymentGroupId, targetId, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest) ?? throw new DeploymentMachineNotFoundException(TaskResources.DeploymentMachineNotFound((object) deploymentGroupId, (object) targetId));
    }
  }
}
