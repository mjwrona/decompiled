// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterCachedAccessService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class OfferMeterCachedAccessService : 
    CommerceCacheBase<OfferMeterCacheContainer>,
    IOfferMeterCachedAccessService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.SetupCaches(new Guid("36e0d364-3440-49b7-bacc-78dcc16bf667"), "VisualStudio.Services.Commerce.OfferMeterCache.Distributed", (ICommerceMemoryCache<OfferMeterCacheContainer>) requestContext.GetService<IOfferMeterMemoryCacheService>(), "VisualStudio.Services.Commerce.OfferMeterCache.Memory", new TimeSpan?(TimeSpan.FromHours(24.0)));
      this.RegisterSqlNotification(requestContext, SqlNotificationEventClasses.OfferMeterChanged, new SqlNotificationHandler(this.OnOfferMeterChanged));
    }

    public void ServiceEnd(IVssRequestContext requestContext) => this.UnregisterSqlNotification(requestContext, new SqlNotificationHandler(this.OnOfferMeterChanged));

    private void OnOfferMeterChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      try
      {
        string offerMeterCacheKey = OfferMeterCachedAccessService.GetOfferMeterCacheKey();
        this.InvalidateMemoryCache(requestContext, offerMeterCacheKey);
        requestContext.Trace(5108478, TraceLevel.Info, this.Area, this.Layer, "Invalidate Meter cache");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108479, this.Area, this.Layer, ex);
      }
    }

    public OfferMeter GetOfferMeter(IVssRequestContext requestContext, int meterId) => this.GetOfferMeterConfiguration(requestContext).GetOfferMeter(meterId);

    public OfferMeter GetOfferMeter(IVssRequestContext requestContext, string meterName) => this.GetOfferMeterConfiguration(requestContext).GetOfferMeter(meterName);

    public void Invalidate(IVssRequestContext requestContext)
    {
      string offerMeterCacheKey = OfferMeterCachedAccessService.GetOfferMeterCacheKey();
      this.InvalidateCache(requestContext, offerMeterCacheKey);
      this.SendMemoryCacheInvalidationSqlNotification(requestContext, TeamFoundationSerializationUtility.SerializeToString<Guid>(requestContext.ServiceHost.InstanceId));
    }

    public IEnumerable<OfferMeter> GetOfferMeters(IVssRequestContext requestContext) => this.GetOfferMeterConfiguration(requestContext).GetOfferMeters();

    private OfferMeterCacheContainer GetOfferMeterConfiguration(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string offerMeterCacheKey = OfferMeterCachedAccessService.GetOfferMeterCacheKey();
      OfferMeterCacheContainer storedValue;
      if (this.TryGetCachedItem(vssRequestContext, offerMeterCacheKey, out storedValue))
      {
        CommerceKpi.PlatformMetersConfigCacheHit.IncrementByOne(vssRequestContext);
        return storedValue;
      }
      CommerceKpi.PlatformMetersConfigCacheMiss.IncrementByOne(vssRequestContext);
      IEnumerable<OfferMeter> list1;
      using (CommerceSqlComponent component = vssRequestContext.CreateComponent<CommerceSqlComponent>())
        list1 = (IEnumerable<OfferMeter>) component.GetOfferMeterConfiguration(new int?()).ToList<OfferMeter>();
      if (!vssRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.GetOfferMeterProductionAndStagedPlans"))
      {
        foreach (OfferMeter offerMeter in list1)
        {
          IEnumerable<AzureOfferPlanDefinition> fixedQuantityPlans = offerMeter.FixedQuantityPlans;
          List<AzureOfferPlanDefinition> list2 = fixedQuantityPlans != null ? fixedQuantityPlans.ToList<AzureOfferPlanDefinition>() : (List<AzureOfferPlanDefinition>) null;
          if (!list2.IsNullOrEmpty<AzureOfferPlanDefinition>() && !list2.All<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (p => !p.IsPublic)))
            offerMeter.FixedQuantityPlans = (IEnumerable<AzureOfferPlanDefinition>) list2.Where<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (p => p.IsPublic)).ToList<AzureOfferPlanDefinition>();
        }
      }
      OfferMeterCacheContainer valueToCache = new OfferMeterCacheContainer();
      valueToCache.AddOfferConfiguration(list1);
      this.SetCacheItem(vssRequestContext, offerMeterCacheKey, valueToCache);
      return valueToCache;
    }

    private static string GetOfferMeterCacheKey() => "MeterConfig-v1-All";

    public void CreateOfferMeterDefinition(
      IVssRequestContext requestContext,
      IOfferMeter meterConfig)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      using (CommerceSqlComponent component = vssRequestContext.CreateComponent<CommerceSqlComponent>())
      {
        try
        {
          component.CreateOfferMeterDefinition(meterConfig);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5107283, this.Area, this.Layer, ex);
          throw;
        }
      }
      this.InvalidateCache(vssRequestContext, OfferMeterCachedAccessService.GetOfferMeterCacheKey());
      this.SendMemoryCacheInvalidationSqlNotification(requestContext, TeamFoundationSerializationUtility.SerializeToString<Guid>(requestContext.ServiceHost.InstanceId));
      this.InvalidateOfferMeterFrameworkCache(requestContext);
    }

    public void UpdateOfferMeterDefinitionName(
      IVssRequestContext requestContext,
      IOfferMeter meterConfig)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      using (CommerceSqlComponent component = vssRequestContext.CreateComponent<CommerceSqlComponent>())
      {
        try
        {
          component.UpdateOfferMeterDefinitionName(meterConfig);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5107294, this.Area, this.Layer, ex);
          throw;
        }
      }
      this.InvalidateCache(vssRequestContext, OfferMeterCachedAccessService.GetOfferMeterCacheKey());
      this.SendMemoryCacheInvalidationSqlNotification(requestContext, TeamFoundationSerializationUtility.SerializeToString<Guid>(requestContext.ServiceHost.InstanceId));
    }

    private void InvalidateOfferMeterFrameworkCache(IVssRequestContext requestContext)
    {
      OfferMeterCacheChangeMessage message = new OfferMeterCacheChangeMessage()
      {
        Scope = OfferMeterCacheChangeScope.All
      };
      requestContext.Trace(5109095, TraceLevel.Info, this.Area, this.Layer, "Sending offer meter cache change message for all offer meter definitions.");
      requestContext.GetService<ICommerceFrameworkCacheInvalidationService>().SendFrameworkInvalidationNotification<OfferMeterCacheChangeMessage>(requestContext, "Microsoft.VisualStudio.Services.Commerce.OfferMeterChange", message);
    }
  }
}
