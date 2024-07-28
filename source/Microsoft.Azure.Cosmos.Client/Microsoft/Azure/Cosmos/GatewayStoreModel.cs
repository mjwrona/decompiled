// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.GatewayStoreModel
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class GatewayStoreModel : IStoreModel, IDisposable
  {
    private static readonly string sessionConsistencyAsString = ConsistencyLevel.Session.ToString();
    private readonly GlobalEndpointManager endpointManager;
    private readonly DocumentClientEventSource eventSource;
    private readonly ISessionContainer sessionContainer;
    private readonly ConsistencyLevel defaultConsistencyLevel;
    private GatewayStoreClient gatewayStoreClient;
    private ClientCollectionCache clientCollectionCache;
    private PartitionKeyRangeCache partitionKeyRangeCache;

    public GatewayStoreModel(
      GlobalEndpointManager endpointManager,
      ISessionContainer sessionContainer,
      ConsistencyLevel defaultConsistencyLevel,
      DocumentClientEventSource eventSource,
      JsonSerializerSettings serializerSettings,
      CosmosHttpClient httpClient)
    {
      this.endpointManager = endpointManager;
      this.sessionContainer = sessionContainer;
      this.defaultConsistencyLevel = defaultConsistencyLevel;
      this.eventSource = eventSource;
      this.gatewayStoreClient = new GatewayStoreClient(httpClient, (ICommunicationEventSource) this.eventSource, serializerSettings);
    }

    public virtual async Task<DocumentServiceResponse> ProcessMessageAsync(
      DocumentServiceRequest request,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      await GatewayStoreModel.ApplySessionTokenAsync(request, this.defaultConsistencyLevel, this.sessionContainer, this.partitionKeyRangeCache, (CollectionCache) this.clientCollectionCache, (IGlobalEndpointManager) this.endpointManager);
      DocumentServiceResponse response;
      try
      {
        Uri physicalAddress = GatewayStoreClient.IsFeedRequest(request.OperationType) ? this.GetFeedUri(request) : this.GetEntityUri(request);
        string regionName;
        if (request.ResourceType.Equals((object) ResourceType.Document) && this.endpointManager.TryGetLocationForGatewayDiagnostics(request.RequestContext.LocationEndpointToRoute, out regionName))
          request.RequestContext.RegionName = regionName;
        response = await this.gatewayStoreClient.InvokeAsync(request, request.ResourceType, physicalAddress, cancellationToken);
      }
      catch (DocumentClientException ex)
      {
        if (!ReplicatedResourceClient.IsMasterResource(request.ResourceType))
        {
          HttpStatusCode? statusCode = ex.StatusCode;
          HttpStatusCode httpStatusCode1 = HttpStatusCode.PreconditionFailed;
          if (!(statusCode.GetValueOrDefault() == httpStatusCode1 & statusCode.HasValue))
          {
            statusCode = ex.StatusCode;
            HttpStatusCode httpStatusCode2 = HttpStatusCode.Conflict;
            if (!(statusCode.GetValueOrDefault() == httpStatusCode2 & statusCode.HasValue))
            {
              statusCode = ex.StatusCode;
              HttpStatusCode httpStatusCode3 = HttpStatusCode.NotFound;
              if (!(statusCode.GetValueOrDefault() == httpStatusCode3 & statusCode.HasValue) || ex.GetSubStatus() == SubStatusCodes.PartitionKeyRangeGone)
                goto label_14;
            }
          }
          await this.CaptureSessionTokenAndHandleSplitAsync(ex.StatusCode, ex.GetSubStatus(), request, ex.Headers);
        }
label_14:
        throw;
      }
      await this.CaptureSessionTokenAndHandleSplitAsync(new HttpStatusCode?(response.StatusCode), response.SubStatusCode, request, response.Headers);
      DocumentServiceResponse documentServiceResponse = response;
      response = (DocumentServiceResponse) null;
      return documentServiceResponse;
    }

    public virtual async Task<AccountProperties> GetDatabaseAccountAsync(
      Func<ValueTask<HttpRequestMessage>> requestMessage,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountProperties databaseAccountAsync = (AccountProperties) null;
      using (HttpResponseMessage responseMessage = await this.gatewayStoreClient.SendHttpAsync(requestMessage, ResourceType.DatabaseAccount, HttpTimeoutPolicyControlPlaneRead.Instance, clientSideRequestStatistics, cancellationToken))
      {
        using (DocumentServiceResponse responseAsync = await ClientExtensions.ParseResponseAsync(responseMessage))
          databaseAccountAsync = CosmosResource.FromStream<AccountProperties>(responseAsync);
        IEnumerable<string> values;
        long result;
        if (responseMessage.Headers.TryGetValues("x-ms-max-media-storage-usage-mb", out values) && values.Count<string>() != 0 && long.TryParse(values.First<string>(), out result))
          databaseAccountAsync.MaxMediaStorageUsageInMB = result;
        if (responseMessage.Headers.TryGetValues("x-ms-media-storage-usage-mb", out values) && values.Count<string>() != 0 && long.TryParse(values.First<string>(), out result))
          databaseAccountAsync.MediaStorageUsageInMB = result;
        if (responseMessage.Headers.TryGetValues("x-ms-databaseaccount-consumed-mb", out values) && values.Count<string>() != 0 && long.TryParse(values.First<string>(), out result))
          databaseAccountAsync.ConsumedDocumentStorageInMB = result;
        if (responseMessage.Headers.TryGetValues("x-ms-databaseaccount-provisioned-mb", out values) && values.Count<string>() != 0 && long.TryParse(values.First<string>(), out result))
          databaseAccountAsync.ProvisionedDocumentStorageInMB = result;
        if (responseMessage.Headers.TryGetValues("x-ms-databaseaccount-reserved-mb", out values))
        {
          if (values.Count<string>() != 0)
          {
            if (long.TryParse(values.First<string>(), out result))
              databaseAccountAsync.ReservedDocumentStorageInMB = result;
          }
        }
      }
      return databaseAccountAsync;
    }

    public void SetCaches(
      PartitionKeyRangeCache partitionKeyRangeCache,
      ClientCollectionCache clientCollectionCache)
    {
      this.clientCollectionCache = clientCollectionCache;
      this.partitionKeyRangeCache = partitionKeyRangeCache;
    }

    public void Dispose() => this.Dispose(true);

    private async Task CaptureSessionTokenAndHandleSplitAsync(
      HttpStatusCode? statusCode,
      SubStatusCodes subStatusCode,
      DocumentServiceRequest request,
      INameValueCollection responseHeaders)
    {
      if (request.IsValidStatusCodeForExceptionlessRetry((int) statusCode.Value, subStatusCode))
      {
        if (ReplicatedResourceClient.IsMasterResource(request.ResourceType))
          return;
        HttpStatusCode? nullable = statusCode;
        HttpStatusCode httpStatusCode1 = HttpStatusCode.PreconditionFailed;
        if (!(nullable.GetValueOrDefault() == httpStatusCode1 & nullable.HasValue))
        {
          nullable = statusCode;
          HttpStatusCode httpStatusCode2 = HttpStatusCode.Conflict;
          if (!(nullable.GetValueOrDefault() == httpStatusCode2 & nullable.HasValue))
          {
            nullable = statusCode;
            HttpStatusCode httpStatusCode3 = HttpStatusCode.NotFound;
            if (!(nullable.GetValueOrDefault() == httpStatusCode3 & nullable.HasValue) || subStatusCode == SubStatusCodes.PartitionKeyRangeGone)
              return;
          }
        }
      }
      if (request.ResourceType == ResourceType.Collection && request.OperationType == Microsoft.Azure.Documents.OperationType.Delete)
      {
        this.sessionContainer.ClearTokenByResourceId(!request.IsNameBased ? request.ResourceId : responseHeaders["x-ms-content-path"]);
      }
      else
      {
        this.sessionContainer.SetSessionToken(request, responseHeaders);
        PartitionKeyRange partitionKeyRange = request.RequestContext.ResolvedPartitionKeyRange;
        string responseHeader = responseHeaders["x-ms-documentdb-partitionkeyrangeid"];
        if (partitionKeyRange == null || string.IsNullOrEmpty(responseHeader) || string.IsNullOrEmpty(request.RequestContext.ResolvedCollectionRid) || responseHeader.Equals(partitionKeyRange.Id, StringComparison.OrdinalIgnoreCase))
          return;
        PartitionKeyRange keyRangeByIdAsync = await this.partitionKeyRangeCache.TryGetPartitionKeyRangeByIdAsync(request.RequestContext.ResolvedCollectionRid, responseHeader, (ITrace) NoOpTrace.Singleton, true);
      }
    }

    internal static async Task ApplySessionTokenAsync(
      DocumentServiceRequest request,
      ConsistencyLevel defaultConsistencyLevel,
      ISessionContainer sessionContainer,
      PartitionKeyRangeCache partitionKeyRangeCache,
      CollectionCache clientCollectionCache,
      IGlobalEndpointManager globalEndpointManager)
    {
      if (request.Headers == null)
        return;
      if (GatewayStoreModel.IsMasterOperation(request.ResourceType, request.OperationType))
      {
        if (string.IsNullOrEmpty(request.Headers["x-ms-session-token"]))
          return;
        request.Headers.Remove("x-ms-session-token");
      }
      else
      {
        if (!string.IsNullOrEmpty(request.Headers["x-ms-session-token"]))
          return;
        string header = request.Headers["x-ms-consistency-level"];
        bool flag1 = request.IsReadOnlyRequest || request.OperationType == Microsoft.Azure.Documents.OperationType.Batch;
        bool flag2 = !string.IsNullOrEmpty(header) & flag1;
        int num1 = flag2 || defaultConsistencyLevel != ConsistencyLevel.Session ? (!flag2 ? 0 : (string.Equals(header, GatewayStoreModel.sessionConsistencyAsString, StringComparison.OrdinalIgnoreCase) ? 1 : 0)) : 1;
        bool flag3 = globalEndpointManager.CanUseMultipleWriteLocations(request);
        if (num1 == 0 || !flag1 && !flag3)
          return;
        bool flag4;
        string str1;
        (await GatewayStoreModel.TryResolveSessionTokenAsync(request, sessionContainer, partitionKeyRangeCache, clientCollectionCache)).Deconstruct<bool, string>(out flag4, out str1);
        int num2 = flag4 ? 1 : 0;
        string str2 = str1;
        if (num2 == 0 || string.IsNullOrEmpty(str2))
          return;
        request.Headers["x-ms-session-token"] = str2;
      }
    }

    internal static async Task<Tuple<bool, string>> TryResolveSessionTokenAsync(
      DocumentServiceRequest request,
      ISessionContainer sessionContainer,
      PartitionKeyRangeCache partitionKeyRangeCache,
      CollectionCache clientCollectionCache)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (sessionContainer == null)
        throw new ArgumentNullException(nameof (sessionContainer));
      if (partitionKeyRangeCache == null)
        throw new ArgumentNullException(nameof (partitionKeyRangeCache));
      if (clientCollectionCache == null)
        throw new ArgumentNullException(nameof (clientCollectionCache));
      if (request.ResourceType.IsPartitioned())
      {
        bool flag;
        PartitionKeyRange partitionKeyRange1;
        (await GatewayStoreModel.TryResolvePartitionKeyRangeAsync(request, sessionContainer, partitionKeyRangeCache, clientCollectionCache, false)).Deconstruct<bool, PartitionKeyRange>(out flag, out partitionKeyRange1);
        int num = flag ? 1 : 0;
        PartitionKeyRange partitionKeyRange2 = partitionKeyRange1;
        if (num != 0 && sessionContainer is SessionContainer sessionContainer1)
        {
          request.RequestContext.ResolvedPartitionKeyRange = partitionKeyRange2;
          string str = sessionContainer1.ResolvePartitionLocalSessionTokenForGateway(request, partitionKeyRange2.Id);
          if (!string.IsNullOrEmpty(str))
            return new Tuple<bool, string>(true, str);
        }
      }
      return new Tuple<bool, string>(false, (string) null);
    }

    private static async Task<Tuple<bool, PartitionKeyRange>> TryResolvePartitionKeyRangeAsync(
      DocumentServiceRequest request,
      ISessionContainer sessionContainer,
      PartitionKeyRangeCache partitionKeyRangeCache,
      CollectionCache clientCollectionCache,
      bool refreshCache)
    {
      if (refreshCache)
      {
        request.ForceMasterRefresh = true;
        request.ForceNameCacheRefresh = true;
      }
      PartitionKeyRange partitonKeyRange = (PartitionKeyRange) null;
      ContainerProperties collection = await clientCollectionCache.ResolveCollectionAsync(request, CancellationToken.None, (ITrace) NoOpTrace.Singleton);
      string partitionKeyString = request.Headers["x-ms-documentdb-partitionkey"];
      if (partitionKeyString != null)
      {
        CollectionRoutingMap collectionRoutingMap = await partitionKeyRangeCache.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, request, (ITrace) NoOpTrace.Singleton);
        if (refreshCache && collectionRoutingMap != null)
          collectionRoutingMap = await partitionKeyRangeCache.TryLookupAsync(collection.ResourceId, collectionRoutingMap, request, (ITrace) NoOpTrace.Singleton);
        if (collectionRoutingMap != null)
          partitonKeyRange = AddressResolver.TryResolveServerPartitionByPartitionKey(request, partitionKeyString, false, collection, collectionRoutingMap);
      }
      else if (request.PartitionKeyRangeIdentity != null)
        partitonKeyRange = await partitionKeyRangeCache.TryGetPartitionKeyRangeByIdAsync(collection.ResourceId, request.PartitionKeyRangeIdentity.PartitionKeyRangeId, (ITrace) NoOpTrace.Singleton, refreshCache);
      else if (request.RequestContext.ResolvedPartitionKeyRange != null)
        partitonKeyRange = request.RequestContext.ResolvedPartitionKeyRange;
      if (partitonKeyRange != null)
        return new Tuple<bool, PartitionKeyRange>(true, partitonKeyRange);
      return refreshCache ? new Tuple<bool, PartitionKeyRange>(false, (PartitionKeyRange) null) : await GatewayStoreModel.TryResolvePartitionKeyRangeAsync(request, sessionContainer, partitionKeyRangeCache, clientCollectionCache, true);
    }

    internal static bool IsMasterOperation(ResourceType resourceType, Microsoft.Azure.Documents.OperationType operationType) => ReplicatedResourceClient.IsMasterResource(resourceType) || GatewayStoreModel.IsStoredProcedureCrudOperation(resourceType, operationType) || resourceType == ResourceType.Trigger || resourceType == ResourceType.UserDefinedFunction || operationType == Microsoft.Azure.Documents.OperationType.QueryPlan;

    internal static bool IsStoredProcedureCrudOperation(
      ResourceType resourceType,
      Microsoft.Azure.Documents.OperationType operationType)
    {
      return resourceType == ResourceType.StoredProcedure && operationType != Microsoft.Azure.Documents.OperationType.ExecuteJavaScript;
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.gatewayStoreClient == null)
        return;
      try
      {
        this.gatewayStoreClient.Dispose();
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("Exception {0} thrown during dispose of HttpClient, this could happen if there are inflight request during the dispose of client", (object) ex);
      }
      this.gatewayStoreClient = (GatewayStoreClient) null;
    }

    private Uri GetEntityUri(DocumentServiceRequest entity)
    {
      string header = entity.Headers["Content-Location"];
      return !string.IsNullOrEmpty(header) ? new Uri(this.endpointManager.ResolveServiceEndpoint(entity), new Uri(header).AbsolutePath) : new Uri(this.endpointManager.ResolveServiceEndpoint(entity), PathsHelper.GeneratePath(entity.ResourceType, entity, false));
    }

    private Uri GetFeedUri(DocumentServiceRequest request) => new Uri(this.endpointManager.ResolveServiceEndpoint(request), PathsHelper.GeneratePath(request.ResourceType, request, true));
  }
}
