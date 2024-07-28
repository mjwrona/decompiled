// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ItemBatchOperationContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class ItemBatchOperationContext : IDisposable
  {
    private readonly IDocumentClientRetryPolicy retryPolicy;
    private readonly TaskCompletionSource<TransactionalBatchOperationResult> taskCompletionSource = new TaskCompletionSource<TransactionalBatchOperationResult>(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly ITrace initialTrace;

    public string PartitionKeyRangeId { get; private set; }

    public bool IsClientEncrypted { get; set; }

    public string IntendedCollectionRidValue { get; set; }

    public BatchAsyncBatcher CurrentBatcher { get; set; }

    public Task<TransactionalBatchOperationResult> OperationTask => this.taskCompletionSource.Task;

    public ItemBatchOperationContext(
      string partitionKeyRangeId,
      ITrace trace,
      IDocumentClientRetryPolicy retryPolicy = null)
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      this.PartitionKeyRangeId = partitionKeyRangeId ?? throw new ArgumentNullException(nameof (partitionKeyRangeId));
      this.initialTrace = trace;
      this.retryPolicy = retryPolicy;
    }

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      TransactionalBatchOperationResult batchOperationResult,
      CancellationToken cancellationToken)
    {
      if (this.retryPolicy == null || batchOperationResult.IsSuccessStatusCode)
        return ShouldRetryResult.NoRetry();
      ShouldRetryResult shouldRetryResult = await this.retryPolicy.ShouldRetryAsync(batchOperationResult.ToResponseMessage(), cancellationToken);
      if (shouldRetryResult.ShouldRetry)
        this.initialTrace.AddChild(batchOperationResult.Trace);
      return shouldRetryResult;
    }

    public void Complete(BatchAsyncBatcher completer, TransactionalBatchOperationResult result)
    {
      if (this.AssertBatcher(completer))
      {
        this.initialTrace.AddChild(result.Trace);
        result.Trace = this.initialTrace;
        this.taskCompletionSource.SetResult(result);
      }
      this.Dispose();
    }

    public void Fail(BatchAsyncBatcher completer, Exception exception)
    {
      if (this.AssertBatcher(completer, exception))
        this.taskCompletionSource.SetException(exception);
      this.Dispose();
    }

    public void ReRouteOperation(string newPartitionKeyRangeId, ITrace trace)
    {
      this.PartitionKeyRangeId = newPartitionKeyRangeId;
      this.initialTrace.AddChild(trace);
    }

    public void Dispose() => this.CurrentBatcher = (BatchAsyncBatcher) null;

    private bool AssertBatcher(BatchAsyncBatcher completer, Exception innerException = null)
    {
      if (completer == this.CurrentBatcher)
        return true;
      DefaultTrace.TraceCritical("Operation was completed by incorrect batcher.");
      this.taskCompletionSource.SetException(new Exception("Operation was completed by incorrect batcher.", innerException));
      return false;
    }
  }
}
