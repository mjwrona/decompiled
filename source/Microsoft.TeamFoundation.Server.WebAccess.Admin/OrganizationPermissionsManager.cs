// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.OrganizationPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class OrganizationPermissionsManager : SecurityNamespacePermissionsManager
  {
    private static readonly Dictionary<int, string> s_knownOrganizationPropertiesPermissionBitToDisplayNameMap = new Dictionary<int, string>()
    {
      {
        4,
        AdminServerResources.OrganizationPermissionsModifyOrgPoliciesAndAttributes
      }
    };
    private static readonly Dictionary<int, string> s_knownCollectionPermissionBitToDisplayNameMap = new Dictionary<int, string>()
    {
      {
        2,
        AdminServerResources.OrganizationPermissionsCreateCollections
      },
      {
        4,
        AdminServerResources.OrganizationPermissionsModifyCollections
      },
      {
        8,
        AdminServerResources.OrganizationPermissionsDeleteCollections
      }
    };
    private static readonly Guid OrganizationCollectionsPermissionSet = new Guid("8775E774-2855-43C9-B7C7-BDEF8BE670A5");
    private static readonly Guid OrganizationPropertiesPermissionSet = new Guid("BE407167-826D-405E-BA27-05A08DEE08B5");

    public OrganizationPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token)
      : base(requestContext, permissionsIdentifier, token)
    {
    }

    public override bool CanEditAdminPermissions => false;

    public override bool CanTokenInheritPermissions => true;

    protected override bool CanUserViewPermissions(IVssRequestContext requestContext) => true;

    protected override bool CanUserManageIdentities(IVssRequestContext requestContext) => true;

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      SecurityNamespacePermissionSet namespacePermissionSet1 = new SecurityNamespacePermissionSet(requestContext, OrganizationSecurity.NamespaceId, OrganizationSecurity.PropertiesToken, 4);
      permissionSets.Add(OrganizationPermissionsManager.OrganizationPropertiesPermissionSet, namespacePermissionSet1);
      SecurityNamespacePermissionSet namespacePermissionSet2 = new SecurityNamespacePermissionSet(requestContext, OrganizationSecurity.NamespaceId, OrganizationSecurity.CollectionsToken, 14);
      permissionSets.Add(OrganizationPermissionsManager.OrganizationCollectionsPermissionSet, namespacePermissionSet2);
      SecurityNamespacePermissionSet namespacePermissionSet3 = new SecurityNamespacePermissionSet(requestContext, OrganizationSecurity.NamespaceId, OrganizationSecurity.RootToken, 1);
      permissionSets.Add(OrganizationSecurity.NamespaceId, namespacePermissionSet3);
      return permissionSets;
    }

    public override IList<SettableAction> GetPermissions(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      return (IList<SettableAction>) OrganizationPermissionsManager.FilterSettableActions(base.GetPermissions(requestContext, descriptor)).OrderBy<SettableAction, string>((Func<SettableAction, string>) (x => x.Token)).ToList<SettableAction>();
    }

    private static IList<SettableAction> FilterSettableActions(IList<SettableAction> settableActions)
    {
      IList<SettableAction> settableActionList = (IList<SettableAction>) new List<SettableAction>();
      foreach (SettableAction settableAction in (IEnumerable<SettableAction>) settableActions)
      {
        if (settableAction != null && settableAction.Token != null && settableAction.ActionDefinition != null && (string.Equals(settableAction.Token, OrganizationSecurity.CollectionsToken) || string.Equals(settableAction.Token, OrganizationSecurity.PropertiesToken)) && settableAction.ActionDefinition.Bit != 1)
        {
          bool flag = false;
          string displayName = (string) null;
          if (string.Equals(settableAction.Token, OrganizationSecurity.CollectionsToken))
            flag = OrganizationPermissionsManager.s_knownCollectionPermissionBitToDisplayNameMap.TryGetValue(settableAction.ActionDefinition.Bit, out displayName);
          if (string.Equals(settableAction.Token, OrganizationSecurity.PropertiesToken))
            flag = OrganizationPermissionsManager.s_knownOrganizationPropertiesPermissionBitToDisplayNameMap.TryGetValue(settableAction.ActionDefinition.Bit, out displayName);
          if (flag && !string.IsNullOrWhiteSpace(displayName))
            settableAction.ActionDefinition = new ActionDefinition(settableAction.ActionDefinition.NamespaceId, settableAction.ActionDefinition.Bit, settableAction.ActionDefinition.Name, displayName);
          settableActionList.Add(settableAction);
        }
      }
      return settableActionList;
    }
  }
}
