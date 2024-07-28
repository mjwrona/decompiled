// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent4
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent4 : BoardSettingsComponent3
  {
    private static SqlMetaData[] typ_BoardColumnTable2 = new SqlMetaData[10]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BoardId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("ItemLimit", SqlDbType.Int),
      new SqlMetaData("ColumnType", SqlDbType.Int),
      new SqlMetaData("RevisedDate", SqlDbType.DateTime),
      new SqlMetaData("Deleted", SqlDbType.Bit),
      new SqlMetaData("IsSplit", SqlDbType.Bit),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2000L)
    };

    protected override IList<BoardColumnRevision> GetBoardColumnRevisions(ResultCollection rc)
    {
      rc.AddBinder<BoardColumnRow>((ObjectBinder<BoardColumnRow>) new BoardColumnRevisionRowBinder());
      return (IList<BoardColumnRevision>) rc.GetCurrent<BoardColumnRow>().Items.Select<BoardColumnRow, BoardColumnRevision>((System.Func<BoardColumnRow, BoardColumnRevision>) (bcr => new BoardColumnRevision()
      {
        Id = bcr.Id,
        Name = bcr.Name,
        Order = bcr.Order,
        ItemLimit = bcr.ItemLimit,
        ColumnType = (BoardColumnType) bcr.ColumnType,
        RevisedDate = bcr.RevisedDate,
        Deleted = bcr.Deleted,
        IsSplit = bcr.IsSplit
      })).ToList<BoardColumnRevision>();
    }

    protected override IEnumerable<BoardColumnRow> GetBoardColumnRows(
      IEnumerable<BoardColumn> columns)
    {
      return columns.Select<BoardColumn, BoardColumnRow>((System.Func<BoardColumn, BoardColumnRow>) (c => new BoardColumnRow()
      {
        Id = new Guid?(c.Id.HasValue ? c.Id.Value : Guid.NewGuid()),
        Name = c.Name,
        Order = c.Order,
        ItemLimit = c.ItemLimit,
        ColumnType = (int) c.ColumnType,
        RevisedDate = new DateTime?(),
        Deleted = false,
        IsSplit = c.IsSplit,
        Description = c.Description
      }));
    }

    protected override SqlParameter BindBoardColumnRowTable(
      string parameterName,
      IEnumerable<BoardColumnRow> rows)
    {
      return this.BindTable(parameterName, "typ_BoardColumnTable2", rows.Select<BoardColumnRow, SqlDataRecord>((System.Func<BoardColumnRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(BoardSettingsComponent4.typ_BoardColumnTable2);
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
        sqlDataRecord1.SetString(2, row.Name);
        sqlDataRecord1.SetInt32(3, row.Order);
        sqlDataRecord1.SetInt32(4, row.ItemLimit);
        sqlDataRecord1.SetValue(5, (object) row.ColumnType);
        if (row.RevisedDate.HasValue)
          sqlDataRecord1.SetDateTime(6, row.RevisedDate.Value);
        else
          sqlDataRecord1.SetDBNull(6);
        sqlDataRecord1.SetValue(7, (object) row.Deleted);
        sqlDataRecord1.SetValue(8, (object) row.IsSplit);
        if (string.IsNullOrEmpty(row.Description))
          sqlDataRecord1.SetDBNull(9);
        else
          sqlDataRecord1.SetValue(9, (object) row.Description);
        return sqlDataRecord1;
      })));
    }

    protected override BoardSettingsDTO GetBoardSettings(ResultCollection rc)
    {
      BoardSettingsDTO boardSettings = (BoardSettingsDTO) null;
      this.AddBoardSettingsBinder(rc);
      rc.AddBinder<BoardColumnRow>((ObjectBinder<BoardColumnRow>) new BoardColumnRowBinder2());
      List<BoardSettingsDTO> items = rc.GetCurrent<BoardSettingsDTO>().Items;
      if (items.Any<BoardSettingsDTO>())
      {
        boardSettings = items[0];
        rc.NextResult();
        boardSettings.Columns = (IEnumerable<BoardColumnRow>) rc.GetCurrent<BoardColumnRow>().Items.OrderBy<BoardColumnRow, int>((System.Func<BoardColumnRow, int>) (c => c.Order));
      }
      return boardSettings;
    }
  }
}
