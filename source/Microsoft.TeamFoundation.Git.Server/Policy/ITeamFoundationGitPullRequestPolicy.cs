// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.ITeamFoundationGitPullRequestPolicy
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  [InheritedExport]
  public interface ITeamFoundationGitPullRequestPolicy : 
    ITeamFoundationPolicy,
    ITfsGitRefUpdatePolicy
  {
    PolicyCheckResult CheckRefUpdate(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      string name,
      Sha1Id oldObjectId,
      Sha1Id newObjectId,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyCheckResult CheckEnterCompletionQueue(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitPullRequestCompletionOptions completionOptions,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    AutoCompleteStatus GetAutoCompleteStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitPullRequestCompletionOptions completionOptions,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult OnCreated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest);

    PolicyNotificationResult OnReactivated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult OnAbandoned(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    TeamFoundationPolicyEvaluationRecordContext OnCommitted(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus statusFromCheckRefUpdate,
      TeamFoundationPolicyEvaluationRecordContext contextFromCheckRefUpdate);

    PolicyNotificationResult OnReviewerVoteChanged(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      TeamFoundationIdentity reviewer,
      ReviewerVote vote,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult OnReviewerListUpdated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      IEnumerable<Guid> addedReviewers,
      IEnumerable<Guid> removedReviewers,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult OnSourceBranchUpdated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      Sha1Id oldCommit,
      Sha1Id newCommit,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult OnTargetBranchWillChange(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      string newTargetRef,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult OnTargetBranchChanged(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      string newTargetRef,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult OnPublished(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult OnNewMergeCommitAvailable(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      Sha1Id mergeCommit,
      Sha1Id conflictResolutionHash,
      bool inCompletionQueue,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult OnNewPullRequestStatusAdded(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      GitPullRequestStatus pullRequestStatus,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult OnPullRequestStatusesDeleted(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext);

    PolicyNotificationResult AutoRequeue(
      IVssRequestContext requestContext,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext,
      GitPullRequestTarget pullRequest);
  }
}
