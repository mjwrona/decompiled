// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.CssPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal abstract class CssPermissionsManager : SecurityNamespacePermissionsManager
  {
    private bool m_canTokenInheritPermissions;

    public CssPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token)
      : base(requestContext, permissionsIdentifier, token)
    {
      this.InitializeInheritance(requestContext, token);
    }

    protected abstract Guid NamespaceId { get; }

    private void InitializeInheritance(IVssRequestContext requestContext, string token)
    {
      try
      {
        this.InheritPermissions = this.PermissionSets[this.NamespaceId].GetAccessControlList(requestContext).InheritPermissions;
        CommonStructureNodeInfo node = requestContext.GetService<CommonStructureService>().GetNode(requestContext.Elevate(), token);
        if (node == null)
          return;
        this.m_canTokenInheritPermissions = !string.IsNullOrEmpty(node.ParentUri);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(599999, "AdminSecurity", TfsTraceLayers.BusinessLogic, ex);
      }
    }

    public override bool CanTokenInheritPermissions => this.m_canTokenInheritPermissions;

    public override void ChangeInheritance(
      IVssRequestContext requestContext,
      bool inheritPermissions)
    {
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[this.NamespaceId];
      this.ChangeInheritance(requestContext, permissionSet, inheritPermissions);
    }

    protected IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      Guid namespaceGuid,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      string objectId = requestContext.GetExtension<IAuthorizationProviderFactory>().GetObjectId(requestContext, namespaceGuid, token);
      CommonStructureNodeInfo node = requestContext.GetService<CommonStructureService>().GetNode(requestContext.Elevate(), objectId);
      if (node == null || string.IsNullOrEmpty(node.ParentUri))
        return (IAccessControlList) null;
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[namespaceGuid];
      string token1 = permissionSet.HandleIncomingToken(requestContext, node.ParentUri);
      return permissionSet.SecuredSecurityNamespace.QueryAccessControlLists(requestContext, token1, (IEnumerable<IdentityDescriptor>) descriptors, true, false).FirstOrDefault<IAccessControlList>();
    }

    protected string GetTokenDisplayName(
      IVssRequestContext requestContext,
      Guid namespaceGuid,
      string token)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      string objectId = requestContext.GetExtension<IAuthorizationProviderFactory>().GetObjectId(requestContext, namespaceGuid, token);
      CommonStructureNodeInfo node = requestContext.GetService<CommonStructureService>().GetNode(requestContext.Elevate(), objectId);
      return node != null ? node.Name : this.GetTokenDisplayName(requestContext, token);
    }
  }
}
