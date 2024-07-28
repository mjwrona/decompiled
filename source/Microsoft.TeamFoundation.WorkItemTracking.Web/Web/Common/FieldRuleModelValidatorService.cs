// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.FieldRuleModelValidatorService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  public class FieldRuleModelValidatorService : IFieldRuleModelValidatorService, IVssFrameworkService
  {
    public const int FriendlyNameMaxLength = 100;
    private static readonly Regex s_invalidCharacters = new Regex("[\0-\b\v\f\u000E-\u001F\u007F-\u009F]", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200.0));

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void FixAndValidate(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      FieldRuleModel fieldRuleModel)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckForNull<FieldRuleModel>(fieldRuleModel, nameof (fieldRuleModel));
      new FieldRuleModelValidatorService.FieldRuleModelValidator(requestContext, processId, witReferenceName, fieldRuleModel).FixAndValidate();
    }

    private class FieldRuleModelValidator
    {
      private readonly IVssRequestContext m_requestContext;
      private readonly Guid m_processId;
      private readonly FieldRuleModel m_fieldRuleModel;
      private readonly string m_witReferenceName;
      private readonly WorkItemTrackingFieldService m_fieldService;
      private readonly IProcessWorkItemTypeService m_processWorkItemTypeService;
      private readonly IReadOnlyCollection<WorkItemStateDefinition> m_stateDefinitions;
      private readonly StringComparer m_fieldNameComparer;
      private readonly int m_maxRulesPerWorkItemType;
      private IDictionary<Guid, FieldRuleModel> m_idToModelMapExcludingGivenRule;
      private IDictionary<Guid, FieldRuleModel> m_idToModelMapIncludingGivenRule;
      private IEnumerable<WorkItemFieldRule> m_witRulesExcludingGivenRule;
      private IEnumerable<WorkItemFieldRule> m_witRulesIncludingGivenRule;
      private IEnumerable<WorkItemFieldRule> m_witRulesForAffectedFieldsIncludingGivenRule;

      public FieldRuleModelValidator(
        IVssRequestContext requestContext,
        Guid processId,
        string witReferenceName,
        FieldRuleModel fieldRuleModel)
      {
        this.m_requestContext = requestContext;
        this.m_processId = processId;
        this.m_fieldRuleModel = fieldRuleModel;
        this.m_witReferenceName = witReferenceName;
        this.m_fieldService = requestContext.GetService<WorkItemTrackingFieldService>();
        this.m_processWorkItemTypeService = requestContext.GetService<IProcessWorkItemTypeService>();
        this.m_stateDefinitions = requestContext.GetService<IWorkItemStateDefinitionService>().GetStateDefinitions(requestContext, processId, witReferenceName, true);
        IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
        this.m_fieldNameComparer = configurationInfo.ServerStringComparer;
        this.m_maxRulesPerWorkItemType = configurationInfo.ValidatorConfiguration.MaxRulesPerWorkItemType;
      }

      public void FixAndValidate()
      {
        this.TrimPropertyValues();
        this.FixAndValidateFieldRuleModelOnly();
        this.ValidateMergedWorkItemTypeRules();
        this.ValidateMergedFieldRules();
        this.CheckForDuplicateRuleDefinitions();
        this.CheckForDuplicateRuleFriendlyNames();
      }

      private void FixAndValidateFieldRuleModelOnly()
      {
        this.CheckAndFixJsonDataTokens();
        this.CheckForValidFieldsAndConvertToReferenceNames();
        this.CheckAndFixValidCharacters();
        this.CheckEmbeddedImages();
        this.CheckCombinationsOfTwoConditions();
        this.CheckCombinationsOfThreeConditions();
        this.ForbidStandaloneWhenWasConditionExceptForState();
        this.ForbidUnconditionalDefaultValue();
        this.ForbidUnconditionalDefaultClock();
        this.ForbidUnconditionalDefaultUser();
        this.ForbidUnconditionalRequiredExceptFor_ForNotRule();
        this.CheckForNotRuleHasNoConditionAndOnlyOneAction();
        this.CheckHideFieldRuleNoConditionAndOnlyForNotAction();
        this.ForbidSystemFieldsForHiddenRule();
        this.ForbidFourConditions();
        this.ForbidConditionDeniedFields();
        this.ForbidDateTimeValuesInConditions();
        this.ForbidActionDeniedFields();
        this.CheckCompatibleFieldActionTypes();
      }

      private void ValidateMergedWorkItemTypeRules() => this.CheckActionsLimitPerWorkItemType();

      private void ValidateMergedFieldRules()
      {
        this.ForbidDuplicateActions();
        this.ForbidServerDefaultConflicts();
        this.ForbidReadOnlyConflicts();
        this.ForbidValueChangeConflicts();
      }

      private void TrimPropertyValues() => this.m_fieldRuleModel.FriendlyName = FieldRuleModelValidatorService.FieldRuleModelValidator.SafeTrim(this.m_fieldRuleModel.FriendlyName);

      private void CheckForValidFieldsAndConvertToReferenceNames()
      {
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? (conditions.Any<RuleConditionModel>() ? 1 : 0) : 0) != 0)
        {
          foreach (RuleConditionModel condition in this.m_fieldRuleModel.Conditions)
          {
            FieldEntry fieldByNameOrRefName = this.CheckAndGetFieldByNameOrRefName(condition.Field, "condition.field");
            condition.Field = fieldByNameOrRefName.ReferenceName;
          }
        }
        foreach (RuleActionModel action in this.m_fieldRuleModel.Actions)
        {
          FieldEntry fieldByNameOrRefName1 = this.CheckAndGetFieldByNameOrRefName(action.TargetField, "action.targetField");
          action.TargetField = fieldByNameOrRefName1.ReferenceName;
          if (action.ActionType == "$setDefaultFromField" || action.ActionType == "$copyFromField")
          {
            FieldEntry fieldByNameOrRefName2 = this.CheckAndGetFieldByNameOrRefName(action.Value, "action.value");
            action.Value = fieldByNameOrRefName2.ReferenceName;
          }
        }
      }

      private void CheckEmbeddedImages()
      {
        IEnumerable<RuleActionModel> actions = this.m_fieldRuleModel.Actions;
        foreach (string document in (actions != null ? (IEnumerable<string>) actions.Where<RuleActionModel>((Func<RuleActionModel, bool>) (a => !string.IsNullOrEmpty(a.Value))).Select<RuleActionModel, string>((Func<RuleActionModel, string>) (a => a.Value)).ToList<string>() : (IEnumerable<string>) null) ?? Enumerable.Empty<string>())
        {
          if (ContentValidationUtil.ExtractBase64DataUriEmbeddedContent(document).Any<DataUriEmbeddedContent>())
            throw new EmbeddedBase64ImageNotSupportedException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemRulesDoNotSupportEmbeddedImages());
        }
      }

      private void CheckAndFixJsonDataTokens()
      {
        this.CheckFieldRuleJsonDataTokens();
        this.CheckAndFixConditionsJsonDataTokens();
        this.CheckAndFixActionsJsonDataTokens();
      }

      private void CheckFieldRuleJsonDataTokens()
      {
        Guid? id = this.m_fieldRuleModel.Id;
        Guid empty = Guid.Empty;
        if ((id.HasValue ? (id.HasValue ? (id.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
        {
          id = this.m_fieldRuleModel.Id;
          if (id.HasValue)
          {
            if (string.IsNullOrWhiteSpace(this.m_fieldRuleModel.FriendlyName))
              throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingRequiredProperty((object) "friendlyName"));
            FieldRuleModelValidatorService.FieldRuleModelValidator.CheckRuleName(this.m_fieldRuleModel.FriendlyName, 100);
            if (this.m_fieldRuleModel.Conditions != null)
              return;
            this.m_fieldRuleModel.Conditions = Enumerable.Empty<RuleConditionModel>();
            return;
          }
        }
        throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingRequiredProperty((object) "id"));
      }

      private void CheckAndFixConditionsJsonDataTokens()
      {
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? (conditions.Any<RuleConditionModel>() ? 1 : 0) : 0) == 0)
          return;
        foreach (RuleConditionModel condition in this.m_fieldRuleModel.Conditions)
          this.CheckAndFixConditionJsonDataTokens(condition);
      }

      private void CheckAndFixConditionJsonDataTokens(RuleConditionModel condition)
      {
        if (string.IsNullOrWhiteSpace(condition.ConditionType))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingRequiredProperty((object) "condition.conditionType"));
        if (string.IsNullOrWhiteSpace(condition.Field))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingRequiredProperty((object) "condition.field"));
        switch (condition.ConditionType)
        {
          case "$when":
          case "$whenWas":
          case "$whenNot":
            if (condition.Value == null)
              throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingRequiredPropertyFor((object) "condition.value", (object) condition.ConditionType));
            this.CheckValidStateValues(condition);
            break;
          case "$whenChanged":
          case "$whenNotChanged":
            condition.Value = (string) null;
            break;
          default:
            throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.UnrecognizedPropertyValue((object) "condition.conditionType", (object) condition.ConditionType));
        }
      }

      private void CheckValidStateValues(RuleConditionModel condition)
      {
        if (this.m_fieldNameComparer.Equals(condition.Field, "System.State") && (!(condition.ConditionType == "$whenWas") || !string.IsNullOrEmpty(condition.Value)) && !this.m_stateDefinitions.Any<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => TFStringComparer.WorkItemStateName.Equals(s.Name, condition.Value))))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.UnrecognizedPropertyValue((object) "condition.value", (object) condition.Value));
      }

      private void CheckAndFixActionsJsonDataTokens()
      {
        if (this.m_fieldRuleModel.Actions == null)
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingRequiredProperty((object) "action"));
        foreach (RuleActionModel action in this.m_fieldRuleModel.Actions)
        {
          string str = !string.IsNullOrWhiteSpace(action.ActionType) ? action.ActionType : throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingRequiredProperty((object) "action.actionType"));
          if (str != null)
          {
            switch (str.Length)
            {
              case 10:
                if (str == "$copyValue")
                  goto label_28;
                else
                  goto label_36;
              case 13:
                switch (str[7])
                {
                  case 'a':
                    if (str == "$makeReadOnly")
                      break;
                    goto label_36;
                  case 'q':
                    if (str == "$makeRequired")
                      break;
                    goto label_36;
                  default:
                    goto label_36;
                }
                break;
              case 14:
                switch (str[9])
                {
                  case 'C':
                    if (str == "$copyFromClock")
                      break;
                    goto label_36;
                  case 'F':
                    if (str == "$copyFromField")
                      goto label_28;
                    else
                      goto label_36;
                  case 'V':
                    if (str == "$disallowValue")
                    {
                      if (action.Value == null)
                        throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.DisallowValueUsedIncorrectly());
                      if (action.TargetField != "System.State")
                        throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.DisallowValueUsedIncorrectly());
                      IEnumerable<RuleConditionModel> source1 = this.m_fieldRuleModel.Conditions.Where<RuleConditionModel>((Func<RuleConditionModel, bool>) (c => c.ConditionType == "$whenWas" && c.Field == "System.State" && !string.IsNullOrEmpty(c.Value)));
                      IEnumerable<RuleConditionModel> source2 = this.m_fieldRuleModel.Conditions.Where<RuleConditionModel>((Func<RuleConditionModel, bool>) (c => c.ConditionType == "$when" && c.Field == "System.State"));
                      if (action.ForVsId == Guid.Empty && action.NotVsId == Guid.Empty && source1.Count<RuleConditionModel>() != 1 || source2.Count<RuleConditionModel>() != 0)
                        throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.DisallowValueUsedIncorrectly());
                      continue;
                    }
                    goto label_36;
                  default:
                    goto label_36;
                }
                break;
              case 16:
                switch (str[4])
                {
                  case 'D':
                    if (str == "$setDefaultValue")
                      goto label_28;
                    else
                      goto label_36;
                  case 'V':
                    if (str == "$setValueToEmpty")
                      break;
                    goto label_36;
                  case 'e':
                    if (str == "$hideTargetField")
                      break;
                    goto label_36;
                  default:
                    goto label_36;
                }
                break;
              case 20:
                switch (str[9])
                {
                  case 'C':
                    if (str == "$copyFromCurrentUser")
                      break;
                    goto label_36;
                  case 'S':
                    if (str == "$copyFromServerClock")
                      break;
                    goto label_36;
                  case 'l':
                    switch (str)
                    {
                      case "$setDefaultFromClock":
                        break;
                      case "$setDefaultFromField":
                        goto label_28;
                      default:
                        goto label_36;
                    }
                    break;
                  default:
                    goto label_36;
                }
                break;
              case 26:
                switch (str[1])
                {
                  case 'c':
                    if (str == "$copyFromServerCurrentUser")
                      break;
                    goto label_36;
                  case 's':
                    if (str == "$setDefaultFromCurrentUser")
                      break;
                    goto label_36;
                  default:
                    goto label_36;
                }
                break;
              default:
                goto label_36;
            }
            action.Value = (string) null;
            continue;
