// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase63
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase63 : TestPlanningDatabase62
  {
    public override void UpdateTestPointOutcome(
      Guid projectGuid,
      IList<TestPointOutcomeUpdateFromTestResultRequest> results)
    {
      this.PrepareStoredProcedure("prc_UpdateTestPointOutcome");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindUpdateTestPointOutcomeTypeTable("@outcomes", (IEnumerable<TestPointOutcomeUpdateFromTestResultRequest>) results);
      this.ExecuteReader();
    }

    public override void SyncPointResultToPoint(List<PointsResults2> pointResults)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestPlanningDatabase.SyncPointResultToPoint");
        this.PrepareStoredProcedure("TestManagement.prc_SyncPointResultToPoint");
        this.BindPointResults3Table("@testPointResults", (IEnumerable<PointsResults2>) pointResults);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestPlanningDatabase.SyncPointResultToPoint");
      }
    }

    public override DateTime GetLeastMigrationStartDate()
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestPlanningDatabase.GetLeastMigrationStartDate");
        this.PrepareStoredProcedure("TestManagement.prc_GetLeastMigrationStartDateFromPoint");
        return (DateTime) this.ExecuteScalar();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestPlanningDatabase.GetLeastMigrationStartDate");
      }
    }

    internal TestPlanningDatabase63(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase63()
    {
    }
  }
}
