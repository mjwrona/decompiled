// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscriptionControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowXml = true)]
  public abstract class OfferSubscriptionControllerBase : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static OfferSubscriptionControllerBase()
    {
      OfferSubscriptionControllerBase.httpExceptions.Add(typeof (CommerceSecurityException), HttpStatusCode.Unauthorized);
      OfferSubscriptionControllerBase.httpExceptions.Add(typeof (InvalidResourceException), HttpStatusCode.BadRequest);
      OfferSubscriptionControllerBase.httpExceptions.Add(typeof (AzureSubscriptionDisabledException), HttpStatusCode.Forbidden);
      OfferSubscriptionControllerBase.httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
    }

    public OfferSubscriptionControllerBase()
    {
    }

    internal OfferSubscriptionControllerBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    public override string ActivityLogArea => "Commerce";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) OfferSubscriptionControllerBase.httpExceptions;

    [HttpPost]
    [TraceFilter(5106900, 5106910)]
    [TraceExceptions(5108780)]
    public virtual void CreateOfferSubscription(
      OfferSubscription offerSubscription,
      string offerCode = null,
      Guid? tenantId = null,
      Guid? objectId = null,
      Guid? billingTarget = null,
      bool skipSubscriptionValidation = false)
    {
      string queryStringToken = CommerceUtil.TryGetParseQueryStringToken(this.Request.Headers?.Referrer?.Query, "source");
      string source = queryStringToken != string.Empty ? queryStringToken : "Marketplace";
      if (CommerceUtil.IsBillingTabUrl(this.Request.Headers?.Referrer))
        source = "BillingTab";
      CommerceUtil.SetRequestSource(this.TfsRequestContext, source);
      Guid guid;
      HttpRequestMessageExtensions.TryGetHeaderGuid((System.Net.Http.Headers.HttpHeaders) this.Request.Headers, "X-TFS-Session", out guid);
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableSkipSubscriptionCheckOfferSubscription"))
        this.TfsRequestContext.GetService<PlatformOfferSubscriptionService>().CreateOfferSubscription(this.TfsRequestContext, offerSubscription.OfferMeter.GalleryId, offerSubscription.AzureSubscriptionId, offerSubscription.RenewalGroup, offerSubscription.CommittedQuantity, offerCode, tenantId, objectId, billingTarget, sessionId: new Guid?(guid), skipSubscriptionValidation: skipSubscriptionValidation);
      else
        this.TfsRequestContext.GetService<PlatformOfferSubscriptionService>().CreateOfferSubscription(this.TfsRequestContext, offerSubscription.OfferMeter.GalleryId, offerSubscription.AzureSubscriptionId, offerSubscription.RenewalGroup, offerSubscription.CommittedQuantity, offerCode, tenantId, objectId, billingTarget, sessionId: new Guid?(guid));
    }

    [HttpPatch]
    [TraceFilter(5106961, 5106962)]
    [TraceExceptions(5108781)]
    public virtual void CancelOfferSubscription(
      OfferSubscription offerSubscription,
      string cancelReason)
    {
      this.CancelOfferSubscription(offerSubscription, cancelReason, new Guid?());
    }

    [HttpPatch]
    [TraceFilter(5106961, 5106962)]
    [TraceExceptions(5108781)]
    public virtual void CancelOfferSubscription(
      OfferSubscription offerSubscription,
      string cancelReason,
      Guid? billingTarget,
      bool immediate = false)
    {
      CommerceUtil.SetRequestSource(this.TfsRequestContext, "Marketplace");
      this.TfsRequestContext.GetService<PlatformOfferSubscriptionService>().CancelOfferSubscription(this.TfsRequestContext, offerSubscription.OfferMeter.GalleryId ?? offerSubscription.OfferMeter.Name, offerSubscription.AzureSubscriptionId, offerSubscription.RenewalGroup, cancelReason, billingTarget, immediate, new Guid?());
    }

    [HttpPost]
    [TraceFilter(5106963, 5106964)]
    [TraceExceptions(5108782)]
    public virtual void EnableTrialOrPreviewOfferSubscription(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup)
    {
      CommerceUtil.SetRequestSource(this.TfsRequestContext, "Marketplace");
      if (this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        this.TfsRequestContext.GetService<PlatformOfferSubscriptionService>().EnableTrialOrPreview(this.TfsRequestContext, offerMeterName, renewalGroup);
      else
        CollectionHelper.WithCollectionContext(this.TfsRequestContext, CollectionHelper.GetDefaultCollectionId(this.TfsRequestContext), (Action<IVssRequestContext>) (collectionContext => collectionContext.GetService<PlatformOfferSubscriptionService>().EnableTrialOrPreview(collectionContext, offerMeterName, renewalGroup)), method: nameof (EnableTrialOrPreviewOfferSubscription));
    }

    [HttpPatch]
    [TraceFilter(5115140, 5115142)]
    [TraceExceptions(5115141)]
    public virtual void EnableTrialOfferSubscriptionExtension(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      DateTime endDate)
    {
      CommerceUtil.SetRequestSource(this.TfsRequestContext, "Marketplace");
      this.TfsRequestContext.GetService<PlatformOfferSubscriptionService>().ExtendTrialDate(this.TfsRequestContext, offerMeterName, renewalGroup, endDate);
    }

    [HttpGet]
    [TraceFilter(5106911, 5106920)]
    public virtual IEnumerable<IOfferSubscription> GetOfferSubscriptions(bool nextBillingPeriod = false)
    {
      try
      {
        return this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(this.TfsRequestContext, nextBillingPeriod);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5106919, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    [HttpGet]
    [TraceFilter(5106921, 5106930)]
    public virtual IEnumerable<IOfferSubscription> GetAllOfferSubscriptionsForUser(
      bool validateAzuresubscription,
      bool nextBillingPeriod)
    {
      try
      {
        this.TfsRequestContext.Trace(5106922, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, "Getting resource status for host {0}", (object) this.TfsRequestContext.ServiceHost.InstanceId);
        return this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptionsForAllAzureSubscription(this.TfsRequestContext, validateAzuresubscription, nextBillingPeriod);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5106929, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    [HttpGet]
    [TraceFilter(5106931, 5106940)]
    public virtual IEnumerable<IOfferSubscription> GetOfferSubscriptionsForGalleryItem(
      string galleryItemId,
      Guid azureSubscriptionId,
      bool nextBillingPeriod = false)
    {
      try
      {
        this.TfsRequestContext.Trace(5106932, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, "Getting resource status for host {0}", (object) this.TfsRequestContext.ServiceHost.InstanceId);
        return this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptionsForGalleryItem(this.TfsRequestContext, azureSubscriptionId, galleryItemId, nextBillingPeriod);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5106939, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    protected virtual IOfferSubscription GetOfferSubscriptionForRenewalGroupImpl(
      string galleryId,
      ResourceRenewalGroup renewalGroup,
      bool nextBillingPeriod = false)
    {
      if (CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(this.TfsRequestContext) && CommerceDeploymentHelper.IsMissingLocalDeployment(this.TfsRequestContext) && !CommerceDeploymentHelper.IsOfferMeterHandledOffline(galleryId))
        return (IOfferSubscription) null;
      try
      {
        this.TfsRequestContext.Trace(5106942, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, string.Format("Getting resource status for host {0} with gallery id {1} {2}", (object) this.TfsRequestContext.ServiceHost.InstanceId, (object) galleryId, (object) renewalGroup));
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        IOfferMeter offerMeter = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, galleryId);
        if (offerMeter == null)
          throw new ArgumentException(string.Format("No OfferMeter found for Gallery-Id {0}", (object) galleryId), nameof (galleryId));
        if (this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          return this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscription(this.TfsRequestContext, offerMeter.Name, renewalGroup, nextBillingPeriod);
        IOfferSubscription offerSubscription = (IOfferSubscription) null;
        CollectionHelper.WithCollectionContext(this.TfsRequestContext, CollectionHelper.GetDefaultCollectionId(this.TfsRequestContext), (Action<IVssRequestContext>) (collectionContext => offerSubscription = collectionContext.GetService<IOfferSubscriptionService>().GetOfferSubscription(collectionContext, offerMeter.Name, renewalGroup, nextBillingPeriod)), method: nameof (GetOfferSubscriptionForRenewalGroupImpl));
        return offerSubscription;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5106949, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    [HttpGet]
    [TraceFilter(5106941, 5106950)]
    public virtual IOfferSubscription GetOfferSubscriptionForRenewalGroup(
      string galleryId,
      ResourceRenewalGroup renewalGroup,
      bool nextBillingPeriod = false)
    {
      return this.GetOfferSubscriptionForRenewalGroupImpl(galleryId, renewalGroup, nextBillingPeriod);
    }

    [HttpGet]
    public virtual IOfferSubscription GetOfferSubscription(string galleryId, bool nextBillingPeriod = false) => this.GetOfferSubscriptionForRenewalGroupImpl(galleryId, ResourceRenewalGroup.Monthly, nextBillingPeriod);

    [HttpPatch]
    [TraceFilter(5106951, 5106960)]
    public virtual void UpdateOfferSubscription(OfferSubscription offerSubscription)
    {
      try
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        IResourceQuantityUpdaterService service = this.TfsRequestContext.GetService<IResourceQuantityUpdaterService>();
        OfferMeter offerMeter = (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, offerSubscription.OfferMeter.Name);
        if (offerMeter == (OfferMeter) null)
          throw new ArgumentException("Offer meter name is invalid.");
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        int? maximumQuantity = new int?(offerSubscription.MaximumQuantity);
        int? includedQuantity = new int?(offerSubscription.IncludedQuantity);
        bool? isPaidBillingEnabled = new bool?(offerSubscription.IsPaidBillingEnabled);
        OfferMeter meterConfig = offerMeter;
        int renewalGroup = (int) offerSubscription.RenewalGroup;
        service.UpdateOfferSubscription(tfsRequestContext, maximumQuantity, includedQuantity, isPaidBillingEnabled, meterConfig, (ResourceRenewalGroup) renewalGroup);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5106959, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    [HttpPatch]
    [TraceFilter(5117001, 5117003)]
    public virtual void SetAccountQuantity(
      string offerMeterName,
      ResourceRenewalGroup meterRenewalGroup,
      int? newIncludedQuantity,
      int? newMaximumQuantity)
    {
      try
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        IResourceQuantityUpdaterService service = this.TfsRequestContext.GetService<IResourceQuantityUpdaterService>();
        OfferMeter offerMeter = (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, offerMeterName);
        if (offerMeter == (OfferMeter) null)
          throw new ArgumentException("Offer meter name is invalid.");
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        int? maximumQuantity = newMaximumQuantity;
        int? includedQuantity = newIncludedQuantity;
        bool? isPaidBillingEnabled = new bool?();
        OfferMeter meterConfig = offerMeter;
        int renewalGroup = (int) meterRenewalGroup;
        service.UpdateOfferSubscription(tfsRequestContext, maximumQuantity, includedQuantity, isPaidBillingEnabled, meterConfig, (ResourceRenewalGroup) renewalGroup);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5117002, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    [HttpPatch]
    [TraceFilter(5109011, 5109013)]
    [TraceExceptions(5109012)]
    public virtual void DecreaseResourceQuantity(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      bool shouldBeImmediate,
      Guid? azureSubscriptionId)
    {
      this.TfsRequestContext.GetService<PlatformOfferSubscriptionService>().DecreaseResourceQuantity(this.TfsRequestContext, azureSubscriptionId, offerMeterName, renewalGroup, quantity, shouldBeImmediate);
    }
  }
}
