// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.EnvironmentSecurityProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class EnvironmentSecurityProvider
  {
    public static readonly Guid EnvironmentNamespaceId = new Guid("83d4c2e6-e57d-4d6e-892b-b87222b7ad20");
    public static readonly string EnvironmentReferenceRoleScopeId = "distributedtask.environmentreferencerole";
    public static readonly string GlobalEnvironmentReferenceRoleScopeId = "distributedtask.globalenvironmentreferencerole";
    public static readonly string EnvironmentRoleScopeId = "distributedtask.environmentrole";
    public static readonly string GlobalEnvironmentRoleScopeId = "distributedtask.globalenvironmentrole";
    private const char NamespaceSeperator = '/';

    public static string EnvironmentTokenPrefix => "Environments";

    public static string GetEnvironmentToken(Guid? projectId = null, int? environmentId = null)
    {
      if (projectId.HasValue && environmentId.HasValue)
        return EnvironmentSecurityProvider.EnvironmentTokenPrefix + (object) '/' + projectId.Value.ToString("D") + (object) '/' + (object) environmentId.Value;
      if (projectId.HasValue)
        return EnvironmentSecurityProvider.EnvironmentTokenPrefix + (object) '/' + projectId.Value.ToString("D");
      return environmentId.HasValue ? EnvironmentSecurityProvider.EnvironmentTokenPrefix + (object) '/' + (object) environmentId.Value : EnvironmentSecurityProvider.EnvironmentTokenPrefix;
    }

    public static void RemoveAccessControlLists(
      IVssRequestContext requestContext,
      Guid projectId,
      int? environmentId = null)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, EnvironmentSecurityProvider.EnvironmentNamespaceId);
      string str = environmentId.HasValue ? EnvironmentSecurityProvider.GetEnvironmentToken(new Guid?(projectId), new int?(environmentId.Value)) : EnvironmentSecurityProvider.GetEnvironmentToken(new Guid?(projectId));
      IVssRequestContext requestContext1 = requestContext;
      string[] tokens = new string[1]{ str };
      securityNamespace.RemoveAccessControlLists(requestContext1, (IEnumerable<string>) tokens, true);
    }

    public void CheckCreatePermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      bool alwaysAllowAdministrators = true)
    {
      IVssRequestContext requestContext1 = requestContext;
      Guid? projectId1 = new Guid?(projectId);
      bool flag = alwaysAllowAdministrators;
      int? environmentId = new int?();
      int num = flag ? 1 : 0;
      this.CheckPermissions(requestContext1, 32, projectId1, environmentId, num != 0);
    }

    public void CheckViewPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      bool alwaysAllowAdministrators = true)
    {
      this.CheckPermissions(requestContext, 1, new Guid?(projectId), new int?(environmentId), alwaysAllowAdministrators);
    }

    public void CheckViewHistoryPermissions(
      IVssRequestContext requestContext,
      Guid? projectId,
      int environmentId,
      bool alwaysAllowAdministrators = true)
    {
      this.CheckPermissions(requestContext, 1, projectId, new int?(environmentId), alwaysAllowAdministrators);
    }

    public void CheckViewHistoryPermissions(
      IVssRequestContext requestContext,
      Guid? projectId,
      bool alwaysAllowAdministrators = true)
    {
      this.CheckPermissions(requestContext, 1, projectId, alwaysAllowAdministrators: alwaysAllowAdministrators);
    }

    public void CheckManageHistoryPermissions(IVssRequestContext requestContext) => this.CheckPermissions(requestContext, 4);

    public void CheckManagePermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      bool alwaysAllowAdministrators = true)
    {
      this.CheckPermissions(requestContext, 2, new Guid?(projectId), new int?(environmentId), alwaysAllowAdministrators);
    }

    public bool HasViewPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      bool alwaysAllowAdministrators = true)
    {
      string environmentToken1 = EnvironmentSecurityProvider.GetEnvironmentToken(environmentId: new int?(environmentId));
      string environmentToken2 = EnvironmentSecurityProvider.GetEnvironmentToken(new Guid?(projectId), new int?(environmentId));
      return this.HasPermissions(requestContext, environmentToken2, 1, alwaysAllowAdministrators) || this.HasPermissions(requestContext, environmentToken1, 1, alwaysAllowAdministrators);
    }

    public static void InitializePermissions(IVssRequestContext requestContext, Guid projectId)
    {
      EnvironmentSecurityProvider.RemoveAccessControlLists(requestContext, projectId);
      IList<IAccessControlEntry> accessControlEntries = EnvironmentSecurityProvider.GetDefaultAccessControlEntries(requestContext, projectId);
      EnvironmentSecurityProvider.SetDefaultAccessControlList(requestContext, accessControlEntries, EnvironmentSecurityProvider.GetEnvironmentToken(new Guid?(projectId)));
    }

    public static void GrantProjectAdministratorsCreateEnvironmentPermissions(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> existingGroups = requestContext.GetService<IdentityService>().ListGroups(requestContext, new Guid[1]
      {
        projectId
      }, false, (IEnumerable<string>) null);
      List<IAccessControlEntry> accessControlEntryList = new List<IAccessControlEntry>();
      EnvironmentSecurityProvider.AddAces((IList<IAccessControlEntry>) accessControlEntryList, existingGroups, TaskResources.ProjectAdministratorsGroupName(), 33);
      EnvironmentSecurityProvider.SetDefaultAccessControlList(requestContext, (IList<IAccessControlEntry>) accessControlEntryList, EnvironmentSecurityProvider.GetEnvironmentToken(new Guid?(projectId)));
    }

    public virtual void CheckPermissions(
      IVssRequestContext requestContext,
      int requiredPermissions,
      Guid? projectId = null,
      int? environmentId = null,
      bool alwaysAllowAdministrators = true)
    {
      if (requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, EnvironmentSecurityProvider.EnvironmentNamespaceId).HasPermission(requestContext, EnvironmentSecurityProvider.GetEnvironmentToken(projectId, environmentId), requiredPermissions, alwaysAllowAdministrators))
        return;
      EnvironmentSecurityProvider.ThrowAccessDenied(requestContext, requiredPermissions, environmentId);
    }

    public virtual bool HasPermissions(
      IVssRequestContext requestContext,
      string token,
      int requiredPermissions,
      bool alwaysAllowAdministrators = true)
    {
      if (requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, EnvironmentSecurityProvider.EnvironmentNamespaceId);
      return securityNamespace.HasPermission(requestContext, token, requiredPermissions, alwaysAllowAdministrators) || securityNamespace.PollForRequestLocalInvalidation(requestContext) && securityNamespace.HasPermission(requestContext, token, requiredPermissions, alwaysAllowAdministrators);
    }

    private static void SetDefaultAccessControlList(
      IVssRequestContext requestContext,
      IList<IAccessControlEntry> accessControlEntries,
      string token)
    {
      if (accessControlEntries.Count <= 0)
        return;
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, EnvironmentSecurityProvider.EnvironmentNamespaceId).SetAccessControlEntries(requestContext, token, (IEnumerable<IAccessControlEntry>) accessControlEntries, true);
    }

    private static IList<IAccessControlEntry> GetDefaultAccessControlEntries(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> existingGroups = requestContext.GetService<IdentityService>().ListGroups(requestContext, new Guid[1]
      {
        projectId
      }, false, (IEnumerable<string>) null);
      List<IAccessControlEntry> aces = new List<IAccessControlEntry>();
      EnvironmentSecurityProvider.AddAces((IList<IAccessControlEntry>) aces, existingGroups, TaskResources.ProjectContributorsGroupName(), 33);
      EnvironmentSecurityProvider.AddAces((IList<IAccessControlEntry>) aces, existingGroups, TaskResources.ProjectAdministratorsGroupName(), 33);
      EnvironmentSecurityProvider.AddAces((IList<IAccessControlEntry>) aces, existingGroups, FrameworkResources.ProjectValidUsersGroupName(), 1);
      return (IList<IAccessControlEntry>) aces;
    }

    private static void AddAces(
      IList<IAccessControlEntry> aces,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> existingGroups,
      string identityGroupName,
      int permissions)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = existingGroups.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group => group.GetProperty<string>("Account", string.Empty).Equals(identityGroupName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        return;
      aces.Add((IAccessControlEntry) new AccessControlEntry(identity.Descriptor, permissions, 0));
    }

    private static void ThrowAccessDenied(
      IVssRequestContext requestContext,
      int requiredPermissions,
      int? environmentId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      string str = string.Join(", ", EnvironmentSecurityProvider.GetPermissionStrings(requiredPermissions));
      if (environmentId.HasValue)
        throw new AccessDeniedException(TaskResources.AccessDeniedForEnvironment((object) userIdentity.DisplayName, (object) str, (object) environmentId));
      throw new AccessDeniedException(TaskResources.AccessDenied((object) userIdentity.DisplayName, (object) str));
    }

    private static string[] GetPermissionStrings(int permissions)
    {
      List<string> stringList = new List<string>();
      if ((permissions & 1) != 0)
        stringList.Add(TaskResources.View());
      if ((permissions & 8) != 0)
        stringList.Add(TaskResources.AdministerPermissions());
      if ((permissions & 32) != 0)
        stringList.Add(TaskResources.Create());
      if ((permissions & 2) != 0)
        stringList.Add(TaskResources.Manage());
      if ((permissions & 16) != 0)
        stringList.Add(TaskResources.Use());
      if ((permissions & 4) != 0)
        stringList.Add(TaskResources.ManageHistory());
      return stringList.ToArray();
    }
  }
}
