// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase79
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
  public class TestManagementDatabase79 : TestManagementDatabase78
  {
    internal TestManagementDatabase79(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase79()
    {
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
        newTestCaseRefIds = new List<int>();
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
        if (reader.NextResult())
        {
          while (reader.Read())
            newTestCaseRefIds.Add(reader.GetInt32(0));
        }
        return testCaseReference;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.CreateTestCaseReference");
      }
    }

    public override List<TestCaseResultIdentifier> QueryNewTestResults(
      int testRunId,
      Guid owner,
      byte testStatus,
      List<byte> outcomes,
      int afnStripId,
      Guid projectId,
      int newFieldId)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryNewTestResults");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindNullableInt("@testRunId", testRunId, 0);
      this.BindGuidPreserveNull("@owner", owner);
      this.BindNullableByte("@state", testStatus, (byte) 0);
      this.BindTestManagement_TinyIntTypeTable("@outcomeList", (IEnumerable<byte>) outcomes);
      this.BindInt("@newFieldId", newFieldId);
      this.BindNullableInt("@afnStripId", afnStripId, 0);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResultIdentifier> resultIdentifierList = new List<TestCaseResultIdentifier>();
      TestManagementDatabase.QueryTestResultsColumns testResultsColumns = new TestManagementDatabase.QueryTestResultsColumns();
      while (reader.Read())
        resultIdentifierList.Add(testResultsColumns.bind(reader));
      return resultIdentifierList;
    }
  }
}
