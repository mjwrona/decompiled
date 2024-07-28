// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.GitHubServiceHooksTracePoints
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public static class GitHubServiceHooksTracePoints
  {
    private const int GitHubServiceHooksPublisherStart = 1064000;
    public const int GitHubServiceHooksPublisherCreateSubscriptionEnter = 1064010;
    public const int GitHubServiceHooksPublisherCreateSubscriptionException = 1064020;
    public const int GitHubServiceHooksPublisherCreateSubscriptionLeave = 1064030;
    public const int GitHubServiceHooksPublisherDeleteSubscriptionEnter = 1064040;
    public const int GitHubServiceHooksPublisherDeleteSubscriptionException = 1064050;
    public const int GitHubServiceHooksPublisherDeleteSubscriptionLeave = 1064060;
    public const int GitHubServiceHooksPublisherGetInputValuesEnter = 1064070;
    public const int GitHubServiceHooksPublisherGetInputValuesException = 1064080;
    public const int GitHubServiceHooksPublisherGetInputValuesLeave = 1064090;
    public const int GitHubServiceHooksPublisherGetRepoContentEnter = 1064100;
    public const int GitHubServiceHooksPublisherGetRepoContentException = 1064110;
    public const int GitHubServiceHooksPublisherGetRepoContentLeave = 1064120;
    public const int GitHubServiceHooksPublisherGetRepoBranchesEnter = 1064130;
    public const int GitHubServiceHooksPublisherGetRepoBranchesException = 1064140;
    public const int GitHubServiceHooksPublisherGetRepoBranchesLeave = 1064150;
    public const int GitHubServiceHooksPublisherGetRepoWebhooksEnter = 1064155;
    public const int GitHubServiceHooksPublisherGetRepoWebhooksException = 1064156;
    public const int GitHubServiceHooksPublisherGetRepoWebhooksLeave = 1064157;
    public const int GitHubServiceHooksGitExternalPushSubscriberEnter = 1064160;
    public const int GitHubServiceHooksGitExternalPushSubscriberException = 1064170;
    public const int GitHubServiceHooksGitExternalPushSubscriberLeave = 1064180;
    public const int GitHubServiceHooksProcessBuildNotificationEnter = 1064220;
    public const int GitHubServiceHooksProcessBuildNotificationException = 1064230;
    public const int GitHubServiceHooksProcessBuildNotificationLeave = 1064240;
    public const int GitHubServiceHooksProcessBuildNotificationTrace = 1064250;
    public const int GitHubServiceHooksUnauthorizedException = 1064260;
    public const int GitHubServiceHooksPublisherGetRepoFileContentEnter = 1064270;
    public const int GitHubServiceHooksPublisherGetRepoFileContentException = 1064280;
    public const int GitHubServiceHooksPublisherGetRepoFileContentLeave = 1064290;
    public const int GitHubPullRequestPollingJobEnter = 1064300;
    public const int GitHubPullRequestPollingJobException = 1064310;
    public const int GitHubPullRequestPollingJob = 1064311;
    public const int GitHubPullRequestPollingJobLeave = 1064320;
    public const int GitHubPullRequestServiceHooksEventRegistrationQueueingJob = 1064330;
    public const int GitHubPullRequestServiceHooksEventRegistrationNotQueueingJob = 1064331;
    public const int GitHubPullRequestServiceHooksEventRegistrationMissingChannelId = 1064332;
    public const int GitHubPullRequestServiceHooksEventRegistrationNotBaseRefChangeEdit = 1064333;
    public const int GitHubPullRequestServiceHooksEventRegistrationMissingAction = 1064334;
    public const int GitHubPullRequestServiceHooksEventRegistrationMissingPayload = 1064335;
    public const int GitHubServiceHooksPullRequestEventRegistrationGetPayloadEnter = 1064336;
    public const int GitHubServiceHooksPullRequestEventRegistrationGetPayloadLeave = 1064337;
    public const int GitHubServiceHooksPublisherGetRepoFileContentHttpError = 1064340;
    public const int GitHubServiceHooksPublisherGetRepoBranchesHttpError = 1064341;
    public const int GitHubServiceHooksPublisherGetUserGitRepoHttpError = 1064342;
    public const int GitHubServiceHooksPublisherGetRepoWebhooksHttpError = 1064343;
    public const int GitHubServiceHooksPublisherGetUserGitReposHttpError = 1064344;
    public const int GitHubServiceHooksPublisherGetRepoBlobsHttpError = 1064345;
    public const int GitHubIssueCommentServiceHooksEventRegistrationQueueingJob = 1064350;
    public const int GitHubIssueCommentServiceHooksEventRegistrationNotQueueingJob = 1064351;
    public const int GitHubCommentJobEnter = 1064360;
    public const int GitHubCommentJobException = 1064361;
    public const int GitHubCommentJobLeave = 1064362;
    public const int GitHubCommentJobExecutionMaxReached = 1064364;
    public const int GitHubCommentJobNoCommitFound = 1064365;
    public const int GitHubWebhookUpdateJobEnter = 1064370;
    public const int GitHubWebhookUpdateJobException = 1064371;
    public const int GitHubWebhookUpdateJobLeave = 1064372;
    public const int GitHubWebhookUpdateJobMissingSubscription = 1064373;
    public const int GitHubWebhookUpdateJobCreateHook = 1064374;
    public const int GitHubWebhookUpdateJobDeleteHook = 1064375;
    public const int GitHubWebhookUpdateJobQueryHooks = 1064376;
    public const int GitHubPushServiceHooksEventRegistrationQueueingJob = 1064380;
    public const int GitHubPushServiceHooksEventRegistrationNotQueueingJob = 1064381;
    public const int GitHubHelperGetGitHubEventNameUnknownEvent = 1064390;
    public const int GitHubHelperGetInternalEventNameUnknownEvent = 1064391;
    public const int GitHubHelperDoesUserHaveRepoWritePermissions = 1064392;
    public const int GitHubCreateCommentWebhookJobEnter = 1064400;
    public const int GitHubCreateCommentWebhookJobException = 1064401;
    public const int GitHubCreateCommentWebhookJobLeave = 1064402;
    public const int GitHubCreateCommentWebhookJobCreateHook = 1064405;
    public const int GitHubAuthorWritePermissionJobEnter = 1064500;
    public const int GitHubAuthorWritePermissionJobException = 1064501;
    public const int GitHubAuthorWritePermissionJobLeave = 1064502;
    public const int GitHubBuildDefinitionDecisionPointListenerQueueingUpdateJob = 1064503;
  }
}
