// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.CardSettingsUtils
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class CardSettingsUtils : ICardSettingsUtils
  {
    private static readonly object s_lock = new object();
    private static ICardSettingsUtils s_instance = (ICardSettingsUtils) null;
    private static IReadOnlyList<FieldSetting> s_defaultCoreFields;
    public const string c_displayFormatPropertyName = "displayFormat";
    public static readonly string[] s_requiredCoreFieldRefNames = new string[1]
    {
      CoreFieldReferenceNames.Title
    };
    public static readonly string[] s_optionalCoreFieldRefNames = new string[3]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.Tags
    };
    public static readonly string[] s_defaultCoreFieldRefNames = new string[5]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.State
    };
    private const string c_SelectStatement = "SELECT [System.Id] FROM WorkItems WHERE ";

    private CardSettingsUtils() => this.InitializeDefaultCardFields();

    public static ICardSettingsUtils Instance
    {
      get
      {
        if (CardSettingsUtils.s_instance == null)
        {
          lock (CardSettingsUtils.s_lock)
          {
            if (CardSettingsUtils.s_instance == null)
              CardSettingsUtils.s_instance = (ICardSettingsUtils) new CardSettingsUtils();
          }
        }
        return CardSettingsUtils.s_instance;
      }
      set => CardSettingsUtils.s_instance = value;
    }

    private void InitializeDefaultCardFields()
    {
      List<FieldSetting> fieldSettingList = new List<FieldSetting>();
      foreach (string coreFieldRefName in CardSettingsUtils.s_defaultCoreFieldRefNames)
      {
        FieldSetting field = new FieldSetting()
        {
          FieldIdentifier = coreFieldRefName
        };
        this.AddPropertiesToField(field);
        fieldSettingList.Add(field);
      }
      CardSettingsUtils.s_defaultCoreFields = (IReadOnlyList<FieldSetting>) fieldSettingList.AsReadOnly();
    }

    public void AddPropertiesToField(FieldSetting field)
    {
      if (!CardSettingsUtils.HasSameFieldIdentifier(field, CoreFieldReferenceNames.AssignedTo))
        return;
      field["displayFormat"] = "AvatarAndFullName";
    }

    private static bool HasSameFieldIdentifier(FieldSetting field, string searchIdentifier) => TFStringComparer.WorkItemFieldReferenceName.Equals(field.FieldIdentifier, searchIdentifier);

    public void AddWorkItemFields(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings.ScopeType scope,
      CardSetting card,
      string workItemType)
    {
      string str = (string) null;
      if (backlogLevel.IsRequirementsBacklog)
        str = settings.Process.EffortField.Name;
      else if (backlogLevel.IsTaskBacklog && settings.BacklogConfiguration.TaskBacklog.WorkItemTypes.Contains<string>(workItemType, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))
        str = settings.Process.RemainingWorkField.Name;
      if (string.IsNullOrEmpty(str))
        return;
      card.Add(new FieldSetting() { FieldIdentifier = str });
    }

    private CardSetting CreateCardSetting(string itemType, IEnumerable<FieldSetting> witFields)
    {
      CardSetting cardSetting = new CardSetting(itemType);
      foreach (FieldSetting witField in witFields)
        cardSetting.Add(witField);
      return cardSetting;
    }

    public virtual IEnumerable<string> GetBoardWorkItems(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings.ScopeType scope)
    {
      return scope != BoardCardSettings.ScopeType.TASKBOARD ? (!backlogLevel.IsRequirementsBacklog ? (IEnumerable<string>) backlogLevel.WorkItemTypes : (IEnumerable<string>) settings.BacklogConfiguration.RequirementBacklog.WorkItemTypes) : settings.BacklogConfiguration.RequirementBacklog.WorkItemTypes.Concat<string>((IEnumerable<string>) settings.BacklogConfiguration.TaskBacklog.WorkItemTypes);
    }

    public BoardCardSettings GetDefaultBoardCardSettings(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings.ScopeType scope,
      Guid scopeId)
    {
      Dictionary<string, List<FieldSetting>> dictionary = new Dictionary<string, List<FieldSetting>>();
      List<FieldSetting> witFields = new List<FieldSetting>();
      witFields.AddRange((IEnumerable<FieldSetting>) CardSettingsUtils.s_defaultCoreFields);
      foreach (string boardWorkItem in this.GetBoardWorkItems(context, backlogLevel, settings, scope))
      {
        CardSetting cardSetting = this.CreateCardSetting(boardWorkItem, (IEnumerable<FieldSetting>) witFields);
        this.AddWorkItemFields(context, backlogLevel, settings, scope, cardSetting, boardWorkItem);
        dictionary[cardSetting.ItemName] = (List<FieldSetting>) cardSetting;
      }
      return new BoardCardSettings(scope, scopeId)
      {
        Cards = dictionary
      };
    }

    public void RemoveNonDefaultCards(
      BoardCardSettings savedSettings,
      BoardCardSettings defaultSettings)
    {
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, List<FieldSetting>> card in savedSettings.Cards)
      {
        string key = card.Key;
        if (!defaultSettings.Cards.Keys.Contains<string>(key, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))
          stringList.Add(key);
      }
      foreach (string key in stringList)
        savedSettings.Cards.Remove(key);
    }

    public void InsertDefaultCards(
      BoardCardSettings savedSettings,
      BoardCardSettings defaultSettings)
    {
      foreach (KeyValuePair<string, List<FieldSetting>> card in defaultSettings.Cards)
      {
        string key = card.Key;
        if (!savedSettings.Cards.Keys.Contains<string>(key, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))
          savedSettings.Cards[key] = card.Value;
      }
    }

    private BoardCardSettings ReconcileBoardCardSettingsForGET(
      IVssRequestContext context,
      IAgileSettings settings,
      BoardCardSettings savedSettings,
      BoardCardSettings defaultSettings)
    {
      ArgumentUtility.CheckForNull<BoardCardSettings>(savedSettings, nameof (savedSettings));
      ArgumentUtility.CheckForNull<BoardCardSettings>(defaultSettings, nameof (defaultSettings));
      if (savedSettings.Cards == null || savedSettings.Cards.Count == 0)
      {
        savedSettings.Cards = defaultSettings.Cards;
      }
      else
      {
        this.RemoveNonDefaultCards(savedSettings, defaultSettings);
        this.InsertDefaultCards(savedSettings, defaultSettings);
      }
      this.SortBasedOnRowNumber(savedSettings);
      this.RemoveAllRowNumberKey(savedSettings);
      return savedSettings;
    }

    public void ValidateAndReconcileSettingsForGET(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings.ScopeType scope,
      Guid scopeId,
      BoardCardSettings savedSettings)
    {
      ArgumentUtility.CheckForNull<BoardCardSettings>(savedSettings, nameof (savedSettings));
      BoardCardSettings boardCardSettings = this.GetDefaultBoardCardSettings(context, backlogLevel, settings, scope, scopeId);
      if (savedSettings.Cards != null && savedSettings.Cards.Count != 0 && this.ValidateBoardCardSettings(context, backlogLevel, settings, savedSettings, new Guid?(), true, true) != null)
        savedSettings.Cards.Clear();
      this.ReconcileBoardCardSettingsForGET(context, settings, savedSettings, boardCardSettings);
    }

    public string ValidateAndReconcileBoardCardSettingsForSET(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings newSettings,
      Guid teamID)
    {
      string str = this.ValidateBoardCardSettings(context, backlogLevel, settings, newSettings, new Guid?(teamID));
      if (str != null)
        return str;
      this.ReconcileBoardCardSettingsForSET(newSettings);
      return str;
    }

    public List<CardRule> GetBoardCardStyles(IVssRequestContext requestContext) => new List<CardRule>()
    {
      this.GetHighPriorityBugStyle(requestContext),
      this.GetHighRiskStoryStyle(requestContext)
    };

    private CardRule GetHighPriorityBugStyle(IVssRequestContext requestContext)
    {
      string str = "[System.WorkItemType] = 'Bug' and [Microsoft.VSTS.Common.Priority] = 1";
      CardStyle cardStyle = new CardStyle("Background", new KeyValuePair<string, string>[1]
      {
        new KeyValuePair<string, string>("background-color", "#ECAAAA")
      });
      FilterModel filterModel = this.GetFilterModel(str, requestContext);
      return new CardRule("NewBug", "fill", true, cardStyle.Properties, str, filterModel);
    }

    private CardRule GetHighRiskStoryStyle(IVssRequestContext requestContext)
    {
      string str = "[System.WorkItemType] = 'User Story' and [Microsoft.VSTS.Common.Risk] = '1 - High'";
      CardStyle cardStyle = new CardStyle("Background", new KeyValuePair<string, string>[1]
      {
        new KeyValuePair<string, string>("background-color", "#E5BBF5")
      });
      FilterModel filterModel = this.GetFilterModel(str, requestContext);
      return new CardRule("HighRiskStory", "fill", true, cardStyle.Properties, str, filterModel);
    }

    public string ValidateCardRules(
      IVssRequestContext context,
      BoardCardRules boardCardRules,
      string type,
      IDictionary<Guid, string> tagNamesReplacedbyId)
    {
      BoardCardRulesValidator cardRulesValidator = new BoardCardRulesValidator(context, boardCardRules, type, tagNamesReplacedbyId);
      int num = cardRulesValidator.Validate(false) ? 1 : 0;
      string str = (string) null;
      if (num == 0)
        str = cardRulesValidator.GetUserFriendlyErrorMessage();
      return str;
    }

    private void ReconcileBoardCardSettingsForSET(BoardCardSettings newSettings) => this.InsertRowNumberData(newSettings);

    private string ValidateBoardCardSettings(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings newSettings,
      Guid? teamID,
      bool makeValid = false,
      bool skipExtraValidation = false)
    {
      string str = (string) null;
      BoardCardSettingsValidator settingsValidator = new BoardCardSettingsValidator(context, backlogLevel, settings, newSettings, teamID, makeValid, skipExtraValidation);
      if (!settingsValidator.Validate(false))
        str = settingsValidator.GetUserFriendlyErrorMessage();
      return str;
    }

    public string TransformWiqlNamesToIds(IVssRequestContext requestContext, string wiql)
    {
      if (string.IsNullOrEmpty(wiql))
        return wiql;
      wiql = "SELECT [System.Id] FROM WorkItems WHERE " + wiql;
      wiql = WiqlTransformUtils.TransformNamesToIds(requestContext, wiql, true);
      wiql = wiql.Substring("SELECT [System.Id] FROM WorkItems WHERE ".Length);
      return wiql;
    }

    public List<CardRule> TransformWiqls(IVssRequestContext requestContext, List<CardRule> rules)
    {
      WiqlIdToNameTransformer toNameTransformer = new WiqlIdToNameTransformer();
      if (rules.IsNullOrEmpty<CardRule>())
        return rules;
      foreach (CardRule rule in rules)
      {
        if (!string.IsNullOrEmpty(rule.QueryText))
        {
          string wiql = "SELECT [System.Id] FROM WorkItems WHERE " + rule.QueryText;
          string str = toNameTransformer.ReplaceIdWithText(requestContext, wiql);
          rule.QueryText = str.Substring("SELECT [System.Id] FROM WorkItems WHERE ".Length);
          rule.QueryExpression = this.GetFilterModel(rule.QueryText, requestContext);
        }
      }
      return rules;
    }

    public virtual FilterModel GetFilterModel(string wiql, IVssRequestContext requestContext)
    {
      if (string.IsNullOrEmpty(wiql))
        return (FilterModel) null;
      QueryEditorModel queryEditorModel = new QueryEditorModel(requestContext, "SELECT [System.Id] FROM WorkItems WHERE " + wiql, false, (IDictionary<string, int>) new Dictionary<string, int>(), (string) null);
      return this.ResolveFieldReferences(requestContext, (FilterModel) queryEditorModel.SourceFilter);
    }

    private FilterModel ResolveFieldReferences(
      IVssRequestContext requestContext,
      FilterModel filterModel)
    {
      WorkItemTrackingRequestContext witRequestContext = new WorkItemTrackingRequestContext(requestContext);
      QueryAdapter queryAdapter = new QueryAdapter(requestContext);
      filterModel.Clauses = (ICollection<FilterClause>) filterModel.Clauses.Select<FilterClause, FilterClause>((Func<FilterClause, FilterClause>) (c =>
      {
        string str;
        try
        {
          str = witRequestContext.FieldDictionary.GetFieldByNameOrId(c.FieldName).ReferenceName;
        }
        catch (WorkItemTrackingFieldDefinitionNotFoundException ex)
        {
          str = c.FieldName;
        }
        c.FieldName = str;
        c.Operator = queryAdapter.GetInvariantOperator(c.Operator);
        c.LogicalOperator = queryAdapter.GetInvariantOperator(c.LogicalOperator);
        if (c.Value.StartsWith("@", StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            c.Value = queryAdapter.GetInvariantFieldValue(c.FieldName, c.Operator, c.Value);
          }
          catch (LegacyValidationException ex)
          {
          }
        }
        return c;
      })).ToList<FilterClause>();
      return filterModel;
    }

    public HashSet<string> GetCoreFieldIdentifiers(IAgileSettings settings) => new HashSet<string>((IEnumerable<string>) ((IEnumerable<string>) ((IEnumerable<string>) new string[2]
    {
      settings.Process.EffortField.Name,
      settings.Process.RemainingWorkField.Name
    }).Concat<string>((IEnumerable<string>) CardSettingsUtils.s_requiredCoreFieldRefNames).ToArray<string>()).Concat<string>((IEnumerable<string>) CardSettingsUtils.s_optionalCoreFieldRefNames).ToArray<string>());

    public void RemoveAllRowNumberKey(BoardCardSettings data)
    {
      foreach (KeyValuePair<string, List<FieldSetting>> card in data.Cards)
      {
        foreach (FieldSetting fieldSetting in card.Value)
        {
          if (fieldSetting.ContainsKey(FieldSetting.ROW_NUMBER))
            fieldSetting.Remove(FieldSetting.ROW_NUMBER);
        }
      }
    }

    public void SortBasedOnRowNumber(BoardCardSettings data)
    {
      if (data == null)
        return;
      foreach (KeyValuePair<string, List<FieldSetting>> card in data.Cards)
      {
        foreach (Dictionary<string, string> dictionary in card.Value)
        {
          if (!dictionary.ContainsKey(FieldSetting.ROW_NUMBER))
            return;
        }
        card.Value.Sort();
      }
    }

    public void InsertRowNumberData(BoardCardSettings data)
    {
      foreach (KeyValuePair<string, List<FieldSetting>> card in data.Cards)
      {
        List<FieldSetting> fieldSettingList = card.Value;
        int num = 0;
        foreach (FieldSetting fieldSetting in fieldSettingList)
        {
          fieldSetting.RowNumber = num;
          ++num;
        }
      }
    }

    public IEnumerable<ICardFieldDefinition> GetCardFieldDefinitions(
      IVssRequestContext context,
      IEnumerable<string> fields)
    {
      return (IEnumerable<ICardFieldDefinition>) context.GetService<WorkItemTrackingFieldService>().GetAllFields(context).Where<FieldEntry>((Func<FieldEntry, bool>) (f => fields.Contains<string>(f.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName))).Select<FieldEntry, WITCardFieldDefinition>((Func<FieldEntry, WITCardFieldDefinition>) (f => new WITCardFieldDefinition(new FieldDefinition(f))));
    }

    public List<string> GetCardsDisplayedFields(BoardCardSettings boardCardSettings)
    {
      List<string> cardsDisplayedFields = new List<string>();
      if (boardCardSettings != null && boardCardSettings.Cards != null)
        cardsDisplayedFields = boardCardSettings.Cards.SelectMany<KeyValuePair<string, List<FieldSetting>>, FieldSetting>((Func<KeyValuePair<string, List<FieldSetting>>, IEnumerable<FieldSetting>>) (cKvp => (IEnumerable<FieldSetting>) cKvp.Value)).Where<FieldSetting>((Func<FieldSetting, bool>) (fieldSetting => !string.IsNullOrEmpty(fieldSetting.FieldIdentifier))).Select<FieldSetting, string>((Func<FieldSetting, string>) (fieldSettings => fieldSettings.FieldIdentifier)).Distinct<string>().ToList<string>();
      return cardsDisplayedFields;
    }

    public IEnumerable<string> GetCardStyleRuleFields(BoardCardSettings boardCardSettings)
    {
      List<string> source = new List<string>();
      if (boardCardSettings != null && boardCardSettings.Rules != null)
      {
        foreach (CardRule rule in boardCardSettings.Rules)
        {
          if (rule.QueryExpression != null)
            source.AddRange(rule.QueryExpression.Clauses.Select<FilterClause, string>((Func<FilterClause, string>) (c => c.FieldName)));
        }
      }
      return source.Distinct<string>();
    }

    public IEnumerable<string> GetBoardFields(
      IVssRequestContext requestContext,
      BoardCardSettings boardCardSettings)
    {
      return requestContext.TraceBlock<IEnumerable<string>>(290929, 290930, "Agile", nameof (CardSettingsUtils), nameof (GetBoardFields), (Func<IEnumerable<string>>) (() =>
      {
        List<string> source = new List<string>();
        FieldEntry fieldEntry = (FieldEntry) null;
        if (boardCardSettings != null)
        {
          IFieldTypeDictionary fieldDictionary = new WorkItemTrackingRequestContext(requestContext).FieldDictionary;
          source.AddRange((IEnumerable<string>) this.GetCardsDisplayedFields(boardCardSettings));
          source.AddRange(this.GetCardStyleRuleFields(boardCardSettings));
          source = source.Where<string>((Func<string, bool>) (field => fieldDictionary.TryGetField(field, out fieldEntry))).ToList<string>();
        }
        return source.Distinct<string>();
      }));
    }

    public IDictionary<string, object> GetBoardCardSettingsCIData(
      BoardCardSettings boardCardSettings,
      IAgileSettings agileSettings)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      string[] array = this.GetCoreFieldIdentifiers(agileSettings).ToArray<string>();
      if (boardCardSettings != null && boardCardSettings.Cards != null)
      {
        Dictionary<string, List<FieldSetting>> cards = boardCardSettings.Cards;
        int count = cards.Keys.Count;
        IEnumerable<FieldSetting> source1 = cards.SelectMany<KeyValuePair<string, List<FieldSetting>>, FieldSetting>((Func<KeyValuePair<string, List<FieldSetting>>, IEnumerable<FieldSetting>>) (cKvp => (IEnumerable<FieldSetting>) cKvp.Value));
        int num1 = source1.Where<FieldSetting>((Func<FieldSetting, bool>) (fieldSettings => TFStringComparer.WorkItemFieldReferenceName.Equals(fieldSettings.FieldIdentifier, CoreFieldReferenceNames.Id))).Count<FieldSetting>();
        int num2 = source1.Where<FieldSetting>((Func<FieldSetting, bool>) (fieldSettings => TFStringComparer.WorkItemFieldReferenceName.Equals(fieldSettings.FieldIdentifier, CoreFieldReferenceNames.Tags))).Count<FieldSetting>();
        List<int> source2 = new List<int>();
        Dictionary<string, List<FieldSetting>>.ValueCollection.Enumerator enumerator = cards.Values.GetEnumerator();
        while (enumerator.MoveNext())
        {
          IEnumerable<string> first = enumerator.Current.Select<FieldSetting, string>((Func<FieldSetting, string>) (fieldSetting => fieldSetting.FieldIdentifier));
          source2.Add(first.Except<string>((IEnumerable<string>) array, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).Count<string>());
        }
        intelligenceData.Add(AgileCustomerIntelligencePropertyName.NumberOfWorkItemTypes, (double) count);
        intelligenceData.Add(AgileCustomerIntelligencePropertyName.WorkItemTypesWithIdEnabled, (double) num1);
        intelligenceData.Add(AgileCustomerIntelligencePropertyName.WorkItemTypesWithTagsEnabled, (double) num2);
        intelligenceData.Add(AgileCustomerIntelligencePropertyName.AverageAdditionalFieldsOnCard, source2.Average());
        intelligenceData.Add(AgileCustomerIntelligencePropertyName.MaximumAdditionalFieldsOnCard, (double) source2.Max());
        intelligenceData.Add(AgileCustomerIntelligencePropertyName.MinimumAdditionalFieldsOnCard, (double) source2.Min());
      }
      return intelligenceData.GetData();
    }

    public void ConvertTagIdToTagName(IVssRequestContext tfsRequestContext, List<CardRule> Rules)
    {
      ITeamFoundationTaggingService service = tfsRequestContext.GetService<ITeamFoundationTaggingService>();
      for (int index = Rules.Count - 1; index >= 0; --index)
      {
        CardRule rule = Rules[index];
        if (string.Equals(rule.Type, "tagStyle", StringComparison.OrdinalIgnoreCase))
        {
          TagDefinition tagDefinition;
          try
          {
            tagDefinition = service.GetTagDefinition(tfsRequestContext, new Guid(rule.Name));
          }
          catch
          {
            tagDefinition = (TagDefinition) null;
          }
          if (tagDefinition == null)
            Rules.RemoveAt(index);
          else
            rule.Name = tagDefinition.Name;
        }
      }
    }

    public enum DisplayFormatForAssignedTo
    {
      AvatarOnly,
      FullName,
      AvatarAndFullName,
    }
  }
}
