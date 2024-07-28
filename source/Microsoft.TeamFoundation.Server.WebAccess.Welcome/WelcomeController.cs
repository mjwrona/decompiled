// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Welcome.WelcomeController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Welcome, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B024A61-082C-4505-8523-CF030F6A8A5A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Welcome.dll

using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Welcome
{
  [SupportedRouteArea(NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [RegisterHubMruPage(true)]
  [DemandFeature("2FF0A29B-5679-44f6-8FAD-F5968AE3E32E", true)]
  public class WelcomeController : WelcomeAreaController
  {
    protected override void Initialize(RequestContext requestContext)
    {
      base.Initialize(requestContext);
      NavigationExtensions.SkipHubGroupMruUpdate(this.ViewData);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index() => this.TfsRequestContext.IsFeatureEnabled("WebAccess.WelcomeHub") && !this.TfsRequestContext.IsFeatureEnabled("WebAccess.ProjectWelcome") ? (ActionResult) this.View((object) new WelcomeViewModel(this.TfsWebContext)
    {
      MaxDashboardPerGroup = DashboardSettings.GetMaxDashboardsPerGroup(this.TfsRequestContext)
    }) : throw new HttpException(404, WACommonResources.PageNotFound);
  }
}
