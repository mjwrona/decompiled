// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VersionControlController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [SupportedRouteArea(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("2FF0A29B-5679-44f6-8FAD-F5968AE3E32E", true)]
  public class VersionControlController : VersionControlAreaController
  {
    private const int c_maxInitialChangesInJsonIsland = 1000;

    protected override void Initialize(RequestContext requestContext)
    {
      base.Initialize(requestContext);
      // ISSUE: reference to a compiler-generated field
      if (VersionControlController.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VersionControlController.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string[], object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "areaLocations", typeof (VersionControlController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = VersionControlController.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) VersionControlController.\u003C\u003Eo__1.\u003C\u003Ep__0, this.ViewBag, new string[1]
      {
        "tfvc"
      });
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510000, 510010)]
    public ActionResult Index()
    {
      if (!this.RepositoryInfoFactory.HasTfsVersionControl)
        return this.HandleGitRedirect((string) null);
      this.ConfigureLeftHubSplitter(VCResources.SourceExplorerText, "explorer");
      this.CreateRepositoriesPlaceHolder();
      return (ActionResult) this.View("NewExplorer");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510020, 510030)]
    public ActionResult Item() => this.Index();

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510400, 510410)]
    public ActionResult Changeset()
    {
      string fromRouteParameters = this.GetVersionFromRouteParameters(out bool _);
      int num = !string.IsNullOrWhiteSpace(fromRouteParameters) ? VersionSpecCommon.ParseChangesetNumber((VersionSpecFactory) VersionSpec.ServerVersionSpecFactory, fromRouteParameters) : TfsVersionControlProvider.GetChangesetId(this.TfsWebContext.TfsRequestContext, this.Request.Params, false);
      if (num > 0)
      {
        if (this.RepositoryInfoFactory.HasTfsVersionControl)
          return (ActionResult) null;
        this.RouteData.Values.Remove("project");
        this.RouteData.Values.Remove("team");
        this.RouteData.Values.Remove("parameters");
        this.RouteData.Values["id"] = (object) num;
        return (ActionResult) this.RedirectToAction("changeset", this.RouteData.Values);
      }
      return this.RepositoryInfoFactory.HasTfsVersionControl ? (ActionResult) this.RedirectToAction("changesets", (object) new
      {
        routeArea = ""
      }) : this.HandleGitRedirect("commits");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [ValidateInput(false)]
    [TfsTraceFilter(510120, 510130)]
    public ActionResult Shelveset()
    {
      this.GetVersionFromRouteParameters(out bool _);
      string shelvesetId = TfsVersionControlProvider.TryParseShelvesetId(this.TfsRequestContext, this.Request.Params);
      if (this.RepositoryInfoFactory.HasTfsVersionControl)
        return (ActionResult) this.RedirectToAction("shelvesets", (object) new
        {
          routeArea = ""
        });
      return string.IsNullOrEmpty(shelvesetId) ? this.HandleGitRedirect("commits") : (ActionResult) null;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    private ActionResult HandleGitRedirect(string gitRedirectAction)
    {
      if (this.RepositoryInfoFactory.GitRepositories.Any<GitRepository>())
        return (ActionResult) this.Redirect(Microsoft.TeamFoundation.Server.WebAccess.GitUrlGenerator.GetActionUrl(this.TfsWebContext, this.RepositoryInfoFactory.DefaultGitRepositoryName, gitRedirectAction));
      throw new TeamFoundationServerUnauthorizedException(VCServerResources.NoSourceControlAccessError);
    }

    [TfsTraceFilter(510150, 510160)]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult PathTile(string path, string version, Guid? repositoryId)
    {
      Dictionary<string, object> pathTileData = this.GetPathTileData(path, version, repositoryId, 2);
      // ISSUE: reference to a compiler-generated field
      if (VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "TileDisplayName", typeof (VersionControlController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__0, this.ViewBag, pathTileData["displayName"]);
      // ISSUE: reference to a compiler-generated field
      if (VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ChangesetCount", typeof (VersionControlController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__1.Target((CallSite) VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__1, this.ViewBag, pathTileData["changesetCount"]);
      // ISSUE: reference to a compiler-generated field
      if (VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "TileTooltip", typeof (VersionControlController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__2.Target((CallSite) VersionControlController.\u003C\u003Eo__7.\u003C\u003Ep__2, this.ViewBag, pathTileData["tooltip"]);
      return (ActionResult) this.View();
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [ValidateInput(false)]
    public JsonResult PathTileWidget(string path, string version, Guid? repositoryId) => this.Json((object) this.GetPathTileData(path, version, repositoryId, 7), JsonRequestBehavior.AllowGet);

    private Dictionary<string, object> GetPathTileData(
      string path,
      string version,
      Guid? repositoryId,
      int numberOfDays)
    {
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      Dictionary<string, object> pathTileData = new Dictionary<string, object>();
      ChangeListSearchCriteria searchCriteria = new ChangeListSearchCriteria()
      {
        ItemPath = path,
        ItemVersion = version
      };
      ChangeListSearchCriteria listSearchCriteria = searchCriteria;
      DateTime dateTime = this.TfsRequestContext.GetTimeZone().ConvertToLocal(DateTime.UtcNow);
      dateTime = dateTime.Date;
      DateTime fromDate = dateTime.AddDays((double) -numberOfDays);
      listSearchCriteria.SetFromDate(fromDate);
      VersionControlRepositoryInfo controlRepositoryInfo;
      string str1;
      string str2;
      if (repositoryId.HasValue && repositoryId.Value != Guid.Empty)
      {
        GitRepositoryInfo gitRepositoryById = this.RepositoryInfoFactory.GetGitRepositoryById(repositoryId.Value);
        controlRepositoryInfo = (VersionControlRepositoryInfo) gitRepositoryById;
        string name = gitRepositoryById.GitProvider.Repository.Name;
        pathTileData.Add("repositoryName", (object) name);
        string versionDescription = !string.IsNullOrEmpty(version) ? gitRepositoryById.GitProvider.GetVersionDescription(version) : "";
        if (string.IsNullOrEmpty(path) || string.Equals(path, "/"))
        {
          if (this.RepositoryInfoFactory.GitRepositories.Count<GitRepository>() > 1)
          {
            str1 = name;
            if (!string.IsNullOrEmpty(versionDescription))
              str1 = str1 + " (" + versionDescription + ")";
          }
          else
            str1 = versionDescription;
        }
        else
        {
          str1 = controlRepositoryInfo.Provider.GetFileNameFromPath(path);
          if (!string.IsNullOrEmpty(versionDescription))
            str1 = str1 + " (" + versionDescription + ")";
        }
        str2 = string.Format(VCServerResources.RepositoryTileTooltip, (object) path, (object) versionDescription, (object) name);
      }
      else
      {
        controlRepositoryInfo = (VersionControlRepositoryInfo) this.RepositoryInfoFactory.TfsRepositoryInfo;
        str1 = controlRepositoryInfo.Provider.GetFileNameFromPath(path);
        str2 = path;
      }
      HistoryQueryResults historyQueryResults = controlRepositoryInfo.Provider.QueryHistory(searchCriteria);
      pathTileData.Add("changesetCount", (object) historyQueryResults.Results.Count<HistoryEntry>());
      pathTileData.Add("displayName", (object) str1);
      pathTileData.Add("tooltip", (object) str2);
      return pathTileData;
    }

    private void CreateRepositoriesPlaceHolder()
    {
      if (this.TfsWebContext.Project == null)
        return;
      this.RepositoryName = this.RepositoryInfoFactory.GetTfvcRepositoryName();
    }

    private VersionControlViewModel CreateViewModel()
    {
      if (this.TfsWebContext.Project != null)
        this.RepositoryName = this.RepositoryInfoFactory.GetTfvcRepositoryName();
      return VersionControlViewModel.Create(this.RepositoryInfoFactory);
    }
  }
}
