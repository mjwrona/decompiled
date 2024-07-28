// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase47
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Charting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase47 : TestPlanningDatabase46
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
        TestPlanningDatabase47.QueryTestAuthoringDetailsColumns authoringDetailsColumns = new TestPlanningDatabase47.QueryTestAuthoringDetailsColumns();
        while (reader.Read())
          authoringChartDetails.Add(authoringDetailsColumns.bind(reader));
        return authoringChartDetails;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "ReportDatabase.GetTestAutoringChartDetails");
      }
    }

    internal TestPlanningDatabase47(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase47()
    {
    }

    internal override Guid QueueDeleteTestPlan(Guid projectGuid, int testPlanId, Guid updatedBy)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.QueueDeleteTestPlan");
        int num = projectGuid.Equals(Guid.Empty) ? 0 : this.GetDataspaceId(projectGuid);
        this.PrepareStoredProcedure("prc_QueueDeleteTestPlan");
        this.BindInt("@dataspaceId", num);
        this.BindInt("@testPlanId", testPlanId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        SqlDataReader reader = this.ExecuteReader();
        if (projectGuid.Equals(Guid.Empty))
        {
          if (reader.Read())
            num = new SqlColumnBinder("DataspaceId").GetInt32((IDataReader) reader);
          if (num > 0)
            projectGuid = this.GetDataspaceIdentifier(num);
        }
        return projectGuid;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.QueueDeleteTestPlan");
      }
    }

    internal override List<int> GetDescendentSuites(Guid projectGuid, int parentSuiteId)
    {
      try
      {
        this.PrepareStoredProcedure("prc_GetDescendentSuites");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentSuiteId);
        SqlDataReader reader = this.ExecuteReader();
        List<int> descendentSuites = new List<int>();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("SuiteId");
        while (reader.Read())
          descendentSuites.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
        return descendentSuites;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    private class QueryTestAuthoringDetailsColumns
    {
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder TesterId = new SqlColumnBinder(nameof (TesterId));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));

      internal TestAuthoringDetails bind(SqlDataReader reader) => new TestAuthoringDetails()
      {
        PointId = this.PointId.GetInt32((IDataReader) reader),
        TesterId = this.TesterId.GetGuid((IDataReader) reader),
        ConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader),
        ConfigurationName = this.ConfigurationName.GetString((IDataReader) reader, false),
        SuiteId = this.SuiteId.GetInt32((IDataReader) reader),
        SuiteName = this.SuiteName.GetString((IDataReader) reader, false)
      };
    }
  }
}
