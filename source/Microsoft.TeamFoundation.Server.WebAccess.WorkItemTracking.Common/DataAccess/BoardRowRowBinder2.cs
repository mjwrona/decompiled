// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardRowRowBinder2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardRowRowBinder2 : BoardRowRowBinder
  {
    protected SqlColumnBinder ColorColumn = new SqlColumnBinder("Color");

    protected override BoardRowTable Bind() => new BoardRowTable()
    {
      Id = new Guid?(this.IdColumn.GetGuid((IDataReader) this.Reader)),
      BoardId = new Guid?(this.BoardIdColumn.GetGuid((IDataReader) this.Reader)),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, true),
      Order = this.OrderColumn.GetInt32((IDataReader) this.Reader),
      RevisedDate = new DateTime?(this.RevisedDateColumn.GetDateTime((IDataReader) this.Reader)),
      Deleted = this.DeletedColumn.GetBoolean((IDataReader) this.Reader),
      Color = this.ColorColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
