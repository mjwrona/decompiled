// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.DeploymentResourceController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.DeploymentTracking.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "DeploymentTracking", ResourceName = "deploymentresources", ResourceVersion = 1)]
  public class DeploymentResourceController : TfsProjectApiController
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Vssf require this")]
    [StaticSafe]
    private static readonly IDictionary<Type, HttpStatusCode> HttpExceptionsMap = (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (DeploymentResourceAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (DeploymentResourceNotFoundException),
        HttpStatusCode.NotFound
      }
    };
    private const int MaxTop = 100;
    private const string ContinuationTokenHeaderName = "x-ms-continuationtoken";

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>), null, null)]
    public HttpResponseMessage GetDeploymentResources(
      [FromUri] int releaseDefinitionId = 0,
      [FromUri] string resourceIdentifier = null,
      [FromUri(Name = "$top")] int top = 100,
      [FromUri(Name = "$continuationToken")] int continuationToken = 0)
    {
      if (top < 0)
        top = 100;
      DeploymentResourceService service = this.TfsRequestContext.GetService<DeploymentResourceService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      int num = releaseDefinitionId;
      string resourceIdentifier1 = resourceIdentifier;
      int releaseDefinitionId1 = num;
      int maxDeploymentResourcesCount = top + 1;
      int continuationToken1 = continuationToken;
      List<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource> deploymentResourceList = this.FilterResourcesByPermission(ReleaseManagementSecurityPermissions.ViewReleaseDefinition, (IEnumerable<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>) service.GetDeploymentResources(tfsRequestContext, projectId, 0, resourceIdentifier1, releaseDefinitionId1, maxDeploymentResourcesCount, continuationToken1).Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentResource, Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentResource, Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>) (resource => resource.ConvertToWebApiContract(this.ProjectId))).ToList<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>());
      this.FillProjectReference(deploymentResourceList);
      HttpResponseMessage responseMessage = (HttpResponseMessage) null;
      try
      {
        if (top >= 0 && deploymentResourceList.Count<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>() > top && deploymentResourceList[top] != null)
        {
          string tokenValue = deploymentResourceList.Last<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>().Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          deploymentResourceList.RemoveAt(top);
          responseMessage = this.Request.CreateResponse<List<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>>(HttpStatusCode.OK, deploymentResourceList);
          DeploymentResourceController.SetContinuationToken(responseMessage, tokenValue);
        }
        else
          responseMessage = this.Request.CreateResponse<List<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>>(HttpStatusCode.OK, deploymentResourceList);
        return responseMessage;
      }
      catch
      {
        responseMessage?.Dispose();
        throw;
      }
    }

    [HttpGet]
    public Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource GetDeploymentResource(
      int resourceId)
    {
      List<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource> deploymentResourceList = this.FilterResourcesByPermission(ReleaseManagementSecurityPermissions.ViewReleaseDefinition, (IEnumerable<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>) this.TfsRequestContext.GetService<DeploymentResourceService>().GetDeploymentResources(this.TfsRequestContext, this.ProjectId, resourceId, string.Empty, 0, 1).Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentResource, Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentResource, Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>) (resource => resource.ConvertToWebApiContract(this.ProjectId))).ToList<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>());
      if (deploymentResourceList.Count<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>() == 0)
        throw new DeploymentResourceNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeploymentResourceNotFound, (object) resourceId));
      this.FillProjectReference(deploymentResourceList);
      return deploymentResourceList.First<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>();
    }

    [HttpPost]
    [ReleaseManagementSecurityPermission("deploymentResource", ReleaseManagementSecurityArgumentType.DeploymentResource, ReleaseManagementSecurityPermissions.EditReleaseDefinition)]
    public Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource AddDeploymentResource(
      [FromBody] PostDeploymentResource deploymentResource)
    {
      if (deploymentResource == null)
        throw new InvalidRequestException(Resources.DeploymentResourceCanNotBeNull);
      Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource webApiContract = this.TfsRequestContext.GetService<DeploymentResourceService>().AddDeploymentResource(this.TfsRequestContext, this.ProjectId, deploymentResource.ResourceIdentifier, deploymentResource.ReleaseDefinitionId, deploymentResource.DefinitionEnvironmentId).ConvertToWebApiContract(this.ProjectId);
      this.FillProjectReference(new List<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>()
      {
        webApiContract
      });
      return webApiContract;
    }

    [HttpPatch]
    [ReleaseManagementSecurityPermission("deploymentResource", ReleaseManagementSecurityArgumentType.DeploymentResource, ReleaseManagementSecurityPermissions.EditReleaseDefinition)]
    public Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource UpdateDeploymentResource(
      int resourceId,
      [FromBody] PatchDeploymentResource deploymentResource)
    {
      if (deploymentResource == null)
        throw new InvalidRequestException(Resources.DeploymentResourceCanNotBeNull);
      Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource webApiContract = this.TfsRequestContext.GetService<DeploymentResourceService>().UpdateDeploymentResource(this.TfsRequestContext, this.ProjectId, resourceId, deploymentResource.ReleaseDefinitionId, deploymentResource.DefinitionEnvironmentId).ConvertToWebApiContract(this.ProjectId);
      this.FillProjectReference(new List<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>()
      {
        webApiContract
      });
      return webApiContract;
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => DeploymentResourceController.HttpExceptionsMap;

    public override string ActivityLogArea => "DeploymentTracking";

    protected static void SetContinuationToken(
      HttpResponseMessage responseMessage,
      string tokenValue)
    {
      if (responseMessage == null)
        throw new ArgumentNullException(nameof (responseMessage));
      if (string.IsNullOrWhiteSpace(tokenValue))
        return;
      responseMessage.Headers.Add("x-ms-continuationtoken", tokenValue);
    }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "It is internally called function.")]
    protected List<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource> FilterResourcesByPermission(
      ReleaseManagementSecurityPermissions permission,
      IEnumerable<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource> deploymentResources)
    {
      IDictionary<int, string> folderPaths = ReleaseManagementSecurityProcessor.GetFolderPaths(this.TfsRequestContext, this.ProjectId, (IList<int>) deploymentResources.Select<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource, int>((Func<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource, int>) (d => d.ReleaseDefinitionId)).ToList<int>());
      return ReleaseManagementSecurityProcessor.FilterComponents<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>(this.TfsRequestContext, deploymentResources, (Func<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource, ReleaseManagementSecurityInfo>) (deploymentResource => ReleaseManagementSecurityProcessor.GetSecurityInfo(this.ProjectId, folderPaths[deploymentResource.ReleaseDefinitionId], deploymentResource.ReleaseDefinitionId, permission)), false).ToList<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource>();
    }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "It is internally called function.")]
    protected List<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource> FillProjectReference(
      List<Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource> deploymentResources)
    {
      if (deploymentResources != null && deploymentResources.Count > 0)
      {
        List<ProjectInfo> list = this.TfsRequestContext.GetService<IProjectService>().GetProjects(this.TfsRequestContext, ProjectState.WellFormed).ToList<ProjectInfo>();
        foreach (Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource deploymentResource in deploymentResources)
        {
          Microsoft.VisualStudio.Services.DeploymentTracking.WebApi.DeploymentResource resource = deploymentResource;
          if (resource.ProjectReference != null && !resource.ProjectReference.Id.Equals(Guid.Empty))
          {
            ProjectInfo projectInfo = list.FirstOrDefault<ProjectInfo>((Func<ProjectInfo, bool>) (p => p.Id.Equals(resource.ProjectReference.Id)));
            if (projectInfo != null)
              resource.ProjectReference.Name = projectInfo.Name;
          }
        }
      }
      return deploymentResources;
    }
  }
}
