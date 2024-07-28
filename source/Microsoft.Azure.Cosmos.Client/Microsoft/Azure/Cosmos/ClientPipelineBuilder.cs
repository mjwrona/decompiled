// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ClientPipelineBuilder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Handlers;
using Microsoft.Azure.Cosmos.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos
{
  internal class ClientPipelineBuilder
  {
    private readonly CosmosClient client;
    private readonly ConsistencyLevel? requestedClientConsistencyLevel;
    private readonly DiagnosticsHandler diagnosticsHandler;
    private readonly RequestHandler invalidPartitionExceptionRetryHandler;
    private readonly RequestHandler transportHandler;
    private readonly TelemetryHandler telemetryHandler;
    private IReadOnlyCollection<RequestHandler> customHandlers;
    private RequestHandler retryHandler;

    public ClientPipelineBuilder(
      CosmosClient client,
      ConsistencyLevel? requestedClientConsistencyLevel,
      IReadOnlyCollection<RequestHandler> customHandlers,
      ClientTelemetry telemetry)
    {
      this.client = client ?? throw new ArgumentNullException(nameof (client));
      this.requestedClientConsistencyLevel = requestedClientConsistencyLevel;
      this.transportHandler = (RequestHandler) new TransportHandler(client);
      this.invalidPartitionExceptionRetryHandler = (RequestHandler) new NamedCacheRetryHandler();
      this.PartitionKeyRangeHandler = (RequestHandler) new Microsoft.Azure.Cosmos.Handlers.PartitionKeyRangeHandler(client);
      this.diagnosticsHandler = new DiagnosticsHandler();
      if (telemetry != null)
        this.telemetryHandler = new TelemetryHandler(telemetry);
      this.UseRetryPolicy();
      this.AddCustomHandlers(customHandlers);
    }

    internal IReadOnlyCollection<RequestHandler> CustomHandlers
    {
      get => this.customHandlers;
      private set
      {
        if (value != null && value.Any<RequestHandler>((Func<RequestHandler, bool>) (x => x?.InnerHandler != null)))
          throw new ArgumentOutOfRangeException(nameof (CustomHandlers));
        this.customHandlers = value;
      }
    }

    internal RequestHandler PartitionKeyRangeHandler { get; set; }

    public RequestInvokerHandler Build()
    {
      RequestInvokerHandler requestInvokerHandler = new RequestInvokerHandler(this.client, this.requestedClientConsistencyLevel);
      RequestHandler requestHandler1 = (RequestHandler) requestInvokerHandler;
      if (this.CustomHandlers != null && this.CustomHandlers.Any<RequestHandler>())
      {
        foreach (RequestHandler customHandler in (IEnumerable<RequestHandler>) this.CustomHandlers)
        {
          requestHandler1.InnerHandler = customHandler;
          requestHandler1 = requestHandler1.InnerHandler;
        }
      }
      if (this.diagnosticsHandler != null)
      {
        requestHandler1.InnerHandler = (RequestHandler) this.diagnosticsHandler;
        requestHandler1 = requestHandler1.InnerHandler;
      }
      if (this.telemetryHandler != null)
      {
        requestHandler1.InnerHandler = (RequestHandler) this.telemetryHandler;
        requestHandler1 = requestHandler1.InnerHandler;
      }
      requestHandler1.InnerHandler = this.retryHandler;
      RequestHandler innerHandler1 = requestHandler1.InnerHandler;
      RequestHandler requestHandler2 = (RequestHandler) new RouterHandler(this.CreateDocumentFeedPipeline(), this.transportHandler);
      innerHandler1.InnerHandler = requestHandler2;
      RequestHandler innerHandler2 = innerHandler1.InnerHandler;
      return requestInvokerHandler;
    }

    internal static RequestHandler CreatePipeline(params RequestHandler[] requestHandlers)
    {
      RequestHandler pipeline = (RequestHandler) null;
      for (int index = requestHandlers.Length - 1; index >= 0; --index)
      {
        RequestHandler requestHandler = requestHandlers[index];
        if (requestHandler.InnerHandler != null)
          throw new ArgumentOutOfRangeException(string.Format("The requestHandlers[{0}].InnerHandler is required to be null to allow the pipeline to chain the handlers.", (object) index));
        if (pipeline != null)
          requestHandler.InnerHandler = pipeline;
        pipeline = requestHandler;
      }
      return pipeline;
    }

    private ClientPipelineBuilder UseRetryPolicy()
    {
      this.retryHandler = (RequestHandler) new RetryHandler(this.client);
      return this;
    }

    private ClientPipelineBuilder AddCustomHandlers(
      IReadOnlyCollection<RequestHandler> customHandlers)
    {
      this.CustomHandlers = customHandlers;
      return this;
    }

    private RequestHandler CreateDocumentFeedPipeline() => ClientPipelineBuilder.CreatePipeline(this.invalidPartitionExceptionRetryHandler, this.PartitionKeyRangeHandler, this.transportHandler);
  }
}
