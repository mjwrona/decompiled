// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusLogger
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusLogger
  {
    private const string s_Area = "ServiceBus";
    private string s_Layer = "ServiceBusLoggerService";
    private const string KpiMetricPublishMessageSuccessCount = "PublishMessageSuccessCount";
    private const string KpiMetricPublishMessageFailureCount = "PublishMessageFailureCount";
    private const string KpiMetricSubscribeMessageSuccessCount = "SubscribeMessageSuccessCount";
    private const string KpiMetricSubscribeMessageFailureCount = "SubscribeMessageFailureCount";
    private ConcurrentDictionary<string, RollingResultAggregator<bool>> m_resultAggregators;
    private static readonly Random s_random = new Random();

    internal ServiceBusLogger() => this.m_resultAggregators = new ConcurrentDictionary<string, RollingResultAggregator<bool>>();

    internal void LogMessageProcessing(
      IVssRequestContext requestContext,
      string serviceBusNamespace,
      string messageBusName,
      bool success,
      string subscriberName = null)
    {
      try
      {
        requestContext.Trace(1005165, TraceLevel.Verbose, "ServiceBus", this.s_Layer, "Logging 'Publish' result. MessageBus:{0}, Success:{1}, Subscriber: {2}, Namespace: {3}", (object) messageBusName, (object) success, (object) (subscriberName ?? string.Empty), (object) serviceBusNamespace);
        this.GetResultAggregator(serviceBusNamespace, messageBusName, subscriberName).LogResult(requestContext, success);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1005168, "ServiceBus", this.s_Layer, ex);
      }
    }

    private RollingResultAggregator<bool> GetResultAggregator(
      string serviceBusNamespace,
      string messageBusName,
      string subscriberName)
    {
      return this.m_resultAggregators.GetOrAdd(serviceBusNamespace + "/" + messageBusName + "/" + subscriberName, (Func<string, RollingResultAggregator<bool>>) (messageBusName2 =>
      {
        RollingResultAggregator<bool> instance = RollingResultAggregator<bool>.CreateInstance(5, (object) messageBusName2);
        if (subscriberName == null)
          instance.OnInterval += new RollingResultAggregator<bool>.IntervalProcessor(this.PublishKpisPublisher);
        else
          instance.OnInterval += new RollingResultAggregator<bool>.IntervalProcessor(this.PublishKpisSubscriber);
        return instance;
      }));
    }

    private void PublishKpisPublisher(
      IVssRequestContext requestContext,
      IDictionary<bool, Tuple<int, int>> resultCounts,
      object sourceData)
    {
      this.PublishKpis(requestContext, resultCounts, sourceData, "PublishMessageSuccessCount", "PublishMessageFailureCount");
    }

    private void PublishKpisSubscriber(
      IVssRequestContext requestContext,
      IDictionary<bool, Tuple<int, int>> resultCounts,
      object sourceData)
    {
      this.PublishKpis(requestContext, resultCounts, sourceData, "SubscribeMessageSuccessCount", "SubscribeMessageFailureCount");
    }

    private void PublishKpis(
      IVssRequestContext requestContext,
      IDictionary<bool, Tuple<int, int>> resultCounts,
      object sourceData,
      string successMetricName,
      string failureMetricName)
    {
      requestContext.TraceEnter(1005190, "ServiceBus", this.s_Layer, nameof (PublishKpis));
      try
      {
        string scope = sourceData as string;
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        Tuple<int, int> tuple;
        if (resultCounts.TryGetValue(false, out tuple))
        {
          num1 = tuple.Item1;
          num2 = tuple.Item2;
        }
        if (resultCounts.TryGetValue(true, out tuple))
        {
          num3 = tuple.Item1;
          num4 = tuple.Item2;
        }
        requestContext.Trace(1005195, TraceLevel.Verbose, "ServiceBus", this.s_Layer, "Sending ServiceBusKpi. MessageBus: {0}, #failures={1}, #totalFailures={2}, #successes={3}, #totalSuccesses={4}", (object) scope, (object) num1, (object) num2, (object) num3, (object) num4);
        List<KpiMetric> metrics = new List<KpiMetric>()
        {
          new KpiMetric()
          {
            Name = failureMetricName,
            Value = (double) num1
          },
          new KpiMetric()
          {
            Name = successMetricName,
            Value = (double) num3
          }
        };
        requestContext.GetService<KpiService>().Publish(requestContext, "Microsoft.VisualStudio.ServiceBusMetrics", scope, metrics);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1005199, "ServiceBus", this.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1005199, "ServiceBus", this.s_Layer, nameof (PublishKpis));
      }
    }
  }
}
