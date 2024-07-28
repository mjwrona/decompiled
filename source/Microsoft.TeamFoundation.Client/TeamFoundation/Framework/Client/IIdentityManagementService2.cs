// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IIdentityManagementService2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface IIdentityManagementService2 : IIdentityManagementService
  {
    TeamFoundationIdentity[] GetMostRecentlyUsedUsers();

    TeamFoundationIdentity[] GetMostRecentlyUsedUsersEx(Guid teamId);

    void AddRecentUser(TeamFoundationIdentity identity);

    void AddRecentUser(Guid teamFoundationId);

    TeamFoundationIdentity ReadIdentity(string generalSearchValue);

    FilteredIdentitiesList ReadFilteredIdentities(
      string expression,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward,
      int queryMembership);

    void SetCustomDisplayName(string customDisplayName);

    void ClearCustomDisplayName();

    void UpdateExtendedProperties(TeamFoundationIdentity identity);

    TeamFoundationIdentity[] ReadIdentities(
      IdentityDescriptor[] descriptors,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    TeamFoundationIdentity ReadIdentity(
      IdentityDescriptor descriptor,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    TeamFoundationIdentity[] ReadIdentities(
      Guid[] teamFoundationIds,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    TeamFoundationIdentity[][] ReadIdentities(
      IdentitySearchFactor searchFactor,
      string[] searchFactorValues,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    TeamFoundationIdentity ReadIdentity(
      IdentitySearchFactor searchFactor,
      string searchFactorValue,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);

    TeamFoundationIdentity[] ListApplicationGroups(
      string scopeId,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope);
  }
}
