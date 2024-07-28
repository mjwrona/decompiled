// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.OrderByDocumentQueryExecutionContext
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections.Generic;
using Microsoft.Azure.Documents.Query.ParallelQuery;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class OrderByDocumentQueryExecutionContext : 
    CrossPartitionQueryExecutionContext<OrderByQueryResult>
  {
    private const string FormatPlaceHolder = "{documentdb-formattableorderbyquery-filter}";
    private const string True = "true";
    private static readonly Func<DocumentProducerTree<OrderByQueryResult>, int> FetchPriorityFunction = (Func<DocumentProducerTree<OrderByQueryResult>, int>) (documentProducerTree => documentProducerTree.BufferedItemCount);
    private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
    {
      StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
    };
    private int skipCount;
    private string previousRid;

    private OrderByDocumentQueryExecutionContext(
      DocumentQueryExecutionContextBase.InitParams initPararms,
      string rewrittenQuery,
      OrderByConsumeComparer consumeComparer)
      : base(initPararms, rewrittenQuery, (IComparer<DocumentProducerTree<OrderByQueryResult>>) consumeComparer, OrderByDocumentQueryExecutionContext.FetchPriorityFunction, (IEqualityComparer<OrderByQueryResult>) new OrderByDocumentQueryExecutionContext.OrderByEqualityComparer(consumeComparer))
    {
    }

    protected override string ContinuationToken
    {
      get
      {
        if (this.IsDone)
          return (string) null;
        IEnumerable<DocumentProducer<OrderByQueryResult>> documentProducers = this.GetActiveDocumentProducers();
        return JsonConvert.SerializeObject((object) JToken.Parse(documentProducers.Count<DocumentProducer<OrderByQueryResult>>() > 0 ? JsonConvert.SerializeObject((object) documentProducers.Select<DocumentProducer<OrderByQueryResult>, OrderByContinuationToken>((Func<DocumentProducer<OrderByQueryResult>, OrderByContinuationToken>) (documentProducer =>
        {
          OrderByQueryResult current = documentProducer.Current;
          string filter = documentProducer.Filter;
          return new OrderByContinuationToken(new CompositeContinuationToken()
          {
            Token = documentProducer.PreviousContinuationToken,
            Range = documentProducer.PartitionKeyRange.ToRange()
          }, current.OrderByItems, current.Rid, this.ShouldIncrementSkipCount(documentProducer) ? this.skipCount + 1 : 0, filter);
        })), OrderByDocumentQueryExecutionContext.JsonSerializerSettings) : (string) null), OrderByDocumentQueryExecutionContext.JsonSerializerSettings);
      }
    }

    public static async Task<OrderByDocumentQueryExecutionContext> CreateAsync(
      DocumentQueryExecutionContextBase.InitParams constructorParams,
      CrossPartitionQueryExecutionContext<object>.CrossPartitionInitParams initParams,
      CancellationToken token)
    {
      OrderByDocumentQueryExecutionContext context = new OrderByDocumentQueryExecutionContext(constructorParams, initParams.PartitionedQueryExecutionInfo.QueryInfo.RewrittenQuery, new OrderByConsumeComparer(initParams.PartitionedQueryExecutionInfo.QueryInfo.OrderBy));
      await context.InitializeAsync(initParams.RequestContinuation, initParams.CollectionRid, initParams.PartitionKeyRanges, initParams.InitialPageSize, initParams.PartitionedQueryExecutionInfo.QueryInfo.OrderBy, initParams.PartitionedQueryExecutionInfo.QueryInfo.OrderByExpressions, token);
      return context;
    }

    public override async Task<FeedResponse<object>> DrainAsync(
      int maxElements,
      CancellationToken cancellationToken)
    {
      OrderByDocumentQueryExecutionContext executionContext = this;
      cancellationToken.ThrowIfCancellationRequested();
      List<object> results = new List<object>();
      while (!executionContext.IsDone && results.Count < maxElements)
      {
        DocumentProducerTree<OrderByQueryResult> currentDocumentProducerTree = executionContext.PopCurrentDocumentProducerTree();
        OrderByQueryResult current = currentDocumentProducerTree.Current;
        results.Add(current.Payload);
        if (executionContext.ShouldIncrementSkipCount(currentDocumentProducerTree.CurrentDocumentProducerTree.Root))
          ++executionContext.skipCount;
        else
          executionContext.skipCount = 0;
        executionContext.previousRid = current.Rid;
        int num = await currentDocumentProducerTree.MoveNextAsync(cancellationToken) ? 1 : 0;
        executionContext.PushCurrentDocumentProducerTree(currentDocumentProducerTree);
        currentDocumentProducerTree = (DocumentProducerTree<OrderByQueryResult>) null;
      }
      return new FeedResponse<object>((IEnumerable<object>) results, results.Count, executionContext.GetResponseHeaders(), queryMetrics: executionContext.GetQueryMetrics(), partitionedClientSideRequestStatistics: executionContext.GetRequestStats(), responseLengthBytes: executionContext.GetAndResetResponseLengthBytes());
    }

    private bool ShouldIncrementSkipCount(
      DocumentProducer<OrderByQueryResult> currentDocumentProducer)
    {
      return !currentDocumentProducer.IsAtBeginningOfPage && string.Equals(this.previousRid, currentDocumentProducer.Current.Rid, StringComparison.Ordinal);
    }

    private async Task InitializeAsync(
      string requestContinuation,
      string collectionRid,
      List<PartitionKeyRange> partitionKeyRanges,
      int initialPageSize,
      SortOrder[] sortOrders,
      string[] orderByExpressions,
      CancellationToken cancellationToken)
    {
      OrderByDocumentQueryExecutionContext executionContext1 = this;
      if (requestContinuation == null)
      {
        OrderByDocumentQueryExecutionContext executionContext2 = executionContext1;
        string collectionRid1 = collectionRid;
        List<PartitionKeyRange> partitionKeyRanges1 = partitionKeyRanges;
        int initialPageSize1 = initialPageSize;
        CancellationToken cancellationToken1 = cancellationToken;
        SqlQuerySpec querySpecForInit = new SqlQuerySpec(executionContext1.QuerySpec.QueryText.Replace("{documentdb-formattableorderbyquery-filter}", "true"), executionContext1.QuerySpec.Parameters);
        CancellationToken token = cancellationToken1;
        await executionContext2.InitializeAsync(collectionRid1, (IReadOnlyList<PartitionKeyRange>) partitionKeyRanges1, initialPageSize1, querySpecForInit, (Dictionary<string, string>) null, false, (string) null, (Func<DocumentProducerTree<OrderByQueryResult>, Task>) null, token);
      }
      else
      {
        OrderByContinuationToken[] continuationToken1 = executionContext1.ValidateAndExtractContinuationToken(requestContinuation, sortOrders, orderByExpressions);
        Dictionary<string, OrderByContinuationToken> targetRangeToOrderByContinuationMap = (Dictionary<string, OrderByContinuationToken>) null;
        RangeFilterInitializationInfo[] initializationInfoArray = executionContext1.GetPartitionKeyRangesInitializationInfo(continuationToken1, partitionKeyRanges, sortOrders, orderByExpressions, out targetRangeToOrderByContinuationMap);
        for (int index = 0; index < initializationInfoArray.Length; ++index)
        {
          RangeFilterInitializationInfo initializationInfo = initializationInfoArray[index];
          if (initializationInfo.StartIndex <= initializationInfo.EndIndex)
          {
            PartialReadOnlyList<PartitionKeyRange> partitionKeyRanges2 = new PartialReadOnlyList<PartitionKeyRange>((IReadOnlyList<PartitionKeyRange>) partitionKeyRanges, initializationInfo.StartIndex, initializationInfo.EndIndex - initializationInfo.StartIndex + 1);
            await executionContext1.InitializeAsync(collectionRid, (IReadOnlyList<PartitionKeyRange>) partitionKeyRanges2, initialPageSize, new SqlQuerySpec(executionContext1.QuerySpec.QueryText.Replace("{documentdb-formattableorderbyquery-filter}", initializationInfo.Filter), executionContext1.QuerySpec.Parameters), targetRangeToOrderByContinuationMap.ToDictionary<KeyValuePair<string, OrderByContinuationToken>, string, string>((Func<KeyValuePair<string, OrderByContinuationToken>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, OrderByContinuationToken>, string>) (kvp => kvp.Value.CompositeContinuationToken.Token)), false, initializationInfo.Filter, (Func<DocumentProducerTree<OrderByQueryResult>, Task>) (async documentProducerTree =>
            {
              OrderByContinuationToken continuationToken2;
              if (!targetRangeToOrderByContinuationMap.TryGetValue(documentProducerTree.Root.PartitionKeyRange.Id, out continuationToken2))
                return;
              await this.FilterAsync(documentProducerTree, sortOrders, continuationToken2, cancellationToken);
            }), cancellationToken);
          }
        }
        initializationInfoArray = (RangeFilterInitializationInfo[]) null;
      }
    }

    private OrderByContinuationToken[] ValidateAndExtractContinuationToken(
      string requestContinuation,
      SortOrder[] sortOrders,
      string[] orderByExpressions)
    {
      if (string.IsNullOrWhiteSpace(requestContinuation))
        throw new ArgumentNullException("continuation can not be null or empty.");
      try
      {
        OrderByContinuationToken[] continuationToken1 = JsonConvert.DeserializeObject<OrderByContinuationToken[]>(requestContinuation, DefaultJsonSerializationSettings.Value);
        if (continuationToken1.Length == 0)
          throw new BadRequestException("Order by continuation token can not be empty: " + requestContinuation + ".");
        foreach (OrderByContinuationToken continuationToken2 in continuationToken1)
        {
          if (continuationToken2.OrderByItems.Length != sortOrders.Length)
            throw new BadRequestException("Invalid order-by items in ontinutaion token " + requestContinuation + " for OrderBy~Context.");
        }
        return continuationToken1;
      }
      catch (JsonException ex)
      {
        throw new BadRequestException("Invalid JSON in continuation token " + requestContinuation + " for OrderBy~Context, exception: " + ex.Message);
      }
    }

    private async Task FilterAsync(
      DocumentProducerTree<OrderByQueryResult> producer,
      SortOrder[] sortOrders,
      OrderByContinuationToken continuationToken,
      CancellationToken cancellationToken)
    {
      foreach (DocumentProducerTree<OrderByQueryResult> tree in producer)
      {
        ResourceId continuationRid;
        if (!ResourceId.TryParse(continuationToken.Rid, out continuationRid))
          throw new BadRequestException("Invalid Rid in the continuation token " + continuationToken.CompositeContinuationToken.Token + " for OrderBy~Context.");
        Dictionary<string, ResourceId> resourceIds = new Dictionary<string, ResourceId>();
        int itemToSkip = continuationToken.SkipCount;
        bool continuationRidVerified = false;
        do
        {
          OrderByQueryResult current = tree.Current;
          if (current != null)
          {
            int num1 = 0;
            for (int index = 0; index < sortOrders.Length; ++index)
            {
              num1 = ItemComparer.Instance.Compare(continuationToken.OrderByItems[index].GetItem(), current.OrderByItems[index].GetItem());
              if (num1 != 0)
              {
                num1 = sortOrders[index] != SortOrder.Descending ? num1 : -num1;
                break;
              }
            }
            if (num1 >= 0)
            {
              if (num1 == 0)
              {
                ResourceId rid;
                if (!resourceIds.TryGetValue(current.Rid, out rid))
                {
                  if (!ResourceId.TryParse(current.Rid, out rid))
                    throw new BadRequestException("Invalid Rid in the continuation token " + continuationToken.CompositeContinuationToken.Token + " for OrderBy~Context.");
                  resourceIds.Add(current.Rid, rid);
                }
                if (!continuationRidVerified)
                {
                  if ((int) continuationRid.Database != (int) rid.Database || (int) continuationRid.DocumentCollection != (int) rid.DocumentCollection)
                    throw new BadRequestException("Invalid Rid in the continuation token " + continuationToken.CompositeContinuationToken.Token + " for OrderBy~Context.");
                  continuationRidVerified = true;
                }
                int num2 = continuationRid.Document.CompareTo(rid.Document);
                if (sortOrders[0] == SortOrder.Descending)
                  num2 = -num2;
                if (num2 < 0 || num2 == 0 && itemToSkip-- <= 0)
                  break;
              }
            }
            else
              break;
          }
          else
            break;
        }
        while (await tree.MoveNextAsync(cancellationToken));
        continuationRid = (ResourceId) null;
        resourceIds = (Dictionary<string, ResourceId>) null;
      }
    }

    private RangeFilterInitializationInfo[] GetPartitionKeyRangesInitializationInfo(
      OrderByContinuationToken[] suppliedContinuationTokens,
      List<PartitionKeyRange> partitionKeyRanges,
      SortOrder[] sortOrders,
      string[] orderByExpressions,
      out Dictionary<string, OrderByContinuationToken> targetRangeToContinuationTokenMap)
    {
      int continuationTokens = this.FindTargetRangeAndExtractContinuationTokens<OrderByContinuationToken>(partitionKeyRanges, ((IEnumerable<OrderByContinuationToken>) suppliedContinuationTokens).Select<OrderByContinuationToken, Tuple<OrderByContinuationToken, Range<string>>>((Func<OrderByContinuationToken, Tuple<OrderByContinuationToken, Range<string>>>) (token => Tuple.Create<OrderByContinuationToken, Range<string>>(token, token.CompositeContinuationToken.Range))), out targetRangeToContinuationTokenMap);
      OrderByDocumentQueryExecutionContext.FormattedFilterInfo formattedFilters = this.GetFormattedFilters(orderByExpressions, suppliedContinuationTokens, sortOrders);
      return new RangeFilterInitializationInfo[3]
      {
        new RangeFilterInitializationInfo(formattedFilters.FilterForRangesLeftOfTargetRanges, 0, continuationTokens - 1),
        new RangeFilterInitializationInfo(formattedFilters.FiltersForTargetRange, continuationTokens, continuationTokens),
        new RangeFilterInitializationInfo(formattedFilters.FilterForRangesRightOfTargetRanges, continuationTokens + 1, partitionKeyRanges.Count - 1)
      };
    }

    private OrderByDocumentQueryExecutionContext.FormattedFilterInfo GetFormattedFilters(
      string[] expressions,
      OrderByContinuationToken[] continuationTokens,
      SortOrder[] sortOrders)
    {
      int num = 0;
      while (num < continuationTokens.Length)
        ++num;
      Tuple<string, string, string> formattedFilters = this.GetFormattedFilters(expressions, ((IEnumerable<QueryItem>) continuationTokens[0].OrderByItems).Select<QueryItem, object>((Func<QueryItem, object>) (queryItem => queryItem.GetItem())).ToArray<object>(), sortOrders);
      return new OrderByDocumentQueryExecutionContext.FormattedFilterInfo(formattedFilters.Item1, formattedFilters.Item2, formattedFilters.Item3);
    }

    private void AppendToBuilders(
      Tuple<StringBuilder, StringBuilder, StringBuilder> builders,
      object str)
    {
      this.AppendToBuilders(builders, str, str, str);
    }

    private void AppendToBuilders(
      Tuple<StringBuilder, StringBuilder, StringBuilder> builders,
      object left,
      object target,
      object right)
    {
      builders.Item1.Append(left);
      builders.Item2.Append(target);
      builders.Item3.Append(right);
    }

    private Tuple<string, string, string> GetFormattedFilters(
      string[] expressions,
      object[] orderByItems,
      SortOrder[] sortOrders)
    {
      int length = expressions.Length;
      int num = length == 1 ? 1 : 0;
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      StringBuilder stringBuilder3 = new StringBuilder();
      Tuple<StringBuilder, StringBuilder, StringBuilder> builders = new Tuple<StringBuilder, StringBuilder, StringBuilder>(stringBuilder1, stringBuilder3, stringBuilder2);
      if (num != 0)
      {
        string str1 = ((IEnumerable<string>) expressions).First<string>();
        SortOrder sortOrder = ((IEnumerable<SortOrder>) sortOrders).First<SortOrder>();
        string str2 = JsonConvert.SerializeObject(((IEnumerable<object>) orderByItems).First<object>(), DefaultJsonSerializationSettings.Value);
        stringBuilder1.Append(str1 + " " + (sortOrder == SortOrder.Descending ? "<" : ">") + " " + str2);
        stringBuilder2.Append(str1 + " " + (sortOrder == SortOrder.Descending ? "<=" : ">=") + " " + str2);
        stringBuilder3.Append(str1 + " " + (sortOrder == SortOrder.Descending ? "<=" : ">=") + " " + str2);
      }
      else
      {
        for (int count = 1; count <= length; ++count)
        {
          ArraySegment<string> source1 = new ArraySegment<string>(expressions, 0, count);
          ArraySegment<SortOrder> source2 = new ArraySegment<SortOrder>(sortOrders, 0, count);
          ArraySegment<object> source3 = new ArraySegment<object>(orderByItems, 0, count);
          bool flag1 = count == length;
          this.AppendToBuilders(builders, (object) "(");
          for (int index = 0; index < count; ++index)
          {
            string str3 = source1.ElementAt<string>(index);
            SortOrder sortOrder = source2.ElementAt<SortOrder>(index);
            object obj = source3.ElementAt<object>(index);
            bool flag2 = index == count - 1;
            this.AppendToBuilders(builders, (object) str3);
            this.AppendToBuilders(builders, (object) " ");
            if (flag2)
            {
              string str4 = sortOrder == SortOrder.Descending ? "<" : ">";
              this.AppendToBuilders(builders, (object) str4);
              if (flag1)
                this.AppendToBuilders(builders, (object) "", (object) "=", (object) "=");
            }
            else
              this.AppendToBuilders(builders, (object) "=");
            JsonSerializerSettings settings = DefaultJsonSerializationSettings.Value;
            string str5 = JsonConvert.SerializeObject(obj, settings);
            this.AppendToBuilders(builders, (object) " ");
            this.AppendToBuilders(builders, (object) str5);
            this.AppendToBuilders(builders, (object) " ");
            if (!flag2)
              this.AppendToBuilders(builders, (object) "AND ");
          }
          this.AppendToBuilders(builders, (object) ")");
          if (!flag1)
            this.AppendToBuilders(builders, (object) " OR ");
        }
      }
      return new Tuple<string, string, string>(stringBuilder1.ToString(), stringBuilder2.ToString(), stringBuilder3.ToString());
    }

    private struct FormattedFilterInfo
    {
      public readonly string FiltersForTargetRange;
      public readonly string FilterForRangesLeftOfTargetRanges;
      public readonly string FilterForRangesRightOfTargetRanges;

      public FormattedFilterInfo(string leftFilter, string targetFilter, string rightFilters)
      {
        this.FilterForRangesLeftOfTargetRanges = leftFilter;
        this.FiltersForTargetRange = targetFilter;
        this.FilterForRangesRightOfTargetRanges = rightFilters;
      }
    }

    private sealed class OrderByEqualityComparer : IEqualityComparer<OrderByQueryResult>
    {
      private readonly OrderByConsumeComparer orderByConsumeComparer;

      public OrderByEqualityComparer(OrderByConsumeComparer orderByConsumeComparer) => this.orderByConsumeComparer = orderByConsumeComparer != null ? orderByConsumeComparer : throw new ArgumentNullException("orderByConsumeComparer can not be null.");

      public bool Equals(OrderByQueryResult x, OrderByQueryResult y) => this.orderByConsumeComparer.CompareOrderByItems(x.OrderByItems, y.OrderByItems) == 0;

      public int GetHashCode(OrderByQueryResult obj) => 0;
    }
  }
}
