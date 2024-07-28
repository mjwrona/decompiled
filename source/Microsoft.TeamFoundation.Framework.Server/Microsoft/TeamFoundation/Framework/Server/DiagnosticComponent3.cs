// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent3 : DiagnosticComponent2
  {
    public override List<TableSpaceUsage> QueryTableSpaceUsage()
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryTableSpaceUsage");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TableSpaceUsage>((ObjectBinder<TableSpaceUsage>) new DiagnosticComponent3.TableSpaceUsageBinder());
        return resultCollection.GetCurrent<TableSpaceUsage>().Items;
      }
    }

    protected class TableSpaceUsageBinder : ObjectBinder<TableSpaceUsage>
    {
      private SqlColumnBinder m_schemaNameColumn = new SqlColumnBinder("SchemaName");
      private SqlColumnBinder m_tableNameColumn = new SqlColumnBinder("TableName");
      private SqlColumnBinder m_indexNameColumn = new SqlColumnBinder("IndexName");
      private SqlColumnBinder m_indexTypeColumn = new SqlColumnBinder("IndexType");
      private SqlColumnBinder m_indexIdColumn = new SqlColumnBinder("IndexId");
      private SqlColumnBinder m_compressionColumn = new SqlColumnBinder("Compression");
      private SqlColumnBinder m_rowCountColumn = new SqlColumnBinder("RowCount");
      private SqlColumnBinder m_reservedPageCount = new SqlColumnBinder("ReservedPageCount");
      private SqlColumnBinder m_usedPageCount = new SqlColumnBinder("UsedPageCount");
      private SqlColumnBinder m_inRowReservedPageCount = new SqlColumnBinder("InRowReservedPageCount");
      private SqlColumnBinder m_inRowUsedPageCount = new SqlColumnBinder("InRowUsedPageCount");
      private SqlColumnBinder m_lobReservedPageCount = new SqlColumnBinder("LobReservedPageCount");
      private SqlColumnBinder m_lobUsedPageCount = new SqlColumnBinder("LobUsedPageCount");
      private SqlColumnBinder m_partitionNumber = new SqlColumnBinder("PartitionNumber");
      private SqlColumnBinder m_partitionBoundary = new SqlColumnBinder("PartitionBoundary");

      protected override TableSpaceUsage Bind() => new TableSpaceUsage()
      {
        SchemaName = string.Intern(this.m_schemaNameColumn.GetString((IDataReader) this.Reader, false)),
        TableName = this.m_tableNameColumn.GetString((IDataReader) this.Reader, false),
        IndexName = this.m_indexNameColumn.GetString((IDataReader) this.Reader, true),
        IndexType = this.m_indexTypeColumn.GetString((IDataReader) this.Reader, false),
        IndexId = this.m_indexIdColumn.GetInt32((IDataReader) this.Reader),
        Compression = string.Intern(this.m_compressionColumn.GetString((IDataReader) this.Reader, false)),
        RowCount = this.m_rowCountColumn.GetInt64((IDataReader) this.Reader),
        ReservedPageCount = this.m_reservedPageCount.GetInt64((IDataReader) this.Reader),
        UsedPageCount = this.m_usedPageCount.GetInt64((IDataReader) this.Reader),
        InRowReservedPageCount = this.m_inRowReservedPageCount.GetInt64((IDataReader) this.Reader),
        InRowUsedPageCount = this.m_inRowUsedPageCount.GetInt64((IDataReader) this.Reader),
        LobReservedPageCount = this.m_lobReservedPageCount.GetInt64((IDataReader) this.Reader),
        LobUsedPageCount = this.m_lobUsedPageCount.GetInt64((IDataReader) this.Reader),
        PartitionNumber = this.m_partitionNumber.GetInt32((IDataReader) this.Reader, 0, 0),
        PartitionBoundary = this.m_partitionBoundary.GetString((IDataReader) this.Reader, true, "NA")
      };
    }
  }
}
