// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Controllers.WidgetController
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Dashboards.Controllers
{
  [VersionedApiControllerCustomName("Dashboard", "Widgets", 1)]
  public class WidgetController : DashboardApiControllerBase
  {
    private IDashboardService m_DashboardService;

    public override string TraceArea => "Widgets";

    public IDashboardService DashboardService
    {
      get
      {
        if (this.m_DashboardService == null)
          this.m_DashboardService = this.TfsRequestContext.GetService<IDashboardService>();
        return this.m_DashboardService;
      }
    }

    [TraceFilter(10017300, 10017309)]
    [HttpPost]
    public WidgetResponse CreateWidget(Guid groupId, Guid dashboardId, Widget widget)
    {
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForNull<Widget>(widget, nameof (widget), "Dashboards");
      Widget widget1 = this.DashboardService.AddWidget(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(groupId)), dashboardId, widget);
      return this.CreateWidgetResponse(groupId, dashboardId, widget1);
    }

    [TraceFilter(10017330, 10017339)]
    [HttpGet]
    public WidgetResponse GetWidget(Guid groupId, Guid dashboardId, Guid widgetId)
    {
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(widgetId, nameof (widgetId), "Dashboards");
      Widget widgetById = this.DashboardService.GetWidgetById(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(groupId)), dashboardId, widgetId);
      return this.CreateWidgetResponse(groupId, dashboardId, widgetById);
    }

    [TraceFilter(10017310, 10017319)]
    [HttpPatch]
    public WidgetResponse UpdateWidget(
      Guid groupId,
      Guid dashboardId,
      Guid widgetId,
      Widget widget)
    {
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForNull<Widget>(widget, nameof (widget), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(widgetId, nameof (widgetId), "Dashboards");
      widget.Id = new Guid?(widgetId);
      Widget widget1 = this.DashboardService.UpdateWidget(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(groupId)), dashboardId, widget);
      return this.CreateWidgetResponse(groupId, dashboardId, widget1);
    }
  }
}
