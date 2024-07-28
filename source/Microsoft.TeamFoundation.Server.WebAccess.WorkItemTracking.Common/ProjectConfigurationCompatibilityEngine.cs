// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectConfigurationCompatibilityEngine
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal static class ProjectConfigurationCompatibilityEngine
  {
    public static ProjectProcessConfiguration GetProjectProcessConfiguration(
      CommonProjectConfiguration commonConfiguration,
      AgileProjectConfiguration agileConfiguration)
    {
      ArgumentUtility.CheckForNull<CommonProjectConfiguration>(commonConfiguration, nameof (commonConfiguration));
      ArgumentUtility.CheckForNull<AgileProjectConfiguration>(agileConfiguration, nameof (agileConfiguration));
      return new ProjectProcessConfiguration()
      {
        TypeFields = commonConfiguration.TypeFields,
        Weekends = commonConfiguration.Weekends,
        FeedbackRequestWorkItems = ProjectConfigurationCompatibilityEngine.ConvertToCategoryConfiguration(commonConfiguration.FeedbackRequestWorkItems),
        FeedbackResponseWorkItems = ProjectConfigurationCompatibilityEngine.ConvertToCategoryConfiguration(commonConfiguration.FeedbackResponseWorkItems),
        FeedbackWorkItems = ProjectConfigurationCompatibilityEngine.ConvertToCategoryConfiguration(commonConfiguration.FeedbackWorkItems),
        BugWorkItems = ProjectConfigurationCompatibilityEngine.ConvertToCategoryConfiguration(commonConfiguration.BugWorkItems),
        RequirementBacklog = ProjectConfigurationCompatibilityEngine.ConvertToBacklogCategoryConfiguration(commonConfiguration.RequirementWorkItems, agileConfiguration.ProductBacklog.AddPanel, agileConfiguration.ProductBacklog.Columns, 1000),
        TaskBacklog = ProjectConfigurationCompatibilityEngine.ConvertToBacklogCategoryConfiguration(commonConfiguration.TaskWorkItems, (AddPanelConfiguration) null, agileConfiguration.IterationBacklog.Columns, agileConfiguration.IterationBacklog.WorkItemCountLimit)
      };
    }

    public static CommonProjectConfiguration GetCommonProjectConfiguration(
      ProjectProcessConfiguration projectConfiguration)
    {
      return new CommonProjectConfiguration()
      {
        TypeFields = projectConfiguration.TypeFields,
        Weekends = projectConfiguration.Weekends,
        FeedbackRequestWorkItems = ProjectConfigurationCompatibilityEngine.ConvertToWorkItemCategory(projectConfiguration.FeedbackRequestWorkItems),
        FeedbackResponseWorkItems = ProjectConfigurationCompatibilityEngine.ConvertToWorkItemCategory(projectConfiguration.FeedbackResponseWorkItems),
        FeedbackWorkItems = ProjectConfigurationCompatibilityEngine.ConvertToWorkItemCategory(projectConfiguration.FeedbackWorkItems),
        BugWorkItems = ProjectConfigurationCompatibilityEngine.ConvertToWorkItemCategory(projectConfiguration.BugWorkItems),
        RequirementWorkItems = ProjectConfigurationCompatibilityEngine.ConvertToWorkItemCategory((CategoryConfiguration) projectConfiguration.RequirementBacklog),
        TaskWorkItems = ProjectConfigurationCompatibilityEngine.ConvertToWorkItemCategory((CategoryConfiguration) projectConfiguration.TaskBacklog)
      };
    }

    public static AgileProjectConfiguration GetAgileProjectConfiguration(
      ProjectProcessConfiguration projectConfiguration)
    {
      AgileProjectConfiguration projectConfiguration1 = new AgileProjectConfiguration();
      if (projectConfiguration.TaskBacklog != null)
      {
        projectConfiguration1.IterationBacklog.Columns = projectConfiguration.TaskBacklog.Columns;
        projectConfiguration1.IterationBacklog.WorkItemCountLimit = projectConfiguration.TaskBacklog.WorkItemCountLimit;
      }
      if (projectConfiguration.RequirementBacklog != null)
      {
        projectConfiguration1.ProductBacklog.Columns = projectConfiguration.RequirementBacklog.Columns;
        projectConfiguration1.ProductBacklog.AddPanel = projectConfiguration.RequirementBacklog.AddPanel;
      }
      return projectConfiguration1;
    }

    private static CategoryConfiguration ConvertToCategoryConfiguration(WorkItemCategory category)
    {
      if (category == null)
        return (CategoryConfiguration) null;
      CategoryConfiguration target = new CategoryConfiguration();
      ProjectConfigurationCompatibilityEngine.CopyWorkItemCategoryToCategoryConfiguration(category, target);
      return target;
    }

    private static BacklogCategoryConfiguration ConvertToBacklogCategoryConfiguration(
      WorkItemCategory category,
      AddPanelConfiguration addPanel,
      Column[] columns,
      int workItemCountLimit)
    {
      if (category == null)
        return (BacklogCategoryConfiguration) null;
      BacklogCategoryConfiguration target = new BacklogCategoryConfiguration();
      ProjectConfigurationCompatibilityEngine.CopyWorkItemCategoryToCategoryConfiguration(category, (CategoryConfiguration) target);
      target.AddPanel = addPanel;
      target.Columns = columns;
      target.WorkItemCountLimit = workItemCountLimit;
      return target;
    }

    private static void CopyWorkItemCategoryToCategoryConfiguration(
      WorkItemCategory source,
      CategoryConfiguration target)
    {
      target.CategoryReferenceName = source.CategoryName;
      target.PluralName = source.PluralName;
      target.SingularName = (string) null;
      target.States = source.States;
    }

    private static WorkItemCategory ConvertToWorkItemCategory(CategoryConfiguration category)
    {
      if (category == null)
        return (WorkItemCategory) null;
      return new WorkItemCategory()
      {
        CategoryName = category.CategoryReferenceName,
        PluralName = category.PluralName,
        States = category.States
      };
    }
  }
}
