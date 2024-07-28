// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase56
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase56 : TestManagementDatabase55
  {
    internal TestManagementDatabase56(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase56()
    {
    }

    public override TestHistoryContinuationTokenAndResults QueryTestHistory(
      Guid projectId,
      TestHistoryQuery filter,
      int continuationTokenMinRunId,
      int continuationTokenMaxRunId,
      int testResultBatchLimit,
      int runIdThreshold = 0)
    {
      string empty = string.Empty;
      string storedProcedure;
      if (filter.GroupBy == TestResultGroupBy.Branch)
      {
        storedProcedure = "TestResult.prc_QueryTestHistoryByBranch";
        this.PrepareStoredProcedure(storedProcedure);
        this.BindInt("@buildDefinitionId", filter.BuildDefinitionId);
      }
      else
      {
        storedProcedure = "TestResult.prc_QueryTestHistoryByEnvironment";
        this.PrepareStoredProcedure(storedProcedure);
        this.BindInt("@releaseEnvDefinitionId", filter.ReleaseEnvDefinitionId);
      }
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindBinary("@automatedTestNameHash", this.GetSHA256Hash(filter.AutomatedTestName ?? string.Empty), 32, SqlDbType.VarBinary);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindString("@branchName", filter.Branch, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt("@continuationTokenMinRunId", continuationTokenMinRunId);
      this.BindInt("@continuationTokenMaxRunId", continuationTokenMaxRunId);
      this.BindInt("@testResultBatchLimit", testResultBatchLimit);
      this.BindInt("@runIdThreshold", runIdThreshold);
      this.BindInt("@testCaseId", filter.TestCaseId);
      TestResultHistoryGroupBy groupBy = TestResultHistoryGroupBy.Branch;
      if (filter.GroupBy == TestResultGroupBy.Branch)
      {
        if (filter.BuildDefinitionId > 0)
          groupBy = TestResultHistoryGroupBy.BuildDefinitionId;
      }
      else
        groupBy = TestResultHistoryGroupBy.Environment;
      TestHistoryContinuationTokenAndResults continuationTokenAndResults = new TestHistoryContinuationTokenAndResults()
      {
        ContinuationTokenMinRunId = 0,
        ContinuationTokenMaxRunId = 0,
        results = new Dictionary<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>()
      };
      Dictionary<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>> resultsMap = new Dictionary<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>();
      TestManagementDatabase42.QueryTestHistoryColumns testHistoryColumns = new TestManagementDatabase42.QueryTestHistoryColumns();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          testHistoryColumns.bindResultMap(reader, resultsMap, groupBy, filter.BuildDefinitionId, projectId);
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(storedProcedure);
        reader.Read();
        int continuationTokenMinRunId1;
        int continuationTokenMaxRunId1;
        new TestManagementDatabase42.QueryTestHistoryColumns().bindContinuationToken(reader, out continuationTokenMinRunId1, out continuationTokenMaxRunId1);
        continuationTokenAndResults.ContinuationTokenMinRunId = continuationTokenMinRunId1;
        continuationTokenAndResults.ContinuationTokenMaxRunId = continuationTokenMaxRunId1;
      }
      continuationTokenAndResults.results = resultsMap;
      return continuationTokenAndResults;
    }

    public override void UpdateTestRunSummaryForNonConfigRuns(
      Guid projectId,
      int testRunId,
      IEnumerable<RunSummaryByOutcome> runSummaryByOutcomes)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestReportsDatabase.UpdateTestRunSummaryForNonConfigRuns");
      this.PrepareStoredProcedure("TestResult.prc_UpdateTestRunSummaryForNonConfigRuns");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindTestRunSummaryByOutcomeTypeTable("@testRunSummaryDetails", runSummaryByOutcomes);
      this.ExecuteNonQuery();
    }
  }
}
