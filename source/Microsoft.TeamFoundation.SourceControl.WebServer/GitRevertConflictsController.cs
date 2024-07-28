// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRevertConflictsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer.ObjectModel;
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
  [FeatureEnabled("SourceControl.GitRevert.Conflicts")]
  public class GitRevertConflictsController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("10D7AE6D-1050-446D-852A-BD5D99F834BF")]
    [PublicProjectRequestRestrictions]
    public IPagedList<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict> GetRevertConflicts(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int revertId,
      string continuationToken = null,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri] bool excludeResolved = false,
      [FromUri] bool onlyResolved = false,
      [FromUri] bool includeObsolete = false,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        try
        {
          int num1 = 0;
          GitConflictsContinuationToken token;
          if (GitConflictsContinuationToken.TryParse(continuationToken, out token))
            num1 = token.ConflictId;
          IVssRequestContext tfsRequestContext1 = this.TfsRequestContext;
          ITfsGitRepository repository = tfsGitRepository;
          int conflictSourceId = revertId;
          int top1 = top ?? 1000;
          bool flag1 = includeObsolete;
          bool flag2 = excludeResolved;
          bool flag3 = onlyResolved;
          int minConflictId = num1;
          int num2 = flag1 ? 1 : 0;
          int num3 = flag2 ? 1 : 0;
          int num4 = flag3 ? 1 : 0;
          List<Microsoft.TeamFoundation.Git.Server.GitConflict> serverConflictList = GitConflictService.QueryGitConflicts(tfsRequestContext1, repository, GitConflictSourceType.AsyncOperation, conflictSourceId, top1, minConflictId: minConflictId, includeObsolete: num2 != 0, excludeResolved: num3 != 0, onlyResolved: num4 != 0);
          IVssRequestContext tfsRequestContext2 = this.TfsRequestContext;
          RepoKey key = tfsGitRepository.Key;
          GitMergeOriginRef conflictOrigin = new GitMergeOriginRef();
          conflictOrigin.RevertId = new int?(revertId);
          UrlHelper url = this.Url;
          ISecuredObject securedObject = repositoryReadOnly;
          List<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict> apiGitConflictList = serverConflictList.ToWebApiGitConflictList(tfsRequestContext2, key, conflictOrigin, url, securedObject);
          return (IPagedList<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict>) new PagedList<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict>((IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict>) apiGitConflictList, new GitConflictsContinuationToken((IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict>) apiGitConflictList).ToString());
        }
        catch (GitAsyncOperationNotFoundException ex)
        {
          throw new HttpException(404, ex.Message);
        }
      }
    }

    [HttpGet]
    [ClientLocationId("10D7AE6D-1050-446D-852A-BD5D99F834BF")]
    public Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict GetRevertConflict(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int revertId,
      int conflictId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        try
        {
          Microsoft.TeamFoundation.Git.Server.GitConflict gitConflictById = GitConflictService.GetGitConflictById(this.TfsRequestContext, tfsGitRepository, GitConflictSourceType.AsyncOperation, revertId, conflictId);
          if (gitConflictById == null)
            throw new HttpException(404, Resources.Get("NotFound"));
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          RepoKey key = tfsGitRepository.Key;
          GitMergeOriginRef conflictOrigin = new GitMergeOriginRef();
          conflictOrigin.RevertId = new int?(revertId);
          UrlHelper url = this.Url;
          ISecuredObject securedObject = repositoryReadOnly;
          return gitConflictById.ToWebApiGitConflict(tfsRequestContext, key, conflictOrigin, url, securedObject);
        }
        catch (GitAsyncOperationNotFoundException ex)
        {
          throw new HttpException(404, ex.Message);
        }
      }
    }

    [HttpPatch]
    [ClientLocationId("10D7AE6D-1050-446D-852A-BD5D99F834BF")]
    public Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict UpdateRevertConflict(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int revertId,
      int conflictId,
      [ClientParameterType(typeof (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict), false)] JObject conflict,
      [ClientIgnore] string projectId = null)
    {
      conflict = conflict ?? new JObject();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        GitRevertConflictService revertConflictService = new GitRevertConflictService();
        Microsoft.TeamFoundation.Git.Server.GitConflict gitConflict;
        try
        {
          gitConflict = revertConflictService.UpdateGitConflictResolution(this.TfsRequestContext, tfsGitRepository, GitConflictSourceType.AsyncOperation, revertId, conflictId, conflict);
        }
        catch (GitAsyncOperationNotFoundException ex)
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
        conflictOrigin.RevertId = new int?(revertId);
        UrlHelper url = this.Url;
        ISecuredObject securedObject = repositoryReadOnly;
        return serverItem.ToWebApiGitConflict(tfsRequestContext, key, conflictOrigin, url, securedObject);
      }
    }

    [HttpPatch]
    [ClientLocationId("10D7AE6D-1050-446D-852A-BD5D99F834BF")]
    [ClientResponseType(typeof (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult>), null, null)]
    public HttpResponseMessage UpdateRevertConflicts(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int revertId,
      [ClientParameterType(typeof (List<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict>), false)] List<JObject> conflictUpdates,
      [ClientIgnore] string projectId = null)
    {
      conflictUpdates = conflictUpdates ?? new List<JObject>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        GitRevertConflictService revertConflictService = new GitRevertConflictService();
        List<Microsoft.TeamFoundation.Git.Server.GitConflictUpdateResult> conflictUpdateResultList;
        try
        {
          conflictUpdateResultList = revertConflictService.UpdateGitConflictResolutions(this.TfsRequestContext, tfsGitRepository, GitConflictSourceType.AsyncOperation, revertId, conflictUpdates);
        }
        catch (GitAsyncOperationNotFoundException ex)
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
        conflictOrigin.RevertId = new int?(revertId);
        UrlHelper url = this.Url;
        ISecuredObject securedObject = repositoryReadOnly;
        return this.GenerateResponse<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult>((IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult>) serverUpdateResultList.ToWebApiGitConflictUpdateResultList(tfsRequestContext, key, conflictOrigin, url, securedObject));
      }
    }
  }
}
