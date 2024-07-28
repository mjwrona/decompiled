// Decompiled with JetBrains decompiler
// Type: Nest.BulkAllObservable`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  public class BulkAllObservable<T> : IDisposable, IObservable<BulkAllResponse> where T : class
  {
    private readonly int _backOffRetries;
    private readonly TimeSpan _backOffTime;
    private readonly int _bulkSize;
    private readonly IElasticClient _client;
    private readonly CancellationToken _compositeCancelToken;
    private readonly CancellationTokenSource _compositeCancelTokenSource;
    private readonly Action<BulkResponseItemBase, T> _droppedDocumentCallBack;
    private readonly int _maxDegreeOfParallelism;
    private readonly IBulkAllRequest<T> _partitionedBulkRequest;
    private readonly Func<BulkResponseItemBase, T, bool> _retryPredicate;
    private Action _incrementFailed = (Action) (() => { });
    private Action _incrementRetries = (Action) (() => { });
    private readonly Action<BulkResponse> _bulkResponseCallback;

    public BulkAllObservable(
      IElasticClient client,
      IBulkAllRequest<T> partitionedBulkRequest,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this._client = client;
      this._partitionedBulkRequest = partitionedBulkRequest;
      this._backOffRetries = this._partitionedBulkRequest.BackOffRetries.GetValueOrDefault(CoordinatedRequestDefaults.BulkAllBackOffRetriesDefault);
      this._backOffTime = this._partitionedBulkRequest?.BackOffTime?.ToTimeSpan() ?? CoordinatedRequestDefaults.BulkAllBackOffTimeDefault;
      int? nullable = this._partitionedBulkRequest.Size;
      this._bulkSize = nullable ?? CoordinatedRequestDefaults.BulkAllSizeDefault;
      this._retryPredicate = this._partitionedBulkRequest.RetryDocumentPredicate ?? new Func<BulkResponseItemBase, T, bool>(BulkAllObservable<T>.RetryBulkActionPredicate);
      this._droppedDocumentCallBack = this._partitionedBulkRequest.DroppedDocumentCallback ?? new Action<BulkResponseItemBase, T>(BulkAllObservable<T>.DroppedDocumentCallbackDefault);
      this._bulkResponseCallback = this._partitionedBulkRequest.BulkResponseCallback;
      nullable = this._partitionedBulkRequest.MaxDegreeOfParallelism;
      this._maxDegreeOfParallelism = nullable ?? CoordinatedRequestDefaults.BulkAllMaxDegreeOfParallelismDefault;
      this._compositeCancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      this._compositeCancelToken = this._compositeCancelTokenSource.Token;
    }

    public void Dispose()
    {
      this._compositeCancelTokenSource?.Cancel();
      this._compositeCancelTokenSource?.Dispose();
    }

    public IDisposable Subscribe(IObserver<BulkAllResponse> observer)
    {
      observer.ThrowIfNull<IObserver<BulkAllResponse>>(nameof (observer));
      this.BulkAll(observer);
      return (IDisposable) this;
    }

    public IDisposable Subscribe(BulkAllObserver observer)
    {
      this._incrementFailed = new Action(observer.IncrementTotalNumberOfFailedBuffers);
      this._incrementRetries = new Action(observer.IncrementTotalNumberOfRetries);
      return this.Subscribe((IObserver<BulkAllResponse>) observer);
    }

    private void BulkAll(IObserver<BulkAllResponse> observer) => new PartitionHelper<T>(this._partitionedBulkRequest.Documents, this._bulkSize).ForEachAsync<IList<T>, BulkAllResponse>((Func<IList<T>, long, Task<BulkAllResponse>>) ((buffer, page) => this.BulkAsync(buffer, page, 0)), (Action<IList<T>, BulkAllResponse>) ((buffer, response) => observer.OnNext(response)), (Action<Exception>) (ex => this.OnCompleted(ex, observer)), this._maxDegreeOfParallelism);

    private void OnCompleted(Exception exception, IObserver<BulkAllResponse> observer)
    {
      if (exception != null)
      {
        observer.OnError(exception);
      }
      else
      {
        try
        {
          this.RefreshOnCompleted();
          observer.OnCompleted();
        }
        catch (Exception ex)
        {
          observer.OnError(ex);
        }
      }
    }

    private void RefreshOnCompleted()
    {
      if (!this._partitionedBulkRequest.RefreshOnCompleted)
        return;
      Indices indices = this._partitionedBulkRequest.RefreshIndices;
      if ((object) indices == null)
        indices = (Indices) this._partitionedBulkRequest.Index;
      Indices index = indices;
      if (index == (Indices) null)
        return;
      RefreshResponse refreshResponse = this._client.Indices.Refresh(index, (Func<RefreshDescriptor, IRefreshRequest>) (r => (IRefreshRequest) r.RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (rc =>
      {
        if (this._partitionedBulkRequest is IHelperCallable partitionedBulkRequest2 && partitionedBulkRequest2.ParentMetaData != null)
          rc.RequestMetaData(partitionedBulkRequest2.ParentMetaData);
        else
          rc.RequestMetaData(RequestMetaDataFactory.BulkHelperRequestMetaData());
        return (IRequestConfiguration) rc;
      }))));
      if (!refreshResponse.IsValid)
        throw BulkAllObservable<T>.Throw("Refreshing after all documents have indexed failed", refreshResponse.ApiCall);
    }

    private async Task<BulkAllResponse> BulkAsync(IList<T> buffer, long page, int backOffRetries)
    {
      this._compositeCancelToken.ThrowIfCancellationRequested();
      IBulkAllRequest<T> request = this._partitionedBulkRequest;
      BulkResponse response = await this._client.BulkAsync((Func<BulkDescriptor, IBulkRequest>) (s =>
      {
        s.Index(request.Index);
        s.Timeout(request.Timeout);
        if (request.BufferToBulk != null)
          request.BufferToBulk(s, buffer);
        else
          s.IndexMany<T>((IEnumerable<T>) buffer);
        if (!string.IsNullOrEmpty(request.Pipeline))
          s.Pipeline(request.Pipeline);
        if (request.Routing != (Routing) null)
          s.Routing(request.Routing);
        if (request.WaitForActiveShards.HasValue)
          s.WaitForActiveShards(request.WaitForActiveShards.ToString());
        // ISSUE: reference to a compiler-generated field
        IHelperCallable partitionedBulkRequest = this.\u003C\u003E4__this._partitionedBulkRequest as IHelperCallable;
        if (partitionedBulkRequest != null && partitionedBulkRequest.ParentMetaData != null)
          s.RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (rc => (IRequestConfiguration) rc.RequestMetaData(partitionedBulkRequest.ParentMetaData)));
        else
          s.RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (rc => (IRequestConfiguration) rc.RequestMetaData(RequestMetaDataFactory.BulkHelperRequestMetaData())));
        return (IBulkRequest) s;
      }), this._compositeCancelToken).ConfigureAwait(false);
      this._compositeCancelToken.ThrowIfCancellationRequested();
      Action<BulkResponse> responseCallback = this._bulkResponseCallback;
      if (responseCallback != null)
        responseCallback(response);
      if (!response.ApiCall.Success)
        return await this.HandleBulkRequest(buffer, page, backOffRetries, response).ConfigureAwait(false);
      List<T> retryDocuments = new List<T>();
      List<Tuple<BulkResponseItemBase, T>> droppedDocuments = new List<Tuple<BulkResponseItemBase, T>>();
      foreach (Tuple<BulkResponseItemBase, T> tuple in response.Items.Zip<BulkResponseItemBase, T, Tuple<BulkResponseItemBase, T>>((IEnumerable<T>) buffer, new Func<BulkResponseItemBase, T, Tuple<BulkResponseItemBase, T>>(Tuple.Create<BulkResponseItemBase, T>)))
      {
        if (!tuple.Item1.IsValid)
        {
          if (this._retryPredicate(tuple.Item1, tuple.Item2))
            retryDocuments.Add(tuple.Item2);
          else
            droppedDocuments.Add(tuple);
        }
      }
      this.HandleDroppedDocuments(droppedDocuments, response);
      if (retryDocuments.Count > 0 && backOffRetries < this._backOffRetries)
        return await this.RetryDocuments(page, ++backOffRetries, (IList<T>) retryDocuments).ConfigureAwait(false);
      if (retryDocuments.Count > 0)
        throw this.ThrowOnBadBulk((IElasticsearchResponse) response, string.Format("Bulk indexing failed and after retrying {0} times", (object) backOffRetries));
      request.BackPressure?.Release();
      return new BulkAllResponse()
      {
        Retries = backOffRetries,
        Page = page,
        Items = (IReadOnlyCollection<BulkResponseItemBase>) response.Items
      };
    }

    private void HandleDroppedDocuments(
      List<Tuple<BulkResponseItemBase, T>> droppedDocuments,
      BulkResponse response)
    {
      if (droppedDocuments.Count <= 0)
        return;
      foreach (Tuple<BulkResponseItemBase, T> droppedDocument in droppedDocuments)
        this._droppedDocumentCallBack(droppedDocument.Item1, droppedDocument.Item2);
      if (!this._partitionedBulkRequest.ContinueAfterDroppedDocuments)
        throw this.ThrowOnBadBulk((IElasticsearchResponse) response, "BulkAll halted after receiving failures that can not be retried from _bulk");
    }

    private async Task<BulkAllResponse> HandleBulkRequest(
      IList<T> buffer,
      long page,
      int backOffRetries,
      BulkResponse response)
    {
      PipelineFailure? nullable = response.ApiCall.OriginalException is ElasticsearchClientException originalException ? originalException.FailureReason : new PipelineFailure?();
      string reason = (nullable.HasValue ? nullable.GetValueOrDefault().GetStringValue() : (string) null) ?? "BadRequest";
      if (nullable.HasValue)
      {
        switch (nullable.GetValueOrDefault())
        {
          case PipelineFailure.BadAuthentication:
          case PipelineFailure.SniffFailure:
          case PipelineFailure.CouldNotStartSniffOnStartup:
          case PipelineFailure.Unexpected:
          case PipelineFailure.NoNodesAttempted:
            throw this.ThrowOnBadBulk((IElasticsearchResponse) response, "BulkAll halted after PipelineFailure." + reason + " from _bulk");
          case PipelineFailure.MaxRetriesReached:
            if (response.ApiCall.AuditTrail.Last<Audit>().Event == AuditEvent.FailedOverAllNodes)
              throw this.ThrowOnBadBulk((IElasticsearchResponse) response, "BulkAll halted after attempted bulk failed over all the active nodes");
            ThrowOnExhaustedRetries();
            return await this.RetryDocuments(page, ++backOffRetries, buffer).ConfigureAwait(false);
          case PipelineFailure.FailedProductCheck:
            throw this.ThrowOnBadBulk((IElasticsearchResponse) response, "BulkAll halted after failed product check");
        }
      }
      ThrowOnExhaustedRetries();
      return await this.RetryDocuments(page, ++backOffRetries, buffer).ConfigureAwait(false);

      void ThrowOnExhaustedRetries()
      {
        if (backOffRetries >= this._backOffRetries)
          throw this.ThrowOnBadBulk((IElasticsearchResponse) response, string.Format("{0} halted after {1}.{2} from _bulk and exhausting retries ({3})", (object) "BulkAll", (object) "PipelineFailure", (object) reason, (object) backOffRetries));
      }
    }

    private async Task<BulkAllResponse> RetryDocuments(
      long page,
      int backOffRetries,
      IList<T> retryDocuments)
    {
      this._incrementRetries();
      await Task.Delay(this._backOffTime, this._compositeCancelToken).ConfigureAwait(false);
      return await this.BulkAsync(retryDocuments, page, backOffRetries).ConfigureAwait(false);
    }

    private Exception ThrowOnBadBulk(IElasticsearchResponse response, string message)
    {
      this._incrementFailed();
      this._partitionedBulkRequest.BackPressure?.Release();
      return (Exception) BulkAllObservable<T>.Throw(message, response.ApiCall);
    }

    private static ElasticsearchClientException Throw(string message, IApiCallDetails details) => new ElasticsearchClientException(PipelineFailure.BadResponse, message, details);

    private static bool RetryBulkActionPredicate(BulkResponseItemBase bulkResponseItem, T d) => bulkResponseItem.Status == 429;

    private static void DroppedDocumentCallbackDefault(BulkResponseItemBase bulkResponseItem, T d)
    {
    }
  }
}
