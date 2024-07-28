// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ReadManyQueryHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Serializer;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ReadManyQueryHelper : ReadManyHelper
  {
    private readonly List<string> partitionKeySelectors;
    private readonly PartitionKeyDefinition partitionKeyDefinition;
    private readonly int maxConcurrency = Environment.ProcessorCount * 10;
    private readonly int maxItemsPerQuery = 1000;
    private readonly ContainerCore container;
    private readonly CosmosClientContext clientContext;

    public ReadManyQueryHelper(
      PartitionKeyDefinition partitionKeyDefinition,
      ContainerCore container)
    {
      this.partitionKeyDefinition = partitionKeyDefinition;
      this.partitionKeySelectors = this.CreatePkSelectors(partitionKeyDefinition);
      this.container = container;
      this.clientContext = container.ClientContext;
    }

    public override async Task<ResponseMessage> ExecuteReadManyRequestAsync(
      IReadOnlyList<(string, PartitionKey)> items,
      ReadManyRequestOptions readManyRequestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      string resourceId;
      IDictionary<PartitionKeyRange, List<(string, PartitionKey)>> itemListMapAsync;
      try
      {
        resourceId = await this.container.GetCachedRIDAsync(cancellationToken);
        itemListMapAsync = await this.CreatePartitionKeyRangeItemListMapAsync(items, trace, cancellationToken);
      }
      catch (CosmosException ex)
      {
        return ex.ToCosmosResponseMessage((RequestMessage) null);
      }
      return this.CombineStreamsFromQueryResponses(await this.ReadManyTaskHelperAsync(itemListMapAsync, readManyRequestOptions, trace, cancellationToken), resourceId, trace);
    }

    public override async Task<FeedResponse<T>> ExecuteReadManyRequestAsync<T>(
      IReadOnlyList<(string, PartitionKey)> items,
      ReadManyRequestOptions readManyRequestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.CombineFeedResponseFromQueryResponses<T>(await this.ReadManyTaskHelperAsync(await this.CreatePartitionKeyRangeItemListMapAsync(items, trace, cancellationToken), readManyRequestOptions, trace, cancellationToken), trace);
    }

    internal async Task<List<ResponseMessage>[]> ReadManyTaskHelperAsync(
      IDictionary<PartitionKeyRange, List<(string, PartitionKey)>> partitionKeyRangeItemMap,
      ReadManyRequestOptions readManyRequestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      SemaphoreSlim semaphore = new SemaphoreSlim(this.maxConcurrency, this.maxConcurrency);
      List<Task<List<ResponseMessage>>> tasks = new List<Task<List<ResponseMessage>>>();
      foreach (KeyValuePair<PartitionKeyRange, List<(string, PartitionKey)>> partitionKeyRangeItem in (IEnumerable<KeyValuePair<PartitionKeyRange, List<(string, PartitionKey)>>>) partitionKeyRangeItemMap)
      {
        KeyValuePair<PartitionKeyRange, List<(string, PartitionKey)>> entry = partitionKeyRangeItem;
        for (int startIndex = 0; startIndex < entry.Value.Count; startIndex += this.maxItemsPerQuery)
        {
          await semaphore.WaitAsync();
          ITrace childTrace = trace.StartChild("Execute query for a partitionkeyrange", TraceComponent.Query, TraceLevel.Info);
          int indexCopy = startIndex;
          tasks.Add(Task.Run<List<ResponseMessage>>((Func<Task<List<ResponseMessage>>>) (async () =>
          {
            List<ResponseMessage> forPartitionAsync;
            try
            {
              forPartitionAsync = await this.GenerateStreamResponsesForPartitionAsync(this.partitionKeySelectors.Count != 1 || !(this.partitionKeySelectors[0] == "[\"id\"]") ? this.CreateReadManyQueryDefinitionForOther(entry.Value, indexCopy) : this.CreateReadManyQueryDefinitionForId(entry.Value, indexCopy), entry.Key, readManyRequestOptions, childTrace, cancellationToken);
            }
            finally
            {
              semaphore.Release();
              childTrace.Dispose();
            }
            return forPartitionAsync;
          })));
        }
      }
      List<ResponseMessage>[] responseMessageListArray = await Task.WhenAll<List<ResponseMessage>>((IEnumerable<Task<List<ResponseMessage>>>) tasks);
      tasks = (List<Task<List<ResponseMessage>>>) null;
      return responseMessageListArray;
    }

    private async Task<IDictionary<PartitionKeyRange, List<(string, PartitionKey)>>> CreatePartitionKeyRangeItemListMapAsync(
      IReadOnlyList<(string, PartitionKey)> items,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CollectionRoutingMap collectionRoutingMap = await this.container.GetRoutingMapAsync(cancellationToken);
      IDictionary<PartitionKeyRange, List<(string, PartitionKey)>> partitionKeyRangeItemMap = (IDictionary<PartitionKeyRange, List<(string, PartitionKey)>>) new Dictionary<PartitionKeyRange, List<(string, PartitionKey)>>();
      foreach ((_, _) in (IEnumerable<(string, PartitionKey)>) items)
      {
        (string, PartitionKey) item;
        PartitionKeyRange effectivePartitionKey = collectionRoutingMap.GetRangeByEffectivePartitionKey((await this.GetPartitionKeyInternalAsync(item.Item2, trace, cancellationToken)).GetEffectivePartitionKeyString(this.partitionKeyDefinition));
        List<(string, PartitionKey)> valueTupleList1;
        if (partitionKeyRangeItemMap.TryGetValue(effectivePartitionKey, out valueTupleList1))
        {
          valueTupleList1.Add(item);
        }
        else
        {
          List<(string, PartitionKey)> valueTupleList2 = new List<(string, PartitionKey)>()
          {
            item
          };
          partitionKeyRangeItemMap[effectivePartitionKey] = valueTupleList2;
        }
        item = ();
      }
      IDictionary<PartitionKeyRange, List<(string, PartitionKey)>> itemListMapAsync = partitionKeyRangeItemMap;
      collectionRoutingMap = (CollectionRoutingMap) null;
      partitionKeyRangeItemMap = (IDictionary<PartitionKeyRange, List<(string, PartitionKey)>>) null;
      return itemListMapAsync;
    }

    private ResponseMessage CombineStreamsFromQueryResponses(
      List<ResponseMessage>[] queryResponses,
      string collectionRid,
      ITrace trace)
    {
      List<CosmosElement> cosmosElementList = new List<CosmosElement>();
      double num = 0.0;
      foreach (List<ResponseMessage> queryResponse1 in queryResponses)
      {
        if (queryResponse1 != null)
        {
          foreach (ResponseMessage responseMessage in queryResponse1)
          {
            using (responseMessage)
            {
              if (!responseMessage.IsSuccessStatusCode)
                return new ResponseMessage(responseMessage.StatusCode, (RequestMessage) null, responseMessage.Headers, responseMessage.CosmosException, trace);
              if (!(responseMessage is QueryResponse queryResponse2))
                throw new InvalidOperationException("Read Many is being used with Query");
              cosmosElementList.AddRange((IEnumerable<CosmosElement>) queryResponse2.CosmosElements);
              num += queryResponse2.Headers.RequestCharge;
            }
          }
        }
      }
      return new ResponseMessage(HttpStatusCode.OK)
      {
        Content = (Stream) CosmosElementSerializer.ToStream(collectionRid, (IEnumerable<CosmosElement>) cosmosElementList, ResourceType.Document),
        Trace = trace,
        Headers = {
          RequestCharge = num
        }
      };
    }

    private FeedResponse<T> CombineFeedResponseFromQueryResponses<T>(
      List<ResponseMessage>[] queryResponses,
      ITrace trace)
    {
      int resourceCount = 0;
      double num = 0.0;
      List<FeedResponse<T>> queryResponses1 = new List<FeedResponse<T>>();
      foreach (List<ResponseMessage> queryResponse in queryResponses)
      {
        if (queryResponse != null)
        {
          foreach (ResponseMessage responseMessage in queryResponse)
          {
            using (responseMessage)
            {
              responseMessage.EnsureSuccessStatusCode();
              FeedResponse<T> userTypeResponse = this.clientContext.ResponseFactory.CreateQueryFeedUserTypeResponse<T>(responseMessage);
              resourceCount += userTypeResponse.Count;
              num += userTypeResponse.RequestCharge;
              queryResponses1.Add(userTypeResponse);
            }
          }
        }
      }
      Headers responseMessageHeaders = new Headers()
      {
        RequestCharge = num
      };
      return (FeedResponse<T>) new ReadFeedResponse<T>(HttpStatusCode.OK, (IEnumerable<T>) new ReadManyQueryHelper.ReadManyFeedResponseEnumerable<T>(queryResponses1), resourceCount, responseMessageHeaders, (CosmosDiagnostics) new CosmosTraceDiagnostics(trace), (RequestMessage) null);
    }

    private QueryDefinition CreateReadManyQueryDefinitionForId(
      List<(string, PartitionKey)> items,
      int startIndex)
    {
      int num = Math.Min(items.Count, startIndex + this.maxItemsPerQuery);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("SELECT * FROM c WHERE c.id IN ( ");
      for (int index = startIndex; index < num; ++index)
      {
        stringBuilder.Append("'" + items[index].Item1 + "'");
        if (index < num - 1)
          stringBuilder.Append(",");
      }
      stringBuilder.Append(" )");
      return new QueryDefinition(stringBuilder.ToString());
    }

    private QueryDefinition CreateReadManyQueryDefinitionForOther(
      List<(string, PartitionKey)> items,
      int startIndex)
    {
      int num = Math.Min(items.Count, startIndex + this.maxItemsPerQuery);
      StringBuilder stringBuilder = new StringBuilder();
      SqlParameterCollection parameters = new SqlParameterCollection();
      stringBuilder.Append("SELECT * FROM c WHERE ( ");
      for (int index1 = startIndex; index1 < num; ++index1)
      {
        string str = "@param_pk" + index1.ToString();
        string name1 = "@param_id" + index1.ToString();
        parameters.Add(new SqlParameter(name1, (object) items[index1].Item1));
        stringBuilder.Append("( ");
        stringBuilder.Append("c.id = ");
        stringBuilder.Append(name1);
        if (items[index1].Item2.IsNone)
        {
          foreach (string partitionKeySelector in this.partitionKeySelectors)
          {
            stringBuilder.Append(" AND ");
            stringBuilder.Append("IS_DEFINED(c");
            stringBuilder.Append(partitionKeySelector);
            stringBuilder.Append(") = false");
          }
        }
        else
        {
          object[] objectArray = items[index1].Item2.InternalKey.ToObjectArray();
          if (objectArray.Length != this.partitionKeyDefinition.Paths.Count)
            throw new ArgumentException("Number of components in the partition key value does not match the definition.");
          for (int index2 = 0; index2 < this.partitionKeySelectors.Count; ++index2)
          {
            stringBuilder.Append(" AND ");
            stringBuilder.Append("c");
            stringBuilder.Append(this.partitionKeySelectors[index2]);
            stringBuilder.Append(" = ");
            string name2 = str + index2.ToString();
            parameters.Add(new SqlParameter(name2, objectArray[index2]));
            stringBuilder.Append(name2);
          }
        }
        stringBuilder.Append(" )");
        if (index1 < num - 1)
          stringBuilder.Append(" OR ");
      }
      stringBuilder.Append(" )");
      return QueryDefinition.CreateFromQuerySpec(new SqlQuerySpec(stringBuilder.ToString(), parameters));
    }

    private List<string> CreatePkSelectors(PartitionKeyDefinition partitionKeyDefinition)
    {
      List<string> pkSelectors = new List<string>();
      foreach (string path in partitionKeyDefinition.Paths)
      {
        IReadOnlyList<string> pathParts = PathParser.GetPathParts(path);
        List<string> values = new List<string>();
        foreach (string str in (IEnumerable<string>) pathParts)
          values.Add("[\"" + str + "\"]");
        string str1 = string.Join(string.Empty, (IEnumerable<string>) values);
        pkSelectors.Add(str1);
      }
      return pkSelectors;
    }

    private async Task<List<ResponseMessage>> GenerateStreamResponsesForPartitionAsync(
      QueryDefinition queryDefinition,
      PartitionKeyRange partitionKeyRange,
      ReadManyRequestOptions readManyRequestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
        return (List<ResponseMessage>) null;
      List<ResponseMessage> pages = new List<ResponseMessage>();
      FeedIteratorInternal feedIterator = (FeedIteratorInternal) this.container.GetItemQueryStreamIterator((FeedRange) new FeedRangeEpk(partitionKeyRange.ToRange()), queryDefinition, (string) null, readManyRequestOptions?.ConvertToQueryRequestOptions());
      while (feedIterator.HasMoreResults)
      {
        try
        {
          ResponseMessage responseMessage = await feedIterator.ReadNextAsync(trace, cancellationToken);
          if (!responseMessage.IsSuccessStatusCode)
            this.CancelCancellationToken(cancellationToken);
          pages.Add(responseMessage);
        }
        catch
        {
          this.CancelCancellationToken(cancellationToken);
          throw;
        }
      }
      return pages;
    }

    private void CancelCancellationToken(CancellationToken cancellationToken)
    {
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
        linkedTokenSource.Cancel();
    }

    private ValueTask<PartitionKeyInternal> GetPartitionKeyInternalAsync(
      PartitionKey partitionKey,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return partitionKey.IsNone ? new ValueTask<PartitionKeyInternal>(this.container.GetNonePartitionKeyValueAsync(trace, cancellationToken)) : new ValueTask<PartitionKeyInternal>(partitionKey.InternalKey);
    }

    private class ReadManyFeedResponseEnumerable<T> : IEnumerable<T>, IEnumerable
    {
      private readonly List<FeedResponse<T>> typedResponses;

      public ReadManyFeedResponseEnumerable(List<FeedResponse<T>> queryResponses) => this.typedResponses = queryResponses;

      public IEnumerator<T> GetEnumerator()
      {
        foreach (FeedResponse<T> typedResponse in this.typedResponses)
        {
          foreach (T obj in typedResponse)
            yield return obj;
        }
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
    }
  }
}
