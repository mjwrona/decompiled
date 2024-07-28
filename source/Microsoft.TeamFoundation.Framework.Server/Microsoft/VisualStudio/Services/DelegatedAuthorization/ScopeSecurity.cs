// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.ScopeSecurity
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public static class ScopeSecurity
  {
    public const string ScopesSecurityNamespaceToken = "/Scopes";
    public const string TokenScopesSecurityNamespaceToken = "/Scopes/Token";
    public const string TokenAdminScopesSecurityNamespaceToken = "/Scopes/TokenAdmin";

    public static bool HasPermission(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string token,
      int requestedPermission,
      bool alwaysAllowAdministrators = true)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, namespaceId).HasPermission(vssRequestContext, token, requestedPermission, alwaysAllowAdministrators);
    }

    public static void CheckPermission(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string token,
      int requestedPermission,
      bool alwaysAllowAdministrators = true)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, namespaceId).CheckPermission(vssRequestContext, token, requestedPermission, alwaysAllowAdministrators);
    }
  }
}
