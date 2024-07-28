// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase39
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase39 : TestManagementDatabase38
  {
    public override void DeleteTestBuild(
      Guid projectId,
      string[] buildUris,
      Guid lastUpdatedBy,
      bool deleteOnlyAutomatedRuns = false,
      bool isTcmService = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteTestBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindNameTypeTable("@buildUris", (IEnumerable<string>) buildUris);
      this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
      this.BindBoolean("@deleteOnlyAutomatedRuns", deleteOnlyAutomatedRuns);
      this.ExecuteNonQuery();
    }

    public override void MarkTestBuildDeleted(Guid projectId, string[] buildUris)
    {
      this.PrepareStoredProcedure("prc_MarkTestBuildDeleted");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindNameTypeTable("@buildUris", (IEnumerable<string>) buildUris);
      this.ExecuteNonQuery();
    }

    internal TestManagementDatabase39(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase39()
    {
    }

    public override void AddOrUpdateBuildInProgressTestSignal(Guid projectId, int buildId)
    {
      this.PrepareStoredProcedure("TestResult.prc_MergeInProgressTestResultsNotificationForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@buildId", buildId);
      this.BindBoolean("@isNotificationEnabled", true);
      this.ExecuteReader();
    }

    public override void AddOrUpdateReleaseInProgressTestSignal(
      Guid projectId,
      int releaseId,
      int environmentId)
    {
      this.PrepareStoredProcedure("TestResult.prc_MergeInProgressTestResultsNotificationForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@releaseEnvId", environmentId);
      this.BindBoolean("@isNotificationEnabled", true);
      this.ExecuteReader();
    }

    public override List<int> QueryAndUpdateBuildInProgressTestSignals(
      IEnumerable<Guid> projectIds,
      int batchSize)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryAndUpdateInProgressTestResultsNotificationForBuild");
      this.BindInt32TypeTable("@dataSpaceIds", projectIds.Select<Guid, int>((System.Func<Guid, int>) (x => this.GetDataspaceIdWithLazyInitialization(x))));
      this.BindInt("@batchSize", batchSize);
      SqlDataReader reader = this.ExecuteReader();
      List<int> intList = new List<int>();
      while (reader.Read())
      {
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("BuildId");
        intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      }
      return intList;
    }

    public override List<(int ReleaseId, int EnvironmentId)> QueryAndUpdateReleaseInProgressTestSignals(
      IEnumerable<Guid> projectIds,
      int batchSize)
    {
      List<(int, int)> valueTupleList = new List<(int, int)>();
      this.PrepareStoredProcedure("TestResult.prc_QueryAndUpdateInProgressTestResultsNotificationForRelease");
      this.BindInt32TypeTable("@dataSpaceIds", projectIds.Select<Guid, int>((System.Func<Guid, int>) (x => this.GetDataspaceIdWithLazyInitialization(x))));
      this.BindInt("@batchSize", batchSize);
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
      {
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("ReleaseId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("ReleaseEnvId");
        valueTupleList.Add((sqlColumnBinder1.GetInt32((IDataReader) reader), sqlColumnBinder2.GetInt32((IDataReader) reader)));
      }
      return valueTupleList;
    }

    public override void DeleteBuildInProgressTestSignal(Guid projectId, int buildId)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteInProgressTestResultNotificationForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@buildId", buildId);
      this.ExecuteReader();
    }

    public override void DeleteReleaseInProgressTestSignal(
      Guid projectId,
      int releaseId,
      int environmentId)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteInProgressTestResultNotificationForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@releaseEnvId", environmentId);
      this.ExecuteReader();
    }

    public override (int TotalBuildSignalsToDelete, int TotalReleaseSignalsToDelete) DeleteInprogressTestResultSignals(
      Guid projectId,
      int batchSize,
      int retentionDays)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteInProgressTestResultNotifcations");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@batchSize", batchSize);
      this.BindInt("@retentionDays", retentionDays);
      int num1 = 0;
      int num2 = 0;
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
        num1 = new SqlColumnBinder("TotalBuildNotificationsToDelete").GetInt32((IDataReader) reader);
      if (reader.NextResult())
      {
        while (reader.Read())
          num2 = new SqlColumnBinder("TotalReleaseNotificationToDelete").GetInt32((IDataReader) reader);
      }
      return (num1, num2);
    }

    public override RunSummaryAndInsights QueryTestRunSummaryAndInsightsForBuild(
      GuidAndString projectId,
      string sourceWorkflow,
      BuildConfiguration buildRef,
      bool returnSummary,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount,
      out bool isBuildOld,
      int rundIdThreshold = 0)
    {
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryAndInsightsForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindInt("@buildId", buildRef != null ? buildRef.BuildId : 0);
      this.BindString("@buildUri", buildRef?.BuildUri, 256, true, SqlDbType.NVarChar);
      this.BindBoolean("@returnSummary", returnSummary);
      this.BindBoolean("@returnFailureDetails", returnFailureDetails);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          CurrentAggregatedRunsByState = (IList<RunSummaryByState>) new List<RunSummaryByState>(),
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      isBuildOld = false;
      if (reader.Read())
        isBuildOld = new SqlColumnBinder("IsBuildOld").GetBoolean((IDataReader) reader);
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      runsCount = 0;
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader.Read())
      {
        RunSummaryByState runSummaryByState = runsByStateColumns.bind(reader);
        runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runSummaryByState);
        runsCount += runSummaryByState.RunsCount;
      }
      if (runsCount > 0)
      {
        if (returnSummary)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (!isBuildOld)
          {
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
            while (reader.Read())
              runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          }
        }
        if (returnFailureDetails && !isBuildOld)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            runSummaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
        TestManagementDatabase19.PopulateAggregateDataByCategory(categoryName, reader, runSummaryAndInsights, "prc_QueryTestRunSummaryAndInsightsForBuild");
      }
      return runSummaryAndInsights;
    }

    public override RunSummaryAndInsights QueryTestRunSummaryAndInsightsForRelease(
      GuidAndString projectId,
      string sourceWorkflow,
      ReleaseReference releaseRef,
      bool returnSummary,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryAndInsightsForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindInt("@releaseId", releaseRef != null ? releaseRef.ReleaseId : 0);
      this.BindInt("@releaseEnvId", releaseRef != null ? releaseRef.ReleaseEnvId : 0);
      this.BindInt("@attempt", releaseRef != null ? releaseRef.Attempt : 0);
      this.BindBoolean("@returnSummary", returnSummary);
      this.BindBoolean("@returnFailureDetails", returnFailureDetails);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          CurrentAggregatedRunsByState = (IList<RunSummaryByState>) new List<RunSummaryByState>(),
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      runsCount = 0;
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader.Read())
      {
        RunSummaryByState runSummaryByState = runsByStateColumns.bind(reader);
        runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runSummaryByState);
        runsCount += runSummaryByState.RunsCount;
      }
      if (runsCount > 0)
      {
        if (returnSummary)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForRelease");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (reader.NextResult())
          {
            while (reader.Read())
              runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          }
        }
        if (returnFailureDetails)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForRelease");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            runSummaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
        TestManagementDatabase19.PopulateAggregateDataByCategory(categoryName, reader, runSummaryAndInsights, "prc_QueryTestRunSummaryAndInsightsForRelease");
      }
      return runSummaryAndInsights;
    }

    protected class QueryTestResultMaxSubResultIds
    {
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder MaxReservedSubResultId = new SqlColumnBinder("FieldValue");

      internal int bind(SqlDataReader reader, out int testResultId)
      {
        testResultId = this.TestResultId.GetInt32((IDataReader) reader);
        return this.MaxReservedSubResultId.GetInt32((IDataReader) reader, 0);
      }
    }
  }
}
