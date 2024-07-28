// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.OpenTelemetryResponse`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Telemetry;
using System.Net;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class OpenTelemetryResponse<T> : OpenTelemetryAttributes
  {
    internal OpenTelemetryResponse(
      FeedResponse<T> responseMessage,
      string containerName = null,
      string databaseName = null)
      : this(responseMessage.StatusCode, responseMessage.Headers?.RequestCharge, responseMessage?.Headers?.ContentLength, responseMessage.Diagnostics, responseMessage.Headers?.ItemCount, databaseName, containerName, responseMessage.RequestMessage)
    {
    }

    internal OpenTelemetryResponse(
      Response<T> responseMessage,
      string containerName = null,
      string databaseName = null)
      : this(responseMessage.StatusCode, responseMessage.Headers?.RequestCharge, responseMessage?.Headers?.ContentLength, responseMessage.Diagnostics, responseMessage.Headers?.ItemCount, databaseName, containerName, responseMessage.RequestMessage)
    {
    }

    private OpenTelemetryResponse(
      HttpStatusCode statusCode,
      double? requestCharge,
      string responseContentLength,
      CosmosDiagnostics diagnostics,
      string itemCount,
      string databaseName,
      string containerName,
      RequestMessage requestMessage)
      : base(requestMessage, containerName, databaseName)
    {
      this.StatusCode = statusCode;
      this.RequestCharge = requestCharge;
      this.ResponseContentLength = responseContentLength ?? "information not available";
      this.Diagnostics = diagnostics;
      this.ItemCount = itemCount ?? "information not available";
    }
  }
}
