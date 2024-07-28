// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.DashboardsViewHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Dashboards.Telemetry;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  internal sealed class DashboardsViewHelper
  {
    internal static DashboardDefaultViewModel GetViewModel(TfsWebContext tfsWebContext)
    {
      IVssRequestContext tfsRequestContext = tfsWebContext.TfsRequestContext;
      tfsRequestContext.ServiceName = "Dashboards";
      using (TelemetryCollector.TraceMonitor(tfsRequestContext, 10017600, "DashboardWeb", "DashboardViewHelper.GetViewModel"))
      {
        DashboardDefaultViewModel viewModel = new DashboardDefaultViewModel();
        IDashboardService service = tfsRequestContext.GetService<IDashboardService>();
        Guid id1 = tfsWebContext.Project.Id;
        if (tfsWebContext.Team == null || tfsWebContext.Team.Identity == null)
          throw new DefaultTeamNotFoundException(id1.ToString());
        Guid id2 = tfsWebContext.Team.Id;
        IDashboardConsumer dashboardConsumer = DashboardConsumerFactory.CreateDashboardConsumer(tfsRequestContext, id1, new Guid?(id2));
        IEnumerable<DashboardGroupEntry> dashboardEntries = service.GetDashboardGroup(tfsRequestContext, dashboardConsumer).DashboardEntries;
        Guid currentDashboard = DashboardsViewHelper.getCurrentDashboard(tfsWebContext);
        if (!Guid.Empty.Equals(currentDashboard) && dashboardEntries != null && !dashboardEntries.Any<DashboardGroupEntry>((Func<DashboardGroupEntry, bool>) (entry => entry != null && currentDashboard.Equals(entry.Id.GetValueOrDefault()))))
          throw new DashboardDoesNotExistException(currentDashboard);
        Guid id3 = !Guid.Empty.Equals(currentDashboard) ? currentDashboard : dashboardEntries.FirstOrDefault<DashboardGroupEntry>().Id.Value;
        Dashboard dashboard = service.GetDashboard(tfsRequestContext, dashboardConsumer, id3);
        using (TelemetryCollector.TraceMonitor(tfsRequestContext, 10017600, "DashboardWeb", "DashboardsViewHelper.PackDetails"))
        {
          viewModel.DefaultDashboardId = id3.ToString();
          viewModel.DefaultDashboardWidgets = dashboard;
          viewModel.MaxDashboardPerGroup = DashboardSettings.GetMaxDashboardsPerGroup(tfsRequestContext);
          viewModel.MaxWidgetsPerDashboard = DashboardSettings.GetMaxWidgetsPerDashboard(tfsRequestContext);
          viewModel.IsStakeholder = tfsRequestContext.IsStakeholder();
          string[] contributionsForDashboard = DashboardsViewHelper.getWidgetContributionsForDashboard(viewModel.DefaultDashboardWidgets);
          DashboardsViewHelper.AddWidgetContributionsToPageContext(tfsWebContext, contributionsForDashboard);
          DashboardsViewHelper.PublishDashboardUrlCIData(tfsWebContext);
        }
        return viewModel;
      }
    }

    private static void AddWidgetContributionsToPageContext(
      TfsWebContext tfsWebContext,
      string[] widgetContributions)
    {
      try
      {
        WebContextFactory.GetPageContext(tfsWebContext.RequestContext).AddContributions((IEnumerable<string>) widgetContributions);
      }
      catch (Exception ex)
      {
        tfsWebContext.TfsRequestContext.TraceException(0, "WebAccess", TfsTraceLayers.Content, ex);
      }
    }

    private static void PublishDashboardUrlCIData(TfsWebContext tfsWebContext)
    {
      Uri urlReferrer = tfsWebContext.RequestContext.HttpContext.Request.UrlReferrer;
      Guid currentDashboard = DashboardsViewHelper.getCurrentDashboard(tfsWebContext);
      if (!(urlReferrer == (Uri) null) && (!(urlReferrer != (Uri) null) || urlReferrer.AbsoluteUri.Contains(tfsWebContext.Host.Authority)) || !(currentDashboard != Guid.Empty))
        return;
      CustomerIntelligenceService service = tfsWebContext.TfsRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(CustomerIntelligenceProperty.ActivityId, tfsWebContext.TfsRequestContext.ActivityId.ToString());
      intelligenceData.Add("Referrer", urlReferrer == (Uri) null ? "-" : urlReferrer.AbsoluteUri);
      intelligenceData.Add("DashboardId", currentDashboard.ToString());
      IVssRequestContext tfsRequestContext = tfsWebContext.TfsRequestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(tfsRequestContext, "Dashboard", "DashboardViewDirectLink", properties);
    }

    private static bool IsNotificationDismissed(
      IVssRequestContext requestContext,
      string notificationId)
    {
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(requestContext))
        return userSettingsHive.ReadSetting<bool>("/notification/dismiss/" + notificationId, false);
    }

    internal static Guid getCurrentDashboard(TfsWebContext tfsWebContext)
    {
      Guid result;
      Guid.TryParse(tfsWebContext.RequestContext.HttpContext.Request.QueryString["activeDashboardId"], out result);
      return result;
    }

    private static string[] getWidgetContributionsForDashboard(Dashboard dashboard)
    {
      HashSet<string> source = new HashSet<string>();
      if (dashboard.Widgets != null)
      {
        foreach (Widget widget in dashboard.Widgets)
        {
          if (!string.IsNullOrEmpty(widget.ContributionId) && !source.Contains(widget.ContributionId))
            source.Add(widget.ContributionId);
        }
      }
      return source.ToArray<string>();
    }
  }
}
