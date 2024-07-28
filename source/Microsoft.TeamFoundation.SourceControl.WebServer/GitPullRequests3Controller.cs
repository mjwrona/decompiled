// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequests3Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(7.2)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "pullRequests", ResourceVersion = 2)]
  public class GitPullRequests3Controller : GitPullRequestsController
  {
    [HttpGet]
    [ClientLocationId("9946FD70-0D40-406E-B686-B4744CBBCC37")]
    [PublicProjectRequestRestrictions]
    public override GitPullRequest GetPullRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [ClientIgnore] string projectId = null,
      int? maxCommentLength = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null,
      bool includeCommits = false,
      bool includeWorkItemRefs = false)
    {
      return this.GetGitPullRequestInternal(repositoryId, pullRequestId, projectId, includeCommits, includeWorkItemRefs);
    }

    [HttpGet]
    [ClientLocationId("01A46DEA-7D46-4D40-BC84-319E7C260D99")]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests__pullRequestId_.json", null, null, null)]
    [PublicProjectRequestRestrictions]
    public override GitPullRequest GetPullRequestById(int pullRequestId) => this.GetPullRequestByIdInternal(pullRequestId);

    [HttpGet]
    [ClientResponseType(typeof (IList<GitPullRequest>), null, null)]
    [ClientLocationId("9946FD70-0D40-406E-B686-B4744CBBCC37")]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests.json", "Pull requests by repository", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests_status-completed.json", "Just completed pull requests", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests_targetRefName-refs_heads_master.json", "Targeting a specific branch", null, null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetPullRequests(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ModelBinder] GitPullRequestSearchCriteria searchCriteria,
      [ClientIgnore] string projectId = null,
      int? maxCommentLength = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null)
    {
      (List<GitPullRequest> gitPullRequests, ISecuredObject securedObject) requestsInternal = this.GetPullRequestsInternal(repositoryId, searchCriteria, projectId, skip, top);
      return this.GenerateResponse<GitPullRequest>((IEnumerable<GitPullRequest>) requestsInternal.gitPullRequests, requestsInternal.securedObject);
    }

    [HttpGet]
    [ClientResponseType(typeof (IList<GitPullRequest>), null, null)]
    [ClientLocationId("A5D28130-9CD2-40FA-9F08-902E7DAA9EFB")]
    [ClientExample("GET__git_pullRequests.json", "Pull requests by project", null, null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetPullRequestsByProject(
      [ModelBinder] GitPullRequestSearchCriteria searchCriteria,
      int? maxCommentLength = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null)
    {
      return this.GetPullRequests((string) null, searchCriteria, (string) null, maxCommentLength, skip, top);
    }

    [HttpPatch]
    [ClientLocationId("9946FD70-0D40-406E-B686-B4744CBBCC37")]
    [ClientResponseType(typeof (GitPullRequest), null, null)]
    [ClientExample("PATCH__git_repositories__repositoryId__pullRequests__pullRequestId_.json", "Update title", null, null)]
    [ClientExample("PATCH__git_repositories__repositoryId__pullRequests__pullRequestId_2.json", "Update description", null, null)]
    [ClientExample("PATCH__git_repositories__repositoryId__pullRequests__autoCompletePullRequestId_.json", "Enable auto-completion and set other completion options", null, null)]
    public override HttpResponseMessage UpdatePullRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      GitPullRequest gitPullRequestToUpdate,
      [ClientIgnore] string projectId = null)
    {
      return this.Request.CreateResponse<GitPullRequest>(HttpStatusCode.OK, this.UpdatePullRequestInternal(repositoryId, pullRequestId, gitPullRequestToUpdate, projectId, false));
    }
  }
}
