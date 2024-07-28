// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  [DataContract]
  public class FieldRuleModel
  {
    [DataMember]
    public Guid? Id { get; set; }

    [DataMember]
    public string FriendlyName { get; set; }

    [DataMember]
    public IEnumerable<RuleConditionModel> Conditions { get; set; }

    [DataMember]
    public IEnumerable<RuleActionModel> Actions { get; set; }

    [DataMember]
    public bool IsDisabled { get; set; }

    [DataMember]
    public bool IsSystem { get; set; }

    public bool StrictEquals(FieldRuleModel other)
    {
      Guid? id = this.Id;
      Guid valueOrDefault1 = id.GetValueOrDefault();
      id = other.Id;
      Guid valueOrDefault2 = id.GetValueOrDefault();
      return !(valueOrDefault1 != valueOrDefault2) && StringComparer.Ordinal.Equals(this.FriendlyName, other.FriendlyName) && this.IsDisabled == other.IsDisabled && FieldRuleModel.UnorderedEquals<RuleConditionModel>(this.Conditions, other.Conditions) && FieldRuleModel.UnorderedEquals<RuleActionModel>(this.Actions, other.Actions);
    }

    public void SortConditions()
    {
      IEnumerable<RuleConditionModel> conditions = this.Conditions;
      if ((conditions != null ? (conditions.Count<RuleConditionModel>() >= 2 ? 1 : 0) : 0) == 0)
        return;
      List<RuleConditionModel> list = this.Conditions.ToList<RuleConditionModel>();
      list.Sort();
      this.Conditions = (IEnumerable<RuleConditionModel>) list;
    }

    public FieldRuleModel Clone() => JsonConvert.DeserializeObject<FieldRuleModel>(JsonConvert.SerializeObject((object) this, Formatting.None));

    public IDictionary<string, WorkItemRule> CreateFieldToWorkItemRuleDictionary(
      IVssRequestContext requestContext,
      Guid processId)
    {
      IDictionary<string, List<RuleActionModel>> dictionary = (IDictionary<string, List<RuleActionModel>>) this.Actions.GroupBy<RuleActionModel, string>((Func<RuleActionModel, string>) (a => a.TargetField)).ToDictionary<IGrouping<string, RuleActionModel>, string, List<RuleActionModel>>((Func<IGrouping<string, RuleActionModel>, string>) (g => g.Key), (Func<IGrouping<string, RuleActionModel>, List<RuleActionModel>>) (g => g.ToList<RuleActionModel>()));
      IDictionary<string, WorkItemRule> itemRuleDictionary = (IDictionary<string, WorkItemRule>) new Dictionary<string, WorkItemRule>();
      ProcessTypeletRuleValidationContext validationHelper = new ProcessTypeletRuleValidationContext(requestContext);
      IProcessFieldService service = requestContext.GetService<IProcessFieldService>();
      foreach (string key in (IEnumerable<string>) dictionary.Keys)
      {
        List<RuleActionModel> ruleActionModelList = dictionary[key];
        List<WorkItemRule> workItemRuleList1 = new List<WorkItemRule>();
        FieldEntry fieldEntry = service.EnsureFieldExists(requestContext, key);
        ProhibitedValuesRule prohibitedValuesRule = (ProhibitedValuesRule) null;
        foreach (RuleActionModel ruleActionModel in ruleActionModelList)
        {
          bool isIdentity = fieldEntry != null && fieldEntry.IsIdentity;
          if (fieldEntry != null)
            FieldRuleModel.ReplaceIdentityDisplayNamesWithVsid(requestContext, fieldEntry, ruleActionModel);
          if (ruleActionModel.ActionType == "$disallowValue")
          {
            if (prohibitedValuesRule == null)
              prohibitedValuesRule = (ProhibitedValuesRule) ruleActionModel.ToWorkItemRule(requestContext, this.Id.GetValueOrDefault(), this.FriendlyName, this.IsDisabled, isIdentity);
            else
              prohibitedValuesRule.Values.Add(ruleActionModel.Value);
          }
          else
            workItemRuleList1.Add(ruleActionModel.ToWorkItemRule(requestContext, this.Id.GetValueOrDefault(), this.FriendlyName, this.IsDisabled, isIdentity));
        }
        if (prohibitedValuesRule != null)
        {
          List<WorkItemRule> workItemRuleList2 = workItemRuleList1;
          WhenChangedRule whenChangedRule1 = new WhenChangedRule();
          whenChangedRule1.Field = CoreFieldReferenceNames.State;
          whenChangedRule1.SubRules = new WorkItemRule[1]
          {
            (WorkItemRule) prohibitedValuesRule
          };
          WhenChangedRule whenChangedRule2 = whenChangedRule1;
          workItemRuleList2.Add((WorkItemRule) whenChangedRule2);
        }
        RuleBlock innerBlock = (RuleBlock) null;
        RuleBlock ruleBlock = this.CreateConditionRuleBlock(out innerBlock);
        if (ruleBlock == null)
        {
          ruleBlock = new RuleBlock();
          innerBlock = ruleBlock;
        }
        innerBlock.SubRules = workItemRuleList1.ToArray();
        ruleBlock.FixFieldReferences((IRuleValidationContext) validationHelper);
        itemRuleDictionary.Add(key, (WorkItemRule) ruleBlock);
      }
      return itemRuleDictionary;
    }

    private static void ReplaceIdentityDisplayNamesWithVsid(
      IVssRequestContext requestContext,
      FieldEntry fieldEntry,
      RuleActionModel ruleActionModel)
    {
      if (!fieldEntry.IsIdentity || !string.Equals(ruleActionModel.ActionType, "$copyValue", StringComparison.OrdinalIgnoreCase) || Guid.TryParse(ruleActionModel.Value, out Guid _) || string.IsNullOrEmpty(ruleActionModel.Value))
        return;
      IEnumerable<IdentityConstantRecord> source = requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().SearchConstantIdentityRecords(requestContext, ruleActionModel.Value);
      if (!source.Any<IdentityConstantRecord>() || source.Count<IdentityConstantRecord>() != 1)
        return;
      ruleActionModel.Value = source.First<IdentityConstantRecord>().TeamFoundationId.ToString();
    }

    private static bool UnorderedEquals<T>(IEnumerable<T> objects1, IEnumerable<T> objects2) where T : IEquatable<T>
    {
      bool flag1 = objects1 == null || !objects1.Any<T>();
      bool flag2 = objects2 == null || !objects2.Any<T>();
      if (flag1 & flag2)
        return true;
      if (flag1 != flag2)
        return false;
      return objects1.Count<T>() == 1 && objects2.Count<T>() == 1 ? objects1.First<T>().Equals(objects2.First<T>()) : new HashSet<T>(objects1).SetEquals((IEnumerable<T>) new HashSet<T>(objects2));
    }

    private RuleBlock CreateConditionRuleBlock(out RuleBlock innerBlock)
    {
      innerBlock = (RuleBlock) null;
      if (this.Conditions == null)
        return (RuleBlock) null;
      RuleBlock conditionRuleBlock = (RuleBlock) null;
      foreach (RuleConditionModel condition in this.Conditions)
      {
        ConditionalBlockRule ruleCondition = condition.ToRuleCondition();
        if (innerBlock == null)
        {
          conditionRuleBlock = (RuleBlock) ruleCondition;
          innerBlock = (RuleBlock) ruleCondition;
        }
        else
        {
          innerBlock.SubRules = (WorkItemRule[]) new ConditionalBlockRule[1]
          {
            ruleCondition
          };
          innerBlock = (RuleBlock) ruleCondition;
        }
      }
      return conditionRuleBlock;
    }
  }
}
