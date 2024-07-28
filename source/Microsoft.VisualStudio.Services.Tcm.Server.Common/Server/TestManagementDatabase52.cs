// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase52
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase52 : TestManagementDatabase51
  {
    internal override Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData GetTestExecutionReport2(
      Guid projectId,
      int planId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> testAuthoringDetails,
      List<string> dimensionList)
    {
      this.RequestContext.TraceEnter("Database", "ChartingDatabase.GetTestExecutionReport2");
      Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData executionReport2 = new Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData();
      try
      {
        this.PrepareStoredProcedure("TestManagement.prc_GetTestExecutionReport2");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@planId", planId);
        this.BindTestAuthoringDetails2TypeTableTable("@authoringDetails", (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails>) testAuthoringDetails);
        this.BindNameTypeTable("@dimensions", (IEnumerable<string>) dimensionList);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            Dictionary<string, object> dimensionValues = new Dictionary<string, object>();
            foreach (string dimension in dimensionList)
            {
              object obj = new SqlColumnBinder(dimension).GetObject((IDataReader) reader);
              dimensionValues[dimension] = obj;
            }
            long int64 = Convert.ToInt64(new SqlColumnBinder("AggTestsCount").GetInt32((IDataReader) reader));
            executionReport2.AddReportDatarow(dimensionValues, int64);
          }
        }
        return executionReport2;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "ChartingDatabase.GetTestExecutionReport2");
      }
    }

    internal TestManagementDatabase52(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase52()
    {
    }
  }
}
