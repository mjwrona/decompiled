// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.Controllers.AlertsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts.Controllers
{
  [SupportedRouteArea(NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  public class AlertsController : AlertsAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index(bool? useLegacyPage) => !useLegacyPage.HasValue && !string.IsNullOrEmpty(this.GetUserNotificationUrl(this.TfsRequestContext)) ? (ActionResult) this.Redirect(this.GetUserNotificationUrl(this.TfsRequestContext)) : (ActionResult) this.View();

    private string GetUserNotificationUrl(IVssRequestContext requestContext) => requestContext.GetService<IContributionRoutingService>().RouteUrl(requestContext, "ms.vss-notifications-web.user-notifications-route");
  }
}
