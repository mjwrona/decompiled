// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebAccessModule
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.ErrorHandler;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.Utils;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class WebAccessModule : WebModule<TeamFoundationApplication>
  {
    private readonly ExceptionLogger exceptionLogger;

    public WebAccessModule()
      : this(new Action<IVssRequestContext, string, Exception, int, EventLogEntryType>(TeamFoundationEventLog.Default.LogException), WebAccessModule.\u003C\u003EO.\u003C0\u003E__TraceExceptionRaw ?? (WebAccessModule.\u003C\u003EO.\u003C0\u003E__TraceExceptionRaw = new Action<int, TraceLevel, string, string, string, Exception>(TeamFoundationTracingService.TraceExceptionRaw)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    internal WebAccessModule(
      Action<IVssRequestContext, string, Exception, int, EventLogEntryType> logInEventLog,
      Action<int, TraceLevel, string, string, string, Exception> logExceptionRawInTeamFoundation)
    {
      this.exceptionLogger = new ExceptionLogger(logInEventLog, logExceptionRawInTeamFoundation);
    }

    protected override void OnBeginInitialize()
    {
      base.OnBeginInitialize();
      ErrorManager.Instance.RegisterHandler((IErrorPageHandler) new RequestAccessAuthorizationExceptionHandler());
      ErrorManager.Instance.RegisterHandler((IErrorPageHandler) new ProjectNotFoundExceptionHandler());
    }

    public override WebContext CreateWebContext(RequestContext requestContext) => (WebContext) new TfsWebContext(requestContext);

    public override PageContext CreatePageContext(RequestContext requestContext)
    {
      TfsWebPageGlobalContext pageContext = new TfsWebPageGlobalContext(WebContextFactory.GetWebContext(requestContext));
      pageContext.FeatureAvailability.IncludeFeatureFlags((IEnumerable<string>) TfsWebAccessBootstrapFeatureFlags.FeatureFlagNames);
      pageContext.WebAccessConfiguration.IncludeRegistryItem(pageContext.WebContext.TfsRequestContext, "/Service/WorkItemTracking/IndexedDbVersion", true);
      this.UpdateModuleLoaderConfiguration(pageContext.ModuleLoaderConfig, pageContext.WebContext);
      return (PageContext) pageContext;
    }

    private void UpdateModuleLoaderConfiguration(
      ModuleLoaderConfiguration moduleLoaderConfig,
      WebContext webContext)
    {
      bool debugMode = webContext.Diagnostics.DebugMode;
      string absolute = VirtualPathUtility.ToAbsolute("~/_api/_ScriptResource/Module/" + StaticResources.Versioned.Version + "/" + CultureInfo.CurrentUICulture.Name + "/", webContext.TfsRequestContext.VirtualPath());
      moduleLoaderConfig.AddResourceModulePaths(absolute);
      moduleLoaderConfig.Paths["Engagement"] = webContext.Url.TfsScriptContent("Engagement", debugMode);
      moduleLoaderConfig.AddContributionPath("TFS");
      moduleLoaderConfig.AddContributionPath("Notifications");
      moduleLoaderConfig.AddContributionPath("Presentation/Scripts/marked");
      moduleLoaderConfig.AddContributionPath("Presentation/Scripts/URI");
      moduleLoaderConfig.AddContributionPath("Presentation/Scripts/punycode");
      moduleLoaderConfig.AddContributionPath("Presentation/Scripts/IPv6");
      moduleLoaderConfig.AddContributionPath("Presentation/Scripts/SecondLevelDomains");
      moduleLoaderConfig.Paths["d3"] = StaticResources.ThirdParty.Scripts.GetLocation(debugMode ? "d3.v3" : "d3.v3.min");
      moduleLoaderConfig.AddShimConfig("d3");
      this.AddHighchartsScripts(moduleLoaderConfig, webContext);
      moduleLoaderConfig.Paths["gridster"] = StaticResources.ThirdParty.Scripts.GetLocation(debugMode ? "jquery.gridster" : "jquery.gridster.min");
      moduleLoaderConfig.AddShimConfig("gridster");
      moduleLoaderConfig.Paths["signalR"] = StaticResources.ThirdParty.Scripts.GetLocation(debugMode ? "jquery.signalR-upd.2.2.0" : "jquery.signalR-upd.2.2.0.min");
      moduleLoaderConfig.AddShimConfig("signalR");
      moduleLoaderConfig.Map["*"] = new Dictionary<string, string>()
      {
        {
          "office-ui-fabric-react/lib",
          "OfficeFabric"
        }
      };
      moduleLoaderConfig.AddContributionPath("ContentRendering");
      moduleLoaderConfig.AddContributionPath("Analytics");
    }

    private void AddHighchartsScripts(
      ModuleLoaderConfiguration moduleLoaderConfig,
      WebContext webContext)
    {
      this.AddHighchartsFile(moduleLoaderConfig, webContext, "highcharts", "highcharts");
      this.AddHighchartsFile(moduleLoaderConfig, webContext, "highcharts/highcharts-more", "highcharts-more");
      this.AddHighchartsFile(moduleLoaderConfig, webContext, "highcharts/modules/accessibility", "highcharts-accessibility");
      this.AddHighchartsFile(moduleLoaderConfig, webContext, "highcharts/modules/heatmap", "highcharts-heatmap");
      this.AddHighchartsFile(moduleLoaderConfig, webContext, "highcharts/modules/funnel", "highcharts-funnel");
    }

    private void AddHighchartsFile(
      ModuleLoaderConfiguration moduleLoaderConfig,
      WebContext webContext,
      string moduleName,
      string rootFilename)
    {
      string str1 = ".v9.0.1";
      string str2 = webContext.Diagnostics.DebugMode ? ".src" : "";
      string str3 = rootFilename + str1 + str2;
      moduleLoaderConfig.Paths[moduleName] = StaticResources.ThirdParty.Scripts.GetLocation(str3);
      moduleLoaderConfig.AddContributionPath(moduleName, ContributionPathType.ThirdParty, str3);
    }

    protected override bool? OnApplicationReportError(Exception exception)
    {
      if (TfsController.IsCurrentRequestFromWebAccess())
        return new bool?(UserFriendlyError.ShouldReportException(exception));
      return exception is HttpAntiForgeryException ? new bool?(false) : new bool?();
    }

    protected override void LogExceptionInternal(HttpContextBase httpContext, Exception exception) => this.exceptionLogger.LogException(httpContext, exception);

    protected override void RegisterDefaultRoutes(RouteCollection routes)
    {
      base.RegisterDefaultRoutes(routes);
      WebAccessModule.RegisterRoutes(routes, VirtualPathUtility.ToAbsolute("~/"));
    }

    public static void RegisterRoutes(RouteCollection routes, string rootPath)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.MapTfsRoute("Error", TeamFoundationHostType.All, "_error", (object) new
      {
        controller = "error",
        action = "index"
      });
      routes.MapTfsRoute("CheckEndpoint", TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, "_check", (object) new
      {
        controller = "Home",
        action = "Check"
      }, (object) null);
      TeamFoundationHostType hostType = TeamFoundationHostType.Application | TeamFoundationHostType.Deployment;
      if (TeamFoundationApplicationCore.DeploymentServiceHost.IsHosted)
        hostType |= TeamFoundationHostType.ProjectCollection;
      routes.MapTfsRoute("PermaLink", hostType, "_permaLink/{*relativePath}", (object) new
      {
        controller = "PermaLink",
        action = "RedirectRoute"
      });
      routes.MapTfsRoute("Compatibility", hostType, "web/{*page}", (object) new
      {
        controller = "compatibility",
        action = "route"
      }, (object) new{ page = "((?!_)[a-zA-Z_/]+.aspx)?" });
      if (!TeamFoundationApplicationCore.DeploymentServiceHost.IsHosted)
        routes.MapTfsRoute("Rooms", TeamFoundationHostType.ProjectCollection, "_rooms", (object) new
        {
          controller = "Compatibility",
          action = "RouteRooms"
        });
      routes.RegisterRouteArea("Api");
      routes.RegisterRouteArea("Public");
      routes.Add((RouteBase) new ContributionProxyRoute());
      RegisterGitRoutes.RegisterGitRoutesInit();
      routes.RegisterRouteArea("");
      routes.RegisterRouteArea("Admin");
      routes.RegisterRouteArea("Oi");
      routes.RegisterRouteArea("BuiltInExtensions");
      routes.MapTfsRoute("FallbackRoute", TeamFoundationHostType.All, "{*params}", (object) new
      {
        controller = "error",
        action = "notFound"
      });
    }
  }
}
