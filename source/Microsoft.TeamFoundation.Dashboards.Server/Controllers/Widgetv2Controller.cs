// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Controllers.Widgetv2Controller
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
  [VersionedApiControllerCustomName("Dashboard", "Widgets", 2)]
  [ControllerApiVersion(3.0)]
  [ResolveTfsProjectAndTeamFilter(RequireExplicitTeam = true)]
  public class Widgetv2Controller : DashboardApiv2ControllerBase
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

    [ClientExample("POST_widgets.json", null, null, null)]
    [TraceFilter(10017300, 10017309)]
    [HttpPost]
    public Widget CreateWidget(Guid dashboardId, Widget widget)
    {
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForNull<Widget>(widget, nameof (widget), "Dashboards");
      Widget response = this.DashboardService.AddWidget(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboardId, widget);
      response.AddLinks(this.TfsRequestContext, this.Url, this.ProjectId, dashboardId);
      this.PopulateMetadata(new List<Widget>() { response });
      return response;
    }

    [ClientExample("GET_widgets_widgetid.json", null, null, null)]
    [TraceFilter(10017330, 10017339)]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public Widget GetWidget(Guid dashboardId, Guid widgetId)
    {
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(widgetId, nameof (widgetId), "Dashboards");
      Widget widgetById = this.DashboardService.GetWidgetById(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboardId, widgetId);
      widgetById.AddLinks(this.TfsRequestContext, this.Url, this.ProjectId, dashboardId);
      return widgetById;
    }

    [ClientExample("PATCH_widgets_widgetid.json", null, null, null)]
    [TraceFilter(10017310, 10017319)]
    [HttpPatch]
    public Widget UpdateWidget(Guid dashboardId, Guid widgetId, Widget widget)
    {
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForNull<Widget>(widget, nameof (widget), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(widgetId, nameof (widgetId), "Dashboards");
      widget.Id = new Guid?(widgetId);
      Widget response = this.DashboardService.UpdateWidget(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboardId, widget);
      response.AddLinks(this.TfsRequestContext, this.Url, this.ProjectId, dashboardId);
      return response;
    }

    [ClientExample("PUT_widgets_widgetid.json", null, null, null)]
    [TraceFilter(10017340, 10017349)]
    [HttpPut]
    public Widget ReplaceWidget(Guid dashboardId, Guid widgetId, Widget widget)
    {
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForNull<Widget>(widget, nameof (widget), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(widgetId, nameof (widgetId), "Dashboards");
      widget.Id = new Guid?(widgetId);
      Widget response = this.DashboardService.ReplaceWidget(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboardId, widget);
      response.AddLinks(this.TfsRequestContext, this.Url, this.ProjectId, dashboardId);
      return response;
    }

    [ClientExample("DELETE__dashboard_groups__groupId__dashboards__dashboardId__widgets__widgetId_.json", null, null, null)]
    [TraceFilter(10017320, 10017329)]
    [HttpDelete]
    public Dashboard DeleteWidget(Guid dashboardId, Guid widgetId)
    {
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForEmptyGuid(widgetId, nameof (widgetId), "Dashboards");
      Dashboard response = this.DashboardService.DeleteWidget(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboardId, widgetId);
      response.AddLinks(this.TfsRequestContext, this.Url, this.ProjectId);
      return response;
    }

    [ClientExample("GET_widgets.json", null, null, null)]
    [TraceFilter(10017390, 10017399)]
    [ClientResponseType(typeof (WidgetsVersionedList), null, null)]
    [ClientHeaderParameter("ETag", typeof (string), "eTag", "Dashboard Widgets Version", true, false)]
    [HttpGet]
    public HttpResponseMessage GetWidgets(Guid dashboardId)
    {
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      return this.generateWidgetsResponse(this.DashboardService.GetDashboard(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboardId));
    }

    [ClientExample("PUT_widgets.json", null, null, null)]
    [TraceFilter(10017350, 10017359)]
    [ClientResponseType(typeof (WidgetsVersionedList), null, null)]
    [ClientHeaderParameter("ETag", typeof (string), "eTag", "Dashboard Widgets Version", true, false)]
    [HttpPut]
    public HttpResponseMessage ReplaceWidgets(Guid dashboardId, [FromBody] IEnumerable<Widget> widgets)
    {
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForNull<IEnumerable<Widget>>(widgets, nameof (widgets), "Dashboards");
      Dashboard dashboard = new Dashboard()
      {
        Id = new Guid?(dashboardId),
        ETag = this.getDashboardETag(),
        Widgets = widgets
      };
      return this.generateWidgetsResponse(this.DashboardService.ReplaceDashboardWidgets(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboard));
    }

    [ClientExample("PATCH_widgets.json", null, null, null)]
    [TraceFilter(10017420, 10017429)]
    [ClientResponseType(typeof (WidgetsVersionedList), null, null)]
    [ClientHeaderParameter("ETag", typeof (string), "eTag", "Dashboard Widgets Version", true, false)]
    [HttpPatch]
    public HttpResponseMessage UpdateWidgets(Guid dashboardId, [FromBody] IEnumerable<Widget> widgets)
    {
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId), "Dashboards");
      ArgumentUtility.CheckForNull<IEnumerable<Widget>>(widgets, nameof (widgets), "Dashboards");
      Dashboard dashboard = new Dashboard()
      {
        Id = new Guid?(dashboardId),
        ETag = this.getDashboardETag(),
        Widgets = widgets
      };
      return this.generateWidgetsResponse(this.DashboardService.UpdateDashboardWidgets(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(this.TeamId)), dashboard));
    }

    private string getDashboardETag()
    {
      IEnumerable<string> values;
      int num = this.Request.Headers.TryGetValues("ETag", out values) ? 1 : 0;
      string dashboardEtag = (string) null;
      if (num != 0)
      {
        string str = values.FirstOrDefault<string>();
        dashboardEtag = !(str == "null") ? str.Replace("\"", "") : (string) null;
      }
      return dashboardEtag;
    }

    private HttpResponseMessage generateWidgetsResponse(Dashboard result)
    {
      List<Widget> widgets = new List<Widget>();
      if (result.Widgets != null)
      {
        widgets = result.Widgets.ToList<Widget>();
        this.PopulateMetadata(widgets);
      }
      HttpResponseMessage response = this.Request.CreateResponse<List<Widget>>(HttpStatusCode.OK, widgets);
      if (result.ETag != null)
        response.Headers.Add("ETag", "\"" + result.ETag + "\"");
      response.Headers.Add("Access-Control-Expose-Headers", "ETag");
      return response;
    }
  }
}
