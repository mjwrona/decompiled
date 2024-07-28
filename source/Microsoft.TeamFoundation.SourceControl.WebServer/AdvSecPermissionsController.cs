// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.AdvSecPermissionsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "advsecPermissions")]
  public class AdvSecPermissionsController : TfsApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (bool), null, null)]
    [ClientLocationId("61B21A05-A60F-4910-A733-BA5347C2142D")]
    [ClientInternalUseOnly(true)]
    public bool GetPermission([FromUri(Name = "$projectName")] string projectName = null, [FromUri(Name = "$repositoryId")] string repositoryId = null, [FromUri(Name = "$permission")] string permission = null)
    {
      if (!this.TfsRequestContext.GetService<IGitAdvSecService>().CheckS2SCall(this.TfsRequestContext))
        throw new HttpException(403, "AdvSec calls are S2S only.");
      using (IVssRequestContext userContext = this.TfsRequestContext.CreateUserContext(this.TfsRequestContext.UserContext))
      {
        ITeamFoundationGitRepositoryService service = userContext.GetService<ITeamFoundationGitRepositoryService>();
        if (!string.IsNullOrEmpty(repositoryId))
        {
          Guid result;
          if (Guid.TryParse(repositoryId, out result))
          {
            using (ITfsGitRepository repositoryById = service.FindRepositoryById(userContext, result, includeDisabled: true))
              return AdvSecPermissionsController.GetPermission(repositoryById.Permissions, permission);
          }
          else
          {
            if (string.IsNullOrWhiteSpace(projectName))
              throw new InvalidArgumentValueException("$projectName", "Invalid projectName value");
            using (ITfsGitRepository repositoryByName = service.FindRepositoryByName(userContext, projectName, repositoryId, true))
              return AdvSecPermissionsController.GetPermission(repositoryByName.Permissions, permission);
          }
        }
        else
        {
          if (string.IsNullOrWhiteSpace(projectName))
            return AdvSecPermissionsController.GetPermission(service.GetOrgLevelRepoPermissionsManager(userContext), permission);
          Guid result;
          if (!Guid.TryParse(projectName, out result))
            result = (userContext.GetService<IProjectService>().GetProject(userContext, projectName) ?? throw new ProjectNameNotRecognizedException(projectName)).Id;
          return AdvSecPermissionsController.GetPermission(service.GetProjectLevelRepoPermissionsManager(userContext, result), permission);
        }
      }
    }

    private static bool GetPermission(IRepoPermissionsManager permissions, string permission)
    {
      switch (permission)
      {
        case "viewAlert":
          return permissions.HasViewAdvSecAlert();
        case "dismissAlert":
          return permissions.HasDismissAdvSecAlert();
        case "manage":
          return permissions.HasManageAdvSec();
        case "viewEnablement":
          return permissions.HasViewAdvSecEnablement();
        case "repoRead":
          return permissions.HasRead();
        default:
          throw new InvalidArgumentValueException("$permission", "Invalid permission value");
      }
    }
  }
}
