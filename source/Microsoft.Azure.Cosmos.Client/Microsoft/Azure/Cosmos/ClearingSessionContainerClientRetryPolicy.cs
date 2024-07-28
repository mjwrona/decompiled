// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ClearingSessionContainerClientRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ClearingSessionContainerClientRetryPolicy : 
    IDocumentClientRetryPolicy,
    IRetryPolicy
  {
    private readonly IDocumentClientRetryPolicy retryPolicy;
    private readonly ISessionContainer sessionContainer;
    private DocumentServiceRequest request;
    private bool hasTriggered;

    public ClearingSessionContainerClientRetryPolicy(
      ISessionContainer sessionContainer,
      IDocumentClientRetryPolicy retryPolicy)
    {
      this.retryPolicy = retryPolicy;
      this.sessionContainer = sessionContainer;
    }

    public void OnBeforeSendRequest(DocumentServiceRequest request)
    {
      this.request = request;
      this.retryPolicy.OnBeforeSendRequest(request);
    }

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult shouldRetryResult = await this.retryPolicy.ShouldRetryAsync(exception, cancellationToken);
      DocumentClientException documentClientException = exception as DocumentClientException;
      return this.ShouldRetryInternal((HttpStatusCode?) documentClientException?.StatusCode, documentClientException?.GetSubStatus(), shouldRetryResult);
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      ResponseMessage cosmosResponseMessage,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    private ShouldRetryResult ShouldRetryInternal(
      HttpStatusCode? statusCode,
      SubStatusCodes? subStatusCode,
      ShouldRetryResult shouldRetryResult)
    {
      if (this.request == null || shouldRetryResult.ShouldRetry || this.hasTriggered || !statusCode.HasValue || !subStatusCode.HasValue || !this.request.IsNameBased || statusCode.Value != HttpStatusCode.NotFound || subStatusCode.Value != SubStatusCodes.PartitionKeyRangeGone)
        return shouldRetryResult;
      DefaultTrace.TraceWarning("Clear the the token for named base request {0}", (object) this.request.ResourceAddress);
      this.sessionContainer.ClearTokenByCollectionFullname(this.request.ResourceAddress);
      this.hasTriggered = true;
      return shouldRetryResult;
    }
  }
}
