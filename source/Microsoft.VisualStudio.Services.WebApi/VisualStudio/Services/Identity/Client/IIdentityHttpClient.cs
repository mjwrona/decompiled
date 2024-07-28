// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Client.IIdentityHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Identity.Client
{
  [ResourceArea("8A3D49B8-91F0-46EF-B33D-DDA338C25DB3")]
  [VssClientServiceImplementation(typeof (IdentityHttpClient))]
  public interface IIdentityHttpClient : IVssHttpClient, IDisposable
  {
    Task<bool> AddMemberToGroupAsync(
      IdentityDescriptor containerId,
      Guid memberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<bool> AddMemberToGroupAsync(
      IdentityDescriptor containerId,
      IdentityDescriptor memberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<bool> AddMemberToGroupAsyncInternal(
      object routeParams,
      IEnumerable<KeyValuePair<string, string>> query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<Microsoft.VisualStudio.Services.Identity.Identity> CreateFrameworkIdentityAsync(
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> CreateGroupsAsync(
      Guid scopeId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<Microsoft.VisualStudio.Services.Identity.Identity> CreateOrBindIdentity(
      Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentityScope> CreateScopeAsync(
      Guid scopeId,
      Guid parentScopeId,
      GroupScopeType scopeType,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription,
      Guid creatorId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<HttpResponseMessage> DeleteGroupAsync(
      Guid groupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<HttpResponseMessage> DeleteGroupAsync(
      IdentityDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<HttpResponseMessage> DeleteScopeAsync(
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<bool> ForceRemoveMemberFromGroupAsync(
      IdentityDescriptor containerId,
      IdentityDescriptor memberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentityDescriptor> GetDescriptorByIdAsync(
      Guid id,
      bool? isMasterId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ChangedIdentities> GetIdentityChangesAsync(
      int identitySequenceId,
      int groupSequenceId,
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ChangedIdentities> GetIdentityChangesAsync(
      int identitySequenceId,
      int groupSequenceId,
      int organizationIdentitySequenceId,
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ChangedIdentities> GetIdentityChangesAsync(
      int identitySequenceId,
      int groupSequenceId,
      int organizationIdentitySequenceId,
      int pageSize,
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitySelf> GetIdentitySelfAsync(object userState = null, CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitySnapshot> GetIdentitySnapshotAsync(
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<int> GetMaxSequenceIdAsync(object userState = null, CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentityScope> GetScopeAsync(
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentityScope> GetScopeAsync(
      string scopeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AccessTokenResult> GetSignedInToken(object userState = null, CancellationToken cancellationToken = default (CancellationToken));

    Task<AccessTokenResult> GetSignoutToken(object userState = null, CancellationToken cancellationToken = default (CancellationToken));

    Task<TenantInfo> GetTenant(
      string tenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IList<Guid>> GetUserIdentityIdsByDomainIdAsync(
      Guid domainId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<bool> IsMember(
      IdentityDescriptor containerId,
      IdentityDescriptor memberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> ListGroupsAsync(
      Guid[] scopeIds = null,
      bool recurse = false,
      bool deleted = false,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<PagedIdentities> ListUsersAsync(
      string scopeDescriptor = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> ReadIdentitiesAsync(
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> ReadIdentitiesAsync(
      Guid scopeId,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> ReadIdentitiesAsync(
      IdentitySearchFilter searchFilter,
      string filterValue,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> ReadIdentitiesAsync(
      IdentitySearchFilter searchFilter,
      string filterValue,
      ReadIdentitiesOptions options,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<Guid> identityIds,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<IdentityDescriptor> descriptors,
      RequestHeadersContext requestHeadersContext,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentityAsync(
      Guid identityId,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentityAsync(
      string identityPuid,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<bool> RemoveMemberFromGroupAsync(
      IdentityDescriptor containerId,
      IdentityDescriptor memberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<HttpResponseMessage> RenameScopeAsync(
      Guid scopeId,
      string newName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<HttpResponseMessage> RestoreGroupScopeAsync(
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<HttpResponseMessage> SwapIdentityAsync(
      Guid id1,
      Guid id2,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task TransferIdentityRightsBatchAsync(
      IdentityRightsTransferData identityRightsTransferData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task TransferIdentityRightsSingleAsync(
      Guid sourceId,
      Guid targetMasterId,
      Guid sourceMasterId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<IdentityUpdateData>> UpdateIdentitiesAsync(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<IdentityUpdateData>> UpdateIdentitiesAsync(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetaDataUpdate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<HttpResponseMessage> UpdateIdentityAsync(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
