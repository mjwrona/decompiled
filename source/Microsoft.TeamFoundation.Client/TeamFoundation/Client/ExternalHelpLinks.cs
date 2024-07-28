// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ExternalHelpLinks
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ExternalHelpLinks
  {
    private const string BaseForwardLinkUrl = "https://go.microsoft.com/fwlink/?LinkId=";
    public static int ProcessGuidanceWebSiteHelpLink = 136525;
    public static int SiteOwnershipHelpLink = 136526;
    public static int ConfigurePortal = 136526;
    public static int ConfigureProcessGuidance = 136525;
    public static int ConfigureDefaultSiteCreationLocation = 136529;
    public static int ConfigureReporting = 136530;
    public static int ConfigureWebAccess = 136531;
    public static int CreateTpcReportFolderHelpLink = 136533;
    public static int CreateTpcSharePointSiteHelpLink = 136532;
    public static int AddOrRemoveSharepointWebApplication = 136529;
    public static int CreateTeamProject = 2047798;
    public static int TpcAdminPermissions = 166742;
    public static int InstructionsForSettingSiteOwner = 147580;
    public static int SharePointTfsIntegration = 198174;
    public static int ReportingTfsIntegration = 254456;
    public static int AnalyticsHelpLink = 875448;
    public static int AnalyticsMarketplace = 849049;
    public static int ReportingDepricating = 2112624;
    public static int ReportingDisable = 2112648;
    public static int UsingSplitterInWitForm = 164816;
    public static int HostedLandingPage = 228158;
    public static int LearnMoreAboutVso = 613806;
    public static readonly int SqlServerReporting = 865219;
    public static readonly int LearnMoreProcessModels = 865220;
    public static readonly int AnalyticsHelp = 875190;

    public static Uri GetUri(int linkId) => new Uri("https://go.microsoft.com/fwlink/?LinkId=" + linkId.ToString());

    public static string GetHtmlLink(int linkId, string text) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<a href=\"{0}\">{1}</a>", (object) ExternalHelpLinks.GetUri(linkId).AbsoluteUri, (object) text);

    public static void Launch(int linkId) => BrowserHelper.LaunchBrowser(ExternalHelpLinks.GetUri(linkId).AbsoluteUri, (NetworkCredential) null);
  }
}
