// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase55
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
  public class TestPlanningDatabase55 : TestPlanningDatabase54
  {
    internal TestPlanningDatabase55(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase55()
    {
    }

    internal override Dictionary<int, TestPoint> QuerySuitePoints(
      int planId,
      List<byte> pointStates,
      List<byte> pointOutcomes,
      List<Guid> assignedTesters,
      List<int> configurationIds,
      int minPointId,
      int batchSize,
      out int maxPointId)
    {
      this.PrepareStoredProcedure("TestManagement.prc_QuerySuitePoints");
      this.BindInt("@planId", planId);
      this.BindGuidTable("@assignedTesters", (IEnumerable<Guid>) assignedTesters);
      this.BindInt32TypeTable("@configurationIds", (IEnumerable<int>) configurationIds);
      this.BindInt("@minPointId", minPointId);
      this.BindInt("@batchSize", batchSize);
      Dictionary<int, TestPoint> dictionary = new Dictionary<int, TestPoint>();
      TestPlanningDatabase55.QuerySuitePointsColumns suitePointsColumns = new TestPlanningDatabase55.QuerySuitePointsColumns();
      SqlDataReader reader = this.ExecuteReader();
      maxPointId = minPointId;
      while (reader.Read())
      {
        TestPoint testPoint = suitePointsColumns.bind(reader);
        dictionary[testPoint.PointId] = testPoint;
        maxPointId = testPoint.PointId;
      }
      return dictionary;
    }

    private class QuerySuitePointsColumns
    {
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastResetToActive = new SqlColumnBinder(nameof (LastResetToActive));

      internal TestPoint bind(SqlDataReader reader, bool includeSuiteName = false) => new TestPoint()
      {
        PointId = this.PointId.GetInt32((IDataReader) reader),
        SuiteId = this.SuiteId.GetInt32((IDataReader) reader),
        State = this.State.GetByte((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader),
        LastResetToActive = this.LastResetToActive.GetDateTime((IDataReader) reader)
      };
    }
  }
}
