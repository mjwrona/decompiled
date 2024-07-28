// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ExecutionComponent.GroupByDocumentQueryExecutionComponent
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query.ExecutionComponent
{
  internal sealed class GroupByDocumentQueryExecutionComponent : DocumentQueryExecutionComponentBase
  {
    public const string ContinuationTokenNotSupportedWithGroupBy = "Continuation token is not supported for queries with GROUP BY. Do not use FeedResponse.ResponseContinuation or remove the GROUP BY from the query.";
    private static readonly List<object> EmptyResults = new List<object>();
    private static readonly DictionaryNameValueCollection EmptyHeaders = new DictionaryNameValueCollection();
    private static readonly Dictionary<string, QueryMetrics> EmptyQueryMetrics = new Dictionary<string, QueryMetrics>();
    private static readonly AggregateOperator[] EmptyAggregateOperators = new AggregateOperator[0];
    private readonly IReadOnlyDictionary<string, AggregateOperator?> groupByAliasToAggregateType;
    private readonly IReadOnlyList<string> orderedAliases;
    private readonly Dictionary<UInt192, SingleGroupAggregator> groupingTable;
    private readonly bool hasSelectValue;
    private JsonSerializer jsonSerializer;
    private string ownerFullName;
    private int numPagesDrainedFromGroupingTable;
    private bool isDone;

    private GroupByDocumentQueryExecutionComponent(
      IReadOnlyDictionary<string, AggregateOperator?> groupByAliasToAggregateType,
      IReadOnlyList<string> orderedAliases,
      bool hasSelectValue,
      IDocumentQueryExecutionComponent source)
      : base(source)
    {
      if (groupByAliasToAggregateType == null)
        throw new ArgumentNullException(nameof (groupByAliasToAggregateType));
      if (orderedAliases == null)
        throw new ArgumentNullException(nameof (orderedAliases));
      this.groupingTable = new Dictionary<UInt192, SingleGroupAggregator>();
      this.groupByAliasToAggregateType = groupByAliasToAggregateType;
      this.orderedAliases = orderedAliases;
      this.hasSelectValue = hasSelectValue;
    }

    public override bool IsDone => this.isDone;

    public static async Task<IDocumentQueryExecutionComponent> CreateAsync(
      string requestContinuation,
      Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback,
      IReadOnlyDictionary<string, AggregateOperator?> groupByAliasToAggregateType,
      IReadOnlyList<string> orderedAliases,
      bool hasSelectValue)
    {
      IReadOnlyDictionary<string, AggregateOperator?> groupByAliasToAggregateType1 = groupByAliasToAggregateType;
      IReadOnlyList<string> orderedAliases1 = orderedAliases;
      bool hasSelectValue1 = hasSelectValue;
      return (IDocumentQueryExecutionComponent) new GroupByDocumentQueryExecutionComponent(groupByAliasToAggregateType1, orderedAliases1, hasSelectValue1, await createSourceCallback(requestContinuation));
    }

    public override async Task<FeedResponse<object>> DrainAsync(
      int maxElements,
      CancellationToken cancellationToken)
    {
      GroupByDocumentQueryExecutionComponent executionComponent = this;
      cancellationToken.ThrowIfCancellationRequested();
      FeedResponse<object> feedResponse1;
      if (!executionComponent.Source.IsDone)
      {
        // ISSUE: reference to a compiler-generated method
        FeedResponse<object> feedResponse2 = await executionComponent.\u003C\u003En__0(int.MaxValue, cancellationToken);
        foreach (object document in feedResponse2)
        {
          GroupByDocumentQueryExecutionComponent.RewrittenGroupByProjection groupByProjection = new GroupByDocumentQueryExecutionComponent.RewrittenGroupByProjection(JTokenAndQueryResultConversionUtils.GetJTokenFromObject(document, out executionComponent.jsonSerializer, out executionComponent.ownerFullName));
          UInt192 hashToken = DistinctHash.GetHashToken((JToken) groupByProjection.GroupByItems);
          SingleGroupAggregator singleGroupAggregator;
          if (!executionComponent.groupingTable.TryGetValue(hashToken, out singleGroupAggregator))
          {
            singleGroupAggregator = SingleGroupAggregator.Create(GroupByDocumentQueryExecutionComponent.EmptyAggregateOperators, executionComponent.groupByAliasToAggregateType, executionComponent.orderedAliases, executionComponent.hasSelectValue);
            executionComponent.groupingTable[hashToken] = singleGroupAggregator;
          }
          JToken payload = groupByProjection.Payload;
          singleGroupAggregator.AddValues(payload);
        }
        feedResponse1 = new FeedResponse<object>((IEnumerable<object>) GroupByDocumentQueryExecutionComponent.EmptyResults, GroupByDocumentQueryExecutionComponent.EmptyResults.Count, feedResponse2.Headers, feedResponse2.UseETagAsContinuation, feedResponse2.QueryMetrics, feedResponse2.PartitionedClientSideRequestStatistics, "Continuation token is not supported for queries with GROUP BY. Do not use FeedResponse.ResponseContinuation or remove the GROUP BY from the query.", feedResponse2.ResponseLengthBytes);
        executionComponent.isDone = false;
      }
      else
      {
        IEnumerable<SingleGroupAggregator> singleGroupAggregators = executionComponent.groupingTable.OrderBy<KeyValuePair<UInt192, SingleGroupAggregator>, UInt192>((Func<KeyValuePair<UInt192, SingleGroupAggregator>, UInt192>) (kvp => kvp.Key)).Skip<KeyValuePair<UInt192, SingleGroupAggregator>>(executionComponent.numPagesDrainedFromGroupingTable * maxElements).Take<KeyValuePair<UInt192, SingleGroupAggregator>>(maxElements).Select<KeyValuePair<UInt192, SingleGroupAggregator>, SingleGroupAggregator>((Func<KeyValuePair<UInt192, SingleGroupAggregator>, SingleGroupAggregator>) (kvp => kvp.Value));
        List<object> result = new List<object>();
        foreach (SingleGroupAggregator singleGroupAggregator in singleGroupAggregators)
        {
          object objectFromJtoken = JTokenAndQueryResultConversionUtils.GetObjectFromJToken(JTokenAndQueryResultConversionUtils.GetJTokenFromObject(singleGroupAggregator.GetResult(), out JsonSerializer _, out string _), executionComponent.jsonSerializer, executionComponent.ownerFullName);
          result.Add(objectFromJtoken);
        }
        feedResponse1 = new FeedResponse<object>((IEnumerable<object>) result, result.Count, (INameValueCollection) GroupByDocumentQueryExecutionComponent.EmptyHeaders, queryMetrics: (IReadOnlyDictionary<string, QueryMetrics>) GroupByDocumentQueryExecutionComponent.EmptyQueryMetrics, disallowContinuationTokenMessage: "Continuation token is not supported for queries with GROUP BY. Do not use FeedResponse.ResponseContinuation or remove the GROUP BY from the query.");
        ++executionComponent.numPagesDrainedFromGroupingTable;
        if (executionComponent.numPagesDrainedFromGroupingTable * maxElements >= executionComponent.groupingTable.Count)
          executionComponent.isDone = true;
      }
      return feedResponse1;
    }

    private struct RewrittenGroupByProjection
    {
      public RewrittenGroupByProjection(JToken jToken)
      {
        if (jToken == null)
          throw new ArgumentNullException(nameof (jToken));
        JToken jtoken = jToken[(object) "groupByItems"] is JArray jarray ? jToken[(object) "payload"] : throw new ArgumentException("RewrittenGroupByProjection does not have a 'groupByItems' array.");
        if (jtoken == null)
          throw new ArgumentException("RewrittenGroupByProjection does not have a 'payload' property.");
        this.GroupByItems = jarray;
        this.Payload = jtoken;
      }

      public JArray GroupByItems { get; }

      public JToken Payload { get; }
    }
  }
}
