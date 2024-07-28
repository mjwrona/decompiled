// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IdentityManagementService2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class IdentityManagementService2 : 
    IdentityManagementService,
    IIdentityManagementService2,
    IIdentityManagementService
  {
    private IdentityManagementWebService2 m_proxy2;

    internal IdentityManagementService2(TfsConnection tfsBase)
      : base(tfsBase)
    {
      this.m_proxy = (IdentityManagementWebService) new IdentityManagementWebService2(tfsBase);
      this.m_proxy2 = (IdentityManagementWebService2) this.m_proxy;
    }

    public TeamFoundationIdentity[] GetMostRecentlyUsedUsers()
    {
      TeamFoundationIdentity[] recentlyUsedUsers = this.m_proxy2.GetMostRecentlyUsedUsers(1);
      this.InitializeFromWebService(recentlyUsedUsers);
      return recentlyUsedUsers;
    }

    public TeamFoundationIdentity[] GetMostRecentlyUsedUsersEx(Guid teamId)
    {
      TeamFoundationIdentity[] recentlyUsedUsers = this.GetMostRecentlyUsedUsers();
      if (teamId != Guid.Empty)
      {
        TeamFoundationIdentity[] foundationIdentityArray = this.ReadIdentities(new Guid[1]
        {
          teamId
        }, MembershipQuery.Expanded);
        if (foundationIdentityArray != null && foundationIdentityArray.Length != 0)
        {
          TeamFoundationIdentity[] recentlyUsedUsersEx = this.ReadIdentities(foundationIdentityArray[0].Members, MembershipQuery.None, ReadIdentityOptions.None);
          if (recentlyUsedUsersEx != null && recentlyUsedUsersEx.Length != 0)
          {
            if (recentlyUsedUsers == null)
              return recentlyUsedUsersEx;
            Dictionary<Guid, TeamFoundationIdentity> dictionary = ((IEnumerable<TeamFoundationIdentity>) recentlyUsedUsers).ToDictionary<TeamFoundationIdentity, Guid, TeamFoundationIdentity>((Func<TeamFoundationIdentity, Guid>) (x => x.TeamFoundationId), (Func<TeamFoundationIdentity, TeamFoundationIdentity>) (x => x));
            foreach (TeamFoundationIdentity foundationIdentity in recentlyUsedUsersEx)
            {
              if (!foundationIdentity.IsContainer && !dictionary.ContainsKey(foundationIdentity.TeamFoundationId))
                dictionary.Add(foundationIdentity.TeamFoundationId, foundationIdentity);
            }
            return dictionary.Values.ToArray<TeamFoundationIdentity>();
          }
        }
      }
      return recentlyUsedUsers;
    }

    public void AddRecentUser(TeamFoundationIdentity identity)
    {
      if (identity == null || !(identity.TeamFoundationId != Guid.Empty))
        return;
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.AddUser), (object) identity.TeamFoundationId);
    }

    public void AddRecentUser(Guid teamFoundationId)
    {
      if (!(teamFoundationId != Guid.Empty))
        return;
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.AddUser), (object) teamFoundationId);
    }

    public FilteredIdentitiesList ReadFilteredIdentities(
      string expression,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward,
      int queryMembership)
    {
      FilteredIdentitiesList filteredIdentitiesList = this.m_proxy2.ReadFilteredIdentities(expression, suggestedPageSize, lastSearchResult, lookForward, queryMembership, 1);
      this.InitializeFromWebService(filteredIdentitiesList.Items);
      return filteredIdentitiesList;
    }

    public TeamFoundationIdentity ReadIdentity(string generalSearchValue) => this.ReadIdentity(IdentitySearchFactor.General, generalSearchValue, MembershipQuery.None, ReadIdentityOptions.None);

    public TeamFoundationIdentity[] ListApplicationGroups(
      string scopeId,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      TeamFoundationIdentity[] data = this.m_proxy2.ListApplicationGroups(scopeId, (int) readOptions, 1, propertyNameFilters, (int) propertyScope);
      this.InitializeFromWebService(data);
      return data;
    }

    public void SetCustomDisplayName(string customDisplayName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(customDisplayName, nameof (customDisplayName));
      this.m_proxy2.SetCustomDisplayName(customDisplayName);
    }

    public void ClearCustomDisplayName() => this.m_proxy2.SetCustomDisplayName((string) null);

    private void AddUser(object obj) => this.m_proxy2.AddRecentUser((Guid) obj);

    public TeamFoundationIdentity[] ReadIdentities(
      IdentityDescriptor[] descriptors,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor[]>(descriptors, nameof (descriptors));
      foreach (IdentityDescriptor descriptor in descriptors)
        IdentityHelper.CheckDescriptor(descriptor, "descriptors element");
      TeamFoundationIdentity[] data = this.m_proxy2.ReadIdentitiesByDescriptor(descriptors, (int) queryMembership, (int) readOptions, 1, propertyNameFilters, (int) propertyScope);
      this.InitializeFromWebService(data);
      return data;
    }

    public TeamFoundationIdentity ReadIdentity(
      IdentityDescriptor descriptor,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      return this.ReadIdentities(new IdentityDescriptor[1]
      {
        descriptor
      }, queryMembership, readOptions, propertyNameFilters, propertyScope)[0];
    }

    public TeamFoundationIdentity[] ReadIdentities(
      Guid[] teamFoundationIds,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      ArgumentUtility.CheckForNull<Guid[]>(teamFoundationIds, nameof (teamFoundationIds));
      TeamFoundationIdentity[] data = this.m_proxy2.ReadIdentitiesById(teamFoundationIds, (int) queryMembership, 1, (int) readOptions, propertyNameFilters, (int) propertyScope);
      this.InitializeFromWebService(data);
      return data;
    }

    public TeamFoundationIdentity[][] ReadIdentities(
      IdentitySearchFactor searchFactor,
      string[] searchFactorValues,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      ArgumentUtility.CheckForNull<string[]>(searchFactorValues, nameof (searchFactorValues));
      foreach (string searchFactorValue in searchFactorValues)
        ArgumentUtility.CheckStringForNullOrEmpty(searchFactorValue, "searchFactorValues element");
      TeamFoundationIdentity[][] foundationIdentityArray = this.m_proxy2.ReadIdentities((int) searchFactor, searchFactorValues, (int) queryMembership, (int) readOptions, 1, propertyNameFilters, (int) propertyScope);
      for (int index = 0; index < searchFactorValues.Length; ++index)
        this.InitializeFromWebService(foundationIdentityArray[index]);
      return foundationIdentityArray;
    }

    public TeamFoundationIdentity ReadIdentity(
      IdentitySearchFactor searchFactor,
      string searchFactorValue,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      TeamFoundationIdentity[] readIdentity = this.ReadIdentities(searchFactor, new string[1]
      {
        searchFactorValue
      }, queryMembership, readOptions, propertyNameFilters, propertyScope)[0];
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
          throw new MultipleIdentitiesFoundException(searchFactorValue, readIdentity);
        return foundationIdentity1;
      }
      return length == 1 ? readIdentity[0] : (TeamFoundationIdentity) null;
    }

    public void UpdateExtendedProperties(TeamFoundationIdentity identity)
    {
      HashSet<string> modifiedPropertiesLog1 = identity.GetModifiedPropertiesLog(IdentityPropertyScope.Global);
      HashSet<string> modifiedPropertiesLog2 = identity.GetModifiedPropertiesLog(IdentityPropertyScope.Local);
      if (modifiedPropertiesLog1 == null && modifiedPropertiesLog2 == null)
        return;
      PropertyValue[] updates = this.BuildModifiedProperties(IdentityPropertyScope.Global, identity, modifiedPropertiesLog1);
      PropertyValue[] localUpdates = this.BuildModifiedProperties(IdentityPropertyScope.Local, identity, modifiedPropertiesLog2);
      this.m_proxy2.UpdateIdentityExtendedProperties(identity.Descriptor, updates, localUpdates);
      identity.ResetModifiedProperties();
    }

    private PropertyValue[] BuildModifiedProperties(
      IdentityPropertyScope propertyScope,
      TeamFoundationIdentity identity,
      HashSet<string> modifiedPropertiesLog)
    {
      PropertyValue[] propertyValueArray = new PropertyValue[modifiedPropertiesLog != null ? modifiedPropertiesLog.Count : 0];
      if (modifiedPropertiesLog != null)
      {
        int num = 0;
        foreach (string str in modifiedPropertiesLog)
          propertyValueArray[num++] = new PropertyValue(str, identity.GetProperty(propertyScope, str));
      }
      return propertyValueArray;
    }
  }
}
