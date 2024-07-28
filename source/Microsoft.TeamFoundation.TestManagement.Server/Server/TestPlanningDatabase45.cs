// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase45
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
  public class TestPlanningDatabase45 : TestPlanningDatabase44
  {
    internal TestPlanningDatabase45(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase45()
    {
    }

    internal override Guid QueueDeleteTestPlan(Guid projectGuid, int testPlanId, Guid updatedBy)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.QueueDeleteTestPlan");
        int dataspaceId = projectGuid.Equals(Guid.Empty) ? 0 : this.GetDataspaceId(projectGuid);
        this.PrepareStoredProcedure("prc_QueueDeleteTestPlan");
        this.BindInt("@dataspaceId", dataspaceId);
        this.BindInt("@testPlanId", testPlanId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.ExecuteNonQuery();
        return projectGuid;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.QueueDeleteTestPlan");
      }
    }

    internal override UpdatedProperties DeleteSuiteEntries2(
      Guid projectGuid,
      UpdatedProperties parentProps,
      List<TestSuiteEntry> entries,
      out (int, List<int>) planPointIds)
    {
      planPointIds = (-1, (List<int>) null);
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.DeleteSuiteEntries2");
        this.PrepareStoredProcedure("TestManagement.prc_DeleteSuiteEntriesV2");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindTestSuiteEntryTypeTable("@entriesTable", (IEnumerable<TestSuiteEntry>) entries);
        this.BindDateTime("@lastUpdated", parentProps.LastUpdated);
        this.BindGuid("@lastUpdatedBy", parentProps.LastUpdatedBy);
        planPointIds = this.ReadPlanIdAndAffectedPointIds(this.ExecuteReader());
        return parentProps;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.DeleteSuiteEntries2");
      }
    }

    internal override UpdatedProperties DeleteSuiteEntries(
      Guid projectGuid,
      UpdatedProperties parentProps,
      List<TestSuiteEntry> entries,
      out (int, List<int>) planPointIds)
    {
      planPointIds = (-1, (List<int>) null);
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.DeleteSuiteEntries");
        this.PrepareStoredProcedure("prc_DeleteSuiteEntriesV2");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindTestSuiteEntryTypeTable("@entriesTable", (IEnumerable<TestSuiteEntry>) entries);
        this.BindDateTime("@lastUpdated", parentProps.LastUpdated);
        this.BindGuid("@lastUpdatedBy", parentProps.LastUpdatedBy);
        planPointIds = this.ReadPlanIdAndAffectedPointIds(this.ExecuteReader());
        return parentProps;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.DeleteSuiteEntries");
      }
    }

    internal override (int PlanId, List<int> PointIds) DestroyTestCase(
      int testCaseId,
      List<UpdatedProperties> suiteProps,
      out Guid projectGuid)
    {
      projectGuid = Guid.Empty;
      this.PrepareStoredProcedure("prc_DestroyTestCase");
      this.BindInt("@testCaseId", testCaseId);
      this.BindUpdatedPropertiesTypeTable("@suitePropTable", (IEnumerable<UpdatedProperties>) suiteProps);
      return this.ReadPlanIdAndAffectedPointIds(this.ExecuteReader());
    }

    protected (int PlanId, List<int> PointIds) ReadPlanIdAndAffectedPointIds(SqlDataReader reader)
    {
      int num = 0;
      if (reader.Read())
        num = new SqlColumnBinder("PlanId").GetInt32((IDataReader) reader);
      (int, List<int>) valueTuple = (num, new List<int>());
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("PointId");
      if (reader.NextResult())
      {
        while (reader.Read())
          valueTuple.Item2.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      }
      return valueTuple;
    }
  }
}
