// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IdentityManagementService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class IdentityManagementService : IIdentityManagementService
  {
    protected IdentityManagementWebService m_proxy;
    private SecurityIdentifier m_domainSid;
    private string m_domainSidWithWellKnownPrefix;
    private string m_domainScope;

    internal IdentityManagementService(TfsConnection tfsBase)
    {
      this.m_proxy = new IdentityManagementWebService(tfsBase);
      this.m_domainSid = SidIdentityHelper.GetDomainSid(tfsBase.InstanceId);
      this.m_domainSidWithWellKnownPrefix = this.m_domainSid.Value + SidIdentityHelper.WellKnownSidType;
      this.m_domainScope = TFCommonUtil.GetIdentityDomainScope(tfsBase.InstanceId);
    }

    public TeamFoundationIdentity[] ReadIdentities(
      IdentityDescriptor[] descriptors,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor[]>(descriptors, nameof (descriptors));
      foreach (IdentityDescriptor descriptor in descriptors)
        IdentityHelper.CheckDescriptor(descriptor, "descriptors element");
      TeamFoundationIdentity[] data = this.m_proxy.ReadIdentitiesByDescriptor(descriptors, (int) queryMembership, (int) readOptions, 1, (IEnumerable<string>) null, 0);
      this.InitializeFromWebService(data);
      return data;
    }

    public TeamFoundationIdentity ReadIdentity(
      IdentityDescriptor descriptor,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions)
    {
      return this.ReadIdentities(new IdentityDescriptor[1]
      {
        descriptor
      }, queryMembership, readOptions)[0];
    }

    public TeamFoundationIdentity[] ReadIdentities(
      Guid[] teamFoundationIds,
      MembershipQuery queryMembership)
    {
      ArgumentUtility.CheckForNull<Guid[]>(teamFoundationIds, nameof (teamFoundationIds));
      TeamFoundationIdentity[] data = this.m_proxy.ReadIdentitiesById(teamFoundationIds, (int) queryMembership, 1, 0, (IEnumerable<string>) null, 0);
      this.InitializeFromWebService(data);
      return data;
    }

    public TeamFoundationIdentity[][] ReadIdentities(
      IdentitySearchFactor searchFactor,
      string[] factorValues,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions)
    {
      ArgumentUtility.CheckForNull<string[]>(factorValues, nameof (factorValues));
      foreach (string factorValue in factorValues)
        ArgumentUtility.CheckStringForNullOrEmpty(factorValue, "factorValues element");
      TeamFoundationIdentity[][] foundationIdentityArray = this.m_proxy.ReadIdentities((int) searchFactor, factorValues, (int) queryMembership, (int) readOptions, 1, (IEnumerable<string>) null, 0);
      for (int index = 0; index < factorValues.Length; ++index)
        this.InitializeFromWebService(foundationIdentityArray[index]);
      return foundationIdentityArray;
    }

    public TeamFoundationIdentity ReadIdentity(
      IdentitySearchFactor searchFactor,
      string factorValue,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions)
    {
      TeamFoundationIdentity[] readIdentity = this.ReadIdentities(searchFactor, new string[1]
      {
        factorValue
      }, queryMembership, readOptions)[0];
      int length = readIdentity.Length;
      if (length > 1)
      {
        int num = 0;
        TeamFoundationIdentity foundationIdentity1 = (TeamFoundationIdentity) null;
        foreach (TeamFoundationIdentity foundationIdentity2 in readIdentity)
        {
          if (foundationIdentity2.IsActive)
          {
            foundationIdentity1 = foundationIdentity2;
            ++num;
          }
        }
        if (num != 1)
          throw new MultipleIdentitiesFoundException(factorValue, readIdentity);
        return foundationIdentity1;
      }
      return length == 1 ? readIdentity[0] : (TeamFoundationIdentity) null;
    }

    public IdentityDescriptor CreateApplicationGroup(
      string projectUri,
      string groupName,
      string groupDescription)
    {
      TFCommonUtil.CheckProjectUri(ref projectUri, true);
      TFCommonUtil.CheckGroupName(ref groupName);
      TFCommonUtil.CheckGroupDescription(ref groupDescription);
      return this.m_proxy.CreateApplicationGroup(projectUri, groupName, groupDescription);
    }

    public TeamFoundationIdentity[] ListApplicationGroups(
      string projectUri,
      ReadIdentityOptions readOptions)
    {
      TeamFoundationIdentity[] data = this.m_proxy.ListApplicationGroups(projectUri, (int) readOptions, 1, (IEnumerable<string>) null, 0);
      this.InitializeFromWebService(data);
      return data;
    }

    public string GetScopeName(string scopeId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(scopeId, nameof (scopeId));
      return this.m_proxy.GetScopeName(scopeId);
    }

    public void UpdateApplicationGroup(
      IdentityDescriptor groupDescriptor,
      GroupProperty property,
      string newValue)
    {
      IdentityHelper.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      this.m_proxy.UpdateApplicationGroup(groupDescriptor, (int) property, newValue);
    }

    public void DeleteApplicationGroup(IdentityDescriptor groupDescriptor)
    {
      IdentityHelper.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      this.m_proxy.DeleteApplicationGroup(groupDescriptor);
    }

    public void AddMemberToApplicationGroup(
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      IdentityHelper.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityHelper.CheckDescriptor(descriptor, nameof (descriptor));
      this.m_proxy.AddMemberToApplicationGroup(groupDescriptor, descriptor);
    }

    public void RemoveMemberFromApplicationGroup(
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      IdentityHelper.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityHelper.CheckDescriptor(descriptor, nameof (descriptor));
      this.m_proxy.RemoveMemberFromApplicationGroup(groupDescriptor, descriptor);
    }

    public bool IsMember(IdentityDescriptor groupDescriptor, IdentityDescriptor descriptor)
    {
      IdentityHelper.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityHelper.CheckDescriptor(descriptor, nameof (descriptor));
      return this.m_proxy.IsMember(groupDescriptor, descriptor);
    }

    public bool RefreshIdentity(IdentityDescriptor descriptor)
    {
      IdentityHelper.CheckDescriptor(descriptor, nameof (descriptor));
      return this.m_proxy.RefreshIdentity(descriptor);
    }

    public bool IsOwner(IdentityDescriptor descriptor)
    {
      IdentityHelper.CheckDescriptor(descriptor, nameof (descriptor));
      if (!string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        return false;
      return descriptor.Identifier.StartsWith(this.m_domainSid.Value, StringComparison.OrdinalIgnoreCase) || descriptor.Identifier.StartsWith(SidIdentityHelper.WellKnownSidPrefix, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsOwnedWellKnownGroup(IdentityDescriptor descriptor)
    {
      IdentityHelper.CheckDescriptor(descriptor, nameof (descriptor));
      if (!string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        return false;
      return descriptor.Identifier.StartsWith(this.m_domainSidWithWellKnownPrefix, StringComparison.OrdinalIgnoreCase) || descriptor.Identifier.StartsWith(SidIdentityHelper.WellKnownSidPrefix, StringComparison.OrdinalIgnoreCase);
    }

    public string IdentityDomainScope => this.m_domainScope;

    protected void InitializeFromWebService(TeamFoundationIdentity[] data)
    {
      for (int index = 0; index < data.Length; ++index)
      {
        if (data[index] != null)
          data[index].InitializeFromWebService();
      }
    }
  }
}
