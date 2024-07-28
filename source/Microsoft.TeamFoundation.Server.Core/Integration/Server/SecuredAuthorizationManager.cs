// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.SecuredAuthorizationManager
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class SecuredAuthorizationManager
  {
    private IntegrationSecurityManager m_securityManager;
    private IAuthorizationProviderFactory m_authorizationProvider;

    public SecuredAuthorizationManager(IVssRequestContext systemRequestContext)
    {
      this.m_authorizationProvider = systemRequestContext.GetExtension<IAuthorizationProviderFactory>();
      this.m_securityManager = systemRequestContext.GetService<IntegrationSecurityManager>();
    }

    internal void RegisterObject(
      IVssRequestContext requestContext,
      string objectId,
      string objectClassId,
      string projectUri,
      string parentId)
    {
      this.m_securityManager.CheckProjectPermission(requestContext, projectUri, "GENERIC_WRITE");
      this.m_authorizationProvider.RegisterObject(requestContext, requestContext.UserContext, objectId, objectClassId, projectUri, parentId);
    }

    internal void UnregisterObject(IVssRequestContext requestContext, string objectId)
    {
      this.m_securityManager.CheckObjectPermission(requestContext, objectId, "GENERIC_WRITE");
      this.m_authorizationProvider.UnregisterObject(requestContext, objectId);
    }

    internal void ResetInheritance(
      IVssRequestContext requestContext,
      string objectId,
      string parentObejctId)
    {
      this.m_securityManager.CheckObjectPermission(requestContext, objectId, "GENERIC_WRITE");
      this.m_authorizationProvider.ResetInheritance(requestContext, objectId, parentObejctId);
    }

    internal string GetObjectClass(IVssRequestContext requestContext, string objectId)
    {
      this.m_securityManager.CheckObjectPermission(requestContext, objectId, "GENERIC_READ");
      return this.m_authorizationProvider.GetObjectClass(requestContext, objectId);
    }

    internal string[] ListObjectClassActions(
      IVssRequestContext requestContext,
      string objectClassId)
    {
      this.m_securityManager.CheckGlobalPermission(requestContext, "GENERIC_READ");
      return this.m_authorizationProvider.ListObjectClassActions(requestContext, objectClassId);
    }

    internal string[] ListLocalizedActionNames(
      IVssRequestContext requestContext,
      string objectClassId,
      string[] actionId)
    {
      this.m_securityManager.CheckGlobalPermission(requestContext, "GENERIC_READ");
      return this.m_authorizationProvider.ListLocalizedActionNames(requestContext, objectClassId, actionId);
    }

    internal void ReplaceAccessControlList(
      IVssRequestContext requestContext,
      string objectId,
      Microsoft.Azure.Boards.CssNodes.AccessControlEntry[] aces)
    {
      this.m_securityManager.CheckObjectPermission(requestContext, objectId, "GENERIC_WRITE");
      this.m_authorizationProvider.ReplaceAccessControlList(requestContext, objectId, aces);
    }

    internal void AddAccessControlEntry(
      IVssRequestContext requestContext,
      string objectId,
      Microsoft.Azure.Boards.CssNodes.AccessControlEntry ace)
    {
      this.m_securityManager.CheckObjectPermission(requestContext, objectId, "GENERIC_WRITE");
      this.m_authorizationProvider.AddAccessControlEntry(requestContext, objectId, ace);
    }

    internal void RemoveAccessControlEntry(
      IVssRequestContext requestContext,
      string objectId,
      Microsoft.Azure.Boards.CssNodes.AccessControlEntry ace)
    {
      this.m_securityManager.CheckObjectPermission(requestContext, objectId, "GENERIC_WRITE");
      this.m_authorizationProvider.RemoveAccessControlEntry(requestContext, objectId, ace);
    }

    internal string GetChangedAccessControlEntries(
      IVssRequestContext requestContext,
      int sequence_id)
    {
      this.m_securityManager.CheckGlobalPermission(requestContext, "SYNCHRONIZE_READ");
      return this.m_authorizationProvider.GetChangedAccessControlEntries(requestContext, sequence_id);
    }

    internal bool IsPermitted(
      IVssRequestContext requestContext,
      string objectId,
      string actionId,
      IdentityDescriptor descriptor)
    {
      this.m_securityManager.CheckGlobalPermission(requestContext, "GENERIC_READ");
      return this.m_authorizationProvider.IsPermitted(requestContext, objectId, actionId, descriptor);
    }

    internal Microsoft.Azure.Boards.CssNodes.AccessControlEntry[] ReadAccessControlList(
      IVssRequestContext requestContext,
      string objectId)
    {
      this.m_securityManager.CheckObjectPermission(requestContext, objectId, "GENERIC_READ");
      return this.m_authorizationProvider.ReadAccessControlList(requestContext, objectId);
    }

    internal string[] ListObjectClasses(IVssRequestContext requestContext)
    {
      this.m_securityManager.CheckGlobalPermission(requestContext, "GENERIC_READ");
      return this.m_authorizationProvider.ListObjectClasses(requestContext);
    }
  }
}
