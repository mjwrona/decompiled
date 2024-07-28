// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.GridColumnRowBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class GridColumnRowBinder : ObjectBinder<GridColumnRow>
  {
    private SqlColumnBinder GridColumnTypeColumn = new SqlColumnBinder("GridColumnType");
    private SqlColumnBinder FieldRefNameColumn = new SqlColumnBinder("FieldRefName");
    private SqlColumnBinder DisplayWidthColumn = new SqlColumnBinder("DisplayWidth");
    private SqlColumnBinder OrderColumn = new SqlColumnBinder("Order");

    protected override GridColumnRow Bind() => new GridColumnRow()
    {
      GridColumnType = (GridColumnTypeEnum) this.GridColumnTypeColumn.GetByte((IDataReader) this.Reader),
      FieldRefName = this.FieldRefNameColumn.GetString((IDataReader) this.Reader, false),
      DisplayWidth = this.DisplayWidthColumn.GetInt32((IDataReader) this.Reader, 0),
      Order = this.OrderColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
