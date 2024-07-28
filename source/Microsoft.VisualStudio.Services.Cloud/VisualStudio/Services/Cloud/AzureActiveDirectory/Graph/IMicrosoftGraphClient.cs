// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.IMicrosoftGraphClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraph;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public interface IMicrosoftGraphClient
  {
    GetUserStatusWithIdResponse GetUserStatusWithId(
      IVssRequestContext context,
      GetUserStatusWithIdRequest request);

    Microsoft.VisualStudio.Services.Aad.Graph.GetServicePrincipalsByIdsResponse GetServicePrincipalsByIds(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Aad.Graph.GetServicePrincipalsByIdsRequest request);

    MsGraphGetServicePrincipalsResponse GetServicePrincipals(
      IVssRequestContext context,
      MsGraphGetServicePrincipalsRequest request);

    MsGraphGetUsersResponse GetUsers(IVssRequestContext context, MsGraphGetUsersRequest request);

    MsGraphGetGroupsResponse GetGroups(IVssRequestContext context, MsGraphGetGroupsRequest request);

    MsGraphGetUsersWithIdsResponse GetUsersWithIds(
      IVssRequestContext context,
      MsGraphGetUsersWithIdsRequest request);

    MsGraphGetUserRolesAndGroupsResponse GetUserRolesAndGroups(
      IVssRequestContext context,
      MsGraphGetUserRolesAndGroupsRequest request);

    MsGraphGetDirectoryRoleMembersResponse GetDirectoryRoleMembers(
      IVssRequestContext context,
      MsGraphGetDirectoryRoleMembersRequest request);

    MsGraphGetDirectoryRolesWithIdsResponse GetDirectoryRolesWithIds(
      IVssRequestContext context,
      MsGraphGetDirectoryRolesWithIdsRequest request);

    MsGraphGetDirectoryRolesResponse GetDirectoryRoles(
      IVssRequestContext context,
      MsGraphGetDirectoryRolesRequest request);

    MsGraphGetTenantReponse GetTenant(IVssRequestContext context, MsGraphGetTenantRequest request);

    MsGraphGetUserThumbnailReponse GetUserThumbnail(
      IVssRequestContext context,
      MsGraphGetUserThumbnailRequest request);

    MsGraphGetDescendantIdsResponse GetDescendantIds(
      IVssRequestContext context,
      MsGraphGetDescendantIdsRequest request);

    MsGraphGetDescendantsResponse GetDescendants(
      IVssRequestContext context,
      MsGraphGetDescendantsRequest request);

    MsGraphGetGroupsWithIdsResponse GetGroupsWithIds(
      IVssRequestContext context,
      MsGraphGetGroupsWithIdsRequest request);

    MsGraphGetSoftDeletedObjectsReponse<T> GetSoftDeletedObjects<T>(
      IVssRequestContext context,
      MsGraphGetSoftDeletedObjectsRequest<T> request)
      where T : AadObject;

    MsGraphGetAncestorIdsResponse GetAncestorIds(
      IVssRequestContext context,
      MsGraphGetAncestorIdsRequest request);

    MsGraphGetApplicationByIdResponse GetApplicationById(
      IVssRequestContext context,
      MsGraphGetApplicationByIdRequest request);

    MsGraphCreateApplicationResponse CreateApplication(
      IVssRequestContext context,
      MsGraphCreateApplicationRequest request);

    MsGraphUpdateApplicationResponse UpdateApplication(
      IVssRequestContext context,
      MsGraphUpdateApplicationRequest request);

    MsGraphDeleteApplicationResponse DeleteApplication(
      IVssRequestContext context,
      MsGraphDeleteApplicationRequest request);

    MsGraphCreateServicePrincipalResponse CreateServicePrincipal(
      IVssRequestContext context,
      MsGraphCreateServicePrincipalRequest request);

    MsGraphDeleteServicePrincipalResponse DeleteServicePrincipal(
      IVssRequestContext context,
      MsGraphDeleteServicePrincipalRequest request);

    MsGraphAddApplicationPasswordResponse AddApplicationPassword(
      IVssRequestContext context,
      MsGraphAddApplicationPasswordRequest request);

    MsGraphRemoveApplicationPasswordResponse RemoveApplicationPassword(
      IVssRequestContext context,
      MsGraphRemoveApplicationPasswordRequest request);

    MsGraphAddApplicationFederatedCredentialsResponse AddApplicationFederatedCredentials(
      IVssRequestContext context,
      MsGraphAddApplicationFederatedCredentialsRequest request);

    MsGraphRemoveApplicationFederatedCredentialsResponse RemoveApplicationFederatedCredentials(
      IVssRequestContext context,
      MsGraphRemoveApplicationFederatedCredentialsRequest request);

    MsGraphGetResourceTenantsResponse GetResourceTenants(
      IVssRequestContext context,
      MsGraphGetResourceTenantsRequest request);

    MsGraphGetOrganizationDataBoundaryResponse GetOrganizationDataBoundary(
      IVssRequestContext context,
      MsGraphGetOrganizationDataBoundaryRequest request);

    MsGraphGetProfileDataResponse GetProfileData(
      IVssRequestContext context,
      MsGraphGetProfileDataRequest request);
  }
}
