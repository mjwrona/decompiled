// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase32
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
  public class TestManagementDatabase32 : TestManagementDatabase31
  {
    public override Dictionary<int, AggregatedDataForResultTrend> QueryAggregatedDataByRequirementForBuild2(
      GuidAndString projectId,
      List<int> testCaseRefIds,
      BuildConfiguration buildConfigurationInfo,
      string sourceWorkflow,
      int numberOfDaysAgo,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryMostRecentSummaryByRequirementForBuild2");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId.GuidId));
      this.BindInt("@buildDefinitionId", buildConfigurationInfo.BuildDefinitionId);
      this.BindString("@branchName", buildConfigurationInfo.BranchName, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("@repositoryId", buildConfigurationInfo.RepositoryId, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt("@numberOfDaysAgo", numberOfDaysAgo);
      this.BindIdTypeTable("@testCaseRefIds", (IEnumerable<int>) testCaseRefIds);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<int, AggregatedDataForResultTrend> testCaseRefToAggregateOutcomeMap = new Dictionary<int, AggregatedDataForResultTrend>();
      TestManagementDatabase32.FetchAggregatedDataByRequirementColumns2 requirementColumns2 = new TestManagementDatabase32.FetchAggregatedDataByRequirementColumns2();
      while (reader.Read())
        requirementColumns2.bindAndAdd(reader, testCaseRefToAggregateOutcomeMap);
      if (reader.NextResult() && reader.Read())
      {
        BuildConfiguration buildConfiguration = new TestManagementDatabase14.FetchBuildInformationColumns().bind(reader);
        testCaseRefToAggregateOutcomeMap.Values.ToList<AggregatedDataForResultTrend>().ForEach((Action<AggregatedDataForResultTrend>) (d => d.TestResultsContext = new TestResultsContext()
        {
          ContextType = TestResultsContextType.Build,
          Build = new BuildReference()
          {
            Id = buildConfiguration.BuildId,
            Number = buildConfiguration.BuildNumber,
            DefinitionId = buildConfiguration.BuildDefinitionId,
            BranchName = buildConfiguration.BranchName
          }
        }));
      }
      return testCaseRefToAggregateOutcomeMap;
    }

    public override Dictionary<int, AggregatedDataForResultTrend> QueryAggregatedDataByRequirementForRelease2(
      GuidAndString projectId,
      List<int> testCaseRefIds,
      int releaseDefinitionId,
      int releaseEnvironmentDefId,
      string sourceWorkflow,
      int numberOfDaysAgo,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryMostRecentSummaryByRequirementForRelease2");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId.GuidId));
      this.BindInt("@releaseDefinitionId", releaseDefinitionId);
      this.BindInt("@releaseEnvironmentDefinitionId", releaseEnvironmentDefId);
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt("@numberOfDaysAgo", numberOfDaysAgo);
      this.BindIdTypeTable("@testCaseRefIds", (IEnumerable<int>) testCaseRefIds);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<int, AggregatedDataForResultTrend> testCaseRefToAggregateOutcomeMap = new Dictionary<int, AggregatedDataForResultTrend>();
      TestManagementDatabase32.FetchAggregatedDataByRequirementColumns2 requirementColumns2 = new TestManagementDatabase32.FetchAggregatedDataByRequirementColumns2();
      while (reader.Read())
        requirementColumns2.bindAndAdd(reader, testCaseRefToAggregateOutcomeMap);
      if (reader.NextResult() && reader.Read())
      {
        ReleaseReference releaseRef = new TestManagementDatabase15.FetchReleaseInformationColumns().bind(reader);
        testCaseRefToAggregateOutcomeMap.Values.ToList<AggregatedDataForResultTrend>().ForEach((Action<AggregatedDataForResultTrend>) (d => d.TestResultsContext = new TestResultsContext()
        {
          ContextType = TestResultsContextType.Release,
          Release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
          {
            Id = releaseRef.ReleaseId,
            EnvironmentId = releaseRef.ReleaseEnvId,
            Name = releaseRef.ReleaseName
          }
        }));
      }
      return testCaseRefToAggregateOutcomeMap;
    }

    public override void SyncRequirementTestLinks(
      Guid projectId,
      int workItemId,
      IEnumerable<int> testCaseRefIds,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("TestResult.prc_SyncRequirementTestLinks");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@workItemId", workItemId);
      this.BindIdTypeTable("@testCaseRefIds", testCaseRefIds);
      this.BindGuid("@updatedBy", updatedBy);
      this.ExecuteNonQuery();
    }

    internal TestManagementDatabase32(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase32()
    {
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
      this.BindIdTypeTable("@testCaseRefIds", (IEnumerable<int>) filter.TestCaseReferenceIds);
      SqlDataReader reader = this.ExecuteReader();
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      TestManagementDatabase4.QueryTestResultTrendColumns resultTrendColumns = new TestManagementDatabase4.QueryTestResultTrendColumns();
      while (reader.Read())
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = resultTrendColumns.bind(reader);
        testCaseResult.Project = new ShallowReference()
        {
          Id = projectId.ToString()
        };
        testCaseResultList.Add(testCaseResult);
      }
      return testCaseResultList;
    }

    public override Dictionary<int, List<RunSummaryByOutcome>> QueryTestRunsOutComeSummary(
      Guid projectId,
      IList<int> testRunIds)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunsOutComeSummary");
        this.PrepareStoredProcedure("TestResult.prc_QueryRunOutcomeSummary");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindIdTypeTable("@testRunIds", (IEnumerable<int>) testRunIds);
        TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, List<RunSummaryByOutcome>> dictionary = new Dictionary<int, List<RunSummaryByOutcome>>();
        while (reader.Read())
        {
          RunSummaryByOutcome summaryByOutcome = runSummaryColumns.bind(reader);
          if (dictionary.ContainsKey(summaryByOutcome.TestRunId))
            dictionary[summaryByOutcome.TestRunId].Add(summaryByOutcome);
          else
            dictionary.Add(summaryByOutcome.TestRunId, new List<RunSummaryByOutcome>()
            {
              summaryByOutcome
            });
        }
        return dictionary;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunsOutComeSummary");
      }
    }

    public override void UpdateTestCaseReference2(
      Guid projectId,
      IEnumerable<TestCaseResult> results)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.UpdateTestCaseReference2");
        this.PrepareStoredProcedure("TestResult.prc_UpdateTestCaseReference2");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindTestResult_TestCaseReference4TypeTableForUpdate("@testCaseReferenceTable", results);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.UpdateTestCaseReference2");
      }
    }

    public override List<TestRun> QueryTestRunsbyFilters(
      Guid projectId,
      QueryTestRunsFilter filters,
      int top,
      int batchSize,
      out int minNextBatchRunId,
      out DateTime minNextBatchLastUpdated)
    {
      try
      {
        minNextBatchRunId = -1;
        minNextBatchLastUpdated = DateTime.MaxValue;
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunsbyFilters");
        Dictionary<int, TestRun> dictionary = new Dictionary<int, TestRun>();
        this.PrepareStoredProcedure("TestResult.prc_QueryTestRuns");
        this.BindIdTypeTable("@planIds", (IEnumerable<int>) filters.PlanIds);
        this.BindNullableInt("@state", filters.State, -1);
        this.BindNullableDateTime("@minLastUpdatedDate", filters.MinLastUpdatedDate);
        this.BindNullableDateTime("@maxLastUpdatedDate", filters.MaxLastUpdatedDate);
        this.BindNullableBoolean("@isAutomated", filters.IsAutomated);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindIdTypeTable("@buildIds", (IEnumerable<int>) filters.BuildIds);
        this.BindIdTypeTable("@buildDefIds", (IEnumerable<int>) filters.BuildDefIds);
        this.BindIdTypeTable("@releaseIds", (IEnumerable<int>) filters.ReleaseIds);
        this.BindIdTypeTable("@releaseDefIds", (IEnumerable<int>) filters.ReleaseDefIds);
        this.BindIdTypeTable("@releaseEnvIds", (IEnumerable<int>) filters.ReleaseEnvIds);
        this.BindIdTypeTable("@releaseEnvDefIds", (IEnumerable<int>) filters.ReleaseEnvDefIds);
        this.BindString("@branchName", filters.BranchName, 400, true, SqlDbType.NVarChar);
        this.BindString("@runTitle", filters.RunTitle, 256, true, SqlDbType.NVarChar);
        this.BindString("@sourceWorkflow", filters.SourceWorkflow, 128, true, SqlDbType.NVarChar);
        this.BindInt("@top", top);
        TestManagementDatabase29.QueryTestRunColumnsByFilters columnsByFilters = new TestManagementDatabase29.QueryTestRunColumnsByFilters();
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
        {
          TestRun testRun = columnsByFilters.bind(reader);
          dictionary.Add(testRun.TestRunId, testRun);
        }
        return dictionary.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunsbyFilters");
      }
    }

    protected class FetchAggregatedDataByRequirementColumns2
    {
      private SqlColumnBinder TestCaseReferenceId = new SqlColumnBinder("TestCaseRefId");
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder TestCount = new SqlColumnBinder(nameof (TestCount));

      internal void bindAndAdd(
        SqlDataReader reader,
        Dictionary<int, AggregatedDataForResultTrend> testCaseRefToAggregateOutcomeMap)
      {
        int int32_1 = this.TestCaseReferenceId.GetInt32((IDataReader) reader);
        TestOutcome key = (TestOutcome) this.TestOutcome.GetByte((IDataReader) reader);
        int int32_2 = this.TestCount.GetInt32((IDataReader) reader);
        if (!testCaseRefToAggregateOutcomeMap.ContainsKey(int32_1))
          testCaseRefToAggregateOutcomeMap[int32_1] = new AggregatedDataForResultTrend()
          {
            ResultsByOutcome = (IDictionary<TestOutcome, AggregatedResultsByOutcome>) new Dictionary<TestOutcome, AggregatedResultsByOutcome>()
          };
        testCaseRefToAggregateOutcomeMap[int32_1].ResultsByOutcome[key] = new AggregatedResultsByOutcome()
        {
          Outcome = key,
          Count = int32_2
        };
      }
    }
  }
}
