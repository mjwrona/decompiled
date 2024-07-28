// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ComposedWorkItemType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ComposedWorkItemType : ProcessTypelet
  {
    private ProcessWorkItemTypeDefinition m_legacyWorkItem;
    private ProcessWorkItemType m_workItemType;

    public bool IsDisabled { get; protected set; }

    public Guid ParentProcessId { get; private set; }

    public virtual string ParentTypeRefName { get; protected set; }

    public virtual ProcessWorkItemTypeDefinition ParentWorkItemType { get; protected set; }

    public virtual ProcessWorkItemType DerivedWorkItemType { get; protected set; }

    public virtual ProcessWorkItemType CustomWorkItemType { get; protected set; }

    public virtual bool IsDerived => !string.IsNullOrEmpty(this.ParentTypeRefName) && !TFStringComparer.WorkItemTypeReferenceName.Equals(this.ParentTypeRefName, SystemWorkItemType.ReferenceName);

    public virtual bool IsCustomType { get; private set; }

    public override Layout Form => throw new InvalidOperationException("Call GetFormLayout()");

    public virtual LayoutInfo GetFormLayoutInfo(
      IVssRequestContext requestContext,
      bool resolveContributions = true)
    {
      if (this.IsCustomType)
      {
        LayoutInfo formLayoutInfo = this.CustomWorkItemType.GetFormLayoutInfo(requestContext, resolveContributions);
        formLayoutInfo.ComposedLayout.ShowEmptyReadOnlyFields = new bool?(true);
        return formLayoutInfo;
      }
      if (this.IsDerived)
      {
        ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, this.ProcessId);
        IFormLayoutService service = requestContext.GetService<IFormLayoutService>();
        Layout form = this.m_legacyWorkItem.Form;
        Layout deltaLayout = this.DerivedWorkItemType.Form;
        if (!processDescriptor.IsCustom & resolveContributions)
          deltaLayout = FormExtensionsTransformer.AddContributions(requestContext, form, deltaLayout);
        Layout composedLayout = service.CombineLayouts(requestContext, form, deltaLayout);
        composedLayout.ShowEmptyReadOnlyFields = new bool?(true);
        return new LayoutInfo(composedLayout, form, deltaLayout);
      }
      Layout layout = this.m_legacyWorkItem.Form;
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && !requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, this.ProcessId).IsCustom & resolveContributions)
      {
        layout = FormExtensionsTransformer.AddContributions(requestContext, layout, layout);
        layout.ShowEmptyReadOnlyFields = new bool?(true);
      }
      return new LayoutInfo(layout, layout, new Layout());
    }

    public virtual Layout GetFormLayout(
      IVssRequestContext requestContext,
      bool resolveContributions = true)
    {
      return this.GetFormLayoutInfo(requestContext, resolveContributions).ComposedLayout;
    }

    public virtual IReadOnlyCollection<ProcessFieldResult> GetLegacyFields(
      IVssRequestContext requestContext)
    {
      return ComposedWorkItemType.GetFields(requestContext, this.m_legacyWorkItem, this.m_workItemType);
    }

    public virtual IReadOnlyCollection<WorkItemFieldRule> GetFieldRules(
      IVssRequestContext requestContext,
      bool includeDisabledRules = false)
    {
      if (this.IsDerived)
        return ComposedWorkItemType.GetFieldRulesInternal(requestContext, this.ParentWorkItemType, this.DerivedWorkItemType, includeDisabledRules);
      return this.m_workItemType != null ? ComposedWorkItemType.GetFieldRulesInternal(requestContext, (ProcessWorkItemTypeDefinition) null, this.m_workItemType, includeDisabledRules) : ComposedWorkItemType.GetFieldRulesInternal(requestContext, this.m_legacyWorkItem, (ProcessWorkItemType) null, includeDisabledRules);
    }

    internal static ComposedWorkItemType Create(
      string name,
      string description,
      string referenceName,
      string parentRefName,
      string color,
      string icon,
      bool isDisabled)
    {
      ComposedWorkItemType composedWorkItemType = new ComposedWorkItemType();
      composedWorkItemType.Name = name;
      composedWorkItemType.Description = description;
      composedWorkItemType.ReferenceName = referenceName;
      composedWorkItemType.ParentTypeRefName = parentRefName;
      composedWorkItemType.Color = color;
      composedWorkItemType.Icon = icon;
      composedWorkItemType.IsDisabled = isDisabled;
      composedWorkItemType.IsCustomType = !string.IsNullOrEmpty(parentRefName) && TFStringComparer.WorkItemTypeReferenceName.Equals(parentRefName, SystemWorkItemType.ReferenceName);
      return composedWorkItemType;
    }

    internal static ComposedWorkItemType Create(
      IVssRequestContext requestContext,
      ProcessWorkItemTypeDefinition legacyWorkItemType)
    {
      IEnumerable<WorkItemFieldRule> systemFieldRules = ComposedWorkItemType.GetSystemFieldRules(requestContext, legacyWorkItemType);
      ComposedWorkItemType composedWorkItemType = new ComposedWorkItemType();
      composedWorkItemType.Name = legacyWorkItemType.Name;
      composedWorkItemType.ReferenceName = legacyWorkItemType.ReferenceName;
      composedWorkItemType.Description = legacyWorkItemType.Description;
      composedWorkItemType.ProcessId = legacyWorkItemType.ProcessId;
      composedWorkItemType.FieldRules = systemFieldRules;
      composedWorkItemType.m_legacyWorkItem = legacyWorkItemType;
      composedWorkItemType.Color = legacyWorkItemType.Color;
      composedWorkItemType.Icon = legacyWorkItemType.Icon ?? WorkItemTypeIconUtils.GetDefaultIcon();
      composedWorkItemType.IsCustomType = false;
      composedWorkItemType.IsDisabled = false;
      return composedWorkItemType;
    }

    internal static ComposedWorkItemType Create(
      IVssRequestContext requestContext,
      ProcessWorkItemType workItemType,
      ProcessWorkItemTypeDefinition parentworkItemType)
    {
      ComposedWorkItemType composedWorkItemType1 = new ComposedWorkItemType();
      composedWorkItemType1.Name = workItemType.Name;
      composedWorkItemType1.ReferenceName = workItemType.ReferenceName;
      composedWorkItemType1.Description = workItemType.Description;
      composedWorkItemType1.ProcessId = workItemType.ProcessId;
      composedWorkItemType1.FieldRules = workItemType.FieldRules;
      ComposedWorkItemType composedWorkItemType2 = composedWorkItemType1;
      composedWorkItemType2.ParentTypeRefName = workItemType.ParentTypeRefName;
      composedWorkItemType2.m_workItemType = workItemType;
      if (parentworkItemType != null)
      {
        composedWorkItemType2.ParentProcessId = parentworkItemType.ProcessId;
        composedWorkItemType2.ParentWorkItemType = parentworkItemType;
        composedWorkItemType2.DerivedWorkItemType = workItemType;
        composedWorkItemType2.m_legacyWorkItem = parentworkItemType;
        composedWorkItemType2.IsCustomType = false;
      }
      else
      {
        composedWorkItemType2.IsCustomType = true;
        composedWorkItemType2.CustomWorkItemType = workItemType;
      }
      composedWorkItemType2.Color = ComposedWorkItemType.GetColor(workItemType.Color, parentworkItemType?.Color);
      composedWorkItemType2.Icon = (workItemType.Icon ?? parentworkItemType?.Icon) ?? WorkItemTypeIconUtils.GetDefaultIcon();
      composedWorkItemType2.IsDisabled = workItemType.IsDisabled;
      return composedWorkItemType2;
    }

    private static IReadOnlyCollection<FieldEntry> GetNonIgnoredCoreFields(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<FieldEntry>) requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext).GetCoreFields().Where<FieldEntry>((Func<FieldEntry, bool>) (f => !f.IsIgnored && f.Usage == InternalFieldUsages.WorkItem)).ToList<FieldEntry>();
    }

    private static IReadOnlyCollection<ProcessFieldResult> GetFields(
      IVssRequestContext requestContext,
      ProcessWorkItemTypeDefinition legacyWorkItemType,
      ProcessWorkItemType workItemType)
    {
      HashSet<ProcessFieldResult> processFieldResultSet = new HashSet<ProcessFieldResult>();
      IDictionary<string, string> systemProcessFields = requestContext.GetService<IProcessFieldService>().GetAllOutOfBoxFieldReferenceNameToNameMappings(requestContext);
      if (workItemType != null)
      {
        if (workItemType.Fields != null)
          processFieldResultSet.UnionWith(workItemType.GetCombinedFields(requestContext).Select<FieldEntry, ProcessFieldResult>((Func<FieldEntry, ProcessFieldResult>) (f => new ProcessFieldResult()
          {
            Name = f.Name,
            ReferenceName = f.ReferenceName,
            Description = f.Description,
            Type = f.FieldType,
            ProcessId = workItemType.ProcessId,
            IsSystem = systemProcessFields.ContainsKey(f.ReferenceName),
            IsBehaviorField = false,
            IsIdentity = f.IsIdentity,
            IsLocked = f.IsLocked
          })));
        if (workItemType.BehaviorRelations != null)
        {
          foreach (BehaviorRelation behaviorRelation in (IEnumerable<BehaviorRelation>) workItemType.BehaviorRelations)
          {
            Behavior behavior = behaviorRelation.Behavior;
            IEnumerable<ProcessFieldResult> processFieldResults = behavior.GetLegacyCombinedFields(requestContext).Select<KeyValuePair<string, ProcessFieldDefinition>, ProcessFieldResult>((Func<KeyValuePair<string, ProcessFieldDefinition>, ProcessFieldResult>) (f => new ProcessFieldResult()
            {
              Name = f.Value.Name,
              ReferenceName = f.Value.ReferenceName,
              Description = f.Value.HelpText,
              Type = f.Value.Type,
              ProcessId = behavior.ProcessId,
              IsSystem = systemProcessFields.ContainsKey(f.Value.ReferenceName),
              IsBehaviorField = true,
              IsLocked = f.Value.IsLocked
            }));
            processFieldResultSet.Intersect<ProcessFieldResult>(processFieldResults).ToList<ProcessFieldResult>().ForEach((Action<ProcessFieldResult>) (f => f.IsBehaviorField = true));
            processFieldResultSet.UnionWith(processFieldResults);
          }
        }
      }
      if (legacyWorkItemType != null && legacyWorkItemType.FieldDefinitions != null)
        processFieldResultSet.UnionWith(legacyWorkItemType.FieldDefinitions.Select<ProcessFieldDefinition, ProcessFieldResult>((Func<ProcessFieldDefinition, ProcessFieldResult>) (f => new ProcessFieldResult()
        {
          Name = f.Name,
          ReferenceName = f.ReferenceName,
          Description = f.HelpText,
          Type = f.Type,
          ProcessId = f.ProcessId,
          IsSystem = systemProcessFields.ContainsKey(f.ReferenceName),
          IsIdentity = f.IsIdentity,
          IsLocked = f.IsLocked
        })));
      foreach (FieldEntry ignoredCoreField in (IEnumerable<FieldEntry>) ComposedWorkItemType.GetNonIgnoredCoreFields(requestContext))
      {
        FieldEntry coreField = ignoredCoreField;
        if (!processFieldResultSet.Any<ProcessFieldResult>((Func<ProcessFieldResult, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, coreField.ReferenceName))))
          processFieldResultSet.Add(new ProcessFieldResult()
          {
            Name = coreField.Name,
            ReferenceName = coreField.ReferenceName,
            Description = coreField.Description,
            Type = coreField.FieldType,
            IsSystem = true,
            ProcessId = legacyWorkItemType != null ? legacyWorkItemType.ProcessId : workItemType.ProcessId,
            IsIdentity = coreField.IsIdentity,
            IsLocked = coreField.IsLocked
          });
      }
      return (IReadOnlyCollection<ProcessFieldResult>) processFieldResultSet.ToList<ProcessFieldResult>();
    }

    private static IReadOnlyCollection<WorkItemFieldRule> GetFieldRulesInternal(
      IVssRequestContext requestContext,
      ProcessWorkItemTypeDefinition legacyWorkItemType,
      ProcessWorkItemType workItemType,
      bool includeDisabledRules)
    {
      Dictionary<int, WorkItemFieldRule> rules = new Dictionary<int, WorkItemFieldRule>();
      HashSet<Guid> disabledRules = new HashSet<Guid>();
      bool isProcessCustomizationEnabled = WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);
      Action<Guid, string, IEnumerable<WorkItemFieldRule>> action = (Action<Guid, string, IEnumerable<WorkItemFieldRule>>) ((processId, witRefName, additionalRules) =>
      {
        foreach (WorkItemFieldRule additionalRule in additionalRules)
        {
          if (disabledRules != null && disabledRules.Any<Guid>())
          {
            if (!disabledRules.Contains(additionalRule.Id))
              additionalRule.RemoveRules((ISet<Guid>) disabledRules);
            else
              continue;
          }
          if (additionalRule.FieldId == 0)
          {
            if (isProcessCustomizationEnabled)
            {
              FieldEntry field;
              if (!requestContext.WitContext().FieldDictionary.TryGetField(additionalRule.Field, out field))
                throw new ComposedWorkItemTypeFieldIdIsNotPresentException(processId, witRefName, additionalRule.Field);
              additionalRule.FieldId = field.FieldId;
            }
            else
              continue;
          }
          WorkItemFieldRule workItemFieldRule;
          if (rules.TryGetValue(additionalRule.FieldId, out workItemFieldRule))
          {
            foreach (WorkItemRule subRule1 in additionalRule.SubRules)
            {
              if (subRule1 is WorkItemTypeScopedRules)
              {
                foreach (WorkItemRule subRule2 in (subRule1 as WorkItemTypeScopedRules).SubRules)
                  workItemFieldRule.AddRule<WorkItemRule>(subRule2);
              }
              else
                workItemFieldRule.AddRule<WorkItemRule>(subRule1);
            }
          }
          else
          {
            IEnumerable<WorkItemRule> workItemRules = ((IEnumerable<WorkItemRule>) additionalRule.SubRules).Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is WorkItemTypeScopedRules));
            additionalRule.SubRules = ((IEnumerable<WorkItemRule>) additionalRule.SubRules).Except<WorkItemRule>(workItemRules).ToArray<WorkItemRule>();
            foreach (WorkItemRule rule in workItemRules.SelectMany<WorkItemRule, WorkItemRule>((Func<WorkItemRule, IEnumerable<WorkItemRule>>) (r => (IEnumerable<WorkItemRule>) (r as WorkItemTypeScopedRules).SubRules)))
              additionalRule.AddRule<WorkItemRule>(rule);
            rules.Add(additionalRule.FieldId, additionalRule);
          }
        }
      });
      if (workItemType != null)
      {
        IEnumerable<WorkItemFieldRule> combinedFieldRules = (IEnumerable<WorkItemFieldRule>) workItemType.GetCombinedFieldRules(requestContext, includeDisabledRules);
        if (combinedFieldRules != null)
        {
          ComposedWorkItemType.SetIsSystemFlag(combinedFieldRules, false);
          action(workItemType.ProcessId, workItemType.ReferenceName, combinedFieldRules);
        }
        disabledRules = new HashSet<Guid>(workItemType.DisabledRules);
      }
      if (legacyWorkItemType != null)
      {
        IEnumerable<WorkItemFieldRule> systemFieldRules = ComposedWorkItemType.GetSystemFieldRules(requestContext, legacyWorkItemType);
        ComposedWorkItemType.SetIsSystemFlag(systemFieldRules, true);
        action(legacyWorkItemType.ProcessId, legacyWorkItemType.ReferenceName, systemFieldRules);
      }
      return (IReadOnlyCollection<WorkItemFieldRule>) rules.Values.ToList<WorkItemFieldRule>();
    }

    private static IEnumerable<WorkItemFieldRule> GetSystemFieldRules(
      IVssRequestContext requestContext,
      ProcessWorkItemTypeDefinition legacyWorkItemType)
    {
      return (IEnumerable<WorkItemFieldRule>) requestContext.TraceBlock<IReadOnlyCollection<WorkItemFieldRule>>(911324, 911325, "ProcessWorkItemType", nameof (ComposedWorkItemType), nameof (GetSystemFieldRules), (Func<IReadOnlyCollection<WorkItemFieldRule>>) (() =>
      {
        IReadOnlyCollection<WorkItemFieldRule> systemFieldRules = (IReadOnlyCollection<WorkItemFieldRule>) null;
        string empty = string.Empty;
        bool flag = false;
        string key = string.Join("|", new string[3]
        {
          "OOBRules",
          legacyWorkItemType.ProcessId.ToString(),
          legacyWorkItemType.ReferenceName
        });
        if (requestContext.Items.TryGetValue<string, IReadOnlyCollection<WorkItemFieldRule>>(key, out systemFieldRules))
          return systemFieldRules;
        if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
        {
          ITeamFoundationProcessService service1 = requestContext.GetService<ITeamFoundationProcessService>();
          IWorkItemRulesService service2 = requestContext.GetService<IWorkItemRulesService>();
          IVssRequestContext requestContext1 = requestContext;
          Guid processId = legacyWorkItemType.ProcessId;
          ProcessDescriptor processDescriptor = service1.GetProcessDescriptor(requestContext1, processId);
          IReadOnlyCollection<WorkItemFieldRule> rules;
          if (!processDescriptor.IsCustom && service2.TryGetOutOfBoxRules(requestContext, processDescriptor, legacyWorkItemType.ReferenceName, out rules))
          {
            systemFieldRules = rules == null ? rules : (IReadOnlyCollection<WorkItemFieldRule>) rules.ToList<WorkItemFieldRule>().Select<WorkItemFieldRule, WorkItemFieldRule>((Func<WorkItemFieldRule, WorkItemFieldRule>) (r => r.Clone(false) as WorkItemFieldRule)).ToList<WorkItemFieldRule>();
            flag = true;
          }
        }
        if (!flag)
          systemFieldRules = (IReadOnlyCollection<WorkItemFieldRule>) WorkItemRulesService.ResolveFields(requestContext, legacyWorkItemType.FieldDefinitions.Select<ProcessFieldDefinition, WorkItemFieldRule>((Func<ProcessFieldDefinition, WorkItemFieldRule>) (f => f.ConvertToFieldRule())));
        requestContext.Items.Add(key, (object) systemFieldRules);
        return systemFieldRules;
      }));
    }

    private static string GetColor(string color, string parentColor) => !string.IsNullOrEmpty(color) ? color : parentColor;

    private static void SetIsSystemFlag(IEnumerable<WorkItemFieldRule> rules, bool isSystem)
    {
      foreach (WorkItemRule rule in rules)
        rule.Walk((Func<WorkItemRule, bool>) (currentRule =>
        {
          if (currentRule.Id != Guid.Empty)
            currentRule.IsSystem = new bool?(isSystem);
          return true;
        }));
    }
  }
}
