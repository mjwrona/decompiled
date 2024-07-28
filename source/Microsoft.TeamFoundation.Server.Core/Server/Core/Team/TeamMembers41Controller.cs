// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Team.TeamMembers41Controller
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.Team
{
  [VersionedApiControllerCustomName("core", "members", 2)]
  [ClientGroupByResource("teams")]
  [ControllerApiVersion(4.1)]
  public class TeamMembers41Controller : ServerCoreApiController
  {
    [ClientResponseType(typeof (IEnumerable<TeamMember>), null, null)]
    [HttpGet]
    [ClientExample("GET__projects__projectId__teams__teamId__members4.1_.json", null, null, null)]
    public HttpResponseMessage GetTeamMembersWithExtendedProperties(
      string projectId,
      string teamId,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null)
    {
      ITeamService service = this.TfsRequestContext.GetService<ITeamService>();
      WebApiTeam team = TeamsUtility.GetTeam(this.TfsRequestContext, ProjectsUtility.GetTeamProject(this.TfsRequestContext, projectId, false, false, false), teamId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Microsoft.VisualStudio.Services.Identity.Identity identity = team.Identity;
      IReadOnlyCollection<Microsoft.VisualStudio.Services.Identity.Identity> teamAdmins = service.GetTeamAdmins(tfsRequestContext, identity);
      int topValue;
      int skipValue;
      ServerCoreApiController.EvaluateTopSkip(top, skip, out topValue, out skipValue);
      return this.GenerateResponse<TeamMember>(this.BuildTeamMembersCollection(this.TfsRequestContext, TeamsUtility.GetTeamMembers(this.TfsRequestContext, team.Id, topValue, skipValue), (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) teamAdmins));
    }

    internal IEnumerable<TeamMember> BuildTeamMembersCollection(
      IVssRequestContext requestContext,
      IEnumerable<IdentityRef> members,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> teamAdmins)
    {
      if (members == null || !members.Any<IdentityRef>())
        return (IEnumerable<TeamMember>) new List<TeamMember>();
      HashSet<string> adminIds = this.GetTeamAdminIds(requestContext, teamAdmins);
      return members.Select<IdentityRef, TeamMember>((Func<IdentityRef, TeamMember>) (member => new TeamMember()
      {
        Identity = member,
        IsTeamAdmin = adminIds.Contains(member.Id)
      }));
    }

    protected virtual HashSet<string> GetTeamAdminIds(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> teamAdmins)
    {
      HashSet<string> collection = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.Guid);
      if (teamAdmins != null && teamAdmins.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
        collection.AddRange<string, HashSet<string>>(teamAdmins.Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (admin => admin.ToIdentityRef(requestContext).Id)));
      return collection;
    }
  }
}
