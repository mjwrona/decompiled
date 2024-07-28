// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestConflictsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ClientInternalUseOnly(false)]
  [FeatureEnabled("SourceControl.GitPullRequests.Conflicts")]
  public class GitPullRequestConflictsController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("D840FB74-BBEF-42D3-B250-564604C054A4")]
    [PublicProjectRequestRestrictions]
    public IList<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict> GetPullRequestConflicts(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri] bool includeObsolete = false,
      [FromUri] bool excludeResolved = false,
      [FromUri] bool onlyResolved = false,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        try
        {
          List<Microsoft.TeamFoundation.Git.Server.GitConflict> serverConflictList = GitConflictService.QueryGitConflicts(this.TfsRequestContext, tfsGitRepository, GitConflictSourceType.PullRequest, pullRequestId, top ?? 1000, skip.GetValueOrDefault(), includeObsolete: includeObsolete, excludeResolved: excludeResolved, onlyResolved: onlyResolved);
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          RepoKey key = tfsGitRepository.Key;
          GitMergeOriginRef conflictOrigin = new GitMergeOriginRef();
          conflictOrigin.PullRequestId = new int?(pullRequestId);
          UrlHelper url = this.Url;
          ISecuredObject securedObject = repositoryReadOnly;
          return (IList<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict>) serverConflictList.ToWebApiGitConflictList(tfsRequestContext, key, conflictOrigin, url, securedObject);
        }
        catch (GitPullRequestNotFoundException ex)
        {
          throw new HttpException(404, ex.Message);
        }
      }
    }

    [HttpGet]
    [ClientLocationId("D840FB74-BBEF-42D3-B250-564604C054A4")]
    public Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict GetPullRequestConflict(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int conflictId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        try
        {
          Microsoft.TeamFoundation.Git.Server.GitConflict gitConflictById = GitConflictService.GetGitConflictById(this.TfsRequestContext, tfsGitRepository, GitConflictSourceType.PullRequest, pullRequestId, conflictId);
          if (gitConflictById == null)
            throw new HttpException(404, Resources.Get("NotFound"));
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          RepoKey key = tfsGitRepository.Key;
          GitMergeOriginRef conflictOrigin = new GitMergeOriginRef();
          conflictOrigin.PullRequestId = new int?(pullRequestId);
          UrlHelper url = this.Url;
          ISecuredObject securedObject = repositoryReadOnly;
          return gitConflictById.ToWebApiGitConflict(tfsRequestContext, key, conflictOrigin, url, securedObject);
        }
        catch (GitPullRequestNotFoundException ex)
        {
          throw new HttpException(404, ex.Message);
        }
      }
    }

    [HttpPatch]
    [ClientLocationId("D840FB74-BBEF-42D3-B250-564604C054A4")]
    public Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict UpdatePullRequestConflict(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int conflictId,
      [ClientParameterType(typeof (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict), false)] JObject conflict,
      [ClientIgnore] string projectId = null)
    {
      conflict = conflict ?? new JObject();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        GitPullRequestConflictService requestConflictService = new GitPullRequestConflictService();
        Microsoft.TeamFoundation.Git.Server.GitConflict gitConflict;
        try
        {
          gitConflict = requestConflictService.UpdateGitConflictResolution(this.TfsRequestContext, tfsGitRepository, GitConflictSourceType.PullRequest, pullRequestId, conflictId, conflict);
        }
        catch (GitPullRequestNotFoundException ex)
        {
          throw new HttpException(404, ex.Message);
        }
        catch (Exception ex) when (
        {
          // ISSUE: unable to correctly present filter
          int num;
          switch (ex)
          {
            case JsonSerializationException _:
            case InvalidOperationException _:
              num = 1;
              break;
            default:
              num = ex is NotSupportedException ? 1 : 0;
              break;
          }
          if ((uint) num > 0U)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
          throw new HttpException(400, ex.Message);
        }
        if (gitConflict == null)
          throw new HttpException(404, Resources.Get("NotFound"));
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        Microsoft.TeamFoundation.Git.Server.GitConflict serverItem = gitConflict;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        RepoKey key = tfsGitRepository.Key;
        GitMergeOriginRef conflictOrigin = new GitMergeOriginRef();
        conflictOrigin.PullRequestId = new int?(pullRequestId);
        UrlHelper url = this.Url;
        ISecuredObject securedObject = repositoryReadOnly;
        return serverItem.ToWebApiGitConflict(tfsRequestContext, key, conflictOrigin, url, securedObject);
      }
    }

    [HttpPatch]
    [ClientLocationId("D840FB74-BBEF-42D3-B250-564604C054A4")]
    [ClientResponseType(typeof (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult>), null, null)]
    public HttpResponseMessage UpdatePullRequestConflicts(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [ClientParameterType(typeof (List<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict>), false)] List<JObject> conflictUpdates,
      [ClientIgnore] string projectId = null)
    {
      conflictUpdates = conflictUpdates ?? new List<JObject>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        GitPullRequestConflictService requestConflictService = new GitPullRequestConflictService();
        List<Microsoft.TeamFoundation.Git.Server.GitConflictUpdateResult> conflictUpdateResultList;
        try
        {
          conflictUpdateResultList = requestConflictService.UpdateGitConflictResolutions(this.TfsRequestContext, tfsGitRepository, GitConflictSourceType.PullRequest, pullRequestId, conflictUpdates);
        }
        catch (GitPullRequestNotFoundException ex)
        {
          throw new HttpException(404, ex.Message);
        }
        catch (GitArgumentException ex)
        {
          throw new HttpException(400, ex.Message);
        }
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        List<Microsoft.TeamFoundation.Git.Server.GitConflictUpdateResult> serverUpdateResultList = conflictUpdateResultList;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        RepoKey key = tfsGitRepository.Key;
        GitMergeOriginRef conflictOrigin = new GitMergeOriginRef();
        conflictOrigin.PullRequestId = new int?(pullRequestId);
        UrlHelper url = this.Url;
        ISecuredObject securedObject = repositoryReadOnly;
        return this.GenerateResponse<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult>((IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult>) serverUpdateResultList.ToWebApiGitConflictUpdateResultList(tfsRequestContext, key, conflictOrigin, url, securedObject));
      }
    }
  }
}
