// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.TestResultsConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement
{
  public static class TestResultsConstants
  {
    public const int MaxTestResultsPageSize = 10000;
    public const int TestResultsPageSize = 1000;
    public const int MaxTestResultsPageSizeIncludingDetails = 200;
    public const int TestResultsPageSizeIncludingDetails = 100;
    public const int TestResultsCommentMaxSize = 1000;
    public const int DefaultBuildCountForResultTrend = 10;
    public const int DefaultResultsCountTrend = 10;
    public const int MaxBuildCountForResultTrend = 30;
    public const int MaxDaysForTestResultsTrend = 15;
    public const int MaxDaysForTestResultsWorkItems = 60;
    public const int MaxDaysForTestCaseResultHistory = 7;
    public const int TestHistoryOperatingTestResultBatchLimit = 4000;
    public const int MaxWorkItemCountForResults = 100;
    public const int MaxResultCountForTestCaseResultHistory = 50;
    public const int MaxHistoryDaysForResultTrendByBuild = 30;
    public const int DefaultBatchSizeForPublishTestResultsByJob = 1000;
    public const int InvalidTestResultId = -1;
    public const int BatchSizeToFetchAssociatedWorkItems = 100;
    public const int TestRunSystemMaxSize = 16;
    public const int PassedOnRerunIntValue = 15;
    public const int TestResultsGroupsPageSize = 50000;
    public const int FetchAllTestResultsForBuildOrReleasePageSize = 20000;
    public const int TestResultsForBuildOrReleasePageSize = 10000;
    public const int DefaultTestResultsNotificationBatchSize = 1000;
    public const int SimilarTestResultsBatchSize = 10000;
    public const int DefaultTestResultNotificationProcessorJobDelayInSec = 10;
    public const int DefaultTestResultNotificationProcessorJobTimeoutInSec = 30;
    public const int DefaultTestResultNotificationCleanupBatchSize = 10000;
    public const int DefaultTestResultNotificationRetentionInDays = 1;
    public const int TestSubResultJsonMaxSize = 104857600;
    public const int TestSubResultMaxHierarchyLevel = 3;
    public const int TestSubResultMaxSubResultPerLevel = 1000;
    public const int MaxSubResultCountForAttachmentInLogStore = 10000;
    public static readonly string TestSubResultFileName = "SubResult.json";
    public static readonly string TestSubResultV2FileName = "SubResultBatch.json";
    public static readonly string TestSubResultV2LogStoreFilePath = "SubResult/";
    public static readonly string TestSubResultV2LogStoreFileName = "Batch_{0}.json";
    public static readonly string TestSubResultJsonAttachmentType = "SubResultJson";
    public static readonly string TestRunLogStoreFileName = "TestRun_{0}_";
    public static readonly string TestRunLogStoreFilePath = "TestRun/";
    public static readonly string TestResultLogStoreFileName = "TestResult_{0}_";
    public static readonly string TestResultLogStoreFilePath = "TestResult/";
    public static readonly string TestSubResultLogStoreFileNameNotJson = "TestSubResult_{0}_";
    public static readonly string TestResultExtensionLogStoreFilenamePattern = "extension";
    public static readonly string TestResultPropertyDuration = "Duration";
    public static readonly string TestResultPropertyOwner = "Owner";
    public static readonly string TestResultPropertyTestCaseOwner = "TestCaseOwner";
    public static readonly string TestResultPropertyContainer = "AutomatedTestStorage";
    public static readonly string TestResultPropertyDateStarted = "DateStarted";
    public static readonly string TestResultPropertyDateCompleted = "DateCompleted";
    public static readonly string TestResultPropertyPriority = "Priority";
    public static readonly string TestResultPropertyTestRunId = "TestRunId";
    public static readonly string TestResultPropertyTestResultId = "TestResultId";
    public static readonly string TestResultPropertyTestCaseRefId = "TestCaseRefId";
    public static readonly string DefaultTestRunSystem = "VSTS";
    public static readonly string PassedOnRerun = nameof (PassedOnRerun);
    public static readonly string TestResultField = nameof (TestResultField);
    public static readonly int RegexTimeOutInMilliSeconds = 100;
    public static readonly string TestRunTitleColumnName = "run.Title";
    public static readonly string GroupByTestRunTitleColumnName = "result.Title";
    public static readonly string TestRunIdColumnName = "result.TestRunId";
    public static readonly string PriorityColumnName = "result.Priority";
    public static readonly string TestPointColumnName = "result.TestPointId";
    public static readonly string TestPlanColumnName = "point.PlanId";
    public static readonly string TestSuiteColumnName = "point.SuiteId";
    public static readonly string TestSuiteTitleColumnName = "suite.Title";
    public static readonly string ParentSuiteIdColumnName = "suite.ParentSuiteId";
    public static readonly string PlanIdColumnName = "suite.PlanId";
    public static readonly string ContainerColumnName = "result.AutomatedTestStorage";
    public static readonly string OutcomeColumnName = "Outcome";
    public static readonly string DurationColumnName = "result.Duration";
    public static readonly string AutomatedTestNameColumn = "AutomatedTestName";
    public static readonly string TestCaseTitleColumnName = "TestCaseTitle";
    public static readonly string GroupByTestRunStateColumnName = "result.State";
    public static readonly string GroupByTestRunCompleteDateColumnName = "result.CompleteDate";
    public static readonly string BuildReferenceColumnName = "BuildReference";
    public static readonly string ReleaseReferenceColumnName = "ReleaseReference";
    public static readonly string ReleaseEnvironmentIdColumnName = "ReleaseEnvId";
    public static readonly string ReleaseEnvDefinitionIdColumnName = "ReleaseEnvDefId";
    public static readonly string BuildDefinitionIdColumnName = "BuildDefinitionId";
    public static readonly string ReleaseDefinitionIdColumnName = "ReleaseDefId";
    public static readonly string BranchNameColumnName = "BranchName";
    public static readonly string CommentColumnName = "Comment";
    public static readonly string RequirementColumnName = "requirementMapping.WorkItemId";
    public static readonly string ErrorMessageColumnName = "ErrorMessage";
    public static readonly string OwnerNameColumnName = "result.TestCaseOwner";
    public static readonly string DefaultGroupByFields = TestResultsConstants.ContainerColumnName;
    public static readonly string OwnerFilterName = "Owner";
    public static readonly string ContainerFilterName = "AutomatedTestStorage";
    public static readonly string DefaultIdentifierProperties = "result.TestRunId, result.TestResultId, result.TestCaseRefId, result.Duration";
    public static readonly string DefaultFilterClause = " AND result.Outcome = 3";
    public static readonly string TrueCondition = "1 = 1";
    public static readonly string FalseCondition = "1 = 0";
    public static readonly string OldTestCaseRefId = "result.OldTestCaseRefId";
    public static readonly HashSet<string> TestResultProperties = new HashSet<string>()
    {
      "CreationDate",
      "TestRunId",
      "TestResultId",
      "TestCaseId",
      "ConfigurationId",
      "TestPointId",
      "State",
      "Outcome",
      "ResolutionStateId",
      "Comment",
      "ErrorMessage",
      "LastUpdated",
      "DateStarted",
      "DateCompleted",
      "Duration",
      "Owner",
      "Priority",
      "TestCaseTitle",
      "TestCaseRevision",
      "ComputerName",
      "AfnStripId",
      "ResetCount",
      "FailureType",
      "AutomatedTestName",
      "AutomatedTestStorage",
      "AutomatedTestType",
      "AutomatedTestTypeId",
      "AutomatedTestId",
      "Revision",
      "RunBy",
      "LastUpdatedBy"
    };
  }
}
