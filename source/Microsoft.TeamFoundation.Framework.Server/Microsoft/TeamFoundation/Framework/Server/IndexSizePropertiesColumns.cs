// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IndexSizePropertiesColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class IndexSizePropertiesColumns : ObjectBinder<IndexSizeProperties>
  {
    private SqlColumnBinder schemaColumn = new SqlColumnBinder("Schema");
    private SqlColumnBinder tableColumn = new SqlColumnBinder("Table");
    private SqlColumnBinder indexColumn = new SqlColumnBinder("Index");
    private SqlColumnBinder indexIdColumn = new SqlColumnBinder("IndexId");
    private SqlColumnBinder reservedColumn = new SqlColumnBinder("Reserved");
    private SqlColumnBinder usedColumn = new SqlColumnBinder("Used");

    protected override IndexSizeProperties Bind() => new IndexSizeProperties()
    {
      Schema = this.schemaColumn.GetString((IDataReader) this.Reader, false),
      Table = this.tableColumn.GetString((IDataReader) this.Reader, false),
      Index = this.indexColumn.GetString((IDataReader) this.Reader, true),
      IndexId = this.indexIdColumn.GetInt16((IDataReader) this.Reader, (short) 0),
      ReservedBytes = this.reservedColumn.GetInt64((IDataReader) this.Reader, 0L),
      UsedBytes = this.usedColumn.GetInt64((IDataReader) this.Reader, 0L)
    };
  }
}
