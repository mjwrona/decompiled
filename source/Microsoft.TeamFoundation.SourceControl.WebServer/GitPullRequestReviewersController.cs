// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestReviewersController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
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
  public class GitPullRequestReviewersController : GitPullRequestsBaseController
  {
    [HttpGet]
    [ClientLocationId("4B6702C7-AA35-4B89-9C96-B9ABF6D3E540")]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests__pullRequestId__reviewers.json", null, null, null)]
    public IList<IdentityRefWithVote> GetPullRequestReviewers(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      [ClientIgnore] string projectId = null)
    {
      int result;
      if (string.IsNullOrEmpty(pullRequestId) || !int.TryParse(pullRequestId, out result))
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestId"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        return (IList<IdentityRefWithVote>) (this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, result) ?? throw new GitPullRequestNotFoundException()).GenerateVoters(this.TfsRequestContext, tfsGitRepository.Key, out IdentityRef _, out IdentityRef _, out IdentityRef _);
    }

    [HttpGet]
    [ClientLocationId("4B6702C7-AA35-4B89-9C96-B9ABF6D3E540")]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests__pullRequestId__reviewers__additionalReviewerId_.json", null, null, null)]
    public IdentityRefWithVote GetPullRequestReviewer(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      string reviewerId,
      [ClientIgnore] string projectId = null)
    {
      if (string.IsNullOrEmpty(reviewerId))
        throw new InvalidArgumentValueException(Resources.Get("InvalidReviewer"));
      IdentityRefWithVote pullRequestReviewer = this.GetPullRequestReviewers(repositoryId, pullRequestId, projectId).FirstOrDefault<IdentityRefWithVote>((Func<IdentityRefWithVote, bool>) (r => string.Equals(r.Id, reviewerId, StringComparison.OrdinalIgnoreCase)));
      if (pullRequestReviewer == null)
        throw new InvalidArgumentValueException(Resources.Get("InvalidReviewer"));
      GitPullRequestsBaseController.ThrowIfIdentityInvalid(pullRequestReviewer.Id);
      return pullRequestReviewer;
    }

    [HttpPost]
    [ClientLocationId("4B6702C7-AA35-4B89-9C96-B9ABF6D3E540")]
    [ClientResponseType(typeof (IList<IdentityRefWithVote>), null, null)]
    public HttpResponseMessage CreatePullRequestReviewers(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      [ClientParameterType(typeof (IdentityRef[]), false)] IdentityRefWithVote[] reviewers,
      [ClientIgnore] string projectId = null)
    {
      int result;
      if (string.IsNullOrEmpty(pullRequestId) || !int.TryParse(pullRequestId, out result))
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestId"));
      if (reviewers == null || reviewers.Length < 1)
        throw new InvalidArgumentValueException(Resources.Get("InvalidReviewer"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        if (service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, result) == null)
          throw new GitPullRequestNotFoundException();
        List<TfsGitPullRequest.ReviewerBase> reviewersToUpdate = this.ProcessReviewersList(reviewers, tfsGitRepository);
        return this.Request.CreateResponse<IList<IdentityRefWithVote>>(HttpStatusCode.OK, (IList<IdentityRefWithVote>) service.UpdatePullRequestReviewers(this.TfsRequestContext, tfsGitRepository, result, (IEnumerable<TfsGitPullRequest.ReviewerBase>) reviewersToUpdate, Enumerable.Empty<Guid>(), true, true, true).ToIdentityRefWithVotes(this.TfsRequestContext, tfsGitRepository.Key, result));
      }
    }

    [HttpPut]
    [ClientLocationId("4B6702C7-AA35-4B89-9C96-B9ABF6D3E540")]
    [ClientResponseType(typeof (IdentityRefWithVote), null, null)]
    public HttpResponseMessage CreateUnmaterializedPullRequestReviewer(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      IdentityRefWithVote reviewer,
      [ClientIgnore] string projectId = null)
    {
      int result;
      if (string.IsNullOrEmpty(pullRequestId) || !int.TryParse(pullRequestId, out result))
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestId"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        if (service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, result) == null)
          throw new GitPullRequestNotFoundException();
        List<TfsGitPullRequest.ReviewerBase> reviewersToUpdate = this.ProcessReviewersList(new IdentityRefWithVote[1]
        {
          reviewer
        }, tfsGitRepository);
        return this.Request.CreateResponse<IdentityRefWithVote>(HttpStatusCode.OK, ((IEnumerable<IdentityRefWithVote>) service.UpdatePullRequestReviewers(this.TfsRequestContext, tfsGitRepository, result, (IEnumerable<TfsGitPullRequest.ReviewerBase>) reviewersToUpdate, Enumerable.Empty<Guid>(), true, true, true).ToIdentityRefWithVotes(this.TfsRequestContext, tfsGitRepository.Key, result)).FirstOrDefault<IdentityRefWithVote>());
      }
    }

    [HttpDelete]
    [ClientLocationId("4B6702C7-AA35-4B89-9C96-B9ABF6D3E540")]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE__git_repositories__repositoryId__pullRequests__pullRequestId__reviewers__additionalReviewerId_.json", null, null, null)]
    public HttpResponseMessage DeletePullRequestReviewer(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      string reviewerId,
      [ClientIgnore] string projectId = null)
    {
      int result;
      if (string.IsNullOrEmpty(pullRequestId) || !int.TryParse(pullRequestId, out result))
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestId"));
      if (string.IsNullOrEmpty(reviewerId))
        throw new InvalidArgumentValueException(Resources.Get("InvalidReviewer"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        Guid guid = GitPullRequestsBaseController.ThrowIfIdentityInvalid(reviewerId);
        this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().UpdatePullRequestReviewers(this.TfsRequestContext, tfsGitRepository, result, Enumerable.Empty<TfsGitPullRequest.ReviewerBase>(), (IEnumerable<Guid>) new Guid[1]
        {
          guid
        }, true, true, true);
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
    }

    [HttpPut]
    [ClientLocationId("4B6702C7-AA35-4B89-9C96-B9ABF6D3E540")]
    [ClientResponseType(typeof (IdentityRefWithVote), null, null)]
    [ClientExample("PUT__git_repositories__repositoryId__pullRequests__pullRequestId__reviewers__initialReviewerId_.json", "Set vote", null, null)]
    [ClientExample("PUT__git_repositories__repositoryId__pullRequests__pullRequestId__reviewers__additionalReviewerId_.json", "Add a reviewer", null, null)]
    public HttpResponseMessage CreatePullRequestReviewer(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      string reviewerId,
      IdentityRefWithVote reviewer,
      [ClientIgnore] string projectId = null)
    {
      int result1;
      if (string.IsNullOrEmpty(pullRequestId) || !int.TryParse(pullRequestId, out result1))
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestId"));
      if (string.IsNullOrEmpty(reviewerId))
        throw new InvalidArgumentValueException(Resources.Get("InvalidReviewer"));
      if (reviewer == null)
        throw new InvalidArgumentValueException(Resources.Get("MissingVote"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        Guid tfid = GitPullRequestsBaseController.ThrowIfIdentityInvalid(reviewerId);
        Guid result2;
        if (!string.IsNullOrEmpty(reviewer.Id) && (!Guid.TryParse(reviewer.Id, out result2) || tfid != result2))
          throw new InvalidArgumentValueException(Resources.Get("InvalidReviewer"));
        IVssSecurityNamespace securityNamespace;
        string securityToken = GitPermissionsUtility.CreateSecurityToken(this.TfsRequestContext, tfsGitRepository, out securityNamespace);
        Guid teamFoundationId = this.TfsRequestContext.GetService<ITeamFoundationIdentityService>().ReadRequestIdentity(this.TfsRequestContext).TeamFoundationId;
        IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewerWithVotes = (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) Array.Empty<TfsGitPullRequest.ReviewerWithVote>();
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        bool flag = true;
        if (tfid == teamFoundationId)
        {
          reviewerWithVotes = service.UpdateCurrentUserVote(this.TfsRequestContext, tfsGitRepository, result1, (ReviewerVote) reviewer.Vote, reviewer.IsFlagged, reviewer.HasDeclined, reviewer.IsReapprove);
          bool? isRequired1 = reviewerWithVotes.FirstOrDefault<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (r => r.Reviewer == tfid))?.IsRequired;
          bool isRequired2 = reviewer.IsRequired;
          flag = !(isRequired1.GetValueOrDefault() == isRequired2 & isRequired1.HasValue);
        }
        if (flag)
        {
          if (tfid != teamFoundationId)
          {
            if (reviewer.Vote != (short) 0)
              throw new InvalidArgumentValueException(Resources.Format("CannotVoteForAnother", (object) tfid.ToString()));
            GitPermissionsUtility.VerifyPermissionsForTfid(this.TfsRequestContext, tfid, securityNamespace, securityToken, false, false);
          }
          reviewerWithVotes = service.UpdatePullRequestReviewers(this.TfsRequestContext, tfsGitRepository, result1, (IEnumerable<TfsGitPullRequest.ReviewerBase>) new TfsGitPullRequest.ReviewerBase[1]
          {
            new TfsGitPullRequest.ReviewerBase(tfid, reviewer.IsRequired)
          }, Enumerable.Empty<Guid>(), true, true, true);
        }
        return this.Request.CreateResponse<IdentityRefWithVote>(HttpStatusCode.OK, ((IEnumerable<IdentityRefWithVote>) reviewerWithVotes.ToIdentityRefWithVotes(this.TfsRequestContext, tfsGitRepository.Key, result1)).FirstOrDefault<IdentityRefWithVote>((Func<IdentityRefWithVote, bool>) (id => string.Equals(id.Id, reviewerId, StringComparison.OrdinalIgnoreCase))));
      }
    }

    [HttpPatch]
    [ClientLocationId("4B6702C7-AA35-4B89-9C96-B9ABF6D3E540")]
    [RequestContentTypeRestriction(AllowJson = true)]
    public void UpdatePullRequestReviewers(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      [FromBody] List<IdentityRefWithVote> patchVotes,
      [ClientIgnore] string projectId = null)
    {
      int result1;
      if (string.IsNullOrEmpty(pullRequestId) || !int.TryParse(pullRequestId, out result1))
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestId"));
      List<Guid> reviewerGuidsToReset = patchVotes != null ? new List<Guid>(patchVotes.Count) : throw new InvalidArgumentValueException(Resources.Get("InvalidReviewerId"));
      foreach (IdentityRefWithVote patchVote in patchVotes)
      {
        Guid result2;
        if (!Guid.TryParse(patchVote.Id, out result2))
          throw new InvalidArgumentValueException(Resources.Get("InvalidReviewerId"));
        if (patchVote.DirectoryAlias != null || patchVote.DisplayName != null || patchVote.ImageUrl != null || patchVote.Inactive || patchVote.IsRequired || patchVote.IsAadIdentity || patchVote.IsContainer || patchVote.ProfileUrl != null || patchVote.ReviewerUrl != null || patchVote.UniqueName != null || patchVote.Url != null || patchVote.VotedFor != null)
          throw new InvalidArgumentValueException(Resources.Get("OnlyPatchVoteValue"));
        if (patchVote.Vote != (short) 0)
          throw new InvalidArgumentValueException(Resources.Get("MustSetVoteToZero"));
        reviewerGuidsToReset.Add(result2);
      }
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        IVssSecurityNamespace securityNamespace;
        string securityToken = GitPermissionsUtility.CreateSecurityToken(this.TfsRequestContext, tfsGitRepository, out securityNamespace);
        Guid teamFoundationId = this.TfsRequestContext.GetService<ITeamFoundationIdentityService>().ReadRequestIdentity(this.TfsRequestContext).TeamFoundationId;
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        foreach (Guid tfId in reviewerGuidsToReset)
          GitPermissionsUtility.VerifyPermissionsForTfid(this.TfsRequestContext, tfId, securityNamespace, securityToken, false, false);
        service.ResetSomeReviewerVotes(this.TfsRequestContext, tfsGitRepository, result1, (IList<Guid>) reviewerGuidsToReset);
      }
    }

    [HttpPatch]
    [FeatureEnabled("SourceControl.GitPullRequests.ReviewerFlags")]
    [ClientLocationId("4B6702C7-AA35-4B89-9C96-B9ABF6D3E540")]
    [RequestContentTypeRestriction(AllowJson = true)]
    [ClientResponseType(typeof (IdentityRefWithVote), null, null)]
    public HttpResponseMessage UpdatePullRequestReviewer(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      string reviewerId,
      IdentityRefWithVote reviewer,
      [ClientIgnore] string projectId = null)
    {
      int result1;
      if (string.IsNullOrEmpty(pullRequestId) || !int.TryParse(pullRequestId, out result1))
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestId"));
      if (string.IsNullOrEmpty(reviewerId))
        throw new InvalidArgumentValueException(Resources.Get("InvalidReviewer"));
      if (reviewer == null || !reviewer.IsFlagged.HasValue && !reviewer.HasDeclined.HasValue)
        throw new InvalidArgumentValueException(Resources.Get("MissingReviewerFlagOrDecline"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        Guid guid = GitPullRequestsBaseController.ThrowIfIdentityInvalid(reviewerId);
        Guid result2;
        if (!string.IsNullOrEmpty(reviewer.Id) && (!Guid.TryParse(reviewer.Id, out result2) || guid != result2))
          throw new InvalidArgumentValueException(Resources.Get("InvalidReviewer"));
        IVssSecurityNamespace securityNamespace;
        string securityToken = GitPermissionsUtility.CreateSecurityToken(this.TfsRequestContext, tfsGitRepository, out securityNamespace);
        GitPermissionsUtility.VerifyPermissionsForTfid(this.TfsRequestContext, guid, securityNamespace, securityToken, false, false);
        return this.Request.CreateResponse<IdentityRefWithVote>(HttpStatusCode.OK, ((IEnumerable<IdentityRefWithVote>) this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().UpdateReviewer(this.TfsRequestContext, tfsGitRepository, result1, guid, reviewer.IsFlagged, reviewer.HasDeclined).ToIdentityRefWithVotes(this.TfsRequestContext, tfsGitRepository.Key, result1)).FirstOrDefault<IdentityRefWithVote>((Func<IdentityRefWithVote, bool>) (id => string.Equals(id.Id, reviewerId, StringComparison.OrdinalIgnoreCase))));
      }
    }
  }
}
