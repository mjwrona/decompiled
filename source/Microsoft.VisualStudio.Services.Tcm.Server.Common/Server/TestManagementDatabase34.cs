// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase34
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase34 : TestManagementDatabase33
  {
    internal TestManagementDatabase34(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase34()
    {
    }

    internal override List<TestCaseResultIdentifier> QueryTestResults(
      int testRunId,
      Guid owner,
      byte testStatus,
      List<byte> outcomes,
      int afnStripId,
      Guid projectId,
      bool isTcmService)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResults");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindNullableInt("@testRunId", testRunId, 0);
      this.BindGuidPreserveNull("@owner", owner);
      this.BindNullableByte("@state", testStatus, (byte) 0);
      this.BindTestManagement_TinyIntTypeTable("@outcomeList", (IEnumerable<byte>) outcomes);
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
