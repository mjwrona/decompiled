// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.DashboardsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [SupportedRouteArea(NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [RegisterHubMruPage(true)]
  [RemoveMruEntryOnRenamedProject]
  [RemoveMruEntryonDeletedProjectTeam]
  public class DashboardsController : TfsAreaController
  {
    public DashboardsController() => this.m_executeContributedRequestHandlers = true;

    public override string AreaName => "Dashboards";

    public override string TraceArea => "WebAccess.Dashboards";

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult Index()
    {
      try
      {
        return (ActionResult) this.View("~/_views/Home/ProjectIndex.aspx", (object) DashboardsViewHelper.GetViewModel(this.TfsWebContext));
      }
      catch (DashboardDoesNotExistException ex)
      {
        throw new HttpException(404, ex.Message, (Exception) ex);
      }
      catch (TeamNotFoundException ex)
      {
        throw new HttpException(404, ex.Message, (Exception) ex);
      }
    }
  }
}
