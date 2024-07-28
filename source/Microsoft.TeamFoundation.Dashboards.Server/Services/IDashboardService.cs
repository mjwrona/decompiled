// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.IDashboardService
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  [DefaultServiceImplementation(typeof (DashboardService))]
  public interface IDashboardService : IVssFrameworkService
  {
    Dashboard AddDashboard(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard entry);

    CopyDashboardResponse CopyDashboard(
      IVssRequestContext requestContext,
      Guid sourceDashboardId,
      IDashboardConsumer sourceDashboardConsumer,
      IDashboardConsumer targetDashboardConsumer,
      CopyDashboardOptions options);

    Dashboard UpdateDashboard(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard dashboard);

    DashboardGroup UpdateDashboards(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      DashboardGroup dashboards,
      bool enforceUniqueNames = true);

    void DeleteDashboard(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid id);

    DashboardGroup GetDashboardGroup(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer);

    List<Dashboard> GetDashboards(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> dashboardIds = null);

    Dashboard GetDashboard(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid id);

    Widget AddWidget(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Widget widget);

    Widget UpdateWidget(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Widget widget);

    Widget ReplaceWidget(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Widget widget);

    Dashboard UpdateDashboardWidgets(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard dashboard);

    Dashboard ReplaceDashboardWidgets(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard dashboard);

    Dashboard DeleteWidget(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Guid id);

    Widget GetWidgetById(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Guid id);

    void UpdateDashboardLastAccessedDate(
      IVssRequestContext requestContext,
      Guid dashboardId,
      Guid projectId,
      Guid groupId);
  }
}
