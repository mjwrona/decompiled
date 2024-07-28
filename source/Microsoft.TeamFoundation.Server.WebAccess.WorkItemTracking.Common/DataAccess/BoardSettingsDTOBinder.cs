// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsDTOBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsDTOBinder : ObjectBinder<BoardSettingsDTO>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder TeamIdColumn = new SqlColumnBinder("TeamId");
    private SqlColumnBinder ExtensionIdColumn = new SqlColumnBinder("ExtensionId");
    private SqlColumnBinder ExtensionLastChangeDateColumn = new SqlColumnBinder("ExtensionLastChangeDate");
    private SqlColumnBinder CategoryReferenceNameColumn = new SqlColumnBinder("CategoryReferenceName");

    protected override BoardSettingsDTO Bind()
    {
      BoardSettingsDTO boardSettingsDto = new BoardSettingsDTO();
      boardSettingsDto.Id = this.IdColumn.GetGuid((IDataReader) this.Reader);
      boardSettingsDto.TeamId = this.TeamIdColumn.GetGuid((IDataReader) this.Reader);
      boardSettingsDto.ExtensionId = this.ExtensionIdColumn.GetGuid((IDataReader) this.Reader, true);
      boardSettingsDto.ExtensionLastChangedDate = DateTime.MinValue;
      if (this.ExtensionLastChangeDateColumn.ColumnExists((IDataReader) this.Reader))
        boardSettingsDto.ExtensionLastChangedDate = this.ExtensionLastChangeDateColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      boardSettingsDto.BacklogLevelId = string.Empty;
      if (this.CategoryReferenceNameColumn.ColumnExists((IDataReader) this.Reader))
        boardSettingsDto.BacklogLevelId = this.CategoryReferenceNameColumn.GetString((IDataReader) this.Reader, string.Empty);
      return boardSettingsDto;
    }
  }
}
