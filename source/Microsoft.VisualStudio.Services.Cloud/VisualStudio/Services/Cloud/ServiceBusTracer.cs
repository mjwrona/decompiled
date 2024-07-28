// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusTracer
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal static class ServiceBusTracer
  {
    private const string c_area = "ServiceBusTracer";
    private const string c_layer = "ServiceBus";
    private const string c_featureFlagName = "VisualStudio.Services.ServiceBus.DisableServiceBusActivityLogTracing";

    public static void TraceServiceBusSubscriberActivity(
      IVssRequestContext requestContext,
      string sbNamespace,
      string topicName,
      string plugin,
      string messageId,
      string contentType,
      DateTime startTime,
      Guid sourceInstanceId,
      Guid sourceInstanceType,
      bool success,
      Exception exception)
    {
      try
      {
        if (!ServiceBusTracer.IsTracing(requestContext))
          return;
        ILogRequestInternal logRequestInternal = requestContext.RequestLogger.RequestLoggerInternal();
        WellKnownPerformanceTimings performanceTimings = PerformanceTimer.GetWellKnownParsedPerformanceTimings(requestContext);
        requestContext.TracingService().TraceServiceBusSubscriberActivity(requestContext, sbNamespace ?? string.Empty, topicName ?? string.Empty, plugin ?? string.Empty, messageId ?? string.Empty, contentType ?? string.Empty, sourceInstanceId, sourceInstanceType, success, exception, startTime, logRequestInternal.LogicalReads, logRequestInternal.PhysicalReads, logRequestInternal.CpuTime, requestContext.CPUCycles, logRequestInternal.ElapsedTime, ref performanceTimings);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(106767546, nameof (ServiceBusTracer), "ServiceBus", ex);
      }
    }

    public static void TraceServiceBusPublishLog(
      IVssRequestContext requestContext,
      string sbNamespace,
      string topicName,
      List<BrokeredMessage> messages,
      bool success,
      DateTime startTime,
      long publishTimeMs)
    {
      try
      {
        if (!ServiceBusTracer.IsTracing(requestContext))
          return;
        requestContext.RequestLogger.RequestLoggerInternal();
        foreach (BrokeredMessage message in messages)
        {
          ServiceBusTracer.ServiceBusMetadata messageMetadata = ServiceBusTracer.GetMessageMetadata(message);
          requestContext.TracingService().TraceServiceBusMetadataActivity(requestContext, messageMetadata.SourceHostId, (byte) messageMetadata.SourceHostType, sbNamespace ?? string.Empty, topicName ?? string.Empty, message.MessageId ?? string.Empty, message.ContentType ?? string.Empty, messageMetadata.TargetScaleUnits ?? string.Empty, success, startTime, publishTimeMs);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(59438424, nameof (ServiceBusTracer), "ServiceBus", ex);
      }
    }

    private static bool IsTracing(IVssRequestContext requestContext) => !requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.Services.ServiceBus.DisableServiceBusActivityLogTracing");

    private static ServiceBusTracer.ServiceBusMetadata GetMessageMetadata(BrokeredMessage message)
    {
      StringBuilder stringBuilder = new StringBuilder();
      Guid sourceHostId = Guid.Empty;
      int sourceHostType = 0;
      if (message.Properties != null)
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) message.Properties)
        {
          Guid result;
          if (property.Key.StartsWith("VSService_") && Guid.TryParse(property.Key.Substring(10), out result))
            stringBuilder.Append(string.Format("{0}:{1};", (object) result, (object) property.Value.ToString()));
        }
        object obj1;
        if (message.Properties.TryGetValue("SourceHostId", out obj1))
          sourceHostId = (Guid) obj1;
        object obj2;
        if (message.Properties.TryGetValue("SourceHostType", out obj2))
          sourceHostType = (int) obj2;
      }
      return new ServiceBusTracer.ServiceBusMetadata(stringBuilder.ToString(), sourceHostId, sourceHostType);
    }

    private struct ServiceBusMetadata
    {
      public ServiceBusMetadata(string targetScaleUnits, Guid sourceHostId, int sourceHostType)
      {
        this.TargetScaleUnits = targetScaleUnits;
        this.SourceHostId = sourceHostId;
        this.SourceHostType = sourceHostType;
      }

      public string TargetScaleUnits { get; private set; }

      public Guid SourceHostId { get; private set; }

      public int SourceHostType { get; private set; }
    }
  }
}
