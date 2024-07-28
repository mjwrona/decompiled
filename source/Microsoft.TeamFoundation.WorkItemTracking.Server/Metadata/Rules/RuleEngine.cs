// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.RuleEngine
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Utility;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public class RuleEngine
  {
    private IDictionary<int, WorkItemFieldRule> m_fieldRules;
    private int[] m_defaultFieldEvaluationOrder;

    public RuleEngine(
      IEnumerable<WorkItemFieldRule> fieldRules,
      RuleEngineConfiguration configuration = null,
      IEnumerable<int> defaultFieldOrder = null)
    {
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemFieldRule>>(fieldRules, nameof (fieldRules));
      this.Configuration = configuration ?? RuleEngineConfiguration.ServerFull;
      this.m_fieldRules = (IDictionary<int, WorkItemFieldRule>) RuleEngine.GroupAndMergeFieldRules(fieldRules).ToDictionary<WorkItemFieldRule, int, WorkItemFieldRule>((Func<WorkItemFieldRule, int>) (fr => fr.FieldId), (Func<WorkItemFieldRule, WorkItemFieldRule>) (fr => fr));
      if (defaultFieldOrder != null)
        this.m_defaultFieldEvaluationOrder = defaultFieldOrder.ToArray<int>();
      else
        this.m_defaultFieldEvaluationOrder = fieldRules.Select<WorkItemFieldRule, int>((Func<WorkItemFieldRule, int>) (fr => fr.FieldId)).ToArray<int>();
    }

    internal IDictionary<int, WorkItemFieldRule> GetFieldRules() => (IDictionary<int, WorkItemFieldRule>) new ReadOnlyDictionary<int, WorkItemFieldRule>(this.m_fieldRules);

    public RuleEngineConfiguration Configuration { get; private set; }

    public void Evaluate(IRuleEvaluationContext evaluationContext, IEnumerable<int> fieldIds = null)
    {
      IVssRequestContext requestContext = evaluationContext.RequestContext;
      try
      {
        requestContext.TraceEnter(904641, "Services", "WorkItemService", "RuleEngine.Evaluate");
        ArgumentUtility.CheckForNull<IRuleEvaluationContext>(evaluationContext, nameof (evaluationContext));
        if (fieldIds == null)
          fieldIds = (IEnumerable<int>) ((ICollection<int>) this.m_defaultFieldEvaluationOrder ?? this.m_fieldRules.Keys);
        Dictionary<int, bool> evalStack = new Dictionary<int, bool>();
        foreach (int fieldId in fieldIds)
          this.EvaluateFieldRule(evaluationContext, fieldId, false, evalStack);
      }
      finally
      {
        requestContext.TraceLeave(904643, "Services", "WorkItemService", "RuleEngine.Evaluate");
      }
    }

    public void SetFieldValue(IRuleEvaluationContext evaluationContext, int fieldId, object value)
    {
      ArgumentUtility.CheckForNull<IRuleEvaluationContext>(evaluationContext, nameof (evaluationContext));
      object currentFieldValue = evaluationContext.GetCurrentFieldValue(fieldId);
      evaluationContext.SetFieldRuleEvalutionStatus(new FieldRuleEvalutionStatus()
      {
        FieldId = fieldId,
        Value = value,
        Flags = evaluationContext.GetFieldFlags(fieldId)
      });
      bool flag = evaluationContext.IsIdentityField(fieldId);
      object fieldValue2 = value;
      int num = flag ? 1 : 0;
      if (RuleEngine.FieldRuleEvaluator.CompareFieldValues(currentFieldValue, fieldValue2, true, num != 0))
        return;
      Dictionary<int, bool> evalStack = new Dictionary<int, bool>();
      this.EvaluateFieldRule(evaluationContext, fieldId, true, evalStack);
    }

    private void EvaluateFieldRule(
      IRuleEvaluationContext evaluationContext,
      int fieldId,
      bool valueLocked,
      Dictionary<int, bool> evalStack)
    {
      evalStack[fieldId] = true;
      object currentFieldValue = evaluationContext.GetCurrentFieldValue(fieldId);
      object originalFieldValue = evaluationContext.GetOriginalFieldValue(fieldId);
      RuleEngine.FieldRuleEvaluator fieldRuleEvaluator = new RuleEngine.FieldRuleEvaluator(this, evaluationContext, fieldId, currentFieldValue, originalFieldValue, valueLocked);
      WorkItemFieldRule fieldRule;
      if (this.m_fieldRules.TryGetValue(fieldId, out fieldRule))
        fieldRuleEvaluator.Evaluate((RuleBlock) fieldRule);
      fieldRuleEvaluator.Validate();
      evaluationContext.SetFieldRuleEvalutionStatus(fieldRuleEvaluator.EvaluationStatus);
      if ((valueLocked ? 1 : (!object.Equals(currentFieldValue, evaluationContext.GetCurrentFieldValue(fieldId)) ? 1 : 0)) != 0 && fieldRuleEvaluator.TriggerFields.Any<int>())
      {
        foreach (int triggerField in fieldRuleEvaluator.TriggerFields)
        {
          bool flag;
          if (!evalStack.TryGetValue(triggerField, out flag) || !flag)
            this.EvaluateFieldRule(evaluationContext, triggerField, false, evalStack);
        }
      }
      evalStack[fieldId] = false;
    }

    public static IEnumerable<WorkItemFieldRule> PrepareRulesForExecution(
      IEnumerable<WorkItemFieldRule> rules,
      bool addInverseRules = true)
    {
      Dictionary<int, WorkItemFieldRule> ruleDict = rules.ToDictionary<WorkItemFieldRule, int, WorkItemFieldRule>((Func<WorkItemFieldRule, int>) (fr => fr.FieldId), (Func<WorkItemFieldRule, WorkItemFieldRule>) (fr => (WorkItemFieldRule) fr.GetMergableClone()));
      foreach (WorkItemFieldRule rule1 in rules)
      {
        WorkItemFieldRule fieldRule = rule1;
        Stack<WorkItemRule> stack = new Stack<WorkItemRule>();
        fieldRule.Walk((WorkItemRule) null, (RuleVisitor) ((currentRule, parentRule) =>
        {
          if (addInverseRules && currentRule is MapRule mapRule2)
          {
            WorkItemFieldRule workItemFieldRule;
            if (!ruleDict.TryGetValue(mapRule2.FieldId, out workItemFieldRule))
            {
              workItemFieldRule = new WorkItemFieldRule()
              {
                FieldId = mapRule2.FieldId
              };
              ruleDict[mapRule2.FieldId] = workItemFieldRule;
            }
            RuleBlock ruleBlock = (RuleBlock) workItemFieldRule;
            if (stack.Count > 1)
            {
              foreach (WorkItemRule workItemRule in stack.Take<WorkItemRule>(stack.Count - 1).Reverse<WorkItemRule>())
              {
                RuleBlock mergableClone = workItemRule.GetMergableClone() as RuleBlock;
                mergableClone.ClearSubRules();
                ruleBlock = ruleBlock.AddRule<RuleBlock>(mergableClone);
              }
            }
            MapRule rule2 = mapRule2.Clone() as MapRule;
            rule2.Inverse = !rule2.Inverse;
            rule2.FieldId = fieldRule.FieldId;
            rule2.Field = fieldRule.Field;
            ruleBlock.AddRule<MapRule>(rule2);
          }
          stack.Push(currentRule);
          return true;
        }), (RuleVisitor) ((currentRule, parentRule) =>
        {
          stack.Pop();
          return true;
        }));
      }
      Dictionary<int, HashSet<int>> dictionary = new Dictionary<int, HashSet<int>>();
      foreach (KeyValuePair<int, WorkItemFieldRule> keyValuePair in ruleDict)
      {
        foreach (int key in keyValuePair.Value.GetDependencies().Select<RuleFieldDependency, int>((Func<RuleFieldDependency, int>) (rfd => rfd.FieldId)))
        {
          if (key != keyValuePair.Key)
          {
            HashSet<int> intSet;
            if (!dictionary.TryGetValue(key, out intSet))
            {
              intSet = new HashSet<int>();
              dictionary[key] = intSet;
            }
            intSet.Add(keyValuePair.Key);
          }
        }
      }
      int order = 0;
      Dictionary<int, int> triggerList = new Dictionary<int, int>();
      foreach (KeyValuePair<int, WorkItemFieldRule> keyValuePair in (IEnumerable<KeyValuePair<int, WorkItemFieldRule>>) ruleDict.OrderBy<KeyValuePair<int, WorkItemFieldRule>, int>((Func<KeyValuePair<int, WorkItemFieldRule>, int>) (pair => pair.Key)))
        RuleEngine.BuildTriggerList(triggerList, ref order, ruleDict, keyValuePair.Key);
      foreach (KeyValuePair<int, HashSet<int>> keyValuePair in dictionary)
      {
        WorkItemFieldRule workItemFieldRule;
        if (!ruleDict.TryGetValue(keyValuePair.Key, out workItemFieldRule))
        {
          workItemFieldRule = new WorkItemFieldRule()
          {
            FieldId = keyValuePair.Key
          };
          ruleDict[keyValuePair.Key] = workItemFieldRule;
        }
        workItemFieldRule.AddRule<TriggerRule>(new TriggerRule()
        {
          FieldIds = keyValuePair.Value.OrderBy<int, int>((Func<int, int>) (fieldId => triggerList[fieldId])).ToArray<int>()
        });
      }
      return triggerList.OrderBy<KeyValuePair<int, int>, int>((Func<KeyValuePair<int, int>, int>) (pair => pair.Value)).Select<KeyValuePair<int, int>, WorkItemFieldRule>((Func<KeyValuePair<int, int>, WorkItemFieldRule>) (pair => ruleDict[pair.Key]));
    }

    private static void BuildTriggerList(
      Dictionary<int, int> triggerList,
      ref int order,
      Dictionary<int, WorkItemFieldRule> rules,
      int fieldId)
    {
      if (triggerList.ContainsKey(fieldId))
        return;
      triggerList.Add(fieldId, int.MaxValue);
      WorkItemFieldRule workItemFieldRule;
      if (rules.TryGetValue(fieldId, out workItemFieldRule))
      {
        foreach (int fieldId1 in workItemFieldRule.GetDependencies().Select<RuleFieldDependency, int>((Func<RuleFieldDependency, int>) (rfd => rfd.FieldId)).Where<int>((Func<int, bool>) (id => id != fieldId)))
          RuleEngine.BuildTriggerList(triggerList, ref order, rules, fieldId1);
      }
      triggerList[fieldId] = order++;
    }

    public static IEnumerable<WorkItemFieldRule> GroupAndMergeFieldRules(
      IEnumerable<WorkItemFieldRule> rules)
    {
      return rules.GroupBy<WorkItemFieldRule, int, WorkItemFieldRule>((Func<WorkItemFieldRule, int>) (fr => fr.FieldId), (Func<WorkItemFieldRule, WorkItemFieldRule>) (fr => fr)).Select<IGrouping<int, WorkItemFieldRule>, WorkItemFieldRule>((Func<IGrouping<int, WorkItemFieldRule>, WorkItemFieldRule>) (g =>
      {
        WorkItemFieldRule workItemFieldRule1 = (WorkItemFieldRule) null;
        WorkItemFieldRule workItemFieldRule2 = (WorkItemFieldRule) null;
        foreach (WorkItemFieldRule workItemFieldRule3 in (IEnumerable<WorkItemFieldRule>) g)
        {
          if (workItemFieldRule2 == null)
          {
            workItemFieldRule2 = workItemFieldRule3;
          }
          else
          {
            if (workItemFieldRule1 == null)
            {
              workItemFieldRule1 = new WorkItemFieldRule()
              {
                FieldId = g.Key
              };
              workItemFieldRule1.AddRule<WorkItemRule>(workItemFieldRule2.GetMergableClone());
            }
            workItemFieldRule1.AddRule<WorkItemRule>(workItemFieldRule3.GetMergableClone());
          }
        }
        return workItemFieldRule1 ?? workItemFieldRule2;
      }));
    }

    internal struct ListEntry
    {
      public ISet<string> KnownValues { get; set; }

      public IEnumerable<ConstantSetReference> SetReferences { get; set; }
    }

    internal class FieldRuleEvaluator
    {
      private RuleEngine m_ruleEngine;
      private int m_fieldId;
      private InternalFieldType m_fieldType;
      private bool m_isIdentityField;
      private object m_value;
      private object m_originalValue;
      private bool m_valueLocked;
      private bool m_userChange;
      private RuleEnginePhase m_evalPhase;
      private List<int> m_triggerFields = new List<int>(8);
      private List<RuleEngine.ListEntry> m_allowedValuesList;
      private List<RuleEngine.ListEntry> m_suggestedValuesList;
      private List<RuleEngine.ListEntry> m_prohibitedValuesList;
      private List<RuleEngine.ListEntry> m_matchValuesList;
      private FieldRuleEvalutionStatus m_evalStatus;
      private object m_otherFieldValue;
      private bool m_notSameAs;
      private bool m_sameAs;
      private bool m_setByRule;
      private bool m_allowExistingValue;
      private bool m_readOnly;
      private bool m_required;
      private bool m_empty;
      private bool m_frozen;
      private bool m_cannotLoseValue;
      private bool m_validUser;
      private bool m_setByDefaultRule;
      private bool m_serverDefault;
      private bool m_setByComputedRule;
      private IRuleEvaluationContext m_evaluationContext;

      public FieldRuleEvaluator(
        RuleEngine ruleEngine,
        IRuleEvaluationContext evaluationContext,
        int fieldId,
        object currentFieldValue,
        object originalFieldValue,
        bool valueLocked)
      {
        this.m_ruleEngine = ruleEngine;
        this.m_evaluationContext = evaluationContext;
        this.m_fieldId = fieldId;
        this.m_value = currentFieldValue;
        this.m_originalValue = originalFieldValue;
        this.m_valueLocked = valueLocked;
        this.m_userChange = valueLocked;
        this.m_fieldType = this.m_evaluationContext.GetFieldType(this.m_fieldId);
        this.m_isIdentityField = this.m_evaluationContext.IsIdentityField(this.m_fieldId);
        this.m_evalStatus = new FieldRuleEvalutionStatus()
        {
          FieldId = this.m_fieldId,
          Value = this.m_value,
          Flags = FieldStatusFlags.None
        };
      }

      public IEnumerable<int> TriggerFields => this.m_triggerFields.Distinct<int>();

      public List<RuleEngine.ListEntry> AllowedValuesList
      {
        get
        {
          if (this.m_allowedValuesList == null)
            this.m_allowedValuesList = new List<RuleEngine.ListEntry>();
          return this.m_allowedValuesList;
        }
      }

      public List<RuleEngine.ListEntry> ProhibitedValuesList
      {
        get
        {
          if (this.m_prohibitedValuesList == null)
            this.m_prohibitedValuesList = new List<RuleEngine.ListEntry>();
          return this.m_prohibitedValuesList;
        }
      }

      public List<RuleEngine.ListEntry> SuggestedValuesList
      {
        get
        {
          if (this.m_suggestedValuesList == null)
            this.m_suggestedValuesList = new List<RuleEngine.ListEntry>();
          return this.m_suggestedValuesList;
        }
      }

      public List<RuleEngine.ListEntry> MatchValuesList
      {
        get
        {
          if (this.m_matchValuesList == null)
            this.m_matchValuesList = new List<RuleEngine.ListEntry>();
          return this.m_matchValuesList;
        }
      }

      public FieldRuleEvalutionStatus EvaluationStatus => this.m_evalStatus;

      private object GetValueFrom(int fieldId, RuleValueFrom ruleValueFrom, object value)
      {
        int fieldType = (int) this.m_evaluationContext.GetFieldType(fieldId);
        switch (ruleValueFrom)
        {
          case RuleValueFrom.Value:
            return value;
          case RuleValueFrom.CurrentValue:
            return this.m_value;
          case RuleValueFrom.OriginalValue:
            return this.m_originalValue;
          case RuleValueFrom.OtherFieldCurrentValue:
            return this.GetFieldValue(int.Parse(value as string), false);
          case RuleValueFrom.OtherFieldOriginalValue:
            return this.GetFieldValue(int.Parse(value as string), true);
          case RuleValueFrom.CurrentUser:
            return (object) this.m_evaluationContext.GetCurrentUser();
          case RuleValueFrom.Clock:
            return (object) DateTime.UtcNow;
          default:
            return value;
        }
      }

      private object GetFieldValue(int fieldId, bool oldValue) => oldValue ? this.m_evaluationContext.GetOriginalFieldValue(fieldId) : this.m_evaluationContext.GetCurrentFieldValue(fieldId);

      internal void Evaluate(RuleBlock fieldRule)
      {
        if (fieldRule == null)
          return;
        this.TryEvaluate(fieldRule, RuleEnginePhase.CopyRules);
        this.TryEvaluate(fieldRule, RuleEnginePhase.DefaultRules);
        this.TryEvaluate(fieldRule, RuleEnginePhase.OtherRules);
      }

      private void TryEvaluate(RuleBlock fieldRule, RuleEnginePhase phase)
      {
        if ((this.m_ruleEngine.Configuration.ExecutablePhases & phase & fieldRule.Phase) == RuleEnginePhase.None)
          return;
        this.m_evalPhase = phase;
        this.Block(fieldRule);
      }

      private void Block(RuleBlock block)
      {
        if (block == null)
          return;
        foreach (WorkItemRule orderedRule in block.GetOrderedRules(this.m_evaluationContext.RuleFilter))
        {
          if ((this.m_ruleEngine.Configuration.ExecutablePhases & orderedRule.Phase & this.m_evalPhase) != RuleEnginePhase.None)
          {
            switch (orderedRule.Name)
            {
              case WorkItemRuleName.Block:
              case WorkItemRuleName.Collection:
              case WorkItemRuleName.Project:
              case WorkItemRuleName.WorkItemType:
                this.Block((RuleBlock) orderedRule);
                continue;
              case WorkItemRuleName.Required:
                this.Required((RequiredRule) orderedRule);
                continue;
              case WorkItemRuleName.ReadOnly:
                this.ReadOnly((ReadOnlyRule) orderedRule);
                continue;
              case WorkItemRuleName.Empty:
                this.Empty((EmptyRule) orderedRule);
                continue;
              case WorkItemRuleName.Frozen:
                this.Frozen((FrozenRule) orderedRule);
                continue;
              case WorkItemRuleName.CannotLoseValue:
                this.CannotLoseValue((CannotLoseValueRuleRule) orderedRule);
                continue;
              case WorkItemRuleName.OtherField:
                this.SameAs((SameAsRule) orderedRule);
                continue;
              case WorkItemRuleName.ValidUser:
                this.ValidUser((ValidUserRule) orderedRule);
                continue;
              case WorkItemRuleName.AllowExistingValue:
                this.AllowExistingValue((AllowExistingValueRule) orderedRule);
                continue;
              case WorkItemRuleName.Match:
                this.Match((MatchRule) orderedRule);
                continue;
              case WorkItemRuleName.AllowedValues:
                this.AllowedValues((AllowedValuesRule) orderedRule);
                continue;
              case WorkItemRuleName.SuggestedValues:
                this.SuggestedValues((SuggestedValuesRule) orderedRule);
                continue;
              case WorkItemRuleName.ProhibitedValues:
                this.ProhibitedValues((ProhibitedValuesRule) orderedRule);
                continue;
              case WorkItemRuleName.Default:
                this.Default((DefaultRule) orderedRule);
                continue;
              case WorkItemRuleName.Copy:
                this.Copy((CopyRule) orderedRule);
                continue;
              case WorkItemRuleName.ServerDefault:
                this.ServerDefault((ServerDefaultRule) orderedRule);
                continue;
              case WorkItemRuleName.Map:
                this.Map((MapRule) orderedRule);
                continue;
              case WorkItemRuleName.When:
                this.When((WhenRule) orderedRule);
                continue;
              case WorkItemRuleName.WhenWas:
                this.WhenWas((WhenWasRule) orderedRule);
                continue;
              case WorkItemRuleName.WhenChanged:
                this.WhenChanged((WhenChangedRule) orderedRule);
                continue;
              case WorkItemRuleName.WhenBecameNonEmpty:
                this.WhenBecameNonEmpty((WhenBecameNonEmptyRule) orderedRule);
                continue;
              case WorkItemRuleName.WhenRemainedNonEmpty:
                this.WhenRemainedNonEmpty((WhenRemainedNonEmptyRule) orderedRule);
                continue;
              case WorkItemRuleName.Computed:
                this.Computed((ComputedRule) orderedRule);
                continue;
              case WorkItemRuleName.Trigger:
                this.Trigger((TriggerRule) orderedRule);
                continue;
              default:
                continue;
            }
          }
        }
      }

      private void When(WhenRule rule)
      {
        object valueFrom = this.GetValueFrom(rule.FieldId, rule.ValueFrom, (object) rule.Value);
        bool useIdentityFieldComparison = this.IsIdentityField(rule.FieldId) && rule.ValueFrom == RuleValueFrom.Value;
        if ((RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.GetFieldValue(rule.FieldId, false), valueFrom, true, useIdentityFieldComparison) ? (!rule.Inverse ? 1 : 0) : (rule.Inverse ? 1 : 0)) == 0)
          return;
        this.Block((RuleBlock) rule);
      }

      private void WhenWas(WhenWasRule rule)
      {
        object valueFrom = this.GetValueFrom(rule.FieldId, rule.ValueFrom, (object) rule.Value);
        bool useIdentityFieldComparison = this.IsIdentityField(rule.FieldId) && rule.ValueFrom == RuleValueFrom.Value;
        if ((RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.GetFieldValue(rule.FieldId, true), valueFrom, true, useIdentityFieldComparison) ? (!rule.Inverse ? 1 : 0) : (rule.Inverse ? 1 : 0)) == 0)
          return;
        this.Block((RuleBlock) rule);
      }

      private void WhenChanged(WhenChangedRule rule)
      {
        if ((RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.GetFieldValue(rule.FieldId, false), this.GetFieldValue(rule.FieldId, true), true) ? (rule.Inverse ? 1 : 0) : (!rule.Inverse ? 1 : 0)) == 0)
          return;
        this.Block((RuleBlock) rule);
      }

      private void WhenBecameNonEmpty(WhenBecameNonEmptyRule rule)
      {
        if ((!RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.GetFieldValue(rule.FieldId, true), (object) null, true) || RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.GetFieldValue(rule.FieldId, false), (object) null, true) ? (rule.Inverse ? 1 : 0) : (!rule.Inverse ? 1 : 0)) == 0)
          return;
        this.Block((RuleBlock) rule);
      }

      private void WhenRemainedNonEmpty(WhenRemainedNonEmptyRule rule)
      {
        if ((RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.GetFieldValue(rule.FieldId, true), (object) null, true) || RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.GetFieldValue(rule.FieldId, false), (object) null, true) ? (rule.Inverse ? 1 : 0) : (!rule.Inverse ? 1 : 0)) == 0)
          return;
        this.Block((RuleBlock) rule);
      }

      private void SameAs(SameAsRule sameAsRule)
      {
        this.m_otherFieldValue = this.GetFieldValue(sameAsRule.FieldId, sameAsRule.CheckOriginalValue);
        if (sameAsRule.Inverse)
          this.m_notSameAs = true;
        else
          this.m_sameAs = true;
        if (!this.m_ruleEngine.Configuration.ApplyMakeTrueLogic || !this.m_sameAs || this.m_userChange || !sameAsRule.CheckOriginalValue)
          return;
        this.m_value = this.m_otherFieldValue;
        this.m_setByRule = true;
      }

      private void AllowExistingValue(AllowExistingValueRule allowExistingValueRule) => this.m_allowExistingValue = true;

      private void ReadOnly(ReadOnlyRule readOnlyRule)
      {
        this.m_readOnly = true;
        if (!this.m_ruleEngine.Configuration.ApplyMakeTrueLogic || this.m_userChange)
          return;
        this.m_value = this.m_originalValue;
        this.m_setByRule = true;
      }

      private void Empty(EmptyRule emptyRule)
      {
        this.m_empty = true;
        if (!this.m_ruleEngine.Configuration.ApplyMakeTrueLogic || this.m_userChange)
          return;
        this.m_value = (object) null;
        this.m_setByRule = true;
      }

      private void Required(RequiredRule requiredRule) => this.m_required = true;

      private void Frozen(FrozenRule frozenRule)
      {
        this.m_frozen = true;
        if (!this.m_ruleEngine.Configuration.ApplyMakeTrueLogic || this.m_userChange || RuleEngine.FieldRuleEvaluator.IsFieldEmpty(this.m_originalValue))
          return;
        this.m_value = this.m_originalValue;
        this.m_setByRule = true;
      }

      private void CannotLoseValue(CannotLoseValueRuleRule cannotLoseValueRuleRule) => this.m_cannotLoseValue = true;

      private void ValidUser(ValidUserRule validUserRule) => this.m_validUser = true;

      private void Copy(CopyRule copyRule)
      {
        if (this.m_valueLocked)
          return;
        this.m_value = this.GetValueFrom(this.m_fieldId, copyRule.ValueFrom, (object) copyRule.Value);
        this.m_valueLocked = true;
        this.m_setByRule = true;
      }

      private void Default(DefaultRule defaultRule)
      {
        if (this.m_valueLocked || !RuleEngine.FieldRuleEvaluator.IsFieldEmpty(this.m_value))
          return;
        this.m_value = this.GetValueFrom(this.m_fieldId, defaultRule.ValueFrom, (object) defaultRule.Value);
        this.m_valueLocked = true;
        this.m_setByRule = true;
        this.m_setByDefaultRule = true;
      }

      private void ServerDefault(ServerDefaultRule serverDefaultRule)
      {
        this.m_value = (object) new ServerDefaultFieldValue(serverDefaultRule.From);
        this.m_serverDefault = true;
        this.m_valueLocked = true;
        this.m_setByRule = true;
        this.m_setByDefaultRule = true;
      }

      private void Trigger(TriggerRule triggerRule)
      {
        if (triggerRule.FieldIds == null || !((IEnumerable<int>) triggerRule.FieldIds).Any<int>())
          return;
        this.m_triggerFields.AddRange((IEnumerable<int>) triggerRule.FieldIds);
      }

      private void Map(MapRule mapRule)
      {
        if (!mapRule.Inverse)
        {
          if (this.m_userChange)
            return;
          object fieldValue = this.GetFieldValue(mapRule.FieldId, false);
          MapValues mapValues = (MapValues) null;
          if (mapRule.Cases != null && ((IEnumerable<MapCase>) mapRule.Cases).Any<MapCase>())
          {
            foreach (MapCase mapCase in mapRule.Cases)
            {
              if (RuleEngine.FieldRuleEvaluator.CompareFieldValues(fieldValue, (object) mapCase.Value, true))
              {
                mapValues = (MapValues) mapCase;
                break;
              }
            }
          }
          if (mapValues == null)
            mapValues = mapRule.Else;
          if (mapValues == null)
            return;
          IEnumerable<string> source = (IEnumerable<string>) mapValues.Values ?? Enumerable.Empty<string>();
          if (source.FirstOrDefault<string>((Func<string, bool>) (v => RuleEngine.FieldRuleEvaluator.CompareFieldValues((object) v, this.m_value, true))) != null)
            return;
          if (mapValues.Default == null)
          {
            if (!source.Any<string>())
              return;
            this.m_value = (object) source.FirstOrDefault<string>();
            this.m_valueLocked = true;
            this.m_setByRule = true;
          }
          else
          {
            if (RuleEngine.FieldRuleEvaluator.CompareFieldValues((object) mapValues.Default, this.m_value, true))
              return;
            this.m_value = (object) mapValues.Default;
            this.m_valueLocked = true;
            this.m_setByRule = true;
          }
        }
        else
        {
          if (!this.m_ruleEngine.Configuration.ApplyInverseMapRules || this.m_userChange)
            return;
          object fieldValue = this.GetFieldValue(mapRule.FieldId, false);
          object fieldValue2 = this.m_evaluationContext.GetOriginalFieldValue(mapRule.FieldId) ?? this.GetFieldValue(mapRule.FieldId, false);
          if (RuleEngine.FieldRuleEvaluator.CompareFieldValues(fieldValue, fieldValue2, true))
            return;
          object fieldValue1 = (object) null;
          if (mapRule.Cases != null && ((IEnumerable<MapCase>) mapRule.Cases).Any<MapCase>())
          {
            foreach (MapCase mapCase in mapRule.Cases)
            {
              if (this.CompareMapCaseValues(fieldValue, mapCase.Values, true))
              {
                fieldValue1 = (object) mapCase.Value;
                break;
              }
            }
          }
          if (RuleEngine.FieldRuleEvaluator.CompareFieldValues(fieldValue1, this.m_value, true))
            return;
          this.m_value = fieldValue1;
          this.m_valueLocked = true;
          this.m_setByRule = true;
        }
      }

      private void Computed(ComputedRule computedRule)
      {
        object obj;
        if (this.m_valueLocked || !this.m_evaluationContext.TryComputeFieldValue(this.m_fieldId, computedRule.FieldId, out obj))
          return;
        this.m_value = obj;
        this.m_valueLocked = true;
        this.m_setByRule = true;
        this.m_setByComputedRule = true;
      }

      private void AllowedValues(AllowedValuesRule rule) => this.AllowedValuesList.Add(this.CreateListEntry((ListRule) rule));

      private void SuggestedValues(SuggestedValuesRule rule) => this.SuggestedValuesList.Add(this.CreateListEntry((ListRule) rule));

      private void ProhibitedValues(ProhibitedValuesRule rule) => this.ProhibitedValuesList.Add(this.CreateListEntry((ListRule) rule));

      private void Match(MatchRule rule) => this.MatchValuesList.Add(this.CreateListEntry((ListRule) rule));

      internal void Validate() => this.m_evaluationContext.RequestContext.TraceBlock(904521, 904530, 904525, "Services", "WorkItemService", "RuleEngine.Validate", (Action) (() =>
      {
        object internalValue;
        this.m_evalStatus.Flags |= FieldValueHelpers.TryConvertValueToInternal(this.m_value, this.m_fieldType, out internalValue);
        this.m_evalStatus.Value = !this.m_ruleEngine.Configuration.ExpandServerDefaultValue ? internalValue : this.m_evaluationContext.ServerDefaultValueTransformer.TransformValue(internalValue, this.m_fieldType);
        if (this.m_setByRule)
        {
          this.m_evalStatus.Flags |= FieldStatusFlags.SetByRule;
          if (this.m_isIdentityField)
          {
            string str = this.m_evaluationContext.ResolveIdentityValue(this.m_fieldId, this.m_evalStatus.Value);
            if (str != null && !this.m_readOnly)
              this.m_evalStatus.Value = (object) str;
          }
        }
        if (this.m_setByDefaultRule)
          this.m_evalStatus.Flags |= FieldStatusFlags.SetByDefaultRule;
        if (this.m_setByComputedRule)
          this.m_evalStatus.Flags |= FieldStatusFlags.SetByComputedRule;
        if (this.m_readOnly || this.m_serverDefault || this.m_empty)
          this.m_evalStatus.Flags |= FieldStatusFlags.ReadOnly;
        if (this.m_cannotLoseValue && !RuleEngine.FieldRuleEvaluator.IsFieldEmpty(this.m_originalValue))
          this.m_required = true;
        if (this.m_required)
          this.m_evalStatus.Flags |= FieldStatusFlags.Required;
        if (this.m_allowedValuesList != null && this.m_allowedValuesList.Count > 0)
          this.m_evalStatus.Flags |= FieldStatusFlags.HasValues | FieldStatusFlags.LimitedToValues;
        if (this.m_suggestedValuesList != null && this.m_suggestedValuesList.Count > 0)
        {
          this.m_evalStatus.Flags |= FieldStatusFlags.HasValues;
          this.m_evalStatus.Flags &= ~FieldStatusFlags.LimitedToValues;
        }
        if (this.m_matchValuesList != null && this.m_matchValuesList.Count > 0)
          this.m_evalStatus.Flags |= FieldStatusFlags.HasFormats;
        if (this.m_allowExistingValue || this.IsExistingIdentityFieldAmbiguous(this.m_evalStatus.Value))
          this.m_evalStatus.Flags |= FieldStatusFlags.AllowsOldValue;
        if (RuleEngine.FieldRuleEvaluator.IsFieldEmpty(this.m_evalStatus.Value))
        {
          if (this.m_empty)
            return;
          FieldStatusFlags fieldStatusFlags;
          if (this.m_required)
          {
            fieldStatusFlags = this.m_evalStatus.Flags;
          }
          else
          {
            switch (this.m_fieldId)
            {
              case -105:
                fieldStatusFlags = this.m_evaluationContext.GetFieldFlags(-104);
                break;
              case -7:
                fieldStatusFlags = this.m_evaluationContext.GetFieldFlags(-2);
                break;
              default:
                fieldStatusFlags = this.m_evalStatus.Flags;
                break;
            }
          }
          if ((fieldStatusFlags & FieldStatusFlags.Required) != FieldStatusFlags.None)
          {
            this.m_evalStatus.Flags |= FieldStatusFlags.Required;
            this.m_evalStatus.Flags |= FieldStatusFlags.InvalidEmpty;
          }
          else
          {
            if (!this.m_notSameAs && !this.m_sameAs)
              return;
            if (RuleEngine.FieldRuleEvaluator.IsFieldEmpty(this.m_otherFieldValue))
            {
              if (!this.m_notSameAs)
                return;
              this.m_evalStatus.Flags |= FieldStatusFlags.Required;
              this.m_evalStatus.Flags |= FieldStatusFlags.InvalidValueNotInOtherField;
            }
            else
            {
              if (!this.m_sameAs)
                return;
              this.m_evalStatus.Flags |= FieldStatusFlags.Required;
              this.m_evalStatus.Flags |= FieldStatusFlags.InvalidValueInOtherField;
            }
          }
        }
        else if (this.m_empty)
        {
          this.m_evalStatus.Flags &= ~FieldStatusFlags.ReadOnly;
          this.m_evalStatus.Flags |= FieldStatusFlags.InvalidNotEmpty;
        }
        else if (this.m_readOnly && !this.m_serverDefault && !this.m_setByDefaultRule && !RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.m_evalStatus.Value, this.m_originalValue, false, this.m_isIdentityField))
        {
          this.m_evalStatus.Flags |= FieldStatusFlags.InvalidNotOldValue;
        }
        else
        {
          if (this.m_sameAs || this.m_notSameAs)
          {
            if (RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.m_evalStatus.Value, this.m_otherFieldValue, false))
            {
              if (this.m_notSameAs)
              {
                this.m_evalStatus.Flags |= FieldStatusFlags.InvalidValueInOtherField;
                return;
              }
            }
            else if (this.m_sameAs)
            {
              this.m_evalStatus.Flags |= FieldStatusFlags.InvalidValueNotInOtherField;
              return;
            }
          }
          if (this.m_frozen && !RuleEngine.FieldRuleEvaluator.IsFieldEmpty(this.m_originalValue) && !RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.m_evalStatus.Value, this.m_originalValue, false))
          {
            this.m_evalStatus.Flags |= FieldStatusFlags.InvalidNotEmptyOrOldValue;
          }
          else
          {
            switch (this.m_fieldId)
            {
              case -105:
                if (!this.m_evaluationContext.IsIterationPathValid(this.m_evalStatus.Value as string))
                {
                  this.m_evalStatus.Flags |= FieldStatusFlags.InvalidPath;
                  return;
                }
                break;
              case -7:
                if (!this.m_evaluationContext.IsAreaPathValid(this.m_evalStatus.Value as string))
                {
                  this.m_evalStatus.Flags |= FieldStatusFlags.InvalidPath;
                  return;
                }
                break;
            }
            if (this.m_evalStatus.Value is ServerDefaultFieldValue)
              return;
            string value = RuleEngine.FieldRuleEvaluator.ConvertFieldValueToText(this.m_evalStatus.Value);
            int num = this.m_validUser ? 1 : 0;
            if (this.m_matchValuesList != null && this.m_matchValuesList.Count > 0)
            {
              ISet<string> source = RuleEngine.FieldRuleEvaluator.Intersect(this.m_matchValuesList.Select<RuleEngine.ListEntry, ISet<string>>((Func<RuleEngine.ListEntry, ISet<string>>) (le => le.KnownValues)));
              if (source.Count > 0 && !source.Any<string>((Func<string, bool>) (pattern => RuleEngine.FieldRuleEvaluator.IsMatch(pattern, value))))
              {
                this.m_evalStatus.Flags |= FieldStatusFlags.InvalidFormat;
                return;
              }
            }
            IList<IEnumerable<ConstantSetReference>> sets1 = (IList<IEnumerable<ConstantSetReference>>) null;
            if (this.m_prohibitedValuesList != null && !this.CheckValueAgainstLists(value, this.m_prohibitedValuesList, false, out sets1))
              this.m_evalStatus.Flags |= FieldStatusFlags.InvalidListValue;
            else if (this.m_allowExistingValue && !RuleEngine.FieldRuleEvaluator.IsFieldEmpty(this.m_originalValue) && RuleEngine.FieldRuleEvaluator.CompareFieldValues(this.m_evalStatus.Value, this.m_originalValue, true))
            {
              if (sets1 == null || sets1.Count <= 0)
                return;
              this.m_evalStatus.Flags |= FieldStatusFlags.PendingListCheck;
            }
            else
            {
              IList<IEnumerable<ConstantSetReference>> sets2 = (IList<IEnumerable<ConstantSetReference>>) null;
              if (this.m_allowedValuesList != null && !this.CheckValueAgainstLists(value, this.m_allowedValuesList, true, out sets2))
              {
                this.m_evalStatus.Flags |= FieldStatusFlags.InvalidListValue;
              }
              else
              {
                if ((sets1 == null || sets1.Count <= 0) && (sets2 == null || sets2.Count <= 0))
                  return;
                this.m_evalStatus.Flags |= FieldStatusFlags.PendingListCheck;
                this.m_evalStatus.PendingAllowedValueChecks = (IEnumerable<IEnumerable<ConstantSetReference>>) sets2;
                this.m_evalStatus.PendingProhibitedValueChecks = (IEnumerable<IEnumerable<ConstantSetReference>>) sets1;
              }
            }
          }
        }
      }));

      private bool CheckValueAgainstLists(
        string value,
        List<RuleEngine.ListEntry> listEntries,
        bool isAllowedValuesList,
        out IList<IEnumerable<ConstantSetReference>> sets)
      {
        IVssRequestContext requestContext = this.m_evaluationContext.RequestContext;
        try
        {
          requestContext.TraceEnter(904531, "Services", "WorkItemService", "RuleEngine.CheckValueAgainstLists");
          bool isIdentityField = this.IsIdentityField();
          sets = (IList<IEnumerable<ConstantSetReference>>) null;
          requestContext.Trace(904532, TraceLevel.Verbose, "Services", "WorkItemService", string.Format("RuleEngine.CheckValueAgainstLists for {0}. isIdentity: {1}: isAllowedList: {2}, isDistinctValue: {3}", (object) this.m_fieldId, (object) isIdentityField, (object) isAllowedValuesList, (object) value.Contains("<")));
          foreach (RuleEngine.ListEntry listEntry in listEntries)
          {
            IEnumerable<ConstantSetReference> constantSetReferences;
            ISet<string> stringSet;
            if (((!this.m_isIdentityField ? 0 : (listEntry.SetReferences != null ? 1 : 0)) & (isAllowedValuesList ? 1 : 0)) != 0 && listEntry.SetReferences.Any<ConstantSetReference>() && listEntry.SetReferences.Any<ConstantSetReference>((Func<ConstantSetReference, bool>) (set => set is ExtendedConstantSetRef)))
            {
              constantSetReferences = (IEnumerable<ConstantSetReference>) listEntry.SetReferences.Where<ConstantSetReference>((Func<ConstantSetReference, bool>) (set =>
              {
                if (!(set is ExtendedConstantSetRef extendedConstantSetRef2))
                  return true;
                return extendedConstantSetRef2.IsGroup && !set.Direct && set.TeamFoundationId != Guid.Empty;
              })).Distinct<ConstantSetReference>().ToList<ConstantSetReference>();
              stringSet = (ISet<string>) new HashSet<string>(listEntry.SetReferences.OfType<ExtendedConstantSetRef>().Where<ExtendedConstantSetRef>((Func<ExtendedConstantSetRef, bool>) (set => (!set.IsGroup || set.IsGroup && set.Direct && !set.ExcludeGroups) && set.DistinctDisplayName != null)).Select<ExtendedConstantSetRef, string>((Func<ExtendedConstantSetRef, string>) (set => set.DistinctDisplayName)).Concat<string>((IEnumerable<string>) listEntry.KnownValues).Distinct<string>());
            }
            else
            {
              constantSetReferences = listEntry.SetReferences;
              stringSet = listEntry.KnownValues;
            }
            if (isIdentityField & isAllowedValuesList)
            {
              string str1 = string.Empty;
              if (constantSetReferences != null)
                str1 = string.Join<ConstantSetReference>(";", constantSetReferences);
              string str2 = string.Empty;
              if (stringSet != null)
                str2 = string.Join(";", (IEnumerable<string>) stringSet);
              requestContext.Trace(904532, TraceLevel.Info, "Services", "WorkItemService", string.Format("{0}: {1} : Sets# {2}: Values# {3}", (object) this.m_fieldId, (object) value, (object) str1, (object) str2));
            }
            if (RuleEngine.FieldRuleEvaluator.IsMember<string>(stringSet, value))
            {
              if (!isAllowedValuesList)
                return false;
              requestContext.Trace(904532, TraceLevel.Verbose, "Services", "WorkItemService", string.Format("Validated membership via expanded values for {0}. isIdentity: {1}: isAllowedList: {2}", (object) this.m_fieldId, (object) isIdentityField, (object) isAllowedValuesList));
            }
            else if (constantSetReferences != null && constantSetReferences.Any<ConstantSetReference>())
            {
              IList<ConstantSetReference> pendingSets;
              bool flag = this.IsMember(value, constantSetReferences, isIdentityField, out pendingSets);
              if (pendingSets != null && pendingSets.Any<ConstantSetReference>())
              {
                if (sets == null)
                  sets = (IList<IEnumerable<ConstantSetReference>>) new List<IEnumerable<ConstantSetReference>>();
                sets.Add((IEnumerable<ConstantSetReference>) pendingSets);
                requestContext.Trace(904532, TraceLevel.Verbose, "Services", "WorkItemService", string.Format("Update pending value checks sets for {0}. isIdentity: {1}: isAllowedList: {2}", (object) this.m_fieldId, (object) isIdentityField, (object) isAllowedValuesList));
              }
              else
              {
                if (!(flag & isAllowedValuesList) && (flag || isAllowedValuesList))
                  return false;
                requestContext.Trace(904532, TraceLevel.Verbose, "Services", "WorkItemService", string.Format("Ignore membership check for {0}. isIdentity: {1}: isAllowedList: {2}", (object) this.m_fieldId, (object) isIdentityField, (object) isAllowedValuesList));
              }
            }
            else if (isAllowedValuesList)
              return false;
          }
          return true;
        }
        finally
        {
          requestContext.TraceLeave(904540, "Services", "WorkItemService", "RuleEngine.CheckValueAgainstLists");
        }
      }

      private RuleEngine.ListEntry CreateListEntry(ListRule rule) => new RuleEngine.ListEntry()
      {
        SetReferences = (IEnumerable<ConstantSetReference>) rule.Sets,
        KnownValues = (ISet<string>) rule.Values
      };

      private bool IsIdentityField() => this.m_evaluationContext.IsIdentityField(this.m_fieldId);

      private bool IsIdentityField(int fieldId) => this.m_evaluationContext.IsIdentityField(fieldId);

      private bool IsExistingIdentityFieldAmbiguous(object fieldValue) => this.IsIdentityField() && this.m_evaluationContext.IsIdentityFieldValueAmbiguous(fieldValue);

      private static bool IsFieldEmpty(object fieldValue)
      {
        if (fieldValue == null)
          return true;
        return !(fieldValue is ServerDefaultFieldValue) && string.IsNullOrEmpty(Convert.ToString(fieldValue, (IFormatProvider) CultureInfo.InvariantCulture));
      }

      public bool CompareMapCaseValues(object fieldValue1, string[] values, bool caseInsensitive)
      {
        if (values != null)
        {
          foreach (string fieldValue2 in values)
          {
            if (RuleEngine.FieldRuleEvaluator.CompareFieldValues(fieldValue1, (object) fieldValue2, caseInsensitive))
              return true;
          }
        }
        return false;
      }

      public static bool CompareFieldValues(
        object fieldValue1,
        object fieldValue2,
        bool caseInsensitive,
        bool useIdentityFieldComparison = false)
      {
        string x = !(fieldValue1 is DateTime dateTime2) ? (fieldValue1 == null ? "" : Convert.ToString(fieldValue1)) : dateTime2.ToString("u");
        string y = !(fieldValue2 is DateTime dateTime2) ? (fieldValue2 == null ? "" : Convert.ToString(fieldValue2)) : dateTime2.ToString("u");
        bool flag = !caseInsensitive ? StringComparer.Ordinal.Equals(x, y) : StringComparer.OrdinalIgnoreCase.Equals(x, y);
        if (!flag & useIdentityFieldComparison)
          flag = RuleEngine.FieldRuleEvaluator.DoUniqueIdentityComponentsMatch(x, y);
        return flag;
      }

      private static bool DoUniqueIdentityComponentsMatch(string value1, string value2)
      {
        if (string.IsNullOrEmpty(value1) || string.IsNullOrEmpty(value2))
          return false;
        IdentityDisplayName disambiguatedSearchTerm1 = IdentityDisplayName.GetDisambiguatedSearchTerm(value1);
        IdentityDisplayName disambiguatedSearchTerm2 = IdentityDisplayName.GetDisambiguatedSearchTerm(value2);
        return disambiguatedSearchTerm1.Type != SearchTermType.DisplayName && disambiguatedSearchTerm1.Type == disambiguatedSearchTerm2.Type && disambiguatedSearchTerm1.ScopeId == disambiguatedSearchTerm2.ScopeId && disambiguatedSearchTerm1.Vsid == disambiguatedSearchTerm2.Vsid && StringComparer.OrdinalIgnoreCase.Equals(disambiguatedSearchTerm1.Domain, disambiguatedSearchTerm2.Domain) && StringComparer.OrdinalIgnoreCase.Equals(disambiguatedSearchTerm1.AccountName, disambiguatedSearchTerm2.AccountName);
      }

      private static string ConvertFieldValueToText(object value)
      {
        switch (value)
        {
          case null:
          case ServerDefaultFieldValue _:
            return "";
          default:
            return Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
        }
      }

      private static bool IsMember<T>(ISet<T> set, T value) => set != null && set.Contains(value);

      private bool IsMember(
        string fieldValue,
        IEnumerable<ConstantSetReference> setReferences,
        bool isIdentityField,
        out IList<ConstantSetReference> pendingSets)
      {
        pendingSets = (IList<ConstantSetReference>) null;
        IVssRequestContext requestContext = this.m_evaluationContext.RequestContext;
        try
        {
          requestContext.TraceEnter(904647, "Services", "WorkItemService", "RuleEngine.IsMember");
          foreach (ConstantSetReference setReference in setReferences)
          {
            ConstantSetReference pendingSet;
            bool flag = this.IsMember(fieldValue, setReference, isIdentityField, out pendingSet);
            if (pendingSet != null)
            {
              if (pendingSets == null)
                pendingSets = (IList<ConstantSetReference>) new List<ConstantSetReference>();
              pendingSets.Add(pendingSet);
            }
            else if (flag)
            {
              pendingSets = (IList<ConstantSetReference>) null;
              return true;
            }
          }
          return false;
        }
        finally
        {
          requestContext.TraceLeave(904650, "Services", "WorkItemService", "RuleEngine.IsMember");
        }
      }

      private bool IsMember(
        string fieldValue,
        ConstantSetReference setReference,
        bool isIdentityField,
        out ConstantSetReference pendingSet)
      {
        pendingSet = (ConstantSetReference) null;
        try
        {
          this.m_evaluationContext.RequestContext.TraceEnter(904644, "Services", "WorkItemService", "RuleEngine.IsMemberInner");
          if (!isIdentityField)
          {
            pendingSet = setReference;
            this.m_evaluationContext.RequestContext.Trace(904645, TraceLevel.Info, "Services", "WorkItemService", string.Format("fieldId: {0}, pendingSet: {1}", (object) this.m_fieldId, (object) pendingSet));
          }
          else
          {
            bool flag1 = this.m_evaluationContext.IsRealIdentity(fieldValue);
            List<string> source = new List<string>();
            if (!flag1)
            {
              Microsoft.VisualStudio.Services.Identity.Identity[] ambiguousIdentities = this.m_evaluationContext.GetAmbiguousIdentities((object) fieldValue);
              if (((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) ambiguousIdentities).Any<Microsoft.VisualStudio.Services.Identity.Identity>())
                source.AddRange(((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) ambiguousIdentities).Distinct<Microsoft.VisualStudio.Services.Identity.Identity>().Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => x.GetLegacyDistinctDisplayName())));
            }
            bool flag2 = setReference.IdentityDescriptor != (IdentityDescriptor) null || setReference.Id < 0;
            this.m_evaluationContext.RequestContext.Trace(904645, TraceLevel.Info, "Services", "WorkItemService", string.Format("fieldId: {0}, isSetRealGroup: {1}, isFieldRealIdentity: {2}", (object) this.m_fieldId, (object) flag2, (object) flag1));
            if (flag2 && !flag1)
            {
              List<string> list = source.Where<string>((Func<string, bool>) (value => this.m_evaluationContext.IsIdentityMemberOfGroup(value, setReference))).ToList<string>().Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
              if (list.Count == 1)
              {
                this.m_evalStatus.Value = (object) list[0];
                return true;
              }
              return list.Count > 1 && this.m_evaluationContext.GetIdentityDisplayType() == IdentityDisplayType.DisplayName;
            }
            if (flag1 & flag2)
              return this.m_evaluationContext.IsIdentityMemberOfGroup(fieldValue, setReference);
            if (!flag2)
              pendingSet = setReference;
          }
          return false;
        }
        finally
        {
          this.m_evaluationContext.RequestContext.TraceLeave(904646, "Services", "WorkItemService", "RuleEngine.IsMemberInner");
        }
      }

      private static bool IsMatch(string pattern, string value)
      {
        if (string.IsNullOrEmpty(pattern))
          return true;
        pattern = Regex.Escape(pattern);
        pattern = Regex.Replace(pattern, "n", "\\d", RegexOptions.IgnoreCase);
        pattern = Regex.Replace(pattern, "a", "[a-zA-Z]", RegexOptions.IgnoreCase);
        pattern = Regex.Replace(pattern, "x", "[a-zA-Z0-9]", RegexOptions.IgnoreCase);
        return Regex.IsMatch(value, "^" + pattern + "$", RegexOptions.None, CommonWorkItemTrackingConstants.RegexMatchTimeout);
      }

      private static ISet<string> Intersect(IEnumerable<ISet<string>> sets)
      {
        if (!sets.Any<ISet<string>>() || sets.Any<ISet<string>>((Func<ISet<string>, bool>) (s => s.Count == 0)))
          return (ISet<string>) new HashSet<string>((IEqualityComparer<string>) TFStringComparer.AllowedValue);
        HashSet<string> stringSet = (HashSet<string>) null;
        foreach (ISet<string> set in sets)
        {
          if (stringSet == null)
            stringSet = new HashSet<string>((IEnumerable<string>) set, (IEqualityComparer<string>) TFStringComparer.AllowedValue);
          else
            stringSet.IntersectWith((IEnumerable<string>) set);
        }
        return (ISet<string>) stringSet;
      }
    }
  }
}
