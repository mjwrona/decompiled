// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearchIndex
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations
{
  internal class ElasticSearchIndex : ISearchIndex
  {
    private readonly IElasticClient m_elasticSearchClient;

    public IndexIdentity IndexIdentity { get; }

    internal ElasticSearchIndex(string indexIdentity, IElasticClient elasticSearchClient)
      : this(IndexIdentity.CreateIndexIdentity(indexIdentity), elasticSearchClient)
    {
    }

    internal ElasticSearchIndex(IndexIdentity indexIdentity, IElasticClient elasticSearchClient)
    {
      this.IndexIdentity = indexIdentity;
      this.m_elasticSearchClient = elasticSearchClient;
    }

    public IndexOperationsResponse BulkDeleteByQuery<T>(
      ExecutionContext executionContext,
      BulkDeleteByQueryRequest<T> bulkDeleteByQueryRequest,
      bool forceComplete = false)
      where T : AbstractSearchDocumentContract
    {
      if (bulkDeleteByQueryRequest == null)
        throw new ArgumentNullException(nameof (bulkDeleteByQueryRequest));
      if (bulkDeleteByQueryRequest.Query == null || bulkDeleteByQueryRequest.Query is EmptyExpression)
        throw new ArgumentException("Request query is either null or EmptyExpression.", nameof (bulkDeleteByQueryRequest));
      Tracer.TraceEnter(1082242, "Search Engine", "Search Engine", nameof (BulkDeleteByQuery));
      Stopwatch stopwatch = new Stopwatch();
      bool flag = false;
      IndexOperationsResponse esDeleteManyResponse;
      try
      {
        ElasticSearchClientWrapper elasticSearchClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
        esDeleteManyResponse = GenericInvoker.Instance.InvokeWithFaultCheck<IndexOperationsResponse>((Func<IndexOperationsResponse>) (() => elasticSearchClientWrapper.BulkDeleteByQuery<T>(executionContext.RequestContext, bulkDeleteByQueryRequest, this.IndexIdentity, forceComplete)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082243, "Search Engine", "Search Engine"));
        Tracer.TraceVerboseConditionally(1082280, "Search Engine", "Search Engine", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("Bulk Delete by Query response = [{0}].", (object) esDeleteManyResponse))));
        flag = true;
      }
      finally
      {
        stopwatch.Stop();
        executionContext.ExecutionTracerContext.PublishKpi(flag ? "ESBulkDeleteByQueryTime" : "ESFailedBulkDeleteByQueryTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds, true);
        Tracer.TraceLeave(1082243, "Search Engine", "Search Engine", nameof (BulkDeleteByQuery));
      }
      return esDeleteManyResponse;
    }

    public IndexOperationsResponse BulkScriptUpdateByQuery<T>(
      ExecutionContext executionContext,
      BulkScriptUpdateByQueryRequest<T> scriptUpdateByQueryRequest)
      where T : AbstractSearchDocumentContract
    {
      if (scriptUpdateByQueryRequest == null)
        throw new ArgumentNullException(nameof (scriptUpdateByQueryRequest));
      if (scriptUpdateByQueryRequest.Query == null || scriptUpdateByQueryRequest.Query is EmptyExpression)
        throw new ArgumentException("Request query is either null or EmptyExpression.", nameof (scriptUpdateByQueryRequest));
      Tracer.TraceEnter(1082274, "Search Engine", "Search Engine", nameof (BulkScriptUpdateByQuery));
      IndexOperationsResponse operationsResponse = (IndexOperationsResponse) null;
      bool flag = false;
      try
      {
        if (scriptUpdateByQueryRequest.Query == null)
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null", (object) nameof (scriptUpdateByQueryRequest), (object) "Query")));
        Stopwatch stopwatch = new Stopwatch();
        scriptUpdateByQueryRequest.IndexIdentity = this.IndexIdentity;
        try
        {
          ElasticSearchClientWrapper elasticSearchClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
          operationsResponse = GenericInvoker.Instance.InvokeWithFaultCheck<IndexOperationsResponse>((Func<IndexOperationsResponse>) (() => elasticSearchClientWrapper.BulkScriptUpdateByQuery<T>(executionContext.RequestContext, scriptUpdateByQueryRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082275, "Search Engine", "Search Engine"));
          flag = true;
        }
        finally
        {
          stopwatch.Stop();
          if (flag)
            executionContext.ExecutionTracerContext.PublishKpi("ESScriptUpdateByQueryTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
          else
            executionContext.ExecutionTracerContext.PublishKpi("ESFailedScriptUpdateByQueryTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
        }
      }
      finally
      {
        Tracer.TraceLeave(1082275, "Search Engine", "Search Engine", nameof (BulkScriptUpdateByQuery));
      }
      return operationsResponse;
    }

    public IndexOperationsResponse BulkDelete<T>(
      ExecutionContext executionContext,
      BulkDeleteRequest<T> bulkDeleteRequest)
      where T : AbstractSearchDocumentContract
    {
      Tracer.TraceEnter(1082236, "Search Engine", "Search Engine", nameof (BulkDelete));
      Stopwatch stopwatch = new Stopwatch();
      bool flag = false;
      BulkResponse response = (BulkResponse) null;
      IndexOperationsResponse operationsResponse1;
      try
      {
        if (bulkDeleteRequest.Batch != null)
        {
          if (bulkDeleteRequest.Batch.Any<T>())
          {
            try
            {
              ElasticSearchClientWrapper elasticSearchClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
              stopwatch.Start();
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              response = GenericInvoker.Instance.InvokeWithFaultCheck<BulkResponse>((Func<BulkResponse>) (() => elasticSearchClientWrapper.BulkDelete<T>(bulkDeleteRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082237, "Search Engine", "Search Engine"), ElasticSearchIndex.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse ?? (ElasticSearchIndex.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse = new Action<BulkResponse>(ElasticSearchIndex.AnalyzeResponse)));
              flag = true;
            }
            finally
            {
              stopwatch.Stop();
              if (flag)
              {
                response.ThrowOnInvalidOrFailedResponse();
                executionContext.ExecutionTracerContext.PublishKpi("ESBulkIndexingTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds, true);
              }
              else
                executionContext.ExecutionTracerContext.PublishKpi("ESFailedBulkIndexingTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
            }
            if (response.IsValid)
            {
              operationsResponse1 = new IndexOperationsResponse()
              {
                Success = true,
                FailedItemsCount = 0L,
                ItemsCount = (long) bulkDeleteRequest.Batch.Count<T>()
              };
              goto label_14;
            }
            else
            {
              IEnumerable<BulkResponseItemBase> itemsWithErrors1 = response.ItemsWithErrors;
              long num = itemsWithErrors1 != null ? (long) itemsWithErrors1.Count<BulkResponseItemBase>() : 0L;
              operationsResponse1 = new IndexOperationsResponse()
              {
                Success = false,
                FailedItemsCount = num,
                ItemsCount = (long) bulkDeleteRequest.Batch.Count<T>() - num
              };
              if (operationsResponse1.FailedItemsCount != 0L)
              {
                IndexOperationsResponse operationsResponse2 = operationsResponse1;
                IEnumerable<BulkResponseItemBase> itemsWithErrors2 = response.ItemsWithErrors;
                IEnumerable<FailedItem> failedItems = itemsWithErrors2 != null ? itemsWithErrors2.Select<BulkResponseItemBase, FailedItem>((Func<BulkResponseItemBase, FailedItem>) (failedItem => new FailedItem()
                {
                  File = (AbstractSearchDocumentContract) bulkDeleteRequest.Batch.FirstOrDefault<T>((Func<T, bool>) (x => x.DocumentId.Equals(failedItem.Id, StringComparison.Ordinal))),
                  Status = failedItem.Status,
                  Error = failedItem.Error
                })) : (IEnumerable<FailedItem>) null;
                operationsResponse2.FailedItems = failedItems;
                goto label_14;
              }
              else
                goto label_14;
            }
          }
        }
        throw new ArgumentException("bulkDelete.DocsIds");
      }
      finally
      {
        Tracer.TraceLeave(1082237, "Search Engine", "Search Engine", nameof (BulkDelete));
      }
label_14:
      return operationsResponse1;
    }

    public IndexOperationsResponse BulkIndexSync<T>(
      ExecutionContext executionContext,
      BulkIndexSyncRequest<T> bulkIndexSyncRequest)
      where T : AbstractSearchDocumentContract
    {
      Tracer.TraceEnter(1082232, "Search Engine", "Search Engine", nameof (BulkIndexSync));
      Stopwatch stopwatch = new Stopwatch();
      bool flag = false;
      BulkResponse response = (BulkResponse) null;
      IndexOperationsResponse operationsResponse1 = (IndexOperationsResponse) null;
      try
      {
        if (bulkIndexSyncRequest.Batch != null)
        {
          if (bulkIndexSyncRequest.Batch.Any<T>())
          {
            try
            {
              ElasticSearchClientWrapper esClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
              stopwatch.Start();
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              response = GenericInvoker.Instance.InvokeWithFaultCheck<BulkResponse>((Func<BulkResponse>) (() => esClientWrapper.BulkIndexSync<T>(bulkIndexSyncRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082233, "Search Engine", "Search Engine"), ElasticSearchIndex.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse ?? (ElasticSearchIndex.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse = new Action<BulkResponse>(ElasticSearchIndex.AnalyzeResponse)));
              flag = true;
            }
            finally
            {
              stopwatch.Stop();
              if (response != null && response.ApiCall != null && response.ApiCall.RequestBodyInBytes != null && response.IsValid && !response.ItemsWithErrors.Any<BulkResponseItemBase>())
                executionContext.ExecutionTracerContext.PublishClientTrace("Search Engine", "IndexingOperation", "SerializedRequestSizeInNest", (object) response.ApiCall.RequestBodyInBytes.Length);
              if (flag)
              {
                response.ThrowOnInvalidOrFailedResponse();
                executionContext.ExecutionTracerContext.PublishKpi("ESBulkIndexingTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds, true);
              }
              else
                executionContext.ExecutionTracerContext.PublishKpi("ESFailedBulkIndexingTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
            }
            IndexOperationsResponse operationsResponse2 = new IndexOperationsResponse();
            operationsResponse2.Success = response.IsValid;
            IEnumerable<BulkResponseItemBase> itemsWithErrors1 = response.ItemsWithErrors;
            operationsResponse2.FailedItemsCount = itemsWithErrors1 != null ? (long) itemsWithErrors1.Count<BulkResponseItemBase>() : 0L;
            IReadOnlyList<BulkResponseItemBase> items = response.Items;
            operationsResponse2.ItemsCount = items != null ? (long) items.Count<BulkResponseItemBase>() : 0L;
            operationsResponse1 = operationsResponse2;
            if (operationsResponse1.FailedItemsCount != 0L)
            {
              IndexOperationsResponse operationsResponse3 = operationsResponse1;
              IEnumerable<BulkResponseItemBase> itemsWithErrors2 = response.ItemsWithErrors;
              IEnumerable<FailedItem> failedItems = itemsWithErrors2 != null ? itemsWithErrors2.Select<BulkResponseItemBase, FailedItem>((Func<BulkResponseItemBase, FailedItem>) (failedItem => new FailedItem()
              {
                File = (AbstractSearchDocumentContract) bulkIndexSyncRequest.Batch.FirstOrDefault<T>((Func<T, bool>) (x => x.DocumentId.Equals(failedItem.Id, StringComparison.Ordinal))),
                Status = failedItem.Status,
                Error = failedItem.Error
              })) : (IEnumerable<FailedItem>) null;
              operationsResponse3.FailedItems = failedItems;
              goto label_14;
            }
            else
              goto label_14;
          }
        }
        throw new ArgumentException("bulkIndexSyncRequest.Batch");
      }
      finally
      {
        Tracer.TraceLeave(1082233, "Search Engine", "Search Engine", nameof (BulkIndexSync));
      }
label_14:
      return operationsResponse1;
    }

    public IndexOperationsResponse BulkScriptUpdateSync<T>(
      ExecutionContext executionContext,
      BulkScriptUpdateRequest<T> bulkScriptUpdateRequest)
      where T : AbstractSearchDocumentContract
    {
      Tracer.TraceEnter(1082288, "Search Engine", "Search Engine", nameof (BulkScriptUpdateSync));
      Stopwatch stopwatch = new Stopwatch();
      bool flag = false;
      BulkResponse response = (BulkResponse) null;
      IndexOperationsResponse operationsResponse1 = (IndexOperationsResponse) null;
      try
      {
        if (bulkScriptUpdateRequest.Batch != null)
        {
          if (bulkScriptUpdateRequest.Batch.Any<T>())
          {
            try
            {
              ElasticSearchClientWrapper esClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
              stopwatch.Start();
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              response = GenericInvoker.Instance.InvokeWithFaultCheck<BulkResponse>((Func<BulkResponse>) (() => esClientWrapper.BulkScriptUpdateSync<T>(bulkScriptUpdateRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082289, "Search Engine", "Search Engine"), ElasticSearchIndex.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse ?? (ElasticSearchIndex.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse = new Action<BulkResponse>(ElasticSearchIndex.AnalyzeResponse)));
              flag = true;
            }
            finally
            {
              stopwatch.Stop();
              if (flag)
              {
                response.ThrowOnInvalidOrFailedResponse();
                Tracer.PublishKpi("ESBulkScriptUpdateTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
              }
              else
                Tracer.PublishKpi("ESFailedBulkScriptUpdateTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
            }
            if (response == null)
              return new IndexOperationsResponse()
              {
                Success = false,
                FailedItemsCount = (long) bulkScriptUpdateRequest.Batch.Count<T>(),
                ItemsCount = 0
              };
            IndexOperationsResponse operationsResponse2 = new IndexOperationsResponse();
            operationsResponse2.Success = response.IsValid;
            IEnumerable<BulkResponseItemBase> itemsWithErrors1 = response.ItemsWithErrors;
            operationsResponse2.FailedItemsCount = itemsWithErrors1 != null ? (long) itemsWithErrors1.Count<BulkResponseItemBase>() : 0L;
            IReadOnlyList<BulkResponseItemBase> items = response.Items;
            operationsResponse2.ItemsCount = items != null ? (long) items.Count<BulkResponseItemBase>() : 0L;
            operationsResponse1 = operationsResponse2;
            if (operationsResponse1.FailedItemsCount != 0L)
            {
              IndexOperationsResponse operationsResponse3 = operationsResponse1;
              IEnumerable<BulkResponseItemBase> itemsWithErrors2 = response.ItemsWithErrors;
              IEnumerable<FailedItem> failedItems = itemsWithErrors2 != null ? itemsWithErrors2.Select<BulkResponseItemBase, FailedItem>((Func<BulkResponseItemBase, FailedItem>) (failedItem => new FailedItem()
              {
                File = (AbstractSearchDocumentContract) bulkScriptUpdateRequest.Batch.FirstOrDefault<T>((Func<T, bool>) (x => x.DocumentId.Equals(failedItem.Id, StringComparison.Ordinal))),
                Status = failedItem.Status,
                Error = failedItem.Error
              })) : (IEnumerable<FailedItem>) null;
              operationsResponse3.FailedItems = failedItems;
              goto label_14;
            }
            else
              goto label_14;
          }
        }
        throw new ArgumentException("BulkScriptUpdateRequest.Batch");
      }
      finally
      {
        Tracer.TraceLeave(1082289, "Search Engine", "Search Engine", nameof (BulkScriptUpdateSync));
      }
label_14:
      return operationsResponse1;
    }

    public EntitySearchGetDocumentsResponse<T> GetDocuments<T>(
      ExecutionContext executionContext,
      EntitySearchGetDocumentsRequest request)
      where T : AbstractSearchDocumentContract
    {
      EntitySearchGetDocumentsResponse<T> documents = (EntitySearchGetDocumentsResponse<T>) null;
      Stopwatch stopwatch = new Stopwatch();
      bool flag = false;
      Tracer.TraceEnter(1083025, "Search Engine", "Search Engine", nameof (GetDocuments));
      try
      {
        if (request != null && request.DocumentIds != null)
        {
          if (request.DocumentIds.Any<string>())
          {
            try
            {
              stopwatch.Start();
              ElasticSearchClientWrapper esClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
              documents = GenericInvoker.Instance.InvokeWithFaultCheck<EntitySearchGetDocumentsResponse<T>>((Func<EntitySearchGetDocumentsResponse<T>>) (() => esClientWrapper.GetDocuments<T>(request)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082233, "Search Engine", "Search Engine"));
              flag = true;
              goto label_12;
            }
            finally
            {
              stopwatch.Stop();
              if (flag)
                Tracer.PublishKpi("ESGetDocumentsTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
              else
                Tracer.PublishKpi("ESFailedGetDocumentsTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
              if (documents == null)
                documents = new EntitySearchGetDocumentsResponse<T>()
                {
                  Documents = (IDictionary<string, T>) new Dictionary<string, T>()
                };
            }
          }
        }
        Tracer.TraceError(1083025, "Search Engine", "Search Engine", "Invalid arguments");
        throw new ArgumentException("EntitySearchGetDocumentsRequest.DocumentIds");
      }
      finally
      {
        Tracer.TraceLeave(1083026, "Search Engine", "Search Engine", nameof (GetDocuments));
      }
label_12:
      return documents;
    }

    public IndexOperationsResponse BulkUpdateSync<T>(
      ExecutionContext executionContext,
      BulkUpdateRequest<T> bulkUpdateRequest)
      where T : AbstractSearchDocumentContract
    {
      Tracer.TraceEnter(1082288, "Search Engine", "Search Engine", nameof (BulkUpdateSync));
      Stopwatch stopwatch = new Stopwatch();
      bool flag = false;
      BulkResponse response = (BulkResponse) null;
      IndexOperationsResponse operationsResponse1 = (IndexOperationsResponse) null;
      try
      {
        if (bulkUpdateRequest.Batch != null)
        {
          if (bulkUpdateRequest.Batch.Any<T>())
          {
            try
            {
              ElasticSearchClientWrapper esClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
              stopwatch.Start();
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              response = GenericInvoker.Instance.InvokeWithFaultCheck<BulkResponse>((Func<BulkResponse>) (() => esClientWrapper.BulkUpdateSync<T>(bulkUpdateRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082289, "Search Engine", "Search Engine"), ElasticSearchIndex.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse ?? (ElasticSearchIndex.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse = new Action<BulkResponse>(ElasticSearchIndex.AnalyzeResponse)));
              flag = true;
            }
            finally
            {
              stopwatch.Stop();
              if (flag)
              {
                response.ThrowOnInvalidOrFailedResponse();
                Tracer.PublishKpi("ESBulkScriptUpdateTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
              }
              else
                Tracer.PublishKpi("ESFailedBulkUpdateTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
            }
            if (response == null)
              return new IndexOperationsResponse()
              {
                Success = false,
                FailedItemsCount = (long) bulkUpdateRequest.Batch.Count<T>(),
                ItemsCount = 0
              };
            IndexOperationsResponse operationsResponse2 = new IndexOperationsResponse();
            operationsResponse2.Success = response.IsValid;
            IEnumerable<BulkResponseItemBase> itemsWithErrors1 = response.ItemsWithErrors;
            operationsResponse2.FailedItemsCount = itemsWithErrors1 != null ? (long) itemsWithErrors1.Count<BulkResponseItemBase>() : 0L;
            IReadOnlyList<BulkResponseItemBase> items = response.Items;
            operationsResponse2.ItemsCount = items != null ? (long) items.Count<BulkResponseItemBase>() : 0L;
            operationsResponse1 = operationsResponse2;
            if (operationsResponse1.FailedItemsCount != 0L)
            {
              IndexOperationsResponse operationsResponse3 = operationsResponse1;
              IEnumerable<BulkResponseItemBase> itemsWithErrors2 = response.ItemsWithErrors;
              IEnumerable<FailedItem> failedItems = itemsWithErrors2 != null ? itemsWithErrors2.Select<BulkResponseItemBase, FailedItem>((Func<BulkResponseItemBase, FailedItem>) (failedItem => new FailedItem()
              {
                File = (AbstractSearchDocumentContract) bulkUpdateRequest.Batch.FirstOrDefault<T>((Func<T, bool>) (x => x.DocumentId.Equals(failedItem.Id, StringComparison.Ordinal))),
                Status = failedItem.Status,
                Error = failedItem.Error
              })) : (IEnumerable<FailedItem>) null;
              operationsResponse3.FailedItems = failedItems;
              goto label_14;
            }
            else
              goto label_14;
          }
        }
        throw new ArgumentException("bulkUpdateRequest.Batch");
      }
      finally
      {
        Tracer.TraceLeave(1082295, "Search Engine", "Search Engine", nameof (BulkUpdateSync));
      }
label_14:
      return operationsResponse1;
    }

    public IIndexSettings GetSettings()
    {
      Tracer.TraceEnter(1082227, "Search Engine", "Search Engine", nameof (GetSettings));
      bool flag = false;
      GetIndexSettingsResponse response = (GetIndexSettingsResponse) null;
      try
      {
        response = this.m_elasticSearchClient.Indices.GetSettings((Indices) this.IndexIdentity.Name, (Func<GetIndexSettingsDescriptor, IGetIndexSettingsRequest>) (i => (IGetIndexSettingsRequest) i.Index((Indices) this.IndexIdentity.Name)));
        flag = true;
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      finally
      {
        try
        {
          if (flag)
            response.ThrowOnInvalidOrFailedResponse();
        }
        finally
        {
          Tracer.TraceLeave(1082228, "Search Engine", "Search Engine", nameof (GetSettings));
        }
      }
      return response.Indices.First<KeyValuePair<IndexName, IndexState>>((Func<KeyValuePair<IndexName, IndexState>, bool>) (i => i.Key.Equals((object) this.IndexIdentity.Name))).Value.Settings;
    }

    public IndexOperationsResponse Refresh(ExecutionContext executionContext)
    {
      Tracer.TraceEnter(1082207, "Search Engine", "Search Engine", nameof (Refresh));
      try
      {
        ElasticSearchClientWrapper elasticSearchClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
        GenericInvoker.Instance.InvokeWithFaultCheck<RefreshResponse>((Func<RefreshResponse>) (() => elasticSearchClientWrapper.Refresh(this.IndexIdentity)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082226, "Search Engine", "Search Engine"));
        return new IndexOperationsResponse()
        {
          Success = true
        };
      }
      finally
      {
        Tracer.TraceLeave(1082208, "Search Engine", "Search Engine", nameof (Refresh));
      }
    }

    public int GetNumberOfCollections(DocumentContractType contractType)
    {
      Tracer.TraceEnter(1082248, "Search Engine", "Search Engine", nameof (GetNumberOfCollections));
      int numberOfCollections = -1;
      try
      {
        SearchDescriptor<object> searchDescriptor = new SearchDescriptor<object>().Index((Indices) this.IndexIdentity.Name).Size(new int?(0)).Aggregations((Func<AggregationContainerDescriptor<object>, IAggregationContainer>) (a => (IAggregationContainer) a.Cardinality("collection_aggs", (Func<CardinalityAggregationDescriptor<object>, ICardinalityAggregation>) (c => (ICardinalityAggregation) c.Field((Field) "collectionId")))));
        ISearchResponse<object> searchResponse = this.m_elasticSearchClient.Search<object>((Func<SearchDescriptor<object>, ISearchRequest>) (s => (ISearchRequest) searchDescriptor));
        if (searchResponse.Aggregations.Count == 1)
        {
          if (searchResponse.Aggregations.Keys.Contains<string>("collection_aggs"))
            numberOfCollections = (int) (searchResponse.Aggregations["collection_aggs"] as ValueAggregate).Value.Value;
        }
      }
      catch (Exception ex)
      {
        throw new SearchPlatformException("Query to ElasticSearch for getting number of collections in index failed", ex);
      }
      finally
      {
        Tracer.TraceLeave(1082249, "Search Engine", "Search Engine", nameof (GetNumberOfCollections));
      }
      return numberOfCollections;
    }

    public bool UpdateSettings(ExecutionContext executionContext, string property, object newValue)
    {
      if (string.IsNullOrWhiteSpace(property))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Index setting name cannot be null or whitespace")), nameof (property));
      Tracer.TraceEnter(1082205, "Search Engine", "Search Engine", nameof (UpdateSettings));
      try
      {
        ElasticSearchClientWrapper elasticSearchClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
        return GenericInvoker.Instance.InvokeWithFaultCheck<bool>((Func<bool>) (() => elasticSearchClientWrapper.SetIndexSetting(this.IndexIdentity.Name, property, newValue)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082269, "Search Engine", "Search Engine"));
      }
      finally
      {
        Tracer.TraceLeave(1082206, "Search Engine", "Search Engine", nameof (UpdateSettings));
      }
    }

    public HealthStatus GetHealth()
    {
      Tracer.TraceEnter(1082240, "Search Engine", "Search Engine", nameof (GetHealth));
      try
      {
        switch (this.m_elasticSearchClient.Cluster.Health((Indices) this.IndexIdentity.Name, (Func<ClusterHealthDescriptor, IClusterHealthRequest>) (i => (IClusterHealthRequest) i.Index((Indices) this.IndexIdentity.Name).WaitForStatus(new WaitForStatus?(WaitForStatus.Yellow)).Timeout((Time) "300s"))).Status)
        {
          case Health.Green:
            return HealthStatus.Green;
          case Health.Yellow:
            return HealthStatus.Yellow;
          case Health.Red:
            return HealthStatus.Red;
          default:
            return HealthStatus.Unknown;
        }
      }
      finally
      {
        Tracer.TraceLeave(1082239, "Search Engine", "Search Engine", nameof (GetHealth));
      }
    }

    public IDictionary<string, object> GetMetadata()
    {
      Tracer.TraceEnter(1080103, "Search Engine", "Search Engine", nameof (GetMetadata));
      try
      {
        string name = this.IndexIdentity.Name;
        GetMappingRequest request = new GetMappingRequest((Indices) Indices.Index(name));
        request.FilterPath = new string[1]
        {
          FormattableString.Invariant(FormattableStringFactory.Create("{0}.mappings.*._meta", (object) name))
        };
        GetMappingResponse mapping = this.m_elasticSearchClient.Indices.GetMapping((IGetMappingRequest) request);
        mapping.ThrowOnInvalidOrFailedResponse();
        IndexMappings indexMappings;
        if (mapping.Indices.TryGetValue((IndexName) name, out indexMappings))
        {
          if (indexMappings.Mappings != null)
            return indexMappings.Mappings.Meta;
          Tracer.TraceError(1080119, "Search Engine", "Search Engine", FormattableString.Invariant(FormattableStringFactory.Create("No mapping in index [{0}] found in response.", (object) name)));
        }
        else
          Tracer.TraceError(1080119, "Search Engine", "Search Engine", FormattableString.Invariant(FormattableStringFactory.Create("Index [{0}] not found in response from ES cluster [{1}]", (object) name, (object) mapping?.ApiCall?.Uri)));
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, this.IndexIdentity);
      }
      finally
      {
        Tracer.TraceLeave(1080104, "Search Engine", "Search Engine", nameof (GetMetadata));
      }
      return (IDictionary<string, object>) new FriendlyDictionary<string, object>();
    }

    public IEnumerable<T> BulkGetByQuery<T>(
      ExecutionContext executionContext,
      BulkGetByQueryRequest bulkGetByQueryRequest)
      where T : AbstractSearchDocumentContract
    {
      if (bulkGetByQueryRequest == null)
        throw new ArgumentNullException(nameof (bulkGetByQueryRequest));
      Tracer.TraceEnter(1082302, "Search Engine", "Search Engine", nameof (BulkGetByQuery));
      Stopwatch stopwatch = new Stopwatch();
      try
      {
        ElasticSearchClientWrapper esClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
        stopwatch.Start();
        return GenericInvoker.Instance.InvokeWithFaultCheck<IEnumerable<T>>((Func<IEnumerable<T>>) (() => esClientWrapper.BulkGetByQuery<T>(executionContext, bulkGetByQueryRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082302, "Search Engine", "Search Engine"));
      }
      finally
      {
        stopwatch.Stop();
        Tracer.PublishKpi("ESBulkGetByQueryTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
        Tracer.TraceLeave(1082302, "Search Engine", "Search Engine", nameof (BulkGetByQuery));
      }
    }

    public IndexOperationsResponse BulkUpdateByQuery(
      ExecutionContext executionContext,
      BulkUpdateByQueryRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (request.Query == null || request.Query is EmptyExpression)
        throw new ArgumentException("Request query is either null or EmptyExpression.", nameof (request));
      Tracer.TraceEnter(1082282, "Search Engine", "Search Engine", nameof (BulkUpdateByQuery));
      IndexOperationsResponse operationsResponse = new IndexOperationsResponse();
      Stopwatch stopwatch = new Stopwatch();
      try
      {
        if (request == null)
          throw new ArgumentNullException(nameof (request));
        ElasticSearchClientWrapper esClientWrapper = new ElasticSearchClientWrapper(executionContext, this.m_elasticSearchClient);
        stopwatch.Start();
        operationsResponse = GenericInvoker.Instance.InvokeWithFaultCheck<IndexOperationsResponse>((Func<IndexOperationsResponse>) (() => esClientWrapper.BulkUpdateByQuery(executionContext, request)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082283, "Search Engine", "Search Engine"));
        Tracer.TraceVerbose(1082285, "Search Engine", "Search Engine", FormattableString.Invariant(FormattableStringFactory.Create("Bulk Update by Query response = [{0}].", (object) operationsResponse)));
        this.Refresh(executionContext);
      }
      finally
      {
        stopwatch.Stop();
        if (operationsResponse.Success)
          Tracer.PublishKpi("ESBulkUpdateByQueryTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
        else
          Tracer.PublishKpi("ESFailedBulkUpdateByQueryTime", "Search Engine", (double) stopwatch.ElapsedMilliseconds);
        Tracer.TraceLeave(1082284, "Search Engine", "Search Engine", nameof (BulkUpdateByQuery));
      }
      return operationsResponse;
    }

    public long GetIndexedDocumentCount()
    {
      Tracer.TraceEnter(1082230, "Search Engine", "Search Engine", nameof (GetIndexedDocumentCount));
      try
      {
        return this.m_elasticSearchClient.Count((ICountRequest) new CountRequest((Indices) Indices.Index(this.IndexIdentity.Name))).Count;
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      finally
      {
        Tracer.TraceLeave(1082231, "Search Engine", "Search Engine", nameof (GetIndexedDocumentCount));
      }
      return 0;
    }

    public IEnumerable<DocumentContractType> GetDocumentContracts(ExecutionContext executionContext)
    {
      Tracer.TraceEnter(1083051, "Search Engine", "Search Engine", nameof (GetDocumentContracts));
      IList<DocumentContractType> collection = (IList<DocumentContractType>) new List<DocumentContractType>();
      try
      {
        string name = this.IndexIdentity.Name;
        string[] strArray1;
        if (name == null)
          strArray1 = (string[]) null;
        else
          strArray1 = name.Split('_');
        string[] strArray2 = strArray1;
        if (strArray2 != null & strArray2.Length >= 2)
          collection.AddRange<DocumentContractType, IList<DocumentContractType>>((IEnumerable<DocumentContractType>) strArray2[1].GetDocumentContractTypes());
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      finally
      {
        Tracer.TraceLeave(1083052, "Search Engine", "Search Engine", nameof (GetDocumentContracts));
      }
      return (IEnumerable<DocumentContractType>) collection;
    }

    private static void AnalyzeResponse(IResponse resp) => resp.ThrowOnInvalidOrFailedResponse();
  }
}
