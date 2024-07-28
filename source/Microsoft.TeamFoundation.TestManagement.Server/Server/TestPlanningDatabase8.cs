// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase8
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase8 : TestPlanningDatabase7
  {
    internal TestPlanningDatabase8(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase8()
    {
    }

    internal override List<int> FetchDeletedTestPlanIds(Guid projectGuid, List<int> idsToFetch)
    {
      this.RequestContext.TraceEnter("Database", "TestPlanDatabase.FetchDeletedTestPlanIds");
      this.PrepareStoredProcedure("prc_FetchDeletedTestPlanIds");
      this.BindIdTypeTable("@idsTable", (IEnumerable<int>) idsToFetch);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      SqlDataReader reader = this.ExecuteReader();
      List<int> intList = new List<int>();
      TestPlanningDatabase.PlanIdColumn planIdColumn = new TestPlanningDatabase.PlanIdColumn();
      while (reader.Read())
        intList.Add(planIdColumn.PlanId.GetInt32((IDataReader) reader));
      return intList;
    }

    internal override List<TestPlan> FetchTestPlans(
      TestManagementRequestContext context,
      List<int> deletedIds,
      Guid projectGuid,
      bool fetchSuitesMetaData,
      IdAndRev[] idsToFetch,
      List<TestPlan> witPlans,
      out Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap,
      bool excludePlansWithNoRootSuite = true)
    {
      List<int> idsToFetch1 = new List<int>(witPlans.Select<TestPlan, int>((System.Func<TestPlan, int>) (p => p.PlanId)));
      List<int> deletedPlanIds = this.FetchDeletedTestPlanIds(projectGuid, idsToFetch1);
      List<TestPlan> list = witPlans.Where<TestPlan>((System.Func<TestPlan, bool>) (p => !deletedPlanIds.Contains(p.PlanId))).ToList<TestPlan>();
      return base.FetchTestPlans(context, deletedIds, projectGuid, fetchSuitesMetaData, idsToFetch, list, out projectsSuitesMap, excludePlansWithNoRootSuite);
    }
  }
}
