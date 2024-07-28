// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterPriceCachedAccessService
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
  internal class OfferMeterPriceCachedAccessService : 
    CommerceCacheBase<OfferMeterPriceCacheContainer>,
    IOfferMeterPriceCachedAccessService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.RegisterSqlNotification(requestContext, SqlNotificationEventClasses.OfferMeterPriceChanged, new SqlNotificationHandler(this.OnOfferMeterPriceChanged));
      this.SetupCaches(new Guid("8ABBB752-8107-4585-B82E-9DA8A9BC2090"), "VisualStudio.Services.Commerce.OfferMeterPriceCache.Distributed", (ICommerceMemoryCache<OfferMeterPriceCacheContainer>) requestContext.GetService<IOfferMeterPriceMemoryCacheService>(), "VisualStudio.Services.Commerce.OfferMeterPriceCache.Memory", new TimeSpan?(TimeSpan.FromDays(14.0)));
    }

    public void ServiceEnd(IVssRequestContext requestContext) => this.UnregisterSqlNotification(requestContext, new SqlNotificationHandler(this.OnOfferMeterPriceChanged));

    private void OnOfferMeterPriceChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      if (string.IsNullOrEmpty(args?.Data))
      {
        requestContext.Trace(5108404, TraceLevel.Info, this.Area, this.Layer, "No notification event arguments passed");
      }
      else
      {
        try
        {
          string offerMeter = TeamFoundationSerializationUtility.Deserialize<string>(args.Data);
          if (!string.IsNullOrEmpty(offerMeter))
          {
            requestContext.Trace(5108405, TraceLevel.Info, this.Area, this.Layer, "fetching meter info for key " + offerMeter);
            string meterPriceCacheKey = OfferMeterPriceCachedAccessService.GetOfferMeterPriceCacheKey(offerMeter);
            this.InvalidateMemoryCache(requestContext, meterPriceCacheKey);
            requestContext.Trace(5108401, TraceLevel.Info, this.Area, this.Layer, "Invalidated Meter price cache");
          }
          else
            requestContext.Trace(5108402, TraceLevel.Info, this.Area, this.Layer, "Invalid cache key " + offerMeter);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5108403, this.Area, this.Layer, ex);
        }
      }
    }

    public List<OfferMeterPrice> GetOfferMeterPrice(
      IVssRequestContext requestContext,
      string galleryId)
    {
      return this.GetOfferMeterPriceCacheContainer(requestContext, galleryId)?.GetOfferMeterPriceList();
    }

    private OfferMeterPriceCacheContainer GetOfferMeterPriceCacheContainer(
      IVssRequestContext requestContext,
      string galleryId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string meterPriceCacheKey = OfferMeterPriceCachedAccessService.GetOfferMeterPriceCacheKey(galleryId);
      OfferMeterPriceCacheContainer storedValue;
      if (this.TryGetCachedItem(vssRequestContext, meterPriceCacheKey, out storedValue) && storedValue != null && !storedValue.GetOfferMeterPriceList().IsNullOrEmpty<OfferMeterPrice>())
      {
        CommerceKpi.PlatformMeterPriceCacheHit.IncrementByOne(vssRequestContext);
        return storedValue;
      }
      CommerceKpi.PlatformMeterPriceCacheMiss.IncrementByOne(vssRequestContext);
      IList<OfferMeterPrice> offerMeterPrice;
      using (CommerceSqlComponent component = vssRequestContext.CreateComponent<CommerceSqlComponent>())
        offerMeterPrice = component.GetOfferMeterPrice(galleryId);
      OfferMeterPriceCacheContainer valueToCache = new OfferMeterPriceCacheContainer();
      valueToCache.AddOfferMeterPrice(offerMeterPrice);
      this.SetCacheItem(vssRequestContext, meterPriceCacheKey, valueToCache);
      return valueToCache;
    }

    private static string GetOfferMeterPriceCacheKey(string offerMeter) => "OfferMeterPrice-v1-" + offerMeter;

    protected override string Layer => nameof (OfferMeterPriceCachedAccessService);
  }
}
