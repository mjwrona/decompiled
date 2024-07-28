// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent12
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent12 : BoardSettingsComponent11
  {
    protected override BoardSettingsDTO GetBoardSettings(ResultCollection rc)
    {
      BoardSettingsDTO boardSettings = base.GetBoardSettings(rc);
      if (boardSettings != null && rc.TryNextResult())
      {
        rc.AddBinder<BoardOptionRecord>((ObjectBinder<BoardOptionRecord>) new BoardOptionRecordBinder());
        boardSettings.Options = rc.GetCurrent<BoardOptionRecord>().Items.FirstOrDefault<BoardOptionRecord>();
      }
      return boardSettings;
    }

    public override void UpdateBoardOptions(
      Guid projectId,
      Guid boardId,
      int cardReordering,
      bool statusBadgeIsPublic = false)
    {
      this.PrepareStoredProcedure("prc_SetBoardOptions");
      this.BindDataspace(projectId);
      this.BindGuid("@boardId", boardId);
      this.BindInt("@cardReordering", cardReordering);
      this.ExecuteNonQuery();
    }

    public override Dictionary<string, string> GetBoardOptions(Guid projectId, Guid boardId)
    {
      Dictionary<string, string> boardOptions = new Dictionary<string, string>();
      BoardOptionRecord boardOptionsRecord = this.GetBoardOptionsRecord(projectId, boardId);
      boardOptions["cardReordering"] = boardOptionsRecord.CardReordering.ToString();
      boardOptions["statusBadgeIsPublic"] = boardOptionsRecord.StatusBadgeIsPublic.ToString();
      return boardOptions;
    }

    public override BoardOptionRecord GetBoardOptionsRecord(Guid projectId, Guid boardId)
    {
      this.PrepareStoredProcedure("prc_GetBoardOptions");
      this.BindDataspace(projectId);
      this.BindGuid("@boardId", boardId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BoardOptionRecord>((ObjectBinder<BoardOptionRecord>) new BoardOptionRecordBinder());
        return resultCollection.GetCurrent<BoardOptionRecord>().Items.FirstOrDefault<BoardOptionRecord>();
      }
    }
  }
}
