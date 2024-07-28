// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent11
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent11 : BoardSettingsComponent10
  {
    public override BoardInput GetBoardInput(Guid projectId, Guid boardId)
    {
      this.PrepareStoredProcedure("prc_GetBoardById");
      this.BindDataspace(projectId);
      this.BindGuid("@boardId", boardId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BoardRecord>((ObjectBinder<BoardRecord>) new BoardRecordBinder());
        List<BoardRecord> items = resultCollection.GetCurrent<BoardRecord>().Items;
        return new BoardInput()
        {
          TeamId = items[0].TeamId,
          BacklogLevelId = items[0].BacklogLevelId
        };
      }
    }
  }
}
