// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.Diagnostics.DiagnosticsFilterHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;

namespace Microsoft.Azure.Cosmos.Telemetry.Diagnostics
{
  internal static class DiagnosticsFilterHelper
  {
    public static bool IsTracingNeeded(
      DistributedTracingOptions config,
      OpenTelemetryAttributes response)
    {
      TimeSpan timeSpan = config == null || !config.DiagnosticsLatencyThreshold.HasValue ? (response.OperationType == OperationType.Query.ToOperationTypeString() ? DistributedTracingOptions.DefaultQueryTimeoutThreshold : DistributedTracingOptions.DefaultCrudLatencyThreshold) : config.DiagnosticsLatencyThreshold.Value;
      return response.Diagnostics.GetClientElapsedTime() > timeSpan || !response.StatusCode.IsSuccess();
    }
  }
}
