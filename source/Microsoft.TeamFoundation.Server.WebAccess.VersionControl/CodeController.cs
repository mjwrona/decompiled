// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.CodeController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [SupportedRouteArea(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("2FF0A29B-5679-44f6-8FAD-F5968AE3E32E", true)]
  public class CodeController : VersionControlAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index(string toRouteId = null, string vcType = null) => this.RedirectToContributedHubOrDefault(toRouteId, vcType);

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Branches() => this.RedirectToDefaultRepository("branches", "changesets");

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult PullRequest() => this.RedirectToDefaultRepository("pullrequests", "index");

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult PullRequestCreate() => this.RedirectToDefaultRepository("pullrequestcreate", "index");

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult PullRequestReview() => this.RedirectToDefaultRepository("pullrequests", "index");

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult PullRequests() => this.RedirectToDefaultRepository("pullrequests", "index");

    private ActionResult RedirectToDefaultRepository(
      string gitRedirectAction,
      string tfvcRedirectAction)
    {
      return this.RepositoryInfoFactory.DefaultRepositoryIsTfvc ? this.RedirectToTfvc(tfvcRedirectAction) : this.RedirectToGit(gitRedirectAction);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    private ActionResult RedirectToGit(string gitRedirectAction)
    {
      if (this.RepositoryInfoFactory.GitRepositories.Any<GitRepository>())
        return (ActionResult) this.Redirect(GitUrlGenerator.GetActionUrl(this.TfsWebContext, this.RepositoryInfoFactory.DefaultRepositoryName, gitRedirectAction));
      throw new TeamFoundationServerUnauthorizedException(VCServerResources.NoSourceControlAccessError);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    private ActionResult RedirectToTfvc(string tfvcRedirectAction)
    {
      if (this.RepositoryInfoFactory.HasTfsVersionControl)
        return (ActionResult) this.RedirectToAction(tfvcRedirectAction, "versionControl", this.RouteData.Values);
      throw new TeamFoundationServerUnauthorizedException(VCServerResources.NoSourceControlAccessError);
    }

    private ActionResult RedirectToContributedHubOrDefault(string routeId, string vcType)
    {
      if (string.IsNullOrEmpty(routeId) || string.IsNullOrEmpty(vcType))
        return this.RedirectToDefaultRepository((string) null, (string) null);
      RouteValueDictionary routeValues = new RouteValueDictionary()
      {
        {
          "project",
          (object) this.TfsWebContext.NavigationContext.Project
        },
        {
          "team",
          (object) this.TfsWebContext.NavigationContext.Team
        }
      };
      vcType = vcType.ToLowerInvariant();
      return this.RepositoryInfoFactory.DefaultRepositoryIsTfvc && vcType == "tfvc" || this.RepositoryInfoFactory.DefaultRepositoryIsGit && vcType == "git" ? (ActionResult) this.Redirect(NavigationHelpers.GetHubDefaultUrl((WebContext) this.TfsWebContext, routeId, routeValues)) : this.RedirectToDefaultRepository((string) null, (string) null);
    }
  }
}
