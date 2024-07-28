// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.ISecurityRoleMappingService
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BBA245E2-CEA0-4262-9E17-EB6FDFC84F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SecurityRoles.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.SecurityRoles
{
  [DefaultServiceImplementation(typeof (SecurityRoleMappingService))]
  public interface ISecurityRoleMappingService : IVssFrameworkService
  {
    List<SecurityRole> GetRoles(IVssRequestContext requestContext, string scopeId);

    List<RoleAssignment> GetRoleAssignments(
      IVssRequestContext requestContext,
      string resourceId,
      string scopeId);

    List<RoleAssignment> SetRoleAssignments(
      IVssRequestContext requestContext,
      List<UserRoleAssignmentRef> userRoles,
      string resourceId,
      string scopeId,
      bool limitToCallerIdentityDomain = false);

    bool RemoveRoleAssignments(
      IVssRequestContext requestContext,
      List<Guid> identityIds,
      string resourceId,
      string scopeId);

    void ChangeInheritance(
      IVssRequestContext requestContext,
      string scopeId,
      string resourceId,
      bool inheritPermissions);
  }
}
