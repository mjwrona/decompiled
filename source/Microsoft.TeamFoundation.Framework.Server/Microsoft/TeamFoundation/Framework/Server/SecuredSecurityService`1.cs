// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuredSecurityService`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class SecuredSecurityService<T> : 
    ITeamFoundationSecurityService,
    IVssFrameworkService
    where T : class, ITeamFoundationSecurityService
  {
    private ITeamFoundationSecurityService m_securityService;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.m_securityService = (ITeamFoundationSecurityService) systemRequestContext.GetService<T>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IVssSecurityNamespace CreateSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description)
    {
      ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(description, nameof (description));
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
      return this.m_securityService.CreateSecurityNamespace(requestContext, description);
    }

    public IVssSecurityNamespace UpdateSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description)
    {
      ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(description, nameof (description));
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
      IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(requestContext, description.NamespaceId);
      if (securityNamespace == null)
        throw new InvalidSecurityNamespaceException(description.NamespaceId);
      if (!securityNamespace.NamespaceExtension.AlwaysAllowAdministrators)
        securityNamespace.ThrowAccessDeniedException(requestContext, securityNamespace.Description.Name, securityNamespace.Description.WritePermission);
      return this.m_securityService.UpdateSecurityNamespace(requestContext, description);
    }

    public bool DeleteSecurityNamespace(IVssRequestContext requestContext, Guid namespaceId)
    {
      ArgumentUtility.CheckForEmptyGuid(namespaceId, nameof (namespaceId));
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
      IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(requestContext, namespaceId);
      if (securityNamespace != null && !securityNamespace.NamespaceExtension.AlwaysAllowAdministrators)
        securityNamespace.ThrowAccessDeniedException(requestContext, securityNamespace.Description.Name, securityNamespace.Description.WritePermission);
      return this.m_securityService.DeleteSecurityNamespace(requestContext, namespaceId);
    }

    public IVssSecurityNamespace GetSecurityNamespace(
      IVssRequestContext requestContext,
      Guid namespaceId)
    {
      ArgumentUtility.CheckForEmptyGuid(namespaceId, nameof (namespaceId));
      IVssSecurityNamespace securityNamespace1 = this.m_securityService.GetSecurityNamespace(requestContext, SecuritySecurityConstants.NamespaceId);
      if (securityNamespace1 != null)
        securityNamespace1.CheckPermission(requestContext, "", 1);
      else
        this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);
      IVssSecurityNamespace securityNamespace2 = this.m_securityService.GetSecurityNamespace(requestContext, namespaceId);
      if (securityNamespace2 != null)
        securityNamespace2 = securityNamespace2.Secured();
      return securityNamespace2;
    }

    public IList<IVssSecurityNamespace> GetSecurityNamespaces(IVssRequestContext requestContext)
    {
      IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(requestContext, SecuritySecurityConstants.NamespaceId);
      if (securityNamespace != null)
        securityNamespace.CheckPermission(requestContext, "", 1);
      else
        this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);
      return (IList<IVssSecurityNamespace>) new List<IVssSecurityNamespace>(this.m_securityService.GetSecurityNamespaces(requestContext).Select<IVssSecurityNamespace, IVssSecurityNamespace>((Func<IVssSecurityNamespace, IVssSecurityNamespace>) (s => s.Secured())));
    }

    public void RemoveIdentityACEs(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
      this.m_securityService.RemoveIdentityACEs(requestContext, identities);
    }
  }
}
