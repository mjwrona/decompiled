// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPullRequestTarget
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class GitPullRequestTarget : GitBranchNameTarget, ITeamFoundationPolicyTarget
  {
    public GitPullRequestTarget(
      string teamProjectUri,
      TfsGitPullRequest pullRequest,
      bool isDefaultBranch)
      : base(teamProjectUri, pullRequest.RepositoryId, pullRequest.TargetBranchName, isDefaultBranch)
    {
      ArgumentUtility.CheckForNull<string>(teamProjectUri, nameof (teamProjectUri));
      ArgumentUtility.CheckForNull<TfsGitPullRequest>(pullRequest, nameof (pullRequest));
      this.PullRequest = pullRequest;
    }

    bool ITeamFoundationPolicyTarget.ShouldDynamicEvaluatePolicies(IVssRequestContext requestContext) => this.PullRequest != null && this.PullRequest.Status == PullRequestStatus.Active;

    bool ITeamFoundationPolicyTarget.HasBypassPermissionInTarget(IVssRequestContext requestContext) => SecurityHelper.Instance.HasBypassPolicyWithPullRequestPermission(requestContext, new RepoScope(this.TeamProjectId, this.RepositoryId), this.RefName);

    public TfsGitPullRequest PullRequest { get; private set; }
  }
}
