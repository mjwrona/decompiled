// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Model.DashboardExtension
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Dashboards.Model
{
  public static class DashboardExtension
  {
    public static void Validate(this Dashboard entry)
    {
      if (string.IsNullOrWhiteSpace(entry.Name))
        throw new EmptyDashboardNameException();
      if (entry.Name.Length > 32)
        throw new DashboardNameLengthExceededException(32, entry.Name.Length);
      if (!string.IsNullOrEmpty(entry.Description) && entry.Description.Length > 128)
        throw new DashboardDescriptionLengthExceededException(128, entry.Description.Length);
      if (entry.Position < 1)
        throw new DashboardInvalidPositionException(entry.Position);
    }

    public static void ValidateNameIsUnique(
      this Dashboard entry,
      IVssRequestContext requestContext,
      IEnumerable<Dashboard> existingDashboards,
      string feature = "DashboardService.AddDashboard",
      int tracepoint = 10017050)
    {
      if (existingDashboards.Any<Dashboard>((Func<Dashboard, bool>) (o => string.Equals(o.Name, entry.Name, StringComparison.OrdinalIgnoreCase))))
      {
        requestContext.TraceAlways(tracepoint, TraceLevel.Error, feature, "DashboardService", "search for: " + entry.Name + " in (" + string.Join(",", existingDashboards.Select<Dashboard, string>((Func<Dashboard, string>) (x => x.Name)).ToArray<string>()) + ")");
        throw new DuplicateDashboardNameException();
      }
    }

    public static void ValidateAllNamesAreUnique(IEnumerable<Dashboard> existingDashboards)
    {
      if (new HashSet<string>(existingDashboards.Select<Dashboard, string>((Func<Dashboard, string>) (o => o.Name)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Count < existingDashboards.Count<Dashboard>())
        throw new DuplicateDashboardNameException();
    }

    public static void AddLinksForCopy(
      this Dashboard response,
      IVssRequestContext TfsRequestContext,
      UrlHelper Url,
      Guid groupId,
      bool isTeamScoped = true,
      Guid teamId = default (Guid))
    {
      response.Url = DashboardExtension.GetLinkUsingRouteValues(Url, DashboardsLocationIds.DashboardV2, groupId, teamId, response.Id.Value);
      response.Links = new ReferenceLinks();
      response.Links.AddLink("self", response.Url, (ISecuredObject) response);
      response.Links.AddLink("group", DashboardExtension.GetLinkUsingRouteValues(Url, DashboardsLocationIds.DashboardV2, groupId, teamId, response.Id.Value), (ISecuredObject) response);
      if (response.Widgets == null)
        return;
      foreach (Widget widget in response.Widgets)
      {
        string usingRouteValues = DashboardExtension.GetLinkUsingRouteValues(Url, DashboardsLocationIds.WidgetV2, groupId, teamId, response.Id.Value, widget.Id.Value);
        widget.Url = usingRouteValues;
        response.Links.AddLink("widget", widget.Url, (ISecuredObject) response);
      }
    }

    public static string GetLinkUsingRouteValues(
      UrlHelper urlHelper,
      Guid locationId,
      Guid groupId,
      Guid teamId = default (Guid),
      Guid dashboardId = default (Guid),
      Guid widgetId = default (Guid))
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      routeValues.Add("project", (object) groupId.ToString());
      if (teamId != new Guid())
        routeValues.Add("team", (object) groupId.ToString());
      if (dashboardId != new Guid())
        routeValues.Add(nameof (dashboardId), (object) dashboardId.ToString());
      if (widgetId != new Guid())
        routeValues.Add(nameof (widgetId), (object) widgetId.ToString());
      return urlHelper.Link(locationId, (object) routeValues);
    }

    public static void AddLinks(
      this Dashboard response,
      IVssRequestContext TfsRequestContext,
      UrlHelper Url,
      Guid groupId,
      bool isTeamScoped = true,
      Guid teamId = default (Guid))
    {
      string scopePath = "";
      if (!isTeamScoped)
      {
        scopePath = groupId.ToString();
        if (teamId != Guid.Empty)
          scopePath = scopePath + "/" + teamId.ToString();
      }
      response.Url = DashboardExtension.GetDashboardUrl(TfsRequestContext, Url, new Guid?(response.Id.Value), scopePath);
      response.Links = new ReferenceLinks();
      response.Links.AddLink("self", response.Url, (ISecuredObject) response);
      response.Links.AddLink("group", DashboardExtension.GetDashboardUrl(TfsRequestContext, Url, new Guid?(), scopePath), (ISecuredObject) response);
      if (response.Widgets == null)
        return;
      foreach (Widget widget in response.Widgets)
      {
        Widget response1 = widget;
        IVssRequestContext TfsRequestContext1 = TfsRequestContext;
        UrlHelper Url1 = Url;
        Guid groupId1 = groupId;
        Guid? id = response.Id;
        Guid dashboardId1 = id.Value;
        response1.AddLinks(TfsRequestContext1, Url1, groupId1, dashboardId1);
        response.Links.AddLink("widget", widget.Url, (ISecuredObject) response);
        Widget response2 = widget;
        IVssRequestContext TfsRequestContext2 = TfsRequestContext;
        UrlHelper Url2 = Url;
        Guid groupId2 = groupId;
        id = response.Id;
        Guid dashboardId2 = id.Value;
        response2.AddLinks(TfsRequestContext2, Url2, groupId2, dashboardId2);
      }
    }

    public static string GetGroupUrl(
      IVssRequestContext TfsRequestContext,
      UrlHelper Url,
      Guid groupId)
    {
      return Url.RestLink(TfsRequestContext, DashboardsLocationIds.DashboardGroup, (object) new
      {
        groupId = groupId
      });
    }

    public static string GetDashboardUrl(
      IVssRequestContext TfsRequestContext,
      UrlHelper Url,
      Guid? dashboardId,
      string scopePath = "")
    {
      return Url.RestLink(TfsRequestContext, DashboardsLocationIds.DashboardV2, scopePath, (object) new
      {
        dashboardId = dashboardId
      });
    }

    public static string GetWidgetUrl(
      IVssRequestContext TfsRequestContext,
      UrlHelper Url,
      Guid? widgetId)
    {
      return Url.RestLink(TfsRequestContext, DashboardsLocationIds.WidgetV2, (object) new
      {
        widgetId = widgetId
      });
    }

    public static void ValidateCopyOptions(CopyDashboardOptions options)
    {
      if (options.CopyDashboardScope != DashboardScope.Project && options.CopyDashboardScope != DashboardScope.Project_Team)
        throw new ArgumentException("Invalid Scope argument for Copy Dashboard Operation", "CopyDashboardScope").Expected("Dashboards");
      ArgumentUtility.CheckForEmptyGuid(options.ProjectId, "ProjectId", "Dashboards");
      Guid? nullable;
      if (options.CopyDashboardScope == DashboardScope.Project_Team)
      {
        ArgumentUtility.CheckForNull<Guid>(options.TeamId, "TeamId", "Dashboards");
        ArgumentUtility.CheckForEmptyGuid(options.TeamId.Value, "TeamId", "Dashboards");
      }
      else
      {
        nullable = options.TeamId;
        if (nullable.HasValue)
          throw new ArgumentException("Value should be null for Project Copy: ", "TeamId").Expected("Dashboards");
      }
      bool? copyQueriesFlag = options.CopyQueriesFlag;
      if (!copyQueriesFlag.HasValue)
        return;
      copyQueriesFlag = options.CopyQueriesFlag;
      bool flag = true;
      if (!(copyQueriesFlag.GetValueOrDefault() == flag & copyQueriesFlag.HasValue))
        return;
      nullable = options.QueryFolderPath;
      if (!nullable.HasValue)
        return;
      nullable = options.QueryFolderPath;
      ArgumentUtility.CheckForEmptyGuid(nullable.Value, "QueryFolderPath", "Dashboards");
      try
      {
        nullable = options.QueryFolderPath;
        Guid result;
        if (!Guid.TryParse(nullable.ToString(), out result))
          throw new ArgumentException("Value should be not be null for query folder location copy: ", "QueryFolderPath").Expected("Dashboards");
        if (result == Guid.Empty)
          throw new ArgumentException("The selected Folder does not exist.", "QueryFolderPath").Expected("Dashboards");
      }
      catch (Exception ex)
      {
        throw new ArgumentException("The selected Folder does not exist.", "QueryFolderPath").Expected("Dashboards");
      }
    }
  }
}
