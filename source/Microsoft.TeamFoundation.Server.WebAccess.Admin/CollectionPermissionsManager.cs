// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.CollectionPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class CollectionPermissionsManager : SecurityNamespacePermissionsManager
  {
    public CollectionPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token)
      : base(requestContext, permissionsIdentifier, token)
    {
    }

    public override void SetPermission(
      IVssRequestContext requestContext,
      SettableAction settableAction,
      bool allowSet,
      bool denySet)
    {
      base.SetPermission(requestContext, settableAction, allowSet, denySet);
      if (!(settableAction.ActionDefinition.NamespaceId == FrameworkSecurity.ProcessNamespaceId) || (settableAction.ActionDefinition.Bit & 1) != 1)
        return;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.NamespaceSecurityGuid);
      if (securityNamespace == null)
        return;
      IAccessControlEntry accessControlEntry = !allowSet ? (IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(settableAction.AccessControlEntry.Descriptor, 0, AuthorizationNamespacePermissions.ManageTemplate) : (IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(settableAction.AccessControlEntry.Descriptor, AuthorizationNamespacePermissions.ManageTemplate, 0);
      securityNamespace.SetAccessControlEntry(requestContext, AuthorizationSecurityConstants.NamespaceSecurityToken, accessControlEntry, true);
    }

    protected override bool CanUserViewPermissions(IVssRequestContext requestContext)
    {
      SecurityNamespacePermissionSet namespacePermissionSet;
      if (!this.PermissionSets.TryGetValue(FrameworkSecurity.FrameworkNamespaceId, out namespacePermissionSet))
        return false;
      string token = namespacePermissionSet.HandleIncomingToken(requestContext, FrameworkSecurity.FrameworkNamespaceToken);
      return namespacePermissionSet.HasReadPermission(requestContext, token);
    }

    protected override bool CanUserManageIdentities(IVssRequestContext requestContext) => this.CanUserManageIdentities(requestContext, AuthorizationSecurityConstants.NamespaceSecurityGuid);

    protected override IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      string str = FrameworkSecurity.CollectionManagementNamespaceToken + (object) FrameworkSecurity.CollectionManagementPathSeparator;
      return token.StartsWith(str, StringComparison.OrdinalIgnoreCase) ? this.PermissionSets[FrameworkSecurity.CollectionManagementNamespaceId].SecuredSecurityNamespace.QueryAccessControlLists(requestContext, FrameworkSecurity.CollectionManagementNamespaceToken, (IEnumerable<IdentityDescriptor>) descriptors, true, false).FirstOrDefault<IAccessControlList>() : (IAccessControlList) null;
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int num = AuthorizationNamespacePermissions.GenericRead | AuthorizationNamespacePermissions.GenericWrite | AuthorizationNamespacePermissions.TriggerEvent;
      if (requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy"))
      {
        num |= AuthorizationNamespacePermissions.ManageTemplate;
        SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, FrameworkSecurity.ProcessNamespaceId, PermissionNamespaces.Process, SecurityNamespacePermissionSet.AllPermissions);
        permissionSets.Add(FrameworkSecurity.ProcessNamespaceId, namespacePermissionSet);
      }
      SecurityNamespacePermissionSet namespacePermissionSet1 = new SecurityNamespacePermissionSet(requestContext, AuthorizationSecurityConstants.NamespaceSecurityGuid, AuthorizationSecurityConstants.NamespaceSecurityObjectId, SecurityNamespacePermissionSet.AllPermissions & ~num);
      permissionSets.Add(AuthorizationSecurityConstants.NamespaceSecurityGuid, namespacePermissionSet1);
      SecurityNamespacePermissionSet namespacePermissionSet2 = new SecurityNamespacePermissionSet(requestContext, new Guid("66312704-DEB5-43f9-B51C-AB4FF5E351C3"), "Global", SecurityNamespacePermissionSet.AllPermissions & -49);
      permissionSets.Add(new Guid("66312704-DEB5-43f9-B51C-AB4FF5E351C3"), namespacePermissionSet2);
      SecurityNamespacePermissionSet namespacePermissionSet3 = new SecurityNamespacePermissionSet(requestContext, FrameworkSecurity.FrameworkNamespaceId, FrameworkSecurity.FrameworkNamespaceToken);
      permissionSets.Add(FrameworkSecurity.FrameworkNamespaceId, namespacePermissionSet3);
      TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
      if (executionEnvironment.IsOnPremisesDeployment)
      {
        string token = FrameworkSecurity.CollectionManagementNamespaceToken + (object) FrameworkSecurity.CollectionManagementPathSeparator + requestContext.ServiceHost.InstanceId.ToString();
        SecurityNamespacePermissionSet namespacePermissionSet4 = new SecurityNamespacePermissionSet(requestContext.To(TeamFoundationHostType.Application), FrameworkSecurity.CollectionManagementNamespaceId, token, 2);
        permissionSets.Add(FrameworkSecurity.CollectionManagementNamespaceId, namespacePermissionSet4);
      }
      SecurityNamespacePermissionSet namespacePermissionSet5 = new SecurityNamespacePermissionSet(requestContext, AuthorizationSecurityConstants.ProjectSecurityGuid, AuthorizationSecurityConstants.ProjectSecurityPrefix, AuthorizationProjectPermissions.Delete);
      permissionSets.Add(AuthorizationSecurityConstants.ProjectSecurityGuid, namespacePermissionSet5);
      SecurityNamespacePermissionSet namespacePermissionSet6 = new SecurityNamespacePermissionSet(requestContext, new Guid("302ACACA-B667-436d-A946-87133492041C"), "BuildPrivileges");
      permissionSets.Add(new Guid("302ACACA-B667-436d-A946-87133492041C"), namespacePermissionSet6);
      executionEnvironment = requestContext.ExecutionEnvironment;
      int permissionsToDisplay = executionEnvironment.IsHostedDeployment ? 13 : 1;
      SecurityNamespacePermissionSet namespacePermissionSet7 = new SecurityNamespacePermissionSet(requestContext, AuditLogConstants.SecurityNamespaceId, "AllPermissions", permissionsToDisplay);
      permissionSets.Add(AuditLogConstants.SecurityNamespaceId, namespacePermissionSet7);
      return permissionSets;
    }
  }
}
