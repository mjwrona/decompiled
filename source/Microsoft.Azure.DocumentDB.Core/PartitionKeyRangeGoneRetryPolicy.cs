// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKeyRangeGoneRetryPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Common;
using Microsoft.Azure.Documents.Routing;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal class PartitionKeyRangeGoneRetryPolicy : IDocumentClientRetryPolicy, IRetryPolicy
  {
    private readonly CollectionCache collectionCache;
    private readonly IDocumentClientRetryPolicy nextRetryPolicy;
    private readonly PartitionKeyRangeCache partitionKeyRangeCache;
    private readonly string collectionLink;
    private bool retried;

    public PartitionKeyRangeGoneRetryPolicy(
      CollectionCache collectionCache,
      PartitionKeyRangeCache partitionKeyRangeCache,
      string collectionLink,
      IDocumentClientRetryPolicy nextRetryPolicy)
    {
      this.collectionCache = collectionCache;
      this.partitionKeyRangeCache = partitionKeyRangeCache;
      this.collectionLink = collectionLink;
      this.nextRetryPolicy = nextRetryPolicy;
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

    public void OnBeforeSendRequest(DocumentServiceRequest request) => this.nextRetryPolicy.OnBeforeSendRequest(request);

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
          using (DocumentServiceRequest request = DocumentServiceRequest.Create(OperationType.Read, ResourceType.Collection, this.collectionLink, (Stream) null, AuthorizationTokenType.PrimaryMasterKey))
          {
            DocumentCollection collection = await this.collectionCache.ResolveCollectionAsync(request, cancellationToken);
            CollectionRoutingMap previousValue = await this.partitionKeyRangeCache.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, request, cancellationToken);
            if (previousValue != null)
            {
              CollectionRoutingMap collectionRoutingMap = await this.partitionKeyRangeCache.TryLookupAsync(collection.ResourceId, previousValue, request, cancellationToken);
            }
            collection = (DocumentCollection) null;
          }
          this.retried = true;
          return ShouldRetryResult.RetryAfter(TimeSpan.FromSeconds(0.0));
        }
      }
      return (ShouldRetryResult) null;
    }
  }
}
