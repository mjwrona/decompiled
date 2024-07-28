// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase42
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
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase42 : TestManagementDatabase41_1
  {
    internal override int GetAttachmentTfsFileId(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int sessionId,
      int attachmentId)
    {
      this.PrepareStoredProcedure("prc_GetAttachmentTfsFileId");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@sessionId", sessionId);
      this.BindInt("@attachmentId", attachmentId);
      using (SqlDataReader reader = this.ExecuteReader())
        return reader.Read() ? new TestManagementDatabase.GetAttachmentTfsFileIdColumns().TfsFileId.GetInt32((IDataReader) reader, 0) : throw new TestObjectNotFoundException(this.RequestContext, attachmentId, ObjectTypes.Attachment);
    }

    internal override Dictionary<TestCaseResultIdentifier, List<int>> CreateAttachmentsWithFileId(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments,
      bool changeCounterInterval = false)
    {
      if (attachments == null || !attachments.Any<TestResultAttachment>())
        return new Dictionary<TestCaseResultIdentifier, List<int>>();
      this.AssignTempIdToAttachments(attachments, changeCounterInterval);
      this.PrepareStoredProcedure("TestResult.prc_CreateAttachmentsWithFileId");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindTestResultAttachmentTypeTable4("@attachments", attachments);
      this.BindBoolean("@changeCounterInterval", changeCounterInterval);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException("prc_CreateAttachmentsWithFileId");
      List<int> intList1 = new List<int>();
      TestManagementDatabase.CreateAttachmentColumns attachmentColumns = new TestManagementDatabase.CreateAttachmentColumns();
      do
      {
        intList1.Add(attachmentColumns.AttachmentId.GetInt32((IDataReader) reader));
      }
      while (reader.Read());
      Dictionary<TestCaseResultIdentifier, List<int>> attachmentsWithFileId = new Dictionary<TestCaseResultIdentifier, List<int>>();
      int num = 0;
      foreach (TestResultAttachment attachment in attachments)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(attachment.TestRunId, attachment.TestResultId);
        List<int> intList2;
        if (!attachmentsWithFileId.TryGetValue(key, out intList2))
        {
          intList2 = new List<int>();
          attachmentsWithFileId.Add(key, intList2);
        }
        intList2.Add(intList1[num++]);
      }
      return attachmentsWithFileId;
    }

    internal TestManagementDatabase42(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase42()
    {
    }

    internal override TestResultsGroupsDataWithWaterMark GetTestResultAutomatedTestStorageAndOwnersByBuild(
      Guid projectId,
      int buildId,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsGroupsByBuildWithWaterMark");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", publishContext, 128, true, SqlDbType.NVarChar);
      this.BindInt("@buildId", buildId);
      this.BindInt("@continuationTokenRunId", continuationTokenRunId);
      this.BindInt("@continuationTokenResultId", continuationTokenResultId);
      this.BindInt("@top", top);
      SqlDataReader reader = this.ExecuteReader();
      TestCaseResultIdentifier resultIdentifier = TestManagementDatabase42.GetLastResultIdentifier(reader);
      TestResultsGroupsData resultsGroupsData = new TestResultsGroupsData();
      if (reader.NextResult())
        TestManagementDatabase42.GetTestResultsGroupResponse(reader, resultsGroupsData);
      return new TestResultsGroupsDataWithWaterMark(resultsGroupsData, resultIdentifier);
    }

    internal override TestResultsGroupsDataWithWaterMark GetTestResultAutomatedTestStorageAndOwnersByRelease(
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top,
      int runIdThreshold)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsGroupsByReleaseWithWatermark");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", publishContext, 128, true, SqlDbType.NVarChar);
      this.BindNullableInt("@releaseId", releaseId, 0);
      this.BindNullableInt("@releaseEnvId", releaseEnvId, 0);
      this.BindInt("@continuationTokenRunId", continuationTokenRunId);
      this.BindInt("@continuationTokenResultId", continuationTokenResultId);
      this.BindInt("@top", top);
      SqlDataReader reader = this.ExecuteReader();
      TestCaseResultIdentifier resultIdentifier = TestManagementDatabase42.GetLastResultIdentifier(reader);
      TestResultsGroupsData resultsGroupsData = new TestResultsGroupsData();
      if (reader.NextResult())
        TestManagementDatabase42.GetTestResultsGroupResponse(reader, resultsGroupsData);
      return new TestResultsGroupsDataWithWaterMark(resultsGroupsData, resultIdentifier);
    }

    private static TestCaseResultIdentifier GetLastResultIdentifier(SqlDataReader reader)
    {
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("LastRunId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("LastResultId");
      return reader.Read() ? new TestCaseResultIdentifier(sqlColumnBinder1.GetInt32((IDataReader) reader), sqlColumnBinder2.GetInt32((IDataReader) reader)) : (TestCaseResultIdentifier) null;
    }

    private static void GetTestResultsGroupResponse(
      SqlDataReader reader,
      TestResultsGroupsData testResultsGroupsData)
    {
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("AutomatedTestStorage");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("Owner");
      while (reader.Read())
      {
        testResultsGroupsData.AutomatedTestStorage.Add(sqlColumnBinder1.GetString((IDataReader) reader, true));
        testResultsGroupsData.Owner.Add(sqlColumnBinder2.GetString((IDataReader) reader, true));
      }
    }

    internal override List<TestCaseResultIdentifier> QueryTestResultsForTestCaseId(
      Guid projectId,
      int testCaseId,
      int top)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestResultsForTestCaseId");
        this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsByTestCase");
        this.BindInt("@testCaseId", testCaseId);
        this.BindInt("@top", top);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        SqlDataReader reader = this.ExecuteReader();
        List<TestCaseResultIdentifier> resultIdentifierList = new List<TestCaseResultIdentifier>();
        TestManagementDatabase.QueryTestResultsColumns testResultsColumns = new TestManagementDatabase.QueryTestResultsColumns();
        while (reader.Read())
          resultIdentifierList.Add(testResultsColumns.bind(reader));
        return resultIdentifierList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestResultsForTestCaseId");
      }
    }

    internal override List<TestConfigurationRecord> QueryTestConfigurationsByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      this.PrepareStoredProcedure("TestManagement.prc_QueryTestConfigurationsByChangedDate");
      this.BindInt("@dataspaceId", dataspaceId);
      if (DateTime.Compare(fromDate, SqlDateTime.MinValue.Value) < 0)
        fromDate = SqlDateTime.MinValue.Value;
      this.BindInt("@batchSize", batchSize);
      this.BindDateTime("@fromChangedDate", fromDate, true);
      SqlDataReader reader = this.ExecuteReader();
      List<TestConfigurationRecord> configurationRecordList = new List<TestConfigurationRecord>();
      TestManagementDatabase42.FetchTestConfigurationRecord configurationRecord = new TestManagementDatabase42.FetchTestConfigurationRecord();
      Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
      toDate = fromDate;
      while (reader.Read())
        configurationRecordList.Add(configurationRecord.bind(reader, dataspaceIdentifier, dataSource, out toDate));
      return configurationRecordList;
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

    public override UpdatedProperties CreateIfNotExistsTestSettings(
      TestManagementRequestContext context,
      Guid projectId,
      TestSettings settings,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("prc_CreateIfNotExistsTestSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@name", settings.Name, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", settings.Description, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindXml("@settings", settings.Settings);
      this.BindXml("@machineRoles", TestSettingsMachineRole.ToXml(settings.MachineRoles));
      this.BindInt("@areaId", settings.AreaId);
      this.BindBoolean("@isPublic", settings.IsPublic);
      this.BindBoolean("@isAutomated", settings.IsAutomated);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedProperties existsTestSettings = reader.Read() ? new TestManagementDatabase.CreateTestSettingsColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_CreateIfNotExistsTestSettings");
      existsTestSettings.LastUpdatedBy = updatedBy;
      return existsTestSettings;
    }

    protected class FetchTestConfigurationRecord
    {
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));

      internal virtual TestConfigurationRecord bind(
        SqlDataReader reader,
        Guid projectId,
        TestArtifactSource datasource,
        out DateTime lastUpdatedDate)
      {
        lastUpdatedDate = this.LastUpdated.GetDateTime((IDataReader) reader);
        return new TestConfigurationRecord()
        {
          ProjectGuid = projectId,
          TestConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader),
          Name = this.Name.GetString((IDataReader) reader, false),
          State = ((Microsoft.TeamFoundation.TestManagement.Client.TestConfigurationState) this.State.GetByte((IDataReader) reader)).ToString(),
          DataSourceId = datasource
        };
      }
    }

    protected class QueryTestHistoryColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder ReleaseEnvDefId = new SqlColumnBinder(nameof (ReleaseEnvDefId));
      private SqlColumnBinder ContinuationTokenMinRunId = new SqlColumnBinder(nameof (ContinuationTokenMinRunId));
      private SqlColumnBinder ContinuationTokenMaxRunId = new SqlColumnBinder(nameof (ContinuationTokenMaxRunId));

      internal void bindResultMap(
        SqlDataReader reader,
        Dictionary<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>> resultsMap,
        TestResultHistoryGroupBy groupBy,
        int buildDefinitionId,
        Guid projectId)
      {
        int int32_1 = this.TestRunId.ColumnExists((IDataReader) reader) ? this.TestRunId.GetInt32((IDataReader) reader) : 0;
        int int32_2 = this.TestResultId.ColumnExists((IDataReader) reader) ? this.TestResultId.GetInt32((IDataReader) reader) : 0;
        byte num1 = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader) : (byte) 0;
        DateTime dateTime1 = this.DateStarted.ColumnExists((IDataReader) reader) ? this.DateStarted.GetDateTime((IDataReader) reader) : new DateTime();
        DateTime dateTime2 = this.DateCompleted.ColumnExists((IDataReader) reader) ? this.DateCompleted.GetDateTime((IDataReader) reader) : new DateTime();
        string str1 = this.BranchName.ColumnExists((IDataReader) reader) ? this.BranchName.GetString((IDataReader) reader, true) : string.Empty;
        int int32_3 = this.BuildId.ColumnExists((IDataReader) reader) ? this.BuildId.GetInt32((IDataReader) reader, 0) : 0;
        string str2 = this.BuildUri.ColumnExists((IDataReader) reader) ? this.BuildUri.GetString((IDataReader) reader, true) : string.Empty;
        string str3 = this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : string.Empty;
        int int32_4 = this.ReleaseId.ColumnExists((IDataReader) reader) ? this.ReleaseId.GetInt32((IDataReader) reader, 0) : 0;
        string str4 = this.ReleaseName.ColumnExists((IDataReader) reader) ? this.ReleaseName.GetString((IDataReader) reader, true) : string.Empty;
        int int32_5 = this.ReleaseEnvId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvId.GetInt32((IDataReader) reader, 0) : 0;
        int int32_6 = this.ReleaseEnvDefId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvDefId.GetInt32((IDataReader) reader, 0) : 0;
        double num2 = 0.0;
        if (!dateTime1.Equals(new DateTime()) && !dateTime2.Equals(new DateTime()))
          num2 = (dateTime2 - dateTime1).TotalMilliseconds;
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
        {
          TestRun = new ShallowReference()
          {
            Id = int32_1.ToString()
          },
          Id = int32_2,
          CompletedDate = dateTime2,
          DurationInMs = num2,
          Outcome = ((Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) num1).ToString(),
          Project = new ShallowReference()
          {
            Id = projectId.ToString()
          }
        };
        testCaseResult.BuildReference = new BuildReference();
        testCaseResult.BuildReference.Id = int32_3;
        testCaseResult.BuildReference.Uri = str2;
        testCaseResult.BuildReference.Number = str3;
        testCaseResult.ReleaseReference = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference();
        testCaseResult.ReleaseReference.Id = int32_4;
        testCaseResult.ReleaseReference.EnvironmentId = int32_5;
        testCaseResult.ReleaseReference.Name = str4;
        string key = string.Empty;
        switch (groupBy)
        {
          case TestResultHistoryGroupBy.Branch:
            key = str1;
            break;
          case TestResultHistoryGroupBy.Environment:
            key = int32_6.ToString();
            break;
          case TestResultHistoryGroupBy.BuildDefinitionId:
            key = buildDefinitionId.ToString();
            break;
        }
        if (!resultsMap.ContainsKey(key))
          resultsMap[key] = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
        resultsMap[key].Add(testCaseResult);
      }

      internal void bindContinuationToken(
        SqlDataReader reader,
        out int continuationTokenMinRunId,
        out int continuationTokenMaxRunId)
      {
        continuationTokenMinRunId = this.ContinuationTokenMinRunId.ColumnExists((IDataReader) reader) ? this.ContinuationTokenMinRunId.GetInt32((IDataReader) reader, 0) : 0;
        continuationTokenMaxRunId = this.ContinuationTokenMaxRunId.ColumnExists((IDataReader) reader) ? this.ContinuationTokenMaxRunId.GetInt32((IDataReader) reader, 0) : 0;
      }
    }
  }
}
