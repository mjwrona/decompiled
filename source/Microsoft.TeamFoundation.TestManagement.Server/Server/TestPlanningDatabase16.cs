// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase16
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase16 : TestPlanningDatabase15
  {
    public override IEnumerable<int> GetTestPointdIdsForOutcomeSync(int planId, int[] testPointIds)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPointDatabase.GetTestPointdIdsForOutcomeSync"))
      {
        this.PrepareStoredProcedure("prc_GetTestPointIdsForOutcomeSync");
        this.BindInt("@planId", planId);
        this.BindIdTypeTable("@testPointIds", (IEnumerable<int>) testPointIds);
        return this.ReadPointIdsFromDb();
      }
    }

    protected IEnumerable<int> ReadPointIdsFromDb()
    {
      List<int> intList = new List<int>();
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("PointId");
      while (reader.Read())
        intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      return (IEnumerable<int>) intList;
    }

    internal TestPlanningDatabase16(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase16()
    {
    }

    internal override TestPlanHubData FetchTestPlanHubData(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool includeTestPoints,
      int configuration,
      Guid tester)
    {
      TestPlanningDatabase.FetchTestPlanHubDataColumnsBase fetchTestPlanHubDataColumns = (TestPlanningDatabase.FetchTestPlanHubDataColumnsBase) new TestPlanningDatabase15.FetchTestPlanHubDataColumns();
      return this.FetchTestPlanHubData(projectGuid, planId, suiteId, includeTestPoints, configuration, tester, fetchTestPlanHubDataColumns);
    }

    internal TestPlanHubData FetchTestPlanHubData(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool includeTestPoints,
      int configuration,
      Guid tester,
      TestPlanningDatabase.FetchTestPlanHubDataColumnsBase fetchTestPlanHubDataColumns)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.FetchTestPlanHubData"))
      {
        this.PrepareStoredProcedure("prc_FetchTestPlanHubData");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", planId);
        this.BindInt("@suiteId", suiteId);
        this.BindBoolean("@includeTestPoints", includeTestPoints);
        this.BindGuid("@testerFilter", tester);
        this.BindInt("@configuration", configuration);
        SqlDataReader reader = this.ExecuteReader();
        return fetchTestPlanHubDataColumns.bind(reader, "prc_FetchTestPlanHubData", includeTestPoints);
      }
    }
  }
}
