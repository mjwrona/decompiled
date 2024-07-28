// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureAvailabilitySecurityManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FeatureAvailabilitySecurityManager : IFeatureAvailabilitySecurityManager
  {
    public void CheckPermissions(
      IVssRequestContext requestContext,
      FeatureAvailabilityPermissions permissions,
      bool userScope)
    {
      if (requestContext.IsSystemContext || permissions == FeatureAvailabilityPermissions.ViewFeatureFlagByName)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssSecurityNamespace securityNamespace = vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext.Elevate(), FeatureAvailabilitySecurityConstants.NamespaceGuid);
      string token = "DeploymentScopedFeatureAvailabilityPrivileges";
      if (securityNamespace == null)
      {
        Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
        throw new AccessCheckException(authenticatedIdentity.Descriptor, authenticatedIdentity.DisplayName, token, (int) permissions, FeatureAvailabilitySecurityConstants.NamespaceGuid, FrameworkResources.AccessCheckNoNamespaceFound((object) FeatureAvailabilitySecurityConstants.NamespaceGuid));
      }
      if (securityNamespace.HasPermission(vssRequestContext, token, (int) permissions, false))
        return;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) | userScope)
      {
        token = "AccountScopedFeatureAvailabilityPrivileges";
        if (securityNamespace.HasPermission(vssRequestContext, token, (int) permissions, false))
          return;
      }
      securityNamespace.ThrowAccessDeniedException(vssRequestContext, token, (int) permissions);
    }
  }
}
