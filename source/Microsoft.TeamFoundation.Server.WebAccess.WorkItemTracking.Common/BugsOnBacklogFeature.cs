// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsOnBacklogFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class BugsOnBacklogFeature : ProjectFeatureBase
  {
    public BugsOnBacklogFeature()
      : base(Resources.BugsOnBackLogFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata)
    {
      ProjectProcessConfiguration processConfiguration = projectMetadata.GetProcessConfiguration();
      CategoryConfiguration bugWorkItems;
      if (!this.IsBugCategoryPresentInProject(projectMetadata, out bugWorkItems))
        return ProjectFeatureState.NotConfigured;
      IEnumerable<string> commonWorkItemTypes;
      IEnumerable<string> strings;
      if (!this.IsCategorySubsetOrNotOverlapping(projectMetadata, bugWorkItems.CategoryReferenceName, processConfiguration.RequirementBacklog.CategoryReferenceName, out commonWorkItemTypes) || !this.IsCategorySubsetOrNotOverlapping(projectMetadata, bugWorkItems.CategoryReferenceName, processConfiguration.TaskBacklog.CategoryReferenceName, out strings) || this.IsCategoryOverlappingWithPortfolios(projectMetadata, bugWorkItems.CategoryReferenceName))
        return ProjectFeatureState.PartiallyConfigured;
      return commonWorkItemTypes.Any<string>() || !this.IsBugWorkItemsMetastateMappingValid(projectMetadata, out strings) || !this.FieldExistsOnCategoryWorkItems(projectMetadata, processConfiguration.EffortField.Name, processConfiguration.BugWorkItems.CategoryReferenceName) ? ProjectFeatureState.NotConfigured : ProjectFeatureState.FullyConfigured;
    }

    public override void Process(IProjectProvisioningContext context)
    {
      this.EnsureProcessConfigurationSettings(context);
      this.EnsureProcessSettingsCategoryNode(context, (Func<ProjectProcessConfiguration, CategoryConfiguration>) (s => s.BugWorkItems), (Action<ProjectProcessConfiguration, CategoryConfiguration>) ((s, c) => s.BugWorkItems = c), false);
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      string categoryReferenceName1 = processConfiguration.RequirementBacklog.CategoryReferenceName;
      string categoryReferenceName2 = processConfiguration.BugWorkItems.CategoryReferenceName;
      this.EnsureCategory(context, categoryReferenceName1, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
      this.EnsureCategory(context, categoryReferenceName2, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
      IEnumerable<string> source = this.RemoveOverlappingWorkItemTypesInCategory(context, categoryReferenceName2, categoryReferenceName1);
      this.EnsureBugWorkItemStateMappings(context, source.Any<string>());
      IEnumerable<string> invalidWorkItemTypes;
      if (!this.IsBugWorkItemsMetastateMappingValid((IProjectMetadata) context, out invalidWorkItemTypes))
      {
        foreach (object obj in invalidWorkItemTypes)
        {
          string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Validation_ElementError, (object) "BugWorkItems/States", (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Validation_BacklogWorkItemNotMappedCompletedState, obj));
          context.ReportIssue(new ProjectProvisioningIssue(message, IssueLevel.Error));
        }
      }
      this.EnsureShowBugsOnBacklogProperty(context, source.Any<string>());
      try
      {
        if (this.FieldExistsOnCategoryWorkItems((IProjectMetadata) context, processConfiguration.EffortField.Name, categoryReferenceName2))
          return;
        this.AddFieldToCategoryWorkItemType(context, categoryReferenceName2, processConfiguration.EffortField.Name);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        context.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BugsOnBacklogEffortFieldMissingWarning, (object) Resources.BugsOnBackLogFeatureName, (object) categoryReferenceName2), IssueLevel.Warning));
      }
    }

    private bool IsBugCategoryPresentInProject(
      IProjectMetadata projectMetaData,
      out CategoryConfiguration bugWorkItems)
    {
      ProjectProcessConfiguration processConfiguration = projectMetaData.GetProcessConfiguration();
      if (processConfiguration != null && processConfiguration.BugWorkItems != null)
      {
        bugWorkItems = processConfiguration.BugWorkItems;
        return true;
      }
      bugWorkItems = (CategoryConfiguration) null;
      return false;
    }

    private bool IsBugWorkItemsMetastateMappingValid(
      IProjectMetadata projectMetadata,
      out IEnumerable<string> invalidWorkItemTypes)
    {
      List<string> stringList = new List<string>();
      bool flag = true;
      CategoryConfiguration bugWorkItems;
      if (this.IsBugCategoryPresentInProject(projectMetadata, out bugWorkItems) && ((IEnumerable<State>) bugWorkItems.States).Any<State>())
      {
        IEnumerable<string> second = ((IEnumerable<State>) bugWorkItems.States).Where<State>((Func<State, bool>) (s => s.Type == StateTypeEnum.Complete)).Select<State, string>((Func<State, string>) (s => s.Value));
        foreach (string name in this.GetWorkItemTypeNamesForCategory(projectMetadata, bugWorkItems.CategoryReferenceName))
        {
          WorkItemTypeMetadata workItemType = projectMetadata.GetWorkItemType(name);
          if (workItemType != null && !workItemType.States.Intersect<string>(second).Any<string>())
          {
            stringList.Add(name);
            flag = false;
          }
        }
      }
      invalidWorkItemTypes = (IEnumerable<string>) stringList.ToArray();
      return flag;
    }

    private void EnsureShowBugsOnBacklogProperty(
      IProjectProvisioningContext context,
      bool bugsAlreadyOnRequirementBackLog)
    {
      if (!bugsAlreadyOnRequirementBackLog)
        return;
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      Property[] second = new Property[1]
      {
        new Property()
        {
          Name = "BugsBehavior",
          Value = "AsRequirements"
        }
      };
      if (processConfiguration.Properties == null)
        processConfiguration.Properties = second;
      else
        processConfiguration.Properties = ((IEnumerable<Property>) processConfiguration.Properties).Where<Property>((Func<Property, bool>) (p => !VssStringComparer.PropertyName.Equals(p.Name, "BugsBehavior"))).Concat<Property>((IEnumerable<Property>) second).ToArray<Property>();
    }

    private void EnsureBugWorkItemStateMappings(
      IProjectProvisioningContext context,
      bool bugsAlreadyOnRequirementBackLog)
    {
      CategoryConfiguration bugWorkItems;
      if (!this.IsBugCategoryPresentInProject((IProjectMetadata) context, out bugWorkItems) || !bugsAlreadyOnRequirementBackLog && this.IsBugWorkItemsMetastateMappingValid((IProjectMetadata) context, out IEnumerable<string> _))
        return;
      ProjectProcessConfiguration processConfiguration = context.ProcessTemplate.GetProcessConfiguration();
      BacklogCategoryConfiguration requirementBacklog = context.GetProcessConfiguration().RequirementBacklog;
      List<State> stateList = new List<State>();
      if (bugsAlreadyOnRequirementBackLog)
      {
        foreach (State state in requirementBacklog.States)
          this.AddStateToBacklog((IProjectMetadata) context, bugWorkItems, state, true);
        this.RemoveInvalidStatesFromBacklog((IProjectMetadata) context, (CategoryConfiguration) requirementBacklog);
      }
      else
      {
        foreach (State state in processConfiguration.BugWorkItems.States)
          this.AddStateToBacklog((IProjectMetadata) context, bugWorkItems, state, false);
      }
    }

    private bool IsStateValidForCategory(
      IProjectMetadata projectMetadata,
      string categoryReferenceName,
      string workItemStateName)
    {
      foreach (string name in this.GetWorkItemTypeNamesForCategory(projectMetadata, categoryReferenceName))
      {
        WorkItemTypeMetadata workItemType = projectMetadata.GetWorkItemType(name);
        if (workItemType != null && workItemType.ContainsState(workItemStateName))
          return true;
      }
      return false;
    }

    private void AddStateToBacklog(
      IProjectMetadata projectMetadata,
      CategoryConfiguration backlog,
      State state,
      bool overwriteIfExists)
    {
      if (!this.IsStateValidForCategory(projectMetadata, backlog.CategoryReferenceName, state.Value))
        return;
      backlog.States = backlog.States != null ? backlog.States : new State[0];
      State state1 = Array.Find<State>(backlog.States, (Predicate<State>) (s => s.Value == state.Value));
      if (state1 != null & overwriteIfExists)
      {
        state1.Type = state.Type;
      }
      else
      {
        if (state1 != null)
          return;
        backlog.States = ((IEnumerable<State>) backlog.States).Concat<State>((IEnumerable<State>) new State[1]
        {
          state
        }).ToArray<State>();
      }
    }

    private void RemoveInvalidStatesFromBacklog(
      IProjectMetadata projectMetadata,
      CategoryConfiguration backlog)
    {
      if (backlog == null || backlog.States == null)
        return;
      foreach (State state1 in backlog.States)
      {
        State state = state1;
        if (!this.IsStateValidForCategory(projectMetadata, backlog.CategoryReferenceName, state.Value))
        {
          IEnumerable<State> second = ((IEnumerable<State>) backlog.States).Where<State>((Func<State, bool>) (s => s.Value == state.Value));
          backlog.States = ((IEnumerable<State>) backlog.States).Except<State>(second).ToArray<State>();
        }
      }
    }
  }
}
