// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.Controller.AuthorizationController
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Authorization.Server.Controller
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "pipelinePermissions", ResourceName = "pipelinePermissions", ResourceVersion = 1)]
  public class AuthorizationController : AuthorizationApiController
  {
    private IPipelineResourceAuthorizationService ResourceAuthorizationService => this.TfsRequestContext.GetService<IPipelineResourceAuthorizationService>();

    [HttpGet]
    [ClientExample("GET__GetPipelinePermissionsForResource.json", null, null, null)]
    public async Task<ResourcePipelinePermissions> GetPipelinePermissionsForResource(
      string resourceType,
      string resourceId)
    {
      AuthorizationController authorizationController = this;
      return await authorizationController.ResourceAuthorizationService.GetPipelinePermissionsForResource(authorizationController.TfsRequestContext, authorizationController.ProjectId, resourceType, resourceId);
    }

    [HttpPatch]
    [ClientExample("PATCH__UpdatePipelinePermissionsForResource.json", null, null, null)]
    public async Task<ResourcePipelinePermissions> UpdatePipelinePermisionsForResource(
      string resourceType,
      string resourceId,
      [FromBody] ResourcePipelinePermissions resourceAuthorization)
    {
      AuthorizationController authorizationController = this;
      return await authorizationController.ResourceAuthorizationService.UpdatePipelinePermisionsForResource(authorizationController.TfsRequestContext, authorizationController.ProjectId, resourceType, resourceId, resourceAuthorization);
    }

    [HttpPatch]
    [ClientExample("PATCH__UpdatePipelinePermissionsForResources.json", null, null, null)]
    public async Task<List<ResourcePipelinePermissions>> UpdatePipelinePermisionsForResources(
      [FromBody] List<ResourcePipelinePermissions> resourceAuthorizations)
    {
      AuthorizationController authorizationController = this;
      return await authorizationController.ResourceAuthorizationService.UpdatePipelinePermisionsForResources(authorizationController.TfsRequestContext, authorizationController.ProjectId, resourceAuthorizations);
    }
  }
}
