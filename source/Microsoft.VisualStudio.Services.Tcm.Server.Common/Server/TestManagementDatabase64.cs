// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase64
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase64 : TestManagementDatabase63
  {
    internal TestManagementDatabase64(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase64()
    {
    }

    internal override void MarkTestCaseRefsFlaky(
      Guid projectId,
      List<int> testRunIds,
      List<int> allowedPipelines)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.MarkTestCaseRefsFlaky");
        this.PrepareStoredProcedure("TestResult.prc_MarkTestCaseRefsFlaky");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindIdTypeTable("@testRunIds", (IEnumerable<int>) testRunIds);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.MarkTestCaseRefsFlaky");
      }
    }

    internal override List<PointsResults2> QueryManualTestResultsByUpdateDate(
      Guid projectId,
      int runBatchSize,
      int resultBatchSize,
      TestResultWatermark fromWatermark,
      out TestResultWatermark toWatermark)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryManualTestResultsByUpdateDate");
        List<PointsResults2> source = new List<PointsResults2>();
        this.PrepareStoredProcedure("TestManagement.prc_QueryManualTestResultsByTestRunChangedDate");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@runBatchSize", runBatchSize);
        this.BindInt("@resultBatchSize", resultBatchSize);
        this.BindDateTime("@fromRunChangedDate", fromWatermark.ChangedDate, true);
        this.BindInt("@fromRunId", fromWatermark != null ? fromWatermark.TestRunId : 0);
        this.BindInt("@fromResultId", fromWatermark != null ? fromWatermark.TestResultId : 0);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase64.FetchPointResults2 fetchPointResults2 = new TestManagementDatabase64.FetchPointResults2();
        DateTime runChangedDate = new DateTime();
        toWatermark = new TestResultWatermark()
        {
          ChangedDate = fromWatermark != null ? fromWatermark.ChangedDate : SqlDateTime.MinValue.Value,
          TestRunId = fromWatermark != null ? fromWatermark.TestRunId : 0,
          TestResultId = fromWatermark != null ? fromWatermark.TestResultId : 0
        };
        while (reader.Read())
          source.Add(fetchPointResults2.Bind(reader, out runChangedDate));
        if (reader.NextResult())
        {
          while (reader.Read())
            source.Add(fetchPointResults2.Bind(reader, out runChangedDate));
        }
        if (source.Any<PointsResults2>())
        {
          toWatermark.ChangedDate = runChangedDate;
          toWatermark.TestRunId = source.Last<PointsResults2>().LastTestRunId;
          toWatermark.TestResultId = source.Last<PointsResults2>().LastTestResultId;
        }
        return source;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryManualTestResultsByUpdateDate");
      }
    }

    public override TestResultsDetails GetTestResultsGroupDetails(
      Guid projectId,
      PipelineReference pipelineReference,
      IList<byte> runStates,
      bool shouldIncludeFailedAndAbortedResults,
      bool queryGroupSummaryForInProgress)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestRunDetails");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@pipelineId", pipelineReference.PipelineId);
      this.BindString("@stageName", pipelineReference.StageReference?.StageName, 256, true, SqlDbType.NVarChar);
      this.BindString("@phaseName", pipelineReference.PhaseReference?.PhaseName, 256, true, SqlDbType.NVarChar);
      this.BindString("@jobName", pipelineReference.JobReference?.JobName, 256, true, SqlDbType.NVarChar);
      this.BindTestManagement_TinyIntTypeTable("@runStates", (IEnumerable<byte>) runStates);
      this.BindBoolean("@shouldIncludeFailedAndAbortedResults", shouldIncludeFailedAndAbortedResults);
      this.BindBoolean("@queryGroupSummaryForInProgress", queryGroupSummaryForInProgress);
      TestResultsDetails resultsGroupDetails = new TestResultsDetails();
      resultsGroupDetails.GroupByField = "TestRun";
      resultsGroupDetails.ResultsForGroup = (IList<TestResultsDetailsForGroup>) new List<TestResultsDetailsForGroup>();
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> dictionary = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      Dictionary<string, TestResultsDetailsForGroup> resultsForGroupMap = new Dictionary<string, TestResultsDetailsForGroup>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (!reader.HasRows)
          return resultsGroupDetails;
        TestManagementDatabase64.TestResultsGroupRunDetailsColumns runDetailsColumns = new TestManagementDatabase64.TestResultsGroupRunDetailsColumns();
        while (reader.Read())
          runDetailsColumns.bind(reader, resultsForGroupMap, pipelineReference.PipelineId);
        if (reader.NextResult())
        {
          TestManagementDatabase64.TestResultsGroupAggregatedDetailsColumns aggregatedDetailsColumns = new TestManagementDatabase64.TestResultsGroupAggregatedDetailsColumns();
          while (reader.Read())
            aggregatedDetailsColumns.bind(reader, resultsForGroupMap);
        }
        if (reader.NextResult())
        {
          TestManagementDatabase64.TestResultsDetailsColumns resultsDetailsColumns = new TestManagementDatabase64.TestResultsDetailsColumns();
          while (reader.Read())
            resultsDetailsColumns.bind(reader, dictionary, projectId);
        }
        if (dictionary.Count<KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>() > 0)
        {
          foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier key in dictionary.Keys)
          {
            string id = dictionary[key].TestRun.Id;
            resultsForGroupMap[id].Results.Add(dictionary[key]);
          }
        }
      }
      resultsGroupDetails.ResultsForGroup = (IList<TestResultsDetailsForGroup>) resultsForGroupMap.Values.ToList<TestResultsDetailsForGroup>();
      return resultsGroupDetails;
    }

    public override RunSummaryAndInsights QueryTestRunSummaryAndInsightsForPipeline(
      GuidAndString projectId,
      PipelineReference pipelineReference,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestRunSummaryAndInsightsForPipeline");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindInt("@pipelineId", pipelineReference.PipelineId);
      this.BindString("@stageName", pipelineReference.StageReference?.StageName, 256, true, SqlDbType.NVarChar);
      this.BindString("@phaseName", pipelineReference.PhaseReference?.PhaseName, 256, true, SqlDbType.NVarChar);
      this.BindString("@jobName", pipelineReference.JobReference?.JobName, 256, true, SqlDbType.NVarChar);
      this.BindBoolean("@returnFailureDetails", returnFailureDetails);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      SqlDataReader reader1 = this.ExecuteReader();
      RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          CurrentAggregatedRunsByState = (IList<RunSummaryByState>) new List<RunSummaryByState>(),
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>(),
          PreviousAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      runsCount = 0;
      if (!reader1.HasRows)
        return runSummaryAndInsights;
      bool flag = true;
      if (reader1.Read())
        flag = new SqlColumnBinder("TestRunExists").GetBoolean((IDataReader) reader1);
      if (!flag)
        return runSummaryAndInsights;
      if (!reader1.NextResult())
        throw new UnexpectedDatabaseResultException("TestResult.prc_QueryTestRunSummaryAndInsightsForPipeline");
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
        throw new UnexpectedDatabaseResultException("TestResult.prc_QueryTestRunSummaryAndInsightsForPipeline");
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader1.Read())
      {
        RunSummaryByState runSummaryByState = runsByStateColumns.bind(reader1);
        runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runSummaryByState);
        runsCount += runSummaryByState.RunsCount;
      }
      if (runsCount > 0)
      {
        if (!reader1.NextResult())
          throw new UnexpectedDatabaseResultException("TestResult.prc_QueryTestRunSummaryAndInsightsForPipeline");
        TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
        while (reader1.Read())
          runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader1));
        if (!reader1.NextResult())
          throw new UnexpectedDatabaseResultException("TestResult.prc_QueryTestRunSummaryAndInsightsForPipeline");
        while (reader1.Read())
          runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader1));
        if (returnFailureDetails)
        {
          if (!reader1.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_QueryTestRunSummaryAndInsightsForPipeline");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader1.Read())
            runSummaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader1));
        }
        TestManagementDatabase19.PopulateAggregateDataByCategory(categoryName, reader1, runSummaryAndInsights, "TestResult.prc_QueryTestRunSummaryAndInsightsForPipeline");
      }
      return runSummaryAndInsights;
    }

    public override void UpdateFlakinessFieldForResults(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results)
    {
      this.PrepareStoredProcedure("TestResult.prc_UpdateFlakinessFieldForResults");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindTestResult_TestCaseResult2TypeTable("@testresultsTable", results);
      this.ExecuteReader();
    }

    protected class FetchPointResults2
    {
      private SqlColumnBinder RunChangedDate = new SqlColumnBinder(nameof (RunChangedDate));
      private TestManagementDatabase.FetchTestResultsColumns m_fetchTestResultsColumns;

      internal FetchPointResults2() => this.m_fetchTestResultsColumns = new TestManagementDatabase.FetchTestResultsColumns();

      internal PointsResults2 Bind(SqlDataReader reader, out DateTime runChangedDate)
      {
        TestCaseResult testCaseResult = this.m_fetchTestResultsColumns.bind(reader);
        runChangedDate = this.RunChangedDate.GetDateTime((IDataReader) reader);
        return new PointsResults2()
        {
          PlanId = testCaseResult.TestPlanId,
          LastTestRunId = testCaseResult.TestRunId,
          LastTestResultId = testCaseResult.TestResultId,
          LastResultOutcome = new byte?(testCaseResult.Outcome),
          LastUpdated = testCaseResult.DateCompleted,
          PointId = testCaseResult.TestPointId,
          LastResolutionStateId = testCaseResult.ResolutionStateId,
          LastFailureType = new byte?(testCaseResult.FailureType),
          LastResultState = new byte?(testCaseResult.State),
          LastUpdatedBy = testCaseResult.LastUpdatedBy
        };
      }
    }

    protected class TestResultsGroupRunDetailsColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestRunTitle = new SqlColumnBinder("Title");
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder StageName = new SqlColumnBinder(nameof (StageName));
      private SqlColumnBinder PhaseName = new SqlColumnBinder(nameof (PhaseName));
      private SqlColumnBinder JobName = new SqlColumnBinder(nameof (JobName));

      public void bind(
        SqlDataReader reader,
        Dictionary<string, TestResultsDetailsForGroup> resultsForGroupMap,
        int pipelineId)
      {
        int int32 = this.TestRunId.GetInt32((IDataReader) reader);
        string key = int32.ToString();
        string str1 = ((TestRunState) this.State.GetByte((IDataReader) reader)).ToString();
        DateTime dateTime = this.CompleteDate.GetDateTime((IDataReader) reader);
        PipelineReference pipelineReference = new PipelineReference()
        {
          PipelineId = pipelineId
        };
        string str2 = this.StageName.GetString((IDataReader) reader, true);
        string str3 = this.PhaseName.GetString((IDataReader) reader, true);
        string str4 = this.JobName.GetString((IDataReader) reader, true);
        if (!string.IsNullOrEmpty(str2))
          pipelineReference.StageReference = new StageReference()
          {
            StageName = str2
          };
        if (!string.IsNullOrEmpty(str3))
          pipelineReference.PhaseReference = new PhaseReference()
          {
            PhaseName = str3
          };
        if (!string.IsNullOrEmpty(str4))
          pipelineReference.JobReference = new JobReference()
          {
            JobName = str4
          };
        object obj = (object) new Microsoft.TeamFoundation.TestManagement.WebApi.TestRun()
        {
          Id = int32,
          Name = this.TestRunTitle.GetString((IDataReader) reader, false),
          State = str1,
          CompletedDate = dateTime,
          PipelineReference = pipelineReference
        };
        if (!resultsForGroupMap.ContainsKey(key))
          resultsForGroupMap[key] = new TestResultsDetailsForGroup()
          {
            Results = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(),
            ResultsCountByOutcome = (IDictionary<TestOutcome, AggregatedResultsByOutcome>) new Dictionary<TestOutcome, AggregatedResultsByOutcome>()
          };
        resultsForGroupMap[key].GroupByValue = obj;
      }
    }

    protected class TestResultsGroupAggregatedDetailsColumns
    {
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder AggregatedDuration = new SqlColumnBinder(nameof (AggregatedDuration));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));

      public void bind(
        SqlDataReader reader,
        Dictionary<string, TestResultsDetailsForGroup> resultsForGroupMap)
      {
        string key1 = this.TestRunId.GetInt32((IDataReader) reader).ToString();
        byte key2 = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader) : (byte) 0;
        int int32 = this.ResultCount.ColumnExists((IDataReader) reader) ? this.ResultCount.GetInt32((IDataReader) reader) : 0;
        long int64 = this.AggregatedDuration.ColumnExists((IDataReader) reader) ? this.AggregatedDuration.GetInt64((IDataReader) reader) : 0L;
        if (!resultsForGroupMap[key1].ResultsCountByOutcome.ContainsKey((TestOutcome) key2))
        {
          resultsForGroupMap[key1].ResultsCountByOutcome[(TestOutcome) key2] = new AggregatedResultsByOutcome()
          {
            Outcome = (TestOutcome) key2,
            Count = int32,
            Duration = TimeSpan.FromMilliseconds((double) int64)
          };
        }
        else
        {
          resultsForGroupMap[key1].ResultsCountByOutcome[(TestOutcome) key2].Count += int32;
          resultsForGroupMap[key1].ResultsCountByOutcome[(TestOutcome) key2].Duration = Validator.CheckOverflowAndGetSafeValue(resultsForGroupMap[key1].ResultsCountByOutcome[(TestOutcome) key2].Duration, TimeSpan.FromMilliseconds((double) int64));
        }
      }
    }

    protected class TestResultsDetailsColumns
    {
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder Priority = new SqlColumnBinder(nameof (Priority));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));

      public void bind(
        SqlDataReader reader,
        Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap,
        Guid ProjectId)
      {
        int num = this.Priority.ColumnExists((IDataReader) reader) ? (int) this.Priority.GetByte((IDataReader) reader) : (int) byte.MaxValue;
        int int32_1 = this.TestRunId.GetInt32((IDataReader) reader);
        int int32_2 = this.TestResultId.GetInt32((IDataReader) reader);
        int int32_3 = this.TestCaseRefId.ColumnExists((IDataReader) reader) ? this.TestCaseRefId.GetInt32((IDataReader) reader) : 0;
        string name = this.Outcome.ColumnExists((IDataReader) reader) ? Enum.GetName(typeof (TestOutcome), (object) this.Outcome.GetByte((IDataReader) reader)) : (string) null;
        long int64 = this.Duration.ColumnExists((IDataReader) reader) ? this.Duration.GetInt64((IDataReader) reader) : 0L;
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier key = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier(int32_1, int32_2);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
        {
          TestRun = new ShallowReference()
          {
            Id = int32_1.ToString()
          },
          Id = int32_2,
          TestCaseReferenceId = int32_3,
          Priority = num,
          Project = new ShallowReference()
          {
            Id = ProjectId.ToString()
          },
          Outcome = name,
          DurationInMs = (double) int64
        };
        resultsMap[key] = testCaseResult;
      }
    }
  }
}
