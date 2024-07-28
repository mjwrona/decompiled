// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.WebDiagnosticsService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class WebDiagnosticsService : IWebDiagnosticsService, IVssFrameworkService
  {
    private static readonly RegistryQuery s_debugModeRegistryQuery = new RegistryQuery("/Configuration/WebAccess/DebugMode");
    private static readonly RegistryQuery s_bundlingRegistryQuery = new RegistryQuery("/Configuration/WebAccess/BundlingMode");
    private static readonly RegistryQuery s_tracePointsRegistryQuery = new RegistryQuery("/Configuration/WebAccess/TracePointCollection");
    private readonly IEnumerable<string> c_unsupportedUAsForESNext = (IEnumerable<string>) new List<string>()
    {
      "Trident/7.0",
      "Edge/12",
      "Edge/13",
      "Edge/14",
      "PhantomJS"
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool IsBundlingEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.DoNotLoadDebugAssets") || !this.SupportsESNextScripts(requestContext) || this.GetDiagnosticValue(requestContext, "BUNDLING", defaultValue: true, defaultValueQuery: WebDiagnosticsService.s_bundlingRegistryQuery);

    public bool IsDebugContentEnabled(IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.DoNotLoadDebugAssets"))
        return false;
      return !this.IsBundlingEnabled(requestContext) || this.GetDiagnosticValue(requestContext, "DEBUG", defaultValue: this.RunningDebugBits, defaultValueQuery: WebDiagnosticsService.s_debugModeRegistryQuery);
    }

    public bool IsCdnAvailable(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.UseCDN");

    public bool IsCdnEnabled(IVssRequestContext requestContext) => this.IsBundlingEnabled(requestContext) && this.IsCdnAvailable(requestContext) && this.GetDiagnosticValue(requestContext, "CDN", true, true);

    public bool IsTracePointCollectionEnabled(IVssRequestContext requestContext) => this.GetDiagnosticValue(requestContext, "TRACEPOINT-COLLECTOR", true, this.RunningDebugBits, WebDiagnosticsService.s_tracePointsRegistryQuery);

    public bool IsRequestDiagnosticsEnabled(IVssRequestContext requestContext) => this.GetDiagnosticValue(requestContext, "DIAG");

    public bool SupportsESNextScripts(IVssRequestContext requestContext)
    {
      string userAgent = requestContext.UserAgent;
      if (string.IsNullOrEmpty(userAgent))
        return true;
      foreach (string str in this.c_unsupportedUAsForESNext)
      {
        if (userAgent.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1)
          return false;
      }
      return true;
    }

    private bool GetDiagnosticValue(
      IVssRequestContext requestContext,
      string cookieName,
      bool negativeCookie = false,
      bool defaultValue = false,
      RegistryQuery defaultValueQuery = default (RegistryQuery))
    {
      string a;
      return !requestContext.GetSessionValue(cookieName, out a) ? (defaultValueQuery.Path == null ? defaultValue : requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in defaultValueQuery, true, defaultValue)) : (!negativeCookie ? string.Equals(a, "enabled", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "true", StringComparison.OrdinalIgnoreCase) : !string.IsNullOrWhiteSpace(a) && !"disabled".Equals(a, StringComparison.OrdinalIgnoreCase));
    }

    public bool RunningDebugBits => HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled;
  }
}
