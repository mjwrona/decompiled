// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ContentSecurityPolicyHeaderManagementService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Web;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ContentSecurityPolicyHeaderManagementService : 
    IContentSecurityPolicyHeaderManagementService,
    IVssFrameworkService
  {
    private ContentSecurityPolicyHeaderManagementService.ContentSecurityPolicySettings m_cspSettings;
    private static readonly RegistryQuery s_registrySettingsReadOnlyPolicyQuery = (RegistryQuery) "/Configuration/WebSecurity/ContentSecurityPolicy/ReadOnlyPolicyValues/*";
    private const string c_reportUriDirective = "report-uri";
    private const string c_requestContextExtraCspDirectives = "$vssExtraCspDirectives";
    private static readonly List<string> s_directives = new List<string>()
    {
      "default-src",
      "base-uri",
      "font-src",
      "style-src",
      "connect-src",
      "img-src",
      "script-src",
      "child-src",
      "frame-src",
      "worker-src",
      "media-src",
      "frame-ancestors"
    };
    private const string c_area = "ContentSecurityPolicy";
    private const string c_layer = "HeaderService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1514200, "ContentSecurityPolicy", "HeaderService", nameof (ServiceStart));
      systemRequestContext.CheckDeploymentRequestContext();
      try
      {
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in ContentSecurityPolicyHeaderManagementService.s_registrySettingsReadOnlyPolicyQuery);
        Interlocked.CompareExchange<ContentSecurityPolicyHeaderManagementService.ContentSecurityPolicySettings>(ref this.m_cspSettings, new ContentSecurityPolicyHeaderManagementService.ContentSecurityPolicySettings(systemRequestContext), (ContentSecurityPolicyHeaderManagementService.ContentSecurityPolicySettings) null);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1514201, "ContentSecurityPolicy", "HeaderService", ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1514202, "ContentSecurityPolicy", "HeaderService", nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1514210, "ContentSecurityPolicy", "HeaderService", nameof (ServiceEnd));
      try
      {
        systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1514211, "ContentSecurityPolicy", "HeaderService", ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1514212, "ContentSecurityPolicy", "HeaderService", nameof (ServiceEnd));
      }
    }

    public string GetReportOnlyHeaderValue(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      bool includeReportUri)
    {
      requestContext.TraceEnter(1514230, "ContentSecurityPolicy", "HeaderService", nameof (GetReportOnlyHeaderValue));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        if (this.m_cspSettings == null)
          return string.Empty;
        ContentSecurityPolicyHeaderManagementService.ContentSecurityPolicySettings cspSettings = this.m_cspSettings;
        IContentSecurityPolicyNonceManagementService service = requestContext.GetService<IContentSecurityPolicyNonceManagementService>();
        object obj = httpContext.Items[(object) "$vssExtraCspDirectives"];
        Dictionary<string, HashSet<string>> dictionary = obj == null || !(obj is Dictionary<string, HashSet<string>>) ? new Dictionary<string, HashSet<string>>() : obj as Dictionary<string, HashSet<string>>;
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string directive in ContentSecurityPolicyHeaderManagementService.s_directives)
        {
          string format = cspSettings.GetDirectiveValue(directive);
          HashSet<string> stringSet = (HashSet<string>) null;
          if (dictionary.TryGetValue(directive, out stringSet) && stringSet != null && stringSet.Count > 0)
          {
            foreach (string str in stringSet)
            {
              if (!format.Contains(str))
                format = format + " " + str;
            }
          }
          if (directive.Equals("script-src", StringComparison.OrdinalIgnoreCase))
            format = string.Format(format, (object) service.GetNonceValue(requestContext, httpContext));
          if (!string.IsNullOrEmpty(format))
            stringBuilder.Append(string.Format("{0} {1}; ", (object) directive, (object) format));
        }
        if (includeReportUri)
          stringBuilder.Append("report-uri " + cspSettings.ReportUri.AbsoluteUri);
        return stringBuilder.ToString().Trim();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1514245, "ContentSecurityPolicy", "HeaderService", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1514250, "ContentSecurityPolicy", "HeaderService", nameof (GetReportOnlyHeaderValue));
      }
    }

    public string GetHeaderValue(IVssRequestContext requestContext, HttpContextBase context) => this.GetReportOnlyHeaderValue(requestContext, context, false);

    public void AddAdditionalPolicyForDirective(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      string directive,
      string policyValue)
    {
      object obj = httpContext.Items[(object) "$vssExtraCspDirectives"];
      Dictionary<string, HashSet<string>> dictionary;
      if (obj == null || !(obj is Dictionary<string, HashSet<string>>))
      {
        dictionary = new Dictionary<string, HashSet<string>>();
        httpContext.Items[(object) "$vssExtraCspDirectives"] = (object) dictionary;
      }
      else
        dictionary = obj as Dictionary<string, HashSet<string>>;
      HashSet<string> stringSet = (HashSet<string>) null;
      if (!dictionary.TryGetValue(directive, out stringSet))
      {
        stringSet = new HashSet<string>();
        dictionary[directive] = stringSet;
      }
      stringSet.Add(policyValue);
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(1514220, "ContentSecurityPolicy", "HeaderService", nameof (OnRegistrySettingsChanged));
      try
      {
        Volatile.Write<ContentSecurityPolicyHeaderManagementService.ContentSecurityPolicySettings>(ref this.m_cspSettings, new ContentSecurityPolicyHeaderManagementService.ContentSecurityPolicySettings(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1514221, "ContentSecurityPolicy", "HeaderService", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1514222, "ContentSecurityPolicy", "HeaderService", nameof (OnRegistrySettingsChanged));
      }
    }

    private class ContentSecurityPolicySettings
    {
      public readonly string ChildSrc;
      public readonly string ConnectSrc;
      public readonly string DefaultSrc;
      public readonly string FontSrc;
      public readonly string ImgSrc;
      public readonly string ScriptSrc;
      public readonly string StyleSrc;
      public readonly string MediaSrc;
      public readonly string FrameAncestorsSrc;
      public readonly Uri ReportUri;
      private Dictionary<string, string> m_directiveValues;

      public ContentSecurityPolicySettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, ContentSecurityPolicyHeaderManagementService.s_registrySettingsReadOnlyPolicyQuery);
        this.ChildSrc = registryEntryCollection.GetValueFromPath<string>(nameof (ChildSrc), "");
        this.ConnectSrc = registryEntryCollection.GetValueFromPath<string>(nameof (ConnectSrc), "");
        this.DefaultSrc = registryEntryCollection.GetValueFromPath<string>(nameof (DefaultSrc), "");
        this.FontSrc = registryEntryCollection.GetValueFromPath<string>(nameof (FontSrc), "");
        this.ImgSrc = registryEntryCollection.GetValueFromPath<string>(nameof (ImgSrc), "");
        this.ScriptSrc = registryEntryCollection.GetValueFromPath<string>(nameof (ScriptSrc), "");
        this.StyleSrc = registryEntryCollection.GetValueFromPath<string>(nameof (StyleSrc), "");
        this.MediaSrc = registryEntryCollection.GetValueFromPath<string>(nameof (MediaSrc), this.FontSrc);
        this.FrameAncestorsSrc = registryEntryCollection.GetValueFromPath<string>(nameof (FrameAncestorsSrc), "");
        string uriString = registryEntryCollection.GetValueFromPath<string>(nameof (ReportUri), "");
        if (string.IsNullOrEmpty(uriString))
          uriString = requestContext.GetService<ILocationService>().GetPublicAccessMapping(requestContext).AccessPoint + "_apis/CspReport";
        if (!Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out this.ReportUri))
          requestContext.Trace(1514225, TraceLevel.Error, "ContentSecurityPolicy", "HeaderService", "CSP report-uri was invalid: " + uriString);
        this.m_directiveValues = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.m_directiveValues["child-src"] = this.ChildSrc;
        this.m_directiveValues["frame-src"] = this.ChildSrc;
        this.m_directiveValues["worker-src"] = this.ChildSrc;
        this.m_directiveValues["connect-src"] = this.ConnectSrc;
        this.m_directiveValues["default-src"] = this.DefaultSrc;
        this.m_directiveValues["base-uri"] = this.DefaultSrc;
        this.m_directiveValues["font-src"] = this.FontSrc;
        this.m_directiveValues["media-src"] = this.MediaSrc;
        this.m_directiveValues["img-src"] = this.ImgSrc;
        this.m_directiveValues["script-src"] = this.ScriptSrc;
        this.m_directiveValues["style-src"] = this.StyleSrc;
        this.m_directiveValues["frame-ancestors"] = this.FrameAncestorsSrc;
      }

      public string GetDirectiveValue(string directiveName) => this.m_directiveValues[directiveName];
    }
  }
}
