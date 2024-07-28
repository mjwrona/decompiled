// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BulkExecutionRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class BulkExecutionRetryPolicy : IDocumentClientRetryPolicy, IRetryPolicy
  {
    private const int SubstatusCodeBatchResponseSizeExceeded = 3402;
    private const int MaxRetryOn410 = 10;
    private readonly IDocumentClientRetryPolicy nextRetryPolicy;
    private readonly Microsoft.Azure.Documents.OperationType operationType;
    private readonly ContainerInternal container;
    private int retriesOn410;

    public BulkExecutionRetryPolicy(
      ContainerInternal container,
      Microsoft.Azure.Documents.OperationType operationType,
      IDocumentClientRetryPolicy nextRetryPolicy)
    {
      this.container = container ?? throw new ArgumentNullException(nameof (container));
      this.operationType = operationType;
      this.nextRetryPolicy = nextRetryPolicy;
    }

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      if (exception is CosmosException cosmosException)
      {
        ShouldRetryResult shouldRetryResult = await this.ShouldRetryInternalAsync(new HttpStatusCode?(cosmosException.StatusCode), new SubStatusCodes?((SubStatusCodes) cosmosException.SubStatusCode), cancellationToken);
        if (shouldRetryResult != null)
          return shouldRetryResult;
        if (this.nextRetryPolicy == null)
          return ShouldRetryResult.NoRetry();
      }
      return await this.nextRetryPolicy.ShouldRetryAsync(exception, cancellationToken);
    }

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      ResponseMessage cosmosResponseMessage,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult shouldRetryResult = await this.ShouldRetryInternalAsync(cosmosResponseMessage?.StatusCode, cosmosResponseMessage?.Headers.SubStatusCode, cancellationToken);
      if (shouldRetryResult != null)
        return shouldRetryResult;
      return this.nextRetryPolicy == null ? ShouldRetryResult.NoRetry() : await this.nextRetryPolicy.ShouldRetryAsync(cosmosResponseMessage, cancellationToken);
    }

    public void OnBeforeSendRequest(DocumentServiceRequest request) => this.nextRetryPolicy.OnBeforeSendRequest(request);

    private async Task<ShouldRetryResult> ShouldRetryInternalAsync(
      HttpStatusCode? statusCode,
      SubStatusCodes? subStatusCode,
      CancellationToken cancellationToken)
    {
      HttpStatusCode? nullable1 = statusCode;
      HttpStatusCode httpStatusCode1 = HttpStatusCode.Gone;
      if (nullable1.GetValueOrDefault() == httpStatusCode1 & nullable1.HasValue)
      {
        ++this.retriesOn410;
        if (this.retriesOn410 > 10)
          return ShouldRetryResult.NoRetry();
        SubStatusCodes? nullable2 = subStatusCode;
        SubStatusCodes subStatusCodes1 = SubStatusCodes.PartitionKeyRangeGone;
        if (!(nullable2.GetValueOrDefault() == subStatusCodes1 & nullable2.HasValue))
        {
          nullable2 = subStatusCode;
          SubStatusCodes subStatusCodes2 = SubStatusCodes.CompletingSplit;
          if (!(nullable2.GetValueOrDefault() == subStatusCodes2 & nullable2.HasValue))
          {
            nullable2 = subStatusCode;
            SubStatusCodes subStatusCodes3 = SubStatusCodes.CompletingPartitionMigration;
            if (!(nullable2.GetValueOrDefault() == subStatusCodes3 & nullable2.HasValue))
            {
              nullable2 = subStatusCode;
              SubStatusCodes subStatusCodes4 = SubStatusCodes.NameCacheIsStale;
              if (nullable2.GetValueOrDefault() == subStatusCodes4 & nullable2.HasValue)
                return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
              goto label_12;
            }
          }
        }
        PartitionKeyRangeCache partitionKeyRangeCache = await this.container.ClientContext.DocumentClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton);
        IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await partitionKeyRangeCache.TryGetOverlappingRangesAsync(await this.container.GetCachedRIDAsync(false, (ITrace) NoOpTrace.Singleton, cancellationToken), FeedRangeEpk.FullRange.Range, (ITrace) NoOpTrace.Singleton, true);
        return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
      }
label_12:
      nullable1 = statusCode;
      HttpStatusCode httpStatusCode2 = HttpStatusCode.RequestEntityTooLarge;
      return !(nullable1.GetValueOrDefault() == httpStatusCode2 & nullable1.HasValue) || subStatusCode.Value != (SubStatusCodes) 3402 ? (ShouldRetryResult) null : ShouldRetryResult.RetryAfter(TimeSpan.Zero);
    }
  }
}
