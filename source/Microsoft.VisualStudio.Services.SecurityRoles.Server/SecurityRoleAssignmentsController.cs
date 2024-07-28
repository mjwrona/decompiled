// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.SecurityRoleAssignmentsController
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BBA245E2-CEA0-4262-9E17-EB6FDFC84F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SecurityRoles.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.SecurityRoles
{
  [ApplyRequestLanguage]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "securityroles", ResourceName = "roleassignments")]
  public class SecurityRoleAssignmentsController : TfsApiController
  {
    [HttpGet]
    [ClientLocationId("9461C234-C84C-4ED2-B918-2F0F92AD0A35")]
    public List<RoleAssignment> GetRoleAssignments(string scopeId, string resourceId) => this.TfsRequestContext.GetService<ISecurityRoleMappingService>().GetRoleAssignments(this.TfsRequestContext, resourceId, scopeId);

    [HttpPut]
    [ClientLocationId("9461C234-C84C-4ED2-B918-2F0F92AD0A35")]
    public RoleAssignment SetRoleAssignment(
      string scopeId,
      string resourceId,
      Guid identityId,
      UserRoleAssignmentRef roleAssignment)
    {
      ISecurityRoleMappingService service = this.TfsRequestContext.GetService<ISecurityRoleMappingService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<UserRoleAssignmentRef> userRoles = new List<UserRoleAssignmentRef>();
      userRoles.Add(roleAssignment);
      string resourceId1 = resourceId;
      string scopeId1 = scopeId;
      return service.SetRoleAssignments(tfsRequestContext, userRoles, resourceId1, scopeId1).FirstOrDefault<RoleAssignment>();
    }

    [HttpPut]
    [ClientExample("AddRoleAssignments.json", "Set Role assignments", null, null)]
    [ClientLocationId("9461C234-C84C-4ED2-B918-2F0F92AD0A35")]
    public List<RoleAssignment> SetRoleAssignments(
      string scopeId,
      string resourceId,
      List<UserRoleAssignmentRef> roleAssignments,
      bool limitToCallerIdentityDomain = false)
    {
      return this.TfsRequestContext.GetService<ISecurityRoleMappingService>().SetRoleAssignments(this.TfsRequestContext, roleAssignments, resourceId, scopeId, limitToCallerIdentityDomain);
    }

    [HttpDelete]
    [ClientLocationId("9461C234-C84C-4ED2-B918-2F0F92AD0A35")]
    public void RemoveRoleAssignment(string scopeId, string resourceId, Guid identityId)
    {
      ISecurityRoleMappingService service = this.TfsRequestContext.GetService<ISecurityRoleMappingService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<Guid> identityIds = new List<Guid>();
      identityIds.Add(identityId);
      string resourceId1 = resourceId;
      string scopeId1 = scopeId;
      service.RemoveRoleAssignments(tfsRequestContext, identityIds, resourceId1, scopeId1);
    }

    [HttpPatch]
    [ClientLocationId("9461C234-C84C-4ED2-B918-2F0F92AD0A35")]
    public void RemoveRoleAssignments(string scopeId, string resourceId, List<Guid> identityIds) => this.TfsRequestContext.GetService<ISecurityRoleMappingService>().RemoveRoleAssignments(this.TfsRequestContext, identityIds, resourceId, scopeId);

    [HttpPatch]
    [ClientLocationId("9461C234-C84C-4ED2-B918-2F0F92AD0A35")]
    public void ChangeInheritance(string scopeId, string resourceId, bool inheritPermissions) => this.TfsRequestContext.GetService<ISecurityRoleMappingService>().ChangeInheritance(this.TfsRequestContext, scopeId, resourceId, inheritPermissions);
  }
}
