// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundleCSSContent
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public class BundleCSSContent : 
    IBundledCSSContent,
    IBundledContent<CSSBundleDefinition, BundledCSSFile>
  {
    private static ConcurrentDictionary<string, string> s_localThemedPaths = new ConcurrentDictionary<string, string>();

    public ISet<string> IncludedCSSFiles { get; internal set; }

    public ISet<string> ExcludedCSSFiles { get; internal set; }

    public ISet<string> IncludedScripts { get; set; }

    public ISet<string> ExcludedScripts { get; set; }

    public IEnumerable<string> ExcludedScriptPaths { get; set; }

    public string GetDefinitionHash()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.IncludedCSSFiles != null)
        stringBuilder.Append(string.Join(",", (IEnumerable<string>) this.IncludedCSSFiles.OrderBy<string, string>((Func<string, string>) (m => m))));
      stringBuilder.Append(':');
      if (this.IncludedScripts != null)
        stringBuilder.Append(string.Join(",", (IEnumerable<string>) this.IncludedScripts.OrderBy<string, string>((Func<string, string>) (m => m))));
      stringBuilder.Append(':');
      if (this.ExcludedScripts != null && this.ExcludedScripts.Any<string>())
        stringBuilder.Append(string.Join(",", (IEnumerable<string>) this.ExcludedScripts.OrderBy<string, string>((Func<string, string>) (m => m))));
      stringBuilder.Append(':');
      if (this.ExcludedScriptPaths != null && this.ExcludedScriptPaths.Any<string>())
        stringBuilder.Append(string.Join(",", (IEnumerable<string>) this.ExcludedScriptPaths.OrderBy<string, string>((Func<string, string>) (m => m))));
      stringBuilder.Append(':');
      return stringBuilder.ToString();
    }

    public string GetContent(
      IVssRequestContext requestContext,
      CSSBundleDefinition bundleDefinition,
      BundledCSSFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache)
    {
      VssCSSBundleBuilder builder = this.GetBuilder(requestContext, bundleDefinition, bundledFile, urlHelper, contentCache);
      string bundleContent = builder.GetBundleContent(contentCache);
      this.SetIncludedCssFiles(builder, bundledFile);
      return bundleContent;
    }

    public IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(
      IVssRequestContext requestContext,
      CSSBundleDefinition bundleDefinition,
      BundledCSSFile bundledFile,
      UrlHelper urlHelper)
    {
      VssCSSBundleBuilder builder = this.GetBuilder(requestContext, bundleDefinition, bundledFile, urlHelper, (Dictionary<string, string[]>) null);
      IEnumerable<IBundleStreamProvider> bundleStreamProviders = builder.GetBundleStreamProviders((Func<string, string>) (cssFile => FontRegistration.GetRegisteredFontFaces(requestContext, cssFile)));
      this.SetIncludedCssFiles(builder, bundledFile);
      return bundleStreamProviders;
    }

    public List<byte[]> GetFileHash(
      IVssRequestContext requestContext,
      CSSBundleDefinition bundleDefinition,
      BundledCSSFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache,
      out int contentLength)
    {
      VssCSSBundleBuilder builder = this.GetBuilder(requestContext, bundleDefinition, bundledFile, urlHelper, contentCache);
      List<byte[]> fileHash = builder.BuildBundleHashCodeList(contentCache, out contentLength);
      this.SetIncludedCssFiles(builder, bundledFile);
      return fileHash;
    }

    private VssCSSBundleBuilder GetBuilder(
      IVssRequestContext requestContext,
      CSSBundleDefinition bundleDefinition,
      BundledCSSFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache)
    {
      bool debugStyles = false;
      string themeName = bundleDefinition.ThemeName;
      debugStyles = bundleDefinition.DebugStyles;
      IEnumerable<string> cssModulePrefixes = bundleDefinition.CSSModulePrefixes;
      VssCSSBundleBuilder builder = new VssCSSBundleBuilder((IEnumerable<string>) this.IncludedCSSFiles, (IEnumerable<string>) this.ExcludedCSSFiles, (Func<string, string>) (m =>
      {
        string str = debugStyles ? ".css" : ".min.css";
        return this.GetLocalThemedPath(urlHelper, m, themeName, bundleDefinition.StaticContentVersion) + str;
      }));
      builder.AddCSSFilesFromScripts(new VssScriptsBundle(bundledFile.Name, (IEnumerable<string>) this.IncludedScripts, (IEnumerable<string>) this.ExcludedScripts, (IEnumerable<string>) null, CultureInfo.CurrentUICulture, (Func<string, string>) (m => BundlingHelper.ConvertModuleToPath(m, urlHelper, false, bundleDefinition.StaticContentVersion)), staticContentVersion: bundleDefinition.StaticContentVersion), requestContext.ExecutionEnvironment.IsOnPremisesDeployment, cssModulePrefixes, this.ExcludedScriptPaths, contentCache);
      return builder;
    }

    private string GetLocalThemedPath(
      UrlHelper urlHelper,
      string bundleName,
      string themeName,
      string staticContentVersion)
    {
      string key = staticContentVersion ?? string.Empty;
      string localThemedPath;
      if (!BundleCSSContent.s_localThemedPaths.TryGetValue(key, out localThemedPath))
      {
        localThemedPath = urlHelper.Content(PlatformHtmlExtensions.LocalVersionedStaticContentRoot(staticContentVersion));
        BundleCSSContent.s_localThemedPaths.TryAdd(key, localThemedPath);
      }
      localThemedPath += PlatformHtmlExtensions.GetThemedPartialPath(bundleName, themeName);
      return localThemedPath;
    }

    private void SetIncludedCssFiles(VssCSSBundleBuilder builder, BundledCSSFile bundledFile)
    {
      if (bundledFile.IncludedCssFiles == null)
        bundledFile.IncludedCssFiles = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      bundledFile.IncludedCssFiles.UnionWith(builder.GetIncludedCssFiles());
    }
  }
}
