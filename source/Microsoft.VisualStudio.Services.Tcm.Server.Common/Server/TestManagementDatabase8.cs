// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase8
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
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase8 : TestManagementDatabase7
  {
    internal TestManagementDatabase8(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase8()
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
        this.PrepareStoredProcedure("TestResult.prc_ResetTestResult");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@testResultId", testResultId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindInt("@maxCommentSize", TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext));
        SqlDataReader reader = this.ExecuteReader();
        TestCaseResult testCaseResult = reader.Read() ? new TestManagementDatabase.FetchTestResultsColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_ResetTestResult");
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
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
        this.PrepareStoredProcedure("TestResult.prc_AbortTestRun");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@revision", revision);
        this.BindInt("@options", (int) options);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindByte("@substate", substate);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase8.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_AbortTestRun");
        updatedProperties.LastUpdatedBy = updatedBy;
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
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
        UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase8.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_CancelTestRun");
        updatedProperties.LastUpdatedBy = canceledBy;
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
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
        List<TestRun> source = dictionary1.Values.ToList<TestRun>();
        if (planId != -1)
          source = source.FindAll((Predicate<TestRun>) (testRun => testRun.TestPlanId == planId));
        if (source != null)
          source = source.Skip<TestRun>(skip).Take<TestRun>(top).ToList<TestRun>();
        return source;
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
        this.PrepareDynamicProcedure("prc_QueryTestRuns2_V3");
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
        if (reader.NextResult())
        {
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
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_QueryTestRunByTmiRunId");
        this.BindGuid("@tmiRunId", tmiRunId);
        TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
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

    public override void UpdateTestRunSummaryAndInsights(
      GuidAndString projectId,
      int testRunId,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      TestResultsContextType resultsContextType)
    {
      this.PrepareStoredProcedure("TestResult.prc_UpdateTestSummaryAndInsights");
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
      this.BindInt("@resultsContextType", (int) resultsContextType);
      this.ExecuteReader();
    }

    public override void UpdateTestRunSummaryAndInsights2(
      GuidAndString projectId,
      BuildConfiguration buildToUpdate,
      BuildConfiguration previousBuild,
      ReleaseReference releaseToUpdate,
      ReleaseReference previousRelease)
    {
      this.PrepareStoredProcedure("TestResult.prc_UpdateTestSummaryAndInsights2");
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
      List<ReleaseReference> releases1;
      if (releaseToUpdate != null)
        releases1 = new List<ReleaseReference>()
        {
          releaseToUpdate
        };
      else
        releases1 = (List<ReleaseReference>) null;
      this.BindReleaseRefTypeTable2("@releaseToUpdate", (IEnumerable<ReleaseReference>) releases1);
      List<ReleaseReference> releases2;
      if (previousRelease != null)
        releases2 = new List<ReleaseReference>()
        {
          previousRelease
        };
      else
        releases2 = (List<ReleaseReference>) null;
      this.BindReleaseRefTypeTable2("@previousRelease", (IEnumerable<ReleaseReference>) releases2);
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
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryAndInsightsForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindInt("@buildId", buildRef != null ? buildRef.BuildId : 0);
      this.BindString("@buildUri", buildRef?.BuildUri, 256, true, SqlDbType.NVarChar);
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
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
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
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            summaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (!isBuildOld)
          {
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
            while (reader.Read())
              summaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          }
        }
        if (returnFailureDetails && !isBuildOld)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            summaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
      }
      return summaryAndInsights;
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
      if (reader.Read())
        runsCount = new SqlColumnBinder("RunsCount").GetInt32((IDataReader) reader);
      if (runsCount > 0)
      {
        if (returnSummary)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForRelease");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            summaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (reader.NextResult())
          {
            while (reader.Read())
              summaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          }
        }
        if (returnFailureDetails)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForRelease");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            summaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
      }
      return summaryAndInsights;
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
              PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>()
            }
          };
        dictionary[key].TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
      }
      return dictionary;
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild(
      Guid projectId,
      TestResultTrendFilter filter)
    {
      string empty = string.Empty;
      string str1 = string.Empty;
      string str2 = string.Format(SQLConstants.FilterClause_IN, (object) TestResultsConstants.BuildDefinitionIdColumnName, (object) "SELECT Id FROM @definitionIds");
      if (filter.BranchNames != null && filter.BranchNames.Count > 0)
      {
        string str3 = string.Format(SQLConstants.FilterClause_IN, (object) TestResultsConstants.BranchNameColumnName, (object) "SELECT BranchName FROM @branchNames");
        str2 += " " + SQLConstants.Operator_AND + " " + str3;
      }
      if (filter.TestRunTitles != null && filter.TestRunTitles.Count > 0)
        str1 = string.Format(SQLConstants.FilterClause_IN, (object) TestResultsConstants.TestRunTitleColumnName, (object) "SELECT Name FROM @runTitles");
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementDynamicSqlBatchStatements.dynprc_QueryTestResultTrendForBuild, !string.IsNullOrEmpty(str2) ? (object) str2 : (object) TestResultsConstants.TrueCondition, !string.IsNullOrEmpty(str1) ? (object) str1 : (object) TestResultsConstants.TrueCondition);
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement, 0, true, true);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", filter.PublishContext, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@definitionIds", filter.DefinitionIds != null ? filter.DefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindBranchNameTypeTable("@branchNames", (IEnumerable<string>) filter.BranchNames);
      this.BindNameTypeTable("@runTitles", (IEnumerable<string>) filter.TestRunTitles);
      this.BindInt("@buildCount", filter.BuildCount);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      TestManagementDatabase8.QueryAggregatedResultsForBuild aggregatedResultsForBuild = new TestManagementDatabase8.QueryAggregatedResultsForBuild();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader.HasRows)
        {
          while (reader.Read())
            aggregatedResultsForBuild.bindBuildReference(reader, aggregatedResultsMap);
          if (reader.NextResult())
          {
            while (reader.Read())
              aggregatedResultsForBuild.bindAggregateValues(reader, aggregatedResultsMap);
          }
        }
      }
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
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

    protected new class QueryTestRunColumns
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
      private SqlColumnBinder TargetBranchName = new SqlColumnBinder(nameof (TargetBranchName));
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
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder ReleaseDefId = new SqlColumnBinder(nameof (ReleaseDefId));
      private SqlColumnBinder ReleaseEnvDefId = new SqlColumnBinder(nameof (ReleaseEnvDefId));
      private SqlColumnBinder ReleaseRefId = new SqlColumnBinder(nameof (ReleaseRefId));
      private SqlColumnBinder ReleaseCreationDate = new SqlColumnBinder(nameof (ReleaseCreationDate));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));
      private SqlColumnBinder StageName = new SqlColumnBinder(nameof (StageName));
      private SqlColumnBinder StageAttempt = new SqlColumnBinder(nameof (StageAttempt));
      private SqlColumnBinder PhaseName = new SqlColumnBinder(nameof (PhaseName));
      private SqlColumnBinder PhaseAttempt = new SqlColumnBinder(nameof (PhaseAttempt));
      private SqlColumnBinder JobName = new SqlColumnBinder(nameof (JobName));
      private SqlColumnBinder JobAttempt = new SqlColumnBinder(nameof (JobAttempt));

      internal TestRun bind(SqlDataReader reader, out int dataspaceId, out string iterationUri)
      {
        TestRun testRun = new TestRun();
        testRun.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRun.Title = this.Title.GetString((IDataReader) reader, false);
        testRun.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        testRun.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testRun.Owner = this.Owner.GetGuid((IDataReader) reader, false);
        testRun.State = this.State.GetByte((IDataReader) reader);
        testRun.BuildNumber = this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : (string) null;
        testRun.BuildPlatform = this.BuildPlatform.ColumnExists((IDataReader) reader) ? this.BuildPlatform.GetString((IDataReader) reader, true) : (string) null;
        testRun.BuildFlavor = this.BuildFlavor.ColumnExists((IDataReader) reader) ? this.BuildFlavor.GetString((IDataReader) reader, true) : (string) null;
        testRun.StartDate = this.StartDate.GetDateTime((IDataReader) reader);
        testRun.CompleteDate = this.CompleteDate.GetDateTime((IDataReader) reader);
        testRun.TestPlanId = this.TestPlanId.GetInt32((IDataReader) reader);
        testRun.PublicTestSettingsId = this.PublicTestSettingsId.GetInt32((IDataReader) reader);
        testRun.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        testRun.IsAutomated = this.IsAutomated.GetBoolean((IDataReader) reader);
        testRun.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        testRun.IncompleteTests = this.IncompleteTests.GetInt32((IDataReader) reader);
        testRun.NotApplicableTests = this.NotApplicableTests.GetInt32((IDataReader) reader);
        testRun.PassedTests = this.PassedTests.GetInt32((IDataReader) reader);
        testRun.UnanalyzedTests = this.UnanalyzedTests.GetInt32((IDataReader) reader);
        testRun.ErrorMessage = this.ErrorMessage.ColumnExists((IDataReader) reader) ? this.ErrorMessage.GetString((IDataReader) reader, false) : string.Empty;
        testRun.BuildUri = this.BuildUri.ColumnExists((IDataReader) reader) ? this.BuildUri.GetString((IDataReader) reader, true) : string.Empty;
        testRun.DropLocation = this.DropLocation.ColumnExists((IDataReader) reader) ? this.DropLocation.GetString((IDataReader) reader, true) : string.Empty;
        testRun.PostProcessState = this.PostProcessState.ColumnExists((IDataReader) reader) ? this.PostProcessState.GetByte((IDataReader) reader) : (byte) 0;
        testRun.DueDate = this.DueDate.ColumnExists((IDataReader) reader) ? this.DueDate.GetDateTime((IDataReader) reader) : DateTime.MinValue;
        testRun.IterationId = this.IterationId.ColumnExists((IDataReader) reader) ? this.IterationId.GetInt32((IDataReader) reader, 0) : 0;
        testRun.Controller = this.Controller.ColumnExists((IDataReader) reader) ? this.Controller.GetString((IDataReader) reader, true) : string.Empty;
        testRun.BuildConfigurationId = this.BuildConfigurationId.ColumnExists((IDataReader) reader) ? this.BuildConfigurationId.GetInt32((IDataReader) reader, 0) : 0;
        testRun.TestMessageLogId = this.TestMessageLogId.ColumnExists((IDataReader) reader) ? this.TestMessageLogId.GetInt32((IDataReader) reader) : 0;
        testRun.LegacySharePath = this.LegacySharePath.ColumnExists((IDataReader) reader) ? this.LegacySharePath.GetString((IDataReader) reader, false) : string.Empty;
        testRun.TestSettingsId = this.TestSettingsId.ColumnExists((IDataReader) reader) ? this.TestSettingsId.GetInt32((IDataReader) reader) : 0;
        testRun.TestEnvironmentId = this.TestEnvironmentId.ColumnExists((IDataReader) reader) ? this.TestEnvironmentId.GetGuid((IDataReader) reader) : Guid.Empty;
        testRun.Type = this.Type.ColumnExists((IDataReader) reader) ? this.Type.GetByte((IDataReader) reader) : (byte) 0;
        testRun.Version = this.Version.ColumnExists((IDataReader) reader) ? this.Version.GetInt32((IDataReader) reader) : 0;
        testRun.Revision = this.Revision.ColumnExists((IDataReader) reader) ? this.Revision.GetInt32((IDataReader) reader) : 0;
        testRun.IsBvt = this.IsBvt.ColumnExists((IDataReader) reader) && this.IsBvt.GetBoolean((IDataReader) reader);
        testRun.Comment = this.Comment.ColumnExists((IDataReader) reader) ? this.Comment.GetString((IDataReader) reader, false) : string.Empty;
        testRun.RowVersion = this.RV.ColumnExists((IDataReader) reader) ? this.RV.GetBytes((IDataReader) reader, false) : (byte[]) null;
        testRun.BugsCount = this.BugsCount.ColumnExists((IDataReader) reader) ? this.BugsCount.GetInt32((IDataReader) reader) : 0;
        testRun.ReleaseUri = this.ReleaseUri.ColumnExists((IDataReader) reader) ? this.ReleaseUri.GetString((IDataReader) reader, true) : string.Empty;
        testRun.ReleaseEnvironmentUri = this.ReleaseEnvironmentUri.ColumnExists((IDataReader) reader) ? this.ReleaseEnvironmentUri.GetString((IDataReader) reader, true) : string.Empty;
        testRun.ReleaseReference = new ReleaseReference()
        {
          ReleaseId = this.ReleaseId.ColumnExists((IDataReader) reader) ? this.ReleaseId.GetInt32((IDataReader) reader, 0) : 0,
          ReleaseEnvId = this.ReleaseEnvId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvId.GetInt32((IDataReader) reader, 0) : 0,
          ReleaseDefId = this.ReleaseDefId.ColumnExists((IDataReader) reader) ? this.ReleaseDefId.GetInt32((IDataReader) reader, 0) : 0,
          ReleaseEnvDefId = this.ReleaseEnvDefId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvDefId.GetInt32((IDataReader) reader, 0) : 0,
          ReleaseRefId = this.ReleaseRefId.ColumnExists((IDataReader) reader) ? this.ReleaseRefId.GetInt32((IDataReader) reader, 0) : 0,
          ReleaseUri = this.ReleaseUri.ColumnExists((IDataReader) reader) ? this.ReleaseUri.GetString((IDataReader) reader, true) : string.Empty,
          ReleaseEnvUri = this.ReleaseEnvironmentUri.ColumnExists((IDataReader) reader) ? this.ReleaseEnvironmentUri.GetString((IDataReader) reader, true) : string.Empty,
          ReleaseCreationDate = this.ReleaseCreationDate.ColumnExists((IDataReader) reader) ? this.ReleaseCreationDate.GetDateTime((IDataReader) reader, DateTime.Now) : DateTime.Now,
          ReleaseName = this.ReleaseName.ColumnExists((IDataReader) reader) ? this.ReleaseName.GetString((IDataReader) reader, true) : string.Empty
        };
        testRun.BuildReference = new BuildConfiguration()
        {
          BuildId = this.BuildId.ColumnExists((IDataReader) reader) ? this.BuildId.GetInt32((IDataReader) reader, 0) : 0,
          BuildUri = this.BuildUri.ColumnExists((IDataReader) reader) ? this.BuildUri.GetString((IDataReader) reader, true) : (string) null,
          BuildNumber = this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : (string) null,
          BuildPlatform = this.BuildPlatform.ColumnExists((IDataReader) reader) ? this.BuildPlatform.GetString((IDataReader) reader, true) : (string) null,
          BuildFlavor = this.BuildFlavor.ColumnExists((IDataReader) reader) ? this.BuildFlavor.GetString((IDataReader) reader, true) : (string) null,
          BuildConfigurationId = this.BuildConfigurationId.ColumnExists((IDataReader) reader) ? this.BuildConfigurationId.GetInt32((IDataReader) reader, 0) : 0,
          RepositoryId = this.RepoId.ColumnExists((IDataReader) reader) ? this.RepoId.GetString((IDataReader) reader, true) : string.Empty,
          RepositoryType = this.RepoType.ColumnExists((IDataReader) reader) ? this.RepoType.GetString((IDataReader) reader, true) : string.Empty,
          BuildDefinitionId = this.BuildDefinitionId.ColumnExists((IDataReader) reader) ? this.BuildDefinitionId.GetInt32((IDataReader) reader, 0) : 0,
          BranchName = this.BranchName.ColumnExists((IDataReader) reader) ? this.BranchName.GetString((IDataReader) reader, true) : string.Empty,
          TargetBranchName = this.TargetBranchName.ColumnExists((IDataReader) reader) ? this.TargetBranchName.GetString((IDataReader) reader, true) : string.Empty,
          SourceVersion = this.SourceVersion.ColumnExists((IDataReader) reader) ? this.SourceVersion.GetString((IDataReader) reader, true) : string.Empty,
          BuildSystem = this.BuildSystem.ColumnExists((IDataReader) reader) ? this.BuildSystem.GetString((IDataReader) reader, true) : string.Empty
        };
        if (testRun.BuildReference.BuildId > 0)
        {
          testRun.PipelineReference = new PipelineReference()
          {
            PipelineId = testRun.BuildReference.BuildId,
            PipelineDefinitionId = testRun.BuildReference.BuildDefinitionId
          };
          if (this.StageName.ColumnExists((IDataReader) reader))
            testRun.PipelineReference.StageReference = new StageReference()
            {
              StageName = this.StageName.GetString((IDataReader) reader, true),
              Attempt = this.StageAttempt.GetInt32((IDataReader) reader, 0, 0)
            };
          if (this.PhaseName.ColumnExists((IDataReader) reader))
            testRun.PipelineReference.PhaseReference = new PhaseReference()
            {
              PhaseName = this.PhaseName.GetString((IDataReader) reader, true),
              Attempt = this.PhaseAttempt.GetInt32((IDataReader) reader, 0, 0)
            };
          if (this.JobName.ColumnExists((IDataReader) reader))
            testRun.PipelineReference.JobReference = new JobReference()
            {
              JobName = this.JobName.GetString((IDataReader) reader, true),
              Attempt = this.JobAttempt.GetInt32((IDataReader) reader, 0, 0)
            };
        }
        if (((int) testRun.Type & 16) != 0)
        {
          testRun.DtlTestEnvironment = new ShallowReference()
          {
            Url = this.TestEnvironmentUrl.ColumnExists((IDataReader) reader) ? this.TestEnvironmentUrl.GetString((IDataReader) reader, true) : (string) null
          };
          testRun.DtlAutEnvironment = new ShallowReference()
          {
            Url = this.AutEnvironmentUrl.ColumnExists((IDataReader) reader) ? this.AutEnvironmentUrl.GetString((IDataReader) reader, true) : (string) null
          };
          if (testRun.IsAutomated)
          {
            testRun.Filter = new RunFilter();
            testRun.Filter.SourceFilter = this.SourceFilter.ColumnExists((IDataReader) reader) ? this.SourceFilter.GetString((IDataReader) reader, false) : (string) null;
            testRun.Filter.TestCaseFilter = this.TestCaseFilter.ColumnExists((IDataReader) reader) ? this.TestCaseFilter.GetString((IDataReader) reader, true) : (string) null;
            testRun.Substate = this.Substate.ColumnExists((IDataReader) reader) ? this.Substate.GetByte((IDataReader) reader) : (byte) 0;
            testRun.CsmContent = this.CsmContent.ColumnExists((IDataReader) reader) ? this.CsmContent.GetString((IDataReader) reader, true) : (string) null;
            testRun.CsmParameters = this.CsmParameters.ColumnExists((IDataReader) reader) ? this.CsmParameters.GetString((IDataReader) reader, true) : (string) null;
            testRun.SubscriptionName = this.SubscriptionName.ColumnExists((IDataReader) reader) ? this.SubscriptionName.GetString((IDataReader) reader, true) : (string) null;
          }
        }
        dataspaceId = this.DataspaceId.ColumnExists((IDataReader) reader) ? this.DataspaceId.GetInt32((IDataReader) reader) : 0;
        iterationUri = this.IterationUri.ColumnExists((IDataReader) reader) ? this.IterationUri.GetString((IDataReader) reader, true) : (string) null;
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

    protected class QueryAggregatedResultsForBuild
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

      internal void bindBuildReference(
        SqlDataReader reader,
        Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap)
      {
        int int32_1 = this.BuildId.GetInt32((IDataReader) reader);
        string str1 = this.BuildUri.GetString((IDataReader) reader, false);
        string str2 = this.BuildNumber.GetString((IDataReader) reader, true);
        int int32_2 = this.DefinitionId.GetInt32((IDataReader) reader);
        string str3 = this.BranchName.GetString((IDataReader) reader, true);
        if (aggregatedResultsMap.ContainsKey(int32_1))
          return;
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
          ResultsByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>(),
          Duration = new TimeSpan()
        });
      }

      internal void bindAggregateValues(
        SqlDataReader reader,
        Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap)
      {
        int int32_1 = this.BuildId.GetInt32((IDataReader) reader);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome key = (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) this.TestOutcome.GetByte((IDataReader) reader);
        int int32_2 = this.ResultCount.GetInt32((IDataReader) reader);
        int int32_3 = this.TestRunId.GetInt32((IDataReader) reader);
        long int64 = Convert.ToInt64(this.RunDuration.GetObject((IDataReader) reader));
        if (!aggregatedResultsMap.ContainsKey(int32_1))
          return;
        if (!aggregatedResultsMap[int32_1].Item2.ResultsByOutcome.ContainsKey(key))
          aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key] = new AggregatedResultsByOutcome()
          {
            Count = 0,
            Outcome = key
          };
        aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key].Count += int32_2;
        if (aggregatedResultsMap[int32_1].Item1.Contains(int32_3))
          return;
        aggregatedResultsMap[int32_1].Item1.Add(int32_3);
        aggregatedResultsMap[int32_1].Item2.Duration = Validator.CheckOverflowAndGetSafeValue(aggregatedResultsMap[int32_1].Item2.Duration, TimeSpan.FromMilliseconds((double) int64));
      }
    }
  }
}
