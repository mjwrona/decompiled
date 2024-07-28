// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase6
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase6 : TestManagementDatabase5
  {
    internal override void AddOrUpdateCodeCoverageSummary(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.AddOrUpdateCodeCoverageSummary");
        this.PrepareStoredProcedure("prc_AddOrUpdateCodeCoverageSummary");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
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
        this.BindCoverageSummaryTypeTable("@coverageStatsDataTable", (IEnumerable<CodeCoverageStatistics>) coverageData.CoverageStats);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.AddOrUpdateCodeCoverageSummary");
      }
    }

    internal TestManagementDatabase6(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase6()
    {
    }

    internal override TestCaseResult ResetTestResult(
      Guid projectId,
      int testRunId,
      int testResultId,
      Guid updatedBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.ResetTestResult");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_ResetTestResult");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@testResultId", testResultId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindInt("@maxCommentSize", TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext));
        SqlDataReader reader = this.ExecuteReader();
        TestCaseResult testCaseResult = reader.Read() ? new TestManagementDatabase.FetchTestResultsColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_ResetTestResult");
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase6.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase6.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return testCaseResult;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.ResetTestResult");
      }
    }

    public override List<TestCaseResult> FetchTestResults(
      TestCaseResultIdAndRev[] resultIds,
      List<TestCaseResultIdentifier> deletedIds,
      Guid projectId,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResults");
        this.PrepareStoredProcedure("TestResult.prc_FetchTestResults");
        this.BindBoolean("@includeActionResults", includeActionResults);
        this.BindTestCaseResultIdAndRevTypeTable("@idsTable", (IEnumerable<TestCaseResultIdAndRev>) resultIds);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@maxCommentSize", TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext));
        Dictionary<TestCaseResultIdentifier, TestCaseResult> dictionary = new Dictionary<TestCaseResultIdentifier, TestCaseResult>(resultIds.Length);
        TestManagementDatabase.FetchTestResultsColumns testResultsColumns1 = new TestManagementDatabase.FetchTestResultsColumns();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          while (reader.Read())
          {
            TestCaseResult testCaseResult = testResultsColumns1.bind(reader);
            testCaseResult.CustomFields = new List<TestExtensionField>();
            dictionary.Add(new TestCaseResultIdentifier(testCaseResult.TestRunId, testCaseResult.TestResultId), testCaseResult);
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResults");
          TestManagementDatabase.FetchTestResultsExColumns resultsExColumns = new TestManagementDatabase.FetchTestResultsExColumns();
          while (reader.Read())
          {
            Tuple<int, int, TestExtensionField> tuple = resultsExColumns.bind(reader);
            TestCaseResultIdentifier key = new TestCaseResultIdentifier(tuple.Item1, tuple.Item2);
            if (dictionary.ContainsKey(key))
            {
              if (string.Equals("StackTrace", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                dictionary[key].StackTrace = tuple.Item3;
              else if (string.Equals("FailingSince", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string jsonString)
                {
                  FailingSince convertedObject = (FailingSince) null;
                  if (TestResultHelper.TryJsonConvertWithRetry<FailingSince>(jsonString, out convertedObject, true))
                    dictionary[key].FailingSince = convertedObject;
                }
              }
              else if (string.Equals("Comment", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                dictionary[key].Comment = tuple.Item3.Value as string;
              else if (string.Equals("ErrorMessage", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                dictionary[key].ErrorMessage = tuple.Item3.Value as string;
              else if (string.Equals("UnsanitizedTestCaseTitle", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string str1)
                  dictionary[key].TestCaseTitle = str1;
              }
              else if (string.Equals("UnsanitizedAutomatedTestName", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string str2)
                  dictionary[key].AutomatedTestName = str2;
              }
              else if (string.Equals("MaxReservedSubResultId", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                dictionary[key].SubResultCount = (int) tuple.Item3.Value;
              else if (string.Equals("TestResultGroupType", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                dictionary[key].ResultGroupType = (ResultGroupType) TestManagementServiceUtility.ValidateAndGetEnumValue<ResultGroupType>((string) tuple.Item3.Value, ResultGroupType.None);
              else
                dictionary[key].CustomFields.Add(tuple.Item3);
            }
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResults");
          if (deletedIds != null)
          {
            TestManagementDatabase.QueryTestResultsColumns testResultsColumns2 = new TestManagementDatabase.QueryTestResultsColumns();
            while (reader.Read())
              deletedIds.Add(testResultsColumns2.bindDeleted(reader));
          }
          if (includeActionResults)
          {
            actionResults = new List<TestActionResult>();
            parameters = new List<TestResultParameter>();
            attachments = new List<TestResultAttachment>();
            this.ReadTestActionResults(actionResults, parameters, attachments, reader, "prc_FetchTestResults");
          }
          else
          {
            actionResults = (List<TestActionResult>) null;
            parameters = (List<TestResultParameter>) null;
            attachments = (List<TestResultAttachment>) null;
          }
        }
        return dictionary.Values.ToList<TestCaseResult>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResults");
      }
    }

    private List<Tuple<int, int, TestExtensionField>> GetExtensionFieldsMap(
      int testRunId,
      IEnumerable<TestCaseResult> results,
      bool useOrderIndex)
    {
      List<Tuple<int, int, TestExtensionField>> extensionFieldsMap = new List<Tuple<int, int, TestExtensionField>>();
      int orderIndex = 0;
      foreach (TestCaseResult result1 in results)
      {
        TestCaseResult result = result1;
        if (result.StackTrace != null)
          extensionFieldsMap.Add(new Tuple<int, int, TestExtensionField>(testRunId, useOrderIndex ? orderIndex : result.TestResultId, result.StackTrace));
        if (result.CustomFields != null && result.CustomFields.Any<TestExtensionField>())
          extensionFieldsMap.AddRange(result.CustomFields.Select<TestExtensionField, Tuple<int, int, TestExtensionField>>((System.Func<TestExtensionField, Tuple<int, int, TestExtensionField>>) (f => new Tuple<int, int, TestExtensionField>(testRunId, useOrderIndex ? orderIndex : result.TestResultId, f))));
        orderIndex++;
      }
      return extensionFieldsMap;
    }

    public override int FindAndStoreMaxRunIdWithoutInsights()
    {
      this.PrepareStoredProcedure("prc_FindAndStoreMaxRunIdWithoutInsights");
      SqlDataReader reader = this.ExecuteReader();
      return reader.HasRows && reader.Read() ? new SqlColumnBinder("MaxTestRunId").GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException("prc_FindAndStoreMaxRunIdWithoutInsights");
    }

    public override void DeleteSummaryAndInsightsForXamlBuilds(int batchSize)
    {
      this.PrepareStoredProcedure("prc_DeleteSummaryAndInsightsForXamlBuilds");
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
    }

    public override UpdatedProperties AbortTestRun(
      Guid projectId,
      int testRunId,
      int revision,
      TestRunAbortOptions options,
      byte substate,
      Guid updatedBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.AbortTestRun");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_AbortTestRun");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@revision", revision);
        this.BindInt("@options", (int) options);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindByte("@substate", substate);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase6.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_AbortTestRun");
        updatedProperties.LastUpdatedBy = updatedBy;
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase6.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase6.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return updatedProperties;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.AbortTestRun");
      }
    }

    public override UpdatedProperties CancelTestRun(
      Guid projectId,
      int testRunId,
      Guid canceledBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.CancelTestRun");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_CancelTestRun");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindGuid("@canceledBy", canceledBy);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase6.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_CancelTestRun");
        updatedProperties.LastUpdatedBy = canceledBy;
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase6.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase6.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return updatedProperties;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.CancelTestRun");
      }
    }

    public override List<TestRun> QueryTestRuns(
      int testRunId,
      Guid owner,
      string buildUri,
      Guid projectId,
      out Dictionary<int, string> iterationMap,
      out Dictionary<Guid, List<TestRun>> projectsRunsMap,
      int planId = -1,
      int skip = 0,
      int top = 2147483647,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns");
        Dictionary<int, TestRun> dictionary1 = new Dictionary<int, TestRun>();
        this.PrepareStoredProcedure("prc_QueryTestRuns");
        this.BindNullableInt("@testRunId", testRunId, 0);
        this.BindGuidPreserveNull("@owner", owner);
        this.BindString("@buildUri", buildUri, 256, true, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        TestManagementDatabase6.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase6.QueryTestRunColumns();
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
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestRuns");
        TestManagementDatabase.FetchTestRunsExColumns testRunsExColumns = new TestManagementDatabase.FetchTestRunsExColumns();
        while (reader.Read())
        {
          Tuple<int, TestExtensionField> tuple = testRunsExColumns.bind(reader);
          if (dictionary1.ContainsKey(tuple.Item1))
          {
            TestRun testRun = dictionary1[tuple.Item1];
            testRun.CustomFields = testRun.CustomFields ?? new List<TestExtensionField>();
            testRun.CustomFields.Add(tuple.Item2);
          }
        }
        return dictionary1.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns");
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
        this.PrepareDynamicProcedure("prc_QueryTestRuns2_V2");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        TestManagementDatabase6.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase6.QueryTestRunColumns();
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
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestRuns2");
        TestManagementDatabase.FetchTestRunsExColumns testRunsExColumns = new TestManagementDatabase.FetchTestRunsExColumns();
        while (reader.Read())
        {
          Tuple<int, TestExtensionField> tuple = testRunsExColumns.bind(reader);
          if (dictionary1.ContainsKey(tuple.Item1))
          {
            TestRun testRun = dictionary1[tuple.Item1];
            testRun.CustomFields = testRun.CustomFields ?? new List<TestExtensionField>();
            testRun.CustomFields.Add(tuple.Item2);
          }
        }
        return dictionary1.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns2");
      }
    }

    internal override TestRun QueryTestRunByTmiRunId(
      Guid tmiRunId,
      out string iterationUri,
      out Guid runProjGuid)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunByTmiRunId");
        iterationUri = (string) null;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_QueryTestRunByTmiRunId");
        this.BindGuid("@tmiRunId", tmiRunId);
        TestManagementDatabase6.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase6.QueryTestRunColumns();
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          return (TestRun) null;
        int dataspaceId;
        TestRun testRun = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
        runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        return testRun;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunByTmiRunId");
      }
    }

    public override void UpdateTestRunSummaryAndInsights(
      GuidAndString projectId,
      int testRunId,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      TestResultsContextType resultsContextType)
    {
      this.PrepareStoredProcedure("prc_UpdateTestSummaryAndInsights");
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
      this.ExecuteReader();
    }

    public override void UpdateTestRunSummaryAndInsights2(
      GuidAndString projectId,
      BuildConfiguration buildToUpdate,
      BuildConfiguration previousBuild,
      ReleaseReference releaseToUpdate,
      ReleaseReference previousRelease)
    {
      this.PrepareStoredProcedure("prc_UpdateTestSummaryAndInsights2");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      List<BuildConfiguration> builds1;
      if (buildToUpdate != null)
        builds1 = new List<BuildConfiguration>()
        {
          buildToUpdate
        };
      else
        builds1 = (List<BuildConfiguration>) null;
      this.BindBuildRefTypeTable3("@buildToUpdate", (IEnumerable<BuildConfiguration>) builds1);
      List<BuildConfiguration> builds2;
      if (previousBuild != null)
        builds2 = new List<BuildConfiguration>()
        {
          previousBuild
        };
      else
        builds2 = (List<BuildConfiguration>) null;
      this.BindBuildRefTypeTable3("@previousBuild", (IEnumerable<BuildConfiguration>) builds2);
      this.ExecuteReader();
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
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryAndInsights");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, true, SqlDbType.NVarChar);
      this.BindInt("@buildId", buildRef != null ? buildRef.BuildId : 0);
      this.BindString("@buildUri", buildRef?.BuildUri, 256, true, SqlDbType.NVarChar);
      this.BindInt("@testRunId", 0);
      this.BindBoolean("@returnSummary", returnSummary);
      this.BindBoolean("@returnFailureDetails", returnFailureDetails);
      SqlDataReader reader = this.ExecuteReader();
      RunSummaryAndInsights summaryAndInsights = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>()
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
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsights");
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
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsights");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            summaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (!isBuildOld)
          {
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsights");
            while (reader.Read())
              summaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          }
        }
        if (returnFailureDetails && !isBuildOld)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsights");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            summaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
      }
      return summaryAndInsights;
    }

    private new class QueryTestRunColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder DropLocation = new SqlColumnBinder(nameof (DropLocation));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));
      private SqlColumnBinder RepoId = new SqlColumnBinder(nameof (RepoId));
      private SqlColumnBinder RepoType = new SqlColumnBinder(nameof (RepoType));
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder SourceVersion = new SqlColumnBinder(nameof (SourceVersion));
      private SqlColumnBinder BuildSystem = new SqlColumnBinder(nameof (BuildSystem));
      private SqlColumnBinder StartDate = new SqlColumnBinder(nameof (StartDate));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      private SqlColumnBinder PostProcessState = new SqlColumnBinder(nameof (PostProcessState));
      private SqlColumnBinder DueDate = new SqlColumnBinder(nameof (DueDate));
      private SqlColumnBinder IterationUri = new SqlColumnBinder(nameof (IterationUri));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder Controller = new SqlColumnBinder(nameof (Controller));
      private SqlColumnBinder ErrorMessage = new SqlColumnBinder(nameof (ErrorMessage));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestPlanId = new SqlColumnBinder(nameof (TestPlanId));
      private SqlColumnBinder TestMessageLogId = new SqlColumnBinder(nameof (TestMessageLogId));
      private SqlColumnBinder Guid = new SqlColumnBinder(nameof (Guid));
      private SqlColumnBinder LegacySharePath = new SqlColumnBinder(nameof (LegacySharePath));
      private SqlColumnBinder TestSettingsId = new SqlColumnBinder(nameof (TestSettingsId));
      private SqlColumnBinder PublicTestSettingsId = new SqlColumnBinder(nameof (PublicTestSettingsId));
      private SqlColumnBinder TestEnvironmentId = new SqlColumnBinder(nameof (TestEnvironmentId));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Type = new SqlColumnBinder(nameof (Type));
      private SqlColumnBinder IsAutomated = new SqlColumnBinder(nameof (IsAutomated));
      private SqlColumnBinder Version = new SqlColumnBinder(nameof (Version));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder IsBvt = new SqlColumnBinder(nameof (IsBvt));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder RV = new SqlColumnBinder(nameof (RV));
      private SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      private SqlColumnBinder IncompleteTests = new SqlColumnBinder(nameof (IncompleteTests));
      private SqlColumnBinder NotApplicableTests = new SqlColumnBinder(nameof (NotApplicableTests));
      private SqlColumnBinder PassedTests = new SqlColumnBinder(nameof (PassedTests));
      private SqlColumnBinder UnanalyzedTests = new SqlColumnBinder(nameof (UnanalyzedTests));
      private SqlColumnBinder BugsCount = new SqlColumnBinder(nameof (BugsCount));
      private SqlColumnBinder SourceFilter = new SqlColumnBinder(nameof (SourceFilter));
      private SqlColumnBinder TestCaseFilter = new SqlColumnBinder(nameof (TestCaseFilter));
      private SqlColumnBinder TestEnvironmentUrl = new SqlColumnBinder(nameof (TestEnvironmentUrl));
      private SqlColumnBinder AutEnvironmentUrl = new SqlColumnBinder(nameof (AutEnvironmentUrl));
      private SqlColumnBinder Substate = new SqlColumnBinder("SubState");
      private SqlColumnBinder CsmContent = new SqlColumnBinder(nameof (CsmContent));
      private SqlColumnBinder CsmParameters = new SqlColumnBinder(nameof (CsmParameters));
      private SqlColumnBinder SubscriptionName = new SqlColumnBinder(nameof (SubscriptionName));
      private SqlColumnBinder ReleaseUri = new SqlColumnBinder(nameof (ReleaseUri));
      private SqlColumnBinder ReleaseEnvironmentUri = new SqlColumnBinder(nameof (ReleaseEnvironmentUri));

      internal TestRun bind(SqlDataReader reader, out int dataspaceId, out string iterationUri)
      {
        TestRun testRun = new TestRun();
        testRun.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRun.Title = this.Title.GetString((IDataReader) reader, false);
        testRun.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        testRun.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testRun.Owner = this.Owner.GetGuid((IDataReader) reader, false);
        testRun.State = this.State.GetByte((IDataReader) reader);
        testRun.ErrorMessage = this.ErrorMessage.GetString((IDataReader) reader, false);
        testRun.BuildUri = this.BuildUri.GetString((IDataReader) reader, true);
        testRun.DropLocation = this.DropLocation.GetString((IDataReader) reader, true);
        testRun.BuildNumber = this.BuildNumber.GetString((IDataReader) reader, true);
        testRun.BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, true);
        testRun.BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, true);
        testRun.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader, 0);
        testRun.StartDate = this.StartDate.GetDateTime((IDataReader) reader);
        testRun.CompleteDate = this.CompleteDate.GetDateTime((IDataReader) reader);
        testRun.PostProcessState = this.PostProcessState.GetByte((IDataReader) reader);
        testRun.DueDate = this.DueDate.GetDateTime((IDataReader) reader);
        testRun.IterationId = this.IterationId.GetInt32((IDataReader) reader, 0);
        testRun.Controller = this.Controller.GetString((IDataReader) reader, true);
        testRun.TestPlanId = this.TestPlanId.GetInt32((IDataReader) reader);
        testRun.TestMessageLogId = this.TestMessageLogId.GetInt32((IDataReader) reader);
        testRun.LegacySharePath = this.LegacySharePath.GetString((IDataReader) reader, false);
        testRun.TestSettingsId = this.TestSettingsId.GetInt32((IDataReader) reader);
        testRun.PublicTestSettingsId = this.PublicTestSettingsId.GetInt32((IDataReader) reader);
        testRun.TestEnvironmentId = this.TestEnvironmentId.GetGuid((IDataReader) reader);
        testRun.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        testRun.Type = this.Type.GetByte((IDataReader) reader);
        testRun.IsAutomated = this.IsAutomated.GetBoolean((IDataReader) reader);
        testRun.Version = this.Version.GetInt32((IDataReader) reader);
        testRun.Revision = this.Revision.GetInt32((IDataReader) reader);
        testRun.IsBvt = this.IsBvt.GetBoolean((IDataReader) reader);
        testRun.Comment = this.Comment.GetString((IDataReader) reader, false);
        testRun.RowVersion = this.RV.GetBytes((IDataReader) reader, false);
        testRun.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        testRun.IncompleteTests = this.IncompleteTests.GetInt32((IDataReader) reader);
        testRun.NotApplicableTests = this.NotApplicableTests.GetInt32((IDataReader) reader);
        testRun.PassedTests = this.PassedTests.GetInt32((IDataReader) reader);
        testRun.UnanalyzedTests = this.UnanalyzedTests.GetInt32((IDataReader) reader);
        testRun.BugsCount = this.BugsCount.GetInt32((IDataReader) reader);
        testRun.ReleaseUri = this.ReleaseUri.GetString((IDataReader) reader, true);
        testRun.ReleaseEnvironmentUri = this.ReleaseEnvironmentUri.GetString((IDataReader) reader, true);
        testRun.BuildReference = new BuildConfiguration()
        {
          BuildId = this.BuildId.ColumnExists((IDataReader) reader) ? this.BuildId.GetInt32((IDataReader) reader, 0) : 0,
          BuildUri = this.BuildUri.GetString((IDataReader) reader, true),
          BuildNumber = this.BuildNumber.GetString((IDataReader) reader, true),
          BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, true),
          BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, true),
          BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader, 0),
          RepositoryId = this.RepoId.ColumnExists((IDataReader) reader) ? this.RepoId.GetString((IDataReader) reader, true) : string.Empty,
          RepositoryType = this.RepoType.ColumnExists((IDataReader) reader) ? this.RepoType.GetString((IDataReader) reader, true) : string.Empty,
          BuildDefinitionId = this.BuildDefinitionId.ColumnExists((IDataReader) reader) ? this.BuildDefinitionId.GetInt32((IDataReader) reader, 0) : 0,
          BranchName = this.BranchName.ColumnExists((IDataReader) reader) ? this.BranchName.GetString((IDataReader) reader, true) : string.Empty,
          SourceVersion = this.SourceVersion.ColumnExists((IDataReader) reader) ? this.SourceVersion.GetString((IDataReader) reader, true) : string.Empty,
          BuildSystem = this.BuildSystem.ColumnExists((IDataReader) reader) ? this.BuildSystem.GetString((IDataReader) reader, true) : string.Empty
        };
        if (((int) testRun.Type & 16) != 0)
        {
          testRun.DtlTestEnvironment = new ShallowReference()
          {
            Url = this.TestEnvironmentUrl.GetString((IDataReader) reader, true)
          };
          testRun.DtlAutEnvironment = new ShallowReference()
          {
            Url = this.AutEnvironmentUrl.GetString((IDataReader) reader, true)
          };
          if (testRun.IsAutomated)
          {
            testRun.Filter = new RunFilter();
            testRun.Filter.SourceFilter = this.SourceFilter.GetString((IDataReader) reader, false);
            testRun.Filter.TestCaseFilter = this.TestCaseFilter.GetString((IDataReader) reader, true);
            testRun.Substate = this.Substate.GetByte((IDataReader) reader);
            testRun.CsmContent = this.CsmContent.GetString((IDataReader) reader, true);
            testRun.CsmParameters = this.CsmParameters.GetString((IDataReader) reader, true);
            testRun.SubscriptionName = this.SubscriptionName.GetString((IDataReader) reader, true);
          }
        }
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        iterationUri = this.IterationUri.GetString((IDataReader) reader, true);
        return testRun;
      }
    }

    private new class UpdatedPropertyColumns
    {
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      internal SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));
      internal SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      internal SqlColumnBinder IsRunStarted = new SqlColumnBinder(nameof (IsRunStarted));
      internal SqlColumnBinder IsRunCompleted = new SqlColumnBinder(nameof (IsRunCompleted));

      internal UpdatedProperties bindUpdatedProperties(SqlDataReader reader) => new UpdatedProperties()
      {
        Revision = this.Revision.GetInt32((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader)
      };

      internal BlockedPointProperties bindBlockedTestPointProperties(SqlDataReader reader)
      {
        BlockedPointProperties blockedPointProperties = new BlockedPointProperties();
        blockedPointProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        blockedPointProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        blockedPointProperties.LastTestResultId = this.LastTestResultId.GetInt32((IDataReader) reader);
        return blockedPointProperties;
      }

      internal UpdatedRunProperties bindUpdatedRunProperties(SqlDataReader reader)
      {
        UpdatedRunProperties updatedRunProperties = new UpdatedRunProperties();
        updatedRunProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        updatedRunProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        updatedRunProperties.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        updatedRunProperties.IsRunStarted = this.IsRunStarted.GetBoolean((IDataReader) reader);
        updatedRunProperties.IsRunCompleted = this.IsRunCompleted.GetBoolean((IDataReader) reader);
        return updatedRunProperties;
      }
    }

    private new class CreateTestRunColumns
    {
      internal SqlColumnBinder testRunId = new SqlColumnBinder("TestRunId");
      internal SqlColumnBinder revision = new SqlColumnBinder("Revision");

      internal TestRun bind(SqlDataReader reader)
      {
        TestRun testRun = new TestRun();
        testRun.TestRunId = this.testRunId.GetInt32((IDataReader) reader);
        testRun.Revision = this.revision.GetInt32((IDataReader) reader);
        return testRun;
      }
    }
  }
}
