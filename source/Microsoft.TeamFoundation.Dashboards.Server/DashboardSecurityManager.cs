// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DashboardSecurityManager
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Dashboards.Security;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards
{
  public class DashboardSecurityManager : IDashboardSecurityManager
  {
    public void SetDefaultDashboardWithManageGroupPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      ITeamFoundationSecurityService service = requestContext.GetService<ITeamFoundationSecurityService>();
      IVssSecurityNamespace securityNamespace1 = service.GetSecurityNamespace(requestContext, DashboardGroupPrivileges.NamespaceId);
      IVssSecurityNamespace securityNamespace2 = service.GetSecurityNamespace(requestContext, DashboardsPrivileges.NamespaceId);
      WebApiTeam teamInProject = requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, projectId, teamId.ToString());
      this.ThrowIfTeamNotMaterialized(teamInProject, teamId);
      string newNamespaceToken = DashboardSecurityManager.GetNewNamespaceToken(projectId, new Guid?(teamId));
      IdentityDescriptor descriptor = teamInProject.Identity.Descriptor;
      IAccessControlEntry accessControlEntry = securityNamespace2.QueryAccessControlList(requestContext, newNamespaceToken, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, false)?.QueryAccessControlEntry(descriptor);
      if (accessControlEntry != null && !accessControlEntry.IsEmpty)
        return;
      if (securityNamespace1 != null)
        securityNamespace1.SetPermissions(requestContext, DashboardSecurityManager.GetLegacyToken(teamId), descriptor, 7, 0, false);
      if (DashboardSecurityManager.IsNamespaceNullOrEmpty(securityNamespace2, requestContext))
        return;
      securityNamespace2.SetPermissions(requestContext, newNamespaceToken, descriptor, 15, 0, false);
    }

    public void SetDashboardAllPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid dashboardId,
      Guid? ownerId)
    {
      IdentityDescriptor descriptor = requestContext.UserContext;
      if (ownerId.HasValue)
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        IList<Guid> guidList = (IList<Guid>) new List<Guid>();
        guidList.Add(ownerId.Value);
        IVssRequestContext requestContext1 = requestContext;
        IList<Guid> identityIds = guidList;
        descriptor = (service.ReadIdentities(requestContext1, identityIds, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() ?? throw new IdentityNotFoundException("Owner identity is invalid.")).Descriptor;
      }
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DashboardsPrivileges.NamespaceId).SetPermissions(requestContext, DashboardSecurityManager.GetNewNamespaceToken(projectId, new Guid?(Guid.Empty), new Guid?(dashboardId)), descriptor, DashboardsPrivileges.AllPermissions(), 0, false);
    }

    public void SetDashboardWithReadGroupPermission(IVssRequestContext requestContext, Guid teamId)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DashboardGroupPrivileges.NamespaceId);
      WebApiTeam teamByGuid = requestContext.GetService<ITeamService>().GetTeamByGuid(requestContext, teamId);
      this.ThrowIfTeamNotMaterialized(teamByGuid, teamId);
      IVssRequestContext requestContext1 = requestContext;
      string legacyToken = DashboardSecurityManager.GetLegacyToken(teamByGuid.Id);
      IdentityDescriptor descriptor = teamByGuid.Identity.Descriptor;
      securityNamespace.SetPermissions(requestContext1, legacyToken, descriptor, 1, 0, false);
    }

    public void SetProjectDashboardsCreatePermission(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DashboardsPrivileges.NamespaceId);
      try
      {
        IdentityDescriptor projectValidUserGroup = DashboardSecurityManager.GetProjectValidUserGroup(requestContext, projectId);
        securityNamespace.SetPermissions(requestContext, DashboardSecurityManager.GetNewNamespaceToken(projectId, new Guid?(Guid.Empty)), projectValidUserGroup, 3, 0, false);
      }
      catch (GroupScopeDoesNotExistException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "Permission", "Dashboards", "No project valid users group found for project '{0}'", (object) projectId);
      }
    }

    private static IdentityDescriptor GetProjectValidUserGroup(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return IdentityDomain.MapFromWellKnownIdentifier(requestContext.GetService<IdentityService>().GetScope(requestContext, projectId).Id, GroupWellKnownIdentityDescriptors.EveryoneGroup);
    }

    private static IVssSecurityNamespace GetSecurityNamespace(IVssRequestContext requestContext)
    {
      ITeamFoundationSecurityService service = requestContext.GetService<ITeamFoundationSecurityService>();
      Guid namespaceId1 = DashboardsPrivileges.NamespaceId;
      IVssRequestContext requestContext1 = requestContext;
      Guid namespaceId2 = namespaceId1;
      return service.GetSecurityNamespace(requestContext1, namespaceId2);
    }

    public int GetTeamMemberGroupPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      IVssSecurityNamespace securityNamespace = DashboardSecurityManager.GetSecurityNamespace(requestContext);
      WebApiTeam teamInProject = requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, projectId, teamId.ToString());
      this.ThrowIfTeamNotMaterialized(teamInProject, teamId);
      IVssRequestContext requestContext1 = requestContext;
      string securityToken = DashboardSecurityManager.GetSecurityToken(requestContext, projectId, new Guid?(teamId));
      EvaluationPrincipal descriptor = (EvaluationPrincipal) teamInProject.Identity.Descriptor;
      return securityNamespace.QueryEffectivePermissions(requestContext1, securityToken, descriptor);
    }

    public int GetEffectivePermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId)
    {
      IVssSecurityNamespace securityNamespace = DashboardSecurityManager.GetSecurityNamespace(requestContext);
      string securityToken = DashboardSecurityManager.GetSecurityToken(requestContext, projectId, new Guid?(teamId), dashboardId);
      IVssRequestContext requestContext1 = requestContext;
      string token = securityToken;
      return securityNamespace.QueryEffectivePermissions(requestContext1, token);
    }

    public void CheckReadPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId)
    {
      DashboardSecurityManager.CheckDashboardsPermission(requestContext, 1, 1, projectId, teamId, dashboardId);
    }

    public bool HasReadPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId)
    {
      return DashboardSecurityManager.HasDashboardsPermission(requestContext, 1, 1, projectId, teamId, dashboardId);
    }

    public void CheckCreatePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      DashboardSecurityManager.CheckDashboardsPermission(requestContext, 2, 4, projectId, teamId, new Guid?());
    }

    public void CheckEditPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId)
    {
      DashboardSecurityManager.CheckDashboardsPermission(requestContext, 4, 2, projectId, teamId, dashboardId);
    }

    public void CheckDeletePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId)
    {
      DashboardSecurityManager.CheckDashboardsPermission(requestContext, 8, 4, projectId, teamId, dashboardId);
    }

    private static void CheckDashboardsPermission(
      IVssRequestContext requestContext,
      int permission,
      int groupPermission,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId)
    {
      IVssSecurityNamespace securityNamespace = DashboardSecurityManager.GetSecurityNamespace(requestContext);
      string securityToken = DashboardSecurityManager.GetSecurityToken(requestContext, projectId, new Guid?(teamId), dashboardId);
      int num = permission;
      IVssRequestContext requestContext1 = requestContext;
      string token = securityToken;
      int requestedPermissions = num;
      securityNamespace.CheckPermission(requestContext1, token, requestedPermissions);
    }

    private static bool HasDashboardsPermission(
      IVssRequestContext requestContext,
      int permission,
      int groupPermission,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId)
    {
      IVssSecurityNamespace securityNamespace = DashboardSecurityManager.GetSecurityNamespace(requestContext);
      string securityToken = DashboardSecurityManager.GetSecurityToken(requestContext, projectId, new Guid?(teamId), dashboardId);
      int num = permission;
      IVssRequestContext requestContext1 = requestContext;
      string token = securityToken;
      int requestedPermissions = num;
      return securityNamespace.HasPermission(requestContext1, token, requestedPermissions);
    }

    public static string GetSecurityToken(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? teamId,
      Guid? dashboardId = null)
    {
      return DashboardSecurityManager.GetNewNamespaceToken(projectId, teamId, dashboardId);
    }

    private static string GetNewNamespaceToken(Guid projectId, Guid? teamId, Guid? dashboardId = null)
    {
      if (!teamId.HasValue)
        return string.Format("{0}/{1}", (object) DashboardsPrivileges.Root, (object) projectId);
      if (!dashboardId.HasValue)
        return string.Format("{0}/{1}/{2}", (object) DashboardsPrivileges.Root, (object) projectId, (object) teamId.Value);
      return string.Format("{0}/{1}/{2}/{3}", (object) DashboardsPrivileges.Root, (object) projectId, (object) teamId.Value, (object) dashboardId.Value);
    }

    private static string GetLegacyToken(Guid teamId, Guid? dashboardId = null) => !dashboardId.HasValue ? string.Format("{0}/{1}", (object) DashboardGroupPrivileges.Root, (object) teamId) : string.Format("{0}/{1}/{2}", (object) DashboardGroupPrivileges.Root, (object) teamId, (object) dashboardId.Value);

    private void ThrowIfTeamNotMaterialized(WebApiTeam team, Guid teamId)
    {
      if (team == null || team.Identity == null)
        throw new Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException(teamId.ToString());
    }

    public static int GetDashboardScopedPermissions(int permissions)
    {
      if (permissions == 0)
        return 0;
      if ((permissions & 8) != 0)
        return 31;
      if ((permissions & 4) != 0)
        return 15;
      return (permissions & 2) != 0 ? 5 : 1;
    }

    private static bool IsNamespaceNullOrEmpty(
      IVssSecurityNamespace @namespace,
      IVssRequestContext requestContext)
    {
      return @namespace == null || !@namespace.QueryAccessControlLists(requestContext, (string) null, false, true).Any<IAccessControlList>();
    }

    public bool HasMaterializeDashboardsPermission(IVssRequestContext requestContext)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DashboardsPrivileges.NamespaceId);
      string materializeDashboardsRoot = DashboardsPrivileges.MaterializeDashboardsRoot;
      IVssRequestContext requestContext1 = requestContext;
      string token = materializeDashboardsRoot;
      return securityNamespace.HasPermission(requestContext1, token, 32);
    }
  }
}
