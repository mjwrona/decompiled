// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CodeReviewFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class CodeReviewFeature : ProjectFeatureBase
  {
    public CodeReviewFeature()
      : base(Resources.CodeReviewFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata)
    {
      WorkItemTypeCategory itemTypeCategory1 = projectMetadata.GetWorkItemTypeCategory("Microsoft.CodeReviewRequestCategory");
      WorkItemTypeCategory itemTypeCategory2 = projectMetadata.GetWorkItemTypeCategory("Microsoft.CodeReviewResponseCategory");
      if (itemTypeCategory1 != null && itemTypeCategory2 != null)
        return ProjectFeatureState.FullyConfigured;
      return itemTypeCategory1 == null && itemTypeCategory2 == null ? ProjectFeatureState.NotConfigured : ProjectFeatureState.PartiallyConfigured;
    }

    public override void Process(IProjectProvisioningContext context)
    {
      this.EnsureCategory(context, "Microsoft.CodeReviewRequestCategory", ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
      this.EnsureCategory(context, "Microsoft.CodeReviewResponseCategory", ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
    }
  }
}
