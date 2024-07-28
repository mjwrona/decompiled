// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase4
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
  public class TestManagementDatabase4 : TestManagementDatabase3
  {
    internal TestManagementDatabase4(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase4()
    {
    }

    public override List<TestCaseResult> QueryTestResultHistory(
      Guid projectId,
      string automatedTestName,
      int testCaseId,
      DateTime maxCompleteDate,
      int historyDays)
    {
      this.PrepareStoredProcedure("prc_QueryTestResultHistory");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@runId", 0);
      this.BindInt("@resultId", 0);
      this.BindInt("@historyDays", historyDays);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
      TestManagementDatabase4.QueryTestResultHistoryColumns resultHistoryColumns = new TestManagementDatabase4.QueryTestResultHistoryColumns();
      while (reader.Read())
        testCaseResultList.Add(resultHistoryColumns.bind(reader));
      return testCaseResultList;
    }

    public override List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> QueryTestResultTrendReport(
      Guid projectId,
      ResultsFilter filter)
    {
      this.PrepareStoredProcedure("prc_QueryTestResultTrend");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@runId", 0);
      this.BindInt("@resultId", 0);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindInt("@top", filter.ResultsCount);
      SqlDataReader reader = this.ExecuteReader();
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      TestManagementDatabase4.QueryTestResultTrendColumns resultTrendColumns = new TestManagementDatabase4.QueryTestResultTrendColumns();
      while (reader.Read())
        testCaseResultList.Add(resultTrendColumns.bind(reader));
      return testCaseResultList;
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

    protected class QueryTestResultHistoryColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));

      internal TestCaseResult bind(SqlDataReader reader)
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.TestRunId = this.TestRunId.ColumnExists((IDataReader) reader) ? this.TestRunId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestResultId = this.TestResultId.ColumnExists((IDataReader) reader) ? this.TestResultId.GetInt32((IDataReader) reader) : 0;
        return testCaseResult;
      }
    }

    protected class QueryTestResultTrendColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder TestCaseTitle = new SqlColumnBinder(nameof (TestCaseTitle));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));

      internal Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult bind(
        SqlDataReader reader)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult();
        testCaseResult.BuildReference = new BuildReference();
        testCaseResult.ReleaseReference = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference();
        int int32 = this.TestRunId.ColumnExists((IDataReader) reader) ? this.TestRunId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestRun = new ShallowReference()
        {
          Id = int32.ToString()
        };
        testCaseResult.Id = this.TestResultId.ColumnExists((IDataReader) reader) ? this.TestResultId.GetInt32((IDataReader) reader) : 0;
        byte num = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader) : (byte) 0;
        testCaseResult.Outcome = ((TestOutcome) num).ToString();
        testCaseResult.DurationInMs = this.Duration.ColumnExists((IDataReader) reader) ? (double) this.Duration.GetInt64((IDataReader) reader) : 0.0;
        DateTime dateTime = this.DateStarted.ColumnExists((IDataReader) reader) ? this.DateStarted.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.CompletedDate = this.DateCompleted.ColumnExists((IDataReader) reader) ? this.DateCompleted.GetDateTime((IDataReader) reader) : new DateTime();
        if (!dateTime.Equals(new DateTime()) && !testCaseResult.CompletedDate.Equals(new DateTime()))
          testCaseResult.DurationInMs = (testCaseResult.CompletedDate - dateTime).TotalMilliseconds;
        testCaseResult.TestCaseTitle = this.TestCaseTitle.ColumnExists((IDataReader) reader) ? this.TestCaseTitle.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestCaseReferenceId = this.TestCaseRefId.ColumnExists((IDataReader) reader) ? this.TestCaseRefId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.BuildReference.Id = this.BuildId.ColumnExists((IDataReader) reader) ? this.BuildId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.BuildReference.Uri = this.BuildUri.ColumnExists((IDataReader) reader) ? this.BuildUri.GetString((IDataReader) reader, true) : string.Empty;
        testCaseResult.BuildReference.Number = this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : string.Empty;
        testCaseResult.ReleaseReference.Id = this.ReleaseId.ColumnExists((IDataReader) reader) ? this.ReleaseId.GetInt32((IDataReader) reader, 0) : 0;
        testCaseResult.ReleaseReference.EnvironmentId = this.ReleaseEnvId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvId.GetInt32((IDataReader) reader, 0) : 0;
        testCaseResult.ReleaseReference.Name = this.ReleaseName.ColumnExists((IDataReader) reader) ? this.ReleaseName.GetString((IDataReader) reader, true) : string.Empty;
        return testCaseResult;
      }
    }
  }
}
