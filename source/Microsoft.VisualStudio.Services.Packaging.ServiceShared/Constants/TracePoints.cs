// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants
{
  internal static class TracePoints
  {
    private const int PackagingSharedBase = 5724000;

    internal static class CommitLogService
    {
      internal const int GetNewest = 5724100;
      internal const int GetEntry = 5724110;
      internal const int AppendEntry = 5724120;
      internal const int GetOldest = 5724130;
      internal const int UnsafeBatchAppend = 5724140;
      internal const int BatchRead = 5724150;
      internal const int GetCommitLogIdAsync = 5724160;
      internal const int DeleteEntryAsync = 5724170;
      internal const int MarkCommitLogEntryCorrupt = 5724180;
      internal const int DeleteHeadTailEntryAsync = 5724190;
      private const int CommitLogBase = 5724100;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (CommitLogService)
      };
    }

    internal static class ItemStoreBookmarkTokenProvider
    {
      internal const int GetToken = 5724200;
      internal const int StoreToken = 5724210;
      internal const int GetOrStoreToken = 5724220;
      private const int ItemStoreBookmarkTokenProviderBase = 5724200;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (ItemStoreBookmarkTokenProvider)
      };
    }

    internal static class PackageMetadataService
    {
      internal const int GetLatestStateForPackageVersion = 5724300;
      internal const int AddMetadataEntry = 5724310;
      internal const int GetLatestStateForAllVersionsOfPackage = 5724320;
      internal const int GetAllVersionsOfPackage = 5724330;
      internal const int GetLatestStateForPackageVersions = 5724340;
      private const int PackageMetadataServiceBase = 5724300;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (PackageMetadataService)
      };
    }

    internal static class FeedJobQueuer
    {
      internal const int QueueFeedProcessingJob = 5724500;
      internal const int RegisterJobDefinition = 5724510;
      internal const int UpdateJobPriorityClassIfRequired = 5724520;
      internal const int TryRetryFailedJob = 5724530;
      internal const int QueryLatestJobResult = 5724540;
      internal const int TryQueueFeedProcessingJob = 5724550;
      private const int FeedJobQueuerBase = 5724500;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (FeedJobQueuer)
      };
    }

    internal static class CollectionChangeProcessingJob
    {
      internal const int Run = 5724600;
      internal const int QueueFeedLevelProcessingJob = 5724610;
      internal const int RunInternal = 5724620;
      private const int CollectionChangeProcessingJobBase = 5724600;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (CollectionChangeProcessingJob)
      };
    }

    internal static class CollectionDeletedFeedsProcessingJob
    {
      internal const int Run = 5724800;
      internal const int ProcessDeletedFeedChanges = 5724810;
      internal const int TrySetFeedChangesContinuationToken = 5724820;
      private const int CollectionDeletedFeedsProcessingJobBase = 5724800;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (CollectionDeletedFeedsProcessingJob)
      };
    }

    internal static class DeletedFeedCommitLogChangeProcessor
    {
      internal const int ProcessChange = 5724900;
      internal const int DeleteDropIfExists = 5724910;
      private const int DeletedFeedCommitLogChangeProcessorBase = 5724900;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (DeletedFeedCommitLogChangeProcessor)
      };
    }

    internal static class UpstreamCacheProcessingJobQueuer
    {
      internal const int TryQueueUserInitiatedFeedJob = 5725200;
      private const int UpstreamCacheProcessingJobQueuerBase = 5725200;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (UpstreamCacheProcessingJobQueuer)
      };
    }

    internal static class UpstreamCacheProcessingJob
    {
      internal const int Run = 5725300;
      internal const int RunInternal = 5725310;
      private const int UpstreamCacheProcessingJobBase = 5725300;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (UpstreamCacheProcessingJob)
      };
    }

    internal static class UpstreamCachingPackageMetadataService
    {
      internal const int GetLatestStateForPackageVersion = 5725400;
      internal const int AddMetadataEntry = 5725410;
      internal const int GetLatestStateForAllVersionsOfPackage = 5725420;
      internal const int GetAllVersionsOfPackage = 5725430;
      internal const int IsPackageNameOwnedByRegistry = 5725440;
      internal const int GetLatestCachedStateForAllVersionsOfPackage = 5725450;
      private const int UpstreamCachingPackageMetadataServiceBase = 5725400;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (UpstreamCachingPackageMetadataService)
      };
    }

    internal static class UpstreamRevisionCleanupService
    {
      internal const int AddRevisionToClean = 5725700;
      internal const int GetRevisionsForCleanup = 5725710;
      internal const int RemoveRevisionToClean = 5725720;
      internal const int CleanRevision = 5725730;
      internal const int GetStaleEntries = 5725740;
      private const int UpstreamRevisionCleanupServiceBase = 5725700;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (UpstreamRevisionCleanupService)
      };
    }

    internal static class UpstreamPackageMetadataCacheService
    {
      internal const int AddIsOwnedByLocalRegistry = 5725600;
      internal const int TryGetIsOwnedByLocalRegistry = 5725610;
      internal const int HandleOwnershipChangedSqlNotification = 5725620;
      internal const int HandleOwnershipChanged = 5725630;
      private const int UpstreamMetadataCacheServiceBase = 5725600;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (UpstreamPackageMetadataCacheService)
      };
    }

    internal static class UpstreamPackageMetadataHelperCache
    {
      internal const int IsPackageNameOwnedByLocalRegistry = 5725700;
      private const int UpstreamPackageMetadataHelperCacheBase = 5725700;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (UpstreamPackageMetadataHelperCache)
      };
    }

    internal static class RecoveryJobMaster
    {
      internal const int RecoveryJobMasterTracePoint = 5725800;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (RecoveryJobMaster)
      };
    }

    internal static class RecoveryWorkerJob
    {
      internal const int RecoveryWorkerJobTracePoint = 5725900;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (RecoveryWorkerJob)
      };
    }

    internal static class TracingPackageBlobEdgeCachingService
    {
      internal const int GetEdgeUri = 5726000;
      internal const int UserIsExcluded = 5726010;
      private const int TracingPackageBlobEdgeCachingServiceBase = 5726000;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (TracingPackageBlobEdgeCachingService)
      };
    }

    internal static class TracingItemStore
    {
      internal const int AssociateItemsAsync = 5726100;
      internal const int CompareSwapItemAsync = 5726110;
      internal const int CompareSwapItemsAsync = 5726120;
      internal const int DeleteContainerAsync = 5726130;
      internal const int DeleteItemAsync = 5726140;
      internal const int GetContainerAsync = 5726150;
      internal const int GetContainersConcurrentIteratorAsync = 5726160;
      internal const int GetItemAsync = 5726170;
      internal const int GetItemsConcurrentIteratorAsync = 5726180;
      internal const int GetItemPagesConcurrentIteratorAsync = 5726180;
      internal const int GetOrAddContainerAsync = 5726190;
      private const int TracingItemStoreBase = 5726100;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (TracingItemStore)
      };
    }

    internal static class TracingItemStoreContinued
    {
      internal const int MoveItemsAsync = 5726200;
      internal const int TryAddBlobItemAsync = 5726210;
      internal const int UpdateContainerAsync = 5726220;
      private const int TracingItemStoreContinuedBase = 5726200;
    }

    internal static class TracingFeedIndexInternalClient
    {
      internal const int AddFilesToPackageVersionAsync = 5726300;
      internal const int DeletePackageVersionAsync = 5726310;
      internal const int GetPackageIdAsync = 5726320;
      internal const int GetPackagesAsync = 5726330;
      internal const int PackageVersionViewOperationAsync = 5726340;
      internal const int SetIndexEntryAsync = 5726350;
      internal const int UpdatePackageVersionAsync = 5726360;
      internal const int UpdatePackageVersionsAsync = 5726370;
      internal const int UpdatePackageVersion = 5726380;
      private const int TracingFeedIndexInternalClientBase = 5726300;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (TracingFeedIndexInternalClient)
      };
    }

    internal static class PackageMetricsService
    {
      internal const int UpdatePackageMetricsAsync = 5726700;
      internal const int ServiceStart = 5726710;
      internal const int ServiceEnd = 5726720;
      internal const int UpdatePackageMetricsInFeed = 5726730;
      private const int PackageMetricsServiceBase = 5726700;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (PackageMetricsService)
      };
    }

    internal static class Miscellaneous
    {
      internal const int PackageUpstreamBehaviorServiceOverrideByPatternError = 5726800;
      internal const int PackageUpstreamBehaviorServiceOverrideByPatternMatch = 5726801;
      private const int MiscBase = 5726800;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Packaging"
      };
    }

    internal static class Provenance
    {
      internal const int ProvenanceUtils = 5726900;
      internal const int BuildClaimSessionRequestProvider = 5726910;
      private const int ProvenanceBase = 5726900;
    }

    internal static class SqlComponents
    {
      public const int SqlComponentsBase = 5727000;
    }
  }
}
