// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestIterationChangesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitPullRequestIterationChangesController : GitApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (GitPullRequestIterationChanges), null, null)]
    [ClientLocationId("4216BDCF-B6B1-4D59-8B82-C34CC183FC8B")]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests__pullRequestId__iterations__iterationId__changes.json", "Changes in a specific iteration", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests__pullRequestId__iterations__iterationId__changes__compareTo.json", "Changes since an earlier iteration", null, null)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetPullRequestIterationChanges(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$compareTo")] int? compareTo = null,
      [ClientIgnore] string projectId = null)
    {
      ArgumentUtility.CheckForOutOfRange(iterationId, nameof (iterationId), 1);
      ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        GitPullRequestIterationChanges changes = service.GetChanges(this.TfsRequestContext, tfsGitRepository, service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId) ?? throw new GitPullRequestNotFoundException(), iterationId, out int _, top, skip, compareTo);
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        changes.SetSecuredObject(repositoryReadOnly);
        return this.Request.CreateResponse<GitPullRequestIterationChanges>(HttpStatusCode.OK, changes);
      }
    }
  }
}
