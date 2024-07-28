// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsPermission
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public static class AnalyticsPermission
  {
    private const string c_area = "Permission";
    private const string c_layer = "Analytics";
    private const string c_stagingPermissionAlreadySetRegistryKey = "/Configuration/AnalyticsService/StagingPermissionApplied/";

    public static void EmbedProjectInfoToRequestContext(
      this IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      if (projectInfo != null)
        requestContext.Items["AnalyticsProjectSK"] = (object) projectInfo.Id;
      else
        requestContext.Items.Remove("AnalyticsProjectSK");
    }

    public static void SetAnalyticsProjectPermission(
      this IVssRequestContext requestContext,
      ProjectInfo project,
      Func<ProjectInfo, bool> shouldSetPermissionPredicate = null)
    {
      AnalyticsPermission.CreateDataspaceIfNotExists(requestContext, project.Id);
      if (AnalyticsPermission.IsPermissionSetAlready(requestContext, project.Id.ToString()) || shouldSetPermissionPredicate != null && !shouldSetPermissionPredicate(project))
        return;
      requestContext.TraceAlways(12017002, TraceLevel.Info, "Permission", "Analytics", string.Format("Setting View Analytics permission for {0}({1})", (object) project.Name, (object) project.Id));
      IVssSecurityNamespace securityNamespace = AnalyticsPermission.GetSecurityNamespace(requestContext);
      IdentityDescriptor projectValidUserGroup = AnalyticsPermission.GetProjectValidUserGroup(requestContext, project);
      IVssRequestContext requestContext1 = requestContext;
      string securityToken = AnalyticsSecurityNamespace.GetSecurityToken(project.Id);
      IdentityDescriptor descriptor = projectValidUserGroup;
      securityNamespace.SetPermissions(requestContext1, securityToken, descriptor, 1, 0, false);
      AnalyticsPermission.WritePermissionSetFlag(requestContext, project.Id.ToString());
    }

    public static IdentityDescriptor GetProjectValidUserGroup(
      IVssRequestContext requestContext,
      ProjectInfo project)
    {
      return AnalyticsPermission.GetProjectValidUserGroup(requestContext, project.Id);
    }

    public static IdentityDescriptor GetProjectValidUserGroup(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return IdentityDomain.MapFromWellKnownIdentifier(requestContext.GetService<IdentityService>().GetScope(requestContext, projectId).Id, GroupWellKnownIdentityDescriptors.EveryoneGroup);
    }

    public static void SetAnalyticsProjectPermissionIfNotSet(this IVssRequestContext requestContext)
    {
      Func<ProjectInfo, bool> shouldSetPermissionPredicate = (Func<ProjectInfo, bool>) (project =>
      {
        IVssSecurityNamespace securityNamespace = AnalyticsPermission.GetSecurityNamespace(requestContext);
        string securityToken = AnalyticsSecurityNamespace.GetSecurityToken(project.Id);
        IVssRequestContext requestContext1 = requestContext;
        string token = securityToken;
        if (!securityNamespace.QueryAccessControlLists(requestContext1, token, false, false).Any<IAccessControlList>())
          return true;
        requestContext.TraceAlways(12017001, TraceLevel.Info, "Permission", "Analytics", string.Format("Skipping setting permission for project {0}({1}) because it already has View Analytics permission set", (object) project.Name, (object) project.Id));
        return false;
      });
      foreach (ProjectInfo project in requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed))
        requestContext.SetAnalyticsProjectPermission(project, shouldSetPermissionPredicate);
    }

    public static void RemoveAdministerPermissionFromUserStore(IVssRequestContext requestContext)
    {
      IVssSecurityNamespace securityNamespace = AnalyticsPermission.GetSecurityNamespace(requestContext);
      foreach (ProjectInfo project in requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed))
      {
        string securityToken = AnalyticsSecurityNamespace.GetSecurityToken(project.Id);
        securityNamespace.RemovePermissions(requestContext, securityToken, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, 3);
      }
    }

    public static void RemoveAnalyticsProjectPermissions(
      this IVssRequestContext requestContext,
      ProjectInfo project)
    {
      IVssSecurityNamespace securityNamespace = AnalyticsPermission.GetSecurityNamespace(requestContext);
      foreach (IAccessControlList accessControlList in securityNamespace.QueryAccessControlLists(requestContext, AnalyticsSecurityNamespace.GetSecurityToken(project.Id), false, true))
        securityNamespace.RemoveAccessControlEntries(requestContext, accessControlList.Token, accessControlList.AccessControlEntries);
      AnalyticsPermission.RemovePermissionSetFlag(requestContext, project.Id.ToString());
    }

    public static void RemoveAnalyticsProjectPermissions(
      IVssRequestContext requestContext,
      ICollection<Guid> projectGuids)
    {
      AnalyticsPermission.GetSecurityNamespace(requestContext).RemoveAccessControlLists(requestContext, projectGuids.Select<Guid, string>((Func<Guid, string>) (guid => AnalyticsSecurityNamespace.GetSecurityToken(guid))), true);
      foreach (Guid projectGuid in (IEnumerable<Guid>) projectGuids)
        AnalyticsPermission.RemovePermissionSetFlag(requestContext, projectGuid.ToString());
    }

    public static ICollection<IAccessControlList> GetExistingProjectAcls(
      IVssRequestContext requestContext)
    {
      return (ICollection<IAccessControlList>) AnalyticsPermission.GetSecurityNamespace(requestContext).QueryAccessControlLists(requestContext, "$", (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        requestContext.UserContext
      }, false, true).ToList<IAccessControlList>();
    }

    public static void SetProjectDefaultPermissionsForViews(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      AnalyticsPermission.SetProjectDefaultPermissionsForViews(requestContext, service.GetProject(requestContext, projectId));
    }

    public static void SetProjectDefaultPermissionsForViews(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, AnalyticsViewsSecurityNamespace.Id);
      if (securityNamespace == null)
        return;
      AnalyticsPermission.CreateDataspaceIfNotExists(requestContext, projectInfo.Id);
      string token = AnalyticsViewsSecurityNamespace.GetSecurityToken(AnalyticsViewVisibility.Shared, projectInfo.Id);
      if (!((Func<bool>) (() => !securityNamespace.QueryAccessControlLists(requestContext, token, false, false).Any<IAccessControlList>()))())
        return;
      try
      {
        securityNamespace.SetPermissions(requestContext, AnalyticsViewsSecurityNamespace.GetSecurityToken(AnalyticsViewVisibility.Shared, projectInfo.Id), AnalyticsPermission.GetProjectValidUserGroup(requestContext, projectInfo), AnalyticsViewsSecurityNamespace.DefaultProjectValidUsersAllowPermissions, AnalyticsViewsSecurityNamespace.DefaultProjectValidUsersDenyPermissions, false);
      }
      catch (GroupScopeDoesNotExistException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "Permission", "Analytics", "No project valid users group found for project '{0}'", (object) projectInfo.Uri);
      }
      try
      {
        securityNamespace.SetPermissions(requestContext, AnalyticsViewsSecurityNamespace.GetSecurityToken(AnalyticsViewVisibility.Shared, projectInfo.Id), AnalyticsPermission.GetProjectAdminsGroup(requestContext, projectInfo), AnalyticsViewsSecurityNamespace.DefaultProjectAdminAllowPermissions, AnalyticsViewsSecurityNamespace.DefaultProjectAdminDenyPermissions, false);
      }
      catch (GroupScopeDoesNotExistException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "Permission", "Analytics", "No project admins group found for project '{0}'", (object) projectInfo.Uri);
      }
    }

    public static IdentityDescriptor GetProjectAdminsGroup(
      IVssRequestContext requestContext,
      ProjectInfo project)
    {
      requestContext.GetService<IdentityService>();
      return requestContext.GetService<IdentityService>().GetScope(requestContext, project.Id).Administrators;
    }

    public static void CreateDataspaceIfNotExists(IVssRequestContext requestContext, Guid projectId)
    {
      IDataspaceService service = requestContext.GetService<IDataspaceService>();
      if (service.QueryDataspace(requestContext, "Default", projectId, false) != null)
        return;
      service.CreateDataspace(requestContext, "Default", projectId, DataspaceState.Active);
    }

    internal static bool IsPermissionSetAlready(IVssRequestContext requestContext, string scope) => requestContext.GetService<ISqlRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) AnalyticsPermission.GetPermissionRegistryKey(scope), false);

    private static void WritePermissionSetFlag(IVssRequestContext requestContext, string scope) => requestContext.GetService<ISqlRegistryService>().SetValue<bool>(requestContext, AnalyticsPermission.GetPermissionRegistryKey(scope), true);

    private static void RemovePermissionSetFlag(IVssRequestContext requestContext, string scope) => requestContext.GetService<ISqlRegistryService>().DeleteEntries(requestContext, AnalyticsPermission.GetPermissionRegistryKey(scope));

    private static string GetPermissionRegistryKey(string scope) => AnalyticsSecurityNamespace.GetODataPermissionAlreadySetRegistryKey(scope);

    public static IVssSecurityNamespace GetSecurityNamespace(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, AnalyticsSecurityNamespace.Id).Secured();
  }
}
