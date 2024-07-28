// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility.CompatibilityRulesGenerator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility
{
  public static class CompatibilityRulesGenerator
  {
    internal static readonly CommandPropertiesSetter CommandProperties = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithExecutionTimeout(TimeSpan.MaxValue);
    private static RuleRecord[] m_collectionScopedRules = new RuleRecord[19]
    {
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = 8,
        ThenConstID = -10003,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.DenyAdmin | RuleFlags.Unless | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = -2,
        ThenConstID = -10000,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.DenyAdmin | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = 25,
        ThenConstID = -10000,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.DenyAdmin | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = 2,
        ThenConstID = -10000,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.DenyAdmin | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = 22,
        ThenConstID = -10000,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.DenyAdmin | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = 3,
        ThenConstID = -10000,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.DenyAdmin | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        IfFldID = 2,
        IfConstID = -10001,
        ThenFldID = 22,
        ThenConstID = -10001,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = -2,
        ThenConstID = -10016,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.DenyAdmin | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = -2,
        ThenConstID = -10017,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = -104,
        ThenConstID = -10016,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.DenyAdmin | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = 33,
        ThenConstID = -10000,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.DenyAdmin | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        IfFldID = 33,
        IfConstID = -10000,
        ThenFldID = 33,
        ThenConstID = -10002,
        RuleFlags = RuleFlags.FlowdownTree | RuleFlags.Default
      },
      new RuleRecord()
      {
        PersonID = -1,
        IfFldID = 33,
        IfConstID = -10014,
        ThenFldID = 33,
        ThenConstID = -10002,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        IfFldID = 33,
        IfConstID = -10015,
        ThenFldID = 33,
        ThenConstID = -10001,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -1,
        IfFldID = 9,
        IfConstID = -10000,
        ThenFldID = 9,
        ThenConstID = -10002,
        RuleFlags = RuleFlags.FlowdownTree | RuleFlags.Default
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = 3,
        ThenConstID = -10030,
        RuleFlags = RuleFlags.DenyWrite | RuleFlags.DenyAdmin | RuleFlags.Unless | RuleFlags.FlowdownTree
      },
      new RuleRecord()
      {
        PersonID = -30,
        RuleFlags = RuleFlags.GrantRead | RuleFlags.GrantWrite | RuleFlags.GrantAdmin | RuleFlags.FlowdownTree | RuleFlags.SemiEditable
      },
      new RuleRecord()
      {
        PersonID = -1,
        RuleFlags = RuleFlags.GrantRead | RuleFlags.GrantWrite | RuleFlags.FlowdownTree | RuleFlags.SemiEditable
      },
      new RuleRecord()
      {
        PersonID = -1,
        ThenFldID = 33,
        ThenConstID = -2,
        RuleFlags = RuleFlags.ThenAllNodes | RuleFlags.FlowdownTree | RuleFlags.Suggestion
      }
    };

    public static IEnumerable<RuleRecord> GetRules(
      IVssRequestContext requestContext,
      int projectId,
      string workItemType)
    {
      IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
      List<RuleRecord> ruleRecordList = new List<RuleRecord>();
      TreeNode node;
      ProcessDescriptor processDescriptor;
      if (WorkItemTrackingFeatureFlags.IsSharedProcessEnabled(requestContext) && requestContext.WitContext().TreeService.LegacyTryGetTreeNode(projectId, out node) && service.TryGetProjectProcessDescriptor(requestContext, node.ProjectId, out processDescriptor) && !processDescriptor.IsCustom)
      {
        MetadataCompatibilityContext compatibilityContext = new MetadataCompatibilityContextGenerator().GetCompatibilityContext(requestContext, new Guid?(node.ProjectId), workItemType);
        ruleRecordList = CompatibilityRulesGenerator.GetOutOfBoxRules(requestContext, compatibilityContext).ToList<RuleRecord>();
        ruleRecordList = CompatibilityRulesGenerator.DeleteStateRulesForCustomizedWorkItemTypes(requestContext, compatibilityContext, ruleRecordList);
        ruleRecordList.AddRange((IEnumerable<RuleRecord>) CompatibilityRulesGenerator.GetCustomRules(requestContext, compatibilityContext));
        IDictionary<int, string> dictionary = (IDictionary<int, string>) compatibilityContext.ConstantMap.ToDictionary<KeyValuePair<string, int>, int, string>((Func<KeyValuePair<string, int>, int>) (x => x.Value), (Func<KeyValuePair<string, int>, string>) (x => x.Key));
        CompatibilityRulesGenerator.FillRulesRecordsWithConstants((IEnumerable<RuleRecord>) ruleRecordList, dictionary);
        requestContext.Trace(910605, TraceLevel.Info, nameof (CompatibilityRulesGenerator), nameof (GetRules), "GetRules was called and rules were generated by metadata-backcompat-generator.");
      }
      else
      {
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "WorkItemTracking.").AndCommandKey((CommandKey) "CompatibilityRulesGenerator.GetRules").AndCommandPropertiesDefaults(CompatibilityRulesGenerator.CommandProperties);
        ruleRecordList = new CommandService<List<RuleRecord>>(requestContext, setter, new Func<List<RuleRecord>>(Run)).Execute();
      }
      return (IEnumerable<RuleRecord>) ruleRecordList;

      List<RuleRecord> Run()
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          ruleRecordList = component.GetRules(projectId, workItemType).ToList<RuleRecord>();
        return ruleRecordList;
      }
    }

    public static IReadOnlyCollection<RuleRecord> GetOutOfBoxRules(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext)
    {
      List<RuleRecord> outOfBoxRules = new List<RuleRecord>();
      if (WorkItemTrackingFeatureFlags.IsFullRuleGenerationEnabled(requestContext))
        outOfBoxRules.AddRange((IEnumerable<RuleRecord>) CompatibilityRulesGenerator.m_collectionScopedRules);
      foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) compatContext.ProjectDescriptors)
      {
        ProcessDescriptor processDescriptor;
        if (projectDescriptor.TryGetProcessDescriptor(requestContext, out processDescriptor))
        {
          if (!processDescriptor.IsCustom)
          {
            foreach (MetadataWorkItemTypeCompatibilityDescriptor typeDescriptor in (IEnumerable<MetadataWorkItemTypeCompatibilityDescriptor>) projectDescriptor.TypeDescriptors)
              outOfBoxRules.AddRange(CompatibilityRulesGenerator.GetOutOfBoxRuleRecordsForWorkItemType(requestContext, projectDescriptor.ProjectNode.Id, processDescriptor, typeDescriptor, compatContext));
            string key = CompatibilityConstants.WorkItemTypeAllowedValueListHead(projectDescriptor.ProjectNode.Id);
            int setConstId;
            if (compatContext.ConstantMap.TryGetValue(key, out setConstId))
              outOfBoxRules.Add(CompatibilityRulesGenerator.CreateProjectScopedAllowedValuesRuleRecord(projectDescriptor.ProjectNode.Id, 25, setConstId));
            else
              MetadataCompatibilityContext.ReportError(requestContext, "GetCustomRules", string.Format("Constant not found for work item type allowed values list '{0}'.", (object) key));
          }
        }
        else
          requestContext.Trace(910601, TraceLevel.Warning, nameof (CompatibilityRulesGenerator), "GetCustomRules", string.Format("ProcessDescriptor not found for project '{0}'.", (object) projectDescriptor.ProjectNode.ProjectId));
      }
      return (IReadOnlyCollection<RuleRecord>) outOfBoxRules;
    }

    public static IReadOnlyCollection<RuleRecord> GetCustomRules(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext)
    {
      List<RuleRecord> customRules = new List<RuleRecord>();
      foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) compatContext.ProjectDescriptors)
      {
        foreach (MetadataWorkItemTypeCompatibilityDescriptor compatibilityDescriptor in projectDescriptor.TypeDescriptors.Where<MetadataWorkItemTypeCompatibilityDescriptor>((Func<MetadataWorkItemTypeCompatibilityDescriptor, bool>) (t => t.Type.IsDerived || t.Type.IsCustomType)))
        {
          customRules.AddRange(CompatibilityRulesGenerator.GetCustomRulesForWorkItemType(requestContext, projectDescriptor.ProjectNode.Id, compatibilityDescriptor, compatContext.ConstantMap));
          customRules.AddRange(CompatibilityRulesGenerator.GetCustomHelpTextRulesForWorkItemType(requestContext, projectDescriptor.ProjectNode.Id, compatibilityDescriptor, compatContext.ConstantMap));
          customRules.AddRange(CompatibilityRulesGenerator.GetRulesForPicklists(requestContext, projectDescriptor.ProjectNode.Id, compatibilityDescriptor, compatContext.ConstantMap));
          customRules.AddRange(CompatibilityRulesGenerator.GetRulesForCustomWorkItemType(requestContext, compatContext, projectDescriptor.ProjectNode.Id, compatibilityDescriptor));
          if (compatibilityDescriptor.AreStatesCustomized)
          {
            customRules.AddRange(CompatibilityRulesGenerator.GetCustomStateRules(requestContext, projectDescriptor.ProjectNode.Id, compatibilityDescriptor, compatContext.ConstantMap));
            customRules.AddRange(CompatibilityRulesGenerator.GenerateStateCategoryTransitionRules(requestContext, projectDescriptor.ProjectNode.Id, compatibilityDescriptor, compatContext.ConstantMap));
          }
          customRules.AddRange(CompatibilityRulesGenerator.GenerateStateChangeDateRules(requestContext, projectDescriptor.ProjectNode.Id, compatibilityDescriptor, compatContext.ConstantMap));
          customRules.AddRange(CompatibilityRulesGenerator.GetAllowedAndSuggestedValuesRulesForOOBFields(requestContext, compatContext, projectDescriptor.ProjectNode.Id, compatibilityDescriptor));
        }
      }
      return (IReadOnlyCollection<RuleRecord>) customRules;
    }

    public static Dictionary<int, HashSet<int>> GetProjectToTypesWithCustomizedStatesMap(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext)
    {
      Dictionary<int, HashSet<int>> customizedStatesMap = new Dictionary<int, HashSet<int>>();
      foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) compatContext.ProjectDescriptors)
      {
        foreach (MetadataWorkItemTypeCompatibilityDescriptor compatibilityDescriptor in projectDescriptor.TypeDescriptors.Where<MetadataWorkItemTypeCompatibilityDescriptor>((Func<MetadataWorkItemTypeCompatibilityDescriptor, bool>) (t => t.AreStatesCustomized)))
        {
          HashSet<int> intSet;
          if (!customizedStatesMap.TryGetValue(projectDescriptor.ProjectNode.Id, out intSet))
          {
            intSet = new HashSet<int>();
            customizedStatesMap[projectDescriptor.ProjectNode.Id] = intSet;
          }
          int num;
          if (compatContext.ConstantMap != null && compatContext.ConstantMap.TryGetValue(compatibilityDescriptor.Type.Name, out num))
            intSet.Add(num);
          else
            MetadataCompatibilityContext.ReportError(requestContext, nameof (GetProjectToTypesWithCustomizedStatesMap), string.Format("Constant not found for work item type '{0}'.", (object) compatibilityDescriptor.Type.Name));
        }
      }
      return customizedStatesMap;
    }

    internal static IEnumerable<RuleRecord> GetOutOfBoxRuleRecordsForWorkItemType(
      IVssRequestContext requestContext,
      int projectId,
      ProcessDescriptor processDescriptor,
      MetadataWorkItemTypeCompatibilityDescriptor workItemTypeContext,
      MetadataCompatibilityContext compatContext)
    {
      return requestContext.TraceBlock<IEnumerable<RuleRecord>>(910602, 910604, 910603, nameof (CompatibilityRulesGenerator), nameof (GetOutOfBoxRuleRecordsForWorkItemType), (Func<IEnumerable<RuleRecord>>) (() =>
      {
        if (processDescriptor.IsCustom)
          return Enumerable.Empty<RuleRecord>();
        if (compatContext.OutOfBoxRuleRecordsCache == null)
          compatContext.OutOfBoxRuleRecordsCache = (IDictionary<string, List<RuleRecord>>) new Dictionary<string, List<RuleRecord>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        string key = string.Format("{0}:{1}", (object) processDescriptor.TypeId.ToString("D"), (object) workItemTypeContext.Type.Name);
        List<RuleRecord> ruleRecordList;
        if (!compatContext.OutOfBoxRuleRecordsCache.TryGetValue(key, out ruleRecordList))
        {
          List<RuleRecord> recordsForWorkItemType = new List<RuleRecord>();
          IReadOnlyCollection<WorkItemFieldRule> oobRules;
          IReadOnlyCollection<HelpTextDescriptor> oobHelpTexts;
          if (workItemTypeContext.TryGetOutOfBoxRulesAndHelpTexts(requestContext, processDescriptor, out oobRules, out oobHelpTexts))
          {
            int typeNameConstId;
            if (compatContext.ConstantMap != null && compatContext.ConstantMap.TryGetValue(workItemTypeContext.Type.Name, out typeNameConstId))
            {
              foreach (WorkItemFieldRule fieldRule in (IEnumerable<WorkItemFieldRule>) oobRules)
              {
                if (fieldRule.FieldId != 2 || !workItemTypeContext.AreStatesCustomized)
                  recordsForWorkItemType.AddRange(CompatibilityRulesGenerator.GenerateRuleRecordsFromWorkItemFieldRules(requestContext, fieldRule, projectId, typeNameConstId, compatContext.ConstantMap));
              }
              foreach (HelpTextDescriptor helpTextDescriptor in (IEnumerable<HelpTextDescriptor>) oobHelpTexts)
              {
                int helpTextConstId;
                if (compatContext.ConstantMap != null && compatContext.ConstantMap.TryGetValue(helpTextDescriptor.HelpText, out helpTextConstId))
                  recordsForWorkItemType.Add(CompatibilityRulesGenerator.CreateHelpTextRuleRecord(projectId, typeNameConstId, helpTextDescriptor.FieldId, helpTextConstId));
                else
                  MetadataCompatibilityContext.ReportError(requestContext, nameof (GetOutOfBoxRuleRecordsForWorkItemType), string.Format("Constant not found for help text '{0}'.", (object) helpTextDescriptor.HelpText));
              }
              int propId;
              if (compatContext.TypeIdToFormPropIdMap != null && compatContext.TypeIdToFormPropIdMap.TryGetValue(workItemTypeContext.Type.Id.Value, out propId))
                recordsForWorkItemType.Add(CompatibilityRulesGenerator.CreateFormRule(projectId, typeNameConstId, propId));
              else
                MetadataCompatibilityContext.ReportError(requestContext, nameof (GetOutOfBoxRuleRecordsForWorkItemType), string.Format("PropID not found for work item type '{0}'.", (object) workItemTypeContext.Type.Name));
              compatContext.OutOfBoxRuleRecordsCache[key] = recordsForWorkItemType;
            }
            else
              MetadataCompatibilityContext.ReportError(requestContext, nameof (GetOutOfBoxRuleRecordsForWorkItemType), string.Format("Constant not found for work item type '{0}'.", (object) workItemTypeContext.Type.Name));
          }
          return (IEnumerable<RuleRecord>) recordsForWorkItemType;
        }
        List<RuleRecord> recordsForWorkItemType1 = new List<RuleRecord>();
        foreach (RuleRecord ruleRecord1 in ruleRecordList)
        {
          RuleRecord ruleRecord2 = ruleRecord1.Clone();
          ruleRecord2.RootTreeID = projectId;
          ruleRecord2.AreaID = projectId;
          recordsForWorkItemType1.Add(ruleRecord2);
        }
        return (IEnumerable<RuleRecord>) recordsForWorkItemType1;
      }), nameof (GetOutOfBoxRuleRecordsForWorkItemType));
    }

    internal static IEnumerable<RuleRecord> GenerateRuleRecordsFromWorkItemFieldRules(
      IVssRequestContext requestContext,
      WorkItemFieldRule fieldRule,
      int projectId,
      int typeNameConstId,
      IDictionary<string, int> constantRecordsByDisplayText)
    {
      List<RuleRecord> workItemFieldRules = new List<RuleRecord>();
      if (fieldRule.SubRules != null)
      {
        CompatibilityRulesGenerator.RuleNode parent = new CompatibilityRulesGenerator.RuleNode((WorkItemRule) fieldRule);
        foreach (WorkItemRule subRule in fieldRule.SubRules)
          workItemFieldRules.AddRange(CompatibilityRulesGenerator.GenerateFieldScopedRuleRecordsForWorkItemRule(requestContext, new CompatibilityRulesGenerator.RuleNode(parent, subRule), projectId, typeNameConstId, fieldRule.FieldId, constantRecordsByDisplayText));
      }
      return (IEnumerable<RuleRecord>) workItemFieldRules;
    }

    internal static IEnumerable<RuleRecord> GetCustomHelpTextRulesForWorkItemType(
      IVssRequestContext requestContext,
      int projectId,
      MetadataWorkItemTypeCompatibilityDescriptor workItemTypeContext,
      IDictionary<string, int> constantMap)
    {
      int typeNameConstId;
      if (constantMap != null && constantMap.TryGetValue(workItemTypeContext.Type.Name, out typeNameConstId))
      {
        foreach (FieldEntry field in (IEnumerable<FieldEntry>) workItemTypeContext.Fields)
        {
          if (!string.IsNullOrWhiteSpace(field.Description))
          {
            int helpTextConstId;
            if (constantMap.TryGetValue(field.Description, out helpTextConstId))
              yield return CompatibilityRulesGenerator.CreateHelpTextRuleRecord(projectId, typeNameConstId, field.FieldId, helpTextConstId);
            else
              MetadataCompatibilityContext.ReportError(requestContext, nameof (GetCustomHelpTextRulesForWorkItemType), string.Format("Description constant '{0}' not found for field '{1}'.", (object) field.Description, (object) field.ReferenceName));
          }
        }
      }
    }

    internal static IEnumerable<RuleRecord> GetCustomRulesForWorkItemType(
      IVssRequestContext requestContext,
      int projectId,
      MetadataWorkItemTypeCompatibilityDescriptor workItemTypeContext,
      IDictionary<string, int> constantMap)
    {
      bool isProcessCustomizationEnabled = WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);
      int typeNameConstId;
      if (constantMap != null && constantMap.TryGetValue(workItemTypeContext.Type.Name, out typeNameConstId))
      {
        foreach (WorkItemFieldRule fieldRule in workItemTypeContext.Rules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => r != null)))
        {
          foreach (WorkItemRule rule in ((IEnumerable<WorkItemRule>) fieldRule.SubRules).Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => r != null)))
          {
            if (isProcessCustomizationEnabled)
            {
              foreach (RuleRecord ruleRecord in CompatibilityRulesGenerator.GenerateFieldScopedRuleRecordsForWorkItemRule(requestContext, new CompatibilityRulesGenerator.RuleNode(rule), projectId, typeNameConstId, fieldRule.FieldId, constantMap))
                yield return ruleRecord;
            }
            else
            {
              switch (rule)
              {
                case DefaultRule _:
                  int num1 = int.MinValue;
                  if ((rule as DefaultRule).ValueFrom == RuleValueFrom.Clock)
                    num1 = -10028;
                  else if ((rule as DefaultRule).ValueFrom == RuleValueFrom.CurrentUser)
                    num1 = -10002;
                  else if ((rule as DefaultRule).ValueFrom == RuleValueFrom.Value && !constantMap.TryGetValue((rule as DefaultRule).Value, out num1))
                    MetadataCompatibilityContext.ReportError(requestContext, nameof (GetCustomRulesForWorkItemType), string.Format("Rule constant '{0}' not found.", (object) (rule as DefaultRule).Value));
                  if (num1 != int.MinValue)
                  {
                    RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldRule.FieldId);
                    scopedRuleRecord.Fld2ID = fieldRule.FieldId;
                    scopedRuleRecord.Fld2IsConstID = -10000;
                    scopedRuleRecord.ThenConstID = num1;
                    scopedRuleRecord.RuleFlags = RuleFlags.Editable | RuleFlags.FlowdownTree | RuleFlags.Default;
                    yield return scopedRuleRecord;
                    continue;
                  }
                  MetadataCompatibilityContext.ReportError(requestContext, nameof (GetCustomRulesForWorkItemType), "ThenConstId for DefaultRule was not found.");
                  continue;
                case RequiredRule _:
                  yield return CompatibilityRulesGenerator.GetRequiredRuleForField(projectId, typeNameConstId, fieldRule.FieldId);
                  continue;
                case AllowedValuesRule _:
                  AllowedValuesRule allowedValuesRule = rule as AllowedValuesRule;
                  if (allowedValuesRule.Sets != null && ((IEnumerable<ConstantSetReference>) allowedValuesRule.Sets).Any<ConstantSetReference>())
                  {
                    int num2 = int.MinValue;
                    if (((IEnumerable<ConstantSetReference>) allowedValuesRule.Sets).First<ConstantSetReference>().Id == -1)
                      num2 = -1;
                    if (((IEnumerable<ConstantSetReference>) allowedValuesRule.Sets).First<ConstantSetReference>().Id == -2)
                      num2 = -2;
                    if (num2 != int.MinValue)
                    {
                      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldRule.FieldId);
                      scopedRuleRecord.ThenConstID = num2;
                      scopedRuleRecord.RuleFlags = RuleFlags.ThenAllNodes | RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree;
                      scopedRuleRecord.RuleFlags2 = RuleFlags2.ThenImplicitEmpty | RuleFlags2.ThenImplicitUnchanged;
                      yield return scopedRuleRecord;
                      continue;
                    }
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            }
          }
        }
      }
    }

    internal static IEnumerable<RuleRecord> GetRulesForPicklists(
      IVssRequestContext requestContext,
      int projectId,
      MetadataWorkItemTypeCompatibilityDescriptor workItemTypeContext,
      IDictionary<string, int> constantMap)
    {
      List<RuleRecord> rulesForPicklists = new List<RuleRecord>();
      IWorkItemPickListService service = requestContext.GetService<IWorkItemPickListService>();
      int typeNameConstId;
      if (constantMap != null && constantMap.TryGetValue(workItemTypeContext.Type.Name, out typeNameConstId))
      {
        foreach (FieldEntry fieldEntry in workItemTypeContext.Fields.Where<FieldEntry>((Func<FieldEntry, bool>) (f => f.IsPicklist)))
        {
          WorkItemPickList list = service.GetList(requestContext, fieldEntry.PickListId.Value);
          Guid typeletId = workItemTypeContext.Type.IsDerived ? workItemTypeContext.Type.InheritedWorkItemType.Id : workItemTypeContext.Type.Source.Id;
          RuleRecord ruleRecord = !list.IsSuggested(requestContext, typeletId, fieldEntry.FieldId) ? CompatibilityRulesGenerator.CreateAllowedValuesRuleRecord(projectId, typeNameConstId, fieldEntry.FieldId, list.ConstId) : CompatibilityRulesGenerator.CreateSuggestedValuesRuleRecord(projectId, typeNameConstId, fieldEntry.FieldId, list.ConstId);
          ruleRecord.RuleFlags2 |= RuleFlags2.ThenImplicitEmpty | RuleFlags2.ThenImplicitUnchanged;
          rulesForPicklists.Add(ruleRecord);
        }
      }
      else
        MetadataCompatibilityContext.ReportError(requestContext, nameof (GetRulesForPicklists), string.Format("Work item type name constant '{0}' not found.", (object) workItemTypeContext.Type.Name));
      return (IEnumerable<RuleRecord>) rulesForPicklists;
    }

    internal static IEnumerable<RuleRecord> GetCustomStateRules(
      IVssRequestContext requestContext,
      int projectId,
      MetadataWorkItemTypeCompatibilityDescriptor workItemTypeContext,
      IDictionary<string, int> constantMap)
    {
      List<RuleRecord> customStateRules = new List<RuleRecord>();
      string key = StatesConstants.AllowedValuesListHead(workItemTypeContext.BaseTypeReferenceName);
      int num1;
      if (constantMap != null && constantMap.TryGetValue(workItemTypeContext.Type.Name, out num1))
      {
        RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, num1, 2);
        int num2;
        if (constantMap.TryGetValue(key, out num2))
        {
          scopedRuleRecord.ThenConstID = num2;
          scopedRuleRecord.RuleFlags = RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree | RuleFlags.ThenLeaf | RuleFlags.ThenOneLevel;
          customStateRules.Add(scopedRuleRecord);
        }
        else
          MetadataCompatibilityContext.ReportError(requestContext, nameof (GetCustomStateRules), string.Format("Could not find states AV list head constant for {0}.", (object) workItemTypeContext.BaseTypeReferenceName));
        WorkItemStateDefinition itemStateDefinition1 = workItemTypeContext.States.OrderBy<WorkItemStateDefinition, int>((Func<WorkItemStateDefinition, int>) (s => s.Order)).First<WorkItemStateDefinition>();
        int stateConstId;
        if (constantMap.TryGetValue(itemStateDefinition1.Name, out stateConstId))
        {
          RuleRecord initialStateRuleRecord1 = CompatibilityRulesGenerator.CreateInitialStateRuleRecord(projectId, num1, stateConstId, -10006);
          initialStateRuleRecord1.RuleFlags = RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree;
          customStateRules.Add(initialStateRuleRecord1);
          RuleRecord initialStateRuleRecord2 = CompatibilityRulesGenerator.CreateInitialStateRuleRecord(projectId, num1, stateConstId, -10000);
          initialStateRuleRecord2.RuleFlags = RuleFlags.Editable | RuleFlags.FlowdownTree | RuleFlags.Default;
          customStateRules.Add(initialStateRuleRecord2);
        }
        else
          MetadataCompatibilityContext.ReportError(requestContext, nameof (GetCustomStateRules), string.Format("Could not find constant for default state {0}.", (object) itemStateDefinition1.Name));
        foreach (WorkItemStateDefinition itemStateDefinition2 in workItemTypeContext.Type.IsCustomType ? (IEnumerable<WorkItemStateDefinition>) workItemTypeContext.States : (IEnumerable<WorkItemStateDefinition>) workItemTypeContext.DeltaStates)
        {
          if (!itemStateDefinition2.Hidden)
          {
            int minValue;
            int reasonConstId1 = minValue = int.MinValue;
            int reasonConstId2 = minValue;
            int num3 = minValue;
            int num4 = minValue;
            if (constantMap.TryGetValue(itemStateDefinition2.Name, out num4))
            {
              foreach (WorkItemStateDefinition state in (IEnumerable<WorkItemStateDefinition>) workItemTypeContext.States)
              {
                if (!TFStringComparer.WorkItemStateName.Equals(state.Name, itemStateDefinition2.Name))
                {
                  if (constantMap.TryGetValue(state.Name, out num3))
                  {
                    if (constantMap.TryGetValue(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReason((object) itemStateDefinition2.Name), out reasonConstId2))
                      customStateRules.Add(CompatibilityRulesGenerator.CreateStateDefaultReasonRuleRecord(projectId, num1, num4, num3, reasonConstId2));
                    else
                      MetadataCompatibilityContext.ReportError(requestContext, nameof (GetCustomStateRules), string.Format("Could not find constant for reason {0}.", (object) Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReason((object) itemStateDefinition2.Name)));
                    if (constantMap.TryGetValue(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReasonOut((object) itemStateDefinition2.Name), out reasonConstId1))
                      customStateRules.Add(CompatibilityRulesGenerator.CreateStateDefaultReasonRuleRecord(projectId, num1, num3, num4, reasonConstId1));
                    else
                      MetadataCompatibilityContext.ReportError(requestContext, nameof (GetCustomStateRules), string.Format("Could not find constant for reason {0}.", (object) Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReasonOut((object) itemStateDefinition2.Name)));
                  }
                  else
                    MetadataCompatibilityContext.ReportError(requestContext, nameof (GetCustomStateRules), string.Format("Could not find constant for state {0}.", (object) state.Name));
                }
              }
              if (TFStringComparer.WorkItemStateName.Equals(itemStateDefinition2.Name, itemStateDefinition1.Name) && reasonConstId2 != int.MinValue)
                customStateRules.Add(CompatibilityRulesGenerator.CreateStateDefaultReasonRuleRecord(projectId, num1, num4, -10000, reasonConstId2));
            }
            else
              MetadataCompatibilityContext.ReportError(requestContext, nameof (GetCustomStateRules), string.Format("Could not find constant for State {0}.", (object) itemStateDefinition2.Name));
          }
        }
      }
      else
        MetadataCompatibilityContext.ReportError(requestContext, nameof (GetCustomStateRules), string.Format("Work item type name constant '{0}' not found.", (object) workItemTypeContext.Type.Name));
      return (IEnumerable<RuleRecord>) customStateRules;
    }

    internal static IEnumerable<RuleRecord> GenerateStateCategoryTransitionRules(
      IVssRequestContext requestContext,
      int projectId,
      MetadataWorkItemTypeCompatibilityDescriptor workItemTypeContext,
      IDictionary<string, int> constantsMap)
    {
      int typeNameConstId;
      if (constantsMap.TryGetValue(workItemTypeContext.Type.Name, out typeNameConstId))
        return requestContext.GetService<IWorkItemStateDefinitionService>().GenerateStateCategoryTransitionRules(requestContext, workItemTypeContext.Type).SelectMany<WorkItemFieldRule, RuleRecord>((Func<WorkItemFieldRule, IEnumerable<RuleRecord>>) (fieldRule => CompatibilityRulesGenerator.GenerateRuleRecordsFromWorkItemFieldRules(requestContext, fieldRule, projectId, typeNameConstId, constantsMap)));
      MetadataCompatibilityContext.ReportError(requestContext, nameof (GenerateStateCategoryTransitionRules), string.Format("Work item type name constant '{0}' not found.", (object) workItemTypeContext.Type.Name));
      return Enumerable.Empty<RuleRecord>();
    }

    internal static IEnumerable<RuleRecord> GenerateStateChangeDateRules(
      IVssRequestContext requestContext,
      int projectId,
      MetadataWorkItemTypeCompatibilityDescriptor workItemTypeContext,
      IDictionary<string, int> constantsMap)
    {
      int typeNameConstId;
      if (!constantsMap.TryGetValue(workItemTypeContext.Type.Name, out typeNameConstId))
      {
        MetadataCompatibilityContext.ReportError(requestContext, nameof (GenerateStateChangeDateRules), string.Format("Work item type name constant '{0}' not found.", (object) workItemTypeContext.Type.Name));
        return Enumerable.Empty<RuleRecord>();
      }
      FieldEntry stateChangeDateFieldEntry;
      if (!requestContext.WitContext().FieldDictionary.TryGetFieldByNameOrId("Microsoft.VSTS.Common.StateChangeDate", out stateChangeDateFieldEntry))
        return Enumerable.Empty<RuleRecord>();
      return !workItemTypeContext.Type.GetFieldIds(requestContext).Any<int>((Func<int, bool>) (id => id == stateChangeDateFieldEntry.FieldId)) ? Enumerable.Empty<RuleRecord>() : requestContext.GetService<IWorkItemStateDefinitionService>().GenerateStateChangeDateFieldRules(requestContext).SelectMany<WorkItemFieldRule, RuleRecord>((Func<WorkItemFieldRule, IEnumerable<RuleRecord>>) (fieldRule => CompatibilityRulesGenerator.GenerateRuleRecordsFromWorkItemFieldRules(requestContext, fieldRule, projectId, typeNameConstId, constantsMap)));
    }

    internal static IEnumerable<RuleRecord> GetRulesForCustomWorkItemType(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      int projectId,
      MetadataWorkItemTypeCompatibilityDescriptor typeDescriptor)
    {
      List<RuleRecord> customWorkItemType = new List<RuleRecord>();
      if (typeDescriptor.Type.IsCustomType)
      {
        WorkItemType type = typeDescriptor.Type;
        int typeNameConstId;
        if (compatContext.ConstantMap != null && compatContext.ConstantMap.TryGetValue(type.Name, out typeNameConstId))
        {
          int propId;
          if (compatContext.TypeIdToFormPropIdMap != null && compatContext.TypeIdToFormPropIdMap.TryGetValue(type.Id.Value, out propId))
            customWorkItemType.Add(CompatibilityRulesGenerator.CreateFormRule(projectId, typeNameConstId, propId));
          else
            MetadataCompatibilityContext.ReportError(requestContext, nameof (GetRulesForCustomWorkItemType), string.Format("PropID not found for work item type '{0}'.", (object) type.Name));
          customWorkItemType.Add(CompatibilityRulesGenerator.GetRequiredRuleForField(projectId, typeNameConstId, 1));
          foreach (FieldEntry combinedField in (IEnumerable<FieldEntry>) type.Source.GetCombinedFields(requestContext))
          {
            if (combinedField.IsIdentity)
              customWorkItemType.Add(CompatibilityRulesGenerator.CreateValidUsersRuleRecord(projectId, typeNameConstId, combinedField.FieldId));
          }
        }
        else
          MetadataCompatibilityContext.ReportError(requestContext, nameof (GetRulesForCustomWorkItemType), string.Format("Work item type name constant '{0}' not found.", (object) type.Name));
      }
      return (IEnumerable<RuleRecord>) customWorkItemType;
    }

    private static IEnumerable<RuleRecord> GetAllowedAndSuggestedValuesRulesForOOBFields(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      int projectId,
      MetadataWorkItemTypeCompatibilityDescriptor typeDescriptor)
    {
      List<RuleRecord> rulesForOobFields = new List<RuleRecord>();
      WorkItemType type = typeDescriptor.Type;
      if (type.IsDerived || type.IsCustomType)
      {
        int typeNameConstId;
        if (compatContext.ConstantMap != null && compatContext.ConstantMap.TryGetValue(type.Name, out typeNameConstId))
        {
          foreach (FieldEntry field in (IEnumerable<FieldEntry>) typeDescriptor.Fields)
          {
            if (!CompatibilityRulesGenerator.IsRuleAvailableForField<AllowedValuesRule>(requestContext, typeDescriptor.Rules, field))
            {
              if (OOBFieldValues.HasAllowedValues(requestContext, field.ReferenceName))
              {
                int setConstId;
                if (compatContext.ConstantMap.TryGetValue(FieldsConstants.AllowedValuesListHead(field.ReferenceName), out setConstId))
                  rulesForOobFields.Add(CompatibilityRulesGenerator.CreateAllowedValuesRuleRecord(projectId, typeNameConstId, field.FieldId, setConstId));
                else
                  MetadataCompatibilityContext.ReportError(requestContext, nameof (GetAllowedAndSuggestedValuesRulesForOOBFields), string.Format("Could not find allowed value list head constant for field {0}.", (object) field.ReferenceName));
              }
              if (!CompatibilityRulesGenerator.IsRuleAvailableForField<SuggestedValuesRule>(requestContext, typeDescriptor.Rules, field) && OOBFieldValues.HasSuggestedValues(requestContext, field.ReferenceName))
              {
                int setConstId;
                if (compatContext.ConstantMap.TryGetValue(FieldsConstants.SuggestedValuesListHead(field.ReferenceName), out setConstId))
                  rulesForOobFields.Add(CompatibilityRulesGenerator.CreateSuggestedValuesRuleRecord(projectId, typeNameConstId, field.FieldId, setConstId));
                else
                  MetadataCompatibilityContext.ReportError(requestContext, nameof (GetAllowedAndSuggestedValuesRulesForOOBFields), string.Format("Could not find suggested value list head constant for field {0}.", (object) field.ReferenceName));
              }
            }
          }
        }
        else
          MetadataCompatibilityContext.ReportError(requestContext, nameof (GetAllowedAndSuggestedValuesRulesForOOBFields), string.Format("Work item type name constant '{0}' not found.", (object) type.Name));
      }
      return (IEnumerable<RuleRecord>) rulesForOobFields;
    }

    private static bool IsRuleAvailableForField<T>(
      IVssRequestContext requestContext,
      IReadOnlyCollection<WorkItemFieldRule> rules,
      FieldEntry field)
    {
      if (rules != null)
      {
        foreach (WorkItemFieldRule rule in (IEnumerable<WorkItemFieldRule>) rules)
        {
          if (rule.FieldId == field.FieldId && (object) rule.SubRules.OfType<T>().FirstOrDefault<T>() != null)
            return true;
        }
      }
      return false;
    }

    private static void FillRulesRecordsWithConstants(
      IEnumerable<RuleRecord> rules,
      IDictionary<int, string> ConstIdToDisplayPartMap)
    {
      foreach (RuleRecord rule in rules)
      {
        if (rule.Fld2IsConstID != 0 && ConstIdToDisplayPartMap.ContainsKey(rule.Fld2IsConstID))
          rule.Fld2Is = ConstIdToDisplayPartMap[rule.Fld2IsConstID];
        if (rule.Fld2WasConstID != 0 && ConstIdToDisplayPartMap.ContainsKey(rule.Fld2WasConstID))
          rule.Fld2Was = ConstIdToDisplayPartMap[rule.Fld2WasConstID];
        if (rule.Fld3IsConstID != 0 && ConstIdToDisplayPartMap.ContainsKey(rule.Fld3IsConstID))
          rule.Fld3Is = ConstIdToDisplayPartMap[rule.Fld3IsConstID];
        if (rule.IfConstID != 0 && ConstIdToDisplayPartMap.ContainsKey(rule.IfConstID))
          rule.If = ConstIdToDisplayPartMap[rule.IfConstID];
        if (rule.ThenConstID != 0 && ConstIdToDisplayPartMap.ContainsKey(rule.ThenConstID))
          rule.Then = ConstIdToDisplayPartMap[rule.ThenConstID];
        if (rule.PersonID != 0 && ConstIdToDisplayPartMap.ContainsKey(rule.PersonID))
          rule.Person = ConstIdToDisplayPartMap[rule.PersonID];
      }
    }

    private static List<RuleRecord> DeleteStateRulesForCustomizedWorkItemTypes(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      List<RuleRecord> rules)
    {
      List<RuleRecord> ruleRecordList = new List<RuleRecord>();
      Dictionary<int, HashSet<int>> customizedStatesMap = CompatibilityRulesGenerator.GetProjectToTypesWithCustomizedStatesMap(requestContext, compatContext);
      if (!customizedStatesMap.Any<KeyValuePair<int, HashSet<int>>>())
        return rules;
      string.Format("Fld{0}IsConstID", (object) 1);
      string.Format("Fld{0}ID", (object) 2);
      foreach (RuleRecord rule in rules)
      {
        HashSet<int> intSet;
        if (customizedStatesMap.TryGetValue(rule.RootTreeID, out intSet))
        {
          int num1 = intSet.Contains(rule.Fld1IsConstID) ? 1 : 0;
          bool flag1 = Convert.ToInt32(rule.Fld2ID) == 0;
          bool flag2 = rule.ThenFldID == 2;
          int num2 = flag1 ? 1 : 0;
          if ((num1 & num2 & (flag2 ? 1 : 0)) != 0)
            continue;
        }
        ruleRecordList.Add(rule);
      }
      return ruleRecordList;
    }

    private static IEnumerable<RuleRecord> GenerateFieldScopedRuleRecordsForWorkItemRule(
      IVssRequestContext requestContext,
      CompatibilityRulesGenerator.RuleNode ruleNode,
      int projectId,
      int typeNameConstId,
      int fieldId,
      IDictionary<string, int> constantsMap)
    {
      List<RuleRecord> recordsForWorkItemRule = new List<RuleRecord>();
      WorkItemRule rule = ruleNode.Rule;
      if (rule.IsDisabled)
        return (IEnumerable<RuleRecord>) recordsForWorkItemRule;
      if (rule is HideFieldRule)
        return (IEnumerable<RuleRecord>) recordsForWorkItemRule;
      if (rule.GetType() == typeof (RuleBlock) || rule.GetType() == typeof (WorkItemTypeScopedRules))
      {
        RuleBlock ruleBlock = (RuleBlock) rule;
        if (ruleBlock.SubRules != null)
        {
          foreach (WorkItemRule subRule in ruleBlock.SubRules)
            recordsForWorkItemRule.AddRange(CompatibilityRulesGenerator.GenerateFieldScopedRuleRecordsForWorkItemRule(requestContext, new CompatibilityRulesGenerator.RuleNode(ruleNode, subRule), projectId, typeNameConstId, fieldId, constantsMap));
        }
      }
      else if (rule.Name == WorkItemRuleName.Required)
      {
        if (!CompatibilityRulesGenerator.IsForNotRule(rule))
        {
          RuleRecord requiredRuleForField = CompatibilityRulesGenerator.GetRequiredRuleForField(projectId, typeNameConstId, fieldId);
          CompatibilityRulesGenerator.AddParentContextToRuleRecord(requestContext, ruleNode, requiredRuleForField, constantsMap);
          recordsForWorkItemRule.Add(requiredRuleForField);
        }
      }
      else if (rule.Name == WorkItemRuleName.ValidUser)
      {
        RuleRecord validUsersRuleRecord = CompatibilityRulesGenerator.CreateValidUsersRuleRecord(projectId, typeNameConstId, fieldId);
        CompatibilityRulesGenerator.AddParentContextToRuleRecord(requestContext, ruleNode, validUsersRuleRecord, constantsMap);
        recordsForWorkItemRule.Add(validUsersRuleRecord);
      }
      else if (rule.Name == WorkItemRuleName.AllowedValues)
      {
        AllowedValuesRule allowedValuesRule = (AllowedValuesRule) rule;
        List<RuleRecord> ruleRecordList = new List<RuleRecord>();
        if (allowedValuesRule.Values != null && allowedValuesRule.Values.Any<string>())
        {
          int setConstId;
          if (constantsMap.TryGetValue(allowedValuesRule.Id.ToString(), out setConstId))
            ruleRecordList.Add(CompatibilityRulesGenerator.CreateAllowedValuesRuleRecord(projectId, typeNameConstId, fieldId, setConstId));
          else
            MetadataCompatibilityContext.ReportError(requestContext, nameof (GenerateFieldScopedRuleRecordsForWorkItemRule), string.Format("Constant not found for allowed value rule '{0}'.", (object) allowedValuesRule.Id));
        }
        else if (allowedValuesRule.Sets != null)
        {
          foreach (ConstantSetReference set in allowedValuesRule.Sets)
            ruleRecordList.Add(CompatibilityRulesGenerator.CreateAllowedValuesRuleRecord(projectId, typeNameConstId, fieldId, set.Id));
        }
        foreach (RuleRecord ruleRecord in ruleRecordList)
        {
          CompatibilityRulesGenerator.AddParentContextToRuleRecord(requestContext, ruleNode, ruleRecord, constantsMap);
          recordsForWorkItemRule.Add(ruleRecord);
        }
      }
      else if (rule.Name == WorkItemRuleName.SuggestedValues)
      {
        SuggestedValuesRule suggestedValuesRule = (SuggestedValuesRule) rule;
        List<RuleRecord> ruleRecordList = new List<RuleRecord>();
        if (suggestedValuesRule.Values != null && suggestedValuesRule.Values.Any<string>())
        {
          int setConstId;
          if (constantsMap.TryGetValue(suggestedValuesRule.Id.ToString(), out setConstId))
            ruleRecordList.Add(CompatibilityRulesGenerator.CreateSuggestedValuesRuleRecord(projectId, typeNameConstId, fieldId, setConstId));
          else
            MetadataCompatibilityContext.ReportError(requestContext, nameof (GenerateFieldScopedRuleRecordsForWorkItemRule), string.Format("Constant not found for suggested value rule '{0}'.", (object) suggestedValuesRule.Id));
        }
        else if (suggestedValuesRule.Sets != null)
        {
          foreach (ConstantSetReference set in suggestedValuesRule.Sets)
            ruleRecordList.Add(CompatibilityRulesGenerator.CreateSuggestedValuesRuleRecord(projectId, typeNameConstId, fieldId, set.Id));
        }
        foreach (RuleRecord ruleRecord in ruleRecordList)
        {
          CompatibilityRulesGenerator.AddParentContextToRuleRecord(requestContext, ruleNode, ruleRecord, constantsMap);
          recordsForWorkItemRule.Add(ruleRecord);
        }
      }
      else if (rule.Name == WorkItemRuleName.ProhibitedValues)
      {
        ProhibitedValuesRule prohibitedValuesRule = (ProhibitedValuesRule) rule;
        List<RuleRecord> ruleRecordList = new List<RuleRecord>();
        if (prohibitedValuesRule.Values != null && prohibitedValuesRule.Values.Any<string>())
        {
          int setConstId;
          if (constantsMap.TryGetValue(prohibitedValuesRule.Id.ToString(), out setConstId))
            ruleRecordList.Add(CompatibilityRulesGenerator.CreateProhibitedValuesRuleRecord(projectId, typeNameConstId, fieldId, setConstId));
          else
            MetadataCompatibilityContext.ReportError(requestContext, nameof (GenerateFieldScopedRuleRecordsForWorkItemRule), string.Format("Constant not found for prohibited value rule '{0}'.", (object) prohibitedValuesRule.Id));
        }
        else if (prohibitedValuesRule.Sets != null)
        {
          foreach (ConstantSetReference set in prohibitedValuesRule.Sets)
            ruleRecordList.Add(CompatibilityRulesGenerator.CreateProhibitedValuesRuleRecord(projectId, typeNameConstId, fieldId, set.Id));
        }
        foreach (RuleRecord ruleRecord in ruleRecordList)
        {
          CompatibilityRulesGenerator.AddParentContextToRuleRecord(requestContext, ruleNode, ruleRecord, constantsMap);
          recordsForWorkItemRule.Add(ruleRecord);
        }
      }
      else if (rule.Name == WorkItemRuleName.When)
      {
        WhenRule whenRule = (WhenRule) rule;
        if (whenRule.SubRules != null)
        {
          foreach (WorkItemRule subRule in whenRule.SubRules)
            recordsForWorkItemRule.AddRange(CompatibilityRulesGenerator.GenerateFieldScopedRuleRecordsForWorkItemRule(requestContext, new CompatibilityRulesGenerator.RuleNode(ruleNode, subRule), projectId, typeNameConstId, fieldId, constantsMap));
        }
      }
      else if (rule.Name == WorkItemRuleName.WhenWas)
      {
        WhenWasRule whenWasRule = (WhenWasRule) rule;
        if (whenWasRule.SubRules != null)
        {
          foreach (WorkItemRule subRule in whenWasRule.SubRules)
            recordsForWorkItemRule.AddRange(CompatibilityRulesGenerator.GenerateFieldScopedRuleRecordsForWorkItemRule(requestContext, new CompatibilityRulesGenerator.RuleNode(ruleNode, subRule), projectId, typeNameConstId, fieldId, constantsMap));
        }
      }
      else if (rule.Name == WorkItemRuleName.WhenChanged)
      {
        WhenChangedRule whenChangedRule = (WhenChangedRule) rule;
        if (whenChangedRule.SubRules != null)
        {
          foreach (WorkItemRule subRule in whenChangedRule.SubRules)
            recordsForWorkItemRule.AddRange(CompatibilityRulesGenerator.GenerateFieldScopedRuleRecordsForWorkItemRule(requestContext, new CompatibilityRulesGenerator.RuleNode(ruleNode, subRule), projectId, typeNameConstId, fieldId, constantsMap));
        }
      }
      else if (rule.Name == WorkItemRuleName.Default || rule.Name == WorkItemRuleName.Copy)
      {
        ActionRule actionRule = (ActionRule) rule;
        int constId = int.MinValue;
        switch (actionRule.ValueFrom)
        {
          case RuleValueFrom.Value:
            if (actionRule.Value == string.Empty)
            {
              constId = -10000;
              break;
            }
            if (!constantsMap.TryGetValue(actionRule.Value, out constId))
            {
              MetadataCompatibilityContext.ReportError(requestContext, nameof (GenerateFieldScopedRuleRecordsForWorkItemRule), string.Format("Constant not found for copy/default rule value '{0}'.", (object) actionRule.Value));
              break;
            }
            break;
          case RuleValueFrom.OtherFieldCurrentValue:
          case RuleValueFrom.OtherFieldOriginalValue:
            constId = -10012;
            break;
          case RuleValueFrom.CurrentUser:
            constId = -10002;
            break;
          case RuleValueFrom.Clock:
            constId = -10028;
            break;
          default:
            constId = -10000;
            break;
        }
        if (constId != int.MinValue)
        {
          RuleRecord defaultRuleRecord = CompatibilityRulesGenerator.CreateBaseCopyDefaultRuleRecord(projectId, typeNameConstId, fieldId, constId);
          if (actionRule.ValueFrom == RuleValueFrom.OtherFieldCurrentValue || actionRule.ValueFrom == RuleValueFrom.OtherFieldOriginalValue)
          {
            int result;
            if (int.TryParse(actionRule.Value, out result))
              defaultRuleRecord.If2FldID = result;
            else
              MetadataCompatibilityContext.ReportError(requestContext, nameof (GenerateFieldScopedRuleRecordsForWorkItemRule), string.Format("If2FldId not found for copy rule value '{0}'.", (object) actionRule.Value));
            defaultRuleRecord.If2ConstID = -10012;
          }
          if (actionRule.Name == WorkItemRuleName.Default)
          {
            defaultRuleRecord.Fld2ID = fieldId;
            defaultRuleRecord.Fld2IsConstID = -10000;
          }
          CompatibilityRulesGenerator.AddParentContextToRuleRecord(requestContext, ruleNode, defaultRuleRecord, constantsMap);
          recordsForWorkItemRule.Add(defaultRuleRecord);
        }
      }
      else if (rule.Name == WorkItemRuleName.ServerDefault)
      {
        int constId;
        switch (((ServerDefaultRule) rule).From)
        {
          case ServerDefaultType.ServerDateTime:
            constId = -10013;
            break;
          case ServerDefaultType.CallerIdentity:
            constId = -10026;
            break;
          case ServerDefaultType.RandomGuid:
            constId = -10031;
            break;
          default:
            constId = -10000;
            break;
        }
        foreach (RuleRecord serverDefaultRule in CompatibilityRulesGenerator.CreateServerDefaultRules(projectId, typeNameConstId, fieldId, constId))
        {
          CompatibilityRulesGenerator.AddParentContextToRuleRecord(requestContext, ruleNode, serverDefaultRule, constantsMap);
          recordsForWorkItemRule.Add(serverDefaultRule);
        }
      }
      else if (rule.Name == WorkItemRuleName.ReadOnly)
      {
        if (!CompatibilityRulesGenerator.IsForNotRule(rule))
        {
          RuleRecord readOnlyRuleRecord = CompatibilityRulesGenerator.CreateReadOnlyRuleRecord(projectId, typeNameConstId, fieldId);
          CompatibilityRulesGenerator.AddParentContextToRuleRecord(requestContext, ruleNode, readOnlyRuleRecord, constantsMap);
          recordsForWorkItemRule.Add(readOnlyRuleRecord);
        }
      }
      else if (rule.Name == WorkItemRuleName.Empty)
      {
        RuleRecord emptyRuleRecord = CompatibilityRulesGenerator.CreateEmptyRuleRecord(projectId, typeNameConstId, fieldId);
        CompatibilityRulesGenerator.AddParentContextToRuleRecord(requestContext, ruleNode, emptyRuleRecord, constantsMap);
        recordsForWorkItemRule.Add(emptyRuleRecord);
      }
      return (IEnumerable<RuleRecord>) recordsForWorkItemRule;
    }

    private static bool IsForNotRule(WorkItemRule workItemRule) => workItemRule.ForVsId != Guid.Empty || workItemRule.NotVsId != Guid.Empty;

    private static void AddParentContextToRuleRecord(
      IVssRequestContext requestContext,
      CompatibilityRulesGenerator.RuleNode ruleNode,
      RuleRecord ruleRecord,
      IDictionary<string, int> constantsMap)
    {
      CompatibilityRulesGenerator.RuleNode parent = ruleNode.Parent;
      while (parent != null && parent.Rule.GetType() == typeof (RuleBlock))
        parent = parent.Parent;
      if (parent == null)
      {
        if (ruleRecord.IfFldID != 0 || ruleRecord.Fld2ID != 2 || ruleRecord.Fld2WasConstID != 0 || ruleRecord.Fld3ID != 0)
          return;
        ruleRecord.IfFldID = ruleRecord.Fld2ID;
        ruleRecord.IfConstID = ruleRecord.Fld2IsConstID;
        ruleRecord.Fld2ID = 0;
        ruleRecord.Fld2IsConstID = -10000;
      }
      else
      {
        WorkItemRule rule = parent.Rule;
        if (rule.Name == WorkItemRuleName.When)
        {
          WhenRule whenRule = (WhenRule) rule;
          int constId;
          if (whenRule.Value == string.Empty)
            constId = -10000;
          else if (!constantsMap.TryGetValue(whenRule.Value, out constId))
            MetadataCompatibilityContext.ReportError(requestContext, nameof (AddParentContextToRuleRecord), string.Format("Constant not found for when rule value '{0}'.", (object) whenRule.Value));
          CompatibilityRulesGenerator.MergeWhenRuleInformation(whenRule.FieldId, ruleRecord, constId, whenRule.Inverse);
        }
        else if (rule.Name == WorkItemRuleName.WhenWas)
        {
          WhenWasRule whenWasRule = (WhenWasRule) rule;
          int constId;
          if (whenWasRule.Value == string.Empty)
            constId = -10000;
          else if (!constantsMap.TryGetValue(whenWasRule.Value, out constId))
            MetadataCompatibilityContext.ReportError(requestContext, nameof (AddParentContextToRuleRecord), string.Format("Constant not found for when was rule value '{0}'.", (object) whenWasRule.Value));
          CompatibilityRulesGenerator.MergeWhenWasRuleInformation(whenWasRule.FieldId, ruleRecord, constId, whenWasRule.Inverse);
        }
        else if (rule.Name == WorkItemRuleName.WhenChanged)
        {
          WhenChangedRule whenChangedRule = (WhenChangedRule) rule;
          CompatibilityRulesGenerator.MergeWhenChangedRuleInformation(whenChangedRule.FieldId, ruleRecord, whenChangedRule.Inverse);
        }
        CompatibilityRulesGenerator.AddParentContextToRuleRecord(requestContext, parent, ruleRecord, constantsMap);
      }
    }

    private static void MergeWhenRuleInformation(
      int fldId,
      RuleRecord ruleRecord,
      int constId,
      bool inverse)
    {
      switch (fldId)
      {
        case 2:
          ruleRecord.Fld2ID = fldId;
          ruleRecord.Fld2IsConstID = constId;
          break;
        case 22:
        case 24:
          ruleRecord.Fld3ID = fldId;
          ruleRecord.Fld3IsConstID = constId;
          break;
        default:
          ruleRecord.IfFldID = fldId;
          ruleRecord.IfConstID = constId;
          break;
      }
      if (!inverse)
        return;
      ruleRecord.RuleFlags |= RuleFlags.IfNot;
    }

    private static void MergeWhenWasRuleInformation(
      int fieldId,
      RuleRecord ruleRecord,
      int constId,
      bool inverse)
    {
      switch (fieldId)
      {
        case 2:
          ruleRecord.Fld2ID = fieldId;
          ruleRecord.Fld2WasConstID = constId;
          break;
        case 22:
          ruleRecord.Fld3ID = fieldId;
          ruleRecord.Fld3WasConstID = constId;
          break;
      }
      if (!inverse)
        return;
      ruleRecord.RuleFlags |= RuleFlags.IfNot;
    }

    private static void MergeWhenChangedRuleInformation(
      int fieldId,
      RuleRecord ruleRecord,
      bool inverse)
    {
      ruleRecord.IfFldID = fieldId;
      ruleRecord.IfConstID = -10001;
      if (inverse)
        return;
      ruleRecord.RuleFlags |= RuleFlags.IfNot;
    }

    private static RuleRecord CreateTypeScopedRuleRecord(
      int projectId,
      int workItemTypeConstId,
      int fieldId)
    {
      return new RuleRecord()
      {
        PersonID = -1,
        ObjectTypeScopeID = -100,
        RootTreeID = projectId,
        AreaID = projectId,
        Fld1ID = 25,
        Fld1IsConstID = workItemTypeConstId,
        ThenFldID = fieldId
      };
    }

    private static RuleRecord CreateProjectScopedRuleRecord(int projectId, int fieldId) => new RuleRecord()
    {
      PersonID = -1,
      ObjectTypeScopeID = -100,
      RootTreeID = projectId,
      AreaID = projectId,
      ThenFldID = fieldId
    };

    private static RuleRecord GetRequiredRuleForField(
      int projectId,
      int typeNameConstId,
      int fieldId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldId);
      scopedRuleRecord.ThenConstID = -10000;
      scopedRuleRecord.RuleFlags = RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree | RuleFlags.ThenNot;
      return scopedRuleRecord;
    }

    private static RuleRecord CreateBaseCopyDefaultRuleRecord(
      int projectId,
      int typeNameConstId,
      int fieldId,
      int constId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldId);
      scopedRuleRecord.ThenConstID = constId;
      scopedRuleRecord.RuleFlags = RuleFlags.Editable | RuleFlags.FlowdownTree | RuleFlags.Default;
      scopedRuleRecord.RuleFlags2 = RuleFlags2.ThenImplicitUnchanged;
      return scopedRuleRecord;
    }

    private static IEnumerable<RuleRecord> CreateServerDefaultRules(
      int projectId,
      int typeNameConstId,
      int fieldId,
      int constId)
    {
      RuleRecord scopedRuleRecord1 = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldId);
      scopedRuleRecord1.ThenConstID = constId;
      scopedRuleRecord1.RuleFlags = RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree;
      RuleRecord scopedRuleRecord2 = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldId);
      scopedRuleRecord2.ThenConstID = constId;
      scopedRuleRecord2.RuleFlags = RuleFlags.Editable | RuleFlags.FlowdownTree | RuleFlags.Default;
      return (IEnumerable<RuleRecord>) new RuleRecord[2]
      {
        scopedRuleRecord1,
        scopedRuleRecord2
      };
    }

    private static RuleRecord CreateReadOnlyRuleRecord(
      int projectId,
      int typeNameConstId,
      int fieldId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldId);
      scopedRuleRecord.ThenConstID = -10001;
      scopedRuleRecord.RuleFlags = RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree;
      scopedRuleRecord.RuleFlags2 = RuleFlags2.ThenImplicitUnchanged;
      return scopedRuleRecord;
    }

    private static RuleRecord CreateEmptyRuleRecord(
      int projectId,
      int typeNameConstId,
      int fieldId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldId);
      scopedRuleRecord.ThenConstID = -10000;
      scopedRuleRecord.RuleFlags = RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree;
      scopedRuleRecord.RuleFlags2 = RuleFlags2.ThenImplicitUnchanged;
      return scopedRuleRecord;
    }

    private static RuleRecord CreateStateDefaultReasonRuleRecord(
      int projectId,
      int workItemTypeNameConstId,
      int isStateConstId,
      int wasStateConstId,
      int reasonConstId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, workItemTypeNameConstId, 22);
      scopedRuleRecord.Fld2ID = 2;
      scopedRuleRecord.Fld2IsConstID = isStateConstId;
      scopedRuleRecord.Fld2WasConstID = wasStateConstId;
      scopedRuleRecord.ThenConstID = reasonConstId;
      scopedRuleRecord.RuleFlags = RuleFlags.Editable | RuleFlags.FlowdownTree | RuleFlags.Default;
      return scopedRuleRecord;
    }

    private static RuleRecord CreateInitialStateRuleRecord(
      int projectId,
      int typeNameConstId,
      int stateConstId,
      int ifConstId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, 2);
      scopedRuleRecord.IfFldID = 2;
      scopedRuleRecord.ThenConstID = stateConstId;
      scopedRuleRecord.IfConstID = ifConstId;
      return scopedRuleRecord;
    }

    private static RuleRecord CreateValidUsersRuleRecord(
      int projectId,
      int typeNameConstId,
      int fieldId)
    {
      return CompatibilityRulesGenerator.CreateAllowedValuesRuleRecord(projectId, typeNameConstId, fieldId, -2);
    }

    private static RuleRecord CreateAllowedValuesRuleRecord(
      int projectId,
      int typeNameConstId,
      int fieldId,
      int setConstId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldId);
      scopedRuleRecord.ThenConstID = setConstId;
      scopedRuleRecord.RuleFlags = RuleFlags.ThenAllNodes | RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree;
      scopedRuleRecord.RuleFlags2 = RuleFlags2.ThenImplicitEmpty | RuleFlags2.ThenImplicitUnchanged;
      return scopedRuleRecord;
    }

    private static RuleRecord CreateProjectScopedAllowedValuesRuleRecord(
      int projectId,
      int fieldId,
      int setConstId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateProjectScopedRuleRecord(projectId, fieldId);
      scopedRuleRecord.ThenConstID = setConstId;
      scopedRuleRecord.RuleFlags = RuleFlags.ThenAllNodes | RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree;
      return scopedRuleRecord;
    }

    private static RuleRecord CreateSuggestedValuesRuleRecord(
      int projectId,
      int typeNameConstId,
      int fieldId,
      int setConstId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldId);
      scopedRuleRecord.ThenConstID = setConstId;
      scopedRuleRecord.RuleFlags = RuleFlags.ThenAllNodes | RuleFlags.Editable | RuleFlags.FlowdownTree | RuleFlags.Suggestion;
      scopedRuleRecord.RuleFlags2 = RuleFlags2.ThenImplicitEmpty | RuleFlags2.ThenImplicitUnchanged;
      return scopedRuleRecord;
    }

    private static RuleRecord CreateProhibitedValuesRuleRecord(
      int projectId,
      int typeNameConstId,
      int fieldId,
      int setConstId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldId);
      scopedRuleRecord.ThenConstID = setConstId;
      scopedRuleRecord.RuleFlags = RuleFlags.ThenAllNodes | RuleFlags.Editable | RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.FlowdownTree | RuleFlags.ThenNot;
      return scopedRuleRecord;
    }

    private static RuleRecord CreateHelpTextRuleRecord(
      int projectId,
      int typeNameConstId,
      int fieldId,
      int helpTextConstId)
    {
      RuleRecord scopedRuleRecord = CompatibilityRulesGenerator.CreateTypeScopedRuleRecord(projectId, typeNameConstId, fieldId);
      scopedRuleRecord.ThenConstID = helpTextConstId;
      scopedRuleRecord.RuleFlags = RuleFlags.Editable | RuleFlags.FlowdownTree | RuleFlags.Helptext;
      return scopedRuleRecord;
    }

    private static RuleRecord CreateFormRule(int projectId, int typeNameConstId, int propId) => new RuleRecord()
    {
      RootTreeID = projectId,
      AreaID = projectId,
      PersonID = -1,
      ObjectTypeScopeID = -100,
      IfFldID = 25,
      IfConstID = typeNameConstId,
      ThenFldID = -14,
      ThenConstID = propId,
      RuleFlags = RuleFlags.Editable | RuleFlags.FlowdownTree | RuleFlags.Default,
      RuleFlags2 = RuleFlags2.ThenConstLargetext
    };

    private class RuleNode
    {
      public CompatibilityRulesGenerator.RuleNode Parent;
      public WorkItemRule Rule;

      public RuleNode(WorkItemRule rule)
      {
        this.Parent = (CompatibilityRulesGenerator.RuleNode) null;
        this.Rule = rule;
      }

      public RuleNode(CompatibilityRulesGenerator.RuleNode parent, WorkItemRule rule)
      {
        this.Parent = parent;
        this.Rule = rule;
      }
    }
  }
}
