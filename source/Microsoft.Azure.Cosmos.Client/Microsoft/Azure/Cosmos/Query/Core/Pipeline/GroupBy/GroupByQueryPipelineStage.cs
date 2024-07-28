// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.GroupBy.GroupByQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Metrics;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.GroupBy
{
  internal abstract class GroupByQueryPipelineStage : QueryPipelineStageBase
  {
    private readonly GroupByQueryPipelineStage.GroupingTable groupingTable;
    protected readonly int pageSize;
    protected bool returnedLastPage;

    protected GroupByQueryPipelineStage(
      IQueryPipelineStage source,
      CancellationToken cancellationToken,
      GroupByQueryPipelineStage.GroupingTable groupingTable,
      int pageSize)
      : base(source, cancellationToken)
    {
      this.groupingTable = groupingTable ?? throw new ArgumentNullException(nameof (groupingTable));
      this.pageSize = pageSize;
    }

    public static TryCatch<IQueryPipelineStage> MonadicCreate(
      ExecutionEnvironment executionEnvironment,
      CosmosElement continuationToken,
      CancellationToken cancellationToken,
      MonadicCreatePipelineStage monadicCreatePipelineStage,
      IReadOnlyDictionary<string, AggregateOperator?> groupByAliasToAggregateType,
      IReadOnlyList<string> orderedAliases,
      bool hasSelectValue,
      int pageSize)
    {
      if (executionEnvironment == ExecutionEnvironment.Client)
        return GroupByQueryPipelineStage.ClientGroupByQueryPipelineStage.MonadicCreate(continuationToken, cancellationToken, monadicCreatePipelineStage, groupByAliasToAggregateType, orderedAliases, hasSelectValue, pageSize);
      if (executionEnvironment == ExecutionEnvironment.Compute)
        return GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.MonadicCreate(continuationToken, cancellationToken, monadicCreatePipelineStage, groupByAliasToAggregateType, orderedAliases, hasSelectValue, pageSize);
      throw new ArgumentException(string.Format("Unknown {0}: {1}", (object) "ExecutionEnvironment", (object) executionEnvironment));
    }

    protected void AggregateGroupings(IReadOnlyList<CosmosElement> cosmosElements)
    {
      foreach (CosmosElement cosmosElement in (IEnumerable<CosmosElement>) cosmosElements)
        this.groupingTable.AddPayload(new GroupByQueryPipelineStage.RewrittenGroupByProjection(cosmosElement));
    }

    private sealed class ClientGroupByQueryPipelineStage : GroupByQueryPipelineStage
    {
      public const string ContinuationTokenNotSupportedWithGroupBy = "Continuation token is not supported for queries with GROUP BY. Do not use FeedResponse.ResponseContinuation or remove the GROUP BY from the query.";

      private ClientGroupByQueryPipelineStage(
        IQueryPipelineStage source,
        CancellationToken cancellationToken,
        GroupByQueryPipelineStage.GroupingTable groupingTable,
        int pageSize)
        : base(source, cancellationToken, groupingTable, pageSize)
      {
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreate(
        CosmosElement requestContinuation,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage,
        IReadOnlyDictionary<string, AggregateOperator?> groupByAliasToAggregateType,
        IReadOnlyList<string> orderedAliases,
        bool hasSelectValue,
        int pageSize)
      {
        TryCatch<GroupByQueryPipelineStage.GroupingTable> continuationToken = GroupByQueryPipelineStage.GroupingTable.TryCreateFromContinuationToken(groupByAliasToAggregateType, orderedAliases, hasSelectValue, (CosmosElement) null);
        if (continuationToken.Failed)
          return TryCatch<IQueryPipelineStage>.FromException(continuationToken.Exception);
        TryCatch<IQueryPipelineStage> tryCatch = monadicCreatePipelineStage(requestContinuation, cancellationToken);
        return tryCatch.Failed ? tryCatch : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new GroupByQueryPipelineStage.ClientGroupByQueryPipelineStage(tryCatch.Result, cancellationToken, continuationToken.Result, pageSize));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        GroupByQueryPipelineStage.ClientGroupByQueryPipelineStage queryPipelineStage = this;
        queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        if (queryPipelineStage.returnedLastPage)
        {
          queryPipelineStage.Current = new TryCatch<QueryPage>();
          return false;
        }
        double requestCharge = 0.0;
        long responseLengthInBytes = 0;
        IReadOnlyDictionary<string, string> addtionalHeaders = (IReadOnlyDictionary<string, string>) null;
        TryCatch<QueryPage> current;
        while (true)
        {
          if (await queryPipelineStage.inputStage.MoveNextAsync(trace))
          {
            queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
            current = queryPipelineStage.inputStage.Current;
            if (!current.Failed)
            {
              QueryPage result = current.Result;
              requestCharge += result.RequestCharge;
              responseLengthInBytes += result.ResponseLengthInBytes;
              addtionalHeaders = result.AdditionalHeaders;
              queryPipelineStage.AggregateGroupings(result.Documents);
            }
            else
              break;
          }
          else
            goto label_10;
        }
        queryPipelineStage.Current = current;
        return true;
label_10:
        IReadOnlyList<CosmosElement> documents = queryPipelineStage.groupingTable.Drain(queryPipelineStage.pageSize);
        if (queryPipelineStage.groupingTable.Count == 0)
          queryPipelineStage.returnedLastPage = true;
        double requestCharge1 = requestCharge;
        long responseLengthInBytes1 = responseLengthInBytes;
        IReadOnlyDictionary<string, string> additionalHeaders = addtionalHeaders;
        QueryPage result1 = new QueryPage(documents, requestCharge1, (string) null, responseLengthInBytes1, (Lazy<CosmosQueryExecutionInfo>) null, "Continuation token is not supported for queries with GROUP BY. Do not use FeedResponse.ResponseContinuation or remove the GROUP BY from the query.", additionalHeaders, (QueryState) null);
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result1);
        return true;
      }
    }

    private sealed class ComputeGroupByQueryPipelineStage : GroupByQueryPipelineStage
    {
      private const string DoneReadingGroupingsContinuationToken = "DONE";
      private static readonly CosmosElement DoneCosmosElementToken = (CosmosElement) CosmosString.Create("DONE");
      private static readonly IReadOnlyList<CosmosElement> EmptyResults = (IReadOnlyList<CosmosElement>) new List<CosmosElement>().AsReadOnly();
      private static readonly IReadOnlyDictionary<string, QueryMetrics> EmptyQueryMetrics = (IReadOnlyDictionary<string, QueryMetrics>) new Dictionary<string, QueryMetrics>();

      private ComputeGroupByQueryPipelineStage(
        IQueryPipelineStage source,
        CancellationToken cancellationToken,
        GroupByQueryPipelineStage.GroupingTable groupingTable,
        int pageSize)
        : base(source, cancellationToken, groupingTable, pageSize)
      {
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreate(
        CosmosElement requestContinuation,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage,
        IReadOnlyDictionary<string, AggregateOperator?> groupByAliasToAggregateType,
        IReadOnlyList<string> orderedAliases,
        bool hasSelectValue,
        int pageSize)
      {
        GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken groupByContinuationToken;
        if (requestContinuation != (CosmosElement) null)
        {
          if (!GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken.TryParse(requestContinuation, out groupByContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid {0}: '{1}'", (object) "GroupByContinuationToken", (object) requestContinuation)));
        }
        else
          groupByContinuationToken = new GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken((CosmosElement) null, (CosmosElement) null);
        TryCatch<IQueryPipelineStage> tryCatch = !(groupByContinuationToken.SourceContinuationToken is CosmosString continuationToken1) || !UtfAnyString.op_Equality(continuationToken1.Value, "DONE") ? monadicCreatePipelineStage(groupByContinuationToken.SourceContinuationToken, cancellationToken) : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) EmptyQueryPipelineStage.Singleton);
        if (!tryCatch.Succeeded)
          return TryCatch<IQueryPipelineStage>.FromException(tryCatch.Exception);
        TryCatch<GroupByQueryPipelineStage.GroupingTable> continuationToken2 = GroupByQueryPipelineStage.GroupingTable.TryCreateFromContinuationToken(groupByAliasToAggregateType, orderedAliases, hasSelectValue, groupByContinuationToken.GroupingTableContinuationToken);
        return !continuationToken2.Succeeded ? TryCatch<IQueryPipelineStage>.FromException(continuationToken2.Exception) : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage(tryCatch.Result, cancellationToken, continuationToken2.Result, pageSize));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage queryPipelineStage = this;
        queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        if (queryPipelineStage.returnedLastPage)
        {
          queryPipelineStage.Current = new TryCatch<QueryPage>();
          return false;
        }
        QueryPage result1;
        if (await queryPipelineStage.inputStage.MoveNextAsync(trace))
        {
          TryCatch<QueryPage> current = queryPipelineStage.inputStage.Current;
          if (current.Failed)
          {
            queryPipelineStage.Current = current;
            return true;
          }
          QueryPage result2 = current.Result;
          queryPipelineStage.AggregateGroupings(result2.Documents);
          CosmosElement sourceContinuationToken = result2.State == null ? GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.DoneCosmosElementToken : result2.State.Value;
          QueryState state = new QueryState(GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken.ToCosmosElement(new GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken(queryPipelineStage.groupingTable.GetCosmosElementContinuationToken(), sourceContinuationToken)));
          result1 = new QueryPage(GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.EmptyResults, result2.RequestCharge, result2.ActivityId, result2.ResponseLengthInBytes, result2.CosmosQueryExecutionInfo, (string) null, result2.AdditionalHeaders, state);
        }
        else
        {
          IReadOnlyList<CosmosElement> documents = queryPipelineStage.groupingTable.Drain(queryPipelineStage.pageSize);
          QueryState queryState;
          if (queryPipelineStage.groupingTable.IsDone)
          {
            queryState = (QueryState) null;
            queryPipelineStage.returnedLastPage = true;
          }
          else
            queryState = new QueryState(GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken.ToCosmosElement(new GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken(queryPipelineStage.groupingTable.GetCosmosElementContinuationToken(), GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.DoneCosmosElementToken)));
          QueryState state = queryState;
          result1 = new QueryPage(documents, 0.0, (string) null, 0L, (Lazy<CosmosQueryExecutionInfo>) null, (string) null, (IReadOnlyDictionary<string, string>) null, state);
        }
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result1);
        return true;
      }

      private readonly struct GroupByContinuationToken
      {
        public GroupByContinuationToken(
          CosmosElement groupingTableContinuationToken,
          CosmosElement sourceContinuationToken)
        {
          this.GroupingTableContinuationToken = groupingTableContinuationToken;
          this.SourceContinuationToken = sourceContinuationToken;
        }

        public CosmosElement GroupingTableContinuationToken { get; }

        public CosmosElement SourceContinuationToken { get; }

        public static CosmosElement ToCosmosElement(
          GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken groupByContinuationToken)
        {
          return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
          {
            {
              "SourceToken",
              groupByContinuationToken.SourceContinuationToken
            },
            {
              "GroupingTableContinuationToken",
              groupByContinuationToken.GroupingTableContinuationToken
            }
          });
        }

        public static bool TryParse(
          CosmosElement value,
          out GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken groupByContinuationToken)
        {
          if (!(value is CosmosObject cosmosObject))
          {
            groupByContinuationToken = new GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken();
            return false;
          }
          CosmosElement groupingTableContinuationToken;
          if (!cosmosObject.TryGetValue("GroupingTableContinuationToken", out groupingTableContinuationToken))
          {
            groupByContinuationToken = new GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken();
            return false;
          }
          CosmosElement sourceContinuationToken;
          if (!cosmosObject.TryGetValue("SourceToken", out sourceContinuationToken))
          {
            groupByContinuationToken = new GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken();
            return false;
          }
          groupByContinuationToken = new GroupByQueryPipelineStage.ComputeGroupByQueryPipelineStage.GroupByContinuationToken(groupingTableContinuationToken, sourceContinuationToken);
          return true;
        }

        private static class PropertyNames
        {
          public const string SourceToken = "SourceToken";
          public const string GroupingTableContinuationToken = "GroupingTableContinuationToken";
        }
      }
    }

    protected readonly struct RewrittenGroupByProjection
    {
      private const string GroupByItemsPropertyName = "groupByItems";
      private const string PayloadPropertyName = "payload";
      private readonly CosmosObject cosmosObject;

      public RewrittenGroupByProjection(CosmosElement cosmosElement)
      {
        if (cosmosElement == (CosmosElement) null)
          throw new ArgumentNullException(nameof (cosmosElement));
        this.cosmosObject = cosmosElement is CosmosObject cosmosObject ? cosmosObject : throw new ArgumentException("cosmosElement must not be an object.");
      }

      public CosmosArray GroupByItems
      {
        get
        {
          CosmosElement cosmosElement;
          if (!this.cosmosObject.TryGetValue("groupByItems", out cosmosElement))
            throw new InvalidOperationException("Underlying object does not have an 'groupByItems' field.");
          return cosmosElement is CosmosArray cosmosArray ? cosmosArray : throw new ArgumentException("RewrittenGroupByProjection['groupByItems'] was not an array.");
        }
      }

      public bool TryGetPayload(out CosmosElement payload)
      {
        if (!this.cosmosObject.TryGetValue(nameof (payload), out payload))
          payload = (CosmosElement) CosmosUndefined.Create();
        return true;
      }
    }

    protected sealed class GroupingTable : 
      IEnumerable<KeyValuePair<UInt128, SingleGroupAggregator>>,
      IEnumerable
    {
      private static readonly IReadOnlyList<AggregateOperator> EmptyAggregateOperators = (IReadOnlyList<AggregateOperator>) new AggregateOperator[0];
      private readonly Dictionary<UInt128, SingleGroupAggregator> table;
      private readonly IReadOnlyDictionary<string, AggregateOperator?> groupByAliasToAggregateType;
      private readonly IReadOnlyList<string> orderedAliases;
      private readonly bool hasSelectValue;

      private GroupingTable(
        IReadOnlyDictionary<string, AggregateOperator?> groupByAliasToAggregateType,
        IReadOnlyList<string> orderedAliases,
        bool hasSelectValue)
      {
        this.groupByAliasToAggregateType = groupByAliasToAggregateType ?? throw new ArgumentNullException(nameof (groupByAliasToAggregateType));
        this.orderedAliases = orderedAliases;
        this.hasSelectValue = hasSelectValue;
        this.table = new Dictionary<UInt128, SingleGroupAggregator>();
      }

      public int Count => this.table.Count;

      public bool IsDone { get; private set; }

      public void AddPayload(
        GroupByQueryPipelineStage.RewrittenGroupByProjection rewrittenGroupByProjection)
      {
        CosmosElement payload;
        if (!rewrittenGroupByProjection.TryGetPayload(out payload))
          return;
        UInt128 hash = DistinctHash.GetHash((CosmosElement) rewrittenGroupByProjection.GroupByItems);
        SingleGroupAggregator result;
        if (!this.table.TryGetValue(hash, out result))
        {
          result = SingleGroupAggregator.TryCreate(GroupByQueryPipelineStage.GroupingTable.EmptyAggregateOperators, this.groupByAliasToAggregateType, this.orderedAliases, this.hasSelectValue, (CosmosElement) null).Result;
          this.table[hash] = result;
        }
        result.AddValues(payload);
      }

      public IReadOnlyList<CosmosElement> Drain(int maxItemCount)
      {
        List<UInt128> list = this.table.Keys.Take<UInt128>(maxItemCount).ToList<UInt128>();
        List<SingleGroupAggregator> singleGroupAggregatorList = new List<SingleGroupAggregator>(list.Count);
        foreach (UInt128 key in list)
        {
          SingleGroupAggregator singleGroupAggregator = this.table[key];
          singleGroupAggregatorList.Add(singleGroupAggregator);
        }
        foreach (UInt128 key in list)
          this.table.Remove(key);
        List<CosmosElement> cosmosElementList = new List<CosmosElement>();
        foreach (SingleGroupAggregator singleGroupAggregator in singleGroupAggregatorList)
          cosmosElementList.Add(singleGroupAggregator.GetResult());
        if (this.Count == 0)
          this.IsDone = true;
        return (IReadOnlyList<CosmosElement>) cosmosElementList;
      }

      public CosmosElement GetCosmosElementContinuationToken()
      {
        Dictionary<string, CosmosElement> dictionary = new Dictionary<string, CosmosElement>();
        foreach (KeyValuePair<UInt128, SingleGroupAggregator> keyValuePair in this.table)
          dictionary.Add(keyValuePair.Key.ToString(), keyValuePair.Value.GetCosmosElementContinuationToken());
        return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) dictionary);
      }

      public IEnumerator<KeyValuePair<UInt128, SingleGroupAggregator>> GetEnumerator => (IEnumerator<KeyValuePair<UInt128, SingleGroupAggregator>>) this.table.GetEnumerator();

      public static TryCatch<GroupByQueryPipelineStage.GroupingTable> TryCreateFromContinuationToken(
        IReadOnlyDictionary<string, AggregateOperator?> groupByAliasToAggregateType,
        IReadOnlyList<string> orderedAliases,
        bool hasSelectValue,
        CosmosElement continuationToken)
      {
        GroupByQueryPipelineStage.GroupingTable result = new GroupByQueryPipelineStage.GroupingTable(groupByAliasToAggregateType, orderedAliases, hasSelectValue);
        if (continuationToken != (CosmosElement) null)
        {
          if (!(continuationToken is CosmosObject cosmosObject))
            return TryCatch<GroupByQueryPipelineStage.GroupingTable>.FromException((Exception) new MalformedContinuationTokenException("Invalid GroupingTableContinuationToken"));
          foreach (KeyValuePair<string, CosmosElement> keyValuePair in cosmosObject)
          {
            string key1 = keyValuePair.Key;
            CosmosElement continuationToken1 = keyValuePair.Value;
            UInt128 key2;
            ref UInt128 local = ref key2;
            if (!UInt128.TryParse(key1, out local))
              return TryCatch<GroupByQueryPipelineStage.GroupingTable>.FromException((Exception) new MalformedContinuationTokenException("Invalid GroupingTableContinuationToken"));
            TryCatch<SingleGroupAggregator> tryCatch = SingleGroupAggregator.TryCreate(GroupByQueryPipelineStage.GroupingTable.EmptyAggregateOperators, groupByAliasToAggregateType, orderedAliases, hasSelectValue, continuationToken1);
            if (!tryCatch.Succeeded)
              return TryCatch<GroupByQueryPipelineStage.GroupingTable>.FromException(tryCatch.Exception);
            result.table[key2] = tryCatch.Result;
          }
        }
        return TryCatch<GroupByQueryPipelineStage.GroupingTable>.FromResult(result);
      }

      IEnumerator<KeyValuePair<UInt128, SingleGroupAggregator>> IEnumerable<KeyValuePair<UInt128, SingleGroupAggregator>>.GetEnumerator() => (IEnumerator<KeyValuePair<UInt128, SingleGroupAggregator>>) this.table.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.table.GetEnumerator();
    }
  }
}
