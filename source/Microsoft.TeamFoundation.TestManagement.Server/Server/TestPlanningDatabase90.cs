// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase90
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
  public class TestPlanningDatabase90 : TestPlanningDatabase89
  {
    internal TestPlanningDatabase90(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase90()
    {
    }

    internal override List<int> DeleteTestPointsAndRunsForSuites(
      Guid projectGuid,
      Guid updatedBy,
      List<int> suiteIds,
      int deletionBatchSize,
      int deleteTestPointsAndRunsForSuitesSprocExecTimeOutInSec)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.DeleteTestPointsAndRunsForSuites");
        this.PrepareStoredProcedure("TestManagement.prc_DeleteTestPointsAndRunsForSuites", deleteTestPointsAndRunsForSuitesSprocExecTimeOutInSec);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindIdTypeTable("@suiteIds", (IEnumerable<int>) suiteIds);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindInt("@softDeletionBatchSize", deletionBatchSize);
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TestRunId");
        List<int> intList = new List<int>();
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
          intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
        return intList;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.DeleteTestPointsAndRunsForSuites");
      }
    }

    internal override void FetchTestRunsToBeDeletedForSuites(
      Guid projectGuid,
      List<int> suiteIds,
      int fetchTestRunsToBeDeletedForSuitesSprocExecTimeOutInSec,
      out (int, List<int>) planRunIds)
    {
      planRunIds = (int.MinValue, (List<int>) null);
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.FetchTestRunsToBeDeletedForSuites");
        this.PrepareStoredProcedure("TestManagement.prc_FetchTestRunsToBeDeletedForSuites", fetchTestRunsToBeDeletedForSuitesSprocExecTimeOutInSec);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindIdTypeTable("@suiteIds", (IEnumerable<int>) suiteIds);
        planRunIds = this.ReadPlanIdAndAffectedRunIds(this.ExecuteReader());
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.FetchTestRunsToBeDeletedForSuites");
      }
    }

    protected (int PlanId, List<int> PointIds) ReadPlanIdAndAffectedRunIds(SqlDataReader reader)
    {
      int num = 0;
      if (reader.Read())
        num = new SqlColumnBinder("PlanId").GetInt32((IDataReader) reader);
      (int, List<int>) valueTuple = (num, new List<int>());
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TestRunId");
      if (reader.NextResult())
      {
        while (reader.Read())
          valueTuple.Item2.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      }
      return valueTuple;
    }
  }
}
