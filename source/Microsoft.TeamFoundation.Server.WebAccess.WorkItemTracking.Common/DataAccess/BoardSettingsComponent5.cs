// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent5
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent5 : BoardSettingsComponent4
  {
    private const int c_MaxBoardScopeLength = 255;
    private const int c_MaxWitNameLength = 50;
    private static SqlMetaData[] typ_CardFieldsTable = new SqlMetaData[4]
    {
      new SqlMetaData("Type", SqlDbType.NVarChar, 50L),
      new SqlMetaData("FieldName", SqlDbType.NVarChar, 70L),
      new SqlMetaData("Property", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Value", SqlDbType.NVarChar, (long) byte.MaxValue)
    };

    public override IEnumerable<BoardCardSettingRow> GetBoardCardSettings(
      Guid projectId,
      BoardCardSettings.ScopeType scopeType,
      Guid scopeId)
    {
      this.PrepareStoredProcedure("prc_GetBoardCardSettings");
      this.BindDataspace(projectId);
      this.BindString("@boardScope", scopeType.ToString(), (int) byte.MaxValue, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindGuid("@boardScopeId", scopeId);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetBoardCardSettings(scopeType, scopeId, rc);
    }

    public override void SetBoardCardSettings(Guid projectId, BoardCardSettings cardSettings)
    {
      ArgumentUtility.CheckForNull<BoardCardSettings>(cardSettings, nameof (cardSettings));
      this.PrepareStoredProcedure("prc_SetBoardCardSettings");
      this.BindDataspace(projectId);
      this.BindString("@boardScope", cardSettings.Scope.ToString(), (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindGuid("@boardScopeId", cardSettings.ScopeId);
      this.BindCardFieldsTable("@cardFields", this.GetCardFieldsRows(cardSettings));
      this.ExecuteScalar();
    }

    protected virtual IEnumerable<BoardCardSettingRow> GetBoardCardSettings(
      BoardCardSettings.ScopeType scopeType,
      Guid scopeId,
      ResultCollection rc)
    {
      List<BoardCardSettingRow> boardCardSettings = (List<BoardCardSettingRow>) null;
      rc.AddBinder<BoardCardSettingRow>((ObjectBinder<BoardCardSettingRow>) new BoardCardSettingRowBinder());
      List<BoardCardSettingRow> items = rc.GetCurrent<BoardCardSettingRow>().Items;
      if (items.Count > 0)
        boardCardSettings = items;
      return (IEnumerable<BoardCardSettingRow>) boardCardSettings;
    }

    protected virtual SqlParameter BindCardFieldsTable(
      string parameterName,
      IEnumerable<BoardCardSettingRow> rows)
    {
      return this.BindTable(parameterName, "typ_CardFieldsTable", rows.Select<BoardCardSettingRow, SqlDataRecord>((System.Func<BoardCardSettingRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BoardSettingsComponent5.typ_CardFieldsTable);
        sqlDataRecord.SetString(0, row.Type);
        sqlDataRecord.SetString(1, row.Field);
        sqlDataRecord.SetString(2, row.Property);
        sqlDataRecord.SetString(3, row.Value);
        return sqlDataRecord;
      })));
    }

    protected virtual IEnumerable<BoardCardSettingRow> GetCardFieldsRows(BoardCardSettings board)
    {
      IEnumerable<BoardCardSettingRow> first = (IEnumerable<BoardCardSettingRow>) new List<BoardCardSettingRow>();
      if (board.Cards != null)
      {
        foreach (KeyValuePair<string, List<FieldSetting>> card in board.Cards)
        {
          foreach (FieldSetting fieldSetting in card.Value)
          {
            IEnumerable<BoardCardSettingRow> itemFieldRows = this.GetItemFieldRows(board, card.Key, fieldSetting);
            first = first.Concat<BoardCardSettingRow>(itemFieldRows);
          }
        }
      }
      return first;
    }

    private IEnumerable<BoardCardSettingRow> GetItemFieldRows(
      BoardCardSettings board,
      string cardType,
      FieldSetting fieldSetting)
    {
      List<BoardCardSettingRow> itemFieldRows = new List<BoardCardSettingRow>();
      if (!fieldSetting.IsNullOrEmpty<KeyValuePair<string, string>>())
      {
        foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) fieldSetting)
        {
          BoardCardSettingRow boardCardSettingRow = new BoardCardSettingRow()
          {
            Type = cardType,
            Field = fieldSetting.FieldIdentifier,
            Property = keyValuePair.Key,
            Value = keyValuePair.Value
          };
          itemFieldRows.Add(boardCardSettingRow);
        }
      }
      return (IEnumerable<BoardCardSettingRow>) itemFieldRows;
    }

    protected override BoardSettingsDTO GetBoardSettings(ResultCollection rc)
    {
      BoardSettingsDTO boardSettings = base.GetBoardSettings(rc);
      if (boardSettings != null)
      {
        rc.NextResult();
        boardSettings.BoardCardSettings = this.GetBoardCardSettings(BoardCardSettings.ScopeType.KANBAN, boardSettings.Id, rc);
      }
      return boardSettings;
    }
  }
}
