// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosClientContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Handlers;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class CosmosClientContext : IDisposable
  {
    internal abstract CosmosClient Client { get; }

    internal abstract DocumentClient DocumentClient { get; }

    internal abstract CosmosSerializerCore SerializerCore { get; }

    internal abstract CosmosResponseFactoryInternal ResponseFactory { get; }

    internal abstract RequestInvokerHandler RequestHandler { get; }

    internal abstract CosmosClientOptions ClientOptions { get; }

    internal abstract string UserAgent { get; }

    internal abstract BatchAsyncContainerExecutor GetExecutorForContainer(
      ContainerInternal container);

    internal abstract string CreateLink(string parentLink, string uriPathSegment, string id);

    internal abstract void ValidateResource(string id);

    internal abstract Task<ContainerProperties> GetCachedContainerPropertiesAsync(
      string containerUri,
      ITrace trace,
      CancellationToken cancellationToken);

    internal abstract Task<TResult> OperationHelperAsync<TResult>(
      string operationName,
      RequestOptions requestOptions,
      Func<ITrace, Task<TResult>> task,
      Func<TResult, OpenTelemetryAttributes> openTelemetry = null,
      TraceComponent traceComponent = TraceComponent.Transport,
      TraceLevel traceLevel = TraceLevel.Info);

    internal abstract Task<ResponseMessage> ProcessResourceOperationStreamAsync(
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal cosmosContainerCore,
      PartitionKey? partitionKey,
      string itemId,
      Stream streamPayload,
      Action<RequestMessage> requestEnricher,
      ITrace trace,
      CancellationToken cancellationToken);

    internal abstract Task<ResponseMessage> ProcessResourceOperationStreamAsync(
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal cosmosContainerCore,
      FeedRange feedRange,
      Stream streamPayload,
      Action<RequestMessage> requestEnricher,
      ITrace trace,
      CancellationToken cancellationToken);

    internal abstract Task<T> ProcessResourceOperationAsync<T>(
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal containerInternal,
      FeedRange feedRange,
      Stream streamPayload,
      Action<RequestMessage> requestEnricher,
      Func<ResponseMessage, T> responseCreator,
      ITrace trace,
      CancellationToken cancellationToken);

    public abstract void Dispose();
  }
}
