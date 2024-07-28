// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ReleasePipelineFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class ReleasePipelineFeature : ProjectFeatureBase
  {
    public ReleasePipelineFeature()
      : base(Resources.ReleasePipelineFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata)
    {
      ProjectProcessConfiguration processConfiguration = projectMetadata.GetProcessConfiguration();
      if (processConfiguration == null)
        return ProjectFeatureState.NotConfigured;
      bool flag1 = processConfiguration.ReleaseWorkItems != null && projectMetadata.GetWorkItemTypeCategory(processConfiguration.ReleaseWorkItems.CategoryReferenceName) != null;
      bool flag2 = processConfiguration.ReleaseStageWorkItems != null && projectMetadata.GetWorkItemTypeCategory(processConfiguration.ReleaseStageWorkItems.CategoryReferenceName) != null;
      bool flag3 = processConfiguration.StageSignoffTaskWorkItems != null && projectMetadata.GetWorkItemTypeCategory(processConfiguration.StageSignoffTaskWorkItems.CategoryReferenceName) != null;
      if (flag1 && flag2 && flag3)
        return ProjectFeatureState.FullyConfigured;
      return !flag1 && !flag2 && !flag3 ? ProjectFeatureState.NotConfigured : ProjectFeatureState.PartiallyConfigured;
    }

    public override void Process(IProjectProvisioningContext context)
    {
      this.EnsureProcessConfigurationSettings(context);
      this.EnsureProcessSettingsCategoryNode(context, (Func<ProjectProcessConfiguration, CategoryConfiguration>) (s => s.ReleaseWorkItems), (Action<ProjectProcessConfiguration, CategoryConfiguration>) ((s, c) => s.ReleaseWorkItems = c), false);
      this.EnsureProcessSettingsCategoryNode(context, (Func<ProjectProcessConfiguration, CategoryConfiguration>) (s => s.ReleaseStageWorkItems), (Action<ProjectProcessConfiguration, CategoryConfiguration>) ((s, c) => s.ReleaseStageWorkItems = c), false);
      this.EnsureProcessSettingsCategoryNode(context, (Func<ProjectProcessConfiguration, CategoryConfiguration>) (s => s.StageSignoffTaskWorkItems), (Action<ProjectProcessConfiguration, CategoryConfiguration>) ((s, c) => s.StageSignoffTaskWorkItems = c), false);
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      this.EnsureCategory(context, processConfiguration.ReleaseWorkItems.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
      this.EnsureCategory(context, processConfiguration.ReleaseStageWorkItems.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
      this.EnsureCategory(context, processConfiguration.StageSignoffTaskWorkItems.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
      this.EnsureCategory(context, "Microsoft.HiddenCategory", ProjectFeatureBase.EnsureCategoryMode.Merge, ProjectFeatureBase.EnsureWorkItemMode.RemoveFromCategoryIfMissingDoNotOverwriteIfExists);
    }
  }
}
