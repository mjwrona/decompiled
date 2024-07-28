// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CookieHandlerSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CookieHandlerSettings
  {
    public CookieHandlerSettings()
    {
    }

    public CookieHandlerSettings(RegistryEntryCollection settings)
    {
      this.DefaultDomain = settings["Domain"].GetValue<string>((string) null);
      if (settings[nameof (HideFromClientScript)].GetValue((string) null) != null)
        this.HideFromClientScript = new bool?(settings[nameof (HideFromClientScript)].GetValue<bool>(false));
      this.RequireSsl = settings[nameof (RequireSsl)].GetValue<bool>(false);
      this.SlidingExpiration = TimeSpan.FromSeconds((double) settings["SlidingExpirationSeconds"].GetValue<int>(604800));
      this.TokenReissueDelay = TimeSpan.FromSeconds((double) settings[nameof (TokenReissueDelay)].GetValue<int>(36000));
      this.Domains = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistryEntry setting in settings)
      {
        if (setting.Path.StartsWith(FederatedAuthRegistryConstants.AlternateDomains, StringComparison.OrdinalIgnoreCase))
          this.Domains.Add(setting.Name, setting.Value);
        if (setting.Path.StartsWith(FederatedAuthRegistryConstants.CookieHandler + "/Domain/", StringComparison.OrdinalIgnoreCase))
        {
          if (this.CookieDomains == null)
            this.CookieDomains = new List<CookieHandlerSettings.CookieDomain>();
          this.CookieDomains.Add(new CookieHandlerSettings.CookieDomain(setting.Name, setting.Value));
        }
      }
      this.Domains[AccessMappingConstants.PublicAccessMappingMoniker] = this.DefaultDomain;
    }

    public string GetCookieDomain(string domain)
    {
      if (domain != null && this.CookieDomains != null)
      {
        foreach (CookieHandlerSettings.CookieDomain cookieDomain1 in this.CookieDomains)
        {
          string cookieDomain2;
          if (cookieDomain1.TryMatch(domain, out cookieDomain2))
            return cookieDomain2;
        }
      }
      return (string) null;
    }

    public string DefaultDomain { get; set; }

    public Dictionary<string, string> Domains { get; set; }

    public bool? HideFromClientScript { get; set; }

    public bool RequireSsl { get; set; }

    public TimeSpan SlidingExpiration { get; set; }

    public TimeSpan TokenReissueDelay { get; set; }

    private List<CookieHandlerSettings.CookieDomain> CookieDomains { get; set; }

    private class CookieDomain
    {
      private string m_domain;
      private bool m_recursive;
      private string m_cookieDomain;

      public CookieDomain(string domainPattern, string cookieDomain)
      {
        if (domainPattern.StartsWith(".", StringComparison.Ordinal))
        {
          this.m_recursive = true;
          domainPattern = domainPattern.Substring(1);
        }
        this.m_domain = domainPattern;
        this.m_cookieDomain = cookieDomain;
      }

      public bool TryMatch(string domain, out string cookieDomain)
      {
        if (this.m_recursive && UriUtility.IsSubdomainOf(domain, this.m_domain) || !this.m_recursive && StringComparer.OrdinalIgnoreCase.Equals(domain, this.m_domain))
        {
          cookieDomain = this.m_cookieDomain;
          return true;
        }
        cookieDomain = (string) null;
        return false;
      }
    }
  }
}
