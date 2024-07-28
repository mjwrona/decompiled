// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestPlansHubRefreshRouteHandler
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestPlansHubRefreshRouteHandler : 
    IPreExecuteContributedRequestHandler,
    IContributedRequestHandler
  {
    private const string c_selectedPlanIdKey = "SelectedPlanId";
    private const string c_planIdParam = "planId";
    private const string c_contribution_mine_all = "ms.vss-test-web.test-mine-directory-route-LWP";
    private const string c_contribution_lite = "ms.vss-test-web.test-testplan-route";
    private const string c_contribution_testhub_refresh = "ms.vss-test-web.testplans-hub-refresh-route";
    private const string c_pivot = "pivot";
    private const string c_mine = "mine";
    private const string c_testManagementUrl = "_testManagement";

    public string Name => "TestHubRefresh.RouteHandler";

    public void OnPreExecute(
      IVssRequestContext requestContext,
      ActionExecutingContext actionExecutingContext,
      JObject properties)
    {
      IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
      (Guid projectId, string projectName, Guid teamId, string _) = this.GetProjectAndTeamData(requestContext);
      if (this.isUrlOldTestHubUrl(requestContext))
      {
        actionExecutingContext.Result = (ActionResult) new RedirectResult(this.GetNewTestHubRefreshUrl(requestContext, projectName));
      }
      else
      {
        NameValueCollection queryParameters = service.GetQueryParameters(requestContext);
        if (queryParameters != null && queryParameters["planId"] != null && int.TryParse(queryParameters["planId"], out int _))
          return;
        int num1 = requestContext.GetService<ISettingsService>().GetValue<int>(requestContext, SettingsUserScope.User, "project", projectId.ToString(), "SelectedPlanId", 0, false);
        int num2 = num1 != 0 ? num1 : this.GetTestPlanLoadSettings(requestContext, projectId, teamId).SelectedPlanId;
        if (num2 > 0)
          service.SetQueryParameter(requestContext, "planId", num2.ToString());
        else
          actionExecutingContext.Result = (ActionResult) new RedirectResult(this.GetUrlToRedirect(requestContext, projectName));
      }
    }

    protected internal virtual string GetUrlToRedirect(
      IVssRequestContext requestContext,
      string projectName)
    {
      return new Uri(new Uri(TestManagementUrlHelper.GetTfsBaseUrl(requestContext)), TestPlansHubRefreshRouteHandler.GetRouteUrl(requestContext, projectName, "ms.vss-test-web.test-mine-directory-route-LWP")).AbsoluteUri;
    }

    protected internal virtual (Guid projectId, string projectName, Guid teamId, string pageUrl) GetProjectAndTeamData(
      IVssRequestContext requestContext)
    {
      WebPageDataProviderPageSource pageSource = WebPageDataProviderUtil.GetPageSource(requestContext);
      Guid id = pageSource.Project.Id;
      string name = pageSource.Project.Name;
      ContextIdentifier team = pageSource.Team;
      Guid guid = team != null ? team.Id : Guid.Empty;
      string url = pageSource.Url;
      return (id, name, guid, url);
    }

    protected internal virtual TestManagementTestPlanLiteLoadSettings GetTestPlanLoadSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      return new TestManagementTestPlanLiteLoadSettings(requestContext, projectId, teamId);
    }

    protected internal bool isUrlOldTestHubUrl(IVssRequestContext requestContext)
    {
      string[] segments = requestContext.RequestUri().Segments;
      int index = 3;
      if (segments.Length == 5)
        index = 4;
      return segments.Length > index && segments[index].Equals("_testManagement", StringComparison.OrdinalIgnoreCase);
    }

    protected internal string GetNewTestHubRefreshUrl(
      IVssRequestContext requestContext,
      string projectName)
    {
      Uri uri = requestContext.RequestUri();
      return new Uri(new Uri(TestManagementUrlHelper.GetTfsBaseUrl(requestContext)), TestPlansHubRefreshRouteHandler.GetRouteUrl(requestContext, projectName, "ms.vss-test-web.testplans-hub-refresh-route") + uri.Query).AbsoluteUri;
    }

    private static string GetRouteUrl(
      IVssRequestContext requestContext,
      string projectName,
      string contributedRouteId)
    {
      IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary()
      {
        {
          "project",
          (object) projectName
        }
      };
      if (contributedRouteId == "ms.vss-test-web.test-mine-directory-route-LWP")
        routeValueDictionary.Add("pivot", (object) "mine");
      IVssRequestContext requestContext1 = requestContext;
      string contributionId = contributedRouteId;
      RouteValueDictionary routeValues = routeValueDictionary;
      return service.RouteUrl(requestContext1, contributionId, routeValues);
    }
  }
}
