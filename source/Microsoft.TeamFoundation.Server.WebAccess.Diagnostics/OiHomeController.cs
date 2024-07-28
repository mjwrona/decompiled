// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.OiHomeController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6A4F2FF9-BE93-434B-9864-FE0D09D21D75
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.dll

using Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.Controllers;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Diagnostics
{
  [SupportedRouteArea("Oi", NavigationContextLevels.All)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class OiHomeController : DiagnosticsAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index() => (ActionResult) this.RedirectToAction("activityLog", "diagnostics", (object) new
    {
      routeArea = "Oi"
    });
  }
}
