// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitCommits2Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
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
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "Commits", ResourceVersion = 2)]
  public class GitCommits2Controller : GitCommitsController
  {
    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__commits.json", "All commits", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits_author-_author_.json", "By author", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits_committer-_committer_.json", "By committer", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits_branch.json", "On a branch", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits_branch_itemPath-_itemPath_.json", "On a branch and in a path", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits_commit.json", "Reachable from a commit", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits_commit_itemPath-_itemPath_.json", "Reachable from a commit and path", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits_fromDate-_fromDate__toDate-_toDate_.json", "In a date range", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits__skip-_skip___top-_top_.json", "Paging", null, null)]
    [ClientResponseType(typeof (IList<GitCommitRef>), null, null)]
    [ClientLocationId("C2570C3B-5B3F-41B8-98BF-5407BFDE8D58")]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetCommits(
      [ModelBinder(typeof (GitQueryCommitsCriteriaModelBinder))] GitQueryCommitsCriteria searchCriteria,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      [ClientIgnore, FromUri(Name = "branch")] string branch = null,
      [ClientIgnore, FromUri(Name = "commit")] string commit = null,
      [ClientInclude(~RestClientLanguages.Swagger2), FromUri(Name = "$skip")] int? skip = null,
      [ClientInclude(~RestClientLanguages.Swagger2), FromUri(Name = "$top")] int? top = null)
    {
      if (branch != null && commit != null)
        throw new ArgumentException(Resources.Get("ErrorBranchAndCommit")).Expected("git");
      if (branch != null)
      {
        searchCriteria.ItemVersion = new GitVersionDescriptor();
        searchCriteria.ItemVersion.Version = branch;
        searchCriteria.ItemVersion.VersionType = GitVersionType.Branch;
      }
      else if (commit != null)
      {
        searchCriteria.ItemVersion = new GitVersionDescriptor();
        searchCriteria.ItemVersion.Version = commit;
        searchCriteria.ItemVersion.VersionType = GitVersionType.Commit;
      }
      return this.QueryCommits2(searchCriteria, repositoryId, projectId, skip, top);
    }

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__commits__commitId_.json", "Get by ID", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits__commitId__changeCount-10.json", "With limited changes", null, null)]
    [ClientLocationId("C2570C3B-5B3F-41B8-98BF-5407BFDE8D58")]
    [PublicProjectRequestRestrictions]
    public override GitCommit GetCommit(
      string commitId,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      int changeCount = 0)
    {
      return this.GetCommitInternal(commitId, repositoryId, projectId, changeCount);
    }

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__pushes__pushId__commits.json", null, null, null)]
    [ClientResponseType(typeof (IList<GitCommitRef>), null, null)]
    [ClientLocationId("C2570C3B-5B3F-41B8-98BF-5407BFDE8D58")]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetPushCommits(
      int pushId,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      int top = 100,
      int skip = 0,
      bool includeLinks = true)
    {
      List<GitCommitRef> pushCommitsInternal = this.GetPushCommitsInternal(pushId, repositoryId, projectId, top, skip, includeLinks);
      return pushCommitsInternal == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<List<GitCommitRef>>(HttpStatusCode.OK, pushCommitsInternal);
    }
  }
}
