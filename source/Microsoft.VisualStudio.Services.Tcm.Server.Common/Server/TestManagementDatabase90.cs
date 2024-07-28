// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase90
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase90 : TestManagementDatabase89
  {
    public override List<int> DeleteTestSuiteRunsData(
      Guid projectGuid,
      Guid updatedBy,
      int testPlanId,
      List<int> runIds,
      int deleteTestSuiteRunsDataSprocExecTimeOutInSec)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.DeleteTestSuiteRunsData");
        this.PrepareStoredProcedure("TestResult.prc_DeleteTestSuiteRunsData", deleteTestSuiteRunsDataSprocExecTimeOutInSec);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectGuid));
        this.BindInt("@testPlanId", testPlanId);
        this.BindInt32TypeTable("@runIds", (IEnumerable<int>) runIds);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TestRunId");
        List<int> intList = new List<int>();
        while (reader.Read())
          intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
        return intList;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.DeleteTestSuiteRunsData");
      }
    }

    internal TestManagementDatabase90(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase90()
    {
    }
  }
}
