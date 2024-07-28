// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent16
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
  internal class BoardSettingsComponent16 : BoardSettingsComponent15
  {
    public override IList<BoardColumnRevisionForReporting> GetBoardColumnRevisions(int watermark)
    {
      this.PrepareStoredProcedure("prc_GetBoardColumnRevisionsForReporting");
      this.BindInt("@watermark", watermark);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetBoardColumnIncrementalRevisions(rc);
    }

    public override IList<BoardRowRevisionForReporting> GetBoardRowRevisions(int watermark)
    {
      this.PrepareStoredProcedure("prc_GetBoardRowRevisionsForReporting");
      this.BindInt("@watermark", watermark);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetBoardRowIncrementalRevisions(rc);
    }

    protected virtual IList<BoardColumnRevisionForReporting> GetBoardColumnIncrementalRevisions(
      ResultCollection rc)
    {
      rc.AddBinder<BoardColumnRevisionForReportingTable>((ObjectBinder<BoardColumnRevisionForReportingTable>) new BoardColumnRevisionForReportingBinder());
      return (IList<BoardColumnRevisionForReporting>) rc.GetCurrent<BoardColumnRevisionForReportingTable>().Items.Select<BoardColumnRevisionForReportingTable, BoardColumnRevisionForReporting>((System.Func<BoardColumnRevisionForReportingTable, BoardColumnRevisionForReporting>) (bcr =>
      {
        return new BoardColumnRevisionForReporting()
        {
          Id = bcr.Id,
          Name = bcr.Name,
          Order = bcr.Order,
          ItemLimit = bcr.ItemLimit,
          ColumnType = (BoardColumnType) bcr.ColumnType,
          RevisedDate = bcr.RevisedDate,
          ChangedDate = new DateTime?(bcr.ChangedDate),
          IsSplit = bcr.IsSplit,
          Deleted = bcr.Deleted,
          Watermark = bcr.Watermark,
          BoardId = bcr.BoardId,
          BoardExtensionId = bcr.BoardExtensionId,
          BoardCategoryReferenceName = bcr.BoardCategoryReferenceName,
          TeamId = bcr.TeamId
        };
      })).ToList<BoardColumnRevisionForReporting>();
    }

    protected virtual IList<BoardRowRevisionForReporting> GetBoardRowIncrementalRevisions(
      ResultCollection rc)
    {
      rc.AddBinder<BoardRowRevisionForReportingTable>((ObjectBinder<BoardRowRevisionForReportingTable>) new BoardRowRevisionForReportingBinder());
      return (IList<BoardRowRevisionForReporting>) rc.GetCurrent<BoardRowRevisionForReportingTable>().Items.Select<BoardRowRevisionForReportingTable, BoardRowRevisionForReporting>((System.Func<BoardRowRevisionForReportingTable, BoardRowRevisionForReporting>) (brr =>
      {
        return new BoardRowRevisionForReporting()
        {
          Id = brr.Id,
          Name = brr.Name,
          Order = brr.Order,
          RevisedDate = brr.RevisedDate,
          ChangedDate = brr.ChangedDate,
          IsDeleted = brr.Deleted,
          Watermark = brr.Watermark,
          BoardId = brr.BoardId,
          BoardExtensionId = brr.BoardExtensionId,
          BoardCategoryReferenceName = brr.BoardCategoryReferenceName,
          TeamId = brr.TeamId
        };
      })).ToList<BoardRowRevisionForReporting>();
    }
  }
}
