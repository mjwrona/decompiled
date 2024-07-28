// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BatchAsyncBatcher
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class BatchAsyncBatcher
  {
    private readonly CosmosSerializerCore serializerCore;
    private readonly List<ItemBatchOperation> batchOperations;
    private readonly BatchAsyncBatcherExecuteDelegate executor;
    private readonly BatchAsyncBatcherRetryDelegate retrier;
    private readonly int maxBatchByteSize;
    private readonly int maxBatchOperationCount;
    private readonly InterlockIncrementCheck interlockIncrementCheck = new InterlockIncrementCheck();
    private readonly CosmosClientContext clientContext;
    private long currentSize;
    private bool dispatched;
    private bool isClientEncrypted;
    private string intendedCollectionRidValue;

    public bool IsEmpty => this.batchOperations.Count == 0;

    public BatchAsyncBatcher(
      int maxBatchOperationCount,
      int maxBatchByteSize,
      CosmosSerializerCore serializerCore,
      BatchAsyncBatcherExecuteDelegate executor,
      BatchAsyncBatcherRetryDelegate retrier,
      CosmosClientContext clientContext)
    {
      if (maxBatchOperationCount < 1)
        throw new ArgumentOutOfRangeException(nameof (maxBatchOperationCount));
      if (maxBatchByteSize < 1)
        throw new ArgumentOutOfRangeException(nameof (maxBatchByteSize));
      this.batchOperations = new List<ItemBatchOperation>(maxBatchOperationCount);
      this.executor = executor ?? throw new ArgumentNullException(nameof (executor));
      this.retrier = retrier ?? throw new ArgumentNullException(nameof (retrier));
      this.maxBatchByteSize = maxBatchByteSize;
      this.maxBatchOperationCount = maxBatchOperationCount;
      this.serializerCore = serializerCore ?? throw new ArgumentNullException(nameof (serializerCore));
      this.clientContext = clientContext;
    }

    public virtual bool TryAdd(ItemBatchOperation operation)
    {
      if (this.dispatched)
      {
        DefaultTrace.TraceCritical("Add operation attempted on dispatched batch.");
        return false;
      }
      if (operation == null)
        throw new ArgumentNullException(nameof (operation));
      if (operation.Context == null)
        throw new ArgumentNullException("Context");
      if (operation.Context.IsClientEncrypted && !this.isClientEncrypted)
      {
        this.isClientEncrypted = true;
        this.intendedCollectionRidValue = operation.Context.IntendedCollectionRidValue;
      }
      if (this.batchOperations.Count == this.maxBatchOperationCount)
      {
        DefaultTrace.TraceInformation(string.Format("Batch is full - Max operation count {0} reached.", (object) this.maxBatchOperationCount));
        return false;
      }
      int serializedLength = operation.GetApproximateSerializedLength();
      if (this.batchOperations.Count > 0 && (long) serializedLength + this.currentSize > (long) this.maxBatchByteSize)
      {
        DefaultTrace.TraceInformation(string.Format("Batch is full - Max byte size {0} reached.", (object) this.maxBatchByteSize));
        return false;
      }
      this.currentSize += (long) serializedLength;
      operation.OperationIndex = this.batchOperations.Count;
      operation.Context.CurrentBatcher = this;
      this.batchOperations.Add(operation);
      return true;
    }

    public virtual async Task DispatchAsync(
      BatchPartitionMetric partitionMetric,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (ITrace trace = (ITrace) Microsoft.Azure.Cosmos.Tracing.Trace.GetRootTrace("Batch Dispatch Async", TraceComponent.Batch, TraceLevel.Info))
      {
        object obj = await this.DispatchHelperAsync(trace, partitionMetric, cancellationToken);
      }
    }

    private async Task<object> DispatchHelperAsync(
      ITrace trace,
      BatchPartitionMetric partitionMetric,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BatchAsyncBatcher completer = this;
      completer.interlockIncrementCheck.EnterLockCheck();
      PartitionKeyRangeServerBatchRequest serverRequest = (PartitionKeyRangeServerBatchRequest) null;
      try
      {
        try
        {
          Tuple<PartitionKeyRangeServerBatchRequest, ArraySegment<ItemBatchOperation>> serverRequestAsync = await completer.CreateServerRequestAsync(cancellationToken);
          serverRequest = serverRequestAsync.Item1;
          foreach (ItemBatchOperation operation in serverRequestAsync.Item2)
            await completer.retrier(operation, cancellationToken);
        }
        catch (Exception ex)
        {
          foreach (ItemBatchOperation batchOperation in completer.batchOperations)
            batchOperation.Context.Fail(completer, ex);
          throw;
        }
        try
        {
          ValueStopwatch stopwatch = ValueStopwatch.StartNew();
          PartitionKeyRangeBatchExecutionResult batchExecutionResult = await completer.executor(serverRequest, trace, cancellationToken);
          int numberOfThrottles = batchExecutionResult.ServerResponse.Any<TransactionalBatchOperationResult>((Func<TransactionalBatchOperationResult, bool>) (r => r.StatusCode == (HttpStatusCode) 429)) ? 1 : 0;
          partitionMetric.Add((long) batchExecutionResult.ServerResponse.Count, stopwatch.ElapsedMilliseconds, (long) numberOfThrottles);
          using (PartitionKeyRangeBatchResponse batchResponse = new PartitionKeyRangeBatchResponse(serverRequest.Operations.Count, batchExecutionResult.ServerResponse, completer.serializerCore))
          {
            foreach (ItemBatchOperation itemBatchOperation in (IEnumerable<ItemBatchOperation>) batchResponse.Operations)
            {
              TransactionalBatchOperationResult response = batchResponse[itemBatchOperation.OperationIndex];
              if (!response.IsSuccessStatusCode)
              {
                if ((await itemBatchOperation.Context.ShouldRetryAsync(response, cancellationToken)).ShouldRetry)
                {
                  await completer.retrier(itemBatchOperation, cancellationToken);
                  continue;
                }
              }
              itemBatchOperation.Context.Complete(completer, response);
              response = (TransactionalBatchOperationResult) null;
            }
          }
          stopwatch = new ValueStopwatch();
        }
        catch (Exception ex)
        {
          foreach (ItemBatchOperation operation in (IEnumerable<ItemBatchOperation>) serverRequest.Operations)
            operation.Context.Fail(completer, ex);
          throw;
        }
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError("Exception during BatchAsyncBatcher: {0}", (object) ex);
      }
      finally
      {
        completer.batchOperations.Clear();
        completer.dispatched = true;
      }
      object obj = (object) null;
      serverRequest = (PartitionKeyRangeServerBatchRequest) null;
      return obj;
    }

    internal virtual async Task<Tuple<PartitionKeyRangeServerBatchRequest, ArraySegment<ItemBatchOperation>>> CreateServerRequestAsync(
      CancellationToken cancellationToken)
    {
      return await PartitionKeyRangeServerBatchRequest.CreateAsync(this.batchOperations[0].Context.PartitionKeyRangeId, new ArraySegment<ItemBatchOperation>(this.batchOperations.ToArray()), this.maxBatchByteSize, this.maxBatchOperationCount, false, this.serializerCore, this.isClientEncrypted, this.intendedCollectionRidValue, cancellationToken).ConfigureAwait(false);
    }
  }
}
