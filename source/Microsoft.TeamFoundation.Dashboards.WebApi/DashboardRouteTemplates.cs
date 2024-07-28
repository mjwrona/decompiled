// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardRouteTemplates
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  public class DashboardRouteTemplates
  {
    public const string DashboardGroupRouteTemplate = "{area}/{resource}/{groupId}";
    public const string DashboardsRouteTemplate = "{area}/groups/{groupId}/{resource}/{dashboardId}";
    public const string WidgetsRouteTemplate = "{area}/groups/{groupId}/dashboards/{dashboardId}/{resource}/{widgetId}";
    public const string WidgetTypesRouteTemplate = "{area}/{resource}/{contributionId}";
    public const string DashboardsRouteTemplateV2 = "{area}/{resource}/{dashboardId}";
    public const string WidgetsRouteTemplateV2 = "{area}/dashboards/{dashboardId}/{resource}/{widgetId}";
  }
}
