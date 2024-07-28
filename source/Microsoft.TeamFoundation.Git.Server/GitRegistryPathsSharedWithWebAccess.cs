// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRegistryPathsSharedWithWebAccess
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Git.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class GitRegistryPathsSharedWithWebAccess
  {
    public const string WitMentionsEnabledPathFormat = "/VersionControl/Repositories/{0}/WitMentionsEnabled";
    public const string WitResolutionMentionsEnabledPathFormat = "/VersionControl/Repositories/{0}/WitResolutionMentionsEnabled";
    public const string WitTransitionsStickyPathFormat = "/VersionControl/Repositories/{0}/WitTransitionsSticky";
    public const string WebSettingPrefix = "/WebAccess";
    public const string DisableGravatarRegistryKey = "/WebAccess/IdentityImage/DisableGravatar";
    public const string MaxRebaseCommitCount = "/Service/Git/Settings/MaxRebaseCommitCount";
    public const int MaxRebaseCommitCountDefaultValueHosted = 100;
    public const int MaxRebaseCommitCountDefaultValueOnPrem = 500;
    public const string DefaultBranchName = "/Service/Git/Settings/DefaultBranchName";
    public const string DisableTfvcRepositories = "/Service/Git/Settings/DisableTfvcRepositories";
    public const string RepoCreatedBranchesManagePermissionsEnabledPathFormat = "/VersionControl/Repositories/{0}/CreatedBranchesManagePermissionsEnabled";
    public const string NewReposCreatedBranchesManagePermissionsEnabledPathFormat = "VersionControl/Projects/{0}/CreatedBranchesManagePermissionsEnabled";
    public const string PullRequestDetailsBuildPolicyMaxCount = "/Service/Git/Settings/PullRequestDetails/BuildPolicyMaximumCount";
    public const string PullRequestDetailsCacheExpirationSeconds = "/Service/Git/Settings/PullRequestDetails/PullRequestDetailsCacheExpirationSeconds";
    public const string PullRequestAsDraftByDefaultPathFormat = "VersionControl/Repositories/PullRequestAsDraftByDefault";
    public const string MaxNumberOfCommitsWhichItemsCanBeLinkedToPR = "/Service/Git/Settings/MaxNumberOfCommitsWhichItemsCanBeLinkedToPR";
    public const string DisableOldTfvcCheckinPolicies = "VersionControl/Repositories/DisableOldTfvcCheckinPolicies";
  }
}
