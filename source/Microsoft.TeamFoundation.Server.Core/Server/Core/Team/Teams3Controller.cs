// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Team.Teams3Controller
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.Team
{
  [VersionedApiControllerCustomName("core", "teams", 3)]
  [ControllerApiVersion(5.1)]
  public class Teams3Controller : TeamsControllerBase
  {
    [ClientResponseType(typeof (WebApiTeam), null, null)]
    [HttpGet]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    [ClientLocationId("D30A3DD1-F8BA-442A-B86A-BD0C0C383E59")]
    [ClientExample("GET__projects__projectId__teams__teamId_.json", null, null, null)]
    public HttpResponseMessage GetTeam(string projectId, string teamId, [FromUri(Name = "$expandIdentity")] bool expandIdentity = false) => this.Request.CreateResponse<WebApiTeam>(HttpStatusCode.OK, this.GetTeamInProjectInternal(projectId, teamId, expandIdentity, true));

    [ClientResponseType(typeof (IEnumerable<WebApiTeam>), null, null)]
    [HttpGet]
    [ClientLocationId("D30A3DD1-F8BA-442A-B86A-BD0C0C383E59")]
    [ClientExample("GET__projects__projectId__teams.json", null, null, null)]
    public HttpResponseMessage GetTeams(
      string projectId,
      [FromUri(Name = "$mine")] bool mine = false,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$expandIdentity")] bool expandIdentity = false)
    {
      return this.GenerateResponse<WebApiTeam>((IEnumerable<WebApiTeam>) this.GetTeamsInProjectInternal(projectId, mine, top, skip, expandIdentity));
    }

    [ClientResponseType(typeof (IEnumerable<WebApiTeam>), null, null)]
    [HttpGet]
    [ClientLocationId("7A4D9EE9-3433-4347-B47A-7A80F1CF307E")]
    [ClientExample("GET__teams__mine-_mine.json", null, null, null)]
    public HttpResponseMessage GetAllTeams([FromUri(Name = "$mine")] bool mine = false, [FromUri(Name = "$top")] int? top = null, [FromUri(Name = "$skip")] int? skip = null, [FromUri(Name = "$expandIdentity")] bool expandIdentity = false) => this.GenerateResponse<WebApiTeam>((IEnumerable<WebApiTeam>) this.GetAllTeamsInternal(mine, top, skip, expandIdentity));
  }
}
