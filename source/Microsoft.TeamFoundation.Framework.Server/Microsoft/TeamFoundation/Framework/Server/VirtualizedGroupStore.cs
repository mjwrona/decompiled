// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VirtualizedGroupStore
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VirtualizedGroupStore
  {
    private Guid m_domainId;
    private IdentityMapper m_mapper;
    private Dictionary<IdentityDescriptor, VirtualizedGroupStore.WellKnownGroupInformation> m_descriptorMap;
    protected Dictionary<Guid, VirtualizedGroupStore.WellKnownGroupInformation> m_vsidMap;
    private const string c_scopeName = "TEAM FOUNDATION";

    public VirtualizedGroupStore(
      IVssRequestContext requestContext,
      Guid domainId,
      IdentityMapper mapper)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckHostedDeployment();
      IDictionary<Guid, Guid> groupIdOverrides = this.GetGroupIdOverrides(requestContext);
      this.m_domainId = domainId;
      this.m_mapper = mapper;
      this.m_descriptorMap = VirtualizedGroupStore.CreateDescriptorMap(groupIdOverrides, this.m_domainId, this.m_mapper);
      this.m_vsidMap = this.m_descriptorMap.Values.ToDictionary<VirtualizedGroupStore.WellKnownGroupInformation, Guid>((Func<VirtualizedGroupStore.WellKnownGroupInformation, Guid>) (s => s.Id));
    }

    public IdentityScope GetScope(IVssRequestContext requestContext, string scopeName)
    {
      if (!StringComparer.Ordinal.Equals(scopeName, "TEAM FOUNDATION"))
        throw new GroupScopeDoesNotExistException(scopeName);
      return this.GetScopeHelper();
    }

    public IdentityScope GetScope(IVssRequestContext requestContext, Guid scopeId)
    {
      if (scopeId != this.m_domainId)
        throw new GroupScopeDoesNotExistException(scopeId);
      return this.GetScopeHelper();
    }

    public Guid GetScopeParentId(IVssRequestContext requestContext, Guid scopeId)
    {
      if (scopeId != this.m_domainId)
        throw new GroupScopeDoesNotExistException(scopeId);
      return Guid.Empty;
    }

    private IdentityScope GetScopeHelper() => new IdentityScope()
    {
      Administrators = this.m_mapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup),
      Id = this.m_domainId,
      IsActive = false,
      IsGlobal = true,
      LocalScopeId = this.m_domainId,
      Name = "TEAM FOUNDATION",
      ParentId = Guid.Empty,
      ScopeType = GroupScopeType.ServiceHost,
      SecuringHostId = this.m_domainId
    };

    public Microsoft.VisualStudio.Services.Identity.Identity CreateGroup(
      IVssRequestContext requestContext,
      Guid scopeId,
      string groupSid,
      string groupName,
      string groupDescription,
      SpecialGroupType specialType = SpecialGroupType.Generic,
      bool scopeLocal = true,
      bool hasRestrictedVisibility = false)
    {
      VirtualizedGroupStore.WellKnownGroupInformation groupInformation;
      if (((!(scopeId == Guid.Empty) && !(scopeId == this.m_domainId) || !this.m_descriptorMap.TryGetValue(IdentityHelper.CreateTeamFoundationDescriptor(groupSid), out groupInformation) || !StringComparer.Ordinal.Equals(groupName, groupInformation.ProviderDisplayName) || !StringComparer.Ordinal.Equals(groupDescription, groupInformation.Description) ? 0 : (groupInformation.SpecialType == specialType.ToString() ? 1 : 0)) & (scopeLocal ? 1 : 0)) != 0 && !hasRestrictedVisibility)
        return groupInformation.GetIdentity();
      throw new NotImplementedException();
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateGroup(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid groupId,
      string groupSid,
      string groupName,
      string groupDescription,
      SpecialGroupType specialType = SpecialGroupType.Generic,
      bool scopeLocal = true,
      bool hasRestrictedVisibility = false)
    {
      VirtualizedGroupStore.WellKnownGroupInformation groupInformation;
      if (((!(scopeId == Guid.Empty) && !(scopeId == this.m_domainId) || !this.m_descriptorMap.TryGetValue(IdentityHelper.CreateTeamFoundationDescriptor(groupSid), out groupInformation) || !(groupId == Guid.Empty) || !StringComparer.Ordinal.Equals(groupName, groupInformation.ProviderDisplayName) || !StringComparer.Ordinal.Equals(groupDescription, groupInformation.Description) ? 0 : (groupInformation.SpecialType == specialType.ToString() ? 1 : 0)) & (scopeLocal ? 1 : 0)) != 0 && !hasRestrictedVisibility)
        return groupInformation.GetIdentity();
      throw new NotImplementedException();
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      if (propertyNameFilters != null && propertyNameFilters.Any<string>())
        throw new NotImplementedException();
      List<Microsoft.VisualStudio.Services.Identity.Identity> toReturn = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (scopeIds == null || ((IEnumerable<Guid>) scopeIds).Contains<Guid>(this.m_domainId))
        this.m_descriptorMap.Values.ForEach<VirtualizedGroupStore.WellKnownGroupInformation>((Action<VirtualizedGroupStore.WellKnownGroupInformation>) (wkg => toReturn.Add(wkg.GetIdentity())));
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) toReturn;
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ListDeletedGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      if (this.IsMember(requestContext, groupDescriptor, memberDescriptor))
        return false;
      throw new NotImplementedException();
    }

    public bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity member)
    {
      return this.AddMemberToGroup(requestContext, groupDescriptor, member.Descriptor);
    }

    public virtual bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      bool flag = false;
      if (string.Equals(memberDescriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
      {
        groupDescriptor = this.m_mapper.MapToWellKnownIdentifier(groupDescriptor);
        memberDescriptor = this.m_mapper.MapToWellKnownIdentifier(memberDescriptor);
        if (IdentityDescriptorComparer.Instance.Equals(memberDescriptor, GroupWellKnownIdentityDescriptors.ServiceUsersGroup) && IdentityDescriptorComparer.Instance.Equals(groupDescriptor, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup))
          flag = true;
        else if (IdentityDescriptorComparer.Instance.Equals(groupDescriptor, GroupWellKnownIdentityDescriptors.EveryoneGroup) && this.m_descriptorMap.ContainsKey(memberDescriptor))
          flag = true;
      }
      else if (IdentityDescriptorComparer.Instance.Equals(groupDescriptor, GroupWellKnownIdentityDescriptors.ServicePrincipalGroup) && ServicePrincipals.IsServicePrincipal(requestContext, memberDescriptor))
        flag = true;
      else if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        groupDescriptor = this.m_mapper.MapToWellKnownIdentifier(groupDescriptor);
        if (IdentityDescriptorComparer.Instance.Equals(groupDescriptor, GroupWellKnownIdentityDescriptors.EveryoneGroup))
          flag = true;
      }
      return flag;
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false,
      bool includeRequestIdentity = false)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity(false);
      if (includeRequestIdentity)
        identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[identityIds.Count];
      for (int index = 0; index < identityIds.Count; ++index)
      {
        Guid identityId = identityIds[index];
        VirtualizedGroupStore.WellKnownGroupInformation groupInformation;
        if (this.m_vsidMap.TryGetValue(identityId, out groupInformation))
        {
          if (propertyNameFilters != null && propertyNameFilters.Any<string>())
            throw new NotImplementedException();
          if (queryMembership != QueryMembership.None)
            throw new NotImplementedException();
          if (identityArray == null)
            identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[identityIds.Count];
          identityArray[index] = groupInformation.GetIdentity();
        }
        else if (includeRequestIdentity)
        {
          Guid guid1 = identityId;
          Guid? nullable = userIdentity?.Id;
          if ((nullable.HasValue ? (guid1 == nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          {
            Guid guid2 = identityId;
            nullable = userIdentity?.MasterId;
            if ((nullable.HasValue ? (guid2 == nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
              continue;
          }
          identityArray[index] = userIdentity;
        }
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false,
      bool includeRequestIdentity = false)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
      if (includeRequestIdentity)
        identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[descriptors.Count];
      for (int index = 0; index < descriptors.Count; ++index)
      {
        IdentityDescriptor descriptor = descriptors[index];
        if (!(descriptor == (IdentityDescriptor) null))
        {
          VirtualizedGroupStore.WellKnownGroupInformation groupInformation;
          if (this.m_descriptorMap.TryGetValue(this.m_mapper.MapToWellKnownIdentifier(descriptor), out groupInformation))
          {
            if (propertyNameFilters != null && propertyNameFilters.Any<string>())
              throw new NotImplementedException();
            if (queryMembership != QueryMembership.None)
              throw new NotImplementedException();
            if (identityArray == null)
              identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[descriptors.Count];
            identityArray[index] = groupInformation.GetIdentity();
          }
          else if (includeRequestIdentity)
          {
            if (descriptor == requestContext.UserContext)
              identityArray[index] = requestContext.GetUserIdentity(false);
            else if (requestContext.Items.ContainsKey(descriptor.ToString()))
              identityArray[index] = (Microsoft.VisualStudio.Services.Identity.Identity) requestContext.Items[descriptor.ToString()];
            else if (requestContext.RootContext.Items.ContainsKey(descriptor.ToString()))
              identityArray[index] = (Microsoft.VisualStudio.Services.Identity.Identity) requestContext.RootContext.Items[descriptor.ToString()];
          }
        }
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
    }

    private IDictionary<Guid, Guid> GetGroupIdOverrides(IVssRequestContext requestContext) => (IDictionary<Guid, Guid>) requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Service/VirtualizedGroupStore/GroupIdOverrides/*").ToDictionary<RegistryEntry, Guid, Guid>((Func<RegistryEntry, Guid>) (x => new Guid(x.Name)), (Func<RegistryEntry, Guid>) (x => new Guid(x.Value)));

    private static Dictionary<IdentityDescriptor, VirtualizedGroupStore.WellKnownGroupInformation> CreateDescriptorMap(
      IDictionary<Guid, Guid> groupIdOverrides,
      Guid domainId,
      IdentityMapper mapper)
    {
      return new Dictionary<IdentityDescriptor, VirtualizedGroupStore.WellKnownGroupInformation>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance)
      {
        [GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup] = new VirtualizedGroupStore.WellKnownGroupInformation()
        {
          Id = VirtualizedGroupStore.GetVirtualGroupId(groupIdOverrides, new Guid("EBC7BCED-5A90-440D-824C-37EEA5BCD643"), domainId),
          Descriptor = mapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup),
          DomainId = domainId,
          ProviderDisplayName = FrameworkResources.AdministratorsGroupName(),
          Description = FrameworkResources.AdministratorsGroupDescription(),
          SpecialType = "AdministrativeApplicationGroup"
        },
        [GroupWellKnownIdentityDescriptors.ServiceUsersGroup] = new VirtualizedGroupStore.WellKnownGroupInformation()
        {
          Id = VirtualizedGroupStore.GetVirtualGroupId(groupIdOverrides, new Guid("D1C6F3F6-2BFE-46B9-92A3-33EF9F753C0A"), domainId),
          Descriptor = mapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.ServiceUsersGroup),
          DomainId = domainId,
          ProviderDisplayName = FrameworkResources.ServiceGroupName(),
          Description = FrameworkResources.ServiceGroupDescription(),
          SpecialType = "ServiceApplicationGroup"
        },
        [GroupWellKnownIdentityDescriptors.EveryoneGroup] = new VirtualizedGroupStore.WellKnownGroupInformation()
        {
          Id = VirtualizedGroupStore.GetVirtualGroupId(groupIdOverrides, new Guid("01CC4EB1-2A9B-47CD-A55C-5CDBD22DC0AF"), domainId),
          Descriptor = mapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup),
          DomainId = domainId,
          ProviderDisplayName = FrameworkResources.ValidUsersGroupName(),
          Description = FrameworkResources.ValidUsersGroupDescription(),
          SpecialType = "EveryoneApplicationGroup"
        },
        [GroupWellKnownIdentityDescriptors.ServicePrincipalGroup] = new VirtualizedGroupStore.WellKnownGroupInformation()
        {
          Id = VirtualizedGroupStore.GetVirtualGroupId(groupIdOverrides, new Guid("D501B9D3-60C2-49C0-82FD-831772373448"), domainId),
          Descriptor = mapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.ServicePrincipalGroup),
          DomainId = domainId,
          ProviderDisplayName = "Azure Active Directory Service Principals",
          Description = "This group includes service principals defined in Azure Active Directory.",
          SpecialType = "Generic"
        },
        [GroupWellKnownIdentityDescriptors.Proxy.ServiceAccounts] = new VirtualizedGroupStore.WellKnownGroupInformation()
        {
          Id = VirtualizedGroupStore.GetVirtualGroupId(groupIdOverrides, new Guid("747A885A-9368-455B-8A11-63BF5783AD4A"), domainId),
          Descriptor = mapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.Proxy.ServiceAccounts),
          DomainId = domainId,
          ProviderDisplayName = "DevOps Proxy Service Accounts",
          Description = "This group should only include service accounts used by Azure DevOps Server Proxy.",
          SpecialType = "Generic"
        },
        [GroupWellKnownIdentityDescriptors.FeatureAvailability.AdminUsersGroup] = new VirtualizedGroupStore.WellKnownGroupInformation()
        {
          Id = VirtualizedGroupStore.GetVirtualGroupId(groupIdOverrides, new Guid("7DFC7B66-88DA-4F5F-B72B-C91D05B07F9F"), domainId),
          Descriptor = mapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.FeatureAvailability.AdminUsersGroup),
          DomainId = domainId,
          ProviderDisplayName = FrameworkResources.FeatureAvailabilityAdminUsersGroupName(),
          Description = FrameworkResources.FeatureAvailabilityAdminUsersGroupName(),
          SpecialType = "Generic"
        },
        [GroupWellKnownIdentityDescriptors.FeatureAvailability.AccountAdminUsersGroup] = new VirtualizedGroupStore.WellKnownGroupInformation()
        {
          Id = VirtualizedGroupStore.GetVirtualGroupId(groupIdOverrides, new Guid("51E3106B-8A0C-46DB-9544-49EA35C45E44"), domainId),
          Descriptor = mapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.FeatureAvailability.AccountAdminUsersGroup),
          DomainId = domainId,
          ProviderDisplayName = FrameworkResources.FeatureAvailabilityAccountAdminUsersGroupName(),
          Description = FrameworkResources.FeatureAvailabilityAccountAdminUsersGroupName(),
          SpecialType = "Generic"
        },
        [GroupWellKnownIdentityDescriptors.FeatureAvailability.ReadersUsersGroup] = new VirtualizedGroupStore.WellKnownGroupInformation()
        {
          Id = VirtualizedGroupStore.GetVirtualGroupId(groupIdOverrides, new Guid("4ADAC68E-D733-43E2-AB0A-A29A08B863E7"), domainId),
          Descriptor = mapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.FeatureAvailability.ReadersUsersGroup),
          DomainId = domainId,
          ProviderDisplayName = FrameworkResources.FeatureAvailabilityReadersUsersGroupName(),
          Description = FrameworkResources.FeatureAvailabilityReadersUsersGroupName(),
          SpecialType = "Generic"
        }
      };
    }

    private static Guid GetVirtualGroupId(
      IDictionary<Guid, Guid> groupIdOverrides,
      Guid groupWellKnownId,
      Guid domainId)
    {
      Guid virtualGroupId;
      if (!groupIdOverrides.TryGetValue(groupWellKnownId, out virtualGroupId))
        virtualGroupId = VirtualizedGroupStore.XorGuids(groupWellKnownId, domainId);
      return virtualGroupId;
    }

    private static unsafe Guid XorGuids(Guid x, Guid y)
    {
      int* numPtr1 = (int*) &x;
      int* numPtr2 = (int*) &y;
      int* numPtr3 = numPtr1;
      int* numPtr4 = (int*) ((IntPtr) numPtr3 + 4);
      int num1 = *numPtr3;
      int* numPtr5 = numPtr2;
      int* numPtr6 = (int*) ((IntPtr) numPtr5 + 4);
      int num2 = *numPtr5;
      int num3 = num1 ^ num2;
      *numPtr3 = num3;
      int* numPtr7 = numPtr4;
      int* numPtr8 = (int*) ((IntPtr) numPtr7 + 4);
      int num4 = *numPtr7;
      int* numPtr9 = numPtr6;
      int* numPtr10 = (int*) ((IntPtr) numPtr9 + 4);
      int num5 = *numPtr9;
      int num6 = num4 ^ num5;
      *numPtr7 = num6;
      int* numPtr11 = numPtr8;
      int* numPtr12 = (int*) ((IntPtr) numPtr11 + 4);
      int num7 = *numPtr11;
      int* numPtr13 = numPtr10;
      int* numPtr14 = (int*) ((IntPtr) numPtr13 + 4);
      int num8 = *numPtr13;
      int num9 = num7 ^ num8;
      *numPtr11 = num9;
      int* numPtr15 = numPtr12;
      int num10 = *numPtr15 ^ *numPtr14;
      *numPtr15 = num10;
      return x;
    }

    protected struct WellKnownGroupInformation
    {
      public Guid Id;
      public IdentityDescriptor Descriptor;
      public Guid DomainId;
      public string ProviderDisplayName;
      public string Description;
      public string SpecialType;

      public Microsoft.VisualStudio.Services.Identity.Identity GetIdentity()
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
        identity.CustomDisplayName = (string) null;
        identity.Descriptor = this.Descriptor;
        identity.ProviderDisplayName = "[TEAM FOUNDATION]\\" + this.ProviderDisplayName;
        identity.Id = this.Id;
        identity.IsActive = true;
        identity.IsContainer = true;
        identity.MasterId = this.Id;
        identity.SetProperty("SchemaClassName", (object) "Group");
        identity.SetProperty("Description", (object) this.Description);
        identity.SetProperty("Domain", (object) ("vstfs:///Framework/IdentityDomain/" + this.DomainId.ToString("D")));
        identity.SetProperty("Account", (object) this.ProviderDisplayName);
        identity.SetProperty("SecurityGroup", (object) "SecurityGroup");
        identity.SetProperty("SpecialType", (object) this.SpecialType);
        identity.SetProperty("ScopeId", (object) this.DomainId);
        identity.SetProperty("ScopeType", (object) "ServiceHost");
        identity.SetProperty("LocalScopeId", (object) this.DomainId);
        identity.SetProperty("SecuringHostId", (object) this.DomainId);
        identity.SetProperty("ScopeName", (object) "TEAM FOUNDATION");
        identity.SetProperty("GlobalScope", (object) "GlobalScope");
        identity.ResetModifiedProperties();
        return identity;
      }
    }
  }
}
