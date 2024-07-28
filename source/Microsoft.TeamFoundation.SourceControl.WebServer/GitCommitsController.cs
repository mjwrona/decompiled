// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitCommitsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitCommitsController : GitCommitsBaseController
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
    public virtual HttpResponseMessage GetCommits(
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
      return this.QueryCommits(searchCriteria, repositoryId, projectId, skip, top);
    }

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__commits__commitId_.json", "Get by ID", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits__commitId__changeCount-10.json", "With limited changes", null, null)]
    [ClientLocationId("C2570C3B-5B3F-41B8-98BF-5407BFDE8D58")]
    [PublicProjectRequestRestrictions]
    public virtual GitCommit GetCommit(
      string commitId,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      int changeCount = 0)
    {
      GitCommit commitInternal = this.GetCommitInternal(commitId, repositoryId, projectId, changeCount);
      GitStatusStateMapper.MapGitEntity<GitCommit>(commitInternal, this.TfsRequestContext);
      return commitInternal;
    }

    protected GitCommit GetCommitInternal(
      string commitId,
      string repositoryId,
      string projectId,
      int changeCount)
    {
      Sha1Id sha1Id = GitCommitUtility.ParseSha1Id(commitId);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        TfsGitCommit commitById = GitVersionParser.GetCommitById(tfsGitRepository, sha1Id);
        GitCommit gitCommit = new GitCommitTranslator(this.TfsRequestContext, tfsGitRepository.Key, this.Url).ToGitCommit(commitById, repositoryReadOnly, true);
        ITeamFoundationGitCommitService service = this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>();
        int? nullable = new int?();
        if (changeCount > 0)
        {
          CommitMetadataAndChanges commitManifest = service.GetCommitManifest(this.TfsRequestContext, tfsGitRepository, sha1Id, true);
          nullable = commitManifest.PushId;
          GitCommitChanges gitCommitChanges = commitManifest.Changes.ToGitCommitChanges(this.TfsRequestContext, tfsGitRepository.Key, changeCount, 0, repositoryReadOnly, out bool _);
          gitCommit.ChangeCounts = gitCommitChanges.ChangeCounts;
          gitCommit.Changes = gitCommitChanges.Changes;
        }
        else
        {
          IReadOnlyDictionary<Sha1Id, int> pushIdsByCommitIds = service.GetPushIdsByCommitIds(this.TfsRequestContext, tfsGitRepository, (IEnumerable<Sha1Id>) new Sha1Id[1]
          {
            sha1Id
          });
          if (pushIdsByCommitIds.Any<KeyValuePair<Sha1Id, int>>())
            nullable = new int?(pushIdsByCommitIds.First<KeyValuePair<Sha1Id, int>>().Value);
        }
        if (!nullable.HasValue)
          throw new GitCommitDoesNotExistException(commitId);
        ITfsGitRepository repository = tfsGitRepository;
        Dictionary<GitCommitRef, int> commitRefsToPushIds = new Dictionary<GitCommitRef, int>();
        commitRefsToPushIds.Add((GitCommitRef) gitCommit, nullable.Value);
        ISecuredObject securedObject = repositoryReadOnly;
        this.PopulatePushes(repository, (IReadOnlyDictionary<GitCommitRef, int>) commitRefsToPushIds, securedObject);
        ProjectInfo project = this.TfsRequestContext.GetService<IProjectService>().GetProject(this.TfsRequestContext, ProjectInfo.GetProjectId(tfsGitRepository.Key.GetProjectUri()));
        gitCommit.RemoteUrl = WebLinksUtility.GetCommitRemoteUrl(this.TfsRequestContext, tfsGitRepository.Name, project.Name, gitCommit.CommitId);
        gitCommit.Links = gitCommit.GetCommitReferenceLinks(this.TfsRequestContext, tfsGitRepository.Key, repositoryReadOnly);
        return gitCommit;
      }
    }

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__pushes__pushId__commits.json", null, null, null)]
    [ClientResponseType(typeof (IList<GitCommitRef>), null, null)]
    [ClientLocationId("C2570C3B-5B3F-41B8-98BF-5407BFDE8D58")]
    [PublicProjectRequestRestrictions]
    public virtual HttpResponseMessage GetPushCommits(
      int pushId,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      int top = 100,
      int skip = 0,
      bool includeLinks = true)
    {
      List<GitCommitRef> pushCommitsInternal = this.GetPushCommitsInternal(pushId, repositoryId, projectId, top, skip, includeLinks);
      if (pushCommitsInternal == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      GitStatusStateMapper.MapGitEntity<List<GitCommitRef>>(pushCommitsInternal, this.TfsRequestContext);
      return this.Request.CreateResponse<List<GitCommitRef>>(HttpStatusCode.OK, pushCommitsInternal);
    }

    protected List<GitCommitRef> GetPushCommitsInternal(
      int pushId,
      string repositoryId,
      string projectId,
      int top,
      int skip,
      bool includeLinks)
    {
      top = Math.Max(Math.Min(top, 1000), 0);
      skip = Math.Max(skip, 0);
      List<GitCommitRef> pushCommitsInternal = new List<GitCommitRef>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        ITeamFoundationGitCommitService service = this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>();
        TfsGitPushMetadata tfsGitPushMetadata = service.GetPushDataForPushIds(this.TfsRequestContext, tfsGitRepository.Key, new int[1]
        {
          pushId
        }).FirstOrDefault<TfsGitPushMetadata>();
        if (tfsGitPushMetadata != null)
        {
          if (tfsGitPushMetadata.RepoId != tfsGitRepository.Key.RepoId)
            return (List<GitCommitRef>) null;
          IReadOnlyList<TfsGitCommitMetadata> pushCommitsByPushId = service.GetPushCommitsByPushId(this.TfsRequestContext, tfsGitRepository, pushId, new int?(skip), new int?(top));
          GitCommitTranslator commitTranslator = new GitCommitTranslator(this.TfsRequestContext, tfsGitRepository.Key, this.Url);
          foreach (TfsGitCommitMetadata metadata in (IEnumerable<TfsGitCommitMetadata>) pushCommitsByPushId)
          {
            GitCommitRef gitCommitShallow = commitTranslator.ToGitCommitShallow(metadata, false, repositoryReadOnly);
            ProjectInfo project = this.TfsRequestContext.GetService<IProjectService>().GetProject(this.TfsRequestContext, tfsGitRepository.Key.ProjectId);
            gitCommitShallow.RemoteUrl = WebLinksUtility.GetCommitRemoteUrl(this.TfsRequestContext, tfsGitRepository.Name, project.Name, gitCommitShallow.CommitId);
            gitCommitShallow.Links = includeLinks ? gitCommitShallow.GetCommitReferenceLinks(this.TfsRequestContext, tfsGitRepository.Key, repositoryReadOnly) : (ReferenceLinks) null;
            pushCommitsInternal.Add(gitCommitShallow);
          }
        }
      }
      return pushCommitsInternal;
    }
  }
}
