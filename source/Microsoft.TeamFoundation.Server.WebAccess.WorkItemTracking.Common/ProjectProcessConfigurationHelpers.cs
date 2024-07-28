// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectProcessConfigurationHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal static class ProjectProcessConfigurationHelpers
  {
    private static bool IsBugCategoryPresentInProject(ProjectProcessConfiguration configuration) => configuration.BugWorkItems != null;

    private static bool IsWorkItemsTypeCommonInBugAndBacklogCategories(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration configuration,
      CommonStructureProjectInfo projectInfo)
    {
      if (configuration.BugWorkItems != null)
      {
        IEnumerable<string> workItemTypes1 = configuration.BugWorkItems.GetWorkItemTypes(requestContext, projectInfo.Name);
        foreach (CategoryConfiguration allBacklog in (IEnumerable<BacklogCategoryConfiguration>) configuration.AllBacklogs)
        {
          IEnumerable<string> workItemTypes2 = allBacklog.GetWorkItemTypes(requestContext, projectInfo.Name);
          if (workItemTypes1.Intersect<string>(workItemTypes2).Any<string>())
            return true;
        }
      }
      return false;
    }

    private static bool IsWorkItemTypeCommonBetweenCategories(
      IVssRequestContext requestContext,
      string projectName,
      CategoryConfiguration sourceCategory,
      CategoryConfiguration targetCategory)
    {
      return sourceCategory != null && targetCategory != null && sourceCategory.GetWorkItemTypes(requestContext, projectName).Intersect<string>(targetCategory.GetWorkItemTypes(requestContext, projectName), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName).Any<string>();
    }

    private static bool IsBugWorkItemsMetastateMappingValid(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration configuration,
      CommonStructureProjectInfo projectInfo)
    {
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      CategoryConfiguration bugWorkItems = configuration.BugWorkItems;
      if (bugWorkItems == null || !((IEnumerable<State>) bugWorkItems.States).Any<State>())
        return false;
      IEnumerable<string> second = ((IEnumerable<State>) bugWorkItems.States).Where<State>((Func<State, bool>) (s => s.Type == StateTypeEnum.Complete)).Select<State, string>((Func<State, string>) (s => s.Value));
      foreach (IWorkItemType workItemType in bugWorkItems.GetWorkItemTypesMetadata(requestContext, projectInfo.Name))
      {
        if (!service.GetAllowedValues(requestContext, 2, projectInfo.Name, (IEnumerable<string>) new List<string>()
        {
          workItemType.Name
        }).Intersect<string>(second).Any<string>())
          return false;
      }
      return true;
    }

    public static IEnumerable<string> GetMissingFieldsInBugCategory(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration configuration,
      IEnumerable<FieldTypeEnum> fields,
      string projectName,
      out string workItemTypeName)
    {
      workItemTypeName = (string) null;
      if (configuration.BugWorkItems == null)
        return Enumerable.Empty<string>();
      List<string> fieldsInBugCategory = new List<string>();
      foreach (IWorkItemType workItemType in configuration.BugWorkItems.GetWorkItemTypesMetadata(requestContext, projectName))
      {
        foreach (FieldTypeEnum field in fields)
        {
          string fieldName = configuration.GetFieldName(field);
          if (!workItemType.GetFields(requestContext).TryGetByName(fieldName, out FieldDefinition _))
            fieldsInBugCategory.Add(fieldName);
        }
        if (fieldsInBugCategory.Count > 0)
        {
          workItemTypeName = workItemType.Name;
          return (IEnumerable<string>) fieldsInBugCategory;
        }
      }
      return (IEnumerable<string>) fieldsInBugCategory;
    }

    private static bool TeamFieldExistsOnBugCategory(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration configuration,
      string projectName)
    {
      return !ProjectProcessConfigurationHelpers.GetMissingFieldsInBugCategory(requestContext, configuration, (IEnumerable<FieldTypeEnum>) new FieldTypeEnum[1]
      {
        FieldTypeEnum.Team
      }, projectName, out string _).Any<string>();
    }

    public static bool IsAnyBugWorkItemTrackedAsPartOfRequirementBacklog(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration configuration,
      string projectName)
    {
      return ProjectProcessConfigurationHelpers.IsWorkItemTypeCommonBetweenCategories(requestContext, projectName, configuration.BugWorkItems, (CategoryConfiguration) configuration.RequirementBacklog);
    }

    public static bool IsAnyBugWorkItemTrackedAsPartOfTaskBacklog(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration configuration,
      string projectName)
    {
      return ProjectProcessConfigurationHelpers.IsWorkItemTypeCommonBetweenCategories(requestContext, projectName, configuration.BugWorkItems, (CategoryConfiguration) configuration.TaskBacklog);
    }

    public static bool IsConfigValidForBugsBehavior(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration configuration,
      CommonStructureProjectInfo projectInfo)
    {
      return ProjectProcessConfigurationHelpers.IsBugCategoryPresentInProject(configuration) && !ProjectProcessConfigurationHelpers.IsWorkItemsTypeCommonInBugAndBacklogCategories(requestContext, configuration, projectInfo) && ProjectProcessConfigurationHelpers.IsBugWorkItemsMetastateMappingValid(requestContext, configuration, projectInfo) && ProjectProcessConfigurationHelpers.TeamFieldExistsOnBugCategory(requestContext, configuration, projectInfo.Name);
    }

    public static IEnumerable<string> GetMissingFieldsForBugBehavior(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration projectSettings,
      string projectName,
      out string typeName)
    {
      return ProjectProcessConfigurationHelpers.GetMissingFieldsInBugCategory(requestContext, projectSettings, (IEnumerable<FieldTypeEnum>) new FieldTypeEnum[3]
      {
        FieldTypeEnum.Effort,
        FieldTypeEnum.RemainingWork,
        FieldTypeEnum.Activity
      }, projectName, out typeName);
    }

    public static IReadOnlyDictionary<string, WorkItemStateCategory> GetStateToStateCategoryMap(
      State[] states)
    {
      Dictionary<string, WorkItemStateCategory> stateCategoryMap = new Dictionary<string, WorkItemStateCategory>();
      foreach (State state in states)
      {
        string key = state.Value;
        switch (state.Type)
        {
          case StateTypeEnum.Proposed:
            stateCategoryMap[key] = WorkItemStateCategory.Proposed;
            break;
          case StateTypeEnum.InProgress:
            stateCategoryMap[key] = WorkItemStateCategory.InProgress;
            break;
          case StateTypeEnum.Complete:
            stateCategoryMap[key] = WorkItemStateCategory.Completed;
            break;
          case StateTypeEnum.Resolved:
            stateCategoryMap[key] = WorkItemStateCategory.Resolved;
            break;
          case StateTypeEnum.Removed:
            stateCategoryMap[key] = WorkItemStateCategory.Removed;
            break;
          default:
            stateCategoryMap[key] = WorkItemStateCategory.Removed;
            break;
        }
      }
      return (IReadOnlyDictionary<string, WorkItemStateCategory>) stateCategoryMap;
    }
  }
}
