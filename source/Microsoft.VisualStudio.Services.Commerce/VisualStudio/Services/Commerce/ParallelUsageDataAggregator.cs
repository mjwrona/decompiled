// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ParallelUsageDataAggregator
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class ParallelUsageDataAggregator : UsageDataAggregator
  {
    private const string Layer = "ParallelUsageDataAggregator";
    private const string Area = "Commerce";

    public ParallelUsageDataAggregator(TimeSpan timeGrain) => this.timeGrain = timeGrain;

    public override ResourceMetricResponses AggregateUsage(
      IVssRequestContext accountContext,
      IUsageEventsStore usageEventStore,
      DateTime startTime,
      DateTime endTime,
      IEnumerable<ResourceName> resourceNames)
    {
      ResourceMetricResponses resourceMetricResponses = new ResourceMetricResponses();
      long length = (long) (int) Math.Ceiling((endTime - startTime).TotalMinutes / this.timeGrain.TotalMinutes);
      Dictionary<ResourceName, ResourceMetricResponse> dictionary1 = new Dictionary<ResourceName, ResourceMetricResponse>();
      Dictionary<ResourceName, ResourceMetricSample[]> dictionary2 = new Dictionary<ResourceName, ResourceMetricSample[]>();
      if (!(resourceNames is ResourceName[] resourceNameArray1))
        resourceNameArray1 = resourceNames.ToArray<ResourceName>();
      ResourceName[] resourceNameArray2 = resourceNameArray1;
      IVssRequestContext vssRequestContext = accountContext.To(TeamFoundationHostType.Deployment);
      Dictionary<ResourceName, MeteredResource> dictionary3 = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext).Select<IOfferMeter, MeteredResource>((Func<IOfferMeter, MeteredResource>) (m => ((OfferMeter) m).ToMeteredResource())).Where<MeteredResource>((Func<MeteredResource, bool>) (m => m != null)).ToDictionary<MeteredResource, ResourceName>((Func<MeteredResource, ResourceName>) (m => m.ResourceName));
      foreach (ResourceName key in resourceNameArray2)
      {
        ResourceMetricResponse resourceMetricResponse = new ResourceMetricResponse()
        {
          Data = new ResourceMetricSet()
          {
            StartTime = startTime,
            TimeGrain = this.timeGrain,
            EndTime = endTime,
            Unit = dictionary3[key].Unit,
            Name = key.ToString(),
            DisplayName = key.ToString()
          }
        };
        dictionary1.Add(key, resourceMetricResponse);
        dictionary2.Add(key, new ResourceMetricSample[length]);
      }
      TimeSpan timeGrain = this.timeGrain;
      for (int index = 0; (long) index < length; ++index)
      {
        DateTime startTime1 = startTime.AddMinutes((double) index * timeGrain.TotalMinutes);
        DateTime endTime1 = startTime.AddMinutes((double) (index + 1) * timeGrain.TotalMinutes);
        TableContinuationToken continuationTokenOut = (TableContinuationToken) null;
        List<UsageEvent> source = new List<UsageEvent>();
        do
        {
          int num = 0;
          AggregateException aggregateException = (AggregateException) null;
          while (num++ <= this.RetryThreshold)
          {
            try
            {
              source.AddRange(usageEventStore.GetProcessedEvents(accountContext, startTime1, endTime1, out continuationTokenOut));
              break;
            }
            catch (AggregateException ex)
            {
              aggregateException = ex;
            }
          }
          if (aggregateException != null)
            accountContext.TraceException(5106079, "Commerce", nameof (ParallelUsageDataAggregator), (Exception) aggregateException);
        }
        while (continuationTokenOut != null);
        foreach (ResourceName key in resourceNameArray2)
        {
          ResourceName name = key;
          dictionary2[key][index] = new ResourceMetricSample()
          {
            Total = new double?((double) source.Where<UsageEvent>((Func<UsageEvent, bool>) (x => x.MeterName == name.ToString())).Sum<UsageEvent>((Func<UsageEvent, int>) (e => e.Quantity))),
            TimeCreated = startTime1
          };
        }
      }
      foreach (ResourceName key in resourceNameArray2)
      {
        dictionary1[key].Data.Payload.AddRange((IEnumerable<ResourceMetricSample>) dictionary2[key]);
        resourceMetricResponses.Add(dictionary1[key]);
      }
      return resourceMetricResponses;
    }
  }
}
