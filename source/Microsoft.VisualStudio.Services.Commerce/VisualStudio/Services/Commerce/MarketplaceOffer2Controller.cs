// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.MarketplaceOffer2Controller
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
  public class MarketplaceOffer2Controller : CommerceControllerBase
  {
    public static int OfferMeterNotFound = 101;
    private readonly IVssDateTimeProvider dateTimeProvider;

    public MarketplaceOffer2Controller() => this.dateTimeProvider = VssDateTimeProvider.DefaultProvider;

    internal MarketplaceOffer2Controller(
      HttpControllerContext controllerContext,
      IVssDateTimeProvider dateTimeProvider)
    {
      this.Initialize(controllerContext);
      this.dateTimeProvider = dateTimeProvider;
    }

    [HttpPost]
    [TraceDetailsFilter(5107370, 5107376)]
    [TraceExceptions(5107371)]
    public HttpResponseMessage CreateOfferMeterDefinitionFromMarketplace(
      [FromBody] MarketplaceOfferContract2 requestData)
    {
      ArgumentUtility.CheckForNull<MarketplaceOfferContract2>(requestData, nameof (requestData));
      ArgumentUtility.CheckForNull<AssetDetails>(requestData.AssetDetails, "AssetDetails");
      ArgumentUtility.CheckStringForNullOrEmpty(requestData.AssetDetails.OfferTypeId, "OfferTypeId");
      ArgumentUtility.CheckStringForNullOrEmpty(requestData.AssetDetails.Id, "Id");
      ArgumentUtility.CheckStringForNullOrEmpty(requestData.PublisherDisplayName, "PublisherDisplayName");
      ArgumentUtility.CheckStringForNullOrEmpty(requestData.PublisherId, "PublisherId");
      ArgumentUtility.CheckForNull<Definition>(requestData.AssetDetails.Definition, "Definition");
      ArgumentUtility.CheckForNull<List<OfferPlan>>(requestData.AssetDetails.Definition.Plans, "Plans");
      bool isPublic = requestData.Operation == RESTApiRequestOperationType2.Production;
      string offerName = requestData.AssetDetails.Id;
      string offerId = requestData.AssetDetails.Id;
      string publisherName = requestData.PublisherDisplayName;
      string publisherId = requestData.PublisherId;
      string galleryId = publisherId + "." + offerId;
      this.TfsRequestContext.Trace(5107372, TraceLevel.Info, this.Area, this.Layer, string.Format("galleryId: {0}, offerId: {1}, offerName: {2}, isPublic: {3}", (object) galleryId, (object) offerId, (object) offerName, (object) isPublic));
      try
      {
        IOfferMeterService service = this.TfsRequestContext.GetService<IOfferMeterService>();
        IOfferMeter offerMeter = service.GetOfferMeter(this.TfsRequestContext, galleryId);
        if (offerMeter == null)
          return this.CreateResponseMessage(HttpStatusCode.OK, RestApiResponseStatus.Failed, HostingResources.MarketplaceOfferControllerError((object) MarketplaceOffer2Controller.OfferMeterNotFound, (object) HostingResources.OfferMeterNotFound((object) galleryId)), requestData);
        offerMeter.FixedQuantityPlans = (IEnumerable<AzureOfferPlanDefinition>) requestData.AssetDetails.Definition.Plans.Select<OfferPlan, AzureOfferPlanDefinition>((Func<OfferPlan, AzureOfferPlanDefinition>) (p => new AzureOfferPlanDefinition()
        {
          Publisher = publisherId,
          PlanId = p.PlanId,
          PlanName = p.VsMarketplaceExtensionsSkuTitle,
          OfferName = offerName,
          OfferId = offerId,
          Quantity = p.VsMarketplaceExtensionsSkuUsers,
          IsPublic = isPublic,
          PublisherName = publisherName,
          PlanVersion = requestData.AssetVersion.ToString()
        })).ToList<AzureOfferPlanDefinition>();
        this.TfsRequestContext.TraceProperties<IEnumerable<AzureOfferPlanDefinition>>(5107375, this.Area, this.Layer, offerMeter.FixedQuantityPlans, (string) null);
        if (isPublic)
        {
          try
          {
            this.TfsRequestContext.GetExtension<IDataMarketPriceHandler>();
            List<OfferMeterPrice> list = requestData.AssetDetails.Definition.Plans.SelectMany<OfferPlan, OfferMeterPrice>((Func<OfferPlan, IEnumerable<OfferMeterPrice>>) (p =>
            {
              OfferPlan offerPlan = p;
              bool? nullable;
              if (offerPlan == null)
              {
                nullable = new bool?();
              }
              else
              {
                MonthlyPricing monthlyPricing = offerPlan.MonthlyPricing;
                if (monthlyPricing == null)
                {
                  nullable = new bool?();
                }
                else
                {
                  Dictionary<string, RegionPrice> regionPrices = monthlyPricing.RegionPrices;
                  nullable = regionPrices != null ? new bool?(regionPrices.Values.IsNullOrEmpty<RegionPrice>()) : new bool?();
                }
              }
              return ((int) nullable ?? 1) != 0 ? Enumerable.Empty<OfferMeterPrice>() : p.MonthlyPricing.RegionPrices.Select<KeyValuePair<string, RegionPrice>, OfferMeterPrice>((Func<KeyValuePair<string, RegionPrice>, OfferMeterPrice>) (rp => new OfferMeterPrice()
              {
                CurrencyCode = rp.Value.Currency,
                MeterName = offerName,
                PlanName = p.PlanId,
                Price = (double) rp.Value.Price,
                Quantity = (double) p.VsMarketplaceExtensionsSkuUsers,
                Region = rp.Key
              }));
            })).ToList<OfferMeterPrice>();
            if (!list.IsNullOrEmpty<OfferMeterPrice>())
              this.UpdatePricing(this.TfsRequestContext, offerMeter.GalleryId, list);
          }
          catch (Exception ex)
          {
            this.TfsRequestContext.TraceException(5107371, this.Area, this.Layer, ex);
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
        offerMeter.Name = requestData.AssetDetails.Definition.DisplayText;
        service.CreateOfferMeterDefinition(this.TfsRequestContext, offerMeter);
        return this.CreateResponseMessage(HttpStatusCode.OK, RestApiResponseStatus.Completed, HostingResources.Success(), requestData);
      }
      catch (InvalidOperationException ex)
      {
        this.TfsRequestContext.TraceException(5107371, this.Area, this.Layer, (Exception) ex);
        return this.CreateResponseMessage(HttpStatusCode.OK, RestApiResponseStatus.Failed, ex.Message, requestData);
      }
      catch (VssServiceResponseException ex) when (ex.InnerException is InvalidOperationException)
      {
        this.TfsRequestContext.TraceException(5107371, this.Area, this.Layer, (Exception) ex);
        return this.CreateResponseMessage(HttpStatusCode.OK, RestApiResponseStatus.Failed, ex.Message, requestData);
      }
      catch (AccessCheckException ex)
      {
        this.TfsRequestContext.TraceException(5107371, this.Area, this.Layer, (Exception) ex);
        return this.CreateResponseMessage(HttpStatusCode.OK, RestApiResponseStatus.Failed, ex.Message, requestData);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5107371, this.Area, this.Layer, ex);
        return this.CreateResponseMessage(HttpStatusCode.InternalServerError, RestApiResponseStatus.Failed, HostingResources.ErrorAddingPlans((object) this.TfsRequestContext.ActivityId), requestData);
      }
    }

    internal HttpResponseMessage CreateResponseMessage(
      HttpStatusCode statusCode,
      RestApiResponseStatus responseStatus,
      string message,
      MarketplaceOfferContract2 requestData)
    {
      RestApiResponseStatusModel responseStatusModel = new RestApiResponseStatusModel()
      {
        Status = responseStatus,
        StatusMessage = message,
        OperationId = this.TfsRequestContext.ActivityId.ToString("D"),
        PercentageCompleted = responseStatus == RestApiResponseStatus.Completed ? 100 : 0
      };
      requestData.OperationStatus = responseStatusModel;
      return this.Request.CreateResponse<MarketplaceOfferContract2>(statusCode, requestData);
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

    internal override string Layer => nameof (MarketplaceOffer2Controller);
  }
}
