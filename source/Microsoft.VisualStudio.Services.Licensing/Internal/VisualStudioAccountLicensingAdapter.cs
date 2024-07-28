// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.VisualStudioAccountLicensingAdapter
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Microsoft.VisualStudio.Services.UserMapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class VisualStudioAccountLicensingAdapter : ILicensingAdapter
  {
    internal static readonly Dictionary<long, EntitlementToRightsRule> RulesMap = VisualStudioAccountLicensingAdapter.CreateRulesMap();
    internal static readonly Dictionary<long, EntitlementToRightsRule> MacRulesMap = VisualStudioAccountLicensingAdapter.CreateMacRulesMap();
    internal static readonly Dictionary<long, EntitlementToRightsRule> TestManagerRulesMap = VisualStudioAccountLicensingAdapter.CreateTestManagerRulesMap();
    private readonly ILicensingConfigurationManager m_settingsManager;
    private readonly ServiceFactory<IVssRegistryService> m_registryServiceFactory;
    protected LicensingServiceConfiguration m_serviceSettings;
    protected VisualStudioAccountAdapterConfiguration m_adapterSettings;
    private IVssDateTimeProvider m_dateTimeProvider;
    private const VisualStudioOnlineServiceLevel DefaultServiceLevel = VisualStudioOnlineServiceLevel.Stakeholder;
    private const string s_area = "VisualStudio.Services.LicensingService.VisualStudioAccountLicensingAdapter";
    private const string s_layer = "BusinessLogic";
    private const string s_VsoSubscriptionProgram = "VSTS Subscription";

    public VisualStudioAccountLicensingAdapter()
      : this((ILicensingConfigurationManager) new LicensingConfigurationRegistryManager(), (ServiceFactory<IVssRegistryService>) (x => (IVssRegistryService) x.GetService<CachedRegistryService>()), VssDateTimeProvider.DefaultProvider)
    {
    }

    public VisualStudioAccountLicensingAdapter(
      ILicensingConfigurationManager settingsManager,
      ServiceFactory<IVssRegistryService> registryServiceFactory,
      IVssDateTimeProvider dateTimeProvider)
    {
      this.m_settingsManager = settingsManager;
      this.m_registryServiceFactory = registryServiceFactory;
      this.m_dateTimeProvider = dateTimeProvider;
    }

    public void Start(
      IVssRequestContext requestContext,
      LicensingServiceConfiguration serviceSettings,
      IVssDateTimeProvider dateTimeProvider)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<LicensingServiceConfiguration>(serviceSettings, nameof (serviceSettings));
      this.m_dateTimeProvider = dateTimeProvider;
      this.m_registryServiceFactory(requestContext).RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, "/Service/Licensing/VisualStudioAccount/...");
      this.m_serviceSettings = serviceSettings;
      this.PopulateSettings(requestContext);
    }

    public void Stop(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_registryServiceFactory(requestContext).UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    public IList<IUsageRight> GetRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IRightsQueryContext>(queryContext, nameof (queryContext));
      requestContext.TraceEnter(1033200, "VisualStudio.Services.LicensingService.VisualStudioAccountLicensingAdapter", "BusinessLogic", nameof (GetRights));
      VisualStudioAccountLicensingAdapter.ValidateQueryContext(requestContext, queryContext);
      IList<IUsageRight> rights = (IList<IUsageRight>) null;
      try
      {
        switch (queryContext.RequestType)
        {
          case LicensingRequestType.Client:
            rights = this.CreateClientRights(requestContext, queryContext);
            return rights;
          case LicensingRequestType.Service:
            rights = this.CreateServiceRights(requestContext, queryContext);
            return rights;
          default:
            requestContext.Trace(1033201, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioAccountLicensingAdapter", "BusinessLogic", "Not recognized queryContext.RequestType: {0}.", (object) queryContext.RequestType);
            rights = (IList<IUsageRight>) new List<IUsageRight>();
            return rights;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1033208, "VisualStudio.Services.LicensingService.VisualStudioAccountLicensingAdapter", "BusinessLogic", ex);
        return (IList<IUsageRight>) new List<IUsageRight>();
      }
      finally
      {
        requestContext.TraceProperties<IList<IUsageRight>>(1033207, "VisualStudio.Services.LicensingService.VisualStudioAccountLicensingAdapter", "BusinessLogic", rights, (string) null);
        requestContext.TraceLeave(1033209, "VisualStudio.Services.LicensingService.VisualStudioAccountLicensingAdapter", "BusinessLogic", nameof (GetRights));
      }
    }

    private void PopulateSettings(IVssRequestContext requestContext) => this.m_adapterSettings = this.m_settingsManager.GetVisualStudioAccountAdapterConfiguration(requestContext);

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(1033210, "VisualStudio.Services.LicensingService.VisualStudioAccountLicensingAdapter", "BusinessLogic", nameof (OnRegistryChanged));
      try
      {
        if (changedEntries == null || !changedEntries.Any<RegistryEntry>())
          return;
        this.PopulateSettings(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1033218, "VisualStudio.Services.LicensingService.VisualStudioAccountLicensingAdapter", "BusinessLogic", ex);
      }
      finally
      {
        requestContext.TraceLeave(1033219, "VisualStudio.Services.LicensingService.VisualStudioAccountLicensingAdapter", "BusinessLogic", nameof (OnRegistryChanged));
      }
    }

    public IList<IUsageRight> CreateClientRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      UserClaims userClaims = UserClaims.GetUserClaims(requestContext, queryContext.UserIdentity);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      HashSet<Guid> guidSet = new HashSet<Guid>();
      using (IDisposableReadOnlyList<IUserAccountMappingMigrationExtension> extensions = requestContext.GetExtensions<IUserAccountMappingMigrationExtension>())
      {
        foreach (IUserAccountMappingMigrationExtension migrationExtension in (IEnumerable<IUserAccountMappingMigrationExtension>) extensions)
        {
          IEnumerable<Guid> values = (IEnumerable<Guid>) migrationExtension.QueryAccountIds(vssRequestContext.Elevate(), queryContext.UserIdentity, UserRole.Member);
          guidSet.AddRange<Guid, HashSet<Guid>>(values);
        }
      }
      IEnumerable<License> accountEntitlements = requestContext.GetService<IAccountEntitlementCache>().GetAccountEntitlements(requestContext.Elevate(), (IEnumerable<Guid>) guidSet, queryContext.UserIdentity.MasterId);
      List<IUsageRight> clientRights = new List<IUsageRight>();
      foreach (License license in accountEntitlements)
      {
        EntitlementToRightsRule rule = (EntitlementToRightsRule) null;
        long ruleKey = VisualStudioAccountLicensingAdapter.GenerateRuleKey(license.Source, license.GetLicenseAsInt32());
        if (queryContext.VisualStudioFamily == VisualStudioFamily.TestManager)
          VisualStudioAccountLicensingAdapter.TestManagerRulesMap.TryGetValue(ruleKey, out rule);
        else if (queryContext.VisualStudioFamily == VisualStudioFamily.VisualStudioMac)
          VisualStudioAccountLicensingAdapter.MacRulesMap.TryGetValue(ruleKey, out rule);
        else
          VisualStudioAccountLicensingAdapter.RulesMap.TryGetValue(ruleKey, out rule);
        if (rule != null && rule.ClientFamily != VisualStudioFamily.Invalid && (queryContext.VisualStudioEdition != VisualStudioEdition.Community || queryContext.ProductVersion.Major >= 14))
        {
          if (queryContext.VisualStudioEdition != VisualStudioEdition.Unspecified && 0 > VisualStudioRight.CompareVisualStudioEdition(rule.ClientEdition, queryContext.VisualStudioEdition))
          {
            if (rule.ClientEdition == VisualStudioEdition.Professional && queryContext.VisualStudioEdition == VisualStudioEdition.Enterprise)
            {
              VisualStudioRight visualStudioRight = this.CreateVisualStudioRight(requestContext, queryContext, rule);
              clientRights.Add((IUsageRight) this.UpdateRightsWithTrialProperties(requestContext, visualStudioRight));
            }
          }
          else
          {
            if (queryContext.VisualStudioEdition != VisualStudioEdition.Unspecified && queryContext.VisualStudioEdition != VisualStudioEdition.Community && 0 < VisualStudioRight.CompareVisualStudioEdition(rule.ClientEdition, queryContext.VisualStudioEdition))
              rule.ClientEdition = queryContext.VisualStudioEdition;
            if (userClaims == null || !(userClaims.Type == "ORGID") || (rule.ClientFamily == VisualStudioFamily.TestManager ? 0 : (rule.ClientEdition != VisualStudioEdition.Professional ? 1 : 0)) == 0)
            {
              if (rule.ClientFamily == VisualStudioFamily.TestManager)
                clientRights.Add((IUsageRight) this.CreateTestManagerRight(requestContext, queryContext, rule));
              if (rule.ClientFamily != VisualStudioFamily.TestManager)
                clientRights.Add((IUsageRight) this.CreateVisualStudioRight(requestContext, queryContext, rule));
            }
          }
        }
      }
      return (IList<IUsageRight>) clientRights;
    }

    public IList<IUsageRight> CreateServiceRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      List<IUsageRight> serviceRights = new List<IUsageRight>();
      License license1 = requestContext.GetService<IAccountEntitlementCache>().GetAccountEntitlement(requestContext.Elevate(), queryContext.AccountId, queryContext.UserIdentity.MasterId);
      if ((object) license1 == null)
        license1 = License.None;
      License license2 = license1;
      EntitlementToRightsRule rule = (EntitlementToRightsRule) null;
      long ruleKey = VisualStudioAccountLicensingAdapter.GenerateRuleKey(license2.Source, license2.GetLicenseAsInt32());
      if (VisualStudioAccountLicensingAdapter.RulesMap.TryGetValue(ruleKey, out rule) && rule.ServiceLevel != VisualStudioOnlineServiceLevel.None)
        serviceRights.Add((IUsageRight) this.CreateServiceRight(requestContext, rule));
      return (IList<IUsageRight>) serviceRights;
    }

    private VisualStudioRight UpdateRightsWithTrialProperties(
      IVssRequestContext requestContext,
      VisualStudioRight maxRight)
    {
      maxRight.LicenseDescriptionId = "VSExtensionTrial";
      maxRight.LicenseFallbackDescription = this.m_serviceSettings.GetLicenseDescription(requestContext, "VSExtensionTrial");
      maxRight.Attributes["IsTrialLicense"] = (object) true;
      maxRight.Attributes["SubscriptionChannel"] = (object) "Evaluation";
      return maxRight;
    }

    private VisualStudioRight CreateVisualStudioRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      EntitlementToRightsRule rule)
    {
      return VisualStudioRight.Create(rule.ClientRightName, queryContext.ProductVersion, rule.ClientEdition, DateTimeOffset.MaxValue.AddSeconds(-1.0), string.Empty, string.Empty, "VSOSubscription", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSOSubscription"), new Dictionary<string, object>()
      {
        {
          "SubscriptionChannel",
          (object) "VSTS Subscription"
        },
        {
          "SubscriptionLevel",
          (object) rule.ClientEdition.ToString()
        }
      });
    }

    private TestManagerRight CreateTestManagerRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      EntitlementToRightsRule rule)
    {
      return TestManagerRight.Create(queryContext.SingleRightName, queryContext.ProductVersion, queryContext.VisualStudioEdition, DateTimeOffset.MaxValue.AddSeconds(-1.0), string.Empty, string.Empty, "VSOSubscription", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSOSubscription"), new Dictionary<string, object>()
      {
        {
          "SubscriptionChannel",
          (object) "VSTS Subscription"
        },
        {
          "SubscriptionLevel",
          (object) rule.ClientEdition.ToString()
        }
      });
    }

    private VisualStudioOnlineServiceRight CreateServiceRight(
      IVssRequestContext requestContext,
      EntitlementToRightsRule rule)
    {
      return VisualStudioOnlineServiceRight.Create(rule.ServiceLevel, this.GetOffsetUtcNow().AddMonths(this.m_adapterSettings.LicenseDurationMonths));
    }

    private static void ValidateQueryContext(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      if (queryContext.RequestType == LicensingRequestType.Client && queryContext.ProductVersion == (Version) null)
      {
        requestContext.Trace(1033220, TraceLevel.Error, "VisualStudio.Services.LicensingService.VisualStudioAccountLicensingAdapter", "BusinessLogic", "Invalid product version.");
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductVersion());
      }
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.m_dateTimeProvider.UtcNow;

    [ExcludeFromCodeCoverage]
    internal virtual DateTimeOffset GetOffsetUtcNow() => new DateTimeOffset(this.m_dateTimeProvider.UtcNow);

    private static Dictionary<long, EntitlementToRightsRule> CreateRulesMap()
    {
      Dictionary<long, EntitlementToRightsRule> map = new Dictionary<long, EntitlementToRightsRule>();
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.EarlyAdopter, VisualStudioOnlineServiceLevel.AdvancedPlus, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Stakeholder, VisualStudioOnlineServiceLevel.Stakeholder, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Express, VisualStudioOnlineServiceLevel.Express, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Professional, VisualStudioOnlineServiceLevel.Express, "VisualStudio", VisualStudioFamily.VisualStudio, VisualStudioEdition.Professional));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Advanced, VisualStudioOnlineServiceLevel.Advanced, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Eligible, VisualStudioOnlineServiceLevel.Stakeholder, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Professional, VisualStudioOnlineServiceLevel.Express, "VisualStudio", VisualStudioFamily.VisualStudio, VisualStudioEdition.Professional));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Platforms, VisualStudioOnlineServiceLevel.AdvancedPlus, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Premium, VisualStudioOnlineServiceLevel.AdvancedPlus, "VisualStudio", VisualStudioFamily.VisualStudio, VisualStudioEdition.Premium));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Ultimate, VisualStudioOnlineServiceLevel.AdvancedPlus, "VisualStudio", VisualStudioFamily.VisualStudio, VisualStudioEdition.Ultimate));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.TestProfessional, VisualStudioOnlineServiceLevel.AdvancedPlus, "TestProfessional", VisualStudioFamily.VisualStudioTestProfessional, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Enterprise, VisualStudioOnlineServiceLevel.AdvancedPlus, "VisualStudio", VisualStudioFamily.VisualStudio, VisualStudioEdition.Enterprise));
      return map;
    }

    private static Dictionary<long, EntitlementToRightsRule> CreateMacRulesMap()
    {
      Dictionary<long, EntitlementToRightsRule> map = new Dictionary<long, EntitlementToRightsRule>();
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.EarlyAdopter, VisualStudioOnlineServiceLevel.AdvancedPlus, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Stakeholder, VisualStudioOnlineServiceLevel.Stakeholder, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Express, VisualStudioOnlineServiceLevel.Express, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Professional, VisualStudioOnlineServiceLevel.Express, "VSonMac", VisualStudioFamily.VisualStudioMac, VisualStudioEdition.Professional));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Advanced, VisualStudioOnlineServiceLevel.Advanced, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Eligible, VisualStudioOnlineServiceLevel.Stakeholder, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Professional, VisualStudioOnlineServiceLevel.Express, "VSonMac", VisualStudioFamily.VisualStudioMac, VisualStudioEdition.Professional));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Platforms, VisualStudioOnlineServiceLevel.AdvancedPlus, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Premium, VisualStudioOnlineServiceLevel.AdvancedPlus, "VSonMac", VisualStudioFamily.VisualStudioMac, VisualStudioEdition.Premium));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Ultimate, VisualStudioOnlineServiceLevel.AdvancedPlus, "VSonMac", VisualStudioFamily.VisualStudioMac, VisualStudioEdition.Ultimate));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Enterprise, VisualStudioOnlineServiceLevel.AdvancedPlus, "VSonMac", VisualStudioFamily.VisualStudioMac, VisualStudioEdition.Enterprise));
      return map;
    }

    private static Dictionary<long, EntitlementToRightsRule> CreateTestManagerRulesMap()
    {
      Dictionary<long, EntitlementToRightsRule> map = new Dictionary<long, EntitlementToRightsRule>();
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.EarlyAdopter, VisualStudioOnlineServiceLevel.None, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Stakeholder, VisualStudioOnlineServiceLevel.None, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Express, VisualStudioOnlineServiceLevel.None, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Professional, VisualStudioOnlineServiceLevel.None, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) AccountLicense.Advanced, VisualStudioOnlineServiceLevel.None, "TestManager", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Professional, VisualStudioOnlineServiceLevel.None, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Platforms, VisualStudioOnlineServiceLevel.None, "TestManager", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Premium, VisualStudioOnlineServiceLevel.None, "TestManager", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Ultimate, VisualStudioOnlineServiceLevel.None, "TestManager", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.TestProfessional, VisualStudioOnlineServiceLevel.None, "TestManager", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified));
      VisualStudioAccountLicensingAdapter.AddRule(map, new EntitlementToRightsRule((License) MsdnLicense.Enterprise, VisualStudioOnlineServiceLevel.None, "TestManager", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified));
      return map;
    }

    private static void AddRule(
      Dictionary<long, EntitlementToRightsRule> map,
      EntitlementToRightsRule rule)
    {
      map.Add(VisualStudioAccountLicensingAdapter.GenerateRuleKey(rule.Source, rule.License), rule);
    }

    internal static long GenerateRuleKey(LicensingSource source, int license) => (long) (2 ^ 32 * (int) source + license);
  }
}
