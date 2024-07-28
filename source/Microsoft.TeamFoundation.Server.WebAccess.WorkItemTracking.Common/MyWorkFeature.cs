// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.MyWorkFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class MyWorkFeature : ProjectFeatureBase
  {
    private readonly string StartWorkActionName = "Microsoft.VSTS.Actions.StartWork";
    private readonly string StopWorkActionName = "Microsoft.VSTS.Actions.StopWork";

    public MyWorkFeature()
      : base(Resources.MyWorkFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata)
    {
      ProjectProcessConfiguration processConfiguration = projectMetadata.GetProcessConfiguration();
      if (processConfiguration != null && processConfiguration.TaskBacklog != null && projectMetadata.GetWorkItemTypeCategory(processConfiguration.TaskBacklog.CategoryReferenceName) != null)
        return ProjectFeatureState.FullyConfigured;
      return processConfiguration == null ? ProjectFeatureState.NotConfigured : ProjectFeatureState.PartiallyConfigured;
    }

    public override void Process(IProjectProvisioningContext context)
    {
      this.EnsureProcessConfigurationSettings(context);
      this.EnsureProcessSettingsCategoryNode(context, (Func<ProjectProcessConfiguration, CategoryConfiguration>) (s => (CategoryConfiguration) s.TaskBacklog), (Action<ProjectProcessConfiguration, CategoryConfiguration>) ((s, c) => s.TaskBacklog = (BacklogCategoryConfiguration) c), false);
      this.EnsureProcessSettingsCategoryNode(context, (Func<ProjectProcessConfiguration, CategoryConfiguration>) (s => s.BugWorkItems), (Action<ProjectProcessConfiguration, CategoryConfiguration>) ((s, c) => s.BugWorkItems = c), true);
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      this.EnsureCategory(context, processConfiguration.TaskBacklog.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.FailIfMissingDoNotOverwriteIfExists);
      foreach (string workItemTypeName in context.GetWorkItemTypeCategory(processConfiguration.TaskBacklog.CategoryReferenceName).WorkItemTypeNames)
      {
        this.AddActionToType(context, workItemTypeName, this.StartWorkActionName, false);
        this.AddActionToType(context, workItemTypeName, this.StopWorkActionName, false);
      }
      if (processConfiguration.BugWorkItems == null)
        return;
      this.EnsureCategory(context, processConfiguration.BugWorkItems.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.FailIfMissingDoNotOverwriteIfExists);
      foreach (string workItemTypeName in context.GetWorkItemTypeCategory(processConfiguration.BugWorkItems.CategoryReferenceName).WorkItemTypeNames)
      {
        this.AddActionToType(context, workItemTypeName, this.StartWorkActionName, true);
        this.AddActionToType(context, workItemTypeName, this.StopWorkActionName, true);
      }
    }

    private void AddActionToType(
      IProjectProvisioningContext context,
      string typeName,
      string actionName,
      bool optional)
    {
      WorkItemTypeMetadata workItemType1 = context.GetWorkItemType(typeName);
      if (workItemType1.GetAction(actionName) != null)
        return;
      WorkItemTypeMetadata workItemType2 = context.ProcessTemplate.GetWorkItemType(typeName);
      if (workItemType2 != null)
      {
        WorkItemTypeAction action;
        if ((action = workItemType2.GetAction(actionName)) != null)
        {
          try
          {
            workItemType1.AddAction(action);
            return;
          }
          catch (LegacyValidationException ex)
          {
            context.ReportIssue(new ProjectProvisioningIssue(ex.Message, IssueLevel.Warning));
            return;
          }
        }
      }
      if (optional)
        return;
      context.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionWarningActionNotInProcessTemplate, (object) actionName, (object) typeName), IssueLevel.Warning));
    }
  }
}
