// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.InvalidPartitionExceptionRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal class InvalidPartitionExceptionRetryPolicy : IDocumentClientRetryPolicy, IRetryPolicy
  {
    private readonly IDocumentClientRetryPolicy nextPolicy;
    private DocumentServiceRequest documentServiceRequest;
    private bool retried;

    public InvalidPartitionExceptionRetryPolicy(IDocumentClientRetryPolicy nextPolicy) => this.nextPolicy = nextPolicy;

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      DocumentClientException documentClientException = exception as DocumentClientException;
      ShouldRetryResult shouldRetryResult1 = this.ShouldRetryInternal((HttpStatusCode?) documentClientException?.StatusCode, documentClientException?.GetSubStatus(), documentClientException?.ResourceAddress);
      if (shouldRetryResult1 != null)
        return shouldRetryResult1;
      ShouldRetryResult shouldRetryResult2;
      if (this.nextPolicy != null)
        shouldRetryResult2 = await this.nextPolicy.ShouldRetryAsync(exception, cancellationToken);
      else
        shouldRetryResult2 = ShouldRetryResult.NoRetry();
      return shouldRetryResult2;
    }

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      ResponseMessage httpResponseMessage,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult shouldRetryResult1 = this.ShouldRetryInternal(new HttpStatusCode?(httpResponseMessage.StatusCode), new SubStatusCodes?(httpResponseMessage.Headers.SubStatusCode), httpResponseMessage.GetResourceAddress());
      if (shouldRetryResult1 != null)
        return shouldRetryResult1;
      ShouldRetryResult shouldRetryResult2;
      if (this.nextPolicy != null)
        shouldRetryResult2 = await this.nextPolicy.ShouldRetryAsync(httpResponseMessage, cancellationToken);
      else
        shouldRetryResult2 = ShouldRetryResult.NoRetry();
      return shouldRetryResult2;
    }

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
          if (this.retried)
            return ShouldRetryResult.NoRetry();
          if (this.documentServiceRequest == null)
            throw new InvalidOperationException("OnBeforeSendRequest was never called");
          this.documentServiceRequest.ForceNameCacheRefresh = true;
          this.documentServiceRequest.ClearRoutingHints();
          this.retried = true;
          return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
        }
      }
      return (ShouldRetryResult) null;
    }

    public void OnBeforeSendRequest(DocumentServiceRequest request)
    {
      this.documentServiceRequest = request;
      this.nextPolicy?.OnBeforeSendRequest(request);
    }
  }
}
