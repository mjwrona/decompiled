// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.CIName
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public static class CIName
  {
    public const string JobName = "JobName";
    public const string AttemptCount = "AttemptCount";
    public const string OperationStatus = "OperationStatus";
    public const string RunCriteriaStatus = "RunCriteriaStatus";
    public const string CollectionId = "CollectionId";
    public const string OperationCorrelationId = "OperationCorrelationId";
    public const string FinalizeIndex = "FinalizeIndex";
    public const string SupportedFileExtensionsHavingBinaryContent = "SupportedFileExtensionsHavingBinaryContent";
    public const string TimeInMiliSecondsForSortingDataForTFSDocMetaDataStore = "TimeInMiliSecondsForSortingDataForTFSDocMetaDataStore";
    public const string TimeInMiliSecondsForAddingGitMetadataToLocalStoreForTFSDocMetaDataStore = "TimeInMiliSecondsForAddingGitMetadataToLocalStoreForTFSDocMetaDataStore";
    public const string TimeInMiliSecondsForAddingTfvcMetadataToLocalStoreForTFSDocMetaDataStore = "TimeInMiliSecondsForAddingTfvcMetadataToLocalStoreForTFSDocMetaDataStore";
    public const string TimeInMiliSecondsForSortingDataForSQLDocMetadataStore = "TimeInMiliSecondsForSortingDataForSQLDocMetadataStore";
    public const string TimeInMiliSecondsForAddingMetadataToLocalStoreForSQLDocMetadataStore = "TimeInMiliSecondsForAddingMetadataToLocalStoreForSQLDocMetadataStore";
    public const string TimeInMiliSecondsForSortingDataForESDocMetaDataStore = "TimeInMiliSecondsForSortingDataForESDocMetaDataStore";
    public const string TimeInMiliSecondsForGitAddingMetadataToLocalStoreForESDocMetaDataStore = "TimeInMiliSecondsForGitAddingMetadataToLocalStoreForESDocMetaDataStore";
    public const string TimeInMiliSecondsForAddingTfvcMetadataToLocalStoreForESDocMetaDataStore = "TimeInMiliSecondsForAddingTfvcMetadataToLocalStoreForESDocMetaDataStore";
    public const string TimeInMiliSecondsForSortingDataForTFSMultiBranchDocMetaDataStore = "TimeInMiliSecondsForSortingDataForTFSMultiBranchDocMetaDataStore";
    public const string TimeInMiliSecondsForAddingMultiBranchDocMetadataToLocalStoreForTFS = "TimeInMiliSecondsForAddingMultiBranchDocMetadataToLocalStoreForTFS";
    public const string TimeInMiliSecondsForSortingMultiBranchDocMetadataForTFS = "TimeInMiliSecondsForSortingMultiBranchDocMetadataForTFS";
    public const string TimeInMiliSecondsForAddingMultiBranchDocMetadataToLocalStoreForES = "TimeInMiliSecondsForAddingMultiBranchDocMetadataToLocalStoreForES";
    public const string TimeInMiliSecondsForSortingMultiBranchDocMetadataForES = "TimeInMiliSecondsForSortingMultiBranchDocMetadataForES";
    public const string ItemLevelFailurePerEntityPerIndexingUnit = "ItemLevelFailurePerEntityPerIndexingUnit";
    public const string GitBranchesForBulkIndexing = "GitBranchesForBulkIndexing";
    public const string GitBranchesForContinuousIndexing = "GitBranchesForContinuousIndexing";
    public const string SkipDocs = "SkipDocs";
    public const string NoOfFilesAdded = "NoOfFilesAdded";
    public const string NoOfFilesEdited = "NoOfFilesEdited";
    public const string NoOfFilesDeleted = "NoOfFilesDeleted";
    public const string NoOfFilesForMetadataUpdate = "NoOfFilesForMetadataUpdate";
    public const string NoOfItemsAcceptedByES = "NoOfItemsAcceptedByES";
    public const string NoOfItemsFedToES = "NoOfItemsFedToES";
    public const string NoOfItemsFailed = "NoOfItemsFailed";
    public const string TimeTakenToFeedABatch = "TimeTakenToFeedABatch";
    public const string FirstUnprocessedPushId = "FirstUnprocessedPushId";
    public const string FirstUnprocessedPushTime = "FirstUnprocessedPushTime";
    public const string LatestPushId = "LatestPushId";
    public const string LatestPushTime = "LatestPushTime";
    public const string LastProcessedPushId = "LastProcessedPushId";
    public const string LastProcessedPushTime = "LastProcessedPushTime";
    public const string CIProcessingDelayInMiliseconds = "CIProcessingDelayInMiliseconds";
    public const string ReferenceTimeForStalenessMeasurement = "ReferenceTimeForStalenessMeasurement";
    public const string E2ETimeInSeconds = "E2ETimeInSeconds";
    public const string JobYieldCount = "JobYieldCount";
    public const string ItemsProcessedAcrossJobYields = "ItemsProcessedAcrossJobYields";
    public const string TotalSize = "TotalSize";
    public const string TotalSizeWithOriginalContent = "TotalSizeWithOriginalContent";
    public const string ProjectsAffectedInCheckinNotification = "ProjectsAffectedInCheckinNotification";
    public const string RepoIndexingUnitIdAffectedInDestroyNotification = "RepoIndexingUnitIdAffectedInDestroyNotification";
    public const string DestroyPath = "DestroyPath";
    public const string DestroyedPathsCount = "DestroyedPathsCount";
    public const string Resource = "Resource";
    public const string Publisher = "Publisher";
    public const string CommitIdRecievedInCheckinNotification = "CommitIdRecievedInCheckinNotification";
    public const string ChangeEventIdCreatedOnCheckinNotification = "ChangeEventIdCreatedOnCheckinNotification";
    public const string ChangeEventIdCreatedOnDestroyNotification = "ChangeEventIdCreatedOnDestroyNotification";
    public const string ChangeEventCreatedTimeUtc = "ChangeEventCreatedTimeUtc";
    public const string ChangeEventActionTakenByProcessor = "ChangeEventActionTakenByProcessor";
    public const string ChangeEventWaitTimeInSec = "ChangeEventWaitTimeInSec";
    public const string DelayAddedWhileQueuingJobInSec = "DelayAddedWhileQueuingJobInSec";
    public const string ChangeEventCorrelationId = "ChangeEventCorrelationId";
    public const string ChangeEventId = "ChangeEventId";
    public const string IsLastFeedRequestByIndexer = "IsLastFeedRequestByIndexer";
    public const string FeatureFlagOffNotificationCount = "FeatureFlagOffNotificationCount";
    public const string FailureCountOnCheckInNotification = "FailureCountOnCheckInNotification";
    public const string IndexingUnitId = "IndexingUnitId";
    public const string JobTrigger = "JobTrigger";
    public const string JobTriggerTime = "JobTriggerTime";
    public const string TimeDifferenceFromTriggerTimeInSec = "TimeDifferenceFromTriggerTimeInSec";
    public const string IndexingUnitNeedsFinalization = "IndexingUnitNeedsFinalization";
    public const string SerializedRequestSizeInNest = "SerializedRequestSizeInNest";
    public const string TotalTfvcBranchesInProject = "TotalTfvcBranchesInProject";
    public const string TotalTfvcBranchesGroupedViaFolderPath = "TotalTfvcBranchesGroupedViaFolderPath";
    public const string TotalTfvcBranchesRemovedDueToExcludedFolderPaths = "TotalTfvcBranchesRemovedDueToExcludedFolderPaths";
    public const string TotalGroupsRemovedWithSingleTfvcBranch = "TotalGroupsRemovedWithSingleTfvcBranch";
    public const string TfvcBranchGroupSplitCountDueToUpperCapOnParallelBranchesToCrawl = "TfvcBranchGroupSplitCountDueToUpperCapOnParallelBranchesToCrawl";
    public const string TfvcBranchGroupsThatExceededUpperCapOnParallelBranchesToCrawl = "TfvcBranchGroupsThatExceededUpperCapOnParallelBranchesToCrawl";
    public const string MaxPotentialTfvcBranchCountInGroup = "MaxPotentialTfvcBranchCountInGroup";
    public const string TotalTfvcBranchGroups = "TotalTfvcBranchGroups";
    public const string TfvcBranchGroupIdToBranchesCount = "TfvcBranchGroupIdToBranchesCount";
    public const string TfvcCurrentBranchGroupInProgress = "TfvcCurrentBranchGroupInProgress";
    public const string TfvcCurrentCrawlType = "TfvcCurrentCrawlType";
    public const string TfvcPreviousBranchGroupInProgress = "TfvcPreviousBranchGroupInProgress";
    public const string TfvcPreviousCrawlType = "TfvcPreviousCrawlType";
    public const string TfvcTotalBranchCountInPreviousGroup = "TfvcTotalBranchCountInPreviousGroup";
    public const string TfvcBranchCountResumedFromPreviousGroup = "TfvcBranchCountResumedFromPreviousGroup";
    public const string TimeTakenInSecByTfvcGetBranches = "TimeTakenInSecByTfvcGetBranches";
    public const string BaseCommitIdInYieldData = "BaseCommitIdInYieldData";
    public const string IntermediateCommitIdInYieldData = "IntermediateCommitIdInYieldData";
    public const string TargetCommitIdInYieldData = "TargetCommitIdInYieldData";
    public const string ShardAllocationCurrentEstimatedSize = "ShardAllocationCurrentEstimatedSize";
    public const string ShardAllocationReservedEstimatedSize = "ShardAllocationReservedEstimatedSize";
    public const string ShardAllocationTotalEstimatedSize = "ShardAllocationTotalEstimatedSize";
    public const string ShardAllocationTotalIndexingUnits = "ShardAllocationTotalIndexingUnits";
    public const string ShardAllocationTotalShardsAllocated = "ShardAllocationTotalShardsAllocated";
    public const string ShardAllocationTotalTimeTakenInMillis = "ShardAllocationTotalTimeTakenInMillis";
    public const string ShardAllocationExistingShardAllocatedDuringCI = "ShardAllocationExistingShardAllocatedDuringCI";
    public const string ShardAllocationNumberOfMaxShardsAllottedLimitMet = "ShardAllocationNumberOfMaxShardsAllottedLimitMet";
    public const string NumberOfSplitIndexingUnitsForShardAllocation = "NumberOfSplitIndexingUnitsForShardAllocation";
    public const string TimeTakenInIndexAssignmentInMillis = "TimeTakenInIndexAssignmentInMillis";
    public const string TimeTakenInShardsAssignmentInMillis = "TimeTakenInShardsAssignmentInMillis";
    public const string NumberOfActiveIndices = "NumberOfActiveIndices";
    public const string NewIndexCreated = "NewIndexCreated";
    public const string IndexMarkedInactive = "IndexMarkedInactive";
    public const string TotalTimeTakenForRoutingAssignmentInSeconds = "TotalTimeTakenForRoutingAssignmentInSeconds";
    public const string TotalAttemptsForRoutingAssignment = "TotalAttemptsForRoutingAssignment";
    public const string FallbackSizeEstimationsUsedForShardAllocation = "FallbackSizeEstimationsUsedForShardAllocation";
    public const string TotalIndexingUnitsForRoutingAssignment = "TotalIndexingUnitsForRoutingAssignment";
    public const string TotalIndexingUnitsWithSuccessfulRoutingAssignment = "TotalIndexingUnitsWithSuccessfulRoutingAssignment";
    public const string ShardAllocationComplete = "ShardAllocationComplete";
    public const string TfvcGetStatisticsCallTimedOut = "TfvcGetStatisticsCallTimedOut";
    public const string NumberOfRecordsMergedByMultiBranchDocMetadataStoreForTFS = "NumberOfRecordsMergedByMultiBranchDocMetadataStoreForTFS";
    public const string NumberOfRecordsMergedByMultiBranchDocMetadataStoreForES = "NumberOfRecordsMergedByMultiBranchDocMetadataStoreForES";
    public const string NumberOfPatchDescriptionsCrawledForAdd = "NumberOfPatchDescriptionsCrawledForAdd";
    public const string NumberOfPatchDescriptionsCrawledForEdit = "NumberOfPatchDescriptionsCrawledForEdit";
    public const string NumberOfPatchDescriptionsCrawledForDelete = "NumberOfPatchDescriptionsCrawledForDelete";
    public const string NumberOfTempRecordsCreatedFromPatchDescriptions = "NumberOfTempRecordsCreatedFromPatchDescriptions";
    public const string MinAttemptCountToPatchFailedFiles = "MinAttemptCountToPatchFailedFiles";
    public const string MaxAttemptCountToPatchFailedFiles = "MaxAttemptCountToPatchFailedFiles";
    public const string PercentageOfFailedFilesPatchedWithMinAttemptCount = "PercentageOfFailedFilesPatchedWithMinAttemptCount";
    public const string PercentageOfFailedFilesPatchedWithMaxAttemptCount = "PercentageOfFailedFilesPatchedWithMaxAttemptCount";
    public const string MismatchedCollectionQueryIndexName = "MismatchedCollectionQueryIndexName";
    public const string MismatchedIndexNameWithParent = "MismatchedIndexNameWithParent";
    public const string IndexUnitIdWithNoRoutingIds = "IndexUnitIdWithNoRoutingIds";
    public const string RoutingIdsNotPresentInChildren = "RoutingIdsNotPresentInChildren";
    public const string RoutingIdsNotPresentInParent = "RoutingIdsNotPresentInParent";
    public const string BulkIndexThresholdViolations = "BulkIndexThresholdViolations";
    public const string UpdateIndexThresholdViolations = "UpdateIndexThresholdViolations";
    public const string IndexName = "IndexName";
    public const string IndexVersion = "IndexVersion";
    public const string ESConnectionString = "ESConnectionString";

    public static class ChangeEventProcessorAction
    {
      public const int Queued = 0;
      public const int NotQueued_LockAcquireFailure = 1;
      public const int NotQueued_ChangeEventWithLeaseIdAlreadyPresent = 2;
      public const int NotQueued_ResourcesUnavailable = 3;
      public const int NotQueued_PrerequisiteNotMet = 4;
    }
  }
}
