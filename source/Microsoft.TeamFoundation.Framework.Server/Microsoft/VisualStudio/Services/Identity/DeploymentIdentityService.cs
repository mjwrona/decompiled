// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.DeploymentIdentityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class DeploymentIdentityService : 
    IdentityService,
    IUserIdentityService,
    ISystemIdentityService,
    IVssFrameworkService,
    IIdentityServiceInternalRestricted,
    IIdentityServiceInternal,
    IIdentityServiceInternalRequired,
    IInstallableIdentityService,
    IDisposable
  {
    private IdentityDomain m_domain;
    private VirtualizedGroupStore m_virtualizedGroupStore;
    private IDeploymentUserIdentityStore m_userStore;
    private Dictionary<string, IIdentityProvider> m_syncAgents;
    private Lazy<IList<IVssFrameworkService>> m_initList = new Lazy<IList<IVssFrameworkService>>((Func<IList<IVssFrameworkService>>) (() => (IList<IVssFrameworkService>) new List<IVssFrameworkService>()), LazyThreadSafetyMode.PublicationOnly);
    private IDisposableReadOnlyList<IIdentityProviderExtension> m_extensionIdentityProviders;
    private static readonly string[] SupportedIdentityProperties = ((IEnumerable<string>) IdentityPropertiesCache.DefaultExtendedPropertiesCacheWhitelist).Concat<string>((IEnumerable<string>) new string[7]
    {
      "DirectoryAlias",
      "MetadataUpdateDate",
      "UserId",
      "AuthenticationCredentialValidFrom",
      "NotificationSubscriberDeliveryPreference",
      "NotificationSubscriberPreferrredEmailAddress",
      "*"
    }).ToArray<string>();
    private const string s_Area = "Identity";
    private const string s_Layer = "DeploymentIdentityService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      DeploymentIdentityService.ValidateRequestContext(systemRequestContext);
      this.m_domain = new IdentityDomain(systemRequestContext);
      this.m_virtualizedGroupStore = new VirtualizedGroupStore(systemRequestContext, this.m_domain.DomainId, this.m_domain.IdentityMapper);
      this.m_userStore = systemRequestContext.GetService<IDeploymentUserIdentityStore>();
      this.m_syncAgents = new Dictionary<string, IIdentityProvider>();
      IList<IIdentityProvider> providers = (IList<IIdentityProvider>) new List<IIdentityProvider>()
      {
        (IIdentityProvider) new BindPendingProvider(),
        (IIdentityProvider) new ImportedIdentityProvider(),
        (IIdentityProvider) new UnauthenticatedProvider(),
        (IIdentityProvider) new ServiceIdentityProvider(),
        (IIdentityProvider) new AggregateIdentityProvider(),
        (IIdentityProvider) new ClaimsProvider(),
        (IIdentityProvider) new ServicePrincipalProvider(),
        (IIdentityProvider) new CspPartnerIdentityProvider()
      };
      this.ResolveSyncAgents(systemRequestContext, (IEnumerable<IIdentityProvider>) providers);
      this.m_extensionIdentityProviders = systemRequestContext.GetExtensions<IIdentityProviderExtension>();
      this.ResolveSyncAgents(systemRequestContext, (IEnumerable<IIdentityProvider>) this.m_extensionIdentityProviders);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!this.m_initList.IsValueCreated)
        return;
      foreach (IVssFrameworkService frameworkService in (IEnumerable<IVssFrameworkService>) this.m_initList.Value)
        frameworkService.ServiceEnd(systemRequestContext);
    }

    void IDisposable.Dispose()
    {
      if (this.m_extensionIdentityProviders == null)
        return;
      this.m_extensionIdentityProviders.Dispose();
      this.m_extensionIdentityProviders = (IDisposableReadOnlyList<IIdentityProviderExtension>) null;
    }

    public void Install(IVssRequestContext requestContext)
    {
    }

    public void Uninstall(IVssRequestContext requestContext, IdentityDomain domain)
    {
    }

    public IdentityScope GetScope(IVssRequestContext requestContext, Guid scopeId) => this.m_virtualizedGroupStore.GetScope(requestContext, scopeId);

    public IdentityScope GetScope(IVssRequestContext requestContext, string scopeName)
    {
      ArgumentUtility.CheckForNull<string>(scopeName, nameof (scopeName));
      return this.m_virtualizedGroupStore.GetScope(requestContext, scopeName);
    }

    public Guid GetScopeParentId(IVssRequestContext requestContext, Guid scopeId) => this.m_virtualizedGroupStore.GetScopeParentId(requestContext, scopeId);

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
      if (scopeId == Guid.Empty)
        scopeId = this.Domain.DomainId;
      TFCommonUtil.CheckGroupName(ref groupName);
      TFCommonUtil.CheckGroupDescription(ref groupDescription);
      return this.m_virtualizedGroupStore.CreateGroup(requestContext, scopeId, groupSid, groupName, groupDescription, specialType, scopeLocal, hasRestrictedVisibility);
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
      if (scopeId == Guid.Empty)
        scopeId = this.Domain.DomainId;
      TFCommonUtil.CheckGroupName(ref groupName);
      TFCommonUtil.CheckGroupDescription(ref groupDescription);
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId));
      return this.m_virtualizedGroupStore.CreateGroup(requestContext, scopeId, groupId, groupSid, groupName, groupDescription, specialType, scopeLocal, hasRestrictedVisibility);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      return this.m_virtualizedGroupStore.ListGroups(requestContext, scopeIds, recurse, propertyNameFilters);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ListDeletedGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      return this.m_virtualizedGroupStore.ListDeletedGroups(requestContext, scopeIds, recurse, propertyNameFilters);
    }

    public bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckTeamFoundationType(groupDescriptor);
      IdentityValidation.CheckDescriptor(memberDescriptor, nameof (memberDescriptor));
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
      return this.m_virtualizedGroupStore.AddMemberToGroup(requestContext, groupDescriptor, memberDescriptor);
    }

    public bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity member)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckTeamFoundationType(groupDescriptor);
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(member, nameof (member));
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
      return this.m_virtualizedGroupStore.AddMemberToGroup(requestContext, groupDescriptor, member);
    }

    public virtual bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckDescriptor(memberDescriptor, nameof (memberDescriptor));
      return this.m_virtualizedGroupStore.IsMember(requestContext, groupDescriptor, memberDescriptor);
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      DeploymentIdentityService.ValidateRequestContext(requestContext);
      if (identityIds.IsNullOrEmpty<Guid>())
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      DeploymentIdentityService.ValidatePropertyFilters(propertyNameFilters);
      if (queryMembership != QueryMembership.None)
        queryMembership = QueryMembership.None;
      IList<Guid> identityIds1 = identityIds;
      bool includeRequestIdentity = requestContext.IsFeatureEnabled("AzureDevOps.Services.RequestContext.UseUserRequestContext");
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = this.m_virtualizedGroupStore.ReadIdentities(requestContext, identityIds1, queryMembership, propertyNameFilters, includeRestrictedVisibility, includeRequestIdentity);
      if (identityList1 != null)
      {
        identityIds1 = (IList<Guid>) new List<Guid>((IEnumerable<Guid>) identityIds);
        for (int index = 0; index < identityList1.Count; ++index)
        {
          if (identityList1[index] != null)
            identityIds1[index] = Guid.Empty;
        }
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = this.m_userStore.ReadIdentities(requestContext, identityIds1);
      if (identityList1 != null)
      {
        for (int index = 0; index < identityList2.Count; ++index)
        {
          if (identityList1[index] != null)
            identityList2[index] = identityList1[index];
        }
      }
      return identityList2;
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      DeploymentIdentityService.ValidateRequestContext(requestContext);
      if (descriptors.IsNullOrEmpty<IdentityDescriptor>())
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      DeploymentIdentityService.ValidatePropertyFilters(propertyNameFilters);
      if (queryMembership != QueryMembership.None)
        queryMembership = QueryMembership.None;
      IdentityDescriptor[] identityDescriptorArray = new IdentityDescriptor[descriptors.Count];
      for (int index = 0; index < descriptors.Count; ++index)
      {
        IdentityValidation.CheckDescriptor(descriptors[index], "descriptors element");
        identityDescriptorArray[index] = descriptors[index];
      }
      bool includeRequestIdentity = requestContext.IsFeatureEnabled("AzureDevOps.Services.RequestContext.UseUserRequestContext");
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = this.m_virtualizedGroupStore.ReadIdentities(requestContext, (IList<IdentityDescriptor>) identityDescriptorArray, queryMembership, propertyNameFilters, includeRestrictedVisibility, includeRequestIdentity);
      for (int index = 0; index < identityDescriptorArray.Length; ++index)
      {
        if (identityDescriptorArray[index].IsTeamFoundationType())
          identityDescriptorArray[index] = (IdentityDescriptor) null;
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = this.m_userStore.ReadIdentities(requestContext, (IList<IdentityDescriptor>) identityDescriptorArray);
      if (identityList1 != null)
      {
        for (int index = 0; index < identityList2.Count; ++index)
        {
          if (identityList1[index] != null)
            identityList2[index] = identityList1[index];
        }
      }
      return identityList2;
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SocialDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      requestContext.Trace(80153, TraceLevel.Error, "Identity", nameof (DeploymentIdentityService), "ReadIdentities by social descriptor API called.");
      throw new NotSupportedException("ReadIdentities by social descriptor API called.");
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      DeploymentIdentityService.ValidateRequestContext(requestContext);
      if (descriptors.IsNullOrEmpty<SubjectDescriptor>())
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      DeploymentIdentityService.ValidatePropertyFilters(propertyNameFilters);
      if (queryMembership != QueryMembership.None)
        queryMembership = QueryMembership.None;
      SubjectDescriptor[] subjectDescriptors = new SubjectDescriptor[descriptors.Count];
      IdentityDescriptor[] descriptors1 = new IdentityDescriptor[descriptors.Count];
      for (int index = 0; index < descriptors.Count; ++index)
      {
        SubjectDescriptor descriptor1 = descriptors[index];
        if (descriptor1.IsVstsGroupType())
        {
          IdentityDescriptor descriptor2 = new IdentityDescriptor("Microsoft.TeamFoundation.Identity", descriptor1.Identifier);
          IdentityValidation.CheckDescriptor(descriptor2, "descriptors element");
          descriptors1[index] = descriptor2;
        }
        else
          subjectDescriptors[index] = descriptor1.IsCuidBased() ? descriptor1 : throw new NotSupportedException("Unsupported subject type: " + (string) descriptor1);
      }
      bool includeRequestIdentity = requestContext.IsFeatureEnabled("AzureDevOps.Services.RequestContext.UseUserRequestContext");
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = this.m_virtualizedGroupStore.ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors1, queryMembership, propertyNameFilters, includeRestrictedVisibility, includeRequestIdentity);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = this.m_userStore.ReadIdentities(requestContext, (IList<SubjectDescriptor>) subjectDescriptors);
      if (identityList1 != null)
      {
        for (int index = 0; index < identityList2.Count; ++index)
        {
          if (identityList1[index] != null)
            identityList2[index] = identityList1[index];
        }
      }
      return identityList2;
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ReadIdentities(requestContext, searchFactor, factorValue, queryMembership, propertyNameFilters, ReadIdentitiesOptions.None);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      ReadIdentitiesOptions options)
    {
      DeploymentIdentityService.ValidateRequestContext(requestContext);
      IdentityValidation.CheckFactorAndValue(searchFactor, ref factorValue);
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      DeploymentIdentityService.ValidatePropertyFilters(propertyNameFilters);
      if (queryMembership != QueryMembership.None)
        queryMembership = QueryMembership.None;
      if (options != ReadIdentitiesOptions.None)
        throw new NotSupportedException();
      requestContext.Trace(930532, TraceLevel.Info, "Identity", nameof (DeploymentIdentityService), "Search factor reads are not supported at the deployment level. SearchFactor: {0}. FactorValue: {1}", (object) searchFactor, (object) factorValue);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadRequestIdentity(
      IVssRequestContext requestContext)
    {
      return requestContext.UserContext != (IdentityDescriptor) null ? requestContext.ReadIdentity(requestContext.UserContext) : (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadRequestIdentity(
      IVssRequestContext requestContext,
      IEnumerable<string> propertyNameFilters)
    {
      DeploymentIdentityService.ValidateRequestContext(requestContext);
      DeploymentIdentityService.ValidatePropertyFilters(propertyNameFilters);
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadRequestIdentity(requestContext);
      if (propertyNameFilters != null && propertyNameFilters.Contains<string>("UserId"))
      {
        identity = identity.Clone();
        identity.SetProperty("UserId", (object) identity.Id);
      }
      return identity;
    }

    public virtual void CreateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      DeploymentIdentityService.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckForNull<IList<Microsoft.VisualStudio.Services.Identity.Identity>>(identities, nameof (identities));
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identity1;
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, "identity");
        string property = identity.GetProperty<string>("Alias", (string) null);
        if (!string.IsNullOrEmpty(property))
          IdentityValidation.CheckAlias(ref property);
        if (!identity.Descriptor.IsCuidBased())
        {
          requestContext.TraceDataConditionally(41522234, TraceLevel.Verbose, "Identity", nameof (DeploymentIdentityService), "Unsupported descriptor", (Func<object>) (() => (object) new
          {
            identity = identity
          }), nameof (CreateIdentities));
          throw new NotSupportedException(string.Format("Cannot create identity with unsupported descriptor '{0}'.", (object) identity.Descriptor));
        }
      }
      this.m_userStore.CreateIdentities(requestContext, identities);
    }

    public virtual bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      return this.UpdateIdentities(requestContext, identities, false);
    }

    public virtual bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates)
    {
      DeploymentIdentityService.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckForNull<IList<Microsoft.VisualStudio.Services.Identity.Identity>>(identities, nameof (identities));
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identity1;
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, "identity");
        string property = identity.GetProperty<string>("Alias", (string) null);
        if (!string.IsNullOrEmpty(property))
          IdentityValidation.CheckAlias(ref property);
        if (!identity.Descriptor.IsCuidBased())
        {
          requestContext.TraceDataConditionally(41522236, TraceLevel.Verbose, "Identity", nameof (DeploymentIdentityService), "Unsupported descriptor", (Func<object>) (() => (object) new
          {
            identity = identity
          }), nameof (UpdateIdentities));
          throw new NotSupportedException(string.Format("Cannot create identity with unsupported descriptor '{0}'.", (object) identity.Descriptor));
        }
        if (identity.SubjectDescriptor == new SubjectDescriptor())
          identity.SubjectDescriptor = new SubjectDescriptor(identity.GetSubjectTypeForCuidBasedIdentity(requestContext), identity.Cuid().ToString("D"));
      }
      return this.m_userStore.UpdateIdentities(requestContext, identities, allowMetadataUpdates);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      string mailAddress = null)
    {
      throw new NotImplementedException();
    }

    public void ClearIdentityCache(IVssRequestContext requestContext) => throw new NotImplementedException();

    public void InvalidateIdentities(
      IVssRequestContext requestContext,
      ICollection<Guid> identityIds)
    {
      throw new NotImplementedException();
    }

    public IdentityScope CreateScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid parentScopeId,
      GroupScopeType scopeType,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription,
      Guid creatorId)
    {
      throw new NotImplementedException();
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateUser(
      IVssRequestContext requestContext,
      Guid scopeId,
      string userSid,
      string domainName,
      string accountName,
      string description)
    {
      throw new NotImplementedException();
    }

    public void DeleteGroup(IVssRequestContext requestContext, IdentityDescriptor groupDescriptor) => throw new NotImplementedException();

    public void DeleteScope(IVssRequestContext requestContext, Guid scopeId) => throw new NotImplementedException();

    public int GetCurrentSequenceId(IVssRequestContext requestContext) => 1;

    public int GetCurrentChangeId() => 1;

    public ChangedIdentities GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext)
    {
      throw new NotImplementedException();
    }

    public IdentityChanges GetIdentityChanges(
      IVssRequestContext requestContext,
      int sequenceId,
      long identitySequenceId,
      long groupSequenceId,
      long organizationIdentitySequenceId)
    {
      throw new NotImplementedException();
    }

    public string GetSignoutToken(IVssRequestContext requestContext) => throw new NotImplementedException();

    public bool LastUserAccessUpdateScheduled(IVssRequestContext requestContext, Guid identityId) => throw new NotImplementedException();

    public FilteredIdentitiesList ReadFilteredIdentities(
      IVssRequestContext requestContext,
      Guid scopeId,
      IList<IdentityDescriptor> descriptors,
      IEnumerable<IdentityFilter> filters,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      throw new NotImplementedException();
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IVssRequestContext requestContext,
      IList<string> accountNames,
      QueryMembership queryMembership)
    {
      throw new NotImplementedException();
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership)
    {
      throw new NotImplementedException();
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadSystemSubjectIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      throw new NotImplementedException();
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadSystemSubjectIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      throw new NotImplementedException();
    }

    public bool RefreshIdentity(IVssRequestContext requestContext, IdentityDescriptor descriptor) => throw new NotImplementedException();

    public void RefreshSearchIdentitiesCache(IVssRequestContext requestContext, Guid scopeId) => throw new NotImplementedException();

    public bool RemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      throw new NotImplementedException();
    }

    public bool ForceRemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      throw new NotImplementedException();
    }

    public void RenameScope(IVssRequestContext requestContext, Guid scopeId, string newName) => throw new NotImplementedException();

    public void RestoreScope(IVssRequestContext requestContext, Guid scopeId) => throw new NotImplementedException();

    public void ScheduleLastUserAccessUpdate(IVssRequestContext requestContext, Guid identityId) => throw new NotImplementedException();

    public IdentitySearchResult SearchIdentities(
      IVssRequestContext requestContext,
      IdentitySearchParameters searchParameters)
    {
      throw new NotImplementedException();
    }

    public int UpgradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      throw new NotImplementedException();
    }

    public int DowngradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      throw new NotImplementedException();
    }

    private void ResolveSyncAgents(
      IVssRequestContext requestContext,
      IEnumerable<IIdentityProvider> providers)
    {
      foreach (IIdentityProvider provider in providers)
      {
        if (provider is IVssFrameworkService frameworkService)
        {
          frameworkService.ServiceStart(requestContext);
          this.m_initList.Value.Add(frameworkService);
        }
        foreach (string supportedIdentityType in provider.SupportedIdentityTypes())
        {
          IIdentityProvider identityProvider;
          if (this.SyncAgents.TryGetValue(supportedIdentityType, out identityProvider))
          {
            if (identityProvider is IIdentityProviderExtension)
              requestContext.Trace(80151, TraceLevel.Warning, "Identity", nameof (DeploymentIdentityService), "Skipping provider {0} for identity type {1} because provider {2} is already set", (object) provider.GetType().FullName, (object) supportedIdentityType, (object) identityProvider.GetType().FullName);
            else if (provider is IIdentityProviderExtension)
            {
              requestContext.Trace(80151, TraceLevel.Warning, "Identity", nameof (DeploymentIdentityService), "Overriding provider {0} for identity type {1} because provider {2} is built-in.", (object) identityProvider.GetType().FullName, (object) supportedIdentityType, (object) provider.GetType().FullName);
              this.SyncAgents[supportedIdentityType] = provider;
            }
            else
            {
              string str = "Bad Configuration -- Multiple built in providers for the same identity type: " + supportedIdentityType + ", previousProvider: " + identityProvider.GetType().FullName + ", currentProvider: " + provider.GetType().FullName;
              requestContext.Trace(80151, TraceLevel.Error, "Identity", nameof (DeploymentIdentityService), str);
              throw new InvalidConfigurationException(str);
            }
          }
          else
          {
            requestContext.Trace(80152, TraceLevel.Info, "Identity", nameof (DeploymentIdentityService), "Setting provider {0} for identity type {1}", (object) provider.GetType().FullName, (object) supportedIdentityType);
            this.SyncAgents[supportedIdentityType] = provider;
          }
        }
      }
    }

    private static void ValidateRequestContext(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      requestContext.CheckDeploymentRequestContext();
    }

    private static void ValidatePropertyFilters(IEnumerable<string> propertyNameFilters)
    {
      if (propertyNameFilters != null && propertyNameFilters.Any<string>() && propertyNameFilters.Except<string>((IEnumerable<string>) DeploymentIdentityService.SupportedIdentityProperties).Any<string>())
        throw new NotSupportedException();
    }

    protected void FireDescriptorsChanged(object sender, DescriptorChangeEventArgs e) => this.DescriptorsChanged((object) this, e);

    protected void FireDescriptorsChangedNotification(
      object sender,
      DescriptorChangeNotificationEventArgs e)
    {
      this.DescriptorsChangedNotification((object) this, e);
    }

    public Guid DomainId => this.m_domain.DomainId;

    public IdentityMapper IdentityMapper => this.m_domain.IdentityMapper;

    public IDictionary<string, IIdentityProvider> SyncAgents => (IDictionary<string, IIdentityProvider>) this.m_syncAgents;

    public IdentityDomain Domain => this.m_domain;

    public event EventHandler<DescriptorChangeEventArgs> DescriptorsChanged;

    public event EventHandler<DescriptorChangeNotificationEventArgs> DescriptorsChangedNotification;
  }
}
