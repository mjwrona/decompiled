// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase58
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
  public class TestManagementDatabase58 : TestManagementDatabase57
  {
    internal TestManagementDatabase58(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase58()
    {
    }

    internal override List<TestResultExArchivalRecord> QueryTestResultExtensionsByTestRunChangedDate(
      int dataspaceId,
      int runBatchSize,
      int resultExBatchSize,
      TestResultExArchivalWatermark fromWatermark,
      DateTime maxTestRunUpdatedDate,
      out TestResultExArchivalWatermark toWatermark,
      TestArtifactSource dataSource,
      List<string> fieldNames = null,
      List<int> runStates = null,
      List<int> excludedRunTypes = null)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultExtensionsByTestRunChangedDate");
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindInt("@runBatchSize", runBatchSize);
      this.BindInt("@resultExBatchSize", resultExBatchSize);
      this.BindDateTime("@fromRunChangedDate", fromWatermark != null ? fromWatermark.TestRunUpdatedDate : new DateTime(), true);
      this.BindDateTime("@toRunChangedDate", maxTestRunUpdatedDate, true);
      this.BindInt("@fromRunId", fromWatermark != null ? fromWatermark.TestRunId : 0);
      this.BindInt("@fromResultId", fromWatermark != null ? fromWatermark.TestResultId : 0);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase58.FetchTestResultExArchivalRecord exArchivalRecord1 = new TestManagementDatabase58.FetchTestResultExArchivalRecord();
      TestManagementDatabase58.FetchFieldExMappingColumn fieldExMappingColumn = new TestManagementDatabase58.FetchFieldExMappingColumn();
      Dictionary<int, string> mapping = new Dictionary<int, string>();
      mapping[0] = (string) null;
      toWatermark = new TestResultExArchivalWatermark()
      {
        TestRunUpdatedDate = fromWatermark != null ? fromWatermark.TestRunUpdatedDate : new DateTime(),
        TestRunId = fromWatermark != null ? fromWatermark.TestRunId : 0,
        TestResultId = fromWatermark != null ? fromWatermark.TestResultId : 0
      };
      List<TestResultExArchivalRecord> source = new List<TestResultExArchivalRecord>();
      List<TestResultExArchivalRecord> collection = new List<TestResultExArchivalRecord>();
      TestResultExArchivalRecord record = (TestResultExArchivalRecord) null;
      bool flag = false;
      Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
      while (reader.Read())
      {
        KeyValue<int, string> keyValue = fieldExMappingColumn.Bind(reader);
        mapping[keyValue.Key] = keyValue.Value;
      }
      if (reader.NextResult())
      {
        while (reader.Read())
          collection.Add(exArchivalRecord1.Bind(reader, (IDictionary<int, string>) mapping, dataspaceIdentifier));
      }
      if (reader.NextResult())
      {
        while (reader.Read())
        {
          TestResultExArchivalRecord exArchivalRecord2 = exArchivalRecord1.Bind(reader, (IDictionary<int, string>) mapping, dataspaceIdentifier);
          flag = flag || exArchivalRecord2.TestRunId == (fromWatermark != null ? fromWatermark.TestRunId : 0);
          source.Add(exArchivalRecord2);
        }
      }
      if (!flag)
        source.InsertRange(0, (IEnumerable<TestResultExArchivalRecord>) collection);
      if (reader.NextResult() && reader.Read())
        record = exArchivalRecord1.Bind(reader, (IDictionary<int, string>) mapping, dataspaceIdentifier);
      toWatermark = source.Any<TestResultExArchivalRecord>() || record != null ? (record != null ? new TestResultExArchivalWatermark(record) : new TestResultExArchivalWatermark(source.Last<TestResultExArchivalRecord>())) : new TestResultExArchivalWatermark(maxTestRunUpdatedDate, 0, 0);
      return source;
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
      this.BindInt("@runIdThreshold", runIdThreshold);
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
      if (reader.Read())
      {
        int totalRunsCount;
        int noConfigRunsCount;
        new TestManagementDatabase58.FetchTestRunsCount().bind(reader, out totalRunsCount, out noConfigRunsCount);
        runSummaryAndInsights.TestRunSummary.TotalRunsCount = totalRunsCount;
        runSummaryAndInsights.TestRunSummary.NoConfigRunsCount = noConfigRunsCount;
      }
      runsCount = 0;
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      if (reader.NextResult())
      {
        while (reader.Read())
        {
          RunSummaryByState runSummaryByState = runsByStateColumns.bind(reader);
          runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runSummaryByState);
          runsCount += runSummaryByState.RunsCount;
        }
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
      this.BindInt("@runIdThreshold", rundIdThreshold);
      SqlDataReader reader1 = this.ExecuteReader();
      bool flag = true;
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
      runsCount = 0;
      SqlColumnBinder sqlColumnBinder;
      if (reader1.Read())
      {
        sqlColumnBinder = new SqlColumnBinder("IsRunsAvailable");
        flag = sqlColumnBinder.GetBoolean((IDataReader) reader1);
      }
      if (!flag)
        return runSummaryAndInsights;
      if (!reader1.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      if (reader1.Read())
      {
        ref bool local = ref isBuildOld;
        sqlColumnBinder = new SqlColumnBinder("IsBuildOld");
        int num = sqlColumnBinder.GetBoolean((IDataReader) reader1) ? 1 : 0;
        local = num != 0;
      }
      if (!reader1.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      TestManagementDatabase58.FetchTestRunsCount fetchTestRunsCount = new TestManagementDatabase58.FetchTestRunsCount();
      reader1.Read();
      SqlDataReader reader2 = reader1;
      int num1;
      ref int local1 = ref num1;
      int num2;
      ref int local2 = ref num2;
      fetchTestRunsCount.bind(reader2, out local1, out local2);
      runSummaryAndInsights.TestRunSummary.TotalRunsCount = num1;
      runSummaryAndInsights.TestRunSummary.NoConfigRunsCount = num2;
      if (!reader1.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader1.Read())
      {
        RunSummaryByState runSummaryByState = runsByStateColumns.bind(reader1);
        runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runSummaryByState);
        runsCount += runSummaryByState.RunsCount;
      }
      if (runsCount > 0)
      {
        if (returnSummary)
        {
          if (!reader1.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader1.Read())
            runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader1));
          if (!isBuildOld)
          {
            if (!reader1.NextResult())
              throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
            while (reader1.Read())
              runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader1));
          }
        }
        if (returnFailureDetails && !isBuildOld)
        {
          if (!reader1.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader1.Read())
            runSummaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader1));
        }
        TestManagementDatabase19.PopulateAggregateDataByCategory(categoryName, reader1, runSummaryAndInsights, "prc_QueryTestRunSummaryAndInsightsForBuild");
      }
      return runSummaryAndInsights;
    }

    protected class FetchTestResultExArchivalRecord
    {
      private SqlColumnBinder TestRunUpdatedDate = new SqlColumnBinder(nameof (TestRunUpdatedDate));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder FieldId = new SqlColumnBinder(nameof (FieldId));
      private SqlColumnBinder FieldValue = new SqlColumnBinder(nameof (FieldValue));

      internal TestResultExArchivalRecord Bind(
        SqlDataReader reader,
        IDictionary<int, string> mapping,
        Guid projectId)
      {
        return new TestResultExArchivalRecord()
        {
          TestRunUpdatedDate = this.TestRunUpdatedDate.GetDateTime((IDataReader) reader),
          TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
          TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
          FieldName = mapping[this.FieldId.GetInt32((IDataReader) reader)],
          FieldValue = this.FieldValue.GetString((IDataReader) reader, true),
          ProjectGuid = projectId
        };
      }
    }

    protected class FetchFieldExMappingColumn
    {
      private SqlColumnBinder FieldId = new SqlColumnBinder(nameof (FieldId));
      private SqlColumnBinder FieldName = new SqlColumnBinder(nameof (FieldName));

      internal KeyValue<int, string> Bind(SqlDataReader reader) => new KeyValue<int, string>(this.FieldId.GetInt32((IDataReader) reader), this.FieldName.GetString((IDataReader) reader, false));
    }

    protected class FetchTestRunsCount
    {
      private SqlColumnBinder TotalRunsCountBinder = new SqlColumnBinder("TotalRunsCount");
      private SqlColumnBinder NoConfigRunsCountBinder = new SqlColumnBinder("NoConfigRunsCount");

      internal void bind(SqlDataReader reader, out int totalRunsCount, out int noConfigRunsCount)
      {
        totalRunsCount = this.TotalRunsCountBinder.GetInt32((IDataReader) reader, 0);
        noConfigRunsCount = this.NoConfigRunsCountBinder.GetInt32((IDataReader) reader, 0);
      }
    }
  }
}
