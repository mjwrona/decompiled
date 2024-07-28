// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility.MetadataCompatibilityContextGenerator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility
{
  public class MetadataCompatibilityContextGenerator
  {
    public virtual MetadataCompatibilityContext GetCompatibilityContext(
      IVssRequestContext requestContext,
      Guid? projectId = null,
      string workItemType = null,
      IEnumerable<Guid> projectsToLoad = null)
    {
      return requestContext.TraceBlock<MetadataCompatibilityContext>(900814, 900815, "Compatibility", "MetadataCompatibility", nameof (GetCompatibilityContext), (Func<MetadataCompatibilityContext>) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        IProjectService service1 = requestContext.GetService<IProjectService>();
        IWorkItemTypeService service2 = requestContext.GetService<IWorkItemTypeService>();
        List<MetadataProjectCompatibilityDescriptor> projectDescriptors = new List<MetadataProjectCompatibilityDescriptor>();
        WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IEnumerable<ProjectInfo> source1;
        if (projectId.HasValue)
        {
          source1 = (IEnumerable<ProjectInfo>) new List<ProjectInfo>()
          {
            service1.GetProject(requestContext, projectId.Value)
          };
        }
        else
        {
          source1 = service1.GetProjects(vssRequestContext, ProjectState.WellFormed);
          if (projectsToLoad != null && projectsToLoad.Any<Guid>())
          {
            HashSet<Guid> projectIdSet = new HashSet<Guid>(projectsToLoad);
            source1 = source1.Where<ProjectInfo>((Func<ProjectInfo, bool>) (project => projectIdSet.Contains(project.Id)));
          }
        }
        IEnumerable<ProjectInfo> source2 = source1.Where<ProjectInfo>((Func<ProjectInfo, bool>) (p => p.Visibility != ProjectVisibility.SystemPrivate));
        Dictionary<Guid, ProcessDescriptor> dictionary1 = vssRequestContext.GetService<IWorkItemTrackingProcessService>().GetProjectProcessDescriptorMappings(vssRequestContext, source2.Select<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (p => p.Id)), true).ToDictionary<ProjectProcessDescriptorMapping, Guid, ProcessDescriptor>((Func<ProjectProcessDescriptorMapping, Guid>) (p => p.Project.Id), (Func<ProjectProcessDescriptorMapping, ProcessDescriptor>) (p => p.Descriptor));
        Dictionary<Guid, TreeNode> dictionary2 = new Dictionary<Guid, TreeNode>();
        foreach (ProjectInfo projectInfo in source2)
        {
          TreeNode node;
          if (trackingRequestContext.TreeService.TryGetTreeNode(projectInfo.Id, projectInfo.Id, out node))
            dictionary2[projectInfo.Id] = node;
          else
            requestContext.Trace(900810, TraceLevel.Warning, "Compatibility", "MetadataCompatibility", string.Format("Missing project '{0}' in work item tracking. Skipping generating project compatibility for this project.", (object) projectInfo.Id));
        }
        IEnumerable<WorkItemType> source3 = (IEnumerable<WorkItemType>) new List<WorkItemType>();
        if (workItemType == null)
        {
          source3 = (IEnumerable<WorkItemType>) service2.GetWorkItemTypes(vssRequestContext, (IEnumerable<Guid>) dictionary2.Keys).Select<WorkItemType, WorkItemType>((Func<WorkItemType, WorkItemType>) (t => t.Clone())).ToList<WorkItemType>();
        }
        else
        {
          WorkItemType workItemType1;
          if (service2.TryGetWorkItemTypeByName(requestContext, projectId.Value, workItemType, out workItemType1))
            source3 = (IEnumerable<WorkItemType>) new List<WorkItemType>()
            {
              workItemType1.Clone()
            };
        }
        ILookup<Guid, WorkItemType> lookup = source3.ToLookup<WorkItemType, Guid>((Func<WorkItemType, Guid>) (type => type.ProjectId));
        source3.Select<WorkItemType, int?>((Func<WorkItemType, int?>) (t => t.Id)).Where<int?>((Func<int?, bool>) (i => i.HasValue)).Select<int?, int>((Func<int?, int>) (i => i.Value)).DefaultIfEmpty<int>(0).Max();
        Dictionary<string, MetadataCompatibilityContextGenerator.StateDefinitionCacheEntry> dictionary3 = new Dictionary<string, MetadataCompatibilityContextGenerator.StateDefinitionCacheEntry>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName);
        Dictionary<Guid, IEnumerable<WorkItemTypeCategory>> dictionary4 = new Dictionary<Guid, IEnumerable<WorkItemTypeCategory>>();
        IWorkItemTypeCategoryService service3 = requestContext.GetService<IWorkItemTypeCategoryService>();
        foreach (KeyValuePair<Guid, TreeNode> keyValuePair in dictionary2)
        {
          IEnumerable<WorkItemType> source4 = lookup[keyValuePair.Key];
          List<MetadataWorkItemTypeCompatibilityDescriptor> compatibilityDescriptorList = new List<MetadataWorkItemTypeCompatibilityDescriptor>();
          if (!source4.Any<WorkItemType>())
            requestContext.Trace(900816, TraceLevel.Warning, "Compatibility", "MetadataCompatibility", string.Format("Project '{0}' has no types.", (object) keyValuePair.Key));
          foreach (WorkItemType workItemType2 in source4)
          {
            WorkItemType type = workItemType2;
            MetadataCompatibilityContextGenerator.StateDefinitionCacheEntry definitionCacheEntry = (MetadataCompatibilityContextGenerator.StateDefinitionCacheEntry) null;
            if (type.IsCustomType)
              definitionCacheEntry = dictionary3.GetOrAddValue<string, MetadataCompatibilityContextGenerator.StateDefinitionCacheEntry>(type.ReferenceName, (Func<MetadataCompatibilityContextGenerator.StateDefinitionCacheEntry>) (() => new MetadataCompatibilityContextGenerator.StateDefinitionCacheEntry(type.Source.GetStates(requestContext), (IReadOnlyCollection<WorkItemStateDefinition>) new List<WorkItemStateDefinition>())));
            else if (type.IsDerived)
              definitionCacheEntry = dictionary3.GetOrAddValue<string, MetadataCompatibilityContextGenerator.StateDefinitionCacheEntry>(type.InheritedWorkItemType.ReferenceName, (Func<MetadataCompatibilityContextGenerator.StateDefinitionCacheEntry>) (() => new MetadataCompatibilityContextGenerator.StateDefinitionCacheEntry(type.InheritedWorkItemType.GetStates(requestContext), type.InheritedWorkItemType.GetDeltaStates(requestContext))));
            compatibilityDescriptorList.Add(MetadataWorkItemTypeCompatibilityDescriptor.Create(vssRequestContext, type, definitionCacheEntry?.States, definitionCacheEntry?.DeltaStates));
          }
          if (dictionary1 != null)
          {
            ProcessDescriptor processDescriptor;
            dictionary1.TryGetValue(keyValuePair.Key, out processDescriptor);
            IEnumerable<WorkItemTypeCategory> categories = (IEnumerable<WorkItemTypeCategory>) null;
            if (dictionary4 != null && processDescriptor != null && !processDescriptor.IsCustom && !dictionary4.TryGetValue(processDescriptor.TypeId, out categories))
            {
              categories = (IEnumerable<WorkItemTypeCategory>) service3.GetWorkItemTypeCategories(requestContext.Elevate(), keyValuePair.Value.ProjectId).ToList<WorkItemTypeCategory>().Select<WorkItemTypeCategory, WorkItemTypeCategory>((Func<WorkItemTypeCategory, WorkItemTypeCategory>) (c => new WorkItemTypeCategory(c.Name, c.ReferenceName, c.WorkItemTypeNames, c.DefaultWorkItemTypeName))).ToList<WorkItemTypeCategory>();
              dictionary4[processDescriptor.TypeId] = categories;
            }
            projectDescriptors.Add(new MetadataProjectCompatibilityDescriptor(processDescriptor, categories)
            {
              ProjectNode = keyValuePair.Value,
              TypeDescriptors = (IReadOnlyCollection<MetadataWorkItemTypeCompatibilityDescriptor>) compatibilityDescriptorList
            });
          }
          else
            projectDescriptors.Add(new MetadataProjectCompatibilityDescriptor()
            {
              ProjectNode = keyValuePair.Value,
              TypeDescriptors = (IReadOnlyCollection<MetadataWorkItemTypeCompatibilityDescriptor>) compatibilityDescriptorList
            });
        }
        return new MetadataCompatibilityContext()
        {
          ProjectDescriptors = (IReadOnlyCollection<MetadataProjectCompatibilityDescriptor>) projectDescriptors,
          ConstantMap = MetadataCompatibilityContextGenerator.GetConstantMap(requestContext, (IEnumerable<MetadataProjectCompatibilityDescriptor>) projectDescriptors)
        };
      }));
    }

    public static IDictionary<string, int> GetConstantMap(
      IVssRequestContext requestContext,
      IEnumerable<MetadataProjectCompatibilityDescriptor> projectDescriptors)
    {
      return (IDictionary<string, int>) requestContext.TraceBlock<Dictionary<string, int>>(900823, 900824, "Compatibility", "MetadataCompatibility", nameof (GetConstantMap), (Func<Dictionary<string, int>>) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IEnumerable<MetadataProjectCompatibilityDescriptor>>(projectDescriptors, nameof (projectDescriptors));
        HashSet<string> constants = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int> constantMap = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        constants.Add("299f07ef-6201-41b3-90fc-03eeb3977587");
        HashSet<Guid> guidSet = new HashSet<Guid>();
        foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in projectDescriptors)
        {
          constants.Add(CompatibilityConstants.WorkItemTypeAllowedValueListHead(projectDescriptor.ProjectNode.Id));
          ProcessDescriptor processDescriptor;
          if (projectDescriptor.TryGetProcessDescriptor(requestContext, out processDescriptor))
          {
            if (!guidSet.Contains(processDescriptor.TypeId))
              guidSet.Add(processDescriptor.TypeId);
            else
              continue;
          }
          foreach (MetadataWorkItemTypeCompatibilityDescriptor typeDescriptor in (IEnumerable<MetadataWorkItemTypeCompatibilityDescriptor>) projectDescriptor.TypeDescriptors)
          {
            constants.Add(typeDescriptor.Type.Name);
            constants.UnionWith(MetadataCompatibilityContextGenerator.GetRuleConstants(typeDescriptor.Rules));
            MetadataCompatibilityContextGenerator.AddIdentityConstants(constantMap, (IEnumerable<WorkItemFieldRule>) typeDescriptor.Rules);
            foreach (FieldEntry field in (IEnumerable<FieldEntry>) typeDescriptor.Fields)
            {
              if (!string.IsNullOrEmpty(field.Description))
                constants.Add(field.Description);
              IReadOnlyCollection<string> values;
              if (OOBFieldValues.TryGetAllowedValues(requestContext, field.FieldId, out values))
              {
                constants.UnionWith((IEnumerable<string>) values);
                constants.Add(FieldsConstants.AllowedValuesListHead(field.ReferenceName));
              }
              else if (OOBFieldValues.TryGetSuggestedValues(requestContext, field.FieldId, out values))
              {
                constants.UnionWith((IEnumerable<string>) values);
                constants.Add(FieldsConstants.SuggestedValuesListHead(field.ReferenceName));
              }
            }
            if (typeDescriptor.AreStatesCustomized)
            {
              foreach (WorkItemStateDefinition state in (IEnumerable<WorkItemStateDefinition>) typeDescriptor.States)
              {
                constants.Add(state.Name);
                if (typeDescriptor.Type.IsCustomType)
                {
                  constants.Add(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReason((object) state.Name));
                  constants.Add(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReasonOut((object) state.Name));
                }
              }
              foreach (WorkItemStateDefinition deltaState in (IEnumerable<WorkItemStateDefinition>) typeDescriptor.DeltaStates)
              {
                constants.Add(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReason((object) deltaState.Name));
                constants.Add(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReasonOut((object) deltaState.Name));
              }
              constants.Add(StatesConstants.AllowedValuesListHead(typeDescriptor.BaseTypeReferenceName));
            }
            IReadOnlyCollection<WorkItemFieldRule> oobRules;
            IReadOnlyCollection<HelpTextDescriptor> oobHelpTexts;
            if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && processDescriptor != null && typeDescriptor.TryGetOutOfBoxRulesAndHelpTexts(requestContext, processDescriptor, out oobRules, out oobHelpTexts))
            {
              constants.UnionWith(MetadataCompatibilityContextGenerator.GetRuleConstants(oobRules));
              constants.UnionWith(MetadataCompatibilityContextGenerator.GetHelpTextConstants(oobHelpTexts));
            }
          }
        }
        if (constants.Count > 0)
        {
          foreach (ConstantRecord constantRecord in (IEnumerable<ConstantRecord>) MetadataCompatibilityContextGenerator.GetConstantRecordsWithRetry(requestContext, (IEnumerable<string>) constants))
            constantMap[constantRecord.DisplayText] = constantRecord.Id;
        }
        return constantMap;
      }));
    }

    public static void AddIdentityConstants(
      Dictionary<string, int> constantMap,
      IEnumerable<WorkItemFieldRule> rules)
    {
      foreach (IdentityDefaultRule identityDefaultRule in rules.SelectMany<WorkItemFieldRule, IdentityDefaultRule>((Func<WorkItemFieldRule, IEnumerable<IdentityDefaultRule>>) (rule => rule.SelectRules<IdentityDefaultRule>().Where<IdentityDefaultRule>((Func<IdentityDefaultRule, bool>) (dr => dr.ValueFrom == RuleValueFrom.Value)))))
        constantMap[identityDefaultRule.Value] = identityDefaultRule.ConstId;
      foreach (IdentityCopyRule identityCopyRule in rules.SelectMany<WorkItemFieldRule, IdentityCopyRule>((Func<WorkItemFieldRule, IEnumerable<IdentityCopyRule>>) (rule => rule.SelectRules<IdentityCopyRule>())))
        constantMap[identityCopyRule.Value] = identityCopyRule.ConstId;
    }

    public static IEnumerable<string> GetRuleConstants(
      IReadOnlyCollection<WorkItemFieldRule> fieldRules)
    {
      foreach (CopyRule copyRule in fieldRules.SelectMany<WorkItemFieldRule, CopyRule>((Func<WorkItemFieldRule, IEnumerable<CopyRule>>) (r => r.SelectRules<CopyRule>())).Where<CopyRule>((Func<CopyRule, bool>) (dr => !(dr is IdentityCopyRule))))
      {
        if (copyRule.ValueFrom == RuleValueFrom.Value && !string.IsNullOrWhiteSpace(copyRule.Value))
          yield return copyRule.Value;
      }
      foreach (AllowedValuesRule avRule in fieldRules.SelectMany<WorkItemFieldRule, AllowedValuesRule>((Func<WorkItemFieldRule, IEnumerable<AllowedValuesRule>>) (r => r.SelectRules<AllowedValuesRule>())))
      {
        if (avRule.Id != Guid.Empty)
          yield return avRule.Id.ToString();
        foreach (string ruleConstant in avRule.Values)
        {
          if (!string.IsNullOrWhiteSpace(ruleConstant))
            yield return ruleConstant;
        }
      }
      foreach (SuggestedValuesRule svRule in fieldRules.SelectMany<WorkItemFieldRule, SuggestedValuesRule>((Func<WorkItemFieldRule, IEnumerable<SuggestedValuesRule>>) (r => r.SelectRules<SuggestedValuesRule>())))
      {
        if (svRule.Id != Guid.Empty)
          yield return svRule.Id.ToString();
        foreach (string ruleConstant in svRule.Values)
        {
          if (!string.IsNullOrWhiteSpace(ruleConstant))
            yield return ruleConstant;
        }
      }
      foreach (WhenRule whenRule in fieldRules.SelectMany<WorkItemFieldRule, WhenRule>((Func<WorkItemFieldRule, IEnumerable<WhenRule>>) (r => r.SelectRules<WhenRule>())))
      {
        if (!string.IsNullOrWhiteSpace(whenRule.Value))
          yield return whenRule.Value;
      }
      foreach (WhenWasRule whenWasRule in fieldRules.SelectMany<WorkItemFieldRule, WhenWasRule>((Func<WorkItemFieldRule, IEnumerable<WhenWasRule>>) (r => r.SelectRules<WhenWasRule>())))
      {
        if (!string.IsNullOrWhiteSpace(whenWasRule.Value))
          yield return whenWasRule.Value;
      }
      foreach (DefaultRule defaultRule in fieldRules.SelectMany<WorkItemFieldRule, DefaultRule>((Func<WorkItemFieldRule, IEnumerable<DefaultRule>>) (r => r.SelectRules<DefaultRule>())).Where<DefaultRule>((Func<DefaultRule, bool>) (dr => dr.ValueFrom == RuleValueFrom.Value && !(dr is IdentityDefaultRule))))
      {
        if (!string.IsNullOrWhiteSpace(defaultRule.Value))
          yield return defaultRule.Value;
      }
    }

    public static IEnumerable<string> GetHelpTextConstants(
      IReadOnlyCollection<HelpTextDescriptor> oobHelpTexts)
    {
      foreach (HelpTextDescriptor oobHelpText in (IEnumerable<HelpTextDescriptor>) oobHelpTexts)
        yield return oobHelpText.HelpText;
    }

    private static IReadOnlyCollection<ConstantRecord> GetConstantRecordsWithRetry(
      IVssRequestContext requestContext,
      IEnumerable<string> constants)
    {
      ITeamFoundationWorkItemTrackingMetadataService service = requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>();
      IEnumerable<ConstantRecord> constantRecords = service.GetNonIdentityConstants(requestContext, constants, false);
      if (constants.Count<string>() != constantRecords.Count<ConstantRecord>())
      {
        List<string> list = constants.Where<string>((Func<string, bool>) (c => !constantRecords.Any<ConstantRecord>((Func<ConstantRecord, bool>) (r => r.DisplayText.Equals(c, StringComparison.OrdinalIgnoreCase))))).ToList<string>();
        using (WorkItemTypeExtensionComponent component = requestContext.CreateComponent<WorkItemTypeExtensionComponent>())
          component.EnsureConstantsForBackcompat((IList<string>) list);
        constantRecords = service.GetNonIdentityConstants(requestContext, constants, false);
      }
      return (IReadOnlyCollection<ConstantRecord>) constantRecords.ToList<ConstantRecord>();
    }

    private class StateDefinitionCacheEntry
    {
      public StateDefinitionCacheEntry(
        IReadOnlyCollection<WorkItemStateDefinition> states,
        IReadOnlyCollection<WorkItemStateDefinition> deltaStates)
      {
        this.States = states;
        this.DeltaStates = deltaStates;
      }

      public IReadOnlyCollection<WorkItemStateDefinition> States { get; set; }

      public IReadOnlyCollection<WorkItemStateDefinition> DeltaStates { get; set; }
    }
  }
}
