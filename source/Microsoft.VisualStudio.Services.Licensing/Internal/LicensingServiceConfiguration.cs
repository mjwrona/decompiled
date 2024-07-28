// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.LicensingServiceConfiguration
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class LicensingServiceConfiguration
  {
    public const string DefaultEnvelopeIssuer = "Team Foundation Service";
    public const string DefaultEnvelopeAppliesToAddress = "http://visualstudio.com/";
    public const int DefaultExpressRightsPrereleaseEnvelopeExpirationOffsetSeconds = 63072000;
    public const int DefaultExpressRightsReleaseEnvelopeExpirationOffsetSeconds = -1;
    public const int DefaultNonExpressRightsEnvelopeExpirationOffsetSeconds = 2592000;
    public const int DefaultTrialRightsReleaseEnvelopeExpirationOffsetSeconds = -1;
    public const int DefaultCommunityRightsReleaseEnvelopeExpirationOffsetSeconds = 31536000;
    public const int DefaultEmulatorAndroidRightsPrereleaseEnvelopeRefreshIntervalSeconds = 31536000;
    public const int DefaultEmulatorAndroidRightsReleaseEnvelopeRefreshIntervalSeconds = 31536000;
    public const int DefaultExpressRightsPrereleaseEnvelopeRefreshIntervalSeconds = 604800;
    public const int DefaultExpressRightsReleaseEnvelopeRefreshIntervalSeconds = 604800;
    public const int DefaultNonExpressRightsEnvelopeRefreshIntervalSeconds = 86400;
    public const int DefaultTrialRightsReleaseEnvelopeRefreshIntervalSeconds = 604800;
    public static readonly DateTimeOffset DefaultPrereleaseExpirationDate12 = new DateTimeOffset(2014, 1, 11, 0, 0, 0, TimeSpan.Zero);
    public static readonly DateTimeOffset DefaultPrereleaseExpirationDate14 = new DateTimeOffset(2015, 1, 11, 0, 0, 0, TimeSpan.Zero);
    public static readonly DateTimeOffset DefaultPrereleaseExpirationDate15 = new DateTimeOffset(2016, 1, 11, 0, 0, 0, TimeSpan.Zero);
    public static readonly DateTimeOffset DefaultPrereleaseExpirationDate16 = new DateTimeOffset(2019, 3, 31, 0, 0, 0, TimeSpan.Zero);
    public const string DefaultLicensingUrl = "https://go.microsoft.com/fwlink/?LinkID=290934";
    public const string DefaultLicensingSourceNameAccount = "Account";
    public const string DefaultLicensingSourceNameMsdn = "MSDN";
    public const string DefaultLicensingSourceNameProfile = "Profile";
    public const string DefaultLicenseDescriptionMsdn = "MSDN Subscription";
    public const string DefaultLicenseDescriptionMsdnMpn = "MSDN Subscription";
    public const string DefaultLicenseDescriptionVSCommunity = "Visual Studio Community";
    public const string DefaultLicenseDescriptionVSEmulatorAndroid = "Android Emulator";
    public const string DefaultLicenseDescriptionVSExpress = "Visual Studio Express";
    public const string DefaultLicenseDescriptionVSExtensionTrial = "90 Day Trial Extension (For evaluation purposes only)";
    public const string DefaultLicenseDescriptionVSPrerelease = "Visual Studio Prerelease";
    public const string DefaultLicenseDescriptionVSOSubscription = "Visual Studio Subscription";
    public const string DefaultLicenseDescriptionTestMangerPrerelease = "Prerelease software";
    public const string DefaultLicenseDescriptionVSCloudProfessionalAnnual = "Visual Studio Professional subscription";
    public const string DefaultLicenseDescriptionVSCloudProfessionalMonthly = "Visual Studio Professional subscription";
    public const string DefaultLicenseDescriptionVSCloudEnterpriseAnnual = "Visual Studio Enterprise subscription";
    public const string DefaultLicenseDescriptionVSCloudEnterpriseMonthly = "Visual Studio Enterprise subscription";
    public const string DefaultLicenseDescriptionVSEnterpriseGitHub = "Visual Studio Enterprise with GitHub subscription";
    public const string DefaultLicenseDescriptionVSProfessionalGitHub = "Visual Studio Professional with GitHub subscription";
    public const string DefaultLicenseDescriptionVSProfessionalMpn = "Visual Studio Professional with MPN subscription";
    public const string DefaultLicenseDescriptionVSEnterpriseMpn = "Visual Studio Enterprise with MPN subscription";
    public const string DefaultLicenseDescriptionVSEnterpriseNFRBasic = "Visual Studio Enterprise (NFR-Basic) subscription";
    public const string DefaultLicenseDescriptionVSEnterpriseISV = "Visual Studio Enterprise - ISV Subscription";
    public const string DefaultLicenseDescriptionTestManagerExtension = "Test Manager";
    private const string s_area = "VisualStudio.Services.LicensingService";
    private const string s_layer = "BusinessLogic";
    public const string LicensingCertDrawerName = "LicensingCertificates";

    public string EnvelopeIssuer { get; set; }

    public string EnvelopeAppliesToAddress { get; set; }

    public int ExpressRightsPrereleaseEnvelopeExpirationOffsetSeconds { get; set; }

    public int ExpressRightsReleaseEnvelopeExpirationOffsetSeconds { get; set; }

    public int NonExpressRightsEnvelopeExpirationOffsetSeconds { get; set; }

    public int TrialRightsReleaseEnvelopeExpirationOffsetSeconds { get; set; }

    public int ExpressRightsPrereleaseEnvelopeRefreshIntervalSeconds { get; set; }

    public int ExpressRightsReleaseEnvelopeRefreshIntervalSeconds { get; set; }

    public int NonExpressRightsEnvelopeRefreshIntervalSeconds { get; set; }

    public int TrialRightsReleaseEnvelopeRefreshIntervalSeconds { get; set; }

    public int CommunityRightsReleaseEnvelopeExpirationOffsetSeconds { get; set; }

    public int EmulatorAndroidRightsPrereleaseEnvelopeRefreshIntervalSeconds { get; set; }

    public int EmulatorAndroidRightsReleaseEnvelopeExpirationOffsetSeconds { get; set; }

    public DateTimeOffset PrereleaseExpirationDate12 { get; set; }

    public DateTimeOffset PrereleaseExpirationDate14 { get; set; }

    public DateTimeOffset PrereleaseExpirationDate15 { get; set; }

    public DateTimeOffset PrereleaseExpirationDate16 { get; set; }

    public string LicenseUrl { get; set; }

    public Dictionary<LicensingSource, string> LicensingSourceNames { get; set; }

    public Dictionary<string, string> LicenseDescriptions { get; set; }

    public Dictionary<string, string> LicenseDescriptionIds { get; set; }

    public string GetLicensingSourceName(IVssRequestContext requestContext, LicensingSource source)
    {
      string empty = string.Empty;
      if (this.LicensingSourceNames == null)
      {
        requestContext.Trace(1030061, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "Not initialized licensing source names.");
        return empty;
      }
      if (!this.LicensingSourceNames.TryGetValue(source, out empty))
        requestContext.Trace(1030052, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "Licensing source '{0}' not found in licensing source names.", (object) source);
      return empty;
    }

    public string GetLicenseDescriptionId(IVssRequestContext requestContext, string EntitlementCode)
    {
      string empty = string.Empty;
      if (this.LicenseDescriptionIds == null)
      {
        requestContext.Trace(1030071, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "Not initialized license descriptions Ids.");
        return empty;
      }
      if (!this.LicenseDescriptionIds.TryGetValue(EntitlementCode, out empty))
        requestContext.Trace(1030072, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "License descriptionId with EntitlementCode={0} not found in licenseId container.", (object) EntitlementCode);
      return empty;
    }

    public string GetLicenseDescription(IVssRequestContext requestContext, string descriptionId)
    {
      string empty = string.Empty;
      if (this.LicenseDescriptions == null)
      {
        requestContext.Trace(1030051, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "Not initialized license descriptions.");
        return empty;
      }
      if (!this.LicenseDescriptions.TryGetValue(descriptionId, out empty))
        requestContext.Trace(1030052, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "License description with id={0} not found in license descriptions container.", (object) descriptionId);
      return empty;
    }
  }
}
