// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureResourceHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class AzureResourceHelper : IAzureResourceHelper, IVssFrameworkService
  {
    private List<string> azureEligibleRoles = new List<string>()
    {
      "CoAdministrator",
      "AccountAdministrator",
      "ServiceAdministrator"
    };
    private const string Area = "Commerce";
    private const string Layer = "AzureResourceHelper";
    private static readonly ActionPerformanceTracer performanceTracer = new ActionPerformanceTracer("Commerce", nameof (AzureResourceHelper));

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.CheckDeploymentRequestContext();

    public IDictionary<double, double> GetMeterPricing(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      Guid meterId,
      out string currencyCode,
      out string locale,
      Guid tenantId,
      Guid objectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForEmptyGuid(objectId, nameof (objectId));
      requestContext.TraceEnter(5106752, "Commerce", nameof (AzureResourceHelper), nameof (GetMeterPricing));
      currencyCode = string.Empty;
      locale = string.Empty;
      if (!this.IsValidAadUser(requestContext))
        return OfferPriceList.GetPricingForMeter(meterId);
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IArmAdapterService service1 = requestContext.GetService<IArmAdapterService>();
        AzureSubscriptionInfo subscriptionForUser = service1.GetSubscriptionForUser(requestContext, subscriptionId);
        AzureOfferType azureOfferType = subscriptionForUser != null ? subscriptionForUser.OfferType : AzureOfferType.None;
        if (azureOfferType != AzureOfferType.Standard)
        {
          requestContext.Trace(5106704, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), string.Format("Unable to retrieve pricing for meterId: {0}, subscriptionId {1} because the offer type ({2}) is not Standard (PayAsYouGo).", (object) meterId, (object) subscriptionId, (object) azureOfferType));
          return (IDictionary<double, double>) null;
        }
        IAzureBillingService service2 = vssRequestContext.GetService<IAzureBillingService>();
        CommerceBillingContextInfo billingContext = new CommerceBillingContextInfo()
        {
          TenantId = new Guid?(tenantId),
          ObjectId = new Guid?(objectId)
        };
        CommerceBillingAccountInfo billingAccountInfo = (CommerceBillingAccountInfo) null;
        try
        {
          billingAccountInfo = service2.GetBillingAccountInfo(vssRequestContext, billingContext);
        }
        catch (UnsupportedSubscriptionTypeException ex)
        {
        }
        locale = string.IsNullOrEmpty(billingAccountInfo?.CommunicationCulture) ? "en-us" : billingAccountInfo.CommunicationCulture;
        string regionInfo = string.IsNullOrEmpty(billingAccountInfo?.Region) ? "US" : billingAccountInfo.Region;
        currencyCode = string.IsNullOrEmpty(billingAccountInfo?.CurrencyCode) ? "USD" : billingAccountInfo.CurrencyCode;
        if (regionInfo.Equals("us", StringComparison.OrdinalIgnoreCase))
          return OfferPriceList.GetPricingForMeter(meterId);
        requestContext.Trace(5106710, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), string.Format("Getting price for meter {0} from ARM adapter.", (object) meterId));
        AzureRateCard meterPricing1 = service1.GetMeterPricing(vssRequestContext, subscriptionId, locale, currencyCode, "MS-AZR-0003P", regionInfo);
        Dictionary<double, double> meterPricing2;
        if (meterPricing1 == null)
        {
          meterPricing2 = (Dictionary<double, double>) null;
        }
        else
        {
          List<AzureMeterResource> meters = meterPricing1.Meters;
          meterPricing2 = meters != null ? meters.FirstOrDefault<AzureMeterResource>((Func<AzureMeterResource, bool>) (x => x.MeterId == meterId.ToString()))?.MeterRates : (Dictionary<double, double>) null;
        }
        return (IDictionary<double, double>) meterPricing2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106706, "Commerce", nameof (AzureResourceHelper), ex);
        return (IDictionary<double, double>) null;
      }
      finally
      {
        requestContext.TraceLeave(5106752, "Commerce", nameof (AzureResourceHelper), nameof (GetMeterPricing));
      }
    }

    public IEnumerable<ISubscriptionAccount> GetAzureSubscriptionsForUser(
      IVssRequestContext requestContext,
      Guid? subscriptionId,
      bool queryAcrossTenants = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      try
      {
        requestContext.TraceEnter(5108461, "Commerce", nameof (AzureResourceHelper), nameof (GetAzureSubscriptionsForUser));
        if (!queryAcrossTenants || AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext))
          return this.GetAzureSubscriptionsForUserInternal(requestContext, subscriptionId);
        IEnumerable<Guid> tenants = this.GetTenants(requestContext);
        return this.GetAzureSubscriptionsForTenants(requestContext, tenants);
      }
      finally
      {
        requestContext.TraceLeave(5108462, "Commerce", nameof (AzureResourceHelper), nameof (GetAzureSubscriptionsForUser));
      }
    }

    public ISubscriptionAccount GetAzureSubscriptionForPurchase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string galleryItemId,
      Guid? collectionId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(5108464, "Commerce", nameof (AzureResourceHelper), new object[3]
      {
        (object) subscriptionId,
        (object) galleryItemId,
        (object) collectionId
      }, nameof (GetAzureSubscriptionForPurchase));
      SubscriptionAccount subscriptionAccount = new SubscriptionAccount()
      {
        SubscriptionId = new Guid?(subscriptionId),
        FailedPurchaseReason = PurchaseErrorReason.None
      };
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      try
      {
        ciData.Add("SubscriptionId", subscriptionId.ToString());
        ciData.Add("GalleryItemId", galleryItemId);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IOfferMeter offerMeter = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, galleryItemId);
        if (offerMeter == null)
          throw new OfferMeterNotFoundException(galleryItemId);
        bool isBundle = offerMeter.Category == MeterCategory.Bundle;
        ciData.Add("IsBundle", isBundle.ToString());
        int num = this.IsValidAadUser(requestContext) ? 1 : 0;
        bool flag1 = false;
        Guid guid;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableUseSubscriptionTenantFromRequestContext") && requestContext.Items.TryGetValue<Guid>("subscriptionTenantId", out guid) && guid != Guid.Empty)
        {
          requestContext.TraceAlways(5108464, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), string.Format("Found tenantId in Request context {0} so skipped AAd check ", (object) guid));
          flag1 = true;
        }
        if (num == 0 && !flag1)
        {
          subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.InvalidUser;
          return (ISubscriptionAccount) subscriptionAccount;
        }
        IAzureBillingService azureBillingService = vssRequestContext.GetService<IAzureBillingService>();
        AzureSubscriptionInfo azureSubscriptionInfo = vssRequestContext.GetService<IArmAdapterService>().GetSubscriptionForUser(vssRequestContext, subscriptionId, AzureErrorBehavior.Throw);
        if (azureSubscriptionInfo == null)
        {
          subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.AzureServiceError;
          return (ISubscriptionAccount) subscriptionAccount;
        }
        subscriptionAccount.SubscriptionStatus = azureSubscriptionInfo.Status;
        subscriptionAccount.SubscriptionName = azureSubscriptionInfo.DisplayName;
        subscriptionAccount.OfferType = new AzureOfferType?(azureSubscriptionInfo.OfferType);
        ciData.Add("SubscriptionName", "[NonEmail:" + subscriptionAccount.SubscriptionName + "]");
        ciData.Add("SubscriptionStatus", subscriptionAccount.SubscriptionStatus.ToString());
        ciData.Add("OfferCode", azureSubscriptionInfo.QuotaId);
        ciData.Add("LocationPlacementId", azureSubscriptionInfo.LocationPlacementId);
        CommerceBillingContextInfo billingContext = new CommerceBillingContextInfo()
        {
          TenantId = new Guid?(this.GetUserTenant(requestContext)),
          ObjectId = new Guid?(this.GetUserObjectIdForTenant(requestContext))
        };
        ciData.Add("UserTenantId", billingContext.TenantId.ToString());
        CustomerIntelligenceData intelligenceData1 = ciData;
        Guid? nullable1 = billingContext.ObjectId;
        string str1 = nullable1.ToString();
        intelligenceData1.Add("UserObjectId", str1);
        CommerceBillingContextInfo contextForSubscription = azureBillingService.GetBillingContextForSubscription(vssRequestContext, subscriptionId);
        if (contextForSubscription != null)
        {
          if (contextForSubscription.BillingSystemType == CommerceBillingSystemType.Mint)
          {
            ciData.Add("IsMintSubscription", true);
            subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.UnsupportedSubscription;
            return (ISubscriptionAccount) subscriptionAccount;
          }
          nullable1 = contextForSubscription.TenantId;
          if (nullable1.HasValue)
          {
            CommerceBillingContextInfo billingContextInfo = billingContext;
            nullable1 = contextForSubscription.TenantId;
            Guid? nullable2 = new Guid?(nullable1.Value);
            billingContextInfo.TenantId = nullable2;
            CustomerIntelligenceData intelligenceData2 = ciData;
            nullable1 = billingContext.TenantId;
            string str2 = nullable1.ToString();
            intelligenceData2.Add("BillingTenantId", str2);
          }
          nullable1 = contextForSubscription.ObjectId;
          if (nullable1.HasValue)
          {
            CommerceBillingContextInfo billingContextInfo = billingContext;
            nullable1 = contextForSubscription.ObjectId;
            Guid? nullable3 = new Guid?(nullable1.Value);
            billingContextInfo.ObjectId = nullable3;
            CustomerIntelligenceData intelligenceData3 = ciData;
            nullable1 = billingContext.ObjectId;
            string str3 = nullable1.ToString();
            intelligenceData3.Add("BillingObjectId", str3);
          }
          billingContext.AdministratorEmail = contextForSubscription.AdministratorEmail;
          billingContext.AdministratorPuid = contextForSubscription.AdministratorPuid;
        }
        else
          ciData.Add("SubscriptionBillingContext", "null");
        if (!isBundle && collectionId.HasValue)
        {
          CollectionHelper.WithCollectionContext(requestContext.Elevate(), collectionId.Value, (Action<IVssRequestContext>) (collectionContext =>
          {
            ciData.Add("AccountId", collectionId.ToString());
            if (!offerMeter.IsFirstParty)
            {
              if (azureSubscriptionInfo.OfferType == AzureOfferType.Csp)
              {
                subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.UnsupportedSubscriptionCsp;
                return;
              }
              try
              {
                requestContext.Trace(5108840, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), "Checking third party offer region");
                HashSet<string> validRegions = requestContext.GetService<IAzureStoreService>().GetOfferAvailableRegions(requestContext, offerMeter).SelectMany<KeyValuePair<string, string[]>, string>((Func<KeyValuePair<string, string[]>, IEnumerable<string>>) (r => (IEnumerable<string>) r.Value)).ToHashSet<string>();
                string twoLetterIsoRegionName = azureBillingService.GetBillingAccountInfo(requestContext, billingContext).Region;
                ciData.Add("SubscriptionRegionCode", twoLetterIsoRegionName);
                ciData.Add("ValidOfferRegions", string.Join(",", (IEnumerable<string>) validRegions));
                if (!string.IsNullOrWhiteSpace(twoLetterIsoRegionName))
                {
                  if (!validRegions.Contains(twoLetterIsoRegionName))
                  {
                    string regionEnglishName = new RegionInfo(twoLetterIsoRegionName).EnglishName;
                    requestContext.TraceConditionally(5108839, TraceLevel.Warning, "Commerce", nameof (AzureResourceHelper), (Func<string>) (() => "Third party offer regions (" + string.Join(",", (IEnumerable<string>) validRegions) + ") doesn't contain subscription region " + twoLetterIsoRegionName + " (" + regionEnglishName + ")"));
                    subscriptionAccount.Locale = twoLetterIsoRegionName;
                    subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.InvalidOfferRegion;
                    subscriptionAccount.RegionDisplayName = regionEnglishName;
                    return;
                  }
                }
              }
              catch (Exception ex)
              {
                requestContext.TraceException(5108839, TraceLevel.Error, "Commerce", nameof (AzureResourceHelper), ex);
              }
            }
            IVssRequestContext context = collectionContext.Elevate();
            string preferredRegion = context.GetService<ICollectionService>().GetCollection(context, (IEnumerable<string>) null)?.PreferredRegion;
            bool flag2 = this.IsSubscriptionValidForRegion(collectionContext, preferredRegion, azureSubscriptionInfo);
            ciData.Add("IsValidRegionPurchase", flag2.ToString());
            if (flag2)
              return;
            subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.InvalidRegionPurchase;
          }), method: nameof (GetAzureSubscriptionForPurchase));
          if (subscriptionAccount.FailedPurchaseReason != PurchaseErrorReason.None)
            return (ISubscriptionAccount) subscriptionAccount;
        }
        SubscriptionAuthorizationSource subscriptionAuthorization = this.GetSubscriptionAuthorization(requestContext, subscriptionId, isBundle);
        ciData.Add("IsUserAdminOrCoAdmin", subscriptionAuthorization != 0);
        ciData.Add("AuthorizationSource", (object) subscriptionAuthorization);
        if (subscriptionAccount.SubscriptionStatus == SubscriptionStatus.Disabled)
        {
          subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.DisabledSubscription;
          return (ISubscriptionAccount) subscriptionAccount;
        }
        subscriptionAccount.SubscriptionTenantId = billingContext.TenantId;
        subscriptionAccount.SubscriptionObjectId = billingContext.ObjectId;
        subscriptionAccount.SubscriptionOfferCode = string.Empty;
        if (subscriptionAuthorization == SubscriptionAuthorizationSource.Unauthorized)
        {
          subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.NotAdminOrCoAdmin;
          return (ISubscriptionAccount) subscriptionAccount;
        }
        PurchaseErrorReason purchaseErrorReason = this.IsSubscriptionEligibleForPurchase(requestContext, subscriptionId, azureSubscriptionInfo, collectionId, isBundle, galleryItemId);
        if (purchaseErrorReason != PurchaseErrorReason.None)
        {
          subscriptionAccount.FailedPurchaseReason = purchaseErrorReason;
          return (ISubscriptionAccount) subscriptionAccount;
        }
        if (offerMeter.BillingEntity == BillingProvider.AzureStoreManaged && collectionId.HasValue && !this.ValidatePaymentInstrumentIsCreditCard(vssRequestContext, azureSubscriptionInfo, billingContext, ciData))
        {
          subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.PaymentInstrumentNotCreditCard;
          return (ISubscriptionAccount) subscriptionAccount;
        }
        subscriptionAccount.IsPrepaidFundSubscription = azureSubscriptionInfo.Attributes.HasFlag((Enum) AzureSubscriptionAttributes.IsPrePaidFundWarningRequired);
        subscriptionAccount.IsPricingAvailable = azureSubscriptionInfo.Attributes.HasFlag((Enum) AzureSubscriptionAttributes.IsPricingAvailable);
        return (ISubscriptionAccount) subscriptionAccount;
      }
      catch (AzureResponseException ex)
      {
        requestContext.TraceException(5108466, "Commerce", nameof (AzureResourceHelper), (Exception) ex);
        subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.AzureServiceError;
        return (ISubscriptionAccount) subscriptionAccount;
      }
      catch (AzureUnauthorizedAccessException ex)
      {
        requestContext.TraceException(5108466, "Commerce", nameof (AzureResourceHelper), (Exception) ex);
        subscriptionAccount.FailedPurchaseReason = PurchaseErrorReason.NotSubscriptionUser;
        return (ISubscriptionAccount) subscriptionAccount;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108466, "Commerce", nameof (AzureResourceHelper), ex);
        throw;
      }
      finally
      {
        subscriptionAccount.IsEligibleForPurchase = subscriptionAccount.FailedPurchaseReason == PurchaseErrorReason.None;
        try
        {
          if (ciData.GetData().Any<KeyValuePair<string, object>>())
          {
            ciData.Add("PurchaseErrorReason", subscriptionAccount.FailedPurchaseReason.ToString());
            CustomerIntelligenceData intelligenceData4 = ciData;
            bool flag = subscriptionAccount.IsEligibleForPurchase;
            string lower = flag.ToString().ToLower();
            intelligenceData4.Add("IsEligibleForPurchase", lower);
            CustomerIntelligenceData intelligenceData5 = ciData;
            flag = subscriptionAccount.IsPricingAvailable;
            string str = flag.ToString();
            intelligenceData5.Add("IsPricingAvailable", str);
            CustomerIntelligence.PublishEvent(requestContext, "SubscriptionEvaluated", ciData);
          }
        }
        catch
        {
        }
        requestContext.TraceLeave(5108465, "Commerce", nameof (AzureResourceHelper), nameof (GetAzureSubscriptionForPurchase));
      }
    }

    public IEnumerable<AzureSubscriptionInfo> GetAzureSubscriptions(
      IVssRequestContext requestContext,
      bool doValidation,
      Guid? subscriptionId,
      Guid? tenantId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IArmAdapterService service = vssRequestContext.GetService<IArmAdapterService>();
      List<AzureSubscriptionInfo> source = new List<AzureSubscriptionInfo>();
      if (!tenantId.HasValue && !this.IsValidAadUser(requestContext))
        return (IEnumerable<AzureSubscriptionInfo>) source;
      if (subscriptionId.HasValue)
      {
        Guid? nullable = subscriptionId;
        Guid guid = new Guid();
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != guid ? 1 : 0) : 0) : 1) != 0)
        {
          IArmAdapterService armAdapterService = service;
          IVssRequestContext requestContext1 = vssRequestContext;
          Guid subscriptionId1 = subscriptionId.Value;
          nullable = new Guid?();
          Guid? tenantId1 = nullable;
          AzureSubscriptionInfo subscriptionForUser = armAdapterService.GetSubscriptionForUser(requestContext1, subscriptionId1, tenantId: tenantId1);
          if (subscriptionForUser != null)
          {
            source.Add(subscriptionForUser);
            goto label_9;
          }
          else
            goto label_9;
        }
      }
      IEnumerable<AzureSubscriptionInfo> subscriptionsForUser = service.GetSubscriptionsForUser(vssRequestContext, tenantId);
      if (!subscriptionsForUser.IsNullOrEmpty<AzureSubscriptionInfo>())
      {
        source = subscriptionsForUser.ToList<AzureSubscriptionInfo>();
        if (tenantId.HasValue)
          source.ForEach((Action<AzureSubscriptionInfo>) (azureSubscription => azureSubscription.TenantId = tenantId.Value));
      }
