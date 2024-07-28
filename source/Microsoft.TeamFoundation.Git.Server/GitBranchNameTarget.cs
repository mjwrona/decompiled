// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitBranchNameTarget
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using Microsoft.TeamFoundation.Policy.Server;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitBranchNameTarget : GitRepositoryTarget, ITeamFoundationPolicyTarget
  {
    public GitBranchNameTarget(
      string teamProjectUri,
      Guid repositoryId,
      string refName,
      bool isDefaultBranch)
      : base(teamProjectUri, repositoryId)
    {
      this.RefName = refName;
      this.IsDefaultBranch = isDefaultBranch;
    }

    public string RefName { get; private set; }

    public bool IsDefaultBranch { get; private set; }

    public bool IsInScope(GitPolicyRefScope refScope) => refScope.IsInScope(this.RepositoryId, this.RefName, this.IsDefaultBranch);

    bool ITeamFoundationPolicyTarget.HasBypassPermissionInTarget(IVssRequestContext requestContext) => SecurityHelper.Instance.HasBypassPolicyWithPushPermission(requestContext, new RepoScope(this.TeamProjectId, this.RepositoryId), this.RefName);

    public override string[] Scopes => GitPolicyScopeResolver.RepositoryPathToScopes(new Guid?(this.RepositoryId), (this.IsDefaultBranch ? 1 : 0) != 0, this.RefName);
  }
}
