// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.RenameCollectionAwareClientRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
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

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      ResponseMessage cosmosResponseMessage,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult shouldRetryResult = await this.retryPolicy.ShouldRetryAsync(cosmosResponseMessage, cancellationToken);
      return await this.ShouldRetryInternalAsync(cosmosResponseMessage?.StatusCode, cosmosResponseMessage?.Headers.SubStatusCode, shouldRetryResult, cancellationToken);
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
          ContainerProperties containerProperties = await this.collectionCache.ResolveCollectionAsync(this.request, cancellationToken, (ITrace) NoOpTrace.Singleton);
          if (containerProperties == null)
            DefaultTrace.TraceCritical("Can't recover from session unavailable exception because resolving collection name {0} returned null", (object) this.request.ResourceAddress);
          else if (!string.IsNullOrEmpty(oldCollectionRid))
          {
            if (!string.IsNullOrEmpty(containerProperties.ResourceId))
            {
              if (!oldCollectionRid.Equals(containerProperties.ResourceId))
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
