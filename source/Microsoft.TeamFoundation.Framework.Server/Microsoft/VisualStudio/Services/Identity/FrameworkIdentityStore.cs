// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.FrameworkIdentityStore
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkIdentityStore : IdentityStoreBase
  {
    private readonly IdentityDomain m_masterDomain;
    private readonly FrameworkIdentityStore m_parentIdentityStore;
    private readonly IFrameworkIdentityCache m_cache;
    private IDictionary<string, IIdentityProvider> m_syncAgents;
    private readonly List<IdentityMessageBusData> m_identityUpdates = new List<IdentityMessageBusData>();
    private readonly ILockName m_processChangesTaskLockName;
    private IdentityStoreBase.TaskState m_processChangesTaskState;
    private readonly object m_processChangesLock = new object();
    private int m_changeId;
    private const string s_area = "IdentityService";
    private const string s_layer = "FrameworkIdentityStore";
    private const string s_readIdentityLayer = "FrameworkIdentityStoreReadIdentities";
    private const string s_featureNameCacheExtendedProperties = "VisualStudio.IdentityStore.CacheExtendedProperties";
    private const string s_featureNameDisableTraceErrorIfIdIsMasterId = "VisualStudio.Services.Identity.Framework.DisableTraceErrorIfIdIsMasterId";
    private const string s_featureNameEnableReduceAadMembershipsLatency = "VisualStudio.Services.Aad.EnableReduceAadUserMembershipsLatency";
    internal const string s_featureNameIgnoreCacheOnStrongInvalidations = "VisualStudio.Services.Identity.IgnoreCacheOnStrongInvalidations";
    internal const string s_featureNamePopulateMinSequenceIdRequestHeaders = "VisualStudio.Services.Identity.HttpRequests.PopulateMinSequenceIdRequestHeaders";
    internal const string s_featureNamePopulateMinSequenceIdRequestHeadersForAadBackedOrgs = "VisualStudio.Services.Identity.HttpRequests.AadBackedOrgs.PopulateMinSequenceIdRequestHeaders";
    internal const string s_featureNameInitializeSequenceContextForAadBackedOrgs = "VisualStudio.Services.Identity.AadBackedOrgs.InitializeSequenceContext";
    internal const string s_featureNameForceRemoveMemberFromGroup = "VisualStudio.Services.Identity.ForceRemoveMemberFromGroup";
    private CircuitBreakerSettings m_circuitBreakerSettings;

    public FrameworkIdentityStore(
      IVssRequestContext systemRequestContext,
      IdentityDomain hostDomain,
      IDictionary<string, IIdentityProvider> syncAgents,
      FrameworkIdentityStore parentIdentityStore)
    {
      int cacheSize = this.GetCacheSize(systemRequestContext);
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      int propertyCacheSize = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Integration/Settings/PropertyCacheSize", cacheSize * 6);
      this.m_cache = (IFrameworkIdentityCache) new FrameworkIdentityCache(systemRequestContext, hostDomain, cacheSize, propertyCacheSize);
      this.m_processChangesTaskLockName = systemRequestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}.{1}.{2}", (object) nameof (FrameworkIdentityStore), (object) nameof (m_processChangesTaskLockName), (object) hostDomain.DomainId));
      systemRequestContext.GetService<IImsCacheService>().Initialize(systemRequestContext, ImsOperation.IdentityIdsByDisplayNamePrefixSearch | ImsOperation.IdentityIdsByEmailPrefixSearch | ImsOperation.IdentityIdsByAccountNamePrefixSearch, ImsOperation.IdentitiesByDescriptor | ImsOperation.IdentitiesByDisplayName | ImsOperation.IdentitiesByAccountName | ImsOperation.IdentitiesById | ImsOperation.IdentityIdsByDisplayNamePrefixSearch | ImsOperation.IdentityIdsByEmailPrefixSearch | ImsOperation.IdentityIdsByAccountNamePrefixSearch | ImsOperation.IdentityIdsByAppIdSearch);
      this.m_masterDomain = hostDomain;
      this.m_syncAgents = syncAgents;
      this.m_parentIdentityStore = parentIdentityStore;
      this.m_masterDomain.IdentityStore = (object) this;
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.IMS2FrameworkIdentityChanged, new SqlNotificationCallback(this.OnIdentityChangedNotification), false);
      if (this.m_parentIdentityStore != null)
      {
        this.m_parentIdentityStore.IdentitiesChanged += new EventHandler<IdentityChangeEventArgs>(this.OnParentIdentityChangeEvent);
        this.m_parentIdentityStore.DescriptorsChangedNotification += new EventHandler<DescriptorChangeNotificationEventArgs>(this.OnParentDescriptorChangeNotifcationEvent);
        this.m_parentIdentityStore.IdentityPropertiesChanged += new EventHandler<IdentityPropertiesChangeEventArgs>(this.OnIdentityPropertiesChangeEvent);
      }
      this.InitializeSequenceContext(systemRequestContext, hostDomain);
      this.m_circuitBreakerSettings = CircuitBreakerSettings.Default;
    }

    private int GetCacheSize(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      int defaultValue = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Identity/Settings/HostCacheSize", true, 1024);
      if (systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        defaultValue = 10000;
      else if (systemRequestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        defaultValue = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Identity/Settings/DeploymentCacheSize", 100000);
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      int cacheSize = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Integration/Settings/IdentityHostCacheSize", defaultValue);
      systemRequestContext.Trace(80117, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "FrameworkIdentityStore CacheSize set to value: {0}", (object) cacheSize);
      return cacheSize;
    }

    internal FrameworkIdentityStore()
    {
    }

    private void OnIdentityPropertiesChangeEvent(object sender, IdentityPropertiesChangeEventArgs e)
    {
      if (sender == this || !this.m_cache.UpdateIdentityProperties(e.RequestContext, (IDictionary<Guid, Dictionary<string, object>>) e.ChangedIdentityProperties))
        return;
      this.OnIdentityPropertiesChanged((object) this, new IdentityPropertiesChangeEventArgs()
      {
        RequestContext = e.RequestContext,
        ChangedIdentityProperties = e.ChangedIdentityProperties
      });
    }

    internal void AddDomain(IVssRequestContext systemRequestContext, IdentityDomain hostDomain)
    {
      hostDomain.IdentityStore = (object) this;
      this.m_cache.AddDomain(systemRequestContext, hostDomain);
      systemRequestContext.GetService<IImsCacheService>().Initialize(systemRequestContext, ImsOperation.IdentityIdsByDisplayNamePrefixSearch | ImsOperation.IdentityIdsByEmailPrefixSearch | ImsOperation.IdentityIdsByAccountNamePrefixSearch, ImsOperation.IdentitiesByDescriptor | ImsOperation.IdentitiesByDisplayName | ImsOperation.IdentitiesByAccountName | ImsOperation.IdentitiesById | ImsOperation.IdentityIdsByDisplayNamePrefixSearch | ImsOperation.IdentityIdsByEmailPrefixSearch | ImsOperation.IdentityIdsByAccountNamePrefixSearch | ImsOperation.IdentityIdsByAppIdSearch);
    }

    internal void Unload(IVssRequestContext requestContext)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.IMS2FrameworkIdentityChanged, new SqlNotificationCallback(this.OnIdentityChangedNotification), false);
      if (this.m_parentIdentityStore != null)
      {
        this.m_parentIdentityStore.IdentitiesChanged -= new EventHandler<IdentityChangeEventArgs>(this.OnParentIdentityChangeEvent);
        this.m_parentIdentityStore.DescriptorsChangedNotification -= new EventHandler<DescriptorChangeNotificationEventArgs>(this.OnParentDescriptorChangeNotifcationEvent);
        this.m_parentIdentityStore.IdentityPropertiesChanged -= new EventHandler<IdentityPropertiesChangeEventArgs>(this.OnIdentityPropertiesChangeEvent);
      }
      this.m_cache.Unload(requestContext);
    }

    public virtual IdentityScope CreateScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      Guid parentScopeId,
      GroupScopeType scopeType,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription,
      Guid creatorId)
    {
      IdentityScope scope = this.GetHttpClient(requestContext).CreateScopeAsync(scopeId, parentScopeId, scopeType, scopeName, adminGroupName, adminGroupDescription, creatorId, (object) null, new CancellationToken()).SyncResult<IdentityScope>();
      this.InvalidateAccessControlEntries(requestContext);
      return scope;
    }

    public override IdentityScope GetScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId)
    {
      return this.GetHttpClient(requestContext).GetScopeAsync(scopeId, (object) null, new CancellationToken()).SyncResult<IdentityScope>();
    }

    public IdentityScope GetScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      string scopeName)
    {
      return this.GetHttpClient(requestContext).GetScopeAsync(scopeName, (object) null, new CancellationToken()).SyncResult<IdentityScope>();
    }

    public void DeleteScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId)
    {
      TaskExtensions.SyncResult(this.GetHttpClient(requestContext).DeleteScopeAsync(scopeId, (object) null, new CancellationToken()));
      this.InvalidateAccessControlEntries(requestContext);
    }

    public void RenameScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      string newName)
    {
      TaskExtensions.SyncResult(this.GetHttpClient(requestContext).RenameScopeAsync(scopeId, newName, (object) null, new CancellationToken()));
    }

    public void RestoreScope(IVssRequestContext requestContext, Guid scopeId) => TaskExtensions.SyncResult(this.GetHttpClient(requestContext).RestoreGroupScopeAsync(scopeId, (object) null, new CancellationToken()));

    public Microsoft.VisualStudio.Services.Identity.Identity CreateUnauthenticatedIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      IdentityDescriptor descriptor,
      string domainName,
      string accountName,
      string description)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity.Descriptor = descriptor;
      identity.ProviderDisplayName = accountName;
      identity.IsActive = false;
      identity.SetProperty("Description", (object) description);
      identity.SetProperty("Domain", (object) domainName);
      identity.SetProperty("Account", (object) accountName);
      return this.GetHttpClient(requestContext).CreateGroupsAsync(scopeId, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      }, (object) null, new CancellationToken()).SyncResult<IdentitiesCollection>()[0];
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> CreateGroups(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      params GroupDescription[] groupDescriptions)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] groups1 = new Microsoft.VisualStudio.Services.Identity.Identity[groupDescriptions.Length];
      for (int index = 0; index < groupDescriptions.Length; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
        identity.Descriptor = groupDescriptions[index].Descriptor;
        identity.ProviderDisplayName = groupDescriptions[index].Name;
        identity.IsActive = true;
        identity.SetProperty("Description", (object) groupDescriptions[index].Description);
        identity.SetProperty("SpecialType", (object) groupDescriptions[index].SpecialType.ToString());
        if (groupDescriptions[index].HasRestrictedVisibility)
          identity.SetProperty("RestrictedVisible", (object) "RestrictedVisible");
        if (!groupDescriptions[index].ScopeLocal)
          identity.SetProperty("CrossProject", (object) "CrossProject");
        groups1[index] = identity;
      }
      IdentitiesCollection groups2 = this.GetHttpClient(requestContext).CreateGroupsAsync(scopeId, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) groups1, (object) null, new CancellationToken()).SyncResult<IdentitiesCollection>();
      this.InvalidateSequenceContext(requestContext);
      this.InvalidateAccessControlEntries(requestContext);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) groups2;
    }

    public void DeleteGroup(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor)
    {
      TaskExtensions.SyncResult(this.GetHttpClient(requestContext).DeleteGroupAsync(groupDescriptor, (object) null, new CancellationToken()));
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, (ICollection<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        groupDescriptor
      });
      this.InvalidateSequenceContext(requestContext);
      this.InvalidateAccessControlEntries(requestContext);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid[] scopeIds,
      bool recurse,
      bool deleted,
      IEnumerable<string> propertyNameFilters)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetHttpClient(requestContext).ListGroupsAsync(scopeIds, recurse, deleted, propertyNameFilters, (object) null, new CancellationToken()).SyncResult<IdentitiesCollection>();
    }

    public bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity member)
    {
      bool group = this.GetHttpClient(requestContext).AddMemberToGroupAsync(groupDescriptor, member.Descriptor, (object) null, new CancellationToken()).SyncResult<bool>();
      if (group)
      {
        this.InvalidateSequenceContext(requestContext);
        this.ProcessMembershipChangesOnAuthor(requestContext, hostDomain, (ICollection<MembershipChangeInfo>) new MembershipChangeInfo[1]
        {
          new MembershipChangeInfo()
          {
            ContainerDescriptor = groupDescriptor,
            MemberId = member.Id,
            MemberDescriptor = member.Descriptor,
            IsMemberGroup = member.IsContainer,
            Active = true
          }
        });
      }
      return group;
    }

    public virtual bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      bool group = this.GetHttpClient(requestContext).AddMemberToGroupAsync(groupDescriptor, memberDescriptor, (object) null, new CancellationToken()).SyncResult<bool>();
      if (group)
      {
        this.InvalidateSequenceContext(requestContext);
        this.ProcessMembershipChangesOnAuthor(requestContext, hostDomain, (ICollection<MembershipChangeInfo>) new MembershipChangeInfo[1]
        {
          new MembershipChangeInfo()
          {
            ContainerDescriptor = groupDescriptor,
            MemberDescriptor = memberDescriptor,
            IsMemberGroup = IdentityValidation.IsTeamFoundationType(memberDescriptor),
            Active = true
          }
        });
      }
      return group;
    }

    public bool RemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor,
      bool forceRemove = false)
    {
      bool flag = !forceRemove || !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.ForceRemoveMemberFromGroup") ? this.GetHttpClient(requestContext).RemoveMemberFromGroupAsync(groupDescriptor, memberDescriptor, (object) null, new CancellationToken()).SyncResult<bool>() : this.GetHttpClient(requestContext).ForceRemoveMemberFromGroupAsync(groupDescriptor, memberDescriptor, (object) null, new CancellationToken()).SyncResult<bool>();
      if (flag)
      {
        this.InvalidateSequenceContext(requestContext);
        this.ProcessMembershipChangesOnAuthor(requestContext, hostDomain, (ICollection<MembershipChangeInfo>) new MembershipChangeInfo[1]
        {
          new MembershipChangeInfo()
          {
            ContainerDescriptor = groupDescriptor,
            MemberDescriptor = memberDescriptor,
            IsMemberGroup = IdentityValidation.IsTeamFoundationType(memberDescriptor),
            Active = false
          }
        });
      }
      return flag;
    }

    public override bool IsMember(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      long cacheStamp;
      IdentityMembershipInfo membershipInfo;
      bool? nullable = this.m_cache.IsMember(requestContext, hostDomain, groupDescriptor, memberDescriptor, out cacheStamp, out membershipInfo);
      bool flag = requestContext.IsTracingSecurityEvaluation(80099, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore));
      if (!nullable.HasValue)
      {
        this.IncrementCacheMissPerfCounters();
        if (flag)
          requestContext.TraceSecurityEvaluation(80100, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "[IsMember] Cache miss - member descriptor: {0}, group descriptor: {1}, host domain: {2}", (object) memberDescriptor, (object) groupDescriptor, (object) hostDomain);
        List<IdentityDescriptor> descriptors = new List<IdentityDescriptor>()
        {
          memberDescriptor
        };
        QueryMembership queryMembership = QueryMembership.ExpandedUp;
        if (flag)
          requestContext.TraceSecurityEvaluation(80108, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "[IsMember] ReadIdentities with queryMembership {0} and member descriptor {1} with strong invalidation flag {2}", (object) queryMembership, (object) memberDescriptor, (object) membershipInfo?.IsStronglyInvalidated);
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity = this.ReadIdentities(requestContext.Elevate(), hostDomain, (IList<IdentityDescriptor>) descriptors, queryMembership, (IEnumerable<string>) null, true, membershipInfo != null && membershipInfo.IsStronglyInvalidated)[0];
        if (readIdentity != null)
        {
          if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.ExecutionEnvironment.IsHostedDeployment && !readIdentity.IsContainer)
            readIdentity.MemberOf.Add(this.m_masterDomain.DomainRoot);
          this.m_cache.UpdateParentMemberships(requestContext, hostDomain, readIdentity);
          nullable = this.m_cache.IsMember(requestContext, hostDomain, groupDescriptor, memberDescriptor, out cacheStamp, out IdentityMembershipInfo _);
          if (!nullable.HasValue && readIdentity.MemberOf != null)
            nullable = new bool?(readIdentity.MemberOf.Contains<IdentityDescriptor>(groupDescriptor, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance));
        }
        else
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
          identity1.Descriptor = memberDescriptor;
          identity1.MemberOf = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
          identity1.MemberIds = (ICollection<Guid>) Array.Empty<Guid>();
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
          this.m_cache.UpdateParentMemberships(requestContext, hostDomain, identity2);
        }
      }
      else
      {
        this.IncrementCacheHitPerfCounters();
        if (flag)
          requestContext.TraceSecurityEvaluation(80101, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "[IsMember] Cache hit - member descriptor: {0}, group descriptor: {1}, host domain: {2}", (object) memberDescriptor, (object) groupDescriptor, (object) hostDomain);
      }
      bool valueOrDefault = nullable.GetValueOrDefault();
      if (flag)
        requestContext.TraceSecurityEvaluation(88888, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "[IsMember] IsMember: {0}, member descriptor: {1}, group descriptor: {2}, host domain: {3}", (object) valueOrDefault, (object) memberDescriptor, (object) groupDescriptor, (object) hostDomain);
      return valueOrDefault;
    }

    public override Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      SocialDescriptor socialDescriptor,
      bool bypassCache = false)
    {
      throw new NotImplementedException();
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] identities = new Microsoft.VisualStudio.Services.Identity.Identity[subjectDescriptors.Count];
      List<Tuple<int, SubjectDescriptor>> source = new List<Tuple<int, SubjectDescriptor>>();
      bool[] descriptorsToSkip = this.GetSubjectDescriptorsToSkip(requestContext, subjectDescriptors);
      bool flag1 = this.UseExtendedPropertiesCache(requestContext, propertyNameFilters);
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
      for (int index = 0; index < subjectDescriptors.Count; ++index)
      {
        if (descriptorsToSkip[index])
        {
          identities[index] = identity1;
        }
        else
        {
          identities[index] = this.m_cache.ReadIdentity(requestContext, hostDomain, subjectDescriptors[index], queryMembership);
          if (identities[index] == null)
          {
            this.IncrementCacheMissPerfCounters();
            requestContext.Trace(111475, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Cache miss when reading identity - {0}, {1}, {2}, {3}", (object) hostDomain.DomainId, (object) subjectDescriptors[index], (object) queryMembership, (object) includeRestrictedVisibility);
            source.Add(new Tuple<int, SubjectDescriptor>(index, subjectDescriptors[index]));
          }
          else
          {
            this.IncrementCacheHitPerfCounters();
            requestContext.Trace(503129, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Cache hit when reading identity - {0}, {1}, {2}, {3}", (object) hostDomain.DomainId, (object) subjectDescriptors[index], (object) queryMembership, (object) includeRestrictedVisibility);
            if (!this.TryEnrichIdentityProperties(requestContext, hostDomain, propertyNameFilters, identities[index], flag1))
              source.Add(new Tuple<int, SubjectDescriptor>(index, subjectDescriptors[index]));
          }
        }
      }
      if (source.Count > 0)
      {
        IEnumerable<string> propertyNameFilters1 = propertyNameFilters;
        int num1 = flag1 ? 1 : 0;
        int num2 = (int) queryMembership;
        IEnumerable<SubjectDescriptor> subjectDescriptors1 = (IEnumerable<SubjectDescriptor>) subjectDescriptors;
        IdentitySearchFilter? searchFilter = new IdentitySearchFilter?();
        IEnumerable<SubjectDescriptor> subjectDescriptors2 = subjectDescriptors1;
        IEnumerable<string> propertiesToFetch = this.GetPropertiesToFetch(propertyNameFilters1, num1 != 0, (QueryMembership) num2, searchFilter: searchFilter, subjectDescriptors: subjectDescriptors2);
        List<SubjectDescriptor> subjectDescriptorsToFetch = source.Select<Tuple<int, SubjectDescriptor>, SubjectDescriptor>((Func<Tuple<int, SubjectDescriptor>, SubjectDescriptor>) (cacheMiss => cacheMiss.Item2)).ToList<SubjectDescriptor>();
        requestContext.Trace(444178, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Fetching {0} identities from identity service", (object) subjectDescriptorsToFetch.Count);
        requestContext.TraceConditionally(257447, TraceLevel.Verbose, "IdentityService", "FrameworkIdentityStoreReadIdentities", (Func<string>) (() => string.Format("FrameworkIdentityStore.ReadIdentities where subjectDescriptors : {0}, queryMembership : {1}, propertyNames : {2}, includeRestrictedVisibility : {3}, stackTrace : {4}", (object) subjectDescriptors.Serialize<IList<SubjectDescriptor>>(), (object) queryMembership, propertiesToFetch == null ? (object) string.Empty : (object) propertiesToFetch.Serialize<IEnumerable<string>>(), (object) includeRestrictedVisibility, (object) EnvironmentWrapper.ToReadableStackTrace())));
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.SafeReadIdentities(requestContext, (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
        {
          bool flag2 = FrameworkIdentityStore.UseMinSequenceContext(requestContext);
          if (!flag2)
          {
            requestContext.Trace(2432359, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityStore), "ReadIndentities without sequence context after deciding");
            return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetHttpClient(requestContext).ReadIdentitiesAsync((IList<SubjectDescriptor>) subjectDescriptorsToFetch, queryMembership, propertiesToFetch, includeRestrictedVisibility, (object) null, new CancellationToken()).SyncResult<IdentitiesCollection>();
          }
          RequestHeadersContext requestHeadersContext = new RequestHeadersContext(flag2 ? this.GetSequenceContext(requestContext, hostDomain) : (SequenceContext) null);
          requestContext.Trace(2432358, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityStore), "ReadIndentities using sequence context after deciding");
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetHttpClient(requestContext).ReadIdentitiesAsync((IList<SubjectDescriptor>) subjectDescriptorsToFetch, requestHeadersContext, queryMembership, propertiesToFetch, includeRestrictedVisibility).SyncResult<IdentitiesCollection>();
        }), (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() => (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[subjectDescriptorsToFetch.Count]));
        int num3 = 0;
        for (int index1 = 0; index1 < identityList.Count; ++index1)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = identityList[index1];
          if (identity2 != null)
          {
            int index2 = source[index1].Item1;
            identities[index2] = identity2;
            ++num3;
            this.CacheIdentity(requestContext, hostDomain, queryMembership, propertiesToFetch, identity2, flag1);
          }
        }
        requestContext.Trace(52889, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Successfully read {0} identities from identity service. {1} identities not found", (object) num3, (object) (subjectDescriptorsToFetch.Count - num3));
      }
      for (int index = 0; index < identities.Length; ++index)
      {
        if (identities[index] == identity1)
          identities[index] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      this.CheckIdentitiesForInvalidKeys(requestContext, identities);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] identities = new Microsoft.VisualStudio.Services.Identity.Identity[socialDescriptors.Count];
      List<Tuple<int, SocialDescriptor>> source = new List<Tuple<int, SocialDescriptor>>();
      bool flag = this.UseExtendedPropertiesCache(requestContext, propertyNameFilters);
      for (int index = 0; index < socialDescriptors.Count; ++index)
      {
        identities[index] = this.m_cache.ReadIdentity(requestContext, hostDomain, socialDescriptors[index], queryMembership);
        if (identities[index] == null)
        {
          this.IncrementCacheMissPerfCounters();
          requestContext.Trace(111475, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Cache miss when reading identity - {0}, {1}, {2}, {3}", (object) hostDomain.DomainId, (object) socialDescriptors[index], (object) queryMembership, (object) includeRestrictedVisibility);
          source.Add(new Tuple<int, SocialDescriptor>(index, socialDescriptors[index]));
        }
        else
        {
          this.IncrementCacheHitPerfCounters();
          requestContext.Trace(503129, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Cache hit when reading identity - {0}, {1}, {2}, {3}", (object) hostDomain.DomainId, (object) socialDescriptors[index], (object) queryMembership, (object) includeRestrictedVisibility);
          if (!this.TryEnrichIdentityProperties(requestContext, hostDomain, propertyNameFilters, identities[index], flag))
            source.Add(new Tuple<int, SocialDescriptor>(index, socialDescriptors[index]));
        }
      }
      if (source.Count > 0)
      {
        IEnumerable<string> propertiesToFetch = propertyNameFilters;
        List<SocialDescriptor> socialDescriptorsToFetch = source.Select<Tuple<int, SocialDescriptor>, SocialDescriptor>((Func<Tuple<int, SocialDescriptor>, SocialDescriptor>) (cacheMiss => cacheMiss.Item2)).ToList<SocialDescriptor>();
        requestContext.Trace(444178, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Fetching {0} identities from identity service", (object) socialDescriptorsToFetch.Count);
        requestContext.TraceConditionally(257447, TraceLevel.Verbose, "IdentityService", "FrameworkIdentityStoreReadIdentities", (Func<string>) (() => string.Format("FrameworkIdentityStore.ReadIdentities where subjectDescriptors : {0}, queryMembership : {1}, propertyNames : {2}, includeRestrictedVisibility : {3}, stackTrace : {4}", (object) socialDescriptors.Serialize<IList<SocialDescriptor>>(), (object) queryMembership, propertiesToFetch == null ? (object) string.Empty : (object) propertiesToFetch.Serialize<IEnumerable<string>>(), (object) includeRestrictedVisibility, (object) EnvironmentWrapper.ToReadableStackTrace())));
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.SafeReadIdentities(requestContext, (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
        {
          RequestHeadersContext requestHeadersContext = new RequestHeadersContext((SequenceContext) null);
          requestContext.Trace(2432360, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityStore), "ReadIndentities without sequence context without deciding");
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetHttpClient(requestContext).ReadIdentitiesAsync((IList<SocialDescriptor>) socialDescriptorsToFetch, requestHeadersContext, queryMembership, propertiesToFetch, includeRestrictedVisibility).SyncResult<IdentitiesCollection>();
        }), (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() => (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[socialDescriptorsToFetch.Count]));
        int num = 0;
        for (int index1 = 0; index1 < identityList.Count; ++index1)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index1];
          if (identity != null)
          {
            int index2 = source[index1].Item1;
            identities[index2] = identity;
            ++num;
            this.CacheIdentity(requestContext, hostDomain, queryMembership, propertiesToFetch, identity, flag);
          }
        }
        requestContext.Trace(52889, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Successfully read {0} identities from identity service. {1} identities not found", (object) num, (object) (socialDescriptorsToFetch.Count - num));
      }
      this.CheckIdentitiesForInvalidKeys(requestContext, identities);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility)
    {
      bool ignoreCache = false;
      if (requestContext.Items != null && requestContext.Items.TryGetValue<bool>("$BypassIdentityCacheWhenReadingByDescriptor", out ignoreCache))
        requestContext.Trace(1312216998, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Ignore cache value is - {0}", (object) ignoreCache);
      return this.ReadIdentities(requestContext, hostDomain, descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility, ignoreCache);
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      bool ignoreCache)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] results = new Microsoft.VisualStudio.Services.Identity.Identity[descriptors.Count];
      List<Tuple<int, IdentityDescriptor>> source1 = new List<Tuple<int, IdentityDescriptor>>();
      bool ignoreCacheFlag = ignoreCache && requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.IgnoreCacheOnStrongInvalidations");
      bool[] descriptorsToSkip = this.GetDescriptorsToSkip(requestContext, descriptors);
      bool flag1 = this.UseExtendedPropertiesCache(requestContext, propertyNameFilters);
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
      for (int index = 0; index < descriptors.Count; ++index)
      {
        if (descriptorsToSkip[index])
        {
          results[index] = identity1;
        }
        else
        {
          results[index] = ignoreCacheFlag ? (Microsoft.VisualStudio.Services.Identity.Identity) null : this.m_cache.ReadIdentity(requestContext, hostDomain, descriptors[index], queryMembership);
          if (results[index] == null)
            source1.Add(new Tuple<int, IdentityDescriptor>(index, descriptors[index]));
        }
      }
      if (!ignoreCacheFlag && source1.Count > 0)
      {
        this.ReadIdentitiesFromImsCache(requestContext, hostDomain, descriptors, queryMembership, results);
        foreach (Tuple<int, IdentityDescriptor> tuple in source1.Where<Tuple<int, IdentityDescriptor>>((Func<Tuple<int, IdentityDescriptor>, bool>) (cacheMiss => results[cacheMiss.Item1] != null)))
          this.CacheIdentity(requestContext, hostDomain, queryMembership, (IEnumerable<string>) null, results[tuple.Item1], flag1);
      }
      List<Tuple<int, IdentityDescriptor>> source2 = new List<Tuple<int, IdentityDescriptor>>();
      for (int index = 0; index < descriptors.Count; ++index)
      {
        if (results[index] == null)
        {
          this.IncrementCacheMissPerfCounters();
          requestContext.Trace(80102, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Cache miss when reading identity - {0}, {1}, {2}, {3}", (object) hostDomain.DomainId, (object) descriptors[index], (object) queryMembership, (object) includeRestrictedVisibility);
          source2.Add(new Tuple<int, IdentityDescriptor>(index, descriptors[index]));
        }
        else
        {
          this.IncrementCacheHitPerfCounters();
          requestContext.Trace(80103, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Cache hit when reading identity - {0}, {1}, {2}, {3}", (object) hostDomain.DomainId, (object) descriptors[index], (object) queryMembership, (object) includeRestrictedVisibility);
          if (!this.TryEnrichIdentityProperties(requestContext, hostDomain, propertyNameFilters, results[index], flag1))
            source2.Add(new Tuple<int, IdentityDescriptor>(index, descriptors[index]));
        }
      }
      if (source2.Count > 0)
      {
        IEnumerable<string> propertiesToFetch = this.GetPropertiesToFetch(propertyNameFilters, flag1, queryMembership, (IEnumerable<IdentityDescriptor>) descriptors);
        List<IdentityDescriptor> descriptorsToFetch = source2.Select<Tuple<int, IdentityDescriptor>, IdentityDescriptor>((Func<Tuple<int, IdentityDescriptor>, IdentityDescriptor>) (cacheMiss => cacheMiss.Item2)).ToList<IdentityDescriptor>();
        requestContext.Trace(80110, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Fetching {0} identities from identity service", (object) descriptorsToFetch.Count);
        IVssRequestContext requestContext1 = requestContext;
        int num1 = (int) queryMembership;
        IEnumerable<string> source3 = propertiesToFetch;
        int extendedPropertiesCount = source3 != null ? source3.Count<string>() : 0;
        int count = descriptors.Count;
        int identityTracePoint = IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ByDescriptor, (QueryMembership) num1, extendedPropertiesCount, count);
        Func<string> message = (Func<string>) (() => string.Format("FrameworkIdentityStore.ReadIdentities where descriptors : {0}, queryMembership : {1}, propertyNames : {2}, includeRestrictedVisibility : {3}, ignoreCache : {4},  stackTrace : {5}", (object) descriptors.Serialize<IList<IdentityDescriptor>>(), (object) queryMembership, propertiesToFetch == null ? (object) string.Empty : (object) propertiesToFetch.Serialize<IEnumerable<string>>(), (object) includeRestrictedVisibility, (object) ignoreCacheFlag, (object) EnvironmentWrapper.ToReadableStackTrace()));
        requestContext1.TraceConditionally(identityTracePoint, TraceLevel.Verbose, "IdentityService", "FrameworkIdentityStoreReadIdentities", message);
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsTracing(414583, TraceLevel.Warning, "IdentityService", nameof (FrameworkIdentityStore)))
        {
          List<IdentityDescriptor> servicePrincipalDescriptors = descriptorsToFetch.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => ServicePrincipals.IsServicePrincipal(requestContext, x))).ToList<IdentityDescriptor>();
          if (servicePrincipalDescriptors.Any<IdentityDescriptor>())
            requestContext.TraceDataConditionally(414583, TraceLevel.Warning, "IdentityService", nameof (FrameworkIdentityStore), "Trying to read S2S identity using IMS, this shouldn't happen", (Func<object>) (() => (object) new
            {
              servicePrincipalDescriptors = servicePrincipalDescriptors,
              StackTrace = Environment.StackTrace
            }), nameof (ReadIdentities));
        }
        IList<Microsoft.VisualStudio.Services.Identity.Identity> results1 = this.SafeReadIdentities(requestContext, (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
        {
          bool flag2 = FrameworkIdentityStore.UseMinSequenceContext(requestContext);
          if (!flag2 && !ignoreCacheFlag)
          {
            requestContext.Trace(2432359, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityStore), "ReadIndentities without sequence context after deciding");
            return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetHttpClient(requestContext).ReadIdentitiesAsync((IList<IdentityDescriptor>) descriptorsToFetch, queryMembership, propertiesToFetch, includeRestrictedVisibility, (object) null, new CancellationToken()).SyncResult<IdentitiesCollection>();
          }
          RequestHeadersContext requestHeadersContext = new RequestHeadersContext(flag2 ? this.GetSequenceContext(requestContext, hostDomain) : (SequenceContext) null, ignoreCacheFlag);
          requestContext.Trace(2432358, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityStore), "ReadIndentities using sequence context after deciding");
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetHttpClient(requestContext).ReadIdentitiesAsync((IList<IdentityDescriptor>) descriptorsToFetch, requestHeadersContext, queryMembership, propertiesToFetch, includeRestrictedVisibility, (object) null, new CancellationToken()).SyncResult<IdentitiesCollection>();
        }), (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() => (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[descriptorsToFetch.Count]));
        int num2 = 0;
        for (int index1 = 0; index1 < results1.Count; ++index1)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = results1[index1];
          if (identity2 != null)
          {
            int index2 = source2[index1].Item1;
            results[index2] = identity2;
            ++num2;
            this.CacheIdentity(requestContext, hostDomain, queryMembership, propertiesToFetch, identity2, flag1);
          }
        }
        requestContext.Trace(80111, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Successfully read {0} identities from identity service. {1} identities not found", (object) num2, (object) (descriptorsToFetch.Count - num2));
        this.UpdateIdentitiesInImsCache(requestContext, hostDomain, queryMembership, descriptorsToFetch, results1);
      }
      for (int index = 0; index < results.Length; ++index)
      {
        if (results[index] == identity1)
          results[index] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      this.CheckIdentitiesForInvalidKeys(requestContext, results);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results;
    }

    private bool[] GetSubjectDescriptorsToSkip(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors)
    {
      bool[] descriptorsToSkip = new bool[subjectDescriptors.Count];
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return descriptorsToSkip;
      int num = 0;
      for (int index = 0; index < subjectDescriptors.Count; ++index)
      {
        SubjectDescriptor subjectDescriptor = subjectDescriptors[index];
        if (subjectDescriptor == new SubjectDescriptor())
          descriptorsToSkip[index] = true;
        else if (subjectDescriptor.IsSubjectStoreType() || subjectDescriptor.IsWindowsType())
        {
          requestContext.TraceSerializedConditionally(516144, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Caller attempted to ReadIdentities by subject descriptor for a SubjectStore descriptor or Windows type in hosted. Requested descriptor: {0}", (object) subjectDescriptor);
          descriptorsToSkip[index] = true;
          ++num;
        }
      }
      return descriptorsToSkip;
    }

    private bool[] GetDescriptorsToSkip(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors)
    {
      bool[] descriptorsToSkip = new bool[descriptors.Count];
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return descriptorsToSkip;
      int num = 0;
      for (int index = 0; index < descriptors.Count; ++index)
      {
        IdentityDescriptor descriptor = descriptors[index];
        if (descriptor == (IdentityDescriptor) null)
          descriptorsToSkip[index] = true;
        else if (descriptor.IsSubjectStoreType() || requestContext.ExecutionEnvironment.IsHostedDeployment && descriptor.IsWindowsType())
        {
          descriptorsToSkip[index] = true;
          ++num;
        }
      }
      if (num > 0)
        requestContext.TraceSerializedConditionally(80126, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Caller attempted to ReadIdentities by Descriptor for a SubjectStore descriptor or Windows type in hosted {0} times. Requested descriptors: {1}", (object) num, (object) descriptors);
      return descriptorsToSkip;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] results = new Microsoft.VisualStudio.Services.Identity.Identity[identityIds.Count];
      List<Tuple<int, Guid>> source1 = new List<Tuple<int, Guid>>();
      bool flag1;
      if (requestContext.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.BypassIdentityCacheWhenReadingByVsid, out flag1))
        requestContext.RootContext.Items.Remove(RequestContextItemsKeys.BypassIdentityCacheWhenReadingByVsid);
      if (requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application) && requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        requestContext.TraceSerializedConditionally(801001, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Caller attempted to ReadIdentities by ID by elevating to organization level. Requested Ids: {0}", (object) identityIds);
      bool flag2 = this.UseExtendedPropertiesCache(requestContext, propertyNameFilters);
      int num1 = 0;
      for (int index = 0; index < identityIds.Count; ++index)
      {
        if (identityIds[index] == Guid.Empty)
        {
          ++num1;
        }
        else
        {
          if (!flag1)
            results[index] = this.m_cache.ReadIdentity(requestContext, hostDomain, identityIds[index], queryMembership);
          if (results[index] == null)
            source1.Add(new Tuple<int, Guid>(index, identityIds[index]));
        }
      }
      if (num1 > 0)
        requestContext.TraceSerializedConditionally(80109, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Caller attempted to ReadIdentities by ID for an Empty Guid {0} times. Requested Ids: {1}", (object) num1, (object) identityIds);
      if (source1.Count > 0 && !flag1)
      {
        this.ReadIdentitiesFromImsCache(requestContext, hostDomain, identityIds, queryMembership, results);
        foreach (Tuple<int, Guid> tuple in source1.Where<Tuple<int, Guid>>((Func<Tuple<int, Guid>, bool>) (cacheMiss => results[cacheMiss.Item1] != null)))
          this.CacheIdentity(requestContext, hostDomain, queryMembership, (IEnumerable<string>) null, results[tuple.Item1], flag2);
      }
      List<Tuple<int, Guid>> source2 = new List<Tuple<int, Guid>>();
      for (int index = 0; index < identityIds.Count; ++index)
      {
        if (results[index] != null || !(identityIds[index] == Guid.Empty))
        {
          if (results[index] == null)
          {
            this.IncrementCacheMissPerfCounters();
            requestContext.Trace(80104, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Cache miss when reading identity - {0}, {1}, {2}, {3}", (object) hostDomain.DomainId, (object) identityIds[index], (object) queryMembership, (object) includeRestrictedVisibility);
            source2.Add(new Tuple<int, Guid>(index, identityIds[index]));
          }
          else
          {
            this.IncrementCacheHitPerfCounters();
            requestContext.Trace(80105, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Cache hit when reading identity - {0}, {1}, {2}, {3}", (object) hostDomain.DomainId, (object) identityIds[index], (object) queryMembership, (object) includeRestrictedVisibility);
            if (!this.TryEnrichIdentityProperties(requestContext, hostDomain, propertyNameFilters, results[index], flag2))
              source2.Add(new Tuple<int, Guid>(index, identityIds[index]));
          }
        }
      }
      if (source2.Count > 0)
      {
        List<Guid> cacheMissesIdentityIds = source2.Select<Tuple<int, Guid>, Guid>((Func<Tuple<int, Guid>, Guid>) (cacheMiss => cacheMiss.Item2)).ToList<Guid>();
        IVssRequestContext requestContext1 = requestContext;
        int num2 = (int) queryMembership;
        IEnumerable<string> source3 = propertyNameFilters;
        int extendedPropertiesCount = source3 != null ? source3.Count<string>() : 0;
        int count = identityIds.Count;
        int identityTracePoint = IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ById, (QueryMembership) num2, extendedPropertiesCount, count);
        Func<string> message = (Func<string>) (() => string.Format("FrameworkIdentityStore.ReadIdentities where identityIds : {0}, queryMembership : {1}, propertyNames : {2}, includeRestrictedVisibility : {3}, stackTrace : {4}", (object) identityIds.Serialize<IList<Guid>>(), (object) queryMembership, propertyNameFilters == null ? (object) string.Empty : (object) propertyNameFilters.Serialize<IEnumerable<string>>(), (object) includeRestrictedVisibility, (object) EnvironmentWrapper.ToReadableStackTrace()));
        requestContext1.TraceConditionally(identityTracePoint, TraceLevel.Verbose, "IdentityService", "FrameworkIdentityStoreReadIdentities", message);
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsTracing(414583, TraceLevel.Warning, "IdentityService", nameof (FrameworkIdentityStore)))
        {
          List<Guid> servicePrincipalIdentityIds = cacheMissesIdentityIds.Where<Guid>((Func<Guid, bool>) (x => ServicePrincipals.IsInternalServicePrincipalId(x))).ToList<Guid>();
          if (servicePrincipalIdentityIds.Any<Guid>())
            requestContext.TraceDataConditionally(414583, TraceLevel.Warning, "IdentityService", nameof (FrameworkIdentityStore), "Trying to read S2S identity using IMS, this shouldn't happen", (Func<object>) (() => (object) new
            {
              servicePrincipalIdentityIds = servicePrincipalIdentityIds,
              StackTrace = Environment.StackTrace
            }), nameof (ReadIdentities));
        }
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.SafeReadIdentities(requestContext, (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
        {
          if (FrameworkIdentityStore.UseMinSequenceContext(requestContext))
          {
            RequestHeadersContext requestHeadersContext = new RequestHeadersContext(this.GetSequenceContext(requestContext, hostDomain));
            requestContext.Trace(2432358, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityStore), "ReadIndentities using sequence context after deciding");
            return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetHttpClient(requestContext).ReadIdentitiesAsync((IList<Guid>) cacheMissesIdentityIds, requestHeadersContext, queryMembership, propertyNameFilters, includeRestrictedVisibility).SyncResult<IdentitiesCollection>();
          }
          requestContext.Trace(2432359, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityStore), "ReadIndentities without sequence context after deciding");
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetHttpClient(requestContext).ReadIdentitiesAsync((IList<Guid>) cacheMissesIdentityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility, (object) null, new CancellationToken()).SyncResult<IdentitiesCollection>();
        }), (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() => (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[cacheMissesIdentityIds.Count]));
        FrameworkIdentityStore.CheckForLeakedMasterIds(requestContext, hostDomain, (IList<Guid>) cacheMissesIdentityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility, identityList);
        for (int index1 = 0; index1 < identityList.Count; ++index1)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index1];
          if (identity != null)
          {
            int index2 = source2[index1].Item1;
            results[index2] = identity;
            this.CacheIdentity(requestContext, hostDomain, queryMembership, propertyNameFilters, identity, flag2);
          }
        }
        this.UpdateIdentitiesInImsCache(requestContext, hostDomain, queryMembership, cacheMissesIdentityIds, identityList);
      }
      this.CheckIdentitiesForInvalidKeys(requestContext, results);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results;
    }

    private void InvalidateSequenceContext(IVssRequestContext requestContext)
    {
      if (!FrameworkIdentityStore.UseMinSequenceContext(requestContext))
        return;
      this.InvalidateSequenceContextInternal(requestContext);
    }

    private static bool UseMinSequenceContext(IVssRequestContext requestContext)
    {
      if ((requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? 0 : (requestContext.IsImsFeatureEnabled("VisualStudio.Services.Identity.HttpRequests.PopulateMinSequenceIdRequestHeaders") ? 1 : 0)) == 0)
        return false;
      try
      {
        requestContext.TraceConditionally(2432357, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityStore), (Func<string>) (() => string.Format("isOrganizationActivated={0}; {1}", (object) requestContext.IsOrganizationActivated(), (object) requestContext.IsOrganizationAadBacked())));
        return requestContext.IsOrganizationActivated() || requestContext.IsOrganizationAadBacked() && requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.HttpRequests.AadBackedOrgs.PopulateMinSequenceIdRequestHeaders");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(8000200, "IdentityService", nameof (FrameworkIdentityStore), ex);
        return false;
      }
    }

    private static void CheckForLeakedMasterIds(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> outputIdentities)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.Framework.DisableTraceErrorIfIdIsMasterId") || requestContext.RootContext.Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.IsProcessingAuthenticationModules))
        return;
      List<Guid> guidList = (List<Guid>) null;
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = (List<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      for (int index = 0; index < identityIds.Count; ++index)
      {
        Guid identityId = identityIds[index];
        Microsoft.VisualStudio.Services.Identity.Identity outputIdentity = outputIdentities[index];
        if (outputIdentity != null && outputIdentity.Id != identityId)
        {
          if (guidList == null)
          {
            guidList = new List<Guid>();
            identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
          }
          guidList.Add(identityId);
          identityList.Add(outputIdentity);
        }
      }
      if (guidList == null)
        return;
      requestContext.TraceSerializedConditionally(80118, TraceLevel.Error, "IdentityService", nameof (FrameworkIdentityStore), true, "Detected master ID leak: leakedMasterIds = {0}, leakedMasterIdIdentities = {1}, DomainId = {2}, queryMembership = {3}, propertyNameFilters = {4}, includeRestrictedVisibility = {5}", (object) guidList, (object) identityList, (object) hostDomain.DomainId, (object) queryMembership, (object) propertyNameFilters, (object) includeRestrictedVisibility);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      ReadIdentitiesOptions options = ReadIdentitiesOptions.None)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = (List<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      bool flag = this.UseExtendedPropertiesCache(requestContext, propertyNameFilters);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && searchFactor == IdentitySearchFilter.AccountName)
      {
        if (factorValue.Contains("\\"))
          identity1 = this.m_cache.ReadIdentity(requestContext, hostDomain, searchFactor, factorValue, queryMembership);
      }
      else
        identity1 = this.m_cache.ReadIdentity(requestContext, hostDomain, searchFactor, factorValue, queryMembership);
      if (identity1 != null)
        identityList1 = new List<Microsoft.VisualStudio.Services.Identity.Identity>() { identity1 };
      if (identityList1 == null)
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source = this.ReadIdentitiesFromImsCache(requestContext, hostDomain, queryMembership, searchFactor, factorValue);
        if (source != null)
        {
          identityList1 = source.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
          foreach (Microsoft.VisualStudio.Services.Identity.Identity identity2 in identityList1)
            this.CacheIdentity(requestContext, hostDomain, queryMembership, (IEnumerable<string>) null, identity2, flag);
        }
      }
      if (identityList1 != null)
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity3 in identityList1)
        {
          if (this.TryEnrichIdentityProperties(requestContext, hostDomain, propertyNameFilters, identity3, flag))
          {
            identityList2.Add(identity3);
          }
          else
          {
            identityList1 = (List<Microsoft.VisualStudio.Services.Identity.Identity>) null;
            break;
          }
        }
      }
      if (identityList1 != null)
      {
        this.IncrementCacheHitPerfCounters();
        requestContext.Trace(80106, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Cache hit when reading identity - {0}, {1}, {2}", (object) searchFactor, (object) factorValue, (object) queryMembership);
      }
      else
      {
        this.IncrementCacheMissPerfCounters();
        requestContext.Trace(80107, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Cache miss when reading identity - {0}, {1}, {2}", (object) searchFactor, (object) factorValue, (object) queryMembership);
      }
      if (identityList1 == null)
      {
        IEnumerable<string> propertiesToFetch = this.GetPropertiesToFetch(propertyNameFilters, flag, queryMembership, searchFilter: new IdentitySearchFilter?(searchFactor));
        IVssRequestContext requestContext1 = requestContext;
        int identityTraceKind = (int) IdentityTracing.CovertToReadIdentityTraceKind(searchFactor);
        int num = (int) queryMembership;
        IEnumerable<string> source = propertiesToFetch;
        int extendedPropertiesCount = source != null ? source.Count<string>() : 0;
        int identityTracePoint = IdentityTracing.ConvertToReadIdentityTracePoint((IdentityTracing.ReadIdentityTraceKind) identityTraceKind, (QueryMembership) num, extendedPropertiesCount, 0);
        Func<string> message = (Func<string>) (() => string.Format("FrameworkIdentityStore.ReadIdentities where searchFactor : {0}, filterValue : {1}, queryMembership : {2}, propertyNames : {3}, stackTrace: {4}", (object) searchFactor, (object) factorValue, (object) queryMembership, propertiesToFetch == null ? (object) string.Empty : (object) propertiesToFetch.Serialize<IEnumerable<string>>(), (object) EnvironmentWrapper.ToReadableStackTrace()));
        requestContext1.TraceConditionally(identityTracePoint, TraceLevel.Verbose, "IdentityService", "FrameworkIdentityStoreReadIdentities", message);
        identityList2 = this.SafeReadIdentities(requestContext, (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
        {
          requestContext.Trace(2432360, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityStore), "ReadIndentities without sequence context without deciding");
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetHttpClient(requestContext).ReadIdentitiesAsync(searchFactor, factorValue, options, queryMembership, propertiesToFetch, (object) null, new CancellationToken()).SyncResult<IdentitiesCollection>();
        }), (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() => (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
        {
          (Microsoft.VisualStudio.Services.Identity.Identity) null
        }));
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity4 in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList2)
        {
          if (identity4 != null)
            this.CacheIdentity(requestContext, hostDomain, queryMembership, propertiesToFetch, identity4, flag);
        }
        this.UpdateIdentitiesInImsCache(requestContext, hostDomain, queryMembership, searchFactor, factorValue, identityList2);
      }
      this.CheckIdentitiesForInvalidKeys(requestContext, identityList2.ToArray<Microsoft.VisualStudio.Services.Identity.Identity>());
      return identityList2;
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> SafeReadIdentities(
      IVssRequestContext requestContext,
      Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>> happyPath,
      Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>> onExceptionResults)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> Identities;
      try
      {
        Identities = happyPath();
        IdentityHelper.LogInvalidServiceIdentityWhenNecessary(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) Identities, true);
      }
      catch (IdentityNotFoundException ex)
      {
        requestContext.TraceException(8000169, "IdentityService", nameof (FrameworkIdentityStore), (Exception) ex);
        Identities = onExceptionResults();
      }
      return Identities;
    }

    private static bool TryPopulatePropertyUserId(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (!IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) identity))
      {
        requestContext.TraceDataConditionally(54508640, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Skipping population of property UserId for non-user identity", (Func<object>) (() => (object) new
        {
          identity = identity
        }), nameof (TryPopulatePropertyUserId));
        return false;
      }
      if (requestContext.IsDeploymentFallbackIdentityReadAllowed())
      {
        requestContext.TraceDataConditionally(54508641, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Skipping population of property UserId since fallback identity reads are still enabled", (Func<object>) (() => (object) new
        {
          identity = identity
        }), nameof (TryPopulatePropertyUserId));
        return false;
      }
      if (identity.SubjectDescriptor == new SubjectDescriptor())
      {
        Guid stackTraceId = Guid.NewGuid();
        requestContext.TraceDataConditionally(54508642, TraceLevel.Error, "IdentityService", nameof (FrameworkIdentityStore), "Skipping population of property UserId for user identity that does not have a subject descriptor", (Func<object>) (() => (object) new
        {
          identity = identity,
          stackTraceId = stackTraceId
        }), nameof (TryPopulatePropertyUserId));
        requestContext.Trace(54508642, TraceLevel.Error, "IdentityService", nameof (FrameworkIdentityStore), string.Format("Stack trace {0}: {1}", (object) stackTraceId, (object) Environment.StackTrace));
        return false;
      }
      requestContext.TraceDataConditionally(54508643, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Trying to retrieve property UserId by calling UserIdentifierConversionService", (Func<object>) (() => (object) new
      {
        identity = identity
      }), nameof (TryPopulatePropertyUserId));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IUserIdentifierConversionService service = vssRequestContext.GetService<IUserIdentifierConversionService>();
      Guid guid = Guid.Empty;
      try
      {
        guid = service.GetStorageKeyByDescriptor(vssRequestContext, identity.SubjectDescriptor);
      }
      catch (UserDoesNotExistException ex)
      {
        requestContext.TraceException(54508644, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), (Exception) ex);
      }
      if (guid == Guid.Empty)
      {
        Guid stackTraceId = Guid.NewGuid();
        requestContext.TraceDataConditionally(54508645, TraceLevel.Error, "IdentityService", nameof (FrameworkIdentityStore), "Failed to retrieve property UserId by calling UserIdentifierConversionService", (Func<object>) (() => (object) new
        {
          identity = identity,
          stackTraceId = stackTraceId
        }), nameof (TryPopulatePropertyUserId));
        requestContext.Trace(54508646, TraceLevel.Error, "IdentityService", nameof (FrameworkIdentityStore), string.Format("Stack trace {0}: {1}", (object) stackTraceId, (object) Environment.StackTrace));
        return false;
      }
      identity.SetProperty("UserId", (object) guid);
      requestContext.TraceDataConditionally(54508647, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Successfully retrieved property UserId by calling UserIdentifierConversionService", (Func<object>) (() => (object) new
      {
        identity = identity
      }), nameof (TryPopulatePropertyUserId));
      return true;
    }

    private bool TryEnrichIdentityProperties(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool useExtendedPropertiesCache)
    {
      if (propertyNameFilters.IsNullOrEmpty<string>())
      {
        requestContext.TraceDataConditionally(54508820, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "No properties to enrich", (Func<object>) (() => (object) new
        {
          DomainId = hostDomain.DomainId,
          propertyNameFilters = propertyNameFilters,
          identity = identity,
          useExtendedPropertiesCache = useExtendedPropertiesCache
        }), nameof (TryEnrichIdentityProperties));
        return true;
      }
      requestContext.TraceDataConditionally(54508821, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Trying to enrich identity properties (from property cache and user identifier service)", (Func<object>) (() => (object) new
      {
        DomainId = hostDomain.DomainId,
        propertyNameFilters = propertyNameFilters,
        identity = identity,
        useExtendedPropertiesCache = useExtendedPropertiesCache
      }), nameof (TryEnrichIdentityProperties));
      if (propertyNameFilters.Contains<string>("UserId", (IEqualityComparer<string>) VssStringComparer.UserId) && FrameworkIdentityStore.TryPopulatePropertyUserId(requestContext, identity))
      {
        propertyNameFilters = propertyNameFilters.Except<string>((IEnumerable<string>) new string[1]
        {
          "UserId"
        }, (IEqualityComparer<string>) VssStringComparer.UserId);
        requestContext.TraceDataConditionally(54508822, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Enriched UserId property from user identifier service and removed from property list", (Func<object>) (() => (object) new
        {
          DomainId = hostDomain.DomainId,
          propertyNameFilters = propertyNameFilters,
          identity = identity,
          useExtendedPropertiesCache = useExtendedPropertiesCache
        }), nameof (TryEnrichIdentityProperties));
      }
      if (propertyNameFilters.IsNullOrEmpty<string>())
      {
        requestContext.TraceDataConditionally(54508823, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "No more properties to enrich", (Func<object>) (() => (object) new
        {
          DomainId = hostDomain.DomainId,
          propertyNameFilters = propertyNameFilters,
          identity = identity,
          useExtendedPropertiesCache = useExtendedPropertiesCache
        }), nameof (TryEnrichIdentityProperties));
        return true;
      }
      requestContext.TraceDataConditionally(54508824, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Trying to enrich identity properties (from property cache)", (Func<object>) (() => (object) new
      {
        DomainId = hostDomain.DomainId,
        propertyNameFilters = propertyNameFilters,
        identity = identity,
        useExtendedPropertiesCache = useExtendedPropertiesCache
      }), nameof (TryEnrichIdentityProperties));
      if (useExtendedPropertiesCache && this.m_cache.EnrichIdentityProperties(requestContext, hostDomain, propertyNameFilters, identity))
      {
        requestContext.TraceDataConditionally(54508825, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Enriched remaining properties from property cache", (Func<object>) (() => (object) new
        {
          DomainId = hostDomain.DomainId,
          propertyNameFilters = propertyNameFilters,
          identity = identity,
          useExtendedPropertiesCache = useExtendedPropertiesCache
        }), nameof (TryEnrichIdentityProperties));
        return true;
      }
      requestContext.TraceDataConditionally(54508826, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Failed to enrich properties", (Func<object>) (() => (object) new
      {
        DomainId = hostDomain.DomainId,
        propertyNameFilters = propertyNameFilters,
        identity = identity,
        useExtendedPropertiesCache = useExtendedPropertiesCache
      }), nameof (TryEnrichIdentityProperties));
      return false;
    }

    public FilteredIdentitiesList ReadFilteredIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      IList<IdentityDescriptor> descriptors,
      IEnumerable<IdentityFilter> filters,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> unfilteredIdentities = descriptors == null ? this.ReadIdentitiesInScope(requestContext, hostDomain, scopeId, queryMembership, propertyNameFilters) : this.ReadIdentities(requestContext, hostDomain, descriptors, queryMembership, propertyNameFilters, false);
      return IdentityFilterHelper.FilterIdentities(requestContext, hostDomain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) unfilteredIdentities, filters, suggestedPageSize, lastSearchResult, lookForward);
    }

    protected void OnIdentityPropertiesChanged(
      object source,
      IdentityPropertiesChangeEventArgs args)
    {
      EventHandler<IdentityPropertiesChangeEventArgs> propertiesChanged = this.IdentityPropertiesChanged;
      if (propertiesChanged == null)
        return;
      propertiesChanged(source, args);
    }

    internal override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesInScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = this.CommandReadIdentitiesInScope(requestContext, hostDomain, scopeId, queryMembership, propertyNameFilters);
      if (source != null)
        this.UpdateMegaTenantState(requestContext, source.Count);
      this.CheckIdentitiesForInvalidKeys(requestContext, source.ToArray<Microsoft.VisualStudio.Services.Identity.Identity>());
      return source;
    }

    internal ChangedIdentities GetIdentityChanges(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      ChangedIdentitiesContext sequenceContext)
    {
      return this.GetHttpClient(requestContext).GetIdentityChangesAsync(sequenceContext.IdentitySequenceId, sequenceContext.GroupSequenceId, sequenceContext.OrganizationIdentitySequenceId, sequenceContext.PageSize, hostDomain.DomainId, (object) null, new CancellationToken()).SyncResult<ChangedIdentities>();
    }

    public bool UpdateIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      this.FilterExtendedPropertiesWithNoUpdates(requestContext, hostDomain, identities);
      IEnumerable<IdentityUpdateData> source = this.GetHttpClient(requestContext).UpdateIdentitiesAsync(identities, (object) null, new CancellationToken()).SyncResult<IEnumerable<IdentityUpdateData>>();
      this.ProcessExtendedPropertyChanges(requestContext, hostDomain, identities);
      foreach (IdentityUpdateData identityUpdateData in source)
        identities[identityUpdateData.Index].Id = identityUpdateData.Id;
      List<Guid> identityChanges = (List<Guid>) null;
      List<Guid> groupChanges = (List<Guid>) null;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        if (identity != null)
        {
          if (IdentityValidation.IsTeamFoundationType(identity.Descriptor))
          {
            if (groupChanges == null)
              groupChanges = new List<Guid>();
            groupChanges.Add(identity.Id);
          }
          else
          {
            if (identityChanges == null)
              identityChanges = new List<Guid>();
            identityChanges.Add(identity.Id);
          }
        }
      }
      IdentityHelper.LogInvalidServiceIdentityWhenNecessary(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities, true);
      bool flag = false;
      if (source.Any<IdentityUpdateData>((Func<IdentityUpdateData, bool>) (x => x.Updated)))
      {
        this.InvalidateSequenceContext(requestContext);
        this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, (ICollection<Guid>) null, (ICollection<Guid>) identityChanges, (ICollection<Guid>) groupChanges, (ICollection<MembershipChangeInfo>) null);
        flag = true;
      }
      return flag;
    }

    internal void InvalidateIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      ICollection<Guid> identityIds)
    {
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, (ICollection<Guid>) null, identityIds, (ICollection<Guid>) null, (ICollection<MembershipChangeInfo>) null);
    }

    public AccessTokenResult GetSignoutToken(IVssRequestContext requestContext)
    {
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return this.GetHttpClient(requestContext).GetSignoutToken((object) null, new CancellationToken()).SyncResult<AccessTokenResult>();
    }

    internal void CheckIdentitiesForInvalidKeys(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity[] identities)
    {
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in identities)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identity1;
        if (identity != null)
        {
          if (identity.Id == Guid.Empty)
          {
            requestContext.TraceDataConditionally(80124, TraceLevel.Error, "IdentityService", nameof (FrameworkIdentityStore), IdentityResources.InvalidIdentityIdException((object) identity), (Func<object>) (() => (object) identity), nameof (CheckIdentitiesForInvalidKeys));
            requestContext.TraceConditionally(80131, TraceLevel.Error, "IdentityService", nameof (FrameworkIdentityStore), (Func<string>) (() => Environment.StackTrace));
            throw new InvalidIdentityKeyException(IdentityResources.InvalidIdentityIdException((object) identity));
          }
          if (identity.Descriptor == (IdentityDescriptor) null)
          {
            requestContext.TraceDataConditionally(80127, TraceLevel.Error, "IdentityService", nameof (FrameworkIdentityStore), IdentityResources.InvalidIdentityDescriptorException((object) identity), (Func<object>) (() => (object) identity), nameof (CheckIdentitiesForInvalidKeys));
            requestContext.TraceConditionally(80132, TraceLevel.Error, "IdentityService", nameof (FrameworkIdentityStore), (Func<string>) (() => Environment.StackTrace));
            throw new InvalidIdentityKeyException(IdentityResources.InvalidIdentityDescriptorException((object) identity));
          }
        }
      }
    }

    private void OnIdentityChangedNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(899001, "IdentityService", nameof (FrameworkIdentityStore), nameof (OnIdentityChangedNotification));
      IdentityMessageBusData identityMessageBusData = TeamFoundationSerializationUtility.Deserialize<IdentityMessageBusData>(eventData);
      TeamFoundationTask task = (TeamFoundationTask) null;
      requestContext.TraceSerializedConditionally(899002, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), false, "OnIdentityChangedNotification parameters, eventClass = {0}, sqlNotification = {1} ", (object) eventClass, (object) identityMessageBusData);
      using (requestContext.Lock(this.m_processChangesTaskLockName))
      {
        this.m_identityUpdates.Add(identityMessageBusData);
        if (this.m_processChangesTaskState == IdentityStoreBase.TaskState.NotQueued)
        {
          task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessChangesTask), (object) null, 0);
          this.m_processChangesTaskState = IdentityStoreBase.TaskState.Queueing;
        }
      }
      if (task != null)
      {
        try
        {
          requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext.ServiceHost.InstanceId, task);
        }
        catch (Exception ex)
        {
          this.m_processChangesTaskState = IdentityStoreBase.TaskState.NotQueued;
          throw;
        }
        using (requestContext.Lock(this.m_processChangesTaskLockName))
        {
          if (this.m_processChangesTaskState == IdentityStoreBase.TaskState.Queueing)
            this.m_processChangesTaskState = IdentityStoreBase.TaskState.Queued;
        }
      }
      requestContext.TraceLeave(899009, "IdentityService", nameof (FrameworkIdentityStore), nameof (OnIdentityChangedNotification));
    }

    private void ProcessChangesTask(IVssRequestContext requestContext, object taskArgs)
    {
      List<IdentityMessageBusData> identityMessageBusDataList = (List<IdentityMessageBusData>) null;
      using (requestContext.Lock(this.m_processChangesTaskLockName))
      {
        identityMessageBusDataList = new List<IdentityMessageBusData>((IEnumerable<IdentityMessageBusData>) this.m_identityUpdates);
        this.m_identityUpdates.Clear();
        this.m_processChangesTaskState = IdentityStoreBase.TaskState.NotQueued;
      }
      foreach (IdentityMessageBusData identityMessageBusData in identityMessageBusDataList)
        this.ProcessIdentityChange(requestContext, this.m_masterDomain, (ICollection<Guid>) identityMessageBusData.DescriptorChanges, (ICollection<Guid>) identityMessageBusData.DescriptorChangesWithMasterId, (ICollection<Guid>) identityMessageBusData.IdentityChanges, (ICollection<Guid>) identityMessageBusData.GroupChanges, (ICollection<MembershipChangeInfo>) identityMessageBusData.MembershipChanges, identityMessageBusData.DescriptorChangeType, (ICollection<GroupScopeVisibiltyChangeInfo>) identityMessageBusData.GroupScopeVisibiltyChanges, false, identityMessageBusData.IdentitySequenceId, identityMessageBusData.GroupSequenceId);
    }

    private void ProcessMembershipChangesOnAuthor(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      ICollection<MembershipChangeInfo> membershipChanges)
    {
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, (ICollection<Guid>) null, (ICollection<Guid>) null, (ICollection<Guid>) null, membershipChanges);
    }

    private void ProcessIdentityChangeOnAuthor(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      ICollection<Guid> descriptorChanges,
      ICollection<Guid> identityChanges,
      ICollection<Guid> groupChanges,
      ICollection<MembershipChangeInfo> membershipChanges)
    {
      this.ProcessIdentityChange(requestContext, hostDomain, descriptorChanges, (ICollection<Guid>) null, identityChanges, groupChanges, membershipChanges, DescriptorChangeType.None, (ICollection<GroupScopeVisibiltyChangeInfo>) null, true);
      this.FireEvents(requestContext, DescriptorChangeType.None, FrameworkIdentityStore.Count<Guid>(descriptorChanges), FrameworkIdentityStore.Count<Guid>(identityChanges), FrameworkIdentityStore.Count<Guid>(groupChanges), FrameworkIdentityStore.Count<MembershipChangeInfo>(membershipChanges));
    }

    private void ProcessIdentityChange(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      ICollection<Guid> descriptorChanges,
      ICollection<Guid> descriptorChangeMasterIds,
      ICollection<Guid> identityChanges,
      ICollection<Guid> groupChanges,
      ICollection<MembershipChangeInfo> membershipChanges,
      DescriptorChangeType descriptorChangeType,
      ICollection<GroupScopeVisibiltyChangeInfo> groupScopeVisibiltyChanges,
      bool useTaskToBroadcast,
      long identitySequenceId = -1,
      long groupSequenceId = -1)
    {
      requestContext.TraceEnter(8000161, "IdentityService", nameof (FrameworkIdentityStore), nameof (ProcessIdentityChange));
      bool flag = false;
      identityChanges = (ICollection<Guid>) ((object) identityChanges ?? (object) Array.Empty<Guid>());
      groupChanges = (ICollection<Guid>) ((object) groupChanges ?? (object) Array.Empty<Guid>());
      requestContext.TraceSerializedConditionally(8000162, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), false, "ProcessIdentityChange input parameter, hostDomain = {0} , descriptorChanges = {1} , descriptorChangeMasterIds = {2} , identityChanges = {3} , groupChanges = {4} , membershipChanges = {5} , descriptorChangeType = {6} , groupScopeVisibiltyChanges = {7} , useTaskToBroadcast = {8}", (object) hostDomain, (object) descriptorChanges, (object) descriptorChangeMasterIds, (object) identityChanges, (object) groupChanges, (object) membershipChanges, (object) descriptorChangeType, (object) groupScopeVisibiltyChanges, (object) useTaskToBroadcast);
      this.ProcessGroupSequenceIdChange(groupSequenceId);
      try
      {
        if (!identityChanges.IsNullOrEmpty<Guid>() || !groupChanges.IsNullOrEmpty<Guid>() || !descriptorChanges.IsNullOrEmpty<Guid>())
        {
          List<Guid> identityChanges1 = new List<Guid>((IEnumerable<Guid>) identityChanges);
          identityChanges1.AddRange((IEnumerable<Guid>) groupChanges);
          if (!descriptorChanges.IsNullOrEmpty<Guid>())
            identityChanges1.AddRange((IEnumerable<Guid>) descriptorChanges);
          requestContext.GetService<IImsCacheService>().ProcessChanges(requestContext, hostDomain.DomainId, (IList<Guid>) identityChanges1);
        }
        if (!membershipChanges.IsNullOrEmpty<MembershipChangeInfo>())
          requestContext.GetService<IImsCacheService>().ProcessChanges(requestContext, hostDomain.DomainId, membershipChanges);
        IdentitySearchHelper.ProcessChangesOnSearchCaches(requestContext, groupScopeVisibiltyChanges);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(8000168, "IdentityService", nameof (FrameworkIdentityStore), ex);
        throw;
      }
      lock (this.m_processChangesLock)
        flag = this.ProcessChangesOnCache(requestContext, descriptorChanges, identityChanges, groupChanges, membershipChanges, descriptorChangeType, useTaskToBroadcast);
      if (descriptorChangeType != DescriptorChangeType.None)
      {
        EventHandler<DescriptorChangeNotificationEventArgs> changedNotification = this.DescriptorsChangedNotification;
        if (changedNotification != null)
        {
          DescriptorChangeNotificationEventArgs e = new DescriptorChangeNotificationEventArgs(requestContext, descriptorChangeType, descriptorChangeMasterIds);
          changedNotification((object) this, e);
        }
      }
      if (flag)
      {
        EventHandler<DescriptorChangeEventArgs> descriptorsChanged = this.DescriptorsChanged;
        if (descriptorsChanged != null)
        {
          DescriptorChangeEventArgs e = new DescriptorChangeEventArgs(requestContext);
          descriptorsChanged((object) this, e);
        }
      }
      if (!useTaskToBroadcast)
        this.BroadcastChangesIfNecessary(requestContext, identityChanges, descriptorChanges, membershipChanges, useTaskToBroadcast);
      if (descriptorChangeType == DescriptorChangeType.Major)
        this.FireEvents(requestContext, descriptorChangeType, FrameworkIdentityStore.Count<Guid>(descriptorChanges), FrameworkIdentityStore.Count<Guid>(identityChanges), FrameworkIdentityStore.Count<Guid>(groupChanges), FrameworkIdentityStore.Count<MembershipChangeInfo>(membershipChanges));
      requestContext.TraceLeave(8000169, "IdentityService", nameof (FrameworkIdentityStore), nameof (ProcessIdentityChange));
    }

    private bool ProcessChangesOnCache(
      IVssRequestContext requestContext,
      ICollection<Guid> descriptorChanges,
      ICollection<Guid> identityChanges,
      ICollection<Guid> groupChanges,
      ICollection<MembershipChangeInfo> membershipChanges,
      DescriptorChangeType descriptorChangeType,
      bool useTaskToBroadcast)
    {
      requestContext.TraceEnter(899011, "IdentityService", nameof (FrameworkIdentityStore), nameof (ProcessChangesOnCache));
      requestContext.TraceSerializedConditionally(899012, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), false, "ProcessChangesOnCache input parameters, descriptorChanges = {0}, identityChanges = {1}, groupChanges = {2}, membershipChanges = {3}, descriptorChangeType = {4}, useTaskToBroadcast = {5}", (object) descriptorChanges, (object) identityChanges, (object) groupChanges, (object) membershipChanges, (object) descriptorChangeType, (object) useTaskToBroadcast);
      bool flag;
      if (descriptorChangeType == DescriptorChangeType.Major)
      {
        this.m_cache.Clear(requestContext);
        flag = true;
      }
      else
      {
        flag = this.m_cache.ProcessChanges(requestContext, descriptorChanges, (ICollection<Guid>) null, (ICollection<Guid>) null, (ICollection<MembershipChangeInfo>) null, (ICollection<Guid>) null, (SequenceContext) null);
        this.m_cache.ProcessChanges(requestContext, (ICollection<Guid>) null, identityChanges, groupChanges, membershipChanges, (ICollection<Guid>) null, (SequenceContext) null);
      }
      if (useTaskToBroadcast)
        this.BroadcastChangesIfNecessary(requestContext, identityChanges, descriptorChanges, membershipChanges, useTaskToBroadcast);
      requestContext.TraceLeave(899019, "IdentityService", nameof (FrameworkIdentityStore), nameof (ProcessChangesOnCache));
      Interlocked.Increment(ref this.m_changeId);
      return flag;
    }

    private void ProcessIdentityChangeOnAuthor(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      ICollection<IdentityDescriptor> groupChanges)
    {
      this.m_cache.ProcessChanges(requestContext, hostDomain, groupChanges);
      this.FireEvents(requestContext, DescriptorChangeType.Major, groupChangeCount: FrameworkIdentityStore.Count<IdentityDescriptor>(groupChanges));
    }

    private void InvalidateAccessControlEntries(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId)?.OnDataChanged(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(8000178, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
    }

    private void BroadcastChangesIfNecessary(
      IVssRequestContext requestContext,
      ICollection<Guid> identityChangeIds,
      ICollection<Guid> descriptorChangeIds,
      ICollection<MembershipChangeInfo> membershipChanges,
      bool useTaskToBroadcast)
    {
      if ((descriptorChangeIds == null || descriptorChangeIds.Count <= 0) && (identityChangeIds == null || identityChangeIds.Count <= 0) && (membershipChanges == null || membershipChanges.Count <= 0))
        return;
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Aad.EnableReduceAadUserMembershipsLatency"))
        membershipChanges = (ICollection<MembershipChangeInfo>) null;
      EventHandler<IdentityChangeEventArgs> identitiesChanged = this.IdentitiesChanged;
      if (identitiesChanged == null)
        return;
      IdentityChangeEventArgs identityChangeEventArgs = new IdentityChangeEventArgs()
      {
        RequestContext = requestContext,
        PropertyChangeIds = identityChangeIds,
        DescriptorChangeIds = descriptorChangeIds,
        MembershipChangeInfos = membershipChanges
      };
      if (useTaskToBroadcast)
      {
        TeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.BroadcastIdentityChanges), (object) identityChangeEventArgs, 0);
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(instanceId, task);
      }
      else
      {
        try
        {
          identitiesChanged((object) this, identityChangeEventArgs);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, "IdentityService", nameof (FrameworkIdentityStore), ex);
        }
      }
    }

    private void BroadcastIdentityChanges(IVssRequestContext requestContext, object taskArgs)
    {
      IdentityChangeEventArgs e = (IdentityChangeEventArgs) taskArgs;
      e.RequestContext = requestContext;
      EventHandler<IdentityChangeEventArgs> identitiesChanged = this.IdentitiesChanged;
      if (identitiesChanged == null)
        return;
      try
      {
        identitiesChanged((object) this, e);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
    }

    private void OnParentIdentityChangeEvent(object sender, IdentityChangeEventArgs e)
    {
      try
      {
        this.ProcessParentIdentityChange(e.RequestContext, e.PropertyChangeIds, e.DescriptorChangeIds, e.MembershipChangeInfos);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
    }

    private void ProcessParentIdentityChange(
      IVssRequestContext requestContext,
      ICollection<Guid> propertyChangeIds,
      ICollection<Guid> descriptorChangeIds,
      ICollection<MembershipChangeInfo> membershipChangeInfos)
    {
      bool flag = false;
      lock (this.m_processChangesLock)
        flag = this.ProcessParentIdentityChangeOnCache(requestContext, propertyChangeIds, descriptorChangeIds, membershipChangeInfos);
      if (!flag)
        return;
      EventHandler<DescriptorChangeEventArgs> descriptorsChanged = this.DescriptorsChanged;
      if (descriptorsChanged == null)
        return;
      DescriptorChangeEventArgs e = new DescriptorChangeEventArgs(requestContext);
      descriptorsChanged((object) this, e);
    }

    private bool ProcessParentIdentityChangeOnCache(
      IVssRequestContext requestContext,
      ICollection<Guid> propertyChangeIds,
      ICollection<Guid> descriptorChangeIds,
      ICollection<MembershipChangeInfo> membershipChangeInfos)
    {
      int num = this.m_cache.ProcessChanges(requestContext, descriptorChangeIds, (ICollection<Guid>) null, (ICollection<Guid>) null, (ICollection<MembershipChangeInfo>) null, (ICollection<Guid>) null, (SequenceContext) null) ? 1 : 0;
      this.m_cache.ProcessChanges(requestContext, (ICollection<Guid>) null, propertyChangeIds, (ICollection<Guid>) null, membershipChangeInfos, (ICollection<Guid>) null, (SequenceContext) null);
      Interlocked.Increment(ref this.m_changeId);
      return num != 0;
    }

    private void OnParentDescriptorChangeNotifcationEvent(
      object sender,
      DescriptorChangeNotificationEventArgs e)
    {
      try
      {
        EventHandler<DescriptorChangeNotificationEventArgs> changedNotification = this.DescriptorsChangedNotification;
        if (changedNotification == null)
          return;
        DescriptorChangeNotificationEventArgs notificationEventArgs = new DescriptorChangeNotificationEventArgs(e.RequestContext, e.DescriptorChangeType, e.DescriptorChangeIds);
        changedNotification((object) this, e);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
    }

    private void FireEvents(
      IVssRequestContext requestContext,
      DescriptorChangeType descriptorChangeType,
      int descriptorChangeCount = 0,
      int identityChangeCount = 0,
      int groupChangeCount = 0,
      int membershipChangeCount = 0)
    {
      requestContext.Trace(80113, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "FireEvents: Descriptor change type {0}, descriptor change count: {1}, identity change count: {2}, group change count: {3}, membership change count: {4}", (object) descriptorChangeType, (object) descriptorChangeCount, (object) identityChangeCount, (object) groupChangeCount, (object) membershipChangeCount);
      requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new IdentityChangedNotification(-1));
    }

    internal event EventHandler<DescriptorChangeEventArgs> DescriptorsChanged;

    internal event EventHandler<DescriptorChangeNotificationEventArgs> DescriptorsChangedNotification;

    internal event EventHandler<IdentityChangeEventArgs> IdentitiesChanged;

    internal event EventHandler<IdentityPropertiesChangeEventArgs> IdentityPropertiesChanged;

    internal override IdentityDomain Domain => this.m_masterDomain;

    internal override IIdentityCache IdentityCache => (IIdentityCache) this.m_cache;

    internal int GetCurrentChangeId() => this.m_changeId;

    protected override SequenceContext GetLatestSequenceContext(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain)
    {
      ChangedIdentitiesContext sequenceContext = this.GetHttpClient(requestContext).GetIdentityChangesAsync(int.MaxValue, int.MaxValue, int.MaxValue, hostDomain.DomainId, (object) null, new CancellationToken()).SyncResult<ChangedIdentities>().SequenceContext;
      return new SequenceContext((long) sequenceContext.IdentitySequenceId, (long) sequenceContext.GroupSequenceId, (long) sequenceContext.OrganizationIdentitySequenceId);
    }

    internal virtual IdentityHttpClient GetHttpClient(IVssRequestContext context) => context.GetClient<IdentityHttpClient>();

    private void IncrementCacheMissPerfCounters()
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_cache_misses").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_cache_misses_persec").Increment();
    }

    private void IncrementCacheHitPerfCounters() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_cache_hits").Increment();

    private void FilterExtendedPropertiesWithNoUpdates(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      if (!requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.IdentityStore.CacheExtendedProperties"))
        return;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        HashSet<string> modifiedProperties = identity != null ? identity.GetModifiedProperties() : (HashSet<string>) null;
        if (modifiedProperties != null)
          this.m_cache.FilterUnchangedIdentityProperties(requestContext, hostDomain, (IEnumerable<string>) modifiedProperties, identity);
      }
    }

    private IEnumerable<string> GetPropertiesToFetch(
      IEnumerable<string> propertyNameFilters,
      bool useExtendedPropertiesCache,
      QueryMembership queryMembership,
      IEnumerable<IdentityDescriptor> descriptors = null,
      IdentitySearchFilter? searchFilter = null,
      IEnumerable<SubjectDescriptor> subjectDescriptors = null)
    {
      if (!useExtendedPropertiesCache || queryMembership > QueryMembership.Direct || descriptors != null && descriptors.Any<IdentityDescriptor>() && descriptors.All<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null && x.Identifier != null && x.Identifier.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix))) || subjectDescriptors != null && subjectDescriptors.Any<SubjectDescriptor>() && subjectDescriptors.All<SubjectDescriptor>((Func<SubjectDescriptor, bool>) (x => x.Identifier != null && x.Identifier.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix))) || searchFilter.HasValue && searchFilter.Value != IdentitySearchFilter.AccountName)
        return propertyNameFilters;
      IEnumerable<string> prefetchedProperties = this.m_cache.GetPrefetchedProperties();
      if (propertyNameFilters == null)
        return prefetchedProperties;
      return prefetchedProperties == null ? propertyNameFilters : propertyNameFilters.Union<string>(prefetchedProperties);
    }

    private void ProcessExtendedPropertyChanges(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      if (!requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.IdentityStore.CacheExtendedProperties"))
        return;
      bool flag = this.UpdateIdentityProperties(requestContext, identities);
      if (this.m_parentIdentityStore == null || !flag)
        return;
      requestContext.Trace(80116, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "ParentIdentityStore Extended property cache update on identity update");
      this.m_parentIdentityStore.UpdateIdentityProperties(requestContext, identities);
    }

    private bool UpdateIdentityProperties(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      if (!requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.IdentityStore.CacheExtendedProperties"))
        return false;
      requestContext.Trace(80116, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Extended property cache update on identity update - # of identities: {0}", (object) (identities != null ? identities.Count : 0));
      Dictionary<Guid, Dictionary<string, object>> dictionary = new Dictionary<Guid, Dictionary<string, object>>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        HashSet<string> modifiedProperties = identity != null ? identity.GetModifiedProperties() : (HashSet<string>) null;
        if (modifiedProperties != null)
        {
          requestContext.Trace(80116, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityStore), "Extended property cache updated - identity: {0} properties: {1}", (object) identity.Descriptor, (object) string.Join(",", (IEnumerable<string>) modifiedProperties));
          dictionary.Add(identity.Id, new Dictionary<string, object>(modifiedProperties.Count));
          foreach (string key in modifiedProperties)
            dictionary[identity.Id].Add(key, identity.Properties[key]);
        }
      }
      if (dictionary.IsNullOrEmpty<KeyValuePair<Guid, Dictionary<string, object>>>())
        return false;
      int num = this.m_cache.UpdateIdentityProperties(requestContext, (IDictionary<Guid, Dictionary<string, object>>) dictionary) ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.OnIdentityPropertiesChanged((object) this, new IdentityPropertiesChangeEventArgs()
      {
        RequestContext = requestContext,
        ChangedIdentityProperties = dictionary
      });
      return num != 0;
    }

    private void CacheIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool extendedPropertiesCacheEnabled)
    {
      if (!extendedPropertiesCacheEnabled && propertyNameFilters != null)
        return;
      if (extendedPropertiesCacheEnabled)
      {
        this.m_cache.UpdateIdentity(requestContext, hostDomain, queryMembership, identity);
        this.m_cache.UpdateIdentityProperties(requestContext, hostDomain, queryMembership, propertyNameFilters, identity);
      }
      else
        this.m_cache.UpdateIdentity(requestContext, hostDomain, queryMembership, identity);
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromImsCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      IdentitySearchFilter searchFilter,
      string factorValue)
    {
      try
      {
        if (queryMembership != QueryMembership.None || !requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || string.IsNullOrWhiteSpace(factorValue) || searchFilter != IdentitySearchFilter.DisplayName && searchFilter != IdentitySearchFilter.AccountName)
          return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
        IImsCacheService service = requestContext.GetService<IImsCacheService>();
        if (searchFilter == IdentitySearchFilter.AccountName)
          return service.GetIdentitiesByAccountName(requestContext, hostDomain.DomainId, factorValue);
        if (searchFilter == IdentitySearchFilter.DisplayName)
          return service.GetIdentitiesByDisplayName(requestContext, hostDomain.DomainId, factorValue);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(8000138, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
    }

    private void UpdateIdentitiesInImsCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      IdentitySearchFilter searchFilter,
      string factorValue,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results)
    {
      try
      {
        if (queryMembership != QueryMembership.None || !requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || string.IsNullOrWhiteSpace(factorValue) || searchFilter != IdentitySearchFilter.DisplayName && searchFilter != IdentitySearchFilter.AccountName)
          return;
        IImsCacheService service = requestContext.GetService<IImsCacheService>();
        if (searchFilter != IdentitySearchFilter.AccountName)
        {
          if (searchFilter != IdentitySearchFilter.DisplayName)
            return;
          service.SetIdentitiesByDisplayName(requestContext, hostDomain.DomainId, factorValue, (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) results);
        }
        else
          service.SetIdentitiesByAccountName(requestContext, hostDomain.DomainId, factorValue, (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) results);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(8000128, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
    }

    private void ReadIdentitiesFromImsCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      Microsoft.VisualStudio.Services.Identity.Identity[] results)
    {
      try
      {
        if (queryMembership != QueryMembership.None || !requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        List<IdentityDescriptor> cacheMisses = FrameworkIdentityStore.ComputeCacheMisses<IdentityDescriptor>(descriptors, results);
        if (!cacheMisses.Any<IdentityDescriptor>())
          return;
        Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> identities = requestContext.GetService<IImsCacheService>().GetIdentities(requestContext, hostDomain.DomainId, (ICollection<IdentityDescriptor>) cacheMisses);
        FrameworkIdentityStore.UpdateResults<IdentityDescriptor>(descriptors, results, (IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) identities);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(8000108, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
    }

    private void ReadIdentitiesFromImsCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Guid> ids,
      QueryMembership queryMembership,
      Microsoft.VisualStudio.Services.Identity.Identity[] results)
    {
      try
      {
        if (queryMembership != QueryMembership.None || !requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        List<Guid> cacheMisses = FrameworkIdentityStore.ComputeCacheMisses<Guid>(ids, results);
        if (!cacheMisses.Any<Guid>())
          return;
        Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identities = requestContext.GetService<IImsCacheService>().GetIdentities(requestContext, hostDomain.DomainId, (ICollection<Guid>) cacheMisses);
        FrameworkIdentityStore.UpdateResults<Guid>(ids, results, (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) identities);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(8000148, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
    }

    private void UpdateIdentitiesInImsCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      List<IdentityDescriptor> identityDescriptors,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results)
    {
      try
      {
        if (queryMembership != QueryMembership.None || !requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        IEnumerable<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>> values = identityDescriptors.Zip<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) results, (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>) ((d, i) => new KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(d, i)));
        requestContext.GetService<IImsCacheService>().SetIdentities(requestContext, hostDomain.DomainId, values);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(8000118, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
    }

    private void UpdateIdentitiesInImsCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      List<Guid> ids,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results)
    {
      try
      {
        if (queryMembership != QueryMembership.None || !requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        IEnumerable<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>> values = ids.Zip<Guid, Microsoft.VisualStudio.Services.Identity.Identity, KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) results, (Func<Guid, Microsoft.VisualStudio.Services.Identity.Identity, KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>) ((d, i) => new KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(d, i)));
        requestContext.GetService<IImsCacheService>().SetIdentities(requestContext, hostDomain.DomainId, values);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(8000158, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
    }

    private static List<T> ComputeCacheMisses<T>(IList<T> ids, Microsoft.VisualStudio.Services.Identity.Identity[] results)
    {
      List<T> cacheMisses = new List<T>();
      for (int index = 0; index < results.Length; ++index)
      {
        if (results[index] == null && (object) ids[index] != null)
          cacheMisses.Add(ids[index]);
      }
      return cacheMisses;
    }

    private static void UpdateResults<T>(
      IList<T> ids,
      Microsoft.VisualStudio.Services.Identity.Identity[] results,
      IDictionary<T, Microsoft.VisualStudio.Services.Identity.Identity> cachedIdentityMap)
    {
      if (cachedIdentityMap == null)
        return;
      for (int index = 0; index < ids.Count; ++index)
      {
        if (results[index] == null && cachedIdentityMap.ContainsKey(ids[index]))
        {
          Microsoft.VisualStudio.Services.Identity.Identity cachedIdentity = cachedIdentityMap[ids[index]];
          if (cachedIdentity != null)
            results[index] = cachedIdentity;
        }
      }
    }

    private bool UseExtendedPropertiesCache(
      IVssRequestContext requestContext,
      IEnumerable<string> propertyNameFilters)
    {
      if (!requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.IdentityStore.CacheExtendedProperties"))
        return false;
      return propertyNameFilters == null || propertyNameFilters.All<string>((Func<string, bool>) (propertyNameFilter => propertyNameFilter.Last<char>() != '*'));
    }

    private static int Count<T>(ICollection<T> collection) => collection == null ? 0 : collection.Count;

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> CommandReadIdentitiesInScope(
      IVssRequestContext context,
      IdentityDomain hostDomain,
      Guid scopeId,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      string str = hostDomain.DomainId == scopeId ? this.m_circuitBreakerSettings.CommandKeyForReadIdentitiesInRootScope : this.m_circuitBreakerSettings.CommandKeyForReadIdentitiesInChildScope;
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) this.m_circuitBreakerSettings.CommandGroupKey).AndCommandKey((CommandKey) str).AndCommandPropertiesDefaults(this.m_circuitBreakerSettings.CircuitBreakerSettingsForReadIdentitiesInScope);
      return new CommandService<IList<Microsoft.VisualStudio.Services.Identity.Identity>>(context, setter, (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
      {
        context.Trace(2432360, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityStore), "ReadIndentities without sequence context without deciding");
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetHttpClient(context).ReadIdentitiesAsync(scopeId, queryMembership, propertyNameFilters, (object) null, new CancellationToken()).SyncResult<IdentitiesCollection>();
      }), (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
      {
        IdentityStoreNotAvailableException availableException = new IdentityStoreNotAvailableException(string.Format("ReadIdentitiesInScope failed at ReadIdentitiesAsync for hostDomain {0} and scopeId {1}", (object) hostDomain.DomainId.ToString(), (object) scopeId.ToString()));
        availableException.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
        context.TraceException(8000198, "IdentityService", nameof (FrameworkIdentityStore), (Exception) availableException);
        throw availableException;
      })).Execute();
    }

    private void InitializeSequenceContext(IVssRequestContext context, IdentityDomain hostDomain)
    {
      try
      {
        if (!context.IsFeatureEnabled("VisualStudio.Services.Identity.AadBackedOrgs.InitializeSequenceContext") || !context.IsOrganizationAadBacked())
          return;
        this.GetSequenceContext(context, hostDomain);
      }
      catch (Exception ex)
      {
        context.TraceException(8000200, "IdentityService", nameof (FrameworkIdentityStore), ex);
      }
    }
  }
}
