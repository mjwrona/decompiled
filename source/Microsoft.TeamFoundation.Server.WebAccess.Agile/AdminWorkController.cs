// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AdminWorkController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Admin;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [SupportedRouteArea("Admin", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("65AC9DB3-BB0A-42fe-B584-A690FB0D817B", true)]
  public class AdminWorkController : AdminAreaController
  {
    public AdminWorkController() => this.m_executeContributedRequestHandlers = true;

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index()
    {
      this.ViewData["view-title"] = (object) AgileResources.AdminWork_Title;
      return this.NavigationContext.Levels.HasFlag((Enum) NavigationContextLevels.Team) ? this.ServeTeamWorkView() : this.ServeProjectWorkView();
    }

    [HttpPost]
    public ActionResult TeamField([ModelBinder(typeof (JsonModelBinder))] TeamFieldData saveData, string userAction)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(userAction, nameof (userAction));
      if (userAction.Equals("Save", StringComparison.OrdinalIgnoreCase))
      {
        ArgumentUtility.CheckForNull<TeamFieldData>(saveData, nameof (saveData));
        WebApiTeam requestTeamOrDefault = this.TfsRequestContext.GetRequestTeamOrDefault();
        this.TfsRequestContext.GetService<ITeamConfigurationService>().SaveTeamFields(this.TfsRequestContext, requestTeamOrDefault, (ITeamFieldValue[]) saveData.TeamFieldValues, saveData.DefaultValueIndex);
      }
      return (ActionResult) this.Json((object) new
      {
        success = true
      });
    }

    private ActionResult ServeProjectWorkView() => (ActionResult) this.View("ProjectWork", (object) AgileAdminHelpers.CreateProjectModel(this.TfsRequestContext, this.TfsWebContext.Project));

    private ActionResult ServeTeamWorkView()
    {
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project = this.TfsWebContext.Project;
      TeamViewModel teamViewModel = AgileAdminHelpers.CreateTeamViewModel(this.TfsRequestContext, project.Uri, this.TfsWebContext.Team);
      bool flag = AgileAdminHelpers.IsTeamFieldAreaPath(this.TfsRequestContext, project);
      if (!flag)
        this.ViewData["TeamFieldData"] = AgileAdminHelpers.AddTeamFieldViewData(this.TfsRequestContext, teamViewModel.ProcessSettings, project, this.TfsWebContext.Team, teamViewModel.Settings);
      return (ActionResult) this.View("TeamWork", (object) new AdminWorkModel()
      {
        TeamViewModel = teamViewModel,
        IsTeamFieldAreaPath = flag,
        ProjectWorkModel = AgileAdminHelpers.CreateProjectModel(this.TfsRequestContext, project)
      });
    }
  }
}
