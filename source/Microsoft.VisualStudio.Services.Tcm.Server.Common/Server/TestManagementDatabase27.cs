// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase27
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase27 : TestManagementDatabase26
  {
    internal TestManagementDatabase27(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase27()
    {
    }

    public override List<TestCaseReference> QueryTestCaseReference(
      Guid projectId,
      List<string> automatedTestNames,
      List<int> testCaseIds,
      int planId,
      List<int> suiteIds,
      List<int> pointIds)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestCaseReference");
        this.PrepareStoredProcedure("TestResult.prc_QueryTestCaseReference");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindTestResult_TestCaseReference2HashTypeTable("@automatedTestNameHashes", (IEnumerable<string>) automatedTestNames);
        this.BindInt32TypeTable("@testCaseIds", (IEnumerable<int>) testCaseIds);
        this.BindInt("@planId", planId);
        this.BindInt32TypeTable("@suiteIds", (IEnumerable<int>) suiteIds);
        this.BindInt32TypeTable("@pointIds", (IEnumerable<int>) pointIds);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase23.TestCaseReferenceColumns referenceColumns = new TestManagementDatabase23.TestCaseReferenceColumns();
        List<TestCaseReference> testCaseReferenceList = new List<TestCaseReference>();
        while (reader.Read())
          testCaseReferenceList.Add(referenceColumns.bind(reader));
        return testCaseReferenceList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestCaseReference");
      }
    }
  }
}
