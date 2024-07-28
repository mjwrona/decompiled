// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestCaseResultsRouteHandler
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestCaseResultsRouteHandler : 
    IPreExecuteContributedRequestHandler,
    IContributedRequestHandler
  {
    private const string c_pivot = "pivot";
    private const string c_execute_pivot = "execute";
    private const string c_testplans_hub_refresh_route = "ms.vss-test-web.testplans-hub-refresh-route";

    public string Name => "TestCaseResults.RouteHandler";

    public void OnPreExecute(
      IVssRequestContext requestContext,
      ActionExecutingContext actionExecutingContext,
      JObject properties)
    {
      NameValueCollection queryParameters = requestContext.GetService<IContributionRoutingService>().GetQueryParameters(requestContext);
      int result;
      if (queryParameters != null && queryParameters["testCaseId"] != null && int.TryParse(queryParameters["testCaseId"], out result) && result > 0)
        return;
      string projectName = this.GetProjectAndTeamData(requestContext).projectName;
      actionExecutingContext.Result = (ActionResult) new RedirectResult(this.GetUrlToRedirect(requestContext, projectName));
    }

    protected internal virtual string GetUrlToRedirect(
      IVssRequestContext requestContext,
      string projectName)
    {
      return new Uri(new Uri(requestContext.RequestUri().GetLeftPart(UriPartial.Authority)), TestCaseResultsRouteHandler.GetRouteUrl(requestContext, projectName, "ms.vss-test-web.testplans-hub-refresh-route")).AbsoluteUri;
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
      IVssRequestContext requestContext1 = requestContext;
      string contributionId = contributedRouteId;
      RouteValueDictionary routeValues = routeValueDictionary;
      return service.RouteUrl(requestContext1, contributionId, routeValues);
    }
  }
}
