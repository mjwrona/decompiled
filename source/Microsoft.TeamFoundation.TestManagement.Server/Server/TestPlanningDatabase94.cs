// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase94
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase94 : TestPlanningDatabase93
  {
    internal TestPlanningDatabase94(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase94()
    {
    }

    internal override void BulkUpdateOfTestPointStateAndTester(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool updateRunResultsInTCM,
      List<UpdatePointStateAndTester> updatePointStateAndTesters)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanDatabase.BulkUpdateOfTestPointStateAndTester");
        this.PrepareStoredProcedure("TestManagement.prc_BulkUpdateTestPointStateAndTesterModified");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", planId);
        this.BindInt("@suiteId", suiteId);
        this.BindTestManagement_UpdatePointStateAndTesterTypeTable("@updatePointStateAndTesterTable", (IEnumerable<UpdatePointStateAndTester>) updatePointStateAndTesters);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanDatabase.BulkUpdateOfTestPointStateAndTester");
      }
    }
  }
}
