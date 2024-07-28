// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitReverts2Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [FeatureEnabled("SourceControl.Revert")]
  [ControllerApiVersion(7.2)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "Reverts", ResourceVersion = 2)]
  public class GitReverts2Controller : GitRevertsController
  {
    [HttpPost]
    [ClientExample("CreateRevert.json", null, null, null)]
    [ClientLocationId("BC866058-5449-4715-9CF1-A510B6FF193C")]
    [ClientResponseType(typeof (GitRevert), null, null)]
    public override HttpResponseMessage CreateRevert(
      [FromBody] Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationParameters revertToCreate,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      return this.Request.CreateResponse<GitRevert>(HttpStatusCode.Created, this.CreateRevertInternal(revertToCreate, repositoryId, projectId));
    }

    [HttpGet]
    [ClientExample("GetRevert.json", null, null, null)]
    [ClientLocationId("BC866058-5449-4715-9CF1-A510B6FF193C")]
    public override GitRevert GetRevertForRefName(
      [FromUri] string refName,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      if (refName == null)
        throw new InvalidArgumentValueException(refName);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        return this.TfsRequestContext.GetService<ITeamFoundationGitRevertService>().GetAsyncRefOperationByRef(this.TfsRequestContext, tfsGitRepository, refName).ToWebApiRevertItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
    }

    [HttpGet]
    [ClientLocationId("BC866058-5449-4715-9CF1-A510B6FF193C")]
    public override GitRevert GetRevert(int revertId, [ClientParameterType(typeof (Guid), true)] string repositoryId, [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        GitRevert webApiRevertItem = this.TfsRequestContext.GetService<ITeamFoundationGitRevertService>().GetAsyncRefOperationById(this.TfsRequestContext, tfsGitRepository, revertId).ToWebApiRevertItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
        GitStatusStateMapper.MapGitEntity<GitRevert>(webApiRevertItem, this.TfsRequestContext);
        return webApiRevertItem;
      }
    }
  }
}
