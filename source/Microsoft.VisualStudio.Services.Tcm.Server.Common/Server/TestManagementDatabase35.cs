// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase35
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase35 : TestManagementDatabase34
  {
    internal TestManagementDatabase35(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase35()
    {
    }

    internal override TestResultsGroupsData GetTestResultAutomatedTestStorageAndOwnersByBuild(
      Guid projectId,
      int buildId,
      string publishContext,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsGroupsByBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", publishContext, 128, true, SqlDbType.NVarChar);
      this.BindInt("@buildId", buildId);
      SqlDataReader reader = this.ExecuteReader();
      TestResultsGroupsData andOwnersByBuild = new TestResultsGroupsData();
      TestResultsGroupsData testResultsGroupsData = andOwnersByBuild;
      TestManagementDatabase35.GetTestResultsGroupResponse(reader, testResultsGroupsData);
      return andOwnersByBuild;
    }

    internal override TestResultsGroupsData GetTestResultAutomatedTestStorageAndOwnersByRelease(
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsGroupsByRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", publishContext, 128, true, SqlDbType.NVarChar);
      this.BindNullableInt("@releaseId", releaseId, 0);
      this.BindNullableInt("@releaseEnvId", releaseEnvId, 0);
      SqlDataReader reader = this.ExecuteReader();
      TestResultsGroupsData andOwnersByRelease = new TestResultsGroupsData();
      TestResultsGroupsData testResultsGroupsData = andOwnersByRelease;
      TestManagementDatabase35.GetTestResultsGroupResponse(reader, testResultsGroupsData);
      return andOwnersByRelease;
    }

    private static void GetTestResultsGroupResponse(
      SqlDataReader reader,
      TestResultsGroupsData testResultsGroupsData)
    {
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("AutomatedTestStorage");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("Owner");
      while (reader.Read())
      {
        testResultsGroupsData.AutomatedTestStorage.Add(sqlColumnBinder1.GetString((IDataReader) reader, true));
        testResultsGroupsData.Owner.Add(sqlColumnBinder2.GetString((IDataReader) reader, true));
      }
    }

    public override void FetchTestFailureDetails(
      GuidAndString projectId,
      List<int> testRunIds,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      string sourceWorkflow,
      bool fetchPreviousFailedResults,
      bool shouldByPassFlaky,
      out Dictionary<int, List<TestCaseResult>> currentFailedResults,
      out Dictionary<int, TestCaseResult> previousFailedResultsMap,
      out int prevTestRunContextId)
    {
      currentFailedResults = testRunIds.ToDictionary<int, int, List<TestCaseResult>>((System.Func<int, int>) (testRunId => testRunId), (System.Func<int, List<TestCaseResult>>) (testRunId => new List<TestCaseResult>()));
      previousFailedResultsMap = new Dictionary<int, TestCaseResult>();
      prevTestRunContextId = 0;
      foreach (int num in (IEnumerable<int>) testRunIds ?? Enumerable.Empty<int>())
      {
        List<TestCaseResult> currentFailedResults1;
        this.FetchTestFailureDetails(projectId, num, buildToCompare, releaseToCompare, out currentFailedResults1, out previousFailedResultsMap, out prevTestRunContextId);
        currentFailedResults[num] = currentFailedResults1;
      }
    }

    public override void PublishTestSummaryAndFailureDetails(
      GuidAndString projectId,
      List<int> testRunIds,
      Dictionary<int, ResultInsights> resultInsights,
      Dictionary<int, Dictionary<int, string>> failingSinceDetails,
      List<TestResultIdentifierRecord> flakyResults,
      bool includeFailureDetails,
      bool publishPassCountOnly = false,
      bool shouldPublishFlakiness = false,
      bool shouldByPassFlaky = false)
    {
      foreach (int num in (IEnumerable<int>) testRunIds ?? Enumerable.Empty<int>())
        this.PublishTestSummaryAndFailureDetails(projectId, num, resultInsights[num], failingSinceDetails[num], includeFailureDetails);
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild3(
      Guid projectId,
      TestResultTrendFilter filter,
      bool calculateEffectiveRunDuration,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultTrendForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", filter.PublishContext, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@definitionIds", filter.DefinitionIds != null ? filter.DefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindBranchNameTypeTable("@branchNames", (IEnumerable<string>) filter.BranchNames);
      this.BindNameTypeTable("@runTitles", (IEnumerable<string>) filter.TestRunTitles);
      this.BindInt("@buildCount", filter.BuildCount);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      TestManagementDatabase35.QueryAggregatedResultsForBuild3 resultsForBuild3 = new TestManagementDatabase35.QueryAggregatedResultsForBuild3();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      Dictionary<int, SortedSet<RunSummaryByOutcome>> executionDetailsForTestRuns = new Dictionary<int, SortedSet<RunSummaryByOutcome>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultsForBuild3.bindAggregatedResultsForBuild(reader, aggregatedResultsMap, executionDetailsForTestRuns);
      }
      foreach (KeyValuePair<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> keyValuePair in aggregatedResultsMap)
        keyValuePair.Value.Item2.Duration = TimeSpan.FromMilliseconds((double) TestManagementServiceUtility.CalculateEffectiveTestRunDuration(executionDetailsForTestRuns[keyValuePair.Key], calculateEffectiveRunDuration));
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease3(
      Guid projectId,
      TestResultTrendFilter filter,
      bool calculateEffectiveRunDuration,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultTrendForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", filter.PublishContext, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@definitionIds", filter.DefinitionIds != null ? filter.DefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindIdTypeTable("@envDefinitionIds", filter.EnvDefinitionIds != null ? filter.EnvDefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindNameTypeTable("@runTitles", (IEnumerable<string>) filter.TestRunTitles);
      this.BindInt("@releaseCount", filter.BuildCount);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      TestManagementDatabase35.QueryAggregatedResultsForRelease3 resultsForRelease3 = new TestManagementDatabase35.QueryAggregatedResultsForRelease3();
      Dictionary<int, SortedSet<RunSummaryByOutcome>> executionDetailsForTestRuns = new Dictionary<int, SortedSet<RunSummaryByOutcome>>();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultsForRelease3.bindAggregatedResultsForRelease(reader, aggregatedResultsMap, executionDetailsForTestRuns);
      }
      foreach (KeyValuePair<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> keyValuePair in aggregatedResultsMap)
        keyValuePair.Value.Item2.Duration = TimeSpan.FromMilliseconds((double) TestManagementServiceUtility.CalculateEffectiveTestRunDuration(executionDetailsForTestRuns[keyValuePair.Key], calculateEffectiveRunDuration));
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
    }

    protected class QueryAggregatedResultsForBuild3
    {
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder DefinitionId = new SqlColumnBinder("BuildDefinitionId");
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder RunDuration = new SqlColumnBinder(nameof (RunDuration));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));

      internal void bindAggregatedResultsForBuild(
        SqlDataReader reader,
        Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap,
        Dictionary<int, SortedSet<RunSummaryByOutcome>> executionDetailsForTestRuns)
      {
        int int32_1 = this.BuildId.GetInt32((IDataReader) reader);
        string str1 = this.BuildUri.GetString((IDataReader) reader, false);
        string str2 = this.BuildNumber.GetString((IDataReader) reader, true);
        int int32_2 = this.DefinitionId.GetInt32((IDataReader) reader);
        string str3 = this.BranchName.GetString((IDataReader) reader, true);
        TestOutcome key = (TestOutcome) this.TestOutcome.GetByte((IDataReader) reader);
        int int32_3 = this.ResultCount.GetInt32((IDataReader) reader);
        int int32_4 = this.TestRunId.GetInt32((IDataReader) reader);
        long int64 = Convert.ToInt64(this.RunDuration.GetObject((IDataReader) reader));
        DateTime dateTime = this.CompleteDate.GetDateTime((IDataReader) reader);
        if (!aggregatedResultsMap.ContainsKey(int32_1))
        {
          aggregatedResultsMap[int32_1] = new Tuple<HashSet<int>, AggregatedDataForResultTrend>(new HashSet<int>(), new AggregatedDataForResultTrend()
          {
            TestResultsContext = new TestResultsContext()
            {
              ContextType = TestResultsContextType.Build,
              Build = new BuildReference()
              {
                Id = int32_1,
                Uri = str1,
                Number = str2,
                DefinitionId = int32_2,
                BranchName = str3
              }
            },
            ResultsByOutcome = (IDictionary<TestOutcome, AggregatedResultsByOutcome>) new Dictionary<TestOutcome, AggregatedResultsByOutcome>(),
            Duration = new TimeSpan()
          });
          executionDetailsForTestRuns[int32_1] = new SortedSet<RunSummaryByOutcome>((IComparer<RunSummaryByOutcome>) new TestRunComparer());
        }
        if (!aggregatedResultsMap[int32_1].Item2.ResultsByOutcome.ContainsKey(key))
          aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key] = new AggregatedResultsByOutcome()
          {
            Count = 0,
            Outcome = key
          };
        aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key].Count += int32_3;
        if (aggregatedResultsMap[int32_1].Item1.Contains(int32_4))
          return;
        aggregatedResultsMap[int32_1].Item1.Add(int32_4);
        executionDetailsForTestRuns[int32_1].Add(new RunSummaryByOutcome()
        {
          TestRunId = int32_4,
          RunDuration = int64,
          RunCompletedDate = dateTime
        });
      }
    }

    protected class QueryAggregatedResultsForRelease3
    {
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));
      private SqlColumnBinder DefinitionId = new SqlColumnBinder("ReleaseDefId");
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder RunDuration = new SqlColumnBinder(nameof (RunDuration));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));

      internal void bindAggregatedResultsForRelease(
        SqlDataReader reader,
        Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap,
        Dictionary<int, SortedSet<RunSummaryByOutcome>> executionDetailsForTestRuns)
      {
        int int32_1 = this.ReleaseId.GetInt32((IDataReader) reader);
        string str = this.ReleaseName.GetString((IDataReader) reader, true);
        int int32_2 = this.DefinitionId.GetInt32((IDataReader) reader);
        TestOutcome key = (TestOutcome) this.TestOutcome.GetByte((IDataReader) reader);
        int int32_3 = this.ResultCount.GetInt32((IDataReader) reader);
        int int32_4 = this.TestRunId.GetInt32((IDataReader) reader);
        long int64 = Convert.ToInt64(this.RunDuration.GetObject((IDataReader) reader));
        DateTime dateTime = this.CompleteDate.GetDateTime((IDataReader) reader);
        if (!aggregatedResultsMap.ContainsKey(int32_1))
        {
          aggregatedResultsMap[int32_1] = new Tuple<HashSet<int>, AggregatedDataForResultTrend>(new HashSet<int>(), new AggregatedDataForResultTrend()
          {
            TestResultsContext = new TestResultsContext()
            {
              ContextType = TestResultsContextType.Release,
              Release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
              {
                Id = int32_1,
                Name = str,
                DefinitionId = int32_2
              }
            },
            ResultsByOutcome = (IDictionary<TestOutcome, AggregatedResultsByOutcome>) new Dictionary<TestOutcome, AggregatedResultsByOutcome>(),
            Duration = new TimeSpan()
          });
          executionDetailsForTestRuns[int32_1] = new SortedSet<RunSummaryByOutcome>((IComparer<RunSummaryByOutcome>) new TestRunComparer());
        }
        if (!aggregatedResultsMap[int32_1].Item2.ResultsByOutcome.ContainsKey(key))
          aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key] = new AggregatedResultsByOutcome()
          {
            Count = 0,
            Outcome = key
          };
        aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key].Count += int32_3;
        if (aggregatedResultsMap[int32_1].Item1.Contains(int32_4))
          return;
        aggregatedResultsMap[int32_1].Item1.Add(int32_4);
        executionDetailsForTestRuns[int32_1].Add(new RunSummaryByOutcome()
        {
          TestRunId = int32_4,
          RunDuration = int64,
          RunCompletedDate = dateTime
        });
      }
    }
  }
}
