// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourcePermissionsControllerBase`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class ResourcePermissionsControllerBase<TResourceIdentifier> : TfsApiController
  {
    public override string TraceArea => "SecurityService";

    public override string ActivityLogArea => "Security";

    [ClientIgnore]
    public IQueryable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> SetPermissions(
      TResourceIdentifier identifier,
      IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> accessControlEntries,
      bool merge)
    {
      string securityToken = this.CreateSecurityToken(identifier);
      Guid securityNamespace1 = this.GetSecurityNamespace();
      IVssSecurityNamespace securityNamespace2 = this.TfsRequestContext.GetService<ISecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespace1);
      if (securityNamespace2 == null)
        throw new InvalidSecurityNamespaceException(securityNamespace1);
      IEnumerable<AccessControlEntry> accessControlEntries1 = SecurityConverter.Convert(accessControlEntries);
      return SecurityConverter.Convert(securityNamespace2.SetAccessControlEntries(this.TfsRequestContext, securityToken, (IEnumerable<IAccessControlEntry>) accessControlEntries1, merge)).AsQueryable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>();
    }

    [ClientIgnore]
    public IQueryable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> QueryPermissions(
      TResourceIdentifier identifier,
      IEnumerable<IdentityDescriptor> identities)
    {
      return this.QueryPermissions(identifier, identities, true);
    }

    [ClientIgnore]
    public IQueryable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> QueryPermissions(
      TResourceIdentifier identifier,
      IEnumerable<IdentityDescriptor> identities,
      bool includeExtendedInfo)
    {
      string securityToken = this.CreateSecurityToken(identifier);
      Guid securityNamespace = this.GetSecurityNamespace();
      return SecurityConverter.Convert((this.TfsRequestContext.GetService<ISecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespace) ?? throw new InvalidSecurityNamespaceException(securityNamespace)).QueryAccessControlList(this.TfsRequestContext, securityToken, identities, includeExtendedInfo).AccessControlEntries).AsQueryable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>();
    }

    [ClientIgnore]
    public bool RemovePermissions(
      TResourceIdentifier identifier,
      IEnumerable<IdentityDescriptor> identities)
    {
      string securityToken = this.CreateSecurityToken(identifier);
      Guid securityNamespace = this.GetSecurityNamespace();
      return (this.TfsRequestContext.GetService<ISecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespace) ?? throw new InvalidSecurityNamespaceException(securityNamespace)).RemoveAccessControlEntries(this.TfsRequestContext, securityToken, identities);
    }

    protected abstract string CreateSecurityToken(TResourceIdentifier identifier);

    protected abstract Guid GetSecurityNamespace();
  }
}
