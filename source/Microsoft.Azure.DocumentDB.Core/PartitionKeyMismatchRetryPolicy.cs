// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKeyMismatchRetryPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Common;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class PartitionKeyMismatchRetryPolicy : IDocumentClientRetryPolicy, IRetryPolicy
  {
    private readonly CollectionCache clientCollectionCache;
    private readonly IDocumentClientRetryPolicy nextRetryPolicy;
    private const int MaxRetries = 1;
    private int retriesAttempted;

    public PartitionKeyMismatchRetryPolicy(
      CollectionCache clientCollectionCache,
      IDocumentClientRetryPolicy nextRetryPolicy)
    {
      if (clientCollectionCache == null)
        throw new ArgumentNullException(nameof (clientCollectionCache));
      if (nextRetryPolicy == null)
        throw new ArgumentNullException(nameof (nextRetryPolicy));
      this.clientCollectionCache = clientCollectionCache;
      this.nextRetryPolicy = nextRetryPolicy;
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult result = this.ShouldRetryInternal(exception is DocumentClientException documentClientException ? documentClientException.StatusCode : new HttpStatusCode?(), documentClientException?.GetSubStatus(), documentClientException?.ResourceAddress);
      return result != null ? Task.FromResult<ShouldRetryResult>(result) : this.nextRetryPolicy.ShouldRetryAsync(exception, cancellationToken);
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
            this.clientCollectionCache.Refresh(resourceIdOrFullName);
          ++this.retriesAttempted;
          return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
        }
      }
      return (ShouldRetryResult) null;
    }
  }
}
