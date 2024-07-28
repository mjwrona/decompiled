// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.ProcessRuleModelDictionary
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  internal class ProcessRuleModelDictionary : Dictionary<Guid, ProcessRule>
  {
    public void MergeWorkItemFieldRule(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      WorkItemFieldRule fieldRule)
    {
      foreach (WorkItemRule subRule in fieldRule.SubRules)
        this.MergeWorkItemRule(requestContext, processId, witRefName, subRule, fieldRule.Field);
    }

    private void MergeWorkItemRule(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      WorkItemRule ruleNode,
      string fieldRefName,
      List<ConditionalBlockRule> parentConditions = null)
    {
      if (ruleNode is RuleBlock)
      {
        RuleBlock ruleBlock = ruleNode as RuleBlock;
        parentConditions = ProcessRuleModelDictionary.PushRuleBlockToConditions(parentConditions, ruleBlock);
        foreach (WorkItemRule subRule in ruleBlock.SubRules)
          this.MergeWorkItemRule(requestContext, processId, witRefName, subRule, fieldRefName, parentConditions);
        parentConditions = ProcessRuleModelDictionary.PopRuleBlockToConditions(parentConditions, ruleBlock);
      }
      else
        this.AddOrMergeFieldRuleModel(requestContext, processId, witRefName, ruleNode, fieldRefName, parentConditions);
    }

    private static List<ConditionalBlockRule> PushRuleBlockToConditions(
      List<ConditionalBlockRule> conditions,
      RuleBlock ruleBlock)
    {
      if (ruleBlock is ConditionalBlockRule)
      {
        ConditionalBlockRule conditionalBlockRule = (ConditionalBlockRule) ruleBlock;
        if (conditions == null)
          conditions = new List<ConditionalBlockRule>()
          {
            conditionalBlockRule
          };
        else
          conditions.Add(conditionalBlockRule);
      }
      return conditions;
    }

    private static List<ConditionalBlockRule> PopRuleBlockToConditions(
      List<ConditionalBlockRule> conditions,
      RuleBlock ruleBlock)
    {
      if (ruleBlock is ConditionalBlockRule)
        conditions.RemoveAt(conditions.Count - 1);
      return conditions;
    }

    private void AddOrMergeFieldRuleModel(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      WorkItemRule ruleNode,
      string fieldRefName,
      List<ConditionalBlockRule> parentConditions)
    {
      Guid id = ruleNode.Id;
      IEnumerable<RuleAction> actions;
      if (id == Guid.Empty || !ProcessRuleModelDictionary.TryCreateAction(fieldRefName, ruleNode, out actions))
        return;
      ProcessRule processRule1;
      if (this.TryGetValue(id, out processRule1))
      {
        foreach (RuleAction action in actions)
          this.ConvertFieldIdsToRefNamesInAction(requestContext, action);
        ((List<RuleAction>) processRule1.Actions).AddRange(actions);
      }
      else
      {
        List<RuleCondition> ruleConditionList = new List<RuleCondition>();
        if (parentConditions != null)
        {
          foreach (ConditionalBlockRule parentCondition in parentConditions)
          {
            RuleCondition condition;
            if (ProcessRuleModelDictionary.TryCreateRule(parentCondition, out condition))
              ruleConditionList.Add(condition);
            else
              break;
          }
        }
        if (ruleNode.ForVsId != Guid.Empty)
          ruleConditionList.Add(new RuleCondition()
          {
            ConditionType = RuleConditionType.WhenCurrentUserIsMemberOfGroup,
            Value = ruleNode.ForVsId.ToString()
          });
        else if (ruleNode.NotVsId != Guid.Empty)
          ruleConditionList.Add(new RuleCondition()
          {
            ConditionType = RuleConditionType.WhenCurrentUserIsNotMemberOfGroup,
            Value = ruleNode.NotVsId.ToString()
          });
        ProcessRule processRule2 = new ProcessRule();
        processRule2.Id = id;
        processRule2.Name = ruleNode.FriendlyName;
        processRule2.Actions = (IEnumerable<RuleAction>) actions.ToList<RuleAction>();
        processRule2.Conditions = (IEnumerable<RuleCondition>) ruleConditionList;
        processRule2.IsDisabled = ruleNode.IsDisabled;
        processRule2.CustomizationType = ruleNode.IsSystem.GetValueOrDefault() ? Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.System : Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.Custom;
        processRule2.Url = ProcessRuleModelFactory.GetLocationUrlForWorkItemTypeRule(requestContext, processId, witRefName, id);
        ProcessRule processRule3 = processRule2;
        ProcessRule refNames = this.ConvertFieldIdsToRefNames(requestContext, processRule3);
        this.Add(id, refNames);
      }
    }

    public static bool TryCreateRule(
      ConditionalBlockRule conditionBlockRule,
      out RuleCondition condition)
    {
      RuleConditionType conditionType;
      string str;
      if (ProcessRuleModelDictionary.TryGetConditionType(conditionBlockRule, out conditionType, out str))
      {
        condition = new RuleCondition()
        {
          ConditionType = conditionType,
          Field = conditionBlockRule.Field,
          Value = str
        };
        return true;
      }
      condition = (RuleCondition) null;
      return false;
    }

    private static bool TryGetConditionType(
      ConditionalBlockRule conditionRule,
      out RuleConditionType conditionType,
      out string value)
    {
      conditionType = RuleConditionType.When;
      value = (string) null;
      switch (conditionRule)
      {
        case WhenRule _:
          conditionType = !conditionRule.Inverse ? RuleConditionType.When : RuleConditionType.WhenNot;
          value = conditionRule.Value;
          return true;
        case WhenChangedRule _:
          if (((IEnumerable<WorkItemRule>) conditionRule.SubRules).Any<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is ProhibitedValuesRule)))
            return false;
          conditionType = !conditionRule.Inverse ? RuleConditionType.WhenChanged : RuleConditionType.WhenNotChanged;
          return true;
        case WhenWasRule _:
          conditionType = conditionRule.Inverse ? RuleConditionType.When : RuleConditionType.WhenWas;
          value = conditionRule.Value;
          return true;
        default:
          return false;
      }
    }

    public static bool TryCreateAction(
      string targetField,
      WorkItemRule rule,
      out IEnumerable<RuleAction> actions)
    {
      RuleActionType actionType;
      IEnumerable<string> values;
      if (ProcessRuleModelDictionary.TryGetActionType(rule, out actionType, out values))
      {
        actions = (IEnumerable<RuleAction>) ProcessRuleModelDictionary.Create(targetField, actionType, values).ToList<RuleAction>();
        return true;
      }
      actions = (IEnumerable<RuleAction>) null;
      return false;
    }

    public static IEnumerable<RuleAction> Create(
      string targetField,
      RuleActionType actionType,
      IEnumerable<string> values)
    {
      foreach (string str in values)
        yield return new RuleAction()
        {
          ActionType = actionType,
          Value = str,
          TargetField = targetField
        };
    }

    private static bool TryGetActionType(
      WorkItemRule rule,
      out RuleActionType actionType,
      out IEnumerable<string> values)
    {
      values = Enumerable.Repeat<string>((string) null, 1);
      actionType = RuleActionType.SetDefaultValue;
      switch (rule)
      {
        case RequiredRule _:
          actionType = RuleActionType.MakeRequired;
          return true;
        case ReadOnlyRule _:
          actionType = RuleActionType.MakeReadOnly;
          return true;
        case HideFieldRule _:
          actionType = RuleActionType.HideTargetField;
          return true;
        case DefaultRule _:
          if (rule is IdentityDefaultRule)
            return false;
          DefaultRule defaultRule = rule as DefaultRule;
          if (defaultRule.ValueFrom == RuleValueFrom.Value)
          {
            actionType = RuleActionType.SetDefaultValue;
            values = Enumerable.Repeat<string>(defaultRule.Value, 1);
            return true;
          }
          if (defaultRule.ValueFrom == RuleValueFrom.Clock)
          {
            actionType = RuleActionType.SetDefaultFromClock;
            return true;
          }
          if (defaultRule.ValueFrom == RuleValueFrom.CurrentUser)
          {
            actionType = RuleActionType.SetDefaultFromCurrentUser;
            return true;
          }
          if (defaultRule.ValueFrom == RuleValueFrom.OtherFieldCurrentValue)
          {
            actionType = RuleActionType.SetDefaultFromField;
            values = Enumerable.Repeat<string>(defaultRule.Value, 1);
            return true;
          }
          break;
        case CopyRule _:
          CopyRule copyRule = rule as CopyRule;
          if (copyRule.ValueFrom == RuleValueFrom.Value)
          {
            actionType = RuleActionType.CopyValue;
            values = Enumerable.Repeat<string>(copyRule.Value, 1);
            return true;
          }
          if (copyRule.ValueFrom == RuleValueFrom.Clock)
          {
            actionType = RuleActionType.CopyFromClock;
            return true;
          }
          if (copyRule.ValueFrom == RuleValueFrom.CurrentUser)
          {
            actionType = RuleActionType.CopyFromCurrentUser;
            return true;
          }
          if (copyRule.ValueFrom == RuleValueFrom.OtherFieldCurrentValue)
          {
            actionType = RuleActionType.CopyFromField;
            values = Enumerable.Repeat<string>(copyRule.Value, 1);
            return true;
          }
          break;
        case EmptyRule _:
          actionType = RuleActionType.SetValueToEmpty;
          return true;
        case ServerDefaultRule _:
          ServerDefaultRule serverDefaultRule = rule as ServerDefaultRule;
          if (serverDefaultRule.From == ServerDefaultType.ServerDateTime)
          {
            actionType = RuleActionType.CopyFromServerClock;
            return true;
          }
          if (serverDefaultRule.From == ServerDefaultType.CallerIdentity)
          {
            actionType = RuleActionType.CopyFromServerCurrentUser;
            return true;
          }
          break;
        case ProhibitedValuesRule _:
          actionType = RuleActionType.DisallowValue;
          ProhibitedValuesRule prohibitedValuesRule = (ProhibitedValuesRule) rule;
          values = (IEnumerable<string>) prohibitedValuesRule.Values;
          return true;
      }
      return false;
    }

    private ProcessRule ConvertFieldIdsToRefNames(
      IVssRequestContext requestContext,
      ProcessRule processRule)
    {
      if (processRule != null && processRule.Actions != null)
      {
        foreach (RuleAction action in processRule.Actions)
          this.ConvertFieldIdsToRefNamesInAction(requestContext, action);
      }
      return processRule;
    }

    private void ConvertFieldIdsToRefNamesInAction(
      IVssRequestContext requestContext,
      RuleAction action)
    {
      int result;
      if (action.ActionType != RuleActionType.CopyFromField && action.ActionType != RuleActionType.SetDefaultFromField || !int.TryParse(action.Value, out result))
        return;
      FieldEntry fieldById = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldById(requestContext, result, new bool?(false));
      if (fieldById == null)
        return;
      action.Value = fieldById.ReferenceName;
    }
  }
}
