// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureStoreService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class AzureStoreService : AzureClientBase, IAzureStoreService, IVssFrameworkService
  {
    private static readonly RegistryQuery baseRegistryQuery = (RegistryQuery) "/Service/Commerce/*";
    private static readonly RegistryQuery storeApiRegistryQuery = (RegistryQuery) "/Service/Commerce/AzureStoreApi/*";
    private static readonly RegistryQuery storeRatingApiRegistryQuery = (RegistryQuery) "/Service/Commerce/AzureStoreRatingApi/*";
    private static readonly RegistryQuery billingRegistryQuery = (RegistryQuery) "/Service/Commerce/AzureBilling/*";
    private AzureStoreService.ServiceSettings serviceSettings;
    private const string Area = "Commerce";
    private const string Layer = "AzureStoreService";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in AzureStoreService.baseRegistryQuery);
      Interlocked.CompareExchange<AzureStoreService.ServiceSettings>(ref this.serviceSettings, new AzureStoreService.ServiceSettings(requestContext), (AzureStoreService.ServiceSettings) null);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));
    }

    public List<KeyValuePair<double, double>> GetMeterPricing(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      OfferMeter offerMeter,
      CommerceBillingAccountInfo billingAccountInfo,
      out string currencyCode)
    {
      requestContext.TraceEnter(5106781, "Commerce", nameof (AzureStoreService), nameof (GetMeterPricing));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      string currencyCode1 = string.Empty;
      try
      {
        return this.GetRatingResponse(requestContext, subscriptionId, offerMeter, billingAccountInfo, out currencyCode1);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106783, "Commerce", nameof (AzureStoreService), ex);
        throw;
      }
      finally
      {
        currencyCode = currencyCode1;
        requestContext.TraceLeave(5106782, "Commerce", nameof (AzureStoreService), nameof (GetMeterPricing));
      }
    }

    public IDictionary<string, string[]> GetOfferAvailableRegions(
      IVssRequestContext requestContext,
      IOfferMeter offerMeter)
    {
      IEnumerable<AzureOfferPlanDefinition> source = !offerMeter.FixedQuantityPlans.IsNullOrEmpty<AzureOfferPlanDefinition>() ? offerMeter.FixedQuantityPlans : throw new ArgumentException("Offer meter fixed quantity plans must contain some offer.");
      AzureOfferPlanDefinition firstPlan = offerMeter.FixedQuantityPlans.First<AzureOfferPlanDefinition>();
      if (firstPlan == (AzureOfferPlanDefinition) null)
        throw new ArgumentException("Offer meter fixed quantity plans may not be null.");
      Func<AzureOfferPlanDefinition, bool> predicate = (Func<AzureOfferPlanDefinition, bool>) (x => !string.Equals(firstPlan.Publisher, x.Publisher, StringComparison.OrdinalIgnoreCase) && !string.Equals(firstPlan.OfferId, x.OfferId, StringComparison.OrdinalIgnoreCase));
      if (source.Any<AzureOfferPlanDefinition>(predicate))
        requestContext.Trace(5108838, TraceLevel.Error, "Commerce", nameof (AzureStoreService), "Ambiguous offer plan definition for meter " + offerMeter.Name + ". Call directly for the desired publisher/offer.");
      return this.GetOfferAvailableRegions(requestContext, firstPlan.Publisher, firstPlan.OfferId);
    }

    public IDictionary<string, string[]> GetOfferAvailableRegions(
      IVssRequestContext requestContext,
      string publisher,
      string offerId)
    {
      Uri serviceUri = this.PrepareUri(this.serviceSettings.StoreBaseUri, this.serviceSettings.StoreApiVersion, urlPath: "/catalog/publishers/" + publisher + "/offers/" + offerId);
      HttpResponseMessage response = this.GetResponse<object>(requestContext, serviceUri, (object) null, HttpMethod.Get);
      if (response?.Content == null)
        return (IDictionary<string, string[]>) null;
      CatalogOfferResponse result = response.Content.ReadAsAsync<CatalogOfferResponse>((IEnumerable<MediaTypeFormatter>) new MediaTypeFormatter[1]
      {
        (MediaTypeFormatter) CommerceHttpHelper.JsonMediaTypeFormatter
      }).Result;
      if (!string.IsNullOrEmpty(result?.error?.code))
        throw new AzureResponseException(result.error.message, (string) null);
      if (result?.marketingMaterial?.servicePlanDescriptions == null)
        throw new AzureResponseException("Failed to get service plan descriptions", (string) null);
      return (IDictionary<string, string[]>) ((IEnumerable<CatalogMarketingServicePlan>) result.marketingMaterial.servicePlanDescriptions).Where<CatalogMarketingServicePlan>((Func<CatalogMarketingServicePlan, bool>) (servicePlan => !string.IsNullOrWhiteSpace(servicePlan.servicePlanNaturalIdentifier))).ToDictionary<CatalogMarketingServicePlan, string, string[]>((Func<CatalogMarketingServicePlan, string>) (servicePlan => servicePlan.servicePlanNaturalIdentifier), (Func<CatalogMarketingServicePlan, string[]>) (servicePlan => servicePlan.regionsAvailable));
    }

    private List<KeyValuePair<double, double>> GetRatingResponse(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      OfferMeter offerMeter,
      CommerceBillingAccountInfo billingAccountInfo,
      out string currencyCode)
    {
      List<KeyValuePair<double, double>> ratingResponse = new List<KeyValuePair<double, double>>();
      currencyCode = string.Empty;
      RatingRequest ratingRequest = this.PrepareRatingRequest(subscriptionId, offerMeter, billingAccountInfo);
      Uri serviceUri = this.PrepareUri(this.serviceSettings.StoreBaseUri, this.serviceSettings.StoreApiVersion, urlPath: "/rating");
      HttpResponseMessage response = this.GetResponse<RatingRequest>(requestContext, serviceUri, ratingRequest, HttpMethod.Post);
      if ((response?.Content == null ? 0 : (response.IsSuccessStatusCode ? 1 : 0)) == 0)
        return ratingResponse;
      List<Product> priceInfo = response.Content.ReadAsAsync<RatingResponse>((IEnumerable<MediaTypeFormatter>) new MediaTypeFormatter[1]
      {
        (MediaTypeFormatter) CommerceHttpHelper.JsonMediaTypeFormatter
      }).Result.products;
      if (priceInfo != null)
        requestContext.TraceConditionally(5106785, TraceLevel.Info, "Commerce", nameof (AzureStoreService), (Func<string>) (() => string.Format("Received {0} price results for offer meter {1} ", (object) priceInfo.Count, (object) offerMeter.GalleryId)));
      foreach (Product product1 in priceInfo.Where<Product>((Func<Product, bool>) (p => p.unitPrices != null)))
      {
        Product product = product1;
        try
        {
          AzureOfferPlanDefinition offerPlanDefinition = offerMeter.FixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (x => x.PlanId == product.id));
          if (!(offerPlanDefinition == (AzureOfferPlanDefinition) null))
          {
            requestContext.Trace(5106786, TraceLevel.Info, "Commerce", nameof (AzureStoreService), string.Format("Received currency code {0} for offer meter {1}, subscription currency code {2} and subscriptionId {3}", (object) product.unitPrices.currency, (object) offerMeter.GalleryId, (object) billingAccountInfo.CurrencyCode, (object) subscriptionId));
            currencyCode = product.unitPrices.currency;
            if (product.unitPrices.prices != null)
            {
              double amount = product.unitPrices.prices.First<UnitPrice>().priceRules.First<PriceRule>().amount;
              ratingResponse.Add(new KeyValuePair<double, double>((double) offerPlanDefinition.Quantity, amount));
            }
            else
              ratingResponse.Add(new KeyValuePair<double, double>((double) offerPlanDefinition.Quantity, product.unitPrices.costPerTerm));
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5106784, "Commerce", nameof (AzureStoreService), ex);
        }
      }
      return ratingResponse;
    }

    private HttpResponseMessage GetResponse<T>(
      IVssRequestContext requestContext,
      Uri serviceUri,
      T ratingRequest,
      HttpMethod method)
    {
      ICommerceHttpHelper service = requestContext.GetService<ICommerceHttpHelper>();
      int storeRequestTimeOut = this.serviceSettings.StoreRequestTimeOut;
      X509Certificate2 storedCertificate = this.GetStoredCertificate(requestContext, this.serviceSettings.CertificateThumbprint);
      HttpClient clientWithCertificate = service.GetHttpClientWithCertificate(requestContext, storedCertificate, storeRequestTimeOut);
      return service.GetHttpResponseMessage(requestContext, clientWithCertificate, serviceUri, method, (object) ratingRequest, storeRequestTimeOut);
    }

    internal virtual X509Certificate2 GetStoredCertificate(
      IVssRequestContext requestContext,
      string certificateThumbprint)
    {
      return new CommerceCertificateHelper().InstantiateCertificate(requestContext, certificateThumbprint);
    }

    private RatingRequest PrepareRatingRequest(
      Guid subscriptionId,
      OfferMeter offerMeter,
      CommerceBillingAccountInfo billingAccountInfo)
    {
      return new RatingRequest()
      {
        subscriptionId = subscriptionId.ToString(),
        accountCurrency = billingAccountInfo.CurrencyCode,
        accountRegion = billingAccountInfo.Region,
        products = offerMeter.FixedQuantityPlans.Select<AzureOfferPlanDefinition, Product>((Func<AzureOfferPlanDefinition, Product>) (offer => new Product()
        {
          publisher = offer.Publisher,
          offer = offer.OfferId,
          plan = offer.PlanName,
          id = offer.PlanId
        })).ToList<Product>()
      };
    }

    internal void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<AzureStoreService.ServiceSettings>(ref this.serviceSettings, new AzureStoreService.ServiceSettings(requestContext));
    }

    private class ServiceSettings
    {
      private const string defaultStoreApiVersion = "2015-01-01";
      private const string azureStoreApiBaseUrlDefaultValue = "https://storeapi.azure.com/";
      private const int azureStoreApiDefaultTimeout = 10000;

      public ServiceSettings(IVssRequestContext requestContext)
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        this.ReadStoreApiSettings(requestContext, service);
        this.ReadAzureBillingSettings(requestContext, service);
      }

      private void ReadStoreApiSettings(
        IVssRequestContext requestContext,
        IVssRegistryService registryService)
      {
        RegistryEntryCollection registryEntryCollection = registryService.ReadEntries(requestContext, AzureStoreService.storeApiRegistryQuery);
        this.StoreApiVersion = registryEntryCollection.GetValueFromPath<string>("Version", "2015-01-01");
        this.StoreRequestTimeOut = registryEntryCollection.GetValueFromPath<int>("Timeout", 10000);
        if (this.StoreRequestTimeOut <= 0)
          this.StoreRequestTimeOut = 10000;
        this.StoreBaseUri = new Uri(registryService.ReadEntries(requestContext, AzureStoreService.storeRatingApiRegistryQuery).GetValueFromPath<string>("BaseUrl", "https://storeapi.azure.com/"));
      }

      private void ReadAzureBillingSettings(
        IVssRequestContext requestContext,
        IVssRegistryService registryService)
      {
        this.CertificateThumbprint = registryService.ReadEntries(requestContext, AzureStoreService.billingRegistryQuery).GetValueFromPath<string>("CertificateThumbprint", string.Empty);
      }

      public string CertificateThumbprint { get; private set; }

      public Uri StoreBaseUri { get; private set; }

      public string StoreApiVersion { get; private set; }

      public int StoreRequestTimeOut { get; private set; }
    }
  }
}
