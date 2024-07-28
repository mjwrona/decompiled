// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ITeamFoundationIdentityService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Server.Core
{
  [DefaultServiceImplementation(typeof (TeamFoundationIdentityService))]
  public interface ITeamFoundationIdentityService : IVssFrameworkService
  {
    void AddGroupAdministrator(
      IVssRequestContext requestContext,
      IdentityDescriptor groupIdentity,
      IdentityDescriptor descriptor);

    void RemoveGroupAdministrator(
      IVssRequestContext requestContext,
      IdentityDescriptor groupIdentity,
      IdentityDescriptor descriptor);

    void AddRecentUser(IVssRequestContext requestContext, Guid recentUser);

    TeamFoundationIdentity[] GetMostRecentlyUsedUsers(IVssRequestContext requestContext);

    TeamFoundationDataReader GetIdentityChanges(
      IVssRequestContext requestContext,
      Tuple<int, int> sequenceId);

    TeamFoundationDataReader GetIdentityChanges(
      IVssRequestContext requestContext,
      Tuple<int, int, int> sequenceId);

    TeamFoundationDataReader GetIdentityChanges(
      IVssRequestContext requestContext,
      Tuple<int, int, int> sequenceId,
      int pageSize);

    IdentityDescriptor CreateDescriptor(IVssRequestContext requestContext, IIdentity identity);

    TeamFoundationIdentity ReadIdentityFromSource(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool withDirectMembership);

    TeamFoundationFilteredIdentitiesList ReadFilteredIdentities(
      IVssRequestContext requestContext,
      string expression,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward,
      MembershipQuery membershipQuery);

    TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDescriptor[] descriptors);

    TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDescriptor[] descriptors,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters);

    TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDescriptor[] descriptors,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    TeamFoundationIdentity ReadRequestIdentity(IVssRequestContext requestContext);

    TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions);

    TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      Guid[] teamFoundationIds);

    TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      Guid[] teamFoundationIds,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters);

    TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      Guid[] teamFoundationIds,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    TeamFoundationIdentity[][] ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string[] factorValues);

    TeamFoundationIdentity[][] ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string[] factorValues,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters);

    TeamFoundationIdentity[][] ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string[] factorValues,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    TeamFoundationIdentitySearchResult SearchIdentities(
      IVssRequestContext requestContext,
      IdentitySearchParameters searchParameters);

    TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      string generalSearchValue);

    TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string factorValue);

    TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string factorValue,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters);

    TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string factorValue,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    IdentityDescriptor CreateScope(
      IVssRequestContext requestContext,
      string scopeId,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription);

    void GetScopeInfo(
      IVssRequestContext requestContext,
      string scopeId,
      out string scopeName,
      out IdentityDescriptor administrators,
      out bool isGlobal);

    void DeleteScope(IVssRequestContext requestContext, string scopeId);

    void RenameScope(IVssRequestContext requestContext, string scopeId, string newName);

    TeamFoundationIdentity CreateApplicationGroup(
      IVssRequestContext requestContext,
      string scopeId,
      string groupName,
      string groupDescription);

    TeamFoundationIdentity CreateApplicationGroup(
      IVssRequestContext requestContext,
      string projectUri,
      string groupName,
      string groupDescription,
      bool scopeLocal,
      bool hasRestrictedVisibility);

    void EnsureWellKnownGroupExists(
      IVssRequestContext requestContext,
      string groupSid,
      string groupName,
      string groupDescription);

    IdentityDescriptor CreateUser(
      IVssRequestContext requestContext,
      string userDomain,
      string account,
      string description);

    TeamFoundationIdentity[] ListApplicationGroups(
      IVssRequestContext requestContext,
      string scope,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters);

    TeamFoundationIdentity[] ListApplicationGroups(
      IVssRequestContext requestContext,
      string scope,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    void UpdateApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      GroupProperty groupProperty,
      string newValue);

    void DeleteApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor);

    ServicingJobDetail DeleteUser(
      IVssRequestContext requestContext,
      IdentityDescriptor userDescriptor,
      bool deleteArtifacts = true);

    TeamFoundationIdentity AddMemberToApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      TeamFoundationIdentity member);

    TeamFoundationIdentity AddMemberToApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor);

    TeamFoundationIdentity EnsureIsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor);

    TeamFoundationIdentity EnsureIsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      TeamFoundationIdentity member);

    void RemoveMemberFromApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor);

    void RemoveMemberFromApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor,
      bool errorOnNotMember);

    void EnsureNotMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor);

    bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor);

    bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor,
      bool forceCacheReload);

    bool RefreshIdentity(IVssRequestContext requestContext, IdentityDescriptor descriptor);

    void SetCustomDisplayName(IVssRequestContext requestContext, string customDisplayName);

    void SetPreferredEmailAddress(IVssRequestContext requestContext, string preferredEmailAddress);

    string GetPreferredEmailAddress(IVssRequestContext requestContext, Guid teamFoundationId);

    string GetPreferredEmailAddress(
      IVssRequestContext requestContext,
      Guid teamFoundationId,
      bool confirmed);

    bool IsEmailConfirmationPending(IVssRequestContext requestContext, Guid TeamFoundationId);

    void UpdateIdentity(
      IVssRequestContext requestContext,
      TeamFoundationIdentity identity,
      IdentityPropertyScope scope = IdentityPropertyScope.Both);

    void UpdateIdentities(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationIdentity> identities,
      IdentityPropertyScope scope = IdentityPropertyScope.Both);

    void UpdateExtendedProperties(
      IVssRequestContext requestContext,
      IdentityPropertyScope propertyScope,
      IdentityDescriptor descriptor,
      IEnumerable<PropertyValue> properties);

    string GetProjectAdminSid(IVssRequestContext requestContext, string projectUri);

    bool IsIdentityCached(IVssRequestContext requestContext, SecurityIdentifier securityId);

    int ReadBatchSizeLimit { get; }

    TeamFoundationFilteredIdentitiesList ReadFilteredIdentitiesByDescriptor(
      IVssRequestContext requestContext,
      IEnumerable<IdentityDescriptor> identityDescriptors,
      int suggestedPageSize,
      IEnumerable<IdentityFilter> filters,
      string lastSearchResult,
      bool lookForward,
      MembershipQuery membershipQueryForTfids,
      MembershipQuery membershipQuery,
      IEnumerable<string> propertyNameFilters);
  }
}
