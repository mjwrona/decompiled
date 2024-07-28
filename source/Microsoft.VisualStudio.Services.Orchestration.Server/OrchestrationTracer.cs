// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationTracer
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Tracing;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  public class OrchestrationTracer : IOrchestrationTracer
  {
    private readonly string m_area;
    private readonly string m_layer;
    private readonly string m_hubName;
    private readonly bool m_traceAlways;
    private readonly IVssRequestContext m_requestContext;

    public OrchestrationTracer(
      IVssRequestContext requestContext,
      string area,
      string layer,
      string hubName,
      bool traceAlways = false)
    {
      this.m_area = area;
      this.m_layer = layer;
      this.m_hubName = hubName;
      this.m_requestContext = requestContext;
      this.m_traceAlways = traceAlways;
    }

    public void TraceStarted(string orchestrationId, string orchestrationName) => this.m_requestContext.GetService<IOrchestrationLogTracingService>().TraceOrchestrationLogNewOrchestration(this.m_requestContext, orchestrationId, -1L, "Pipelines", this.m_hubName);

    public void TracePhaseStarted(string orchestrationId, string orchestrationName, string action) => this.m_requestContext.GetService<IOrchestrationLogTracingService>().TraceOrchestrationLogPhaseStarted(this.m_requestContext, orchestrationId, -1L, "Pipelines", this.m_hubName, orchestrationName + "." + action);

    public void TraceCompleted(string orchestrationId, string orchestrationName, string action) => this.m_requestContext.GetService<IOrchestrationLogTracingService>().TraceOrchestrationLogCompletion(this.m_requestContext, orchestrationId, "Pipelines", this.m_hubName, orchestrationName + "." + action);

    public void TraceCompletedWithError(
      string orchestrationId,
      string orchestrationName,
      string action,
      string errorCode,
      string errorMessage,
      bool errorIsExpected)
    {
      this.m_requestContext.GetService<IOrchestrationLogTracingService>().TraceOrchestrationLogCompletionWithError(this.m_requestContext, orchestrationId, "Pipelines", this.m_hubName, orchestrationName + "." + action, errorCode, errorMessage, errorIsExpected);
    }

    public void Trace(
      string orchestrationId,
      int eventId,
      TraceLevel level,
      string format,
      params object[] arguments)
    {
      this.Trace(orchestrationId, eventId, level, this.m_area, this.m_layer, format, arguments);
    }

    public void Trace(
      string orchestrationId,
      int eventId,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] arguments)
    {
      using (this.m_requestContext.CreateOrchestrationIdScope(orchestrationId))
      {
        try
        {
          if (this.m_traceAlways)
            this.m_requestContext.TraceAlways(eventId, level, area, layer, format, arguments);
          else
            VssRequestContextExtensions.Trace(this.m_requestContext, eventId, level, area, layer, format, arguments);
        }
        catch (Exception ex)
        {
          this.m_requestContext.TraceException(0, area, layer, ex);
        }
      }
    }
  }
}
