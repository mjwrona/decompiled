// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PlatformIdentityService
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Common.Utility;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity.Events;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class PlatformIdentityService : 
    IdentityServiceBase,
    IInstallableIdentityService,
    IAuditableIdentityService,
    ITransferIdentityRightsService,
    IVssFrameworkService,
    IPlatformIdentityServiceInternal,
    IGraphMembershipChangeHandler,
    ISwapIdentityService,
    ILegacySpsIdentityServiceInternal,
    IdentityService,
    IUserIdentityService,
    ISystemIdentityService,
    IPagedScopedIdentityReader
  {
    private const string s_Area = "IdentityService";
    private const string s_Layer = "BusinessLogic";
    private const string s_AccountLinkingArea = "AccountLinking";
    private const string s_AccountLinkingLayer = "IdentityService";
    private const int c_filterReadResultsShortcut = 20;
    private const int c_defaultReadIdentitiesBatchSize = 1000;
    private const string s_readIdentitesBatchSizeRegistryKey = "/Service/Identity/ReadIdentitiesBatchSizeKey";
    private int m_readIdentitiesBatchSize;
    private static readonly string s_everyoneGroupSuffix = GroupWellKnownSidConstants.EveryoneGroupSid.Substring(SidIdentityHelper.WellKnownDomainSid.Length);
    internal const string s_featureNameSkipAssigningDefaultAceForScopes = "VisualStudio.Services.Identity.SkipAssigningDefaultAceForScopes";
    internal const string s_useSecurityNamespaceExtensionToComputePermissions = "VisualStudio.Services.Identity.UseSecurityNamespaceExtensionToComputePermissions";
    private const string s_featureNameGetIdentitiesWhenGettingAuditRecords = "VisualStudio.Services.Identity.ReadIdentitiesWhenRetrievingAuditRecords";
    private const string s_featureNameProfileAvatarDisabled = "Microsoft.VisualStudio.Services.Identity.ProfileAvatar.Disabled";
    private const string s_featureNameProfileAvatarCallsTraceEnabled = "Microsoft.VisualStudio.Services.Identity.ProfileAvatarCallsTrace.Enabled";
    private const string s_enableCreateIdentityForAadGroupFeatureFlag = "VisualStudio.Services.Identity.EnableCreateIdentityForAadGroup";
    private const string s_featureNameDisableRemoveMemberFromGroupExtensions = "VisualStudio.Services.Identity.RemoveMemberFromGroupExtensions.Disable";
    private const string s_featureNameDisableSplitCollectionAndOrganizationAdminGroupsServicing = "Microsoft.VisualStudio.Services.Identity.DisableSplitCollectionAndOrganizationAdminGroupsServicing";
    private const string FeatureHandleTranslationsForHistoricalIdentityUpgrade = "VisualStudio.Services.Identity.UpdateTranslationWhenUpgradingHistoricalIdentity";
    private const string s_featureNameDisableScopeIdMatchCheck = "VisualStudio.Services.Identity.DisableScopeIdMatchCheck";
    private const string s_featureNameForceRemoveMemberFromGroup = "VisualStudio.Services.Identity.ForceRemoveMemberFromGroup";
    private const string c_createAadGroupWithActiveScopeMembership = "VisualStudio.Services.Identity.CreateAadGroupWithActiveScopeMembership";
    private const string s_featureNameUsersCannotCreateGroupsWithWellKnownNames = "VisualStudio.Services.Identity.UsersCannotCreateGroupsWithWellKnownNames";
    private const string IdentityPropertyKpiPublishTime = "/Service/Identity/IdentityPropertyKpiPublishTime";
    private static readonly TimeSpan IdentityPropertyKpiPublishTimeDefault = TimeSpan.FromMinutes(30.0);
    private TeamFoundationTask m_cacheWarmerTask;
    private static readonly IdentityDescriptor AnonymousUsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.AnonymousUsersGroup);
    private static readonly VssPerformanceCounter s_readAncestorMembershipsCallsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.Calls");
    private static readonly VssPerformanceCounter s_readAncestorMembershipsCallsPerSecondCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CallsPerSecond");
    private static readonly VssPerformanceCounter s_readAncestorMembershipsCacheHitsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheHits");
    private static readonly VssPerformanceCounter s_readAncestorMembershipsCacheHitsPerSecondCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheHitsPerSecond");
    private static readonly VssPerformanceCounter s_readAncestorMembershipsCacheMissesCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheMisses");
    private static readonly VssPerformanceCounter s_readAncestorMembershipsCacheMissesPerSecondCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheMissesPerSecond");
    private static readonly VssPerformanceCounter s_readAncestorMembershipsCacheAdditionsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheAdditions");
    private static readonly VssPerformanceCounter s_readAncestorMembershipsCacheAdditionsPerSecondCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheAdditionsPerSecond");

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment && systemRequestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS)
      {
        systemRequestContext.Trace(80123, TraceLevel.Error, "IdentityService", "BusinessLogic", "A non SPS service attempted to start a PlatformIdentityService instance.  ServiceInstanceType: [{0}]  Stack Trace: {1}", (object) systemRequestContext.ServiceInstanceType(), (object) Environment.StackTrace);
        throw new InvalidOperationException("PlatformIdentityService cannot be instantiated from non-SPS services.");
      }
      base.ServiceStart(systemRequestContext);
      PlatformIdentityStore parentIdentityStore = (PlatformIdentityStore) null;
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        parentIdentityStore = systemRequestContext.To(TeamFoundationHostType.Parent).GetService<PlatformIdentityService>().IdentityStore;
      }
      else
      {
        TimeSpan timeSpan = systemRequestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(systemRequestContext, (RegistryQuery) "/Service/Identity/IdentityPropertyKpiPublishTime", PlatformIdentityService.IdentityPropertyKpiPublishTimeDefault);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        systemRequestContext.GetService<ITeamFoundationTaskService>().AddTask(systemRequestContext, new TeamFoundationTask(PlatformIdentityService.\u003C\u003EO.\u003C0\u003E__PublishKpis ?? (PlatformIdentityService.\u003C\u003EO.\u003C0\u003E__PublishKpis = new TeamFoundationTaskCallback(IdentityPropertyKpis.PublishKpis)), (object) null, DateTime.UtcNow.Add(timeSpan), (int) timeSpan.TotalMilliseconds));
      }
      IVssRegistryService service1 = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      IVssRequestContext requestContext1 = systemRequestContext.To(TeamFoundationHostType.Deployment);
      RegistryQuery registryQuery = (RegistryQuery) "/Service/Identity/ReadIdentitiesBatchSizeKey";
      ref RegistryQuery local1 = ref registryQuery;
      this.m_readIdentitiesBatchSize = service1.GetValue<int>(requestContext1, in local1, 1000);
      if (this.Domain.IsMaster)
      {
        this.IdentityStore = new PlatformIdentityStore(systemRequestContext, this.Domain, this.SyncAgents, parentIdentityStore);
        this.IdentityStore.DescriptorsChanged += new EventHandler<DescriptorChangeEventArgs>(((IdentityServiceBase) this).FireDescriptorsChanged);
        this.IdentityStore.DescriptorsChangedNotification += new EventHandler<DescriptorChangeNotificationEventArgs>(((IdentityServiceBase) this).FireDescriptorsChangedNotification);
      }
      else
      {
        this.IdentityStore = parentIdentityStore;
        this.IdentityStore.AddDomain(systemRequestContext, this.Domain);
      }
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || !systemRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache.Warmup"))
        return;
      IVssRegistryService service2 = systemRequestContext.GetService<IVssRegistryService>();
      IVssRequestContext requestContext2 = systemRequestContext;
      registryQuery = (RegistryQuery) "/Configuration/Identity/CacheWarmupTask/ScheduleInterval";
      ref RegistryQuery local2 = ref registryQuery;
      TimeSpan scheduleIntervalDefault = IdentityCacheWarmer.ScheduleIntervalDefault;
      TimeSpan timeSpan1 = service2.GetValue<TimeSpan>(requestContext2, in local2, scheduleIntervalDefault);
      ITeamFoundationTaskService service3 = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.m_cacheWarmerTask = new TeamFoundationTask(PlatformIdentityService.\u003C\u003EO.\u003C1\u003E__WarmUpExpandedDownCaches ?? (PlatformIdentityService.\u003C\u003EO.\u003C1\u003E__WarmUpExpandedDownCaches = new TeamFoundationTaskCallback(IdentityCacheWarmer.WarmUpExpandedDownCaches)), (object) null, (int) timeSpan1.TotalMilliseconds);
      IVssRequestContext requestContext3 = systemRequestContext;
      TeamFoundationTask cacheWarmerTask = this.m_cacheWarmerTask;
      service3.AddTask(requestContext3, cacheWarmerTask);
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      base.ServiceEnd(systemRequestContext);
      if (this.IdentityStore != null)
      {
        if (this.Domain.IsMaster)
        {
          this.IdentityStore.DescriptorsChanged -= new EventHandler<DescriptorChangeEventArgs>(((IdentityServiceBase) this).FireDescriptorsChanged);
          this.IdentityStore.DescriptorsChangedNotification -= new EventHandler<DescriptorChangeNotificationEventArgs>(((IdentityServiceBase) this).FireDescriptorsChangedNotification);
          this.IdentityStore.Unload(systemRequestContext);
        }
        else
          this.IdentityStore.RemoveDomain(systemRequestContext, this.Domain);
      }
      if (this.m_cacheWarmerTask == null)
        return;
      systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().RemoveTask(systemRequestContext, this.m_cacheWarmerTask);
    }

    void IInstallableIdentityService.Install(IVssRequestContext requestContext)
    {
      bool flag1 = requestContext.ServiceHost.Is(TeamFoundationHostType.Application);
      bool flag2 = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection);
      IdentityDescriptor identityDescriptor1 = this.Domain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
      IdentityDescriptor descriptor = this.Domain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.ServiceUsersGroup);
      IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
      WellKnownGroupsInfo wellKnownGroupsInfo = new WellKnownGroupsInfo(requestContext);
      this.IdentityStore.Install(requestContext, this.Domain, securityContext.ServiceHost.InstanceId, this.Domain.DomainRoot.Identifier, wellKnownGroupsInfo.ValidUsersGroupName, wellKnownGroupsInfo.ValidUsersGroupDescription, identityDescriptor1.Identifier, wellKnownGroupsInfo.AdministratorsGroupName, wellKnownGroupsInfo.AdministratorsGroupDescription, true);
      this.IdentityStore.CreateGroups(requestContext, this.Domain, this.Domain.DomainId, false, true, new GroupDescription(descriptor, wellKnownGroupsInfo.ServiceAccountsGroupName, wellKnownGroupsInfo.ServiceAccountsGroupDescription, SpecialGroupType.ServiceApplicationGroup, scopeLocal: true));
      if (requestContext.ExecutionEnvironment.IsHostedDeployment & flag2)
        this.CreateGroup(requestContext, this.Domain.DomainId, GroupWellKnownSidConstants.LicensedUsersGroupSid, FrameworkResources.LicenseeGroupName(), FrameworkResources.LicenseeGroupDescription(), SpecialGroupType.Generic, true, true);
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity1 = this.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, QueryMembership.None, (IEnumerable<string>) null, false)[0];
      List<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>> additions = new List<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>();
      if (this.Domain.IsMaster)
      {
        additions.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(identityDescriptor1, readIdentity1));
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          IdentityDescriptor windowsDescriptor = IdentityHelper.CreateWindowsDescriptor(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, (SecurityIdentifier) null));
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentitiesFromSource(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            windowsDescriptor
          }, QueryMembership.None)[0];
          additions.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(identityDescriptor1, identity));
        }
        else if (flag1)
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          RegistryEntryCollection source = vssRequestContext.GetService<CachedRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) HighTrustIdentitiesRegistryConstants.Wildcard);
          IdentityService service = vssRequestContext.GetService<IdentityService>();
          IList<IdentityDescriptor> list = (IList<IdentityDescriptor>) source.Select<RegistryEntry, IdentityDescriptor>((Func<RegistryEntry, IdentityDescriptor>) (x => new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", x.Value))).ToList<IdentityDescriptor>();
          foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity2 in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(vssRequestContext, list, QueryMembership.None, (IEnumerable<string>) null))
          {
            if (readIdentity2 != null)
              additions.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(readIdentity1.Descriptor, readIdentity2));
          }
        }
      }
      else
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Parent);
        IdentityService service = vssRequestContext.GetService<IdentityService>();
        if (requestContext.ExecutionEnvironment.IsHostedDeployment & flag2)
        {
          IList<IdentityDescriptor> descriptors = (IList<IdentityDescriptor>) new IdentityDescriptor[3]
          {
            GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup,
            GroupWellKnownIdentityDescriptors.ServiceUsersGroup,
            GroupWellKnownIdentityDescriptors.InvitedUsersGroup
          };
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(vssRequestContext, descriptors, QueryMembership.None, (IEnumerable<string>) null);
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.DisableSplitCollectionAndOrganizationAdminGroupsServicing"))
            additions.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(identityDescriptor1, identityList[0]));
          additions.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(descriptor, identityList[1]));
          additions.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(identityDescriptor1, readIdentity1));
          IdentityDescriptor identityDescriptor2 = this.Domain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.LicensedUsersGroup);
          additions.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(identityDescriptor2, identityList[2]));
        }
        else
        {
          IList<IdentityDescriptor> descriptors = (IList<IdentityDescriptor>) new IdentityDescriptor[2]
          {
            GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup,
            GroupWellKnownIdentityDescriptors.ServiceUsersGroup
          };
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(vssRequestContext, descriptors, QueryMembership.None, (IEnumerable<string>) null);
          additions.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(identityDescriptor1, identityList[0]));
          additions.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(descriptor, identityList[1]));
          additions.Add(new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(identityDescriptor1, readIdentity1));
        }
      }
      if (additions.Count > 0)
        this.IdentityStore.AddMembersToApplicationGroups(requestContext, this.Domain, false, (IEnumerable<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>) additions);
      this.EnsureSecurityNamespaces(requestContext);
      this.SetDefaultPermissions(requestContext, (IdentityScope) null);
    }

    void IInstallableIdentityService.Uninstall(
      IVssRequestContext requestContext,
      IdentityDomain domain)
    {
      if (domain == null)
        domain = this.Domain;
      this.IdentityStore.Uninstall(requestContext, domain);
    }

    public override IdentityScope CreateScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid parentScopeId,
      GroupScopeType scopeType,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription,
      Guid creatorId)
    {
      requestContext.TraceEnter(80101, "IdentityService", "BusinessLogic", nameof (CreateScope));
      TFCommonUtil.CheckGroupName(ref scopeName);
      TFCommonUtil.CheckGroupName(ref adminGroupName);
      TFCommonUtil.CheckGroupDescription(ref adminGroupDescription);
      ArgumentUtility.CheckForNull<IdentityDescriptor>(requestContext.UserContext, "UserContext");
      if (!this.HasPermission(requestContext, (string) null, 16, false))
        this.CheckPermission(requestContext, (string) null, 2, false);
      Guid rootScopeId = this.IdentityStore.MapScopeId(requestContext, this.Domain, requestContext.ServiceHost.InstanceId);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !this.IsScopeWithinRootScope(requestContext, rootScopeId, parentScopeId))
        throw new IncompatibleScopeException(rootScopeId.ToString(), parentScopeId.ToString());
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1;
      if (!(creatorId != Guid.Empty))
        identity1 = requestContext.GetUserIdentity();
      else
        identity1 = service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          creatorId
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
      Guid localScopeId = scopeId;
      if (scopeType == GroupScopeType.TeamProject)
        scopeId = Guid.NewGuid();
      if (parentScopeId == Guid.Empty)
        parentScopeId = this.Domain.DomainId;
      string rootGroupName;
      string rootGroupDescription;
      if (requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection) && scopeType == GroupScopeType.ServiceHost)
      {
        rootGroupName = FrameworkResources.ProjectCollectionValidUsers();
        rootGroupDescription = FrameworkResources.ProjectCollectionValidUsersDescription();
      }
      else if (scopeType == GroupScopeType.TeamProject)
      {
        rootGroupName = FrameworkResources.ProjectValidUsersGroupName();
        rootGroupDescription = FrameworkResources.ProjectValidUsersGroupDescription();
      }
      else
      {
        rootGroupName = FrameworkResources.ValidUsersGroupName();
        rootGroupDescription = FrameworkResources.ValidUsersGroupDescription();
      }
      IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
      IdentityDomain identityDomain = new IdentityDomain(scopeId, scopeName);
      try
      {
        this.IdentityStore.CreateScope(requestContext, this.Domain, parentScopeId, scopeType, scopeId, localScopeId, scopeName, securityContext.ServiceHost.InstanceId, identityDomain.DomainRoot.Identifier, rootGroupName, rootGroupDescription, identityDomain.DomainAdmin.Identifier, adminGroupName, adminGroupDescription);
        requestContext.Trace(80104, TraceLevel.Info, "IdentityService", "BusinessLogic", "Created scope:{0} with scopeId:{1}, localScopeId:{2}, parentScopeId:{3}, scopeType:{4}", (object) scopeName, (object) scopeId, (object) localScopeId, (object) parentScopeId, (object) scopeType);
        if (identity2 != null)
        {
          if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || !Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, identity2.Descriptor))
          {
            this.IdentityStore.AddMembersToApplicationGroups(requestContext, this.Domain, true, new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(identityDomain.DomainAdmin, identity2));
            requestContext.Trace(80107, TraceLevel.Info, "IdentityService", "BusinessLogic", "Added identityId:{0} with displayName:{1} to GroupId:{2} with displayName:{3}", (object) identity2.Id, (object) identity2.DisplayName, (object) identityDomain.DomainAdmin.Identifier, (object) adminGroupName);
          }
          else
            requestContext.Trace(80107, TraceLevel.Info, "IdentityService", "BusinessLogic", "Not adding identityId:{0} with displayName:{1} to GroupId:{2} with displayName:{3} because it is a service principal", (object) identity2.Id, (object) identity2.DisplayName, (object) identityDomain.DomainAdmin.Identifier, (object) adminGroupName);
        }
        if (scopeType != GroupScopeType.TeamProject)
        {
          string name;
          string description;
          if (requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection) && scopeType == GroupScopeType.ServiceHost)
          {
            name = FrameworkResources.ProjectCollectionServiceAccounts();
            description = FrameworkResources.ProjectCollectionServiceAccountsDescription();
          }
          else
          {
            name = FrameworkResources.ServiceGroupName();
            description = FrameworkResources.ServiceGroupDescription();
          }
          IdentityDescriptor descriptor = identityDomain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.ServiceUsersGroup);
          this.IdentityStore.CreateGroups(requestContext, this.Domain, scopeId, false, true, new GroupDescription(descriptor, name, description, SpecialGroupType.ServiceApplicationGroup, scopeLocal: true));
          requestContext.Trace(80110, TraceLevel.Info, "IdentityService", "BusinessLogic", "Created group with name:{0}, scopeId:{1} and specialGroupType:{2}", (object) name, (object) scopeId, (object) SpecialGroupType.ServiceApplicationGroup);
          Microsoft.VisualStudio.Services.Identity.Identity readIdentity = this.ReadIdentities(requestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            descriptor
          }, QueryMembership.None, (IEnumerable<string>) null, false)[0];
          this.IdentityStore.AddMembersToApplicationGroups(requestContext, this.Domain, true, new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(identityDomain.DomainAdmin, readIdentity));
          requestContext.Trace(80115, TraceLevel.Info, "IdentityService", "BusinessLogic", "Added identityId:{0} with displayName:{1} to GroupId:{2} with displayName:{3}", (object) readIdentity.Id, (object) readIdentity.DisplayName, (object) identityDomain.DomainAdmin.Identifier, (object) adminGroupName);
        }
        IdentityScope identityScope = new IdentityScope()
        {
          Id = scopeId,
          LocalScopeId = localScopeId,
          ParentId = parentScopeId,
          ScopeType = scopeType,
          Name = scopeName,
          Administrators = identityDomain.DomainAdmin,
          IsGlobal = false,
          SecuringHostId = requestContext.ServiceHost.InstanceId
        };
        this.SetDefaultPermissions(requestContext, identityScope);
        requestContext.Trace(80120, TraceLevel.Info, "IdentityService", "BusinessLogic", "SetDefaultPermissions for scope:{0} with scopeId:{1}", (object) scopeName, (object) localScopeId);
        return identityScope;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(80125, TraceLevel.Error, "IdentityService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(80130, "IdentityService", "BusinessLogic", nameof (CreateScope));
      }
    }

    public override IdentityScope GetScope(IVssRequestContext requestContext, Guid scopeId)
    {
      this.CheckPermission(requestContext, scopeId.ToString("D"), 1, true);
      Guid scopeId1 = this.IdentityStore.MapScopeId(requestContext, this.Domain, scopeId);
      return this.IdentityStore.GetScope(requestContext, this.Domain, scopeId1);
    }

    IdentityScope IPlatformIdentityServiceInternal.GetScopeByMasterId(
      IVssRequestContext requestContext,
      Guid masterScopeId)
    {
      IdentityScope scope = this.IdentityStore.GetScope(requestContext, this.Domain, masterScopeId);
      if (scope == null)
        return (IdentityScope) null;
      this.CheckPermission(requestContext, scope.LocalScopeId.ToString("D"), 1, true);
      return scope;
    }

    IdentityScope IPlatformIdentityServiceInternal.GetScopeByLocalId(
      IVssRequestContext requestContext,
      Guid localScopeId)
    {
      this.CheckPermission(requestContext, localScopeId.ToString("D"), 1, true);
      Guid scopeId = this.IdentityStore.MapScopeId(requestContext, this.Domain, localScopeId);
      try
      {
        IdentityScope scope = this.IdentityStore.GetScope(requestContext, this.Domain, scopeId);
        return (scope != null ? (scope.ScopeType != GroupScopeType.TeamProject ? 1 : 0) : 1) != 0 || scopeId == localScopeId && !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableScopeIdMatchCheck") ? (IdentityScope) null : scope;
      }
      catch (GroupScopeDoesNotExistException ex)
      {
        if (scopeId == localScopeId)
          return (IdentityScope) null;
        throw;
      }
    }

    IList<Microsoft.VisualStudio.Services.Identity.Identity> IPlatformIdentityServiceInternal.ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      SequenceContext minSequenceContext)
    {
      return this.ReadIdentities(requestContext, descriptors, queryMembership, propertyNameFilters, minSequenceContext, includeRestrictedVisibility);
    }

    IList<Microsoft.VisualStudio.Services.Identity.Identity> IPlatformIdentityServiceInternal.ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool bypassCache)
    {
      return this.ReadIdentities(requestContext, descriptors, queryMembership, propertyNameFilters, (SequenceContext) null, bypassCache: bypassCache);
    }

    IList<Microsoft.VisualStudio.Services.Identity.Identity> IPlatformIdentityServiceInternal.ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool bypassCache)
    {
      return this.ReadIdentities(requestContext, identityIds, queryMembership, propertyNameFilters, false, true, (SequenceContext) null, bypassCache);
    }

    public override IdentityScope GetScope(IVssRequestContext requestContext, string scopeName)
    {
      ArgumentUtility.CheckForNull<string>(scopeName, nameof (scopeName));
      Guid scopeId = this.IdentityStore.GetScopeId(requestContext, this.Domain, scopeName, true);
      IdentityScope scope = this.GetScope(requestContext.Elevate(), scopeId);
      if (scope != null)
        this.CheckPermission(requestContext, scope.LocalScopeId.ToString("D"), 1, true);
      return scope;
    }

    public override void DeleteScope(IVssRequestContext requestContext, Guid scopeId)
    {
      Guid guid = this.IdentityStore.MapScopeId(requestContext, this.Domain, scopeId);
      Guid rootScopeId = this.IdentityStore.MapScopeId(requestContext, this.Domain, requestContext.ServiceHost.InstanceId);
      if (!this.IsScopeWithinRootScope(requestContext, rootScopeId, guid))
        throw new IncompatibleScopeException(rootScopeId.ToString(), scopeId.ToString());
      this.IdentityStore.GetScope(requestContext, this.Domain, guid);
      this.CheckPermission(requestContext, scopeId.ToString("D"), 4, false);
      this.DeleteScopeInternal(requestContext, guid, false);
    }

    public override void RestoreScope(IVssRequestContext requestContext, Guid scopeId)
    {
      requestContext.Trace(80156, TraceLevel.Info, "IdentityService", "BusinessLogic", string.Format("Restoring ScopeId: {0}", (object) scopeId));
      this.CheckPermission(requestContext, scopeId.ToString("D"), 32, false);
      Guid scopeId1 = this.IdentityStore.MapScopeId(requestContext, this.Domain, scopeId);
      this.IdentityStore.RestoreScope(requestContext, this.Domain, scopeId1);
    }

    internal virtual void HardDeleteScope(IVssRequestContext requestContext, Guid scopeId)
    {
      if (!requestContext.IsSystemContext)
        throw new UnexpectedRequestContextTypeException(RequestContextType.ServicingContext);
      Guid guid = this.IdentityStore.MapScopeId(requestContext, this.Domain, scopeId);
      Guid rootScopeId = this.IdentityStore.MapScopeId(requestContext, this.Domain, requestContext.ServiceHost.InstanceId);
      if (!this.IsScopeWithinRootScope(requestContext, rootScopeId, guid))
        throw new IncompatibleScopeException(rootScopeId.ToString(), scopeId.ToString());
      this.IdentityStore.GetScope(requestContext, this.Domain, guid);
      this.DeleteScopeInternal(requestContext, guid, true);
    }

    private void DeleteScopeInternal(
      IVssRequestContext requestContext,
      Guid scopeId,
      bool hardDelete)
    {
      requestContext.Trace(80155, TraceLevel.Info, "IdentityService", "BusinessLogic", "{0} deleting ScopeId:{1}", hardDelete ? (object) "Hard" : (object) "Soft", (object) scopeId);
      this.IdentityStore.DeleteScope(requestContext, this.Domain, scopeId, hardDelete);
      IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
      IVssSecurityNamespace securityNamespace = this.GetSecurityNamespace(securityContext);
      if (!hardDelete)
        return;
      securityNamespace.RemoveAccessControlLists(securityContext, (IEnumerable<string>) new string[1]
      {
        scopeId.ToString("D")
      }, true);
    }

    public override void RenameScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      string newName)
    {
      this.CheckPermission(requestContext, scopeId.ToString("D"), 2, false);
      TFCommonUtil.CheckGroupName(ref newName);
      Guid guid = this.IdentityStore.MapScopeId(requestContext, this.Domain, scopeId);
      Guid rootScopeId = this.IdentityStore.MapScopeId(requestContext, this.Domain, requestContext.ServiceHost.InstanceId);
      if (!this.IsScopeWithinRootScope(requestContext, rootScopeId, guid))
        throw new IncompatibleScopeException(rootScopeId.ToString(), scopeId.ToString());
      this.IdentityStore.RenameScope(requestContext, this.Domain, guid, newName);
    }

    protected override Guid GetScopeParentId(IVssRequestContext requestContext, Guid scopeId)
    {
      this.CheckPermission(requestContext, scopeId.ToString("D"), 1, false);
      Guid scopeId1 = this.IdentityStore.MapScopeId(requestContext, this.Domain, scopeId);
      return this.IdentityStore.GetScopeParentId(requestContext, this.Domain, scopeId1);
    }

    protected override Microsoft.VisualStudio.Services.Identity.Identity CreateUser(
      IVssRequestContext requestContext,
      Guid scopeId,
      string userSid,
      string domainName,
      string accountName,
      string description)
    {
      if (scopeId == Guid.Empty)
        scopeId = this.Domain.DomainId;
      this.CheckPermission(requestContext, scopeId.ToString("D"), 2, false);
      TFCommonUtil.CheckGroupName(ref accountName);
      TFCommonUtil.CheckGroupDescription(ref description);
      IdentityDescriptor descriptor = (IdentityDescriptor) null;
      if (!string.IsNullOrEmpty(userSid))
        descriptor = IdentityHelper.CreateUnauthenticatedDescriptor(userSid);
      return this.IdentityStore.CreateUnauthenticatedIdentity(requestContext, this.Domain, descriptor, domainName, accountName, description, Guid.Empty);
    }

    Microsoft.VisualStudio.Services.Identity.Identity IPlatformIdentityServiceInternal.CreateFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      Guid identityIdGuid,
      string mailAddress)
    {
      return this.CreateFrameworkIdentity(requestContext, identityType, role, identifier, displayName, mailAddress, (string) null, role, true, identityIdGuid);
    }

    public override Microsoft.VisualStudio.Services.Identity.Identity CreateFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      string mailAddress = null)
    {
      return this.CreateFrameworkIdentity(requestContext, identityType, role, identifier, displayName, mailAddress, (string) null, role, true, Guid.Empty);
    }

    Microsoft.VisualStudio.Services.Identity.Identity IPlatformIdentityServiceInternal.BuildFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      string mailAddress,
      string directoryAlias,
      string domain)
    {
      return this.CreateFrameworkIdentity(requestContext, identityType, role, identifier, displayName, mailAddress, directoryAlias, domain, false, Guid.Empty);
    }

    private Microsoft.VisualStudio.Services.Identity.Identity CreateFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      string mailAddress,
      string directoryAlias,
      string domain,
      bool persistToDatabase,
      Guid identityIdGuid)
    {
      if (identityType == FrameworkIdentityType.None)
        throw new ArgumentException(CommonResources.EmptyOrWhiteSpaceStringNotAllowed(), nameof (identityType));
      TFCommonUtil.CheckDisplayName(ref displayName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(role, nameof (role));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(domain, nameof (domain));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(identifier, nameof (identifier));
      if (mailAddress != null)
        ArgumentUtility.CheckEmailAddress(mailAddress, nameof (mailAddress));
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        requestContext.TraceConditionally(80740, TraceLevel.Warning, "IdentityService", "BusinessLogic", (Func<string>) (() => string.Format("Trying to create framework identity in not collection host. identity Type:{0}, role:{1}, displayName:{2}, mailaddress:{3}, domain:{4}, persistToDatabase:{5}", (object) identityType, (object) role, (object) displayName, (object) mailAddress, (object) domain, (object) persistToDatabase)));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
        throw new AccessCheckException(FrameworkResources.InvalidAccessException());
      IdentityDescriptor identityDescriptor = IdentityHelper.CreateFrameworkIdentityDescriptor(identityType, requestContext.ServiceHost.InstanceId, role, identifier);
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (persistToDatabase)
        frameworkIdentity = this.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identityDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null, false).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (frameworkIdentity == null)
        frameworkIdentity = this.IdentityStore.CreateUnauthenticatedIdentity(requestContext, this.Domain, identityDescriptor, domain, identifier, string.Empty, identityIdGuid, displayName, mailAddress, directoryAlias, persistToDatabase);
      return frameworkIdentity;
    }

    public override Microsoft.VisualStudio.Services.Identity.Identity CreateGroup(
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
      TFCommonUtil.CheckGroupName(ref groupName);
      TFCommonUtil.CheckGroupDescription(ref groupDescription);
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId));
      scopeId = scopeId == Guid.Empty ? this.Domain.DomainId : scopeId;
      Guid guid = this.IdentityStore.MapScopeId(requestContext, this.Domain, scopeId);
      IdentityDescriptor identityDescriptor = (IdentityDescriptor) null;
      bool isWellKnownGroup = false;
      if (!string.IsNullOrEmpty(groupSid))
      {
        if (groupSid.StartsWith(SidIdentityHelper.WellKnownSidPrefix, StringComparison.OrdinalIgnoreCase))
        {
          groupSid = IdentityMapper.MapFromWellKnownIdentifier(groupSid, guid);
          isWellKnownGroup = true;
        }
        identityDescriptor = IdentityHelper.CreateTeamFoundationDescriptor(groupSid);
      }
      bool isAadGroup = PlatformIdentityService.CheckAndValidateAadGroupCreation(requestContext, scopeId, identityDescriptor, specialType);
      this.CheckPermission(requestContext, scopeId.ToString("D"), 2, true);
      this.CheckGroupNameIsNotReserved(requestContext, groupName, isWellKnownGroup, isAadGroup);
      Guid rootScopeId = this.IdentityStore.MapScopeId(requestContext, this.Domain, requestContext.ServiceHost.InstanceId);
      if (!this.IsScopeWithinRootScope(requestContext, rootScopeId, guid))
        throw new IncompatibleScopeException(rootScopeId.ToString(), scopeId.ToString());
      if (isAadGroup && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableCreateIdentityForAadGroup"))
        PlatformIdentityService.AddAggregateIdentityForAadGroup(requestContext, identityDescriptor);
      GroupDescription groupDescription1 = new GroupDescription(identityDescriptor, groupName, groupDescription, specialType, hasRestrictedVisibility, scopeLocal, guid, groupId);
      bool flag = !isAadGroup || requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.CreateAadGroupWithActiveScopeMembership");
      this.IdentityStore.CreateGroups(requestContext, this.Domain, guid, (!isWellKnownGroup ? 1 : 0) != 0, (flag ? 1 : 0) != 0, groupDescription1);
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = this.ReadIdentities(requestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        groupDescription1.Descriptor
      }, QueryMembership.None, (IEnumerable<string>) null, false)[0];
      if (!isWellKnownGroup & flag)
      {
        Microsoft.TeamFoundation.Framework.Server.AccessControlList accessControlList = new Microsoft.TeamFoundation.Framework.Server.AccessControlList(IdentityHelper.CreateSecurityToken((IReadOnlyVssIdentity) readIdentity), true);
        Microsoft.TeamFoundation.Framework.Server.AccessControlEntry newAce = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(groupDescription1.Descriptor, 2, 0);
        accessControlList.SetAccessControlEntry(newAce, false);
        IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
        this.GetSecurityNamespace(securityContext).SetAccessControlLists(securityContext.Elevate(), (IEnumerable<IAccessControlList>) new Microsoft.TeamFoundation.Framework.Server.AccessControlList[1]
        {
          accessControlList
        });
      }
      requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new AfterCreateGroupsEvent()
      {
        CreatedGroups = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          readIdentity
        }
      });
      return readIdentity;
    }

    public override void DeleteGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      TFCommonUtil.CheckSid(groupDescriptor.Identifier, "groupSid");
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = this.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        groupDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null, false)[0];
      string token = readIdentity != null ? IdentityHelper.CreateSecurityToken((IReadOnlyVssIdentity) readIdentity) : throw new FindGroupSidDoesNotExistException(groupDescriptor.Identifier);
      this.CheckPermission(requestContext, token, 4, false);
      if (readIdentity.Descriptor.Identifier.StartsWith(this.Domain.DomainSid, StringComparison.OrdinalIgnoreCase) && readIdentity.Descriptor.Identifier.Substring(this.Domain.DomainSid.Length).StartsWith(SidIdentityHelper.WellKnownSidType, StringComparison.OrdinalIgnoreCase))
      {
        SpecialGroupType specialGroupType = IdentityHelper.GetSpecialGroupType((IReadOnlyVssIdentity) readIdentity);
        switch (specialGroupType)
        {
          case SpecialGroupType.LicenseesApplicationGroup:
          case SpecialGroupType.AzureActiveDirectoryApplicationGroup:
            break;
          default:
            IdentityDescriptor foundationDescriptor = IdentityHelper.CreateTeamFoundationDescriptor(WebAccessConstants.WorkItemOnlyViewUsersGroup);
            IdentityDescriptor wellKnownIdentifier = this.Domain.MapToWellKnownIdentifier(readIdentity.Descriptor);
            if (!IdentityDescriptorComparer.Instance.Equals(wellKnownIdentifier, foundationDescriptor) && !IdentityDescriptorComparer.Instance.Equals(wellKnownIdentifier, PlatformIdentityService.AnonymousUsersGroup))
              throw new RemoveSpecialGroupException(readIdentity.Descriptor.Identifier, specialGroupType);
            break;
        }
      }
      this.IdentityStore.DeleteApplicationGroup(requestContext, this.Domain, readIdentity);
      IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
      this.GetSecurityNamespace(securityContext).RemoveAccessControlLists(securityContext.Elevate(), (IEnumerable<string>) new string[1]
      {
        token
      }, true);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ListGroups(requestContext, scopeIds, recurse, propertyNameFilters, false);
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters,
      bool listOnlyDeleted)
    {
      if (scopeIds == null)
        scopeIds = new Guid[1]{ this.Domain.DomainId };
      for (int index = 0; index < scopeIds.Length; ++index)
        scopeIds[index] = this.IdentityStore.MapScopeId(requestContext, this.Domain, scopeIds[index]);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.IdentityStore.ListApplicationGroups(requestContext, this.Domain, scopeIds, recurse, listOnlyDeleted, propertyNameFilters != null, propertyNameFilters);
      return this.FilterReadResults(requestContext, results, true);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ListDeletedGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ListGroups(requestContext, scopeIds, recurse, propertyNameFilters, true);
    }

    public override bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity member)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(member, nameof (member));
      IIdentityProvider identityProvider;
      return this.SyncAgents.TryGetValue(member.Descriptor.IdentityType, out identityProvider) && identityProvider.IsSyncable ? this.AddMemberToGroup(requestContext, groupDescriptor, member.Descriptor) : this.AddMemberToGroupInternal(requestContext, groupDescriptor, member);
    }

    private bool AddMemberToGroupInternal(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity member,
      bool extend = true)
    {
      requestContext.TraceEnter(0, "IdentityService", "BusinessLogic", "AddMemberToGroup");
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckTeamFoundationType(groupDescriptor);
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(member, nameof (member));
      this.CheckGroupPermission(requestContext, groupDescriptor, member.Descriptor, 8);
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.CheckGroupExistsInRequestScope(requestContext, groupDescriptor);
      IdentityScopeHelper.CheckGroupMembershipManagementIsAllowedInRequestScope(requestContext, identity.LocalScopeId, groupDescriptor.ToSubjectDescriptor(requestContext));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, member.Descriptor))
        throw new AddGroupMemberIllegalMemberException(HostingResources.CannotAddServicePrincipalToGroup((object) member.Descriptor.ToString()));
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        requestContext.Trace(80728, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Issuing a BeforeGroupMembershipChangeSecurityPolicyEvent for member: {0} and group: {1}", (object) member.Descriptor, (object) groupDescriptor);
        service.PublishNotification(requestContext, (object) new BeforeGroupMembershipChangeSecurityPolicyEvent()
        {
          GroupDescriptor = groupDescriptor,
          MemberDescriptor = member.Descriptor,
          EventSource = GroupMembershipSecurityPolicyEventSource.AddMemberToGroup
        });
      }
      if (IdentityTranslationHelper.IsEnabled(requestContext))
      {
        requestContext.Trace(80722, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
        service.PublishDecisionPoint(requestContext, (object) new BeforeAddMemberToGroupOnService()
        {
          IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
          {
            Identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
            {
              member
            }
          }
        });
      }
      else
        requestContext.Trace(80723, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
      if (member.IsMsaIdentity() && member.Descriptor.Identifier.IndexOf("@Live.com") == 14)
        member.Descriptor.Identifier = "00" + member.Descriptor.Identifier;
      bool flag = this.IdentityStore.IsMember(requestContext, this.Domain, GroupWellKnownIdentityDescriptors.EveryoneGroup, member.Descriptor, false);
      if (extend)
      {
        BeforeAddMemberToGroupEvent notificationEvent = new BeforeAddMemberToGroupEvent()
        {
          GroupDescriptor = groupDescriptor,
          Member = member
        };
        service.PublishDecisionPoint(requestContext, (object) notificationEvent);
      }
      if (requestContext.IsUserContext)
      {
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && string.Equals(member.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase))
          throw new AddGroupMemberIllegalWindowsIdentityException(member);
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && (string.Equals(member.Descriptor.IdentityType, "Microsoft.IdentityModel.Claims.ClaimsIdentity", StringComparison.OrdinalIgnoreCase) || string.Equals(member.Descriptor.IdentityType, "Microsoft.TeamFoundation.BindPendingIdentity", StringComparison.OrdinalIgnoreCase) || string.Equals(member.Descriptor.IdentityType, "Microsoft.VisualStudio.Services.Claims.AadServicePrincipal", StringComparison.OrdinalIgnoreCase)))
          throw new AddGroupMemberIllegalInternetIdentityException(member);
      }
      groupDescriptor = this.Domain.MapFromWellKnownIdentifier(groupDescriptor);
      if (groupDescriptor.Identifier.EndsWith(PlatformIdentityService.s_everyoneGroupSuffix, StringComparison.OrdinalIgnoreCase))
        throw new ModifyEveryoneGroupException();
      if (IdentityDescriptorComparer.Instance.Equals(member.Descriptor, this.Domain.DomainRoot) || IdentityDescriptorComparer.Instance.Equals(member.Descriptor, this.IdentityStore.Domain.DomainRoot))
        return true;
      if (string.IsNullOrEmpty(member.GetProperty<string>("SecurityGroup", (string) null)) && member.IsContainer && string.Equals(member.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase))
      {
        if (!requestContext.IsSystemContext)
          throw new NotASecurityGroupException(member.DisplayName);
        TeamFoundationEventLog.Default.Log((IVssRequestContext) null, FrameworkResources.NOT_A_SECURITY_GROUP_ADDED((object) member.DisplayName), TeamFoundationEventId.TeamFoundationIdentityServiceException, EventLogEntryType.Warning);
      }
      bool applicationGroups = this.IdentityStore.AddMembersToApplicationGroups(requestContext, this.Domain, false, new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(groupDescriptor, member));
      if (member.IsContainer && !IdentityValidation.IsTeamFoundationType(member.Descriptor))
        this.IdentityStore.UpdateSyncQueue(requestContext, this.Domain, (IList<Guid>) null, (IList<Tuple<Guid, bool, bool>>) new Tuple<Guid, bool, bool>[1]
        {
          new Tuple<Guid, bool, bool>(member.MasterId, true, true)
        });
      if (extend)
      {
        AfterAddMemberToGroupEvent notificationEvent = new AfterAddMemberToGroupEvent()
        {
          GroupDescriptor = groupDescriptor,
          Member = member,
          Success = applicationGroups
        };
        service.PublishNotification(requestContext, (object) notificationEvent);
      }
      if (IdentityTranslationHelper.IsEnabled(requestContext))
      {
        requestContext.Trace(80724, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
        service.PublishDecisionPoint(requestContext, (object) new AfterAddMemberToGroupOnService()
        {
          IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
          {
            Identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
            {
              member
            }
          },
          AlreadyMemberOfAccount = flag
        });
      }
      else
        requestContext.Trace(80725, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
      requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", "AddMemberToGroup");
      return applicationGroups;
    }

    public override bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      this.CheckGroupExistsInRequestScope(requestContext, groupDescriptor);
      IdentityValidation.CheckDescriptor(memberDescriptor, nameof (memberDescriptor));
      memberDescriptor = this.Domain.MapFromWellKnownIdentifier(memberDescriptor);
      Microsoft.VisualStudio.Services.Identity.Identity member = this.ReadIdentitiesFromSource(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        memberDescriptor
      }, QueryMembership.None)[0];
      if (member == null)
      {
        member = this.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          memberDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null, false).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (member == null && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          requestContext.Trace(80726, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Attempt to read member identity at Application Level.");
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
          member = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            memberDescriptor
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
        }
        if (member == null)
        {
          if (requestContext.IsDeploymentFallbackIdentityReadAllowed())
          {
            requestContext.Trace(80727, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Attempt to read member identity at Deployment Level");
            IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
            member = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
            {
              memberDescriptor
            }, QueryMembership.None, (IEnumerable<string>) null)[0];
            if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && member != null && member.IsContainer)
              member = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          }
          else
            requestContext.TraceDataConditionally(7070911, TraceLevel.Error, "IdentityService", "BusinessLogic", "Fallback identity read not allowed", (Func<object>) (() => (object) new
            {
              groupDescriptor = groupDescriptor,
              memberDescriptor = memberDescriptor,
              stackTrace = Environment.StackTrace
            }), nameof (AddMemberToGroup));
        }
        if (member != null && !this.SyncAgents.TryGetValue(member.Descriptor.IdentityType, out IIdentityProvider _))
          member = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (member != null && !member.IsActive)
          member.IsActive = true;
      }
      else if (IdentityDescriptorComparer.Instance.Equals(memberDescriptor, this.Domain.DomainRoot) || IdentityDescriptorComparer.Instance.Equals(memberDescriptor, this.IdentityStore.Domain.DomainRoot))
        return true;
      if (member == null && VssStringComparer.IdentityDescriptor.Equals(memberDescriptor.IdentityType, "Microsoft.TeamFoundation.BindPendingIdentity"))
      {
        if (IdentityHelper.IsValidBindPendingDescriptor(memberDescriptor))
        {
          string domain;
          string accountName;
          IdentityHelper.RetrieveDomainAndAccountNameFromBindPendingDescriptor(memberDescriptor, out domain, out accountName);
          if (requestContext.IsDeploymentFallbackIdentityReadAllowed())
          {
            IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
            member = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, IdentitySearchFilter.AccountName, domain + "\\" + accountName, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          }
          else if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          {
            IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
            member = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, IdentitySearchFilter.AccountName, domain + "\\" + accountName, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          }
          else
            requestContext.TraceDataConditionally(7071911, TraceLevel.Error, "IdentityService", "BusinessLogic", "Fallback identity read not allowed", (Func<object>) (() => (object) new
            {
              groupDescriptor = groupDescriptor,
              memberDescriptor = memberDescriptor,
              searchFactor = IdentitySearchFilter.AccountName,
              searchValue = (domain + "\\" + accountName),
              stackTrace = Environment.StackTrace
            }), nameof (AddMemberToGroup));
          if (member == null)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
            identity.Descriptor = memberDescriptor;
            identity.ProviderDisplayName = accountName;
            identity.IsActive = true;
            member = identity;
            if (string.Equals(domain, "Windows Live ID", StringComparison.OrdinalIgnoreCase))
              member.SetProperty("Mail", (object) accountName);
            member.SetProperty("Domain", (object) domain);
            member.SetProperty("Account", (object) accountName);
          }
        }
        else
        {
          string str = IdentityHelper.IsValidLegacyBindPendingDescriptor(memberDescriptor) ? IdentityAuthenticationHelper.RetrieveEmailAddressFromTemporaryDescriptor(memberDescriptor) : throw new InvalidBindPendingIdentityDescriptorException(FrameworkResources.InvalidBindPendingIdentityDescriptorErrorV2((object) memberDescriptor.Identifier));
          Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
          identity.Descriptor = memberDescriptor;
          identity.ProviderDisplayName = str;
          identity.IsActive = true;
          member = identity;
          member.SetProperty("Mail", (object) str);
          member.SetProperty("Account", (object) str);
        }
      }
      if (member != null)
      {
        if (member.IsActive)
        {
          try
          {
            return this.AddMemberToGroupInternal(requestContext, groupDescriptor, member);
          }
          catch (ModifyEveryoneGroupException ex)
          {
            requestContext.TraceException(80211, TraceLevel.Error, "IdentityService", "BusinessLogic", (Exception) ex);
            return true;
          }
        }
      }
      throw new IdentityNotFoundException(memberDescriptor);
    }

    public override bool RemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      return this.RemoveMemberFromGroupInternal(requestContext, ref groupDescriptor, ref memberDescriptor, false);
    }

    public override bool ForceRemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.ForceRemoveMemberFromGroup"))
      {
        requestContext.TraceConditionally(80762, TraceLevel.Warning, "IdentityService", "BusinessLogic", (Func<string>) (() => "ForceRemoveMemberFromGroup called with unset feature flag."));
        return false;
      }
      if (this.HasPermission(requestContext, (string) null, 64, false))
        return this.RemoveMemberFromGroupInternal(requestContext, ref groupDescriptor, ref memberDescriptor, true);
      requestContext.TraceConditionally(80763, TraceLevel.Warning, "IdentityService", "BusinessLogic", (Func<string>) (() => "ForceRemoveMemberFromGroup called from service without valid ForceDelete permissions in SPS."));
      return false;
    }

    public IDictionary<IdentityDescriptor, bool> HasAadGroups(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aadGroupDescriptors)
    {
      ArgumentUtility.CheckForNull<IList<IdentityDescriptor>>(aadGroupDescriptors, nameof (aadGroupDescriptors));
      foreach (IdentityDescriptor aadGroupDescriptor in (IEnumerable<IdentityDescriptor>) aadGroupDescriptors)
        IdentityValidation.CheckDescriptor(aadGroupDescriptor, nameof (aadGroupDescriptors));
      return this.IdentityStore.HasAadGroups(requestContext, aadGroupDescriptors);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadAadGroupsAncestorMemberships(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aadGroupDescriptors,
      SequenceContext minSequenceContext)
    {
      requestContext.TraceEnter(80760, "IdentityService", "BusinessLogic", nameof (ReadAadGroupsAncestorMemberships));
      try
      {
        ArgumentUtility.CheckForNull<IList<IdentityDescriptor>>(aadGroupDescriptors, nameof (aadGroupDescriptors));
        foreach (IdentityDescriptor aadGroupDescriptor in (IEnumerable<IdentityDescriptor>) aadGroupDescriptors)
          IdentityValidation.CheckDescriptor(aadGroupDescriptor, nameof (aadGroupDescriptors));
        IDictionary<IdentityDescriptor, bool> hasAadGroupMap = this.IdentityStore.HasAadGroups(requestContext, aadGroupDescriptors);
        IList<IdentityDescriptor> materializedAadGroupDescriptors = (IList<IdentityDescriptor>) hasAadGroupMap.Where<KeyValuePair<IdentityDescriptor, bool>>((Func<KeyValuePair<IdentityDescriptor, bool>, bool>) (x => x.Value)).Select<KeyValuePair<IdentityDescriptor, bool>, IdentityDescriptor>((Func<KeyValuePair<IdentityDescriptor, bool>, IdentityDescriptor>) (x => x.Key)).ToList<IdentityDescriptor>();
        requestContext.TraceConditionally(80761, TraceLevel.Info, "IdentityService", "BusinessLogic", (Func<string>) (() => string.Format("HasAadGroups aadGroups.Count: {0}, hasAadGroupMap.Count: {1}, materializedAadGroups.Count: {2}", (object) aadGroupDescriptors.Count, (object) hasAadGroupMap.Count, (object) materializedAadGroupDescriptors.Count)));
        if (materializedAadGroupDescriptors.IsNullOrEmpty<IdentityDescriptor>())
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>(aadGroupDescriptors.Count);
        IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> aadGroupsAncestorMembershipsMap = this.ReadAncestorMemberships(requestContext, materializedAadGroupDescriptors, (IEnumerable<string>) null, minSequenceContext);
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) aadGroupDescriptors.Select<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) (identityDescriptor =>
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          aadGroupsAncestorMembershipsMap.TryGetValue(identityDescriptor, out identity);
          return identity;
        })).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      finally
      {
        requestContext.TraceLeave(80769, "IdentityService", "BusinessLogic", nameof (ReadAadGroupsAncestorMemberships));
      }
    }

    internal IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> ReadAncestorMemberships(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      IEnumerable<string> propertyNameFilters,
      SequenceContext minSequenceContext)
    {
      requestContext.TraceEnter(80780, "IdentityService", "BusinessLogic", nameof (ReadAncestorMemberships));
      try
      {
        Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> descriptorToAncestorMembershipsMap = descriptors.ToDedupedDictionary<IdentityDescriptor, IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<IdentityDescriptor, IdentityDescriptor>) (x => x), (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) (x => (Microsoft.VisualStudio.Services.Identity.Identity) null), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = this.ReadIdentities(requestContext, descriptors, QueryMembership.None, propertyNameFilters, false);
        requestContext.TraceConditionally(80781, TraceLevel.Info, "IdentityService", "BusinessLogic", (Func<string>) (() => "ReadIdentities descriptors: " + descriptors.Serialize<IList<IdentityDescriptor>>() + ", identities: " + identities.Serialize<IList<Microsoft.VisualStudio.Services.Identity.Identity>>()));
        PlatformIdentityService.s_readAncestorMembershipsCallsCounter.IncrementBy((long) descriptors.Count);
        PlatformIdentityService.s_readAncestorMembershipsCallsPerSecondCounter.Increment();
        IList<IdentityDescriptor> descriptorsToReadFromCache = (IList<IdentityDescriptor>) new List<IdentityDescriptor>();
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
        {
          if (identity != null)
          {
            descriptorToAncestorMembershipsMap[identity.Descriptor] = identity;
            if (identity.IsActive)
              descriptorsToReadFromCache.Add(identity.Descriptor);
          }
        }
        IDictionary<IdentityDescriptor, IdentityMembershipInfo> descriptorsAncestorMembershipsReadFromCache = this.IdentityStore.ReadIdentityMembershipInfo(requestContext, this.Domain, descriptorsToReadFromCache, minSequenceContext);
        requestContext.TraceConditionally(80783, TraceLevel.Info, "IdentityService", "BusinessLogic", (Func<string>) (() => "ReadIdentityMembershipInfo descriptorsToReadFromCache: " + descriptorsToReadFromCache.Serialize<IList<IdentityDescriptor>>() + ", descriptorsAncestorMembershipsReadFromCache: " + descriptorsAncestorMembershipsReadFromCache.Serialize<IDictionary<IdentityDescriptor, IdentityMembershipInfo>>()));
        IDictionary<IdentityDescriptor, long> dictionary = (IDictionary<IdentityDescriptor, long>) new Dictionary<IdentityDescriptor, long>();
        VssPerformanceCounter performanceCounter1;
        foreach (IdentityDescriptor key in (IEnumerable<IdentityDescriptor>) descriptorsToReadFromCache)
        {
          IdentityMembershipInfo membershipInfo;
          if (descriptorsAncestorMembershipsReadFromCache.TryGetValue(key, out membershipInfo) && membershipInfo != null && !membershipInfo.IsInvalid() && !membershipInfo.IsPartialResult)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = descriptorToAncestorMembershipsMap[key];
            HashSet<IdentityDescriptor> parents = membershipInfo.Parents;
            identity.MemberOf = (ICollection<IdentityDescriptor>) ((parents != null ? parents.ToList<IdentityDescriptor>() : (List<IdentityDescriptor>) null) ?? new List<IdentityDescriptor>());
            HashSet<Guid> parentIds = membershipInfo.ParentIds;
            identity.MemberOfIds = (ICollection<Guid>) ((parentIds != null ? parentIds.ToList<Guid>() : (List<Guid>) null) ?? new List<Guid>());
            performanceCounter1 = PlatformIdentityService.s_readAncestorMembershipsCacheHitsCounter;
            performanceCounter1.Increment();
            performanceCounter1 = PlatformIdentityService.s_readAncestorMembershipsCacheHitsPerSecondCounter;
            performanceCounter1.Increment();
          }
          else
          {
            dictionary[key] = membershipInfo.GetCacheStamp();
            performanceCounter1 = PlatformIdentityService.s_readAncestorMembershipsCacheMissesCounter;
            performanceCounter1.Increment();
            performanceCounter1 = PlatformIdentityService.s_readAncestorMembershipsCacheMissesPerSecondCounter;
            performanceCounter1.Increment();
          }
        }
        IList<IdentityDescriptor> descriptorsToReadFromStore = (IList<IdentityDescriptor>) dictionary.Keys.ToList<IdentityDescriptor>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> descriptorsReadFromStore = this.ReadIdentities(requestContext, descriptorsToReadFromStore, QueryMembership.ExpandedUp, (IEnumerable<string>) null, minSequenceContext, true);
        requestContext.TraceConditionally(80785, TraceLevel.Info, "IdentityService", "BusinessLogic", (Func<string>) (() => "ReadIdentities.ExpandedUp descriptorsToReadFromStore: " + descriptorsToReadFromStore.Serialize<IList<IdentityDescriptor>>() + ", descriptorsReadFromStore: " + descriptorsReadFromStore.Serialize<IList<Microsoft.VisualStudio.Services.Identity.Identity>>()));
        for (int index = 0; index < descriptorsReadFromStore.Count; ++index)
        {
          IdentityDescriptor key = descriptorsToReadFromStore[index];
          Microsoft.VisualStudio.Services.Identity.Identity identityReadFromStore = descriptorsReadFromStore[index];
          if (identityReadFromStore != null)
          {
            descriptorToAncestorMembershipsMap[key] = identityReadFromStore;
            long cacheStamp = dictionary[key];
            bool success = this.IdentityStore.IdentityCache.CompareAndSwapParentMemberships(requestContext, this.Domain, identityReadFromStore, cacheStamp);
            requestContext.TraceConditionally(80787, TraceLevel.Info, "IdentityService", "BusinessLogic", (Func<string>) (() => string.Format("CompareAndSwapParentMemberships identityReadFromStore: {0}, cacheStamp:{1}, success:{2}", (object) identityReadFromStore.Serialize<Microsoft.VisualStudio.Services.Identity.Identity>(), (object) cacheStamp, (object) success)));
            VssPerformanceCounter performanceCounter2 = PlatformIdentityService.s_readAncestorMembershipsCacheAdditionsCounter;
            performanceCounter2.Increment();
            performanceCounter2 = PlatformIdentityService.s_readAncestorMembershipsCacheAdditionsPerSecondCounter;
            performanceCounter2.Increment();
          }
        }
        requestContext.TraceConditionally(80788, TraceLevel.Info, "IdentityService", "BusinessLogic", (Func<string>) (() => "descriptorToAncestorMembershipsMap:" + descriptorToAncestorMembershipsMap.Serialize<Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>()));
        return (IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) descriptorToAncestorMembershipsMap;
      }
      finally
      {
        requestContext.TraceLeave(80789, "IdentityService", "BusinessLogic", nameof (ReadAncestorMemberships));
      }
    }

    public override bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckDescriptor(memberDescriptor, nameof (memberDescriptor));
      groupDescriptor = this.Domain.MapFromWellKnownIdentifier(groupDescriptor);
      memberDescriptor = this.Domain.MapFromWellKnownIdentifier(memberDescriptor);
      return this.IdentityStore.IsMember(requestContext.Elevate(), this.Domain, groupDescriptor, memberDescriptor, false);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      SocialDescriptor socialDescriptor)
    {
      try
      {
        requestContext.TraceEnter(616793, "IdentityService", "BusinessLogic", nameof (ReadIdentity));
        this.CheckIdentityReadPermissions(requestContext, 1);
        return this.IdentityStore.ReadIdentity(requestContext, this.Domain, socialDescriptor, false);
      }
      finally
      {
        requestContext.TraceLeave(616793, "IdentityService", "BusinessLogic", nameof (ReadIdentity));
      }
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      if (queryMembership != QueryMembership.None)
        throw new ArgumentException(nameof (queryMembership));
      if (socialDescriptors == null)
      {
        requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentities));
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      this.CheckIdentityReadPermissions(requestContext, 1);
      return this.IdentityStore.ReadIdentities(requestContext, this.Domain, socialDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      if (subjectDescriptors == null)
      {
        requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentities));
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      this.CheckIdentityReadPermissions(requestContext, 1);
      bool[] flagArray = new bool[subjectDescriptors.Count];
      List<IdentityDescriptor> descriptors = new List<IdentityDescriptor>(subjectDescriptors.Count);
      for (int index = 0; index < subjectDescriptors.Count; ++index)
      {
        IdentityDescriptor identityDescriptor = subjectDescriptors[index].ToIdentityDescriptor(requestContext);
        flagArray[index] = identityDescriptor == (IdentityDescriptor) null;
        if (!flagArray[index])
          descriptors.Add(identityDescriptor);
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = this.ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = new List<Microsoft.VisualStudio.Services.Identity.Identity>(subjectDescriptors.Count);
      int num = 0;
      for (int index = 0; index < subjectDescriptors.Count; ++index)
        identityList2.Add(flagArray[index] ? (Microsoft.VisualStudio.Services.Identity.Identity) null : identityList1[num++]);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityList2;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      SequenceContext minSequenceContext = (SequenceContext) null;
      if (requestContext.RootContext.Items.TryGetValue<SequenceContext>(RequestContextItemsKeys.MinSequenceContext, out minSequenceContext))
        requestContext.RootContext.Items[RequestContextItemsKeys.MinSequenceContext] = (object) null;
      bool flag;
      if (requestContext.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.IgnoreMembershipCache, out flag))
        requestContext.RootContext.Items.Remove(RequestContextItemsKeys.IgnoreMembershipCache);
      if (!includeRestrictedVisibility || minSequenceContext == null || queryMembership != QueryMembership.ExpandedUp || !Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor()) || !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.PlatformIdentityService.UseReadAncestorMemberships") || flag)
        return this.ReadIdentitiesByBatch(requestContext, descriptors, queryMembership, propertyNameFilters, minSequenceContext, includeRestrictedVisibility);
      IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> descriptorToIdentityMap = this.ReadAncestorMemberships(requestContext, descriptors, propertyNameFilters, minSequenceContext);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) descriptors.Select<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) (identityDescriptor =>
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity;
        descriptorToIdentityMap.TryGetValue(identityDescriptor, out identity);
        return identity;
      })).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByBatch(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      SequenceContext minSequenceContext,
      bool includeRestrictedVisibility)
    {
      if (descriptors == null)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> collection = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (this.m_readIdentitiesBatchSize <= 0)
        return this.ReadIdentities(requestContext, descriptors, queryMembership, propertyNameFilters, minSequenceContext, includeRestrictedVisibility);
      foreach (IList<IdentityDescriptor> descriptors1 in descriptors.Batch<IdentityDescriptor>(this.m_readIdentitiesBatchSize))
        collection.AddRange<Microsoft.VisualStudio.Services.Identity.Identity, IList<Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadIdentities(requestContext, descriptors1, queryMembership, propertyNameFilters, minSequenceContext, includeRestrictedVisibility));
      return collection;
    }

    internal IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      SequenceContext minSequenceContext,
      bool includeRestrictedVisibility = false,
      bool bypassCache = false)
    {
      requestContext.TraceEnter(0, "IdentityService", "BusinessLogic", nameof (ReadIdentities));
      if (descriptors == null)
      {
        requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentities));
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      this.CheckIdentityReadPermissions(requestContext, 1);
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      if (requestContext.IsTracing(80990, TraceLevel.Info, "IdentityService", "BusinessLogic"))
      {
        string callStack = requestContext.IsTracing(80991, TraceLevel.Info, "IdentityService", "BusinessLogic") ? Environment.StackTrace : string.Empty;
        IdentityTracing.TraceReadIdentity(nameof (PlatformIdentityService), descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility, callStack);
      }
      this.TraceApplicationLevelDescriptorRead(requestContext, descriptors);
      IdentityDescriptor[] descriptors1 = new IdentityDescriptor[descriptors.Count];
      for (int index = 0; index < descriptors.Count; ++index)
      {
        IdentityValidation.CheckDescriptor(descriptors[index], "descriptors element");
        descriptors1[index] = this.Domain.MapFromWellKnownIdentifier(descriptors[index]);
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.IdentityStore.ReadIdentities(requestContext, this.Domain, (IList<IdentityDescriptor>) descriptors1, queryMembership, propertyNameFilters != null, propertyNameFilters, minSequenceContext, includeRestrictedVisibility, bypassCache);
      this.FilterPropertiesInIdentityExtension(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) results, propertyNameFilters);
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      bool checkPermissions = !requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);
      requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentities));
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = this.FilterReadResults(requestContext, results, false, checkPermissions);
      if (IdentityTranslationHelper.IsEnabled(requestContext))
      {
        requestContext.Trace(80700, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities");
        service.PublishDecisionPoint(requestContext, (object) new AfterReadIdentitiesOnService()
        {
          IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
          {
            Identities = identities
          },
          Results = identities,
          ChildQueryMembership = queryMembership,
          ParentQueryMembership = queryMembership,
          IdentitiesInCurrentScope = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null
        });
      }
      else
        requestContext.Trace(80701, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled.  Skipping identity translation.");
      this.ReplacePropertiesInProfile(requestContext, identities, propertyNameFilters);
      return identities;
    }

    internal virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromStore(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false,
      bool filterResults = true)
    {
      requestContext.TraceEnter(0, "IdentityService", "BusinessLogic", nameof (ReadIdentitiesFromStore));
      if (descriptors.IsNullOrEmpty<IdentityDescriptor>())
      {
        requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentitiesFromStore));
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = this.IdentityStore.ReadIdentities(requestContext, this.Domain, descriptors, queryMembership, propertyNameFilters != null, propertyNameFilters, includeRestrictedVisibility, filterResults: filterResults);
      requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentitiesFromStore));
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      return this.ReadIdentities(requestContext, identityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility, true);
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      bool filterResults)
    {
      if (identityIds == null)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> collection1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Guid> collection2 = (IList<Guid>) new List<Guid>();
      if (this.m_readIdentitiesBatchSize <= 0)
        return this.ReadIdentities(requestContext, identityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility, filterResults, (SequenceContext) null);
      foreach (IList<Guid> guidList in identityIds.Batch<Guid>(this.m_readIdentitiesBatchSize))
      {
        collection1.AddRange<Microsoft.VisualStudio.Services.Identity.Identity, IList<Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.ReadIdentities(requestContext, guidList, queryMembership, propertyNameFilters, includeRestrictedVisibility, filterResults, (SequenceContext) null));
        collection2.AddRange<Guid, IList<Guid>>((IEnumerable<Guid>) guidList);
      }
      for (int index = 0; index < identityIds.Count; ++index)
        identityIds[index] = collection2[index];
      return collection1;
    }

    internal IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      bool filterResults,
      SequenceContext minSequenceContext,
      bool bypassCache = false)
    {
      requestContext.TraceEnter(0, "IdentityService", "BusinessLogic", nameof (ReadIdentities));
      if (identityIds == null)
      {
        requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentities));
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      if (minSequenceContext == null)
        requestContext.RootContext.Items.TryGetValue<SequenceContext>(RequestContextItemsKeys.MinSequenceContext, out minSequenceContext);
      this.CheckIdentityReadPermissions(requestContext, 1);
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      if (requestContext.IsTracing(80990, TraceLevel.Info, "IdentityService", "BusinessLogic"))
      {
        string callStack = requestContext.IsTracing(80991, TraceLevel.Info, "IdentityService", "BusinessLogic") ? Environment.StackTrace : string.Empty;
        IdentityTracing.TraceReadIdentity(nameof (PlatformIdentityService), identityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility, callStack);
      }
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      if (IdentityTranslationHelper.IsEnabled(requestContext))
      {
        requestContext.Trace(80702, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
        service.PublishDecisionPoint(requestContext, (object) new BeforeReadIdentitiesOnService()
        {
          IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
          {
            IdentityIds = identityIds
          }
        });
      }
      else
        requestContext.Trace(80703, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
      PlatformIdentityStore identityStore = this.IdentityStore;
      IVssRequestContext requestContext1 = requestContext;
      IdentityDomain domain = this.Domain;
      IList<Guid> identityIds1 = identityIds;
      int num1 = (int) queryMembership;
      int num2 = propertyNameFilters != null ? 1 : 0;
      IEnumerable<string> propertyNameFilters1 = propertyNameFilters;
      SequenceContext minSequenceContext1 = minSequenceContext;
      int num3 = includeRestrictedVisibility ? 1 : 0;
      bool flag = filterResults;
      int num4 = bypassCache ? 1 : 0;
      int num5 = flag ? 1 : 0;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityStore.ReadIdentities(requestContext1, domain, identityIds1, (QueryMembership) num1, num2 != 0, propertyNameFilters1, minSequenceContext1, num3 != 0, num4 != 0, num5 != 0);
      this.FilterPropertiesInIdentityExtension(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) results, propertyNameFilters);
      if (IdentityTranslationHelper.IsEnabled(requestContext))
      {
        requestContext.Trace(80704, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
        service.PublishDecisionPoint(requestContext, (object) new AfterReadIdentitiesOnService()
        {
          IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
          {
            Identities = results
          },
          Results = results,
          ChildQueryMembership = queryMembership,
          ParentQueryMembership = queryMembership,
          IdentitiesInCurrentScope = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null
        });
      }
      else
        requestContext.Trace(80705, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
      bool checkPermissions = !requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = this.FilterReadResults(requestContext, results, false, checkPermissions);
      this.ReplacePropertiesInProfile(requestContext, identities, propertyNameFilters);
      requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentities));
      return identities;
    }

    internal virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromStore(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false,
      bool filterResults = true)
    {
      requestContext.TraceEnter(0, "IdentityService", "BusinessLogic", nameof (ReadIdentitiesFromStore));
      if (identityIds.IsNullOrEmpty<Guid>())
      {
        requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentitiesFromStore));
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = this.IdentityStore.ReadIdentities(requestContext, this.Domain, identityIds, queryMembership, propertyNameFilters != null, propertyNameFilters, includeRestrictedVisibility, filterResults: filterResults);
      requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentitiesFromStore));
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ReadIdentities(requestContext, searchFactor, factorValue, queryMembership, propertyNameFilters, ReadIdentitiesOptions.None);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      ReadIdentitiesOptions options)
    {
      requestContext.TraceEnter(0, "IdentityService", "BusinessLogic", nameof (ReadIdentities));
      if (searchFactor == IdentitySearchFilter.TeamGroupName)
        searchFactor = IdentitySearchFilter.LocalGroupName;
      IdentityValidation.CheckFactorAndValue(searchFactor, ref factorValue);
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      IdentitySearchFilter identitySearchFilter = searchFactor;
      int permission = searchFactor == IdentitySearchFilter.LocalGroupName ? 1 : 2;
      this.CheckIdentityReadPermissions(requestContext, permission);
      if (identitySearchFilter == IdentitySearchFilter.LocalGroupName)
        searchFactor = IdentitySearchFilter.AccountName;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      if (requestContext.IsTracing(80990, TraceLevel.Info, "IdentityService", "BusinessLogic"))
      {
        string callStack = requestContext.IsTracing(80991, TraceLevel.Info, "IdentityService", "BusinessLogic") ? Environment.StackTrace : string.Empty;
        IdentityTracing.TraceReadIdentity(nameof (PlatformIdentityService), searchFactor, factorValue, queryMembership, propertyNameFilters, options, callStack);
      }
      switch (searchFactor)
      {
        case IdentitySearchFilter.DisplayName:
        case IdentitySearchFilter.General:
          IdentityDisplayName disambiguatedSearchTerm = IdentityDisplayName.GetDisambiguatedSearchTerm(factorValue);
          if (requestContext.IsTracing(80310, TraceLevel.Info, "IdentityService", "BusinessLogic"))
            requestContext.Trace(80310, TraceLevel.Info, "IdentityService", "BusinessLogic", string.Format("Search term type returned as - {0}", (object) disambiguatedSearchTerm.Type));
          if (disambiguatedSearchTerm.Type != SearchTermType.Unknown)
          {
            if (requestContext.IsTracing(80311, TraceLevel.Info, "IdentityService", "BusinessLogic"))
              requestContext.Trace(80311, TraceLevel.Info, "IdentityService", "BusinessLogic", string.Format("Before GetDisambiguatedSearchTerm value of searchFactor - {0} factorValue - {1}", (object) searchFactor, (object) factorValue));
            if (disambiguatedSearchTerm.Type == SearchTermType.Scope)
            {
              Guid scopeId = this.IdentityStore.MapScopeId(requestContext, this.Domain, disambiguatedSearchTerm.ScopeId);
              List<Microsoft.VisualStudio.Services.Identity.Identity> identities = this.IdentityStore.ReadGroupsFromDatabase(requestContext, this.Domain, scopeId, (string) null, disambiguatedSearchTerm.DisplayName, queryMembership, false);
              this.ReplacePropertiesInProfile(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities, propertyNameFilters);
              return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities;
            }
            if (disambiguatedSearchTerm.Type == SearchTermType.Vsid)
              results1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.IdentityStore.ReadIdentities(requestContext, this.Domain, (IList<Guid>) new Guid[1]
              {
                disambiguatedSearchTerm.Vsid
              }, queryMembership, (propertyNameFilters != null ? 1 : 0) != 0, propertyNameFilters);
            else if (disambiguatedSearchTerm.Type == SearchTermType.DomainAndAccountName)
            {
              searchFactor = IdentitySearchFilter.AccountName;
              factorValue = string.Format("{0}\\{1}", (object) disambiguatedSearchTerm.Domain, (object) disambiguatedSearchTerm.AccountName);
            }
            else if (disambiguatedSearchTerm.Type == SearchTermType.AccoutName)
            {
              searchFactor = IdentitySearchFilter.AccountName;
              factorValue = disambiguatedSearchTerm.AccountName;
            }
            if (requestContext.IsTracing(80311, TraceLevel.Info, "IdentityService", "BusinessLogic"))
            {
              requestContext.Trace(80311, TraceLevel.Info, "IdentityService", "BusinessLogic", string.Format("After GetDisambiguatedSearchTerm value of searchFactor - {0} factorValue - {1}", (object) searchFactor, (object) factorValue));
              break;
            }
            break;
          }
          break;
        case IdentitySearchFilter.Identifier:
          factorValue = this.Domain.MapFromWellKnownIdentifier(factorValue);
          break;
        case IdentitySearchFilter.DirectoryAlias:
          if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          {
            searchFactor = IdentitySearchFilter.AccountName;
            break;
          }
          break;
      }
      if (results1 == null)
      {
        if (searchFactor == IdentitySearchFilter.AccountName && options.HasFlag((Enum) ReadIdentitiesOptions.FilterIllegalMemberships))
          factorValue = IdentityHelper.QualifyAccountName(requestContext, factorValue);
        IList<Microsoft.VisualStudio.Services.Identity.Identity> results2 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.IdentityStore.ReadIdentities(requestContext, this.Domain, searchFactor, factorValue, queryMembership, propertyNameFilters != null, propertyNameFilters);
        results1 = PlatformIdentityService.FilterCspIdentities(requestContext, searchFactor, factorValue, results2);
      }
      if (identitySearchFilter == IdentitySearchFilter.LocalGroupName)
        results1 = PlatformIdentityService.FilterLocalGroupIdentities(requestContext, results1);
      this.FilterPropertiesInIdentityExtension(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) results1, propertyNameFilters);
      if (IdentityTranslationHelper.IsEnabled(requestContext))
      {
        requestContext.Trace(80312, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
        requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new AfterReadIdentitiesOnService()
        {
          IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
          {
            Identities = results1
          },
          Results = results1,
          ChildQueryMembership = queryMembership,
          ParentQueryMembership = queryMembership,
          IdentitiesInCurrentScope = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null
        });
      }
      else
        requestContext.Trace(80313, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
      bool checkPermissions = !requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);
      requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (ReadIdentities));
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities1 = this.FilterReadResults(requestContext, results1, false, checkPermissions);
      this.ReplacePropertiesInProfile(requestContext, identities1, propertyNameFilters);
      return identities1;
    }

    IList<Microsoft.VisualStudio.Services.Identity.Identity> IPlatformIdentityServiceInternal.ReadAggregateIdentitiesFromDatabase(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aggregateIdentityDescriptors)
    {
      return this.IdentityStore.ReadAggregateIdentitiesFromDatabase(requestContext, this.Domain, aggregateIdentityDescriptors);
    }

    Microsoft.VisualStudio.Services.Identity.Identity[] IPlatformIdentityServiceInternal.ReadGroupsFromDatabase(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> groupIdentityDescriptors)
    {
      return this.IdentityStore.ReadGroupsFromDatabase(requestContext, this.Domain, groupIdentityDescriptors);
    }

    Microsoft.VisualStudio.Services.Identity.Identity[] IPlatformIdentityServiceInternal.ReadGroupsFromDatabase(
      IVssRequestContext requestContext,
      IList<Guid> groupIds)
    {
      return this.IdentityStore.ReadGroupsFromDatabase(requestContext, this.Domain, (IList<IdentityDescriptor>) null, groupIds, QueryMembership.None);
    }

    IList<Microsoft.VisualStudio.Services.Identity.Identity> IPlatformIdentityServiceInternal.ReadAadGroupsFromDatabase(
      IVssRequestContext requestContext,
      bool readInactive)
    {
      return this.IdentityStore.ReadAadGroupsFromDatabase(requestContext, readInactive);
    }

    public override IdentitySearchResult SearchIdentities(
      IVssRequestContext requestContext,
      IdentitySearchParameters searchParameters)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IdentitySearchParameters>(searchParameters, nameof (searchParameters));
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new InvalidRequestContextHostException(FrameworkResources.ApplicationHostRequired());
      if (searchParameters.ScopeId != new Guid())
      {
        this.CheckPermission(requestContext, searchParameters.ScopeId.ToString("D"), 1, true);
        searchParameters.ScopeId = this.IdentityStore.MapScopeId(requestContext, this.Domain, searchParameters.ScopeId);
      }
      return this.IdentityStore.SearchIdentities(requestContext, this.Domain, searchParameters);
    }

    public override void RefreshSearchIdentitiesCache(
      IVssRequestContext requestContext,
      Guid scopeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.IdentityStore.RefreshSearchIdentitiesCache(requestContext, this.Domain, scopeId);
    }

    public override FilteredIdentitiesList ReadFilteredIdentities(
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
      if (scopeId != Guid.Empty)
        scopeId = this.IdentityStore.MapScopeId(requestContext, this.Domain, scopeId);
      FilteredIdentitiesList filteredIdentitiesList = this.IdentityStore.ReadFilteredIdentities(requestContext, this.Domain, scopeId, descriptors, filters, suggestedPageSize, lastSearchResult, lookForward, queryMembership, propertyNameFilters);
      this.FilterPropertiesInIdentityExtension(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) filteredIdentitiesList.Items, propertyNameFilters);
      if (IdentityTranslationHelper.IsEnabled(requestContext))
      {
        requestContext.Trace(80706, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
        requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new AfterReadIdentitiesOnService()
        {
          IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
          {
            Identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) filteredIdentitiesList.Items
          }
        });
      }
      else
        requestContext.Trace(80707, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
      this.ReplacePropertiesInProfile(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) filteredIdentitiesList.Items, propertyNameFilters);
      return filteredIdentitiesList;
    }

    void ITransferIdentityRightsService.TransferIdentityRights(
      IVssRequestContext collectionContext,
      IList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>> identityTranslations,
      bool validateSourceData)
    {
      collectionContext.TraceEnter(80600, "AccountLinking", "IdentityService", "TransferIdentityRights");
      try
      {
        this.TransferIdentityRights(collectionContext, (IList<IdentityIdTranslation>) identityTranslations.Select<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>, IdentityIdTranslation>((Func<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>, IdentityIdTranslation>) (x => new IdentityIdTranslation()
        {
          Id = x.Item1.Id,
          MasterId = x.Item2.MasterId
        })).ToList<IdentityIdTranslation>(), (IList<KeyValuePair<Guid, Guid>>) identityTranslations.Select<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>, KeyValuePair<Guid, Guid>>((Func<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>, KeyValuePair<Guid, Guid>>) (x => new KeyValuePair<Guid, Guid>(x.Item1.MasterId, x.Item2.MasterId))).ToList<KeyValuePair<Guid, Guid>>(), validateSourceData);
      }
      finally
      {
        collectionContext.TraceLeave(80609, "AccountLinking", "IdentityService", "TransferIdentityRights");
      }
    }

    void ITransferIdentityRightsService.TransferIdentityRights(
      IVssRequestContext collectionContext,
      IList<IdentityRightsTransfer> identityRightsTransfers)
    {
      collectionContext.TraceEnter(80610, "AccountLinking", "IdentityService", "TransferIdentityRights");
      try
      {
        this.TransferIdentityRights(collectionContext, (IList<IdentityIdTranslation>) identityRightsTransfers.Select<IdentityRightsTransfer, IdentityIdTranslation>((Func<IdentityRightsTransfer, IdentityIdTranslation>) (x => new IdentityIdTranslation()
        {
          Id = x.SourceId,
          MasterId = x.TargetMasterId
        })).ToList<IdentityIdTranslation>(), (IList<KeyValuePair<Guid, Guid>>) identityRightsTransfers.Select<IdentityRightsTransfer, KeyValuePair<Guid, Guid>>((Func<IdentityRightsTransfer, KeyValuePair<Guid, Guid>>) (x => new KeyValuePair<Guid, Guid>(x.SourceMasterId, x.TargetMasterId))).ToList<KeyValuePair<Guid, Guid>>());
      }
      finally
      {
        collectionContext.TraceLeave(80619, "AccountLinking", "IdentityService", "TransferIdentityRights");
      }
    }

    private void TransferIdentityRights(
      IVssRequestContext collectionContext,
      IList<IdentityIdTranslation> identityIdTranslations,
      IList<KeyValuePair<Guid, Guid>> identityRightsMap,
      bool validateSourceData = false)
    {
      collectionContext.TraceEnter(80400, "AccountLinking", "IdentityService", nameof (TransferIdentityRights));
      try
      {
        collectionContext.Trace(80401, TraceLevel.Info, "AccountLinking", "IdentityService", "Identity id translations count: {0}", (object) identityIdTranslations.Count);
        collectionContext.Trace(80402, TraceLevel.Info, "AccountLinking", "IdentityService", "Identity rights map count: {0}", (object) identityRightsMap.Count);
        collectionContext.Trace(80403, TraceLevel.Info, "AccountLinking", "IdentityService", "Transfering identity rights.");
        this.IdentityStore.TransferIdentityRights(collectionContext, this.Domain, (IEnumerable<KeyValuePair<Guid, Guid>>) identityRightsMap, identityIdTranslations, validateSourceData);
      }
      finally
      {
        collectionContext.TraceLeave(80409, "AccountLinking", "IdentityService", nameof (TransferIdentityRights));
      }
    }

    public void InvalidateIdentities(IVssRequestContext requestContext, IList<Guid> identityIds)
    {
      requestContext.TraceEnter(80410, "AccountLinking", "IdentityService", nameof (InvalidateIdentities));
      try
      {
        requestContext.Trace(80411, TraceLevel.Info, "AccountLinking", "IdentityService", "Invalidating identities: {0}.", (object) identityIds.Count);
        this.IdentityStore.InvalidateIdentities(requestContext, this.Domain, identityIds);
      }
      finally
      {
        requestContext.TraceLeave(80419, "AccountLinking", "IdentityService", nameof (InvalidateIdentities));
      }
    }

    bool IPlatformIdentityServiceInternal.DeleteHistoricalIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds)
    {
      requestContext.TraceEnter(80510, "IdentityService", "BusinessLogic", "DeleteHistoricalIdentities");
      try
      {
        requestContext.Trace(80511, TraceLevel.Info, "IdentityService", "BusinessLogic", "Deleting historical identities: {0}.", (object) identityIds.Count);
        requestContext.TraceSerializedConditionally(805112, TraceLevel.Info, "IdentityService", "BusinessLogic", "Deleting historical identities: {0}.", (object) identityIds);
        return this.IdentityStore.DeleteHistoricalIdentities(requestContext, identityIds);
      }
      finally
      {
        requestContext.TraceLeave(80519, "IdentityService", "BusinessLogic", "DeleteHistoricalIdentities");
      }
    }

    bool IPlatformIdentityServiceInternal.TryUpgradeHistoricalIdentityToClaimsIdentity(
      IVssRequestContext collectionContext,
      Guid identityId,
      out Microsoft.VisualStudio.Services.Identity.Identity claimsIdentity)
    {
      collectionContext.TraceEnter(80640, "IdentityService", "BusinessLogic", "TryUpgradeHistoricalIdentityToClaimsIdentity");
      try
      {
        IVssRequestContext context = collectionContext.To(TeamFoundationHostType.Deployment);
        IdentityService service = context.GetService<IdentityService>();
        Guid identityId1 = identityId;
        IList<IdentityIdTranslation> idTranslation = (IList<IdentityIdTranslation>) null;
        IVssRequestContext requestContext = context;
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity1 = service.ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
        {
          identityId
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        claimsIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (readIdentity1 == null || !readIdentity1.IsClaims)
          return false;
        collectionContext.Trace(80641, TraceLevel.Info, "IdentityService", "BusinessLogic", "Trying to upgrade historical identity to claims identity: {0}.", (object) identityId);
        if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Identity.UpdateTranslationWhenUpgradingHistoricalIdentity"))
        {
          Microsoft.VisualStudio.Services.Identity.Identity readIdentity2 = this.ReadIdentities(collectionContext, (IList<Guid>) new List<Guid>()
          {
            identityId
          }, QueryMembership.None, (IEnumerable<string>) null, false)[0];
          if (readIdentity2.MasterId != identityId)
          {
            idTranslation = (IList<IdentityIdTranslation>) new List<IdentityIdTranslation>()
            {
              new IdentityIdTranslation()
              {
                Id = identityId,
                MasterId = identityId
              }
            };
            identityId1 = readIdentity2.MasterId;
            collectionContext.TraceSerializedConditionally(80948, TraceLevel.Info, "IdentityService", "BusinessLogic", "Created self mapping for claims identity {0} and removing historical identity {1}", (object) identityId, (object) readIdentity2.MasterId);
          }
        }
        bool claimsIdentity1 = this.IdentityStore.SafelyRemoveHistoricalIdentity(collectionContext, identityId1, idTranslation);
        if (claimsIdentity1)
          claimsIdentity = readIdentity1;
        collectionContext.TraceSerializedConditionally(80642, TraceLevel.Info, "IdentityService", "BusinessLogic", "Tried to upgrade historical identity to claims identity: upgraded = {0}, claimsIdentity = {1}.", (object) claimsIdentity1, (object) claimsIdentity);
        return claimsIdentity1;
      }
      finally
      {
        collectionContext.TraceLeave(80649, "IdentityService", "BusinessLogic", "TryUpgradeHistoricalIdentityToClaimsIdentity");
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      scopeId = scopeId == Guid.Empty ? this.Domain.DomainId : this.IdentityStore.MapScopeId(requestContext, this.Domain, scopeId);
      if (requestContext.IsTracing(80990, TraceLevel.Info, "IdentityService", "BusinessLogic"))
      {
        string callStack = requestContext.IsTracing(80991, TraceLevel.Info, "IdentityService", "BusinessLogic") ? Environment.StackTrace : string.Empty;
        IdentityTracing.TraceReadIdentity(nameof (PlatformIdentityService), scopeId, queryMembership, propertyNameFilters, callStack);
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.IdentityStore.ReadIdentitiesInScope(requestContext, this.Domain, scopeId, queryMembership, propertyNameFilters);
      this.FilterPropertiesInIdentityExtension(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList, propertyNameFilters);
      this.PublishIfTranslationIsEnabled(requestContext, identityList);
      this.ReplacePropertiesInProfile(requestContext, identityList, propertyNameFilters);
      return identityList;
    }

    public IdentitiesPage ReadIdentitiesByScopeByPage(
      IVssRequestContext requestContext,
      ScopePagingContext scopePagingContext,
      bool forceFilterIdentities = false)
    {
      this.CheckPermission(requestContext, scopePagingContext.ScopeId.ToString("D"), 1, true);
      Guid scopeId = this.IdentityStore.MapScopeId(requestContext, this.Domain, scopePagingContext.ScopeId);
      if (requestContext.IsTracing(80880, TraceLevel.Info, "IdentityService", "BusinessLogic"))
      {
        string callStack = requestContext.IsTracing(80881, TraceLevel.Info, "IdentityService", "BusinessLogic") ? Environment.StackTrace : string.Empty;
        IdentityTracing.TraceReadIdentity(nameof (PlatformIdentityService), scopeId, QueryMembership.None, (IEnumerable<string>) null, callStack);
      }
      IdentitiesPage identitiesPage = this.IdentityStore.ReadIdentitiesInScopeByPage(requestContext, this.Domain, scopeId, scopePagingContext);
      identitiesPage.Identities = this.FilterReadResults(requestContext, identitiesPage.Identities, true, forceFilterIdentities: forceFilterIdentities);
      this.FilterPropertiesInIdentityExtension(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identitiesPage.Identities, (IEnumerable<string>) null);
      this.PublishIfTranslationIsEnabled(requestContext, identitiesPage.Identities);
      return identitiesPage;
    }

    private void PublishIfTranslationIsEnabled(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results)
    {
      if (IdentityTranslationHelper.IsEnabled(requestContext))
      {
        requestContext.Trace(80708, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
        requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new AfterReadIdentitiesOnService()
        {
          IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
          {
            Identities = results
          }
        });
      }
      else
        requestContext.Trace(80709, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
    }

    private void ReplacePropertiesInProfile(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IEnumerable<string> propertyNameFilters)
    {
      using (requestContext.TraceBlock(80314, 80318, "IdentityService", "BusinessLogic", nameof (ReplacePropertiesInProfile)))
      {
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.ProfileAvatar.Disabled"))
          return;
        bool flag1 = propertyNameFilters != null && propertyNameFilters.Contains<string>("Microsoft.TeamFoundation.Identity.CandidateImage.Data");
        bool flag2 = propertyNameFilters != null && propertyNameFilters.Contains<string>("Microsoft.TeamFoundation.Identity.Image.Data");
        if (propertyNameFilters != null && propertyNameFilters.Contains<string>("*"))
        {
          flag1 = true;
          flag2 = true;
        }
        if (!flag2 && !flag1)
          return;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        IUserService service = vssRequestContext.GetService<IUserService>();
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
        {
          try
          {
            if (identity != null)
            {
              if (!identity.IsContainer)
              {
                if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.ProfileAvatarCallsTrace.Enabled"))
                  requestContext.Trace(80314, TraceLevel.Error, "IdentityService", "BusinessLogic", "Had to fetch avatar from profile for identity: {0}", (object) identity.Id);
                if (flag2)
                {
                  Avatar avatar = service.GetAvatar(vssRequestContext, identity.Id);
                  if (avatar != null)
                    identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Data", (object) avatar.Image);
                }
                if (flag1)
                {
                  UserAttribute userAttribute = (UserAttribute) null;
                  try
                  {
                    userAttribute = service.GetAttribute(vssRequestContext, identity.Id, WellKnownUserAttributeNames.AvatarPreview);
                  }
                  catch (UserAttributeDoesNotExistException ex)
                  {
                    requestContext.TraceException(80323, TraceLevel.Warning, "IdentityService", "BusinessLogic", (Exception) ex);
                  }
                  if (userAttribute != null)
                  {
                    byte[] numArray = Convert.FromBase64String(userAttribute.Value);
                    identity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", (object) numArray);
                  }
                }
                identity.ResetModifiedProperties();
              }
            }
          }
          catch (UserDoesNotExistException ex)
          {
            requestContext.TraceCatch(80316, "IdentityService", "BusinessLogic", (Exception) ex);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(80317, "IdentityService", "BusinessLogic", ex);
          }
        }
      }
    }

    public virtual IdentitySnapshot ReadIdentitySnapshot(
      IVssRequestContext requestContext,
      string scopeId,
      HashSet<Guid> excludedIdentities,
      bool readAllIdentities = false,
      bool readInactiveMemberships = false)
    {
      Guid scopeIdGuid = Guid.Parse(scopeId);
      return this.IdentityStore.ReadIdentitySnapshotFromDatabase(requestContext, scopeIdGuid, excludedIdentities, readAllIdentities, readInactiveMemberships);
    }

    public virtual IList<Guid> GetIdentityIdsByDomainId(
      IVssRequestContext requestContext,
      Guid domainId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
        throw new InvalidAccessException(FrameworkResources.InvalidAccessException());
      return this.IdentityStore.GetIdentityIdsByDomainIdFromDatabase(requestContext, new byte[2]
      {
        IdentityTypeMapper.Instance.GetTypeIdFromName("Microsoft.TeamFoundation.BindPendingIdentity"),
        IdentityTypeMapper.Instance.GetTypeIdFromName("Microsoft.IdentityModel.Claims.ClaimsIdentity")
      }, domainId);
    }

    public virtual IList<Guid> GetUserIdentityIdsByDomain(
      IVssRequestContext requestContext,
      Guid? domain)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
        throw new InvalidAccessException(FrameworkResources.InvalidAccessException());
      return this.IdentityStore.GetUserIdentityIdsByDomainFromDatabase(requestContext, domain);
    }

    public IList<Guid> ReadIdentityIdsInScope(IVssRequestContext requestContext, Guid scopeId)
    {
      ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
      return this.IdentityStore.ReadIdentityIdsInScope(requestContext, scopeId);
    }

    protected override ChangedIdentities GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext)
    {
      return this.GetIdentityChanges(requestContext, sequenceContext, this.Domain.DomainId);
    }

    public ChangedIdentities GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext,
      Guid scopeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ChangedIdentitiesContext>(sequenceContext, nameof (sequenceContext));
      this.CheckPermission(requestContext, (string) null, 1, true);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && sequenceContext.GroupSequenceId.Equals(int.MaxValue) && sequenceContext.IdentitySequenceId.Equals(int.MaxValue) && (sequenceContext.OrganizationIdentitySequenceId == int.MaxValue || sequenceContext.OrganizationIdentitySequenceId == -1))
      {
        SequenceContext currentSequenceContext = this.IdentityStore.GetCurrentSequenceContext(requestContext, this.Domain);
        return new ChangedIdentities((IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>(), new ChangedIdentitiesContext(checked ((int) currentSequenceContext.IdentitySequenceId), checked ((int) currentSequenceContext.GroupSequenceId), checked ((int) currentSequenceContext.OrganizationIdentitySequenceId)), false);
      }
      using (CommandGetIdentityChanges command = new CommandGetIdentityChanges(requestContext, this.Domain))
      {
        command.Execute(sequenceContext.IdentitySequenceId, sequenceContext.GroupSequenceId, sequenceContext.OrganizationIdentitySequenceId, this.IdentityStore, scopeId, sequenceContext.PageSize);
        requestContext.TraceSerializedConditionally(850003, TraceLevel.Verbose, "IdentityService", "BusinessLogic", false, "Identities returned by CommandGetIdentityChanges (pre-translation): {0}", (object) command.Identities);
        if (IdentityTranslationHelper.IsEnabled(requestContext))
        {
          requestContext.Trace(80260, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
          requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new AfterGetIdentityChangesOnService()
          {
            IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
            {
              Identities = command.Identities
            }
          });
        }
        else
          requestContext.Trace(80261, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
        if (command.Identities != null && command.Identities.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = command.Identities;
          foreach (Tuple<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>> tuple in identities.GroupBy<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (i => i.Id)).Select<IGrouping<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Tuple<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>>((Func<IGrouping<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Tuple<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>>) (g => new Tuple<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>(g.Key, g.ToList<Microsoft.VisualStudio.Services.Identity.Identity>()))).Where<Tuple<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>>((Func<Tuple<Guid, List<Microsoft.VisualStudio.Services.Identity.Identity>>, bool>) (tuple => tuple.Item2 != null && tuple.Item2.Count > 1)))
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = tuple.Item2.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.Id == i.MasterId));
            if (identity != null)
              identities.Remove(identity);
          }
        }
        IReadOnlyList<IdentityIdTranslation> identityIdTranslations = this.GetIdentityIdTranslations(requestContext);
        if (identityIdTranslations.Count > 0)
        {
          HashSet<Guid> localIds = new HashSet<Guid>(identityIdTranslations.Select<IdentityIdTranslation, Guid>((Func<IdentityIdTranslation, Guid>) (x => x.Id)));
          List<Microsoft.VisualStudio.Services.Identity.Identity> localIdentities = command.Identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => !i.IsActive && localIds.Contains(i.MasterId))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (localIdentities.Count > 0)
          {
            requestContext.TraceConditionally(850004, TraceLevel.Verbose, "IdentityService", "BusinessLogic", (Func<string>) (() => "Removing local identities from the GetIdentityChanges results.  Identities removed: [" + string.Join("; ", localIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => x.ToString()))) + "]"));
            localIdentities.ForEach((Action<Microsoft.VisualStudio.Services.Identity.Identity>) (localIdentity => command.Identities.Remove(localIdentity)));
          }
        }
        bool moreData = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.IgnorePayload") ? command.LastSequenceId.Item3 > sequenceContext.OrganizationIdentitySequenceId : command.LastSequenceId.Item3 > sequenceContext.OrganizationIdentitySequenceId && command.Identities != null && command.Identities.Count > 0;
        ChangedIdentities identityChanges = new ChangedIdentities(command.Identities, new ChangedIdentitiesContext(command.LastSequenceId.Item1, command.LastSequenceId.Item2, command.LastSequenceId.Item3), moreData);
        if (requestContext.IsTracing(850001, TraceLevel.Verbose, "IdentityService", "BusinessLogic"))
          requestContext.Trace(850001, TraceLevel.Verbose, "IdentityService", "BusinessLogic", string.Format("Returning {0} identities, identitySequenceId {1}, groupSequenceId {2} for scope {3} with identitySequenceId {4}, groupSequenceId {5}.", (object) identityChanges?.Identities?.Count, (object) identityChanges?.SequenceContext?.IdentitySequenceId, (object) identityChanges?.SequenceContext?.GroupSequenceId, (object) scopeId, (object) sequenceContext.IdentitySequenceId, (object) sequenceContext.GroupSequenceId));
        if (requestContext.IsTracing(850002, TraceLevel.Verbose, "IdentityService", "BusinessLogic"))
        {
          IVssRequestContext requestContext1 = requestContext;
          object[] objArray = new object[7]
          {
            (object) identityChanges?.Identities?.Count,
            (object) identityChanges?.SequenceContext?.IdentitySequenceId,
            (object) identityChanges?.SequenceContext?.GroupSequenceId,
            (object) scopeId,
            (object) sequenceContext.IdentitySequenceId,
            (object) sequenceContext.GroupSequenceId,
            null
          };
          string str;
          if (identityChanges == null)
          {
            str = (string) null;
          }
          else
          {
            IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = identityChanges.Identities;
            str = identities != null ? identities.Serialize<IList<Microsoft.VisualStudio.Services.Identity.Identity>>() : (string) null;
          }
          objArray[6] = (object) str;
          string message = string.Format("Returning {0} identities, identitySequenceId {1}, groupSequenceId {2} for scope {3} with identitySequenceId {4}, groupSequenceId {5}. {6}", objArray);
          requestContext1.Trace(850002, TraceLevel.Verbose, "IdentityService", "BusinessLogic", message);
        }
        if (identityIdTranslations.Count > 0)
        {
          Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
          string strA = organizationAadTenantId == Guid.Empty ? "Windows Live ID" : organizationAadTenantId.ToString();
          Dictionary<Guid, Guid> dictionary = identityIdTranslations.ToDictionary<IdentityIdTranslation, Guid, Guid>((Func<IdentityIdTranslation, Guid>) (t => t.MasterId), (Func<IdentityIdTranslation, Guid>) (t => t.Id));
          foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityChanges.Identities)
          {
            StringBuilder stringBuilder = new StringBuilder();
            if (dictionary.ContainsKey(identity.Id))
              stringBuilder.Append("Local ID found in the set of master IDs from translation data");
            string property = identity.GetProperty<string>("Domain", string.Empty);
            if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableGetIdChangesDomainValidation") && strA != "Windows Live ID" && identity.IsActive && (identity.IsClaims || identity.IsBindPending) && IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) identity) && string.Compare(strA, property, StringComparison.OrdinalIgnoreCase) != 0)
            {
              if (stringBuilder.Length > 0)
                stringBuilder.Append(" and ");
              stringBuilder.Append("Current account's domain is not the same as Identity's domain");
            }
            if (stringBuilder.Length > 0)
            {
              IdentityIdTranslationService service = requestContext.GetService<IdentityIdTranslationService>();
              Guid guid = service.TranslateFromMasterId(requestContext, identity.MasterId);
              Guid masterId = service.TranslateToMasterId(requestContext, identity.Id);
              string message = string.Format("Failure Reason: {0}, IdentitySequenceId: {1}, GroupSequenceId: {2}, InvalidChangedIdentity: {3}, TenantId: {4}, Identity Domain: {5}, IdentityTranslationsInDB: {6}, CachedTranslationLocalId: {7}, CachedTranslationMasterId: {8}", (object) stringBuilder, (object) sequenceContext.IdentitySequenceId, (object) sequenceContext.GroupSequenceId, (object) identity.Serialize<Microsoft.VisualStudio.Services.Identity.Identity>(), (object) organizationAadTenantId, (object) property, (object) identityIdTranslations.Serialize<IReadOnlyList<IdentityIdTranslation>>(), (object) guid, (object) masterId);
              requestContext.Trace(15131010, TraceLevel.Error, "IdentityService", "BusinessLogic", message);
              throw new InvalidChangedIdentityException(string.Format("GetIdentityChanges failed for identity with local id: {0} and master id: {1} with failure reason: {2}", (object) identity.Id, (object) identity.MasterId, (object) stringBuilder));
            }
          }
        }
        return identityChanges;
      }
    }

    private IReadOnlyList<IdentityIdTranslation> GetIdentityIdTranslations(
      IVssRequestContext requestContext)
    {
      if (!IdentityTranslationHelper.IsEnabled(requestContext))
        return (IReadOnlyList<IdentityIdTranslation>) new List<IdentityIdTranslation>();
      IList<IdentityIdTranslation> translations = this.IdentityStore.GetTranslations(requestContext);
      return translations == null || translations.Count <= 0 ? (IReadOnlyList<IdentityIdTranslation>) new List<IdentityIdTranslation>() : (IReadOnlyList<IdentityIdTranslation>) translations.Where<IdentityIdTranslation>((Func<IdentityIdTranslation, bool>) (t => t.Id != t.MasterId)).ToList<IdentityIdTranslation>();
    }

    public override bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      return this.UpdateIdentities(requestContext, identities, false);
    }

    public override bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates)
    {
      requestContext.TraceEnter(80270, "IdentityService", "BusinessLogic", nameof (UpdateIdentities));
      if (requestContext.IsTracing(80271, TraceLevel.Info, "IdentityService", "BusinessLogic"))
        requestContext.Trace(80271, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "UpdateIdentities Stack Trace: {0}", (object) new StackTrace().ToString());
      requestContext.Trace(80272, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "UpdateIdentities Request Url: {0}", requestContext.RequestUri() != (Uri) null ? (object) requestContext.RequestUri().ToString() : (object) string.Empty);
      try
      {
        ArgumentUtility.CheckForNull<IList<Microsoft.VisualStudio.Services.Identity.Identity>>(identities, nameof (identities));
        this.UpdateAvatarsInProfile(requestContext, identities);
        return this.UpdateIdentitiesInternal(requestContext, identities, allowMetadataUpdates);
      }
      finally
      {
        requestContext.TraceLeave(80270, "IdentityService", "BusinessLogic", nameof (UpdateIdentities));
      }
    }

    private void UpdateAvatarsInProfile(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.ProfileAvatar.Disabled"))
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IUserService service = vssRequestContext.GetService<IUserService>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        try
        {
          object obj;
          bool property1 = identity.TryGetProperty("Microsoft.TeamFoundation.Identity.Image.Data", out obj);
          byte[] numArray = obj as byte[];
          if (property1 && numArray != null)
            service.UpdateAvatar(vssRequestContext, identity.Id, new Avatar()
            {
              Image = numArray
            });
          else if (property1)
            service.DeleteAvatar(vssRequestContext, identity.Id);
          byte[] property2 = identity.GetProperty<byte[]>("Microsoft.TeamFoundation.Identity.CandidateImage.Data", (byte[]) null);
          if (property2 != null)
            throw new NotImplementedException();
          if (numArray == null)
          {
            if (property2 == null)
              continue;
          }
          requestContext.Trace(80280, TraceLevel.Error, "IdentityService", "BusinessLogic", "Had to set avatar in profile from identity for identity: {0}", (object) identity.Id);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public override string GetSignoutToken(IVssRequestContext requestContext) => throw new NotImplementedException("Not available on Platform - use PlatformDelegatedAuthorizationService directly");

    protected override int GetCurrentSequenceId(IVssRequestContext requestContext) => this.IdentityStore.GetCurrentSequenceId(requestContext);

    internal long GetCurrentIdentitySequenceId(IVssRequestContext requestContext) => this.IdentityStore.GetCurrentIdentitySequenceId(requestContext);

    internal long GetCurrentGroupSequenceId(IVssRequestContext requestContext) => this.IdentityStore.GetCurrentGroupSequenceId(requestContext);

    protected override IdentityChanges GetIdentityChanges(
      IVssRequestContext requestContext,
      int sequenceId,
      long identitySequenceId = -1,
      long groupSequenceId = -1,
      long organizationIdentitySequenceId = -1)
    {
      IdentityChanges identityChanges = this.IdentityStore.GetIdentityChanges(requestContext, sequenceId, identitySequenceId, groupSequenceId, organizationIdentitySequenceId);
      if (identityChanges != null && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        if (identityChanges.DescriptorChanges != null)
        {
          if (IdentityTranslationHelper.IsEnabled(requestContext))
          {
            requestContext.Trace(80712, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
            service.PublishDecisionPoint(requestContext, (object) new BeforePublishMessageBusOnStore()
            {
              IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
              {
                IdentityIds = (IList<Guid>) identityChanges.DescriptorChanges
              }
            });
          }
          else
            requestContext.Trace(80713, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
        }
        if (identityChanges.IdentityChangeIds != null)
        {
          if (IdentityTranslationHelper.IsEnabled(requestContext))
          {
            requestContext.Trace(80714, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
            service.PublishDecisionPoint(requestContext, (object) new BeforePublishMessageBusOnStore()
            {
              IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
              {
                IdentityIds = (IList<Guid>) identityChanges.IdentityChangeIds
              }
            });
          }
          else
            requestContext.Trace(80715, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
        }
        if (identityChanges.MembershipChanges != null && identityChanges.MembershipChanges.Any<MembershipChangeInfo>())
        {
          List<Guid> list1 = identityChanges.MembershipChanges.Select<MembershipChangeInfo, Guid>((Func<MembershipChangeInfo, Guid>) (m => m.MemberId)).ToList<Guid>();
          IList<Guid> list2 = (IList<Guid>) identityChanges.MembershipChanges.Where<MembershipChangeInfo>((Func<MembershipChangeInfo, bool>) (x => !x.IsMemberGroup && x.MemberDescriptor == (IdentityDescriptor) null)).Select<MembershipChangeInfo, Guid>((Func<MembershipChangeInfo, Guid>) (m => m.MemberId)).Distinct<Guid>().ToList<Guid>();
          if (!list2.IsNullOrEmpty<Guid>())
          {
            IList<Microsoft.VisualStudio.Services.Identity.Identity> second = this.ReadIdentities(requestContext, list2, QueryMembership.None, (IEnumerable<string>) null, false);
            Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> collection = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
            if (second.Count == list2.Count)
              collection.AddRange<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>(list2.Zip<Guid, Microsoft.VisualStudio.Services.Identity.Identity, KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) second, (Func<Guid, Microsoft.VisualStudio.Services.Identity.Identity, KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>) ((id, identity) => new KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(id, identity))));
            foreach (MembershipChangeInfo membershipChangeInfo in identityChanges.MembershipChanges.Where<MembershipChangeInfo>((Func<MembershipChangeInfo, bool>) (x => !x.IsMemberGroup && x.MemberDescriptor == (IdentityDescriptor) null)))
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity;
              if (collection.TryGetValue(membershipChangeInfo.MemberId, out identity))
                membershipChangeInfo.MemberDescriptor = identity?.Descriptor;
            }
          }
          if (IdentityTranslationHelper.IsEnabled(requestContext))
          {
            requestContext.Trace(80716, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
            service.PublishDecisionPoint(requestContext, (object) new BeforePublishMessageBusOnStore()
            {
              IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
              {
                IdentityIds = (IList<Guid>) list1
              }
            });
          }
          else
            requestContext.Trace(80717, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
          for (int index = 0; index < identityChanges.MembershipChanges.Count; ++index)
            identityChanges.MembershipChanges[index].MemberId = list1[index];
          PlatformIdentityStore.DeduplicateMembershipChanges(identityChanges.MembershipChanges);
        }
      }
      return identityChanges;
    }

    internal bool UpdateIdentitiesInternal(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates)
    {
      requestContext.TraceEnter(80274, "IdentityService", "BusinessLogic", nameof (UpdateIdentitiesInternal));
      try
      {
        ArgumentUtility.CheckForNull<IList<Microsoft.VisualStudio.Services.Identity.Identity>>(identities, nameof (identities));
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
        {
          ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, "identity");
          string property = identity.GetProperty<string>("Alias", (string) null);
          if (!string.IsNullOrEmpty(property))
            IdentityValidation.CheckAlias(ref property);
          this.CheckPermissionByDescriptor(requestContext, identity.Descriptor, 2);
          this.CheckIdentityNameIsNotReserved(requestContext, identity);
        }
        ITeamFoundationEventService service1 = requestContext.GetService<ITeamFoundationEventService>();
        if (IdentityTranslationHelper.IsEnabled(requestContext))
        {
          requestContext.Trace(80718, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
          service1.PublishDecisionPoint(requestContext, (object) new BeforeUpdateIdentitiesOnService()
          {
            IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
            {
              Identities = identities
            }
          });
        }
        else
          requestContext.Trace(80719, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identities1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = identities;
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          PartitionResults<Microsoft.VisualStudio.Services.Identity.Identity> partitionResults = identities.Partition<Microsoft.VisualStudio.Services.Identity.Identity>((Predicate<Microsoft.VisualStudio.Services.Identity.Identity>) (x => x != null && x.IsCspPartnerUser));
          identities1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) partitionResults.MatchingPartition;
          identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) partitionResults.NonMatchingPartition;
        }
        bool flag = false;
        if (identities1 != null && identities1.Count > 0)
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          IdentityService service2 = vssRequestContext.GetService<IdentityService>();
          flag |= service2.UpdateIdentities(vssRequestContext, identities1);
        }
        if (identityList.Count > 0)
        {
          try
          {
            flag |= this.IdentityStore.UpdateIdentities(requestContext, this.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList, allowMetadataUpdates);
          }
          catch (IdentityAccountNameAlreadyInUseException ex)
          {
            if (identityList.Count == 1 && !identityList[0].IsExternalUser)
            {
              ((IPlatformIdentityServiceInternal) this).RepairAccountNameCollision(requestContext, identityList[0], "AccountNameReuse");
              flag |= this.IdentityStore.UpdateIdentities(requestContext, this.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList, allowMetadataUpdates);
            }
            else
              throw;
          }
        }
        if (IdentityTranslationHelper.IsEnabled(requestContext))
        {
          requestContext.Trace(80720, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation enabled. Attempting to translate identities.");
          service1.PublishDecisionPoint(requestContext, (object) new AfterUpdateIdentitiesOnService()
          {
            IdentityIdTranslationEventData = new IdentityIdTranslationEvent()
            {
              Identities = identities
            }
          });
        }
        else
          requestContext.Trace(80721, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identity translation disabled. Skipping identity translation.");
        GroupAvatarValidator.ValidateGroupAvatarsAsync(requestContext, identities);
        return flag;
      }
      finally
      {
        requestContext.TraceLeave(80274, "IdentityService", "BusinessLogic", nameof (UpdateIdentitiesInternal));
      }
    }

    private bool RemoveMemberFromGroupInternal(
      IVssRequestContext requestContext,
      ref IdentityDescriptor groupDescriptor,
      ref IdentityDescriptor memberDescriptor,
      bool forceRemove)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckDescriptor(memberDescriptor, nameof (memberDescriptor));
      this.CheckGroupPermission(requestContext, groupDescriptor, memberDescriptor, 8);
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.CheckGroupExistsInRequestScope(requestContext, groupDescriptor);
      IdentityScopeHelper.CheckGroupMembershipManagementIsAllowedInRequestScope(requestContext, identity.LocalScopeId, groupDescriptor.ToSubjectDescriptor(requestContext));
      groupDescriptor = this.Domain.MapFromWellKnownIdentifier(groupDescriptor);
      memberDescriptor = this.Domain.MapFromWellKnownIdentifier(memberDescriptor);
      if (!forceRemove)
        this.ValidateRemoveMemberFromGroup(requestContext, groupDescriptor, memberDescriptor);
      else
        requestContext.TraceConditionally(80761, TraceLevel.Info, "IdentityService", "BusinessLogic", (Func<string>) (() => "RemoveMemberFromGroup validation skipped due to forceRemove flag"));
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        service.PublishNotification(requestContext, (object) new BeforeGroupMembershipChangeSecurityPolicyEvent()
        {
          GroupDescriptor = groupDescriptor,
          MemberDescriptor = memberDescriptor,
          EventSource = GroupMembershipSecurityPolicyEventSource.RemoveMemberFromGroup
        });
      if (groupDescriptor.Identifier.EndsWith(PlatformIdentityService.s_everyoneGroupSuffix, StringComparison.OrdinalIgnoreCase))
        throw new ModifyEveryoneGroupException();
      IdentityDescriptor y1 = this.Domain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.ServiceUsersGroup);
      if (IdentityDescriptorComparer.Instance.Equals(groupDescriptor, y1))
      {
        IdentityDescriptor y2 = this.Domain.MapFromWellKnownIdentifier(requestContext.ServiceHost.SystemDescriptor());
        if (IdentityDescriptorComparer.Instance.Equals(memberDescriptor, y2))
          throw new RemoveMemberServiceAccountException();
      }
      return this.IdentityStore.RemoveMemberFromApplicationGroup(requestContext, this.Domain, false, new Tuple<IdentityDescriptor, IdentityDescriptor>(groupDescriptor, memberDescriptor));
    }

    private void FilterPropertiesInIdentityExtension(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IEnumerable<string> propertyNameFilters)
    {
      requestContext.TraceEnter(0, "IdentityService", "BusinessLogic", nameof (FilterPropertiesInIdentityExtension));
      if (identities == null || propertyNameFilters != null && propertyNameFilters.Contains<string>("*"))
        return;
      try
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
        {
          if (identity != null && !identity.IsContainer && identity.Properties != null)
          {
            IEnumerable<string> source = propertyNameFilters == null || !propertyNameFilters.Any<string>() ? identity.Properties.Keys.Where<string>((Func<string, bool>) (p => p != null && p != "http://schemas.microsoft.com/identity/claims/objectidentifier" && IdentityExtendedPropertyKeys.IdentityExtensionSupportedProperties.Contains(p))) : identity.Properties.Keys.Where<string>((Func<string, bool>) (p => p != null && p != "http://schemas.microsoft.com/identity/claims/objectidentifier" && IdentityExtendedPropertyKeys.IdentityExtensionSupportedProperties.Contains(p) && !propertyNameFilters.Contains<string>(p)));
            if (source != null && source.Any<string>())
            {
              foreach (string key in source.ToList<string>())
                identity.Properties.Remove(key);
            }
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (FilterPropertiesInIdentityExtension));
      }
    }

    void IPlatformIdentityServiceInternal.RepairAccountNameCollision(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string reason)
    {
      string property1 = identity.GetProperty<string>("Domain", string.Empty);
      string property2 = identity.GetProperty<string>("Account", string.Empty);
      string str1 = string.Format("{0}\\{1}", (object) property1, (object) property2);
      if (identity.Descriptor == (IdentityDescriptor) null || !"Microsoft.IdentityModel.Claims.ClaimsIdentity".Equals(identity.Descriptor.IdentityType, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.Trace(80250, TraceLevel.Info, "IdentityService", "BusinessLogic", "Identity '{0}' is not a claims identity so repairing the conflict could be unsafe.", (object) str1);
        throw new IdentityAccountNameCollisionRepairUnsafeException(str1);
      }
      string[] strArray = property2.Split('@');
      if (strArray.Length != 2)
      {
        requestContext.Trace(80251, TraceLevel.Info, "IdentityService", "BusinessLogic", "Identity account name '{0}' is not of the expected UPN format so repairing the conflict could be unsafe", (object) str1);
        throw new IdentityAccountNameCollisionRepairUnsafeException(str1);
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = this.ReadIdentities(requestContext, IdentitySearchFilter.AccountName, str1, QueryMembership.None, (IEnumerable<string>) null);
      if (source == null || source.Count > 1)
      {
        requestContext.Trace(80252, TraceLevel.Info, "IdentityService", "BusinessLogic", "Identity '{0}' has multiple conflicting rows and so repairing the conflict could be unsafe.", (object) str1);
        throw new IdentityAccountNameCollisionRepairUnsafeException(str1);
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity1 == null)
      {
        requestContext.Trace(80253, TraceLevel.Info, "IdentityService", "BusinessLogic", "Identity '{0}' collision might have been reparied in a different thread, retrying.", (object) str1);
      }
      else
      {
        string str2 = "NOCONFLICT.";
        if (identity1.Descriptor.Identifier.Contains(str2))
        {
          requestContext.Trace(80257, TraceLevel.Info, "IdentityService", "BusinessLogic", "Identity '{0}' already has an account name collision and could not be repaired automatically.", (object) str1);
          throw new IdentityAccountNameCollisionRepairFailedException(str1);
        }
        try
        {
          string str3 = str2 + reason + "." + (object) identity1.MasterId;
          string str4 = string.Format("{0}.{1}@{2}", (object) strArray[0], (object) str3, (object) strArray[1]);
          if (identity1.IsBindPending)
          {
            requestContext.TraceAlways(80254, TraceLevel.Info, "IdentityService", "BusinessLogic", "Repairing identity account name '{0}' collision by invalidating the existing bind pending identity. Updated account name: {1}", (object) str1, (object) str4);
            identity1.SetProperty("Account", (object) str4);
            identity1.Descriptor.Identifier = str3;
            this.IdentityStore.UpdateIdentities(requestContext, this.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
            {
              identity1
            }, false);
          }
          if (!"Microsoft.IdentityModel.Claims.ClaimsIdentity".Equals(identity1.Descriptor.IdentityType, StringComparison.OrdinalIgnoreCase))
            return;
          requestContext.TraceAlways(80255, TraceLevel.Info, "IdentityService", "BusinessLogic", "Repairing identity account name '{0}' collision by invalidating the existing claims identity. Updated account name: {1}", (object) str1, (object) str4);
          identity1.SetProperty("Account", (object) str4);
          this.IdentityStore.UpdateIdentities(requestContext, this.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            identity1
          }, false);
        }
        catch (Exception ex)
        {
          requestContext.Trace(80256, TraceLevel.Info, "IdentityService", "BusinessLogic", "Identity '{0}' has an account name collision that could not be repaired automatically.", (object) str1);
          throw new IdentityAccountNameCollisionRepairFailedException(str1, ex);
        }
      }
    }

    public override int UpgradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      return this.IdentityStore.UpgradeIdentitiesToTargetResourceVersion(requestContext, targetResourceVersion, updateBatchSize, maxNumberOfIdentitiesToUpdate);
    }

    public override int DowngradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      return this.IdentityStore.DowngradeIdentitiesToTargetResourceVersion(requestContext, targetResourceVersion, updateBatchSize, maxNumberOfIdentitiesToUpdate);
    }

    public int UpdateIdentityVsid(IVssRequestContext requestContext, Guid oldVsid, Guid newVsid) => this.IdentityStore.UpdateIdentityVsid(requestContext, oldVsid, newVsid);

    public int SendMajorDescriptorChangeNotification(IVssRequestContext requestContext) => this.IdentityStore.SendMajorDescriptorChangeNotification(requestContext);

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByDomainAndOid(
      IVssRequestContext requestContext,
      string domain,
      Guid externalId)
    {
      return this.IdentityStore.ReadIdentityByDomainAndOidWithLargestSequenceId(requestContext, domain, externalId);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByDomainAndOid(
      IVssRequestContext requestContext,
      string domain,
      Guid externalId)
    {
      return this.IdentityStore.ReadIdentitiesByDomainAndOid(requestContext.To(TeamFoundationHostType.Deployment), domain, externalId);
    }

    protected override bool RefreshIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      if (descriptor == (IdentityDescriptor) null)
        descriptor = requestContext.UserContext;
      if (!requestContext.IsSystemContext && !IdentityDescriptorComparer.Instance.Equals(descriptor, requestContext.UserContext))
        this.CheckPermission(requestContext, (string) null, 2, false);
      IdentityValidation.CheckDescriptor(descriptor, nameof (descriptor));
      descriptor = this.Domain.MapFromWellKnownIdentifier(descriptor);
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = this.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, QueryMembership.None, (IEnumerable<string>) null, false)[0];
      if (readIdentity == null || !readIdentity.IsActive)
        return false;
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentitiesFromSource(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, QueryMembership.None)[0];
      if (identity == null && string.Equals(descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase))
        this.IdentityStore.DeleteGroupMemberships(requestContext, this.Domain, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          descriptor
        }, true, true);
      else if (!identity.IsContainer)
        this.IdentityStore.UpdateIdentities(requestContext, this.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          identity
        }, false);
      else
        this.IdentityStore.UpdateSyncQueue(requestContext, this.Domain, (IList<Guid>) null, (IList<Tuple<Guid, bool, bool>>) new Tuple<Guid, bool, bool>[1]
        {
          new Tuple<Guid, bool, bool>(readIdentity.MasterId, false, true)
        });
      return true;
    }

    protected override int GetCurrentChangeId() => this.IdentityStore.GetCurrentChangeId();

    internal void EnsureSecurityNamespaces(IVssRequestContext requestContext)
    {
      this.CreateGroup(requestContext, Guid.Empty, GroupWellKnownSidConstants.SecurityServiceGroupSid, FrameworkResources.SecurityServiceGroupName(), FrameworkResources.SecurityServiceGroupDescription(), SpecialGroupType.Generic, false, false);
      this.CreateIdentitiesNamespace(requestContext);
      this.CreateIdentities2Namesapce(requestContext);
      IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
      securityContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(securityContext, FrameworkSecurity.IdentitiesNamespaceId).OnDataChanged(securityContext);
    }

    private void CreateIdentitiesNamespace(IVssRequestContext requestContext)
    {
      IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
      TeamFoundationSecurityService service = securityContext.Elevate().GetService<TeamFoundationSecurityService>();
      if (service.GetSecurityNamespace(securityContext.Elevate(), FrameworkSecurity.IdentitiesNamespaceId) != null)
        return;
      List<Microsoft.TeamFoundation.Framework.Server.ActionDefinition> actions = new List<Microsoft.TeamFoundation.Framework.Server.ActionDefinition>()
      {
        new Microsoft.TeamFoundation.Framework.Server.ActionDefinition(1, "Read", FrameworkResources.PermissionIdentityRead()),
        new Microsoft.TeamFoundation.Framework.Server.ActionDefinition(2, "Write", FrameworkResources.PermissionIdentityWrite()),
        new Microsoft.TeamFoundation.Framework.Server.ActionDefinition(4, "Delete", FrameworkResources.PermissionIdentityDelete()),
        new Microsoft.TeamFoundation.Framework.Server.ActionDefinition(8, "ManageMembership", FrameworkResources.PermissionIdentityManageMembership()),
        new Microsoft.TeamFoundation.Framework.Server.ActionDefinition(16, "CreateScope", FrameworkResources.PermissionIdentityCreateScope()),
        new Microsoft.TeamFoundation.Framework.Server.ActionDefinition(32, "RestoreScope", FrameworkResources.PermissionIdentityRestoreScope())
      };
      service.CreateSecurityNamespace(securityContext.Elevate(), new Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription(FrameworkSecurity.IdentitiesNamespaceId, "Identity", (string) null, "Default", FrameworkSecurity.IdentitySecurityPathSeparator, -1, SecurityNamespaceStructure.Hierarchical, 4, 1, actions, typeof (IdentitySecurityNamespaceExtension).FullName, true, false));
    }

    private void CreateIdentities2Namesapce(IVssRequestContext requestContext)
    {
      IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
      TeamFoundationSecurityService service = securityContext.Elevate().GetService<TeamFoundationSecurityService>();
      if (service.GetSecurityNamespace(securityContext.Elevate(), FrameworkSecurity.Identities2NamespaceId) != null)
        return;
      List<Microsoft.TeamFoundation.Framework.Server.ActionDefinition> actions = new List<Microsoft.TeamFoundation.Framework.Server.ActionDefinition>()
      {
        new Microsoft.TeamFoundation.Framework.Server.ActionDefinition(1, "Read", FrameworkResources.PermissionIdentitiesRead()),
        new Microsoft.TeamFoundation.Framework.Server.ActionDefinition(2, "Write", FrameworkResources.PermissionIdentitiesWrite()),
        new Microsoft.TeamFoundation.Framework.Server.ActionDefinition(4, "Delete", FrameworkResources.PermissionIdentitiesDelete()),
        new Microsoft.TeamFoundation.Framework.Server.ActionDefinition(8, "Impersonate", FrameworkResources.PermissionIdentitiesImpersonate())
      };
      service.CreateSecurityNamespace(securityContext.Elevate(), new Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription(FrameworkSecurity.Identities2NamespaceId, "Identity2", (string) null, "Default", FrameworkSecurity.IdentitySecurityPathSeparator, -1, SecurityNamespaceStructure.Hierarchical, 2, 1, actions, (string) null, false, false));
    }

    internal void SetDefaultPermissions(
      IVssRequestContext requestContext,
      IdentityScope identityScope)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.SkipAssigningDefaultAceForScopes") && requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseSecurityNamespaceExtensionToComputePermissions"))
      {
        requestContext.TraceConditionally(80258, TraceLevel.Verbose, "IdentityService", "BusinessLogic", (Func<string>) (() => string.Format("Skipped assigning default ACEs for scope id : {0}", (object) identityScope?.Id)));
      }
      else
      {
        if (identityScope == null)
          identityScope = new IdentityScope()
          {
            Id = this.Domain.DomainId,
            LocalScopeId = this.Domain.DomainId,
            ScopeType = GroupScopeType.ServiceHost
          };
        requestContext = requestContext.Elevate();
        IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
        IVssSecurityNamespace securityNamespace = securityContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(securityContext, FrameworkSecurity.IdentitiesNamespaceId);
        IdentityDescriptor descriptor1 = (IdentityDescriptor) null;
        IdentityDomain hostDomain;
        if ((identityScope.ScopeType == GroupScopeType.TeamProject || identityScope.ScopeType == GroupScopeType.Generic) && identityScope.ParentId != Guid.Empty)
        {
          hostDomain = new IdentityDomain(identityScope.ParentId);
          descriptor1 = IdentityMapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, identityScope.Id);
          if (identityScope.ScopeType == GroupScopeType.Generic)
          {
            IdentityScope scope = this.IdentityStore.GetScope(requestContext, hostDomain, identityScope.ParentId);
            if (scope != null && scope.ScopeType == GroupScopeType.TeamProject)
              hostDomain = new IdentityDomain(scope.ParentId);
          }
        }
        else
          hostDomain = new IdentityDomain(identityScope.Id);
        IdentityDescriptor descriptor2 = hostDomain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup);
        IdentityDescriptor descriptor3 = hostDomain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.ServiceUsersGroup);
        IdentityDescriptor descriptor4 = hostDomain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
        List<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry> accessControlEntryList1 = new List<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry>();
        accessControlEntryList1.Add(new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(descriptor2, 1, 0));
        accessControlEntryList1.Add(new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(descriptor3, 31, 0));
        accessControlEntryList1.Add(new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(descriptor4, 31, 0));
        if (descriptor1 != (IdentityDescriptor) null)
          accessControlEntryList1.Add(new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(descriptor1, 31, 0));
        string str = identityScope.LocalScopeId.ToString("D");
        IVssRequestContext requestContext1 = securityContext;
        string token = str;
        List<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry> accessControlEntryList2 = accessControlEntryList1;
        securityNamespace.SetAccessControlEntries(requestContext1, token, (IEnumerable<IAccessControlEntry>) accessControlEntryList2, false);
      }
    }

    private void CheckGroupPermission(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor,
      int requestedPermissions)
    {
      if (!IdentityValidation.IsTeamFoundationType(groupDescriptor))
        throw new NotApplicationGroupException();
      try
      {
        this.CheckPermissionByDescriptor(requestContext, groupDescriptor, requestedPermissions);
      }
      catch (AccessCheckException ex)
      {
        this.CheckPermissionByDescriptor(requestContext, groupDescriptor, requestedPermissions, true);
        requestContext.TraceAlways(80790, TraceLevel.Info, "IdentityService", "BusinessLogic", string.Format("Check group permission worked with alwaysAllowAdministrators flag set to true but failed with the flag set to false for user: {0}, group: {1}", (object) requestContext.UserContext, (object) groupDescriptor));
      }
      IdentityPermissionHelper.CheckUpdateMembershipPermissionsOnJITManagedOrganizations(requestContext, groupDescriptor.ToSubjectDescriptor(requestContext));
      IdentityPermissionHelper.CheckUpdateMembershipPermissionsOnEnterpriseServiceAccountsGroup(requestContext, groupDescriptor.ToSubjectDescriptor(requestContext), memberDescriptor.ToSubjectDescriptor(requestContext));
    }

    internal void CheckPermissionByDescriptor(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      int requestedPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext.IsSystemContext)
        return;
      if (IdentityValidation.IsTeamFoundationType(descriptor))
      {
        string securableToken = this.GetSecurableToken(requestContext, descriptor);
        this.CheckPermission(requestContext, securableToken, requestedPermissions, alwaysAllowAdministrators);
      }
      else
      {
        if (IdentityDescriptorComparer.Instance.Equals(requestContext.UserContext, descriptor))
          return;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<IPlatformIdentityServiceInternal>().CheckUserWritePermission(vssRequestContext);
      }
    }

    void IPlatformIdentityServiceInternal.CheckUserWritePermission(IVssRequestContext requestContext) => this.CheckPermission(requestContext, (string) null, 2, false);

    internal void CheckPermission(
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      if (requestContext.IsSystemContext || this.HasPermission(requestContext, token, requestedPermissions, alwaysAllowAdministrators))
        return;
      string token1 = token;
      if (string.IsNullOrEmpty(token1))
        token1 = this.Domain.DomainId.ToString("D");
      IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
      this.GetSecurityNamespace(securityContext).CheckPermission(securityContext, token1, requestedPermissions, alwaysAllowAdministrators);
    }

    internal void CheckIdentityNameIsNotReserved(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity groupIdentity)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(groupIdentity, nameof (groupIdentity));
      if (!IdentityValidation.IsTeamFoundationType(groupIdentity.Descriptor))
        return;
      this.CheckGroupNameIsNotReserved(requestContext, groupIdentity);
    }

    internal void CheckGroupNameIsNotReserved(
      IVssRequestContext requestContext,
      string groupName,
      bool isWellKnownGroup,
      bool isAadGroup)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(groupName, nameof (groupName));
      this.CheckGroupNameIsNotReserved(requestContext, (Microsoft.VisualStudio.Services.Identity.Identity) null, groupName, isWellKnownGroup, isAadGroup);
    }

    internal void CheckGroupNameIsNotReserved(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity groupIdentity = null,
      string groupName = null,
      bool isWellKnownGroup = false,
      bool isAadGroup = false)
    {
      if (groupIdentity == null && string.IsNullOrWhiteSpace(groupName))
        throw new ArgumentNullException("Either groupIdentity or groupName should have a value. Both are null.");
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UsersCannotCreateGroupsWithWellKnownNames"))
        return;
      if (groupIdentity != null)
      {
        string identifier = groupIdentity.Descriptor.Identifier;
        groupName = groupIdentity.GetProperty<string>("Account", string.Empty);
        string wellKnownSidPrefix = SidIdentityHelper.WellKnownSidPrefix;
        isWellKnownGroup = identifier.StartsWith(wellKnownSidPrefix, StringComparison.OrdinalIgnoreCase);
        SpecialGroupType property = groupIdentity.GetProperty<SpecialGroupType>("SpecialType", SpecialGroupType.Generic);
        isAadGroup = property == SpecialGroupType.AzureActiveDirectoryApplicationGroup || property == SpecialGroupType.AzureActiveDirectoryRole;
      }
      if (!(isWellKnownGroup | isAadGroup) && ReservedGroupWellKnownNames.WellKnownGroupNames.Contains<string>(groupName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
      {
        string str = FrameworkResources.GroupNameIsReservedBySystemError((object) groupName);
        requestContext.TraceAlways(80793, TraceLevel.Warning, "IdentityService", "BusinessLogic", str);
        throw new GroupNameIsReservedBySystemException(str);
      }
    }

    private void ValidateRemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.RemoveMemberFromGroupExtensions.Disable"))
        return;
      if (this.HasPermission(requestContext, (string) null, 64, false))
      {
        requestContext.Trace(80293, TraceLevel.Info, "IdentityService", "BusinessLogic", "ValidateRemoveMemberFromGroup skipped due to ForceDelete permissions");
      }
      else
      {
        IDisposableReadOnlyList<IRemoveMemberFromGroupExtension> extensions = requestContext.GetExtensions<IRemoveMemberFromGroupExtension>(ExtensionLifetime.Service);
        if (extensions == null)
          return;
        foreach (IRemoveMemberFromGroupExtension fromGroupExtension in extensions.Where<IRemoveMemberFromGroupExtension>((Func<IRemoveMemberFromGroupExtension, bool>) (x => x != null)))
          fromGroupExtension.Validate(requestContext, groupDescriptor, memberDescriptor);
      }
    }

    private bool HasPermissionOnUserIdentity(
      IVssRequestContext requestContext,
      string token,
      int requiredPermissions,
      bool alwaysAllowAdministrators,
      bool allChildren = false)
    {
      requestContext.TraceEnter(80028, "IdentityService", "BusinessLogic", nameof (HasPermissionOnUserIdentity));
      try
      {
        IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
        IVssSecurityNamespace securityNamespace = securityContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(securityContext, FrameworkSecurity.Identities2NamespaceId);
        if (securityNamespace != null)
          return !allChildren ? securityNamespace.HasPermission(securityContext, token, requiredPermissions, alwaysAllowAdministrators) : securityNamespace.HasPermissionForAllChildren(securityContext, token, requiredPermissions, alwaysAllowAdministrators: alwaysAllowAdministrators);
        requestContext.Trace(80292, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "HasPermissionOnUserIdentity, Identity2 NS not found");
        return false;
      }
      finally
      {
        requestContext.TraceLeave(80029, "IdentityService", "BusinessLogic", nameof (HasPermissionOnUserIdentity));
      }
    }

    private bool HasPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      int requiredPermissions,
      bool alwaysAllowAdministrators)
    {
      if (requestContext.IsSystemContext || IdentityDescriptorComparer.Instance.Equals(requestContext.UserContext, identity.Descriptor))
        return true;
      string token = (string) null;
      if (IdentityValidation.IsTeamFoundationType(identity.Descriptor) && identity.IsContainer && this.IsSecuringHost(requestContext, identity))
        token = identity.GetProperty<Guid>("LocalScopeId", Guid.Empty).ToString("D");
      return this.HasPermission(requestContext, token, requiredPermissions, alwaysAllowAdministrators);
    }

    private bool HasPermission(
      IVssRequestContext requestContext,
      string token,
      int requiredPermissions,
      bool alwaysAllowAdministrators,
      bool allChildren = false)
    {
      requestContext.TraceEnter(80026, "IdentityService", "BusinessLogic", nameof (HasPermission));
      try
      {
        string token1 = token;
        if (string.IsNullOrEmpty(token1))
          token1 = this.Domain.DomainId.ToString("D");
        IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
        IVssSecurityNamespace securityNamespace = this.GetSecurityNamespace(securityContext);
        return !allChildren ? securityNamespace.HasPermission(securityContext, token1, requiredPermissions, alwaysAllowAdministrators) : securityNamespace.HasPermissionForAllChildren(securityContext, token1, requiredPermissions, alwaysAllowAdministrators: alwaysAllowAdministrators);
      }
      finally
      {
        requestContext.TraceLeave(80027, "IdentityService", "BusinessLogic", nameof (HasPermission));
      }
    }

    private IVssSecurityNamespace GetSecurityNamespace(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId);

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> FilterReadResults(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results,
      bool compactResults,
      bool checkPermissions = true,
      bool forceFilterIdentities = false)
    {
      requestContext.TraceEnter(0, "IdentityService", "BusinessLogic", nameof (FilterReadResults));
      try
      {
        if (requestContext.IsSystemContext)
          return results;
        if (this.HasPermissionOnUserIdentity(requestContext, FrameworkSecurity.IdentitySecurityRootToken, 1, false))
        {
          requestContext.Trace(80291, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Identities not filtered, verified Read permission on Identities2 NS for user: {0}", (object) requestContext.UserContext);
          return results;
        }
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) && !forceFilterIdentities)
        {
          requestContext.TraceDataConditionally(80289, TraceLevel.Info, "IdentityService", "BusinessLogic", "Returning results without filtering since call is from service principal", (Func<object>) (() => (object) new
          {
            UserContext = requestContext.UserContext,
            ids = results.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid?>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid?>) (i => i?.Id))
          }), nameof (FilterReadResults));
          return results;
        }
        Dictionary<Guid, bool> dictionary = (Dictionary<Guid, bool>) null;
        int num = 0;
        if (checkPermissions && results.Count > 20)
        {
          dictionary = new Dictionary<Guid, bool>();
          foreach (Microsoft.VisualStudio.Services.Identity.Identity result in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) results)
          {
            if (result != null && IdentityValidation.IsTeamFoundationType(result.Descriptor))
              dictionary[result.GetProperty<Guid>("LocalScopeId", Guid.Empty)] = false;
          }
          foreach (Guid key in dictionary.Keys.ToArray<Guid>())
            dictionary[key] = this.HasPermission(requestContext, key.ToString("D"), 1, true, true);
        }
        for (int index = 0; index < results.Count; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity result = results[index];
          bool flag = false;
          if (dictionary != null && result != null)
            dictionary.TryGetValue(result.GetProperty<Guid>("LocalScopeId", Guid.Empty), out flag);
          if (result == null)
          {
            requestContext.Trace(80281, TraceLevel.Warning, "IdentityService", "BusinessLogic", "Got a null identity for user: {0}, removing it from results", (object) requestContext.UserContext);
            ++num;
          }
          else if (!string.IsNullOrEmpty(result.GetProperty<string>("RestrictedVisible", (string) null)))
          {
            requestContext.Trace(80282, TraceLevel.Warning, "IdentityService", "BusinessLogic", "IdentityAttributeTags.RestrictedVisible is not null or empty for user: {0}, identity descriptor: {1}", (object) requestContext.UserContext, (object) result.Descriptor);
            results[index] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
            ++num;
          }
          else if (checkPermissions && !flag && !this.HasPermission(requestContext, result, 1, true))
          {
            requestContext.Trace(80283, TraceLevel.Warning, "IdentityService", "BusinessLogic", "Request doesn't have identity read permission. user: {0}, identity descriptor: {1}", (object) requestContext.UserContext, (object) result.Descriptor);
            results[index] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
            ++num;
          }
          else if (result.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) && result.Descriptor.Identifier.StartsWith(SidIdentityHelper.PhantomSidPrefix, StringComparison.OrdinalIgnoreCase))
          {
            requestContext.Trace(80284, TraceLevel.Warning, "IdentityService", "BusinessLogic", "Identity descriptor doesn't have expected format. user: {0}, identity descriptor: {1}", (object) requestContext.UserContext, (object) result.Descriptor);
            results[index] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
            ++num;
          }
        }
        return num > 0 & compactResults ? (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (result => result != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>() : results;
      }
      finally
      {
        requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (FilterReadResults));
      }
    }

    private string GetSecurableToken(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = this.IdentityStore.ReadIdentities(requestContext, this.Domain, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        groupDescriptor
      }, QueryMembership.None, false, (IEnumerable<string>) null)[0];
      if (readIdentity == null)
        return (string) null;
      if (requestContext.IsSystemContext || readIdentity == null || this.IsSecuringHost(requestContext, readIdentity))
        return IdentityHelper.CreateSecurityToken((IReadOnlyVssIdentity) readIdentity);
      if (IdentityValidation.IsTeamFoundationType(groupDescriptor))
        throw new IdentityDomainMismatchException(this.Domain.Name, string.Empty);
      throw new NotApplicationGroupException();
    }

    private bool IsSecuringHost(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity group)
    {
      IVssRequestContext securityContext = this.GetSecurityContext(requestContext);
      return group.GetProperty<Guid>("SecuringHostId", Guid.Empty) == securityContext.ServiceHost.InstanceId;
    }

    private IVssRequestContext GetSecurityContext(IVssRequestContext requestContext) => !requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Application) ? requestContext : requestContext.To(TeamFoundationHostType.Application);

    int IPlatformIdentityServiceInternal.CleanupDescriptorChangeQueue(
      IVssRequestContext requestContext,
      int beforeDays)
    {
      return this.IdentityStore.CleanupDescriptorChangeQueue(requestContext, beforeDays);
    }

    IdentityAudit IAuditableIdentityService.GetIdentityAudit(
      IVssRequestContext requestContext,
      DateTime lastUpdatedOnOrBefore)
    {
      IdentityAudit identityAudit = new IdentityAudit()
      {
        IdentityAuditRecords = this.IdentityStore.GetIdentityAuditRecords(requestContext, lastUpdatedOnOrBefore)
      };
      List<Guid> list = identityAudit.IdentityAuditRecords.Select<IdentityAuditRecord, Guid>((Func<IdentityAuditRecord, Guid>) (x => x.IdentityId)).Distinct<Guid>().ToList<Guid>();
      identityAudit.UpdatedIdentities = !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.ReadIdentitiesWhenRetrievingAuditRecords") ? (IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>() : (IReadOnlyList<Microsoft.VisualStudio.Services.Identity.Identity>) this.IdentityStore.ReadIdentities(requestContext, this.Domain, (IList<Guid>) list, QueryMembership.None, false, (IEnumerable<string>) null);
      return identityAudit;
    }

    void IAuditableIdentityService.DeleteIdentityAuditRecords(
      IVssRequestContext requestContext,
      long sequenceId)
    {
      this.IdentityStore.DeleteIdentityAuditRecords(requestContext, sequenceId);
    }

    internal virtual bool IsScopeWithinRootScope(
      IVssRequestContext requestContext,
      Guid rootScopeId,
      Guid scopeIdToCheck)
    {
      if (scopeIdToCheck == rootScopeId)
        return true;
      IList<Guid> ancestorScopeIds = this.IdentityStore.GetAncestorScopeIds(requestContext, scopeIdToCheck);
      return ancestorScopeIds != null && ancestorScopeIds.Contains(rootScopeId);
    }

    internal virtual Microsoft.VisualStudio.Services.Identity.Identity CheckGroupExistsInRequestScope(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor)
    {
      return this.ReadIdentities(requestContext.Elevate(), (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
      {
        groupDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null, false).Single<Microsoft.VisualStudio.Services.Identity.Identity>() ?? throw new FindGroupSidDoesNotExistException(groupDescriptor.Identifier);
    }

    public override void ClearIdentityCache(IVssRequestContext requestContext)
    {
      requestContext.GetService<IdentityIdTranslationService>().ClearCaches(requestContext);
      this.IdentityStore.ClearCache(requestContext);
    }

    public override void InvalidateIdentities(
      IVssRequestContext requestContext,
      ICollection<Guid> identityIds)
    {
      requestContext.GetService<IdentityIdTranslationService>();
      this.IdentityStore.InvalidateIdentities(requestContext, identityIds);
    }

    void IPlatformIdentityServiceInternal.InvalidateMembershipsCache(
      IVssRequestContext requestContext,
      ICollection<MembershipChangeInfo> membershipChangeInfos)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) membershipChangeInfos, nameof (membershipChangeInfos));
      requestContext.CheckDeploymentRequestContext();
      requestContext.TraceConditionally(80778, TraceLevel.Info, "IdentityService", "BusinessLogic", (Func<string>) (() => "InvalidateMembershipsCache with membershipChangeInfos for Members IDs : " + string.Join(", ", membershipChangeInfos.Select<MembershipChangeInfo, string>((Func<MembershipChangeInfo, string>) (membership => membership?.MemberId.ToString() ?? "")))));
      this.IdentityStore.ProcessParentIdentityChangeOnAuthor(requestContext, this.Domain, -1, (ICollection<Guid>) null, DescriptorChangeType.None, (ICollection<Guid>) null, membershipChangeInfos);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal void InvalidateMembershipsCacheHelper(
      IVssRequestContext requestContext,
      ICollection<MembershipChangeInfo> membershipChangeInfos)
    {
      ((IPlatformIdentityServiceInternal) this).InvalidateMembershipsCache(requestContext, membershipChangeInfos);
    }

    public override void SweepIdentityCache(IVssRequestContext requestContext) => this.IdentityStore.SweepCache(requestContext);

    void IPlatformIdentityServiceInternal.CheckForLeakedMasterIds(
      IVssRequestContext requestContext,
      IEnumerable<Guid> ids)
    {
      if (ids.IsNullOrEmpty<Guid>() || !IdentityTranslationHelper.IsEnabled(requestContext) || !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.IdentityIdTranslator.TraceErrorIfIdIsMasterId"))
        return;
      requestContext.GetService<IdentityIdTranslationService>().CheckForLeakedMasterIds(requestContext, ids);
    }

    bool ISwapIdentityService.SwapIdentity(IVssRequestContext requestContext, Guid id1, Guid id2)
    {
      requestContext.TraceAlways(80779, TraceLevel.Info, "IdentityService", "BusinessLogic", string.Format("SwapIdentity {0} <-> {1}", (object) id1, (object) id2));
      this.CheckPermission(requestContext, (string) null, 2, true);
      return this.IdentityStore.SwapIdentity(requestContext, this.Domain, id1, id2);
    }

    private void TraceApplicationLevelDescriptorRead(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || !requestContext.IsTracing(80741, TraceLevel.Warning, "IdentityService", "BusinessLogic"))
        return;
      Guid instanceId = requestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId;
      string str = SidIdentityHelper.GetDomainSid(instanceId).ToString();
      foreach (IdentityDescriptor descriptor in (IEnumerable<IdentityDescriptor>) descriptors)
      {
        if ((!descriptor.IsUnauthenticatedIdentity() || !descriptor.Identifier.StartsWith(str) ? (!descriptor.IsServiceIdentityType() ? 0 : (descriptor.Identifier.StartsWith(instanceId.ToString()) ? 1 : 0)) : 1) != 0)
          requestContext.Trace(80741, TraceLevel.Warning, "IdentityService", "BusinessLogic", "Trying to read the framework identity with descriptor generated based on application host id. Descriptor: {0}", (object) descriptor);
      }
    }

    void IGraphMembershipChangeHandler.FireMembershipChange(
      IVssRequestContext requestContext,
      int sequenceId)
    {
      this.IdentityStore.ProcessIdentityChangeOnAuthor(requestContext, this.Domain, -1, sequenceId);
    }

    private static IList<Microsoft.VisualStudio.Services.Identity.Identity> FilterLocalGroupIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results)
    {
      string expectedIdentityType = "IsLocalGroup";
      List<Microsoft.VisualStudio.Services.Identity.Identity> filteredResults = results.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity =>
      {
        if (identity == null)
        {
          requestContext.TraceDataConditionally(602963213, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Filtering out null identity", (Func<object>) (() => (object) new
          {
            identity = identity
          }), nameof (FilterLocalGroupIdentities));
          return false;
        }
        if (!IdentityExtensions.IsLocalGroup(requestContext, identity))
        {
          requestContext.TraceDataConditionally(602963214, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Filtering out identity that doesn't match expected type", (Func<object>) (() => (object) new
          {
            identity = identity,
            expectedIdentityType = expectedIdentityType
          }), nameof (FilterLocalGroupIdentities));
          return false;
        }
        requestContext.TraceDataConditionally(602963215, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Including identity that matches expected type", (Func<object>) (() => (object) new
        {
          identity = identity,
          expectedIdentityType = expectedIdentityType
        }), nameof (FilterLocalGroupIdentities));
        return true;
      })).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      requestContext.TraceDataConditionally(602963216, TraceLevel.Verbose, "IdentityService", "BusinessLogic", "Filtered results to include only identities of expected type", (Func<object>) (() => (object) new
      {
        input = results,
        output = filteredResults,
        expectedIdentityType = expectedIdentityType
      }), nameof (FilterLocalGroupIdentities));
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) filteredResults;
    }

    private static IList<Microsoft.VisualStudio.Services.Identity.Identity> FilterCspIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> results)
    {
      if (results.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsCspPartnerUser)))
      {
        requestContext.TraceAlways(80319, TraceLevel.Warning, "IdentityService", "BusinessLogic", string.Format("Found CSP identities in ReadIdentities by searchFactor: {0}, factorValue: {1}. CSP identities will be filtered.", (object) searchFactor, (object) factorValue));
        results = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) results.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && !x.IsCspPartnerUser)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      return results;
    }

    private void CheckIdentityReadPermissions(IVssRequestContext requestContext, int permission)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess") || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GraphSecurityConstants.NamespaceId);
      if (securityNamespace == null)
        return;
      permission |= 1;
      securityNamespace.CheckPermission(requestContext, GraphSecurityConstants.SubjectsToken, permission);
    }

    private static bool CheckAndValidateAadGroupCreation(
      IVssRequestContext requestContext,
      Guid scopeId,
      IdentityDescriptor groupDescriptor,
      SpecialGroupType specialType)
    {
      bool flag1 = AadIdentityHelper.IsAadGroup(groupDescriptor);
      bool flag2 = specialType == SpecialGroupType.AzureActiveDirectoryApplicationGroup || specialType == SpecialGroupType.AzureActiveDirectoryRole;
      if (flag1 != flag2)
        throw new ArgumentException("Trying to create an AAD group, but the parameter: " + (flag1 ? nameof (specialType) : "groupSid") + " value: " + (flag1 ? specialType.ToString() : groupDescriptor.Identifier) + " is not correct for AAD group.");
      if (flag2)
      {
        requestContext.CheckOrganizationRequestContext();
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        if (scopeId != instanceId)
          throw new ArgumentException(string.Format("AAD groups can only be created within Organization scope. The input scope: {0} is not the Organization scope.", (object) scopeId));
      }
      return flag2;
    }

    private static void AddAggregateIdentityForAadGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      IDictionary<IdentityDescriptor, AadGroup> groups = vssRequestContext.GetService<AadService>().GetGroupsWithIds<IdentityDescriptor>(vssRequestContext, new GetGroupsWithIdsRequest<IdentityDescriptor>()
      {
        Identifiers = (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          groupDescriptor
        }
      }).Groups;
      if (!groups.Any<KeyValuePair<IdentityDescriptor, AadGroup>>())
        return;
      IList<AadGroup> aadGroupList = AadGroupToIdentityUtils.FilterAndTraceOrphanedGroups(requestContext, groups);
      if (!aadGroupList.Any<AadGroup>())
        return;
      AadGroupToIdentityUtils.AddAggregateIdentitiesForAadGroups(vssRequestContext, aadGroupList);
    }

    internal PlatformIdentityStore IdentityStore { get; set; }
  }
}
