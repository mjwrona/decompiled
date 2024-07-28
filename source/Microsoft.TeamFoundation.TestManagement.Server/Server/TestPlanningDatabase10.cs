// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase10
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Charting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase10 : TestPlanningDatabase9
  {
    internal override List<TestFieldData> GetTestExecutionReport(
      string planName,
      int suiteId,
      List<KeyValuePair<int, string>> dimensionList)
    {
      this.RequestContext.TraceEnter("Database", "ReportDatabase.GetTestExecutionReport");
      List<TestFieldData> testExecutionReport = new List<TestFieldData>();
      try
      {
        this.PrepareStoredProcedure("TestManagement.prc_GetTestExecutionReport");
        this.BindString("@planName", planName, 512, false, SqlDbType.NVarChar);
        this.BindInt("@suiteId", suiteId);
        this.BindNameTypeTable("@dimensions", (IEnumerable<string>) dimensionList.Select<KeyValuePair<int, string>, string>((System.Func<KeyValuePair<int, string>, string>) (m => m.Value)).ToList<string>());
        SqlDataReader reader = this.ExecuteReader();
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            Dictionary<string, object> dimensions = new Dictionary<string, object>();
            foreach (string str in dimensionList.OrderBy<KeyValuePair<int, string>, int>((System.Func<KeyValuePair<int, string>, int>) (x => x.Key)).Select<KeyValuePair<int, string>, string>((System.Func<KeyValuePair<int, string>, string>) (x => x.Value)))
            {
              object obj = new SqlColumnBinder(str).GetObject((IDataReader) reader);
              dimensions[str] = obj;
            }
            long int64 = Convert.ToInt64(new SqlColumnBinder("AggTestsCount").GetInt32((IDataReader) reader));
            testExecutionReport.Add(new TestFieldData(dimensions, int64));
          }
        }
        return testExecutionReport;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "ReportDatabase.GetTestExecutionReport");
      }
    }

    internal TestPlanningDatabase10(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase10()
    {
    }

    internal override Dictionary<int, string> QueryConfigurations(Guid projectGuid, int planId)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.QueryConfigurations"))
      {
        this.PrepareStoredProcedure("prc_QueryConfigurations");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", planId);
        Dictionary<int, string> dictionary = new Dictionary<int, string>();
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("ConfigurationId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("Name");
        while (reader.Read())
        {
          int int32 = sqlColumnBinder1.GetInt32((IDataReader) reader);
          string str = sqlColumnBinder2.GetString((IDataReader) reader, false);
          dictionary[int32] = str;
        }
        return dictionary;
      }
    }
  }
}
