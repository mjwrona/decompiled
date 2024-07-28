// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ClientRetryPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class ClientRetryPolicy : IDocumentClientRetryPolicy, IRetryPolicy
  {
    private const int RetryIntervalInMS = 1000;
    private const int MaxRetryCount = 120;
    private readonly IDocumentClientRetryPolicy throttlingRetry;
    private readonly GlobalEndpointManager globalEndpointManager;
    private readonly bool enableEndpointDiscovery;
    private int failoverRetryCount;
    private int sessionTokenRetryCount;
    private bool isReadRequest;
    private bool canUseMultipleWriteLocations;
    private Uri locationEndpoint;
    private ClientRetryPolicy.RetryContext retryContext;
    private IClientSideRequestStatistics sharedStatistics;

    public ClientRetryPolicy(
      GlobalEndpointManager globalEndpointManager,
      bool enableEndpointDiscovery,
      RetryOptions retryOptions)
    {
      this.throttlingRetry = (IDocumentClientRetryPolicy) new ResourceThrottleRetryPolicy(retryOptions.MaxRetryAttemptsOnThrottledRequests, retryOptions.MaxRetryWaitTimeInSeconds);
      this.globalEndpointManager = globalEndpointManager;
      this.failoverRetryCount = 0;
      this.enableEndpointDiscovery = enableEndpointDiscovery;
      this.sessionTokenRetryCount = 0;
      this.canUseMultipleWriteLocations = false;
      this.sharedStatistics = (IClientSideRequestStatistics) new ClientSideRequestStatistics();
    }

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      this.retryContext = (ClientRetryPolicy.RetryContext) null;
      if (exception is HttpRequestException)
      {
        DefaultTrace.TraceWarning("Endpoint not reachable. Refresh cache and retry");
        return await this.ShouldRetryOnEndpointFailureAsync(this.isReadRequest, false);
      }
      if ((exception is DocumentClientException documentClientException ? documentClientException.RequestStatistics : (IClientSideRequestStatistics) null) != null)
        this.sharedStatistics = documentClientException.RequestStatistics;
      ShouldRetryResult shouldRetryResult = await this.ShouldRetryInternalAsync((HttpStatusCode?) documentClientException?.StatusCode, documentClientException?.GetSubStatus());
      return shouldRetryResult != null ? shouldRetryResult : await this.throttlingRetry.ShouldRetryAsync(exception, cancellationToken);
    }

    public void OnBeforeSendRequest(DocumentServiceRequest request)
    {
      this.isReadRequest = request.IsReadOnlyRequest;
      this.canUseMultipleWriteLocations = this.globalEndpointManager.CanUseMultipleWriteLocations(request);
      request.RequestContext.ClientRequestStatistics = this.sharedStatistics;
      request.RequestContext.ClearRouteToLocation();
      if (this.retryContext != null)
        request.RequestContext.RouteToLocation(this.retryContext.RetryCount, this.retryContext.RetryRequestOnPreferredLocations);
      this.locationEndpoint = this.globalEndpointManager.ResolveServiceEndpoint(request);
      request.RequestContext.RouteToLocation(this.locationEndpoint);
    }

    private async Task<ShouldRetryResult> ShouldRetryInternalAsync(
      HttpStatusCode? statusCode,
      SubStatusCodes? subStatusCode)
    {
      if (!statusCode.HasValue && (!subStatusCode.HasValue || subStatusCode.Value == SubStatusCodes.Unknown))
        return (ShouldRetryResult) null;
      HttpStatusCode? nullable1 = statusCode;
      HttpStatusCode httpStatusCode1 = HttpStatusCode.Forbidden;
      SubStatusCodes? nullable2;
      if (nullable1.GetValueOrDefault() == httpStatusCode1 & nullable1.HasValue)
      {
        nullable2 = subStatusCode;
        SubStatusCodes subStatusCodes = SubStatusCodes.WriteForbidden;
        if (nullable2.GetValueOrDefault() == subStatusCodes & nullable2.HasValue)
        {
          DefaultTrace.TraceWarning("Endpoint not writable. Refresh cache and retry");
          return await this.ShouldRetryOnEndpointFailureAsync(false, true);
        }
      }
      nullable1 = statusCode;
      HttpStatusCode httpStatusCode2 = HttpStatusCode.Forbidden;
      if (nullable1.GetValueOrDefault() == httpStatusCode2 & nullable1.HasValue)
      {
        nullable2 = subStatusCode;
        SubStatusCodes subStatusCodes = SubStatusCodes.CompletingPartitionMigration;
        if (nullable2.GetValueOrDefault() == subStatusCodes & nullable2.HasValue && (this.isReadRequest || this.canUseMultipleWriteLocations))
        {
          DefaultTrace.TraceWarning(string.Format("Endpoint not available. Refresh cache and retry.  IsReadRequest: {0}. CanUseMultipleWriteLocations: {1}", (object) this.isReadRequest, (object) this.canUseMultipleWriteLocations));
          return await this.ShouldRetryOnEndpointFailureAsync(this.isReadRequest, false);
        }
      }
      nullable1 = statusCode;
      HttpStatusCode httpStatusCode3 = HttpStatusCode.NotFound;
      if (nullable1.GetValueOrDefault() == httpStatusCode3 & nullable1.HasValue)
      {
        nullable2 = subStatusCode;
        SubStatusCodes subStatusCodes = SubStatusCodes.PartitionKeyRangeGone;
        if (nullable2.GetValueOrDefault() == subStatusCodes & nullable2.HasValue)
          return this.ShouldRetryOnSessionNotAvailable();
      }
      return (ShouldRetryResult) null;
    }

    private async Task<ShouldRetryResult> ShouldRetryOnEndpointFailureAsync(
      bool isReadRequest,
      bool forceRefresh)
    {
      ClientRetryPolicy clientRetryPolicy = this;
      if (!clientRetryPolicy.enableEndpointDiscovery || clientRetryPolicy.failoverRetryCount > 120)
      {
        DefaultTrace.TraceInformation("ShouldRetryOnEndpointFailureAsync() Not retrying. Retry count = {0}", (object) clientRetryPolicy.failoverRetryCount);
        return ShouldRetryResult.NoRetry();
      }
      ++clientRetryPolicy.failoverRetryCount;
      if (clientRetryPolicy.locationEndpoint != (Uri) null)
      {
        if (isReadRequest)
          clientRetryPolicy.globalEndpointManager.MarkEndpointUnavailableForRead(clientRetryPolicy.locationEndpoint);
        else
          clientRetryPolicy.globalEndpointManager.MarkEndpointUnavailableForWrite(clientRetryPolicy.locationEndpoint);
      }
      TimeSpan retryDelay = TimeSpan.Zero;
      if (!isReadRequest)
      {
        DefaultTrace.TraceInformation("Failover happening. retryCount {0}", (object) clientRetryPolicy.failoverRetryCount);
        if (clientRetryPolicy.failoverRetryCount > 1)
          retryDelay = TimeSpan.FromMilliseconds(1000.0);
      }
      else
        retryDelay = TimeSpan.FromMilliseconds(1000.0);
      await clientRetryPolicy.globalEndpointManager.RefreshLocationAsync((DatabaseAccount) null, forceRefresh);
      clientRetryPolicy.retryContext = new ClientRetryPolicy.RetryContext()
      {
        RetryCount = clientRetryPolicy.failoverRetryCount,
        RetryRequestOnPreferredLocations = false
      };
      return ShouldRetryResult.RetryAfter(retryDelay);
    }

    private ShouldRetryResult ShouldRetryOnSessionNotAvailable()
    {
      ++this.sessionTokenRetryCount;
      if (!this.enableEndpointDiscovery)
        return ShouldRetryResult.NoRetry();
      if (this.canUseMultipleWriteLocations)
      {
        if (this.sessionTokenRetryCount > (this.isReadRequest ? this.globalEndpointManager.ReadEndpoints : this.globalEndpointManager.WriteEndpoints).Count)
          return ShouldRetryResult.NoRetry();
        this.retryContext = new ClientRetryPolicy.RetryContext()
        {
          RetryCount = this.sessionTokenRetryCount - 1,
          RetryRequestOnPreferredLocations = this.sessionTokenRetryCount > 1
        };
        return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
      }
      if (this.sessionTokenRetryCount > 1)
        return ShouldRetryResult.NoRetry();
      this.retryContext = new ClientRetryPolicy.RetryContext()
      {
        RetryCount = this.sessionTokenRetryCount - 1,
        RetryRequestOnPreferredLocations = false
      };
      return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
    }

    private sealed class RetryContext
    {
      public int RetryCount { get; set; }

      public bool RetryRequestOnPreferredLocations { get; set; }
    }
  }
}
