// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitCherryPicksController
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
  [FeatureEnabled("SourceControl.CherryPick")]
  public class GitCherryPicksController : GitApiController
  {
    [HttpPost]
    [ClientLocationId("033BAD68-9A14-43D1-90E0-59CB8856FEF6")]
    [ClientResponseType(typeof (GitCherryPick), null, null)]
    public virtual HttpResponseMessage CreateCherryPick(
      [FromBody] Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationParameters cherryPickToCreate,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      GitCherryPick cherryPickInternal = this.CreateCherryPickInternal(cherryPickToCreate, repositoryId, projectId);
      GitStatusStateMapper.MapGitEntity<GitCherryPick>(cherryPickInternal, this.TfsRequestContext);
      return this.Request.CreateResponse<GitCherryPick>(HttpStatusCode.Created, cherryPickInternal);
    }

    protected GitCherryPick CreateCherryPickInternal(
      Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationParameters cherryPickToCreate,
      string repositoryId,
      string projectId)
    {
      if (cherryPickToCreate == null || string.IsNullOrEmpty(cherryPickToCreate.OntoRefName) || string.IsNullOrEmpty(cherryPickToCreate.GeneratedRefName))
        throw new InvalidArgumentValueException(Resources.Get("MissingCherryPickRef"));
      if (cherryPickToCreate.Source == null)
        throw new GitAsyncRefOperationInvalidSourceException(Resources.Get("CherryPickNeedsSingleSource"));
      if (this.TfsRequestContext.IsFeatureEnabled("SourceControl.EnableReturningPartiallySucceededGitStatusState") && cherryPickToCreate.Source.CommitList != null && ((IEnumerable<GitCommitRef>) cherryPickToCreate.Source.CommitList).Any<GitCommitRef>((Func<GitCommitRef, bool>) (c => c?.Statuses != null && c.Statuses.Any<GitStatus>((Func<GitStatus, bool>) (s => s != null && s.State == GitStatusState.PartiallySucceeded)))))
        throw new InvalidArgumentValueException(Resources.Get("InvalidGitStatusStateValue"), "State");
      int num1 = cherryPickToCreate.Source.PullRequestId.HasValue ? 1 : 0;
      GitCommitRef[] commitList = cherryPickToCreate.Source.CommitList;
      bool flag = commitList != null && commitList.Length != 0;
      int num2 = flag ? 1 : 0;
      if (num1 == num2)
        throw new GitAsyncRefOperationInvalidSourceException(Resources.Get("CherryPickNeedsSingleSource"));
      if (flag)
      {
        foreach (GitCommitRef commit in cherryPickToCreate.Source.CommitList)
        {
          if (commit?.CommitId == null)
            throw new ArgumentNullException("commit");
        }
      }
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        if (cherryPickToCreate.Repository != null && cherryPickToCreate.Repository.Id != Guid.Empty && cherryPickToCreate.Repository.Id != tfsGitRepository.Key.RepoId)
          throw new InvalidArgumentValueException(Resources.Get("MismatchRepositoryId"));
        try
        {
          return this.TfsRequestContext.GetService<ITeamFoundationGitCherryPickService>().CreateAsyncRefOperation(this.TfsRequestContext, tfsGitRepository, cherryPickToCreate.ToServerItem()).ToWebApiCherryPickItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
        }
        catch (GitAsyncRefOperationInvalidOntoOrGeneratedRefs ex)
        {
          throw new InvalidArgumentValueException(nameof (cherryPickToCreate), (Exception) ex);
        }
      }
    }

    [HttpGet]
    [ClientLocationId("033BAD68-9A14-43D1-90E0-59CB8856FEF6")]
    public virtual GitCherryPick GetCherryPickForRefName(
      [FromUri] string refName,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      if (refName == null)
        throw new InvalidArgumentValueException(refName);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        GitCherryPick apiCherryPickItem = this.TfsRequestContext.GetService<ITeamFoundationGitCherryPickService>().GetAsyncRefOperationByRef(this.TfsRequestContext, tfsGitRepository, refName).ToWebApiCherryPickItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
        GitStatusStateMapper.MapGitEntity<GitCherryPick>(apiCherryPickItem, this.TfsRequestContext);
        return apiCherryPickItem;
      }
    }

    [HttpGet]
    [ClientLocationId("033BAD68-9A14-43D1-90E0-59CB8856FEF6")]
    public virtual GitCherryPick GetCherryPick(
      int cherryPickId,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        GitCherryPick apiCherryPickItem = this.TfsRequestContext.GetService<ITeamFoundationGitCherryPickService>().GetAsyncRefOperationById(this.TfsRequestContext, tfsGitRepository, cherryPickId).ToWebApiCherryPickItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
        GitStatusStateMapper.MapGitEntity<GitCherryPick>(apiCherryPickItem, this.TfsRequestContext);
        return apiCherryPickItem;
      }
    }
  }
}
