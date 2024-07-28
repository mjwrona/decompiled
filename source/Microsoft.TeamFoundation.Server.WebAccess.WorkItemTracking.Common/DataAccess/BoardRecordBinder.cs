// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardRecordBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardRecordBinder : ObjectBinder<BoardRecord>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder TeamIdColumn = new SqlColumnBinder("TeamId");
    private SqlColumnBinder ExtensionIdColumn = new SqlColumnBinder("ExtensionId");
    private SqlColumnBinder CategoryReferenceNameColumn = new SqlColumnBinder("CategoryReferenceName");

    protected override BoardRecord Bind() => new BoardRecord()
    {
      Id = this.IdColumn.GetGuid((IDataReader) this.Reader),
      TeamId = this.TeamIdColumn.GetGuid((IDataReader) this.Reader),
      WorkItemTypeExtensionId = this.ExtensionIdColumn.GetGuid((IDataReader) this.Reader, true),
      BacklogLevelId = this.CategoryReferenceNameColumn.GetString((IDataReader) this.Reader, string.Empty)
    };
  }
}
