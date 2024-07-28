// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Requirements.Controllers.FeedbackController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Requirements, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6C113FD4-8DA1-49E9-A859-47B7ED9A5698
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Requirements.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Requirements.Controllers
{
  [SupportedRouteArea(NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("BB000720-4CF7-466A-BA47-1AB40B7A8DFB", false)]
  public class FeedbackController : TfsController
  {
    private const string RequestFeedbackAction = "requestFeedback";

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(310000, 310010)]
    public ActionResult Index() => (ActionResult) this.View(nameof (Index));

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult RequestFeedback() => (ActionResult) this.Redirect(this.Url.Action("index", "home", (object) new
    {
      routeArea = ""
    }) + UrlUtilities.BuildFragmentAction("requestFeedback"));
  }
}
