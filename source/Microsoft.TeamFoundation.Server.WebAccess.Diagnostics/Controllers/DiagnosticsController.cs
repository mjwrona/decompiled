// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.Controllers.DiagnosticsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6A4F2FF9-BE93-434B-9864-FE0D09D21D75
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.Controllers
{
  [SupportedRouteArea(NavigationContextLevels.All)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class DiagnosticsController : DiagnosticsAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index() => (ActionResult) this.View();

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult EnableTracePointCollector(bool? disable)
    {
      this.SetDiagnosticCookie("TFS-TRACEPOINT-COLLECTOR", disable);
      return (ActionResult) this.View("Index", (object) DateTime.Now);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult EnableScriptDebugMode(bool? disable)
    {
      this.SetDiagnosticCookie("TFS-DEBUG", disable);
      return (ActionResult) this.View("Index", (object) DateTime.Now);
    }

    private void SetDiagnosticCookie(string cookieName, bool? disable)
    {
      string str = !disable.HasValue || !disable.Value ? "enabled" : "disabled";
      HttpCookie cookie = this.Request.Cookies[cookieName] ?? new HttpCookie(cookieName);
      cookie.Value = str;
      cookie.Expires = DateTime.MaxValue;
      this.Request.Cookies.Set(cookie);
      this.Response.Cookies.Set(cookie);
    }
  }
}
