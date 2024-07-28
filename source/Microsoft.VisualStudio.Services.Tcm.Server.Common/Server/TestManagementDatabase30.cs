// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase30
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
  public class TestManagementDatabase30 : TestManagementDatabase29
  {
    internal TestManagementDatabase30(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase30()
    {
    }

    public override List<TestCaseResult> GetTestResultsByFQDN(
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      int buildId,
      string sourceWorkflow,
      List<TestCaseReference> testRefs)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.GetTestResultsByFQDN");
        this.PrepareStoredProcedure("TestResult.prc_QueryTestCaseResult");
        this.BindNullableInt("@releaseId", releaseId, 0);
        this.BindNullableInt("@releaseEnvId", releaseEnvId, 0);
        this.BindNullableInt("@buildId", buildId, 0);
        this.BindString("@sourceWorkflow", sourceWorkflow, 256, true, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindTestResult_TestCaseReference3TypeTableForPopulateHash("@testcaseReferences", (IEnumerable<TestCaseReference>) testRefs);
        TestManagementDatabase30.QueryTestCaseResultColumns caseResultColumns = new TestManagementDatabase30.QueryTestCaseResultColumns();
        List<TestCaseResult> testResultsByFqdn = new List<TestCaseResult>();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          if (reader.HasRows)
          {
            while (reader.Read())
              testResultsByFqdn.Add(caseResultColumns.bind(reader));
          }
        }
        return testResultsByFqdn;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestCaseResultByDefinition");
      }
    }

    protected override void BindQueryTestResultTrendParams(Guid projectId, ResultsFilter filter)
    {
      int parameterValue1 = 0;
      int parameterValue2 = 0;
      if (filter.TestResultsContext.ContextType == TestResultsContextType.Build)
        parameterValue1 = filter.TestResultsContext.Build.DefinitionId;
      else if (filter.TestResultsContext.ContextType == TestResultsContextType.Release)
        parameterValue2 = filter.TestResultsContext.Release.DefinitionId;
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@automatedTestName", filter.AutomatedTestName, 512, true, SqlDbType.NVarChar);
      this.BindInt("@buildDefId", parameterValue1);
      this.BindInt("@releaseDefId", parameterValue2);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      this.BindInt("@trendDays", filter.TrendDays);
      this.BindInt("@resultsCount", filter.ResultsCount);
      int parameterValue3 = 0;
      if (filter.TestResultsContext.ContextType == TestResultsContextType.Release)
        parameterValue3 = filter.TestResultsContext.Release.EnvironmentDefinitionId;
      this.BindInt("@environmentDefId", parameterValue3);
      string parameterValue4 = (string) null;
      switch (filter.TestResultsContext.ContextType)
      {
        case TestResultsContextType.Build:
          parameterValue4 = filter.TestResultsContext.Build.BranchName;
          break;
        case TestResultsContextType.Release:
          parameterValue4 = filter.Branch;
          break;
      }
      this.BindString("@branchName", parameterValue4, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
    }

    internal override void SaveMaxRVValueBeforeTestResultSchemaMigration()
    {
      this.PrepareStoredProcedure("TestResult.prc_SetMaxRVValueBeforeMigration");
      this.ExecuteNonQuery();
    }

    public override int[] CreateTestCaseReference(
      Guid projectId,
      IEnumerable<TestCaseResult> results,
      Guid updatedBy,
      out int newTestCaseReferences,
      out List<int> newTestCaseRefIds)
    {
      newTestCaseReferences = -1;
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.CreateTestCaseReference");
        this.PrepareStoredProcedure("TestResult.prc_CreateTestCaseReference");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindTestResult_TestCaseReference3TypeTableForCreate("@testCaseReferenceTable", results);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TestCaseRefId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("OrderIndex");
        int[] testCaseReference = new int[results.Count<TestCaseResult>()];
        newTestCaseRefIds = (List<int>) null;
        while (reader.Read())
        {
          int int32 = sqlColumnBinder2.GetInt32((IDataReader) reader);
          testCaseReference[int32] = sqlColumnBinder1.GetInt32((IDataReader) reader);
        }
        if (reader.NextResult() && reader.Read())
          newTestCaseReferences = reader.GetInt32(0);
        return testCaseReference;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.CreateTestCaseReference");
      }
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
        this.BindTestResult_TestCaseReference3TypeTableForUpdate("@testCaseReferenceTable", results);
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
        this.BindTestResult_TestCaseReference3HashTypeTable("@automatedTestNameHashes", (IEnumerable<string>) automatedTestNames);
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

    public override void PopulateAutomatedTestNameHash(
      int dataspaceId,
      List<TestCaseReference> testCaseReferences)
    {
      this.PrepareStoredProcedure("TestResult.prc_PopulateAutomatedTestNameHash");
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindTestResult_TestCaseReference3TypeTableForPopulateHash("@testCaseReferenceTable", (IEnumerable<TestCaseReference>) testCaseReferences);
      this.ExecuteNonQuery();
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
      this.BindInt("@prevTestRunContextId", resultInsights.PrevRunContextId);
      this.ExecuteNonQuery();
    }

    protected class QueryTestCaseResultColumns
    {
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));

      internal TestCaseResult bind(SqlDataReader reader)
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.AutomatedTestName = this.AutomatedTestName.GetString((IDataReader) reader, false);
        testCaseResult.Duration = (long) this.Duration.GetInt32((IDataReader) reader, 0);
        testCaseResult.Outcome = this.Outcome.GetByte((IDataReader) reader);
        testCaseResult.DateStarted = this.DateStarted.GetDateTime((IDataReader) reader);
        testCaseResult.DateCompleted = this.DateCompleted.GetDateTime((IDataReader) reader);
        testCaseResult.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        return testCaseResult;
      }
    }
  }
}
