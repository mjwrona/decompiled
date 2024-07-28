// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BatchCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Telemetry;
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
  internal class BatchCore : TransactionalBatchInternal
  {
    private readonly PartitionKey partitionKey;
    private readonly ContainerInternal container;
    private List<ItemBatchOperation> operations;

    internal BatchCore(ContainerInternal container, PartitionKey partitionKey)
    {
      this.container = container;
      this.partitionKey = partitionKey;
      this.operations = new List<ItemBatchOperation>();
    }

    public override TransactionalBatch CreateItem<T>(
      T item,
      TransactionalBatchItemRequestOptions requestOptions = null)
    {
      if ((object) item == null)
        throw new ArgumentNullException(nameof (item));
      List<ItemBatchOperation> operations = this.operations;
      int count = this.operations.Count;
      T resource = item;
      TransactionalBatchItemRequestOptions itemRequestOptions = requestOptions;
      ContainerInternal container = this.container;
      TransactionalBatchItemRequestOptions requestOptions1 = itemRequestOptions;
      ItemBatchOperation<T> itemBatchOperation = new ItemBatchOperation<T>(OperationType.Create, count, resource, container, requestOptions: requestOptions1);
      operations.Add((ItemBatchOperation) itemBatchOperation);
      return (TransactionalBatch) this;
    }

    public override TransactionalBatch CreateItemStream(
      Stream streamPayload,
      TransactionalBatchItemRequestOptions requestOptions = null)
    {
      if (streamPayload == null)
        throw new ArgumentNullException(nameof (streamPayload));
      List<ItemBatchOperation> operations = this.operations;
      int count = this.operations.Count;
      Stream stream = streamPayload;
      TransactionalBatchItemRequestOptions itemRequestOptions = requestOptions;
      ContainerInternal container = this.container;
      Stream resourceStream = stream;
      TransactionalBatchItemRequestOptions requestOptions1 = itemRequestOptions;
      ItemBatchOperation itemBatchOperation = new ItemBatchOperation(OperationType.Create, count, container, resourceStream: resourceStream, requestOptions: requestOptions1);
      operations.Add(itemBatchOperation);
      return (TransactionalBatch) this;
    }

    public override TransactionalBatch ReadItem(
      string id,
      TransactionalBatchItemRequestOptions requestOptions = null)
    {
      if (id == null)
        throw new ArgumentNullException(nameof (id));
      List<ItemBatchOperation> operations = this.operations;
      int count = this.operations.Count;
      string str = id;
      TransactionalBatchItemRequestOptions itemRequestOptions = requestOptions;
      ContainerInternal container = this.container;
      string id1 = str;
      TransactionalBatchItemRequestOptions requestOptions1 = itemRequestOptions;
      ItemBatchOperation itemBatchOperation = new ItemBatchOperation(OperationType.Read, count, container, id1, requestOptions: requestOptions1);
      operations.Add(itemBatchOperation);
      return (TransactionalBatch) this;
    }

    public override TransactionalBatch UpsertItem<T>(
      T item,
      TransactionalBatchItemRequestOptions requestOptions = null)
    {
      if ((object) item == null)
        throw new ArgumentNullException(nameof (item));
      List<ItemBatchOperation> operations = this.operations;
      int count = this.operations.Count;
      T resource = item;
      TransactionalBatchItemRequestOptions itemRequestOptions = requestOptions;
      ContainerInternal container = this.container;
      TransactionalBatchItemRequestOptions requestOptions1 = itemRequestOptions;
      ItemBatchOperation<T> itemBatchOperation = new ItemBatchOperation<T>(OperationType.Upsert, count, resource, container, requestOptions: requestOptions1);
      operations.Add((ItemBatchOperation) itemBatchOperation);
      return (TransactionalBatch) this;
    }

    public override TransactionalBatch UpsertItemStream(
      Stream streamPayload,
      TransactionalBatchItemRequestOptions requestOptions = null)
    {
      if (streamPayload == null)
        throw new ArgumentNullException(nameof (streamPayload));
      List<ItemBatchOperation> operations = this.operations;
      int count = this.operations.Count;
      Stream stream = streamPayload;
      TransactionalBatchItemRequestOptions itemRequestOptions = requestOptions;
      ContainerInternal container = this.container;
      Stream resourceStream = stream;
      TransactionalBatchItemRequestOptions requestOptions1 = itemRequestOptions;
      ItemBatchOperation itemBatchOperation = new ItemBatchOperation(OperationType.Upsert, count, container, resourceStream: resourceStream, requestOptions: requestOptions1);
      operations.Add(itemBatchOperation);
      return (TransactionalBatch) this;
    }

    public override TransactionalBatch ReplaceItem<T>(
      string id,
      T item,
      TransactionalBatchItemRequestOptions requestOptions = null)
    {
      if (id == null)
        throw new ArgumentNullException(nameof (id));
      if ((object) item == null)
        throw new ArgumentNullException(nameof (item));
      List<ItemBatchOperation> operations = this.operations;
      int count = this.operations.Count;
      string str = id;
      T resource = item;
      TransactionalBatchItemRequestOptions itemRequestOptions = requestOptions;
      ContainerInternal container = this.container;
      string id1 = str;
      TransactionalBatchItemRequestOptions requestOptions1 = itemRequestOptions;
      ItemBatchOperation<T> itemBatchOperation = new ItemBatchOperation<T>(OperationType.Replace, count, resource, container, id1, requestOptions1);
      operations.Add((ItemBatchOperation) itemBatchOperation);
      return (TransactionalBatch) this;
    }

    public override TransactionalBatch ReplaceItemStream(
      string id,
      Stream streamPayload,
      TransactionalBatchItemRequestOptions requestOptions = null)
    {
      if (id == null)
        throw new ArgumentNullException(nameof (id));
      if (streamPayload == null)
        throw new ArgumentNullException(nameof (streamPayload));
      List<ItemBatchOperation> operations = this.operations;
      int count = this.operations.Count;
      string str = id;
      Stream stream = streamPayload;
      TransactionalBatchItemRequestOptions itemRequestOptions = requestOptions;
      ContainerInternal container = this.container;
      string id1 = str;
      Stream resourceStream = stream;
      TransactionalBatchItemRequestOptions requestOptions1 = itemRequestOptions;
      ItemBatchOperation itemBatchOperation = new ItemBatchOperation(OperationType.Replace, count, container, id1, resourceStream, requestOptions1);
      operations.Add(itemBatchOperation);
      return (TransactionalBatch) this;
    }

    public override TransactionalBatch DeleteItem(
      string id,
      TransactionalBatchItemRequestOptions requestOptions = null)
    {
      if (id == null)
        throw new ArgumentNullException(nameof (id));
      List<ItemBatchOperation> operations = this.operations;
      int count = this.operations.Count;
      string str = id;
      TransactionalBatchItemRequestOptions itemRequestOptions = requestOptions;
      ContainerInternal container = this.container;
      string id1 = str;
      TransactionalBatchItemRequestOptions requestOptions1 = itemRequestOptions;
      ItemBatchOperation itemBatchOperation = new ItemBatchOperation(OperationType.Delete, count, container, id1, requestOptions: requestOptions1);
      operations.Add(itemBatchOperation);
      return (TransactionalBatch) this;
    }

    public override Task<TransactionalBatchResponse> ExecuteAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ExecuteAsync((TransactionalBatchRequestOptions) null, cancellationToken);
    }

    public override Task<TransactionalBatchResponse> ExecuteAsync(
      TransactionalBatchRequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.container.ClientContext.OperationHelperAsync<TransactionalBatchResponse>(nameof (ExecuteAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<TransactionalBatchResponse>>) (trace =>
      {
        BatchExecutor batchExecutor = new BatchExecutor(this.container, this.partitionKey, (IReadOnlyList<ItemBatchOperation>) this.operations, (RequestOptions) requestOptions);
        this.operations = new List<ItemBatchOperation>();
        ITrace trace1 = trace;
        CancellationToken cancellationToken1 = cancellationToken;
        return batchExecutor.ExecuteAsync(trace1, cancellationToken1);
      }), (Func<TransactionalBatchResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response, this.container?.Id, this.container?.Database?.Id)));
    }

    public virtual TransactionalBatch PatchItemStream(
      string id,
      Stream patchStream,
      TransactionalBatchPatchItemRequestOptions requestOptions = null)
    {
      List<ItemBatchOperation> operations = this.operations;
      int count = this.operations.Count;
      string str = id;
      Stream stream = patchStream;
      TransactionalBatchItemRequestOptions itemRequestOptions = (TransactionalBatchItemRequestOptions) requestOptions;
      ContainerInternal container = this.container;
      string id1 = str;
      Stream resourceStream = stream;
      TransactionalBatchItemRequestOptions requestOptions1 = itemRequestOptions;
      ItemBatchOperation itemBatchOperation = new ItemBatchOperation(OperationType.Patch, count, container, id1, resourceStream, requestOptions1);
      operations.Add(itemBatchOperation);
      return (TransactionalBatch) this;
    }

    public override TransactionalBatch PatchItem(
      string id,
      IReadOnlyList<PatchOperation> patchOperations,
      TransactionalBatchPatchItemRequestOptions requestOptions = null)
    {
      if (string.IsNullOrWhiteSpace(id))
        throw new ArgumentNullException(nameof (id));
      PatchSpec patchSpec = patchOperations != null && patchOperations.Any<PatchOperation>() ? new PatchSpec(patchOperations, (Either<PatchItemRequestOptions, TransactionalBatchPatchItemRequestOptions>) requestOptions) : throw new ArgumentNullException(nameof (patchOperations));
      List<ItemBatchOperation> operations = this.operations;
      int count = this.operations.Count;
      string str = id;
      PatchSpec resource = patchSpec;
      TransactionalBatchItemRequestOptions itemRequestOptions = (TransactionalBatchItemRequestOptions) requestOptions;
      ContainerInternal container = this.container;
      string id1 = str;
      TransactionalBatchItemRequestOptions requestOptions1 = itemRequestOptions;
      ItemBatchOperation<PatchSpec> itemBatchOperation = new ItemBatchOperation<PatchSpec>(OperationType.Patch, count, resource, container, id1, requestOptions1);
      operations.Add((ItemBatchOperation) itemBatchOperation);
      return (TransactionalBatch) this;
    }
  }
}
