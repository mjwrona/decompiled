// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CodeReviewDiscussionConstants
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [GenerateAllConstants(null)]
  public static class CodeReviewDiscussionConstants
  {
    public static readonly string CodeReviewThreadType = nameof (CodeReviewThreadType);
    public static readonly string CodeReviewMergeStatus = nameof (CodeReviewMergeStatus);
    public static readonly string CodeReviewMergeCommit = nameof (CodeReviewMergeCommit);
    public static readonly string CodeReviewSourceCommit = nameof (CodeReviewSourceCommit);
    public static readonly string CodeReviewTargetCommit = nameof (CodeReviewTargetCommit);
    public static readonly string CodeReviewRefName = nameof (CodeReviewRefName);
    public static readonly string CodeReviewRefNewHeadCommit = nameof (CodeReviewRefNewHeadCommit);
    public static readonly string CodeReviewRefNewCommits = nameof (CodeReviewRefNewCommits);
    public static readonly string CodeReviewRefNewCommitsCount = nameof (CodeReviewRefNewCommitsCount);
    public static readonly string CodeReviewRefUpdatedBy = nameof (CodeReviewRefUpdatedBy);
    public static readonly string CodeReviewRefUpdatedByDisplayName = nameof (CodeReviewRefUpdatedByDisplayName);
    public static readonly string CodeReviewRefUpdatedByTfId = nameof (CodeReviewRefUpdatedByTfId);
    public static readonly string CodeReviewVotedByDisplayName = nameof (CodeReviewVotedByDisplayName);
    public static readonly string CodeReviewVotedByTfId = nameof (CodeReviewVotedByTfId);
    public static readonly string CodeReviewVotedByInitiatorDisplayName = nameof (CodeReviewVotedByInitiatorDisplayName);
    public static readonly string CodeReviewVotedByInitiatorTfId = nameof (CodeReviewVotedByInitiatorTfId);
    public static readonly string CodeReviewVoteReason = nameof (CodeReviewVoteReason);
    public static readonly string CodeReviewVoteResult = nameof (CodeReviewVoteResult);
    public static readonly string CodeReviewResetAllVotesReason = nameof (CodeReviewResetAllVotesReason);
    public static readonly string CodeReviewResetAllVotesInitiatorDisplayName = nameof (CodeReviewResetAllVotesInitiatorDisplayName);
    public static readonly string CodeReviewResetAllVotesInitiatorTfId = nameof (CodeReviewResetAllVotesInitiatorTfId);
    public static readonly string CodeReviewResetMultipleVotesReason = nameof (CodeReviewResetMultipleVotesReason);
    public static readonly string CodeReviewResetMultipleVotesInitiatorDisplayName = nameof (CodeReviewResetMultipleVotesInitiatorDisplayName);
    public static readonly string CodeReviewResetMultipleVotesInitiatorTfId = nameof (CodeReviewResetMultipleVotesInitiatorTfId);
    public static readonly string CodeReviewResetMultipleVotesNumVoters = nameof (CodeReviewResetMultipleVotesNumVoters);
    public static readonly string CodeReviewResetMultipleVotesExampleVoterDisplayNames = nameof (CodeReviewResetMultipleVotesExampleVoterDisplayNames);
    public static readonly string CodeReviewResetMultipleVotesExampleVoterIds = nameof (CodeReviewResetMultipleVotesExampleVoterIds);
    public static readonly string CodeReviewStatusUpdatedByDisplayName = nameof (CodeReviewStatusUpdatedByDisplayName);
    public static readonly string CodeReviewStatusUpdatedByTfId = nameof (CodeReviewStatusUpdatedByTfId);
    public static readonly string CodeReviewStatus = nameof (CodeReviewStatus);
    public static readonly string CodeReviewIterationPublishedByDisplayName = nameof (CodeReviewIterationPublishedByDisplayName);
    public static readonly string CodeReviewIterationPublishedByTfId = nameof (CodeReviewIterationPublishedByTfId);
    public static readonly string CodeReviewIterationId = nameof (CodeReviewIterationId);
    public static readonly string CodeReviewSourceBranchChanged = nameof (CodeReviewSourceBranchChanged);
    public static readonly string CodeReviewSourceBranchChangedByDisplayName = nameof (CodeReviewSourceBranchChangedByDisplayName);
    public static readonly string CodeReviewSourceBranchChangedByTfId = nameof (CodeReviewSourceBranchChangedByTfId);
    public static readonly string CodeReviewStatusUpdateAssociatedCommit = nameof (CodeReviewStatusUpdateAssociatedCommit);
    public static readonly string CodeReviewPolicyType = nameof (CodeReviewPolicyType);
    public static readonly string CodeReviewReviewersUpdatedByDisplayname = nameof (CodeReviewReviewersUpdatedByDisplayname);
    public static readonly string CodeReviewReviewersUpdatedByTfId = nameof (CodeReviewReviewersUpdatedByTfId);
    public static readonly string CodeReviewReviewersUpdatedNumAdded = nameof (CodeReviewReviewersUpdatedNumAdded);
    public static readonly string CodeReviewReviewersUpdatedNumChanged = nameof (CodeReviewReviewersUpdatedNumChanged);
    public static readonly string CodeReviewReviewersUpdatedNumDeclined = nameof (CodeReviewReviewersUpdatedNumDeclined);
    public static readonly string CodeReviewReviewersUpdatedNumRemoved = nameof (CodeReviewReviewersUpdatedNumRemoved);
    public static readonly string CodeReviewReviewersUpdatedAddedDisplayName = nameof (CodeReviewReviewersUpdatedAddedDisplayName);
    public static readonly string CodeReviewReviewersUpdatedAddedTfId = nameof (CodeReviewReviewersUpdatedAddedTfId);
    public static readonly string CodeReviewReviewersUpdatedChangedDisplayName = nameof (CodeReviewReviewersUpdatedChangedDisplayName);
    public static readonly string CodeReviewReviewersUpdatedChangedTfId = nameof (CodeReviewReviewersUpdatedChangedTfId);
    public static readonly string CodeReviewReviewersUpdatedChangedToRequired = nameof (CodeReviewReviewersUpdatedChangedToRequired);
    public static readonly string CodeReviewReviewersUpdatedChangedDecline = nameof (CodeReviewReviewersUpdatedChangedDecline);
    public static readonly string CodeReviewReviewersUpdatedRemovedDisplayName = nameof (CodeReviewReviewersUpdatedRemovedDisplayName);
    public static readonly string CodeReviewReviewersUpdatedRemovedTfId = nameof (CodeReviewReviewersUpdatedRemovedTfId);
    public static readonly string CodeReviewAutoCompleteUpdatedByDisplayName = nameof (CodeReviewAutoCompleteUpdatedByDisplayName);
    public static readonly string CodeReviewAutoCompleteUpdatedByTfId = nameof (CodeReviewAutoCompleteUpdatedByTfId);
    public static readonly string CodeReviewAutoCompleteNowSet = nameof (CodeReviewAutoCompleteNowSet);
    public static readonly string CodeReviewAutoCompleteFailedReason = nameof (CodeReviewAutoCompleteFailedReason);
    public static readonly string CodeReviewIsDraftUpdatedByDisplayName = nameof (CodeReviewIsDraftUpdatedByDisplayName);
    public static readonly string CodeReviewIsDraftUpdatedByTfId = nameof (CodeReviewIsDraftUpdatedByTfId);
    public static readonly string CodeReviewIsDraftNowSet = nameof (CodeReviewIsDraftNowSet);
    public static readonly string CodeReviewAssociatedStatusUpdatedByDisplayName = nameof (CodeReviewAssociatedStatusUpdatedByDisplayName);
    public static readonly string CodeReviewAssociatedStatusUpdatedByTfId = nameof (CodeReviewAssociatedStatusUpdatedByTfId);
    public static readonly string CodeReviewAssociatedStatusName = nameof (CodeReviewAssociatedStatusName);
    public static readonly string CodeReviewAssociatedStatus = nameof (CodeReviewAssociatedStatus);
    public static readonly string CodeReviewAssociatedStatusId = nameof (CodeReviewAssociatedStatusId);
    public static readonly string CodeReviewAssociatedStatusIds = nameof (CodeReviewAssociatedStatusIds);
    public static readonly string CodeReviewAssociatedStatusIterationId = nameof (CodeReviewAssociatedStatusIterationId);
    public static readonly string CodeReviewAssociatedStatusTargetUrl = nameof (CodeReviewAssociatedStatusTargetUrl);
    public static readonly string CodeReviewAssociatedStatusContextGenre = nameof (CodeReviewAssociatedStatusContextGenre);
    public static readonly string CodeReviewAssociatedStatusContextName = nameof (CodeReviewAssociatedStatusContextName);
    public static readonly string CodeReviewTargetChangedByDisplayName = nameof (CodeReviewTargetChangedByDisplayName);
    public static readonly string CodeReviewTargetChangedByTfId = nameof (CodeReviewTargetChangedByTfId);
    public static readonly string CodeReviewAttachmentUpdatedByDisplayName = nameof (CodeReviewAttachmentUpdatedByDisplayName);
    public static readonly string CodeReviewAttachmentUpdatedById = nameof (CodeReviewAttachmentUpdatedById);
    public static readonly string CodeReviewAttachmentDisplayName = nameof (CodeReviewAttachmentDisplayName);
    public static readonly string BypassPolicy = nameof (BypassPolicy);
    public static readonly string BypassReason = nameof (BypassReason);
    public static readonly string CodeReviewRequiredReviewerExamplePathThatTriggered = nameof (CodeReviewRequiredReviewerExamplePathThatTriggered);
    public static readonly string CodeReviewRequiredReviewerNumFilesThatTriggered = nameof (CodeReviewRequiredReviewerNumFilesThatTriggered);
    public static readonly string CodeReviewRequiredReviewerUserConfiguredMessage = nameof (CodeReviewRequiredReviewerUserConfiguredMessage);
    public static readonly string CodeReviewRequiredReviewerExampleReviewerDisplayName = nameof (CodeReviewRequiredReviewerExampleReviewerDisplayName);
    public static readonly string CodeReviewRequiredReviewerExampleReviewerId = nameof (CodeReviewRequiredReviewerExampleReviewerId);
    public static readonly string CodeReviewRequiredReviewerNumReviewers = nameof (CodeReviewRequiredReviewerNumReviewers);
    public static readonly string CodeReviewRequiredReviewerIsRequired = nameof (CodeReviewRequiredReviewerIsRequired);
    public static readonly string MergeAttempt = nameof (MergeAttempt);
    public static readonly string RefUpdate = nameof (RefUpdate);
    public static readonly string VoteUpdate = nameof (VoteUpdate);
    public static readonly string ResetAllVotes = nameof (ResetAllVotes);
    public static readonly string ResetMultipleVotes = nameof (ResetMultipleVotes);
    public static readonly string StatusUpdate = nameof (StatusUpdate);
    public static readonly string PolicyStatusUpdate = nameof (PolicyStatusUpdate);
    public static readonly string ReviewersUpdate = nameof (ReviewersUpdate);
    public static readonly string AutoCompleteUpdate = nameof (AutoCompleteUpdate);
    public static readonly string IsDraftUpdate = nameof (IsDraftUpdate);
    public static readonly string AssociatedStatusUpdate = nameof (AssociatedStatusUpdate);
    public static readonly string AttachmentUpdate = nameof (AttachmentUpdate);
    public static readonly string TargetChanged = nameof (TargetChanged);
    public static readonly string ConflictsResolved = nameof (ConflictsResolved);
    public static readonly string BrokenState = nameof (BrokenState);
    public static readonly string SourceBranchChanged = nameof (SourceBranchChanged);
    public static readonly string IterationPublished = nameof (IterationPublished);
  }
}
