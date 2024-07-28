// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.GeneralHtmlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class GeneralHtmlExtensions
  {
    private const string c_globalMessageKey = "__globalMessage";
    private const string c_bundleKeyFormat = "__bundles:{0}";
    private const string c_modulesToRequireKey = "__modulesToRequire";
    private const string c_excludedPathsKey = "__excludedPaths";
    private const string c_commonScriptsBundle = "common";
    private const string c_areaScriptsBundle = "area";
    private const string c_viewScriptsBundle = "view";
    private const string c_allScriptsBundle = "all";
    private const string c_commonCssBundle = "commoncss";
    private const string c_areaCssBundle = "areacss";
    private const string c_viewCssBundle = "viewcss";
    private const string c_allCssBundle = "allcss";
    private const string c_bodyClassesKey = "__bodyClasses";
    private const string c_fullscreenKey = "fullScreen";
    private const string c_useNewPlatformHostKey = "useNewPlatformHost";

    public static string Base64Json(this HtmlHelper htmlHelper, object data) => Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Formatting.None)));

    public static void PageTitle(this HtmlHelper htmlHelper, string title) => htmlHelper.ViewData["__pageTitle"] = (object) title;

    public static string PageTitle(this HtmlHelper htmlHelper) => htmlHelper.ViewData["__pageTitle"] as string;

    public static string HtmlPageTitle(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (HtmlPageTitle));
      try
      {
        string str = htmlHelper.PageTitle();
        return string.IsNullOrEmpty(str) ? GeneralHtmlExtensions.GeneratePageTitle(htmlHelper) : str;
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (HtmlPageTitle));
      }
    }

    private static string GeneratePageTitle(HtmlHelper htmlHelper)
    {
      IVssRequestContext vssRequestContext = htmlHelper.ViewContext.RequestContext.TfsRequestContext();
      int num = vssRequestContext == null ? 0 : (vssRequestContext.ExecutionEnvironment.IsHostedDeployment ? 1 : 0);
      string str1 = htmlHelper.ContentTitle();
      bool flag = !string.IsNullOrEmpty(str1);
      string pageTitle;
      if (num != 0)
      {
        string str2;
        if (!flag)
          str2 = WACommonResources.PageTitle_Hosted;
        else
          str2 = WACommonResources.PageTitleWithContent_Hosted.FormatUI((object) str1);
        pageTitle = str2;
      }
      else
      {
        string str3;
        if (!flag)
          str3 = WACommonResources.PageTitle;
        else
          str3 = WACommonResources.PageTitleWithContent.FormatUI((object) str1);
        pageTitle = str3;
      }
      return pageTitle;
    }

    public static void ContentTitle(this HtmlHelper htmlHelper, string title)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, "SetContentTitle");
      try
      {
        htmlHelper.ViewData["__contentTitle"] = (object) title;
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, "SetContentTitle");
      }
    }

    public static void AddHubViewClass(this HtmlHelper htmlHelper, string className)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (AddHubViewClass));
      try
      {
        string str = htmlHelper.HubViewClasses();
        if (string.IsNullOrEmpty(str))
          htmlHelper.ViewData["__hubViewClasses"] = (object) className;
        else
          htmlHelper.ViewData["__hubViewClasses"] = (object) string.Join(" ", new string[2]
          {
            str,
            className
          });
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (AddHubViewClass));
      }
    }

    public static string HubViewClasses(this HtmlHelper htmlHelper) => htmlHelper.ViewData["__hubViewClasses"] as string;

    public static void AddBodyClass(this HtmlHelper htmlHelper, string className) => GeneralHtmlExtensions.AddBodyClass(htmlHelper.ViewContext.HttpContext, className);

    public static string BodyClasses(this HtmlHelper htmlHelper)
    {
      if (htmlHelper.Chromeless())
      {
        GeneralHtmlExtensions.AddBodyClass(htmlHelper.ViewContext.HttpContext, "no-chrome");
        GeneralHtmlExtensions.AddBodyClass(htmlHelper.ViewContext.HttpContext, "server-shim");
      }
      return htmlHelper.ViewContext.HttpContext.Items[(object) "__bodyClasses"] as string;
    }

    public static void AddBodyClass(HttpContextBase httpContext, string bodyClass)
    {
      string str1 = httpContext.Items[(object) "__bodyClasses"] as string;
      string str2;
      if (string.IsNullOrEmpty(str1))
        str2 = bodyClass;
      else
        str2 = string.Join(" ", new string[2]
        {
          str1,
          bodyClass
        });
      httpContext.Items[(object) "__bodyClasses"] = (object) str2;
    }

    public static void Chromeless(this ViewDataDictionary viewData, bool isChromeless) => viewData["__chromeless"] = (object) isChromeless;

    public static bool Chromeless(this HtmlHelper htmlHelper)
    {
      bool? nullable = htmlHelper.ViewData["__chromeless"] as bool?;
      bool flag = true;
      return nullable.GetValueOrDefault() == flag & nullable.HasValue;
    }

    public static void Chromeless(this HtmlHelper htmlHelper, bool isChromeless) => htmlHelper.ViewData.Chromeless(isChromeless);

    public static bool IsFullscreen(HttpContextBase httpContext)
    {
      bool result = false;
      return ((httpContext.Request.QueryString == null ? 0 : (bool.TryParse(httpContext.Request.QueryString["fullScreen"], out result) ? 1 : 0)) & (result ? 1 : 0)) != 0;
    }

    public static void AddFullscreenClassesIfRequested(HttpContextBase httpContext)
    {
      if (!GeneralHtmlExtensions.IsFullscreen(httpContext))
        return;
      GeneralHtmlExtensions.AddFullscreenClasses(httpContext);
    }

    public static void AddFullscreenClasses(HttpContextBase httpContext) => GeneralHtmlExtensions.AddBodyClass(httpContext, "no-chrome full-screen-mode");

    public static void AddFullscreenRouteValue(this RouteValueDictionary routeValues) => routeValues["fullScreen"] = (object) "true";

    public static WebGlobalMessage GetHighestLevelGlobalMessage(this HtmlHelper htmlHelper)
    {
      if (!(htmlHelper.ViewData["__globalMessage"] is List<WebGlobalMessage> webGlobalMessageList) || webGlobalMessageList.Count <= 0)
        return (WebGlobalMessage) null;
      WebGlobalMessage levelGlobalMessage = webGlobalMessageList[0];
      for (int index = 1; index < webGlobalMessageList.Count; ++index)
      {
        WebGlobalMessage webGlobalMessage = webGlobalMessageList[index];
        if (levelGlobalMessage != null && webGlobalMessage != null && webGlobalMessage.MessageLevel > levelGlobalMessage.MessageLevel)
          levelGlobalMessage = webGlobalMessage;
      }
      return levelGlobalMessage;
    }

    public static void AddGlobalMessage(
      this HtmlHelper htmlHelper,
      string globalMessage,
      WebMessageLevel level = WebMessageLevel.Info,
      bool htmlFormat = false,
      string cssClass = null,
      bool closeable = false)
    {
      GeneralHtmlExtensions.AddGlobalmessage(htmlHelper.ViewData, globalMessage, level, htmlFormat, cssClass, closeable);
    }

    public static void AddGlobalMessage(
      this AsyncController controller,
      string globalMessage,
      WebMessageLevel level = WebMessageLevel.Info,
      bool htmlFormat = false,
      string cssClass = null,
      bool closeable = false)
    {
      GeneralHtmlExtensions.AddGlobalmessage(controller.ViewData, globalMessage, level, htmlFormat, cssClass, closeable);
    }

    private static void AddGlobalmessage(
      ViewDataDictionary viewData,
      string globalMessage,
      WebMessageLevel level,
      bool htmlFormat,
      string cssClass,
      bool closeable)
    {
      if (!(viewData["__globalMessage"] is List<WebGlobalMessage> webGlobalMessageList))
      {
        webGlobalMessageList = new List<WebGlobalMessage>();
        viewData["__globalMessage"] = (object) webGlobalMessageList;
      }
      webGlobalMessageList.Add(new WebGlobalMessage()
      {
        CssClass = cssClass,
        Message = globalMessage,
        MessageLevel = level,
        HtmlFormat = htmlFormat,
        Closeable = closeable
      });
    }

    public static string ContentTitle(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, "GetContentTitle");
      try
      {
        return htmlHelper.ViewData["__contentTitle"] as string;
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, "GetContentTitle");
      }
    }

    public static MvcHtmlString HubTitle(this HtmlHelper htmlHelper) => htmlHelper.HubTitle(htmlHelper.ContentTitle());

    public static MvcHtmlString HubTitle(this HtmlHelper htmlHelper, string title) => new TagBuilder("div").AddClass("hub-title").Text(title).ToHtmlString();

    public static MvcHtmlString HubTitle(this HtmlHelper htmlHelper, MvcHtmlString htmlTitle) => new TagBuilder("div").AddClass("hub-title").Html(htmlTitle.ToHtmlString()).ToHtmlString();

    public static void RegisterCommonScriptModules(
      this HtmlHelper htmlHelper,
      params string[] modules)
    {
      GeneralHtmlExtensions.RegisterCommonScriptModules(htmlHelper.ViewContext.HttpContext, modules);
    }

    public static void RegisterCommonScriptModules(
      HttpContextBase httpContext,
      params string[] modules)
    {
      GeneralHtmlExtensions.AddToScriptBundle(httpContext, (IEnumerable<string>) modules, "common");
      GeneralHtmlExtensions.AddToScriptBundle(httpContext, (IEnumerable<string>) modules, "all");
    }

    public static void UseCommonScriptModules(this HtmlHelper htmlHelper, params string[] modules) => GeneralHtmlExtensions.UseCommonScriptModules(htmlHelper.ViewContext.HttpContext, modules);

    public static void UseCommonScriptModules(HttpContextBase httpContext, params string[] modules)
    {
      GeneralHtmlExtensions.RegisterCommonScriptModules(httpContext, modules);
      GeneralHtmlExtensions.AddScriptsToRequire(httpContext, (IEnumerable<string>) modules);
    }

    public static void RegisterAreaScriptModules(
      this HtmlHelper htmlHelper,
      params string[] modules)
    {
      GeneralHtmlExtensions.RegisterAreaScriptModules(htmlHelper.ViewContext.HttpContext, modules);
    }

    public static void RegisterAreaScriptModules(
      HttpContextBase httpContext,
      params string[] modules)
    {
      GeneralHtmlExtensions.AddToScriptBundle(httpContext, (IEnumerable<string>) modules, "area");
      GeneralHtmlExtensions.AddToScriptBundle(httpContext, (IEnumerable<string>) modules, "all");
    }

    public static void UseAreaScriptModules(this HtmlHelper htmlHelper, params string[] modules) => GeneralHtmlExtensions.UseAreaScriptModules(htmlHelper.ViewContext.HttpContext, modules);

    public static void UseAreaScriptModules(HttpContextBase httpContext, params string[] modules)
    {
      GeneralHtmlExtensions.RegisterAreaScriptModules(httpContext, modules);
      GeneralHtmlExtensions.AddScriptsToRequire(httpContext, (IEnumerable<string>) modules);
    }

    public static void RegisterViewScriptModules(
      this HtmlHelper htmlHelper,
      params string[] modules)
    {
      GeneralHtmlExtensions.RegisterViewScriptModules(htmlHelper.ViewContext.HttpContext, modules);
    }

    public static void RegisterViewScriptModules(
      HttpContextBase httpContext,
      params string[] modules)
    {
      GeneralHtmlExtensions.AddToScriptBundle(httpContext, (IEnumerable<string>) modules, "view");
      GeneralHtmlExtensions.AddToScriptBundle(httpContext, (IEnumerable<string>) modules, "all");
    }

    public static void UseScriptModules(this HtmlHelper htmlHelper, params string[] modules) => GeneralHtmlExtensions.UseScriptModules(htmlHelper.ViewContext.HttpContext, modules);

    public static void UseScriptModules(HttpContextBase httpContext, params string[] modules)
    {
      GeneralHtmlExtensions.RegisterViewScriptModules(httpContext, modules);
      GeneralHtmlExtensions.AddScriptsToRequire(httpContext, (IEnumerable<string>) modules);
    }

    public static void UseCommonCSS(this HtmlHelper htmlHelper, params string[] cssFiles) => GeneralHtmlExtensions.UseCommonCSS(htmlHelper.ViewContext.HttpContext, cssFiles);

    public static void UseCommonCSS(HttpContextBase httpContext, params string[] cssFiles)
    {
      GeneralHtmlExtensions.AddToCSSBundle(httpContext, (IEnumerable<string>) cssFiles, "commoncss");
      GeneralHtmlExtensions.AddToCSSBundle(httpContext, (IEnumerable<string>) cssFiles, "allcss");
    }

    public static void UseAreaCSS(this HtmlHelper htmlHelper, params string[] cssFiles) => GeneralHtmlExtensions.UseAreaCSS(htmlHelper.ViewContext.HttpContext, cssFiles);

    public static void UseAreaCSS(HttpContextBase httpContext, params string[] cssFiles)
    {
      GeneralHtmlExtensions.AddToCSSBundle(httpContext, (IEnumerable<string>) cssFiles, "areacss");
      GeneralHtmlExtensions.AddToCSSBundle(httpContext, (IEnumerable<string>) cssFiles, "allcss");
    }

    public static void UseCSS(this HtmlHelper htmlHelper, params string[] cssFiles) => GeneralHtmlExtensions.UseCSS(htmlHelper.ViewContext.HttpContext, cssFiles);

    public static void UseCSS(HttpContextBase httpContext, params string[] cssFiles)
    {
      GeneralHtmlExtensions.AddToCSSBundle(httpContext, (IEnumerable<string>) cssFiles, "viewcss");
      GeneralHtmlExtensions.AddToCSSBundle(httpContext, (IEnumerable<string>) cssFiles, "allcss");
    }

    public static void AddNewPlatformScriptExcludePaths(this HtmlHelper htmlHelper) => GeneralHtmlExtensions.AddScriptExcludePaths(htmlHelper.ViewContext.RequestContext.HttpContext, BundlingHelper.ExcludedNewPlatformPaths);

    public static void AddScriptExcludePaths(
      HttpContextBase httpContext,
      IEnumerable<string> excludePaths)
    {
      HashSet<string> scriptExcludePaths = GeneralHtmlExtensions.GetScriptExcludePaths(httpContext);
      foreach (string excludePath in excludePaths)
        scriptExcludePaths.Add(excludePath);
    }

    private static HashSet<string> GetScriptExcludePaths(HttpContextBase httpContext)
    {
      if (!(httpContext.Items[(object) "__excludedPaths"] is HashSet<string> scriptExcludePaths))
      {
        scriptExcludePaths = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        httpContext.Items[(object) "__excludedPaths"] = (object) scriptExcludePaths;
      }
      return scriptExcludePaths;
    }

    private static void AddScriptsToRequire(
      HttpContextBase httpContext,
      IEnumerable<string> modules)
    {
      GeneralHtmlExtensions.GetScriptsToRequire(httpContext).UnionWith(modules);
    }

    public static ISet<string> GetScriptsToRequire(HttpContextBase httpContextr)
    {
      if (!(httpContextr.Items[(object) "__modulesToRequire"] is HashSet<string> scriptsToRequire))
      {
        scriptsToRequire = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        httpContextr.Items[(object) "__modulesToRequire"] = (object) scriptsToRequire;
      }
      return (ISet<string>) scriptsToRequire;
    }

    private static void AddToScriptBundle(
      HttpContextBase httpContext,
      IEnumerable<string> modules,
      string group)
    {
      GeneralHtmlExtensions.GetScriptBundle(httpContext, group, true).IncludedScripts.UnionWith(modules);
    }

    private static void AddToCSSBundle(
      HttpContextBase httpContext,
      IEnumerable<string> modules,
      string group)
    {
      GeneralHtmlExtensions.GetCSSBundle(httpContext, group, true).IncludedCSSFiles.UnionWith(modules);
    }

    private static BundleScriptModulesContent GetScriptBundle(
      HttpContextBase httpContext,
      string group,
      bool create)
    {
      string key = string.Format("__bundles:{0}", (object) group);
      BundleScriptModulesContent scriptBundle = httpContext.Items[(object) key] as BundleScriptModulesContent;
      if (scriptBundle == null & create)
      {
        scriptBundle = new BundleScriptModulesContent();
        scriptBundle.IncludedScripts = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        scriptBundle.ExcludedScripts = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        httpContext.Items[(object) key] = (object) scriptBundle;
      }
      return scriptBundle;
    }

    private static BundleCSSContent GetCSSBundle(
      HttpContextBase httpContext,
      string group,
      bool create)
    {
      string key = string.Format("__bundles:{0}", (object) group);
      BundleCSSContent cssBundle = httpContext.Items[(object) key] as BundleCSSContent;
      if (cssBundle == null & create)
      {
        cssBundle = new BundleCSSContent();
        cssBundle.IncludedCSSFiles = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        httpContext.Items[(object) key] = (object) cssBundle;
      }
      return cssBundle;
    }

    public static MvcHtmlString ScriptModulesWithCallback(
      this HtmlHelper htmlHelper,
      string callbackScript,
      params string[] defaultModules)
    {
      return htmlHelper.ScriptModulesInternal(callbackScript, defaultModules);
    }

    public static MvcHtmlString ScriptModules(
      this HtmlHelper htmlHelper,
      params string[] defaultModules)
    {
      return htmlHelper.ScriptModulesInternal((string) null, defaultModules);
    }

    public static void UseNewPlatformHost(this HttpContextBase httpContext, bool value) => httpContext.Items[(object) "useNewPlatformHost"] = (object) value;

    public static bool UseNewPlatformHost(this HttpContextBase httpContext)
    {
      object obj = httpContext.Items[(object) "useNewPlatformHost"];
      return obj != null && obj is bool flag && flag;
    }

    private static MvcHtmlString ScriptModulesInternal(
      this HtmlHelper htmlHelper,
      string callbackScript,
      params string[] defaultModules)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "Html.ScriptModules"))
      {
        htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, "ScriptModules");
        try
        {
          bool flag = htmlHelper.ViewContext.HttpContext.UseNewPlatformHost();
          StringBuilder stringBuilder = new StringBuilder();
          ISet<string> scriptsToRequire = GeneralHtmlExtensions.GetScriptsToRequire(htmlHelper.ViewContext.HttpContext);
          if (defaultModules.Length != 0)
            scriptsToRequire.UnionWith((IEnumerable<string>) defaultModules);
          if (scriptsToRequire.Any<string>())
          {
            JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
            stringBuilder.AppendLine("if (window.performance && window.performance.mark) { window.performance.mark('requireStart'); }");
            string str = "if (window.performance && window.performance.mark) { window.performance.mark('requireEnd'); } window.requiredModulesLoaded=true;";
            stringBuilder.AppendLine("require(" + scriptSerializer.Serialize((object) scriptsToRequire.ToArray<string>()) + ", function(){  " + str + " " + (!string.IsNullOrEmpty(callbackScript) ? callbackScript : string.Empty) + " });");
          }
          PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
          StringBuilder builder = new StringBuilder();
          if (pageContext.WebContext.Diagnostics.BundlingEnabled)
          {
            htmlHelper.RegisterCommonScriptModules("VSS/Bundling");
            IList<BundledScriptFile> bundledScriptFiles = GeneralHtmlExtensions.GetBundledScriptFiles(pageContext, defaultModules);
            if (flag)
            {
              string lastBundleCallback = htmlHelper.ModuleLoaderConfigContent() + Environment.NewLine + stringBuilder.ToString();
              builder.AppendLine(BundlingHelper.GetScriptBundlesNewPlatformHtml(htmlHelper, (IEnumerable<BundledScriptFile>) bundledScriptFiles, lastBundleCallback).ToString());
            }
            else
              builder.AppendLine(BundlingHelper.GetScriptBundlesHtml(htmlHelper, (IEnumerable<BundledScriptFile>) bundledScriptFiles).ToString());
          }
          foreach (DynamicScriptBundle contributedScriptBundle in htmlHelper.GetContributedScriptBundles())
            BundlingHelper.AppendBundleScript(pageContext.WebContext, builder, contributedScriptBundle.Uri, (IEnumerable<string>) null, (string) null, contributedScriptBundle.ContentLength, contributedScriptBundle.Integrity);
          if (!flag || !pageContext.WebContext.Diagnostics.BundlingEnabled)
            builder.AppendLine(htmlHelper.JavaScript(stringBuilder.ToString()).ToString());
          return new MvcHtmlString(builder.ToString());
        }
        finally
        {
          htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, "ScriptModules");
        }
      }
    }

    public static IList<BundledScriptFile> GetBundledScriptFiles(
      PageContext pageContext,
      params string[] defaultModules)
    {
      WebContext webContext = pageContext.WebContext;
      IVssRequestContext tfsRequestContext = webContext.TfsRequestContext;
      HttpContextBase httpContext = webContext.RequestContext.HttpContext;
      HashSet<string> scriptExcludePaths = GeneralHtmlExtensions.GetScriptExcludePaths(httpContext);
      GeneralHtmlExtensions.RegisterCommonScriptModules(httpContext, "VSS/Bundling");
      List<ScriptBundleDefinition> scriptBundles = new List<ScriptBundleDefinition>();
      BundleScriptModulesContent scriptBundle1 = GeneralHtmlExtensions.GetScriptBundle(httpContext, "common", false);
      if (scriptBundle1 != null && scriptBundle1.IncludedScripts.Count > 0)
      {
        scriptBundle1.ExcludedPaths = (IEnumerable<string>) scriptExcludePaths;
        List<ScriptBundleDefinition> bundleDefinitionList = scriptBundles;
        ScriptBundleDefinition bundleDefinition = new ScriptBundleDefinition();
        bundleDefinition.FriendlyName = "common";
        bundleDefinition.Culture = CultureInfo.CurrentUICulture;
        bundleDefinition.DebugScripts = webContext.Diagnostics.DebugMode;
        bundleDefinition.ContentList = (IList<IBundledScriptContent>) BundlingHelper.GenerateInstrumentedContent(new List<IBundledScriptContent>()
        {
          (IBundledScriptContent) scriptBundle1
        }, "common");
        bundleDefinition.Diagnose = webContext.Diagnostics.DiagnoseBundles;
        bundleDefinitionList.Add(bundleDefinition);
      }
      BundleScriptModulesContent scriptBundle2 = GeneralHtmlExtensions.GetScriptBundle(httpContext, "area", false);
      if (scriptBundle2 != null && scriptBundle2.IncludedScripts.Count > 0)
      {
        scriptBundle2.ExcludedPaths = (IEnumerable<string>) scriptExcludePaths;
        List<ScriptBundleDefinition> bundleDefinitionList = scriptBundles;
        ScriptBundleDefinition bundleDefinition = new ScriptBundleDefinition();
        bundleDefinition.FriendlyName = "area";
        bundleDefinition.Culture = CultureInfo.CurrentUICulture;
        bundleDefinition.DebugScripts = webContext.Diagnostics.DebugMode;
        bundleDefinition.ContentList = (IList<IBundledScriptContent>) BundlingHelper.GenerateInstrumentedContent(new List<IBundledScriptContent>()
        {
          (IBundledScriptContent) scriptBundle2
        }, "view");
        bundleDefinition.Diagnose = webContext.Diagnostics.DiagnoseBundles;
        bundleDefinitionList.Add(bundleDefinition);
      }
      ISet<string> stringSet = (ISet<string>) new HashSet<string>();
      if (webContext.TfsRequestContext.IntendedHostType() > TeamFoundationHostType.Deployment && webContext.TfsRequestContext.UserContext != (IdentityDescriptor) null)
        stringSet = ContributionUtility.GetRequiredModules(pageContext.Contributions);
      BundleScriptModulesContent scriptBundle3 = GeneralHtmlExtensions.GetScriptBundle(httpContext, "view", stringSet.Any<string>() || defaultModules.Length != 0);
      if (scriptBundle3 != null)
      {
        scriptBundle3.ExcludedPaths = (IEnumerable<string>) scriptExcludePaths;
        if (defaultModules.Length != 0)
          scriptBundle3.IncludedScripts.UnionWith((IEnumerable<string>) defaultModules);
        if (stringSet.Any<string>())
          scriptBundle3.IncludedScripts.UnionWith((IEnumerable<string>) stringSet);
        List<ScriptBundleDefinition> bundleDefinitionList = scriptBundles;
        ScriptBundleDefinition bundleDefinition = new ScriptBundleDefinition();
        bundleDefinition.FriendlyName = "view";
        bundleDefinition.Culture = CultureInfo.CurrentUICulture;
        bundleDefinition.DebugScripts = webContext.Diagnostics.DebugMode;
        bundleDefinition.ContentList = (IList<IBundledScriptContent>) BundlingHelper.GenerateInstrumentedContent(new List<IBundledScriptContent>()
        {
          (IBundledScriptContent) scriptBundle3
        }, "view");
        bundleDefinition.Diagnose = webContext.Diagnostics.DiagnoseBundles;
        bundleDefinitionList.Add(bundleDefinition);
      }
      using (PerformanceTimer.StartMeasure(tfsRequestContext, "Html.ScriptModules.RegisterScriptBundles"))
        return BundlingHelper.RegisterScriptBundles(webContext, (IEnumerable<ScriptBundleDefinition>) scriptBundles);
    }

    public static BundleCSSContent GetAllCssBundle(HttpContextBase httpContext) => GeneralHtmlExtensions.GetCSSBundle(httpContext, "allcss", true);

    public static MvcHtmlString RenderCSSBundles(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "Html.RenderCSS"))
      {
        htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, "RenderCSS");
        try
        {
          PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
          WebContext webContext = pageContext.WebContext;
          if (!webContext.Diagnostics.BundlingEnabled || GeneralHtmlExtensions.IsLegacyBrowserWithCSSLimits(webContext.RequestContext.HttpContext.Request))
          {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string includedCssFile in (IEnumerable<string>) GeneralHtmlExtensions.GetAllCssBundle(htmlHelper.ViewContext.HttpContext).IncludedCSSFiles)
              stringBuilder.AppendLine(htmlHelper.RenderThemedCssFile(includedCssFile + ".css").ToString());
            return new MvcHtmlString(stringBuilder.ToString());
          }
          IList<BundledCSSFile> bundledCssFiles = GeneralHtmlExtensions.GetBundledCSSFiles(pageContext);
          return BundlingHelper.GetCSSBundlesHtml(htmlHelper, (IEnumerable<BundledCSSFile>) bundledCssFiles);
        }
        finally
        {
          htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, "RenderCSS");
        }
      }
    }

    public static IList<BundledCSSFile> GetBundledCSSFiles(PageContext pageContext)
    {
      WebContext webContext = pageContext.WebContext;
      IVssRequestContext tfsRequestContext = webContext.TfsRequestContext;
      HttpContextBase httpContext = webContext.RequestContext.HttpContext;
      HashSet<string> scriptExcludePaths = GeneralHtmlExtensions.GetScriptExcludePaths(httpContext);
      List<CSSBundleDefinition> cssBundles = new List<CSSBundleDefinition>();
      using (PerformanceTimer.StartMeasure(tfsRequestContext, "Html.RenderCSS.DefineBundles"))
      {
        string str = pageContext.WebContext.Url.ThemeName();
        BundleCSSContent cssBundle1 = GeneralHtmlExtensions.GetCSSBundle(httpContext, "commoncss", true);
        cssBundle1.IncludedScripts = GeneralHtmlExtensions.GetScriptBundle(httpContext, "common", true).IncludedScripts;
        cssBundle1.ExcludedScriptPaths = (IEnumerable<string>) scriptExcludePaths;
        List<CSSBundleDefinition> bundleDefinitionList1 = cssBundles;
        CSSBundleDefinition bundleDefinition1 = new CSSBundleDefinition();
        bundleDefinition1.FriendlyName = "commoncss";
        bundleDefinition1.DebugStyles = webContext.Diagnostics.DebugMode;
        bundleDefinition1.ThemeName = str;
        bundleDefinition1.CSSModulePrefixes = (IEnumerable<string>) pageContext.CssModulePrefixes;
        bundleDefinition1.ContentList = (IList<IBundledCSSContent>) new BundleCSSContent[1]
        {
          cssBundle1
        };
        CSSBundleDefinition bundleDefinition2 = bundleDefinition1;
        bundleDefinitionList1.Add(bundleDefinition2);
        BundleCSSContent cssBundle2 = GeneralHtmlExtensions.GetCSSBundle(httpContext, "areacss", true);
        cssBundle2.IncludedScripts = GeneralHtmlExtensions.GetScriptBundle(httpContext, "area", true).IncludedScripts;
        cssBundle2.ExcludedScriptPaths = (IEnumerable<string>) scriptExcludePaths;
        List<CSSBundleDefinition> bundleDefinitionList2 = cssBundles;
        CSSBundleDefinition bundleDefinition3 = new CSSBundleDefinition();
        bundleDefinition3.FriendlyName = "areacss";
        bundleDefinition3.DebugStyles = webContext.Diagnostics.DebugMode;
        bundleDefinition3.ThemeName = str;
        bundleDefinition3.CSSModulePrefixes = (IEnumerable<string>) pageContext.CssModulePrefixes;
        bundleDefinition3.ContentList = (IList<IBundledCSSContent>) new BundleCSSContent[1]
        {
          cssBundle2
        };
        CSSBundleDefinition bundleDefinition4 = bundleDefinition3;
        bundleDefinitionList2.Add(bundleDefinition4);
        BundleCSSContent cssBundle3 = GeneralHtmlExtensions.GetCSSBundle(httpContext, "viewcss", true);
        cssBundle3.IncludedScripts = (ISet<string>) new HashSet<string>((IEnumerable<string>) GeneralHtmlExtensions.GetScriptBundle(httpContext, "view", true).IncludedScripts, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        cssBundle3.ExcludedScriptPaths = (IEnumerable<string>) scriptExcludePaths;
        using (PerformanceTimer.StartMeasure(tfsRequestContext, "Html.RenderCSS.DefineBundles.Contributed"))
          cssBundle3.IncludedScripts.UnionWith((IEnumerable<string>) ContributionUtility.GetRequiredModules(pageContext.Contributions));
        List<CSSBundleDefinition> bundleDefinitionList3 = cssBundles;
        CSSBundleDefinition bundleDefinition5 = new CSSBundleDefinition();
        bundleDefinition5.FriendlyName = "viewcss";
        bundleDefinition5.DebugStyles = webContext.Diagnostics.DebugMode;
        bundleDefinition5.ThemeName = str;
        bundleDefinition5.CSSModulePrefixes = (IEnumerable<string>) pageContext.CssModulePrefixes;
        bundleDefinition5.ContentList = (IList<IBundledCSSContent>) new BundleCSSContent[1]
        {
          cssBundle3
        };
        CSSBundleDefinition bundleDefinition6 = bundleDefinition5;
        bundleDefinitionList3.Add(bundleDefinition6);
      }
      using (PerformanceTimer.StartMeasure(tfsRequestContext, "Html.RenderCSS.RegisterBundles"))
        return BundlingHelper.RegisterCSSBundles(webContext, (IEnumerable<CSSBundleDefinition>) cssBundles);
    }

    private static bool IsLegacyBrowserWithCSSLimits(HttpRequestBase requestContext) => BrowserFilterService.IsBrowserIE9OrOlder(requestContext);

    public static MvcHtmlString Error(this HtmlHelper htmlHelper, string message)
    {
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.AddCssClass("inline-error");
      tagBuilder.SetInnerText(message);
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static void RenderPartial(
      this HtmlHelper helper,
      string areaName,
      string viewName,
      object viewModel)
    {
      using (PerformanceTimer performanceTimer = WebPerformanceTimer.StartMeasure(helper, "Html.RenderPartial"))
      {
        performanceTimer.AddProperty("area", (object) areaName);
        performanceTimer.AddProperty("view", (object) viewName);
        helper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (RenderPartial));
        try
        {
          HtmlHelper htmlHelper = helper;
          if (!string.IsNullOrEmpty(areaName) && !StringComparer.OrdinalIgnoreCase.Equals(helper.ViewContext.RouteData.GetRouteValue<string>("area", ""), areaName))
          {
            ViewContext viewContext = new ViewContext((ControllerContext) helper.ViewContext, helper.ViewContext.View, helper.ViewData, helper.ViewContext.TempData, helper.ViewContext.Writer);
            RouteData routeData = helper.ViewContext.RouteData.Clone();
            routeData.DataTokens["area"] = (object) areaName;
            viewContext.RequestContext = (RequestContext) new WrappedRequestContext(helper.ViewContext.RequestContext, routeData);
            htmlHelper = new HtmlHelper(viewContext, helper.ViewDataContainer, helper.RouteCollection);
          }
          htmlHelper.RenderPartial(viewName, viewModel);
        }
        finally
        {
          helper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (RenderPartial));
        }
      }
    }

    public static MvcHtmlString HelpLink(
      this HtmlHelper htmlHelper,
      string caption,
      string url,
      string className)
    {
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.AddCssClass(className);
      tagBuilder.InnerHtml = new TagBuilder("a")
      {
        Attributes = {
          ["href"] = url,
          ["tabindex"] = "0",
          ["target"] = "_blank"
        },
        InnerHtml = caption
      }.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public static void TraceEnter(
      this HtmlHelper htmlHelper,
      int tracePoint,
      string traceArea,
      string traceLayer,
      string methodName)
    {
      IVssRequestContext requestContext = htmlHelper.ViewContext.TfsRequestContext(false);
      if (requestContext == null)
        return;
      requestContext.TraceEnter(tracePoint, traceArea, traceLayer, methodName);
    }

    public static void TraceLeave(
      this HtmlHelper htmlHelper,
      int tracePoint,
      string traceArea,
      string traceLayer,
      string methodName)
    {
      IVssRequestContext requestContext = htmlHelper.ViewContext.TfsRequestContext(false);
      if (requestContext == null)
        return;
      requestContext.TraceLeave(tracePoint, traceArea, traceLayer, methodName);
    }

    public static MvcHtmlString IFrameControl(this HtmlHelper htmlHelper, string contentUrl)
    {
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.AddClass("iframe-control");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(htmlHelper.JsonIsland((object) new
      {
        contentUrl = contentUrl
      }, (object) new{ @class = "options" }).ToHtmlString());
      tagBuilder.InnerHtml = stringBuilder.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public static MvcHtmlString IFrameControl(
      this HtmlHelper htmlHelper,
      string contentUrl,
      string cssClass)
    {
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.AddClass("iframe-control");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(htmlHelper.JsonIsland((object) new
      {
        contentUrl = contentUrl,
        cssClass = cssClass
      }, (object) new{ @class = "options" }).ToHtmlString());
      tagBuilder.InnerHtml = stringBuilder.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public static string GenerateNonce(this HtmlHelper htmlHelper, bool includeAttribute = false)
    {
      string nonce = string.Empty;
      IVssRequestContext requestContext = htmlHelper.ViewContext.TfsRequestContext(false);
      if (requestContext != null && (requestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyFeatureFlag) || requestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyReportOnlyFeatureFlag)))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        nonce = vssRequestContext.GetService<IContentSecurityPolicyNonceManagementService>().GetNonceValue(vssRequestContext, htmlHelper.ViewContext.HttpContext);
      }
      if (includeAttribute && !string.IsNullOrEmpty(nonce))
        nonce = string.Format(" nonce=\"{0}\"", (object) nonce);
      return nonce;
    }
  }
}
