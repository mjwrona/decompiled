// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessWorkItemType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class ProcessWorkItemType : ProcessTypelet
  {
    private IReadOnlyDictionary<string, bool> m_behaviorRelations;
    private IEnumerable<Guid> m_disabledRules = (IEnumerable<Guid>) Array.Empty<Guid>();

    public bool IsDisabled { get; protected set; }

    public string ParentTypeRefName { get; protected set; }

    public ProcessWorkItemType ParentWorkItemType { get; protected set; }

    public IReadOnlyCollection<BehaviorRelation> BehaviorRelations { get; protected set; }

    public bool IsDerived => !string.IsNullOrEmpty(this.ParentTypeRefName) && !TFStringComparer.WorkItemTypeReferenceName.Equals(this.ParentTypeRefName, SystemWorkItemType.ReferenceName);

    public bool IsCustomType => !string.IsNullOrEmpty(this.ParentTypeRefName) && TFStringComparer.WorkItemTypeReferenceName.Equals(this.ParentTypeRefName, SystemWorkItemType.ReferenceName);

    public override IEnumerable<WorkItemFieldRule> FieldRules
    {
      get => base.FieldRules != null ? base.FieldRules.Select<WorkItemFieldRule, WorkItemFieldRule>((Func<WorkItemFieldRule, WorkItemFieldRule>) (r => r.Clone(false) as WorkItemFieldRule)) : base.FieldRules;
      protected set => base.FieldRules = value;
    }

    public IEnumerable<Guid> DisabledRules
    {
      get
      {
        if (this.m_disabledRules == null)
          this.m_disabledRules = (IEnumerable<Guid>) Array.Empty<Guid>();
        return this.m_disabledRules;
      }
      set => this.m_disabledRules = value != null ? (IEnumerable<Guid>) value.Where<Guid>((Func<Guid, bool>) (r => r != Guid.Empty)).Distinct<Guid>().ToList<Guid>() : (IEnumerable<Guid>) null;
    }

    public IEnumerable<Guid> ForNotGroups { get; private set; } = (IEnumerable<Guid>) Array.Empty<Guid>();

    public virtual LayoutInfo GetFormLayoutInfo(
      IVssRequestContext requestContext,
      bool resolveContributions = true)
    {
      if (this.ParentWorkItemType == null || this.ParentWorkItemType.Form == null)
        return new LayoutInfo(this.Form, this.Form, new Layout());
      ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, this.ProcessId);
      Layout form = this.ParentWorkItemType.Form;
      Layout deltaLayout = this.Form;
      if (((!WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) ? 0 : (!processDescriptor.IsCustom ? 1 : 0)) & (resolveContributions ? 1 : 0)) != 0)
        deltaLayout = FormExtensionsTransformer.AddContributions(requestContext, form, deltaLayout);
      return new LayoutInfo(requestContext.GetService<IFormLayoutService>().CombineLayouts(requestContext, form, deltaLayout), form, deltaLayout);
    }

    public virtual Layout GetCombinedForm(IVssRequestContext requestContext) => this.GetFormLayoutInfo(requestContext).ComposedLayout;

    public virtual IReadOnlyCollection<FieldEntry> GetCombinedFields(
      IVssRequestContext requestContext)
    {
      Dictionary<int, FieldEntry> dictionary = new Dictionary<int, FieldEntry>();
      if (this.Fields != null)
        dictionary = this.Fields.Select<WorkItemTypeExtensionFieldEntry, FieldEntry>((Func<WorkItemTypeExtensionFieldEntry, FieldEntry>) (f => f.Field)).ToDictionary<FieldEntry, int>((Func<FieldEntry, int>) (f => f.FieldId));
      if (this.ParentWorkItemType != null && this.ParentWorkItemType.Fields != null)
      {
        foreach (FieldEntry combinedField in (IEnumerable<FieldEntry>) this.ParentWorkItemType.GetCombinedFields(requestContext))
        {
          if (!dictionary.ContainsKey(combinedField.FieldId))
            dictionary[combinedField.FieldId] = combinedField;
        }
      }
      if (this.BehaviorRelations != null)
      {
        foreach (BehaviorRelation behaviorRelation in (IEnumerable<BehaviorRelation>) this.BehaviorRelations)
        {
          foreach (FieldEntry fieldEntry in behaviorRelation.Behavior.GetCombinedBehaviorFields(requestContext).Values)
          {
            if (!dictionary.ContainsKey(fieldEntry.FieldId))
              dictionary[fieldEntry.FieldId] = fieldEntry;
          }
        }
      }
      return (IReadOnlyCollection<FieldEntry>) dictionary.Values;
    }

    public virtual IReadOnlyCollection<WorkItemFieldRule> GetExecutableRules(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<WorkItemFieldRule>) RuleEngine.PrepareRulesForExecution((IEnumerable<WorkItemFieldRule>) this.GetCombinedFieldRules(requestContext)).ToList<WorkItemFieldRule>();
    }

    public virtual IReadOnlyCollection<WorkItemFieldRule> GetCombinedFieldRules(
      IVssRequestContext requestContext,
      bool includeDisabledRules = false)
    {
      Dictionary<int, WorkItemFieldRule> fieldRulesDictionary = new Dictionary<int, WorkItemFieldRule>();
      if (this.FieldRules != null)
        fieldRulesDictionary = this.FieldRules.ToDictionary<WorkItemFieldRule, int>((Func<WorkItemFieldRule, int>) (rr => rr.FieldId));
      this.GetAllowedValuesRules(requestContext).MergeIntoDictionary((IDictionary<int, WorkItemFieldRule>) fieldRulesDictionary);
      this.GetSuggestedValuesRules(requestContext).MergeIntoDictionary((IDictionary<int, WorkItemFieldRule>) fieldRulesDictionary);
      if (this.ParentWorkItemType != null && this.ParentWorkItemType.FieldRules != null)
        this.ParentWorkItemType.GetCombinedFieldRules(requestContext).MergeIntoDictionary((IDictionary<int, WorkItemFieldRule>) fieldRulesDictionary);
      if (!this.IsDerived && !this.IsAbstract)
      {
        IWorkItemStateDefinitionService service = requestContext.GetService<IWorkItemStateDefinitionService>();
        service.GenerateWorkFlowRules(requestContext, this.ProcessId, this.ReferenceName).MergeIntoDictionary((IDictionary<int, WorkItemFieldRule>) fieldRulesDictionary);
        service.GenerateStateCategoryTransitionRules(requestContext, this).MergeIntoDictionary((IDictionary<int, WorkItemFieldRule>) fieldRulesDictionary);
        if (this.GetCombinedFields(requestContext).Any<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, "Microsoft.VSTS.Common.StateChangeDate"))))
          service.GenerateStateChangeDateFieldRules(requestContext).MergeIntoDictionary((IDictionary<int, WorkItemFieldRule>) fieldRulesDictionary);
      }
      List<WorkItemFieldRule> list = fieldRulesDictionary.Values.ToList<WorkItemFieldRule>();
      if (!includeDisabledRules && this.DisabledRules.Any<Guid>())
      {
        HashSet<Guid> ruleIdsToRemove = new HashSet<Guid>(this.DisabledRules);
        foreach (RuleBlock ruleBlock in list)
          ruleBlock.RemoveRules((ISet<Guid>) ruleIdsToRemove);
      }
      return (IReadOnlyCollection<WorkItemFieldRule>) list;
    }

    public IReadOnlyCollection<WorkItemStateDefinition> GetStates(IVssRequestContext requestContext) => requestContext.TraceBlock<IReadOnlyCollection<WorkItemStateDefinition>>(910403, 910404, nameof (ProcessWorkItemType), "Service", nameof (GetStates), (Func<IReadOnlyCollection<WorkItemStateDefinition>>) (() => requestContext.GetService<IWorkItemStateDefinitionService>().GetCombinedStateDefinitions(requestContext, this.ProcessId, this.ReferenceName)));

    public IReadOnlyCollection<WorkItemStateDefinition> GetDeltaStates(
      IVssRequestContext requestContext)
    {
      return requestContext.TraceBlock<IReadOnlyCollection<WorkItemStateDefinition>>(910405, 910406, nameof (ProcessWorkItemType), "Service", nameof (GetDeltaStates), (Func<IReadOnlyCollection<WorkItemStateDefinition>>) (() => requestContext.GetService<IWorkItemStateDefinitionService>().GetDeltaStateDefinitions(requestContext, this)));
    }

    private bool IsRuleAvailableForField<T>(IVssRequestContext requestContext, string fieldRefName)
    {
      if (this.FieldRules != null)
      {
        foreach (WorkItemFieldRule fieldRule in this.FieldRules)
        {
          if (TFStringComparer.WorkItemFieldReferenceName.Equals(fieldRule.Field, fieldRefName))
          {
            WorkItemRule[] subRules = fieldRule.SubRules;
            if (subRules != null)
            {
              foreach (WorkItemRule workItemRule in subRules)
              {
                if (workItemRule is T)
                  return true;
              }
            }
          }
        }
      }
      return false;
    }

    private IReadOnlyCollection<WorkItemFieldRule> GetAllowedValuesRules(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<WorkItemFieldRule>) requestContext.TraceBlock<List<WorkItemFieldRule>>(910401, 910402, nameof (ProcessWorkItemType), "Service", nameof (GetAllowedValuesRules), (Func<List<WorkItemFieldRule>>) (() =>
      {
        bool flag = WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);
        Dictionary<int, WorkItemFieldRule> dictionary = new Dictionary<int, WorkItemFieldRule>();
        if (this.Fields != null)
        {
          IWorkItemPickListService service = requestContext.GetService<IWorkItemPickListService>();
          foreach (FieldEntry field in this.Fields.Select<WorkItemTypeExtensionFieldEntry, FieldEntry>((Func<WorkItemTypeExtensionFieldEntry, FieldEntry>) (ef => ef.Field)))
          {
            IReadOnlyCollection<string> values = (IReadOnlyCollection<string>) null;
            if (flag && field.IsPicklist)
            {
              Guid? pickListId = field.PickListId;
              if (pickListId.HasValue)
              {
                IWorkItemPickListService itemPickListService = service;
                IVssRequestContext requestContext1 = requestContext;
                pickListId = field.PickListId;
                Guid listId = pickListId.Value;
                WorkItemPickList workItemPickList;
                ref WorkItemPickList local = ref workItemPickList;
                if (itemPickListService.TryGetList(requestContext1, listId, out local) && !workItemPickList.IsSuggested(requestContext, this.Id, field.FieldId))
                  dictionary.Add(field.FieldId, this.CreateAllowedValuesRule(field, workItemPickList.Items.Select<WorkItemPickListMember, string>((Func<WorkItemPickListMember, string>) (i => i.Value))));
              }
            }
            else if (!this.IsRuleAvailableForField<AllowedValuesRule>(requestContext, field.ReferenceName) && OOBFieldValues.TryGetAllowedValues(requestContext, field.ReferenceName, out values))
              dictionary.Add(field.FieldId, this.CreateAllowedValuesRule(field, (IEnumerable<string>) values));
          }
        }
        return dictionary.Values.ToList<WorkItemFieldRule>();
      }));
    }

    private IReadOnlyCollection<WorkItemFieldRule> GetSuggestedValuesRules(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<WorkItemFieldRule>) requestContext.TraceBlock<List<WorkItemFieldRule>>(910407, 910408, nameof (ProcessWorkItemType), "Service", nameof (GetSuggestedValuesRules), (Func<List<WorkItemFieldRule>>) (() =>
      {
        bool flag = WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);
        Dictionary<int, WorkItemFieldRule> dictionary = new Dictionary<int, WorkItemFieldRule>();
        if (this.Fields != null)
        {
          IWorkItemPickListService service = requestContext.GetService<IWorkItemPickListService>();
          foreach (FieldEntry field in this.Fields.Select<WorkItemTypeExtensionFieldEntry, FieldEntry>((Func<WorkItemTypeExtensionFieldEntry, FieldEntry>) (ef => ef.Field)))
          {
            IReadOnlyCollection<string> values = (IReadOnlyCollection<string>) null;
            if (flag && field.IsPicklist)
            {
              Guid? pickListId = field.PickListId;
              if (pickListId.HasValue)
              {
                IWorkItemPickListService itemPickListService = service;
                IVssRequestContext requestContext1 = requestContext;
                pickListId = field.PickListId;
                Guid listId = pickListId.Value;
                WorkItemPickList workItemPickList;
                ref WorkItemPickList local = ref workItemPickList;
                if (itemPickListService.TryGetList(requestContext1, listId, out local) && workItemPickList.IsSuggested(requestContext, this.Id, field.FieldId))
                  dictionary.Add(field.FieldId, this.CreateSuggestedValuesRule(field, workItemPickList.Items.Select<WorkItemPickListMember, string>((Func<WorkItemPickListMember, string>) (i => i.Value))));
              }
            }
            else if (!this.IsRuleAvailableForField<SuggestedValuesRule>(requestContext, field.ReferenceName) && OOBFieldValues.TryGetSuggestedValues(requestContext, field.ReferenceName, out values))
              dictionary.Add(field.FieldId, this.CreateSuggestedValuesRule(field, (IEnumerable<string>) values));
          }
        }
        return dictionary.Values.ToList<WorkItemFieldRule>();
      }));
    }

    private WorkItemFieldRule CreateAllowedValuesRule(FieldEntry field, IEnumerable<string> values)
    {
      WorkItemFieldRule allowedValuesRule = new WorkItemFieldRule();
      allowedValuesRule.Field = field.ReferenceName;
      allowedValuesRule.FieldId = field.FieldId;
      AllowedValuesRule rule = new AllowedValuesRule();
      rule.Values = new HashSet<string>(values, (IEqualityComparer<string>) TFStringComparer.AllowedValue);
      allowedValuesRule.AddRule<AllowedValuesRule>(rule);
      allowedValuesRule.AddRule<AllowExistingValueRule>(new AllowExistingValueRule());
      return allowedValuesRule;
    }

    private WorkItemFieldRule CreateSuggestedValuesRule(
      FieldEntry field,
      IEnumerable<string> values)
    {
      WorkItemFieldRule suggestedValuesRule = new WorkItemFieldRule();
      suggestedValuesRule.Field = field.ReferenceName;
      suggestedValuesRule.FieldId = field.FieldId;
      SuggestedValuesRule rule = new SuggestedValuesRule();
      rule.Values = new HashSet<string>(values, (IEqualityComparer<string>) TFStringComparer.AllowedValue);
      suggestedValuesRule.AddRule<SuggestedValuesRule>(rule);
      suggestedValuesRule.AddRule<AllowExistingValueRule>(new AllowExistingValueRule());
      return suggestedValuesRule;
    }

    internal static ProcessWorkItemType Create(
      string name,
      string description,
      string referenceName,
      string parentRefName,
      string color,
      string icon,
      bool isDisabled)
    {
      ProcessWorkItemType processWorkItemType = new ProcessWorkItemType();
      processWorkItemType.Name = name;
      processWorkItemType.Description = description;
      processWorkItemType.ReferenceName = referenceName;
      processWorkItemType.ParentTypeRefName = parentRefName;
      processWorkItemType.Color = color;
      processWorkItemType.Icon = icon;
      processWorkItemType.IsDisabled = isDisabled;
      return processWorkItemType;
    }

    internal static ProcessWorkItemType Create(
      IVssRequestContext requestContext,
      WorkItemTypeletRecord record,
      WorkItemTrackingFieldService fieldDict)
    {
      ProcessWorkItemType typelet = new ProcessWorkItemType();
      typelet.Id = record.Id;
      typelet.Name = record.Name;
      typelet.Description = record.Description;
      typelet.LastChangedDate = record.LastChangeDate;
      typelet.Form = ProcessWorkItemType.GetFormLayout(requestContext, record.Form);
      typelet.ProcessId = record.ProcessId;
      typelet.ParentTypeRefName = record.ParentTypeRefName;
      typelet.ReferenceName = record.ReferenceName;
      typelet.IsDeleted = record.IsDeleted;
      typelet.Color = record.Color;
      typelet.Icon = record.Icon;
      typelet.IsAbstract = record.IsAbstract;
      typelet.IsDisabled = record.Disabled;
      if (record.Fields != null)
        typelet.Fields = (IEnumerable<WorkItemTypeExtensionFieldEntry>) ((IEnumerable<WorkItemTypeletFieldRecord>) record.Fields).Select<WorkItemTypeletFieldRecord, WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeletFieldRecord, WorkItemTypeExtensionFieldEntry>) (field =>
        {
          return new WorkItemTypeExtensionFieldEntry(typelet.Id, fieldDict.GetFieldById(requestContext, field.FieldId, new bool?(true)) ?? throw new WorkItemTrackingFieldDefinitionNotFoundException(field.FieldId));
        })).ToList<WorkItemTypeExtensionFieldEntry>();
      if (!string.IsNullOrEmpty(record.Rules))
      {
        WorkItemFieldRule[] fieldRules = TeamFoundationSerializationUtility.Deserialize<WorkItemFieldRule[]>(record.Rules, new XmlRootAttribute("rules"));
        ProcessWorkItemType.FixFieldRules(requestContext, (IEnumerable<WorkItemFieldRule>) fieldRules);
        ProcessWorkItemType.FixIdentityRules(requestContext, fieldRules);
        typelet.FieldRules = (IEnumerable<WorkItemFieldRule>) fieldRules;
        IEnumerable<Guid> disabledCustomRules = ProcessWorkItemType.GetDisabledCustomRules((IEnumerable<WorkItemRule>) typelet.FieldRules);
        typelet.DisabledRules = disabledCustomRules;
        typelet.ForNotGroups = ProcessWorkItemType.GetForNotGroups(requestContext, typelet.FieldRules);
      }
      if (!string.IsNullOrEmpty(record.DisabledRules))
      {
        Guid[] second = TeamFoundationSerializationUtility.Deserialize<Guid[]>(record.DisabledRules);
        typelet.DisabledRules = typelet.DisabledRules != null ? typelet.DisabledRules.Concat<Guid>((IEnumerable<Guid>) second) : (IEnumerable<Guid>) second;
      }
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && record.Behaviors != null)
        typelet.m_behaviorRelations = (IReadOnlyDictionary<string, bool>) ((IEnumerable<WorkItemTypeExtensionBehaviorRecord>) record.Behaviors).ToDictionary<WorkItemTypeExtensionBehaviorRecord, string, bool>((Func<WorkItemTypeExtensionBehaviorRecord, string>) (br => br.BehaviorReferenceName), (Func<WorkItemTypeExtensionBehaviorRecord, bool>) (br => br.IsDefault), (IEqualityComparer<string>) TFStringComparer.BehaviorReferenceName);
      return typelet;
    }

    internal static ProcessWorkItemType Create(
      ProcessWorkItemType typelet,
      IEnumerable<WorkItemFieldRule> rules)
    {
      ProcessWorkItemType processWorkItemType = (ProcessWorkItemType) typelet.Clone();
      processWorkItemType.FieldRules = rules;
      return processWorkItemType;
    }

    private static IEnumerable<Guid> GetDisabledCustomRules(IEnumerable<WorkItemRule> rules)
    {
      List<Guid> disabledCustomRules = new List<Guid>();
      foreach (WorkItemRule rule in rules)
      {
        if (rule.IsDisabled)
          disabledCustomRules.Add(rule.Id);
        if (rule is RuleBlock ruleBlock && ruleBlock.SubRules != null && ((IEnumerable<WorkItemRule>) ruleBlock.SubRules).Any<WorkItemRule>())
          disabledCustomRules.AddRange(ProcessWorkItemType.GetDisabledCustomRules((IEnumerable<WorkItemRule>) ruleBlock.SubRules));
      }
      return (IEnumerable<Guid>) disabledCustomRules;
    }

    private static Layout GetFormLayout(IVssRequestContext requestContext, string form) => string.IsNullOrEmpty(form) ? new Layout() : JsonConvert.DeserializeObject<Layout>(form);

    public override void ResolveTypeReference(
      IReadOnlyCollection<ProcessTypelet> processTypelets)
    {
      this.ParentWorkItemType = processTypelets.OfType<ProcessWorkItemType>().FirstOrDefault<ProcessWorkItemType>((Func<ProcessWorkItemType, bool>) (t => TFStringComparer.WorkItemTypeReferenceName.Equals(t.ReferenceName, this.ParentTypeRefName)));
      if (this.m_behaviorRelations != null)
        this.BehaviorRelations = (IReadOnlyCollection<BehaviorRelation>) this.m_behaviorRelations.Select<KeyValuePair<string, bool>, Behavior>((Func<KeyValuePair<string, bool>, Behavior>) (b => processTypelets.OfType<Behavior>().FirstOrDefault<Behavior>((Func<Behavior, bool>) (t => TFStringComparer.BehaviorReferenceName.Equals(t.ReferenceName, b.Key))))).Where<Behavior>((Func<Behavior, bool>) (b => b != null)).ToList<Behavior>().Select<Behavior, BehaviorRelation>((Func<Behavior, BehaviorRelation>) (b => new BehaviorRelation()
        {
          Behavior = b,
          IsDefault = this.m_behaviorRelations[b.ReferenceName]
        })).ToList<BehaviorRelation>();
      else
        this.BehaviorRelations = (IReadOnlyCollection<BehaviorRelation>) new List<BehaviorRelation>();
    }

    public override ProcessTypelet Clone() => (ProcessTypelet) this.MemberwiseClone();

    protected ProcessWorkItemType()
    {
    }

    private static void FixFieldRules(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemFieldRule> fieldRules)
    {
      if (fieldRules == null || !fieldRules.Any<WorkItemFieldRule>())
        return;
      ProcessTypeletRuleValidationContext validationHelper = new ProcessTypeletRuleValidationContext(requestContext);
      foreach (WorkItemRule fieldRule in fieldRules)
        fieldRule.FixFieldReferences((IRuleValidationContext) validationHelper);
    }

    private static IEnumerable<Guid> GetForNotGroups(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemFieldRule> fieldRules)
    {
      HashSet<Guid> forNotVsids = new HashSet<Guid>();
      if (fieldRules != null)
      {
        foreach (WorkItemRule fieldRule in fieldRules)
          fieldRule.Walk((Func<WorkItemRule, bool>) (rule =>
          {
            if (rule.ForVsId != Guid.Empty)
              forNotVsids.Add(rule.ForVsId);
            if (rule.NotVsId != Guid.Empty)
              forNotVsids.Add(rule.NotVsId);
            return true;
          }));
      }
      return (IEnumerable<Guid>) forNotVsids;
    }

    private static void FixIdentityRules(
      IVssRequestContext requestContext,
      WorkItemFieldRule[] fieldRules)
    {
      if (fieldRules == null || !((IEnumerable<WorkItemFieldRule>) fieldRules).Any<WorkItemFieldRule>())
        return;
      foreach (WorkItemRule fieldRule in fieldRules)
        fieldRule.Accept((IRuleVisitor) new ResolveVsidVisitor(requestContext));
    }
  }
}
