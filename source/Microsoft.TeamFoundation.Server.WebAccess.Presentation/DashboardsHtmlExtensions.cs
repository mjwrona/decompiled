// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.DashboardsHtmlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  public static class DashboardsHtmlExtensions
  {
    public static MvcHtmlString CreateDefaultDashboardIdJsonIsland(
      this HtmlHelper htmlHelper,
      string htmlClass,
      string defaultDashboardId)
    {
      return htmlHelper.RestApiJsonIsland((object) defaultDashboardId, (object) new
      {
        @class = htmlClass
      });
    }

    public static MvcHtmlString CreateDefaultDashboardWidgetsJsonIsland(
      this HtmlHelper htmlHelper,
      string htmlClass,
      Dashboard defaultDashboardWidgets)
    {
      return htmlHelper.RestApiJsonIsland((object) defaultDashboardWidgets, (object) new
      {
        @class = htmlClass
      });
    }

    public static MvcHtmlString CreateMaxWidgetsJsonIsland(
      this HtmlHelper htmlHelper,
      string htmlClass,
      int maxWidgetsPerDashboard)
    {
      return htmlHelper.RestApiJsonIsland((object) maxWidgetsPerDashboard, (object) new
      {
        @class = htmlClass
      });
    }

    public static MvcHtmlString CreateMaxDashboardsJsonIsland(
      this HtmlHelper htmlHelper,
      string htmlClass,
      int maxDashboardsPerGroup)
    {
      return htmlHelper.RestApiJsonIsland((object) maxDashboardsPerGroup, (object) new
      {
        @class = htmlClass
      });
    }

    public static MvcHtmlString CreateIsStakeholderJsonIsland(
      this HtmlHelper htmlHelper,
      string htmlClass,
      bool isStakeholder)
    {
      return htmlHelper.RestApiJsonIsland((object) isStakeholder, (object) new
      {
        @class = htmlClass
      });
    }
  }
}
