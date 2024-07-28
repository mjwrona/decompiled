// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal static class ImsCacheConstants
  {
    internal const string BypassCacheToken = "ImsCacheConstants.Token.BypassCache";

    internal static class Tracing
    {
      internal const string Area = "Microsoft.VisualStudio.Services.Identity";
      internal const string TracePointHitsKey = "Microsoft.VisualStudio.Services.Identity.Cache.TracePointHits";
      internal const int MaxSameRequestTraces = 4;

      internal static class Layer
      {
        internal const string CacheService = "ImsCacheService";
        internal const string LocalDataCache = "ImsLocalDataCache";
        internal const string LocalSearchCache = "ImsLocalSearchCache";
        internal const string RemoteCache = "ImsRemoteCache";
      }
    }

    internal static class FeatureFlags
    {
      internal const string ImsCacheFeatureFlag = "Microsoft.VisualStudio.Services.Identity.Cache2";
      internal const string ImsSearchCacheWarmupFeatureFlag = "Microsoft.VisualStudio.Services.Identity.Cache2.SearchCacheWarmup";
      internal const string ImsLocalDataCacheFeatureFlag = "Microsoft.VisualStudio.Services.Identity.Cache2.LocalCache";
      internal const string ImsLocalSearchCacheFeatureFlag = "Microsoft.VisualStudio.Services.Identity.Cache2.LocalSearchCache";
      internal const string ImsRemoteCacheFeatureFlag = "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache";
      internal const string ImsRemoteCacheCacheNullResultsAccountNameFeatureFlag = "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.AccountName.CacheNullResults";
      internal const string ImsRemoteCacheDontCacheNullResultsDisplayNameFeatureFlag = "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.DisplayName.DontCacheNullResults";
      internal const string ImsRemoteCacheDontCacheIdentitiesByDescriptor = "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.IdentitiesByDescriptor.Disable";
      internal const string ImsRemoteCacheDontCacheIdentitiesById = "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.IdentitiesById.Disable";
      internal const string ImsRemoteCacheCacheGroups = "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.CacheGroups";
      internal const string ImsRemoteCacheCacheIdentitiesByScope = "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.IdentitiesByScope";
      internal const string ImsLocalDataCacheDontCacheIdentitiesByScope = "Microsoft.VisualStudio.Services.Identity.Cache2.LocalDataCache.IdentitiesByScope.Disable";
      internal const string ImsCacheServiceDontCacheIdentitiesByScope = "Microsoft.VisualStudio.Services.Identity.Cache2.ImsCacheService.IdentitiesByScope.Disable";
      internal const string ImsRemoteCacheDontCacheShardedIdentities = "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.CacheShardedIdentity.Disable";
      internal const string ImsRemoteCacheScopeMembershipAtOrgLevelFeatureFlag = "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.ScopeMembershipAtOrgLevel";
      internal const string ImsSearchCacheWarmupFeatureFlagAtEnterpriseDomain = "Microsoft.VisualStudio.Services.Identity.Cache2.SearchCacheWarmup.EnterpriseDomain.Enable";
      internal const string ImsRemoteCacheStorageIsAzureBlobStore = "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.AzureBlobStoreIsStorage";
    }

    internal static class Registry
    {
      internal static class CacheService
      {
        internal static class Keys
        {
          internal const string Root = "/Configuration/Identity/Cache/ImsCacheService";
          internal const string RootFilter = "/Configuration/Identity/Cache/ImsCacheService/...";
          internal const string ChildrenTimeToLiveKey = "/Configuration/Identity/Cache/ImsCacheService/ChildrenTimeToLive";
          internal const string DescendantsTimeToLiveKey = "/Configuration/Identity/Cache/ImsCacheService/DescendantsTimeToLive";
          internal const string IdentitiesInScopeTimeToLiveKey = "/Configuration/Identity/Cache/ImsCacheService/IdentitiesInScopeTimeToLive";
          internal const string IdentityIdTimeToLiveKey = "/Configuration/Identity/Cache/ImsCacheService/IdentityIdTimeToLive";
          internal const string IdentityTimeToLiveKey = "/Configuration/Identity/Cache/ImsCacheService/IdentityTimeToLive";
          internal const string ScopeMembershipTimeToLiveKey = "/Configuration/Identity/Cache/ImsCacheService/ScopeMembershipTimeToLive";
          internal const string DisplayNameQueryResultsTimeToLiveKey = "/Configuration/Identity/Cache/ImsCacheService/DisplayNameQueryResultsTimeToLive";
          internal const string AccountNameQueryResultsTimeToLiveKey = "/Configuration/Identity/Cache/ImsCacheService/AccountNameQueryResultsTimeToLive";
          internal const string MruIdentityIdsTimeToLiveKey = "/Configuration/Identity/Cache/ImsCacheService/MruIdentityIdsTimeToLive";
          internal const string FreshEntryTimeSpan = "/Configuration/Identity/Cache/ImsCacheService/FreshEntryTimeSpan";
          internal const string MaxDescendantLevelsToIterateKey = "/Configuration/Identity/Cache/ImsCacheService/MaxDescendantLevelsToIterate";
          internal const string IdentitesInScopeCountThresholdBeyondWhichCacheKey = "/Configuration/Identity/Cache/ImsCacheService/IdentitesInScopeCountThresholdBeyondWhichCache";
          internal const string WarningDescendantsCountThresholdKey = "/Configuration/Identity/Cache/ImsCacheService/WarningDescendantsCountThreshold";
          internal const string WarningDescendantsLevelsThresholdKey = "/Configuration/Identity/Cache/ImsCacheService/WarningDescendantsLevelsThreshold";
        }

        internal static class Defaults
        {
          internal static readonly TimeSpan DefaultIdentityTimeToLive = TimeSpan.FromHours(12.0);
          internal static readonly TimeSpan DefaultChildrenTimeToLive = TimeSpan.FromHours(1.0);
          internal static readonly TimeSpan DefaultDescendantsTimeToLive = TimeSpan.FromHours(4.0);
          internal static readonly TimeSpan DefaultIdentitiesInScopeTimeToLive = TimeSpan.FromHours(4.0);
          internal static readonly TimeSpan DefaultIdentityIdTimeToLive = TimeSpan.FromHours(12.0);
          internal static readonly TimeSpan DefaultScopeMembershipTimeToLive = TimeSpan.FromHours(4.0);
          internal static readonly TimeSpan DefaultDisplayNameQueryResultsTimeToLive = TimeSpan.FromMinutes(20.0);
          internal static readonly TimeSpan DefaultAccountNameQueryResultsTimeToLive = TimeSpan.FromHours(4.0);
          internal static readonly TimeSpan DefaultMruIdentityIdsTimeToLive = TimeSpan.FromHours(4.0);
          internal static readonly TimeSpan FreshEntryTimeSpan = TimeSpan.FromMinutes(15.0);
          internal const int DefaultMaxDescendantLevelsToIterate = 100;
          internal const int DefaultIdentitesInScopeCountThresholdBeyondWhichCache = 10000;
          internal const int DefaultWarningDescendantsCountThreshold = 10000;
          internal const int DefaultWarningDescendantsLevelsThreshold = 7;
        }
      }

      internal static class LocalOrRemoteCache
      {
        internal static class Keys
        {
          internal const string Root = "/Configuration/Identity/Cache/Settings";
          internal const string RootFilter = "/Configuration/Identity/Cache/Settings/...";
          internal const string LocalCacheCleanUpInterval = "/Configuration/Identity/Cache/Settings/CleanUpInterval";
          internal const string SearchCacheTinyTimeToLive = "/Configuration/Identity/Cache/Settings/SearchCacheTinyTimeToLive";
          internal const string SearchCacheSmallTimeToLive = "/Configuration/Identity/Cache/Settings/SearchCacheSmallTimeToLive";
          internal const string SearchCacheMediumTimeToLive = "/Configuration/Identity/Cache/Settings/SearchCacheMediumTimeToLive";
          internal const string SearchCacheLargeTimeToLive = "/Configuration/Identity/Cache/Settings/SearchCacheLargeTimeToLive";
          internal const string SearchCacheMegaTimeToLive = "/Configuration/Identity/Cache/Settings/SearchCacheMegaTimeToLive";
          internal const string SearchCacheSoonToExpireAlertDuration = "/Configuration/Identity/Cache/Settings/SearchCacheSoonToExpireAlertDuration";
          internal const string SearchCacheTinySizeThreshold = "/Configuration/Identity/Cache/Settings/SearchCacheTinySizeThreshold";
          internal const string SearchCacheSmallSizeThreshold = "/Configuration/Identity/Cache/Settings/SearchCacheSmallSizeThreshold";
          internal const string SearchCacheMediumSizeThreshold = "/Configuration/Identity/Cache/Settings/SearchCacheMediumSizeThreshold";
          internal const string SearchCacheLargeSizeThreshold = "/Configuration/Identity/Cache/Settings/SearchCacheLargeSizeThreshold";
          internal const string RemoteNamespaceFormat = "/Configuration/Identity/Cache/Settings/CacheItems/{0}/RemoteNamespace";
          internal const string RemoteCacheEntryTimeToLiveFormat = "/Configuration/Identity/Cache/Settings/CacheItems/{0}/RemoteCacheEntryTTL";
          internal const string RemoteCacheEntryUseCompressionSetting = "/Configuration/Identity/Cache/Settings/CacheItems/{0}/RemoteCacheUseCompression";
          internal const string LocalCacheEntryTimeToLiveFormat = "/Configuration/Identity/Cache/Settings/CacheItems/{0}/LocalCacheEntryTTL";
          internal static string DefaultRemoteCacheEntryTimeToLiveFormat = string.Format("/Configuration/Identity/Cache/Settings/CacheItems/{0}/RemoteCacheEntryTTL", (object) "Default");
          internal static string DefaultLocalCacheEntryTimeToLiveFormat = string.Format("/Configuration/Identity/Cache/Settings/CacheItems/{0}/LocalCacheEntryTTL", (object) "Default");
        }

        internal static class Defaults
        {
          internal static readonly TimeSpan DefaultRemoteCacheEntryTimeToLive = TimeSpan.FromHours(12.0);
          internal static readonly TimeSpan DefaultLocalCacheEntryTimeToLive = TimeSpan.FromHours(12.0);
          internal static readonly TimeSpan DefaultLocalCacheCleanUpInterval = TimeSpan.FromMinutes(15.0);
          internal static readonly TimeSpan DefaultSearchCacheTinyTimeToLive = TimeSpan.FromMinutes(5.0);
          internal static readonly TimeSpan DefaultSearchCacheSmallTimeToLive = TimeSpan.FromMinutes(15.0);
          internal static readonly TimeSpan DefaultSearchCacheMediumTimeToLive = TimeSpan.FromHours(1.0);
          internal static readonly TimeSpan DefaultSearchCacheLargeTimeToLive = TimeSpan.FromHours(4.0);
          internal static readonly TimeSpan DefaultSearchCacheMegaTimeToLive = TimeSpan.FromDays(7.0);
          internal static readonly TimeSpan DefaultSearchCacheSoonToExpireAlertDuration = TimeSpan.FromMinutes(15.0);
          internal const int DefaultSearchCacheSizeTinyThreshold = 100;
          internal const int DefaultSearchCacheSizeSmallThreshold = 200;
          internal const int DefaultSearchCacheSizeMediumThreshold = 2000;
          internal const int DefaultSearchCacheSizeLargeThreshold = 5000;
        }
      }
    }
  }
}
