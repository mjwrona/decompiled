// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase9
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase9 : TestManagementDatabase8
  {
    internal TestManagementDatabase9(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase9()
    {
    }

    protected List<Tuple<int, int, TestExtensionField>> GetExtensionFieldsMap(
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

    public override List<TestCaseResult> QueryTestResultHistory(
      Guid projectId,
      string automatedTestName,
      int testCaseId,
      DateTime maxCompleteDate,
      int historyDays)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultHistory");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@automatedTestName", automatedTestName, 256, true, SqlDbType.NVarChar);
      this.BindInt("@testCaseId", testCaseId);
      this.BindInt("@historyDays", historyDays);
      this.BindNullableDateTime("@maxCompleteDate", maxCompleteDate);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
      TestManagementDatabase4.QueryTestResultHistoryColumns resultHistoryColumns = new TestManagementDatabase4.QueryTestResultHistoryColumns();
      while (reader.Read())
        testCaseResultList.Add(resultHistoryColumns.bind(reader));
      return testCaseResultList;
    }
  }
}
