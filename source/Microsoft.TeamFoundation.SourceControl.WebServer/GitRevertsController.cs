// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRevertsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [FeatureEnabled("SourceControl.Revert")]
  public class GitRevertsController : GitApiController
  {
    [HttpPost]
    [ClientExample("CreateRevert.json", null, null, null)]
    [ClientLocationId("BC866058-5449-4715-9CF1-A510B6FF193C")]
    [ClientResponseType(typeof (GitRevert), null, null)]
    public virtual HttpResponseMessage CreateRevert(
      [FromBody] Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationParameters revertToCreate,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      GitRevert revertInternal = this.CreateRevertInternal(revertToCreate, repositoryId, projectId);
      GitStatusStateMapper.MapGitEntity<GitRevert>(revertInternal, this.TfsRequestContext);
      return this.Request.CreateResponse<GitRevert>(HttpStatusCode.Created, revertInternal);
    }

    protected GitRevert CreateRevertInternal(
      Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationParameters revertToCreate,
      string repositoryId,
      string projectId)
    {
      if (revertToCreate == null || string.IsNullOrEmpty(revertToCreate.OntoRefName))
        throw new InvalidArgumentValueException(Resources.Get("MissingRevertRef"));
      if (revertToCreate.Source == null)
        throw new GitAsyncRefOperationInvalidSourceException(Resources.Get("RevertNeedsSingleSource"));
      if (this.TfsRequestContext.IsFeatureEnabled("SourceControl.EnableReturningPartiallySucceededGitStatusState") && revertToCreate.Source.CommitList != null && ((IEnumerable<GitCommitRef>) revertToCreate.Source.CommitList).Any<GitCommitRef>((Func<GitCommitRef, bool>) (c => c?.Statuses != null && c.Statuses.Any<GitStatus>((Func<GitStatus, bool>) (s => s != null && s.State == GitStatusState.PartiallySucceeded)))))
        throw new InvalidArgumentValueException(Resources.Get("InvalidGitStatusStateValue"), "State");
      int num1 = revertToCreate.Source.PullRequestId.HasValue ? 1 : 0;
      GitCommitRef[] commitList = revertToCreate.Source.CommitList;
      int num2 = commitList != null ? (commitList.Length != 0 ? 1 : 0) : (false ? 1 : 0);
      if (num1 == num2)
        throw new GitAsyncRefOperationInvalidSourceException(Resources.Get("RevertNeedsSingleSource"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        if (revertToCreate.Repository != null && revertToCreate.Repository.Id != Guid.Empty && revertToCreate.Repository.Id != tfsGitRepository.Key.RepoId)
          throw new InvalidArgumentValueException(Resources.Get("MismatchRepositoryId"));
        try
        {
          return this.TfsRequestContext.GetService<ITeamFoundationGitRevertService>().CreateAsyncRefOperation(this.TfsRequestContext, tfsGitRepository, revertToCreate.ToServerItem()).ToWebApiRevertItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
        }
        catch (GitAsyncRefOperationInvalidOntoOrGeneratedRefs ex)
        {
          throw new InvalidArgumentValueException(nameof (revertToCreate), (Exception) ex);
        }
      }
    }

    [HttpGet]
    [ClientExample("GetRevert.json", null, null, null)]
    [ClientLocationId("BC866058-5449-4715-9CF1-A510B6FF193C")]
    public virtual GitRevert GetRevertForRefName(
      [FromUri] string refName,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      if (refName == null)
        throw new InvalidArgumentValueException(refName);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        GitRevert webApiRevertItem = this.TfsRequestContext.GetService<ITeamFoundationGitRevertService>().GetAsyncRefOperationByRef(this.TfsRequestContext, tfsGitRepository, refName).ToWebApiRevertItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
        GitStatusStateMapper.MapGitEntity<GitRevert>(webApiRevertItem, this.TfsRequestContext);
        return webApiRevertItem;
      }
    }

    [HttpGet]
    [ClientLocationId("BC866058-5449-4715-9CF1-A510B6FF193C")]
    public virtual GitRevert GetRevert(int revertId, [ClientParameterType(typeof (Guid), true)] string repositoryId, [ClientIgnore] string projectId = null)
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
