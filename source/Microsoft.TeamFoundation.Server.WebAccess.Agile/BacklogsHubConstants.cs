// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BacklogsHubConstants
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [GenerateAllConstants(null)]
  public class BacklogsHubConstants
  {
    public const string BacklogPivot = "backlog";
    public const string AnalyticsPivot = "analytics";
    public const string InProgressParameter = "inProgress";
    public const string CompletedChildItemsParameter = "completedChildItems";
    public const string InProgressSetting = "Agile/BacklogsHub/InProgressFilter";
    public const string CompletedChildItemsSetting = "Agile/BacklogsHub/CompletedChildItemsFilter";
    public const string ShowParentsQueryParameter = "showParents";
    public const string RightPaneQueryParameter = "rightPane";
    public const string ShowParentsSetting = "Agile/BacklogsHub/ShowParentsFilter";
    public const string ForecastingParameter = "forecasting";
    public const string ForecastingSetting = "Agile/BacklogsHub/Forecasting";
    public const string PaneParameter = "pane";
    public const string PaneSetting = "Agile/BacklogsHub/Pane";
    public const string MruBacklogKey = "Agile/BacklogsHub/BacklogId";
    public const string HUB_CONTRIBUTION_ID = "ms.vss-work-web.backlogs-hub";
    public const string PRODUCTBACKLOG_DATAPROVIDER_ID = "ms.vss-work-web.backlogs-hub-backlog-data-provider";
    public const string HEADER_DATAPROVIDER_ID = "ms.vss-work-web.backlogs-hub-content-header-data-provider";
    public static readonly string HUB_NAME = "Backlogs";
    public const string ContentRouteContributionId = "ms.vss-work-web.backlogs-content-route";
    public const string DirectoryRouteContributionId = "ms.vss-work-web.backlogs-directory-route";
    public const string NewContentRouteContributionId = "ms.vss-work-web.new-backlogs-content-route";
    public const string NewDirectoryRouteContributionId = "ms.vss-work-web.new-backlogs-directory-route";
    public const string LegacyBacklogsHubContributionRouteId = "ms.vss-work-web.agile-route";
    public const string DirectoryViewName = "directory";
    public const string WorkItemIds = "workItemIds";
    public const string ColumnOptionsFormat = "Agile/BacklogsHub/ColumnOptions/{0}";
    public const string CFDReportName = "cfd";
    public const string VelocityReportName = "velocity";
    public const string NewBoardsHubFeature = "ms.vss-work-web.new-boards-hub-feature";
  }
}
