// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BatchExecutor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class BatchExecutor
  {
    private readonly ContainerInternal container;
    private readonly CosmosClientContext clientContext;
    private readonly IReadOnlyList<ItemBatchOperation> inputOperations;
    private readonly PartitionKey partitionKey;
    private readonly RequestOptions batchOptions;

    public BatchExecutor(
      ContainerInternal container,
      PartitionKey partitionKey,
      IReadOnlyList<ItemBatchOperation> operations,
      RequestOptions batchOptions)
    {
      this.container = container;
      this.clientContext = this.container.ClientContext;
      this.inputOperations = operations;
      this.partitionKey = partitionKey;
      this.batchOptions = batchOptions;
    }

    public async Task<TransactionalBatchResponse> ExecuteAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      TransactionalBatchResponse transactionalBatchResponse;
      using (ITrace executeNextBatchTrace = trace.StartChild("Execute Next Batch", TraceComponent.Batch, TraceLevel.Info))
      {
        BatchExecUtils.EnsureValid(this.inputOperations, this.batchOptions);
        PartitionKey? partitionKey = new PartitionKey?(this.partitionKey);
        if (this.batchOptions != null && this.batchOptions.IsEffectivePartitionKeyRouting)
          partitionKey = new PartitionKey?();
        transactionalBatchResponse = await this.ExecuteServerRequestAsync(await SinglePartitionKeyServerBatchRequest.CreateAsync(partitionKey, new ArraySegment<ItemBatchOperation>(this.inputOperations.ToArray<ItemBatchOperation>()), this.clientContext.SerializerCore, executeNextBatchTrace, cancellationToken), executeNextBatchTrace, cancellationToken);
      }
      return transactionalBatchResponse;
    }

    private async Task<TransactionalBatchResponse> ExecuteServerRequestAsync(
      SinglePartitionKeyServerBatchRequest serverRequest,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      TransactionalBatchResponse transactionalBatchResponse;
      using (ITrace executeBatchTrace = trace.StartChild("Execute Batch Request", TraceComponent.Batch, TraceLevel.Info))
      {
        using (Stream serverRequestPayload = (Stream) serverRequest.TransferBodyStream())
          transactionalBatchResponse = await TransactionalBatchResponse.FromResponseMessageAsync(await this.clientContext.ProcessResourceOperationStreamAsync(this.container.LinkUri, ResourceType.Document, OperationType.Batch, this.batchOptions, this.container, serverRequest.PartitionKey.HasValue ? (FeedRange) new FeedRangePartitionKey(serverRequest.PartitionKey.Value) : (FeedRange) null, serverRequestPayload, (Action<RequestMessage>) (requestMessage =>
          {
            requestMessage.Headers.Add("x-ms-cosmos-is-batch-request", bool.TrueString);
            requestMessage.Headers.Add("x-ms-cosmos-batch-atomic", bool.TrueString);
            requestMessage.Headers.Add("x-ms-cosmos-batch-ordered", bool.TrueString);
          }), executeBatchTrace, cancellationToken), (ServerBatchRequest) serverRequest, this.clientContext.SerializerCore, true, executeBatchTrace, cancellationToken);
      }
      return transactionalBatchResponse;
    }
  }
}
