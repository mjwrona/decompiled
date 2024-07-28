// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.MruController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [SupportedRouteArea(NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [RemoveMruEntryOnRenamedProject]
  [RemoveMruEntryonDeletedProjectTeam]
  public class MruController : TfsController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(530000, 530010)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult RedirectMru(
      string toController,
      string toAction,
      string toRouteArea,
      string toParameters = null,
      string toHubContribution = null,
      string toRouteId = null)
    {
      string url = (string) null;
      if (string.IsNullOrEmpty(toRouteArea))
        toRouteArea = "";
      if (string.IsNullOrEmpty(toController))
        toController = "home";
      if (string.IsNullOrEmpty(toAction))
        toAction = "index";
      string parameters = (string) null;
      if (!string.IsNullOrEmpty(toParameters))
        parameters = NavigationExtensions.GetTargetRouteParametersValue(this.TfsWebContext, toController, toAction, toParameters);
      if (!string.IsNullOrEmpty(toHubContribution) || !string.IsNullOrEmpty(toRouteId))
        url = this.GetContributedHubRouteUrl(this.TfsWebContext, toHubContribution, toRouteId, toParameters);
      if (string.IsNullOrEmpty(url))
        url = NavigationExtensions.GetTargetUrl(this.TfsWebContext, toRouteArea, toController, toAction, parameters, this.TfsWebContext.NavigationContext.TopMostLevel, this.TfsWebContext.NavigationContext.TopMostLevel, new RouteValueDictionary(), false);
      return (ActionResult) this.Redirect(url);
    }

    private string GetContributedHubRouteUrl(
      TfsWebContext tfsWebContext,
      string contributionId = null,
      string routeId = null,
      string toParameters = null)
    {
      string contributedHubRouteUrl = (string) null;
      IContributionService service = this.TfsRequestContext.GetService<IContributionService>();
      RouteValueDictionary routeValues = new RouteValueDictionary()
      {
        {
          "project",
          (object) this.TfsWebContext.NavigationContext.Project
        },
        {
          "team",
          (object) this.TfsWebContext.NavigationContext.Team
        }
      };
      if (!string.IsNullOrEmpty(toParameters) || !string.IsNullOrEmpty(this.RouteData.GetRouteValue<string>("parameters")))
        routeValues["parameters"] = (object) toParameters;
      if (!string.IsNullOrEmpty(contributionId))
      {
        Contribution contribution = service.QueryContribution(this.TfsRequestContext, contributionId);
        if (contribution != null && this.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<IContributionFilterService>().ApplyConstraints(this.TfsRequestContext, contribution, (string) null, string.Empty, (ICollection<EvaluatedCondition>) null))
          contributedHubRouteUrl = NavigationHelpers.GetHubDefaultUrl((WebContext) this.TfsWebContext, contribution, routeValues);
      }
      else if (!string.IsNullOrEmpty(routeId))
        contributedHubRouteUrl = NavigationHelpers.GetHubDefaultUrl((WebContext) this.TfsWebContext, routeId, routeValues);
      return contributedHubRouteUrl;
    }
  }
}
