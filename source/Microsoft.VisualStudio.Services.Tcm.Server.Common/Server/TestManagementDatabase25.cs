// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase25
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase25 : TestManagementDatabase24
  {
    internal TestManagementDatabase25(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase25()
    {
    }

    internal override List<int> QueryTestRunIds(
      string whereClause,
      string orderByClause,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunIds");
      try
      {
        this.PrepareStoredProcedure("TestResult.prc_QueryTestRunIds");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@orderByClause", orderByClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TestRunId");
        List<int> intList = new List<int>();
        while (reader.Read())
          intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
        return intList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunIds");
      }
    }

    public override List<TestRun> QueryTestRuns2(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList,
      out Dictionary<int, string> iterationMap,
      out Dictionary<Guid, List<TestRun>> projectsRunsMap)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns2");
        Dictionary<int, TestRun> dictionary1 = new Dictionary<int, TestRun>();
        this.PrepareStoredProcedure("TestResult.prc_QueryTestRuns2");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, Guid> dictionary2 = new Dictionary<int, Guid>();
        iterationMap = new Dictionary<int, string>();
        projectsRunsMap = new Dictionary<Guid, List<TestRun>>();
        while (reader.Read())
        {
          int dataspaceId;
          string iterationUri;
          TestRun testRun = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          if (!string.IsNullOrEmpty(iterationUri))
            iterationMap[testRun.TestRunId] = iterationUri;
          if (dictionary2.ContainsKey(dataspaceId))
          {
            Guid key = dictionary2[dataspaceId];
            projectsRunsMap[key].Add(testRun);
          }
          else
          {
            Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
            dictionary2[dataspaceId] = dataspaceIdentifier;
            projectsRunsMap[dataspaceIdentifier] = new List<TestRun>();
            projectsRunsMap[dataspaceIdentifier].Add(testRun);
          }
          dictionary1.Add(testRun.TestRunId, testRun);
        }
        return dictionary1.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns2");
      }
    }

    public override List<TestCaseResultIdentifier> QueryTestResults2(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResults2");
      this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResultIdentifier> resultIdentifierList = new List<TestCaseResultIdentifier>();
      TestManagementDatabase.QueryTestResultsColumns testResultsColumns = new TestManagementDatabase.QueryTestResultsColumns();
      while (reader.Read())
        resultIdentifierList.Add(testResultsColumns.bind(reader));
      return resultIdentifierList;
    }

    public override List<TestCaseResult> QueryTestResultHistory(
      Guid projectId,
      string automatedTestName,
      int testCaseId,
      DateTime maxCompleteDate,
      int historyDays)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultHistory");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindBinary("@automatedTestNameHash", this.GetSHA256Hash(automatedTestName ?? string.Empty), 32, SqlDbType.VarBinary);
      this.BindInt("@testCaseId", testCaseId);
      this.BindInt("@historyDays", historyDays);
      this.BindNullableDateTime("@maxCompleteDate", maxCompleteDate);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
      TestManagementDatabase4.QueryTestResultHistoryColumns resultHistoryColumns = new TestManagementDatabase4.QueryTestResultHistoryColumns();
      while (reader.Read())
        testCaseResultList.Add(resultHistoryColumns.bind(reader));
      return testCaseResultList;
    }

    public override List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> QueryTestResultTrendReport(
      Guid projectId,
      ResultsFilter filter)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultTrend");
      int parameterValue1 = 0;
      int parameterValue2 = 0;
      if (filter.TestResultsContext.ContextType == TestResultsContextType.Build)
        parameterValue1 = filter.TestResultsContext.Build.DefinitionId;
      else if (filter.TestResultsContext.ContextType == TestResultsContextType.Release)
        parameterValue2 = filter.TestResultsContext.Release.DefinitionId;
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindBinary("@automatedTestNameHash", this.GetSHA256Hash(filter.AutomatedTestName ?? string.Empty), 32, SqlDbType.VarBinary);
      this.BindInt("@buildDefId", parameterValue1);
      this.BindInt("@releaseDefId", parameterValue2);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      this.BindInt("@trendDays", filter.TrendDays);
      this.BindInt("@resultsCount", filter.ResultsCount);
      string parameterValue3 = (string) null;
      switch (filter.TestResultsContext.ContextType)
      {
        case TestResultsContextType.Build:
          parameterValue3 = filter.TestResultsContext.Build.BranchName;
          break;
        case TestResultsContextType.Release:
          parameterValue3 = filter.Branch;
          break;
      }
      this.BindString("@branchName", parameterValue3, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      int parameterValue4 = 0;
      if (filter.TestResultsContext.ContextType == TestResultsContextType.Release)
        parameterValue4 = filter.TestResultsContext.Release.EnvironmentDefinitionId;
      this.BindInt("@environmentDefId", parameterValue4);
      SqlDataReader reader = this.ExecuteReader();
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      TestManagementDatabase4.QueryTestResultTrendColumns resultTrendColumns = new TestManagementDatabase4.QueryTestResultTrendColumns();
      while (reader.Read())
        testCaseResultList.Add(resultTrendColumns.bind(reader));
      return testCaseResultList;
    }

    internal override List<IdAndRev> GetRunsAssociatedWithRelease(
      Guid projectId,
      byte state,
      int releaseId,
      int releaseEnvId)
    {
      this.PrepareStoredProcedure("TestResult.prc_GetRunsAssociatedWithRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindByte("@state", state);
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@releaseEnvId", releaseEnvId);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.IdAndRevColumns idAndRevColumns = new TestManagementDatabase.IdAndRevColumns();
      List<IdAndRev> associatedWithRelease = new List<IdAndRev>();
      while (reader.Read())
        associatedWithRelease.Add(idAndRevColumns.bind(reader));
      return associatedWithRelease;
    }

    public override List<TestCaseResult> GetPreMigrationAutomatedTestCaseReferencesWithoutHash(
      int top)
    {
      this.PrepareStoredProcedure("dbo.prc_GetAutomatedTestNamesWithoutHash");
      this.BindInt("@top", top);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase25.PreMigrationAutomatedTestNamesColumns testNamesColumns = new TestManagementDatabase25.PreMigrationAutomatedTestNamesColumns();
      List<TestCaseResult> referencesWithoutHash = new List<TestCaseResult>();
      while (reader.Read())
        referencesWithoutHash.Add(testNamesColumns.bind(reader));
      return referencesWithoutHash;
    }

    public override void PopulatePreMigrationAutomatedTestNameHash(
      List<TestCaseResult> automatedTestDetails)
    {
      this.PrepareStoredProcedure("dbo.prc_PopulateAutomatedTestNameHash");
      this.Bind_AutomatedTestDetailsTypeTable("@automatedTestDetailsTable", (IEnumerable<TestCaseResult>) automatedTestDetails);
      this.ExecuteNonQuery();
    }

    public override TestResultHistory QueryTestCaseResultHistory(
      Guid projectId,
      ResultsFilter filter,
      bool isTfvcBranchFilteringEnabled)
    {
      string groupBy = filter.GroupBy;
      int definitionId = 0;
      string definitionFilter = this.GetDefinitionFilter(filter, out definitionId);
      string format = string.Empty;
      if (groupBy.Equals("Branch", StringComparison.OrdinalIgnoreCase))
        format = !isTfvcBranchFilteringEnabled ? TestManagementDynamicSqlBatchStatements.dynprc_QueryTestCaseResultHistoryV2 : TestManagementDynamicSqlBatchStatements.dynprc_QueryTestCaseResultHistoryWithTfvcBranchFiltering;
      else if (groupBy.Equals("Environment", StringComparison.OrdinalIgnoreCase))
        format = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestCaseResultHistoryForEnvironmentV2;
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) definitionFilter, (object) TestManagementDynamicSqlBatchStatements.idynprc_FilterRunContextIdsAndQueryFailingSinceV3);
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement, 0, true, true);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindBinary("@automatedTestNameHash", this.GetSHA256Hash(filter.AutomatedTestName ?? string.Empty), 32, SqlDbType.VarBinary);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      if (groupBy.Equals("Branch", StringComparison.OrdinalIgnoreCase))
        this.BindInt("@buildDays", 15);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindInt("@definitionId", definitionId);
      this.BindString("@branchName", filter.Branch, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      TestResultHistory testResultHistory = new TestResultHistory();
      testResultHistory.GroupByField = groupBy;
      Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> dictionary = new Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      Dictionary<string, TestResultHistoryDetailsForGroup> resultsMap = new Dictionary<string, TestResultHistoryDetailsForGroup>();
      Dictionary<TestCaseResultIdentifier, FailingSince> failingSinceMap = new Dictionary<TestCaseResultIdentifier, FailingSince>();
      TestManagementDatabase13.QueryTestCaseResultHistoryColumns resultHistoryColumns1 = new TestManagementDatabase13.QueryTestCaseResultHistoryColumns();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultHistoryColumns1.bindGroupedResultHistory(reader, resultsMap, groupBy);
        if (reader.NextResult())
        {
          TestManagementDatabase13.QueryTestCaseResultHistoryColumns resultHistoryColumns2 = new TestManagementDatabase13.QueryTestCaseResultHistoryColumns();
          while (reader.Read())
            resultHistoryColumns2.bindFailingSinceColumn(reader, failingSinceMap);
        }
      }
      foreach (TestResultHistoryDetailsForGroup historyDetailsForGroup in resultsMap.Values)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult latestResult = historyDetailsForGroup.LatestResult;
        int result;
        if (int.TryParse(latestResult.TestRun.Id, out result))
        {
          TestCaseResultIdentifier key = new TestCaseResultIdentifier(result, latestResult.Id);
          historyDetailsForGroup.LatestResult.FailingSince = failingSinceMap.ContainsKey(key) ? failingSinceMap[key] : (FailingSince) null;
        }
      }
      testResultHistory.ResultsForGroup = (IList<TestResultHistoryDetailsForGroup>) resultsMap.Values.ToList<TestResultHistoryDetailsForGroup>();
      return testResultHistory;
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
      this.BindBinary("@automatedTestNameHash", this.GetSHA256Hash(filter.AutomatedTestName ?? string.Empty), 32, SqlDbType.VarBinary);
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

    public override Dictionary<ReleaseReference, RunSummaryAndInsights> QueryTestRunSummaryForReleases(
      GuidAndString projectId,
      List<ReleaseReference> releases,
      string categoryName,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryForReleases");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindReleaseRefTypeTable4("@releases", (IEnumerable<ReleaseReference>) releases);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<ReleaseReference, RunSummaryAndInsights> dictionary = new Dictionary<ReleaseReference, RunSummaryAndInsights>();
      TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
      while (reader.Read())
      {
        ReleaseReference key = runSummaryColumns.bindRelease(reader);
        runSummaryColumns.bind(reader);
        if (!dictionary.ContainsKey(key))
          dictionary[key] = new RunSummaryAndInsights()
          {
            TestRunSummary = new RunSummary()
            {
              CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
              PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
              CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
            }
          };
        dictionary[key].TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
      }
      if (!string.IsNullOrEmpty(categoryName))
      {
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryForReleases");
        while (reader.Read())
        {
          TestManagementDatabase19.FetchTestRunSummaryByCategoryColumns byCategoryColumns = new TestManagementDatabase19.FetchTestRunSummaryByCategoryColumns();
          ReleaseReference key = runSummaryColumns.bindRelease(reader);
          dictionary[key].TestRunSummary.CurrentAggregateDataByReportingCategory.Add(byCategoryColumns.bind(reader));
        }
      }
      return dictionary;
    }

    protected class PreMigrationAutomatedTestNamesColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder AutomatedTestStorage = new SqlColumnBinder(nameof (AutomatedTestStorage));

      internal TestCaseResult bind(SqlDataReader reader)
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testCaseResult.TestResultId = this.TestResultId.GetInt32((IDataReader) reader);
        testCaseResult.AutomatedTestName = this.AutomatedTestName.ColumnExists((IDataReader) reader) ? this.AutomatedTestName.GetString((IDataReader) reader, false) : string.Empty;
        testCaseResult.AutomatedTestStorage = this.AutomatedTestStorage.ColumnExists((IDataReader) reader) ? this.AutomatedTestStorage.GetString((IDataReader) reader, false) : string.Empty;
        return testCaseResult;
      }
    }
  }
}
