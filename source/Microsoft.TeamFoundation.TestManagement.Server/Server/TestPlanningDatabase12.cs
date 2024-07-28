// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase12
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase12 : TestPlanningDatabase11
  {
    internal override IEnumerable<SuiteEntry> GetSuiteEntries(Guid projectId, int suiteId)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "SuiteEntryDatabase.GetSuiteEntries"))
      {
        this.PrepareStoredProcedure("prc_GetSuiteEntriesForSuite");
        this.BindInt("@suiteId", suiteId);
        return this.GetSuiteEntriesFromDb(suiteId);
      }
    }

    protected IEnumerable<SuiteEntry> GetSuiteEntriesFromDb(int suiteId)
    {
      HashSet<SuiteEntry> source = new HashSet<SuiteEntry>();
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TestCaseId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("SequenceNumber");
      SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("ChildSuiteId");
      while (reader.Read())
      {
        int int32_1 = sqlColumnBinder2.GetInt32((IDataReader) reader);
        int int32_2 = sqlColumnBinder1.GetInt32((IDataReader) reader);
        int int32_3 = sqlColumnBinder3.GetInt32((IDataReader) reader);
        source.Add(new SuiteEntry(suiteId, int32_1, int32_2, int32_3));
      }
      return (IEnumerable<SuiteEntry>) source.ToList<SuiteEntry>();
    }

    internal TestPlanningDatabase12(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase12()
    {
    }

    internal override List<int> GetInUseConfigurationsForSuite(Guid projectGuid, int suiteId)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestSuiteDatabase.GetInUseConfigurationsForSuite"))
      {
        this.PrepareStoredProcedure("prc_QueryInUseConfigurationsForSuite");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", suiteId);
        return this.GetConfigurationsFromDb();
      }
    }

    internal override List<int> GetInUseConfigurationsForTestCases(
      Guid projectGuid,
      Dictionary<int, List<int>> testCases)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestSuiteDatabase.GetInUseConfigurationsForTestCases"))
      {
        this.PrepareStoredProcedure("prc_QueryInUseConfigurationsForTestCases");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindTestCaseAndSuiteTypeTable("@testCases", testCases);
        return this.GetConfigurationsFromDb();
      }
    }
  }
}
