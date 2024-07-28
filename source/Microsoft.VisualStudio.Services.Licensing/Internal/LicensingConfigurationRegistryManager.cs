// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.LicensingConfigurationRegistryManager
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class LicensingConfigurationRegistryManager : ILicensingConfigurationManager
  {
    private readonly ServiceFactory<IVssRegistryService> m_registryServiceFactory;
    private const string s_area = "VisualStudio.Services.LicensingService.LicensingSettingsRegistryManager";
    private const string s_layer = "BusinessLogic";
    private const string s_licensingRegistryRoot = "/Service/Licensing/";
    internal const string LicensingRegistryNotificationFilter = "/Service/Licensing/...";
    private const string s_envelopeIssuerRegistryKey = "/Service/Licensing/EnvelopeIssuer";
    private const string s_envelopeAppliesToAddressRegistryKey = "/Service/Licensing/EnvelopeAppliesToAddress";
    private const string s_expressRightsPrereleaseEnvelopeExpirationOffsetSecondsRegistryKey = "/Service/Licensing/ExpressRightsPrereleaseEnvelopeExpirationOffsetSeconds";
    private const string s_expressRightsReleaseEnvelopeExpirationOffsetSecondsRegistryKey = "/Service/Licensing/ExpressRightsReleaseEnvelopeExpirationOffsetSeconds";
    private const string s_nonExpressRightsEnvelopeExpirationOffsetSecondsRegistryKey = "/Service/Licensing/NonExpressRightsEnvelopeExpirationOffsetSeconds";
    private const string s_trialRightsReleaseEnvelopeExpirationOffsetSecondsRegistryKey = "/Service/Licensing/TrialRightsReleaseEnvelopeExpirationOffsetSeconds";
    private const string s_expressRightsPrereleaseEnvelopeRefreshIntervalSecondsRegistryKey = "/Service/Licensing/ExpressRightsPrereleaseEnvelopeRefreshIntervalSeconds";
    private const string s_expressRightsReleaseEnvelopeRefreshIntervalSecondsRegistryKey = "/Service/Licensing/ExpressRightsReleaseEnvelopeRefreshIntervalSeconds";
    private const string s_nonExpressRightsEnvelopeRefreshIntervalSecondsRegistryKey = "/Service/Licensing/NonExpressRightsEnvelopeRefreshIntervalSeconds";
    private const string s_trialRightsReleaseEnvelopeRefreshIntervalSecondsRegistryKey = "/Service/Licensing/TrialRightsReleaseEnvelopeRefreshIntervalSeconds";
    private const string s_communityRightsReleaseEnvelopeRefreshIntervalSecondsRegistryKey = "/Service/Licensing/CommunityRightsReleaseEnvelopeRefreshIntervalSeconds";
    private const string s_emulatorAndroidRightsReleaseEnvelopeRefreshIntervalSecondsRegistryKey = "/Service/Licensing/EmulatorAndroidRightsReleaseEnvelopeRefreshIntervalSeconds";
    private const string s_prereleaseExpirationDate12RegistryKey = "/Service/Licensing/12/PrereleaseExpirationDate";
    private const string s_prereleaseExpirationDate14RegistryKey = "/Service/Licensing/14/PrereleaseExpirationDate";
    private const string s_prereleaseExpirationDate15RegistryKey = "/Service/Licensing/15/PrereleaseExpirationDate";
    private const string s_prereleaseExpirationDate16RegistryKey = "/Service/Licensing/16/PrereleaseExpirationDate";
    private const string s_ctp5PrereleaseExpirationDate14RegistryKey = "/Service/Licensing/14/Ctp5PrereleaseExpirationDate";
    private const string s_ctp6PrereleaseExpirationDate14RegistryKey = "/Service/Licensing/14/Ctp6PrereleaseExpirationDate";
    private const string s_licensingUrlRegistryKey = "/Service/Licensing/LicensingUrl";
    private const string s_licensingSourceNamesRegistryPrefix = "/Service/Licensing/SourceNames/{0}";
    private const string s_licenseDescriptionsRegistryPrefix = "/Service/Licensing/LicenseDescriptions/{0}";
    private const string s_licenseDescriptionIdsRegistryPrefix = "/Service/Licensing/LicenseDescriptionIds/{0}";
    private const string s_accountEntitlementBranch = "AccountEntitlement/";
    private const string s_userEntitlementsBatchMaxSizeRegistryKey = "/Service/Licensing/AccountEntitlement/UserEntitlementsBatchMaxSize";
    private const string s_accountEntitlementSkuCollapseAnnouncementDate = "/Service/Licensing/AccountEntitlement/SkuCollapseAnnouncementDate";
    private const string s_accountEntitlementSearchMaxPageSizeRegistryKey = "/Service/Licensing/AccountEntitlement/SearchMaxPageSize";
    internal const string AccountEntitlementRegistryNotificationFilter = "/Service/Licensing/AccountEntitlement/...";
    private const string s_accountEntitlementUserCachedTokenTimeToLive = "/Service/Licensing/AccountEntitlement/TokenTimeToLive";
    private const string s_msdnBranch = "Msdn/";
    internal const string MsdnAdapterRegistryNotificationFilter = "/Service/Licensing/Msdn/...";
    private const string s_entitlementServiceCertificateThumbprintRegistryKey = "/Service/Licensing/Msdn/MsdnCertificateThumbprint";
    private const string s_entitlementServiceUrlNeedsCertificateRegistryKey = "/Service/Licensing/Msdn/EntitlementServiceNeedsCertificate";
    private const string s_entitlementServiceUrlRegistryKey = "/Service/Licensing/Msdn/EntitlementServiceUrl";
    private const string s_getEntitlementsTimeoutSecondsRegistryKey = "/Service/Licensing/Msdn/GetEntitlementsTimeoutSeconds";
    private const string s_getEntitlementsRetriesRegistryKey = "/Service/Licensing/Msdn/GetEntitlementsRetries";
    private const string s_getEntitlementsSlowRequestThresholdSecondsRegistryKey = "/Service/Licensing/Msdn/GetEntitlementsSlowRequestThresholdSeconds";
    private const string s_vsaccountBranch = "VisualStudioAccount/";
    internal const string VisualStudioAccountAdapterRegistryNotificationFilter = "/Service/Licensing/VisualStudioAccount/...";
    private const string s_visualStudioAccountLicenseDurationMonths = "/Service/Licensing/VisualStudioAccount/LicenseDurationMonths";
    private const string s_extensionBranch = "Extension/";
    internal const string ExtensionAdapterRegistryNotificationFilter = "/Service/Licensing/Extension/...";
    private const string s_extensionLicenseDurationMonths = "/Service/Licensing/Extension/LicenseDurationMonths";
    private const string s_vscommunityBranch = "VisualStudioCommunity/";
    internal const string VisualStudioCommunityAdapterRegistryNotificationFilter = "/Service/Licensing/VisualStudioCommunity/...";
    private const string s_visualStudioCommunityLicenseDuration = "/Service/Licensing/VisualStudioCommunity/LicenseDuration";
    private const string s_vsEmulatorAndroidBranch = "VisualStudioEmulatorAndroid/";
    internal const string VisualStudioEmulatorAndroidAdapterRegistryNotificationFilter = "/Service/Licensing/VisualStudioEmulatorAndroid/...";
    private const string s_visualStudioEmulatorAndroidLicenseDuration = "/Service/Licensing/VisualStudioEmulatorAndroid/LicenseDuration";
    private const string s_vstrialBranch = "VisualStudioTrial/";
    internal const string VisualStudioTrialAdapterRegistryNotificationFilter = "/Service/Licensing/VisualStudioTrial/...";
    private const string s_visualStudioTrialExtensionDuration = "/Service/Licensing/VisualStudioTrial/TrialExtensionDuration";
    private const string s_extensionEntitlementBranch = "AccountExtensionEntitlement/";
    internal const string ExtensionEntitlementRegistryNotificationFilter = "/Service/Licensing/AccountExtensionEntitlement/...";
    private const string s_extensionEntitlementUserCachedTokenTimeToLive = "/Service/Licensing/AccountExtensionEntitlement/TokenTimeToLive";

    public LicensingConfigurationRegistryManager()
      : this((ServiceFactory<IVssRegistryService>) (x => (IVssRegistryService) x.GetService<CachedRegistryService>()))
    {
    }

    internal LicensingConfigurationRegistryManager(
      ServiceFactory<IVssRegistryService> registryServiceFactory)
    {
      this.m_registryServiceFactory = registryServiceFactory;
    }

    public LicensingServiceConfiguration GetLicensingServiceConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService registryService = this.m_registryServiceFactory(vssRequestContext);
      return new LicensingServiceConfiguration()
      {
        EnvelopeIssuer = registryService.GetValue(vssRequestContext, (RegistryQuery) "/Service/Licensing/EnvelopeIssuer", "Team Foundation Service"),
        EnvelopeAppliesToAddress = registryService.GetValue(vssRequestContext, (RegistryQuery) "/Service/Licensing/EnvelopeAppliesToAddress", "http://visualstudio.com/"),
        ExpressRightsPrereleaseEnvelopeExpirationOffsetSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/ExpressRightsPrereleaseEnvelopeExpirationOffsetSeconds", 63072000),
        ExpressRightsReleaseEnvelopeExpirationOffsetSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/ExpressRightsReleaseEnvelopeExpirationOffsetSeconds", -1),
        NonExpressRightsEnvelopeExpirationOffsetSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/NonExpressRightsEnvelopeExpirationOffsetSeconds", 2592000),
        TrialRightsReleaseEnvelopeExpirationOffsetSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/TrialRightsReleaseEnvelopeExpirationOffsetSeconds", -1),
        ExpressRightsPrereleaseEnvelopeRefreshIntervalSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/ExpressRightsPrereleaseEnvelopeRefreshIntervalSeconds", 604800),
        ExpressRightsReleaseEnvelopeRefreshIntervalSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/ExpressRightsReleaseEnvelopeRefreshIntervalSeconds", 604800),
        NonExpressRightsEnvelopeRefreshIntervalSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/NonExpressRightsEnvelopeRefreshIntervalSeconds", 86400),
        TrialRightsReleaseEnvelopeRefreshIntervalSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/TrialRightsReleaseEnvelopeRefreshIntervalSeconds", 604800),
        CommunityRightsReleaseEnvelopeExpirationOffsetSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/CommunityRightsReleaseEnvelopeRefreshIntervalSeconds", 31536000),
        EmulatorAndroidRightsReleaseEnvelopeExpirationOffsetSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/EmulatorAndroidRightsReleaseEnvelopeRefreshIntervalSeconds", 31536000),
        PrereleaseExpirationDate12 = registryService.GetValue<DateTimeOffset>(vssRequestContext, (RegistryQuery) "/Service/Licensing/12/PrereleaseExpirationDate", LicensingServiceConfiguration.DefaultPrereleaseExpirationDate12),
        PrereleaseExpirationDate14 = registryService.GetValue<DateTimeOffset>(vssRequestContext, (RegistryQuery) "/Service/Licensing/14/PrereleaseExpirationDate", LicensingServiceConfiguration.DefaultPrereleaseExpirationDate14),
        PrereleaseExpirationDate15 = registryService.GetValue<DateTimeOffset>(vssRequestContext, (RegistryQuery) "/Service/Licensing/15/PrereleaseExpirationDate", LicensingServiceConfiguration.DefaultPrereleaseExpirationDate15),
        PrereleaseExpirationDate16 = registryService.GetValue<DateTimeOffset>(vssRequestContext, (RegistryQuery) "/Service/Licensing/16/PrereleaseExpirationDate", LicensingServiceConfiguration.DefaultPrereleaseExpirationDate16),
        LicenseUrl = registryService.GetValue(vssRequestContext, (RegistryQuery) "/Service/Licensing/LicensingUrl", "https://go.microsoft.com/fwlink/?LinkID=290934"),
        LicensingSourceNames = new Dictionary<LicensingSource, string>()
        {
          {
            LicensingSource.Account,
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/SourceNames/{0}", (object) LicensingSource.Account), "Account")
          },
          {
            LicensingSource.Msdn,
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/SourceNames/{0}", (object) LicensingSource.Msdn), "MSDN")
          },
          {
            LicensingSource.Profile,
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/SourceNames/{0}", (object) LicensingSource.Profile), "Profile")
          }
        },
        LicenseDescriptions = new Dictionary<string, string>()
        {
          {
            "MSDN",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "MSDN"), "MSDN Subscription")
          },
          {
            "MSDNMPN",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "MSDNMPN"), "MSDN Subscription")
          },
          {
            "VSExpress",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSExpress"), "Visual Studio Express")
          },
          {
            "VSExtensionTrial",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSExtensionTrial"), "90 Day Trial Extension (For evaluation purposes only)")
          },
          {
            "VSPrerelease",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSPrerelease"), "Visual Studio Prerelease")
          },
          {
            "VSOSubscription",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSOSubscription"), "Visual Studio Subscription")
          },
          {
            "VSCommunity",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSCommunity"), "Visual Studio Community")
          },
          {
            "VSEmulatorAndroid",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSEmulatorAndroid"), "Android Emulator")
          },
          {
            "MTMPrerelease",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "MTMPrerelease"), "Prerelease software")
          },
          {
            "VSEnterpriseAnnual",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSEnterpriseAnnual"), "Visual Studio Enterprise subscription")
          },
          {
            "VSEnterpriseMonthly",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSEnterpriseMonthly"), "Visual Studio Enterprise subscription")
          },
          {
            "VSProfessionalAnnual",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSProfessionalAnnual"), "Visual Studio Professional subscription")
          },
          {
            "VSProfessionalMonthly",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSProfessionalMonthly"), "Visual Studio Professional subscription")
          },
          {
            "TestManager",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "TestManager"), "Test Manager")
          },
          {
            "VSEnterpriseGitHub",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSEnterpriseGitHub"), "Visual Studio Enterprise with GitHub subscription")
          },
          {
            "VSProfessionalGitHub",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSProfessionalGitHub"), "Visual Studio Professional with GitHub subscription")
          },
          {
            "VSProfessionalMPN",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSProfessionalMPN"), "Visual Studio Professional with MPN subscription")
          },
          {
            "VSEnterpriseNFRBasic",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSEnterpriseNFRBasic"), "Visual Studio Enterprise (NFR-Basic) subscription")
          },
          {
            "VSEnterpriseISV",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSEnterpriseISV"), "Visual Studio Enterprise - ISV Subscription")
          },
          {
            "VSEnterpriseMPN",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptions/{0}", (object) "VSEnterpriseMPN"), "Visual Studio Enterprise with MPN subscription")
          }
        },
        LicenseDescriptionIds = new Dictionary<string, string>()
        {
          {
            "X-RTL-000052",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "X-RTL-000052"), "VSProfessionalMPN")
          },
          {
            "X-RTL-000056",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "X-RTL-000056"), "VSEnterpriseMPN")
          },
          {
            "VSE-GIT-000001",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "VSE-GIT-000001"), "VSEnterpriseGitHub")
          },
          {
            "VSP-GIT-000001",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "VSP-GIT-000001"), "VSProfessionalGitHub")
          },
          {
            "VSP-MON-0001",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "VSP-MON-0001"), "VSProfessionalMonthly")
          },
          {
            "VSP-ANU-0001",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "VSP-ANU-0001"), "VSProfessionalAnnual")
          },
          {
            "VSE-MON-0001",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "VSE-MON-0001"), "VSEnterpriseMonthly")
          },
          {
            "VSE-ANU-0001",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "VSE-ANU-0001"), "VSEnterpriseAnnual")
          },
          {
            "ENT-NFR-BASIC",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "ENT-NFR-BASIC"), "VSEnterpriseNFRBasic")
          },
          {
            "VSE-ISV-0001",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "VSE-ISV-0001"), "VSEnterpriseISV")
          },
          {
            "EXT-MTM-0001",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "EXT-MTM-0001"), "TestManager")
          },
          {
            "TestManager",
            registryService.GetValue<string>(vssRequestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/Licensing/LicenseDescriptionIds/{0}", (object) "TestManager"), "TestManager")
          }
        }
      };
    }

    public MsdnAdapterConfiguration GetMsdnAdapterConfiguration(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService registryService = this.m_registryServiceFactory(vssRequestContext);
      MsdnAdapterConfiguration adapterConfiguration = new MsdnAdapterConfiguration();
      adapterConfiguration.GetEntitlementsTimeoutSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/Msdn/GetEntitlementsTimeoutSeconds", 8);
      if (adapterConfiguration.GetEntitlementsTimeoutSeconds < 0)
        throw new LicensingInvalidSettingsException("GetEntitlementsTimeoutSeconds should not be negative value.");
      adapterConfiguration.GetEntitlementsRetries = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/Msdn/GetEntitlementsRetries", 2);
      if (adapterConfiguration.GetEntitlementsRetries < 0)
        throw new LicensingInvalidSettingsException("GetEntitlementsRetries should not be negative value.");
      adapterConfiguration.GetEntitlementsSlowRequestThresholdSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/Msdn/GetEntitlementsSlowRequestThresholdSeconds", 3);
      if (adapterConfiguration.GetEntitlementsSlowRequestThresholdSeconds < 0)
        throw new LicensingInvalidSettingsException("GetEntitlementsSlowRequestThresholdSeconds should not be negative value.");
      adapterConfiguration.CircuitBreakerSettingsForGetMsdnEntitlements = MsdnAdapterConfiguration.DefaultCircuitBreakerSettingsForGetMsdnEntitlements;
      return adapterConfiguration;
    }

    public VisualStudioAccountAdapterConfiguration GetVisualStudioAccountAdapterConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService registryService = this.m_registryServiceFactory(vssRequestContext);
      return new VisualStudioAccountAdapterConfiguration()
      {
        LicenseDurationMonths = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/VisualStudioAccount/LicenseDurationMonths", 1)
      };
    }

    public ExtensionAdapterConfiguration GetExtensionAdapterConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService registryService = this.m_registryServiceFactory(vssRequestContext);
      return new ExtensionAdapterConfiguration()
      {
        LicenseDurationMonths = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/Extension/LicenseDurationMonths", 1)
      };
    }

    public VisualStudioTrialAdapterConfiguration GetVisualStudioTrialAdapterConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService registryService = this.m_registryServiceFactory(vssRequestContext);
      return new VisualStudioTrialAdapterConfiguration()
      {
        TrialExtensionDuration = registryService.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Service/Licensing/VisualStudioTrial/TrialExtensionDuration", VisualStudioTrialAdapterConfiguration.DefaultTrialExtensionDuration)
      };
    }

    public VisualStudioCommunityAdapterConfiguration GetVisualStudioCommunityAdapterConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService registryService = this.m_registryServiceFactory(vssRequestContext);
      return new VisualStudioCommunityAdapterConfiguration()
      {
        LicenseDurationSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/VisualStudioCommunity/LicenseDuration", -1)
      };
    }

    public VisualStudioEmulatorAndroidAdapterConfiguration GetVisualStudioEmulatorAndroidAdapterConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService registryService = this.m_registryServiceFactory(vssRequestContext);
      return new VisualStudioEmulatorAndroidAdapterConfiguration()
      {
        LicenseDurationSeconds = registryService.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/VisualStudioEmulatorAndroid/LicenseDuration", -1)
      };
    }

    public AccountEntitlementServiceConfiguration GetAccountEntitlementServiceConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      return new AccountEntitlementServiceConfiguration()
      {
        SkuCollapseAnnouncementDate = service.GetValue<DateTime>(vssRequestContext, (RegistryQuery) "/Service/Licensing/AccountEntitlement/SkuCollapseAnnouncementDate", AccountEntitlementServiceConfiguration.DefaultSkuCollapseAnnouncementDate),
        UserEntitlementsBatchMaxSize = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/AccountEntitlement/UserEntitlementsBatchMaxSize", 100)
      };
    }

    public AccountEntitlementCacheConfiguration GetAccountEntitlementCacheConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService registryService = this.m_registryServiceFactory(vssRequestContext);
      return new AccountEntitlementCacheConfiguration()
      {
        TokenTimeToLive = registryService.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Service/Licensing/AccountEntitlement/TokenTimeToLive", AccountEntitlementCacheConfiguration.DefaultTokenTimeToLive)
      };
    }

    public ExtensionEntitlementCacheConfiguration GetExtensionEntitlementCacheConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService registryService = this.m_registryServiceFactory(vssRequestContext);
      return new ExtensionEntitlementCacheConfiguration()
      {
        TokenTimeToLive = registryService.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Service/Licensing/AccountExtensionEntitlement/TokenTimeToLive", ExtensionEntitlementCacheConfiguration.DefaultTokenTimeToLive)
      };
    }

    public int GetAccountEntitlementSearchConfiguration(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return this.m_registryServiceFactory(vssRequestContext).GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/AccountEntitlement/SearchMaxPageSize", 100);
    }
  }
}
