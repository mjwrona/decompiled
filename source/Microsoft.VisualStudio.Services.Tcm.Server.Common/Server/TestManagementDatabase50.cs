// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase50
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
  public class TestManagementDatabase50 : TestManagementDatabase49
  {
    internal TestManagementDatabase50(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase50()
    {
    }

    public override List<TestCaseResult> FetchTestResultsByRun(
      Guid projectId,
      int testRunId,
      List<int> resultIds,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResultsByRun");
        this.PrepareStoredProcedure("TestResult.prc_FetchTestResultsByRun");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindBoolean("@includeActionResults", includeActionResults);
        this.BindInt32TypeTable("@idsTable", (IEnumerable<int>) resultIds);
        this.BindInt("@maxCommentSize", TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext));
        List<TestCaseResult> results = new List<TestCaseResult>(resultIds.Count);
        Dictionary<int, TestCaseResult> resultsMap = new Dictionary<int, TestCaseResult>(resultIds.Count);
        TestManagementDatabase50.FetchTestResultsByRunColumns resultsByRunColumns = new TestManagementDatabase50.FetchTestResultsByRunColumns();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          int PlanId = 0;
          string empty = string.Empty;
          TestManagementDatabase50.TestRunMetadataForResultsColumns forResultsColumns = new TestManagementDatabase50.TestRunMetadataForResultsColumns();
          if (reader.Read())
            (PlanId, empty) = forResultsColumns.bind(reader);
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResultsByRun");
          while (reader.Read())
          {
            TestCaseResult testCaseResult = resultsByRunColumns.bind(reader);
            testCaseResult.CustomFields = new List<TestExtensionField>();
            testCaseResult.TestPlanId = PlanId;
            testCaseResult.BuildNumber = empty;
            resultsMap.Add(testCaseResult.TestResultId, testCaseResult);
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResultsByRun");
          TestManagementDatabase.FetchTestResultsExColumns resultsExColumns = new TestManagementDatabase.FetchTestResultsExColumns();
          while (reader.Read())
          {
            Tuple<int, int, TestExtensionField> tuple = resultsExColumns.bind(reader);
            int key = tuple.Item2;
            if (resultsMap.ContainsKey(key))
            {
              if (string.Equals("StackTrace", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].StackTrace = tuple.Item3;
              else if (string.Equals("FailingSince", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string jsonString)
                {
                  FailingSince convertedObject = (FailingSince) null;
                  if (TestResultHelper.TryJsonConvertWithRetry<FailingSince>(jsonString, out convertedObject, true))
                    resultsMap[key].FailingSince = convertedObject;
                }
              }
              else if (string.Equals("Comment", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].Comment = tuple.Item3.Value as string;
              else if (string.Equals("ErrorMessage", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].ErrorMessage = tuple.Item3.Value as string;
              else if (string.Equals("UnsanitizedTestCaseTitle", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string str1)
                  resultsMap[key].TestCaseTitle = str1;
              }
              else if (string.Equals("UnsanitizedAutomatedTestName", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string str2)
                  resultsMap[key].AutomatedTestName = str2;
              }
              else if (string.Equals("MaxReservedSubResultId", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].SubResultCount = (int) tuple.Item3.Value;
              else if (string.Equals("TestResultGroupType", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].ResultGroupType = (ResultGroupType) TestManagementServiceUtility.ValidateAndGetEnumValue<ResultGroupType>((string) tuple.Item3.Value, ResultGroupType.None);
              else
                resultsMap[key].CustomFields.Add(tuple.Item3);
            }
          }
          if (includeActionResults)
          {
            actionResults = new List<TestActionResult>();
            parameters = new List<TestResultParameter>();
            attachments = new List<TestResultAttachment>();
            this.ReadTestActionResults(actionResults, parameters, attachments, reader, "TestResult.prc_FetchTestResultsByRun");
          }
          else
          {
            actionResults = (List<TestActionResult>) null;
            parameters = (List<TestResultParameter>) null;
            attachments = (List<TestResultAttachment>) null;
          }
        }
        resultIds.ForEach((Action<int>) (r =>
        {
          if (!resultsMap.ContainsKey(r))
            return;
          results.Add(resultsMap[r]);
        }));
        return results;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResults");
      }
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
        if (reader.NextResult())
        {
          while (reader.Read())
          {
            int int32 = sqlColumnBinder2.GetInt32((IDataReader) reader);
            testCaseReference[int32] = sqlColumnBinder1.GetInt32((IDataReader) reader);
          }
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

    internal override List<TestCaseReferenceRecord> GetTestResultsMetaData(
      Guid projectId,
      IList<int> testcaseReferenceIds,
      bool shouldIncludeFlakyDetails = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.GetTestResultsMetaData");
        List<TestCaseReferenceRecord> testResultsMetaData = new List<TestCaseReferenceRecord>();
        this.PrepareStoredProcedure("TestResult.prc_QueryTestCaseReferenceByIds");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindIdTypeTable("@testcaseReferenceIdTable", testcaseReferenceIds != null ? (IEnumerable<int>) testcaseReferenceIds.Distinct<int>().ToList<int>() : (IEnumerable<int>) null);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase31.FetchTestCaseRefRecords testCaseRefRecords = new TestManagementDatabase31.FetchTestCaseRefRecords(TestArtifactSource.Tfs);
        while (reader.Read())
          testResultsMetaData.Add(testCaseRefRecords.Bind(reader, projectId, out DateTime _));
        return testResultsMetaData;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.GetTestResultsMetaData");
      }
    }

    protected class TestRunMetadataForResultsColumns
    {
      private SqlColumnBinder TestPlanId = new SqlColumnBinder(nameof (TestPlanId));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));

      internal (int PlanId, string BuildNumber) bind(SqlDataReader reader)
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        return (this.TestPlanId.ColumnExists((IDataReader) reader) ? this.TestPlanId.GetInt32((IDataReader) reader) : 0, this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : string.Empty);
      }
    }

    protected class FetchTestResultsByRunColumns
    {
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder TestPointId = new SqlColumnBinder(nameof (TestPointId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder ResolutionStateId = new SqlColumnBinder(nameof (ResolutionStateId));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder Priority = new SqlColumnBinder(nameof (Priority));
      private SqlColumnBinder TestCaseTitle = new SqlColumnBinder(nameof (TestCaseTitle));
      private SqlColumnBinder TestCaseArea = new SqlColumnBinder(nameof (TestCaseArea));
      private SqlColumnBinder TestCaseRevision = new SqlColumnBinder(nameof (TestCaseRevision));
      private SqlColumnBinder ComputerName = new SqlColumnBinder(nameof (ComputerName));
      private SqlColumnBinder AfnStripId = new SqlColumnBinder(nameof (AfnStripId));
      private SqlColumnBinder ResetCount = new SqlColumnBinder(nameof (ResetCount));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder AutomatedTestStorage = new SqlColumnBinder(nameof (AutomatedTestStorage));
      private SqlColumnBinder AutomatedTestType = new SqlColumnBinder(nameof (AutomatedTestType));
      private SqlColumnBinder AutomatedTestId = new SqlColumnBinder(nameof (AutomatedTestId));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder RunBy = new SqlColumnBinder(nameof (RunBy));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));
      private SqlColumnBinder TestCaseOwner = new SqlColumnBinder(nameof (TestCaseOwner));

      internal TestCaseResult bind(SqlDataReader reader)
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.CreationDate = this.CreationDate.ColumnExists((IDataReader) reader) ? this.CreationDate.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.TestRunId = this.TestRunId.ColumnExists((IDataReader) reader) ? this.TestRunId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestResultId = this.TestResultId.ColumnExists((IDataReader) reader) ? this.TestResultId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestCaseReferenceId = this.TestCaseRefId.ColumnExists((IDataReader) reader) ? this.TestCaseRefId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestCaseId = this.TestCaseId.ColumnExists((IDataReader) reader) ? this.TestCaseId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ConfigurationId = this.ConfigurationId.ColumnExists((IDataReader) reader) ? this.ConfigurationId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestPointId = this.TestPointId.ColumnExists((IDataReader) reader) ? this.TestPointId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.State = this.State.ColumnExists((IDataReader) reader) ? this.State.GetByte((IDataReader) reader) : (byte) 0;
        testCaseResult.Outcome = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader) : (byte) 0;
        testCaseResult.ResolutionStateId = this.ResolutionStateId.ColumnExists((IDataReader) reader) ? this.ResolutionStateId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.LastUpdated = this.LastUpdated.ColumnExists((IDataReader) reader) ? this.LastUpdated.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.DateStarted = this.DateStarted.ColumnExists((IDataReader) reader) ? this.DateStarted.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.DateCompleted = this.DateCompleted.ColumnExists((IDataReader) reader) ? this.DateCompleted.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.Owner = this.Owner.ColumnExists((IDataReader) reader) ? this.Owner.GetGuid((IDataReader) reader, false) : Guid.Empty;
        testCaseResult.Priority = this.Priority.ColumnExists((IDataReader) reader) ? this.Priority.GetByte((IDataReader) reader) : (byte) 0;
        testCaseResult.TestCaseTitle = this.TestCaseTitle.ColumnExists((IDataReader) reader) ? this.TestCaseTitle.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestCaseAreaUri = this.TestCaseArea.ColumnExists((IDataReader) reader) ? this.TestCaseArea.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestCaseRevision = this.TestCaseRevision.ColumnExists((IDataReader) reader) ? this.TestCaseRevision.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ComputerName = this.ComputerName.ColumnExists((IDataReader) reader) ? this.ComputerName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AfnStripId = this.AfnStripId.ColumnExists((IDataReader) reader) ? this.AfnStripId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ResetCount = this.ResetCount.ColumnExists((IDataReader) reader) ? this.ResetCount.GetInt32((IDataReader) reader) : 0;
        testCaseResult.FailureType = this.FailureType.ColumnExists((IDataReader) reader) ? this.FailureType.GetByte((IDataReader) reader, (byte) 0) : (byte) 0;
        testCaseResult.AutomatedTestName = this.AutomatedTestName.ColumnExists((IDataReader) reader) ? this.AutomatedTestName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestStorage = this.AutomatedTestStorage.ColumnExists((IDataReader) reader) ? this.AutomatedTestStorage.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestType = this.AutomatedTestType.ColumnExists((IDataReader) reader) ? this.AutomatedTestType.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestId = this.AutomatedTestId.ColumnExists((IDataReader) reader) ? this.AutomatedTestId.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.Revision = this.Revision.ColumnExists((IDataReader) reader) ? this.Revision.GetInt32((IDataReader) reader) : 0;
        testCaseResult.RunBy = this.RunBy.ColumnExists((IDataReader) reader) ? this.RunBy.GetGuid((IDataReader) reader, true) : Guid.Empty;
        testCaseResult.LastUpdatedBy = this.LastUpdatedBy.ColumnExists((IDataReader) reader) ? this.LastUpdatedBy.GetGuid((IDataReader) reader, false) : Guid.Empty;
        testCaseResult.SuiteName = this.SuiteName.ColumnExists((IDataReader) reader) ? this.SuiteName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.OwnerName = this.TestCaseOwner.ColumnExists((IDataReader) reader) ? this.TestCaseOwner.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.Duration = TestManagementDatabase.GetDurationFromStartAndCompleteDates(testCaseResult.DateStarted, testCaseResult.DateCompleted);
        testCaseResult.Duration = testCaseResult.Duration == 0L ? TestManagementDatabase.GetDurationFromStartAndCompleteDates(testCaseResult.DateStarted, testCaseResult.DateCompleted) : testCaseResult.Duration;
        return testCaseResult;
      }
    }
  }
}
