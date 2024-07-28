// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.MsdnLicensingAdapter
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class MsdnLicensingAdapter : ILicensingAdapter
  {
    protected ILicensingConfigurationManager m_settingsManager;
    private readonly ServiceFactory<IVssRegistryService> m_registryServiceFactory;
    protected LicensingServiceConfiguration m_serviceSettings;
    protected MsdnAdapterConfiguration m_adapterSettings;
    private static readonly Dictionary<MsdnEntitlementCodeTuple, MsdnLicense> s_MsdnEntitlementCodeTupleToMsdnLicenseMap = MsdnLicensingAdapter.CreateMsdnEntitlementCodeTupleToMsdnLicenseMap();
    private static readonly Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> s_MsdnVsEditionMap = MsdnLicensingAdapter.CreateMsdnVsEditionMap();
    private static readonly Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> s_MsdnVsMacEditionMap = MsdnLicensingAdapter.CreateMsdnVsMacEditionMap();
    private static readonly Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> s_MsdnTestManagerMap = MsdnLicensingAdapter.CreateMsdnTestManagerMap();
    private static readonly Dictionary<string, int> s_VsWeightageRule = MsdnLicensingAdapter.CreateVsWeightageRule();
    private static readonly Dictionary<string, VisualStudioOnlineServiceLevel> s_MsdnServiceLevelMap = MsdnLicensingAdapter.CreateMsdnServiceLevelMap();
    internal const string s_MsdnPlatformSubscriptionLevelCode = "MDN-SDG-000001";
    internal const string s_ClientLicensingEntitlementType = "LicensingIde";
    internal const string s_ServiceLicensingEntitlementType = "LicensingVso";
    internal const string s_LicensingExtensionEntitlementType = "VSTSExtension";
    private const string s_SubscriptionStatusActive = "Active";
    private const string s_SubscriptionStatusGrace = "Grace";
    private const string s_mtmExtensionEntitlementCode = "EXT-MTM-0001";
    private const string s_vsoAdvancedLicense = "VSO-ADVP";
    private const string s_dontUseCircuitBreakerForGettingMsdnEntitlementsFeatureFlag = "VisualStudio.Services.Licensing.MsdnLicensingAdapter.GetMsdnEntitlements.CircuitBreaker.Disable";
    private const string s_useMsdnLicensingMockFeatureName = "VisualStudio.Services.LicensingService.UseMockMsdnEntitlement";
    private const string s_BypassMsdnFallbackWithLastKnownLicense = "VisualStudio.Services.LicensingService.BypassMsdnFallbackWithLastKnownLicense";
    private const string s_area = "VisualStudio.Services.LicensingService.MsdnLicensingAdapter";
    private const string s_layer = "BusinessLogic";
    private ILockName m_lockName;
    private IVssDateTimeProvider m_dateTimeProvider;

    public MsdnLicensingAdapter()
      : this((ILicensingConfigurationManager) new LicensingConfigurationRegistryManager(), (ServiceFactory<IVssRegistryService>) (x => (IVssRegistryService) x.GetService<CachedRegistryService>()), VssDateTimeProvider.DefaultProvider)
    {
    }

    public MsdnLicensingAdapter(
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
      requestContext.TraceEnter(1031100, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (Start));
      this.m_dateTimeProvider = dateTimeProvider;
      try
      {
        this.m_registryServiceFactory(requestContext).RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, "/Service/Licensing/Msdn/...");
        this.m_serviceSettings = serviceSettings;
        this.m_lockName = requestContext.ServiceHost.CreateUniqueLockName("MsdnLicensingAdapterRequestHandlerLock");
        this.PopulateSettings(requestContext);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, ex.Message, ex);
        requestContext.TraceException(1031108, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1031109, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (Start));
      }
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
      requestContext.TraceEnter(1031000, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (GetRights));
      try
      {
        List<MsdnEntitlement> entitlements = this.GetEntitlements(requestContext, new LicensingRequestType?(queryContext.RequestType));
        List<IUsageRight> rightsInternal = this.GetRightsInternal(requestContext, queryContext, entitlements);
        requestContext.TraceProperties<List<IUsageRight>>(1031007, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", rightsInternal, (string) null);
        return (IList<IUsageRight>) rightsInternal;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1031008, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", ex);
        return (IList<IUsageRight>) MsdnLicensingAdapter.CreateDefaultRights();
      }
      finally
      {
        requestContext.TraceLeave(1031009, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (GetRights));
      }
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(1031010, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (OnRegistryChanged));
      try
      {
        if (changedEntries == null || !changedEntries.Any<RegistryEntry>())
          return;
        this.PopulateSettings(requestContext);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, ex.Message, ex);
        requestContext.TraceException(1031018, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", ex);
      }
      finally
      {
        requestContext.TraceLeave(1031019, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (OnRegistryChanged));
      }
    }

    public virtual List<MsdnEntitlement> GetEntitlements(
      IVssRequestContext requestContext,
      LicensingRequestType? requestType,
      License license = null)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return this.GetEntitlementsForIdentity(requestContext, userIdentity, requestType, license);
    }

    public List<MsdnEntitlement> GetEntitlementsForIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      LicensingRequestType? requestType,
      License license = null,
      bool dontUseFallback = false)
    {
      requestContext.TraceEnter(1031030, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (GetEntitlementsForIdentity));
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment || requestContext.IsFeatureEnabled("VisualStudio.Services.LicensingService.UseMockMsdnEntitlement"))
        return new MockMsdnEntitlement().GetEntitlementsForIdentity(requestContext, userIdentity);
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.MsdnLicensingAdapter.GetMsdnEntitlements.CircuitBreaker.Disable"))
        return this.GetEntitlementsInternal(requestContext, requestType, userIdentity);
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "LicensingAndAccounts.").AndCommandKey((CommandKey) this.m_adapterSettings.CircuitBreakerSettingsForGetMsdnEntitlements.CommandKeyForGetMsdnEntitlements).AndCommandPropertiesDefaults(this.m_adapterSettings.CircuitBreakerSettingsForGetMsdnEntitlements.CircuitBreakerSettingsForGetMsdnEntitlements);
      List<MsdnEntitlement> msdnEntitlements = new CommandService<List<MsdnEntitlement>>(requestContext, setter, (Func<List<MsdnEntitlement>>) (() => this.GetEntitlementsInternal(requestContext, requestType, userIdentity)), (Func<List<MsdnEntitlement>>) (() => this.GetMaxEntitlements(requestContext, requestType, license, dontUseFallback))).Execute();
      if (msdnEntitlements == null)
        throw new MsdnServiceUnavailableException(string.Format("Check CircuitBreaker '{0}' GroupKey health report.", (object) this.m_adapterSettings.CircuitBreakerSettingsForGetMsdnEntitlements.CommandGroupKey));
      requestContext.TraceConditionally(1031038, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", (Func<string>) (() =>
      {
        string str = msdnEntitlements.IsNullOrEmpty<MsdnEntitlement>() ? string.Empty : string.Join(",", msdnEntitlements.Select<MsdnEntitlement, string>((Func<MsdnEntitlement, string>) (x => x != null ? x.EntitlementCode ?? string.Empty : string.Empty)));
        return string.Format("EntitlementCodes for user:{0} from MSDN were: {1}", (object) userIdentity.Id.ToString(), (object) str);
      }));
      requestContext.TraceLeave(1031039, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (GetEntitlementsForIdentity));
      return msdnEntitlements;
    }

    protected List<MsdnEntitlement> GetMaxEntitlements(
      IVssRequestContext requestContext,
      LicensingRequestType? requestType,
      License license,
      bool dontUseFallback = false)
    {
      requestContext.TraceEnter(1031031, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (GetMaxEntitlements));
      List<MsdnEntitlement> maxEntitlements = new List<MsdnEntitlement>();
      if (dontUseFallback)
        return maxEntitlements;
      try
      {
        MsdnEntitlementCodeTuple entitlementCodeTuple = (MsdnEntitlementCodeTuple) null;
        if (license != (License) null && !requestContext.IsFeatureEnabled("VisualStudio.Services.LicensingService.BypassMsdnFallbackWithLastKnownLicense"))
        {
          requestContext.Trace(1031032, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", string.Format("GetMaxEntitlements: using last known license {0} for user {1}", (object) license.ToString(), (object) requestContext.GetUserId()));
          IEnumerable<KeyValuePair<MsdnEntitlementCodeTuple, MsdnLicense>> source = MsdnLicensingAdapter.s_MsdnEntitlementCodeTupleToMsdnLicenseMap.Where<KeyValuePair<MsdnEntitlementCodeTuple, MsdnLicense>>((Func<KeyValuePair<MsdnEntitlementCodeTuple, MsdnLicense>, bool>) (_ => (License) _.Value == license));
          if (source.Any<KeyValuePair<MsdnEntitlementCodeTuple, MsdnLicense>>())
            entitlementCodeTuple = source.First<KeyValuePair<MsdnEntitlementCodeTuple, MsdnLicense>>().Key;
        }
        string str = entitlementCodeTuple != null ? string.Format("({0},{1})", (object) entitlementCodeTuple.Item1, (object) entitlementCodeTuple.Item2) : "null";
        requestContext.Trace(1031033, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", string.Format("GetMaxEntitlements: last known entitlement is {0} for user {1}", (object) str, (object) requestContext.GetUserId()));
        LicensingRequestType? nullable;
        if (requestType.HasValue)
        {
          nullable = requestType;
          LicensingRequestType licensingRequestType1 = LicensingRequestType.All;
          if (!(nullable.GetValueOrDefault() == licensingRequestType1 & nullable.HasValue))
          {
            nullable = requestType;
            LicensingRequestType licensingRequestType2 = LicensingRequestType.Client;
            if (!(nullable.GetValueOrDefault() == licensingRequestType2 & nullable.HasValue))
              goto label_9;
          }
        }
        maxEntitlements.Add(new MsdnEntitlement()
        {
          SubscriptionStatus = "Active",
          SubscriptionExpirationDate = this.GetOffsetUtcNow().AddMonths(1),
          EntitlementType = "LicensingIde",
          SubscriptionLevelCode = "VSOSubscription",
          EntitlementCode = entitlementCodeTuple != null ? entitlementCodeTuple.Item1 : "IDE-ENTERPRISE"
        });
label_9:
        if (requestType.HasValue)
        {
          nullable = requestType;
          LicensingRequestType licensingRequestType3 = LicensingRequestType.All;
          if (!(nullable.GetValueOrDefault() == licensingRequestType3 & nullable.HasValue))
          {
            nullable = requestType;
            LicensingRequestType licensingRequestType4 = LicensingRequestType.Service;
            if (!(nullable.GetValueOrDefault() == licensingRequestType4 & nullable.HasValue))
              goto label_15;
          }
        }
        MsdnEntitlement sourceEntitlement = new MsdnEntitlement()
        {
          SubscriptionStatus = "Active",
          SubscriptionExpirationDate = this.GetOffsetUtcNow().AddMonths(1),
          EntitlementType = "LicensingVso",
          SubscriptionLevelCode = "VSOSubscription",
          EntitlementCode = entitlementCodeTuple != null ? entitlementCodeTuple.Item2 : "VSO-ADVP"
        };
        maxEntitlements.Add(sourceEntitlement);
        if (sourceEntitlement.EntitlementCode == "VSO-ADVP")
          maxEntitlements.Add(this.CreateMTMEntitlement(sourceEntitlement));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1031038, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", ex);
        throw;
      }
label_15:
      requestContext.TraceLeave(1031034, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (GetMaxEntitlements));
      return maxEntitlements;
    }

    private List<MsdnEntitlement> GetEntitlementsInternal(
      IVssRequestContext requestContext,
      LicensingRequestType? requestType,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      requestContext.TraceEnter(1031120, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (GetEntitlementsInternal));
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      DateTime utcNow1 = this.GetUtcNow();
      UserClaims userClaims = UserClaims.GetUserClaims(requestContext, userIdentity);
      try
      {
        if (userClaims == null)
          return new List<MsdnEntitlement>();
        List<MsdnEntitlement> msdnEntitlements = this.GetMsdnEntitlements(requestContext, userIdentity);
        string str = msdnEntitlements.IsNullOrEmpty<MsdnEntitlement>() ? string.Empty : string.Join(",", msdnEntitlements.Select<MsdnEntitlement, string>((Func<MsdnEntitlement, string>) (x => x != null ? x.EntitlementCode ?? string.Empty : string.Empty)));
        requestContext.Trace(1031121, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "GetEntitlementsInternal: user puid {0}, call to EV2 returned {1}", (object) userClaims.Identifier, (object) str);
        List<MsdnEntitlement> list = msdnEntitlements.Where<MsdnEntitlement>((Func<MsdnEntitlement, bool>) (_ => _.EntitlementCode == "VSO-ADVP")).ToList<MsdnEntitlement>();
        if (list.Any<MsdnEntitlement>())
        {
          requestContext.Trace(1031122, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "GetEntitlementsInternal: user puid {0} has VSO Advance License. Adding MTM extension entitlement", (object) userClaims.Identifier);
          msdnEntitlements.AddRange(list.Select<MsdnEntitlement, MsdnEntitlement>((Func<MsdnEntitlement, MsdnEntitlement>) (x => this.CreateMTMEntitlement(x))));
        }
        if (msdnEntitlements.Any<MsdnEntitlement>())
          eventData.Add(CustomerIntelligenceProperty.MsdnEntitlements, string.Join<MsdnEntitlement>(", ", (IEnumerable<MsdnEntitlement>) msdnEntitlements));
        else
          eventData.Add(CustomerIntelligenceProperty.MsdnEntitlements, string.Empty);
        DateTime utcNow2 = this.GetUtcNow();
        eventData.Add(CustomerIntelligenceProperty.StartTime, (object) utcNow1);
        eventData.Add(CustomerIntelligenceProperty.EndTime, (object) utcNow2);
        eventData.Add(CustomerIntelligenceProperty.AccountId, (object) requestContext.ServiceHost.InstanceId);
        eventData.Add(CustomerIntelligenceProperty.RequestType, requestType?.ToString() ?? string.Empty);
        eventData.Add(CustomerIntelligenceProperty.UserClaimType, userClaims.Type);
        eventData.Add(CustomerIntelligenceProperty.UserId, userIdentity.Id.ToString());
        return msdnEntitlements;
      }
      catch (Exception ex)
      {
        string message = string.Format("GetEntitlementsInternal: Exception is thrown for the User '{0}'", (object) userClaims.Identifier.ToString());
        requestContext.Trace(1031127, TraceLevel.Warning, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", message);
        requestContext.TraceException(1031128, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        try
        {
          TelemetryHelper.PublishCustomerIntelligenceEvent(requestContext, "GetEntitlementsMsdnCIevent", eventData);
        }
        catch
        {
        }
        requestContext.TraceLeave(1031129, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (GetEntitlementsInternal));
      }
    }

    private List<MsdnEntitlement> GetMsdnEntitlements(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      IVssRequestContext systemRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(systemRequestContext.CancellationToken))
      {
        linkedTokenSource.CancelAfter(TimeSpan.FromSeconds((double) this.m_adapterSettings.GetEntitlementsTimeoutSeconds));
        IEntitlementService service = systemRequestContext.GetService<IEntitlementService>();
        return MsdnLicensingAdapter.ConvertEv4EntitlementToMSDN(systemRequestContext.RunSynchronously<List<Ev4Entitlement>>((Func<Task<List<Ev4Entitlement>>>) (() => service.GetMyEntitlementsAsync(systemRequestContext, "AzureDevOps", userIdentity, linkedTokenSource.Token))));
      }
    }

    internal static List<MsdnEntitlement> ConvertEv4EntitlementToMSDN(
      List<Ev4Entitlement> ev4Entitlements)
    {
      List<MsdnEntitlement> msdn = new List<MsdnEntitlement>();
      foreach (Ev4Entitlement ev4Entitlement in ev4Entitlements)
      {
        if (ev4Entitlement != null)
        {
          MsdnEntitlement msdnEntitlement = new MsdnEntitlement()
          {
            EntitlementCode = ev4Entitlement.EntitlementCode,
            EntitlementName = ev4Entitlement.EntitlementName,
            EntitlementType = ev4Entitlement.EntitlementType,
            SubscriptionId = ev4Entitlement.BenefitDetailGuid.ToString(),
            SubscriptionChannel = ev4Entitlement.SubscriptionChannel,
            SubscriptionExpirationDate = (DateTimeOffset) ev4Entitlement.SubscriptionExpirationDate,
            SubscriptionLevelCode = ev4Entitlement.SubscriptionLevelCode,
            SubscriptionLevelName = ev4Entitlement.SubscriptionLevelName,
            SubscriptionStatus = string.Equals(ev4Entitlement.SubscriptionStatus, "ACT", StringComparison.OrdinalIgnoreCase) ? "Active" : ev4Entitlement.SubscriptionStatus
          };
          msdn.Add(msdnEntitlement);
        }
      }
      return msdn;
    }

    private MsdnEntitlement CreateMTMEntitlement(MsdnEntitlement sourceEntitlement) => new MsdnEntitlement()
    {
      EntitlementCode = "EXT-MTM-0001",
      EntitlementName = "Test Manager Extension",
      EntitlementType = "VSTSExtension",
      SubscriptionStatus = sourceEntitlement.SubscriptionStatus,
      SubscriptionExpirationDate = sourceEntitlement.SubscriptionExpirationDate,
      IsEntitlementAvailable = sourceEntitlement.IsEntitlementAvailable,
      IsActivated = sourceEntitlement.IsActivated,
      SubscriptionId = sourceEntitlement.SubscriptionId,
      SubscriptionLevelCode = sourceEntitlement.SubscriptionLevelCode,
      SubscriptionLevelName = sourceEntitlement.SubscriptionLevelName,
      SubscriptionChannel = sourceEntitlement.SubscriptionChannel
    };

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.m_dateTimeProvider.UtcNow;

    [ExcludeFromCodeCoverage]
    internal virtual DateTimeOffset GetOffsetUtcNow() => new DateTimeOffset(this.m_dateTimeProvider.UtcNow);

    internal List<IUsageRight> GetRightsInternal(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      List<MsdnEntitlement> entitlements)
    {
      if (entitlements == null || entitlements.Count < 1)
      {
        requestContext.Trace(1031020, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "No Msdn entitlements for user: {0}.", (object) queryContext.UserIdentity.MasterId);
        return MsdnLicensingAdapter.CreateDefaultRights();
      }
      switch (queryContext.RequestType)
      {
        case LicensingRequestType.Client:
          CustomerIntelligenceData eventData = new CustomerIntelligenceData();
          eventData.Add(CustomerIntelligenceProperty.StartTime, (object) this.GetUtcNow());
          List<IUsageRight> clientRights = this.CreateClientRights(requestContext, queryContext, entitlements);
          eventData.Add(CustomerIntelligenceProperty.EndTime, (object) this.GetUtcNow());
          eventData.Add(CustomerIntelligenceProperty.MsdnRights, string.Join(",", clientRights.Select<IUsageRight, string>((Func<IUsageRight, string>) (x => x.Name))));
          eventData.Add(CustomerIntelligenceProperty.VisualStudioFamily, queryContext.VisualStudioFamily.ToString());
          eventData.Add(CustomerIntelligenceProperty.VisualStudioEdition, queryContext.VisualStudioEdition.ToString());
          eventData.Add(CustomerIntelligenceProperty.MasterId, (object) queryContext.UserIdentity.MasterId);
          TelemetryHelper.PublishCustomerIntelligenceEvent(requestContext, "GetClientRightsMsdnCIevent", eventData);
          return clientRights;
        case LicensingRequestType.Service:
          return this.CreateServiceRights(requestContext, queryContext, entitlements);
        default:
          requestContext.Trace(1031021, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "Not recognized queryContext.RequestType: {0}.", (object) queryContext.RequestType);
          return MsdnLicensingAdapter.CreateDefaultRights();
      }
    }

    private List<IUsageRight> CreateClientRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      List<MsdnEntitlement> entitlements)
    {
      if (queryContext.VisualStudioFamily == VisualStudioFamily.TestManager)
      {
        requestContext.Trace(1031041, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "CreateClientRights: User {0} requested edition TestManager. Creating TestManager right.", (object) queryContext.UserIdentity.MasterId);
        return this.CreateTestManagerClientRights(requestContext, queryContext, entitlements);
      }
      VisualStudioRight studioClientRights = this.CreateVisualStudioClientRights(requestContext, queryContext, entitlements);
      if (studioClientRights == null)
      {
        requestContext.Trace(1031042, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "CreateClientRights: User {0}: No active client entitlements.", (object) queryContext.UserIdentity.MasterId);
        return MsdnLicensingAdapter.CreateDefaultRights();
      }
      VisualStudioRight maxRight = studioClientRights;
      if (queryContext.VisualStudioEdition != VisualStudioEdition.Unspecified)
      {
        if (queryContext.ProductVersion.Major < 14)
        {
          if (queryContext.VisualStudioEdition == VisualStudioEdition.Community)
          {
            requestContext.Trace(1031043, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "CreateClientRights: User {0} requested edition Community. No entitlement granted.", (object) queryContext.UserIdentity.MasterId);
            return MsdnLicensingAdapter.CreateDefaultRights();
          }
          if (0 > VisualStudioRight.CompareVisualStudioEdition(maxRight.Edition, queryContext.VisualStudioEdition))
          {
            requestContext.Trace(1031044, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "CreateClientRights: User {0} requested edition {1} but max Msdn entitlement is {2}. No entitlement granted.", (object) queryContext.UserIdentity.MasterId, (object) queryContext.VisualStudioEdition, (object) maxRight.Edition);
            return MsdnLicensingAdapter.CreateDefaultRights();
          }
          maxRight.Edition = queryContext.VisualStudioEdition;
        }
        else if (0 > VisualStudioRight.CompareVisualStudioEdition(maxRight.Edition, queryContext.VisualStudioEdition))
        {
          requestContext.Trace(1031045, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "CreateClientRights: User {0} requested edition {1} but max Msdn entitlement is {2}. No entitlement granted.", (object) queryContext.UserIdentity.MasterId, (object) queryContext.VisualStudioEdition, (object) maxRight.Edition);
          if (maxRight.Edition != VisualStudioEdition.Professional || queryContext.VisualStudioEdition != VisualStudioEdition.Enterprise)
            return MsdnLicensingAdapter.CreateDefaultRights();
          return new List<IUsageRight>()
          {
            (IUsageRight) this.UpdateRightsWithTrialProperties(requestContext, maxRight)
          };
        }
      }
      requestContext.Trace(1031046, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "CreateClientRights: User {0} gets service right {1}.", (object) queryContext.UserIdentity.MasterId, (object) maxRight.ToString());
      return new List<IUsageRight>()
      {
        (IUsageRight) maxRight
      };
    }

    private List<IUsageRight> CreateServiceRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      List<MsdnEntitlement> entitlements)
    {
      requestContext.TraceEnter(1031050, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (CreateServiceRights));
      IEnumerable<VisualStudioOnlineServiceRight> source = entitlements.Where<MsdnEntitlement>((Func<MsdnEntitlement, bool>) (entitlement => MsdnLicensingAdapter.IsActiveEntitlement(entitlement))).Select<MsdnEntitlement, VisualStudioOnlineServiceRight>((Func<MsdnEntitlement, VisualStudioOnlineServiceRight>) (entitlement => this.CreateServiceRight(requestContext, queryContext, entitlement))).Where<VisualStudioOnlineServiceRight>((Func<VisualStudioOnlineServiceRight, bool>) (right => right != null));
      if (!source.Any<VisualStudioOnlineServiceRight>())
      {
        requestContext.Trace(1031051, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "CreateServiceRights: User {0}: No active service Msdn entitlements for user.", (object) queryContext.UserIdentity.MasterId);
        return MsdnLicensingAdapter.CreateDefaultRights();
      }
      string str = string.Join(",", source.Select<VisualStudioOnlineServiceRight, string>((Func<VisualStudioOnlineServiceRight, string>) (x => x != null ? x.ServiceLevel.ToString() ?? string.Empty : string.Empty)));
      requestContext.Trace(1031052, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "CreateServiceRights: User {0} has active service rights {1}.", (object) queryContext.UserIdentity.MasterId, (object) str);
      VisualStudioOnlineServiceRight onlineServiceRight = source.OrderByDescending<VisualStudioOnlineServiceRight, VisualStudioOnlineServiceLevel>((Func<VisualStudioOnlineServiceRight, VisualStudioOnlineServiceLevel>) (right => right.ServiceLevel)).First<VisualStudioOnlineServiceRight>();
      requestContext.Trace(1031053, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "CreateServiceRights: User {0} gets service right {1}.", (object) queryContext.UserIdentity.MasterId, (object) onlineServiceRight.ToString());
      requestContext.TraceEnter(1031059, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (CreateServiceRights));
      return new List<IUsageRight>()
      {
        (IUsageRight) onlineServiceRight
      };
    }

    private List<IUsageRight> CreateTestManagerClientRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      List<MsdnEntitlement> entitlements)
    {
      IEnumerable<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> familyEditionRules = entitlements.Where<MsdnEntitlement>((Func<MsdnEntitlement, bool>) (entitlement => MsdnLicensingAdapter.IsActiveEntitlement(entitlement) && MsdnLicensingAdapter.IsTestManagerEntitlementType(entitlement))).Select<MsdnEntitlement, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>((Func<MsdnEntitlement, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>) (entitlement => MsdnLicensingAdapter.CreateRightFamilyEditionRule(requestContext, queryContext, entitlement, MsdnLicensingAdapter.s_MsdnTestManagerMap))).Where<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>((Func<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule, bool>) (right => right != null && right.Family != VisualStudioFamily.Invalid));
      if (!familyEditionRules.Any<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>())
      {
        requestContext.Trace(1031131, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: No active client entitlements.", (object) queryContext.UserIdentity.MasterId);
        return MsdnLicensingAdapter.CreateDefaultRights();
      }
      MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule familyEditionRule = MsdnLicensingAdapter.GetMaxRightFamilyEditionRule(familyEditionRules);
      MsdnEntitlement maxEntitlement = MsdnLicensingAdapter.GetMaxEntitlement(familyEditionRule, entitlements);
      TestManagerRight managerClientRight = this.CreateTestManagerClientRight(requestContext, queryContext, familyEditionRule, maxEntitlement);
      if (managerClientRight == null)
      {
        requestContext.Trace(1031132, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: No active client entitlements.", (object) queryContext.UserIdentity.MasterId);
        return MsdnLicensingAdapter.CreateDefaultRights();
      }
      return new List<IUsageRight>()
      {
        (IUsageRight) managerClientRight
      };
    }

    private TestManagerRight CreateTestManagerClientRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule vsRule,
      MsdnEntitlement entitlement)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      ArgumentUtility.CheckForNull<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>(vsRule, nameof (vsRule));
      if (string.IsNullOrEmpty(vsRule.EntitlementCode))
      {
        requestContext.Trace(1031061, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected empty entitlement code", (object) queryContext.UserIdentity.MasterId);
        return (TestManagerRight) null;
      }
      Dictionary<string, object> attributes = (Dictionary<string, object>) null;
      if (!string.IsNullOrEmpty(entitlement.SubscriptionChannel))
        attributes = new Dictionary<string, object>()
        {
          {
            "SubscriptionChannel",
            (object) entitlement.SubscriptionChannel
          }
        };
      if (string.IsNullOrEmpty(entitlement.SubscriptionLevelCode))
      {
        requestContext.Trace(1031062, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected empty Subscription Level Code", (object) queryContext.UserIdentity.MasterId);
        return (TestManagerRight) null;
      }
      bool flag = MsdnLicensingAdapter.IsVisualStudioBundleSubscription(entitlement);
      string licenseDescriptionId = flag ? "VSOSubscription" : "MSDN";
      if (string.IsNullOrEmpty(licenseDescriptionId))
      {
        requestContext.Trace(1031063, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected empty licenseDescriptionId as there is no description Id found for Subscription Level Code {1}", (object) queryContext.UserIdentity.MasterId, flag ? (object) entitlement.SubscriptionLevelCode : (object) "MSDN");
        return (TestManagerRight) null;
      }
      string licenseDescription = this.m_serviceSettings.GetLicenseDescription(requestContext, flag ? licenseDescriptionId : "MSDN");
      if (!string.IsNullOrEmpty(licenseDescription))
        return TestManagerRight.Create(vsRule.RightName, queryContext.ProductVersion, vsRule.Edition, DateTimeOffset.MaxValue, string.Empty, string.Empty, licenseDescriptionId, licenseDescription, attributes);
      requestContext.Trace(1031064, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected empty licenseFallbackDescription as there is no licenseFallbackDescription found for id {1}", (object) queryContext.UserIdentity.MasterId, (object) licenseDescriptionId);
      return (TestManagerRight) null;
    }

    private VisualStudioRight CreateVisualStudioClientRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      List<MsdnEntitlement> entitlements)
    {
      Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> msdnEditionMap = queryContext.VisualStudioFamily == VisualStudioFamily.VisualStudioMac ? MsdnLicensingAdapter.s_MsdnVsMacEditionMap : MsdnLicensingAdapter.s_MsdnVsEditionMap;
      IEnumerable<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> familyEditionRules = entitlements.Where<MsdnEntitlement>((Func<MsdnEntitlement, bool>) (entitlement => MsdnLicensingAdapter.IsActiveEntitlement(entitlement) && MsdnLicensingAdapter.IsClientEntitlementType(entitlement))).Select<MsdnEntitlement, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>((Func<MsdnEntitlement, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>) (entitlement => MsdnLicensingAdapter.CreateRightFamilyEditionRule(requestContext, queryContext, entitlement, msdnEditionMap))).Where<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>((Func<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule, bool>) (right => right != null));
      MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule familyEditionRule = (MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule) null;
      if (familyEditionRules.Count<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>() > 0)
        familyEditionRule = MsdnLicensingAdapter.GetMaxRightFamilyEditionRule(familyEditionRules);
      MsdnEntitlement entitlement1 = (MsdnEntitlement) null;
      if (familyEditionRule != null)
        entitlement1 = MsdnLicensingAdapter.GetMaxEntitlement(familyEditionRule, entitlements);
      return familyEditionRule != null && entitlement1 != null ? this.CreateVisualStudioClientRight(requestContext, queryContext, familyEditionRule, entitlement1) : (VisualStudioRight) null;
    }

    private static MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule GetMaxRightFamilyEditionRule(
      IEnumerable<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> rightsEditionRules)
    {
      return rightsEditionRules.OrderByDescending<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule, VisualStudioEdition>((Func<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule, VisualStudioEdition>) (entitlement => entitlement.Edition)).ThenByDescending<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule, int>((Func<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule, int>) (entitlement => entitlement.Priority)).First<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>();
    }

    private static MsdnEntitlement GetMaxEntitlement(
      MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule rule,
      List<MsdnEntitlement> entitlements)
    {
      List<MsdnEntitlement> list = entitlements.Where<MsdnEntitlement>((Func<MsdnEntitlement, bool>) (entitlement => entitlement.EntitlementCode == rule.EntitlementCode)).ToList<MsdnEntitlement>();
      list.Sort((IComparer<MsdnEntitlement>) new MsdnLicensingAdapter.MsdnEntitlementComparer());
      return list.FirstOrDefault<MsdnEntitlement>();
    }

    private VisualStudioRight CreateVisualStudioClientRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule vsRule,
      MsdnEntitlement entitlement)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      ArgumentUtility.CheckForNull<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>(vsRule, nameof (vsRule));
      if (string.IsNullOrEmpty(vsRule.EntitlementCode))
      {
        requestContext.Trace(1031061, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected empty entitlement code", (object) queryContext.UserIdentity.MasterId);
        return (VisualStudioRight) null;
      }
      Dictionary<string, object> attributes = (Dictionary<string, object>) null;
      if (!string.IsNullOrEmpty(entitlement.SubscriptionChannel))
        attributes = new Dictionary<string, object>()
        {
          {
            "SubscriptionChannel",
            (object) entitlement.SubscriptionChannel
          }
        };
      if (string.IsNullOrEmpty(entitlement.SubscriptionLevelCode))
      {
        requestContext.Trace(1031062, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected empty Subscription Level Code", (object) queryContext.UserIdentity.MasterId);
        return (VisualStudioRight) null;
      }
      if (!string.IsNullOrEmpty(entitlement.SubscriptionLevelName))
      {
        string entitlementNameAttribute = this.GetEntitlementNameAttribute(entitlement);
        if (attributes != null)
          attributes.Add("SubscriptionLevel", (object) entitlementNameAttribute);
        else
          attributes = new Dictionary<string, object>()
          {
            {
              "SubscriptionLevel",
              (object) entitlementNameAttribute
            }
          };
      }
      bool flag = MsdnLicensingAdapter.IsVisualStudioBundleSubscription(entitlement);
      string licenseDescriptionId = flag ? this.m_serviceSettings.GetLicenseDescriptionId(requestContext, entitlement.SubscriptionLevelCode) : "MSDN";
      if (string.IsNullOrEmpty(licenseDescriptionId))
      {
        requestContext.Trace(1031063, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected empty licenseDescriptionId as there is no description Id found for EntitlementCode {1}", (object) queryContext.UserIdentity.MasterId, !flag ? (object) "MSDN" : (object) entitlement.SubscriptionLevelCode);
        return (VisualStudioRight) null;
      }
      string licenseDescription = this.m_serviceSettings.GetLicenseDescription(requestContext, flag ? licenseDescriptionId : "MSDN");
      if (!string.IsNullOrEmpty(licenseDescription))
        return VisualStudioRight.Create(vsRule.RightName, queryContext.ProductVersion, vsRule.Edition, DateTimeOffset.MaxValue, string.Empty, string.Empty, licenseDescriptionId, licenseDescription, attributes);
      requestContext.Trace(1031064, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected empty licenseFallbackDescription as there is no licenseFallbackDescription found for id {1}", (object) queryContext.UserIdentity.MasterId, (object) licenseDescriptionId);
      return (VisualStudioRight) null;
    }

    private string GetEntitlementNameAttribute(MsdnEntitlement entitlement)
    {
      if (entitlement.SubscriptionLevelName.Contains("Enterprise") || entitlement.SubscriptionLevelName.Contains("ADVP") || entitlement.SubscriptionLevelName.Contains("Ultimate"))
        return "Enterprise";
      if (entitlement.SubscriptionLevelName.Contains("Professional") || entitlement.SubscriptionLevelName.Contains("Pro"))
        return "Professional";
      return entitlement.SubscriptionLevelName.Contains("Premium") ? "Premium" : "Community";
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

    private static MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule CreateRightFamilyEditionRule(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      MsdnEntitlement entitlement,
      Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> editionMap)
    {
      if (string.IsNullOrEmpty(entitlement.EntitlementCode))
      {
        requestContext.Trace(1031061, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected empty entitlement code", (object) queryContext.UserIdentity.MasterId);
        return (MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule) null;
      }
      MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule familyEditionRule;
      if (editionMap.TryGetValue(entitlement.EntitlementCode, out familyEditionRule))
        return familyEditionRule;
      requestContext.Trace(1031062, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unrecognized client entitlement code: {1}.", (object) queryContext.UserIdentity.MasterId, (object) entitlement.EntitlementCode);
      return (MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule) null;
    }

    private VisualStudioOnlineServiceRight CreateServiceRight(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext,
      MsdnEntitlement entitlement)
    {
      requestContext.Trace(1031070, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "Creating service rights for User {0}, MsdnEntitlementCode {1}.", (object) queryContext.UserIdentity.MasterId, (object) entitlement.EntitlementCode);
      if (!LicensingComparers.MsdnEntitlementComparer.Equals("LicensingVso", entitlement.EntitlementType))
      {
        requestContext.Trace(1031071, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected service entitlement type: {1}.", (object) queryContext.UserIdentity.MasterId, (object) entitlement.EntitlementType);
        return (VisualStudioOnlineServiceRight) null;
      }
      if (string.IsNullOrEmpty(entitlement.EntitlementCode))
      {
        requestContext.Trace(1031072, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unexpected empty entitlement code", (object) queryContext.UserIdentity.MasterId);
        return (VisualStudioOnlineServiceRight) null;
      }
      VisualStudioOnlineServiceLevel serviceLevel;
      if (!MsdnLicensingAdapter.s_MsdnServiceLevelMap.TryGetValue(entitlement.EntitlementCode, out serviceLevel))
      {
        requestContext.Trace(1031073, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "User {0}: Unrecognized service entitlement code: {1}.", (object) queryContext.UserIdentity.MasterId, (object) entitlement.EntitlementCode);
        return (VisualStudioOnlineServiceRight) null;
      }
      requestContext.Trace(1031074, TraceLevel.Info, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "Returnign service rights for User {0}. serviceLevel {1}.", (object) queryContext.UserIdentity.MasterId, (object) serviceLevel.ToString());
      return VisualStudioOnlineServiceRight.Create(serviceLevel, entitlement.SubscriptionExpirationDate);
    }

    private static List<IUsageRight> CreateDefaultRights() => new List<IUsageRight>();

    internal static bool IsActiveEntitlement(MsdnEntitlement entitlement) => LicensingComparers.MsdnEntitlementComparer.Compare("Active", entitlement.SubscriptionStatus) == 0 || LicensingComparers.MsdnEntitlementComparer.Compare("Grace", entitlement.SubscriptionStatus) == 0;

    internal static bool IsTestManagerEntitlementType(MsdnEntitlement entitlement) => MsdnLicensingAdapter.IsClientEntitlementType(entitlement) || LicensingComparers.MsdnEntitlementComparer.Compare("VSTSExtension", entitlement.EntitlementType) == 0;

    internal static bool IsClientEntitlementType(MsdnEntitlement entitlement) => LicensingComparers.MsdnEntitlementComparer.Compare("LicensingIde", entitlement.EntitlementType) == 0;

    internal static bool IsMsdnPlatformSubscription(MsdnEntitlement entitlement) => LicensingComparers.MsdnEntitlementComparer.Equals("MDN-SDG-000001", entitlement.SubscriptionLevelCode);

    internal static bool IsVisualStudioBundleSubscription(MsdnEntitlement entitlement) => MsdnLicensingAdapter.s_VsWeightageRule.Keys.Contains<string>(entitlement.SubscriptionLevelCode, (IEqualityComparer<string>) LicensingComparers.MsdnEntitlementComparer);

    private static Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> CreateMsdnVsEditionMap() => new Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>((IEqualityComparer<string>) LicensingComparers.RightNameComparer)
    {
      {
        "IDE-PRO",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("VisualStudio", "IDE-PRO", VisualStudioFamily.VisualStudio, VisualStudioEdition.Professional, 10)
      },
      {
        "IDE-TESTPRO",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("TestProfessional", "IDE-TESTPRO", VisualStudioFamily.VisualStudioTestProfessional, VisualStudioEdition.Unspecified, 20)
      },
      {
        "IDE-PREMIUM",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("VisualStudio", "IDE-PREMIUM", VisualStudioFamily.VisualStudio, VisualStudioEdition.Premium, 30)
      },
      {
        "IDE-ULTIMATE",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("VisualStudio", "IDE-ULTIMATE", VisualStudioFamily.VisualStudio, VisualStudioEdition.Ultimate, 40)
      },
      {
        "IDE-ENTERPRISE",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("VisualStudio", "IDE-ENTERPRISE", VisualStudioFamily.VisualStudio, VisualStudioEdition.Enterprise, 50)
      }
    };

    private static Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> CreateMsdnVsMacEditionMap() => new Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>((IEqualityComparer<string>) LicensingComparers.RightNameComparer)
    {
      {
        "IDE-PRO",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("VSonMac", "IDE-PRO", VisualStudioFamily.VisualStudio, VisualStudioEdition.Professional, 10)
      },
      {
        "IDE-PREMIUM",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("VSonMac", "IDE-PREMIUM", VisualStudioFamily.VisualStudio, VisualStudioEdition.Premium, 30)
      },
      {
        "IDE-ULTIMATE",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("VSonMac", "IDE-ULTIMATE", VisualStudioFamily.VisualStudio, VisualStudioEdition.Ultimate, 40)
      },
      {
        "IDE-ENTERPRISE",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("VSonMac", "IDE-ENTERPRISE", VisualStudioFamily.VisualStudio, VisualStudioEdition.Enterprise, 50)
      }
    };

    private static Dictionary<string, int> CreateVsWeightageRule() => new Dictionary<string, int>()
    {
      {
        "VSE-GIT-000001",
        30
      },
      {
        "VSE-ISV-0001",
        25
      },
      {
        "VSE-ANU-0001",
        20
      },
      {
        "VSP-GIT-000001",
        15
      },
      {
        "VSP-ANU-0001",
        10
      },
      {
        "VSP-MON-0001",
        -10
      },
      {
        "VSE-MON-0001",
        -20
      },
      {
        "X-RTL-000056",
        -30
      },
      {
        "X-RTL-000052",
        -40
      },
      {
        "ENT-NFR-BASIC",
        -50
      }
    };

    private static Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule> CreateMsdnTestManagerMap() => new Dictionary<string, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>((IEqualityComparer<string>) LicensingComparers.RightNameComparer)
    {
      {
        "IDE-PRO",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule(string.Empty, "IDE-PRO", VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified, 10)
      },
      {
        "IDE-PREMIUM",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("TestManager", "IDE-PREMIUM", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified, 20)
      },
      {
        "IDE-ULTIMATE",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("TestManager", "IDE-ULTIMATE", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified, 30)
      },
      {
        "IDE-TESTPRO",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("TestManager", "IDE-TESTPRO", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified, 40)
      },
      {
        "IDE-ENTERPRISE",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("TestManager", "IDE-ENTERPRISE", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified, 50)
      },
      {
        "EXT-MTM-0001",
        new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule("TestManager", "EXT-MTM-0001", VisualStudioFamily.TestManager, VisualStudioEdition.Unspecified, 60)
      }
    };

    private static Dictionary<string, VisualStudioOnlineServiceLevel> CreateMsdnServiceLevelMap() => new Dictionary<string, VisualStudioOnlineServiceLevel>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "VSO-STD",
        VisualStudioOnlineServiceLevel.Express
      },
      {
        "VSO-ADVP",
        VisualStudioOnlineServiceLevel.AdvancedPlus
      }
    };

    private void PopulateSettings(IVssRequestContext requestContext) => this.m_adapterSettings = this.m_settingsManager.GetMsdnAdapterConfiguration(requestContext);

    internal static License TranslateMsdnEntitlementsToMsdnLicense(
      List<MsdnEntitlement> entitlements)
    {
      IList<MsdnLicense> source1 = (IList<MsdnLicense>) new List<MsdnLicense>();
      if (entitlements != null)
      {
        foreach (IGrouping<string, MsdnEntitlement> source2 in entitlements.GroupBy<MsdnEntitlement, string>((Func<MsdnEntitlement, string>) (e => e.SubscriptionId), (IEqualityComparer<string>) LicensingComparers.MsdnEntitlementComparer))
        {
          string str1 = string.Empty;
          MsdnEntitlement msdnEntitlement1 = source2.Where<MsdnEntitlement>((Func<MsdnEntitlement, bool>) (e => MsdnLicensingAdapter.IsActiveEntitlement(e) && !MsdnLicensingAdapter.IsMsdnPlatformSubscription(e) && LicensingComparers.MsdnEntitlementComparer.Equals("LicensingIde", e.EntitlementType))).OrderByDescending<MsdnEntitlement, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>((Func<MsdnEntitlement, MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>) (e => MsdnLicensingAdapter.TranslateEntitlementCodeToVsEdition(e.EntitlementCode))).FirstOrDefault<MsdnEntitlement>();
          if (msdnEntitlement1 != null)
            str1 = msdnEntitlement1.EntitlementCode;
          string str2 = string.Empty;
          MsdnEntitlement msdnEntitlement2 = source2.Where<MsdnEntitlement>((Func<MsdnEntitlement, bool>) (e => MsdnLicensingAdapter.IsActiveEntitlement(e) && LicensingComparers.MsdnEntitlementComparer.Equals("LicensingVso", e.EntitlementType))).OrderByDescending<MsdnEntitlement, VisualStudioOnlineServiceLevel>((Func<MsdnEntitlement, VisualStudioOnlineServiceLevel>) (e => MsdnLicensingAdapter.TranslateEntitlementCodeToVsOnlineServiceLevel(e.EntitlementCode))).FirstOrDefault<MsdnEntitlement>();
          if (msdnEntitlement2 != null)
            str2 = msdnEntitlement2.EntitlementCode;
          MsdnEntitlementCodeTuple key = new MsdnEntitlementCodeTuple(str1, str2);
          MsdnLicense msdnLicense;
          if (MsdnLicensingAdapter.s_MsdnEntitlementCodeTupleToMsdnLicenseMap.TryGetValue(key, out msdnLicense))
            source1.Add(msdnLicense);
        }
      }
      return (License) source1.OrderByDescending<MsdnLicense, MsdnLicenseType>((Func<MsdnLicense, MsdnLicenseType>) (l => l.License)).FirstOrDefault<MsdnLicense>() ?? License.None;
    }

    internal static ICollection<string> TranslateMsdnEntitlementsToMsdnExtensions(
      List<MsdnEntitlement> entitlements)
    {
      IList<string> msdnExtensions = (IList<string>) new List<string>();
      List<MsdnEntitlement> msdnEntitlementList = new List<MsdnEntitlement>();
      if (entitlements != null)
      {
        foreach (IGrouping<string, MsdnEntitlement> source in entitlements.GroupBy<MsdnEntitlement, string>((Func<MsdnEntitlement, string>) (e => e.SubscriptionId), (IEqualityComparer<string>) LicensingComparers.MsdnEntitlementComparer))
        {
          string empty = string.Empty;
          msdnEntitlementList.AddRange(source.Where<MsdnEntitlement>((Func<MsdnEntitlement, bool>) (e =>
          {
            if (!MsdnLicensingAdapter.IsActiveEntitlement(e))
              return false;
            return LicensingComparers.MsdnEntitlementComparer.Equals("VSTSExtension", e.EntitlementType) || LicensingComparers.MsdnEntitlementComparer.Equals("LicensingVso", e.EntitlementType);
          })));
        }
        foreach (MsdnEntitlement msdnEntitlement in msdnEntitlementList)
        {
          string str;
          if (ExtensionLicensingAdapter.TestManagerRulesMap.TryGetValue(msdnEntitlement.EntitlementCode, out str) && !msdnExtensions.Contains(str))
            msdnExtensions.Add(str);
        }
      }
      return (ICollection<string>) msdnExtensions;
    }

    private static MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule TranslateEntitlementCodeToVsEdition(
      string entitlementCode)
    {
      MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule familyEditionRule;
      return !MsdnLicensingAdapter.s_MsdnVsEditionMap.TryGetValue(entitlementCode, out familyEditionRule) ? MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule.Invalid : familyEditionRule;
    }

    private static VisualStudioOnlineServiceLevel TranslateEntitlementCodeToVsOnlineServiceLevel(
      string entitlementCode)
    {
      VisualStudioOnlineServiceLevel onlineServiceLevel;
      return !MsdnLicensingAdapter.s_MsdnServiceLevelMap.TryGetValue(entitlementCode, out onlineServiceLevel) ? VisualStudioOnlineServiceLevel.None : onlineServiceLevel;
    }

    private static Dictionary<MsdnEntitlementCodeTuple, MsdnLicense> CreateMsdnEntitlementCodeTupleToMsdnLicenseMap() => new Dictionary<MsdnEntitlementCodeTuple, MsdnLicense>()
    {
      {
        new MsdnEntitlementCodeTuple("IDE-PRO", "VSO-STD"),
        MsdnLicense.Professional
      },
      {
        new MsdnEntitlementCodeTuple("IDE-PRO", "VSO-ADVP"),
        MsdnLicense.Platforms
      },
      {
        new MsdnEntitlementCodeTuple("IDE-PREMIUM", "VSO-ADVP"),
        MsdnLicense.Premium
      },
      {
        new MsdnEntitlementCodeTuple("IDE-ULTIMATE", "VSO-ADVP"),
        MsdnLicense.Ultimate
      },
      {
        new MsdnEntitlementCodeTuple("IDE-TESTPRO", "VSO-ADVP"),
        MsdnLicense.TestProfessional
      },
      {
        new MsdnEntitlementCodeTuple("IDE-ENTERPRISE", "VSO-ADVP"),
        MsdnLicense.Enterprise
      },
      {
        new MsdnEntitlementCodeTuple(string.Empty, "VSO-ADVP"),
        MsdnLicense.Platforms
      },
      {
        new MsdnEntitlementCodeTuple("IDE-ENTERPRISE", "VSO-STD"),
        MsdnLicense.Enterprise
      }
    };

    private class VisualStudioRightToFamilyEditionRule : 
      IComparable<MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule>
    {
      private static readonly MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule s_invalid = new MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule(string.Empty, string.Empty, VisualStudioFamily.Invalid, VisualStudioEdition.Unspecified, 0);

      public string RightName { get; private set; }

      public string EntitlementCode { get; private set; }

      public VisualStudioFamily Family { get; private set; }

      public VisualStudioEdition Edition { get; private set; }

      public int Priority { get; private set; }

      public VisualStudioRightToFamilyEditionRule(
        string rightName,
        string entitlementCode,
        VisualStudioFamily family,
        VisualStudioEdition edition,
        int priority)
      {
        this.RightName = rightName;
        this.EntitlementCode = entitlementCode;
        this.Family = family;
        this.Edition = edition;
        this.Priority = priority;
      }

      public static MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule Invalid => MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule.s_invalid;

      public int CompareTo(
        MsdnLicensingAdapter.VisualStudioRightToFamilyEditionRule other)
      {
        return other == null ? -1 : this.Edition.CompareTo((object) other.Edition);
      }
    }

    private class MsdnEntitlementComparer : IComparer<MsdnEntitlement>
    {
      public int Compare(MsdnEntitlement left, MsdnEntitlement right)
      {
        int num1 = 0;
        if (left == null && right == null)
          return 0;
        if (left == null || right == null)
          return -1;
        if (!MsdnLicensingAdapter.s_VsWeightageRule.TryGetValue(left.SubscriptionLevelCode, out num1) && !MsdnLicensingAdapter.s_VsWeightageRule.TryGetValue(right.SubscriptionLevelCode, out num1))
          return 0;
        int num2;
        MsdnLicensingAdapter.s_VsWeightageRule.TryGetValue(right.SubscriptionLevelCode, out num2);
        int num3;
        MsdnLicensingAdapter.s_VsWeightageRule.TryGetValue(left.SubscriptionLevelCode, out num3);
        if (num2 == num3)
          return 0;
        if (num3 > num2)
          return -1;
        return num3 < num2 ? 1 : num1;
      }
    }
  }
}
