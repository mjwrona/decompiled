// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.ContentTracePoints
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class ContentTracePoints
  {
    public static class XStore
    {
      public static readonly string Area = nameof (XStore);
      public static readonly string Layer = "Table";
      public static readonly SingleLocationTracePoint ExecuteQuerySegmentedAsync = ContentTracePoints.XStore.CreateSingleLocationTracePoint(5703000);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, ContentTracePoints.XStore.Area, ContentTracePoints.XStore.Layer);
    }

    public static class ServerCommon
    {
      public static readonly string Area = nameof (ServerCommon);
      public static readonly string Layer = "Service";
      public static readonly SingleLocationTracePoint GetTotalPartitionsInfo = ContentTracePoints.ServerCommon.CreateSingleLocationTracePoint(5701950);
      public static readonly SingleLocationTracePoint GetTotalPartitionsException = ContentTracePoints.ServerCommon.CreateSingleLocationTracePoint(5701951);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, ContentTracePoints.ServerCommon.Area, ContentTracePoints.ServerCommon.Layer);

      private static EnterLeaveTracePoint CreateEnterLeaveTracePoint(
        int enterTracePoint,
        int leaveTracePoint)
      {
        return new EnterLeaveTracePoint(enterTracePoint, leaveTracePoint, ContentTracePoints.ServerCommon.Area, ContentTracePoints.ServerCommon.Layer);
      }
    }

    public static class StorageAccountConfigurationService
    {
      public static readonly string Area = nameof (StorageAccountConfigurationService);
      public static readonly string Layer = "Service";
      public static readonly EnterLeaveTracePoint ReadStorageAccountsCall = ContentTracePoints.StorageAccountConfigurationService.CreateEnterLeaveTracePoint(5701801, 5701802);
      public static readonly SingleLocationTracePoint ReadStorageAccountsInfo = ContentTracePoints.StorageAccountConfigurationService.CreateSingleLocationTracePoint(5701803);
      public static readonly EnterLeaveTracePoint ReadLocationModeCall = ContentTracePoints.StorageAccountConfigurationService.CreateEnterLeaveTracePoint(5701804, 5701805);
      public static readonly SingleLocationTracePoint ReadLocationModeInfo = ContentTracePoints.StorageAccountConfigurationService.CreateSingleLocationTracePoint(5701806);
      public static readonly EnterLeaveTracePoint OnConnectionStringSecretChangedCall = ContentTracePoints.StorageAccountConfigurationService.CreateEnterLeaveTracePoint(5701807, 5701808);
      public static readonly SingleLocationTracePoint ConnectionStringUpdated = ContentTracePoints.StorageAccountConfigurationService.CreateSingleLocationTracePoint(5701809);
      public static readonly SingleLocationTracePoint ConnectionStringMissing = ContentTracePoints.StorageAccountConfigurationService.CreateSingleLocationTracePoint(5701810);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, ContentTracePoints.StorageAccountConfigurationService.Area, ContentTracePoints.StorageAccountConfigurationService.Layer);

      private static EnterLeaveTracePoint CreateEnterLeaveTracePoint(
        int enterTracePoint,
        int leaveTracePoint)
      {
        return new EnterLeaveTracePoint(enterTracePoint, leaveTracePoint, ContentTracePoints.StorageAccountConfigurationService.Area, ContentTracePoints.StorageAccountConfigurationService.Layer);
      }
    }

    public static class ItemStore
    {
      public static readonly string Area = nameof (ItemStore);
      public static readonly string Layer = "Service";
      public static readonly EnterLeaveTracePoint GetContainerAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713020, 5713021);
      public static readonly SingleLocationTracePoint GetContainerAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713022);
      public static readonly EnterLeaveTracePoint GetOrAddContainerAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713023, 5713024);
      public static readonly SingleLocationTracePoint GetOrAddContainerAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713025);
      public static readonly EnterLeaveTracePoint AssociateItemsAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713026, 5713027);
      public static readonly SingleLocationTracePoint AssociateItemsAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713028);
      public static readonly SingleLocationTracePoint TraceBlobIdAndReference = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713029);
      public static readonly EnterLeaveTracePoint RemoveBlobItemReferencesAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713030, 5713031);
      public static readonly SingleLocationTracePoint RemoveBlobItemReferencesAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713032);
      public static readonly EnterLeaveTracePoint UpdateContainerAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713033, 5713034);
      public static readonly SingleLocationTracePoint UpdateContainerAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713035);
      public static readonly EnterLeaveTracePoint DeleteContainerAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713036, 5713037);
      public static readonly SingleLocationTracePoint DeleteContainerAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713038);
      public static readonly EnterLeaveTracePoint DeleteItemAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713039, 5713040);
      public static readonly SingleLocationTracePoint DeleteItemAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713041);
      public static readonly EnterLeaveTracePoint DeleteItemsAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713042, 5713043);
      public static readonly SingleLocationTracePoint DeleteItemsAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713044);
      public static readonly EnterLeaveTracePoint TryAddBlobItemAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713045, 5713046);
      public static readonly SingleLocationTracePoint MaterializeFileList = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713047);
      public static readonly SingleLocationTracePoint FileListDoesNotExist = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713048);
      public static readonly SingleLocationTracePoint UpdatingDropItemFailed = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713049);
      public static readonly SingleLocationTracePoint UpdatingDropItemLostRace = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713050);
      public static readonly SingleLocationTracePoint UpdatingDropItemLostRaceMismatch = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713051);
      public static readonly SingleLocationTracePoint UpdatingDropItemSuccess = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713052);
      public static readonly SingleLocationTracePoint ManifestRollbackAttempt = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713053);
      public static readonly SingleLocationTracePoint ManifestRollbackSuccess = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713054);
      public static readonly EnterLeaveTracePoint CompareSwapItemAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713055, 5713056);
      public static readonly SingleLocationTracePoint CompareSwapItemAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713057);
      public static readonly EnterLeaveTracePoint CompareSwapItemsAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713058, 5713059);
      public static readonly SingleLocationTracePoint CompareSwapItemsAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713060);
      public static readonly EnterLeaveTracePoint GetItemAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713061, 5713062);
      public static readonly SingleLocationTracePoint GetItemAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713063);
      public static readonly EnterLeaveTracePoint GetOrAddItemAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713064, 5713065);
      public static readonly SingleLocationTracePoint GetOrAddItemAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713066);
      public static readonly EnterLeaveTracePoint GetContainersConcurrentIteratorAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713067, 5713068);
      public static readonly SingleLocationTracePoint GetContainersConcurrentIteratorAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713069);
      public static readonly EnterLeaveTracePoint GetItemsConcurrentIteratorAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713070, 5713071);
      public static readonly SingleLocationTracePoint GetItemsConcurrentIteratorAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713072);
      public static readonly EnterLeaveTracePoint GetResumableItemsConcurrentIteratorAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713073, 5713074);
      public static readonly SingleLocationTracePoint GetResumableItemsConcurrentIteratorAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713075);
      public static readonly EnterLeaveTracePoint GetItemPagesConcurrentIteratorAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713076, 5713077);
      public static readonly SingleLocationTracePoint GetItemPagesConcurrentIteratorAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713078);
      public static readonly EnterLeaveTracePoint GetItemsConcurrentIteratorAsync2Call = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713079, 5713080);
      public static readonly SingleLocationTracePoint GetItemsConcurrentIteratorAsync2Exception = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713081);
      public static readonly EnterLeaveTracePoint MoveItemsAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713082, 5713083);
      public static readonly SingleLocationTracePoint MoveItemsAsyncException = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713084);
      public static readonly EnterLeaveTracePoint RemoveBlobReferencesAsyncCall = ContentTracePoints.ItemStore.CreateEnterLeaveTracePoint(5713085, 5713086);
      public static readonly SingleLocationTracePoint RemoveBlobBlobId = ContentTracePoints.ItemStore.CreateSingleLocationTracePoint(5713087);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, ContentTracePoints.ItemStore.Area, ContentTracePoints.ItemStore.Layer);

      private static EnterLeaveTracePoint CreateEnterLeaveTracePoint(
        int enterTracePoint,
        int leaveTracePoint)
      {
        return new EnterLeaveTracePoint(enterTracePoint, leaveTracePoint, ContentTracePoints.ItemStore.Area, ContentTracePoints.ItemStore.Layer);
      }
    }

    public static class PlatformBlobStore
    {
      public const string Area = "BlobStore";
      public const string Layer = "PlatformBlobStore";
      public static readonly EnterLeaveTracePoint GetBlobAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701010, 5701011);
      public static readonly SingleLocationTracePoint GetBlobAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701012);
      public static readonly EnterLeaveTracePoint PutBlobAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701013, 5701014);
      public static readonly SingleLocationTracePoint PutBlobAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701015);
      public static readonly EnterLeaveTracePoint RemoveReferencesWithResultsAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701016, 5701017);
      public static readonly SingleLocationTracePoint RemoveReferencesWithResultsAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701018);
      public static readonly SingleLocationTracePoint RemoveReferencesWithResultsAsyncBlobInfo = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701019);
      public static readonly EnterLeaveTracePoint TryReferenceAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701020, 5701021);
      public static readonly SingleLocationTracePoint TryReferenceAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701022);
      public static readonly EnterLeaveTracePoint TryReferenceWithBlocksAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701023, 5701024);
      public static readonly SingleLocationTracePoint TryReferenceWithBlocksAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701025);
      public static readonly EnterLeaveTracePoint PutBlobAndReferenceAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701026, 5701027);
      public static readonly SingleLocationTracePoint PutBlobAndReferenceAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701028);
      public static readonly EnterLeaveTracePoint GetSasUrisAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701029, 5701030);
      public static readonly SingleLocationTracePoint GetSasUrisAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701031);
      public static readonly EnterLeaveTracePoint PutSingleBlockBlobAndReferenceAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701032, 5701033);
      public static readonly SingleLocationTracePoint PutSingleBlockBlobAndReferenceAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701034);
      public static readonly EnterLeaveTracePoint GetDownloadUriAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701035, 5701036);
      public static readonly SingleLocationTracePoint GetDownloadUriAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701037);
      public static readonly EnterLeaveTracePoint GetDownloadUrisAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701038, 5701039);
      public static readonly SingleLocationTracePoint GetDownloadUrisAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701040);
      public static readonly EnterLeaveTracePoint ValidateKeepUntilReferencesAsyncCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701041, 5701042);
      public static readonly SingleLocationTracePoint ValidateKeepUntilReferencesAsyncException = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701043);
      public static readonly SingleLocationTracePoint PutBlobBlockHashMismatch = ContentTracePoints.PlatformBlobStore.CreateSingleLocationTracePoint(5701044);
      public static readonly EnterLeaveTracePoint ConfigureBlobStorageCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701045, 5701046);
      public static readonly EnterLeaveTracePoint ConfigureMetadataStorageCall = ContentTracePoints.PlatformBlobStore.CreateEnterLeaveTracePoint(5701047, 5701048);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, "BlobStore", nameof (PlatformBlobStore));

      private static EnterLeaveTracePoint CreateEnterLeaveTracePoint(
        int enterTracePoint,
        int leaveTracePoint)
      {
        return new EnterLeaveTracePoint(enterTracePoint, leaveTracePoint, "BlobStore", nameof (PlatformBlobStore));
      }
    }

    public static class AdminPlatformBlobstore
    {
      public const string Area = "BlobStore";
      public const string Layer = "AdminPlatformBlobstore";
      public static readonly EnterLeaveTracePoint FixMetadataAfterDisasterAsyncCall = ContentTracePoints.AdminPlatformBlobstore.CreateEnterLeaveTracePoint(5701049, 5701050);
      public static readonly SingleLocationTracePoint FixMetadataAfterDisasterAsyncException = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701051);
      public static readonly SingleLocationTracePoint FixMetadataAfterDisasterAsyncBegin = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701052);
      public static readonly SingleLocationTracePoint FixMetadataAfterDisasterAsyncRangeFinish = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701053);
      public static readonly SingleLocationTracePoint FixMetadataAfterDisasterAsyncResult = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701054);
      public static readonly SingleLocationTracePoint ServiceDeleteRetentionOnStorageAccountsInfo = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701055);
      public static readonly SingleLocationTracePoint AccountForSoftDeletedBytesFromContainersAsyncInfo = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701056);
      public static readonly SingleLocationTracePoint AccountForSoftDeletedBytesFromContainersAsyncListed = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701057);
      public static readonly SingleLocationTracePoint AccountForSoftDeletedBytesFromContainersAsyncException = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701058);
      public static readonly SingleLocationTracePoint FetchBlobContainersInfo = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701059);
      public static readonly EnterLeaveTracePoint CollectStorageDataFromContainersAsyncCall = ContentTracePoints.AdminPlatformBlobstore.CreateEnterLeaveTracePoint(5701060, 5701061);
      public static readonly SingleLocationTracePoint CollectStorageDataFromContainersAsyncException = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701062);
      public static readonly EnterLeaveTracePoint CheckForDeletionAsyncCall = ContentTracePoints.AdminPlatformBlobstore.CreateEnterLeaveTracePoint(5701063, 5701064);
      public static readonly SingleLocationTracePoint UpdateServicePointSettingsInfo = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701065);
      public static readonly SingleLocationTracePoint CheckForDeletionAsyncStartDelete = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701066);
      public static readonly SingleLocationTracePoint CheckForDeletionAsyncProcessingBlobId = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701067);
      public static readonly SingleLocationTracePoint CheckForDeletionAsyncException = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701068);
      public static readonly SingleLocationTracePoint CheckForDeletionAsyncCompleted = ContentTracePoints.AdminPlatformBlobstore.CreateSingleLocationTracePoint(5701069);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, "BlobStore", nameof (AdminPlatformBlobstore));

      private static EnterLeaveTracePoint CreateEnterLeaveTracePoint(
        int enterTracePoint,
        int leaveTracePoint)
      {
        return new EnterLeaveTracePoint(enterTracePoint, leaveTracePoint, "BlobStore", nameof (AdminPlatformBlobstore));
      }
    }

    public static class DedupStoreService
    {
      public const string Area = "DedupStore";
      public const string Layer = "DedupStoreService";
      public static readonly EnterLeaveTracePoint GetChunksAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702010, 5702011);
      public static readonly SingleLocationTracePoint GetChunksAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702012);
      public static readonly EnterLeaveTracePoint PutChunkAndKeepUntilReferenceAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702020, 5702021);
      public static readonly SingleLocationTracePoint PutChunkAndKeepUntilReferenceAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702022);
      public static readonly EnterLeaveTracePoint TryKeepUntilReferenceChunkAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702030, 5702031);
      public static readonly SingleLocationTracePoint TryKeepUntilReferenceChunkAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702032);
      public static readonly EnterLeaveTracePoint GetNodeAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702040, 5702041);
      public static readonly SingleLocationTracePoint GetNodeAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702042);
      public static readonly EnterLeaveTracePoint PutNodeAndKeepUntilReferenceAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702050, 5702051);
      public static readonly SingleLocationTracePoint PutNodeAndKeepUntilReferenceAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702052);
      public static readonly EnterLeaveTracePoint TryKeepUntilReferenceNodeAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702060, 5702061);
      public static readonly SingleLocationTracePoint TryKeepUntilReferenceNodeAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702062);
      public static readonly EnterLeaveTracePoint GetUrisCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702070, 5702071);
      public static readonly SingleLocationTracePoint GetUrisException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702072);
      public static readonly EnterLeaveTracePoint GetDownloadInfoAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702045, 5702046);
      public static readonly SingleLocationTracePoint GetDownloadInfoAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702047);
      public static readonly EnterLeaveTracePoint DeleteRootAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702080, 5702081);
      public static readonly SingleLocationTracePoint DeleteRootAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702082);
      public static readonly EnterLeaveTracePoint PutRootAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702090, 5702091);
      public static readonly SingleLocationTracePoint PutRootAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702092);
      public static readonly EnterLeaveTracePoint VisitDedupsTopDownAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702095, 5702096);
      public static readonly SingleLocationTracePoint VisitDedupsTopDownAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702114);
      public static readonly SingleLocationTracePoint VisitDedupsTopDownAsyncInfo = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702094);
      public static readonly SingleLocationTracePoint MissingNodeException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702093);
      public static readonly SingleLocationTracePoint MissingNodeRecovered = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702193);
      public static readonly EnterLeaveTracePoint MarkRootsAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702100, 5702101);
      public static readonly SingleLocationTracePoint MarkRootsAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702102);
      public static readonly SingleLocationTracePoint MarkRootsAsyncInfo = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702109);
      public static readonly EnterLeaveTracePoint VerifyRootsAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702103, 5702104);
      public static readonly SingleLocationTracePoint VerifyRootsAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702105);
      public static readonly SingleLocationTracePoint VerifyRootsAsyncInfo = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702110);
      public static readonly EnterLeaveTracePoint DeleteExpiredDedupsAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702106, 5702107);
      public static readonly SingleLocationTracePoint DeleteExpiredDedupsAsyncException = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702108);
      public static readonly SingleLocationTracePoint DeleteExpiredDedupsAsyncInfo = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702113);
      public static readonly EnterLeaveTracePoint SaveCheckpointCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702111, 5702112);
      public static readonly SingleLocationTracePoint ChunkDedupLauncherJobInfo = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702119);
      public static readonly SingleLocationTracePoint InvalidKUDedupFound = ContentTracePoints.DedupStoreService.CreateSingleLocationTracePoint(5702124);
      public static readonly EnterLeaveTracePoint GetDedupSizeAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702120, 5702121);
      public static readonly EnterLeaveTracePoint RestoreRootAsyncCall = ContentTracePoints.DedupStoreService.CreateEnterLeaveTracePoint(5702122, 5702123);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, "DedupStore", nameof (DedupStoreService));

      private static EnterLeaveTracePoint CreateEnterLeaveTracePoint(
        int enterTracePoint,
        int leaveTracePoint)
      {
        return new EnterLeaveTracePoint(enterTracePoint, leaveTracePoint, "DedupStore", nameof (DedupStoreService));
      }
    }

    public static class StorageAccountPingJob
    {
      public static readonly string Area = nameof (StorageAccountPingJob);
      public static readonly string Layer = "Job";
      public static readonly SingleLocationTracePoint PingJobStatistics = ContentTracePoints.StorageAccountPingJob.CreateSingleLocationTracePoint(5703010);
      public static readonly SingleLocationTracePoint PingJobTrace = ContentTracePoints.StorageAccountPingJob.CreateSingleLocationTracePoint(5703011);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, ContentTracePoints.StorageAccountPingJob.Area, ContentTracePoints.StorageAccountPingJob.Layer);
    }

    public static class StorageMetricsTransactionPopulatorJob
    {
      public static readonly string Area = nameof (StorageMetricsTransactionPopulatorJob);
      public static readonly string Layer = "Job";
      public static readonly SingleLocationTracePoint StorageMetricsTransactionPopulatorTrace = ContentTracePoints.StorageMetricsTransactionPopulatorJob.CreateSingleLocationTracePoint(5702125);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, ContentTracePoints.StorageMetricsTransactionPopulatorJob.Area, ContentTracePoints.StorageMetricsTransactionPopulatorJob.Layer);
    }

    public static class BlobStoreLogSettingsJob
    {
      public static readonly string Area = nameof (BlobStoreLogSettingsJob);
      public static readonly string Layer = "Job";
      public static readonly SingleLocationTracePoint SettingsInfo = ContentTracePoints.BlobStoreLogSettingsJob.CreateSingleLocationTracePoint(5703012);
      public static readonly SingleLocationTracePoint SettingsDetailedTrace = ContentTracePoints.BlobStoreLogSettingsJob.CreateSingleLocationTracePoint(5703013);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, ContentTracePoints.BlobStoreLogSettingsJob.Area, ContentTracePoints.BlobStoreLogSettingsJob.Layer);
    }

    public static class ClientTool
    {
      public static readonly string Area = nameof (ClientTool);
      public static readonly string Layer = "Service";
      public const int GetClientSettingController = 5709040;
      public static readonly EnterLeaveTracePoint GetSettingsCall = ContentTracePoints.ClientTool.CreateEnterLeaveTracePoint(5709041, 5709042);
      public static readonly SingleLocationTracePoint GetSettingsException = ContentTracePoints.ClientTool.CreateSingleLocationTracePoint(5709043);
      public static readonly EnterLeaveTracePoint SetSettingsCall = ContentTracePoints.ClientTool.CreateEnterLeaveTracePoint(5709044, 5709045);
      public static readonly SingleLocationTracePoint SetSettingsException = ContentTracePoints.ClientTool.CreateSingleLocationTracePoint(5709046);
      public static readonly EnterLeaveTracePoint DeleteSettingsCall = ContentTracePoints.ClientTool.CreateEnterLeaveTracePoint(5709047, 5709048);
      public static readonly SingleLocationTracePoint DeleteSettingsException = ContentTracePoints.ClientTool.CreateSingleLocationTracePoint(5709049);

      private static SingleLocationTracePoint CreateSingleLocationTracePoint(int tracePoint) => new SingleLocationTracePoint(tracePoint, ContentTracePoints.ClientTool.Area, ContentTracePoints.ClientTool.Layer);

      private static EnterLeaveTracePoint CreateEnterLeaveTracePoint(
        int enterTracePoint,
        int leaveTracePoint)
      {
        return new EnterLeaveTracePoint(enterTracePoint, leaveTracePoint, ContentTracePoints.ClientTool.Area, ContentTracePoints.ClientTool.Layer);
      }
    }
  }
}
