// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Team.TeamsControllerBase
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.Team
{
  public abstract class TeamsControllerBase : ServerCoreApiController
  {
    private const string s_area = "Teams";
    private const string s_layer = "WebApi";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddTranslation<Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException, Microsoft.TeamFoundation.Core.WebApi.TeamNotFoundException>();
      exceptionMap.AddStatusCode<Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Core.WebApi.TeamNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TeamAlreadyExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<ProjectDoesNotExistWithNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProjectDoesNotExistException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTeamNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTeamDescriptionException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TeamLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CreateTeamInputValuesException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UpdateTeamInputValuesException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<DeleteTeamInputValuesException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TeamUpdateInvalidPermissionException>(HttpStatusCode.Forbidden);
    }

    [ClientResponseType(typeof (WebApiTeam), null, null)]
    [HttpPost]
    [ClientLocationId("D30A3DD1-F8BA-442A-B86A-BD0C0C383E59")]
    [ClientExample("POST__projects__projectId__teams.json", null, null, null)]
    public HttpResponseMessage CreateTeam(string projectId, WebApiTeam team)
    {
      this.CheckProjectIdInputValue<CreateTeamInputValuesException>(projectId);
      this.CheckWebApiTeamInputValue<CreateTeamInputValuesException>(team);
      try
      {
        TeamProject teamProject = ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId, false, false, false);
        return this.Request.CreateResponse<WebApiTeam>(HttpStatusCode.Created, this.TfsRequestContext.GetService<ITeamService>().CreateTeam(this.TfsRequestContext, teamProject.TfsUri, team.Name, team.Description));
      }
      catch (GroupCreationException ex)
      {
        throw new TeamAlreadyExistsException(team.Name);
      }
    }

    [ClientResponseType(typeof (WebApiTeam), null, null)]
    [HttpPatch]
    [ClientLocationId("D30A3DD1-F8BA-442A-B86A-BD0C0C383E59")]
    [ClientExample("PATCH__projects__projectId__teams.json", null, null, null)]
    public HttpResponseMessage UpdateTeam(string projectId, string teamId, WebApiTeam teamData)
    {
      this.CheckProjectIdInputValue<UpdateTeamInputValuesException>(projectId);
      this.CheckTeamIdInputValue<UpdateTeamInputValuesException>(teamId);
      this.CheckWebApiTeamInputValue<UpdateTeamInputValuesException>(teamData);
      TeamProject teamProject = ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId, false, false, false);
      ITeamService service = this.TfsRequestContext.GetService<ITeamService>();
      WebApiTeam team = Microsoft.TeamFoundation.Server.Core.TeamsUtility.GetTeam(this.TfsRequestContext, teamProject, teamId);
      Microsoft.TeamFoundation.Core.WebApi.Team.UpdateTeam newTeamProperties = new Microsoft.TeamFoundation.Core.WebApi.Team.UpdateTeam()
      {
        Name = teamData.Name?.Trim() ?? team.Name,
        Description = teamData.Description?.Trim() ?? team.Description
      };
      try
      {
        service.UpdateTeam(this.TfsRequestContext, teamProject.Id, team.Id, newTeamProperties);
        team.Name = newTeamProperties.Name;
        team.Description = newTeamProperties.Description;
      }
      catch (GroupRenameException ex)
      {
        throw new TeamAlreadyExistsException(ex.Message);
      }
      return this.Request.CreateResponse<WebApiTeam>(HttpStatusCode.OK, team);
    }

    [HttpDelete]
    [ClientLocationId("D30A3DD1-F8BA-442A-B86A-BD0C0C383E59")]
    [ClientExample("DELETE__projects__projectId__teams__newTeamId_.json", null, null, null)]
    public void DeleteTeam(string projectId, string teamId)
    {
      this.CheckProjectIdInputValue<DeleteTeamInputValuesException>(projectId);
      this.CheckTeamIdInputValue<DeleteTeamInputValuesException>(teamId);
      try
      {
        WebApiTeam team = Microsoft.TeamFoundation.Server.Core.TeamsUtility.GetTeam(this.TfsRequestContext, ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId, false, false, false), teamId);
        this.TfsRequestContext.GetService<ITeamService>().DeleteTeam(this.TfsRequestContext, team.Id);
      }
      catch (ProjectDoesNotExistException ex)
      {
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
      }
      catch (Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException ex)
      {
      }
    }

    [NonAction]
    public WebApiTeam GetTeamInProjectInternal(
      string projectId,
      string teamId,
      bool expandIdentity,
      bool setSecuredObject)
    {
      WebApiTeam team = Microsoft.TeamFoundation.Server.Core.TeamsUtility.GetTeam(this.TfsRequestContext, ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId, false, false, false), teamId, setSecuredObject);
      if (!expandIdentity)
        team.Identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      return team;
    }

    [NonAction]
    public IReadOnlyCollection<WebApiTeam> GetTeamsInProjectInternal(
      string projectId,
      bool mine,
      int? top,
      int? skip,
      bool expandIdentity)
    {
      ITeamService service = this.TfsRequestContext.GetService<ITeamService>();
      TeamProject teamProject = ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId, false, false, false);
      int topValue;
      int skipValue;
      ServerCoreApiController.EvaluateTopSkip(top, skip, out topValue, out skipValue);
      return mine ? (IReadOnlyCollection<WebApiTeam>) service.QueryMyTeamsInProject(this.TfsRequestContext, this.TfsRequestContext.UserContext, teamProject.Id).Skip<WebApiTeam>(skipValue).Take<WebApiTeam>(topValue).Select<WebApiTeam, WebApiTeam>((Func<WebApiTeam, WebApiTeam>) (team =>
      {
        if (!expandIdentity)
          team.Identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        return team;
      })).ToList<WebApiTeam>() : (IReadOnlyCollection<WebApiTeam>) service.QueryTeamsInProject(this.TfsRequestContext, teamProject.Id).Skip<WebApiTeam>(skipValue).Take<WebApiTeam>(topValue).Select<WebApiTeam, WebApiTeam>((Func<WebApiTeam, WebApiTeam>) (team =>
      {
        if (!expandIdentity)
          team.Identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        return team;
      })).ToList<WebApiTeam>();
    }

    [NonAction]
    public IReadOnlyCollection<WebApiTeam> GetAllTeamsInternal(
      bool mine,
      int? top,
      int? skip,
      bool expandIdentity)
    {
      int topValue;
      int skipValue;
      ServerCoreApiController.EvaluateTopSkip(top, skip, out topValue, out skipValue);
      ITeamService service = this.TfsRequestContext.GetService<ITeamService>();
      return mine ? (IReadOnlyCollection<WebApiTeam>) service.QueryMyTeamsInCollection(this.TfsRequestContext, this.TfsRequestContext.UserContext).Skip<WebApiTeam>(skipValue).Take<WebApiTeam>(topValue).Select<WebApiTeam, WebApiTeam>((Func<WebApiTeam, WebApiTeam>) (team =>
      {
        if (!expandIdentity)
          team.Identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        return team;
      })).ToList<WebApiTeam>() : (IReadOnlyCollection<WebApiTeam>) service.QueryAllTeamsInCollection(this.TfsRequestContext).Skip<WebApiTeam>(skipValue).Take<WebApiTeam>(topValue).Select<WebApiTeam, WebApiTeam>((Func<WebApiTeam, WebApiTeam>) (team =>
      {
        if (!expandIdentity)
          team.Identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        return team;
      })).ToList<WebApiTeam>();
    }

    internal void CheckProjectIdInputValue<ExceptionTypeToThrow>(string projectId) where ExceptionTypeToThrow : Exception
    {
      if (string.IsNullOrWhiteSpace(projectId))
        throw (object) (ExceptionTypeToThrow) Activator.CreateInstance(typeof (ExceptionTypeToThrow), (object) Microsoft.TeamFoundation.Server.Core.Resources.MissingProjectId());
    }

    internal void CheckTeamIdInputValue<ExceptionTypeToThrow>(string teamId) where ExceptionTypeToThrow : Exception
    {
      if (string.IsNullOrWhiteSpace(teamId))
        throw (object) (ExceptionTypeToThrow) Activator.CreateInstance(typeof (ExceptionTypeToThrow), (object) Microsoft.TeamFoundation.Server.Core.Resources.MissingTeamId());
    }

    internal void CheckWebApiTeamInputValue<ExceptionTypeToThrow>(WebApiTeam webApiTeam) where ExceptionTypeToThrow : Exception
    {
      if (webApiTeam == null)
        throw (object) (ExceptionTypeToThrow) Activator.CreateInstance(typeof (ExceptionTypeToThrow), (object) Microsoft.TeamFoundation.Server.Core.Resources.MissingTeamData());
    }
  }
}
