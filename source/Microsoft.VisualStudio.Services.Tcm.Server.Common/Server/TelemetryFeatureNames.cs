// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TelemetryFeatureNames
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TelemetryFeatureNames
  {
    public const string SyncSuiteFeature = "SyncSuites";
    public const string RepopulateSuiteFeature = "RepopulateSuites";
    public const string TestRunFeature = "TestRun";
    public const string TestExtensibilityFeature = "TestExtensibility";
    public const string ResultsRetentionSettings = "ResultsRetentionSettings";
    public const string DownloadTestRunAttachment_REST = "DownloadTestRunAttachment_REST";
    public const string DownloadTestResultAttachment_REST = "DownloadTestResultAttachment_REST";
    public const string DownloadTestIterationAttachment_REST = "DownloadTestIterationAttachment_REST";
    public const string ShowTestPlanAdvertisement = "ShowTestPlanAdvertisement ";
    public const string TestResultTypeFeature = "TestResultType";
    public const string TestRunCompletedFeature = "TestRunCompleted";
    public const string PublishTestResults = "PublishTestResults";
    public const string FlakyTestCase = "FlakyTestCase";
    public const string GetResultByIdOldJson = "GetResultByIdOldJson";
    public const string TestRunDetails = "TestRunDetails";
    public const string TestRunDetailsOM = "TestRunDetailsOM";
    public const string TestRunDetailsService = "TestRunDetailsService";
    public const string CleanupJob = "CleanupJob";
    public const string TestCaseRefCleanupJob = "TestCaseRefCleanupJob";
    public const string SignalR = "SignalR";
    public const string RetainedResults = "RetainedResults";
    public const string NewTestCaseReferences = "NewTestCaseReferences";
    public const string HardDeletedReleases = "HardDeletedReleases";
    public const string CalculateSummaryAndInsights = "CalculateSummaryAndInsights";
    public const string CoverageBuildEventListener = "ModuleCoverageBuildEventListener";
    public const string CoverageStatusCheckPublisher = "CoverageStatusCheckPublisher";
    public const string ModuleCoverageMergeJob = "ModuleCoverageMergeJob";
    public const string ModuleCoverageMonitorJob = "ModuleCoverageMonitorJob";
    public const string SourceViewGeneratorJob = "SourceViewGeneratorJob";
    public const string VstestDotCoverageFileOperator = "VstestDotCoverageFileOperator";
    public const string ModuleMergeJobInvokerJob = "ModuleMergeJobInvokerJob";
    public const string VstestCoverageMetadataUpdater = "VstestCoverageMetadataUpdater";
    public const string FileCoverageEvaluationJob = "FileCoverageEvaluationJob";
    public const string PipelineScopeLevelFileCoverageAggregationJob = "PipelineScopeLevelFileCoverageAggregationJob";
    public const string PublishCoverageCheckStatusJob = "PublishCoverageCheckStatusJob";
    public const string FileCoverageProvider = "FileCoverageProvider";
    public const string FileCoverageUploader = "FileCoverageUploader";
    public const string FileDiffCoverageUploader = "FileDiffCoverageUploader";
    public const string AzureRepoPRMetricsPublisher = "AzureRepoPRMetricsPublisher";
    public const string GetCoverageScopes = "GetCoverageScopes";
    public const string GetFileCoverageDetails = "GetFileCoverageDetails";
    public const string UploadFileCoverageChangeSummary = "UploadFileCoverageChangeSummary";
    public const string UploadCoverageScopes = "UploadCoverageScopes";
    public const string GetPipelineCoverageSummary = "GetPipelineCoverageSummary";
    public const string UpdatePipelineCoverageSummary = "UpdatePipelineCoverageSummary";
    public const string UploadFileCoverageSummaryList = "UploadFileCoverageSummaryList";
    public const string PipelineCoverageService = "PipelineCoverageService";
    public const string UploadFileCoverageDetails = "UploadFileCoverageDetails";
    public const string UploadFileCoverageDetailsIndex = "UploadFileCoverageDetailsIndex";
    public const string GetDirectoryCoverageSummaryStreamAsync = "GetDirectoryCoverageSummaryStreamAsync";
    public const string UploadDirectoryCoverageSummary = "UploadDirectoryCoverageSummary";
    public const string UploadDirectoryCoverageSummaryIndex = "UploadDirectoryCoverageSummaryIndex";
    public const string GetFileCoverageDetailsStreamAsync = "GetFileCoverageDetailsStreamAsync";
    public const string PipelineCoverageEvaluationJob = "PipelineCoverageEvaluationJob";
    public const string TestCloneJob = "TestCloneJob";
    public const string SuiteCloneJob = "SuiteCloneJob";
    public const string ExportTestCaseFeature = "ExportTestCase";
    public const string DuplicateResultsWithSamePointId = "DuplicateResultsWithSamePointId";
    public const string FlakyUpdateSettings = "FlakyUpdateSettings";
    public const string FlakySettings = "FlakySettingsTelemetry";
    public const string ExistingFlakySettings = "ExistingFlakySettings";
    public const string DeleteFlakyData = "DeleteFlakyData";
    public const string GetTestAttachmentsFromLogStore = "GetTestAttachmentsFromLogStore";
    public const string GetTestAttachmentFromLogStore = "GetTestAttachmentFromLogStore";
    public const string TestRunTagFeature = "TestRunTag";
    public const string SyncSuiteJob_WIQLQueryCount = "SyncSuiteJob_WIQLQueryCount";
    public const string SyncSuiteJob_SyncCount = "SyncSuiteJob_SyncCount";
    public const string QueueInvokerJobForPatchSummaryRequest = "QueueInvokerJobForPatchSummaryRequest";
    public const string PointOutcomeSync = "PointOutcomeSync";
    public const string GetSourceControlPaths = "GetSourceControlPaths";
    public const string PipelineDeltaCoverageProvider = "PipelineDeltaCoverageProvider";
    public const string GetContinuationTokenForCoverageChanges = "GetContinuationTokenForCoverageChanges";
    public const string CoverageCacheProvider = "CoverageCacheProvider";
    public const string GetCoverageSummaryList = "GetCoverageSummaryList";
    public const string PipelineDirectorySummaryProvider = "PipelineDirectorySummaryProvider ";
    public const string GetTestAttachments = "GetTestAttachments";
    public const string UpdateTestRunForLogStoreAttachments = "UpdateTestRunForLogStoreAttachments";
    public const string ProcessSingleFileUploadInLogStore = "ProcessSingleFileUploadInLogStore";
    public const string ProcessMultiFilesUploadInLogStore = "ProcessMultiFilesUploadInLogStore";
    public const string FolderLevelPolicy = "FolderLevelCodeCoveragePolicy";
    public const string TestRunCustomFieldsFeature = "TestRunCustomFields";
    public const string QueryTestPlans = "QueryTestPlans";
    public const string QueryTestSuites = "QueryTestSuites";
    public const string QueryTestPoints = "QueryTestPoints";
    public const string AssignTesterPointState = "AssignTesterPointState";
    public const string QueryTestPointStatistics = "QueryTestPointStatistics";
    public const string QueryTestSummaryForReleases = "QueryTestSummaryForReleases";
    public const string LogStoreMigration = "LogStoreMigration";
    public const string LogStoreGetTestLogs = "LogStoreGetTestLogs";
    public const string LogStoreGetEndPoint = "LogStoreGetEndPoint";
    public const string LogStoreContentSizeMonitor = "LogStoreContentSizeMonitor";
    public const string LogStoreContentSizeDeletion = "LogStoreContentSizeDeletion";
    public const string UploadBuildAttachmentsToLogStore = "UploadBuildAttachmentsToLogStore";
    public const string DeleteTestSuite = "DeleteTestSuite";
    public const string DeleteTestPlan = "DeleteTestPlan";
    public const string TestOutcomeOutOfSync = "TestOutcomeOutOfSync";
    public const string CodeCoverageSummaryUpdater = "CodeCoverageSummaryUpdater";
    public const string CreateTestAttachmentInLogStore_NewApi = "CreateTestAttachmentInLogStore_NewApi";
    public const string CreateTestAttachment = "CreateTestAttachment";
    public const string GetTestAttachmentsFromLogStore_NewApi = "GetTestAttachmentsFromLogStore_NewApi";
    public const string DownloadTestAttachmentFromLogStore = "DownloadTestAttachmentFromLogStore";
    public const string DeleteTestAttachmentFromLogStore = "DeleteTestAttachmentFromLogStore";
    public const string DeleteTestAttachment = "DeleteTestAttachment";
    public const string UploadAttachmentToLogStoreThroughCreateAttachment = "UploadAttachmentToLogStoreThroughCreateAttachment";
    public const string TryDeleteTestAttachmentFromLogStore = "TryDeleteTestAttachmentFromLogStore";
    public const string DeleteTestPointsAndRunsForSuites = "DeleteTestPointsAndRunsForSuites";
    public const string DeleteTestRunsForSuites = "DeleteTestRunsForSuites";
    public const string RepopulateAllTestSuitesInTestPlans = "RepopulateAllTestSuitesInTestPlans";
    public const string QueueDeleteTestPlan = "QueueDeleteTestPlan";
    public const string GetTestPlanIds = "GetTestPlanIds";
    public const string TestPlanObject = "TestPlanObject";
    public const string GetTestResultFailureType = "GetTestResultFailureType";
    public const string CreateTestResultFailureType = " CreateTestResultFailureType";
    public const string DeleteTestResultFailureType = "DeleteTestResultFailureType";
    public const string TestSubResultDetails = "TestSubResultDetails";
  }
}
