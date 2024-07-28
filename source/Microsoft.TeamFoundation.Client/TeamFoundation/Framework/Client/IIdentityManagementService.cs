// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IIdentityManagementService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface IIdentityManagementService
  {
    TeamFoundationIdentity[] ReadIdentities(
      IdentityDescriptor[] descriptors,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions);

    TeamFoundationIdentity ReadIdentity(
      IdentityDescriptor descriptor,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions);

    TeamFoundationIdentity[] ReadIdentities(
      Guid[] teamFoundationIds,
      MembershipQuery queryMembership);

    TeamFoundationIdentity[][] ReadIdentities(
      IdentitySearchFactor searchFactor,
      string[] searchFactorValues,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions);

    TeamFoundationIdentity ReadIdentity(
      IdentitySearchFactor searchFactor,
      string searchFactorValue,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions);

    IdentityDescriptor CreateApplicationGroup(
      string scopeId,
      string groupName,
      string groupDescription);

    TeamFoundationIdentity[] ListApplicationGroups(string scopeId, ReadIdentityOptions readOptions);

    void UpdateApplicationGroup(
      IdentityDescriptor groupDescriptor,
      GroupProperty groupProperty,
      string newValue);

    void DeleteApplicationGroup(IdentityDescriptor groupDescriptor);

    void AddMemberToApplicationGroup(
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor);

    void RemoveMemberFromApplicationGroup(
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor);

    bool IsMember(IdentityDescriptor groupDescriptor, IdentityDescriptor descriptor);

    bool RefreshIdentity(IdentityDescriptor descriptor);

    string GetScopeName(string scopeId);

    bool IsOwner(IdentityDescriptor descriptor);

    bool IsOwnedWellKnownGroup(IdentityDescriptor descriptor);

    string IdentityDomainScope { get; }
  }
}
