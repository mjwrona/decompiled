// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.CopyWidgetAdapterFactory
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.Services.CopyWidgetAdapters.QueryAdapters;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class CopyWidgetAdapterFactory
  {
    private Dictionary<string, IWidgetAdapter> m_ContributionIdToAdapterMap;

    public CopyWidgetAdapterFactory()
    {
      IWidgetAdapter widgetAdapter1 = (IWidgetAdapter) new TeamBasedWidgetAdapter();
      IWidgetAdapter widgetAdapter2 = (IWidgetAdapter) new MarkdownWidgetAdapter();
      IWidgetAdapter widgetAdapter3 = (IWidgetAdapter) new ProjectBasedWidgetAdapter();
      IWidgetAdapter widgetAdapter4 = (IWidgetAdapter) new WidgetAdapterBase();
      IWidgetAdapter widgetAdapter5 = (IWidgetAdapter) new CycleOrLeadTimeWidgetAdapter();
      IWidgetAdapter widgetAdapter6 = (IWidgetAdapter) new VelocityWidgetAdapter();
      IWidgetAdapter widgetAdapter7 = (IWidgetAdapter) new SprintOverviewWidgetAdapter();
      IWidgetAdapter widgetAdapter8 = (IWidgetAdapter) new LegacySprintBurndownWidgetAdapter();
      IWidgetAdapter widgetAdapter9 = (IWidgetAdapter) new SprintBurndownWidgetAdapter();
      IWidgetAdapter widgetAdapter10 = (IWidgetAdapter) new TestResultsTrendsWidgetAdapter();
      IWidgetAdapter widgetAdapter11 = (IWidgetAdapter) new TestResultsTrendsAdvancedWidgetAdapter();
      TestPlansChartWidgetAdapter chartWidgetAdapter = new TestPlansChartWidgetAdapter();
      IWidgetAdapter widgetAdapter12 = (IWidgetAdapter) new QueryResultsWidgetAdapter();
      IWidgetAdapter widgetAdapter13 = (IWidgetAdapter) new QueryTileWidgetAdapter();
      IWidgetAdapter widgetAdapter14 = (IWidgetAdapter) new WitChartWidgetAdapter();
      IWidgetAdapter widgetAdapter15 = (IWidgetAdapter) new TcmWidgetAdapter();
      IWidgetAdapter widgetAdapter16 = (IWidgetAdapter) new BurndownWidgetAdapter();
      this.m_ContributionIdToAdapterMap = new Dictionary<string, IWidgetAdapter>()
      {
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.SprintCapacityWidget",
          widgetAdapter1
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.TeamMembersWidget",
          widgetAdapter1
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.MarkdownWidget",
          widgetAdapter2
        },
        {
          "ms.vss-mywork-web.Microsoft.VisualStudioOnline.MyWork.PullRequestWidget",
          widgetAdapter3
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.CodeScalarWidget",
          widgetAdapter3
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.BuildHistogramWidget",
          widgetAdapter3
        },
        {
          "ms.vss-releaseManagement-web.rm-deployment-status-widget",
          widgetAdapter3
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.BuildChartWidget",
          widgetAdapter3
        },
        {
          "ms.vss-releaseManagement-web.release-definition-summary-widget",
          widgetAdapter3
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.AssignedToMeWidget",
          widgetAdapter4
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.IFrameWidget",
          widgetAdapter4
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.NewWorkItemWidget",
          widgetAdapter4
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.OtherLinksWidget",
          widgetAdapter4
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.VSLinksWidget",
          widgetAdapter4
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.HowToLinksWidget",
          widgetAdapter4
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.WorkLinksWidget",
          widgetAdapter4
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.BurndownWidget",
          widgetAdapter16
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.BurnupWidget",
          widgetAdapter16
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.CycleTimeWidget",
          widgetAdapter5
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.LeadTimeWidget",
          widgetAdapter5
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.VelocityWidget",
          widgetAdapter6
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.SprintOverviewWidget",
          widgetAdapter7
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.SprintBurndownWidget",
          widgetAdapter8
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.AnalyticsSprintBurndownWidget",
          widgetAdapter9
        },
        {
          "ms.vss-test-web.Microsoft.VisualStudioTeamServices.Dashboards.TestResultsTrendWidget",
          widgetAdapter10
        },
        {
          "ms.vss-test-web.Microsoft.VisualStudioTeamServices.TestManagement.AnalyticsTestTrendWidget",
          widgetAdapter11
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.TcmChartWidget",
          widgetAdapter15
        },
        {
          "ms.vss-mywork-web.Microsoft.VisualStudioOnline.MyWork.WitViewWidget",
          widgetAdapter12
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.QueryScalarWidget",
          widgetAdapter13
        },
        {
          "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.WitChartWidget",
          widgetAdapter14
        }
      };
    }

    public IWidgetAdapter GetWidgetAdapter(string WidgetContributionId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(WidgetContributionId, nameof (WidgetContributionId), "Dashboards");
      return !this.m_ContributionIdToAdapterMap.ContainsKey(WidgetContributionId) ? (IWidgetAdapter) new WidgetAdapterBase() : this.m_ContributionIdToAdapterMap[WidgetContributionId];
    }
  }
}
