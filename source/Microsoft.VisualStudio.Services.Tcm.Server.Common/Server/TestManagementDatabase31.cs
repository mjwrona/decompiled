// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase31
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase31 : TestManagementDatabase30
  {
    internal TestManagementDatabase31(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase31()
    {
    }

    internal override List<TestRunRecord> QueryTestRunsByChangedDate(
      int dataspaceId,
      int batchSize,
      string prBranchName,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestRunsByChangedDate");
      this.BindInt("@dataspaceId", dataspaceId);
      if (DateTime.Compare(fromDate, SqlDateTime.MinValue.Value) < 0)
        fromDate = SqlDateTime.MinValue.Value;
      this.BindInt("@batchSize", batchSize);
      this.BindDateTime("@fromChangedDate", fromDate, true);
      this.BindString("@prBranchName", prBranchName, 50, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      List<TestRunRecord> testRunRecordList = new List<TestRunRecord>();
      TestManagementDatabase29.QueryTestRunColumnsByFilters columnsByFilters = new TestManagementDatabase29.QueryTestRunColumnsByFilters();
      Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
      toDate = fromDate;
      while (reader.Read())
      {
        TestRun testRun = columnsByFilters.bind(reader);
        testRunRecordList.Add(new TestRunRecord()
        {
          ProjectGuid = dataspaceIdentifier,
          TestRunId = testRun.TestRunId,
          TestRunTitle = testRun.Title,
          DateStarted = !DateTime.Equals(testRun.StartDate, new DateTime()) ? new DateTime?(testRun.StartDate) : new DateTime?(),
          DateCompleted = !DateTime.Equals(testRun.CompleteDate, new DateTime()) ? new DateTime?(testRun.CompleteDate) : new DateTime?(),
          IsAutomated = testRun.IsAutomated,
          TotalTests = testRun.TotalTests,
          ChangedDate = testRun.LastUpdated,
          SourceWorkflow = testRun.SourceWorkflow,
          BuildDefinitionId = testRun.BuildReference.BuildDefinitionId,
          BuildId = testRun.BuildReference.BuildId,
          BranchName = testRun.BuildReference.BranchName,
          RepositoryId = testRun.BuildReference.RepositoryId,
          ReleaseDefinitionId = testRun.ReleaseReference.ReleaseDefId,
          ReleaseEnvironmentDefinitionId = testRun.ReleaseReference.ReleaseEnvDefId,
          ReleaseId = testRun.ReleaseReference.ReleaseId,
          ReleaseEnvironmentId = testRun.ReleaseReference.ReleaseEnvId,
          Attempt = testRun.ReleaseReference.Attempt,
          DataSourceId = dataSource
        });
        toDate = testRun.LastUpdated;
      }
      return testRunRecordList;
    }

    internal override List<TestResultRecord> QueryTestResultsByTestRunChangedDate(
      int dataspaceId,
      int runBatchSize,
      int resultBatchSize,
      string prBranchName,
      TestResultWatermark fromWatermark,
      out TestResultWatermark toWatermark,
      TestArtifactSource dataSource,
      bool includeFlakyData = false)
    {
      return this.QueryTestResultsByTestRunChangedDateInternal(dataspaceId, runBatchSize, resultBatchSize, prBranchName, fromWatermark, out toWatermark, dataSource, false);
    }

    protected List<TestResultRecord> QueryTestResultsByTestRunChangedDateInternal(
      int dataspaceId,
      int runBatchSize,
      int resultBatchSize,
      string prBranchName,
      TestResultWatermark fromWatermark,
      out TestResultWatermark toWatermark,
      TestArtifactSource dataSource,
      bool includeFlakyData)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsByTestRunChangedDate");
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindInt("@runBatchSize", runBatchSize);
      this.BindInt("@resultBatchSize", resultBatchSize);
      this.BindDateTime("@fromRunChangedDate", fromWatermark == null || DateTime.Compare(fromWatermark.ChangedDate, SqlDateTime.MinValue.Value) < 0 ? SqlDateTime.MinValue.Value : fromWatermark.ChangedDate, true);
      this.BindInt("@fromRunId", fromWatermark != null ? fromWatermark.TestRunId : 0);
      this.BindInt("@fromResultId", fromWatermark != null ? fromWatermark.TestResultId : 0);
      this.BindString("@prBranchName", prBranchName, 50, true, SqlDbType.NVarChar);
      if (includeFlakyData)
        this.BindBoolean("@includeFlakyData", includeFlakyData);
      SqlDataReader reader = this.ExecuteReader();
      DateTime runChangedDate = new DateTime();
      bool flag = false;
      List<TestResultRecord> collection = new List<TestResultRecord>();
      List<TestResultRecord> source = new List<TestResultRecord>();
      TestManagementDatabase31.FetchTestResultRecord testResultRecord1 = new TestManagementDatabase31.FetchTestResultRecord(dataSource);
      Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
      toWatermark = new TestResultWatermark()
      {
        ChangedDate = fromWatermark != null ? fromWatermark.ChangedDate : SqlDateTime.MinValue.Value,
        TestRunId = fromWatermark != null ? fromWatermark.TestRunId : 0,
        TestResultId = fromWatermark != null ? fromWatermark.TestResultId : 0
      };
      while (reader.Read())
      {
        TestResultRecord testResultRecord2 = testResultRecord1.Bind(reader, dataspaceIdentifier, out runChangedDate);
        collection.Add(testResultRecord2);
      }
      if (reader.NextResult())
      {
        while (reader.Read())
        {
          TestResultRecord testResultRecord3 = testResultRecord1.Bind(reader, dataspaceIdentifier, out runChangedDate);
          flag = flag || testResultRecord3.TestRunId == (fromWatermark != null ? fromWatermark.TestRunId : 0);
          source.Add(testResultRecord3);
        }
      }
      if (!flag)
        source.InsertRange(0, (IEnumerable<TestResultRecord>) collection);
      if (source.Any<TestResultRecord>())
      {
        toWatermark.ChangedDate = runChangedDate;
        toWatermark.TestRunId = source.Last<TestResultRecord>().TestRunId;
        toWatermark.TestResultId = source.Last<TestResultRecord>().TestResultId;
      }
      return source;
    }

    public override List<TestCaseReferenceRecord> QueryTestCaseReferenceByChangedDate(
      int dataspaceId,
      int testCaseRefBatchSize,
      TestCaseReferenceWatermark fromWatermark,
      out TestCaseReferenceWatermark toWatermark,
      TestArtifactSource dataSource)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestCaseReferenceByChangedDate");
        this.PrepareStoredProcedure("TestResult.prc_QueryTestCaseReferencesByChangedDate");
        this.BindInt("@dataspaceId", dataspaceId);
        this.BindInt("@testCaseRefBatchSize", testCaseRefBatchSize);
        SqlDateTime minValue;
        DateTime changedDate1;
        if (fromWatermark != null && DateTime.Compare(fromWatermark.ChangedDate, SqlDateTime.MinValue.Value) >= 0)
        {
          changedDate1 = fromWatermark.ChangedDate;
        }
        else
        {
          minValue = SqlDateTime.MinValue;
          changedDate1 = minValue.Value;
        }
        this.BindDateTime("@fromLastUpdatedDate", changedDate1, true);
        this.BindInt("@fromTestCaseRefId", fromWatermark != null ? fromWatermark.TestCaseReferenceId : 0);
        SqlDataReader reader = this.ExecuteReader();
        DateTime changedDate2;
        if (fromWatermark == null)
        {
          minValue = SqlDateTime.MinValue;
          changedDate2 = minValue.Value;
        }
        else
          changedDate2 = fromWatermark.ChangedDate;
        DateTime lastRefChangedDate = changedDate2;
        List<TestCaseReferenceRecord> source = new List<TestCaseReferenceRecord>();
        TestManagementDatabase31.FetchTestCaseRefRecords testCaseRefRecords = new TestManagementDatabase31.FetchTestCaseRefRecords(dataSource);
        Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
        ref TestCaseReferenceWatermark local = ref toWatermark;
        TestCaseReferenceWatermark referenceWatermark = new TestCaseReferenceWatermark();
        DateTime changedDate3;
        if (fromWatermark == null)
        {
          minValue = SqlDateTime.MinValue;
          changedDate3 = minValue.Value;
        }
        else
          changedDate3 = fromWatermark.ChangedDate;
        referenceWatermark.ChangedDate = changedDate3;
        local = referenceWatermark;
        while (reader.Read())
          source.Add(testCaseRefRecords.Bind(reader, dataspaceIdentifier, out lastRefChangedDate));
        if (reader.NextResult())
        {
          while (reader.Read())
            source.Add(testCaseRefRecords.Bind(reader, dataspaceIdentifier, out lastRefChangedDate));
        }
        if (source.Any<TestCaseReferenceRecord>())
        {
          toWatermark.ChangedDate = lastRefChangedDate;
          toWatermark.TestCaseReferenceId = source.Last<TestCaseReferenceRecord>().TestCaseReferenceId;
        }
        return source;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestCaseReferenceByChangedDate");
      }
    }

    internal override void CleanDeletedTestRunDimensions(
      Guid projectId,
      int maxDimensionRowsToDelete,
      int deletionBatchSize,
      int waitDurationForCleanup,
      out int? deletedTestCaseRefs,
      int cleanDeletedTestRunDimensionsSprocExecTimeOutInSec)
    {
      deletedTestCaseRefs = new int?();
      this.PrepareStoredProcedure("TestResult.prc_DeleteTestRunDimensions", cleanDeletedTestRunDimensionsSprocExecTimeOutInSec);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@batchSize", deletionBatchSize);
      this.BindInt("@maxDetailsToDelete", maxDimensionRowsToDelete);
      this.BindInt("@waitDaysForCleanup", waitDurationForCleanup);
      this.ExecuteNonQuery();
    }

    public override void UpdateTestCaseReference2(
      Guid projectId,
      IEnumerable<TestCaseResult> results)
    {
    }

    internal override List<Tuple<TestConfiguration, string>> QueryTestConfigurationById(
      List<int> configurationIds,
      Guid projectId,
      bool returnVariables)
    {
      Dictionary<int, Tuple<TestConfiguration, string>> source = new Dictionary<int, Tuple<TestConfiguration, string>>();
      this.PrepareStoredProcedure("prc_QueryConfigurationByIds");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt32TypeTable("@configurationId", (IEnumerable<int>) configurationIds);
      this.BindBoolean("@returnVariables", returnVariables);
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
      {
        string areaUri;
        TestConfiguration testConfiguration = new TestManagementDatabase.QueryTestConfigurationsColumns().bind(reader, out areaUri);
        source[testConfiguration.Id] = new Tuple<TestConfiguration, string>(testConfiguration, areaUri);
      }
      if (returnVariables)
      {
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestConfigurationByIds");
        if (source.Any<KeyValuePair<int, Tuple<TestConfiguration, string>>>())
        {
          TestManagementDatabase.QueryTestConfigurationsColumns2 configurationsColumns2 = new TestManagementDatabase.QueryTestConfigurationsColumns2();
          while (reader.Read())
          {
            int id;
            NameValuePair nameValuePair = configurationsColumns2.bind(reader, out id);
            source[id].Item1.Values.Add(nameValuePair);
          }
        }
      }
      return source.Values.ToList<Tuple<TestConfiguration, string>>();
    }

    protected class FetchTestResultRecord
    {
      private SqlColumnBinder RunChangedDate = new SqlColumnBinder(nameof (RunChangedDate));
      private SqlColumnBinder IsFlaky = new SqlColumnBinder(nameof (IsFlaky));
      private TestManagementDatabase.FetchTestResultsColumns m_fetchTestResultsColumns;
      private TestArtifactSource DataSource;

      internal FetchTestResultRecord(TestArtifactSource resultsSource)
      {
        this.m_fetchTestResultsColumns = new TestManagementDatabase.FetchTestResultsColumns();
        this.DataSource = resultsSource;
      }

      internal TestResultRecord Bind(
        SqlDataReader reader,
        Guid projectId,
        out DateTime runChangedDate)
      {
        TestCaseResult testCaseResult = this.m_fetchTestResultsColumns.bind(reader);
        runChangedDate = this.RunChangedDate.GetDateTime((IDataReader) reader);
        return new TestResultRecord()
        {
          ProjectGuid = projectId,
          TestRunId = testCaseResult.TestRunId,
          TestResultId = testCaseResult.TestResultId,
          TestCaseReferenceId = testCaseResult.TestCaseReferenceId,
          DateStarted = !DateTime.Equals(testCaseResult.DateStarted, new DateTime()) ? new DateTime?(testCaseResult.DateStarted) : new DateTime?(),
          DateCompleted = !DateTime.Equals(testCaseResult.DateCompleted, new DateTime()) ? new DateTime?(testCaseResult.DateCompleted) : new DateTime?(),
          Outcome = testCaseResult.Outcome,
          DataSourceId = this.DataSource,
          IsFlaky = this.IsFlaky.ColumnExists((IDataReader) reader) ? this.IsFlaky.GetNullableBoolean((IDataReader) reader) : new bool?()
        };
      }
    }

    protected class FetchTestCaseRefRecords
    {
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder AutomatedTestStorage = new SqlColumnBinder(nameof (AutomatedTestStorage));
      private SqlColumnBinder Priority = new SqlColumnBinder(nameof (Priority));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder PointId = new SqlColumnBinder("TestPointId");
      private SqlColumnBinder LastRefTestRunDate = new SqlColumnBinder(nameof (LastRefTestRunDate));
      private SqlColumnBinder TestCaseTitle = new SqlColumnBinder(nameof (TestCaseTitle));
      private TestArtifactSource Source;

      public FetchTestCaseRefRecords(TestArtifactSource dataSource) => this.Source = dataSource;

      internal TestCaseReferenceRecord Bind(
        SqlDataReader reader,
        Guid projectId,
        out DateTime lastRefChangedDate)
      {
        lastRefChangedDate = this.LastRefTestRunDate.GetDateTime((IDataReader) reader);
        return new TestCaseReferenceRecord()
        {
          ProjectGuid = projectId,
          TestCaseReferenceId = this.TestCaseRefId.GetInt32((IDataReader) reader),
          AutomatedTestName = this.AutomatedTestName.GetString((IDataReader) reader, true),
          AutomatedTestStorage = this.AutomatedTestStorage.GetString((IDataReader) reader, true),
          Priority = (int) this.Priority.GetByte((IDataReader) reader, (byte) 0),
          TestPointId = this.PointId.GetInt32((IDataReader) reader, 0),
          Owner = this.Owner.GetString((IDataReader) reader, true),
          TestCaseTitle = this.TestCaseTitle.ColumnExists((IDataReader) reader) ? this.TestCaseTitle.GetString((IDataReader) reader, true) : (string) null,
          DataSourceId = this.Source
        };
      }
    }
  }
}
