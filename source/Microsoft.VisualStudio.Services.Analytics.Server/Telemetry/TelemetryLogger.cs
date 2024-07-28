// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Telemetry.TelemetryLogger
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Analytics.Telemetry
{
  public class TelemetryLogger : ITelemetryLogger
  {
    static TelemetryLogger() => TelemetryLogger.Instance = (ITelemetryLogger) new TelemetryLogger();

    public void Publish(
      IVssRequestContext requestContext,
      string feature,
      Dictionary<string, object> eventData)
    {
      try
      {
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        CustomerIntelligenceData intelligenceData = this.CreateCustomerIntelligenceData(eventData);
        IVssRequestContext requestContext1 = requestContext;
        Guid hostId = instanceId;
        string feature1 = feature;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, hostId, "Analytics", feature1, properties);
      }
      catch (Exception ex)
      {
        requestContext.Trace(12099999, TraceLevel.Error, "Analytics", "Service", "Failed to publish telemetry: " + ex.Message);
      }
    }

    private CustomerIntelligenceData CreateCustomerIntelligenceData(
      Dictionary<string, object> eventData)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      foreach (KeyValuePair<string, object> keyValuePair in eventData)
        intelligenceData.Add(keyValuePair.Key, keyValuePair.Value);
      return intelligenceData;
    }

    public static ITelemetryLogger Instance { get; set; }
  }
}
