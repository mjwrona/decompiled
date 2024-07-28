// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase67
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
  public class TestManagementDatabase67 : TestManagementDatabase66
  {
    public override long UpdateLogStoreProjectSummary(
      Guid projectId,
      long blobSize,
      bool isinitialize = false)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "LogStorage", nameof (UpdateLogStoreProjectSummary));
      this.PrepareStoredProcedure("prc_UpdateLogStoreProjectSummary");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindLong("@blobSize", blobSize);
      this.BindBoolean("@isInitialize", isinitialize);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TotalBlobSize");
      if (reader.Read())
        return sqlColumnBinder.GetInt64((IDataReader) reader);
      this.RequestContext.TraceLeave(0, "TestManagement", "LogStorage", nameof (UpdateLogStoreProjectSummary));
      return 0;
    }

    public override void UpdateLogStoreContainerStateField(
      Guid projectId,
      int testRunId,
      int fieldId,
      int fieldValue,
      int newFieldValue,
      int batchSize)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "LogStorage", nameof (UpdateLogStoreContainerStateField));
      this.PrepareStoredProcedure("prc_UpdateTestRunsForLogStoreScan");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@fieldId", fieldId);
      this.BindInt("@fieldValue", fieldValue);
      this.BindInt("@newFieldValue", newFieldValue);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteReader();
      this.RequestContext.TraceLeave(0, "TestManagement", "LogStorage", nameof (UpdateLogStoreContainerStateField));
    }

    internal TestManagementDatabase67(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase67()
    {
    }

    public override void UpdateTestRunExtension(
      Guid projectId,
      IEnumerable<Tuple<int, TestExtensionField>> extensionFields)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "LogStorage", nameof (UpdateTestRunExtension));
      this.PrepareStoredProcedure("prc_UpdateTestRunExt");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindTestExtensionFieldValuesTypeTable("@additionalRunFields", (IEnumerable<Tuple<int, int, TestExtensionField>>) this.GetExtensionFieldsMap(extensionFields));
      this.ExecuteNonQuery();
      this.RequestContext.TraceLeave(0, "TestManagement", "LogStorage", nameof (UpdateTestRunExtension));
    }

    public override long UpdateLogStoreContentSizeByBuild(
      Guid projectId,
      int buildId,
      int fieldId,
      int stateFieldId,
      int stateFieldValue,
      int newStateFieldValue,
      bool isDeleted = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", nameof (UpdateLogStoreContentSizeByBuild));
        this.PrepareStoredProcedure("prc_UpdateLogStoreContentSizeByBuild");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@buildId", buildId);
        this.BindInt("@fieldId", fieldId);
        this.BindInt("@stateFieldId", stateFieldId);
        this.BindInt("@stateFieldValue", stateFieldValue);
        this.BindInt("@newStateFieldValue", newStateFieldValue);
        this.BindBoolean("@isDeleted", isDeleted);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TotalBlobSize");
        if (reader.Read())
          return sqlColumnBinder.GetInt64((IDataReader) reader);
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", nameof (UpdateLogStoreContentSizeByBuild));
      }
      return 0;
    }

    public override long UpdateLogStoreContentSizeByRuns(
      Guid projectId,
      List<int> runIds,
      int fieldId,
      int stateFieldId,
      int stateFieldValue,
      int newStateFieldValue,
      bool isDeleted = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", nameof (UpdateLogStoreContentSizeByRuns));
        this.PrepareStoredProcedure("prc_UpdateLogStoreContentSizeByRuns");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindIdTypeTable("@runIds", (IEnumerable<int>) runIds);
        this.BindInt("@fieldId", fieldId);
        this.BindInt("@stateFieldId", stateFieldId);
        this.BindInt("@stateFieldValue", stateFieldValue);
        this.BindInt("@newStateFieldValue", newStateFieldValue);
        this.BindBoolean("@isDeleted", isDeleted);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TotalBlobSize");
        if (reader.Read())
          return sqlColumnBinder.GetInt64((IDataReader) reader);
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", nameof (UpdateLogStoreContentSizeByRuns));
      }
      return 0;
    }

    private List<Tuple<int, int, TestExtensionField>> GetExtensionFieldsMap(
      IEnumerable<Tuple<int, TestExtensionField>> extensionFields)
    {
      List<Tuple<int, int, TestExtensionField>> extensionFieldsMap = new List<Tuple<int, int, TestExtensionField>>();
      foreach (Tuple<int, TestExtensionField> extensionField in extensionFields)
        extensionFieldsMap.Add(new Tuple<int, int, TestExtensionField>(extensionField.Item1, 0, extensionField.Item2));
      return extensionFieldsMap;
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
