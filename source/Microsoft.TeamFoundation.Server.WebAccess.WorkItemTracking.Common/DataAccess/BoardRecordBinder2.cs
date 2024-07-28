// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardRecordBinder2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardRecordBinder2 : BoardRecordBinder
  {
    private SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");
    private BoardSettingsComponent m_boardSettingComponent;

    public BoardRecordBinder2(BoardSettingsComponent boardSettingsComponent) => this.m_boardSettingComponent = boardSettingsComponent;

    protected override BoardRecord Bind()
    {
      BoardRecord boardRecord = base.Bind();
      boardRecord.ProjectId = this.m_boardSettingComponent.GetDataspaceIdentifier(this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader));
      return boardRecord;
    }
  }
}
