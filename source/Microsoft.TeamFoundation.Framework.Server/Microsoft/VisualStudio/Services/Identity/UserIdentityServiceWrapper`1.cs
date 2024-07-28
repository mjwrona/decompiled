// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.UserIdentityServiceWrapper`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal abstract class UserIdentityServiceWrapper<TFallbackIdentityService> : 
    IdentityService,
    IUserIdentityService,
    ISystemIdentityService,
    IVssFrameworkService,
    IIdentityServiceInternalRestricted,
    IIdentityServiceInternal,
    IIdentityServiceInternalRequired,
    IInstallableIdentityService
    where TFallbackIdentityService : class, IdentityService, IIdentityServiceInternalRestricted, IInstallableIdentityService
  {
    private IdentityService m_identityService;
    private EventHandler<DescriptorChangeEventArgs> m_descriptorChanged;
    private EventHandler<DescriptorChangeNotificationEventArgs> m_descriptorChangedNotification;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>();
      this.Initialize(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>();
      if (!(this.m_identityService is IIdentityServiceInternal))
        return;
      ((IIdentityServiceInternal) this.m_identityService).DescriptorsChanged -= new EventHandler<DescriptorChangeEventArgs>(this.OnDescriptorsChanged);
      ((IIdentityServiceInternal) this.m_identityService).DescriptorsChangedNotification -= new EventHandler<DescriptorChangeNotificationEventArgs>(this.OnDescriptorsChangedNotification);
    }

    public void Install(IVssRequestContext requestContext)
    {
      if (!(this.m_identityService is IInstallableIdentityService identityService))
        return;
      identityService.Install(requestContext);
    }

    public void Uninstall(IVssRequestContext requestContext, IdentityDomain domain)
    {
      if (!(this.m_identityService is IInstallableIdentityService identityService))
        return;
      identityService.Uninstall(requestContext, domain);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadRequestIdentity(
      IVssRequestContext requestContext)
    {
      return this.m_identityService.ReadRequestIdentity(requestContext);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadRequestIdentity(
      IVssRequestContext requestContext,
      IEnumerable<string> propertyNameFilters)
    {
      return this.m_identityService.ReadRequestIdentity(requestContext, propertyNameFilters);
    }

    public string GetSignoutToken(IVssRequestContext requestContext) => this.m_identityService.GetSignoutToken(requestContext);

    public IdentitySearchResult SearchIdentities(
      IVssRequestContext requestContext,
      IdentitySearchParameters searchParameters)
    {
      return this.m_identityService.SearchIdentities(requestContext, searchParameters);
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
      return this.m_identityService.ReadFilteredIdentities(requestContext, scopeId, descriptors, filters, suggestedPageSize, lastSearchResult, lookForward, queryMembership, propertyNameFilters);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IVssRequestContext requestContext,
      IList<string> accountNames,
      QueryMembership queryMembership)
    {
      return this.m_identityService.ReadIdentitiesFromSource(requestContext, accountNames, queryMembership);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership)
    {
      return this.m_identityService.ReadIdentitiesFromSource(requestContext, descriptors, queryMembership);
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
      return this.m_identityService.CreateScope(requestContext, scopeId, parentScopeId, scopeType, scopeName, adminGroupName, adminGroupDescription, creatorId);
    }

    public IdentityScope GetScope(IVssRequestContext requestContext, Guid scopeId) => this.m_identityService.GetScope(requestContext, scopeId);

    public IdentityScope GetScope(IVssRequestContext requestContext, string scopeName) => this.m_identityService.GetScope(requestContext, scopeName);

    public void DeleteScope(IVssRequestContext requestContext, Guid scopeId) => this.m_identityService.DeleteScope(requestContext, scopeId);

    public void RenameScope(IVssRequestContext requestContext, Guid scopeId, string newName) => this.m_identityService.RenameScope(requestContext, scopeId, newName);

    public void RestoreScope(IVssRequestContext requestContext, Guid scopeId) => this.m_identityService.RestoreScope(requestContext, scopeId);

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
      return this.m_identityService.CreateGroup(requestContext, scopeId, groupSid, groupName, groupDescription, specialType, scopeLocal, hasRestrictedVisibility);
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
      return this.m_identityService.CreateGroup(requestContext, scopeId, groupId, groupSid, groupName, groupDescription, specialType, scopeLocal, hasRestrictedVisibility);
    }

    public void DeleteGroup(IVssRequestContext requestContext, IdentityDescriptor groupDescriptor) => this.m_identityService.DeleteGroup(requestContext, groupDescriptor);

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ListDeletedGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      return this.m_identityService.ListDeletedGroups(requestContext, scopeIds, recurse, propertyNameFilters);
    }

    public bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity member)
    {
      return this.m_identityService.AddMemberToGroup(requestContext, groupDescriptor, member);
    }

    public bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      return this.m_identityService.AddMemberToGroup(requestContext, groupDescriptor, memberDescriptor);
    }

    public bool RemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      return this.m_identityService.RemoveMemberFromGroup(requestContext, groupDescriptor, memberDescriptor);
    }

    public bool ForceRemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      return this.m_identityService.ForceRemoveMemberFromGroup(requestContext, groupDescriptor, memberDescriptor);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      string mailAddress = null)
    {
      return this.m_identityService.CreateFrameworkIdentity(requestContext, identityType, role, identifier, displayName, mailAddress);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      return this.m_identityService.ReadIdentities(requestContext, searchFactor, factorValue, queryMembership, propertyNameFilters);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      ReadIdentitiesOptions options)
    {
      return this.m_identityService.ReadIdentities(requestContext, searchFactor, factorValue, queryMembership, propertyNameFilters, options);
    }

    public bool UpdateIdentities(IVssRequestContext requestContext, IList<Microsoft.VisualStudio.Services.Identity.Identity> identities) => this.m_identityService.UpdateIdentities(requestContext, identities);

    public virtual bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates)
    {
      return this.m_identityService.UpdateIdentities(requestContext, identities, allowMetadataUpdates);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      return this.m_identityService.ListGroups(requestContext, scopeIds, recurse, propertyNameFilters);
    }

    public bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      return this.m_identityService.IsMember(requestContext, groupDescriptor, memberDescriptor);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      return this.m_identityService.ReadIdentities(requestContext, descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      return this.m_identityService.IdentityServiceInternal().ReadIdentities(requestContext, subjectDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      return this.m_identityService.IdentityServiceInternal().ReadIdentities(requestContext, socialDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      return this.m_identityService.ReadIdentities(requestContext, identityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility);
    }

    public void ClearIdentityCache(IVssRequestContext requestContext) => this.m_identityService.IdentityServiceInternalRestricted().ClearIdentityCache(requestContext);

    public void InvalidateIdentities(
      IVssRequestContext requestContext,
      ICollection<Guid> identityIds)
    {
      this.m_identityService.IdentityServiceInternalRestricted().InvalidateIdentities(requestContext, identityIds);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateUser(
      IVssRequestContext requestContext,
      Guid scopeId,
      string userSid,
      string domainName,
      string accountName,
      string description)
    {
      return this.m_identityService.IdentityServiceInternal().CreateUser(requestContext, scopeId, userSid, domainName, accountName, description);
    }

    public ChangedIdentities GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext)
    {
      return this.m_identityService.IdentityServiceInternal().GetIdentityChanges(requestContext, sequenceContext);
    }

    public IdentityChanges GetIdentityChanges(
      IVssRequestContext requestContext,
      int sequenceId,
      long identitySequenceId,
      long groupSequenceId,
      long organizationIdentitySequenceId)
    {
      return this.m_identityService.IdentityServiceInternal().GetIdentityChanges(requestContext, sequenceId, identitySequenceId, groupSequenceId, organizationIdentitySequenceId);
    }

    public bool RefreshIdentity(IVssRequestContext requestContext, IdentityDescriptor descriptor) => this.m_identityService.IdentityServiceInternal().RefreshIdentity(requestContext, descriptor);

    public bool LastUserAccessUpdateScheduled(IVssRequestContext requestContext, Guid identityId) => this.m_identityService.IdentityServiceInternal().LastUserAccessUpdateScheduled(requestContext, identityId);

    public void ScheduleLastUserAccessUpdate(IVssRequestContext requestContext, Guid identityId) => this.m_identityService.IdentityServiceInternal().ScheduleLastUserAccessUpdate(requestContext, identityId);

    public void RefreshSearchIdentitiesCache(IVssRequestContext requestContext, Guid scopeId) => this.m_identityService.IdentityServiceInternal().RefreshSearchIdentitiesCache(requestContext, scopeId);

    public int UpgradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      return this.m_identityService.IdentityServiceInternal().UpgradeIdentitiesToTargetResourceVersion(requestContext, targetResourceVersion, updateBatchSize, maxNumberOfIdentitiesToUpdate);
    }

    public int DowngradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      return this.m_identityService.IdentityServiceInternal().DowngradeIdentitiesToTargetResourceVersion(requestContext, targetResourceVersion, updateBatchSize, maxNumberOfIdentitiesToUpdate);
    }

    public Guid GetScopeParentId(IVssRequestContext requestContext, Guid scopeId) => this.m_identityService.IdentityServiceInternal().GetScopeParentId(requestContext, scopeId);

    public int GetCurrentSequenceId(IVssRequestContext requestContext) => this.m_identityService.IdentityServiceInternal().GetCurrentSequenceId(requestContext);

    public int GetCurrentChangeId() => this.m_identityService.IdentityServiceInternal().GetCurrentChangeId();

    protected virtual IdentityService Initialize(IVssRequestContext requestContext)
    {
      IdentityService identityService = this.m_identityService;
      IdentityService dentityService = !requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? (IdentityService) requestContext.GetService<TFallbackIdentityService>() : (IdentityService) requestContext.GetService<DeploymentIdentityService>();
      if (identityService != null && dentityService != identityService && identityService is IIdentityServiceInternal)
      {
        ((IIdentityServiceInternal) identityService).DescriptorsChanged -= new EventHandler<DescriptorChangeEventArgs>(this.OnDescriptorsChanged);
        ((IIdentityServiceInternal) identityService).DescriptorsChangedNotification -= new EventHandler<DescriptorChangeNotificationEventArgs>(this.OnDescriptorsChangedNotification);
      }
      if (dentityService is IIdentityServiceInternal)
      {
        ((IIdentityServiceInternal) dentityService).DescriptorsChanged += new EventHandler<DescriptorChangeEventArgs>(this.OnDescriptorsChanged);
        ((IIdentityServiceInternal) dentityService).DescriptorsChangedNotification += new EventHandler<DescriptorChangeNotificationEventArgs>(this.OnDescriptorsChangedNotification);
      }
      this.m_identityService = dentityService;
      return dentityService;
    }

    private void OnDescriptorsChangedNotification(
      object sender,
      DescriptorChangeNotificationEventArgs e)
    {
      EventHandler<DescriptorChangeNotificationEventArgs> changedNotification = this.m_descriptorChangedNotification;
      if (changedNotification == null)
        return;
      changedNotification(sender, e);
    }

    private void OnDescriptorsChanged(object sender, DescriptorChangeEventArgs e)
    {
      EventHandler<DescriptorChangeEventArgs> descriptorChanged = this.m_descriptorChanged;
      if (descriptorChanged == null)
        return;
      descriptorChanged(sender, e);
    }

    public event EventHandler<DescriptorChangeEventArgs> DescriptorsChanged
    {
      add => this.m_descriptorChanged += value;
      remove => this.m_descriptorChanged -= value;
    }

    public event EventHandler<DescriptorChangeNotificationEventArgs> DescriptorsChangedNotification
    {
      add => this.m_descriptorChangedNotification += value;
      remove => this.m_descriptorChangedNotification -= value;
    }

    protected IdentityService IdentityService => this.m_identityService;

    public Guid DomainId => this.m_identityService.DomainId;

    public IdentityMapper IdentityMapper => this.m_identityService.IdentityMapper;

    public IDictionary<string, IIdentityProvider> SyncAgents => this.m_identityService.SyncAgents;

    public IdentityDomain Domain => this.m_identityService.IdentityServiceInternal().Domain;
  }
}
