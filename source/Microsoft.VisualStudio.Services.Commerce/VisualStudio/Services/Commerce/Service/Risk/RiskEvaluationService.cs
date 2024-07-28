// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Service.Risk.RiskEvaluationService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.Service.Risk
{
  public class RiskEvaluationService : IRiskEvaluationService, IVssFrameworkService
  {
    private RiskEvaluationService.ServiceSettings serviceSettings;
    private static readonly RegistryQuery s_commerceRiskEvaluationRegistryQuery = (RegistryQuery) "/Service/Commerce/RiskEvaluation/*";
    private const string TrackingIdLabel = "x-ms-tracking-id";
    private const string CorrelationIdLabel = "x-ms-correlation-id";
    private const string ApiVersionLabel = "api-version";
    private const string ApiVersion = "2015-02-28";
    private const string ContentType = "application/json";
    private const string DecisionLabel = "decision";
    private const string ResponseLabel = "Response";
    private const string AadTag = "aad:";
    private const string Area = "Commerce";
    private const string Layer = "RiskEvaluationService";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckSystemRequestContext();
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in RiskEvaluationService.s_commerceRiskEvaluationRegistryQuery);
      Interlocked.CompareExchange<RiskEvaluationService.ServiceSettings>(ref this.serviceSettings, new RiskEvaluationService.ServiceSettings(requestContext), (RiskEvaluationService.ServiceSettings) null);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    internal void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      Volatile.Write<RiskEvaluationService.ServiceSettings>(ref this.serviceSettings, new RiskEvaluationService.ServiceSettings(requestContext));
    }

    public RiskEvaluationResult GetRiskEvaluation(
      IVssRequestContext requestSystemContext,
      string ipAddress,
      string userAgent,
      Guid azureSubscriptionId,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid hostId,
      Guid? sessionId,
      Guid? tenantId,
      Guid? objectId,
      string galleryId,
      int changedQuantity)
    {
      requestSystemContext.CheckSystemRequestContext();
      RiskEvaluationResult riskEvaluationResult = RiskEvaluationResult.Unknown;
      string riskResponseId = (string) null;
      try
      {
        IVssRequestContext vssRequestContext = requestSystemContext.Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, "ConfigurationSecrets", "PurchasesRiskEvaluationCertificate", true);
        X509Certificate2 certificateThumbprint = service.RetrieveFileAsCertificate(vssRequestContext, itemInfo.DrawerId, itemInfo.LookupKey);
        HttpClient clientWithCertificate = requestSystemContext.GetService<ICommerceHttpHelper>().GetHttpClientWithCertificate(requestSystemContext, certificateThumbprint, this.serviceSettings.RequestTimeOut);
        string trackingId = Guid.NewGuid().ToString();
        string correlationId = Guid.NewGuid().ToString();
        RiskEvaluationRequestPayload evaluationPayload = this.GetRiskEvaluationPayload(requestSystemContext, ipAddress, userAgent, azureSubscriptionId, identity, sessionId, tenantId, objectId, galleryId, changedQuantity);
        HttpResponseMessage result = this.CreateAndSendMessage(requestSystemContext, clientWithCertificate, evaluationPayload, trackingId, correlationId).Result;
        if (result != null && result.IsSuccessStatusCode && result.Content != null)
          riskEvaluationResult = this.ParseRiskEvaluationResult(requestSystemContext, result.Content, out riskResponseId);
        requestSystemContext.TraceAlways(5109228, TraceLevel.Info, "Commerce", nameof (RiskEvaluationService), "Risk Evaluation result: " + riskEvaluationResult.ToString() + ". Risk Evaluation's Request - TrackingId: " + trackingId + ", " + string.Format("CorrelationId: {0}. Response - Status Code: {1} ", (object) correlationId, (object) result?.StatusCode) + string.Format("Reason: {0}. Azure Subscription ID: {1}, ", (object) result?.ReasonPhrase, (object) azureSubscriptionId) + string.Format("galleryId: {0}, changedQuantity: {1}, sessionId: {2}.", (object) galleryId, (object) changedQuantity, (object) sessionId));
        return riskEvaluationResult;
      }
      catch (Exception ex)
      {
        requestSystemContext.TraceException(5109229, TraceLevel.Error, "Commerce", nameof (RiskEvaluationService), ex);
        return riskEvaluationResult;
      }
      finally
      {
        this.WriteRiskEvaluationResultCustomerEvent(requestSystemContext, azureSubscriptionId, hostId, sessionId, tenantId, objectId, galleryId, changedQuantity, riskEvaluationResult, riskResponseId);
      }
    }

    private async Task<HttpResponseMessage> CreateAndSendMessage(
      IVssRequestContext requestSystemContext,
      HttpClient httpClient,
      RiskEvaluationRequestPayload payload,
      string trackingId,
      string correlationId)
    {
      HttpContent httpContent = (HttpContent) new ObjectContent<RiskEvaluationRequestPayload>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this.serviceSettings.ModernRiskBaseUri);
      request.Content = httpContent;
      request.Headers.Add("api-version", this.serviceSettings.ModernRiskApiVersion);
      request.Headers.Add("x-ms-tracking-id", trackingId);
      request.Headers.Add("x-ms-correlation-id", correlationId);
      requestSystemContext.Trace(5109261, TraceLevel.Info, "Commerce", nameof (RiskEvaluationService), string.Format("CreateAndSendMessage - requestUri: {0}, ", (object) this.serviceSettings.ModernRiskBaseUri) + "ApiVersion: " + this.serviceSettings.ModernRiskApiVersion + ", payload: " + payload.Serialize<RiskEvaluationRequestPayload>());
      return await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, new CancellationToken()).ConfigureAwait(false);
    }

    private RiskEvaluationRequestPayload GetRiskEvaluationPayload(
      IVssRequestContext deploymentContext,
      string ipAddress,
      string userAgent,
      Guid azureSubscriptionId,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid? sessionId,
      Guid? tenantId,
      Guid? objectId,
      string galleryId,
      int changedQuantity)
    {
      IOfferMeterService service = deploymentContext.GetService<IOfferMeterService>();
      PurchasableOfferMeter purchasableOfferMeter = service.GetPurchasableOfferMeter(deploymentContext, galleryId, "GalleryId", new Guid?(azureSubscriptionId), true, tenantId, objectId);
      IOfferMeterPrice offerMeterPrice = service.GetOfferMeterPrice(deploymentContext, galleryId).ToList<IOfferMeterPrice>().Where<IOfferMeterPrice>((Func<IOfferMeterPrice, bool>) (price => string.Equals(purchasableOfferMeter.CurrencyCode, price.CurrencyCode, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<IOfferMeterPrice>();
      string currencyCode = purchasableOfferMeter?.CurrencyCode != null ? purchasableOfferMeter.CurrencyCode : "unknown";
      string localeCode = purchasableOfferMeter?.LocaleCode != null ? purchasableOfferMeter.LocaleCode : "unknown";
      Guid platformMeterId = purchasableOfferMeter?.OfferMeterDefinition?.PlatformMeterId ?? Guid.Empty;
      double priceValue = offerMeterPrice != null ? offerMeterPrice.Price : -1.0;
      deploymentContext.Trace(5109232, TraceLevel.Info, "Commerce", nameof (RiskEvaluationService), "PurchasableOfferMeter and OfferMeterPrice Details - CurrencyCode: " + currencyCode + " LocaleCode: " + localeCode + " " + string.Format("PlatformId: {0} OfferMeterPrice's price value: {1}", (object) platformMeterId, (object) priceValue));
      string paymentInstrumentId;
      string offerCode;
      SubscriptionStatus subscriptionStatus;
      string quotaId;
      this.GetSubscriptionInfo(deploymentContext, azureSubscriptionId, out paymentInstrumentId, out offerCode, out subscriptionStatus, out quotaId, tenantId, objectId);
      string skuId = deploymentContext.GetService<IVssRegistryService>().GetValue(deploymentContext, new RegistryQuery("/Service/Commerce/RiskEvaluation/AzureDevOpsProductSku"), "7UD-00001");
      string puid = this.GetPuid(identity);
      string property = identity.GetProperty<string>("Mail", string.Empty);
      string formattedPuid;
      string idNamespace;
      CommerceUtil.TryParsePuidToDecimalFormat(deploymentContext, puid, out formattedPuid, out idNamespace);
      return this.ContructRiskEvaluationPayload(ipAddress, userAgent, property, formattedPuid, idNamespace, currencyCode, localeCode, priceValue, platformMeterId, skuId, sessionId, paymentInstrumentId, offerCode, subscriptionStatus, azureSubscriptionId, changedQuantity, quotaId);
    }

    private void GetSubscriptionInfo(
      IVssRequestContext deploymentContext,
      Guid azureSubscriptionId,
      out string paymentInstrumentId,
      out string offerCode,
      out SubscriptionStatus subscriptionStatus,
      out string quotaId,
      Guid? tenantId,
      Guid? objectId)
    {
      IAzureBillingService service1 = deploymentContext.GetService<IAzureBillingService>();
      CommerceBillingContextInfo billingContext = new CommerceBillingContextInfo()
      {
        TenantId = tenantId,
        ObjectId = objectId
      };
      if (!tenantId.HasValue || !objectId.HasValue)
        billingContext = service1.GetBillingContextForSubscription(deploymentContext, azureSubscriptionId);
      Guid? nullable1;
      int num1;
      if (billingContext == null)
      {
        num1 = 1;
      }
      else
      {
        nullable1 = billingContext.TenantId;
        num1 = !nullable1.HasValue ? 1 : 0;
      }
      if (num1 == 0)
      {
        int num2;
        if (billingContext == null)
        {
          num2 = 1;
        }
        else
        {
          nullable1 = billingContext.ObjectId;
          num2 = !nullable1.HasValue ? 1 : 0;
        }
        if (num2 == 0)
          goto label_17;
      }
      IVssRequestContext requestContext1 = deploymentContext;
      Guid? nullable2;
      if (billingContext == null)
      {
        nullable1 = new Guid?();
        nullable2 = nullable1;
      }
      else
        nullable2 = billingContext.TenantId;
      string str1 = string.Format("{0} returned tenantId:{1}, ", (object) "GetBillingContextForSubscription", (object) nullable2);
      Guid? nullable3;
      if (billingContext == null)
      {
        nullable1 = new Guid?();
        nullable3 = nullable1;
      }
      else
        nullable3 = billingContext.ObjectId;
      string str2 = string.Format("objectId:{0}", (object) nullable3);
      string message = str1 + str2;
      requestContext1.Trace(5109230, TraceLevel.Warning, "Commerce", nameof (RiskEvaluationService), message);
label_17:
      IArmAdapterService service2 = deploymentContext.GetService<IArmAdapterService>();
      AzureSubscriptionInfo subscriptionInfo1 = (AzureSubscriptionInfo) null;
      CommerceBillingSubscriptionInfo subscriptionInfo2 = (CommerceBillingSubscriptionInfo) null;
      try
      {
        IArmAdapterService armAdapterService = service2;
        IVssRequestContext requestContext2 = deploymentContext;
        Guid subscriptionId = azureSubscriptionId;
        Guid? tenantId1;
        if (billingContext == null)
        {
          nullable1 = new Guid?();
          tenantId1 = nullable1;
        }
        else
          tenantId1 = billingContext.TenantId;
        subscriptionInfo1 = armAdapterService.GetSubscriptionForUser(requestContext2, subscriptionId, AzureErrorBehavior.Throw, tenantId1);
        if (subscriptionInfo1?.QuotaId != "EnterpriseAgreement_2014-09-01")
        {
          if (subscriptionInfo1?.QuotaId != "CSP_2015-05-01")
            subscriptionInfo2 = service1.GetBillingSubscriptionInfo(deploymentContext, azureSubscriptionId, billingContext, AzureErrorBehavior.Throw);
        }
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(5109262, "Commerce", nameof (RiskEvaluationService), ex);
      }
      paymentInstrumentId = subscriptionInfo2?.PaymentInstrumentId ?? "unknown";
      offerCode = subscriptionInfo2?.OfferCode ?? "unknown";
      subscriptionStatus = subscriptionInfo2 != null ? subscriptionInfo2.Status : (subscriptionInfo1 != null ? subscriptionInfo1.Status : SubscriptionStatus.Unknown);
      quotaId = subscriptionInfo1?.QuotaId ?? "unknown";
      deploymentContext.TraceAlways(5109232, TraceLevel.Info, "Commerce", nameof (RiskEvaluationService), string.Format("Billing Subcription Info Details State: {0} ", (object) subscriptionStatus) + "PaymentInstrumentId: " + paymentInstrumentId + " OfferId: " + offerCode + ". QuotaId: " + quotaId);
    }

    private string GetPuid(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string property = identity.GetProperty<string>("PUID", string.Empty);
      int startIndex = property.IndexOf("aad:");
      return startIndex >= 0 ? property.Remove(startIndex, "aad:".Length) : property;
    }

    private RiskEvaluationRequestPayload ContructRiskEvaluationPayload(
      string ipAddress,
      string userAgent,
      string emailAddress,
      string puid,
      string idNamespace,
      string currencyCode,
      string localeCode,
      double priceValue,
      Guid platformMeterId,
      string skuId,
      Guid? sessionId,
      string paymentInstrumentId,
      string offerCode,
      SubscriptionStatus azureSubscriptionStatus,
      Guid azureSubscriptionId,
      int changedQuantity,
      string QuotaId)
    {
      string str1 = "unknown";
      RiskEvaluationAccountDetails evaluationAccountDetails = new RiskEvaluationAccountDetails()
      {
        email = emailAddress,
        id = puid,
        id_namespace = idNamespace,
        account_id = string.Format("g.{0}", (object) Guid.NewGuid())
      };
      double num = priceValue * (double) changedQuantity;
      RiskEvaluationBillingDetails evaluationBillingDetails = new RiskEvaluationBillingDetails()
      {
        currency = currencyCode,
        amount = num
      };
      RiskEvaluationOrderItem evaluationOrderItem = new RiskEvaluationOrderItem()
      {
        line_item_id = "1",
        product_id = platformMeterId,
        product_type = RiskEvaluationOrderItem.GetProductType(platformMeterId),
        sku_id = skuId,
        availability_id = "unknown",
        quantity = changedQuantity,
        list_price = num,
        retail_price = num,
        price_currency_code = currencyCode
      };
      if (priceValue == -1.0)
      {
        evaluationBillingDetails.amount = -1.0;
        evaluationOrderItem.list_price = -1.0;
        evaluationOrderItem.retail_price = -1.0;
      }
      RiskEvaluationCatalogDetails evaluationCatalogDetails = new RiskEvaluationCatalogDetails()
      {
        provider = "unknown",
        display_catalog_id = "unknown",
        language = "unknown",
        market = "unknown"
      };
      RiskEvaluationDeviceDetails evaluationDeviceDetails = new RiskEvaluationDeviceDetails()
      {
        id_source = "unknown",
        ip_address = string.IsNullOrEmpty(ipAddress) ? "unknown" : ipAddress,
        green_id = sessionId ?? Guid.Empty,
        id = string.IsNullOrEmpty(userAgent) ? "unknown" : userAgent,
        locale = localeCode,
        device_type = "unknown"
      };
      RiskEvaluationSubscriptionDetails subscriptionDetails = new RiskEvaluationSubscriptionDetails()
      {
        subscription_id = azureSubscriptionId,
        subscription_status = azureSubscriptionStatus.ToString()
      };
      string str2 = string.IsNullOrEmpty(QuotaId) ? "unknown" : QuotaId;
      RiskEvaluationPaymentInstrument paymentInstrument = new RiskEvaluationPaymentInstrument()
      {
        payment_instrument_id = paymentInstrumentId
      };
      return new RiskEvaluationRequestPayload()
      {
        event_type = "purchase",
        event_details = new RiskEvaluationEventDetails()
        {
          order_id = str1,
          account_details = evaluationAccountDetails,
          billing_details = evaluationBillingDetails,
          order_line_items = new List<RiskEvaluationOrderItem>((IEnumerable<RiskEvaluationOrderItem>) new RiskEvaluationOrderItem[1]
          {
            evaluationOrderItem
          }),
          catalog_details = evaluationCatalogDetails,
          client = "azure_devops",
          device_details = evaluationDeviceDetails,
          subscription_details = subscriptionDetails,
          payment_instruments = new List<RiskEvaluationPaymentInstrument>((IEnumerable<RiskEvaluationPaymentInstrument>) new RiskEvaluationPaymentInstrument[1]
          {
            paymentInstrument
          }),
          offer_details = new RiskEvaluationOfferDetails()
          {
            offer_id = offerCode
          },
          quota_id = str2
        }
      };
    }

    private void WriteRiskEvaluationResultCustomerEvent(
      IVssRequestContext deploymentContext,
      Guid azureSubscriptionId,
      Guid hostId,
      Guid? sessionId,
      Guid? tenantId,
      Guid? objectId,
      string galleryId,
      int quantity,
      RiskEvaluationResult riskEvaluationResult,
      string riskResponseId)
    {
      try
      {
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        eventData.Add("SubscriptionId", (object) azureSubscriptionId);
        eventData.Add("IdentityTenant", (object) tenantId);
        eventData.Add("ObjectId", (object) objectId);
        eventData.Add("HostId", (object) hostId);
        eventData.Add("GalleryId", galleryId);
        eventData.Add("Quantity", (double) quantity);
        eventData.Add("SessionId", (object) sessionId);
        eventData.Add("RiskEvaluationResult", riskEvaluationResult.ToString());
        eventData.Add("RiskResponseId", riskResponseId);
        CustomerIntelligence.PublishEvent(deploymentContext, "RiskEvaluation", eventData);
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(5109231, "Commerce", nameof (RiskEvaluationService), ex);
      }
    }

    private RiskEvaluationResult ParseRiskEvaluationResult(
      IVssRequestContext requestContext,
      HttpContent content,
      out string riskResponseId)
    {
      string result = content.ReadAsStringAsync().Result;
      RiskEvaluationResponse evaluationResponse = JsonConvert.DeserializeObject<RiskEvaluationResponse>(result);
      string decision = evaluationResponse?.decision;
      riskResponseId = evaluationResponse?.Id;
      requestContext.TraceAlways(5109237, TraceLevel.Info, "Commerce", nameof (RiskEvaluationService), "jsonString: " + result + " decision: " + decision + " riskResponseId: " + riskResponseId);
      switch (decision)
      {
        case "Approved":
          return RiskEvaluationResult.Approved;
        case "Review":
          return RiskEvaluationResult.Review;
        case "Rejected":
          return RiskEvaluationResult.Rejected;
        default:
          return RiskEvaluationResult.Unknown;
      }
    }

    private class ServiceSettings
    {
      private const string commerceModernRiskApiBaseUrlDefaultValue = "https://ks.cp.microsoft-int.com/risk/risk-evaluation";
      private const int modernRiskApiRequestDefaultTimeout = 90000;

      public ServiceSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, RiskEvaluationService.s_commerceRiskEvaluationRegistryQuery);
        this.ModernRiskBaseUri = new Uri(registryEntryCollection.GetValueFromPath<string>("BaseUrl", "https://ks.cp.microsoft-int.com/risk/risk-evaluation"));
        this.ModernRiskApiVersion = registryEntryCollection.GetValueFromPath<string>("ApiVersion", "2015-02-28");
        this.RequestTimeOut = registryEntryCollection.GetValueFromPath<int>(nameof (RequestTimeOut), 90000);
        if (this.RequestTimeOut > 0)
          return;
        this.RequestTimeOut = 90000;
      }

      public Uri ModernRiskBaseUri { get; private set; }

      public string ModernRiskApiVersion { get; private set; }

      public int RequestTimeOut { get; private set; }
    }
  }
}
