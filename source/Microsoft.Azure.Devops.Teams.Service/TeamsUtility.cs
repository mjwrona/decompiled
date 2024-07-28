// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.TeamsUtility
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Http.Controllers;

namespace Microsoft.Azure.Devops.Teams.Service
{
  public static class TeamsUtility
  {
    public static bool IsValidTeamName(string name) => CssUtils.IsValidProjectName(name);

    public static WebApiTeam GetTeamFromRequest(
      IVssRequestContext tfsRequestContext,
      HttpControllerContext controllerContext,
      ProjectInfo projectInfo,
      bool requireExplicitTeam)
    {
      WebApiTeam teamFromRequest = (WebApiTeam) null;
      string teamNameOrId = TeamsUtility.GetTeamNameOrID(controllerContext);
      if (string.IsNullOrEmpty(teamNameOrId))
      {
        if (!requireExplicitTeam)
        {
          ITeamService service = tfsRequestContext.GetService<ITeamService>();
          teamFromRequest = service.GetDefaultTeam(tfsRequestContext, projectInfo.Id);
          if (teamFromRequest == null)
          {
            WebApiTeam webApiTeam = service.QueryTeamsInProject(tfsRequestContext, projectInfo.Id).FirstOrDefault<WebApiTeam>();
            if (webApiTeam == null)
              throw new NoTeamsWereFoundException(projectInfo.Name);
            try
            {
              service.SetDefaultTeamId(tfsRequestContext, projectInfo.Id, webApiTeam.Id);
            }
            catch (Exception ex) when (ex is AccessCheckException || ex is UnauthorizedAccessException)
            {
              throw new DefaultTeamNotFoundException(projectInfo.Name);
            }
            teamFromRequest = webApiTeam;
          }
        }
      }
      else
      {
        teamFromRequest = tfsRequestContext.GetService<ITeamService>().GetTeamInProject(tfsRequestContext, projectInfo.Id, teamNameOrId);
        if (teamFromRequest == null)
          throw new TeamNotFoundException(teamNameOrId);
      }
      if (teamFromRequest != null)
        tfsRequestContext.RootContext.Items["RequestTeam"] = (object) teamFromRequest;
      return teamFromRequest;
    }

    private static string GetTeamNameOrID(HttpControllerContext controllerContext)
    {
      string empty = string.Empty;
      if (controllerContext.RouteData != null && controllerContext.RouteData.Values != null)
        controllerContext.RouteData.Values.TryGetValue<string>("team", out empty);
      if (string.IsNullOrEmpty(empty) && controllerContext.Request.RequestUri != (Uri) null)
      {
        NameValueCollection queryString = controllerContext.Request.RequestUri.ParseQueryString();
        if (queryString != null && queryString["team"] != null)
          empty = queryString["team"];
      }
      return empty;
    }

    private static WebApiTeam GetDefaultTeam(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      return requestContext.GetService<ITeamService>().GetDefaultTeam(requestContext, projectInfo.Id);
    }
  }
}
