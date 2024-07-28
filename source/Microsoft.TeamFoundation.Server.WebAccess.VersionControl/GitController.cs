// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.GitController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.SignalR;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Presentation;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [SupportedRouteArea(NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("2FF0A29B-5679-44f6-8FAD-F5968AE3E32E", true)]
  public class GitController : VersionControlAreaController
  {
    private const int c_maxInitialChangesInJsonIsland = 1000;
    private string m_gitArea;

    protected override void Initialize(RequestContext requestContext)
    {
      base.Initialize(requestContext);
      this.RepositoryName = this.RouteData.GetRouteValue<string>("GitRepositoryName", (string) null);
      this.m_gitArea = this.RouteData.GetRouteValue<string>("GitArea", "_git");
      NavigationExtensions.SkipHubGroupMruUpdate(this.ViewData);
      // ISSUE: reference to a compiler-generated field
      if (GitController.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitController.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string[], object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "areaLocations", typeof (GitController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = GitController.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) GitController.\u003C\u003Eo__1.\u003C\u003Ep__0, this.ViewBag, new string[1]
      {
        "git"
      });
    }

    protected override void OnException(ExceptionContext filterContext)
    {
      if (filterContext.Exception is GitRepositoryNotFoundException || filterContext.Exception is TeamFoundationServerUnauthorizedException)
        throw new HttpException(404, filterContext.Exception.Message);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Empty()
    {
      this.RouteData.Values["SkipDefaultBranchCheck"] = (object) true;
      return (ActionResult) this.View(nameof (Empty));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510000, 510010)]
    public ActionResult Index() => this.Explorer(this.IsEmptyRepository() ? "Explorer" : "NewExplorer");

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510000, 510010)]
    public ActionResult OldExplorer() => this.Explorer("Explorer");

    private ActionResult Explorer(string viewName)
    {
      ActionResult actionResult = this.CheckForRedirect("index", (string) null);
      if (actionResult != null)
        return actionResult;
      using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "GitController.Index.ConfigureSplitter"))
        this.ConfigureLeftHubSplitter(VCResources.SourceExplorerText, "explorer", hidePane: FullScreenHelper.IsFullScreen(this.TfsWebContext));
      using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "GitController.Index.ConfigureSearchBox"))
      {
        if (this.IsSearchEnabled())
          this.ConfigureSearchBox("search-adapter-search");
      }
      return (ActionResult) this.View(viewName);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510300, 510302)]
    public ActionResult Commit(Guid? repoId) => this.CommitInternal(repoId, "NewChangeList");

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510323, 510324)]
    public ActionResult Push()
    {
      string routeValue = this.RouteData.GetRouteValue<string>("parameters", (string) null);
      ActionResult actionResult = this.CheckForRedirect("pushes", "changesets");
      if (actionResult != null)
        return actionResult;
      int result;
      if (string.IsNullOrEmpty(routeValue) || !int.TryParse(routeValue, out result))
        return (ActionResult) this.Redirect(GitUrlGenerator.GetActionUrl(this.TfsWebContext, this.RepositoryName, "pushes"));
      // ISSUE: reference to a compiler-generated field
      if (GitController.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitController.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "PushId", typeof (GitController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = GitController.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) GitController.\u003C\u003Eo__8.\u003C\u003Ep__0, this.ViewBag, result);
      this.ConfigureLeftHubSplitter(VCServerResources.PushExplorerText, "GitPush");
      return (ActionResult) this.View(nameof (Push));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510303, 510304)]
    public ActionResult BranchesOld()
    {
      ActionResult actionResult = this.CheckForRedirect("branches", "changesets");
      if (actionResult != null)
        return actionResult;
      this.ConfigureLeftHubSplitter(VCServerResources.BranchExplorerText, "branches-history", true);
      return (ActionResult) this.View("Branches");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510320, 510321)]
    public ActionResult Branches()
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("WebAccess.VersionControl.NewBranches"))
        return this.BranchesOld();
      ActionResult actionResult = this.CheckForRedirect("branches", "changesets");
      if (actionResult != null)
        return actionResult;
      this.RouteData.Values["SkipDefaultBranchCheck"] = (object) true;
      return (ActionResult) this.View("NewBranches");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510320, 510321)]
    public ActionResult NewBranches() => (ActionResult) this.Redirect(GitUrlGenerator.GetActionUrl(this.TfsWebContext, this.RepositoryName, "branches"));

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510307, 510319)]
    public ActionResult PullRequest(bool? legacy = false) => !this.IsPullRequestValid() ? (ActionResult) this.Redirect(GitUrlGenerator.GetActionUrl(this.TfsWebContext, this.RepositoryName, "pullrequests")) : this.PullRequestReviewResult();

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510309, 510311)]
    public ActionResult PullRequests()
    {
      this.ConfigureLeftHubSplitter(VCServerResources.PullRequest_Filter_Title, "pullRequests-filter");
      this.ConfigureRightHubSplitter();
      this.RouteData.Values["SkipDefaultBranchCheck"] = (object) true;
      return (ActionResult) this.View(nameof (PullRequests));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510325, 510326)]
    public ActionResult PullRequestCreate()
    {
      this.RouteData.Values["SkipDefaultBranchCheck"] = (object) true;
      return (ActionResult) this.View(nameof (PullRequestCreate));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    private ActionResult CheckForRedirect(string gitRedirectAction, string tfsRedirectAction)
    {
      using (PerformanceTimer performanceTimer = WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "GitController.CheckForRedirect"))
      {
        performanceTimer.AddProperty("HasRepositoryName", (object) !string.IsNullOrEmpty(this.RepositoryName));
        if (string.IsNullOrEmpty(this.RepositoryName))
          return (ActionResult) this.Redirect(GitUrlGenerator.GetActionUrl(this.TfsWebContext, this.RepositoryInfoFactory.DefaultGitRepositoryName, gitRedirectAction));
        if (!this.m_gitArea.Equals("_git"))
          return (ActionResult) this.Redirect(GitUrlGenerator.GetActionUrl(this.TfsWebContext, this.RepositoryName, gitRedirectAction));
        try
        {
          this.RepositoryInfoFactory.GetRepositoryInfo(this.RepositoryName, new VersionControlRepositoryType?(VersionControlRepositoryType.Git));
        }
        catch (GitRepositoryNotFoundException ex)
        {
          string gitRepositoryName = this.RepositoryInfoFactory.DefaultGitRepositoryName;
          if (this.RepositoryInfoFactory.GitRepositories.Any<GitRepository>() && !StringComparer.OrdinalIgnoreCase.Equals(this.RepositoryName, gitRepositoryName))
            return (ActionResult) this.Redirect(GitUrlGenerator.GetActionUrl(this.TfsWebContext, gitRepositoryName, gitRedirectAction));
          if (this.RepositoryInfoFactory.HasTfsVersionControl)
            return (ActionResult) this.RedirectToAction(tfsRedirectAction, "versionControl", (object) new
            {
              routeArea = ""
            });
          this.TfsWebContext.TfsRequestContext.TraceCatch(1013589, this.TraceArea, TfsTraceLayers.Controller, (Exception) ex);
          throw new TeamFoundationServerUnauthorizedException(VCServerResources.NoSourceControlAccessError);
        }
        return (ActionResult) null;
      }
    }

    private void AddSelectedTab()
    {
      string str = this.Request.QueryString["_a"];
      if (string.IsNullOrEmpty(str))
        str = this.Request.QueryString["view"];
      if (!string.IsNullOrEmpty(str))
        str = str.ToLowerInvariant();
      if (str == "compare")
        str = "files";
      else if (str != "files" && str != "comments" && str != "commits")
        str = "overview";
      this.ViewData["SelectedTab"] = (object) (char.ToUpper(str[0]).ToString() + str.Substring(1));
    }

    private void AddSignalRData()
    {
      this.ViewData["SignalrHubUrl"] = (object) TfsSignalRUtility.GetHubsUrl(this.TfsRequestContext);
      this.ViewData["SignalrConnectionUrl"] = (object) TfsSignalRUtility.GetConnectionUrl(this.TfsRequestContext);
    }

    private VersionControlViewModel CreateViewModel(bool checkDefaultGitBranch = true)
    {
      using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "GitController.CreateViewModel"))
      {
        VersionControlViewModel viewModel = VersionControlViewModel.Create(this.RepositoryInfoFactory, ((GitRepositoryInfo) this.RepositoryInfoFactory.GetRepositoryInfo(this.RepositoryName, new VersionControlRepositoryType?(VersionControlRepositoryType.Git))).GitProvider.GetRepository(checkDefaultGitBranch));
        if (checkDefaultGitBranch)
        {
          using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "GitController.CreateViewModel.CheckDefaultGitBranch"))
            this.CheckDefaultGitBranch(viewModel);
        }
        return viewModel;
      }
    }

    private void CheckDefaultGitBranch(VersionControlViewModel viewModel)
    {
      TfsGitRef tfsGitRef = this.GetGitVcProviderById(viewModel.Repository.Id).Repository.Refs.MatchingName("refs/heads/" + viewModel.DefaultGitBranchName);
      viewModel.DeletedUserDefaultBranchName = (string) null;
      if (tfsGitRef != null)
        return;
      viewModel.DeletedUserDefaultBranchName = viewModel.DefaultGitBranchName;
      viewModel.DefaultGitBranchName = GitUtils.GetFriendlyBranchName(viewModel.Repository.DefaultBranch);
      this.TfsWebContext.TfsRequestContext.GetService<ISettingsService>().SetValue(this.TfsWebContext.TfsRequestContext, SettingsUserScope.User, "Repository", viewModel.Repository.Id.ToString(), "Git/DefaultUserBranch", (object) viewModel.DefaultGitBranchName);
    }

    private bool IsEmptyRepository()
    {
      GitRepositoryInfo repositoryInfo = (GitRepositoryInfo) this.RepositoryInfoFactory.GetRepositoryInfo(this.RepositoryName, new VersionControlRepositoryType?(VersionControlRepositoryType.Git));
      return string.IsNullOrEmpty(repositoryInfo.GitProvider.GetDefaultBranchName(true, repositoryInfo.GitProvider.GetRepository(true).DefaultBranch));
    }

    private ActionResult CommitInternal(Guid? repoId, string viewName)
    {
      bool reviewMode;
      string fromRouteParameters = this.GetVersionFromRouteParameters(out reviewMode);
      if (repoId.HasValue)
      {
        GitRepositoryInfo gitRepositoryById = this.RepositoryInfoFactory.GetGitRepositoryById(repoId.Value);
        string name1 = gitRepositoryById.GitProvider.Repository.Name;
        string name2 = this.TfsWebContext.ProjectContext == null ? (string) null : this.TfsWebContext.ProjectContext.Name;
        string projectUri = gitRepositoryById.GitProvider.Repository.Key.GetProjectUri();
        if (this.TfsWebContext.CurrentProjectUri == (Uri) null || !string.Equals(this.TfsWebContext.CurrentProjectUri.AbsoluteUri, projectUri, StringComparison.OrdinalIgnoreCase))
          name2 = TfsProjectHelpers.GetProject(this.TfsRequestContext, new Uri(projectUri)).Name;
        return (ActionResult) this.Redirect(GitUrlGenerator.GetActionUrl(this.TfsWebContext, name2, name1, "commit", (object) new
        {
          parameters = fromRouteParameters
        }));
      }
      ActionResult actionResult = this.CheckForRedirect("commits", "changesets");
      if (actionResult != null)
        return actionResult;
      if (string.IsNullOrEmpty(fromRouteParameters))
        return (ActionResult) this.Redirect(GitUrlGenerator.GetActionUrl(this.TfsWebContext, this.RepositoryName, "commits"));
      GitRepositoryInfo repositoryInfo = (GitRepositoryInfo) this.RepositoryInfoFactory.GetRepositoryInfo(this.RepositoryName, new VersionControlRepositoryType?(VersionControlRepositoryType.Git));
      string str = (string) null;
      string versionDescription = (string) null;
      if (!repositoryInfo.GitProvider.TryGetCommitFromVersion(ref str, fromRouteParameters, out versionDescription, out TfsGitCommit _))
        return (ActionResult) this.Redirect(GitUrlGenerator.GetActionUrl(this.TfsWebContext, this.RepositoryName, "commits"));
      this.ConfigureLeftHubSplitter(VCServerResources.CommitExplorerText, reviewMode ? "GitCommitReview" : "GitCommit");
      if (viewName.Equals("NewChangeList"))
      {
        if (this.TfsRequestContext.IsFeatureEnabled("SourceControl.CherryPick") || this.TfsRequestContext.IsFeatureEnabled("SourceControl.Revert"))
        {
          using (WebPerformanceTimerHelpers.StartMeasure((WebContext) this.TfsWebContext, "GitController.Commit.AddSignalRData"))
            this.AddSignalRData();
        }
        this.AddSelectedTabDataForCommit();
      }
      this.ViewData["ReviewMode"] = this.RouteData.Values["ReviewMode"] = (object) reviewMode;
      return (ActionResult) this.View(viewName);
    }

    private void AddSelectedTabDataForCommit() => this.ViewData["SelectedTab"] = (object) this.Request.QueryString["_a"];

    public static int? GetPullRequestIdFromRouteParameters(RouteData routeData)
    {
      string routeValue = routeData.GetRouteValue<string>("parameters", (string) null);
      if (!string.IsNullOrEmpty(routeValue))
      {
        int num = routeValue.LastIndexOf('/');
        int result;
        if (int.TryParse(num < 0 ? routeValue : routeValue.Substring(num + 1), out result))
          return new int?(result);
      }
      return new int?();
    }

    private ActionResult PullRequestDetailResult()
    {
      this.ConfigureLeftHubSplitter(VCResources.PullRequest_Details_Title, "pullRequest-details-filter");
      this.AddSignalRData();
      this.ConfigureRightHubSplitter();
      this.RouteData.Values["SkipDefaultBranchCheck"] = (object) true;
      return (ActionResult) this.View("PullRequestDetails");
    }

    private ActionResult PullRequestReviewResult()
    {
      using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "GitController.PullRequestDetail.ConfigurePage"))
      {
        this.ConfigureLeftHubSplitter(VCResources.PullRequest_Details_Title, "pullRequest-details-filter");
        this.AddSignalRData();
        this.AddSelectedTab();
      }
      this.RouteData.Values["SkipDefaultBranchCheck"] = (object) true;
      return (ActionResult) this.View("PullRequestReview");
    }

    private bool IsPullRequestValid()
    {
      using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "GitController.PullRequestDetail.HasPullRequest"))
      {
        int? fromRouteParameters = GitController.GetPullRequestIdFromRouteParameters(this.RouteData);
        GitRepositoryInfo repositoryInfo = (GitRepositoryInfo) this.RepositoryInfoFactory.GetRepositoryInfo(this.RepositoryName, new VersionControlRepositoryType?(VersionControlRepositoryType.Git));
        return fromRouteParameters.HasValue && repositoryInfo != null && this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().HasPullRequest(this.TfsRequestContext, repositoryInfo.GitProvider.Repository, fromRouteParameters.Value);
      }
    }
  }
}
