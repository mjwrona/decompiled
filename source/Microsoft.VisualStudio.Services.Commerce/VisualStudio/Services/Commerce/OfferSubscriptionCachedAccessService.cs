// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscriptionCachedAccessService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class OfferSubscriptionCachedAccessService : 
    CommerceCacheBase<OfferSubscriptionCacheContainer>,
    IOfferSubscriptionCachedAccessService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.SetupCaches(new Guid("ea9fbad5-4390-4946-8c47-98d326ecec82"), "VisualStudio.Services.Commerce.OfferSubscriptionCache.Distributed", (ICommerceMemoryCache<OfferSubscriptionCacheContainer>) requestContext.GetService<IOfferSubscriptionMemoryCacheService>(), "VisualStudio.Services.Commerce.OfferSubscriptionCache.Memory", new TimeSpan?());
      this.RegisterSqlNotification(requestContext, SqlNotificationEventClasses.OfferSubscriptionChanged, new SqlNotificationHandler(this.OnOfferSubscriptionChanged));
    }

    public void ServiceEnd(IVssRequestContext requestContext) => this.UnregisterSqlNotification(requestContext, new SqlNotificationHandler(this.OnOfferSubscriptionChanged));

    public virtual IEnumerable<OfferSubscriptionInternal> GetOfferSubscriptions(
      IVssRequestContext requestContext,
      int? meterId)
    {
      return this.GetOfferSubscriptions(requestContext, meterId, true);
    }

    private void OnOfferSubscriptionChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      if (string.IsNullOrEmpty(args?.Data))
      {
        requestContext.TraceAlways(5107004, TraceLevel.Info, this.Area, this.Layer, "No notification event arguments passed");
      }
      else
      {
        Guid empty = Guid.Empty;
        try
        {
          Guid guid = TeamFoundationSerializationUtility.Deserialize<Guid>(args.Data);
          if (!Guid.Empty.Equals(guid))
          {
            string meterUsageCacheKey = OfferSubscriptionCachedAccessService.GetMeterUsageCacheKey(guid);
            this.InvalidateMemoryCache(requestContext, meterUsageCacheKey);
            requestContext.Trace(5107001, TraceLevel.Info, this.Area, this.Layer, string.Format("Invalidate cache for the {0}", (object) guid));
          }
          else
            requestContext.Trace(5107002, TraceLevel.Info, this.Area, this.Layer, string.Format("Invalid cache key {0}", (object) guid));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5107003, this.Area, this.Layer, ex);
        }
      }
    }

    public virtual IEnumerable<OfferSubscriptionInternal> GetOfferSubscriptions(
      IVssRequestContext requestContext,
      int? meterId,
      bool useCache)
    {
      OfferSubscriptionCacheContainer storedValue = (OfferSubscriptionCacheContainer) null;
      string meterUsageCacheKey = OfferSubscriptionCachedAccessService.GetMeterUsageCacheKey(requestContext);
      List<OfferSubscriptionInternal> resources;
      if (useCache && this.TryGetCachedItem(requestContext, meterUsageCacheKey, out storedValue) && storedValue.TryGetOfferSubscription(meterId, out resources))
        return (IEnumerable<OfferSubscriptionInternal>) resources;
      using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
        resources = component.GetMeteredResources(meterId, new byte?()).ToList<OfferSubscriptionInternal>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Dictionary<int, IOfferMeter> meterConfigs = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext).ToDictionary<IOfferMeter, int>((Func<IOfferMeter, int>) (m => m.MeterId));
      resources.RemoveAll((Predicate<OfferSubscriptionInternal>) (m => !meterConfigs.ContainsKey(m.MeterId)));
      resources.ForEach((Action<OfferSubscriptionInternal>) (u => u.Meter = meterConfigs[u.MeterId].ToReadOnlyMeter()));
      OfferSubscriptionCacheContainer valueToCache = storedValue ?? new OfferSubscriptionCacheContainer();
      valueToCache.Update((IEnumerable<OfferSubscriptionInternal>) resources, meterId);
      this.SetCacheItem(requestContext, meterUsageCacheKey, valueToCache);
      return (IEnumerable<OfferSubscriptionInternal>) resources;
    }

    public void UpdateAccountQuantities(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      int? includedQuantity,
      int? maximumQuantity,
      int defaultIncludedQuantity,
      int defaultMaxQuantity,
      int absoluteMaxQuantity,
      ResourceBillingMode billingMode,
      Guid userIdentityId)
    {
      this.UpdateAccountQuantities(requestContext, meterId, renewalGroup, includedQuantity, maximumQuantity, defaultIncludedQuantity, defaultMaxQuantity, absoluteMaxQuantity, billingMode, userIdentityId, false);
    }

    public void UpdateAccountQuantities(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      int? includedQuantity,
      int? maximumQuantity,
      int defaultIncludedQuantity,
      int defaultMaxQuantity,
      int absoluteMaxQuantity,
      ResourceBillingMode billingMode,
      Guid userIdentityId,
      bool resetUsage)
    {
      using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
        component.UpdateAccountQuantities(meterId, (byte) renewalGroup, includedQuantity, maximumQuantity, defaultIncludedQuantity, defaultMaxQuantity, absoluteMaxQuantity, billingMode, userIdentityId, resetUsage);
      this.InvalidateSingleResource(requestContext, meterId);
    }

    public int UpdateCommittedAndCurrentQuantities(
      IVssRequestContext requestContext,
      int meterId,
      byte resourceSeq,
      int committedQuantity,
      int currentQuantity,
      Guid userIdentityId)
    {
      int num = 0;
      using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
        num = component.UpdateCommittedAndCurrentQuantities(meterId, resourceSeq, committedQuantity, currentQuantity, userIdentityId);
      this.InvalidateSingleResource(requestContext, meterId);
      return num;
    }

    public void UpdatePaidBillingMode(
      IVssRequestContext requestContext,
      int meterId,
      bool paidBillingEnabled,
      Guid userIdentityId)
    {
      using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
        component.UpdatePaidBillingMode(meterId, paidBillingEnabled, userIdentityId);
      this.InvalidateSingleResource(requestContext, meterId);
    }

    public AggregateUsageEventResult AggregateUsageEvents(
      IVssRequestContext requestContext,
      OfferMeter meter,
      ResourceRenewalGroup renewalGroup,
      UsageEvent usageEvent,
      Guid lastUpdatedBy,
      DateTime executionDate)
    {
      AggregateUsageEventResult usageEventResult = (AggregateUsageEventResult) null;
      using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
        usageEventResult = component.AggregateUsageEvents(meter.MeterId, (byte) renewalGroup, usageEvent, lastUpdatedBy, meter.IncludedQuantity, meter.BillingMode, meter.RenewalFrequency == MeterRenewalFrequecy.Monthly, meter.BillingEntity, new DateTime?(executionDate));
      if (usageEventResult?.UpdatedOfferSubscription != null)
        this.ReadMeterAndUpdateCache(requestContext, usageEventResult.UpdatedOfferSubscription);
      else
        this.InvalidateSingleResource(requestContext, meter.MeterId);
      return usageEventResult;
    }

    public MeterResetEvents ResetResourceUsage(
      IVssRequestContext collectionContext,
      bool monthlyReset,
      Guid subscriptionId,
      IEnumerable<KeyValuePair<int, int>> includedQuantities,
      IEnumerable<KeyValuePair<int, string>> billingModes,
      bool isResetOnlyCurrentQuantities,
      DateTime executionDate)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      MeterResetEvents meterResetEvents = (MeterResetEvents) null;
      using (CommerceMeteringComponent component = collectionContext.CreateComponent<CommerceMeteringComponent>())
        meterResetEvents = component.ResetResourceUsage(monthlyReset, subscriptionId, includedQuantities, billingModes, isResetOnlyCurrentQuantities, new DateTime?(executionDate));
      if (meterResetEvents?.RenewedOfferSubscriptions != null)
      {
        IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
        IEnumerable<IOfferMeter> offerMeters = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext);
        meterResetEvents.BillableEvents = this.RetrieveBillableEvents(collectionContext, meterResetEvents.RenewedOfferSubscriptions, subscriptionId, offerMeters);
      }
      this.Invalidate(collectionContext);
      return meterResetEvents;
    }

    internal IEnumerable<BillableEvent> RetrieveBillableEvents(
      IVssRequestContext collectionContext,
      IEnumerable<OfferSubscriptionInternal> offerSubscriptions,
      Guid subscriptionId,
      IEnumerable<IOfferMeter> offerMeters)
    {
      collectionContext.To(TeamFoundationHostType.Deployment).GetService<IOfferMeterService>();
      Dictionary<int, IOfferMeter> dictionary = offerMeters.ToDictionary<IOfferMeter, int>((Func<IOfferMeter, int>) (m => m.MeterId));
      List<BillableEvent> billableEventList = new List<BillableEvent>();
      foreach (OfferSubscriptionInternal offerSubscription in offerSubscriptions)
      {
        if (dictionary.Keys.Contains<int>(offerSubscription.MeterId))
        {
          offerSubscription.Meter = dictionary[offerSubscription.MeterId].ToReadOnlyMeter();
          billableEventList.Add(offerSubscription.ToBillableEvent(collectionContext, subscriptionId));
        }
      }
      return (IEnumerable<BillableEvent>) billableEventList;
    }

    public void CreateTrialOrPreview(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      Guid lastUpdatedBy,
      int includedQuantity = 0)
    {
      OfferSubscriptionInternal offerSubscription = (OfferSubscriptionInternal) null;
      using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
        offerSubscription = component.AddTrialOfferSubscription(meterId, (byte) renewalGroup, lastUpdatedBy, includedQuantity);
      if (offerSubscription == null)
        return;
      this.ReadMeterAndUpdateCache(requestContext, offerSubscription);
    }

    public void ExtendTrialExpiryDate(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      Guid lastUpdatedBy,
      int trialDays)
    {
      OfferSubscriptionInternal offerSubscription = (OfferSubscriptionInternal) null;
      using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
        offerSubscription = component.ExtendTrialOfferSubscription(meterId, (byte) renewalGroup, lastUpdatedBy, trialDays);
      if (offerSubscription == null)
        return;
      this.ReadMeterAndUpdateCache(requestContext, offerSubscription);
    }

    public void RemoveTrialForPaidOfferSubscription(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      int includedQuantity,
      Guid lastUpdatedBy)
    {
      OfferSubscriptionInternal offerSubscription = (OfferSubscriptionInternal) null;
      using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
        offerSubscription = component.RemoveTrialForPaidOfferSubscription(meterId, (byte) renewalGroup, includedQuantity, lastUpdatedBy);
      if (offerSubscription == null)
        return;
      this.ReadMeterAndUpdateCache(requestContext, offerSubscription);
    }

    public void ResetCloudLoadTestUsage(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      Guid lastUpdatedBy)
    {
      OfferSubscriptionInternal offerSubscription = (OfferSubscriptionInternal) null;
      using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
        offerSubscription = component.ResetCloudLoadTestUsage(meterId, (byte) renewalGroup, lastUpdatedBy);
      if (offerSubscription == null)
        return;
      this.ReadMeterAndUpdateCache(requestContext, offerSubscription);
    }

    public void Invalidate(IVssRequestContext requestContext)
    {
      this.InvalidateCache(requestContext, OfferSubscriptionCachedAccessService.GetMeterUsageCacheKey(requestContext));
      this.SendMemoryCacheInvalidationSqlNotification(requestContext, TeamFoundationSerializationUtility.SerializeToString<Guid>(requestContext.ServiceHost.InstanceId));
    }

    private void ReadMeterAndUpdateCache(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal offerSubscription)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Dictionary<int, IOfferMeter> dictionary = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext).ToDictionary<IOfferMeter, int>((Func<IOfferMeter, int>) (m => m.MeterId));
      offerSubscription.Meter = dictionary[offerSubscription.MeterId].ToReadOnlyMeter();
      string meterUsageCacheKey = OfferSubscriptionCachedAccessService.GetMeterUsageCacheKey(requestContext);
      OfferSubscriptionCacheContainer storedValue;
      if (!this.TryGetCachedItem(requestContext, meterUsageCacheKey, out storedValue))
        storedValue = new OfferSubscriptionCacheContainer();
      storedValue.SelectiveUpdate(offerSubscription);
      this.SetCacheItem(requestContext, meterUsageCacheKey, storedValue);
      this.SendMemoryCacheInvalidationSqlNotification(requestContext, TeamFoundationSerializationUtility.SerializeToString<Guid>(requestContext.ServiceHost.InstanceId));
    }

    private static string GetMeterUsageCacheKey(IVssRequestContext requestContext) => OfferSubscriptionCachedAccessService.GetMeterUsageCacheKey(requestContext.ServiceHost.InstanceId);

    private static string GetMeterUsageCacheKey(Guid accountId) => string.Format("MeterUsage-v2-{0}", (object) accountId);

    private void InvalidateSingleResource(IVssRequestContext requestContext, int meterId)
    {
      string meterUsageCacheKey = OfferSubscriptionCachedAccessService.GetMeterUsageCacheKey(requestContext);
      OfferSubscriptionCacheContainer storedValue;
      if (this.TryGetCachedItem(requestContext, meterUsageCacheKey, out storedValue))
      {
        storedValue.SelectiveInvalidate(meterId);
        this.SetCacheItem(requestContext, meterUsageCacheKey, storedValue);
      }
      this.SendMemoryCacheInvalidationSqlNotification(requestContext, TeamFoundationSerializationUtility.SerializeToString<Guid>(requestContext.ServiceHost.InstanceId));
      this.InvalidateMemoryCache(requestContext, meterUsageCacheKey);
    }

    protected override string Layer => nameof (OfferSubscriptionCachedAccessService);
  }
}
