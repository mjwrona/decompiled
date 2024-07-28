// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase23
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
  public class TestManagementDatabase23 : TestManagementDatabase22
  {
    internal TestManagementDatabase23(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase23()
    {
    }

    public override void UpdateTestCaseReference(
      Guid projectId,
      IEnumerable<TestCaseResult> results)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.UpdateTestCaseReference");
        this.PrepareStoredProcedure("TestResult.prc_UpdateTestCaseReference");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindTestResult_TestCaseReferenceTypeTableForUpdate("@testCaseReferenceTable", results);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.UpdateTestCaseReference");
      }
    }

    public override List<TestCaseReference> QueryTestCaseReference(
      Guid projectId,
      List<string> automatedTestNames,
      List<int> testCaseIds,
      int planId,
      List<int> suiteIds,
      List<int> pointIds)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestCaseReference");
        this.PrepareStoredProcedure("TestResult.prc_QueryTestCaseReference");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindNameTypeTable("@automatedTestNames", (IEnumerable<string>) automatedTestNames);
        this.BindInt32TypeTable("@testCaseIds", (IEnumerable<int>) testCaseIds);
        this.BindInt("@planId", planId);
        this.BindInt32TypeTable("@suiteIds", (IEnumerable<int>) suiteIds);
        this.BindInt32TypeTable("@pointIds", (IEnumerable<int>) pointIds);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase23.TestCaseReferenceColumns referenceColumns = new TestManagementDatabase23.TestCaseReferenceColumns();
        List<TestCaseReference> testCaseReferenceList = new List<TestCaseReference>();
        while (reader.Read())
          testCaseReferenceList.Add(referenceColumns.bind(reader));
        return testCaseReferenceList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestCaseReference");
      }
    }

    public override TestResultHistory QueryTestCaseResultHistory2(
      Guid projectId,
      ResultsFilter filter,
      int runIdThreshold = 0)
    {
      string groupBy = filter.GroupBy;
      int definitionId = 0;
      byte contextTypeFromFilter = this.GetResultContextTypeFromFilter(filter, out definitionId);
      if (groupBy.Equals("Branch", StringComparison.OrdinalIgnoreCase))
        this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsTrendByBranch");
      else if (groupBy.Equals("Environment", StringComparison.OrdinalIgnoreCase))
        this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsTrendByEnvironment");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@automatedTestName", filter.AutomatedTestName, 256, false, SqlDbType.NVarChar);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      if (groupBy.Equals("Branch", StringComparison.OrdinalIgnoreCase))
      {
        this.BindInt("@buildDays", 15);
        this.BindByte("@testResultContextType", contextTypeFromFilter);
      }
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindInt("@definitionId", definitionId);
      if (groupBy.Equals("Environment", StringComparison.OrdinalIgnoreCase))
        this.BindString("@branchName", filter.Branch, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      TestResultHistory testResultHistory = new TestResultHistory();
      testResultHistory.GroupByField = groupBy;
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap = new Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      Dictionary<string, TestResultHistoryDetailsForGroup> aggregatedResultsMap = new Dictionary<string, TestResultHistoryDetailsForGroup>();
      TestManagementDatabase23.QueryTestCaseResultHistoryColumns2 resultHistoryColumns2_1 = new TestManagementDatabase23.QueryTestCaseResultHistoryColumns2();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultHistoryColumns2_1.bindResultsMap(reader, resultsMap);
        if (reader.NextResult())
        {
          TestManagementDatabase23.QueryTestCaseResultHistoryColumns2 resultHistoryColumns2_2 = new TestManagementDatabase23.QueryTestCaseResultHistoryColumns2();
          while (reader.Read())
            resultHistoryColumns2_2.bindGroupedResultHistory2(reader, aggregatedResultsMap, resultsMap, groupBy);
        }
      }
      testResultHistory.ResultsForGroup = (IList<TestResultHistoryDetailsForGroup>) aggregatedResultsMap.Values.ToList<TestResultHistoryDetailsForGroup>();
      return testResultHistory;
    }

    protected byte GetResultContextTypeFromFilter(ResultsFilter filter, out int definitionId)
    {
      byte contextTypeFromFilter = 0;
      definitionId = 0;
      if (filter.TestResultsContext != null)
      {
        if (filter.TestResultsContext.ContextType == TestResultsContextType.Build)
        {
          definitionId = filter.TestResultsContext.Build.DefinitionId;
          contextTypeFromFilter = (byte) 1;
        }
        else if (filter.TestResultsContext.ContextType == TestResultsContextType.Release)
        {
          definitionId = filter.TestResultsContext.Release.DefinitionId;
          contextTypeFromFilter = (byte) 2;
        }
      }
      return contextTypeFromFilter;
    }

    public override void FetchTestFailureDetails(
      GuidAndString projectId,
      int testRunId,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      out List<TestCaseResult> currentFailedResults,
      out Dictionary<int, TestCaseResult> previousFailedResultsMap,
      out int prevTestRunContextId)
    {
      currentFailedResults = new List<TestCaseResult>();
      previousFailedResultsMap = new Dictionary<int, TestCaseResult>();
      this.PrepareStoredProcedure("TestResult.prc_FetchTestFailureDetails");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindInt("@testRunId", testRunId);
      List<BuildConfiguration> builds;
      if (buildToCompare != null)
        builds = new List<BuildConfiguration>()
        {
          buildToCompare
        };
      else
        builds = (List<BuildConfiguration>) null;
      this.BindBuildRefTypeTable3("@buildToCompare", (IEnumerable<BuildConfiguration>) builds);
      List<ReleaseReference> releases;
      if (releaseToCompare != null)
        releases = new List<ReleaseReference>()
        {
          releaseToCompare
        };
      else
        releases = (List<ReleaseReference>) null;
      this.BindReleaseRefTypeTable2("@releaseToCompare", (IEnumerable<ReleaseReference>) releases);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase23.FetchTestResultFailuresColumns resultFailuresColumns1 = new TestManagementDatabase23.FetchTestResultFailuresColumns();
      prevTestRunContextId = 0;
      while (reader.Read())
      {
        TestCaseResult testCaseResult = resultFailuresColumns1.bind(reader);
        previousFailedResultsMap[testCaseResult.TestCaseReferenceId] = testCaseResult;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestFailureDetails");
      TestManagementDatabase23.FetchTestResultFailuresColumns resultFailuresColumns2 = new TestManagementDatabase23.FetchTestResultFailuresColumns();
      while (reader.Read())
        currentFailedResults.Add(resultFailuresColumns2.bind(reader));
      if (!reader.NextResult() || !reader.Read())
        return;
      prevTestRunContextId = new SqlColumnBinder("TestRunContextId").GetInt32((IDataReader) reader, 0);
    }

    public override void PublishTestSummaryAndFailureDetails(
      GuidAndString projectId,
      int testRunId,
      ResultInsights resultInsights,
      Dictionary<int, string> failingSinceDetails,
      bool includeFailureDetails)
    {
      this.PrepareStoredProcedure("TestResult.prc_PublishTestSummaryAndFailureDetails");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@newFailures", resultInsights != null ? resultInsights.NewFailures : 0);
      this.BindStringPreserveNull("@newFailedResults", resultInsights?.NewFailedResults, int.MaxValue, SqlDbType.VarChar);
      this.BindInt("@existingFailures", resultInsights != null ? resultInsights.ExistingFailures : 0);
      this.BindStringPreserveNull("@existingFailedResults", resultInsights?.ExistingFailedResults, int.MaxValue, SqlDbType.VarChar);
      this.BindTestResult_FailingSinceDetailsTable("@failingSinceDetails", failingSinceDetails);
      this.BindBoolean("@updateFailureDetails", includeFailureDetails);
      this.ExecuteNonQuery();
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild2(
      Guid projectId,
      TestResultTrendFilter filter)
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
      TestManagementDatabase23.QueryAggregatedResultsForBuild2 resultsForBuild2 = new TestManagementDatabase23.QueryAggregatedResultsForBuild2();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultsForBuild2.bindAggregatedResultsForBuild(reader, aggregatedResultsMap);
      }
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease2(
      Guid projectId,
      TestResultTrendFilter filter)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultTrendForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", filter.PublishContext, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@definitionIds", filter.DefinitionIds != null ? filter.DefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindNameTypeTable("@runTitles", (IEnumerable<string>) filter.TestRunTitles);
      this.BindInt("@releaseCount", filter.BuildCount);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      TestManagementDatabase23.QueryAggregatedResultsForRelease2 resultsForRelease2 = new TestManagementDatabase23.QueryAggregatedResultsForRelease2();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultsForRelease2.bindAggregatedResultsForRelease(reader, aggregatedResultsMap);
      }
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
    }

    protected new class TestCaseReferenceColumns
    {
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder AutomatedTestStorage = new SqlColumnBinder(nameof (AutomatedTestStorage));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));

      internal TestCaseReference bind(SqlDataReader reader) => new TestCaseReference()
      {
        TestCaseReferenceId = this.TestCaseRefId.GetInt32((IDataReader) reader),
        TestCaseId = this.TestCaseId.ColumnExists((IDataReader) reader) ? this.TestCaseId.GetInt32((IDataReader) reader) : 0,
        AutomatedTestName = this.AutomatedTestName.ColumnExists((IDataReader) reader) ? this.AutomatedTestName.GetString((IDataReader) reader, false) : string.Empty,
        AutomatedTestStorage = this.AutomatedTestStorage.ColumnExists((IDataReader) reader) ? this.AutomatedTestStorage.GetString((IDataReader) reader, false) : string.Empty,
        PlanId = this.PlanId.ColumnExists((IDataReader) reader) ? this.PlanId.GetInt32((IDataReader) reader) : 0,
        SuiteId = this.SuiteId.ColumnExists((IDataReader) reader) ? this.SuiteId.GetInt32((IDataReader) reader) : 0,
        PointId = this.PointId.ColumnExists((IDataReader) reader) ? this.PointId.GetInt32((IDataReader) reader) : 0
      };
    }

    protected class QueryTestCaseResultHistoryColumns2
    {
      private SqlColumnBinder TestRunContextId = new SqlColumnBinder(nameof (TestRunContextId));
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder ReleaseEnvDefId = new SqlColumnBinder(nameof (ReleaseEnvDefId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));
      private SqlColumnBinder FailingSince = new SqlColumnBinder(nameof (FailingSince));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));

      internal void bindResultsMap(SqlDataReader reader, Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap)
      {
        int int32_1 = this.TestRunContextId.ColumnExists((IDataReader) reader) ? this.TestRunContextId.GetInt32((IDataReader) reader) : 0;
        int int32_2 = this.TestRunId.ColumnExists((IDataReader) reader) ? this.TestRunId.GetInt32((IDataReader) reader) : 0;
        int int32_3 = this.TestResultId.ColumnExists((IDataReader) reader) ? this.TestResultId.GetInt32((IDataReader) reader) : 0;
        DateTime dateTime1 = this.DateStarted.ColumnExists((IDataReader) reader) ? this.DateStarted.GetDateTime((IDataReader) reader) : new DateTime();
        DateTime dateTime2 = this.DateCompleted.ColumnExists((IDataReader) reader) ? this.DateCompleted.GetDateTime((IDataReader) reader) : new DateTime();
        double num1 = 0.0;
        if (!dateTime1.Equals(new DateTime()) && !dateTime2.Equals(new DateTime()))
          num1 = (dateTime2 - dateTime1).TotalMilliseconds;
        byte num2 = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader) : (byte) 0;
        string jsonString = this.FailingSince.GetString((IDataReader) reader, true);
        FailingSince convertedObject = (FailingSince) null;
        if (!string.IsNullOrEmpty(jsonString))
          TestResultHelper.TryJsonConvertWithRetry<FailingSince>(jsonString, out convertedObject, true);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
        {
          TestRun = new ShallowReference()
          {
            Id = int32_2.ToString()
          },
          Id = int32_3,
          CompletedDate = dateTime2,
          DurationInMs = num1,
          Outcome = ((TestOutcome) num2).ToString(),
          FailingSince = convertedObject
        };
        resultsMap[int32_1] = testCaseResult;
      }

      internal void bindGroupedResultHistory2(
        SqlDataReader reader,
        Dictionary<string, TestResultHistoryDetailsForGroup> aggregatedResultsMap,
        Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap,
        string groupByFieldString)
      {
        int int32_1 = this.TestRunContextId.ColumnExists((IDataReader) reader) ? this.TestRunContextId.GetInt32((IDataReader) reader) : 0;
        string str1 = this.BranchName.ColumnExists((IDataReader) reader) ? this.BranchName.GetString((IDataReader) reader, true) : string.Empty;
        int int32_2 = this.ReleaseEnvDefId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvDefId.GetInt32((IDataReader) reader) : 0;
        int int32_3 = this.BuildId.ColumnExists((IDataReader) reader) ? this.BuildId.GetInt32((IDataReader) reader, 0) : 0;
        string str2 = this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : string.Empty;
        int int32_4 = this.ReleaseId.ColumnExists((IDataReader) reader) ? this.ReleaseId.GetInt32((IDataReader) reader, 0) : 0;
        string str3 = this.ReleaseName.ColumnExists((IDataReader) reader) ? this.ReleaseName.GetString((IDataReader) reader, true) : string.Empty;
        string key = string.Empty;
        object obj = (object) string.Empty;
        if (!string.IsNullOrEmpty(groupByFieldString))
        {
          if (string.Equals(groupByFieldString, "Branch", StringComparison.OrdinalIgnoreCase))
          {
            key = str1;
            obj = (object) key;
          }
          else if (string.Equals(groupByFieldString, "Environment", StringComparison.OrdinalIgnoreCase))
          {
            key = int32_2.ToString();
            obj = (object) new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
            {
              Id = int32_4,
              Name = str3,
              EnvironmentDefinitionId = int32_2
            };
          }
        }
        if (key == null)
        {
          key = string.Empty;
          obj = (object) string.Empty;
        }
        if (!resultsMap.ContainsKey(int32_1))
          return;
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult results = resultsMap[int32_1];
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult1 = results;
        ShallowReference shallowReference1;
        if (int32_3 == 0)
        {
          shallowReference1 = (ShallowReference) null;
        }
        else
        {
          shallowReference1 = new ShallowReference();
          shallowReference1.Id = int32_3.ToString();
          shallowReference1.Name = str2;
        }
        testCaseResult1.Build = shallowReference1;
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult2 = results;
        ShallowReference shallowReference2;
        if (int32_4 == 0)
        {
          shallowReference2 = (ShallowReference) null;
        }
        else
        {
          shallowReference2 = new ShallowReference();
          shallowReference2.Id = int32_4.ToString();
          shallowReference2.Name = str3;
        }
        testCaseResult2.Release = shallowReference2;
        aggregatedResultsMap.Add(key, new TestResultHistoryDetailsForGroup()
        {
          LatestResult = results
        });
        aggregatedResultsMap[key].GroupByValue = obj;
      }
    }

    protected class FetchTestResultFailuresColumns
    {
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder FailingSince = new SqlColumnBinder(nameof (FailingSince));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));

      internal TestCaseResult bind(SqlDataReader reader)
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.TestCaseReferenceId = this.TestCaseRefId.GetInt32((IDataReader) reader);
        testCaseResult.Outcome = this.Outcome.GetByte((IDataReader) reader);
        testCaseResult.TestResultId = this.TestResultId.ColumnExists((IDataReader) reader) ? this.TestResultId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.DateCompleted = this.DateCompleted.ColumnExists((IDataReader) reader) ? this.DateCompleted.GetDateTime((IDataReader) reader) : DateTime.UtcNow;
        string json = this.FailingSince.ColumnExists((IDataReader) reader) ? this.FailingSince.GetString((IDataReader) reader, true) : string.Empty;
        if (!string.IsNullOrEmpty(json))
          testCaseResult.FailingSince = JsonUtilities.Deserialize<FailingSince>(json, true);
        return testCaseResult;
      }
    }

    protected class QueryAggregatedResultsForBuild2
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

      internal void bindAggregatedResultsForBuild(
        SqlDataReader reader,
        Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap)
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
        if (!aggregatedResultsMap.ContainsKey(int32_1))
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
        aggregatedResultsMap[int32_1].Item2.Duration = Validator.CheckOverflowAndGetSafeValue(aggregatedResultsMap[int32_1].Item2.Duration, TimeSpan.FromMilliseconds((double) int64));
      }
    }

    protected class QueryAggregatedResultsForRelease2
    {
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));
      private SqlColumnBinder DefinitionId = new SqlColumnBinder("ReleaseDefId");
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder RunDuration = new SqlColumnBinder(nameof (RunDuration));

      internal void bindAggregatedResultsForRelease(
        SqlDataReader reader,
        Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap)
      {
        int int32_1 = this.ReleaseId.GetInt32((IDataReader) reader);
        string str = this.ReleaseName.GetString((IDataReader) reader, true);
        int int32_2 = this.DefinitionId.GetInt32((IDataReader) reader);
        TestOutcome key = (TestOutcome) this.TestOutcome.GetByte((IDataReader) reader);
        int int32_3 = this.ResultCount.GetInt32((IDataReader) reader);
        int int32_4 = this.TestRunId.GetInt32((IDataReader) reader);
        long int64 = Convert.ToInt64(this.RunDuration.GetObject((IDataReader) reader));
        if (!aggregatedResultsMap.ContainsKey(int32_1))
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
        aggregatedResultsMap[int32_1].Item2.Duration = Validator.CheckOverflowAndGetSafeValue(aggregatedResultsMap[int32_1].Item2.Duration, TimeSpan.FromMilliseconds((double) int64));
      }
    }
  }
}
