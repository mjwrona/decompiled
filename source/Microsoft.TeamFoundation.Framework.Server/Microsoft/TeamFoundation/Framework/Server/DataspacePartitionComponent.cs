// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspacePartitionComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspacePartitionComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<DataspacePartitionComponent>(1),
      (IComponentCreator) new ComponentCreator<DataspacePartitionComponent2>(2)
    }, "DataspacePartition");
    private static readonly SqlMetaData[] typ_DataspacePartitionMapTable = new SqlMetaData[3]
    {
      new SqlMetaData("HashStart", SqlDbType.Int),
      new SqlMetaData("HashEnd", SqlDbType.Int),
      new SqlMetaData("DataspaceIdentifier", SqlDbType.UniqueIdentifier)
    };

    public DataspacePartitionComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected SqlParameter BindDataspacePartitionMapTable(
      string parameterName,
      DataspacePartitionMap partitionMap)
    {
      return this.BindTable(parameterName, "typ_DataspacePartitionMapTable", DataspacePartitionComponent.BindDataspaceHashRangeRow((IEnumerable<DataspaceHashRange>) partitionMap.Ranges));
    }

    internal static IEnumerable<SqlDataRecord> BindDataspaceHashRangeRow(
      IEnumerable<DataspaceHashRange> rows)
    {
      if (rows != null)
      {
        foreach (DataspaceHashRange row in rows)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(DataspacePartitionComponent.typ_DataspacePartitionMapTable);
          sqlDataRecord.SetInt32(0, row.HashStart);
          sqlDataRecord.SetInt32(1, row.HashEnd);
          sqlDataRecord.SetGuid(2, row.DataspaceIdentifier);
          yield return sqlDataRecord;
        }
      }
    }

    public virtual void SaveDataspacePartitionMap(DataspacePartitionMap partitionMap)
    {
      this.PrepareStoredProcedure("prc_SaveDataspacePartitionMap");
      this.BindString("@dataspaceCategory", partitionMap.Category, 260, false, SqlDbType.NVarChar);
      this.BindDataspacePartitionMapTable("@partitionMap", partitionMap);
      this.ExecuteNonQuery();
    }

    public virtual DataspacePartitionMap GetDataspacePartitionMap(string dataspaceCategory)
    {
      this.PrepareStoredProcedure("prc_GetDataspacePartitionMap");
      this.BindString("@dataspaceCategory", dataspaceCategory, 260, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DataspaceHashRange>((ObjectBinder<DataspaceHashRange>) new DataspacePartitionComponent.DataspaceHashRangeColumns());
      List<DataspaceHashRange> items = resultCollection.GetCurrent<DataspaceHashRange>().Items;
      return items.Count == 0 ? (DataspacePartitionMap) null : new DataspacePartitionMap(dataspaceCategory, items.ToArray(), (DataspacePartitionMapOverride[]) null);
    }

    internal int GetStringStableHashCode(string partitionKey)
    {
      string sqlStatement = "SELECT dbo.func_GetStringStableHashCode(@partitionKey)";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement, 1);
      this.BindString("@partitionKey", partitionKey, 256, false, SqlDbType.VarChar);
      return (int) this.ExecuteScalar();
    }

    internal int GetGuidStableHashCode(Guid partitionKey)
    {
      string sqlStatement = "SELECT dbo.func_GetGuidStableHashCode(@partitionKey)";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement, 1);
      this.BindGuid("@partitionKey", partitionKey);
      return (int) this.ExecuteScalar();
    }

    protected class DataspaceHashRangeColumns : ObjectBinder<DataspaceHashRange>
    {
      private SqlColumnBinder HashStartColumn = new SqlColumnBinder("HashStart");
      private SqlColumnBinder HashEndColumn = new SqlColumnBinder("HashEnd");
      private SqlColumnBinder DataspaceIdentifierColumn = new SqlColumnBinder("DataspaceIdentifier");

      protected override DataspaceHashRange Bind() => new DataspaceHashRange()
      {
        HashStart = this.HashStartColumn.GetInt32((IDataReader) this.Reader),
        HashEnd = this.HashEndColumn.GetInt32((IDataReader) this.Reader),
        DataspaceIdentifier = this.DataspaceIdentifierColumn.GetGuid((IDataReader) this.Reader)
      };
    }
  }
}
