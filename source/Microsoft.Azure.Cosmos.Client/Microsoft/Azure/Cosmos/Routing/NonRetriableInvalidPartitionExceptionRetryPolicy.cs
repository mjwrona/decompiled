// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.NonRetriableInvalidPartitionExceptionRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal class NonRetriableInvalidPartitionExceptionRetryPolicy : 
    IDocumentClientRetryPolicy,
    IRetryPolicy
  {
    private readonly CollectionCache clientCollectionCache;
    private readonly IDocumentClientRetryPolicy nextPolicy;

    public NonRetriableInvalidPartitionExceptionRetryPolicy(
      CollectionCache clientCollectionCache,
      IDocumentClientRetryPolicy nextPolicy)
    {
      if (clientCollectionCache == null)
        throw new ArgumentNullException(nameof (clientCollectionCache));
      if (nextPolicy == null)
        throw new ArgumentNullException(nameof (nextPolicy));
      this.clientCollectionCache = clientCollectionCache;
      this.nextPolicy = nextPolicy;
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult result = this.ShouldRetryInternal(exception is DocumentClientException documentClientException ? documentClientException.StatusCode : new HttpStatusCode?(), documentClientException?.GetSubStatus(), documentClientException?.ResourceAddress);
      return result != null ? Task.FromResult<ShouldRetryResult>(result) : this.nextPolicy.ShouldRetryAsync(exception, cancellationToken);
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      ResponseMessage cosmosResponseMessage,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public void OnBeforeSendRequest(DocumentServiceRequest request) => this.nextPolicy.OnBeforeSendRequest(request);

    private ShouldRetryResult ShouldRetryInternal(
      HttpStatusCode? statusCode,
      SubStatusCodes? subStatusCode,
      string resourceIdOrFullName)
    {
      if (!statusCode.HasValue && (!subStatusCode.HasValue || subStatusCode.Value == SubStatusCodes.Unknown))
        return (ShouldRetryResult) null;
      HttpStatusCode? nullable1 = statusCode;
      HttpStatusCode httpStatusCode = HttpStatusCode.Gone;
      if (nullable1.GetValueOrDefault() == httpStatusCode & nullable1.HasValue)
      {
        SubStatusCodes? nullable2 = subStatusCode;
        SubStatusCodes subStatusCodes = SubStatusCodes.NameCacheIsStale;
        if (nullable2.GetValueOrDefault() == subStatusCodes & nullable2.HasValue)
        {
          if (!string.IsNullOrEmpty(resourceIdOrFullName))
            this.clientCollectionCache.Refresh(resourceIdOrFullName);
          return ShouldRetryResult.NoRetry((Exception) new NotFoundException());
        }
      }
      return (ShouldRetryResult) null;
    }
  }
}
