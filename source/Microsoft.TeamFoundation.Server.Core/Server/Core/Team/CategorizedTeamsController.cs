// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Team.CategorizedTeamsController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.Team
{
  [VersionedApiControllerCustomName("core", "categorizedTeams", 1)]
  [ControllerApiVersion(7.1)]
  public class CategorizedTeamsController : ServerCoreApiController
  {
    private CategorizedTeamsController.WebApiTeamComparer _teamComparer = new CategorizedTeamsController.WebApiTeamComparer();

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

    [ClientResponseType(typeof (CategorizedWebApiTeams), null, null)]
    [HttpGet]
    [ClientLocationId("6F9619FF-8B86-D011-B42D-00C04FC964FF")]
    public HttpResponseMessage GetProjectTeamsByCategory(
      string projectId,
      [FromUri(Name = "$expandIdentity")] bool expandIdentity = false,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null)
    {
      return this.Request.CreateResponse<CategorizedWebApiTeams>(HttpStatusCode.OK, this.GetTeamsByCategoryInternal(projectId, top, skip, expandIdentity));
    }

    [NonAction]
    public CategorizedWebApiTeams GetTeamsByCategoryInternal(
      string projectId,
      int? top,
      int? skip,
      bool expandIdentity)
    {
      int topValue;
      int skipValue;
      ServerCoreApiController.EvaluateTopSkip(top, skip, out topValue, out skipValue);
      TeamProject teamProject = ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId, false, false, false);
      ITeamService service = this.TfsRequestContext.GetService<ITeamService>();
      IEnumerable<WebApiTeam> webApiTeams1 = service.QueryTeamsInProject(this.TfsRequestContext, teamProject.Id).Skip<WebApiTeam>(skipValue).Take<WebApiTeam>(topValue).Select<WebApiTeam, WebApiTeam>((Func<WebApiTeam, WebApiTeam>) (team =>
      {
        if (!expandIdentity)
          team.Identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        return team;
      }));
      IReadOnlyCollection<WebApiTeam> webApiTeams2 = service.QueryMyTeamsInCollection(this.TfsRequestContext, this.TfsRequestContext.UserContext);
      List<WebApiTeam> list = webApiTeams1.Except<WebApiTeam>((IEnumerable<WebApiTeam>) webApiTeams2, (IEqualityComparer<WebApiTeam>) this._teamComparer).ToList<WebApiTeam>();
      return new CategorizedWebApiTeams((IList<WebApiTeam>) webApiTeams2.Intersect<WebApiTeam>(webApiTeams1, (IEqualityComparer<WebApiTeam>) this._teamComparer).ToList<WebApiTeam>(), (IList<WebApiTeam>) list);
    }

    private class WebApiTeamComparer : IEqualityComparer<WebApiTeam>
    {
      public bool Equals(WebApiTeam x, WebApiTeam y) => x.Id == y.Id;

      public int GetHashCode(WebApiTeam obj) => obj.Id.GetHashCode();
    }
  }
}
