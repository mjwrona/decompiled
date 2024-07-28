// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PartitionKeyRangeGoneRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class PartitionKeyRangeGoneRetryPolicy : IDocumentClientRetryPolicy, IRetryPolicy
  {
    private readonly CollectionCache collectionCache;
    private readonly IDocumentClientRetryPolicy nextRetryPolicy;
    private readonly PartitionKeyRangeCache partitionKeyRangeCache;
    private readonly string collectionLink;
    private readonly ITrace trace;
    private bool retried;

    public PartitionKeyRangeGoneRetryPolicy(
      CollectionCache collectionCache,
      PartitionKeyRangeCache partitionKeyRangeCache,
      string collectionLink,
      IDocumentClientRetryPolicy nextRetryPolicy)
      : this(collectionCache, partitionKeyRangeCache, collectionLink, nextRetryPolicy, (ITrace) NoOpTrace.Singleton)
    {
    }

    public PartitionKeyRangeGoneRetryPolicy(
      CollectionCache collectionCache,
      PartitionKeyRangeCache partitionKeyRangeCache,
      string collectionLink,
      IDocumentClientRetryPolicy nextRetryPolicy,
      ITrace trace)
    {
      this.collectionCache = collectionCache;
      this.partitionKeyRangeCache = partitionKeyRangeCache;
      this.collectionLink = collectionLink;
      this.nextRetryPolicy = nextRetryPolicy;
      this.trace = trace;
    }

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      DocumentClientException documentClientException = exception as DocumentClientException;
      ShouldRetryResult shouldRetryResult1 = await this.ShouldRetryInternalAsync((HttpStatusCode?) documentClientException?.StatusCode, documentClientException?.GetSubStatus(), cancellationToken);
      if (shouldRetryResult1 != null)
        return shouldRetryResult1;
      ShouldRetryResult shouldRetryResult2;
      if (this.nextRetryPolicy != null)
        shouldRetryResult2 = await this.nextRetryPolicy?.ShouldRetryAsync(exception, cancellationToken);
      else
        shouldRetryResult2 = ShouldRetryResult.NoRetry();
      return shouldRetryResult2;
    }

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      ResponseMessage cosmosResponseMessage,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult shouldRetryResult1 = await this.ShouldRetryInternalAsync(cosmosResponseMessage?.StatusCode, cosmosResponseMessage?.Headers.SubStatusCode, cancellationToken);
      if (shouldRetryResult1 != null)
        return shouldRetryResult1;
      ShouldRetryResult shouldRetryResult2;
      if (this.nextRetryPolicy != null)
        shouldRetryResult2 = await this.nextRetryPolicy?.ShouldRetryAsync(cosmosResponseMessage, cancellationToken);
      else
        shouldRetryResult2 = ShouldRetryResult.NoRetry();
      return shouldRetryResult2;
    }

    public void OnBeforeSendRequest(DocumentServiceRequest request) => this.nextRetryPolicy?.OnBeforeSendRequest(request);

    private async Task<ShouldRetryResult> ShouldRetryInternalAsync(
      HttpStatusCode? statusCode,
      SubStatusCodes? subStatusCode,
      CancellationToken cancellationToken)
    {
      if (!statusCode.HasValue && (!subStatusCode.HasValue || subStatusCode.Value == SubStatusCodes.Unknown))
        return (ShouldRetryResult) null;
      HttpStatusCode? nullable1 = statusCode;
      HttpStatusCode httpStatusCode = HttpStatusCode.Gone;
      if (nullable1.GetValueOrDefault() == httpStatusCode & nullable1.HasValue)
      {
        SubStatusCodes? nullable2 = subStatusCode;
        SubStatusCodes subStatusCodes = SubStatusCodes.PartitionKeyRangeGone;
        if (nullable2.GetValueOrDefault() == subStatusCodes & nullable2.HasValue)
        {
          if (this.retried)
            return ShouldRetryResult.NoRetry();
          using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Collection, this.collectionLink, (Stream) null, AuthorizationTokenType.PrimaryMasterKey))
          {
            ContainerProperties collection = await this.collectionCache.ResolveCollectionAsync(request, cancellationToken, this.trace);
            CollectionRoutingMap previousValue = await this.partitionKeyRangeCache.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, request, this.trace);
            if (previousValue != null)
            {
              CollectionRoutingMap collectionRoutingMap = await this.partitionKeyRangeCache.TryLookupAsync(collection.ResourceId, previousValue, request, this.trace);
            }
            collection = (ContainerProperties) null;
          }
          this.retried = true;
          return ShouldRetryResult.RetryAfter(TimeSpan.FromSeconds(0.0));
        }
      }
      return (ShouldRetryResult) null;
    }
  }
}
