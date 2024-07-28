// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ReplicatedResourceClient
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class ReplicatedResourceClient
  {
    private const string EnableGlobalStrongConfigurationName = "EnableGlobalStrong";
    private const int GoneAndRetryWithRetryTimeoutInSeconds = 30;
    private const int StrongGoneAndRetryWithRetryTimeoutInSeconds = 60;
    private readonly TimeSpan minBackoffForFallingBackToOtherRegions = TimeSpan.FromSeconds(1.0);
    private readonly AddressSelector addressSelector;
    private readonly IAddressResolver addressResolver;
    private readonly ConsistencyReader consistencyReader;
    private readonly ConsistencyWriter consistencyWriter;
    private readonly Protocol protocol;
    private readonly TransportClient transportClient;
    private readonly IServiceConfigurationReader serviceConfigReader;
    private readonly bool enableReadRequestsFallback;
    private readonly bool useMultipleWriteLocations;
    private readonly bool detectClientConnectivityIssues;
    private readonly RetryWithConfiguration retryWithConfiguration;
    private readonly ConnectionStateListener connectionStateListener;
    private static readonly Lazy<bool> enableGlobalStrong = new Lazy<bool>((Func<bool>) (() => true));

    public ReplicatedResourceClient(
      IAddressResolver addressResolver,
      ISessionContainer sessionContainer,
      Protocol protocol,
      TransportClient transportClient,
      ConnectionStateListener connectionStateListener,
      IServiceConfigurationReader serviceConfigReader,
      IAuthorizationTokenProvider authorizationTokenProvider,
      bool enableReadRequestsFallback,
      bool useMultipleWriteLocations,
      bool detectClientConnectivityIssues,
      RetryWithConfiguration retryWithConfiguration = null)
    {
      this.addressResolver = addressResolver;
      this.addressSelector = new AddressSelector(addressResolver, protocol);
      this.protocol = protocol == Protocol.Https || protocol == Protocol.Tcp ? protocol : throw new ArgumentOutOfRangeException(nameof (protocol));
      this.transportClient = transportClient;
      this.serviceConfigReader = serviceConfigReader;
      this.connectionStateListener = connectionStateListener;
      this.consistencyReader = new ConsistencyReader(this.addressSelector, sessionContainer, transportClient, serviceConfigReader, authorizationTokenProvider, this.connectionStateListener);
      this.consistencyWriter = new ConsistencyWriter(this.addressSelector, sessionContainer, transportClient, serviceConfigReader, authorizationTokenProvider, this.connectionStateListener, useMultipleWriteLocations);
      this.enableReadRequestsFallback = enableReadRequestsFallback;
      this.useMultipleWriteLocations = useMultipleWriteLocations;
      this.detectClientConnectivityIssues = detectClientConnectivityIssues;
      this.retryWithConfiguration = retryWithConfiguration;
    }

    public string LastReadAddress
    {
      get => this.consistencyReader.LastReadAddress;
      set => this.consistencyReader.LastReadAddress = value;
    }

    public string LastWriteAddress => this.consistencyWriter.LastWriteAddress;

    public bool ForceAddressRefresh { get; set; }

    public Task<StoreResponse> InvokeAsync(
      DocumentServiceRequest request,
      Func<DocumentServiceRequest, Task> prepareRequestAsyncDelegate = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Func<GoneAndRetryRequestRetryPolicyContext, Task<StoreResponse>> executeAsync = (Func<GoneAndRetryRequestRetryPolicyContext, Task<StoreResponse>>) (async contextArguments =>
      {
        if (prepareRequestAsyncDelegate != null)
          await prepareRequestAsyncDelegate(request);
        request.Headers["x-ms-client-retry-attempt-count"] = contextArguments.ClientRetryCount.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        request.Headers["x-ms-remaining-time-in-ms-on-client"] = contextArguments.RemainingTimeInMsOnClientRequest.TotalMilliseconds.ToString();
        return await this.InvokeAsync(request, new TimeoutHelper(contextArguments.RemainingTimeInMsOnClientRequest, cancellationToken), contextArguments.IsInRetry, contextArguments.ForceRefresh || this.ForceAddressRefresh, cancellationToken);
      });
      Func<GoneAndRetryRequestRetryPolicyContext, Task<StoreResponse>> func = (Func<GoneAndRetryRequestRetryPolicyContext, Task<StoreResponse>>) null;
      if (request.OperationType.IsReadOperation() && this.enableReadRequestsFallback || this.CheckWriteRetryable(request))
      {
        IClientSideRequestStatistics sharedStatistics = (IClientSideRequestStatistics) null;
        if (request.RequestContext.ClientRequestStatistics == null)
        {
          sharedStatistics = (IClientSideRequestStatistics) new ClientSideRequestStatistics();
          request.RequestContext.ClientRequestStatistics = sharedStatistics;
        }
        else
          sharedStatistics = request.RequestContext.ClientRequestStatistics;
        DocumentServiceRequest freshRequest = request.Clone();
        func = (Func<GoneAndRetryRequestRetryPolicyContext, Task<StoreResponse>>) (async retryContext =>
        {
          DocumentServiceRequest requestClone = freshRequest.Clone();
          requestClone.RequestContext.ClientRequestStatistics = sharedStatistics;
          if (prepareRequestAsyncDelegate != null)
            await prepareRequestAsyncDelegate(requestClone);
          DefaultTrace.TraceInformation("Executing inBackoffAlternateCallbackMethod on regionIndex {0}", (object) retryContext.RegionRerouteAttemptCount);
          requestClone.RequestContext.RouteToLocation(retryContext.RegionRerouteAttemptCount, true);
          return await RequestRetryUtility.ProcessRequestAsync<GoneOnlyRequestRetryPolicyContext, DocumentServiceRequest, StoreResponse>((Func<GoneOnlyRequestRetryPolicyContext, Task<StoreResponse>>) (innerRetryContext => this.InvokeAsync(requestClone, new TimeoutHelper(innerRetryContext.RemainingTimeInMsOnClientRequest, cancellationToken), innerRetryContext.IsInRetry, innerRetryContext.ForceRefresh, cancellationToken)), (Func<DocumentServiceRequest>) (() =>
          {
            requestClone.RequestContext.ClientRequestStatistics?.RecordRequest(requestClone);
            return requestClone;
          }), (IRequestRetryPolicy<GoneOnlyRequestRetryPolicyContext, DocumentServiceRequest, StoreResponse>) new GoneOnlyRequestRetryPolicy<StoreResponse>(retryContext.TimeoutForInBackoffRetryPolicy), cancellationToken);
        });
      }
      int num = this.serviceConfigReader.DefaultConsistencyLevel == ConsistencyLevel.Strong ? 60 : 30;
      Func<DocumentServiceRequest> prepareRequest = (Func<DocumentServiceRequest>) (() =>
      {
        request.RequestContext.ClientRequestStatistics?.RecordRequest(request);
        return request;
      });
      GoneAndRetryWithRequestRetryPolicy<StoreResponse> policy = new GoneAndRetryWithRequestRetryPolicy<StoreResponse>(new int?(num), this.minBackoffForFallingBackToOtherRegions, this.detectClientConnectivityIssues, this.retryWithConfiguration);
      Func<GoneAndRetryRequestRetryPolicyContext, Task<StoreResponse>> inBackoffAlternateCallbackMethod = func;
      TimeSpan backToOtherRegions = this.minBackoffForFallingBackToOtherRegions;
      CancellationToken cancellationToken1 = cancellationToken;
      return RequestRetryUtility.ProcessRequestAsync<GoneAndRetryRequestRetryPolicyContext, DocumentServiceRequest, StoreResponse>(executeAsync, prepareRequest, (IRequestRetryPolicy<GoneAndRetryRequestRetryPolicyContext, DocumentServiceRequest, StoreResponse>) policy, inBackoffAlternateCallbackMethod, backToOtherRegions, cancellationToken1);
    }

    private Task<StoreResponse> InvokeAsync(
      DocumentServiceRequest request,
      TimeoutHelper timeout,
      bool isInRetry,
      bool forceRefresh,
      CancellationToken cancellationToken)
    {
      if (request.OperationType == OperationType.ExecuteJavaScript)
        return request.IsReadOnlyScript ? this.consistencyReader.ReadAsync(request, timeout, isInRetry, forceRefresh) : this.consistencyWriter.WriteAsync(request, timeout, forceRefresh, cancellationToken);
      if (request.OperationType.IsWriteOperation())
        return this.consistencyWriter.WriteAsync(request, timeout, forceRefresh, cancellationToken);
      if (request.OperationType.IsReadOperation())
        return this.consistencyReader.ReadAsync(request, timeout, isInRetry, forceRefresh);
      return request.OperationType == OperationType.Throttle || request.OperationType == OperationType.PreCreateValidation || request.OperationType == OperationType.OfferPreGrowValidation ? this.HandleThrottlePreCreateOrOfferPreGrowAsync(request, forceRefresh) : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected operation type {0}", (object) request.OperationType));
    }

    private async Task<StoreResponse> HandleThrottlePreCreateOrOfferPreGrowAsync(
      DocumentServiceRequest request,
      bool forceRefresh)
    {
      DocumentServiceRequest requestReplica = DocumentServiceRequest.Create(OperationType.Create, ResourceType.Database, request.RequestAuthorizationTokenType);
      PartitionAddressInformation addressInformation = await this.addressResolver.ResolveAsync(requestReplica, forceRefresh, CancellationToken.None);
      Uri primaryUri = addressInformation.GetPrimaryUri(requestReplica, this.protocol);
      this.connectionStateListener?.UpdateConnectionState(primaryUri, addressInformation.AddressCacheToken);
      return await this.transportClient.InvokeResourceOperationAsync(primaryUri, request);
    }

    private bool CheckWriteRetryable(DocumentServiceRequest request)
    {
      bool flag = false;
      if (this.useMultipleWriteLocations && (request.OperationType == OperationType.Execute && request.ResourceType == ResourceType.StoredProcedure || request.OperationType.IsWriteOperation() && request.ResourceType == ResourceType.Document))
        flag = true;
      return flag;
    }

    internal static bool IsGlobalStrongEnabled() => true;

    internal static bool IsReadingFromMaster(ResourceType resourceType, OperationType operationType) => resourceType == ResourceType.Offer || resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.ClientEncryptionKey || resourceType == ResourceType.UserDefinedType || resourceType == ResourceType.Permission || resourceType == ResourceType.DatabaseAccount || resourceType == ResourceType.Snapshot || resourceType == ResourceType.Topology || resourceType == ResourceType.PartitionKeyRange && operationType != OperationType.GetSplitPoint && operationType != OperationType.GetSplitPoints && operationType != OperationType.AbortSplit || resourceType == ResourceType.Collection && (operationType == OperationType.ReadFeed || operationType == OperationType.Query || operationType == OperationType.SqlQuery);

    internal static bool IsMasterResource(ResourceType resourceType) => resourceType == ResourceType.Offer || resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.ClientEncryptionKey || resourceType == ResourceType.UserDefinedType || resourceType == ResourceType.Permission || resourceType == ResourceType.Topology || resourceType == ResourceType.DatabaseAccount || resourceType == ResourceType.PartitionKeyRange || resourceType == ResourceType.Collection || resourceType == ResourceType.Snapshot;
  }
}
