// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ClientContextCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Handler;
using Microsoft.Azure.Cosmos.Handlers;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class ClientContextCore : CosmosClientContext
  {
    private readonly BatchAsyncContainerExecutorCache batchExecutorCache;
    private readonly CosmosClient client;
    private readonly DocumentClient documentClient;
    private readonly CosmosSerializerCore serializerCore;
    private readonly CosmosResponseFactoryInternal responseFactory;
    private readonly RequestInvokerHandler requestHandler;
    private readonly CosmosClientOptions clientOptions;
    private readonly ClientTelemetry telemetry;
    private readonly string userAgent;
    private bool isDisposed;

    private ClientContextCore(
      CosmosClient client,
      CosmosClientOptions clientOptions,
      CosmosSerializerCore serializerCore,
      CosmosResponseFactoryInternal cosmosResponseFactory,
      RequestInvokerHandler requestHandler,
      DocumentClient documentClient,
      string userAgent,
      BatchAsyncContainerExecutorCache batchExecutorCache,
      ClientTelemetry telemetry)
    {
      this.client = client;
      this.clientOptions = clientOptions;
      this.serializerCore = serializerCore;
      this.responseFactory = cosmosResponseFactory;
      this.requestHandler = requestHandler;
      this.documentClient = documentClient;
      this.userAgent = userAgent;
      this.batchExecutorCache = batchExecutorCache;
      this.telemetry = telemetry;
    }

    internal static CosmosClientContext Create(
      CosmosClient cosmosClient,
      CosmosClientOptions clientOptions)
    {
      if (cosmosClient == null)
        throw new ArgumentNullException(nameof (cosmosClient));
      clientOptions = ClientContextCore.CreateOrCloneClientOptions(clientOptions);
      HttpMessageHandler httpClientHandler = CosmosHttpClientCore.CreateHttpClientHandler(clientOptions.GatewayModeMaxConnectionLimit, clientOptions.WebProxy);
      Uri endpoint = cosmosClient.Endpoint;
      AuthorizationTokenProvider authorizationTokenProvider = cosmosClient.AuthorizationTokenProvider;
      ApiType apiType = clientOptions.ApiType;
      EventHandler<SendingRequestEventArgs> requestEventArgs = clientOptions.SendingRequestEventArgs;
      Func<TransportClient, TransportClient> clientHandlerFactory = clientOptions.TransportClientHandlerFactory;
      ConnectionPolicy connectionPolicy = clientOptions.GetConnectionPolicy(cosmosClient.ClientId);
      bool? enableCpuMonitor1 = clientOptions.EnableCpuMonitor;
      IStoreClientFactory storeClientFactory1 = clientOptions.StoreClientFactory;
      Microsoft.Azure.Documents.ConsistencyLevel? consistencyLevel = clientOptions.GetDocumentsConsistencyLevel();
      int apitype = (int) apiType;
      HttpMessageHandler handler = httpClientHandler;
      ISessionContainer sessionContainer = clientOptions.SessionContainer;
      bool? enableCpuMonitor2 = enableCpuMonitor1;
      Func<TransportClient, TransportClient> transportClientHandlerFactory = clientHandlerFactory;
      IStoreClientFactory storeClientFactory2 = storeClientFactory1;
      DocumentClient documentClient = new DocumentClient(endpoint, authorizationTokenProvider, requestEventArgs, connectionPolicy, consistencyLevel, apitype: (ApiType) apitype, handler: handler, sessionContainer: sessionContainer, enableCpuMonitor: enableCpuMonitor2, transportClientHandlerFactory: transportClientHandlerFactory, storeClientFactory: storeClientFactory2);
      return ClientContextCore.Create(cosmosClient, documentClient, clientOptions);
    }

    internal static CosmosClientContext Create(
      CosmosClient cosmosClient,
      DocumentClient documentClient,
      CosmosClientOptions clientOptions,
      RequestInvokerHandler requestInvokerHandler = null)
    {
      if (cosmosClient == null)
        throw new ArgumentNullException(nameof (cosmosClient));
      if (documentClient == null)
        throw new ArgumentNullException(nameof (documentClient));
      clientOptions = ClientContextCore.CreateOrCloneClientOptions(clientOptions);
      ConnectionPolicy connectionPolicy = clientOptions.GetConnectionPolicy(cosmosClient.ClientId);
      ClientTelemetry telemetry = (ClientTelemetry) null;
      if (connectionPolicy.EnableClientTelemetry)
      {
        try
        {
          telemetry = ClientTelemetry.CreateAndStartBackgroundTelemetry(cosmosClient.Id, documentClient, connectionPolicy.UserAgentContainer.UserAgent, connectionPolicy.ConnectionMode, cosmosClient.AuthorizationTokenProvider, DiagnosticsHandlerHelper.Instance, clientOptions.ApplicationPreferredRegions);
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceInformation("Error While starting Telemetry Job : " + ex.Message + ". Hence disabling Client Telemetry");
          connectionPolicy.EnableClientTelemetry = false;
        }
      }
      else
        DefaultTrace.TraceInformation("Client Telemetry Disabled.");
      if (requestInvokerHandler == null)
        requestInvokerHandler = new ClientPipelineBuilder(cosmosClient, clientOptions.ConsistencyLevel, (IReadOnlyCollection<Microsoft.Azure.Cosmos.RequestHandler>) clientOptions.CustomHandlers, telemetry).Build();
      CosmosSerializerCore cosmosSerializerCore = CosmosSerializerCore.Create(clientOptions.Serializer, clientOptions.SerializerOptions);
      clientOptions.SetSerializerIfNotConfigured(cosmosSerializerCore.GetCustomOrDefaultSerializer());
      CosmosResponseFactoryInternal cosmosResponseFactory = (CosmosResponseFactoryInternal) new CosmosResponseFactoryCore(cosmosSerializerCore);
      return (CosmosClientContext) new ClientContextCore(cosmosClient, clientOptions, cosmosSerializerCore, cosmosResponseFactory, requestInvokerHandler, documentClient, documentClient.ConnectionPolicy.UserAgentContainer.UserAgent, new BatchAsyncContainerExecutorCache(), telemetry);
    }

    internal override CosmosClient Client => this.ThrowIfDisposed<CosmosClient>(this.client);

    internal override DocumentClient DocumentClient => this.ThrowIfDisposed<DocumentClient>(this.documentClient);

    internal override CosmosSerializerCore SerializerCore => this.ThrowIfDisposed<CosmosSerializerCore>(this.serializerCore);

    internal override CosmosResponseFactoryInternal ResponseFactory => this.ThrowIfDisposed<CosmosResponseFactoryInternal>(this.responseFactory);

    internal override RequestInvokerHandler RequestHandler => this.ThrowIfDisposed<RequestInvokerHandler>(this.requestHandler);

    internal override CosmosClientOptions ClientOptions => this.ThrowIfDisposed<CosmosClientOptions>(this.clientOptions);

    internal override string UserAgent => this.ThrowIfDisposed<string>(this.userAgent);

    internal override string CreateLink(string parentLink, string uriPathSegment, string id)
    {
      this.ThrowIfDisposed();
      int length = parentLink != null ? parentLink.Length : 0;
      string str = Uri.EscapeUriString(id);
      StringBuilder stringBuilder = new StringBuilder(length + 2 + uriPathSegment.Length + str.Length);
      if (length > 0)
      {
        stringBuilder.Append(parentLink);
        stringBuilder.Append("/");
      }
      stringBuilder.Append(uriPathSegment);
      stringBuilder.Append("/");
      stringBuilder.Append(str);
      return stringBuilder.ToString();
    }

    internal override void ValidateResource(string resourceId)
    {
      this.ThrowIfDisposed();
      this.DocumentClient.ValidateResource(resourceId);
    }

    internal override Task<TResult> OperationHelperAsync<TResult>(
      string operationName,
      RequestOptions requestOptions,
      Func<ITrace, Task<TResult>> task,
      Func<TResult, OpenTelemetryAttributes> openTelemetry,
      TraceComponent traceComponent = TraceComponent.Transport,
      TraceLevel traceLevel = TraceLevel.Info)
    {
      return SynchronizationContext.Current != null ? this.OperationHelperWithRootTraceWithSynchronizationContextAsync<TResult>(operationName, requestOptions, task, openTelemetry, traceComponent, traceLevel) : this.OperationHelperWithRootTraceAsync<TResult>(operationName, requestOptions, task, openTelemetry, traceComponent, traceLevel);
    }

    private async Task<TResult> OperationHelperWithRootTraceAsync<TResult>(
      string operationName,
      RequestOptions requestOptions,
      Func<ITrace, Task<TResult>> task,
      Func<TResult, OpenTelemetryAttributes> openTelemetry,
      TraceComponent traceComponent,
      TraceLevel traceLevel)
    {
      TResult result;
      using (ITrace trace = requestOptions != null && requestOptions.DisablePointOperationDiagnostics ? (ITrace) NoOpTrace.Singleton : (ITrace) Microsoft.Azure.Cosmos.Tracing.Trace.GetRootTrace(operationName, traceComponent, traceLevel))
      {
        trace.AddDatum("Client Configuration", (TraceDatum) this.client.ClientConfigurationTraceDatum);
        result = await this.RunWithDiagnosticsHelperAsync<TResult>(trace, task, openTelemetry, operationName, requestOptions);
      }
      return result;
    }

    private Task<TResult> OperationHelperWithRootTraceWithSynchronizationContextAsync<TResult>(
      string operationName,
      RequestOptions requestOptions,
      Func<ITrace, Task<TResult>> task,
      Func<TResult, OpenTelemetryAttributes> openTelemetry,
      TraceComponent traceComponent,
      TraceLevel traceLevel)
    {
      string syncContextVirtualAddress = SynchronizationContext.Current.ToString();
      return Task.Run<TResult>((Func<Task<TResult>>) (async () =>
      {
        TResult result;
        using (ITrace trace = requestOptions != null && requestOptions.DisablePointOperationDiagnostics ? (ITrace) NoOpTrace.Singleton : (ITrace) Microsoft.Azure.Cosmos.Tracing.Trace.GetRootTrace(operationName, traceComponent, traceLevel))
        {
          trace.AddDatum("Synchronization Context", (object) syncContextVirtualAddress);
          result = await this.RunWithDiagnosticsHelperAsync<TResult>(trace, task, openTelemetry, operationName, requestOptions);
        }
        return result;
      }));
    }

    internal override Task<ResponseMessage> ProcessResourceOperationStreamAsync(
      string resourceUri,
      ResourceType resourceType,
      Microsoft.Azure.Documents.OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal cosmosContainerCore,
      PartitionKey? partitionKey,
      string itemId,
      Stream streamPayload,
      Action<RequestMessage> requestEnricher,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      if (!this.IsBulkOperationSupported(resourceType, operationType))
        return this.ProcessResourceOperationStreamAsync(resourceUri, resourceType, operationType, requestOptions, cosmosContainerCore, partitionKey.HasValue ? (FeedRange) new FeedRangePartitionKey(partitionKey.Value) : (FeedRange) null, streamPayload, requestEnricher, trace, cancellationToken);
      if (!partitionKey.HasValue)
        throw new ArgumentOutOfRangeException(nameof (partitionKey));
      if (requestEnricher != null)
        throw new ArgumentException("Bulk does not support requestEnricher");
      return this.ProcessResourceOperationAsBulkStreamAsync(operationType, requestOptions, cosmosContainerCore, partitionKey.Value, itemId, streamPayload, trace, cancellationToken);
    }

    internal override Task<ResponseMessage> ProcessResourceOperationStreamAsync(
      string resourceUri,
      ResourceType resourceType,
      Microsoft.Azure.Documents.OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal cosmosContainerCore,
      FeedRange feedRange,
      Stream streamPayload,
      Action<RequestMessage> requestEnricher,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      return this.RequestHandler.SendAsync(resourceUri, resourceType, operationType, requestOptions, cosmosContainerCore, feedRange, streamPayload, requestEnricher, trace, cancellationToken);
    }

    internal override Task<T> ProcessResourceOperationAsync<T>(
      string resourceUri,
      ResourceType resourceType,
      Microsoft.Azure.Documents.OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal cosmosContainerCore,
      FeedRange feedRange,
      Stream streamPayload,
      Action<RequestMessage> requestEnricher,
      Func<ResponseMessage, T> responseCreator,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      return this.RequestHandler.SendAsync<T>(resourceUri, resourceType, operationType, requestOptions, cosmosContainerCore, feedRange, streamPayload, requestEnricher, responseCreator, trace, cancellationToken);
    }

    internal override async Task<ContainerProperties> GetCachedContainerPropertiesAsync(
      string containerUri,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ClientContextCore clientContextCore = this;
      ContainerProperties containerPropertiesAsync;
      using (ITrace childTrace = trace.StartChild("Get Container Properties", TraceComponent.Transport, TraceLevel.Info))
      {
        clientContextCore.ThrowIfDisposed();
        ClientCollectionCache collectionCacheAsync = await clientContextCore.DocumentClient.GetCollectionCacheAsync(childTrace);
        try
        {
          containerPropertiesAsync = await collectionCacheAsync.ResolveByNameAsync(HttpConstants.Versions.CurrentVersion, containerUri, false, childTrace, (IClientSideRequestStatistics) null, cancellationToken);
        }
        catch (DocumentClientException ex)
        {
          ITrace trace1 = childTrace;
          throw CosmosExceptionFactory.Create(ex, trace1);
        }
      }
      return containerPropertiesAsync;
    }

    internal override BatchAsyncContainerExecutor GetExecutorForContainer(
      ContainerInternal container)
    {
      this.ThrowIfDisposed();
      return !this.ClientOptions.AllowBulkExecution ? (BatchAsyncContainerExecutor) null : this.batchExecutorCache.GetExecutorForContainer(container, (CosmosClientContext) this);
    }

    public override void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (disposing)
      {
        this.telemetry?.Dispose();
        this.batchExecutorCache.Dispose();
        this.DocumentClient.Dispose();
      }
      this.isDisposed = true;
    }

    private async Task<TResult> RunWithDiagnosticsHelperAsync<TResult>(
      ITrace trace,
      Func<ITrace, Task<TResult>> task,
      Func<TResult, OpenTelemetryAttributes> openTelemetry,
      string operationName,
      RequestOptions requestOptions)
    {
      ClientContextCore clientContextCore = this;
      TResult result1;
      using (OpenTelemetryCoreRecorder recorder = OpenTelemetryRecorderFactory.CreateRecorder(operationName, requestOptions, clientContextCore.isDisposed ? (CosmosClientContext) null : (CosmosClientContext) clientContextCore))
      {
        using (new ActivityScope(Guid.NewGuid()))
        {
          try
          {
            recorder.Record("db.operation", operationName);
            TResult result2 = await task(trace).ConfigureAwait(false);
            if (openTelemetry != null && recorder.IsEnabled)
              recorder.Record(openTelemetry(result2));
            result1 = result2;
          }
          catch (OperationCanceledException ex) when (!(ex is CosmosOperationCanceledException))
          {
            CosmosOperationCanceledException canceledException = new CosmosOperationCanceledException(ex, trace);
            recorder.MarkFailed((Exception) canceledException);
            throw canceledException;
          }
          catch (ObjectDisposedException ex) when (!(ex is CosmosObjectDisposedException))
          {
            CosmosObjectDisposedException disposedException = new CosmosObjectDisposedException(ex, clientContextCore.client, trace);
            recorder.MarkFailed((Exception) disposedException);
            throw disposedException;
          }
          catch (NullReferenceException ex) when (!(ex is CosmosNullReferenceException))
          {
            CosmosNullReferenceException referenceException = new CosmosNullReferenceException(ex, trace);
            recorder.MarkFailed((Exception) referenceException);
            throw referenceException;
          }
          catch (Exception ex)
          {
            recorder.MarkFailed(ex);
            throw;
          }
        }
      }
      return result1;
    }

    private async Task<ResponseMessage> ProcessResourceOperationAsBulkStreamAsync(
      Microsoft.Azure.Documents.OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal cosmosContainerCore,
      PartitionKey partitionKey,
      string itemId,
      Stream streamPayload,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ClientContextCore clientContextCore = this;
      clientContextCore.ThrowIfDisposed();
      ItemRequestOptions itemRequestOptions = requestOptions as ItemRequestOptions;
      return (await cosmosContainerCore.BatchExecutor.AddAsync(new ItemBatchOperation(operationType, 0, partitionKey, itemId, streamPayload, TransactionalBatchItemRequestOptions.FromItemRequestOptions(itemRequestOptions), (CosmosClientContext) clientContextCore), trace, itemRequestOptions, cancellationToken)).ToResponseMessage(cosmosContainerCore);
    }

    private bool IsBulkOperationSupported(ResourceType resourceType, Microsoft.Azure.Documents.OperationType operationType)
    {
      this.ThrowIfDisposed();
      if (!this.ClientOptions.AllowBulkExecution || resourceType != ResourceType.Document)
        return false;
      return operationType == Microsoft.Azure.Documents.OperationType.Create || operationType == Microsoft.Azure.Documents.OperationType.Upsert || operationType == Microsoft.Azure.Documents.OperationType.Read || operationType == Microsoft.Azure.Documents.OperationType.Delete || operationType == Microsoft.Azure.Documents.OperationType.Replace || operationType == Microsoft.Azure.Documents.OperationType.Patch;
    }

    private static CosmosClientOptions CreateOrCloneClientOptions(CosmosClientOptions clientOptions) => clientOptions == null ? new CosmosClientOptions() : clientOptions.Clone();

    internal T ThrowIfDisposed<T>(T input)
    {
      this.ThrowIfDisposed();
      return input;
    }

    private void ThrowIfDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException("Accessing CosmosClient after it is disposed is invalid.");
    }
  }
}
