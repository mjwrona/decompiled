// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.BitbucketServiceHooksTracePoints
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public static class BitbucketServiceHooksTracePoints
  {
    private const int BitbucketServiceHooksPublisherStart = 1064500;
    public const int BitbucketServiceHooksPublisherCreateSubscriptionEnter = 1064510;
    public const int BitbucketServiceHooksPublisherCreateSubscriptionException = 1064520;
    public const int BitbucketServiceHooksPublisherCreateSubscriptionLeave = 1064530;
    public const int BitbucketServiceHooksPublisherDeleteSubscriptionEnter = 1064540;
    public const int BitbucketServiceHooksPublisherDeleteSubscriptionException = 1064550;
    public const int BitbucketServiceHooksPublisherDeleteSubscriptionLeave = 1064560;
    public const int BitbucketServiceHooksPublisherGetInputValuesEnter = 1064570;
    public const int BitbucketServiceHooksPublisherGetInputValuesException = 1064580;
    public const int BitbucketServiceHooksPublisherGetInputValuesLeave = 1064590;
    public const int BitbucketServiceHooksPublisherGetRepoContentEnter = 1064600;
    public const int BitbucketServiceHooksPublisherGetRepoContentException = 1064610;
    public const int BitbucketServiceHooksPublisherGetRepoContentLeave = 1064620;
    public const int BitbucketServiceHooksPublisherGetRepoBranchesEnter = 1064630;
    public const int BitbucketServiceHooksPublisherGetRepoBranchesException = 1064640;
    public const int BitbucketServiceHooksPublisherGetRepoBranchesLeave = 1064650;
    public const int BitbucketServiceHooksGitExternalPushSubscriberEnter = 1064660;
    public const int BitbucketServiceHooksGitExternalPushSubscriberException = 1064670;
    public const int BitbucketServiceHooksGitExternalPushSubscriberLeave = 1064680;
    public const int BitbucketServiceHooksProcessBuildNotificationEnter = 1064720;
    public const int BitbucketServiceHooksProcessBuildNotificationException = 1064730;
    public const int BitbucketServiceHooksProcessBuildNotificationLeave = 1064740;
    public const int BitbucketServiceHooksProcessBuildNotificationTrace = 1064750;
    public const int BitbucketServiceHooksUnauthorizedException = 1064760;
    public const int BitbucketServiceHooksPublisherGetRepoFileContentEnter = 1064770;
    public const int BitbucketServiceHooksPublisherGetRepoFileContentException = 1064780;
    public const int BitbucketServiceHooksPublisherGetRepoFileContentLeave = 1064790;
    public const int BitbucketPullRequestPollingJobEnter = 1064800;
    public const int BitbucketPullRequestPollingJobException = 1064810;
    public const int BitbucketPullRequestPollingJobLeave = 1064820;
    public const int BitbucketPullRequestServiceHooksEventRegistrationQueueingJob = 1064830;
    public const int BitbucketPullRequestServiceHooksEventRegistrationNotQueueingJob = 1064831;
    public const int BitbucketServiceHooksPushEventToExternalGitPush = 1064840;
    public const int BitbucketServiceHooksPullRequestEventRegistrationException = 1064850;
    public const int BitbucketBuildDefinitionDecisionPointListenerQueueingUpdateJob = 1064851;
  }
}
