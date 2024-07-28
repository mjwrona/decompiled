// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Handlers.TelemetryHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Telemetry;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Handlers
{
  internal class TelemetryHandler : RequestHandler
  {
    private readonly ClientTelemetry telemetry;

    public TelemetryHandler(ClientTelemetry telemetry) => this.telemetry = telemetry ?? throw new ArgumentNullException(nameof (telemetry));

    public override async Task<ResponseMessage> SendAsync(
      RequestMessage request,
      CancellationToken cancellationToken)
    {
      ResponseMessage response = await base.SendAsync(request, cancellationToken);
      if (this.IsAllowed(request))
      {
        try
        {
          this.telemetry.Collect(response.Diagnostics, response.StatusCode, this.GetPayloadSize(response), request.ContainerId, request.DatabaseId, request.OperationType, request.ResourceType, request.Headers?["x-ms-consistency-level"], response.Headers.RequestCharge, response.Headers.SubStatusCodeLiteral);
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceError("Error while collecting telemetry information : " + ex.Message);
        }
      }
      return response;
    }

    private bool IsAllowed(RequestMessage request) => ClientTelemetryOptions.AllowedResourceTypes.Equals((object) request.ResourceType);

    private long GetPayloadSize(ResponseMessage response)
    {
      if (response != null)
      {
        if (response.Content != null && response.Content is MemoryStream)
          return response.Content.Length;
        if (response.Headers != null && response.Headers.ContentLength != null)
          return long.Parse(response.Headers.ContentLength);
      }
      return 0;
    }
  }
}
