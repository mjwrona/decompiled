// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Hub.IPullRequestDetailClient
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Hub
{
  public interface IPullRequestDetailClient
  {
    void OnPullRequestUpdate(int pullRequestId, string message, string[] args);

    void OnAutoCompleteUpdated(AutoCompleteUpdatedEvent autoCompleteUpdatedEvent);

    void OnBranchUpdated(BranchUpdatedEvent branchUpdatedEvent);

    void OnCompletionErrors(CompletionErrorsEvent completionErrorsEvent);

    void OnDiscussionsUpdated(DiscussionsUpdatedEvent discussionsUpdatedEvent);

    void OnIsDraftUpdated(IsDraftUpdatedEvent isDraftUpdatedEvent);

    void OnLabelsUpdated(LabelsUpdatedEvent labelsUpdatedEvent);

    void OnMergeCompleted(MergeCompletedEvent mergeCompletedEvent);

    void OnPolicyEvaluationUpdated(
      PolicyEvaluationUpdatedEvent policyEvaluationUpdatedEvent);

    void OnReviewersUpdated(ReviewersUpdatedEvent reviewersUpdatedEvent);

    void OnReviewersVotesReset(ReviewersVotesResetEvent reviewersVotesResetEvent);

    void OnReviewerVoteUpdated(ReviewerVoteUpdatedEvent reviewerVoteUpdatedEvent);

    void OnStatusAdded(StatusAddedEvent statusAddedEvent);

    void OnStatusesDeleted(StatusesDeletedEvent statusesDeletedEvent);

    void OnStatusUpdated(StatusUpdatedEvent statusUpdatedEvent);

    void OnTitleDescriptionUpdated(
      TitleDescriptionUpdatedEvent titleDescriptionUpdatedEvent);

    void OnRealTimePullRequestUpdated(RealTimePullRequestEvent realTimePullRequestEvent);

    void OnTargetChanged(RetargetEvent retargetEvent);
  }
}
