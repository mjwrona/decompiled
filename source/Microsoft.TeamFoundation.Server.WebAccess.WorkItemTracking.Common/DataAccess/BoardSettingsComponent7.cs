// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent7
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent7 : BoardSettingsComponent6
  {
    protected const int c_MaxBoardScopeLength = 255;
    private static SqlMetaData[] typ_CardFieldsTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("Type", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("FieldName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Property", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Value", SqlDbType.NVarChar, (long) byte.MaxValue)
    };

    public override void UpdateBoardExtension(Guid projectId, Guid boardId, Guid extensionId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(boardId, nameof (boardId));
      ArgumentUtility.CheckForEmptyGuid(extensionId, nameof (extensionId));
      this.PrepareStoredProcedure("prc_UpdateBoardExtension");
      this.BindDataspace(projectId);
      this.BindGuid("@boardId", boardId);
      this.BindGuid("@extensionId", extensionId);
      this.ExecuteNonQuery();
    }

    public override void SetCardRules(
      Guid projectId,
      string scope,
      Guid boardId,
      IEnumerable<BoardCardRuleRow> rules,
      IEnumerable<RuleAttributeRow> attributes)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(boardId, nameof (boardId));
      ArgumentUtility.CheckForNull<IEnumerable<BoardCardRuleRow>>(rules, nameof (rules));
      ArgumentUtility.CheckForNull<IEnumerable<RuleAttributeRow>>(attributes, nameof (attributes));
      ArgumentUtility.CheckStringForNullOrEmpty(scope, nameof (scope));
      this.PrepareStoredProcedure("prc_SetBoardCardRules");
      this.BindDataspace(projectId);
      this.BindString("@boardScope", scope, (int) byte.MaxValue, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindGuid("@boardScopeId", boardId);
      this.BindTable("@rules", "typ_CardRuleTable", TVPBinders.BindCardRuleRows(rules));
      this.BindTable("@attributes", "typ_CardRuleAttributeTable", TVPBinders.BindCardStyleRows(attributes));
      this.ExecuteNonQuery();
    }

    public override BoardCardRulesDTO GetBoardCardRules(
      Guid projectId,
      BoardCardSettings.ScopeType scopeType,
      Guid scopeId)
    {
      this.PrepareStoredProcedure("prc_GetBoardCardRules");
      this.BindDataspace(projectId);
      this.BindString("@boardScope", scopeType.ToString(), (int) byte.MaxValue, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindGuid("@boardScopeId", scopeId);
      BoardCardRulesDTO boardCardRules = (BoardCardRulesDTO) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BoardCardRuleRow>((ObjectBinder<BoardCardRuleRow>) new BoardCardRuleBinder());
        resultCollection.AddBinder<CardRuleAttributeRow>((ObjectBinder<CardRuleAttributeRow>) new BoardCardStyleBinder());
        boardCardRules = new BoardCardRulesDTO();
        boardCardRules.BoardCardRules = (IEnumerable<BoardCardRuleRow>) resultCollection.GetCurrent<BoardCardRuleRow>().Items;
        if (boardCardRules.BoardCardRules != null)
        {
          if (boardCardRules.BoardCardRules.Any<BoardCardRuleRow>())
          {
            resultCollection.NextResult();
            boardCardRules.BoardCardRuleAttributes = (IEnumerable<CardRuleAttributeRow>) resultCollection.GetCurrent<CardRuleAttributeRow>().Items;
          }
        }
      }
      return boardCardRules;
    }

    protected override BoardSettingsDTO GetBoardSettings(ResultCollection rc)
    {
      BoardSettingsDTO boardSettings = base.GetBoardSettings(rc);
      if (boardSettings != null && rc.TryNextResult())
      {
        rc.AddBinder<BoardCardRuleRow>((ObjectBinder<BoardCardRuleRow>) new BoardCardRuleBinder());
        rc.AddBinder<CardRuleAttributeRow>((ObjectBinder<CardRuleAttributeRow>) new BoardCardStyleBinder());
        boardSettings.BoardCardRules = new BoardCardRulesDTO();
        boardSettings.BoardCardRules.BoardCardRules = (IEnumerable<BoardCardRuleRow>) rc.GetCurrent<BoardCardRuleRow>().Items;
        rc.NextResult();
        if (boardSettings.BoardCardRules.BoardCardRules != null && boardSettings.BoardCardRules.BoardCardRules.Any<BoardCardRuleRow>())
          boardSettings.BoardCardRules.BoardCardRuleAttributes = (IEnumerable<CardRuleAttributeRow>) rc.GetCurrent<CardRuleAttributeRow>().Items;
      }
      return boardSettings;
    }

    protected override SqlParameter BindCardFieldsTable(
      string parameterName,
      IEnumerable<BoardCardSettingRow> rows)
    {
      return this.BindTable(parameterName, "typ_CardFieldsTable2", rows.Select<BoardCardSettingRow, SqlDataRecord>((System.Func<BoardCardSettingRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BoardSettingsComponent7.typ_CardFieldsTable2);
        sqlDataRecord.SetString(0, row.Type);
        sqlDataRecord.SetString(1, row.Field);
        sqlDataRecord.SetString(2, row.Property);
        sqlDataRecord.SetString(3, row.Value);
        return sqlDataRecord;
      })));
    }
  }
}
