// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostMoveStatusColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostMoveStatusColumns : ObjectBinder<HostMoveStatus>
  {
    private SqlColumnBinder schemaNameColumn = new SqlColumnBinder("SchemaName");
    private SqlColumnBinder tableNameColumn = new SqlColumnBinder("TableName");
    private SqlColumnBinder isCompleteColumn = new SqlColumnBinder("IsComplete");

    protected override HostMoveStatus Bind() => new HostMoveStatus()
    {
      Schema = this.schemaNameColumn.GetString((IDataReader) this.Reader, true),
      Table = this.tableNameColumn.GetString((IDataReader) this.Reader, true),
      IsComplete = this.isCompleteColumn.GetBoolean((IDataReader) this.Reader)
    };
  }
}
