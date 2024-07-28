// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions.LibraryRolesScope
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F602B8A6-9B4B-4971-8764-E3FEAFAB8CD5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SecurityRoles;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions
{
  public class LibraryRolesScope : ISecurityRoleScope
  {
    protected const char NamespaceSeparatorToReplace = '$';
    private static readonly Lazy<List<SecurityRole>> s_SecurityRoles = new Lazy<List<SecurityRole>>(new Func<List<SecurityRole>>(LibraryRolesScope.LoadRoles));
    private static readonly string LibraryRoleScopeId = "distributedtask.library";

    public virtual string GetSecurityToken(IVssRequestContext requestContext, string resourceId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
      string[] strArray = resourceId.Split('$');
      if (strArray.Length == 2)
      {
        string str = strArray[0];
        ArgumentUtility.CheckForNull<string>(str, "projectId");
        Guid result;
        Guid.TryParse(str, out result);
        ArgumentUtility.CheckForEmptyGuid(result, "projectId");
        return LibrarySecurityProvider.Library + (object) LibrarySecurityProvider.NamespaceSeparator + str;
      }
      if (strArray.Length == 1)
        return LibrarySecurityProvider.Library + (object) LibrarySecurityProvider.NamespaceSeparator + LibrarySecurityProvider.Collection;
      throw new ArgumentException(nameof (resourceId));
    }

    public virtual List<SecurityRole> GetRoles() => LibraryRolesScope.s_SecurityRoles.Value;

    public virtual string GetScopeId() => LibraryRolesScope.LibraryRoleScopeId;

    public SecurityRole GetRole(int permissions) => this.GetRoles().FirstOrDefault<SecurityRole>((Func<SecurityRole, bool>) (role => (role.AllowPermissions & permissions) == role.AllowPermissions));

    public bool ShouldExcludeFromRoleAssignmentListing(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      return IdentityHelper.IsWellKnownGroup(identity.Descriptor, GroupWellKnownIdentityDescriptors.ServiceUsersGroup) || IdentityHelper.IsWellKnownGroup(identity.Descriptor, GroupWellKnownIdentityDescriptors.ServicePrincipalGroup);
    }

    public virtual int GetReadPermission() => 1;

    public int GetWritePermission() => 2;

    public Guid GetSecurityNamespace() => LibrarySecurityProvider.LibraryNamespaceId;

    public virtual void CompleteSetRoleAssignments(
      IVssRequestContext requestContext,
      List<RoleAssignment> userRoles,
      string resourceId)
    {
      requestContext.TraceInfo(34000214, "ServiceEndpoints", "Roles for {0} has been set", (object) resourceId);
      if (userRoles.Count<RoleAssignment>() <= 0)
        return;
      requestContext.GetExtension<IVariableGroupSecurityMigrationHelper>()?.PromoteAllProjectLevelAdminsToCollectionLevelAdminsForAllVariableGroup(requestContext, resourceId);
    }

    public virtual void CompleteRemoveRoleAssignments(
      IVssRequestContext requestContext,
      List<Guid> identityIds,
      string resourceId)
    {
      requestContext.TraceInfo(34000215, "ServiceEndpoints", "Roles for {0} has been removed", (object) resourceId);
      if (identityIds.Count<Guid>() <= 0)
        return;
      requestContext.GetExtension<IVariableGroupSecurityMigrationHelper>()?.PromoteAllProjectLevelAdminsToCollectionLevelAdminsForAllVariableGroup(requestContext, resourceId);
    }

    private static List<SecurityRole> LoadRoles() => new List<SecurityRole>()
    {
      new SecurityRole(ServiceEndpointSdkResources.AdministratorRole(), LibraryGroupRoleConstants.AdministratorRole, 19, 0, LibraryRolesScope.LibraryRoleScopeId, ServiceEndpointSdkResources.LibraryAdministratorRoleDescription()),
      new SecurityRole(ServiceEndpointSdkResources.CreatorRole(), LibraryGroupRoleConstants.CreatorRole, 5, 0, LibraryRolesScope.LibraryRoleScopeId, ServiceEndpointSdkResources.LibraryCreatorRoleDescription()),
      new SecurityRole(ServiceEndpointSdkResources.UserRole(), LibraryGroupRoleConstants.UserRole, 17, 0, LibraryRolesScope.LibraryRoleScopeId, ServiceEndpointSdkResources.LibraryUserRoleDescription()),
      new SecurityRole(ServiceEndpointSdkResources.ReaderRole(), LibraryGroupRoleConstants.ReaderRole, 1, 0, LibraryRolesScope.LibraryRoleScopeId, ServiceEndpointSdkResources.LibraryReaderRoleDescription())
    };
  }
}
