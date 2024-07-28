// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RepoPermissionsManager
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class RepoPermissionsManager : IRepoPermissionsManager
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly RepoScope m_scope;
    private readonly ISecurityHelper m_securityHelper;

    public RepoPermissionsManager(
      IVssRequestContext requestContext,
      RepoScope scope,
      ISecurityHelper securityHelper = null)
    {
      this.m_requestContext = requestContext;
      this.m_securityHelper = securityHelper ?? SecurityHelper.Instance;
      this.m_scope = scope;
    }

    public void CheckWrite(bool considerAnyBranches) => this.m_securityHelper.CheckWritePermission(this.m_requestContext, this.m_scope, (string) null, considerAnyBranches);

    public void CheckWrite(string refName) => this.m_securityHelper.CheckWritePermission(this.m_requestContext, this.m_scope, refName);

    public bool CanCreateBranch(string refName) => this.m_securityHelper.HasCreateBranchPermission(this.m_requestContext, this.m_scope, refName, isPermissionCritical: true);

    public bool CanCreateTag(string refName) => this.m_securityHelper.HasCreateTagPermission(this.m_requestContext, this.m_scope, refName, true);

    public void CheckEditPolicies(string refName) => this.m_securityHelper.CheckEditPoliciesPermission(this.m_requestContext, this.m_scope, refName);

    public bool HasEditPolicies(string refName) => this.m_securityHelper.HasEditPoliciesPermission(this.m_requestContext, this.m_scope, refName);

    public void CheckPullRequestContribute() => this.m_securityHelper.CheckPullRequestContributePermission(this.m_requestContext, this.m_scope);

    public bool HasPullRequestContribute() => this.m_securityHelper.HasPullRequestContributePermission(this.m_requestContext, this.m_scope);

    public bool HasRead() => this.m_securityHelper.HasReadPermission(this.m_requestContext, this.m_scope);

    public bool HasCreateBranch(string refName = null, bool considerAnyBranches = false) => this.m_securityHelper.HasCreateBranchPermission(this.m_requestContext, this.m_scope, refName, considerAnyBranches);

    public bool HasViewAdvSecAlert() => this.m_securityHelper.HasViewAdvSecAlertsPermission(this.m_requestContext, this.m_scope);

    public bool HasDismissAdvSecAlert() => this.m_securityHelper.HasDismissAdvSecAlertsPermission(this.m_requestContext, this.m_scope);

    public bool HasManageAdvSec() => this.m_securityHelper.HasManageAdvSecPermission(this.m_requestContext, this.m_scope);

    public bool HasViewAdvSecEnablement() => this.m_securityHelper.HasViewAdvSecEnablementPermission(this.m_requestContext, this.m_scope);
  }
}
