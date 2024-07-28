// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ExecutionContext
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class ExecutionContext
  {
    internal ExecutionContext()
    {
    }

    public ExecutionContext(
      IVssRequestContext requestContext,
      ITracerCICorrelationDetails tracerCICorrelationDetails)
      : this(requestContext, tracerCICorrelationDetails, new EventProcessingContext(requestContext))
    {
    }

    public ExecutionContext(
      IVssRequestContext requestContext,
      ITracerCICorrelationDetails tracerCICorrelationDetails,
      EventProcessingContext eventProcessingContext)
    {
      this.RequestContext = requestContext;
      this.ServiceSettings = new ServiceSettings(requestContext);
      this.ExecutionTracerContext = new ExecutionTracerContext(tracerCICorrelationDetails);
      this.EventProcessingContext = eventProcessingContext;
    }

    public IVssRequestContext RequestContext { get; internal set; }

    public ServiceSettings ServiceSettings { get; set; }

    public EventProcessingContext EventProcessingContext { get; set; }

    public IIndexerFaultService FaultService { get; set; }

    public ExecutionTracerContext ExecutionTracerContext { get; internal set; }
  }
}
