// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardSettingsDTOConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class BoardSettingsDTOConverter
  {
    public static BoardSettings BoardSettingsFromDTO(BoardSettingsDTO dto)
    {
      if (dto == null)
        return (BoardSettings) null;
      BoardSettings boardSettings = new BoardSettings();
      boardSettings.Id = new Guid?(dto.Id);
      boardSettings.TeamId = dto.TeamId;
      boardSettings.ExtensionId = new Guid?(dto.ExtensionId);
      boardSettings.BacklogLevelId = dto.BacklogLevelId;
      boardSettings.ExtensionLastChangeDate = dto.ExtensionLastChangedDate;
      if (dto.Rows.Any<BoardRowTable>())
        boardSettings.Rows = (IEnumerable<BoardRow>) dto.Rows.Select<BoardRowTable, BoardRow>((Func<BoardRowTable, BoardRow>) (r => new BoardRow()
        {
          Id = r.Id,
          Name = r.Name,
          Order = r.Order,
          Color = r.Color
        })).ToList<BoardRow>();
      if (dto.Columns.Any<BoardColumnRow>())
        boardSettings.Columns = (IEnumerable<BoardColumn>) dto.Columns.Select<BoardColumnRow, BoardColumn>((Func<BoardColumnRow, BoardColumn>) (c => new BoardColumn()
        {
          Id = c.Id,
          Name = c.Name,
          Order = c.Order,
          ItemLimit = c.ItemLimit,
          ColumnType = (BoardColumnType) c.ColumnType,
          IsSplit = c.IsSplit,
          Description = c.Description
        })).ToList<BoardColumn>();
      if (dto.BoardCardSettings != null)
        boardSettings.BoardCardSettings = BoardSettingsDTOConverter.BoardCardSettingsFromDTO(BoardCardSettings.ScopeType.KANBAN, dto.Id, dto.BoardCardSettings);
      if (dto.BoardCardRules != null)
      {
        if (boardSettings.BoardCardSettings == null)
          boardSettings.BoardCardSettings = new BoardCardSettings(BoardCardSettings.ScopeType.KANBAN, dto.Id);
        boardSettings.BoardCardSettings.Rules = BoardSettingsDTOConverter.BoardCardRulesFromDTO(dto.BoardCardRules).ToList<CardRule>();
      }
      if (dto.Options != null)
      {
        boardSettings.PreserveBacklogOrder = dto.Options.CardReordering == 1;
        boardSettings.StatusBadgeIsPublic = dto.Options.StatusBadgeIsPublic;
      }
      return boardSettings;
    }

    public static BoardCardSettings BoardCardSettingsFromDTO(
      BoardCardSettings.ScopeType scope,
      Guid boardId,
      IEnumerable<BoardCardSettingRow> boardCardRules)
    {
      BoardCardSettings boardCardSettings = (BoardCardSettings) null;
      if (boardCardRules != null && boardCardRules.Any<BoardCardSettingRow>())
        boardCardSettings = new BoardCardSettings(scope, boardId, boardCardRules);
      return boardCardSettings;
    }

    public static IEnumerable<CardRule> BoardCardRulesFromDTO(BoardCardRulesDTO boardCardRules)
    {
      if (boardCardRules.BoardCardRules == null || !boardCardRules.BoardCardRules.Any<BoardCardRuleRow>())
        return Enumerable.Empty<CardRule>();
      Dictionary<int, CardRule> dictionary = new Dictionary<int, CardRule>();
      foreach (BoardCardRuleRow boardCardRule in boardCardRules.BoardCardRules)
      {
        CardRule cardRule = new CardRule(boardCardRule.Name, boardCardRule.Type, boardCardRule.IsEnabled, (List<KeyValuePair<string, string>>) null, boardCardRule.Filter, BoardSettingsDTOConverter.GetFilterModel(boardCardRule.FilterExpression));
        dictionary[boardCardRule.Id] = cardRule;
      }
      foreach (CardRuleAttributeRow cardRuleAttribute in boardCardRules.BoardCardRuleAttributes)
      {
        if (dictionary[cardRuleAttribute.RuleId].StyleAttributes == null)
          dictionary[cardRuleAttribute.RuleId].StyleAttributes = new List<KeyValuePair<string, string>>();
        dictionary[cardRuleAttribute.RuleId].StyleAttributes.Add(new KeyValuePair<string, string>(cardRuleAttribute.Name, cardRuleAttribute.Value));
      }
      return (IEnumerable<CardRule>) dictionary.Values.ToList<CardRule>();
    }

    public static FilterModel GetFilterModel(string json)
    {
      FilterModel filterModel = (FilterModel) null;
      if (!string.IsNullOrEmpty(json))
      {
        try
        {
          filterModel = JsonConvert.DeserializeObject<FilterModel>(json);
        }
        catch
        {
          filterModel = (FilterModel) null;
        }
      }
      return filterModel;
    }
  }
}
