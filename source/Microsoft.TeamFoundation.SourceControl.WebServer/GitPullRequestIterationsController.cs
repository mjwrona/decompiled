// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestIterationsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer.ObjectModel;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "pullRequestIterations", ResourceVersion = 1)]
  public class GitPullRequestIterationsController : GitApiController
  {
    protected static readonly IEnumerable<IterationReason> s_version1SupportedReasons = (IEnumerable<IterationReason>) new List<IterationReason>()
    {
      IterationReason.Push,
      IterationReason.ForcePush,
      IterationReason.Create,
      IterationReason.Rebase,
      IterationReason.Unknown
    };

    [HttpGet]
    [ClientResponseType(typeof (List<GitPullRequestIteration>), null, null)]
    [ClientLocationId("D43911EE-6958-46B0-A42B-8445B8A0D004")]
    public virtual HttpResponseMessage GetPullRequestIterations(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [ClientIgnore] string projectId = null,
      [FromUri(Name = "includeCommits")] bool includeCommits = false)
    {
      return this.GenerateResponse<GitPullRequestIteration>((IEnumerable<GitPullRequestIteration>) this.GetPullRequestIterationsInternal(repositoryId, pullRequestId, projectId, includeCommits, GitPullRequestIterationsController.s_version1SupportedReasons));
    }

    protected List<GitPullRequestIteration> GetPullRequestIterationsInternal(
      string repositoryId,
      int pullRequestId,
      string projectId,
      bool includeCommits,
      IEnumerable<IterationReason> supportedReasons)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        List<GitPullRequestIteration> iterations = service.GetIterations(this.TfsRequestContext, tfsGitRepository, pullRequestDetails).ToList<GitPullRequestIteration>().FilterOutPartiallySavedIterations(this.TfsRequestContext, pullRequestDetails);
        if (includeCommits)
        {
          List<IterationCommitsData> commitsBatch = pullRequestDetails.GenerateCommitsBatch(this.TfsRequestContext, tfsGitRepository, (IList<GitPullRequestIteration>) iterations, false);
          for (int index = 0; index < iterations.Count; ++index)
          {
            GitPullRequestIteration requestIteration1 = iterations[index];
            IterationCommitsData iterationCommitsData = commitsBatch[index];
            IList<GitCommitRef> commits = iterationCommitsData.Commits;
            requestIteration1.Commits = commits;
            GitPullRequestIteration requestIteration2 = iterations[index];
            iterationCommitsData = commitsBatch[index];
            int num = iterationCommitsData.HasMore ? 1 : 0;
            requestIteration2.hasMoreCommits = num != 0;
            GitPullRequestIteration requestIteration3 = iterations[index];
            iterationCommitsData = commitsBatch[index];
            int reason = (int) iterationCommitsData.Reason;
            requestIteration3.Reason = (IterationReason) reason;
          }
        }
        iterations.ForEach((Action<GitPullRequestIteration>) (i => this.EnsureSupportedReason(i, supportedReasons)));
        return iterations;
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (GitPullRequestIteration), null, null)]
    [ClientLocationId("D43911EE-6958-46B0-A42B-8445B8A0D004")]
    public virtual HttpResponseMessage GetPullRequestIteration(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      [ClientIgnore] string projectId = null)
    {
      return this.Request.CreateResponse<GitPullRequestIteration>(HttpStatusCode.OK, this.GetPullRequestIterationInternal(repositoryId, pullRequestId, iterationId, projectId, GitPullRequestIterationsController.s_version1SupportedReasons));
    }

    protected GitPullRequestIteration GetPullRequestIterationInternal(
      string repositoryId,
      int pullRequestId,
      int iterationId,
      string projectId,
      IEnumerable<IterationReason> supportedReasons)
    {
      ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        GitPullRequestIteration iteration = service.GetIteration(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, iterationId);
        iteration.AddReferenceLinks(this.TfsRequestContext, pullRequestDetails);
        this.EnsureSupportedReason(iteration, supportedReasons);
        return iteration;
      }
    }

    private void EnsureSupportedReason(
      GitPullRequestIteration iteration,
      IEnumerable<IterationReason> supportedReasons)
    {
      if (supportedReasons.Contains<IterationReason>(iteration.Reason))
        return;
      iteration.Reason = IterationReason.Unknown;
    }
  }
}
