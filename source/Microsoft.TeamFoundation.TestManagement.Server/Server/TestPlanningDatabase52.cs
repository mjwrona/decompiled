// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase52
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Charting;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase52 : TestPlanningDatabase51
  {
    internal override List<TestAuthoringDetails> GetTestAuthoringChartDetails(
      string planName,
      int suiteId)
    {
      this.RequestContext.TraceEnter("Database", "ReportDatabase.GetTestAutoringChartDetails");
      try
      {
        List<TestAuthoringDetails> authoringChartDetails = new List<TestAuthoringDetails>();
        this.PrepareStoredProcedure("TestManagement.prc_GetTestAuthoringDetails");
        this.BindString("@planName", planName, 512, false, SqlDbType.NVarChar);
        this.BindInt("@suiteId", suiteId);
        SqlDataReader reader = this.ExecuteReader();
        TestPlanningDatabase52.QueryTestAuthoringDetailsColumns authoringDetailsColumns = new TestPlanningDatabase52.QueryTestAuthoringDetailsColumns();
        while (reader.Read())
          authoringChartDetails.Add(authoringDetailsColumns.bind(reader));
        return authoringChartDetails;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "ReportDatabase.GetTestAutoringChartDetails");
      }
    }

    internal TestPlanningDatabase52(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase52()
    {
    }

    private new class QueryTestAuthoringDetailsColumns
    {
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder TesterId = new SqlColumnBinder(nameof (TesterId));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Priority = new SqlColumnBinder(nameof (Priority));
      private SqlColumnBinder RunBy = new SqlColumnBinder(nameof (RunBy));
      private SqlColumnBinder IsAutomated = new SqlColumnBinder(nameof (IsAutomated));

      internal TestAuthoringDetails bind(SqlDataReader reader) => new TestAuthoringDetails()
      {
        PointId = this.PointId.GetInt32((IDataReader) reader),
        TesterId = this.TesterId.GetGuid((IDataReader) reader),
        State = this.State.GetByte((IDataReader) reader),
        ConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader),
        ConfigurationName = this.ConfigurationName.GetString((IDataReader) reader, false),
        SuiteId = this.SuiteId.GetInt32((IDataReader) reader),
        SuiteName = this.SuiteName.GetString((IDataReader) reader, false),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader),
        Priority = this.Priority.GetNullableByte((IDataReader) reader),
        RunBy = this.RunBy.GetNullableGuid((IDataReader) reader),
        IsAutomated = this.IsAutomated.GetNullableBoolean((IDataReader) reader)
      };
    }
  }
}
