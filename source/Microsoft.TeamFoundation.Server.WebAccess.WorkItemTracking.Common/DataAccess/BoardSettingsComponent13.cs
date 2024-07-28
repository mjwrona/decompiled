// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent13
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
  internal class BoardSettingsComponent13 : BoardSettingsComponent12
  {
    public override IEnumerable<BoardRecord> GetAllBoards(Guid? projectId = null, Guid? teamId = null)
    {
      this.PrepareStoredProcedure("prc_GetAllBoards");
      if (projectId.HasValue)
        this.BindDataspace(projectId.Value);
      if (teamId.HasValue)
        this.BindGuid("@teamId", teamId.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BoardRecord>((ObjectBinder<BoardRecord>) this.GetBoardRecordBinder());
        return (IEnumerable<BoardRecord>) resultCollection.GetCurrent<BoardRecord>().Items;
      }
    }

    protected override BoardRecordBinder GetBoardRecordBinder() => (BoardRecordBinder) new BoardRecordBinder2((BoardSettingsComponent) this);

    public override IEnumerable<BoardRowRevision> GetBoardRowRevisions(
      Guid projectId,
      Guid teamId,
      string backlogLevelId)
    {
      this.PrepareStoredProcedure("prc_GetBoardRowRevisions");
      this.BindDataspace(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindString("@categoryReferenceName", backlogLevelId, 256, false, SqlDbType.NVarChar);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return (IEnumerable<BoardRowRevision>) this.GetBoardRowRevisions(rc);
    }

    protected virtual IList<BoardRowRevision> GetBoardRowRevisions(ResultCollection rc)
    {
      rc.AddBinder<BoardRowTable>((ObjectBinder<BoardRowTable>) new BoardRowRowBinder());
      return (IList<BoardRowRevision>) rc.GetCurrent<BoardRowTable>().Items.Select<BoardRowTable, BoardRowRevision>((System.Func<BoardRowTable, BoardRowRevision>) (bcr =>
      {
        return new BoardRowRevision()
        {
          Id = bcr.Id,
          Name = bcr.Name,
          Order = bcr.Order,
          RevisedDate = bcr.RevisedDate,
          IsDeleted = bcr.Deleted
        };
      })).ToList<BoardRowRevision>();
    }
  }
}
