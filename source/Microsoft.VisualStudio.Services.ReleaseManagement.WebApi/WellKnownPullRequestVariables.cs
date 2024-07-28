// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.WellKnownPullRequestVariables
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [GenerateAllConstants(null)]
  public static class WellKnownPullRequestVariables
  {
    public static readonly string PullRequestSystemType = "pullRequestSystemType";
    public static readonly string PullRequestId = "pullRequestId";
    public static readonly string PullRequestIterationId = "pullRequestIterationId";
    public static readonly string PullRequestSourceBranchCommitId = "pullRequestSourceBranchCommitId";
    public static readonly string PullRequestTargetBranch = "pullRequestTargetBranch";
    public static readonly string PullRequestSourceBranch = "pullRequestSourceBranch";
    public static readonly string PullRequestMergeCommitId = "pullRequestMergeCommitId";
    public static readonly string PullRequestMergedAt = "pullRequestMergedAt";
    public static readonly string PullRequestStatusPolicyName = "pullRequestStatusPolicyName";
    public const string TfsGitRepositoryId = "pullRequestRepositoryId";
    public const string TfsGitProjectId = "pullRequestProjectId";
    public const string GitHubRepositoryName = "pullRequestRepositoryName";
    public const string GitHubConnection = "pullRequestSystemConnectionId";
  }
}
