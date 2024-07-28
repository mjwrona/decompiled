// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.MyExperiences.ApiAccountSearchController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.MyExperiences, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD340A61-D28F-4435-96FD-F6CA1BCEA981
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.MyExperiences.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.MyExperiences
{
  [SupportedRouteArea("Api", NavigationContextLevels.Collection)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiAccountSearchController : TfsController
  {
    protected override void Initialize(RequestContext requestContext) => base.Initialize(requestContext);

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetMyTeams()
    {
      List<WebApiTeam> source = new List<WebApiTeam>();
      HashSet<string> defaultTeamsToExclude = new HashSet<string>();
      IProjectService service1 = this.TfsRequestContext.GetService<IProjectService>();
      ITeamService service2 = this.TfsRequestContext.GetService<ITeamService>();
      using (PerformanceTimer.StartMeasure(this.TfsWebContext.TfsRequestContext, "ApiAccountSearchController.GetMyTeams.Load"))
      {
        List<ProjectInfo> list = service1.GetProjects(this.TfsRequestContext, ProjectState.WellFormed).PopulateProperties(this.TfsRequestContext, TeamConstants.DefaultTeamPropertyName).ToList<ProjectInfo>();
        if (list != null)
        {
          list.ForEach((Action<ProjectInfo>) (x =>
          {
            ProjectProperty projectProperty = x.Properties.ToList<ProjectProperty>().Find((Predicate<ProjectProperty>) (p => p.Name.Equals(TeamConstants.DefaultTeamPropertyName)));
            if (projectProperty == null)
              return;
            defaultTeamsToExclude.Add((string) projectProperty.Value);
          }));
          source = service2.QueryAllTeamsInCollection(this.TfsRequestContext).ToList<WebApiTeam>();
          if (defaultTeamsToExclude.Count > 0)
            source.RemoveAll((Predicate<WebApiTeam>) (t => defaultTeamsToExclude.Contains(t.Id.ToString())));
        }
      }
      WebPerformanceTimerHelpers.SendCustomerIntelligenceData((WebContext) this.TfsWebContext);
      return (ActionResult) this.Json((object) new
      {
        teams = source.Select<WebApiTeam, ApiAccountSearchController.WebApiTeamExtended>((Func<WebApiTeam, ApiAccountSearchController.WebApiTeamExtended>) (x => new ApiAccountSearchController.WebApiTeamExtended(x))).ToList<ApiAccountSearchController.WebApiTeamExtended>()
      }, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetMyTeamMemberships() => (ActionResult) this.Json((object) new
    {
      teamMemberships = this.TfsRequestContext.GetService<ITeamService>().QueryMyTeamsInCollection(this.TfsRequestContext, this.TfsRequestContext.UserContext).ToList<WebApiTeam>().ToList<WebApiTeam>()
    }, JsonRequestBehavior.AllowGet);

    public class WebApiTeamExtended
    {
      public Guid TeamId { get; private set; }

      public string TeamName { get; private set; }

      public string ProjectId { get; private set; }

      public WebApiTeamExtended(WebApiTeam team)
      {
        this.TeamId = team.Id;
        this.TeamName = team.Name;
        this.ProjectId = team.ProjectId.ToString();
      }
    }
  }
}
