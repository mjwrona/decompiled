// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.TelemetryPublisherFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class TelemetryPublisherFacade : 
    ITelemetryService,
    IAsyncHandler<ICiData>,
    IAsyncHandler<ICiData, NullResult>,
    IHaveInputType<ICiData>,
    IHaveOutputType<NullResult>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFactory<ITelemetryPublisher> telemetryPublisher;

    public TelemetryPublisherFacade(
      IVssRequestContext requestContext,
      IFactory<ITelemetryPublisher> telemetryPublisher)
    {
      this.requestContext = requestContext;
      this.telemetryPublisher = telemetryPublisher;
    }

    public Task<NullResult> Handle(ICiData request)
    {
      if (request != null)
        this.telemetryPublisher.Get().Publish(this.requestContext, request);
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
