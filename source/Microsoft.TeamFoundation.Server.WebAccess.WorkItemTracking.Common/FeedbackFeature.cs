// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FeedbackFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class FeedbackFeature : ProjectFeatureBase
  {
    public FeedbackFeature()
      : base(Resources.FeedbackFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata)
    {
      ProjectProcessConfiguration processConfiguration = projectMetadata.GetProcessConfiguration();
      WorkItemTypeCategory itemTypeCategory1 = projectMetadata.GetWorkItemTypeCategory(WitCategoryRefName.FeedbackRequestCategory);
      WorkItemTypeCategory itemTypeCategory2 = projectMetadata.GetWorkItemTypeCategory(WitCategoryRefName.FeedbackResponseCategory);
      if (processConfiguration != null && itemTypeCategory1 != null && itemTypeCategory2 != null)
        return ProjectFeatureState.FullyConfigured;
      return itemTypeCategory1 == null && itemTypeCategory2 == null ? ProjectFeatureState.NotConfigured : ProjectFeatureState.PartiallyConfigured;
    }

    public override void Process(IProjectProvisioningContext context)
    {
      this.EnsureProcessConfigurationSettings(context);
      this.EnsureProcessSettingsCategoryNode(context, (Func<ProjectProcessConfiguration, CategoryConfiguration>) (s => s.FeedbackRequestWorkItems), (Action<ProjectProcessConfiguration, CategoryConfiguration>) ((s, c) => s.FeedbackRequestWorkItems = c), false);
      this.EnsureProcessSettingsCategoryNode(context, (Func<ProjectProcessConfiguration, CategoryConfiguration>) (s => s.FeedbackResponseWorkItems), (Action<ProjectProcessConfiguration, CategoryConfiguration>) ((s, c) => s.FeedbackResponseWorkItems = c), false);
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      this.EnsureCategory(context, processConfiguration.FeedbackRequestWorkItems.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
      this.EnsureCategory(context, processConfiguration.FeedbackResponseWorkItems.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
    }
  }
}
