// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SignalR;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  [SupportedRouteArea(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  public class BuildController : BuildAreaController
  {
    private const string c_allBuildsTabAction = "allBuilds";
    private const string c_queuedTabAction = "queued";
    private const string c_definitionsIndexView = "definitions";
    private const string c_defintionIdQueryParameter = "definitionId";
    private const string c_editBuildDefinitionAction = "edit-build-definition";
    private const string c_newEditorGettingStartedAction = "build-definition-getting-started";
    private const string c_newDefintionAction = "new";
    private const string c_othersSource = "others";
    private const string c_editBuildDefinitonActionFormat = "{0}?{1}";
    private const string HideBuildExplorerWarningBannerCookieKey = "Tfs-HideBuildExplorerWarningBanner";

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult Explorer() => this.ExplorerInternal();

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult Index()
    {
      if (!this.TfsWebContext.IsFeatureAvailable("WebAccess.Build.Definitions"))
        return this.ExplorerInternal();
      if (this.IsNewDefinitionAction())
        return (ActionResult) this.Redirect(this.GetEditorUrlForCreateDefinition());
      string viewName = "definitions";
      int? nullable = this.GetInt32Parameter("buildId");
      int? int32Parameter1 = this.GetInt32Parameter("definitionId");
      if (!nullable.HasValue && !int32Parameter1.HasValue)
      {
        string uri = this.Request.Params["buildUri"];
        if (!string.IsNullOrEmpty(uri))
        {
          int result;
          if (int.TryParse(LinkingUtilities.DecodeUri(uri).ToolSpecificId, out result))
            nullable = new int?(result);
        }
        else
        {
          int? int32Parameter2 = this.GetInt32Parameter("favDefinitionId");
          if (int32Parameter2.HasValue)
          {
            RouteValueDictionary routeValues = this.CopyRouteValues(this.Request.QueryString);
            routeValues["definitionId"] = (object) int32Parameter2;
            routeValues.Remove("favDefinitionId");
            return (ActionResult) new ClientRedirectResult(this.Url.Action("index", routeValues));
          }
        }
      }
      if (nullable.HasValue)
      {
        string str = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Build", nullable.ToString()));
        using (TeamFoundationDataReader foundationDataReader = this.TfsRequestContext.GetService<ITeamFoundationBuildService>().QueryBuildsByUri(this.TfsRequestContext, (IList<string>) new string[1]
        {
          str
        }, (IList<string>) null, QueryOptions.None, Microsoft.TeamFoundation.Build.Server.QueryDeletedOption.IncludeDeleted, xamlBuildsOnly: true))
        {
          if (foundationDataReader.Current<BuildQueryResult>().Builds.FirstOrDefault<BuildDetail>() != null)
            return this.RedirectToXaml();
        }
        viewName = "buildDetail";
      }
      else if (int32Parameter1.HasValue)
      {
        string str = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Definition", int32Parameter1.ToString()));
        BuildDefinitionQueryResult definitionQueryResult = this.TfsRequestContext.GetService<ITeamFoundationBuildService>().QueryBuildDefinitionsByUri(this.TfsRequestContext, (IList<string>) new string[1]
        {
          str
        }, (IList<string>) null, QueryOptions.None);
        if (definitionQueryResult.Definitions.Count == 1 && definitionQueryResult.Definitions[0] != null)
          return this.RedirectToXaml();
        viewName = "definition";
      }
      if (!string.IsNullOrEmpty(this.Request.Params["templateid"]))
        return this.RedirectToDefinitionEditor();
      this.AddSignalRData();
      string a = this.Request.Params["_a"];
      if (viewName == "definitions" && string.Equals(a, "queued", StringComparison.OrdinalIgnoreCase) && this.TfsWebContext.IsFeatureAvailable("WebAccess.Build.Definitions.AllBuilds"))
      {
        RouteValueDictionary routeValues = this.CopyRouteValues(this.Request.QueryString);
        routeValues["_a"] = (object) "allBuilds";
        return (ActionResult) new ClientRedirectResult(this.Url.Action("definitions", routeValues));
      }
      if (this.TfsWebContext.IsFeatureAvailable("WebAccess.Build.Definitions.All"))
        this.ViewData["BuildAllDefinitionsExtensionAvailable"] = (object) true;
      return (ActionResult) this.View(viewName);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult DefinitionEditor() => this.Request.QueryString.Count == 0 ? (ActionResult) this.RedirectToAction("index") : (ActionResult) this.Redirect(this.GetEditorUrlForEditDefinition(BuildHelpers.ExtractBuildParameterFromRequest(this.Request, "definitionId", false)));

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult Definition() => this.Index();

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult Definitions() => this.Index();

    [FeatureEnabled("Build.XamlHub")]
    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult Xaml()
    {
      this.AddSignalRData();
      string viewName = nameof (Xaml);
      if (this.ClientHostCapabilities is IdeClientHostCapabilities)
        viewName = "XamlIde";
      return (ActionResult) this.View(viewName);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(515020, 515030)]
    public ActionResult Detail()
    {
      string buildUriFromRequest = BuildHelpers.ExtractBuildUriFromRequest(this.Request);
      RouteValueDictionary routeValues = new RouteValueDictionary((object) new
      {
        routeArea = ""
      });
      if (this.TfsWebContext.NavigationContext.TopMostLevel == NavigationContextLevels.Collection)
      {
        TeamFoundationBuildService xamlBuildService = this.XamlBuildService;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        List<string> uris = new List<string>();
        uris.Add(buildUriFromRequest);
        Guid projectId = new Guid();
        using (TeamFoundationDataReader foundationDataReader = xamlBuildService.QueryBuildsByUri(tfsRequestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.None, Microsoft.TeamFoundation.Build.Server.QueryDeletedOption.ExcludeDeleted, projectId, false))
        {
          string teamProject = (foundationDataReader.Current<BuildQueryResult>().Builds.FirstOrDefault<BuildDetail>() ?? throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildResources.BuildNotFound, (object) buildUriFromRequest))).TeamProject;
          routeValues["project"] = (object) teamProject;
        }
      }
      int result;
      if (!int.TryParse(LinkingUtilities.DecodeUri(buildUriFromRequest).ToolSpecificId, out result))
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildResources.BuildNotFound, (object) buildUriFromRequest));
      string clientHost = this.ClientHost;
      if (!string.IsNullOrEmpty(clientHost))
        routeValues["clienthost"] = (object) clientHost;
      routeValues["buildId"] = (object) result;
      routeValues["_a"] = (object) "summary";
      return (ActionResult) new ClientRedirectResult(this.Url.Action("index", routeValues));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult Artifact()
    {
      string resourceType = this.Request.Params["type"];
      string resourceData = this.Request.Params["resource"];
      if (!string.IsNullOrEmpty(resourceType) && !string.IsNullOrEmpty(resourceData))
      {
        IBuildArtifactProviderService service = this.TfsRequestContext.GetService<IBuildArtifactProviderService>();
        IArtifactProvider artifactProvider = (IArtifactProvider) null;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string artifactResourceType = resourceType;
        ref IArtifactProvider local = ref artifactProvider;
        string url;
        if (service.TryGetArtifactProvider(tfsRequestContext, artifactResourceType, out local) && artifactProvider.TryGetWebUrl(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid, resourceType, resourceData, out url))
          return (ActionResult) new ClientRedirectResult(url);
      }
      return (ActionResult) new ClientRedirectResult(this.Url.Action("index"));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(515040, 515050)]
    public ActionResult XamlDefinitionTile(string definitionUri)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      ArgumentUtility.CheckForNull<string>(definitionUri, nameof (definitionUri));
      Microsoft.TeamFoundation.Build.Server.BuildDefinition definition = (Microsoft.TeamFoundation.Build.Server.BuildDefinition) null;
      BuildDetail build1 = (BuildDetail) null;
      if (!BuildHelpers.TryGetXamlBuildDefinition(this.TfsRequestContext, definitionUri, out definition))
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildServerResources.BuildDefinitionNotFound, (object) definitionUri));
      BuildDetailSpec buildDetailSpec = new BuildDetailSpec()
      {
        DefinitionFilter = (object) new BuildDefinitionSpec()
        {
          FullPath = definition.FullPath
        },
        DefinitionFilterType = DefinitionFilterType.DefinitionSpec,
        QueryOrder = Microsoft.TeamFoundation.Build.Server.BuildQueryOrder.FinishTimeDescending,
        QueryOptions = QueryOptions.Definitions,
        Status = Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded | Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded | Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed,
        MaxBuildsPerDefinition = 1,
        MinFinishTime = DateTime.MinValue,
        BuildNumber = "*"
      };
      this.Trace(515042, TraceLevel.Info, "Querying last build of definition.  definitionPath: {0}", (object) definition.FullPath);
      using (TeamFoundationDataReader foundationDataReader = this.TfsRequestContext.GetService<TeamFoundationBuildService>().QueryBuilds(this.TfsRequestContext, (IList<BuildDetailSpec>) new BuildDetailSpec[1]
      {
        buildDetailSpec
      }, new Guid()))
      {
        foreach (BuildQueryResult current in foundationDataReader.CurrentEnumerable<BuildQueryResult>())
        {
          foreach (BuildDetail build2 in current.Builds)
            build1 = build2;
        }
      }
      // ISSUE: reference to a compiler-generated field
      if (BuildController.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildController.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "DefinitionUri", typeof (BuildController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = BuildController.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) BuildController.\u003C\u003Eo__8.\u003C\u003Ep__0, this.ViewBag, definition.Uri);
      // ISSUE: reference to a compiler-generated field
      if (BuildController.\u003C\u003Eo__8.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildController.\u003C\u003Eo__8.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "DefinitionName", typeof (BuildController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = BuildController.\u003C\u003Eo__8.\u003C\u003Ep__1.Target((CallSite) BuildController.\u003C\u003Eo__8.\u003C\u003Ep__1, this.ViewBag, definition.Name);
      // ISSUE: reference to a compiler-generated field
      if (BuildController.\u003C\u003Eo__8.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildController.\u003C\u003Eo__8.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "BuildSummary", typeof (BuildController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = BuildController.\u003C\u003Eo__8.\u003C\u003Ep__2.Target((CallSite) BuildController.\u003C\u003Eo__8.\u003C\u003Ep__2, this.ViewBag, build1 != null ? build1.BuildSummaryText(this.TfsWebContext) : BuildServerResources.NoBuildExistsForDefinition);
      // ISSUE: reference to a compiler-generated field
      if (BuildController.\u003C\u003Eo__8.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildController.\u003C\u003Eo__8.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "StatusIcon", typeof (BuildController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = BuildController.\u003C\u003Eo__8.\u003C\u003Ep__3.Target((CallSite) BuildController.\u003C\u003Eo__8.\u003C\u003Ep__3, this.ViewBag, build1 != null ? build1.Status.ToString() ?? "" : string.Empty);
      return (ActionResult) this.View();
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(515050, 515060)]
    public ActionResult DefinitionTile(int definitionId, Guid projectId)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      ArgumentUtility.CheckForOutOfRange(definitionId, nameof (definitionId), 1);
      Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition = (Microsoft.TeamFoundation.Build.WebApi.BuildDefinition) null;
      if (!BuildHelpers.TryGetBuildDefinition(this.TfsRequestContext, definitionId, projectId, out definition))
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildServerResources.BuildDefinitionNotFound, (object) definitionId), HttpStatusCode.NotFound);
      // ISSUE: reference to a compiler-generated field
      if (BuildController.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildController.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "DefinitionId", typeof (BuildController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = BuildController.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) BuildController.\u003C\u003Eo__9.\u003C\u003Ep__0, this.ViewBag, definitionId);
      // ISSUE: reference to a compiler-generated field
      if (BuildController.\u003C\u003Eo__9.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildController.\u003C\u003Eo__9.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "DefinitionName", typeof (BuildController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = BuildController.\u003C\u003Eo__9.\u003C\u003Ep__1.Target((CallSite) BuildController.\u003C\u003Eo__9.\u003C\u003Ep__1, this.ViewBag, definition.Name);
      // ISSUE: reference to a compiler-generated field
      if (BuildController.\u003C\u003Eo__9.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildController.\u003C\u003Eo__9.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, Guid, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ProjectId", typeof (BuildController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = BuildController.\u003C\u003Eo__9.\u003C\u003Ep__2.Target((CallSite) BuildController.\u003C\u003Eo__9.\u003C\u003Ep__2, this.ViewBag, projectId);
      return (ActionResult) this.View();
    }

    protected bool HideBuildExplorerWarningBanner
    {
      get
      {
        HttpCookie cookie = this.Request.Cookies["Tfs-HideBuildExplorerWarningBanner"];
        bool result;
        return cookie != null && bool.TryParse(cookie.Value, out result) && result;
      }
    }

    private void AddSignalRData() => this.ViewData["SignalrHubUrl"] = (object) VssSignalRUtility.GetHubsUrl(this.TfsRequestContext);

    private ActionResult RedirectToDefinitionEditor()
    {
      RedirectToRouteResult action = this.RedirectToAction("definitionEditor");
      foreach (string allKey in this.Request.QueryString.AllKeys)
      {
        if (!action.RouteValues.ContainsKey(allKey))
          action.RouteValues.Add(allKey, (object) this.Request.QueryString[allKey]);
      }
      return (ActionResult) action;
    }

    private ActionResult RedirectToXaml()
    {
      RedirectToRouteResult action = this.RedirectToAction("xaml");
      foreach (string allKey in this.Request.QueryString.AllKeys)
      {
        if (!action.RouteValues.ContainsKey(allKey))
          action.RouteValues.Add(allKey, (object) this.Request.QueryString[allKey]);
      }
      return (ActionResult) action;
    }

    private ActionResult ExplorerInternal()
    {
      string viewName = "index";
      if (this.ClientHostCapabilities is IdeClientHostCapabilities)
        viewName = "indexide";
      string actionName = "index";
      if (this.TfsWebContext.IsFeatureAvailable("WebAccess.Build.Definitions"))
        actionName = "explorer";
      string s1 = this.Request.Params["buildId"];
      if (!string.IsNullOrEmpty(s1) && int.TryParse(s1, out int _))
      {
        string str1 = this.Url.Action(actionName, (object) new
        {
          clienthost = this.Request.Params["clienthost"]
        });
        RouteValueDictionary routeValues = this.CopyRouteValues(this.Request.QueryString);
        string str2 = this.Url.FragmentAction(this.Request.QueryString["_a"] ?? "summary", (object) routeValues);
        return (ActionResult) new ClientRedirectResult(str1 + str2);
      }
      string uri = this.Request.Params["buildUri"];
      if (!string.IsNullOrEmpty(uri) && int.TryParse(LinkingUtilities.DecodeUri(uri).ToolSpecificId, out int _))
      {
        string str3 = this.Url.Action(actionName);
        RouteValueDictionary routeValues = this.CopyRouteValues(this.Request.QueryString);
        string str4 = this.Url.FragmentAction(this.Request.QueryString["_a"] ?? "summary", (object) routeValues);
        return (ActionResult) new ClientRedirectResult(str3 + str4);
      }
      string s2 = this.Request.Params["definitionId"];
      int result;
      if (!string.IsNullOrEmpty(s2) && int.TryParse(s2, out result))
        return (ActionResult) new ClientRedirectResult(this.Url.Action(actionName) + this.Url.FragmentAction(this.Request.QueryString["_a"] ?? "completed", (object) new
        {
          definitionId = result
        }));
      this.ViewData["Tfs-HideBuildExplorerWarningBanner"] = (object) this.HideBuildExplorerWarningBanner.ToString();
      this.AddSignalRData();
      return (ActionResult) this.View(viewName);
    }

    private RouteValueDictionary CopyRouteValues(NameValueCollection source)
    {
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
      foreach (string allKey in source.AllKeys)
      {
        string str = source[allKey];
        if (!string.IsNullOrEmpty(str) && !routeValueDictionary.ContainsKey(allKey))
          routeValueDictionary.Add(allKey, (object) str);
      }
      return routeValueDictionary;
    }

    private bool IsNewDefinitionAction()
    {
      bool flag = false;
      if (string.Equals(BuildHelpers.ExtractBuildParameterFromRequest(this.Request, "_a", false), "new", StringComparison.OrdinalIgnoreCase))
        flag = true;
      return flag;
    }

    private string GetEditorUrlForEditDefinition(string buildDefinitionId)
    {
      bool isYamlEditor = this.IsYamlDefinition(buildDefinitionId);
      string editorBaseUrl = BuildHelpers.GetEditorBaseUrl(this.TfsRequestContext, this.TfsWebContext, isYamlEditor);
      NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
      if (isYamlEditor)
      {
        queryString["pipelineId"] = buildDefinitionId;
      }
      else
      {
        queryString["_a"] = "edit-build-definition";
        queryString["id"] = buildDefinitionId;
      }
      return string.Format("{0}?{1}", (object) editorBaseUrl, (object) queryString.ToString());
    }

    private string GetEditorUrlForCreateDefinition()
    {
      string editorBaseUrl = BuildHelpers.GetEditorBaseUrl(this.TfsRequestContext, this.TfsWebContext);
      string parameterFromRequest1 = BuildHelpers.ExtractBuildParameterFromRequest(this.Request, "path", false);
      string parameterFromRequest2 = BuildHelpers.ExtractBuildParameterFromRequest(this.Request, "source", false);
      NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
      queryString["_a"] = "build-definition-getting-started";
      queryString["path"] = string.IsNullOrEmpty(parameterFromRequest1) ? "\\" : parameterFromRequest1;
      queryString["source"] = string.IsNullOrEmpty(parameterFromRequest2) ? "others" : parameterFromRequest2;
      return string.Format("{0}?{1}", (object) editorBaseUrl, (object) queryString.ToString());
    }

    private bool IsYamlDefinition(string buildDefinitionId)
    {
      ProjectInfo project = this.TfsWebContext.Project;
      Guid projectId = project != null ? project.Id : Guid.Empty;
      int result;
      Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition;
      if (!(projectId != Guid.Empty) || !int.TryParse(buildDefinitionId, out result) || !BuildHelpers.TryGetBuildDefinition(this.TfsRequestContext, result, projectId, out definition))
        return false;
      Microsoft.TeamFoundation.Build.WebApi.BuildProcess process = definition.Process;
      return process != null && process.Type == 2;
    }
  }
}
