// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentGroupsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "deploymentgroups")]
  public sealed class DeploymentGroupsController : DeploymentGroupApiController
  {
    [HttpPost]
    [ClientExample("POST__distributedtask_DeploymentGroups.json", "Create a deployment group", null, null)]
    public DeploymentGroup AddDeploymentGroup(DeploymentGroupCreateParameter deploymentGroup)
    {
      if (deploymentGroup == null)
        throw new ArgumentNullException(nameof (deploymentGroup));
      return this.DeploymentGroupService.AddDeploymentGroup(this.TfsRequestContext, this.ProjectId, deploymentGroup.ToDeploymentGroup());
    }

    [HttpDelete]
    [ClientExample("DELETE__distributedtask_DeploymentGroup__Id_.json", "Delete a deployment group", null, null)]
    public void DeleteDeploymentGroup(int deploymentGroupId) => this.DeploymentGroupService.DeleteDeploymentGroup(this.TfsRequestContext, this.ProjectId, deploymentGroupId);

    [HttpGet]
    [ClientExample("GET__distributedtask_DeploymentGroups__Id_.json", "Get a deployment group by its ID", null, null)]
    public DeploymentGroup GetDeploymentGroup(
      int deploymentGroupId,
      DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None,
      [FromUri(Name = "$expand")] DeploymentGroupExpands expands = DeploymentGroupExpands.Machines)
    {
      bool includeMachines = (expands & DeploymentGroupExpands.Machines) == DeploymentGroupExpands.Machines;
      bool includeTags = (expands & DeploymentGroupExpands.Tags) == DeploymentGroupExpands.Tags;
      return this.DeploymentGroupService.GetDeploymentGroup(this.TfsRequestContext, this.ProjectId, deploymentGroupId, actionFilter, includeMachines, includeTags) ?? throw new DeploymentGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) deploymentGroupId));
    }

    [HttpGet]
    [ClientExample("GET__distributedtask_DeploymentGroups__Name_.json", "Get a deployment group by name", null, null)]
    [ClientExample("GET__distributedtask_DeploymentGroups__Ids_.json", "Get deployment groups by IDs", null, null)]
    [ClientExample("GET__distributedtask_DeploymentGroups.json", "List all deployment groups", null, null)]
    [ClientExample("GET__distributedtask_DeploymentGroups__Page_.json", "Get deployment groups in pages", null, null)]
    [ClientResponseType(typeof (IPagedList<DeploymentGroup>), null, null)]
    public HttpResponseMessage GetDeploymentGroups(
      [ClientQueryParameter] string name = null,
      [ClientQueryParameter] DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None,
      [FromUri(Name = "$expand")] DeploymentGroupExpands expands = DeploymentGroupExpands.None,
      [ClientQueryParameter] string continuationToken = "",
      [FromUri(Name = "$top")] int top = 1000,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string ids = null)
    {
      bool includeMachines = (expands & DeploymentGroupExpands.Machines) == DeploymentGroupExpands.Machines;
      if (includeMachines)
        throw new NotSupportedException(TaskResources.DeploymentGroupsExpandMachinesDeprecated());
      IList<int> array = this.ParseArray(ids);
      if (array.Count > 1000)
        throw new ArgumentOutOfRangeException(TaskResources.RequestedMoreThanMaxSupport());
      HttpResponseMessage response;
      if (array.Count > 0)
      {
        response = this.Request.CreateResponse<IList<DeploymentGroup>>(HttpStatusCode.OK, this.DeploymentGroupService.GetDeploymentGroupsByIds(this.TfsRequestContext, this.ProjectId, array, actionFilter, includeMachines));
      }
      else
      {
        IPagedList<DeploymentGroup> deploymentGroupsPaged = this.DeploymentGroupService.GetDeploymentGroupsPaged(this.TfsRequestContext, this.ProjectId, name, actionFilter, includeMachines, continuationToken, top);
        response = this.Request.CreateResponse<IPagedList<DeploymentGroup>>(HttpStatusCode.OK, deploymentGroupsPaged);
        if (!string.IsNullOrWhiteSpace(deploymentGroupsPaged.ContinuationToken))
          DistributedTaskProjectApiController.SetContinuationToken(response, deploymentGroupsPaged.ContinuationToken);
      }
      return response;
    }

    [HttpPatch]
    [ClientExample("PATCH__distributedtask_DeploymentGroups__Id_.json", "Update a deployment group", null, null)]
    public DeploymentGroup UpdateDeploymentGroup(
      int deploymentGroupId,
      DeploymentGroupUpdateParameter deploymentGroup)
    {
      if (deploymentGroup == null)
        throw new ArgumentNullException(nameof (deploymentGroup));
      return this.DeploymentGroupService.UpdateDeploymentGroup(this.TfsRequestContext, this.ProjectId, deploymentGroupId, deploymentGroup.ToDeploymentGroup(deploymentGroupId));
    }

    private bool GetFeatureFlagValue(string featureFlag) => this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(this.TfsRequestContext, featureFlag);
  }
}
