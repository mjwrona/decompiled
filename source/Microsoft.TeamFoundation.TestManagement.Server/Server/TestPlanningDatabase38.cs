// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase38
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase38 : TestPlanningDatabase37
  {
    internal TestPlanningDatabase38(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase38()
    {
    }

    internal override List<int> FetchTestCaseIds(Guid projectId, int suiteId)
    {
      this.PrepareStoredProcedure("prc_FetchTestCasesFromSuiteRecursive");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@suiteId", suiteId);
      List<int> intList = new List<int>();
      SqlDataReader sqlDataReader = this.ExecuteReader();
      while (sqlDataReader.Read())
        intList.Add(sqlDataReader.GetInt32(0));
      return intList;
    }
  }
}
