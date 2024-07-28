// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase31
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
  public class TestPlanningDatabase31 : TestPlanningDatabase30
  {
    internal TestPlanningDatabase31(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase31()
    {
    }

    internal override List<ServerTestSuite> FetchSuiteSyncStatus(
      Guid projectGuid,
      List<int> suiteIds)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.FetchSuiteSyncStatus");
        this.PrepareStoredProcedure("TestManagement.prc_FetchSuiteSyncStatus");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt32TypeTable("@suiteIds", (IEnumerable<int>) suiteIds);
        SqlDataReader reader = this.ExecuteReader();
        TestPlanningDatabase31.FetchTestSuiteSyncStatusColumns syncStatusColumns = new TestPlanningDatabase31.FetchTestSuiteSyncStatusColumns();
        List<ServerTestSuite> serverTestSuiteList = new List<ServerTestSuite>();
        while (reader.Read())
          serverTestSuiteList.Add(syncStatusColumns.bind(reader));
        return serverTestSuiteList;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.FetchSuiteSyncStatus");
      }
    }

    private class FetchTestSuiteSyncStatusColumns
    {
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder LastPopulated = new SqlColumnBinder(nameof (LastPopulated));
      private SqlColumnBinder LastSynced = new SqlColumnBinder(nameof (LastSynced));

      internal virtual ServerTestSuite bind(SqlDataReader reader) => new ServerTestSuite()
      {
        Id = this.SuiteId.GetInt32((IDataReader) reader),
        LastPopulated = this.LastPopulated.GetDateTime((IDataReader) reader),
        LastSynced = this.LastSynced.IsNull((IDataReader) reader) ? this.LastSynced.GetDateTime((IDataReader) reader) : DateTime.MinValue
      };
    }
  }
}
