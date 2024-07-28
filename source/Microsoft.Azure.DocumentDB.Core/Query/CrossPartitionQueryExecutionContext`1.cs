// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.CrossPartitionQueryExecutionContext`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Collections.Generic;
using Microsoft.Azure.Documents.Common;
using Microsoft.Azure.Documents.Query.ExecutionComponent;
using Microsoft.Azure.Documents.Query.ParallelQuery;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal abstract class CrossPartitionQueryExecutionContext<T> : 
    DocumentQueryExecutionContextBase,
    IDocumentQueryExecutionComponent,
    IDisposable
  {
    private const double DynamicPageSizeAdjustmentFactor = 1.6;
    private readonly PriorityQueue<DocumentProducerTree<T>> documentProducerForest;
    private readonly Func<DocumentProducerTree<T>, int> fetchPrioirtyFunction;
    private readonly ComparableTaskScheduler comparableTaskScheduler;
    private readonly IEqualityComparer<T> equalityComparer;
    private readonly RequestChargeTracker requestChargeTracker;
    private readonly long actualMaxPageSize;
    private readonly long actualMaxBufferedItemCount;
    private IReadOnlyDictionary<string, QueryMetrics> groupedQueryMetrics;
    private IReadOnlyDictionary<string, List<IClientSideRequestStatistics>> groupedClientSideRequestStatistics;
    private ConcurrentBag<Tuple<string, IClientSideRequestStatistics>> partitionedRequestStatistics;
    private ConcurrentBag<Tuple<string, QueryMetrics>> partitionedQueryMetrics;
    private long totalBufferedItems;
    private long totalResponseLengthBytes;

    protected CrossPartitionQueryExecutionContext(
      DocumentQueryExecutionContextBase.InitParams initParams,
      string rewrittenQuery,
      IComparer<DocumentProducerTree<T>> moveNextComparer,
      Func<DocumentProducerTree<T>, int> fetchPrioirtyFunction,
      IEqualityComparer<T> equalityComparer)
      : base(initParams)
    {
      if (!string.IsNullOrWhiteSpace(rewrittenQuery))
        this.querySpec = new SqlQuerySpec(rewrittenQuery, this.QuerySpec.Parameters);
      if (moveNextComparer == null)
        throw new ArgumentNullException("moveNextComparer can not be null");
      if (fetchPrioirtyFunction == null)
        throw new ArgumentNullException("fetchPrioirtyFunction can not be null");
      if (equalityComparer == null)
        throw new ArgumentNullException("equalityComparer can not be null");
      this.documentProducerForest = new PriorityQueue<DocumentProducerTree<T>>(moveNextComparer, true);
      this.fetchPrioirtyFunction = fetchPrioirtyFunction;
      this.comparableTaskScheduler = new ComparableTaskScheduler(initParams.FeedOptions.MaxDegreeOfParallelism);
      this.equalityComparer = equalityComparer;
      this.requestChargeTracker = new RequestChargeTracker();
      this.partitionedQueryMetrics = new ConcurrentBag<Tuple<string, QueryMetrics>>();
      this.partitionedRequestStatistics = new ConcurrentBag<Tuple<string, IClientSideRequestStatistics>>();
      this.actualMaxPageSize = (long) this.MaxItemCount.GetValueOrDefault(ParallelQueryConfig.GetConfig().ClientInternalMaxItemCount);
      if (this.actualMaxPageSize < 0L)
        throw new OverflowException("actualMaxPageSize should never be less than 0");
      if (this.actualMaxPageSize > (long) int.MaxValue)
        throw new OverflowException("actualMaxPageSize should never be greater than int.MaxValue");
      this.actualMaxBufferedItemCount = !CrossPartitionQueryExecutionContext<T>.IsMaxBufferedItemCountSet(this.MaxBufferedItemCount) ? ParallelQueryConfig.GetConfig().DefaultMaximumBufferSize : (long) this.MaxBufferedItemCount;
      if (this.actualMaxBufferedItemCount < 0L)
        throw new OverflowException("actualMaxBufferedItemCount should never be less than 0");
      if (this.actualMaxBufferedItemCount > (long) int.MaxValue)
        throw new OverflowException("actualMaxBufferedItemCount should never be greater than int.MaxValue");
    }

    public override bool IsDone => !this.HasMoreResults;

    protected int ActualMaxBufferedItemCount => (int) this.actualMaxBufferedItemCount;

    protected int ActualMaxPageSize => (int) this.actualMaxPageSize;

    protected abstract override string ContinuationToken { get; }

    private bool CanPrefetch => this.MaxDegreeOfParallelism != 0;

    private bool HasMoreResults => this.documentProducerForest.Count != 0 && this.CurrentDocumentProducerTree().HasMoreResults;

    private long FreeItemSpace => this.actualMaxBufferedItemCount - Interlocked.Read(ref this.totalBufferedItems);

    public INameValueCollection GetResponseHeaders()
    {
      DictionaryNameValueCollection responseHeaders = new DictionaryNameValueCollection();
      responseHeaders["x-ms-continuation"] = this.ContinuationToken;
      if (this.ContinuationToken == "[]")
        throw new InvalidOperationException("Somehow a document query execution context returned an empty array of continuations.");
      this.SetQueryMetrics();
      this.SetClientSideRequestStatistics();
      IReadOnlyDictionary<string, QueryMetrics> queryMetrics = this.GetQueryMetrics();
      if (queryMetrics != null && queryMetrics.Count != 0)
      {
        QueryMetrics fromIenumerable = QueryMetrics.CreateFromIEnumerable(queryMetrics.Values);
        responseHeaders["x-ms-documentdb-query-metrics"] = fromIenumerable.ToDelimitedString();
        string s = JsonConvert.SerializeObject((object) fromIenumerable.IndexUtilizationInfo);
        responseHeaders["x-ms-cosmos-index-utilization"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
      }
      responseHeaders["x-ms-request-charge"] = this.requestChargeTracker.GetAndResetCharge().ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return (INameValueCollection) responseHeaders;
    }

    public PartitionedClientSideRequestStatistics GetRequestStats()
    {
      PartitionedClientSideRequestStatistics empty = PartitionedClientSideRequestStatistics.CreateEmpty();
      foreach (KeyValuePair<string, List<IClientSideRequestStatistics>> requestStatistic in (IEnumerable<KeyValuePair<string, List<IClientSideRequestStatistics>>>) this.groupedClientSideRequestStatistics)
      {
        string key = requestStatistic.Key;
        foreach (IClientSideRequestStatistics clientSideRequestStatistics in requestStatistic.Value)
          empty.AddClientSideRequestStatisticsToPartition(key, clientSideRequestStatistics);
      }
      return empty;
    }

    public IReadOnlyDictionary<string, QueryMetrics> GetQueryMetrics() => (IReadOnlyDictionary<string, QueryMetrics>) new PartitionedQueryMetrics(this.groupedQueryMetrics);

    public IEnumerable<DocumentProducer<T>> GetActiveDocumentProducers()
    {
      lock (this.documentProducerForest)
      {
        DocumentProducerTree<T> documentProducerTree1 = this.documentProducerForest.Peek().CurrentDocumentProducerTree;
        if (documentProducerTree1.HasMoreResults && !documentProducerTree1.IsActive)
          yield return documentProducerTree1.Root;
        foreach (DocumentProducerTree<T> documentProducerTree2 in this.documentProducerForest)
        {
          foreach (DocumentProducer<T> documentProducer in documentProducerTree2.GetActiveDocumentProducers())
            yield return documentProducer;
        }
      }
    }

    public DocumentProducerTree<T> CurrentDocumentProducerTree() => this.documentProducerForest.Peek();

    public void PushCurrentDocumentProducerTree(DocumentProducerTree<T> documentProducerTree) => this.documentProducerForest.Enqueue(documentProducerTree);

    public DocumentProducerTree<T> PopCurrentDocumentProducerTree() => this.documentProducerForest.Dequeue();

    public override void Dispose() => this.comparableTaskScheduler.Dispose();

    public void Stop() => this.comparableTaskScheduler.Stop();

    public abstract Task<FeedResponse<object>> DrainAsync(int maxElements, CancellationToken token);

    protected async Task InitializeAsync(
      string collectionRid,
      IReadOnlyList<PartitionKeyRange> partitionKeyRanges,
      int initialPageSize,
      SqlQuerySpec querySpecForInit,
      Dictionary<string, string> targetRangeToContinuationMap,
      bool deferFirstPage,
      string filter,
      Func<DocumentProducerTree<T>, Task> filterCallback,
      CancellationToken token)
    {
      CrossPartitionQueryExecutionContext<T> executionContext = this;
      CollectionCache collectionCache = await executionContext.Client.GetCollectionCacheAsync();
      INameValueCollection requestHeaders = await executionContext.CreateCommonHeadersAsync(executionContext.GetFeedOptions((string) null));
      List<DocumentProducerTree<T>> documentProducerTreeList = new List<DocumentProducerTree<T>>();
      foreach (PartitionKeyRange partitionKeyRange in (IEnumerable<PartitionKeyRange>) partitionKeyRanges)
      {
        string rangeToContinuation = targetRangeToContinuationMap == null || !targetRangeToContinuationMap.ContainsKey(partitionKeyRange.Id) ? (string) null : targetRangeToContinuationMap[partitionKeyRange.Id];
        // ISSUE: reference to a compiler-generated field
        DocumentProducerTree<T> documentProducerTree = new DocumentProducerTree<T>(partitionKeyRange, (Func<PartitionKeyRange, string, int, DocumentServiceRequest>) ((pkRange, continuationToken, pageSize) =>
        {
          INameValueCollection requestHeaders1 = requestHeaders.Clone();
          requestHeaders1["x-ms-continuation"] = continuationToken;
          requestHeaders1["x-ms-max-item-count"] = pageSize.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          // ISSUE: reference to a compiler-generated field
          return this.\u003C\u003E4__this.CreateDocumentServiceRequest(requestHeaders1, querySpecForInit, pkRange, collectionRid);
        }), new Func<DocumentServiceRequest, IDocumentClientRetryPolicy, CancellationToken, Task<FeedResponse<T>>>(((DocumentQueryExecutionContextBase) executionContext).ExecuteRequestAsync<T>), (Func<IDocumentClientRetryPolicy>) (() => (IDocumentClientRetryPolicy) new NonRetriableInvalidPartitionExceptionRetryPolicy(collectionCache, this.\u003C\u003E4__this.Client.ResetSessionTokenRetryPolicy.GetRequestPolicy())), new Action<DocumentProducerTree<T>, int, double, QueryMetrics, IClientSideRequestStatistics, long, CancellationToken>(executionContext.OnDocumentProducerTreeCompleteFetching), executionContext.documentProducerForest.Comparer, executionContext.equalityComparer, executionContext.Client, deferFirstPage, collectionRid, (long) initialPageSize, rangeToContinuation);
        documentProducerTree.Filter = filter;
        if (executionContext.CanPrefetch)
          executionContext.TryScheduleFetch(documentProducerTree);
        documentProducerTreeList.Add(documentProducerTree);
      }
      foreach (DocumentProducerTree<T> documentProducerTree in documentProducerTreeList)
      {
        if (!deferFirstPage)
        {
          int num = await documentProducerTree.MoveNextIfNotSplit(token) ? 1 : 0;
        }
        if (filterCallback != null)
          await filterCallback(documentProducerTree);
        if (documentProducerTree.HasMoreResults)
          executionContext.documentProducerForest.Enqueue(documentProducerTree);
      }
    }

    protected int FindTargetRangeAndExtractContinuationTokens<TContinuationToken>(
      List<PartitionKeyRange> partitionKeyRanges,
      IEnumerable<Tuple<TContinuationToken, Range<string>>> suppliedContinuationTokens,
      out Dictionary<string, TContinuationToken> targetRangeToContinuationTokenMap)
    {
      if (partitionKeyRanges == null)
        throw new ArgumentNullException("partitionKeyRanges can not be null.");
      if (partitionKeyRanges.Count < 1)
        throw new ArgumentException("partitionKeyRanges must have atleast one element.");
      foreach (PartitionKeyRange partitionKeyRange in partitionKeyRanges)
      {
        if (partitionKeyRange == null)
          throw new ArgumentException("partitionKeyRanges can not have null elements.");
      }
      if (suppliedContinuationTokens == null)
        throw new ArgumentNullException("suppliedContinuationTokens can not be null.");
      if (suppliedContinuationTokens.Count<Tuple<TContinuationToken, Range<string>>>() < 1)
        throw new ArgumentException("suppliedContinuationTokens must have atleast one element.");
      if (suppliedContinuationTokens.Count<Tuple<TContinuationToken, Range<string>>>() > partitionKeyRanges.Count)
        throw new ArgumentException("suppliedContinuationTokens can not have more elements than partitionKeyRanges.");
      targetRangeToContinuationTokenMap = new Dictionary<string, TContinuationToken>();
      Tuple<TContinuationToken, Range<string>> tuple1 = suppliedContinuationTokens.OrderBy<Tuple<TContinuationToken, Range<string>>, string>((Func<Tuple<TContinuationToken, Range<string>>, string>) (tuple => tuple.Item2.Min)).First<Tuple<TContinuationToken, Range<string>>>();
      TContinuationToken continuationToken1 = tuple1.Item1;
      PartitionKeyRange partitionKeyRange1 = new PartitionKeyRange()
      {
        MinInclusive = tuple1.Item2.Min,
        MaxExclusive = tuple1.Item2.Max
      };
      int continuationTokens = partitionKeyRanges.BinarySearch(partitionKeyRange1, (IComparer<PartitionKeyRange>) Comparer<PartitionKeyRange>.Create((Comparison<PartitionKeyRange>) ((range1, range2) => string.CompareOrdinal(range1.MinInclusive, range2.MinInclusive))));
      if (continuationTokens < 0)
        throw new BadRequestException(string.Format("{0} - Could not find continuation token: {1}", (object) RMResources.InvalidContinuationToken, (object) continuationToken1));
      foreach (Tuple<TContinuationToken, Range<string>> continuationToken2 in suppliedContinuationTokens)
      {
        TContinuationToken continuationToken3 = continuationToken2.Item1;
        Range<string> range = continuationToken2.Item2;
        IEnumerable<PartitionKeyRange> source = (IEnumerable<PartitionKeyRange>) partitionKeyRanges.Where<PartitionKeyRange>((Func<PartitionKeyRange, bool>) (partitionKeyRange => string.CompareOrdinal(range.Min, partitionKeyRange.MinInclusive) <= 0 && string.CompareOrdinal(range.Max, partitionKeyRange.MaxExclusive) >= 0)).OrderBy<PartitionKeyRange, string>((Func<PartitionKeyRange, string>) (partitionKeyRange => partitionKeyRange.MinInclusive));
        if (source.Count<PartitionKeyRange>() == 0)
          throw new BadRequestException(string.Format("{0} - Could not find continuation token: {1}", (object) RMResources.InvalidContinuationToken, (object) continuationToken3));
        string max = range.Max;
        string maxExclusive1 = source.Last<PartitionKeyRange>().MaxExclusive;
        string minInclusive1 = source.Last<PartitionKeyRange>().MinInclusive;
        string maxExclusive2 = source.First<PartitionKeyRange>().MaxExclusive;
        string minInclusive2 = source.First<PartitionKeyRange>().MinInclusive;
        string min = range.Min;
        string str = maxExclusive1;
        if (!(max == str) || string.CompareOrdinal(maxExclusive1, minInclusive1) < 0 || (source.Count<PartitionKeyRange>() == 1 ? 1 : (string.CompareOrdinal(minInclusive1, maxExclusive2) >= 0 ? 1 : 0)) == 0 || string.CompareOrdinal(maxExclusive2, minInclusive2) < 0 || !(minInclusive2 == min))
          throw new BadRequestException(string.Format("{0} - PMax = C2Max > C2Min > C1Max > C1Min = PMin: {1}", (object) RMResources.InvalidContinuationToken, (object) continuationToken3));
        foreach (PartitionKeyRange partitionKeyRange2 in source)
          targetRangeToContinuationTokenMap.Add(partitionKeyRange2.Id, continuationToken3);
      }
      return continuationTokens;
    }

    protected virtual long GetAndResetResponseLengthBytes() => Interlocked.Exchange(ref this.totalResponseLengthBytes, 0L);

    protected virtual long IncrementResponseLengthBytes(long incrementValue) => Interlocked.Add(ref this.totalResponseLengthBytes, incrementValue);

    protected override Task<FeedResponse<object>> ExecuteInternalAsync(CancellationToken token) => throw new NotImplementedException();

    private void SetQueryMetrics() => this.groupedQueryMetrics = (IReadOnlyDictionary<string, QueryMetrics>) Interlocked.Exchange<ConcurrentBag<Tuple<string, QueryMetrics>>>(ref this.partitionedQueryMetrics, new ConcurrentBag<Tuple<string, QueryMetrics>>()).GroupBy<Tuple<string, QueryMetrics>, string, QueryMetrics>((Func<Tuple<string, QueryMetrics>, string>) (tuple => tuple.Item1), (Func<Tuple<string, QueryMetrics>, QueryMetrics>) (tuple => tuple.Item2)).ToDictionary<IGrouping<string, QueryMetrics>, string, QueryMetrics>((Func<IGrouping<string, QueryMetrics>, string>) (group => group.Key), (Func<IGrouping<string, QueryMetrics>, QueryMetrics>) (group => QueryMetrics.CreateFromIEnumerable((IEnumerable<QueryMetrics>) group)));

    private void SetClientSideRequestStatistics() => this.groupedClientSideRequestStatistics = (IReadOnlyDictionary<string, List<IClientSideRequestStatistics>>) Interlocked.Exchange<ConcurrentBag<Tuple<string, IClientSideRequestStatistics>>>(ref this.partitionedRequestStatistics, new ConcurrentBag<Tuple<string, IClientSideRequestStatistics>>()).GroupBy<Tuple<string, IClientSideRequestStatistics>, string, IClientSideRequestStatistics>((Func<Tuple<string, IClientSideRequestStatistics>, string>) (tuple => tuple.Item1), (Func<Tuple<string, IClientSideRequestStatistics>, IClientSideRequestStatistics>) (tuple => tuple.Item2)).ToDictionary<IGrouping<string, IClientSideRequestStatistics>, string, List<IClientSideRequestStatistics>>((Func<IGrouping<string, IClientSideRequestStatistics>, string>) (group => group.Key), (Func<IGrouping<string, IClientSideRequestStatistics>, List<IClientSideRequestStatistics>>) (group => group.ToList<IClientSideRequestStatistics>()));

    private bool TryScheduleFetch(DocumentProducerTree<T> documentProducerTree) => this.comparableTaskScheduler.TryQueueTask((IComparableTask) new CrossPartitionQueryExecutionContext<T>.DocumentProducerTreeComparableTask(documentProducerTree, this.fetchPrioirtyFunction));

    private void OnDocumentProducerTreeCompleteFetching(
      DocumentProducerTree<T> producer,
      int itemsBuffered,
      double resourceUnitUsage,
      QueryMetrics queryMetrics,
      IClientSideRequestStatistics requestStatistics,
      long responseLengthBytes,
      CancellationToken token)
    {
      this.requestChargeTracker.AddCharge(resourceUnitUsage);
      Interlocked.Add(ref this.totalBufferedItems, (long) itemsBuffered);
      this.IncrementResponseLengthBytes(responseLengthBytes);
      this.partitionedQueryMetrics.Add(Tuple.Create<string, QueryMetrics>(producer.PartitionKeyRange.Id, queryMetrics));
      if (requestStatistics != null)
        this.partitionedRequestStatistics.Add(Tuple.Create<string, IClientSideRequestStatistics>(producer.PartitionKeyRange.Id, requestStatistics));
      producer.PageSize = Math.Min((long) ((double) producer.PageSize * 1.6), this.actualMaxPageSize);
      if (!producer.HasMoreBackendResults)
        return;
      long num = Math.Min(producer.PageSize, 4194304L);
      if (!this.CanPrefetch || this.FreeItemSpace <= num)
        return;
      this.TryScheduleFetch(producer);
    }

    private string GetTrace(string message) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, CorrelatedActivityId: {1}, ActivityId: {2} | {3}", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.CorrelatedActivityId, (object) (this.documentProducerForest.Count != 0 ? this.CurrentDocumentProducerTree().ActivityId : Guid.Empty), (object) message);

    private static bool IsMaxBufferedItemCountSet(int maxBufferedItemCount) => maxBufferedItemCount != 0;

    public struct CrossPartitionInitParams
    {
      public CrossPartitionInitParams(
        string collectionRid,
        PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
        List<PartitionKeyRange> partitionKeyRanges,
        int initialPageSize,
        string requestContinuation)
      {
        if (string.IsNullOrWhiteSpace(collectionRid))
          throw new ArgumentException("collectionRid can not be null, empty, or white space.");
        if (partitionedQueryExecutionInfo == null)
          throw new ArgumentNullException("partitionedQueryExecutionInfo can not be null.");
        if (partitionKeyRanges == null)
          throw new ArgumentNullException("partitionKeyRanges can not be null.");
        foreach (PartitionKeyRange partitionKeyRange in partitionKeyRanges)
        {
          if (partitionKeyRange == null)
            throw new ArgumentNullException("partitionKeyRange can not be null.");
        }
        if (initialPageSize <= 0)
          throw new ArgumentOutOfRangeException("initialPageSize must be atleast 1.");
        this.CollectionRid = collectionRid;
        this.PartitionedQueryExecutionInfo = partitionedQueryExecutionInfo;
        this.PartitionKeyRanges = partitionKeyRanges;
        this.InitialPageSize = initialPageSize;
        this.RequestContinuation = requestContinuation;
      }

      public string CollectionRid { get; }

      public PartitionedQueryExecutionInfo PartitionedQueryExecutionInfo { get; }

      public List<PartitionKeyRange> PartitionKeyRanges { get; }

      public int InitialPageSize { get; }

      public string RequestContinuation { get; }
    }

    private sealed class DocumentProducerTreeComparableTask : ComparableTask
    {
      private readonly DocumentProducerTree<T> producer;

      public DocumentProducerTreeComparableTask(
        DocumentProducerTree<T> producer,
        Func<DocumentProducerTree<T>, int> taskPriorityFunction)
        : base(taskPriorityFunction(producer))
      {
        this.producer = producer;
      }

      public override Task StartAsync(CancellationToken token) => this.producer.BufferMoreDocuments(token);

      public override bool Equals(IComparableTask other) => this.Equals(other as CrossPartitionQueryExecutionContext<T>.DocumentProducerTreeComparableTask);

      public override int GetHashCode() => this.producer.PartitionKeyRange.GetHashCode();

      private bool Equals(
        CrossPartitionQueryExecutionContext<T>.DocumentProducerTreeComparableTask other)
      {
        return this.producer.PartitionKeyRange.Equals(other.producer.PartitionKeyRange);
      }
    }
  }
}
