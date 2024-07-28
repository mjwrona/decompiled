// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Telemetry.NuGetPackagingTelemetryBuilder
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Telemetry
{
  public class NuGetPackagingTelemetryBuilder
  {
    public ITelemetryPublisher Build(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment ? (ITelemetryPublisher) new HostedTelemetryPublisher(NuGetTracePoints.Telemetry.TraceInfo) : (ITelemetryPublisher) new OnPremTelemetryPublisher();
  }
}
