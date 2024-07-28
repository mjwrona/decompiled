// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RenameCollectionAwareClientRetryPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class RenameCollectionAwareClientRetryPolicy : 
    IDocumentClientRetryPolicy,
    IRetryPolicy
  {
    private readonly IDocumentClientRetryPolicy retryPolicy;
    private readonly ISessionContainer sessionContainer;
    private readonly ClientCollectionCache collectionCache;
    private DocumentServiceRequest request;
    private bool hasTriggered;

    public RenameCollectionAwareClientRetryPolicy(
      ISessionContainer sessionContainer,
      ClientCollectionCache collectionCache,
      IDocumentClientRetryPolicy retryPolicy)
    {
      this.retryPolicy = retryPolicy;
      this.sessionContainer = sessionContainer;
      this.collectionCache = collectionCache;
      this.request = (DocumentServiceRequest) null;
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
      return await this.ShouldRetryInternalAsync((HttpStatusCode?) documentClientException?.StatusCode, documentClientException?.GetSubStatus(), shouldRetryResult, cancellationToken);
    }

    private async Task<ShouldRetryResult> ShouldRetryInternalAsync(
      HttpStatusCode? statusCode,
      SubStatusCodes? subStatusCode,
      ShouldRetryResult shouldRetryResult,
      CancellationToken cancellationToken)
    {
      if (this.request == null)
      {
        DefaultTrace.TraceWarning("Cannot apply RenameCollectionAwareClientRetryPolicy as OnBeforeSendRequest has not been called and there is no DocumentServiceRequest context.");
        return shouldRetryResult;
      }
      if (!shouldRetryResult.ShouldRetry && !this.hasTriggered && statusCode.HasValue && subStatusCode.HasValue && this.request.IsNameBased && statusCode.Value == HttpStatusCode.NotFound && subStatusCode.Value == SubStatusCodes.PartitionKeyRangeGone)
      {
        DefaultTrace.TraceWarning("Clear the the token for named base request {0}", (object) this.request.ResourceAddress);
        this.sessionContainer.ClearTokenByCollectionFullname(this.request.ResourceAddress);
        this.hasTriggered = true;
        string oldCollectionRid = this.request.RequestContext.ResolvedCollectionRid;
        this.request.ForceNameCacheRefresh = true;
        this.request.RequestContext.ResolvedCollectionRid = (string) null;
        try
        {
          DocumentCollection documentCollection = await this.collectionCache.ResolveCollectionAsync(this.request, cancellationToken);
          if (documentCollection == null)
            DefaultTrace.TraceCritical("Can't recover from session unavailable exception because resolving collection name {0} returned null", (object) this.request.ResourceAddress);
          else if (!string.IsNullOrEmpty(oldCollectionRid))
          {
            if (!string.IsNullOrEmpty(documentCollection.ResourceId))
            {
              if (!oldCollectionRid.Equals(documentCollection.ResourceId))
                return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
            }
          }
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceCritical("Can't recover from session unavailable exception because resolving collection name {0} failed with {1}", (object) this.request.ResourceAddress, (object) ex.ToString());
        }
        oldCollectionRid = (string) null;
      }
      return shouldRetryResult;
    }
  }
}
