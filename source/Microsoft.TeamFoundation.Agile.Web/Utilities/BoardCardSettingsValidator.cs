// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.BoardCardSettingsValidator
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class BoardCardSettingsValidator : BoardValidatorBase
  {
    private const int c_maxAdditionalFieldsCount = 10;

    protected BacklogLevelConfiguration Backlog { get; private set; }

    protected IAgileSettings AgileSettings { get; private set; }

    protected BoardCardSettings Settings { get; private set; }

    protected Guid? TeamID { get; private set; }

    protected bool MakeValid { get; private set; }

    protected bool SkipExtraValidation { get; private set; }

    protected string BoardBacklogLevelId { get; set; }

    public BoardCardSettingsValidator(
      IVssRequestContext context,
      BacklogLevelConfiguration backlog,
      IAgileSettings settings,
      BoardCardSettings newSettings,
      Guid? teamID,
      bool makeValid = false,
      bool skipExtraValidation = false)
      : base(context)
    {
      this.Backlog = backlog;
      this.AgileSettings = settings;
      this.Settings = newSettings;
      this.TeamID = teamID;
      this.MakeValid = makeValid;
      this.SkipExtraValidation = skipExtraValidation;
    }

    public override bool Validate(bool throwOnFail = false) => this.ExecuteValidator((Action) (() => this.ValidateInternal()), throwOnFail);

    public bool ValidateInternal()
    {
      this.ValidateCommonSettings();
      if (!this.SkipExtraValidation)
      {
        this.ValidateBoardId();
        this.ValidateBacklog();
      }
      this.ValidateRequiredCoreFieldsPresent();
      this.ValidateNoDuplicateFieldSettings();
      this.ValidateAdditionalFieldCount();
      this.ValidateCardTypes();
      this.AreAllFieldsValid();
      return true;
    }

    private void ValidateCommonSettings()
    {
      this.Assert((Func<bool>) (() => this.Settings != null), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_SettingsNULL());
      this.Assert((Func<bool>) (() => this.Settings.Scope == BoardCardSettings.ScopeType.KANBAN || this.Settings.Scope == BoardCardSettings.ScopeType.TASKBOARD), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_BadBoardType());
      this.Assert((Func<bool>) (() => this.Settings.ScopeId != Guid.Empty), Microsoft.TeamFoundation.Agile.Web.Resources.EmptyBoardId());
      this.Assert((Func<bool>) (() => this.Settings.Cards != null && this.Settings.Cards.Count != 0), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_NoCardsFound());
      this.ValidateFieldSettings();
      this.ValidateNoDuplicateCards();
    }

    private void ValidateFieldSettings()
    {
      foreach (List<FieldSetting> fieldSettingList in this.Settings.Cards.Values)
      {
        foreach (FieldSetting field in fieldSettingList)
          this.ValidateFieldSetting(field);
      }
    }

    private void ValidateFieldSetting(FieldSetting field)
    {
      foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) field)
      {
        KeyValuePair<string, string> setting = keyValuePair;
        this.Assert((Func<bool>) (() => setting.Key != null), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_CardKeyCannotBeNull());
        this.Assert((Func<bool>) (() => setting.Value != null), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_CardValueCannotBeNull((object) setting.Key));
      }
    }

    private void ValidateNoDuplicateCards()
    {
      HashSet<string> cardNames = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      foreach (KeyValuePair<string, List<FieldSetting>> card1 in this.Settings.Cards)
      {
        KeyValuePair<string, List<FieldSetting>> card = card1;
        this.Assert((Func<bool>) (() => !cardNames.Contains(card.Key)), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_DuplicateCards((object) card.Key));
        cardNames.Add(card.Key);
      }
    }

    internal void ValidateBoardId()
    {
      if (this.Settings.Scope == BoardCardSettings.ScopeType.TASKBOARD)
      {
        this.Assert((Func<bool>) (() => this.Settings.ScopeId == this.TeamID.Value), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_UnknownBoardId((object) this.Settings.ScopeId));
      }
      else
      {
        if (this.Settings.Scope != BoardCardSettings.ScopeType.KANBAN)
          return;
        BoardRecord boardMatchId = KanbanUtils.Instance.GetBoardRecordById(this.RequestContext.GetService<BoardService>(), this.RequestContext, this.TeamID.Value, this.Settings.ScopeId);
        this.Assert((Func<bool>) (() => boardMatchId != null), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_UnknownBoardId((object) this.Settings.ScopeId));
        this.BoardBacklogLevelId = boardMatchId.BacklogLevelId;
      }
    }

    internal void ValidateBacklog()
    {
      if (this.Settings.Scope == BoardCardSettings.ScopeType.TASKBOARD || this.Backlog != null)
        return;
      BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
      bool isBacklogValid = this.AgileSettings.BacklogConfiguration.TryGetBacklogLevelConfiguration(this.BoardBacklogLevelId, out backlogLevel);
      this.Assert((Func<bool>) (() => isBacklogValid), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_UnknownBoardId((object) this.Settings.ScopeId));
      this.Backlog = backlogLevel;
    }

    internal void ValidateRequiredCoreFieldsPresent()
    {
      foreach (KeyValuePair<string, List<FieldSetting>> card in this.Settings.Cards)
      {
        IEnumerable<FieldSetting> titleField = card.Value.Where<FieldSetting>((Func<FieldSetting, bool>) (field => TFStringComparer.WorkItemTypeName.Equals(field.FieldIdentifier, CoreFieldReferenceNames.Title)));
        this.Assert((Func<bool>) (() => titleField.Count<FieldSetting>() > 0), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_RequiredFieldMissing((object) CoreFieldReferenceNames.Title, (object) card.Key));
      }
    }

    private void ValidateNoDuplicateFieldSettings()
    {
      foreach (KeyValuePair<string, List<FieldSetting>> card in this.Settings.Cards)
      {
        List<string> dup = card.Value.GroupBy<FieldSetting, string>((Func<FieldSetting, string>) (x => x.FieldIdentifier), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).Where<IGrouping<string, FieldSetting>>((Func<IGrouping<string, FieldSetting>, bool>) (x => x.Count<FieldSetting>() > 1)).Select<IGrouping<string, FieldSetting>, string>((Func<IGrouping<string, FieldSetting>, string>) (x => x.Key)).ToList<string>();
        this.Assert((Func<bool>) (() => dup.Count == 0), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_DuplicateFields((object) dup.FirstOrDefault<string>(), (object) card.Key));
      }
    }

    internal void ValidateAdditionalFieldCount()
    {
      HashSet<string> fieldIdentifiers = CardSettingsUtils.Instance.GetCoreFieldIdentifiers(this.AgileSettings);
      foreach (KeyValuePair<string, List<FieldSetting>> card in this.Settings.Cards)
      {
        int count = 0;
        foreach (FieldSetting fieldSetting in card.Value)
        {
          if (!fieldIdentifiers.Contains(fieldSetting.FieldIdentifier))
            count++;
        }
        this.Assert((Func<bool>) (() => count <= 10), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_MaxAdditionalFieldsExceeded((object) 10, (object) card.Key));
      }
    }

    internal void ValidateCardTypes()
    {
      IEnumerable<string> boardWorkItems = CardSettingsUtils.Instance.GetBoardWorkItems(this.RequestContext, this.Backlog, this.AgileSettings, this.Settings.Scope);
      List<string> invalidCards = this.Settings.Cards.Select<KeyValuePair<string, List<FieldSetting>>, string>((Func<KeyValuePair<string, List<FieldSetting>>, string>) (c => c.Key)).Except<string>(boardWorkItems, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName).ToList<string>();
      if (!invalidCards.Any<string>())
        return;
      if (this.MakeValid)
      {
        foreach (string key in invalidCards)
          this.Settings.Cards.Remove(key);
      }
      else
        this.Assert((Func<bool>) (() => !invalidCards.Any<string>()), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_InvalidCardType((object) invalidCards.FirstOrDefault<string>()));
    }

    internal void AreAllFieldsValid()
    {
      WebAccessWorkItemService service = this.RequestContext.GetService<WebAccessWorkItemService>();
      IReadOnlyCollection<IWorkItemType> workItemTypes = service.GetWorkItemTypes(this.RequestContext, service.GetProjectId(this.RequestContext, this.AgileSettings.ProjectName));
      foreach (KeyValuePair<string, List<FieldSetting>> card1 in this.Settings.Cards)
      {
        KeyValuePair<string, List<FieldSetting>> card = card1;
        IWorkItemType workitemType = workItemTypes.Where<IWorkItemType>((Func<IWorkItemType, bool>) (w => TFStringComparer.WorkItemTypeName.Equals(w.Name, card.Key))).FirstOrDefault<IWorkItemType>();
        this.Assert((Func<bool>) (() => workitemType != null), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_InvalidCardType((object) card.Key));
        FieldDefinitionCollection validFieldsDefinitions = workitemType.GetFields(this.RequestContext);
        IEnumerable<FieldSetting> source = card.Value.Where<FieldSetting>((Func<FieldSetting, bool>) (f => !this.IsValidCardField(f, validFieldsDefinitions) && !string.IsNullOrEmpty(f.FieldIdentifier)));
        if (source.Any<FieldSetting>())
        {
          List<string> invalidFieldNames = source.Select<FieldSetting, string>((Func<FieldSetting, string>) (i => i.FieldIdentifier)).ToList<string>();
          if (this.MakeValid)
            card.Value.RemoveAll((Predicate<FieldSetting>) (f => invalidFieldNames.Contains<string>(f.FieldIdentifier, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName)));
          else
            this.Assert((Func<bool>) (() => !invalidFieldNames.Any<string>()), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_InvalidFieldType((object) invalidFieldNames.First<string>(), (object) card.Key));
        }
      }
    }

    private bool IsValidCardField(FieldSetting field, FieldDefinitionCollection validDefinitions)
    {
      FieldDefinition fieldDefinition = validDefinitions.Where<FieldDefinition>((Func<FieldDefinition, bool>) (d => TFStringComparer.WorkItemFieldReferenceName.Equals(d.ReferenceName, field.FieldIdentifier))).FirstOrDefault<FieldDefinition>();
      return fieldDefinition != null && this.IsValidCardFieldType(fieldDefinition.FieldType) && fieldDefinition.IsQueryable;
    }

    private bool IsValidCardFieldType(InternalFieldType fieldType)
    {
      switch (fieldType)
      {
        case InternalFieldType.String:
        case InternalFieldType.Integer:
        case InternalFieldType.DateTime:
        case InternalFieldType.PlainText:
        case InternalFieldType.TreePath:
        case InternalFieldType.Double:
        case InternalFieldType.Boolean:
          return true;
        default:
          return false;
      }
    }
  }
}
