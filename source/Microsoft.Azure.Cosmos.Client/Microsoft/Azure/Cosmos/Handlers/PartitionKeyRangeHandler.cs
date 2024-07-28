// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Handlers.PartitionKeyRangeHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Handlers
{
  internal class PartitionKeyRangeHandler : RequestHandler
  {
    private readonly CosmosClient client;
    private readonly PartitionRoutingHelper partitionRoutingHelper;

    public PartitionKeyRangeHandler(
      CosmosClient client,
      PartitionRoutingHelper partitionRoutingHelper = null)
    {
      this.client = client ?? throw new ArgumentNullException(nameof (client));
      this.partitionRoutingHelper = partitionRoutingHelper ?? new PartitionRoutingHelper();
    }

    public override async Task<ResponseMessage> SendAsync(
      RequestMessage request,
      CancellationToken cancellationToken)
    {
      PartitionKeyRangeHandler partitionKeyRangeHandler = this;
      using (ITrace childTrace = request.Trace.StartChild(partitionKeyRangeHandler.FullHandlerName, TraceComponent.RequestHandler, TraceLevel.Info))
      {
        request.Trace = childTrace;
        ResponseMessage response = (ResponseMessage) null;
        string originalContinuation = request.Headers.ContinuationToken;
        try
        {
          RntbdConstants.RntdbEnumerationDirection rntdbEnumerationDirection = RntbdConstants.RntdbEnumerationDirection.Forward;
          object obj;
          if (request.Properties.TryGetValue("x-ms-enumeration-direction", out obj))
            rntdbEnumerationDirection = (byte) obj == (byte) 2 ? RntbdConstants.RntdbEnumerationDirection.Reverse : RntbdConstants.RntdbEnumerationDirection.Forward;
          request.Headers.Remove("x-ms-documentdb-query-iscontinuationexpected");
          request.Headers.Add("x-ms-documentdb-query-iscontinuationexpected", bool.TrueString);
          object effectivePartitionKey1;
          if (!request.Properties.TryGetValue("x-ms-start-epk-string", out effectivePartitionKey1))
            effectivePartitionKey1 = (object) PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey;
          object effectivePartitionKey2;
          if (!request.Properties.TryGetValue("x-ms-end-epk-string", out effectivePartitionKey2))
            effectivePartitionKey2 = (object) PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey;
          if (effectivePartitionKey1 == null)
            effectivePartitionKey1 = (object) PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey;
          if (effectivePartitionKey2 == null)
            effectivePartitionKey2 = (object) PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey;
          List<Range<string>> providedRanges = new List<Range<string>>()
          {
            new Range<string>((string) effectivePartitionKey1, (string) effectivePartitionKey2, true, false)
          };
          DocumentServiceRequest serviceRequest = request.ToDocumentServiceRequest();
          PartitionKeyRangeCache routingMapProvider = await partitionKeyRangeHandler.client.DocumentClient.GetPartitionKeyRangeCacheAsync(childTrace);
          CollectionCache collectionCache = (CollectionCache) await partitionKeyRangeHandler.client.DocumentClient.GetCollectionCacheAsync(childTrace);
          ContainerProperties collectionFromCache = await collectionCache.ResolveCollectionAsync(serviceRequest, CancellationToken.None, childTrace);
          List<CompositeContinuationToken> suppliedTokens;
          Range<string> rangeFromContinuationToken = partitionKeyRangeHandler.partitionRoutingHelper.ExtractPartitionKeyRangeFromContinuationToken(serviceRequest.Headers, out suppliedTokens);
          PartitionRoutingHelper.ResolvedRangeInfo resolvedRangeInfo = await partitionKeyRangeHandler.partitionRoutingHelper.TryGetTargetRangeFromContinuationTokenRangeAsync((IReadOnlyList<Range<string>>) providedRanges, (IRoutingMapProvider) routingMapProvider, collectionFromCache.ResourceId, rangeFromContinuationToken, suppliedTokens, childTrace, rntdbEnumerationDirection);
          if (serviceRequest.IsNameBased && resolvedRangeInfo.ResolvedRange == null && resolvedRangeInfo.ContinuationTokens == null)
          {
            serviceRequest.ForceNameCacheRefresh = true;
            collectionFromCache = await collectionCache.ResolveCollectionAsync(serviceRequest, CancellationToken.None, childTrace);
            resolvedRangeInfo = await partitionKeyRangeHandler.partitionRoutingHelper.TryGetTargetRangeFromContinuationTokenRangeAsync((IReadOnlyList<Range<string>>) providedRanges, (IRoutingMapProvider) routingMapProvider, collectionFromCache.ResourceId, rangeFromContinuationToken, suppliedTokens, childTrace, rntdbEnumerationDirection);
          }
          if (resolvedRangeInfo.ResolvedRange == null && resolvedRangeInfo.ContinuationTokens == null)
            return new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": Was not able to get queryRoutingInfo even after resolve collection async with force name cache refresh to the following collectionRid: " + collectionFromCache.ResourceId + " with the supplied tokens: " + JsonConvert.SerializeObject((object) suppliedTokens)).ToCosmosResponseMessage(request);
          serviceRequest.RouteTo(new PartitionKeyRangeIdentity(collectionFromCache.ResourceId, resolvedRangeInfo.ResolvedRange.Id));
          // ISSUE: reference to a compiler-generated method
          response = await partitionKeyRangeHandler.\u003C\u003En__0(request, cancellationToken);
          if (!response.IsSuccessStatusCode)
            partitionKeyRangeHandler.SetOriginalContinuationToken(request, response, originalContinuation);
          else if (!await partitionKeyRangeHandler.partitionRoutingHelper.TryAddPartitionKeyRangeToContinuationTokenAsync(response.Headers.CosmosMessageHeaders.INameValueCollection, (IReadOnlyList<Range<string>>) providedRanges, (IRoutingMapProvider) routingMapProvider, collectionFromCache.ResourceId, resolvedRangeInfo, childTrace, rntdbEnumerationDirection))
            return new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": Call to TryAddPartitionKeyRangeToContinuationTokenAsync failed to the following collectionRid: " + collectionFromCache.ResourceId + " with the supplied tokens: " + JsonConvert.SerializeObject((object) suppliedTokens)).ToCosmosResponseMessage(request);
          return response;
        }
        catch (DocumentClientException ex)
        {
          RequestMessage requestMessage = request;
          ResponseMessage cosmosResponseMessage = ex.ToCosmosResponseMessage(requestMessage);
          partitionKeyRangeHandler.SetOriginalContinuationToken(request, cosmosResponseMessage, originalContinuation);
          return cosmosResponseMessage;
        }
        catch (CosmosException ex)
        {
          RequestMessage request1 = request;
          ResponseMessage cosmosResponseMessage = ex.ToCosmosResponseMessage(request1);
          partitionKeyRangeHandler.SetOriginalContinuationToken(request, cosmosResponseMessage, originalContinuation);
          return cosmosResponseMessage;
        }
        catch (AggregateException ex)
        {
          partitionKeyRangeHandler.SetOriginalContinuationToken(request, response, originalContinuation);
          Exception exception = ex.Flatten().InnerExceptions.FirstOrDefault<Exception>((Func<Exception, bool>) (innerEx => innerEx is DocumentClientException));
          if (exception != null)
            return ((DocumentClientException) exception).ToCosmosResponseMessage(request);
          throw;
        }
      }
    }

    private void SetOriginalContinuationToken(
      RequestMessage request,
      ResponseMessage response,
      string originalContinuation)
    {
      request.Headers.ContinuationToken = originalContinuation;
      if (response == null)
        return;
      response.Headers.ContinuationToken = originalContinuation;
    }
  }
}
