// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TelemetryParameterNames
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TelemetryParameterNames
  {
    public const string ProjectId = "ProjectId";
    public const string Exception = "Exception";
    public const string SuitesSyncCount = "SuitesCount";
    public const string RepopulateSuiteId = "RepopulateSuiteId";
    public const string TestRunSource = "TestRunSource";
    public const string TestRunType = "TestRunType";
    public const string TestRunAutomatedOrManual = "TestRunAutomatedOrManual";
    public const string TestRunId = "TestRunId";
    public const string TestRunPoints = "TestRunPoints";
    public const string TestRunSettings = "TestRunSettings";
    public const string SourceWorkflow = "Workflow";
    public const string BuildUri = "BuildUri";
    public const string ReleaseUri = "ReleaseUri";
    public const string ReleaseEnvUri = "ReleaseEnvUri";
    public const string Attempt = "Attempt";
    public const string TestRunSystem = "TestRunSystem";
    public const string TestRunCommentLength = "TestRunCommentLength";
    public const string PipelineId = "PipelineId";
    public const string PipelineInstanceId = "PipelineInstanceId";
    public const string ResultCommentsSize = "ResultCommentsSize";
    public const string ResultErrorMessageSize = "ResultErrorMessageSize";
    public const string ResultStackTraceSize = "ResultStackTraceSize";
    public const string ResultOperation = "ResultOperation";
    public const string RunState = "RunState";
    public const string NumberOfCustomTestFields = "NumberOfCustomTestFields";
    public const string CreateTestRunTag = "CreateTestRunTag";
    public const string UpdateTestRunTag = "UpdateTestRunTag";
    public const string TestRunTagCount = "TestRunTagCount";
    public const string SubResultOldJson = "SubResultOldJson";
    public const string TestType = "TestType";
    public const string TestResultCount = "TestResultCount";
    public const string NewTestCaseRefRows = "NewTestCaseRefRows";
    public const string TestResultId = "TestResultId";
    public const string TestSubResultId = "TestSubResultId";
    public const string TestRunState = "TestRunState";
    public const string PassedTestsCount = "PassedTests";
    public const string FailedTestsCount = "FailedTests";
    public const string TotalTestsCount = "TotalTests";
    public const string NotApplicableTestsCount = "NotAvailableTestsCount";
    public const string UnAnalyzedTestsCount = "UnAnalyzedTestsCount";
    public const string TotalAutomatedTests = "TotalAutomatedTests";
    public const string TotalIncompleteTests = "TotalIncompleteTests";
    public const string Duration = "RunDurationMs";
    public const string TestResultFailureTypeId = "TestResultFailureTypeId";
    public const string TestResultFailureTypeName = "TestResultFailureType";
    public const string TestExtensionFieldAdded = "TestExtensionFieldAdded";
    public const string RunScopedTestExtensionFieldAdded = "RunScopedTestExtensionFieldAdded";
    public const string ResultScopedTestExtensionFieldAdded = "ResultScopedTestExtensionFieldAdded";
    public const string TestExtensionFieldQueryScope = "TestExtensionFieldQueryScope";
    public const string UpdateResultRetentionSettings = "UpdateResultRetentionSettings";
    public const string CreateResultRetentionSettings = "CreateResultRetentionSettings";
    public const string AutomatedRunsDeleted = "AutomatedRunsDeleted";
    public const string ManualRunsDeleted = "ManualRunsDeleted";
    public const string FileExtensionType = "FileExtensionType";
    public const string FileName = "FileName";
    public const string FilePath = "FilePath";
    public const string ShowAdvertisementsPage = "ShowAdvertisementsPage";
    public const string TotalResults = "TotalResults";
    public const string ResultsWithOwnerInfo = "ResultsWithOwnerInfo";
    public const string ResultsWithValidOwnerDisplayName = "ResultsWithValidOwnerDisplayName";
    public const string ResultsWithValidOwnerDirectioryAlias = "ResultsWithValidOwnerDirectioryAlias";
    public const string TotalSubResults = "TotalSubResults";
    public const string TotalHierarchicalResults = "TotalHierarchicalResults";
    public const string MaxResultIterations = "MaxResultIterations";
    public const string TotalRerunResults = "TotalRerunResults";
    public const string TotalDDResults = "TotalDDResults";
    public const string TotalOTResults = "TotalOTResults";
    public const string TotalGenericResults = "TotalGenericResults";
    public const string MaxRerunAttempts = "MaxRerunAttempts";
    public const string TotalPassedOnRerun = "TotalPassedOnRerun";
    public const string TotalDDRerun = "TotalDDRerun";
    public const string TotalOTRerun = "TotalOTRerun";
    public const string CoverageAsyncApi = "CoverageAsyncApi";
    public const string HardDeletedRunsCount = "HardDeletedRunsCount";
    public const string HardDeletedResultsCount = "HardDeletedResultsCount";
    public const string DeletedTestCaseRefs = "DeletedTestCaseRefs";
    public const string ProjectsTestCaseRefsCleanupInfo = "ProjectsTestCaseRefsCleanupInfo";
    public const string TotalTestCaseRefDeletedCount = "TotalTestCaseRefDeletedCount";
    public const string TotalSuccessfulLogStoreContainerDeletion = "TotalSuccessfulLogStoreContainerDeletion";
    public const string TotalFailureLogStoreContainerDeletion = "TotalFailureLogStoreContainerDeletion";
    public const string TotalNonExistLogStoreContainer = "TotalNonExistLogStoreContainer";
    public const string TotalTimeElapsedForLogStoreContainerDeletion = "TotalTimeElapsedForLogStoreContainerDeletion";
    public const string TotalTimeElpasedForLogStoreMigration = "TotalTimeElpasedForLogStoreMigration";
    public const string LogStoreMigrationId = "LogStoreMigrationId";
    public const string LogStoreTotalContainersMigrated = "LogStoreTotalContainersMigrated";
    public const string LogStoreMigrationStatus = "LogStoreMigrationStatus";
    public const string LogStoreMigrationStatusReason = "LogStoreMigrationStatusReason";
    public const string LogStoreFailedContainersMigrated = "LogStoreFailedContainersMigrated";
    public const string SecondaryStorageAccountUsed = "SecondaryStorageAccountUsed";
    public const string TotalBlobs = "TotalBlobs";
    public const string IsPagedResult = "IsPagedResult";
    public const string SuccessFullyCreatedURI = "SuccessFullyCreatedURI";
    public const string BuildId = "BuildId";
    public const string ReleaseId = "ReleaseId";
    public const string PullRequestId = "PullRequestId";
    public const string PullRequestIterationId = "PullRequestIterationId";
    public const string IsPullRequestScenario = "IsPullRequestScenario";
    public const string E2ETrackingId = "E2ETrackingId";
    public const string BuildDefinitionId = "BuildDefinitionId";
    public const string BuildDefinintionName = "BuildDefinitionName";
    public const string ReleaseDefinitionId = "ReleaseDefinitionId";
    public const string HardDeletedReleasesCount = "HardDeletedReleasesCount";
    public const string HardDeletedReleasesIds = "HardDeletedReleasesIds";
    public const string TotalNumberOfRuns = "TotalNumberOfRuns";
    public const string TotalNumberOfRunsWithoutSummary = "TotalNumberOfRunsWithoutSummary";
    public const string SummaryAndInsightsCalculationFlow = "SummaryAndInsightsCalculationFlow";
    public const string Query = "Query";
    public const string Command = "Command";
    public const string IncludeStatistics = "IncludeStatistics";
    public const string ContainsLastResultDetails = "ContainsLastResultDetails";
    public const string TestCaseProperties = "TestCaseProperties";
    public const string ActiveTestCaseCount = "ActiveTestCaseCount";
    public const string NonActiveTestCaseCount = "NonActiveTestCaseCount";
    public const string SourceProjectname = "SourceProjectname";
    public const string DestinationProjectName = "DestinationProjectName";
    public const string CloneRequirements = "CloneRequirements";
    public const string CopyAllSuites = "CopyAllSuites";
    public const string CopyAncestorHierarchy = "CopyAncestorHierarchy";
    public const string DestinationWorkItemType = "DestinationWorkItemType";
    public const string OverrideParameters = "OverrideParameters";
    public const string RelatedLinkComment = "RelatedLinkComment";
    public const string SourcePlanId = "SourcePlanId";
    public const string SourceSuiteIds = "SourceSuiteIds";
    public const string DestinationPlanId = "DestinationPlanId";
    public const string Area = "Area";
    public const string Iteration = "Iteration";
    public const string SourceSuiteId = "SourceSuiteId";
    public const string DestinationSuiteId = "DestinationSuiteId";
    public const string LogStoreAttachmentsFetched = "LogStoreAttachmentsFetched";
    public const string EmptyLogStoreAttachmentsCreated = "EmptyLogStoreAttachmentsCreated";
    public const string EmptyTestLogAndTestLogComparison = "EmptyTestLogAndTestLogComparison";
    public const string LogStoreAttachmentDownloaded = "LogStoreAttachmentDownloaded";
    public const string AttachmentId = "AttachmentId";
    public const string FlakyTestCaseReferenceId = "FlakyTestCaseReferenceId";
    public const string FlakyTestCaseReferenceCount = "FlakyTestCaseReferenceCount";
    public const string UnFlakyTestCaseReferenceCount = "UnFlakyTestCaseReferenceCount";
    public const string TotalReleasesCount = "TotalReleasesCount";
    public const string ProjectName = "ProjectName";
    public const string SuiteIds = "SuiteIds";
    public const string SuiteId = "SuiteId";
    public const string SuiteCount = "SuiteCount";
    public const string SuiteType = "SuiteType";
    public const string TestCaseIds = "TestCaseIds";
    public const string ParentSuiteId = "ParentSuiteId";
    public const string PlanId = "PlanId";
    public const string WorkItemsCount = "WorkItemsCount";
    public const string ServerEntriesCount = "ServerEntriesCount";
    public const string TestCasesCount = "TestCasesCount";
    public const string ExportedTestCaseFileLength = "ExportedTestCaseFileLength";
    public const string BuildNumber = "BuildNumber";
    public const string BuildResult = "BuidResult";
    public const string BuildStatus = "BuildStatus";
    public const string RepositoryId = "RepositoryId";
    public const string DeletedRunIds = "DeletedRunIds";
    public const string TestRunProperties = "TestRunProperties";
    public const string TestRunIds = "TestRunIds";
    public const string TestResultIds = "TestResultIds";
    public const string AttachmentsAdded = "AttachmentsAdded";
    public const string AttachmentsDeleted = "AttachmentsDeleted";
    public const string AttachmentsToAdd = "AttachmentsToAdd";
    public const string AttachmentsToDelete = "AttachmentsToDelete";
    public const string IsSuccessful = "IsSuccessful";
    public const string UncompressedLength = "UncompressedLength";
    public const string DeletedFlakyTestCount = "DeletedFlakyTestCount";
    public const string IsWiql = "IsWiql";
    public const string ExcludeOrphanPlans = "ExcludeOrphanPlans";
    public const string Top = "Top";
    public const string PlanIds = "PlanIds";

    public enum SummaryAndInsightsCalculationFlowType
    {
      QueryFlow,
      EventFlow,
      JobFlow,
    }
  }
}
