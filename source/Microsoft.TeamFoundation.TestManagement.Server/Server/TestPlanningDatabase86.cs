// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase86
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase86 : TestPlanningDatabase85
  {
    internal TestPlanningDatabase86(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase86()
    {
    }

    internal override List<int> GetAssignedConfigurationsForSuite(Guid projectGuid, int suiteId)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestSuiteDatabase.GetInUseConfigurationsForSuite"))
      {
        this.PrepareStoredProcedure("prc_QueryInUseConfigurationsForSuite");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", suiteId);
        this.BindBoolean("@includeAllConfigsInUse", false);
        return this.GetConfigurationsFromDb();
      }
    }

    internal override List<int> GetInUseConfigurationsForSuite(Guid projectGuid, int suiteId)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestSuiteDatabase.GetInUseConfigurationsForSuite"))
      {
        this.PrepareStoredProcedure("prc_QueryInUseConfigurationsForSuite");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", suiteId);
        this.BindBoolean("@includeAllConfigsInUse", true);
        return this.GetConfigurationsFromDb();
      }
    }
  }
}
