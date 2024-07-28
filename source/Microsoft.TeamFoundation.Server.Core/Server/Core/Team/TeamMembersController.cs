// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Team.TeamMembersController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.Team
{
  [VersionedApiControllerCustomName("core", "members", 1)]
  [ClientGroupByResource("teams")]
  [ControllerApiVersion(1.0)]
  public class TeamMembersController : ServerCoreApiController
  {
    [ClientResponseType(typeof (IEnumerable<IdentityRef>), null, null)]
    [HttpGet]
    [ClientExample("GET__projects__projectId__teams__teamId__members_.json", null, null, null)]
    public HttpResponseMessage GetTeamMembers(
      string projectId,
      string teamId,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null)
    {
      WebApiTeam team = TeamsUtility.GetTeam(this.TfsRequestContext, ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId, false, false, false), teamId);
      int topValue;
      int skipValue;
      ServerCoreApiController.EvaluateTopSkip(top, skip, out topValue, out skipValue);
      return this.GenerateResponse<IdentityRef>(TeamsUtility.GetTeamMembers(this.TfsRequestContext, team.Id, topValue, skipValue));
    }
  }
}
