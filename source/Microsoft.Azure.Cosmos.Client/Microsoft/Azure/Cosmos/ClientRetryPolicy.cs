// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ClientRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ClientRetryPolicy : IDocumentClientRetryPolicy, IRetryPolicy
  {
    private const int RetryIntervalInMS = 1000;
    private const int MaxRetryCount = 120;
    private const int MaxServiceUnavailableRetryCount = 1;
    private readonly IDocumentClientRetryPolicy throttlingRetry;
    private readonly GlobalEndpointManager globalEndpointManager;
    private readonly GlobalPartitionEndpointManager partitionKeyRangeLocationCache;
    private readonly bool enableEndpointDiscovery;
    private int failoverRetryCount;
    private int sessionTokenRetryCount;
    private int serviceUnavailableRetryCount;
    private bool isReadRequest;
    private bool canUseMultipleWriteLocations;
    private Uri locationEndpoint;
    private ClientRetryPolicy.RetryContext retryContext;
    private DocumentServiceRequest documentServiceRequest;

    public ClientRetryPolicy(
      GlobalEndpointManager globalEndpointManager,
      GlobalPartitionEndpointManager partitionKeyRangeLocationCache,
      bool enableEndpointDiscovery,
      RetryOptions retryOptions)
    {
      this.throttlingRetry = (IDocumentClientRetryPolicy) new ResourceThrottleRetryPolicy(retryOptions.MaxRetryAttemptsOnThrottledRequests, retryOptions.MaxRetryWaitTimeInSeconds);
      this.globalEndpointManager = globalEndpointManager;
      this.partitionKeyRangeLocationCache = partitionKeyRangeLocationCache;
      this.failoverRetryCount = 0;
      this.enableEndpointDiscovery = enableEndpointDiscovery;
      this.sessionTokenRetryCount = 0;
      this.serviceUnavailableRetryCount = 0;
      this.canUseMultipleWriteLocations = false;
    }

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      this.retryContext = (ClientRetryPolicy.RetryContext) null;
      if (exception is HttpRequestException)
      {
        DefaultTrace.TraceWarning("ClientRetryPolicy: Gateway HttpRequestException Endpoint not reachable. Failed Location: {0}; ResourceAddress: {1}", (object) (this.documentServiceRequest?.RequestContext?.LocationEndpointToRoute?.ToString() ?? string.Empty), (object) (this.documentServiceRequest?.ResourceAddress ?? string.Empty));
        return await this.ShouldRetryOnEndpointFailureAsync(this.isReadRequest, true, false, true);
      }
      if (exception is DocumentClientException documentClientException)
      {
        ShouldRetryResult shouldRetryResult = await this.ShouldRetryInternalAsync((HttpStatusCode?) documentClientException?.StatusCode, documentClientException?.GetSubStatus());
        if (shouldRetryResult != null)
          return shouldRetryResult;
      }
      return await this.throttlingRetry.ShouldRetryAsync(exception, cancellationToken);
    }

    public async Task<ShouldRetryResult> ShouldRetryAsync(
      ResponseMessage cosmosResponseMessage,
      CancellationToken cancellationToken)
    {
      this.retryContext = (ClientRetryPolicy.RetryContext) null;
      ShouldRetryResult shouldRetryResult = await this.ShouldRetryInternalAsync(cosmosResponseMessage?.StatusCode, cosmosResponseMessage?.Headers.SubStatusCode);
      return shouldRetryResult != null ? shouldRetryResult : await this.throttlingRetry.ShouldRetryAsync(cosmosResponseMessage, cancellationToken);
    }

    public void OnBeforeSendRequest(DocumentServiceRequest request)
    {
      this.isReadRequest = request.IsReadOnlyRequest;
      this.canUseMultipleWriteLocations = this.globalEndpointManager.CanUseMultipleWriteLocations(request);
      this.documentServiceRequest = request;
      request.RequestContext.ClearRouteToLocation();
      if (this.retryContext != null)
        request.RequestContext.RouteToLocation(this.retryContext.RetryLocationIndex, this.retryContext.RetryRequestOnPreferredLocations);
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
      HttpStatusCode httpStatusCode1 = HttpStatusCode.RequestTimeout;
      if (nullable1.GetValueOrDefault() == httpStatusCode1 & nullable1.HasValue)
      {
        DefaultTrace.TraceWarning("ClientRetryPolicy: RequestTimeout. Failed Location: {0}; ResourceAddress: {1}", (object) (this.documentServiceRequest?.RequestContext?.LocationEndpointToRoute?.ToString() ?? string.Empty), (object) (this.documentServiceRequest?.ResourceAddress ?? string.Empty));
        this.partitionKeyRangeLocationCache.TryMarkEndpointUnavailableForPartitionKeyRange(this.documentServiceRequest);
      }
      nullable1 = statusCode;
      HttpStatusCode httpStatusCode2 = HttpStatusCode.Forbidden;
      SubStatusCodes? nullable2;
      if (nullable1.GetValueOrDefault() == httpStatusCode2 & nullable1.HasValue)
      {
        nullable2 = subStatusCode;
        SubStatusCodes subStatusCodes = SubStatusCodes.WriteForbidden;
        if (nullable2.GetValueOrDefault() == subStatusCodes & nullable2.HasValue)
        {
          if (this.partitionKeyRangeLocationCache.TryMarkEndpointUnavailableForPartitionKeyRange(this.documentServiceRequest))
            return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
          DefaultTrace.TraceWarning("ClientRetryPolicy: Endpoint not writable. Refresh cache and retry. Failed Location: {0}; ResourceAddress: {1}", (object) (this.documentServiceRequest?.RequestContext?.LocationEndpointToRoute?.ToString() ?? string.Empty), (object) (this.documentServiceRequest?.ResourceAddress ?? string.Empty));
          return await this.ShouldRetryOnEndpointFailureAsync(false, false, true, false);
        }
      }
      nullable1 = statusCode;
      HttpStatusCode httpStatusCode3 = HttpStatusCode.Forbidden;
      if (nullable1.GetValueOrDefault() == httpStatusCode3 & nullable1.HasValue)
      {
        nullable2 = subStatusCode;
        SubStatusCodes subStatusCodes = SubStatusCodes.CompletingPartitionMigration;
        if (nullable2.GetValueOrDefault() == subStatusCodes & nullable2.HasValue && (this.isReadRequest || this.canUseMultipleWriteLocations))
        {
          DefaultTrace.TraceWarning("ClientRetryPolicy: Endpoint not available for reads. Refresh cache and retry. Failed Location: {0}; ResourceAddress: {1}", (object) (this.documentServiceRequest?.RequestContext?.LocationEndpointToRoute?.ToString() ?? string.Empty), (object) (this.documentServiceRequest?.ResourceAddress ?? string.Empty));
          return await this.ShouldRetryOnEndpointFailureAsync(this.isReadRequest, false, false, false);
        }
      }
      nullable1 = statusCode;
      HttpStatusCode httpStatusCode4 = HttpStatusCode.NotFound;
      if (nullable1.GetValueOrDefault() == httpStatusCode4 & nullable1.HasValue)
      {
        nullable2 = subStatusCode;
        SubStatusCodes subStatusCodes = SubStatusCodes.PartitionKeyRangeGone;
        if (nullable2.GetValueOrDefault() == subStatusCodes & nullable2.HasValue)
          return this.ShouldRetryOnSessionNotAvailable();
      }
      nullable1 = statusCode;
      HttpStatusCode httpStatusCode5 = HttpStatusCode.ServiceUnavailable;
      if (!(nullable1.GetValueOrDefault() == httpStatusCode5 & nullable1.HasValue) || !ClientRetryPolicy.IsRetriableServiceUnavailable(subStatusCode))
        return (ShouldRetryResult) null;
      DefaultTrace.TraceWarning("ClientRetryPolicy: ServiceUnavailable. Refresh cache and retry. Failed Location: {0}; ResourceAddress: {1}", (object) (this.documentServiceRequest?.RequestContext?.LocationEndpointToRoute?.ToString() ?? string.Empty), (object) (this.documentServiceRequest?.ResourceAddress ?? string.Empty));
      this.partitionKeyRangeLocationCache.TryMarkEndpointUnavailableForPartitionKeyRange(this.documentServiceRequest);
      return this.ShouldRetryOnServiceUnavailable();
    }

    private static bool IsRetriableServiceUnavailable(SubStatusCodes? subStatusCode)
    {
      SubStatusCodes? nullable = subStatusCode;
      SubStatusCodes subStatusCodes = SubStatusCodes.Unknown;
      if (nullable.GetValueOrDefault() == subStatusCodes & nullable.HasValue)
        return true;
      return subStatusCode.HasValue && subStatusCode.Value.IsSDKGeneratedSubStatus();
    }

    private async Task<ShouldRetryResult> ShouldRetryOnEndpointFailureAsync(
      bool isReadRequest,
      bool markBothReadAndWriteAsUnavailable,
      bool forceRefresh,
      bool retryOnPreferredLocations)
    {
      ClientRetryPolicy clientRetryPolicy = this;
      if (!clientRetryPolicy.enableEndpointDiscovery || clientRetryPolicy.failoverRetryCount > 120)
      {
        DefaultTrace.TraceInformation("ClientRetryPolicy: ShouldRetryOnEndpointFailureAsync() Not retrying. Retry count = {0}, Endpoint = {1}", (object) clientRetryPolicy.failoverRetryCount, (object) (clientRetryPolicy.locationEndpoint?.ToString() ?? string.Empty));
        return ShouldRetryResult.NoRetry();
      }
      ++clientRetryPolicy.failoverRetryCount;
      if (clientRetryPolicy.locationEndpoint != (Uri) null)
      {
        if (isReadRequest | markBothReadAndWriteAsUnavailable)
          clientRetryPolicy.globalEndpointManager.MarkEndpointUnavailableForRead(clientRetryPolicy.locationEndpoint);
        if (!isReadRequest | markBothReadAndWriteAsUnavailable)
          clientRetryPolicy.globalEndpointManager.MarkEndpointUnavailableForWrite(clientRetryPolicy.locationEndpoint);
      }
      TimeSpan retryDelay = TimeSpan.Zero;
      if (!isReadRequest)
      {
        DefaultTrace.TraceInformation("ClientRetryPolicy: Failover happening. retryCount {0}", (object) clientRetryPolicy.failoverRetryCount);
        if (clientRetryPolicy.failoverRetryCount > 1)
          retryDelay = TimeSpan.FromMilliseconds(1000.0);
      }
      else
        retryDelay = TimeSpan.FromMilliseconds(1000.0);
      await clientRetryPolicy.globalEndpointManager.RefreshLocationAsync(forceRefresh);
      int num = clientRetryPolicy.failoverRetryCount;
      if (retryOnPreferredLocations)
        num = 0;
      clientRetryPolicy.retryContext = new ClientRetryPolicy.RetryContext()
      {
        RetryLocationIndex = num,
        RetryRequestOnPreferredLocations = retryOnPreferredLocations
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
          RetryLocationIndex = this.sessionTokenRetryCount,
          RetryRequestOnPreferredLocations = true
        };
        return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
      }
      if (this.sessionTokenRetryCount > 1)
        return ShouldRetryResult.NoRetry();
      this.retryContext = new ClientRetryPolicy.RetryContext()
      {
        RetryLocationIndex = 0,
        RetryRequestOnPreferredLocations = false
      };
      return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
    }

    private ShouldRetryResult ShouldRetryOnServiceUnavailable()
    {
      if (this.serviceUnavailableRetryCount++ >= 1)
      {
        DefaultTrace.TraceInformation(string.Format("ClientRetryPolicy: ShouldRetryOnServiceUnavailable() Not retrying. Retry count = {0}.", (object) this.serviceUnavailableRetryCount));
        return ShouldRetryResult.NoRetry();
      }
      if (!this.canUseMultipleWriteLocations && !this.isReadRequest)
        return ShouldRetryResult.NoRetry();
      int preferredLocationCount = this.globalEndpointManager.PreferredLocationCount;
      if (preferredLocationCount <= 1)
      {
        DefaultTrace.TraceInformation(string.Format("ClientRetryPolicy: ShouldRetryOnServiceUnavailable() Not retrying. No other regions available for the request. AvailablePreferredLocations = {0}.", (object) preferredLocationCount));
        return ShouldRetryResult.NoRetry();
      }
      DefaultTrace.TraceInformation(string.Format("ClientRetryPolicy: ShouldRetryOnServiceUnavailable() Retrying. Received on endpoint {0}, IsReadRequest = {1}.", (object) this.locationEndpoint, (object) this.isReadRequest));
      this.retryContext = new ClientRetryPolicy.RetryContext()
      {
        RetryLocationIndex = this.serviceUnavailableRetryCount,
        RetryRequestOnPreferredLocations = true
      };
      return ShouldRetryResult.RetryAfter(TimeSpan.Zero);
    }

    private sealed class RetryContext
    {
      public int RetryLocationIndex { get; set; }

      public bool RetryRequestOnPreferredLocations { get; set; }
    }
  }
}
