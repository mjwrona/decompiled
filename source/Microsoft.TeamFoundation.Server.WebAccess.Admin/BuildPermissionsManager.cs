// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.BuildPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class BuildPermissionsManager : SecurityNamespacePermissionsManager
  {
    public BuildPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token,
      string projectName)
      : base(requestContext, permissionsIdentifier, token)
    {
      if (this.UserHasReadAccess)
        this.InheritPermissions = this.PermissionSets[BuildSecurity.BuildNamespaceId].GetAccessControlList(requestContext).InheritPermissions;
      this.ProjectName = projectName;
    }

    public override bool CanEditAdminPermissions => true;

    public override bool CanTokenInheritPermissions => this.Token.Contains(BuildSecurity.NamespaceSeparator.ToString());

    private string ProjectName { get; set; }

    public override void ChangeInheritance(
      IVssRequestContext requestContext,
      bool inheritPermissions)
    {
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[BuildSecurity.BuildNamespaceId];
      this.ChangeInheritance(requestContext, permissionSet, inheritPermissions);
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int allPermissions = BuildPermissions.AllPermissions;
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, BuildSecurity.BuildNamespaceId, this.Token, allPermissions);
      permissionSets.Add(BuildSecurity.BuildNamespaceId, namespacePermissionSet);
      return permissionSets;
    }

    protected override IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      string parentToken = BuildPermissionsManager.GetParentToken(token);
      return string.IsNullOrEmpty(parentToken) ? (IAccessControlList) null : this.PermissionSets[BuildSecurity.BuildNamespaceId].SecuredSecurityNamespace.QueryAccessControlLists(requestContext, parentToken, (IEnumerable<IdentityDescriptor>) descriptors, true, false).FirstOrDefault<IAccessControlList>();
    }

    protected override string GetTokenDisplayName(IVssRequestContext requestContext, string token)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      return !token.Contains(BuildSecurity.NamespaceSeparator.ToString()) ? this.ProjectName : base.GetTokenDisplayName(requestContext, token);
    }

    private static string GetParentToken(string token)
    {
      int length = token.LastIndexOf(BuildSecurity.NamespaceSeparator);
      return length < 0 ? (string) null : token.Substring(0, length);
    }
  }
}
