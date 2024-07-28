// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryComponent8
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RegistryComponent8 : RegistryComponent7
  {
    private static readonly SqlMetaData[] typ_RegistryQueryTable = new SqlMetaData[3]
    {
      new SqlMetaData("QueryId", SqlDbType.Int, true, false, System.Data.SqlClient.SortOrder.Unspecified, -1),
      new SqlMetaData("RegistryPath", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Depth", SqlDbType.TinyInt)
    };

    public override IEnumerable<RegistryComponent.RegistryItemWithIndex> QueryRegistry(
      RegistryComponent.RegistryComponentQuery[] componentQueries,
      out long sequenceId)
    {
      this.PrepareStoredProcedure("prc_QueryRegistryMultiple");
      System.Func<RegistryComponent.RegistryComponentQuery, SqlDataRecord> selector = (System.Func<RegistryComponent.RegistryComponentQuery, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(RegistryComponent8.typ_RegistryQueryTable);
        sqlDataRecord.SetString(1, RegistryComponent.RegistryToDatabasePath(row.Path));
        sqlDataRecord.SetByte(2, RegistryComponent4.GetSqlDepth(row.Depth));
        return sqlDataRecord;
      });
      this.BindTable("@registryQueries", "typ_RegistryQueryTable", ((IEnumerable<RegistryComponent.RegistryComponentQuery>) componentQueries).Select<RegistryComponent.RegistryComponentQuery, SqlDataRecord>(selector));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<long>((ObjectBinder<long>) new RegistryComponent5.SequenceIdColumns());
      resultCollection.AddBinder<RegistryComponent.RegistryItemWithIndex>((ObjectBinder<RegistryComponent.RegistryItemWithIndex>) new RegistryComponent8.RegistryItemWithIndexColumns());
      resultCollection.AddBinder<RegistryComponent.RegistryItemWithIndex>((ObjectBinder<RegistryComponent.RegistryItemWithIndex>) new RegistryComponent8.RegistryItemWithIndexColumns());
      sequenceId = resultCollection.GetCurrent<long>().FirstOrDefault<long>();
      resultCollection.NextResult();
      List<RegistryComponent.RegistryItemWithIndex> items = resultCollection.GetCurrent<RegistryComponent.RegistryItemWithIndex>().Items;
      resultCollection.NextResult();
      ObjectBinder<RegistryComponent.RegistryItemWithIndex> current = resultCollection.GetCurrent<RegistryComponent.RegistryItemWithIndex>();
      return items.Merge<RegistryComponent.RegistryItemWithIndex>((IEnumerable<RegistryComponent.RegistryItemWithIndex>) current, (Func<RegistryComponent.RegistryItemWithIndex, RegistryComponent.RegistryItemWithIndex, int>) ((a, b) => a.QueryIndex - b.QueryIndex));
    }

    protected class RegistryItemWithIndexColumns : 
      ObjectBinder<RegistryComponent.RegistryItemWithIndex>
    {
      private SqlColumnBinder m_queryIdColumn = new SqlColumnBinder("QueryId");
      private SqlColumnBinder m_registryPathColumn = new SqlColumnBinder("RegistryPath");
      private SqlColumnBinder m_registryValueColumn = new SqlColumnBinder("RegValue");

      protected override RegistryComponent.RegistryItemWithIndex Bind() => new RegistryComponent.RegistryItemWithIndex(this.m_queryIdColumn.GetInt32((IDataReader) this.Reader), new RegistryItem(RegistryComponent.DatabaseToRegistryPath(this.m_registryPathColumn.GetString((IDataReader) this.Reader, false)), this.m_registryValueColumn.GetString((IDataReader) this.Reader, false)));
    }
  }
}
