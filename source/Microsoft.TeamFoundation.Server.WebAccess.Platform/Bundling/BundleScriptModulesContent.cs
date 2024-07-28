// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundleScriptModulesContent
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public class BundleScriptModulesContent : 
    IBundledScriptContent,
    IBundledContent<ScriptBundleDefinition, BundledScriptFile>
  {
    public ISet<string> IncludedScripts { get; set; }

    public ISet<string> ExcludedScripts { get; set; }

    public IEnumerable<string> ExcludedPaths { get; set; }

    public string GetDefinitionHash()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.IncludedScripts != null && this.IncludedScripts.Any<string>())
        stringBuilder.Append(string.Join(",", (IEnumerable<string>) this.IncludedScripts.Select<string, string>((Func<string, string>) (m => m + "/v2")).OrderBy<string, string>((Func<string, string>) (m => m))));
      stringBuilder.Append(':');
      if (this.ExcludedScripts != null && this.ExcludedScripts.Any<string>())
        stringBuilder.Append(string.Join(",", (IEnumerable<string>) this.ExcludedScripts.OrderBy<string, string>((Func<string, string>) (m => m))));
      stringBuilder.Append(':');
      if (this.ExcludedPaths != null && this.ExcludedPaths.Any<string>())
        stringBuilder.Append(string.Join(",", (IEnumerable<string>) this.ExcludedPaths.OrderBy<string, string>((Func<string, string>) (m => m))));
      stringBuilder.Append(':');
      return stringBuilder.ToString();
    }

    public string GetContent(
      IVssRequestContext requestContext,
      ScriptBundleDefinition bundleDefinition,
      BundledScriptFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache)
    {
      VssScriptsBundle vssScriptsBundle = this.GetVssScriptsBundle(bundleDefinition, bundledFile, urlHelper);
      if (vssScriptsBundle == null)
        return string.Empty;
      HashSet<string> stringSet = new HashSet<string>();
      string bundleContent = vssScriptsBundle.GetBundleContent(contentCache, stringSet, bundleDefinition.StaticContentVersion);
      this.SetExcludedScripts(vssScriptsBundle, bundledFile);
      if (stringSet.Any<string>())
        requestContext.TraceAlways(15060023, TraceLevel.Warning, BundlingService.s_area, BundlingService.s_layer, "Missing module ids for bundle {0}: {1}", (object) bundledFile.Name, (object) string.Join(", ", (IEnumerable<string>) stringSet));
      return bundleContent;
    }

    public List<byte[]> GetFileHash(
      IVssRequestContext requestContext,
      ScriptBundleDefinition bundleDefinition,
      BundledScriptFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache,
      out int contentLength)
    {
      VssScriptsBundle vssScriptsBundle = this.GetVssScriptsBundle(bundleDefinition, bundledFile, urlHelper);
      if (vssScriptsBundle != null)
      {
        List<byte[]> bundleHashCode = vssScriptsBundle.GetBundleHashCode(contentCache, out contentLength);
        this.SetExcludedScripts(vssScriptsBundle, bundledFile);
        return bundleHashCode;
      }
      contentLength = 0;
      return new List<byte[]>();
    }

    private VssScriptsBundle GetVssScriptsBundle(
      ScriptBundleDefinition bundleDefinition,
      BundledScriptFile bundledFile,
      UrlHelper urlHelper)
    {
      bool debugScripts = false;
      if (bundleDefinition != null)
      {
        debugScripts = bundleDefinition.DebugScripts;
        if (this.IncludedScripts != null && this.IncludedScripts.Any<string>())
          return new VssScriptsBundle(bundledFile.Name, (IEnumerable<string>) this.IncludedScripts, (IEnumerable<string>) this.ExcludedScripts, this.ExcludedPaths, bundleDefinition.Culture, (Func<string, string>) (m => BundlingHelper.ConvertModuleToPath(m, urlHelper, debugScripts, bundleDefinition.StaticContentVersion)), bundleDefinition.Diagnose, bundleDefinition.StaticContentVersion);
      }
      return (VssScriptsBundle) null;
    }

    private void SetExcludedScripts(VssScriptsBundle bundle, BundledScriptFile bundledFile)
    {
      if (bundledFile.ScriptsExcludedByPath == null)
        bundledFile.ScriptsExcludedByPath = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      bundledFile.ScriptsExcludedByPath.UnionWith((IEnumerable<string>) bundle.BundleBuilder.GetModulesExcludedByPath());
    }

    public IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(
      IVssRequestContext requestContext,
      ScriptBundleDefinition bundleDefinition,
      BundledScriptFile bundledFile,
      UrlHelper urlHelper)
    {
      VssScriptsBundle vssScriptsBundle = this.GetVssScriptsBundle(bundleDefinition, bundledFile, urlHelper);
      if (vssScriptsBundle == null)
        return (IEnumerable<IBundleStreamProvider>) Array.Empty<IBundleStreamProvider>();
      IEnumerable<IBundleStreamProvider> bundleStreamProviders = vssScriptsBundle.GetBundleStreamProviders(bundleDefinition.StaticContentVersion);
      this.SetExcludedScripts(vssScriptsBundle, bundledFile);
      return bundleStreamProviders;
    }
  }
}
