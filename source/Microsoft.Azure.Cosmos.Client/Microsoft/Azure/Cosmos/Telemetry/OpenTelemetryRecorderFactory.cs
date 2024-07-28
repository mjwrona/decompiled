// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.OpenTelemetryRecorderFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure.Core.Pipeline;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  internal static class OpenTelemetryRecorderFactory
  {
    private static DiagnosticScopeFactory ScopeFactory { get; set; }

    public static OpenTelemetryCoreRecorder CreateRecorder(
      string operationName,
      RequestOptions requestOptions,
      CosmosClientContext clientContext)
    {
      if (clientContext != null)
      {
        CosmosClientOptions clientOptions = clientContext.ClientOptions;
        if (clientOptions != null && clientOptions.EnableDistributedTracing)
        {
          OpenTelemetryRecorderFactory.ScopeFactory = new DiagnosticScopeFactory("Azure.Cosmos", "Microsoft.DocumentDB", true);
          DiagnosticScope scope = OpenTelemetryRecorderFactory.ScopeFactory.CreateScope("Cosmos." + operationName);
          if (scope.IsEnabled)
            return new OpenTelemetryCoreRecorder(scope, clientContext, requestOptions?.DistributedTracingOptions ?? clientContext.ClientOptions?.DistributedTracingOptions);
        }
      }
      return new OpenTelemetryCoreRecorder();
    }
  }
}
