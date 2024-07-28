// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.FieldRuleModelDictionary
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories
{
  internal class FieldRuleModelDictionary : Dictionary<Guid, FieldRuleModel>
  {
    public void MergeWorkItemFieldRule(WorkItemFieldRule fieldRule)
    {
      foreach (WorkItemRule subRule in fieldRule.SubRules)
        this.MergeWorkItemRule(subRule, fieldRule.Field);
    }

    private void MergeWorkItemRule(
      WorkItemRule ruleNode,
      string fieldRefName,
      List<ConditionalBlockRule> parentConditions = null)
    {
      if (ruleNode is RuleBlock)
      {
        RuleBlock ruleBlock = ruleNode as RuleBlock;
        parentConditions = FieldRuleModelDictionary.PushRuleBlockToConditions(parentConditions, ruleBlock);
        foreach (WorkItemRule subRule in ruleBlock.SubRules)
          this.MergeWorkItemRule(subRule, fieldRefName, parentConditions);
        parentConditions = FieldRuleModelDictionary.PopRuleBlockToConditions(parentConditions, ruleBlock);
      }
      else
        this.AddOrMergeFieldRuleModel(ruleNode, fieldRefName, parentConditions);
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
      WorkItemRule ruleNode,
      string fieldRefName,
      List<ConditionalBlockRule> parentConditions)
    {
      Guid id = ruleNode.Id;
      IEnumerable<RuleActionModel> actions;
      if (id == Guid.Empty || !RuleActionModel.TryCreate(fieldRefName, ruleNode, out actions))
        return;
      FieldRuleModel fieldRuleModel1;
      if (this.TryGetValue(id, out fieldRuleModel1))
      {
        ((List<RuleActionModel>) fieldRuleModel1.Actions).AddRange(actions);
      }
      else
      {
        List<RuleConditionModel> ruleConditionModelList = new List<RuleConditionModel>();
        if (parentConditions != null)
        {
          foreach (ConditionalBlockRule parentCondition in parentConditions)
          {
            RuleConditionModel condition;
            if (RuleConditionModel.TryCreate(parentCondition, out condition))
              ruleConditionModelList.Add(condition);
          }
        }
        FieldRuleModel fieldRuleModel2 = new FieldRuleModel()
        {
          Id = new Guid?(id),
          FriendlyName = ruleNode.FriendlyName,
          Actions = (IEnumerable<RuleActionModel>) actions.ToList<RuleActionModel>(),
          Conditions = (IEnumerable<RuleConditionModel>) ruleConditionModelList,
          IsDisabled = ruleNode.IsDisabled,
          IsSystem = ruleNode.IsSystem.GetValueOrDefault()
        };
        this.Add(id, fieldRuleModel2);
      }
    }
  }
}
