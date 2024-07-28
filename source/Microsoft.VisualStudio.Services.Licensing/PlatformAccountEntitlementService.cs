// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.PlatformAccountEntitlementService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.Azure.DevOps.Licensing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using Microsoft.VisualStudio.Services.Licensing.Internal;
using Microsoft.VisualStudio.Services.Licensing.Service;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class PlatformAccountEntitlementService : 
    ILicensingEntitlementService,
    IVssFrameworkService,
    IInternalPlatformEntitlementService
  {
    private readonly ILicensingConfigurationManager m_settingsManager;
    private Guid m_serviceHostId;
    private ILicensingRepository m_licensingRepository;
    private readonly ServiceFactory<IOfferSubscriptionService> m_offerSubscriptionServiceFactory;
    private readonly ServiceFactory<AzCommerceService> m_azCommerceServiceFactory;
    private readonly IVssDateTimeProvider m_dateTimeProvider;
    protected CommandPropertiesSetter m_serviceCircuitBreakerSettings;
    private const string s_circuitBreakerUpdateLastAccessedDate = "UserLastAccessedDate";
    private static readonly CommandPropertiesSetter s_defaultCircuitbreakerProperties = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithFallbackDisabled(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(50).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(10.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(60000).WithMetricsRollingStatisticalWindowBuckets(60);
    private const string s_area = "VisualStudio.Services.PlatformAccountEntitlementService";
    private const string s_layer = "BusinessLogic";
    internal const string s_ClientLicensingEntitlementType = "LicensingIde";
    internal const string s_ServiceLicensingEntitlementType = "LicensingVso";
    private const string s_SubscriptionStatusActive = "Active";
    private const string s_SubscriptionStatusGrace = "Grace";
    private const string s_featureGetAccountEntitlements = "VisualStudio.LicensingService.GetAccountEntitlements";
    internal const string s_featureGetUserAccountRights = "VisualStudio.LicensingService.GetUserAccountRights";
    internal const string s_featureReturnsHighestAccountRightsForStakeholder = "VisualStudio.LicensingService.ReturnHighestAccountRightsForStakeholder";
    internal const string s_featureReturnsHighestAccountRightsForMsdnEligible = "VisualStudio.LicensingService.ReturnHighestAccountRightsForMsdnEligible";
    internal const string s_featureBypassBusinessPolicyRevalidationOnUserAssignment = "VisualStudio.LicensingService.BypassBusinessPolicyRevalidationOnUserAssignment";
    internal const string s_featureEnableAutoUpgradeLicenseFromMsdn = "VisualStudio.LicensingService.EnableAutoUpgradeLicenseFromMsdn";
    internal const string s_featureEnableAutoUpgradeFromProfessionalFromMsdn = "VisualStudio.LicensingService.EnableAutoUpgradeFromProfessionalToMsdn";
    internal const string s_featureDisableInternalDesignation = "VisualStudio.Services.Licensing.DisableInternalDesignation";
    internal const string s_featureDefaultLicense = "AzureDevOps.Services.Licensing.DefaultLicense";
    internal const string s_featureEnableActiveLicensesTracing = "AzureDevOps.Services.Licensing.EnableActiveLicensesTracing";
    internal const string s_featureDisableUpdatingUserStatusOnSeedEntitlement = "AzureDevOps.Services.Licensing.DisableUpdatingUserStatusOnSeedEntitlement";
    internal const string s_featureCheckUserStatusBeforeUpdatingLastAccessDate = "AzureDevOps.Services.Licensing.CheckUserStatusBeforeUpdatingLastAccessDate";
    internal const string s_featureEnableTracingVsLicenseDataForPossibleDowngrade = "AzureDevOps.Services.Licensing.EnableTracingVsLicenseDataForPossibleDowngrade";
    internal const string s_featureEnableVsLicenseDowngrade = "AzureDevOps.Services.Licensing.EnableVsLicenseDowngrade";
    private const VisualStudioOnlineServiceLevel DefaultServiceLevel = VisualStudioOnlineServiceLevel.Stakeholder;

    public PlatformAccountEntitlementService()
      : this((ServiceFactory<IOfferSubscriptionService>) (x => x.GetService<IOfferSubscriptionService>()), (ServiceFactory<AzCommerceService>) (x => x.GetService<AzCommerceService>()))
    {
    }

    internal PlatformAccountEntitlementService(
      ServiceFactory<IOfferSubscriptionService> offerSubscriptionServiceFactory,
      ServiceFactory<AzCommerceService> azCommerceServiceFactory)
      : this(offerSubscriptionServiceFactory, azCommerceServiceFactory, VssDateTimeProvider.DefaultProvider, (ILicensingConfigurationManager) new LicensingConfigurationRegistryManager())
    {
    }

    internal PlatformAccountEntitlementService(
      ServiceFactory<IOfferSubscriptionService> offerSubscriptionServiceFactory,
      ServiceFactory<AzCommerceService> azCommerceServiceFactory,
      IVssDateTimeProvider dateTimeProvider,
      ILicensingConfigurationManager settingsManager)
    {
      this.m_offerSubscriptionServiceFactory = offerSubscriptionServiceFactory;
      this.m_azCommerceServiceFactory = azCommerceServiceFactory;
      this.m_dateTimeProvider = dateTimeProvider;
      this.m_settingsManager = settingsManager;
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      this.m_serviceHostId = requestContext.ServiceHost.InstanceId;
      this.ValidateNonDeploymentContext(requestContext);
      this.m_licensingRepository = LicensingRepositoryFactory<ApplicationLicensingRepository>.Create(requestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IList<AccountEntitlement> GetAccountEntitlements(IVssRequestContext requestContext) => this.GetAccountEntitlementsInternal(requestContext);

    public IList<AccountEntitlement> GetAccountEntitlements(
      IVssRequestContext requestContext,
      int top,
      int skip = 0)
    {
      return this.GetAccountEntitlementsInternal(requestContext, new int?(top), skip);
    }

    public PagedAccountEntitlements SearchAccountEntitlements(
      IVssRequestContext requestContext,
      string continuation,
      string filter,
      string orderBy,
      bool includeServicePrincipals = false)
    {
      this.ValidateCollectionContext(requestContext);
      SecurityUtil.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      IEnumerable<AccountEntitlement> list = Enumerable.Empty<AccountEntitlement>();
      string continuationToken = string.Empty;
      using (PerformanceTimer.StartMeasure(requestContext, "PlatformAccountEntitlementService_SearchAccountEntitlements"))
      {
        AccountEntitlementFilter entitlementFilter = AccountEntitlementFilter.Parse(filter);
        if (!includeServicePrincipals)
          entitlementFilter = this.FilterUsersOnly(entitlementFilter);
        AccountEntitlementSort sort = AccountEntitlementSort.Parse(orderBy);
        IPagedList<UserLicense> filteredEntitlements = this.m_licensingRepository.GetFilteredEntitlements(requestContext, continuation, entitlementFilter, sort);
        list = (IEnumerable<AccountEntitlement>) PlatformAccountEntitlementService.ExtractAccountEntitlements((IEnumerable<UserLicense>) filteredEntitlements).WithIdentityRefs(requestContext);
        continuationToken = filteredEntitlements.ContinuationToken;
      }
      return new PagedAccountEntitlements(list, continuationToken);
    }

    private AccountEntitlementFilter FilterUsersOnly(AccountEntitlementFilter filterOptions)
    {
      if (filterOptions == null)
      {
        filterOptions = new AccountEntitlementFilter();
      }
      else
      {
        filterOptions.UserTypes.Remove(IdentityMetaType.Application);
        filterOptions.UserTypes.Remove(IdentityMetaType.ManagedIdentity);
      }
      if (filterOptions.UserTypes.Any<IdentityMetaType>())
        return filterOptions;
      filterOptions.UserTypes.Add(IdentityMetaType.Unknown);
      filterOptions.UserTypes.Add(IdentityMetaType.Guest);
      filterOptions.UserTypes.Add(IdentityMetaType.Member);
      return filterOptions;
    }

    public PagedAccountEntitlements SearchMemberAccountEntitlements(
      IVssRequestContext requestContext,
      string continuation,
      string filter,
      string orderBy)
    {
      return this.SearchAccountEntitlements(requestContext, continuation, filter, orderBy, true);
    }

    private IList<AccountEntitlement> GetAccountEntitlementsInternal(
      IVssRequestContext requestContext,
      int? top = null,
      int skip = 0)
    {
      this.ValidateCollectionContext(requestContext);
      SecurityUtil.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      List<AccountEntitlement> accountEntitlements = PlatformAccountEntitlementService.ExtractAccountEntitlements(top.HasValue ? (IEnumerable<UserLicense>) this.m_licensingRepository.GetEntitlements(requestContext, top.Value, skip) : (IEnumerable<UserLicense>) this.m_licensingRepository.GetEntitlements(requestContext));
      if (requestContext.IsFeatureEnabled("VisualStudio.LicensingService.GetAccountEntitlements"))
      {
        foreach (AccountEntitlement accountEntitlement in accountEntitlements)
          accountEntitlement.License = (License) AccountLicense.EarlyAdopter;
      }
      return (IList<AccountEntitlement>) accountEntitlements;
    }

    public AccountEntitlement GetAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      bool bypassCache = false,
      bool determineRights = true,
      bool createIfNotExists = true)
    {
      this.ValidateNonDeploymentContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      Guid userId1 = requestContext.GetUserId();
      requestContext.Trace(1030144, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", string.Format("Retrieving account entitlement for {0} by {1}", (object) userId, (object) userId1));
      if (userId1 != Guid.Empty && userId1 != userId)
        SecurityUtil.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      Guid userId2 = ValidationHelper.ValidateIdentityId(requestContext, userId, true).EnterpriseStorageKey(requestContext);
      return ((IInternalPlatformEntitlementService) this).GetAccountEntitlementForAccountUserInternal(requestContext, userId2, true, determineRights, createIfNotExists).Item1;
    }

    public IList<AccountEntitlement> GetAccountEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      bool bypassCache = false)
    {
      this.ValidateCollectionContext(requestContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userIds, nameof (userIds));
      AccountEntitlementServiceConfiguration serviceConfiguration = this.m_settingsManager.GetAccountEntitlementServiceConfiguration(requestContext);
      ArgumentUtility.CheckCollectionForMaxLength<Guid>((ICollection<Guid>) userIds, nameof (userIds), serviceConfiguration.UserEntitlementsBatchMaxSize);
      requestContext.TraceConditionally(1030145, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", (Func<string>) (() => string.Format("Retrieving account entitlements for User Ids{0} by {1}", (object) string.Join<Guid>(",", (IEnumerable<Guid>) userIds), (object) requestContext.GetUserId())));
      SecurityUtil.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      List<Guid> userIdsToFetchLicenseFromDb = requestContext.GetService<IdentityService>().ReadIdentitiesWithFallback(requestContext, (IEnumerable<Guid>) userIds).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (u => u.EnterpriseStorageKey(requestContext))).ToList<Guid>();
      requestContext.TraceConditionally(1030147, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", (Func<string>) (() => "Fetching Entitlements for following User Ids from DB " + string.Join<Guid>(",", (IEnumerable<Guid>) userIdsToFetchLicenseFromDb)));
      return (IList<AccountEntitlement>) this.m_licensingRepository.GetEntitlements(requestContext, (IList<Guid>) userIdsToFetchLicenseFromDb).Select<UserLicense, AccountEntitlement>((Func<UserLicense, AccountEntitlement>) (x => (AccountEntitlement) x)).ToList<AccountEntitlement>();
    }

    public IList<AccountEntitlement> ObtainAvailableAccountEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds)
    {
      throw new NotSupportedException();
    }

    public AccountEntitlement AssignAvailableAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      LicensingOrigin origin = LicensingOrigin.None)
    {
      return this.AssignAvailableAccountEntitlement(requestContext, userId, origin, AssignmentSource.Unknown, false);
    }

    private (bool hasMsdnLicense, License msdnLicense) TryGetMsdnLicense(
      IVssRequestContext requestContext,
      Guid userId,
      AccountEntitlement accountEntitlement = null,
      bool dontFallbackOnFailure = false)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.LicensingService.EnableAutoUpgradeLicenseFromMsdn"))
        return (false, License.None);
      License msdnLicense = MsdnLicensingAdapter.TranslateMsdnEntitlementsToMsdnLicense(this.GetUserMsdnEntitlements(requestContext, userId, accountEntitlement, dontFallbackOnFailure));
      return (msdnLicense != License.None, msdnLicense);
    }

    public AccountEntitlement AssignAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin = LicensingOrigin.None)
    {
      return this.AssignAccountEntitlement(requestContext, userId, license, origin, AssignmentSource.Unknown);
    }

    public void DeleteAccountEntitlement(IVssRequestContext requestContext, Guid userId)
    {
      this.ValidateCollectionContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      try
      {
        SecurityUtil.CheckPermission(requestContext, 8, LicensingSecurity.AccountEntitlementsToken);
        requestContext.TraceEnter(1030110, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", new object[1]
        {
          (object) userId
        }, nameof (DeleteAccountEntitlement));
        Guid userId1 = userId;
        Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        try
        {
          identity = ValidationHelper.ValidateIdentityId(requestContext, userId, false);
          userId1 = identity.EnterpriseStorageKey(requestContext);
        }
        catch (InvalidLicensingOperation ex)
        {
          requestContext.Trace(1030261, TraceLevel.Warning, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", string.Format("Given userId that was not found in identity, attempting to delete from licensing anyways. id: {0}", (object) userId));
        }
        requestContext.GetService<PlatformExtensionEntitlementService>().UnassignmentExtensionOnUserRemove(requestContext, userId1);
        UserLicense entitlement = this.m_licensingRepository.GetEntitlement(requestContext, userId1);
        if (entitlement == null)
          return;
        License license = License.GetLicense(entitlement.Source, entitlement.License);
        this.m_licensingRepository.DeleteEntitlement(requestContext, userId1);
        if (identity == null)
          return;
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        this.Publish<AccountEntitlementEvent>(requestContext, new AccountEntitlementEvent(instanceId, identity, license, AccountEntitlementEventKind.Removed, userId1));
        requestContext.GetService<ILicensingAuditService>().LogLicenseRemoved(requestContext, identity, license);
        if (!requestContext.LicensingTracingEnabled())
          return;
        TeamFoundationTracingService.TraceAccountUserLicensingChanges(instanceId, requestContext.ServiceHost.ParentServiceHost.InstanceId, requestContext.ServiceHost.HostType, userId, identity.Cuid(), 3, new int?(0), new int?(-1), "Deleted", "None", "-1", DateTime.MinValue, DateTime.MinValue, this.GetUtcNow(), "Deleted");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030118, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1030119, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", nameof (DeleteAccountEntitlement));
      }
    }

    public IList<AccountLicenseUsage> GetLicensesUsage(
      IVssRequestContext requestContext,
      bool bypassCache = false)
    {
      this.ValidateCollectionContext(requestContext);
      requestContext.TraceEnter(1030100, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", nameof (GetLicensesUsage));
      SecurityUtil.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      try
      {
        List<AccountLicenseUsage> results = new List<AccountLicenseUsage>();
        IDictionary<AccountLicenseType, int> accountLicensesUsageMap;
        IDictionary<MsdnLicenseType, int> msdnLicenseUsageMap;
        IDictionary<AccountLicenseType, int> accountInvalidLicensesMap;
        IDictionary<MsdnLicenseType, int> msdnInvalidLicensesMap;
        this.GetLicenseUsageMaps(requestContext, out accountLicensesUsageMap, out msdnLicenseUsageMap, out accountInvalidLicensesMap, out msdnInvalidLicensesMap);
        if (!CommerceUtil.IsAzCommV2ApiEnabled(requestContext))
        {
          this.GetAcountLicenseUsageLegacy(requestContext, results, accountLicensesUsageMap, accountInvalidLicensesMap);
        }
        else
        {
          Dictionary<AccountLicenseType, MeterUsage2GetResponse> dictionary = this.m_azCommerceServiceFactory(requestContext).GetLicenseMeterUsages(requestContext).Where<MeterUsage2GetResponse>((Func<MeterUsage2GetResponse, bool>) (u => CommerceUtil.GetMeterIdToAccountLicenseTypeMap().Keys.Contains<Guid>(u.MeterId))).ToDictionary<MeterUsage2GetResponse, AccountLicenseType>((Func<MeterUsage2GetResponse, AccountLicenseType>) (i => CommerceUtil.GetMeterIdToAccountLicenseTypeMap()[i.MeterId]));
          foreach (AccountLicenseType accountLicenseType in Enum.GetValues(typeof (AccountLicenseType)))
          {
            if (accountLicenseType != AccountLicenseType.None)
            {
              int usedCount = 0;
              if (accountLicensesUsageMap.ContainsKey(accountLicenseType))
                accountLicensesUsageMap.TryGetValue(accountLicenseType, out usedCount);
              int provisionedCount = usedCount;
              int pendingProvisionedCount = usedCount;
              if (accountLicenseType == AccountLicenseType.Stakeholder)
              {
                provisionedCount = int.MaxValue;
                pendingProvisionedCount = int.MaxValue;
              }
              else if (dictionary.ContainsKey(accountLicenseType))
              {
                MeterUsage2GetResponse usage2GetResponse;
                dictionary.TryGetValue(accountLicenseType, out usage2GetResponse);
                provisionedCount = Convert.ToInt32(usage2GetResponse.MaxQuantity);
                pendingProvisionedCount = Convert.ToInt32(usage2GetResponse.MaxQuantity);
              }
              int disabledCount = 0;
              if (accountInvalidLicensesMap.ContainsKey(accountLicenseType))
                accountInvalidLicensesMap.TryGetValue(accountLicenseType, out disabledCount);
              results.Add(new AccountLicenseUsage(new AccountUserLicense(LicensingSource.Account, (int) accountLicenseType), provisionedCount, usedCount, disabledCount, pendingProvisionedCount));
            }
          }
        }
        foreach (MsdnLicenseType msdnLicenseType in Enum.GetValues(typeof (MsdnLicenseType)))
        {
          if (msdnLicenseType != MsdnLicenseType.None)
          {
            int num = 0;
            if (msdnLicenseUsageMap.ContainsKey(msdnLicenseType))
              msdnLicenseUsageMap.TryGetValue(msdnLicenseType, out num);
            int disabledCount = 0;
            if (msdnInvalidLicensesMap.ContainsKey(msdnLicenseType))
              msdnInvalidLicensesMap.TryGetValue(msdnLicenseType, out disabledCount);
            results.Add(new AccountLicenseUsage(new AccountUserLicense(LicensingSource.Msdn, (int) msdnLicenseType), num, num, disabledCount, num));
          }
        }
        return (IList<AccountLicenseUsage>) results;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030108, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1030109, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", nameof (GetLicensesUsage));
      }
    }

    private void GetAcountLicenseUsageLegacy(
      IVssRequestContext requestContext,
      List<AccountLicenseUsage> results,
      IDictionary<AccountLicenseType, int> accountLicensesUsageMap,
      IDictionary<AccountLicenseType, int> accountInvalidLicensesMap)
    {
      IOfferSubscriptionService subscriptionService = this.m_offerSubscriptionServiceFactory(requestContext);
      IEnumerable<IOfferSubscription> offerSubscriptions1 = subscriptionService.GetOfferSubscriptions(requestContext);
      CommonUtil.ValidateOfferSubscriptions(requestContext, offerSubscriptions1);
      requestContext.Trace(1030113, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Subscription resources returned from Commerce: " + string.Join(", ", offerSubscriptions1.Select<IOfferSubscription, string>((Func<IOfferSubscription, string>) (e => string.Format("{0} | {1} | {2}", (object) e.OfferMeter?.Name, (object) e.IncludedQuantity, (object) e.CommittedQuantity)))));
      IEnumerable<IOfferSubscription> list1 = (IEnumerable<IOfferSubscription>) offerSubscriptions1.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => o.OfferMeter.IsLicenseCategory())).ToList<IOfferSubscription>();
      requestContext.Trace(1030114, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Filtered legacy subscriptions: " + string.Join(", ", list1.Select<IOfferSubscription, string>((Func<IOfferSubscription, string>) (e => string.Format("{0} | {1} | {2}", (object) e.OfferMeter?.Name, (object) e.IncludedQuantity, (object) e.CommittedQuantity)))));
      IEnumerable<IOfferSubscription> offerSubscriptions2 = subscriptionService.GetOfferSubscriptions(requestContext, true);
      CommonUtil.ValidateOfferSubscriptions(requestContext, offerSubscriptions2);
      IEnumerable<IOfferSubscription> list2 = (IEnumerable<IOfferSubscription>) offerSubscriptions2.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (m => m.OfferMeter.IsLicenseCategory())).ToList<IOfferSubscription>();
      Dictionary<string, IOfferSubscription> dictionary1 = list1.ToDictionary<IOfferSubscription, string>((Func<IOfferSubscription, string>) (x => x.OfferMeter.Name));
      Dictionary<string, IOfferSubscription> dictionary2 = list2.ToDictionary<IOfferSubscription, string>((Func<IOfferSubscription, string>) (x => x.OfferMeter.Name));
      Func<Dictionary<string, IOfferSubscription>, Dictionary<AccountLicenseType, IOfferSubscription>> func = (Func<Dictionary<string, IOfferSubscription>, Dictionary<AccountLicenseType, IOfferSubscription>>) (dict => PlatformAccountEntitlementService.GetOfferMeterToAccountLicenseMap(requestContext).ToList<KeyValuePair<string, OrderedAccountUserLicense>>().Where<KeyValuePair<string, OrderedAccountUserLicense>>((Func<KeyValuePair<string, OrderedAccountUserLicense>, bool>) (license => dict.ContainsKey(license.Key))).ToDedupedDictionary<KeyValuePair<string, OrderedAccountUserLicense>, AccountLicenseType, IOfferSubscription>((Func<KeyValuePair<string, OrderedAccountUserLicense>, AccountLicenseType>) (license => (AccountLicenseType) license.Value.License), (Func<KeyValuePair<string, OrderedAccountUserLicense>, IOfferSubscription>) (license => dict[license.Key])));
      Dictionary<AccountLicenseType, IOfferSubscription> dictionary3 = func(dictionary1);
      Dictionary<AccountLicenseType, IOfferSubscription> dictionary4 = func(dictionary2);
      foreach (AccountLicenseType accountLicenseType in Enum.GetValues(typeof (AccountLicenseType)))
      {
        if (accountLicenseType != AccountLicenseType.None)
        {
          int usedCount = 0;
          if (accountLicensesUsageMap.ContainsKey(accountLicenseType))
            accountLicensesUsageMap.TryGetValue(accountLicenseType, out usedCount);
          int provisionedCount = usedCount;
          int pendingProvisionedCount = usedCount;
          if (accountLicenseType == AccountLicenseType.Stakeholder)
          {
            provisionedCount = int.MaxValue;
            pendingProvisionedCount = int.MaxValue;
          }
          else
          {
            if (dictionary3.ContainsKey(accountLicenseType))
            {
              IOfferSubscription offerSubscription;
              dictionary3.TryGetValue(accountLicenseType, out offerSubscription);
              provisionedCount = offerSubscription.CommittedQuantity;
              pendingProvisionedCount = offerSubscription.CommittedQuantity;
            }
            if (dictionary4.ContainsKey(accountLicenseType))
            {
              IOfferSubscription offerSubscription;
              dictionary4.TryGetValue(accountLicenseType, out offerSubscription);
              pendingProvisionedCount = offerSubscription.CommittedQuantity;
            }
          }
          int disabledCount = 0;
          if (accountInvalidLicensesMap.ContainsKey(accountLicenseType))
            accountInvalidLicensesMap.TryGetValue(accountLicenseType, out disabledCount);
          results.Add(new AccountLicenseUsage(new AccountUserLicense(LicensingSource.Account, (int) accountLicenseType), provisionedCount, usedCount, disabledCount, pendingProvisionedCount));
        }
      }
    }

    public IList<UserLicenseCount> GetExplicitLicenseUsage(IVssRequestContext requestContext) => this.m_licensingRepository.GetUserLicenseUsage(requestContext);

    private void GetLicenseUsageMaps(
      IVssRequestContext requestContext,
      out IDictionary<AccountLicenseType, int> accountLicensesUsageMap,
      out IDictionary<MsdnLicenseType, int> msdnLicenseUsageMap,
      out IDictionary<AccountLicenseType, int> accountInvalidLicensesMap,
      out IDictionary<MsdnLicenseType, int> msdnInvalidLicensesMap)
    {
      IList<UserLicenseCount> source1 = this.GetExplicitLicenseUsage(requestContext) ?? (IList<UserLicenseCount>) new List<UserLicenseCount>();
      IEnumerable<UserLicenseCount> source2 = source1.Where<UserLicenseCount>((Func<UserLicenseCount, bool>) (lc =>
      {
        AccountUserLicense license = lc.License;
        return license != null && license.Source == LicensingSource.Account;
      }));
      accountLicensesUsageMap = (IDictionary<AccountLicenseType, int>) source2.Where<UserLicenseCount>((Func<UserLicenseCount, bool>) (lc => PlatformAccountEntitlementService.IsValidLicenseStatus(lc.UserStatus))).GroupBy<UserLicenseCount, AccountLicenseType>((Func<UserLicenseCount, AccountLicenseType>) (lc => (AccountLicenseType) lc.License.License)).ToDictionary<IGrouping<AccountLicenseType, UserLicenseCount>, AccountLicenseType, int>((Func<IGrouping<AccountLicenseType, UserLicenseCount>, AccountLicenseType>) (grouping => grouping.Key), (Func<IGrouping<AccountLicenseType, UserLicenseCount>, int>) (grouping => grouping.Sum<UserLicenseCount>((Func<UserLicenseCount, int>) (lc => lc.Count))));
      accountInvalidLicensesMap = (IDictionary<AccountLicenseType, int>) source2.Where<UserLicenseCount>((Func<UserLicenseCount, bool>) (lc => !PlatformAccountEntitlementService.IsValidLicenseStatus(lc.UserStatus))).GroupBy<UserLicenseCount, AccountLicenseType>((Func<UserLicenseCount, AccountLicenseType>) (lc => (AccountLicenseType) lc.License.License)).ToDictionary<IGrouping<AccountLicenseType, UserLicenseCount>, AccountLicenseType, int>((Func<IGrouping<AccountLicenseType, UserLicenseCount>, AccountLicenseType>) (grouping => grouping.Key), (Func<IGrouping<AccountLicenseType, UserLicenseCount>, int>) (grouping => grouping.Sum<UserLicenseCount>((Func<UserLicenseCount, int>) (lc => lc.Count))));
      IEnumerable<UserLicenseCount> source3 = source1.Where<UserLicenseCount>((Func<UserLicenseCount, bool>) (lc =>
      {
        AccountUserLicense license = lc.License;
        return license != null && license.Source == LicensingSource.Msdn;
      }));
      msdnLicenseUsageMap = (IDictionary<MsdnLicenseType, int>) source3.Where<UserLicenseCount>((Func<UserLicenseCount, bool>) (lc => PlatformAccountEntitlementService.IsValidLicenseStatus(lc.UserStatus))).GroupBy<UserLicenseCount, MsdnLicenseType>((Func<UserLicenseCount, MsdnLicenseType>) (lc => (MsdnLicenseType) lc.License.License)).ToDictionary<IGrouping<MsdnLicenseType, UserLicenseCount>, MsdnLicenseType, int>((Func<IGrouping<MsdnLicenseType, UserLicenseCount>, MsdnLicenseType>) (grouping => grouping.Key), (Func<IGrouping<MsdnLicenseType, UserLicenseCount>, int>) (grouping => grouping.Sum<UserLicenseCount>((Func<UserLicenseCount, int>) (lc => lc.Count))));
      msdnInvalidLicensesMap = (IDictionary<MsdnLicenseType, int>) source3.Where<UserLicenseCount>((Func<UserLicenseCount, bool>) (lc => !PlatformAccountEntitlementService.IsValidLicenseStatus(lc.UserStatus))).GroupBy<UserLicenseCount, MsdnLicenseType>((Func<UserLicenseCount, MsdnLicenseType>) (lc => (MsdnLicenseType) lc.License.License)).ToDictionary<IGrouping<MsdnLicenseType, UserLicenseCount>, MsdnLicenseType, int>((Func<IGrouping<MsdnLicenseType, UserLicenseCount>, MsdnLicenseType>) (grouping => grouping.Key), (Func<IGrouping<MsdnLicenseType, UserLicenseCount>, int>) (grouping => grouping.Sum<UserLicenseCount>((Func<UserLicenseCount, int>) (lc => lc.Count))));
    }

    private static bool IsValidLicenseStatus(AccountUserStatus status) => status == AccountUserStatus.Active || status == AccountUserStatus.Pending || status == AccountUserStatus.Expired;

    private DateTimeOffset UpdateUserLastAccessedDate(
      IVssRequestContext requestContext,
      Guid userId)
    {
      if (this.m_serviceCircuitBreakerSettings == null)
        this.m_serviceCircuitBreakerSettings = PlatformAccountEntitlementService.s_defaultCircuitbreakerProperties;
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "LicensingAndAccounts.").AndCommandKey((CommandKey) "UserLastAccessedDate").AndCommandPropertiesDefaults(this.m_serviceCircuitBreakerSettings);
      return new CommandService<DateTimeOffset>(requestContext.To(TeamFoundationHostType.Deployment), setter, (Func<DateTimeOffset>) (() => (DateTimeOffset) this.UpdateUserLastAccessedDateInternal(requestContext, userId)), (Func<DateTimeOffset>) (() => DateTimeOffset.MinValue)).Execute();
    }

    void IInternalPlatformEntitlementService.ReconcileLicenceUsage(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1030160, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "ReconcileLicenceUsage");
      try
      {
        this.ValidateCollectionContext(requestContext);
        new EntitlementProcessor(requestContext.Elevate()).Reconcile();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030168, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1030169, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "ReconcileLicenceUsage");
      }
    }

    public AccountEntitlement AssignAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource)
    {
      this.ValidateCollectionContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      ArgumentUtility.CheckForNull<License>(license, nameof (license));
      this.ValidateAssignmentSource(assignmentSource);
      AccountEntitlement accountEntitlement = this.GetAccountEntitlement(requestContext, userId, false, false, false);
      if (license == License.Auto)
        return this.AssignAvailableAccountEntitlement(requestContext, userId, origin, assignmentSource, false);
      try
      {
        requestContext.TraceEnter(1030190, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", new object[2]
        {
          (object) userId,
          (object) license
        }, nameof (AssignAccountEntitlement));
        Microsoft.VisualStudio.Services.Identity.Identity identity = ValidationHelper.ValidateIdentityId(requestContext, userId, true);
        ValidationHelper.ValidateLicenseIsAssignable(license);
        Guid userId1 = identity.EnterpriseStorageKey(requestContext);
        SecurityUtil.CheckPermission(requestContext, 16, LicensingSecurity.AccountEntitlementsToken);
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        if (license == License.None && !requestContext.IsSystemContext)
          throw new LicenseNotAvailableException(LicensingResources.UserCannotBeAssignedNoneLicense());
        AccountEntitlement entitlement = (AccountEntitlement) null;
        if (this.IsInternalHost(requestContext))
        {
          if (license != (License) AccountLicense.EarlyAdopter)
          {
            requestContext.Trace(1030249, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", string.Format("Assigning early adopter license to the user {0}", (object) userId));
            license = (License) AccountLicense.EarlyAdopter;
          }
          entitlement = this.AddLicensedUser(requestContext, userId1, license, AccountUserStatus.Pending, origin, assignmentSource);
        }
        else
        {
          if (license == (License) AccountLicense.EarlyAdopter)
            throw new LicenseNotAvailableException(LicensingResources.RequestedLicenseNotAvailable((object) license));
          if (license == (License) AccountLicense.Stakeholder && CollectionHelper.GetCollectionOwner(requestContext) == userId1)
            throw new LicenseNotAvailableException(LicensingResources.OwnerCannotBeAssignedStakeholderLicense());
          if (license == License.AutoLicense.Msdn)
          {
            License msdnLicenseForUser = ((IInternalPlatformEntitlementService) this).GetMsdnLicenseForUser(requestContext1, userId);
            if (msdnLicenseForUser == (License) null || msdnLicenseForUser == License.None)
              throw new LicenseNotAvailableException(LicensingResources.RequestedLicenseNotAvailable((object) license));
            license = (License) MsdnLicense.Eligible;
          }
          if (!new EntitlementProcessor(requestContext.Elevate()).Assign(userId1, license, origin, assignmentSource, out entitlement))
          {
            requestContext.TraceConditionally(1030196, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", (Func<string>) (() => string.Join(";", this.GetLicensesUsage(requestContext, false).Select<AccountLicenseUsage, string>((Func<AccountLicenseUsage, string>) (r => string.Format("{0} - Used Count : {1}, Provisioned Count : {2}", (object) r.License, (object) r.UsedCount, (object) r.ProvisionedCount))))));
            throw new LicenseNotAvailableException(LicensingResources.RequestedLicenseNotAvailable((object) license));
          }
        }
        if (entitlement != (AccountEntitlement) null)
        {
          entitlement.UserId = identity.Id;
          entitlement.Rights = new AccountRights(PlatformAccountEntitlementService.ExtractVisualStudioOnlineServiceLevel(entitlement.License));
          requestContext.TraceProperties<AccountEntitlement>(1030192, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", entitlement, (string) null);
          ILicensingAuditService service = requestContext.GetService<ILicensingAuditService>();
          if (accountEntitlement == (AccountEntitlement) null)
            service.LogLicenseAssigned(requestContext, identity, entitlement.License, assignmentSource);
          else
            service.LogLicenseModified(requestContext, identity, accountEntitlement.License, entitlement.License, assignmentSource);
        }
        else
          requestContext.Trace(1030197, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Null Entitlement being returned");
        return entitlement;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030198, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1030199, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", nameof (AssignAccountEntitlement));
      }
    }

    public AccountEntitlement AssignAccountEntitlementWithAutoUpgradeAndFallback(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource)
    {
      this.ValidateCollectionContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      ArgumentUtility.CheckForNull<License>(license, nameof (license));
      this.ValidateAssignmentSource(assignmentSource);
      (bool hasMsdnLicense, License msdnLicense) = this.TryGetMsdnLicense(requestContext, userId, dontFallbackOnFailure: true);
      if (hasMsdnLicense && msdnLicense > license)
        return this.AssignAccountEntitlement(requestContext, userId, (License) MsdnLicense.Eligible, origin, AssignmentSource.Unknown);
      try
      {
        return this.AssignAccountEntitlement(requestContext, userId, license, origin, assignmentSource);
      }
      catch (LicenseNotAvailableException ex)
      {
        AccountEntitlement accountEntitlement = this.GetAccountEntitlement(requestContext, userId, false, false, false);
        return accountEntitlement == (AccountEntitlement) null ? this.AssignAccountEntitlement(requestContext, userId, (License) AccountLicense.Stakeholder, origin, assignmentSource) : accountEntitlement;
      }
    }

    public AccountEntitlement AssignAvailableAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      bool overwriteExistingEntitlement)
    {
      this.ValidateCollectionContext(requestContext);
      this.ValidateAssignmentSource(assignmentSource);
      SecurityUtil.CheckPermission(requestContext, 16, LicensingSecurity.AccountEntitlementsToken);
      AccountEntitlement accountEntitlement1 = this.GetAccountEntitlement(requestContext, userId, false, false, false);
      if (accountEntitlement1 != (AccountEntitlement) null && !overwriteExistingEntitlement)
        return accountEntitlement1;
      License defaultLicense = this.GetDefaultLicense(requestContext);
      (bool hasMsdnLicense, License msdnLicense) = this.TryGetMsdnLicense(requestContext, userId, dontFallbackOnFailure: true);
      AccountEntitlement accountEntitlement2;
      if (hasMsdnLicense && msdnLicense > defaultLicense)
      {
        requestContext.Trace(1030232, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "User was detected to have an MSDN subscription of " + msdnLicense.ToString() + " which is better than the default license of " + defaultLicense.ToString() + ". Assigning Msdn-Eligible.");
        accountEntitlement2 = this.AssignAccountEntitlement(requestContext, userId, (License) MsdnLicense.Eligible, origin, AssignmentSource.Unknown);
      }
      else
      {
        Guid collectionOwner = CollectionHelper.GetCollectionOwner(requestContext);
        if (userId == collectionOwner)
        {
          requestContext.Trace(1030238, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Assigning Basic license to owner.");
          accountEntitlement2 = this.AssignDefaultLicenseWithFallback(requestContext, (License) AccountLicense.Express, userId, origin, assignmentSource);
        }
        else
        {
          requestContext.Trace(1030223, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Assigning default license " + defaultLicense.ToString() + ".");
          accountEntitlement2 = this.AssignDefaultLicenseWithFallback(requestContext, defaultLicense, userId, origin, assignmentSource);
        }
      }
      return accountEntitlement2;
    }

    AccountEntitlement IInternalPlatformEntitlementService.AssignAccountEntitlementInternal(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource)
    {
      this.ValidateCollectionContext(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity identity = ValidationHelper.ValidateIdentityId(requestContext, userId, true);
      SecurityUtil.CheckPermission(requestContext, 16, LicensingSecurity.AccountEntitlementsToken);
      Guid userId1 = identity.EnterpriseStorageKey(requestContext);
      UserLicense userLicense = this.m_licensingRepository.AssignEntitlement(requestContext, userId1, license, origin, assignmentSource, AccountUserStatus.Pending, identity.ToLicensedIdentity());
      Guid instanceId1 = requestContext.ServiceHost.InstanceId;
      this.Publish<AccountEntitlementEvent>(requestContext, new AccountEntitlementEvent(instanceId1, identity, License.GetLicense(userLicense.Source, userLicense.License), AccountEntitlementEventKind.Updated, userId1));
      requestContext.GetService<IExtensionEntitlementService>().EvaluateExtensionAssignmentsOnAccessLevelChange(requestContext, identity, license);
      if (requestContext.LicensingTracingEnabled())
      {
        Guid accountId = userLicense.AccountId;
        Guid instanceId2 = requestContext.ServiceHost.ParentServiceHost.InstanceId;
        int hostType = (int) requestContext.ServiceHost.HostType;
        Guid userID = userId;
        Guid userCUID = identity.Cuid();
        int status = (int) userLicense.Status;
        int? licenseSourceId = new int?((int) userLicense.Source);
        int? licenseId = new int?(userLicense.License);
        string userStatusName = userLicense.Status.ToString();
        string licenseSourceName = userLicense.Source.ToString();
        string licenseName = License.GetLicense(userLicense.Source, userLicense.License).ToString();
        DateTimeOffset dateTimeOffset = userLicense.AssignmentDate;
        DateTime dateTime1 = dateTimeOffset.DateTime;
        dateTimeOffset = userLicense.DateCreated;
        DateTime dateTime2 = dateTimeOffset.DateTime;
        DateTime utcNow = this.GetUtcNow();
        TeamFoundationTracingService.TraceAccountUserLicensingChanges(accountId, instanceId2, (TeamFoundationHostType) hostType, userID, userCUID, status, licenseSourceId, licenseId, userStatusName, licenseSourceName, licenseName, dateTime1, dateTime2, utcNow, "Assigned");
      }
      return (AccountEntitlement) userLicense;
    }

    public List<AccountEntitlement> GetPreviousAccountEntitlements(
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> userDescriptors)
    {
      this.ValidateCollectionContext(requestContext);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = ValidationHelper.ValidateIdentities(requestContext, userDescriptors, false);
      SecurityUtil.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      return PlatformAccountEntitlementService.ExtractAccountEntitlements((IEnumerable<UserLicense>) this.m_licensingRepository.GetPreviousUserEntitlements(requestContext, (IList<Guid>) source.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (i => i.EnterpriseStorageKey(requestContext))).ToList<Guid>()));
    }

    public List<AccountEntitlement> GetPreviousAccountEntitlements(
      IVssRequestContext requestContext,
      IEnumerable<Guid> userIds)
    {
      this.ValidateCollectionContext(requestContext);
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source = requestContext.GetService<IdentityService>().ReadIdentitiesWithFallback(requestContext, (IEnumerable<Guid>) userIds.ToList<Guid>());
      SecurityUtil.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      return PlatformAccountEntitlementService.ExtractAccountEntitlements((IEnumerable<UserLicense>) this.m_licensingRepository.GetPreviousUserEntitlements(requestContext, (IList<Guid>) source.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (i => i.EnterpriseStorageKey(requestContext))).ToList<Guid>()));
    }

    (AccountEntitlement, bool) IInternalPlatformEntitlementService.GetAccountEntitlementForAccountUserInternal(
      IVssRequestContext requestContext,
      Guid userId,
      bool transferIdentity,
      bool determineRights,
      bool createIfNotExists)
    {
      this.ValidateNonDeploymentContext(requestContext);
      bool flag = false;
      Microsoft.VisualStudio.Services.Identity.Identity identity = ValidationHelper.ValidateIdentityId(requestContext, userId, false);
      Guid userId1 = identity.EnterpriseStorageKey(requestContext);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
      {
        requestContext.Trace(1030219, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Requesting account entitlement at the enterprise level");
        return (this.GetOrgLevelOnDemandEntitlement(requestContext, userId), flag);
      }
      AccountEntitlement seedEntitlement = this.GetUserEntitlement(requestContext, userId1);
      if (seedEntitlement != (AccountEntitlement) null)
      {
        if (determineRights)
          (seedEntitlement, flag) = this.GetAccountRightsForEntitlement(requestContext, seedEntitlement, identity);
        if (transferIdentity)
          seedEntitlement.UserId = identity.Id;
      }
      else if (determineRights)
      {
        flag = true;
        if (CollectionHelper.GetCollectionOwner(requestContext) == userId)
          seedEntitlement = this.GetAccountOwnerServiceLevel(requestContext, userId, (AccountEntitlement) null);
        else if (createIfNotExists)
          seedEntitlement = this.TryApplyOnDemandLicensing(requestContext, userId, identity);
      }
      return (seedEntitlement, flag);
    }

    private AccountEntitlement GetOrgLevelOnDemandEntitlement(
      IVssRequestContext requestContext,
      Guid userId)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        return (AccountEntitlement) null;
      bool flag = this.IsInternalHost(requestContext);
      DateTime utcNow = this.GetUtcNow();
      return new AccountEntitlement()
      {
        AccountId = requestContext.ServiceHost.InstanceId,
        UserId = userId,
        License = flag ? (License) AccountLicense.EarlyAdopter : (License) AccountLicense.Stakeholder,
        AssignmentDate = (DateTimeOffset) utcNow,
        UserStatus = AccountUserStatus.Active,
        LastAccessedDate = (DateTimeOffset) utcNow,
        Rights = new AccountRights(flag ? VisualStudioOnlineServiceLevel.Advanced : VisualStudioOnlineServiceLevel.Stakeholder)
      };
    }

    License IInternalPlatformEntitlementService.GetMsdnLicenseForUser(
      IVssRequestContext requestContext,
      Guid userId)
    {
      requestContext.TraceEnter(1030220, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", new object[1]
      {
        (object) userId
      }, "GetMsdnLicenseForUser");
      try
      {
        if (requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          userId
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() != null)
        {
          License msdnLicense = this.TryGetMsdnLicense(requestContext, userId).msdnLicense;
          requestContext.Trace(1030221, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "GetMsdnLicenseForUser: Returning license {0} for user {1}", (object) msdnLicense.ToString(), (object) userId);
          return msdnLicense;
        }
      }
      catch (Exception ex)
      {
        LicensingTrace.Log.PlatformLicensingService_GetMsdnLicenseForUser_Catch(requestContext, ex);
      }
      requestContext.Trace(1030229, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "GetMsdnLicenseForUser: Returning license None for user {0}.", (object) userId);
      return License.None;
    }

    public virtual void ImportLicenses(
      IVssRequestContext requestContext,
      LicensingSnapshot licensingSnapshot)
    {
      SecurityUtil.CheckMigratePermission(requestContext);
      if (licensingSnapshot.UserLicenses == null)
        licensingSnapshot.UserLicenses = new List<UserLicense>();
      if (licensingSnapshot.PreviousUserLicenses == null)
        licensingSnapshot.PreviousUserLicenses = new List<UserLicense>();
      if (licensingSnapshot.UserExtensionLicenses == null)
        licensingSnapshot.UserExtensionLicenses = new List<UserExtensionLicense>();
      this.m_licensingRepository.ImportScope(requestContext, licensingSnapshot.ScopeId, licensingSnapshot.UserLicenses, licensingSnapshot.PreviousUserLicenses, licensingSnapshot.UserExtensionLicenses);
    }

    public virtual LicensingSnapshot ExportLicenses(
      IVssRequestContext requestContext,
      int pageSize,
      int pageNo)
    {
      SecurityUtil.CheckMigratePermission(requestContext);
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Application);
      ILicensingRepository licensingRepository = LicensingRepositoryFactory<ApplicationLicensingRepository>.Create(requestContext1);
      List<UserLicense> userLicenseList = new List<UserLicense>();
      List<UserLicense> source1 = new List<UserLicense>();
      List<UserExtensionLicense> source2 = new List<UserExtensionLicense>();
      Guid instanceId1 = requestContext.ServiceHost.InstanceId;
      IVssRequestContext requestContext2 = requestContext1;
      Guid instanceId2 = requestContext.ServiceHost.InstanceId;
      ref List<UserLicense> local1 = ref userLicenseList;
      ref List<UserLicense> local2 = ref source1;
      ref List<UserExtensionLicense> local3 = ref source2;
      licensingRepository.GetScopeSnapshot(requestContext2, instanceId2, out local1, out local2, out local3);
      List<LicensingSnapshot> licensingSnapshotList = new List<LicensingSnapshot>();
      ILookup<Guid, UserLicense> lookup1 = source1.ToLookup<UserLicense, Guid, UserLicense>((Func<UserLicense, Guid>) (x => x.UserId), (Func<UserLicense, UserLicense>) (x => x));
      ILookup<Guid, UserExtensionLicense> lookup2 = source2.ToLookup<UserExtensionLicense, Guid, UserExtensionLicense>((Func<UserExtensionLicense, Guid>) (x => x.UserId), (Func<UserExtensionLicense, UserExtensionLicense>) (x => x));
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      List<UserLicense> licenses = new List<UserLicense>();
      List<UserLicense> previousLicenses = new List<UserLicense>();
      List<UserExtensionLicense> extLicenses = new List<UserExtensionLicense>();
      foreach (UserLicense userLicense in userLicenseList)
      {
        if (pageSize > 0 && num1 == pageSize)
        {
          licensingSnapshotList.Add(new LicensingSnapshot(instanceId1, licenses, previousLicenses, extLicenses));
          num1 = 0;
          licenses = new List<UserLicense>();
          previousLicenses = new List<UserLicense>();
          extLicenses = new List<UserExtensionLicense>();
        }
        licenses.Add(userLicense);
        ++num1;
        if (lookup1.Contains(userLicense.UserId))
        {
          previousLicenses.AddRange(lookup1[userLicense.UserId]);
          ++num3;
        }
        if (lookup2.Contains(userLicense.UserId))
        {
          extLicenses.AddRange(lookup2[userLicense.UserId]);
          num2 += lookup2[userLicense.UserId].Count<UserExtensionLicense>();
        }
      }
      licensingSnapshotList.Add(new LicensingSnapshot(requestContext.ServiceHost.InstanceId, licenses, previousLicenses, extLicenses, new int?(userLicenseList.Count), new int?(num3), new int?(num2)));
      return pageNo < licensingSnapshotList.Count ? licensingSnapshotList[pageNo] : (LicensingSnapshot) null;
    }

    public virtual void RemoveAllLicenses(IVssRequestContext requestContext)
    {
      SecurityUtil.CheckMigratePermission(requestContext);
      this.m_licensingRepository.DeleteScope(requestContext, requestContext.ServiceHost.InstanceId);
    }

    private License GetDefaultLicense(IVssRequestContext requestContext)
    {
      IOfferSubscriptionService subscriptionService = this.m_offerSubscriptionServiceFactory(requestContext);
      AzCommerceService azCommerceService = this.m_azCommerceServiceFactory(requestContext);
      try
      {
        AccountLicenseType license;
        if (!CommerceUtil.IsAzCommV2ApiEnabled(requestContext))
        {
          license = subscriptionService.GetDefaultLicenseLevel(requestContext, requestContext.ServiceHost.InstanceId);
          requestContext.Trace(1030237, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", string.Format("License returned from commerce service: {0}.", (object) license));
        }
        else
          license = azCommerceService.GetDefaultLicenseType(requestContext);
        return AccountLicense.GetLicense(license);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030233, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
    }

    internal (AccountEntitlement, bool) GetAccountRightsForEntitlement(
      IVssRequestContext requestContext,
      AccountEntitlement seedEntitlement,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      this.ValidateNonDeploymentContext(requestContext);
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (seedEntitlement));
      requestContext.TraceEnter(1030130, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", new object[1]
      {
        (object) seedEntitlement
      }, nameof (GetAccountRightsForEntitlement));
      SecurityUtil.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      if (requestContext.IsFeatureEnabled("VisualStudio.LicensingService.GetUserAccountRights"))
      {
        requestContext.Trace(1030143, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Due to feature flag override, returning AdvancedPlus right away without any computation.");
        seedEntitlement.Rights = new AccountRights(VisualStudioOnlineServiceLevel.AdvancedPlus);
        return (seedEntitlement, false);
      }
      try
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && this.IsRightsUpdateRequiredForUser(requestContext.GetUserIdentity(), seedEntitlement))
        {
          if (requestContext.IsFeatureEnabled("VisualStudio.LicensingService.EnableAutoUpgradeLicenseFromMsdn") && seedEntitlement != (AccountEntitlement) null && seedEntitlement.License != (License) null && seedEntitlement.License.Source == LicensingSource.Account && (seedEntitlement.License == (License) AccountLicense.Stakeholder || seedEntitlement.License == (License) AccountLicense.Express || seedEntitlement.License == (License) AccountLicense.Professional && requestContext.IsFeatureEnabled("VisualStudio.LicensingService.EnableAutoUpgradeFromProfessionalToMsdn") || seedEntitlement.License == (License) AccountLicense.Advanced))
          {
            AccountEntitlement updatedEntitlement;
            int num = (int) this.SynchronizeMsdnEntitlement(requestContext, seedEntitlement, out updatedEntitlement, true);
            if (updatedEntitlement != (AccountEntitlement) null)
              seedEntitlement = updatedEntitlement;
          }
          AccountEntitlement entitlement = this.GetUserRights(requestContext, seedEntitlement);
          Guid userId1 = seedEntitlement == (AccountEntitlement) null ? Guid.Empty : seedEntitlement.UserId;
          if (this.ShouldCheckForAccountOwnership(requestContext, entitlement) && userId1 == CollectionHelper.GetCollectionOwner(requestContext))
            entitlement = this.GetAccountOwnerServiceLevel(requestContext, userId1, seedEntitlement);
          if (!requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.CheckUserStatusBeforeUpdatingLastAccessDate") || !entitlement.UserStatus.IsDisabled())
            entitlement.LastAccessedDate = this.UpdateUserLastAccessedDate(requestContext, seedEntitlement.UserId);
          if (PlatformAccountEntitlementService.ShouldTraceActiveLicense(requestContext, seedEntitlement, identity))
          {
            Guid instanceId = requestContext.ServiceHost.InstanceId;
            Guid userCuid = identity.Cuid();
            Guid userId2 = seedEntitlement.UserId;
            string license = seedEntitlement.License.ToString();
            DateTime utcDateTime1 = seedEntitlement.AssignmentDate.UtcDateTime;
            DateTimeOffset dateTimeOffset = seedEntitlement.DateCreated;
            DateTime utcDateTime2 = dateTimeOffset.UtcDateTime;
            dateTimeOffset = seedEntitlement.LastAccessedDate;
            DateTime utcDateTime3 = dateTimeOffset.UtcDateTime;
            TeamFoundationTracingService.TraceActiveLicenses(instanceId, userCuid, userId2, license, utcDateTime1, utcDateTime2, utcDateTime3);
          }
          requestContext.Trace(1030137, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", string.Format("Computed AccountRights: Level: {0}, Reason: {1}", entitlement.Rights == null ? (object) "null" : (object) entitlement.Rights.Level.ToString(), entitlement.Rights == null ? (object) "null" : (object) entitlement.Rights.Reason));
          return (entitlement, true);
        }
        seedEntitlement = this.AssignVisualStudioOnlineServiceLevelForEntitlement(requestContext, seedEntitlement);
        return (seedEntitlement, false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030138, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1030139, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", nameof (GetAccountRightsForEntitlement));
      }
    }

    internal virtual bool ShouldCheckForAccountOwnership(
      IVssRequestContext requestContext,
      AccountEntitlement entitlement)
    {
      AccountRights rights1 = entitlement.Rights;
      int num;
      if ((rights1 != null ? (rights1.Level == VisualStudioOnlineServiceLevel.Stakeholder ? 1 : 0) : 0) == 0)
      {
        AccountRights rights2 = entitlement.Rights;
        num = rights2 != null ? (rights2.Level == VisualStudioOnlineServiceLevel.None ? 1 : 0) : 0;
      }
      else
        num = 1;
      bool flag = (object) entitlement != null && entitlement.UserStatus == AccountUserStatus.Active;
      if (num == 0 & flag)
        return false;
      if ((requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? 1 : (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? 1 : 0)) != 0)
        return true;
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        entitlement.UserId
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (readIdentity == null)
      {
        requestContext.Trace(1030168, TraceLevel.Error, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", string.Format("Failed to find identity {0}", (object) entitlement.UserId));
        return true;
      }
      return service.IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, readIdentity.Descriptor);
    }

    private AccountEntitlement AddLicensedUser(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      AccountUserStatus userStatus,
      LicensingOrigin origin,
      AssignmentSource assignmentSource)
    {
      this.ValidateCollectionContext(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity identity = ValidationHelper.ValidateIdentityId(requestContext, userId, true);
      Guid userId1 = identity.EnterpriseStorageKey(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      PlatformAccountMembershipService service = requestContext.GetService<PlatformAccountMembershipService>();
      Guid userId2 = userId1;
      AccountUser accountUser = new AccountUser(instanceId, userId2)
      {
        UserStatus = userStatus
      };
      service.AddUserToAccount(requestContext.Elevate(), accountUser, license, origin, assignmentSource, identity.ToLicensedIdentity());
      try
      {
        return ((IInternalPlatformEntitlementService) this).AssignAccountEntitlementInternal(requestContext, userId1, license, origin, assignmentSource);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030140, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
        service.RemoveUserFromAccount(requestContext.Elevate(), accountUser);
        throw new LicensingOperationFailException();
      }
    }

    private DateTime UpdateUserLastAccessedDateInternal(
      IVssRequestContext requestContext,
      Guid userId)
    {
      this.ValidateCollectionContext(requestContext);
      requestContext.TraceEnter(1030180, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", new object[1]
      {
        (object) userId
      }, nameof (UpdateUserLastAccessedDateInternal));
      try
      {
        DateTime utcNow = this.GetUtcNow();
        this.m_licensingRepository.UpdateUserLastAccessed(requestContext, userId, (DateTimeOffset) utcNow);
        requestContext.Trace(1030184, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Updated user:{0} last accessed date to: {1}  on the Account:{2}", (object) userId, (object) utcNow.ToString(), (object) requestContext.ServiceHost.InstanceId);
        return utcNow;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030188, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1030189, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", nameof (UpdateUserLastAccessedDateInternal));
      }
    }

    private static List<AccountEntitlement> ExtractAccountEntitlements(
      IEnumerable<UserLicense> userLicenses)
    {
      return userLicenses.Where<UserLicense>((Func<UserLicense, bool>) (userLicense => userLicense != null)).Select<UserLicense, AccountEntitlement>((Func<UserLicense, AccountEntitlement>) (x => (AccountEntitlement) x)).ToList<AccountEntitlement>();
    }

    internal virtual AccountEntitlement GetUserEntitlement(
      IVssRequestContext requestContext,
      Guid userId)
    {
      return (AccountEntitlement) this.m_licensingRepository.GetEntitlement(requestContext, userId);
    }

    internal List<MsdnEntitlement> GetUserMsdnEntitlements(
      IVssRequestContext requestContext,
      Guid userId,
      AccountEntitlement accountEntitlement = null,
      bool dontFallbackOnFailure = false)
    {
      MsdnLicensingAdapter adapter = requestContext.To(TeamFoundationHostType.Deployment).GetService<PlatformLicensingService>().GetAdapter<MsdnLicensingAdapter>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = ValidationHelper.ValidateIdentityId(requestContext, userId, false);
      IVssRequestContext requestContext1 = requestContext;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = identity;
      LicensingRequestType? requestType = new LicensingRequestType?(LicensingRequestType.All);
      License license = accountEntitlement?.License;
      int num = dontFallbackOnFailure ? 1 : 0;
      return adapter.GetEntitlementsForIdentity(requestContext1, userIdentity, requestType, license, num != 0);
    }

    internal virtual VisualStudioOnlineServiceLevel SynchronizeMsdnEntitlement(
      IVssRequestContext requestContext,
      AccountEntitlement accountEntitlement,
      out AccountEntitlement updatedEntitlement,
      bool dontUseFallback = false)
    {
      requestContext.TraceEnter(1030150, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", new object[2]
      {
        (object) accountEntitlement,
        (object) dontUseFallback
      }, nameof (SynchronizeMsdnEntitlement));
      updatedEntitlement = accountEntitlement;
      VisualStudioOnlineServiceLevel onlineServiceLevel = PlatformAccountEntitlementService.ExtractVisualStudioOnlineServiceLevel(accountEntitlement.License);
      try
      {
        List<MsdnEntitlement> msdnEntitlements = this.GetUserMsdnEntitlements(requestContext, accountEntitlement.UserId, accountEntitlement, dontUseFallback);
        License msdnLicense = MsdnLicensingAdapter.TranslateMsdnEntitlementsToMsdnLicense(msdnEntitlements);
        if (accountEntitlement.UserStatus == AccountUserStatus.Active && accountEntitlement.License != (License) null && accountEntitlement.License.Source == LicensingSource.Msdn)
          this.CheckForInvalidMsdnEntitlement(requestContext, (IReadOnlyList<MsdnEntitlement>) msdnEntitlements, msdnLicense);
        if (msdnLicense != License.None && msdnLicense >= accountEntitlement.License)
        {
          requestContext.Trace(1030152, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Fetched Msdn License For User:{0} , License:{1}", (object) accountEntitlement.UserId.ToString(), (object) msdnLicense);
          if (accountEntitlement.License != msdnLicense)
          {
            requestContext.Trace(1030151, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Updating Msdn License For User:{0} , From:{1} To:{2}", (object) accountEntitlement.UserId.ToString(), (object) accountEntitlement.License, (object) msdnLicense);
            updatedEntitlement = ((IInternalPlatformEntitlementService) this).AssignAccountEntitlementInternal(requestContext.Elevate(), accountEntitlement.UserId, msdnLicense, LicensingOrigin.None, AssignmentSource.Unknown);
            requestContext.GetService<ILicensingAuditService>().LogLicenseModified(requestContext, accountEntitlement.UserId.ToSubjectDescriptor(requestContext), accountEntitlement.License, msdnLicense, AssignmentSource.Unknown);
          }
          onlineServiceLevel = PlatformAccountEntitlementService.ExtractVisualStudioOnlineServiceLevel(msdnLicense);
          requestContext.GetService<PlatformExtensionEntitlementService>().SynchronizeMsdnExtensions(requestContext, msdnEntitlements, accountEntitlement.UserId);
        }
        else if (accountEntitlement.License != msdnLicense)
        {
          if (requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableTracingVsLicenseDataForPossibleDowngrade"))
          {
            \u003C\u003Ef__AnonymousType13<string, string>[] dataArray;
            if (msdnEntitlements == null)
            {
              dataArray = null;
            }
            else
            {
              IEnumerable<MsdnEntitlement> source1 = msdnEntitlements.Where<MsdnEntitlement>((Func<MsdnEntitlement, bool>) (e => string.Equals("Active", e.SubscriptionStatus, StringComparison.OrdinalIgnoreCase) || string.Equals("Grace", e.SubscriptionStatus, StringComparison.OrdinalIgnoreCase)));
              if (source1 == null)
              {
                dataArray = null;
              }
              else
              {
                IEnumerable<\u003C\u003Ef__AnonymousType13<string, string>> source2 = source1.Select(e => new
                {
                  EntitlementCode = e.EntitlementCode,
                  SubscriptionId = e.SubscriptionId
                });
                dataArray = source2 != null ? source2.ToArray() : null;
              }
            }
            \u003C\u003Ef__AnonymousType13<string, string>[] source = dataArray;
            requestContext.TraceAlways(1030156, TraceLevel.Warning, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "User '{0}' getting '{1}' from MSDN; previously had '{2}'; MSDN returned '{3}' count. Licensing Entitlements = '{4}' : subscription IDs = {5}", (object) accountEntitlement.UserId.ToString(), (object) msdnLicense, (object) accountEntitlement.License, (object) msdnEntitlements.Count, source != null ? (object) string.Join(":", source.Select(le => le.EntitlementCode)) : (object) "(No Active or Grace entitlements found)", source != null ? (object) string.Join(":", source.Select(le => le.SubscriptionId)) : (object) "(No Active or Grace entitlements found)");
          }
          if (requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableVsLicenseDowngrade"))
          {
            if ((object) accountEntitlement != null)
            {
              LicensingSource? source = accountEntitlement.License?.Source;
              LicensingSource licensingSource = LicensingSource.Msdn;
              if (source.GetValueOrDefault() == licensingSource & source.HasValue)
              {
                if (accountEntitlement?.License != (License) MsdnLicense.Eligible)
                {
                  if (msdnLicense == License.None)
                  {
                    requestContext.Trace(1030162, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Returning temporary statkeholder for msdn user {0}", (object) accountEntitlement.UserId);
                    onlineServiceLevel = VisualStudioOnlineServiceLevel.Stakeholder;
                  }
                }
              }
            }
          }
        }
      }
      catch (MsdnServiceUnavailableException ex)
      {
        requestContext.TraceException(1030157, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", (Exception) ex);
        requestContext.Trace(1030154, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", string.Format("MsdnServiceUnavailableException: ExtractedVSOServiceLevel for License={0} is {1}", (object) accountEntitlement.License, (object) onlineServiceLevel));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030158, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
        requestContext.Trace(1030155, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", string.Format("Exception: ExtractedVSOServiceLevel for License={0} is {1}", (object) accountEntitlement.License, (object) onlineServiceLevel));
      }
      finally
      {
        requestContext.TraceLeave(1030159, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", nameof (SynchronizeMsdnEntitlement));
      }
      if (requestContext.IsFeatureEnabled("VisualStudio.LicensingService.ReturnHighestAccountRightsForMsdnEligible") && accountEntitlement.License == (License) MsdnLicense.Eligible && (onlineServiceLevel == VisualStudioOnlineServiceLevel.None || onlineServiceLevel == VisualStudioOnlineServiceLevel.Stakeholder))
      {
        requestContext.Trace(1030270, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", string.Format("Due to feature flag override returning AdvancedPlus service level for user: {0}", (object) requestContext.GetUserIdentity().Id));
        onlineServiceLevel = VisualStudioOnlineServiceLevel.AdvancedPlus;
      }
      return onlineServiceLevel;
    }

    private void CheckForInvalidMsdnEntitlement(
      IVssRequestContext requestContext,
      IReadOnlyList<MsdnEntitlement> msdnEntitlements,
      License extractedLicense)
    {
      requestContext.TraceEnter(1030265, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", nameof (CheckForInvalidMsdnEntitlement));
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (MsdnEntitlement msdnEntitlement in (IEnumerable<MsdnEntitlement>) msdnEntitlements)
        {
          if (!MsdnLicensingAdapter.IsActiveEntitlement(msdnEntitlement))
            stringBuilder.AppendLine(msdnEntitlement.ToString());
        }
        string str = stringBuilder.ToString();
        if (string.IsNullOrEmpty(str) && !(extractedLicense == License.None))
          return;
        requestContext.TraceAlways(1030266, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", string.Format("User with VSID {0} is receiving a possibly wrong", (object) requestContext.GetUserIdentity().Id) + string.Format(" configuration of the Msdn License. Extracted license from received MsdnEntitlements: {0};", (object) extractedLicense) + " Inactive MsdnEntitlements:" + Environment.NewLine + str);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030267, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
      }
      finally
      {
        requestContext.TraceLeave(1030268, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", nameof (CheckForInvalidMsdnEntitlement));
      }
    }

    private void Publish<TEventArgs>(IVssRequestContext requestContext, TEventArgs eventArgs) => requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) eventArgs);

    private bool IsRightsUpdateRequiredForUser(Microsoft.VisualStudio.Services.Identity.Identity user, AccountEntitlement seedEntitlement)
    {
      if (seedEntitlement.UserStatus == AccountUserStatus.Pending || seedEntitlement.UserStatus == AccountUserStatus.Disabled || seedEntitlement.UserStatus == AccountUserStatus.PendingDisabled || seedEntitlement.UserStatus == AccountUserStatus.Active && seedEntitlement.License == (License) MsdnLicense.Eligible || seedEntitlement.AssignmentDate > seedEntitlement.LastAccessedDate)
        return true;
      TimeSpan timeSpan = new TimeSpan(new Random(user.Id.GetHashCode()).Next(8, 12), 0, 0);
      return DateTimeOffset.UtcNow - seedEntitlement.LastAccessedDate > timeSpan;
    }

    private AccountEntitlement GetUserRights(
      IVssRequestContext requestContext,
      AccountEntitlement seedEntitlement)
    {
      this.ValidateCollectionContext(requestContext);
      AccountEntitlement accountEntitlement = seedEntitlement;
      if (accountEntitlement.UserStatus != AccountUserStatus.Active && accountEntitlement.UserStatus != AccountUserStatus.Pending && !(accountEntitlement.License is MsdnLicense))
        return this.GetAccountErrorMessage(accountEntitlement);
      AccountRights accountRights;
      if (accountEntitlement.License is AccountLicense)
      {
        accountEntitlement = this.AssignVisualStudioOnlineServiceLevelForEntitlement(requestContext, accountEntitlement);
        accountRights = accountEntitlement.Rights;
      }
      else if (accountEntitlement.License is MsdnLicense)
      {
        AccountEntitlement updatedEntitlement = (AccountEntitlement) null;
        accountRights = this.GetAccountRightsForMsdnLicense(requestContext, accountEntitlement, out updatedEntitlement);
        if (accountEntitlement != updatedEntitlement)
          accountEntitlement = updatedEntitlement;
      }
      else if (this.IsInternalHost(requestContext))
      {
        VisualStudioOnlineServiceLevel advancedLicense = VisualStudioOnlineServiceLevel.Advanced;
        requestContext.TraceConditionally(1030142, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", (Func<string>) (() => string.Format("Creating AccountRights = {0} for User = {1} in internal account = {2} when entitlement license is of type AccountLicense", (object) advancedLicense.ToString(), (object) accountEntitlement.UserId, (object) requestContext.ServiceHost.InstanceId)));
        accountRights = new AccountRights(advancedLicense);
      }
      else
        accountRights = new AccountRights(VisualStudioOnlineServiceLevel.None, LicensingResources.NoLicenseFound());
      PlatformAccountEntitlementService.UpdateUserStatusIfRequired(requestContext, accountRights, accountEntitlement);
      accountEntitlement.Rights = accountRights;
      return accountEntitlement;
    }

    private AccountRights GetAccountRightsForMsdnLicense(
      IVssRequestContext requestContext,
      AccountEntitlement accountEntitlement,
      out AccountEntitlement updatedEntitlement)
    {
      VisualStudioOnlineServiceLevel level = this.SynchronizeMsdnEntitlement(requestContext, accountEntitlement, out updatedEntitlement);
      switch (level)
      {
        case VisualStudioOnlineServiceLevel.None:
        case VisualStudioOnlineServiceLevel.Stakeholder:
          return new AccountRights(VisualStudioOnlineServiceLevel.Stakeholder, LicensingResources.NoMSDNSubscription());
        default:
          return new AccountRights(level);
      }
    }

    internal virtual AccountEntitlement AssignVisualStudioOnlineServiceLevelForEntitlement(
      IVssRequestContext requestContext,
      AccountEntitlement accountEntitlement)
    {
      AccountRights accountRights;
      if (this.IsInternalHost(requestContext))
      {
        VisualStudioOnlineServiceLevel advancedLicense = VisualStudioOnlineServiceLevel.Advanced;
        requestContext.TraceConditionally(1030141, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", (Func<string>) (() => string.Format("Creating AccountRights = {0} for User = {1} in internal account = {2} when entitlement license is of type AccountLicense", (object) advancedLicense.ToString(), (object) accountEntitlement.UserId, (object) requestContext.ServiceHost.InstanceId)));
        accountRights = new AccountRights(advancedLicense);
      }
      else
      {
        if (accountEntitlement.License == (License) AccountLicense.EarlyAdopter)
        {
          requestContext.Trace(1030132, TraceLevel.Warning, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Expected: Account user {0} for account {1} does not have any licensing rights\r\n                        because earlyAdopter is not a valid license any more for this account, assigning available will be called for this user.", (object) accountEntitlement.UserId, (object) requestContext.ServiceHost.InstanceId);
          try
          {
            accountEntitlement = this.AssignAvailableAccountEntitlement(requestContext, accountEntitlement.UserId, LicensingOrigin.None);
          }
          catch (Exception ex)
          {
            accountEntitlement.License = License.None;
            requestContext.TraceException(1030133, TraceLevel.Error, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
          }
        }
        VisualStudioOnlineServiceLevel onlineServiceLevel = PlatformAccountEntitlementService.ExtractVisualStudioOnlineServiceLevel(accountEntitlement.License);
        accountRights = onlineServiceLevel != VisualStudioOnlineServiceLevel.None ? new AccountRights(onlineServiceLevel) : new AccountRights(VisualStudioOnlineServiceLevel.Stakeholder);
      }
      accountEntitlement.Rights = accountRights;
      return accountEntitlement;
    }

    private AccountEntitlement GetAccountErrorMessage(AccountEntitlement accountEntitlement)
    {
      AccountRights accountRights = (AccountRights) null;
      bool flag = accountEntitlement.License is AccountLicense && accountEntitlement.License == (License) AccountLicense.EarlyAdopter;
      if (accountEntitlement.UserStatus != AccountUserStatus.Active && accountEntitlement.UserStatus != AccountUserStatus.Pending)
      {
        switch (accountEntitlement.UserStatus)
        {
          case AccountUserStatus.Disabled:
            accountRights = new AccountRights(VisualStudioOnlineServiceLevel.Stakeholder, flag ? LicensingResources.EarlyAdopterLicenseExpired() : LicensingResources.UserLicenseExpired());
            break;
          case AccountUserStatus.Deleted:
            accountRights = new AccountRights(VisualStudioOnlineServiceLevel.None, LicensingResources.UserIsDeletedFromAccount());
            break;
          case AccountUserStatus.Expired:
            accountRights = new AccountRights(VisualStudioOnlineServiceLevel.Stakeholder, LicensingResources.UserLicenseExpired());
            break;
          case AccountUserStatus.PendingDisabled:
            accountRights = new AccountRights(VisualStudioOnlineServiceLevel.Stakeholder, flag ? LicensingResources.EarlyAdopterLicenseExpired() : LicensingResources.UserLicenseExpired());
            break;
          default:
            accountRights = new AccountRights(VisualStudioOnlineServiceLevel.None);
            break;
        }
      }
      accountEntitlement.Rights = accountRights;
      return accountEntitlement;
    }

    private static void UpdateUserStatusIfRequired(
      IVssRequestContext requestContext,
      AccountRights accountRights,
      AccountEntitlement accountEntitlement)
    {
      bool flag = false;
      AccountUserStatus accountUserStatus = AccountUserStatus.None;
      if (accountRights == null || accountEntitlement == (AccountEntitlement) null || accountEntitlement.License == (License) null)
        return;
      if (accountRights.Level == VisualStudioOnlineServiceLevel.None)
      {
        if (accountEntitlement.License is MsdnLicense)
        {
          requestContext.Trace(1030136, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Failed to get Msdn entitlement For MsdnLicense.Eligible UserId:{0} For Account:{1}", (object) accountEntitlement.UserId, (object) accountEntitlement.AccountId);
          flag = false;
        }
        else
        {
          accountUserStatus = accountEntitlement.UserStatus == AccountUserStatus.Pending ? AccountUserStatus.PendingDisabled : AccountUserStatus.Disabled;
          requestContext.Trace(1030134, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Changing User Status to Disabled/Expired For UserId:{0} For Account:{1}", (object) accountEntitlement.UserId, (object) accountEntitlement.AccountId);
          flag = true;
        }
      }
      else if (accountRights.Level != VisualStudioOnlineServiceLevel.None && (accountEntitlement.UserStatus == AccountUserStatus.Pending || accountEntitlement.UserStatus == AccountUserStatus.PendingDisabled || accountEntitlement.UserStatus == AccountUserStatus.Disabled))
      {
        accountUserStatus = AccountUserStatus.Active;
        requestContext.Trace(1030135, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Changing User Status to Active For UserId:{0} For Account:{1}", (object) accountEntitlement.UserId, (object) accountEntitlement.AccountId);
        flag = true;
      }
      if (!flag || accountUserStatus == AccountUserStatus.None)
        return;
      if (!requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.DisableUpdatingUserStatusOnSeedEntitlement"))
        accountEntitlement.UserStatus = accountUserStatus;
      AccountUser accountUser = new AccountUser(accountEntitlement.AccountId, accountEntitlement.UserId)
      {
        UserStatus = accountUserStatus
      };
      requestContext.GetService<PlatformAccountMembershipService>().UpdateUserInAccount(requestContext.Elevate(), accountUser);
    }

    private AccountEntitlement GetAccountOwnerServiceLevel(
      IVssRequestContext requestContext,
      Guid userId,
      AccountEntitlement seedEntitlement)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = ValidationHelper.ValidateIdentityId(requestContext, userId, true);
      Guid userId1 = identity.EnterpriseStorageKey(requestContext);
      userId = userId1;
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      AccountEntitlement accountEntitlement = seedEntitlement;
      if (accountEntitlement == (AccountEntitlement) null || accountEntitlement.UserStatus != AccountUserStatus.Active)
      {
        AccountUser accountUser = new AccountUser(instanceId, userId)
        {
          UserStatus = AccountUserStatus.Active
        };
        PlatformAccountMembershipService service = requestContext.GetService<PlatformAccountMembershipService>();
        if (accountEntitlement == (AccountEntitlement) null)
        {
          requestContext.Trace(1030170, TraceLevel.Error, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Unexpected: Account owner {0} for account {1} is not a member of the account, adding it.", (object) userId, (object) instanceId);
          service.AddUserToAccount(requestContext.Elevate(), accountUser, License.None, identity.ToLicensedIdentity());
        }
        else
        {
          requestContext.Trace(1030171, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Activating account owner {0} for account {1} - may be signing in for the first time.", (object) userId, (object) instanceId);
          service.UpdateUserInAccount(requestContext.Elevate(), accountUser);
        }
        accountEntitlement = this.GetUserEntitlement(requestContext, userId);
      }
      VisualStudioOnlineServiceLevel level = VisualStudioOnlineServiceLevel.None;
      switch (accountEntitlement.License.Source)
      {
        case LicensingSource.Account:
          if (accountEntitlement.License != (License) AccountLicense.EarlyAdopter || this.IsInternalHost(requestContext))
          {
            level = PlatformAccountEntitlementService.ExtractVisualStudioOnlineServiceLevel(accountEntitlement.License);
            break;
          }
          break;
        case LicensingSource.Msdn:
          AccountEntitlement updatedEntitlement = (AccountEntitlement) null;
          level = this.SynchronizeMsdnEntitlement(requestContext, accountEntitlement, out updatedEntitlement);
          break;
        default:
          level = VisualStudioOnlineServiceLevel.None;
          break;
      }
      if (level == VisualStudioOnlineServiceLevel.None || level == VisualStudioOnlineServiceLevel.Stakeholder)
      {
        requestContext.Trace(1030172, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Unexpected: Account owner {0} for account {1} does not have any licensing rights, assigning available rights.", (object) userId, (object) instanceId);
        License license = new EntitlementProcessor(requestContext.Elevate()).AssignAvailable(userId, true);
        level = PlatformAccountEntitlementService.ExtractVisualStudioOnlineServiceLevel(license);
        accountEntitlement.License = license;
        this.Publish<AccountEntitlementEvent>(requestContext, new AccountEntitlementEvent(instanceId, identity, accountEntitlement.License, AccountEntitlementEventKind.Updated, userId1));
      }
      accountEntitlement.Rights = new AccountRights(level);
      return accountEntitlement;
    }

    private AccountEntitlement TryApplyOnDemandLicensing(
      IVssRequestContext requestContext,
      Guid userId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      try
      {
        requestContext.CheckProjectCollectionRequestContext();
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
        Guid guid = identity.EnterpriseStorageKey(requestContext);
        if (identity.Id != userId && guid != userId)
          throw new ArgumentException(string.Format("The specified parameters - userId {0} and identity {1} are not referring to the same identity", (object) userId, (object) identity));
        if (requestContext.GetService<IdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, identity.Descriptor))
        {
          requestContext.TraceAlways(1030242, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "User {0} was found to have group memberships but no license. Therefore trying to apply on-demand licensing for the user.", (object) userId);
          IdentityHelper.MaterializeUser(requestContext, (IVssIdentity) identity, nameof (TryApplyOnDemandLicensing));
          return LicenseAssignmentHelper.AssignLicenseToIdentity(requestContext.Elevate(), userId, requestContext.IsPublicResourceLicense(), true);
        }
      }
      catch (InvalidLicensingOperation ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030248, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", ex);
      }
      return (AccountEntitlement) null;
    }

    private AccountEntitlement AssignDefaultLicenseWithFallback(
      IVssRequestContext requestContext,
      License defaultLicense,
      Guid userId,
      LicensingOrigin origin = LicensingOrigin.None,
      AssignmentSource assignmentSource = AssignmentSource.None)
    {
      try
      {
        return this.AssignAccountEntitlement(requestContext, userId, defaultLicense, origin, assignmentSource);
      }
      catch (LicenseNotAvailableException ex)
      {
        requestContext.Trace(1030231, TraceLevel.Info, "VisualStudio.Services.PlatformAccountEntitlementService", "BusinessLogic", "Not enough licenses available. Falling back to assigning Stakeholder.");
        return this.AssignAccountEntitlement(requestContext, userId, (License) AccountLicense.Stakeholder, origin, assignmentSource);
      }
    }

    public virtual bool IsInternalHost(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.DisableInternalDesignation"))
        return false;
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      return context.GetService<IOrganizationPolicyService>().GetPolicy<bool>(context.Elevate(), "Policy.IsInternal", false).EffectiveValue;
    }

    internal static VisualStudioOnlineServiceLevel ExtractVisualStudioOnlineServiceLevel(
      License license)
    {
      long ruleKey = VisualStudioAccountLicensingAdapter.GenerateRuleKey(license.Source, license.GetLicenseAsInt32());
      EntitlementToRightsRule entitlementToRightsRule;
      return !VisualStudioAccountLicensingAdapter.RulesMap.TryGetValue(ruleKey, out entitlementToRightsRule) ? VisualStudioOnlineServiceLevel.None : entitlementToRightsRule.ServiceLevel;
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.m_dateTimeProvider.UtcNow;

    private static bool ShouldTraceActiveLicense(
      IVssRequestContext requestContext,
      AccountEntitlement entitlement,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      return requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableActiveLicensesTracing") && requestContext.LicensingTracingEnabled() && entitlement != (AccountEntitlement) null && identity != null && entitlement.UserStatus == AccountUserStatus.Active;
    }

    private void ValidateAssignmentSource(AssignmentSource assignmentSource)
    {
      if (assignmentSource == AssignmentSource.None)
        throw new ArgumentException(string.Format("AssignmentSource '{0}' is not supported", (object) assignmentSource));
    }

    private void ValidateCollectionContext(IVssRequestContext context)
    {
      context.CheckProjectCollectionRequestContext();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private void ValidateNonDeploymentContext(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(context.ServiceHost.HostType);
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private static IDictionary<string, OrderedAccountUserLicense> GetOfferMeterToAccountLicenseMap(
      IVssRequestContext requestContext)
    {
      return (IDictionary<string, OrderedAccountUserLicense>) new Dictionary<string, OrderedAccountUserLicense>()
      {
        {
          "Test Manager",
          new OrderedAccountUserLicense(LicensingSource.Account, 4, 1)
        },
        {
          "StandardLicense",
          new OrderedAccountUserLicense(LicensingSource.Account, 2, 2)
        },
        {
          "ProfessionalLicense",
          new OrderedAccountUserLicense(LicensingSource.Account, 3, 3)
        }
      };
    }
  }
}
