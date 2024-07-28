// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityServiceBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal abstract class IdentityServiceBase : 
    IdentityService,
    IUserIdentityService,
    ISystemIdentityService,
    IVssFrameworkService,
    IIdentityServiceInternalRestricted,
    IIdentityServiceInternal,
    IIdentityServiceInternalRequired,
    IDisposable
  {
    private IdentityDomain m_domain;
    protected Guid m_serviceHostId;
    private IDisposableReadOnlyList<IIdentityProviderExtension> m_extensionIdentityProviders;
    private IDictionary<string, IIdentityProvider> m_syncAgents;
    private Lazy<IList<IVssFrameworkService>> m_initList = new Lazy<IList<IVssFrameworkService>>((Func<IList<IVssFrameworkService>>) (() => (IList<IVssFrameworkService>) new List<IVssFrameworkService>()), LazyThreadSafetyMode.PublicationOnly);
    private ConcurrentDictionary<Guid, bool> m_recentlyAccessedUsers = new ConcurrentDictionary<Guid, bool>();
    private const int c_defaultUpdateLastUserAccessIntervalInMinutes = 5;
    private const int c_defaultIdentityEvictionOperationIntervalInHours = 4;
    private const string s_Area = "IdentityService";
    private const string s_Layer = "BusinessLogic";

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        this.Domain = new IdentityDomain(systemRequestContext);
        this.m_syncAgents = (IDictionary<string, IIdentityProvider>) new Dictionary<string, IIdentityProvider>();
        IList<IIdentityProvider> providers = (IList<IIdentityProvider>) new List<IIdentityProvider>()
        {
          (IIdentityProvider) new BindPendingProvider(),
          (IIdentityProvider) new ImportedIdentityProvider(),
          (IIdentityProvider) new UnauthenticatedProvider(),
          (IIdentityProvider) new ServiceIdentityProvider()
        };
        if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          providers.Add((IIdentityProvider) new AggregateIdentityProvider());
          providers.Add((IIdentityProvider) new ClaimsProvider());
          providers.Add((IIdentityProvider) new ServicePrincipalProvider());
          providers.Add((IIdentityProvider) new CspPartnerIdentityProvider());
        }
        else
        {
          providers.Add((IIdentityProvider) new WindowsProvider());
          providers.Add((IIdentityProvider) new ClaimsProvider());
        }
        this.ResolveSyncAgents(systemRequestContext, (IEnumerable<IIdentityProvider>) providers);
        this.m_extensionIdentityProviders = systemRequestContext.GetExtensions<IIdentityProviderExtension>();
        this.ResolveSyncAgents(systemRequestContext, (IEnumerable<IIdentityProvider>) this.m_extensionIdentityProviders);
        CachedRegistryService service1 = systemRequestContext.GetService<CachedRegistryService>();
        int num1 = Math.Max(1, service1.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Integration/Settings/UpdateLastUserAccessInterval", 5));
        TeamFoundationTaskService service2 = systemRequestContext.GetService<TeamFoundationTaskService>();
        service2.AddTask(systemRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.UpdateLastUserAccess), (object) null, num1 * 60 * 1000));
        if (!service1.GetValue<bool>(systemRequestContext, (RegistryQuery) "/Service/Integration/Settings/IdentityEvictionEnabled", false))
          return;
        int num2 = Math.Max(1, service1.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Integration/Settings/IdentityEvictionOperationIntervalInHours", 4));
        service2.AddTask(systemRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.EvictExpiredIdentities), (object) null, num2 * 60 * 60 * 1000));
      }
      else
      {
        IIdentityServiceInternal identityServiceInternal = systemRequestContext.To(TeamFoundationHostType.Parent).GetService<IdentityService>().IdentityServiceInternal();
        this.m_syncAgents = identityServiceInternal.SyncAgents;
        this.Domain = new IdentityDomain(systemRequestContext, identityServiceInternal.Domain);
        this.ScheduleSearchCacheWarmup(systemRequestContext);
      }
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
              requestContext.Trace(80151, TraceLevel.Warning, "IdentityService", "BusinessLogic", "Skipping provider {0} for identity type {1} because provider {2} is already set", (object) provider.GetType().FullName, (object) supportedIdentityType, (object) identityProvider.GetType().FullName);
            else if (provider is IIdentityProviderExtension)
            {
              requestContext.Trace(80151, TraceLevel.Warning, "IdentityService", "BusinessLogic", "Overriding provider {0} for identity type {1} because provider {2} is built-in.", (object) identityProvider.GetType().FullName, (object) supportedIdentityType, (object) provider.GetType().FullName);
              this.SyncAgents[supportedIdentityType] = provider;
            }
            else
            {
              string str = "Bad Configuration -- Multiple built in providers for the same identity type: " + supportedIdentityType + ", previousProvider: " + identityProvider.GetType().FullName + ", currentProvider: " + provider.GetType().FullName;
              requestContext.Trace(80151, TraceLevel.Error, "IdentityService", "BusinessLogic", str);
              throw new InvalidConfigurationException(str);
            }
          }
          else
          {
            requestContext.Trace(80152, TraceLevel.Info, "IdentityService", "BusinessLogic", "Setting provider {0} for identity type {1}", (object) provider.GetType().FullName, (object) supportedIdentityType);
            this.SyncAgents[supportedIdentityType] = provider;
          }
        }
      }
    }

    private void ScheduleSearchCacheWarmup(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.SearchCacheWarmup") || !IdentityStoreBase.IsSearchContext(systemRequestContext) || !IdentityStoreBase.IsMegaTenant(systemRequestContext))
        return;
      bool shouldPublishForEnterpriseDomain = systemRequestContext.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.SearchCacheWarmup.EnterpriseDomain.Enable");
      IdentitySearchHelper.PublishImsSearchCacheExpiryEvent(systemRequestContext, this.DomainId, shouldPublishForEnterpriseDomain);
    }

    public abstract void RefreshSearchIdentitiesCache(
      IVssRequestContext requestContext,
      Guid scopeId);

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      TeamFoundationTaskService service = systemRequestContext.GetService<TeamFoundationTaskService>();
      service.RemoveTask(systemRequestContext, new TeamFoundationTaskCallback(this.UpdateLastUserAccess));
      service.RemoveTask(systemRequestContext, new TeamFoundationTaskCallback(this.EvictExpiredIdentities));
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

    public Guid DomainId => this.Domain.DomainId;

    public IdentityMapper IdentityMapper => this.Domain.IdentityMapper;

    public abstract IdentityScope CreateScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid parentScopeId,
      GroupScopeType scopeType,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription,
      Guid creatorId);

    public abstract IdentityScope GetScope(IVssRequestContext requestContext, Guid scopeId);

    public abstract IdentityScope GetScope(IVssRequestContext requestContext, string scopeName);

    public abstract void DeleteScope(IVssRequestContext requestContext, Guid scopeId);

    public abstract void RestoreScope(IVssRequestContext requestContext, Guid scopeId);

    public abstract void RenameScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      string newName);

    protected abstract Guid GetScopeParentId(IVssRequestContext requestContext, Guid scopeId);

    Guid IIdentityServiceInternalRequired.GetScopeParentId(
      IVssRequestContext requestContext,
      Guid scopeId)
    {
      return this.GetScopeParentId(requestContext, scopeId);
    }

    protected abstract Microsoft.VisualStudio.Services.Identity.Identity CreateUser(
      IVssRequestContext requestContext,
      Guid scopeId,
      string userSid,
      string domainName,
      string accountName,
      string description);

    Microsoft.VisualStudio.Services.Identity.Identity IIdentityServiceInternal.CreateUser(
      IVssRequestContext requestContext,
      Guid scopeId,
      string userSid,
      string domainName,
      string accountName,
      string description)
    {
      return this.CreateUser(requestContext, scopeId, userSid, domainName, accountName, description);
    }

    public abstract Microsoft.VisualStudio.Services.Identity.Identity CreateFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      string mailAddress = null);

    public virtual Microsoft.VisualStudio.Services.Identity.Identity CreateGroup(
      IVssRequestContext requestContext,
      Guid scopeId,
      string groupSid,
      string groupName,
      string groupDescription,
      SpecialGroupType specialType = SpecialGroupType.Generic,
      bool scopeLocal = true,
      bool hasRestrictedVisibility = false)
    {
      Guid groupId = Guid.NewGuid();
      return this.CreateGroup(requestContext, scopeId, groupId, groupSid, groupName, groupDescription, specialType, scopeLocal, hasRestrictedVisibility);
    }

    public abstract Microsoft.VisualStudio.Services.Identity.Identity CreateGroup(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid groupId,
      string groupSid,
      string groupName,
      string groupDescription,
      SpecialGroupType specialType = SpecialGroupType.Generic,
      bool scopeLocal = true,
      bool hasRestrictedVisibility = false);

    public abstract void DeleteGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ListDeletedGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters);

    public abstract bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity member);

    public abstract bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor);

    public abstract bool RemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor);

    public abstract bool ForceRemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor);

    public abstract bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor);

    [Obsolete("This method should not be used. Use IVssRequestContext members or extension methods instead, such as GetUserId, UserContext, or GetUserIdentity.")]
    public virtual Microsoft.VisualStudio.Services.Identity.Identity ReadRequestIdentity(
      IVssRequestContext requestContext)
    {
      return requestContext.UserContext != (IdentityDescriptor) null ? requestContext.ReadIdentity(requestContext.UserContext) : (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public abstract string GetSignoutToken(IVssRequestContext requestContext);

    public virtual Microsoft.VisualStudio.Services.Identity.Identity ReadRequestIdentity(
      IVssRequestContext requestContext,
      IEnumerable<string> propertyNameFilters)
    {
      if (!(requestContext.UserContext != (IdentityDescriptor) null))
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      return this.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        requestContext.UserContext
      }, QueryMembership.None, propertyNameFilters, false).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      ReadIdentitiesOptions options);

    public virtual IdentitySearchResult SearchIdentities(
      IVssRequestContext requestContext,
      IdentitySearchParameters searchParameters)
    {
      return (IdentitySearchResult) null;
    }

    public abstract FilteredIdentitiesList ReadFilteredIdentities(
      IVssRequestContext requestContext,
      Guid scopeId,
      IList<IdentityDescriptor> descriptors,
      IEnumerable<IdentityFilter> filters,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters);

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IVssRequestContext requestContext,
      IList<string> accountNames,
      QueryMembership queryMembership)
    {
      IList<IdentityDescriptor> descriptors = (IList<IdentityDescriptor>) new List<IdentityDescriptor>();
      foreach (string accountName in (IEnumerable<string>) accountNames)
      {
        IdentityDescriptor descriptorByAccountName = this.GetDescriptorByAccountName(requestContext, accountName);
        if (descriptorByAccountName != (IdentityDescriptor) null)
          descriptors.Add(descriptorByAccountName);
      }
      return descriptors.Count == 0 ? (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>() : this.ReadIdentitiesFromSource(requestContext, descriptors, queryMembership);
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[descriptors.Count];
      for (int index = 0; index < descriptors.Count; ++index)
      {
        IdentityDescriptor descriptor = descriptors[index];
        if (descriptor != (IdentityDescriptor) null)
        {
          IIdentityProvider identityProvider;
          if (!this.SyncAgents.TryGetValue(descriptors[index].IdentityType, out identityProvider))
            throw new NotSupportedException(FrameworkResources.IdentityProviderNotFoundMessage((object) descriptor.IdentityType));
          SyncErrors syncErrors = new SyncErrors();
          syncErrors.Initialize(descriptor.Identifier);
          try
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (identityProvider.TrySyncIdentity(requestContext, descriptor, queryMembership != 0, (string) null, syncErrors, out identity))
              identityArray[index] = identity;
          }
          catch (Exception ex)
          {
            throw new IdentitySyncException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, {1} - {2}", (object) descriptor.IdentityType, (object) descriptor.Identifier, (object) ex.Message), ex);
          }
        }
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
    }

    protected abstract ChangedIdentities GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext);

    ChangedIdentities IIdentityServiceInternal.GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext)
    {
      return this.GetIdentityChanges(requestContext, sequenceContext);
    }

    protected abstract IdentityChanges GetIdentityChanges(
      IVssRequestContext requestContext,
      int sequenceId,
      long identitySequenceId,
      long groupSequenceId,
      long organizationIdentitySequenceId);

    IdentityChanges IIdentityServiceInternal.GetIdentityChanges(
      IVssRequestContext requestContext,
      int sequenceId,
      long identitySequenceId,
      long groupSequenceId,
      long organizationIdentitySequenceId)
    {
      return this.GetIdentityChanges(requestContext, sequenceId, identitySequenceId, groupSequenceId, organizationIdentitySequenceId);
    }

    protected abstract int GetCurrentSequenceId(IVssRequestContext requestContext);

    int IIdentityServiceInternalRequired.GetCurrentSequenceId(IVssRequestContext requestContext) => this.GetCurrentSequenceId(requestContext);

    public abstract bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities);

    public abstract bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates);

    protected abstract bool RefreshIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor);

    bool IIdentityServiceInternal.RefreshIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      return this.RefreshIdentity(requestContext, descriptor);
    }

    protected abstract int GetCurrentChangeId();

    int IIdentityServiceInternalRequired.GetCurrentChangeId() => this.GetCurrentChangeId();

    private IdentityDescriptor GetDescriptorByAccountName(
      IVssRequestContext requestContext,
      string accountName)
    {
      IdentityDescriptor descriptorByAccountName = (IdentityDescriptor) null;
      if (string.IsNullOrEmpty(accountName))
        return (IdentityDescriptor) null;
      string userName;
      int uniqueUserId;
      if (UserNameUtil.TryParseUniqueUserName(accountName, out userName, out uniqueUserId) && uniqueUserId != 0)
        return (IdentityDescriptor) null;
      foreach (IIdentityProvider identityProvider in (IEnumerable<IIdentityProvider>) this.SyncAgents.Values)
      {
        descriptorByAccountName = identityProvider.CreateDescriptor(requestContext, userName);
        if (descriptorByAccountName != (IdentityDescriptor) null)
          break;
      }
      return descriptorByAccountName;
    }

    private void UpdateLastUserAccess(IVssRequestContext requestContext, object taskArgs)
    {
      try
      {
        TeamFoundationTracingService.TraceEnterRaw(0, "IdentityService", "BusinessLogic", nameof (UpdateLastUserAccess), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
        if (this.m_recentlyAccessedUsers.Count == 0)
          return;
        ICollection<Guid> keys = Interlocked.Exchange<ConcurrentDictionary<Guid, bool>>(ref this.m_recentlyAccessedUsers, new ConcurrentDictionary<Guid, bool>()).Keys;
        if (keys.Count <= 0)
          return;
        List<Microsoft.VisualStudio.Services.Identity.Identity> list = this.ReadIdentities(requestContext, (IList<Guid>) keys.ToArray<Guid>(), QueryMembership.None, (IEnumerable<string>) null, false).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        foreach (IdentityBase identityBase in list)
          identityBase.SetProperty("LastAccessedTime", (object) DateTime.UtcNow);
        this.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, TraceLevel.Error, "IdentityService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(0, "IdentityService", "BusinessLogic", nameof (UpdateLastUserAccess));
      }
    }

    private void EvictExpiredIdentities(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(0, "IdentityService", "BusinessLogic", nameof (EvictExpiredIdentities));
      try
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache.EnableTTLForDeploymentCache"))
          this.SweepIdentityCache(requestContext);
        else
          requestContext.Trace(80139, TraceLevel.Info, "IdentityService", "BusinessLogic", "Feature flag is disabled, skipping eviction of identities");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, TraceLevel.Error, "IdentityService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "IdentityService", "BusinessLogic", nameof (EvictExpiredIdentities));
      }
    }

    bool IIdentityServiceInternal.LastUserAccessUpdateScheduled(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return this.m_recentlyAccessedUsers.ContainsKey(identityId);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IdentityService>().IdentityServiceInternal().LastUserAccessUpdateScheduled(vssRequestContext, identityId);
    }

    void IIdentityServiceInternal.ScheduleLastUserAccessUpdate(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<IdentityService>().IdentityServiceInternal().ScheduleLastUserAccessUpdate(vssRequestContext, identityId);
      }
      else
      {
        try
        {
          TeamFoundationTracingService.TraceEnterRaw(0, "IdentityService", "BusinessLogic", "ScheduleLastUserAccessUpdate", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
          string message = (string) null;
          this.m_recentlyAccessedUsers.AddOrUpdate(identityId, (Func<Guid, bool>) (id =>
          {
            message = string.Format("Identity ID '{0}' added to {1}", (object) id, (object) "m_recentlyAccessedUsers");
            return true;
          }), (Func<Guid, bool, bool>) ((id, currentValue) =>
          {
            message = string.Format("Identity ID '{0}' was already in {1}", (object) id, (object) "m_recentlyAccessedUsers");
            return currentValue;
          }));
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, "IdentityService", "BusinessLogic", message, (object) identityId);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, "IdentityService", "BusinessLogic", ex);
          throw;
        }
        finally
        {
          TeamFoundationTracingService.TraceLeaveRaw(0, "IdentityService", "BusinessLogic", "ScheduleLastUserAccessUpdate");
        }
      }
    }

    internal static void Trace(TraceLevel traceLevel, string format, params object[] args)
    {
      string[] authentication = TraceKeywordSets.Authentication;
      if (!TeamFoundationTrace.IsTracing(authentication, traceLevel))
        return;
      switch (traceLevel)
      {
        case TraceLevel.Off:
          break;
        case TraceLevel.Error:
          TeamFoundationTrace.Error(authentication, format, args);
          break;
        case TraceLevel.Warning:
          TeamFoundationTrace.Warning(authentication, format, args);
          break;
        case TraceLevel.Info:
          TeamFoundationTrace.Info(authentication, format, args);
          break;
        default:
          TeamFoundationTrace.Verbose(authentication, format, args);
          break;
      }
    }

    public virtual void SweepIdentityCache(IVssRequestContext requestContext)
    {
    }

    public abstract void ClearIdentityCache(IVssRequestContext requestContext);

    public abstract void InvalidateIdentities(
      IVssRequestContext requestContext,
      ICollection<Guid> identityIds);

    protected void FireDescriptorsChanged(object sender, DescriptorChangeEventArgs e)
    {
      EventHandler<DescriptorChangeEventArgs> descriptorsChanged = this.DescriptorsChanged;
      if (descriptorsChanged == null)
        return;
      if (this.m_serviceHostId != e.RequestContext.ServiceHost.InstanceId)
      {
        IVssRequestContext vssRequestContext = e.RequestContext.To(TeamFoundationHostType.Deployment);
        using (IVssRequestContext requestContext = vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, this.m_serviceHostId, RequestContextType.SystemContext, false, false))
        {
          if (requestContext == null)
            return;
          DescriptorChangeEventArgs e1 = new DescriptorChangeEventArgs(requestContext);
          descriptorsChanged((object) this, e1);
        }
      }
      else
        descriptorsChanged((object) this, e);
    }

    protected void FireDescriptorsChangedNotification(
      object sender,
      DescriptorChangeNotificationEventArgs e)
    {
      EventHandler<DescriptorChangeNotificationEventArgs> changedNotification = this.DescriptorsChangedNotification;
      if (changedNotification == null)
        return;
      if (this.m_serviceHostId != e.RequestContext.ServiceHost.InstanceId)
      {
        IVssRequestContext vssRequestContext = e.RequestContext.To(TeamFoundationHostType.Deployment);
        using (IVssRequestContext requestContext = vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, this.m_serviceHostId, RequestContextType.SystemContext, false, false))
        {
          if (requestContext == null)
            return;
          DescriptorChangeNotificationEventArgs e1 = new DescriptorChangeNotificationEventArgs(requestContext, e.DescriptorChangeType, e.DescriptorChangeIds);
          changedNotification((object) this, e1);
        }
      }
      else
        changedNotification((object) this, e);
    }

    public virtual int UpgradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      throw new NotImplementedException();
    }

    public virtual int DowngradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      throw new NotImplementedException();
    }

    public virtual IDictionary<string, IIdentityProvider> SyncAgents => this.m_syncAgents;

    IdentityDomain IIdentityServiceInternal.Domain => this.m_domain;

    public IdentityDomain Domain
    {
      protected get => this.m_domain;
      set => this.m_domain = value;
    }

    protected event EventHandler<DescriptorChangeEventArgs> DescriptorsChanged;

    protected event EventHandler<DescriptorChangeNotificationEventArgs> DescriptorsChangedNotification;

    event EventHandler<DescriptorChangeEventArgs> IIdentityServiceInternal.DescriptorsChanged
    {
      add => this.DescriptorsChanged += value;
      remove => this.DescriptorsChanged -= value;
    }

    event EventHandler<DescriptorChangeNotificationEventArgs> IIdentityServiceInternal.DescriptorsChangedNotification
    {
      add => this.DescriptorsChangedNotification += value;
      remove => this.DescriptorsChangedNotification -= value;
    }
  }
}
