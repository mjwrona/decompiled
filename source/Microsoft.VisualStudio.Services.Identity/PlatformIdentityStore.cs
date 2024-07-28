// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PlatformIdentityStore
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Cache;
using Microsoft.VisualStudio.Services.Identity.Events;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class PlatformIdentityStore : IdentityStoreBase, IIdentityReader
  {
    private GroupMembershipReadReplicaHelper groupMembershipReadReplicaHelper = new GroupMembershipReadReplicaHelper();
    internal static readonly string EnableUseReadGroupsXtpProcFeatureFlag = "VisualStudio.Services.Identity.EnableUseReadGroupsXtpProc";
    internal static readonly string EnableUseReadGroupMembershipXtpProcFeatureFlag = "VisualStudio.Services.Identity.EnableUseReadGroupMembershipXtpProc";
    internal static readonly string DisableExhaustiveDeploymentLevelIsMemberTracing = "VisualStudio.Services.Identity.UserSharding.DisableExhaustiveDeploymentLevelIsMemberTracing";
    internal IIdentityEventHandler<PlatformIdentityStore.IdentityChangeEventArgs> IdentitiesChanged;
    private readonly PlatformIdentityStore m_parentIdentityStore;
    private readonly IPlatformIdentityCache m_cache;
    internal PlatformIdentityStore.PlatformIdentityChangeProcessor m_changeProcessor;
    private readonly int? m_inactiveMemberLifespan;
    private bool m_sendBisNotifications;
    private readonly IdentityDomain m_masterDomain;
    private readonly IDictionary<string, IIdentityProvider> m_syncAgents;
    private int m_percentageCallsToRouteToReadReplica = 75;
    private const int c_defaultPercentageCallsToRouteToReadReplica = 75;
    private int m_readGroupsChunkSize;
    private const int c_defaultReadGroupsChunkSize = 1000;
    private readonly IDisposableReadOnlyList<IReadIdentitiesFromDatabaseExtension> m_readIdentitiesFromDatabaseExtensions;
    private readonly IDisposableReadOnlyList<IUpdateIdentitiesInDatabaseExtension> m_updateIdentitiesInDatabaseExtensions;
    private const int FallbackIdentityReadEnterExitTracepoint = 6060910;
    private const int FallbackIdentityReadErrorInfoTracepoint = 6060911;
    private const int FallbackIdentityReadErrorExceptionTracepoint = 6060912;
    private const int FallbackIdentityReadUnexpectedExceptionTracepoint = 6060913;
    private const int FallbackIdentityFilterReadUnexpectedExceptionTracepoint = 6060914;
    private const int FallbackIdentityFilterReadStackTraceTracepoint = 6060915;
    private const string s_area = "IdentityService";
    private const string s_layer = "IdentityStore";
    private const string s_AccountLinkingArea = "AccountLinking";
    private const string s_AccountLinkingLayer = "IdentityStore";
    private const string c_featureNameCacheExtendedProperties = "VisualStudio.IdentityStore.CacheExtendedProperties";
    private const string c_featureNameExpandedDownFromCache = "VisualStudio.Services.Identity.ExpandedDownFromCache";
    internal const string c_featureNameEnableScopeCache = "VisualStudio.Services.Identity.EnableScopeCache";
    private const string c_featureNameUseRGMV2ForReadIdentitiesByScope = "VisualStudio.Services.Identity.UseRGMV2ForReadIdentitiesByScope";
    private const string TraceCacheCountsFeatureEnabled = "VisualStudio.IdentityStore.TraceCacheCounts";
    private const string c_enableReadGroupMembershipComponentFeatureFlag = "VisualStudio.Services.Identity.EnableReadGroupMembershipsComponent";
    internal static readonly string EnableAutoMigrateNewIdentities = "VisualStudio.Services.Identity.AutoMigrateNewIdentities.Enabled";
    internal static readonly string ResolveByOid = "VisualStudio.Services.Identity.ResolveByOid";
    internal static readonly string EnableTransferAcesDuringIdentityTranslation = "VisualStudio.Services.Identity.IdentityIdTranslator.EnableTransferAces";
    private const int c_ReadIdentitiesByScopeBatchSize = 1000;
    internal static readonly string s_featureNameUseProviderDisplayName = "VisualStudio.Profile.UseProviderDisplayName";
    internal const bool s_defaultUseProviderDisplayName = false;
    internal const bool c_defaultDirectoryAliasFeature = false;
    internal const bool c_defaultSocialDescriptorFeature = true;
    private static readonly TimeSpan s_traceCacheStatsInterval = TimeSpan.FromMinutes(5.0);
    internal const string c_FeaturePartitionedGroupProperties = "VisualStudio.Services.Identity.UsePartitionedGroupProperties";
    private const string ClaimsMSASidPattern = "0_______________@Live.com";
    private static readonly string[] m_identityAttributes = new string[6]
    {
      "Description",
      nameof (Domain),
      "Account",
      "DN",
      "Mail",
      "PUID"
    };
    private static readonly VssPerformanceCounter s_imsCacheHitsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_cache_hits");
    private static readonly VssPerformanceCounter s_imsCacheMissesCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_cache_misses");
    private static readonly VssPerformanceCounter s_imsCacheMissesPerSecCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_cache_misses_persec");
    private static readonly VssPerformanceCounter s_imsMembershipInvalidationsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_membership_invalidations");
    private static readonly VssPerformanceCounter s_imsMembershipInvalidationsPerSecCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_membership_invalidations_persec");
    private static readonly VssPerformanceCounter s_imsInvalidationsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_invalidations");
    private static readonly VssPerformanceCounter s_imsInvalidationsPerSecCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_invalidations_persec");

    public void Install(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid securingHostId,
      string rootGroupSid,
      string rootGroupName,
      string rootGroupDescription,
      string adminGroupSid,
      string adminGroupName,
      string adminGroupDescription,
      bool idempotent)
    {
      Guid parentScopeId;
      string scopeName;
      if (hostDomain.IsMaster)
      {
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
          identityComponent.Install();
        parentScopeId = Guid.Empty;
        scopeName = "TEAM FOUNDATION";
      }
      else
      {
        parentScopeId = this.m_masterDomain.DomainId;
        scopeName = requestContext.ServiceHost.Name;
      }
      try
      {
        long scope;
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
          scope = groupComponent.CreateScope(parentScopeId, hostDomain.DomainId, hostDomain.DomainId, scopeName, securingHostId, GroupScopeType.ServiceHost, rootGroupSid, rootGroupName, rootGroupDescription, adminGroupSid, adminGroupName, adminGroupDescription);
        this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) scope));
      }
      catch (GroupScopeCreationException ex)
      {
        if (idempotent)
          return;
        throw;
      }
    }

    public void Uninstall(IVssRequestContext requestContext, IdentityDomain hostDomain)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> items;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        items = groupComponent.QueryGroups((IEnumerable<Guid>) new Guid[1]
        {
          hostDomain.DomainId
        }, true, false).GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UsePartitionedGroupProperties"))
        this.PropertyHelper.ClearExtendedProperties(requestContext, IdentityPropertyScope.Both, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) items);
      else
        this.PropertyHelper.ClearExtendedProperties(requestContext, IdentityPropertyScope.Global, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) items);
      long groupSequenceId;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext, 3600))
        groupSequenceId = groupComponent.UpdateScopes(true, (IEnumerable<Guid>) new Guid[1]
        {
          hostDomain.DomainId
        }, (IEnumerable<KeyValuePair<Guid, string>>) null);
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId));
      this.m_cache.Clear(requestContext, hostDomain);
    }

    public virtual void CreateScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid parentScopeId,
      GroupScopeType scopeType,
      Guid scopeId,
      Guid localScopeId,
      string scopeName,
      Guid securingHostId,
      string rootGroupSid,
      string rootGroupName,
      string rootGroupDescription,
      string adminGroupSid,
      string adminGroupName,
      string adminGroupDescription)
    {
      long scope;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        scope = groupComponent.CreateScope(parentScopeId, scopeId, localScopeId, scopeName, securingHostId, scopeType, rootGroupSid, rootGroupName, rootGroupDescription, adminGroupSid, adminGroupName, adminGroupDescription);
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) scope));
    }

    public override IdentityScope GetScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId)
    {
      IdentityScope scope;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableScopeCache") && this.m_cache.TryGetScope(requestContext, hostDomain, scopeId, out scope) && scope != null)
        return scope;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
      {
        using (ResultCollection resultCollection = groupComponent.QueryScopes(scopeId, (string) null))
        {
          IdentityScope identityScope = resultCollection.GetCurrent<IdentityScope>().Items.FirstOrDefault<IdentityScope>();
          if (identityScope == null || requestContext.ExecutionEnvironment.IsHostedDeployment && identityScope.SecuringHostId != this.GetSecurityContext(requestContext).ServiceHost.InstanceId)
            throw new GroupScopeDoesNotExistException(scopeId);
          scope = identityScope;
        }
      }
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableScopeCache"))
        this.m_cache.AddScope(requestContext, hostDomain, scopeId, scope);
      return scope;
    }

    public virtual void RenameScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      string newName)
    {
      long groupSequenceId;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groupSequenceId = groupComponent.UpdateScopes(false, (IEnumerable<Guid>) null, (IEnumerable<KeyValuePair<Guid, string>>) new KeyValuePair<Guid, string>[1]
        {
          new KeyValuePair<Guid, string>(scopeId, newName)
        });
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId), groupScopeChangeIds: (ICollection<Guid>) new List<Guid>()
      {
        scopeId
      });
    }

    public void DeleteScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId)
    {
      this.DeleteScope(requestContext, hostDomain, scopeId, false);
    }

    public void RestoreScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId)
    {
      requestContext.TraceConditionally(2432363, TraceLevel.Info, "IdentityService", "IdentityStore", (Func<string>) (() => string.Format("Restore scopeId: {0} on hostDomainId: {1} - platform before component call", (object) scopeId, (object) hostDomain.DomainId)));
      long sequenceId;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
      {
        RestoreProjectOptions restoreOptions = requestContext.ExecutionEnvironment.IsHostedDeployment ? RestoreProjectOptions.Visible : RestoreProjectOptions.All;
        sequenceId = groupComponent.RestoreGroupScope(scopeId, restoreOptions);
      }
      requestContext.TraceConditionally(2432364, TraceLevel.Info, "IdentityService", "IdentityStore", (Func<string>) (() => string.Format("Restore scopeId: {0} on hostDomainId: {1} - platform after component call, resulted in sequenceId of: {2}", (object) scopeId, (object) hostDomain.DomainId, (object) sequenceId)));
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) sequenceId), groupScopeChangeIds: (ICollection<Guid>) new List<Guid>()
      {
        scopeId
      });
    }

    public void DeleteScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      bool hardDelete)
    {
      long groupSequenceId;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groupSequenceId = groupComponent.UpdateScopes((hardDelete ? 1 : 0) != 0, (IEnumerable<Guid>) new Guid[1]
        {
          scopeId
        }, (IEnumerable<KeyValuePair<Guid, string>>) null);
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId), groupScopeChangeIds: (ICollection<Guid>) new List<Guid>()
      {
        scopeId
      });
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadAadGroupsFromDatabase(
      IVssRequestContext requestContext,
      bool readInactiveGroups = false)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
      {
        using (ResultCollection resultCollection = groupComponent.ReadAadGroups(readInactiveGroups))
        {
          foreach (GroupComponent.GroupIdentityData groupIdentityData in resultCollection.GetCurrent<GroupComponent.GroupIdentityData>())
            identityList.Add(groupIdentityData.Identity);
        }
      }
      return identityList;
    }

    public virtual IdentityDescriptor[] CreateGroups(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      bool errorOnDuplicate,
      bool addActiveScopeMembership = true,
      params GroupDescription[] groupDescriptions)
    {
      IdentityDescriptor[] groups1 = new IdentityDescriptor[groupDescriptions.Length];
      for (int index = 0; index < groupDescriptions.Length; ++index)
      {
        GroupDescription groupDescription = groupDescriptions[index];
        if (groupDescription.Descriptor == (IdentityDescriptor) null)
        {
          SecurityIdentifier securityId = SidIdentityHelper.NewSid(scopeId);
          groupDescription.Descriptor = IdentityHelper.CreateTeamFoundationDescriptor(securityId);
        }
        groups1[index] = groupDescription.Descriptor;
        if (groupDescription.Id == Guid.Empty)
          groupDescription.Id = Guid.NewGuid();
      }
      long groups2;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groups2 = groupComponent.CreateGroups(scopeId, errorOnDuplicate, groupDescriptions, addActiveScopeMembership);
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groups2));
      return groups1;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity GetApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity applicationGroup = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
      {
        using (ResultCollection resultCollection = groupComponent.ReadGroupBySid(hostDomain.DomainId, groupDescriptor.Identifier))
        {
          foreach (GroupComponent.GroupIdentityData groupIdentityData in resultCollection.GetCurrent<GroupComponent.GroupIdentityData>())
            applicationGroup = groupIdentityData.Identity;
        }
      }
      if (applicationGroup == null)
        throw new FindGroupSidDoesNotExistException(groupDescriptor.Identifier);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IIdMapper idMapper = this.m_cache.GetIdMapper(requestContext, hostDomain);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        IVssRequestContext requestContext1 = requestContext;
        AfterGetApplicationGroupOnStoreEvent groupOnStoreEvent = new AfterGetApplicationGroupOnStoreEvent();
        groupOnStoreEvent.Identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          applicationGroup
        };
        groupOnStoreEvent.IdentityMapper = idMapper;
        groupOnStoreEvent.IdentityDomain = hostDomain;
        AfterGetApplicationGroupOnStoreEvent notificationEvent = groupOnStoreEvent;
        service.PublishDecisionPoint(requestContext1, (object) notificationEvent);
      }
      return applicationGroup;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid[] scopeIds,
      bool recurse,
      bool deleted,
      IEnumerable<string> propertyNameFilters)
    {
      if (scopeIds == null)
        scopeIds = new Guid[1]{ this.Domain.DomainId };
      for (int index = 0; index < scopeIds.Length; ++index)
        scopeIds[index] = this.MapScopeId(requestContext, this.Domain, scopeIds[index]);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.ListApplicationGroups(requestContext, this.Domain, scopeIds, recurse, deleted, propertyNameFilters != null, propertyNameFilters);
    }

    public List<Microsoft.VisualStudio.Services.Identity.Identity> ListApplicationGroups(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid[] scopeIds,
      bool recurse,
      bool deleted,
      bool extendedProperties,
      IEnumerable<string> propertyNameFilters)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> identities = (List<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
      {
        using (ResultCollection resultCollection = groupComponent.QueryGroups((IEnumerable<Guid>) scopeIds, recurse, deleted))
          identities = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items;
      }
      IIdMapper idMapper = this.m_cache.GetIdMapper(requestContext, hostDomain);
      if (extendedProperties)
        this.ReadExtendedProperties(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities, propertyNameFilters, idMapper);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        IVssRequestContext requestContext1 = requestContext;
        AfterListApplicationGroupsOnStoreEvent notificationEvent = new AfterListApplicationGroupsOnStoreEvent();
        notificationEvent.Identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities;
        notificationEvent.IdentityMapper = idMapper;
        notificationEvent.IdentityDomain = hostDomain;
        service.PublishDecisionPoint(requestContext1, (object) notificationEvent);
      }
      return identities;
    }

    public void UpdateApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor,
      GroupProperty groupProperty,
      string newValue)
    {
      long groupSequenceId;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groupSequenceId = groupComponent.UpdateGroups(hostDomain.DomainId, (IEnumerable<string>) null, (IEnumerable<GroupComponent.GroupUpdate>) new GroupComponent.GroupUpdate[1]
        {
          new GroupComponent.GroupUpdate()
          {
            GroupSid = groupDescriptor.Identifier,
            Name = groupProperty == GroupProperty.Name ? newValue : (string) null,
            Description = groupProperty == GroupProperty.Description ? newValue : (string) null
          }
        });
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId));
    }

    public void DeleteApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Microsoft.VisualStudio.Services.Identity.Identity group)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UsePartitionedGroupProperties"))
        this.PropertyHelper.ClearExtendedProperties(requestContext, IdentityPropertyScope.Both, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          group
        });
      else
        this.PropertyHelper.ClearExtendedProperties(requestContext, IdentityPropertyScope.Global, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          group
        });
      long groupSequenceId;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groupSequenceId = groupComponent.UpdateGroups(hostDomain.DomainId, (IEnumerable<string>) new string[1]
        {
          group.Descriptor.Identifier
        }, (IEnumerable<GroupComponent.GroupUpdate>) null);
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId));
    }

    private bool UpdateGroupsInDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups)
    {
      List<GroupComponent.GroupUpdate> groupUpdateList = new List<GroupComponent.GroupUpdate>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in new List<Microsoft.VisualStudio.Services.Identity.Identity>(groups))
      {
        if (IdentityValidation.IsTeamFoundationType(identity.Descriptor))
        {
          string property1 = identity.GetProperty<string>("Account", string.Empty);
          string property2 = identity.GetProperty<string>("Description", string.Empty);
          groupUpdateList.Add(new GroupComponent.GroupUpdate()
          {
            GroupSid = identity.Descriptor.Identifier,
            Name = property1.Trim(),
            Description = property2
          });
        }
      }
      long groupSequenceId = -1;
      if (groupUpdateList.Count > 0)
      {
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
          groupSequenceId = groupComponent.UpdateGroups(hostDomain.DomainId, (IEnumerable<string>) null, (IEnumerable<GroupComponent.GroupUpdate>) groupUpdateList);
        this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId));
      }
      return groupSequenceId != -1L;
    }

    internal virtual List<IdentityDescriptor> GetAllIdentityGroups(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain)
    {
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
      {
        using (ResultCollection groupsToSync = groupComponent.GetGroupsToSync(new DateTime?()))
          return groupsToSync.GetCurrent<IdentityDescriptor>().Items;
      }
    }

    internal virtual List<IdentityDescriptor> GetGroupsToSync(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      DateTime lastSync,
      out int totalIdentities)
    {
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
      {
        using (ResultCollection groupsToSync = groupComponent.GetGroupsToSync(new DateTime?(lastSync)))
        {
          totalIdentities = groupsToSync.GetCurrent<int>().Items[0];
          groupsToSync.NextResult();
          return groupsToSync.GetCurrent<IdentityDescriptor>().Items;
        }
      }
    }

    public IdentitySnapshot ReadIdentitySnapshotFromDatabase(
      IVssRequestContext requestContext,
      Guid scopeIdGuid,
      HashSet<Guid> excludedIdentities = null,
      bool readAllIdentities = false,
      bool readInactiveMemberships = false)
    {
      IdentitySnapshot identitySnapshot = new IdentitySnapshot()
      {
        ScopeId = scopeIdGuid
      };
      if (excludedIdentities == null)
        excludedIdentities = new HashSet<Guid>();
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
      {
        using (ResultCollection resultCollection = groupComponent.ReadSnapshot(scopeIdGuid, readAllIdentities, readInactiveMemberships))
        {
          identitySnapshot.Scopes = (List<IdentityScope>) new IdentityScopeCollection((IList<IdentityScope>) resultCollection.GetCurrent<IdentityScope>().Items);
          if (identitySnapshot.Scopes.Count == 0)
            throw new GroupScopeDoesNotExistException(scopeIdGuid);
          resultCollection.NextResult();
          List<Microsoft.VisualStudio.Services.Identity.Identity> list1 = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => !excludedIdentities.Contains(i.Id))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
          identitySnapshot.Groups = (List<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentitiesCollection((IList<Microsoft.VisualStudio.Services.Identity.Identity>) list1);
          resultCollection.NextResult();
          List<GroupMembership> list2 = resultCollection.GetCurrent<GroupMembership>().Items.Where<GroupMembership>((Func<GroupMembership, bool>) (gm => !excludedIdentities.Contains(gm.Id) && !excludedIdentities.Contains(gm.QueriedId))).ToList<GroupMembership>();
          identitySnapshot.Memberships = (List<GroupMembership>) new GroupMembershipCollection((IList<GroupMembership>) list2);
          resultCollection.NextResult();
          List<Guid> list3 = resultCollection.GetCurrent<Guid>().Items.Where<Guid>((Func<Guid, bool>) (id => !excludedIdentities.Contains(id))).ToList<Guid>();
          identitySnapshot.IdentityIds = list3;
        }
      }
      return identitySnapshot;
    }

    public virtual bool AddMembersToApplicationGroups(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      bool errorOnDuplicate,
      Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> addition)
    {
      return this.AddMembersToApplicationGroups(requestContext, hostDomain, (errorOnDuplicate ? 1 : 0) != 0, (IEnumerable<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>) new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>[1]
      {
        addition
      });
    }

    public bool AddMembersToApplicationGroups(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      bool errorOnDuplicate,
      IEnumerable<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>> additions)
    {
      bool applicationGroups = false;
      if (additions.Any<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>())
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> list1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) additions.Where<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (addition => !IdentityValidation.IsTeamFoundationType(addition.Item2.Descriptor))).Select<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (addition => addition.Item2)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (list1.Count > 0)
        {
          Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = this.ReadIdentities(requestContext, hostDomain, (IList<IdentityDescriptor>) list1.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (x => x.Descriptor)).ToList<IdentityDescriptor>(), QueryMembership.None, false, (IEnumerable<string>) null);
          for (int index = list1.Count - 1; index >= 0; --index)
          {
            if (identityArray[index] != null && identityArray[index].UniqueUserId == 0)
            {
              list1[index].Id = identityArray[index].Id;
              list1[index].MasterId = identityArray[index].MasterId;
              list1.RemoveAt(index);
            }
          }
          this.UpdateIdentitiesInDatabase(requestContext, hostDomain, list1, false);
        }
        IEnumerable<Tuple<IdentityDescriptor, Guid, bool>> updates = additions.Select<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Tuple<IdentityDescriptor, Guid, bool>>((Func<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Tuple<IdentityDescriptor, Guid, bool>>) (a => new Tuple<IdentityDescriptor, Guid, bool>(a.Item1, a.Item2.MasterId, true)));
        long groupSequenceId;
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
          groupSequenceId = groupComponent.UpdateGroupMembership(hostDomain.DomainId, !errorOnDuplicate, true, false, updates);
        applicationGroups = groupSequenceId > 0L;
        this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId));
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          IIdMapper idMapper = this.m_cache.GetIdMapper(requestContext, hostDomain);
          ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
          List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = additions.Select<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (a => a.Item2)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
          IVssRequestContext requestContext1 = requestContext;
          AfterAddMembersToApplicationGroupsOnStoreEvent notificationEvent = new AfterAddMembersToApplicationGroupsOnStoreEvent();
          notificationEvent.Identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list2;
          notificationEvent.IdentityMapper = idMapper;
          notificationEvent.IdentityDomain = hostDomain;
          service.PublishDecisionPoint(requestContext1, (object) notificationEvent);
        }
      }
      return applicationGroups;
    }

    public bool RemoveMemberFromApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      bool errorOnNotMember,
      params Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>[] removals)
    {
      bool flag = false;
      if (removals.Length != 0)
      {
        IEnumerable<Tuple<IdentityDescriptor, Guid, bool>> updates = ((IEnumerable<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>) removals).Select<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Tuple<IdentityDescriptor, Guid, bool>>((Func<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Tuple<IdentityDescriptor, Guid, bool>>) (a => new Tuple<IdentityDescriptor, Guid, bool>(a.Item1, a.Item2.MasterId == Guid.Empty ? a.Item2.Id : a.Item2.MasterId, false)));
        long groupSequenceId;
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
          groupSequenceId = groupComponent.UpdateGroupMembership(hostDomain.DomainId, !errorOnNotMember, true, false, updates);
        flag = groupSequenceId > 0L;
        this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId));
      }
      return flag;
    }

    public bool RemoveMemberFromApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      bool errorOnNotMember,
      params Tuple<IdentityDescriptor, IdentityDescriptor>[] removals)
    {
      Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>[] tupleArray = new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>[removals.Length];
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = this.ReadIdentities(requestContext, this.m_masterDomain, (IList<IdentityDescriptor>) ((IEnumerable<Tuple<IdentityDescriptor, IdentityDescriptor>>) removals).Select<Tuple<IdentityDescriptor, IdentityDescriptor>, IdentityDescriptor>((Func<Tuple<IdentityDescriptor, IdentityDescriptor>, IdentityDescriptor>) (update => update.Item2)).ToList<IdentityDescriptor>(), QueryMembership.None, false, (IEnumerable<string>) null);
      for (int index = 0; index < identityArray.Length; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityArray[index];
        if (identity == null)
          return false;
        tupleArray[index] = new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(removals[index].Item1, identity);
      }
      return this.RemoveMemberFromApplicationGroup(requestContext, hostDomain, errorOnNotMember, tupleArray);
    }

    public void UpdateGroupMembership(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      bool incremental,
      IList<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>> updates)
    {
      long groupSequenceId;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groupSequenceId = groupComponent.UpdateGroupMembership(hostDomain.DomainId, true, incremental, false, updates.Select<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>, Tuple<IdentityDescriptor, Guid, bool>>((Func<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>, Tuple<IdentityDescriptor, Guid, bool>>) (update => new Tuple<IdentityDescriptor, Guid, bool>(update.Item1, update.Item2.MasterId == Guid.Empty ? update.Item2.Id : update.Item2.MasterId, update.Item3))));
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId));
    }

    public void UpdateGroupMembership(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      bool incremental,
      IList<Tuple<IdentityDescriptor, IdentityDescriptor, bool>> updates)
    {
      List<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>> updates1 = new List<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>(updates.Count);
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = this.ReadIdentitiesFromDatabase(requestContext, hostDomain, (IList<IdentityDescriptor>) updates.Select<Tuple<IdentityDescriptor, IdentityDescriptor, bool>, IdentityDescriptor>((Func<Tuple<IdentityDescriptor, IdentityDescriptor, bool>, IdentityDescriptor>) (update => update.Item2)).ToList<IdentityDescriptor>(), (IList<Guid>) null, QueryMembership.None, QueryMembership.None, false, false, false, (SequenceContext) null);
      for (int index = 0; index < identityArray.Length; ++index)
        updates1.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>(updates[index].Item1, identityArray[index] ?? throw new IdentityNotFoundException(updates[index].Item2), updates[index].Item3));
      this.UpdateGroupMembership(requestContext, hostDomain, incremental, (IList<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>) updates1);
    }

    internal void DeleteGroupMemberships(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool removeParents,
      bool removeChildren)
    {
      long groupSequenceId;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groupSequenceId = groupComponent.DeleteGroupMemberships(identities.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => !(identity.MasterId == Guid.Empty) ? identity.MasterId : identity.Id)), removeParents, removeChildren);
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId));
    }

    internal void DeleteGroupMemberships(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      bool removeParents,
      bool removeChildren)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadIdentitiesFromDatabase(requestContext, hostDomain, descriptors, (IList<Guid>) null, QueryMembership.None, QueryMembership.None, false, false, false, (SequenceContext) null)).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null));
      this.DeleteGroupMemberships(requestContext, hostDomain, identities, removeParents, removeChildren);
    }

    public void UpdateIdentityMembership(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      bool incremental,
      IList<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>> updates)
    {
      long groupSequenceId;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groupSequenceId = groupComponent.UpdateIdentityMembership(incremental, updates.Select<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>, Tuple<IdentityDescriptor, Guid, bool>>((Func<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>, Tuple<IdentityDescriptor, Guid, bool>>) (update => new Tuple<IdentityDescriptor, Guid, bool>(update.Item1, update.Item2.MasterId == Guid.Empty ? update.Item2.Id : update.Item2.MasterId, update.Item3))));
      this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, -1, checked ((int) groupSequenceId));
    }

    public void UpdateIdentityMembership(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      bool incremental,
      IList<Tuple<IdentityDescriptor, IdentityDescriptor, bool>> updates)
    {
      List<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>> updates1 = new List<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>(updates.Count);
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = this.ReadIdentitiesFromDatabase(requestContext, hostDomain, (IList<IdentityDescriptor>) updates.Select<Tuple<IdentityDescriptor, IdentityDescriptor, bool>, IdentityDescriptor>((Func<Tuple<IdentityDescriptor, IdentityDescriptor, bool>, IdentityDescriptor>) (update => update.Item2)).ToList<IdentityDescriptor>(), (IList<Guid>) null, QueryMembership.None, QueryMembership.None, false, false, false, (SequenceContext) null);
      for (int index = 0; index < identityArray.Length; ++index)
        updates1.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>(updates[index].Item1, identityArray[index] ?? throw new IdentityNotFoundException(updates[index].Item2), updates[index].Item3));
      this.UpdateIdentityMembership(requestContext, hostDomain, incremental, (IList<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>) updates1);
    }

    private void UpdateMembership(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      List<Tuple<IdentityDescriptor, Guid, bool>> updates,
      bool idempotent,
      bool incremental,
      bool insertInactiveUpdates)
    {
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groupComponent.UpdateGroupMembership(hostDomain.DomainId, idempotent, incremental, insertInactiveUpdates, (IEnumerable<Tuple<IdentityDescriptor, Guid, bool>>) updates);
    }

    internal void TransferMembership(
      IVssRequestContext requestContext,
      KeyValuePair<Guid, Guid> identityToTransfer)
    {
      this.TransferMembership(requestContext, (IEnumerable<KeyValuePair<Guid, Guid>>) new KeyValuePair<Guid, Guid>[1]
      {
        identityToTransfer
      }, DescriptorChangeType.None);
    }

    internal void TransferMembership(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> identitiesToTransfer,
      DescriptorChangeType descriptorChangeType)
    {
      long groupSequenceId = -1;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groupSequenceId = groupComponent.TransferGroupMembership(identitiesToTransfer);
      if (groupSequenceId == -1L)
        return;
      this.ProcessIdentityChangeOnAuthor(requestContext, this.Domain, -1, checked ((int) groupSequenceId), descriptorChangeType);
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity[] ReadGroupsFromDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      IList<Guid> groupIds,
      QueryMembership queryMembership)
    {
      bool includeRestrictedMembers = false;
      if (descriptors != null)
      {
        foreach (IdentityDescriptor descriptor in (IEnumerable<IdentityDescriptor>) descriptors)
        {
          if (IdentityDescriptorComparer.Instance.Equals(descriptor, hostDomain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.SecurityServiceGroup)))
            includeRestrictedMembers = true;
        }
      }
      return this.ReadGroupsFromDatabase(requestContext, hostDomain, descriptors, groupIds, queryMembership, queryMembership, includeRestrictedMembers, false);
    }

    internal virtual Microsoft.VisualStudio.Services.Identity.Identity[] ReadGroupsFromDatabase(
      IVssRequestContext context,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      IList<Guid> groupIds,
      QueryMembership parentQuery,
      QueryMembership childQuery,
      bool includeRestrictedMembers,
      bool includeInactivatedMembers,
      bool filterResults = true,
      SequenceContext minSequenceContext = null)
    {
      List<string> list = descriptors != null ? descriptors.Select<IdentityDescriptor, string>((Func<IdentityDescriptor, string>) (descriptor => !(descriptor != (IdentityDescriptor) null) || !IdentityValidation.IsTeamFoundationType(descriptor) ? (string) null : descriptor.Identifier)).ToList<string>() : (List<string>) null;
      // ISSUE: explicit non-virtual call
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>(list != null ? __nonvirtual (list.Count) : groupIds.Count);
      if (!list.IsNullOrEmpty<string>())
      {
        context.Trace(2432361, TraceLevel.Info, "IdentityService", "IdentityStore", string.Format("Reading {0} group sids in a batch of {1}.", (object) list.Count, (object) this.m_readGroupsChunkSize));
        foreach (IList<string> groupSids in list.Batch<string>(this.m_readGroupsChunkSize))
          identityList.AddRange<Microsoft.VisualStudio.Services.Identity.Identity, IList<Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadGroupsFromDatabaseInternal(context, hostDomain, groupSids, (IList<Guid>) null, parentQuery, childQuery, includeRestrictedMembers, includeInactivatedMembers, filterResults, minSequenceContext));
      }
      else if (!groupIds.IsNullOrEmpty<Guid>())
      {
        context.Trace(2432362, TraceLevel.Info, "IdentityService", "IdentityStore", string.Format("Reading {0} group Ids in a batch of {1}.", (object) groupIds.Count, (object) this.m_readGroupsChunkSize));
        foreach (IList<Guid> groupIds1 in groupIds.Batch<Guid>(this.m_readGroupsChunkSize))
          identityList.AddRange<Microsoft.VisualStudio.Services.Identity.Identity, IList<Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadGroupsFromDatabaseInternal(context, hostDomain, (IList<string>) null, groupIds1, parentQuery, childQuery, includeRestrictedMembers, includeInactivatedMembers, filterResults, minSequenceContext));
      }
      return identityList.ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private Microsoft.VisualStudio.Services.Identity.Identity[] ReadGroupsFromDatabaseInternal(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<string> groupSids,
      IList<Guid> groupIds,
      QueryMembership parentQuery,
      QueryMembership childQuery,
      bool includeRestrictedMembers,
      bool includeInactivatedMembers,
      bool filterResults = true,
      SequenceContext minSequenceContext = null)
    {
      IList<string> stringList1 = groupSids;
      int capacity = stringList1 != null ? stringList1.Count : groupIds.Count;
      Microsoft.VisualStudio.Services.Identity.Identity[] results = new Microsoft.VisualStudio.Services.Identity.Identity[capacity];
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityTable = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(capacity);
      if (groupSids != null && groupSids.Any<string>((Func<string, bool>) (s => s != null)) || groupIds != null && groupIds.Any<Guid>())
      {
        bool useXtp = requestContext.IsFeatureEnabled(PlatformIdentityStore.EnableUseReadGroupsXtpProcFeatureFlag);
        using (GroupComponent db = PlatformIdentityStore.CreateGroupComponent(requestContext))
        {
          IList<string> stringList2 = groupSids;
          Func<ResultCollection> func;
          if ((stringList2 != null ? (stringList2.Count == 1 ? 1 : 0) : 0) != 0 && groupIds.IsNullOrEmpty<Guid>())
          {
            func = (Func<ResultCollection>) (() => db.ReadGroupBySid(hostDomain.DomainId, groupSids.First<string>()));
          }
          else
          {
            IList<Guid> guidList = groupIds;
            func = (guidList != null ? (guidList.Count == 1 ? 1 : 0) : 0) == 0 || !groupSids.IsNullOrEmpty<string>() ? (Func<ResultCollection>) (() => db.ReadGroups(hostDomain.DomainId, (IEnumerable<string>) groupSids, (IEnumerable<Guid>) groupIds, useXtp)) : (Func<ResultCollection>) (() => db.ReadGroupById(hostDomain.DomainId, groupIds.First<Guid>()));
          }
          using (ResultCollection resultCollection = func())
          {
            foreach (GroupComponent.GroupIdentityData groupIdentityData in resultCollection.GetCurrent<GroupComponent.GroupIdentityData>())
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity;
              if (identityTable.TryGetValue(groupIdentityData.Identity.Id, out identity))
              {
                results[groupIdentityData.OrderId] = identity;
              }
              else
              {
                identityTable.Add(groupIdentityData.Identity.Id, groupIdentityData.Identity);
                results[groupIdentityData.OrderId] = groupIdentityData.Identity;
              }
            }
          }
        }
      }
      this.PostProcessBeforeFilterExtensionsAfterReadIdentitiesFromDatabase(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results);
      this.ResolveQueryMembership(childQuery, parentQuery, out childQuery, out parentQuery);
      this.PopulateMembershipAndFilter(requestContext, hostDomain, childQuery, parentQuery, includeRestrictedMembers, includeInactivatedMembers, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results, identityTable, filterResults, minSequenceContext);
      this.PostProcessExtensionsAfterReadIdentitiesFromDatabase(requestContext, parentQuery, childQuery, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results);
      return results;
    }

    internal List<Microsoft.VisualStudio.Services.Identity.Identity> ReadGroupsFromDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      string scopeName,
      string groupName,
      QueryMembership queryMembership,
      bool recurse,
      bool filterResults = true)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> results = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityTable = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
      {
        using (ResultCollection resultCollection = groupComponent.ReadGroup(scopeId, scopeName, groupName, recurse))
        {
          foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>())
          {
            identityTable.Add(identity.Id, identity);
            results.Add(identity);
          }
        }
      }
      this.PostProcessBeforeFilterExtensionsAfterReadIdentitiesFromDatabase(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results);
      QueryMembership childrenQuery = QueryMembership.None;
      QueryMembership parentsQuery = QueryMembership.None;
      this.ResolveQueryMembership(queryMembership, out childrenQuery, out parentsQuery);
      this.PopulateMembershipAndFilter(requestContext, hostDomain, childrenQuery, parentsQuery, false, false, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results, identityTable, filterResults, (SequenceContext) null);
      this.PostProcessExtensionsAfterReadIdentitiesFromDatabase(requestContext, parentsQuery, childrenQuery, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results);
      return results;
    }

    public virtual bool IsMember(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor,
      bool forceCacheReload)
    {
      if (!descriptor.IsTeamFoundationType() && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !requestContext.RootContext.IsDeploymentFallbackIdentityReadAllowed())
      {
        TraceLevel level = requestContext.IsFeatureEnabled(PlatformIdentityStore.DisableExhaustiveDeploymentLevelIsMemberTracing) ? TraceLevel.Verbose : TraceLevel.Error;
        if (!descriptor.IsCuidBased() && !ServicePrincipals.IsServicePrincipal(requestContext, descriptor))
        {
          requestContext.TraceDataConditionally(7070911, level, "IdentityService", "IdentityStore", "IsMember returning false for non-user non-service-principal identity at deployment level", (Func<object>) (() => (object) new
          {
            DomainId = hostDomain.DomainId,
            groupDescriptor = groupDescriptor,
            descriptor = descriptor,
            forceCacheReload = forceCacheReload
          }), nameof (IsMember));
          requestContext.TraceConditionally(7070911, level, "IdentityService", "IdentityStore", (Func<string>) (() => Environment.StackTrace));
          return false;
        }
        if (!IdentityDescriptorComparer.Instance.Equals(this.m_masterDomain.DomainRoot, groupDescriptor))
        {
          requestContext.TraceDataConditionally(7070912, level, "IdentityService", "IdentityStore", "IsMember returning false for user in non-everyone-group at deployment level", (Func<object>) (() => (object) new
          {
            DomainId = hostDomain.DomainId,
            groupDescriptor = groupDescriptor,
            descriptor = descriptor,
            forceCacheReload = forceCacheReload
          }), nameof (IsMember));
          requestContext.TraceConditionally(7070912, level, "IdentityService", "IdentityStore", (Func<string>) (() => Environment.StackTrace));
          return false;
        }
        requestContext.TraceDataConditionally(7070913, TraceLevel.Verbose, "IdentityService", "IdentityStore", "IsMember returning true for user in everyone-group at deployment level", (Func<object>) (() => (object) new
        {
          DomainId = hostDomain.DomainId,
          groupDescriptor = groupDescriptor,
          descriptor = descriptor,
          forceCacheReload = forceCacheReload
        }), nameof (IsMember));
        requestContext.TraceConditionally(7070913, TraceLevel.Verbose, "IdentityService", "IdentityStore", (Func<string>) (() => Environment.StackTrace));
        return true;
      }
      long cacheStamp;
      bool? nullable = this.m_cache.IsMember(requestContext, hostDomain, groupDescriptor, descriptor, out cacheStamp, out IdentityMembershipInfo _);
      bool flag1 = requestContext.IsTracingSecurityEvaluation(80034, TraceLevel.Verbose, "IdentityService", "IdentityStore");
      if (forceCacheReload || !nullable.HasValue)
      {
        this.IncrementCacheMissPerfCounters();
        if (flag1)
          requestContext.TraceSecurityEvaluation(80035, TraceLevel.Verbose, "IdentityService", "IdentityStore", "[IsMember] Cache miss - member descriptor: {0}, group descriptor: {1}, host domain: {2}, forceCacheReload: {3}", (object) descriptor, (object) groupDescriptor, (object) hostDomain, (object) forceCacheReload);
        SequenceContext minSequenceContext = (SequenceContext) null;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.IsMember.RGM.PopulateMinSequenceId"))
          minSequenceContext = this.GetSequenceContext(requestContext, hostDomain);
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = this.ReadIdentityFromDatabase(requestContext, this.m_masterDomain, descriptor, QueryMembership.Expanded, QueryMembership.None, true, false, false, minSequenceContext);
        if (identity1 != null)
        {
          if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.ExecutionEnvironment.IsHostedDeployment && !identity1.IsContainer)
            identity1.MemberOf.Add(this.m_masterDomain.DomainRoot);
          requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext.Elevate(), (object) new AfterCoreReadIdentitiesEvent()
          {
            ReadIdentities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
            {
              identity1
            },
            ParentQueryMembership = QueryMembership.Expanded,
            ChildQueryMembership = QueryMembership.None,
            IdentitiesInScope = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null,
            MinSequenceContext = minSequenceContext
          });
          bool flag2 = this.m_cache.CompareAndSwapParentMemberships(requestContext, hostDomain, identity1, cacheStamp);
          if (flag1)
            requestContext.TraceSecurityEvaluation(flag2 ? 80037 : 80038, TraceLevel.Verbose, "IdentityService", "IdentityStore", "[IsMember] Compare and swap parent memberships operation result - success: {0}, descriptor: {1}, group descriptor: {2}, host domain: {3}, forceCacheReload: {4}, cacheStamp: {5}", (object) flag2, (object) descriptor, (object) groupDescriptor, (object) hostDomain, (object) forceCacheReload, (object) cacheStamp);
          nullable = new bool?(identity1.MemberOf.Contains<IdentityDescriptor>(groupDescriptor, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance));
        }
        else if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = new Microsoft.VisualStudio.Services.Identity.Identity();
          identity2.Descriptor = descriptor;
          identity2.MemberOf = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
          identity2.MemberIds = (ICollection<Guid>) Array.Empty<Guid>();
          Microsoft.VisualStudio.Services.Identity.Identity identity3 = identity2;
          this.m_cache.CompareAndSwapParentMemberships(requestContext, hostDomain, identity3, cacheStamp);
        }
      }
      else
      {
        this.IncrementCacheHitPerfCounters();
        if (flag1)
          requestContext.TraceSecurityEvaluation(80036, TraceLevel.Verbose, "IdentityService", "IdentityStore", "[IsMember] Cache hit - member descriptor: {0}, group descriptor: {1}, host domain: {2}, forceCacheReload: {3}", (object) descriptor, (object) groupDescriptor, (object) hostDomain, (object) forceCacheReload);
      }
      bool valueOrDefault = nullable.GetValueOrDefault();
      if (flag1)
        requestContext.TraceSecurityEvaluation(88888, TraceLevel.Verbose, "IdentityService", "IdentityStore", "[IsMember] IsMember: {0}, member descriptor: {1}, group descriptor: {2}, host domain: {3}", (object) valueOrDefault, (object) descriptor, (object) groupDescriptor, (object) hostDomain);
      return valueOrDefault;
    }

    private void PopulateMembershipAndFilter(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership childrenQuery,
      QueryMembership parentsQuery,
      bool includeRestrictedMembers,
      bool includeInactivatedMembers,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results,
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityTable,
      bool filterResults,
      SequenceContext minSequenceContext)
    {
      bool flag1 = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);
      bool flag2 = !requestContext.ExecutionEnvironment.IsHostedDeployment;
      bool flag3 = requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application);
      bool nullOutIdentities = !flag1 && !flag3;
      bool deactivateAllIdentities = ((flag1 ? 0 : (!flag3 ? 1 : 0)) | (flag2 ? 1 : 0)) != 0;
      bool flag4 = flag1 && !flag2 && results.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsContainer));
      bool flag5 = flag3 && results.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x =>
      {
        if (x == null)
          return false;
        return x.IsContainer || x.Descriptor.IsAggregateIdentityType();
      }));
      bool flag6 = ((flag1 ? 0 : (!flag3 ? 1 : 0)) | (flag2 ? 1 : 0) | (flag4 ? 1 : 0) | (flag5 ? 1 : 0)) != 0;
      bool flag7 = childrenQuery != QueryMembership.None || parentsQuery != 0;
      if (results.Count <= 0 || identityTable.Count <= 0 || !flag7 && !(filterResults & flag6))
        return;
      CollectionDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> unresolvedMembership = new CollectionDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>((CollectionDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>.CreateCollectionDelegate) (() => (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()));
      bool enableUseXtpProc = requestContext.IsFeatureEnabled(PlatformIdentityStore.EnableUseReadGroupMembershipXtpProcFeatureFlag);
      List<Microsoft.VisualStudio.Services.Identity.Identity> nulledOutIdentities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      Dictionary<Guid, bool> filteredItems = (Dictionary<Guid, bool>) null;
      bool shouldReadFromReplica = this.groupMembershipReadReplicaHelper.CanReadFromReplica(requestContext, childrenQuery, parentsQuery, minSequenceContext);
      bool isUnderFault;
      long minGroupSequenceIdToEnforce = PlatformIdentityStore.EvaluteMinGroupSequenceIdToEnforce(requestContext, minSequenceContext, shouldReadFromReplica, out isUnderFault);
      bool inScopeMembershipsOnly = IdentityMembershipHelper.ShouldReturnInScopeMemberships(requestContext);
      requestContext.TraceConditionally(2432352, TraceLevel.Info, "IdentityService", "IdentityStore", (Func<string>) (() => string.Format("Reading group memberships [childrenQuery: {0}, parentsQuery: {1}, includeRestricted: {2}, filterResults: {3}, scopeId: {4}, ids: {5}, minSequenceContext: {6}, shouldReadFromReplica: {7}, minGroupSequenceIdToEnforce:{8}, isUnderFault:{9}, inScopeMembershipsOnly:{10}]", (object) childrenQuery, (object) parentsQuery, (object) includeRestrictedMembers, (object) filterResults, (object) hostDomain.DomainId, (object) string.Join<Guid>(",", (IEnumerable<Guid>) identityTable.Keys), (object) (minSequenceContext?.ToString() ?? "null"), (object) shouldReadFromReplica, (object) minGroupSequenceIdToEnforce, (object) isUnderFault, (object) inScopeMembershipsOnly)));
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableReadGroupMembershipsComponent"))
        SqlReadReplicaHelper.ExecuteReadReplicaAwareOperation<ReadGroupMembershipsComponent>(PlatformIdentityStore.GetMembershipComponentContext(requestContext), shouldReadFromReplica, new Action<ReadGroupMembershipsComponent, bool>(ReadMemberships));
      else
        SqlReadReplicaHelper.ExecuteReadReplicaAwareOperation<GroupComponent>(PlatformIdentityStore.GetMembershipComponentContext(requestContext), shouldReadFromReplica, new Action<GroupComponent, bool>(ReadMemberships));
      PlatformIdentityStore.FilterCspPartnerUserIdentities(requestContext, results, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) nulledOutIdentities);
      if (filterResults)
      {
        requestContext.TraceConditionally(2432353, TraceLevel.Info, "IdentityService", "IdentityStore", (Func<string>) (() => string.Format("Group scope visibility results: {0}", filteredItems == null ? (object) string.Empty : (object) filteredItems.Serialize<Dictionary<Guid, bool>>())));
        requestContext.TraceConditionally(2432354, TraceLevel.Info, "IdentityService", "IdentityStore", (Func<string>) (() => string.Format("Nulled out identities: {0}", (object) string.Join(",", nulledOutIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => x.ToString()))))));
      }
      requestContext.TraceConditionally(2432355, TraceLevel.Info, "IdentityService", "IdentityStore", (Func<string>) (() => string.Format("Computed visibility results: {0}", (object) string.Join(",", results.Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => x != null ? x.Id.ToString() + "-" + x.IsActive.ToString() : "null"))))));
      if (unresolvedMembership.Count <= 0)
        return;
      PlatformIdentityStore.GetIdentityDatabaseTypes(requestContext, (IEnumerable<Guid>) unresolvedMembership.Keys).HasFlag((Enum) IdentityDatabaseTypes.SPS);

      void ReadMemberships(ReadGroupMembershipsComponentBase component, bool isFallback)
      {
        using (component)
        {
          using (ResultCollection resultCollection = component.ReadMemberships((IEnumerable<Guid>) identityTable.Keys, childrenQuery, parentsQuery, includeRestrictedMembers, includeInactivatedMembers ? this.m_inactiveMemberLifespan : new int?(), hostDomain.DomainId, filterResults, enableUseXtpProc, isFallback & isUnderFault ? -1L : minGroupSequenceIdToEnforce, inScopeMembershipsOnly))
          {
            foreach (GroupMembership groupMembership in resultCollection.GetCurrent<GroupMembership>())
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity = identityTable[groupMembership.QueriedId];
              identity.MemberOfIds.Add(groupMembership.Id);
              identity.MemberOf.Add(groupMembership.Descriptor);
            }
            resultCollection.NextResult();
            foreach (GroupMembership groupMembership in resultCollection.GetCurrent<GroupMembership>())
            {
              Microsoft.VisualStudio.Services.Identity.Identity element = identityTable[groupMembership.QueriedId];
              element.MemberIds.Add(groupMembership.Id);
              if (groupMembership.Descriptor != (IdentityDescriptor) null)
                element.Members.Add(groupMembership.Descriptor);
              else
                unresolvedMembership.AddElement(groupMembership.Id, element);
            }
            if (!filterResults)
              return;
            resultCollection.NextResult();
            filteredItems = new Dictionary<Guid, bool>();
            foreach (ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData filteredIdentityData in resultCollection.GetCurrent<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>())
              filteredItems[filteredIdentityData.IdentityId] = filteredIdentityData.Active;
            IdentityDescriptor y = this.Domain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup);
            for (int index = 0; index < results.Count; ++index)
            {
              Microsoft.VisualStudio.Services.Identity.Identity result = results[index];
              if (result != null && !result.IsCspPartnerUser && !ServicePrincipals.IsServicePrincipal(requestContext, result.Descriptor))
              {
                bool flag;
                if (filteredItems.TryGetValue(result.MasterId, out flag))
                {
                  if (!flag && (deactivateAllIdentities || result.IsContainer))
                    result.IsActive = false;
                }
                else if (!IdentityDescriptorComparer.Instance.Equals(result.Descriptor, y))
                {
                  if (nullOutIdentities || results[index].Descriptor.IsAggregateIdentityType())
                  {
                    nulledOutIdentities.Add(results[index]);
                    results[index] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
                  }
                  else if (deactivateAllIdentities || result.IsContainer)
                    result.IsActive = false;
                }
              }
            }
          }
        }
      }
    }

    internal static IVssRequestContext GetMembershipComponentContext(
      IVssRequestContext requestContext)
    {
      return requestContext.To(TeamFoundationHostType.Application);
    }

    internal static GroupComponent CreateGroupComponent(IVssRequestContext requestContext) => PlatformIdentityStore.GetMembershipComponentContext(requestContext).CreateComponent<GroupComponent>();

    internal static GroupComponent CreateGroupComponent(
      IVssRequestContext requestContext,
      int commandTimeout)
    {
      GroupComponent component = PlatformIdentityStore.GetMembershipComponentContext(requestContext).CreateComponent<GroupComponent>();
      component.CommandTimeout = SqlCommandTimeout.Max(component.CommandTimeout, commandTimeout);
      return component;
    }

    internal static ReadGroupMembershipsComponentBase CreateReadGroupMembershipsComponent(
      IVssRequestContext requestContext)
    {
      int num = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableReadGroupMembershipsComponent") ? 1 : 0;
      IVssRequestContext componentContext = PlatformIdentityStore.GetMembershipComponentContext(requestContext);
      return num != 0 ? (ReadGroupMembershipsComponentBase) componentContext.CreateComponent<ReadGroupMembershipsComponent>() : (ReadGroupMembershipsComponentBase) componentContext.CreateComponent<GroupComponent>();
    }

    private static long EvaluteMinGroupSequenceIdToEnforce(
      IVssRequestContext context,
      SequenceContext minSequenceContext,
      bool readFromReplica,
      out bool isUnderFault)
    {
      isUnderFault = false;
      if (!readFromReplica || minSequenceContext == null)
        return -1;
      isUnderFault = context.IsFeatureEnabled("VisualStudio.Services.Identity.RGM.EnableReadFromReadReplica.Fault");
      return !isUnderFault ? minSequenceContext.GroupSequenceId : long.MaxValue;
    }

    private bool ParseGroupName(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      string nameString,
      out Guid scopeId,
      out string scopeName,
      out string groupName,
      out bool recurse)
    {
      scopeId = hostDomain.DomainId;
      scopeName = (string) null;
      groupName = (string) null;
      recurse = true;
      int length = nameString.IndexOf('\\');
      if (length < 0)
      {
        groupName = nameString;
        return true;
      }
      string str = nameString.Substring(0, length).Trim();
      groupName = nameString.Substring(length + 1);
      bool flag = str.Length > 2 && str[0] == '[' && str[str.Length - 1] == ']';
      if (!flag && !LinkingUtilities.IsUriWellFormed(str))
        return false;
      if (string.Equals(str, "[SERVER]", StringComparison.OrdinalIgnoreCase))
      {
        scopeId = hostDomain.DomainId;
        recurse = false;
      }
      else if (!string.IsNullOrEmpty(str))
      {
        if (flag)
          str = str.Substring(1, str.Length - 2);
        Guid scopeId1;
        if (this.TryGetScopeId(requestContext, hostDomain, str, false, out scopeId1, out GroupScopeType _))
        {
          scopeId = scopeId1;
          recurse = false;
        }
        else
          scopeName = str;
      }
      return true;
    }

    internal virtual Guid GetScopeId(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      string scope,
      bool resolveFriendlyName)
    {
      return this.GetScopeId(requestContext, hostDomain, scope, resolveFriendlyName, out GroupScopeType _);
    }

    internal Guid GetScopeId(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      string scope,
      bool resolveFriendlyName,
      out GroupScopeType scopeType)
    {
      Guid scopeId;
      if (this.TryGetScopeId(requestContext, hostDomain, scope, resolveFriendlyName, out scopeId, out scopeType))
        return scopeId;
      throw new GroupScopeDoesNotExistException(scope);
    }

    internal bool TryGetScopeId(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      string scope,
      bool resolveFriendlyName,
      out Guid scopeId,
      out GroupScopeType scopeType)
    {
      if (!IdentityHelper.TryParseScopeId(scope, hostDomain.DomainId, out scopeId, out scopeType))
      {
        if (resolveFriendlyName)
        {
          int length = scope.Length;
          if (length > 2 && scope[0] == '[' && scope[length - 1] == ']')
            scope = scope.Substring(1, length - 2);
          using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
          {
            List<IdentityScope> items = groupComponent.QueryScopes(hostDomain.DomainId, scope).GetCurrent<IdentityScope>().Items;
            if (items.Count > 0)
            {
              IdentityScope identityScope = items.FirstOrDefault<IdentityScope>((Func<IdentityScope, bool>) (s => !s.IsGlobal)) ?? items[0];
              scopeId = identityScope.Id;
              scopeType = identityScope.ScopeType;
              return true;
            }
            scopeId = Guid.Empty;
            scopeType = GroupScopeType.Generic;
            return false;
          }
        }
        else
        {
          scopeId = Guid.Empty;
          scopeType = GroupScopeType.Generic;
          return false;
        }
      }
      else
      {
        scopeId = this.MapScopeId(requestContext, hostDomain, scopeId);
        return true;
      }
    }

    internal virtual Guid MapScopeId(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId)
    {
      if (scopeId != hostDomain.DomainId)
      {
        IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
        scopeId = this.m_cache.GetScopeMapper(requestContext, hostDomain).MapFromLocalId(requestContext, securityContext.ServiceHost.InstanceId, scopeId);
      }
      return scopeId;
    }

    internal virtual IReadOnlyList<GroupAuditRecord> GetGroupAuditRecords(
      IVssRequestContext requestContext,
      long startSequenceIdInclusive,
      long endSequenceIdInclusive)
    {
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        return groupComponent.GetGroupAuditRecords(startSequenceIdInclusive, endSequenceIdInclusive);
    }

    internal long GetCurrentGroupSequenceId(IVssRequestContext requestContext)
    {
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        return groupComponent.GetLatestGroupSequenceId(this.m_masterDomain.DomainRoot.Identifier, this.m_masterDomain.DomainId);
    }

    internal IList<Guid> GetAncestorScopeIds(IVssRequestContext requestContext, Guid scopeId)
    {
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        return groupComponent.GetAncestorScopeIds(scopeId);
    }

    private static void FilterCspPartnerUserIdentities(
      IVssRequestContext context,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> nullOutIdentities)
    {
      if (context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      Lazy<Guid> lazy = new Lazy<Guid>(new Func<Guid>(((RequestContextExtensions) context).GetOrganizationAadTenantId));
      for (int index = 0; index < results.Count; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity result = results[index];
        if (result != null && result.IsCspPartnerUser && PlatformIdentityStore.ShouldNullOutCspPartnerUserIdentity(context, result, lazy.Value))
        {
          nullOutIdentities.Add(result);
          results[index] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        }
      }
    }

    private static bool ShouldNullOutCspPartnerUserIdentity(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Identity.Identity cspIdentity,
      Guid hostTenantId)
    {
      return context.ExecutionEnvironment.IsOnPremisesDeployment || AadIdentityHelper.GetIdentityTenantId(cspIdentity.Descriptor) != hostTenantId;
    }

    internal PlatformIdentityStore()
    {
    }

    public PlatformIdentityStore(
      IVssRequestContext systemRequestContext,
      IdentityDomain hostDomain,
      IDictionary<string, IIdentityProvider> syncAgents,
      PlatformIdentityStore parentIdentityStore)
    {
      int cacheSize = this.GetCacheSize(systemRequestContext);
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service1 = vssRequestContext.GetService<IVssRegistryService>();
      this.m_inactiveMemberLifespan = new int?(service1.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Framework/Settings/DataAge/GroupMembership", (int) TimeSpan.FromDays(15.0).TotalMinutes));
      this.m_sendBisNotifications = service1.GetValue<bool>(vssRequestContext, (RegistryQuery) "/Service/Integration/Settings/SendIdentityChangedEvents", true);
      this.m_readIdentitiesFromDatabaseExtensions = systemRequestContext.GetExtensions<IReadIdentitiesFromDatabaseExtension>();
      this.m_updateIdentitiesInDatabaseExtensions = systemRequestContext.GetExtensions<IUpdateIdentitiesInDatabaseExtension>();
      PlatformIdentityCacheWarmer cacheWarmer = new PlatformIdentityCacheWarmer((IIdentityReader) this);
      this.m_cache = (IPlatformIdentityCache) new PlatformIdentityCache(systemRequestContext, hostDomain, cacheSize, (IIdentityCacheWarmer) cacheWarmer);
      Guid domainId = hostDomain.DomainId;
      this.InitializeIdentiesChangedEventProcessor();
      this.m_changeProcessor = new PlatformIdentityStore.PlatformIdentityChangeProcessor(systemRequestContext, hostDomain, this.m_cache, (Func<EventHandler<DescriptorChangeEventArgs>>) (() => this.DescriptorsChanged), (Func<EventHandler<DescriptorChangeNotificationEventArgs>>) (() => this.DescriptorsChangedNotification), (Func<IIdentityEventHandler<PlatformIdentityStore.IdentityChangeEventArgs>>) (() => this.IdentitiesChanged), PlatformIdentityStore.AddParentIdentitiesChangedHandlerDelegate(domainId, parentIdentityStore), PlatformIdentityStore.RemoveParentIdentitiesChangedHandlerDelegate(domainId, parentIdentityStore), new Action<long>(((IdentityStoreBase) this).ProcessGroupSequenceIdChange), (PlatformIdentityStore.PlatformIdentityChangeProcessor.ISequenceIDUtility) new PlatformIdentityStore.PlatformIdentityChangeProcessor.SequenceIDUtility());
      this.m_changeProcessor.Load(systemRequestContext);
      IImsCacheService service2 = systemRequestContext.GetService<IImsCacheService>();
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        ImsOperation operationsToCacheLocally = ImsOperation.Children | ImsOperation.IdentitiesInScope | ImsOperation.IdentityIdsByDisplayNamePrefixSearch | ImsOperation.IdentityIdsByEmailPrefixSearch | ImsOperation.IdentityIdsByAccountNamePrefixSearch;
        ImsOperation operationsToCacheRemotely = ImsOperation.Descendants | ImsOperation.IdentitiesInScope | ImsOperation.IdentityIdsByDisplayNamePrefixSearch | ImsOperation.IdentityIdsByEmailPrefixSearch | ImsOperation.IdentityIdsByAccountNamePrefixSearch;
        service2.Initialize(systemRequestContext, operationsToCacheLocally, operationsToCacheRemotely);
      }
      else
      {
        ImsOperation operationsToCacheLocally = ImsOperation.IdentityIdsByDisplayNamePrefixSearch | ImsOperation.IdentityIdsByEmailPrefixSearch | ImsOperation.IdentityIdsByAccountNamePrefixSearch | ImsOperation.IdentityIdsByDomainAccountNamePrefixSearch;
        service2.Initialize(systemRequestContext, operationsToCacheLocally, ImsOperation.None);
      }
      this.PropertyHelper = new IdentityPropertyHelper();
      this.m_masterDomain = hostDomain;
      this.m_syncAgents = syncAgents;
      this.m_parentIdentityStore = parentIdentityStore;
      this.m_masterDomain.IdentityStore = (object) this;
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        TeamFoundationTaskService service3 = systemRequestContext.GetService<TeamFoundationTaskService>();
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.TraceCacheStats), (object) null, (int) PlatformIdentityStore.s_traceCacheStatsInterval.TotalMilliseconds);
        Guid instanceId = systemRequestContext.ServiceHost.InstanceId;
        TeamFoundationTask task = teamFoundationTask;
        service3.AddTask(instanceId, task);
      }
      this.InitializeReadReplicaSettings(systemRequestContext);
      this.InitializeReadGroupsSettings(systemRequestContext);
    }

    private void InitializeReadReplicaSettings(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.UpdateReadReplicaSettings), true, (RegistryQuery) "/Service/Identity/Settings/ReadReplica");
      this.UpdateReadReplicaSettings(systemRequestContext, (RegistryEntryCollection) null);
    }

    private void UpdateReadReplicaSettings(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      this.m_percentageCallsToRouteToReadReplica = context.GetService<IVssRegistryService>().GetValue<int>(context, (RegistryQuery) "/Service/Identity/Settings/ReadReplica/PercentageCallsToRouteToReadReplica", true, 75);
    }

    private void InitializeReadGroupsSettings(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.UpdateReadGroupsSettings), true, (RegistryQuery) "/Service/Identity/Settings/IdentityStore/ReadGroupsChunkSize");
      this.UpdateReadGroupsSettings(systemRequestContext, (RegistryEntryCollection) null);
    }

    private void UpdateReadGroupsSettings(
      IVssRequestContext systemRequestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_readGroupsChunkSize = systemRequestContext.GetService<IVssRegistryService>().GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Identity/Settings/IdentityStore/ReadGroupsChunkSize", true, 1000);
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
      systemRequestContext.Trace(80117, TraceLevel.Verbose, "IdentityService", "IdentityStore", "PlatformIdentityStore CacheSize set to value: {0}", (object) cacheSize);
      return cacheSize;
    }

    internal void AddDomain(IVssRequestContext systemRequestContext, IdentityDomain hostDomain)
    {
      hostDomain.IdentityStore = (object) this;
      this.m_cache.AddDomain(systemRequestContext, hostDomain);
      IImsCacheService service = systemRequestContext.GetService<IImsCacheService>();
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        ImsOperation operationsToCacheLocally = ImsOperation.Children | ImsOperation.IdentitiesInScope | ImsOperation.IdentityIdsByDisplayNamePrefixSearch | ImsOperation.IdentityIdsByEmailPrefixSearch | ImsOperation.IdentityIdsByAccountNamePrefixSearch;
        ImsOperation operationsToCacheRemotely = ImsOperation.Descendants | ImsOperation.IdentitiesInScope | ImsOperation.IdentityIdsByDisplayNamePrefixSearch | ImsOperation.IdentityIdsByEmailPrefixSearch | ImsOperation.IdentityIdsByAccountNamePrefixSearch;
        service.Initialize(systemRequestContext, operationsToCacheLocally, operationsToCacheRemotely);
      }
      else
      {
        ImsOperation operationsToCacheLocally = ImsOperation.IdentityIdsByDisplayNamePrefixSearch | ImsOperation.IdentityIdsByEmailPrefixSearch | ImsOperation.IdentityIdsByAccountNamePrefixSearch | ImsOperation.IdentityIdsByDomainAccountNamePrefixSearch;
        service.Initialize(systemRequestContext, operationsToCacheLocally, ImsOperation.None);
      }
      systemRequestContext.GetService<TeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.IMS2PlatformIdentityIdTranslationChanged, new SqlNotificationCallback(this.OnIdentityIdTranslationChangedNotification), false);
    }

    internal void RemoveDomain(IVssRequestContext systemRequestContext, IdentityDomain hostDomain) => systemRequestContext.GetService<TeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.IMS2PlatformIdentityIdTranslationChanged, new SqlNotificationCallback(this.OnIdentityIdTranslationChangedNotification), false);

    internal void Unload(IVssRequestContext requestContext)
    {
      this.m_changeProcessor.Unload(requestContext);
      this.m_readIdentitiesFromDatabaseExtensions.Dispose();
      this.m_updateIdentitiesInDatabaseExtensions.Dispose();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.UpdateReadReplicaSettings));
      service.UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.UpdateReadGroupsSettings));
      this.m_cache.Unload(requestContext);
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity CreateUnauthenticatedIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor descriptor,
      string domain,
      string accountName,
      string description,
      Guid identityId,
      string customDisplayName = null,
      string mailAddress = null,
      string directoryAlias = null,
      bool persistToDatabase = true)
    {
      if (descriptor == (IdentityDescriptor) null)
        descriptor = IdentityHelper.CreateUnauthenticatedDescriptor(SidIdentityHelper.NewSid(hostDomain.DomainId));
      if (identityId == Guid.Empty)
        identityId = Guid.NewGuid();
      Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity.Id = identityId;
      identity.Descriptor = descriptor;
      identity.ProviderDisplayName = accountName;
      identity.CustomDisplayName = customDisplayName;
      identity.IsActive = false;
      identity.UniqueUserId = 0;
      identity.IsContainer = false;
      identity.Members = (ICollection<IdentityDescriptor>) null;
      identity.MemberOf = (ICollection<IdentityDescriptor>) null;
      identity.MasterId = identityId;
      Microsoft.VisualStudio.Services.Identity.Identity unauthenticatedIdentity = identity;
      unauthenticatedIdentity.SetProperty("Description", (object) description);
      unauthenticatedIdentity.SetProperty("Domain", (object) domain);
      unauthenticatedIdentity.SetProperty("Account", (object) accountName);
      unauthenticatedIdentity.SetProperty("Mail", (object) mailAddress);
      unauthenticatedIdentity.SetProperty("DirectoryAlias", (object) directoryAlias);
      unauthenticatedIdentity.SetProperty("ConfirmedNotificationAddress", (object) mailAddress);
      unauthenticatedIdentity.SetProperty("CustomNotificationAddresses", (object) mailAddress);
      if (persistToDatabase)
      {
        this.UpdateIdentitiesInDatabase(requestContext, hostDomain, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          unauthenticatedIdentity
        }, true);
        List<Tuple<IdentityDescriptor, Guid, bool>> updates = new List<Tuple<IdentityDescriptor, Guid, bool>>()
        {
          new Tuple<IdentityDescriptor, Guid, bool>(hostDomain.DomainRoot, identityId, false)
        };
        this.UpdateMembership(requestContext, hostDomain, updates, false, true, true);
      }
      return unauthenticatedIdentity;
    }

    internal List<SyncQueueData> ReadSyncQueue(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      int limit)
    {
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
      {
        using (ResultCollection resultCollection = identityComponent.ReadSyncQueue(limit))
          return resultCollection.GetCurrent<SyncQueueData>().Items;
      }
    }

    internal void UpdateSyncQueue(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Guid> queueDeletes,
      IList<Tuple<Guid, bool, bool>> queueUpdates)
    {
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        identityComponent.UpdateSyncQueue((IEnumerable<Guid>) queueDeletes, (IEnumerable<Tuple<Guid, bool, bool>>) queueUpdates);
      if (queueUpdates == null || queueUpdates.Count <= 0)
        return;
      this.QueueSyncJob(requestContext);
    }

    private void QueueSyncJob(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      vssRequestContext.GetService<TeamFoundationJobService>().QueueJobsNow(vssRequestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        TeamFoundationJobReferences.IdentitySyncNow
      });
    }

    internal int CleanupDescriptorChangeQueue(IVssRequestContext requestContext, int fromDays)
    {
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        return identityComponent.CleanupDescriptorChangeQueue(fromDays);
    }

    public virtual bool UpdateIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>(identities, nameof (identities));
      List<Microsoft.VisualStudio.Services.Identity.Identity> source1 = new List<Microsoft.VisualStudio.Services.Identity.Identity>(identities);
      List<Microsoft.VisualStudio.Services.Identity.Identity> list1 = source1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => IdentityValidation.IsTeamFoundationType(identity.Descriptor))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> externalIdentities = source1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => !IdentityValidation.IsTeamFoundationType(identity.Descriptor))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (requestContext.IsTracing(80060, TraceLevel.Info, "IdentityService", "IdentityStore"))
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Update Identities count: {0}", (object) source1.Count));
        bool flag1 = false;
        bool flag2 = false;
        if (requestContext.IsTracing(80061, TraceLevel.Info, "IdentityService", "IdentityStore"))
          flag1 = true;
        else
          stringBuilder.AppendLine("To track changed Core Properties, enable TP: 80061");
        if (requestContext.IsTracing(80062, TraceLevel.Info, "IdentityService", "IdentityStore"))
          flag2 = true;
        else
          stringBuilder.AppendLine("To track property bag, enable TP: 80062");
        requestContext.Trace(80060, TraceLevel.Verbose, "IdentityService", "IdentityStore", "UpdateIdentities {0}", (object) stringBuilder.ToString());
        stringBuilder.Clear();
        int num = 1;
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in source1)
        {
          if (num > 10)
          {
            stringBuilder.AppendLine("Identities > 10. Tracing only utmost 10");
            break;
          }
          IdentityDescriptor descriptor = identity1.Descriptor;
          stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Identity descriptor: {0}", (object) identity1.Descriptor));
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = this.m_cache.ReadIdentity(requestContext, hostDomain, identity1.Id, QueryMembership.None);
          if (flag1)
          {
            stringBuilder.AppendLine("CHANGED CORE PROPERTIES");
            string x = "Not in Cache";
            if (identity2 != null)
              x = identity2.ProviderDisplayName;
            if (!StringComparer.OrdinalIgnoreCase.Equals(x, identity1.ProviderDisplayName))
              stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ProviderDisplayName Old:{0} New:{1})", (object) x, (object) identity1.ProviderDisplayName));
            foreach (string identityAttribute in PlatformIdentityStore.m_identityAttributes)
            {
              string y = identity2 != null ? identity2.GetProperty<string>(identityAttribute, string.Empty) : "Not in Cache";
              string property = identity1.GetProperty<string>(identityAttribute, string.Empty);
              if (!StringComparer.OrdinalIgnoreCase.Equals(property, y))
                stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} Old:{1} New:{2}", (object) identityAttribute, (object) y, (object) property));
            }
          }
          if (flag2)
          {
            stringBuilder.AppendLine("PROPERTY BAG ENTRIES");
            if (identity1.HasModifiedProperties)
            {
              stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Old - {0}", identity2 != null ? (object) this.GetPropertiesStringForTracing(identity2) : (object) "Not in Cache"));
              stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "New - {0}", (object) this.GetPropertiesStringForTracing(identity1)));
            }
          }
          requestContext.Trace(80060, TraceLevel.Verbose, "IdentityService", "IdentityStore", stringBuilder.ToString());
          stringBuilder.Clear();
          ++num;
        }
      }
      if (requestContext.IsTracing(80063, TraceLevel.Info, "IdentityService", "IdentityStore"))
      {
        try
        {
          List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = source1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && identity.IsClaims && identity.GetProperty<string>("PUID", (string) null) == string.Empty)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (list2.Count > 0)
          {
            foreach (Microsoft.VisualStudio.Services.Identity.Identity identity3 in list2)
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity4 = this.m_cache.ReadIdentity(requestContext, hostDomain, identity3.Id, QueryMembership.None);
              string str;
              if (identity4 == null)
              {
                str = "Not in cache";
              }
              else
              {
                string property = identity4.GetProperty<string>("PUID", (string) null);
                switch (property)
                {
                  case null:
                    str = "a Null value";
                    break;
                  case "":
                    str = "an Empty value";
                    break;
                  default:
                    str = property;
                    break;
                }
              }
              requestContext.Trace(80063, TraceLevel.Info, "IdentityService", "IdentityStore", string.Format("Identity with id = '{0} ' and master id = '{1}' is being updated with an Empty Puid.  Previous value was: {2}", (object) identity3.Id, (object) identity3.MasterId, (object) str));
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(80063, "IdentityService", "IdentityStore", ex);
        }
      }
      IdentityHelper.LogInvalidServiceIdentityWhenNecessary(requestContext, identities);
      bool flag = false;
      if (list1.Count > 0)
        flag = this.UpdateGroupsInDatabase(requestContext, hostDomain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) list1);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updatedIdentities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      if (externalIdentities.Count > 0)
      {
        requestContext.TraceDataConditionally(414581, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Updating external identities.", (Func<object>) (() => (object) new
        {
          Count = externalIdentities.Count,
          identity = externalIdentities.First<Microsoft.VisualStudio.Services.Identity.Identity>()
        }), nameof (UpdateIdentities));
        updatedIdentities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.UpdateIdentitiesInDatabase(requestContext, hostDomain, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) externalIdentities, allowMetadataUpdates);
        if (updatedIdentities.Count > 0)
        {
          requestContext.Trace(414580, TraceLevel.Info, "IdentityService", "IdentityStore", "Updated Identities count: {0}", (object) updatedIdentities.Count);
          flag = true;
        }
        else
          requestContext.Trace(414582, TraceLevel.Info, "IdentityService", "IdentityStore", "Failed to update identities: {0}", (object) externalIdentities.Count);
      }
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in source1)
      {
        HashSet<string> modifiedProperties = identity.GetModifiedProperties();
        if (modifiedProperties != null)
        {
          foreach (string propertyName in modifiedProperties)
            IdentityPropertyKpis.IncrementWritesForProperty(propertyName);
        }
      }
      if (requestContext.IsTracing(80055, TraceLevel.Info, "IdentityService", "IdentityStore") && source1 != null && source1.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source2 = source1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null && i.ResourceVersion < 2));
        string str = string.Join(",", source2.Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (i => i.Id.ToString())).Take<string>(10));
        requestContext.Trace(80055, TraceLevel.Info, "IdentityService", "IdentityStore", "Identities with invalid resource version, count {0}: {1}", (object) source2.Count<Microsoft.VisualStudio.Services.Identity.Identity>(), (object) str);
      }
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UsePartitionedGroupProperties") && list1.Count<Microsoft.VisualStudio.Services.Identity.Identity>() > 0)
        this.PropertyHelper.UpdateExtendedProperties(requestContext, IdentityPropertyScope.Local, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) list1, new Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity, IdentityPropertyScope, string, bool>(this.SkipPropertyUpdateInHostedPropertyService));
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source3 = requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? this.PropertyHelper.UpdateExtendedProperties(requestContext, IdentityPropertyScope.Global, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source1) : this.PropertyHelper.UpdateExtendedProperties(requestContext, IdentityPropertyScope.Global, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source1, new Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity, IdentityPropertyScope, string, bool>(this.SkipPropertyUpdateInHostedPropertyService));
      if (requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.IdentityStore.CacheExtendedProperties") && source3 != null && source3.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identitiesToPublish = updatedIdentities == null ? source3 : source3.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (id => !updatedIdentities.Contains(id)));
        this.m_changeProcessor.PublishIdentityChanges(requestContext, hostDomain, identitiesToPublish, (Action<IVssRequestContext, IdentityDomain, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>) ((publishContext, publishDomain, publishIdentities) => this.InvalidateIdentities(publishContext, publishDomain, (IList<Guid>) publishIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (i => i.Id)).ToList<Guid>())));
      }
      return flag;
    }

    public virtual bool SwapIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid id1,
      Guid id2)
    {
      bool flag = false;
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity1 = this.ReadIdentities(requestContext, hostDomain, (IList<Guid>) new List<Guid>()
      {
        id1
      }, QueryMembership.None, (IEnumerable<string>) null, false)[0];
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity2 = this.ReadIdentities(requestContext, hostDomain, (IList<Guid>) new List<Guid>()
      {
        id2
      }, QueryMembership.None, (IEnumerable<string>) null, false)[0];
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(readIdentity1, "identity1");
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(readIdentity2, "identity2");
      string property1 = readIdentity1.GetProperty<string>("Domain", string.Empty);
      string property2 = readIdentity2.GetProperty<string>("Domain", string.Empty);
      if (string.IsNullOrEmpty(property1) || !string.Equals(property1, property2, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Both identities must have the same domain");
      string property3 = readIdentity1.GetProperty<string>("Account", string.Empty);
      string property4 = readIdentity2.GetProperty<string>("Account", string.Empty);
      if (string.IsNullOrEmpty(property3) || !string.Equals(property3, property4, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Both identities must have the same account name");
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        flag = identityComponent.SwapIdentity(property1, property3, readIdentity1.Id, readIdentity2.Id);
      List<Microsoft.VisualStudio.Services.Identity.Identity> identitiesToPublish = new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        readIdentity1,
        readIdentity2
      };
      this.m_changeProcessor.PublishIdentityChanges(requestContext, this.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identitiesToPublish, (Action<IVssRequestContext, IdentityDomain, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>) ((publishContext, publishDomain, publishIdentities) => this.InvalidateIdentities(publishContext, publishDomain, (IList<Guid>) publishIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (i => i.Id)).ToList<Guid>())));
      return flag;
    }

    private string GetPropertiesStringForTracing(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string key in identity.Properties.Keys)
        stringBuilder.Append(string.Format("{0}:{1} ", (object) key, identity.Properties[key]));
      return stringBuilder.ToString();
    }

    internal List<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentitiesInDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates,
      HashSet<string> propertiesToUpdate = null,
      bool favorCurrentlyActive = false)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      HashSet<Guid> createdAndBoundIdentityIds = new HashSet<Guid>();
      List<Guid> deletedIdentityIds = new List<Guid>();
      List<KeyValuePair<Guid, Guid>> identitiesToTransfer = new List<KeyValuePair<Guid, Guid>>();
      IdentityChangedData identityChangedData = (IdentityChangedData) null;
      bool flag1 = requestContext.IsFeatureEnabled(PlatformIdentityStore.EnableAutoMigrateNewIdentities);
      if (((identities == null ? 0 : (identities.Any<Microsoft.VisualStudio.Services.Identity.Identity>() ? 1 : 0)) & (flag1 ? 1 : 0)) != 0)
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
        {
          if (identity != null && identity.Id == Guid.Empty)
            identity.ResourceVersion = 2;
        }
      }
      bool flag2 = requestContext.IsFeatureEnabled(PlatformIdentityStore.ResolveByOid);
      requestContext.RootContext.Items[PlatformIdentityStore.ResolveByOid] = (object) flag2;
      IdentityChangedData parentIdentityChangedData = (IdentityChangedData) null;
      List<Microsoft.VisualStudio.Services.Identity.Identity> parentChangedIdentities = (List<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      List<Microsoft.VisualStudio.Services.Identity.Identity> sprocIdentities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        if (PlatformIdentityStore.GetIdentityDatabaseType(requestContext, identity.Descriptor) == IdentityDatabaseTypes.SPS)
        {
          requestContext.Trace(351923, TraceLevel.Info, "IdentityService", "IdentityStore", "Adding identity for partition level update: {0} {1}", (object) identity.Id, (object) identity.SubjectDescriptor);
          sprocIdentities.Add(identity);
        }
      }
      if (sprocIdentities.Count > 0)
      {
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
          identityList.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) UpdateIdentitiesInDatabase(identityComponent, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) sprocIdentities, false));
      }
      this.PostProcessExtensionsAfterUpdateIdentitiesInDatabase(requestContext, createdAndBoundIdentityIds, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities, allowMetadataUpdates);
      if (createdAndBoundIdentityIds.Count > 0)
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
        {
          if (createdAndBoundIdentityIds.Contains(identity.Id))
          {
            EtwIdentityTracer.Instance.Trace(requestContext, requestContext.RequestTracer, identity);
            if (requestContext.IsTracing(80063, TraceLevel.Info, "IdentityService", "IdentityStore") && !identity.Descriptor.IsBindPendingType() && identity.GetProperty<string>("PUID", (string) null) == null)
              requestContext.Trace(80063, TraceLevel.Info, "IdentityService", "IdentityStore", string.Format("Identity with id = '{0}' and master id = '{1}' was created with a null PUID.", (object) identity.Id, (object) identity.MasterId));
          }
        }
      }
      if (deletedIdentityIds.Count > 0)
      {
        long num;
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
          num = groupComponent.DeleteGroupMemberships((IEnumerable<Guid>) deletedIdentityIds, true, true);
        if (num > (long) identityChangedData.GroupSequenceId)
          identityChangedData.GroupSequenceId = checked ((int) num);
      }
      if (identitiesToTransfer.Count > 0)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<IdentityMembershipTransferService>().TransferMembership(vssRequestContext, (IEnumerable<KeyValuePair<Guid, Guid>>) identitiesToTransfer);
      }
      if (parentIdentityChangedData != null && parentIdentityChangedData.IdentitySequenceId > -1)
      {
        List<Guid> list = parentChangedIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (id => id.MasterId)).ToList<Guid>();
        this.ProcessParentIdentityChangeOnAuthor(requestContext, hostDomain, parentIdentityChangedData.IdentitySequenceId, (ICollection<Guid>) list, parentIdentityChangedData.DescriptorChangeType, (ICollection<Guid>) parentIdentityChangedData.DescriptorChangeIds);
      }
      if (identityChangedData != null)
        this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, identityChangedData.IdentitySequenceId, identityChangedData.GroupSequenceId, identityChangedData.DescriptorChangeType, (ICollection<Guid>) identityChangedData.DescriptorChangeIds);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IIdMapper idMapper = this.m_cache.GetIdMapper(requestContext, hostDomain);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        IVssRequestContext requestContext1 = requestContext;
        AfterUpdateIdentitiesOnStoreEvent notificationEvent = new AfterUpdateIdentitiesOnStoreEvent();
        notificationEvent.Identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        notificationEvent.IdentityMapper = idMapper;
        notificationEvent.IdentityDomain = hostDomain;
        service.PublishDecisionPoint(requestContext1, (object) notificationEvent);
      }
      return identityList;

      IList<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentitiesInDatabase(
        IdentityManagementComponent db,
        IList<Microsoft.VisualStudio.Services.Identity.Identity> sprocIdentities,
        bool isParentStore)
      {
        bool updateIdentityAudit = IdentityStoreUtilities.IdentityAuditEnabled(requestContext);
        List<Guid> createdAndBoundIds;
        List<Guid> deletedIds;
        IdentityChangedData identityChangedData;
        List<KeyValuePair<Guid, Guid>> identitiesToTransfer;
        List<Microsoft.VisualStudio.Services.Identity.Identity> source;
        try
        {
          requestContext.Trace(247890, TraceLevel.Info, "IdentityService", "IdentityStore", "Updating identities in database. Count: {0}", (object) sprocIdentities.Count);
          source = db.UpdateIdentities(sprocIdentities, propertiesToUpdate, favorCurrentlyActive, updateIdentityAudit, allowMetadataUpdates, out createdAndBoundIds, out deletedIds, out identityChangedData, out identitiesToTransfer);
          requestContext.Trace(851272, TraceLevel.Info, "IdentityService", "IdentityStore", "Updated identities. Change count: {0}", (object) source.Count);
          try
          {
            if (requestContext.IsTracing(80995, TraceLevel.Info, "IdentityService", "IdentityStore"))
            {
              string callStack = requestContext.IsTracing(80996, TraceLevel.Info, "IdentityService", "IdentityStore") ? Environment.StackTrace : string.Empty;
              Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dedupedDictionary = source.ToDedupedDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x));
              foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
              {
                Microsoft.VisualStudio.Services.Identity.Identity valueOrDefault = dedupedDictionary.GetValueOrDefault<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(identity.Id, (Microsoft.VisualStudio.Services.Identity.Identity) null);
                IdentityTracing.TraceUpdateIdentityInDatabase(hostDomain, identity, allowMetadataUpdates, propertiesToUpdate, valueOrDefault != null, valueOrDefault, callStack);
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(80995, "IdentityService", "IdentityStore", ex);
          }
        }
        catch (Exception ex) when (ex is SqlTypeException || ex is SqlException)
        {
          requestContext.TraceSerializedConditionally(80395, TraceLevel.Error, "IdentityService", "IdentityStore", "IdentityManagementComponent.UpdateIdentities failed with ex when called with \n sprocIdentities = {0} \n propertiesToUpdate = {1} \n favorCurrentlyActive = {2} \n updateIdentityAudit = {3} \n allowMetadataUpdates = {4}", (object) sprocIdentities, (object) propertiesToUpdate, (object) favorCurrentlyActive, (object) updateIdentityAudit, (object) allowMetadataUpdates);
          throw;
        }
        createdAndBoundIdentityIds.UnionWith((IEnumerable<Guid>) createdAndBoundIds);
        deletedIdentityIds.AddRange((IEnumerable<Guid>) deletedIds);
        identitiesToTransfer.AddRange((IEnumerable<KeyValuePair<Guid, Guid>>) identitiesToTransfer);
        if (!isParentStore)
        {
          identityChangedData = identityChangedData;
        }
        else
        {
          parentIdentityChangedData = identityChangedData;
          parentChangedIdentities = source;
        }
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) source;
      }
    }

    private void PostProcessExtensionsAfterUpdateIdentitiesInDatabase(
      IVssRequestContext requestContext,
      HashSet<Guid> createdAndBoundIds,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> updatedIdentities,
      bool allowMetadataUpdates)
    {
      foreach (IUpdateIdentitiesInDatabaseExtension databaseExtension in (IEnumerable<IUpdateIdentitiesInDatabaseExtension>) this.m_updateIdentitiesInDatabaseExtensions)
        databaseExtension.PostProcess(requestContext, createdAndBoundIds, updatedIdentities, allowMetadataUpdates);
    }

    public void TransferIdentityRights(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<KeyValuePair<Guid, Guid>> identityRightsMap,
      IList<IdentityIdTranslation> identityIdTranslations,
      bool validateSourceData = false)
    {
      requestContext.TraceEnter(80420, "AccountLinking", "IdentityStore", nameof (TransferIdentityRights));
      try
      {
        if (!IdentityTranslationHelper.IsEnabled(requestContext))
          throw new InvalidOperationException("Identity ID translation feature is not enabled.");
        requestContext.CheckProjectCollectionRequestContext();
        try
        {
          IDisposableReadOnlyList<ITransferIdentityRightsSubscriber> extensions = requestContext.GetExtensions<ITransferIdentityRightsSubscriber>(ExtensionLifetime.Service);
          if (extensions.Count == 0)
            throw new InvalidOperationException("No handlers are registered for ITransferIdentityRightsSubscriber.");
          if (validateSourceData)
          {
            foreach (ITransferIdentityRightsSubscriber rightsSubscriber in (IEnumerable<ITransferIdentityRightsSubscriber>) extensions)
              rightsSubscriber.PreValidateTransferIdentityRights(requestContext, identityRightsMap);
          }
          Dictionary<Guid, Guid> oldMasterMap = new Dictionary<Guid, Guid>();
          if (requestContext.IsFeatureEnabled(PlatformIdentityStore.EnableTransferAcesDuringIdentityTranslation))
          {
            IdentityIdTranslationService tranService = requestContext.GetService<IdentityIdTranslationService>();
            identityRightsMap.ForEach<KeyValuePair<Guid, Guid>>((Action<KeyValuePair<Guid, Guid>>) (kvp => oldMasterMap.Add(kvp.Key, tranService.TranslateToMasterId(requestContext, kvp.Key))));
          }
          this.UpdateTranslations(requestContext, identityIdTranslations);
          requestContext.Trace(80423, TraceLevel.Info, "AccountLinking", "IdentityStore", "Translations are updated.");
          this.TransferAces(requestContext, identityRightsMap, oldMasterMap);
          requestContext.Trace(80423, TraceLevel.Info, "AccountLinking", "IdentityStore", "ACEs are transferred.");
          this.TransferMembership(requestContext, identityRightsMap, DescriptorChangeType.Major);
          requestContext.Trace(80423, TraceLevel.Info, "AccountLinking", "IdentityStore", "Memberships are transferred.");
          foreach (ITransferIdentityRightsSubscriber rightsSubscriber in (IEnumerable<ITransferIdentityRightsSubscriber>) extensions)
            rightsSubscriber.TransferIdentityRights(requestContext, identityRightsMap);
        }
        catch (InvalidIdentityIdTranslationException ex)
        {
          IList<IdentityIdTranslation> identityIdTranslationList = (IList<IdentityIdTranslation>) null;
          IList<IdentityIdTranslation> traceEnumerable = (IList<IdentityIdTranslation>) null;
          try
          {
            identityIdTranslationList = this.GetTranslations(requestContext);
            List<IdentityIdTranslation> newTranslationsWithoutSelfMappings = identityIdTranslations.Where<IdentityIdTranslation>((Func<IdentityIdTranslation, bool>) (x => x.Id != x.MasterId)).ToList<IdentityIdTranslation>();
            traceEnumerable = (IList<IdentityIdTranslation>) identityIdTranslationList.Where<IdentityIdTranslation>((Func<IdentityIdTranslation, bool>) (x => x.Id != x.MasterId)).ToList<IdentityIdTranslation>().Where<IdentityIdTranslation>((Func<IdentityIdTranslation, bool>) (currentTranslation => newTranslationsWithoutSelfMappings.Select<IdentityIdTranslation, Guid>((Func<IdentityIdTranslation, Guid>) (newTranslation => newTranslation.MasterId)).Contains<Guid>(currentTranslation.Id) || newTranslationsWithoutSelfMappings.Select<IdentityIdTranslation, Guid>((Func<IdentityIdTranslation, Guid>) (newTranslation => newTranslation.Id)).Contains<Guid>(currentTranslation.MasterId))).ToList<IdentityIdTranslation>();
          }
          catch
          {
          }
          requestContext.TraceException(80424, "AccountLinking", "IdentityStore", (Exception) ex);
          requestContext.TraceEnumerableConditionally<IdentityIdTranslation>(80424, TraceLevel.Error, "AccountLinking", "IdentityStore", "Exception is thrown while executing transfer identity rights, possibleProblematicTranslations: ", (IEnumerable<IdentityIdTranslation>) traceEnumerable, methodName: nameof (TransferIdentityRights));
          requestContext.TraceEnumerableConditionally<IdentityIdTranslation>(80424, TraceLevel.Error, "AccountLinking", "IdentityStore", "Exception is thrown while executing transfer identity rights, currentTranslations: ", (IEnumerable<IdentityIdTranslation>) identityIdTranslationList, methodName: nameof (TransferIdentityRights));
          requestContext.TraceEnumerableConditionally<IdentityIdTranslation>(80424, TraceLevel.Error, "AccountLinking", "IdentityStore", "Exception is thrown while executing transfer identity rights, identityIdTranslations: ", (IEnumerable<IdentityIdTranslation>) identityIdTranslations, methodName: nameof (TransferIdentityRights));
          throw;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(80424, "AccountLinking", "IdentityStore", ex);
          requestContext.TraceSerializedConditionally(80424, TraceLevel.Error, "AccountLinking", "IdentityStore", "Exception is thrown while executing transfer identity rights. New Translations: {0}", (object) identityIdTranslations);
          throw;
        }
        requestContext.Trace(80423, TraceLevel.Info, "AccountLinking", "IdentityStore", "Clearing identity id translation cache.");
        this.m_cache.OnIdentityIdTranslationChanged(requestContext, new IdentityIdTranslationChangeData()
        {
          TranslationChangeType = IdentityIdTranslationChangeType.BulkChange
        });
      }
      finally
      {
        requestContext.TraceLeave(80429, "AccountLinking", "IdentityStore", nameof (TransferIdentityRights));
      }
    }

    private void UpdateTranslations(
      IVssRequestContext requestContext,
      IList<IdentityIdTranslation> identityIdTranslations)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
      {
        requestContext.TraceSerializedConditionally(80422, TraceLevel.Error, "AccountLinking", "IdentityStore", "Transfer identity rights are called at Organization level. Translations: {0}", (object) identityIdTranslations);
      }
      else
      {
        using (IdentityIdTranslationComponent component = requestContext.CreateComponent<IdentityIdTranslationComponent>())
          component.UpdateTranslations(identityIdTranslations);
      }
    }

    private void TransferAces(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap,
      Dictionary<Guid, Guid> oldMasterMap)
    {
      if (!requestContext.IsFeatureEnabled(PlatformIdentityStore.EnableTransferAcesDuringIdentityTranslation))
        return;
      IVssRequestContext enterpriseRC = requestContext.To(TeamFoundationHostType.Application);
      IdentityService service = enterpriseRC.GetService<IdentityService>();
      IList<IVssSecurityNamespace> securityNamespaces1 = PlatformIdentityStore.GetLocalSecurityNamespaces(requestContext);
      IList<IVssSecurityNamespace> securityNamespaces2 = PlatformIdentityStore.GetLocalSecurityNamespaces(enterpriseRC);
      foreach (KeyValuePair<Guid, Guid> userIdTransfer in userIdTransferMap)
      {
        Guid key;
        if (!oldMasterMap.TryGetValue(userIdTransfer.Key, out key))
          key = userIdTransfer.Key;
        IdentityDescriptor fromDescriptorForAces = service.ReadIdentities(enterpriseRC, (IList<Guid>) new List<Guid>()
        {
          key
        }, QueryMembership.None, (IEnumerable<string>) null)?[0]?.Descriptor;
        if (!(fromDescriptorForAces == (IdentityDescriptor) null))
        {
          IdentityDescriptor toDescriptorForAces = service.ReadIdentities(enterpriseRC, (IList<Guid>) new List<Guid>()
          {
            userIdTransfer.Value
          }, QueryMembership.None, (IEnumerable<string>) null)?[0]?.Descriptor;
          securityNamespaces1.ForEach<IVssSecurityNamespace>((Action<IVssSecurityNamespace>) (collectionNamespace => PlatformIdentityStore.TransferAcesInSecurityNamespace(requestContext, collectionNamespace, fromDescriptorForAces, toDescriptorForAces)));
          securityNamespaces2.ForEach<IVssSecurityNamespace>((Action<IVssSecurityNamespace>) (enterpriseNamespace => PlatformIdentityStore.TransferAcesInSecurityNamespace(enterpriseRC, enterpriseNamespace, fromDescriptorForAces, toDescriptorForAces)));
        }
      }
    }

    private static IList<IVssSecurityNamespace> GetLocalSecurityNamespaces(
      IVssRequestContext requestContext)
    {
      return (IList<IVssSecurityNamespace>) requestContext.GetService<LocalSecurityService>().GetSecurityNamespaces(requestContext).Select<LocalSecurityNamespace, IVssSecurityNamespace>((Func<LocalSecurityNamespace, IVssSecurityNamespace>) (ns => (IVssSecurityNamespace) requestContext.GetService<LocalSecurityService>().GetSecurityNamespace(requestContext, ns.Description.NamespaceId))).ToList<IVssSecurityNamespace>();
    }

    private static void TransferAcesInSecurityNamespace(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      IdentityDescriptor fromDescriptorForAces,
      IdentityDescriptor toDescriptorForAces)
    {
      if (!(securityNamespace.GetType() == typeof (LocalSecurityNamespace)))
        return;
      securityNamespace.QueryAccessControlLists(requestContext, (string) null, (IEnumerable<EvaluationPrincipal>) new List<EvaluationPrincipal>()
      {
        (EvaluationPrincipal) fromDescriptorForAces
      }, false, false).ForEach<QueriedAccessControlList>((Action<QueriedAccessControlList>) (queriedAcl => queriedAcl.AccessControlEntries.Where<QueriedAccessControlEntry>((Func<QueriedAccessControlEntry, bool>) (queriedAce => queriedAce.IdentityDescriptor.Equals(fromDescriptorForAces))).ForEach<QueriedAccessControlEntry>((Action<QueriedAccessControlEntry>) (queriedAce =>
      {
        if (!queriedAce.IdentityDescriptor.Equals(fromDescriptorForAces) || queriedAce.Allow + queriedAce.Deny == 0)
          return;
        securityNamespace.EnsurePermissions(requestContext, queriedAcl.Token, toDescriptorForAces, queriedAce.Allow, queriedAce.Deny, true);
        requestContext.TraceConditionally(80423, TraceLevel.Info, "AccountLinking", "IdentityStore", (Func<string>) (() => string.Format("Gave allow: {0}, deny: {1} on token {2} in namespace {3} to identity with descriptor {4}.", (object) queriedAce.Allow, (object) queriedAce.Deny, (object) queriedAcl.Token, (object) securityNamespace.Description.NamespaceId, (object) toDescriptorForAces)));
        securityNamespace.RemoveAccessControlEntries(requestContext, queriedAcl.Token, (IEnumerable<IAccessControlEntry>) new List<IAccessControlEntry>()
        {
          (IAccessControlEntry) new AccessControlEntry(fromDescriptorForAces, queriedAce.Allow, queriedAce.Deny)
        });
        requestContext.TraceConditionally(80423, TraceLevel.Info, "AccountLinking", "IdentityStore", (Func<string>) (() => string.Format("Removed allow: {0}, deny: {1} on token {2} in namespace {3} from identity with descriptor {4}.", (object) queriedAce.Allow, (object) queriedAce.Deny, (object) queriedAcl.Token, (object) securityNamespace.Description.NamespaceId, (object) fromDescriptorForAces)));
      }))));
    }

    public void InvalidateIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Guid> identityIds)
    {
      requestContext.TraceEnter(80430, "AccountLinking", "IdentityStore", nameof (InvalidateIdentities));
      try
      {
        bool updateIdentityAudit = IdentityStoreUtilities.IdentityAuditEnabled(requestContext);
        int identitySequenceId;
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        {
          identitySequenceId = identityComponent.InvalidateIdentities(updateIdentityAudit, identityIds);
          requestContext.Trace(80431, TraceLevel.Info, "AccountLinking", "IdentityStore", "Sequence id from invalidating identities: {0}", (object) identitySequenceId);
          try
          {
            if (requestContext.IsTracing(80995, TraceLevel.Info, "IdentityService", "IdentityStore"))
            {
              string callStack = requestContext.IsTracing(80996, TraceLevel.Info, "IdentityService", "IdentityStore") ? Environment.StackTrace : string.Empty;
              foreach (Guid identityId in (IEnumerable<Guid>) identityIds)
                IdentityTracing.TraceIdentitySqlInvalidateById(hostDomain, identityId, callStack);
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(80995, "IdentityService", "IdentityStore", ex);
          }
        }
        requestContext.Trace(80432, TraceLevel.Info, "AccountLinking", "IdentityStore", "Processing identity change on author.");
        this.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, identitySequenceId, -1);
      }
      finally
      {
        requestContext.TraceLeave(80439, "AccountLinking", "IdentityStore", nameof (InvalidateIdentities));
      }
    }

    public int UpgradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        return identityComponent.UpgradeIdentitiesToTargetResourceVersion(targetResourceVersion, updateBatchSize, maxNumberOfIdentitiesToUpdate);
    }

    public int DowngradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        return identityComponent.DowngradeIdentitiesToTargetResourceVersion(targetResourceVersion, updateBatchSize, maxNumberOfIdentitiesToUpdate);
    }

    public int UpdateIdentityVsid(IVssRequestContext requestContext, Guid oldVsid, Guid newVsid)
    {
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        return identityComponent.UpdateIdentityVsid(oldVsid, newVsid);
    }

    public int SendMajorDescriptorChangeNotification(IVssRequestContext requestContext)
    {
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        return identityComponent.FireMajorDescriptorChangeNotification();
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByDomainAndOid(
      IVssRequestContext requestContext,
      string domain,
      Guid externalId)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results;
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        results = identityComponent.ReadIdentitiesByDomainAndOid(domain, externalId);
      if (results == null)
        return results;
      this.PostProcessBeforeFilterExtensionsAfterReadIdentitiesFromDatabase(requestContext, results);
      this.PostProcessExtensionsAfterReadIdentitiesFromDatabase(requestContext, QueryMembership.None, QueryMembership.None, results);
      return results;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByDomainAndOidWithLargestSequenceId(
      IVssRequestContext requestContext,
      string domain,
      Guid externalId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Application);
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext1))
        identity = identityComponent.ReadIdentityByDomainAndOidWithLargestSequenceId(domain, externalId);
      if (identity == null)
        return identity;
      this.PostProcessBeforeFilterExtensionsAfterReadIdentitiesFromDatabase(requestContext1, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      });
      this.PostProcessExtensionsAfterReadIdentitiesFromDatabase(requestContext1, QueryMembership.None, QueryMembership.None, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      });
      return identity;
    }

    public bool DeleteHistoricalIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds)
    {
      requestContext.TraceEnter(80550, "IdentityService", "IdentityStore", nameof (DeleteHistoricalIdentities));
      try
      {
        bool flag = false;
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
          flag = identityComponent.DeleteHistoricalIdentities(identityIds);
        if (flag)
          this.m_cache.ProcessChanges(requestContext, (ICollection<Guid>) null, (ICollection<Guid>) identityIds, (ICollection<Guid>) null, (ICollection<MembershipChangeInfo>) null, (ICollection<Guid>) null, (SequenceContext) null);
        return flag;
      }
      finally
      {
        requestContext.TraceLeave(80559, "IdentityService", "IdentityStore", nameof (DeleteHistoricalIdentities));
      }
    }

    public bool SafelyRemoveHistoricalIdentity(
      IVssRequestContext collectionContext,
      Guid identityId,
      IList<IdentityIdTranslation> idTranslation = null)
    {
      collectionContext.TraceEnter(80650, "IdentityService", "IdentityStore", nameof (SafelyRemoveHistoricalIdentity));
      try
      {
        if (idTranslation != null)
        {
          this.UpdateTranslations(collectionContext, idTranslation);
          this.m_cache.OnIdentityIdTranslationChanged(collectionContext, new IdentityIdTranslationChangeData()
          {
            TranslationChangeType = IdentityIdTranslationChangeType.Added
          });
        }
        return this.DeleteHistoricalIdentities(collectionContext, (IList<Guid>) new Guid[1]
        {
          identityId
        });
      }
      finally
      {
        collectionContext.TraceLeave(80659, "IdentityService", "IdentityStore", nameof (SafelyRemoveHistoricalIdentity));
      }
    }

    public virtual Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      bool extendedProperties,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedMembers = false,
      bool bypassCache = false,
      bool filterResults = true)
    {
      return this.ReadIdentities(requestContext, hostDomain, descriptors, queryMembership, extendedProperties, propertyNameFilters, (SequenceContext) null, includeRestrictedMembers, bypassCache, filterResults);
    }

    public virtual Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      bool extendedProperties,
      IEnumerable<string> propertyNameFilters,
      SequenceContext minSequenceContext,
      bool includeRestrictedMembers = false,
      bool bypassCache = false,
      bool filterResults = true)
    {
      if (includeRestrictedMembers)
        bypassCache = true;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      CachedRegistryService service1 = vssRequestContext.GetService<CachedRegistryService>();
      requestContext.RootContext.Items["IdentityMinimumResourceVersion"] = (object) service1.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Identity/Settings/IdentityMinimumResourceVersion", -1);
      requestContext.Trace(80765, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Applying a Resource Version Override : {0}", requestContext.RootContext.Items["IdentityMinimumResourceVersion"]);
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray1 = new Microsoft.VisualStudio.Services.Identity.Identity[descriptors.Count];
      Microsoft.VisualStudio.Services.Identity.Identity[] results = new Microsoft.VisualStudio.Services.Identity.Identity[descriptors.Count];
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityTable = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(descriptors.Count);
      List<IdentityDescriptor> descriptors1 = new List<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) descriptors);
      List<Microsoft.VisualStudio.Services.Identity.Identity> identities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      int incrementBy;
      if (!bypassCache)
      {
        for (int index = 0; index < descriptors.Count; ++index)
        {
          identityArray1[index] = this.m_cache.ReadIdentity(requestContext, hostDomain, descriptors[index], queryMembership);
          if (identityArray1[index] != null)
            descriptors1[index] = (IdentityDescriptor) null;
        }
        this.ReadGroupsFromImsCache(requestContext, hostDomain, descriptors, queryMembership, filterResults, identityArray1);
        for (int index = 0; index < descriptors.Count; ++index)
        {
          if (identityArray1[index] != null)
          {
            this.IncrementCacheHitPerfCounters();
            requestContext.Trace(80046, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Cache hit (ReadIdentities w/descriptor) - descriptor: {0}, queryMembership: {1}", (object) descriptors[index], (object) queryMembership);
          }
          else
          {
            this.IncrementCacheMissPerfCounters();
            requestContext.Trace(80047, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Cache miss (ReadIdentities w/descriptor) - descriptor: {0}, queryMembership: {1}", (object) descriptors[index], (object) queryMembership);
          }
        }
        incrementBy = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1).Count<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null));
      }
      else
      {
        incrementBy = descriptors.Count;
        this.IncrementCacheMissPerfCounters((long) incrementBy);
        requestContext.Trace(80048, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Cache bypassed (ReadIdentites w/descriptors) - host domain: {0}, # of descriptors: {1}, queryMembership: {2}, extended properties: {3}, # of propertyNameFilters: {4}, include restricted members: {5}, bypass cache: {6}, filter results: {7}", (object) hostDomain, (object) descriptors, (object) queryMembership, (object) extendedProperties, (object) propertyNameFilters, (object) includeRestrictedMembers, (object) bypassCache, (object) filterResults);
      }
      if (incrementBy > 0)
      {
        bool flag = requestContext.IsFeatureEnabled(PlatformIdentityStore.s_featureNameUseProviderDisplayName);
        requestContext.RootContext.Items[PlatformIdentityStore.s_featureNameUseProviderDisplayName] = (object) flag;
        Microsoft.VisualStudio.Services.Identity.Identity[] identityArray2 = this.ReadGroupsFromDatabase(requestContext, hostDomain, (IList<IdentityDescriptor>) descriptors1, (IList<Guid>) null, QueryMembership.None, QueryMembership.None, includeRestrictedMembers, false, false);
        for (int index = 0; index < identityArray2.Length; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = identityArray2[index];
          if (identity != null)
          {
            results[index] = identity;
            descriptors1[index] = (IdentityDescriptor) null;
            identityTable[identity.Id] = identity;
            --incrementBy;
          }
        }
        if (incrementBy > 0)
        {
          Microsoft.VisualStudio.Services.Identity.Identity[] identityArray3 = this.ReadIdentitiesFromDatabase(requestContext, hostDomain, (IList<IdentityDescriptor>) descriptors1, (IList<Guid>) null, QueryMembership.None, QueryMembership.None, includeRestrictedMembers, false, false, (SequenceContext) null);
          for (int index = 0; index < identityArray3.Length; ++index)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = identityArray3[index];
            if (identity != null)
            {
              results[index] = identity;
              descriptors1[index] = (IdentityDescriptor) null;
              identityTable[identity.Id] = identity;
              --incrementBy;
            }
          }
        }
        if (identityTable.Count > 0)
        {
          QueryMembership childrenQuery = QueryMembership.None;
          QueryMembership parentsQuery = QueryMembership.None;
          this.ResolveQueryMembership(queryMembership, out childrenQuery, out parentsQuery);
          this.PopulateMembershipAndFilter(requestContext, hostDomain, childrenQuery, parentsQuery, includeRestrictedMembers, false, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results, identityTable, filterResults, minSequenceContext);
          for (int index = 0; index < results.Length; ++index)
          {
            if (results[index] != null)
            {
              identityArray1[index] = results[index];
              identities.Add(identityArray1[index]);
            }
          }
        }
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.ReadAllMembersInScopeOnExtended(requestContext, hostDomain, queryMembership, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1);
      requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new AfterCoreReadIdentitiesEvent()
      {
        ReadIdentities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1,
        ParentQueryMembership = queryMembership,
        ChildQueryMembership = queryMembership,
        IdentitiesInScope = identityList,
        MinSequenceContext = minSequenceContext
      });
      if (!bypassCache)
        this.CacheIdentities(requestContext, identities, hostDomain, queryMembership);
      IIdMapper idMapper = this.m_cache.GetIdMapper(requestContext, hostDomain);
      if (extendedProperties)
        this.ReadExtendedProperties(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1, propertyNameFilters, idMapper);
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
        IVssRequestContext requestContext1 = requestContext;
        AfterReadIdentitiesOnStoreEvent notificationEvent = new AfterReadIdentitiesOnStoreEvent();
        notificationEvent.Identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list;
        notificationEvent.IdentityMapper = idMapper;
        notificationEvent.IdentityDomain = hostDomain;
        service2.PublishDecisionPoint(requestContext1, (object) notificationEvent);
      }
      return list.ToArray();
    }

    public virtual IDictionary<IdentityDescriptor, bool> HasAadGroups(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors)
    {
      return this.m_cache.HasAadGroups(requestContext, descriptors);
    }

    public virtual IDictionary<IdentityDescriptor, IdentityMembershipInfo> ReadIdentityMembershipInfo(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      SequenceContext minSequenceContext)
    {
      return this.m_cache.ReadIdentityMembershipInfo(requestContext, hostDomain, descriptors, minSequenceContext);
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadAllMembersInScopeOnExtended(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> results)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.ExpandedDownFromCache") || queryMembership != QueryMembership.ExpandedDown || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || results == null)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (results.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsContainer && AadIdentityHelper.IsAadGroup(x.Descriptor))) ? 1 : (results.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsContainer && !x.Members.IsNullOrEmpty<IdentityDescriptor>() && x.Members.Any<IdentityDescriptor>(PlatformIdentityStore.\u003C\u003EO.\u003C0\u003E__IsAadGroup ?? (PlatformIdentityStore.\u003C\u003EO.\u003C0\u003E__IsAadGroup = new Func<IdentityDescriptor, bool>(AadIdentityHelper.IsAadGroup))))) ? 1 : 0)) != 0 ? this.ReadIdentitiesInScope(requestContext, hostDomain, hostDomain.DomainId, QueryMembership.None, (IEnumerable<string>) null) : (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
    }

    public virtual Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      bool extendedProperties,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedMembers = false,
      bool bypassCache = false,
      bool filterResults = true)
    {
      return this.ReadIdentities(requestContext, hostDomain, identityIds, queryMembership, extendedProperties, propertyNameFilters, (SequenceContext) null, includeRestrictedMembers, bypassCache, filterResults);
    }

    public virtual Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      bool extendedProperties,
      IEnumerable<string> propertyNameFilters,
      SequenceContext minSequenceContext,
      bool includeRestrictedMembers = false,
      bool bypassCache = false,
      bool filterResults = true)
    {
      if (includeRestrictedMembers)
        bypassCache = true;
      if (requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application) && requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        requestContext.TraceSerializedConditionally(801001, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Caller attempted to ReadIdentities by ID by elevating to organization level. Requested Ids: {0}", (object) identityIds);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      CachedRegistryService service1 = vssRequestContext.GetService<CachedRegistryService>();
      requestContext.RootContext.Items["IdentityMinimumResourceVersion"] = (object) service1.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Identity/Settings/IdentityMinimumResourceVersion", -1);
      requestContext.Trace(80765, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Applying a Resource Version Override : {0}", requestContext.RootContext.Items["IdentityMinimumResourceVersion"]);
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray1 = new Microsoft.VisualStudio.Services.Identity.Identity[identityIds.Count];
      Microsoft.VisualStudio.Services.Identity.Identity[] results = new Microsoft.VisualStudio.Services.Identity.Identity[identityIds.Count];
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityTable = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(identityIds.Count);
      List<Guid> guidList = new List<Guid>((IEnumerable<Guid>) identityIds);
      List<Microsoft.VisualStudio.Services.Identity.Identity> identities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      IIdMapper idMapper = this.m_cache.GetIdMapper(requestContext, hostDomain);
      ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
      TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
      if (executionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        ITeamFoundationEventService foundationEventService = service2;
        IVssRequestContext requestContext1 = requestContext;
        BeforeReadIdentitiesOnStoreEvent notificationEvent = new BeforeReadIdentitiesOnStoreEvent();
        notificationEvent.IdentityIds = (IList<Guid>) guidList;
        notificationEvent.IdentityMapper = idMapper;
        notificationEvent.IdentityDomain = hostDomain;
        foundationEventService.PublishDecisionPoint(requestContext1, (object) notificationEvent);
      }
      int incrementBy;
      if (!bypassCache)
      {
        for (int index = 0; index < guidList.Count; ++index)
        {
          if (guidList[index] != Guid.Empty)
          {
            identityArray1[index] = this.m_cache.ReadIdentity(requestContext, hostDomain, guidList[index], queryMembership);
            if (identityArray1[index] != null)
            {
              this.IncrementCacheHitPerfCounters();
              requestContext.Trace(80039, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Cache hit (ReadIdentities) - identityId: {0}, queryMembership: {1}", (object) guidList[index], (object) queryMembership);
              guidList[index] = Guid.Empty;
            }
            else
            {
              this.IncrementCacheMissPerfCounters();
              requestContext.Trace(80044, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Cache miss (ReadIdentities) - identityId: {0}, queryMembership: {1}", (object) guidList[index], (object) queryMembership);
            }
          }
        }
        this.ReadGroupsFromImsCache(requestContext, hostDomain, guidList, queryMembership, filterResults, identityArray1);
        incrementBy = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1).Count<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null));
      }
      else
      {
        incrementBy = guidList.Count;
        this.IncrementCacheMissPerfCounters((long) incrementBy);
        requestContext.Trace(80045, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Cache bypassed - host domain: {0}, # of identity ids: {1}, querymembership: {2}, extendedproperties: {3}, # of property name filters: {4}, includerestrictedmembers: {5}, bypasscache: {6}, filterResults: {7}", (object) hostDomain, (object) identityIds, (object) queryMembership, (object) extendedProperties, (object) propertyNameFilters, (object) includeRestrictedMembers, (object) bypassCache, (object) filterResults);
      }
      if (incrementBy > 0)
      {
        Microsoft.VisualStudio.Services.Identity.Identity[] identityArray2 = this.ReadGroupsFromDatabase(requestContext, hostDomain, (IList<IdentityDescriptor>) null, (IList<Guid>) guidList, QueryMembership.None, QueryMembership.None, includeRestrictedMembers, false, false);
        for (int index = 0; index < identityArray2.Length; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = identityArray2[index];
          if (identity != null)
          {
            results[index] = identity;
            guidList[index] = Guid.Empty;
            identityTable[identity.Id] = identity;
            --incrementBy;
          }
        }
        if (incrementBy > 0)
        {
          Microsoft.VisualStudio.Services.Identity.Identity[] identityArray3 = this.ReadIdentitiesFromDatabase(requestContext, hostDomain, (IList<IdentityDescriptor>) null, (IList<Guid>) guidList, QueryMembership.None, QueryMembership.None, includeRestrictedMembers, false, false, (SequenceContext) null);
          for (int index = 0; index < identityArray3.Length; ++index)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = identityArray3[index];
            if (identity != null)
            {
              results[index] = identity;
              identityTable[identity.Id] = identity;
              --incrementBy;
            }
          }
        }
        if (identityTable.Count > 0)
        {
          QueryMembership childrenQuery = QueryMembership.None;
          QueryMembership parentsQuery = QueryMembership.None;
          this.ResolveQueryMembership(queryMembership, out childrenQuery, out parentsQuery);
          this.PopulateMembershipAndFilter(requestContext, hostDomain, childrenQuery, parentsQuery, includeRestrictedMembers, false, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results, identityTable, filterResults, minSequenceContext);
          for (int index = 0; index < results.Length; ++index)
          {
            if (results[index] != null)
            {
              identityArray1[index] = results[index];
              identities.Add(identityArray1[index]);
            }
          }
        }
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.ReadAllMembersInScopeOnExtended(requestContext, hostDomain, queryMembership, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1);
        requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new AfterCoreReadIdentitiesEvent()
        {
          ReadIdentities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1,
          ParentQueryMembership = queryMembership,
          ChildQueryMembership = queryMembership,
          IdentitiesInScope = identityList,
          MinSequenceContext = minSequenceContext
        });
        if (!bypassCache)
          this.CacheIdentities(requestContext, identities, hostDomain, queryMembership);
      }
      if (extendedProperties)
        this.ReadExtendedProperties(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1, propertyNameFilters, idMapper);
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      executionEnvironment = requestContext.ExecutionEnvironment;
      if (executionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        ITeamFoundationEventService foundationEventService = service2;
        IVssRequestContext requestContext2 = requestContext;
        AfterReadIdentitiesOnStoreEvent notificationEvent = new AfterReadIdentitiesOnStoreEvent();
        notificationEvent.Identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list;
        notificationEvent.IdentityMapper = idMapper;
        notificationEvent.IdentityDomain = hostDomain;
        foundationEventService.PublishDecisionPoint(requestContext2, (object) notificationEvent);
      }
      return list.ToArray();
    }

    private void ReadGroupsFromImsCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      List<Guid> identityIdList,
      QueryMembership queryMembership,
      bool filterResults,
      Microsoft.VisualStudio.Services.Identity.Identity[] results)
    {
      if (queryMembership != QueryMembership.ExpandedDown || !requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      IEnumerable<Guid> source1 = identityIdList.Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty));
      if (!source1.Any<Guid>())
        return;
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadIdentities(requestContext, hostDomain, (IList<Guid>) source1.ToList<Guid>(), QueryMembership.None, false, (IEnumerable<string>) null, filterResults: filterResults)).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsContainer)).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (y => y.Id), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (y => y));
      if (dictionary.Count <= 0)
        return;
      Dictionary<Guid, ISet<IdentityId>> descendants = requestContext.GetService<IImsCacheService>().GetDescendants(requestContext, (IEnumerable<Guid>) dictionary.Keys);
      if (descendants == null)
        return;
      for (int index = 0; index < identityIdList.Count; ++index)
      {
        if (!(identityIdList[index] == Guid.Empty) && descendants.ContainsKey(identityIdList[index]))
        {
          ISet<IdentityId> source2 = descendants[identityIdList[index]];
          if (source2 != null)
          {
            results[index] = dictionary[identityIdList[index]];
            results[index].MemberIds = (ICollection<Guid>) source2.Select<IdentityId, Guid>((Func<IdentityId, Guid>) (x => x.Id)).ToList<Guid>();
            results[index].Members = (ICollection<IdentityDescriptor>) source2.Select<IdentityId, IdentityDescriptor>((Func<IdentityId, IdentityDescriptor>) (x => x.Descriptor)).ToList<IdentityDescriptor>();
            identityIdList[index] = Guid.Empty;
          }
        }
      }
    }

    private void ReadGroupsFromImsCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      bool filterResults,
      Microsoft.VisualStudio.Services.Identity.Identity[] results)
    {
      if (queryMembership != QueryMembership.ExpandedDown || !requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      IEnumerable<IdentityDescriptor> source1 = descriptors.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null));
      if (!source1.Any<IdentityDescriptor>())
        return;
      Microsoft.VisualStudio.Services.Identity.Identity[] source2 = this.ReadIdentities(requestContext, hostDomain, (IList<IdentityDescriptor>) source1.ToList<IdentityDescriptor>(), QueryMembership.None, false, (IEnumerable<string>) null, filterResults: filterResults);
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary1 = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source2).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsContainer)).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (y => y.Id), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (y => y));
      if (dictionary1.Count <= 0)
        return;
      Dictionary<IdentityDescriptor, Guid> dictionary2 = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source2).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsContainer)).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (y => y.Descriptor), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (y => y.Id), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      Dictionary<Guid, ISet<IdentityId>> descendants = requestContext.GetService<IImsCacheService>().GetDescendants(requestContext, (IEnumerable<Guid>) dictionary1.Keys);
      if (descendants == null)
        return;
      for (int index = 0; index < descriptors.Count; ++index)
      {
        Guid key = dictionary2[descriptors[index]];
        if (!(descriptors[index] == (IdentityDescriptor) null) && descendants.ContainsKey(key))
        {
          ISet<IdentityId> source3 = descendants[key];
          if (source3 != null)
          {
            results[index] = dictionary1[key];
            results[index].MemberIds = (ICollection<Guid>) source3.Select<IdentityId, Guid>((Func<IdentityId, Guid>) (x => x.Id)).ToList<Guid>();
            results[index].Members = (ICollection<IdentityDescriptor>) source3.Select<IdentityId, IdentityDescriptor>((Func<IdentityId, IdentityDescriptor>) (x => x.Descriptor)).ToList<IdentityDescriptor>();
            descriptors[index] = (IdentityDescriptor) null;
          }
        }
      }
    }

    public virtual Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      bool extendedProperties,
      IEnumerable<string> propertyNameFilters)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> results = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> identities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      CachedRegistryService service1 = vssRequestContext.GetService<CachedRegistryService>();
      requestContext.RootContext.Items["IdentityMinimumResourceVersion"] = (object) service1.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Identity/Settings/IdentityMinimumResourceVersion", -1);
      requestContext.Trace(80765, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Applying a Resource Version Override : {0}", requestContext.RootContext.Items["IdentityMinimumResourceVersion"]);
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && searchFactor == IdentitySearchFilter.AccountName)
      {
        if (factorValue.Contains("\\"))
          identity1 = this.m_cache.ReadIdentity(requestContext, hostDomain, searchFactor, factorValue, queryMembership);
      }
      else
        identity1 = this.m_cache.ReadIdentity(requestContext, hostDomain, searchFactor, factorValue, queryMembership);
      TeamFoundationExecutionEnvironment executionEnvironment;
      if (identity1 != null)
      {
        results.Add(identity1);
        this.IncrementCacheHitPerfCounters();
        requestContext.Trace(80040, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Cache hit - search factor: {0}, factor value: {1}, querymembership: {2}", (object) searchFactor, (object) factorValue, (object) queryMembership);
      }
      else
      {
        bool flag = false;
        this.IncrementCacheMissPerfCounters();
        requestContext.Trace(80041, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Cache miss - search factor: {0}, factor value: {1}, querymembership: {2}", (object) searchFactor, (object) factorValue, (object) queryMembership);
        if (searchFactor == IdentitySearchFilter.AccountName || searchFactor == IdentitySearchFilter.DisplayName || searchFactor == IdentitySearchFilter.General)
        {
          Guid scopeId;
          string scopeName;
          string groupName;
          bool recurse;
          if (this.ParseGroupName(requestContext, hostDomain, factorValue, out scopeId, out scopeName, out groupName, out recurse))
          {
            results = this.ReadGroupsFromDatabase(requestContext, hostDomain, scopeId, scopeName, groupName, QueryMembership.None, recurse, false);
            flag = true;
          }
        }
        else if (searchFactor == IdentitySearchFilter.AdministratorsGroup)
        {
          Guid scopeId = this.GetScopeId(requestContext, hostDomain, factorValue, false);
          IdentityDescriptor identityDescriptor = IdentityMapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, scopeId);
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = this.ReadGroupsFromDatabase(requestContext, hostDomain, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            identityDescriptor
          }, (IList<Guid>) null, queryMembership)[0];
          if (identity2 != null)
            results.Add(identity2);
        }
        else if (searchFactor == IdentitySearchFilter.Identifier)
        {
          IdentityDescriptor foundationDescriptor = IdentityHelper.CreateTeamFoundationDescriptor(factorValue);
          Microsoft.VisualStudio.Services.Identity.Identity identity3 = this.ReadGroupsFromDatabase(requestContext, hostDomain, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            foundationDescriptor
          }, (IList<Guid>) null, queryMembership)[0];
          if (identity3 != null)
            results.Add(identity3);
        }
        Guid guid;
        if (searchFactor == IdentitySearchFilter.General || searchFactor == IdentitySearchFilter.AccountName || searchFactor == IdentitySearchFilter.DisplayName || searchFactor == IdentitySearchFilter.MailAddress || searchFactor == IdentitySearchFilter.Alias || searchFactor == IdentitySearchFilter.DirectoryAlias || searchFactor == IdentitySearchFilter.Identifier && results.Count == 0)
        {
          string domain = (string) null;
          string account = (string) null;
          int uniqueUserId = 0;
          if (searchFactor == IdentitySearchFilter.AccountName || searchFactor == IdentitySearchFilter.General)
            PlatformIdentityStore.ParseIdentityName(factorValue, out domain, out account, out uniqueUserId);
          else if (searchFactor == IdentitySearchFilter.DirectoryAlias)
          {
            executionEnvironment = requestContext.ExecutionEnvironment;
            if (executionEnvironment.IsHostedDeployment && (requestContext.ServiceHost.Is(TeamFoundationHostType.Application) || requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection)))
            {
              guid = requestContext.GetOrganizationAadTenantId();
              domain = guid.ToString();
            }
          }
          results.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadIdentitiesFromDatabase(requestContext, hostDomain, searchFactor, factorValue, domain, account, uniqueUserId, QueryMembership.None, false));
          flag = true;
        }
        if (flag)
        {
          Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityTable = (Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) null;
          Microsoft.VisualStudio.Services.Identity.Identity[] results1 = (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
          for (int i = 0; i < results.Count; i++)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity4 = this.m_cache.ReadIdentity(requestContext, hostDomain, results[i].Descriptor, queryMembership);
            if (identity4 != null)
            {
              results[i] = identity4;
              this.IncrementCacheHitPerfCounters();
              requestContext.Trace(80042, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Cache hit (populateAndFilterRequired) - descriptor: {0}, querymembership: {1}", (object) results[i].Descriptor, (object) queryMembership);
            }
            else
            {
              this.IncrementCacheMissPerfCounters();
              requestContext.Trace(80043, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Cache miss (populateAndFilterRequired) - descriptor: {0}, querymembership: {1}", (object) results[i].Descriptor, (object) queryMembership);
              if (identityTable == null)
              {
                identityTable = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(results.Count);
                results1 = new Microsoft.VisualStudio.Services.Identity.Identity[results.Count];
              }
              if (!identityTable.ContainsKey(results[i].Id))
                identityTable.Add(results[i].Id, results[i]);
              else
                requestContext.TraceConditionally(800065, TraceLevel.Error, "IdentityService", "IdentityStore", (Func<string>) (() => "Duplicate key in results identity table with Id: " + results[i].Id.ToString() + " and descriptor: " + results[i].Descriptor?.ToString() + ". The input parameters are hostid: " + requestContext.ServiceHost.InstanceId.ToString() + ", hostDomain: " + hostDomain.DomainId.ToString() + ", searchFactor: " + searchFactor.ToString() + ", factorValue: " + factorValue));
              results1[i] = results[i];
              results[i] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
            }
          }
          if (identityTable != null)
          {
            QueryMembership childrenQuery = QueryMembership.None;
            QueryMembership parentsQuery = QueryMembership.None;
            this.ResolveQueryMembership(queryMembership, out childrenQuery, out parentsQuery);
            this.PopulateMembershipAndFilter(requestContext, hostDomain, childrenQuery, parentsQuery, false, false, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results1, identityTable, true, (SequenceContext) null);
            Microsoft.VisualStudio.Services.Identity.Identity identity5 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
            for (int index = 0; index < results1.Length; ++index)
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity6 = results1[index];
              if (identity6 != null)
              {
                if (identity6.IsActive)
                {
                  results[index] = identity6;
                  identities.Add(identity6);
                }
                else if (identity6.Descriptor.IdentityType == "Microsoft.TeamFoundation.UnauthenticatedIdentity")
                {
                  requestContext.Trace(80071, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Found inactive unauthenticated identity. Factorvalue = {0}; Id = {1}", (object) factorValue, (object) identity6.Id);
                  if (identity5 != null)
                  {
                    guid = identity6.Id;
                    if (guid.CompareTo(identity5.Id) <= 0)
                      continue;
                  }
                  identity5 = identity6;
                }
              }
            }
            if (results.All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null)) && identity5 != null)
            {
              requestContext.Trace(80070, TraceLevel.Info, "IdentityService", "IdentityStore", "Found no active identities. But found an inactive unauthenticated identity, so returning Identity {0} instead.", (object) identity5.Id);
              results[0] = identity5;
            }
          }
        }
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.ReadAllMembersInScopeOnExtended(requestContext, hostDomain, queryMembership, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) results);
        requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new AfterCoreReadIdentitiesEvent()
        {
          ReadIdentities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results,
          ParentQueryMembership = queryMembership,
          ChildQueryMembership = queryMembership,
          IdentitiesInScope = identityList
        });
        this.CacheIdentities(requestContext, identities, hostDomain, queryMembership);
      }
      IIdMapper idMapper = this.m_cache.GetIdMapper(requestContext, hostDomain);
      if (extendedProperties)
        this.ReadExtendedProperties(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results, propertyNameFilters, idMapper);
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = results.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (result => result != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      executionEnvironment = requestContext.ExecutionEnvironment;
      if (executionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
        IVssRequestContext requestContext1 = requestContext;
        AfterReadIdentitiesOnStoreEvent notificationEvent = new AfterReadIdentitiesOnStoreEvent();
        notificationEvent.Identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list;
        notificationEvent.IdentityMapper = idMapper;
        notificationEvent.IdentityDomain = hostDomain;
        service2.PublishDecisionPoint(requestContext1, (object) notificationEvent);
      }
      return list.ToArray();
    }

    public IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesInScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      IEnumerable<string> propertyNameFilters = null)
    {
      requestContext.TraceEnter(1620320, "IdentityService", "IdentityStore", nameof (ReadIdentitiesInScope));
      try
      {
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseRGMV2ForReadIdentitiesByScope"))
          {
            try
            {
              return this.ReadIdentitiesInScopeUsingScopeVisibility(requestContext, hostDomain, scopeId, propertyNameFilters, (ScopePagingContext) null, (Action<int>) null);
            }
            catch (NotImplementedException ex)
            {
              requestContext.TraceException(1620321, "IdentityService", "IdentityStore", (Exception) ex);
              return this.ReadIdentitiesInScopeExpandEveryone(requestContext, hostDomain, scopeId, propertyNameFilters);
            }
          }
        }
        return this.ReadIdentitiesInScopeExpandEveryone(requestContext, hostDomain, scopeId, propertyNameFilters);
      }
      finally
      {
        requestContext.TraceLeave(1620329, "IdentityService", "IdentityStore", nameof (ReadIdentitiesInScope));
      }
    }

    internal override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesInScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      try
      {
        requestContext.TraceEnter(1620330, "IdentityService", "IdentityStore", nameof (ReadIdentitiesInScope));
        IImsCacheService service = requestContext.GetService<IImsCacheService>();
        Dictionary<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>> identitiesInScope = service.GetIdentitiesInScope(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          scopeId
        });
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList;
        if (identitiesInScope?[scopeId] != null)
        {
          identityList = identitiesInScope[scopeId];
          requestContext.Trace(1620331, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Found {0} identities in cache", (object) (identityList != null ? identityList.Count : 0));
        }
        else
        {
          identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>(this.ReadIdentitiesInScope(requestContext, hostDomain, scopeId));
          try
          {
            requestContext.Trace(1620332, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Updating cache with {0} identities", (object) (identityList != null ? identityList.Count : 0));
            service.SetIdentitiesInScope(requestContext, (IEnumerable<KeyValuePair<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>>) new KeyValuePair<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>[1]
            {
              new KeyValuePair<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>(scopeId, identityList)
            });
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1620333, "IdentityService", "IdentityStore", ex);
          }
        }
        if (queryMembership != QueryMembership.None)
        {
          Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = identityList.ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id));
          QueryMembership childrenQuery = QueryMembership.None;
          QueryMembership parentsQuery = QueryMembership.None;
          this.ResolveQueryMembership(queryMembership, out childrenQuery, out parentsQuery);
          this.PopulateMembershipAndFilter(requestContext, this.Domain, childrenQuery, parentsQuery, false, false, identityList, dictionary, false, (SequenceContext) null);
        }
        if (propertyNameFilters != null)
        {
          IIdMapper idMapper = this.m_cache.GetIdMapper(requestContext, hostDomain);
          this.ReadExtendedProperties(requestContext, identityList, propertyNameFilters, idMapper);
        }
        if (identityList != null)
          this.UpdateMegaTenantState(requestContext, identityList.Count);
        return identityList;
      }
      finally
      {
        requestContext.TraceLeave(1620339, "IdentityService", "IdentityStore", nameof (ReadIdentitiesInScope));
      }
    }

    internal IdentitiesPage ReadIdentitiesInScopeByPage(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      ScopePagingContext scopePagingContext)
    {
      try
      {
        requestContext.TraceEnter(1620340, "IdentityService", "IdentityStore", nameof (ReadIdentitiesInScopeByPage));
        ArgumentUtility.CheckForOutOfRange(scopePagingContext.PageSize, "PageSize", 1, 1000);
        ScopePagingContext scopePagingContext1 = new ScopePagingContext(scopePagingContext.ScopeId, scopePagingContext.PageSize + 1, scopePagingContext.IncludeGroups, scopePagingContext.IncludeNonGroups, scopePagingContext.PagenationToken);
        int initialTotalIdentitiesCount = 0;
        IList<Microsoft.VisualStudio.Services.Identity.Identity> list = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadIdentitiesInScopeUsingScopeVisibility(requestContext, hostDomain, scopeId, (IEnumerable<string>) null, scopePagingContext1, (Action<int>) (t => initialTotalIdentitiesCount = t)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        Guid? pagenationToken = new Guid?();
        if (initialTotalIdentitiesCount > scopePagingContext.PageSize)
        {
          list.RemoveAt(list.Count - 1);
          pagenationToken = new Guid?(list.Last<Microsoft.VisualStudio.Services.Identity.Identity>().Id);
        }
        return new IdentitiesPage()
        {
          Identities = list,
          ContinuationToken = scopePagingContext.ToContinuationToken(pagenationToken)
        };
      }
      finally
      {
        requestContext.TraceLeave(1620349, "IdentityService", "IdentityStore", nameof (ReadIdentitiesInScopeByPage));
      }
    }

    public IList<Guid> ReadIdentityIdsInScope(IVssRequestContext requestContext, Guid scopeId)
    {
      try
      {
        requestContext.TraceEnter(1620350, "IdentityService", "IdentityStore", nameof (ReadIdentityIdsInScope));
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        {
          IList<Guid> identityIdsInScope = groupComponent.GetIdentityIdsInScope(scopeId);
          requestContext.Trace(1620351, TraceLevel.Info, "IdentityService", "IdentityStore", string.Format("Read all identity in scope {0} and found {1} identity ids.", (object) scopeId, (object) identityIdsInScope.Count));
          return identityIdsInScope;
        }
      }
      finally
      {
        requestContext.TraceLeave(1620350, "IdentityService", "IdentityStore", nameof (ReadIdentityIdsInScope));
      }
    }

    public override bool IsMember(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      return this.IsMember(requestContext, hostDomain, groupDescriptor, memberDescriptor, false);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility)
    {
      throw new NotImplementedException();
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>(socialDescriptors.Count);
      for (int index = 0; index < socialDescriptors.Count; ++index)
      {
        SocialDescriptor socialDescriptor = socialDescriptors[index];
        if (socialDescriptor == new SocialDescriptor())
        {
          identityList.Add((Microsoft.VisualStudio.Services.Identity.Identity) null);
        }
        else
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentity(requestContext, hostDomain, socialDescriptor, false);
          identityList.Add(identity);
        }
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityList;
    }

    public override Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      SocialDescriptor socialDescriptor,
      bool bypassCache = false)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (!bypassCache)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = this.m_cache.ReadIdentity(requestContext, hostDomain, socialDescriptor);
        if (identity2 != null)
          return identity2;
      }
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        identity1 = identityComponent.ReadSocialIdentity(socialDescriptor);
      if (identity1 != null)
      {
        IVssRequestContext requestContext1 = requestContext;
        List<Microsoft.VisualStudio.Services.Identity.Identity> identities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        identities.Add(identity1);
        IdentityDomain hostDomain1 = hostDomain;
        this.CacheIdentities(requestContext1, identities, hostDomain1, QueryMembership.None);
      }
      return identity1;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadIdentities(requestContext, hostDomain, identityIds, queryMembership, false, propertyNameFilters, includeRestrictedVisibility);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadIdentities(requestContext, hostDomain, descriptors, queryMembership, true, propertyNameFilters, (SequenceContext) null, includeRestrictedVisibility);
    }

    internal IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadAggregateIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> aggregateIdentityDescriptors)
    {
      foreach (IdentityDescriptor identityDescriptor in (IEnumerable<IdentityDescriptor>) aggregateIdentityDescriptors)
      {
        if (!identityDescriptor.IsAggregateIdentityType())
          throw new ArgumentException(string.Format("Identity decriptor '{0}' is not of type '{1}'", (object) identityDescriptor, (object) "Microsoft.TeamFoundation.AggregateIdentity"));
      }
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[aggregateIdentityDescriptors.Count];
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
      {
        using (ResultCollection resultCollection = identityComponent.ReadIdentities((IEnumerable<IdentityDescriptor>) aggregateIdentityDescriptors, (IEnumerable<Guid>) null))
        {
          foreach (IdentityManagementComponent.IdentityData identityData in resultCollection.GetCurrent<IdentityManagementComponent.IdentityData>())
            identityArray[identityData.OrderId] = identityData.Identity;
        }
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity[] ReadGroupsFromDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> groupIdentityDescriptors)
    {
      List<string> list = groupIdentityDescriptors != null ? groupIdentityDescriptors.Select<IdentityDescriptor, string>((Func<IdentityDescriptor, string>) (descriptor => !(descriptor != (IdentityDescriptor) null) || !IdentityValidation.IsTeamFoundationType(descriptor) ? (string) null : descriptor.Identifier)).ToList<string>() : (List<string>) null;
      // ISSUE: explicit non-virtual call
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[list != null ? __nonvirtual (list.Count) : list.Count];
      if (list != null && list.Any<string>((Func<string, bool>) (s => s != null)))
      {
        bool useXtpProc = requestContext.IsFeatureEnabled(PlatformIdentityStore.EnableUseReadGroupsXtpProcFeatureFlag);
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        {
          using (ResultCollection resultCollection = groupComponent.ReadGroups(hostDomain.DomainId, (IEnumerable<string>) list, Enumerable.Empty<Guid>(), useXtpProc))
          {
            foreach (GroupComponent.GroupIdentityData groupIdentityData in resultCollection.GetCurrent<GroupComponent.GroupIdentityData>())
              identityArray[groupIdentityData.OrderId] = groupIdentityData.Identity;
          }
        }
      }
      return identityArray;
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      IList<Guid> identityIds,
      QueryMembership QueryMembership)
    {
      return this.ReadIdentitiesFromDatabase(requestContext, hostDomain, descriptors, identityIds, QueryMembership, QueryMembership, false, false, true, (SequenceContext) null);
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityFromDatabase(
      IVssRequestContext requestContext,
      IdentityDomain domain,
      IdentityDescriptor descriptor,
      QueryMembership parentQuery,
      QueryMembership childQuery,
      bool includeRestrictedMembers,
      bool includeInactivatedMembers,
      bool filterResults,
      SequenceContext minSequenceContext)
    {
      return IdentityValidation.IsTeamFoundationType(descriptor) ? this.ReadGroupsFromDatabase(requestContext, domain, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, (IList<Guid>) null, parentQuery, childQuery, (includeRestrictedMembers ? 1 : 0) != 0, (includeInactivatedMembers ? 1 : 0) != 0, (filterResults ? 1 : 0) != 0, minSequenceContext)[0] : this.ReadIdentitiesFromDatabase(requestContext, domain, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, (IList<Guid>) null, parentQuery, childQuery, (includeRestrictedMembers ? 1 : 0) != 0, (includeInactivatedMembers ? 1 : 0) != 0, (filterResults ? 1 : 0) != 0, minSequenceContext)[0];
    }

    Microsoft.VisualStudio.Services.Identity.Identity[] IIdentityReader.ReadIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      IList<Guid> identityIds,
      QueryMembership parentQuery,
      QueryMembership childQuery,
      bool includeRestrictedMembers,
      bool includeInactivatedMembers,
      bool filterResults)
    {
      return this.ReadIdentitiesFromDatabase(requestContext, hostDomain, descriptors, identityIds, parentQuery, childQuery, includeRestrictedMembers, includeInactivatedMembers, filterResults, (SequenceContext) null);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      IList<Guid> identityIds,
      QueryMembership parentQuery,
      QueryMembership childQuery,
      bool includeRestrictedMembers,
      bool includeInactivatedMembers,
      bool filterResults = true,
      SequenceContext minSequenceContext = null)
    {
      int capacity = descriptors != null ? descriptors.Count : identityIds.Count;
      Microsoft.VisualStudio.Services.Identity.Identity[] results = new Microsoft.VisualStudio.Services.Identity.Identity[capacity];
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityTable = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(capacity);
      IList<IdentityDescriptor> descriptorList = (IList<IdentityDescriptor>) null;
      IList<IdentityDescriptor> descriptors1 = (IList<IdentityDescriptor>) null;
      IList<Guid> identityList1 = (IList<Guid>) null;
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < capacity; ++index)
      {
        if (descriptors != null)
        {
          switch (PlatformIdentityStore.GetIdentityDatabaseType(requestContext, descriptors[index]))
          {
            case IdentityDatabaseTypes.SPS:
              descriptorList = (IList<IdentityDescriptor>) ((object) descriptorList ?? (object) new IdentityDescriptor[capacity]);
              descriptorList[index] = descriptors[index];
              flag1 = true;
              continue;
            case IdentityDatabaseTypes.UserService:
              descriptors1 = (IList<IdentityDescriptor>) ((object) descriptors1 ?? (object) new IdentityDescriptor[capacity]);
              descriptors1[index] = descriptors[index];
              flag2 = true;
              continue;
            default:
              continue;
          }
        }
        else if (PlatformIdentityStore.GetIdentityDatabaseType(requestContext, identityIds[index]).HasFlag((Enum) IdentityDatabaseTypes.SPS))
        {
          identityList1 = (IList<Guid>) ((object) identityList1 ?? (object) new Guid[capacity]);
          identityList1[index] = identityIds[index];
          flag1 = true;
        }
      }
      if ((descriptors != null ? descriptors.Count<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (descriptor => descriptor != (IdentityDescriptor) null)) : identityIds.Count<Guid>((Func<Guid, bool>) (identityId => identityId != Guid.Empty))) > 0 & flag1)
      {
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
          ReadIdentitiesFromDatabase(identityComponent, descriptorList, identityList1);
      }
      if (flag2 && descriptors1 != null && descriptors1.Count > 0)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, descriptors1, QueryMembership.None, (IEnumerable<string>) null);
        for (int index = 0; index < descriptors1.Count; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = identityList2[index];
          if (identity != null)
          {
            identityTable.Add(identity.Id, identity);
            results[index] = identity;
          }
        }
      }
      this.PostProcessBeforeFilterExtensionsAfterReadIdentitiesFromDatabase(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results);
      this.ResolveQueryMembership(childQuery, parentQuery, out childQuery, out parentQuery);
      this.PopulateMembershipAndFilter(requestContext, hostDomain, childQuery, parentQuery, includeRestrictedMembers, includeInactivatedMembers, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results, identityTable, filterResults, minSequenceContext);
      this.PostProcessExtensionsAfterReadIdentitiesFromDatabase(requestContext, parentQuery, childQuery, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results);
      try
      {
        if (requestContext.IsTracing(80994, TraceLevel.Info, "IdentityService", "IdentityStore"))
        {
          string callStack = requestContext.IsTracing(80996, TraceLevel.Info, "IdentityService", "IdentityStore") ? Environment.StackTrace : string.Empty;
          if (descriptors != null)
          {
            foreach (IdentityDescriptor descriptor in (IEnumerable<IdentityDescriptor>) descriptors)
              IdentityTracing.TraceReadIdentityFromDatabase(hostDomain, descriptor, parentQuery, childQuery, includeRestrictedMembers, includeInactivatedMembers, filterResults, callStack);
          }
          if (identityIds != null)
          {
            Microsoft.VisualStudio.Services.Identity.Identity[] source = results;
            Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dedupedDictionary = source != null ? ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).ToDedupedDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x)) : (Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) null;
            foreach (Guid identityId in (IEnumerable<Guid>) identityIds)
            {
              Microsoft.VisualStudio.Services.Identity.Identity valueOrDefault = dedupedDictionary.GetValueOrDefault<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(identityId, (Microsoft.VisualStudio.Services.Identity.Identity) null);
              IdentityTracing.TraceReadIdentityFromDatabase(hostDomain, identityId, parentQuery, childQuery, includeRestrictedMembers, includeInactivatedMembers, filterResults, valueOrDefault, callStack);
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(80994, "IdentityService", "IdentityStore", ex);
      }
      PlatformIdentityStore.CheckForFallbackIdentityRead(requestContext, hostDomain, descriptors, identityIds, parentQuery, childQuery, includeRestrictedMembers, includeInactivatedMembers, filterResults, minSequenceContext, results);
      return results;

      int ReadIdentitiesFromDatabase(
        IdentityManagementComponent identityComponent,
        IList<IdentityDescriptor> descriptorList,
        IList<Guid> identityList)
      {
        int num = 0;
        using (ResultCollection resultCollection = identityComponent.ReadIdentities((IEnumerable<IdentityDescriptor>) descriptorList, (IEnumerable<Guid>) identityList))
        {
          foreach (IdentityManagementComponent.IdentityData identityData in resultCollection.GetCurrent<IdentityManagementComponent.IdentityData>())
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (identityTable.TryGetValue(identityData.Identity.Id, out identity))
            {
              results[identityData.OrderId] = identity;
            }
            else
            {
              identityTable.Add(identityData.Identity.Id, identityData.Identity);
              results[identityData.OrderId] = identityData.Identity;
            }
            Microsoft.VisualStudio.Services.Identity.Identity result = results[identityData.OrderId];
            if (result != null)
            {
              IdentityDescriptor descriptor = result.Descriptor;
            }
            ++num;
          }
        }
        return num;
      }
    }

    private static void CheckForFallbackIdentityRead(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      IList<Guid> identityIds,
      QueryMembership parentQuery,
      QueryMembership childQuery,
      bool includeRestrictedMembers,
      bool includeInactivatedMembers,
      bool filterResults,
      SequenceContext minSequenceContext,
      Microsoft.VisualStudio.Services.Identity.Identity[] results)
    {
      requestContext.TraceEnter(6060910, "IdentityService", "IdentityStore", nameof (CheckForFallbackIdentityRead));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.RootContext.IsDeploymentFallbackIdentityReadAllowed())
          return;
        Guid readAttemptId = Guid.NewGuid();
        IList<object> keys = descriptors != null ? (IList<object>) descriptors.Cast<object>().ToList<object>() : (IList<object>) identityIds.Cast<object>().ToList<object>();
        requestContext.TraceDataConditionally(6060911, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Fallback identity read", (Func<object>) (() => (object) new
        {
          readAttemptId = readAttemptId,
          DomainId = hostDomain.DomainId,
          count = keys.Count,
          parentQuery = parentQuery,
          childQuery = childQuery,
          includeRestrictedMembers = includeRestrictedMembers,
          includeInactivatedMembers = includeInactivatedMembers,
          filterResults = filterResults,
          minSequenceContext = minSequenceContext
        }), nameof (CheckForFallbackIdentityRead));
        for (int i = 0; i < Math.Max(keys.Count, results.Length); i++)
        {
          object inputKey = keys.Count > i ? keys[i] : (object) false;
          object outputResult = results.Length > i ? (object) results[i] : (object) "no result, index out of bounds";
          requestContext.TraceDataConditionally(6060911, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Fallback identity read returned result", (Func<object>) (() => (object) new
          {
            readAttemptId = readAttemptId,
            resultIndex = i,
            inputKey = inputKey,
            outputResult = outputResult
          }), nameof (CheckForFallbackIdentityRead));
        }
        requestContext.Trace(6060911, TraceLevel.Verbose, "IdentityService", "IdentityStore", string.Format("{0}:{1}, {2}:{3}", (object) "readAttemptId", (object) readAttemptId, (object) "StackTrace", (object) Environment.StackTrace));
        if (descriptors != null && descriptors.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (descriptor => descriptor != (IdentityDescriptor) null && !ServicePrincipals.IsServicePrincipal(requestContext, descriptor))).ToList<IdentityDescriptor>().Count != 0)
          requestContext.TraceDataConditionally(6060912, TraceLevel.Verbose, "IdentityService", "IdentityStore", string.Format("Attempted to read deployment-level identities from a non-deployment level context (by descriptor). See tracepoint {0} for {1}:{2} for more details", (object) 6060912, (object) "readAttemptId", (object) readAttemptId), methodName: nameof (CheckForFallbackIdentityRead));
        if (identityIds != null && identityIds.Where<Guid>((Func<Guid, bool>) (identityId => !ServicePrincipals.IsInternalServicePrincipalId(identityId))).ToList<Guid>().Count != 0)
          requestContext.TraceDataConditionally(6060912, TraceLevel.Verbose, "IdentityService", "IdentityStore", string.Format("Attempted to read deployment-level identities from a non-deployment level context (by ID). See tracepoint {0} for {1}:{2} for more details", (object) 6060912, (object) "readAttemptId", (object) readAttemptId), methodName: nameof (CheckForFallbackIdentityRead));
        if (((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) results).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (result => result != null && !ServicePrincipals.IsServicePrincipal(requestContext, result.Descriptor))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>().Count == 0)
          return;
        requestContext.TraceDataConditionally(6060912, TraceLevel.Verbose, "IdentityService", "IdentityStore", string.Format("Attempted to read deployment-level identities from a non-deployment level context (by descriptor or ID). See tracepoint {0} for {1}:{2} for more details", (object) 6060912, (object) "readAttemptId", (object) readAttemptId), methodName: nameof (CheckForFallbackIdentityRead));
      }
      catch (Exception ex) when (!(ex is FallbackIdentityOperationNotAllowedException))
      {
        requestContext.TraceException(6060913, "IdentityService", "IdentityStore", ex);
      }
      finally
      {
        requestContext.TraceLeave(6060910, "IdentityService", "IdentityStore", nameof (CheckForFallbackIdentityRead));
      }
    }

    public IList<Guid> GetIdentityIdsByDomainIdFromDatabase(
      IVssRequestContext requestContext,
      byte[] typeIds,
      Guid domainId)
    {
      List<Guid> domainIdFromDatabase = new List<Guid>();
      using (IdentityManagementComponent component = requestContext.CreateComponent<IdentityManagementComponent>())
      {
        byte typeIdFromName1 = IdentityTypeMapper.Instance.GetTypeIdFromName("Microsoft.TeamFoundation.BindPendingIdentity");
        byte typeIdFromName2 = IdentityTypeMapper.Instance.GetTypeIdFromName("Microsoft.IdentityModel.Claims.ClaimsIdentity");
        foreach (byte typeId in typeIds)
        {
          if ((int) typeId == (int) typeIdFromName1)
          {
            domainIdFromDatabase.AddRange((IEnumerable<Guid>) component.GetIdentityIdsByTypeIdAndPartialSid(typeId, "upn:" + (object) domainId));
          }
          else
          {
            if ((int) typeId != (int) typeIdFromName2)
              throw new IdentityInvalidTypeIdException("Only type BindPendingIdentity and ClaimsIdentity are supported right now");
            domainIdFromDatabase.AddRange((IEnumerable<Guid>) component.GetIdentityIdsByTypeIdAndPartialSid(typeId, domainId.ToString()));
          }
        }
      }
      return (IList<Guid>) domainIdFromDatabase;
    }

    public IList<Guid> GetUserIdentityIdsByDomainFromDatabase(
      IVssRequestContext requestContext,
      Guid? domain)
    {
      List<Guid> domainFromDatabase = new List<Guid>();
      using (IdentityManagementComponent component = requestContext.CreateComponent<IdentityManagementComponent>())
      {
        byte typeIdFromName1 = IdentityTypeMapper.Instance.GetTypeIdFromName("Microsoft.TeamFoundation.BindPendingIdentity");
        byte typeIdFromName2 = IdentityTypeMapper.Instance.GetTypeIdFromName("Microsoft.IdentityModel.Claims.ClaimsIdentity");
        if (domain.HasValue)
        {
          domainFromDatabase.AddRange((IEnumerable<Guid>) component.GetIdentityIdsByTypeIdAndPartialSid(typeIdFromName1, string.Format("{0}{1}{2}", (object) "upn:", (object) domain, (object) '\\')));
          domainFromDatabase.AddRange((IEnumerable<Guid>) component.GetIdentityIdsByTypeIdAndPartialSid(typeIdFromName2, string.Format("{0}{1}", (object) domain, (object) '\\')));
        }
        else
        {
          domainFromDatabase.AddRange((IEnumerable<Guid>) component.GetIdentityIdsByTypeIdAndPartialSid(typeIdFromName1, string.Format("{0}{1}{2}", (object) "upn:", (object) "Windows Live ID", (object) '\\')));
          domainFromDatabase.AddRange((IEnumerable<Guid>) component.GetIdentityIdsByTypeIdAndPartialSid(typeIdFromName2, "0_______________@Live.com", false));
        }
      }
      return (IList<Guid>) domainFromDatabase;
    }

    private void PostProcessExtensionsAfterReadIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      QueryMembership parentQuery,
      QueryMembership childQuery,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results)
    {
      foreach (IReadIdentitiesFromDatabaseExtension databaseExtension in (IEnumerable<IReadIdentitiesFromDatabaseExtension>) this.m_readIdentitiesFromDatabaseExtensions)
        databaseExtension.PostProcess(requestContext, results, parentQuery, childQuery);
    }

    private void PostProcessBeforeFilterExtensionsAfterReadIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results)
    {
      foreach (IReadIdentitiesFromDatabaseExtension databaseExtension in (IEnumerable<IReadIdentitiesFromDatabaseExtension>) this.m_readIdentitiesFromDatabaseExtensions)
        databaseExtension.PostProcessBeforeFilter(requestContext, results);
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesInScopeExpandEveryone(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      IEnumerable<string> propertyNameFilters = null)
    {
      List<IdentityDescriptor> descriptors = new List<IdentityDescriptor>(1)
      {
        IdentityMapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup, scopeId)
      };
      Microsoft.VisualStudio.Services.Identity.Identity rootGroup = this.ReadGroupsFromDatabase(requestContext, hostDomain, (IList<IdentityDescriptor>) descriptors, (IList<Guid>) null, QueryMembership.None, QueryMembership.Expanded, true, false)[0];
      if (rootGroup != null)
      {
        yield return rootGroup;
        IEnumerable<IdentityDescriptor> descriptorsPage = (IEnumerable<IdentityDescriptor>) rootGroup.Members;
        int count = rootGroup.Members.Count;
        for (int i = 0; i < count; i += 1000)
        {
          Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = this.ReadIdentities(requestContext, hostDomain, (IList<IdentityDescriptor>) descriptorsPage.Take<IdentityDescriptor>(1000).ToList<IdentityDescriptor>(), QueryMembership.None, propertyNameFilters != null, propertyNameFilters, bypassCache: true, filterResults: false);
          for (int index = 0; index < identityArray.Length; ++index)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = identityArray[index];
            if (identity != null)
              yield return identity;
          }
          identityArray = (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
          if (i + 1000 < count)
            descriptorsPage = descriptorsPage.Skip<IdentityDescriptor>(1000);
        }
        descriptorsPage = (IEnumerable<IdentityDescriptor>) null;
      }
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesInScopeUsingScopeVisibility(
      IVssRequestContext requestContext,
      IdentityDomain identityDomain,
      Guid scopeId,
      IEnumerable<string> propertyNameFilters,
      ScopePagingContext scopePagingContext,
      Action<int> setInitialTotalIdentities)
    {
      requestContext.TraceEnter(1620310, "IdentityService", "IdentityStore", nameof (ReadIdentitiesInScopeUsingScopeVisibility));
      bool flag;
      try
      {
        IList<Guid> source = (IList<Guid>) null;
        if (scopePagingContext == null)
        {
          using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
            source = groupComponent.GetIdentityIdsVisibleInScope(scopeId);
        }
        else
        {
          using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
            source = groupComponent.GetIdentityIdsVisibleInScopeByPage(scopeId, scopePagingContext.PagenationToken, scopePagingContext.PageSize, scopePagingContext.IncludeGroups, scopePagingContext.IncludeNonGroups);
        }
        int count = source != null ? source.Count : 0;
        requestContext.Trace(1620311, TraceLevel.Verbose, "IdentityService", "IdentityStore", string.Format("Found {0} identities in scope {1} using GroupScopeVisibility", (object) count, (object) scopeId));
        Action<int> action = setInitialTotalIdentities;
        if (action != null)
          action(count);
        if (count == 0)
        {
          flag = false;
        }
        else
        {
          foreach (IList<Guid> identityBatch in source.Batch<Guid>(1000))
          {
            Microsoft.VisualStudio.Services.Identity.Identity[] identities = this.ReadIdentities(requestContext, identityDomain, identityBatch, QueryMembership.None, propertyNameFilters != null, propertyNameFilters, bypassCache: true, filterResults: false);
            Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = identities;
            if ((identityArray != null ? identityArray.Length : 0) > 0)
            {
              for (int identityCounter = 0; identityCounter < identities.Length; ++identityCounter)
              {
                Microsoft.VisualStudio.Services.Identity.Identity identity = identities[identityCounter];
                if (identity == null)
                  requestContext.Trace(1620312, TraceLevel.Verbose, "IdentityService", "IdentityStore", string.Format("IdentityService returned null for with id {0} not found", (object) identityBatch[identityCounter]));
                else
                  yield return identity;
              }
            }
            identities = (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
          }
          flag = false;
        }
      }
      finally
      {
        requestContext.TraceLeave(1620319, "IdentityService", "IdentityStore", nameof (ReadIdentitiesInScopeUsingScopeVisibility));
      }
      return flag;
    }

    internal List<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentitySearchFilter searchFactor,
      string factorValue,
      string domain,
      string account,
      int uniqueUserId,
      QueryMembership queryMembership,
      bool filterResults = true)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !requestContext.RootContext.IsDeploymentFallbackIdentityReadAllowed())
      {
        Guid readAttemptId = Guid.NewGuid();
        requestContext.TraceDataConditionally(6060914, TraceLevel.Error, "IdentityService", "IdentityStore", "Fallback identity read", (Func<object>) (() => (object) new
        {
          readAttemptId = readAttemptId,
          DomainId = hostDomain.DomainId,
          searchFactor = searchFactor,
          factorValue = factorValue,
          domain = domain,
          account = account,
          uniqueUserId = uniqueUserId,
          queryMembership = queryMembership,
          filterResults = filterResults
        }), nameof (ReadIdentitiesFromDatabase));
        requestContext.Trace(6060915, TraceLevel.Error, "IdentityService", "IdentityStore", string.Format("{0}:{1}, {2}:{3}", (object) "readAttemptId", (object) readAttemptId, (object) "StackTrace", (object) Environment.StackTrace));
      }
      HashSet<Microsoft.VisualStudio.Services.Identity.Identity> source = new HashSet<Microsoft.VisualStudio.Services.Identity.Identity>();
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityTable = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      if (PlatformIdentityStore.GetIdentityDatabaseTypes(requestContext, (IEnumerable<Guid>) null).HasFlag((Enum) IdentityDatabaseTypes.SPS))
      {
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        {
          using (ResultCollection resultCollection = identityComponent.ReadIdentity(searchFactor, factorValue, domain, account, uniqueUserId, new bool?()))
          {
            foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>())
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity2 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
              if (identityTable.TryGetValue(identity1.Id, out identity2))
              {
                requestContext.TraceSerializedConditionally(80791, TraceLevel.Warning, "IdentityService", "IdentityStore", "Multiple results found with same identity ID => {0} {1}", (object) identity1, (object) identity2);
              }
              else
              {
                identityTable.Add(identity1.Id, identity1);
                source.Add(identity1);
              }
            }
          }
        }
      }
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = source.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.PostProcessBeforeFilterExtensionsAfterReadIdentitiesFromDatabase(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list);
      QueryMembership childrenQuery = QueryMembership.None;
      QueryMembership parentsQuery = QueryMembership.None;
      this.ResolveQueryMembership(queryMembership, out childrenQuery, out parentsQuery);
      this.PopulateMembershipAndFilter(requestContext, hostDomain, childrenQuery, parentsQuery, false, false, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list, identityTable, filterResults, (SequenceContext) null);
      this.PostProcessExtensionsAfterReadIdentitiesFromDatabase(requestContext, parentsQuery, childrenQuery, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list);
      try
      {
        if (requestContext.IsTracing(80994, TraceLevel.Info, "IdentityService", "IdentityStore"))
        {
          string callStack = requestContext.IsTracing(80996, TraceLevel.Info, "IdentityService", "IdentityStore") ? Environment.StackTrace : string.Empty;
          IdentityTracing.TraceReadIdentityFromDatabase(hostDomain, searchFactor, factorValue, domain, account, uniqueUserId, queryMembership, filterResults, list.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id)), callStack);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(80994, "IdentityService", "IdentityStore", ex);
      }
      return list;
    }

    private void ResolveQueryMembership(
      QueryMembership queryMembership,
      out QueryMembership childrenQuery,
      out QueryMembership parentsQuery)
    {
      switch (queryMembership)
      {
        case QueryMembership.None:
          parentsQuery = QueryMembership.None;
          childrenQuery = QueryMembership.None;
          break;
        case QueryMembership.Direct:
          parentsQuery = QueryMembership.Direct;
          childrenQuery = QueryMembership.Direct;
          break;
        case QueryMembership.Expanded:
          parentsQuery = QueryMembership.Expanded;
          childrenQuery = QueryMembership.Expanded;
          break;
        case QueryMembership.ExpandedUp:
          parentsQuery = QueryMembership.Expanded;
          childrenQuery = QueryMembership.None;
          break;
        case QueryMembership.ExpandedDown:
          parentsQuery = QueryMembership.None;
          childrenQuery = QueryMembership.Expanded;
          break;
        default:
          parentsQuery = QueryMembership.None;
          childrenQuery = QueryMembership.None;
          break;
      }
    }

    private void ResolveQueryMembership(
      QueryMembership childQuery,
      QueryMembership parentQuery,
      out QueryMembership childrenQuery,
      out QueryMembership parentsQuery)
    {
      if (childQuery > QueryMembership.Expanded || parentQuery > QueryMembership.Expanded)
      {
        QueryMembership queryMembership = QueryMembership.None;
        if (childQuery == parentQuery || parentQuery == QueryMembership.None)
          queryMembership = childQuery;
        else if (childQuery == QueryMembership.None)
          queryMembership = parentQuery;
        this.ResolveQueryMembership(queryMembership, out childrenQuery, out parentsQuery);
      }
      else
      {
        childrenQuery = childQuery;
        parentsQuery = parentQuery;
      }
    }

    internal FilteredIdentitiesList ReadFilteredIdentities(
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
      if (suggestedPageSize <= 0)
        throw new ArgumentOutOfRangeException(nameof (suggestedPageSize));
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> unfilteredIdentities = descriptors == null ? this.ReadIdentitiesInScope(requestContext, hostDomain, scopeId) : (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadIdentities(requestContext, hostDomain, descriptors, QueryMembership.None, false, (IEnumerable<string>) null, bypassCache: true);
      FilteredIdentitiesList filteredIdentitiesList = IdentityFilterHelper.FilterIdentities(requestContext, hostDomain, unfilteredIdentities, filters, suggestedPageSize, lastSearchResult, lookForward);
      if (queryMembership != QueryMembership.None)
      {
        Microsoft.VisualStudio.Services.Identity.Identity[] items = filteredIdentitiesList.Items;
        Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityTable = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
        for (int index = 0; index < items.Length; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          if (identityTable.TryGetValue(items[index].Id, out identity))
            items[index] = identity;
          else
            identityTable.Add(items[index].Id, identity);
        }
        QueryMembership childrenQuery = QueryMembership.None;
        QueryMembership parentsQuery = QueryMembership.None;
        this.ResolveQueryMembership(queryMembership, out childrenQuery, out parentsQuery);
        this.PopulateMembershipAndFilter(requestContext, hostDomain, childrenQuery, parentsQuery, false, false, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) items, identityTable, false, (SequenceContext) null);
      }
      IIdMapper idMapper = this.m_cache.GetIdMapper(requestContext, hostDomain);
      this.ReadExtendedProperties(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) filteredIdentitiesList.Items, propertyNameFilters, idMapper);
      return filteredIdentitiesList;
    }

    internal bool IsIdentityCached(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
      requestContext.RootContext.Items["IdentityMinimumResourceVersion"] = (object) service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Identity/Settings/IdentityMinimumResourceVersion", -1);
      requestContext.Trace(80765, TraceLevel.Verbose, "IdentityService", "IdentityStore", "Applying a Resource Version Override : {0}", requestContext.RootContext.Items["IdentityMinimumResourceVersion"]);
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentitiesFromDatabase(requestContext, hostDomain, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
      {
        sourceIdentity.Descriptor
      }, (IList<Guid>) null, QueryMembership.Direct)[0];
      return identity != null && new HashSet<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) sourceIdentity.Members, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance).SetEquals((IEnumerable<IdentityDescriptor>) new HashSet<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) identity.Members, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance));
    }

    public int GetCurrentSequenceId(IVssRequestContext requestContext) => (int) this.GetCurrentIdentitySequenceId(requestContext);

    protected override SequenceContext GetLatestSequenceContext(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain)
    {
      return new SequenceContext(this.GetCurrentIdentitySequenceId(requestContext), this.GetCurrentGroupSequenceId(requestContext), this.GetCurrentOrgIdentitySequenceId(requestContext));
    }

    internal SequenceContext GetCurrentSequenceContext(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain)
    {
      int identitySequenceId = checked ((int) this.GetCurrentOrgIdentitySequenceId(requestContext));
      return new SequenceContext((long) identitySequenceId, this.GetCurrentGroupSequenceId(requestContext), (long) identitySequenceId);
    }

    internal long GetCurrentIdentitySequenceId(IVssRequestContext requestContext)
    {
      int lastSequenceId = 0;
      long firstAuditSequenceId = IdentityStoreUtilities.IdentityAuditFirstSequenceId(requestContext);
      bool useIdentityAudit = IdentityStoreUtilities.IdentityAuditEnabled(requestContext);
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
      {
        using (identityComponent.GetChanges(int.MaxValue, ref lastSequenceId, firstAuditSequenceId, useIdentityAudit))
          ;
      }
      return (long) lastSequenceId;
    }

    internal long GetCurrentOrgIdentitySequenceId(IVssRequestContext requestContext)
    {
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        return identityComponent.GetLatestIdentitySequenceId(true);
    }

    public IdentityChanges GetIdentityChanges(
      IVssRequestContext requestContext,
      int sequenceId,
      long identitySequenceId = -1,
      long groupSequenceId = -1,
      long organizationIdentitySequenceId = -1)
    {
      if (identitySequenceId < 0L)
        identitySequenceId = (long) sequenceId;
      if (groupSequenceId < 0L)
        groupSequenceId = (long) sequenceId;
      if (organizationIdentitySequenceId < 0L)
        organizationIdentitySequenceId = (long) sequenceId;
      IdentityChanges identityChanges = new IdentityChanges();
      int lastSequenceId1 = 0;
      long lastSequenceId2 = 0;
      long lastSequenceId3 = 0;
      long lastSequenceId4 = 0;
      long firstAuditSequenceId = IdentityStoreUtilities.IdentityAuditFirstSequenceId(requestContext);
      bool useIdentityAudit = IdentityStoreUtilities.IdentityAuditEnabled(requestContext);
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.ServiceBusDuplicateDataFix"))
      {
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        {
          List<IdentityDescriptorChange> items;
          using (ResultCollection changes = identityComponent.GetChanges((int) identitySequenceId, ref lastSequenceId1, firstAuditSequenceId, useIdentityAudit, includeDescriptorChanges: true))
          {
            identityChanges.IdentityChangeIds = changes.GetCurrent<Guid>().Items;
            changes.NextResult();
            items = changes.GetCurrent<IdentityDescriptorChange>().Items;
          }
          if (items != null)
          {
            if (items.Any<IdentityDescriptorChange>())
            {
              if (items.Any<IdentityDescriptorChange>((Func<IdentityDescriptorChange, bool>) (change => change.ChangeType == DescriptorChangeType.Major)))
              {
                identityChanges.DescriptorChangeType = DescriptorChangeType.Major;
              }
              else
              {
                identityChanges.DescriptorChangeType = DescriptorChangeType.Minor;
                identityChanges.DescriptorChanges = items.Select<IdentityDescriptorChange, Guid>((Func<IdentityDescriptorChange, Guid>) (change => change.IdentityId)).ToList<Guid>();
                identityChanges.DescriptorChangesWithMasterId = new List<Guid>((IEnumerable<Guid>) identityChanges.DescriptorChanges);
              }
            }
          }
        }
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        {
          using (ResultCollection scopedIdentityChanges = identityComponent.GetScopedIdentityChanges(organizationIdentitySequenceId, this.Domain.DomainId, useIdentityAudit, false, out lastSequenceId3))
            identityChanges.OrganizationIdentityChangeIds = scopedIdentityChanges.GetCurrent<IdentityManagementComponent.ReferencedIdentity>().Where<IdentityManagementComponent.ReferencedIdentity>((Func<IdentityManagementComponent.ReferencedIdentity, bool>) (x => x.Location != IdentityManagementComponent.ReferencedIdentityLocation.Remote)).Select<IdentityManagementComponent.ReferencedIdentity, Guid>((Func<IdentityManagementComponent.ReferencedIdentity, Guid>) (x => x.IdentityId)).ToList<Guid>();
        }
      }
      else
      {
        identityChanges.OrganizationIdentityChangeIds = new List<Guid>();
        lastSequenceId3 = organizationIdentitySequenceId;
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        {
          List<IdentityDescriptorChange> items;
          using (ResultCollection scopedIdentityChanges = identityComponent.GetScopedIdentityChanges(organizationIdentitySequenceId, this.Domain.DomainId, useIdentityAudit, true, out lastSequenceId4))
          {
            identityChanges.IdentityChangeIds = scopedIdentityChanges.GetCurrent<IdentityManagementComponent.ReferencedIdentity>().Where<IdentityManagementComponent.ReferencedIdentity>((Func<IdentityManagementComponent.ReferencedIdentity, bool>) (x => x.Location != IdentityManagementComponent.ReferencedIdentityLocation.Remote)).Select<IdentityManagementComponent.ReferencedIdentity, Guid>((Func<IdentityManagementComponent.ReferencedIdentity, Guid>) (x => x.IdentityId)).ToList<Guid>();
            scopedIdentityChanges.NextResult();
            items = scopedIdentityChanges.GetCurrent<IdentityDescriptorChange>().Items;
            lastSequenceId1 = (int) lastSequenceId4;
          }
          if (items != null)
          {
            if (items.Any<IdentityDescriptorChange>())
            {
              if (items.Any<IdentityDescriptorChange>((Func<IdentityDescriptorChange, bool>) (change => change.ChangeType == DescriptorChangeType.Major)))
              {
                identityChanges.DescriptorChangeType = DescriptorChangeType.Major;
              }
              else
              {
                identityChanges.DescriptorChangeType = DescriptorChangeType.Minor;
                identityChanges.DescriptorChanges = items.Select<IdentityDescriptorChange, Guid>((Func<IdentityDescriptorChange, Guid>) (change => change.IdentityId)).ToList<Guid>();
                identityChanges.DescriptorChangesWithMasterId = new List<Guid>((IEnumerable<Guid>) identityChanges.DescriptorChanges);
              }
            }
          }
        }
      }
      bool scopedGroupChanges = IdentityMembershipHelper.ShouldGetScopedGroupChanges(requestContext);
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
      {
        using (ResultCollection changes = groupComponent.GetChanges(groupSequenceId, this.m_masterDomain.DomainRoot.Identifier, requestContext.ServiceHost.InstanceId, out Guid _, out lastSequenceId2, getScopedGroupChanges: scopedGroupChanges))
        {
          identityChanges.MembershipChanges = changes.GetCurrent<MembershipChangeInfo>().Items;
          changes.NextResult();
          identityChanges.GroupChangeIds = changes.GetCurrent<Guid>().Items;
        }
      }
      PlatformIdentityStore.PlatformIdentityChangeProcessor.AugmentMembershipChangeInfo(requestContext, this.m_cache, this.m_masterDomain, identityChanges.MembershipChanges);
      identityChanges.LatestSequenceId = lastSequenceId1;
      identityChanges.LatestIdentitySequenceId = (long) lastSequenceId1;
      identityChanges.LatestGroupSequenceId = lastSequenceId2;
      identityChanges.LatestOrgIdentitySequenceId = lastSequenceId3;
      return identityChanges;
    }

    internal IList<IdentityIdTranslation> GetTranslations(IVssRequestContext requestContext)
    {
      using (IdentityIdTranslationComponent component = requestContext.CreateComponent<IdentityIdTranslationComponent>())
        return component.GetTranslations();
    }

    private void TraceCacheStats(IVssRequestContext requestContext, object taskArgs)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.IdentityStore.TraceCacheCounts"))
        return;
      Microsoft.VisualStudio.Services.Identity.IdentityCache identityCacheByDomain = this.m_cache.GetIdentityCacheByDomain(this.m_masterDomain);
      if (identityCacheByDomain != null)
      {
        IEnumerable<int> cacheCounts = identityCacheByDomain.GetCacheCounts(requestContext);
        requestContext.Trace(80054, TraceLevel.Info, "IdentityService", "IdentityStore", string.Join<int>(",", cacheCounts));
      }
      else
        requestContext.Trace(80054, TraceLevel.Info, "IdentityService", "IdentityStore", "IdentityCache is not initialized");
    }

    internal void ProcessIdentityChangeOnAuthor(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      int identitySequenceId,
      int groupSequenceId,
      DescriptorChangeType descriptorChangeType = DescriptorChangeType.None,
      ICollection<Guid> descriptorChangeIds = null,
      ICollection<Guid> groupScopeChangeIds = null,
      ICollection<MembershipChangeInfo> membershipChangeInfos = null)
    {
      this.m_changeProcessor.ProcessIdentityChangeOnAuthor(requestContext, hostDomain, identitySequenceId, groupSequenceId, descriptorChangeType, descriptorChangeIds, groupScopeChangeIds, membershipChangeInfos);
    }

    internal void ProcessParentIdentityChangeOnAuthor(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      int parentIdentitySequenceId,
      ICollection<Guid> propertyChangeIds,
      DescriptorChangeType descriptorChangeType,
      ICollection<Guid> descriptorChangeIds,
      ICollection<MembershipChangeInfo> membershipChangeInfos = null)
    {
      this.m_changeProcessor.ProcessParentIdentityChangeOnAuthor(requestContext, hostDomain, parentIdentitySequenceId, propertyChangeIds, descriptorChangeType, descriptorChangeIds, membershipChangeInfos);
    }

    private static void TraceConditionally(
      IVssRequestContext requestContext,
      int tracePoint,
      TraceLevel traceLevel,
      string traceArea,
      string traceLayer,
      Func<string> message)
    {
      if (!requestContext.IsTracing(tracePoint, traceLevel, traceArea, traceLayer))
        return;
      requestContext.Trace(tracePoint, traceLevel, traceArea, traceLayer, message());
    }

    public static void DeduplicateMembershipChanges(List<MembershipChangeInfo> membershipChanges)
    {
      foreach (Tuple<Guid, Guid, List<MembershipChangeInfo>> tuple in membershipChanges.GroupBy(m => new
      {
        ContainerId = m.ContainerId,
        MemberId = m.MemberId
      }).Select<IGrouping<\u003C\u003Ef__AnonymousType11<Guid, Guid>, MembershipChangeInfo>, Tuple<Guid, Guid, List<MembershipChangeInfo>>>(g => new Tuple<Guid, Guid, List<MembershipChangeInfo>>(g.Key.ContainerId, g.Key.MemberId, g.ToList<MembershipChangeInfo>())).Where<Tuple<Guid, Guid, List<MembershipChangeInfo>>>((Func<Tuple<Guid, Guid, List<MembershipChangeInfo>>, bool>) (gm => gm.Item3 != null && gm.Item3.Count > 1)))
      {
        Tuple<Guid, Guid, List<MembershipChangeInfo>> duplicateMembershipChange = tuple;
        if (duplicateMembershipChange.Item3.Any<MembershipChangeInfo>((Func<MembershipChangeInfo, bool>) (dm => dm.Active)) && duplicateMembershipChange.Item3.Any<MembershipChangeInfo>((Func<MembershipChangeInfo, bool>) (dm => !dm.Active)))
          membershipChanges.RemoveAll((Predicate<MembershipChangeInfo>) (m => m.ContainerId == duplicateMembershipChange.Item1 && m.MemberId == duplicateMembershipChange.Item2 && !m.Active));
      }
    }

    internal override void ClearCache(IVssRequestContext requestContext) => this.m_cache.Clear(requestContext);

    internal void InvalidateIdentities(
      IVssRequestContext requestContext,
      ICollection<Guid> identityIds)
    {
      this.InvalidateIdentities(requestContext, this.Domain, (IList<Guid>) identityIds.ToList<Guid>());
    }

    private bool SkipPropertyUpdateInHostedPropertyService(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IdentityPropertyScope identityPropertyScope,
      string property)
    {
      if (identity.ResourceVersion == 0 || identity.IsContainer)
        return false;
      if (IdentityExtendedPropertyKeys.IdentityExtensionSupportedProperties.Contains(property))
        return true;
      if (identityPropertyScope == IdentityPropertyScope.Local || requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      PlatformIdentityStore.TraceConditionally(requestContext, 80901, TraceLevel.Verbose, "IdentityService", "IdentityStore", (Func<string>) (() => "Skipping property " + property + " in PlatformIdentityStore as this property is not a whitelisted property."));
      return true;
    }

    private void ReadExtendedProperties(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IEnumerable<string> propertyNameFilters,
      IIdMapper identityMapper)
    {
      if (propertyNameFilters == null)
        return;
      foreach (string propertyNameFilter in propertyNameFilters)
        IdentityPropertyKpis.IncrementReadsForProperty(propertyNameFilter);
      if (identities == null || !identities.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
        return;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        this.ReadPropertiesFromPropertyStore(requestContext, identities, propertyNameFilters);
      }
      else
      {
        List<Microsoft.VisualStudio.Services.Identity.Identity> list1 = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null && i.IsContainer)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null && !i.IsContainer)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (list1 != null && list1.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
          this.ReadPropertiesFromPropertyStoreForGroups(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list1, propertyNameFilters);
        if (list2 == null || !list2.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
          return;
        if (propertyNameFilters != null && propertyNameFilters.Any<string>((Func<string, bool>) (p => p != null && "UserId".Equals(p, StringComparison.InvariantCultureIgnoreCase))))
        {
          requestContext.Trace(80091, TraceLevel.Info, "IdentityService", "IdentityStore", "Read identity with Extended property : UserId");
          propertyNameFilters = propertyNameFilters.Except<string>((IEnumerable<string>) new string[1]
          {
            "UserId"
          }, (IEqualityComparer<string>) VssStringComparer.UserId);
          IVssRequestContext vssRequestContext = (IVssRequestContext) null;
          IUserIdentifierConversionService conversionService = (IUserIdentifierConversionService) null;
          bool flag = requestContext.IsDeploymentFallbackIdentityReadAllowed();
          if (!flag)
          {
            vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
            conversionService = vssRequestContext.GetService<IUserIdentifierConversionService>();
          }
          foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in list2)
          {
            Guid guid = Guid.Empty;
            if (!flag)
            {
              if (IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) identity))
              {
                try
                {
                  guid = conversionService.GetStorageKeyByDescriptor(vssRequestContext, identity.SubjectDescriptor);
                  goto label_26;
                }
                catch (UserDoesNotExistException ex)
                {
                  requestContext.TraceException(10101005, TraceLevel.Verbose, "IdentityService", "IdentityStore", (Exception) ex);
                  goto label_26;
                }
              }
            }
            guid = identity.MasterId;
label_26:
            identity.SetProperty("UserId", (object) guid);
          }
        }
        propertyNameFilters = propertyNameFilters.Except<string>((IEnumerable<string>) IdentityExtendedPropertyKeys.IdentityUnsupportedProperties);
        List<string> propertiesInPropertyService = propertyNameFilters.Where<string>((Func<string, bool>) (p => p != null && !IdentityExtendedPropertyKeys.IdentityExtensionSupportedProperties.Contains(p))).ToList<string>();
        if (propertiesInPropertyService == null || !propertiesInPropertyService.Any<string>())
          return;
        requestContext.TraceConditionally(80055, TraceLevel.Verbose, "IdentityService", "IdentityStore", (Func<string>) (() => "Reading extended properties: " + string.Join(", ", (IEnumerable<string>) propertiesInPropertyService)));
        this.ReadPropertiesFromPropertyStore(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list2, (IEnumerable<string>) propertiesInPropertyService);
      }
    }

    private void ReadPropertiesFromPropertyStore(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IEnumerable<string> propertyNameFilters)
    {
      if (propertyNameFilters == null)
        return;
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities1 = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null && propertyNameFilters.Any<string>((Func<string, bool>) (p => p != null && !i.Properties.ContainsKey(p)))));
      if (identities1 == null || !identities1.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
        return;
      this.PropertyHelper.ReadExtendedProperties(requestContext, identities1, propertyNameFilters, IdentityPropertyScope.Global);
    }

    private void ReadPropertiesFromPropertyStoreForGroups(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IEnumerable<string> propertyNameFilters)
    {
      if (propertyNameFilters == null)
        return;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UsePartitionedGroupProperties"))
        ReadFrom(IdentityPropertyScope.Both);
      else
        ReadFrom(IdentityPropertyScope.Global);

      void ReadFrom(IdentityPropertyScope propertyScope)
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null && propertyNameFilters.Any<string>((Func<string, bool>) (p => p != null && !i.Properties.ContainsKey(p)))));
        if (identities == null || !identities.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
          return;
        this.PropertyHelper.ReadExtendedProperties(requestContext, identities, propertyNameFilters, propertyScope);
      }
    }

    internal static bool ParseIdentityName(
      string nameString,
      out string domain,
      out string account,
      out int uniqueUserId)
    {
      domain = (string) null;
      account = (string) null;
      uniqueUserId = 0;
      int length = nameString.IndexOf('\\');
      if (length < 0)
      {
        account = nameString;
        UserNameUtil.TryParseUniqueUserName(account, out account, out uniqueUserId);
        return false;
      }
      domain = nameString.Substring(0, length).Trim();
      account = nameString.Substring(length + 1);
      UserNameUtil.TryParseUniqueUserName(account, out account, out uniqueUserId);
      return true;
    }

    internal virtual IReadOnlyList<IdentityAuditRecord> GetIdentityAuditRecords(
      IVssRequestContext requestContext,
      DateTime lastUpdatedOnOrBefore)
    {
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        return (IReadOnlyList<IdentityAuditRecord>) identityComponent.GetIdentityAuditRecords(lastUpdatedOnOrBefore).AsReadOnly();
    }

    internal virtual void DeleteIdentityAuditRecords(
      IVssRequestContext requestContext,
      long sequenceId)
    {
      using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
        identityComponent.DeleteIdentityAuditRecords(sequenceId);
    }

    internal static IdentityDatabaseTypes GetIdentityDatabaseTypes(
      IVssRequestContext requestContext,
      IEnumerable<Guid> identityIds)
    {
      IdentityDatabaseTypes identityDatabaseTypes = IdentityDatabaseTypes.None;
      if (identityIds != null)
      {
        foreach (Guid identityId in identityIds)
          identityDatabaseTypes |= PlatformIdentityStore.GetIdentityDatabaseType(requestContext, identityId);
      }
      else
        identityDatabaseTypes |= IdentityDatabaseTypes.SPS;
      return identityDatabaseTypes;
    }

    internal static IdentityDatabaseTypes GetIdentityDatabaseType(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      if (identityId == Guid.Empty)
        return IdentityDatabaseTypes.None;
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !ServicePrincipals.IsInternalServicePrincipalId(identityId))
        return IdentityDatabaseTypes.SPS;
      requestContext.TraceDataConditionally(414583, TraceLevel.Error, "IdentityService", "IdentityStore", "Trying to read or write S2S identity using IMS, this shouldn't happen", (Func<object>) (() => (object) new
      {
        identityId = identityId,
        StackTrace = Environment.StackTrace
      }), nameof (GetIdentityDatabaseType));
      return IdentityDatabaseTypes.None;
    }

    internal static IdentityDatabaseTypes GetIdentityDatabaseType(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      if (descriptor == (IdentityDescriptor) null)
        return IdentityDatabaseTypes.None;
      if (descriptor.IsCspPartnerIdentityType())
        return IdentityDatabaseTypes.UserService;
      if (!ServicePrincipals.IsServicePrincipal(requestContext, descriptor))
        return IdentityDatabaseTypes.SPS;
      requestContext.TraceDataConditionally(414583, TraceLevel.Error, "IdentityService", "IdentityStore", "Trying to read or write S2S identity using IMS, this shouldn't happen", (Func<object>) (() => (object) new
      {
        descriptor = descriptor,
        StackTrace = Environment.StackTrace
      }), nameof (GetIdentityDatabaseType));
      return IdentityDatabaseTypes.None;
    }

    internal static IdentityManagementComponent CreateDeploymentIdentityComponent(
      IVssRequestContext requestContext)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<IdentityManagementComponent>();
    }

    internal static IdentityManagementComponent CreateOrganizationIdentityComponent(
      IVssRequestContext requestContext)
    {
      return requestContext.To(TeamFoundationHostType.Application).CreateComponent<IdentityManagementComponent>();
    }

    internal IVssRequestContext GetSecurityContext(IVssRequestContext requestContext) => !requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Application) ? requestContext : requestContext.To(TeamFoundationHostType.Application);

    private void IncrementCacheMissPerfCounters()
    {
      PlatformIdentityStore.s_imsCacheMissesCounter.Increment();
      PlatformIdentityStore.s_imsCacheMissesPerSecCounter.Increment();
    }

    private void IncrementMembershipCacheInvalidationsPerfCounters()
    {
      PlatformIdentityStore.s_imsMembershipInvalidationsCounter.Increment();
      PlatformIdentityStore.s_imsMembershipInvalidationsPerSecCounter.Increment();
    }

    private void IncrementCacheMissPerfCounters(long incrementBy)
    {
      PlatformIdentityStore.s_imsCacheMissesCounter.IncrementBy(incrementBy);
      PlatformIdentityStore.s_imsCacheMissesPerSecCounter.IncrementBy(incrementBy);
    }

    private void CacheIdentities(
      IVssRequestContext requestContext,
      List<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IdentityDomain hostDomain,
      QueryMembership queryMembership)
    {
      bool flag1 = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.CacheUnauthenticatedTypes");
      bool flag2 = requestContext.IsTracing(80099, TraceLevel.Info, "IdentityService", "IdentityStore");
      bool flag3 = requestContext.IsTracing(80751, TraceLevel.Verbose, "IdentityService", "IdentityStore");
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
      {
        if (identity?.Descriptor != (IdentityDescriptor) null)
        {
          if (identity.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.UnauthenticatedIdentity", StringComparison.OrdinalIgnoreCase) && !flag1)
          {
            if (flag2)
            {
              requestContext.Trace(80099, TraceLevel.Info, "IdentityService", "IdentityStore", "Skipping caching unauthenticated identity : {0}", (object) identity.Descriptor);
              continue;
            }
            continue;
          }
          if (flag3 && identity.Descriptor.IdentityType == "Microsoft.TeamFoundation.ServiceIdentity")
            requestContext.Trace(80751, TraceLevel.Verbose, "IdentityService", "IdentityStore", string.Format("IdentityDomain: {0} | HostDomain: {1} | Descriptor: {2} | Stack: {3}", (object) identity.GetProperty<string>("Domain", "NULL"), (object) hostDomain.DomainId, (object) identity.Descriptor.ToString(), (object) Environment.StackTrace));
        }
        this.m_cache.UpdateIdentity(requestContext, hostDomain, queryMembership, identity);
      }
    }

    private void IncrementCacheHitPerfCounters() => PlatformIdentityStore.s_imsCacheHitsCounter.Increment();

    private void OnIdentityIdTranslationChangedNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      IdentityIdTranslationChangeData identityIdTranslationChangeData = TeamFoundationSerializationUtility.Deserialize<IdentityIdTranslationChangeData>(eventData);
      if (identityIdTranslationChangeData == null)
        return;
      this.m_cache.OnIdentityIdTranslationChanged(requestContext, identityIdTranslationChangeData);
    }

    internal event EventHandler<DescriptorChangeNotificationEventArgs> DescriptorsChangedNotification;

    internal event EventHandler<DescriptorChangeEventArgs> DescriptorsChanged;

    private static Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> AddParentIdentitiesChangedHandlerDelegate(
      Guid hostId,
      PlatformIdentityStore parentIdentityStore)
    {
      return parentIdentityStore == null ? (Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>>) (_ => { }) : (Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>>) (handler =>
      {
        if (parentIdentityStore.IdentitiesChanged is IDictionaryStoredIdentityEventHandler<Guid, PlatformIdentityStore.IdentityChangeEventArgs> identitiesChanged2)
          identitiesChanged2.AddHandler(hostId, handler);
        else
          parentIdentityStore.IdentitiesChanged.AddHandler(handler);
      });
    }

    private static Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> RemoveParentIdentitiesChangedHandlerDelegate(
      Guid hostId,
      PlatformIdentityStore parentIdentityStore)
    {
      return parentIdentityStore == null ? (Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>>) (_ => { }) : (Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>>) (handler =>
      {
        if (parentIdentityStore.IdentitiesChanged is IDictionaryStoredIdentityEventHandler<Guid, PlatformIdentityStore.IdentityChangeEventArgs> identitiesChanged2)
          identitiesChanged2.RemoveHandler(hostId);
        else
          parentIdentityStore.IdentitiesChanged.RemoveHandler(handler);
      });
    }

    internal override IIdentityCache IdentityCache => (IIdentityCache) this.m_cache;

    internal override IdentityDomain Domain => this.m_masterDomain;

    internal IdentityPropertyHelper PropertyHelper { get; }

    internal virtual int GetCurrentChangeId() => this.m_changeProcessor.GetCurrentChangeId();

    internal virtual void InitializeIdentiesChangedEventProcessor() => this.IdentitiesChanged = (IIdentityEventHandler<PlatformIdentityStore.IdentityChangeEventArgs>) new DictionaryStoredIdentityEventHandler<Guid, PlatformIdentityStore.IdentityChangeEventArgs>();

    internal IIdMapper GetIdMapper(IVssRequestContext requestContext, IdentityDomain hostDomain) => this.m_cache.GetIdMapper(requestContext, hostDomain);

    internal class IdentityChangeEventArgs : EventArgs
    {
      public IVssRequestContext RequestContext { get; set; }

      public int IdentitySequenceId { get; set; }

      public ICollection<Guid> PropertyChangeIds { get; set; }

      public DescriptorChangeType DescriptorChangeType { get; set; }

      public ICollection<Guid> DescriptorChangeIds { get; set; }

      public ICollection<Guid> GroupScopeChangeIds { get; set; }

      public ICollection<MembershipChangeInfo> MembershipChangeInfos { get; set; }
    }

    internal class ScopeChangeEventArgs : EventArgs
    {
      public IVssRequestContext RequestContext { get; set; }

      public ICollection<Guid> GroupScopeChangeIds { get; set; }
    }

    internal class ReadReplicaFeatureFlags
    {
      internal const string IsMemberRGMPopulateMinSequenceId = "VisualStudio.Services.Identity.IsMember.RGM.PopulateMinSequenceId";
      internal const string ReadGroupMembershipComponent = "VisualStudio.Services.Identity.ReadGroupMembershipComponent.EnableReadFromReadReplica";
      internal const string QueryMembershipDirect = "VisualStudio.Services.Identity.RGM.EnableReadFromReadReplica.Direct";
      internal const string QueryMembershipExpandedUp = "VisualStudio.Services.Identity.RGM.EnableReadFromReadReplica.ExpandedUp";
      internal const string QueryMembershipExpandedDown = "VisualStudio.Services.Identity.RGM.EnableReadFromReadReplica.ExpandedDown";
      internal const string Fault = "VisualStudio.Services.Identity.RGM.EnableReadFromReadReplica.Fault";
    }

    internal class PlatformIdentityChangeProcessor
    {
      protected readonly IdentityDomain m_masterDomain;
      protected readonly IPlatformIdentityCache m_cache;
      private readonly object m_processChangesLock = new object();
      private static readonly Guid m_MessageBusIdentityChangePublisherJobId = new Guid("27E30CB7-A0D4-442E-A2C4-845E49FDC362");
      private ConcurrentHashSet<Guid> m_alreadyInProcessTasksSet;
      private PlatformIdentityStore.PlatformIdentityChangeProcessor.ISequenceIDUtility m_sequenceIDUtility;
      private readonly ILockName m_processChangesTaskLockName;
      private IdentityStoreBase.TaskState m_processChangesTaskState;
      private int m_currentMembershipChangeTasks;
      private int m_membershipChangeTaskThreshold;
      protected int m_changeId;
      protected int m_identitySequenceId;
      protected int m_groupSequenceId;
      protected int m_targetIdentitySequenceId;
      protected int m_targetGroupSequenceId;
      protected SortedSet<int> m_descriptorChangeIdsProcessed;
      protected SortedSet<int> m_parentDescriptorChangeIdsProcessed;
      protected const int c_maxDescriptorChangeIds = 100;
      protected const string c_augmentMembershipChangeInfoFeatureFlagDisable = "VisualStudio.Services.Identity.AugmentMembershipChangeInfo.Disable";
      protected const string c_publishIdentityChangedDataEventFeatureFlagDisable = "VisualStudio.Services.Identity.PublishIdentityChangedDataEvent.Disable";
      protected const string c_immediateProcessingOfMembershipChangesFeatureFlag = "VisualStudio.Services.Identity.ImmediateProcessingOfMembershipChanges";
      private uint m_identityChangePublisherTaskQueueDelayInSeconds;
      private const uint c_identityChangePublisherTaskQueueDelayInSeconds = 10;
      private const string s_area = "IdentityService";
      private const string s_layer = "IdentityChangeService";

      protected PlatformIdentityChangeProcessor(
        Func<IIdentityEventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> getIdentitiesChangedEvent,
        Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> addParentIdentitiesChangedListener,
        Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> removeParentIdentitiesChangedListener)
      {
        this.GetIdentitiesChangedEvent = getIdentitiesChangedEvent;
        this.AddParentIdentitiesChangedHandler = addParentIdentitiesChangedListener;
        this.RemoveParentIdentitiesChangedHandler = removeParentIdentitiesChangedListener;
        this.m_alreadyInProcessTasksSet = new ConcurrentHashSet<Guid>((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default);
      }

      public PlatformIdentityChangeProcessor(
        IVssRequestContext systemRequestContext,
        IdentityDomain masterDomain,
        IPlatformIdentityCache identityCache,
        Func<EventHandler<DescriptorChangeEventArgs>> getDescriptorsChangedEvent,
        Func<EventHandler<DescriptorChangeNotificationEventArgs>> getDescriptorsChangedNotficationEvent,
        Func<IIdentityEventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> getIdentitiesChangedEvent,
        Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> addParentIdentitiesChangedListener,
        Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> removeParentIdentitiesChangedListener,
        Action<long> updateGroupSequenceId,
        PlatformIdentityStore.PlatformIdentityChangeProcessor.ISequenceIDUtility sequenceIDUtility)
        : this(getIdentitiesChangedEvent, addParentIdentitiesChangedListener, removeParentIdentitiesChangedListener)
      {
        this.m_masterDomain = masterDomain;
        this.m_cache = identityCache;
        this.GetDescriptorsChangedEvent = getDescriptorsChangedEvent;
        this.GetDescriptorsChangedNotificationEvent = getDescriptorsChangedNotficationEvent;
        this.UpdateGroupSequenceId = updateGroupSequenceId;
        this.m_sequenceIDUtility = sequenceIDUtility;
        ICachedRegistryService service = systemRequestContext.GetService<ICachedRegistryService>();
        this.m_processChangesTaskLockName = systemRequestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}.{1}.{2}", (object) nameof (PlatformIdentityStore), (object) nameof (m_processChangesTaskLockName), (object) masterDomain.DomainId));
        this.m_descriptorChangeIdsProcessed = new SortedSet<int>();
        this.m_parentDescriptorChangeIdsProcessed = new SortedSet<int>();
        this.InitializeSequenceIds(systemRequestContext, masterDomain);
        int num = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Identity/Settings/IdentityChangePublisherJobSequenceId", -1);
        this.m_membershipChangeTaskThreshold = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Identity/Settings/MembershipChangeMaxTaskThreshold", 100);
        if (num != -1)
          return;
        service.SetValue<int>(systemRequestContext, "/Service/Identity/Settings/IdentityChangePublisherJobSequenceId", this.m_identitySequenceId);
        systemRequestContext.Trace(80049, TraceLevel.Info, "IdentityService", "IdentityChangeService", "ServiceStart: Updated the identity sequence id from {0} to {1} in the registry for identity change publisher job.", (object) num, (object) this.m_identitySequenceId);
      }

      public void Load(
        IVssRequestContext requestContext,
        PlatformIdentityStore.PlatformIdentityChangeProcessor previousProcessor = null)
      {
        PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.Load.CallsPerSecond.Increment();
        this.PullLatestState(previousProcessor);
        this.AddParentIdentitiesChangedHandler(new EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>(this.OnParentIdentityChangeEvent));
        TeamFoundationSqlNotificationService service = requestContext.GetService<TeamFoundationSqlNotificationService>();
        service.RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.IMS2PlatformIdentityChanged, new SqlNotificationCallback(this.OnIdentityChangedNotification), false);
        service.RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.IMS2ClearCache, new SqlNotificationCallback(this.OnIdentityClearCacheNotification), false);
        this.InitializeAndRegisterNotificationForIdentityChangePublisherTaskSettings(requestContext);
      }

      public void Unload(IVssRequestContext requestContext)
      {
        PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.Unload.CallsPerSecond.Increment();
        this.UnregisterNotificationForIdentityChangePublisherTaskSettings(requestContext);
        TeamFoundationSqlNotificationService service = requestContext.GetService<TeamFoundationSqlNotificationService>();
        service.UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.IMS2ClearCache, new SqlNotificationCallback(this.OnIdentityClearCacheNotification), false);
        service.UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.IMS2PlatformIdentityChanged, new SqlNotificationCallback(this.OnIdentityChangedNotification), false);
        this.RemoveParentIdentitiesChangedHandler(new EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>(this.OnParentIdentityChangeEvent));
      }

      public int GetCurrentChangeId() => this.m_changeId;

      public void ProcessIdentityChangeOnAuthor(
        IVssRequestContext requestContext,
        IdentityDomain hostDomain,
        int identitySequenceId,
        int groupSequenceId,
        DescriptorChangeType descriptorChangeType = DescriptorChangeType.None,
        ICollection<Guid> descriptorChangeIds = null,
        ICollection<Guid> groupScopeChangeIds = null,
        ICollection<MembershipChangeInfo> membershipChangeInfos = null)
      {
        if (!this.ProcessIdentityChange(requestContext, hostDomain, identitySequenceId, groupSequenceId, descriptorChangeType, descriptorChangeIds, groupScopeChangeIds, membershipChangeInfos, true))
          return;
        this.FireEvents(requestContext, Math.Max(this.m_identitySequenceId, this.m_groupSequenceId));
      }

      public void ProcessParentIdentityChangeOnAuthor(
        IVssRequestContext requestContext,
        IdentityDomain hostDomain,
        int parentIdentitySequenceId,
        ICollection<Guid> propertyChangeIds,
        DescriptorChangeType descriptorChangeType,
        ICollection<Guid> descriptorChangeIds,
        ICollection<MembershipChangeInfo> membershipChangeInfos)
      {
        this.ProcessParentIdentityChange(requestContext, parentIdentitySequenceId, propertyChangeIds, descriptorChangeType, descriptorChangeIds, membershipChangeInfos);
        this.FireEvents(requestContext, Math.Max(this.m_identitySequenceId, this.m_groupSequenceId));
        if (!membershipChangeInfos.IsNullOrEmpty<MembershipChangeInfo>())
        {
          requestContext.TraceConditionally(80777, TraceLevel.Info, "IdentityService", "IdentityChangeService", (Func<string>) (() => "Firing message bus signal and sql notification for membershipchanges for Members IDs : " + string.Join(", ", membershipChangeInfos.Select<MembershipChangeInfo, string>((Func<MembershipChangeInfo, string>) (membership => membership?.MemberId.ToString() ?? "")))));
          PlatformIdentityStore.PlatformIdentityChangeProcessor.SendMembershipChangesSqlNotification(requestContext, (IEnumerable<MembershipChangeInfo>) membershipChangeInfos);
          IdentityMessage identityMessage = new IdentityMessage()
          {
            MembershipChanges = membershipChangeInfos.ToArray<MembershipChangeInfo>()
          };
          requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Graph", (object[]) new IdentityMessage[1]
          {
            identityMessage
          }, false);
        }
        IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Application);
        if (!vssRequestContext1.ExecutionEnvironment.IsHostedDeployment)
          return;
        vssRequestContext1.Trace(80049, TraceLevel.Info, "IdentityService", "IdentityChangeService", "ProcessParentIdentityChangeOnAuthor: Queuing identity change publisher job.");
        CachedRegistryService service = vssRequestContext1.GetService<CachedRegistryService>();
        IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext2.GetService<TeamFoundationJobService>().QueueDelayedJobs(vssRequestContext2, (IEnumerable<Guid>) new Guid[1]
        {
          PlatformIdentityStore.PlatformIdentityChangeProcessor.m_MessageBusIdentityChangePublisherJobId
        }, service.GetValue<int>(vssRequestContext1, (RegistryQuery) "/Service/Identity/Settings/IdentityChangePublisherJobQueueDelayInSeconds", 5), service.GetValue<JobPriorityLevel>(vssRequestContext1, (RegistryQuery) "/Service/Identity/Settings/IdentityChangePublisherJobQueuePriority", JobPriorityLevel.Highest));
      }

      public void PublishIdentityChanges(
        IVssRequestContext requestContext,
        IdentityDomain hostDomain,
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identitiesToPublish,
        Action<IVssRequestContext, IdentityDomain, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>> beforePublish)
      {
        IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Application);
        if (!vssRequestContext1.ExecutionEnvironment.IsHostedDeployment || identitiesToPublish == null || !identitiesToPublish.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
          return;
        vssRequestContext1.Trace(80049, TraceLevel.Info, "IdentityService", "IdentityChangeService", "UpdateIdentities (extended properties): Queuing identity change publisher job.");
        beforePublish(vssRequestContext1, hostDomain, identitiesToPublish);
        CachedRegistryService service = vssRequestContext1.GetService<CachedRegistryService>();
        IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext2.GetService<TeamFoundationJobService>().QueueDelayedJobs(vssRequestContext2, (IEnumerable<Guid>) new Guid[1]
        {
          PlatformIdentityStore.PlatformIdentityChangeProcessor.m_MessageBusIdentityChangePublisherJobId
        }, service.GetValue<int>(vssRequestContext1, (RegistryQuery) "/Service/Identity/Settings/IdentityChangePublisherJobQueueDelayInSeconds", 5), service.GetValue<JobPriorityLevel>(vssRequestContext1, (RegistryQuery) "/Service/Identity/Settings/IdentityChangePublisherJobQueuePriority", JobPriorityLevel.Highest));
      }

      public static void AugmentMembershipChangeInfo(
        IVssRequestContext requestContext,
        IPlatformIdentityCache cache,
        IdentityDomain hostDomain,
        List<MembershipChangeInfo> membershipChanges)
      {
        try
        {
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AugmentMembershipChangeInfo.Disable") || !requestContext.ExecutionEnvironment.IsHostedDeployment || membershipChanges.IsNullOrEmpty<MembershipChangeInfo>())
            return;
          HashSet<Guid> enumerable = new HashSet<Guid>(membershipChanges.Where<MembershipChangeInfo>((Func<MembershipChangeInfo, bool>) (x => x != null && x.ContainerScopeId != Guid.Empty)).Select<MembershipChangeInfo, Guid>((Func<MembershipChangeInfo, Guid>) (x => x.ContainerScopeId)));
          if (enumerable.IsNullOrEmpty<Guid>())
            return;
          IDictionary<Guid, HashSet<Guid>> dictionary = (IDictionary<Guid, HashSet<Guid>>) new Dictionary<Guid, HashSet<Guid>>();
          foreach (Guid guid in enumerable)
          {
            HashSet<Guid> ancestorScopeIds = PlatformIdentityStore.PlatformIdentityChangeProcessor.GetAncestorScopeIds(requestContext, cache, hostDomain, guid);
            dictionary[guid] = ancestorScopeIds;
          }
          foreach (MembershipChangeInfo membershipChange in membershipChanges)
          {
            HashSet<Guid> source;
            if (dictionary.TryGetValue(membershipChange.ContainerScopeId, out source))
              membershipChange.ContainerAncestorScopeIds = source.ToList<Guid>();
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5645868, "IdentityService", "IdentityChangeService", ex);
        }
      }

      private void InitializeAndRegisterNotificationForIdentityChangePublisherTaskSettings(
        IVssRequestContext requestContext)
      {
        requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.UpdateIdentityChangePublisherTaskSettings), true, (RegistryQuery) "/Service/Identity/Settings/IdentityChangePublisherTask");
        this.UpdateIdentityChangePublisherTaskSettings(requestContext, (RegistryEntryCollection) null);
      }

      private void UnregisterNotificationForIdentityChangePublisherTaskSettings(
        IVssRequestContext requestContext)
      {
        requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.UpdateIdentityChangePublisherTaskSettings));
      }

      private void UpdateIdentityChangePublisherTaskSettings(
        IVssRequestContext context,
        RegistryEntryCollection changedEntries)
      {
        this.m_identityChangePublisherTaskQueueDelayInSeconds = context.GetService<IVssRegistryService>().GetValue<uint>(context, (RegistryQuery) "/Service/Identity/Settings/IdentityChangePublisherTask/QueueDelayInSeconds", true, 10U);
      }

      private static HashSet<Guid> GetAncestorScopeIds(
        IVssRequestContext requestContext,
        IPlatformIdentityCache cache,
        IdentityDomain hostDomain,
        Guid scopeId)
      {
        HashSet<Guid> ancestorScopeIds;
        if (cache.TryReadScopeAncestorIds(requestContext, hostDomain, scopeId, out ancestorScopeIds))
          return ancestorScopeIds;
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
          ancestorScopeIds = new HashSet<Guid>((IEnumerable<Guid>) groupComponent.GetAncestorScopeIds(scopeId, true));
        ancestorScopeIds.Remove(Guid.Empty);
        cache.UpdateScopeAncestorIds(requestContext, hostDomain, scopeId, ancestorScopeIds);
        return ancestorScopeIds;
      }

      protected void InitializeSequenceIds(
        IVssRequestContext requestContext,
        IdentityDomain hostDomain)
      {
        this.m_sequenceIDUtility.InitializeSequenceIds(requestContext, hostDomain, this.m_masterDomain, ref this.m_identitySequenceId, out this.m_groupSequenceId);
        this.UpdateGroupSequenceId((long) this.m_groupSequenceId);
      }

      private void PullLatestState(
        PlatformIdentityStore.PlatformIdentityChangeProcessor previousProcessor)
      {
        if (previousProcessor == null)
          return;
        if (previousProcessor.m_identitySequenceId > this.m_identitySequenceId)
          this.m_identitySequenceId = previousProcessor.m_identitySequenceId;
        if (previousProcessor.m_groupSequenceId > this.m_groupSequenceId)
          this.m_groupSequenceId = previousProcessor.m_groupSequenceId;
        if (previousProcessor.m_changeId > this.m_changeId)
          this.m_changeId = previousProcessor.m_changeId;
        if (previousProcessor.m_descriptorChangeIdsProcessed.Max > this.m_descriptorChangeIdsProcessed.Max)
          this.m_descriptorChangeIdsProcessed = previousProcessor.m_descriptorChangeIdsProcessed;
        if (previousProcessor.m_parentDescriptorChangeIdsProcessed.Max <= this.m_parentDescriptorChangeIdsProcessed.Max)
          return;
        this.m_parentDescriptorChangeIdsProcessed = previousProcessor.m_parentDescriptorChangeIdsProcessed;
      }

      private void OnIdentityClearCacheNotification(
        IVssRequestContext requestContext,
        Guid eventClass,
        string eventData)
      {
        this.m_cache.Clear(requestContext);
      }

      private void OnIdentityChangedNotification(
        IVssRequestContext requestContext,
        Guid eventClass,
        string eventData)
      {
        IdentityChangedData sqlNotification = TeamFoundationSerializationUtility.Deserialize<IdentityChangedData>(eventData);
        requestContext.Trace(80700, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Received notification. Identity Seq. ID: {0}, Group Seq. ID: {1}", (object) sqlNotification.IdentitySequenceId, (object) sqlNotification.GroupSequenceId);
        requestContext.Trace(80703, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "m_identitySequenceId: {0}, m_targetIdentitySequenceId: {1}", (object) this.m_identitySequenceId, (object) this.m_targetIdentitySequenceId);
        requestContext.Trace(80706, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "m_groupSequenceId: {0}, m_targetGroupSequenceId: {1}", (object) this.m_groupSequenceId, (object) this.m_targetGroupSequenceId);
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.IdentityCacheBase.CompareAndSwapSequenceContextIfGreater.Disable"))
          this.UpdateGroupSequenceId((long) sqlNotification.GroupSequenceId);
        if (sqlNotification.DescriptorChangeType != DescriptorChangeType.None)
        {
          requestContext.Trace(80701, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Descriptor Change");
          PlatformIdentityStore.IdentityChangeEventArgs taskArgs = new PlatformIdentityStore.IdentityChangeEventArgs()
          {
            RequestContext = requestContext,
            IdentitySequenceId = sqlNotification.IdentitySequenceId,
            PropertyChangeIds = (ICollection<Guid>) null,
            DescriptorChangeType = sqlNotification.DescriptorChangeType,
            DescriptorChangeIds = (ICollection<Guid>) sqlNotification.DescriptorChangeIds
          };
          TeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
          TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessDescriptorChangesTask), (object) taskArgs, 0);
          Guid instanceId = requestContext.ServiceHost.InstanceId;
          TeamFoundationTask task = teamFoundationTask;
          service.AddTask(instanceId, task);
        }
        else
        {
          TeamFoundationTask task1 = (TeamFoundationTask) null;
          bool flag1 = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.ImmediateProcessingOfMembershipChanges");
          using (requestContext.Lock(this.m_processChangesTaskLockName))
          {
            bool flag2 = false;
            requestContext.TraceConditionally(80702, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", (Func<string>) (() => "No descriptor changed."));
            if (sqlNotification.IdentitySequenceId > this.m_identitySequenceId && sqlNotification.IdentitySequenceId > this.m_targetIdentitySequenceId)
            {
              this.m_targetIdentitySequenceId = sqlNotification.IdentitySequenceId;
              flag2 = true;
              requestContext.TraceConditionally(80704, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", (Func<string>) (() => "Updated m_targetIdentitySequenceId to sqlNotificationIdentitySequenceId"));
            }
            if (sqlNotification.GroupSequenceId > this.m_groupSequenceId && sqlNotification.GroupSequenceId > this.m_targetGroupSequenceId)
            {
              this.m_targetGroupSequenceId = sqlNotification.GroupSequenceId;
              flag2 = true;
              requestContext.TraceConditionally(80705, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", (Func<string>) (() => "Updated m_targetGroupSequenceId to sqlNotificationGroupSequenceId"));
            }
            PlatformIdentityStore.IdentityChangeEventArgs taskArgs = (PlatformIdentityStore.IdentityChangeEventArgs) null;
            if (!flag1 && !((IEnumerable<MembershipChangeInfo>) sqlNotification.MembershipChanges).IsNullOrEmpty<MembershipChangeInfo>())
            {
              flag2 = true;
              requestContext.TraceConditionally(80743, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", (Func<string>) (() => "Queueing identity change event args with membership changes for Members IDs : " + string.Join(", ", ((IEnumerable<MembershipChangeInfo>) sqlNotification.MembershipChanges).Select<MembershipChangeInfo, string>((Func<MembershipChangeInfo, string>) (membership => membership?.MemberId.ToString() ?? "")))));
              taskArgs = new PlatformIdentityStore.IdentityChangeEventArgs()
              {
                MembershipChangeInfos = (ICollection<MembershipChangeInfo>) sqlNotification.MembershipChanges
              };
            }
            if (flag2)
            {
              if (this.m_processChangesTaskState == IdentityStoreBase.TaskState.NotQueued)
              {
                task1 = new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessChangesTask), (object) taskArgs, DateTime.UtcNow.AddSeconds((double) this.m_identityChangePublisherTaskQueueDelayInSeconds), 0);
                this.m_processChangesTaskState = IdentityStoreBase.TaskState.Queueing;
              }
            }
          }
          if (task1 != null)
          {
            try
            {
              requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext.ServiceHost.InstanceId, task1);
            }
            catch (Exception ex)
            {
              this.m_processChangesTaskState = IdentityStoreBase.TaskState.NotQueued;
              throw;
            }
            this.m_processChangesTaskState = IdentityStoreBase.TaskState.Queued;
          }
          if (flag1 && !((IEnumerable<MembershipChangeInfo>) sqlNotification.MembershipChanges).IsNullOrEmpty<MembershipChangeInfo>())
          {
            int num1;
            if (sqlNotification.MembershipChanges.Length == 1)
            {
              MembershipChangeInfo membershipChange = sqlNotification.MembershipChanges[0];
              if (membershipChange == null)
              {
                num1 = 0;
              }
              else
              {
                Guid memberId = membershipChange.MemberId;
                num1 = 1;
              }
            }
            else
              num1 = 0;
            bool flag3 = num1 != 0;
            if (this.m_currentMembershipChangeTasks > this.m_membershipChangeTaskThreshold)
              requestContext.Trace(81035, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Current task {0} count have reached the threshold of {1}. Members IDs : {2}", (object) this.m_currentMembershipChangeTasks, (object) this.m_membershipChangeTaskThreshold, (object) string.Join(", ", ((IEnumerable<MembershipChangeInfo>) sqlNotification.MembershipChanges).Select<MembershipChangeInfo, string>((Func<MembershipChangeInfo, string>) (membership => membership?.MemberId.ToString() ?? ""))));
            else if (flag3 && !this.m_alreadyInProcessTasksSet.Add(sqlNotification.MembershipChanges[0].MemberId))
            {
              requestContext.Trace(81036, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "There is currently a task for Member ID : {0} being processed now, will discard this one as it is a duplicate, Current tasks count {1}", (object) sqlNotification.MembershipChanges[0].MemberId, (object) this.m_currentMembershipChangeTasks);
            }
            else
            {
              PlatformIdentityStore.IdentityChangeEventArgs taskArgs = new PlatformIdentityStore.IdentityChangeEventArgs()
              {
                MembershipChangeInfos = (ICollection<MembershipChangeInfo>) sqlNotification.MembershipChanges
              };
              ITeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
              Interlocked.Increment(ref this.m_currentMembershipChangeTasks);
              requestContext.Trace(81034, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Queueing identity change event args with membership changes. CurrentTaskCount: {0}, Members IDs : {1}", (object) this.m_currentMembershipChangeTasks, (object) string.Join(", ", ((IEnumerable<MembershipChangeInfo>) sqlNotification.MembershipChanges).Select<MembershipChangeInfo, string>((Func<MembershipChangeInfo, string>) (membership => membership?.MemberId.ToString() ?? ""))));
              TeamFoundationTaskCallback callback = (TeamFoundationTaskCallback) ((context, args) =>
              {
                try
                {
                  this.ProcessMembershipChangesTask(context, args);
                }
                finally
                {
                  Interlocked.Decrement(ref this.m_currentMembershipChangeTasks);
                  PlatformIdentityStore.IdentityChangeEventArgs identityChangeEventArgs = (PlatformIdentityStore.IdentityChangeEventArgs) args;
                  int num2;
                  if (identityChangeEventArgs != null)
                  {
                    int? count = identityChangeEventArgs.MembershipChangeInfos?.Count;
                    int num3 = 1;
                    if (count.GetValueOrDefault() == num3 & count.HasValue)
                    {
                      MembershipChangeInfo membershipChangeInfo = identityChangeEventArgs.MembershipChangeInfos.ElementAt<MembershipChangeInfo>(0);
                      if (membershipChangeInfo == null)
                      {
                        num2 = 0;
                        goto label_7;
                      }
                      else
                      {
                        Guid memberId = membershipChangeInfo.MemberId;
                        num2 = 1;
                        goto label_7;
                      }
                    }
                  }
                  num2 = 0;
label_7:
                  if (num2 != 0)
                    this.m_alreadyInProcessTasksSet.Remove(identityChangeEventArgs.MembershipChangeInfos.ElementAt<MembershipChangeInfo>(0).MemberId);
                }
              });
              Guid instanceId = requestContext.ServiceHost.InstanceId;
              TeamFoundationTask task2 = new TeamFoundationTask(callback, (object) taskArgs, 0);
              service.AddTask(instanceId, task2);
            }
          }
        }
        List<Guid> groupScopeChangeIds = sqlNotification.GroupScopeChangeIds;
        // ISSUE: explicit non-virtual call
        if ((groupScopeChangeIds != null ? (__nonvirtual (groupScopeChangeIds.Count) > 0 ? 1 : 0) : 0) == 0)
          return;
        requestContext.TraceSerializedConditionally(80698, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Group scope change for scopeids : {0}", (object) sqlNotification.GroupScopeChangeIds);
        PlatformIdentityStore.ScopeChangeEventArgs taskArgs1 = new PlatformIdentityStore.ScopeChangeEventArgs()
        {
          RequestContext = requestContext,
          GroupScopeChangeIds = (ICollection<Guid>) sqlNotification.GroupScopeChangeIds
        };
        TeamFoundationTaskService service1 = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
        TeamFoundationTask teamFoundationTask1 = new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessScopeChangesTask), (object) taskArgs1, 0);
        Guid instanceId1 = requestContext.ServiceHost.InstanceId;
        TeamFoundationTask task3 = teamFoundationTask1;
        service1.AddTask(instanceId1, task3);
      }

      protected virtual void OnParentIdentityChangeEvent(
        object sender,
        PlatformIdentityStore.IdentityChangeEventArgs e)
      {
        try
        {
          lock (this.m_processChangesLock)
          {
            bool flag = true;
            if (e.DescriptorChangeType != DescriptorChangeType.None)
            {
              VssPerformanceCounter performanceCounter;
              if (this.m_parentDescriptorChangeIdsProcessed.Contains(e.IdentitySequenceId))
              {
                performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessParentIdentityChange.CallsDroppedAfterConfirmingDescriptorChangesAlreadyProcessed;
                performanceCounter.Increment();
                TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, "IdentityService", "IdentityChangeService", "Avoiding processing descriptor changes associated with parent sequence id {0}.", (object) e.IdentitySequenceId);
                flag = false;
              }
              if (e.IdentitySequenceId < this.m_parentDescriptorChangeIdsProcessed.Min)
              {
                performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessParentIdentityChange.CallsContinuedBecauseCreditLostForProcessingDescriptorChanges;
                performanceCounter.Increment();
                TeamFoundationTracingService.TraceRaw(0, TraceLevel.Warning, "IdentityService", "IdentityChangeService", "We may have lost credit for processing descriptor changes for identity sequence id {0}. m_descriptorChangesProcessed may limited to too few entries ({1}).", (object) e.IdentitySequenceId, (object) 100);
              }
            }
            if (!flag)
              return;
            this.ProcessParentIdentityChange(e.RequestContext, e.IdentitySequenceId, e.PropertyChangeIds, e.DescriptorChangeType, e.DescriptorChangeIds, e.MembershipChangeInfos);
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, "IdentityService", "IdentityChangeService", ex);
        }
      }

      private void ProcessDescriptorChangesTask(IVssRequestContext requestContext, object taskArgs)
      {
        PlatformIdentityStore.IdentityChangeEventArgs identityChangeEventArgs = (PlatformIdentityStore.IdentityChangeEventArgs) taskArgs;
        this.ProcessIdentityChange(requestContext, this.m_masterDomain, identityChangeEventArgs.IdentitySequenceId, -1, identityChangeEventArgs.DescriptorChangeType, identityChangeEventArgs.DescriptorChangeIds, identityChangeEventArgs.GroupScopeChangeIds, (ICollection<MembershipChangeInfo>) null, false);
      }

      private void ProcessScopeChangesTask(IVssRequestContext requestContext, object taskArgs)
      {
        PlatformIdentityStore.ScopeChangeEventArgs scopeChangeEventArgs = (PlatformIdentityStore.ScopeChangeEventArgs) taskArgs;
        this.ProcessIdentityChange(requestContext, this.m_masterDomain, -1, -1, DescriptorChangeType.None, (ICollection<Guid>) null, scopeChangeEventArgs.GroupScopeChangeIds, (ICollection<MembershipChangeInfo>) null, false);
      }

      private void ProcessChangesTask(IVssRequestContext requestContext, object taskArgs)
      {
        PlatformIdentityStore.IdentityChangeEventArgs identityChangeEventArgs = taskArgs as PlatformIdentityStore.IdentityChangeEventArgs;
        int identitySequenceId;
        int targetGroupSequenceId;
        using (requestContext.Lock(this.m_processChangesTaskLockName))
        {
          identitySequenceId = this.m_targetIdentitySequenceId;
          targetGroupSequenceId = this.m_targetGroupSequenceId;
          this.m_processChangesTaskState = IdentityStoreBase.TaskState.NotQueued;
        }
        this.ProcessIdentityChange(requestContext, this.m_masterDomain, identitySequenceId, targetGroupSequenceId, DescriptorChangeType.None, (ICollection<Guid>) null, (ICollection<Guid>) null, identityChangeEventArgs?.MembershipChangeInfos, false);
      }

      private void ProcessMembershipChangesTask(IVssRequestContext requestContext, object taskArgs)
      {
        PlatformIdentityStore.IdentityChangeEventArgs identityChangeEventArgs = (PlatformIdentityStore.IdentityChangeEventArgs) taskArgs;
        this.ProcessIdentityChange(requestContext, this.m_masterDomain, -1, -1, DescriptorChangeType.None, (ICollection<Guid>) null, (ICollection<Guid>) null, identityChangeEventArgs.MembershipChangeInfos, false);
      }

      protected virtual bool ApplyIdentityChange(
        IVssRequestContext requestContext,
        IdentityDomain hostDomain,
        int identitySequenceId,
        int groupSequenceId,
        DescriptorChangeType descriptorChangeType,
        ICollection<Guid> descriptorChangeIds,
        ICollection<Guid> groupScopeChangeIds,
        ICollection<MembershipChangeInfo> membershipChangeInfos,
        bool useTaskToBroadcast,
        out bool notifyDescriptorChange,
        out List<Guid> identityChangeIds,
        out List<Guid> groupChangeIds,
        out List<MembershipChangeInfo> membershipChanges)
      {
        requestContext.Trace(80707, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Entering ProcessIdentityChange");
        requestContext.Trace(80708, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "m_groupSequenceId: {0}, groupSequenceId: {1}", (object) this.m_groupSequenceId, (object) groupSequenceId);
        requestContext.Trace(80709, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "m_identitySequenceId: {0}, identitySequenceId: {1}", (object) this.m_identitySequenceId, (object) identitySequenceId);
        requestContext.Trace(80710, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Descriptor change type: {0}", (object) descriptorChangeType.ToString());
        identityChangeIds = new List<Guid>();
        groupChangeIds = new List<Guid>();
        membershipChanges = (List<MembershipChangeInfo>) null;
        notifyDescriptorChange = false;
        List<MembershipChangeInfo> membershipChangeInfoList = new List<MembershipChangeInfo>();
        if (identitySequenceId <= this.m_identitySequenceId && groupSequenceId <= this.m_groupSequenceId && descriptorChangeType == DescriptorChangeType.None && groupScopeChangeIds.IsNullOrEmpty<Guid>() && membershipChangeInfos.IsNullOrEmpty<MembershipChangeInfo>())
        {
          PlatformIdentityStore.TraceConditionally(requestContext, 80711, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", (Func<string>) (() => "No unknown changes to process"));
          PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsDroppedImmediatelyBecauseChangesAlreadyProcessed.Increment();
          return false;
        }
        bool flag1 = false;
        bool useIdentityAudit = IdentityStoreUtilities.IdentityAuditEnabled(requestContext);
        long firstAuditSequenceId = IdentityStoreUtilities.IdentityAuditFirstSequenceId(requestContext);
        requestContext.Trace(80712, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Identity Audit Enabled: {0}, firstIdentityAuditSequenceId: {1}", (object) useIdentityAudit, (object) firstAuditSequenceId);
        lock (this.m_processChangesLock)
        {
          bool flag2 = identitySequenceId > this.m_identitySequenceId;
          bool flag3 = groupSequenceId > this.m_groupSequenceId;
          bool flag4 = groupScopeChangeIds != null && groupScopeChangeIds.Count > 0;
          bool flag5 = !membershipChangeInfos.IsNullOrEmpty<MembershipChangeInfo>();
          requestContext.Trace(80713, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "ProcessIdentities: {0}, ProcessGroups: {1}, ProcessScopes: {2}, ProcessMembershipChanges: {3}", (object) flag2, (object) flag3, (object) flag4, (object) flag5);
          if (flag5)
            membershipChangeInfoList.AddRange((IEnumerable<MembershipChangeInfo>) membershipChangeInfos);
          VssPerformanceCounter performanceCounter;
          if (!flag2 && !flag3 && !flag4 && !flag5)
          {
            if (descriptorChangeType == DescriptorChangeType.None)
            {
              PlatformIdentityStore.TraceConditionally(requestContext, 80714, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", (Func<string>) (() => "Nothing to update - no descriptor changes"));
              PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsDroppedAfterConfirmingNoDescriptorChanges.Increment();
              return false;
            }
            if (this.m_descriptorChangeIdsProcessed.Contains(identitySequenceId))
            {
              PlatformIdentityStore.TraceConditionally(requestContext, 80715, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", (Func<string>) (() => "We already have credit for processing descriptor changes"));
              PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsDroppedAfterConfirmingDescriptorChangesAlreadyProcessed.Increment();
              return false;
            }
            if (identitySequenceId < this.m_descriptorChangeIdsProcessed.Min)
            {
              requestContext.Trace(80716, TraceLevel.Warning, "IdentityService", "IdentityChangeService", "We may have lost credit for processing descriptor changes for identity sequence id {0}. m_descriptorChangesProcessed may limited to too few entries ({1}).", (object) identitySequenceId, (object) 100);
              requestContext.Trace(80717, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "m_descriptorChangeIdsProcessed.Min: {0}", (object) this.m_descriptorChangeIdsProcessed.Min);
              performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsContinuedBecauseCreditLostForProcessingDescriptorChanges;
              performanceCounter.Increment();
            }
          }
          int lastSequenceId1 = 0;
          int groupSequenceId1 = 0;
          switch (descriptorChangeType)
          {
            case DescriptorChangeType.Minor:
              performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsWithMinorDescriptorChangeType;
              performanceCounter.Increment();
              break;
            case DescriptorChangeType.Major:
              performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsWithMajorDescriptorChangeType;
              performanceCounter.Increment();
              PlatformIdentityStore.TraceConditionally(requestContext, 80718, TraceLevel.Warning, "IdentityService", "IdentityChangeService", (Func<string>) (() => "Major descriptor changes. Invalidating the entire cache."));
              this.m_cache.Clear(requestContext);
              notifyDescriptorChange = true;
              goto label_42;
            default:
              if (!descriptorChangeIds.IsNullOrEmpty<Guid>())
              {
                performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsWithInvalidDescriptorChangeType;
                performanceCounter.Increment();
                requestContext.TraceSerializedConditionally(80023, TraceLevel.Error, "IdentityService", "IdentityChangeService", "ProcessIdentityChange was called with invalid descriptor change type: DomainId = {0}, identitySequenceId = {1}, groupSequenceId = {2}, descriptorChangeType = {3}, descriptorChangeIds = {4}, groupScopeChangeIds = {5}, useTaskToBroadcast = {6}", (object) hostDomain.DomainId, (object) identitySequenceId, (object) groupSequenceId, (object) descriptorChangeType, (object) descriptorChangeIds, (object) groupScopeChangeIds, (object) useTaskToBroadcast);
                break;
              }
              break;
          }
          if (flag2)
          {
            performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsWithReadsToIdentityComponentGetChanges;
            performanceCounter.Increment();
            using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
            {
              using (ResultCollection changes = identityComponent.GetChanges(this.m_identitySequenceId, ref lastSequenceId1, firstAuditSequenceId, useIdentityAudit))
                identityChangeIds = changes.GetCurrent<Guid>().Items;
            }
            requestContext.Trace(80727, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Called GetIdentityChanges. Received last sequence id: {0}", (object) lastSequenceId1);
          }
          if (flag3)
          {
            performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsWithReadsToGroupComponentGetChanges;
            performanceCounter.Increment();
            bool scopedGroupChanges = IdentityMembershipHelper.ShouldGetScopedGroupChanges(requestContext);
            Guid everyoneId;
            using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
            {
              long lastSequenceId2;
              using (ResultCollection changes = groupComponent.GetChanges((long) this.m_groupSequenceId, this.m_masterDomain.DomainRoot.Identifier, this.m_masterDomain.DomainId, out everyoneId, out lastSequenceId2, getScopedGroupChanges: scopedGroupChanges))
              {
                groupSequenceId1 = checked ((int) lastSequenceId2);
                membershipChanges = changes.GetCurrent<MembershipChangeInfo>().Items;
                changes.NextResult();
                groupChangeIds = changes.GetCurrent<Guid>().Items;
              }
            }
            membershipChangeInfoList.AddRange((IEnumerable<MembershipChangeInfo>) membershipChanges);
            PlatformIdentityStore.PlatformIdentityChangeProcessor.AugmentMembershipChangeInfo(requestContext, this.m_cache, hostDomain, membershipChanges);
            requestContext.Trace(80719, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Called GetGroupChanges. Received last sequence id: {0}, everyoneId: {1}", (object) groupSequenceId1, (object) everyoneId);
          }
          notifyDescriptorChange |= this.m_cache.ProcessChanges(requestContext, descriptorChangeIds, (ICollection<Guid>) null, (ICollection<Guid>) null, (ICollection<MembershipChangeInfo>) null, (ICollection<Guid>) null, (SequenceContext) null);
          this.m_cache.ProcessChanges(requestContext, (ICollection<Guid>) null, (ICollection<Guid>) identityChangeIds, (ICollection<Guid>) groupChangeIds, (ICollection<MembershipChangeInfo>) membershipChangeInfoList, groupScopeChangeIds, new SequenceContext(-1L, (long) groupSequenceId1));
          if (!membershipChangeInfoList.IsNullOrEmpty<MembershipChangeInfo>())
          {
            performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsWithMembershipChanges;
            performanceCounter.Increment();
            requestContext.GetService<IImsCacheService>().ProcessChanges(requestContext, hostDomain.DomainId, (ICollection<MembershipChangeInfo>) membershipChangeInfoList);
          }
          PlatformIdentitySearchHelper.ProcessChangesOnPlatformSearchCaches(requestContext, hostDomain.DomainId, (IList<MembershipChangeInfo>) membershipChangeInfoList);
label_42:
          if (useTaskToBroadcast)
          {
            performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsThatUsedTaskToBroadcast;
            performanceCounter.Increment();
            this.BroadcastChangesIfNecessary(requestContext, identitySequenceId, (ICollection<Guid>) identityChangeIds, descriptorChangeType, descriptorChangeIds, membershipChangeInfos, useTaskToBroadcast);
          }
          if (descriptorChangeType != DescriptorChangeType.None)
          {
            if (this.m_descriptorChangeIdsProcessed.Count >= 100)
              this.m_descriptorChangeIdsProcessed.Remove(this.m_descriptorChangeIdsProcessed.Min);
            this.m_descriptorChangeIdsProcessed.Add(identitySequenceId);
          }
          if (descriptorChangeType == DescriptorChangeType.Major)
          {
            flag1 = true;
            this.InitializeSequenceIds(requestContext, hostDomain);
          }
          else
          {
            if (flag2 && this.m_identitySequenceId != lastSequenceId1)
            {
              performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsThatUpdatedIdentitySequenceId;
              performanceCounter.Increment();
              flag1 = true;
              this.m_identitySequenceId = lastSequenceId1;
              requestContext.Trace(80724, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Identity changes up through {0} processed", (object) this.m_identitySequenceId);
            }
            if (flag3 && this.m_groupSequenceId != groupSequenceId1)
            {
              performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsThatUpdatedGroupSequenceId;
              performanceCounter.Increment();
              flag1 = true;
              this.m_groupSequenceId = groupSequenceId1;
              requestContext.Trace(80725, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Group changes up through {0} processed", (object) this.m_groupSequenceId);
            }
            if (flag4)
            {
              flag1 = true;
              requestContext.TraceSerializedConditionally(80699, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Invalidating cache for scopes : {0}", (object) groupScopeChangeIds);
            }
            if (flag5)
            {
              performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsWithUserMembershipChangesPerSecond;
              performanceCounter.Increment();
              flag1 = true;
              requestContext.TraceConditionally(80742, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", (Func<string>) (() => "Invalidating cache for membership change for Members IDs : " + string.Join(", ", membershipChangeInfos.Select<MembershipChangeInfo, string>((Func<MembershipChangeInfo, string>) (membership => membership?.MemberId.ToString() ?? "")))));
            }
          }
          if (flag1)
          {
            performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsThatProcessedChanges;
            performanceCounter.Increment();
            Interlocked.Increment(ref this.m_changeId);
          }
          else
          {
            performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsThatDidNotProcessChanges;
            performanceCounter.Increment();
            PlatformIdentityStore.TraceConditionally(requestContext, 80726, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", (Func<string>) (() => "No changes processed."));
          }
        }
        return flag1;
      }

      protected bool ProcessIdentityChange(
        IVssRequestContext requestContext,
        IdentityDomain hostDomain,
        int identitySequenceId,
        int groupSequenceId,
        DescriptorChangeType descriptorChangeType,
        ICollection<Guid> descriptorChangeIds,
        ICollection<Guid> groupScopeChangeIds,
        ICollection<MembershipChangeInfo> membershipChangeInfos,
        bool useTaskToBroadcast)
      {
        PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsPerSecond.Increment();
        bool notifyDescriptorChange = false;
        List<Guid> identityChangeIds = (List<Guid>) null;
        List<Guid> groupChangeIds = (List<Guid>) null;
        List<MembershipChangeInfo> membershipChanges1 = (List<MembershipChangeInfo>) null;
        bool flag = this.ApplyIdentityChange(requestContext, hostDomain, identitySequenceId, groupSequenceId, descriptorChangeType, descriptorChangeIds, groupScopeChangeIds, membershipChangeInfos, useTaskToBroadcast, out notifyDescriptorChange, out identityChangeIds, out groupChangeIds, out membershipChanges1);
        if (descriptorChangeType != DescriptorChangeType.None)
        {
          EventHandler<DescriptorChangeNotificationEventArgs> eventHandler = this.GetDescriptorsChangedNotificationEvent();
          if (eventHandler != null)
          {
            DescriptorChangeNotificationEventArgs e = new DescriptorChangeNotificationEventArgs(requestContext, descriptorChangeType, descriptorChangeIds);
            eventHandler((object) this, e);
          }
        }
        if (notifyDescriptorChange)
        {
          PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsThatNotifiedDescriptorChange.Increment();
          EventHandler<DescriptorChangeEventArgs> eventHandler = this.GetDescriptorsChangedEvent();
          if (eventHandler != null)
          {
            DescriptorChangeEventArgs e = new DescriptorChangeEventArgs(requestContext);
            eventHandler((object) this, e);
          }
        }
        ITeamFoundationEventService service1 = requestContext.GetService<ITeamFoundationEventService>();
        AfterProcessMembershipChangesOnStoreEvent notificationEvent1 = new AfterProcessMembershipChangesOnStoreEvent()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          InputMembershipChanges = membershipChanges1,
          IsAuthorOfChange = useTaskToBroadcast
        };
        VssPerformanceCounter performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsThatPublishedAfterProcessMembershipChangesOnStoreEvent;
        performanceCounter.Increment();
        service1.PublishDecisionPoint(requestContext, (object) notificationEvent1);
        if (notifyDescriptorChange && !descriptorChangeIds.IsNullOrEmpty<Guid>() && !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.PublishIdentityChangedDataEvent.Disable"))
        {
          IdentityChangedData notificationEvent2 = new IdentityChangedData()
          {
            DescriptorChanges = descriptorChangeIds.Select<Guid, DescriptorChange>((Func<Guid, DescriptorChange>) (x => new DescriptorChange()
            {
              MasterId = x
            })).ToArray<DescriptorChange>(),
            DescriptorChangeType = descriptorChangeType
          };
          service1.PublishNotification(requestContext, (object) notificationEvent2);
        }
        TeamFoundationExecutionEnvironment executionEnvironment;
        if (!notificationEvent1.OutputMembershipChanges.IsNullOrEmpty<MembershipChangeInfo>())
        {
          List<MembershipChangeInfo> membershipChanges2 = notificationEvent1.OutputMembershipChanges;
          this.m_cache.ProcessChanges(requestContext, (ICollection<Guid>) null, (ICollection<Guid>) null, (ICollection<Guid>) null, (ICollection<MembershipChangeInfo>) membershipChanges2, (ICollection<Guid>) null, (SequenceContext) null);
          executionEnvironment = requestContext.ExecutionEnvironment;
          if (executionEnvironment.IsHostedDeployment)
            requestContext.GetService<IImsCacheService>().ProcessChanges(requestContext, hostDomain.DomainId, (ICollection<MembershipChangeInfo>) membershipChanges2);
        }
        if (!useTaskToBroadcast)
        {
          performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsThatDidNotUseTaskToBroadcast;
          performanceCounter.Increment();
          this.BroadcastChangesIfNecessary(requestContext, identitySequenceId, (ICollection<Guid>) identityChangeIds, descriptorChangeType, descriptorChangeIds, membershipChangeInfos, useTaskToBroadcast);
        }
        else if (flag)
        {
          executionEnvironment = requestContext.ExecutionEnvironment;
          if (executionEnvironment.IsHostedDeployment)
          {
            IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
            performanceCounter = PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessIdentityChange.CallsThatQueuedIdentityChangePublisherJob;
            performanceCounter.Increment();
            CachedRegistryService service2 = vssRequestContext.GetService<CachedRegistryService>();
            vssRequestContext.Trace(80049, TraceLevel.Info, "IdentityService", "IdentityChangeService", "ProcessIdentityChange: Queuing identity change publisher job.");
            vssRequestContext.GetService<TeamFoundationJobService>().QueueDelayedJobs(vssRequestContext, (IEnumerable<Guid>) new Guid[1]
            {
              PlatformIdentityStore.PlatformIdentityChangeProcessor.m_MessageBusIdentityChangePublisherJobId
            }, service2.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Identity/Settings/IdentityChangePublisherJobQueueDelayInSeconds", 5), service2.GetValue<JobPriorityLevel>(vssRequestContext, (RegistryQuery) "/Service/Identity/Settings/IdentityChangePublisherJobQueuePriority", JobPriorityLevel.Highest));
          }
        }
        return flag;
      }

      protected virtual void ApplyParentIdentityChange(
        IVssRequestContext requestContext,
        int parentIdentitySequenceId,
        ICollection<Guid> propertyChangeIds,
        DescriptorChangeType descriptorChangeType,
        ICollection<Guid> descriptorChangeIds,
        ICollection<MembershipChangeInfo> membershipChangeInfos,
        out bool notifyDescriptorChange)
      {
        notifyDescriptorChange = false;
        lock (this.m_processChangesLock)
        {
          switch (descriptorChangeType)
          {
            case DescriptorChangeType.Minor:
              PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessParentIdentityChange.CallsWithMinorDescriptorChangeType.Increment();
              break;
            case DescriptorChangeType.Major:
              PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessParentIdentityChange.CallsWithMajorDescriptorChangeType.Increment();
              TeamFoundationTracingService.TraceRaw(0, TraceLevel.Warning, "IdentityService", "IdentityChangeService", "Major descriptor changes. Invalidating the entire cache.");
              this.m_cache.Clear(requestContext);
              requestContext.Trace(80030, TraceLevel.Info, "IdentityService", "IdentityChangeService", "Identity cache cleared -  parent identity sequence id: {0}, # of propertychangeids: {1}, # of descriptorchangeids: {2}", (object) parentIdentitySequenceId, (object) (propertyChangeIds != null ? propertyChangeIds.Count : 0), (object) (descriptorChangeIds != null ? descriptorChangeIds.Count : 0));
              notifyDescriptorChange = true;
              goto label_8;
            default:
              if (!descriptorChangeIds.IsNullOrEmpty<Guid>())
              {
                PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessParentIdentityChange.CallsWithInvalidDescriptorChangeType.Increment();
                requestContext.TraceSerializedConditionally(80024, TraceLevel.Error, "IdentityService", "IdentityChangeService", "ProcessParentIdentityChange was called with invalid descriptor change type: parentIdentitySequenceId = {0}, propertyChangeIds = {1}, descriptorChangeType = {2}, descriptorChangeIds = {3}", (object) parentIdentitySequenceId, (object) propertyChangeIds, (object) descriptorChangeType, (object) descriptorChangeIds);
                break;
              }
              break;
          }
          notifyDescriptorChange = this.m_cache.ProcessChanges(requestContext, descriptorChangeIds, (ICollection<Guid>) null, (ICollection<Guid>) null, (ICollection<MembershipChangeInfo>) null, (ICollection<Guid>) null, (SequenceContext) null);
          this.m_cache.ProcessChanges(requestContext, (ICollection<Guid>) null, propertyChangeIds, (ICollection<Guid>) null, membershipChangeInfos, (ICollection<Guid>) null, (SequenceContext) null);
          if (requestContext.IsTracing(80031, TraceLevel.Verbose, "IdentityService", "IdentityChangeService"))
            requestContext.Trace(80031, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", string.Format("Identity cache invalidated - parent identity sequence id: {0}, # of propertychangeids: {1}, # of descriptorchangeids: {2}, # of membershipChangeInfos: {3}", (object) parentIdentitySequenceId, (object) (propertyChangeIds != null ? propertyChangeIds.Count : 0), (object) (descriptorChangeIds != null ? descriptorChangeIds.Count : 0), (object) (membershipChangeInfos != null ? membershipChangeInfos.Count : 0)));
label_8:
          if (descriptorChangeType != DescriptorChangeType.None)
          {
            if (this.m_parentDescriptorChangeIdsProcessed.Count >= 100)
              this.m_parentDescriptorChangeIdsProcessed.Remove(this.m_parentDescriptorChangeIdsProcessed.Min);
            this.m_parentDescriptorChangeIdsProcessed.Add(parentIdentitySequenceId);
          }
          Interlocked.Increment(ref this.m_changeId);
        }
      }

      protected void ProcessParentIdentityChange(
        IVssRequestContext requestContext,
        int parentIdentitySequenceId,
        ICollection<Guid> propertyChangeIds,
        DescriptorChangeType descriptorChangeType,
        ICollection<Guid> descriptorChangeIds,
        ICollection<MembershipChangeInfo> membershipChangeInfos)
      {
        PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessParentIdentityChange.CallsPerSecond.Increment();
        bool notifyDescriptorChange = false;
        this.ApplyParentIdentityChange(requestContext, parentIdentitySequenceId, propertyChangeIds, descriptorChangeType, descriptorChangeIds, membershipChangeInfos, out notifyDescriptorChange);
        if (descriptorChangeType != DescriptorChangeType.None)
        {
          EventHandler<DescriptorChangeNotificationEventArgs> eventHandler = this.GetDescriptorsChangedNotificationEvent();
          if (eventHandler != null)
          {
            DescriptorChangeNotificationEventArgs e = new DescriptorChangeNotificationEventArgs(requestContext, descriptorChangeType, descriptorChangeIds);
            eventHandler((object) this, e);
          }
        }
        if (!notifyDescriptorChange)
          return;
        PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.ProcessParentIdentityChange.CallsThatNotifiedDescriptorChange.Increment();
        EventHandler<DescriptorChangeEventArgs> eventHandler1 = this.GetDescriptorsChangedEvent();
        if (eventHandler1 == null)
          return;
        DescriptorChangeEventArgs e1 = new DescriptorChangeEventArgs(requestContext);
        eventHandler1((object) this, e1);
      }

      protected void BroadcastChangesIfNecessary(
        IVssRequestContext requestContext,
        int identitySequenceId,
        ICollection<Guid> identityChangeIds,
        DescriptorChangeType descriptorChangeType,
        ICollection<Guid> descriptorChangeIds,
        ICollection<MembershipChangeInfo> membershipChangeInfos,
        bool useTaskToBroadcast)
      {
        if (descriptorChangeType == DescriptorChangeType.None && (identityChangeIds == null || identityChangeIds.Count <= 0) && membershipChangeInfos.IsNullOrEmpty<MembershipChangeInfo>())
          return;
        IIdentityEventHandler<PlatformIdentityStore.IdentityChangeEventArgs> identityEventHandler1 = this.GetIdentitiesChangedEvent();
        if (identityEventHandler1 == null || !identityEventHandler1.ContainsEvents())
          return;
        PlatformIdentityStore.IdentityChangeEventArgs identityChangeEventArgs = new PlatformIdentityStore.IdentityChangeEventArgs()
        {
          RequestContext = requestContext,
          IdentitySequenceId = identitySequenceId,
          PropertyChangeIds = identityChangeIds,
          DescriptorChangeType = descriptorChangeType,
          DescriptorChangeIds = descriptorChangeIds,
          MembershipChangeInfos = membershipChangeInfos
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
          if (requestContext.IsTracing(80066, TraceLevel.Info, "IdentityService", "IdentityChangeService") && identityEventHandler1 is IDictionaryStoredIdentityEventHandler<Guid, PlatformIdentityStore.IdentityChangeEventArgs> identityEventHandler2)
            requestContext.Trace(80066, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "DictionaryStoredIdentityEventHandler is going to process {0} events", (object) identityEventHandler2.Count);
          try
          {
            identityEventHandler1.RaiseEvents((object) this, identityChangeEventArgs);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "IdentityService", "IdentityChangeService", ex);
          }
        }
      }

      private void BroadcastIdentityChanges(IVssRequestContext requestContext, object taskArgs)
      {
        PlatformIdentityStore.IdentityChangeEventArgs eventArgs = (PlatformIdentityStore.IdentityChangeEventArgs) taskArgs;
        eventArgs.RequestContext = requestContext;
        IIdentityEventHandler<PlatformIdentityStore.IdentityChangeEventArgs> identityEventHandler1 = this.GetIdentitiesChangedEvent();
        if (identityEventHandler1 == null || !identityEventHandler1.ContainsEvents())
          return;
        if (requestContext.IsTracing(80065, TraceLevel.Info, "IdentityService", "IdentityChangeService") && identityEventHandler1 is IDictionaryStoredIdentityEventHandler<Guid, PlatformIdentityStore.IdentityChangeEventArgs> identityEventHandler2)
          requestContext.Trace(80065, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "DictionaryStoredIdentityEventHandler is going to process {0} events", (object) identityEventHandler2.Count);
        try
        {
          identityEventHandler1.RaiseEvents((object) this, eventArgs);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, "IdentityService", "IdentityChangeService", ex);
        }
      }

      private void FireEvents(IVssRequestContext requestContext, int sequenceId) => requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new IdentityChangedNotification(sequenceId));

      private static void SendMembershipChangesSqlNotification(
        IVssRequestContext requestContext,
        IEnumerable<MembershipChangeInfo> membershipChangeInfos)
      {
        requestContext.CheckDeploymentRequestContext();
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) membershipChangeInfos, nameof (membershipChangeInfos));
        string eventData = TeamFoundationSerializationUtility.SerializeToString<IdentityChangedData>(new IdentityChangedData()
        {
          MembershipChanges = membershipChangeInfos.ToArray<MembershipChangeInfo>()
        });
        requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.IMS2PlatformIdentityChanged, eventData);
      }

      private Func<EventHandler<DescriptorChangeEventArgs>> GetDescriptorsChangedEvent { get; }

      private Func<EventHandler<DescriptorChangeNotificationEventArgs>> GetDescriptorsChangedNotificationEvent { get; }

      internal Action<long> UpdateGroupSequenceId { get; }

      protected Func<IIdentityEventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> GetIdentitiesChangedEvent { get; }

      protected Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> AddParentIdentitiesChangedHandler { get; }

      protected Action<EventHandler<PlatformIdentityStore.IdentityChangeEventArgs>> RemoveParentIdentitiesChangedHandler { get; }

      protected static class PerfCounters
      {
        internal static class Load
        {
          internal static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.Load.CallsPerSecond");
        }

        internal static class Unload
        {
          internal static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.Unload.CallsPerSecond");
        }

        internal static class InitializeSequenceIds
        {
          internal static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.InitializeSequenceIds.CallsPerSecond");
        }

        internal static class ProcessIdentityChange
        {
          internal static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsPerSecond");
          internal static readonly VssPerformanceCounter CallsDroppedImmediatelyBecauseChangesAlreadyProcessed = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsDroppedImmediatelyBecauseChangesAlreadyProcessed");
          internal static readonly VssPerformanceCounter CallsDroppedAfterConfirmingNoDescriptorChanges = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsDroppedAfterConfirmingNoDescriptorChanges");
          internal static readonly VssPerformanceCounter CallsDroppedAfterConfirmingDescriptorChangesAlreadyProcessed = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsDroppedAfterConfirmingDescriptorChangesAlreadyProcessed");
          internal static readonly VssPerformanceCounter CallsContinuedBecauseCreditLostForProcessingDescriptorChanges = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsContinuedBecauseCreditLostForProcessingDescriptorChanges");
          internal static readonly VssPerformanceCounter CallsWithMajorDescriptorChangeType = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithMajorDescriptorChangeType");
          internal static readonly VssPerformanceCounter CallsWithMinorDescriptorChangeType = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithMinorDescriptorChangeType");
          internal static readonly VssPerformanceCounter CallsWithInvalidDescriptorChangeType = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithInvalidDescriptorChangeType");
          internal static readonly VssPerformanceCounter CallsWithReadsToIdentityComponentGetChanges = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithReadsToIdentityComponentGetChanges");
          internal static readonly VssPerformanceCounter CallsWithReadsToGroupComponentGetChanges = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithReadsToGroupComponentGetChanges");
          internal static readonly VssPerformanceCounter CallsWithMembershipChanges = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithMembershipChanges");
          internal static readonly VssPerformanceCounter CallsThatUsedTaskToBroadcast = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatUsedTaskToBroadcast");
          internal static readonly VssPerformanceCounter CallsThatDidNotUseTaskToBroadcast = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatDidNotUseTaskToBroadcast");
          internal static readonly VssPerformanceCounter CallsThatUpdatedIdentitySequenceId = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatUpdatedIdentitySequenceId");
          internal static readonly VssPerformanceCounter CallsThatUpdatedGroupSequenceId = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatUpdatedGroupSequenceId");
          internal static readonly VssPerformanceCounter CallsWithUserMembershipChangesPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithUserMembershipChangesPerSecond");
          internal static readonly VssPerformanceCounter CallsThatProcessedChanges = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatProcessedChanges");
          internal static readonly VssPerformanceCounter CallsThatDidNotProcessChanges = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatDidNotProcessChanges");
          internal static readonly VssPerformanceCounter CallsThatNotifiedDescriptorChange = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatNotifiedDescriptorChange");
          internal static readonly VssPerformanceCounter CallsThatPublishedAfterProcessMembershipChangesOnStoreEvent = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatPublishedAfterProcessMembershipChangesOnStoreEvent");
          internal static readonly VssPerformanceCounter CallsThatQueuedIdentityChangePublisherJob = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatQueuedIdentityChangePublisherJob");
        }

        internal static class ProcessParentIdentityChange
        {
          internal static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsPerSecond");
          internal static readonly VssPerformanceCounter CallsDroppedAfterConfirmingDescriptorChangesAlreadyProcessed = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsDroppedAfterConfirmingDescriptorChangesAlreadyProcessed");
          internal static readonly VssPerformanceCounter CallsContinuedBecauseCreditLostForProcessingDescriptorChanges = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsContinuedBecauseCreditLostForProcessingDescriptorChanges");
          internal static readonly VssPerformanceCounter CallsWithMajorDescriptorChangeType = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsWithMajorDescriptorChangeType");
          internal static readonly VssPerformanceCounter CallsWithMinorDescriptorChangeType = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsWithMinorDescriptorChangeType");
          internal static readonly VssPerformanceCounter CallsWithInvalidDescriptorChangeType = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsWithInvalidDescriptorChangeType");
          internal static readonly VssPerformanceCounter CallsThatNotifiedDescriptorChange = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsThatNotifiedDescriptorChange");
        }
      }

      internal interface ISequenceIDUtility
      {
        void InitializeSequenceIds(
          IVssRequestContext requestContext,
          IdentityDomain hostDomain,
          IdentityDomain masterDomain,
          ref int identitySequenceId,
          out int groupSequenceId);
      }

      internal class SequenceIDUtility : 
        PlatformIdentityStore.PlatformIdentityChangeProcessor.ISequenceIDUtility
      {
        void PlatformIdentityStore.PlatformIdentityChangeProcessor.ISequenceIDUtility.InitializeSequenceIds(
          IVssRequestContext requestContext,
          IdentityDomain hostDomain,
          IdentityDomain masterDomain,
          ref int identitySequenceId,
          out int groupSequenceId)
        {
          bool useIdentityAudit = IdentityStoreUtilities.IdentityAuditEnabled(requestContext);
          long firstAuditSequenceId = IdentityStoreUtilities.IdentityAuditFirstSequenceId(requestContext);
          PlatformIdentityStore.PlatformIdentityChangeProcessor.PerfCounters.InitializeSequenceIds.CallsPerSecond.Increment();
          requestContext.Trace(80720, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Enter InitializeSequenceIds");
          requestContext.Trace(80721, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "Identity Audit Enabled: {0}, firstIdentityAuditSequenceId: {1}", (object) useIdentityAudit, (object) firstAuditSequenceId);
          using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(requestContext))
          {
            using (identityComponent.GetChanges(int.MaxValue, ref identitySequenceId, firstAuditSequenceId, useIdentityAudit))
              ;
          }
          requestContext.Trace(80722, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "identitySequenceId returned as {0}", (object) identitySequenceId);
          using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
            groupSequenceId = checked ((int) groupComponent.GetLatestGroupSequenceId(masterDomain.DomainRoot.Identifier, masterDomain.DomainId));
          requestContext.Trace(80723, TraceLevel.Verbose, "IdentityService", "IdentityChangeService", "groupSequenceId returned as {0}", (object) groupSequenceId);
        }
      }
    }
  }
}
