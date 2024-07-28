// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ServiceRegistryConstants
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public sealed class ServiceRegistryConstants
  {
    internal const string WorkerThreadsPerCoreRegistryPath = "/Configuration/BlobStore/WorkerThreadsPerCore";
    internal const string CompletionThreadsPerCoreRegistryPath = "/Configuration/BlobStore/CompletionThreadsPerCore";
    internal const string BlobProviderImplementationRegistryPath = "/Configuration/BlobStore/BlobProviderImplementation";
    internal const string FileBlobProvider = "FILE";
    internal const string AzureBlobProvider = "AZURE";
    public const string MemoryBlobProvider = "MEMORY";
    internal const string SqlBlobProvider = "SQL";
    internal static readonly string[] BlobProviders = new string[4]
    {
      "AZURE",
      "FILE",
      "MEMORY",
      "SQL"
    };
    internal const string BlobMetadataProviderImplementationRegistryPath = "/Configuration/BlobStore/BlobMetadataProviderImplementation";
    public const string MemoryTableMetadataProvider = "MEMORYTABLEBLOBMETADATAPROVIDER";
    internal const string AzureTableBlobMetadataProvider = "AZURETABLEBLOBMETADATAPROVIDER";
    internal const string SQLTableBlobMetadataProvider = "SQLTABLEBLOBMETADATAPROVIDER";
    internal static readonly string[] BlobMetadataProviders = new string[3]
    {
      "AZURETABLEBLOBMETADATAPROVIDER",
      "MEMORYTABLEBLOBMETADATAPROVIDER",
      "SQLTABLEBLOBMETADATAPROVIDER"
    };
    internal const string ShardingStrategyRegistryPath = "/Configuration/BlobStore/ShardingStrategy";
    private const string SharedAccessPolicyRegistryIdSuffixPath = "SharedAccessPolicy/Id";
    public const string SharedAccessPolicyRegistryPath = "/Configuration/BlobStore/SharedAccessPolicy/Id";
    public const int DefaultSharedAccessPolicyIdValue = 1;
    internal const string RegistryRootPath = "/Configuration/BlobStore";
    private const string BlobProviderImplementationKey = "BlobProviderImplementation";
    private const string BlobMetadataProviderImplementationKey = "BlobMetadataProviderImplementation";
    private const string WorkerThreadsPerCoreKey = "WorkerThreadsPerCore";
    private const string CompletionThreadsPerCoreKey = "CompletionThreadsPerCore";
    public const string BlobDeletionJobRootPath = "/Configuration/BlobStore/BlobDeletionJob";
    public const string StorageInfoJobRootPath = "/Configuration/BlobStore/StorageInfoJob";
    public const string DedupStorageInfoJobRootPath = "/Configuration/BlobStore/DedupStorageInfoJob";
    public const string ChunkDedupLogicalSizeJobRootPath = "/Configuration/BlobStore/ChunkDedupLogicalSizeJob";
    public const string ChunkDedupPhysicalSizeJobRootPath = "/Configuration/BlobStore/ChunkDedupPhysicalSizeJob";
    public const string ChunkPackageVolumeByFeedJobRootPath = "/Configuration/BlobStore/ChunkPackageVolumeByFeedJob";
    public const string FileStorageSizeJobRootPath = "/Configuration/BlobStore/FileStorageSizeJob";
    public const string FilePackageVolumeByFeedJobRootPath = "/Configuration/BlobStore/FilePackageVolumeByFeedJob";
    public const string EgressComputeJobRootPath = "/Configuration/BlobStore/EgressComputeJob";
    public const string HardDeleteRootsJobRootPath = "/Configuration/BlobStore/HardDeleteRootsJob";
    public const string StorageVolumeMeterJobRootPath = "/Configuration/BlobStore/StorageVolumeMeterJob";
    public const string BillingQuotaCappingRootPath = "/Configuration/BlobStore/QuotaCapping";
    public const string DedupTreeRestoreRootPath = "/Configuration/BlobStore/DedupTreeRestoreJob";
    public const string DeleteDanglingPARootsJobPath = "/Configuration/BlobStore/DeleteDanglingPARootsJobPath";
    public const string ReferenceAuditJobPath = "/Configuration/BlobStore/ReferenceAuditJob";
    public const string SoftDeleteRetentionAccountingJobPath = "/Configuration/BlobStore/SoftDeleteRetentionAccountingJob";
    public const string DeleteRetentionConfigurationJobPath = "/Configuration/BlobStore/DeleteRetentionConfigurationJob";
    public static readonly string BillingConfigRootPath = "/Configuration/BlobStore/BillingConfig";
    public static readonly string GetIndividualOrphanedFeedsThresholdPath = ServiceRegistryConstants.BillingConfigRootPath + "/GetIndividualOrphanedFeedsThreshold";
    public const int DefaultGetIndividualOrphanedFeedsThresholdValue = 10;
    public const string BlobDeletionBlobIdKey = "BlobId";
    public static readonly string BlobDeletionCheckpointPath = "/Configuration/BlobStore/BlobDeletionJob/Checkpoint";
    public const string EnabledStateKey = "EnabledState";
    public static readonly string EnabledStatePath = "/Configuration/BlobStore/BlobDeletionJob/EnabledState";
    private const string DedupeGCDeletionStageKey = "DedupeGCDeletionStageEnabled";
    public static readonly string DedupeGCDeletionStagePath = "/Configuration/BlobStore/DedupeGCDeletionStageEnabled";
    private const string MaxRequestContentLengthKey = "MaxRequestContentLength";
    public static readonly string MaxRequestContentLengthPath = "/Configuration/BlobStore/MaxRequestContentLength";
    internal const string EdgeCachingRegistryRootPath = "/Configuration/BlobStore/AzureFrontDoor";
    private const string DRCleanupParallelismPerAccountKey = "DRCleanupParallelismPerAccount";
    public static readonly string DRCleanupParallelismPerAccountPath = "/Configuration/BlobStore/DRCleanupParallelismPerAccount";
    private const string StorageLogStatsJobFilterStringKey = "StorageLogStatsJobFilterString";
    public static readonly string StorageLogsStatsJobFilterStringPath = "/Configuration/BlobStore/StorageLogStatsJobFilterString";
    private const string StorageLogStatsJobStartTimeKey = "StorageLogStatsJobStartTime";
    public static readonly string StorageLogsStatsJobStartTimePath = "/Configuration/BlobStore/StorageLogStatsJobStartTime";
    private const string StorageLogStatsJobEndTimeKey = "StorageLogStatsJobEndTime";
    public static readonly string StorageLogsStatsJobEndTimePath = "/Configuration/BlobStore/StorageLogStatsJobEndTime";
    private const string StorageStatsJobShard = "StorageLogStatsJobShard";
    public static readonly string StorageLogStatsJobShard = "/Configuration/BlobStore/StorageLogStatsJobShard";
    private const string StorageLogStatsJobMode = "StorageLogStatsJobMode";
    public static readonly string StorageLogStatsJobModePath = "/Configuration/BlobStore/StorageLogStatsJobMode";
    private const string StorageLogExportLocation = "StorageLogExportLocation";
    public static readonly string StorageLogExportLocationPath = "/Configuration/BlobStore/StorageLogExportLocation";
    private const string ChunkDedupGarbageCollectionJobCheckpointKey = "ChunkDedupGarbageCollectionJobCheckpoint";
    public static readonly string ChunkDedupGCMaxConcurrentJobs = "/Configuration/BlobStore/ChunkDedupGCMaxConcurrentJobs";
    public static readonly string ChunkDedupGCJobDelayInSeconds = "/Configuration/BlobStore/ChunkDedupGCJobDelayInSeconds";
    public static readonly string ChunkDedupGarbageCollectionJobCheckpointFormat = "/Configuration/BlobStore/ChunkDedupGarbageCollectionJobCheckpoint/{0}";
    private const string ChunkDedupGarbageCollectionLauncherJobPastJobsKey = "ChunkDedupGarbageCollectionLauncherJobPastJobs";
    private const string ChunkDedupGarbageCollectionPastDomainJobIdKey = "ChunkDedupGarbageCollectionPastDomainJobId";
    public static readonly string ChunkDedupGarbageCollectionPastDomainJobIdFormat = "/Configuration/BlobStore/ChunkDedupGarbageCollectionPastDomainJobId/{0}";
    public static readonly string ChunkDedupGarbageCollectionLauncherJobPastJobsPath = "/Configuration/BlobStore/ChunkDedupGarbageCollectionLauncherJobPastJobs";
    private const string ChunkDedupGarbageCollectionLauncherJobWaitPeriodBetweenSuccessfulChildJobsKey = "ChunkDedupGarbageCollectionLauncherJobWaitPeriodBetweenSuccessfulChildJobs";
    public static readonly string ChunkDedupGarbageCollectionLauncherJobWaitPeriodBetweenSuccessfulChildJobs = "/Configuration/BlobStore/ChunkDedupGarbageCollectionLauncherJobWaitPeriodBetweenSuccessfulChildJobs";
    private const string ChunkDedupGarbageCollectionParallelismKey = "ChunkDedupGarbageCollectionParallelism";
    public static readonly string ChunkDedupGarbageCollectionParallelismPath = "/Configuration/BlobStore/ChunkDedupGarbageCollectionParallelism";
    public const int DefaultChunkDedupGarbageCollectionParallelism = 10;
    private const string ChunkDedupGarbageCollectionCheckpointTimerMillisecondsKey = "ChunkDedupGarbageCollectionCheckpointTimer";
    public static readonly string ChunkDedupGarbageCollectionCheckpointTimerMillisecondsPath = "/Configuration/BlobStore/ChunkDedupGarbageCollectionCheckpointTimer";
    public const long DefaultChunkDedupGarbageCollectionCheckpointTimerMilliseconds = 300000;
    public const string ChunkDedupValidationOnlyKey = "ChunkDedupValidationOnlyEnabled";
    public static readonly string ChunkDedupValidationOnlyPath = "/Configuration/BlobStore/ChunkDedupValidationOnlyEnabled";
    private const string ChunkDedupGarbageCollectionDeleteTelemetryBatchSizeKey = "ChunkDedupGarbageColletionDeleteTelemetryBatchSize";
    public static readonly string ChunkDedupGarbageCollectionDeleteTelemetryBatchSizePath = "/Configuration/BlobStore/ChunkDedupGarbageColletionDeleteTelemetryBatchSize";
    public const int DefaultChunkDedupGarbageCollectionDeleteTelemetryBatchSize = 500;
    public const string ChunkDedupExcludedDomainsKey = "ChunkDedupExcludedDomains";
    public static readonly string ChunkDedupExcludedDomainsPath = "/Configuration/BlobStore/ChunkDedupExcludedDomains";
    public const string ChunkDedupMinMarkingTime = "ChunkDedupMinMarkingTime";
    public static readonly string ChunkDedupMinMarkingTimePath = "/Configuration/BlobStore/ChunkDedupMinMarkingTime";
    public static readonly DateTimeOffset DefaultValidMinimumMarkingDate = new DateTimeOffset(new DateTime(2023, 11, 20));
    private const string HardDeleteRootsJobParallelismKey = "HardDeleteRootsJobParallelism";
    public static readonly string HardDeleteRootsJobParallelismPath = "/Configuration/BlobStore/HardDeleteRootsJob/HardDeleteRootsJobParallelism";
    public const int DefaultHardDeleteRootsJobParallelism = 5;
    public const string StorageMetricsTransactionPopulatorJobRootPath = "/Configuration/BlobStore/StorageMetricsTransactionPopulatorJob";
    public const string StorageMetricsTransactionPopulatorJobLastRunTimePath = "/Configuration/BlobStore/StorageMetricsTransactionPopulatorJob/LastRunTime";
    public const string ReferenceAuditCrawlStatus = "/Configuration/BlobStore/ReferenceAuditJob/CrawlStatus";
    private const string DedupProviderImplementationKey = "DedupProviderImplementation";
    internal const string DedupProviderImplementationRegistryPath = "/Configuration/BlobStore/DedupProviderImplementation";
    internal const string MemoryBlobTableDedupProvider = "MEMORYBLOBTABLEDEDUPPROVIDER";
    internal const string AzureBlobTableDedupProvider = "AZUREBLOBTABLEDEDUPPROVIDER";
    internal const string SqlTableDedupProvider = "SQLTABLEDEDUPPROVIDER";
    internal static readonly string[] DedupProviders = new string[3]
    {
      "AZUREBLOBTABLEDEDUPPROVIDER",
      "MEMORYBLOBTABLEDEDUPPROVIDER",
      "SQLTABLEDEDUPPROVIDER"
    };
    private const string DedupBloomFilterSizeKey = "DedupBloomFilterSize";
    internal const string DedupBloomFilterSizeRegistryPath = "/Configuration/BlobStore/DedupBloomFilterSize";
    internal const string DedupReceiptsEnabledRegistryPath = "/Configuration/BlobStore/DedupReceiptsEnabled";
    public static readonly string DedupProviderInitStatus = "/Configuration/BlobStore/DedupProviderImplementation/InitializationStatus";
    private const string HostDomainProviderImplementationKey = "HostDomainProviderImplementation";
    internal const string HostDomainProviderImplementationRegistryPath = "/Configuration/BlobStore/HostDomainProviderImplementation";
    internal const string HostDomainRegistryProvider = "REGISTRYHOSTDOMAINPROVIDER";
    public const string StitchingBoundedCapacityRegistryKey = "/Configuration/BlobStore/StitchingBoundedCapacity";
    public const string StitchingParallelismRegistryKey = "/Configuration/BlobStore/StitchingParallelism";
    public const string ConcurrentIteratorCapacity = "/Configuration/BlobStore/ConcurrentIteratorCapacity";
    public const string JobOutputRootPath = "/JobOutput/BlobStore";
    private const string ProjectDomainImplementationKey = "ProjectDomains";
    internal const string ProjectDomainImplementationRegistryPath = "/Configuration/BlobStore/ProjectDomains";
    public const string EventGridClientPath = "/Configuration/BlobStore/EventGridClient";
    public const string ClientSettingRegistryPath = "/Configuration/ClientSettings";
  }
}
