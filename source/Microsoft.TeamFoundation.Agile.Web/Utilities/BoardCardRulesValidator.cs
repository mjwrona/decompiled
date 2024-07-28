// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.BoardCardRulesValidator
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class BoardCardRulesValidator : BoardValidatorBase
  {
    private const int MaxNumberofFillRules = 10;
    private const int MaxNumberofTagStyleRules = 20;
    private const int MaxNumberOfSwimlaneRules = 20;
    private const int MaxNumberofConditionsInsideRules = 5;
    private const int MaxRuleNameSize = 255;
    private const int MaxAttributeNameSize = 255;
    private const int MaxAttributeValueSize = 255;
    private const string RowIdAttributeName = "rowId";
    private BoardCardRules m_boardCardRules;
    private string m_ruleType;
    private IDictionary<Guid, string> m_tagNamesReplacedbyId;

    public BoardCardRulesValidator(
      IVssRequestContext context,
      BoardCardRules boardCardRules,
      string type,
      IDictionary<Guid, string> tagNamesReplacedbyId)
      : base(context)
    {
      this.m_boardCardRules = boardCardRules;
      this.m_ruleType = type;
      this.m_tagNamesReplacedbyId = tagNamesReplacedbyId;
    }

    public override bool Validate(bool throwOnFail = false) => this.ExecuteValidator((Action) (() => this._validate()), throwOnFail);

    public virtual FilterClauseValidator GetFilterClauseValidator() => new FilterClauseValidator(this.RequestContext);

    internal string GetRuleDisplayName(
      BoardCardRulesValidator.AllowedRuleType ruleTypeEnum,
      string ruleName)
    {
      string ruleDisplayName = ruleName;
      Guid result;
      string str;
      if (BoardCardRulesValidator.AllowedRuleType.TagStyle == ruleTypeEnum && this.m_tagNamesReplacedbyId != null && Guid.TryParse(ruleName, out result) && this.m_tagNamesReplacedbyId.TryGetValue(result, out str))
        ruleDisplayName = str;
      return ruleDisplayName;
    }

    private bool _validate()
    {
      BoardCardRulesValidator.AllowedRuleType ruleType = BoardCardRulesValidator.AllowedRuleType.None;
      this.Assert((Func<bool>) (() => this.m_boardCardRules != null), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_SettingsNULL());
      this.Assert((Func<bool>) (() => this.m_boardCardRules.Scope == "KANBAN" || this.m_boardCardRules.Scope == "TASKBOARD"), Microsoft.TeamFoundation.Agile.Web.Resources.CardCustomization_BadBoardType());
      this.Assert((Func<bool>) (() => this.m_boardCardRules.ScopeId != Guid.Empty), Microsoft.TeamFoundation.Agile.Web.Resources.EmptyBoardId());
      this.Assert((Func<bool>) (() => Enum.TryParse<BoardCardRulesValidator.AllowedRuleType>(this.m_ruleType, true, out ruleType)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_InvalidType((object) this.m_ruleType));
      bool flag;
      switch (ruleType)
      {
        case BoardCardRulesValidator.AllowedRuleType.Fill:
          this.ValidateFillRules(this.m_boardCardRules.Rules);
          this.ValidateCardRuleAttributes(ruleType);
          flag = true;
          break;
        case BoardCardRulesValidator.AllowedRuleType.TagStyle:
          this.ValidateTagColorRules(this.m_boardCardRules.Rules);
          this.ValidateCardRuleAttributes(ruleType);
          flag = true;
          break;
        case BoardCardRulesValidator.AllowedRuleType.Annotation:
          flag = true;
          break;
        case BoardCardRulesValidator.AllowedRuleType.SwimlaneRule:
          this.ValidateSwimlaneRules(this.m_boardCardRules.Rules);
          this.ValidateCardRuleAttributes(ruleType);
          this.ValidateSwimlaneRuleAttributes();
          flag = true;
          break;
        default:
          this.Assert((Func<bool>) (() => false), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_InvalidType((object) this.m_ruleType));
          flag = false;
          break;
      }
      return flag;
    }

    private void ValidateRuleNames(string name, HashSet<string> ruleNames)
    {
      this.Assert((Func<bool>) (() => !string.IsNullOrEmpty(name)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_RuleNameNullOrEmpty());
      this.Assert((Func<bool>) (() => name.Length <= (int) byte.MaxValue), string.Format(Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_RuleNameExceedsLimit((object) (int) byte.MaxValue)));
      name = name.Trim();
      this.Assert((Func<bool>) (() => !ruleNames.Contains(name)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_DuplicateName((object) name));
      ruleNames.Add(name);
    }

    private bool ValidateTagColorRules(List<BoardCardRuleRow> rules)
    {
      this.Assert((Func<bool>) (() => this.m_boardCardRules.Rules.Count <= 20), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_MaxLimitForNumberOfRules((object) 20));
      HashSet<string> ruleNames = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.TagName);
      foreach (BoardCardRuleRow rule1 in rules)
      {
        BoardCardRuleRow rule = rule1;
        this.ValidateRuleNames(this.GetRuleDisplayName(BoardCardRulesValidator.AllowedRuleType.TagStyle, rule.Name), ruleNames);
        this.Assert((Func<bool>) (() => Enum.TryParse<BoardCardRulesValidator.AllowedRuleType>(rule.Type, true, out BoardCardRulesValidator.AllowedRuleType _)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_InvalidType((object) rule.Type));
        this.Assert((Func<bool>) (() => string.Equals(rule.FilterExpression, string.Empty)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_EmptyOrNullWiql(), (object) rule.Type);
      }
      return true;
    }

    private bool ValidateFillRules(List<BoardCardRuleRow> rules)
    {
      this.Assert((Func<bool>) (() => this.m_boardCardRules.Rules.Count <= 10), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_MaxLimitForNumberOfRules((object) 10));
      HashSet<string> ruleNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      foreach (BoardCardRuleRow rule1 in rules)
      {
        BoardCardRuleRow rule = rule1;
        this.ValidateRuleNames(rule.Name, ruleNames);
        this.Assert((Func<bool>) (() => Enum.TryParse<BoardCardRulesValidator.AllowedRuleType>(rule.Type, true, out ruleType)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_InvalidType((object) rule.Type));
        this.IsValidWiql(rule.FilterExpression);
      }
      return true;
    }

    private void ValidateSwimlaneRules(List<BoardCardRuleRow> rules)
    {
      this.Assert((Func<bool>) (() => this.m_boardCardRules.Rules.Count <= 20), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_MaxLimitForNumberOfRules((object) 20));
      HashSet<string> ruleNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      foreach (BoardCardRuleRow rule1 in rules)
      {
        BoardCardRuleRow rule = rule1;
        this.ValidateRuleNames(rule.Name, ruleNames);
        this.Assert((Func<bool>) (() => Enum.TryParse<BoardCardRulesValidator.AllowedRuleType>(rule.Type, true, out BoardCardRulesValidator.AllowedRuleType _)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_InvalidType((object) rule.Type));
        this.IsValidWiql(rule.FilterExpression, (Func<FilterClauseValidator, FilterClause, bool>) ((validator, clause) =>
        {
          FieldDefinition fieldDefinition = validator.GetFieldDefinition(clause);
          if (fieldDefinition == null || !fieldDefinition.IsIdentity)
            return true;
          this.Assert((Func<bool>) (() => !clause.Value.Equals("@Me", StringComparison.OrdinalIgnoreCase)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_MeMacroNotSupported());
          return true;
        }));
      }
    }

    private bool IsValidWiql(
      string filterExpression,
      Func<FilterClauseValidator, FilterClause, bool> extraAssertion = null)
    {
      FilterModel filterModel = (FilterModel) null;
      if (string.IsNullOrEmpty(filterExpression))
        return true;
      try
      {
        filterModel = JsonConvert.DeserializeObject<FilterModel>(filterExpression);
      }
      catch (Exception ex)
      {
        this.Assert((Func<bool>) (() => false), ex.Message);
      }
      this.Assert((Func<bool>) (() => filterModel.Clauses.Count <= 5), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_MaxLimitForConditions((object) 5));
      this.Assert((Func<bool>) (() => filterModel.Groups.Count == 0), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_WIQLGroups());
      FilterClauseValidator clauseValidator = this.GetFilterClauseValidator();
      foreach (FilterClause clause1 in (IEnumerable<FilterClause>) filterModel.Clauses)
      {
        FilterClause clause = clause1;
        this.ValidateClause(clauseValidator, clause);
        if (extraAssertion != null)
          this.Assert((Func<bool>) (() => extraAssertion(clauseValidator, clause)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_InvalidField((object) clause.FieldName));
      }
      return true;
    }

    private bool ValidateClause(FilterClauseValidator clauseValidator, FilterClause clause)
    {
      this.Assert((Func<bool>) (() => clauseValidator.isFieldTypeValid(clause)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_InvalidField((object) clause.FieldName));
      FieldDefinition fieldDefinition = clauseValidator.GetFieldDefinition(clause);
      this.Assert((Func<bool>) (() => clauseValidator.isFieldTypeSupported(clause)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_UnSupportedType((object) fieldDefinition.FieldType));
      this.Assert((Func<bool>) (() => clauseValidator.isOperatorSupportedForType(clause)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_InvalidOperator((object) clause.Operator, (object) fieldDefinition.FieldType));
      if (!string.Empty.Equals(clause.LogicalOperator))
        this.Assert((Func<bool>) (() => clauseValidator.isLogicalOperatorAnd(clause)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRules_InvalidLogicalOperator((object) clause.LogicalOperator));
      return true;
    }

    private void ValidateCardRuleAttributes(
      BoardCardRulesValidator.AllowedRuleType ruleTypeEnum)
    {
      Dictionary<string, List<RuleAttributeRow>> dictionary = this.IndexAttributesByRuleName();
      foreach (BoardCardRuleRow rule in this.m_boardCardRules.Rules)
      {
        string ruleDisplayName = this.GetRuleDisplayName(ruleTypeEnum, rule.Name);
        List<RuleAttributeRow> ruleAttributeRowList;
        bool ruleContainsAttributes = dictionary.TryGetValue(rule.Name, out ruleAttributeRowList);
        this.Assert((Func<bool>) (() => ruleContainsAttributes), Microsoft.TeamFoundation.Agile.Web.Resources.CardRule_NoAttributesSupplied((object) ruleDisplayName));
        foreach (RuleAttributeRow attribute in ruleAttributeRowList)
          this.ValidateCardRuleAttribute(ruleDisplayName, attribute);
      }
    }

    private void ValidateCardRuleAttribute(string ruleDisplayName, RuleAttributeRow attribute)
    {
      this.Assert((Func<bool>) (() => !string.IsNullOrWhiteSpace(attribute.Name)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRuleAttribute_InvalidSettings((object) ruleDisplayName, (object) Microsoft.TeamFoundation.Agile.Web.Resources.CardRuleAttribute_MissingName()));
      this.Assert((Func<bool>) (() => attribute.Name.Length <= (int) byte.MaxValue), Microsoft.TeamFoundation.Agile.Web.Resources.CardRuleAttribute_InvalidSettings((object) ruleDisplayName, (object) Microsoft.TeamFoundation.Agile.Web.Resources.CardRuleAttribute_SettingNameExceedsLimit((object) (int) byte.MaxValue)));
      this.Assert((Func<bool>) (() => !string.IsNullOrWhiteSpace(attribute.Value)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRuleAttribute_InvalidSettings((object) ruleDisplayName, (object) Microsoft.TeamFoundation.Agile.Web.Resources.CardRuleAttribute_MissingValue()));
      this.Assert((Func<bool>) (() => attribute.Value.Length <= (int) byte.MaxValue), Microsoft.TeamFoundation.Agile.Web.Resources.CardRuleAttribute_InvalidSettings((object) ruleDisplayName, (object) Microsoft.TeamFoundation.Agile.Web.Resources.CardRuleAttribute_SettingValueExceedsLimit((object) (int) byte.MaxValue)));
    }

    private void ValidateSwimlaneRuleAttributes()
    {
      Dictionary<string, List<RuleAttributeRow>> dictionary = this.IndexAttributesByRuleName();
      foreach (BoardCardRuleRow rule in this.m_boardCardRules.Rules)
      {
        string ruleDisplayName = this.GetRuleDisplayName(BoardCardRulesValidator.AllowedRuleType.SwimlaneRule, rule.Name);
        List<RuleAttributeRow> source;
        bool ruleContainsAttributes = dictionary.TryGetValue(rule.Name, out source);
        this.Assert((Func<bool>) (() => ruleContainsAttributes), Microsoft.TeamFoundation.Agile.Web.Resources.CardRule_NoAttributesSupplied((object) ruleDisplayName));
        RuleAttributeRow rowIdAttribute = source.FirstOrDefault<RuleAttributeRow>((Func<RuleAttributeRow, bool>) (a => a.Name.Equals("rowId", StringComparison.OrdinalIgnoreCase)));
        this.Assert((Func<bool>) (() => rowIdAttribute != null), Microsoft.TeamFoundation.Agile.Web.Resources.CardRuleAttribute_InvalidRowIdAttribute((object) ruleDisplayName));
        this.Assert((Func<bool>) (() => Guid.TryParse(rowIdAttribute.Value, out Guid _)), Microsoft.TeamFoundation.Agile.Web.Resources.CardRuleAttribute_InvalidRowIdAttribute((object) ruleDisplayName));
      }
    }

    private Dictionary<string, List<RuleAttributeRow>> IndexAttributesByRuleName()
    {
      Dictionary<string, List<RuleAttributeRow>> dictionary = new Dictionary<string, List<RuleAttributeRow>>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      foreach (RuleAttributeRow attribute in this.m_boardCardRules.Attributes)
      {
        List<RuleAttributeRow> ruleAttributeRowList;
        if (dictionary.TryGetValue(attribute.RuleName, out ruleAttributeRowList))
          ruleAttributeRowList.Add(attribute);
        else
          dictionary[attribute.RuleName] = new List<RuleAttributeRow>()
          {
            attribute
          };
      }
      return dictionary;
    }

    internal enum AllowedRuleType
    {
      None = -1, // 0xFFFFFFFF
      Fill = 0,
      TagStyle = 1,
      Annotation = 2,
      SwimlaneRule = 3,
    }
  }
}
