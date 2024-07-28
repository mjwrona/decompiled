// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRepositoryTarget
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using Microsoft.TeamFoundation.Policy.Server;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitRepositoryTarget : ITeamFoundationPolicyTarget
  {
    private Guid m_teamProjectId;

    public GitRepositoryTarget(string teamProjectUri, Guid repositoryId)
    {
      CommonStructureUtils.TryParseProjectUri(teamProjectUri, out this.m_teamProjectId);
      this.TeamProjectUri = teamProjectUri;
      this.RepositoryId = repositoryId;
    }

    public string TeamProjectUri { get; private set; }

    public Guid RepositoryId { get; private set; }

    public Guid TeamProjectId => this.m_teamProjectId;

    public bool IsInScope(GitPolicyRepositoryScope scope) => scope.IsInScope(this.RepositoryId);

    public virtual string[] Scopes => GitPolicyScopeResolver.RepositoryPathToScopes(new Guid?(this.RepositoryId), false);

    bool ITeamFoundationPolicyTarget.HasBypassPermissionInTarget(IVssRequestContext requestContext) => SecurityHelper.Instance.HasBypassPolicyWithPushPermission(requestContext, new RepoScope(this.m_teamProjectId, this.RepositoryId));

    bool ITeamFoundationPolicyTarget.HasReadPermissionInTarget(IVssRequestContext requestContext) => SecurityHelper.Instance.HasReadPermission(requestContext, new RepoScope(this.m_teamProjectId, this.RepositoryId));

    bool ITeamFoundationPolicyTarget.ShouldDynamicEvaluatePolicies(IVssRequestContext requestContext) => true;
  }
}
