// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.ISecurityRoleScope
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BBA245E2-CEA0-4262-9E17-EB6FDFC84F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SecurityRoles.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.SecurityRoles
{
  [InheritedExport]
  public interface ISecurityRoleScope
  {
    string GetScopeId();

    string GetSecurityToken(IVssRequestContext requestContext, string resourceId);

    List<SecurityRole> GetRoles();

    SecurityRole GetRole(int permissions);

    bool ShouldExcludeFromRoleAssignmentListing(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    int GetReadPermission();

    int GetWritePermission();

    Guid GetSecurityNamespace();

    void CompleteSetRoleAssignments(
      IVssRequestContext requestContext,
      List<RoleAssignment> userRoles,
      string resourceId);

    void CompleteRemoveRoleAssignments(
      IVssRequestContext requestContext,
      List<Guid> identityIds,
      string resourceId);
  }
}
