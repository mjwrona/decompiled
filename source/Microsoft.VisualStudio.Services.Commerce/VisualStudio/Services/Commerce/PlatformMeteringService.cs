// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformMeteringService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PlatformMeteringService : IMeteringService, IVssFrameworkService
  {
    internal Type usageEventStoreProviderType;
    internal IUsageEventsStore usageEventStore;
    private const string storageProviderRegistryPath = "/Service/Commerce/Metering/UsageEventStorageProvider";
    private const string Area = "Commerce";
    private const string Layer = "PlatformMeteringService";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      if (!requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Application) && !requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.usageEventStoreProviderType = Type.GetType(vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/Commerce/Metering/UsageEventStorageProvider", string.Empty), true);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      if (this.usageEventStore == null)
        return;
      this.usageEventStore.Cleanup(requestContext);
    }

    public IEnumerable<ISubscriptionResource> GetResourceStatus(
      IVssRequestContext collectionContext,
      bool nextBillingPeriod = false)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      return (IEnumerable<ISubscriptionResource>) collectionContext.GetService<PlatformOfferSubscriptionService>().GetOfferSubscriptions(collectionContext, nextBillingPeriod).Select<IOfferSubscription, ISubscriptionResource>((Func<IOfferSubscription, ISubscriptionResource>) (m => m.ToSubscriptionResource())).Where<ISubscriptionResource>((Func<ISubscriptionResource, bool>) (m => m != null)).ToList<ISubscriptionResource>();
    }

    public ISubscriptionResource GetResourceStatus(
      IVssRequestContext collectionContext,
      ResourceName resourceName,
      bool nextBillingPeriod = false)
    {
      return collectionContext.GetService<PlatformOfferSubscriptionService>().GetOfferSubscription(collectionContext, resourceName.ToString(), nextBillingPeriod).ToSubscriptionResource();
    }

    public void ReportUsage(
      IVssRequestContext collectionContext,
      Guid eventUserId,
      ResourceName resourceName,
      int quantity,
      string eventId,
      DateTime billingEventDateTime)
    {
      collectionContext.GetService<PlatformOfferSubscriptionService>().ReportUsage(collectionContext, eventUserId, resourceName.ToString(), ResourceRenewalGroup.Monthly, quantity, eventId, billingEventDateTime, false);
    }

    public void TogglePaidBilling(
      IVssRequestContext collectionContext,
      ResourceName resourceName,
      bool paidBillingState)
    {
      collectionContext.GetService<PlatformOfferSubscriptionService>().TogglePaidBilling(collectionContext, resourceName.ToString(), paidBillingState);
    }

    public void SetAccountQuantity(
      IVssRequestContext collectionContext,
      ResourceName resourceName,
      int includedQuantity,
      int maximumQuantity)
    {
      collectionContext.GetService<PlatformOfferSubscriptionService>().SetAccountQuantity(collectionContext, resourceName.ToString(), new int?(includedQuantity), new int?(maximumQuantity));
    }

    public IEnumerable<IUsageEventAggregate> GetUsage(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeSpan,
      ResourceName resource)
    {
      if (this.GetMeterConfiguration(requestContext, resource.ToString()).BillingMode != ResourceBillingMode.PayAsYouGo)
        return Enumerable.Empty<IUsageEventAggregate>();
      AggregationInterval interval = AggregationInterval.Hourly;
      if (timeSpan.TotalDays == (double) (int) timeSpan.TotalDays)
      {
        interval = AggregationInterval.Daily;
      }
      else
      {
        if (timeSpan.TotalHours != (double) (int) timeSpan.TotalHours)
          throw new ArgumentException("Argument timeSpan currently only supports multiple of an hour or a day.");
        interval = AggregationInterval.Hourly;
      }
      this.usageEventStore = this.GetUsageEventsStoreInstance(requestContext.To(TeamFoundationHostType.Deployment));
      AggregationTotal[] array = this.usageEventStore.GetAggregationTotals(requestContext, resource.ToString(), startTime, endTime, interval).OrderBy<AggregationTotal, string>((Func<AggregationTotal, string>) (x => x.RowKey)).ToArray<AggregationTotal>();
      List<IUsageEventAggregate> usage = new List<IUsageEventAggregate>();
      DateTime sliceStartTime = startTime;
      DateTime sliceEndTime = startTime + timeSpan;
      while (sliceStartTime < endTime)
      {
        IEnumerable<AggregationTotal> source = ((IEnumerable<AggregationTotal>) array).Where<AggregationTotal>((Func<AggregationTotal, bool>) (x =>
        {
          string format = interval == AggregationInterval.Hourly ? "yyyy-MM-dd-HH" : "yyyy-MM-dd";
          DateTime exact = DateTime.ParseExact(x.RowKey, format, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
          return exact >= sliceStartTime && exact < sliceEndTime;
        }));
        UsageEventAggregate usageEventAggregate = new UsageEventAggregate()
        {
          StartTime = sliceStartTime,
          EndTime = sliceEndTime,
          Resource = resource,
          Value = source.Aggregate<AggregationTotal, int>(0, (Func<int, AggregationTotal, int>) ((x, y) => x + y.Value))
        };
        usage.Add((IUsageEventAggregate) usageEventAggregate);
        sliceStartTime = sliceEndTime;
        sliceEndTime = sliceStartTime + timeSpan;
      }
      return (IEnumerable<IUsageEventAggregate>) usage;
    }

    internal OfferMeter GetMeterConfiguration(
      IVssRequestContext requestContext,
      string resourceName)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      OfferMeter offerMeter = (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, resourceName);
      return !(offerMeter == (OfferMeter) null) ? offerMeter : throw new ArgumentException("Invalid resource name", nameof (resourceName));
    }

    internal IEnumerable<OfferMeter> GetMeterConfigurations(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext).Cast<OfferMeter>();
    }

    [ExcludeFromCodeCoverage]
    internal virtual IUsageEventsStore GetUsageEventsStoreInstance(
      IVssRequestContext deploymentContext)
    {
      return Activator.CreateInstance(this.usageEventStoreProviderType, (object) deploymentContext) as IUsageEventsStore;
    }
  }
}
