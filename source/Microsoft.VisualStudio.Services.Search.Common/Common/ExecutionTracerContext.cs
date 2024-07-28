// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ExecutionTracerContext
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class ExecutionTracerContext
  {
    public ExecutionTracerContext(
      ITracerCICorrelationDetails tracerCICorrelationDetails)
    {
      this.TracerCICorrelationDetails = tracerCICorrelationDetails;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("ExecutionContext", "ExecutionContext", this.TracerCICorrelationDetails.GetAllCorrelationDetails(), true);
    }

    public ITracerCICorrelationDetails TracerCICorrelationDetails { get; }

    public int Trigger => this.TracerCICorrelationDetails.Trigger;

    public void PublishCi(
      string area,
      string feature,
      CustomerIntelligenceData ci,
      bool requiredForOnPrem = false)
    {
      if (ci == null)
        throw new ArgumentNullException(nameof (ci));
      ci.Add("CorrelationId", this.TracerCICorrelationDetails.CorrelationId);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi(area, feature, ci, requiredForOnPrem);
    }

    public void PublishCi(
      string area,
      string feature,
      IDictionary<string, object> properties,
      bool requiredForOnPrem = false)
    {
      this.PublishCi(area, feature, new CustomerIntelligenceData(properties), requiredForOnPrem);
    }

    public void PublishCi(
      string area,
      string feature,
      string property,
      bool value,
      bool requiredForOnPrem = false)
    {
      CustomerIntelligenceData ci = new CustomerIntelligenceData();
      ci.Add(property, value);
      this.PublishCi(area, feature, ci, requiredForOnPrem);
    }

    public void PublishCi(
      string area,
      string feature,
      string property,
      double value,
      bool requiredForOnPrem = false)
    {
      CustomerIntelligenceData ci = new CustomerIntelligenceData();
      ci.Add(property, value);
      this.PublishCi(area, feature, ci, requiredForOnPrem);
    }

    public void PublishCi(
      string area,
      string feature,
      string property,
      string value,
      bool requiredForOnPremise = false)
    {
      CustomerIntelligenceData ci = new CustomerIntelligenceData();
      ci.Add(property, value);
      this.PublishCi(area, feature, ci, requiredForOnPremise);
    }

    public virtual void PublishClientTrace(
      string area,
      string feature,
      ClientTraceData clientTraceData,
      bool requiredForOnPrem = false)
    {
      if (clientTraceData == null)
        throw new ArgumentNullException(nameof (clientTraceData));
      clientTraceData.Add("CorrelationId", (object) this.TracerCICorrelationDetails.CorrelationId);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace(area, feature, clientTraceData, requiredForOnPrem);
    }

    public void PublishClientTrace(
      string area,
      string feature,
      IDictionary<string, object> properties,
      bool requiredForOnPrem = false)
    {
      this.PublishClientTrace(area, feature, new ClientTraceData(properties), requiredForOnPrem);
    }

    public void PublishClientTrace(
      string area,
      string feature,
      string property,
      object value,
      bool requiredForOnPrem = false)
    {
      ClientTraceData clientTraceData = new ClientTraceData();
      clientTraceData.Add(property, value);
      this.PublishClientTrace(area, feature, clientTraceData, requiredForOnPrem);
    }

    public void PublishKpi(string kpiName, string kpiArea, double value, bool requiredForOnPrem = false) => Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi(kpiName, kpiArea, value, requiredForOnPrem);
  }
}
