// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent6
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent6 : BoardSettingsComponent5
  {
    private static SqlMetaData[] typ_BoardRowTable = new SqlMetaData[6]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BoardId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("RevisedDate", SqlDbType.DateTime),
      new SqlMetaData("Deleted", SqlDbType.Bit)
    };

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

    public override void UpdateBoard(Guid projectId, BoardSettings board)
    {
    }

    public override void UpdateBoardColumns(
      Guid projectId,
      Guid boardId,
      IEnumerable<BoardColumn> columns)
    {
      ArgumentUtility.CheckForEmptyGuid(boardId, "board");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) columns, nameof (columns));
      this.PrepareStoredProcedure("prc_SetBoardColumns");
      this.BindDataspace(projectId);
      this.BindGuid("@boardId", boardId);
      this.BindBoardColumnRowTable("@boardColumnTable", this.GetBoardColumnRows(columns));
      this.ExecuteNonQuery();
    }

    public override void UpdateBoardRows(Guid projectId, Guid boardId, IEnumerable<BoardRow> rows)
    {
      ArgumentUtility.CheckForEmptyGuid(boardId, "board");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) rows, nameof (rows));
      this.PrepareStoredProcedure("prc_SetBoardRows");
      this.BindDataspace(projectId);
      this.BindGuid("@boardId", boardId);
      this.BindBoardRowRowTable("@boardRowTable", this.GetBoardRowRows(rows));
      this.ExecuteNonQuery();
    }

    protected override void AddBoardSettingsBinder(ResultCollection rc) => rc.AddBinder<BoardSettingsDTO>((ObjectBinder<BoardSettingsDTO>) new BoardSettingsDTOBinder2());

    protected override BoardSettingsDTO GetBoardSettings(ResultCollection rc)
    {
      BoardSettingsDTO boardSettings = base.GetBoardSettings(rc);
      if (boardSettings != null)
      {
        rc.NextResult();
        this.AddBoardRowRowBinder(rc);
        boardSettings.Rows = (IEnumerable<BoardRowTable>) rc.GetCurrent<BoardRowTable>().Items.OrderBy<BoardRowTable, int>((System.Func<BoardRowTable, int>) (r => r.Order));
      }
      return boardSettings;
    }

    private void BindBoardRowRowTable(string parameterName, IEnumerable<BoardRowTable> rows) => this.BindTable(parameterName, "typ_BoardRowTable", rows.Select<BoardRowTable, SqlDataRecord>((System.Func<BoardRowTable, SqlDataRecord>) (row =>
    {
      SqlDataRecord sqlDataRecord1 = new SqlDataRecord(BoardSettingsComponent6.typ_BoardRowTable);
      Guid? nullable;
      if (row.Id.HasValue)
      {
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        nullable = row.Id;
        Guid guid = nullable.Value;
        sqlDataRecord2.SetGuid(0, guid);
      }
      else
        sqlDataRecord1.SetDBNull(0);
      nullable = row.BoardId;
      if (nullable.HasValue)
      {
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        nullable = row.BoardId;
        Guid guid = nullable.Value;
        sqlDataRecord3.SetGuid(1, guid);
      }
      else
        sqlDataRecord1.SetDBNull(1);
      if (!string.IsNullOrEmpty(row.Name))
        sqlDataRecord1.SetString(2, row.Name);
      else
        sqlDataRecord1.SetDBNull(2);
      sqlDataRecord1.SetInt32(3, row.Order);
      DateTime? revisedDate = row.RevisedDate;
      if (revisedDate.HasValue)
      {
        SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
        revisedDate = row.RevisedDate;
        DateTime dateTime = revisedDate.Value;
        sqlDataRecord4.SetDateTime(4, dateTime);
      }
      else
        sqlDataRecord1.SetDBNull(4);
      sqlDataRecord1.SetValue(5, (object) row.Deleted);
      return sqlDataRecord1;
    })));

    private IEnumerable<BoardRowTable> GetBoardRowRows(IEnumerable<BoardRow> rows) => rows.Select<BoardRow, BoardRowTable>((System.Func<BoardRow, BoardRowTable>) (r => new BoardRowTable()
    {
      Id = new Guid?(r.Id.HasValue ? r.Id.Value : Guid.NewGuid()),
      Name = r.Name,
      Order = r.Order,
      RevisedDate = new DateTime?(),
      Deleted = false
    }));
  }
}
