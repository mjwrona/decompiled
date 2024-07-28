// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.RoleAssignmentSecurityContext
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BBA245E2-CEA0-4262-9E17-EB6FDFC84F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SecurityRoles.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.SecurityRoles
{
  internal class RoleAssignmentSecurityContext
  {
    public RoleAssignmentSecurityContext(
      IVssRequestContext requestContext,
      ISecurityRoleScope scope,
      string resourceId)
    {
      this.ScopeContext = RoleAssignmentSecurityContext.MapIncomingRequestContext(requestContext, scope);
      this.ScopeSecurityToken = scope.GetSecurityToken(this.ScopeContext, resourceId);
      this.ScopeSecurityNamespace = RoleAssignmentSecurityContext.GetSecurityNamespace(this.ScopeContext, scope);
      if (scope is IRoleAssignmentsSecurityInfoProvider securityInfoProvider)
      {
        this.SecurityNamespace = RoleAssignmentSecurityContext.GetSecurityNamespaceById(requestContext, securityInfoProvider.GetRoleAssignmentSecurityNamespace());
        this.SecurityToken = securityInfoProvider.GetRoleAssignmentSecurityToken(requestContext, resourceId);
      }
      else
      {
        this.SecurityNamespace = this.ScopeSecurityNamespace;
        this.SecurityToken = this.ScopeSecurityToken;
      }
    }

    public void CheckPermission(int permission) => this.ScopeSecurityNamespace.CheckPermission(this.ScopeContext, this.ScopeSecurityToken, permission);

    public void ChangeInheritance(bool inheritPermissions) => SecurityServiceHelpers.ChangeInheritance(this.ScopeContext, this.SecurityNamespace, this.ScopeSecurityToken, inheritPermissions);

    private static IVssRequestContext MapIncomingRequestContext(
      IVssRequestContext requestContext,
      ISecurityRoleScope scope)
    {
      return scope is ISecurityRoleScopeRequestContextMapper requestContextMapper ? requestContextMapper.GetRequestContext(requestContext) : requestContext;
    }

    private static IVssSecurityNamespace GetSecurityNamespace(
      IVssRequestContext requestContext,
      ISecurityRoleScope scope)
    {
      return RoleAssignmentSecurityContext.GetSecurityNamespaceById(requestContext, scope.GetSecurityNamespace());
    }

    private static IVssSecurityNamespace GetSecurityNamespaceById(
      IVssRequestContext requestContext,
      Guid securityNamespaceId)
    {
      return requestContext.GetService<ISecuredTeamFoundationSecurityService>().GetSecurityNamespace(requestContext, securityNamespaceId) ?? throw new InvalidSecurityNamespaceException(securityNamespaceId);
    }

    public IVssRequestContext ScopeContext { get; private set; }

    public IVssSecurityNamespace SecurityNamespace { get; private set; }

    public string SecurityToken { get; private set; }

    private IVssSecurityNamespace ScopeSecurityNamespace { get; set; }

    private string ScopeSecurityToken { get; set; }
  }
}
