// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CoreReferencesContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class CoreReferencesContext : WebSdkMetadata
  {
    internal const string jQueryDebugFilename = "jquery-3.6.0.js";
    internal const string jQueryMinFilename = "jquery-3.6.0.min.js";
    internal const string jQueryCorsDebugFilename = "jQuery.XDomainRequest.js";
    internal const string jQueryCorsMinFilename = "jquery.xdomainrequest.min.js";
    internal const string RequireJsDebugFilename = "require.js";
    internal const string RequireJsMinFilename = "require.min.js";
    internal const string PromiseJsDebugFilename = "debug/promise.js";
    internal const string PromiseJsMinFilename = "min/promise.js";
    private const string PreLoaderShimDebugFilename = "pre-loader-shim.debug.js";
    private const string PreLoaderShimMinFilename = "pre-loader-shim.min.js";
    private const string PostLoaderShimDebugFilename = "post-loader-shim.debug.js";
    private const string PostLoaderShimMinFilename = "post-loader-shim.min.js";
    private const string GlobalScriptsDebugFilename = "debug/global-scripts.js";
    private const string GlobalScriptsMinFilename = "min/global-scripts.js";
    private const string TslibFilename = "tslib.js";
    private static readonly ScriptBundleDefinition s_baseJsModuleMin;
    private static readonly ScriptBundleDefinition s_baseJsModuleDebug;
    private static readonly ScriptBundleDefinition s_vssExtensionCoreMin;
    private static readonly ScriptBundleDefinition s_vssExtensionCoreDebug;
    private static readonly IList<IBundledCSSContent> s_coreCssBundleContent;

    static CoreReferencesContext()
    {
      BundleTextContent bundleTextContent = new BundleTextContent()
      {
        Text = "define('jquery', [], function() { return jQuery; });"
      };
      string physicalLocation1 = StaticResources.ThirdParty.Scripts.GetPhysicalLocation(string.Empty);
      string physicalLocation2 = StaticResources.Versioned.Scripts.TFS.GetPhysicalLocation(string.Empty);
      string physicalLocation3 = StaticResources.Versioned.Scripts.TFS.Debug.GetPhysicalLocation(string.Empty);
      ScriptBundleDefinition bundleDefinition1 = new ScriptBundleDefinition();
      bundleDefinition1.FriendlyName = "basejs";
      bundleDefinition1.ContentList = (IList<IBundledScriptContent>) BundlingHelper.GenerateInstrumentedContent(new List<IBundledScriptContent>()
      {
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation1, "jquery-3.6.0.min.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation1, "jquery.xdomainrequest.min.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation2, "min/promise.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation2, "pre-loader-shim.min.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation3, "tslib.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation1, "require.min.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation2, "post-loader-shim.min.js"),
        (IBundledScriptContent) bundleTextContent
      }, "basejs");
      CoreReferencesContext.s_baseJsModuleMin = bundleDefinition1;
      ScriptBundleDefinition bundleDefinition2 = new ScriptBundleDefinition();
      bundleDefinition2.FriendlyName = "basejs";
      bundleDefinition2.ContentList = (IList<IBundledScriptContent>) BundlingHelper.GenerateInstrumentedContent(new List<IBundledScriptContent>()
      {
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation1, "jquery-3.6.0.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation1, "jQuery.XDomainRequest.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation2, "debug/promise.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation2, "pre-loader-shim.debug.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation3, "tslib.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation1, "require.js"),
        CoreReferencesContext.CreateThirdPartyScriptContent(physicalLocation2, "post-loader-shim.debug.js"),
        (IBundledScriptContent) bundleTextContent
      }, "basejs");
      CoreReferencesContext.s_baseJsModuleDebug = bundleDefinition2;
      List<IBundledScriptContent> bundledScriptContentList = new List<IBundledScriptContent>()
      {
        (IBundledScriptContent) new BundleScriptModulesContent()
        {
          IncludedScripts = (ISet<string>) new HashSet<string>()
          {
            "tslib",
            "VSS/Bundling",
            "VSS/Service",
            "VSS/Error"
          }
        }
      };
      ScriptBundleDefinition bundleDefinition3 = new ScriptBundleDefinition();
      bundleDefinition3.DebugScripts = false;
      bundleDefinition3.FriendlyName = "ext-core";
      bundleDefinition3.ContentList = (IList<IBundledScriptContent>) bundledScriptContentList;
      CoreReferencesContext.s_vssExtensionCoreMin = bundleDefinition3;
      ScriptBundleDefinition bundleDefinition4 = new ScriptBundleDefinition();
      bundleDefinition4.DebugScripts = true;
      bundleDefinition4.FriendlyName = "ext-core";
      bundleDefinition4.ContentList = (IList<IBundledScriptContent>) bundledScriptContentList;
      CoreReferencesContext.s_vssExtensionCoreDebug = bundleDefinition4;
      CoreReferencesContext.s_coreCssBundleContent = (IList<IBundledCSSContent>) new List<IBundledCSSContent>()
      {
        (IBundledCSSContent) new BundleCSSContent()
        {
          IncludedCSSFiles = (ISet<string>) new HashSet<string>()
          {
            "jQueryUI-Modified",
            "Core",
            "ExtensionStyles",
            "Splitter",
            "PivotView"
          }
        }
      };
    }

    private static IBundledScriptContent CreateThirdPartyScriptContent(
      string baseThirdPartyPath,
      string thirdPartyScriptName)
    {
      return (IBundledScriptContent) new BundleFileContent()
      {
        Name = thirdPartyScriptName,
        LocalFilePath = (baseThirdPartyPath + thirdPartyScriptName)
      };
    }

    public CoreReferencesContext(WebContext webContext)
    {
      this.Stylesheets = this.GetDefaultStylesheetReferences(webContext);
      this.Scripts = this.GetDefaultScriptReferences(webContext);
      this.CoreScriptsBundle = this.GetCoreScriptBundleReference(webContext);
    }

    public CoreReferencesContext()
    {
    }

    [DataMember]
    public List<StylesheetReference> Stylesheets { get; set; }

    [DataMember]
    public List<JavascriptFileReference> Scripts { get; set; }

    [DataMember]
    public JavascriptFileReference CoreScriptsBundle { get; set; }

    [DataMember]
    public JavascriptFileReference ExtensionCoreReferences { get; set; }

    public BundledScriptFile ThirdPartyScriptsBundle { get; set; }

    public static List<JavascriptFileReference> GetThirdPartyScriptReferences(
      WebContext webContext,
      bool debugScripts)
    {
      List<JavascriptFileReference> scriptReferences = new List<JavascriptFileReference>();
      UrlHelper url = webContext.Url;
      scriptReferences.Add(new JavascriptFileReference()
      {
        Identifier = "JQuery",
        Url = StaticResources.ThirdParty.Scripts.GetLocation(debugScripts ? "jquery-3.6.0.js" : "jquery-3.6.0.min.js"),
        IsCoreModule = true
      });
      scriptReferences.Add(new JavascriptFileReference()
      {
        Identifier = "JQueryXDomain",
        Url = StaticResources.ThirdParty.Scripts.GetLocation(debugScripts ? "jQuery.XDomainRequest.js" : "jquery.xdomainrequest.min.js"),
        IsCoreModule = true
      });
      scriptReferences.Add(new JavascriptFileReference()
      {
        Identifier = "Promise",
        Url = StaticResources.Versioned.Scripts.TFS.GetLocation(debugScripts ? "debug/promise.js" : "min/promise.js"),
        IsCoreModule = true
      });
      scriptReferences.Add(new JavascriptFileReference()
      {
        Identifier = "GlobalScripts",
        Url = StaticResources.Versioned.Scripts.TFS.GetLocation(debugScripts ? "debug/global-scripts.js" : "min/global-scripts.js"),
        IsCoreModule = true
      });
      return scriptReferences;
    }

    public static string SelectJQueryMinVersionForBrowser() => "jquery-3.6.0.min.js";

    private static IEnumerable<JavascriptFileReference> GetLoaderScriptReferences(
      WebContext webContext,
      bool debugScripts)
    {
      return (IEnumerable<JavascriptFileReference>) new JavascriptFileReference[3]
      {
        new JavascriptFileReference()
        {
          Identifier = "LoaderFixes",
          Url = StaticResources.Versioned.Scripts.TFS.GetLocation(debugScripts ? "pre-loader-shim.debug.js" : "pre-loader-shim.min.js")
        },
        new JavascriptFileReference()
        {
          Identifier = "AMDLoader",
          Url = StaticResources.ThirdParty.Scripts.GetLocation(debugScripts ? "require.js" : "require.min.js"),
          IsCoreModule = true
        },
        new JavascriptFileReference()
        {
          Identifier = "LoaderFixes",
          Url = StaticResources.Versioned.Scripts.TFS.GetLocation(debugScripts ? "post-loader-shim.debug.js" : "post-loader-shim.min.js")
        }
      };
    }

    private List<StylesheetReference> GetDefaultStylesheetReferences(WebContext webContext)
    {
      List<StylesheetReference> stylesheetReferences = new List<StylesheetReference>();
      CSSBundleDefinition bundleDefinition = new CSSBundleDefinition();
      bundleDefinition.DebugStyles = webContext.Diagnostics.DebugMode;
      bundleDefinition.ContentList = CoreReferencesContext.s_coreCssBundleContent;
      bundleDefinition.FriendlyName = "ext-core-css";
      bundleDefinition.ThemeName = webContext.Url.ThemeName();
      CSSBundleDefinition cssBundle = bundleDefinition;
      BundledCSSFile bundle = BundlingHelper.RegisterCSSBundle(webContext, cssBundle);
      stylesheetReferences.Add(new StylesheetReference()
      {
        Url = BundlingHelper.GetLocalBundleCssUrl(webContext.TfsRequestContext, bundle),
        IsCoreStylesheet = true
      });
      return stylesheetReferences;
    }

    private List<JavascriptFileReference> GetDefaultScriptReferences(WebContext webContext)
    {
      List<JavascriptFileReference> scriptReferences = new List<JavascriptFileReference>();
      bool debugMode = webContext.Diagnostics.DebugMode;
      scriptReferences.AddRange((IEnumerable<JavascriptFileReference>) CoreReferencesContext.GetThirdPartyScriptReferences(webContext, debugMode));
      scriptReferences.AddRange(CoreReferencesContext.GetLoaderScriptReferences(webContext, debugMode));
      return scriptReferences;
    }

    private JavascriptFileReference GetCoreScriptBundleReference(WebContext webContext)
    {
      JavascriptFileReference scriptBundleReference = (JavascriptFileReference) null;
      if (webContext.Diagnostics.BundlingEnabled)
      {
        ScriptBundleDefinition scriptBundle1 = webContext.Diagnostics.DebugMode ? CoreReferencesContext.s_baseJsModuleDebug : CoreReferencesContext.s_baseJsModuleMin;
        this.ThirdPartyScriptsBundle = BundlingHelper.RegisterScriptBundle(webContext, scriptBundle1);
        scriptBundleReference = new JavascriptFileReference()
        {
          Identifier = "CoreBundle",
          Url = BundlingHelper.GetLocalBundleScriptUrl(webContext.TfsRequestContext, this.ThirdPartyScriptsBundle),
          IsCoreModule = true
        };
        ScriptBundleDefinition scriptBundle2 = webContext.Diagnostics.DebugMode ? CoreReferencesContext.s_vssExtensionCoreDebug : CoreReferencesContext.s_vssExtensionCoreMin;
        BundledScriptFile bundle = BundlingHelper.RegisterScriptBundle(webContext, scriptBundle2);
        string str = (string) null;
        if (webContext.TfsRequestContext.GetService<IWebDiagnosticsService>().IsCdnEnabled(webContext.TfsRequestContext) && !string.IsNullOrEmpty(bundle.CDNRelativeUri))
        {
          IVssRequestContext vssRequestContext = webContext.TfsRequestContext.To(TeamFoundationHostType.Deployment);
          string cdnUrl = vssRequestContext.GetService<ICdnLocationService>().GetCdnUrl(vssRequestContext, bundle.CDNRelativeUri);
          if (!string.IsNullOrEmpty(cdnUrl))
            str = cdnUrl;
        }
        if (string.IsNullOrEmpty(str))
          str = BundlingHelper.GetLocalBundleScriptUrl(webContext.TfsRequestContext, bundle);
        this.ExtensionCoreReferences = new JavascriptFileReference()
        {
          Identifier = "CoreBundle",
          IsCoreModule = true,
          Url = str
        };
      }
      return scriptBundleReference;
    }
  }
}
