// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase69
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase69 : TestManagementDatabase68
  {
    internal TestManagementDatabase69(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase69()
    {
    }

    public override void CreateTestRunCompletedEntry(
      Guid projectId,
      int testRunId,
      int buildId,
      int releaseId,
      int releaseEnvId,
      bool isSimilarResultsEnabled)
    {
      this.PrepareStoredProcedure("TestResult.prc_CreateTestRunCompletedEntry");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@buildId", buildId);
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@releaseEnvId", releaseEnvId);
      this.BindBoolean("@isSimilarResultsEnabled", isSimilarResultsEnabled);
      this.ExecuteNonQuery();
    }

    public override void GetTestRunsForFailureFailureBucketing(
      int batchSize,
      out Dictionary<Guid, List<int>> runIds)
    {
      this.PrepareStoredProcedure("TestResult.prc_GetTestRunsForFailureBucketing");
      this.BindInt("@batchSize", batchSize);
      runIds = new Dictionary<Guid, List<int>>();
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
      {
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("DataspaceId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("testRunId");
        int int32_1 = sqlColumnBinder1.GetInt32((IDataReader) reader);
        int int32_2 = sqlColumnBinder2.GetInt32((IDataReader) reader);
        Guid dataspaceIdentifier = this.GetDataspaceIdentifier(int32_1);
        if (!runIds.ContainsKey(dataspaceIdentifier))
          runIds[dataspaceIdentifier] = new List<int>();
        runIds[dataspaceIdentifier].Add(int32_2);
      }
    }

    public override void DeleteRunCompletedEntriesForSimilarResultsJob(
      Guid projectId,
      List<int> testRunIds)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteRunCompletedEntriesForSimilarResultsJob");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt32TypeTable("@runsToDelete", (IEnumerable<int>) testRunIds);
      this.ExecuteNonQuery();
    }

    public override void InsertSimilarTestResults(Guid projectId, List<TestResultField> results)
    {
      this.PrepareStoredProcedure("TestResult.prc_InsertSimilarTestResults");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindTestResult_TestCaseResultFieldValueHashTypeTable("@results", (IEnumerable<TestResultField>) results);
      this.ExecuteNonQuery();
    }

    public override List<TestResultField> QueryFieldsValuesForBucketing(
      Guid projectId,
      List<int> testRunIds)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryFieldsValuesForBucketing");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt32TypeTable("@testRunIds", (IEnumerable<int>) testRunIds);
      List<TestResultField> testResultFieldList = new List<TestResultField>();
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
      {
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TestRunId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("TestResultId");
        SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("FieldId");
        SqlColumnBinder sqlColumnBinder4 = new SqlColumnBinder("StringValue");
        testResultFieldList.Add(new TestResultField()
        {
          TestRunId = sqlColumnBinder1.GetInt32((IDataReader) reader),
          TestResultId = sqlColumnBinder2.GetInt32((IDataReader) reader),
          FieldId = sqlColumnBinder3.GetInt32((IDataReader) reader),
          FieldValue = sqlColumnBinder4.GetString((IDataReader) reader, false),
          TestSubResultId = 0
        });
      }
      return testResultFieldList;
    }

    public override List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> QuerySimilarTestResults(
      Guid projectId,
      int testRunId,
      int testCaseResultId,
      int testSubResultId,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      this.PrepareStoredProcedure("TestResult.prc_QuerySimilarTestResults");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testCaseResultId);
      this.BindInt("@testSubResultId", testSubResultId);
      this.BindInt("@continuationTokenRunId", continuationTokenRunId);
      this.BindInt("@continuationTokenResultId", continuationTokenResultId);
      this.BindInt("@top", top);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
      {
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TestRunId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("TestResultId");
        SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("BuildId");
        SqlColumnBinder sqlColumnBinder4 = new SqlColumnBinder("ReleaseId");
        SqlColumnBinder sqlColumnBinder5 = new SqlColumnBinder("ReleaseEnvId");
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
        {
          Project = new ShallowReference()
          {
            Id = projectId.ToString()
          }
        };
        testCaseResult.TestRun = new ShallowReference()
        {
          Id = sqlColumnBinder1.GetInt32((IDataReader) reader).ToString()
        };
        testCaseResult.Id = sqlColumnBinder2.GetInt32((IDataReader) reader);
        testCaseResult.ReleaseReference = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
        {
          Id = sqlColumnBinder4.ColumnExists((IDataReader) reader) ? sqlColumnBinder4.GetInt32((IDataReader) reader, 0) : 0,
          EnvironmentId = sqlColumnBinder5.ColumnExists((IDataReader) reader) ? sqlColumnBinder5.GetInt32((IDataReader) reader, 0) : 0
        };
        testCaseResult.BuildReference = new BuildReference()
        {
          Id = sqlColumnBinder3.ColumnExists((IDataReader) reader) ? sqlColumnBinder3.GetInt32((IDataReader) reader, 0) : 0
        };
        testCaseResultList.Add(testCaseResult);
      }
      return testCaseResultList;
    }
  }
}
