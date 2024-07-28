// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent19
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
  internal class BoardSettingsComponent19 : BoardSettingsComponent18
  {
    private static SqlMetaData[] typ_BoardRowTable = new SqlMetaData[7]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BoardId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("RevisedDate", SqlDbType.DateTime),
      new SqlMetaData("Deleted", SqlDbType.Bit),
      new SqlMetaData("Color", SqlDbType.NVarChar, 10L)
    };

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

    protected override void AddBoardRowRowBinder(ResultCollection rc) => rc.AddBinder<BoardRowTable>((ObjectBinder<BoardRowTable>) new BoardRowRowBinder2());

    private void BindBoardRowRowTable(string parameterName, IEnumerable<BoardRowTable> rows) => this.BindTable(parameterName, "typ_BoardRowTable2", rows.Select<BoardRowTable, SqlDataRecord>((System.Func<BoardRowTable, SqlDataRecord>) (row =>
    {
      SqlDataRecord sqlDataRecord1 = new SqlDataRecord(BoardSettingsComponent19.typ_BoardRowTable);
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
      if (!string.IsNullOrEmpty(row.Color))
        sqlDataRecord1.SetString(6, row.Color);
      else
        sqlDataRecord1.SetDBNull(6);
      return sqlDataRecord1;
    })));

    private IEnumerable<BoardRowTable> GetBoardRowRows(IEnumerable<BoardRow> rows) => rows.Select<BoardRow, BoardRowTable>((System.Func<BoardRow, BoardRowTable>) (r => new BoardRowTable()
    {
      Id = new Guid?(r.Id.HasValue ? r.Id.Value : Guid.NewGuid()),
      Name = r.Name,
      Order = r.Order,
      RevisedDate = new DateTime?(),
      Deleted = false,
      Color = r.Color
    }));
  }
}
