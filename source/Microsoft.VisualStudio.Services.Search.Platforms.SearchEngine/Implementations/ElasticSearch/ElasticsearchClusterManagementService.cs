// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.ElasticsearchClusterManagementService
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch
{
  internal class ElasticsearchClusterManagementService : 
    ElasticSearchBasePlatform,
    ISearchClusterManagementService
  {
    private readonly object m_lock = new object();
    private string m_clusterName;

    internal ElasticsearchClusterManagementService(
      string elasticSearchConnectionString,
      string platformSettings,
      bool isOnPrem)
      : base(elasticSearchConnectionString, platformSettings, isOnPrem)
    {
    }

    [Info("InternalForTestPurpose")]
    internal ElasticsearchClusterManagementService(IElasticClient elasticClient)
      : base(elasticClient)
    {
    }

    public string GetSmallestShard(ExecutionContext executionContext, string indexName)
    {
      if (indexName == null)
        throw new ArgumentNullException(nameof (indexName));
      return this.GetIndexShardsSizeInBytes(executionContext, indexName).Aggregate<KeyValuePair<string, double>>((Func<KeyValuePair<string, double>, KeyValuePair<string, double>, KeyValuePair<string, double>>) ((left, right) => left.Value >= right.Value ? right : left)).Key;
    }

    public IEnumerable<KeyValuePair<string, double>> GetIndexShardsSizeInBytes(
      ExecutionContext executionContext,
      string indexName)
    {
      IndicesStatsRequest indicesStatsRequest = indexName != null ? new IndicesStatsRequest((Indices) Indices.Index(indexName), (Metrics) IndicesStatsMetric.Store)
      {
        Level = new Level?(Level.Shards)
      } : throw new ArgumentNullException(nameof (indexName));
      return this.GenericESInvoker<IndicesStatsResponse>(executionContext, (Func<IndicesStatsResponse>) (() => this.ElasticClient.Indices.Stats((IIndicesStatsRequest) indicesStatsRequest)), 1083039).Indices.First<KeyValuePair<string, IndicesStats>>().Value.Shards.Select<KeyValuePair<string, ShardStats[]>, KeyValuePair<string, double>>((Func<KeyValuePair<string, ShardStats[]>, KeyValuePair<string, double>>) (i => new KeyValuePair<string, double>(i.Key, (double) ((IEnumerable<ShardStats>) i.Value).First<ShardStats>((Func<ShardStats, bool>) (j => j.Routing.Primary)).Store.SizeInBytes)));
    }

    public List<EsShardDetails> GetShardsDetails(
      ExecutionContext executionContext,
      string indexName)
    {
      if (indexName == null)
        throw new ArgumentNullException(nameof (indexName));
      string clusterName = this.GetClusterName();
      List<EsShardDetails> shardsDetails = new List<EsShardDetails>();
      IndicesStatsRequest indicesStatsRequest = new IndicesStatsRequest((Indices) Indices.Index(indexName), (Metrics) (IndicesStatsMetric.Store | IndicesStatsMetric.Docs))
      {
        Level = new Level?(Level.Shards)
      };
      IndicesStatsResponse indicesStatsResponse = this.GenericESInvoker<IndicesStatsResponse>(executionContext, (Func<IndicesStatsResponse>) (() => this.ElasticClient.Indices.Stats((IIndicesStatsRequest) indicesStatsRequest)), 1083039);
      if (indicesStatsResponse.Indices.Count <= 0)
        return shardsDetails;
      foreach (KeyValuePair<string, ShardStats[]> shard in (IEnumerable<KeyValuePair<string, ShardStats[]>>) indicesStatsResponse.Indices.First<KeyValuePair<string, IndicesStats>>().Value.Shards)
      {
        ShardStats shardStats = ((IEnumerable<ShardStats>) shard.Value).First<ShardStats>((Func<ShardStats, bool>) (i => i.Routing.Primary));
        shardsDetails.Add(new EsShardDetails(clusterName, indexName, short.Parse(shard.Key, (IFormatProvider) CultureInfo.InvariantCulture), (int) shardStats.Documents.Count, (int) shardStats.Documents.Deleted, shardStats.Store.SizeInBytes));
      }
      return shardsDetails;
    }

    public long GetShardSizeInBytes(
      ExecutionContext executionContext,
      string indexName,
      string shard)
    {
      if (indexName == null)
        throw new ArgumentNullException(nameof (indexName));
      if (shard == null)
        throw new ArgumentNullException(nameof (shard));
      IndicesStatsRequest indicesStatsRequest = new IndicesStatsRequest((Indices) Indices.Index(indexName), (Metrics) IndicesStatsMetric.Store)
      {
        Level = new Level?(Level.Shards)
      };
      IndicesStatsResponse indicesStatsResponse = this.GenericESInvoker<IndicesStatsResponse>(executionContext, (Func<IndicesStatsResponse>) (() => this.ElasticClient.Indices.Stats((IIndicesStatsRequest) indicesStatsRequest)), 1083037);
      return this.InvokeFunctionWithSearchPlatformExceptionWrapper<long>((Func<long>) (() => ((IEnumerable<ShardStats>) indicesStatsResponse.Indices[indexName].Shards.First<KeyValuePair<string, ShardStats[]>>((Func<KeyValuePair<string, ShardStats[]>, bool>) (i => i.Key.Equals(shard, StringComparison.Ordinal))).Value).First<ShardStats>((Func<ShardStats, bool>) (j => j.Routing.Primary)).Store.SizeInBytes), SearchServiceErrorCode.ElasticsearchClusterStateServiceError);
    }

    public long GetIndexSizeInBytes(ExecutionContext executionContext, string indexName)
    {
      IndicesStatsRequest indicesStatsRequest = indexName != null ? new IndicesStatsRequest((Indices) Indices.Index(indexName), (Metrics) IndicesStatsMetric.Store)
      {
        Level = new Level?(Level.Indices)
      } : throw new ArgumentNullException(nameof (indexName));
      IndicesStatsResponse indicesStatsResponse = this.GenericESInvoker<IndicesStatsResponse>(executionContext, (Func<IndicesStatsResponse>) (() => this.ElasticClient.Indices.Stats((IIndicesStatsRequest) indicesStatsRequest)), 1083037);
      return this.InvokeFunctionWithSearchPlatformExceptionWrapper<long>((Func<long>) (() =>
      {
        double? sizeInBytes = indicesStatsResponse?.Indices?[indexName]?.Primaries?.Store?.SizeInBytes;
        return sizeInBytes.HasValue ? (long) sizeInBytes.Value : 0L;
      }), SearchServiceErrorCode.ElasticsearchClusterStateServiceError);
    }

    public Task<ForceMergeResponse> ForceMergeIndicesAsync(List<string> indices)
    {
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      ForceMergeDescriptor forceMergeDescriptor = new ForceMergeDescriptor().OnlyExpungeDeletes();
      return this.ElasticClient.Indices.ForceMergeAsync((Indices) Indices.Index((IEnumerable<string>) indices), (Func<ForceMergeDescriptor, IForceMergeRequest>) (s => (IForceMergeRequest) forceMergeDescriptor));
    }

    public List<KeyValuePair<string, long>> GetFieldWiseDocumentCount(
      ExecutionContext executionContext,
      string indexName,
      DocumentContractType contractType,
      string fieldName,
      string routing = null)
    {
      if (indexName == null)
        throw new ArgumentNullException(nameof (indexName));
      if (fieldName == null)
        throw new ArgumentNullException(nameof (fieldName));
      SearchDescriptor<object> searchDescriptor = ElasticsearchClusterManagementService.CreateSearchDescriptor(indexName, fieldName, routing);
      return ElasticsearchClusterManagementService.GetFieldValueDocumentCountMap(this.GenericESInvoker<ISearchResponse<object>>(executionContext, (Func<ISearchResponse<object>>) (() => this.ElasticClient.Search<object>((Func<SearchDescriptor<object>, ISearchRequest>) (s => (ISearchRequest) searchDescriptor))), 1083037));
    }

    private static List<KeyValuePair<string, long>> GetFieldValueDocumentCountMap(
      ISearchResponse<object> queryResponse)
    {
      TermsAggregate<string> termsAggregate = queryResponse.Aggregations.Terms("term_aggs");
      return termsAggregate.Buckets.Any<KeyedBucket<string>>() ? termsAggregate.Buckets.Select<KeyedBucket<string>, KeyValuePair<string, long>>((Func<KeyedBucket<string>, KeyValuePair<string, long>>) (bucket => new KeyValuePair<string, long>(bucket.Key, bucket.DocCount.Value))).ToList<KeyValuePair<string, long>>() : new List<KeyValuePair<string, long>>();
    }

    public IDictionary<string, string> GetFieldValues(
      string termName,
      string termValue,
      string[] fields,
      string indexName,
      DocumentContractType contractType,
      string routing = null)
    {
      if (termName == null)
        throw new ArgumentNullException(nameof (termName));
      if (termValue == null)
        throw new ArgumentNullException(nameof (termValue));
      if (fields == null)
        throw new ArgumentNullException(nameof (fields));
      SearchDescriptor<object> searchDescriptor = indexName != null ? ElasticsearchClusterManagementService.CreateSearchDescriptor(indexName) : throw new ArgumentNullException(nameof (indexName));
      searchDescriptor.Query((Func<QueryContainerDescriptor<object>, QueryContainer>) (s => new QueryContainerDescriptor<object>().Term((Func<TermQueryDescriptor<object>, ITermQuery>) (q => (ITermQuery) new TermQueryDescriptor<object>().Field((Field) termName).Value((object) termValue))))).Size(new int?(1)).StoredFields((Fields) fields);
      ISearchResponse<object> searchResponse = this.ElasticClient.Search<object>((Func<SearchDescriptor<object>, ISearchRequest>) (s => (ISearchRequest) searchDescriptor));
      IDictionary<string, string> fieldValues = (IDictionary<string, string>) new Dictionary<string, string>();
      foreach (string field in fields)
        fieldValues.Add(field, searchResponse.Hits.First<IHit<object>>().Fields.Value<string>((Field) field));
      return fieldValues;
    }

    public NodesStatsResponse GetClusterResourceUsage() => this.ElasticClient.Nodes.Stats((INodesStatsRequest) new NodesStatsRequest((Metrics) (NodesStatsMetric.Jvm | NodesStatsMetric.Os)));

    private static Func<AggregationContainerDescriptor<object>, AggregationContainerDescriptor<object>> GetAggregator(
      string aggregateTermName)
    {
      return (Func<AggregationContainerDescriptor<object>, AggregationContainerDescriptor<object>>) (agg => agg.Terms("term_aggs", (Func<TermsAggregationDescriptor<object>, ITermsAggregation>) (tad => (ITermsAggregation) tad.Field((Field) aggregateTermName).Size(new int?(6000)))));
    }

    private static SearchDescriptor<object> CreateSearchDescriptor(
      string indexName,
      string aggregateTermName = null,
      string routing = null)
    {
      SearchDescriptor<object> searchDescriptor = new SearchDescriptor<object>().Timeout("300s").Skip(new int?(0)).Take(new int?(0)).Index((Indices) Indices.Index(indexName));
      if (!string.IsNullOrWhiteSpace(aggregateTermName))
        searchDescriptor.Aggregations((Func<AggregationContainerDescriptor<object>, IAggregationContainer>) ElasticsearchClusterManagementService.GetAggregator(aggregateTermName));
      if (!string.IsNullOrWhiteSpace(routing))
        searchDescriptor = searchDescriptor.Routing((Routing) routing);
      return searchDescriptor;
    }

    public string GetClusterName()
    {
      if (this.m_clusterName == null)
      {
        lock (this.m_lock)
        {
          if (this.m_clusterName == null)
            this.m_clusterName = this.ElasticClient.Cluster.Health().ClusterName;
        }
      }
      return this.m_clusterName;
    }

    public CloseIndexResponse CloseIndex(
      ExecutionContext executionContext,
      IndexIdentity indexIdentity)
    {
      if (string.IsNullOrWhiteSpace(indexIdentity?.Name))
        throw new ArgumentException("indexIdentity cannot be null or contain only whitespace");
      Tracer.TraceEnter(1082252, "Search Engine", "Search Engine", nameof (CloseIndex));
      CloseIndexResponse response = (CloseIndexResponse) null;
      try
      {
        if (!this.IndexExists(executionContext, indexIdentity))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Index: '{0}' does not exist", (object) indexIdentity));
        try
        {
          ElasticSearchClientWrapper elasticSearchClientWrapper = new ElasticSearchClientWrapper(executionContext, this.ElasticClient);
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          response = GenericInvoker.Instance.InvokeWithFaultCheck<CloseIndexResponse>((Func<CloseIndexResponse>) (() => elasticSearchClientWrapper.CloseIndex(indexIdentity)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082254, "Search Engine", "Search Engine"), ElasticsearchClusterManagementService.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse ?? (ElasticsearchClusterManagementService.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse = new Action<CloseIndexResponse>(ElasticSearchBasePlatform.AnalyzeResponse)));
          return response;
        }
        finally
        {
          Tracer.TraceInfo(1082254, "Search Engine", "Search Engine", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Closing index '{0}'", (object) indexIdentity.Name));
          response.ThrowOnInvalidOrFailedResponse();
        }
      }
      finally
      {
        Tracer.TraceLeave(1082253, "Search Engine", "Search Engine", nameof (CloseIndex));
      }
    }

    public OpenIndexResponse OpenIndex(
      ExecutionContext executionContext,
      IndexIdentity indexIdentity)
    {
      if (string.IsNullOrWhiteSpace(indexIdentity?.Name))
        throw new ArgumentException("indexIdentity cannot be null or contain only whitespace");
      Tracer.TraceEnter(1083067, "Search Engine", "Search Engine", nameof (OpenIndex));
      OpenIndexResponse response = (OpenIndexResponse) null;
      try
      {
        if (!this.IndexExists(executionContext, indexIdentity))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Index: '{0}' does not exist", (object) indexIdentity));
        try
        {
          ElasticSearchClientWrapper elasticSearchClientWrapper = new ElasticSearchClientWrapper(executionContext, this.ElasticClient);
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          response = GenericInvoker.Instance.InvokeWithFaultCheck<OpenIndexResponse>((Func<OpenIndexResponse>) (() => elasticSearchClientWrapper.OpenIndex(indexIdentity)), executionContext.FaultService, 2, 1000, new TraceMetaData(1083068, "Search Engine", "Search Engine"), ElasticsearchClusterManagementService.\u003C\u003EO.\u003C1\u003E__AnalyzeResponse ?? (ElasticsearchClusterManagementService.\u003C\u003EO.\u003C1\u003E__AnalyzeResponse = new Action<OpenIndexResponse>(ElasticSearchBasePlatform.AnalyzeResponse)));
          return response;
        }
        finally
        {
          Tracer.TraceInfo(1083068, "Search Engine", "Search Engine", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Opening index '{0}'", (object) indexIdentity.Name));
          response.ThrowOnInvalidOrFailedResponse();
        }
      }
      finally
      {
        Tracer.TraceLeave(1083069, "Search Engine", "Search Engine", nameof (OpenIndex));
      }
    }

    public bool IndexExists(ExecutionContext executionContext, IndexIdentity indexIdentity)
    {
      if (string.IsNullOrWhiteSpace(indexIdentity?.Name))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null or whitespace", (object) nameof (indexIdentity), (object) indexIdentity.Name)), nameof (indexIdentity));
      Tracer.TraceEnter(1082299, "Search Engine", "Search Engine", nameof (IndexExists));
      try
      {
        ElasticSearchClientWrapper elasticSearchClientWrapper = new ElasticSearchClientWrapper(executionContext, this.ElasticClient);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ExistsResponse response = GenericInvoker.Instance.InvokeWithFaultCheck<ExistsResponse>((Func<ExistsResponse>) (() => elasticSearchClientWrapper.IndexExists(indexIdentity)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082300, "Search Engine", "Search Engine"), ElasticsearchClusterManagementService.\u003C\u003EO.\u003C2\u003E__AnalyzeResponse ?? (ElasticsearchClusterManagementService.\u003C\u003EO.\u003C2\u003E__AnalyzeResponse = new Action<ExistsResponse>(ElasticSearchBasePlatform.AnalyzeResponse)));
        response.ThrowOnInvalidOrFailedResponse(true);
        return response.Exists;
      }
      finally
      {
        Tracer.TraceLeave(1082300, "Search Engine", "Search Engine", nameof (IndexExists));
      }
    }
  }
}
