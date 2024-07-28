// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ISecurityHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public interface ISecurityHelper
  {
    void CheckCreateRepositoryPermission(IVssRequestContext requestContext, Guid projectId);

    void CheckCreateTeamProjectPermission(IVssRequestContext requestContext);

    void CheckDestroyAllPermission(IVssRequestContext requestContext, string uri);

    bool HasReadPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      bool alwaysAllowAdministrators = false);

    bool HasReadProjectPermission(IVssRequestContext rc, string teamProjectUri);

    void CheckReadProjectPermission(IVssRequestContext rc, string teamProjectUri);

    void CheckReadPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string repositoryNameOrId,
      bool alwaysAllowAdministrators = false);

    bool HasCreateBranchPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool considerAnyBranches = false,
      bool isPermissionCritical = false);

    bool HasCreateTagPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool isPermissionCritical = false);

    bool HasManageNotePermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName);

    bool HasBypassPolicyWithPushPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    bool HasBypassPolicyWithPullRequestPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    void CheckViewAdvSecAlertsPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    bool HasViewAdvSecAlertsPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    void CheckDismissAdvSecAlertsPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    bool HasDismissAdvSecAlertsPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    void CheckManageAdvSecPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    bool HasManageAdvSecPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    bool HasViewAdvSecEnablementPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    bool RemoveACLs(IVssRequestContext requestContext, RepoScope repoScope, string refName);

    bool SetPermissions(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      IEnumerable<IAccessControlEntry> permissions);

    bool SetDefaultRepositoryPermissions(IVssRequestContext requestContext, RepoScope repoScope);

    bool HasWritePermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool considerAnyBranches = false);

    void CheckWritePermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool considerAnyBranches = false);

    bool HasForcePushPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName);

    bool IsForcePushRequired(
      IVssRequestContext requestContext,
      ITfsGitRepository objectDb,
      TfsGitRefUpdateRequest refUpdate);

    bool HasRemoveOthersLocksPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    void CheckEditPoliciesPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    bool HasEditPoliciesPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null);

    void CheckRenameRepositoryPermission(IVssRequestContext requestContext, RepoScope repoScope);

    void CheckDeleteRepositoryPermission(IVssRequestContext requestContext, RepoScope repoScope);

    void CheckPullRequestContributePermission(IVssRequestContext rc, RepoScope repoScope);

    bool HasPullRequestContributePermission(IVssRequestContext rc, RepoScope repoScope);

    bool TryGetDefaultAceForRefCreator(
      IVssRequestContext requestContext,
      string token,
      out QueriedAccessControlEntry defaultAce,
      out IdentityDescriptor refCreator);

    int GetRepositoryDefaultAllowBranchPermissions(
      IVssRequestContext requestContext,
      Guid? repositoryId);

    void EnsureBuildServiceHasAdvSecPerms(IVssRequestContext requestContext, Guid projectId);
  }
}
