// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase24
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
  public class TestManagementDatabase24 : TestManagementDatabase23
  {
    public override List<TestResultAttachment> QueryAttachments3(
      Guid projectId,
      Dictionary<string, List<object>> parametersMap)
    {
      int[] ids1 = (int[]) null;
      int[] ids2 = (int[]) null;
      int[] ids3 = (int[]) null;
      Guid result = Guid.Empty;
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      if (parametersMap.ContainsKey("Id"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids3 = Array.ConvertAll<object, int>(parametersMap["Id"].ToArray(), TestManagementDatabase24.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestManagementDatabase24.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("TestRunId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids1 = Array.ConvertAll<object, int>(parametersMap["TestRunId"].ToArray(), TestManagementDatabase24.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestManagementDatabase24.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("TestResultId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids2 = Array.ConvertAll<object, int>(parametersMap["TestResultId"].ToArray(), TestManagementDatabase24.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestManagementDatabase24.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      this.PrepareStoredProcedure("TestResult.prc_QueryAttachments3");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindIdTypeTable("@testRunIds", (IEnumerable<int>) ids1);
      this.BindIdTypeTable("@testResultIds", (IEnumerable<int>) ids2);
      this.BindIdTypeTable("@attachmentIds", (IEnumerable<int>) ids3);
      if (parametersMap.ContainsKey("IsComplete"))
        this.BindBoolean("@isComplete", Convert.ToBoolean(parametersMap["IsComplete"].First<object>()));
      if (parametersMap.ContainsKey("TmiRunId") && Guid.TryParse(parametersMap["TmiRunId"].First<object>().ToString(), out result) && result != Guid.Empty)
        this.BindGuid("@tmiRunId", result);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryAttachmentsColumns2 attachmentsColumns2 = new TestManagementDatabase.QueryAttachmentsColumns2();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumns2.Bind(reader));
      return resultAttachmentList;
    }

    internal TestManagementDatabase24(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase24()
    {
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
        this.BindNullableInt("@planId", planId, -1);
        this.BindGuidPreserveNull("@owner", owner);
        this.BindString("@buildUri", buildUri, 256, true, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@skip", skip);
        this.BindInt("@top", top);
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
        return dictionary1.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns");
      }
    }

    public override Dictionary<int, List<TestCaseReference>> GetAutomatedTestCaseReferencesWithoutHash(
      int top)
    {
      this.PrepareStoredProcedure("TestResult.prc_GetAutomatedTestNamesWithoutHash");
      this.BindInt("@top", top);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<int, List<TestCaseReference>> projectToTestRefMap = new Dictionary<int, List<TestCaseReference>>();
      TestManagementDatabase24.AutomatedTestNamesColumns testNamesColumns = new TestManagementDatabase24.AutomatedTestNamesColumns();
      while (reader.Read())
        testNamesColumns.bind(reader, projectToTestRefMap);
      return projectToTestRefMap;
    }

    public override void PopulateAutomatedTestNameHash(
      int dataspaceId,
      List<TestCaseReference> testCaseReferences)
    {
      this.PrepareStoredProcedure("TestResult.prc_PopulateAutomatedTestNameHash");
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindTestResult_TestCaseReference2TypeTableForPopulateHash("@testCaseReferenceTable", (IEnumerable<TestCaseReference>) testCaseReferences);
      this.ExecuteNonQuery();
    }

    public override List<TestSettings> QueryTestSettings2(
      Guid projectId,
      Dictionary<string, List<object>> parametersMap,
      bool omitSettings,
      out List<KeyValuePair<string, TestSettings>> areaUris)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestSettings2");
        string[] names = (string[]) null;
        int[] ids = (int[]) null;
        if (parametersMap.ContainsKey("Name"))
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          names = Array.ConvertAll<object, string>(parametersMap["Name"].ToArray(), TestManagementDatabase24.\u003C\u003EO.\u003C1\u003E__ToString ?? (TestManagementDatabase24.\u003C\u003EO.\u003C1\u003E__ToString = new Converter<object, string>(Convert.ToString)));
        }
        if (parametersMap.ContainsKey("Id"))
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          ids = Array.ConvertAll<object, int>(parametersMap["Id"].ToArray(), TestManagementDatabase24.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestManagementDatabase24.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
        }
        this.PrepareStoredProcedure("TestManagement.prc_QueryTestSettings2");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindNameTypeTable("@settingNames", (IEnumerable<string>) names);
        this.BindIdTypeTable("@settingIds", (IEnumerable<int>) ids);
        this.BindBoolean("@omitSettings", omitSettings);
        areaUris = new List<KeyValuePair<string, TestSettings>>();
        List<TestSettings> testSettingsList = new List<TestSettings>();
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase.QueryTestSettingsColumns testSettingsColumns = new TestManagementDatabase.QueryTestSettingsColumns();
        while (reader.Read())
        {
          string areaUri;
          TestSettings testSettings = testSettingsColumns.Bind(this.RequestContext, reader, out areaUri);
          if (!string.IsNullOrEmpty(areaUri))
            areaUris.Add(new KeyValuePair<string, TestSettings>(areaUri, testSettings));
          testSettingsList.Add(testSettings);
        }
        return testSettingsList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestSettings2");
      }
    }

    protected new class UpdatedPropertyColumns
    {
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      internal SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      internal SqlColumnBinder IsRunStarted = new SqlColumnBinder(nameof (IsRunStarted));
      internal SqlColumnBinder IsRunCompleted = new SqlColumnBinder(nameof (IsRunCompleted));
      internal SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      internal SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));

      internal UpdatedRunProperties BindUpdatedRunProperties(SqlDataReader reader)
      {
        UpdatedRunProperties updatedRunProperties = new UpdatedRunProperties();
        updatedRunProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        updatedRunProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        updatedRunProperties.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        updatedRunProperties.IsRunStarted = this.IsRunStarted.GetBoolean((IDataReader) reader);
        updatedRunProperties.IsRunCompleted = this.IsRunCompleted.GetBoolean((IDataReader) reader);
        updatedRunProperties.CompleteDate = this.CompleteDate.GetDateTime((IDataReader) reader, new DateTime());
        return updatedRunProperties;
      }

      internal BlockedPointProperties BindBlockedTestPointProperties(SqlDataReader reader)
      {
        BlockedPointProperties blockedPointProperties = new BlockedPointProperties();
        blockedPointProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        blockedPointProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        blockedPointProperties.LastTestResultId = this.LastTestResultId.GetInt32((IDataReader) reader);
        return blockedPointProperties;
      }

      internal UpdatedProperties BindAbortedRunProperties(SqlDataReader reader) => new UpdatedProperties()
      {
        Revision = this.Revision.GetInt32((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader)
      };
    }

    protected class AutomatedTestNamesColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder AutomatedTestStorage = new SqlColumnBinder(nameof (AutomatedTestStorage));

      internal TestCaseReference bind(
        SqlDataReader reader,
        Dictionary<int, List<TestCaseReference>> projectToTestRefMap)
      {
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        TestCaseReference testCaseReference = new TestCaseReference();
        testCaseReference.TestCaseReferenceId = this.TestCaseRefId.GetInt32((IDataReader) reader);
        testCaseReference.AutomatedTestName = this.AutomatedTestName.ColumnExists((IDataReader) reader) ? this.AutomatedTestName.GetString((IDataReader) reader, false) : string.Empty;
        testCaseReference.AutomatedTestStorage = this.AutomatedTestStorage.ColumnExists((IDataReader) reader) ? this.AutomatedTestStorage.GetString((IDataReader) reader, false) : string.Empty;
        if (!projectToTestRefMap.ContainsKey(int32))
          projectToTestRefMap[int32] = new List<TestCaseReference>();
        projectToTestRefMap[int32].Add(testCaseReference);
        return testCaseReference;
      }
    }
  }
}
