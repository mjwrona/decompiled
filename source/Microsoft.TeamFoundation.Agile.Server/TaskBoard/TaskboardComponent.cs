// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardComponent
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server.TaskBoard
{
  internal class TaskboardComponent : TeamFoundationSqlResourceComponent
  {
    public const string ServiceName = "Taskboard";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<TaskboardComponent>(1)
    }, "Taskboard", "WorkItem");
    private static SqlMetaData[] typ_TaskboardColumnTable = new SqlMetaData[4]
    {
      new SqlMetaData("ColumnId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ColumnName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("ColumnOrder", SqlDbType.Int),
      new SqlMetaData("Mapping", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_Int32Table = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Int)
    };
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1300011,
        new SqlExceptionFactory(typeof (TaskboardColumnNotFoundException))
      },
      {
        1300012,
        new SqlExceptionFactory(typeof (TaskboardColumnUpdateException))
      }
    };

    public TaskboardComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public IReadOnlyCollection<TaskboardColumn> GetColumns(Guid projectId, Guid teamId)
    {
      this.PrepareStoredProcedure("prc_GetTaskBoardColumns");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindGuid("@teamId", teamId);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      List<TaskboardColumn> source1 = new List<TaskboardColumn>();
      while (sqlDataReader.Read())
      {
        Guid guid = sqlDataReader.GetGuid(0);
        string name = sqlDataReader.GetString(1);
        int int32 = sqlDataReader.GetInt32(2);
        string serializedObject = sqlDataReader.GetString(3);
        DateTime dateTime = sqlDataReader.GetDateTime(4);
        List<TaskboardColumnMappingEntry> source2 = (List<TaskboardColumnMappingEntry>) null;
        if (!string.IsNullOrWhiteSpace(serializedObject))
          source2 = TeamFoundationSerializationUtility.Deserialize<List<TaskboardColumnMappingEntry>>(serializedObject);
        source1.Add(new TaskboardColumn(guid, name, int32, dateTime, (IReadOnlyCollection<TaskboardColumnMapping>) source2.Select<TaskboardColumnMappingEntry, TaskboardColumnMapping>((System.Func<TaskboardColumnMappingEntry, TaskboardColumnMapping>) (dbo => dbo.Convert())).ToList<TaskboardColumnMapping>()));
      }
      return (IReadOnlyCollection<TaskboardColumn>) source1.OrderBy<TaskboardColumn, int>((System.Func<TaskboardColumn, int>) (c => c.Order)).ToList<TaskboardColumn>();
    }

    public void UpdateColumns(
      Guid projectId,
      Guid teamId,
      Guid changeBy,
      DateTime readTime,
      IReadOnlyCollection<UpdateTaskboardColumn> columnUpdates)
    {
      this.PrepareStoredProcedure("prc_UpdateTaskboardColumns");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindGuid("@teamId", teamId);
      this.BindGuid("@changeBy", changeBy);
      this.BindDateTime2("@readDateTime", readTime);
      this.BindTaskboardColumnTable("@columnTable", columnUpdates);
      this.ExecuteNonQuery();
    }

    public Dictionary<int, (Guid columnId, string columnName)> GetWorkItemColumns(
      Guid projectId,
      Guid teamId,
      IReadOnlyCollection<int> workItemIds)
    {
      this.PrepareStoredProcedure("prc_GetTaskBoardWorkItemColumns");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindGuid("@teamId", teamId);
      this.BindTable("@workItemIds", "typ_Int32Table", workItemIds.Select<int, SqlDataRecord>((System.Func<int, SqlDataRecord>) (id =>
      {
        SqlDataRecord workItemColumns = new SqlDataRecord(TaskboardComponent.typ_Int32Table);
        workItemColumns.SetInt32(0, id);
        return workItemColumns;
      })));
      SqlDataReader sqlDataReader = this.ExecuteReader();
      Dictionary<int, (Guid, string)> workItemColumns1 = new Dictionary<int, (Guid, string)>();
      while (sqlDataReader.Read())
      {
        int int32 = sqlDataReader.GetInt32(0);
        Guid guid = sqlDataReader.GetGuid(1);
        string str = sqlDataReader.GetString(2);
        workItemColumns1[int32] = (guid, str);
      }
      return workItemColumns1;
    }

    public void UpdateWorkItemColumn(
      Guid projectId,
      Guid teamId,
      int workItemId,
      Guid columnId,
      Guid changeBy)
    {
      this.PrepareStoredProcedure("prc_UpdateTaskBoardWorkItemColumn");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindGuid("@teamId", teamId);
      this.BindInt("@workItemId", workItemId);
      this.BindGuid("@columnId", columnId);
      this.BindGuid("@changeBy", changeBy);
      this.ExecuteNonQuery();
    }

    protected virtual SqlParameter BindTaskboardColumnTable(
      string parameterName,
      IReadOnlyCollection<UpdateTaskboardColumn> columnUpdates)
    {
      return this.BindTable(parameterName, "typ_TaskboardColumnTable", columnUpdates.Select<UpdateTaskboardColumn, SqlDataRecord>((System.Func<UpdateTaskboardColumn, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TaskboardComponent.typ_TaskboardColumnTable);
        if (row.Id.HasValue)
          sqlDataRecord.SetGuid(0, row.Id.Value);
        sqlDataRecord.SetSqlString(1, (SqlString) row.Name);
        sqlDataRecord.SetInt32(2, row.Order);
        sqlDataRecord.SetSqlString(3, (SqlString) TeamFoundationSerializationUtility.SerializeToString<List<TaskboardColumnMappingEntry>>(TaskboardColumnMappingEntry.Convert(row.Mappings)));
        return sqlDataRecord;
      })));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) TaskboardComponent.s_sqlExceptionFactories;
  }
}
