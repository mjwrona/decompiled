// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TableNameBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TableNameBinder : ObjectBinder<TableName>
  {
    private SqlColumnBinder m_schemaNameColumn = new SqlColumnBinder("SchemaName");
    private SqlColumnBinder m_tableNameColumn = new SqlColumnBinder("TableName");

    protected override TableName Bind() => new TableName(this.m_schemaNameColumn.GetString((IDataReader) this.Reader, false), this.m_tableNameColumn.GetString((IDataReader) this.Reader, false));
  }
}