label_28:
            if (action.Value == null)
              throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingRequiredPropertyFor((object) "action.value", (object) action.ActionType));
            continue;
          }
label_36:
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.UnrecognizedPropertyValue((object) "action.actionType", (object) action.ActionType));
        }
      }

      private void CheckAndFixValidCharacters()
      {
        this.CheckAndTransformValidCharactersInConditions();
        this.CheckValidCharactersInActions();
      }

      private void CheckAndTransformValidCharactersInConditions()
      {
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? (conditions.Any<RuleConditionModel>() ? 1 : 0) : 0) == 0)
          return;
        foreach (RuleConditionModel condition in this.m_fieldRuleModel.Conditions)
        {
          FieldEntry fieldByNameOrRefName = this.CheckAndGetFieldByNameOrRefName(condition.Field, "condition.field");
          string conditionType = condition.ConditionType;
          if (conditionType == "$when" || conditionType == "$whenWas" || conditionType == "$whenNot")
            condition.Value = this.ValidateAndTransformValueString(fieldByNameOrRefName, condition.Value, RuleValueFrom.Value, "condition.value");
        }
      }

      private void CheckValidCharactersInActions()
      {
        foreach (RuleActionModel action in this.m_fieldRuleModel.Actions)
        {
          string actionType = action.ActionType;
          if (actionType == "$setDefaultValue" || actionType == "$copyValue")
            this.ValidateAndTransformValueString(this.CheckAndGetFieldByNameOrRefName(action.TargetField, "action.TargetField"), action.Value, RuleValueFrom.Value, "action.value");
        }
      }

      private string ValidateAndTransformValueString(
        FieldEntry fieldEntry,
        string value,
        RuleValueFrom ruleValueFrom,
        string propertyName)
      {
        IVssRequestContext requestContext = this.m_requestContext;
        FieldEntry field = fieldEntry;
        string str = value;
        WorkItemFieldRule fieldRule = this.WitRulesIncludingGivenRule.FirstOrDefault<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => r.FieldId == fieldEntry.FieldId));
        int valueFrom = (int) ruleValueFrom;
        Guid processId1 = this.m_processId;
        string witReferenceName = this.m_witReferenceName;
        Guid identityVsid = new Guid();
        Guid processId2 = processId1;
        string witRefName = witReferenceName;
        RuleValueValidationResult validationResult = RuleValueValidator.ValidateAndTransformRuleValue(requestContext, field, str, fieldRule, (RuleValueFrom) valueFrom, identityVsid: identityVsid, processId: processId2, witRefName: witRefName);
        switch (validationResult.Status)
        {
          case RuleValueValidationStatus.Valid:
            return validationResult.TransformedValue;
          case RuleValueValidationStatus.InvalidTooLong:
            throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RuleStringTooLong((object) propertyName, (object) (int) byte.MaxValue, (object) value.Length));
          default:
            throw new FieldRuleModelValidationException(validationResult.ErrorMessage);
        }
      }

      private void CheckForNotRuleHasNoConditionAndOnlyOneAction()
      {
        if (this.m_fieldRuleModel.Actions == null || !this.m_fieldRuleModel.Actions.Any<RuleActionModel>((Func<RuleActionModel, bool>) (a => a.ForVsId != Guid.Empty || a.NotVsId != Guid.Empty)))
          return;
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? (conditions.Count<RuleConditionModel>() > 0 ? 1 : 0) : 0) != 0)
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.WorkItemForNotRuleConditionInvalid((object) this.m_fieldRuleModel.Conditions.Count<RuleConditionModel>()));
        if (this.m_fieldRuleModel.Actions.Where<RuleActionModel>((Func<RuleActionModel, bool>) (a => a.ActionType != "$disallowValue")).Count<RuleActionModel>() > 1)
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.WorkItemForNotRuleActionInvalid((object) this.m_fieldRuleModel.Actions.Count<RuleActionModel>()));
      }

      private void CheckHideFieldRuleNoConditionAndOnlyForNotAction()
      {
        IEnumerable<RuleActionModel> actions = this.m_fieldRuleModel.Actions;
        IEnumerable<RuleActionModel> source = actions != null ? actions.Where<RuleActionModel>((Func<RuleActionModel, bool>) (a => string.Equals(a.ActionType, "$hideTargetField".ToString(), StringComparison.OrdinalIgnoreCase))) : (IEnumerable<RuleActionModel>) null;
        if (source == null || !source.Any<RuleActionModel>())
          return;
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? (conditions.Count<RuleConditionModel>() > 0 ? 1 : 0) : 0) != 0)
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.WorkItemInvalidHideFieldRuleCondition((object) this.m_fieldRuleModel.Conditions.Count<RuleConditionModel>()));
        if (this.m_fieldRuleModel.Actions.Count<RuleActionModel>() > 1)
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.WorkItemInvalidHideFieldRuleMultipleActions((object) this.m_fieldRuleModel.Actions.Count<RuleActionModel>()));
        RuleActionModel ruleActionModel = source.First<RuleActionModel>();
        if (ruleActionModel.ForVsId == Guid.Empty && ruleActionModel.NotVsId == Guid.Empty)
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.WorkItemInvalidHideFieldRuleCondition((object) 0));
      }

      private void ForbidSystemFieldsForHiddenRule()
      {
        IEnumerable<RuleActionModel> actions = this.m_fieldRuleModel.Actions;
        RuleActionModel ruleActionModel = actions != null ? actions.Where<RuleActionModel>((Func<RuleActionModel, bool>) (a => string.Equals(a.ActionType, "$hideTargetField".ToString(), StringComparison.OrdinalIgnoreCase))).FirstOrDefault<RuleActionModel>() : (RuleActionModel) null;
        if (ruleActionModel != null && this.m_requestContext.GetService<IProcessFieldService>().IsSystemField(ruleActionModel.TargetField))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.FieldNotAllowedInActions((object) ruleActionModel.TargetField));
      }

      private void CheckCombinationsOfTwoConditions()
      {
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? (conditions.Count<RuleConditionModel>() == 2 ? 1 : 0) : 0) == 0)
          return;
        bool flag = false;
        if (this.m_fieldRuleModel.Conditions.Any<RuleConditionModel>((Func<RuleConditionModel, bool>) (condition => condition.ConditionType == "$when" && this.m_fieldNameComparer.Equals(condition.Field, "System.State"))))
          flag = true;
        if (flag)
        {
          FieldRuleModelValidatorService.FieldRuleModelValidator.RequireOneMatchingCondition(FieldRuleModelValidatorService.FieldRuleModelValidator.RequireOneMatchingCondition(this.m_fieldRuleModel.Conditions, (Func<RuleConditionModel, bool>) (condition => condition.ConditionType == "$when" && this.m_fieldNameComparer.Equals(condition.Field, "System.State")), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.TwoConditionsRequirePattern((object) this.GetConditionsPatternAsString(), (object) "$when System.State")), (Func<RuleConditionModel, bool>) (condition => condition.ConditionType == "$whenWas" && this.m_fieldNameComparer.Equals(condition.Field, "System.State") || condition.ConditionType != "$whenWas"), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.TwoConditionsRequireEitherPattern((object) this.GetConditionsPatternAsString(), (object) "$whenWas System.State", (object) "not($whenWas)"));
        }
        else
        {
          FieldRuleModelValidatorService.FieldRuleModelValidator.RequireOneMatchingCondition(this.m_fieldRuleModel.Conditions, (Func<RuleConditionModel, bool>) (condition => condition.ConditionType == "$whenWas" && this.m_fieldNameComparer.Equals(condition.Field, "System.State") && string.IsNullOrEmpty(condition.Value)), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.TwoConditionsRequireEitherPattern((object) this.GetConditionsPatternAsString(), (object) "$when System.State", (object) "$whenWas) (System.State = empty)"));
          FieldRuleModelValidatorService.FieldRuleModelValidator.RequireOneMatchingCondition(this.m_fieldRuleModel.Conditions, (Func<RuleConditionModel, bool>) (condition => condition.ConditionType != "$whenWas" && !this.m_fieldNameComparer.Equals(condition.Field, "System.State")), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.TwoConditionsRequirePattern((object) this.GetConditionsPatternAsString(), (object) "not($whenWas) not(System.State)"));
        }
      }

      private void CheckCombinationsOfThreeConditions()
      {
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? (conditions.Count<RuleConditionModel>() == 3 ? 1 : 0) : 0) == 0)
          return;
        FieldRuleModelValidatorService.FieldRuleModelValidator.RequireOneMatchingCondition(FieldRuleModelValidatorService.FieldRuleModelValidator.RequireOneMatchingCondition(FieldRuleModelValidatorService.FieldRuleModelValidator.RequireOneMatchingCondition(this.m_fieldRuleModel.Conditions, (Func<RuleConditionModel, bool>) (condition => condition.ConditionType == "$when" && this.m_fieldNameComparer.Equals(condition.Field, "System.State")), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ThreeConditionsRequirePattern((object) this.GetConditionsPatternAsString(), (object) "$when System.State")), (Func<RuleConditionModel, bool>) (condition => condition.ConditionType == "$whenWas" && this.m_fieldNameComparer.Equals(condition.Field, "System.State")), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ThreeConditionsRequirePattern((object) this.GetConditionsPatternAsString(), (object) "$whenWas System.State")), (Func<RuleConditionModel, bool>) (condition => condition.ConditionType != "$whenWas" && !this.m_fieldNameComparer.Equals(condition.Field, "System.State")), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ThreeConditionsRequirePattern((object) this.GetConditionsPatternAsString(), (object) "not($whenWas) not(System.State)"));
      }

      private void ForbidStandaloneWhenWasConditionExceptForState()
      {
        IEnumerable<RuleConditionModel> conditions1 = this.m_fieldRuleModel.Conditions;
        RuleConditionModel ruleConditionModel = conditions1 != null ? conditions1.FirstOrDefault<RuleConditionModel>() : (RuleConditionModel) null;
        IEnumerable<RuleConditionModel> conditions2 = this.m_fieldRuleModel.Conditions;
        if ((conditions2 != null ? (conditions2.Count<RuleConditionModel>() == 1 ? 1 : 0) : 0) != 0 && ruleConditionModel.ConditionType == "$whenWas" && !this.m_fieldNameComparer.Equals(ruleConditionModel.Field, "System.State"))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ConditionTypeRequiresOtherCondition((object) "$whenWas"));
      }

      private void ForbidUnconditionalDefaultValue()
      {
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? new int?(conditions.Count<RuleConditionModel>()) : new int?()).GetValueOrDefault() == 0 && this.m_fieldRuleModel.Actions.Any<RuleActionModel>((Func<RuleActionModel, bool>) (a => a.ActionType == "$setDefaultValue")))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ActionTypeRequiresConditions((object) "$setDefaultValue"));
      }

      private void ForbidUnconditionalDefaultClock()
      {
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? new int?(conditions.Count<RuleConditionModel>()) : new int?()).GetValueOrDefault() == 0 && this.m_fieldRuleModel.Actions.Any<RuleActionModel>((Func<RuleActionModel, bool>) (a => a.ActionType == "$setDefaultFromClock")))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ActionTypeRequiresConditions((object) "$setDefaultFromClock"));
      }

      private void ForbidUnconditionalDefaultUser()
      {
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? new int?(conditions.Count<RuleConditionModel>()) : new int?()).GetValueOrDefault() == 0 && this.m_fieldRuleModel.Actions.Any<RuleActionModel>((Func<RuleActionModel, bool>) (a => a.ActionType == "$setDefaultFromCurrentUser")))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ActionTypeRequiresConditions((object) "$setDefaultFromCurrentUser"));
      }

      private void ForbidUnconditionalRequiredExceptFor_ForNotRule()
      {
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? new int?(conditions.Count<RuleConditionModel>()) : new int?()).GetValueOrDefault() == 0 && this.m_fieldRuleModel.Actions.Any<RuleActionModel>((Func<RuleActionModel, bool>) (a => a.ActionType == "$makeRequired" && a.ForVsId == Guid.Empty && a.NotVsId == Guid.Empty)))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ActionTypeRequiresConditions((object) "$makeRequired"));
      }

      private void ForbidFourConditions()
      {
        IEnumerable<RuleConditionModel> conditions = this.m_fieldRuleModel.Conditions;
        if ((conditions != null ? (conditions.Count<RuleConditionModel>() > 3 ? 1 : 0) : 0) != 0)
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RulesTooManyConditions((object) 3));
      }

      private void ForbidConditionDeniedFields() => this.m_fieldRuleModel.Conditions.ForEach<RuleConditionModel>((Action<RuleConditionModel>) (condition =>
      {
        if (this.m_fieldNameComparer.Equals(condition.Field, "System.Reason") || this.m_fieldNameComparer.Equals(condition.Field, "System.AreaPath") || this.m_fieldNameComparer.Equals(condition.Field, "System.IterationPath") || this.m_fieldNameComparer.Equals(condition.Field, "System.History") || this.m_fieldNameComparer.Equals(condition.Field, "System.Id") || this.m_fieldNameComparer.Equals(condition.Field, "System.IterationId") || this.m_fieldNameComparer.Equals(condition.Field, "System.NodeName") || this.m_fieldNameComparer.Equals(condition.Field, "System.Tags") || this.m_fieldNameComparer.Equals(condition.Field, "System.Watermark") || this.m_fieldNameComparer.Equals(condition.Field, "System.BoardColumn") || this.m_fieldNameComparer.Equals(condition.Field, "System.BoardColumnDone") || this.m_fieldNameComparer.Equals(condition.Field, "System.BoardLane") || this.m_fieldNameComparer.Equals(condition.Field, "System.CommentCount"))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.FieldNotAllowedInConditions((object) condition.Field));
      }));

      private void ForbidDateTimeValuesInConditions() => this.m_fieldRuleModel.Conditions.ForEach<RuleConditionModel>((Action<RuleConditionModel>) (condition =>
      {
        if ((condition.ConditionType == "$when" || condition.ConditionType == "$whenNot") && this.CheckAndGetFieldByNameOrRefName(condition.Field, "condition.field").FieldType == InternalFieldType.DateTime && condition.Value != "")
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ConditionsOnDateTimeValue());
      }));

      private void ForbidActionDeniedFields() => this.m_fieldRuleModel.Actions.ForEach<RuleActionModel>((Action<RuleActionModel>) (action =>
      {
        if (this.IsExplicitlyDeniedFieldsForActions(action.TargetField))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.FieldNotAllowedInActions((object) action.TargetField));
        if (action.ActionType == "$copyFromField" && this.IsExplicitlyDeniedFieldsForActions(action.Value))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.FieldNotAllowedInActions((object) action.Value));
      }));

      private void CheckCompatibleFieldActionTypes() => this.m_fieldRuleModel.Actions.ForEach<RuleActionModel>((Action<RuleActionModel>) (action =>
      {
        FieldEntry fieldByNameOrRefName1 = this.CheckAndGetFieldByNameOrRefName(action.TargetField, "action.targetField");
        if (action.ActionType == "$copyFromServerClock")
        {
          if (!this.isFieldTypeCompatibleForValueType(InternalFieldType.DateTime, fieldByNameOrRefName1.FieldType))
            throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.TargetFieldNotCompatibleWithFieldType((object) InternalFieldType.DateTime, (object) fieldByNameOrRefName1.Name));
        }
        else if (action.ActionType == "$copyFromCurrentUser")
        {
          if (!this.isFieldTypeCompatibleForValueType(InternalFieldType.Identity, fieldByNameOrRefName1.FieldType))
            throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.TargetFieldNotCompatibleWithFieldType((object) InternalFieldType.Identity, (object) fieldByNameOrRefName1.Name));
        }
        else
        {
          if (!(action.ActionType == "$copyFromField"))
            return;
          FieldEntry fieldByNameOrRefName2 = this.CheckAndGetFieldByNameOrRefName(action.Value, "action.value");
          int fieldType = (int) fieldByNameOrRefName2.FieldType;
          if (!this.isFieldTypeCompatibleForValueType(fieldByNameOrRefName2.FieldType, fieldByNameOrRefName1.FieldType))
            throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.TargetFieldNotCompatibleWithFieldType((object) fieldByNameOrRefName2.FieldType, (object) fieldByNameOrRefName1.Name));
        }
      }));

      private bool isFieldTypeCompatibleForValueType(
        InternalFieldType fromFieldType,
        InternalFieldType toFieldType)
      {
        Array array = Enum.GetValues(typeof (InternalFieldType));
        switch (fromFieldType)
        {
          case InternalFieldType.String:
            array = (Array) new InternalFieldType[4]
            {
              InternalFieldType.String,
              InternalFieldType.PicklistString,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.Integer:
            array = (Array) new InternalFieldType[8]
            {
              InternalFieldType.String,
              InternalFieldType.Integer,
              InternalFieldType.Double,
              InternalFieldType.PicklistString,
              InternalFieldType.PicklistInteger,
              InternalFieldType.PicklistDouble,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.DateTime:
            array = (Array) new InternalFieldType[4]
            {
              InternalFieldType.String,
              InternalFieldType.DateTime,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.PlainText:
            array = (Array) new InternalFieldType[2]
            {
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.Html:
            array = (Array) new InternalFieldType[2]
            {
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.TreePath:
            array = (Array) new InternalFieldType[4]
            {
              InternalFieldType.String,
              InternalFieldType.TreePath,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.History:
            array = (Array) new InternalFieldType[3]
            {
              InternalFieldType.History,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.Double:
            array = (Array) new InternalFieldType[8]
            {
              InternalFieldType.String,
              InternalFieldType.Integer,
              InternalFieldType.Double,
              InternalFieldType.PicklistString,
              InternalFieldType.PicklistInteger,
              InternalFieldType.PicklistDouble,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.Guid:
            array = (Array) new InternalFieldType[4]
            {
              InternalFieldType.String,
              InternalFieldType.Guid,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.Boolean:
            array = (Array) new InternalFieldType[4]
            {
              InternalFieldType.String,
              InternalFieldType.Boolean,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.Identity:
            array = (Array) new InternalFieldType[4]
            {
              InternalFieldType.String,
              InternalFieldType.Identity,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.PicklistInteger:
            array = (Array) new InternalFieldType[8]
            {
              InternalFieldType.String,
              InternalFieldType.Integer,
              InternalFieldType.Double,
              InternalFieldType.PicklistString,
              InternalFieldType.PicklistInteger,
              InternalFieldType.PicklistDouble,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.PicklistString:
            array = (Array) new InternalFieldType[4]
            {
              InternalFieldType.String,
              InternalFieldType.PicklistString,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
          case InternalFieldType.PicklistDouble:
            array = (Array) new InternalFieldType[8]
            {
              InternalFieldType.String,
              InternalFieldType.Integer,
              InternalFieldType.Double,
              InternalFieldType.PicklistString,
              InternalFieldType.PicklistInteger,
              InternalFieldType.PicklistDouble,
              InternalFieldType.PlainText,
              InternalFieldType.Html
            };
            break;
        }
        return Array.IndexOf(array, (object) toFieldType) >= 0;
      }

      private bool IsExplicitlyDeniedFieldsForActions(string fieldReferenceName) => this.m_fieldNameComparer.Equals(fieldReferenceName, "System.Reason") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.AreaPath") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.IterationPath") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.AreaId") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.Id") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.IterationId") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.NodeName") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.Tags") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.Watermark") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.BoardColumn") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.BoardColumnDone") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.BoardLane") || this.m_fieldNameComparer.Equals(fieldReferenceName, "System.CommentCount");

      private void CheckActionsLimitPerWorkItemType()
      {
        int numActions = 0;
        FieldRuleModelValidatorService.FieldRuleModelValidator.WalkEachSubRuleGroup((IEnumerable<WorkItemRule>) this.WitRulesIncludingGivenRule, (Action<IEnumerable<WorkItemRule>>) (nonRuleBlocks => numActions += nonRuleBlocks.Count<WorkItemRule>()));
        if (numActions > this.m_maxRulesPerWorkItemType)
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.WorkItemTooManyRules((object) this.m_maxRulesPerWorkItemType));
      }

      private void ForbidDuplicateActions() => FieldRuleModelValidatorService.FieldRuleModelValidator.WalkEachSubRuleGroup((IEnumerable<WorkItemRule>) this.WitRulesForAffectedFieldsIncludingGivenRule, (Action<IEnumerable<WorkItemRule>>) (nonRuleBlocks =>
      {
        if (nonRuleBlocks.Count<WorkItemRule>() < 2)
          return;
        IGrouping<Tuple<WorkItemRuleName, Guid, Guid>, WorkItemRule> source = nonRuleBlocks.GroupBy<WorkItemRule, Tuple<WorkItemRuleName, Guid, Guid>>((Func<WorkItemRule, Tuple<WorkItemRuleName, Guid, Guid>>) (r => new Tuple<WorkItemRuleName, Guid, Guid>(r.Name, r.ForVsId, r.NotVsId))).Where<IGrouping<Tuple<WorkItemRuleName, Guid, Guid>, WorkItemRule>>((Func<IGrouping<Tuple<WorkItemRuleName, Guid, Guid>, WorkItemRule>, bool>) (e => e.Count<WorkItemRule>() >= 2)).FirstOrDefault<IGrouping<Tuple<WorkItemRuleName, Guid, Guid>, WorkItemRule>>();
        if (source != null)
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.DuplicateRuleAction((object) ("'" + string.Join("', '", source.Select<WorkItemRule, string>((Func<WorkItemRule, string>) (r => r.FriendlyName))) + "'"), (object) source.Key.ToString()));
      }));

      private void ForbidServerDefaultConflicts() => FieldRuleModelValidatorService.FieldRuleModelValidator.WalkEachSubRuleGroup((IEnumerable<WorkItemRule>) this.WitRulesForAffectedFieldsIncludingGivenRule, (Action<IEnumerable<WorkItemRule>>) (nonRuleBlocks =>
      {
        if (nonRuleBlocks.Count<WorkItemRule>() >= 2 && nonRuleBlocks.Any<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is ServerDefaultRule)))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RuleActionNotAllowedOverSystemRule());
      }));

      private void ForbidReadOnlyConflicts()
      {
        if (WorkItemTrackingFeatureFlags.IsSetValueAndReadOnlyRuleEnabled(this.m_requestContext))
          FieldRuleModelValidatorService.FieldRuleModelValidator.WalkEachSubRuleGroup((IEnumerable<WorkItemRule>) this.WitRulesForAffectedFieldsIncludingGivenRule, (Action<IEnumerable<WorkItemRule>>) (nonRuleBlocks =>
          {
            if (nonRuleBlocks.Count<WorkItemRule>() < 2 || !nonRuleBlocks.Any<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is ReadOnlyRule)) || !nonRuleBlocks.Any<WorkItemRule>((Func<WorkItemRule, bool>) (r =>
            {
              switch (r)
              {
                case ReadOnlyRule _:
                case AllowExistingValueRule _:
                  return false;
                default:
                  return !(r is EmptyRule);
              }
            })))
              return;
            WorkItemRule workItemRule1 = nonRuleBlocks.First<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is ReadOnlyRule));
            foreach (WorkItemRule workItemRule2 in nonRuleBlocks.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r =>
            {
              switch (r)
              {
                case ReadOnlyRule _:
                case AllowExistingValueRule _:
                  return false;
                default:
                  return !(r is EmptyRule);
              }
            })).ToArray<WorkItemRule>())
            {
              if (WorkItemTrackingFeatureFlags.IsForNotReadOnlyRuleDisabled(this.m_requestContext) || workItemRule1.ForVsId == workItemRule2.ForVsId && workItemRule1.NotVsId == workItemRule2.NotVsId)
                throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RuleActionConflictsWithAction((object) WorkItemRuleName.ReadOnly, (object) workItemRule2.Name));
            }
          }));
        else
          FieldRuleModelValidatorService.FieldRuleModelValidator.WalkEachSubRuleGroup((IEnumerable<WorkItemRule>) this.WitRulesForAffectedFieldsIncludingGivenRule, (Action<IEnumerable<WorkItemRule>>) (nonRuleBlocks =>
          {
            if (nonRuleBlocks.Count<WorkItemRule>() < 2 || !nonRuleBlocks.Any<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is ReadOnlyRule)) || !nonRuleBlocks.Any<WorkItemRule>((Func<WorkItemRule, bool>) (r => !(r is ReadOnlyRule) && !(r is AllowExistingValueRule))))
              return;
            WorkItemRule workItemRule3 = nonRuleBlocks.First<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is ReadOnlyRule));
            foreach (WorkItemRule workItemRule4 in nonRuleBlocks.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => !(r is ReadOnlyRule) && !(r is AllowExistingValueRule))).ToArray<WorkItemRule>())
            {
              if (WorkItemTrackingFeatureFlags.IsForNotReadOnlyRuleDisabled(this.m_requestContext) || workItemRule3.ForVsId == workItemRule4.ForVsId && workItemRule3.NotVsId == workItemRule4.NotVsId)
                throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RuleActionConflictsWithAction((object) WorkItemRuleName.ReadOnly, (object) workItemRule4.Name));
            }
          }));
      }

      private void ForbidValueChangeConflicts()
      {
        foreach (WorkItemFieldRule workItemFieldRule in this.WitRulesForAffectedFieldsIncludingGivenRule)
        {
          WorkItemFieldRule fieldRule = workItemFieldRule;
          FieldRuleModelValidatorService.FieldRuleModelValidator.WalkEachSubRuleGroup((IEnumerable<WorkItemRule>) ((IEnumerable<WorkItemRule>) fieldRule.SubRules).ToList<WorkItemRule>(), (Action<IEnumerable<WorkItemRule>>) (nonRuleBlocks =>
          {
            if (nonRuleBlocks.Count<WorkItemRule>() < 2)
              return;
            IEnumerable<WorkItemRule> source = nonRuleBlocks.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r =>
            {
              switch (r)
              {
                case DefaultRule _:
                case CopyRule _:
                case IdentityDefaultRule _:
                case EmptyRule _:
                  return true;
                default:
                  return r is FrozenRule;
              }
            }));
            if (source.Count<WorkItemRule>() >= 2)
              throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.DuplicateValueChangeRules((object) string.Join<WorkItemRuleName>(", ", source.Select<WorkItemRule, WorkItemRuleName>((Func<WorkItemRule, WorkItemRuleName>) (r => r.Name))), (object) fieldRule.Field));
          }));
        }
      }

      private void CheckForDuplicateRuleDefinitions()
      {
        FieldRuleModel fieldRuleModel1 = this.m_fieldRuleModel.Clone();
        fieldRuleModel1.SortConditions();
        IDictionary<string, WorkItemRule> itemRuleDictionary = fieldRuleModel1.CreateFieldToWorkItemRuleDictionary(this.m_requestContext, this.m_processId);
        foreach (string key in (IEnumerable<string>) itemRuleDictionary.Keys)
        {
          string fieldReferenceName = key;
          WorkItemFieldRule workItemFieldRule = this.WitRulesExcludingGivenRule.FirstOrDefault<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => this.m_fieldNameComparer.Equals(fr.Field, fieldReferenceName)));
          Guid duplicateRuleId;
          if (workItemFieldRule != null && ((IEnumerable<WorkItemRule>) workItemFieldRule.SubRules).Any<WorkItemRule>() && workItemFieldRule.TryFindDuplicateRuleId<WorkItemRule>(itemRuleDictionary[fieldReferenceName], out duplicateRuleId))
          {
            FieldRuleModel fieldRuleModel2 = this.IdToModelMapIncludingGivenRule[duplicateRuleId];
            if (string.IsNullOrEmpty(fieldRuleModel2.FriendlyName))
              throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.CannotAddDuplicateRule((object) duplicateRuleId));
            throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.CannotAddDuplicateRuleWithName((object) fieldRuleModel2.FriendlyName));
          }
        }
      }

      private void CheckForDuplicateRuleFriendlyNames()
      {
        foreach (WorkItemRule workItemRule in this.WitRulesExcludingGivenRule)
          workItemRule.Walk((Func<WorkItemRule, bool>) (rule =>
          {
            if (!string.IsNullOrEmpty(rule.FriendlyName) && StringComparer.OrdinalIgnoreCase.Equals(rule.FriendlyName, this.m_fieldRuleModel.FriendlyName))
              throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.DuplicateRuleFriendlyName((object) rule.FriendlyName));
            return true;
          }));
      }

      private static string SafeTrim(string s) => !string.IsNullOrEmpty(s) ? s.Trim() : s;

      private static void CheckRuleName(string inputString, int maxLength)
      {
        if (inputString == null)
          return;
        if (inputString.Length > maxLength)
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RuleStringTooLong((object) Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RuleName(), (object) maxLength, (object) inputString.Length));
        if (ArgumentUtility.HasMismatchedSurrogates(inputString))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RuleStringContainsInvalidChars());
        if (FieldRuleModelValidatorService.s_invalidCharacters.IsMatch(inputString))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RuleStringContainsInvalidChars());
      }

      private static IEnumerable<RuleConditionModel> RequireOneMatchingCondition(
        IEnumerable<RuleConditionModel> conditions,
        Func<RuleConditionModel, bool> matches,
        string errorMessage)
      {
        IEnumerable<RuleConditionModel> source = conditions.Where<RuleConditionModel>((Func<RuleConditionModel, bool>) (c => !matches(c)));
        return source.Count<RuleConditionModel>() == conditions.Count<RuleConditionModel>() - 1 ? source : throw new FieldRuleModelValidationException(errorMessage);
      }

      private static void WalkEachSubRuleGroup(
        IEnumerable<WorkItemRule> workItemRules,
        Action<IEnumerable<WorkItemRule>> actionOnEachSubRuleGroup)
      {
        Queue<RuleBlock> ruleBlockQueue = new Queue<RuleBlock>(workItemRules.OfType<RuleBlock>());
        while (ruleBlockQueue.Count > 0)
        {
          WorkItemRule[] subRules = ruleBlockQueue.Dequeue().SubRules;
          IEnumerable<WorkItemRule> source = ((IEnumerable<WorkItemRule>) subRules).Where<WorkItemRule>((Func<WorkItemRule, bool>) (subRule => !(subRule is RuleBlock)));
          if (source.Any<WorkItemRule>())
            actionOnEachSubRuleGroup(source);
          foreach (RuleBlock ruleBlock in subRules.OfType<RuleBlock>())
            ruleBlockQueue.Enqueue(ruleBlock);
        }
      }

      private string GetConditionsPatternAsString() => string.Join(", ", this.m_fieldRuleModel.Conditions.Select<RuleConditionModel, string>((Func<RuleConditionModel, string>) (c => "[" + c.ConditionType + " " + this.CheckAndGetFieldByNameOrRefName(c.Field, "").Name + "]")));

      private FieldEntry CheckAndGetFieldByNameOrRefName(
        string fieldNameOrRefName,
        string propertyName)
      {
        if (string.IsNullOrWhiteSpace(fieldNameOrRefName))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingRequiredProperty((object) propertyName));
        FieldEntry field;
        if (!this.m_fieldService.TryGetField(this.m_requestContext, fieldNameOrRefName, out field))
          throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.UnknownFieldReferenceName((object) fieldNameOrRefName, (object) propertyName));
        return field;
      }

      private List<WorkItemFieldRule> GetWitRulesFromServiceExcludingGivenRule()
      {
        List<WorkItemFieldRule> list = this.m_processWorkItemTypeService.GetAllRulesForWorkItemType(this.m_requestContext, this.m_processId, this.m_witReferenceName, true).ToList<WorkItemFieldRule>();
        if (this.m_fieldRuleModel.Id.HasValue)
        {
          Guid? id = this.m_fieldRuleModel.Id;
          if (id.Value != Guid.Empty)
          {
            foreach (WorkItemFieldRule workItemFieldRule in list)
            {
              id = this.m_fieldRuleModel.Id;
              Guid ruleIdToRemove = id.Value;
              workItemFieldRule.RemoveRule(ruleIdToRemove);
            }
          }
        }
        return list;
      }

      private IDictionary<Guid, FieldRuleModel> GetIdToModelMapExcludingGivenRule() => this.GetSortedIdToFieldRuleModelDictionary((IEnumerable<WorkItemFieldRule>) this.GetWitRulesFromServiceExcludingGivenRule());

      private IDictionary<Guid, FieldRuleModel> GetIdToModelMapIncludingGivenRule()
      {
        List<WorkItemFieldRule> excludingGivenRule = this.GetWitRulesFromServiceExcludingGivenRule();
        IDictionary<string, WorkItemRule> itemRuleDictionary = this.m_fieldRuleModel.CreateFieldToWorkItemRuleDictionary(this.m_requestContext, this.m_processId);
        IEnumerable<RuleActionModel> actions = this.m_fieldRuleModel.Actions;
        foreach (string str in (actions != null ? actions.Select<RuleActionModel, string>((Func<RuleActionModel, string>) (a => a.TargetField)) : (IEnumerable<string>) null) ?? Enumerable.Empty<string>())
          ProcessWorkItemTypeService.MergeWorkItemRules(this.m_requestContext, excludingGivenRule, str, itemRuleDictionary[str]);
        return this.GetSortedIdToFieldRuleModelDictionary((IEnumerable<WorkItemFieldRule>) excludingGivenRule);
      }

      private IDictionary<Guid, FieldRuleModel> GetSortedIdToFieldRuleModelDictionary(
        IEnumerable<WorkItemFieldRule> workItemFieldRules)
      {
        IDictionary<Guid, FieldRuleModel> ruleModelDictionary = FieldRuleModelFactory.CreateIdToFieldRuleModelDictionary(workItemFieldRules);
        foreach (FieldRuleModel fieldRuleModel in (IEnumerable<FieldRuleModel>) ruleModelDictionary.Values)
          fieldRuleModel.SortConditions();
        return ruleModelDictionary;
      }

      private IEnumerable<WorkItemFieldRule> GetWitRulesExcludingGivenRule() => this.ConvertToWorkItemFieldRules((IEnumerable<FieldRuleModel>) this.IdToModelMapExcludingGivenRule.Values);

      private IEnumerable<WorkItemFieldRule> GetWitRulesIncludingGivenRule() => this.ConvertToWorkItemFieldRules((IEnumerable<FieldRuleModel>) this.IdToModelMapIncludingGivenRule.Values);

      private IEnumerable<WorkItemFieldRule> ConvertToWorkItemFieldRules(
        IEnumerable<FieldRuleModel> fieldRuleModels)
      {
        List<WorkItemFieldRule> allFieldRules = new List<WorkItemFieldRule>();
        foreach (FieldRuleModel fieldRuleModel in fieldRuleModels)
        {
          foreach (KeyValuePair<string, WorkItemRule> fieldToWorkItemRule in (IEnumerable<KeyValuePair<string, WorkItemRule>>) fieldRuleModel.CreateFieldToWorkItemRuleDictionary(this.m_requestContext, this.m_processId))
          {
            string key = fieldToWorkItemRule.Key;
            WorkItemRule ruleToAdd = fieldToWorkItemRule.Value;
            ProcessWorkItemTypeService.MergeWorkItemRules(this.m_requestContext, allFieldRules, key, ruleToAdd);
          }
        }
        return (IEnumerable<WorkItemFieldRule>) allFieldRules;
      }

      private IEnumerable<WorkItemFieldRule> GetWitRulesForAffectedFieldsIncludingGivenRule()
      {
        IEnumerable<RuleActionModel> actions = this.m_fieldRuleModel.Actions;
        IEnumerable<string> fieldReferenceNames = (actions != null ? actions.Select<RuleActionModel, string>((Func<RuleActionModel, string>) (action => action.TargetField)) : (IEnumerable<string>) null) ?? Enumerable.Empty<string>();
        return this.WitRulesIncludingGivenRule.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (rule => fieldReferenceNames.Any<string>((Func<string, bool>) (fieldRefName => this.m_fieldNameComparer.Equals(rule.Field, fieldRefName)))));
      }

      private IDictionary<Guid, FieldRuleModel> IdToModelMapExcludingGivenRule => this.m_idToModelMapExcludingGivenRule ?? (this.m_idToModelMapExcludingGivenRule = this.GetIdToModelMapExcludingGivenRule());

      private IDictionary<Guid, FieldRuleModel> IdToModelMapIncludingGivenRule => this.m_idToModelMapIncludingGivenRule ?? (this.m_idToModelMapIncludingGivenRule = this.GetIdToModelMapIncludingGivenRule());

      private IEnumerable<WorkItemFieldRule> WitRulesExcludingGivenRule => this.m_witRulesExcludingGivenRule ?? (this.m_witRulesExcludingGivenRule = this.GetWitRulesExcludingGivenRule());

      private IEnumerable<WorkItemFieldRule> WitRulesIncludingGivenRule => this.m_witRulesIncludingGivenRule ?? (this.m_witRulesIncludingGivenRule = this.GetWitRulesIncludingGivenRule());

      private IEnumerable<WorkItemFieldRule> WitRulesForAffectedFieldsIncludingGivenRule => this.m_witRulesForAffectedFieldsIncludingGivenRule ?? (this.m_witRulesForAffectedFieldsIncludingGivenRule = this.GetWitRulesForAffectedFieldsIncludingGivenRule());
    }
  }
}
