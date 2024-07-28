// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent2 : BoardSettingsComponent
  {
    protected const int c_MaxCategoryReferenceNameLength = 256;

    public override BoardSettingsDTO CreateBoard(Guid projectId, BoardSettings board)
    {
      ArgumentUtility.CheckForNull<BoardSettings>(board, nameof (board));
      this.PrepareStoredProcedure("prc_CreateBoard");
      this.BindDataspace(projectId);
      this.BindGuid("@teamId", board.TeamId);
      this.BindString("@categoryReferenceName", board.BacklogLevelId, 256, false, SqlDbType.NVarChar);
      if (board.ExtensionId.HasValue)
        this.BindGuid("@extensionId", board.ExtensionId.Value);
      else
        this.BindNullValue("@extensionId", SqlDbType.UniqueIdentifier);
      this.BindBoardColumnRowTable("@boardColumnTable", this.GetBoardColumnRows(board.Columns));
      board.Id = new Guid?((Guid) this.ExecuteScalar());
      return this.GetBoard(projectId, board.TeamId, board.BacklogLevelId);
    }

    public override BoardSettingsDTO GetBoard(Guid projectId, Guid teamId, string backlogLevelId)
    {
      this.PrepareStoredProcedure("prc_GetBoard");
      this.BindDataspace(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindString("@categoryReferenceName", backlogLevelId, 256, false, SqlDbType.NVarChar);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetBoardSettings(rc);
    }

    public override IEnumerable<BoardColumnRevision> GetBoardRevisions(
      Guid projectId,
      Guid teamId,
      string backlogLevelId)
    {
      this.PrepareStoredProcedure("prc_GetBoardColumnRevisions");
      this.BindDataspace(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindString("@categoryReferenceName", backlogLevelId, 256, false, SqlDbType.NVarChar);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return (IEnumerable<BoardColumnRevision>) this.GetBoardColumnRevisions(rc);
    }

    public override void DeleteBoards(IEnumerable<Guid> boardIds)
    {
      this.PrepareStoredProcedure("prc_DeleteBoards");
      this.BindGuidTable("@boardIds", boardIds);
      this.ExecuteNonQuery();
    }

    public override List<BoardRecord> GetTeamBoards(IEnumerable<Guid> teamIds)
    {
      this.PrepareStoredProcedure("prc_GetBoards");
      this.BindGuidTable("@teamIds", teamIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BoardRecord>((ObjectBinder<BoardRecord>) this.GetBoardRecordBinder());
        return resultCollection.GetCurrent<BoardRecord>().Items;
      }
    }

    protected virtual BoardRecordBinder GetBoardRecordBinder() => new BoardRecordBinder();
  }
}
