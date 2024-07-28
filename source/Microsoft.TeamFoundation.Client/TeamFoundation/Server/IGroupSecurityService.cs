// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.IGroupSecurityService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Client;
using System;

namespace Microsoft.TeamFoundation.Server
{
  [Obsolete("IGroupSecurityService is obsolete.  Please use the IIdentityManagementService or ISecurityService instead.", false)]
  public interface IGroupSecurityService
  {
    Identity ReadIdentity(SearchFactor factor, string factorValue, QueryMembership queryMembership);

    Identity[] ReadIdentities(
      SearchFactor factor,
      string[] factorValue,
      QueryMembership queryMembership);

    Identity ReadIdentityFromSource(SearchFactor factor, string factorValue);

    bool IsIdentityCached(SearchFactor factor, string factorValue);

    string GetChangedIdentities(int sequenceId);

    string CreateApplicationGroup(string projectUri, string groupName, string groupDescription);

    Identity[] ListApplicationGroups(string projectUri);

    void UpdateApplicationGroup(
      string groupSid,
      ApplicationGroupProperty groupProperty,
      string newValue);

    void DeleteApplicationGroup(string groupSid);

    void AddMemberToApplicationGroup(string groupSid, string identitySid);

    void RemoveMemberFromApplicationGroup(string groupSid, string identitySid);

    bool IsMember(string groupSid, string identitySid);

    Identity Convert(TeamFoundationIdentity identity);

    TeamFoundationIdentity Convert(Identity identity);
  }
}
