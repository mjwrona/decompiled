// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AreaPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class AreaPermissionsManager : CssPermissionsManager
  {
    public AreaPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token)
      : base(requestContext, permissionsIdentifier, token)
    {
    }

    protected override Guid NamespaceId => AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid;

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid, this.Token, SecurityNamespacePermissionsManager.AllPermissions);
      permissionSets.Add(AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid, namespacePermissionSet);
      return permissionSets;
    }

    protected override IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      return this.GetParentAccessControlList(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid, token, descriptors);
    }

    protected override string GetTokenDisplayName(IVssRequestContext requestContext, string token) => this.GetTokenDisplayName(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid, token);
  }
}
