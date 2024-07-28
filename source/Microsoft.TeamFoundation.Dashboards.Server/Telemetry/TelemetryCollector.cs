// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Telemetry.TelemetryCollector
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Dashboards.Telemetry
{
  public class TelemetryCollector : IDisposable
  {
    private string m_featureName;
    private IVssRequestContext m_requestContext;
    private Dictionary<string, string> m_properties;
    private PerformanceTimer m_performanceTimer;
    private int? m_tracePointId;
    private string m_layer;
    private static readonly string Area = "Dashboard";

    private bool TraceEnabled => this.m_tracePointId.HasValue && !string.IsNullOrWhiteSpace(this.m_layer);

    public Dictionary<string, string> Properties => this.m_properties;

    public static TelemetryCollector TraceMonitor(
      IVssRequestContext requestContext,
      int tracePointId,
      string layer,
      string feature)
    {
      return new TelemetryCollector(requestContext, new int?(tracePointId), layer, feature);
    }

    public void Dispose() => this.TraceLeave();

    private TelemetryCollector(
      IVssRequestContext requestContext,
      int? tracepointId,
      string layer,
      string featureName)
    {
      this.m_featureName = featureName;
      this.m_requestContext = requestContext;
      this.m_requestContext = requestContext;
      this.m_tracePointId = tracepointId;
      this.m_layer = layer;
      this.m_properties = new Dictionary<string, string>();
      this.TraceEnter();
    }

    private void TraceEnter()
    {
      if (this.TraceEnabled)
        this.m_requestContext.TraceEnter(this.m_tracePointId.Value, TelemetryCollector.Area, this.m_layer, this.m_featureName);
      this.m_performanceTimer = PerformanceTimer.StartMeasure(this.m_requestContext, this.m_featureName);
    }

    private void TraceLeave()
    {
      if (this.TraceEnabled)
        this.m_requestContext.TraceLeave(this.m_tracePointId.Value + 1, TelemetryCollector.Area, this.m_layer, this.m_featureName);
      this.m_performanceTimer.Dispose();
      if (this.m_properties.Keys.Count <= 0)
        return;
      this.SaveCustomerIntelligenceData();
    }

    private void SaveCustomerIntelligenceData()
    {
      CustomerIntelligenceService service = this.m_requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.ActivityId, this.m_requestContext.ActivityId.ToString());
      foreach (KeyValuePair<string, string> property in this.Properties)
        properties.Add(property.Key, property.Value);
      service.Publish(this.m_requestContext, TelemetryCollector.Area, this.m_featureName, properties);
    }
  }
}
