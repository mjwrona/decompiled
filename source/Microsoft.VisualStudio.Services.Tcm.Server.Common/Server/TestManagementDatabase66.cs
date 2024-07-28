// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase66
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
  public class TestManagementDatabase66 : TestManagementDatabase65
  {
    internal TestManagementDatabase66(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase66()
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
        this.BindTestActionResultUpdate2TypeTable("@actionsUpdatedTable", (IEnumerable<TestActionResult>) testActionResultSet1);
        this.BindTestActionResultForDeleteTypeTable("@actionsDeletedTable", (IEnumerable<TestActionResult>) testActionResultSet2);
        this.BindTestResultParameterTypeTable("@parametersUpdatedTable", (IEnumerable<TestResultParameter>) testResultParameterSet1);
        this.BindTestResultParameterForDeleteTypeTable("@parametersDeletedTable", (IEnumerable<TestResultParameter>) testResultParameterSet2);
        this.BindBoolean("@autoComputeTestRunState", autoComputeTestRunState);
        this.BindTestExtensionFieldValuesTypeTable("@additionalResultFields", (IEnumerable<Tuple<int, int, TestExtensionField>>) this.GetExtensionFieldsMap(testRunId, (IEnumerable<TestCaseResult>) results, false));
        this.BindBoolean("@updateRunSummary", updateRunSummary);
        this.BindBoolean("@isTcmService", isTcmService);
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
          int int32 = new SqlColumnBinder("TestPlanId").GetInt32((IDataReader) reader, 0);
          if (int32 != 0)
          {
            foreach (ResultUpdateResponse resultUpdateResponse in dictionary.Values)
              resultUpdateResponse.TestPlanId = int32;
          }
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
      string storedProcedure = "TestResult.prc_FetchTestFailureDetailsInMultipleRuns";
      this.PrepareStoredProcedure(storedProcedure);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindIdTypeTable("@testRuns", (IEnumerable<int>) testRunIds);
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
      this.BindString("@sourceWorkflow", sourceWorkflow, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindBoolean("@fetchPreviousFailedResults", fetchPreviousFailedResults);
      this.BindBoolean("@shouldByPassFlaky", shouldByPassFlaky);
      SqlDataReader reader1 = this.ExecuteReader();
      if (fetchPreviousFailedResults)
      {
        TestManagementDatabase23.FetchTestResultFailuresColumns resultFailuresColumns = new TestManagementDatabase23.FetchTestResultFailuresColumns();
        while (reader1.Read())
        {
          TestCaseResult testCaseResult = resultFailuresColumns.bind(reader1);
          previousFailedResultsMap[testCaseResult.TestCaseReferenceId] = testCaseResult;
        }
        if (!reader1.NextResult())
          throw new UnexpectedDatabaseResultException(storedProcedure);
      }
      while (reader1.Read())
      {
        TestManagementDatabase23.FetchTestResultFailuresColumns resultFailuresColumns = new TestManagementDatabase23.FetchTestResultFailuresColumns();
        int int32 = new SqlColumnBinder("TestRunId").GetInt32((IDataReader) reader1);
        SqlDataReader reader2 = reader1;
        TestCaseResult testCaseResult = resultFailuresColumns.bind(reader2);
        currentFailedResults[int32].Add(testCaseResult);
      }
      if (!reader1.NextResult() || !reader1.Read())
        return;
      prevTestRunContextId = new SqlColumnBinder("TestRunContextId").GetInt32((IDataReader) reader1, 0);
    }

    public override List<TestResultIdentifierRecord> QueryFlakyTestResults(
      Guid projectId,
      List<int> testRunIds)
    {
      List<TestResultIdentifierRecord> identifierRecordList = new List<TestResultIdentifierRecord>();
      this.PrepareStoredProcedure("TestResult.prc_QueryFlakyTestResults");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindIdTypeTable("@testRunIds", (IEnumerable<int>) testRunIds);
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
      {
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TestRunId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("TestResultId");
        SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("TestCaseRefId");
        TestResultIdentifierRecord identifierRecord = new TestResultIdentifierRecord()
        {
          TestRunId = sqlColumnBinder1.GetInt32((IDataReader) reader),
          TestResultId = sqlColumnBinder2.GetInt32((IDataReader) reader),
          TestCaseReferenceId = sqlColumnBinder3.GetInt32((IDataReader) reader)
        };
        identifierRecordList.Add(identifierRecord);
      }
      return identifierRecordList;
    }

    public override void PublishTestSummaryAndFailureDetails(
      GuidAndString projectId,
      List<int> testRunId,
      Dictionary<int, ResultInsights> resultInsights,
      Dictionary<int, Dictionary<int, string>> failingSinceDetails,
      List<TestResultIdentifierRecord> flakyResults,
      bool includeFailureDetails,
      bool publishPassCountOnly = false,
      bool shouldPublishFlakiness = false,
      bool shouldByPassFlaky = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_PublishTestSummaryAndFailureDetails");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindTestResult_FailureCountForTestRunTypeTable("@runsFailureCount", resultInsights);
      this.BindTestResult_FailingSinceDetailsTable("@failingSinceDetails", failingSinceDetails);
      this.BindBoolean("@updateFailureDetails", includeFailureDetails);
      this.BindInt("@prevTestRunContextId", includeFailureDetails ? resultInsights.First<KeyValuePair<int, ResultInsights>>().Value.PrevRunContextId : 0);
      this.BindBoolean("@publishPassCountOnly", publishPassCountOnly);
      this.BindBoolean("@shouldPublishFlakiness", shouldPublishFlakiness);
      this.BindBoolean("@shouldByPassFlaky", shouldByPassFlaky);
      this.BindTestResultIdentifierRecordTypeTable("@flakyTestResults", flakyResults);
      this.ExecuteNonQuery();
    }
  }
}