label_9:
      foreach (AzureSubscriptionInfo subscriptionInfo in source)
      {
        requestContext.Trace(5108445, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), string.Format("AzureSubscriptionID : {0}", (object) subscriptionInfo.SubscriptionId));
        subscriptionInfo.IsAdministrator = doValidation && this.GetSubscriptionAuthorization(vssRequestContext, subscriptionInfo.SubscriptionId, false) != SubscriptionAuthorizationSource.Unauthorized;
      }
      requestContext.TraceAlways(5109363, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), new
      {
        Msg = "GetAzureSubscription",
        subscription_count = source.Count,
        subscriptions = source.Select(x => new
        {
          SubscriptionId = x.SubscriptionId,
          IsAdministrator = x.IsAdministrator
        })
      }.Serialize());
      return (IEnumerable<AzureSubscriptionInfo>) source;
    }

    public AzureSubscriptionInfo GetAzureSubscriptionForUser(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return !this.IsValidAadUser(vssRequestContext) ? (AzureSubscriptionInfo) null : vssRequestContext.GetService<IArmAdapterService>().GetSubscriptionForUser(vssRequestContext, subscriptionId);
    }

    public virtual SubscriptionAuthorizationSource GetSubscriptionAuthorization(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      bool isBundle = false)
    {
      if (this.HasCollectionPurchasePermission(requestContext) && this.CanUserAccessSubscription(requestContext, subscriptionId))
        return SubscriptionAuthorizationSource.SpecializedLocalPermission;
      if (this.IsUserAdminOrCoAdmin(requestContext, subscriptionId))
        return SubscriptionAuthorizationSource.AdminOrCoAdmin;
      SubscriptionRolebasedAccessControlSource rbacSource;
      return this.IsUserSubscriptionOwnerOrContributor(requestContext, subscriptionId, out rbacSource) && (!isBundle || !requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.RestrictContributorFromBundlePurchase") || rbacSource == SubscriptionRolebasedAccessControlSource.Owner) ? SubscriptionAuthorizationSource.Rbac : SubscriptionAuthorizationSource.Unauthorized;
    }

    private bool CanUserAccessSubscription(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IArmAdapterService>().GetSubscriptionForUser(vssRequestContext, azureSubscriptionId) != null;
    }

    private bool HasCollectionPurchasePermission(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IPermissionCheckerService>().CheckPermission(vssRequestContext, 2, CommerceSecurity.CommerceSecurityNamespaceId, "/PartnerPurchase", false);
    }

    private bool IsUserAdminOrCoAdmin(IVssRequestContext requestContext, Guid subscriptionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(5106753, "Commerce", nameof (AzureResourceHelper), nameof (IsUserAdminOrCoAdmin));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        AzureAuthorizationResponseWrapper subscriptionAuthorization = vssRequestContext.GetService<IArmAdapterService>().GetAzureSubscriptionAuthorization(vssRequestContext, subscriptionId);
        vssRequestContext.TraceProperties<AzureAuthorizationResponseWrapper>(5109092, "Commerce", nameof (AzureResourceHelper), subscriptionAuthorization, (string) null);
        if (subscriptionAuthorization?.Value == null)
        {
          requestContext.Trace(5108863, TraceLevel.Verbose, "Commerce", nameof (AzureResourceHelper), "Subscription Authorization / Wrapper is null.");
          return false;
        }
        string loginAddress = this.GetLoginAddressOfIdentity(vssRequestContext).ToLowerInvariant();
        IEnumerable<AzureAuthorizationResponseValue> userSubscriptionAuthorizations = subscriptionAuthorization.Value.Where<AzureAuthorizationResponseValue>((Func<AzureAuthorizationResponseValue, bool>) (x => string.Equals(x.Properties.EmailAddress, loginAddress, StringComparison.InvariantCultureIgnoreCase)));
        if (!userSubscriptionAuthorizations.Any<AzureAuthorizationResponseValue>())
        {
          requestContext.Trace(5108864, TraceLevel.Verbose, "Commerce", nameof (AzureResourceHelper), string.Format("Subscription Authorization does not contain an entry for CUID {0}.", (object) requestContext.GetUserCuid()));
          return false;
        }
        int num = userSubscriptionAuthorizations.Any<AzureAuthorizationResponseValue>((Func<AzureAuthorizationResponseValue, bool>) (x => this.azureEligibleRoles.Any<string>((Func<string, bool>) (r => x.Properties.Role.Contains(r))))) ? 1 : 0;
        if (num == 0)
          requestContext.TraceConditionally(5108865, TraceLevel.Verbose, "Commerce", nameof (AzureResourceHelper), (Func<string>) (() =>
          {
            string str = string.Join("/", userSubscriptionAuthorizations.Select<AzureAuthorizationResponseValue, string>((Func<AzureAuthorizationResponseValue, string>) (x => x.Properties.Role)));
            return string.Format("Subscription Authorization for CUID {0} does not contain an eligible role, available roles: {1}.", (object) requestContext.GetUserCuid(), (object) str);
          }));
        return num != 0;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106734, "Commerce", nameof (AzureResourceHelper), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106753, "Commerce", nameof (AzureResourceHelper), nameof (IsUserAdminOrCoAdmin));
      }
    }

    private bool IsUserSubscriptionOwnerOrContributor(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      out SubscriptionRolebasedAccessControlSource rbacSource)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(5108929, "Commerce", nameof (AzureResourceHelper), nameof (IsUserSubscriptionOwnerOrContributor));
      rbacSource = SubscriptionRolebasedAccessControlSource.Other;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IArmAdapterService service = vssRequestContext.GetService<IArmAdapterService>();
        AzureRoleDefinitionResponseWrapper subscriptionRoleDefinitions = service.GetSubscriptionRoleDefinitions(vssRequestContext, subscriptionId);
        if (subscriptionRoleDefinitions?.Value == null)
        {
          requestContext.TraceAlways(5109032, TraceLevel.Verbose, "Commerce", nameof (AzureResourceHelper), "Subscription role definition response / wrapper is null.");
          return false;
        }
        AzureRoleDefinitionResponse definitionResponse1 = subscriptionRoleDefinitions.Value.SingleOrDefault<AzureRoleDefinitionResponse>((Func<AzureRoleDefinitionResponse, bool>) (v => v.Properties.RoleName == "Owner"));
        if (definitionResponse1 == null)
          throw new ArgumentException(string.Format("Unable to locate Owner role definition in subscription {0}", (object) subscriptionId));
        vssRequestContext.TraceProperties<AzureRoleDefinitionResponse>(5109226, "Commerce", nameof (AzureResourceHelper), definitionResponse1, (string) null);
        AzureRoleDefinitionResponse definitionResponse2 = subscriptionRoleDefinitions.Value.SingleOrDefault<AzureRoleDefinitionResponse>((Func<AzureRoleDefinitionResponse, bool>) (v => v.Properties.RoleName == "Contributor"));
        if (definitionResponse2 == null)
          throw new ArgumentException(string.Format("Unable to locate Contributor role definition in subscription {0}", (object) subscriptionId));
        vssRequestContext.TraceProperties<AzureRoleDefinitionResponse>(5109227, "Commerce", nameof (AzureResourceHelper), definitionResponse2, (string) null);
        vssRequestContext.TraceProperties<AzureRoleDefinitionResponseWrapper>(5109091, "Commerce", nameof (AzureResourceHelper), subscriptionRoleDefinitions, (string) null);
        AzureRoleAssignmentResponseWrapper assignmentResponseWrapper = !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableUseSubscriptionTenantFromRequestContext") ? service.GetSubscriptionRoleAssignments(requestContext, subscriptionId, string.Format("assignedTo('{0}')", (object) this.GetUserObjectIdForTenant(requestContext))) : service.GetSubscriptionRoleAssignmentsMSA(requestContext, subscriptionId, string.Format("assignedTo('{0}')", (object) this.GetUserObjectIdForTenant(requestContext)));
        if (assignmentResponseWrapper?.Value == null)
        {
          requestContext.TraceAlways(5109033, TraceLevel.Verbose, "Commerce", nameof (AzureResourceHelper), "Role assignment response / wrapper is null.");
          return false;
        }
        vssRequestContext.TraceProperties<AzureRoleAssignmentResponseWrapper>(5109225, "Commerce", nameof (AzureResourceHelper), assignmentResponseWrapper, (string) null);
        foreach (AzureRoleAssignmentValue roleAssignmentValue in assignmentResponseWrapper.Value)
        {
          if (roleAssignmentValue.Properties.Scope.Equals(string.Format("/subscriptions/{0}", (object) subscriptionId), StringComparison.InvariantCultureIgnoreCase))
          {
            if (roleAssignmentValue.Properties.RoleDefinitionId == definitionResponse1.Id)
            {
              rbacSource = SubscriptionRolebasedAccessControlSource.Owner;
              return true;
            }
            if (roleAssignmentValue.Properties.RoleDefinitionId == definitionResponse2.Id)
            {
              rbacSource = SubscriptionRolebasedAccessControlSource.Contributor;
              return true;
            }
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108930, "Commerce", nameof (AzureResourceHelper), ex);
        throw;
      }
      finally
      {
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        eventData.Add("SubscriptionId", subscriptionId.ToString());
        eventData.Add("SubscriptionRolebasedAccessControlSource", rbacSource.ToString());
        CustomerIntelligence.PublishEvent(requestContext, "SubscriptionOwnerOrContributor", eventData);
        requestContext.TraceLeave(5108929, "Commerce", nameof (AzureResourceHelper), nameof (IsUserSubscriptionOwnerOrContributor));
      }
    }

    private bool ValidatePaymentInstrumentIsCreditCard(
      IVssRequestContext requestContext,
      AzureSubscriptionInfo azureSubscriptionInfo,
      CommerceBillingContextInfo billingContext,
      CustomerIntelligenceData ciData)
    {
      if (azureSubscriptionInfo.OfferType == AzureOfferType.Ea || azureSubscriptionInfo.OfferType == AzureOfferType.Csp || azureSubscriptionInfo.QuotaId == "MSDNDevTest_2014-09-01")
        return true;
      IAzureBillingService service = requestContext.GetService<IAzureBillingService>();
      CommerceBillingSubscriptionInfo subscriptionInfo;
      using (AzureResourceHelper.performanceTracer.Trace(requestContext, CommerceTracePoints.GetBillingSubscriptionInfoPerformance, "GetBillingSubscriptionInfo"))
        subscriptionInfo = service.GetBillingSubscriptionInfo(requestContext, azureSubscriptionInfo.SubscriptionId, billingContext, AzureErrorBehavior.Throw);
      ciData.Add("PaymentInstrumentId", subscriptionInfo.PaymentInstrumentId);
      requestContext.Trace(5108841, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), "Checking type of PaymentInstrumentId " + subscriptionInfo.PaymentInstrumentId + " for AzureStoreManaged extension");
      if (string.IsNullOrEmpty(subscriptionInfo.PaymentInstrumentId))
      {
        requestContext.Trace(5108842, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), string.Format("Null/empty PaymentInstrumentId for subscription {0}", (object) azureSubscriptionInfo.SubscriptionId));
        return false;
      }
      if (!subscriptionInfo.PaymentInstrumentId.Contains("+"))
      {
        CommerceBillingPaymentInstrument instrumentDetails = service.GetPaymentInstrumentDetails(requestContext, subscriptionInfo.PaymentInstrumentId, billingContext, AzureErrorBehavior.Throw);
        if (instrumentDetails == null || !instrumentDetails.IsCreditCard)
        {
          if (instrumentDetails == null)
            requestContext.Trace(5108842, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), string.Format("ARM returned null BillingPaymentInstrument for subscription {0}", (object) azureSubscriptionInfo.SubscriptionId));
          else
            requestContext.Trace(5108842, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), string.Format("Subscription {0} with PaymentInstrumentId {1} is not backed by a credit card", (object) azureSubscriptionInfo.SubscriptionId, (object) subscriptionInfo.PaymentInstrumentId));
          return false;
        }
      }
      return true;
    }

    public List<string> GetEmailsOfAdminAndCoAdmins(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      Guid tenantId)
    {
      AzureAuthorizationResponseWrapper subscriptionAuthorization = requestContext.GetService<IArmAdapterService>().GetAzureSubscriptionAuthorization(requestContext, subscriptionId, new Guid?(tenantId));
      if (subscriptionAuthorization?.Value == null)
        return (List<string>) null;
      List<string> source = new List<string>();
      foreach (AzureAuthorizationResponseValue authorizationResponseValue in subscriptionAuthorization.Value.Where<AzureAuthorizationResponseValue>((Func<AzureAuthorizationResponseValue, bool>) (v => v.Properties.EmailAddress != null)).Where<AzureAuthorizationResponseValue>((Func<AzureAuthorizationResponseValue, bool>) (v => this.azureEligibleRoles.Contains(v.Properties.Role))))
      {
        AzureAuthorizationResponseValue value = authorizationResponseValue;
        requestContext.TraceConditionally(5115107, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), (Func<string>) (() => "Email Address: " + value.Properties.EmailAddress));
        source.Add(value.Properties.EmailAddress);
      }
      return source.Distinct<string>().ToList<string>();
    }

    public bool IsValidAadUser(IVssRequestContext requestContext)
    {
      if (AadIdentityHelper.IsAadUser((IReadOnlyVssIdentity) requestContext.GetUserIdentity()))
        return true;
      requestContext.Trace(5106705, TraceLevel.Error, "Commerce", nameof (AzureResourceHelper), "User is not an Aad user");
      return false;
    }

    internal virtual bool IsSubscriptionValidForRegion(
      IVssRequestContext requestContext,
      string regionCode,
      AzureSubscriptionInfo subscriptionInfo)
    {
      try
      {
        requestContext.TraceEnter(5106708, "Commerce", nameof (AzureResourceHelper), new object[2]
        {
          (object) regionCode,
          (object) subscriptionInfo
        }, nameof (IsSubscriptionValidForRegion));
        requestContext.Trace(5106709, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), "Collection region: " + regionCode);
        if (!string.IsNullOrEmpty(regionCode))
        {
          AzureLocationPlacementSettings settingsForRegion = AzureLocationPlacement.GetPlacementSettingsForRegion(regionCode);
          if (settingsForRegion != null)
          {
            if (settingsForRegion.HasRegionRestrictions)
              return subscriptionInfo.LocationPlacementSettings.AllowedRegionCodes.Contains(regionCode);
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(5108827, "Commerce", nameof (AzureResourceHelper), nameof (IsSubscriptionValidForRegion));
      }
      return true;
    }

    internal PurchaseErrorReason IsSubscriptionEligibleForPurchase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AzureSubscriptionInfo subscriptionInfo,
      Guid? collectionId,
      bool isBundle = false,
      string galleryId = "")
    {
      requestContext.TraceEnter(5108442, "Commerce", nameof (AzureResourceHelper), new object[2]
      {
        (object) subscriptionId,
        (object) subscriptionInfo
      }, nameof (IsSubscriptionEligibleForPurchase));
      try
      {
        if (!CommerceUtil.IsBundleEligibleForPurchase(requestContext, galleryId, subscriptionId) || subscriptionInfo.OfferType == AzureOfferType.Unsupported || isBundle && !CommerceUtil.IsExistingPurchase(requestContext, subscriptionId) && !CommerceUtil.IsSubscriptionEligibleForBundlePurchase(requestContext, subscriptionInfo))
          return PurchaseErrorReason.InvalidOfferCode;
        if (collectionId.HasValue && !requestContext.IsFeatureEnabled(collectionId.Value, "VisualStudio.Services.Commerce.AllowFirstPartyCspPurchases") && subscriptionInfo.OfferType == AzureOfferType.Csp)
          return PurchaseErrorReason.UnsupportedSubscriptionCsp;
        if (subscriptionInfo.Attributes.HasFlag((Enum) AzureSubscriptionAttributes.IsMonetaryCheckRequired))
        {
          if (subscriptionInfo.SpendingLimit == AzureSpendingLimit.On)
            return PurchaseErrorReason.MonetaryLimitSet;
          if (subscriptionInfo.SpendingLimit == AzureSpendingLimit.CurrentPeriodOff)
            return PurchaseErrorReason.TemporarySpendingLimit;
        }
        return PurchaseErrorReason.None;
      }
      finally
      {
        requestContext.TraceLeave(5108444, "Commerce", nameof (AzureResourceHelper), nameof (IsSubscriptionEligibleForPurchase));
      }
    }

    private Guid GetUserObjectIdForTenant(IVssRequestContext requestContext)
    {
      object obj;
      requestContext.GetUserIdentity().TryGetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", out obj);
      return obj == null ? Guid.Empty : new Guid(obj.ToString());
    }

    private Guid GetUserTenant(IVssRequestContext requestContext) => AadIdentityHelper.GetIdentityTenantId(requestContext.UserContext);

    public string GetLoginAddressOfIdentity(IVssRequestContext requestContext)
    {
      object obj;
      requestContext.GetUserIdentity().TryGetProperty("Account", out obj);
      if (obj == null)
        return string.Empty;
      string str = obj.ToString();
      string[] strArray = str.Split('\\');
      return strArray.Length > 1 ? strArray[1] : str;
    }

    internal virtual T GetAdapter<T>() where T : class, new() => new T();

    private IEnumerable<Guid> GetTenants(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5106996, "Commerce", nameof (AzureResourceHelper), nameof (GetTenants));
      try
      {
        IVssRequestContext context1 = requestContext.Elevate();
        AadService service = context1.GetService<AadService>();
        IVssRequestContext context2 = context1;
        GetTenantsRequest request = new GetTenantsRequest();
        request.ToMicrosoftServicesTenant = true;
        List<Guid> list = service.GetTenants(context2, request).Tenants.Select<AadTenant, Guid>((Func<AadTenant, Guid>) (tenant => tenant.ObjectId)).ToList<Guid>();
        if (!list.Any<Guid>())
          list.Add(AadIdentityHelper.GetIdentityTenantId(requestContext.UserContext));
        string message = list.Any<Guid>() ? "Tenants returned: " + string.Join(",", list.Select<Guid, string>((Func<Guid, string>) (id => id.ToString())).ToArray<string>()) : "No tenants returned";
        requestContext.Trace(5106997, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), message);
        return (IEnumerable<Guid>) list;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106999, "Commerce", nameof (AzureResourceHelper), ex);
        return (IEnumerable<Guid>) new List<Guid>();
      }
      finally
      {
        requestContext.TraceEnter(5106998, "Commerce", nameof (AzureResourceHelper), nameof (GetTenants));
      }
    }

    internal virtual IEnumerable<ISubscriptionAccount> GetAzureSubscriptionsForUserInternal(
      IVssRequestContext requestContext,
      Guid? subscriptionId,
      Guid? tenantId = null)
    {
      requestContext.TraceEnter(5106990, "Commerce", nameof (AzureResourceHelper), new object[2]
      {
        (object) subscriptionId,
        (object) tenantId
      }, nameof (GetAzureSubscriptionsForUserInternal));
      try
      {
        bool useProdAzureResourcesOnDevFabric = AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext);
        return (IEnumerable<ISubscriptionAccount>) this.GetAzureSubscriptions(requestContext, false, subscriptionId, tenantId).Select<AzureSubscriptionInfo, SubscriptionAccount>((Func<AzureSubscriptionInfo, SubscriptionAccount>) (subscription => new SubscriptionAccount()
        {
          SubscriptionId = new Guid?(subscription.SubscriptionId),
          SubscriptionName = subscription.DisplayName,
          SubscriptionStatus = subscription.Status,
          SubscriptionTenantId = new Guid?(useProdAzureResourcesOnDevFabric ? new Guid("72f988bf-86f1-41af-91ab-2d7cd011db47") : subscription.TenantId)
        }));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106992, "Commerce", nameof (AzureResourceHelper), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106991, "Commerce", nameof (AzureResourceHelper), nameof (GetAzureSubscriptionsForUserInternal));
      }
    }

    private IEnumerable<ISubscriptionAccount> GetAzureSubscriptionsForTenants(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tenantIds)
    {
      Dictionary<Guid, ISubscriptionAccount> dictionary1 = new Dictionary<Guid, ISubscriptionAccount>();
      requestContext.TraceEnter(5106993, "Commerce", nameof (AzureResourceHelper), new object[1]
      {
        (object) tenantIds
      }, nameof (GetAzureSubscriptionsForTenants));
      try
      {
        foreach (Guid guid in tenantIds.Where<Guid>((Func<Guid, bool>) (tid => !Guid.Empty.Equals(tid))))
        {
          IEnumerable<ISubscriptionAccount> subscriptionsForUserInternal;
          using (AzureResourceHelper.performanceTracer.Trace(requestContext, 5106984, "GetAzureSubscriptionsForUserInternal"))
            subscriptionsForUserInternal = this.GetAzureSubscriptionsForUserInternal(requestContext, new Guid?(), new Guid?(guid));
          foreach (ISubscriptionAccount subscriptionAccount1 in subscriptionsForUserInternal.Where<ISubscriptionAccount>((Func<ISubscriptionAccount, bool>) (sub => sub.SubscriptionId.HasValue)))
          {
            Dictionary<Guid, ISubscriptionAccount> dictionary2 = dictionary1;
            Guid? subscriptionId = subscriptionAccount1.SubscriptionId;
            Guid key1 = subscriptionId.Value;
            if (!dictionary2.ContainsKey(key1))
            {
              Dictionary<Guid, ISubscriptionAccount> dictionary3 = dictionary1;
              subscriptionId = subscriptionAccount1.SubscriptionId;
              Guid key2 = subscriptionId.Value;
              ISubscriptionAccount subscriptionAccount2 = subscriptionAccount1;
              dictionary3[key2] = subscriptionAccount2;
            }
            else
            {
              subscriptionId = subscriptionAccount1.SubscriptionId;
              string message = string.Format("Subscription '{0}' is part of multiple tenants", (object) subscriptionId.Value);
              requestContext.Trace(5106994, TraceLevel.Info, "Commerce", nameof (AzureResourceHelper), message);
            }
          }
        }
        return (IEnumerable<ISubscriptionAccount>) dictionary1.Values;
      }
      finally
      {
        requestContext.TraceLeave(5106995, "Commerce", nameof (AzureResourceHelper), nameof (GetAzureSubscriptionsForTenants));
      }
    }
  }
}
