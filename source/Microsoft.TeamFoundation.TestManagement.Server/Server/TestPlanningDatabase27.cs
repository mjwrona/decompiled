// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase27
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase27 : TestPlanningDatabase26
  {
    internal TestPlanningDatabase27(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase27()
    {
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
        this.ExecuteNonQuery();
        return parentProps;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.DeleteSuiteEntries2");
      }
    }

    internal override void DeleteTestPointsForSuites(
      Guid projectGuid,
      List<int> suiteIds,
      int deletionBatchSize)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.DeleteTestPointsForSuites");
        this.PrepareStoredProcedure("TestManagement.prc_DeleteTestPointsForSuites");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindIdTypeTable("@suiteIds", (IEnumerable<int>) suiteIds);
        this.BindInt("@deletionBatchSize", deletionBatchSize);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.DeleteTestPointsForSuites");
      }
    }
  }
}
