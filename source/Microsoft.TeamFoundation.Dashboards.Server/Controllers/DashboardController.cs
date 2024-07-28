// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Controllers.DashboardController
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Dashboards.Controllers
{
  [VersionedApiControllerCustomName("Dashboard", "Dashboards", 1)]
  public class DashboardController : DashboardApiControllerBase
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

    [TraceFilter(10017200, 10017209)]
    [HttpPost]
    public DashboardGroupEntryResponse CreateDashboard(Guid groupId, DashboardGroupEntry entry)
    {
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId), "Dashboards");
      ArgumentUtility.CheckForNull<DashboardGroupEntry>(entry, nameof (entry), "Dashboards");
      Dashboard entry1 = this.DashboardService.AddDashboard(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(groupId)), (Dashboard) entry);
      return this.CreateDashboardEntryResponse(groupId, entry1);
    }

    [TraceFilter(10017230, 10017239)]
    [HttpGet]
    public DashboardResponse GetDashboard(Guid groupId, Guid dashboardId)
    {
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      Dashboard dashboard = this.DashboardService.GetDashboard(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(groupId)), dashboardId);
      return this.CreateDashboardResponse(groupId, dashboard);
    }

    [TraceFilter(10017210, 10017219)]
    [HttpPut]
    public DashboardResponse ReplaceDashboard(Guid groupId, Guid dashboardId, Dashboard dashboard)
    {
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForNull<Dashboard>(dashboard, nameof (dashboard), "Dashboards");
      Dashboard entry = this.DashboardService.ReplaceDashboardWidgets(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(groupId)), dashboard);
      return this.CreateDashboardResponse(groupId, entry);
    }

    [TraceFilter(10017220, 10017229)]
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteDashboard(Guid groupId, Guid dashboardId)
    {
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      this.DashboardService.DeleteDashboard(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(groupId)), dashboardId);
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }
  }
}
