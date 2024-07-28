// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TableSizeColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TableSizeColumns : ObjectBinder<TableSizeProperties>
  {
    private SqlColumnBinder TableNameColumn = new SqlColumnBinder("TableName");
    private SqlColumnBinder SchemaNameColumn = new SqlColumnBinder("SchemaName");
    private SqlColumnBinder RowCountColumn = new SqlColumnBinder("Row_Count");
    private SqlColumnBinder SizeInBytesColumn = new SqlColumnBinder("SizeInBytes");

    protected override TableSizeProperties Bind()
    {
      string str1 = this.TableNameColumn.GetString((IDataReader) this.Reader, false);
      string str2 = this.SchemaNameColumn.GetString((IDataReader) this.Reader, false);
      long int64_1 = this.RowCountColumn.GetInt64((IDataReader) this.Reader);
      long int64_2 = this.SizeInBytesColumn.GetInt64((IDataReader) this.Reader);
      return new TableSizeProperties()
      {
        Name = str1,
        Schema = str2,
        RowCount = int64_1,
        Size = int64_2
      };
    }
  }
}
