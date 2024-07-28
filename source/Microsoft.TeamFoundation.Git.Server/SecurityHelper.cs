// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.SecurityHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class SecurityHelper : ISecurityHelper
  {
    private const string c_traceLayerSecurity = "Security";
    private static ISecurityHelper s_instance = (ISecurityHelper) new SecurityHelper();
    private static readonly string m_logSecurityEvaluationCount = "Git-LogSecurityEvaluationCount";
    private const int c_defaultAllowPermissionsForBranches = 12300;

    public static ISecurityHelper Instance
    {
      get => SecurityHelper.s_instance;
      set => SecurityHelper.s_instance = value;
    }

    public virtual void CheckCreateRepositoryPermission(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (!this.HasRepositoryPermissions(requestContext, new RepoScope(projectId, Guid.Empty), (string) null, GitRepositoryPermissions.CreateRepository, false, false, true))
        throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.CreateRepository, GitPermissionScope.Project);
    }

    public virtual void CheckCreateTeamProjectPermission(IVssRequestContext requestContext)
    {
      if (!requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId).HasPermissionExpect(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceToken, TeamProjectCollectionPermissions.CreateProjects, true, false))
        throw new GitNeedsTeamProjectCreatePermissionException(requestContext.AuthenticatedUserName);
    }

    public virtual void CheckDestroyAllPermission(IVssRequestContext requestContext, string uri)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      if (!securityNamespace.HasPermissionExpect(requestContext, securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, uri), TeamProjectPermissions.Delete, true, false))
        throw new GitNeedsTeamProjectDeletePermissionException(requestContext.AuthenticatedUserName);
    }

    public virtual bool HasReadProjectPermission(IVssRequestContext rc, string teamProjectUri) => rc.GetService<IProjectService>().HasProjectPermission(rc, teamProjectUri, TeamProjectPermissions.GenericRead);

    public virtual void CheckReadProjectPermission(IVssRequestContext rc, string teamProjectUri) => rc.GetService<IProjectService>().CheckProjectPermission(rc, teamProjectUri, TeamProjectPermissions.GenericRead);

    public virtual bool HasReadPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      bool alwaysAllowAdministrators = false)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, (string) null, GitRepositoryPermissions.GenericRead, alwaysAllowAdministrators, false, false);
    }

    protected virtual bool HasReadPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      bool isPermissionCritical,
      bool alwaysAllowAdministrators = false)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, (string) null, GitRepositoryPermissions.GenericRead, alwaysAllowAdministrators, false, isPermissionCritical);
    }

    public virtual void CheckReadPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string repositoryNameOrId,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasReadPermission(requestContext, repoScope, true, alwaysAllowAdministrators))
        return;
      if (!alwaysAllowAdministrators && this.HasReadPermission(requestContext, repoScope, true, true))
        throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.GenericRead, GitPermissionScope.Repository);
      throw new GitRepositoryNotFoundException(repositoryNameOrId);
    }

    public virtual bool HasCreateBranchPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool considerAnyBranches = false,
      bool isPermissionCritical = false)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.CreateBranch, false, considerAnyBranches, isPermissionCritical);
    }

    public virtual bool HasCreateTagPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool isPermissionCritical = false)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.CreateTag, false, false, isPermissionCritical);
    }

    public virtual bool HasManageNotePermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.ManageNote, false, false, false);
    }

    protected virtual bool HasManageNotePermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool isPermissionCritical)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.ManageNote, false, false, isPermissionCritical);
    }

    public virtual bool HasRemoveOthersLocksPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.RemoveOthersLocks, false, false, false);
    }

    protected virtual bool HasRemoveOthersLocksPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      bool isPermissionCritical,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.RemoveOthersLocks, false, false, isPermissionCritical);
    }

    public virtual void CheckEditPoliciesPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      if (!this.HasEditPoliciesPermission(requestContext, repoScope, true, refName))
        throw new PolicyNeedsPermissionException(Resources.Format("GitNeedsPermission", (object) requestContext.AuthenticatedUserName, (object) Enum.GetName(typeof (GitRepositoryPermissions), (object) GitRepositoryPermissions.EditPolicies), (object) this.GetPermissionScope(repoScope, refName)));
    }

    public virtual bool HasEditPoliciesPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.EditPolicies, false, false, false);
    }

    protected virtual bool HasEditPoliciesPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      bool isPermissionCritical,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.EditPolicies, false, false, isPermissionCritical);
    }

    public virtual void CheckRenameRepositoryPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope)
    {
      if (!this.HasRepositoryPermissions(requestContext, repoScope, (string) null, GitRepositoryPermissions.RenameRepository, false, false, true))
        throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.RenameRepository, this.GetPermissionScope(repoScope, (string) null));
    }

    public virtual void CheckDeleteRepositoryPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope)
    {
      if (!this.HasRepositoryPermissions(requestContext, repoScope, (string) null, GitRepositoryPermissions.DeleteRepository, false, false, true))
        throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.DeleteRepository, this.GetPermissionScope(repoScope, (string) null));
    }

    public void CheckPullRequestContributePermission(IVssRequestContext rc, RepoScope repoScope)
    {
      if (!this.HasPullRequestContributePermission(rc, repoScope, true))
        throw new GitNeedsPermissionException(rc.AuthenticatedUserName, GitRepositoryPermissions.PullRequestContribute, this.GetPermissionScope(repoScope, (string) null));
    }

    public virtual bool HasPullRequestContributePermission(
      IVssRequestContext rc,
      RepoScope repoScope)
    {
      return this.HasRepositoryPermissions(rc, repoScope, (string) null, GitRepositoryPermissions.PullRequestContribute, false, false, false);
    }

    protected virtual bool HasPullRequestContributePermission(
      IVssRequestContext rc,
      RepoScope repoScope,
      bool isPermissionCritical)
    {
      return this.HasRepositoryPermissions(rc, repoScope, (string) null, GitRepositoryPermissions.PullRequestContribute, false, false, isPermissionCritical);
    }

    public virtual bool HasBypassPolicyWithPushPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.PolicyExempt, false, false, false);
    }

    protected virtual bool HasPolicyExemptPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      bool isPermissionCritical,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.PolicyExempt, false, false, isPermissionCritical);
    }

    public virtual bool HasBypassPolicyWithPullRequestPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.PullRequestBypassPolicy, false, false, false);
    }

    public virtual void CheckViewAdvSecAlertsPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      if (!this.HasViewAdvSecAlertsPermission(requestContext, repoScope, refName))
        throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.ViewAdvSecAlerts, this.GetPermissionScope(repoScope, refName));
    }

    public virtual bool HasViewAdvSecAlertsPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.ViewAdvSecAlerts, false, false, false);
    }

    public virtual void CheckDismissAdvSecAlertsPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      if (!this.HasDismissAdvSecAlertsPermission(requestContext, repoScope, refName))
        throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.DismissAdvSecAlerts, this.GetPermissionScope(repoScope, refName));
    }

    public virtual bool HasDismissAdvSecAlertsPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.DismissAdvSecAlerts, false, false, false);
    }

    public virtual void CheckManageAdvSecPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      if (!this.HasManageAdvSecPermission(requestContext, repoScope, refName))
        throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.ManageAdvSecScanning, this.GetPermissionScope(repoScope, refName));
    }

    public virtual bool HasManageAdvSecPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.ManageAdvSecScanning, true, false, false);
    }

    public virtual bool HasViewAdvSecEnablementPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName = null)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.ViewAdvSecAlerts, true, false, false);
    }

    public virtual bool RemoveACLs(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName)
    {
      requestContext.Trace(1013018, TraceLevel.Verbose, GitServerUtils.TraceArea, "Security", "Entering {0}: {1}", (object) nameof (RemoveACLs), (object) repoScope);
      string securable = SecurityHelper.CalculateSecurable(repoScope, refName);
      bool flag = SecurityHelper.GetRepositorySecurity(requestContext).RemoveAccessControlLists(requestContext, (IEnumerable<string>) new List<string>()
      {
        securable
      }, true);
      requestContext.Trace(1013019, TraceLevel.Verbose, GitServerUtils.TraceArea, "Security", "Leaving {0}: {1}", (object) nameof (RemoveACLs), flag ? (object) "Passed" : (object) "Failed");
      return flag;
    }

    public virtual bool SetPermissions(
      IVssRequestContext requestContext,
      RepoScope scopeToSet,
      string refName,
      IEnumerable<IAccessControlEntry> permissions)
    {
      requestContext.Trace(1013020, TraceLevel.Verbose, GitServerUtils.TraceArea, "Security", "Entering {0}: {1}", (object) nameof (SetPermissions), (object) scopeToSet);
      string securable = SecurityHelper.CalculateSecurable(scopeToSet, refName);
      List<IAccessControlEntry> list = permissions.Select<IAccessControlEntry, IAccessControlEntry>((Func<IAccessControlEntry, IAccessControlEntry>) (s => SecurityHelper.ResolvePermissions(requestContext, s))).ToList<IAccessControlEntry>();
      IEnumerable<IAccessControlEntry> source = SecurityHelper.GetRepositorySecurity(requestContext).SetAccessControlEntries(requestContext, securable, (IEnumerable<IAccessControlEntry>) list, false);
      bool flag = source != null && source.Any<IAccessControlEntry>();
      requestContext.Trace(1013021, TraceLevel.Verbose, GitServerUtils.TraceArea, "Security", "Leaving {0}: {1}", (object) nameof (SetPermissions), flag ? (object) "Passed" : (object) "Failed");
      return flag;
    }

    public virtual bool SetDefaultRepositoryPermissions(
      IVssRequestContext requestContext,
      RepoScope repoScope)
    {
      int allow = 32382;
      IEnumerable<IAccessControlEntry> permissions = (IEnumerable<IAccessControlEntry>) new AccessControlEntry[1]
      {
        new AccessControlEntry(requestContext.UserContext, allow, 0)
      };
      return this.SetPermissions(requestContext, repoScope, (string) null, permissions);
    }

    public virtual bool HasWritePermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool considerAnyBranches = false)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.GenericContribute, false, considerAnyBranches, false);
    }

    protected virtual bool HasWritePermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool isPermissionCritical,
      bool considerAnyBranches = false)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.GenericContribute, false, considerAnyBranches, isPermissionCritical);
    }

    public virtual void CheckWritePermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool considerAnyBranches = false)
    {
      if (!this.HasWritePermission(requestContext, repoScope, refName, true, considerAnyBranches))
        throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.GenericContribute, this.GetPermissionScope(repoScope, refName));
    }

    public virtual bool HasForcePushPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.ForcePush, false, false, false);
    }

    protected virtual bool HasForcePushPermission(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      string refName,
      bool isPermissionCritical)
    {
      return this.HasRepositoryPermissions(requestContext, repoScope, refName, GitRepositoryPermissions.ForcePush, false, false, isPermissionCritical);
    }

    public virtual bool IsForcePushRequired(
      IVssRequestContext requestContext,
      ITfsGitRepository objectDb,
      TfsGitRefUpdateRequest refUpdate)
    {
      if (refUpdate.NewObjectId.IsEmpty)
        return true;
      if (refUpdate.OldObjectId.IsEmpty)
        return false;
      if (refUpdate.Name.StartsWith("refs/tags/", StringComparison.Ordinal))
        return true;
      TfsGitCommit commit1 = objectDb.LookupObject(refUpdate.OldObjectId).TryResolveToCommit();
      TfsGitCommit commit2 = objectDb.LookupObject(refUpdate.NewObjectId).TryResolveToCommit();
      if (commit1 == null || commit2 == null)
        return true;
      return !GitServerUtils.IsConnected(requestContext, objectDb, new CommitIdSet()
      {
        {
          commit1.ObjectId,
          commit1.GetCommitter().Time
        }
      }, commit2.ObjectId);
    }

    public virtual bool TryGetDefaultAceForRefCreator(
      IVssRequestContext rc,
      string token,
      out QueriedAccessControlEntry defaultAce,
      out IdentityDescriptor refCreator)
    {
      refCreator = (IdentityDescriptor) null;
      defaultAce = new QueriedAccessControlEntry();
      try
      {
        Guid? repoIdFromToken = GitPermissionsUtil.GetRepoIdFromToken(token);
        if (!rc.Items.TryGetValue<IdentityDescriptor>(token, out refCreator))
        {
          Guid? projectIdFromToken = GitPermissionsUtil.GetProjectIdFromToken(token);
          string refNameFromToken = GitPermissionsUtil.GetRefNameFromToken(token);
          if (!repoIdFromToken.HasValue || !projectIdFromToken.HasValue || refNameFromToken == null)
            return false;
          Guid guid = rc.GetService<IInternalGitRefService>().ReadRefCreatorWithDefaultAce(rc, new RepoKey(projectIdFromToken.Value, repoIdFromToken.Value), refNameFromToken);
          if (guid != Guid.Empty)
          {
            IVssRequestContext vssRequestContext = rc.Elevate();
            TeamFoundationIdentity foundationIdentity = ((IEnumerable<TeamFoundationIdentity>) vssRequestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(vssRequestContext, new Guid[1]
            {
              guid
            })).First<TeamFoundationIdentity>();
            if (foundationIdentity != null)
              refCreator = foundationIdentity.Descriptor;
            else
              rc.TraceAlways(1013876, TraceLevel.Error, GitServerUtils.TraceArea, "Security", "Identity with tfId={0} is missing.", (object) guid);
          }
          rc.Items.TryAdd<string, object>(token, (object) refCreator);
        }
        if (refCreator != (IdentityDescriptor) null)
        {
          int branchPermissions = this.GetRepositoryDefaultAllowBranchPermissions(rc, repoIdFromToken);
          defaultAce = new QueriedAccessControlEntry(refCreator, branchPermissions, 0, 0, 0, branchPermissions, 0);
          return true;
        }
      }
      catch (Exception ex)
      {
        rc.TraceException(1013874, TraceLevel.Error, GitServerUtils.TraceArea, "Security", ex);
      }
      return false;
    }

    public bool ValidateRepoClaim(IVssRequestContext requestContext, IPrincipal user, Guid repoId)
    {
      if (requestContext.IsSystemContext || !(user is ClaimsPrincipal claimsPrincipal) || !claimsPrincipal.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type.Equals("repoIds"))))
        return true;
      Claim claim = claimsPrincipal.Claims.Where<Claim>((Func<Claim, bool>) (x => x.Type.Equals("repoIds"))).First<Claim>();
      if (string.IsNullOrEmpty(claim.Value))
        return false;
      return ((IEnumerable<string>) claim.Value.Split(',')).Select<string, Guid>((Func<string, Guid>) (x => new Guid(x))).Any<Guid>((Func<Guid, bool>) (x => x == repoId));
    }

    protected virtual bool HasRepositoryPermissions(
      IVssRequestContext requestContext,
      RepoScope scopeToCheck,
      string refName,
      GitRepositoryPermissions permissionsToCheck,
      bool alwaysAllowAdministrators,
      bool considerAnyBranches,
      bool isPermissionCritical)
    {
      requestContext.Trace(1013016, TraceLevel.Verbose, GitServerUtils.TraceArea, "Security", "Entering {0}: {1} {2}", (object) nameof (HasRepositoryPermissions), (object) scopeToCheck, (object) permissionsToCheck);
      bool flag = true;
      if (!this.ValidateRepoClaim(requestContext, HttpContext.Current?.User, scopeToCheck.RepoId))
      {
        requestContext.Trace(1013024, TraceLevel.Error, GitServerUtils.TraceArea, "Security", "Leaving {0}: {1}", (object) nameof (HasRepositoryPermissions), (object) "Permission Denied, ValidateRepoClaim Failed");
        return false;
      }
      string securable = SecurityHelper.CalculateSecurable(scopeToCheck, refName);
      GitRepositoryPermissions repositoryPermissions = GitRepositoryPermissions.GenericRead | GitRepositoryPermissions.ViewAdvSecAlerts | GitRepositoryPermissions.ManageAdvSecScanning;
      if ((permissionsToCheck & repositoryPermissions) != GitRepositoryPermissions.None)
      {
        int num;
        requestContext.Items.TryGetValue<int>(SecurityHelper.m_logSecurityEvaluationCount, out num);
        if (num == 0)
        {
          flag = SecurityHelper.GetRepositorySecurity(requestContext).HasPermissionExpect(requestContext, securable, (int) permissionsToCheck, true, true, out EvaluationPrincipal _, alwaysAllowAdministrators);
          if (!flag && num == 0)
            requestContext.Items.TryAdd<string, object>(SecurityHelper.m_logSecurityEvaluationCount, (object) 1);
        }
        else
          flag = !isPermissionCritical ? SecurityHelper.GetRepositorySecurity(requestContext).HasPermission(requestContext, securable, (int) permissionsToCheck, alwaysAllowAdministrators) : SecurityHelper.GetRepositorySecurity(requestContext).HasPermissionExpect(requestContext, securable, (int) permissionsToCheck, true, alwaysAllowAdministrators);
      }
      if (flag && (permissionsToCheck & ~repositoryPermissions) != GitRepositoryPermissions.None)
        flag = !isPermissionCritical ? SecurityHelper.GetRepositorySecurity(requestContext).HasPermission(requestContext, securable, (int) permissionsToCheck, false) : SecurityHelper.GetRepositorySecurity(requestContext).HasPermissionExpect(requestContext, securable, (int) permissionsToCheck, true, alwaysAllowAdministrators);
      if (!flag & considerAnyBranches)
        flag = SecurityHelper.GetRepositorySecurity(requestContext).HasPermissionForAnyChildren(requestContext, securable, (int) permissionsToCheck, alwaysAllowAdministrators: false);
      requestContext.Trace(1013017, TraceLevel.Verbose, GitServerUtils.TraceArea, "Security", "Leaving {0}: {1}", (object) nameof (HasRepositoryPermissions), flag ? (object) "Permission Granted" : (object) "Permission Denied");
      return flag;
    }

    private GitPermissionScope GetPermissionScope(RepoScope repoScope, string refName)
    {
      if (repoScope.RepoId == Guid.Empty)
        return GitPermissionScope.Project;
      return string.IsNullOrEmpty(refName) ? GitPermissionScope.Repository : GitPermissionScope.Branch;
    }

    protected static IVssSecurityNamespace GetRepositorySecurity(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GitConstants.GitSecurityNamespaceId);

    protected static IAccessControlEntry ResolvePermissions(
      IVssRequestContext requestContext,
      IAccessControlEntry permission)
    {
      if (!"Microsoft.TeamFoundation.UnauthenticatedIdentity".Equals(permission.Descriptor.IdentityType))
        return permission;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return (IAccessControlEntry) new AccessControlEntry(vssRequestContext.GetService<LocalSecurityService>().EnsureIdentityIsKnown(vssRequestContext, (vssRequestContext.GetService<ITeamFoundationIdentityService>().ReadIdentity(vssRequestContext, permission.Descriptor.Identifier) ?? throw new IdentityNotFoundException(permission.Descriptor.Identifier)).Descriptor).Descriptor, permission.Allow, permission.Deny);
    }

    private static string CalculateSecurable(RepoScope scope, string refName) => GitUtils.CalculateSecurable(scope.ProjectId, scope.RepoId, refName);

    public int GetRepositoryDefaultAllowBranchPermissions(
      IVssRequestContext requestContext,
      Guid? repositoryId)
    {
      int branchPermissions = 12300;
      if (!repositoryId.HasValue)
        return branchPermissions;
      string query = "/WebAccess" + string.Format("/VersionControl/Repositories/{0}/CreatedBranchesManagePermissionsEnabled", (object) repositoryId);
      if (!requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) query, true))
        branchPermissions &= -8193;
      return branchPermissions;
    }

    public void EnsureBuildServiceHasAdvSecPerms(IVssRequestContext requestContext, Guid projectId)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GitConstants.GitSecurityNamespaceId);
      string securable1 = GitUtils.CalculateSecurable(Guid.Empty, Guid.Empty, (string) null);
      IdentityDescriptor foundationDescriptor = Microsoft.TeamFoundation.Framework.Server.IdentityHelper.CreateTeamFoundationDescriptor(BuildGroupWellKnownSecurityIds.BuildServicesGroup);
      SecurityHelper.EnsureIdentityHasAdvSecPerms(requestContext, securityNamespace, foundationDescriptor, securable1);
      string securable2 = GitUtils.CalculateSecurable(projectId, Guid.Empty, (string) null);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity1 = Microsoft.TeamFoundation.Framework.Server.IdentityHelper.GetFrameworkIdentity(requestContext1, FrameworkIdentityType.ServiceIdentity, "Build", projectId.ToString("D"));
      SecurityHelper.EnsureIdentityHasAdvSecPerms(requestContext1, securityNamespace, frameworkIdentity1.Descriptor, securable2);
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity2 = Microsoft.TeamFoundation.Framework.Server.IdentityHelper.GetFrameworkIdentity(requestContext1, FrameworkIdentityType.ServiceIdentity, "Build", requestContext1.ServiceHost.InstanceId.ToString("D"));
      SecurityHelper.EnsureIdentityHasAdvSecPerms(requestContext1, securityNamespace, frameworkIdentity2.Descriptor, securable1);
    }

    private static void EnsureIdentityHasAdvSecPerms(
      IVssRequestContext requestContext,
      IVssSecurityNamespace gitNamespace,
      IdentityDescriptor descriptor,
      string token)
    {
      IAccessControlEntry accessControlEntry1 = gitNamespace.QueryAccessControlList(requestContext, token, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, false)?.QueryAccessControlEntry(descriptor);
      if (accessControlEntry1 != null && (accessControlEntry1.Allow & 196608) == 196608)
        return;
      AccessControlEntry accessControlEntry2 = new AccessControlEntry(descriptor, 196608, 0);
      gitNamespace.SetAccessControlEntry(requestContext, token, (IAccessControlEntry) accessControlEntry2, true);
    }
  }
}
