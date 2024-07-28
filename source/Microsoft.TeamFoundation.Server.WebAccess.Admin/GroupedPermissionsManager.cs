// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.GroupedPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal abstract class GroupedPermissionsManager : SecurityNamespacePermissionsManager
  {
    private IDictionary<int, GroupedPermission> m_groupedPermissionMap = (IDictionary<int, GroupedPermission>) new Dictionary<int, GroupedPermission>();

    public GroupedPermissionsManager(Guid permissionsIdentifier)
      : base(permissionsIdentifier)
    {
      this.ProcessGroupedPermissions();
    }

    public GroupedPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token)
      : base(requestContext, permissionsIdentifier, token)
    {
      this.ProcessGroupedPermissions();
    }

    public abstract Guid SecurityNamespace { get; }

    public abstract IList<GroupedPermission> GroupedPermissions { get; }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      int permissionsToDisplay = this.GroupedPermissions.Aggregate<GroupedPermission, int>(0, (Func<int, GroupedPermission, int>) ((current, groupedPermission) => current | groupedPermission.Permissions));
      return new Dictionary<Guid, SecurityNamespacePermissionSet>()
      {
        {
          this.SecurityNamespace,
          new SecurityNamespacePermissionSet(requestContext, this.SecurityNamespace, this.Token, permissionsToDisplay)
        }
      };
    }

    public override void SetPermission(
      IVssRequestContext requestContext,
      SettableAction settableAction,
      bool allowSet,
      bool denySet)
    {
      GroupedPermission groupedPermission;
      if (!this.m_groupedPermissionMap.TryGetValue(settableAction.ActionDefinition.Bit, out groupedPermission))
        throw new InvalidOperationException("No valid permission was found");
      settableAction.ActionDefinition = new ActionDefinition(settableAction.ActionDefinition.NamespaceId, groupedPermission.Permissions, settableAction.ActionDefinition.Name, settableAction.ActionDefinition.DisplayName);
      base.SetPermission(requestContext, settableAction, allowSet, denySet);
    }

    public override IList<SettableAction> GetPermissions(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      List<SettableAction> permissions = new List<SettableAction>();
      foreach (SettableAction permission in (IEnumerable<SettableAction>) base.GetPermissions(requestContext, descriptor))
      {
        GroupedPermission groupedPermission;
        if (this.m_groupedPermissionMap.TryGetValue(permission.ActionDefinition.Bit, out groupedPermission))
        {
          permission.ActionDefinition = new ActionDefinition(permission.ActionDefinition.NamespaceId, permission.ActionDefinition.Bit, permission.ActionDefinition.Name, groupedPermission.DisplayName);
          permissions.Add(permission);
        }
      }
      return (IList<SettableAction>) permissions;
    }

    public override TracePermissionModel GetTrace(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      PermissionUpdate permissionUpdate)
    {
      GroupedPermission groupedPermission;
      if (!this.m_groupedPermissionMap.TryGetValue(permissionUpdate.PermissionBit, out groupedPermission))
        throw new InvalidOperationException("No valid permission was found");
      TracePermissionModel trace = base.GetTrace(requestContext, descriptor, permissionUpdate);
      trace.ActionDefinition = new ActionDefinition(trace.ActionDefinition.NamespaceId, trace.ActionDefinition.Bit, trace.ActionDefinition.Name, groupedPermission.DisplayName);
      return trace;
    }

    protected abstract override bool CanUserManageIdentities(IVssRequestContext requestContext);

    protected abstract override bool CanUserViewPermissions(IVssRequestContext requestContext);

    public override IEnumerable<IAccessControlEntry> FilterAccessControlList(
      IEnumerable<IAccessControlEntry> accessControlEntries)
    {
      return accessControlEntries.Where<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (ace => this.DoesPermissionIncludeAGroupedPermission(ace.EffectiveAllow) || this.DoesPermissionIncludeAGroupedPermission(ace.EffectiveDeny)));
    }

    protected bool DoesPermissionIncludeAGroupedPermission(int permission)
    {
      foreach (GroupedPermission groupedPermission in (IEnumerable<GroupedPermission>) this.m_groupedPermissionMap.Values)
      {
        if ((groupedPermission.Permissions & permission) == groupedPermission.Permissions)
          return true;
      }
      return false;
    }

    private void ProcessGroupedPermissions()
    {
      foreach (GroupedPermission groupedPermission in (IEnumerable<GroupedPermission>) this.GroupedPermissions)
        this.m_groupedPermissionMap[groupedPermission.RepresentativePermission] = groupedPermission;
    }
  }
}
