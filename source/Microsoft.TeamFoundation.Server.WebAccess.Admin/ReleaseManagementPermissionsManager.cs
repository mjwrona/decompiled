// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ReleaseManagementPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ReleaseManagement.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class ReleaseManagementPermissionsManager : SecurityNamespacePermissionsManager
  {
    public ReleaseManagementPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token,
      string projectName)
      : base(requestContext, permissionsIdentifier, token)
    {
      if (this.UserHasReadAccess)
        this.InheritPermissions = this.PermissionSets[ReleaseManagementSecurity.SecurityNamespaceId].GetAccessControlList(requestContext).InheritPermissions;
      this.ProjectName = projectName;
    }

    public override bool CanEditAdminPermissions => true;

    public override bool CanTokenInheritPermissions => this.Token.Contains(ReleaseManagementSecurity.NamespaceSeparator.ToString());

    private bool IsTokenForEnvironmentDefinition => Regex.Match(this.Token, "\\/\\d+\\/Environment\\/\\d+$").Success;

    private string ProjectName { get; set; }

    public override void ChangeInheritance(
      IVssRequestContext requestContext,
      bool inheritPermissions)
    {
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[ReleaseManagementSecurity.SecurityNamespaceId];
      this.ChangeInheritance(requestContext, permissionSet, inheritPermissions);
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int permissionsToDisplay = ReleaseManagementPermissions.AllPermissions;
      if (this.IsTokenForEnvironmentDefinition)
        permissionsToDisplay = ReleaseManagementPermissions.EnvironmentDefinitionPermissions;
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, ReleaseManagementSecurity.SecurityNamespaceId, this.Token, permissionsToDisplay);
      permissionSets.Add(ReleaseManagementSecurity.SecurityNamespaceId, namespacePermissionSet);
      return permissionSets;
    }

    protected override IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      string parentToken = ReleaseManagementPermissionsManager.GetParentToken(token);
      return string.IsNullOrEmpty(parentToken) ? (IAccessControlList) null : this.PermissionSets[ReleaseManagementSecurity.SecurityNamespaceId].SecuredSecurityNamespace.QueryAccessControlLists(requestContext, parentToken, (IEnumerable<IdentityDescriptor>) descriptors, true, false).FirstOrDefault<IAccessControlList>();
    }

    protected override string GetTokenDisplayName(IVssRequestContext requestContext, string token)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      return !token.Contains(ReleaseManagementSecurity.NamespaceSeparator.ToString()) ? this.ProjectName : base.GetTokenDisplayName(requestContext, token);
    }

    private static string GetParentToken(string token)
    {
      int length = token.LastIndexOf(ReleaseManagementSecurity.NamespaceSeparator);
      return length < 0 ? (string) null : token.Substring(0, length);
    }
  }
}
