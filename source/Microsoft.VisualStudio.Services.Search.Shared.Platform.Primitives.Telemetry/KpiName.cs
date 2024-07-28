// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.KpiName
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public static class KpiName
  {
    public const string ESReportedQueryTime = "ESReportedQueryTime";
    public const string E2EPlatformQueryTime = "E2EPlatformQueryTime";
    public const string E2EPlatformCountRequestTime = "E2EPlatformCountRequestTime";
    public const string E2EQueryTime = "E2EQueryTime";
    public const string E2ETenantQueryTime = "E2ETenantQueryTime";
    public const string RegisterCustomTenantTime = "RegisterCustomTenantTime";
    public const string GetCustomTenantCollectionNamesTime = "GetCustomTenantCollectionNamesTime";
    public const string CustomBulkCodeIndexTime = "CustomBulkCodeIndexTime";
    public const string GetCustomProjectsTime = "GetCustomProjectsTime";
    public const string RegisterCustomRepositoryTime = "RegisterCustomRepositoryTime";
    public const string GetCustomRepositoryHealth = "GetCustomRepositoryHealth";
    public const string GetCustomRepositoriesTime = "GetCustomRepositoriesTime";
    public const string GetCustomRepositoryTime = "GetCustomRepositoryTime";
    public const string GetCustomFileContentTime = "GetCustomFileContentTime";
    public const string GetCustomFilesMetadataTime = "GetCustomFilesMetadataTime";
    public const string GetCustomFileContentNotFound = "GetCustomFileContentNotFound";
    public const string GetCustomFileNewContentFound = "GetCustomFileNewContentFound";
    public const string GetCustomOperationStatusTime = "GetCustomOperationStatusTime";
    public const string NoOfTotalFileHits = "NoOfTotalFileHits";
    public const string NoOfFileHitsForCountRequest = "NoOfFileHitsForCountRequest";
    public const string NoOfFileHitsReturned = "NoOfFileHitsReturned";
    public const string NoOfTotalHighlights = "NoOfTotalHighlights";
    public const string NoOfQueryRequests = "NoOfQueryRequests";
    public const string NoOfResultsCountRequests = "NoOfResultsCountRequests";
    public const string SearchServiceErrorCode = "SearchServiceErrorCode";
    public const string SearchPlatformErrorMessage = "SearchPlatformErrorMessage";
    public const string NoOfTenantQueryRequests = "NoOfTenantQueryRequests";
    public const string NoOfIndexRequests = "NoOfIndexRequests";
    public const string NoOfFailedIndexRequests = "NoOfFailedIndexRequests";
    public const string NoOfFilesInIndexRequests = "NoOfFilesInIndexRequests";
    public const string NoOfFailedQueryRequests = "NoOfFailedQueryRequests";
    public const string NoOfFailedTenantQueryRequests = "NoOfFailedTenantQueryRequests";
    public const string NoOfCodeSearchExtensionPreinstallCallbacks = "NoOfCodeSearchExtensionPreinstallCallbacks";
    public const string NoOfWorkItemSearchExtensionPreinstallCallbacks = "NoOfWorkItemSearchExtensionPreinstallCallbacks";
    public const string NoOfWorkItemSearchOnPremiseExtensionPreinstallCallbacks = "NoOfWorkItemSearchOnPremiseExtensionPreinstallCallbacks";
    public const string NoOfWikiSearchOnPremiseExtensionPreinstallCallbacks = "NoOfWikiSearchOnPremiseExtensionPreinstallCallbacks";
    public const string SearchExtensionInstallSubscriberInvoke = "SearchExtensionInstallSubscriberInvoke";
    public const string SearchExtensionUninstallSubscriberInvoke = "SearchExtensionUninstallSubscriberInvoke";
    public const string SearchExtensionInstallSubscriberFailure = "SearchExtensionInstallSubscriberFailure";
    public const string SearchExtensionUninstallSubscriberFailure = "SearchExtensionUninstallSubscriberFailure";
    public const string SearchExtensionAccountFaultInJobTrigger = "SearchExtensionAccountFaultInJobTrigger";
    public const string CrawlingTime = "CrawlingTime";
    public const string ParsingTime = "ParsingTime";
    public const string FeedingTime = "FeedingTime";
    public const string FeedThreadLockWaitingTime = "FeedThreadLockWaitingTime";
    public const string StoringTime = "StoringTime";
    public const string NumOfFilesStored = "NumOfFilesStored";
    public const string NumOfFilesFailedToStore = "NumOfFilesFailedToStore";
    public const string NumOfFilesDeletedFromStore = "NumOfFilesDeletedFromStore";
    public const string NumOfFilesFailedToDeleteFromStore = "NumOfFilesFailedToDeleteFromStore";
    public const string NoOfItemsCrawled = "NoOfItemsCrawled";
    public const string NoOfTreeItemsCrawled = "NoOfTreeItemsCrawled";
    public const string TreeCrawlingTime = "TreeCrawlingTime";
    public const string ProjectAdminUpdateCacheCount = "ProjectAdminUpdateCacheCount";
    public const string NoOfFilesParsed = "NoOfFilesParsed";
    public const string NoOfFilesFailedToParsed = "NoOfFilesFailedToParsed";
    public const string NoOfFilesCrashingTextParser = "NoOfFilesCrashingTextParser";
    public const string NoOfCSharpFilesParsed = "NoOfCSharpFilesParsed";
    public const string NoOfCppFilesParsed = "NoOfCppFilesParsed";
    public const string NoOfVBFilesParsed = "NoOfVBFilesParsed";
    public const string NoOfJavaFilesParsed = "NoOfJavaFilesParsed";
    public const string NoOfTextFilesParsed = "NoOfTextFilesParsed";
    public const string NoOfMarkDownFilesParsed = "NoOfMarkDownFilesParsed";
    public const string NoOfMarkDownFilesFallenBackToRaw = "NoOfMarkDownFilesFailedParsing";
    public const string NoOfCSharpFilesParsedAsText = "NoOfCSharpFilesParsedAsText";
    public const string NoOfCppFilesParsedAsText = "NoOfCppFilesParsedAsText";
    public const string NoOfVBFilesParsedAsText = "NoOfVBFilesParsedAsText";
    public const string NoOfJavaFilesParsedAsText = "NoOfJavaFilesParsedAsText";
    public const string TotalNumberOfItemsFailedToIndex = "TotalNumberOfItemsFailedToIndex";
    public const string TotalNumberOfFailedItemsReIndexedSuccessfully = "TotalNumberOfFailedItemsReIndexedSuccessfully";
    public const string TotalNumberOfFailedItemsExceedingRetryLimit = "TotalNumberOfFailedItemsExceedingRetryLimit";
    public const string AzureAddRepositoryOperationTime = "AzureAddRepositoryOperationTime";
    public const string AzureDeleteRepositoryOperationTime = "AzureDeleteRepositoryOperationTime";
    public const string GetScopeTime = "GetScopeTime";
    public const string AzureUpdateRepositoryOperationTime = "AzureUpdateRepositoryOperationTime";
    public const string AzureRetrieveRepositoryTableEntityOperationTime = "AzureRetrieveRepositoryTableEntityOperationTime";
    public const string AzureRetrieveRepositoryTableEntityBatchOperationTime = "AzureRetrieveRepositoryTableEntityBatchOperationTime";
    public const string TFSGetRepositoriesCall = "TFSGetRepositoriesCall";
    public const string TFSGetForksAsyncCall = "TFSGetForksAsyncCall";
    public const string GetWikiRepositoriesCall = "GetWikiRepositoriesCall";
    public const string GetWikisCall = "GetWikisCall";
    public const string GitGitItemsBatchCall = "GitGitItemsBatchCall";
    public const string QueueJobCallTime = "QueueJobCallTime";
    public const string UpdateJobDefinitionsCallTime = "UpdateJobDefinitionsCallTime";
    public const string GitCloneCallTime = "GitCloneCallTime";
    public const string TotalItemsCrawled = "TotalItemsCrawled";
    public const string SizeOfSupportedCrawledItems = "SizeOfSupportedCrawledItems";
    public const string NumberOfSkippedUnSupportedFiles = "NumberOfSkippedUnSupportedFiles";
    public const string NumberOfSkippedFilesWithUnsupportedExtension = "NumberOfSkippedFilesWithUnsupportedExtension";
    public const string NumberOfFilesWithUnexpectedUpdateType = "NumberOfFilesWithUnexpectedUpdateType";
    public const string NumberOfFilesWithLFSContent = "NumberOfFilesWithLFSContent";
    public const string NumberOfFilesSkippedDueToUnSupportedFileExtensionForIndexing = "NumberOfFilesSkippedDueToUnSupportedFileExtensionForIndexing";
    public const string NumberOfFilesSkippedDueToSizeLimit = "NumberOfFilesSkippedDueToSizeLimit";
    public const string NumberOfSkippedFilesDueToExcludedFolder = "NumberOfSkippedFilesDueToExcludedFolder";
    public const string NumberOfUniqueFilesCrawled = "NumberOfUniqueFilesCrawled";
    public const string NumberOfFilesCrawledForData = "NumberOfFilesCrawledForData";
    public const string NumberOfFilesHavingBinaryContentForSupportedExtensions = "NumberOfFilesHavingBinaryContentForSupportedExtensions";
    public const string NumberOfSupportedFileExtensionsHavingBinaryContent = "NumberOfSupportedFileExtensionsHavingBinaryContent";
    public const string NumberOfItemsProcessedInCrawling = "NumberOfItemsProcessedInCrawling";
    public const string NumberOfTempStoreRecords = "NumberOfTempStoreRecords";
    public const string NumberOfFilesFailedToCrawl = "NumberOfFilesFailedToCrawl";
    public const string NumberOfProjectsFailedToCrawl = "NumberOfProjectsFailedToCrawl";
    public const string NumberOfProjectsCrawledForData = "NumberOfProjectsCrawledForData";
    public const string PercentageOfProjectsFailedToCrawl = "PercentageOfProjectsFailedToCrawl";
    public const string SlotsAvailableToCreateSharedProject = "SlotsAvailableToCreateSharedProject";
    public const string ExceptionInGetCollectionToCreateSharedProjectAPI = "ExceptionInGetCollectionToCreateSharedProjectAPI";
    public const string EmptyCollectionFromGetCollectionToCreateSharedProjectAPI = "EmptyCollectionFromGetCollectionToCreateSharedProjectAPI";
    public const string E2EQueryTimeGetCollectionToCreateSharedProjectAPI = "E2EQueryTimeGetCollectionToCreateSharedProjectAPI";
    public const string MaxRetriedExhaustedForCollectionFaultIn = "MaxRetriedExhaustedForCollectionFaultIn";
    public const string TimeInMilliSecondsForCrawlingMetaDataFromTreeCrawler = "TimeInMilliSecondsForCrawlingMetaDataFromTreeCrawler";
    public const string TimeInMilliSecondsForDiffFromMetadataStore = "TimeInMilliSecondsForDiffFromMetadataStore";
    public const string TimeInMilliSecondsForAddMetadataToTempStore = "TimeInMilliSecondsForAddMetadataToTempStore";
    public const string TimeInMilliSecondsForUpdateMetadataToTempStore = "TimeInMilliSecondsForUpdateMetadataToTempStore";
    public const string NumberOfCrawledDocsExistingInMetadataStore = "NumberOfCrawledDocsExistingInMetadataStore";
    public const string TotalNumberOfTempRecords = "TotalNumberOfTempRecords";
    public const string ItemsProcessedInOneRun = "ItemsProcessedInOneRun";
    public const string NumberOfCrawledDocs = "NumberOfCrawledDocs";
    public const string NumberOfCrawledDocsForAdd = "NumberOfCrawledDocsForAdd";
    public const string NumberOfCrawledDocsForEdit = "NumberOfCrawledDocsForEdit";
    public const string NumberOfCrawledDocsForDelete = "NumberOfCrawledDocsForDelete";
    public const string NumberOfTreeStoreItems = "NumberOfTreeStoreItems";
    public const string NumberOfTreeStoreItemsAtTreeCrawler = "NumberOfTreeStoreItemsAtTreeCrawler";
    public const string NumberOfFilesCrawledForMetaDataFromTFS = "NumberOfFilesCrawledForMetaDataFromTFS";
    public const string NumberOfSkippedUnSupportedFilesForMetaDataFromTFS = "NumberOfSkippedUnSupportedFilesForMetaDataFromTFS";
    public const string TimeInMilliSecondsForCrawlingMetaDataFromTFS = "TimeInMiliSecondsForCrawlingMetaDataFromTFS";
    public const string NumberOfFilesCrawledForDocMetaDataFromES = "NumberOfFilesCrawledForDocMetaDataFromES";
    public const string TimeInMilliSecondsForCrawlingForDocMetaDataFromES = "TimeInMiliSecondsForCrawlingForDocMetaDataFromES";
    public const string ESBulkIndexingTime = "ESBulkIndexingTime";
    public const string ESFailedBulkIndexingTime = "ESFailedBulkIndexingTime";
    public const string ESScriptUpdateByQueryTime = "ESScriptUpdateByQueryTime";
    public const string ESFailedScriptUpdateByQueryTime = "ESFailedScriptUpdateByQueryTime";
    public const string ESBulkScriptUpdateTime = "ESBulkScriptUpdateTime";
    public const string ESFailedBulkScriptUpdateTime = "ESFailedBulkScriptUpdateTime";
    public const string ESGetDocumentsTime = "ESGetDocumentsTime";
    public const string ESFailedGetDocumentsTime = "ESFailedGetDocumentsTime";
    public const string ESFailedBulkUpdateTime = "ESFailedBulkUpdateTime";
    public const string ESBulkDeleteByQueryTime = "ESBulkDeleteByQueryTime";
    public const string ESFailedBulkDeleteByQueryTime = "ESFailedBulkDeleteByQueryTime";
    public const string ESOptimizeIndexTime = "ESOptimizeIndexTime";
    public const string CrawlerServiceFailure = "CrawlerServiceFailure";
    public const string NoOfItemsAddedToMetaDataStore = "NoOfItemsAddedToMetaDataStore";
    public const string CrawlerServiceFailureReason = "CrawlerServiceFailureReason";
    public const string RunParserExecutionTime = "RunParserExecutionTime";
    public const string ParserServiceFailure = "ParserServiceFailure";
    public const string ParserServiceFailureReason = "ParserServiceFailureReason";
    public const string FeederServiceFailure = "FeederServiceFailure";
    public const string FeederServiceFailureReason = "FeederServiceFailureReason";
    public const string FeederServiceESFailureReason = "FeederServiceESFailureReason";
    public const string StoreServiceFailure = "StoreServiceFailure";
    public const string RepositoryGitPushNotification = "RepositoryGitPushNotification";
    public const string RepositorySecurityHashComputationTime = "RepositorySecurityHashComputationTime";
    public const string AreaSecurityHashComputationTime = "AreaSecurityHashComputationTime";
    public const string FeedSecurityHashComputationTime = "FeedSecurityHashComputationTime";
    public const string GetUserAccessibleRepositoriesTime = "GetUserAccessibleRepositoriesTime";
    public const string GetUserAccessibleWikiRepositoriesTime = "GetUserAccessibleWikiRepositoriesTime";
    public const string NumberOfUserAccessibleRepositories = "NumberOfUserAccessibleRepositories";
    public const string NumberOfUserAccessibleWikiRepositories = "NumberOfUserAccessibleWikiRepositories";
    public const string RepositorySecuritySetCreationTime = "RepositorySecuritySetCreationTime";
    public const string WikiRepositorySecuritySetCreationTime = "WikiRepositorySecuritySetCreationTime";
    public const string NumberOfRepositorySecuritySets = "NumberOfRepositorySecuritySets";
    public const string NumberOfWikiRepositorySecuritySets = "NumberOfWikiRepositorySecuritySets";
    public const string AreaSecuritySetCreationTime = "AreaSecuritySetCreationTime";
    public const string GetUserAccessibleAreasTime = "GetUserAccessibleAreasTime";
    public const string NumberOfUserAccessibleAreas = "NumberOfUserAccessibleAreas";
    public const string NumberOfAreaSecuritySets = "NumberOfAreaSecuritySets";
    public const string PackageSecuritySetCreationTime = "PackageSecuritySetCreationTime";
    public const string GetUserAccessiblePackageContainersTime = "GetUserAccessiblePackageContainersTime";
    public const string NumberOfPackageContainerSecuritySets = "NumberOfPackageContainerSecuritySets";
    public const string NumberOfUserAccessiblePackageContainers = "NumberOfUserAccessiblePackageContainers";
    public const string NumberOfFailuresDuringAccessChecks = "NumberOfFailuresDuringAccessChecks";
    public const string NumberOfFailuresDuringWikiRepoAccessChecks = "NumberOfFailuresDuringWikiRepoAccessChecks";
    public const string GitSecurityChecksTime = "GitSecurityChecksTime";
    public const string ProjectSecurityChecksTime = "ProjectSecurityChecksTime";
    public const string WitSecurityChecksTime = "WitSecurityChecksTime";
    public const string ReadAccessChecksTime = "ReadAccessChecksTime";
    public const string ProjectSearchE2ESecurityChecksTime = "ProjectSearchE2ESecurityChecksTime";
    public const string TfvcSecurityChecksTime = "TfvcSecurityChecksTime";
    public const string FacetsSecurityChecksTime = "FacetsSecurityChecksTime";
    public const string NoOfCodeSearchResultsAfterTrimming = "NoOfCodeSearchResultsAfterTrimming";
    public const string NoOfWikiSearchResults = "NoOfWikiSearchResults";
    public const string NoOfTenantWikiSearchResults = "NoOfTenantWikiSearchResults";
    public const string NoOfCodeSearchResultsBeforeTrimming = "NoOfCodeSearchResultsBeforeTrimming";
    public const string SecurityNamespaceLoadTime = "SecurityNamespaceLoadTime";
    public const string SecurityNamespaceNotLoaded = "SecurityNamespaceNotLoaded";
    public const string SecurityChecksCacheCleanupTime = "SecurityChecksCacheCleanupTime";
    public const string SecurityChecksCacheUserDictSize = "SecurityChecksCacheUserDictSize";
    public const string SecurityChecksCacheRepoHashDictSize = "SecurityChecksCacheRepoHashDictSize";
    public const string SecurityChecksCacheHit = "SecurityChecksCacheHit";
    public const string SecurityChecksCacheCleared = "SecurityChecksCacheCleared";
    public const string SecurityChecksGetRepoListHashTime = "SecurityChecksGetRepoListHashTime";
    public const string SecurityChecksUserDictCleanupCount = "SecurityChecksUserDictCleanupCount";
    public const string SecurityChecksRepoHashDictCleanupCount = "SecurityChecksRepoHashDictCleanupCount";
    public const string ProjectSecurityNamespaceNotLoaded = "ProjectSecurityNamespaceNotLoaded";
    public const string PartiallyOpenStateExpired = "PartiallyOpenStateExpired";
    public const string ClosedStateExpired = "ClosedStateExpired";
    public const string CritialPartiallyOpenStateTransition = "CritialPartiallyOpenStateTransition";
    public const string MediumPartiallyOpenStateTransition = "MediumPartiallyOpenStateTransition";
    public const string HealthyClosedStateTransition = "HealthyClosedStateTransition";
    public const string HealthyClosedForceStateTransition = "HealthyClosedForceStateTransition";
    public const string FaultManagementAsyncApiCancellationCount = "FaultManagementAsyncApiCancellationCount";
    public const string FaultManagementExponentialBackoffApiRetrySuccessCount = "FaultManagementExponentialBackoffRetrySuccessCount";
    public const string FaultManagementExponentialBackoffApiRetryFailureCount = "FaultManagementExponentialBackoffApiRetryFailureCount";
    public const string FaultManagementGenericApiRetrySuccessCount = "FaultManagementGenericApiRetrySuccessCount";
    public const string FaultManagementGenericApiRetryFailureCount = "FaultManagementGenericApiRetryFailureCount";
    public const string FaultManagementFeederBatchRetrySuccessCount = "FaultManagementFeederBatchRetrySuccessCount";
    public const string FaultManagementStateRecordError = "FaultManagementStateRecordError";
    public const string FaultManagementStateOpError = "FaultManagementStateOpError";
    public const string FaultManagementRequestRecordError = "FaultManagementRequestRecordError";
    public const string FaultManagementRequestOpError = "FaultManagementRequestOpError";
    public const string PeriodicCatchUpJobFailedAtGitMissedCommitSync = "PeriodicCatchUpJobFailedAtGitMissedCommitSync";
    public const string PeriodicCatchUpJobFailedAtMissingGitRepositoriesSync = "PeriodicCatchUpJobFailedAtMissingGitRepositoriesSync";
    public const string PeriodicCatchUpJobFailedAtGitDefaultBranchChangeOrDeletionSync = "PeriodicCatchUpJobFailedAtGitDefaultBranchChangeOrDeletionSync";
    public const string PeriodicCatchUpJobFailedAtGitSecurityHashSync = "PeriodicCatchUpJobFailedAtGitSecurityHashSync";
    public const string PeriodicCatchUpJobFailedAtTFVCMissedRepositoriesSync = "PeriodicCatchUpJobFailedAtTFVCMissedRepositoriesSync";
    public const string PeriodicCatchUpJobFailedAtHandleMissingProjectRenameNotifications = "PeriodicCatchUpJobFailedAtHandleMissingProjectRenameNotifications";
    public const string PeriodicCatchUpJobFailedAtTFVCMissedChangeSetsSync = "PeriodicCatchUpJobFailedAtTFVCMissedChangeSetsSync";
    public const string ReindexingStarted = "ReindexingStarted";
    public const string ReindexingPassed = "ReindexingPassed";
    public const string ReindexingFailed = "ReindexingFailed";
    public const string TotalNumberOfEvents = "TotalNumberOfEvents";
    public const string ThresholdViolation = "ThresholdViolation";
    public const string ItemLevelFailureThresholdViolation = "ItemLevelFailureThresholdViolation";
    public const string LongPendingChangeEventThresholdViolation = "LongPendingChangeEventThresholdViolation";
    public const string TotalNumberOfSucceededEvents = "TotalNumberOfSucceededEvents";
    public const string TotalNumberOfFailedEvents = "TotalNumberOfFailedEvents";
    public const string TotalNumberOfLongPendingEvents = "TotalNumberOfLongPendingEvents";
    public const string IndexingStuckForCollection = "IndexingStuckForCollection";
    public const string RepositoryBranchCacheBuildTime = "RepositoryBranchCacheBuildTime";
    public const string GitRepositoryBranchIndexInfoCacheBuildTime = "GitRepositoryBranchIndexInfoCacheBuildTime";
    public const string CustomRepositoryBranchIndexInfoCacheBuildTime = "CustomRepositoryBranchIndexInfoCacheBuildTime";
    public const string NoOfPackageSearchResults = "NoOfPackageSearchResults";
    public const string NoOfBoardSearchResults = "NoOfBoardSearchResults";
    public const string NonFuzzyE2EPlatformQueryTime = "NonFuzzyE2EPlatformQueryTime";
    public const string ProjectE2EPlatformSuggestQueryTime = "ProjectE2EPlatformSuggestQueryTime";
    public const string RepoE2EPlatformSuggestQueryTime = "RepoE2EPlatformSuggestQueryTime";
    public const string WikiE2EPlatformSuggestQueryTime = "WikiE2EPlatformSuggestQueryTime";
    public const string WikiE2EPlatformQueryTime = "WikiE2EPlatformQueryTime";
    public const string PackageE2EPlatformSuggestQueryTime = "PackageE2EPlatformSuggestQueryTime";
    public const string BoardE2EPlatformSuggestQueryTime = "BoardE2EPlatformSuggestQueryTime";
    public const string GetAccessibleProjectIdsTime = "GetAccessibleProjectIdsTime";
    public const string NumOfRequestedJobResources = "NumOfRequestedJobResources";
    public const string NumOfAllottedJobResources = "NumOfAllottedJobResources";
    public const string MaxResoucesAllottablePerCollection = "MaxResoucesAllottablePerCollection";
    public const string MaxResoucesAllottablePerCollectionForCode = "MaxResoucesAllottablePerCollectionForCode";
    public const string MaxResoucesAllottablePerCollectionForWorkItem = "MaxResoucesAllottablePerCollectionForWorkItem";
    public const string GetWorkItemTrackingFieldsTime = "GetWorkItemTrackingFieldsTime";
    public const string NoOfFailedWorkItemFieldsRequests = "NumOfFailedWorkItemFieldsRequests";
    public const string EntityType = "EntityType";
    public const string IsInstantSearch = "IsInstantSearch";
    public const string IndexingUnitType = "IndexingUnitType";
    public const string ChangeType = "ChangeType";
    public const string BranchName = "BranchName";
    public const string BaseCommitId = "BaseCommitId";
    public const string TargetCommitId = "TargetCommitId";
    public const string BulkIndexing = "BulkIndexing";
    public const string ContinuousIndexing = "ContinuousIndexing";
    public const string NoOfGetMetaDataCalls = "NoOfGetMetaDataCalls";
    public const string TotalNoOfMetaDataSeeds = "TotalNoOfMetaDataSeeds";
    public const string JobExecutionTime = "JobExecutionTime";
    public const string WorkItemCIServiceBusDelay = "WorkItemCIServiceBusDelay";
    public const string ESBulkUpdateByQueryTime = "ESBulkUpdateByQueryTime";
    public const string ESBulkGetByQueryTime = "ESBulkGetByQueryTime";
    public const string ESFailedBulkUpdateByQueryTime = "ESFailedBulkUpdateByQueryTime";
    public const string GetUserAccessibleProjectsTime = "GetUserAccessibleProjectsTime";
    public const string NumberOfUserAccessibleProjects = "NumberOfUserAccessibleProjects";
    public const string ProjectsListCreationTime = "ProjectsListCreationTime";
    public const string NumberOfProjects = "NumberOfProjects";
    public const string NumberOfCustomProjects = "NumberOfCustomProjects";
    public const string NumberOfCustomProjectsInSql = "NumberOfCustomProjectsInSql";
    public const string NumberOfHighlights = "NumberOfHighlights";
    public const string ItemLevelFailuresThresholdViolation = "ItemLevelFailuresThresholdViolation";
    public const string IndexingJobFailureRate = "IndexingJobFailureRate";
    public const string GetAreasWhereUserIsAdminTime = "GetAreasWhereUserIsAdminTime";
    public const string GetAreasWhereUserIsAdmin = "GetAreasWhereUserIsAdmin";
    public const string NumberOfWorkItemRevisionsCrawled = "NumberOfWorkItemRevisionsCrawled";
    public const string NumberOfWorkItemDiscussionsCrawled = "NumberOfWorkItemDiscussionsCrawled";
    public const string WikiSearchSecurityChecksTime = "WikiSearchSecurityChecksTime";
    public const string JobExecutionResultMessage = "JobExecutionResultMessage";
    public const string NumberOfDocumetsDeleted = "NumberOfDocumetsDeleted";
    public const string TimeTakenToRunCleanUpJob = "TimeTakenToRunCleanUpJob";
    public const string CleanUpJobTriggered = "CleanUpJobTriggered";
    public const string CleanUpJobFailed = "CleanUpJobFailed";
    public const string DocumentCountOnOldCluster = "DocumentCountOnOldCluster";
    public const string DocumentCountOnNewCluster = "DocumentCountOnNewCluster";
    public const string NumberOfCodeRepoBIOperationsQueued = "NumberOfCodeRepoBIOperationsQueued";
    public const string AverageTimeToIndexWorkItemsInSec = "AverageTimeToIndexWorkItemInSec";
    public const string ESDocsPendingScroll = "ESDocsPendingScroll";
    public const string TfvcRepositoryIndexInfoCacheBuildTime = "TfvcRepositoryIndexInfoCacheBuildTime";

    public static class IndexingUnit
    {
      public const string AddOrUpdateIndexingUnitOperationTime = "AddOrUpdateIndexingUnitOperationTime";
      public const string AddOrUpdateIndexingUnitBatchOperationTime = "AddOrUpdateIndexingUnitBatchOperationTime";
      public const string DeleteIndexingUnitBatchPermanentlyOperationTime = "DeleteIndexingUnitBatchPermanentlyOperationTime";
      public const string DeleteIndexingUnitOperationTime = "DeleteIndexingUnitOperationTime";
      public const string RetrieveIndexingUnitOperationTime = "RetrieveIndexingUnitOperationTime";
      public const string RetrieveIndexingUnitBatchOperationTime = "RetrieveIndexingUnitBatchOperationTime";
      public const string RetrieveDeletedEntityListOperationTime = "RetrieveDeletedEntityListOperationTime";
    }

    public static class IndexingUnitChangeEvent
    {
      public const string AddIndexingUnitChangeEventOperationTime = "AddIndexingUnitChangeEventOperationTime";
      public const string DeleteIndexingUnitChangeEventOperationTime = "DeleteIndexingUnitChangeEventOperationTime";
      public const string DeleteIndexingUnitChangeEventListOperationTime = "DeleteIndexingUnitChangeEventListOperationTime";
      public const string AddIndexingUnitChangeEventsBatchOperationTime = "AddIndexingUnitChangeEventsBatchOperationTime";
      public const string UpdateIndexingUnitChangeEventOperationTime = "UpdateIndexingUnitChangeEventOperationTime";
      public const string RetrieveIndexingUnitChangeEventOperationTime = "RetrieveIndexingUnitChangeEventOperationTime";
      public const string RetrieveIndexingUnitChangeEventBatchOperationTime = "RetrieveIndexingUnitChangeEventBatchOperationTime";
      public const string RetrieveIndexingUnitChangeEventsByStateOperationTime = "RetrieveIndexingUnitChangeEventsByStateOperationTime";
      public const string ArchiveIndexingUnitChangeEventOperationTime = "ArchiveIndexingUnitChangeEventOperationTime";
      public const string DeleteFromIndexingUnitChangeEventArchiveTime = "DeleteFromIndexingUnitChangeEventArchiveTime";
    }

    public static class ReindexingStatus
    {
      public const string AddOrUpdateReindexingStatusEntryOperationTime = "AddOrUpdateReindexingStatusEntryOperationTime";
      public const string GetReindexingStatusEntriesOperationTime = "GetReindexingStatusEntriesOperationTime";
      public const string GetReindexingStatusEntryOperationTime = "GetReindexingStatusEntryOperationTime";
      public const string DeleteTableEntityOperationTime = "DeleteTableEntityOperationTime";
    }

    public static class LockManager
    {
      public const string AcquireLocksOperationTime = "AcquireLocksOperationTime";
      public const string ReleaseLocksOperationTime = "ReleaseLocksOperationTime";
      public const string ReleaseLocksWithLeaseIdOperationTime = "ReleaseLocksWithLeaseIdOperationTime";
    }

    public static class WorkItemFieldCache
    {
      public const string ServiceStartUpTime = "ServiceStartTime";
      public const string CacheWriteLocked = "FieldCacheWriteLockedTime";
      public const string WorkItemFieldFetchTime = "TimeToWorkItemFieldsFetchedFromTfs";
      public const string CacheSyncronousUpdateTime = "CacheSyncronousUpdateTime";
    }

    public static class WorkItemRecencyDataProvider
    {
      public const string ServiceStartUpTime = "ServiceStartTime";
      public const string OverallFetchTime = "OverallFetchTime";
      public const string OverallUpdateTime = "OverallUpdateTime";
    }

    public static class WorkItemRecentActivityCache
    {
      public const string ServiceStartUpTime = "ServiceStartTime";
      public const string OverallFetchTime = "OverallFetchTime";
      public const string OverallUpdateTime = "OverallUpdateTime";
    }

    public static class RoutingIdCacheProvider
    {
      public const string IsScopedQuery = "IsScopedQuery";
      public const string IsScopingUsed = "IsScopingUsed";
      public const string CollectionScoped = "CollectionScoped";
      public const string MultipleProjectScoped = "MultipleProjectScoped";
      public const string ProjectScoped = "ProjectScoped";
      public const string RepositoryScoped = "RepositoryScoped";
      public const string PathScoped = "PathScoped";
      public const string MultipleRepositoryScoped = "MultipleRepositoryScoped";
      public const string ValidProjectFilterAbsent = "ValidProjectFilterAbsent";
      public const string ProjectRoutingUnitNotCached = "ProjectRoutingUnitNotCached";
      public const string ValidRepoFilterAbsent = "ValidRepoFilterAbsent";
      public const string RepoRoutingUnitNotCached = "RepoRoutingUnitNotCached";
      public const string RepoRoutingReturned = "RepoRoutingReturned";
      public const string LargeRepo = "LargeRepo";
      public const string ScopePathFilterAbsent = "ScopePathFilterAbsent";
      public const string ScopedPathCached = "ScopedPathCached";
      public const string ScopedPathUsed = "ScopedPathUsed";
    }

    public static class ProjectRepoKpi
    {
      public const string ProjectRepoOrganizationMigrationTimeInSec = "ProjectRepoOrganizationMigrationTimeInSec";
      public const string ProjectRepoHostShutDownWhileMigration = "ProjectRepoHostShutDownWhileMigration";
      public const string ProjectRepoJobMaintenanceJobTriggerFailed = "ProjectRepoJobMaintenanceJobTriggerFailed";
      public const string TotalProjectSearchCrawler = "TotalProjectSearchCrawler";
      public const string TotalCrawlOnlyRepository = "TotalCrawlOnlyRepository";
      public const string GetRepository = "GetRepository";
      public const string GetLanguageMetrics = "GetLanguageMetrics";
      public const string TotalCrawlOnlyProject = "TotalCrawlOnlyProject";
      public const string TotalCrawlAllProjects = "TotalCrawlAllProjects";
      public const string GetAllProjects = "GetAllProjects";
      public const string GetTeamProjectWithCapabilities = "GetTeamProjectWithCapabilities";
      public const string GetCodeActivities = "GetCodeActivities";
      public const string GetProjectTags = "GetProjectTags";
      public const string GetSocialEngagementActivities = "GetSocialEngagementActivities";
      public const string GetRepositoriesInProject = "GetRepositoriesInProject";
      public const string GetAllGitRepositoriesCodeActivities = "GetAllGitRepositoriesCodeActivities";
      public const string GetGitRawReadmeContentBlob = "GetGitRawReadmeContentBlob";
      public const string GetRepositoryCodeMetrics = "GetRepositoryCodeMetrics";
      public const string GetTfvcRawReadmeContentBlob = "GetTfvcRawReadmeContentBlob";
      public const string NoOfProjectsCrawled = "NoOfProjectsCrawled";
      public const string NoOfRepositoriesCrawled = "NoOfRepositoriesCrawled";
      public const string TimeToConsumeProjectCreateNotificationInMs = "TimeToConsumeProjectCreateNotificationInMs";
      public const string TimeToConsumeRepoCreateNotificationInMs = "TimeToConsumeRepoCreateNotificationInMs";
      public const string TimeToCrawlAndFeedProjectInMs = "TimeToCrawlAndFeedProjectInMs";
      public const string TimeToCrawlAndFeedRepositoryInMs = "TimeToCrawlAndFeedRepositoryInMs";
      public const string TimeToCrawlAndFeedAllProjectsInMs = "TimeToCrawlAndFeedAllProjectsInMs";
    }

    public static class PackageKpi
    {
      public const string PackageOrganizationMigrationTimeInSec = "PackageOrganizationMigrationTimeInSec";
      public const string PackageHostShutDownWhileMigration = "PackageHostShutDownWhileMigration";
      public const string FeedCIServiceBusDelay = "FeedCIServiceBusDelay";
    }

    public static class DataProviderKpi
    {
      public const string NoOfAdvancedCodeQueryRequestsFromDataProvider = "NoOfAdvancedCodeQueryRequestsFromDataProvider";
    }

    public static class WikiKpi
    {
      public const string WikiHostShutDownWhileMigration = "WikiHostShutDownWhileMigration";
      public const string WikiOrganizationMigrationTimeInSec = "WikiOrganizationMigrationTimeInSec";
    }

    public static class HealthManager
    {
      public const string TimeInMilliSecondsForAnalysisPhaseCompletion = "TimeInMilliSecondsForAnalysisPhaseCompletion";
      public const string TimeInMilliSecondsForActionPhaseCompletion = "TimeInMilliSecondsForActionPhaseCompletion";
    }
  }
}
