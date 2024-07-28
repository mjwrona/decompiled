// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionTableUsageBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartitionTableUsageBinder : 
    ObjectBinder<TeamFoundationDatabasePartitionTableUsage>
  {
    private SqlColumnBinder m_tableNameColumn = new SqlColumnBinder("TableName");
    private SqlColumnBinder m_schemaNameColumn = new SqlColumnBinder("SchemaName");
    private SqlColumnBinder m_rowCountColumn = new SqlColumnBinder("DataRowCount");
    private SqlColumnBinder m_reservedSpaceMBColumn = new SqlColumnBinder("ReservedSpaceMB");
    private SqlColumnBinder m_usedSpaceMBColumn = new SqlColumnBinder("UsedSpaceMB");

    protected override TeamFoundationDatabasePartitionTableUsage Bind() => new TeamFoundationDatabasePartitionTableUsage()
    {
      TableName = this.m_tableNameColumn.GetString((IDataReader) this.Reader, false),
      SchemaName = this.m_schemaNameColumn.GetString((IDataReader) this.Reader, false),
      RowCount = this.m_rowCountColumn.GetInt64((IDataReader) this.Reader),
      ReservedSpaceMB = this.m_reservedSpaceMBColumn.GetDouble((IDataReader) this.Reader),
      UsedSpaceMB = this.m_usedSpaceMBColumn.GetDouble((IDataReader) this.Reader)
    };
  }
}
