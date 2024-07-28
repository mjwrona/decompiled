// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasicIdentityService
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
  public class BasicIdentityService : 
    IdentityService,
    IUserIdentityService,
    ISystemIdentityService,
    IVssFrameworkService,
    IIdentityServiceInternalRestricted,
    IIdentityServiceInternal,
    IIdentityServiceInternalRequired,
    IInstallableIdentityService
  {
    private VirtualizedGroupStore m_virtualizedGroupStore;
    private Guid m_domainId;
    private IdentityMapper m_mapper;
    private IDisposableReadOnlyList<IIdentityProviderExtension> m_identityProviders;
    private Dictionary<string, IIdentityProvider> m_syncAgents;
    private const string c_area = "IdentityService";
    private const string c_layer = "BasicIdentityService";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckHostedDeployment();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.m_domainId = requestContext.ServiceHost.InstanceId;
      this.m_mapper = new IdentityMapper(this.m_domainId);
      this.m_virtualizedGroupStore = new VirtualizedGroupStore(requestContext, this.m_domainId, this.m_mapper);
      this.m_syncAgents = new Dictionary<string, IIdentityProvider>();
      this.LoadProvidersForOtherProviderTypes(requestContext, service);
      this.LoadProvidersForAdditionalIdentityTypes(requestContext, service);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      if (this.m_identityProviders == null)
        return;
      this.m_identityProviders.Dispose();
      this.m_identityProviders = (IDisposableReadOnlyList<IIdentityProviderExtension>) null;
    }

    private void LoadProvidersForAdditionalIdentityTypes(
      IVssRequestContext requestContext,
      IVssRegistryService registryService)
    {
      string str = registryService.GetValue(requestContext, new RegistryQuery("/Configuration/SupportedIdentityTypes/", false), (string) null);
      if (string.IsNullOrEmpty(str))
        return;
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) str.Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries));
      this.m_identityProviders = requestContext.GetExtensions<IIdentityProviderExtension>();
      foreach (IIdentityProvider identityProvider in (IEnumerable<IIdentityProviderExtension>) this.m_identityProviders)
      {
        foreach (string supportedIdentityType in identityProvider.SupportedIdentityTypes())
        {
          if (stringSet.Contains(supportedIdentityType) && !this.m_syncAgents.ContainsKey(supportedIdentityType))
            this.m_syncAgents.Add(supportedIdentityType, identityProvider);
        }
      }
    }

    protected virtual void LoadProvidersForOtherProviderTypes(
      IVssRequestContext requestContext,
      IVssRegistryService registryService)
    {
      IIdentityProvider identityProvider = (IIdentityProvider) new ClaimsProvider();
      foreach (string supportedIdentityType in identityProvider.SupportedIdentityTypes())
        this.SyncAgents[supportedIdentityType] = identityProvider;
    }

    public Guid DomainId => this.m_domainId;

    public IdentityMapper IdentityMapper => this.m_mapper;

    public IDictionary<string, IIdentityProvider> SyncAgents => (IDictionary<string, IIdentityProvider>) this.m_syncAgents;

    public IdentityDomain Domain => throw new NotImplementedException();

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

    public void DeleteScope(IVssRequestContext requestContext, Guid scopeId) => throw new NotImplementedException();

    public IdentityScope GetScope(IVssRequestContext requestContext, string scopeName) => this.m_virtualizedGroupStore.GetScope(requestContext, scopeName);

    public IdentityScope GetScope(IVssRequestContext requestContext, Guid scopeId) => this.m_virtualizedGroupStore.GetScope(requestContext, scopeId);

    Guid IIdentityServiceInternalRequired.GetScopeParentId(
      IVssRequestContext requestContext,
      Guid scopeId)
    {
      return this.m_virtualizedGroupStore.GetScopeParentId(requestContext, scopeId);
    }

    public void RenameScope(IVssRequestContext requestContext, Guid scopeId, string newName) => throw new NotImplementedException();

    public void RestoreScope(IVssRequestContext requestContext, Guid scopeId) => throw new NotImplementedException();

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
      return this.m_virtualizedGroupStore.CreateGroup(requestContext, scopeId, groupId, groupSid, groupName, groupDescription, specialType, scopeLocal, hasRestrictedVisibility);
    }

    public void DeleteGroup(IVssRequestContext requestContext, IdentityDescriptor groupDescriptor) => throw new NotImplementedException();

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
      return this.m_virtualizedGroupStore.AddMemberToGroup(requestContext, groupDescriptor, memberDescriptor);
    }

    public bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity member)
    {
      return this.m_virtualizedGroupStore.AddMemberToGroup(requestContext, groupDescriptor, member);
    }

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

    public virtual bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      if (identities != null && identities.Count == 1 && identities[0].Id == this.ServiceInstanceType(requestContext))
        return true;
      throw new NotImplementedException();
    }

    public virtual bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates)
    {
      if (identities != null && identities.Count == 1 && identities[0].Id == this.ServiceInstanceType(requestContext))
        return true;
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

    public string GetSignoutToken(IVssRequestContext requestContext) => throw new NotImplementedException();

    int IIdentityServiceInternalRequired.GetCurrentSequenceId(IVssRequestContext requestContext) => 1;

    int IIdentityServiceInternalRequired.GetCurrentChangeId() => 1;

    public void ClearIdentityCache(IVssRequestContext requestContext) => throw new NotImplementedException();

    protected virtual Guid ServiceInstanceType(IVssRequestContext requestContext) => requestContext.ServiceInstanceType();

    public virtual bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      return this.m_virtualizedGroupStore.IsMember(requestContext, groupDescriptor, memberDescriptor);
    }

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

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      throw new NotImplementedException();
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      if (propertyNameFilters != null && propertyNameFilters.Any<string>())
        throw new NotImplementedException();
      if (queryMembership != QueryMembership.None)
        throw new NotImplementedException();
      return this.m_virtualizedGroupStore.ReadIdentities(requestContext, identityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility, true);
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

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      if (propertyNameFilters != null && propertyNameFilters.Any<string>())
        throw new NotImplementedException();
      if (queryMembership != QueryMembership.None)
        throw new NotImplementedException();
      return this.m_virtualizedGroupStore.ReadIdentities(requestContext, descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility, true);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      requestContext.TraceEnter(422694, "IdentityService", nameof (BasicIdentityService), nameof (ReadIdentities));
      try
      {
        if (propertyNameFilters != null && propertyNameFilters.Any<string>())
          throw new NotImplementedException();
        if (queryMembership != QueryMembership.None)
          throw new NotImplementedException();
        if (subjectDescriptors == null)
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
        IdentityDescriptor[] descriptors = new IdentityDescriptor[subjectDescriptors.Count];
        for (int index = 0; index < subjectDescriptors.Count; ++index)
        {
          SubjectDescriptor subjectDescriptor = subjectDescriptors[index];
          if (subjectDescriptor.IsVstsGroupType())
          {
            IdentityDescriptor identityDescriptor = subjectDescriptor.ToIdentityDescriptor(requestContext);
            IdentityValidation.CheckDescriptor(identityDescriptor, "descriptors element");
            descriptors[index] = identityDescriptor;
          }
        }
        return this.m_virtualizedGroupStore.ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(380386, "IdentityService", nameof (BasicIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(361918, "IdentityService", nameof (BasicIdentityService), nameof (ReadIdentities));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SocialDescriptor> socialDescriptors,
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

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      ReadIdentitiesOptions options)
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

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IVssRequestContext requestContext,
      IList<string> accountNames,
      QueryMembership queryMembership)
    {
      throw new NotImplementedException();
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
      if (propertyNameFilters != null && propertyNameFilters.Any<string>())
        throw new NotImplementedException();
      return this.ReadRequestIdentity(requestContext);
    }

    public IdentitySearchResult SearchIdentities(
      IVssRequestContext requestContext,
      IdentitySearchParameters searchParameters)
    {
      throw new NotImplementedException();
    }

    public void Install(IVssRequestContext requestContext)
    {
    }

    public void Uninstall(IVssRequestContext requestContext, IdentityDomain domain)
    {
    }

    public void InvalidateIdentities(
      IVssRequestContext requestContext,
      ICollection<Guid> identityIds)
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

    public bool RefreshIdentity(IVssRequestContext requestContext, IdentityDescriptor descriptor) => throw new NotImplementedException();

    public bool LastUserAccessUpdateScheduled(IVssRequestContext requestContext, Guid identityId) => throw new NotImplementedException();

    public void ScheduleLastUserAccessUpdate(IVssRequestContext requestContext, Guid identityId) => throw new NotImplementedException();

    public void RefreshSearchIdentitiesCache(IVssRequestContext requestContext, Guid scopeId) => throw new NotImplementedException();

    protected void FireDescriptorsChanged(object sender, DescriptorChangeEventArgs e) => this.DescriptorsChanged((object) this, e);

    protected void FireDescriptorsChangedNotification(
      object sender,
      DescriptorChangeNotificationEventArgs e)
    {
      this.DescriptorsChangedNotification((object) this, e);
    }

    public event EventHandler<DescriptorChangeEventArgs> DescriptorsChanged;

    public event EventHandler<DescriptorChangeNotificationEventArgs> DescriptorsChangedNotification;
  }
}
