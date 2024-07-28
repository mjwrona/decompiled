// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.IntegrationSecurityManager
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Boards.CssNodes
{
  public class IntegrationSecurityManager : IIntegrationSecurityManager, IVssFrameworkService
  {
    private IAuthorizationProviderFactory m_authorizationProvider;
    private static readonly string s_localMachineName = Aux.NormalizeString(Environment.MachineName, false);
    private const uint IdentifierAuthorityNullAuthority = 0;
    private const uint IdentifierAuthorityWorldAuthority = 1;
    private const uint IdentifierAuthorityLocalAuthority = 2;
    private const uint IdentifierAuthorityCreatorAuthority = 3;
    private const uint IdentifierAuthorityNonUniqueAuthority = 4;
    private const uint IdentifierAuthorityNTAuthority = 5;
    private const uint IdentifierAuthoritySiteServerAuthority = 6;
    private const uint IdentifierAuthorityInternetSiteAuthority = 7;
    private const uint IdentifierAuthorityExchangeAuthority = 8;
    private const uint IdentifierAuthorityResourceManagerAuthority = 9;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_authorizationProvider = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetExtension<IAuthorizationProviderFactory>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal bool HasObjectPermission(
      IVssRequestContext requestContext,
      string objectId,
      string actionId)
    {
      return requestContext.IsSystemContext || this.m_authorizationProvider.IsPermitted(requestContext, objectId, actionId, requestContext.UserContext);
    }

    public void CheckObjectPermission(
      IVssRequestContext requestContext,
      string objectId,
      string actionId)
    {
      if (requestContext.IsSystemContext)
        return;
      requestContext.GetExtension<IAuthorizationProviderFactory>().EnsurePermitted(requestContext, objectId, actionId);
    }

    internal bool HasGlobalPermission(IVssRequestContext requestContext, string actionId) => requestContext.IsSystemContext || this.HasObjectPermission(requestContext, PermissionNamespaces.Global, actionId);

    internal void CheckGlobalPermission(IVssRequestContext requestContext, string actionId) => this.CheckObjectPermission(requestContext, PermissionNamespaces.Global, actionId);

    internal bool HasProjectPermission(
      IVssRequestContext requestContext,
      string projectUri,
      string actionId)
    {
      return string.IsNullOrEmpty(projectUri) ? this.HasGlobalPermission(requestContext, actionId) : this.HasObjectPermission(requestContext, PermissionNamespaces.Project + projectUri, actionId);
    }

    internal void CheckProjectPermission(
      IVssRequestContext requestContext,
      string projectUri,
      string actionId)
    {
      if (string.IsNullOrEmpty(projectUri))
        this.CheckGlobalPermission(requestContext, actionId);
      else
        this.CheckObjectPermission(requestContext, PermissionNamespaces.Project + projectUri, actionId);
    }

    internal static int CompareSid(string sid1, string sid2) => VssStringComparer.SID.Compare(sid1, sid2);

    internal static string GetProjectObjectId(string projectUri) => PermissionNamespaces.Project + projectUri;
  }
}
