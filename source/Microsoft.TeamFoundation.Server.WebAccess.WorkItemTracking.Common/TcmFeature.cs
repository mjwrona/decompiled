// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TcmFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class TcmFeature : ProjectFeatureBase
  {
    public TcmFeature()
      : base(Resources.TestPlanAndSuiteCustomizationFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata)
    {
      WorkItemTypeCategory itemTypeCategory1 = projectMetadata.GetWorkItemTypeCategory(WitCategoryRefName.TestPlan);
      WorkItemTypeCategory itemTypeCategory2 = projectMetadata.GetWorkItemTypeCategory(WitCategoryRefName.TestSuite);
      return itemTypeCategory1 != null && itemTypeCategory2 != null ? ProjectFeatureState.FullyConfigured : ProjectFeatureState.NotConfigured;
    }

    public override void Process(IProjectProvisioningContext context)
    {
      this.EnsureProcessConfigurationSettings(context);
      this.EnsureCategory(context, "Microsoft.TestPlanCategory", ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
      this.EnsureCategory(context, "Microsoft.TestSuiteCategory", ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
    }
  }
}
