// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.MetaTaskPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class MetaTaskPermissionsManager : SecurityNamespacePermissionsManager
  {
    public static readonly Guid SecurityNamespaceId = new Guid("f6a4de49-dbe2-4704-86dc-f8ec1a294436");
    public static readonly char NamespaceSeparator = '/';

    public MetaTaskPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token)
      : base(requestContext, permissionsIdentifier, token)
    {
      if (!this.UserHasReadAccess)
        return;
      this.InheritPermissions = this.PermissionSets[MetaTaskPermissionsManager.SecurityNamespaceId].GetAccessControlList(requestContext).InheritPermissions;
    }

    public override bool CanEditAdminPermissions => true;

    public override bool CanTokenInheritPermissions => this.Token.Contains(MetaTaskPermissionsManager.NamespaceSeparator.ToString());

    public override void ChangeInheritance(
      IVssRequestContext requestContext,
      bool inheritPermissions)
    {
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[MetaTaskPermissionsManager.SecurityNamespaceId];
      this.ChangeInheritance(requestContext, permissionSet, inheritPermissions);
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int allPermissions = MetaTaskPermissions.AllPermissions;
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, MetaTaskPermissionsManager.SecurityNamespaceId, this.Token, allPermissions);
      permissionSets.Add(MetaTaskPermissionsManager.SecurityNamespaceId, namespacePermissionSet);
      return permissionSets;
    }

    protected override IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      string parentToken = MetaTaskPermissionsManager.GetParentToken(token);
      return string.IsNullOrEmpty(parentToken) ? (IAccessControlList) null : this.PermissionSets[MetaTaskPermissionsManager.SecurityNamespaceId].SecuredSecurityNamespace.QueryAccessControlLists(requestContext, parentToken, (IEnumerable<IdentityDescriptor>) descriptors, true, false).FirstOrDefault<IAccessControlList>();
    }

    private static string GetParentToken(string token)
    {
      int length = token.LastIndexOf(MetaTaskPermissionsManager.NamespaceSeparator);
      return length < 0 ? (string) null : token.Substring(0, length);
    }
  }
}
