// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  [ClientIncludeModel]
  public class BoardCardSettings
  {
    [DataMember]
    public Dictionary<string, List<FieldSetting>> Cards { get; set; }

    [DataMember]
    public BoardCardSettings.ScopeType Scope { get; set; }

    [DataMember]
    public Guid ScopeId { get; set; }

    [DataMember(Name = "styles")]
    public virtual List<CardRule> Rules { get; set; }

    [DataMember]
    public BoardTestSettings TestSettings { get; set; }

    public BoardCardSettings()
    {
      this.Cards = new Dictionary<string, List<FieldSetting>>();
      this.Rules = new List<CardRule>();
    }

    public BoardCardSettings(Guid? boardId, Guid teamId)
      : this()
    {
      if (boardId.HasValue && boardId.Value != Guid.Empty)
      {
        this.Scope = BoardCardSettings.ScopeType.KANBAN;
        this.ScopeId = boardId.Value;
      }
      else
      {
        this.Scope = BoardCardSettings.ScopeType.TASKBOARD;
        this.ScopeId = teamId;
      }
    }

    public BoardCardSettings(BoardCardSettings.ScopeType scope, Guid scopeId)
      : this()
    {
      this.Scope = scope;
      this.ScopeId = scopeId;
    }

    public void addCard(CardSetting setting)
    {
      if (setting == null)
        return;
      this.Cards[setting.ItemName] = (List<FieldSetting>) setting;
    }

    public void setCard(string key, List<FieldSetting> setting)
    {
      if (this.Cards.ContainsKey(key))
        this.Cards[key] = setting;
      else
        this.Cards.Add(key, setting);
    }

    public BoardCardSettings(
      BoardCardSettings.ScopeType Scope,
      Guid ScopeId,
      IEnumerable<BoardCardSettingRow> boardCardSettingRows)
      : this()
    {
      this.Scope = Scope;
      this.ScopeId = ScopeId;
      foreach (IGrouping<string, BoardCardSettingRow> source in boardCardSettingRows.GroupBy<BoardCardSettingRow, string>((Func<BoardCardSettingRow, string>) (boardCardSettingRow => boardCardSettingRow.Type)))
      {
        CardSetting setting = new CardSetting(source.Key, source.Select<BoardCardSettingRow, BoardCardSettingRow>((Func<BoardCardSettingRow, BoardCardSettingRow>) (boardCardSettingRow => new BoardCardSettingRow(boardCardSettingRow.Field, boardCardSettingRow.Property, boardCardSettingRow.Value))).ToList<BoardCardSettingRow>());
        if (!this.Cards.ContainsKey(source.Key))
          this.addCard(setting);
      }
    }

    public enum ScopeType
    {
      KANBAN = 0,
      TASKBOARD = 1,
      DELIVERYTIMELINE = 2,
      NONE = 255, // 0x000000FF
    }
  }
}
