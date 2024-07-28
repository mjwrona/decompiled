// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.PortfolioBacklogsFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class PortfolioBacklogsFeature : ProjectFeatureBase
  {
    public PortfolioBacklogsFeature()
      : base(Resources.PortfolioBacklogsFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata)
    {
      ProjectProcessConfiguration processConfiguration = projectMetadata.GetProcessConfiguration();
      return processConfiguration != null && ((IEnumerable<BacklogCategoryConfiguration>) processConfiguration.PortfolioBacklogs).Any<BacklogCategoryConfiguration>() ? ProjectFeatureState.FullyConfigured : ProjectFeatureState.NotConfigured;
    }

    public override void Process(IProjectProvisioningContext context)
    {
      this.EnsureProcessConfigurationSettings(context);
      ProjectProcessConfiguration processConfiguration1 = context.ProcessTemplate.GetProcessConfiguration();
      ProjectProcessConfiguration processConfiguration2 = context.GetProcessConfiguration();
      if (processConfiguration1.PortfolioBacklogs == null || !((IEnumerable<BacklogCategoryConfiguration>) processConfiguration1.PortfolioBacklogs).Any<BacklogCategoryConfiguration>())
        throw new LegacyValidationException(Resources.ProcessSettingsInvalid);
      this.EnsurePortfolioBacklogs(context);
      foreach (BacklogCategoryConfiguration portfolioBacklog in processConfiguration1.PortfolioBacklogs)
        this.EnsureCategory(context, portfolioBacklog.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);
      if (!TFStringComparer.WorkItemCategoryName.Equals(processConfiguration1.RequirementBacklog.CategoryReferenceName, processConfiguration2.RequirementBacklog.CategoryReferenceName))
        throw new LegacyValidationException(Resources.ProcessSettingsInvalid);
      this.EnsureCategory(context, processConfiguration1.RequirementBacklog.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.Merge, ProjectFeatureBase.EnsureWorkItemMode.FailIfMissingDoNotOverwriteIfExists);
      this.EnsureWorkItemTypeColors(context);
      if (string.IsNullOrEmpty(processConfiguration2.TaskBacklog.PluralName))
        processConfiguration2.TaskBacklog.PluralName = processConfiguration1.TaskBacklog.PluralName;
      if (processConfiguration2.TaskBacklog.AddPanel != null && processConfiguration2.TaskBacklog.AddPanel.Fields != null && processConfiguration2.TaskBacklog.AddPanel.Fields.Length != 0)
        return;
      processConfiguration2.TaskBacklog.AddPanel = processConfiguration1.TaskBacklog.AddPanel;
    }

    private void EnsurePortfolioBacklogs(IProjectProvisioningContext context)
    {
      ProjectProcessConfiguration processConfiguration1 = context.GetProcessConfiguration();
      ProjectProcessConfiguration processConfiguration2 = context.ProcessTemplate.GetProcessConfiguration();
      if (processConfiguration1.PortfolioBacklogs != null && processConfiguration1.PortfolioBacklogs.Length != 0)
        return;
      processConfiguration1.PortfolioBacklogs = processConfiguration2.PortfolioBacklogs;
    }

    private void EnsureWorkItemTypeColors(IProjectProvisioningContext context)
    {
      ProjectProcessConfiguration processConfiguration1 = context.GetProcessConfiguration();
      ProjectProcessConfiguration processConfiguration2 = context.ProcessTemplate.GetProcessConfiguration();
      if (processConfiguration2.WorkItemColors == null)
        return;
      if (processConfiguration1.WorkItemColors == null)
      {
        processConfiguration1.WorkItemColors = processConfiguration2.WorkItemColors;
      }
      else
      {
        IEnumerable<string> specifiedTypes = ((IEnumerable<WorkItemColor>) processConfiguration1.WorkItemColors).Select<WorkItemColor, string>((Func<WorkItemColor, string>) (c => c.WorkItemTypeName));
        processConfiguration1.WorkItemColors = ((IEnumerable<WorkItemColor>) processConfiguration1.WorkItemColors).Union<WorkItemColor>(((IEnumerable<WorkItemColor>) processConfiguration2.WorkItemColors).Where<WorkItemColor>((Func<WorkItemColor, bool>) (c => !specifiedTypes.Contains<string>(c.WorkItemTypeName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)))).ToArray<WorkItemColor>();
      }
      processConfiguration1.WorkItemColors = ((IEnumerable<WorkItemColor>) processConfiguration1.WorkItemColors).Where<WorkItemColor>((Func<WorkItemColor, bool>) (colorEntry => context.GetWorkItemType(colorEntry.WorkItemTypeName) != null)).ToArray<WorkItemColor>();
    }
  }
}
