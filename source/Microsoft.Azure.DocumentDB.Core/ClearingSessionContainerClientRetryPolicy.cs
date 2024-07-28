// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ClearingSessionContainerClientRetryPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
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
