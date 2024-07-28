// Decompiled with JetBrains decompiler
// Type: Nest.ReindexObservable`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Nest
{
  public class ReindexObservable<TSource, TTarget> : IDisposable, IObservable<BulkAllResponse>
    where TSource : class
    where TTarget : class
  {
    private readonly IElasticClient _client;
    private readonly CancellationToken _compositeCancelToken;
    private readonly CancellationTokenSource _compositeCancelTokenSource;
    private readonly IConnectionSettingsValues _connectionSettings;
    private readonly IReindexRequest<TSource, TTarget> _reindexRequest;
    private Action<long> _incrementSeenDocuments = (Action<long>) (l => { });
    private Action _incrementSeenScrollOperations = (Action) (() => { });

    public ReindexObservable(
      IElasticClient client,
      IConnectionSettingsValues connectionSettings,
      IReindexRequest<TSource, TTarget> reindexRequest,
      CancellationToken cancellationToken)
    {
      this._connectionSettings = connectionSettings;
      this._reindexRequest = reindexRequest;
      this._client = client;
      this._compositeCancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      this._compositeCancelToken = this._compositeCancelTokenSource.Token;
    }

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
      this.IsDisposed = true;
      this._compositeCancelTokenSource?.Cancel();
    }

    public IDisposable Subscribe(IObserver<BulkAllResponse> observer)
    {
      observer.ThrowIfNull<IObserver<BulkAllResponse>>(nameof (observer));
      try
      {
        this.Reindex(observer);
      }
      catch (Exception ex)
      {
        observer.OnError(ex);
      }
      return (IDisposable) this;
    }

    public IDisposable Subscribe(ReindexObserver observer)
    {
      this._incrementSeenDocuments = new Action<long>(observer.IncrementSeenScrollDocuments);
      this._incrementSeenScrollOperations = new Action(observer.IncrementSeenScrollOperations);
      return this.Subscribe((IObserver<BulkAllResponse>) observer);
    }

    private void Reindex(IObserver<BulkAllResponse> observer)
    {
      Func<IEnumerable<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>> bulkAll = this._reindexRequest.BulkAll;
      IBulkAllRequest<IHitMetadata<TTarget>> bulkMeta = bulkAll != null ? bulkAll(Enumerable.Empty<IHitMetadata<TTarget>>()) : (IBulkAllRequest<IHitMetadata<TTarget>>) null;
      IScrollAllRequest scrollAll = this._reindexRequest.ScrollAll;
      string toIndex = bulkMeta != null ? bulkMeta.Index.Resolve(this._connectionSettings) : (string) null;
      int index = this.CreateIndex(toIndex, scrollAll);
      ProducerConsumerBackPressure backPressure = this.CreateBackPressure(bulkMeta, scrollAll, index);
      BulkAllObservable<IHitMetadata<TTarget>> bulkAllObservable = this.BulkAll(this.ScrollAll(index, backPressure).SelectMany<IScrollAllResponse<TSource>, IHit<TSource>>((Func<IScrollAllResponse<TSource>, IEnumerable<IHit<TSource>>>) (r => (IEnumerable<IHit<TSource>>) r.SearchResponse.Hits)).Select<IHit<TSource>, IHitMetadata<TTarget>>((Func<IHit<TSource>, IHitMetadata<TTarget>>) (r => r.Copy<TSource, TTarget>(this._reindexRequest.Map))), backPressure, toIndex);
      if (observer is BulkAllObserver observer1)
        bulkAllObservable.Subscribe(observer1);
      else
        bulkAllObservable.Subscribe(observer);
    }

    private BulkAllObservable<IHitMetadata<TTarget>> BulkAll(
      IEnumerable<IHitMetadata<TTarget>> scrollDocuments,
      ProducerConsumerBackPressure backPressure,
      string toIndex)
    {
      Func<IEnumerable<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>> bulkAll = this._reindexRequest.BulkAll;
      IBulkAllRequest<IHitMetadata<TTarget>> request = bulkAll != null ? bulkAll(scrollDocuments) : (IBulkAllRequest<IHitMetadata<TTarget>>) null;
      if (request == null)
        throw new Exception("BulkAll must set on ReindexRequest in order to get the target of a Reindex operation");
      request.BackPressure = backPressure;
      request.BufferToBulk = (Action<BulkDescriptor, IList<IHitMetadata<TTarget>>>) ((bulk, hits) =>
      {
        foreach (IHitMetadata<TTarget> hit in (IEnumerable<IHitMetadata<TTarget>>) hits)
        {
          BulkIndexOperation<TTarget> operation = new BulkIndexOperation<TTarget>(hit.Source)
          {
            Index = (IndexName) toIndex,
            Id = (Id) hit.Id,
            Routing = (Routing) hit.Routing
          };
          bulk.AddOperation((IBulkOperation) operation);
        }
      });
      if (request is IHelperCallable helperCallable)
      {
        RequestMetaData requestMetaData = RequestMetaDataFactory.ReindexHelperRequestMetaData();
        helperCallable.ParentMetaData = requestMetaData;
      }
      return this._client.BulkAll<IHitMetadata<TTarget>>(request, this._compositeCancelToken);
    }

    private ProducerConsumerBackPressure CreateBackPressure(
      IBulkAllRequest<IHitMetadata<TTarget>> bulkMeta,
      IScrollAllRequest scrollAll,
      int slices)
    {
      int? nullable = this._reindexRequest.BackPressureFactor;
      int num1 = nullable ?? CoordinatedRequestDefaults.ReindexBackPressureFactor;
      nullable = (int?) bulkMeta?.MaxDegreeOfParallelism;
      int val1 = nullable ?? CoordinatedRequestDefaults.BulkAllMaxDegreeOfParallelismDefault;
      nullable = (int?) scrollAll?.MaxDegreeOfParallelism;
      int val2 = nullable ?? slices;
      int maxConcurrency = Math.Min(val1, val2);
      nullable = (int?) bulkMeta?.Size;
      int num2 = nullable ?? CoordinatedRequestDefaults.BulkAllSizeDefault;
      nullable = (int?) scrollAll?.Search?.Size;
      int num3 = nullable ?? 10;
      int num4 = num3 * maxConcurrency * num1;
      if (num4 < num2)
        throw new Exception("The back pressure settings are too conservative in providing enough documents for a single bulk operation. " + string.Format("searchSize:{0} * maxConcurrency:{1} * backPressureFactor:{2} = {3}", (object) num3, (object) maxConcurrency, (object) num1, (object) num4) + string.Format(" which is smaller then the bulkSize:{0}.", (object) num2));
      if (num4 == num2)
        throw new Exception("The back pressure settings are too conservative. They provide enough documents for a single bulk but not enough room to advance " + string.Format("searchSize:{0} * maxConcurrency:{1} * backPressureFactor:{2} = {3}", (object) num3, (object) maxConcurrency, (object) num1, (object) num4) + string.Format(" which is exactly the bulkSize:{0}. Increase the BulkAll max concurrency or the backPressureFactor", (object) num2));
      return new ProducerConsumerBackPressure(new int?(num1), maxConcurrency);
    }

    private IEnumerable<IScrollAllResponse<TSource>> ScrollAll(
      int slices,
      ProducerConsumerBackPressure backPressure)
    {
      IScrollAllRequest scrollAll = this._reindexRequest.ScrollAll;
      Time scrollTime = this._reindexRequest.ScrollAll?.ScrollTime;
      if ((object) scrollTime == null)
        scrollTime = (Time) TimeSpan.FromMinutes(2.0);
      return new GetEnumerator<IScrollAllResponse<TSource>>().ToEnumerable((IObservable<IScrollAllResponse<TSource>>) this._client.ScrollAll<TSource>((IScrollAllRequest) new ScrollAllRequest(scrollTime, slices)
      {
        RoutingField = scrollAll.RoutingField,
        MaxDegreeOfParallelism = new int?(scrollAll.MaxDegreeOfParallelism ?? slices),
        Search = scrollAll.Search,
        BackPressure = backPressure,
        ParentMetaData = RequestMetaDataFactory.ReindexHelperRequestMetaData()
      }, this._compositeCancelToken)).Select<IScrollAllResponse<TSource>, IScrollAllResponse<TSource>>(new Func<IScrollAllResponse<TSource>, IScrollAllResponse<TSource>>(this.ObserveScrollAllResponse));
    }

    private IScrollAllResponse<TSource> ObserveScrollAllResponse(
      IScrollAllResponse<TSource> response)
    {
      this._incrementSeenScrollOperations();
      this._incrementSeenDocuments((long) response.SearchResponse.Hits.Count);
      return response;
    }

    private static ElasticsearchClientException Throw(string message, IApiCallDetails details) => new ElasticsearchClientException(PipelineFailure.BadResponse, message, details);

    private int CreateIndex(string toIndex, IScrollAllRequest scrollAll)
    {
      Indices indices = this._reindexRequest.ScrollAll?.Search?.Index;
      if ((object) indices == null)
        indices = Infer.Indices<TSource>();
      Indices fromIndices = indices;
      int? nullable1 = !string.IsNullOrEmpty(toIndex) ? this.CreateIndexIfNeeded(fromIndices, toIndex) : throw new Exception("Could not resolve the target index name to reindex to make sure the bulk all operation describes one");
      int? nullable2 = scrollAll != null ? new int?(scrollAll.Slices) : nullable1;
      if (scrollAll != null && scrollAll.Slices < 0 && nullable1.HasValue)
        nullable2 = nullable1;
      else if (scrollAll != null && scrollAll.Slices < 0)
        throw new Exception("Slices is a negative number and no sane default could be inferred from the origin index's number_of_shards");
      return nullable2.HasValue ? nullable2.Value : throw new Exception("Slices is not specified and could not be inferred from the number of shards hint from the source. This could happen if the scroll all points to multiple indices and no slices have been set");
    }

    private int? CreateIndexIfNeeded(Indices fromIndices, string resolvedTo)
    {
      RequestMetaData requestMetaData = RequestMetaDataFactory.ReindexHelperRequestMetaData();
      if (this._reindexRequest.OmitIndexCreation)
        return new int?();
      bool flag = fromIndices.Match<bool>((Func<Indices.AllIndicesMarker, bool>) (a => false), (Func<Indices.ManyIndices, bool>) (m => m.Indices.Count == 1));
      if (this._client.Indices.Exists((Indices) resolvedTo, (Func<IndexExistsDescriptor, IIndexExistsRequest>) (e => (IIndexExistsRequest) e.RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (rc => (IRequestConfiguration) rc.RequestMetaData(requestMetaData))))).Exists)
        return new int?();
      this._compositeCancelToken.ThrowIfCancellationRequested();
      IndexState state = (IndexState) null;
      string str = fromIndices.Resolve(this._connectionSettings);
      CancellationToken compositeCancelToken;
      if (flag)
      {
        GetIndexResponse getIndexResponse = this._client.Indices.Get((Indices) str, (Func<GetIndexDescriptor, IGetIndexRequest>) (i => (IGetIndexRequest) i.RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (rc => (IRequestConfiguration) rc.RequestMetaData(requestMetaData)))));
        compositeCancelToken = this._compositeCancelToken;
        compositeCancelToken.ThrowIfCancellationRequested();
        state = getIndexResponse.Indices[(IndexName) str];
        if (this._reindexRequest.OmitIndexCreation)
          return state.Settings.NumberOfShards;
      }
      ICreateIndexRequest request = this._reindexRequest.CreateIndexRequest ?? (state != null ? (ICreateIndexRequest) new CreateIndexRequest((IndexName) resolvedTo, (IIndexState) state) : (ICreateIndexRequest) new CreateIndexRequest((IndexName) resolvedTo));
      request.RequestParameters.SetRequestMetaData(requestMetaData);
      CreateIndexResponse createIndexResponse = this._client.Indices.Create(request);
      compositeCancelToken = this._compositeCancelToken;
      compositeCancelToken.ThrowIfCancellationRequested();
      if (!createIndexResponse.IsValid)
        throw ReindexObservable<TSource, TTarget>.Throw("Could not create destination index " + resolvedTo + ".", createIndexResponse.ApiCall);
      return request.Settings?.NumberOfShards;
    }
  }
}
