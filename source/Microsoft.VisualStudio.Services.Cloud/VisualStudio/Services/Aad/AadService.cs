// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Aad
{
  [DefaultServiceImplementation(typeof (SharedAadService))]
  public abstract class AadService : IVssFrameworkService
  {
    public abstract void ServiceStart(IVssRequestContext context);

    public abstract void ServiceEnd(IVssRequestContext context);

    public abstract bool IsRequestAllowed(IVssRequestContext context, AadServiceRequest request);

    public abstract IDictionary<Guid, bool> AreObjectsVirtuallyInScope(
      IVssRequestContext requestContext,
      AadObjectType objectType,
      IEnumerable<Guid> objectIds);

    public abstract GetAncestorIdsResponse<T> GetAncestorIds<T>(
      IVssRequestContext context,
      GetAncestorIdsRequest<T> request);

    public abstract GetAncestorsResponse GetAncestors<T>(
      IVssRequestContext context,
      GetAncestorsRequest<T> request);

    public abstract GetDescendantIdsResponse GetDescendantIds<T>(
      IVssRequestContext context,
      GetDescendantIdsRequest<T> request);

    public abstract GetDescendantsResponse GetDescendants<T>(
      IVssRequestContext context,
      GetDescendantsRequest<T> request);

    public abstract GetDirectoryRolesResponse GetDirectoryRoles(
      IVssRequestContext context,
      GetDirectoryRolesRequest request);

    public abstract GetDirectoryRoleMembersResponse GetDirectoryRoleMembers(
      IVssRequestContext context,
      GetDirectoryRoleMembersRequest request);

    public abstract GetDirectoryRolesWithIdsResponse GetDirectoryRolesWithIds(
      IVssRequestContext context,
      GetDirectoryRolesWithIdsRequest request);

    public abstract GetGroupsResponse GetGroups(
      IVssRequestContext context,
      GetGroupsRequest request);

    public abstract GetGroupsWithIdsResponse<T> GetGroupsWithIds<T>(
      IVssRequestContext context,
      GetGroupsWithIdsRequest<T> request);

    public abstract GetSoftDeletedObjectsResponse<TIdentifier, TType> GetSoftDeletedObjects<TIdentifier, TType>(
      IVssRequestContext context,
      GetSoftDeletedObjectsRequest<TIdentifier, TType> request)
      where TType : AadObject;

    public abstract GetTenantResponse GetTenant(
      IVssRequestContext context,
      GetTenantRequest request);

    public abstract GetTenantsResponse GetTenants(
      IVssRequestContext context,
      GetTenantsRequest request);

    public abstract GetUsersResponse GetUsers(IVssRequestContext context, GetUsersRequest request);

    public abstract GetUsersWithIdsResponse<T> GetUsersWithIds<T>(
      IVssRequestContext context,
      GetUsersWithIdsRequest<T> request);

    public abstract GetUserStatusWithIdResponse GetUserStatusWithId(
      IVssRequestContext context,
      GetUserStatusWithIdRequest request);

    public abstract GetUserThumbnailResponse GetUserThumbnail<T>(
      IVssRequestContext context,
      GetUserThumbnailRequest<T> request);

    public abstract GetUserRolesAndGroupsResponse GetUserRolesAndGroups(
      IVssRequestContext context,
      GetUserRolesAndGroupsRequest request);

    public abstract GetServicePrincipalsByIdsResponse GetServicePrincipalsByIds(
      IVssRequestContext context,
      GetServicePrincipalsByIdsRequest request);

    public abstract GetServicePrincipalsResponse GetServicePrincipals(
      IVssRequestContext context,
      GetServicePrincipalsRequest request);

    public abstract GetApplicationByIdResponse GetApplicationById(
      IVssRequestContext context,
      GetApplicationByIdRequest request);

    public abstract CreateApplicationResponse CreateApplication(
      IVssRequestContext context,
      CreateApplicationRequest request);

    public abstract UpdateApplicationResponse UpdateApplication(
      IVssRequestContext context,
      UpdateApplicationRequest request);

    public abstract DeleteApplicationResponse DeleteApplication(
      IVssRequestContext context,
      DeleteApplicationRequest request);

    public abstract CreateServicePrincipalResponse CreateServicePrincipal(
      IVssRequestContext context,
      CreateServicePrincipalRequest request);

    public abstract DeleteServicePrincipalResponse DeleteServicePrincipal(
      IVssRequestContext context,
      DeleteServicePrincipalRequest request);

    public abstract AddApplicationPasswordResponse AddApplicationPassword(
      IVssRequestContext context,
      AddApplicationPasswordRequest request);

    public abstract AddApplicationFederatedCredentialsResponse AddApplicationFederatedCredentials(
      IVssRequestContext context,
      AddApplicationFederatedCredentialsRequest request);

    public abstract RemoveApplicationFederatedCredentialsResponse RemoveApplicationFederatedCredentials(
      IVssRequestContext context,
      RemoveApplicationFederatedCredentialsRequest request);

    public abstract RemoveApplicationPasswordResponse RemoveApplicationPassword(
      IVssRequestContext context,
      RemoveApplicationPasswordRequest request);

    public abstract GetOrganizationDataBoundaryResponse GetOrganizationDataBoundary(
      IVssRequestContext context,
      GetOrganizationDataBoundaryRequest request);

    public abstract GetProfileDataResponse GetProfileData(
      IVssRequestContext context,
      GetProfileDataRequest request);

    internal abstract InvalidateStaleAncestorIdsCacheResponse InvalidateStaleAncestorIdsCache<T>(
      IVssRequestContext context,
      InvalidateStaleAncestorIdsCacheRequest<T> request);
  }
}
