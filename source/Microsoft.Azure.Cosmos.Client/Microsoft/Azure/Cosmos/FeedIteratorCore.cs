// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedIteratorCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class FeedIteratorCore : FeedIteratorInternal
  {
    private readonly CosmosClientContext clientContext;
    private readonly string resourceLink;
    private readonly ResourceType resourceType;
    private readonly SqlQuerySpec querySpec;
    private bool hasMoreResultsInternal;

    public FeedIteratorCore(
      CosmosClientContext clientContext,
      string resourceLink,
      ResourceType resourceType,
      QueryDefinition queryDefinition,
      string continuationToken,
      QueryRequestOptions options,
      ContainerInternal container,
      string databaseId = null)
    {
      this.resourceLink = resourceLink;
      this.clientContext = clientContext;
      this.resourceType = resourceType;
      this.querySpec = queryDefinition?.ToSqlQuerySpec();
      this.ContinuationToken = continuationToken;
      this.requestOptions = options;
      this.hasMoreResultsInternal = true;
      this.databaseName = databaseId;
      this.container = container;
    }

    public override bool HasMoreResults => this.hasMoreResultsInternal;

    public QueryRequestOptions requestOptions { get; }

    public string ContinuationToken { get; set; }

    public override Task<ResponseMessage> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ReadNextAsync((ITrace) NoOpTrace.Singleton, new CancellationToken());

    public override async Task<ResponseMessage> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedIteratorCore feedIteratorCore = this;
      cancellationToken.ThrowIfCancellationRequested();
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      OperationType operationType = OperationType.ReadFeed;
      Stream stream;
      if (feedIteratorCore.querySpec != null)
      {
        using (trace.StartChild("QuerySpec to Stream", TraceComponent.Poco, TraceLevel.Info))
          stream = feedIteratorCore.clientContext.SerializerCore.ToStreamSqlQuerySpec(feedIteratorCore.querySpec, feedIteratorCore.resourceType);
        operationType = OperationType.Query;
      }
      else
        stream = (Stream) null;
      CosmosClientContext clientContext = feedIteratorCore.clientContext;
      string resourceLink = feedIteratorCore.resourceLink;
      int resourceType = (int) feedIteratorCore.resourceType;
      int num1 = (int) operationType;
      QueryRequestOptions requestOptions1 = feedIteratorCore.requestOptions;
      QueryRequestOptions requestOptions2 = feedIteratorCore.requestOptions;
      PartitionKey? partitionKey;
      int num2;
      if (requestOptions2 == null)
      {
        num2 = 0;
      }
      else
      {
        partitionKey = requestOptions2.PartitionKey;
        num2 = partitionKey.HasValue ? 1 : 0;
      }
      FeedRangePartitionKey rangePartitionKey;
      if (num2 == 0)
      {
        rangePartitionKey = (FeedRangePartitionKey) null;
      }
      else
      {
        partitionKey = feedIteratorCore.requestOptions.PartitionKey;
        rangePartitionKey = new FeedRangePartitionKey(partitionKey.Value);
      }
      Stream streamPayload = stream;
      // ISSUE: reference to a compiler-generated method
      Action<RequestMessage> requestEnricher = new Action<RequestMessage>(feedIteratorCore.\u003CReadNextAsync\u003Eb__16_0);
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      ResponseMessage responseMessage = await clientContext.ProcessResourceOperationStreamAsync(resourceLink, (ResourceType) resourceType, (OperationType) num1, (RequestOptions) requestOptions1, (ContainerInternal) null, (FeedRange) rangePartitionKey, streamPayload, requestEnricher, trace1, cancellationToken1);
      feedIteratorCore.ContinuationToken = responseMessage.Headers.ContinuationToken;
      feedIteratorCore.hasMoreResultsInternal = feedIteratorCore.ContinuationToken != null && responseMessage.StatusCode != HttpStatusCode.NotModified;
      if (responseMessage.Content != null)
        await FeedIteratorCore.RewriteStreamAsTextAsync(responseMessage, feedIteratorCore.requestOptions, trace);
      ResponseMessage responseMessage1 = responseMessage;
      responseMessage = (ResponseMessage) null;
      return responseMessage1;
    }

    public override CosmosElement GetCosmosElementContinuationToken() => throw new NotImplementedException();

    private static async Task RewriteStreamAsTextAsync(
      ResponseMessage responseMessage,
      QueryRequestOptions requestOptions,
      ITrace trace)
    {
      using (trace.StartChild("Rewrite Stream as Text", TraceComponent.Json, TraceLevel.Info))
      {
        MemoryStream memoryStream;
        if (responseMessage.Content is MemoryStream content)
        {
          memoryStream = content;
        }
        else
        {
          memoryStream = new MemoryStream();
          await responseMessage.Content.CopyToAsync((Stream) memoryStream);
        }
        ArraySegment<byte> buffer;
        IJsonNavigator jsonNavigator = JsonNavigator.Create(!memoryStream.TryGetBuffer(out buffer) ? (ReadOnlyMemory<byte>) memoryStream.ToArray() : (ReadOnlyMemory<byte>) buffer.Array.AsMemory<byte>().Slice(buffer.Offset, buffer.Count));
        if (jsonNavigator.SerializationFormat == JsonSerializationFormat.Text)
          return;
        IJsonWriter jsonWriter = requestOptions?.CosmosSerializationFormatOptions == null ? JsonWriter.Create(JsonSerializationFormat.Text) : requestOptions.CosmosSerializationFormatOptions.CreateCustomWriterCallback();
        jsonNavigator.WriteNode(jsonNavigator.GetRootNode(), jsonWriter);
        ReadOnlyMemory<byte> result = jsonWriter.GetResult();
        ArraySegment<byte> segment;
        MemoryStream memoryStream1;
        if (MemoryMarshal.TryGetArray<byte>(result, out segment))
        {
          memoryStream1 = new MemoryStream(segment.Array, segment.Offset, segment.Count, false, true);
        }
        else
        {
          byte[] array = result.ToArray();
          memoryStream1 = new MemoryStream(array, 0, array.Length, false, true);
        }
        responseMessage.Content = (Stream) memoryStream1;
        memoryStream = (MemoryStream) null;
      }
    }
  }
}
