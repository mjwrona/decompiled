// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BacklogConfigurationService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BacklogConfigurationService : IBacklogConfigurationService, IVssFrameworkService
  {
    private const string c_backlogLevelDefaultColor = "FF009CCC";
    private const int c_defaultWorkItemCountLimit = 1000;
    private const string s_area = "BacklogConfigurationService";
    private const string s_layer = "BacklogConfigurationService";
    private readonly Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn[] m_defaultColumnFields = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn[4]
    {
      new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn()
      {
        ColumnReferenceName = CoreFieldReferenceNames.WorkItemType,
        Width = 100
      },
      new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn()
      {
        ColumnReferenceName = CoreFieldReferenceNames.Title,
        Width = 400
      },
      new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn()
      {
        ColumnReferenceName = CoreFieldReferenceNames.State,
        Width = 100
      },
      new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn()
      {
        ColumnReferenceName = CoreFieldReferenceNames.Tags,
        Width = 200
      }
    };

    public virtual Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration GetProjectBacklogConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      bool validateProcessConfig = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return requestContext.TraceBlock<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>(15165000, 15165001, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), "GetBacklogConfiguration_Project", (Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>) (() =>
      {
        ProcessDescriptor processDescriptor = (ProcessDescriptor) null;
        ProjectProcessConfiguration processSettings = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(projectId), validateProcessConfig);
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = !this.TryGetLatestProjectProcessDescriptor(requestContext, projectId, out processDescriptor) || !processDescriptor.IsDerived ? this.GetBacklogConfiguration(requestContext, projectId, processSettings, validateProcessConfig) : this.GetBacklogConfiguration(requestContext, projectId, processSettings, processDescriptor, validateProcessConfig);
        if (validateProcessConfig)
          backlogConfiguration.PortfolioBacklogs = this.FilterPortfolioBacklogs(requestContext, backlogConfiguration);
        return backlogConfiguration;
      }));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private IReadOnlyCollection<BacklogLevelConfiguration> FilterPortfolioBacklogs(
      IVssRequestContext requestContxt,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration)
    {
      BacklogLevelConfiguration lowestPB = backlogConfiguration.PortfolioBacklogs.Where<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (pb => pb.WorkItemTypes == null || pb.WorkItemTypes.Count == 0)).OrderBy<BacklogLevelConfiguration, int>((Func<BacklogLevelConfiguration, int>) (pb => pb.Rank)).FirstOrDefault<BacklogLevelConfiguration>();
      if (lowestPB == null)
        return backlogConfiguration.PortfolioBacklogs;
      requestContxt.Trace(15165030, TraceLevel.Info, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), string.Format("Filtering any backlog levels that are greater than rank {0} as {1} does not have any work items associated with it.", (object) lowestPB.Rank, (object) lowestPB.Name));
      return (IReadOnlyCollection<BacklogLevelConfiguration>) backlogConfiguration.PortfolioBacklogs.Where<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (pb => pb.Rank < lowestPB.Rank)).ToList<BacklogLevelConfiguration>();
    }

    private Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration GetBacklogConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      ProjectProcessConfiguration processSettings,
      ProcessDescriptor processDescriptor,
      bool validateProcessSettings)
    {
      return requestContext.TraceBlock<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>(15165006, 15165007, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), "GetBacklogConfiguration_Process", (Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>) (() =>
      {
        IBehaviorService service1 = requestContext.GetService<IBehaviorService>();
        IProcessWorkItemTypeService service2 = requestContext.GetService<IProcessWorkItemTypeService>();
        List<BacklogLevelConfiguration> levelConfigurationList = new List<BacklogLevelConfiguration>();
        IVssRequestContext requestContext1 = requestContext;
        Guid typeId = processDescriptor.TypeId;
        IReadOnlyCollection<Behavior> list1 = (IReadOnlyCollection<Behavior>) service1.GetBacklogBehaviors(requestContext1, typeId).Where<Behavior>((Func<Behavior, bool>) (bb => !bb.IsAbstract && !bb.IsDeleted)).ToList<Behavior>();
        IReadOnlyDictionary<string, BehaviorWorkItemTypes> itemTypesInBehavior = service2.GetWorkItemTypesInBehavior(requestContext, processDescriptor.TypeId);
        List<ComposedWorkItemType> list2 = itemTypesInBehavior.SelectMany<KeyValuePair<string, BehaviorWorkItemTypes>, ComposedWorkItemType>((Func<KeyValuePair<string, BehaviorWorkItemTypes>, IEnumerable<ComposedWorkItemType>>) (bwit => (IEnumerable<ComposedWorkItemType>) bwit.Value.WorkItemTypes)).ToList<ComposedWorkItemType>();
        HashSet<string> bugWorkItemTypeNames = new HashSet<string>();
        WorkItemTypeCategory workItemTypeCategory;
        if (validateProcessSettings && processSettings.IsConfigValidForBugsBehavior(requestContext, projectId) && requestContext.GetService<IWorkItemTypeCategoryService>().TryGetWorkItemTypeCategory(requestContext, projectId, processSettings.BugWorkItems.CategoryReferenceName, out workItemTypeCategory))
        {
          bugWorkItemTypeNames = new HashSet<string>(workItemTypeCategory.WorkItemTypeNames, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
          IEnumerable<ComposedWorkItemType> collection = service2.GetAllWorkItemTypes(requestContext, processDescriptor.TypeId).Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (wit => bugWorkItemTypeNames.Contains(wit.Name)));
          list2.AddRange(collection);
        }
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration1 = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration();
        backlogConfiguration1.PortfolioBacklogs = (IReadOnlyCollection<BacklogLevelConfiguration>) levelConfigurationList;
        backlogConfiguration1.BacklogFields = new BacklogFieldInfo()
        {
          TypeFields = this.GetTypeFields(requestContext, processSettings, list1, validateProcessSettings)
        };
        backlogConfiguration1.ProcessDescriptor = processDescriptor;
        backlogConfiguration1.ProjectId = new Guid?(projectId);
        backlogConfiguration1.WorkItemTypeMappedStates = this.GetWorkItemTypeStates(requestContext, processDescriptor.TypeId, list2);
        backlogConfiguration1.IsBugsBehaviorConfigValid = processSettings.IsConfigValidForBugsBehavior(requestContext, projectId);
        IEnumerable<string> hiddenBacklogs = processSettings.HiddenBacklogs;
        backlogConfiguration1.HiddenBacklogs = hiddenBacklogs != null ? (IReadOnlyCollection<string>) hiddenBacklogs.ToList<string>() : (IReadOnlyCollection<string>) null;
        backlogConfiguration1.BugWorkItemTypes = (IReadOnlyCollection<string>) bugWorkItemTypeNames;
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration2 = backlogConfiguration1;
        if (validateProcessSettings && processSettings.IsConfigValidForBugsBehavior(requestContext, projectId))
        {
          backlogConfiguration2.IsBugsBehaviorConfigValid = true;
          backlogConfiguration2.BugsBehavior = AgileSettingsUtils.Convert(processSettings.BugsBehavior);
        }
        foreach (Behavior backlogBehavior in (IEnumerable<Behavior>) list1)
        {
          BacklogLevelConfiguration levelConfiguration = this.GetBacklogLevelConfiguration(requestContext, backlogBehavior, itemTypesInBehavior, processSettings);
          if (TFStringComparer.BehaviorReferenceName.Equals(backlogBehavior.ReferenceName, BehaviorService.TaskBehaviorReferenceName))
          {
            levelConfiguration.IsTaskBacklog = true;
            backlogConfiguration2.TaskBacklog = levelConfiguration;
          }
          else if (TFStringComparer.BehaviorReferenceName.Equals(backlogBehavior.ReferenceName, BehaviorService.RequirementBehaviorReferenceName))
          {
            levelConfiguration.IsRequirementsBacklog = true;
            backlogConfiguration2.RequirementBacklog = levelConfiguration;
          }
          else
            levelConfigurationList.Add(levelConfiguration);
        }
        return backlogConfiguration2;
      }));
    }

    protected virtual BacklogLevelConfiguration GetBacklogLevelConfiguration(
      IVssRequestContext requestContext,
      Behavior backlogBehavior,
      IReadOnlyDictionary<string, BehaviorWorkItemTypes> behaviorWorkItemTypesMap,
      ProjectProcessConfiguration processSettings)
    {
      return requestContext.TraceBlock<BacklogLevelConfiguration>(15165012, 15165013, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), nameof (GetBacklogLevelConfiguration), (Func<BacklogLevelConfiguration>) (() =>
      {
        BacklogCategoryConfiguration backlogCategoryConfiguration = (BacklogCategoryConfiguration) null;
        List<string> source = new List<string>();
        BacklogLevelConfiguration levelConfiguration = new BacklogLevelConfiguration()
        {
          Id = backlogBehavior.ReferenceName,
          Color = backlogBehavior.Color,
          Name = backlogBehavior.Name,
          Rank = backlogBehavior.Rank,
          WorkItemTypes = (IReadOnlyCollection<string>) source,
          AddPanelFields = new string[1]
          {
            CoreFieldReferenceNames.Title
          },
          ColumnFields = this.m_defaultColumnFields,
          WorkItemCountLimit = 1000,
          Custom = backlogBehavior.Custom
        };
        BehaviorWorkItemTypes behaviorWorkItemTypes = (BehaviorWorkItemTypes) null;
        if (behaviorWorkItemTypesMap.TryGetValue(backlogBehavior.ReferenceName, out behaviorWorkItemTypes))
        {
          source.AddRange((IEnumerable<string>) behaviorWorkItemTypes.WorkItemTypes.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (wt => !wt.IsDeleted)).Select<ComposedWorkItemType, string>((Func<ComposedWorkItemType, string>) (wt => wt.Name)).OrderBy<string, string>((Func<string, string>) (name => name), (IComparer<string>) TFStringComparer.WorkItemTypeName));
          levelConfiguration.DefaultWorkItemType = behaviorWorkItemTypes.DefaultWorkItemTypeName;
          if (!source.Contains<string>(levelConfiguration.DefaultWorkItemType, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))
            levelConfiguration.DefaultWorkItemType = source.FirstOrDefault<string>();
        }
        else
          requestContext.Trace(15165005, TraceLevel.Error, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), "No work item types in behavior :" + backlogBehavior.ReferenceName);
        BacklogCategoryConfiguration categoryConfiguration = ((IEnumerable<BacklogCategoryConfiguration>) processSettings.PortfolioBacklogs).FirstOrDefault<BacklogCategoryConfiguration>();
        if (this.TryGetOriginalBacklogLevelConfiguration(processSettings, backlogBehavior.ReferenceName, backlogBehavior.Rank, out backlogCategoryConfiguration))
        {
          levelConfiguration.Id = backlogCategoryConfiguration.CategoryReferenceName;
          levelConfiguration.AddPanelFields = ((IEnumerable<Field>) backlogCategoryConfiguration.AddPanel.Fields).Select<Field, string>((Func<Field, string>) (f => f.Name)).ToArray<string>();
          levelConfiguration.ColumnFields = ((IEnumerable<Column>) backlogCategoryConfiguration.Columns).Select<Column, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>((Func<Column, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>) (c => new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn()
          {
            ColumnReferenceName = c.FieldName,
            Width = c.ColumnWidth
          })).ToArray<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>();
          levelConfiguration.WorkItemCountLimit = backlogCategoryConfiguration.WorkItemCountLimit;
        }
        else if (categoryConfiguration != null)
        {
          levelConfiguration.AddPanelFields = categoryConfiguration.AddPanel.GetFieldNames().ToArray<string>();
          levelConfiguration.ColumnFields = ((IEnumerable<Column>) categoryConfiguration.Columns).Select<Column, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>((Func<Column, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>) (c => new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn()
          {
            ColumnReferenceName = c.FieldName,
            Width = c.ColumnWidth
          })).ToArray<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>();
          levelConfiguration.WorkItemCountLimit = categoryConfiguration.WorkItemCountLimit;
        }
        return levelConfiguration;
      }));
    }

    private IReadOnlyDictionary<FieldTypeEnum, string> GetTypeFields(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration processSettings,
      IReadOnlyCollection<Behavior> backlogBehaviors,
      bool validateProcessSettings)
    {
      return (IReadOnlyDictionary<FieldTypeEnum, string>) requestContext.TraceBlock<Dictionary<FieldTypeEnum, string>>(15165014, 15165015, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), nameof (GetTypeFields), (Func<Dictionary<FieldTypeEnum, string>>) (() =>
      {
        Dictionary<FieldTypeEnum, string> typeFields = new Dictionary<FieldTypeEnum, string>();
        foreach (Behavior backlogBehavior in (IEnumerable<Behavior>) backlogBehaviors)
        {
          foreach (KeyValuePair<string, FieldEntry> combinedBehaviorField in (IEnumerable<KeyValuePair<string, FieldEntry>>) backlogBehavior.GetCombinedBehaviorFields(requestContext))
          {
            FieldTypeEnum result;
            if (Enum.TryParse<FieldTypeEnum>(combinedBehaviorField.Key, out result))
            {
              string strA;
              if (typeFields.TryGetValue(result, out strA) && string.Compare(strA, combinedBehaviorField.Value.ReferenceName, StringComparison.CurrentCultureIgnoreCase) != 0)
              {
                string[] strArray = new string[5]
                {
                  "Overwriting type field '",
                  strA,
                  "' with '",
                  combinedBehaviorField.Value.ReferenceName,
                  "'"
                };
                requestContext.TraceAlways(15165031, TraceLevel.Error, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), string.Concat(strArray));
              }
              typeFields[result] = combinedBehaviorField.Value.ReferenceName;
            }
            else
              requestContext.Trace(15165004, TraceLevel.Error, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), "Could not parse FieldTypeEnum:" + combinedBehaviorField.Key);
          }
        }
        if (validateProcessSettings)
          this.ConvertBacklogField(processSettings.TypeFields).TypeFields.ForEach<KeyValuePair<FieldTypeEnum, string>>((Action<KeyValuePair<FieldTypeEnum, string>>) (typeField =>
          {
            if (typeFields.ContainsKey(typeField.Key))
              return;
            typeFields[typeField.Key] = typeField.Value;
          }));
        return typeFields;
      }));
    }

    private bool TryGetOriginalBacklogLevelConfiguration(
      ProjectProcessConfiguration processSettings,
      string id,
      int behaviorRank,
      out BacklogCategoryConfiguration backlogCategoryConfiguration)
    {
      int index = (behaviorRank - BehaviorService.RequirementBacklogRank) / BehaviorService.BacklogLevelIncrements - 1;
      backlogCategoryConfiguration = (BacklogCategoryConfiguration) null;
      if (TFStringComparer.WorkItemCategoryReferenceName.Equals(BehaviorService.TaskBehaviorReferenceName, id))
        backlogCategoryConfiguration = processSettings.TaskBacklog;
      else if (TFStringComparer.WorkItemCategoryReferenceName.Equals(BehaviorService.RequirementBehaviorReferenceName, id))
        backlogCategoryConfiguration = processSettings.RequirementBacklog;
      else if (behaviorRank > BehaviorService.RequirementBacklogRank && processSettings.PortfolioBacklogs != null && index < processSettings.PortfolioBacklogs.Length)
        backlogCategoryConfiguration = ((IEnumerable<BacklogCategoryConfiguration>) processSettings.PortfolioBacklogs).Reverse<BacklogCategoryConfiguration>().ElementAt<BacklogCategoryConfiguration>(index);
      return backlogCategoryConfiguration != null;
    }

    private bool TryGetLatestProjectProcessDescriptor(
      IVssRequestContext requestContext,
      Guid projectId,
      out ProcessDescriptor processDescriptor)
    {
      processDescriptor = (ProcessDescriptor) null;
      IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
      return WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && service.TryGetLatestProjectProcessDescriptor(requestContext, projectId, out processDescriptor);
    }

    private Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration GetBacklogConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      ProjectProcessConfiguration processSettings,
      bool validateProcessSettings)
    {
      return requestContext.TraceBlock<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>(15165008, 15165009, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), "GetBacklogConfiguration_ProcessSettings", (Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>) (() =>
      {
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration();
        requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId);
        List<BacklogLevelConfiguration> portfolioBacklogConfigurations = new List<BacklogLevelConfiguration>();
        Dictionary<string, WorkItemTypeCategory> workItemTypeCategoriesByReferenceName = new Dictionary<string, WorkItemTypeCategory>(0);
        if (validateProcessSettings)
          workItemTypeCategoriesByReferenceName = requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(requestContext, projectId).ToDictionary<WorkItemTypeCategory, string>((Func<WorkItemTypeCategory, string>) (c => c.ReferenceName), (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
        backlogConfiguration.TaskBacklog = this.CreateBacklogLevelConfiguration(requestContext, processSettings, processSettings.TaskBacklog, BehaviorService.TaskBacklogRank, (IReadOnlyDictionary<string, WorkItemTypeCategory>) workItemTypeCategoriesByReferenceName, validateProcessSettings);
        backlogConfiguration.TaskBacklog.IsTaskBacklog = true;
        backlogConfiguration.RequirementBacklog = this.CreateBacklogLevelConfiguration(requestContext, processSettings, processSettings.RequirementBacklog, BehaviorService.RequirementBacklogRank, (IReadOnlyDictionary<string, WorkItemTypeCategory>) workItemTypeCategoriesByReferenceName, validateProcessSettings);
        backlogConfiguration.RequirementBacklog.IsRequirementsBacklog = true;
        Action action = (Action) (() =>
        {
          BacklogCategoryConfiguration[] array = ((IEnumerable<BacklogCategoryConfiguration>) processSettings.PortfolioBacklogs).Reverse<BacklogCategoryConfiguration>().ToArray<BacklogCategoryConfiguration>();
          for (int index = 0; index < array.Length; ++index)
            portfolioBacklogConfigurations.Add(this.CreateBacklogLevelConfiguration(requestContext, processSettings, array[index], BehaviorService.RequirementBacklogRank + (index + 1) * BehaviorService.BacklogLevelIncrements, (IReadOnlyDictionary<string, WorkItemTypeCategory>) workItemTypeCategoriesByReferenceName, validateProcessSettings));
        });
        backlogConfiguration.PortfolioBacklogs = (IReadOnlyCollection<BacklogLevelConfiguration>) portfolioBacklogConfigurations;
        backlogConfiguration.ProjectId = new Guid?(projectId);
        backlogConfiguration.HiddenBacklogs = processSettings.HiddenBacklogs == null ? (IReadOnlyCollection<string>) new HashSet<string>() : (IReadOnlyCollection<string>) new HashSet<string>(processSettings.HiddenBacklogs, (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
        if (validateProcessSettings)
        {
          action();
          backlogConfiguration.WorkItemTypeMappedStates = this.GetWorkItemMappedStates(requestContext, projectId, processSettings, (IReadOnlyDictionary<string, WorkItemTypeCategory>) workItemTypeCategoriesByReferenceName);
          backlogConfiguration.BacklogFields = this.ConvertBacklogField(processSettings.TypeFields);
        }
        else
        {
          if (processSettings.PortfolioBacklogs != null)
            action();
          backlogConfiguration.WorkItemTypeMappedStates = (IReadOnlyCollection<WorkItemTypeStateInfo>) new List<WorkItemTypeStateInfo>();
          backlogConfiguration.BacklogFields = new BacklogFieldInfo();
        }
        backlogConfiguration.BugWorkItemTypes = (IReadOnlyCollection<string>) new List<string>();
        if (validateProcessSettings && processSettings.IsConfigValidForBugsBehavior(requestContext, projectId))
        {
          backlogConfiguration.IsBugsBehaviorConfigValid = true;
          backlogConfiguration.BugsBehavior = AgileSettingsUtils.Convert(processSettings.BugsBehavior);
          WorkItemTypeCategory itemTypeCategory;
          if (processSettings.BugWorkItems != null && workItemTypeCategoriesByReferenceName.TryGetValue(processSettings.BugWorkItems.CategoryReferenceName, out itemTypeCategory))
            backlogConfiguration.BugWorkItemTypes = (IReadOnlyCollection<string>) itemTypeCategory.WorkItemTypeNames.ToList<string>();
        }
        return backlogConfiguration;
      }));
    }

    private IReadOnlyCollection<WorkItemTypeStateInfo> GetWorkItemMappedStates(
      IVssRequestContext requestContext,
      Guid projectId,
      ProjectProcessConfiguration processSettings,
      IReadOnlyDictionary<string, WorkItemTypeCategory> workItemTypeCategoriesByReferenceName)
    {
      return (IReadOnlyCollection<WorkItemTypeStateInfo>) requestContext.TraceBlock<List<WorkItemTypeStateInfo>>(15165010, 15165011, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), "GetWorkItemTypeStates_ProcessSettings", (Func<List<WorkItemTypeStateInfo>>) (() =>
      {
        List<CategoryConfiguration> list1 = ((IEnumerable<CategoryConfiguration>) ((IEnumerable<BacklogCategoryConfiguration>) processSettings.AllBacklogs).Where<BacklogCategoryConfiguration>((Func<BacklogCategoryConfiguration, bool>) (bc => bc.States != null && ((IEnumerable<State>) bc.States).Any<State>()))).ToList<CategoryConfiguration>();
        if (processSettings.BugWorkItems != null && processSettings.IsConfigValidForBugsBehavior(requestContext, projectId))
          list1.Add(processSettings.BugWorkItems);
        List<WorkItemTypeStateInfo> itemMappedStates = new List<WorkItemTypeStateInfo>();
        foreach (CategoryConfiguration categoryConfiguration in list1)
        {
          IReadOnlyDictionary<string, WorkItemStateCategory> stateCategoryMap = ProjectProcessConfigurationHelpers.GetStateToStateCategoryMap(categoryConfiguration.States);
          WorkItemTypeCategory itemTypeCategory;
          if (workItemTypeCategoriesByReferenceName.TryGetValue(categoryConfiguration.CategoryReferenceName, out itemTypeCategory))
          {
            foreach (string workItemTypeName in itemTypeCategory.WorkItemTypeNames)
            {
              IReadOnlyCollection<WorkItemStateDefinition> list2;
              WorkItemTypeStateInfo itemTypeStateInfo;
              if (categoryConfiguration.ProcessWorkItemTypeStates.TryGetValue(workItemTypeName, out list2))
              {
                list2 = (IReadOnlyCollection<WorkItemStateDefinition>) list2.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (ds => ds.StateCategory != WorkItemStateCategory.Removed && !ds.Hidden)).ToList<WorkItemStateDefinition>();
                IReadOnlyDictionary<string, WorkItemStateCategory> readOnlyDictionary = this.MergedStates(stateCategoryMap, list2);
                itemTypeStateInfo = new WorkItemTypeStateInfo()
                {
                  WorkItemTypeName = workItemTypeName,
                  States = readOnlyDictionary
                };
              }
              else
                itemTypeStateInfo = new WorkItemTypeStateInfo()
                {
                  WorkItemTypeName = workItemTypeName,
                  States = stateCategoryMap
                };
              itemMappedStates.Add(itemTypeStateInfo);
            }
          }
        }
        return itemMappedStates;
      }));
    }

    private IReadOnlyDictionary<string, WorkItemStateCategory> MergedStates(
      IReadOnlyDictionary<string, WorkItemStateCategory> states,
      IReadOnlyCollection<WorkItemStateDefinition> deltaStates)
    {
      Dictionary<string, WorkItemStateCategory> dictionary = new Dictionary<string, WorkItemStateCategory>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      foreach (KeyValuePair<string, WorkItemStateCategory> state in (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) states)
        dictionary[state.Key] = state.Value;
      foreach (WorkItemStateDefinition deltaState in (IEnumerable<WorkItemStateDefinition>) deltaStates)
        dictionary[deltaState.Name] = deltaState.StateCategory;
      return (IReadOnlyDictionary<string, WorkItemStateCategory>) dictionary;
    }

    private IReadOnlyCollection<WorkItemTypeStateInfo> GetWorkItemTypeStates(
      IVssRequestContext requestContext,
      Guid processId,
      List<ComposedWorkItemType> workItemTypes)
    {
      return (IReadOnlyCollection<WorkItemTypeStateInfo>) requestContext.TraceBlock<List<WorkItemTypeStateInfo>>(15165023, 15165024, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), "GetWorkItemTypeStates_Process", (Func<List<WorkItemTypeStateInfo>>) (() =>
      {
        List<WorkItemTypeStateInfo> workItemTypeStates = new List<WorkItemTypeStateInfo>();
        IWorkItemStateDefinitionService service = requestContext.GetService<IWorkItemStateDefinitionService>();
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
        requestContext.GetService<IProcessWorkItemTypeService>();
        foreach (ComposedWorkItemType workItemType in workItemTypes)
        {
          if (!stringSet.Contains(workItemType.Name))
          {
            stringSet.Add(workItemType.Name);
            IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions = service.GetCombinedStateDefinitions(requestContext, workItemType.ProcessId, workItemType.ReferenceName);
            WorkItemTypeStateInfo itemTypeStateInfo = new WorkItemTypeStateInfo()
            {
              WorkItemTypeName = workItemType.Name,
              States = (IReadOnlyDictionary<string, WorkItemStateCategory>) stateDefinitions.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (sd => !sd.Hidden && sd.StateCategory != WorkItemStateCategory.Removed)).ToDedupedDictionary<WorkItemStateDefinition, string, WorkItemStateCategory>((Func<WorkItemStateDefinition, string>) (sd => sd.Name), (Func<WorkItemStateDefinition, WorkItemStateCategory>) (sd => sd.StateCategory), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName)
            };
            workItemTypeStates.Add(itemTypeStateInfo);
          }
        }
        return workItemTypeStates;
      }));
    }

    private BacklogLevelConfiguration CreateBacklogLevelConfiguration(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration processConfiguration,
      BacklogCategoryConfiguration backlogCategoryConfiguration,
      int rank,
      IReadOnlyDictionary<string, WorkItemTypeCategory> workItemTypeCategoriesByReferenceName,
      bool validateProcessSettings)
    {
      return requestContext.TraceBlock<BacklogLevelConfiguration>(15165020, 15165021, nameof (BacklogConfigurationService), nameof (BacklogConfigurationService), "CreateBacklogLevelConfiguration_ProcessSettings", (Func<BacklogLevelConfiguration>) (() =>
      {
        BacklogLevelConfiguration levelConfiguration1 = new BacklogLevelConfiguration();
        if (processConfiguration.IsDefault)
          return levelConfiguration1;
        levelConfiguration1.Id = backlogCategoryConfiguration.CategoryReferenceName;
        levelConfiguration1.Name = backlogCategoryConfiguration.PluralName;
        levelConfiguration1.Rank = rank;
        levelConfiguration1.WorkItemCountLimit = backlogCategoryConfiguration.WorkItemCountLimit;
        if (validateProcessSettings)
        {
          levelConfiguration1.AddPanelFields = ((IEnumerable<Field>) backlogCategoryConfiguration.AddPanel.Fields).Select<Field, string>((Func<Field, string>) (field => field.Name)).ToArray<string>();
          levelConfiguration1.ColumnFields = ((IEnumerable<Column>) backlogCategoryConfiguration.Columns).Select<Column, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>((Func<Column, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>) (column => new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn()
          {
            ColumnReferenceName = column.FieldName,
            Width = column.ColumnWidth
          })).ToArray<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>();
          WorkItemTypeCategory backlogCategory = (WorkItemTypeCategory) null;
          if (workItemTypeCategoriesByReferenceName.TryGetValue(backlogCategoryConfiguration.CategoryReferenceName, out backlogCategory))
          {
            levelConfiguration1.WorkItemTypes = (IReadOnlyCollection<string>) backlogCategory.WorkItemTypeNames.ToList<string>();
            levelConfiguration1.DefaultWorkItemType = backlogCategory.DefaultWorkItemTypeName;
            BacklogLevelConfiguration levelConfiguration2 = levelConfiguration1;
            WorkItemColor[] workItemColors = processConfiguration.WorkItemColors;
            string primaryColor = workItemColors != null ? ((IEnumerable<WorkItemColor>) workItemColors).FirstOrDefault<WorkItemColor>((Func<WorkItemColor, bool>) (w => TFStringComparer.WorkItemTypeName.Equals(w.WorkItemTypeName, backlogCategory.DefaultWorkItemTypeName)))?.PrimaryColor : (string) null;
            levelConfiguration2.Color = primaryColor;
          }
        }
        else
        {
          levelConfiguration1.AddPanelFields = Array.Empty<string>();
          levelConfiguration1.ColumnFields = Array.Empty<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>();
          levelConfiguration1.WorkItemTypes = (IReadOnlyCollection<string>) new List<string>();
          levelConfiguration1.DefaultWorkItemType = string.Empty;
          levelConfiguration1.Color = string.Empty;
        }
        if (string.IsNullOrWhiteSpace(levelConfiguration1.Color))
          levelConfiguration1.Color = "FF009CCC";
        return levelConfiguration1;
      }));
    }

    private BacklogFieldInfo ConvertBacklogField(TypeField[] typeFields)
    {
      BacklogFieldInfo backlogFieldInfo = new BacklogFieldInfo();
      Dictionary<FieldTypeEnum, string> dictionary = new Dictionary<FieldTypeEnum, string>();
      backlogFieldInfo.TypeFields = (IReadOnlyDictionary<FieldTypeEnum, string>) dictionary;
      foreach (TypeField typeField in typeFields)
        dictionary[typeField.Type] = typeField.Name;
      return backlogFieldInfo;
    }
  }
}
