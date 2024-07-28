// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase29
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase29 : TestPlanningDatabase28
  {
    internal TestPlanningDatabase29(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase29()
    {
    }

    internal override void UpdateSuiteSyncStatus(
      Guid projectGuid,
      Dictionary<int, DateTime> suitesLastSyncedMap)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.UpdateSyncStatus");
        this.PrepareStoredProcedure("TestManagement.prc_UpdateSuiteSyncStatus");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindKeyValuePairInt32DateTimeTableRows("@suiteSyncDates", suitesLastSyncedMap);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.UpdateSyncStatus");
      }
    }
  }
}
