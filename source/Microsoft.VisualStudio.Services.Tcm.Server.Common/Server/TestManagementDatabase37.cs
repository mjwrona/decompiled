// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase37
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase37 : TestManagementDatabase36
  {
    public override void DeleteTestReleases(
      Guid projectGuid,
      List<int> releaseIds,
      Guid lastUpdatedBy,
      int testRunDeletionBatchSize,
      bool isTcmService)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteTestReleases");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt32Table(nameof (releaseIds), (IEnumerable<int>) releaseIds);
      this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
      this.BindInt("@testRunDeletionBatchSize", testRunDeletionBatchSize);
      this.ExecuteNonQuery();
    }

    public override ReleaseReference GetReleaseRef(
      Guid projectGuid,
      int releaseId,
      int releaseEnvironmentId)
    {
      this.PrepareStoredProcedure("prc_GetReleaseRef");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@releaseEnvironmentId", releaseEnvironmentId);
      TestManagementDatabase37.ReleaseRefsColumns releaseRefsColumns = new TestManagementDatabase37.ReleaseRefsColumns();
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? releaseRefsColumns.bind(reader) : (ReleaseReference) null;
    }

    internal TestManagementDatabase37(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase37()
    {
    }

    public override void QueueDeleteRunsByRetentionSettings(
      TestManagementRequestContext context,
      Guid projectId,
      DateTime currentUtcDate,
      Guid deletedBy,
      int runsDeletionBatchSize,
      int automatedTestRetentionDuration,
      int manualTestRetentionDuration,
      out int automatedRunsDeleted,
      out int manualRunsDeleted,
      bool isTcmService,
      bool isOnpremService,
      int queueDeleteRunsByRetentionSettingsSprocExecTimeOutInSec)
    {
      try
      {
        context.TraceEnter("Database", "ResultRetentionDatabase.QueueDeleteRunsByRetentionSettings");
        this.PrepareStoredProcedure("TestResult.prc_QueueDeleteRunsByRetentionSettings", queueDeleteRunsByRetentionSettingsSprocExecTimeOutInSec);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@automatedDuration", automatedTestRetentionDuration);
        this.BindInt("@manualDuration", manualTestRetentionDuration);
        this.BindDateTime("@currentUtcDate", currentUtcDate);
        this.BindGuid("@deletedBy", deletedBy);
        this.BindInt("@runsDeletionBatchSize", runsDeletionBatchSize);
        SqlDataReader reader = this.ExecuteReader();
        automatedRunsDeleted = 0;
        manualRunsDeleted = 0;
        if (!reader.Read())
          return;
        automatedRunsDeleted = new SqlColumnBinder("AutomatedRunsDeleted").GetInt32((IDataReader) reader);
        manualRunsDeleted = new SqlColumnBinder("ManualRunsDeleted").GetInt32((IDataReader) reader);
      }
      finally
      {
        context.TraceLeave("Database", "ResultRetentionDatabase.QueueDeleteRunsByRetentionSettings");
      }
    }

    internal override List<TestCaseResult> QueryTestResultsByBuildOrRelease(
      Guid projectId,
      int buildId,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      IList<byte> runStates,
      bool fetchFailedTestsOnly,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      bool parameterValue1 = runStates.Contains((byte) 4);
      bool parameterValue2 = runStates.Contains((byte) 2);
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsByBuildOrRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@buildId", buildId);
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@releaseEnvId", releaseEnvId);
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindInt("@continuationTokenRunId", continuationTokenRunId);
      this.BindInt("@continuationTokenResultId", continuationTokenResultId);
      this.BindInt("@top", top);
      this.BindBoolean("@isAbortedRunEnabled", parameterValue1);
      this.BindBoolean("@isInProgressEnabled", parameterValue2);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
      List<TestCaseResult> testResults = testCaseResultList;
      TestManagementDatabase36.GetTestResultsBind(reader, testResults);
      return testCaseResultList;
    }

    internal override List<int> QuerySoftDeletedRuns(
      Guid projectId,
      int waitDaysForCleanup,
      int runsDeletionBatchSize,
      DateTime deleteStartDate)
    {
      this.PrepareStoredProcedure("TestResult.prc_QuerySoftDeletedRuns");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@runsDeletionBatchSize", runsDeletionBatchSize);
      this.BindInt("@waitDaysForCleanup", waitDaysForCleanup);
      this.BindDateTime("@deleteStartDate", deleteStartDate);
      SqlDataReader reader = this.ExecuteReader();
      List<int> intList = new List<int>();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TestRunId");
      while (reader.Read())
        intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      if (reader.NextResult())
      {
        while (reader.Read())
          intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      }
      return intList;
    }

    internal override void CleanDeletedTestRuns2(
      Guid projectId,
      List<int> runIds,
      int resultsDeletionBatchSize,
      int cleanDeletedTestRunsSprocExecTimeOutInSec,
      int reuseTestRunIdThreshold)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteTestRun2", cleanDeletedTestRunsSprocExecTimeOutInSec);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt32TypeTable("@runsToDelete", (IEnumerable<int>) runIds);
      this.BindInt("@batchSize", resultsDeletionBatchSize);
      this.ExecuteNonQuery();
    }

    internal override List<RetainedResultsDistribution> QueryRetainedResultsDistribution(
      Guid projectId,
      int retainedBeyondDays,
      int queryRetainedResultsDistributionSprocExecTimeOutInSec)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryRetainedResultsDistribution", queryRetainedResultsDistributionSprocExecTimeOutInSec);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@retainedBeyondDays", retainedBeyondDays);
      SqlDataReader reader = this.ExecuteReader();
      List<RetainedResultsDistribution> resultsDistributionList = new List<RetainedResultsDistribution>();
      TestManagementDatabase37.RetainedResultsDistributionColumns distributionColumns = new TestManagementDatabase37.RetainedResultsDistributionColumns();
      while (reader.Read())
        resultsDistributionList.Add(distributionColumns.BindDistributionByBuildDefinition(reader));
      reader.NextResult();
      while (reader.Read())
        resultsDistributionList.Add(distributionColumns.BindDistributionByReleaseDefinition(reader));
      return resultsDistributionList;
    }

    public override List<TestSettings> GetTestSettings(
      Guid projectId,
      int top,
      int continuationTokenId)
    {
      this.PrepareStoredProcedure("prc_QueryTestSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@top", top);
      this.BindInt("@continuationTokenId", continuationTokenId);
      List<TestSettings> testSettings1 = new List<TestSettings>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryTestSettingsColumns testSettingsColumns = new TestManagementDatabase.QueryTestSettingsColumns();
      while (reader.Read())
      {
        TestSettings testSettings2 = testSettingsColumns.Bind(this.RequestContext, reader, out string _);
        testSettings1.Add(testSettings2);
      }
      return testSettings1;
    }

    private new class ReleaseRefsColumns
    {
      private SqlColumnBinder ReleaseRefId = new SqlColumnBinder(nameof (ReleaseRefId));
      private SqlColumnBinder ReleaseUri = new SqlColumnBinder(nameof (ReleaseUri));
      private SqlColumnBinder ReleaseEnvUri = new SqlColumnBinder(nameof (ReleaseEnvUri));
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder ReleaseDefId = new SqlColumnBinder(nameof (ReleaseDefId));
      private SqlColumnBinder ReleaseEnvDefId = new SqlColumnBinder(nameof (ReleaseEnvDefId));
      private SqlColumnBinder Attempt = new SqlColumnBinder(nameof (Attempt));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));
      private SqlColumnBinder ReleaseCreationDate = new SqlColumnBinder(nameof (ReleaseCreationDate));
      private SqlColumnBinder EnvironmentCreationDate = new SqlColumnBinder(nameof (EnvironmentCreationDate));

      internal ReleaseReference bind(SqlDataReader reader) => new ReleaseReference()
      {
        ReleaseRefId = this.ReleaseRefId.GetInt32((IDataReader) reader),
        ReleaseUri = this.ReleaseUri.GetString((IDataReader) reader, true),
        ReleaseEnvUri = this.ReleaseEnvUri.GetString((IDataReader) reader, true),
        ReleaseId = this.ReleaseId.GetInt32((IDataReader) reader),
        ReleaseEnvId = this.ReleaseEnvId.GetInt32((IDataReader) reader),
        ReleaseDefId = this.ReleaseDefId.GetInt32((IDataReader) reader),
        ReleaseEnvDefId = this.ReleaseEnvDefId.GetInt32((IDataReader) reader),
        Attempt = this.Attempt.GetInt32((IDataReader) reader),
        ReleaseName = this.ReleaseName.GetString((IDataReader) reader, true),
        ReleaseCreationDate = this.ReleaseCreationDate.GetDateTime((IDataReader) reader),
        EnvironmentCreationDate = this.EnvironmentCreationDate.GetDateTime((IDataReader) reader)
      };
    }

    private class RetainedResultsDistributionColumns
    {
      private SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));
      private SqlColumnBinder ReleaseDefinitionId = new SqlColumnBinder(nameof (ReleaseDefinitionId));

      internal RetainedResultsDistribution BindDistributionByBuildDefinition(SqlDataReader reader) => new RetainedResultsDistribution()
      {
        TotalTests = this.TotalTests.GetInt32((IDataReader) reader),
        BuildDefinitionId = this.BuildDefinitionId.GetInt32((IDataReader) reader)
      };

      internal RetainedResultsDistribution BindDistributionByReleaseDefinition(SqlDataReader reader) => new RetainedResultsDistribution()
      {
        TotalTests = this.TotalTests.GetInt32((IDataReader) reader),
        ReleaseDefinitionId = this.ReleaseDefinitionId.GetInt32((IDataReader) reader)
      };
    }
  }
}
