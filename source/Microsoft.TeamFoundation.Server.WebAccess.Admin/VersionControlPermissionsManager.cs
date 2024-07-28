// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.VersionControlPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class VersionControlPermissionsManager : SecurityNamespacePermissionsManager
  {
    public VersionControlPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token)
      : base(requestContext, permissionsIdentifier, token)
    {
      if (!this.UserHasReadAccess)
        return;
      this.InheritPermissions = this.PermissionSets[SecurityConstants.RepositorySecurityNamespaceGuid].GetAccessControlList(requestContext).InheritPermissions;
    }

    public override bool CanEditAdminPermissions => true;

    public override bool CanTokenInheritPermissions => !VersionControlPermissionsManager.IsRootFolder(this.Token);

    public override void ChangeInheritance(
      IVssRequestContext requestContext,
      bool inheritPermissions)
    {
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[SecurityConstants.RepositorySecurityNamespaceGuid];
      this.ChangeInheritance(requestContext, permissionSet, inheritPermissions);
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int permissionsToDisplay = 15871;
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, SecurityConstants.RepositorySecurityNamespaceGuid, this.Token, permissionsToDisplay);
      permissionSets.Add(SecurityConstants.RepositorySecurityNamespaceGuid, namespacePermissionSet);
      return permissionSets;
    }

    protected override IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      if (VersionControlPermissionsManager.IsRootFolder(token))
        return (IAccessControlList) null;
      string folderName = VersionControlPath.GetFolderName(token);
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[SecurityConstants.RepositorySecurityNamespaceGuid];
      try
      {
        return permissionSet.SecuredSecurityNamespace.QueryAccessControlLists(requestContext, folderName, (IEnumerable<IdentityDescriptor>) descriptors, true, false).FirstOrDefault<IAccessControlList>();
      }
      catch (ItemNotFoundException ex)
      {
        return (IAccessControlList) null;
      }
    }

    private static bool IsRootFolder(string token) => string.Equals(token, "$/", StringComparison.OrdinalIgnoreCase);
  }
}
