// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Handlers.RequestInvokerHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Handlers
{
  internal class RequestInvokerHandler : RequestHandler
  {
    private static readonly HttpMethod httpPatchMethod = new HttpMethod("PATCH");
    private static (bool, ResponseMessage) clientIsValid = (false, (ResponseMessage) null);
    private readonly CosmosClient client;
    private readonly Microsoft.Azure.Cosmos.ConsistencyLevel? RequestedClientConsistencyLevel;
    private bool? IsLocalQuorumConsistency;
    private Microsoft.Azure.Cosmos.ConsistencyLevel? AccountConsistencyLevel;

    public RequestInvokerHandler(
      CosmosClient client,
      Microsoft.Azure.Cosmos.ConsistencyLevel? requestedClientConsistencyLevel)
    {
      this.client = client;
      this.RequestedClientConsistencyLevel = requestedClientConsistencyLevel;
    }

    public override async Task<ResponseMessage> SendAsync(
      RequestMessage request,
      CancellationToken cancellationToken)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      request.RequestOptions?.PopulateRequestOptions(request);
      if (RequestInvokerHandler.ShouldSetNoContentResponseHeaders(request.RequestOptions, this.client.ClientOptions, request.OperationType, request.ResourceType))
        request.Headers.Add("Prefer", "return=minimal");
      await this.ValidateAndSetConsistencyLevelAsync(request);
      (bool flag, ResponseMessage responseMessage) = await this.EnsureValidClientAsync(request, request.Trace);
      if (flag)
        return responseMessage;
      await request.AssertPartitioningDetailsAsync(this.client, cancellationToken, request.Trace);
      this.FillMultiMasterContext(request);
      return await base.SendAsync(request, cancellationToken);
    }

    public virtual async Task<T> SendAsync<T>(
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal cosmosContainerCore,
      FeedRange feedRange,
      Stream streamPayload,
      Action<RequestMessage> requestEnricher,
      Func<ResponseMessage, T> responseCreator,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (responseCreator == null)
        throw new ArgumentNullException(nameof (responseCreator));
      return responseCreator(await this.SendAsync(resourceUri, resourceType, operationType, requestOptions, cosmosContainerCore, feedRange, streamPayload, requestEnricher, trace, cancellationToken));
    }

    public virtual async Task<ResponseMessage> SendAsync(
      string resourceUriString,
      ResourceType resourceType,
      OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal cosmosContainerCore,
      FeedRange feedRange,
      Stream streamPayload,
      Action<RequestMessage> requestEnricher,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      RequestInvokerHandler requestInvokerHandler = this;
      if (resourceUriString == null)
        throw new ArgumentNullException(nameof (resourceUriString));
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      ActivityScope activityScope = ActivityScope.CreateIfDefaultActivityId();
      using (ITrace childTrace = trace.StartChild(requestInvokerHandler.FullHandlerName, TraceComponent.RequestHandler, TraceLevel.Info))
      {
        try
        {
          RequestMessage request = new RequestMessage(RequestInvokerHandler.GetHttpMethod(resourceType, operationType, streamPayload != null), resourceUriString, childTrace)
          {
            OperationType = operationType,
            ResourceType = resourceType,
            RequestOptions = requestOptions,
            Content = streamPayload
          };
          request.Headers.SDKSupportedCapabilities = Microsoft.Azure.Cosmos.Headers.SDKSUPPORTEDCAPABILITIES;
          switch (feedRange)
          {
            case null:
label_32:
              switch (operationType)
              {
                case OperationType.Patch:
                  request.Headers.ContentType = "application/json-patch+json";
                  break;
                case OperationType.Upsert:
                  request.Headers.IsUpsert = bool.TrueString;
                  break;
              }
              if (ChangeFeedHelper.IsChangeFeedWithQueryRequest(operationType, streamPayload != null))
              {
                request.Headers.Add("x-ms-documentdb-isquery", bool.TrueString);
                request.Headers.Add("Content-Type", "application/query+json");
              }
              if (cosmosContainerCore != null)
              {
                request.ContainerId = cosmosContainerCore?.Id;
                request.DatabaseId = cosmosContainerCore?.Database.Id;
              }
              Action<RequestMessage> action = requestEnricher;
              if (action != null)
                action(request);
              return await requestInvokerHandler.SendAsync(request, cancellationToken);
            case FeedRangePartitionKey rangePartitionKey:
              if (cosmosContainerCore == null && (ValueType) rangePartitionKey.PartitionKey == (ValueType) Microsoft.Azure.Cosmos.PartitionKey.None)
                throw new ArgumentException("cosmosContainerCore can not be null with partition key as PartitionKey.None");
              if (rangePartitionKey.PartitionKey.IsNone)
              {
                try
                {
                  request.Headers.PartitionKey = (await cosmosContainerCore.GetNonePartitionKeyValueAsync(childTrace, cancellationToken)).ToJsonString();
                  goto case null;
                }
                catch (DocumentClientException ex)
                {
                  RequestMessage requestMessage = request;
                  return ex.ToCosmosResponseMessage(requestMessage);
                }
                catch (CosmosException ex)
                {
                  RequestMessage request1 = request;
                  return ex.ToCosmosResponseMessage(request1);
                }
              }
              else
              {
                request.Headers.PartitionKey = rangePartitionKey.PartitionKey.ToJsonString();
                goto case null;
              }
            case FeedRangeEpk feedRangeEpk:
              ContainerProperties collectionFromCache;
              try
              {
                if (cosmosContainerCore == null)
                  throw new ArgumentException("The container core can not be null for FeedRangeEpk");
                collectionFromCache = await cosmosContainerCore.GetCachedContainerPropertiesAsync(false, childTrace, cancellationToken);
              }
              catch (CosmosException ex)
              {
                RequestMessage request2 = request;
                return ex.ToCosmosResponseMessage(request2);
              }
              IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await (await requestInvokerHandler.client.DocumentClient.GetPartitionKeyRangeCacheAsync(childTrace)).TryGetOverlappingRangesAsync(collectionFromCache.ResourceId, feedRangeEpk.Range, childTrace, false);
              if (overlappingRangesAsync == null)
                return new CosmosException("Stale cache for rid '" + collectionFromCache.ResourceId + "'", HttpStatusCode.NotFound, 0, Guid.Empty.ToString(), 0.0).ToCosmosResponseMessage(request);
              if (overlappingRangesAsync.Count > 1)
                return new CosmosException(string.Format("Epk Range: {0} is gone.", (object) feedRangeEpk.Range), HttpStatusCode.Gone, 1002, Guid.NewGuid().ToString(), 0.0).ToCosmosResponseMessage(request);
              Range<string> range = overlappingRangesAsync[0].ToRange();
              if (range.Min == feedRangeEpk.Range.Min && range.Max == feedRangeEpk.Range.Max)
              {
                request.PartitionKeyRangeId = new PartitionKeyRangeIdentity(overlappingRangesAsync[0].Id);
              }
              else
              {
                request.PartitionKeyRangeId = new PartitionKeyRangeIdentity(overlappingRangesAsync[0].Id);
                request.Headers.ReadFeedKeyType = RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKeyRange.ToString();
                request.Headers.StartEpk = feedRangeEpk.Range.Min;
                request.Headers.EndEpk = feedRangeEpk.Range.Max;
              }
              collectionFromCache = (ContainerProperties) null;
              break;
            default:
              RequestMessage requestMessage1 = request;
              PartitionKeyRangeIdentity keyRangeIdentity = feedRange is FeedRangePartitionKeyRange partitionKeyRange ? new PartitionKeyRangeIdentity(partitionKeyRange.PartitionKeyRangeId) : throw new InvalidOperationException(string.Format("Unknown feed range type: '{0}'.", (object) feedRange.GetType()));
              requestMessage1.PartitionKeyRangeId = keyRangeIdentity;
              break;
          }
          feedRangeEpk = (FeedRangeEpk) null;
          goto label_32;
        }
        finally
        {
          activityScope?.Dispose();
        }
      }
    }

    internal static HttpMethod GetHttpMethod(
      ResourceType resourceType,
      OperationType operationType,
      bool hasPayload = false)
    {
      if (operationType == OperationType.Create || operationType == OperationType.Upsert || operationType == OperationType.Query || operationType == OperationType.SqlQuery || operationType == OperationType.QueryPlan || operationType == OperationType.Batch || operationType == OperationType.ExecuteJavaScript || operationType == OperationType.CompleteUserTransaction || resourceType == ResourceType.PartitionKey && operationType == OperationType.Delete || ChangeFeedHelper.IsChangeFeedWithQueryRequest(operationType, hasPayload))
        return HttpMethod.Post;
      switch (operationType)
      {
        case OperationType.Patch:
          return RequestInvokerHandler.httpPatchMethod;
        case OperationType.Read:
        case OperationType.ReadFeed:
          return HttpMethod.Get;
        case OperationType.Delete:
          return HttpMethod.Delete;
        case OperationType.Replace:
        case OperationType.CollectionTruncate:
          return HttpMethod.Put;
        default:
          throw new NotImplementedException();
      }
    }

    private async Task<(bool, ResponseMessage)> EnsureValidClientAsync(
      RequestMessage request,
      ITrace trace)
    {
      try
      {
        await this.client.DocumentClient.EnsureValidClientAsync(trace);
        return RequestInvokerHandler.clientIsValid;
      }
      catch (DocumentClientException ex)
      {
        return (true, ex.ToCosmosResponseMessage(request));
      }
    }

    private void FillMultiMasterContext(RequestMessage request)
    {
      if (!this.client.DocumentClient.UseMultipleWriteLocations)
        return;
      request.Headers.Set("x-ms-cosmos-allow-tentative-writes", bool.TrueString);
    }

    private async Task ValidateAndSetConsistencyLevelAsync(RequestMessage requestMessage)
    {
      Microsoft.Azure.Cosmos.ConsistencyLevel? consistencyLevel = new Microsoft.Azure.Cosmos.ConsistencyLevel?();
      RequestOptions requestOptions = requestMessage.RequestOptions;
      if (requestOptions != null && requestOptions.BaseConsistencyLevel.HasValue)
        consistencyLevel = requestOptions.BaseConsistencyLevel;
      else if (this.RequestedClientConsistencyLevel.HasValue)
        consistencyLevel = this.RequestedClientConsistencyLevel;
      if (!consistencyLevel.HasValue)
        return;
      if (!this.AccountConsistencyLevel.HasValue)
        this.AccountConsistencyLevel = new Microsoft.Azure.Cosmos.ConsistencyLevel?(await this.client.GetAccountConsistencyLevelAsync());
      if (!this.IsLocalQuorumConsistency.HasValue)
        this.IsLocalQuorumConsistency = new bool?(this.client.ClientOptions.EnableUpgradeConsistencyToLocalQuorum);
      if (!ValidationHelpers.IsValidConsistencyLevelOverwrite(this.AccountConsistencyLevel.Value, consistencyLevel.Value, this.IsLocalQuorumConsistency.Value, requestMessage.OperationType, requestMessage.ResourceType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidConsistencyLevel, (object) consistencyLevel.Value.ToString(), (object) this.AccountConsistencyLevel));
      requestMessage.Headers.ConsistencyLevel = consistencyLevel.Value.ToString();
    }

    internal static bool ShouldSetNoContentResponseHeaders(
      RequestOptions requestOptions,
      CosmosClientOptions clientOptions,
      OperationType operationType,
      ResourceType resourceType)
    {
      if (resourceType != ResourceType.Document)
        return false;
      switch (requestOptions)
      {
        case null:
          return RequestInvokerHandler.IsClientNoResponseSet(clientOptions, operationType);
        case ItemRequestOptions itemRequestOptions1:
          return itemRequestOptions1.EnableContentResponseOnWrite.HasValue ? RequestInvokerHandler.IsItemNoRepsonseSet(itemRequestOptions1.EnableContentResponseOnWrite.Value, operationType) : RequestInvokerHandler.IsClientNoResponseSet(clientOptions, operationType);
        case TransactionalBatchItemRequestOptions itemRequestOptions2:
          return itemRequestOptions2.EnableContentResponseOnWrite.HasValue ? RequestInvokerHandler.IsItemNoRepsonseSet(itemRequestOptions2.EnableContentResponseOnWrite.Value, operationType) : RequestInvokerHandler.IsClientNoResponseSet(clientOptions, operationType);
        default:
          return false;
      }
    }

    private static bool IsItemNoRepsonseSet(
      bool enableContentResponseOnWrite,
      OperationType operationType)
    {
      if (enableContentResponseOnWrite)
        return false;
      return operationType == OperationType.Create || operationType == OperationType.Replace || operationType == OperationType.Upsert || operationType == OperationType.Patch;
    }

    private static bool IsClientNoResponseSet(
      CosmosClientOptions clientOptions,
      OperationType operationType)
    {
      return clientOptions != null && clientOptions.EnableContentResponseOnWrite.HasValue && RequestInvokerHandler.IsItemNoRepsonseSet(clientOptions.EnableContentResponseOnWrite.Value, operationType);
    }
  }
}
