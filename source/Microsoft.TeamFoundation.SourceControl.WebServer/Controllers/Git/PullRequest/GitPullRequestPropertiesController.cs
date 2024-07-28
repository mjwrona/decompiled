// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git.PullRequest.GitPullRequestPropertiesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git.PullRequest
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitPullRequestPropertiesController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("48A52185-5B9E-4736-9DC1-BB1E2FEAC80B")]
    [ClientExample("GetPullRequestProperties.json", null, null, null)]
    public PropertiesCollection GetPullRequestProperties(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        return service.GetPullRequestProperties(this.TfsRequestContext, tfsGitRepository, service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId) ?? throw new GitPullRequestNotFoundException());
      }
    }

    [HttpPatch]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientLocationId("48A52185-5B9E-4736-9DC1-BB1E2FEAC80B")]
    [ClientExample("AddPullRequestProperties.json", "Add properties", null, null)]
    [ClientExample("RemoveAndReplacePullRequestProperties.json", "Remove and replace properties", null, null)]
    public PropertiesCollection UpdatePullRequestProperties(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> patchDocument,
      [ClientIgnore] string projectId = null)
    {
      PropertiesCollection properties = PropertiesCollectionPatchHelper.ReadPatchDocument((IPatchDocument<IDictionary<string, object>>) patchDocument);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        return service.UpdatePullRequestProperties(this.TfsRequestContext, tfsGitRepository, service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId) ?? throw new GitPullRequestNotFoundException(), properties);
      }
    }
  }
}
