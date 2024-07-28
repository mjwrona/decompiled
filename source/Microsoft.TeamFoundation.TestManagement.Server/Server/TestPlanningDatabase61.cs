// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase61
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase61 : TestPlanningDatabase60
  {
    internal TestPlanningDatabase61(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase61()
    {
    }

    internal override void SyncSuites(List<ServerTestSuite> suites)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.SyncSuites");
        this.PrepareStoredProcedure("prc_SyncSuitesAccountLevel");
        this.BindServerTestSuiteTypeTable2("@suiteMetaDataTable", (IEnumerable<ServerTestSuite>) suites);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.SyncSuites");
      }
    }
  }
}
