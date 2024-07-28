// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.TableSizeDataspaceInfoColumns
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Configuration
{
  internal class TableSizeDataspaceInfoColumns : ObjectBinder<TableSizeDataspaceInfoProperties>
  {
    private SqlColumnBinder SchemaNameColumn = new SqlColumnBinder("SchemaName");
    private SqlColumnBinder TableNameColumn = new SqlColumnBinder("TableName");
    private SqlColumnBinder SizeColumn = new SqlColumnBinder("SizePages");
    private SqlColumnBinder DataspaceColumnCountColumn = new SqlColumnBinder("DataspaceColumnCount");

    protected override TableSizeDataspaceInfoProperties Bind() => new TableSizeDataspaceInfoProperties()
    {
      Schema = this.SchemaNameColumn.GetString((IDataReader) this.Reader, false),
      TableName = this.TableNameColumn.GetString((IDataReader) this.Reader, false),
      SizePages = this.SizeColumn.GetInt64((IDataReader) this.Reader),
      DataspaceColumnCount = this.DataspaceColumnCountColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
