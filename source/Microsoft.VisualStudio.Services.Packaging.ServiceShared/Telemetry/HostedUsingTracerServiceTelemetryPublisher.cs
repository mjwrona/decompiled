// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.HostedUsingTracerServiceTelemetryPublisher
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class HostedUsingTracerServiceTelemetryPublisher : ITelemetryPublisher
  {
    private readonly ITracerService tracerService;

    public HostedUsingTracerServiceTelemetryPublisher(ITracerService tracerService) => this.tracerService = tracerService;

    public void Publish(IVssRequestContext requestContext, ICiData ciData)
    {
      try
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, ciData.AreaName, ciData.FeatureName, ciData.CiData);
      }
      catch (Exception ex)
      {
        using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Publish)))
          tracerBlock.TraceException(ex);
      }
    }
  }
}
