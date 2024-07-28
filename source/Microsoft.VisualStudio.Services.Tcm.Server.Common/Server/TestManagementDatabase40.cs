// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase40
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase40 : TestManagementDatabase39
  {
    internal override Dictionary<TestCaseResultIdentifier, List<int>> CreateAttachments(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments,
      bool areSessionAttachments,
      bool changeCounterInterval = false)
    {
      this.AssignTempIdToAttachments(attachments, changeCounterInterval);
      this.PrepareStoredProcedure("TestResult.prc_CreateAttachments");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindTestResultAttachmentTypeTable3("@attachments", attachments);
      this.BindBoolean("@areSessionAttachments", areSessionAttachments);
      this.BindBoolean("@changeCounterInterval", changeCounterInterval);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException("prc_CreateAttachments");
      List<int> intList1 = new List<int>();
      TestManagementDatabase.CreateAttachmentColumns attachmentColumns = new TestManagementDatabase.CreateAttachmentColumns();
      do
      {
        intList1.Add(attachmentColumns.AttachmentId.GetInt32((IDataReader) reader));
      }
      while (reader.Read());
      Dictionary<TestCaseResultIdentifier, List<int>> attachments1 = new Dictionary<TestCaseResultIdentifier, List<int>>();
      int num = 0;
      foreach (TestResultAttachment attachment in attachments)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(attachment.TestRunId, attachment.TestResultId);
        List<int> intList2;
        if (!attachments1.TryGetValue(key, out intList2))
        {
          intList2 = new List<int>();
          attachments1.Add(key, intList2);
        }
        intList2.Add(intList1[num++]);
      }
      return attachments1;
    }

    internal override List<TestResultAttachment> QueryAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int sessionId,
      out string areaUri,
      int subResultId = 0)
    {
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      areaUri = string.Empty;
      this.PrepareStoredProcedure("TestResult.prc_QueryAttachments");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindNullableInt("@attachmentId", attachmentId, 0);
      this.BindInt("@sessionId", sessionId);
      this.BindInt("@subResultId", subResultId);
      SqlDataReader reader = this.ExecuteReader();
      if (testResultId > 0)
        areaUri = this.GetResultAreaUri(reader, testRunId, testResultId);
      if (testRunId > 0 && !reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryAttachments");
      TestManagementDatabase.QueryAttachmentsColumns attachmentsColumns = new TestManagementDatabase.QueryAttachmentsColumns();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumns.Bind(reader));
      return resultAttachmentList;
    }

    internal TestManagementDatabase40(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase40()
    {
    }

    internal override List<ResultUpdateResponse> UpdateTestResults2(
      Guid projectId,
      int testRunId,
      IEnumerable<ResultUpdateRequest> resultsForUpdate,
      Guid updatedBy,
      bool autoComputeTestRunState,
      bool updateRunSummary,
      bool isTcmService,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.UpdateTestResults2");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        run = (TestRun) null;
        TestActionResultUpdateComparator comparer1 = new TestActionResultUpdateComparator();
        TestCaseResultUpdateResultComparator comparer2 = new TestCaseResultUpdateResultComparator();
        TestResultParameterUpdateResultComparator comparer3 = new TestResultParameterUpdateResultComparator();
        HashSet<TestCaseResult> results = new HashSet<TestCaseResult>((IEqualityComparer<TestCaseResult>) comparer2);
        HashSet<TestActionResult> testActionResultSet1 = new HashSet<TestActionResult>((IEqualityComparer<TestActionResult>) comparer1);
        HashSet<TestActionResult> testActionResultSet2 = new HashSet<TestActionResult>((IEqualityComparer<TestActionResult>) comparer1);
        HashSet<TestResultParameter> testResultParameterSet1 = new HashSet<TestResultParameter>((IEqualityComparer<TestResultParameter>) comparer3);
        HashSet<TestResultParameter> testResultParameterSet2 = new HashSet<TestResultParameter>((IEqualityComparer<TestResultParameter>) comparer3);
        Dictionary<int, ResultUpdateResponse> dictionary = new Dictionary<int, ResultUpdateResponse>();
        List<ResultUpdateResponse> resultUpdateResponseList = new List<ResultUpdateResponse>();
        if (!this.GetTestCaseResultsFromResultUpdateRequests(resultsForUpdate, resultUpdateResponseList, results, testActionResultSet1, testActionResultSet2, testResultParameterSet1, testResultParameterSet2))
          return resultUpdateResponseList;
        if (results.Count == 1)
          this.PrepareStoredProcedure("TestResult.prc_UpdateTestResultsSingularly2");
        else
          this.PrepareStoredProcedure("TestResult.prc_UpdateTestResults2");
        this.BindTestResult_TestCaseResultUpdate3TypeTable("@resultsTable", (IEnumerable<TestCaseResult>) results);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindTestActionResultUpdateTypeTable("@actionsUpdatedTable", (IEnumerable<TestActionResult>) testActionResultSet1);
        this.BindTestActionResultForDeleteTypeTable("@actionsDeletedTable", (IEnumerable<TestActionResult>) testActionResultSet2);
        this.BindTestResultParameterTypeTable("@parametersUpdatedTable", (IEnumerable<TestResultParameter>) testResultParameterSet1);
        this.BindTestResultParameterForDeleteTypeTable("@parametersDeletedTable", (IEnumerable<TestResultParameter>) testResultParameterSet2);
        this.BindBoolean("@autoComputeTestRunState", autoComputeTestRunState);
        this.BindTestExtensionFieldValuesTypeTable("@additionalResultFields", (IEnumerable<Tuple<int, int, TestExtensionField>>) this.GetExtensionFieldsMap(testRunId, (IEnumerable<TestCaseResult>) results, false));
        this.BindBoolean("@updateRunSummary", updateRunSummary);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          throw new UnexpectedDatabaseResultException("prc_UpdateTestResults2");
        TestManagementDatabase.UpdateTestResultColumns testResultColumns = new TestManagementDatabase.UpdateTestResultColumns();
        do
        {
          ResultUpdateResponse resultUpdateResponse = testResultColumns.bind(reader);
          dictionary.Add(resultUpdateResponse.TestResultId, resultUpdateResponse);
        }
        while (reader.Read());
        TestManagementDatabase39.QueryTestResultMaxSubResultIds resultMaxSubResultIds = new TestManagementDatabase39.QueryTestResultMaxSubResultIds();
        if (reader.NextResult() && reader.Read())
        {
          do
          {
            int testResultId;
            int num = resultMaxSubResultIds.bind(reader, out testResultId);
            if (dictionary.ContainsKey(testResultId))
              dictionary[testResultId].MaxReservedSubResultId = num;
          }
          while (reader.Read());
        }
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        if (resultUpdateResponseList != null && resultUpdateResponseList.Count<ResultUpdateResponse>() > 0)
        {
          foreach (ResultUpdateResponse resultUpdateResponse in resultUpdateResponseList)
          {
            if (!dictionary.ContainsKey(resultUpdateResponse.TestResultId))
              dictionary.Add(resultUpdateResponse.TestResultId, resultUpdateResponse);
          }
        }
        return dictionary.Values.ToList<ResultUpdateResponse>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.UpdateTestResults2");
      }
    }

    public override List<TestRun> GetTestRunIdsWithoutInsightsForBuild(Guid projectId, int buildId)
    {
      this.PrepareStoredProcedure("TestResult.prc_GetTestRunIdsWithoutInsightsForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@buildId", buildId);
      SqlDataReader reader = this.ExecuteReader();
      List<TestRun> insightsForBuild = new List<TestRun>();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TestRunId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("BuildConfigurationId");
      SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("BuildId");
      SqlColumnBinder sqlColumnBinder4 = new SqlColumnBinder("BuildNumber");
      SqlColumnBinder sqlColumnBinder5 = new SqlColumnBinder("BuildUri");
      SqlColumnBinder sqlColumnBinder6 = new SqlColumnBinder("BranchName");
      SqlColumnBinder sqlColumnBinder7 = new SqlColumnBinder("CreatedDate");
      while (reader.Read())
        insightsForBuild.Add(new TestRun()
        {
          TestRunId = sqlColumnBinder1.GetInt32((IDataReader) reader),
          BuildReference = new BuildConfiguration()
          {
            BuildConfigurationId = sqlColumnBinder2.GetInt32((IDataReader) reader),
            BuildId = sqlColumnBinder3.GetInt32((IDataReader) reader),
            BuildNumber = sqlColumnBinder4.GetString((IDataReader) reader, false),
            BuildUri = sqlColumnBinder5.GetString((IDataReader) reader, false),
            BranchName = sqlColumnBinder6.GetString((IDataReader) reader, false),
            CreatedDate = sqlColumnBinder7.GetDateTime((IDataReader) reader, new DateTime())
          }
        });
      return insightsForBuild;
    }

    public override List<TestRun> GetTestRunIdsWithoutInsightsForRelease(
      Guid projectId,
      int releaseId)
    {
      this.PrepareStoredProcedure("TestResult.prc_GetTestRunIdsWithoutInsightsForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@releaseId", releaseId);
      SqlDataReader reader = this.ExecuteReader();
      List<TestRun> insightsForRelease = new List<TestRun>();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TestRunId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("ReleaseRefId");
      SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("ReleaseUri");
      SqlColumnBinder sqlColumnBinder4 = new SqlColumnBinder("ReleaseEnvUri");
      SqlColumnBinder sqlColumnBinder5 = new SqlColumnBinder("ReleaseId");
      SqlColumnBinder sqlColumnBinder6 = new SqlColumnBinder("ReleaseEnvId");
      SqlColumnBinder sqlColumnBinder7 = new SqlColumnBinder("Attempt");
      SqlColumnBinder sqlColumnBinder8 = new SqlColumnBinder("ReleaseCreationDate");
      SqlColumnBinder sqlColumnBinder9 = new SqlColumnBinder("ReleaseEnvDefId");
      SqlColumnBinder sqlColumnBinder10 = new SqlColumnBinder("ReleaseDefId");
      SqlColumnBinder sqlColumnBinder11 = new SqlColumnBinder("ReleaseName");
      while (reader.Read())
        insightsForRelease.Add(new TestRun()
        {
          TestRunId = sqlColumnBinder1.GetInt32((IDataReader) reader),
          ReleaseReference = new ReleaseReference()
          {
            ReleaseRefId = sqlColumnBinder2.GetInt32((IDataReader) reader),
            ReleaseUri = sqlColumnBinder3.GetString((IDataReader) reader, false),
            ReleaseEnvUri = sqlColumnBinder4.GetString((IDataReader) reader, false),
            ReleaseId = sqlColumnBinder5.GetInt32((IDataReader) reader),
            ReleaseEnvId = sqlColumnBinder6.GetInt32((IDataReader) reader),
            Attempt = sqlColumnBinder7.GetInt32((IDataReader) reader),
            ReleaseCreationDate = sqlColumnBinder8.GetDateTime((IDataReader) reader, new DateTime()),
            ReleaseEnvDefId = sqlColumnBinder9.GetInt32((IDataReader) reader),
            ReleaseDefId = sqlColumnBinder10.GetInt32((IDataReader) reader),
            ReleaseName = sqlColumnBinder11.GetString((IDataReader) reader, false)
          }
        });
      return insightsForRelease;
    }

    public override void CreateTestRunCompletedEntry(
      Guid projectId,
      int testRunId,
      int buildId,
      int releaseId,
      int releaseEnvId,
      bool isSimilarResultsEnabled = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_CreateTestRunInsightsCalculationEntry");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@buildId", buildId);
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@releaseEnvId", releaseEnvId);
      this.ExecuteNonQuery();
    }

    public override void DeleteTestRunInsightsCalculationEntry(Guid projectId, List<int> testRunIds)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteTestRunInsightsCalculationEntry");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt32TypeTable("@runsToDelete", (IEnumerable<int>) testRunIds);
      this.ExecuteNonQuery();
    }

    public override void GetTestRunIdsForInsightsCalculation(
      int batchSize,
      out Dictionary<Guid, Dictionary<int, List<int>>> buildTestRunIds,
      out Dictionary<Guid, Dictionary<int, List<int>>> releaseTestRunIds)
    {
      this.PrepareStoredProcedure("TestResult.prc_GetTestRunInsightsCalculationEntries");
      this.BindInt("@batchSize", batchSize);
      buildTestRunIds = new Dictionary<Guid, Dictionary<int, List<int>>>();
      releaseTestRunIds = new Dictionary<Guid, Dictionary<int, List<int>>>();
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
      {
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("DataspaceId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("testRunId");
        SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("buildId");
        SqlColumnBinder sqlColumnBinder4 = new SqlColumnBinder("releaseId");
        SqlColumnBinder sqlColumnBinder5 = new SqlColumnBinder("releaseEnvId");
        int int32_1 = sqlColumnBinder1.GetInt32((IDataReader) reader);
        int int32_2 = sqlColumnBinder2.GetInt32((IDataReader) reader);
        int int32_3 = sqlColumnBinder3.GetInt32((IDataReader) reader);
        int int32_4 = sqlColumnBinder4.GetInt32((IDataReader) reader);
        sqlColumnBinder5.GetInt32((IDataReader) reader);
        Guid dataspaceIdentifier = this.GetDataspaceIdentifier(int32_1);
        if (int32_4 > 0)
        {
          if (!releaseTestRunIds.ContainsKey(dataspaceIdentifier))
            releaseTestRunIds[dataspaceIdentifier] = new Dictionary<int, List<int>>();
          if (!releaseTestRunIds[dataspaceIdentifier].ContainsKey(int32_4))
            releaseTestRunIds[dataspaceIdentifier][int32_4] = new List<int>();
          releaseTestRunIds[dataspaceIdentifier][int32_4].Add(int32_2);
        }
        else if (int32_3 > 0)
        {
          if (!buildTestRunIds.ContainsKey(dataspaceIdentifier))
            buildTestRunIds[dataspaceIdentifier] = new Dictionary<int, List<int>>();
          if (!buildTestRunIds[dataspaceIdentifier].ContainsKey(int32_3))
            buildTestRunIds[dataspaceIdentifier][int32_3] = new List<int>();
          buildTestRunIds[dataspaceIdentifier][int32_3].Add(int32_2);
        }
      }
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
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      bool flag = true;
      RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          CurrentAggregatedRunsByState = (IList<RunSummaryByState>) new List<RunSummaryByState>(),
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      isBuildOld = false;
      runsCount = 0;
      SqlColumnBinder sqlColumnBinder;
      if (reader.Read())
      {
        sqlColumnBinder = new SqlColumnBinder("IsRunsAvailable");
        flag = sqlColumnBinder.GetBoolean((IDataReader) reader);
      }
      if (!flag)
        return runSummaryAndInsights;
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      if (reader.Read())
      {
        ref bool local = ref isBuildOld;
        sqlColumnBinder = new SqlColumnBinder("IsBuildOld");
        int num = sqlColumnBinder.GetBoolean((IDataReader) reader) ? 1 : 0;
        local = num != 0;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader.Read())
      {
        RunSummaryByState runSummaryByState = runsByStateColumns.bind(reader);
        runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runSummaryByState);
        runsCount += runSummaryByState.RunsCount;
      }
      if (runsCount > 0)
      {
        if (returnSummary)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (!isBuildOld)
          {
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
            while (reader.Read())
              runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          }
        }
        if (returnFailureDetails && !isBuildOld)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            runSummaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
        TestManagementDatabase19.PopulateAggregateDataByCategory(categoryName, reader, runSummaryAndInsights, "prc_QueryTestRunSummaryAndInsightsForBuild");
      }
      return runSummaryAndInsights;
    }

    public override void UpdateTestRunSummaryForResults(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results)
    {
      this.PrepareStoredProcedure("TestResult.prc_UpdateTestRunSummaryForResults");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindTestResult_TestCaseResult2TypeTable("@testresultsTable", results, true);
      this.ExecuteReader();
    }
  }
}
