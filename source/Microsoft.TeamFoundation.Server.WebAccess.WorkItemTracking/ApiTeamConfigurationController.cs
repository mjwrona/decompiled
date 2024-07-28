// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.ApiTeamConfigurationController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System.Diagnostics;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiTeamConfigurationController : WorkItemsAreaController
  {
    private const int WebAccessExceptionEaten = 599999;

    [OutputCache(CacheProfile = "NoCache")]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [RequireTeam]
    [HttpGet]
    public ActionResult TeamSettings()
    {
      this.Trace(516201, TraceLevel.Info, "Update WorkItems");
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) TeamSettingsDataSource.TeamSettings(this.TfsRequestContext, this.TfsWebContext.Project));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }
  }
}
