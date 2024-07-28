// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.SingleQueryUsageDataAggregator
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.Common
{
  internal class SingleQueryUsageDataAggregator : UsageDataAggregator
  {
    private const string Layer = "SingleQueryUsageDataAggregator";
    private const string Area = "Commerce";

    public SingleQueryUsageDataAggregator(TimeSpan timeGrain) => this.timeGrain = timeGrain;

    public override ResourceMetricResponses AggregateUsage(
      IVssRequestContext accountContext,
      IUsageEventsStore usageEventStore,
      DateTime startTime,
      DateTime endTime,
      IEnumerable<ResourceName> resourceNames)
    {
      ResourceMetricResponses resourceMetricResponses = new ResourceMetricResponses();
      long toExclusive = (long) (int) Math.Ceiling((endTime - startTime).TotalMinutes / this.timeGrain.TotalMinutes);
      Dictionary<ResourceName, ResourceMetricResponse> dictionary1 = new Dictionary<ResourceName, ResourceMetricResponse>();
      Dictionary<ResourceName, ResourceMetricSample[]> s = new Dictionary<ResourceName, ResourceMetricSample[]>();
      if (!(resourceNames is ResourceName[] resourceNameArray))
        resourceNameArray = resourceNames.ToArray<ResourceName>();
      ResourceName[] enumerable = resourceNameArray;
      IVssRequestContext vssRequestContext = accountContext.To(TeamFoundationHostType.Deployment);
      Dictionary<ResourceName, MeteredResource> dictionary2 = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext).Select<IOfferMeter, MeteredResource>((Func<IOfferMeter, MeteredResource>) (m => ((OfferMeter) m).ToMeteredResource())).Where<MeteredResource>((Func<MeteredResource, bool>) (m => m != null)).ToDictionary<MeteredResource, ResourceName>((Func<MeteredResource, ResourceName>) (m => m.ResourceName));
      foreach (ResourceName key in enumerable)
      {
        ResourceMetricResponse resourceMetricResponse = new ResourceMetricResponse()
        {
          Data = new ResourceMetricSet()
          {
            StartTime = startTime,
            TimeGrain = this.timeGrain,
            EndTime = endTime,
            Unit = dictionary2[key].Unit,
            Name = key.ToString(),
            DisplayName = key.ToString()
          }
        };
        dictionary1.Add(key, resourceMetricResponse);
        s.Add(key, new ResourceMetricSample[toExclusive]);
      }
      List<UsageEvent> usageEvents = new List<UsageEvent>();
      TableContinuationToken continuationTokenOut = (TableContinuationToken) null;
      do
      {
        int num = 0;
        AggregateException aggregateException = (AggregateException) null;
        while (num++ <= this.RetryThreshold)
        {
          try
          {
            usageEvents.AddRange(usageEventStore.GetProcessedEvents(accountContext, startTime, endTime, out continuationTokenOut));
            break;
          }
          catch (AggregateException ex)
          {
            aggregateException = ex;
          }
        }
        if (aggregateException != null)
          accountContext.TraceException(5106079, "Commerce", nameof (SingleQueryUsageDataAggregator), (Exception) aggregateException);
      }
      while (continuationTokenOut != null);
      TimeSpan grain = this.timeGrain;
      Parallel.For(0L, toExclusive, (Action<long>) (sliceIndex =>
      {
        DateTime startDate = startTime.AddMinutes((double) sliceIndex * grain.TotalMinutes);
        DateTime endDate = startTime.AddMinutes((double) (sliceIndex + 1L) * grain.TotalMinutes);
        foreach (ResourceName key in enumerable)
        {
          ResourceName name = key;
          s[key][sliceIndex] = this.GetMetricSample((IList<UsageEvent>) usageEvents, name, startDate, endDate);
        }
      }));
      foreach (ResourceName key in enumerable)
      {
        dictionary1[key].Data.Payload.AddRange((IEnumerable<ResourceMetricSample>) s[key]);
        resourceMetricResponses.Add(dictionary1[key]);
      }
      return resourceMetricResponses;
    }

    internal ResourceMetricSample GetMetricSample(
      IList<UsageEvent> usageEvents,
      ResourceName name,
      DateTime startDate,
      DateTime endDate)
    {
      return new ResourceMetricSample()
      {
        Total = new double?((double) usageEvents.Where<UsageEvent>((Func<UsageEvent, bool>) (x => x.MeterName == name.ToString())).Where<UsageEvent>((Func<UsageEvent, bool>) (x => x.EventTimestamp >= startDate.ToUniversalTime())).Where<UsageEvent>((Func<UsageEvent, bool>) (x => x.EventTimestamp < endDate.ToUniversalTime())).Sum<UsageEvent>((Func<UsageEvent, int>) (e => e.Quantity))),
        TimeCreated = startDate
      };
    }
  }
}
