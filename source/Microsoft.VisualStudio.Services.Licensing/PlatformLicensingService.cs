// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.PlatformLicensingService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class PlatformLicensingService : ILicensingService, IVssFrameworkService
  {
    private readonly ILicensingAdapterFactory m_licensingAdapterFactory;
    private readonly ILicensingConfigurationManager m_settingsManager;
    private readonly ServiceFactory<IVssRegistryService> m_registryServiceFactory;
    private IVssDateTimeProvider m_dateTimeProvider;
    internal LicensingServiceConfiguration m_serviceSettings;
    protected CommandPropertiesSetter m_serviceCircuitBreakerSettings;
    protected X509Certificate2 m_certificate;
    private const int s_maxRightNameLength = 100;
    private const int s_maxClientRightsQueryContextProductEditionLength = 100;
    private const int s_maxClientRightsQueryContextReleaseTypeLength = 100;
    private const int s_maxClientRightsQueryContextCanaryLength = 2000;
    private const int s_maxClientRightsQueryContextMachineIdLength = 100;
    private static readonly Version s_envelopeVersion = new Version(1, 0, 0, 0);
    private const string s_circuitBreakerGroup = "Licensing";
    private const string s_circuitBreakerGetRights = "GetRights";
    private static readonly CommandPropertiesSetter s_defaultCircuitbreakerProperties = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithFallbackDisabled(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(50).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(10.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(60000).WithMetricsRollingStatisticalWindowBuckets(60);
    private const string s_area = "VisualStudio.Services.LicensingService";
    private const string s_layer = "BusinessLogic";
    private const string testManagerExtensionId = "ms.vss-testmanager-web";
    private const string packageManagementExtensionId = "ms.feed";
    private const string disableCommerceFeatureFlag = "VisualStudio.Services.Commerce.DisableOnPremCommerce";
    private const string s_getRightsFallback = "VisualStudio.LicensingService.GetRightsFallback";
    private const string s_getRightsCircuitBreaker = "VisualStudio.LicensingService.GetRightsCircuitBreaker";
    private const string s_platformLicensingShortCircuitExtensionRightsCheckFeatureFlag = "VisualStudio.Services.Licensing.PlatformShortCircuitExtensionRightsCheck";

    public PlatformLicensingService()
      : this((ILicensingAdapterFactory) new LicensingAdapterFactory(), (ILicensingConfigurationManager) new LicensingConfigurationRegistryManager(), (ServiceFactory<IVssRegistryService>) (x => (IVssRegistryService) x.GetService<CachedRegistryService>()), VssDateTimeProvider.DefaultProvider)
    {
    }

    internal PlatformLicensingService(
      ILicensingAdapterFactory licensingAdapterFactory,
      ILicensingConfigurationManager settingsManager,
      ServiceFactory<IVssRegistryService> registryServiceFactory,
      IVssDateTimeProvider dateTimeProvider)
    {
      this.m_licensingAdapterFactory = licensingAdapterFactory;
      this.m_settingsManager = settingsManager;
      this.m_registryServiceFactory = registryServiceFactory;
      this.m_dateTimeProvider = dateTimeProvider;
    }

    public virtual void ServiceStart(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<ILicensingAdapterFactory>(this.m_licensingAdapterFactory, "m_licensingAdapterFactory");
      requestContext.TraceEnter(1030080, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (ServiceStart));
      try
      {
        this.m_registryServiceFactory(requestContext).RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, "/Service/Licensing/...");
        requestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged), FrameworkServerConstants.ConfigurationSecretsDrawerName, (IEnumerable<string>) new string[1]
        {
          ServicingTokenConstants.LicensingServiceCertThumbprint
        });
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        this.PopulateSettings(requestContext);
        this.m_licensingAdapterFactory.Start(requestContext, this.m_serviceSettings, this.m_dateTimeProvider);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, ex.Message, ex);
        requestContext.TraceException(1030088, "VisualStudio.Services.LicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1030089, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (ServiceStart));
      }
    }

    public virtual void ServiceEnd(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<ILicensingAdapterFactory>(this.m_licensingAdapterFactory, "m_licensingAdapterFactory");
      this.m_licensingAdapterFactory.Stop(requestContext);
      this.m_registryServiceFactory(requestContext).UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      requestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged));
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(1030070, "VisualStudio.Services.LicensingService", "BusinessLogic", new object[1]
      {
        (object) changedEntries
      }, nameof (OnRegistryChanged));
      try
      {
        if (changedEntries == null || !changedEntries.Any<RegistryEntry>())
          return;
        this.PopulateSettings(requestContext);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, ex.Message, ex);
        requestContext.TraceException(1030078, "VisualStudio.Services.LicensingService", "BusinessLogic", ex);
      }
      finally
      {
        requestContext.TraceLeave(1030079, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (OnRegistryChanged));
      }
    }

    private void OnStrongBoxChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> strongBoxItemsChanged)
    {
      requestContext.TraceEnter(1030060, "VisualStudio.Services.LicensingService", "BusinessLogic", new object[1]
      {
        (object) strongBoxItemsChanged
      }, nameof (OnStrongBoxChanged));
      try
      {
        if (strongBoxItemsChanged == null || !strongBoxItemsChanged.Any<StrongBoxItemName>())
          return;
        this.InstantiateCertificate(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030068, "VisualStudio.Services.LicensingService", "BusinessLogic", ex);
      }
      finally
      {
        requestContext.TraceLeave(1030069, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (OnStrongBoxChanged));
      }
    }

    public virtual ClientRightsContainer GetClientRightsContainer(
      IVssRequestContext requestContext,
      ClientRightsQueryContext clientRightsQueryContext,
      ClientRightsTelemetryContext telemetryContext = null)
    {
      DateTime utcNow = this.GetUtcNow();
      PlatformLicensingService.ValidateDeploymentRequestContext(requestContext);
      requestContext.TraceEnter(1030030, "VisualStudio.Services.LicensingService", "BusinessLogic", new object[2]
      {
        (object) clientRightsQueryContext,
        (object) telemetryContext
      }, nameof (GetClientRightsContainer));
      try
      {
        this.ValidateClientRightsQueryContext(clientRightsQueryContext);
        IRightsQueryContext clientRightsContext = RightsQueryContext.CreateClientRightsContext(requestContext, clientRightsQueryContext, requestContext.GetUserIdentity());
        PlatformLicensingService.ValidateClientVersion(clientRightsContext);
        IList<IUsageRight> rights = this.GetRights(requestContext, clientRightsContext);
        IClientRightsEnvelope clientRightsEnvelope = this.CreateClientRightsEnvelope(requestContext, rights, clientRightsQueryContext, clientRightsContext);
        int milliseconds = (this.GetUtcNow() - utcNow).Milliseconds;
        string serializedClientRights = PlatformLicensingService.SerializeClientRights(clientRightsEnvelope.Rights);
        TelemetryHelper.StoreClientRightsTelemetry(requestContext, telemetryContext, clientRightsContext, serializedClientRights, milliseconds, "VisualStudio.Services.LicensingService", "BusinessLogic");
        return this.CreateClientRightsContainer(requestContext, clientRightsEnvelope, clientRightsQueryContext);
      }
      finally
      {
        requestContext.TraceLeave(1030039, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetClientRightsContainer));
      }
    }

    public virtual IList<IClientRight> GetMaxClientRights(
      IVssRequestContext requestContext,
      ClientRightsQueryContext clientRightsQueryContext)
    {
      PlatformLicensingService.ValidateDeploymentRequestContext(requestContext);
      requestContext.TraceEnter(1030050, "VisualStudio.Services.LicensingService", "BusinessLogic", new object[1]
      {
        (object) clientRightsQueryContext
      }, nameof (GetMaxClientRights));
      try
      {
        this.ValidateClientRightsQueryContext(clientRightsQueryContext);
        IRightsQueryContext clientRightsContext = RightsQueryContext.CreateMaxClientRightsContext(requestContext, clientRightsQueryContext, requestContext.GetUserIdentity());
        PlatformLicensingService.ValidateClientVersion(clientRightsContext);
        IList<IUsageRight> rights = this.GetRights(requestContext, clientRightsContext);
        return rights == null ? (IList<IClientRight>) new IClientRight[0] : PlatformLicensingService.TransformToClientRights((IEnumerable<IUsageRight>) rights);
      }
      finally
      {
        requestContext.TraceLeave(1030059, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetMaxClientRights));
      }
    }

    private byte[] GetCertificate(IVssRequestContext requestContext)
    {
      PlatformLicensingService.ValidateDeploymentRequestContext(requestContext);
      requestContext.TraceEnter(1030040, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetCertificate));
      try
      {
        return this.m_certificate.RawData;
      }
      finally
      {
        requestContext.TraceLeave(1030049, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetCertificate));
      }
    }

    public virtual License GetMsdnLicense(IVssRequestContext requestContext)
    {
      PlatformLicensingService.ValidateDeploymentRequestContext(requestContext);
      requestContext.TraceEnter(1039630, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetMsdnLicense));
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          requestContext.UserContext
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        return userIdentity == null ? License.None : MsdnLicensingAdapter.TranslateMsdnEntitlementsToMsdnLicense(this.GetAdapter<MsdnLicensingAdapter>().GetEntitlementsForIdentity(requestContext, userIdentity, new LicensingRequestType?(LicensingRequestType.All)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1039638, "VisualStudio.Services.LicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1039639, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetMsdnLicense));
      }
    }

    public virtual IList<MsdnEntitlement> GetMsdnEntitlements(IVssRequestContext requestContext)
    {
      PlatformLicensingService.ValidateDeploymentRequestContext(requestContext);
      requestContext.TraceEnter(1030200, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetMsdnEntitlements));
      try
      {
        MsdnLicensingAdapter adapter = this.GetAdapter<MsdnLicensingAdapter>();
        LicensingRequestType? nullable = new LicensingRequestType?();
        IVssRequestContext requestContext1 = requestContext;
        LicensingRequestType? requestType = nullable;
        return (IList<MsdnEntitlement>) adapter.GetEntitlements(requestContext1, requestType);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030208, "VisualStudio.Services.LicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1030209, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetMsdnEntitlements));
      }
    }

    public IDictionary<string, bool> GetExtensionRights(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds)
    {
      PlatformLicensingService.ValidateCollectionRequestContext(requestContext);
      ArgumentUtility.CheckForNull<IEnumerable<string>>(extensionIds, nameof (extensionIds));
      requestContext.TraceEnter(1030210, "VisualStudio.Services.LicensingService", "BusinessLogic", new object[1]
      {
        (object) extensionIds
      }, nameof (GetExtensionRights));
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableOnPremCommerce"))
        return this.GetExtensionRightsOnPrem(requestContext, extensionIds);
      if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
        return ExtensionLicensingUtil.GetExtensionRightsWhenDisabled(requestContext, extensionIds);
      try
      {
        ExtensionRightsResult extensionRights = this.GetExtensionRights(requestContext);
        if (extensionRights.ResultCode == ExtensionRightsResultCode.AllFree)
          return (IDictionary<string, bool>) extensionIds.ToDictionary<string, string, bool>((Func<string, string>) (k => k), (Func<string, bool>) (k => true));
        if (extensionRights.ResultCode == ExtensionRightsResultCode.FreeExtensionsFree)
        {
          IList<string> paidExtensionIds;
          IList<string> freeExtensionIds;
          this.DetermineFreeExtensions(requestContext, extensionIds, false, out paidExtensionIds, out freeExtensionIds);
          return this.FallbackExtensionRights((IEnumerable<string>) paidExtensionIds, (IEnumerable<string>) freeExtensionIds);
        }
        IList<string> paidExtensionIds1;
        IList<string> freeExtensionIds1;
        this.DetermineFreeExtensions(requestContext, extensionIds, true, out paidExtensionIds1, out freeExtensionIds1);
        Dictionary<string, bool> result = new Dictionary<string, bool>();
        foreach (string key in (IEnumerable<string>) paidExtensionIds1)
        {
          if (extensionRights.EntitledExtensions.Contains<string>(key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            result.Add(key, true);
          else
            result.Add(key, false);
        }
        freeExtensionIds1.ToList<string>().ForEach((Action<string>) (ex => result.Add(ex, true)));
        requestContext.TraceConditionally(1030217, TraceLevel.Info, "VisualStudio.Services.LicensingService", "BusinessLogic", (Func<string>) (() => string.Join(" | ", result.Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (ext => string.Format("{0}:{1}", (object) ext.Key, (object) ext.Value))))));
        return (IDictionary<string, bool>) result;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030218, "VisualStudio.Services.LicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1030219, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetExtensionRights));
      }
    }

    private IDictionary<string, bool> GetExtensionRightsOnPrem(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds)
    {
      PlatformLicensingService.ValidateCollectionRequestContext(requestContext);
      ArgumentUtility.CheckForNull<IEnumerable<string>>(extensionIds, nameof (extensionIds));
      requestContext.TraceEnter(1030210, "VisualStudio.Services.LicensingService", "BusinessLogic", new object[1]
      {
        (object) extensionIds
      }, nameof (GetExtensionRightsOnPrem));
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      License licenseForUser = requestContext.GetExtension<ILegacyLicensingHandler>().GetLicenseForUser(requestContext);
      Dictionary<string, bool> extensionRightsOnPrem = new Dictionary<string, bool>();
      foreach (string extensionId in extensionIds)
      {
        if (extensionId == "ms.vss-testmanager-web" && !LicensingExtensionEntitlementHelper.UsageRightsMatch(VisualStudioOnlineServiceLevel.Advanced, licenseForUser))
        {
          requestContext.Trace(1034300, TraceLevel.Info, "VisualStudio.Services.LicensingService", "BusinessLogic", string.Format("User {0} does not have {1} due to license {2}.", (object) userIdentity.MasterId, (object) extensionId, (object) licenseForUser));
          extensionRightsOnPrem.Add(extensionId, false);
        }
        else if (extensionId == "ms.feed" && !LicensingExtensionEntitlementHelper.UsageRightsMatch(VisualStudioOnlineServiceLevel.Express, licenseForUser))
        {
          requestContext.Trace(1034301, TraceLevel.Info, "VisualStudio.Services.LicensingService", "BusinessLogic", string.Format("User {0} does not have {1} due to license {2}.", (object) userIdentity.MasterId, (object) extensionId, (object) licenseForUser));
          extensionRightsOnPrem.Add(extensionId, false);
        }
        else if (extensionId != null)
        {
          requestContext.Trace(1034302, TraceLevel.Info, "VisualStudio.Services.LicensingService", "BusinessLogic", string.Format("User {0} has {1}.", (object) userIdentity.MasterId, (object) extensionId));
          extensionRightsOnPrem.Add(extensionId, true);
        }
      }
      return (IDictionary<string, bool>) extensionRightsOnPrem;
    }

    internal ExtensionRightsResult GetExtensionRights(IVssRequestContext requestContext)
    {
      PlatformLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1030225, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetExtensionRights));
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.PlatformShortCircuitExtensionRightsCheck"))
      {
        requestContext.Trace(1030226, TraceLevel.Info, "VisualStudio.Services.LicensingService", "BusinessLogic", "GetExtensionRights: Bypassing the computation of extension rights in PlatformLicensingService");
        return new ExtensionRightsResult()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          ResultCode = ExtensionRightsResultCode.AllFree,
          ReasonCode = ExtensionRightsReasonCode.FeatureFlagSet,
          Reason = "VisualStudio.Services.Licensing.PlatformShortCircuitExtensionRightsCheck"
        };
      }
      if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
      {
        IEnumerable<string> collection = ExtensionLicensingUtil.GetExtensionRightsWhenDisabled(requestContext).Where<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (pair => pair.Value)).Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (pair => pair.Key));
        return new ExtensionRightsResult()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          ReasonCode = ExtensionRightsReasonCode.Normal,
          ResultCode = ExtensionRightsResultCode.Normal,
          EntitledExtensions = new HashSet<string>(collection, (IEqualityComparer<string>) StringComparer.Ordinal)
        };
      }
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        Guid userId = userIdentity.EnterpriseStorageKey(requestContext);
        if (userIdentity == null)
          return new ExtensionRightsResult()
          {
            HostId = requestContext.ServiceHost.InstanceId,
            ResultCode = ExtensionRightsResultCode.FreeExtensionsFree,
            ReasonCode = ExtensionRightsReasonCode.NullIdentity
          };
        if (IdentityHelper.IsServiceIdentity(requestContext.To(TeamFoundationHostType.Deployment), (IReadOnlyVssIdentity) userIdentity) || IdentityHelper.IsWellKnownGroup(userIdentity.Descriptor, GroupWellKnownIdentityDescriptors.ServiceUsersGroup))
          return new ExtensionRightsResult()
          {
            HostId = requestContext.ServiceHost.InstanceId,
            ResultCode = ExtensionRightsResultCode.AllFree,
            ReasonCode = ExtensionRightsReasonCode.ServiceIdentity
          };
        IDictionary<string, LicensingSource> extensionsAssignedToUser = requestContext.GetService<IExtensionEntitlementService>().GetExtensionsAssignedToUser(requestContext.Elevate(), userId);
        return new ExtensionRightsResult()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          ReasonCode = ExtensionRightsReasonCode.Normal,
          ResultCode = ExtensionRightsResultCode.Normal,
          EntitledExtensions = new HashSet<string>((IEnumerable<string>) extensionsAssignedToUser.Keys, (IEqualityComparer<string>) StringComparer.Ordinal)
        };
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030227, "VisualStudio.Services.LicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1030230, "VisualStudio.Services.LicensingService", "BusinessLogic", nameof (GetExtensionRights));
      }
    }

    protected virtual void DetermineFreeExtensions(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds,
      bool evaluateUserLicense,
      out IList<string> paidExtensionIds,
      out IList<string> freeExtensionIds)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (this.IsConnectedOnPremisesServer(requestContext).Value && OfferMeters.TrySplitExtensionsToFreeAndPaid(requestContext, extensionIds, out paidExtensionIds, out freeExtensionIds))
          return;
        this.DetermineFreeExtensionsOffline(requestContext, extensionIds, evaluateUserLicense, out paidExtensionIds, out freeExtensionIds);
      }
      else
      {
        paidExtensionIds = (IList<string>) extensionIds.ToList<string>();
        freeExtensionIds = (IList<string>) new List<string>();
      }
    }

    private void DetermineFreeExtensionsOffline(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds,
      bool evaluateUserLicense,
      out IList<string> paidExtensionIds,
      out IList<string> freeExtensionIds)
    {
      bool firstPartyFree = false;
      if (evaluateUserLicense)
        firstPartyFree = requestContext.GetExtension<ILegacyLicensingHandler>().IsEnterpriseUser(requestContext);
      List<string> offlinePaidExtensionIds = requestContext.GetExtension<IOnPremiseOfflineExtensionHandler>().FilterPaidFirstPartyExtensions(requestContext, extensionIds, firstPartyFree);
      freeExtensionIds = (IList<string>) extensionIds.Where<string>((Func<string, bool>) (ext => !offlinePaidExtensionIds.Contains(ext))).ToList<string>();
      paidExtensionIds = (IList<string>) offlinePaidExtensionIds;
    }

    private bool? IsConnectedOnPremisesServer(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? new bool?(!requestContext.GetService<ICloudConnectedService>().GetConnectedAccountId(requestContext).Equals(Guid.Empty)) : new bool?();

    protected IList<IUsageRight> GetRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      if (queryContext.RequestType != LicensingRequestType.Client || !requestContext.IsFeatureEnabled("VisualStudio.LicensingService.GetRightsCircuitBreaker"))
        return this.GetRightsInternal(requestContext, queryContext);
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "LicensingAndAccounts.").AndCommandKey((CommandKey) nameof (GetRights)).AndCommandPropertiesDefaults(this.m_serviceCircuitBreakerSettings);
      return new CommandService<IList<IUsageRight>>(requestContext, setter, (Func<IList<IUsageRight>>) (() => this.GetRightsInternal(requestContext, queryContext)), (Func<IList<IUsageRight>>) (() => this.CreateFallbackRights(requestContext, queryContext))).Execute();
    }

    protected IList<IUsageRight> GetRightsInternal(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      requestContext.TraceEnter(1030120, "VisualStudio.Services.LicensingService", "BusinessLogic", new object[1]
      {
        (object) queryContext
      }, nameof (GetRightsInternal));
      if (queryContext.RequestType == LicensingRequestType.Client && requestContext.IsFeatureEnabled("VisualStudio.LicensingService.GetRightsFallback"))
        return this.CreateFallbackRights(requestContext, queryContext);
      if (queryContext.RequestType == LicensingRequestType.Client && queryContext.ReleaseType == ReleaseType.Prerelease)
        return this.CreatePrereleaseRights(requestContext, queryContext);
      IList<ILicensingAdapter> adapters = this.GetAdapters(queryContext);
      if (!adapters.Any<ILicensingAdapter>())
      {
        requestContext.Trace(1030121, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "Unrecognized right name: '{0}'.", (object) (queryContext.SingleRightName ?? string.Empty));
        return (IList<IUsageRight>) new List<IUsageRight>();
      }
      List<IUsageRight> usageRightList = new List<IUsageRight>();
      foreach (ILicensingAdapter adapter in (IEnumerable<ILicensingAdapter>) adapters)
      {
        IList<IUsageRight> adapterRights = PlatformLicensingService.GetAdapterRights(requestContext, adapter, queryContext);
        usageRightList.AddRange((IEnumerable<IUsageRight>) adapterRights);
        string str = string.Join(",", usageRightList.Select<IUsageRight, string>((Func<IUsageRight, string>) (_ => _.Name)));
        requestContext.Trace(1030122, TraceLevel.Info, "VisualStudio.Services.LicensingService", "BusinessLogic", "GetRightsInternal: Calling adapter {0}, returned rights {1}", (object) adapter.GetType().ToString(), (object) str);
      }
      if (queryContext.RequestType == LicensingRequestType.Client && !string.IsNullOrWhiteSpace(queryContext.MachineId) && !usageRightList.Any<IUsageRight>())
      {
        VisualStudioTrialLicensingAdapter adapter = this.m_licensingAdapterFactory.GetAdapter<VisualStudioTrialLicensingAdapter>();
        IList<IUsageRight> adapterRights = PlatformLicensingService.GetAdapterRights(requestContext, (ILicensingAdapter) adapter, queryContext);
        usageRightList.AddRange((IEnumerable<IUsageRight>) adapterRights);
      }
      List<IUsageRight> source = PlatformLicensingService.AggregateRights(requestContext, usageRightList);
      string str1 = string.Join(",", source.Select<IUsageRight, string>((Func<IUsageRight, string>) (_ => _.Name)));
      requestContext.Trace(1030123, TraceLevel.Info, "VisualStudio.Services.LicensingService", "BusinessLogic", "GetRightsInternal: after aggregating returning rights {0}", (object) str1);
      return (IList<IUsageRight>) source;
    }

    private IList<ILicensingAdapter> GetAdapters(IRightsQueryContext queryContext) => queryContext.RequestType == LicensingRequestType.Client ? (queryContext.SingleRightName != null ? this.m_licensingAdapterFactory.GetClientAdapters(queryContext.SingleRightName) : this.m_licensingAdapterFactory.GetClientAdapters()) : (queryContext.SingleRightName != null ? this.m_licensingAdapterFactory.GetServiceAdapters(queryContext.SingleRightName) : this.m_licensingAdapterFactory.GetServiceAdapters());

    internal static IList<IUsageRight> GetAdapterRights(
      IVssRequestContext requestContext,
      ILicensingAdapter adapter,
      IRightsQueryContext queryContext)
    {
      IList<IUsageRight> usageRightList = (IList<IUsageRight>) null;
      try
      {
        usageRightList = adapter.GetRights(requestContext, queryContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030059, "VisualStudio.Services.LicensingService", "BusinessLogic", ex);
      }
      return usageRightList ?? (IList<IUsageRight>) new List<IUsageRight>();
    }

    private IDictionary<string, bool> FallbackExtensionRights(
      IEnumerable<string> paidExtensionIds,
      IEnumerable<string> freeExtensionIds)
    {
      Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
      if (paidExtensionIds != null)
      {
        foreach (string paidExtensionId in paidExtensionIds)
          dictionary.Add(paidExtensionId, false);
      }
      if (freeExtensionIds != null)
      {
        foreach (string freeExtensionId in freeExtensionIds)
          dictionary.Add(freeExtensionId, true);
      }
      return (IDictionary<string, bool>) dictionary;
    }

    private IList<IUsageRight> CreateFallbackRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      DateTimeOffset expirationDate = this.GetOffsetUtcNow().AddDays(7.0);
      if (LicensingComparers.RightNameComparer.Compare("VisualStudio", queryContext.SingleRightName) == 0 || LicensingComparers.RightNameComparer.Compare("VSonMac", queryContext.SingleRightName) == 0)
      {
        if (queryContext.VisualStudioEdition == VisualStudioEdition.Unspecified)
        {
          requestContext.Trace(1030100, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "Unexpected VisualStudioEdition: '{0}'.", (object) queryContext.VisualStudioEdition);
          return (IList<IUsageRight>) new List<IUsageRight>();
        }
        VisualStudioRight visualStudioRight = VisualStudioRight.Create(VisualStudioRight.MapProductFamilyToRightName(queryContext.VisualStudioFamily), queryContext.ProductVersion, queryContext.VisualStudioEdition, expirationDate, this.m_serviceSettings.GetLicensingSourceName(requestContext, LicensingSource.Profile), (string) null, "VSExtensionTrial", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSExtensionTrial"), new Dictionary<string, object>()
        {
          {
            "IsTrialLicense",
            (object) true
          },
          {
            "SubscriptionLevel",
            (object) "Enterprise"
          }
        });
        requestContext.Trace(1030103, TraceLevel.Info, "VisualStudio.Services.LicensingService", "BusinessLogic", string.Format("Fallback to 7 days {0} trial right", (object) "VisualStudio"));
        return (IList<IUsageRight>) new List<IUsageRight>()
        {
          (IUsageRight) visualStudioRight
        };
      }
      string name;
      if (VisualStudioExpressLicensingAdapter.ExpressEditions.TryGetValue(queryContext.SingleRightName, out name))
      {
        VisualStudioExpressRight studioExpressRight = VisualStudioExpressRight.Create(name, queryContext.ProductVersion, VisualStudioEdition.Unspecified, expirationDate, this.m_serviceSettings.GetLicensingSourceName(requestContext, LicensingSource.Profile), (string) null, "VSExtensionTrial", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSExtensionTrial"), new Dictionary<string, object>()
        {
          {
            "IsTrialLicense",
            (object) true
          },
          {
            "SubscriptionLevel",
            (object) "Enterprise"
          }
        });
        requestContext.Trace(1030106, TraceLevel.Info, "VisualStudio.Services.LicensingService", "BusinessLogic", string.Format("Fallback to 7 Days {0} trial right", (object) name));
        return (IList<IUsageRight>) new List<IUsageRight>()
        {
          (IUsageRight) studioExpressRight
        };
      }
      requestContext.Trace(1030107, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "Unrecognized right name: '{0}'.", (object) (queryContext.SingleRightName ?? string.Empty));
      return (IList<IUsageRight>) new List<IUsageRight>();
    }

    private IList<IUsageRight> CreatePrereleaseRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext)
    {
      IPlatformClientLicensingService service = requestContext.GetService<IPlatformClientLicensingService>();
      DateTimeOffset expirationDate;
      switch (queryContext.ProductVersion.Major)
      {
        case 12:
          expirationDate = this.m_serviceSettings.PrereleaseExpirationDate12;
          break;
        case 14:
          expirationDate = service.GetClientReleaseExpirationDate(requestContext, queryContext, this.m_serviceSettings.PrereleaseExpirationDate14, ClientReleaseType.Preview);
          break;
        case 15:
          expirationDate = service.GetClientReleaseExpirationDate(requestContext, queryContext, this.m_serviceSettings.PrereleaseExpirationDate15, ClientReleaseType.Preview);
          break;
        case 16:
          expirationDate = service.GetClientReleaseExpirationDate(requestContext, queryContext, this.m_serviceSettings.PrereleaseExpirationDate16, ClientReleaseType.Preview);
          break;
        default:
          expirationDate = service.GetClientReleaseExpirationDate(requestContext, queryContext, this.m_serviceSettings.PrereleaseExpirationDate12, ClientReleaseType.Preview);
          break;
      }
      if (LicensingComparers.RightNameComparer.Compare("VisualStudio", queryContext.SingleRightName) == 0)
      {
        if (queryContext.VisualStudioEdition == VisualStudioEdition.Unspecified)
        {
          requestContext.Trace(1030065, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "Unexpected VisualStudioEdition: '{0}'.", (object) queryContext.VisualStudioEdition);
          return (IList<IUsageRight>) new List<IUsageRight>();
        }
        return (IList<IUsageRight>) new List<IUsageRight>()
        {
          (IUsageRight) VisualStudioRight.Create("VisualStudio", queryContext.ProductVersion, queryContext.VisualStudioEdition, expirationDate, this.m_serviceSettings.GetLicensingSourceName(requestContext, LicensingSource.Profile), (string) null, "VSPrerelease", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSPrerelease"))
        };
      }
      if (LicensingComparers.RightNameComparer.Compare("TestManager", queryContext.SingleRightName) == 0)
        return (IList<IUsageRight>) new List<IUsageRight>()
        {
          (IUsageRight) TestManagerRight.Create("TestManager", queryContext.ProductVersion, VisualStudioEdition.Unspecified, expirationDate, this.m_serviceSettings.GetLicensingSourceName(requestContext, LicensingSource.Profile), (string) null, "MTMPrerelease", this.m_serviceSettings.GetLicenseDescription(requestContext, "MTMPrerelease"))
        };
      string name;
      if (VisualStudioExpressLicensingAdapter.ExpressEditions.TryGetValue(queryContext.SingleRightName, out name))
        return (IList<IUsageRight>) new List<IUsageRight>()
        {
          (IUsageRight) VisualStudioExpressRight.Create(name, queryContext.ProductVersion, VisualStudioEdition.Unspecified, expirationDate, this.m_serviceSettings.GetLicensingSourceName(requestContext, LicensingSource.Profile), (string) null, "VSPrerelease", this.m_serviceSettings.GetLicenseDescription(requestContext, "VSPrerelease"))
        };
      requestContext.Trace(1030066, TraceLevel.Warning, "VisualStudio.Services.LicensingService", "BusinessLogic", "Unrecognized right name: '{0}'.", (object) (queryContext.SingleRightName ?? string.Empty));
      return (IList<IUsageRight>) new List<IUsageRight>();
    }

    private static List<IUsageRight> AggregateRights(
      IVssRequestContext requestContext,
      List<IUsageRight> rights)
    {
      if (rights == null || rights.Count <= 1)
        return rights;
      Dictionary<Type, IComparable> dictionary = new Dictionary<Type, IComparable>();
      foreach (IUsageRight right in rights)
      {
        if (!(right is IComparable comparable1))
        {
          requestContext.Trace(1030090, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", "Type '{0}' doesn't implement required IComparable interface.", (object) right.GetType().Name);
        }
        else
        {
          IComparable comparable;
          if (dictionary.TryGetValue(comparable1.GetType(), out comparable))
          {
            if (0 > comparable.CompareTo((object) comparable1))
              dictionary[right.GetType()] = comparable1;
          }
          else
            dictionary.Add(right.GetType(), comparable1);
        }
      }
      return dictionary.Values.Select<IComparable, IUsageRight>((Func<IComparable, IUsageRight>) (item => item as IUsageRight)).ToList<IUsageRight>();
    }

    private static void ValidateClientVersion(IRightsQueryContext queryContext)
    {
      ArgumentUtility.CheckForNull<Version>(queryContext.ProductVersion, "clientVersion");
      if (queryContext.VisualStudioFamily == VisualStudioFamily.VisualStudioEmulatorAndroid)
        return;
      if (queryContext.ProductVersion.Major < 12)
        throw new InvalidClientVersionException();
      if (queryContext.VisualStudioFamily == VisualStudioFamily.TestManager && queryContext.ProductVersion.Major < 14)
        throw new InvalidClientVersionException();
    }

    private static void ValidateRightName(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      if (name.Length > 100)
        throw new InvalidRightNameException(name);
      if (ArgumentUtility.IsInvalidString(name))
        throw new InvalidRightNameException(name);
    }

    private static void ValidateDeploymentRequestContext(IVssRequestContext requestContext) => requestContext.CheckDeploymentRequestContext();

    private static void ValidateCollectionRequestContext(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    private void ValidateClientRightsQueryContext(ClientRightsQueryContext queryContext)
    {
      ArgumentUtility.CheckForNull<ClientRightsQueryContext>(queryContext, nameof (queryContext));
      PlatformLicensingService.ValidateRightName(queryContext.ProductFamily);
      if (string.IsNullOrWhiteSpace(queryContext.ProductVersion))
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductVersion());
      if (!string.IsNullOrEmpty(queryContext.ProductEdition))
      {
        if (queryContext.ProductEdition.Length > 100)
          throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductEdition());
        if (ArgumentUtility.IsInvalidString(queryContext.ProductEdition))
          throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductEdition());
      }
      if (!string.IsNullOrEmpty(queryContext.ReleaseType))
      {
        if (queryContext.ReleaseType.Length > 100)
          throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextReleaseType());
        if (ArgumentUtility.IsInvalidString(queryContext.ReleaseType))
          throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextReleaseType());
      }
      if (!string.IsNullOrEmpty(queryContext.Canary))
      {
        if (queryContext.Canary.Length > 2000)
          throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextCanary());
        if (ArgumentUtility.IsInvalidString(queryContext.Canary))
          throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextCanary());
      }
      if (string.IsNullOrEmpty(queryContext.MachineId))
        return;
      if (queryContext.MachineId.Length > 100)
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextMachineId());
      if (ArgumentUtility.IsInvalidString(queryContext.MachineId))
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextMachineId());
    }

    private void PopulateSettings(IVssRequestContext requestContext)
    {
      this.m_serviceSettings = this.m_settingsManager.GetLicensingServiceConfiguration(requestContext);
      this.m_serviceCircuitBreakerSettings = PlatformLicensingService.s_defaultCircuitbreakerProperties;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.EnableEntitlementV3Api"))
        return;
      this.InstantiateCertificate(requestContext);
    }

    private void InstantiateCertificate(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.IsLps())
        return;
      this.m_certificate = CommonUtil.GetCertificateFromConfigurationSecretsDrawerInStrongBox(requestContext, ServicingTokenConstants.LicensingServiceCertThumbprint);
      if (this.m_certificate == null)
        throw new LicensingCertificateException("Licensing certificate with lookupKey " + ServicingTokenConstants.LicensingServiceCertThumbprint + " not found");
    }

    internal IClientRightsEnvelope CreateClientRightsEnvelope(
      IVssRequestContext requestContext,
      IList<IUsageRight> rights,
      ClientRightsQueryContext clientRightsQueryContext,
      IRightsQueryContext queryContext)
    {
      ArgumentUtility.CheckForNull<IList<IUsageRight>>(rights, nameof (rights));
      ArgumentUtility.CheckForNull<ClientRightsQueryContext>(clientRightsQueryContext, nameof (clientRightsQueryContext));
      IList<IClientRight> clientRights = PlatformLicensingService.TransformToClientRights((IEnumerable<IUsageRight>) rights);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return (IClientRightsEnvelope) new ClientRightsEnvelope(clientRights)
      {
        EnvelopeVersion = PlatformLicensingService.s_envelopeVersion,
        CreationDate = DateTimeOffset.Now,
        ExpirationDate = this.CalculateClientRightsEnvelopeExpiration(requestContext, clientRights, queryContext),
        RefreshInterval = this.CalculateClientRightsEnvelopeRefreshInterval(clientRights, queryContext),
        ActivityId = requestContext.ActivityId,
        UserId = userIdentity.MasterId,
        UserName = IdentityHelper.GetUniqueName(userIdentity),
        Canary = clientRightsQueryContext.Canary
      };
    }

    private DateTimeOffset CalculateClientRightsEnvelopeExpiration(
      IVssRequestContext requestContext,
      IList<IClientRight> rights,
      IRightsQueryContext rightsQueryContext)
    {
      int expirationOffsetSeconds;
      if (rights.All<IClientRight>((Func<IClientRight, bool>) (right => right is VisualStudioExpressRight)))
      {
        switch (rightsQueryContext.ReleaseType)
        {
          case ReleaseType.Release:
            expirationOffsetSeconds = this.m_serviceSettings.ExpressRightsReleaseEnvelopeExpirationOffsetSeconds;
            break;
          case ReleaseType.Prerelease:
            Version productVersion = rightsQueryContext.ProductVersion;
            string productVersionBuildLab = rightsQueryContext.ProductVersionBuildLab;
            switch (requestContext.GetService<IPlatformClientLicensingService>().GetClientReleaseType(requestContext, productVersion, productVersionBuildLab))
            {
              case ClientReleaseType.Preview:
                expirationOffsetSeconds = this.m_serviceSettings.ExpressRightsPrereleaseEnvelopeExpirationOffsetSeconds;
                break;
              case ClientReleaseType.RC:
              case ClientReleaseType.RTM:
                expirationOffsetSeconds = this.m_serviceSettings.ExpressRightsReleaseEnvelopeExpirationOffsetSeconds;
                break;
              default:
                expirationOffsetSeconds = this.m_serviceSettings.ExpressRightsPrereleaseEnvelopeExpirationOffsetSeconds;
                break;
            }
            break;
          default:
            expirationOffsetSeconds = this.m_serviceSettings.ExpressRightsPrereleaseEnvelopeExpirationOffsetSeconds;
            break;
        }
      }
      else if (rights.All<IClientRight>((Func<IClientRight, bool>) (right => right is VisualStudioRight && ((VisualStudioRight) right).Edition == VisualStudioEdition.Community)))
      {
        int releaseType = (int) rightsQueryContext.ReleaseType;
        expirationOffsetSeconds = this.m_serviceSettings.CommunityRightsReleaseEnvelopeExpirationOffsetSeconds;
      }
      else if (rights.All<IClientRight>((Func<IClientRight, bool>) (right => right is VisualStudioEmulatorAndroidRight && ((ClientRightBase) right).Name == "VisualStudioEmulatorAndroid")))
      {
        int releaseType = (int) rightsQueryContext.ReleaseType;
        expirationOffsetSeconds = this.m_serviceSettings.EmulatorAndroidRightsReleaseEnvelopeExpirationOffsetSeconds;
      }
      else if (rights.All<IClientRight>((Func<IClientRight, bool>) (right => right is VisualStudioRight && string.Equals(right.LicenseDescriptionId, "VSExtensionTrial", StringComparison.OrdinalIgnoreCase))) || rights.All<IClientRight>((Func<IClientRight, bool>) (right => right is TestManagerRight && string.Equals(right.LicenseDescriptionId, "VSExtensionTrial", StringComparison.OrdinalIgnoreCase))))
      {
        if (rightsQueryContext.ReleaseType != ReleaseType.Release)
          expirationOffsetSeconds = this.m_serviceSettings.NonExpressRightsEnvelopeExpirationOffsetSeconds;
        else
          expirationOffsetSeconds = this.m_serviceSettings.TrialRightsReleaseEnvelopeExpirationOffsetSeconds;
      }
      else
        expirationOffsetSeconds = this.m_serviceSettings.NonExpressRightsEnvelopeExpirationOffsetSeconds;
      return expirationOffsetSeconds <= 0 ? DateTimeOffset.MaxValue : DateTimeOffset.Now.AddSeconds((double) expirationOffsetSeconds);
    }

    private TimeSpan CalculateClientRightsEnvelopeRefreshInterval(
      IList<IClientRight> rights,
      IRightsQueryContext rightsQueryContext)
    {
      int refreshIntervalSeconds;
      if (rights.All<IClientRight>((Func<IClientRight, bool>) (right => right is VisualStudioExpressRight)))
      {
        if (rightsQueryContext.ReleaseType != ReleaseType.Release)
          refreshIntervalSeconds = this.m_serviceSettings.ExpressRightsPrereleaseEnvelopeRefreshIntervalSeconds;
        else
          refreshIntervalSeconds = this.m_serviceSettings.ExpressRightsReleaseEnvelopeRefreshIntervalSeconds;
      }
      else if (rights.All<IClientRight>((Func<IClientRight, bool>) (right => right is VisualStudioRight && ((VisualStudioRight) right).Edition == VisualStudioEdition.Community)))
      {
        if (rightsQueryContext.ReleaseType != ReleaseType.Release)
          refreshIntervalSeconds = this.m_serviceSettings.ExpressRightsPrereleaseEnvelopeRefreshIntervalSeconds;
        else
          refreshIntervalSeconds = this.m_serviceSettings.ExpressRightsReleaseEnvelopeRefreshIntervalSeconds;
      }
      else if (rights.All<IClientRight>((Func<IClientRight, bool>) (right => right is VisualStudioEmulatorAndroidRight && ((ClientRightBase) right).Name == "VisualStudioEmulatorAndroid")))
      {
        if (rightsQueryContext.ReleaseType != ReleaseType.Release)
          refreshIntervalSeconds = this.m_serviceSettings.ExpressRightsPrereleaseEnvelopeRefreshIntervalSeconds;
        else
          refreshIntervalSeconds = this.m_serviceSettings.ExpressRightsReleaseEnvelopeRefreshIntervalSeconds;
      }
      else if (rights.All<IClientRight>((Func<IClientRight, bool>) (right => right is VisualStudioRight && string.Equals(right.LicenseDescriptionId, "VSExtensionTrial", StringComparison.OrdinalIgnoreCase))))
      {
        if (rightsQueryContext.ReleaseType != ReleaseType.Release)
          refreshIntervalSeconds = this.m_serviceSettings.NonExpressRightsEnvelopeRefreshIntervalSeconds;
        else
          refreshIntervalSeconds = this.m_serviceSettings.TrialRightsReleaseEnvelopeRefreshIntervalSeconds;
      }
      else if (rights.All<IClientRight>((Func<IClientRight, bool>) (right => right is TestManagerRight)))
      {
        if (rightsQueryContext.ReleaseType != ReleaseType.Release)
          refreshIntervalSeconds = this.m_serviceSettings.NonExpressRightsEnvelopeRefreshIntervalSeconds;
        else
          refreshIntervalSeconds = this.m_serviceSettings.TrialRightsReleaseEnvelopeRefreshIntervalSeconds;
      }
      else
        refreshIntervalSeconds = this.m_serviceSettings.NonExpressRightsEnvelopeRefreshIntervalSeconds;
      return TimeSpan.FromSeconds((double) refreshIntervalSeconds);
    }

    protected virtual ClientRightsContainer CreateClientRightsContainer(
      IVssRequestContext requestContext,
      IClientRightsEnvelope envelope,
      ClientRightsQueryContext clientRightsQueryContext)
    {
      ArgumentUtility.CheckForNull<IClientRightsEnvelope>(envelope, nameof (envelope));
      string str = PlatformLicensingService.SerializeClientRights(envelope.Rights);
      string envelopeIssuer = this.m_serviceSettings.EnvelopeIssuer;
      string appliesToAddress = this.m_serviceSettings.EnvelopeAppliesToAddress;
      DateTimeOffset dateTimeOffset = envelope.CreationDate;
      DateTime utcDateTime1 = dateTimeOffset.UtcDateTime;
      dateTimeOffset = envelope.ExpirationDate;
      DateTime utcDateTime2 = dateTimeOffset.UtcDateTime;
      Claim[] additionalClaims = new Claim[7]
      {
        new Claim("EnvelopeVersion", envelope.EnvelopeVersion.ToString()),
        new Claim("UserId", envelope.UserId.ToString()),
        new Claim("UserName", envelope.UserName),
        new Claim("Rights", str),
        new Claim("PollS", ((int) envelope.RefreshInterval.TotalSeconds).ToString()),
        new Claim("ActivityId", envelope.ActivityId.ToString()),
        new Claim("Canary", envelope.Canary ?? string.Empty)
      };
      VssSigningCredentials credentials = VssSigningCredentials.Create(this.m_certificate);
      string encodedToken = JsonWebToken.Create(envelopeIssuer, appliesToAddress, utcDateTime1, utcDateTime2, (IEnumerable<Claim>) additionalClaims, credentials).EncodedToken;
      byte[] certificate = clientRightsQueryContext.IncludeCertificate ? this.GetCertificate(requestContext) : (byte[]) null;
      return new ClientRightsContainer()
      {
        Token = encodedToken,
        CertificateBytes = certificate
      };
    }

    private static IList<IClientRight> TransformToClientRights(IEnumerable<IUsageRight> rights) => (IList<IClientRight>) rights.Where<IUsageRight>((Func<IUsageRight, bool>) (right => right is ClientRightBase)).Select<IUsageRight, IClientRight>((Func<IUsageRight, IClientRight>) (right => (IClientRight) (right as ClientRightBase))).Where<IClientRight>((Func<IClientRight, bool>) (right => right != null)).ToList<IClientRight>();

    private static IList<IServiceRight> TransformToServiceRights(IEnumerable<IUsageRight> rights) => (IList<IServiceRight>) rights.Where<IUsageRight>((Func<IUsageRight, bool>) (right => right is ServiceRightBase)).Select<IUsageRight, IServiceRight>((Func<IUsageRight, IServiceRight>) (right => (IServiceRight) (right as ServiceRightBase))).Where<IServiceRight>((Func<IServiceRight, bool>) (right => right != null)).ToList<IServiceRight>();

    private static string SerializeClientRights(IList<IClientRight> clientRights) => JArray.FromObject((object) clientRights).ToString(Formatting.None);

    internal virtual T GetAdapter<T>() where T : ILicensingAdapter => this.m_licensingAdapterFactory.GetAdapter<T>();

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.m_dateTimeProvider.UtcNow;

    [ExcludeFromCodeCoverage]
    internal virtual DateTimeOffset GetOffsetUtcNow() => new DateTimeOffset(this.m_dateTimeProvider.UtcNow);

    internal void SetDateTimeProvider(IVssDateTimeProvider dateTimeProvider) => this.m_dateTimeProvider = dateTimeProvider;
  }
}
