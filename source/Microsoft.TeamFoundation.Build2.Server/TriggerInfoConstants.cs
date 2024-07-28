// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TriggerInfoConstants
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class TriggerInfoConstants
  {
    public const string PullRequestSourceBranch = "pr.sourceBranch";
    public const string PullRequestSourceSha = "pr.sourceSha";
    public const string PullRequestId = "pr.id";
    public const string PullRequestTitle = "pr.title";
    public const string PullRequestNumber = "pr.number";
    public const string PullRequestIsFork = "pr.isFork";
    public const string PullRequestProviderId = "pr.providerId";
    public const string PullRequestSenderName = "pr.sender.name";
    public const string PullRequestSenderAvatarUrl = "pr.sender.avatarUrl";
    public const string PullRequestSenderIsExternal = "pr.sender.isExternal";
    public const string PullRequestAutoCancel = "pr.autoCancel";
    public const string PullRequestDraft = "pr.draft";
    public const string PullRequestTriggerRepositoryType = "pr.triggerRepository.Type";
    public const string PullRequestTriggerRepositoryEndpointId = "pr.triggerRepository.endpointId";
    public const string PRTriggerRepository = "pr.triggerRepository";
    public const int MaxPullRequestTitleLength = 300;
    public const string CISourceBranch = "ci.sourceBranch";
    public const string CISourceSha = "ci.sourceSha";
    public const string CIMessage = "ci.message";
    public const string CITriggerRepository = "ci.triggerRepository";
    public const string ScheduleName = "scheduleName";
  }
}
