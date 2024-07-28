// Decompiled with JetBrains decompiler
// Type: Nest.ScrollAllObservable`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  public class ScrollAllObservable<T> : IDisposable, IObservable<ScrollAllResponse<T>> where T : class
  {
    private readonly ProducerConsumerBackPressure _backPressure;
    private readonly IElasticClient _client;
    private readonly CancellationToken _compositeCancelToken;
    private readonly CancellationTokenSource _compositeCancelTokenSource;
    private readonly IScrollAllRequest _scrollAllRequest;
    private readonly SemaphoreSlim _scrollInitiationLock = new SemaphoreSlim(1, 1);
    private readonly ISearchRequest _searchRequest;

    public ScrollAllObservable(
      IElasticClient client,
      IScrollAllRequest scrollAllRequest,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this._scrollAllRequest = scrollAllRequest;
      this._searchRequest = scrollAllRequest?.Search ?? (ISearchRequest) new SearchRequest<T>();
      if (this._scrollAllRequest is IHelperCallable scrollAllRequest1 && scrollAllRequest1.ParentMetaData != null)
        this._searchRequest.RequestParameters.SetRequestMetaData(scrollAllRequest1.ParentMetaData);
      else
        this._searchRequest.RequestParameters.SetRequestMetaData(RequestMetaDataFactory.ScrollHelperRequestMetaData());
      if (this._searchRequest.Sort == null)
        this._searchRequest.Sort = FieldSort.ByDocumentOrder;
      this._searchRequest.RequestParameters.Scroll = this._scrollAllRequest.ScrollTime.ToTimeSpan();
      this._client = client;
      this._compositeCancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      this._compositeCancelToken = this._compositeCancelTokenSource.Token;
      this._backPressure = this._scrollAllRequest.BackPressure;
    }

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
      this.IsDisposed = true;
      this._compositeCancelTokenSource?.Cancel();
    }

    public IDisposable Subscribe(IObserver<ScrollAllResponse<T>> observer)
    {
      observer.ThrowIfNull<IObserver<ScrollAllResponse<T>>>(nameof (observer));
      this.ScrollAll(observer);
      return (IDisposable) this;
    }

    private void ScrollAll(IObserver<ScrollAllResponse<T>> observer)
    {
      int slices = this._scrollAllRequest.Slices;
      int maxDegreeOfParallelism = this._scrollAllRequest.MaxDegreeOfParallelism ?? this._scrollAllRequest.Slices;
      Enumerable.Range(0, slices).ForEachAsync<int, bool>((Func<int, long, Task<bool>>) ((slice, l) => this.ScrollSliceAsync(observer, slice)), (Action<int, bool>) ((slice, r) => { }), (Action<Exception>) (t => ScrollAllObservable<T>.OnCompleted(t, observer)), maxDegreeOfParallelism);
    }

    private async Task<bool> ScrollSliceAsync(IObserver<ScrollAllResponse<T>> observer, int slice)
    {
      ISearchResponse<T> searchResult = await this.InitiateSearchAsync(slice).ConfigureAwait(false);
      await this.ScrollToCompletionAsync(slice, observer, searchResult).ConfigureAwait(false);
      return true;
    }

    private static ElasticsearchClientException Throw(string message, IApiCallDetails details) => new ElasticsearchClientException(PipelineFailure.BadResponse, message, details);

    private void ThrowOnBadSearchResult(ISearchResponse<T> result, int slice, int page)
    {
      if (result == null || !result.IsValid)
        throw ScrollAllObservable<T>.Throw(string.Format("scrolling search on {0} with slice {1} was not valid on scroll iteration {2}", (object) (result?.ApiCall.Uri.PathAndQuery ?? "(unknown)"), (object) slice, (object) page), result?.ApiCall);
      this._compositeCancelToken.ThrowIfCancellationRequested();
    }

    private async Task ScrollToCompletionAsync(
      int slice,
      IObserver<ScrollAllResponse<T>> observer,
      ISearchResponse<T> searchResult)
    {
      int page = 0;
      this.ThrowOnBadSearchResult(searchResult, slice, page);
      Time scroll = this._scrollAllRequest.ScrollTime;
      while (searchResult.IsValid)
      {
        if (searchResult.Documents.HasAny<T>())
        {
          if (this._backPressure != null)
            await this._backPressure.WaitAsync(this._compositeCancelToken).ConfigureAwait(false);
          observer.OnNext(new ScrollAllResponse<T>()
          {
            Slice = slice,
            SearchResponse = searchResult,
            Scroll = (long) page
          });
          ++page;
          ScrollRequest request = new ScrollRequest(searchResult.ScrollId, scroll);
          if (request.RequestConfiguration == null)
            request.RequestConfiguration = (IRequestConfiguration) new RequestConfiguration();
          if (this._scrollAllRequest is IHelperCallable scrollAllRequest && scrollAllRequest.ParentMetaData != null)
            request.RequestConfiguration.SetRequestMetaData(scrollAllRequest.ParentMetaData);
          else
            request.RequestConfiguration.SetRequestMetaData(RequestMetaDataFactory.ScrollHelperRequestMetaData());
          searchResult = await this._client.ScrollAsync<T>((IScrollRequest) request, this._compositeCancelToken).ConfigureAwait(false);
          this.ThrowOnBadSearchResult(searchResult, slice, page);
        }
        else
        {
          scroll = (Time) null;
          return;
        }
      }
      scroll = (Time) null;
    }

    private async Task<ISearchResponse<T>> InitiateSearchAsync(int slice)
    {
      await this._scrollInitiationLock.WaitAsync(this._compositeCancelToken).ConfigureAwait(false);
      ISearchResponse<T> searchResponse;
      try
      {
        this._searchRequest.Slice = (ISlicedScroll) new SlicedScroll()
        {
          Id = new int?(slice),
          Max = new int?(this._scrollAllRequest.Slices),
          Field = this._scrollAllRequest.RoutingField
        };
        searchResponse = await this._client.SearchAsync<T>(this._searchRequest, this._compositeCancelToken).ConfigureAwait(false);
      }
      finally
      {
        this._scrollInitiationLock.Release();
      }
      return searchResponse;
    }

    private static void OnCompleted(Exception exception, IObserver<ScrollAllResponse<T>> observer)
    {
      if (exception == null)
        observer.OnCompleted();
      else
        observer.OnError(exception);
    }
  }
}
