// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitCherryPicks2Controller
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
  [FeatureEnabled("SourceControl.CherryPick")]
  [ControllerApiVersion(7.2)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "cherryPicks", ResourceVersion = 2)]
  public class GitCherryPicks2Controller : GitCherryPicksController
  {
    [HttpPost]
    [ClientLocationId("033BAD68-9A14-43D1-90E0-59CB8856FEF6")]
    [ClientResponseType(typeof (GitCherryPick), null, null)]
    public override HttpResponseMessage CreateCherryPick(
      [FromBody] Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationParameters cherryPickToCreate,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      return this.Request.CreateResponse<GitCherryPick>(HttpStatusCode.Created, this.CreateCherryPickInternal(cherryPickToCreate, repositoryId, projectId));
    }

    [HttpGet]
    [ClientLocationId("033BAD68-9A14-43D1-90E0-59CB8856FEF6")]
    public override GitCherryPick GetCherryPickForRefName(
      [FromUri] string refName,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      if (refName == null)
        throw new InvalidArgumentValueException(refName);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        return this.TfsRequestContext.GetService<ITeamFoundationGitCherryPickService>().GetAsyncRefOperationByRef(this.TfsRequestContext, tfsGitRepository, refName).ToWebApiCherryPickItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
    }

    [HttpGet]
    [ClientLocationId("033BAD68-9A14-43D1-90E0-59CB8856FEF6")]
    public override GitCherryPick GetCherryPick(
      int cherryPickId,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        return this.TfsRequestContext.GetService<ITeamFoundationGitCherryPickService>().GetAsyncRefOperationById(this.TfsRequestContext, tfsGitRepository, cherryPickId).ToWebApiCherryPickItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
    }
  }
}
