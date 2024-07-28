// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributedSite
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  public class ContributedSite
  {
    private static HashSet<string> s_defaultNavigationTypes = new HashSet<string>((IEnumerable<string>) new string[1]
    {
      "ms.vss-web.navigation"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private HashSet<string> m_navigationTypes;
    private string m_fallbackRoute;
    private string m_defaultTheme;
    private string m_siteId;

    public ContributedSite()
    {
      this.m_siteId = string.Empty;
      this.m_defaultTheme = "ms.vss-web.vsts-theme";
      this.m_fallbackRoute = "ms.vss-web.fallback-route";
      this.m_navigationTypes = ContributedSite.s_defaultNavigationTypes;
    }

    public ContributedSite(IVssRequestContext requestContext, Contribution siteContribution)
    {
      this.m_siteId = siteContribution.Id;
      if (!siteContribution.Properties.TryGetValue<string>("fallbackRoute", out this.m_fallbackRoute))
        this.m_fallbackRoute = "ms.vss-web.fallback-route";
      if (!siteContribution.Properties.TryGetValue<string>("defaultTheme", out this.m_defaultTheme))
        this.m_defaultTheme = "ms.vss-web.vsts-theme";
      string[] collection;
      if (siteContribution.Properties.TryGetValue<string[]>("navigationElementTypes", out collection))
        this.m_navigationTypes = new HashSet<string>((IEnumerable<string>) collection, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      else
        this.m_navigationTypes = ContributedSite.s_defaultNavigationTypes;
    }

    public string DefaultTheme => this.m_defaultTheme;

    public string FallbackRoute => this.m_fallbackRoute;

    public string Id => this.m_siteId;

    public HashSet<string> NavigationElementTypes => this.m_navigationTypes;
  }
}
