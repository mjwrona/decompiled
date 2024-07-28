// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase20
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase20 : TestPlanningDatabase19
  {
    internal TestPlanningDatabase20(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase20()
    {
    }

    internal override List<TestPoint> GetPointsByQuery(
      Guid projectGuid,
      int[] testCaseIds,
      string[] configurations,
      Guid[] testers,
      int skip,
      int top,
      bool includeSuiteName = false)
    {
      this.PrepareStoredProcedure("prc_QueryTestPoints");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindIdTypeTable("@testCaseIds", (IEnumerable<int>) testCaseIds);
      this.BindNameTypeTable("@configurations", (IEnumerable<string>) configurations);
      this.BindGuidTable("@testers", (IEnumerable<Guid>) testers);
      this.BindInt("@skip", skip);
      this.BindInt("@top", top);
      SqlDataReader reader = this.ExecuteReader();
      TestPlanningDatabase.FetchTestPointsColumns testPointsColumns = new TestPlanningDatabase.FetchTestPointsColumns();
      List<TestPoint> pointsByQuery = new List<TestPoint>();
      while (reader.Read())
        pointsByQuery.Add(testPointsColumns.bind(reader, includeSuiteName));
      return pointsByQuery;
    }
  }
}
