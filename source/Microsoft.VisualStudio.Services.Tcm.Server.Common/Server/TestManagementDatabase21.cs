// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase21
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
  public class TestManagementDatabase21 : TestManagementDatabase20
  {
    internal TestManagementDatabase21(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase21()
    {
    }

    public override UpdatedRunProperties UpdateTestRun(
      Guid projectId,
      TestRun run,
      Guid updatedBy,
      ReleaseReference releaseRef = null,
      BuildConfiguration buildRef = null,
      bool skipRunStateTransitionCheck = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_UpdateTestRun");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", run.TestRunId);
      this.BindStringPreserveNull("@title", run.Title, 256, SqlDbType.NVarChar);
      this.BindGuidPreserveNull("@owner", run.Owner);
      this.BindByte("@state", run.State, (byte) 0);
      this.BindNullableDateTime("@dueDate", run.DueDate);
      this.BindNullableInt("@iterationId", run.IterationId, 0);
      this.BindStringPreserveNull("@controller", run.Controller, 256, SqlDbType.NVarChar);
      this.BindStringPreserveNull("@errorMessage", run.ErrorMessage, 512, SqlDbType.NVarChar);
      this.BindNullableDateTime("@dateStarted", run.StartDate);
      this.BindNullableDateTime("@dateCompleted", run.CompleteDate);
      this.BindInt("@testMessageLogId", run.TestMessageLogId);
      this.BindInt("@testSettingsId", run.TestSettingsId);
      this.BindInt("@publicTestSettingsId", run.PublicTestSettingsId);
      this.BindGuid("@testEnvironmentId", run.TestEnvironmentId);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindByte("@postProcessState", run.PostProcessState, (byte) 0);
      this.BindInt("@version", run.Version);
      this.BindInt("@revision", run.Revision);
      this.BindBoolean("@isBvt", run.IsBvt);
      this.BindStringPreserveNull("@comment", run.Comment, 1048576, SqlDbType.NVarChar);
      this.BindByte("@substate", run.Substate, (byte) 0);
      this.BindString("@testEnvironmentUrl", !run.RunHasDtlEnvironment || run.DtlTestEnvironment == null ? (string) null : run.DtlTestEnvironment.Url, 2048, true, SqlDbType.NVarChar);
      this.BindString("@autEnvironmentUrl", !run.RunHasDtlEnvironment || run.DtlAutEnvironment == null ? (string) null : run.DtlAutEnvironment.Url, 2048, true, SqlDbType.NVarChar);
      this.BindString("@dtlCsmParameters", !run.RunHasDtlEnvironment || run.CsmParameters == null ? (string) null : run.CsmParameters, 2048, true, SqlDbType.NVarChar);
      this.BindBoolean("@skipRunStateTransitionCheck", skipRunStateTransitionCheck);
      this.BindString("@dropLocation", run.DropLocation, 260, true, SqlDbType.NVarChar);
      List<BuildConfiguration> builds;
      if (buildRef == null)
      {
        builds = (List<BuildConfiguration>) null;
      }
      else
      {
        builds = new List<BuildConfiguration>();
        builds.Add(buildRef);
      }
      this.BindBuildRefTypeTable3("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
      List<ReleaseReference> releases;
      if (releaseRef == null)
      {
        releases = (List<ReleaseReference>) null;
      }
      else
      {
        releases = new List<ReleaseReference>();
        releases.Add(releaseRef);
      }
      this.BindReleaseRefTypeTable4("@releaseRefData", (IEnumerable<ReleaseReference>) releases);
      this.BindString("@sourceWorkflow", run.SourceWorkflow, 128, false, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedRunProperties updatedRunProperties = reader.Read() ? new TestManagementDatabase21.UpdatedPropertyColumns().bindUpdatedRunProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestRun");
      updatedRunProperties.LastUpdatedBy = updatedBy;
      if (updatedRunProperties.IsRunCompleted)
        run.CompleteDate = updatedRunProperties.CompleteDate;
      return updatedRunProperties;
    }

    public override List<TestCaseResult> QueryTestResultsByCategory(
      GuidAndString projectId,
      string sourceWorkflow,
      BuildConfiguration buildRef,
      string categoryName)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsByCategory");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindInt("@buildId", buildRef != null ? buildRef.BuildId : 0);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase21.TestResultsByCategoryColumns byCategoryColumns = new TestManagementDatabase21.TestResultsByCategoryColumns();
      List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
      while (reader.Read())
        testCaseResultList.Add(byCategoryColumns.bind(reader, categoryName));
      return testCaseResultList;
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
        format = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestCaseResultHistory;
      else if (groupBy.Equals("Environment", StringComparison.OrdinalIgnoreCase))
        format = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestCaseResultHistoryForEnvironment;
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) definitionFilter, (object) TestManagementDynamicSqlBatchStatements.idynprc_FilterRunContextIdsAndQueryFailingSinceV2);
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement, 0, true, true);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@automatedTestName", filter.AutomatedTestName, 256, false, SqlDbType.NVarChar);
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

    private new class UpdatedPropertyColumns
    {
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      internal SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      internal SqlColumnBinder PassedTests = new SqlColumnBinder(nameof (PassedTests));
      internal SqlColumnBinder FailedTests = new SqlColumnBinder(nameof (FailedTests));
      internal SqlColumnBinder IsRunStarted = new SqlColumnBinder(nameof (IsRunStarted));
      internal SqlColumnBinder IsRunCompleted = new SqlColumnBinder(nameof (IsRunCompleted));
      internal SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));

      internal UpdatedRunProperties bindUpdatedRunProperties(SqlDataReader reader)
      {
        UpdatedRunProperties updatedRunProperties = new UpdatedRunProperties();
        updatedRunProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        updatedRunProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        updatedRunProperties.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        updatedRunProperties.PassedTests = this.PassedTests.ColumnExists((IDataReader) reader) ? this.PassedTests.GetInt32((IDataReader) reader) : 0;
        updatedRunProperties.FailedTests = this.FailedTests.ColumnExists((IDataReader) reader) ? this.FailedTests.GetInt32((IDataReader) reader) : 0;
        updatedRunProperties.IsRunStarted = this.IsRunStarted.GetBoolean((IDataReader) reader);
        updatedRunProperties.IsRunCompleted = this.IsRunCompleted.GetBoolean((IDataReader) reader);
        updatedRunProperties.CompleteDate = this.CompleteDate.GetDateTime((IDataReader) reader);
        return updatedRunProperties;
      }
    }

    private class TestResultsByCategoryColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder IntValue = new SqlColumnBinder(nameof (IntValue));
      private SqlColumnBinder FloatValue = new SqlColumnBinder(nameof (FloatValue));
      private SqlColumnBinder BitValue = new SqlColumnBinder(nameof (BitValue));
      private SqlColumnBinder DateTimeValue = new SqlColumnBinder(nameof (DateTimeValue));
      private SqlColumnBinder GuidValue = new SqlColumnBinder(nameof (GuidValue));
      private SqlColumnBinder StringValue = new SqlColumnBinder(nameof (StringValue));

      internal TestCaseResult bind(SqlDataReader reader, string categoryName)
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testCaseResult.TestResultId = this.TestResultId.GetInt32((IDataReader) reader);
        testCaseResult.Outcome = this.TestOutcome.GetByte((IDataReader) reader);
        TestExtensionField testExtensionField = new TestExtensionField()
        {
          Field = new TestExtensionFieldDetails()
        };
        testExtensionField.Field.Name = categoryName;
        testExtensionField.Value = !this.IntValue.ColumnExists((IDataReader) reader) ? (!this.FloatValue.ColumnExists((IDataReader) reader) ? (!this.BitValue.ColumnExists((IDataReader) reader) ? (!this.DateTimeValue.ColumnExists((IDataReader) reader) ? (!this.GuidValue.ColumnExists((IDataReader) reader) ? (object) this.StringValue.GetString((IDataReader) reader, false) : (object) this.GuidValue.GetGuid((IDataReader) reader)) : (object) this.DateTimeValue.GetDateTime((IDataReader) reader)) : (object) this.BitValue.GetBoolean((IDataReader) reader)) : (object) (float) this.FloatValue.GetDouble((IDataReader) reader)) : (object) this.IntValue.GetInt32((IDataReader) reader);
        if (testExtensionField.Value != null)
          testCaseResult.CustomFields = new List<TestExtensionField>()
          {
            testExtensionField
          };
        return testCaseResult;
      }
    }
  }
}
