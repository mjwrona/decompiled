// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspaceColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspaceColumns : ObjectBinder<Dataspace>
  {
    private SqlColumnBinder dataspaceIdentifierColumn = new SqlColumnBinder("DataspaceIdentifier");
    private SqlColumnBinder dataspaceCategoryColumn = new SqlColumnBinder("DataspaceCategory");
    private SqlColumnBinder databaseIdColumn = new SqlColumnBinder("DatabaseId");
    private SqlColumnBinder dataspaceIdColumn = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder dataspaceStateColumn = new SqlColumnBinder("State");

    protected override Dataspace Bind() => new Dataspace()
    {
      DataspaceIdentifier = this.dataspaceIdentifierColumn.GetGuid((IDataReader) this.Reader),
      DataspaceCategory = this.dataspaceCategoryColumn.GetString((IDataReader) this.Reader, false),
      DatabaseId = this.databaseIdColumn.GetInt32((IDataReader) this.Reader),
      DataspaceId = this.dataspaceIdColumn.GetInt32((IDataReader) this.Reader),
      State = (DataspaceState) this.dataspaceStateColumn.GetByte((IDataReader) this.Reader)
    };
  }
}
