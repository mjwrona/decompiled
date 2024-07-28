// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITeamFoundationGitPullRequestService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Policy.Server.Framework;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationGitPullRequestService))]
  public interface ITeamFoundationGitPullRequestService : IVssFrameworkService
  {
    TfsGitPullRequest CreatePullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string title,
      string description,
      string sourceBranchName,
      string targetBranchName,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewers,
      bool tryMerge,
      bool supportsIterations = true,
      GitPullRequestMergeOptions mergeOptions = null,
      IEnumerable<int> workItemIds = null,
      bool linkBranchWorkItems = false,
      bool linkCommitWorkItems = false,
      GitRepositoryRef sourceForkRepositoryRef = null,
      WebApiTagDefinition[] labels = null,
      bool isDraft = false);

    TfsGitPullRequest UpdatePullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      PullRequestStatus status,
      string title,
      string description,
      GitPullRequestCompletionOptions completionOptions,
      GitPullRequestMergeOptions mergeOptions,
      Guid? autoCompleteAuthority,
      bool? isDraft);

    TfsGitPullRequest RetargetPullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      string targetRefName);

    IEnumerable<TfsGitPullRequest> QueryPullRequestsBulk(
      IVssRequestContext requestContext,
      IEnumerable<Guid> assignedToIdFilters,
      PullRequestStatus statusFilter,
      Guid? creatorIdFilter = null,
      string teamProjectUri = null,
      Guid? repositoryId = null,
      int top = 1000);

    bool HasPullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId);

    TfsGitPullRequest GetPullRequestDetails(IVssRequestContext requestContext, int pullRequestId);

    TfsGitPullRequest GetPullRequestDetails(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId);

    TfsGitPullRequest GetPullRequestDetails(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId);

    bool TryGetPullRequestDetails(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      out TfsGitPullRequest pullRequest);

    IList<TfsGitPullRequest> QueryGitPullRequestsToBackfill(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      int firstPullRequestId = 1,
      int pullRequestsToFetch = 1000);

    IEnumerable<TfsGitPullRequest> QueryPullRequests(
      IVssRequestContext requestContext,
      string teamProjectUri = null,
      Guid? repositoryId = null,
      Guid? sourceRepositoryId = null,
      IEnumerable<string> sourceBranchFilters = null,
      IEnumerable<string> targetBranchFilters = null,
      bool? treatBranchFiltersAsUnion = null,
      PullRequestStatus? statusFilter = null,
      Guid? creatorIdFilter = null,
      Guid? assignedToIdFilter = null,
      bool orderAscending = false,
      TimeFilter timeFilter = null,
      int top = 1000,
      int skip = 0,
      bool completionAuthorityIsSet = false);

    IEnumerable<TfsGitPullRequest> QueryPullRequestsBySourceRepositoryRefs(
      IVssRequestContext requestContext,
      Guid sourceRepositoryId,
      IEnumerable<string> sourceBranchFilters);

    IEnumerable<TfsGitPullRequest> QueryActivePullRequestsBySourceAndTargetBranchFilters(
      IVssRequestContext requestContext,
      string teamProjectUri,
      Guid repositoryId,
      IEnumerable<string> branchFilters);

    IEnumerable<TfsGitPullRequest> QueryActiveTargetPullRequests(
      IVssRequestContext requestContext,
      Guid repositoryId,
      string branchFilter,
      int top);

    IEnumerable<TfsGitPullRequest> QueryActiveSourcePullRequests(
      IVssRequestContext requestContext,
      Guid sourceRepositoryId,
      string sourceBranchFilter,
      int top);

    IEnumerable<TfsGitPullRequest> QueryCompletedSourcePullRequests(
      IVssRequestContext requestContext,
      string teamProjectUri,
      Guid repositoryId,
      IEnumerable<string> sourceBranchFilters);

    IEnumerable<TfsGitPullRequest> QueryActiveSourcePullRequestsForBranches(
      IVssRequestContext requestContext,
      string teamProjectUri,
      Guid repositoryId,
      IEnumerable<string> sourceBranchFilters);

    IEnumerable<TfsGitPullRequest> QueryPullRequestsForSuggestions(
      IVssRequestContext requestContext,
      string teamProjectUri,
      Guid repositoryId,
      Guid sourceRepositoryId,
      IEnumerable<string> sourceBranchFilters);

    IEnumerable<TfsGitPullRequest> QueryPullRequestsMultiPerson(
      IVssRequestContext requestContext,
      string teamProjectUri = null,
      Guid? repositoryId = null,
      Guid? sourceRepositoryId = null,
      IEnumerable<string> sourceBranchFilters = null,
      IEnumerable<string> targetBranchFilters = null,
      bool? treatBranchFiltersAsUnion = null,
      PullRequestStatus? statusFilter = null,
      IEnumerable<Guid> creatorIdFilter = null,
      IEnumerable<Guid> assignedToIdFilter = null,
      bool orderAscending = false,
      int top = 1000,
      int skip = 0,
      bool completionAuthorityIsSet = false,
      DateTime? minCreationTime = null,
      DateTime? minClosedTime = null,
      DateTime? minUpdatedTime = null,
      bool? draftFilter = null);

    ILookup<Sha1Id, TfsGitPullRequest> QueryPullRequestsByMergeCommits(
      IVssRequestContext requestContext,
      IEnumerable<Sha1Id> commits,
      RepoKey repoScope);

    ILookup<Sha1Id, TfsGitPullRequest> QueryPullRequestsByCommits(
      IVssRequestContext requestContext,
      IEnumerable<Sha1Id> commits,
      RepoKey repoScope);

    void CreateGitCommitToPullRequests(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      IDictionary<int, IEnumerable<Sha1Id>> pullRequestCommits);

    bool RefHasActivePullRequests(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string refName,
      int threshold);

    IEnumerable<TfsGitPullRequest.ReviewerWithVote> ResetAllReviewerVotes(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      bool userInitiated = false,
      string reason = null);

    IEnumerable<TfsGitPullRequest.ReviewerWithVote> ResetSomeReviewerVotes(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      IList<Guid> reviewerGuidsToReset,
      bool userInitiated = false,
      string reason = null);

    IEnumerable<TfsGitPullRequest.ReviewerWithVote> UpdatePullRequestReviewers(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewersToUpdate,
      IEnumerable<Guid> reviewerIdsToDelete,
      bool createDiscussionMessage,
      bool sendEmailNotification,
      bool notifyPolicies,
      bool userInitiated = true);

    IEnumerable<TfsGitPullRequest.ReviewerWithVote> UpdateCurrentUserVote(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      ReviewerVote newVoteValue,
      bool? isFlagged = null,
      bool? hasDeclined = null,
      bool? isReapprove = null);

    IEnumerable<TfsGitPullRequest.ReviewerWithVote> UpdateReviewer(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      Guid reviewerId,
      bool? isFlagged,
      bool? hasDeclined);

    GitPullRequestIteration GetIteration(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int iterationId);

    IEnumerable<GitPullRequestIteration> GetIterations(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest);

    GitPullRequestStatus GetStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int statusId,
      int? iterationId = null);

    IEnumerable<GitPullRequestStatus> GetStatuses(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int? iterationId = null,
      bool includeProperties = false);

    ILookup<int, GitPullRequestStatus> GetStatuses(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<TfsGitPullRequest> pullRequests);

    IEnumerable<GitStatusContext> GetLatestStatusContexts(
      IVssRequestContext requestContext,
      ITfsGitRepository repository);

    IEnumerable<GitPullRequestStatus> GetLatestStatuses(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int top = 500);

    GitPullRequestStatus SaveStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      GitPullRequestStatus prStatus,
      int? iterationId = null);

    void DeleteStatuses(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      IEnumerable<int> statusIds,
      int? iterationId = null);

    PropertiesCollection GetPullRequestProperties(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest);

    PropertiesCollection UpdatePullRequestProperties(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      PropertiesCollection properties);

    GitPullRequestCommentThread GetThread(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int? iteration,
      int? baseIteration);

    IEnumerable<GitPullRequestCommentThread> GetThreads(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int? iteration,
      int? baseIteration);

    GitPullRequestCommentThread SaveThread(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      GitPullRequestCommentThread commentThread);

    Comment AddComment(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      Comment comment);

    Comment UpdateComment(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int commentId,
      Comment comment);

    void DeleteComment(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int commentId);

    GitPullRequestIterationChanges GetChanges(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int iterationId,
      out int totalChanges,
      int? top = null,
      int? skip = null,
      int? compareTo = null);

    void SharePullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      ShareNotificationContext userMessage);

    TfsGitPullRequest TryCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      Sha1Id lastSourceCommitSeen,
      GitPullRequestCompletionOptions completionOptions,
      bool useStrictBypass = false,
      bool triggeredByAutoComplete = false);

    TfsGitPullRequest TryMerge(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId);

    void LikeComment(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int commentId);

    List<IdentityRef> GetLikes(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int commentId);

    void WithdrawLikeComment(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int commentId);

    IEnumerable<TagDefinition> GetPullRequestLabels(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest);

    TagDefinition GetPullRequestLabel(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      string labelIdOrName);

    TagDefinition AddLabelToPullRequest(
      IVssRequestContext requestContext,
      string label,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      ClientTraceData ctData = null);

    IEnumerable<TagDefinition> AddLabelsToPullRequest(
      IVssRequestContext requestContext,
      string[] labels,
      ITfsGitRepository respository,
      TfsGitPullRequest pullRequest,
      ClientTraceData ctData = null);

    void VerifyLabelsForPullRequests(
      IVssRequestContext requestContext,
      string[] labels,
      ITfsGitRepository repository);

    void RemoveLabelFromPullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      string labelIdOrName,
      ClientTraceData ctData = null);

    List<PolicyConfigurationRecord> GetBlockingAutoCompletePolicies(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      ITfsGitRepository repository,
      ClientTraceData ctData = null,
      IActivePolicyEvaluationCache policyEvaluationCacheService = null);

    string GetTemplateContent(
      ITfsGitRepository targetRepository,
      string targetRef,
      string templateName,
      out IEnumerable<string> path);

    IEnumerable<string> GetTemplatesList(ITfsGitRepository targetRepository);

    IEnumerable<TfsGitPullRequest> GetPullRequestsChangedSinceLastWatermark(
      IVssRequestContext requestContext,
      DateTime updatedTime,
      int pullRequestId,
      int batchSize);
  }
}
