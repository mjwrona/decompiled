// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CdnLocationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CdnLocationService : ICdnLocationService, IVssFrameworkService
  {
    private const string c_cachedCdnBaseUrl = "CachedCDNBaseUrl";
    private Dictionary<string, string> m_regionalCDNEndpointUrls;
    private string m_cdnEndpointUrl;
    private static readonly string s_area = nameof (CdnLocationService);
    private static readonly string s_layer = "WebPlatform";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/WebAccess/CDN/...");
      this.ReadRegistryValues(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GetCdnUrl(IVssRequestContext requestContext, string relativePath)
    {
      requestContext.TraceEnter(15060020, CdnLocationService.s_area, CdnLocationService.s_layer, nameof (GetCdnUrl));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        if (string.IsNullOrEmpty(this.m_cdnEndpointUrl))
          return (string) null;
        string cdnBaseUrl = (string) null;
        object obj;
        if (requestContext.RootContext.Items.TryGetValue("CachedCDNBaseUrl", out obj) && obj is string && !string.IsNullOrEmpty(obj as string))
        {
          cdnBaseUrl = obj as string;
        }
        else
        {
          if (this.m_regionalCDNEndpointUrls.Keys.Count > 0 && requestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.UseRegionalCDNURLs") || "enabled".Equals(HttpContext.Current?.Request.Cookies["TFS-REGIONAL-CDN"]?.Value, StringComparison.OrdinalIgnoreCase))
          {
            string countryCode = requestContext.GetService<IGeoLocationService>().GetRequestCountryCode(requestContext);
            if (!string.IsNullOrEmpty(countryCode))
            {
              this.m_regionalCDNEndpointUrls.TryGetValue(countryCode, out cdnBaseUrl);
              if (!string.IsNullOrEmpty(cdnBaseUrl))
                requestContext.TraceConditionally(15060027, TraceLevel.Info, CdnLocationService.s_area, CdnLocationService.s_layer, (Func<string>) (() => "CDN URL for country '" + countryCode + "' is '" + cdnBaseUrl + "'"));
              else
                requestContext.TraceConditionally(15060028, TraceLevel.Info, CdnLocationService.s_area, CdnLocationService.s_layer, (Func<string>) (() => "CDN URL for country '" + countryCode + "' without regional CDN url configured."));
            }
          }
          if (string.IsNullOrEmpty(cdnBaseUrl))
            cdnBaseUrl = this.m_cdnEndpointUrl;
          requestContext.RootContext.Items["CachedCDNBaseUrl"] = (object) cdnBaseUrl;
        }
        return string.IsNullOrEmpty(relativePath) ? cdnBaseUrl : cdnBaseUrl + "/" + relativePath;
      }
      finally
      {
        requestContext.TraceLeave(15060021, CdnLocationService.s_area, CdnLocationService.s_layer, nameof (GetCdnUrl));
      }
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.ReadRegistryValues(requestContext);
    }

    private void ReadRegistryValues(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str1 = service.GetValue(requestContext, (RegistryQuery) "/Configuration/WebAccess/CDN/RegionalEndpointUrls", (string) null);
      if (!string.IsNullOrEmpty(str1))
      {
        string str2 = str1;
        char[] chArray = new char[1]{ ';' };
        foreach (string str3 in str2.Split(chArray))
        {
          char[] separator = new char[1]{ ',' };
          string[] strArray = str3.Split(separator, StringSplitOptions.RemoveEmptyEntries);
          if (strArray.Length > 1)
            dictionary.Add(strArray[0], strArray[1]);
        }
      }
      this.m_regionalCDNEndpointUrls = dictionary;
      this.m_cdnEndpointUrl = service.GetValue(requestContext, (RegistryQuery) "/Configuration/WebAccess/CDN/EndpointUrl", (string) null);
    }
  }
}
