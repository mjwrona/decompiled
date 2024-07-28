// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.FrameworkIdentitySecurityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class FrameworkIdentitySecurityService : IIdentitySecurityService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void CheckGroupPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity group,
      int permission)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ISecuredTeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId);
      string securityToken = IdentityHelper.CreateSecurityToken((IReadOnlyVssIdentity) group);
      IVssRequestContext requestContext1 = requestContext;
      string token = securityToken;
      int requestedPermissions = permission;
      securityNamespace.CheckPermission(requestContext1, token, requestedPermissions, false);
    }

    public bool HasTeamPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity teamIdentity,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      string teamSecurableToken = this.GetTeamSecurableToken(requestContext, teamIdentity);
      return requestContext.GetService<ISecuredTeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId).HasPermission(requestContext, teamSecurableToken, requestedPermissions, alwaysAllowAdministrators);
    }

    public string GetTeamSecurableToken(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity teamIdentity)
    {
      IdentityDomain domain = requestContext.GetService<IdentityService>().IdentityServiceInternal().Domain;
      Guid securingHostId = this.GetSecuringHostId(requestContext, teamIdentity, domain);
      if (requestContext.IsSystemContext || teamIdentity == null || !(securingHostId != requestContext.ServiceHost.InstanceId))
        return IdentityHelper.CreateSecurityToken((IReadOnlyVssIdentity) teamIdentity);
      if (IdentityValidation.IsTeamFoundationType(teamIdentity.Descriptor))
        throw new IdentityDomainMismatchException(domain.Name, string.Empty);
      throw new NotApplicationGroupException();
    }

    private Guid GetSecuringHostId(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity group,
      IdentityDomain identityDomain)
    {
      return !string.Equals(group.GetProperty<string>("ScopeType", "Generic"), "ServiceHost", StringComparison.OrdinalIgnoreCase) || identityDomain.IsOwner(group.Descriptor) ? requestContext.ServiceHost.InstanceId : Guid.Empty;
    }
  }
}
