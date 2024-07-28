// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase19
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase19 : TestManagementDatabase18
  {
    internal TestManagementDatabase19(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase19()
    {
    }

    public override List<TestConfiguration> QueryTestConfigurationsWithPaging(
      Guid projectId,
      int skip,
      int top,
      int watermark,
      out List<KeyValuePair<string, TestConfiguration>> areaUris)
    {
      this.PrepareStoredProcedure("prc_QueryConfigurationsWithPaging");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@skip", skip);
      this.BindInt("@top", top);
      this.BindInt("@watermark", watermark);
      areaUris = new List<KeyValuePair<string, TestConfiguration>>();
      return this.GetTestConfigurationsFromReader(this.ExecuteReader(), areaUris);
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
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      runsCount = 0;
      isBuildOld = false;
      SqlColumnBinder sqlColumnBinder;
      if (reader.Read())
      {
        ref bool local = ref isBuildOld;
        sqlColumnBinder = new SqlColumnBinder("IsBuildOld");
        int num = sqlColumnBinder.GetBoolean((IDataReader) reader) ? 1 : 0;
        local = num != 0;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      if (reader.Read())
      {
        ref int local = ref runsCount;
        sqlColumnBinder = new SqlColumnBinder("RunsCount");
        int int32 = sqlColumnBinder.GetInt32((IDataReader) reader);
        local = int32;
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
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      runsCount = 0;
      if (reader.Read())
        runsCount = new SqlColumnBinder("RunsCount").GetInt32((IDataReader) reader);
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

    protected static void PopulateAggregateDataByCategory(
      string categoryName,
      SqlDataReader reader,
      RunSummaryAndInsights runSummaryAndInsights,
      string procedureName)
    {
      if (string.IsNullOrEmpty(categoryName) || !reader.NextResult())
        return;
      TestManagementDatabase19.FetchTestRunSummaryByCategoryColumns byCategoryColumns1 = new TestManagementDatabase19.FetchTestRunSummaryByCategoryColumns();
      runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>();
      while (reader.Read())
      {
        RunSummaryByCategory summaryByCategory = byCategoryColumns1.bind(reader);
        summaryByCategory.CategoryField = categoryName;
        runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByReportingCategory.Add(summaryByCategory);
      }
      if (!reader.NextResult())
        return;
      TestManagementDatabase19.FetchTestRunSummaryByCategoryColumns byCategoryColumns2 = new TestManagementDatabase19.FetchTestRunSummaryByCategoryColumns();
      runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>();
      while (reader.Read())
      {
        RunSummaryByCategory summaryByCategory = byCategoryColumns2.bind(reader);
        summaryByCategory.CategoryField = categoryName;
        runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByReportingCategory.Add(summaryByCategory);
      }
    }

    protected class FetchTestRunSummaryByCategoryColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder IntValue = new SqlColumnBinder(nameof (IntValue));
      private SqlColumnBinder FloatValue = new SqlColumnBinder(nameof (FloatValue));
      private SqlColumnBinder BitValue = new SqlColumnBinder(nameof (BitValue));
      private SqlColumnBinder DateTimeValue = new SqlColumnBinder(nameof (DateTimeValue));
      private SqlColumnBinder GuidValue = new SqlColumnBinder(nameof (GuidValue));
      private SqlColumnBinder StringValue = new SqlColumnBinder(nameof (StringValue));
      private SqlColumnBinder ResultCount = new SqlColumnBinder("ResultsCount");
      private SqlColumnBinder ResultDuration = new SqlColumnBinder(nameof (ResultDuration));

      internal RunSummaryByCategory bind(SqlDataReader reader)
      {
        RunSummaryByCategory summaryByCategory = new RunSummaryByCategory();
        summaryByCategory.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        summaryByCategory.TestOutcome = (TestOutcome) this.TestOutcome.GetByte((IDataReader) reader);
        summaryByCategory.CategoryValue = !this.IntValue.ColumnExists((IDataReader) reader) ? (!this.FloatValue.ColumnExists((IDataReader) reader) ? (!this.BitValue.ColumnExists((IDataReader) reader) ? (!this.DateTimeValue.ColumnExists((IDataReader) reader) ? (!this.GuidValue.ColumnExists((IDataReader) reader) ? (object) this.StringValue.GetString((IDataReader) reader, false) : (object) this.GuidValue.GetGuid((IDataReader) reader)) : (object) this.DateTimeValue.GetDateTime((IDataReader) reader)) : (object) this.BitValue.GetBoolean((IDataReader) reader)) : (object) (float) this.FloatValue.GetDouble((IDataReader) reader)) : (object) this.IntValue.GetInt32((IDataReader) reader);
        summaryByCategory.ResultCount = this.ResultCount.GetInt32((IDataReader) reader);
        summaryByCategory.ResultDuration = this.ResultDuration.ColumnExists((IDataReader) reader) ? this.ResultDuration.GetInt64((IDataReader) reader) : 0L;
        return summaryByCategory;
      }
    }
  }
}
