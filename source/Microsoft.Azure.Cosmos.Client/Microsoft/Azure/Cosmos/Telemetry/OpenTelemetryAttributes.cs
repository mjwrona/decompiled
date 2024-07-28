// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.OpenTelemetryAttributes
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System.Net;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  internal class OpenTelemetryAttributes
  {
    internal const string NotAvailable = "information not available";

    internal OpenTelemetryAttributes()
    {
    }

    internal OpenTelemetryAttributes(
      RequestMessage requestMessage,
      string containerName,
      string databaseName)
    {
      this.RequestContentLength = requestMessage?.Headers?.ContentLength ?? "information not available";
      this.OperationType = (requestMessage != null ? requestMessage.OperationType.ToOperationTypeString() : (string) null) ?? "information not available";
      this.DatabaseName = requestMessage?.DatabaseId ?? databaseName ?? "information not available";
      this.ContainerName = requestMessage?.ContainerId ?? containerName ?? "information not available";
    }

    internal HttpStatusCode StatusCode { get; set; }

    internal double? RequestCharge { get; set; }

    internal string RequestContentLength { get; set; }

    internal string ResponseContentLength { get; set; }

    internal string DatabaseName { get; set; }

    internal string ContainerName { get; set; }

    internal string ItemCount { get; set; }

    internal CosmosDiagnostics Diagnostics { get; set; }

    internal string OperationType { get; set; }
  }
}
