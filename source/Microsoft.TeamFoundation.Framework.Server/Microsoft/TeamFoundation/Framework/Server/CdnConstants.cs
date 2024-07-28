// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CdnConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class CdnConstants
  {
    public const string CdnDisableCookieName = "TFS-CDN";
    public const string CdnDisableCookieValue = "disabled";
    public const string TraceCookieName = "TFS-CDNTRACE";
    public const string TraceCookieValue = "report";
    public const string TimeoutCookieName = "TFS-CDNTIMEOUT";
    public const string TimeoutCookieValue = "report";
    public const string FailCountCookieName = "TFS-CDNFAIL";
    public const string RegionalCdnEnableCookieName = "TFS-REGIONAL-CDN";
    public const string RegionalCdnEnableCookieValue = "enabled";
    public const string RegionalCdnCountryCodeCookieName = "TFS-REGIONALCDN-COUNTRYCODE";
    public const string Not_RegionalCdnCountryCodeCookieValue = "no";
    public const string UseRegionalCdnUrls = "VisualStudio.Services.WebAccess.UseRegionalCDNURLs";
    public const string UseCDN = "VisualStudio.Services.WebAccess.UseCDN";
  }
}
