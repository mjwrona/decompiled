// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.AdditionalWorkItemTypeProperties
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class AdditionalWorkItemTypeProperties
  {
    private static readonly HashSet<int> sm_skippedSetsForExpansion = new HashSet<int>((IEnumerable<int>) new int[4]
    {
      -1,
      -2,
      -10,
      -30
    });
    private IDictionary<int, WorkItemFieldRule> m_fieldRules = (IDictionary<int, WorkItemFieldRule>) new ConcurrentDictionary<int, WorkItemFieldRule>();
    private IDictionary<string, HashSet<string>> m_transitions = (IDictionary<string, HashSet<string>>) new ConcurrentDictionary<string, HashSet<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private IDictionary<WorkItemTypeTransition, HashSet<string>> m_actions = (IDictionary<WorkItemTypeTransition, HashSet<string>>) new ConcurrentDictionary<WorkItemTypeTransition, HashSet<string>>();
    private IDictionary<int, string> m_helpTexts = (IDictionary<int, string>) new ConcurrentDictionary<int, string>();
    private ListRule m_stateAllowedValuesRule;
    private IReadOnlyCollection<WorkItemFieldRule> m_executableRules;
    private IReadOnlyCollection<WorkItemFieldRule> m_derivedFieldRules;
    private IReadOnlyCollection<WorkItemStateDefinition> m_derivedStates;
    private IReadOnlyCollection<WorkItemStateDefinition> m_states;
    private IEnumerable<WorkItemFieldRule> m_closedByAndClosedDateRules;
    private Func<IEnumerable<WorkItemFieldRule>> m_closedByAndClosedDateRulesFactory;
    private IEnumerable<WorkItemFieldRule> m_stateChangeDateRules;
    private Func<IEnumerable<WorkItemFieldRule>> m_stateChangeDateRulesFactory;
    private IDictionary<int, WorkItemFieldRule> m_baseAndDerivedFieldRules;
    private IEnumerable<WorkItemFieldRule> m_allFieldRules;

    public virtual WorkItemType WorkItemType { get; private set; }

    public virtual string Form { get; private set; }

    public virtual Layout GetFormLayout(IVssRequestContext requestContext) => this.WorkItemType.GetFormLayout(requestContext);

    public virtual IEnumerable<WorkItemFieldRule> FieldRules => this.m_allFieldRules ?? (this.m_allFieldRules = this.GetAllFieldRules());

    private IEnumerable<WorkItemFieldRule> ClosedByAndClosedDateRules
    {
      get
      {
        IEnumerable<WorkItemFieldRule> andClosedDateRules = this.m_closedByAndClosedDateRules;
        if (andClosedDateRules != null)
          return andClosedDateRules;
        Func<IEnumerable<WorkItemFieldRule>> dateRulesFactory = this.m_closedByAndClosedDateRulesFactory;
        return this.m_closedByAndClosedDateRules = dateRulesFactory != null ? dateRulesFactory() : (IEnumerable<WorkItemFieldRule>) null;
      }
    }

    private IEnumerable<WorkItemFieldRule> StateChangeDateRules
    {
      get
      {
        IEnumerable<WorkItemFieldRule> stateChangeDateRules = this.m_stateChangeDateRules;
        if (stateChangeDateRules != null)
          return stateChangeDateRules;
        Func<IEnumerable<WorkItemFieldRule>> dateRulesFactory = this.m_stateChangeDateRulesFactory;
        return this.m_stateChangeDateRules = dateRulesFactory != null ? dateRulesFactory() : (IEnumerable<WorkItemFieldRule>) null;
      }
    }

    private IEnumerable<WorkItemFieldRule> GetAllFieldRules()
    {
      if (!this.WorkItemType.IsDerived || this.m_derivedFieldRules == null || this.m_fieldRules == null)
        return (IEnumerable<WorkItemFieldRule>) this.m_executableRules;
      IDictionary<int, WorkItemFieldRule> derivedFieldRules = this.BaseAndDerivedFieldRules;
      if (this.m_derivedStates != null && this.m_derivedStates.Any<WorkItemStateDefinition>())
        this.MergeAutoInjectedCustomStateRules(derivedFieldRules);
      this.MergeAutoInjectedRules(derivedFieldRules);
      return (IEnumerable<WorkItemFieldRule>) RuleEngine.PrepareRulesForExecution((IEnumerable<WorkItemFieldRule>) derivedFieldRules.Values).ToList<WorkItemFieldRule>();
    }

    private IDictionary<int, WorkItemFieldRule> BaseAndDerivedFieldRules
    {
      get
      {
        if (this.m_baseAndDerivedFieldRules == null)
          this.m_baseAndDerivedFieldRules = this.GetBaseAndDerivedFieldRules();
        return this.m_baseAndDerivedFieldRules;
      }
    }

    private IDictionary<int, WorkItemFieldRule> GetBaseAndDerivedFieldRules()
    {
      if (this.m_derivedFieldRules == null)
        return this.m_fieldRules;
      Dictionary<int, WorkItemFieldRule> dictionary = this.m_derivedFieldRules.ToDictionary<WorkItemFieldRule, int>((Func<WorkItemFieldRule, int>) (r => r.FieldId));
      IDictionary<int, WorkItemFieldRule> fieldRules = this.m_fieldRules;
      if (fieldRules != null)
        fieldRules.Values.MergeIntoDictionary((IDictionary<int, WorkItemFieldRule>) dictionary);
      return (IDictionary<int, WorkItemFieldRule>) dictionary;
    }

    public virtual IDictionary<int, string> FieldHelpTexts
    {
      get
      {
        IEnumerable<WorkItemTypeExtensionFieldEntry> source = (IEnumerable<WorkItemTypeExtensionFieldEntry>) null;
        if (this.WorkItemType.IsDerived)
          source = this.WorkItemType.InheritedWorkItemType.Fields;
        else if (this.WorkItemType.IsCustomType)
          source = this.WorkItemType.Source.Fields;
        if (source == null || !source.Any<WorkItemTypeExtensionFieldEntry>())
          return this.m_helpTexts;
        Dictionary<int, string> second = new Dictionary<int, string>();
        foreach (WorkItemTypeExtensionFieldEntry extensionFieldEntry in source)
        {
          if (!string.IsNullOrEmpty(extensionFieldEntry.Field.Description))
            second[extensionFieldEntry.Field.FieldId] = extensionFieldEntry.Field.Description;
        }
        return (IDictionary<int, string>) this.m_helpTexts.Union<KeyValuePair<int, string>>((IEnumerable<KeyValuePair<int, string>>) second).ToDedupedDictionary<KeyValuePair<int, string>, int, string>((Func<KeyValuePair<int, string>, int>) (kv => kv.Key), (Func<KeyValuePair<int, string>, string>) (kv => kv.Value));
      }
    }

    public virtual IDictionary<string, HashSet<string>> Transitions
    {
      get
      {
        if (this.m_derivedStates != null && this.m_derivedStates.Any<WorkItemStateDefinition>())
        {
          List<string> hiddenStates = this.m_derivedStates.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Hidden)).Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)).ToList<string>();
          List<string> list1 = this.m_derivedStates.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => !s.Hidden)).Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)).ToList<string>();
          List<string> list2 = this.m_states.Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)).ToList<string>();
          string name = this.m_states.FirstOrDefault<WorkItemStateDefinition>().Name;
          Dictionary<string, HashSet<string>> transitions = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
          foreach (KeyValuePair<string, HashSet<string>> transition in (IEnumerable<KeyValuePair<string, HashSet<string>>>) this.m_transitions)
          {
            if (!hiddenStates.Contains<string>(transition.Key, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName))
            {
              if (string.IsNullOrEmpty(transition.Key))
              {
                transitions[string.Empty] = new HashSet<string>()
                {
                  name
                };
              }
              else
              {
                List<string> list3 = transition.Value.ToList<string>();
                list3.RemoveAll((Predicate<string>) (s => hiddenStates.Contains<string>(s, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName)));
                list3.AddRange((IEnumerable<string>) list1);
                transitions[transition.Key] = new HashSet<string>((IEnumerable<string>) list3, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
              }
            }
          }
          foreach (string key in list1)
            transitions[key] = new HashSet<string>((IEnumerable<string>) list2, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
          return (IDictionary<string, HashSet<string>>) transitions;
        }
        if (!this.WorkItemType.IsCustomType || this.m_states == null || !this.m_states.Any<WorkItemStateDefinition>())
          return this.m_transitions;
        List<string> list = this.m_states.Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)).ToList<string>();
        string name1 = this.m_states.FirstOrDefault<WorkItemStateDefinition>().Name;
        VssStringComparer workItemStateName = TFStringComparer.WorkItemStateName;
        HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) list, (IEqualityComparer<string>) workItemStateName);
        Dictionary<string, HashSet<string>> transitions1 = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        transitions1[string.Empty] = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName)
        {
          name1
        };
        foreach (WorkItemStateDefinition state in (IEnumerable<WorkItemStateDefinition>) this.m_states)
          transitions1[state.Name] = stringSet;
        return (IDictionary<string, HashSet<string>>) transitions1;
      }
    }

    public virtual IDictionary<WorkItemTypeTransition, HashSet<string>> Actions => this.m_actions;

    public virtual HashSet<string> AllowedStates => this.m_states != null && this.m_states.Any<WorkItemStateDefinition>() ? new HashSet<string>(this.m_states.Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName) : this.m_stateAllowedValuesRule?.Values ?? new HashSet<string>();

    internal static AdditionalWorkItemTypeProperties CreateWithFieldRules(
      IVssRequestContext requestContext,
      WorkItemType workItemType,
      IEnumerable<WorkItemFieldRule> fieldRules,
      IReadOnlyCollection<HelpTextDescriptor> helpTexts)
    {
      return requestContext.TraceBlock<AdditionalWorkItemTypeProperties>(911311, 911312, "WorkItemType", nameof (AdditionalWorkItemTypeProperties), nameof (CreateWithFieldRules), (Func<AdditionalWorkItemTypeProperties>) (() =>
      {
        AdditionalWorkItemTypeProperties properties = AdditionalWorkItemTypeProperties.CreateInternal(requestContext, workItemType, out int _);
        properties.m_fieldRules = (IDictionary<int, WorkItemFieldRule>) new Dictionary<int, WorkItemFieldRule>();
        if (workItemType.IsDerived && workItemType.InheritedWorkItemType.DisabledRules != null && workItemType.InheritedWorkItemType.DisabledRules.Any<Guid>() && fieldRules != null)
        {
          HashSet<Guid> disabledRules = new HashSet<Guid>(workItemType.InheritedWorkItemType.DisabledRules);
          fieldRules = (IEnumerable<WorkItemFieldRule>) DisabledRulesFilter.RunFilter((IReadOnlyCollection<WorkItemFieldRule>) fieldRules.ToList<WorkItemFieldRule>(), disabledRules);
        }
        if (fieldRules != null)
        {
          foreach (WorkItemFieldRule fieldRule in fieldRules)
            properties.m_fieldRules[fieldRule.FieldId] = fieldRule;
        }
        AdditionalWorkItemTypeProperties.PopulateStateTransitionMappingsAndAllowedValues(properties);
        properties.ResolveListRules(requestContext, new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica")), workItemType.ProjectId, (IEnumerable<WorkItemFieldRule>) properties.m_fieldRules.Values);
        properties.m_executableRules = (IReadOnlyCollection<WorkItemFieldRule>) RuleEngine.PrepareRulesForExecution((IEnumerable<WorkItemFieldRule>) properties.m_fieldRules.Values).ToList<WorkItemFieldRule>();
        properties.m_helpTexts = (IDictionary<int, string>) new Dictionary<int, string>();
        foreach (HelpTextDescriptor helpText in (IEnumerable<HelpTextDescriptor>) helpTexts)
          properties.m_helpTexts[helpText.FieldId] = helpText.HelpText;
        return properties;
      }));
    }

    internal static void AugmentWithTypeletFieldRules(
      IVssRequestContext requestContext,
      AdditionalWorkItemTypeProperties additionalProperties,
      Guid processId,
      string witRefName)
    {
      ProcessTypelet processTypelet;
      if (!requestContext.GetService<IProcessWorkItemTypeService>().TryGetTypelet<ProcessTypelet>(requestContext, processId, witRefName, false, out processTypelet) || processTypelet.FieldRules == null)
        return;
      foreach (WorkItemFieldRule fieldRule in processTypelet.FieldRules)
      {
        if (fieldRule.SubRules != null)
        {
          foreach (WorkItemRule subRule in fieldRule.SubRules)
          {
            if (subRule is HelpTextRule helpTextRule)
              additionalProperties.m_helpTexts[fieldRule.FieldId] = helpTextRule.Value;
          }
        }
      }
    }

    private static IEnumerable<WorkItemFieldRule> WrapRulesUnderTypeScope(
      IEnumerable<WorkItemFieldRule> rules)
    {
      foreach (WorkItemFieldRule rule in rules)
      {
        WorkItemFieldRule workItemFieldRule = rule;
        WorkItemTypeScopedRules[] itemTypeScopedRulesArray = new WorkItemTypeScopedRules[1];
        WorkItemTypeScopedRules itemTypeScopedRules = new WorkItemTypeScopedRules();
        itemTypeScopedRules.SubRules = rule.SubRules;
        itemTypeScopedRulesArray[0] = itemTypeScopedRules;
        WorkItemRule[] workItemRuleArray = (WorkItemRule[]) itemTypeScopedRulesArray;
        workItemFieldRule.SubRules = workItemRuleArray;
      }
      return rules;
    }

    private static IEnumerable<WorkItemFieldRule> GenerateStateCategoryTransitionRules(
      IVssRequestContext requestContext,
      WorkItemType workItemType)
    {
      return AdditionalWorkItemTypeProperties.WrapRulesUnderTypeScope(requestContext.GetService<IWorkItemStateDefinitionService>().GenerateStateCategoryTransitionRules(requestContext, workItemType));
    }

    private static IEnumerable<WorkItemFieldRule> GenerateStateChangeDateRules(
      IVssRequestContext requestContext,
      WorkItemType workItemType)
    {
      FieldEntry stateChangeDateFieldEntry;
      if (!requestContext.WitContext().FieldDictionary.TryGetFieldByNameOrId("Microsoft.VSTS.Common.StateChangeDate", out stateChangeDateFieldEntry))
        return Enumerable.Empty<WorkItemFieldRule>();
      return !workItemType.GetFieldIds(requestContext).Any<int>((Func<int, bool>) (id => id == stateChangeDateFieldEntry.FieldId)) ? Enumerable.Empty<WorkItemFieldRule>() : AdditionalWorkItemTypeProperties.WrapRulesUnderTypeScope(requestContext.GetService<IWorkItemStateDefinitionService>().GenerateStateChangeDateFieldRules(requestContext));
    }

    internal static AdditionalWorkItemTypeProperties CreateWithRuleRecords(
      IVssRequestContext requestContext,
      WorkItemType workItemType,
      WitReadReplicaContext? readReplicaContext,
      IEnumerable<RuleRecord> rules)
    {
      return requestContext.TraceBlock<AdditionalWorkItemTypeProperties>(911311, 911312, "WorkItemType", nameof (AdditionalWorkItemTypeProperties), nameof (CreateWithRuleRecords), (Func<AdditionalWorkItemTypeProperties>) (() =>
      {
        AdditionalWorkItemTypeProperties withRuleRecords = AdditionalWorkItemTypeProperties.CreateInternal(requestContext, workItemType, out int _);
        withRuleRecords.ProcessRuleRecords(requestContext, rules, workItemType.ProjectId, readReplicaContext);
        return withRuleRecords;
      }));
    }

    internal static AdditionalWorkItemTypeProperties CreateForCustomWorkItemType(
      IVssRequestContext requestContext,
      WorkItemType workItemType,
      ProcessWorkItemType sourceWorkItemType,
      IDictionary<int, string> helpTextByFieldID)
    {
      return new AdditionalWorkItemTypeProperties()
      {
        WorkItemType = workItemType,
        m_executableRules = sourceWorkItemType.GetExecutableRules(requestContext),
        m_states = sourceWorkItemType.GetStates(requestContext),
        m_derivedFieldRules = workItemType.InheritedWorkItemType?.GetCombinedFieldRules(requestContext),
        m_helpTexts = helpTextByFieldID
      };
    }

    internal static AdditionalWorkItemTypeProperties CreateClone(
      IVssRequestContext requestContext,
      AdditionalWorkItemTypeProperties existingProperties,
      WorkItemType newWorkItemType)
    {
      return requestContext.TraceBlock<AdditionalWorkItemTypeProperties>(911309, 911310, "WorkItemType", nameof (AdditionalWorkItemTypeProperties), nameof (CreateClone), (Func<AdditionalWorkItemTypeProperties>) (() =>
      {
        AdditionalWorkItemTypeProperties clone = new AdditionalWorkItemTypeProperties()
        {
          WorkItemType = newWorkItemType,
          m_fieldRules = existingProperties.m_fieldRules,
          m_actions = existingProperties.m_actions,
          m_executableRules = existingProperties.m_executableRules,
          m_helpTexts = existingProperties.m_helpTexts,
          m_transitions = existingProperties.m_transitions,
          m_stateAllowedValuesRule = existingProperties.m_stateAllowedValuesRule,
          Form = existingProperties.Form,
          m_derivedFieldRules = newWorkItemType.InheritedWorkItemType?.GetCombinedFieldRules(requestContext),
          m_closedByAndClosedDateRules = existingProperties.m_closedByAndClosedDateRules,
          m_closedByAndClosedDateRulesFactory = existingProperties.m_closedByAndClosedDateRulesFactory,
          m_stateChangeDateRules = existingProperties.m_stateChangeDateRules,
          m_stateChangeDateRulesFactory = existingProperties.m_stateChangeDateRulesFactory,
          m_baseAndDerivedFieldRules = existingProperties.m_baseAndDerivedFieldRules,
          m_allFieldRules = existingProperties.m_allFieldRules
        };
        if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
        {
          clone.m_derivedStates = newWorkItemType.InheritedWorkItemType?.GetDeltaStates(requestContext);
          clone.m_states = newWorkItemType.InheritedWorkItemType?.GetStates(requestContext);
        }
        return clone;
      }));
    }

    private void MergeAutoInjectedCustomStateRules(IDictionary<int, WorkItemFieldRule> allRules)
    {
      allRules[2] = this.MergeStateRule(allRules[2]);
      allRules[22] = this.MergeReasonRule(allRules[22]);
      IEnumerable<WorkItemFieldRule> andClosedDateRules = this.ClosedByAndClosedDateRules;
      if (andClosedDateRules == null)
        return;
      andClosedDateRules.MergeIntoDictionary(allRules);
    }

    private void MergeAutoInjectedRules(IDictionary<int, WorkItemFieldRule> allRules)
    {
      IEnumerable<WorkItemFieldRule> stateChangeDateRules = this.StateChangeDateRules;
      if (stateChangeDateRules == null)
        return;
      stateChangeDateRules.MergeIntoDictionary(allRules);
    }

    private WorkItemFieldRule MergeStateRule(WorkItemFieldRule parentStateFieldRule)
    {
      List<string> list1 = this.m_derivedStates.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => !s.Hidden)).Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (S => S.Name)).ToList<string>();
      List<string> hiddenStates = this.m_derivedStates.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Hidden)).Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (S => S.Name)).ToList<string>();
      IEnumerable<string> collection = this.m_states.Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name));
      WorkItemStateDefinition itemStateDefinition = this.m_states.FirstOrDefault<WorkItemStateDefinition>();
      WorkItemFieldRule workItemFieldRule = parentStateFieldRule.Clone(false) as WorkItemFieldRule;
      WorkItemTypeScopedRules itemTypeScopedRules = workItemFieldRule.SelectRules<WorkItemTypeScopedRules>().FirstOrDefault<WorkItemTypeScopedRules>();
      if (itemTypeScopedRules == null)
        return workItemFieldRule;
      foreach (WorkItemRule subRule in itemTypeScopedRules.SubRules)
      {
        switch (subRule)
        {
          case AllowedValuesRule _:
            (subRule as AllowedValuesRule).Values = new HashSet<string>(collection, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
            break;
          case WhenWasRule _:
            WhenWasRule whenWasRule = subRule as WhenWasRule;
            AllowedValuesRule allowedValuesRule = subRule.SelectRules<AllowedValuesRule>().FirstOrDefault<AllowedValuesRule>();
            if (allowedValuesRule != null)
            {
              if (string.IsNullOrEmpty(whenWasRule.Value))
              {
                HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName)
                {
                  itemStateDefinition.Name
                };
                allowedValuesRule.Values = stringSet;
                break;
              }
              List<string> list2 = allowedValuesRule.Values.ToList<string>();
              list2.RemoveAll((Predicate<string>) (s => hiddenStates.Contains<string>(s, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName)));
              list2.AddRange((IEnumerable<string>) list1);
              allowedValuesRule.Values = new HashSet<string>((IEnumerable<string>) list2, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
              break;
            }
            break;
          case DefaultRule _:
            (subRule as DefaultRule).Value = itemStateDefinition.Name;
            break;
        }
      }
      foreach (string str in list1)
      {
        WhenWasRule whenWasRule1 = new WhenWasRule();
        whenWasRule1.Value = str;
        whenWasRule1.FieldId = 2;
        whenWasRule1.Inverse = false;
        WhenWasRule rule1 = whenWasRule1;
        WhenWasRule whenWasRule2 = rule1;
        AllowedValuesRule rule2 = new AllowedValuesRule();
        rule2.Values = new HashSet<string>(collection, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        whenWasRule2.AddRule<AllowedValuesRule>(rule2);
        itemTypeScopedRules.AddRule<WhenWasRule>(rule1);
      }
      return workItemFieldRule;
    }

    private WorkItemFieldRule MergeReasonRule(WorkItemFieldRule parentReasonFieldRule)
    {
      List<string> list = this.m_derivedStates.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => !s.Hidden)).Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (S => S.Name)).ToList<string>();
      WorkItemFieldRule workItemFieldRule = parentReasonFieldRule.Clone(false) as WorkItemFieldRule;
      WorkItemTypeScopedRules itemTypeScopedRules = workItemFieldRule.SelectRules<WorkItemTypeScopedRules>().FirstOrDefault<WorkItemTypeScopedRules>();
      if (itemTypeScopedRules == null)
        return workItemFieldRule;
      foreach (string str in list)
      {
        WhenRule whenRule1 = new WhenRule();
        whenRule1.FieldId = 2;
        whenRule1.Value = str;
        WhenRule rule1 = whenRule1;
        WhenRule whenRule2 = rule1;
        CopyRule rule2 = new CopyRule();
        rule2.Value = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReason((object) str);
        whenRule2.AddRule<CopyRule>(rule2);
        rule1.AddRule<RequiredRule>(new RequiredRule());
        itemTypeScopedRules.AddRule<WhenRule>(rule1);
        WhenWasRule whenWasRule1 = new WhenWasRule();
        whenWasRule1.FieldId = 2;
        whenWasRule1.Value = str;
        WhenWasRule rule3 = whenWasRule1;
        WhenWasRule whenWasRule2 = rule3;
        CopyRule rule4 = new CopyRule();
        rule4.Value = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReasonOut((object) str);
        whenWasRule2.AddRule<CopyRule>(rule4);
        rule3.AddRule<RequiredRule>(new RequiredRule());
        itemTypeScopedRules.AddRule<WhenWasRule>(rule3);
      }
      return workItemFieldRule;
    }

    private void ProcessActions(IVssRequestContext requestContext, string projectName) => requestContext.TraceBlock(911317, 911318, "WorkItemType", nameof (AdditionalWorkItemTypeProperties), nameof (ProcessActions), (Action) (() =>
    {
      foreach (WorkItemTypeAction workItemTypeAction in requestContext.GetService<TeamFoundationWorkItemTrackingMetadataService>().GetWorkItemTypeActions(requestContext, projectName, this.WorkItemType.Name))
      {
        WorkItemTypeTransition key = new WorkItemTypeTransition()
        {
          From = workItemTypeAction.FromState,
          To = workItemTypeAction.ToState
        };
        HashSet<string> stringSet;
        if (!this.m_actions.TryGetValue(key, out stringSet))
        {
          stringSet = new HashSet<string>();
          this.m_actions[key] = stringSet;
        }
        stringSet.Add(workItemTypeAction.Name);
      }
    }));

    private void ProcessRuleRecords(
      IVssRequestContext requestContext,
      IEnumerable<RuleRecord> rules,
      Guid projectId,
      WitReadReplicaContext? readReplicaContext)
    {
      requestContext.TraceBlock(911313, 911314, "WorkItemType", nameof (AdditionalWorkItemTypeProperties), nameof (ProcessRuleRecords), (Action) (() =>
      {
        RuleRecordsTranslator recordsTranslator = new RuleRecordsTranslator(this.m_transitions, this.m_helpTexts, (Action<ListRule>) (stateAllowedValuesRule => this.m_stateAllowedValuesRule = stateAllowedValuesRule), new Func<int, WorkItemFieldRule>(this.GetFieldRule), (Action<string>) (form => this.Form = form));
        foreach (RuleRecord rule in rules)
          recordsTranslator.TranslateRuleRecords(rule);
        this.ResolveListRules(requestContext, readReplicaContext, projectId, (IEnumerable<WorkItemFieldRule>) this.m_fieldRules.Values);
        this.GenerateStateRules();
        this.GenerateSystemRules(requestContext);
        this.m_executableRules = (IReadOnlyCollection<WorkItemFieldRule>) RuleEngine.PrepareRulesForExecution((IEnumerable<WorkItemFieldRule>) this.m_fieldRules.Values).ToList<WorkItemFieldRule>();
      }));
    }

    private void ResolveListRules(
      IVssRequestContext requestContext,
      WitReadReplicaContext? readReplicaContext,
      Guid projectId,
      IEnumerable<WorkItemFieldRule> rules)
    {
      ListRuleResolverState state = new ListRuleResolverState(rules);
      bool isScopedIdentityEnabled = !WorkItemTrackingFeatureFlags.IsHostedXMLProject(requestContext, projectId);
      new ListExpansionResolver(requestContext, AdditionalWorkItemTypeProperties.sm_skippedSetsForExpansion, readReplicaContext, isScopedIdentityEnabled).Resolve(state);
      new GlobalListResolver(requestContext, AdditionalWorkItemTypeProperties.sm_skippedSetsForExpansion).Resolve(state);
      if (!isScopedIdentityEnabled)
        return;
      new WellKnownSetsResolver(requestContext, new Dictionary<int, Tuple<IdentityDescriptor, bool>>()
      {
        {
          -2,
          new Tuple<IdentityDescriptor, bool>(GroupWellKnownIdentityDescriptors.EveryoneGroup, true)
        },
        {
          -1,
          new Tuple<IdentityDescriptor, bool>(GroupWellKnownIdentityDescriptors.EveryoneGroup, false)
        }
      }).Resolve(state);
      new EntityIdResolver(requestContext).Resolve(state);
    }

    private void GenerateStateRules()
    {
      if (this.m_stateAllowedValuesRule == null)
        return;
      WorkItemTypeScopedRules itemTypeScopedRules1 = this.GetFieldRule(2).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
      foreach (string key in this.m_stateAllowedValuesRule.Values.Concat<string>((IEnumerable<string>) new string[1]
      {
        string.Empty
      }))
      {
        HashSet<string> stringSet;
        if (this.m_transitions.TryGetValue(key, out stringSet))
        {
          WorkItemTypeScopedRules itemTypeScopedRules2 = itemTypeScopedRules1;
          WhenWasRule rule1 = new WhenWasRule();
          rule1.FieldId = 2;
          rule1.Value = key;
          WhenWasRule whenWasRule = itemTypeScopedRules2.AddRule<WhenWasRule>(rule1);
          AllowedValuesRule rule2 = new AllowedValuesRule();
          rule2.Values = new HashSet<string>((IEnumerable<string>) stringSet, (IEqualityComparer<string>) TFStringComparer.AllowedValue);
          whenWasRule.AddRule<AllowedValuesRule>(rule2);
          if (string.IsNullOrEmpty(key))
          {
            WorkItemTypeScopedRules itemTypeScopedRules3 = itemTypeScopedRules1;
            DefaultRule rule3 = new DefaultRule();
            rule3.Value = stringSet.FirstOrDefault<string>();
            itemTypeScopedRules3.AddRule<DefaultRule>(rule3);
          }
        }
        else if (!string.IsNullOrEmpty(key))
        {
          WorkItemTypeScopedRules itemTypeScopedRules4 = itemTypeScopedRules1;
          WhenWasRule rule4 = new WhenWasRule();
          rule4.FieldId = 2;
          rule4.Value = key;
          WhenWasRule whenWasRule = itemTypeScopedRules4.AddRule<WhenWasRule>(rule4);
          AllowedValuesRule allowedValuesRule = new AllowedValuesRule();
          allowedValuesRule.Values = new HashSet<string>((IEnumerable<string>) new string[1]
          {
            key
          }, (IEqualityComparer<string>) TFStringComparer.AllowedValue);
          AllowedValuesRule rule5 = allowedValuesRule;
          whenWasRule.AddRule<AllowedValuesRule>(rule5);
        }
      }
    }

    private void GenerateSystemRules(IVssRequestContext requestContext)
    {
      FieldEntry field;
      if (requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext).TryGetField(-43, out field) && (!field.IsIgnored || field.IsTreeNode))
      {
        WorkItemTypeScopedRules itemTypeScopedRules1 = this.GetFieldRule(-43).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule1 = new ComputedRule();
        rule1.FieldId = -2;
        itemTypeScopedRules1.AddRule<ComputedRule>(rule1);
        WorkItemTypeScopedRules itemTypeScopedRules2 = this.GetFieldRule(-44).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule2 = new ComputedRule();
        rule2.FieldId = -2;
        itemTypeScopedRules2.AddRule<ComputedRule>(rule2);
        WorkItemTypeScopedRules itemTypeScopedRules3 = this.GetFieldRule(-45).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule3 = new ComputedRule();
        rule3.FieldId = -2;
        itemTypeScopedRules3.AddRule<ComputedRule>(rule3);
        WorkItemTypeScopedRules itemTypeScopedRules4 = this.GetFieldRule(-46).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule4 = new ComputedRule();
        rule4.FieldId = -2;
        itemTypeScopedRules4.AddRule<ComputedRule>(rule4);
        WorkItemTypeScopedRules itemTypeScopedRules5 = this.GetFieldRule(-47).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule5 = new ComputedRule();
        rule5.FieldId = -2;
        itemTypeScopedRules5.AddRule<ComputedRule>(rule5);
        WorkItemTypeScopedRules itemTypeScopedRules6 = this.GetFieldRule(-48).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule6 = new ComputedRule();
        rule6.FieldId = -2;
        itemTypeScopedRules6.AddRule<ComputedRule>(rule6);
        WorkItemTypeScopedRules itemTypeScopedRules7 = this.GetFieldRule(-49).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule7 = new ComputedRule();
        rule7.FieldId = -2;
        itemTypeScopedRules7.AddRule<ComputedRule>(rule7);
        WorkItemTypeScopedRules itemTypeScopedRules8 = this.GetFieldRule(-50).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule8 = new ComputedRule();
        rule8.FieldId = -104;
        itemTypeScopedRules8.AddRule<ComputedRule>(rule8);
        WorkItemTypeScopedRules itemTypeScopedRules9 = this.GetFieldRule(-51).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule9 = new ComputedRule();
        rule9.FieldId = -104;
        itemTypeScopedRules9.AddRule<ComputedRule>(rule9);
        WorkItemTypeScopedRules itemTypeScopedRules10 = this.GetFieldRule(-52).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule10 = new ComputedRule();
        rule10.FieldId = -104;
        itemTypeScopedRules10.AddRule<ComputedRule>(rule10);
        WorkItemTypeScopedRules itemTypeScopedRules11 = this.GetFieldRule(-53).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule11 = new ComputedRule();
        rule11.FieldId = -104;
        itemTypeScopedRules11.AddRule<ComputedRule>(rule11);
        WorkItemTypeScopedRules itemTypeScopedRules12 = this.GetFieldRule(-54).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule12 = new ComputedRule();
        rule12.FieldId = -104;
        itemTypeScopedRules12.AddRule<ComputedRule>(rule12);
        WorkItemTypeScopedRules itemTypeScopedRules13 = this.GetFieldRule(-55).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule13 = new ComputedRule();
        rule13.FieldId = -104;
        itemTypeScopedRules13.AddRule<ComputedRule>(rule13);
        WorkItemTypeScopedRules itemTypeScopedRules14 = this.GetFieldRule(-56).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
        ComputedRule rule14 = new ComputedRule();
        rule14.FieldId = -104;
        itemTypeScopedRules14.AddRule<ComputedRule>(rule14);
      }
      WorkItemTypeScopedRules itemTypeScopedRules15 = this.GetFieldRule(-2).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
      ComputedRule rule15 = new ComputedRule();
      rule15.FieldId = -7;
      itemTypeScopedRules15.AddRule<ComputedRule>(rule15);
      itemTypeScopedRules15.AddRule<RequiredRule>(new RequiredRule());
      WorkItemTypeScopedRules itemTypeScopedRules16 = this.GetFieldRule(-7).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
      ComputedRule rule16 = new ComputedRule();
      rule16.FieldId = -2;
      itemTypeScopedRules16.AddRule<ComputedRule>(rule16);
      WorkItemTypeScopedRules itemTypeScopedRules17 = this.GetFieldRule(-12).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
      ComputedRule rule17 = new ComputedRule();
      rule17.FieldId = -2;
      itemTypeScopedRules17.AddRule<ComputedRule>(rule17);
      WorkItemTypeScopedRules itemTypeScopedRules18 = this.GetFieldRule(-104).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
      ComputedRule rule18 = new ComputedRule();
      rule18.FieldId = -105;
      itemTypeScopedRules18.AddRule<ComputedRule>(rule18);
      itemTypeScopedRules18.AddRule<RequiredRule>(new RequiredRule());
      WorkItemTypeScopedRules itemTypeScopedRules19 = this.GetFieldRule(-105).AddRule<WorkItemTypeScopedRules>(new WorkItemTypeScopedRules());
      ComputedRule rule19 = new ComputedRule();
      rule19.FieldId = -104;
      itemTypeScopedRules19.AddRule<ComputedRule>(rule19);
      this.GetFieldRule(9).AddRule<AllowExistingValueRule>(new AllowExistingValueRule());
    }

    public IEnumerable<FieldEntry> GetDependentFieldRule(
      WorkItemTrackingRequestContext witRequestContext,
      int fieldId)
    {
      if (!CommonWITUtils.HasReadRulesPermission(witRequestContext.RequestContext))
        throw new ReadDependentFieldsNotAuthorizedException();
      Dictionary<int, FieldEntry> dictionary = new Dictionary<int, FieldEntry>();
      IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
      WorkItemFieldRule workItemFieldRule = this.FieldRules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => r.FieldId == fieldId)).FirstOrDefault<WorkItemFieldRule>();
      if (workItemFieldRule != null && workItemFieldRule.SubRules != null)
      {
        foreach (WorkItemRule subRule in workItemFieldRule.SubRules)
        {
          switch (subRule)
          {
            case TriggerRule _:
              foreach (int fieldId1 in (subRule as TriggerRule).FieldIds)
              {
                if (!dictionary.ContainsKey(fieldId1))
                  dictionary.Add(fieldId1, fieldDictionary.GetField(fieldId1));
              }
              break;
            case WorkItemTypeScopedRules itemTypeScopedRules:
              if (itemTypeScopedRules.SubRules != null)
              {
                using (IEnumerator<RuleFieldDependency> enumerator = ((IEnumerable<WorkItemRule>) itemTypeScopedRules.SubRules).SelectMany<WorkItemRule, RuleFieldDependency>((Func<WorkItemRule, IEnumerable<RuleFieldDependency>>) (s => s.GetDependencies())).GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    RuleFieldDependency current = enumerator.Current;
                    if (!dictionary.ContainsKey(current.FieldId))
                      dictionary.Add(current.FieldId, fieldDictionary.GetField(current.FieldId));
                  }
                  break;
                }
              }
              else
                break;
          }
        }
      }
      return (IEnumerable<FieldEntry>) dictionary.Values;
    }

    private WorkItemFieldRule GetFieldRule(int fieldId)
    {
      WorkItemFieldRule fieldRule;
      if (!this.m_fieldRules.TryGetValue(fieldId, out fieldRule))
      {
        fieldRule = new WorkItemFieldRule()
        {
          FieldId = fieldId
        };
        this.m_fieldRules.Add(fieldId, fieldRule);
      }
      return fieldRule;
    }

    private static AdditionalWorkItemTypeProperties CreateInternal(
      IVssRequestContext requestContext,
      WorkItemType workItemType,
      out int projectId)
    {
      AdditionalWorkItemTypeProperties itemTypeProperties = new AdditionalWorkItemTypeProperties()
      {
        WorkItemType = workItemType,
        m_derivedFieldRules = workItemType.InheritedWorkItemType?.GetCombinedFieldRules(requestContext)
      };
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
      {
        itemTypeProperties.m_derivedStates = workItemType.InheritedWorkItemType?.GetDeltaStates(requestContext);
        itemTypeProperties.m_states = workItemType.InheritedWorkItemType?.GetStates(requestContext);
        itemTypeProperties.m_closedByAndClosedDateRulesFactory = (Func<IEnumerable<WorkItemFieldRule>>) (() => AdditionalWorkItemTypeProperties.GenerateStateCategoryTransitionRules(requestContext, workItemType));
        itemTypeProperties.m_stateChangeDateRulesFactory = (Func<IEnumerable<WorkItemFieldRule>>) (() => AdditionalWorkItemTypeProperties.GenerateStateChangeDateRules(requestContext, workItemType));
        itemTypeProperties.m_closedByAndClosedDateRules = AdditionalWorkItemTypeProperties.GenerateStateCategoryTransitionRules(requestContext, workItemType);
        itemTypeProperties.m_stateChangeDateRules = AdditionalWorkItemTypeProperties.GenerateStateChangeDateRules(requestContext, workItemType);
      }
      TreeNode treeNode = requestContext.WitContext().TreeService.GetTreeNode(workItemType.ProjectId, workItemType.ProjectId);
      itemTypeProperties.ProcessActions(requestContext, treeNode.GetName(requestContext));
      projectId = treeNode.Id;
      return itemTypeProperties;
    }

    private static void PopulateStateTransitionMappingsAndAllowedValues(
      AdditionalWorkItemTypeProperties properties)
    {
      WorkItemFieldRule workItemFieldRule;
      if (properties.m_fieldRules == null || !properties.m_fieldRules.TryGetValue(2, out workItemFieldRule))
        return;
      Dictionary<string, HashSet<string>> dictionary = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      foreach (WhenWasRule whenWasRule in workItemFieldRule.SelectRules<WhenWasRule>().Where<WhenWasRule>((Func<WhenWasRule, bool>) (r => r.Value != null)))
      {
        HashSet<string> source;
        if (!dictionary.TryGetValue(whenWasRule.Value, out source))
          source = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        AllowedValuesRule allowedValuesRule = whenWasRule.SelectRules<AllowedValuesRule>().FirstOrDefault<AllowedValuesRule>();
        if (allowedValuesRule != null && allowedValuesRule.Values != null)
        {
          foreach (string str in allowedValuesRule.Values)
            source.Add(str);
        }
        if (source.Any<string>())
          dictionary[whenWasRule.Value] = source;
      }
      properties.m_transitions = (IDictionary<string, HashSet<string>>) dictionary;
      WorkItemTypeScopedRules itemTypeScopedRules = workItemFieldRule.SelectRules<WorkItemTypeScopedRules>().FirstOrDefault<WorkItemTypeScopedRules>();
      WorkItemRule workItemRule = itemTypeScopedRules != null ? ((IEnumerable<WorkItemRule>) itemTypeScopedRules.SubRules).Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is AllowedValuesRule)).FirstOrDefault<WorkItemRule>() : (WorkItemRule) null;
      if (workItemRule == null)
        return;
      properties.m_stateAllowedValuesRule = workItemRule as ListRule;
    }

    private static string GetFieldRuleXml(
      IVssRequestContext requestContext,
      Dictionary<int, WorkItemFieldRule> fieldRules)
    {
      foreach (KeyValuePair<int, WorkItemFieldRule> fieldRule in fieldRules)
      {
        WorkItemFieldRule workItemFieldRule = fieldRule.Value;
        workItemFieldRule.Field = requestContext.WitContext().FieldDictionary.GetField(fieldRule.Key).ReferenceName;
        foreach (WorkItemRule subRule in workItemFieldRule.SubRules)
          AdditionalWorkItemTypeProperties.AddRuleProperties(requestContext, subRule);
      }
      return CommonWITUtils.GetSerializedRuleXML(fieldRules.Select<KeyValuePair<int, WorkItemFieldRule>, WorkItemFieldRule>((Func<KeyValuePair<int, WorkItemFieldRule>, WorkItemFieldRule>) (r => r.Value)).ToArray<WorkItemFieldRule>());
    }

    private static void AddRuleProperties(IVssRequestContext requestContext, WorkItemRule rule)
    {
      IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
      if (rule is WorkItemFieldRule workItemFieldRule)
        workItemFieldRule.Field = fieldDictionary.GetField(workItemFieldRule.FieldId).ReferenceName;
      if (rule is DependentRule dependentRule)
        dependentRule.Field = fieldDictionary.GetField(dependentRule.FieldId).ReferenceName;
      if (rule is ConditionalBlockRule conditionalBlockRule)
        conditionalBlockRule.Field = fieldDictionary.GetField(conditionalBlockRule.FieldId).ReferenceName;
      if (rule is RuleBlock ruleBlock)
      {
        foreach (WorkItemRule subRule in ruleBlock.SubRules)
          AdditionalWorkItemTypeProperties.AddRuleProperties(requestContext, subRule);
      }
      else
        rule.Id = Guid.NewGuid();
    }

    private static string GetHelpTextXML(
      IVssRequestContext requestContext,
      IDictionary<int, string> helpTexts)
    {
      List<HelpTextDescriptor> helpTextDescriptorList = new List<HelpTextDescriptor>();
      IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
      foreach (KeyValuePair<int, string> helpText in (IEnumerable<KeyValuePair<int, string>>) helpTexts)
        helpTextDescriptorList.Add(new HelpTextDescriptor()
        {
          FieldReferenceName = fieldDictionary.GetField(helpText.Key).ReferenceName,
          HelpText = helpText.Value
        });
      return TeamFoundationSerializationUtility.SerializeToString<HelpTextDescriptor[]>(helpTextDescriptorList.ToArray(), new XmlRootAttribute("help-texts"));
    }
  }
}
