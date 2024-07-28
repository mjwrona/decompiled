// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MemoryCacheIdentityIdentifierRepository
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class MemoryCacheIdentityIdentifierRepository : 
    VssCacheBase,
    IIdentityIdentifierRepository
  {
    private readonly VssMemoryCacheList<Guid, IdentityDescriptor> m_descriptorByMasterIdCache;
    private readonly int m_cacheSize;
    private MemoryCacheIdentityIdentifierRepository parentRepository;
    private Action m_unregisterNotifications;
    private const int c_cacheExpiryIntervalInHours = 8;
    private const int c_defaultCacheEvictionOperationIntervalInHours = 4;
    private static readonly TimeSpan s_traceCacheStatsInterval = TimeSpan.FromMinutes(5.0);
    protected const string TraceArea = "IdentityIdentifierConversion";
    protected const string TraceLayer = "MemoryCacheIdentityIdentifierRepository";

    public MemoryCacheIdentityIdentifierRepository(
      IVssRequestContext requestContext,
      TeamFoundationHostType hostType,
      MemoryCacheIdentityIdentifierRepository parentRepository = null)
    {
      MemoryCacheIdentityIdentifierRepository identifierRepository = this;
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      this.m_cacheSize = service1.GetValue<int>(requestContext, (RegistryQuery) "/Service/Identity/Settings/IdentityIdentifierHostCacheSize", true, 1024);
      this.HostType = hostType;
      this.parentRepository = parentRepository;
      RegistryQuery registryQuery;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        this.m_cacheSize = 10000;
      else if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRegistryService registryService = service1;
        IVssRequestContext requestContext1 = requestContext;
        registryQuery = (RegistryQuery) "/Service/Identity/Settings/IdentityIdentifierDeploymentCacheSize";
        ref RegistryQuery local = ref registryQuery;
        this.m_cacheSize = registryService.GetValue<int>(requestContext1, in local, 100000);
      }
      VssCacheExpiryProvider<Guid, IdentityDescriptor> expiryProvider = (VssCacheExpiryProvider<Guid, IdentityDescriptor>) null;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        ITeamFoundationTaskService service2 = requestContext.GetService<ITeamFoundationTaskService>();
        IVssRegistryService registryService1 = service1;
        IVssRequestContext requestContext2 = requestContext;
        registryQuery = (RegistryQuery) "/Service/Integration/Settings/IdentityIdentifierCacheEvictionEnabled";
        ref RegistryQuery local1 = ref registryQuery;
        if (registryService1.GetValue<bool>(requestContext2, in local1, false))
        {
          IVssRegistryService registryService2 = service1;
          IVssRequestContext requestContext3 = requestContext;
          registryQuery = (RegistryQuery) "/Service/Integration/Settings/IdentityIdentifierCacheInactivityIntervalInHours";
          ref RegistryQuery local2 = ref registryQuery;
          int num1 = Math.Max(1, registryService2.GetValue<int>(requestContext3, in local2, 8));
          expiryProvider = new VssCacheExpiryProvider<Guid, IdentityDescriptor>(Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry), Capture.Create<TimeSpan>(TimeSpan.FromHours((double) num1)));
          IVssRegistryService registryService3 = service1;
          IVssRequestContext requestContext4 = requestContext;
          registryQuery = (RegistryQuery) "/Service/Integration/Settings/IdentityIdentifierCacheEvictionOperationIntervalInHours";
          ref RegistryQuery local3 = ref registryQuery;
          int num2 = Math.Max(1, registryService3.GetValue<int>(requestContext4, in local3, 4));
          service2.AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.EvictExpiredCachedItems), (object) null, num2 * 60 * 60 * 1000));
        }
        service2.AddTask(requestContext.ServiceHost.InstanceId, new TeamFoundationTask(new TeamFoundationTaskCallback(this.TraceCacheStats), (object) null, (int) MemoryCacheIdentityIdentifierRepository.s_traceCacheStatsInterval.TotalMilliseconds));
        requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", DeploymentUserIdentityCacheService.DeploymentUserIdentityChanged, new SqlNotificationHandler(this.DeploymentIdentityService_DescriptorsChangedNotification), false);
      }
      this.m_descriptorByMasterIdCache = new VssMemoryCacheList<Guid, IdentityDescriptor>((IVssCachePerformanceProvider) this, this.m_cacheSize, expiryProvider);
      IIdentityServiceInternal identityService = requestContext.GetService<IdentityService>().IdentityServiceInternal();
      identityService.DescriptorsChangedNotification += new EventHandler<DescriptorChangeNotificationEventArgs>(this.IdentityService_DescriptorsChangedNotification);
      this.m_unregisterNotifications = (Action) (() => identityService.DescriptorsChangedNotification -= new EventHandler<DescriptorChangeNotificationEventArgs>(identifierRepository.IdentityService_DescriptorsChangedNotification));
    }

    public void Unload(IVssRequestContext requestContext)
    {
      Action unregisterNotifications = this.m_unregisterNotifications;
      if (unregisterNotifications != null)
        unregisterNotifications();
      this.m_unregisterNotifications = (Action) null;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      TeamFoundationTaskService service = requestContext.GetService<TeamFoundationTaskService>();
      service.RemoveTask(requestContext, new TeamFoundationTaskCallback(this.TraceCacheStats));
      service.RemoveTask(requestContext, new TeamFoundationTaskCallback(this.EvictExpiredCachedItems));
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", DeploymentUserIdentityCacheService.DeploymentUserIdentityChanged, new SqlNotificationHandler(this.DeploymentIdentityService_DescriptorsChangedNotification), true);
    }

    public IdentityDescriptor GetDescriptorByMasterId(
      IVssRequestContext requestContext,
      Guid masterId)
    {
      try
      {
        requestContext.TraceEnter(6307310, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), nameof (GetDescriptorByMasterId));
        IdentityDescriptor descriptorByMasterId = (IdentityDescriptor) null;
        this.m_descriptorByMasterIdCache.TryGetValue(masterId, out descriptorByMasterId);
        if (descriptorByMasterId == (IdentityDescriptor) null)
          requestContext.Trace(6307314, TraceLevel.Verbose, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), "Cache miss when reading identity descriptor by masterId - {0}", (object) masterId);
        else
          requestContext.Trace(6307315, TraceLevel.Verbose, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), "Cache hit when reading identity descriptor by masterId - {0}", (object) masterId);
        return descriptorByMasterId;
      }
      finally
      {
        requestContext.TraceLeave(6307311, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), nameof (GetDescriptorByMasterId));
      }
    }

    public IdentityDescriptor GetDescriptorByLocalId(
      IVssRequestContext requestContext,
      Guid localId)
    {
      throw new NotImplementedException();
    }

    public void OnDescriptorRetrievedByMasterId(
      IVssRequestContext requestContext,
      Guid masterId,
      IdentityDescriptor identityDescriptor)
    {
      this.CacheIdentityDescriptorByMasterId(requestContext, masterId, identityDescriptor);
    }

    public void OnDescriptorRetrievedByLocalId(
      IVssRequestContext requestContext,
      Guid localId,
      IdentityDescriptor identityDescriptor)
    {
      throw new NotImplementedException();
    }

    internal void CacheIdentityDescriptorByMasterId(
      IVssRequestContext requestContext,
      Guid masterId,
      IdentityDescriptor identityDescriptor)
    {
      if (masterId == new Guid())
        throw new ArgumentNullException(nameof (masterId));
      if (identityDescriptor == (IdentityDescriptor) null)
        throw new ArgumentNullException(nameof (identityDescriptor));
      try
      {
        requestContext.TraceEnter(6307312, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), nameof (CacheIdentityDescriptorByMasterId));
        this.m_descriptorByMasterIdCache[masterId] = identityDescriptor;
      }
      finally
      {
        requestContext.TraceLeave(6307313, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), nameof (CacheIdentityDescriptorByMasterId));
      }
    }

    private void IdentityService_DescriptorsChangedNotification(
      object sender,
      DescriptorChangeNotificationEventArgs e)
    {
      if (e != null)
      {
        if (e.RequestContext != null)
        {
          try
          {
            e.RequestContext.TraceEnter(6307326, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), nameof (IdentityService_DescriptorsChangedNotification));
            e.RequestContext.TraceSerializedConditionally(6307328, TraceLevel.Info, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), "Descriptor change notification {0}", (object) e);
            if (e.DescriptorChangeType == DescriptorChangeType.None)
              return;
            if (e.DescriptorChangeType == DescriptorChangeType.Major)
              this.m_descriptorByMasterIdCache.Clear();
            else if (!e.DescriptorChangeIds.IsNullOrEmpty<Guid>())
            {
              foreach (Guid descriptorChangeId in (IEnumerable<Guid>) e.DescriptorChangeIds)
                this.m_descriptorByMasterIdCache.Remove(descriptorChangeId);
            }
            this.parentRepository?.IdentityService_DescriptorsChangedNotification(sender, e);
            return;
          }
          catch (Exception ex)
          {
            e.RequestContext.TraceException(6307329, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), ex);
            return;
          }
          finally
          {
            e.RequestContext.TraceLeave(6307327, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), nameof (IdentityService_DescriptorsChangedNotification));
          }
        }
      }
      TeamFoundationTracingService.TraceRaw(6307325, TraceLevel.Error, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), "Bad descriptor change notification was sent.");
    }

    private void DeploymentIdentityService_DescriptorsChangedNotification(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      if (args == null)
        requestContext.Trace(6307335, TraceLevel.Error, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), "Receive empty descriptor change notification");
      try
      {
        DeploymentUserIdentityChange userIdentityChange = args.Deserialize<DeploymentUserIdentityChange>();
        requestContext.TraceSerializedConditionally(6307336, TraceLevel.Info, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), "Deployment identity invalidation {0}", (object) userIdentityChange);
        if (userIdentityChange == null)
          return;
        this.m_descriptorByMasterIdCache.Remove(userIdentityChange.StorageKey);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(6307337, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), ex);
      }
    }

    private void TraceCacheStats(IVssRequestContext requestContext, object taskArgs)
    {
      if (!(requestContext.IsVirtualServiceHost() ? requestContext.To(TeamFoundationHostType.Parent) : requestContext).IsFeatureEnabled("VisualStudio.Services.Identity.TraceIdentityIdentifierCacheCounts"))
        return;
      requestContext.Trace(6307316, TraceLevel.Info, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), this.m_descriptorByMasterIdCache.Count.ToString());
    }

    private void EvictExpiredCachedItems(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(6307317, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), nameof (EvictExpiredCachedItems));
      try
      {
        if ((requestContext.IsVirtualServiceHost() ? requestContext.To(TeamFoundationHostType.Parent) : requestContext).IsFeatureEnabled("VisualStudio.Services.Identity.EnableTTLForDeploymentIdentityIdentifierCache"))
          this.Sweep(requestContext);
        else
          requestContext.Trace(6307318, TraceLevel.Info, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), "Feature flag is disabled, skipping eviction of cache");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(6307320, TraceLevel.Error, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(6307317, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), nameof (EvictExpiredCachedItems));
      }
    }

    public void Sweep(IVssRequestContext requestContext)
    {
      if (requestContext.IsTracing(6307319, TraceLevel.Info, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository)))
      {
        int count1 = this.m_descriptorByMasterIdCache.Count;
        int num = this.m_descriptorByMasterIdCache.Sweep();
        int count2 = this.m_descriptorByMasterIdCache.Count;
        requestContext.Trace(6307319, TraceLevel.Info, "IdentityIdentifierConversion", nameof (MemoryCacheIdentityIdentifierRepository), "Sweep Cache called, with {0} items in the cache. {1} items sweeped and remaining number of cached items are {2}", (object) count1, (object) num, (object) count2);
      }
      else
        this.m_descriptorByMasterIdCache.Sweep();
    }

    public TeamFoundationHostType HostType { get; }
  }
}
