// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ExecutionComponent.AggregateDocumentQueryExecutionComponent
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query.ExecutionComponent
{
  internal sealed class AggregateDocumentQueryExecutionComponent : 
    DocumentQueryExecutionComponentBase
  {
    private readonly SingleGroupAggregator singleGroupAggregator;
    private readonly bool isValueAggregateQuery;

    private AggregateDocumentQueryExecutionComponent(
      IDocumentQueryExecutionComponent source,
      SingleGroupAggregator singleGroupAggregator,
      bool isValueAggregateQuery)
      : base(source)
    {
      this.singleGroupAggregator = singleGroupAggregator != null ? singleGroupAggregator : throw new ArgumentNullException(nameof (singleGroupAggregator));
      this.isValueAggregateQuery = isValueAggregateQuery;
    }

    public static async Task<AggregateDocumentQueryExecutionComponent> CreateAsync(
      AggregateOperator[] aggregates,
      IReadOnlyDictionary<string, AggregateOperator?> aliasToAggregateType,
      IReadOnlyList<string> orderedAliases,
      bool hasSelectValue,
      string requestContinuation,
      Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback)
    {
      return new AggregateDocumentQueryExecutionComponent(await createSourceCallback(requestContinuation), SingleGroupAggregator.Create(aggregates, aliasToAggregateType, orderedAliases, hasSelectValue), aggregates != null && ((IEnumerable<AggregateOperator>) aggregates).Count<AggregateOperator>() == 1);
    }

    public override async Task<FeedResponse<object>> DrainAsync(
      int maxElements,
      CancellationToken token)
    {
      AggregateDocumentQueryExecutionComponent executionComponent = this;
      double requestCharge = 0.0;
      long responseLengthBytes = 0;
      PartitionedClientSideRequestStatistics partitionedClientSideRequestStatistics = PartitionedClientSideRequestStatistics.CreateEmpty();
      PartitionedQueryMetrics partitionedQueryMetrics = new PartitionedQueryMetrics();
      JsonSerializer jsonSerializer = (JsonSerializer) null;
      string ownerFullName = (string) null;
      while (!executionComponent.IsDone)
      {
        // ISSUE: reference to a compiler-generated method
        FeedResponse<object> feedResponse = await executionComponent.\u003C\u003En__0(int.MaxValue, token);
        requestCharge += feedResponse.RequestCharge;
        responseLengthBytes += feedResponse.ResponseLengthBytes;
        partitionedQueryMetrics += new PartitionedQueryMetrics(feedResponse.QueryMetrics);
        if (feedResponse.RequestStatistics != null)
          PartitionedClientSideRequestStatistics.CreateFromDictionary((IReadOnlyDictionary<string, IReadOnlyList<IClientSideRequestStatistics>>) feedResponse.PartitionedClientSideRequestStatistics).AddTo(partitionedClientSideRequestStatistics);
        foreach (object document in feedResponse)
        {
          JToken jtokenFromObject = JTokenAndQueryResultConversionUtils.GetJTokenFromObject(document, out jsonSerializer, out ownerFullName);
          AggregateDocumentQueryExecutionComponent.RewrittenAggregateProjections aggregateProjections = new AggregateDocumentQueryExecutionComponent.RewrittenAggregateProjections(executionComponent.isValueAggregateQuery, jtokenFromObject);
          executionComponent.singleGroupAggregator.AddValues(aggregateProjections.Payload);
        }
      }
      List<object> objectList = new List<object>();
      object result1 = executionComponent.singleGroupAggregator.GetResult();
      if (!Undefined.Value.Equals(result1))
      {
        object objectFromJtoken = JTokenAndQueryResultConversionUtils.GetObjectFromJToken(JTokenAndQueryResultConversionUtils.GetJTokenFromObject(result1, out JsonSerializer _, out string _), jsonSerializer, ownerFullName);
        objectList.Add(objectFromJtoken);
      }
      List<object> result2 = objectList;
      int count = objectList.Count;
      DictionaryNameValueCollection responseHeaders = new DictionaryNameValueCollection();
      responseHeaders.Add("x-ms-request-charge", requestCharge.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      PartitionedQueryMetrics queryMetrics = partitionedQueryMetrics;
      PartitionedClientSideRequestStatistics partitionedClientSideRequestStatistics1 = partitionedClientSideRequestStatistics;
      long responseLengthBytes1 = responseLengthBytes;
      return new FeedResponse<object>((IEnumerable<object>) result2, count, (INameValueCollection) responseHeaders, queryMetrics: (IReadOnlyDictionary<string, QueryMetrics>) queryMetrics, partitionedClientSideRequestStatistics: partitionedClientSideRequestStatistics1, responseLengthBytes: responseLengthBytes1);
    }

    private struct RewrittenAggregateProjections
    {
      public RewrittenAggregateProjections(bool isValueAggregateQuery, JToken raw)
      {
        if (raw == null)
          throw new ArgumentNullException(nameof (raw));
        if (isValueAggregateQuery)
          this.Payload = raw is JArray jarray ? jarray[0] : throw new ArgumentException("RewrittenAggregateProjections was not an array for a value aggregate query.");
        else
          this.Payload = raw[(object) "payload"] ?? throw new ArgumentException("RewrittenAggregateProjections does not have a 'payload' property.");
      }

      public JToken Payload { get; }
    }
  }
}
