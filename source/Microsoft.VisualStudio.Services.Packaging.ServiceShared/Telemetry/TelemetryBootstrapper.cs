// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.TelemetryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class TelemetryBootstrapper : IBootstrapper<ITelemetryService>
  {
    private readonly IVssRequestContext requestContext;
    private readonly TelemetryTraceInfo traceInfo;

    public TelemetryBootstrapper(IVssRequestContext requestContext, TelemetryTraceInfo traceInfo)
    {
      this.requestContext = requestContext;
      this.traceInfo = traceInfo;
    }

    public ITelemetryService Bootstrap() => (ITelemetryService) new TelemetryPublisherFacade(this.requestContext, (IFactory<ITelemetryPublisher>) new TelemetryPublisherFactoryFacade(this.traceInfo, this.requestContext.GetExecutionEnvironmentFacade()));
  }
}
