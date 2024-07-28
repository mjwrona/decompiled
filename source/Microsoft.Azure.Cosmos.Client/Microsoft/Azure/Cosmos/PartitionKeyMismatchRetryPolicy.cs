// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PartitionKeyMismatchRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class PartitionKeyMismatchRetryPolicy : IDocumentClientRetryPolicy, IRetryPolicy
  {
    private const int MaxRetries = 1;
    private readonly CollectionCache clientCollectionCache;
    private readonly IDocumentClientRetryPolicy nextRetryPolicy;
    private int retriesAttempted;

    public PartitionKeyMismatchRetryPolicy(
      CollectionCache clientCollectionCache,
      IDocumentClientRetryPolicy nextRetryPolicy)
    {
      this.clientCollectionCache = clientCollectionCache != null ? clientCollectionCache : throw new ArgumentNullException(nameof (clientCollectionCache));
      this.nextRetryPolicy = nextRetryPolicy;
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult result = this.ShouldRetryInternal(exception is DocumentClientException documentClientException ? documentClientException.StatusCode : new HttpStatusCode?(), documentClientException?.GetSubStatus(), documentClientException?.ResourceAddress);
      if (result != null)
        return Task.FromResult<ShouldRetryResult>(result);
      return this.nextRetryPolicy == null ? Task.FromResult<ShouldRetryResult>(ShouldRetryResult.NoRetry()) : this.nextRetryPolicy.ShouldRetryAsync(exception, cancellationToken);
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      ResponseMessage cosmosResponseMessage,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult result = this.ShouldRetryInternal(cosmosResponseMessage?.StatusCode, cosmosResponseMessage?.Headers.SubStatusCode, cosmosResponseMessage?.GetResourceAddress());
      if (result != null)
        return Task.FromResult<ShouldRetryResult>(result);
      return this.nextRetryPolicy == null ? Task.FromResult<ShouldRetryResult>(ShouldRetryResult.NoRetry()) : this.nextRetryPolicy.ShouldRetryAsync(cosmosResponseMessage, cancellationToken);
    }

    public void OnBeforeSendRequest(DocumentServiceRequest request) => this.nextRetryPolicy.OnBeforeSendRequest(request);

    private ShouldRetryResult ShouldRetryInternal(
      HttpStatusCode? statusCode,
      SubStatusCodes? subStatusCode,
      string resourceIdOrFullName)
    {
      if (!statusCode.HasValue && (!subStatusCode.HasValue || subStatusCode.Value == SubStatusCodes.Unknown))
        return (ShouldRetryResult) null;
      HttpStatusCode? nullable1 = statusCode;
      HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
      if (nullable1.GetValueOrDefault() == httpStatusCode & nullable1.HasValue)
      {
        SubStatusCodes? nullable2 = subStatusCode;
        SubStatusCodes subStatusCodes = SubStatusCodes.PartitionKeyMismatch;
        if (nullable2.GetValueOrDefault() == subStatusCodes & nullable2.HasValue && this.retriesAttempted < 1)
        {
          if (!string.IsNullOrEmpty(resourceIdOrFullName))
            this.clientCollectionCache.Refresh(resourceIdOrFullName, HttpConstants.Versions.CurrentVersion);
          ++this.retriesAttempted;
          return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
        }
      }
      return (ShouldRetryResult) null;
    }
  }
}
