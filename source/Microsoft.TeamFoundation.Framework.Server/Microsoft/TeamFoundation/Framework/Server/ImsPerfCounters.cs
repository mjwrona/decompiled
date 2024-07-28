// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ImsPerfCounters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ImsPerfCounters
  {
    private const string UriBase = "Microsoft.VisualStudio.Services.Identity.Perf";
    internal const string ImsInvalidations = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_invalidations";
    internal const string ImsMembershipInvalidations = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_membership_invalidations";
    internal const string ImsEvictions = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_evictions";
    internal const string ImsCacheHits = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_cache_hits";
    internal const string ImsCacheMisses = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_cache_misses";
    internal const string ImsCacheMissesPerSec = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_cache_misses_persec";
    internal const string ImsInvalidationsPerSec = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_invalidations_persec";
    internal const string ImsMembershipInvalidationsPerSec = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_membership_invalidations_persec";
    internal const string ImsExtPropCacheHits = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_extprop_cache_hits";
    internal const string ImsExtPropCacheMisses = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_extprop_cache_misses";
    internal const string ImsExtPropCacheMissesPerSec = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_extprop_cache_misses_persec";
    internal const string ImsNewClientTokens = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_new_client_tokens";
    internal const string ImsNewClientTokensPerSec = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_new_client_tokens_persec";
    internal const string ImsRestampedClientTokens = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_restamped_client_tokens";
    internal const string ImsRestampedClientTokensPerSec = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_restamped_client_tokens_persec";
    internal const string ImsReissuedTokensPerSec = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_reissued_tokens_persec";
    internal const string ImsNewSpsDeploymentTokensPerSec = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_new_sps_deployment_tokens_persec";
    internal const string ImsRestampedSpsDeploymentTokensPerSec = "Microsoft.VisualStudio.Services.Identity.Perf_Ims_restamped_sps_deployment_tokens_persec";
    internal const string LocalCacheGetObjectsObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.GetObjects.Objects";
    internal const string LocalCacheGetObjectsRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.GetObjects.RequestsPerSecond";
    internal const string LocalCacheGetObjectsObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.GetObjects.ObjectsPerSecond";
    internal const string LocalCacheAddObjectsRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.AddObjects.Requests";
    internal const string LocalCacheAddObjectsObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.AddObjects.Objects";
    internal const string LocalCacheAddObjectsRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.AddObjects.RequestsPerSecond";
    internal const string LocalCacheAddObjectsObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.AddObjects.ObjectsPerSecond";
    internal const string LocalCacheRemoveObjectsRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.RemoveObjects.Requests";
    internal const string LocalCacheRemoveObjectsObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.RemoveObjects.Objects";
    internal const string LocalCacheRemoveObjectsRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.RemoveObjects.RequestsPerSecond";
    internal const string LocalCacheRemoveObjectsObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalCache.RemoveObjects.ObjectsPerSecond";
    internal const string RemoteCacheGetObjectsRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.GetObjects.Requests";
    internal const string RemoteCacheGetObjectsObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.GetObjects.Objects";
    internal const string RemoteCacheGetObjectsRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.GetObjects.RequestsPerSecond";
    internal const string RemoteCacheGetObjectsObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.GetObjects.ObjectsPerSecond";
    internal const string RemoteCacheAddObjectsRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.AddObjects.Requests";
    internal const string RemoteCacheAddObjectsObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.AddObjects.Objects";
    internal const string RemoteCacheAddObjectsRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.AddObjects.RequestsPerSecond";
    internal const string RemoteCacheAddObjectsObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.AddObjects.ObjectsPerSecond";
    internal const string RemoteCacheRemoveObjectsRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.RemoveObjects.Requests";
    internal const string RemoteCacheRemoveObjectsObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.RemoveObjects.Objects";
    internal const string RemoteCacheRemoveObjectsRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.RemoveObjects.RequestsPerSecond";
    internal const string RemoteCacheRemoveObjectsObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.RemoveObjects.ObjectsPerSecond";
    internal const string RemoteCacheGetObjectsHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.GetObjects.Hits";
    internal const string RemoteCacheGetObjectsMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.GetObjects.Misses";
    internal const string RemoteCacheGetObjectsHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.GetObjects.HitsPerSecond";
    internal const string RemoteCacheGetObjectsMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsRemoteCache.GetObjects.MissesPerSecond";
    internal const string CacheServiceGetIdentitiesByDescriptorRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.Requests";
    internal const string CacheServiceGetIdentitiesByDescriptorObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.Objects";
    internal const string CacheServiceGetIdentitiesByDescriptorRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.RequestsPerSecond";
    internal const string CacheServiceGetIdentitiesByDescriptorObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.ObjectsPerSecond";
    internal const string CacheServiceGetIdentitiesByDescriptorHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.Hits";
    internal const string CacheServiceGetIdentitiesByDescriptorMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.Misses";
    internal const string CacheServiceGetIdentitiesByDescriptorHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.HitsPerSecond";
    internal const string CacheServiceGetIdentitiesByDescriptorMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.MissesPerSecond";
    internal const string CacheServiceSetIdentitiesByDescriptorRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDescriptor.Requests";
    internal const string CacheServiceSetIdentitiesByDescriptorObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDescriptor.Objects";
    internal const string CacheServiceSetIdentitiesByDescriptorRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDescriptor.RequestsPerSecond";
    internal const string CacheServiceSetIdentitiesByDescriptorObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDescriptor.ObjectsPerSecond";
    internal const string CacheServiceGetIdentitiesByDisplayNameRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Requests";
    internal const string CacheServiceGetIdentitiesByDisplayNameObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Objects";
    internal const string CacheServiceGetIdentitiesByDisplayNameRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.RequestsPerSecond";
    internal const string CacheServiceGetIdentitiesByDisplayNameObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.ObjectsPerSecond";
    internal const string CacheServiceGetIdentitiesByDisplayNameHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Hits";
    internal const string CacheServiceGetIdentitiesByDisplayNameMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Misses";
    internal const string CacheServiceGetIdentitiesByDisplayNameHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.HitsPerSecond";
    internal const string CacheServiceGetIdentitiesByDisplayNameMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.MissesPerSecond";
    internal const string CacheServiceSetIdentitiesByDisplayNameRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDisplayName.Requests";
    internal const string CacheServiceSetIdentitiesByDisplayNameObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDisplayName.Objects";
    internal const string CacheServiceSetIdentitiesByDisplayNameRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDisplayName.RequestsPerSecond";
    internal const string CacheServiceSetIdentitiesByDisplayNameObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDisplayName.ObjectsPerSecond";
    internal const string CacheServiceGetIdentitiesByAccountNameRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Requests";
    internal const string CacheServiceGetIdentitiesByAccountNameObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Objects";
    internal const string CacheServiceGetIdentitiesByAccountNameRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.RequestsPerSecond";
    internal const string CacheServiceGetIdentitiesByAccountNameObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.ObjectsPerSecond";
    internal const string CacheServiceGetIdentitiesByAccountNameHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Hits";
    internal const string CacheServiceGetIdentitiesByAccountNameMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Misses";
    internal const string CacheServiceGetIdentitiesByAccountNameHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.HitsPerSecond";
    internal const string CacheServiceGetIdentitiesByAccountNameMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.MissesPerSecond";
    internal const string CacheServiceSetIdentitiesByAccountNameRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByAccountName.Requests";
    internal const string CacheServiceSetIdentitiesByAccountNameObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByAccountName.Objects";
    internal const string CacheServiceSetIdentitiesByAccountNameRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByAccountName.RequestsPerSecond";
    internal const string CacheServiceSetIdentitiesByAccountNameObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByAccountName.ObjectsPerSecond";
    internal const string CacheServiceGetIdentitiesByIdRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.Requests";
    internal const string CacheServiceGetIdentitiesByIdObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.Objects";
    internal const string CacheServiceGetIdentitiesByIdRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.RequestsPerSecond";
    internal const string CacheServiceGetIdentitiesByIdObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.ObjectsPerSecond";
    internal const string CacheServiceGetIdentitiesByIdHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.Hits";
    internal const string CacheServiceGetIdentitiesByIdMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.Misses";
    internal const string CacheServiceGetIdentitiesByIdHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.HitsPerSecond";
    internal const string CacheServiceGetIdentitiesByIdMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.MissesPerSecond";
    internal const string CacheServiceSetIdentitiesByIdRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesById.Requests";
    internal const string CacheServiceSetIdentitiesByIdObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesById.Objects";
    internal const string CacheServiceSetIdentitiesByIdRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesById.RequestsPerSecond";
    internal const string CacheServiceSetIdentitiesByIdObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesById.ObjectsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByDisplayNameRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.Requests";
    internal const string CacheServiceSearchIdentityIdsByDisplayNameRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.RequestsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByDisplayNameHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.Hits";
    internal const string CacheServiceSearchIdentityIdsByDisplayNameMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.Misses";
    internal const string CacheServiceSearchIdentityIdsByDisplayNameHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.HitsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByDisplayNameMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.MissesPerSecond";
    internal const string CacheServiceCreateSearchIndexByDisplayNameRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByDisplayName.Requests";
    internal const string CacheServiceCreateSearchIndexByDisplayNameRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByDisplayName.RequestsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByAppIdRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.Requests";
    internal const string CacheServiceSearchIdentityIdsByAppIdRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.RequestsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByAppIdHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.Hits";
    internal const string CacheServiceSearchIdentityIdsByAppIdMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.Misses";
    internal const string CacheServiceSearchIdentityIdsByAppIdHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.HitsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByAppIdMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.MissesPerSecond";
    internal const string CacheServiceCreateSearchIndexByAppIdRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAppId.Requests";
    internal const string CacheServiceCreateSearchIndexByAppIdRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAppId.RequestsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByEmailRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.Requests";
    internal const string CacheServiceSearchIdentityIdsByEmailRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.RequestsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByEmailHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.Hits";
    internal const string CacheServiceSearchIdentityIdsByEmailMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.Misses";
    internal const string CacheServiceSearchIdentityIdsByEmailHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.HitsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByEmailMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.MissesPerSecond";
    internal const string CacheServiceCreateSearchIndexByEmailRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByEmail.Requests";
    internal const string CacheServiceCreateSearchIndexByEmailRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByEmail.RequestsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByAccountNameRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Requests";
    internal const string CacheServiceSearchIdentityIdsByAccountNameRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.RequestsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByAccountNameHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Hits";
    internal const string CacheServiceSearchIdentityIdsByAccountNameMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Misses";
    internal const string CacheServiceSearchIdentityIdsByAccountNameHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.HitsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByAccountNameMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.MissesPerSecond";
    internal const string CacheServiceCreateSearchIndexByAccountNameRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAlias.Requests";
    internal const string CacheServiceCreateSearchIndexByAccountNameRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAlias.RequestsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByDomainAccountNameRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Requests";
    internal const string CacheServiceSearchIdentityIdsByDomainAccountNameRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.RequestsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByDomainAccountNameHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Hits";
    internal const string CacheServiceSearchIdentityIdsByDomainAccountNameMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Misses";
    internal const string CacheServiceSearchIdentityIdsByDomainAccountNameHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.HitsPerSecond";
    internal const string CacheServiceSearchIdentityIdsByDomainAccountNameMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.MissesPerSecond";
    internal const string CacheServiceCreateSearchIndexByDomainAccountNameRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAlias.Requests";
    internal const string CacheServiceCreateSearchIndexByDomainAccountNameRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAlias.RequestsPerSecond";
    internal const string LocalSearchCacheSearchIndexRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.Requests";
    internal const string LocalSearchCacheSearchIndexObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.Objects";
    internal const string LocalSearchCacheSearchIndexRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.RequestsPerSecond";
    internal const string LocalSearchCacheSearchIndexObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.ObjectsPerSecond";
    internal const string LocalSearchCacheSearchIndexHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.Hits";
    internal const string LocalSearchCacheSearchIndexMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.Misses";
    internal const string LocalSearchCacheSearchIndexHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.HitsPerSecond";
    internal const string LocalSearchCacheSearchIndexMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.MissesPerSecond";
    internal const string LocalSearchCacheCreateIndexRequests = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.CreateIndex.Requests";
    internal const string LocalSearchCacheCreateIndexObjects = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.CreateIndex.Objects";
    internal const string LocalSearchCacheCreateIndexRequestsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.CreateIndex.RequestsPerSecond";
    internal const string LocalSearchCacheCreateIndexObjectsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.CreateIndex.ObjectsPerSecond";

    internal static class GitHubIdentities
    {
      internal const string GitHubBindPendingIdentityUpgradeToCompleteIdentity = "Microsoft.VisualStudio.Services.Identity.PerfCounters.GitHubIdentities.BindPendingUpgrade";
      internal const string GitHubSignInCounter = "Microsoft.VisualStudio.Services.Identity.PerfCounters.GitHubIdentities.SignInCounter";
      internal const string GitHubSuccessfulGetUserCounter = "Microsoft.VisualStudio.Services.Identity.PerfCounters.GitHubIdentities.SuccessfulGetUserCounter";
      internal const string GitHubFailedGetUserCounter = "Microsoft.VisualStudio.Services.Identity.PerfCounters.GitHubIdentities.FailedGetUserCounter";
    }

    internal static class IdentityChangeProcessor
    {
      internal static class Load
      {
        internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.Load.CallsPerSecond";
      }

      internal static class Unload
      {
        internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.Unload.CallsPerSecond";
      }

      internal static class InitializeSequenceIds
      {
        internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.InitializeSequenceIds.CallsPerSecond";
      }

      internal static class ProcessIdentityChange
      {
        internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsPerSecond";
        internal const string CallsDroppedImmediatelyBecauseChangesAlreadyProcessed = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsDroppedImmediatelyBecauseChangesAlreadyProcessed";
        internal const string CallsDroppedAfterConfirmingNoDescriptorChanges = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsDroppedAfterConfirmingNoDescriptorChanges";
        internal const string CallsDroppedAfterConfirmingDescriptorChangesAlreadyProcessed = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsDroppedAfterConfirmingDescriptorChangesAlreadyProcessed";
        internal const string CallsContinuedBecauseCreditLostForProcessingDescriptorChanges = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsContinuedBecauseCreditLostForProcessingDescriptorChanges";
        internal const string CallsWithMajorDescriptorChangeType = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithMajorDescriptorChangeType";
        internal const string CallsWithMinorDescriptorChangeType = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithMinorDescriptorChangeType";
        internal const string CallsWithInvalidDescriptorChangeType = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithInvalidDescriptorChangeType";
        internal const string CallsWithReadsToIdentityComponentGetChanges = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithReadsToIdentityComponentGetChanges";
        internal const string CallsWithReadsToGroupComponentGetChanges = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithReadsToGroupComponentGetChanges";
        internal const string CallsWithMembershipChanges = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithMembershipChanges";
        internal const string CallsThatUsedTaskToBroadcast = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatUsedTaskToBroadcast";
        internal const string CallsThatDidNotUseTaskToBroadcast = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatDidNotUseTaskToBroadcast";
        internal const string CallsThatUpdatedIdentitySequenceId = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatUpdatedIdentitySequenceId";
        internal const string CallsThatUpdatedGroupSequenceId = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatUpdatedGroupSequenceId";
        internal const string CallsWithUserMembershipChangesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsWithUserMembershipChangesPerSecond";
        internal const string CallsThatProcessedChanges = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatProcessedChanges";
        internal const string CallsThatDidNotProcessChanges = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatDidNotProcessChanges";
        internal const string CallsThatNotifiedDescriptorChange = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatNotifiedDescriptorChange";
        internal const string CallsThatPublishedAfterProcessMembershipChangesOnStoreEvent = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatPublishedAfterProcessMembershipChangesOnStoreEvent";
        internal const string CallsThatQueuedIdentityChangePublisherJob = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessIdentityChange.CallsThatQueuedIdentityChangePublisherJob";
      }

      internal static class ProcessParentIdentityChange
      {
        internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsPerSecond";
        internal const string CallsDroppedAfterConfirmingDescriptorChangesAlreadyProcessed = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsDroppedAfterConfirmingDescriptorChangesAlreadyProcessed";
        internal const string CallsContinuedBecauseCreditLostForProcessingDescriptorChanges = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsContinuedBecauseCreditLostForProcessingDescriptorChanges";
        internal const string CallsWithMajorDescriptorChangeType = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsWithMajorDescriptorChangeType";
        internal const string CallsWithMinorDescriptorChangeType = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsWithMinorDescriptorChangeType";
        internal const string CallsWithInvalidDescriptorChangeType = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsWithInvalidDescriptorChangeType";
        internal const string CallsThatNotifiedDescriptorChange = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityChangeProcessor.ProcessParentIdentityChange.CallsThatNotifiedDescriptorChange";
      }
    }

    internal static class IdentityIdTranslator
    {
      internal static class TranslateToMasterId
      {
        internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateToMasterId.CallsPerSecond";
        internal const string MissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateToMasterId.MissesPerSecond";
        internal const string HitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateToMasterId.HitsPerSecond";
      }

      internal static class TranslateFromMasterId
      {
        internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateFromMasterId.CallsPerSecond";
        internal const string MissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateFromMasterId.MissesPerSecond";
        internal const string HitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateFromMasterId.HitsPerSecond";
      }

      internal static class InitializeIdTranslationMaps
      {
        internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InitializeIdTranslationMaps.CallsPerSecond";
        internal const string CallsDroppedBeforeTakingLock = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InitializeIdTranslationMaps.CallsDroppedBeforeTakingLock";
        internal const string CallsDroppedAfterTakingLock = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InitializeIdTranslationMaps.CallsDroppedAfterTakingLock";
        internal const string CallsAccepted = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InitializeIdTranslationMaps.CallsAccepted";
        internal const string CallsCompleted = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InitializeIdTranslationMaps.CallsCompleted";
      }

      internal static class ClearCaches
      {
        internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.ClearCaches.CallsPerSecond";
        internal const string CallsDroppedBeforeTakingLock = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.ClearCaches.CallsDroppedBeforeTakingLock";
        internal const string CallsDroppedAfterTakingLock = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.ClearCaches.CallsDroppedAfterTakingLock";
        internal const string CallsAccepted = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.ClearCaches.CallsAccepted";
        internal const string CallsCompleted = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.ClearCaches.CallsCompleted";
      }

      internal static class WithReadLockOnIdTranslationMaps
      {
        internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.CallsPerSecond";

        internal static class InnerLoop
        {
          internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.InnerLoop.CallsPerSecond";
          internal const string CallsDroppedBeforeTakingLock = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.InnerLoop.CallsDroppedBeforeTakingLock";
          internal const string CallsDroppedAfterTakingLock = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.InnerLoop.CallsDroppedAfterTakingLock";
          internal const string CallsAccepted = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.InnerLoop.CallsAccepted";
          internal const string CallsCompleted = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.InnerLoop.CallsCompleted";
        }
      }

      internal static class InvalidateIdTranslationCache
      {
        internal static class ChangeTypeAdded
        {
          internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeAdded.CallsPerSecond";
          internal const string CallsDroppedBeforeTakingLock = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeAdded.CallsDroppedBeforeTakingLock";
          internal const string CallsDroppedAfterTakingLock = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeAdded.CallsDroppedAfterTakingLock";
          internal const string CallsAccepted = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeAdded.CallsAccepted";
          internal const string CallsCompleted = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeAdded.CallsCompleted";
        }

        internal static class ChangeTypeRemoved
        {
          internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsPerSecond";
          internal const string CallsDroppedBeforeTakingLock = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsDroppedBeforeTakingLock";
          internal const string CallsDroppedAfterTakingLock = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsDroppedAfterTakingLock";
          internal const string CallsAccepted = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsAccepted";
          internal const string CallsCompleted = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsCompleted";
        }

        internal static class ChangeTypeBulkChange
        {
          internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeBulkChange.CallsPerSecond";
        }
      }
    }

    internal static class ReadAncestorMemberships
    {
      internal const string Calls = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.Calls";
      internal const string CallsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CallsPerSecond";
      internal const string CacheHits = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheHits";
      internal const string CacheHitsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheHitsPerSecond";
      internal const string CacheMisses = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheMisses";
      internal const string CacheMissesPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheMissesPerSecond";
      internal const string CacheAdditions = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheAdditions";
      internal const string CacheAdditionsPerSecond = "Microsoft.VisualStudio.Services.Identity.PerfCounters.ReadAncestorMemberships.CacheAdditionsPerSecond";
    }
  }
}
