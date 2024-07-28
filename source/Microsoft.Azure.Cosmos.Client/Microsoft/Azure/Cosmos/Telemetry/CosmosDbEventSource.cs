// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.CosmosDbEventSource
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure.Core.Diagnostics;
using Microsoft.Azure.Cosmos.Telemetry.Diagnostics;
using System.Diagnostics.Tracing;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  [EventSource(Name = "Azure.Cosmos")]
  internal sealed class CosmosDbEventSource : AzureEventSource
  {
    private const string EventSourceName = "Azure.Cosmos";

    private static CosmosDbEventSource Singleton { get; } = new CosmosDbEventSource();

    private CosmosDbEventSource()
      : base("Azure.Cosmos")
    {
    }

    [NonEvent]
    public static bool IsEnabled(EventLevel level) => CosmosDbEventSource.Singleton.IsEnabled(level, EventKeywords.None);

    [NonEvent]
    public static void RecordDiagnosticsForRequests(
      DistributedTracingOptions config,
      OpenTelemetryAttributes response)
    {
      if (CosmosDbEventSource.IsEnabled(EventLevel.Informational))
      {
        CosmosDbEventSource.Singleton.WriteInfoEvent(response.Diagnostics.ToString());
      }
      else
      {
        if (!DiagnosticsFilterHelper.IsTracingNeeded(config, response) || !CosmosDbEventSource.IsEnabled(EventLevel.Warning))
          return;
        CosmosDbEventSource.Singleton.WriteWarningEvent(response.Diagnostics.ToString());
      }
    }

    [NonEvent]
    public static void RecordDiagnosticsForExceptions(CosmosDiagnostics diagnostics)
    {
      if (!CosmosDbEventSource.IsEnabled(EventLevel.Error))
        return;
      CosmosDbEventSource.Singleton.WriteErrorEvent(diagnostics.ToString());
    }

    [Event(1, Level = EventLevel.Error)]
    private void WriteErrorEvent(string message) => this.WriteEvent(1, message);

    [Event(2, Level = EventLevel.Warning)]
    private void WriteWarningEvent(string message) => this.WriteEvent(2, message);

    [Event(3, Level = EventLevel.Informational)]
    private void WriteInfoEvent(string message) => this.WriteEvent(3, message);
  }
}
