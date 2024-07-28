// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.ElasticSearchClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch
{
  public class ElasticSearchClientWrapper
  {
    private static readonly object s_lock = new object();
    [StaticSafe]
    private static AdjustableSemaphore s_bulkThreadQueue;
    private readonly IElasticClient m_elasticSearchClient;
    private readonly ExecutionContext m_executionContext;

    internal FriendlyDictionary<string, FriendlyDictionary<MetaDataStoreUpdateType, int>> NumberOfDocsPerBranch { get; set; }

    internal ElasticSearchClientWrapper(
      ExecutionContext executionContext,
      IElasticClient elasticSearchClient)
    {
      this.m_elasticSearchClient = elasticSearchClient;
      this.m_executionContext = executionContext;
      this.NumberOfDocsPerBranch = new FriendlyDictionary<string, FriendlyDictionary<MetaDataStoreUpdateType, int>>();
      if (ElasticSearchClientWrapper.s_bulkThreadQueue != null)
        return;
      int feederThreadCount = this.GetFeederThreadCount();
      if (feederThreadCount <= 0)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Value of setting MaxNumberOfParallelFeed should be greater than zero. It is [{0}].", (object) feederThreadCount)));
      lock (ElasticSearchClientWrapper.s_lock)
      {
        if (ElasticSearchClientWrapper.s_bulkThreadQueue != null)
          return;
        ElasticSearchClientWrapper.s_bulkThreadQueue = executionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<IAdjustableSemaphoreService>().CreateOrUpdate("ElasticsearchClientWrapper", feederThreadCount);
      }
    }

    internal void SemaphoreWait()
    {
      int feederThreadCount = this.GetFeederThreadCount();
      if (feederThreadCount <= 0)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Value of setting MaxNumberOfParallelFeed should be greater than zero. It is [{0}].", (object) feederThreadCount)));
      lock (ElasticSearchClientWrapper.s_lock)
      {
        int maxCount = ElasticSearchClientWrapper.s_bulkThreadQueue.MaxCount;
        if (maxCount != feederThreadCount)
        {
          ElasticSearchClientWrapper.s_bulkThreadQueue.MaxCount = feederThreadCount;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1082060, "Indexing Pipeline", "Feed", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Instantiated new semaphore. New Max Value: '{0}', Threads Engaged at time of creation: '{1}'", (object) maxCount, (object) ElasticSearchClientWrapper.s_bulkThreadQueue.Engaged));
        }
      }
      ElasticSearchClientWrapper.s_bulkThreadQueue.Wait();
    }

    internal void SemaphoreRelease() => ElasticSearchClientWrapper.s_bulkThreadQueue.Release();

    private int GetFeederThreadCount() => this.m_executionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxNumberOfParallelFeed");

    public int SemaphoreCount => ElasticSearchClientWrapper.s_bulkThreadQueue.MaxCount;

    internal IndexOperationsResponse BulkDeleteByQuery<T>(
      IVssRequestContext requestContext,
      BulkDeleteByQueryRequest<T> request,
      IndexIdentity indexIdentity,
      bool forceComplete = false)
      where T : AbstractSearchDocumentContract
    {
      IndexOperationsResponse response1 = new IndexOperationsResponse()
      {
        Success = true,
        ItemsCount = 0,
        FailedItemsCount = 0,
        IsOperationIncomplete = false
      };
      try
      {
        IEnumerable<List<ElasticSearchClientWrapper.ScrollResponse<object>>> scrollResponseLists = this.ScanAndScrollDocuments<object>(requestContext, indexIdentity.Name, request.ContractType, request.Query, 1000, TimeSpan.FromMinutes(5.0), routing: request.RoutingIds, throwIfIndexDoesNotExist: !request.Lenient);
        Stopwatch timer = Stopwatch.StartNew();
        foreach (List<ElasticSearchClientWrapper.ScrollResponse<object>> scrollResponseList in scrollResponseLists)
        {
          if (!forceComplete && this.m_executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment && this.IsOperationTimedOut(timer, request.RequestTimeOut))
          {
            response1.IsOperationIncomplete = true;
            break;
          }
          BulkDescriptor descriptor = new BulkDescriptor().Index((IndexName) indexIdentity.Name);
          foreach (ElasticSearchClientWrapper.ScrollResponse<object> scrollResponse in scrollResponseList)
          {
            ElasticSearchClientWrapper.ScrollResponse<object> document = scrollResponse;
            descriptor.Delete<object>((Func<BulkDeleteDescriptor<object>, IBulkDeleteOperation<object>>) (d => (IBulkDeleteOperation<object>) d.Id((Id) document.DocumentId).Routing((Routing) document.RoutingId)));
          }
          this.InvokeBulkApiAndUpdateResponse((IBulkRequest) descriptor, response1, scrollResponseList.Count);
        }
        timer.Stop();
        ExistsResponse response2 = this.IndexExists(indexIdentity);
        response2.ThrowOnInvalidOrFailedResponse(true);
        if (response2.Exists)
          this.Refresh(indexIdentity);
        else if (!request.Lenient)
          throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Index [{0}] does not exist.", (object) indexIdentity.Name)), SearchServiceErrorCode.IndexNotExists);
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      return response1.FailedItemsCount <= 0L ? response1 : throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Bulk delete failed. Number of documents [{0}] out of [{1}] failed.", (object) response1.FailedItemsCount, (object) response1.ItemsCount)), SearchServiceErrorCode.ElasticsearchBulkDeleteFailed);
    }

    internal IndexOperationsResponse BulkScriptUpdateByQuery<T>(
      IVssRequestContext requestContext,
      BulkScriptUpdateByQueryRequest<T> request)
      where T : AbstractSearchDocumentContract
    {
      IndexOperationsResponse response = new IndexOperationsResponse()
      {
        Success = true,
        ItemsCount = 0,
        FailedItemsCount = 0
      };
      try
      {
        int currentHostConfigValue = requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/EsBulkUpdateBatchSize", true, 500);
        foreach (List<ElasticSearchClientWrapper.ScrollResponse<object>> andScrollDocument in this.ScanAndScrollDocuments<object>(requestContext, request.IndexIdentity.Name, request.ContractType, request.Query, currentHostConfigValue, TimeSpan.FromMinutes(5.0), routing: new string[1]
        {
          request.Routing
        }))
        {
          BulkDescriptor descriptor = new BulkDescriptor().Index((IndexName) request.IndexIdentity.Name);
          foreach (ElasticSearchClientWrapper.ScrollResponse<object> scrollResponse in andScrollDocument)
          {
            ElasticSearchClientWrapper.ScrollResponse<object> document = scrollResponse;
            descriptor.Update<T>((Func<BulkUpdateDescriptor<T, T>, IBulkUpdateOperation<T, T>>) (x => (IBulkUpdateOperation<T, T>) x.Id((Id) document.DocumentId).Routing((Routing) document.RoutingId).Script(closure_1 ?? (closure_1 = (Func<ScriptDescriptor, IScript>) (s => (IScript) s.Source(((BulkScriptUpdateRequest<T>) request).ScriptName).Lang("almsearch_scripts").Params((Func<FluentDictionary<string, object>, FluentDictionary<string, object>>) (p => this.AddIndexedTimeStampProperty(request.GetParams((AbstractSearchDocumentContract) null)))))))));
          }
          this.InvokeBulkApiAndUpdateResponse((IBulkRequest) descriptor, response);
        }
      }
      catch (Exception ex)
      {
        string str1 = FormattableString.Invariant(FormattableStringFactory.Create("BulkScriptUpdateByQuery: Feeding to [{0}] ", (object) request.IndexIdentity.Name));
        object[] objArray = new object[1];
        IConnectionSettingsValues connectionSettings = this.m_elasticSearchClient.ConnectionSettings;
        string str2;
        if (connectionSettings == null)
        {
          str2 = (string) null;
        }
        else
        {
          IConnectionPool connectionPool = connectionSettings.ConnectionPool;
          if (connectionPool == null)
          {
            str2 = (string) null;
          }
          else
          {
            IReadOnlyCollection<Elasticsearch.Net.Node> nodes = connectionPool.Nodes;
            str2 = nodes != null ? nodes.FirstOrDefault<Elasticsearch.Net.Node>()?.Uri?.AbsoluteUri : (string) null;
          }
        }
        objArray[0] = (object) str2;
        string str3 = FormattableString.Invariant(FormattableStringFactory.Create("on ES Cluster [{0}] ", objArray));
        string str4 = FormattableString.Invariant(FormattableStringFactory.Create("is failed with exception: {0}", (object) ex.ToString()));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082632, "Indexing Pipeline", "Feed", str1 + str3 + str4);
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      return response.FailedItemsCount <= 0L ? response : throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Bulk script update failed. Number of documents [{0}] out of [{1}] failed.", (object) response.FailedItemsCount, (object) response.ItemsCount)), SearchServiceErrorCode.ElasticsearchBulkUpdateFailed);
    }

    internal BulkResponse BulkDelete<T>(BulkDeleteRequest<T> bulkDeleteRequest) where T : AbstractSearchDocumentContract
    {
      BulkResponse bulkResponse = (BulkResponse) null;
      HashSet<string> stringSet = new HashSet<string>(bulkDeleteRequest.Batch.Count<T>(), (IEqualityComparer<string>) StringComparer.Ordinal);
      BulkDescriptor bulkDescriptor = new BulkDescriptor();
      foreach (T obj in bulkDeleteRequest.Batch)
      {
        T doc = obj;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SmartAssert((stringSet.Add(((T) doc).DocumentId) ? 1 : 0) != 0, FormattableString.Invariant(FormattableStringFactory.Create("Document with Id [{0}] was found multiple times in the same bulk API batch. This is indication of a code bug.", (object) ((T) doc).DocumentId)));
        bulkDescriptor.Delete<T>((Func<BulkDeleteDescriptor<T>, IBulkDeleteOperation<T>>) (op => (IBulkDeleteOperation<T>) op.Id((Id) doc.DocumentId).Routing((Routing) doc.Routing)));
      }
      try
      {
        this.SemaphoreWait();
        bulkResponse = this.m_elasticSearchClient.Bulk((Func<BulkDescriptor, IBulkRequest>) (bulk => (IBulkRequest) bulkDescriptor.Index((IndexName) bulkDeleteRequest.IndexIdentity.Name)));
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, bulkDeleteRequest.IndexIdentity);
      }
      finally
      {
        this.SemaphoreRelease();
      }
      return bulkResponse;
    }

    internal BulkResponse BulkIndexSync<T>(BulkIndexSyncRequest<T> bulkIndexSyncRequest) where T : AbstractSearchDocumentContract
    {
      BulkResponse bulkResponse = (BulkResponse) null;
      HashSet<string> stringSet = new HashSet<string>(bulkIndexSyncRequest.Batch.Count<T>(), (IEqualityComparer<string>) StringComparer.Ordinal);
      BulkDescriptor bulkDescriptor = new BulkDescriptor();
      foreach (T obj in bulkIndexSyncRequest.Batch)
      {
        T doc = obj;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SmartAssert((stringSet.Add(((T) doc).DocumentId) ? 1 : 0) != 0, FormattableString.Invariant(FormattableStringFactory.Create("Document with Id [{0}] was found multiple times in the same bulk API batch. This is indication of a code bug.", (object) ((T) doc).DocumentId)));
        this.UpdateIndexedTimeStampProperty((AbstractSearchDocumentContract) doc);
        bulkDescriptor.Index<T>((Func<BulkIndexDescriptor<T>, IBulkIndexOperation<T>>) (op => (IBulkIndexOperation<T>) op.Document(doc).Id((Id) doc.DocumentId).Routing((Routing) doc.Routing).Version(doc.PreviousDocumentVersion)));
      }
      try
      {
        this.SemaphoreWait();
        bulkResponse = this.m_elasticSearchClient.Bulk((Func<BulkDescriptor, IBulkRequest>) (bulk => (IBulkRequest) bulkDescriptor.Index((IndexName) bulkIndexSyncRequest.IndexIdentity.Name)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1082298, "Search Engine", "Search Engine", new Func<string>(((IResponseExtensions) bulkResponse).SerializeRequestAndResponse));
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, bulkIndexSyncRequest.IndexIdentity);
      }
      finally
      {
        this.SemaphoreRelease();
      }
      return bulkResponse;
    }

    internal BulkResponse BulkScriptUpdateSync<T>(BulkScriptUpdateRequest<T> request) where T : AbstractSearchDocumentContract
    {
      BulkResponse bulkResponse = (BulkResponse) null;
      try
      {
        HashSet<string> stringSet = new HashSet<string>(request.Batch.Count<T>(), (IEqualityComparer<string>) StringComparer.Ordinal);
        BulkDescriptor bulkDescriptor = new BulkDescriptor();
        foreach (T obj in request.Batch)
        {
          T file = obj;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SmartAssert((stringSet.Add(((T) file).DocumentId) ? 1 : 0) != 0, FormattableString.Invariant(FormattableStringFactory.Create("Document with Id [{0}] was found multiple times in the same bulk API batch. This is indication of a code bug.", (object) ((T) file).DocumentId)));
          BulkUpdateDescriptor<T, T> fileDescriptor = new BulkUpdateDescriptor<T, T>();
          this.UpdateIndexedTimeStampProperty((AbstractSearchDocumentContract) file);
          fileDescriptor.Script((Func<ScriptDescriptor, IScript>) (s => (IScript) s.Source(request.ScriptName).Lang("almsearch_scripts").Params((Func<FluentDictionary<string, object>, FluentDictionary<string, object>>) (p => this.AddIndexedTimeStampProperty(request.GetParams((AbstractSearchDocumentContract) file)))))).Id((Id) ((T) file).DocumentId).Routing((Routing) ((T) file).Routing);
          if (request.ShouldUpsert)
            fileDescriptor.Upsert(file);
          if (request.ScriptedUpsert)
            fileDescriptor.ScriptedUpsert();
          bulkDescriptor.Update<T>((Func<BulkUpdateDescriptor<T, T>, IBulkUpdateOperation<T, T>>) (x => (IBulkUpdateOperation<T, T>) fileDescriptor));
        }
        this.SemaphoreWait();
        bulkResponse = this.m_elasticSearchClient.Bulk((IBulkRequest) bulkDescriptor.Index((IndexName) request.IndexIdentity.Name));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1082298, "Search Engine", "Search Engine", (Func<string>) (() => this.ScriptParams<T>(request)));
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, request.IndexIdentity);
      }
      finally
      {
        this.SemaphoreRelease();
      }
      return bulkResponse;
    }

    internal BulkResponse BulkUpdateSync<T>(BulkUpdateRequest<T> request) where T : AbstractSearchDocumentContract
    {
      BulkResponse bulkResponse = (BulkResponse) null;
      try
      {
        HashSet<string> stringSet = new HashSet<string>(request.Batch.Count<T>(), (IEqualityComparer<string>) StringComparer.Ordinal);
        BulkDescriptor bulkDescriptor = new BulkDescriptor();
        foreach (T obj in request.Batch)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SmartAssert((stringSet.Add(obj.DocumentId) ? 1 : 0) != 0, FormattableString.Invariant(FormattableStringFactory.Create("Document with Id [{0}] was found multiple times in the same bulk API batch. This is indication of a code bug.", (object) obj.DocumentId)));
          BulkUpdateDescriptor<AbstractSearchDocumentContract, AbstractSearchDocumentContract> fileDescriptor = new BulkUpdateDescriptor<AbstractSearchDocumentContract, AbstractSearchDocumentContract>();
          this.UpdateIndexedTimeStampProperty((AbstractSearchDocumentContract) obj);
          fileDescriptor.Id((Id) obj.DocumentId).Routing((Routing) obj.Routing).Doc((AbstractSearchDocumentContract) obj);
          if (request.ShouldUpsert)
            fileDescriptor.Upsert((AbstractSearchDocumentContract) obj);
          bulkDescriptor.Update<AbstractSearchDocumentContract>((Func<BulkUpdateDescriptor<AbstractSearchDocumentContract, AbstractSearchDocumentContract>, IBulkUpdateOperation<AbstractSearchDocumentContract, AbstractSearchDocumentContract>>) (x => (IBulkUpdateOperation<AbstractSearchDocumentContract, AbstractSearchDocumentContract>) fileDescriptor));
        }
        this.SemaphoreWait();
        bulkResponse = this.m_elasticSearchClient.Bulk((IBulkRequest) bulkDescriptor.Index((IndexName) request.IndexIdentity.Name));
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, request.IndexIdentity);
      }
      finally
      {
        this.SemaphoreRelease();
      }
      return bulkResponse;
    }

    internal EntitySearchGetDocumentsResponse<T> GetDocuments<T>(
      EntitySearchGetDocumentsRequest request)
      where T : AbstractSearchDocumentContract
    {
      EntitySearchGetDocumentsResponse<T> documents = (EntitySearchGetDocumentsResponse<T>) null;
      try
      {
        HashSet<string> stringSet = new HashSet<string>(request.DocumentIds.Count<string>(), (IEqualityComparer<string>) StringComparer.Ordinal);
        foreach (string documentId in request.DocumentIds)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SmartAssert((stringSet.Add(documentId) ? 1 : 0) != 0, FormattableString.Invariant(FormattableStringFactory.Create("Document with Id [{0}] was found multiple times in the same bulk API batch. This is indication of a code bug.", (object) documentId)));
        MultiGetResponse multiGetResponse = this.m_elasticSearchClient.MultiGet((IMultiGetRequest) new MultiGetDescriptor().Index((IndexName) request.IndexIdentity.Name).GetMany<T>(request.DocumentIds, (Func<MultiGetOperationDescriptor<T>, string, IMultiGetOperation>) ((x, y) => (IMultiGetOperation) x.Index((IndexName) request.IndexIdentity.Name).Routing(request.RoutingId))));
        IDictionary<string, T> dictionary = (IDictionary<string, T>) new Dictionary<string, T>();
        foreach (string documentId in request.DocumentIds)
        {
          MultiGetHit<T> multiGetHit = multiGetResponse.Get<T>(documentId);
          if (multiGetHit.Found)
          {
            T source = multiGetHit.Source;
            source.CurrentDocumentVersion = multiGetHit.Version;
            dictionary.Add(documentId, source);
          }
        }
        documents = new EntitySearchGetDocumentsResponse<T>();
        documents.Documents = dictionary;
        return documents;
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, request.IndexIdentity);
      }
      return documents;
    }

    internal CreateIndexResponse CreateIndex(ICreateIndexRequest request)
    {
      CreateIndexResponse response = (CreateIndexResponse) null;
      try
      {
        response = this.m_elasticSearchClient.Indices.Create(request);
        response.ThrowOnInvalidOrFailedResponse();
      }
      catch (Exception ex) when (!(ex is SearchPlatformException))
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, IndexIdentity.CreateIndexIdentity(request.Index.Name));
      }
      return response;
    }

    internal ExistsResponse IndexExists(IndexIdentity indexIdentity)
    {
      ExistsResponse existsResponse = (ExistsResponse) null;
      try
      {
        existsResponse = this.m_elasticSearchClient.Indices.Exists((Indices) Indices.Index(indexIdentity.Name));
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, indexIdentity);
      }
      return existsResponse;
    }

    internal DeleteIndexResponse DeleteIndex(IndexIdentity indexIdentity)
    {
      DeleteIndexResponse deleteIndexResponse = (DeleteIndexResponse) null;
      try
      {
        deleteIndexResponse = this.m_elasticSearchClient.Indices.Delete((Indices) Indices.Index(indexIdentity.Name));
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, indexIdentity);
      }
      return deleteIndexResponse;
    }

    internal CloseIndexResponse CloseIndex(IndexIdentity indexIdentity)
    {
      CloseIndexResponse closeIndexResponse = (CloseIndexResponse) null;
      try
      {
        closeIndexResponse = this.m_elasticSearchClient.Indices.Close((Indices) Indices.Index(indexIdentity.Name));
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, indexIdentity);
      }
      return closeIndexResponse;
    }

    internal OpenIndexResponse OpenIndex(IndexIdentity indexIdentity)
    {
      OpenIndexResponse openIndexResponse = (OpenIndexResponse) null;
      try
      {
        openIndexResponse = this.m_elasticSearchClient.Indices.Open((Indices) Indices.Index(indexIdentity.Name));
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, indexIdentity);
      }
      return openIndexResponse;
    }

    internal BulkAliasResponse CreateAlias(IBulkAliasRequest aliasRequest)
    {
      BulkAliasResponse alias = (BulkAliasResponse) null;
      try
      {
        alias = this.m_elasticSearchClient.Indices.BulkAlias(aliasRequest);
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      return alias;
    }

    internal BulkAliasResponse RemoveAlias(IndexIdentity indexIdentity, AliasIdentity aliasIdentity)
    {
      BulkAliasResponse bulkAliasResponse = (BulkAliasResponse) null;
      try
      {
        bulkAliasResponse = this.m_elasticSearchClient.Indices.BulkAlias((Func<BulkAliasDescriptor, IBulkAliasRequest>) (a => (IBulkAliasRequest) a.Remove((Func<AliasRemoveDescriptor, IAliasRemoveAction>) (i => (IAliasRemoveAction) i.Index(indexIdentity.Name).Alias(aliasIdentity.Name)))));
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex, indexIdentity);
      }
      return bulkAliasResponse;
    }

    internal GetAliasResponse GetAlias(string aliasIdentity)
    {
      GetAliasResponse response = (GetAliasResponse) null;
      try
      {
        response = this.m_elasticSearchClient.Indices.GetAlias((Indices) aliasIdentity, (Func<GetAliasDescriptor, IGetAliasRequest>) (a => (IGetAliasRequest) a.Name((Names) aliasIdentity)));
        int? httpStatusCode = response.ApiCall.HttpStatusCode;
        int num = 404;
        if (httpStatusCode.GetValueOrDefault() == num & httpStatusCode.HasValue)
          return (GetAliasResponse) null;
        response.ThrowOnInvalidOrFailedResponse();
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      return response;
    }

    internal RefreshResponse Refresh(IndexIdentity indexIdentity)
    {
      RefreshResponse response = (RefreshResponse) null;
      try
      {
        response = this.m_elasticSearchClient.Indices.Refresh((Indices) Indices.Index(indexIdentity.Name));
        response.ThrowOnInvalidOrFailedResponse();
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      return response;
    }

    internal IEnumerable<T> BulkGetByQuery<T>(
      ExecutionContext executionContext,
      BulkGetByQueryRequest bulkGetByQueryRequest)
      where T : AbstractSearchDocumentContract
    {
      IVssRequestContext requestContext = executionContext.RequestContext;
      string indexName = bulkGetByQueryRequest.IndexName;
      int contractType = (int) bulkGetByQueryRequest.ContractType;
      IExpression query = bulkGetByQueryRequest.Query;
      int maxScrollSize = bulkGetByQueryRequest.MaxScrollSize;
      TimeSpan scrollTime = bulkGetByQueryRequest.ScrollTime;
      IEnumerable<string> fields = bulkGetByQueryRequest.Fields;
      string[] routing = new string[1]
      {
        bulkGetByQueryRequest.Routing
      };
      foreach (List<ElasticSearchClientWrapper.ScrollResponse<T>> andScrollDocument in this.ScanAndScrollDocuments<T>(requestContext, indexName, (DocumentContractType) contractType, query, maxScrollSize, scrollTime, fields, routing))
      {
        foreach (ElasticSearchClientWrapper.ScrollResponse<T> scrollResponse in andScrollDocument)
        {
          scrollResponse.Doc.DocumentId = scrollResponse.DocumentId;
          scrollResponse.Doc.Routing = scrollResponse.RoutingId;
          yield return scrollResponse.Doc;
        }
      }
    }

    internal IndexOperationsResponse BulkUpdateByQuery(
      ExecutionContext executionContext,
      BulkUpdateByQueryRequest request)
    {
      IndexOperationsResponse response = new IndexOperationsResponse()
      {
        Success = true,
        ItemsCount = 0,
        FailedItemsCount = 0,
        IsOperationIncomplete = false
      };
      this.UpdateIndexedTimeStampProperty(request.UpdatedPartialAbstractSearchDocument);
      try
      {
        IEnumerable<List<ElasticSearchClientWrapper.ScrollResponse<object>>> scrollResponseLists = this.ScanAndScrollDocuments<object>(executionContext.RequestContext, request.IndexName, request.ContractType, request.Query, 1000, TimeSpan.FromMinutes(5.0), routing: new string[1]
        {
          request.Routing
        });
        Stopwatch timer = Stopwatch.StartNew();
        foreach (List<ElasticSearchClientWrapper.ScrollResponse<object>> scrollResponseList in scrollResponseLists)
        {
          if (this.m_executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment && this.IsOperationTimedOut(timer, request.RequestTimeOut))
          {
            response.IsOperationIncomplete = true;
            break;
          }
          BulkDescriptor descriptor = new BulkDescriptor().Index((IndexName) request.IndexName);
          foreach (ElasticSearchClientWrapper.ScrollResponse<object> scrollResponse in scrollResponseList)
          {
            ElasticSearchClientWrapper.ScrollResponse<object> hit = scrollResponse;
            descriptor.Update<object>((Func<BulkUpdateDescriptor<object, object>, IBulkUpdateOperation<object, object>>) (u => (IBulkUpdateOperation<object, object>) u.Id((Id) hit.DocumentId).Routing((Routing) hit.RoutingId).Doc((object) request.UpdatedPartialAbstractSearchDocument)));
          }
          this.InvokeBulkApiAndUpdateResponse((IBulkRequest) descriptor, response);
        }
        timer.Stop();
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      return response.FailedItemsCount <= 0L ? response : throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Bulk update failed. Number of documents [{0}] out of [{1}] failed.", (object) response.FailedItemsCount, (object) response.ItemsCount)), SearchServiceErrorCode.ElasticsearchBulkUpdateFailed);
    }

    internal virtual bool IsOperationTimedOut(Stopwatch timer, TimeSpan operationTimeLimit) => timer.Elapsed > operationTimeLimit;

    internal SearchShardsResponse SearchShards(SearchShardsRequest searchShardsRequest)
    {
      SearchShardsResponse searchShardsResponse = (SearchShardsResponse) null;
      try
      {
        searchShardsResponse = this.m_elasticSearchClient.SearchShards((ISearchShardsRequest) searchShardsRequest);
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      return searchShardsResponse;
    }

    private IEnumerable<List<ElasticSearchClientWrapper.ScrollResponse<T>>> ScanAndScrollDocuments<T>(
      IVssRequestContext requestContext,
      string indexName,
      DocumentContractType contractType,
      IExpression query,
      int maxScrollSize,
      TimeSpan scrollTime,
      IEnumerable<string> fields = null,
      string[] routing = null,
      bool throwIfIndexDoesNotExist = true)
      where T : class
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (string.IsNullOrWhiteSpace(indexName))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Index name is invalid")), nameof (indexName));
      if (query == null)
        throw new ArgumentNullException(nameof (query));
      if (maxScrollSize < 1 || maxScrollSize > 50000)
        throw new ArgumentOutOfRangeException(nameof (maxScrollSize), (object) maxScrollSize, "Number of documents to scroll is invalid. It should in the range [1, 50000].");
      if (scrollTime < TimeSpan.FromSeconds(1.0) || scrollTime > TimeSpan.FromMinutes(10.0))
        throw new ArgumentOutOfRangeException(nameof (scrollTime), (object) scrollTime, "Invalid scroll time. It should be between 1s and 10 minutes.");
      ExistsResponse response1 = this.IndexExists(IndexIdentity.CreateIndexIdentity(indexName));
      response1.ThrowOnInvalidOrFailedResponse(true);
      if (!response1.Exists)
      {
        if (throwIfIndexDoesNotExist)
          throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Index [{0}] does not exist.", (object) indexName)), SearchServiceErrorCode.IndexNotExists);
      }
      else
      {
        this.Refresh(IndexIdentity.CreateIndexIdentity(indexName));
        CountDescriptor<T> request1 = new CountDescriptor<T>().Index((Indices) indexName).Query((Func<QueryContainerDescriptor<T>, QueryContainer>) (q => query.ToTermLevelQuery()));
        if (routing != null && ((IEnumerable<string>) routing).All<string>((Func<string, bool>) (r => !string.IsNullOrWhiteSpace(r))))
        {
          string str = string.Join(",", routing);
          if (!string.IsNullOrWhiteSpace(str))
            request1 = request1.Routing((Routing) str);
        }
        CountResponse response2 = this.m_elasticSearchClient.Count((ICountRequest) request1);
        response2.ThrowOnInvalidOrFailedResponse();
        long numberOfDocumentsToFetch = response2.Count;
        if (numberOfDocumentsToFetch != 0L)
        {
          string scroll = FormattableString.Invariant(FormattableStringFactory.Create("{0}s", (object) (int) scrollTime.TotalSeconds));
          SearchDescriptor<T> searchDescriptor = new SearchDescriptor<T>().Index((Indices) indexName).Size(new int?(maxScrollSize)).Query((Func<QueryContainerDescriptor<T>, QueryContainer>) (q => query.ToTermLevelQuery())).Scroll((Time) scroll);
          SearchDescriptor<T> request2;
          if (AbstractSearchDocumentContract.CreateContract(contractType).IsSourceEnabled(requestContext))
          {
            List<string> fieldsToGet = new List<string>();
            if (fields != null)
              fieldsToGet.AddRange(fields);
            fieldsToGet.Add("_routing");
            request2 = searchDescriptor.Source((Func<SourceFilterDescriptor<T>, ISourceFilter>) (s => (ISourceFilter) s.Includes((Func<FieldsDescriptor<T>, IPromise<Fields>>) (i => (IPromise<Fields>) i.Fields(fieldsToGet.ToArray())))));
          }
          else
            request2 = searchDescriptor.StoredFields((Fields) new Field("_routing"));
          if (routing != null && ((IEnumerable<string>) routing).All<string>((Func<string, bool>) (r => !string.IsNullOrWhiteSpace(r))))
            request2.Routing((Routing) routing);
          int numPage = 1;
          ISearchResponse<T> searchResponse;
          using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Search Engine", "Query"))
          {
            searchResponse = this.m_elasticSearchClient.Search<T>((ISearchRequest) request2);
            timedCiEvent["Page"] = (object) numPage;
            timedCiEvent["NumHits"] = (object) searchResponse?.Hits?.Count;
          }
          searchResponse.ThrowOnInvalidOrFailedResponse();
          string scrollId = (string) null;
          try
          {
            List<ElasticSearchClientWrapper.ScrollResponse<T>> documentsScrolled = new List<ElasticSearchClientWrapper.ScrollResponse<T>>();
            while (true)
            {
              IEnumerable<IHit<T>> hits = (IEnumerable<IHit<T>>) searchResponse.Hits;
              if (hits.Any<IHit<T>>())
              {
                documentsScrolled.Clear();
                foreach (IHit<T> hit in hits)
                  documentsScrolled.Add(new ElasticSearchClientWrapper.ScrollResponse<T>()
                  {
                    DocumentId = hit.Id,
                    RoutingId = hit.Routing,
                    Doc = hit.Source
                  });
                numberOfDocumentsToFetch -= (long) documentsScrolled.Count;
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1082286, "Search Engine", "Search Engine", FormattableString.Invariant(FormattableStringFactory.Create("Documents pending scroll = {0}.", (object) numberOfDocumentsToFetch)));
                yield return documentsScrolled;
                ElasticSearchClientWrapper.ValidateScrollResponse<T>(searchResponse);
                scrollId = searchResponse.ScrollId;
                ++numPage;
                using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Search Engine", nameof (ScanAndScrollDocuments)))
                {
                  searchResponse = this.m_elasticSearchClient.Scroll<T>((Time) scrollTime, scrollId);
                  timedCiEvent["Page"] = (object) numPage;
                  timedCiEvent["NumHits"] = (object) searchResponse.Hits.Count;
                }
                searchResponse.ThrowOnInvalidOrFailedResponse();
              }
              else
                break;
            }
            documentsScrolled = (List<ElasticSearchClientWrapper.ScrollResponse<T>>) null;
          }
          finally
          {
            if (numberOfDocumentsToFetch > 0L)
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Search Engine", "Search Engine", (IDictionary<string, object>) new Dictionary<string, object>()
              {
                ["ESDocsPendingScroll"] = (object) numberOfDocumentsToFetch
              }, true);
            if (!string.IsNullOrWhiteSpace(scrollId))
              this.m_elasticSearchClient.ClearScroll((Func<ClearScrollDescriptor, IClearScrollRequest>) (d => (IClearScrollRequest) d.ScrollId(scrollId)));
          }
        }
      }
    }

    internal bool SetIndexSetting(string indexName, string settingName, object value)
    {
      try
      {
        UpdateIndexSettingsResponse settingsResponse = this.m_elasticSearchClient.Indices.UpdateSettings((IUpdateIndexSettingsRequest) new UpdateIndexSettingsRequest((Indices) Indices.Index(indexName))
        {
          IndexSettings = (IDynamicIndexSettings) new DynamicIndexSettings((IDictionary<string, object>) new FriendlyDictionary<string, object>()
          {
            [settingName] = value
          })
        });
        return settingsResponse.IsValid ? settingsResponse.IsValid : throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Failed to update setting [{0}] to value [{1}] on index [{2}] with error [{3}].", (object) settingName, value, (object) indexName, (object) settingsResponse.DebugInformation)));
      }
      catch (Exception ex) when (!(ex is SearchPlatformException))
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
        return false;
      }
    }

    internal CatResponse<CatIndicesRecord> GetIndices(List<string> indices = null)
    {
      CatResponse<CatIndicesRecord> response = (CatResponse<CatIndicesRecord>) null;
      try
      {
        CatIndicesDescriptor catIndicesDescriptor = new CatIndicesDescriptor();
        if (indices != null && indices.Any<string>())
          catIndicesDescriptor.Index((Indices) string.Join(",", (IEnumerable<string>) indices));
        response = this.m_elasticSearchClient.Cat.Indices((Func<CatIndicesDescriptor, ICatIndicesRequest>) (s => (ICatIndicesRequest) catIndicesDescriptor));
        response.ThrowOnInvalidOrFailedResponse();
      }
      catch (Exception ex)
      {
        this.m_elasticSearchClient.WrapAndThrowException(ex);
      }
      return response;
    }

    internal void InvokeBulkApiAndUpdateResponse(
      IBulkRequest descriptor,
      IndexOperationsResponse response,
      int documentCountInRequest = -1)
    {
      try
      {
        this.SemaphoreWait();
        BulkResponse response1 = this.m_elasticSearchClient.Bulk(descriptor);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1082298, "Search Engine", "Search Engine", new Func<string>(((IResponseExtensions) response1).SerializeRequestAndResponse));
        response1.ThrowOnInvalidOrFailedResponse();
        if (response1.Errors)
        {
          response.FailedItemsCount += (long) response1.ItemsWithErrors.Count<BulkResponseItemBase>();
          foreach (BulkResponseItemBase itemsWithError in response1.ItemsWithErrors)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceAlways(1082229, TraceLevel.Warning, "Search Engine", "Search Engine", FormattableString.Invariant(FormattableStringFactory.Create("Item failed in bulk request: [Operation: {0}, Index: {1}, Type: {2}, Id: {3}, Error: {4}]", (object) itemsWithError.Operation, (object) itemsWithError.Index, (object) itemsWithError.Type, (object) itemsWithError.Id, (object) itemsWithError.Error)));
        }
        response.ItemsCount += documentCountInRequest == -1 ? (long) response1.Items.Count<BulkResponseItemBase>() : (long) documentCountInRequest - response.FailedItemsCount;
      }
      finally
      {
        this.SemaphoreRelease();
      }
    }

    internal string ScriptParams<T>(BulkScriptUpdateRequest<T> request) where T : AbstractSearchDocumentContract
    {
      StringBuilder stringBuilder = new StringBuilder();
      FluentDictionary<string, object> fluentDictionary = (FluentDictionary<string, object>) null;
      if (request != null)
      {
        stringBuilder.Append("Routing - " + request?.Routing + ", ");
        stringBuilder.Append("Script - " + request?.ScriptName + ", ");
        if (request.Batch != null && request.Batch.Any<T>())
        {
          AbstractSearchDocumentContract documentContract = (AbstractSearchDocumentContract) request.Batch.FirstOrDefault<T>();
          if (documentContract != null)
          {
            try
            {
              fluentDictionary = request.GetParams(documentContract);
            }
            catch (Exception ex)
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082632, "Search Engine", "Search Engine", FormattableString.Invariant(FormattableStringFactory.Create("Get params failed with exception: {0}", (object) ex.ToString())));
            }
            if (fluentDictionary != null)
            {
              stringBuilder.Append("ScriptParams - ");
              stringBuilder.Append(JsonConvert.SerializeObject((object) fluentDictionary));
            }
          }
        }
      }
      return stringBuilder.ToString();
    }

    private static void ValidateScrollResponse<T>(ISearchResponse<T> response) where T : class
    {
      if (string.IsNullOrWhiteSpace(response.ScrollId))
        throw new SearchPlatformException("Invalid scroll_id received.");
    }

    private FluentDictionary<string, object> AddIndexedTimeStampProperty(
      FluentDictionary<string, object> requestParams)
    {
      if (requestParams == null)
        requestParams = new FluentDictionary<string, object>();
      requestParams.Add("indexedTimeStamp", (object) DateUtils.GetUnixTime(DateTime.UtcNow));
      return requestParams;
    }

    private AbstractSearchDocumentContract UpdateIndexedTimeStampProperty(
      AbstractSearchDocumentContract partialAbstractSearchDocument)
    {
      partialAbstractSearchDocument.IndexedTimeStamp = DateUtils.GetUnixTime(DateTime.UtcNow);
      return partialAbstractSearchDocument;
    }

    private class ScrollResponse<T>
    {
      public string DocumentId { get; set; }

      public string RoutingId { get; set; }

      public T Doc { get; set; }
    }
  }
}
