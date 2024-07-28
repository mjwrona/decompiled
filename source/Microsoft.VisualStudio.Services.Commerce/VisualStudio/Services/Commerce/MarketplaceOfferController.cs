// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.MarketplaceOfferController
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowXml = true)]
  public class MarketplaceOfferController : CommerceControllerBase
  {
    private readonly IVssDateTimeProvider dateTimeProvider;

    public MarketplaceOfferController() => this.dateTimeProvider = VssDateTimeProvider.DefaultProvider;

    internal MarketplaceOfferController(
      HttpControllerContext controllerContext,
      IVssDateTimeProvider dateTimeProvider)
    {
      this.Initialize(controllerContext);
      this.dateTimeProvider = dateTimeProvider;
    }

    internal override string Layer => nameof (MarketplaceOfferController);

    [HttpPost]
    [TraceDetailsFilter(5107350, 5107359)]
    [TraceExceptions(5107351)]
    public HttpResponseMessage CreateOfferMeterDefinitionFromMarketPlace(
      [FromBody] MarketplaceOfferContract requestData)
    {
      ArgumentUtility.CheckForNull<MarketplaceOfferContract>(requestData, nameof (requestData));
      ArgumentUtility.CheckForNull<AssetDetailObject>(requestData.AssetDetails, "AssetDetails");
      ArgumentUtility.CheckStringForNullOrEmpty(requestData.AssetDetails.PublisherNaturalIdentifier, "PublisherNaturalIdentifier");
      ArgumentUtility.CheckStringForNullOrEmpty(requestData.AssetDetails.PublisherName, "PublisherName");
      ArgumentUtility.CheckForNull<AnswersDetails>(requestData.AssetDetails.Answers, "Answers");
      ArgumentUtility.CheckStringForNullOrEmpty(requestData.AssetDetails.Answers.VSMarketplacePublisherName, "VSMarketplacePublisherName");
      ArgumentUtility.CheckStringForNullOrEmpty(requestData.AssetDetails.Answers.VSMarketplaceExtensionName, "VSMarketplaceExtensionName");
      ArgumentUtility.CheckForNull<string>(requestData.AssetDetails.ProductTypeNaturalIdentifier, "ProductTypeNaturalIdentifier");
      ArgumentUtility.CheckForNull<string>(requestData.AssetDetails.ServiceNaturalIdentifier, "ServiceNaturalIdentifier");
      bool isPublic = requestData.Operation == RESTApiRequestOperationType.Production;
      string offerName = requestData.AssetDetails.ProductTypeNaturalIdentifier;
      string offerId = requestData.AssetDetails.ServiceNaturalIdentifier;
      string publisherName = requestData.AssetDetails.PublisherName;
      string publisherId = requestData.AssetDetails.PublisherNaturalIdentifier;
      string galleryId = requestData.AssetDetails.Answers.VSMarketplacePublisherName + "." + requestData.AssetDetails.Answers.VSMarketplaceExtensionName;
      this.TfsRequestContext.Trace(5107352, TraceLevel.Info, this.Area, this.Layer, string.Format("galleryId: {0}, offerId: {1}, offerName: {2}, isPublic: {3}", (object) galleryId, (object) offerId, (object) offerName, (object) isPublic));
      this.TfsRequestContext.TraceProperties<MarketplaceOfferContract>(5107354, this.Area, this.Layer, requestData, (string) null);
      try
      {
        IOfferMeterService service = this.TfsRequestContext.GetService<IOfferMeterService>();
        IOfferMeter offerMeter = service.GetOfferMeter(this.TfsRequestContext, galleryId);
        if (offerMeter == null)
          return this.CreateResponseMessage(HttpStatusCode.OK, RestApiResponseStatus.Failed, HostingResources.MarketplaceOfferControllerError((object) MarketplaceOfferController.MarketplaceOfferControllerErrorCodes.OfferMeterNotFound, (object) HostingResources.OfferMeterNotFound((object) galleryId)), requestData);
        offerMeter.FixedQuantityPlans = (IEnumerable<AzureOfferPlanDefinition>) requestData.AssetDetails.AnswersPerPlan.Keys.Select<string, AzureOfferPlanDefinition>((Func<string, AzureOfferPlanDefinition>) (p => new AzureOfferPlanDefinition()
        {
          Publisher = publisherId,
          PlanId = p,
          PlanName = p,
          OfferName = offerName,
          OfferId = offerId,
          Quantity = requestData.AssetDetails.AnswersPerPlan[p].PlanUsers,
          IsPublic = isPublic,
          PublisherName = publisherName,
          PlanVersion = requestData.AssetVersion.ToString()
        })).ToList<AzureOfferPlanDefinition>();
        this.TfsRequestContext.TraceProperties<IEnumerable<AzureOfferPlanDefinition>>(5107355, this.Area, this.Layer, offerMeter.FixedQuantityPlans, (string) null);
        if (isPublic)
        {
          try
          {
            List<OfferMeterPrice> priceFromServicePlan = this.TfsRequestContext.GetExtension<IDataMarketPriceHandler>().GetOfferMeterPriceFromServicePlan(this.TfsRequestContext, galleryId, offerMeter.FixedQuantityPlans, requestData.AssetDetails.ServicePlansByMarket);
            if (!priceFromServicePlan.IsNullOrEmpty<OfferMeterPrice>())
              this.UpdatePricing(this.TfsRequestContext, offerMeter.GalleryId, priceFromServicePlan);
          }
          catch (Exception ex)
          {
            this.TfsRequestContext.TraceException(5107351, this.Area, this.Layer, ex);
          }
          DateTime dateTime1 = this.dateTimeProvider.UtcNow;
          DateTime dateTime2 = dateTime1.AddHours(4.0);
          if (offerMeter.BillingStartDate.HasValue)
          {
            DateTime? billingStartDate = offerMeter.BillingStartDate;
            dateTime1 = dateTime2;
            if ((billingStartDate.HasValue ? (billingStartDate.GetValueOrDefault() > dateTime1 ? 1 : 0) : 0) == 0)
              goto label_10;
          }
          offerMeter.BillingStartDate = new DateTime?(dateTime2);
        }
label_10:
        Dictionary<string, LangDetails> dictionary = new Dictionary<string, LangDetails>((IDictionary<string, LangDetails>) requestData.AssetDetails.Languages, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        if (dictionary.ContainsKey("en-us"))
          offerMeter.Name = dictionary["en-us"].Title;
        service.CreateOfferMeterDefinition(this.TfsRequestContext, offerMeter);
        return this.CreateResponseMessage(HttpStatusCode.OK, RestApiResponseStatus.Completed, HostingResources.Success(), requestData);
      }
      catch (InvalidOperationException ex)
      {
        this.TfsRequestContext.TraceException(5107351, this.Area, this.Layer, (Exception) ex);
        return this.CreateResponseMessage(HttpStatusCode.OK, RestApiResponseStatus.Failed, ex.Message, requestData);
      }
      catch (VssServiceResponseException ex) when (ex.InnerException is InvalidOperationException)
      {
        this.TfsRequestContext.TraceException(5107351, this.Area, this.Layer, (Exception) ex);
        return this.CreateResponseMessage(HttpStatusCode.OK, RestApiResponseStatus.Failed, ex.Message, requestData);
      }
      catch (AccessCheckException ex)
      {
        this.TfsRequestContext.TraceException(5107351, this.Area, this.Layer, (Exception) ex);
        return this.CreateResponseMessage(HttpStatusCode.OK, RestApiResponseStatus.Failed, ex.Message, requestData);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5107351, this.Area, this.Layer, ex);
        return this.CreateResponseMessage(HttpStatusCode.InternalServerError, RestApiResponseStatus.Failed, HostingResources.ErrorAddingPlans((object) this.TfsRequestContext.ActivityId), requestData);
      }
    }

    internal HttpResponseMessage CreateResponseMessage(
      HttpStatusCode statusCode,
      RestApiResponseStatus responseStatus,
      string message,
      MarketplaceOfferContract marketplaceOffer)
    {
      RestApiResponseStatusModel responseStatusModel = new RestApiResponseStatusModel()
      {
        Status = responseStatus,
        StatusMessage = message,
        OperationId = this.TfsRequestContext.ActivityId.ToString("D"),
        PercentageCompleted = responseStatus == RestApiResponseStatus.Completed ? 100 : 0
      };
      marketplaceOffer.operationStatus = responseStatusModel;
      return this.Request.CreateResponse<MarketplaceOfferContract>(statusCode, marketplaceOffer);
    }

    private void UpdatePricing(
      IVssRequestContext requestContext,
      string galleryId,
      List<OfferMeterPrice> offerMeterPrice)
    {
      requestContext.Trace(5107360, TraceLevel.Info, this.Area, this.Layer, string.Format("Inserting price from offer meter definition for {0} count: {1}", (object) galleryId, (object) offerMeterPrice.Count));
      requestContext.GetService<IPricingService>().UpdatePricing(requestContext, galleryId, offerMeterPrice);
      if (!requestContext.IsSpsService() || !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DualWriteOfferMeterDefinitionsToSps"))
        return;
      requestContext.Trace(5107361, TraceLevel.Info, this.Area, this.Layer, string.Format("Dual write price to commerce from offer meter definition for {0} count: {1}", (object) galleryId, (object) offerMeterPrice.Count));
      requestContext.GetClient<OfferMeterHttpClient>().UpdateOfferMeterPriceAsync((IEnumerable<OfferMeterPrice>) offerMeterPrice, galleryId, (object) "GalleryId").SyncResult();
    }

    public static class MarketplaceOfferControllerErrorCodes
    {
      public static int OfferMeterNotFound = 101;
    }
  }
}
