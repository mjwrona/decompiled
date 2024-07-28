// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.ServiceHooksEventTypeConstants
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [GenerateAllConstants(null)]
  public static class ServiceHooksEventTypeConstants
  {
    public const string AllEvents = "*";
    public const string BuildComplete = "build.complete";
    public const string GitPullRequestCreated = "git.pullrequest.created";
    public const string GitPullRequestUpdated = "git.pullrequest.updated";
    public const string GitPullRequestMergeCreated = "git.pullrequest.merged";
    public const string GitPullRequestCommented = "ms.vss-code.git-pullrequest-comment-event";
    public const string GitPush = "git.push";
    public const string GitClone = "git.clone";
    public const string GitFetch = "git.fetch";
    public const string TfvcCheckin = "tfvc.checkin";
    public const string GitBranchDownload = "git.branch.download";
    public const string CodeReviewCreated = "ms.vss-code.codereview-created-event";
    public const string CodeReviewUpdated = "ms.vss-code.codereview-updated-event";
    public const string CodeReviewIterationChanged = "ms.vss-code.codereview-iteration-changed-event";
    public const string CodeReviewReviewersChanged = "ms.vss-code.codereview-reviewers-changed-event";
    public const string ElasticPoolResized = "elasticagentpool.resized";
    public const string MessagePosted = "message.posted";
    public const string WorkItemCommented = "workitem.commented";
    public const string WorkItemCreated = "workitem.created";
    public const string WorkItemUpdated = "workitem.updated";
    public const string WorkItemDeleted = "workitem.deleted";
    public const string WorkItemRestored = "workitem.restored";
    public const string ReleaseCreated = "ms.vss-release.release-created-event";
    public const string ReleaseAbandoned = "ms.vss-release.release-abandoned-event";
    public const string DeploymentApprovalPending = "ms.vss-release.deployment-approval-pending-event";
    public const string DeploymentApprovalCompleted = "ms.vss-release.deployment-approval-completed-event";
    public const string DeploymentStarted = "ms.vss-release.deployment-started-event";
    public const string DeploymentCompleted = "ms.vss-release.deployment-completed-event";
    public const string DeploymentManualInterventionPending = "ms.vss-release.deployment-mi-pending-event";
    public const string ExternalACRPush = "acr.push";
    public const string ExternalBuildCompletion = "externalbuild.completed";
    public const string ExternalComment = "externalgit.issuecomment";
    public const string ExternalDockerPush = "dockerhub.push";
    public const string ExternalGitPullRequest = "externalgit.pullrequest";
    public const string ExternalGitPush = "externalgit.push";
    public const string StageStateChangedEvent = "ms.vss-pipelines.stage-state-changed-event";
    public const string RunStateChangedEvent = "ms.vss-pipelines.run-state-changed-event";
    public const string JobStateChangedEvent = "ms.vss-pipelines.job-state-changed-event";
    public const string PipelineChecksApprovalsPending = "ms.vss-pipelinechecks-events.approval-pending";
    public const string PipelineChecksApprovalsCompleted = "ms.vss-pipelinechecks-events.approval-completed";
    public const string RepositoryCreated = "git.repo.created";
    public const string RepositoryForked = "git.repo.forked";
    public const string RepositoryRenamed = "git.repo.renamed";
    public const string RepositoryDeleted = "git.repo.deleted";
    public const string RepositoryUndeleted = "git.repo.undeleted";
    public const string RepositoryStatusChanged = "git.repo.statuschanged";
  }
}
