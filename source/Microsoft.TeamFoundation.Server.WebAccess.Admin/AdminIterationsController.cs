// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminIterationsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Admin", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  public class AdminIterationsController : AdminBaseClassificationController
  {
    public const string FILTERNAME_ITERATIONS_SHOW = "Iterations.ShowFilter";

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(210301, 210309)]
    public ActionResult Index() => (ActionResult) this.RedirectToAction("", "AdminWork", (object) new
    {
      _a = "iterations"
    });

    [HttpPost]
    [TfsTraceFilter(210311, 210319)]
    public ActionResult UpdateIterationsData([ModelBinder(typeof (JsonModelBinder))] AdminIterationsData saveData)
    {
      ArgumentUtility.CheckForNull<AdminIterationsData>(saveData, nameof (saveData));
      WebApiTeam team = this.Team;
      this.TfsRequestContext.GetService<TeamConfigurationService>().SaveBacklogIterations(this.TfsRequestContext, team, saveData.selectedIterations, saveData.rootIterationId, false);
      return (ActionResult) this.Json((object) new
      {
        success = true
      }, JsonRequestBehavior.AllowGet);
    }
  }
}
