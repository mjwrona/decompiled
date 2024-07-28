// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspacePartitionComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspacePartitionComponent2 : DataspacePartitionComponent
  {
    private static readonly SqlMetaData[] typ_DataspacePartitionMapOverrideTable = new SqlMetaData[2]
    {
      new SqlMetaData("PartitionKey", SqlDbType.VarChar, 256L),
      new SqlMetaData("DataspaceIdentifier", SqlDbType.UniqueIdentifier)
    };

    protected SqlParameter BindDataspacePartitionMapOverrideTable(
      string parameterName,
      DataspacePartitionMap partitionMap)
    {
      return this.BindTable(parameterName, "typ_DataspacePartitionMapOverrideTable", DataspacePartitionComponent2.BindDataspacePartitionMapOverrideRow((IEnumerable<DataspacePartitionMapOverride>) partitionMap.Overrides));
    }

    internal static IEnumerable<SqlDataRecord> BindDataspacePartitionMapOverrideRow(
      IEnumerable<DataspacePartitionMapOverride> rows)
    {
      if (rows != null)
      {
        foreach (DataspacePartitionMapOverride row in rows)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(DataspacePartitionComponent2.typ_DataspacePartitionMapOverrideTable);
          sqlDataRecord.SetString(0, row.PartitionKey);
          sqlDataRecord.SetGuid(1, row.DataspaceIdentifier);
          yield return sqlDataRecord;
        }
      }
    }

    public override void SaveDataspacePartitionMap(DataspacePartitionMap partitionMap)
    {
      this.PrepareStoredProcedure("prc_SaveDataspacePartitionMap");
      this.BindString("@dataspaceCategory", partitionMap.Category, 260, false, SqlDbType.NVarChar);
      this.BindDataspacePartitionMapTable("@partitionMap", partitionMap);
      this.BindDataspacePartitionMapOverrideTable("@overrides", partitionMap);
      this.ExecuteNonQuery();
    }

    public override DataspacePartitionMap GetDataspacePartitionMap(string dataspaceCategory)
    {
      this.PrepareStoredProcedure("prc_GetDataspacePartitionMap");
      this.BindString("@dataspaceCategory", dataspaceCategory, 260, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DataspaceHashRange>((ObjectBinder<DataspaceHashRange>) new DataspacePartitionComponent.DataspaceHashRangeColumns());
      resultCollection.AddBinder<DataspacePartitionMapOverride>((ObjectBinder<DataspacePartitionMapOverride>) new DataspacePartitionComponent2.DataspaceMapOverridesColumns());
      List<DataspaceHashRange> items1 = resultCollection.GetCurrent<DataspaceHashRange>().Items;
      if (items1.Count == 0)
        return (DataspacePartitionMap) null;
      resultCollection.NextResult();
      List<DataspacePartitionMapOverride> items2 = resultCollection.GetCurrent<DataspacePartitionMapOverride>().Items;
      return new DataspacePartitionMap(dataspaceCategory, items1.ToArray(), items2.ToArray());
    }

    protected class DataspaceMapOverridesColumns : ObjectBinder<DataspacePartitionMapOverride>
    {
      private SqlColumnBinder PartitionKeyColumn = new SqlColumnBinder("PartitionKey");
      private SqlColumnBinder DataspaceIdentifierColumn = new SqlColumnBinder("DataspaceIdentifier");

      protected override DataspacePartitionMapOverride Bind() => new DataspacePartitionMapOverride(this.PartitionKeyColumn.GetString((IDataReader) this.Reader, false), this.DataspaceIdentifierColumn.GetGuid((IDataReader) this.Reader));
    }
  }
}
