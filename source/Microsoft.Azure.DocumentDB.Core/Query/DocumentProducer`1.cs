// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.DocumentProducer`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class DocumentProducer<T>
  {
    private readonly AsyncCollection<DocumentProducer<T>.TryMonad<FeedResponse<T>>> bufferedPages;
    private readonly SemaphoreSlim fetchSemaphore;
    private readonly Func<PartitionKeyRange, string, int, DocumentServiceRequest> createRequestFunc;
    private readonly Func<DocumentServiceRequest, IDocumentClientRetryPolicy, CancellationToken, Task<FeedResponse<T>>> executeRequestFunc;
    private readonly Func<IDocumentClientRetryPolicy> createRetryPolicyFunc;
    private readonly DocumentProducer<T>.ProduceAsyncCompleteDelegate produceAsyncCompleteCallback;
    private readonly SchedulingStopwatch fetchSchedulingMetrics;
    private readonly FetchExecutionRangeAccumulator fetchExecutionRangeAccumulator;
    private readonly IEqualityComparer<T> equalityComparer;
    private T current;
    private long pageSize;
    private string previousContinuationToken;
    private string currentContinuationToken;
    private string backendContinuationToken;
    private int itemsLeftInCurrentPage;
    private long bufferedItemCount;
    private IEnumerator<T> currentPage;
    private Guid activityId;
    private bool hasStartedFetching;
    private bool hasMoreResults;
    private string filter;
    private bool isAtBeginningOfPage;
    private bool isActive;
    private bool documentProducerHitException;

    public DocumentProducer(
      PartitionKeyRange partitionKeyRange,
      Func<PartitionKeyRange, string, int, DocumentServiceRequest> createRequestFunc,
      Func<DocumentServiceRequest, IDocumentClientRetryPolicy, CancellationToken, Task<FeedResponse<T>>> executeRequestFunc,
      Func<IDocumentClientRetryPolicy> createRetryPolicyFunc,
      DocumentProducer<T>.ProduceAsyncCompleteDelegate produceAsyncCompleteCallback,
      IEqualityComparer<T> equalityComparer,
      long initialPageSize = 50,
      string initialContinuationToken = null)
    {
      this.bufferedPages = new AsyncCollection<DocumentProducer<T>.TryMonad<FeedResponse<T>>>();
      this.fetchSemaphore = new SemaphoreSlim(1, 1);
      if (partitionKeyRange == null)
        throw new ArgumentNullException(nameof (partitionKeyRange));
      if (createRequestFunc == null)
        throw new ArgumentNullException(nameof (createRequestFunc));
      if (executeRequestFunc == null)
        throw new ArgumentNullException(nameof (executeRequestFunc));
      if (createRetryPolicyFunc == null)
        throw new ArgumentNullException(nameof (createRetryPolicyFunc));
      if (produceAsyncCompleteCallback == null)
        throw new ArgumentNullException(nameof (produceAsyncCompleteCallback));
      if (equalityComparer == null)
        throw new ArgumentNullException(nameof (equalityComparer));
      this.PartitionKeyRange = partitionKeyRange;
      this.createRequestFunc = createRequestFunc;
      this.executeRequestFunc = executeRequestFunc;
      this.createRetryPolicyFunc = createRetryPolicyFunc;
      this.produceAsyncCompleteCallback = produceAsyncCompleteCallback;
      this.equalityComparer = equalityComparer;
      this.pageSize = initialPageSize;
      this.currentContinuationToken = initialContinuationToken;
      this.backendContinuationToken = initialContinuationToken;
      this.previousContinuationToken = initialContinuationToken;
      if (!string.IsNullOrEmpty(initialContinuationToken))
      {
        this.hasStartedFetching = true;
        this.isActive = true;
      }
      this.fetchSchedulingMetrics = new SchedulingStopwatch();
      this.fetchSchedulingMetrics.Ready();
      this.fetchExecutionRangeAccumulator = new FetchExecutionRangeAccumulator();
      this.hasMoreResults = true;
    }

    public PartitionKeyRange PartitionKeyRange { get; }

    public string Filter
    {
      get => this.filter;
      set => this.filter = value;
    }

    public string PreviousContinuationToken => this.previousContinuationToken;

    public string BackendContinuationToken => this.backendContinuationToken;

    public bool IsActive => this.isActive;

    public bool IsAtBeginningOfPage => this.isAtBeginningOfPage;

    public bool HasMoreResults => this.hasMoreResults;

    public bool HasMoreBackendResults
    {
      get
      {
        if (!this.hasStartedFetching)
          return true;
        return this.hasStartedFetching && !string.IsNullOrEmpty(this.backendContinuationToken);
      }
    }

    public int ItemsLeftInCurrentPage => this.itemsLeftInCurrentPage;

    public int BufferedItemCount => (int) this.bufferedItemCount;

    public long PageSize
    {
      get => this.pageSize;
      set => this.pageSize = value;
    }

    public Guid ActivityId => this.activityId;

    public T Current => this.current;

    public async Task<bool> MoveNextAsync(CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      T originalCurrent = this.current;
      int num = await this.MoveNextAsyncImplementation(token) ? 1 : 0;
      if (num == 0 || (object) originalCurrent != null && !this.equalityComparer.Equals(originalCurrent, this.current))
        this.isActive = false;
      return num != 0;
    }

    public async Task BufferMoreIfEmpty(CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      if (this.bufferedPages.Count != 0)
        return;
      await this.BufferMoreDocuments(token);
    }

    public async Task BufferMoreDocuments(CancellationToken token)
    {
      DocumentProducer<T> producer = this;
      token.ThrowIfCancellationRequested();
      try
      {
        await producer.fetchSemaphore.WaitAsync();
        if (!producer.HasMoreBackendResults || producer.documentProducerHitException)
          return;
        producer.fetchSchedulingMetrics.Start();
        producer.fetchExecutionRangeAccumulator.BeginFetchRange();
        int num1 = (int) Math.Min(producer.pageSize, (long) int.MaxValue);
        using (DocumentServiceRequest request = producer.createRequestFunc(producer.PartitionKeyRange, producer.backendContinuationToken, num1))
        {
          IDocumentClientRetryPolicy retryPolicy = producer.createRetryPolicyFunc();
          retryPolicy.OnBeforeSendRequest(request);
          int retries = 0;
          Exception exception;
          ShouldRetryResult shouldRetryResult;
          while (true)
          {
            int num2;
            do
            {
              try
              {
                FeedResponse<T> feedResponse = await producer.executeRequestFunc(request, retryPolicy, token);
                producer.fetchExecutionRangeAccumulator.EndFetchRange(producer.PartitionKeyRange.Id, feedResponse.ActivityId, (long) feedResponse.Count, (long) retries);
                producer.fetchSchedulingMetrics.Stop();
                producer.hasStartedFetching = true;
                producer.backendContinuationToken = feedResponse.ResponseContinuation;
                producer.activityId = Guid.Parse(feedResponse.ActivityId);
                await producer.bufferedPages.AddAsync(DocumentProducer<T>.TryMonad<FeedResponse<T>>.FromResult(feedResponse));
                Interlocked.Add(ref producer.bufferedItemCount, (long) feedResponse.Count);
                QueryMetrics queryMetrics = QueryMetrics.Zero;
                if (feedResponse.ResponseHeaders["x-ms-documentdb-query-metrics"] != null)
                  queryMetrics = QueryMetrics.CreateFromDelimitedStringAndClientSideMetrics(feedResponse.ResponseHeaders["x-ms-documentdb-query-metrics"], feedResponse.ResponseHeaders["x-ms-cosmos-index-utilization"], new ClientSideMetrics((long) retries, feedResponse.RequestCharge, producer.fetchExecutionRangeAccumulator.GetExecutionRanges(), (IEnumerable<Tuple<string, SchedulingTimeSpan>>) new List<Tuple<string, SchedulingTimeSpan>>()));
                if (!producer.HasMoreBackendResults)
                  queryMetrics = QueryMetrics.CreateWithSchedulingMetrics(queryMetrics, new List<Tuple<string, SchedulingTimeSpan>>()
                  {
                    new Tuple<string, SchedulingTimeSpan>(producer.PartitionKeyRange.Id, producer.fetchSchedulingMetrics.Elapsed)
                  });
                producer.produceAsyncCompleteCallback(producer, feedResponse.Count, feedResponse.RequestCharge, queryMetrics, feedResponse.RequestStatistics, feedResponse.ResponseLengthBytes, token);
                goto label_21;
              }
              catch (Exception ex)
              {
                num2 = 1;
              }
            }
            while (num2 != 1);
            exception = ex;
            shouldRetryResult = await retryPolicy.ShouldRetryAsync(exception, token);
            if (shouldRetryResult.ShouldRetry)
            {
              await Task.Delay(shouldRetryResult.BackoffTime);
              ++retries;
              exception = (Exception) null;
            }
            else
              break;
          }
          Exception exception1 = shouldRetryResult.ExceptionToThrow == null ? exception : shouldRetryResult.ExceptionToThrow;
          await producer.bufferedPages.AddAsync(DocumentProducer<T>.TryMonad<FeedResponse<T>>.FromException(exception1));
          producer.documentProducerHitException = true;
label_21:
          retryPolicy = (IDocumentClientRetryPolicy) null;
        }
      }
      finally
      {
        producer.fetchSchedulingMetrics.Stop();
        producer.fetchSemaphore.Release();
      }
    }

    public void Shutdown() => this.hasMoreResults = false;

    private async Task<bool> MoveNextAsyncImplementation(CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      if (!this.HasMoreResults)
        return false;
      if (this.MoveNextDocumentWithinCurrentPage())
        return true;
      if (await this.MoveNextPage(token))
        return true;
      this.hasMoreResults = false;
      return false;
    }

    private bool MoveToFirstDocumentInPage()
    {
      if (this.currentPage == null || !this.currentPage.MoveNext())
        return false;
      this.current = this.currentPage.Current;
      this.isAtBeginningOfPage = true;
      return true;
    }

    private bool MoveNextDocumentWithinCurrentPage()
    {
      if (this.currentPage == null)
        return false;
      int num = this.currentPage.MoveNext() ? 1 : 0;
      this.current = this.currentPage.Current;
      this.isAtBeginningOfPage = false;
      Interlocked.Decrement(ref this.bufferedItemCount);
      Interlocked.Decrement(ref this.itemsLeftInCurrentPage);
      return num != 0;
    }

    private async Task<bool> MoveNextPage(CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      if (this.itemsLeftInCurrentPage != 0)
        throw new InvalidOperationException("Tried to move onto the next page before finishing the first page.");
      await this.BufferMoreIfEmpty(token);
      if (this.bufferedPages.Count == 0)
        return false;
      FeedResponse<T> feedResponse = (await this.bufferedPages.TakeAsync(token)).Match<FeedResponse<T>>((Func<FeedResponse<T>, FeedResponse<T>>) (page => page), (Func<ExceptionDispatchInfo, FeedResponse<T>>) (exceptionDispatchInfo =>
      {
        exceptionDispatchInfo.Throw();
        return (FeedResponse<T>) null;
      }));
      this.previousContinuationToken = this.currentContinuationToken;
      this.currentContinuationToken = feedResponse.ResponseContinuation;
      this.currentPage = feedResponse.GetEnumerator();
      this.isAtBeginningOfPage = true;
      this.itemsLeftInCurrentPage = feedResponse.Count;
      if (this.MoveToFirstDocumentInPage())
        return true;
      return this.currentContinuationToken != null && await this.MoveNextPage(token);
    }

    private static async Task<FeedResponse<T>> CreateFaultedTask(Exception exception)
    {
      int num = await Task.FromResult<int>(0);
      throw exception;
    }

    public delegate void ProduceAsyncCompleteDelegate(
      DocumentProducer<T> producer,
      int numberOfDocuments,
      double requestCharge,
      QueryMetrics queryMetrics,
      IClientSideRequestStatistics RequestStatistics,
      long responseLengthInBytes,
      CancellationToken token);

    private struct TryMonad<TResult>
    {
      private readonly TResult result;
      private readonly ExceptionDispatchInfo exceptionDispatchInfo;
      private readonly bool succeeded;

      private TryMonad(TResult result, ExceptionDispatchInfo exceptionDispatchInfo, bool succeeded)
      {
        this.result = result;
        this.exceptionDispatchInfo = exceptionDispatchInfo;
        this.succeeded = succeeded;
      }

      public static DocumentProducer<T>.TryMonad<TResult> FromResult(TResult result) => new DocumentProducer<T>.TryMonad<TResult>(result, (ExceptionDispatchInfo) null, true);

      public static DocumentProducer<T>.TryMonad<TResult> FromException(Exception exception) => new DocumentProducer<T>.TryMonad<TResult>(default (TResult), ExceptionDispatchInfo.Capture(exception), false);

      public TOutput Match<TOutput>(
        Func<TResult, TOutput> onSuccess,
        Func<ExceptionDispatchInfo, TOutput> onError)
      {
        return this.succeeded ? onSuccess(this.result) : onError(this.exceptionDispatchInfo);
      }
    }
  }
}
