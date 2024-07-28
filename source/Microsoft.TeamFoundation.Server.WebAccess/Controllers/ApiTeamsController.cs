// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.ApiTeamsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.ApplicationAll)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiTeamsController : TfsController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Members(
      Guid teamId,
      bool? includeGroups,
      int? maxResults,
      bool? randomize)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      if (!includeGroups.HasValue)
        includeGroups = new bool?(false);
      if (!maxResults.HasValue)
        maxResults = new int?(100);
      ITeamService service = this.TfsRequestContext.GetService<ITeamService>();
      IEnumerable<IdentityRef> source = service.ReadTeamMembers(this.TfsRequestContext, service.GetTeamInProject(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid, teamId.ToString()).Identity, MembershipQuery.Expanded).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) (member => member.ToIdentityRef(this.TfsRequestContext)));
      if (!includeGroups.Value)
        source = source.Where<IdentityRef>((Func<IdentityRef, bool>) (id => !id.IsContainer));
      int num = source.Count<IdentityRef>();
      if (randomize.HasValue && randomize.Value)
      {
        if (maxResults.Value >= 0 && maxResults.Value != int.MaxValue && maxResults.Value > num)
        {
          Random random = new Random();
          source = source.OrderBy<IdentityRef, int>((Func<IdentityRef, int>) (x => random.Next())).Take<IdentityRef>(maxResults.Value);
        }
      }
      else if (maxResults.Value >= 0 && maxResults.Value != int.MaxValue)
        source = source.OrderBy<IdentityRef, string>((Func<IdentityRef, string>) (id => id.DisplayName), (IComparer<string>) StringComparer.OrdinalIgnoreCase).Take<IdentityRef>(maxResults.Value);
      return (ActionResult) this.Json((object) new
      {
        count = num,
        members = source.Select<IdentityRef, JsObject>((Func<IdentityRef, JsObject>) (id => id.ToJson()))
      }, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult SetDefaultTeam(Guid? teamId)
    {
      if (!teamId.HasValue)
        teamId = (this.NavigationContext.TopMostLevel | NavigationContextLevels.Team) == NavigationContextLevels.None ? new Guid?(Guid.Empty) : new Guid?(this.TfsWebContext.Team.Id);
      ArgumentUtility.CheckForEmptyGuid(teamId.Value, nameof (teamId));
      this.TfsRequestContext.GetService<ITeamService>().SetDefaultTeamId(this.TfsRequestContext, this.TfsWebContext.Project.Id, teamId.Value);
      return (ActionResult) this.Json((object) new
      {
        success = true
      });
    }
  }
}
