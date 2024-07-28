// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.ExtensionLicensingAdapter
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
  public class ExtensionLicensingAdapter : ILicensingAdapter
  {
    internal static readonly IDictionary<string, string> TestManagerRulesMap = ExtensionLicensingAdapter.CreateTestManagerRulesMap();
    private readonly ILicensingConfigurationManager m_settingsManager;
    private readonly ServiceFactory<IVssRegistryService> m_registryServiceFactory;
    protected LicensingServiceConfiguration m_serviceSettings;
    protected ExtensionAdapterConfiguration m_adapterSettings;
    private IVssDateTimeProvider m_dateTimeProvider;
    private const VisualStudioOnlineServiceLevel DefaultServiceLevel = VisualStudioOnlineServiceLevel.Stakeholder;
    private const string s_area = "VisualStudio.Services.LicensingService";
    private const string s_layer = "ExtensionLicensingAdapter";

    public ExtensionLicensingAdapter()
      : this((ILicensingConfigurationManager) new LicensingConfigurationRegistryManager(), (ServiceFactory<IVssRegistryService>) (x => (IVssRegistryService) x.GetService<CachedRegistryService>()), VssDateTimeProvider.DefaultProvider)
    {
    }

    public ExtensionLicensingAdapter(
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
      this.m_registryServiceFactory(requestContext).RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, "/Service/Licensing/Extension/...");
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
      requestContext.TraceEnter(1033700, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), nameof (GetRights));
      ExtensionLicensingAdapter.ValidateQueryContext(requestContext, queryContext);
      try
      {
        switch (queryContext.RequestType)
        {
          case LicensingRequestType.Client:
            return this.CreateClientRights(requestContext, queryContext);
          case LicensingRequestType.Service:
            return this.CreateServiceRights(requestContext, queryContext);
          default:
            requestContext.Trace(1033701, TraceLevel.Error, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), "Not recognized queryContext.RequestType: {0}.", (object) queryContext.RequestType);
            return (IList<IUsageRight>) new List<IUsageRight>();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1033708, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), ex);
        return (IList<IUsageRight>) new List<IUsageRight>();
      }
      finally
      {
        requestContext.TraceLeave(1033709, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), nameof (GetRights));
      }
    }

    private void PopulateSettings(IVssRequestContext requestContext) => this.m_adapterSettings = this.m_settingsManager.GetExtensionAdapterConfiguration(requestContext);

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(1033710, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), nameof (OnRegistryChanged));
      try
      {
        if (changedEntries == null || !changedEntries.Any<RegistryEntry>())
          return;
        this.PopulateSettings(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1033718, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), ex);
      }
      finally
      {
        requestContext.TraceLeave(1033719, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), nameof (OnRegistryChanged));
      }
    }

    public IList<IUsageRight> CreateClientRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      List<IUsageRight> clientRights = new List<IUsageRight>();
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
      IList<string> extensionsAssignedToUser = requestContext.GetService<IExtensionEntitlementCache>().GetExtensionsAssignedToUser(requestContext.Elevate(), (IEnumerable<Guid>) guidSet, queryContext.UserIdentity.MasterId);
      if (!extensionsAssignedToUser.Any<string>())
      {
        requestContext.Trace(1033711, TraceLevel.Info, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), "User {0}: No active extension for this user.", (object) queryContext.UserIdentity.MasterId);
        return (IList<IUsageRight>) ExtensionLicensingAdapter.CreateDefaultRights();
      }
      if (extensionsAssignedToUser.Any<string>((Func<string, bool>) (s => ExtensionLicensingAdapter.TestManagerRulesMap.Values.Contains<string>(s, (IEqualityComparer<string>) LicensingComparers.ExtensionComparer))))
      {
        requestContext.Trace(1033712, TraceLevel.Info, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), "User {0}: has valid extension.", (object) queryContext.UserIdentity.MasterId);
        clientRights.Add((IUsageRight) this.CreateTestManagerRight(requestContext, queryContext));
      }
      return (IList<IUsageRight>) clientRights;
    }

    public IList<IUsageRight> CreateServiceRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      return (IList<IUsageRight>) new List<IUsageRight>();
    }

    private TestManagerRight CreateTestManagerRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string licenseDescriptionId = this.m_serviceSettings.GetLicenseDescriptionId(requestContext, "TestManager");
      if (string.IsNullOrEmpty(licenseDescriptionId))
      {
        requestContext.Trace(1033723, TraceLevel.Error, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), "User {0}: Unexpected empty licenseDescriptionId as there is no description Id found for Test Manager Code {1}", (object) queryContext.UserIdentity.MasterId, (object) "TestManager");
        return (TestManagerRight) null;
      }
      if (!string.IsNullOrEmpty(this.m_serviceSettings.GetLicenseDescription(requestContext, licenseDescriptionId)))
        return TestManagerRight.Create("TestManager", queryContext.ProductVersion, queryContext.VisualStudioEdition, DateTimeOffset.MaxValue.AddSeconds(-1.0), string.Empty, string.Empty, "VSOSubscription", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSOSubscription"));
      requestContext.Trace(1033724, TraceLevel.Error, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), "User {0}: Unexpected empty licenseFallbackDescription as there is no licenseFallbackDescription found for id {1}", (object) queryContext.UserIdentity.MasterId, (object) licenseDescriptionId);
      return (TestManagerRight) null;
    }

    private VisualStudioOnlineServiceRight CreateServiceRight(IVssRequestContext requestContext) => VisualStudioOnlineServiceRight.Create(VisualStudioOnlineServiceLevel.Stakeholder, this.GetOffsetUtcNow().AddMonths(this.m_adapterSettings.LicenseDurationMonths));

    private static void ValidateQueryContext(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      if (queryContext.RequestType == LicensingRequestType.Client && queryContext.ProductVersion == (Version) null)
      {
        requestContext.Trace(1033720, TraceLevel.Error, "VisualStudio.Services.LicensingService", nameof (ExtensionLicensingAdapter), "Invalid product version.");
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductVersion());
      }
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.m_dateTimeProvider.UtcNow;

    [ExcludeFromCodeCoverage]
    internal virtual DateTimeOffset GetOffsetUtcNow() => new DateTimeOffset(this.m_dateTimeProvider.UtcNow);

    private static List<IUsageRight> CreateDefaultRights() => new List<IUsageRight>();

    private static IDictionary<string, string> CreateTestManagerRulesMap() => (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "EXT-MTM-0001",
        "ms.vss-testmanager-web"
      },
      {
        "VSO-ADVP",
        "ms.vss-testmanager-web"
      },
      {
        "EXT-PKM-00001",
        "ms.feed"
      }
    };
  }
}
