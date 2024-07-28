// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.LegacyPlatformShimService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  internal class LegacyPlatformShimService : ILegacyPlatformShimService, IVssFrameworkService
  {
    private const string c_legacyPlatformShimRequestKey = "legacyWebPlatform.shim";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private bool IsLegacyMobileHub(IVssRequestContext requestContext)
    {
      ContributedNavigation selectedElementByType = requestContext.GetService<IContributionNavigationService>().GetSelectedElementByType(requestContext, "ms.vss-web.page");
      if (selectedElementByType != null)
      {
        Contribution contribution = requestContext.GetService<IContributionService>().QueryContribution(requestContext, selectedElementByType.Id);
        if ((contribution.GetProperty<int>("::Attributes") & 1) == 1)
        {
          string property = contribution.GetProperty<string>("viewName");
          if (property != null && property.IndexOf("Mobile", StringComparison.OrdinalIgnoreCase) >= 0)
            return true;
        }
      }
      return false;
    }

    public LegacyPlatformShim GetLegacyPlatformShim(IVssRequestContext requestContext)
    {
      object obj;
      if (requestContext.Items.TryGetValue("legacyWebPlatform.shim", out obj) && obj is LegacyPlatformShim legacyPlatformShim1)
        return legacyPlatformShim1;
      PageContext pageContext = WebContextFactory.GetPageContext(requestContext);
      TfsWebContext webContext = WebContextFactory.GetWebContext<TfsWebContext>(pageContext.WebContext.RequestContext);
      FontRegistration.RegisterFonts(pageContext.WebContext.RequestContext);
      LegacyPlatformShim legacyPlatformShim2 = new LegacyPlatformShim()
      {
        Content = new List<LegacyContent>(),
        FeatureLicenseInfo = LicenseExtensions.GetFeaturePayload(webContext),
        BuiltInPlugins = (IEnumerable<WebAccessPluginModule>) BuiltinPluginManager.GetPlugins().ToArray(),
        BuiltInPluginBases = (IEnumerable<WebAccessPluginBase>) BuiltinPluginManager.GetBases().ToArray(),
        ContributionsPageData = ContributionsHtmlExtensions.GetContributionsPageData(pageContext),
        PageContext = pageContext,
        ScriptsToRequire = new List<string>()
      };
      GeneralHtmlExtensions.UseCommonScriptModules(pageContext.WebContext.RequestContext.HttpContext, "VSS/Error", "VSS/Service", "VSS/Controls/ExternalHub", "VSS/Contributions/PageEvents", "VSS/Contributions/Services", "VSSPreview/Flux/Components/ExternalHubShim");
      GeneralHtmlExtensions.UseCommonCSS(pageContext.WebContext.RequestContext.HttpContext, "jQueryUI-Modified", "Core");
      if (pageContext.WebContext.ProjectContext != null)
        GeneralHtmlExtensions.UseScriptModules(pageContext.WebContext.RequestContext.HttpContext, "TfsCommon/Scripts/ProjectTelemetry");
      if (this.IsLegacyMobileHub(requestContext))
      {
        GeneralHtmlExtensions.UseCSS(pageContext.WebContext.RequestContext.HttpContext, "Mobile/Mobile");
        GeneralHtmlExtensions.UseScriptModules(pageContext.WebContext.RequestContext.HttpContext, "TfsCommon/Scripts/MobileNavigation/Bundle");
      }
      GeneralHtmlExtensions.AddScriptExcludePaths(pageContext.WebContext.RequestContext.HttpContext, BundlingHelper.ExcludedNewPlatformPaths);
      bool useIntegrity = requestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.SubresourceIntegrity");
      this.AddCssFiles(pageContext, legacyPlatformShim2.Content, useIntegrity);
      this.AddScriptFiles(pageContext, legacyPlatformShim2.Content, legacyPlatformShim2.ScriptsToRequire, useIntegrity);
      if (!string.Equals("ContributedPage", requestContext.GetService<IContributionRoutingService>().GetRouteValue<string>(requestContext, "controller"), StringComparison.OrdinalIgnoreCase))
        legacyPlatformShim2.PreloadContent = true;
      requestContext.Items["legacyWebPlatform.shim"] = (object) legacyPlatformShim2;
      return legacyPlatformShim2;
    }

    private void AddScriptFiles(
      PageContext pageContext,
      List<LegacyContent> content,
      List<string> scriptsToRequire,
      bool useIntegrity)
    {
      scriptsToRequire.AddRange((IEnumerable<string>) GeneralHtmlExtensions.GetScriptsToRequire(pageContext.WebContext.RequestContext.HttpContext));
      if (!pageContext.WebContext.Diagnostics.BundlingEnabled)
      {
        foreach (JavascriptFileReference script in pageContext.CoreReferences.Scripts)
        {
          List<LegacyContent> legacyContentList = content;
          LegacyContent legacyContent = new LegacyContent();
          legacyContent.ContentType = "text/javascript";
          legacyContent.Url = script.Url;
          legacyContentList.Add(legacyContent);
        }
      }
      else
      {
        List<BundledScriptFile> bundledScriptFileList = new List<BundledScriptFile>();
        if (pageContext.CoreReferences.ThirdPartyScriptsBundle != null)
          bundledScriptFileList.Add(pageContext.CoreReferences.ThirdPartyScriptsBundle);
        bundledScriptFileList.AddRange((IEnumerable<BundledScriptFile>) GeneralHtmlExtensions.GetBundledScriptFiles(pageContext, scriptsToRequire.ToArray()));
        foreach (BundledScriptFile bundle in bundledScriptFileList)
        {
          string bundleScriptUrl = BundlingHelper.GetBundleScriptUrl(pageContext.WebContext.TfsRequestContext, bundle, pageContext.WebContext.Diagnostics.CdnEnabled);
          List<LegacyContent> legacyContentList = content;
          LegacyContent legacyContent = new LegacyContent();
          legacyContent.ClientId = bundle.Name;
          legacyContent.ContentType = "text/javascript";
          legacyContent.Integrity = useIntegrity ? bundle.Integrity : (string) null;
          legacyContent.ContentLength = (long) bundle.ContentLength;
          legacyContent.Url = bundleScriptUrl;
          legacyContent.Bundle = new BundleInfo()
          {
            Name = bundle.Definition.FriendlyName,
            Length = bundle.ContentLength,
            Includes = bundle.Definition.ContentList.OfType<BundleScriptModulesContent>().SelectMany<BundleScriptModulesContent, string>((Func<BundleScriptModulesContent, IEnumerable<string>>) (a => (IEnumerable<string>) a.IncludedScripts)).ToList<string>()
          };
          legacyContentList.Add(legacyContent);
        }
      }
    }

    private void AddCssFiles(
      PageContext pageContext,
      List<LegacyContent> content,
      bool useIntegrity)
    {
      if (!pageContext.WebContext.Diagnostics.BundlingEnabled)
      {
        foreach (string includedCssFile in (IEnumerable<string>) GeneralHtmlExtensions.GetAllCssBundle(pageContext.WebContext.RequestContext.HttpContext).IncludedCSSFiles)
        {
          string themedCssFileUrl = PlatformHtmlExtensions.GetThemedCssFileUrl(pageContext.WebContext.RequestContext, includedCssFile + ".css");
          List<LegacyContent> legacyContentList = content;
          LegacyContent legacyContent = new LegacyContent();
          legacyContent.ContentType = "text/css";
          legacyContent.Url = themedCssFileUrl;
          legacyContentList.Add(legacyContent);
        }
      }
      else
      {
        List<BundleInfo> bundleInfoList = new List<BundleInfo>();
        IEnumerable<BundledCSSFile> bundledCssFiles = GeneralHtmlExtensions.GetBundledCSSFiles(pageContext).Where<BundledCSSFile>((Func<BundledCSSFile, bool>) (b => !b.IsEmpty));
        IVssRequestContext vssRequestContext = pageContext.WebContext.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        foreach (BundledCSSFile bundle in bundledCssFiles)
        {
          string str = (string) null;
          if (pageContext.WebContext.Diagnostics.CdnEnabled && !string.IsNullOrEmpty(bundle.CDNRelativeUri))
          {
            string cdnUrl = vssRequestContext.GetService<ICdnLocationService>().GetCdnUrl(vssRequestContext, bundle.CDNRelativeUri);
            if (!string.IsNullOrEmpty(cdnUrl))
              str = cdnUrl;
          }
          if (string.IsNullOrEmpty(str))
            str = BundlingHelper.GetLocalBundleCssUrl(vssRequestContext, bundle);
          List<LegacyContent> legacyContentList = content;
          LegacyContent legacyContent = new LegacyContent();
          legacyContent.ClientId = bundle.Name;
          legacyContent.ContentType = "text/css";
          legacyContent.Url = str;
          legacyContent.Bundle = new BundleInfo()
          {
            Name = bundle.Definition.FriendlyName,
            Length = bundle.ContentLength,
            Includes = bundle.IncludedCssFiles.ToList<string>()
          };
          legacyContent.Integrity = useIntegrity ? bundle.Integrity : (string) null;
          legacyContent.ContentLength = (long) bundle.ContentLength;
          legacyContentList.Add(legacyContent);
        }
      }
    }
  }
}
