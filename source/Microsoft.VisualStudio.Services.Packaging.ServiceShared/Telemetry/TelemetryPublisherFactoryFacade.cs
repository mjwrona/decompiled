// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.TelemetryPublisherFactoryFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class TelemetryPublisherFactoryFacade : IFactory<ITelemetryPublisher>
  {
    private readonly TelemetryTraceInfo traceInfo;
    private readonly IExecutionEnvironment executionEnvironment;

    public TelemetryPublisherFactoryFacade(
      TelemetryTraceInfo traceInfo,
      IExecutionEnvironment executionEnvironment)
    {
      this.traceInfo = traceInfo;
      this.executionEnvironment = executionEnvironment;
    }

    public ITelemetryPublisher Get() => this.executionEnvironment.IsHosted() ? (ITelemetryPublisher) new HostedTelemetryPublisher(this.traceInfo) : (ITelemetryPublisher) new OnPremTelemetryPublisher();
  }
}
