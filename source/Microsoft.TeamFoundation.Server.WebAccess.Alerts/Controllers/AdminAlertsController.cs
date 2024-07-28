// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.Controllers.AdminAlertsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts.Controllers
{
  [SupportedRouteArea("Admin", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("65AC9DB3-BB0A-42fe-B584-A690FB0D817B", true)]
  public class AdminAlertsController : AlertsAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index() => (ActionResult) this.View();
  }
}
