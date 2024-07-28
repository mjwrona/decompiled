// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementFeatureFlags
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class TestManagementFeatureFlags
  {
    public const string RepopulateSuitesWhenDefineExecuteTabLoaded = "TestManagement.Server.RepopulateSuitesWhenDefineExecuteTabLoaded";
    public const string DeleteAssociatedRunsOnSuiteDeletion = "TestManagement.Server.DeleteAssociatedRunsOnSuiteDeletion";
    public const string WorkItemTestLinksNewApi = "TestManagement.Server.WorkItemTestLinksNewApi";
    public const string UseStaticSprocInsteadOfDynamic2 = "TestManagement.Server.UseStaticSprocInsteadOfDynamic2";
    public const string EnableLockFreeDeleteTestPlan = "TestManagement.Server.EnableLockFreeDeleteTestPlan";
    public const string LightDeleteSuiteEntries = "TestManagement.Server.LightDeleteSuiteEntries";
    public const string CheckSuiteRepopulateInterval = "TestManagement.Server.CheckSuiteRepopulateInterval";
    public const string BlockTestManagementFeatureFlag = "LabManagement.Server.BlockLabManagement";
    public const string TestPointFiltersCache = "TestManagement.Server.TestPointFiltersCache";
    public const string TestPointResetToActiveFromParentSuite = "TestManagement.Server.TestPointResetToActiveFromParentSuite";
    public const string SuitesApiIncludeTesters = "TestManagement.Server.SuitesApiIncludeTesters";
    public const string TestSuitesCache = "TestManagement.Server.TestSuitesCache";
    public const string ProjectScopedLockInUpdateCss = "TestManagement.Server.ProjectScopedLockInUpdateCss";
    public const string IgnoreWhitelistedFields = "TestManagement.Server.IgnoreWhitelistedFields";
    public const string DisableTestOutcomeSynchronizerJob = "TestManagement.Server.Job.DisableTestOutcomeSynchronizerJob";
    public const string DisableTestCaseSynchronizerJob = "TestManagement.Server.Job.DisableTestCaseSynchronizerJob";
    public const string DisableChildSuitesCleanupJob = "TestManagement.Server.Job.DisableChildSuitesCleanupJob";
    public const string BlockAccessToTestSoapCalls = "TestManagement.Server.BlockAccessToTestSoapCalls";
    public const string EnableChartingMigration = "TestManagement.Server.EnableChartingMigration";
    public const string SyncSuitesViaJob = "TestManagement.Server.SyncSuitesViaJob";
    public const string EnableDualWriteForTestPoint = "TestManagement.Server.EnableDualWriteForTestPoint";
    public const string EnableBulkUpdateUsingServerOM = "TestManagement.Server.BulkUpdateUsingServerOM";
    public const string EnablePointOutcomeSyncJob = "TestManagement.Server.EnablePointOutcomeSyncJob";
    public const string EnableDirectTCMS2SCallFromTFSLegacy = "TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy";
    public const string ApiDisallowMultipleIds = "WebAccess.TestManagement.ApiDisallowMultipleIds";
    public const string ImproveGetTestPlansApiPerformance = "TestManagement.Server.ImproveGetTestPlansApiPerformance";
    public const string DeleteTestPlanEntryAfterRelatedWorkItemDeletion = "TestManagement.Server.DeleteTestPlanEntryAfterRelatedWorkItemDeletion";
    public const string GetTestExecutionSummaryReportWithSqlReadReplica = "TestManagement.Server.GetTestExecutionSummaryReport.EnableSqlReadReplica";
    public const string QueryResultsWithSqlReadReplica = "TestManagement.Server.QueryResults.EnableSqlReadReplica";
    public const string GetManualTestResultsByUpdatedDateWithSqlReadReplica = "TestManagement.Server.GetManualTestResultsByUpdatedDate.EnableSqlReadReplica";
    public const string QueryTestHistoryWithSqlReadReplica = "WebAccess.TestManagement.QueryTestHistory.EnableSqlReadReplica";
    public const string DisableTestPlansAccessWithTestManagerExtension = "TestManagement.Server.DisableTestPlanAccessWithTestManagerExtension";
  }
}
