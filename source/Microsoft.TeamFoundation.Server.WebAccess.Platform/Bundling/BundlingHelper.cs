// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundlingHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public static class BundlingHelper
  {
    private const string GetFallbackThemeLocalUriMessage = "GetFallbackThemeLocalUri";
    private const string GetLocalBundleCssUrlMessage = "GetLocalBundleCssUrl";
    private const string GetLocalBundleScriptUrlMessage = "GetLocalBundleScriptUrl";
    private const string TraceArea = "BundlingHelper";
    private const string TraceLayer = "WebPlatform";
    public static readonly IEnumerable<string> ExcludedNewPlatformPaths = (IEnumerable<string>) new string[10]
    {
      "create-react-class",
      "react",
      "react-dom",
      "react-transition-group",
      "react-dom-factories",
      "@microsoft/load-themed-styles",
      "@uifabric",
      "OfficeFabric",
      "prop-types",
      "VSSUI"
    };
    private static ConcurrentDictionary<string, string> s_contentPathMinPrefix = new ConcurrentDictionary<string, string>();
    private static ConcurrentDictionary<string, string> s_contentPathDebugPrefix = new ConcurrentDictionary<string, string>();
    private static ITraceRequest RawTracer = (ITraceRequest) new RawTraceRequest();

    public static BundledScriptFile RegisterScriptBundle(
      WebContext webContext,
      ScriptBundleDefinition scriptBundle)
    {
      return BundlingHelper.RegisterScriptBundles(webContext, (IEnumerable<ScriptBundleDefinition>) new ScriptBundleDefinition[1]
      {
        scriptBundle
      }).First<BundledScriptFile>();
    }

    public static IList<BundledScriptFile> RegisterScriptBundles(
      WebContext webContext,
      IEnumerable<ScriptBundleDefinition> scriptBundles)
    {
      IVssRequestContext vssRequestContext = webContext.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      BundlingService service = vssRequestContext.GetService<BundlingService>();
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<BundledScriptFile> bundledScriptFileList = new List<BundledScriptFile>();
      foreach (ScriptBundleDefinition scriptBundle in scriptBundles)
      {
        HashSet<string> other = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (BundleScriptModulesContent scriptModulesContent in scriptBundle.ContentList.OfType<BundleScriptModulesContent>())
        {
          if (scriptModulesContent.IncludedScripts != null && scriptModulesContent.IncludedScripts.Any<string>())
          {
            other.UnionWith((IEnumerable<string>) scriptModulesContent.IncludedScripts);
            if (stringSet.Any<string>())
            {
              if (scriptModulesContent.ExcludedScripts == null)
                scriptModulesContent.ExcludedScripts = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
              scriptModulesContent.ExcludedScripts.UnionWith((IEnumerable<string>) stringSet);
            }
          }
        }
        BundledScriptFile bundledScriptFile = service.RegisterScriptBundle(vssRequestContext, scriptBundle, webContext.Url);
        bundledScriptFileList.Add(bundledScriptFile);
        stringSet.UnionWith((IEnumerable<string>) other);
      }
      return (IList<BundledScriptFile>) bundledScriptFileList;
    }

    public static BundledCSSFile RegisterCSSBundle(
      WebContext webContext,
      CSSBundleDefinition cssBundle)
    {
      return BundlingHelper.RegisterCSSBundles(webContext, (IEnumerable<CSSBundleDefinition>) new CSSBundleDefinition[1]
      {
        cssBundle
      }).First<BundledCSSFile>();
    }

    public static IList<BundledCSSFile> RegisterCSSBundles(
      WebContext webContext,
      IEnumerable<CSSBundleDefinition> cssBundles)
    {
      IVssRequestContext vssRequestContext = webContext.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      BundlingService service = vssRequestContext.GetService<BundlingService>();
      List<BundledCSSFile> bundledCssFileList = new List<BundledCSSFile>();
      HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (CSSBundleDefinition cssBundle in cssBundles)
      {
        HashSet<string> other = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (BundleCSSContent bundleCssContent in cssBundle.ContentList.OfType<BundleCSSContent>())
        {
          if (stringSet1.Any<string>())
          {
            if (bundleCssContent.ExcludedCSSFiles == null)
              bundleCssContent.ExcludedCSSFiles = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            bundleCssContent.ExcludedCSSFiles.UnionWith((IEnumerable<string>) stringSet1);
          }
          if (stringSet2.Any<string>())
          {
            if (bundleCssContent.ExcludedScripts == null)
              bundleCssContent.ExcludedScripts = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            bundleCssContent.ExcludedScripts.UnionWith((IEnumerable<string>) stringSet2);
          }
          if (bundleCssContent.IncludedScripts != null)
            other.UnionWith((IEnumerable<string>) bundleCssContent.IncludedScripts);
        }
        BundledCSSFile bundledCssFile = service.RegisterCSSBundle(vssRequestContext, cssBundle, webContext.Url);
        bundledCssFileList.Add(bundledCssFile);
        if (!string.Equals(cssBundle.ThemeName, ThemesUtility.HighContrastThemeName, StringComparison.OrdinalIgnoreCase))
        {
          BundledFile bundledFile = (BundledFile) service.RegisterCSSBundle(vssRequestContext, new CSSBundleDefinition(cssBundle)
          {
            ThemeName = ThemesUtility.HighContrastThemeName
          }, webContext.Url);
          bundledCssFile.FallbackThemeLocalUri = bundledFile.LocalUri;
          bundledCssFile.FallbackThemeCDNRelativeUri = bundledFile.CDNRelativeUri;
        }
        stringSet1.UnionWith((IEnumerable<string>) bundledCssFile.IncludedCssFiles);
        stringSet2.UnionWith((IEnumerable<string>) other);
      }
      return (IList<BundledCSSFile>) bundledCssFileList;
    }

    public static MvcHtmlString GetScriptBundlesHtml(
      HtmlHelper htmlHelper,
      IEnumerable<BundledScriptFile> bundledScriptFiles)
    {
      WebContext webContext = WebContextFactory.GetWebContext(htmlHelper.ViewContext.RequestContext);
      StringBuilder builder = new StringBuilder();
      foreach (BundledScriptFile bundledScriptFile in bundledScriptFiles)
      {
        string bundleScriptUrl = BundlingHelper.GetBundleScriptUrl(htmlHelper.ViewContext.TfsRequestContext(false), bundledScriptFile, webContext.Diagnostics.CdnEnabled);
        IEnumerable<string> includedScripts = bundledScriptFile.Definition.ContentList.OfType<BundleScriptModulesContent>().SelectMany<BundleScriptModulesContent, string>((Func<BundleScriptModulesContent, IEnumerable<string>>) (a => (IEnumerable<string>) a.IncludedScripts));
        TagBuilder tagBuilder1 = new TagBuilder("script");
        tagBuilder1.MergeAttribute("type", "text/javascript");
        tagBuilder1.AddNonceAttribute(htmlHelper.ViewContext.TfsRequestContext(false), htmlHelper.ViewContext.HttpContext);
        tagBuilder1.Html("if (window.performance && window.performance.mark) { window.performance.mark('startLoadBundleOuter-" + bundledScriptFile.Definition.FriendlyName + "'); }");
        TagBuilder tagBuilder2 = new TagBuilder("script");
        tagBuilder2.MergeAttribute("type", "text/javascript");
        tagBuilder2.AddNonceAttribute(htmlHelper.ViewContext.TfsRequestContext(false), htmlHelper.ViewContext.HttpContext);
        tagBuilder2.Html("if (window.performance && window.performance.mark) { window.performance.mark('endLoadBundleOuter-" + bundledScriptFile.Definition.FriendlyName + "'); }");
        builder.AppendLine(tagBuilder1.ToString());
        BundlingHelper.AppendBundleScript(webContext, builder, bundleScriptUrl, includedScripts, bundledScriptFile.Definition.FriendlyName, bundledScriptFile.ContentLength, bundledScriptFile.Integrity);
        builder.AppendLine(tagBuilder2.ToString());
      }
      return MvcHtmlString.Create(builder.ToString());
    }

    public static MvcHtmlString GetScriptBundlesNewPlatformHtml(
      HtmlHelper htmlHelper,
      IEnumerable<BundledScriptFile> bundledScriptFiles,
      string lastBundleCallback)
    {
      PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
      StringBuilder stringBuilder = new StringBuilder();
      List<BundledScriptFile> list = bundledScriptFiles.Where<BundledScriptFile>((Func<BundledScriptFile, bool>) (b => !b.IsEmpty)).ToList<BundledScriptFile>();
      for (int index = 0; index < list.Count; ++index)
      {
        BundledScriptFile bundle = list[index];
        string bundleScriptUrl = BundlingHelper.GetBundleScriptUrl(htmlHelper.ViewContext.TfsRequestContext(false), bundle, pageContext.WebContext.Diagnostics.CdnEnabled);
        IEnumerable<string> strings = bundle.Definition.ContentList.OfType<BundleScriptModulesContent>().SelectMany<BundleScriptModulesContent, string>((Func<BundleScriptModulesContent, IEnumerable<string>>) (a => (IEnumerable<string>) a.IncludedScripts));
        TagBuilder tagBuilder = new TagBuilder("script");
        tagBuilder.MergeAttribute("type", "text/javascript");
        tagBuilder.AddNonceAttribute(pageContext.WebContext.TfsRequestContext, pageContext.WebContext.RequestContext.HttpContext);
        if (strings != null && strings.Any<string>())
          tagBuilder.MergeAttribute("data-includedscripts", string.Join(";", strings));
        if (!string.IsNullOrEmpty(bundle.Definition.FriendlyName))
          tagBuilder.MergeAttribute("data-bundlename", bundle.Definition.FriendlyName);
        tagBuilder.MergeAttribute("data-bundlelength", bundle.ContentLength.ToString());
        string str1 = string.Empty;
        if (!string.IsNullOrEmpty(bundle.Integrity) && pageContext.WebContext.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.SubresourceIntegrity"))
          str1 = ", integrity:'" + bundle.Integrity + "'";
        string str2 = string.Empty;
        if (index == list.Count - 1 && !string.IsNullOrEmpty(lastBundleCallback))
          str2 = ".then(function() {" + Environment.NewLine + lastBundleCallback + Environment.NewLine + "})";
        tagBuilder.InnerHtml = string.Format("window.__loadScript({{ url:'{0}', clientId: '{1}', contributionId:'{2}', contentType:'text/javascript', contentLength:{3}{4} }}){5};" + Environment.NewLine, (object) bundleScriptUrl, (object) bundle.Name, (object) bundle.Definition.FriendlyName, (object) bundle.ContentLength, (object) str1, (object) str2);
        stringBuilder.AppendLine(tagBuilder.ToString());
      }
      return MvcHtmlString.Create(stringBuilder.ToString());
    }

    public static MvcHtmlString GetCSSBundlesHtml(
      HtmlHelper htmlHelper,
      IEnumerable<BundledCSSFile> bundledCssFiles)
    {
      WebContext webContext = WebContextFactory.GetWebContext(htmlHelper.ViewContext.RequestContext);
      IVssRequestContext requestContext = htmlHelper.ViewContext.TfsRequestContext(false);
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      if (requestContext != null)
      {
        vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<BundlingService>();
      }
      StringBuilder builder = new StringBuilder();
      foreach (BundledCSSFile bundledCssFile in bundledCssFiles)
      {
        if (!bundledCssFile.IsEmpty)
        {
          string url = BundlingHelper.GetLocalBundleCssUrl(requestContext, bundledCssFile);
          string fallbackThemeUrl = (string) null;
          if (webContext.Diagnostics.CdnEnabled && requestContext != null && vssRequestContext != null && !string.IsNullOrEmpty(bundledCssFile.CDNRelativeUri))
          {
            ICdnLocationService service = vssRequestContext.GetService<ICdnLocationService>();
            string cdnUrl = service.GetCdnUrl(vssRequestContext, bundledCssFile.CDNRelativeUri);
            if (!string.IsNullOrEmpty(cdnUrl))
            {
              url = cdnUrl;
              if (bundledCssFile.FallbackThemeCDNRelativeUri != null)
                fallbackThemeUrl = service.GetCdnUrl(vssRequestContext, bundledCssFile.FallbackThemeCDNRelativeUri);
            }
          }
          if (string.IsNullOrEmpty(fallbackThemeUrl))
            fallbackThemeUrl = BundlingHelper.GetFallbackThemeLocalUri(webContext, bundledCssFile);
          BundlingHelper.AppendBundleCSS(webContext, htmlHelper, builder, url, fallbackThemeUrl, (IEnumerable<string>) bundledCssFile.IncludedCssFiles, bundledCssFile.Definition.FriendlyName, bundledCssFile.ContentLength);
        }
      }
      return MvcHtmlString.Create(builder.ToString());
    }

    public static DynamicBundlesCollection RegisterDynamicBundles(
      WebContext webContext,
      string bundleFriendlyNamePrefix,
      ISet<string> scriptsToInclude,
      ISet<string> scriptsToExclude,
      ISet<string> cssToInclude,
      ISet<string> cssToExclude,
      IEnumerable<string> pathsToExclude,
      IEnumerable<string> serviceCssModulePrefixes,
      string staticContentVersion,
      bool? diagnoseBundles = null,
      string theme = null)
    {
      DynamicBundlesCollection bundlesCollection = new DynamicBundlesCollection();
      ScriptBundleDefinition bundleDefinition1 = new ScriptBundleDefinition();
      bundleDefinition1.FriendlyName = bundleFriendlyNamePrefix;
      bundleDefinition1.Culture = CultureInfo.CurrentUICulture;
      bundleDefinition1.DebugScripts = webContext.Diagnostics.DebugMode;
      bundleDefinition1.Diagnose = diagnoseBundles.HasValue ? diagnoseBundles.Value : webContext.Diagnostics.DiagnoseBundles;
      bundleDefinition1.StaticContentVersion = staticContentVersion;
      bundleDefinition1.ContentList = (IList<IBundledScriptContent>) BundlingHelper.GenerateInstrumentedContent(new List<IBundledScriptContent>()
      {
        (IBundledScriptContent) new BundleScriptModulesContent()
        {
          IncludedScripts = scriptsToInclude,
          ExcludedScripts = scriptsToExclude,
          ExcludedPaths = pathsToExclude
        }
      }, bundleFriendlyNamePrefix);
      ScriptBundleDefinition scriptBundle = bundleDefinition1;
      IVssRequestContext vssRequestContext = webContext.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      BundledScriptFile bundle = BundlingHelper.RegisterScriptBundle(webContext, scriptBundle);
      if (!bundle.IsEmpty)
      {
        DynamicScriptBundle dynamicScriptBundle = new DynamicScriptBundle();
        if (webContext.TfsRequestContext != null && webContext.Diagnostics.CdnEnabled && !string.IsNullOrEmpty(bundle.CDNRelativeUri))
        {
          string cdnUrl = vssRequestContext.GetService<ICdnLocationService>().GetCdnUrl(vssRequestContext, bundle.CDNRelativeUri);
          if (!string.IsNullOrEmpty(cdnUrl))
            dynamicScriptBundle.Uri = cdnUrl;
        }
        if (string.IsNullOrEmpty(dynamicScriptBundle.Uri))
          dynamicScriptBundle.Uri = BundlingHelper.GetLocalBundleScriptUrl(webContext.TfsRequestContext, bundle);
        dynamicScriptBundle.ClientId = bundle.Name;
        dynamicScriptBundle.ContentLength = bundle.ContentLength;
        dynamicScriptBundle.Integrity = bundle.Integrity;
        bundlesCollection.Scripts = new List<DynamicScriptBundle>();
        bundlesCollection.Scripts.Add(dynamicScriptBundle);
        if (bundle.ScriptsExcludedByPath != null && bundle.ScriptsExcludedByPath.Any<string>())
          bundlesCollection.ScriptsExcludedByPath = bundle.ScriptsExcludedByPath.ToList<string>();
      }
      CSSBundleDefinition bundleDefinition2 = new CSSBundleDefinition();
      bundleDefinition2.FriendlyName = bundleFriendlyNamePrefix + "css";
      bundleDefinition2.DebugStyles = webContext.Diagnostics.DebugMode;
      bundleDefinition2.ThemeName = !string.IsNullOrEmpty(theme) ? theme : webContext.Url.ThemeName();
      bundleDefinition2.CSSModulePrefixes = serviceCssModulePrefixes;
      bundleDefinition2.StaticContentVersion = staticContentVersion;
      bundleDefinition2.ContentList = (IList<IBundledCSSContent>) new List<IBundledCSSContent>()
      {
        (IBundledCSSContent) new BundleCSSContent()
        {
          IncludedCSSFiles = cssToInclude,
          ExcludedCSSFiles = cssToExclude,
          IncludedScripts = scriptsToInclude,
          ExcludedScripts = scriptsToExclude,
          ExcludedScriptPaths = pathsToExclude
        }
      };
      CSSBundleDefinition cssBundle = bundleDefinition2;
      BundledCSSFile bundledCssFile = BundlingHelper.RegisterCSSBundle(webContext, cssBundle);
      if (bundledCssFile != null && !bundledCssFile.IsEmpty)
      {
        DynamicCSSBundle dynamicCssBundle = new DynamicCSSBundle();
        dynamicCssBundle.Uri = BundlingHelper.GetLocalBundleCssUrl(webContext.TfsRequestContext, bundledCssFile);
        if (webContext.Diagnostics.CdnEnabled && webContext.TfsRequestContext != null && !string.IsNullOrEmpty(bundledCssFile.CDNRelativeUri))
        {
          ICdnLocationService service = vssRequestContext.GetService<ICdnLocationService>();
          string cdnUrl = service.GetCdnUrl(vssRequestContext, bundledCssFile.CDNRelativeUri);
          if (!string.IsNullOrEmpty(cdnUrl))
          {
            dynamicCssBundle.Uri = cdnUrl;
            if (bundledCssFile.FallbackThemeCDNRelativeUri != null)
              dynamicCssBundle.FallbackThemeUri = service.GetCdnUrl(vssRequestContext, bundledCssFile.FallbackThemeCDNRelativeUri);
          }
        }
        if (string.IsNullOrEmpty(dynamicCssBundle.FallbackThemeUri) && webContext.TfsRequestContext != null)
          dynamicCssBundle.FallbackThemeUri = BundlingHelper.GetFallbackThemeLocalUri(webContext, bundledCssFile);
        dynamicCssBundle.ClientId = bundledCssFile.Name;
        dynamicCssBundle.CssFiles = bundledCssFile.IncludedCssFiles.ToList<string>();
        dynamicCssBundle.ContentLength = bundledCssFile.ContentLength;
        bundlesCollection.Styles = new List<DynamicCSSBundle>();
        bundlesCollection.Styles.Add(dynamicCssBundle);
      }
      return bundlesCollection;
    }

    public static void RegisterPlugin(string path, string loadAfter) => VssScriptsBundleBuilder.PluginModules.AddPlugin(loadAfter, path);

    public static string GetFallbackThemeLocalUri(
      WebContext webContext,
      BundledCSSFile bundledCssFile)
    {
      string fallbackThemeLocalUri = bundledCssFile.FallbackThemeLocalUri;
      BundlingHelper.TraceIfNullRequestContext(webContext.TfsRequestContext, nameof (GetFallbackThemeLocalUri));
      return fallbackThemeLocalUri;
    }

    public static string GetLocalBundleCssUrl(
      IVssRequestContext requestContext,
      BundledCSSFile bundle)
    {
      string localBundleCssUrl = bundle.LocalUri;
      if (requestContext != null)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        localBundleCssUrl = vssRequestContext.GetService<BundlingService>().GetLocalCssUri(vssRequestContext, bundle.Definition.ThemeName, bundle.Name, bundle.Definition.StaticContentVersion);
        requestContext.Trace(100130005, TraceLevel.Info, nameof (BundlingHelper), "WebPlatform", "Generated bundle css uri: {0}", (object) localBundleCssUrl);
      }
      BundlingHelper.TraceIfNullRequestContext(requestContext, nameof (GetLocalBundleCssUrl));
      return localBundleCssUrl;
    }

    public static string GetLocalBundleScriptUrl(
      IVssRequestContext requestContext,
      BundledScriptFile bundle)
    {
      string localBundleScriptUrl = (string) null;
      if (!bundle.IsEmpty)
      {
        localBundleScriptUrl = bundle.LocalUri;
        if (requestContext != null)
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          localBundleScriptUrl = vssRequestContext.GetService<BundlingService>().GetLocalScriptUri(vssRequestContext, bundle.Name);
          requestContext.Trace(100130010, TraceLevel.Info, nameof (BundlingHelper), "WebPlatform", "GetLocalBundleScriptUrl : {0}", (object) localBundleScriptUrl);
        }
        BundlingHelper.TraceIfNullRequestContext(requestContext, nameof (GetLocalBundleScriptUrl));
      }
      return localBundleScriptUrl;
    }

    private static void TraceIfNullRequestContext(
      IVssRequestContext requestContext,
      string messageContext)
    {
      if (requestContext != null)
        return;
      BundlingHelper.RawTracer.Trace(100130001, TraceLevel.Info, nameof (BundlingHelper), "WebPlatform", "{0} accessed from: {1}", (object) messageContext, HttpContext.Current == null || HttpContext.Current.Request == null || !(HttpContext.Current.Request.Url != (Uri) null) ? (object) "Url not available" : (object) HttpContext.Current.Request.Url.ToString());
    }

    public static string GetBundleScriptUrl(
      IVssRequestContext requestContext,
      BundledScriptFile bundle,
      bool useCDN)
    {
      if (bundle.IsEmpty)
        return (string) null;
      string bundleScriptUrl = (string) null;
      if (useCDN && requestContext != null && !string.IsNullOrEmpty(bundle.CDNRelativeUri))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        bundleScriptUrl = vssRequestContext.GetService<ICdnLocationService>().GetCdnUrl(vssRequestContext, bundle.CDNRelativeUri);
      }
      if (string.IsNullOrEmpty(bundleScriptUrl))
        bundleScriptUrl = BundlingHelper.GetLocalBundleScriptUrl(requestContext, bundle);
      return bundleScriptUrl;
    }

    public static void AppendBundleScript(
      WebContext webContext,
      StringBuilder builder,
      string scriptUrl,
      IEnumerable<string> includedScripts,
      string bundleName,
      int contentLength,
      string integrity)
    {
      if (string.IsNullOrEmpty(scriptUrl))
        return;
      TagBuilder tagBuilder = new TagBuilder("script");
      tagBuilder.MergeAttribute("type", "text/javascript");
      tagBuilder.MergeAttribute("src", scriptUrl);
      tagBuilder.AddNonceAttribute(webContext.TfsRequestContext, webContext.RequestContext.HttpContext);
      if (includedScripts != null && includedScripts.Any<string>())
        tagBuilder.MergeAttribute("data-includedscripts", string.Join(";", includedScripts));
      if (!string.IsNullOrEmpty(bundleName))
        tagBuilder.MergeAttribute("data-bundlename", bundleName);
      if (!string.IsNullOrEmpty(integrity) && webContext.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.SubresourceIntegrity"))
      {
        tagBuilder.MergeAttribute(nameof (integrity), integrity);
        tagBuilder.MergeAttribute("crossorigin", "anonymous");
      }
      tagBuilder.MergeAttribute("data-bundlelength", contentLength.ToString());
      builder.AppendLine(tagBuilder.ToString());
    }

    public static void AppendBundleCSS(
      WebContext webContext,
      HtmlHelper htmlHelper,
      StringBuilder builder,
      string url,
      string fallbackThemeUrl,
      IEnumerable<string> includedCssFiles,
      string bundleName,
      int contentLength)
    {
      TagBuilder cssStyleTag = htmlHelper.GetCssStyleTag(url, fallbackThemeUrl);
      cssStyleTag.MergeAttribute("data-includedstyles", string.Join(";", includedCssFiles));
      if (!string.IsNullOrEmpty(bundleName))
        cssStyleTag.MergeAttribute("data-bundlename", bundleName);
      cssStyleTag.MergeAttribute("data-bundlelength", contentLength.ToString());
      builder.AppendLine(cssStyleTag.ToString(TagRenderMode.SelfClosing));
    }

    internal static List<BundleDefinition> GetBundleDefinitionsFromQueryString(
      IVssRequestContext requestContext,
      string bundleDefinitionString)
    {
      List<BundleDefinition> definitionsFromQueryString = new List<BundleDefinition>(2);
      NameValueCollection queryString = HttpUtility.ParseQueryString(bundleDefinitionString);
      string friendlyName = "async";
      string name = queryString["loc"];
      CultureInfo cultureInfo = (CultureInfo) null;
      if (!string.IsNullOrEmpty(name))
      {
        try
        {
          cultureInfo = CultureInfo.GetCultureInfo(name);
        }
        catch
        {
          cultureInfo = CultureInfo.CurrentUICulture;
        }
      }
      HashSet<string> setFromParam1 = BundlingHelper.GetSetFromParam(queryString["excludepaths"]);
      if (queryString["lwp"] == "true")
        setFromParam1.UnionWith(BundlingHelper.ExcludedNewPlatformPaths);
      bool flag = queryString["debug"] == "1";
      string str = queryString["v"];
      HashSet<string> setFromParam2 = BundlingHelper.GetSetFromParam(queryString["scripts"]);
      HashSet<string> setFromParam3 = BundlingHelper.GetSetFromParam(queryString["exclude"]);
      List<BundleDefinition> bundleDefinitionList = definitionsFromQueryString;
      ScriptBundleDefinition bundleDefinition1 = new ScriptBundleDefinition();
      bundleDefinition1.FriendlyName = friendlyName;
      bundleDefinition1.Culture = cultureInfo;
      bundleDefinition1.DebugScripts = flag;
      bundleDefinition1.StaticContentVersion = str;
      bundleDefinition1.ContentList = (IList<IBundledScriptContent>) BundlingHelper.GenerateInstrumentedContent(new List<IBundledScriptContent>()
      {
        (IBundledScriptContent) new BundleScriptModulesContent()
        {
          IncludedScripts = (ISet<string>) setFromParam2,
          ExcludedScripts = (ISet<string>) setFromParam3,
          ExcludedPaths = (IEnumerable<string>) setFromParam1
        }
      }, friendlyName);
      bundleDefinitionList.Add((BundleDefinition) bundleDefinition1);
      CSSBundleDefinition bundleDefinition2 = new CSSBundleDefinition();
      bundleDefinition2.FriendlyName = friendlyName + "css";
      bundleDefinition2.DebugStyles = flag;
      bundleDefinition2.ThemeName = queryString["theme"];
      bundleDefinition2.StaticContentVersion = str;
      bundleDefinition2.ContentList = (IList<IBundledCSSContent>) new List<IBundledCSSContent>()
      {
        (IBundledCSSContent) new BundleCSSContent()
        {
          IncludedCSSFiles = (ISet<string>) BundlingHelper.GetSetFromParam(queryString["includeCss"]),
          ExcludedCSSFiles = (ISet<string>) BundlingHelper.GetSetFromParam(queryString["excludeCss"]),
          IncludedScripts = (ISet<string>) setFromParam2,
          ExcludedScripts = (ISet<string>) setFromParam3,
          ExcludedScriptPaths = (IEnumerable<string>) setFromParam1
        }
      };
      return definitionsFromQueryString;
    }

    public static HashSet<string> GetSetFromParam(string paramValue)
    {
      if (string.IsNullOrEmpty(paramValue))
        return new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string str1 = "";
      List<string> collection = new List<string>();
      string str2 = paramValue;
      char[] separator = new char[1]{ ';' };
      foreach (string str3 in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        string path = str3;
        if (str3.StartsWith("*"))
          path = str1 + "/" + str3.Substring(1);
        else if (str3.StartsWith("-"))
        {
          string[] source = str1.Split('/');
          int num = 0;
          while (str3[num] == '-' && num < source.Length && num < str3.Length)
            ++num;
          if (num > 0)
            path = string.Join("/", ((IEnumerable<string>) source).Take<string>(num)) + "/" + str3.Substring(num);
        }
        string directoryName = Path.GetDirectoryName(path);
        if (directoryName != null)
        {
          str1 = directoryName.Replace(Path.DirectorySeparatorChar, '/');
          collection.Add(path);
        }
        else
          str1 = "";
      }
      return new HashSet<string>((IEnumerable<string>) collection, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public static List<IBundledScriptContent> GenerateInstrumentedContent(
      List<IBundledScriptContent> bundleContent,
      string friendlyName)
    {
      List<IBundledScriptContent> instrumentedContent = new List<IBundledScriptContent>();
      IBundledScriptContent bundledScriptContent1 = (IBundledScriptContent) new BundleTextContent()
      {
        Text = ("if (window.performance && window.performance.mark) { window.performance.mark('startLoadBundleInner-" + friendlyName + "'); }")
      };
      IBundledScriptContent bundledScriptContent2 = (IBundledScriptContent) new BundleTextContent()
      {
        Text = ("if (window.performance && window.performance.mark) { window.performance.mark('endLoadBundleInner-" + friendlyName + "'); }")
      };
      instrumentedContent.Add(bundledScriptContent1);
      instrumentedContent.AddRange((IEnumerable<IBundledScriptContent>) bundleContent);
      instrumentedContent.Add(bundledScriptContent2);
      return instrumentedContent;
    }

    public static string JoinWithNewLine(IEnumerable<string> lines)
    {
      int capacity = 0;
      foreach (string line in lines)
        capacity += line.Length + 2;
      StringBuilder stringBuilder = new StringBuilder(capacity);
      bool flag = true;
      foreach (string line in lines)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append(line);
      }
      return stringBuilder.ToString();
    }

    public static byte[] CalculateHashFromLines(string[] lines) => lines.Length != 0 || lines.Length == 1 && lines[0] != string.Empty ? BundlingHelper.CalculateHashFromBytes(Encoding.UTF8.GetBytes(BundlingHelper.JoinWithNewLine((IEnumerable<string>) lines))) : new byte[0];

    public static byte[] CalculateHashFromBytes(byte[] bytes)
    {
      if (bytes.Length == 0)
        return new byte[0];
      using (SHA256Cng shA256Cng = new SHA256Cng())
        return shA256Cng.ComputeHash(bytes);
    }

    public static string GetHashCodeFromHashList(IEnumerable<byte[]> contentHashList)
    {
      byte[] array = contentHashList.SelectMany<byte[], byte>((Func<byte[], IEnumerable<byte>>) (h => (IEnumerable<byte>) h)).ToArray<byte>();
      return array.Length != 0 ? Convert.ToBase64String(BundlingHelper.CalculateHashFromBytes(array)) : string.Empty;
    }

    public static string GetCacheableStaticUrl(
      IVssRequestContext requestContext,
      string staticResourcePath)
    {
      if (Uri.IsWellFormedUriString(staticResourcePath, UriKind.Absolute) || !requestContext.IsFeatureEnabled("VisualStudio.Services.Content.UseDeploymentUrlForBundling"))
        return staticResourcePath;
      staticResourcePath = VirtualPathUtility.ToAppRelative(staticResourcePath, requestContext.WebApplicationPath()).TrimStart('~');
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ILocationService service = vssRequestContext.GetService<ILocationService>();
      AccessMapping publicAccessMapping = service.GetPublicAccessMapping(vssRequestContext);
      staticResourcePath = service.LocationForAccessMapping(vssRequestContext, staticResourcePath, RelativeToSetting.WebApplication, publicAccessMapping);
      return staticResourcePath;
    }

    internal static string ConvertModuleToPath(
      string module,
      UrlHelper urlHelper,
      bool debugMode,
      string contentVersion)
    {
      string key = contentVersion ?? string.Empty;
      ConcurrentDictionary<string, string> concurrentDictionary = debugMode ? BundlingHelper.s_contentPathDebugPrefix : BundlingHelper.s_contentPathMinPrefix;
      string str;
      if (!concurrentDictionary.TryGetValue(key, out str))
      {
        str = urlHelper.TfsScriptContent(string.Empty, debugMode, true, contentVersion);
        concurrentDictionary[key] = str;
      }
      return string.Format("{0}{1}.js", (object) str, (object) module);
    }
  }
}
