// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Controllers.Dashboardv2Controller
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Dashboards.Model;
using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Dashboards.Controllers
{
  [VersionedApiControllerCustomName("Dashboard", "Dashboards", 2)]
  [ControllerApiVersion(3.0)]
  [ResolveTfsProjectAndTeamFilter(RequireExplicitTeam = true)]
  public class Dashboardv2Controller : DashboardApiv2ControllerBase
  {
    private IDashboardService m_DashboardService;

    public override string TraceArea => "Dashboards";

    public IDashboardService DashboardService
    {
      get
      {
        if (this.m_DashboardService == null)
          this.m_DashboardService = this.TfsRequestContext.GetService<IDashboardService>();
        return this.m_DashboardService;
      }
    }

    [ClientExample("GET_dashboards.json", null, null, null)]
    [TraceFilter(10017010, 10017019)]
    [HttpGet]
    [ClientResourceOperation(ClientResourceOperationName.List)]
    [PublicProjectRequestRestrictions]
    public DashboardGroup GetDashboards() => this.CreateDashboardGroupResponse(this.TeamId, this.DashboardService.GetDashboardGroup(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId))));

    [ClientExample("PUT__dashboard_groups__groupId__dashboards__dashboardId__.json", null, null, null)]
    [TraceFilter(10017000, 10017091)]
    [HttpPut]
    public DashboardGroup ReplaceDashboards(DashboardGroup group)
    {
      ArgumentUtility.CheckForNull<DashboardGroup>(group, nameof (group), "Dashboards");
      return this.CreateDashboardGroupResponse(this.TeamId, this.DashboardService.UpdateDashboards(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), group));
    }

    [ClientExample("POST_dashboard.json", null, null, null)]
    [TraceFilter(10017200, 10017209)]
    [HttpPost]
    public Dashboard CreateDashboard(Dashboard dashboard)
    {
      ArgumentUtility.CheckForNull<Dashboard>(dashboard, nameof (dashboard), "Dashboards");
      Dashboard response = this.DashboardService.AddDashboard(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboard);
      response.AddLinks(this.TfsRequestContext, this.Url, this.ProjectId);
      return response;
    }

    [ClientExample("GET_dashboard.json", null, null, null)]
    [TraceFilter(10017230, 10017239)]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public Dashboard GetDashboard(Guid dashboardId)
    {
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      Dashboard dashboard = this.DashboardService.GetDashboard(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboardId);
      if (dashboard != null && dashboard.Widgets != null)
      {
        List<Widget> list = dashboard.Widgets.ToList<Widget>();
        this.PopulateMetadata(list);
        dashboard.Widgets = (IEnumerable<Widget>) list;
      }
      dashboard.AddLinks(this.TfsRequestContext, this.Url, this.ProjectId);
      return dashboard;
    }

    [TraceFilter(10017210, 10017219)]
    [HttpPut]
    public Dashboard ReplaceDashboard(Guid dashboardId, Dashboard dashboard)
    {
      ArgumentUtility.CheckForNull<Dashboard>(dashboard, nameof (dashboard), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      IDashboardConsumer dashboardConsumer = DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId));
      dashboard.Id = new Guid?(dashboardId);
      Dashboard response = this.DashboardService.UpdateDashboard(this.TfsRequestContext, dashboardConsumer, dashboard);
      response.AddLinks(this.TfsRequestContext, this.Url, this.ProjectId);
      return response;
    }

    [ClientExample("DELETE__dashboard_groups__groupId__dashboards__dashboardId_.json", null, null, null)]
    [TraceFilter(10017220, 10017229)]
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteDashboard(Guid dashboardId)
    {
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      this.DashboardService.DeleteDashboard(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboardId);
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }
  }
}
