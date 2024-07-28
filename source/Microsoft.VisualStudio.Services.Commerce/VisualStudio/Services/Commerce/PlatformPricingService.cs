// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformPricingService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class PlatformPricingService : 
    IPricingService,
    IVssFrameworkService,
    IPricingServiceInternal
  {
    private const string Area = "Commerce";
    private const string Layer = "PlatformPricingService";

    public virtual int Populate(IVssRequestContext requestContext, IOfferMeter offerMeter)
    {
      IPricingPopulator pricingPopulator = (IPricingPopulator) null;
      List<OfferMeterPrice> offerMeterPriceList = new List<OfferMeterPrice>();
      if (offerMeter.BillingEntity == BillingProvider.AzureStoreManaged)
        pricingPopulator = (IPricingPopulator) new DatamarketPublishingPricePopulator();
      if (pricingPopulator != null)
      {
        offerMeterPriceList = pricingPopulator.GetMeterPriceFromPricingFeed(requestContext, offerMeter);
        if (!offerMeterPriceList.IsNullOrEmpty<OfferMeterPrice>())
          this.UpdatePricing(requestContext, offerMeter, offerMeterPriceList);
      }
      // ISSUE: explicit non-virtual call
      return offerMeterPriceList == null ? 0 : __nonvirtual (offerMeterPriceList.Count);
    }

    public virtual void UpdatePricing(
      IVssRequestContext requestContext,
      string galleryId,
      List<OfferMeterPrice> pricing)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IOfferMeter offerMeter = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, galleryId);
      this.UpdatePricing(vssRequestContext, offerMeter, pricing);
    }

    private void UpdatePricing(
      IVssRequestContext requestContext,
      IOfferMeter offerMeter,
      List<OfferMeterPrice> pricing)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        using (CommerceSqlComponent component = vssRequestContext.CreateComponent<CommerceSqlComponent>())
          component.AddOfferMeterPrice((IEnumerable<OfferMeterPrice>) pricing, offerMeter.GalleryId, offerMeter.IsFirstParty);
        vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(vssRequestContext, SqlNotificationEventClasses.OfferMeterPriceChanged, TeamFoundationSerializationUtility.SerializeToString<string>(offerMeter.GalleryId));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108459, "Commerce", nameof (PlatformPricingService), ex);
        throw;
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.Trace(5105954, TraceLevel.Info, "Commerce", nameof (PlatformPricingService), "PlatformPricingService starting");

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.Trace(5105955, TraceLevel.Info, "Commerce", nameof (PlatformPricingService), "PlatformPricingService ending");
  }
}
