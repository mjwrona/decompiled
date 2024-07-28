// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase17
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase17 : TestPlanningDatabase16
  {
    internal TestPlanningDatabase17(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase17()
    {
    }

    internal override List<IdAndRev> FetchSuitesRevision(Guid projectGuid, List<int> suiteIds)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.FetchSuitesRevision");
        this.PrepareStoredProcedure("prc_FetchSuitesRevision");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindIdTypeTable("@suiteIds", (IEnumerable<int>) suiteIds);
        SqlDataReader reader = this.ExecuteReader();
        List<IdAndRev> idAndRevList = new List<IdAndRev>();
        TestPlanningDatabase.IdAndRevColumns idAndRevColumns = new TestPlanningDatabase.IdAndRevColumns();
        while (reader.Read())
          idAndRevList.Add(idAndRevColumns.bind(reader));
        return idAndRevList;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.FetchSuitesRevision");
      }
    }
  }
}
