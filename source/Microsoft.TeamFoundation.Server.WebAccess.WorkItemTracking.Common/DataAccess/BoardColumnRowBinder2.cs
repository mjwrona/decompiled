// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardColumnRowBinder2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardColumnRowBinder2 : BoardColumnRowBinder
  {
    protected SqlColumnBinder IsSplitColumn = new SqlColumnBinder("IsSplit");
    protected SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");

    protected override BoardColumnRow Bind() => new BoardColumnRow()
    {
      Id = new Guid?(this.IdColumn.GetGuid((IDataReader) this.Reader)),
      BoardId = new Guid?(this.BoardIdColumn.GetGuid((IDataReader) this.Reader)),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
      Order = this.OrderColumn.GetInt32((IDataReader) this.Reader),
      ItemLimit = this.ItemLimitColumn.GetInt32((IDataReader) this.Reader, 0),
      ColumnType = (int) this.ColumnTypeColumn.GetByte((IDataReader) this.Reader),
      RevisedDate = new DateTime?(this.RevisedDateColumn.GetDateTime((IDataReader) this.Reader)),
      Deleted = this.DeletedColumn.GetBoolean((IDataReader) this.Reader),
      IsSplit = this.IsSplitColumn.GetBoolean((IDataReader) this.Reader),
      Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
