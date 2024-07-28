// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.GitRepositoryAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class GitRepositoryAuditConstants
  {
    public const string ActionGitRepositoryCreated = "Git.RepositoryCreated";
    public const string ActionGitRepositoryForked = "Git.RepositoryForked";
    public const string ActionGitRepositoryUndeleted = "Git.RepositoryUndeleted";
    public const string ActionGitRepositoryRenamed = "Git.RepositoryRenamed";
    public const string ActionGitRepositoryDefaultBranchChanged = "Git.RepositoryDefaultBranchChanged";
    public const string ActionGitRepositoryDisabled = "Git.RepositoryDisabled";
    public const string ActionGitRepositoryEnabled = "Git.RepositoryEnabled";
    public const string ActionGitRepositoryDeleted = "Git.RepositoryDeleted";
    public const string ActionGitRepositoryDestroyed = "Git.RepositoryDestroyed";
    public const string ActionGitRefUpdatePoliciesBypassed = "Git.RefUpdatePoliciesBypassed";
    public const string ProjectName = "ProjectName";
    public const string RepoId = "RepoId";
    public const string RepoName = "RepoName";
    public const string ParentProjectId = "ParentProjectId";
    public const string ParentProjectName = "ParentProjectName";
    public const string ParentRepoId = "ParentRepoId";
    public const string ParentRepoName = "ParentRepoName";
    public const string PreviousRepoName = "PreviousRepoName";
    public const string DefaultBranch = "DefaultBranch";
    public const string PreviousDefaultBranch = "PreviousDefaultBranch";
    public const string Name = "Name";
    public const string FriendlyName = "FriendlyName";
    public const string OldObjectId = "OldObjectId";
    public const string NewObjectId = "NewObjectId";
    public const string PullRequestId = "PullRequestId";
    public const string BypassReason = "BypassReason";
    public const string LastMergeSourceCommit = "LastMergeSourceCommit";
    public const string LastMergeTargetCommit = "LastMergeTargetCommit";
  }
}
