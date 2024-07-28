// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.SecurityUtil
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class SecurityUtil
  {
    private static readonly Guid SpsInstanceType = new Guid("00000001-0000-8888-8000-000000000000");
    private static readonly Guid LicensingInstanceType = new Guid("00000043-0000-8888-8000-000000000000");
    private const string ModifyAccountEntitlementsToken = "ModifyAccountEntitlements";

    public static void CheckPermission(
      IVssRequestContext requestContext,
      int requestedPermissions,
      string token)
    {
      IVssRequestContext permissionRequestContext = requestContext.ToLicensingPermissionRequestContext();
      IVssSecurityNamespace securityNamespace = permissionRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(permissionRequestContext, LicensingSecurity.NamespaceId);
      if (securityNamespace.HasPermission(permissionRequestContext, token, requestedPermissions, false) || SecurityUtil.IsImpersonatedCallFromAuthorizedServicePrincipal(permissionRequestContext))
        return;
      securityNamespace.ThrowAccessDeniedException(permissionRequestContext, token, requestedPermissions);
    }

    private static bool IsImpersonatedCallFromAuthorizedServicePrincipal(
      IVssRequestContext requestContext)
    {
      if (!requestContext.IsImpersonating)
        return false;
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return (2 & requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).QueryEffectivePermissions(requestContext, "ModifyAccountEntitlements", (EvaluationPrincipal) requestContext.GetAuthenticatedDescriptor())) != 0;
    }

    public static void CheckMigratePermission(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (!ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) && (userIdentity.Id != SecurityUtil.SpsInstanceType || userIdentity.Id != SecurityUtil.LicensingInstanceType))
        throw new UnauthorizedRequestException();
    }

    public static void CheckMitigationPermision(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (!ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) || userIdentity.Id != SecurityUtil.LicensingInstanceType)
        throw new UnauthorizedRequestException();
    }
  }
}
