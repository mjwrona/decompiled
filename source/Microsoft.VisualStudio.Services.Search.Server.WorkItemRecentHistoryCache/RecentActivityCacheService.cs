// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.RecentActivityCacheService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D4A0500-806F-44D4-BA97-D409A2311716
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Redis;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache
{
  public class RecentActivityCacheService : IRecentActivityCacheService, IVssFrameworkService
  {
    private IMutableDictionaryCacheContainer<CacheKey, RecentActivityDetails> m_storageKeyToRecentActivityDetailsRemoteCache;
    private TimeSpan m_cacheEntryTimeout;
    private int m_maxActivitiesPerUserPerProject;
    protected static readonly Guid s_namespace = new Guid("9BE79A4D-0DCF-4691-809C-F62A1C1F3834");
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1081325, "Data Provider", "WorkItemRecentActivityCacheService");

    public RecentActivityCacheService()
    {
    }

    internal RecentActivityCacheService(
      IMutableDictionaryCacheContainer<CacheKey, RecentActivityDetails> cacheContainer)
    {
      this.m_storageKeyToRecentActivityDetailsRemoteCache = cacheContainer;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      systemRequestContext.CheckProjectCollectionRequestContext();
      IVssRegistryService service1 = systemRequestContext.GetService<IVssRegistryService>();
      this.m_cacheEntryTimeout = service1.GetValue<TimeSpan>(systemRequestContext, (RegistryQuery) "/Service/ALMSearch/Settings/CacheExpiry", ConfigDefaults.RecentActivityCacheEntryTimeout);
      this.m_maxActivitiesPerUserPerProject = service1.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/ALMSearch/Settings/WorkItemMaxRecentActivitiesPerProjectPerUserInCache", 50);
      IRedisCacheService service2 = systemRequestContext.GetService<IRedisCacheService>();
      if (this.m_storageKeyToRecentActivityDetailsRemoteCache == null)
        this.m_storageKeyToRecentActivityDetailsRemoteCache = service2.GetVolatileDictionaryContainer<CacheKey, RecentActivityDetails, RecentActivityCacheService.SecurityToken>(systemRequestContext, RecentActivityCacheService.s_namespace, new ContainerSettings()
        {
          KeyExpiry = new TimeSpan?(this.m_cacheEntryTimeout),
          CiAreaName = nameof (RecentActivityCacheService),
          NoThrowMode = new bool?(true)
        });
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("ServiceStartTime", "WorkItemRecentActivityCacheService", (double) stopwatch.ElapsedMilliseconds);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual RecentActivityDetails GetRecentActivitiesFromCache(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid userId,
      bool isSearchRequest)
    {
      CacheKey storageKey = new CacheKey(userId, projectId);
      RecentActivityDetails activitiesFromCache = this.Get(requestContext, storageKey);
      if (activitiesFromCache != null & isSearchRequest)
        activitiesFromCache.LastSearchedTime = DateTime.UtcNow;
      return activitiesFromCache;
    }

    public virtual void UpdateRecentActivitiesInCache(
      IVssRequestContext requestContext,
      Guid userId,
      Guid projectId,
      DateTime activityDate,
      int artifactId,
      int areaId)
    {
      CacheKey storageKey = new CacheKey(userId, projectId);
      RecentActivityDetails recentActivityDetails = this.Get(requestContext, storageKey);
      if (recentActivityDetails != null)
      {
        ItemDetails itemDetails;
        if (recentActivityDetails.Details.TryGetValue(artifactId, out itemDetails))
        {
          itemDetails.ActivityDate = activityDate;
        }
        else
        {
          if (recentActivityDetails.Details.Count == this.m_maxActivitiesPerUserPerProject)
            recentActivityDetails.Evict();
          recentActivityDetails.Details.Add(artifactId, new ItemDetails(areaId, activityDate));
        }
        this.Set(requestContext, storageKey, recentActivityDetails);
      }
      else
        this.Set(requestContext, storageKey, new RecentActivityDetails()
        {
          Details = {
            {
              artifactId,
              new ItemDetails(areaId, activityDate)
            }
          }
        });
    }

    private RecentActivityDetails Get(IVssRequestContext requestContext, CacheKey storageKey)
    {
      if (storageKey.Equals(new CacheKey()))
        return (RecentActivityDetails) null;
      RecentActivityDetails recentActivityDetails = (RecentActivityDetails) null;
      try
      {
        this.m_storageKeyToRecentActivityDetailsRemoteCache.TryGet<CacheKey, RecentActivityDetails>(requestContext, storageKey, out recentActivityDetails);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(RecentActivityCacheService.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Unable to fetch data from cache. Exception:{0} StackTrace {1}", (object) ex.Message, (object) ex.StackTrace)));
      }
      return recentActivityDetails;
    }

    private void Set(
      IVssRequestContext requestContext,
      CacheKey storageKey,
      RecentActivityDetails recentActivityDetails)
    {
      this.m_storageKeyToRecentActivityDetailsRemoteCache.Set(requestContext, (IDictionary<CacheKey, RecentActivityDetails>) new Dictionary<CacheKey, RecentActivityDetails>()
      {
        {
          storageKey,
          recentActivityDetails
        }
      });
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method to be used in future.")]
    private void Remove(IVssRequestContext requestContext, CacheKey storageKey) => this.m_storageKeyToRecentActivityDetailsRemoteCache.Invalidate<CacheKey, RecentActivityDetails>(requestContext, storageKey);

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    private sealed class SecurityToken
    {
    }
  }
}
