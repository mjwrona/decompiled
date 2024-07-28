// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase62
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase62 : TestPlanningDatabase61
  {
    internal TestPlanningDatabase62(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase62()
    {
    }

    internal override List<TestPointRecord> QueryTestPointsByOutcomeMigrationDate(
      int batchSize,
      TestPointWatermark fromWatermark,
      out TestPointWatermark toWatermark,
      TestArtifactSource dataSource)
    {
      this.PrepareStoredProcedure("TestManagement.prc_QueryTestPointsByOutcomeMigrationDate");
      DateTime parameterValue = fromWatermark.WatermarkDate < SqlDateTime.MinValue.Value ? SqlDateTime.MinValue.Value : fromWatermark.WatermarkDate;
      int pointId1 = fromWatermark.PointId;
      this.BindInt("@batchSize", batchSize);
      this.BindDateTime("@fromMigrationDate", parameterValue, true);
      this.BindInt("@fromPointId", pointId1);
      SqlDataReader reader = this.ExecuteReader();
      List<TestPointRecord> testPointRecordList = new List<TestPointRecord>();
      TestPlanningDatabase43.FetchTestPointRecord fetchTestPointRecord = new TestPlanningDatabase43.FetchTestPointRecord();
      DateTime outcomeMigrationDate = parameterValue;
      int pointId2 = pointId1;
      while (reader.Read())
        testPointRecordList.Add(fetchTestPointRecord.bind(reader, dataSource, out outcomeMigrationDate, out pointId2));
      if (reader.NextResult())
      {
        while (reader.Read())
          testPointRecordList.Add(fetchTestPointRecord.bind(reader, dataSource, out outcomeMigrationDate, out pointId2));
      }
      toWatermark = new TestPointWatermark()
      {
        WatermarkDate = outcomeMigrationDate,
        PointId = pointId2
      };
      return testPointRecordList;
    }

    internal override List<TestPointHistoryRecord> QueryTestPointHistoryByWatermarkDate(
      int batchSize,
      TestPointHistoryWatermark fromWatermark,
      out TestPointHistoryWatermark toWatermark,
      TestArtifactSource dataSource)
    {
      this.PrepareStoredProcedure("TestManagement.prc_QueryTestPointHistoryByWatermarkDate");
      DateTime parameterValue = fromWatermark.WatermarkDate < SqlDateTime.MinValue.Value ? SqlDateTime.MinValue.Value : fromWatermark.WatermarkDate;
      int pointId1 = fromWatermark != null ? fromWatermark.PointId : 0;
      int changeNumber1 = fromWatermark != null ? fromWatermark.ChangeNumber : 0;
      this.BindInt("@batchSize", batchSize);
      this.BindDateTime("@fromWatermarkDate", parameterValue, true);
      this.BindInt("@fromPointId", pointId1);
      this.BindInt("@fromChangeNumber", changeNumber1);
      SqlDataReader reader = this.ExecuteReader();
      List<TestPointHistoryRecord> pointHistoryRecordList = new List<TestPointHistoryRecord>();
      TestPlanningDatabase43.FetchTestPointHistoryRecord pointHistoryRecord = new TestPlanningDatabase43.FetchTestPointHistoryRecord();
      DateTime watermarkDate = parameterValue;
      int pointId2 = pointId1;
      int changeNumber2 = changeNumber1;
      while (reader.Read())
        pointHistoryRecordList.Add(pointHistoryRecord.bind(reader, dataSource, out watermarkDate, out pointId2, out changeNumber2));
      if (reader.NextResult())
      {
        while (reader.Read())
          pointHistoryRecordList.Add(pointHistoryRecord.bind(reader, dataSource, out watermarkDate, out pointId2, out changeNumber2));
      }
      if (reader.NextResult())
      {
        while (reader.Read())
          pointHistoryRecordList.Add(pointHistoryRecord.bind(reader, dataSource, out watermarkDate, out pointId2, out changeNumber2));
      }
      toWatermark = new TestPointHistoryWatermark()
      {
        WatermarkDate = watermarkDate,
        PointId = pointId2,
        ChangeNumber = changeNumber2
      };
      return pointHistoryRecordList;
    }

    internal override List<TestPlanRecord> QueryTestPlansByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      this.PrepareStoredProcedure("TestManagement.prc_QueryTestPlansByChangedDate");
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindInt("@batchSize", batchSize);
      this.BindDateTime("@fromChangedDate", fromDate, true);
      SqlDataReader reader = this.ExecuteReader();
      List<TestPlanRecord> testPlanRecordList = new List<TestPlanRecord>();
      TestPlanningDatabase62.FetchTestPlansRecord fetchTestPlansRecord = new TestPlanningDatabase62.FetchTestPlansRecord();
      Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
      toDate = fromDate;
      while (reader.Read())
        testPlanRecordList.Add(fetchTestPlansRecord.Bind(reader, dataspaceIdentifier, dataSource, out toDate));
      return testPlanRecordList;
    }

    protected class FetchTestPlansRecord
    {
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder RootSuiteId = new SqlColumnBinder(nameof (RootSuiteId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder ReleaseDefinitionId = new SqlColumnBinder(nameof (ReleaseDefinitionId));
      private SqlColumnBinder ReleaseEnvDefinitionId = new SqlColumnBinder(nameof (ReleaseEnvDefinitionId));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));

      internal virtual TestPlanRecord Bind(
        SqlDataReader reader,
        Guid projectId,
        TestArtifactSource datasource,
        out DateTime lastUpdatedDate)
      {
        lastUpdatedDate = this.LastUpdated.GetDateTime((IDataReader) reader);
        return new TestPlanRecord()
        {
          ProjectGuid = projectId,
          TestPlanId = this.PlanId.GetInt32((IDataReader) reader),
          RootSuiteId = this.RootSuiteId.GetInt32((IDataReader) reader),
          State = this.State.GetByte((IDataReader) reader),
          BuildDefinitionId = this.BuildDefinitionId.GetInt32((IDataReader) reader),
          BuildUri = this.BuildUri.GetString((IDataReader) reader, true),
          ReleaseDefinitionId = this.ReleaseDefinitionId.GetInt32((IDataReader) reader),
          ReleaseEnvDefinitionId = this.ReleaseEnvDefinitionId.GetInt32((IDataReader) reader)
        };
      }
    }
  }
}
