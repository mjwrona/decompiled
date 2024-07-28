// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ReplicatedResourceClient
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
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
    private readonly bool disableRetryWithRetryPolicy;
    private static readonly Lazy<bool> enableGlobalStrong = new Lazy<bool>((Func<bool>) (() =>
    {
      bool result = true;
      if (Assembly.GetEntryAssembly() != (Assembly) null)
      {
        string appSetting = ConfigurationManager.AppSettings["EnableGlobalStrong"];
        if (!string.IsNullOrEmpty(appSetting) && !bool.TryParse(appSetting, out result))
          return false;
      }
      return result;
    }));

    public ReplicatedResourceClient(
      IAddressResolver addressResolver,
      ISessionContainer sessionContainer,
      Protocol protocol,
      TransportClient transportClient,
      IServiceConfigurationReader serviceConfigReader,
      IAuthorizationTokenProvider authorizationTokenProvider,
      bool enableReadRequestsFallback,
      bool useMultipleWriteLocations,
      bool detectClientConnectivityIssues,
      bool disableRetryWithRetryPolicy,
      RetryWithConfiguration retryWithConfiguration = null)
    {
      this.addressResolver = addressResolver;
      this.addressSelector = new AddressSelector(addressResolver, protocol);
      this.protocol = protocol == Protocol.Https || protocol == Protocol.Tcp ? protocol : throw new ArgumentOutOfRangeException(nameof (protocol));
      this.transportClient = transportClient;
      this.serviceConfigReader = serviceConfigReader;
      this.consistencyReader = new ConsistencyReader(this.addressSelector, sessionContainer, transportClient, serviceConfigReader, authorizationTokenProvider);
      this.consistencyWriter = new ConsistencyWriter(this.addressSelector, sessionContainer, transportClient, serviceConfigReader, authorizationTokenProvider, useMultipleWriteLocations);
      this.enableReadRequestsFallback = enableReadRequestsFallback;
      this.useMultipleWriteLocations = useMultipleWriteLocations;
      this.detectClientConnectivityIssues = detectClientConnectivityIssues;
      this.retryWithConfiguration = retryWithConfiguration;
      this.disableRetryWithRetryPolicy = disableRetryWithRetryPolicy;
    }

    public string LastReadAddress
    {
      get => this.consistencyReader.LastReadAddress;
      set => this.consistencyReader.LastReadAddress = value;
    }

    public string LastWriteAddress => this.consistencyWriter.LastWriteAddress;

    public bool ForceAddressRefresh { get; set; }

    public int? GoneAndRetryWithRetryTimeoutInSecondsOverride { get; set; }

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
      int? inSecondsOverride = this.GoneAndRetryWithRetryTimeoutInSecondsOverride;
      if (inSecondsOverride.HasValue)
      {
        inSecondsOverride = this.GoneAndRetryWithRetryTimeoutInSecondsOverride;
        num = inSecondsOverride.Value;
      }
      Func<DocumentServiceRequest> prepareRequest = (Func<DocumentServiceRequest>) (() =>
      {
        request.RequestContext.ClientRequestStatistics?.RecordRequest(request);
        return request;
      });
      GoneAndRetryWithRequestRetryPolicy<StoreResponse> policy = new GoneAndRetryWithRequestRetryPolicy<StoreResponse>(this.disableRetryWithRetryPolicy || request.DisableRetryWithPolicy, new int?(num), this.minBackoffForFallingBackToOtherRegions, this.detectClientConnectivityIssues, this.retryWithConfiguration);
      Func<GoneAndRetryRequestRetryPolicyContext, Task<StoreResponse>> inBackoffAlternateCallbackMethod = func;
      TimeSpan backToOtherRegions = this.minBackoffForFallingBackToOtherRegions;
      CancellationToken cancellationToken1 = cancellationToken;
      return RequestRetryUtility.ProcessRequestAsync<GoneAndRetryRequestRetryPolicyContext, DocumentServiceRequest, StoreResponse>(executeAsync, prepareRequest, (IRequestRetryPolicy<GoneAndRetryRequestRetryPolicyContext, DocumentServiceRequest, StoreResponse>) policy, inBackoffAlternateCallbackMethod, backToOtherRegions, cancellationToken1);
    }

    public async Task OpenConnectionsToAllReplicasAsync(
      string databaseName,
      string containerLinkUri,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IAddressResolverExtension addressResolver = (IAddressResolverExtension) this.addressResolver;
      if (addressResolver == null)
        throw new InvalidOperationException("The Address Resolver provided is not an instance of IAddressResolverExtension.");
      await addressResolver.OpenConnectionsToAllReplicasAsync(databaseName, containerLinkUri, new Func<Uri, Task>(this.transportClient.OpenConnectionAsync), cancellationToken);
    }

    private Task<StoreResponse> InvokeAsync(
      DocumentServiceRequest request,
      TimeoutHelper timeout,
      bool isInRetry,
      bool forceRefresh,
      CancellationToken cancellationToken)
    {
      if (request.OperationType == OperationType.ExecuteJavaScript)
        return request.IsReadOnlyScript ? this.consistencyReader.ReadAsync(request, timeout, isInRetry, forceRefresh, cancellationToken) : this.consistencyWriter.WriteAsync(request, timeout, forceRefresh, cancellationToken);
      if (request.OperationType.IsWriteOperation())
        return this.consistencyWriter.WriteAsync(request, timeout, forceRefresh, cancellationToken);
      if (request.OperationType.IsReadOperation())
        return this.consistencyReader.ReadAsync(request, timeout, isInRetry, forceRefresh, cancellationToken);
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected operation type {0}", (object) request.OperationType));
    }

    private async Task<StoreResponse> HandleGetStorageAuthTokenAsync(
      DocumentServiceRequest request,
      bool forceRefresh)
    {
      return await this.transportClient.InvokeResourceOperationAsync((await this.addressResolver.ResolveAsync(request, forceRefresh, CancellationToken.None)).GetPrimaryUri(request, this.protocol), request);
    }

    private async Task<StoreResponse> HandleThrottlePreCreateOrOfferPreGrowAsync(
      DocumentServiceRequest request,
      bool forceRefresh)
    {
      DocumentServiceRequest requestReplica = DocumentServiceRequest.Create(OperationType.Create, ResourceType.Database, request.RequestAuthorizationTokenType);
      StoreResponse offerPreGrowAsync = await this.transportClient.InvokeResourceOperationAsync((await this.addressResolver.ResolveAsync(requestReplica, forceRefresh, CancellationToken.None)).GetPrimaryUri(requestReplica, this.protocol), request);
      requestReplica = (DocumentServiceRequest) null;
      return offerPreGrowAsync;
    }

    private bool CheckWriteRetryable(DocumentServiceRequest request)
    {
      bool flag = false;
      if (this.useMultipleWriteLocations && (request.OperationType == OperationType.Execute && request.ResourceType == ResourceType.StoredProcedure || request.OperationType.IsWriteOperation() && request.ResourceType == ResourceType.Document))
        flag = true;
      return flag;
    }

    internal static bool IsGlobalStrongEnabled() => ReplicatedResourceClient.enableGlobalStrong.Value;

    internal static bool IsReadingFromMaster(ResourceType resourceType, OperationType operationType)
    {
      switch (resourceType)
      {
        case ResourceType.Database:
        case ResourceType.User:
        case ResourceType.Permission:
        case ResourceType.Offer:
        case ResourceType.DatabaseAccount:
        case ResourceType.PartitionKeyRange:
        case ResourceType.UserDefinedType:
        case ResourceType.Snapshot:
        case ResourceType.ClientEncryptionKey:
        case ResourceType.RoleDefinition:
        case ResourceType.RoleAssignment:
        case ResourceType.InteropUser:
        case ResourceType.AuthPolicyElement:
          return true;
        case ResourceType.Collection:
          if (operationType == OperationType.ReadFeed || operationType == OperationType.Query || operationType == OperationType.SqlQuery)
            goto case ResourceType.Database;
          else
            break;
      }
      return false;
    }

    internal static bool IsSessionTokenRequired(
      ResourceType resourceType,
      OperationType operationType)
    {
      return !ReplicatedResourceClient.IsMasterResource(resourceType) && !ReplicatedResourceClient.IsStoredProcedureCrudOperation(resourceType, operationType) && operationType != OperationType.QueryPlan;
    }

    internal static bool IsStoredProcedureCrudOperation(
      ResourceType resourceType,
      OperationType operationType)
    {
      return resourceType == ResourceType.StoredProcedure && operationType != OperationType.ExecuteJavaScript;
    }

    internal static bool IsMasterResource(ResourceType resourceType) => resourceType == ResourceType.Offer || resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.ClientEncryptionKey || resourceType == ResourceType.UserDefinedType || resourceType == ResourceType.Permission || resourceType == ResourceType.DatabaseAccount || resourceType == ResourceType.PartitionKeyRange || resourceType == ResourceType.Collection || resourceType == ResourceType.Snapshot || resourceType == ResourceType.RoleAssignment || resourceType == ResourceType.RoleDefinition || resourceType == ResourceType.Trigger || resourceType == ResourceType.UserDefinedFunction;
  }
}
