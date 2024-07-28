// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.IAadGraphClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public interface IAadGraphClient
  {
    GetAncestorIdsResponse GetAncestorIds(IVssRequestContext context, GetAncestorIdsRequest request);

    GetAncestorsResponse GetAncestors(IVssRequestContext context, GetAncestorsRequest request);

    GetDescendantsResponse GetDescendants(IVssRequestContext context, GetDescendantsRequest request);

    GetDescendantIdsResponse GetDescendantIds(
      IVssRequestContext context,
      GetDescendantIdsRequest request);

    GetDirectoryRolesResponse GetDirectoryRoles(
      IVssRequestContext context,
      GetDirectoryRolesRequest request);

    GetDirectoryRoleMembersResponse GetDirectoryRoleMembers(
      IVssRequestContext context,
      GetDirectoryRoleMembersRequest request);

    GetDirectoryRolesWithIdsResponse GetDirectoryRolesWithIds(
      IVssRequestContext context,
      GetDirectoryRolesWithIdsRequest request);

    GetGroupsResponse GetGroups(IVssRequestContext context, GetGroupsRequest request);

    GetGroupsWithIdsResponse GetGroupsWithIds(
      IVssRequestContext context,
      GetGroupsWithIdsRequest request);

    GetSoftDeletedObjectsResponse<T> GetSoftDeletedObjectsWithIds<T>(
      IVssRequestContext context,
      GetSoftDeletedObjectsRequest<T> request)
      where T : AadObject;

    GetTenantResponse GetTenant(IVssRequestContext context, GetTenantRequest request);

    GetTenantsByAltSecIdResponse GetTenantsByAltSecId(
      IVssRequestContext context,
      GetTenantsByAltSecIdRequest request);

    GetTenantsByKeyResponse GetTenantsByKey(
      IVssRequestContext context,
      GetTenantsByKeyRequest request);

    GetUsersResponse GetUsers(IVssRequestContext context, GetUsersRequest request);

    GetUsersWithIdsResponse GetUsersWithIds(
      IVssRequestContext context,
      GetUsersWithIdsRequest request);

    GetUserThumbnailResponse GetUserThumbnail(
      IVssRequestContext context,
      GetUserThumbnailRequest request);

    GetUserRolesAndGroupsResponse GetUserRolesAndGroups(
      IVssRequestContext context,
      GetUserRolesAndGroupsRequest request);
  }
}
